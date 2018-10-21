namespace Default
{
    partial class RichEditContainer
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.headerListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.keyColumn = new System.Windows.Forms.ColumnHeader();
            this.valueColumn = new System.Windows.Forms.ColumnHeader();
            this.commentColumn = new System.Windows.Forms.ColumnHeader();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.wcsRichTextBox = new System.Windows.Forms.RichTextBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.richTextBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.headerListView);
            this.splitContainer1.Size = new System.Drawing.Size(617, 463);
            this.splitContainer1.SplitterDistance = 122;
            this.splitContainer1.TabIndex = 2;
            // 
            // richTextBox
            // 
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox.Location = new System.Drawing.Point(0, 0);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ReadOnly = true;
            this.richTextBox.Size = new System.Drawing.Size(617, 122);
            this.richTextBox.TabIndex = 1;
            this.richTextBox.Text = "";
            // 
            // headerListView
            // 
            this.headerListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.headerListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerListView.FullRowSelect = true;
            this.headerListView.GridLines = true;
            this.headerListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.headerListView.HideSelection = false;
            this.headerListView.Location = new System.Drawing.Point(0, 0);
            this.headerListView.MultiSelect = false;
            this.headerListView.Name = "headerListView";
            this.headerListView.Size = new System.Drawing.Size(617, 337);
            this.headerListView.TabIndex = 0;
            this.headerListView.UseCompatibleStateImageBehavior = false;
            this.headerListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Key";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 160;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Comment";
            this.columnHeader3.Width = 800;
            // 
            // keyColumn
            // 
            this.keyColumn.DisplayIndex = 0;
            this.keyColumn.Text = "Key";
            this.keyColumn.Width = 80;
            // 
            // valueColumn
            // 
            this.valueColumn.DisplayIndex = 1;
            this.valueColumn.Text = "Value";
            this.valueColumn.Width = 200;
            // 
            // commentColumn
            // 
            this.commentColumn.DisplayIndex = 2;
            this.commentColumn.Text = "Comment";
            this.commentColumn.Width = 1000;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(631, 495);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(623, 469);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Details";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.wcsRichTextBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(623, 469);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "WCS";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // wcsRichTextBox
            // 
            this.wcsRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wcsRichTextBox.Location = new System.Drawing.Point(3, 3);
            this.wcsRichTextBox.Name = "wcsRichTextBox";
            this.wcsRichTextBox.ReadOnly = true;
            this.wcsRichTextBox.Size = new System.Drawing.Size(617, 463);
            this.wcsRichTextBox.TabIndex = 0;
            this.wcsRichTextBox.Text = "";
            this.wcsRichTextBox.WordWrap = false;
            // 
            // RichEditContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "RichEditContainer";
            this.Size = new System.Drawing.Size(631, 495);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.ListView headerListView;
        private System.Windows.Forms.ColumnHeader keyColumn;
        private System.Windows.Forms.ColumnHeader valueColumn;
        private System.Windows.Forms.ColumnHeader commentColumn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox wcsRichTextBox;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;

    }
}
