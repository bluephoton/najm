# =====================================================================================================================
# import what we need
# =====================================================================================================================
import clr
clr.AddReference("System.Windows.Forms", "System.Drawing")
from System.Windows.Forms import *
from System.Drawing import *

# =====================================================================================================================
# class to encapsulate UI logic
# =====================================================================================================================
class UIMngr:
    # --------------------------------------------
    # Constructor - Do whaterver you want here
    # --------------------------------------------
    def __init__(self, handler):
        # create controls
        self.targetDirectoryTextBox = TextBox()
        self.groupBox = GroupBox()
        self.label1 = Label()
        self.browseForFolderButton = Button()
        self.imageConfigFileTextBox = TextBox()
        self.browseForFileButton = Button()
        self.baseFilenameTextBox = TextBox()
        self.label2 = Label()
        self.label3 = Label()
        self.label4 = Label()
        self.framesPerSecondNumeric = NumericUpDown()
        self.generateButton = Button()
        self.pictureBox = PictureBox()
        self.saveFramesCheckbox = CheckBox()
        self.creditsTextBox = Label()
        # listener
        self.handler = handler
    # --------------------------------------------
    # Initialize UI
    # --------------------------------------------
    def Load(self, panel):
        # credits text box
        self.creditsTextBox.Location = Point(0, 2)
        self.creditsTextBox.BorderStyle = BorderStyle.Fixed3D
        self.creditsTextBox.Size = Size(558, 23)
        self.creditsTextBox.TextAlign = ContentAlignment.MiddleCenter
        self.creditsTextBox.Text = "FITS Cube to Animated GIF. By: Marshall Roth, Mohamed Enein  -  Aug. 2009"
        self.creditsTextBox.ForeColor = Color.Navy
        # label1
        self.label1.AutoSize = True;
        self.label1.Location = Point(6, 16);
        self.label1.Size = Size(84, 13);
        self.label1.TabIndex = 1;
        self.label1.Text = "Target directory:";
        # targetDirectoryTextBox
        self.targetDirectoryTextBox.Location = Point(9, 32)
        self.targetDirectoryTextBox.Name = "targetDirectoryTextBox"
        self.targetDirectoryTextBox.Size = Size(460, 20)
        self.targetDirectoryTextBox.TabIndex = 0
        self.targetDirectoryTextBox.TextChanged += self.TargetDirTextChanged
        # browseForFolderButton
        self.browseForFolderButton.Location = Point(476, 31);
        self.browseForFolderButton.Size = Size(75, 23);
        self.browseForFolderButton.TabIndex = 2;
        self.browseForFolderButton.Text = "Browse";
        self.browseForFolderButton.UseVisualStyleBackColor = True;
        self.browseForFolderButton.Click += self.browseForFolderButtonClick
        # label2
        self.label2.AutoSize = True;
        self.label2.Location = Point(6, 59);
        self.label2.Text = "Base filename:";
        # baseFilenameTextBox
        self.baseFilenameTextBox.Location = Point(9, 75);
        self.baseFilenameTextBox.Size = Size(204, 20);
        self.baseFilenameTextBox.TabIndex = 3;
        self.baseFilenameTextBox.TextChanged += self.BaseFileNameTextChanged
        # label3
        self.label3.AutoSize = True;
        self.label3.Location = Point(227, 59);
        self.label3.Text = "Frames per second";
        # framesPerSecondNumeric
        self.framesPerSecondNumeric.Location = Point(230, 75);
        self.framesPerSecondNumeric.Size = Size(70, 20);
        self.framesPerSecondNumeric.TabIndex = 3;
        self.framesPerSecondNumeric.Minimum = 1
        self.framesPerSecondNumeric.Maximum = 30
        self.framesPerSecondNumeric.ValueChanged += self.FramesPerSecondValueChanged
        # saveFramesCheckbox
        self.saveFramesCheckbox.Location = Point(350, 75)
        self.saveFramesCheckbox.Size = Size(140, 20)
        self.saveFramesCheckbox.Text = "Save single frames"
        self.saveFramesCheckbox.CheckStateChanged += self.SaveFramesCheckStateChanged
        # generateButton
        self.generateButton.Location = Point(476, 75);
        self.generateButton.Size = Size(75, 23);
        self.generateButton.TabIndex = 6;
        self.generateButton.Text = "Generate";
        self.generateButton.UseVisualStyleBackColor = True;
        self.generateButton.Click += self.GenerateButtonClick
        # label4
        self.label4.AutoSize = True;
        self.label4.Location = Point(6, 102);
        self.label4.Text = "Image config file:";
        # imageConfigFileTextBox
        self.imageConfigFileTextBox.Location = Point(9, 118);
        self.imageConfigFileTextBox.Size = Size(460, 20);
        self.imageConfigFileTextBox.TabIndex = 3;
        self.imageConfigFileTextBox.TextChanged += self.ImageConfigFileTextBoxTextChanged
        # browseForFileButton
        self.browseForFileButton.Location = Point(476, 118);
        self.browseForFileButton.Size = Size(75, 23);
        self.browseForFileButton.TabIndex = 6;
        self.browseForFileButton.Text = "Browse";
        self.browseForFileButton.UseVisualStyleBackColor = True;
        self.browseForFileButton.Click += self.BrowseForFileButtonClick
        # groupBox
        self.groupBox.Controls.Add(self.generateButton)
        self.groupBox.Controls.Add(self.label3)
        self.groupBox.Controls.Add(self.framesPerSecondNumeric)
        self.groupBox.Controls.Add(self.label2)
        self.groupBox.Controls.Add(self.baseFilenameTextBox)
        self.groupBox.Controls.Add(self.browseForFolderButton)
        self.groupBox.Controls.Add(self.label1)
        self.groupBox.Controls.Add(self.targetDirectoryTextBox)
        self.groupBox.Controls.Add(self.saveFramesCheckbox)
        self.groupBox.Controls.Add(self.label4)
        self.groupBox.Controls.Add(self.imageConfigFileTextBox)
        self.groupBox.Controls.Add(self.browseForFileButton)
        self.groupBox.Location = Point(0, 25)
        self.groupBox.Size = Size(558, 153)
        self.groupBox.TabIndex = 2
        self.groupBox.TabStop = False
        # pictureBox
        self.pictureBox.BorderStyle = BorderStyle.Fixed3D
        self.pictureBox.Location = Point(9, 203)
        self.pictureBox.Size = Size(542, 400)
        self.pictureBox.SizeMode = PictureBoxSizeMode.Zoom
        # add to panel
        panel.Controls.Add(self.creditsTextBox)
        panel.Controls.Add(self.groupBox)
        panel.Controls.Add(self.pictureBox)
    # --------------------------------------------
    # initialize controls vlaues
    # --------------------------------------------
    def Initialize(self, sd):
        self._sd = sd
        self.baseFilenameTextBox.Text = sd._baseFileName
        self.targetDirectoryTextBox.Text = sd._targetDir
        self.framesPerSecondNumeric.Value = sd._framesPerSecond
        self.pictureBox.ImageLocation = self._sd.FilePath()
        self.saveFramesCheckbox.CheckState = (CheckState.Unchecked, CheckState.Checked)[self._sd._saveFrames]
        self.imageConfigFileTextBox.Text = (sd._nicFile, "")[sd._nicFile == None]
    # --------------------------------------------
    # release things
    # --------------------------------------------
    def Release(self):
        self.pictureBox.ImageLocation = None
        self.pictureBox.Image = None
    # --------------------------------------------
    # GenerateButton event handler
    # --------------------------------------------
    def GenerateButtonClick(self, sender, args):
        self.pictureBox.Image = None
        self.handler.GenerateAGIF(self._sd)
        self.pictureBox.ImageLocation = self._sd.FilePath()
    # --------------------------------------------
    # browseForFolderButtonClick event handler
    # --------------------------------------------
    def browseForFolderButtonClick(self, sender, args):
        d = FolderBrowserDialog()
        if d.ShowDialog() == DialogResult.OK:
            self.targetDirectoryTextBox.Text = d.SelectedPath
            self._sd._targetDir = d.SelectedPath
    # --------------------------------------------
    # TargetDirTextChanged event handler
    # --------------------------------------------
    def TargetDirTextChanged(self, sender, args):
        self._sd._targetDir = self.targetDirectoryTextBox.Text
    # --------------------------------------------
    # FramesPerSecondValueChanged event handler
    # --------------------------------------------
    def FramesPerSecondValueChanged(self, sender, args):
        self._sd._framesPerSecond = self.framesPerSecondNumeric.Value
    # --------------------------------------------
    # BaseFileNameTextChanged event handler
    # --------------------------------------------
    def BaseFileNameTextChanged(self, sender, args):
        self._sd._baseFileName = self.baseFilenameTextBox.Text
    # --------------------------------------------
    # SaveFramesCheckStateChanged event handler
    # --------------------------------------------
    def SaveFramesCheckStateChanged(self, sender, args):
        self._sd._saveFrames = (self.saveFramesCheckbox.CheckState == CheckState.Checked)
    # --------------------------------------------
    # imageConfigFileTextBoxTextChanged event handler
    # --------------------------------------------
    def ImageConfigFileTextBoxTextChanged(self, sender, args):
        self._sd._nicFile = self.imageConfigFileTextBox.Text
    # --------------------------------------------
    # BrowseForFileButtonClick event handler
    # --------------------------------------------
    def BrowseForFileButtonClick(self, sender, args):
        d = OpenFileDialog()
        d.Filter = "Najm image config file(*.nic)|*.nic"
        if d.ShowDialog() == DialogResult.OK:
	        self.imageConfigFileTextBox.Text = d.FileName
 