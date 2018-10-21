using System;
using System.Collections.Generic;
using System.Text;
using Najm.FITSIO;
using System.Windows.Forms;

namespace Najm.Handlers.Integration
{
    public delegate void SessionOpenedDelegate(Guid handlerID, int id);
    public delegate void SessionClosedDelegate(Guid handlerID, int id);
    public delegate void NoSessionsDelegate(Guid handlerID);
    public delegate void SessionActivatedDelegate(Guid handlerID, int id);
    public delegate void SessionDeactivatedDelegate(Guid handlerID, int id);

    public interface INajmHandler
    {
        void Load(string arg);                  // this is called right after the handler assembly is loaded
        Guid ID { get; }                        // uniquely identify the assembly
        string Name { get; }                    // for display purposes
        bool CanHandle(IHDU[] hdus);
        void Initialize(INajmHandlersManager nhm, Panel panel);
        int OpenSession(IHDU[] hdus);
        void CloseSession(int sessionID);
        void ActivateSession(int sessionID);
        string ToolstripImageName { get; }
        string Tooltip { get; }

        event SessionOpenedDelegate SessionOpened;
        event SessionClosedDelegate SessionClosed;
        event NoSessionsDelegate NoSessions;
        event SessionActivatedDelegate SessionActivated;
        event SessionDeactivatedDelegate SessionDeactivated;
    }
}
