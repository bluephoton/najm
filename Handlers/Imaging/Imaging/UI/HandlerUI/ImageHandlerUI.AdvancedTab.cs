using System;
using Najm.ImagingCore.ColorMaps;

namespace ImageHandler
{
    partial class ImageHandlerUI
    {
        private void InitAdvancedColormap()
        {
            for (int i = 0; i < liTemplatesComboBox.Items.Count; i++)
            {
                string[] sl = liTemplatesComboBox.Items[i].ToString().Split(new char[] { ':' });
                System.Diagnostics.Debug.Assert(sl.Length == 4);
                liTemplatesComboBox.Items[i] = new LIItemData(sl[0], sl[1], sl[2], sl[3]);
            }
            liTemplatesComboBox.SelectedIndex = 0;
            _rPoints = ((LIItemData)liTemplatesComboBox.Items[0]).RedPoints;
            _gPoints = ((LIItemData)liTemplatesComboBox.Items[0]).GreenPoints;
            _bPoints = ((LIItemData)liTemplatesComboBox.Items[0]).BluePoints;
        }

        private void advColorMappingTabPage_Enter(object sender, EventArgs e)
        {
            _model.SetLinearInterpolatoinPoints(_rPoints, _gPoints, _bPoints);
            listView1.Items[(int)ColorMapTypes.Advanced].Selected = true;
        }

        private void liTemplatesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LIItemData lid = (LIItemData)liTemplatesComboBox.SelectedItem;
            _rPoints = lid.RedPoints;
            _gPoints = lid.GreenPoints;
            _bPoints = lid.BluePoints;
            redPointsGridControl.Points = lid.RedPoints;
            greenPointsGridControl.Points = lid.GreenPoints;
            bluePointsGridControl.Points = lid.BluePoints;
        }

        private void redPointsGridControl_OnPointListChanged(string points)
        {
            _rPoints = points;
            LIPointsChanged();
        }

        private void greenPointsGridControl_OnPointListChanged(string points)
        {
            _gPoints = points;
            LIPointsChanged();
        }

        private void bluePointsGridControl_OnPointListChanged(string points)
        {
            _bPoints = points;
            LIPointsChanged();
        }

        private void LIPointsChanged()
        {
            // apply the change if autoapply
            if (autoApplyCheckBox.Checked)
            {
                PointListChanged(_rPoints, _gPoints, _bPoints);
            }
        }

        private void PointListChanged(string rPoints, string gPoints, string bPoints)
        {
            if (_model != null)
            {
                _model.SetLinearInterpolatoinPoints(rPoints, gPoints, bPoints);
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            PointListChanged(_rPoints, _gPoints, _bPoints);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string path = GetSaveFilePath("Save Najm Colormap", "Colormap files (*.ncm)|*.ncm", "ncm");
            if (!string.IsNullOrEmpty(path))
            {
                _model.SaveColorMap(path);
            }
        }

        private string _rPoints, _gPoints, _bPoints;
    }

    // linear interpolation advanced color mapping
    internal class LIItemData
    {
        public LIItemData(string name, string redPoints, string greenPoints, string bluePoints)
        {
            _name = name;
            _rPoints = redPoints;
            _gPoints = greenPoints;
            _bPoints = bluePoints;
        }
        private string _name;
        private string _rPoints;
        private string _gPoints;
        private string _bPoints;

        public string RedPoints
        {
            get { return _rPoints; }
        }
        public string GreenPoints
        {
            get { return _gPoints; }
        }
        public string BluePoints
        {
            get { return _bPoints; }
        }
        public override string ToString()
        {
            return _name;
        }
    }
}
