using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.Config
{
    internal class HandlerInfo
    {
        internal HandlerInfo(Guid id, string location, string assembly, string param, bool enabled)
        {
            _id = id;   // NOTE: **** This id is for config file only and has nothing with the handler id ****
                        //       **** internal to the handler.                                             ****
            _location = location;
            _assembly = assembly;
            _param = param;
            _enabled = enabled;
        }

        internal Guid Id { get { return _id; } }
        internal string Location { get { return _location; } }
        internal string Assembly { get { return _assembly; } }
        internal string Param { get { return _param; } }
        internal bool Enabled { 
            get { return _enabled; }
            set { _enabled = value; }
        }

        #region data
        private Guid _id;
        private string _location;
        private string _assembly;
        private string _param;
        private bool _enabled;
        #endregion
    }
}
