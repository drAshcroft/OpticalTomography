
import clr
clr.AddReferenceToFileAndPath( 'MathHelpLib.dll')
clr.AddReferenceToFileAndPath( 'ImageViewer.dll')
clr.AddReferenceToFileAndPath( 'dorecons.dll' )

from MathHelpLib import *
from ImageViewer import *
from DoRecons import *

clr.AddReference('System.Drawing')
from System import *
from System.Drawing import *
from System.Drawing.Imaging import *
from System.Threading import Thread, ThreadStart, ParameterizedThreadStart

from System.Threading.Tasks import Parallel

ImageDisp =0


def SetImageOutput(ImageDisplay):
    global ImageDisp
    ImageDisp = ImageDisplay

def ThreadLoopImageSave(Parameters):
    global GlobalPassData
    dataEnvironment = Parameters[0]
    ImageIndex = Parameters[1]
    IProcessFunction=Parameters[2]
    Params = [dataEnvironment, ImageIndex];
    image = IProcessFunction(Params)
    if (ImageDisp !=0):
        ImageDisp.DisplayImage(ImageIndex, image) 
    dataEnvironment.AllImages[ImageIndex] = image

def ThreadLoopImage(Parameters):
    global GlobalPassData
    dataEnvironment = Parameters[0]
    ImageIndex = Parameters[1]
    IProcessFunction=Parameters[2]
    Params = [dataEnvironment, ImageIndex];
    image = IProcessFunction(Params)
    if (ImageDisp !=0):
        ImageDisp.DisplayImage(ImageIndex, image) 

def SelectAndSmoothCellPositions(dataEnvironment, X_PositionsB2, Y_PositionsB):
            NewLines=  Filters.CenterCellsTool2Form.PreFitLines(dataEnvironment, X_PositionsB2, Y_PositionsB) 
            X_PositionsB2 = NewLines["UpdatedX"]
            Y_PositionsB = NewLines["UpdatedY"]
            return (X_PositionsB2,Y_PositionsB)



def FilterImage(Method, FilterSize, Image):
            if (Method == "median"):
                Filter =   Filters.Effects.RankOrder.MedianFilterTool() 
                return Filter.DoEffect(None, Image, None, FilterSize) 
            if (Method == "average"):
                Filter =   Filters.Effects.RankOrder.AverageFilterTool() 
                return Filter.DoEffect(None, Image, None, FilterSize) 
            if (Method == "alphatrimmed"):
                Filter =   Filters.Effects.RankOrder.AlphaTrimmedMeanFilterTool() 
                return Filter.DoEffect(None, Image, None, FilterSize) 
            if (Method == "opening"):
                Filter =   Filters.Effects.Morphology.OpeningTool() 
                return Filter.DoEffect(None, Image, None, FilterSize) 
            if (Method == "closing"):
                Filter =   Filters.Effects.Morphology.ClosingTool() 
                return Filter.DoEffect(None, Image, None, FilterSize) 




def BatchLoopThroughImagesSave(IProcessFunction,dataEnvironment):
            global GlobalPassData
            global ThreadPool
            imageIndex = 0 

            ThreadParams =[]
                                
            for CurrentParticle in range( dataEnvironment.AllImages.Count ):
                imageIndex = CurrentParticle
                Params = [dataEnvironment, imageIndex, IProcessFunction ]
                ThreadParams.append(Params)
                
            Parallel.ForEach(ThreadParams, lambda tParams:ThreadLoopImageSave(tParams))
               
           

def BatchLoopThroughImages(IProcessFunction,dataEnvironment):
            global GlobalPassData
            global ThreadPool
            imageIndex = 0 

            ThreadParams =[]
                                
            for CurrentParticle in range( dataEnvironment.AllImages.Count ):
                imageIndex = CurrentParticle
                Params = [dataEnvironment, imageIndex, IProcessFunction ]
                ThreadParams.append(Params)
                
            Parallel.ForEach(ThreadParams, lambda tParams:ThreadLoopImage(tParams))
                #     PythonScripting.MacroHelper.DoEvents() 
                #    if ( PythonScripting.MacroHelper.ErrorOnThread == True):
                #        Thread.CurrentThread.Abort() 

        
def SaveExampleImages(dataEnvironment, DataSourcePath, DensityGrid):
            try:
                image = dataEnvironment.AllImages[0] 
                MathHelpsFileLoader.Save_Bitmap(DataSourcePath + "\\FirstPP.bmp", image) 
            except ValueError:    
                pass   
            try:
                image = dataEnvironment.AllImages[dataEnvironment.AllImages.Count / 4] 
                MathHelpsFileLoader.Save_Bitmap(DataSourcePath + "\\QuarterPP.bmp", image) 
            except ValueError:    
                pass       
            try:
                image = dataEnvironment.AllImages[dataEnvironment.AllImages.Count / 2] 
                MathHelpsFileLoader.Save_Bitmap(DataSourcePath + "\\HalfPP.bmp", image) 
            except ValueError:    
                pass       
            try:
                image = dataEnvironment.AllImages[dataEnvironment.AllImages.Count * 3 / 4] 
                MathHelpsFileLoader.Save_Bitmap(DataSourcePath + "\\LastQuarterPP.bmp", image) 
            except ValueError:    
                pass       
            try:
                image = dataEnvironment.AllImages[dataEnvironment.AllImages.Count - 1] 
                MathHelpsFileLoader.Save_Bitmap(DataSourcePath + "\\lastPP.bmp", image) 
            except ValueError:    
                pass      
            DensityGrid.SaveCross(DataSourcePath + "\\CrossSections.jpg") 

   
def Average( array):
    return array.Average()

def Stdev(array):
    ave = array.Average()
    um = 0 
    d = 0 
    for i in range(array.Length):
        d = array[i] - ave
        sum += d * d
    
    sum = Math.Sqrt(sum / array.Length)
    return sum

def Stdev(array, Average):
    ave = Average
    sum = 0
    d = 0
    for i in range(array.Length):
        d = array[i] - ave
        sum += d * d
    sum = Math.Sqrt(sum) / array.Length
    return sum
