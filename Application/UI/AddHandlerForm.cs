using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Najm.Config;
using Najm.Handlers;

namespace Najm.UI
{
    public partial class AddHandlerForm : Form
    {
        public AddHandlerForm()
        {
            InitializeComponent();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                handlerModuleTextBox.Text = ofd.FileName;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // init things
            _hi = null;
            Guid id = Guid.Empty;
            string location = Path.GetDirectoryName(handlerModuleTextBox.Text);
            string module = Path.GetFileName(handlerModuleTextBox.Text);
            string paramts = handlerParametersTextBox.Text;
            // special handling for python modules.
            if (0 == string.Compare(Path.GetExtension(handlerModuleTextBox.Text), ".py", true))
            {
                location = "";
                module = "Najm.Handler.IPython.dll";
                paramts = handlerModuleTextBox.Text;
            }

            // try to load handler assembly and get its id
            try
            {
                HandlerAssembly ha = new HandlerAssembly();
                ha.Load(location, module, paramts);
                id = ha.Handler.ID;
                ha.Unload();
            }
            catch (Exception ex)
            {
                string msg = ex.Message + ((ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message)) ? "" : "\n" + ex.InnerException.Message);
                MessageBox.Show(msg);
            }
            finally
            {
                if (!id.Equals(Guid.Empty))
                {
                    // create handler info
                    _hi = new HandlerInfo(id, location, module, paramts, true);
                }
            }
        }

        internal HandlerInfo HandlerInfo { get { return _hi; } }

        private HandlerInfo _hi;
    }
}
