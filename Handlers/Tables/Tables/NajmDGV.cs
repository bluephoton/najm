using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Tables
{
    class NajmDGV : DataGridView
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // this function is from: Daniel S. Soper (http://www.danielsoper.com/programming/DataGridViewNumberedRows.aspx)
        // TODO: too expensive
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
        {
            //store a string representation of the row number in 'strRowNumber'
            string rowNumber = (e.RowIndex + 1).ToString();

            //prepend leading zeros to the string if necessary to improve
            //appearance. For example, if there are ten rows in the grid,
            //row seven will be numbered as "07" instead of "7". Similarly, if 
            //there are 100 rows in the grid, row seven will be numbered as "007".
            while (rowNumber.Length < this.RowCount.ToString().Length) rowNumber = "0" + rowNumber;

            //determine the display size of the row number string using
            //the DataGridView's current font.
            SizeF size = e.Graphics.MeasureString(rowNumber, this.Font);

            //adjust the width of the column that contains the row header cells 
            //if necessary
            if (this.RowHeadersWidth < (int)(size.Width + 20)) this.RowHeadersWidth = (int)(size.Width + 20);

            //this brush will be used to draw the row number string on the
            //row header cell using the system's current ControlText color
            Brush b = SystemBrushes.ControlText;

            //draw the row number string on the current row header cell using
            //the brush defined above and the DataGridView's default font
            e.Graphics.DrawString(rowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));

            //base.OnRowPostPaint(e);
        }
    }
}
