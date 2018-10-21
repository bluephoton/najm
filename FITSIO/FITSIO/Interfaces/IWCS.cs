using System;
using System.Collections.Generic;
using System.Text;
using Najm.LinearAlagebra;

namespace Najm.FITSIO
{
    public interface IWCSInfo
    {
        int NumAlternatives { get; }
        ITransformation PrimaryTransformation { get; }
        ITransformation this[int index] { get; }
        ITransformation this[char letter] { get; }
    }

    public interface ITransformation
    {
        Matrix LinearTransformation { get; }
        Vector PixelReference { get; }
        Vector WorldReference { get; }
        Vector Pixel2Physical { get; }
        IWCSAxis[] Axes { get; }
        bool IsPrimary { get; }
        string Name { get; }
        Vector RandomError { get; }
        Vector SystematicError { get; }
    }

    public interface IWCSAxis
    {
        string Unit { get; }
        string CoordinateType { get; }
        string AlgorithmCode { get; }
        double[] NumericParams { get; }
        string[] TextParams { get; }
    }
}
