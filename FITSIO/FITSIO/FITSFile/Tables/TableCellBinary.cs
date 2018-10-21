using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Najm.FITSIO
{
    class TableCellBinary : TableCellBase
    {
        internal TableCellBinary(TableRow row, FieldInfoBase fi)
            : base(row, fi)
        {
            HDUTable table = row.Header.Table as HDUTable;
            _offset = table.BytesPerRow * row.Index + fi.Offset;
        }
    }
}
