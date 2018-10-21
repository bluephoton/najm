using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Najm.FITSIO
{
    class NativeAPIs
    {
        [DllImport("Najm.Boost.dll", ExactSpelling = true, EntryPoint = "FixByteOrder", CharSet = CharSet.Ansi)]
        static private extern uint FixByteOrder(byte[] rawData, long dataSize, long elementSize);

        internal static void FixByteOrder(byte[] rawData, long elementSize)
        {
            FixByteOrder(rawData, rawData.Length, elementSize);
        }

        [DllImport("Najm.Boost.dll", ExactSpelling = true, EntryPoint = "UnifyData", CharSet = CharSet.Ansi)]
        static private extern uint UnifyData(double[] unifiedData, long numElements, byte[] rawData, long numBytes, long offset, int bitsPerPixel);

        internal static void UnifyData(double[] unifiedData, byte[] rawData, long offset, int bitsPerPixel)
        {
            UnifyData(unifiedData, unifiedData.Length, rawData, rawData.Length, offset, bitsPerPixel);
        }
    }
}
