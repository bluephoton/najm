using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Najm.ImagingCore;
using Najm.ImagingCore.ColorScaling;
using Najm.ImagingCore.ColorMaps;
using Najm.ImagingCore.ColorTables;
using Najm.FITSIO;

namespace ImageHandler
{
    internal partial class ImageHandlerUI : UserControl
    {
        public ImageHandlerUI()
        {
            InitializeComponent();

            InitColorTableUI();
        }

        private void ImageHandlerUI_Load(object sender, EventArgs e)
        {
            // Initialize panel controls to match panel state 
            if (_model != null)
            {
                UpdatePanel();
            }

            // advanced color mapping initialization
            InitAdvancedColormap();
       }

        private void loadImageConfigsButton_Click(object sender, EventArgs e)
        {
            string path = GetOpenFilePath("Open Image config file", "image config file(*.nic)|*.nic");
            if (!string.IsNullOrEmpty(path))
            {
                _model.LoadImageConfig(path);
            }
        }

        private void colorPickButton_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                colorPickButton.BackColor = cd.Color;
                _model.BlankPixelColor = cd.Color;
            }
        }

        private void ImageTypeChanged(ImageTypes it)
        {
            _model.ImageType = it;
        }

        // get state from model and update panel controls
        private void UpdatePanel()
        {
            // set colormap type before updating bias/contrast as setting color map will reset bias/contrast values
            if (listView1.Items.Count > (int)_model.ColorMapType)
            {
                listView1.Items[(int)_model.ColorMapType].Selected = true;
            }
            biasNumericUpDown.Value = (decimal)_model.Bias;
            contrastNumericUpDown.Value = (decimal)_model.Contrast;
            colorTableDepthNumericUpDown.Value = _model.ColorTableDepth;
            indexedImageRadioButton.Checked = _model.ImageType == ImageTypes.Indexed;
            trueColorImageRadioButton.Checked = _model.ImageType == ImageTypes.TrueColor;
            colorPickButton.BackColor = _model.BlankPixelColor;
            switch (_model.ScalingAlgorithmType)
            {
                case ScalingAlgorithms.Linear:
                    linearCMRadioButton.Checked = true;
                    break;
                case ScalingAlgorithms.SquareRoot:
                    squareRootCMRadioButton.Checked = true;
                    break;
                case ScalingAlgorithms.Logarithmic:
                    logarithmicCMRadioButton.Checked = true;
                    break;
                case ScalingAlgorithms.HistoEqualize:
                    histogramCMRadioButton.Checked = true;
                    break;
                case ScalingAlgorithms.Custom:
                    customCMRadioButton.Checked = true;
                    break;
                default:
                    break;
            }
            InitHistogramControls();
        }

        internal IModel Model
        {
            get { return _model; }
            set
            {
                // disconnect old model event
                if (_model != null)
                {
                    _model.CurrentSliceChanged -= new CurrentSliceChanged(Model_CurrentSliceChanged);
                }
                _model = value;
                // connect new model event
                _model.CurrentSliceChanged += new CurrentSliceChanged(Model_CurrentSliceChanged);
                UpdatePanel();
            }
        }

        void Model_CurrentSliceChanged()
        {
            // I don't support histogram per slice while playing This is for better performance and fast play
            if (!_model.Playing)
            {
                InitHistogramControls();
            }
        }

        private void saveImageConfigsButton_Click(object sender, EventArgs e)
        {
            string path = GetSaveFilePath("Save Najm Colormap", "Image Config files (*.nic)|*.nic", "nic");
            if (!string.IsNullOrEmpty(path))
            {
                _model.SaveImageConfig(path);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region utils
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private string GetOpenFilePath(string title, string filter)
        {
            string path = null;
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = filter;
            d.Title = title;
            if (d.ShowDialog() == DialogResult.OK)
            {
                path = d.FileName;
            }
            return path;
        }

        private string GetSaveFilePath(string title, string filter, string defExt)
        {
            string path = null;
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = filter;
            d.AddExtension = true;
            d.DefaultExt = defExt;
            d.OverwritePrompt = true;
            d.Title = title;
            if (d.ShowDialog() == DialogResult.OK)
            {
                path = d.FileName;
            }
            return path;
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private IModel _model;
        #endregion

    }
}
