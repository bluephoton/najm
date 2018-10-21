using System;
using System.Collections.Generic;
using System.Text;


namespace Najm.Handler.SkyMap
{/*
                ρ
                θ
                α 224
                ß 225
                Γ 226
                π 227
                Σ 228
                σ 229
                µ 230
                τ 231
                Φ 232
                Θ 233
                Ω 234
                δ 235
                ∞ 236
                φ 237
                ε 238
    */
    public class _3Vector
    {/*
        #region construction
        _3Vector()
        {
            

            _x = _y = _z = 0;
        }
        _3Vector(double x, double y, double z)
        {
            _x = x; _y = y; _z = z;
        }
        _3Vector(_3Vector v)
        {
            _x = v.X; _y = v.Y; _z = v.Z;
        }
        public static _3Vector FromSpherical(double r, double θ, double φ)
        {
            return new _3Vector(r * Math.Sin(θ) * Math.Cos(φ), r * Math.Sin(θ) * Math.Sin(φ), r * Math.Cos(θ));
        }
        public static _3Vector FromCylindrical(double ρ, double φ, double z)
        {
            return new _3Vector(ρ * Math.Cos(φ), ρ * Math.Sin(φ), z);
        }
        #endregion

        #region properties
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }
        public double this[uint index]
        {
            get
            {
                if (index > 2)
                {
                    throw new IndexOutOfRangeException();
                }
                return (index == 0) ? _x : (index == 1 ? _y : _z); 
            }
            set 
            {
                switch (index)
                {
                    case 0:
                        _x = value;
                        break;
                    case 1:
                        _y = value;
                        break;
                    case 2:
                        _z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
        public double R { get { return Math.Sqrt(_x * _x + _y * _y + _z * _z); } }
        public double R2 { get { return (_x * _x + _y * _y + _z * _z); } }
        #endregion

        #region operators
        public _3Vector operator +(_3Vector v1, _3Vector v2) { return new _3Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z); }
        public _3Vector operator -(_3Vector v1, _3Vector v2) { return new _3Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z); }
        public _3Vector operator *(_3Vector v, double c) { return new _3Vector(c * v.X, c * v.Y, c * v.Z); }
        public _3Vector operator *(double c, _3Vector v) { return new _3Vector(c * v.X, c * v.Y, c * v.Z); }
        public double operator *(_3Vector v1, _3Vector v2) { return (v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z); }
        #endregion

        #region operations
        public double Dot(_3Vector v) { return this * v; }
        public _3Vector Cross(_3Vector v)
        {
            double x = _y * v.Z - _z * v.Y;
            double y = _z * v.X - _x * v.Z;
            double z = _x * v.Y - _y * v.X;
            return new _3Vector(x, y, z);
        }
        #endregion

        #region  data members
        private double _x, _y, _z;
        #endregion
*/    }
}
