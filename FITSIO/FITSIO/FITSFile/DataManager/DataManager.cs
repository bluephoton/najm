using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.FITSIO
{
    class DataManager : IDataManager
    {
        public DataManager(HDU hdu, byte[] data, int bitsPerPixel, double blank, int numSlices)
        {
            // init things
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            _hdu = hdu;
            _rawData = data;
            _bitsPerPixel = bitsPerPixel;
            _elementSize = (bitsPerPixel > 0) ? (bitsPerPixel / 8) : (- bitsPerPixel / 8);  // in bytes
            _blank = blank;
            
            // create slies
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            long numElements = (long)data.Length / numSlices / _elementSize;
            _slices = new List<ISlice>(numSlices);
            for (int i = 0; i < numSlices; i++)
            {
                long offset = i * numElements * _elementSize;
                _slices.Add(new Slice(data, offset, numElements, _elementSize, bitsPerPixel, blank));
            }
        }

        /// <summary>
        /// called for primary and image extentions
        /// </summary>
        internal void InitializeImage()
        {
            if (BitConverter.IsLittleEndian)
            {
                Utils.FixByteOrder(_rawData, _elementSize);
            }
        }

        #region IDataMngr Members

        public int BitsPerPixel { get { return _bitsPerPixel; } }
        public byte[] RawData { get { return _slices[_currentSlice].RawData; } }
        public double[] Data{ get { return _slices[_currentSlice].Data; } }
        public long DataSize { get { return _slices[_currentSlice].DataSize; } }
        public double BlankValue { get { return _blank; } }
        public double Maximum { get { return _slices[_currentSlice].Maximum; } }
        public double Minimum { get { return _slices[_currentSlice].Minimum; } }
        public int NumSlices { get { return _slices.Count; } }
        public int CurrentSlice
        {
            get { return _currentSlice; }
            set
            {
                Utils.CheckBool(value < _slices.Count, "Attempt to  set a slice with index out of range");
                _currentSlice = value;
            }
        }
        public IHDU HDU { get { return _hdu; } }

        #endregion

        #region data members
        private HDU _hdu;
        private byte[] _rawData;
        private int _bitsPerPixel;
        private double _blank;
        private int _currentSlice;
        protected List<ISlice> _slices;
        private int _elementSize;
        #endregion
    }
}
