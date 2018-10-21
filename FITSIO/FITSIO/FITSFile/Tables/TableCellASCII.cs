using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Najm.FITSIO
{
    class TableCellASCII : TableCellBase
    {
        internal TableCellASCII(TableRow row, FieldInfoBase fi)
            : base(row, fi)
        {
            HDUTable table = row.Header.Table as HDUTable;
            _offset = table.BytesPerRow * row.Index + fi.Offset - 1;   	// -1 as FITS TBCOLn starts from 1 not from zero
        }
    }
}
