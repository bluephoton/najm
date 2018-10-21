using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Najm.FITSIO
{
    internal abstract class TableCellBase : ITableCell
    {
        internal TableCellBase(TableRow row, FieldInfoBase fi)
        {
            _fieldInfo = fi;
            _row = row;
            _rawData = (row.Header.Table as HDUTable).Data;
            _offset = long.MaxValue;    // I'd like something really bad to happen if offset is not initialized!
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region ITableCell Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int Depth { get { return _fieldInfo.Depth; } }
        public long ElementSize { get { return _fieldInfo.ElementSize; } }
        public void get<T>(out ICellValue<T> val)
        {
            GetValue();
            val = (ICellValue<T>)_value;
        }
        public T Value<T>()
        {
            GetValue();
            return ((ICellValue<T>)_value)[0];
        }
        #endregion

        protected void GetValue()
        {
            if (_value == null)
            {
                switch (_fieldInfo.Type)
                {
                    case FieldType.Bool:
                        _value = new CellBoolValue(_rawData, _offset, Depth);
                        break;
                    case FieldType.Bits:
                        _value = new CellBitValue(_rawData, _offset, Depth);
                        break;
                    case FieldType.Byte:
                        _value = new CellValue<byte>(_rawData, _offset, Depth);
                        break;
                    case FieldType.Int16:
                        _value = new CellValue<Int16>(_rawData, _offset, Depth);
                        break;
                    case FieldType.Int32:
                        _value = new CellValue<Int32>(_rawData, _offset, Depth);
                        break;
                    case FieldType.Int64:
                        _value = new CellValue<Int64>(_rawData, _offset, Depth);
                        break;
                    case FieldType.Char:
                        _value = new CellStringValue(_rawData, _offset, Depth);
                        break;
                    case FieldType.Float:
                        _value = new CellValue<float>(_rawData, _offset, Depth);
                        break;
                    case FieldType.Double:
                        _value = new CellValue<double>(_rawData, _offset, Depth);
                        break;
                    case FieldType.ComplexF:
                        _value = new CellValue<Complex<float>>(_rawData, _offset, Depth);
                        break;
                    case FieldType.ComplexD:
                        _value = new CellValue<Comparer<double>>(_rawData, _offset, Depth);
                        break;
                    default:
                        throw new Exception("Invalid cell data type");
                }
                _value.Build();
            }
        }

        public override string ToString()
        {
            GetValue();
            return _value.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected FieldInfoBase _fieldInfo;
        protected TableRow _row;
        protected byte[] _rawData;
        protected long _offset;
        protected ICellValueBuilder _value;   // ICellValueBuilder is an interface used to hide the generic typt so i can
                                              // pass things arround easity. C# generics sucks!!!
        #endregion
    }
}
