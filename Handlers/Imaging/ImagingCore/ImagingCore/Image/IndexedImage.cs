using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Najm.ImagingCore.ColorMaps;
using Najm.ImagingCore.ColorTables;

namespace Najm.ImagingCore
{
    class IndexedImage : ImageBase
    {
        internal IndexedImage() : base() { _type = ImageTypes.Indexed; }
        override public Bitmap CreateBitmap(int x, int y, int width, int height)
        {
            // validate arguments
            if (width > _width || width < 0 || height > _height || height < 0)
            {
                throw new ArgumentException("Dimensions of the section are invlaid");
            }

            // if section dimensions are the same, no need to create the bitmap again;
            if (_bitmap != null &&
                !(new Rectangle(x, y, width, height).Equals(new Rectangle(_sectionX, _sectionY, _sectionWidth, _sectionHeight))))
            {
                Reset();
            }

            // create the bitmap
            if (_bitmap == null)
            {
                // cache new dimensions
                _sectionHeight = height;
                _sectionWidth = width;
                _sectionX = x;
                _sectionY = y;

                // Now scale normalized lookup table according to current scaling function
                _colorTable.ScaleIt(_data, _width, _height);

                //
                int bitsperpixel = 8;   // I'll always use this
                int stride = (width * bitsperpixel / 8 + 3) / 4 * 4;
                int padding = stride - width * bitsperpixel / 8;
                byte[] bmd = new byte[stride * height];

                // bitmaps are usually stored with first scan line in buffer 
                // represent last scan line in immage. so I'm maintaining this here.
                int indexSrc = _width * y + x;
                int indexDst = 0;
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        int pixelIndex = _data[indexSrc++];
                        int pixelColor;
                        if(_colorTable.Lookup(pixelIndex, out pixelColor))
                        {
                            // for indexed, we don't use color map here. color maps are used 
                            // later to set image palette.
                            byte b = (byte)pixelColor;
                            System.Diagnostics.Debug.Assert(b != 0xFF);
                            bmd[indexDst++] = b;
                        }
                        else
                        {
                            // invalid pixel color
                            bmd[indexDst++] = 0xFF;
                        }
                    }
                    indexSrc += (_width - width);  // move to next scan line down.
                    indexDst += padding;
                }

                _pBitmapMemory = System.Runtime.InteropServices.Marshal.AllocHGlobal(stride * height);
                System.Runtime.InteropServices.Marshal.Copy(bmd, 0, _pBitmapMemory, stride * height);
                _bitmap = new Bitmap(width, height, stride, PixelFormat.Format8bppIndexed, _pBitmapMemory);
                
                // set the palette
                SetColorPalette();
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
                _colorMap = value;
                SetColorPalette();
            }
        }

        private void SetColorPalette()
        {
            if (_bitmap != null)
            {
                ColorPalette cp = _bitmap.Palette;
                if (_colorMap == null || _colorMap.Length == 0)
                {
                    // we have no map, use gray scale palette!
                    for (int i = 0; i < 255; i++)
                    {
                        cp.Entries[i] = Color.FromArgb(255, i, i, i);
                    }
                }
                else
                {
                    for (int i = 0; i < 256; i++)
                    {
                        RGB rgbColor;
                        _colorMap.Lookup(i, out rgbColor);
                        // I'll make last entry transparent as i use it for invalid pixel color
                        // TODO: take this outside the loop
                        byte A = (byte)((i == 255) ? 0x00 : 0xFF);
                        cp.Entries[i] = Color.FromArgb(A, rgbColor.R, rgbColor.G, rgbColor.B);
                    }
                }
                _bitmap.Palette = cp;
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
