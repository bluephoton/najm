class SessionData:
    def __init__(self):
        self._hdu = None

class NajmHandler:
    # --------------------------------------------
    # Constructor - Do whaterver you want here
    # --------------------------------------------
    def __init__(self):
        # Name of the Handler
        self.Name = "Demo Pythonn Handler"
        # Where to find the toolstrip image
        self.ToolstripImageName = "C:\\temp\\demo\\demoicon.jpg"
        # What tooltip associated with toolstrip button
        self.Tooltip = "Do some nasty thing...indeed!"
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
        return
    # --------------------------------------------
    # Perform any per session initialization tasks
    # --------------------------------------------
    def OpenSession(self, sid, hdus):
        self._sd._hdu = hdus[0]
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
        return
