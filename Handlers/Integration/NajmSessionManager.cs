using System;
using System.Collections.Generic;
using System.Text;
using Najm.FITSIO;

namespace Najm.Handlers.Integration
{
    public class NajmSessionManager<T>
    {
        public class SessionInfo
        {
            internal SessionInfo(int id, T data) {
                _id = id;
                _data = data;
            }
            public int ID { get { return _id; } }
            public T Data { get { return _data; } }
            private T _data;
            private int _id;
        }
        public NajmSessionManager()
        {
            _currentSessionID = -1;     // this is the current avaialbe id number to be assigned to newly added sessions
            _activeSessionID = -1;  // this is the id of the current active session
            _sessionsMap = new Dictionary<int, SessionInfo>();
        }
        public int AddSession(T d)
        {
            SessionInfo si = new NajmSessionManager<T>.SessionInfo(++_currentSessionID, d);
            _sessionsMap[_currentSessionID] = si;
            return _currentSessionID;
        }
        public void RemoveSession(int id)
        {
            // remove the session
            if (_sessionsMap.ContainsKey(id))
            {
                _sessionsMap.Remove(id);
                // make sure we always have a valid current active session
                if (_activeSessionID == id)
                {
                    _activeSessionID = -1;
                    foreach (SessionInfo si in _sessionsMap.Values)
                    {
                        _activeSessionID = si.ID;
                    }
                }
            }
        }
        public bool SessionExists(int id) { return _sessionsMap.ContainsKey(id); }
        public SessionInfo this[int id] { get { return _sessionsMap[id]; } }
        public bool Empty { get { return _sessionsMap.Count == 0; } }
        public SessionInfo ActiveSession { get { return _activeSessionID >= 0 ? _sessionsMap[_activeSessionID] : null; } }
        public int NextFreeSessionID { get { return _currentSessionID + 1; } }
        public void SetActiveSession(int sid) { _activeSessionID = sid; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int _currentSessionID;
        private int _activeSessionID;
        private Dictionary<int, SessionInfo> _sessionsMap;
        #endregion
    }
}
