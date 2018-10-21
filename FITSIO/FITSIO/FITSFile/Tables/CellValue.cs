using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using Bit = System.Boolean;

namespace Najm.FITSIO
{
    internal interface ICellValueBuilder
    {
        void Build();
    }

    internal class CellValue<T> : ICellValue<T>, ICellValueBuilder
    {
        internal CellValue(byte[] data, long offset, int depth)
        {
            _depth = depth;
            _rawData = data;
            _offset = offset;
        }

        public virtual T this[int index] { get { return _value[index]; } }
        public override string ToString()
        {
            string s = "";
            if (_depth == 1)
            {
                s = this[0].ToString();
            }
            else
            {
                StringBuilder sb = new StringBuilder(_depth * 12);
                sb.Append("(");
                for (int i = 0; i < _depth; i++)
                {
                    T e = _value[i];
                    sb.Append(e.ToString());
                    if (i < _depth - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.Append(")");
                s = sb.ToString();
            }
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected T[] _value;
        protected int _depth;
        protected byte[] _rawData;
        protected long _offset;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region ICellValue<T> Members
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        int ICellValue<T>.Depth { get { return _depth; } }
        T ICellValue<T>.this[int index] { get { return _value[index]; } }
        public T[] Vector { get { return _value; } }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region ICellValueBuilder Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Read return depth so that we can override its value for strings to be 1 instead
        // of the number of characters in the string.
        public virtual void Build()
        {
            _value = new T[_depth];
            int elementSize = Marshal.SizeOf(default(T));
            int size = _depth * elementSize;
            // fix byte order
            byte[] data = new byte[size];
            int offset = 0;
            for (int e = 0; e < _depth; e++)
            {
                for (int i = 0; i < elementSize; i++)
                {
                    data[offset + i] = _rawData[_offset + offset + elementSize - 1 - i];
                }
                offset += elementSize;
            }
            Buffer.BlockCopy(data, 0, _value, 0, size);
        }
        public int Depth { get { return _depth; } }
        #endregion
    }

    internal class CellBitValue : CellValue<Bit>
    {
        internal CellBitValue(byte[] data, long offset, int depth) : base(data, offset, depth) { }
        public override void Build()
        {
            // calc size of buffer
            int size = (_depth + 7) / 8;

            // extract data
            byte[] bytes = new byte[size];
            Buffer.BlockCopy(_rawData, (int)_offset, bytes, 0, size);

            // alloc bit array
            _value = new BitArray(bytes);
        }
        public override string ToString()
        {
            return _value.ToString();
        }
        public override bool this[int index] { get { return _value[index]; } }
        private new BitArray _value;
    }

    internal class CellBoolValue : CellValue<bool>
    {
        internal CellBoolValue(byte[] data, long offset, int depth) : base(data, offset, depth) { }
        public override void Build()
        {
            _value = new bool[_depth];
            for (int i = 0; i < _depth; i++)
            {
                _value[i] = (_rawData[_offset + i] == 'T');
            }
        }
    }

    internal class CellStringValue : CellValue<string>
    {
        internal CellStringValue(byte[] data, long offset, int depth) : base(data, offset, depth) { }
        public override void Build()
        {
            // convert to single string
            string val = System.Text.ASCIIEncoding.ASCII.GetString(_rawData, (int)_offset, _depth).Trim();
            _value = new string[1] { val };
            _depth = 1; // make it only one string item
        }
    }
}
