using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Najm.ImagingCore.ColorMaps
{
    class FileColorMap : BaseColorMap
    {
        public FileColorMap(string MapFile)
        {
            StreamReader sr = new StreamReader(MapFile);
            LoadFromStream(sr);
            sr.Close();
            _type = ColorMapTypes.RGBFile;
        }

        public FileColorMap(TextReader r)
        {
            LoadFromStream(r);
        }

        private void LoadFromStream(TextReader sr)
        {
            _originalTable = new RGB[256];

            int colorIndex = 0;
            string line;
            while ((line = sr.ReadLine()) != null && colorIndex < 256 * 3)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    line = line.Trim();
                    string[] colors = line.Split(new char[] { '\t', ',', ';', '|', ':' });
                    if (colors.Length != 3)
                    {
                        throw new FormatException("Invalid file format");
                    }
                    byte r, g, b, t;
                    r = g = b = 0;
                    for (int i = 0; i < 3; i++)
                    {

                        if (byte.TryParse(colors[i].Trim(), out t))
                        {
                            switch (i)
                            {
                                case 0:
                                    r = t;
                                    break;
                                case 1:
                                    g = t;
                                    break;
                                case 2:
                                    b = t;
                                    break;
                            }
                        }
                        else
                        {
                            throw new FormatException("Invalid file format");
                        }
                        _originalTable[colorIndex] = new RGB(r, g, b);
                    }
                    colorIndex++;
                    if (colorIndex >= 256)
                    {
                        break;
                    }
                }
            }
            if (colorIndex != 256)
            {
                throw new FormatException("Map file MUST contain exactly 256 entries");
            }
            _table = new RGB[_originalTable.Length];
            _originalTable.CopyTo(_table, 0);
        }

        public override void Initialize()
        {
            ;
        }

        public override void Lookup(int colorIndex, out RGB rgbColor)
        {
            rgbColor.R = _table[colorIndex].R;
            rgbColor.G = _table[colorIndex].G;
            rgbColor.B = _table[colorIndex].B;
        }

        protected override void SaveCore(XmlWriter w)
        {
            w.WriteStartElement("RGB");
            foreach (RGB c in _originalTable)
            {
                w.WriteString(string.Format("{0},{1},{2}", c.R, c.G, c.B));
            }
            w.WriteEndElement();
        }
    }
}
