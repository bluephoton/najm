using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Najm.LinearAlagebra;

namespace Najm.FITSIO
{
    internal class Transformation : ITransformation
    {
        #region text template
        private string _textTemplate =
            @"Name:\tab\tab\tab {0}\par" + "\n" +
            @"LT Matrix (i,j):\par\pard\li2880 " +
            @"{1}\pard\li360 " + "\n" +
            @"Pixel Reference(j):\tab {2}\par" + "\n" +
            @"World Reference(i):\tab {3}\par" + "\n" +
            @"Pixel to Physical Scale(i):\tab {4}\par" + "\n" +
            @"Random Errors:\tab\tab {5}\par" + "\n" +
            @"Systematic Errors:\tab {6}\par" + "\n" +
            @"Number of Axes:\tab\tab {7}\par\par";
        #endregion
        internal Transformation(WCSInfo wcsi, bool primary)
        {
            Initialize(wcsi, primary);
        }

        internal Transformation(WCSInfo wcsi)
        {
            Initialize(wcsi, false);
        }

        private void Initialize(WCSInfo wcsi, bool priamry)
        {
            _primary = priamry;
            _wcsInfo = wcsi;
            _numWCSAxes = -1;   // means WCSAXIS cardimage doesn't exist
            // those will be filled as we receive cardiamges. Finalize will create an IVector from each.
            _worldRefElements = new List<double>(3);
            _pixelRefElements = new List<double>(3);
            _pix2physElements = new List<double>(3);
            _oldRotationElements = new List<double>(3);
            _randErrElements = new List<double>(3);
            _sysErrElements = new List<double>(3);
            _PCij = new List<List<double>>(4);
            _axes = new List<WCSAxis>();
        }

        internal void FinalizeConstruction()
        {
            _worldReference = (_worldRefElements.Count >0) ? new Vector(_worldRefElements.ToArray()) : null;
            _pixelReference = (_pixelRefElements.Count > 0) ? new Vector(_pixelRefElements.ToArray()) : null;
            _pixel2PhysScale = (_pix2physElements.Count > 0) ? new Vector(_pix2physElements.ToArray()) : null;
            _randomError = (_randErrElements.Count > 0) ? new Vector(_randErrElements.ToArray()) : null;
            _OldRotation = (_oldRotationElements.Count > 0) ? new Vector(_oldRotationElements.ToArray()) : null;
            _systematicError = (_sysErrElements.Count > 0) ? new Vector(_sysErrElements.ToArray()) : null;

            // fix matrix elements arrays to ensure they generate k x k matrices with k = Max(i,j)
            if (_PCij.Count != 0)
            {
                FixMatrixElements(ref _PCij);
                _linearTransformation = new Matrix(_PCij);
            }

            // Apply CDELTi to the PCij matrix - CDELTi exist. I use PCij always to handle both PC and CD formates
            if (_pixel2PhysScale != null)
            {
                if (_linearTransformation == null)
                {
                    _linearTransformation = Matrix.Unit(_pixel2PhysScale.Dimension); 
                }
                _linearTransformation.Scale(_pixel2PhysScale);
            }

            // Apply CROTAi to PCij
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region ITransformation Members
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// return the linear transformation matrix (defines by PCij, CDij, CROTAn)
        /// </summary>
        public Matrix LinearTransformation { get { return _linearTransformation; } }
        /// <summary>
        /// returns a vextor whose elements represent CRPIXj values
        /// </summary>
        public Vector PixelReference { get { return _pixelReference; } }
        /// <summary>
        /// returns a vector whose elements represent CRVALi values
        /// </summary>
        public Vector WorldReference { get { return _worldReference; } }
        /// <summary>
        /// Returns a vector whose elements represent CDELTi values
        /// </summary>
        public Vector Pixel2Physical { get { return _pixel2PhysScale; } }
        /// <summary>
        /// returns an array of coordinates in this WCS transformations
        /// </summary>
        public IWCSAxis[] Axes { get { return _axes.ToArray(); } }
        /// <summary>
        /// true if this is the primary transformation, false if it is one of the alternates
        /// </summary>
        public bool IsPrimary { get { return _primary; } }
        /// <summary>
        /// returns the name of the coordinate transformation - WCSNAMEa
        /// </summary>
        public string Name { get { return _name; } }
        /// <summary>
        /// returns a vector whose elements represent the random errors associated with each coordinate of this transformation
        /// </summary>
        public Vector RandomError { get { return _randomError; } }
        /// <summary>
        /// returns a vector whose elements represent the systematic errors associated with each coordinate of this transformation
        /// </summary>
        public Vector SystematicError { get { return _systematicError; } }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public override string ToString()
        {
            string t = string.Format(_textTemplate,
                        string.IsNullOrEmpty(_name) ? (IsPrimary ? "Primary" : "<Missing>") : _name,
                        _linearTransformation == null ? @"<Missing>\par " : _linearTransformation.ToString(),
                        _pixelReference == null ? "<Missing>" : _pixelReference.ToString(),
                        _worldReference == null ? "<Missing>" : _worldReference.ToString(),
                        _pixel2PhysScale == null ? "<Missing>" : _pixel2PhysScale.ToString(),
                        _randomError == null ? "<Missing>" : _randomError.ToString(),
                        _systematicError == null ? "<Missing>" : _systematicError.ToString(),
                        (_axes != null && _axes.Count > 0) ? _axes.Count.ToString() : "0");
            for (int i = 0; i < _axes.Count; i++)
            {
                t += string.Format(@"\ul\b Axis #{0}\par" + "\n", i);
                t += @"\pard\li720\ulnone\b0" + "\n";
                t += _axes[i].ToString();
                t += @"\pard\li360\par";
            }
            return t;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region privates and internals
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void HandleCardimage(CardImage ci)
        {
            string key = ci.Key;
            string value = ci.Value;

            int i, j;
            if (ci.IsOneIndexForm("WCSAXIS"))
            {
                _numWCSAxes = int.Parse(value);
            }
            else if (ci.IsOneIndexForm("WCSNAME"))
            {
                _name = value;
            }
            else if (ci.IsOneIndexForm("CRVAL"))
            {
                AddElementToVectorElementList(ci, ref _worldRefElements);
            }
            else if (ci.IsOneIndexForm("CRPIX"))
            {
                AddElementToVectorElementList(ci, ref _pixelRefElements);
            }
            else if (ci.IsOneIndexForm("CDELT"))
            {
                AddElementToVectorElementList(ci, ref _pix2physElements);
            }
            else if (ci.IsOneIndexForm("CRDER"))
            {
                AddElementToVectorElementList(ci, ref _randErrElements);
            }
            else if (ci.IsOneIndexForm("CSYER"))
            {
                AddElementToVectorElementList(ci, ref _sysErrElements);
            }
            else if (ci.IsOneIndexForm("CTYPE"))
            {
                i = ParseIndex(key);
                WCSAxis a = GetOrCreateAxis(i);
                a.Type = value;
            }
            else if (ci.IsOneIndexForm("CUNIT"))
            {
                i = ParseIndex(key);
                WCSAxis a = GetOrCreateAxis(i);
                a.Unit = value;
            }
            else if (ci.IsTwoIndexForm("CROTA"))
            {
                AddElementToVectorElementList(ci, ref _oldRotationElements);
            }
            else if (ci.IsTwoIndexForm("PC"))
            {
                AddElementToMatrixRowList(ci, ref _PCij);
            }
            else if (ci.IsTwoIndexForm("CD"))
            {
                AddElementToMatrixRowList(ci, ref _PCij);
            }
            else if (ci.IsTwoIndexForm("PV"))
            {
                ParseTwoIndices(key, out i, out j);
                WCSAxis a = GetOrCreateAxis(i);
                a.AddNumericParam(j, double.Parse(value));
            }
            else if (ci.IsTwoIndexForm("PS"))
            {
                ParseTwoIndices(key, out i, out j);
                WCSAxis a = GetOrCreateAxis(i);
                a.AddTextParam(j, value);
            }
        }

        private WCSAxis GetOrCreateAxis(int i)
        {
            if (_axes.Count > i)
            {
                // we aready have slot for this axis. no need to do anything
            }
            else
            {
                for (int k = _axes.Count; k <= i; k++)
                {
                    _axes.Add(new WCSAxis());
                }
            }
            return _axes[i];
        }

        private void AddElementToVectorElementList(CardImage ci, ref List<double> vectorElement)
        {
            string key = ci.Key;
            string value = ci.Value;
            int i = ParseIndex(key);
            if (vectorElement.Count > i)
            {
                // we have slot ready, no need to do anything
            }
            else
            {
                // create as many elements until we have necessary slot
                for (int k = vectorElement.Count; k < i; k++)
                {
                    vectorElement.Add(0);
                }
            }
            // some files contains invalid values, yet they have images that i would like to see
            // I'm allowing a bogus value now and will assert in debug untill I reconsult the specs.
            // TODO: consult the specs and ENFORCE it
            double val = double.NaN;
            if (!double.TryParse(value, out val))
            {
                System.Diagnostics.Debug.Assert(false, string.Format("value is not a valid double ({0}={1})", ci.Key, ci.Value));
            }
            vectorElement.Add(val);
        }

        private void AddElementToMatrixRowList(CardImage ci, ref List<List<double>> matrixElements)
        {
            string key = ci.Key;
            string value = ci.Value;
            int i, j;

            ParseTwoIndices(key, out i, out j);

            List<double> row;
            if ((matrixElements.Count > i))
            {
                // row already created
                row = matrixElements[i];
            }
            else
            {
                // not yet there, create as many rows as necessary untill we hit i
                List<double> newRow = null;
                for (int k = matrixElements.Count; k <= i; k++)
                {
                    newRow = new List<double>();
                    matrixElements.Add(newRow);
                }
                row = newRow;
            }
            // now, add the element in the j column
            if (row.Count > j)
            {
                // list already has slot at j. Nothing to be done!
            }
            else
            {
                // now, create as many columns as necessary untill we hit j
                for (int k = row.Count; k <= j; k++)
                {
                    row.Add(0);
                }
            }
            // some files contains invalid values, yet they have images that i would like to see
            // I'm allowing a bogus value now and will assert in debug untill I reconsult the specs.
            // TODO: consult the specs and ENFORCE it
            double val = double.NaN;
            if (!double.TryParse(value, out val))
            {
                System.Diagnostics.Debug.Assert(false, string.Format("value is not a valid double ({0}={1})", ci.Key, ci.Value));
            }
            row[j] = val;
            // NOTE:
            // - after we're done with all encountered PCij cardiamges, there is no gurantee
            //   that the rows have equal length.
            // - This will be handled in the FinalizeConstruction call. We'll ensure the matrix
            //   is square with dimension k x k; where k is Max(i,j)
        }

        private int ParseIndex(string key)
        {
            int ind = key.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            Utils.CheckBool(ind > 0, string.Format("invalid WCS key ({0})", key));
            return int.Parse(key.Substring(ind)) - 1;   // 1 indexed
        }

        private void ParseTwoIndices(string key, out int i, out int j)
        {
            int ind = key.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            Utils.CheckBool(ind > 0, string.Format("invalid WCS key ({0})", key));
            string ij = key.Substring(ind);
            string[] ij2 = ij.Split('_');
            Utils.CheckBool(ij2.Length == 2, string.Format("invalid WCS key ({0})", key));
            i = int.Parse(ij2[0]) - 1;
            j = int.Parse(ij2[1]) - 1;
        }

        private void FixMatrixElements(ref List<List<double>> m)
        {
            int dim = m.Count;
            // figure out the dimension of the matrix
            foreach (List<double> row in m)
            {
                if (row.Count > dim)
                {
                    dim = row.Count;
                }
            }
            // ensure all rows are of dim dimension
            foreach (List<double> row in m)
            {
                if (row.Count < dim)
                {
                    row.AddRange(new double[dim - row.Count]);
                }
            }
            // ensure we have dim number of rows all zero initialized
            if (m.Count < dim)
            {
                List<double>[] extraRows = new List<double>[dim - m.Count];
                for (int i = 0; i < extraRows.Length; i++)
                {
                    extraRows[i] = new List<double>(new double[dim - m.Count]);
                }
                m.AddRange(extraRows);
            }
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private WCSInfo _wcsInfo;
        private string _name;
        private bool _primary;
        private int _numWCSAxes;
        // CRVALi
        private List<double> _worldRefElements;
        private Vector _worldReference;
        // CRPIXj
        private List<double> _pixelRefElements;
        private Vector _pixelReference;
        // CDELTi
        private List<double> _pix2physElements;
        private Vector _pixel2PhysScale;
        // CROTAi
        private List<double> _oldRotationElements;
        private Vector _OldRotation;
        // 
        private List<double> _randErrElements;
        private Vector _randomError;
        //
        private List<double> _sysErrElements;
        private Vector _systematicError;
        // 
        private List<List<double>> _PCij;    // list of ROWS, i represnet Row index
        private Matrix _linearTransformation;
        //
        private List<WCSAxis> _axes;
        #endregion
    }
}
