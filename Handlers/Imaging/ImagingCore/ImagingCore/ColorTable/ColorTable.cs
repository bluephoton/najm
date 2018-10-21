using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Najm.ImagingCore.ColorScaling;
using Najm.ImagingCore.ColorTables;

namespace Najm.ImagingCore.ColorTables
{
    // ReducedColors : 
    // pixel values will be mapped from their original range to a smaller one determined
    // by the size of the color table.
    //
    // FullScale:
    // Color table will be constructed from actual pixel values in the image. Its size
    // might grow big to reach the size of the image.
    //
    // Notes:
    // - in both cases we'll normalize the image to make the range of any pixel value goes from 0 to 1.
    // - Exact color mode worked with small images but was sooooo slow with large ones due to bad algorithm
    //   used to sort colors. I didn't see quality gain to justify its troubles!
    public enum ColorTableType { ReducedColors, FullScale, Indexed }
    public interface IColorTable
    {
        void Initialize(int size, double minColorValue, double maxColorValue);
        IScalingAlgorithm ScalingAlgorithm { get;  set; }
        int Add(double pizelValue);
        void Normalize();
        void ScaleIt(int[] image, int width, int height);
        bool Lookup(int pixelIndex, out int pixelColor);
        double Minimum { get; }
        double Maximum { get; }
        int Size { get; }
        void Save(Stream s);
        void Save(string path);
    }

    public class ColorTableFactory
    {
        public static IColorTable Create(ColorTableType ctt)
        {
            IColorTable ct = null;
            switch (ctt)
            {
                case ColorTableType.ReducedColors:
                    ct = new ReducedColorTable();
                    break;
                case ColorTableType.FullScale:
                    ct = new FullScaledColorTable();
                    break;
                case ColorTableType.Indexed:
                    ct = new IndexedColorTable();
                    break;
            }
            return ct;
        }

        static public IColorTable FromFile(string path)
        {
            IColorTable ct = null;
            XmlReader r = null;
            try
            {
                r = XmlReader.Create(path);
                r.ReadToFollowing("ColorTable");
                ColorTableType ctt = (ColorTableType)Enum.Parse(typeof(ColorTableType), r["Type"]);
                int cts = int.Parse(r["Size"]);
                ScalingAlgorithms sa = (ScalingAlgorithms)Enum.Parse(typeof(ScalingAlgorithms), r["Scale"]);
                double min = double.Parse(r["Minimum"]);
                double max = double.Parse(r["Maximum"]);
                ct = ColorTableFactory.Create(ctt);
                if (ct != null)
                {
                    ct.ScalingAlgorithm = ScalingAlgorithmFactory.Create(sa);
                    ct.Initialize(cts, min, max);
                }
            }
            finally
            {
                if (r != null)
                {
                    r.Close();
                }
            }
            return ct;
        }
    }
}
