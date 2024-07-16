
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

import BasicFunctions
from BasicFunctions import *

import scipy

Variables=PythonHelps.SetupVariables('C:\\Development\\CellCT\\DataIN\\cct001_20110418_135007','C:\\Development\\CellCT\\DataIN\\cct001_20110418_135007\\pp')

#region global values
TempPath = Variables["TempPath"] 
ImageDisp =Variables["ImageDisp"] 
SetImageOutput(ImageDisp)
DataSourcePath = Variables["DataPath"] 
Executable = Variables["Executable"] 
ExecutablePath = Variables["ExecutablePath"] 
LibraryPath = Variables["LibraryPath"] 
dataEnvironment = Variables["dataEnvironment"] 
GlobalPassData = Variables["GlobalPassData"] 
ScriptParams=Variables["ScriptParams"] 
ImageDisp=Variables["ImageDisp"]
ColorImage =PythonHelps.IsColorImage(dataEnvironment)
DataOutPath = dataEnvironment.DataOutFolder

if (IO.Directory.Exists(DataOutPath) == False):
    IO.Directory.CreateDirectory(DataOutPath) 

#ThreadPool = PythonHelps.StartThreadPool(20)
#vars for scripts

BackgroundMask=0

CellSize = 170
tCellSize = 0
ntCellSize = 0
CellHalf = 85
       
X_PositionsB2=0
Y_PositionsB=0

def ImageLoadAndFind(Params):
            dataEnvironment=Params[0]
            ImageNumber=Params[1]
            BitmapImage=dataEnvironment.AllImages[ImageNumber]
            
            GlobalPassData=Filters.ReplaceStringDictionary()
            GlobalPassData.AddSafe("Num Loaded Channels", BitmapImage.NChannels) 

            ##make sure there is only one channel to work with
            if BitmapImage.NChannels > 1:
                Filter = Filters.Adjustments.GrayScaleEffectChannel()
                BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData, 1)

            if ColorImage == True:
                BitmapImage = MathHelpsFileLoader.FixVisionGateImage(BitmapImage, 2)

          
            FindRough(dataEnvironment, ImageNumber, BitmapImage, GlobalPassData)

            if dataEnvironment.FluorImage==False:
                if ScriptParams["GlobalFlatten"].ToLower() == "true":
                    Filter =  Filters.Effects.Flattening.FlattenEdges1DErrorCorrected()
                    BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData)

            return BitmapImage

def FindRough(dataEnvironment, ImageNumber, BitmapImage, GlobalPassData):
            global tCellSize
            global ntCellSize
            global Y_PositionsB
            global X_PositionsB2
            global NumBlobs

            #reduce the size of the image to make processing faster
            ReductionAmount = 2
            if dataEnvironment.FluorImage == True:
                ##Invert Contrast
                Filter =  Filters.Adjustments.InvertEffect()
                BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData)
                ReductionAmount = 2

            if ReductionAmount == 2:
                ##reduce size
                Filter =  Filters.Adjustments.downSampleEffect()
                BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData)
            else:
                if ReductionAmount == 4:
                    #reduce size
                    Filter =  Filters.Adjustments.downSampleEffect()
                    BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData)

                    #reduce size
                    Filter =  Filters.Adjustments.downSampleEffect()
                    BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData)

            ThreshImage=None
            try:
                #if (dataEnvironment.FluorImage == True):
                #    #Otsu Threshold
                #    Filter =  Filters.Thresholding.OtsuThresholdEffect()
                #    ThreshImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData)
                #else:
                #    #Iterative Threshold
                #    Filter =  Filters.Thresholding.IterativeThresholdEffect()
                #    ThreshImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData)

                 #Iterative Threshold
                Filter =  Filters.Thresholding.IterativeThresholdEffect()
                ThreshImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData)

                #WaterShed
                Filter =  Filters.Blobs.WaterShedTool()
                Filter.DoEffect(dataEnvironment, ThreshImage, GlobalPassData)

                ##Get Biggest Blob
                Filter =  Filters.Blobs.GetBiggestCenterBlob()
                BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData, GlobalPassData["Blobs"], False)
            except ValueError:
                print("error")

            if (GlobalPassData.ContainsKey("MaxBlob") == False):
                #Otsu Threshold
                Filter =   Filters.Thresholding.OtsuThresholdEffect()
                ThreshImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData) 

                #WaterShed
                Filter =   Filters.Blobs.WaterShedTool()
                Filter.DoEffect(dataEnvironment, ThreshImage, GlobalPassData) 

                #Get Biggest Blob
                Filter =   Filters.Blobs.GetBiggestCenterBlob()
                BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData, GlobalPassData["Blobs"], False) 

            x=0
            y=0
            
            if (GlobalPassData.ContainsKey("MaxBlob") == True):
                try:
                    Rect = GlobalPassData["MaxBlob"]
                    print(" -- " + str(Rect))
                    x = Rect.CenterOfGravity.X * ReductionAmount
                    y = Rect.CenterOfGravity.Y * ReductionAmount

                    tCellSize = tCellSize + Rect.BlobBounds.Width * ReductionAmount
                    tCellSize = tCellSize + Rect.BlobBounds.Height * ReductionAmount
                    ntCellSize+=1 
                except ValueError:
                    x = int.MinValue
                    y = int.MinValue
            else:
                x = int.MinValue
                y = int.MinValue

            try:
                Filters.CenterCellsTool2Form.StoreBlobLocation(dataEnvironment, ImageNumber,GlobalPassData["Blobs"],ReductionAmount)
            except ValueError:
                try:
                    Filters.CenterCellsTool2Form.StoreBlobLocation(dataEnvironment, ImageNumber, GlobalPassData["Blobs"],ReductionAmount)
                except ValueError:
                    pr(ValueError)

            
            X_PositionsB2[ImageNumber]=x+0
            Y_PositionsB[ImageNumber]=y+0

            if GlobalPassData.ContainsKey("NumBigBlobs") == False:
                NumBlobs[ImageNumber]=1
            else:
                NumBlobs[ImageNumber]=GlobalPassData["NumBigBlobs"]
            return BitmapImage

def ImageFindFine(Params):
            global X_PositionsB2
            global Y_PositionsB
            dataEnvironment=Params[0]
            ImageNumber=Params[1]
            BitmapImage=dataEnvironment.AllImages[ImageNumber]

            #each thread has to have its own copy of this.  probably a good idea to get a better method of passing around the data
            GlobalPassData=Filters.ReplaceStringDictionary()

            #get the rough position of this image
            Xo = Math.Truncate(X_PositionsB2[ImageNumber] - CellHalf)
            Yo = Math.Truncate(Y_PositionsB[ImageNumber] - CellHalf)
            CellArea = Rectangle(Xo, Yo, CellSize, CellSize)
            CellArea.Inflate(20, 20)
            Xo = CellArea.X
            Yo = CellArea.Y

            #cut out the rough position to improve the thresholding and the processing speed
            Filter =  PythonScripting.Programming_Tools.ClipImageToNewEffect() 
            BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData, CellArea) 

            if (dataEnvironment.FluorImage == True):
                Filter =  Filters.Adjustments.InvertEffect()
                BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData)


            AveX = 0
            AveY = 0
            #make of a copy of the original image to initialize the find
            OriginalTests = BitmapImage 
            Rect = None 
            #this should be in the configuration file
            NumTestRuns = 4 
            cc=0
            BitmapImage2=None 
            for i in range(NumTestRuns):
                BitmapImage = OriginalTests.Copy() 
                #Add additional noise
                Filter =  Filters.Effects.Artistic.JitterEffect()
                BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData) 

                #Threshold the image
                Filter =   Filters.Thresholding.IterativeThresholdCornerFillEffect() 
                BitmapImage2 = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData) 

                if (ScriptParams["COGMethod"] == "Threshold"):
                    #Center Of Gravity
                    Filter =   Filters.Blobs.CenterOfGravityTool() 
                    Filter.DoEffect(dataEnvironment, BitmapImage2, GlobalPassData) 
                    #Data out of type :
                    GlobalPassData = Filter.PassData 
                else:
                    #Center Of Gravity using intensity
                    Filter =   Filters.Blobs.CenterOfGravityIntensityTool() 
                    Filter.DoEffect(dataEnvironment, OriginalTests, GlobalPassData, BitmapImage2) 
                    #Data out of type :
                    GlobalPassData = Filter.PassData 

                #Get Biggest Blob  ( there is only one blob from each of these.  it should be possible to do the watershedding again to avoid the image being pulled funny by debrie
                Filter =   Filters.Blobs.GetBiggestBlob() 
                Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData, GlobalPassData["Blobs"], False) 
                #Data out of type :
                GlobalPassData = Filter.PassData 

                if (GlobalPassData.ContainsKey("MaxBlob")==True):
                    try:
                        Rect = GlobalPassData["MaxBlob"] 
                        x = Rect.CenterOfGravity.X 
                        y = Rect.CenterOfGravity.Y 
                        AveX += x 
                        AveY += y 
                        cc=cc+1
                    except ValueError:
                        pass
             
            #figure out the average and offset the data after the clip
            AveX = Xo + AveX / cc 
            AveY = Yo + AveY / cc 

            global tCellSize
            tCellSize = tCellSize + Rect.BlobBounds.Width 
            tCellSize = tCellSize + Rect.BlobBounds.Height 
           
            X_PositionsB2[ImageNumber]=AveX
            Y_PositionsB[ImageNumber]=AveX

            return Filters.EffectHelps.FixImageFormat( BitmapImage2) 
     
def DivideAwayBackground(Params):
            global X_PositionsB2
            global Y_PositionsB
            dataEnvironment=Params[0]
            ImageNumber=Params[1]
            BitmapImage=dataEnvironment.AllImages[ImageNumber]
            GlobalPassData=Filters.ReplaceStringDictionary()
            try:
                #get the location of the cell
                CellArea =  Rectangle((X_PositionsB2[ImageNumber] - CellHalf), (Y_PositionsB[ImageNumber] - CellHalf), CellSize, CellSize) 

                #fluor images are bad for division, so divide the area that the cell is in
                if (dataEnvironment.FluorImage == False):
                    CellAreaPadded =  Rectangle(CellArea.Location, CellArea.Size) 
                    CellAreaPadded.Inflate(CellArea.Width, CellArea.Height) 
                    #Divide Image
                    Filter =   Filters.Effects.Flattening.DivideImage() 
                    BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData, BackgroundMask, CellAreaPadded) 
                else:
                    #or just fix the contrast
                    #Invert Contrast
                    Filter =   Filters.Adjustments.InvertEffect() 
                    BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData) 

                return BitmapImage 
            except ValueError:
                raise

def CutOutCell(Params):
            global X_PositionsB2
            global Y_PositionsB
            dataEnvironment=Params[0]
            ImageNumber=Params[1]
            BitmapImage=dataEnvironment.AllImages[ImageNumber]
            GlobalPassData=Filters.ReplaceStringDictionary()
            global CellHalf
            global CellSize
            print ("CellHalf" + str(CellHalf))
            CellArea =  Rectangle(Math.Truncate(X_PositionsB2[ImageNumber] - CellHalf), Math.Truncate(Y_PositionsB[ImageNumber] - CellHalf), CellSize, CellSize) 

            #Clip Image to  Image
            Filter =   PythonScripting.Programming_Tools.ClipImageToNewEffect() 
            #Parameters required: Clip Bounds as Rectangle
            BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData, CellArea) 

            #Invert Contrast
            Filter =   Filters.Adjustments.InvertEffect() 
            BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData) 

            # BitmapImage.Save(@"C:\Development\CellCT\testimages\image" + ImageNumber.ToString() + ".bmp") 
            if (ScriptParams["FlatMethod"].ToLower() == "plane"):
                Filter =   Filters.Effects.Flattening.FlattenEdges() 
                BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData, CellArea) 
            else:
                if (ScriptParams["FlatMethod"].ToLower() == "curve"):
                    BitmapImage =  Filters.Effects.Flattening.FlattenEdges1D.FlattenImageEdges(BitmapImage) 

            if (dataEnvironment.FluorImage == True):
                #Invert Contrast
                Filter =   Filters.Adjustments.InvertEffect() 
                BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData) 

            return BitmapImage 


DensityGrid =0 
ConvolutionFilter =0
Impulse =0
def CreateDensityGrid(dataEnvironment,convolutionFilter,impulse, BitmapImage):
            global ConvolutionFilter
            global DensityGrid
            global Impulse
            Impulse=impulse
            ConvolutionFilter=convolutionFilter

            GlobalPassData=Filters.ReplaceStringDictionary()
            #run one convoluation to check if the gpu is working
            BitmapImage = ConvolutionFilter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData, 1, impulse, True, False, 2, 2) 
            Slice = GlobalPassData["ConvolutionData"] 

            #create the correct densitygrid depending on which one is created
            Filter =   PythonScripting.Projection.CreateFilteredBackProjectionEffect() 
            BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData,(Slice.GetLength(0)), (Slice.GetLength(1)), 2, 2, 1, False, dataEnvironment.RunningOnGPU) 
            DensityGrid=Filter.PassData["FBJObject"]
            return DensityGrid


def DoSARTProjection(Params):
            global DensityGrid
            
            dataEnvironment=Params[0]
            ImageNumber=Params[1]
            BitmapImage=dataEnvironment.AllImages[ImageNumber]
            GlobalPassData=Filters.ReplaceStringDictionary()
            print(ImageNumber)

            if (ImageNumber % 5 ==0):
                AngleRadians = 2.0 * Math.PI / dataEnvironment.AllImages.Count * ImageNumber 
                Filter=PythonScripting.Projection.DoARTSliceProjectionEffect()
                BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData, None, DensityGrid, AngleRadians) 
            return  BitmapImage


def DoFBProjection(Params):
            global DensityGrid
            global ConvolutionFilter
            dataEnvironment=Params[0]
            ImageNumber=Params[1]
            BitmapImage=dataEnvironment.AllImages[ImageNumber]
            GlobalPassData=Filters.ReplaceStringDictionary()
            Slice = None 

            BitmapImage = ConvolutionFilter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData, 1, Impulse, True, False, 2, 2) 
            print("Convolution Filter = " + str(GlobalPassData.ContainsKey("ConvolutionData")))
            Slice = GlobalPassData["ConvolutionData"] 
            
            AngleRadians = 2.0 * Math.PI / dataEnvironment.AllImages.Count * ImageNumber 

            #limited preinterpolation makes the process go faster, but the gpu is fast enough that it can be done on the fly
            if (dataEnvironment.RunningOnGPU == False):
                Filter =   PythonScripting.Projection.InterpolateSliceEffect() 
                BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData, Slice, DensityGrid, AngleRadians) 
                Slice = Filter.PassData["ExpandedArray"] 

            Filter =   PythonScripting.Projection.DoSliceBackProjectionEffect() 
            BitmapImage = Filter.DoEffect(dataEnvironment, BitmapImage, GlobalPassData, Slice, DensityGrid, AngleRadians) 

            #return the convolution image for diagnostics
            return  ImageHolder(Slice) 

def ProcessBeforeConvolution(Params):

            dataEnvironment=Params[0]
            ImageNumber=Params[1]
            BitmapImage=dataEnvironment.AllImages[ImageNumber]
            GlobalPassData=Filters.ReplaceStringDictionary()
            #if (ScriptParams["SaveCenteredImage"].ToLower() == "True"):
            #    try:
            #      Directory.CreateDirectory(dataEnvironment.ExperimentFolder + "Centered\\") 
            #   except ValueError:   
            #   BitmapImage.Save(dataEnvironment.ExperimentFolder + "Centered\\" + string.Format(" 0:000 ", ImageNumber) + ".cct") 

            Method = ScriptParams["PreprocessingMethod"] 
            Radius = Filters.EffectHelps.ConvertToDouble(ScriptParams["ProprocessingRadius"]) 

            return FilterImage(Method, Radius, BitmapImage)

def BatchProcessBackgroundDivide( dataEnvironment):
            global GlobalPassData
            global X_PositionsB2
            global Y_PositionsB
            global tCellSize
            global CellHalf
            global CellSize
            global CellWanderMargin

            dataEnvironment.ProgressLog.AddSafe("Prep", "Divide") 
            #this is just to make the script functions easier, if needed convert to an image from ImageFileList
            BitmapImage =  ImageHolder( Bitmap(10, 10)) 

            dataEnvironment.ProgressLog.AddSafe("Position", "starting process Rough") 
           
            #get the rough size of the cell.  This is going to be a little large
            tCellSize = (tCellSize / len(X_PositionsB2)) / 2 

            #clean up the positions
            (X_PositionsB2,Y_PositionsB) = SelectAndSmoothCellPositions(dataEnvironment, X_PositionsB2, Y_PositionsB) 

            try:
                BackgroundMethod =ScriptParams["BackgroundSubMethod"]
                Filter =   Filters.StationaryPixelsForm(True) 
                #Parameters required: BitmapFiles as string[],  as , X_Positions as double[], Y_Positions as double[], ShowForm as bool, SubtractMethod as string
                BitmapImage = Filter.DoEffect2(dataEnvironment, BitmapImage, GlobalPassData, 100,BackgroundMethod , X_PositionsB2, Y_PositionsB, False, "Divide",  tCellSize, True)
            except ValueError:
                #fluor images just dont divide clean
                if (dataEnvironment.FluorImage == False ):
                    raise
            #Data out of type :
            GlobalPassData = Filter.PassData 

            dataEnvironment.ProgressLog.AddSafe("Position", "saving stationary pixels") 

            #save example backgrounds, both for this example and for use in the other reconstructions
            BackgroundMask = GlobalPassData["BackgroundMask"] 
            MathHelpsFileLoader.Save_Bitmap(DataSourcePath + "\\Background.bmp", BitmapImage) 
            MathHelpsFileLoader.Save_Raw(DataSourcePath + "\\Background.cct", BackgroundMask) 
            
            #maxBlobs = max(NumBlobs) 
            #dataEnvironment.ProgressLog.AddSafe("NumberOfCells", maxBlobs.ToString()) 

            #this is calculated in the stationary pixels form when everything was threaded
            #probably should just do this in the script to make it more readable
            CellSize = (GlobalPassData["MaxCellSize"]) 
            CellHalf = CellSize / 2 
            CellWanderMargin = CellSize 
            
            #mark out the whole region that the cell wanders through
            XMin = (int)(min(X_PositionsB2) - CellWanderMargin) 
            YMin = (int)(min(Y_PositionsB) - CellWanderMargin) 
            XMax = (int)(max(X_PositionsB2) + CellWanderMargin) 
            YMax = (int)(max(Y_PositionsB) + CellWanderMargin) 
            CellWanderArea =  Rectangle(XMin, YMin, XMax - XMin, YMax - YMin) 

            print(CellWanderArea) 
            dataEnvironment.ProgressLog.AddSafe("Position", "Dividing away background") 
            #now do the divide
            BatchLoopThroughImagesSave(DivideAwayBackground, dataEnvironment) 
            dataEnvironment.ProgressLog.AddSafe("Divide", "Divide Succeeded") 

def BatchProcessCenter( dataEnvironment):
            global GlobalPassData
            BitmapImage =  ImageHolder( Bitmap(10, 10)) 

            #do a fine find to get the exact location of the cells
            dataEnvironment.ProgressLog.AddSafe("Position", "Fine") 
            BatchLoopThroughImages(ImageFindFine, dataEnvironment) 

            global CellSize
            global CellHalf
            CellSize = CellSize*2
            CellHalf = CellSize / 2 

            dataEnvironment.ProgressLog.AddSafe("Position", "Centering Fit") 
            #Center Cells calculate the center of the cells, with a little smoothing to get it nice
            Filter =   Filters.CenterCellsTool2Form() 
            #Parameters required: Bitmap_Filenames as string[], X_Positions as int[], Y_Positions as int[], SmoothingTypeX as string, X_Smooth_Param as int, SmootingTypeY as string, Y_Smooth_Param as int, ShowForm as string, CutSize as Size, OptionalOutputDir as string
            BitmapImage = Filter.DoEffect2(dataEnvironment, BitmapImage, GlobalPassData, 100, X_PositionsB2, Y_PositionsB, "MovingAverage", 5, "MovingAverage", 5, False,  Size(CellSize, CellSize),  True, TempPath) 
            #Data out of type :
            GlobalPassData = Filter.PassData 

            #write out the output
            try:
                dataEnvironment.ProgressLog.AddSafe("Centering", "Centering Line Created") 
                dataEnvironment.ProgressLog.AddSafe("CenteringQuality", GlobalPassData["CenterAccuracy"].ToString() + "%") 
                dataEnvironment.ProgressLog.AddSafe("CenteringQualityActual", GlobalPassData["CenterAccuracyActual"].ToString() + "%") 
            except ValueError:    
                pass
             
            #get the smoothed cellpositions
            global CellPosX
            global CellPosY
            CellPosX = GlobalPassData["CorrectedX"] 
            CellPosY = GlobalPassData["CorrectedY"] 

            #and finally cut out the cells
            dataEnvironment.ProgressLog.AddSafe("Position", "Clipping") 
            OutFileList = BatchLoopThroughImagesSave(CutOutCell, dataEnvironment) 
            dataEnvironment.ProgressLog.AddSafe("Clipping", "Images Clipped") 

def BatchProcessRecon(dataEnvironment):
            global GlobalPassData
            dataEnvironment.ProgressLog.AddSafe("Prep", "Recon") 
            dataEnvironment.ProgressLog.AddSafe("Position", "Getting Impulse") 
            
            impulse = Filtering.GetRealSpaceFilter(ScriptParams["FBPWindow"], Filters.EffectHelps.ConvertToInt(ScriptParams["FBPResolution"]), Filters.EffectHelps.ConvertToInt(ScriptParams["FBPResolution"]), 1) 
            
            dataEnvironment.ProgressLog.AddSafe("Position", "Doing Convolution") 
            DesiredMethod = ConvolutionMethod.Convolution1D 
            ConvolutionFilter =   PythonScripting.Projection.Convolution1D() 

            global DensityGrid
            DensityGrid=CreateDensityGrid(dataEnvironment, ConvolutionFilter, impulse, dataEnvironment.AllImages[0])

            #BatchLoopThroughImages(DoSARTProjection, dataEnvironment) 
            BatchLoopThroughImages(DoFBProjection, dataEnvironment) 
            dataEnvironment.ProgressLog.AddSafe("Recon", "Recon Completed") 

            if (dataEnvironment.RunningOnGPU == True and DensityGrid.DataWhole != None):
                    DensityGrid.DataWhole =  PythonScripting.Projection.DoSliceBackProjectionEffect.ReadReconVolume(dataEnvironment) 

            ConvolutionFilter.Dispose() 
            PythonScripting.Projection.DoSliceBackProjectionEffect.Dispose(dataEnvironment)

#this is called once after the whole set has been processed
def SaveEverything( dataEnvironment):
            global GlobalPassData
            global DensityGrid
            dataEnvironment.ProgressLog.AddSafe("Prep", "Post") 
            #/the data is still on the card if we have been using the gpu.  Pull the data down and then save it
            
               
            PythonHelps.Save3DVolume(DataSourcePath,DensityGrid, ScriptParams)

            SaveExampleImages(dataEnvironment, DataSourcePath, DensityGrid)
            
            dataEnvironment.ProgressLog.AddSafe("ImageType", IO.Path.GetExtension(dataEnvironment.WholeFileList[0])) 

            if (ScriptParams["SaveMIP"].ToLower() == "true"):
                PythonHelps.SaveMIPMovie(DataSourcePath, TempPath, DensityGrid, dataEnvironment)

            if (ScriptParams["SaveCenteringMovie"].ToLower() == "true"):
                PythonHelps.SaveCenteringMovie(DataSourcePath, TempPath, DensityGrid, dataEnvironment)

            PythonHelps.GetExampleVisionGateImages("Y:",dataEnvironment)

            try:
               #todo: cut down the stack based by the imagesize from reconquality check tool.  just need a nice tool
               if (ScriptParams["CopyStack"].ToLower() == "True"):
                   ProjectionFilters.CopyAndCutStackEffect.CopyStack(dataEnvironment.ExperimentFolder + "\\stack\\000", dataEnvironment.ExperimentFolder + "\\stack", True, dataEnvironment.ProgressLog["IsColor"].ToString().ToLower() == "True") 
            except ValueError:    
               pass   

           #do the image quality tests
            if (ScriptParams["DoConvolutionQuality"].ToLower() == "true"):
               try:
                  dataEnvironment.ProgressLog.AddSafe("Position", "ImageQuality") 
                  ProjectionFilters.ReconQualityCheckTool.CompareProjection(DensityGrid.DataWhole, dataEnvironment.ExperimentFolder, GlobalPassData) 
               except ValueError:    
                  pass       
                 
            for kvp in GlobalPassData:
               dataEnvironment.ProgressLog.AddSafe(kvp.Key, kvp.Value.ToString()) 

             
X_PositionsB2 = Array.CreateInstance(float,dataEnvironment.AllImages.Count)
Y_PositionsB = Array.CreateInstance(float,dataEnvironment.AllImages.Count)
NumBlobs= Array.CreateInstance(float,dataEnvironment.AllImages.Count)

BatchLoopThroughImagesSave(ImageLoadAndFind, dataEnvironment) 

BatchProcessBackgroundDivide(dataEnvironment) 

BatchProcessCenter(dataEnvironment) 

#do any pre convolution work.  This is where most of the changes should be located
BatchLoopThroughImagesSave(ProcessBeforeConvolution, dataEnvironment) 

BatchProcessRecon(dataEnvironment) 
SaveEverything(dataEnvironment) 
         

       
     
 

