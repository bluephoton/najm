# see: http://ozviz.wasp.uwa.edu.au/~pbourke/dataformats/gif/
# =====================================================================================================================
# import what we need
# =====================================================================================================================
import clr
import array
import System
import System.IO
clr.AddReference("System.Windows.Forms", "System.Drawing")
from System.Windows.Forms import *
from System.Drawing import *
from System.Drawing.Imaging import *
from System.IO import *
from System import Array
from array import *

# =====================================================================================================================
# class to encapsulate Animated GIF creation
# =====================================================================================================================
class AGIFBuilder:
    # --------------------------------------------
    # Constructor - Do whaterver you want here
    # --------------------------------------------
    def __init__(self, filePath, framesPerSecond):
        self.CurrFrameIndex = 0
        self.Delay = int(100 / framesPerSecond)
        self.FileStream = BinaryWriter(File.Open(filePath, FileMode.Create))
    # --------------------------------------------
    # Add frame to the AGIF
    # --------------------------------------------
    def AddFrame(self, image):
        # get image bytes
        memStream = MemoryStream()
        image.Save(memStream, ImageFormat.Gif)
        memStream.Close()
        buffer = memStream.ToArray()
        # Copy blocks once from the first image and apply to whole file
        if self.CurrFrameIndex == 0:
            # write header blocks
            self.FileStream.Write(buffer, 0, 781)
            # Application extension (Netscape extension to repeat rendering)
            NSAppExt = Array[System.Byte]([33, 255, 11, 78, 69, 84, 83, 67, 65, 80, 69, 50, 46, 48, 3, 1, 0, 0, 0])
            self.FileStream.Write(NSAppExt)
        # write Graphics Control Extension
        self.WriteGCE()
        # write image date (pixles)
        self.FileStream.Write(buffer, 789, buffer.Length - 790)
        # increment frame index
        self.CurrFrameIndex += 1
    # --------------------------------------------
    # Finalize
    # --------------------------------------------
    def Close(self):
        # write GIF trailer (';')
        self.FileStream.Write(0x3B)
        self.FileStream.Close()
    # --------------------------------------------
    # Write a GCE to the stream
    # --------------------------------------------
    def WriteGCE(self):
        # extension introducer, GCE label, block size, disposal method, delay time, 
        # transparent color index, block terminator
        gce = Array[System.Byte]([0x21, 0xF9, 0x04, 0x09, self.Delay & 0xFF, self.Delay >> 8, 0xFF, 0x00])
        self.FileStream.Write(gce)
