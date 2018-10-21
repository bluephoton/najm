using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.FITSIO
{
    internal class WCSAxis : IWCSAxis
    {
        #region text template
        private string _textTemplate =
            @"Unit:\tab\tab\tab {0}\par" + "\n" +
            @"Coordinate Type:\tab {1}\par" + "\n" +
            @"Algorithm Code:\tab {2}\par" + "\n" +
            @"Numeric Params:" +
            @"\tab {3}\par" + "\n" +
            @"String Params:" +
            @"\tab {4}\par" + "\n";
        #endregion
        internal WCSAxis()
        {
            //
            _numericParams = new List<double>();
            _textParams = new List<string>();
        }

        internal string Type
        {
            set {
                // store the type as is
                _type = value;
                // pars it if it has coordinate type and algorithm code
                if (_type.Contains("-"))
                {
                    string t = _type.Trim();
                    string[] parts = t.Split('-');
                    _coordinateType = string.IsNullOrEmpty(parts[0]) ? "" : parts[0].Trim();
                    _algorithmCode = (parts.Length > 1 && string.IsNullOrEmpty(parts[parts.Length - 1])) ? "" : parts[parts.Length - 1].Trim();
                }
            }
            get { return _type; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IWCSAxis Members
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Unit
        {
            get { return _unit; }
            internal set { _unit = value; }
        }

        public string CoordinateType { get { return _coordinateType; } }
        public string AlgorithmCode { get { return _algorithmCode; } }
        public double[] NumericParams { get { return _numericParams.ToArray(); } }
        public string[] TextParams { get { return _textParams.ToArray(); } }
        #endregion

        public override string ToString()
        {
            string[] numericParams = new string[_numericParams.Count];
            for (int i = 0; i < _numericParams.Count; i++)
            {
                numericParams[i] = _numericParams[i].ToString();
            }

            string t = string.Format(_textTemplate,
                        string.IsNullOrEmpty(_unit) ? "<Missing>" : _unit,
                        string.IsNullOrEmpty(_coordinateType) ? _type : _coordinateType,
                        string.IsNullOrEmpty(_algorithmCode) ? "N/A" : _algorithmCode,
                        numericParams.Length > 0 ? string.Join(", ", numericParams) : "<Missing>",
                        _textParams.Count > 0 ? string.Join(", ", _textParams.ToArray()) : "<Missing>");
            return t;
        }
        internal void AddNumericParam(int j, double p)
        {
            Utils.CheckBool(j == _numericParams.Count, "invalid PVi_m sequence");
            _numericParams.Add(p);
        }

        internal void AddTextParam(int j, string p)
        {
            Utils.CheckBool(j == _textParams.Count, "invalid PSi_m sequence");
            _textParams.Add(p);
        }

        private string _unit;
        //
        private List<double> _numericParams;
        private List<string> _textParams;
        private string _type;
        private string _coordinateType;
        private string _algorithmCode;
    }
}
