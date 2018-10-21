using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Najm.Controls
{
    class Location
    {
        public Location(float x, float y)
        {
            _x = x;
            _y = y;
        }
        public float X
        {
            get { return _x; }
            set { _x = value; }
        }

        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public void Shift(float dx, float dy)
        {
            _x += dx;
            _y += dy;
        }

        public float DistanceFrom(Location l)
        {
            float d = (float) Math.Sqrt((_x - l.X) * (_x - l.X) + (_y - l.Y) * (_y - l.Y));
            return d;
        }

        public void Scale(float s)
        {
            _x *= s;
            _y *= s;
        }

        public PointF Point
        {
            get { return new PointF(_x,_y);}
        }

        private float _x;
        private float _y;
    }
}
