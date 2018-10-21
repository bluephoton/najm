namespace Najm.UI
{
    partial class MainAppForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainAppForm));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.fileOpenButton = new System.Windows.Forms.ToolStripButton();
            this.settingsButton = new System.Windows.Forms.ToolStripButton();
            this.aboutButton = new System.Windows.Forms.ToolStripButton();
            this.splitContainerVertical = new System.Windows.Forms.SplitContainer();
            this.structureTreeView = new System.Windows.Forms.TreeView();
            this.splitContainerHorizontal = new System.Windows.Forms.SplitContainer();
            this.handlersTabControl = new System.Windows.Forms.TabControl();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.notYetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sorryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.hduContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveBinaryDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileRootContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fileCloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.handlersToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).BeginInit();
            this.splitContainerVertical.Panel1.SuspendLayout();
            this.splitContainerVertical.Panel2.SuspendLayout();
            this.splitContainerVertical.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHorizontal)).BeginInit();
            this.splitContainerHorizontal.Panel1.SuspendLayout();
            this.splitContainerHorizontal.Panel2.SuspendLayout();
            this.splitContainerHorizontal.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.hduContextMenuStrip.SuspendLayout();
            this.fileRootContextMenuStrip.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileOpenButton,
            this.settingsButton,
            this.aboutButton});
            this.toolStrip.Location = new System.Drawing.Point(3, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(72, 25);
            this.toolStrip.TabIndex = 0;
            // 
            // fileOpenButton
            // 
            this.fileOpenButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.fileOpenButton.Image = ((System.Drawing.Image)(resources.GetObject("fileOpenButton.Image")));
            this.fileOpenButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileOpenButton.Name = "fileOpenButton";
            this.fileOpenButton.Size = new System.Drawing.Size(23, 22);
            this.fileOpenButton.Text = "&Open";
            this.fileOpenButton.Click += new System.EventHandler(this.fileOpenButton_Click);
            // 
            // settingsButton
            // 
            this.settingsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.settingsButton.Image = ((System.Drawing.Image)(resources.GetObject("settingsButton.Image")));
            this.settingsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(23, 22);
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // aboutButton
            // 
            this.aboutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.aboutButton.Image = ((System.Drawing.Image)(resources.GetObject("aboutButton.Image")));
            this.aboutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(23, 22);
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // splitContainerVertical
            // 
            this.splitContainerVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerVertical.Location = new System.Drawing.Point(0, 0);
            this.splitContainerVertical.Name = "splitContainerVertical";
            // 
            // splitContainerVertical.Panel1
            // 
            this.splitContainerVertical.Panel1.Controls.Add(this.structureTreeView);
            // 
            // splitContainerVertical.Panel2
            // 
            this.splitContainerVertical.Panel2.Controls.Add(this.splitContainerHorizontal);
            this.splitContainerVertical.Size = new System.Drawing.Size(762, 821);
            this.splitContainerVertical.SplitterDistance = 143;
            this.splitContainerVertical.TabIndex = 1;
            // 
            // structureTreeView
            // 
            this.structureTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.structureTreeView.HideSelection = false;
            this.structureTreeView.Location = new System.Drawing.Point(0, 0);
            this.structureTreeView.Name = "structureTreeView";
            this.structureTreeView.Size = new System.Drawing.Size(143, 821);
            this.structureTreeView.TabIndex = 0;
            this.structureTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.structureTreeView_AfterSelect);
            this.structureTreeView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.structureTreeView_MouseClick);
            // 
            // splitContainerHorizontal
            // 
            this.splitContainerHorizontal.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainerHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerHorizontal.Location = new System.Drawing.Point(0, 0);
            this.splitContainerHorizontal.Name = "splitContainerHorizontal";
            this.splitContainerHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerHorizontal.Panel1
            // 
            this.splitContainerHorizontal.Panel1.Controls.Add(this.handlersTabControl);
            // 
            // splitContainerHorizontal.Panel2
            // 
            this.splitContainerHorizontal.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainerHorizontal.Panel2.Controls.Add(this.statusTextBox);
            this.splitContainerHorizontal.Size = new System.Drawing.Size(615, 821);
            this.splitContainerHorizontal.SplitterDistance = 769;
            this.splitContainerHorizontal.TabIndex = 0;
            // 
            // handlersTabControl
            // 
            this.handlersTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.handlersTabControl.Location = new System.Drawing.Point(0, 0);
            this.handlersTabControl.Name = "handlersTabControl";
            this.handlersTabControl.SelectedIndex = 0;
            this.handlersTabControl.Size = new System.Drawing.Size(615, 769);
            this.handlersTabControl.TabIndex = 1;
            // 
            // statusTextBox
            // 
            this.statusTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusTextBox.Location = new System.Drawing.Point(0, 0);
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ReadOnly = true;
            this.statusTextBox.Size = new System.Drawing.Size(615, 20);
            this.statusTextBox.TabIndex = 0;
            // 
            // notYetToolStripMenuItem
            // 
            this.notYetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.closeAllFilesToolStripMenuItem,
            this.exitToolStripMenuItem1});
            this.notYetToolStripMenuItem.Name = "notYetToolStripMenuItem";
            this.notYetToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.notYetToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // closeAllFilesToolStripMenuItem
            // 
            this.closeAllFilesToolStripMenuItem.Name = "closeAllFilesToolStripMenuItem";
            this.closeAllFilesToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.closeAllFilesToolStripMenuItem.Text = "Close &All";
            this.closeAllFilesToolStripMenuItem.Click += new System.EventHandler(this.closeAllFilesToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(120, 22);
            this.exitToolStripMenuItem1.Text = "E&xit";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem1_Click);
            // 
            // sorryToolStripMenuItem
            // 
            this.sorryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configureToolStripMenuItem,
            this.closeToolStripMenuItem1});
            this.sorryToolStripMenuItem.Name = "sorryToolStripMenuItem";
            this.sorryToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.sorryToolStripMenuItem.Text = "Ha&ndlers";
            // 
            // configureToolStripMenuItem
            // 
            this.configureToolStripMenuItem.Name = "configureToolStripMenuItem";
            this.configureToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.configureToolStripMenuItem.Text = "Con&figure";
            this.configureToolStripMenuItem.Click += new System.EventHandler(this.configureToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem1
            // 
            this.closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
            this.closeToolStripMenuItem1.Size = new System.Drawing.Size(127, 22);
            this.closeToolStripMenuItem1.Text = "&Close";
            this.closeToolStripMenuItem1.Click += new System.EventHandler(this.closeToolStripMenuItem1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.notYetToolStripMenuItem,
            this.sorryToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(762, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualToolStripMenuItem,
            this.rToolStripMenuItem,
            this.aboutToolStripMenuItem1});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // manualToolStripMenuItem
            // 
            this.manualToolStripMenuItem.Name = "manualToolStripMenuItem";
            this.manualToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.manualToolStripMenuItem.Text = "&Users Manual";
            this.manualToolStripMenuItem.Click += new System.EventHandler(this.manualToolStripMenuItem_Click);
            // 
            // rToolStripMenuItem
            // 
            this.rToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.rToolStripMenuItem.ForeColor = System.Drawing.Color.Red;
            this.rToolStripMenuItem.Name = "rToolStripMenuItem";
            this.rToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.rToolStripMenuItem.Text = "Report a &Bug!";
            this.rToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
            this.aboutToolStripMenuItem1.Text = "&About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // hduContextMenuStrip
            // 
            this.hduContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveBinaryDataToolStripMenuItem});
            this.hduContextMenuStrip.Name = "hduContextMenuStrip";
            this.hduContextMenuStrip.Size = new System.Drawing.Size(161, 26);
            // 
            // saveBinaryDataToolStripMenuItem
            // 
            this.saveBinaryDataToolStripMenuItem.Name = "saveBinaryDataToolStripMenuItem";
            this.saveBinaryDataToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.saveBinaryDataToolStripMenuItem.Text = "Save binary data";
            this.saveBinaryDataToolStripMenuItem.Click += new System.EventHandler(this.saveBinaryDataToolStripMenuItem_Click);
            // 
            // fileRootContextMenuStrip
            // 
            this.fileRootContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileCloseToolStripMenuItem});
            this.fileRootContextMenuStrip.Name = "fileRootContextMenuStrip";
            this.fileRootContextMenuStrip.Size = new System.Drawing.Size(104, 26);
            // 
            // fileCloseToolStripMenuItem
            // 
            this.fileCloseToolStripMenuItem.Name = "fileCloseToolStripMenuItem";
            this.fileCloseToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.fileCloseToolStripMenuItem.Text = "Close";
            this.fileCloseToolStripMenuItem.Click += new System.EventHandler(this.fileCloseToolStripMenuItem_Click);
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainerVertical);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(762, 821);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(762, 870);
            this.toolStripContainer1.TabIndex = 3;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripContainer1.TopToolStripPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.handlersToolStrip);
            // 
            // handlersToolStrip
            // 
            this.handlersToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.handlersToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.handlersToolStrip.Location = new System.Drawing.Point(75, 24);
            this.handlersToolStrip.Name = "handlersToolStrip";
            this.handlersToolStrip.Size = new System.Drawing.Size(102, 25);
            this.handlersToolStrip.TabIndex = 3;
            // 
            // MainAppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 870);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainAppForm";
            this.Text = "Najm - The Universe is Under Construction!";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainAppForm_FormClosed);
            this.Load += new System.EventHandler(this.MainAppForm_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.splitContainerVertical.Panel1.ResumeLayout(false);
            this.splitContainerVertical.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).EndInit();
            this.splitContainerVertical.ResumeLayout(false);
            this.splitContainerHorizontal.Panel1.ResumeLayout(false);
            this.splitContainerHorizontal.Panel2.ResumeLayout(false);
            this.splitContainerHorizontal.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHorizontal)).EndInit();
            this.splitContainerHorizontal.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.hduContextMenuStrip.ResumeLayout(false);
            this.fileRootContextMenuStrip.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.SplitContainer splitContainerVertical;
        private System.Windows.Forms.ToolStripButton fileOpenButton;
        private System.Windows.Forms.TreeView structureTreeView;
        private System.Windows.Forms.ToolStripButton settingsButton;
        private System.Windows.Forms.ToolStripButton aboutButton;
        private System.Windows.Forms.ToolStripMenuItem notYetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sorryToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.SplitContainer splitContainerHorizontal;
        private System.Windows.Forms.TabControl handlersTabControl;
        private System.Windows.Forms.ContextMenuStrip hduContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem saveBinaryDataToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip fileRootContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileCloseToolStripMenuItem;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip handlersToolStrip;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.TextBox statusTextBox;

    }
}