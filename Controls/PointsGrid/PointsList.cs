using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;

namespace Najm.Controls
{
    class PointsList
    {
        public PointsList()
        {
            _dirty = true;
            _points = new List<PointF>(10);
        }

        public bool AddPoint(PointF p)
        {
            bool added = false;
            PointF normalizedPoint = Normalize(p);
            int index = FindInsertionIndex(normalizedPoint);
            if (index >= 0)
            {
                _points.Insert(index, normalizedPoint);
                _dirty = true;
                added = true;
            }
            return added;
        }

        private PointF Normalize(PointF p)
        {
            return new PointF((p.X - _drawingArea.Left) / _drawingArea.Width,
                                                 (_drawingArea.Bottom - p.Y) / _drawingArea.Height);
        }

        public void RemovePoint(PointF p)
        {
            int index = FindPoint(p);   // find point will normalize the passed point, so no need to normalize it here
            if (index >= 0)
            {
                _points.RemoveAt(index);
            }
            _dirty = true;
        }

        public void MovePoint(int index, PointF to)
        {
            System.Diagnostics.Debug.Assert(index >= 0 && index < _points.Count);
            if (index >= 0 && index < _points.Count)
            {
                _points[index] = Normalize(to);
            }
        }

        public int FindPoint(PointF p)
        {
            PointF np = Normalize(p);
            int index = -1;
            for (int i = 0; i < _points.Count; i++)
            {
                float dx = np.X - _points[i].X;
                float dy = np.Y - _points[i].Y;
                float d2 = dx * dx + dy * dy;
                if (d2 < 0.0006)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public bool IsDirty { get { return _dirty; } }

        public string Points
        {
            get
            {
                StringBuilder sb = new StringBuilder(100);
                foreach (PointF p in _points)
                {
                    sb.AppendFormat("({0:F2},{1:F2})", p.X, p.Y);
                }
                return sb.ToString();
            }
            set
            {
                _points.Clear();
                while (!string.IsNullOrEmpty(value))
                {
                    PointF p = GetPoint(ref value);
                    _points.Add(p);
                }
                _dirty = true;
            }
        }

        private PointF GetPoint(ref string s)
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
            string[] xy = ss.Split(new char[] { ',' });
            if (xy.Length != 2)
            {
                InvalidFormat();
            }
            float x, y;
            // get x
            if (!float.TryParse(xy[0].Trim(), out x))
            {
                InvalidFormat();
            }
            // get y
            if (!float.TryParse(xy[1].Trim(), out y))
            {
                InvalidFormat();
            }
            return new PointF(x, y);
        }

        private void InvalidFormat()
        {
            throw new FormatException("Invalid points format");
        }

        public void Draw(Graphics g)
        {
            if (_points.Count > 0)
            {
                g.PageUnit = GraphicsUnit.Millimeter;

                PointF[] points = new PointF[_points.Count + 2];    // 2 on axis to close the polygon
                // build polygon
                int i = 0;
                foreach (PointF p in _points)
                {
                    if (i == 0)
                    {
                        // first point, add its shadow on the x axis
                        points[i++] = NormalizedToArea(new PointF(p.X, 0));
                    }
                    // add current point
                    points[i++] = NormalizedToArea(p);
                }
                // add last point's shadow on x-axis
                PointF lastPoint = _points[_points.Count - 1];
                points[i] = NormalizedToArea(new PointF(lastPoint.X, 0));

                // now fill it
                g.FillPolygon(FillBrush, points);

                // draw the envelop
                if (_points.Count >= 2)
                {
                    PointF[] envelope = new PointF[points.Length - 2];
                    Array.Copy(points, 1, envelope, 0, points.Length - 2);
                    g.DrawLines(EnvelopPen, envelope);
                }

                // draw points
                for (int j = 1; j < points.Length - 1; j++)
                {
                    PointF[] tick = new PointF[4]
                    {
                        new PointF(points[j].X - 2, points[j].Y),
                        new PointF(points[j].X    , points[j].Y - 2),
                        new PointF(points[j].X + 2, points[j].Y),
                        new PointF(points[j].X    , points[j].Y + 2)
                    };
                    g.DrawPolygon(PointPen, tick);
                }
            }
        }

        private PointF NormalizedToArea(PointF p)
        {
            return new PointF(_drawingArea.Left + _drawingArea.Width * p.X,
                                _drawingArea.Bottom - _drawingArea.Height * p.Y);
        }

        private int FindInsertionIndex(PointF p)
        {
            int i;
            for (i = 0; i < _points.Count; i++)
            {
                if (_points[i].X > p.X)
                {
                    break;
                }
                else if (_points[i].X == p.X)
                {
                    // if we have point with same x, refuse to add.
                    i = -1;
                    break;
                }
            }
            return i;
        }

        internal Pen PointPen
        {
            get
            {
                if (_pointPen == null)
                {
                    _pointPen = new Pen(Color.Orange, 0.5F);
                }
                return _pointPen;
            }
            set { _pointPen = value; }
        }

        internal Pen EnvelopPen
        {
            get
            {
                if (_envelopPen == null)
                {
                    _envelopPen = new Pen(Color.Black, 0.5F);
                }
                return _envelopPen;
            }
            set { _envelopPen = value; }
        }

        internal Brush FillBrush
        {
            get
            {
                if (_fillBrush == null)
                {
                    _fillBrush = new SolidBrush(Color.Red);
                }
                return _fillBrush;
            }
            set { _fillBrush = value; }
        }

        internal RectangleF DrawingArea
        {
            set { _drawingArea = value; }
        }

        // check if point p is trapped between the points at pintIndex-1 & pointIndex+1
        internal bool IsPointTrapped(PointF p, int pointIndex)
        {
            bool trapped = false;
            // point can't be trapped if its the only one on the drawing area!
            if (_points.Count > 1 && pointIndex >= 0 && pointIndex < _points.Count)
            {
                p = Normalize(p);
                if (pointIndex == 0)
                {
                    trapped = p.X < _points[1].X;
                }
                else if (pointIndex == _points.Count - 1)
                {
                    trapped = p.X > _points[pointIndex - 1].X;
                }
                else
                {
                    trapped = (p.X > _points[pointIndex - 1].X && p.X < _points[pointIndex + 1].X);
                }
            }
            return trapped;
        }

        private List<PointF> _points;
        private RectangleF _drawingArea;
        private bool _dirty;
        private Brush _fillBrush;
        private Pen _pointPen;
        private Pen _envelopPen;
    }
}
