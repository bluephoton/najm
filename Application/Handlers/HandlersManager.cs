using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using Najm.FITSIO;
using Najm.Handlers.Integration;
using System.IO;
using System.Drawing;
using Najm.Config;
using Najm.UI;

namespace Najm.Handlers
{
    // INajmHandlersManager is available to handlers to ask Najm activate and close sessions and avoid
    // implementing their own session management logic
    // It will be passed to Handler on Initialize.
    internal class HandlersManager : INajmHandlersManager
    {
        internal HandlersManager(MainAppForm form)
        {
            _form = form;
            _nodesMap = new Dictionary<string, TreeNode>(10);
            _loadedAssemblies = new Dictionary<Guid,HandlerAssembly>(5);
            _sessions = new Dictionary<Guid, List<int>>(10);
        }

        internal bool IsHandlerLoaded(Guid hid) { return _loadedAssemblies.ContainsKey(hid); }
        internal INajmHandler this[Guid hid] { get { return _loadedAssemblies[hid].Handler; } }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Handler loading
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // called to load handlers from config file
        internal void LoadHandlers(IEnumerable<HandlerInfo> his)
        {
            foreach (HandlerInfo hi in his)
            {
                if (hi.Enabled)
                {
                    LoadHandler(hi);
                }
            }
        }

        internal void LoadHandler(HandlerInfo hi)
        {
            // create handler assembly object
            HandlerAssembly ha = new HandlerAssembly();
            // laod the assembly and get handler pieces
            ha.Load(hi.Location, hi.Assembly, hi.Param);
            // check if we don't have it already
            if (!_loadedAssemblies.ContainsKey(ha.Handler.ID))
            {
                // add it to our collection
                _loadedAssemblies[ha.Handler.ID] = ha;
                // is it our default?
                if (ha.IsDefault)
                {
                    _defaultHandlerAssembly = ha;
                }
                else
                {
                    // add toolstrip button
                    AddToolbarButton(ha.Handler.ID, ha.ToolstrupImage, ha.Tooltip);
                }
                // hook session events
                HookEvents(ha.Handler);

                // alloc collection to store sessions opened for this handler
                _sessions[ha.Handler.ID] = new List<int>(5);
            }
            else
            {
                ha.Unload();
            }
        }

        // will unload anything not in the passed list
        internal void LoadHandlersOnly(IEnumerable<HandlerInfo> his)
        {
            // TODO: you can do better than this. Yes you!

            // first unload what we need to unload
            List<Guid> loadedHandlers = new List<Guid>(_loadedAssemblies.Keys); // need to copy as I can't iterate on a cllection that is going to change inside this loop
            foreach (Guid id in loadedHandlers)
            {
                bool found = false;
                foreach (HandlerInfo hi in his)
                {
                    if (id.Equals(hi.Id))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    UnloadHandler(id);
                }
            }
            // then load these we need
            foreach (HandlerInfo hi in his)
            {
                if (!_loadedAssemblies.ContainsKey(hi.Id))
                {
                    LoadHandler(hi);
                }
            }
        }

        internal void UnloadHandler(Guid hid)
        {
            // can't unload default handler
            if (_loadedAssemblies.ContainsKey(hid) && !_defaultHandlerAssembly.Handler.ID.Equals(hid))
            {
                HandlerAssembly ha = _loadedAssemblies[hid];
                // first, close all sessions for this handler. closing session will modify the sessions list
                // as it will delete items from it. so, I'l copy it first
                int[] sessions = _sessions[hid].ToArray();
                foreach (int sid in sessions)
                {
                    CloseSession(hid, sid);
                }
                _sessions.Remove(hid);

                // remove button
                RemoveToolbarButton(hid);

                // unhook session events
                UnHookEvents(ha.Handler);

                // remove from our map
                _loadedAssemblies.Remove(hid);
            }
        }

        internal void ReloadHandler(Guid hid)
        {
            if (_loadedAssemblies.ContainsKey(hid) && !_defaultHandlerAssembly.Handler.ID.Equals(hid))
            {
                HandlerAssembly ha = _loadedAssemblies[hid];
                // unload
                UnloadHandler(hid);
                // reload
                LoadHandler(new HandlerInfo(hid, "", ha.Assembly, ha.LoadParam, true));
            }
        }

        private void HookEvents(INajmHandler nh)
        {
            // connect events
            nh.SessionOpened += new SessionOpenedDelegate(hduHandler_OnSessionOpened);
            nh.SessionActivated += new SessionActivatedDelegate(hduHandler_OnSessionActivated);
            nh.SessionClosed += new SessionClosedDelegate(hduHandler_OnSessionClosed);
            nh.NoSessions += new NoSessionsDelegate(hduHandler_OnNoSessions);
            nh.SessionDeactivated += new SessionDeactivatedDelegate(hduHandler_OnSessionDeactivated);
        }

        private void UnHookEvents(INajmHandler nh)
        {
            // connect events
            nh.SessionOpened -= new SessionOpenedDelegate(hduHandler_OnSessionOpened);
            nh.SessionActivated -= new SessionActivatedDelegate(hduHandler_OnSessionActivated);
            nh.SessionClosed -= new SessionClosedDelegate(hduHandler_OnSessionClosed);
            nh.NoSessions -= new NoSessionsDelegate(hduHandler_OnNoSessions);
            nh.SessionDeactivated -= new SessionDeactivatedDelegate(hduHandler_OnSessionDeactivated);
        }

        private void AddToolbarButton(Guid hid, Image i, string tooltip)
        {
            // load our built in in case handler didn't provide or we couldn't load its image
            if (i == null)
            {
                Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Najm.Handlers.MissingHandlerToolstripImage.png");
                i = (s == null) ? null : new Bitmap(s);
            }

            //if (i != null) // TODO: do we want to fail just because image is not there?
            {
                // create and add button
                ToolStripButton b = new ToolStripButton(i);
                b.Name = hid.ToString();
                b.ToolTipText = tooltip;
                _form.ToolStrip.Items.Add(b);
                
                // set tag to guid so we can tell when we get a notification which handler to invoke
                b.Tag = hid;

                // wire the event to Najm form
                b.Click += new EventHandler(_form.HandlerButton_Click);
                b.MouseUp += new MouseEventHandler(_form.HandlerButton_MouseUp);
                
                // add the image to the tab control so that its used when tab page is created for this handler
                if (!_form.TabControl.ImageList.Images.ContainsKey(hid.ToString()) && i!= null)
                {
                    _form.TabControl.ImageList.Images.Add(hid.ToString(), i);
                }
            }
        }

        private void RemoveToolbarButton(Guid hid)
        {
            string key = hid.ToString();
            if (_form.ToolStrip.Items.ContainsKey(key))
            {
                _form.ToolStrip.Items[key].Click -= _form.HandlerButton_Click;
                _form.ToolStrip.Items.RemoveByKey(key);
            }
            if (_form.TabControl.ImageList.Images.ContainsKey(key))
            {
                _form.TabControl.ImageList.Images.RemoveByKey(key);
            }
        }
        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region INajmHandlersManager implementation
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // attach handler to HDUs
        public int OpenSession(Guid guidID, IHDU[] hduList)
        {
            int sessionID = -1;
            if (_loadedAssemblies != null && _loadedAssemblies.ContainsKey(guidID))
            {
                INajmHandler hduHandler = _loadedAssemblies[guidID].Handler;
                if (hduHandler != null)
                {
                    if (hduHandler.CanHandle(hduList))
                    {
                        if (!_form.TabControl.TabPages.ContainsKey(guidID.ToString()))
                        {
                            // create tab page for this handler
                            _form.TabControl.TabPages.Add(guidID.ToString(), hduHandler.Name);
                            TabPage handlerTabPage = _form.TabControl.TabPages[guidID.ToString()];

                            //set image to be used
                            handlerTabPage.ImageKey = hduHandler.ID.ToString();

                            // initialize is called ONCE when the handler is attached an HDU for first time.
                            // paenl is passed at this time and will be used for all sessions.
                            hduHandler.Initialize(this, handlerTabPage);
                        }

                        sessionID = hduHandler.OpenSession(hduList);
                    }
                    else
                    {
                        throw new NajmException(string.Format("{0} can't handle these HDUs", hduHandler.Name));
                    }
                }
            }
            return sessionID;
        }
        public void CloseSession(Guid guidID, int sid)
        {
            if (_loadedAssemblies != null && _loadedAssemblies.ContainsKey(guidID))
            {
                INajmHandler hduHandler = _loadedAssemblies[guidID].Handler;
                if (hduHandler != null)
                {
                    hduHandler.CloseSession(sid);
                }
            }
        }
        public void ActivateSession(Guid guid, int session)
        {
            if (_loadedAssemblies != null && _loadedAssemblies.ContainsKey(guid))
            {
                INajmHandler hduHandler = _loadedAssemblies[guid].Handler;
                if (hduHandler != null)
                {
                    hduHandler.ActivateSession(session);
                }
            }
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Handler notifications
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void hduHandler_OnSessionOpened(Guid handlerID, int sessionId)
        {
            // add handler name as a child to this hdu so we can activate this specific
            // handler by simply selecting this child
            TreeNode n = _form.Tree.SelectedNode;
            if (n != null)
            {
                string key = string.Format("{0}:{1}", handlerID.ToString(), sessionId);
                TreeNode newNode = n.Nodes.Add(key, GetNameOfHandler(handlerID));
                // select before adding tag to prevent AfterSelect event from re-activating the session
                newNode.Tag = new KeyValuePair<Guid, int>(handlerID, sessionId);
                _nodesMap.Add(key, newNode);
                n.Expand();
            }

            // be sure UI tab for this session is selected.
            _form.TabControl.SelectedTab = _form.TabControl.TabPages[handlerID.ToString()];

            // keep track of sessions opened for each handler. I used this info to close
            // all sessions before unloading the handler
            _sessions[handlerID].Add(sessionId);
        }

        void hduHandler_OnSessionClosed(Guid handlerID, int sessionId)
        {
            string key = string.Format("{0}:{1}", handlerID, sessionId);
            if (_nodesMap.ContainsKey(key))
            {
                TreeNode n = _nodesMap[key];
                if (n != null)
                {
                    n.Remove();
                }
                _nodesMap.Remove(key);
            }
            // remove session from our list of sessions for this handler
            _sessions[handlerID].Remove(sessionId);
        }

        void hduHandler_OnNoSessions(Guid handlerID)
        {
            // remove the tab for this handler as we have no more sessions
            _form.TabControl.TabPages.RemoveByKey(handlerID.ToString());
        }

        void hduHandler_OnSessionActivated(Guid handlerID, int sessionId)
        {
            // be sure UI tab for this session is selected.
            _form.TabControl.SelectedTab = _form.TabControl.TabPages[handlerID.ToString()];
            // select the handler child node, but NOT if this is the default handler
            string key = string.Format("{0}:{1}", handlerID, sessionId);
            if (_nodesMap != null && _nodesMap.ContainsKey(key) && !handlerID.Equals(_defaultHandlerAssembly.Handler.ID))
            {
                TreeNode n = _nodesMap[key];
                // set tag to null before select to prevent AfterSelect event from activating again. restore it back
                object tag = n.Tag;
                n.Tag = null;
                _form.Tree.SelectedNode = n;
                n.Tag = tag;
            }
        }

        void hduHandler_OnSessionDeactivated(Guid handlerID, int id)
        {
        }

        internal string GetNameOfHandler(Guid g)
        {
            string name = "";
            if (_loadedAssemblies != null && _loadedAssemblies.ContainsKey(g))
            {
                name = _loadedAssemblies[g].Handler.Name;
            }
            return name;
        }
        #endregion

        internal INajmHandler DefaultHandler { get { return _defaultHandlerAssembly.Handler; } }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private Dictionary<string, TreeNode> _nodesMap;
        private Dictionary<Guid, HandlerAssembly> _loadedAssemblies;
        private MainAppForm _form;
        private HandlerAssembly _defaultHandlerAssembly;
        private Dictionary<Guid, List<int>> _sessions;
        #endregion
    }
}
