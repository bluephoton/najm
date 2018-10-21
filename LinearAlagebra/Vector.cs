using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.LinearAlagebra
{
    public class Vector : Tensor
    {
        public Vector(int dimention)
        {
            _elements = new double[dimention];
        }

        public Vector(double i, double j, double k)
        {
            _elements = new double[3] { i, j, k };
        }

        public Vector(double[] elements)
        {
            _elements = new double[elements.Length];
            Assign(elements);
        }

        public void Assign(double[] row)
        {
            Array.Copy(row, _elements, row.Length);
        }

        public int Dimension { get { return _elements.Length; } }
        public double this[int index]
        {
            get
            {
                if (index < 0 || index > _elements.Length)
                {
                    throw new IndexOutOfRangeException("Vector indexer");
                }
                return _elements[index];
            }
            set
            {
                if (index < 0 || index > _elements.Length)
                {
                    throw new IndexOutOfRangeException("Vector indexer");
                }
                _elements[index] = value;
            }
        }

        public Matrix ToDiagonal()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int r = 0; r < _elements.Length; r++)
            {
                sb.Append(string.Format("{0:E}", _elements[r])).Append(r == _elements.Length - 1 ? "" : @",\tab ");
            }
            sb.Append("]");
            return sb.ToString();
        }

        private double[] _elements;
    }
}
