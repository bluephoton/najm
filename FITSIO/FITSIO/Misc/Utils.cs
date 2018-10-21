using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.FITSIO
{
    internal class Utils
    {
        internal static void CheckBool(bool condition, string msg)
        {
            CheckBool(condition, msg, null);
        }

        internal static void CheckBool(bool condition, string msg, System.Exception inner)
        {
            if (!condition)
            {
                throw new Exception("Najm.FITSIO: " + msg, inner);
            }
        }

        internal static void CheckBool(bool condition, System.Exception e)
        {
            if (!condition)
            {
                throw e;
            }
        }

        internal static int FITSTypeCode(Type type)
        {
            int code = 0;
            if (type.Equals(typeof(byte)))
            {
                code = 8;
            }
            else if (type.Equals(typeof(short)))
            {
                code = 16;
            }
            else if (type.Equals(typeof(int)))
            {
                code = 32;
            }
            else if (type.Equals(typeof(float)))
            {
                code = -32;
            }
            else if (type.Equals(typeof(double)))
            {
                code = -64;
            }
            return code;
        }

        internal static void FixByteOrder(byte[] rawData, int elementSize)
        {
            NativeAPIs.FixByteOrder(rawData, elementSize);
        }

        internal static int FindFirstNotOf(string s, char c)
        {
            int i;
            for (i = 0; i < s.Length && s[i] == c; i++) ;
            return (i < s.Length) ? i : -1;
        }

        internal static void UnifyData(double[] unifiedData, byte[] rawData, long offset, int bitsPerPixel)
        {
            NativeAPIs.UnifyData(unifiedData, rawData, offset, bitsPerPixel);
        }
    }
}
