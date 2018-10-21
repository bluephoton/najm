using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.FITSIO
{
    internal class Axis : IAxis
    {
        internal Axis(int sequence, int numPoints)
        {
            _sequence = sequence;
            _numPoints = numPoints;
        }

        #region IAxis Members

        public int Sequence { get { return _sequence; } }
        public int NumPoints { get { return _numPoints; } }
        public string InfoText
        {
            get { return "Axis#:" + _sequence.ToString() + ", Points:" + _numPoints.ToString(); }
        }

        #endregion

        #region data members

        private int _sequence;
        private int _numPoints;
        
        #endregion

    }
}
