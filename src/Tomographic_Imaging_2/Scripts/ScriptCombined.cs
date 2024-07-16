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


namespace Tomographic_Imaging_2
{
    public class ScriptCombined : IScript
    {
        public string GetName()
        {
            return "ScriptCombined";
        }

        private double[,] BackgroundMask;
        private Rectangle CellWanderArea;
        private int CellWanderMargin = 120;
        private double[] XData;
        private double[] YData;
        private int CellSize = 170;
        private int tCellSize = 0;
        private int CellHalf = 85;
        //private double[,] AverageIllumination;
        private ImageViewer.Filters.ReplaceStringDictionary GlobalPassData;
        private double[] CellPosX;
        private double[] CellPosY;

        private bool? FluorImage = null;


        #region global values

        private string DataPath;
        private ImageDisplayer ImageDisp;
        List<ImageFile> ImageFileListIn;
        Dictionary<string, object> ScriptParams;
        // string ImageOutPath;
        // string ImageOutExten;
        // string ImageOutFileName;
        string TempPath;
        string Executable;
        string ExecutablePath;
        string LibraryPath;
        DataEnvironment dataEnvironment;

        bool RunningThreaded;
        int ThreadNumber;

        #endregion

        #region pythonsimulation
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
        private double Average(double[] array)
        {

            return array.Average();
        }
        private double Stdev(double[] array)
        {
            double ave = array.Average();
            double sum = 0;
            double d = 0;
            for (int i = 0; i < array.Length; i++)
            {
                d = array[i] - ave;
                sum += d * d;
            }
            sum = Math.Sqrt(sum / array.Length);
            return sum;
        }
        #endregion

        private ImageHolder ProcessImageFindRough(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ImageViewer.Filters.ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            if (FluorImage == null)
            {
                //Iterative Threshold
                Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdEffect();
                ImageHolder BitmapImage2 = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

                Filter = new ImageViewer.PythonScripting.Statistics.FastBlackWhiteRatioTool();
                Filter.DoEffect(dataEnvironment, BitmapImage, PassData, BitmapImage2);
                double Percent = (double)Filter.PassData["PercentBlack"];
                FluorImage = (Percent > .2);
            }

            int Expander = 4;

            if (FluorImage.Value == true)
            {
                //Invert Contrast
                Filter = new ImageViewer.Filters.Adjustments.InvertEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                Expander = 2;
            }

            if (Expander == 2)
            {
                //reduce size
                Filter = new ImageViewer.Filters.Adjustments.downSampleEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            }
            else if (Expander == 4)
            {
                //reduce size
                Filter = new ImageViewer.Filters.Adjustments.downSampleEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

                //reduce size
                Filter = new ImageViewer.Filters.Adjustments.downSampleEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            }


            //SIS Threshold
            //Filter =new  ImageViewer.Filters.Thresholding.SISThresholdEffect();
            //BitmapImage =(ImageHolder ) Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            if (FluorImage.Value == true)
            {
                //Otsu Threshold
                Filter = new ImageViewer.Filters.Thresholding.OtsuThresholdEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            }
            else
            {
                //Iterative Threshold
                Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            }

            //WaterShed
            Filter = new ImageViewer.Filters.Blobs.WaterShedTool();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            //Data out of type :
            PassData = Filter.PassData;

            //Get Biggest Blob
            Filter = new ImageViewer.Filters.Blobs.GetBiggestCenterBlob();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, PassData["Blobs"], true);
            //Data out of type :
            PassData = Filter.PassData;

            BlobDescription Rect = (BlobDescription)PassData["MaxBlob"];
            int x = Rect.CenterOfGravity.X * Expander; ;
            int y = Rect.CenterOfGravity.Y * Expander;
            //print((Rect.BlobBounds.Width*2).ToString() +"," + (2*Rect.BlobBounds.Height).ToString())
            print(x.ToString() + "," + y.ToString());

            tCellSize = tCellSize + Rect.BlobBounds.Width * Expander;
            tCellSize = tCellSize + Rect.BlobBounds.Height * Expander;

            try
            {
                //Add Array Point
                Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
                //Parameters required: ArrayName as string, datapoint as double
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB", ImageNumber, x);
            }
            catch
            {
                System.Diagnostics.Debug.Print("Error adding");
            }

            try
            {
                //Add Array Point
                Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
                //Parameters required: ArrayName as string, datapoint as double
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_PositionsB", ImageNumber, y);
            }
            catch
            {
                System.Diagnostics.Debug.Print("");
            }

            try
            {
                //Add Array Point
                Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
                //Parameters required: ArrayName as string, datapoint as double
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "NumBlobs", ImageNumber, PassData["NumBigBlobs"]);
            }
            catch
            {
                System.Diagnostics.Debug.Print("");
            }

            return BitmapImage;
        }

        private ImageHolder ProcessImageFindFine(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            int Xo = (int)Math.Truncate(XData[ImageNumber] - CellHalf);
            int Yo = (int)Math.Truncate(YData[ImageNumber] - CellHalf);

            Rectangle CellArea = new Rectangle(Xo, Yo, CellSize, CellSize);

            CellArea.Inflate(20, 20);
            Xo = CellArea.X;
            Yo = CellArea.Y;

            //Clip Image to New Image
            Filter = new ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect();
            //Parameters required: Clip Bounds as Rectangle
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, CellArea);

            double AveX = 0;
            double AveY = 0;

            ImageHolder OriginalTests = BitmapImage;

            BlobDescription Rect = null;
            int NumTestRuns = 4;
            
            for (int i = 0; i < NumTestRuns ; i++)
            {
                BitmapImage = OriginalTests.Copy();
                //Jitter
                Filter = new ImageViewer.Filters.Effects.Artistic.JitterEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

                //Iterative Threshold
                Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

                //Center Of Gravity
                Filter = new ImageViewer.Filters.Blobs.CenterOfGravityTool();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                //Data out of type :
                PassData = Filter.PassData;

                //Get Biggest Blob
                Filter = new ImageViewer.Filters.Blobs.GetBiggestBlob();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, PassData["Blobs"], true);
                //Data out of type :
                PassData = Filter.PassData;

                Rect = (BlobDescription)PassData["MaxBlob"];

                int x = Rect.CenterOfGravity.X;
                int y = Rect.CenterOfGravity.Y;

                AveX += x;
                AveY += y;
            }

            //figure out the average and offset the data after the clip
            AveX = Xo + AveX / (double)NumTestRuns;
            AveY = Yo + AveY / (double)NumTestRuns;

            tCellSize = tCellSize + Rect.BlobBounds.Width;
            tCellSize = tCellSize + Rect.BlobBounds.Height;

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_Positions", ImageNumber, AveX);

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_Positions", ImageNumber, AveY);

            return BitmapImage;
        }

        private ImageHolder ProcessImageDivide(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            try
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

                if (FluorImage == false)
                {
                    Rectangle CellAreaPadded =new Rectangle( CellArea.Location ,CellArea.Size);
                    CellAreaPadded.Inflate(CellArea.Width/2 , CellArea.Height/2 );

                    //Divide Image
                    Filter = new ImageViewer.Filters.Effects.Flattening.DivideImage();
                    BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, BackgroundMask,CellAreaPadded );
                }

                //Average Intensity of Image
                Filter = new ImageViewer.PythonScripting.AverageImage.AverageIntensityPointTool();
                //Parameters required: AverageArray as string, CompareImage or Bounds as Image or Rectange, CompareImage as Image
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, CellArea, BitmapImage);
                PassData = Filter.PassData;

                //Add Array Point
                Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
                //Parameters required: ArrayName as string, datapoint as double
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FGIntensityDivide", ImageNumber, PassData["AverageIntensity"]);

                //now take care of the background intensity stuff 
                CellArea = new Rectangle(0, 0, 3, BitmapImage.Height);

                //Average Intensity of Image
                Filter = new ImageViewer.PythonScripting.AverageImage.AverageIntensityPointTool();
                //Parameters required: AverageArray as string, CompareImage or Bounds as Image or Rectange, CompareImage as Image
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, CellArea, BitmapImage);
                PassData = Filter.PassData;

                //Add Array Point
                Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
                //Parameters required: ArrayName as string, datapoint as double
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "BGIntensityAverage", ImageNumber, PassData["AverageIntensity"]);


                CellArea = new Rectangle((int)(XData[ImageNumber] - CellHalf), (int)(YData[ImageNumber] - CellHalf), CellSize, CellSize);
                Graphics g = Graphics.FromImage(((ImageHolder)BitmapImage).InformationOverLay);
                g.DrawRectangle(Pens.Red, CellArea);

                g = null;

                return BitmapImage;
            }
            catch (Exception ex)
            {
                string[] lines = ex.StackTrace.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                System.Diagnostics.Debug.Print(lines[0]);
                throw ex;
            }
        }

        private ImageHolder ProcessImageClip(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;

            Rectangle CellArea = new Rectangle((int)Math.Truncate(CellPosX[ImageNumber] - CellHalf), (int)Math.Truncate(CellPosY[ImageNumber] - CellHalf), CellSize, CellSize);

            //Clip Image to New Image
            IEffect Filter = new ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect();
            //Parameters required: Clip Bounds as Rectangle
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, CellArea);

            //Invert Contrast
            Filter = new ImageViewer.Filters.Adjustments.InvertEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

            Filter = new ImageViewer.Filters.Effects.Flattening.FlattenEdges();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, CellArea);

            if (CellSize > 200)
            {
                //reduce size
                // Filter = new ImageViewer.Filters.Adjustments.downSampleEffect();
                // BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            }

          /*  //Focus Value of Image
            Filter = new ImageViewer.PythonScripting.Statistics.FocusValueTool();
            //Parameters required: Image as image
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, BitmapImage);
            PassData = Filter.PassData;

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValue", ImageNumber, PassData["FocusValue"]);*/


          //  Filter = new ImageViewer.Filters.Effects.RankOrder.MedianFilterTool();
          //  BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData,3);

            return BitmapImage;
        }

        double[] impulse = null;
        ConvolutionMethod DesiredMethod;
        PhysicalArray DensityGrid = null;

        private ImageHolder ProcessImageConvolute(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            PhysicalArray Slice = null;
            if (ConvolutionMethod.Convolution1D == DesiredMethod)
            {
                int NewSize = (int)(1.5 * CellSize);
                //Pad Image to New Image
                // Filter = new ImageViewer.PythonScripting.Programming_Tools.PadImageToNewEffect();
                //Parameters required: Clip Bounds as Rectangle
                // BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, new Rectangle(0, 0, NewSize , NewSize ));

                Filter = new ImageViewer.PythonScripting.Projection.Convolution1D();
                // Filter = new ImageViewer.PythonScripting.Projection.Convolution1DGPU();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, 1, impulse, false,true, 2, 2);
                Slice = (PhysicalArray)Filter.PassData["ConvolutionData"];

            }
            else
            {
                //Pad Image to New Image
                Filter = new ImageViewer.PythonScripting.Programming_Tools.PadImageToNewEffect();
                //Parameters required: Clip Bounds as Rectangle
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, new Rectangle(0, 0, 256, 256));
            }

            if (DensityGrid == null)
            {
                Filter = new ImageViewer.PythonScripting.Projection.CreateFilteredBackProjectionEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, Slice.GetLength(Axis.XAxis), Slice.GetLength(Axis.YAxis),
                    Slice.PhysicalEnd(Axis.XAxis) - Slice.PhysicalStart(Axis.XAxis),
                    Slice.PhysicalEnd(Axis.XAxis) - Slice.PhysicalStart(Axis.XAxis), 1,true );
                DensityGrid = (PhysicalArray)Filter.PassData["FBJObject"];
            }

            double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * ImageNumber;

            Filter = new ImageViewer.PythonScripting.Projection.InterpolateSliceEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, Slice, DensityGrid, AngleRadians);
            Slice = (PhysicalArray)Filter.PassData["ExpandedArray"];

            Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, Slice, DensityGrid, AngleRadians);

            return BitmapImage;
        }

        private ImageHolder ProcessImageMIP(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            if (ImageNumber % 6 == 0)
            {
                Filter = new ImageViewer.PythonScripting.Projection.MakeMIPProjectionEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DensityGrid, (double)ImageNumber / 500d * 2 * Math.PI, TempPath + "MIP\\Frame_" + string.Format("{0:000}.bmp", (int)(ImageNumber / 6)));

            }
            return BitmapImage;
        }

        //this is called once before all the images are looped through
        //it is good for opening array lists and log files
        private void PreBatchProcessBackgroundDivide(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary)
        {
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ImageViewer.Filters.ReplaceStringDictionary PassData = GlobalPassData;

            IEffect Filter;
            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB", DataPath + "X_PositionsB", dataEnvironment.AllImages.Count);

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_PositionsB", DataPath + "Y_PositionsB", dataEnvironment.AllImages.Count);

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "NumBlobs", DataPath + "NumBlobs", dataEnvironment.AllImages.Count);

            BatchLoopThroughImages(0, dataEnvironment, ImageFileList, ParamDictionary);
            print(tCellSize.ToString());
            print(len(ImageFileList).ToString());
            tCellSize = (tCellSize / len(ImageFileList)) / 2;

            //Stationary Pixels
            Filter = new ImageViewer.Filters.StationaryPixelsForm();
            //Parameters required: BitmapFiles as string[],  as , X_Positions as double[], Y_Positions as double[], ShowForm as bool, SubtractMethod as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ImageFileList, 100, "TopAndBottom", "X_PositionsB", "Y_PositionsB", false, "Divide", RunningThreaded, ThreadNumber, tCellSize, true);
            //Data out of type :
            PassData = Filter.PassData;

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


            //now do the divide
            BatchLoopThroughImagesSave(2, dataEnvironment, ImageFileListIn, ScriptParams);
            // BatchLoopThroughImages(2, dataEnvironment, ImageFileListIn, ScriptParams );

            dataEnvironment.ProgressLog.AddSafe("Divide", "Divide Succeeded");

        }

        private void PreBatchProcessCenter(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary)
        {
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;

            //Create Global Array
            IEffect Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_Positions", DataPath + "X_Positions", dataEnvironment.AllImages.Count);

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_Positions", DataPath + "Y_Positions", dataEnvironment.AllImages.Count);

            BatchLoopThroughImages(1, dataEnvironment, ImageFileList, ParamDictionary);

            print(tCellSize.ToString());
            print(len(ImageFileList).ToString());
            tCellSize = (tCellSize / len(ImageFileList)) / 2;
            CellSize = (int)(tCellSize * 2);
            CellHalf = CellSize / 2;
            print("Cell Size" + CellSize.ToString());

            //Center Cells
            Filter = new ImageViewer.Filters.CenterCellsTool2Form();
            //Parameters required: Bitmap_Filenames as string[], X_Positions as int[], Y_Positions as int[], SmoothingTypeX as string, X_Smooth_Param as int, SmootingTypeY as string, Y_Smooth_Param as int, ShowForm as string, CutSize as Size, OptionalOutputDir as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ImageFileList, 100, "X_Positions", "Y_Positions", "MovingAverage", 5, "MovingAverage", 5, "NoShow", new Size(CellSize, CellSize), RunningThreaded, ThreadNumber, true, TempPath);
            //Data out of type :
            PassData = Filter.PassData;

            dataEnvironment.ProgressLog.AddSafe("Centering", "Centering Line Created");
            dataEnvironment.ProgressLog.AddSafe("CenteringQuality", PassData["CenterAccuracy"].ToString() + "%");

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

            CellPosX = (double[])PassData["CorrectedX"];

            CellPosY = (double[])PassData["CorrectedY"];

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValue", DataPath + "FocusValue", dataEnvironment.AllImages.Count);

            List<ImageFile> OutFileList = BatchLoopThroughImagesSave(3, dataEnvironment, ImageFileListIn, ScriptParams);

            dataEnvironment.ProgressLog.AddSafe("Clipping", "Images Clipped");

        }

        private void PreBatchProcessRecon(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary)
        {
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;

            impulse = (double[])ParamDictionary["ImpulseFilter"];

            DesiredMethod = ConvolutionMethod.Convolution1D;
            BatchLoopThroughImages(4, dataEnvironment, ImageFileList, ParamDictionary);
            dataEnvironment.ProgressLog.AddSafe("Recon", "Recon Completed");

            if (Directory.Exists(TempPath + "MIP\\") == false)
                Directory.CreateDirectory(TempPath + "MIP\\");

           // BatchLoopThroughImages(5, dataEnvironment, ImageFileList, ParamDictionary);
            dataEnvironment.ProgressLog.AddSafe("MIP", "MIP Images completed");

        }

        //this is called once after the whole set has been processed
        private void PostBatchProcess(DataEnvironment dataEnvironment, List<ImageFile> FirstImageFileList, List<ImageFile> OutFileList, Dictionary<string, object> ParamDictionary)
        {
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            //Join Threads
            Filter = new ImageViewer.PythonScripting.Threads.JoinThreadsTool();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ThreadNumber);

            if (DensityGrid.CylinderArtifactRemoved == false)
                DensityGrid.RemoveOutterCylinder();

            System.Diagnostics.Debug.Print("Reached home" + Thread.CurrentThread.ManagedThreadId.ToString());
            if (ThreadNumber == 0)
            {
                ImageHolder image = dataEnvironment.AllImages[0];
                MathHelpsFileLoader.Save_Bitmap(DataPath + "FirstPP.bmp", image);

                image = dataEnvironment.AllImages[dataEnvironment.AllImages.Count / 4];
                MathHelpsFileLoader.Save_Bitmap(DataPath + "HalfPP.bmp", image);

                image = dataEnvironment.AllImages[dataEnvironment.AllImages.Count - 1];
                MathHelpsFileLoader.Save_Bitmap(DataPath + "lastPP.bmp", image);

                //#Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //#Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValue", DataPath + "FocusValue");

                //Read Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.ReadWholeArrayTool();
                //Parameters required: ArrayName as string
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValue");
                //Data out of type :
                PassData = Filter.PassData;

                double[] Focus = (double[])PassData["WholeArray"];

                dataEnvironment.ProgressLog.AddSafe("FocusValue", Average(Focus).ToString() + "," + Stdev(Focus).ToString());

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FGIntensityNoDivide", DataPath + "FGIntensityNoDivide");

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FGIntensityDivide", DataPath + "FGIntensityDivide");

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "BGIntensityAverage", DataPath + "BGIntensityAverage");

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "BGIntensitySD", DataPath + "BGIntensitySD");

                Filter = new ImageViewer.PythonScripting.Projection.MakeMIPProjectionEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DensityGrid, 0);

                BitmapImage.Save(DataPath + "Forward1.bmp");

              /*  try
                {
                    Filter = new ImageViewer.PythonScripting.Projection.CombineProjectionObjects();
                    BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                    DensityGrid = (PhysicalArray)Filter.PassData["CombinedProjection"];
                }
                catch { }*/

               // ImageViewer3D.Filters.Effects.RankOrder.MedianFilterTool3D Filter3d = new ImageViewer3D.Filters.Effects.RankOrder.MedianFilterTool3D();
               // DensityGrid.ReferenceDataDouble = (Filter3d.DoEffect(null, null, PassData, 3, DensityGrid.ReferenceDataDouble)).Data;

                DensityGrid.SaveData(DataPath + "ProjectionObject.cct");

                //Create AVI File From Frames
                /* Filter = new ImageViewer.Filters.Files.AVI.CreateAVIFileFromFramesEffect();
                 //#Parameters required: BitmapFilenames as string[], AVI_filename as string
                 BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DataPath + "Centering.avi", TempPath + "CenterMovie\\Frame_", "bmp");

                 EffectHelps.ClearTempFolder(TempPath + "CenterMovie\\");

                 //Create AVI File From Frames
                 Filter = new ImageViewer.Filters.Files.AVI.CreateAVIFileFromFramesEffect();
                 //#Parameters required: BitmapFilenames as string[], AVI_filename as string
                 BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DataPath + "MIP.avi", TempPath + "MIP\\Frame_", "bmp");
                 EffectHelps.ClearTempFolder(TempPath + "MIP\\");*/
            }
        }

        private object CriticalSection = new object();
        private List<ImageFile> BatchLoopThroughImagesSave(int IProcessFunction, DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary)
        {
            List<ImageFile> FileListOut = new List<ImageFile>();
            int imageIndex = 0;
            for (int CurrentParticle = 0; CurrentParticle < ImageFileList.Count; CurrentParticle++)
            {
                imageIndex = ImageFileList[CurrentParticle].Index;

                ImageHolder image = (ImageHolder)dataEnvironment.AllImages[imageIndex];

                if (IProcessFunction == 0)
                    image = ProcessImageFindRough(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 1)
                    image = ProcessImageFindFine(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 2)
                    image = ProcessImageDivide(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 3)
                    image = ProcessImageClip(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 4)
                    image = ProcessImageConvolute(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 5)
                    image = ProcessImageMIP(dataEnvironment, imageIndex, image);

                string fileout = "";// OutDirectory + OutFilename + String.Format("{0:000}", imageIndex) + OutExtension;

                FileListOut.Add(new ImageFile(imageIndex, fileout));

                ImageDisp.DisplayImage(imageIndex, image);
                dataEnvironment.AllImages[imageIndex] = image;

                if (RunningThreaded == false)
                    ImageViewer.PythonScripting.MacroHelper.DoEvents();
                else if (ImageViewer.PythonScripting.MacroHelper.ErrorOnThread == true)
                    Thread.CurrentThread.Abort();

                Thread.Sleep(0);
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
                ImageHolder image = (ImageHolder)dataEnvironment.AllImages[imageIndex];

                if (IProcessFunction == 0)
                    image = ProcessImageFindRough(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 1)
                    image = ProcessImageFindFine(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 2)
                    image = ProcessImageDivide(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 3)
                    image = ProcessImageClip(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 4)
                    image = ProcessImageConvolute(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 5)
                    image = ProcessImageMIP(dataEnvironment, imageIndex, image);

                ImageDisp.DisplayImage(imageIndex, image);
                if (RunningThreaded == false)
                    ImageViewer.PythonScripting.MacroHelper.DoEvents();
                else if (ImageViewer.PythonScripting.MacroHelper.ErrorOnThread == true)
                    Thread.CurrentThread.Abort();

                Thread.Sleep(0);
            }
        }

        public void RunScript(Dictionary<string, object> Variables)
        {
            ImageFileListIn = (List<ImageFile>)Variables["ImageFileListIn"];
            ScriptParams = (Dictionary<string, object>)Variables["ScriptParams"];

            TempPath = (string)Variables["TempPath"];
            ImageDisp = (ImageDisplayer)Variables["ImageDisp"];
            DataPath = (string)Variables["DataPath"];
            Executable = (string)Variables["Executable"];
            ExecutablePath = (string)Variables["ExecutablePath"];
            LibraryPath = (string)Variables["LibraryPath"];
            dataEnvironment = (DataEnvironment)Variables["dataEnvironment"];
            GlobalPassData = (ReplaceStringDictionary)Variables["GlobalPassData"];
            RunningThreaded = (bool)Variables["RunningThreaded"];
            ThreadNumber = (int)Variables["ThreadNumber"];

            FluorImage = null;

            PreBatchProcessBackgroundDivide(dataEnvironment, ImageFileListIn, ScriptParams);
            PreBatchProcessCenter(dataEnvironment, ImageFileListIn, ScriptParams);

            //Join Threads
            IEffect Filter = new ImageViewer.PythonScripting.Threads.JoinThreadsTool();
            // ImageHolder  BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment,null , GlobalPassData , ThisThreadId);

            //if (MasterThreadID == ThisThreadId)
            {
                PreBatchProcessRecon(dataEnvironment, ImageFileListIn, ScriptParams);
                PostBatchProcess(dataEnvironment, ImageFileListIn, null, ScriptParams);
            }
        }

        public IScript CloneScript()
        {
            return new ScriptCombined();
        }
    }
}

