using System;
using System.Collections.Generic;
using System.Text;
using Najm.Handlers.Integration;
using Najm.FITSIO;
using System.Windows.Forms;

namespace Tables
{
    public class Session
    {
        internal Session(ITable table, TableGrid grid)
        {
            _table = table;
            _grid = grid;
        }
        public ITable _table;
        public TableGrid _grid;
    }

    public class TableHandler : NajmHandler<Session>
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region NajmHandler implementation
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Load(string arg) { /*Nothing to be done!*/}
        public override string Name { get { return "Najm FITS Table Handler"; } }
        public override Guid ID { get { return new Guid("{1E93F6DB-F2D2-4bda-A114-2B6F633859AC}"); } }
        public override string ToolstripImageName { get { return "Tables.ToolstriptImage.png"; } }
        public override string Tooltip { get { return "Invoke Najm Table Handler"; } }
        public override bool CanHandle(IHDU[] hdus)
        {
            return (hdus.Length == 1 && (hdus[0].Type == HDUType.ASCIITable || hdus[0].Type == HDUType.BinaryTable));
        }
        public override void Initialize(INajmHandlersManager nhm, Panel panel) { _panel = panel; }
        protected override Session OpenSession(int sid, IHDU[] hdus) { return new Session(hdus[0].Table, new TableGrid(hdus[0].Table)); }
        protected override void ActivateSession(Session sd)
        {
            _panel.Controls.Clear();
            _panel.Controls.Add(sd._grid);
            sd._grid.Dock = DockStyle.Fill;
        }
        protected override void CloseSession(Session sd) { /*We don't need to do anything*/}
        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        Panel _panel;
        #endregion
    }
}
