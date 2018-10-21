using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Najm.ImagingCore.ColorScaling
{
    // HE = Histogram Equalization
    internal class HEscaling : IScalingAlgorithm
    {
        public byte[] Apply(int[] image, int width, int height, int numActivepixels, double[] colorTable, double dataMin, double dataMax)
        {
            // first build linear 8-bit map
            IScalingAlgorithm alg = ScalingAlgorithmFactory.Create(ScalingAlgorithms.Linear);

            double[] hist = BuildHistogram(image, width, height, numActivepixels, colorTable);
            return alg.Apply(image, width, height, numActivepixels, hist, dataMin, dataMax);
        }

        internal double[] BuildHistogram(int[] image, int width, int height, int numActivepixels, double[] colorTable)
        {
            double[] hist = new double[colorTable.Length];
            double inc = 1.0 / numActivepixels;

            // first, PDF
            int index = 0;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int tblIndex = image[index++];
                    if (tblIndex < colorTable.Length)   // index will not satisfy this for blank pixels (NaN values)
                    {
                        hist[tblIndex] += inc;
                    }
                }
            }

            // to CPDF and 8-bit color scaling
            double sum = 0;
            for (int i = 0; i < colorTable.Length; i++)
            {
                sum += hist[i];
                hist[i] = sum;
            }

            return hist;
        }
        public ScalingAlgorithms Type { get { return ScalingAlgorithms.HistoEqualize; } }
    }
}
