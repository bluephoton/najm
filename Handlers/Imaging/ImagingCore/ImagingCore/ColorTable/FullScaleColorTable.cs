using System;
using System.Collections.Generic;
using System.Text;
//using System.Drawing;
using Najm.ImagingCore.ColorScaling;

namespace Najm.ImagingCore.ColorTables
{
    class FullScaledColorTable : BaseColorTable
    {
        public FullScaledColorTable()
        {
            _numActivePixels = 0;
            _scalingAlgorithm = null;
        }

        #region IColorTable Members

        public override void Initialize(int size, double minColorValue, double maxColorValue)
        {
            _minimum = minColorValue;
            _maximum = maxColorValue;
            _numActivePixels = 0;

            _normalizedPixelValues = new SortedArray(2560 * 2560);  // // sound like a fair size to start with!
        }

        public override int Size { get { return -1; } }  // size is not supported on this implementation (for now, think more later)

        public override IScalingAlgorithm ScalingAlgorithm
        {
            get { return _scalingAlgorithm; }
            set { _scalingAlgorithm = value; }
        }

        public override int Add(double pixelValue)
        {
            int index;
            if (Double.IsNaN(pixelValue))
            {
                index = 0x7FFF;
            }
            else
            {
                _numActivePixels++;
                index = _normalizedPixelValues.Add(pixelValue);
            }
            return index;
        }

        public override void Normalize()
        {
            _normalizedPixelValues.Normalize();
        }

        public override void ScaleIt(int[] image, int width, int height)
        {
            // we'll copy the raw sorted array rather than scaling it in place, this way we
            // can change the scale if we want without loosing the raw normalized data.
            _lookupTable = _scalingAlgorithm.Apply(image, width, height, _numActivePixels, _normalizedPixelValues.Array, _minimum, _maximum);
        }

        public override bool Lookup(int pixelIndex, out int pixelColor)
        {
            bool ret = true;
            if (pixelIndex == 0x7FFF)
            {
                // Ops! we have no color info for this pixel, lets make it Red!
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

        private SortedArray _normalizedPixelValues;     // this is basically the histogram of the Raw image. it will be 
                                                        // normalized after its build then will be used to scale image.
    }
}
