﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Najm.ImagingCore.ColorTables;
using Najm.ImagingCore.ColorMaps;

namespace Najm.ImagingCore
{
    class TrueColorImage : ImageBase
    {
        public TrueColorImage() : base() { _type = ImageTypes.TrueColor; }

        override public Bitmap CreateBitmap(int x, int y, int width, int height)
        {
            // validate arguments
            if (width > _width || width < 0 || height > _height || height < 0)
            {
                throw new ArgumentException("Dimensions of the section are invlaid");
            }

            // if section dimensions are the same, no need to create the bitmap again;
            if (_bitmap != null &&
                new Rectangle(x, y, width, height).Equals(new Rectangle(_sectionX, _sectionY, _sectionWidth, _sectionHeight)))
            {
                Reset();
            }

            // create the bitmap
            if (_bitmap == null)
            {
                // cache new dimensions
                _sectionHeight = height;
                _sectionWidth = width;
                _sectionX = x;;
                _sectionY = y;

                // Now scale normalized lookup table according to current scaling function
                _colorTable.ScaleIt(_data, _width, _height);

                int bitsperpixel = 32;   // I'll always use this
                int stride = (width * bitsperpixel / 8);
                byte[] bmd = new byte[stride * height];

                // bitmaps are usually stored with first scan line in buffer 
                // represent last scan line in immage. so I'm maintaining this here.
                int indexSrc = _width * y + x;
                int indexDst = 0;
                int t0 = System.Environment.TickCount;
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        int pixelIndex = _data[indexSrc++];
                        int pixelColor;
                        if (_colorTable.Lookup(pixelIndex, out pixelColor))
                        {
                            if (_colorMap != null)
                            {
                                RGB rgbColor;
                                _colorMap.Lookup(pixelColor, out rgbColor);
                                bmd[indexDst++] = rgbColor.B;
                                bmd[indexDst++] = rgbColor.G;
                                bmd[indexDst++] = rgbColor.R;
                                bmd[indexDst++] = 0xFF;
                            }
                            else
                            {
                                // well, we don't have a color map. use pixelColor as is
                                bmd[indexDst++] = (byte)pixelColor;
                                bmd[indexDst++] = (byte)pixelColor;
                                bmd[indexDst++] = (byte)pixelColor;
                                bmd[indexDst++] = 0xFF;
                            }
                        }
                        else
                        {
                            // invalid pixel
                            bmd[indexDst++] = _invalidPixelValueColor.B;
                            bmd[indexDst++] = _invalidPixelValueColor.G;
                            bmd[indexDst++] = _invalidPixelValueColor.R;
                            bmd[indexDst++] = _invalidPixelValueColor.A;    // allow transparent background
                        }
                    }
                    indexSrc += (_width - width);  // move to next scan line down.
                }
                _pBitmapMemory = System.Runtime.InteropServices.Marshal.AllocHGlobal(stride * height);
                System.Runtime.InteropServices.Marshal.Copy(bmd, 0, _pBitmapMemory, stride * height);
                _bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppArgb, _pBitmapMemory);
            }
            return _bitmap;
        }

        override public IColorMap ColorMap
        {
            get
            {
                return _colorMap;
            }
            set
            {
                if (_colorMap != value)
                {
                    _colorMap = value;
                    Reset();
                }
            }
        }

        override public void Reset()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
            if (IntPtr.Zero != _pBitmapMemory)
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(_pBitmapMemory);
                _pBitmapMemory = IntPtr.Zero;
            }
        }
    }
}
