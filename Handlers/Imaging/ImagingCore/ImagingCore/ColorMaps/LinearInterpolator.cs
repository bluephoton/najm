using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Najm.ImagingCore.ColorMaps
{
    class LinearInterpolator
    {
        private class point 
        {
            public point(double x, double y) { _x = x; _y = y; }
            public double _x; 
            public double _y;
        }

        public LinearInterpolator(string s)
        {
            try
            {
                if (string.IsNullOrEmpty(s))
                {
                    // TODO: well, I don't mind now!
                }
                _points = new List<point>(10);
                while (!string.IsNullOrEmpty(s))
                {
                    point p = GetPoint(ref s);
                    _points.Add(p);
                }
            }
            catch
            {
                //TODO: eat it for now!
            }
        }

        public double Interpolate(double x)
        {
            double y = 0;
            point before = null;
            point after = null;
            foreach (point p in _points)
            {
                if (p._x >= x)
                {
                    after = p;
                    break;
                }
                else
                {
                    before = p;
                }
            }

            if (before != null && after != null)
            {
                double slope = (after._y - before._y)/(after._x - before._x);
                if (slope == 0)
                {
                    // just optimization
                    y = before._y;
                }
                else
                {
                    y = before._y + (x - before._x) * slope;
                }
            }
            else if (before != null)
            {
                y = before._y;
            }
            else if (after != null)
            {
                y = after._y;
            }
            else
            {
                // TODO: do we need to do something terrible here?!
                ;
            }
            return y;
        }

        private point GetPoint(ref string s)
        {
            // remove spaces if any
            s = s.Trim();
            // check we start with (
            if (!s.StartsWith("("))
            {
                InvalidFormat();
            }

            // we have to have a closing )
            int i = s.IndexOf(")");
            if (i < 0)
            {
                InvalidFormat();
            }
            // isolate x y and remove ( & )
            string ss = s.Substring(0, i).Substring(1);
            // remove this point from s
            s = s.Substring(i + 1);
            // get the two comma separated coordinates
            string[] xy = ss.Split(new char[]{','});
            if(xy.Length != 2)
            {
                InvalidFormat();
            }
            double x,y;
            // get x
            if (!double.TryParse(xy[0].Trim(), out x))
            {
                InvalidFormat();
            }
            // get y
            if (!double.TryParse(xy[1].Trim(), out y))
            {
                InvalidFormat();
            }
            return new point(x, y);
        }

        private void InvalidFormat()
        {
            throw new FormatException("Invalid format");
        }

        private List<point> _points;
    }
}
