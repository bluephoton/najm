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
    partial class ImageHandlerUI
    {
        private void InitColorTableUI()
        {
            // hook histogram events
            histogramGraphCtrl.LowValueChanged += new Najm.Controls.LowValueChanged(histogramGraphCtrl_LowValueChanged);
            histogramGraphCtrl.HighValueChanged += new Najm.Controls.HighValueChanged(histogramGraphCtrl_HighValueChanged);
            histogramGraphCtrl.VerticalMarkerReleased += new Najm.Controls.VerticalMarkerReleased(histogramGraphCtrl_VerticalMarkerReleased);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Color scaling control handling
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void linearCMRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ColorscalingFunctionRadioChanged((RadioButton)sender, ScalingAlgorithms.Linear);
        }

        private void squareRootCMRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ColorscalingFunctionRadioChanged((RadioButton)sender, ScalingAlgorithms.SquareRoot);
        }

        private void logarithmicCMRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ColorscalingFunctionRadioChanged((RadioButton)sender, ScalingAlgorithms.Logarithmic);
        }

        private void squareCMRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ColorscalingFunctionRadioChanged((RadioButton)sender, ScalingAlgorithms.Square);
        }

        private void histogramCMRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ColorscalingFunctionRadioChanged((RadioButton)sender, ScalingAlgorithms.HistoEqualize);
        }

        private void customCMRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ColorscalingFunctionRadioChanged((RadioButton)sender, ScalingAlgorithms.Custom);
        }

        private void ColorscalingFunctionRadioChanged(RadioButton rb, ScalingAlgorithms sat)
        {
            if (rb.Checked)
            {
                _model.ScalingAlgorithmType = sat;
            }
        }
        #endregion

        private void colorTableDepthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _model.ColorTableDepth = (int)colorTableDepthNumericUpDown.Value;
        }

        private void indexedImageRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (indexedImageRadioButton.Checked)
            {
                ImageTypeChanged(ImageTypes.Indexed);
            }
        }

        private void trueColorImageRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (trueColorImageRadioButton.Checked)
            {
                ImageTypeChanged(ImageTypes.TrueColor);
            }
        }

        internal void InitHistogramControls()
        {
            _disableNUDEventHack = true;    //TODO: think abou this later
            histogramGraphCtrl.SetData(_model.Histogram.Data, _model.Histogram.Frequencies);
            double min = _model.HDU.DataMngr.Minimum;
            double max = _model.HDU.DataMngr.Maximum;
            lowNumericUpDown.Minimum = (decimal)min;
            lowNumericUpDown.Maximum = (decimal)max;
            highNumericUpDown.Minimum = (decimal)min;
            highNumericUpDown.Maximum = (decimal)max;
            lowNumericUpDown.Value = (decimal)_model.Minimum;
            highNumericUpDown.Value = (decimal)_model.Maximum;
            lowNumericUpDown.Increment = (decimal)(max - min) / 1000;
            highNumericUpDown.Increment = (decimal)(max - min) / 1000;
            histogramGraphCtrl.LowValue = _model.Minimum;
            histogramGraphCtrl.HighValue = _model.Maximum;
            _disableNUDEventHack = false;
        }

        private void lowNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_disableNUDEventHack)
            {
                histogramGraphCtrl.LowValue = (double)lowNumericUpDown.Value;
            }
        }

        private void highNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_disableNUDEventHack)
            {
                histogramGraphCtrl.HighValue = (double)highNumericUpDown.Value;
            }
        }

        void histogramGraphCtrl_LowValueChanged(object sender, double value)
        {
            lowNumericUpDown.Value = (decimal)value;
        }

        void histogramGraphCtrl_HighValueChanged(object sender, double value)
        {
            highNumericUpDown.Value = (decimal)value;
        }

        void histogramGraphCtrl_VerticalMarkerReleased(object sender, double min, double max)
        {
            _model.SetMinMax(min, max);
        }


        private void applyHistogramButton_Click(object sender, EventArgs e)
        {
            _model.SetMinMax((double)lowNumericUpDown.Value, (double)highNumericUpDown.Value);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            histogramGraphCtrl.LowValue = _model.HDU.DataMngr.Minimum;
            histogramGraphCtrl.HighValue = _model.HDU.DataMngr.Maximum;
            lowNumericUpDown.Value = (decimal)_model.HDU.DataMngr.Minimum;
            highNumericUpDown.Value = (decimal)_model.HDU.DataMngr.Maximum;
            _model.ResetMinMax();
        }

        private bool _disableNUDEventHack;
    }
}
