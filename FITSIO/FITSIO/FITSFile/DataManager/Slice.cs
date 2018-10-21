using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Najm.FITSIO
{
    // ISlice hides the difference between data types and always return double values
    internal interface ISlice
    {
        // access the unified data
        double Minimum { get; }
        double Maximum { get; }
        double[] Data { get; }
        long DataSize { get; }
        // access the raw data
        byte[] RawData { get; }
    }

    internal class Slice : ISlice
    {
        internal Slice(byte[] data, long offset, long numElements, long elementSize, int bitsPerPixel, double blankValue)
        {
            _minimum = _maximum = 0;
            _rawData = data;
            _numElements = numElements;
            _elementSize = elementSize;
            _extremesEvaluated = false;
            _offset = offset;   // offset of the beginning of this slice's data in the rawData array.
            _bitsPerPixel = bitsPerPixel;
            _blankValue = blankValue;
        }

        public double Minimum
        {
            get
            {
                if (!_extremesEvaluated)
                {
                    FindExtremes();
                }
                return _minimum;
            }
        }

        public double Maximum
        {
            get
            {
                if (!_extremesEvaluated)
                {
                    FindExtremes();
                }
                return _maximum;
            }
        }
        public byte[] RawData { get { return _rawData; } }
        public double[] Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new double[_numElements];
                    if (_bitsPerPixel == -64)
                    {
                        Buffer.BlockCopy(_rawData, 0, _data, 0, (int)_numElements);
                    }
                    else
                    {
                        Utils.UnifyData(_data, _rawData, _offset, _bitsPerPixel);
                    }
                }
                return _data;
            }
        }
        public long DataSize { get { return _numElements; } }   // faster than Data.Length as we don't need to alloc & unify data
        private void FindExtremes()
        {
            bool first = true;
            for (int i = 0; i < _numElements; i++)
            {
                double val = Data[i];
                if(!IsBlank(val))
                {
                    if (first)
                    {
                        _minimum = _maximum = val;
                        first = false;
                    }
                    if (val > _maximum) _maximum = val;
                    if (val < _minimum) _minimum = val;
                }
            }
            _extremesEvaluated = true;
        }

        private bool IsBlank(double val)
        {
            return (_bitsPerPixel > 0) ? (val == _blankValue) : double.IsNaN(val);
        }

        #region data members
        private byte[] _rawData;
        private long _numElements;
        private double _minimum;
        private double _maximum;
        private int _bitsPerPixel;
        private double _blankValue;
        private long _offset;  // offset of this slice data in _data.
        private long _elementSize;
        private bool _extremesEvaluated;
        private double[] _data;
        #endregion
    }
}
