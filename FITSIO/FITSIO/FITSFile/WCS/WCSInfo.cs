using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.FITSIO
{
    internal class WCSInfo : IWCSInfo
    {
        #region text info template
        private string _textTemplate =
            @"Number of alternatives: {0}\par\par" + "\n" +
            @"\ul\b Primary Transformation:\par" + "\n" +
            @"\ulnone\b0\pard\li360 ";
        #endregion
        internal WCSInfo(HDU hdu)
        {
            _hdu = hdu;
            // create primary transformation
            _primaryTransformation = new Transformation(this, true);
            // allocate 26 possible transformations. Those missing from the header will be null.
            _transformations = new Transformation[26];
            // map to convert an index to its alternative letter
            _index2LetterMap = new char[26];
            _numAlternatives = 0;
        }

        internal void FinalizeConstruction()
        {
            if (_primaryTransformation != null)
            {
                _primaryTransformation.FinalizeConstruction();
            }
            foreach (Transformation t in _transformations)
            {
                if (t != null)
                {
                    t.FinalizeConstruction();
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IWCSInfo Members
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int NumAlternatives { get { return _numAlternatives; } }
        public ITransformation PrimaryTransformation { get { return _primaryTransformation; } }
        public ITransformation this[int index]
        {
            get
            {
                if (index < 0 || index > 25 || _index2LetterMap[index] == '\0')
                {
                    throw new IndexOutOfRangeException("WCS Transformation index");
                }
                return this[_index2LetterMap[index] - 'A'];
            }
        }
        public ITransformation this[char letter]
        {
            get
            {
                if (letter != '\0' && (letter < 'A' || letter > 'Z'))
                {
                    throw new IndexOutOfRangeException("WCS Transformation letter");
                }
                return _transformations[letter - 'A'];
            }
        }
        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_transformations != null)
            {
                string t = string.Format(_textTemplate, _numAlternatives.ToString());
                sb.Append(t);
                sb.Append(_primaryTransformation.ToString());
                sb.Append(@"\pard\ul").Append(' ',150).Append(@"\par\par");
                int alternate = 0;
                for (int i = 0; i < _transformations.Length; i++)
                {
                    if (_transformations[i] != null)
                    {
                        sb.Append(string.Format(@"\pard\ul\b Alternate #{0}({1}):\par\ulnone\b0\pard\li360 ", ++alternate,(char)('A' + i)));
                        sb.Append(_transformations[i].ToString());
                        sb.Append(@"\ulnone\b0\pard\li360 ");
                    }
                }
            }
            return sb.ToString();
        }

        internal void HandleCardImage(CardImage ci)
        {
            string key = ci.Key;
            string val = ci.Value;

            char alternative;
            Transformation t = null;

            if (key.StartsWith("WCSAXIS") || key.StartsWith("WCSNAME"))
            {
                alternative = ParseAlternative(ref key, 7);
                t = alternative=='\0' ? _primaryTransformation : Alternative(alternative);
            }
            else if (ci.IsOneIndexForm("CRVAL") || ci.IsOneIndexForm("CRPIX") || ci.IsOneIndexForm("CDELT") ||
                        ci.IsOneIndexForm("CTYPE") || ci.IsOneIndexForm("CUNIT") || ci.IsOneIndexForm("CRDER") || ci.IsOneIndexForm("CSYER"))
            {
                alternative = ParseAlternative(ref key, 5);
                t = alternative=='\0' ? _primaryTransformation : Alternative(alternative);
            }
            else if (ci.IsTwoIndexForm("PC") || ci.IsTwoIndexForm("CD") || ci.IsTwoIndexForm("PV") || ci.IsTwoIndexForm("PS"))
            {
                alternative = ParseAlternative(ref key, 2);
                t = alternative == '\0' ? _primaryTransformation : Alternative(alternative);
            }
            if (t != null)
            {
                // we removed the alternate coordinate letter from the key. we need to update ci
                // with the new key as transformations are not aware of the althernatives letter
                CardImage copy = ci.Copy() as CardImage;
                copy.Key = key;
                t.HandleCardimage(copy);
            }
        }

        private char ParseAlternative(ref string key, int lenght)
        {
            char alternative;
            if (key.Length == lenght)
            {
                // no alternative coordinate letter or invalid key.
                // invalid key case will be intercepted in the Transformation class.
                alternative = '\0';
            }
            else
            {
                // we have some extra stuff, it must be numeric ending with the alternative
                // coordinate letter.
                string ind_alt = key.Substring(lenght);
                alternative = ind_alt[ind_alt.Length - 1];
                if (alternative >= 'A' && alternative <= 'Z')
                {
                    // remove the alternative coordinate letter
                    key = key.Substring(0, key.Length - 1);
                }
                else
                {
                    // again, no alternative letter at the end
                    alternative = '\0';
                }                
            }
            return alternative;
        }

        private Transformation Alternative(int index)
        {
            if(index < 0 || index > 25)
            {
                throw new IndexOutOfRangeException("WCS Transformation index");
            }
            if (_transformations[index] == null)
            {
                _transformations[index] = new Transformation(this);
                _index2LetterMap[_numAlternatives] = (char)('A' + index);
                _numAlternatives++;
            }
            return _transformations[index];
        }

        private Transformation Alternative(char letter)
        {
            if (letter < 'A' || letter > 'Z')
            {
                throw new IndexOutOfRangeException("WCS Transformation letter");
            }
            return Alternative(letter - 'A');
        }

        private HDU _hdu;
        private Transformation _primaryTransformation;
        private Transformation[] _transformations;
        private char[] _index2LetterMap;
        private int _numAlternatives;
    }
}
