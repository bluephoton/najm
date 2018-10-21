class SessionData:
    def __init__(self):
        self._hdu = None
        self._textBox = None

class NajmHandler:
    # Constructor
    def __init__(self):
        # ID
        self.ID = "{B705A42E-7671-4097-A366-6DC6534CCFF9}"
        # Name of the Handler
        self.Name = "Sample.Table Fields"
        # Where to find the toolstrip image
        self.ToolstripImageName = "tsi.jpg"
        # What tooltip associated with toolstrip button
        self.Tooltip = "Lists the fields in a FITS table"
        # 
        self._sd = SessionData()
        
    def CanHandle(self, hdus):
        if hdus[0].IsTable:
	        return True
        return False
    def Initialize(self, najmHandlersMngr, panel):
        self._sd._textBox = RichTextBox()
        panel.Controls.Add(self._sd._textBox)
        self._sd._textBox.Dock = DockStyle.Fill
        return
    def OpenSession(self, sid, hdus):
        self._sd._hdu = hdus[0]
        return self._sd
    def CloseSession(self, sd):
        return
    def ActivateSession(self, sd):
        t = ""
        table = sd._hdu.Table
        for fi in table.Header:
            t = t + fi.Name + "\n"
        sd._textBox.Text = t
        return
