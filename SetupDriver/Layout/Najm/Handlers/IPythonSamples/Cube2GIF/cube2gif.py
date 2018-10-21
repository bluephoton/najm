# =====================================================================================================================
# import what we need
# =====================================================================================================================
from ui import *
from AGIF import *
from System.IO import Path
# =====================================================================================================================
# session data
# =====================================================================================================================
class SessionData:
    def __init__(self, hdu, colorTable, colorMap):
        self._hdu = hdu
        self._targetDir = Path.GetDirectoryName(hdu.File.Name)
        self._baseFileName = Path.GetFileNameWithoutExtension(hdu.File.Name)
        self._framesPerSecond = 5
        self._colorTable = colorTable
        self._colorMap = colorMap
        self._saveFrames = False
        self._nicFile = None
    def FilePath(self):
        return Path.Combine(self._targetDir, self._baseFileName) + '.gif'
    def IndexedFilePath(self, index):
        return Path.Combine(self._targetDir, self._baseFileName) + '_' + '%02d' % index + '.gif'
# =====================================================================================================================
# our custom handler
# =====================================================================================================================
class NajmHandler:
    # --------------------------------------------
    # Constructor - Do whaterver you want here
    # --------------------------------------------
    def __init__(self):
        # ID
        self.ID = "{871E9132-A1FF-410b-A897-5C72F003F7A4}"
        # Name of the Handler
        self.Name = "AGIF from FITS Cube"
        # Where to find the toolstrip image
        self.ToolstripImageName ="cube2gif.png"
        # What tooltip associated with toolstrip button
        self.Tooltip = "Creates animated GIF from a FITS Cube"
        # Your extra code goes here
    # --------------------------------------------
    # Verify that you can handle the passed hdus
    # --------------------------------------------
    def CanHandle(self, hdus):
        if hdus[0].DataMngr.NumSlices <= 1:
            MessageBox.Show("This HDU doesn't contain a Cube");
            return False
        return True
    # --------------------------------------------
    # Perform any per handler initialization tasks
    # --------------------------------------------
    def Initialize(self, najmHandlersMngr, panel):
        self.UIMngr = UIMngr(self)
        self.UIMngr.Load(panel)
    # --------------------------------------------
    # Perform any per session initialization tasks
    # --------------------------------------------
    def OpenSession(self, sid, hdus):
        self._hdu = hdus[0]
        # color table and color map, used as a default if no .nic file provided
        colorTable = ColorTableFactory.Create(ColorTableType.Indexed)
        colorTable.Initialize(1 << 14, self._hdu.DataMngr.Minimum, self._hdu.DataMngr.Maximum)
        colorTable.ScalingAlgorithm = ScalingAlgorithmFactory.Create(ScalingAlgorithms.Logarithmic)
        #  color map
        colorMap = ColorMapFactory.Create(ColorMapTypes.Gray, "")
        colorMap.Initialize()
        # return with session data
        return SessionData(hdus[0], colorTable, colorMap)
    # --------------------------------------------
    # Session is closing, clean things here
    # --------------------------------------------
    def CloseSession(self, sd):
        self.UIMngr.Release()
        return
    # --------------------------------------------
    # UI is ready, do whatever you need to do here
    # --------------------------------------------
    def ActivateSession(self, sd):
        self.UIMngr.Initialize(sd)
    # --------------------------------------------
    # Generate AGIF
    # --------------------------------------------
    def GenerateAGIF(self, sd):
        # get param
        agifBuilder = AGIFBuilder(sd.FilePath(), sd._framesPerSecond)
        for i in range(self._hdu.DataMngr.NumSlices):
            image = self.CreateFrame(i, sd)
            bitmap = image.CreateBitmap();
            if sd._saveFrames:
                bitmap.Save(sd.IndexedFilePath(i), ImageFormat.Gif)
            agifBuilder.AddFrame(bitmap)
            image.Reset();
        agifBuilder.Close()
    # --------------------------------------------
    # Create frame
    # --------------------------------------------
    def CreateFrame(self, index, sd):
        # slice
        self._hdu.DataMngr.CurrentSlice = index
        # Image
        width = self._hdu.Axes[0].NumPoints
        height = self._hdu.Axes[1].NumPoints
        data = self._hdu.DataMngr.Data
        if sd._nicFile == None or sd._nicFile == "":
            image = ImageFactory.Create(ImageTypes.Indexed)
            image.Initialize(data, width, height, sd._colorTable, sd._colorMap)
        else:
            image = ImageFactory.FromFile(sd._nicFile)
            image.Initialize(data, width, height)
        # display frame
        return image
