using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Imaging
{
    interface IAnnotation
	{
        void Draw(Graphics g);
    }

    class TextLabel : IAnnotation
    {
        public TextLabel(string text, PointF location)
        {
            _text = text;
            _location = location;
        }

        public Font Font
        {
            set { _font = value; }
        }

        public void Draw(Graphics g)
        {
            Font f;
            if (_font != null)
            {
                f = _font;
            }
            else
            {
                f = new Font("Ms Sans Serif", 10);
            }
            SizeF s = g.MeasureString(_text, f);
            g.FillRectangle(Brushes.Gray, _location.X, _location.Y, s.Width, s.Height);
            g.DrawString(_text, f, Brushes.Black, _location.X, _location.Y);
        }

        private string _text;
        private Font _font;
        PointF _location;
    }
}
