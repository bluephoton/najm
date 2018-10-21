using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Najm.ImagingCore.ColorScaling;
using Najm.ImagingCore.ColorMaps;
using System.Xml;

namespace Najm.ImagingCore.ColorTables
{
    // this is indexed color table:
    // - Image will be 8bit per pixel and must have a palette
    // - color table will not have a palette, palette realization
    //   happen outside it.
    class IndexedColorTable : BaseColorTable
    {
        public IndexedColorTable()
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
                // since I now allow user to choose min/max , I need to clip the pixel value
                // to ensure its withing this range
                pixelValue = pixelValue > _maximum ? _maximum : pixelValue;
                pixelValue = pixelValue < _minimum ? _minimum : pixelValue;
                // increment the number of active pixels
                _numActivePixels++;
                // map
                index = (int)((pixelValue - _minimum) * _scale + 0.5);
            }
            return index;
        }

        public override void Normalize()
        {
            // we already normalized it on initilization.
        }

        // scale the color table to our 256 colors
        public override void ScaleIt(int[] image, int width, int height)
        {
            // we'll copy the raw sorted array rather than scaling it in place, this way we
            // can change the scale if we want without loosing the raw normalized data.
            if (_lookupTable == null)
            {
                _lookupTable = _scalingAlgorithm.Apply(image, width, height, _numActivePixels, _table, _minimum, _maximum);
            }
        }

        public override bool Lookup(int pixelIndex, out int pixelColor)
        {
            bool ret = true;
            if (pixelIndex >= _size)
            {
                // Ops! we have no color info for this pixel, lets make it Red!
                pixelColor = -1;
                ret = false;
            }
            else
            {
                // for indexed image i use last palette entry (255) to represent invalid pixel color (transparent color)
                // if we encountered 255 replace it with 254 instead
                // TODO: this is none sense!!! ensure that we won't exceed 254 during color scaling instead - by passing
                // 254 as max color instead of hardcoded 255. Or figure out if there is a bug around this area!
                // then replace this with: "pixelColor = _lookupTable[pixelIndex];" only.
                byte b = _lookupTable[pixelIndex];
                pixelColor = (b == 255) ? 254 : b;
            }
            return ret;
        }

        public override double Minimum { get { return _minimum; } }
        public override double Maximum { get { return _maximum; } }

        #endregion

        #region Member variables

        private double[] _table;
        private double _scale;

        #endregion
    }
}
