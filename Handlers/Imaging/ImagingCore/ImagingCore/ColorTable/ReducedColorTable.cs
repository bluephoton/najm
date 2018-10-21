using System;
using System.Collections.Generic;
using System.Text;
//using System.Drawing;
using Najm.ImagingCore.ColorScaling;

namespace Najm.ImagingCore.ColorTables
{
    class ReducedColorTable : BaseColorTable
    {
        public ReducedColorTable()
        {
            _numActivePixels = 0;
            _scalingAlgorithm = null;
        }

        #region IColorTable Members

        public override void Initialize(int size, double minColorValue, double maxColorValue)
        {
            _size = size;
            _minimum = minColorValue;
            _maximum = maxColorValue;
            _scale = ((double)size - 1) / (maxColorValue - minColorValue);
            _table = new double[size];
            for (int i = 0; i < size; i++)
            {
                _table[i] = (double)i / (double)(size - 1);   // begin with normalized one. 
            }
        }

        public override int Size { get { return _size; } }

        public override IScalingAlgorithm ScalingAlgorithm
        {
            get { return _scalingAlgorithm; }
            set
            {
                _scalingAlgorithm = value;
                _lookupTable = null;    // we need to force rebuilding this table
            }
        }

        public override int Add(double pixelValue)
        {
            int index = _size;  // use it for error too
            // NaN values don't carry color info to be added to the table, but we need to use
            // a special index so that Lookup can return some good color. I'm using an index 
            // right outside the bounds of the table
            if (Double.IsNaN(pixelValue))
            {
                index = _size;
            }
            else
            {
                _numActivePixels++;
                index = (int)((pixelValue - _minimum) * _scale + 0.5);
            }
            return index;
        }

        public override void Normalize()
        {
            // we already normalized it on initilizations.
        }

        public override void ScaleIt(int[] image, int width, int height)
        {
            // we'll copy the raw sorted array rather than scaling it in place, this way we
            // can change the scale if we want without loosing the raw normalized data.
            if (_lookupTable == null)
            {
                _lookupTable = _scalingAlgorithm.Apply(image, width, height, _numActivePixels, _table, _minimum, _maximum);
            }
        }

        // return false indicate invalid pixel value
        public override bool Lookup(int pixelIndex, out int pixelColor)
        {
            bool ret = true;
            if (pixelIndex >= _size)
            {
                // Ops! we have no color info for this pixel
                pixelColor = -1;
                ret = false;
            }
            else
            {
                pixelColor = _lookupTable[pixelIndex];
            }
            return ret;
        }

        public override double Minimum { get { return _minimum; } }
        public override double Maximum { get { return _maximum; } }
        #endregion

        #region Member variables
        private double _scale;
        private double[] _table;

        #endregion
    }
}