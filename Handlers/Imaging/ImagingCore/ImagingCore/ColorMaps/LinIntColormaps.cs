using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

//------------------------------------------------------------------------------------------------------------
//    These are all Linear Interpolation color maps (even if they didn't use the LinearInterpolator class
//------------------------------------------------------------------------------------------------------------

namespace Najm.ImagingCore.ColorMaps
{
    // I'd rather not use LinearInterpolator unless it is necessary since its much slower.
    class GrayColorMap : BaseColorMap
    {
        public override void Initialize()
        {
            _type = ColorMapTypes.Gray;
            _originalTable = new RGB[256];
            _table = new RGB[256];
            for (int i = 0; i < _table.Length; i++)
            {
                byte b = (byte)i;
                _table[i] = _originalTable[i] = new RGB(b, b, b);
            }
        }
        public override void Lookup(int colorIndex, out RGB rgbColor)
        {
            rgbColor.R = rgbColor.G = rgbColor.B = _table[colorIndex].R;
        }
    }

    class RedColorMap : BaseColorMap
    {
        public override void Initialize()
        {
            _type = ColorMapTypes.Red;
            _originalTable = new RGB[256];
            _table = new RGB[256];
            for (int i = 0; i < _table.Length; i++)
            {
                byte b = (byte)i;
                _table[i] = _originalTable[i] = new RGB(b, 0, 0);
            }
        }
        public override void Lookup(int colorIndex, out RGB rgbColor)
        {
            rgbColor.R = _table[colorIndex].R;
            rgbColor.G = 0;  // i could have copied the rest but since i'm the one implementing the interface, i know its equicalant and I'd rather save instructions!
            rgbColor.B = 0;
        }
    }

    class GreenColorMap : BaseColorMap
    {
        public override void Initialize()
        {
            _type = ColorMapTypes.Green;
            _originalTable = new RGB[256];
            _table = new RGB[256];
            for (int i = 0; i < _table.Length; i++)
            {
                byte b = (byte)i;
                _table[i] = _originalTable[i] = new RGB(0, b, 0);
            }
        }

        public override void Lookup(int colorIndex, out RGB rgbColor)
        {
            rgbColor.R = 0;
            rgbColor.G = _table[colorIndex].G;
            rgbColor.B = 0;
        }
    }

    class BlueColorMap : BaseColorMap
    {
        public override void Initialize()
        {
            _type = ColorMapTypes.Blue;
            _originalTable = new RGB[256];
            _table = new RGB[256];
            for (int i = 0; i < _table.Length; i++)
            {
                byte b = (byte)i;
                _table[i] = _originalTable[i] = new RGB(0, 0, b);
            }
        }
        public override void Lookup(int colorIndex, out RGB rgbColor)
        {
            rgbColor.R = 0;
            rgbColor.G = 0;
            rgbColor.B = _table[colorIndex].B;
        }
    }

    class AColorMap : RGBColorMap
    {
        public override void Initialize()
        {
            _type = ColorMapTypes.A;
            _originalTable = new RGB[256];
            _table = new RGB[256];
            GenerateColors( "(0,0)(.25,0)(.5,1)(1,1)",
                            "(0,0)(.25,1)(.5,0)(.77,0)(1,1)",
                            "(0,0)(.125,0)(.5,1)(.64,.5)(.77,0)(1,0)");
        }
    }

    class BColorMap : RGBColorMap
    {
        public override void Initialize()
        {
            _type = ColorMapTypes.B;
            _originalTable = new RGB[256];
            _table = new RGB[256];
            GenerateColors("(0,0)(.25,0)(.5,1)(1,1)",
                           "(0,0)(.5,0)(.75,1)(1,1)",
                           "(0,0)(.25,1)(.5,0)(.75,0)(1,1)");
        }
    }

    class BBColorMap : RGBColorMap
    {
        public override void Initialize()
        {
            _type = ColorMapTypes.BB;
            _originalTable = new RGB[256];
            _table = new RGB[256];
            GenerateColors("(0,0)(.5,1)(1,1)",
                           "(0,0)(.25,0)(.75,1)(1,1)",
                           "(0,0)(.5,0)(1,1)");
        }
    }

    class HEColorMap : RGBColorMap
    {
        public override void Initialize()
        {
            _type = ColorMapTypes.HE;
            _originalTable = new RGB[256];
            _table = new RGB[256];
            GenerateColors("(0,0)(.015,.5)(.25,.5)(.5,.75)(1,1)",
                           "(0,0)(.065,0)(.125,.5)(.25,.75)(.5,.810)(1,1)",
                           "(0,0)(.015,.125)(.030,.375)(.065,.625)(.25,.25)(1,1)");
        }
    }

    class HeatColorMap : RGBColorMap
    {
        public override void Initialize()
        {
            _type = ColorMapTypes.Heat;
            _originalTable = new RGB[256];
            _table = new RGB[256];
            GenerateColors("(0,0)(.34,1)(1,1)",
                           "(0,0)(1,1)",
                           "(0,0)(.65,0)(.98,1)(1,1)");
        }
    }

    class CoolColorMap : RGBColorMap
    {
        public override void Initialize()
        {
            _type = ColorMapTypes.Cool;
            _originalTable = new RGB[256];
            _table = new RGB[256];
            GenerateColors("(0,0)(.29,0)(.76,.1)(1,1)",
                           "(0,0)(.22,0)(.96,1)(1,1)",
                           "(0,0)(.53,1)(1,1)");
        }
    }

    class RainbowColorMap : RGBColorMap
    {
        public override void Initialize()
        {
            _type = ColorMapTypes.Rainbow;
            _originalTable = new RGB[256];
            _table = new RGB[256];
            GenerateColors("(0,1)(.2,0)(.6,0)(.8,1)(1,1)",
                           "(0,0)(.2,0)(.4,1)(.8,1)(1,0)",
                           "(0,1)(.4,1)(.6,0)(1,0)");
        }
    }

    class StandardColorMap : RGBColorMap
    {
        public override void Initialize()
        {
            _type = ColorMapTypes.Standard;
            _originalTable = new RGB[256];
            _table = new RGB[256];
            GenerateColors("(0,0)(.333,.3)(.333, 0)(.666,.3)(.666,.3)(1, 1)",
                           "(0,0)(.333,.3)(.333,.3)(.666, 1)(.666, 0)(1,.3)",
                           "(0,0)(.333, 1)(.333, 0)(.666,.3)(.666, 0)(1,.3)");
        }
    }

    class CustomLIColorMap : RGBColorMap
    {
        public CustomLIColorMap(string rPoints, string gPoints, string bPoints)
        {
            _type = ColorMapTypes.Advanced;
            _rPoints = rPoints;
            _gPoints = gPoints;
            _bPoints = bPoints;
        }

        public override void Initialize()
        {
            _originalTable = new RGB[256];
            _table = new RGB[256];
            GenerateColors(_rPoints, _gPoints, _bPoints);
        }

        protected override void SaveCore(XmlWriter w)
        {
            w.WriteStartElement("LIP");
            w.WriteStartElement("Red");
            w.WriteString(_rPoints);
            w.WriteEndElement();
            w.WriteStartElement("Green");
            w.WriteString(_gPoints);
            w.WriteEndElement();
            w.WriteStartElement("Blue");
            w.WriteString(_bPoints);
            w.WriteEndElement();
            w.WriteEndElement();
        }

        private string _rPoints;
        private string _gPoints;
        private string _bPoints;
    }
}