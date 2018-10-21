using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Najm.ImagingCore.ColorScaling;

namespace Najm.ImagingCore.ColorTables
{
    abstract class BaseColorTable : IColorTable
    {

        #region IColorTable Members
        public abstract void Initialize(int size, double minColorValue, double maxColorValue);
        public abstract int Size { get; }
        public abstract IScalingAlgorithm ScalingAlgorithm {get; set;}
        public abstract int Add(double pixelValue);
        public abstract void Normalize();
        public abstract void ScaleIt(int[] image, int width, int height);
        public abstract bool Lookup(int pixelIndex, out int pixelColor);
        public abstract double Minimum { get; }
        public abstract double Maximum { get; }
        public void Save(Stream s)
        {
            XmlWriter w = null;
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                w = XmlWriter.Create(s, settings);
                w.WriteStartElement("ColorTable");
                w.WriteAttributeString("Type", "Indexed");
                w.WriteAttributeString("Size", _size.ToString());
                w.WriteAttributeString("Scale", _scalingAlgorithm.Type.ToString());
                w.WriteAttributeString("Minimum", _minimum.ToString());
                w.WriteAttributeString("Maximum", _maximum.ToString());
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
        #endregion

        protected int _size = 0;
        protected double _minimum;
        protected double _maximum;
        protected int _numActivePixels;   // since some pixels are inactive and have NaN value, we need to know the
        // cound of the actual pixels so we use it to properly calculate histogram
        // and anything else that might need this piece of info.
        protected byte[] _lookupTable;

        protected IScalingAlgorithm _scalingAlgorithm;
    }
}
