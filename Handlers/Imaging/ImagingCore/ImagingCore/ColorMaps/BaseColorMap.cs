using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Najm.ImagingCore.ColorMaps
{
    abstract internal class BaseColorMap : IColorMap
    {
        public BaseColorMap()
        {
            _contrast = 1.0;
            _bias = 0.5;
        }

        public ColorMapTypes Type { get { return _type; } }
        public abstract void Initialize();
        public abstract void Lookup(int colorIndex, out RGB rgbColor);
        protected virtual void SaveCore(XmlWriter w)
        {
            // do nothing
        }
        public void Save(Stream s)
        {
            XmlWriter w = null;
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                w = XmlWriter.Create(s, settings);
                w.WriteStartElement("ColorMap");
                w.WriteAttributeString("Type", Type.ToString());
                w.WriteAttributeString("Bias", Bias.ToString());
                w.WriteAttributeString("Contrast", Contrast.ToString());
                SaveCore(w);
                w.WriteEndElement();
            }
            finally
            {
                if (w != null)
                {
                    w.Close();
                }
            }
        }
        public void Save(string path)
        {
            StreamWriter sw = null;
            XmlWriter w = null;
            try
            {
                sw = new StreamWriter(path, false);
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                w = XmlWriter.Create(sw.BaseStream, settings);
                w.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                w.Flush();
                Save(sw.BaseStream);
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

        public double Contrast
        {
            get { return _contrast; }
            set
            {
                _contrast = value;
                UpdateColors();
            }
        }

        public double Bias
        {
            get { return _bias; }
            set {
                _bias = value;
                UpdateColors();
            }
        }

        public int Length
        {
            get { return _table.Length; }
        }

        private void UpdateColors()
        {
            for (int i = 0; i < _table.Length; i++)
            {
                int j = (int)(MapIndex(i) * _originalTable.Length / _table.Length + 0.5);
                _table[i] = _originalTable[j];
            }
        }

        private int MapIndex(int index)
        {
            double tableLength = (double)_table.Length;
            double i = (double)index / tableLength;
            int ii = (int)(((i - _bias) * _contrast + 0.5) * tableLength);
            ii = (ii < 0) ? 0 : ii;
            ii = (ii >= _table.Length) ? (_table.Length - 1) : ii;
            return ii;
        }

        // table must be allocated before calling this helper method
        protected void GenerateColors(string rLI, string gLI, string bLI)
        {
            LinearInterpolator rC = new LinearInterpolator(rLI);
            LinearInterpolator gC = new LinearInterpolator(gLI);
            LinearInterpolator bC = new LinearInterpolator(bLI);
            double maxColor = (double)_table.Length - 1.0;
            for (int i = 0; i < _table.Length; i++)
            {
                double x = (double)i / maxColor;
                byte r = (byte)(int)(255.0 * rC.Interpolate(x) + 0.5);
                byte g = (byte)(int)(255.0 * gC.Interpolate(x) + 0.5);
                byte b = (byte)(int)(255.0 * bC.Interpolate(x) + 0.5);
                _table[i] = _originalTable[i] = new RGB(r, g, b);
            }
        }

        protected double _contrast;
        protected double _bias;
        protected RGB[] _originalTable;     // contains the set of colors as defined by the colormap type
        protected RGB[] _table;             // contains the set of colors after applying bias contrast on them. Always used for render.
        protected ColorMapTypes _type;
    }
}
