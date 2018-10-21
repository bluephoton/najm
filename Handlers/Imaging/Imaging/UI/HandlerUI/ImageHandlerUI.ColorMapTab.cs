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
    internal partial class ImageHandlerUI
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region trick to always have a selected item in the listview control. copied from ms forums
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected && !_insideItemSelectionChanged)
            {
                _insideItemSelectionChanged = true;
                this.BeginInvoke(new ListViewItemSelectionChangedEventHandler(FixupSelection), new object[] { sender, e });
            }
        }
        private bool _insideItemSelectionChanged;

        private void FixupSelection(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv.SelectedItems.Count == 0)
            {
                e.Item.Selected = true;
            }
            _insideItemSelectionChanged = false;
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region color mapping
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // we get this event when item is deselected first (in which case selected indices count will be zero)
            if (listView1.SelectedIndices.Count > 0 && listView1.Items[listView1.SelectedIndices[0]].Selected)
            {
                string[] cmaps = { 
                                    "Gray","Red","Green","Blue","A","B","BB","HE","Heat","Cool","Rainbow",
                                    "Standard","I8","AIPS0","Staircase","Color","SLS","HSV","Advanced", "RGBFile", "None"
                                 };

                ColorMapTypes cp = (ColorMapTypes) Enum.Parse(typeof(ColorMapTypes), cmaps[listView1.SelectedIndices[0]]);
                switch (cp)
                {
                    case ColorMapTypes.RGBFile:
                        // showing dialog while inside this event handler miss things up. schedule
                        // an event to take care of showing the dialog to break this link
                        this.BeginInvoke(new EventHandler(ShowFileDialog), new object[] { sender, e });
                        // I return to avoid executing the code below as the logic will continue in the 
                        // ShowFileDialog event above.
                        break;
                    case ColorMapTypes.Advanced:
                        // if we already in advanced no need to go to advanced tab. perhaps User wants
                        // to change it from colormap
                        if (_model.ColorMapType != ColorMapTypes.Advanced)
                        {
                            tabControl1.SelectedTab = tabControl1.TabPages[2];
                        }
                        break;
                    default:
                        _colorMapListViewSelectedIndex = listView1.SelectedIndices[0];
                        if (cp != ColorMapTypes.None)
                        {
                            _model.SetColormapParams(cp, "");
                            ResetContrastBias();
                        }
                        break;
                }
            }
        }

        private void ShowFileDialog(object sender, EventArgs e)
        {
            string path = GetOpenFilePath("Open colormap file", "Colormap file(*.ncm)|*.ncm");
            if (!string.IsNullOrEmpty(path))
            {
                _model.LoadColorMap(path);
                ResetContrastBias();
            }
            else
            {
                listView1.Items[_colorMapListViewSelectedIndex].Selected = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region contrast/bias
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void contrastTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (!_contrastChanging)
            {
                _contrastChanging = true;
                double val = (double)contrastTrackBar.Value / 1000.0;
                _model.Contrast = val;
                contrastNumericUpDown.Value = (decimal)val;
                _contrastChanging = false;
            }
        }
        private void contrastNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_contrastChanging)
            {
                _contrastChanging = true;
                double val = (double)contrastNumericUpDown.Value;
                _model.Contrast = val;
                contrastTrackBar.Value = (int)(val * 1000);
                _contrastChanging = false;
            }
        }

        private void biasTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (!_biasChanging)
            {
                _biasChanging = true;
                double val = (double)biasTrackBar.Value / 1000.0;
                _model.Bias = val;
                biasNumericUpDown.Value = (decimal)val;
                _biasChanging = false;
            }
        }
        private void biasNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_biasChanging)
            {
                _biasChanging = true;
                double val = (double)biasNumericUpDown.Value;
                _model.Bias = val;
                biasTrackBar.Value = (int)(val * 1000);
                _biasChanging = false;
            }
        }

        private void resetColorMapButton_Click(object sender, EventArgs e)
        {
            ResetContrastBias();
            _model.ResetBiasContrast();
        }

        private void ResetContrastBias()
        {
            contrastTrackBar.Value = 1000;
            biasTrackBar.Value = 500;
        }
        #endregion

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int _colorMapListViewSelectedIndex;
        private bool _biasChanging;
        private bool _contrastChanging;
        #endregion
    }
}
