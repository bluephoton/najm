using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.FITSIO
{
    internal class FieldInfoEnum : IEnumerator<IFieldInfo>
    {
        internal FieldInfoEnum(TableHeader header)
        {
            _header = header;
            _current = -1;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int _current;
        private TableHeader _header;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IEnumerator Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        object System.Collections.IEnumerator.Current { get { return _header[_current]; } }
        public bool MoveNext()
        {
            bool ret = true;
            _current++;
            if (_current >= _header.FieldsCount)
            {
                ret = false;
            }
            return ret;
        }
        public void Reset() { _current = -1; }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IEnumerator<IFieldInfo> Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        IFieldInfo IEnumerator<IFieldInfo>.Current { get { return _header[_current]; } }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IDisposable Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Dispose() { }
        #endregion
    }

    internal class TableHeader : ITableHeader
    {
        internal TableHeader(HDUTable table, int numFields)
        {
            _table = table;
            _numFields = numFields;
            _fieldNameIndexMap = new Dictionary<string, int>(numFields);
            _fieldsInfo = new List<FieldInfoBase>(numFields);
            for (int i = 0; i < numFields; i++)
            {
                switch (table.Type)
                {
                    case TableType.ASCII:
                        _fieldsInfo.Add(new FieldInfoA(this, i));
                        break;
                    case TableType.Binary:
                        _fieldsInfo.Add(new FieldInfoB(this, i));
                        break;
                    default:
                        throw new Exception("Invalid table type encountered");
                }
            }
        }

        internal void FinalizeConstruction(long bytesPerRow)
        {
            switch (_table.Type)
            {
                case TableType.ASCII:
                    FinalizeAsciiConstruction(bytesPerRow);
                    break;
                case TableType.Binary:
                    FinalizeBinaryConstruction();
                    break;
                default:
                    throw new Exception("Invalid table type encountered");
            }
        }

        private void FinalizeBinaryConstruction()
        {
            long offset = 0;
            for (int i = 0; i < _numFields; i++)
            {
                _fieldsInfo[i].Offset = offset;
                offset += _fieldsInfo[i].Size;
            }
        }

        private void FinalizeAsciiConstruction(long bytesPerRow)
        {
            for (int i = 0; i < _numFields; i++)
            {
                if (i == (_numFields - 1))
                {
                    // last one
                    _fieldsInfo[i].Size = bytesPerRow - _fieldsInfo[i].Offset + 1;
                }
                else
                {
                    _fieldsInfo[i].Size = _fieldsInfo[i + 1].Offset - _fieldsInfo[i].Offset;
                }
            }
        }

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region ITableHeader Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int FieldsCount { get { return _numFields; } }
        public IFieldInfo this[int index] { get { return _fieldsInfo[index]; } }
        public IFieldInfo this[string name] { get { return _fieldsInfo[_fieldNameIndexMap[name]]; } }
        public ITable Table { get { return _table; } }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IEnumerable<IFieldInfo> Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEnumerator<IFieldInfo> GetEnumerator()
        {
            return new FieldInfoEnum(this);
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IEnumerable Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new FieldInfoEnum(this);
        }
        #endregion

        internal void InitFromCardImage(string key, string val)
        {
            int fieldIndex = GetFieldIndex(key, 5);
            FieldInfoBase fi = _fieldsInfo[fieldIndex];
            key = key.Substring(0, 5);
            switch (key)
            {
                case "TBCOL":
                    fi.Offset = int.Parse(val);
                    break;
                case "TFORM":
                    fi.Format = val;
                    break;
                case "TSCAL":
                    fi.Scale = double.Parse(val);
                    break;
                case "TZERO":
                    fi.Zero = double.Parse(val);
                    break;
                case "TNULL":
                    fi.NULL = val;
                    break;
                case "TTYPE":
                    fi.Name = val;
                    break;
                case "TUNIT":
                    fi.Unit = val;
                    break;
                case "TDISP":
                    fi.DisplayFormat = val;
                    break;
                default:
                    Utils.CheckBool(false, new TableException("Invalid cardimage used to initialzed binary table header"));
                    break;
            }
        }

        private int GetFieldIndex(string key, int offset)
        {
            int fieldIndex = int.Parse(key.Substring(offset));
            fieldIndex--;	// on the FITS file, index start from 1
            Utils.CheckBool(fieldIndex >= 0 && fieldIndex < _fieldsInfo.Count, "Table fields exceeded the expected count");
            return fieldIndex;
        }

        #region data members
        private HDUTable _table;
        private int _numFields;
        private List<FieldInfoBase> _fieldsInfo;
        private Dictionary<string, int> _fieldNameIndexMap;
        #endregion
    }
}
