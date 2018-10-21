using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.FITSIO
{
    public class Exception : System.Exception
    {
        public Exception(string msg) : base(msg) { }
        public Exception(string msg, System.Exception inner) : base(msg, inner) { }
    }

    public class TableException : System.Exception
    {
        public TableException(string msg) : base(msg) { }
        public TableException(string msg, System.Exception inner) : base(msg, inner) { }
    }
}
