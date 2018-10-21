using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Najm.LinearAlagebra;

namespace Najm.LinearAlagebra
{
    public class Matrix : Tensor
    {
        public Matrix(int rows, int columns)
        {
            CreateElementsArray(rows, columns);
        }

        public static Matrix FromRows(double[][] rows)
        {
            throw new NotImplementedException();
        }

        public static Matrix FromColumns(double[][] rows)
        {
            throw new NotImplementedException();
        }

        public Matrix(List<List<double>> elements)
        {
            CreateElementsArray(elements.Count, elements[0].Count);
            for (int r = 0; r < elements.Count ; r++)
            {
                List<double> row = elements[r];
                _rows[r].Assign(row.ToArray());
            }
        }

        public static Matrix Unit(int dim)
        {
            Matrix m = new Matrix(dim, dim);
            for (int i = 0; i < dim; i++)
            {
                m[i][i] = 1.0;
            }
            return m;
        }

        private void CreateElementsArray(int rows, int columns)
        {
            _rows = new Vector[rows];
            for (int i = 0; i < _rows.Length; i++)
            {
                _rows[i] = new Vector(columns);
            }
        }

        public int NumRows { get { return _rows.Length; } }
        public int NumColumns { get { return _rows[0].Dimension; } }
        public bool IsZero
        {
            get
            {
                bool ret = true;
                for (int r = 0; r < _rows.Length; r++)
                {
                    Vector row = _rows[r];
                    for (int c = 0; c < row.Dimension; c++)
                    {
                        if (_rows[r][c] != 0)
                        {
                            ret = false;
                            break;
                        }
                    }
                }
                return ret;
            }
        }

        public bool IsIdentity
        {
            get {
                bool yes = true;
                for (int r = 0; r < _rows.Length; r++)
                {
                    Vector row = _rows[r];
                    for (int c = 0; c < row.Dimension; c++)
                    {
                        if ((r != c && _rows[r][c] != 0) || (r == c && _rows[r][c] != 1))
                        {
                            yes = false;
                            break;
                        }
                    }
                }
                return yes;
            }
        }
        public Vector this[int index] { get { return _rows[index]; } }
        public Vector Row(int index) { return _rows[index]; }
        public Vector Column(int index)
        {
            double[] col = new double[_rows.Length];
            for (int r = 0; r < _rows.Length; r++)
			{
                col[r] = _rows[r][index];
			}
            return new Vector(col);
        }

        public void SetRow(int index, Vector row)
        {
            throw new NotImplementedException();
        }

        public void SetColumnn(int index, Vector column)
        {
            throw new NotImplementedException();
        }

        public double Trace
        {
            get { throw new NotImplementedException(); }
        }

        public Matrix Inverse()
        {
            throw new NotImplementedException();
        }

        public Matrix Transpose()
        {
            throw new NotImplementedException();
        }

        public void Scale(Vector v)
        {
            // matrix has to be square and dimension match the passed vector
            if (_rows.Length != _rows[0].Dimension || _rows.Length != v.Dimension)
            {
                throw new Exception("Invalid operation");
            }
            for (int i = 0; i < _rows.Length; i++)
            {
                _rows[i][i] *= v[i];
            }
        }

        static public Matrix operator *(Matrix m1, Matrix m2) { throw new NotImplementedException(); }
        static public Matrix operator *(Matrix m1, double s) { throw new NotImplementedException(); }
        static public Matrix operator /(Matrix m1, Matrix m2) { throw new NotImplementedException(); }
        static public Matrix operator /(Matrix m1, double s) { throw new NotImplementedException(); }
        static public Matrix operator +(Matrix m1, Matrix m2) { throw new NotImplementedException(); }
        static public Matrix operator -(Matrix m1, Matrix m2) { throw new NotImplementedException(); }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int r = 0; r < _rows.Length; r++)
            {
                Vector row = _rows[r];
                /*for (int c = 0; c < row.Dimension ; c++)
                {
                    sb.Append(row[c]);
                    sb.Append("\t\t");
                }*/
                sb.Append(row.ToString());
                sb.AppendLine(@"\par");
            }
            return sb.ToString();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        Vector[] _rows;
        #endregion
    }

}
