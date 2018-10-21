using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Najm.FITSIO
{
    internal class FieldInfoA : FieldInfoBase
    {
        internal FieldInfoA(TableHeader th, int index) : base(th, index)
        {
        }

        #region IFieldInfo Members
        override public long Size {
            get { return _size; }
            // Called for ASCII tables only.
            // since all fields are strings, I set the repeat value to the size
            // (size = repear * 1) where 1 is one c/c ascii type. This makes the
            // ascii table string equivalent to Binary table string and makes it 
            // transparent to upper layers.
            internal set
            {
                _size = value;
                _repeat = (int)value;
            }
        }
        #endregion

        protected override void ParseCellFormat()
        {
            char first = _format[0];
            // first char must be one of "AIFED"
            Utils.CheckBool(first == 'A' || first == 'I' || first == 'F' || first == 'E' || first == 'D', 
                                new TableException("Invalid cell format"));
            // content will be treated as string no matter the format. this is because real numbers for example has no
	        // limit, it can exceed the 64-bit limit of double. caller need to handel this.
            _typeChar = 'A';
            SetFieldType();
        }

        #region data members
        #endregion
    }
}
