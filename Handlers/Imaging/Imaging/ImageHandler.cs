using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Najm.FITSIO;
using Najm.Handlers.Integration;

namespace ImageHandler
{
    public class SessionData
    {
        internal SessionData(int sid, IModel model, ImagingForm form)
        {
            _model = model;
            _imageForm = form;
            _sid = sid;
        }
        internal int SessionID { get { return _sid; } }
        internal ImagingForm _imageForm;
        internal int _sid;
        internal IModel _model;
    }

    public class ImageHandler : NajmHandler<SessionData>
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region NajmHandler implementation
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Load(string arg) { /*Nothing to be done!*/}
        public override Guid ID { get { return new Guid("{18E85847-29C2-4cb1-BDA5-0C971BFAB906}"); } }
        public override string Name { get { return "Najm Image Handler"; } }
        public override string ToolstripImageName { get { return "Imaging.ToolstriptImage.png"; } }
        public override string Tooltip { get { return "Invoke Najm Image Handler"; } }
        public override bool CanHandle(IHDU[] hdus)
        {
            return (hdus.Length == 1 && (hdus[0].IsImage) && hdus[0].Axes.Length > 1 && hdus[0].Axes[0].NumPoints > 0 && hdus[0].Axes[0].NumPoints > 0);
        }
        public override void Initialize(INajmHandlersManager nhm, Panel panel)
        {
            // keep the panel
            _panel = panel;
            
            // create it here as we have only one no matter how many sessions can be opened
            _ihUI = new ImageHandlerUI();
            //_ihUI.Dock = DockStyle.Fill;
            panel.Controls.Add(_ihUI);
            panel.Width = _ihUI.Width;
            panel.Height = _ihUI.Height;
            _NajmHandlersManager = nhm; 
        }
        protected override SessionData OpenSession(int sid, IHDU[] hdus)
        {
            // each session has its own model
            //////////////////////////////////////////////////////////////////
            IModel model = new ihUIModel(); // a model per session
            model.Initialize(hdus);

            // create image form and set the hdu
            //////////////////////////////////////////////////////////////////
            ImagingForm f = new ImagingForm(model);
            f.FormClosed += new FormClosedEventHandler(f_FormClosed);
            f.Activated += new EventHandler(f_Activated);

            // new session
            //////////////////////////////////////////////////////////////////
            SessionData sd = new SessionData(sid, model, f);
            f.SessionData = sd;

            // Save panel state of current session
            //////////////////////////////////////////////////////////////////
            if (ActiveSession != null)
            {
                ActiveSession.Data._model = _ihUI.Model;
            }

            return sd;
        }
        protected override void CloseSession(SessionData sd)
        {
            // clear panel if this was last session, otherwise set some other session to keep panel state valid
            if (Empty && _ihUI != null)
            {
                // clear panel
                _panel.Controls.Clear();
                // close ihUI
                _ihUI.Dispose();
                _ihUI = null;
            }
            // close image form if we are not already closing in response to use action. 
            // Set the _imageForm to null to raise a flag telling the form not to try to 
            // close the session again. since it usually calls CloseSession when it was closed by user.
            // not doing this will result in forever recursion!
            if (sd._imageForm != null)
            {
                ImagingForm form = sd._imageForm;
                sd._imageForm = null;
                form.Close();
            }
        }
        protected override void ActivateSession(SessionData sd)
        {
            //SessionData currentSD = ActiveSession == null ? null : ActiveSession.Data;

            // set panel state. 
            //////////////////////////////////////////////////////////////////
            _ihUI.Model = sd._model;
            _panel.Refresh();

            // activate the form showing the image!
            //////////////////////////////////////////////////////////////////
            if (!sd._imageForm.Activating)
            {
                sd._imageForm.Activating = true;   // so that form doesn't call activate again
                sd._imageForm.Show();
                sd._imageForm.BringToFront();
                sd._imageForm.Activate();
                sd._imageForm.Activating = false;
            }
        }

        protected override void OnSessionClosed(Guid hid, int sid)
        {
            base.OnSessionClosed(hid, sid);
        }
        protected override void OnSessionActivated(Guid hid, int sid)
        {
            base.OnSessionActivated(hid, sid);
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Image form events
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void f_Activated(object sender, EventArgs e)
        {
            ImagingForm f = (ImagingForm)sender;
            // we don't want to activate if we are allready activating in response to Najm request
            // to activate 
            if (!f.Activating)
            {
                f.Activating = true;
                // Ask Najm to activate for us. This is to let Najm take care of session management
                // for us. Otherwise we'll have to do a lot of non trivial book keeping.
                _NajmHandlersManager.ActivateSession(ID, f.SessionData.SessionID);
                f.Activating = false;
            }
        }
        void f_FormClosed(object sender, FormClosedEventArgs e)
        {
            ImagingForm f = (ImagingForm)sender;
            // prevent recurive call if we are closing due to use action not in response to Najm request.
            if (f.SessionData._imageForm != null)
            {
                // tell Close session that we are already closing. since it will try to close as it 
                // can be invoked by NahjmHandler.
                f.SessionData._imageForm = null;
                // Ask Najm to Close session for us. This is to let Najm take care of session management
                // for us. Otherwise we'll have to do a lot of non trivial book keeping.
                _NajmHandlersManager.CloseSession(ID, f.SessionData.SessionID);
            }
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private Panel _panel;
        private ImageHandlerUI _ihUI;
        private INajmHandlersManager _NajmHandlersManager;
        #endregion
    }
}
