namespace Najm.UI
{
    partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.settingsTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.handlersDataGridView = new System.Windows.Forms.DataGridView();
            this.okBbutton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.addHandlerButton = new System.Windows.Forms.Button();
            this.removeHandlerButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.removeAllHandlersButton = new System.Windows.Forms.Button();
            this.configsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Assembly = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HandlerLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Param = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HandlerEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.settingsTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.handlersDataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // settingsTabControl
            // 
            this.settingsTabControl.Controls.Add(this.tabPage1);
            this.settingsTabControl.Location = new System.Drawing.Point(10, 19);
            this.settingsTabControl.Name = "settingsTabControl";
            this.settingsTabControl.SelectedIndex = 0;
            this.settingsTabControl.Size = new System.Drawing.Size(543, 245);
            this.settingsTabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.handlersDataGridView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(535, 219);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Handlers";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // handlersDataGridView
            // 
            this.handlersDataGridView.AllowUserToAddRows = false;
            this.handlersDataGridView.AllowUserToResizeRows = false;
            this.handlersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.handlersDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Assembly,
            this.HandlerLocation,
            this.Param,
            this.HandlerEnabled});
            this.handlersDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.handlersDataGridView.Location = new System.Drawing.Point(3, 3);
            this.handlersDataGridView.Name = "handlersDataGridView";
            this.handlersDataGridView.Size = new System.Drawing.Size(529, 213);
            this.handlersDataGridView.TabIndex = 0;
            this.handlersDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.handlersDataGridView_CellValueChanged);
            // 
            // okBbutton
            // 
            this.okBbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBbutton.Location = new System.Drawing.Point(491, 307);
            this.okBbutton.Name = "okBbutton";
            this.okBbutton.Size = new System.Drawing.Size(75, 23);
            this.okBbutton.TabIndex = 1;
            this.okBbutton.Text = "OK";
            this.okBbutton.UseVisualStyleBackColor = true;
            this.okBbutton.Click += new System.EventHandler(this.okBbutton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(410, 307);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // addHandlerButton
            // 
            this.addHandlerButton.Location = new System.Drawing.Point(10, 268);
            this.addHandlerButton.Name = "addHandlerButton";
            this.addHandlerButton.Size = new System.Drawing.Size(75, 23);
            this.addHandlerButton.TabIndex = 3;
            this.addHandlerButton.Text = "Add";
            this.addHandlerButton.UseVisualStyleBackColor = true;
            this.addHandlerButton.Click += new System.EventHandler(this.addHandlerButton_Click);
            // 
            // removeHandlerButton
            // 
            this.removeHandlerButton.Location = new System.Drawing.Point(91, 268);
            this.removeHandlerButton.Name = "removeHandlerButton";
            this.removeHandlerButton.Size = new System.Drawing.Size(75, 23);
            this.removeHandlerButton.TabIndex = 4;
            this.removeHandlerButton.Text = "Remove";
            this.removeHandlerButton.UseVisualStyleBackColor = true;
            this.removeHandlerButton.Click += new System.EventHandler(this.removeHandlerButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.removeAllHandlersButton);
            this.groupBox1.Controls.Add(this.removeHandlerButton);
            this.groupBox1.Controls.Add(this.settingsTabControl);
            this.groupBox1.Controls.Add(this.addHandlerButton);
            this.groupBox1.Location = new System.Drawing.Point(5, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(561, 301);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // removeAllHandlersButton
            // 
            this.removeAllHandlersButton.Location = new System.Drawing.Point(172, 268);
            this.removeAllHandlersButton.Name = "removeAllHandlersButton";
            this.removeAllHandlersButton.Size = new System.Drawing.Size(75, 23);
            this.removeAllHandlersButton.TabIndex = 5;
            this.removeAllHandlersButton.Text = "Remove All";
            this.removeAllHandlersButton.UseVisualStyleBackColor = true;
            this.removeAllHandlersButton.Click += new System.EventHandler(this.removeAllHandlersButton_Click);
            // 
            // configsBindingSource
            // 
            this.configsBindingSource.DataSource = typeof(Najm.Configs);
            // 
            // Id
            // 
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            // 
            // Assembly
            // 
            this.Assembly.HeaderText = "Assembly";
            this.Assembly.Name = "Assembly";
            this.Assembly.ReadOnly = true;
            this.Assembly.Width = 150;
            // 
            // HandlerLocation
            // 
            this.HandlerLocation.HeaderText = "Location";
            this.HandlerLocation.Name = "HandlerLocation";
            this.HandlerLocation.ReadOnly = true;
            this.HandlerLocation.Width = 120;
            // 
            // Param
            // 
            this.Param.HeaderText = "Param";
            this.Param.Name = "Param";
            this.Param.ReadOnly = true;
            // 
            // HandlerEnabled
            // 
            this.HandlerEnabled.HeaderText = "Enabled";
            this.HandlerEnabled.Name = "HandlerEnabled";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 335);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okBbutton);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.Text = "Najm Settings";
            this.settingsTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.handlersDataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.configsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl settingsTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.BindingSource configsBindingSource;
        private System.Windows.Forms.Button okBbutton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button addHandlerButton;
        private System.Windows.Forms.Button removeHandlerButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button removeAllHandlersButton;
        private System.Windows.Forms.DataGridView handlersDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Assembly;
        private System.Windows.Forms.DataGridViewTextBoxColumn HandlerLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn Param;
        private System.Windows.Forms.DataGridViewCheckBoxColumn HandlerEnabled;
     }
}