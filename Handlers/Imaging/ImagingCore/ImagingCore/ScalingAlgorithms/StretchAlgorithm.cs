using System;
using System.Collections.Generic;
using System.Text;

///////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Those algorithms are pretty much copied from DS9. My goal was to get similar results as best as i can.
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Najm.ImagingCore.ColorScaling
{
    public enum ScalingAlgorithms { SquareRoot, Logarithmic, Linear, Square, HistoEqualize, Custom };
    
    public interface IScalingAlgorithm
    {
        byte[] Apply(int[] image, int width, int height, int numActivepixels, double[] colorTable, double dataMin, double dataMax);
        ScalingAlgorithms Type { get; }
    }

    public class ScalingAlgorithmFactory
    {
        public static IScalingAlgorithm Create(ScalingAlgorithms sa)
        {
            IScalingAlgorithm alg = null;

            switch (sa)
            {
                case ScalingAlgorithms.Linear:
                    alg = new Linearscaling();
                    break;
                case ScalingAlgorithms.SquareRoot:
                    alg = new SquareRootscaling();
                    break;
                case ScalingAlgorithms.Logarithmic:
                    alg = new Logarithmicscaling();
                    break;
                case ScalingAlgorithms.HistoEqualize:
                    alg = new HEscaling();
                    break;
                case ScalingAlgorithms.Square:
                    alg = new SquareScaling();
                    break;
                case ScalingAlgorithms.Custom:
                    alg = new Customscaling();
                    break;
                default:
                    break;
            }
            return alg;
        }
    }

    internal class Linearscaling : IScalingAlgorithm
    {
        public byte[] Apply(int[] image, int width, int height, int numActivepixels, double[] colorTable, double dataMin, double dataMax)
        {
            byte[] data = new byte[colorTable.Length];
            for (int i = 0; i < colorTable.Length; i++)
            {
                data[i] = (byte)(int)(255.0 * colorTable[i] + 0.5);
            }
            return data;
        }
        public ScalingAlgorithms Type { get { return ScalingAlgorithms.Linear; } }
    }

    internal class SquareRootscaling : IScalingAlgorithm
    {
        public byte[] Apply(int[] image, int width, int height, int numActivepixels, double[] colorTable, double dataMin, double dataMax)
        {
            byte[] data = new byte[colorTable.Length];
            for (int i = 0; i < colorTable.Length; i++)
            {
                data[i] = (byte)(int)((255.0 * Math.Sqrt(colorTable[i])) + 0.5);
            }
            return data;
        }
        public ScalingAlgorithms Type { get { return ScalingAlgorithms.SquareRoot; } }
    }

    internal class SquareScaling : IScalingAlgorithm
    {
        public byte[] Apply(int[] image, int width, int height, int numActivepixels, double[] colorTable, double dataMin, double dataMax)
        {
            byte[] data = new byte[colorTable.Length];
            for (int i = 0; i < colorTable.Length; i++)
            {
                data[i] = (byte)(int)((255.0 * colorTable[i] * colorTable[i]) + 0.5);
            }
            return data;
        }
        public ScalingAlgorithms Type { get { return ScalingAlgorithms.Square; } }
    }

    internal class Logarithmicscaling : IScalingAlgorithm
    {
        public byte[] Apply(int[] image, int width, int height, int numActivepixels, double[] colorTable, double dataMin, double dataMax)
        {
            // Validate Min/Max values
            ValidateMinMax(ref dataMin, ref dataMax);
            byte[] data = new byte[colorTable.Length];
            for (int i = 0; i < colorTable.Length; i++)
            {
                double colorValue = (colorTable[i] - colorTable[0]) / (colorTable[colorTable.Length - 1] - colorTable[0]);
                double numerator = Math.Log10((dataMax / dataMin) * colorValue + 1);
                double denominator = Math.Log10((dataMax / dataMin) + 1);
                int ind = (int)(numerator * (colorTable.Length - 1) / denominator);
                System.Diagnostics.Debug.Assert(ind < colorTable.Length);
                data[i] = (byte)(int)(255.0 * numerator / denominator + 0.5);
            }
            return data;
        }

        // copied as is from DS9
        private void ValidateMinMax(ref double dataMin, ref double dataMax)
        {
            double magnitude = 10000;
            if (dataMin > 0 && dataMax > 0)
            {
                // we're good! - most probable case first.
            }
            else if (dataMin == 0 && dataMax == 0)
            {
                dataMax = 1;
                dataMin = dataMax / magnitude;
            }
            else if (dataMin == 0 && dataMax > 0)
            {
                dataMin = dataMax / magnitude;
            }
            else if (dataMin < 0 && dataMax == 0)
            {
                dataMax = -dataMin;
                dataMin = dataMax / magnitude;
            }
            else if (dataMin < 0 && dataMax < 0)
            {
                double t = dataMax;
                dataMax = -dataMin;
                dataMin = -t;
            }
            else
            {
                dataMin = dataMax / magnitude;
            }
        }
        public ScalingAlgorithms Type { get { return ScalingAlgorithms.Logarithmic; } }
    }

    internal class Customscaling : IScalingAlgorithm
    {
        public byte[] Apply(int[] image, int width, int height, int numActivepixels, double[] colorTable, double dataMin, double dataMax)
        {
            byte[] data = new byte[colorTable.Length];
            for (int i = 0; i < colorTable.Length; i++)
            {
                data[i] = (byte)(int)((255.0 * (1 - colorTable[i])) +0.5);
            }
            return data;
        }
        public ScalingAlgorithms Type { get { return ScalingAlgorithms.Custom; } }
    }

}
