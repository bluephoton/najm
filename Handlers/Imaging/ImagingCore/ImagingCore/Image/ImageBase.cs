using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Xml;
using Najm.ImagingCore.ColorTables;
using Najm.ImagingCore.ColorMaps;
using Najm.ImagingCore.ColorScaling;
using Najm.FITSIO;

namespace Najm.ImagingCore
{
    abstract class ImageBase : IImage
    {
        public ImageBase()
        {
            _pBitmapMemory = IntPtr.Zero;
        }

        #region IImage interface implementation

        public void Initialize(double[] data, int width, int height, IColorTable colorTable, IColorMap colorMap)
        {
            // set members
            _width = width;
            _height = height;
            _rawData = data;

            // set default for this
            _invalidPixelValueColor = Color.White;

            // allocate image buffer
            _data = new int[width * height];

            // set color table now, we need it during raw buffer creation
            // TODO: throw if null, initialize?
            _colorTable = colorTable;

            // create color map
            if (colorMap != null)
            {
                _colorMap = colorMap;
            }
            else
            {
                _colorMap = ColorMapFactory.Create(ColorMapTypes.Gray);
                _colorMap.Initialize();
                _colorMap.Bias = 0.5;
                _colorMap.Contrast = 1;
            }

            // create raw image buffer.
            // separating this will  add more granularity saving time if we want to rebuild 
            // since we dont want to recalc extremes again...etc
            CreateRawImageBuffer();
        }

        // before calling this method we need to set the colorTable and colorMap on the image object
        // e.g:
        //          IImage i = ImageFactory.Create(...);
        //          i.ColorTable = ct;
        //          i.ColorMap = cm;
        //          i.Initialize(d, w, h);
        public void Initialize(double[] data, int width, int height)
        {
            // set members
            _width = width;
            _height = height;
            _rawData = data;

            // set default for this
            _invalidPixelValueColor = Color.White;

            // allocate image buffer
            _data = new int[width * height];

            // create raw image buffer.
            // separating this will  add more granularity saving time if we want to rebuild 
            // since we dont want to recalc extremes again...etc
            CreateRawImageBuffer();
        }

        public Bitmap CreateBitmap()
        {
            return CreateBitmap(0, 0, _width, _height);
        }

        public abstract Bitmap CreateBitmap(int x, int y, int width, int height);
        public abstract void Reset();
        public abstract IColorMap ColorMap{get; set;}


        public IColorTable ColorTable
        {
            get { return _colorTable; }
            set
            {
                int oldSize = -1;
                if (_colorTable != null)
                {
                    oldSize = _colorTable.Size;
                }
                _colorTable = value;
                Reset();
                if (_colorTable.Size != oldSize && _data != null)   // if _data==null we have nothing to create
                {
                    // changing the size of the color table requires a full rebuild, sorry!
                    CreateRawImageBuffer();
                }                
            }
        }

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public Color InvalidPixelValueColor
        {
            get { return _invalidPixelValueColor; }
            set
            {
                _invalidPixelValueColor = value;
                Reset();
            }
        }

        public void SaveConfig(string path)
        {
            StreamWriter sw = null;
            XmlWriter w = null;
            try
            {
                sw = new StreamWriter(path, false);
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                w = XmlWriter.Create(sw.BaseStream, settings);
                w.WriteStartElement("ImageConfig");
                w.WriteAttributeString("Type", _type.ToString());
                w.WriteAttributeString("NaNColor", _invalidPixelValueColor.ToArgb().ToString());
                w.WriteRaw("\r\n");
                w.Flush();
                _colorTable.Save(sw.BaseStream);
                _colorMap.Save(sw.BaseStream);
                w.WriteEndElement();
                w.Flush();
            }
            finally
            {
                if (w != null)
                {
                    w.Close();
                }
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        #endregion

        private void CreateRawImageBuffer()
        {
            //PerfTest.Start();

            // bitmaps are usually stored with first scan line in buffer 
            // represent last scan line in immage. so I'm maintaining this here.
            int seq = 0;
            int indexDst = _width * (_height - 1);   // fill last scanline first
            for (int r = 0; r < _height; r++)
            {
                for (int c = 0; c < _width; c++)
                {
                    double pixel = _rawData[seq++];
                    int valueIndex = _colorTable.Add(pixel);
                    System.Diagnostics.Debug.Assert(valueIndex >= 0);
                    _data[indexDst++] = valueIndex;
                }
                indexDst -= 2 * _width;
            }

            // now normalize the array!
            _colorTable.Normalize();

            //PerfTest.Stop("CreateRawImageBuffer: ");
        }

        #region data members

        protected int _width;
        protected int _height;
        protected int[] _data;        // this is an array of indices to actual pixel values
                                      // actual pixel values are stored in the color table object.
        protected double[] _rawData;  // will be used if we changed the parameters of the color table (its size) and we
                                      // need to fully rebuild the image.

        protected IColorTable _colorTable;
        protected IColorMap _colorMap;

        protected Bitmap _bitmap;
        protected IntPtr _pBitmapMemory;
        protected int _sectionWidth;
        protected int _sectionHeight;
        protected int _sectionX;
        protected int _sectionY;
        protected Color _invalidPixelValueColor;
        protected ImageTypes _type;
        #endregion
    }
}
