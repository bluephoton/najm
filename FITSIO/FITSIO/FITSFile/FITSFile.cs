using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Najm.FITSIO
{
    public class Factory
    {
        public static IFITSFile CreateFITSFile() { return new Najm.FITSIO.FITSFile(); }
    }

    class FITSFile : IFITSFile
    {
        internal FITSFile()
        {
            _curHDUPosiotion = 0;
            _hdus = new List<HDU>(5);
            _hduIndexMap = new Dictionary<string, int>(5);
        }

        public static IFITSFile Create() { return new FITSFile(); }

        #region IFITSFile Members

        public void Load(string fileName)
        {
            Load(fileName, true);
        }
        public void Load(string fileName, bool readOnly)
        {
            // check if file exist
            ///////////////////////////////////////////////////////////////////
            Utils.CheckBool(File.Exists(fileName), "missing file");

            // Open the file
            ///////////////////////////////////////////////////////////////////
            FileStream fs;
            if(readOnly)
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            }
            else
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite);
            }

            // store file stream
            ///////////////////////////////////////////////////////////////////
            _fileStream = fs;

            // Now, read
            ///////////////////////////////////////////////////////////////////
            Load(fs);
        }

        private void Load(FileStream fs)
        {
            // is it empty?
            ///////////////////////////////////////////////////////////////////
            Utils.CheckBool(fs.Length != 0, "File is empty");
            
            // loop to read all HDUs
            ///////////////////////////////////////////////////////////////////
            _curHDUPosiotion = 0;
            while (fs.Position < fs.Length)
            {
                // load this hdu
                HDU hdu = new HDU(this, _curHDUPosiotion++);
                hdu.Load(fs);
                // if we didn't read any card images in this HDU - which means the first card image in the header
                // is empty, leave at this point
                if (hdu.CardImages.Length == 0)
                {
                    break;
                }
                // add it to our array. do this early so we can free later with array
                _hdus.Add(hdu);

                // keep a map of name index as well
                if (!string.IsNullOrEmpty(hdu.Name) && !_hduIndexMap.ContainsKey(hdu.Name))
                {
                    _hduIndexMap.Add(hdu.Name, _hdus.Count - 1);
                }

                // seek past the data of this hdu to prepare for loading next one
                if (hdu.RawDataSize != 0)
                {
                    long recordsLess = hdu.RawDataSize % (36 * 80);
                    if (recordsLess > 0)
                    {
                        long dataPadding = 36 * 80 - recordsLess;
                        fs.Seek(dataPadding, SeekOrigin.Current);
                    }
                }
            }
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Close() { _fileStream.Close(); }

        public string Name { get { return _fileStream.Name; } }
        public IHDU this[int index] { get { return _hdus[index]; } }
        public IHDU this[string hduName] { get { return _hdus[_hduIndexMap[hduName]]; } }
        public IHDU[] HDUs { get { return _hdus.ToArray(); } }
        #endregion

        #region data members
        private FileStream _fileStream;
        private int _curHDUPosiotion;
        private List<HDU> _hdus;
        private Dictionary<string, int> _hduIndexMap;
        #endregion
    }
}