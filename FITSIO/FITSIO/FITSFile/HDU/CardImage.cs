using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Najm.FITSIO
{
    internal class CardImage : ICardImage
    {
        internal CardImage()
        {
            _type = CardImageType.Unknown;
        }

        internal void Load(FileStream fs)
        {
            byte[] buffer = new byte[80];
            fs.Read(buffer, 0, 80);
            Update(ASCIIEncoding.ASCII.GetString(buffer));
        }

        internal void Save(FileStream fs)
        {
            
        }

        internal CardImageType Type { get { return _type; } }

        #region ICardImage Members

        public string Key { get { return _key; } set { _key = value; } }
        public string Value { get { return _value; } set { _value = value; } }
        public string Comment { get { return _comment; } set { _comment = value; } }
        public string InfoText { get { return _rawString; } }
        public void  Update(string content)
        {
            // cehck we are not exceeding 89 byte limit
            /////////////////////////////////////////////////////////////////////////////////////////////////
            Utils.CheckBool(content.Length <= 80, "can't exceed card image 80 bytes maximum size");

            // copy the whole thing - used mainly for displaying the content of the card image as is.
            /////////////////////////////////////////////////////////////////////////////////////////////////
            _rawString = content;

            //  do we have a blank cardimage?
            /////////////////////////////////////////////////////////////////////////////////////////////////
            if (string.IsNullOrEmpty(content) || Utils.FindFirstNotOf(content, ' ') == -1 || Utils.FindFirstNotOf(content, '\0') == -1)
            {
                _type = CardImageType.Blank;
            }
            else
            {
                // check if key= value
                if (content.IndexOf("= ") == 8)
                {
                    string key, value, comment;
                    key = content.Substring(0, 8);
                    value = content.Substring(10);
                    // do we have a comment?
                    int commentStart = value.IndexOf("/");
                    if (commentStart != -1)
                    {
                        comment = value.Substring(commentStart + 1);
                        value = value.Remove(commentStart);
                        Update(key, value, comment);
                    }
                    else
                    {
                        Update(key, value);
                    }
                    // watchout! even if it contains '=', if it begins with END, I'll still consider it END CardImage
                    if (key.Trim() == "END")
                    {
                        _type = CardImageType.End;
                    }
                    else
                    {
                        _type = CardImageType.KeyValue;
                    }
                }
                else if(content.StartsWith("COMMENT"))
                {
                    _type = CardImageType.Comment;
                    Update("COMMENT", content.Substring(8));
                }
                else if (content.StartsWith("HISTORY"))
                {
                    _type = CardImageType.History;
                    Update("HISTORY", content.Substring(8));
                }
                else if (content.StartsWith("END"))
                {
                    _type = CardImageType.End;
                    Update("END", content.Substring(8));
                }
            }
        }

        public void Update(string key, string value, string comment)
        {
            Update(key, value);
            _comment = comment.Trim();
        }

        public void  Update(string key, string value)
        {
            // validate key
            /////////////////////////////////////////////////////////////////////////////////////////////////
            ValidateKey(key);

            // now key is valid, use it
            /////////////////////////////////////////////////////////////////////////////////////////////////
            _key = key;

            // must be all Caps, so I'm converting to upper
            /////////////////////////////////////////////////////////////////////////////////////////////////
            _key = _key.ToUpper();
            CleanStringValue(ref _key);

            // validate value
            /////////////////////////////////////////////////////////////////////////////////////////////////
            ValidateValue(value);

            // now value is valid, use it
            /////////////////////////////////////////////////////////////////////////////////////////////////
            _value = value;
            CleanStringValue(ref _value);
        }

        private void ValidateKey(string key)
        {
            // validate length
            Utils.CheckBool(key.Length > 0 && key.Length <= 8, "cardiamge key length is invalid");

            // must not start with spaces
            // commented: I'm easing this requirement as i saw many files don't comply
            // Utils.CheckBool(Utils.FindFirstNotOf(key, ' ') == 0, "cardimage key must not start with spaces");

            // No embedded spaces allowed
            Utils.CheckBool(!key.Trim().Contains(" "), "cardimage key contains embedded spaces");

            // must be alpha, numeric, '_', or '-'
            foreach (char c in key)
	        {
                // some files have '/' as well so i just added it - don't see how could this harm!
                Utils.CheckBool((Char.IsLetterOrDigit(c) || (c == ' ') || (c == '_') || (c == '-') || (c == '/')), "cardimage key has invalid charachters");
	        }

            // must be all Caps
            foreach (char c in key)
            {
                Utils.CheckBool(!char.IsLetter(c) || char.IsUpper(c), "cardimage key case sensitivity check failed");
            }
        }

        private void ValidateValue(string value)
        {
            // validate length
            Utils.CheckBool(value.Length <= 72, "cardiamge value length is invalid");
        }

        public bool IsEmpty { get { return string.IsNullOrEmpty(_rawString); } }
        public bool IsComment { get { return Key == "COMMENT"; } }
        public bool IsHistory { get { return Key == "HISTORY"; } }
        public ICardImage Copy()
        {
            CardImage ci = new CardImage();
            ci.Update(_rawString);
            return ci;
        }
        #endregion

        /// <summary>
        /// check if the cardimage key is formatted similar to CRVALn
        /// </summary>
        /// <param name="key"></param>
        /// <param name="startTag"></param>
        /// <returns></returns>
        public bool IsOneIndexForm(string startTag)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(_key) && _key.StartsWith(startTag))
            {
                // one or more decimal
                Regex re = new Regex(@"\d+");
                ret = re.IsMatch(_key, startTag.Length);
            }
            return ret;
        }

        /// <summary>
        /// check if the cardimage has a form similar to PCi_j
        /// </summary>
        /// <param name="key"></param>
        /// <param name="startTag"></param>
        /// <returns></returns>
        public bool IsTwoIndexForm(string startTag)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(_key) && _key.StartsWith(startTag))
            {
                // one or more decimal, _, then one or more decimal
                Regex re = new Regex(@"\d+_\d+");
                ret = re.IsMatch(_key, startTag.Length);
            }
            return ret;
        }

        public bool IsWCS
        {
            get
            {
                bool ret = false;
                if (IsOneIndexForm("WCSAXIS") ||
                    IsOneIndexForm("WCSNAME") ||
                    IsOneIndexForm("CRVAL") ||
                    IsOneIndexForm("CRPIX") ||
                    IsOneIndexForm("CDELT") ||
                    IsOneIndexForm("CRDER") ||
                    IsOneIndexForm("CSYER") ||
                    IsOneIndexForm("CTYPE") ||
                    IsOneIndexForm("CUNIT") ||
                    IsTwoIndexForm("PC") ||
                    IsTwoIndexForm("CD") ||
                    IsTwoIndexForm("PV") ||
                    IsTwoIndexForm("PS"))
                {
                    ret = true;
                }
                return ret;
            }
        }

        public bool IsTable
        {
            get
            {
                bool ret = false;
                if (_key == "TFIELDS" ||
                    IsOneIndexForm("TTYPE") ||
                    IsOneIndexForm("TBCOL") ||
                    IsOneIndexForm("TFORM") ||
                    IsOneIndexForm("TUNIT") ||
                    IsOneIndexForm("TZERO") ||
                    IsOneIndexForm("TSCAL") ||
                    IsOneIndexForm("TNULL") ||
                    IsOneIndexForm("TDISP") ||
                    IsOneIndexForm("THEAP") ||
                    IsOneIndexForm("TDIM"))
                {
                    ret = true;
                }
                return ret;
            }
        }

        private void CleanStringValue(ref string val)
        {
            val = val.Trim();
            if (!string.IsNullOrEmpty(val))
            {
                if (val[0] == '\'')
                {
                    val = val.Remove(0, 1);
                    int ind = val.IndexOf('\'');
                    if(ind != -1)
                    {
                        val = val.Remove(ind);
                        val = val.Trim();
                    }
                }
            }
            val = val.Trim();
        }

        #region data members
        private string _key;
        private string _value;
        private string _comment;
        private string _rawString;
        private CardImageType _type;
        #endregion
    
    }
}
