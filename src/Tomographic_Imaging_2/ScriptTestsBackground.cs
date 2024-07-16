using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ImageViewer;
using ImageViewer.PythonScripting;
using ImageViewer.Filters;
using ImageViewer.Filters.Blobs;

namespace Tomographic_Imaging_2
{
    public class ScriptTestsBackground
    {
        private double[,] BackgroundMask;
        private Rectangle CellWanderArea;
        private int CellWanderMargin = 120;
        private double[] XData;
        private double[] YData;
        private int CellSize = 170;
        private int tCellSize = 0;
        private int CellHalf = 85;
        private double[,] AverageIllumination;
        private ImageViewer.Filters.ReplaceStringDictionary GlobalPassData;
        private string DataPath;
        private ImageDisplayer ImageDisp;


        List<ImageFile> ImageFileListIn;
        Dictionary<string, object> ScriptParams;
        string ImageOutPath;
        string ImageOutExten;
        string ImageOutFileName;
        string TempPath;
        string Executable;
        string ExecutablePath;
        string LibraryPath;
        DataEnvironment dataEnvironment;
        List<ImageHolder> AllImages;
        bool UseGlobalImageList;
        bool RunningThreaded;
        int ThisThreadId;

        int MasterThreadID;

        private void print(object message)
        {
            Console.WriteLine(message.ToString());
        }
        private int len(Array array)
        {
            return array.Length;
        }
        private int len(System.Collections.IList array)
        {
            return array.Count;
        }
        private double min(double[] array)
        {
            return array.Min();
        }
        private double max(double[] array)
        {
            return array.Max();
        }

        private ImageHolder ProcessImageFind(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ImageViewer.Filters.ReplaceStringDictionary PassData = GlobalPassData;

            //Down Sample Image
            IEffect Filter = new ImageViewer.Filters.Adjustments.downSampleEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

            //Invert Contrast
            //Filter =Filters.Adjustments.InvertEffect()
            //BitmapImage=Filter.DoEffect(dataEnvironment,BitmapImage, PassData)

            //SIS Threshold
            //Filter =Filters.Thresholding.SISThresholdEffect()
            //BitmapImage=Filter.DoEffect(dataEnvironment,BitmapImage, PassData)

            //Otsu Threshold
            //Filter =Filters.Thresholding.OtsuThresholdEffect()
            //BitmapImage=Filter.DoEffect(dataEnvironment,BitmapImage, PassData)

            //Iterative Threshold
            Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

            //WaterShed
            Filter = new ImageViewer.Filters.Blobs.WaterShedTool();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            //Data out of type :
            PassData = Filter.PassData;

            //Get Biggest Blob
            Filter = new ImageViewer.Filters.Blobs.GetBiggestBlob();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, PassData["Blobs"], true);
            //Data out of type :
            PassData = Filter.PassData;

            BlobDescription Rect = (BlobDescription)PassData["MaxBlob"];
            int x = Rect.CenterOfGravity.X ;
            int y = Rect.CenterOfGravity.Y ;
            //print((Rect.BlobBounds.Width*2).ToString() +"," + (2*Rect.BlobBounds.Height).ToString())
            print(x.ToString() + "," + y.ToString());

            tCellSize = tCellSize + Rect.BlobBounds.Width ;
            tCellSize = tCellSize + Rect.BlobBounds.Height ;

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB", ImageNumber, x);

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_PositionsB", ImageNumber, y);


            return BitmapImage;
        }
        private ImageHolder ProcessImageClip(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ImageViewer.Filters.ReplaceStringDictionary PassData = GlobalPassData;

            Rectangle CellArea = new Rectangle((int)(XData[ImageNumber] - CellHalf), (int)(YData[ImageNumber] - CellHalf), CellSize, CellSize);

            //Average Intensity of Image
            IEffect Filter = new ImageViewer.PythonScripting.AverageImage.AverageIntensityPointTool();
            //Parameters required: AverageArray as string, CompareImage or Bounds as Image or Rectange, CompareImage as Image
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, CellArea, BitmapImage);
            PassData = Filter.PassData;

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FGIntensityNoDivide", ImageNumber, PassData["AverageIntensity"]);

            //Divide Image
            Filter = new ImageViewer.Filters.Effects.Flattening.DivideImage();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, BackgroundMask);


            //Average Intensity of Image
            Filter = new ImageViewer.PythonScripting.AverageImage.AverageIntensityPointTool();
            //Parameters required: AverageArray as string, CompareImage or Bounds as Image or Rectange, CompareImage as Image
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, CellArea, BitmapImage);
            PassData = Filter.PassData;

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FGIntensityDivide", ImageNumber, PassData["AverageIntensity"]);

            //Invert Contrast
            //Filter =Filters.Adjustments.InvertEffect()
            //BitmapImage=Filter.DoEffect(dataEnvironment,BitmapImage, PassData)

            //Add Average Point
            Filter = new ImageViewer.PythonScripting.AverageImage.AddPointAverageTool();
            //Parameters required: ArrayName as string, ArrayName as string, 
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "OffIllumination", ImageNumber, new Rectangle(0, 0, CellWanderArea.X , CellWanderArea.Height), BitmapImage);

            //Clip Image to New Image
            
            Filter = new ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect();
            //Parameters required: Clip Bounds as Rectangle
            //BitmapImage=Filter.DoEffect(dataEnvironment,BitmapImage, PassData, CellArea)
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, CellWanderArea);

            return BitmapImage;
        }

        //this is called once before all the images are looped through
        //it is good for opening array lists and log files
        private void PreBatchProcess(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary)
        {
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ImageViewer.Filters.ReplaceStringDictionary PassData = GlobalPassData;

            IEffect Filter;

            bool SkipBackground = false;
            if (SkipBackground == true)
            {
                //Read Text Array from File
                Filter = new ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool();
                //Parameters required: ArrayName as string, ArrayFileName as string
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB", "X_PositionsB");
                //Data out of type :
                PassData = Filter.PassData;

                //Read Text Array from File
                Filter = new ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool();
                //Parameters required: ArrayName as string, ArrayFileName as string
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_PositionsB", "Y_PositionsB");
                //Data out of type :
                PassData = Filter.PassData;
            }
            else
            {
                //Create Global Array
                Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
                //Parameters required: Array_Name as string, Array_Filename as string
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB", DataPath + "X_PositionsB");

                //Create Global Array
                Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
                //Parameters required: Array_Name as string, Array_Filename as string
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_PositionsB", DataPath + "Y_PositionsB");

                BatchLoopThroughImages(0, dataEnvironment, ImageFileList, ParamDictionary);
                print(tCellSize.ToString());
                print(len(ImageFileList).ToString());
                tCellSize = (tCellSize / len(ImageFileList)) / 2;
            }

            //Stationary Pixels
            Filter = new ImageViewer.Filters.StationaryPixelsForm();
            //Parameters required: BitmapFiles as string[],  as , X_Positions as double[], Y_Positions as double[], ShowForm as bool, SubtractMethod as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ImageFileList, 100, "TopAndBottom", "X_PositionsB", "Y_PositionsB", false, "Divide", RunningThreaded, ThisThreadId, tCellSize);
            //Data out of type :
            PassData = Filter.PassData;

            if (MasterThreadID == ThisThreadId)
            {
                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB", DataPath + "X_PositionsB");

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_PositionsB", DataPath + "Y_PositionsB");

                ImagingTools.Save_Bitmap(DataPath + "Background.bmp", BitmapImage);
                ImagingTools.Save_Raw(DataPath + "Background.raw", (double[,])PassData["BackgroundMask"]);
            }

            CellSize = (int)PassData["MaxCellSize"];
            CellHalf = CellSize / 2;
            CellWanderMargin = CellSize;
            print("Cell Size" + CellSize.ToString());

            BackgroundMask = (double[,])PassData["BackgroundMask"];

            //Read Whole Array
            Filter = new ImageViewer.PythonScripting.Arrays.ReadWholeArrayTool();
            //Parameters required: ArrayName as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB");
            //Data out of type :
            PassData = Filter.PassData;

            XData = (double[])PassData["WholeArray"];

            //Read Whole Array
            Filter = new ImageViewer.PythonScripting.Arrays.ReadWholeArrayTool();
            //Parameters required: ArrayName as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_PositionsB");
            //Data out of type :
            PassData = Filter.PassData;

            YData = (double[])PassData["WholeArray"];

            int XMin = (int)(min(XData) - CellWanderMargin);
            int YMin = (int)(min(YData) - CellWanderMargin);
            int XMax = (int)(max(XData) + CellWanderMargin);
            int YMax = (int)(max(YData) + CellWanderMargin);

            CellWanderArea = new Rectangle(XMin, YMin, XMax - XMin, YMax - YMin);

            print(CellWanderArea);
            //Create Global Average
            Filter = new ImageViewer.PythonScripting.AverageImage.CreateGlobalAverageTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "OffIllumination", "", null);

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FGIntensityNoDivide", DataPath + "FGIntensityNoDivide");

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FGIntensityDivide", DataPath + "FGIntensityDivide");
        }

        private ImageHolder ProcessImageClip2(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;

            //Divide Image
            IEffect Filter = new ImageViewer.Filters.Effects.Flattening.DivideImage();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, BackgroundMask);


            //Average Intensity of Image
            Filter = new ImageViewer.PythonScripting.AverageImage.AverageIntensityPointTool();
            //Parameters required: AverageArray as string, CompareImage or Bounds as Image or Rectange, CompareImage as Image
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, new Rectangle(0, 0, CellWanderArea.X, CellWanderArea.Height), BitmapImage);
            PassData = Filter.PassData;

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "BGIntensityAverage", ImageNumber, PassData["AverageIntensity"]);

            //Standard Deviation of Image
            Filter = new ImageViewer.PythonScripting.AverageImage.StandardDeviationPointAverageTool();
            //Parameters required: AverageArray as string, CompareImage or Bounds as Image or Rectange, CompareImage as Image
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, AverageIllumination, new Rectangle(0, 0, CellWanderArea.X, CellWanderArea.Height), BitmapImage);
            PassData = Filter.PassData;
            //print(ImageNumber.ToString() + "," + PassData["StdDev"].ToString())

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "BGIntensitySD", ImageNumber, PassData["StdDev"]);

            return BitmapImage;
        }

        //this is called once after the whole set has been processed
        private void PostBatchProcess(DataEnvironment dataEnvironment, List<ImageFile> FirstImageFileList, List<ImageFile> OutFileList, Dictionary<string, object> ParamDictionary)
        {
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;

            //Join Threads
            IEffect Filter = new ImageViewer.PythonScripting.Threads.JoinThreadsTool();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ThisThreadId);

            if (MasterThreadID == ThisThreadId)
            {
                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FGIntensityNoDivide", DataPath + "FGIntensityNoDivide");

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FGIntensityDivide", DataPath + "FGIntensityDivide");
            }
            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "BGIntensityAverage", DataPath + "BGIntensityAverage");

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "BGIntensitySD", DataPath + "BGIntensitySD");

            //Read Whole Array
            Filter = new ImageViewer.PythonScripting.AverageImage.ReadWholeAverageTool();
            //Parameters required: ArrayName as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "OffIllumination");
            //Data out of type :Returns array double[,]|| PassData['WholeAverageArray']
            PassData = Filter.PassData;

            AverageIllumination = (double[,])PassData["WholeAverageArray"];

            BatchLoopThroughImages(2, dataEnvironment, FirstImageFileList, ParamDictionary);

            //Join Threads
            Filter = new ImageViewer.PythonScripting.Threads.JoinThreadsTool();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ThisThreadId);

            if (MasterThreadID == ThisThreadId)
            {
                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "BGIntensityAverage", DataPath + "BGIntensityAverage");

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "BGIntensitySD", DataPath + "BGIntensitySD");

            }

        }

        private  List<ImageFile> BatchLoopThroughImagesSave(int IProcessFunction, DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary, string OutDirectory, string OutFilename, string OutExtension)
        {
            List<ImageFile> FileListOut = new List<ImageFile>();
            int imageIndex;
            for (int CurrentParticle = 0; CurrentParticle < ImageFileList.Count; CurrentParticle++)
            {
                //print(ImageFileList[CurrentParticle])
                imageIndex = ImageFileList[CurrentParticle].Index;
                ImageHolder image;
                if (UseGlobalImageList == false)
                    image = ImagingTools.Load_Bitmap(ImageFileList[CurrentParticle].Filename);
                else
                    image = AllImages[imageIndex ];

                print(image);
                if (IProcessFunction == 0)
                    image = ProcessImageFind(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 1)
                    image = ProcessImageClip(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 2)
                    image = ProcessImageClip2(dataEnvironment, imageIndex, image);

                ImageViewer.PythonScripting.MacroHelper.DoEvents();

                string fileout = OutDirectory + OutFilename + String.Format("{0:000}", imageIndex) + OutExtension;
                //print(fileout)
                FileListOut.Add(new ImageFile(imageIndex, fileout));// .append(fileout);

                if (UseGlobalImageList == false)
                    ImagingTools.Save_Bitmap(fileout, image);
                else
                {
                    AllImages.RemoveAt(CurrentParticle);
                    AllImages.Insert(CurrentParticle, image);
                }
                ImageDisp.DisplayImage(imageIndex, image);
                ImageViewer.PythonScripting.MacroHelper.DoEvents();
            }
            return FileListOut;
        }

        private void BatchLoopThroughImages(int IProcessFunction, DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary)
        {
            int imageIndex;
            for (int CurrentParticle = 0; CurrentParticle < ImageFileList.Count; CurrentParticle++)
            {
                //print(ImageFileList[CurrentParticle])
                imageIndex = ImageFileList[CurrentParticle].Index;
                ImageHolder image;
                if (UseGlobalImageList == false)
                    image = ImagingTools.Load_Bitmap(ImageFileList[CurrentParticle].Filename);
                else
                    image = AllImages[imageIndex];

                if (IProcessFunction == 0)
                    image = ProcessImageFind(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 1)
                    image = ProcessImageClip(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 2)
                    image = ProcessImageClip2(dataEnvironment, imageIndex, image);

                ImageDisp.DisplayImage(imageIndex, image);
                ImageViewer.PythonScripting.MacroHelper.DoEvents();
            }
        }

        public void RemoveBackground(Dictionary<string, object> Variables)
        {
            ImageFileListIn =(List<ImageFile>) Variables["ImageFileListIn"];
            ScriptParams =(Dictionary<string,object>) Variables["ScriptParams"];
            ImageOutPath =(string) Variables["ImageOutPath"];
            ImageOutExten = (string)Variables["ImageOutExten"];
            ImageOutFileName = (string)Variables["ImageOutFileName"];
            TempPath = (string)Variables["TempPath"];
            ImageDisp =(ImageDisplayer) Variables["ImageDisp"];
            DataPath = (string)Variables["DataPath"];
            Executable = (string)Variables["Executable"];
            ExecutablePath = (string)Variables["ExecutablePath"];
            LibraryPath = (string)Variables["LibraryPath"];
            dataEnvironment =(DataEnvironment ) Variables["dataEnvironment"];
            GlobalPassData =(ReplaceStringDictionary) Variables["GlobalPassData"];

            RunningThreaded =(bool) Variables["RunningThreaded"];
            ThisThreadId =(int) Variables["ThisThreadId"];
            MasterThreadID =(int) Variables["MasterThreadID"];

            AllImages = (List<ImageHolder>)Variables["AllImages"];
            UseGlobalImageList = (bool)Variables["UseGlobalImageList"];

            PreBatchProcess(dataEnvironment, ImageFileListIn, ScriptParams);

            List<ImageFile> OutFileList = BatchLoopThroughImagesSave(1, dataEnvironment, ImageFileListIn, ScriptParams, ImageOutPath, ImageOutFileName, ImageOutExten);

            PostBatchProcess(dataEnvironment, ImageFileListIn, OutFileList, ScriptParams);
        }
    }
}
