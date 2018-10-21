using System;
using System.Collections.Generic;
using System.Text;
using Najm.Handlers.Integration;
using Najm.FITSIO;
using System.Windows.Forms;

namespace Default
{
    public class DefaultHandler : NajmHandler<IHDU>
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region INajmHDUHandler Members
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Load(string arg) { /*Nothing to be done!*/}
        public override Guid ID { get { return new Guid("{37581060-646F-49aa-8D1E-1E255B81B471}"); } }
        public override string Name { get { return "Najm Default Handler"; } }
        public override bool CanHandle(IHDU[] hdus) { return hdus.Length > 0; }
        public override void Initialize(INajmHandlersManager nhm, Panel panel)
        {
            _richEditContainer = new RichEditContainer();
            panel.Controls.Add(_richEditContainer);
            _richEditContainer.Dock = DockStyle.Fill;
        }
        public override string ToolstripImageName { get { return ""; } }
        public override string Tooltip { get { return ""; } }
        protected override IHDU OpenSession(int sid, IHDU[] hdus)
        {
            /*IHDU hdu = null;
            if (hdus.Length > 0 && hdus[0] != null)
            {
                hdu = hdus[0];
                _richEditContainer.DisplayHDUHeader(hdu);
            }*/
            return hdus[0];
        }
        protected override void CloseSession(IHDU sd) { /*No need to do anything!*/ }
        protected override void ActivateSession(IHDU hdu)
        {
            _richEditContainer.DisplayHDUHeader(hdu);
        }
        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private RichEditContainer _richEditContainer;
        #endregion
    }
}
