using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPython
{
    class PythonHandlerException : Exception
    {
        public PythonHandlerException(string mes) : base(mes) { }
    }
}
