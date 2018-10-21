using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Najm.Config;

namespace Najm.UI
{
    public partial class Settings : Form
    {
        internal Settings()
        {
            InitializeComponent();
            foreach (HandlerInfo hi in NajmConfigs.Handlers)
            {
                string location = string.IsNullOrEmpty(hi.Location) ? "<Default>" : hi.Location;
                object[] columns = new object[]{hi.Id, hi.Assembly, location, hi.Param, hi.Enabled};
                handlersDataGridView.Rows.Add(columns);
            }
        }

        private void addHandlerButton_Click(object sender, EventArgs e)
        {
            AddHandlerForm ahf = new AddHandlerForm();
            ahf.ShowDialog();
            if(ahf.HandlerInfo != null)
            {
                string location = string.IsNullOrEmpty(ahf.HandlerInfo.Location) ? "<Default>" : ahf.HandlerInfo.Location;
                object[] columns = new object[] { ahf.HandlerInfo.Id, ahf.HandlerInfo.Assembly, location, ahf.HandlerInfo.Param, ahf.HandlerInfo.Enabled };
                handlersDataGridView.Rows.Add(columns);
                NajmConfigs.AddHandler(ahf.HandlerInfo);
            }
        }

        private void removeHandlerButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in handlersDataGridView.SelectedRows)
            {
                handlersDataGridView.Rows.RemoveAt(r.Index);
                NajmConfigs.RemoveHandler((Guid)r.Cells[0].Value);
            }
        }

        private void removeAllHandlersButton_Click(object sender, EventArgs e)
        {
            handlersDataGridView.Rows.Clear();
            NajmConfigs.RemoveAll();
        }

        private void okBbutton_Click(object sender, EventArgs e)
        {
            
        }

        private void handlersDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex > 0)  // enabled
            {
                Guid id = (Guid)handlersDataGridView.Rows[e.RowIndex].Cells[0].Value;
                bool enabled = (bool)handlersDataGridView.Rows[e.RowIndex].Cells[4].Value;
                NajmConfigs.EnableHandler(id, enabled);
            }
        }
    }
}
