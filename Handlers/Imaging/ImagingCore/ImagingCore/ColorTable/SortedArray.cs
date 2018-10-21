using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.ImagingCore.ColorTables
{
    // isolated so i can improve performance "later" :)
    class SortedArray
    {
        public SortedArray(int size)
        {
            _array = new List<double>(size);
        }

        public int Add(double item)
        {
            int insertionPoint;
            bool alreadyThere = FindItem(item, out insertionPoint);
            if (!alreadyThere)
            {
                _array.Insert(insertionPoint, item);
            }
            return insertionPoint;
        }

        private bool FindItem(double item, out int insertionPoint)
        {
            bool ret = false;
            int i = 0;
            for (i = 0; i < _array.Count; i++)
            {
                if (_array[i] == item)
                {
                    ret = true; // already there, so no need to insert again
                    break;
                }
                if (_array[i] > item)
                {
                    break;
                }
            }
            insertionPoint = i;
            return ret;
        }

        internal void Normalize()
        {
            // capture the original min/max values first, before missing up with the array
            // also calculate the original range.
            // these information is important for calculating non-linear scaling functions
            _minimum = _array[0];
            _maximum = _array[_array.Count - 1];
            _range = _maximum- _minimum;

            for (int i = 0; i < _array.Count; i++)
            {
                _array[i] = (_array[i] - _minimum) / _range;
            }
        }

        public double Maximum { get { return _maximum; } }
        public double Minimum { get { return _minimum; } }
        public double Range { get { return _range; } }
        public double[] Array { get { return _array.ToArray(); } }  // TODO : how this impact performance?!!!!!

        private List<double> _array;
        private double _minimum;
        private double _maximum;
        private double _range;  // ORIGINAL range. used for Log calculation as - not like Sqrt() - Log(a)/Log(b) != Log(a/b)
    }
}
