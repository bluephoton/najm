namespace ImageHandler
{
    partial class ImagingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImagingForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.imageCubeToolStrip = new System.Windows.Forms.ToolStrip();
            this.firstSliceToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.playBackwardToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.playToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.lastSliceToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.imagingToolStrip = new System.Windows.Forms.ToolStrip();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.actualSizeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.CopyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.playTimer = new System.Windows.Forms.Timer(this.components);
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.imageCubeToolStrip.SuspendLayout();
            this.imagingToolStrip.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.imageCubeToolStrip);
            this.splitContainer1.Panel1.Controls.Add(this.imagingToolStrip);
            this.splitContainer1.Size = new System.Drawing.Size(634, 398);
            this.splitContainer1.SplitterDistance = 29;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 2;
            // 
            // imageCubeToolStrip
            // 
            this.imageCubeToolStrip.CanOverflow = false;
            this.imageCubeToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.imageCubeToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.imageCubeToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.imageCubeToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.firstSliceToolStripButton,
            this.playBackwardToolStripButton,
            this.playToolStripButton,
            this.lastSliceToolStripButton,
            this.toolStripSeparator1});
            this.imageCubeToolStrip.Location = new System.Drawing.Point(184, 2);
            this.imageCubeToolStrip.Name = "imageCubeToolStrip";
            this.imageCubeToolStrip.Size = new System.Drawing.Size(105, 27);
            this.imageCubeToolStrip.TabIndex = 2;
            this.imageCubeToolStrip.Visible = false;
            // 
            // firstSliceToolStripButton
            // 
            this.firstSliceToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.firstSliceToolStripButton.Image = global::Imaging.Properties.Resources.FirstSlice;
            this.firstSliceToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.firstSliceToolStripButton.Name = "firstSliceToolStripButton";
            this.firstSliceToolStripButton.Size = new System.Drawing.Size(24, 24);
            this.firstSliceToolStripButton.ToolTipText = "First Image";
            this.firstSliceToolStripButton.Click += new System.EventHandler(this.firstSliceToolStripButton_Click);
            // 
            // playBackwardToolStripButton
            // 
            this.playBackwardToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.playBackwardToolStripButton.Image = global::Imaging.Properties.Resources.PlayBackward;
            this.playBackwardToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.playBackwardToolStripButton.Name = "playBackwardToolStripButton";
            this.playBackwardToolStripButton.Size = new System.Drawing.Size(24, 24);
            this.playBackwardToolStripButton.ToolTipText = "Play backward";
            this.playBackwardToolStripButton.Click += new System.EventHandler(this.playBackwardToolStripButton_Click);
            // 
            // playToolStripButton
            // 
            this.playToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.playToolStripButton.Image = global::Imaging.Properties.Resources.Play;
            this.playToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.playToolStripButton.Name = "playToolStripButton";
            this.playToolStripButton.Size = new System.Drawing.Size(24, 24);
            this.playToolStripButton.Text = "toolStripButton2";
            this.playToolStripButton.ToolTipText = "Play";
            this.playToolStripButton.Click += new System.EventHandler(this.playToolStripButton_Click);
            // 
            // lastSliceToolStripButton
            // 
            this.lastSliceToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.lastSliceToolStripButton.Image = global::Imaging.Properties.Resources.LastImage;
            this.lastSliceToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lastSliceToolStripButton.Name = "lastSliceToolStripButton";
            this.lastSliceToolStripButton.Size = new System.Drawing.Size(24, 24);
            this.lastSliceToolStripButton.ToolTipText = "Last Image";
            this.lastSliceToolStripButton.Click += new System.EventHandler(this.lastSliceToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // imagingToolStrip
            // 
            this.imagingToolStrip.CanOverflow = false;
            this.imagingToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.imagingToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.imagingToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.imagingToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripButton,
            this.actualSizeToolStripButton,
            this.CopyToolStripButton});
            this.imagingToolStrip.Location = new System.Drawing.Point(0, 0);
            this.imagingToolStrip.Name = "imagingToolStrip";
            this.imagingToolStrip.Size = new System.Drawing.Size(129, 27);
            this.imagingToolStrip.TabIndex = 1;
            this.imagingToolStrip.Text = "toolStrip1";
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(35, 24);
            this.saveToolStripButton.Text = "Save";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // actualSizeToolStripButton
            // 
            this.actualSizeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.actualSizeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.actualSizeToolStripButton.Name = "actualSizeToolStripButton";
            this.actualSizeToolStripButton.Size = new System.Drawing.Size(67, 24);
            this.actualSizeToolStripButton.Text = "Actual size";
            this.actualSizeToolStripButton.Click += new System.EventHandler(this.actualSizeToolStripButton_Click);
            // 
            // CopyToolStripButton
            // 
            this.CopyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CopyToolStripButton.Image = global::Imaging.Properties.Resources.CopyButton;
            this.CopyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CopyToolStripButton.Name = "CopyToolStripButton";
            this.CopyToolStripButton.Size = new System.Drawing.Size(24, 24);
            this.CopyToolStripButton.ToolTipText = "Copy to clipboard";
            this.CopyToolStripButton.Click += new System.EventHandler(this.CopyToolStripButton_Click);
            // 
            // playTimer
            // 
            this.playTimer.Interval = 250;
            this.playTimer.Tick += new System.EventHandler(this.playTimer_Tick);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(634, 373);
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(634, 398);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(634, 398);
            this.toolStripContainer1.TabIndex = 3;
            this.toolStripContainer1.Text = "toolStripContainer1";
            this.toolStripContainer1.TopToolStripPanelVisible = false;
            // 
            // ImagingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 398);
            this.Controls.Add(this.toolStripContainer1);
            this.KeyPreview = true;
            this.Name = "ImagingForm";
            this.Text = "ImagingForm";
            this.Load += new System.EventHandler(this.ImagingForm_Load);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ImagingForm_MouseWheel);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ImagingForm_FormClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.imageCubeToolStrip.ResumeLayout(false);
            this.imageCubeToolStrip.PerformLayout();
            this.imagingToolStrip.ResumeLayout(false);
            this.imagingToolStrip.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Timer playTimer;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.ToolStrip imagingToolStrip;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripButton actualSizeToolStripButton;
        private System.Windows.Forms.ToolStripButton CopyToolStripButton;
        private System.Windows.Forms.ToolStrip imageCubeToolStrip;
        private System.Windows.Forms.ToolStripButton firstSliceToolStripButton;
        private System.Windows.Forms.ToolStripButton playBackwardToolStripButton;
        private System.Windows.Forms.ToolStripButton playToolStripButton;
        private System.Windows.Forms.ToolStripButton lastSliceToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
    }
}