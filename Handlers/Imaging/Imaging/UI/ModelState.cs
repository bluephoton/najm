using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Najm.ImagingCore;
using Najm.ImagingCore.ColorMaps;
using Najm.ImagingCore.ColorTables;
using Najm.ImagingCore.ColorScaling;

namespace ImageHandler
{
    internal class ModelState
    {
        static internal ModelState Default
        {
            get
            {
                ModelState ps = new ModelState();
                ps.ColorMap.Bias = 0.5;
                ps.ColorMap.Contrast = 1;
                ps.ColorMap.Type = ColorMapTypes.Gray;

                ps.ColorTable.Depth = 14;
                ps.ColorTable.Type = ColorTableType.Indexed;
                ps.ColorTable.ScalingAlgorithmType = ScalingAlgorithms.Logarithmic;
                ps.ImagePixelFormat = ImageTypes.Indexed;
                ps.InvalidPixelValueColor = Color.White;
                return ps;
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region color table
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal class UIColorTable
        {
            private ColorTableType _colorTableType;
            private int _colorTableDepth;
            private ScalingAlgorithms _ScalingAlgorithmType;
            public ColorTableType Type
            {
                get { return _colorTableType; }
                set { _colorTableType = value; }
            }

            public int Depth
            {
                get { return _colorTableDepth; }
                set { _colorTableDepth = value; }
            }
            internal ScalingAlgorithms ScalingAlgorithmType
            {
                get { return _ScalingAlgorithmType; }
                set { _ScalingAlgorithmType = value; }
            }
        }
        private UIColorTable _colorTable = new UIColorTable();
        internal UIColorTable ColorTable { get { return _colorTable; } }
        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region colormap
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal class UIColorMap
        {
            private ColorMapTypes _colorMapType;
            private string _mapParam;
            private double _contrast;
            private double _bias;
            private UILIPoints _uiLIPoints = new UILIPoints();
            internal ColorMapTypes Type
            {
                get { return _colorMapType; }
                set { _colorMapType = value; }
            }
            internal string Param
            {
                get { return _mapParam; }
                set { _mapParam = value; }
            }
            internal double Contrast
            {
                get { return _contrast; }
                set { _contrast = value; }
            }
            internal double Bias
            {
                get { return _bias; }
                set { _bias = value; }
            }
            internal class UILIPoints
            {
                private string _red;
                private string _blue;
                private string _green;
                internal string RedPoints
                {
                    get { return _red; }
                    set { _red = value; }
                }
                internal string BluePoints
                {
                    get { return _blue; }
                    set { _blue = value; }
                }
                internal string GreenPoints
                {
                    get { return _green; }
                    set { _green = value; }
                }
            }
            internal UILIPoints LIPoints
            {
                get { return _uiLIPoints; }
                set { _uiLIPoints = value; }
            }
        }
        private UIColorMap _colorMap = new UIColorMap();        
        internal UIColorMap ColorMap
        {
            get { return _colorMap; }
            set { _colorMap = value; }
        }
        #endregion
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private Color _invalidPixelValueColor;
        public Color InvalidPixelValueColor
        {
            get { return _invalidPixelValueColor; }
            set { _invalidPixelValueColor = value; }
        }

        private ImageTypes _imagePixelFormat;
        internal ImageTypes ImagePixelFormat
        {
            get { return _imagePixelFormat; }
            set { _imagePixelFormat = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private Histogram _histogram;
        internal Histogram Histogram
        {
            get { return _histogram; }
            set { _histogram = value; }
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private double _pixelMinimum;
        private double _pixelMaximum;
        internal double PixelMinimum
        {
            get { return _pixelMinimum; }
            set { _pixelMinimum = value; }
        }
        internal double PixelMaximum
        {
            get { return _pixelMaximum; }
            set { _pixelMaximum = value; }
        }
    }
}
