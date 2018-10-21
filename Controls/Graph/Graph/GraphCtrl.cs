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
    public delegate void LowValueChanged(object sender, double value);
    public delegate void HighValueChanged(object sender, double value);
    public delegate void VerticalMarkerReleased(object sender, double min, double max);

    public partial class GraphCtrl : UserControl
    {
        private enum MarkHit { None, Low, High };
        public GraphCtrl()
        {
            InitializeComponent();
            
            _axisColor = Color.Blue;
            _origin = new PointF(5F, 5F);
            
            // init defaults
            _drawAxes = true;
            _axisColor = Color.Black;
            _axisThickness = 0.5F; //mm
            _axesOffset = 5.0F;

            _showGrid = true;
            GridSpacing = 5F - 0.25F; //mm,  subtracting the line thikness so all add to 5 mm. this is to ensure the axis is drawn on the grid - just cosmetics!
            GridColor = Color.Gray;
            GridLineThickness = 0.25F;
            _logYScale = false;

            _minX = _maxX = _minY = _maxY = 0;

            _markHit = MarkHit.None;
        }

        private void GraphCtrl_Load(object sender, EventArgs e)
        {
            InitializeGraphics();

            Cursor = Cursors.Cross;
        }

        private void InitializeGraphics()
        {
            // get the size in both pixels and millimeters and use them to calibrate the unit converter
            // which will be used by other components when they need to convert units.
            Location size = MapToWorld(ClientRectangle.Width, ClientRectangle.Height);
            UnitsConverter.Calibrate(new Location(ClientRectangle.Width, ClientRectangle.Height), size);

            // create grid bitmap
            CreateGridBitmap();

            // this line is necessary to prevent flicker
            if (!this.DesignMode)   // noticed that in design mode we need to erase background or 
            // things will be overwritten and get messy!
            {
                SetStyle(ControlStyles.Opaque, true);
            }
        }

        private void CreateGridBitmap()
        {
            float axesOffset = _drawAxes ? _axesOffset : 0;

            // create the bitmap object
            _gridBitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(_gridBitmap);
            g.PageUnit = GraphicsUnit.Millimeter;

            // clear its background
            Brush br = new SolidBrush(BackColor);
            g.FillRectangle(br, 0F, 0F, _gridBitmap.Width, _gridBitmap.Height);

            _origin = new PointF(axesOffset, UnitsConverter.Pixel2MM(ClientRectangle.Bottom) - axesOffset);
            // I'm subtracting 1 from width as the 3d frame of the control overrites the last pixel (odd! it shouldn't be part of the client area
            // but this what i noticed). This issue started to show up once i changed to 3D frame and wasn't these with thin frame.
            _drawingArea = new RectangleF(axesOffset, 0, UnitsConverter.Pixel2MM(ClientRectangle.Width - 1) - axesOffset, UnitsConverter.Pixel2MM(ClientRectangle.Height) - axesOffset);
            int axesOffsetPixels = UnitsConverter.MM2Pixel(axesOffset);
            _drawingAreaPixels = new Rectangle(axesOffsetPixels, 0, ClientRectangle.Width - axesOffsetPixels - 1, ClientRectangle.Height - axesOffsetPixels);

            // draw 2 axes
            if (_drawAxes)
            {
                Pen axisPen = new Pen(AxisColor, AxisThickness);
                g.DrawLine(Pens.Black, UnitsConverter.Pixel2MM(ClientRectangle.Left), UnitsConverter.Pixel2MM(ClientRectangle.Bottom) - axesOffset + AxisThickness, UnitsConverter.Pixel2MM(ClientRectangle.Right), UnitsConverter.Pixel2MM(ClientRectangle.Bottom) - axesOffset + AxisThickness);
                g.DrawLine(Pens.Black, UnitsConverter.Pixel2MM(ClientRectangle.Left) + axesOffset - AxisThickness, UnitsConverter.Pixel2MM(ClientRectangle.Top), UnitsConverter.Pixel2MM(ClientRectangle.Left) + axesOffset - AxisThickness, UnitsConverter.Pixel2MM(ClientRectangle.Bottom));
            }

            // draw grid. I draw starting from axes so I'm sure both axis wil be snapped to the grid
            Pen p = new Pen(GridColor, GridLineThickness);
            float x = _drawingArea.Left;
            while (x <= _drawingArea.Right)
            {
                x += GridSpacing;
                g.DrawLine(p, x, 0, x, _gridBitmap.Height);
            }
            if (!_logYScale)
            {
                float y = _drawingArea.Bottom;
                while (y > _drawingArea.Top)
                {
                    y -= GridSpacing;
                    g.DrawLine(p, 0, y, _drawingArea.Right, y);                    
                }
            }
            else
            {
                float decades = 4;
                float dy = _drawingArea.Height / decades;
                for (float yy = _drawingArea.Bottom; yy > _drawingArea.Top; yy -= dy)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        float y = yy - (float)Math.Log10(i) * dy;
                        g.DrawLine(p, 0, y, _drawingArea.Right, y);
                    }
                }
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

        private void InitalizeScales()
        {
            // ensure we have data
            if (DataAvailable)
            {
                // calculate min/max values. we know the x vector is already sorted
                InitXScale(_xVector[0], _xVector[_xVector.Length - 1]);

                InitYScale();
            }
        }

        private void InitXScale(double min, double max)
        {
            _minX = min; _maxX = max;

            // Calculate how much datapoint to skip if data range is larger than number of horizontal ticks
            _xScale = _drawingAreaPixels.Width / (_maxX - _minX);

            // reset vertical marks
            _lowVerticalMark = _drawingAreaPixels.Left;
            _highVerticalMark = _drawingAreaPixels.Right;
        }

        private void InitYScale()
        {
            CalcMinMax(_yVector, ref _minY, ref _maxY);

            // calculate the y scale factor
            // 0.8 to keep some clearance at the top of the graph for better visual.
            _yScale = _drawingAreaPixels.Height / Math.Log10(_maxY) * 0.9;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.PageUnit = GraphicsUnit.Pixel;
            e.Graphics.DrawImage(_gridBitmap, 0, 0);
            e.Graphics.DrawLine(Pens.Lime, _lowVerticalMark, _drawingAreaPixels.Top, _lowVerticalMark, _drawingAreaPixels.Bottom);
            e.Graphics.DrawLine(Pens.Red, _highVerticalMark, _drawingAreaPixels.Top, _highVerticalMark, _drawingAreaPixels.Bottom);
            // not interested on original control's painting!
            //base.OnPaint(e);
        }

        private void DrawGraph()
        {
            if (DataAvailable)
            {
                Graphics g = Graphics.FromImage(_gridBitmap);
                g.PageUnit = GraphicsUnit.Pixel;

                int xo = UnitsConverter.MM2Pixel(_drawingArea.Left);
                int yo = UnitsConverter.MM2Pixel(_drawingArea.Bottom);
                for (int i = 0; i < _xVector.Length; i++)
                {
                    int x = (int)((_xVector[i] - _xVector[0]) * _xScale);
                    if (_yVector[i] > 0)    // binned histogram might have zero frequencies
                    {
                        int y = (int)(_yScale * Math.Log10(_yVector[i]));
                        g.DrawLine(new Pen(ForeColor), xo + x, yo, xo + x, yo - y);
                    }
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region low/high marks
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void GraphCtrl_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsMarkHit(_lowVerticalMark, e.X))
            {
                _markHit = MarkHit.Low;
            }
            else if (IsMarkHit(_highVerticalMark, e.X))
            {
                _markHit = MarkHit.High;
            }
        }

        private bool IsMarkHit(int xMark, int x)
        {
            int d = DistanceFromMark(xMark, x);
            return d < 2;   // 2 pixels seems reasonable
        }

        private int DistanceFromMark(int xMark, int x)
        {
            return (x > xMark) ? (x - xMark) : (xMark - x);
        }

        private void GraphCtrl_MouseMove(object sender, MouseEventArgs e)
        {
            int x;
            switch (_markHit)
            {
                case MarkHit.None:
                    break;
                case MarkHit.Low:
                    // clip the value to ensure it has value between (_drawingAreaPixels.Left, _highVerticalMark - 1)
                    x = (e.X < _highVerticalMark) ? e.X : _highVerticalMark - 1;
                    x = (x >= _drawingAreaPixels.Left) ? x : _drawingAreaPixels.Left;
                    InvalidateVerticalMarkRegion(ref _lowVerticalMark, x);
                    if (LowValueChanged != null)
                    {
                        LowValueChanged(this, PixelToPhysical(x - _drawingAreaPixels.Left));
                    }
                    break;
                case MarkHit.High:
                    // clip the value to ensure it has value between (_lowVerticalMark + 1, _drawingAreaPixels.Right)
                    x = (e.X > _lowVerticalMark) ? e.X : _lowVerticalMark + 1;
                    x = (x <= _drawingAreaPixels.Right) ? x : _drawingAreaPixels.Right;
                    InvalidateVerticalMarkRegion(ref _highVerticalMark, x);
                    if (HighValueChanged != null)
                    {
                        HighValueChanged(this, PixelToPhysical(x - _drawingAreaPixels.Left));
                    }
                    break;
                default:
                    break;
            }
        }

        private void InvalidateVerticalMarkRegion(ref int oldX, int newX)
        {
            Rectangle invalidRect = new Rectangle(
                                        (newX < oldX ? newX : oldX) - 1,
                                        _drawingAreaPixels.Top,
                                        DistanceFromMark(oldX, newX) + 2,
                                        _drawingAreaPixels.Height);
            oldX = newX;
            Invalidate(invalidRect);
        }

        private double PixelToPhysical(int pixelValue)
        {
            double val = _minX + pixelValue / _xScale;
            // clip the value for to ensure its in range
            val = val > _maxX ? _maxX : val;
            val = val < _minX ? _minX : val;
            return val;
        }

        private void GraphCtrl_MouseUp(object sender, MouseEventArgs e)
        {
            if (_markHit != MarkHit.None)
            {
                _markHit = MarkHit.None;
                if (VerticalMarkerReleased != null)
                {
                    VerticalMarkerReleased(this, PixelToPhysical(_lowVerticalMark), PixelToPhysical(_highVerticalMark));
                }
            }
        }
        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region properties and setters
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PointF Origin { get { return _origin; } set { _origin = value; } }
        public void SetXRange(double min, double max) { _minX = min; _maxX = max; }
        public void SetYRange(double min, double max) { _minY = min; _maxY = max; }
        public bool LogX { get { return _logX; } set { _logX = value; } }
        public bool LogY { get { return _logY; } set { _logY = value; } }

        [Description("Show/hide the grid lines"), Category("Grid")]
        public bool ShowGrid { get { return _showGrid; } set { _showGrid = value; } }

        [Description("The color used to draw the grid"), Category("Grid")]
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
        [Description("The distance between grid lines for linear grid"), Category("Grid")]
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

        [Description("thikness (in mm) of grid line"), Category("Grid")]
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
        [Description("Logarithmic spaced horizontal grid lines"), Category("Grid")]
        public bool LogYScale
        {
            get { return _logYScale; }
            set
            {
                _logYScale = value;
                Invalidate();
            }
        }

        [Description("Show or hide axes"), Category("Axes")]
        public bool ShowAxes
        {
            get { return _drawAxes; }
            set { _drawAxes = value; Invalidate(); }
        }

        [Description("offset of both axis from the endjes of the control"), Category("Axes")]
        public float AxesOffset
        {
            get
            {
                return _axesOffset;
            }
            set
            {
                _axesOffset = value;
                Invalidate();
            }
        }
        [Description("Color used to draw the axes"), Category("Axes")]
        public Color AxisColor
        {
            get { return _axisColor; }
            set
            {
                _axisColor = value;
                Invalidate();
            }
        }

        [Description("Thickness (in mm) of axes"), Category("Axes")]
        public float AxisThickness
        {
            get { return _axisThickness; }
            set
            {
                _axisThickness = value;
                Invalidate();
            }
        }
        public void SetData(double[] xVector, double[] yVector)
        {
            _xVector = xVector;
            _yVector = yVector;
            // initialize scaling factors
            InitalizeScales();
            // re create grid as we draw graph on top of it
            CreateGridBitmap();
            // draw the graph
            DrawGraph();
            // redraw
            Invalidate();
        }

        public void SetDataLimits(double min, double max)
        {
            InitXScale(min, max);
            // re create grid as we draw graph on top of it
            CreateGridBitmap();
            // draw the graph
            DrawGraph();
            // redraw
            Invalidate();
        }

        public double LowValue
        {
            set
            {
                int newMarkPos = _drawingAreaPixels.Left + (int)(_drawingAreaPixels.Width * (value - _minX) / (_maxX - _minX));
                InvalidateVerticalMarkRegion(ref _lowVerticalMark, newMarkPos);
            }
        }

        public double HighValue
        {
            set
            {
                int newMarkPos = _drawingAreaPixels.Left + (int)(_drawingAreaPixels.Width * (value - _minX) / (_maxX - _minX));
                InvalidateVerticalMarkRegion(ref _highVerticalMark, newMarkPos);
            }
        }
        #endregion

        private bool DataAvailable { get { return (_xVector != null && _xVector.Length > 0 && _yVector != null && _yVector.Length > 0); } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region min/max calculations
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void CalcMinMax(double[] vector, ref double min, ref double max)
        {
            if (min == 0 && max == 0)
            {
                min = max = vector[0];
                for (int i = 0; i < vector.Length; i++)
                {
                    if (vector[i] < min)
                    {
                        min = vector[i];
                    }
                    if (vector[i] > max)
                    {
                        max = vector[i];
                    }
                }
            }
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private Bitmap _gridBitmap;
        private PointF _origin;
        private RectangleF _drawingArea;
        private Rectangle _drawingAreaPixels;
        // axes
        private bool _drawAxes;
        private float _axesOffset;
        private Color _axisColor;
        private float _axisThickness;
        private bool _logX, _logY;
        // grid
        private Color _gridColor;
        private float _gridLineThickness;
        private float _gridSpacing;
        private bool _showGrid;
        private bool _logYScale;
        // graph data
        private double[] _xVector, _yVector;
        private double _minX, _maxX;
        private double _minY, _maxY;

        private double _xScale;
        private double _yScale;

        private int _lowVerticalMark;
        private int _highVerticalMark;
        private MarkHit _markHit;
        public event LowValueChanged LowValueChanged;
        public event HighValueChanged HighValueChanged;
        public event VerticalMarkerReleased VerticalMarkerReleased;
        #endregion
    }
}
