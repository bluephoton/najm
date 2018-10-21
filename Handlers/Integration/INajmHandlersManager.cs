using System;
using System.Collections.Generic;
using System.Text;
using Najm.FITSIO;

namespace Najm.Handlers.Integration
{
    public interface INajmHandlersManager
    {
        int OpenSession(Guid hid, IHDU[] hduList);
        void CloseSession(Guid hid, int sid);
        void ActivateSession(Guid hid, int sid);
    }
}
