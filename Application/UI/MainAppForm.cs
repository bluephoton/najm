using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Najm.FITSIO;
using Najm.Handlers;
using Najm.Handlers.Integration;
using Najm.Config;

namespace Najm.UI
{
    public partial class MainAppForm : Form
    {
        public MainAppForm()
        {
            InitializeComponent();
        }

        private void MainAppForm_Load(object sender, EventArgs e)
        {
            _curTreeNode = null;

            // TODO: i pass this here now to do the handlers related stuff outside this file keeping it small
            //       perhaps a better way is to create another partial class that handles HandlersManager event
            //       and do these tasks in it...later!
            //       Also it seems that we need a Handler class to wrap the even growing handler things:
            //       like: image, button, events,...
            _handlersManager = new HandlersManager(this);
            
            // create imagelist in the tab control to be used when tab pages are added for handlers
            handlersTabControl.ImageList = new ImageList();

#if DEBUG
            Left += 1920 + 200;
#endif
            
            NajmConfigs.Load();

            _handlersManager.LoadHandlers(NajmConfigs.Handlers);
        }

        // HandlersManager will use these
        // TODO: Not good idea! Pass an interface to the handler manager instead. I then call this interface
        // too do what it wants with the Tree, Tab, and toolstrip
        internal TreeView Tree {get {return structureTreeView;}}
        internal TabControl TabControl {get {return handlersTabControl;}}
        internal ToolStrip ToolStrip { get { return handlersToolStrip; } }

        private void fileOpenButton_Click(object sender, EventArgs e)
        {
            InvokeFileOpenDialog();
        }

        private void InvokeFileOpenDialog()
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Title = "Select data file";
            fd.Filter = "FITS File (*.fits;*.fit)|*.fits;*fit|All Files(*.*)|*.*";
            fd.Multiselect = true;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in fd.FileNames)
                {
                    LoadFile(file);
                }
            }
        }

        private void LoadFile(string filePath)
        {
            // load the file
            IFITSFile ff = Factory.CreateFITSFile();
            StartWaitCursor();
            try
            {
                // load file
                _curTreeNode = null;
                ff.Load(filePath);
                // populate tree
                _curTreeNode = structureTreeView.Nodes.Add(new FileInfo(filePath).Name);
                _curTreeNode.Tag = ff;
                _curTreeNode.ToolTipText = filePath;
                for (int i = 0; i < ff.HDUs.Length; i++)
                {
                    TreeNode n = _curTreeNode.Nodes.Add("HDU - " + ff.HDUs[i].Name);
                    n.Tag = new KeyValuePair<IHDU, int>(ff.HDUs[i], -1);
                }
                _curTreeNode.Expand();
            }
#if !DEBUG
            catch (Najm.FITSIO.Exception ex)
            {
                RemoveCurrentFileNode();
                MessageBox.Show(ex.Message);
            }
            catch(System.Exception ex)
            {
                RemoveCurrentFileNode();
                MessageBox.Show(string.Format("An error occurred while opening this file: {0}", ex.Message));
            }
#endif
            finally
            {
                EndWaitCursor();
            }
        }

        private void RemoveCurrentFileNode()
        {
            TreeNode n = GetCurrentFileNode();
            if (n != null)
            {
                structureTreeView.Nodes.Remove(n);
                _curTreeNode = null;
            }
        }

        private TreeNode GetCurrentFileNode()
        {
            TreeNode n = _curTreeNode;
            if (n != null)
            {
                while (n.Parent != null)
                {
                    n = n.Parent;
                }
            }
            return n;
        }

        private void StartWaitCursor()
        {
            _oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
        }

        private void EndWaitCursor()
        {
            Cursor.Current = _oldCursor;
        }

        private IHDU GetSelectedHDU()
        {
            IHDU hdu = null;
            TreeNode n = (structureTreeView).SelectedNode;
            if (n != null && n.Tag != null && n.Tag is IHDU)
            {
                hdu = ((IHDU)n.Tag);
            }
            return hdu;
        }

        internal void HandlerButton_Click(object sender, EventArgs e)
        {
            int session = -1;
            Guid guidHandler = Guid.Empty;
#if !DEBUG
            try
            {
#endif
                StartWaitCursor();
                ToolStripButton b = (ToolStripButton) sender;
                IHDU hdu = GetSelectedHDU();
                if (hdu != null)
                {
                    IHDU[] hdus = new IHDU[1] { hdu };
                    guidHandler = (Guid)b.Tag;
                    session = _handlersManager.OpenSession(guidHandler, hdus);
                    _handlersManager.ActivateSession(guidHandler, session);
                }
#if !DEBUG
            }
#endif
#if !DEBUG
            catch (System.Exception ex)
            {
                // close session on exception to avoid any leftover UI elemts...etc
                if (session != -1 && guidHandler != Guid.Empty)
                {
                    _handlersManager.CloseSession(guidHandler, session);
                }
                MessageBox.Show(ex.Message);
            }
            finally
            {
#endif
                EndWaitCursor();
#if !DEBUG
            }
#endif
        }

        internal void HandlerButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Guid hid = (Guid)((ToolStripButton)sender).Tag;
                INajmHandler nh = _handlersManager[hid];
                if (MessageBox.Show(string.Format("Reload \'{0}\' ?", nh.Name), "Najm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {                    
                    _handlersManager.ReloadHandler(hid);
                }
            }
        }

        private void saveBinaryDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_hduAtMouse != null)
            {
                string filename = _hduAtMouse.File.Name;
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = new FileInfo(string.IsNullOrEmpty(filename) ? "" : filename).Name + ".dat";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(_hduAtMouse.Axes.Length);
                    for (int i = 0; i < _hduAtMouse.Axes.Length; i++)
                    {
                        bw.Write(_hduAtMouse.Axes[i].NumPoints);
                    }
                    bw.Write(_hduAtMouse.DataMngr.BitsPerPixel);
                    bw.Write(_hduAtMouse.DataMngr.RawData.Length); // for convenience
                    Debug.Assert(false);    //fix line below
                    //bw.Write(_hduAtMouse.Data, 0, (int)_hduAtMouse.DataSize);
                    bw.Close();
                }
                _hduAtMouse = null;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region tree events
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void structureTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode n = ((TreeView)sender).SelectedNode;
            if (n != null && n.Tag != null)
            {
#if !DEBUG
                try
#endif
                {
                    ActivateTabOrSession(n);
                }
#if !DEBUG
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
#endif
            }
        }

        private void structureTreeView_MouseClick(object sender, MouseEventArgs e)
        {
            TreeNode n = structureTreeView.GetNodeAt(e.Location);
            if (n != null && n.Tag != null)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        {
                            // I do this only if the item is selected. this is because when Item is selected i don't get after select
                            // event and tab will not get activated.
                            // if its not selected we'll get the after select event which will take care of this for us
                            if (n.IsSelected)
                            {
                                try
                                {
                                    ActivateTabOrSession(n);
                                }
                                catch (System.Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        }
                        break;
                    case MouseButtons.Right:
                        // show context menu
                        if (n.IsSelected)
                        {
                            if (n.Tag is IHDU)
                            {
                                _hduAtMouse = (IHDU)n.Tag;
                                hduContextMenuStrip.Show(structureTreeView.PointToScreen(e.Location));
                            }
                            else if (n.Tag is IFITSFile || (n.Tag is KeyValuePair<Guid, int> && !((KeyValuePair<Guid, int>)n.Tag).Key.Equals(_handlersManager.DefaultHandler.ID)))
                            {
                                fileRootContextMenuStrip.Show(structureTreeView.PointToScreen(e.Location));
                            }
                        }
                        break;
                }
            }
        }


        private void PassToDefaultHandler(KeyValuePair<IHDU, int> pair)
        {
            IHDU hdu = pair.Key;
            int sessionID = pair.Value;
            if (sessionID == -1) // we don't have opened session
            {
                IHDU[] hdus = new IHDU[1] { hdu };
                sessionID = _handlersManager.OpenSession(_handlersManager.DefaultHandler.ID, hdus);
            }
            _handlersManager.ActivateSession(_handlersManager.DefaultHandler.ID, sessionID);
        }

        private void ActivateTabOrSession(TreeNode n)
        {
            object tag = n.Tag;
            if (tag is KeyValuePair<IHDU, int>)
            {
                PassToDefaultHandler((KeyValuePair<IHDU, int>)tag);
                // HDU node itself is used to cick the default handler. Once session is opened, child node will be used
                // to activate the default handler session. so we need to change the tag to the HDU node forget the HDU now.
                n.Tag = ((KeyValuePair<IHDU, int>)tag).Key;
            }
            else if (tag is KeyValuePair<Guid, int>)
            {
                KeyValuePair<Guid, int> sessionPair = (KeyValuePair<Guid, int>)tag;
                _handlersManager.ActivateSession(sessionPair.Key, sessionPair.Value);
            }
        }
        #endregion

        private TreeNode _curTreeNode;
        private Cursor _oldCursor;
        private IHDU _hduAtMouse;
        private HandlersManager _handlersManager;

        private void fileCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseSelectedItem();
        }

        private void CloseSelectedItem()
        {
            TreeNode n = structureTreeView.SelectedNode;
            if (n != null && n.Tag != null)
            {
                if (n.Tag is IFITSFile)
                {
                    CloseFileFromNode(n);
                }
                else if (n.Tag is KeyValuePair<Guid, int>)
                {
                    CloseHandler(n.Tag);
                }
            }
        }

        private void CloseFileFromNode(TreeNode n)
        {
            IFITSFile ff = (IFITSFile)n.Tag;
            foreach (TreeNode hduNode in n.Nodes)
            {
                foreach (TreeNode handlerNode in hduNode.Nodes)
                {
                    if (handlerNode.Tag is KeyValuePair<Guid, int>)
                    {
                        CloseHandler(handlerNode.Tag);
                    }
                }
            }
            structureTreeView.Nodes.Remove(n);
            ff.Close();
        }

        private void CloseHandler(object tag)
        {
            try
            {
                KeyValuePair<Guid, int> pair = (KeyValuePair<Guid, int>)tag;
                _handlersManager.CloseSession(pair.Key, pair.Value);
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportBug rb = new ReportBug();
            if (rb.ShowDialog() == DialogResult.Yes)
            {
                //System.Net.Mail.MailMessage mm = new System.Net.Mail.MailMessage(from
            }
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            AboutForm f = new AboutForm();
            f.ShowDialog();
        }
        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutForm f = new AboutForm();
            f.ShowDialog();
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            InvokeSettingsDialog();
        }

        private void InvokeSettingsDialog()
        {
            Settings sd = new Settings();
            if (sd.ShowDialog() == DialogResult.OK)
            {
                if (NajmConfigs.IsDirty)
                {
                    _handlersManager.LoadHandlersOnly(NajmConfigs.Handlers);
                    SaveNajmConfigs();
                }
            }
            else
            {
                NajmConfigs.DiscardChanges();
            }
        }

        private void MainAppForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveNajmConfigs();
        }

        private void SaveNajmConfigs()
        {
            try
            {
                NajmConfigs.Save();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string binRoot = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string helpFilePath = Path.Combine(binRoot, @"..\Doc\help.pdf");
            Process.Start(helpFilePath);
        }

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TreeNode n = (structureTreeView).SelectedNode;
            if (n != null && n.Tag != null && n.Tag is KeyValuePair<Guid, int>)
            {
                CloseHandler(n.Tag);
            }
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvokeSettingsDialog();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvokeFileOpenDialog();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode n = structureTreeView.SelectedNode;
            if (n != null && n.Tag != null && n.Tag is IFITSFile)
            {
                CloseFileFromNode(n);
            }
        }

        private void closeAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // closing file will remove nodes so we can't do it inside iterator, instead
            // we collect the nodes references then loop on the collection
            if (structureTreeView.Nodes.Count > 0)
            {
                int i = 0;
                TreeNode[] nodes = new TreeNode[structureTreeView.Nodes.Count];
                foreach (TreeNode n in structureTreeView.Nodes)
                {
                    nodes[i++] = n;
                }
                foreach (TreeNode n in nodes)
                {
                    if (n != null && n.Tag != null && n.Tag is IFITSFile)
                    {
                        CloseFileFromNode(n);
                    }
                }
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
