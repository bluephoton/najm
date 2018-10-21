using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// DS9 color maps are eihter copied as from DS9 or generated using different code. Goal was to get similar results.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


namespace Najm.ImagingCore.ColorMaps
{
    public enum ColorMapTypes
    {
        Gray, Red, Green, Blue, A, B, BB, HE, Heat, Cool, Rainbow, Standard,
        I8, AIPS0, Staircase, Color, SLS, HSV, Advanced, RGBFile, None
    }

    public struct RGB
    {
        public RGB(byte r, byte g, byte b) { R = r; B = b; G = g; }
        public byte R;
        public byte G;
        public byte B;
    };

    public interface IColorMap
    {
        void Initialize();                              // provide a chance for color map provider to initialize it.
        void Lookup(int colorIndex, out RGB rgbColor);  // main point of interest!
        double Contrast { get; set; }                   // update the contrast of color map
        double Bias { get; set; }                       // update the bias of color map
        int Length { get; }
        void Save(string path);
        void Save(Stream s);
        ColorMapTypes Type { get; }
    }

    public class ColorMapFactory
    {
        static public IColorMap Create(ColorMapTypes cp)
        {
            return ColorMapFactory.Create(cp, "");
        }

        static public IColorMap FromFile(string path)
        {
            IColorMap cm = null;
            XmlReader r = null;
            try
            {
                r = XmlReader.Create(path);
                r.ReadToFollowing("ColorMap");
                double bias = double.Parse(r["Bias"]);
                double contrast = double.Parse(r["Contrast"]);
                string type = r["Type"];
                switch (type)
                {
                    case "RGBFile":
                        r.ReadToDescendant("RGB");
                        StringReader sr = new StringReader(r.ReadString());
                        cm = new FileColorMap(sr);
                        sr.Close();
                        break;
                    case "Advanced":
                        // skip LIP tag
                        r.ReadToDescendant("LIP");
                        r.ReadToDescendant("Red");
                        string red = r.ReadString();
                        r.ReadToNextSibling("Green");
                        string green = r.ReadString();
                        r.ReadToNextSibling("Blue");
                        string blue = r.ReadString();
                        cm = new CustomLIColorMap(red, green, blue);
                        break;
                    default:
                        cm = Create((ColorMapTypes)Enum.Parse(typeof(ColorMapTypes), r["Type"], true), null);
                        break;
                }
                if (cm != null)
                {
                    cm.Initialize();
                    cm.Bias = bias;
                    cm.Contrast = contrast;
                }
            }
            finally
            {
                if (r != null)
                {
                    r.Close();
                }
            }
            return cm;
        }

        // param is a colorMapFile for file color maps or LI points for LI color maps
        static public IColorMap Create(ColorMapTypes cp, string param)
        {
            IColorMap cpi = null;
            switch (cp)
            {
                case ColorMapTypes.Gray:
                    cpi = new GrayColorMap();
                    break;
                case ColorMapTypes.Red:
                    cpi = new RedColorMap();
                    break;
                case ColorMapTypes.Green:
                    cpi = new GreenColorMap();
                    break;
                case ColorMapTypes.Blue:
                    cpi = new BlueColorMap();
                    break;
                case ColorMapTypes.A:
                    cpi = new AColorMap();
                    break;
                case ColorMapTypes.B:
                    cpi = new BColorMap();
                    break;
                case ColorMapTypes.BB:
                    cpi = new BBColorMap();
                    break;
                case ColorMapTypes.HE:
                    cpi = new HEColorMap();
                    break;
                case ColorMapTypes.Heat:
                    cpi = new HeatColorMap();
                    break;
                case ColorMapTypes.Cool:
                    cpi = new CoolColorMap();
                    break;
                case ColorMapTypes.Rainbow:
                    cpi = new RainbowColorMap();
                    break;
                case ColorMapTypes.Standard:
                    cpi = new StandardColorMap();
                    break;
                case ColorMapTypes.I8:
                    cpi = new I8ColorMap();
                    break;
                case ColorMapTypes.AIPS0:
                    cpi = new AIPS0ColorMap();
                    break;
                case ColorMapTypes.Staircase:
                    cpi = new StaircaseColorMap();
                    break;
                case ColorMapTypes.Color:
                    cpi = new ColorColorMap();
                    break;
                case ColorMapTypes.SLS:
                    cpi = new SLSColorMap();
                    break;
                case ColorMapTypes.HSV:
                    cpi = new HSVColorMap();
                    break;
                case ColorMapTypes.RGBFile:
                    if (string.IsNullOrEmpty(param))
                    {
                        throw new ArgumentNullException("Custom Map file name is not valid");
                    }
                    cpi = new FileColorMap(param);
                    break;
                case ColorMapTypes.Advanced:
                    {
                        if (string.IsNullOrEmpty(param))
                        {
                            throw new ArgumentNullException("LI points are missing");
                        }
                        // extract color values
                        string[] LIPoitns = param.Split(new char[] { ';' });
                        if (LIPoitns.Length != 3 ||
                            string.IsNullOrEmpty(LIPoitns[0]) ||
                            string.IsNullOrEmpty(LIPoitns[1]) ||
                            string.IsNullOrEmpty(LIPoitns[2]))
                        {
                            throw new ArgumentNullException("Invalid LI points");
                        }
                        cpi = new CustomLIColorMap(LIPoitns[0], LIPoitns[1], LIPoitns[2]);
                    }
                    break;
                default:
                    break;
            }
            return cpi;
        }
    }
}
