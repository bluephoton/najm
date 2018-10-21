using System;
using System.Collections.Generic;
using System.Text;
using Najm.FITSIO;
using System.Windows.Forms;

namespace Najm.Handlers.Integration
{
    abstract public class NajmHandler<T> : NajmSessionManager<T>, INajmHandler
    {
        abstract protected T OpenSession(int sid, IHDU[] hdus);
        abstract protected void CloseSession(T sd);
        abstract protected void ActivateSession(T sd);

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region INajmHDUHandler Members
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        abstract public void Load(string arg);
        abstract public Guid ID { get; }
        abstract public string Name { get; }
        abstract public bool CanHandle(IHDU[] hdus);
        abstract public void Initialize(INajmHandlersManager nhm, Panel panel);
        int INajmHandler.OpenSession(IHDU[] hdus)
        {
            int sid = -1;
            T sd = OpenSession(NextFreeSessionID, hdus);
            sid = AddSession(sd);
            OnSessionOpened(ID, sid);
            return sid;
        }
        void INajmHandler.CloseSession(int sid)
        {
            if (SessionExists(sid))
            {
                // get the session info before we remove it from map
                SessionInfo oldSI = this[sid];
                
                // remove it
                RemoveSession(sid);

                // ask handler to close and give it the Session data of session just removed
                CloseSession(oldSI.Data);

                OnSessionClosed(ID, sid);
            }
        }
        void INajmHandler.ActivateSession(int sid)
        {
            // no need to activate if it is already active
            if (ActiveSession == null || ActiveSession.ID != sid)
            {
                ActivateSession(this[sid].Data);
                SetActiveSession(this[sid].ID);
            }
            // but we still need to notify for Najm to sync its UI elements
            OnSessionActivated(ID, sid);
        }
        abstract public string ToolstripImageName { get; }
        abstract public string Tooltip { get; }
        
        public event SessionOpenedDelegate SessionOpened;
        public event SessionClosedDelegate SessionClosed;
        public event NoSessionsDelegate NoSessions;
        public event SessionActivatedDelegate SessionActivated;
        public event SessionDeactivatedDelegate SessionDeactivated;
        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region event raising methods
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // now, I'm using the design pattern descriped in http://msdn.microsoft.com/en-us/library/hy3sefw3(VS.80).aspx to allow
        // derived classes to raise events here in the base class
        protected virtual void OnSessionOpened(Guid hid, int sid)
        {
            SessionOpenedDelegate d = SessionOpened;
            if (d != null)
            {
                d(hid, sid);
            }
        }
        protected virtual void OnSessionClosed(Guid hid, int sid)
        {
            SessionClosedDelegate d = SessionClosed;
            if (d != null)
            {
                d(hid, sid);
            }

            if (Empty)
            {
                OnNoSessions(ID);
            }
            else
            {
                ActivateSession(ActiveSession.Data);
            }

        }
        protected virtual void OnNoSessions(Guid hid)
        {
            NoSessionsDelegate d = NoSessions;
            if (d != null)
            {
                d(hid);
            }
        }
        protected virtual void OnSessionActivated(Guid hid, int sid)
        {
            SessionActivatedDelegate d = SessionActivated;
            if (d != null)
            {
                d(hid, sid);
            }
        }
        protected virtual void OnSessionDeactivated(Guid hid, int sid)
        {
            SessionDeactivatedDelegate d = SessionDeactivated;
            if (d != null)
            {
                d(hid, sid);
            }
        }
        #endregion
    }
}
