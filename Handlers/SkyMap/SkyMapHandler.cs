using System;
using System.Collections.Generic;
using System.Text;
using Najm.Handlers.Integration;
using Najm.FITSIO;

namespace Najm.SkyMap
{
    public class SessionData
    {
    }

    public class SkyMapHandler : NajmHandler<SessionData>
    {
        public override void Load(string arg) { }
        public override Guid ID { get { return new Guid("{F7FCB61A-E182-4942-974D-AC7D06F89388}"); } }
        public override string Name { get { return "Najm SkyMap Handler"; } }
        public override string ToolstripImageName { get { return "Najm.Handler.SkyMap.ToolstripImage.bmp"; } }
        public override string Tooltip { get { return "Invoke Najm SkyMap Handler"; } }

        public override bool CanHandle(IHDU[] hdus)
        {
            return true;
        }

        public override void Initialize(INajmHandlersManager nhm, System.Windows.Forms.Panel panel)
        {
            throw new NotImplementedException();
        }

        protected override SessionData OpenSession(int sid, IHDU[] hdus)
        {
            throw new NotImplementedException();
        }

        protected override void CloseSession(SessionData sd)
        {
            throw new NotImplementedException();
        }

        protected override void ActivateSession(SessionData sd)
        {
            throw new NotImplementedException();
        }
    }
}
