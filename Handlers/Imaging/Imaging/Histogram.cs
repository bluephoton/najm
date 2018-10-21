using System;
using System.Collections.Generic;
using System.Text;
using Najm.FITSIO;

namespace ImageHandler
{
    // a generic implementation is more appropriate. Later...
    class Histogram
    {
        // this is ok for small data. For large data use the binned version below
        #region exact solution!
        internal Histogram(double[] data)
        {
            if (data.Length == 1)
            {
                // trivial case
                _frequencies = new double[1] { 1 };

            }
            else
            {
                // make a local copy of the data as we sort in place. get rid of NaN values as you copy
                _data = new double[data.Length];
                int j = 0;
                for (int i = 0; i < _data.Length; i++)
                {
                    double val = data[i];
                    if (!Double.IsNaN(val))
                    {
                        _data[j++] = val;
                    }
                }
                // resize the array
                Array.Resize<double>(ref _data, j);

                // invoke the sort
                Array.Sort<double>(_data);

                // allocate frequency array
                _frequencies = new double[_data.Length];

                // build the histogram
                BuildHistogram();
            }
        }
        private void BuildHistogram()
        {
            int length = _data.Length;
            int i = 0;
            int j = 1;
            while (true)
            {
                double item = _data[i];
                double frequency = 1;

                // count as long as item is repeated
                while (j < length && _data[j] == item)
                {
                    frequency++;
                    j++;
                }

                //  - store frequency of current item.
                _frequencies[i] = frequency;

                if (j < length)     // new item found?
                {
                    //  - compact data
                    _data[i + 1] = _data[j];

                    //  - adjust variables to count the new item
                    i++;
                    j++;
                }
                else
                {
                    break;
                }
            }
            // resize our vectors
            Array.Resize<double>(ref _data, i + 1);
            Array.Resize<double>(ref _frequencies, i + 1);
        }
        #endregion

        // problem of this binning technique is that histogram may have items with zero frequency
        #region not exact solution (binning)
        internal Histogram(double[] data, int numBins, double min, double max, double blankValue)
        {
            // if we don't have data do nothing
            if (min != 0 || max != 0)
            {
                // keep min/max, for convenience
                _minimum = min;
                _maximum = max;

                // allocate vectors
                _data = new double[numBins];
                _frequencies = new double[numBins];

                // initialize data
                for (int i = 0; i < numBins; i++)
                {
                    _data[i] = min + i * (max - min) / (numBins - 1);
                }

                // build histogram
                for (int i = 0; i < data.Length; i++)
                {
                    double val = data[i];
                    if (!IsNaN(val, blankValue)) // skip NaNs
                    {
                        int index = (int)((val - min) * (numBins - 1) / (max - min) + 0.5);
                        _frequencies[index]++;
                    }
                }
            }
        }
        #endregion

        private bool IsNaN(double val, double blankValue)
        {
            bool ret;
            // some how comparing Nan!=Nan gives true! so I'm doing this instead
            if (double.IsNaN(blankValue))
            {
                ret = double.IsNaN(val);
            }
            else
            {
                ret = (val == blankValue);
            }
            return ret;
        }

        internal double[] Data { get { return _data; } }
        internal double[] Frequencies { get { return _frequencies; } }
        //internal double Maximum { get { return _maximum; } }
        //internal double Minimum { get { return _minimum; } }

        private double[] _data;
        private double[] _frequencies;
        private double _minimum;
        private double _maximum;
    }
}
