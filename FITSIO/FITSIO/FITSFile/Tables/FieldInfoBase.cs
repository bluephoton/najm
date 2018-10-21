using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Najm.FITSIO
{
    abstract class FieldInfoBase : IFieldInfo
    {
        internal FieldInfoBase(TableHeader th, int index)
        {
            _header = th;
            _index = index;
            _size = 0;
            _zero = double.NaN;
            _scale = double.NaN;
            _type = FieldType.Invalid;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region IFieldInfo Members
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Name { 
            get { return _name; }
            internal set { _name = value; }
        }
        abstract public long Size { get; internal set;}
        public int Index { get { return _index; } }
        public double Zero { 
            get { return _zero; }
            internal set { _zero = value; }
        }
        public double Scale { 
            get { return _scale; }
            internal set { _scale = value; }
        }
        public string NULL { 
            get { return _null; }
            internal set { _null = value; }
        }
        public string Unit { 
            get { return _unit; }
            internal set { _unit = value; }
        }
        public FieldType Type { 
            get { return _type; }
            internal set { _type = value; }
        }
        public ITableHeader Header { get { return _header; } }
        public string Format { 
            get { return _format; }
            internal set
            {
                _format = value.Trim();
                // make sure its upper
                _format = _format.ToUpper();
                ParseCellFormat();
            }
        }
        public string DisplayFormat
        {
            get { return _displayFormat; }
            set { _displayFormat = value; }
        }
        #endregion


        internal long Offset { 
            set { _offset = value; }
            get { return _offset; }
        }
        abstract protected void ParseCellFormat();

        internal int Depth { get { return _repeat; } }
        internal long ElementSize { get { return _size; } }

        protected void SetFieldType()
        {
            switch (_typeChar)
            {
                case 'L':
                    _type = FieldType.Bool;
                    break;
                case 'X':
                    _type = FieldType.Bits;
                    break;
                case 'B':
                    _type = FieldType.Byte;
                    break;
                case 'I':
                    _type = FieldType.Int16;
                    break;
                case 'J':
                    _type = FieldType.Int32;
                    break;
                case 'K':
                    _type = FieldType.Int64;
                    break;
                case 'A':
                    _type = FieldType.Char;
                    break;
                case 'E':
                    _type = FieldType.Float;
                    break;
                case 'D':
                    _type = FieldType.Double;
                    break;
                case 'C':
                    _type = FieldType.ComplexF;
                    break;
                case 'M':
                    _type = FieldType.ComplexD;
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
        }


        internal Type DotNetType
        {
            get
            {
                Type t = null;
                switch (_typeChar)
                {
                    case 'L':
                        t = typeof(bool);
                        break;
                    case 'X':
                        t = typeof(BitArray);
                        break;
                    case 'B':
                        t = typeof(byte);
                        break;
                    case 'I':
                        t = typeof(short);
                        break;
                    case 'J':
                        t = typeof(int);
                        break;
                    case 'K':
                        t = typeof(Int64);
                        break;
                    case 'A':
                        t = typeof(char);
                        break;
                    case 'E':
                        t = typeof(float);
                        break;
                    case 'D':
                        t = typeof(double);
                        break;
                    case 'C':
                        t = typeof(Complex<float>);
                        break;
                    case 'M':
                        t = typeof(Complex<double>);
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
                return t;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private string _name;
        protected TableHeader _header;
        private int _index;
        protected long _size;
        protected long _offset;
        private double _zero;
        private double _scale;
        private string _null;
        private string _unit;
        protected string _format;
        private string _displayFormat;
        protected char _typeChar;
        protected FieldType _type;
        protected int _repeat;
        #endregion
    }
}
