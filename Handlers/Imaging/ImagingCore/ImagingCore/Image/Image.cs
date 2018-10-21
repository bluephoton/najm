using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using Najm.ImagingCore.ColorTables;
using Najm.ImagingCore.ColorMaps;
using Najm.ImagingCore.ColorScaling;
using Najm.FITSIO;

namespace Najm.ImagingCore
{
    public enum ImageTypes { TrueColor, Indexed }
    public interface IImage
    {
        void Initialize(double[] data, int width, int height, IColorTable colorTable, IColorMap colorMap);
        void Initialize(double[] data, int width, int height);
        IColorTable ColorTable { get; set; }
        IColorMap ColorMap { get; set; }
        Bitmap CreateBitmap();
        Bitmap CreateBitmap(int x, int y, int width, int height);
        int Width { get; }
        int Height { get; }
        void Reset();
        Color InvalidPixelValueColor { get; set; }
        void SaveConfig(string file);
    }

    public class ImageFactory
    {
        static public IImage Create(ImageTypes it)
        {
            IImage ii = null;
            switch (it)
            {
                case ImageTypes.TrueColor:
                    ii = new TrueColorImage();
                    break;
                case ImageTypes.Indexed:
                    ii = new IndexedImage();
                    break;
            }
            return ii;
        }

        static public IImage FromFile(string nicFile)
        {
            IImage ii = null;
            StreamReader sr = null;
            XmlReader r = null;
            ImageTypes it = ImageTypes.Indexed;
            Color nan = Color.Empty;
            try
            {
                sr = new StreamReader(nicFile);
                r = XmlReader.Create(sr.BaseStream);
                r.ReadToFollowing("ImageConfig");
                it = (ImageTypes)Enum.Parse(typeof(ImageTypes), r["Type"]);
                nan = Color.FromArgb(int.Parse(r["NaNColor"]));
                r.Close(); r = null;
                sr.Close(); sr = null;
                IColorTable ct = ColorTableFactory.FromFile(nicFile);
                IColorMap cm = ColorMapFactory.FromFile(nicFile);
                ii = ImageFactory.Create(it);
                if (ii != null)
                {
                    ii.ColorTable = ct;
                    ii.ColorMap = cm;
                    ii.InvalidPixelValueColor = nan;
                }
            }
            finally
            {
                if (r != null)
                {
                    r.Close();
                }
                if (sr != null)
                {
                    sr.Close();
                }
            }
            
            return ii;
        }
    }
}
