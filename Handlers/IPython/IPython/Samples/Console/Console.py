HDU = None
class SessionData:
    def __init__(self):
        self._hdu = None

class NajmHandler:
    # --------------------------------------------
    # Constructor - Do whaterver you want here
    # --------------------------------------------
    def __init__(self):
        # ID
        self.ID = "{5CB3C60C-CB36-4925-9C00-69771586F5B9}"
        # Name of the Handler
        self.Name = "Demo Pythonn Handler"
        # Where to find the toolstrip image
        self.ToolstripImageName ="consoleicon.png"
        # What tooltip associated with toolstrip button
        self.Tooltip = "Console window for running immediate python code"
        # Your extra code goes here
        self._sd = SessionData()
    # --------------------------------------------
    # Verify that you can handle the passed hdus
    # --------------------------------------------
    def CanHandle(self, hdus):
        return True
    # --------------------------------------------
    # Perform any per handler initialization tasks
    # --------------------------------------------
    def Initialize(self, najmHandlersMngr, panel):
        self._textBox = RichTextBox()
        panel.Controls.Add(self._textBox)
        self._textBox.Dock = DockStyle.Fill
        self._textBox.KeyPress += self.OnKeyPress
    # ********************************************
    # Called when you hit Enter
    # ********************************************
    def OnKeyPress(self, sender, args):
        if args.KeyChar == u'\r':
            self.currentLineIndex = self._textBox.GetLineFromCharIndex(self._textBox.GetFirstCharIndexOfCurrentLine())
            if not self._textBox.Lines[self.currentLineIndex-1].endswith(u'/'):
                code = self.GetCodeLines().Trim()
                if not "".IsNullOrEmpty(code):
	                self.ExecuteCode(code)
    # **********************************************
    # Get code lines. will get rid of '/' at the end
    # **********************************************
    def GetCodeLines(self):
        lines = self._textBox.Lines
        code = [lines[self.currentLineIndex-1]]
        for i in range(self.currentLineIndex-2, -1, -1):
            if lines[i].endswith('/'):
                code[:0] = [lines[i].Substring(0, lines[i].Length - 1)]
            else:
                break
        return "\n".join(code)
    # **********************************************
    # Get code lines. will get rid of '/' at the end
    # **********************************************
    def ExecuteCode(self, code):
        #MessageBox.Show(code)
        result = eval(code, globals(),locals())
        self._textBox.AppendText(str(result))
        self._textBox.AppendText("\n")
    # --------------------------------------------
    # Perform any per session initialization tasks
    # --------------------------------------------
    def OpenSession(self, sid, hdus):
        self._sd._hdu = hdus[0]
        globals()["HDU"] = hdus[0]
        return self._sd
    # --------------------------------------------
    # Session is closing, clean things here
    # --------------------------------------------
    def CloseSession(self, sd):
        return
    # --------------------------------------------
    # UI is ready, do whatever you need to do here
    # --------------------------------------------
    def ActivateSession(self, sd):
        self._textBox.Text = "Usage:\n\n'HDU' and 'self' are the main objects avilable to you. using them you can access all object model.\n Type a command then enter to execute.\n For multi line commands, each line (except the last one) has to end with '/'\n\n"
        return
