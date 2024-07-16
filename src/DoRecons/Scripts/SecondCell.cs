using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ImageViewer;
using ImageViewer.PythonScripting;
using ImageViewer.Filters;
using ImageViewer.Filters.Blobs;
using System.Threading;
using MathHelpLib;
using System.IO;
using MathHelpLib.ProjectionFilters;
using ImageViewer.PythonScripting.Threads;
using Tomographic_Imaging_2;
using DoRecons.Scripts;

namespace DoRecons
{
    public class SecondCell : BaseScript
    {

        private string OriginalDataSetPath = "";

        protected override ImageHolder ProcessImageLoad(DataEnvironment dataEnvironment, int ImageNumber)
        {
            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];

            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            PassData.AddSafe("Num Loaded Channels", BitmapImage.NChannels);

            //make sure there is only one channel to work with
            if (BitmapImage.NChannels > 1)
            {
                Filter = new ImageViewer.Filters.Adjustments.GrayScaleEffectChannel();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, 1);
            }

            if (ColorImage == true)
            {
                BitmapImage = MathHelpsFileLoader.FixVisionGateImage(BitmapImage, 2);
            }

           /* if (FluorImage == true)
            {
                //Invert Contrast
                Filter = new ImageViewer.Filters.Adjustments.InvertEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            }*/

            if (FluorImage == false)
            {
                // try
                {
                    //divide off the background curvature
                    if (ScriptParams["GlobalFlatten"].ToLower() == "true")
                    {
                        Filter = new ImageViewer.Filters.Effects.Flattening.FlattenEdges1DErrorCorrected();
                        BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                    }
                }
                // catch { }
            }
            return BitmapImage;
        }

        protected override ImageHolder ProcessBeforeConvolution(DataEnvironment dataEnvironment, int ImageNumber)
        {
             ImageHolder BitmapImage = base.ProcessBeforeConvolution(dataEnvironment, ImageNumber);

             if (FluorImage)
             {
                   ReplaceStringDictionary PassData = GlobalPassData;
                   IEffect Filter;

                   //Invert Contrast
                   Filter = new ImageViewer.Filters.Adjustments.InvertEffect();
                   BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

                   dataEnvironment.AllImages[ImageNumber] = BitmapImage;
             }

             return BitmapImage;
        }

        protected override void PreBatchProcessBackgroundDivide(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Divide");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ImageViewer.Filters.ReplaceStringDictionary PassData = GlobalPassData;

            IEffect Filter;
            #region Create Arrays
            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB", OriginalDataSetPath + "\\data\\X_PositionsB");

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_PositionsB", OriginalDataSetPath + "\\data\\Y_PositionsB");

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "NumBlobs", DataPath + "NumBlobs", dataEnvironment.AllImages.Count);


            Filter = new ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "CellSize", OriginalDataSetPath + "\\data\\CellSize");

            //Read Whole Array
            Filter = new ImageViewer.PythonScripting.Arrays.ReadWholeArrayTool();
            //Parameters required: ArrayName as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "CellSize");
            //Data out of type :
            PassData = Filter.PassData;

            double[] CellSizes  = (double[])PassData["WholeArray"];

            tCellSize =(int) CellSizes[ 0];
            #endregion

            ImageViewer.Filters.CenterCellsTool2Form.PreFitLines(dataEnvironment, "X_PositionsB", "Y_PositionsB", ThreadNumber);
            dataEnvironment.ProgressLog.AddSafe("Position", "starting stationary pixels");
            try
            {
                Filter = new ImageViewer.Filters.StationaryPixelsForm(true);
                //Parameters required: BitmapFiles as string[],  as , X_Positions as double[], Y_Positions as double[], ShowForm as bool, SubtractMethod as string
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData,
                    100, ScriptParams["BackgroundSubMethod"], "X_PositionsB", "Y_PositionsB", false, "Divide", ThreadNumber, tCellSize, true, ImageFileList);
            }
            catch (Exception ex)
            {
                //fluor images just dont divide clean
                if (FluorImage == false)
                {
                    throw ex;
                }
                else
                {
                }
            }

            //Data out of type :
            PassData = Filter.PassData;

            dataEnvironment.ProgressLog.AddSafe("Position", "saving stationary pixels");
            #region Save background info
            if ((dataEnvironment.NumberOfRunningThreads > 1 && ThreadNumber == 1) || (dataEnvironment.NumberOfRunningThreads <= 1 && ThreadNumber == 0))
            {
                MathHelpsFileLoader.Save_Bitmap(DataPath + "Background.bmp", BitmapImage);

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB", DataPath + "X_PositionsB");

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_PositionsB", DataPath + "Y_PositionsB");

                if (FluorImage == false || FluorImage == null)
                    MathHelpsFileLoader.Save_Raw(DataPath + "Background.cct", (double[,])PassData["BackgroundMask"]);

                //Read Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.ReadWholeArrayTool();
                //Parameters required: ArrayName as string
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "NumBlobs");
                //Data out of type :
                PassData = Filter.PassData;

                double[] nBlobs = (double[])PassData["WholeArray"];
                double maxBlobs = max(nBlobs);
                dataEnvironment.ProgressLog.AddSafe("NumberOfCells", maxBlobs.ToString());
            }
            #endregion

            CellSize = (int)((int)PassData["MaxCellSize"]);
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

            #region Create More Arrays
            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FGIntensityNoDivide", DataPath + "FGIntensityNoDivide", dataEnvironment.AllImages.Count);

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FGIntensityDivide", DataPath + "FGIntensityDivide", dataEnvironment.AllImages.Count);

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "BGIntensityAverage", DataPath + "BGIntensityAverage", dataEnvironment.AllImages.Count);

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "BGIntensitySD", DataPath + "BGIntensitySD", dataEnvironment.AllImages.Count);
            #endregion

            dataEnvironment.ProgressLog.AddSafe("Position", "Dividing away background");
            //now do the divide
            BatchLoopThroughImagesSave(2, dataEnvironment, ImageFileListIn, ScriptParams);
            // BatchLoopThroughImages(2, dataEnvironment, ImageFileListIn, ScriptParams );

            // Console.WriteLine("RoughMax " + dataEnvironment.AllImages[0].Max());
            //  Console.WriteLine("Rough Width : " + dataEnvironment.AllImages[0].Width.ToString());

            dataEnvironment.ProgressLog.AddSafe("Divide", "Divide Succeeded");

        }

        protected override void PreBatchProcessCenter(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Center");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;

            #region Create Arrays
            //Create Global Array
            IEffect Filter = new ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_Positions", OriginalDataSetPath + "\\data\\X_Positions");

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_Positions", OriginalDataSetPath + "\\data\\Y_Positions");
            #endregion

            if (FluorImage == true)
            {
                CellSize = (int)(CellSize * 1.2);
            }
            else
            {
                CellSize = (int)(CellSize * 1.8);
            }

            CellHalf = CellSize / 2;
            // print("Cell Size" + CellSize.ToString());

            dataEnvironment.ProgressLog.AddSafe("Position", "Centering Fit");
            //Center Cells
            Filter = new ImageViewer.Filters.CenterCellsTool2Form();
            //Parameters required: Bitmap_Filenames as string[], X_Positions as int[], Y_Positions as int[], SmoothingTypeX as string, X_Smooth_Param as int, SmootingTypeY as string, Y_Smooth_Param as int, ShowForm as string, CutSize as Size, OptionalOutputDir as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, 100, "X_Positions", "Y_Positions", "MovingAverage", 5, "MovingAverage", 5, false, new Size(CellSize, CellSize), ThreadNumber, true, TempPath, ImageFileList);
            //Data out of type :
            PassData = Filter.PassData;


            dataEnvironment.ProgressLog.AddSafe("Centering", "Centering Line Created");
            try
            {
                dataEnvironment.ProgressLog.AddSafe("CenteringQualityActual", PassData["CenterAccuracyActual"].ToString() + "%");
            }
            catch { }
            dataEnvironment.ProgressLog.AddSafe("CenteringQuality", PassData["CenterAccuracy"].ToString() + "%");

            #region SaveCentering Info
            if ((dataEnvironment.NumberOfRunningThreads > 1 && ThreadNumber == 1) || (dataEnvironment.NumberOfRunningThreads <= 1 && ThreadNumber == 0))
            {
                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_Positions", DataPath + "X_Positions");

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_Positions", DataPath + "Y_Positions");
            }
            #endregion

            CellPosX = (double[])PassData["CorrectedX"];
            CellPosY = (double[])PassData["CorrectedY"];

            #region Create even more arrays
            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValue", DataPath + "FocusValue", dataEnvironment.AllImages.Count);

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValueMax", DataPath + "FocusValueMax", dataEnvironment.AllImages.Count);

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValueMedian", DataPath + "FocusValueMedian", dataEnvironment.AllImages.Count);

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValueMostProb", DataPath + "FocusValueMostProb", dataEnvironment.AllImages.Count);
            #endregion

            dataEnvironment.ProgressLog.AddSafe("Position", "Clipping");
            List<ImageFile> OutFileList = BatchLoopThroughImagesSave(3, dataEnvironment, ImageFileListIn, ScriptParams);

            dataEnvironment.ProgressLog.AddSafe("Clipping", "Images Clipped");

        }

        protected override void SaveMIP()
        {
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;
           

            if (ScriptParams["SaveMIP"].ToLower() == "true")
            {
                try
                {
                    double[,,] OrigData = MathHelpLib.MathHelpsFileLoader.OpenDensityData(OriginalDataSetPath + "\\data\\projectionobject.cct");

                    BitmapImage = ImageViewer.PythonScripting.Projection.MakeMIPMovie3Effect.MakeFalseColorMovie(OrigData, DensityGrid.DataWhole, Color.Red, Color.Blue, TempPath, DataPath + "MIP.AVI");
                   
                    BitmapImage.Save(DataPath + "Forward1.bmp");
                }
                catch(Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        protected override void DoRun(Dictionary<string, object> Variables)
        {

            OriginalDataSetPath = (string)Variables["SecondCellFolder"];

            if ((Variables.ContainsKey("Rehydrate") != true || (string)Variables["Rehydrate"] == "False"))
            {
                PreBatchProcessBackgroundDivide(dataEnvironment, ImageFileListIn, ScriptParams);
                PreBatchProcessCenter(dataEnvironment, ImageFileListIn, ScriptParams);
            }

            //do any pre convolution work.  This is where most of the changes should be located
            BatchLoopThroughImagesSave(7, dataEnvironment, ImageFileListIn, ScriptParams);

            PreBatchProcessRecon(dataEnvironment, ImageFileListIn, ScriptParams);
            PostBatchProcess(dataEnvironment, ImageFileListIn, null, ScriptParams);
        }

        public override IScript CloneScript()
        {
            return new SecondCell();
        }
    }
}

