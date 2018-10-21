using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.FITSIO
{
    internal class RowEnumerator : IEnumerator<ITableRow>
    {
        private HDUTable _table;
        private long _cursor;
        private long _numRows;
        private TableRow _currentRow;
        internal RowEnumerator(HDUTable table)
        {
            _table = table;
            _cursor = -1;
            _numRows = table.Length;
            _currentRow = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IEnumerator Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        object System.Collections.IEnumerator.Current { get { return _currentRow; } }
        public bool MoveNext()
        {
            bool ret = true;
            _cursor++;
            if (_cursor >= _numRows)
            {
                ret = false;
            }
            else
            {
                _currentRow = new TableRow((TableHeader)_table.Header, _cursor);
            }
            return ret;
        }
        public void Reset()
        {
            _cursor = -1;
            _currentRow = null;
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IEnumerator<ITableRow> Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ITableRow IEnumerator<ITableRow>.Current { get { return _currentRow; } }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IDisposable Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Dispose() { }
        #endregion
    }

    class HDUTable : ITable
    {
        internal HDUTable(HDU hdu, TableType type, long numRows, int numFields, long bytesPerRow)
        {
            _hdu = hdu;
            _type = type;
            _numRows = numRows;
            _bytesPerRow = bytesPerRow;
            _data = null;
            // IMPORTANT: TableHeader must be constructed after setting the type of the table
            //            this is because it will call HDUTable and get the type to decide
            //            what kind of IFieldInfo & ITableCell implementation to use
            _header = new TableHeader(this, numFields);            
        }
        internal void HandleCardImage(CardImage ci)
        {
            string key = ci.Key;
            string val = ci.Value;
            if (key.StartsWith("TBCOL") || key.StartsWith("TFORM") || key.StartsWith("TSCAL") ||
                            key.StartsWith("TZERO") || key.StartsWith("TNULL") || key.StartsWith("TTYPE") ||
                            key.StartsWith("TUNIT") || key.StartsWith("TDISP"))
            {
                _header.InitFromCardImage(key, val);
            }
        }
        internal void FinalizeConstruction()
        {
            _header.FinalizeConstruction(_bytesPerRow);
        }
        internal byte[] Data
        {
            set { _data = value; }
            get { return _data; }
        }
        public long DataSize { get { return _numRows * _bytesPerRow; } }
        internal long BytesPerRow { get { return _bytesPerRow; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region ITable Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ITableHeader Header { get { return _header; } }
        public ITableRow this[long index] { get { return new TableRow(_header, index); } }
        public long Length { get { return _numRows; } }
        public IHDU HDU { get { return _hdu; } }
        public TableType Type { get { return _type; } }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IEnumerable<ITableRow> Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEnumerator<ITableRow> GetEnumerator() { return new RowEnumerator(this); }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IEnumerable Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return new RowEnumerator(this); }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        private TableHeader _header;
        private long _numRows;
        private long _bytesPerRow;
        private byte[] _data;
        private TableType _type;
        private HDU _hdu;
        #endregion
    }
}
