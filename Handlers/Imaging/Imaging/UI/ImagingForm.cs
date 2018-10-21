using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using Najm.FITSIO;
using Najm.ImagingCore.ColorMaps;
using Najm.ImagingCore.ColorScaling;
using Najm.ImagingCore.ColorTables;
using Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ImageHandler
{
    internal enum PlayDirection { None, Forward, Backward }

    internal partial class ImagingForm : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        public ImagingForm(IModel model)
        {
            InitializeComponent();
            _model = model;

            // icon
            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Imaging.ToolstriptImage.png");
            Bitmap b = new Bitmap(s);
            _icon = System.Drawing.Icon.FromHandle(b.GetHicon());
            this.Icon = _icon;
            
            // add Najm image box
            _pictureBox1 = new NajmImageBox();
            _pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            _pictureBox1.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(_pictureBox1);
            this.KeyUp += new KeyEventHandler(ImagingForm_KeyUp);

            InitializeImageCubeToolstrip();

            // adjust toolstrip controls
            _currentSliceNumericDropDown.Minimum = 0;
            _currentSliceNumericDropDown.Maximum = model.HDU.DataMngr.NumSlices - 1;
            if (model.HDU.DataMngr.NumSlices > 1)
            {
                imageCubeToolStrip.Visible = true;
            }

            _model.ImageChanged += Model_ImageChanged;
        }

        private void InitializeImageCubeToolstrip()
        {
            // create cubetoolstrip numeric bontrols
            _currentSliceNumericDropDown = new NumericUpDown();
            _currentSliceNumericDropDown.Width = 50;    // explicitly set it
            _currentSliceNumericDropDown.ValueChanged += new EventHandler(CurrentSliceNumericDropDown_ValueChanged);
            imageCubeToolStrip.Items.Insert(2, new ToolStripControlHost(_currentSliceNumericDropDown));

            _slicePeriodNumericDropDown = new NumericUpDown();
            _slicePeriodNumericDropDown.Width = 80;
            _slicePeriodNumericDropDown.Minimum = 50;
            _slicePeriodNumericDropDown.Maximum = 1000;
            _slicePeriodNumericDropDown.Value = 250;
            _slicePeriodNumericDropDown.ValueChanged += new EventHandler(SlicePeriodNumericDropDown_ValueChanged);
            imageCubeToolStrip.Items.Insert(6, new ToolStripControlHost(_slicePeriodNumericDropDown));

            _playDirection = PlayDirection.None;
        }

        void ImagingForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyImageInView();
            }
        }

        private void ImagingForm_Load(object sender, EventArgs e)
        {
            Text = String.Format("{0} - {1}[{2}]", _model.HDU.File.Name, _model.HDU.Name, _model.HDU.Type);
            _pictureBox1.Image = _model.Bitmap;
        }

        void ImagingForm_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // TODO: better to restrict it more to the picturebox rectangle
            if(ClientRectangle.Contains(e.Location))
            {
                if (e.Delta > 0)
                {
                    _pictureBox1.ZoomIn();
                }
                else if (e.Delta < 0)
                {
                    _pictureBox1.ZoomOut();
                }
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (_model.HDU != null)
            {
                string filename = _model.HDU.File.Name;
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = new FileInfo(string.IsNullOrEmpty(filename) ? "" : filename).Name + ".png";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _pictureBox1.Image.Save(sfd.FileName, ImageFormat.Png);
                }
            }

        }

        private void actualSizeToolStripButton_Click(object sender, EventArgs e)
        {
            if (_model.Bitmap != null)
            {
                // I also add non client stuff, which is important
                Width = _model.Bitmap.Width + Width - ClientRectangle.Width;
                Height = _model.Bitmap.Height + imagingToolStrip.Height + Height - ClientRectangle.Height;
                _pictureBox1.Reset();
            }
        }

        internal IModel Model
        {
            get { return _model; }
            set { _model = value; }
        }

        internal bool Activating
        {
            set { _activating = value; }
            get { return _activating; }
        }

        private void CopyToolStripButton_Click(object sender, EventArgs e)
        {
            CopyImageInView();
        }

        private void CopyImageInView()
        {
            Image i = _pictureBox1.GetImageInView();
            Clipboard.SetImage(i);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region image cube
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void SlicePeriodNumericDropDown_ValueChanged(object sender, EventArgs e)
        {
            playTimer.Interval = (int)_slicePeriodNumericDropDown.Value;
        }

        void CurrentSliceNumericDropDown_ValueChanged(object sender, EventArgs e)
        {
            _model.CurrentSlice = (int)_currentSliceNumericDropDown.Value;
        }

        private void firstSliceToolStripButton_Click(object sender, EventArgs e)
        {
            if (_playDirection == PlayDirection.None)
            {
                _currentSliceNumericDropDown.Value = 0;
                _model.FirstSlice();
            }
        }

        private void lastSliceToolStripButton_Click(object sender, EventArgs e)
        {
            if (_playDirection == PlayDirection.None)
            {
                _currentSliceNumericDropDown.Value = _model.HDU.DataMngr.NumSlices - 1;
                _model.LastSlice();
            }
        }

        private void playBackwardToolStripButton_Click(object sender, EventArgs e)
        {
            if (_model.HDU.DataMngr.NumSlices > 1)
            {
                switch (_playDirection)
                {
                    case PlayDirection.None:
                        // tell model that we are playing to prevent histogram update - updating histogram while playing will
                        // slow things dramatically!
                        _model.Playing = true;
                        _playDirection = PlayDirection.Backward;
                        playBackwardToolStripButton.Image = global::Imaging.Properties.Resources.Stop;
                        playTimer.Start();
                        break;
                    case PlayDirection.Forward:
                        break;
                    case PlayDirection.Backward:
                        playTimer.Stop();
                        _playDirection = PlayDirection.None;
                        playBackwardToolStripButton.Image = global::Imaging.Properties.Resources.PlayBackward;
                        _model.Playing = false;
                        break;
                }
            }
        }

        private void playToolStripButton_Click(object sender, EventArgs e)
        {
            if (_model.HDU.DataMngr.NumSlices > 1)
            {
                switch (_playDirection)
                {
                    case PlayDirection.None:
                        // tell model that we are playing to prevent histogram update - updating histogram while playing will
                        // slow things dramatically!
                        _model.Playing = true;
                        _playDirection = PlayDirection.Forward;
                        playToolStripButton.Image = global::Imaging.Properties.Resources.Stop;
                        playTimer.Start();
                        break;
                    case PlayDirection.Forward:
                        playTimer.Stop();
                        _playDirection = PlayDirection.None;
                        playToolStripButton.Image = global::Imaging.Properties.Resources.Play;
                        _model.Playing = false;
                        break;
                    case PlayDirection.Backward:
                        break;
                }
            }
        }

        private void playTimer_Tick(object sender, EventArgs e)
        {
            if (_playDirection == PlayDirection.Forward)
            {
                if (_model.CurrentSlice >= _model.HDU.DataMngr.NumSlices - 1)
                {
                    _model.FirstSlice();
                }
                else
                {
                    _model.CurrentSlice++;
                }
            }
            else if (_playDirection == PlayDirection.Backward)
            {
                // check first, current slice is unsigned and can't be negative.
                if (_model.CurrentSlice == 0)
                {
                    _model.LastSlice();
                }
                else
                {
                    _model.CurrentSlice--;
                }
            }
            _currentSliceNumericDropDown.Value = _model.CurrentSlice;
        }
        
        private SessionData _sessionData;
        internal SessionData SessionData { get { return _sessionData; } set { _sessionData = value; } }

        public void Model_ImageChanged()
        {
            _pictureBox1.Image = _model.Bitmap;
        }

        private void ImagingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DestroyIcon(_icon.Handle);
        }

        #endregion
        private bool _activating;
        private NajmImageBox _pictureBox1;
        private NumericUpDown _currentSliceNumericDropDown;
        private NumericUpDown _slicePeriodNumericDropDown;
        private PlayDirection _playDirection;
        private IModel _model;
        private Icon _icon;
    }
}
