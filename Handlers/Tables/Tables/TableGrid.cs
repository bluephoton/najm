using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Najm.FITSIO;

namespace Tables
{
    public partial class TableGrid : UserControl
    {
        public TableGrid(ITable table)
        {
            InitializeComponent();
            _table = table;

            _currentRowIndex = -1;

            // hook events
            dataGridView1.CellValueNeeded += new DataGridViewCellValueEventHandler(dataGridView1_CellValueNeeded);
            
            // Adjust the column widths based on the displayed values.
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            //dataGridView1.

            // cell as a template
            DataGridViewCell templateCell = new DataGridViewTextBoxCell();

            // create columns
            foreach (IFieldInfo fi in _table.Header)
            {
                DataGridViewColumn c = new DataGridViewColumn();
                c.HeaderText = fi.Name;
                c.CellTemplate = templateCell;
                c.SortMode = DataGridViewColumnSortMode.Automatic;
                dataGridView1.Columns.Add(c);
            }

            dataGridView1.RowCount = (int)_table.Length;

            // bind! note that datasource has at leat to be IList, this is why I'm using this adapter.
            //dataGridView1.DataSource = Enumerable.ToList<ITableRow>(_table);
        }

        void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (_currentRowIndex == -1 || _currentRowIndex != e.RowIndex)
            {
                _currentRowIndex = e.RowIndex;
                _currentRow = _table[e.RowIndex];
            }

            ITableCell cell = _currentRow[e.ColumnIndex];
            e.Value = cell.ToString();
            if (cell.Depth > 1)
            {
                //dataGridView1[e.ColumnIndex, e.RowIndex].Style.Font = new Font(dataGridView1.DefaultCellStyle.Font, FontStyle.Bold);
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Blue;
            }
        }

        private ITable _table;
        private long _currentRowIndex;
        private ITableRow _currentRow;
    }
}
