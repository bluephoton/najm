using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Imaging
{
    // None     : no panning, we didn't click
    // Entered  : We clicked mouse but didn't actually move it. If we relese mouse without enter no need to do anthing.
    // Panning  : panning entered and we moved mouse.
    internal enum PanningState { None, Entered, Panning }
    public partial class NajmImageBox : Label
    {
        public NajmImageBox()
        {
            InitializeComponent();
            _image = null;
            _zoom = 1F;
            _zoomBefore = _zoom;
            float maxZoom = 10;
            float zoomSteps = 20;
            _zoomMutiplier = (float)Math.Pow(maxZoom, 1.0 / zoomSteps);
            _matrix = new Matrix();
            _imageDisplacement = new PointF(0, 0);

            _panningState = PanningState.None;
            _panningMatrix = new Matrix();

            _origCursor = null;

            this.MouseDown += new MouseEventHandler(ImagingForm_MouseDown);
            this.MouseUp += new MouseEventHandler(ImagingForm_MouseUp);
            this.MouseMove += new MouseEventHandler(NajmImageBox_MouseMove);
            this.Layout += new LayoutEventHandler(NajmImageBox_Layout);
            this.MouseEnter += new EventHandler(NajmImageBox_MouseEnter);
            this.MouseLeave += new EventHandler(NajmImageBox_MouseLeave);

            _annotations = new List<IAnnotation>(10);
        }

        void NajmImageBox_MouseLeave(object sender, EventArgs e)
        {
            //Cursor.Current = Cursors.Arrow;
        }

        void NajmImageBox_MouseEnter(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Cross;
        }

        void NajmImageBox_Layout(object sender, LayoutEventArgs e)
        {
            if (_image != null)
            {
                Reset();
            }
        }

        void ImagingForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (_image != null)
            {
                // start panning, if CTRL is not down
                if (e.Button == MouseButtons.Left && !ImageFullyInView && ModifierKeys != Keys.Control)
                {
                    _panningState = PanningState.Entered;
                    _panStartPoint = new PointF(e.X, e.Y);
                    _origCursor = Cursor.Current;
                    Cursor.Current = Cursors.Hand;
                }

                // if ctrl is down, draw rubber band
                if (ModifierKeys != Keys.Control && e.Button == MouseButtons.Left)
                {
                    //_TrackerRectagle.Begin(e.Location);
                }
            }
        }

        void ImagingForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (_image != null)
            {
                if (_panningState == PanningState.Panning)
                {
                    // since we are leaving panning, persist the final panning displacement into the current transformation matrix
                    // not doing this well result in drawing the image from its old position once we enter pannning again - since
                    // we reset the panning matrix on panning enter and loose old status.
                    _matrix.Translate(_panningMatrix.OffsetX, _panningMatrix.OffsetY, MatrixOrder.Prepend);

                    // we also need to incorporate the panning displacement into the _imageDisplacement so that our zoom algorithm
                    // work as expected. Otherwise calculated feature position will be offset from correct position.
                    _imageDisplacement.X += _panningMatrix.OffsetX * _zoom;
                    _imageDisplacement.Y += _panningMatrix.OffsetY * _zoom;

                    // now no need to this guy, reset it.
                    _panningMatrix.Reset();
                }
                _panningState = PanningState.None;

                Cursor.Current = _origCursor;

                // End rubber band if its started
                /*
                if (_TrackerRectagle.Active)
                {
                    _TrackerRectagle.End(e.Location);
                }
                */
            }
        }



        void NajmImageBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_image != null)
            {
                if (_panningState == PanningState.Entered || _panningState == PanningState.Panning)
                {
                    _panningState = PanningState.Panning;
                    float dx = e.X - _panStartPoint.X;
                    float dy = e.Y - _panStartPoint.Y;
                    _panningMatrix.Reset();
                    _panningMatrix.Translate(dx / _zoom, dy / _zoom);
                    Invalidate();
                }
                // Update rubberband rectangle
                /*
                if (_TrackerRectagle.Active)
                {
                    _TrackerRectagle.Track(e.Location);
                }
                */
            }
        }

        public new Bitmap Image
        {
            get { return _image; }
            set
            {
                _image = value;
                Reset();
            }
        }

        public void Reset()
        {
            CalculateZoomFactor();
            ZoomAtImageCenter();
            Invalidate();
        }

        public void ZoomIn()
        {
            _zoomBefore = _zoom;
            _zoom *= _zoomMutiplier;

            CalculateTransformationMatrix();

            Invalidate();
        }

        public void ZoomOut()
        {
            // don't zoom out more than necessary. keep as mucu of the image as possible inside the frame.
            if (!ImageFullyInView)
            {
                _zoomBefore = _zoom;
                _zoom /= _zoomMutiplier;

                CalculateTransformationMatrix();

                Invalidate();
            }
        }

        public float ZoomFactor
        {
            get { return _zoom; }
            set
            {
                _zoomBefore = _zoom;
                _zoom = value;
                CalculateTransformationMatrix();
                Invalidate();
            }
        }

        private bool ImageFullyInView
        {
            get
            {
                bool ret = false;
                float clientAreaWidth = (float)ClientRectangle.Width;
                float zoomedImageWidth = (float)_image.Width * _zoom;
                float clientAreaHeight = (float)ClientRectangle.Height;
                float zoomedImageHeight = (float)_image.Height * _zoom;
                if ((clientAreaWidth >= zoomedImageWidth) && (clientAreaHeight >= zoomedImageHeight))
                {
                    ret = true;
                }
                return ret;
            }
        }

        private void CalculateTransformationMatrix()
        {
            if (ImageFullyInView)
            {
                // TODO: ZoomAt(featurePosition) method should be able to handle both cases (larger and smaller than view zooming). only variable
                // _imageDisplacement need to be adjusted corretly on the boundary between the two limits. Consider doing this cleaning.

                ZoomAtImageCenter();
            }
            else
            {   // zoom keeping point undr mouse stationary
                
                // 1. calculate the position of the feature under the mouse location
                Point p = this.PointToClient(Control.MousePosition);
                PointF mousePosition = new PointF(p.X, p.Y);
                PointF featurePosition = new PointF((mousePosition.X - _imageDisplacement.X) / _zoomBefore, (mousePosition.Y - _imageDisplacement.Y) / _zoomBefore);

                ZoomAt(featurePosition);
            }
        }

        private void ZoomAtImageCenter()
        {
            PointF featurePosition = new PointF(_image.Width / 2F, _image.Height / 2F);

            // this will zoom keeping center point stationary.
            _matrix.Reset();
            _matrix.Translate(-featurePosition.X, -featurePosition.Y, MatrixOrder.Append);  // move center of the image to origin
            _matrix.Scale(_zoom, _zoom, MatrixOrder.Append);            // scale it
            _matrix.Translate(featurePosition.X, featurePosition.Y, MatrixOrder.Append);   // move it back.

            // c
            float dx = ClientRectangle.Width / 2F - featurePosition.X;
            float dy = ClientRectangle.Height / 2F - featurePosition.Y;
            _matrix.Translate(dx, dy, MatrixOrder.Append);  // Now center in the view

            float dx1 = ((float)ClientRectangle.Width - (float)_image.Width * _zoom) / 2F;
            float dy1 = ((float)ClientRectangle.Height - (float)_image.Height * _zoom) / 2F;
            _imageDisplacement.X = dx1;
            _imageDisplacement.Y = dy1;
        }

        private void ZoomAt(PointF featurePosition)
        {
            // 1. move the feature to the origin before scaling, this way scaling will not change its location. It will 
            //    maintain its location at the origin.
            // First, start with unit matrix.
            _matrix.Reset();
            // then translate
            _matrix.Translate(-featurePosition.X, -featurePosition.Y, MatrixOrder.Append);
            // now scale the image
            _matrix.Scale(_zoom, _zoom, MatrixOrder.Append);
            // 2. Translate such that the feature goes back under the cursor location. This is the location of the feature before 
            //    we change the zoom - hence _zoomBefore is used.
            _matrix.Translate((featurePosition.X * _zoomBefore + _imageDisplacement.X), 
                (featurePosition.Y * _zoomBefore + _imageDisplacement.Y), MatrixOrder.Append);

            // 3. Recalculate the displacemtn of the image left-top point from the view left-top point.
            _imageDisplacement.X = featurePosition.X * _zoomBefore + _imageDisplacement.X - featurePosition.X * _zoom;
            _imageDisplacement.Y = featurePosition.Y * _zoomBefore + _imageDisplacement.Y - featurePosition.Y * _zoom;
        }

        private void CalculateZoomFactor()
        {
            // find zoom required for image to fill the client area - if image is smaller we'll have _zoom = 1.
            //float widthZoom = (_image.Width > ClientRectangle.Width) ? (float)ClientRectangle.Width / (float)_image.Width : 1F;
            float widthZoom = (float)ClientRectangle.Width / (float)_image.Width;
            //float heightZoom = (_image.Height> ClientRectangle.Height) ? (float)ClientRectangle.Height / (float)_image.Height : 1F;
            float heightZoom = (float)ClientRectangle.Height / (float)_image.Height;
            _zoom = (widthZoom < heightZoom) ? widthZoom : heightZoom;
        }

        private void NajmImageBox_Paint(object sender, PaintEventArgs e)
        {
            if (_image != null)
            {
                Draw(e.Graphics);
            }
        }

        private void Draw(Graphics graphics)
        {
            Matrix m = new Matrix();
            m.Multiply(_matrix);
            m.Multiply(_panningMatrix);
            graphics.Transform = m;
            graphics.DrawImage(_image, 0, 0);

            // use differnet graphics for annotations to avoid matrix transformation effects, for now!
            // I should have a local bitmap, draw image on it, then dray annotations. this image will
            // be sent to the graphics object for drawing instead of _image.
            Graphics g = Graphics.FromHdc(graphics.GetHdc());
            if (_annotations.Count > 0)
            {
                foreach (IAnnotation a in _annotations)
                {
                    a.Draw(g);
                }
            }
            graphics.ReleaseHdc();
        }

        public Image GetImageInView()
        {
            Image dst = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            Graphics g = Graphics.FromImage(dst);
            Draw(g);
            g.Dispose();
            return dst;
        }

        #region members
        private Bitmap _image;
        private float _zoom;
        private float _zoomMutiplier;
        private float _zoomBefore;

        private Matrix _matrix;
        private PointF _imageDisplacement;      // relative to the origin of the view rect (the control client area)

        private PanningState _panningState;

        private PointF _panStartPoint;
        private Matrix _panningMatrix;

        private Cursor _origCursor;

        // annotations
        List<IAnnotation> _annotations;

        #endregion
    }
}
