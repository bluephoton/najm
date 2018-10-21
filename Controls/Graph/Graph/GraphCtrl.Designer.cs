namespace Najm.Controls
{
    partial class GraphCtrl
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
            this.SuspendLayout();
            // 
            // GraphCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "GraphCtrl";
            this.Size = new System.Drawing.Size(463, 212);
            this.Load += new System.EventHandler(this.GraphCtrl_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GraphCtrl_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GraphCtrl_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GraphCtrl_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
