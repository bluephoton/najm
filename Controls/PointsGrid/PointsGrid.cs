using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Najm.Controls
{
    public partial class PointsGridControl : UserControl
    {
        public delegate void PointListChanged(string points);
        public PointsGridControl()
        {
            InitializeComponent();

            _liPoints = new PointsList();

            // init defaults
            _axisColor = Color.Black;
            _axisThickness = 0.5F; //mm

            GridSpacing = 5F; //mm
            GridColor = Color.Gray;
            GridLineThickness = 0.25F;

            FillColor = Color.Red;
            PointColor = Color.Blue;
            EnvelopeColor = Color.Black;
        }

        private void UserControl1_MouseDown(object sender, MouseEventArgs e)
        {
            PointF p = new PointF(UnitsConverter.Pixel2MM(e.X), UnitsConverter.Pixel2MM(e.Y));
            if (e.Button == MouseButtons.Left)
            {
                _selectedPoint = _liPoints.FindPoint(p);
                if (_selectedPoint < 0)
                {
                    // we didn't click an already existing point, we want to create new one
                    _currentPointClicked = p;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                _liPoints.RemovePoint(p);
                Invalidate();
                NotifyListeners();
            }
        }

        private void NotifyListeners()
        {
            if (OnPointListChanged != null)
            {
                OnPointListChanged(_liPoints.Points);
            }
        }

        private void UserControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_selectedPoint >= 0)
            {
                PointF to = new PointF(UnitsConverter.Pixel2MM(e.X), UnitsConverter.Pixel2MM(e.Y));
                if (_drawingArea.Contains(to) && _liPoints.IsPointTrapped(to, _selectedPoint))
                {
                    _liPoints.MovePoint(_selectedPoint, to);
                    Invalidate();
                    NotifyListeners();
                }
            }
            // moving the mouse cancels the add point functionality
            _currentPointClicked = PointF.Empty;
        }

        private void UserControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_currentPointClicked.IsEmpty)
            {
                if (!_drawingArea.Contains(_currentPointClicked))
                {
                    _currentPointClicked = FixPoint(_currentPointClicked);
                }
                _liPoints.AddPoint(new PointF(_currentPointClicked.X, _currentPointClicked.Y));
                Invalidate();
                _currentPointClicked = PointF.Empty;
                NotifyListeners();
            }
            _selectedPoint = -1;
        }

        private PointF FixPoint(PointF p)
        {
            if (p.X < _drawingArea.Left)
            {
                p.X = _drawingArea.Left;
            }
            else if (p.X > _drawingArea.Right)
            {
                p.X = _drawingArea.Right;
            }
            if (p.Y < _drawingArea.Top)
            {
                p.Y = _drawingArea.Top;
            }
            else if (p.Y > _drawingArea.Bottom)
            {
                p.Y = _drawingArea.Bottom;
            }
            return p;
        }

        override protected void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = Graphics.FromImage(_finalBitmap);
            g.DrawImage(_gridBitmap, 0, 0);            
            _liPoints.Draw(g);
            e.Graphics.DrawImage(_finalBitmap, 0, 0);
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            InitializeGraphics();
        }

        private void InitializeGraphics()
        {
            // get the size in both pixels and millimeters and use them to calibrate the unit converter
            // which will be used by other components when they need to convert units.
            Location size = MapToWorld(ClientRectangle.Width, ClientRectangle.Height);
            UnitsConverter.Calibrate(new Location(ClientRectangle.Width, ClientRectangle.Height), size);

            // create grid bitmap
            CreateGridBitmap();

            // create final bitmap
            _finalBitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppArgb);

            _liPoints.DrawingArea = _drawingArea;

            _currentPointClicked = PointF.Empty;
            _selectedPoint = -1;

            // this line is necessary to prevent flicker
            if (!this.DesignMode)   // noticed that in design mode we need to erase background or 
            // things will be overwritten and get messy!
            {
                SetStyle(ControlStyles.Opaque, true);
            }
        }

        private void CreateGridBitmap()
        {
            float axesOffset = 5F;

            // create the bitmap object
            _gridBitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(_gridBitmap);
            g.PageUnit = GraphicsUnit.Millimeter;

            // clear its background
            Brush br = new SolidBrush(BackColor);
            g.FillRectangle(br, 0F, 0F, _gridBitmap.Width, _gridBitmap.Height);

            _origin = new PointF(axesOffset, UnitsConverter.Pixel2MM(ClientRectangle.Bottom) - axesOffset);
            _drawingArea = new RectangleF(axesOffset, 0, UnitsConverter.Pixel2MM(ClientRectangle.Width) - axesOffset, UnitsConverter.Pixel2MM(ClientRectangle.Height) - axesOffset);

            // draw 2 axes
            Pen axisPen = new Pen(AxisColor, AxisThickness);
            g.DrawLine(Pens.Black, UnitsConverter.Pixel2MM(ClientRectangle.Left), UnitsConverter.Pixel2MM(ClientRectangle.Bottom) - axesOffset + AxisThickness, UnitsConverter.Pixel2MM(ClientRectangle.Right), UnitsConverter.Pixel2MM(ClientRectangle.Bottom) - axesOffset + AxisThickness);
            g.DrawLine(Pens.Black, UnitsConverter.Pixel2MM(ClientRectangle.Left) + axesOffset - AxisThickness, UnitsConverter.Pixel2MM(ClientRectangle.Top), UnitsConverter.Pixel2MM(ClientRectangle.Left) + axesOffset - AxisThickness, UnitsConverter.Pixel2MM(ClientRectangle.Bottom));

            // draw grid
            Pen p = new Pen(GridColor, GridLineThickness);
            float x = axesOffset;
            while (x <= _drawingArea.Right)
            {
                x += GridSpacing;
                g.DrawLine(p, x, 0, x, _gridBitmap.Height);
            }
            float y = AxisThickness;
            while (y < _drawingArea.Bottom)
            {
                y += GridSpacing;
                g.DrawLine(p, 0, y, _drawingArea.Right, y);
            }
        }

        private Location MapToWorld(float x, float y)
        {
            // create graphics to get properties
            Graphics g = CreateGraphics();
            float DpiX = g.DpiX;
            float DpiY = g.DpiY;
            g.Dispose();
            // map to world coordinates
            float xScalingFactor = 1.0F / (DpiX / 25.4F);
            float yScalingFactor = 1.0F / (DpiY / 25.4F);
            return new Location(
                 (x - AutoScrollPosition.X) * xScalingFactor,
                 (y - AutoScrollPosition.Y) * yScalingFactor);
        }

        public Color AxisColor
        {
            get { return _axisColor; }
            set
            {
                _axisColor = value;
                Invalidate();
            }
        }

        public float AxisThickness
        {
            get { return _axisThickness; }
            set 
            {
                _axisThickness = value;
                Invalidate();
            }
        }

        public Color FillColor
        {
            get { return _fillColor; }
            set
            {
                _fillColor = value;
                _liPoints.FillBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        public Color PointColor
        {
            get { return _pointColor; }
            set
            {
                _pointColor = value;
                _liPoints.PointPen = new Pen(value, 0.5F);
                Invalidate();
            }
        }

        public Color EnvelopeColor
        {
            get { return _envelopeColor; }
            set 
            { 
                _envelopeColor = value;
                _liPoints.EnvelopPen = new Pen(value, 0.5F);
                Invalidate();
            }
        }

        public Color GridColor
        {
            get { return _gridColor; }
            set 
            {
                _gridColor = value;
                CreateGridBitmap();
                Invalidate();
            }
        }

        public float GridSpacing
        {
            get { return _gridSpacing; }
            set
            {
                _gridSpacing = value;
                CreateGridBitmap();
                Invalidate();
            }
        }

        public float GridLineThickness
        {
            get { return _gridLineThickness; }
            set
            {
                _gridLineThickness = value;
                CreateGridBitmap();
                Invalidate();
            }
        }

        public string Points
        {
            get { return _liPoints.Points; }
            set
            {
                if (_liPoints.Points != value)
                {
                    _liPoints.Points = value;
                    Invalidate();
                    NotifyListeners();
                }
            }
        }

        private Bitmap _gridBitmap;
        private Bitmap _finalBitmap;
        private PointF _origin;
        private RectangleF _drawingArea;
        private PointF _currentPointClicked;
        private int _selectedPoint;
        private PointsList _liPoints;
        
        // axes
        private Color _axisColor;
        private float _axisThickness;
        // grid
        private Color _gridColor;
        private float _gridLineThickness;
        private float _gridSpacing;
        // graph
        private Color _fillColor;
        private Color _pointColor;
        private Color _envelopeColor;

        public event PointListChanged OnPointListChanged;
    }
}
