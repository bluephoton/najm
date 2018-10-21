using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Najm.Controls
{
    public interface IGraph
    {
        PointF Origin { set; get; }
        double[] Data { set; }
        void SetXRange(double min, double max);
        void SetYRange(double min, double max);
        bool LogX { get; set; }
        bool LogY { get; set; }
        bool ShowGrid { get; set; }
    }

    public partial class Graph : UserControl, IGraph
    {
        public Graph()
        {
            InitializeComponent();
            _axisColor = Color.Blue;
            _origin = new PointF(5F, 5F);
            _showGrid = true;
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

            // this line is necessary to prevent flicker
            if (!this.DesignMode)   // noticed that in design mode we need to erase background or 
            // things will be overwritten and get messy!
            {
                SetStyle(ControlStyles.Opaque, true);
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IGraph<TabAlignment> Members
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public PointF Origin { get { return _origin; } set { _origin = value; } }
        public double[] Data { set { _data = value; } }
        public void SetXRange(double min, double max) { _minX = min; _maxX = max; }
        public void SetYRange(double min, double max) { _minY = min; _maxY = max; }
        public bool LogX { get { return _logX; } set { _logX = value; } }
        public bool LogY { get { return _logY; } set { _logY = value; } }
        public bool ShowGrid { get { return _showGrid; } set { _showGrid = value; } }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            // get final bitmap
            Graphics g = Graphics.FromImage(_finalBitmap);
            // draw grid on it
            g.DrawImage(_gridBitmap, 0, 0);
            // draw the curve
            ;
            // copy it to the form graphics
            e.Graphics.DrawImage(_finalBitmap, 0, 0);
            // not interested on original control's painting!
            //base.OnPaint(e);
        }

        private void DrawAxis(Graphics g)
        {
            RectangleF cr = ClientRectangle;
            // vertical
            g.DrawLine(new Pen(new SolidBrush(_axisColor), 0.5F), cr.Left + _origin.X, cr.Top, cr.Left + _origin.X, cr.Bottom);
            // Horizontal
            g.DrawLine(new Pen(new SolidBrush(_axisColor), 0.5F), cr.Left + _origin.X, cr.Top, cr.Left + _origin.X, cr.Bottom);
        }

        #region data members
        private Bitmap _gridBitmap;
        private Bitmap _finalBitmap;
        private PointF _origin;
        private RectangleF _drawingArea;
        // axes
        private Color _axisColor;
        private float _axisThickness;
        private bool _logX, _logY;
        // grid
        private Color _gridColor;
        private float _gridLineThickness;
        private float _gridSpacing;
        private bool _showGrid;
        // graph data
        private double[] _data;
        private double _minX, _maxX;
        private double _minY, _maxY;
        #endregion
    }
}
