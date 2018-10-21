using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Najm.FITSIO
{
    internal class FieldInfoB : FieldInfoBase
    {
        internal FieldInfoB(TableHeader th, int index)
            : base(th, index)
        {
        }

        #region IFieldInfo Members
        override public long Size
        {
            get { return _size; }
            // Called for ASCII tables only.
            internal set
            {
                throw new Exception("Call to Size.Set is not supported for Binary tables");
            }
        }
        #endregion

        protected override void ParseCellFormat()
        {
            // shouldn't be empty
            Utils.CheckBool(!string.IsNullOrEmpty(_format), new TableException("Invalid cell format"));

            // find format char
            int ind = _format.IndexOfAny((new char[] {'L','X','B','I','J','K','A','E','D','C','M','P','Q'}));
            Utils.CheckBool(ind != -1, new TableException("Invalid cell format"));
            _typeChar = _format[ind];
            SetFieldType();

            // get repeat count. If missing, 1 is implied
            _repeat = 1;
            if(ind > 0)
            {
                _repeat = int.Parse(_format.Substring(0, ind));
            }

            // get extra junk
            long extra = (++ind < _format.Length) ? long.Parse(_format.Substring(ind)) : 0;

            _size = SizeOfType * _repeat + extra;
        }

        internal long SizeOfType
        {
            get
            {
                long s = 0;
                switch (_typeChar)
                {
                    case 'L':
                    case 'X':
                    case 'B':
                        s = sizeof(byte);
                        break;
                    case 'I':
                        s = sizeof(short);
                        break;
                    case 'J':
                        s = sizeof(int);
                        break;
                    case 'K':
                        s = sizeof(Int64);
                        break;
                    case 'A':
                        s = sizeof(byte);
                        break;
                    case 'E':
                        s = sizeof(float);
                        break;
                    case 'D':
                        s = sizeof(double);
                        break;
                    case 'C':
                        s = sizeof(float ) * 2;
                        break;
                    case 'M':
                        s = sizeof(double) * 2;
                        break;
                    case 'P':
                        Utils.CheckBool(false, "Not supported yet");
                        break;
                    case 'Q':
                        Utils.CheckBool(false, "Not supported yet");
                        break;
                    default:
                        Utils.CheckBool(false, "Invalid TFORM fild type encountered");
                        break;
                }
                return s;
            }
        }

        #region data members
        #endregion
    }
}
