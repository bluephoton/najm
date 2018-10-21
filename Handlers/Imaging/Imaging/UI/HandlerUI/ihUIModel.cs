using System;
using System.Collections.Generic;
using System.Text;
using Najm.ImagingCore;
using Najm.ImagingCore.ColorScaling;
using Najm.ImagingCore.ColorMaps;
using Najm.ImagingCore.ColorTables;
using System.Drawing;
using Najm.FITSIO;

namespace ImageHandler
{
    internal delegate void ImageChanged();
    internal delegate void CurrentSliceChanged();
    interface IModel
    {
        void Initialize(IHDU[] hdus);

        double Minimum { get; }
        double Maximum { get; }
        void SetMinMax(double min, double max);
        void ResetMinMax();

        int ColorTableDepth { set; get; }
        ScalingAlgorithms ScalingAlgorithmType { get; set; }

        ColorMapTypes ColorMapType { get; }
        void SetColormapParams(ColorMapTypes ColorMapType, string filename);
        double Bias { get; set; }
        double Contrast { get; set; }
        void ResetBiasContrast();
        void SetLinearInterpolatoinPoints(string red, string green, string blue);
        ImageTypes ImageType { get; set; }
        Color BlankPixelColor { get; set; }
        void SaveColorMap(string file);
        void LoadColorMap(string file);

        int CurrentSlice { get; set; }
        void FirstSlice();
        void LastSlice();
        IHDU HDU { get; }
        Bitmap Bitmap { get; }
        Histogram Histogram { get; }
        bool Playing { get; set; }
        event ImageChanged ImageChanged;
        event CurrentSliceChanged CurrentSliceChanged;

        void SaveImageConfig(string path);

        void LoadImageConfig(string path);
    }

    class ihUIModel: IModel
    {
        class SliceInfo
        {
            internal SliceInfo(IHDU hdu)
            {
                _hdu = hdu;
                _hdu.DataMngr.CurrentSlice = 0;
                _modelState = ModelState.Default;
                _modelState.PixelMinimum = _hdu.DataMngr.Minimum;
                _modelState.PixelMaximum = _hdu.DataMngr.Maximum;
                _modelState.Histogram = new Histogram(_hdu.DataMngr.Data, 500, _hdu.DataMngr.Minimum, _hdu.DataMngr.Maximum, _hdu.DataMngr.BlankValue);
            }
            private IHDU _hdu;
            private ModelState _modelState;
        }
        #region IModel Members

        public void Initialize(IHDU[] hdus)
        {
            _playing = false;
            _hdu = hdus[0];
            _hdu.DataMngr.CurrentSlice = 0;
            _images = new IImage[_hdu.DataMngr.NumSlices];
            _modelState = ModelState.Default;
            _modelState.PixelMinimum = _hdu.DataMngr.Minimum;
            _modelState.PixelMaximum = _hdu.DataMngr.Maximum;
            _modelState.Histogram = new Histogram(_hdu.DataMngr.Data, 500, _hdu.DataMngr.Minimum, _hdu.DataMngr.Maximum, _hdu.DataMngr.BlankValue);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Color table related
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region min, max, Scaling algorithm
        public double Minimum { get { return _modelState.PixelMinimum; } }
        public double Maximum { get { return _modelState.PixelMaximum; } }
        public void SetMinMax(double min, double max)
        {
            if (_modelState.PixelMinimum != min || _modelState.PixelMaximum != max)
            {
                _modelState.PixelMinimum = min;
                _modelState.PixelMaximum = max;
                ResetColorTable();
                // TODO: we don't need to reset image every time we reset color table. Only because histogram needs 
                // full reset (to calculate number of active pixels) we have to do this.
                ResetImage();
            }
        }
        public void ResetMinMax()
        {
            if (_modelState.PixelMinimum != _hdu.DataMngr.Minimum || _modelState.PixelMaximum != _hdu.DataMngr.Maximum)
            {
                _modelState.PixelMinimum = _hdu.DataMngr.Minimum;
                _modelState.PixelMaximum = _hdu.DataMngr.Maximum;
                ResetColorTable();
                // TODO: we don't need to reset image every time we reset color table. Only because histogram needs 
                // full reset (to calculate number of active pixels) we have to do this.
                ResetImage();
            }
        }

        public int ColorTableDepth
        {
            get { return _modelState.ColorTable.Depth; }
            set
            {
                if (_modelState.ColorTable.Depth != value)
                {
                    _modelState.ColorTable.Depth = value;
                    ResetColorTable();
                    // TODO: we don't need to reset image every time we reset color table. Only because histogram needs 
                    // full reset (to calculate number of active pixels) we have to do this.
                    ResetImage();
                }
            }
        }

        public ScalingAlgorithms ScalingAlgorithmType
        {
            get { return _modelState.ColorTable.ScalingAlgorithmType; }
            set
            {
                if (_modelState.ColorTable.ScalingAlgorithmType != value)
                {
                    _modelState.ColorTable.ScalingAlgorithmType = value;
                    ResetColorTable();
                    // TODO: we don't need to reset image every time we reset color table. Only because histogram needs 
                    // full reset (to calculate number of active pixels) we have to do this.
                    ResetImage();
                }
            }
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Colormap related
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region colormap type, blank pixel color, bias, contrast

        public ColorMapTypes ColorMapType { get { return _modelState.ColorMap.Type; } }
        public void SetColormapParams(ColorMapTypes colorMapType, string param)
        {
            if (_modelState.ColorMap.Type != colorMapType || _modelState.ColorMap.Param != param)
            {
                _modelState.ColorMap.Type = colorMapType;
                _modelState.ColorMap.Param = param;
                IColorMap cm = ColorMapFactory.Create(colorMapType, param);
                if (cm != null)
                {
                    cm.Initialize();
                    if (CurrentImage != null)
                    {
                        CurrentImage.ColorMap = cm;
                        RaiseImageChanged();
                    }
                }
            }
        }

        public double Bias
        {
            get { return CurrentImage.ColorMap.Bias; }
            set
            {
            IColorMap cm = CurrentImage.ColorMap;
                cm.Bias = value;
                CurrentImage.ColorMap = cm;
                RaiseImageChanged();
            }
        }

        public double Contrast
        {
            get { return CurrentImage.ColorMap.Contrast; }
            set
            {
                IColorMap cm = CurrentImage.ColorMap;
                cm.Contrast = value;
                CurrentImage.ColorMap = cm;
                RaiseImageChanged();
            }
        }

        public void ResetBiasContrast()
        {
            IColorMap cm = CurrentImage.ColorMap;
            cm.Bias = 0.5;
            cm.Contrast = 1;
            CurrentImage.ColorMap = cm;
            RaiseImageChanged();
        }

        public void SetLinearInterpolatoinPoints(string red, string green, string blue)
        {
            StringBuilder sb =new StringBuilder(100);
            sb.Append(red).Append(";").Append(green).Append(";").Append(blue);
            SetColormapParams(ColorMapTypes.Advanced, sb.ToString());
        }

        public void SaveColorMap(string file)
        {
            if (CurrentImage != null && CurrentImage.ColorMap != null)
            {
                CurrentImage.ColorMap.Save(file);
            }
        }

        public void LoadColorMap(string file)
        {
            _modelState.ColorMap.Param = "";
            IColorMap cm = ColorMapFactory.FromFile(file);
            if (cm != null)
            {
                _modelState.ColorMap.Type = cm.Type;
                if (CurrentImage != null)
                {
                    CurrentImage.ColorMap = cm;
                    RaiseImageChanged();
                }
            }
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Image related
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region iamge type
        public ImageTypes ImageType
        {
            get { return _modelState.ImagePixelFormat; }
            set
            {
                _modelState.ImagePixelFormat = value;
                ResetImage();
            }
        }

        public Color BlankPixelColor
        {
            get { return _modelState.InvalidPixelValueColor; }
            set
            {
                if (_modelState.InvalidPixelValueColor != value)
                {
                    _modelState.InvalidPixelValueColor = value;
                    CurrentImage.InvalidPixelValueColor = value;
                    RaiseImageChanged();
                }
            }
        }

        #endregion

        public void SaveImageConfig(string path)
        {
            if (CurrentImage != null)
            {
                CurrentImage.SaveConfig(path);
            }
        }

        public void LoadImageConfig(string path)
        {
            CurrentImage = ImageFactory.FromFile(path);
            if (CurrentImage != null)
            {
                int width = _hdu.Axes[0].NumPoints;
                int height = _hdu.Axes[1].NumPoints;
                CurrentImage.Initialize(_hdu.DataMngr.Data, width, height);
                // notify observers
                RaiseImageChanged();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // slices
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Slices
        public int CurrentSlice
        {
            get{return _hdu.DataMngr.CurrentSlice;}
            set
            {
                if (_hdu.DataMngr.CurrentSlice != value)
                {
                    _hdu.DataMngr.CurrentSlice = value;
                    UpdateCurrentSlice();
                    if (_images[value] == null)
                    {
                        ResetImage();
                    }
                    else
                    {
                        RaiseImageChanged();
                    }
                }
            }
        }

        public void FirstSlice()
        {
            if (CurrentSlice != 0)
            {
                CurrentSlice = 0;
                UpdateCurrentSlice();
                RaiseImageChanged();
            }
        }

        public void LastSlice()
        {
            if (CurrentSlice != _hdu.DataMngr.NumSlices - 1)
            {
                CurrentSlice = _hdu.DataMngr.NumSlices - 1;
                UpdateCurrentSlice();
                RaiseImageChanged();
            }
        }
        int NumSlices { get { return _hdu.DataMngr.NumSlices; } }

        public Histogram Histogram { get { return _modelState.Histogram; } }

        #endregion
        #endregion

        private void ResetColorTable()
        {
            IColorTable ct = ColorTableFactory.Create(_modelState.ColorTable.Type);
            ct.Initialize(1 << _modelState.ColorTable.Depth, _modelState.PixelMinimum, _modelState.PixelMaximum);
            ct.ScalingAlgorithm = ScalingAlgorithmFactory.Create(_modelState.ColorTable.ScalingAlgorithmType);
            _colorTable = ct;
        }

        private void ResetImage()
        {
            // need to maintain color map used with image if we have one
            IColorMap cm = null;
            if (CurrentImage != null)
            {
                cm = CurrentImage.ColorMap;
                CurrentImage = null;
            }

            // create new image now
            CreateImage();

            // restore colormap
            if (cm != null)
            {
                CurrentImage.ColorMap = cm;
            }

            // notify observers
            RaiseImageChanged();
        }
        
        private void CreateImage()
        {
            int width = _hdu.Axes[0].NumPoints;
            int height = _hdu.Axes[1].NumPoints;

            // see if our values make little sense
            if (width <= 0 || height <= 0 || (_modelState.PixelMinimum == 0 && _modelState.PixelMaximum == 0))
            {
                // TODO: create a special exception class
                throw new System.Exception("Can't create Image - Invalid image data!");
            }

            // create image object
            CurrentImage = ImageFactory.Create(_modelState.ImagePixelFormat);

            // make sure we have a color table
            if (_colorTable == null)
            {
                ResetColorTable();
            }
            
            // initialize it passing the color table
            CurrentImage.Initialize(_hdu.DataMngr.Data, width, height, _colorTable, null);
        }

        private IImage CurrentImage
        {
            get
            {
                // make sure we have an image
                if (_images[_hdu.DataMngr.CurrentSlice] == null)
                {
                    CreateImage();
                }
                return _images[_hdu.DataMngr.CurrentSlice];
            }
            set
            {
                _images[_hdu.DataMngr.CurrentSlice] = value;
            }
        }

        public Bitmap Bitmap
        {
            get
            {
                return CurrentImage.CreateBitmap();
            }
        }

        private void RaiseImageChanged()
        {
 	        if(ImageChanged != null)
            {
                ImageChanged();
            }
        }

        private void UpdateCurrentSlice()
        {
            _modelState.PixelMinimum = _hdu.DataMngr.Minimum;
            _modelState.PixelMaximum = _hdu.DataMngr.Maximum;
            // see void Model_CurrentSliceChanged() in ImageHandlerUI.cs for why this is commented
            // also notice that by changing slice, you loose min/max settings for previous slice
            if (!Playing)
            {
                _modelState.Histogram = new Histogram(_hdu.DataMngr.Data, 500, _hdu.DataMngr.Minimum, _hdu.DataMngr.Maximum, _hdu.DataMngr.BlankValue);
            }
            RaiseCurrentSliceChanged();
        }

        private void RaiseCurrentSliceChanged()
        {
            if (CurrentSliceChanged != null)
            {
                CurrentSliceChanged();
            }
        }

        public IHDU HDU { get { return _hdu; } }
        public bool Playing
        {
            set { _playing = value; }
            get { return _playing; }
        }

        private IHDU _hdu;
        private ModelState _modelState;
        private IColorTable _colorTable;
        private IImage[] _images;
        public event ImageChanged ImageChanged;
        public event CurrentSliceChanged CurrentSliceChanged;
        private bool _playing;
    }
}
