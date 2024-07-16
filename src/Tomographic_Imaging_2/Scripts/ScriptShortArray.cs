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


namespace Tomographic_Imaging_2
{
    public class ScriptShortArray : IScript
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
        private int ntCellSize = 0;
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
        Dictionary<string, string> ScriptParams;
        // string ImageOutPath;
        // string ImageOutExten;
        // string ImageOutFileName;
        string TempPath;
        string Executable;
        string ExecutablePath;
        string LibraryPath;
        DataEnvironment dataEnvironment;

       // bool RunningThreaded;
        int ThreadNumber;
        bool ColorImage = true;
        #endregion

        #region pythonsimulation
        private void print(object message)
        {
            Console.WriteLine(message.ToString());
            //  dataEnvironment.ProgressLog.AddSafe("Debug", message.ToString());
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
        private double Stdev(double[] array, double Average)
        {
            double ave = Average;
            double sum = 0;
            double d = 0;
            for (int i = 0; i < array.Length; i++)
            {
                d = array[i] - ave;
                sum += d * d;
            }
            sum = Math.Sqrt(sum) / array.Length;
            return sum;
        }
        #endregion

        double[] impulse = null;
        ConvolutionMethod DesiredMethod;
        ProjectionArrayObject DensityGrid = null;


        private ImageHolder ProcessImageLoad(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
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
                BitmapImage = MathHelpsFileLoader. FixVisionGateImage(BitmapImage, 2);
            }

            ProcessImageFindRough(dataEnvironment, ImageNumber, BitmapImage);

           // try
            {
                //divide off the background curvature
                Filter = new ImageViewer.Filters.Effects.Flattening.FlattenEdges1DErrorCorrected();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            }
           // catch { }

            return BitmapImage;
        }

        private ImageHolder ProcessImageFindRough(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ImageViewer.Filters.ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            if (FluorImage == null)
            {
                //Iterative Threshold
                Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdEffect();
                object BitmapImage2 = Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

                Filter = new ImageViewer.PythonScripting.Statistics.FastBlackWhiteRatioTool();
                Filter.DoEffect(dataEnvironment, BitmapImage, PassData, BitmapImage2);
                double Percent = (double)Filter.PassData["PercentBlack"];
                dataEnvironment.ProgressLog.AddSafe("PercentBlack", Percent);
                FluorImage = (Percent > .2);
            }
            FluorImage = false;

            int Expander = 2;

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

            object ThreshImage;
            try
            {
                //SIS Threshold
                //Filter =new  ImageViewer.Filters.Thresholding.SISThresholdEffect();
                //BitmapImage =(ImageHolder ) Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                if (FluorImage.Value == true)
                {
                    //Otsu Threshold
                    Filter = new ImageViewer.Filters.Thresholding.OtsuThresholdEffect();
                    ThreshImage =Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                }
                else
                {
                    //Iterative Threshold
                    Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdEffect();
                    ThreshImage = Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                }

                //WaterShed
                Filter = new ImageViewer.Filters.Blobs.WaterShedTool();
                Filter.DoEffect(dataEnvironment, ThreshImage, PassData);
                //Data out of type :
                PassData = Filter.PassData;

                //Get Biggest Blob
                Filter = new ImageViewer.Filters.Blobs.GetBiggestCenterBlob();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, PassData["Blobs"], false);
                //Data out of type :
                PassData = Filter.PassData;
            }
            catch
            {}

           

            if (PassData.ContainsKey("MaxBlob")==false)
            {

                //Otsu Threshold
                Filter = new ImageViewer.Filters.Thresholding.OtsuThresholdEffect();
                ThreshImage = Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

                //WaterShed
                Filter = new ImageViewer.Filters.Blobs.WaterShedTool();
                Filter.DoEffect(dataEnvironment, ThreshImage, PassData);
                //Data out of type :
                PassData = Filter.PassData;

                //Get Biggest Blob
                Filter = new ImageViewer.Filters.Blobs.GetBiggestCenterBlob();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, PassData["Blobs"], false);
                //Data out of type :
                PassData = Filter.PassData;
            }

             int x ;
             int y;
             if (PassData.ContainsKey("MaxBlob") == true)
             {
                 BlobDescription Rect = (BlobDescription)PassData["MaxBlob"];
                 x = Rect.CenterOfGravity.X * Expander;
                 y = Rect.CenterOfGravity.Y * Expander;
                 //print((Rect.BlobBounds.Width*2).ToString() +"," + (2*Rect.BlobBounds.Height).ToString())
                 //print(x.ToString() + "," + y.ToString());

                 tCellSize = tCellSize + Rect.BlobBounds.Width * Expander;
                 tCellSize = tCellSize + Rect.BlobBounds.Height * Expander;
                 ntCellSize++;
             }
             else
             {
                 x = int.MinValue;
                 y = int.MinValue;
             }

            try
            {
                //Add Array Point
                Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
                //Parameters required: ArrayName as string, datapoint as double
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB", ImageNumber, x);
            }
            catch
            {
                dataEnvironment.ProgressLog.AddSafe("Debug", "Error adding");
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
                dataEnvironment.ProgressLog.AddSafe("Debug", "");
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
                dataEnvironment.ProgressLog.AddSafe("Debug", "");
            }
            // dataEnvironment.ProgressLog.AddSafe("imagenumber", ImageNumber.ToString());
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

            for (int i = 0; i < NumTestRuns; i++)
            {
                BitmapImage = OriginalTests.Copy();
                //Jitter
                Filter = new ImageViewer.Filters.Effects.Artistic.JitterEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

                //Iterative Threshold
                Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdEffect();
                object BitmapImage2 = Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

                if (ScriptParams["COGMethod"] == "Threshold")
                {
                    //Center Of Gravity
                    Filter = new ImageViewer.Filters.Blobs.CenterOfGravityTool();
                    Filter.DoEffect(dataEnvironment, BitmapImage2, PassData);
                    //Data out of type :
                    PassData = Filter.PassData;
                }
                else
                {
                    //Center Of Gravity using intensity
                    Filter = new ImageViewer.Filters.Blobs.CenterOfGravityIntensityTool();
                    Filter.DoEffect(dataEnvironment, OriginalTests, PassData, BitmapImage2);
                    //Data out of type :
                    PassData = Filter.PassData;
                }

                //Get Biggest Blob
                Filter = new ImageViewer.Filters.Blobs.GetBiggestBlob();
                Filter.DoEffect(dataEnvironment, BitmapImage, PassData, PassData["Blobs"], false);
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
                    Rectangle CellAreaPadded = new Rectangle(CellArea.Location, CellArea.Size);
                    CellAreaPadded.Inflate(CellArea.Width, CellArea.Height);

                    //Divide Image
                    Filter = new ImageViewer.Filters.Effects.Flattening.DivideImage();
                    BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, BackgroundMask, CellAreaPadded);
                }
                else
                {
                    //Invert Contrast
                    Filter = new ImageViewer.Filters.Adjustments.InvertEffect();
                    BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

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
                // Graphics g = Graphics.FromImage(((ImageHolder)BitmapImage).InformationOverLay);
                //  g.DrawRectangle(Pens.Red, CellArea);

                // g = null;

                return BitmapImage;
            }
            catch (Exception ex)
            {
                string[] lines = ex.StackTrace.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                dataEnvironment.ProgressLog.AddSafe("Debug", lines[0].ToString());
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

           // BitmapImage.Save(@"C:\Development\CellCT\testimages\image" + ImageNumber.ToString() + ".bmp");
            if (ScriptParams["FlatMethod"].ToLower() == "plane")
            {
                Filter = new ImageViewer.Filters.Effects.Flattening.FlattenEdges();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, CellArea);
            }
            else if (ScriptParams["FlatMethod"].ToLower() == "curve")
            {
                BitmapImage =  ImageViewer.Filters.Effects.Flattening.FlattenEdges1D.FlattenImageEdges(BitmapImage);
            }

            if (CellSize > 200)
            {
                //reduce size
                // Filter = new ImageViewer.Filters.Adjustments.downSampleEffect();
                // BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            }

            //Focus Value of Image
            Filter = new FocusValueTool();
            //Parameters required: Image as image
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, BitmapImage);
            PassData = Filter.PassData;

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValue", ImageNumber, PassData["FocusValue"]);

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValueMax", ImageNumber, PassData["FocusValueMax"]);

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValueMedian", ImageNumber, PassData["FocusValueMedian"]);

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValueMostProb", ImageNumber, PassData["FocusValueMostProb"]);

            return BitmapImage;
        }

        private ImageHolder ProcessBeforeConvolution(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            string Method = ScriptParams["PreprocessingMethod"];
            double Radius = EffectHelps.ConvertToDouble(ScriptParams["ProprocessingRadius"]);

            if (Method == "median")
            {
                Filter = new ImageViewer.Filters.Effects.RankOrder.MedianFilterTool();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, 3);
            }
            else if (Method == "average")
            {
                Filter = new ImageViewer.Filters.Effects.RankOrder.AverageFilterTool();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, 3);
            }
            else if (Method == "alphatrimmed")
            {
                Filter = new ImageViewer.Filters.Effects.RankOrder.AlphaTrimmedMeanFilterTool();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, 3);
            }
            else if (Method == "opening")
            {
                Filter = new ImageViewer.Filters.Effects.Morphology.OpeningTool();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, 3);
            }
            else if (Method == "closing")
            {
                Filter = new ImageViewer.Filters.Effects.Morphology.ClosingTool();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, 3);
            }

            //BitmapImage.Rotate90();
            return BitmapImage;
        }

        private ImageHolder ProcessImageFBP(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            double[,] Slice = null;
            if (ConvolutionMethod.Convolution1D == DesiredMethod)
            {
                int NewSize = (int)(1.5 * CellSize);

                //  Filter = new ImageViewer.PythonScripting.Projection.Convolution1D();
                // Filter = new ImageViewer.PythonScripting.Projection.Convolution1DGPU();
                BitmapImage = (ImageHolder)ConvolutionFilter.DoEffect(dataEnvironment, BitmapImage, PassData, 1, impulse, true, false, 2, 2);
                Slice = (double[,])ConvolutionFilter.PassData["ConvolutionData"];
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
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, (int)(Slice.GetLength(0)), (int)(Slice.GetLength(1)), 2, 2, 1, false, dataEnvironment.RunningOnGPU );

                DensityGrid = (ProjectionArrayObject)Filter.PassData["FBJObject"];
            }

            if (ScriptParams["FBPMedian"].ToLower() == "true")
            {
                Filter = new ImageViewer.Filters.Effects.RankOrder.MedianFilterTool();
                Slice = (double[,])Filter.DoEffect(dataEnvironment, Slice, PassData, 3);
            }

            double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * ImageNumber;
            if (dataEnvironment.RunningOnGPU  == false)
            {
                Filter = new ImageViewer.PythonScripting.Projection.InterpolateSliceEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, Slice, DensityGrid, AngleRadians);
                Slice = (double[,])Filter.PassData["ExpandedArray"];
            }

            Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, Slice, DensityGrid, AngleRadians);

            return new ImageHolder(Slice);
           // return BitmapImage;
        }

        private ImageHolder ProcessImageMIP(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            if (ImageNumber % 6 == 0)
            {
                Filter = new ImageViewer.PythonScripting.Projection.MakeMIPProjectionEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DensityGrid.Data, (double)ImageNumber / 500d * 2 * Math.PI, TempPath + "MIP\\Frame_" + string.Format("{0:000}.bmp", (int)(ImageNumber / 6)));

            }
            return BitmapImage;
        }

        private void PreBatchProcessBackgroundDivide(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Divide");
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

            dataEnvironment.ProgressLog.AddSafe("Position", "starting process Rough");
            //BatchLoopThroughImages(0, dataEnvironment, ImageFileList, ParamDictionary);
            BatchLoopThroughImagesSave(6, dataEnvironment, ImageFileListIn, ScriptParams);
            //print(tCellSize.ToString());
            // print(len(ImageFileList).ToString());
            tCellSize = (tCellSize / len(ImageFileList)) / 2;


            ImageViewer.Filters.CenterCellsTool2Form.PreFitLines(dataEnvironment, "X_PositionsB", "Y_PositionsB", ThreadNumber);

            dataEnvironment.ProgressLog.AddSafe("Position", "starting stationary pixels");
            //Stationary Pixels
            Filter = new ImageViewer.Filters.StationaryPixelsForm();
            //Parameters required: BitmapFiles as string[],  as , X_Positions as double[], Y_Positions as double[], ShowForm as bool, SubtractMethod as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData,  
                100, ScriptParams["BackgroundSubMethod"], "X_PositionsB", "Y_PositionsB", false, "Divide", ThreadNumber, tCellSize, true, ImageFileList);
            //Data out of type :
            PassData = Filter.PassData;

            dataEnvironment.ProgressLog.AddSafe("Position", "saving stationary pixels");
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

            dataEnvironment.ProgressLog.AddSafe("Position", "Dividing away background");
            //now do the divide
            BatchLoopThroughImagesSave(2, dataEnvironment, ImageFileListIn, ScriptParams);
            // BatchLoopThroughImages(2, dataEnvironment, ImageFileListIn, ScriptParams );

            dataEnvironment.ProgressLog.AddSafe("Divide", "Divide Succeeded");

        }

        private void PreBatchProcessCenter(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Center");
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

            dataEnvironment.ProgressLog.AddSafe("Position", "Fine");
            BatchLoopThroughImages(1, dataEnvironment, ImageFileList, ParamDictionary);

           /* print(tCellSize.ToString());
            print(len(ImageFileList).ToString());
            tCellSize = (tCellSize / len(ImageFileList)) / 2;*/
            CellSize = (int)(CellSize) * 2;//*1.4);
            CellHalf = CellSize / 2;
           // print("Cell Size" + CellSize.ToString());

            dataEnvironment.ProgressLog.AddSafe("Position", "Centering Fit");
            //Center Cells
            Filter = new ImageViewer.Filters.CenterCellsTool2Form();
            //Parameters required: Bitmap_Filenames as string[], X_Positions as int[], Y_Positions as int[], SmoothingTypeX as string, X_Smooth_Param as int, SmootingTypeY as string, Y_Smooth_Param as int, ShowForm as string, CutSize as Size, OptionalOutputDir as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData,  100, "X_Positions", "Y_Positions", "MovingAverage", 5, "MovingAverage", 5, false , new Size(CellSize, CellSize), ThreadNumber, true, TempPath,ImageFileList );
            //Data out of type :
            PassData = Filter.PassData;

            dataEnvironment.ProgressLog.AddSafe("Centering", "Centering Line Created");
            try
            {
                dataEnvironment.ProgressLog.AddSafe("CenteringQualityActual", PassData["CenterAccuracyActual"].ToString() + "%");
            }
            catch { }
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

            dataEnvironment.ProgressLog.AddSafe("Position", "Clipping");
            List<ImageFile> OutFileList = BatchLoopThroughImagesSave(3, dataEnvironment, ImageFileListIn, ScriptParams);

            dataEnvironment.ProgressLog.AddSafe("Clipping", "Images Clipped");

        }

        ImageViewer.PythonScripting.Projection.Convolution1D ConvolutionFilter;
        private void PreBatchProcessRecon(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Recon");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;

            dataEnvironment.ProgressLog.AddSafe("Position", "Getting Impulse");

            impulse = Filtering.GetRealSpaceFilter(ParamDictionary["FBPWindow"], EffectHelps.ConvertToInt(ParamDictionary["FBPResolution"]), EffectHelps.ConvertToInt(ParamDictionary["FBPResolution"]), 1);

            dataEnvironment.ProgressLog.AddSafe("Position", "Doing Convolution");
            DesiredMethod = ConvolutionMethod.Convolution1D;

            ConvolutionFilter = new ImageViewer.PythonScripting.Projection.Convolution1D();

            BatchLoopThroughImages(4, dataEnvironment, ImageFileList, ParamDictionary);


            dataEnvironment.ProgressLog.AddSafe("Recon", "Recon Completed");

            dataEnvironment.ProgressLog.AddSafe("Position", "Creating Mip DIR");

        }
        
        //this is called once after the whole set has been processed
        private void PostBatchProcess(DataEnvironment dataEnvironment, List<ImageFile> FirstImageFileList, List<ImageFile> OutFileList, Dictionary<string, string> ParamDictionary)
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Post");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;


            //Join Threads
            ImageViewer.PythonScripting.Threads.JoinThreadsTool.JoinThreads(dataEnvironment, "FinishEverything", ThreadNumber);

        
            dataEnvironment.ProgressLog.AddSafe("Debug", "Reached home" + Thread.CurrentThread.ManagedThreadId.ToString());

          

            if (ThreadNumber == 0)
            {
                ///the data is still on the card if we have been using the gpu.  Pull the data down and then save it
                if (dataEnvironment.RunningOnGPU  == true && DensityGrid.DataWhole !=null)
                {
                    DensityGrid.DataWhole = ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect.ReadReconVolume(dataEnvironment );
                }

               /* ///There is an ungly ring around the array.  This needs to be removed
                ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder FilterCyl = new ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder();
                if (DensityGrid.Data != null)
                    FilterCyl.DoEffect(null, null, GlobalPassData, DensityGrid.Data);
                else
                    FilterCyl.DoEffect(null, null, GlobalPassData, DensityGrid.DataWhole);*/

                /*ImageViewer3D.Filters.Adjustments.RemoveFBPCube FilterCyl = new ImageViewer3D.Filters.Adjustments.RemoveFBPCube();
                if (DensityGrid.Data != null)
                    FilterCyl.DoEffect(null, null, GlobalPassData, DensityGrid.Data);
                else
                    FilterCyl.DoEffect(null, null, GlobalPassData, DensityGrid.DataWhole);*/

                //PostProcessThreadCount = 0;

                ////////////////////////////////////////////save the volume////////////////////////////////////////////
                #region SaveVolume
                if (ScriptParams["SaveAsCCT"].ToLower() == "true")
                {
                    DensityGrid.SaveFile(DataPath + "ProjectionObject.cct");
                }
                if (ScriptParams["SaveAsRawDouble"].ToLower() == "true")
                {
                    DensityGrid.SaveFileRaw(DataPath + "ProjectionObject.raw",ProjectionArrayObject.RawFileTypes.Float32 );
                }
                if (ScriptParams["SaveAsRawFloat"].ToLower() == "true")
                {
                    DensityGrid.SaveFileRaw(DataPath + "ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.Float32);
                }
                if (ScriptParams["SaveAsRawInt"].ToLower() == "true")
                {
                    DensityGrid.SaveFileRaw(DataPath + "ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.UInt16 );
                }
                if (ScriptParams["Save8Bit"].ToLower() == "true")
                {
                    if (Directory.Exists(DataPath + "VirtualStack8\\") == false)
                        Directory.CreateDirectory(DataPath + "VirtualStack8\\");
                    DensityGrid.SaveFile(DataPath + "VirtualStack8\\VStack.tif", 8);
                }
                if (ScriptParams["Save16Bit"].ToLower() == "true")
                {
                    if (Directory.Exists(DataPath + "VirtualStack16\\") == false)
                        Directory.CreateDirectory(DataPath + "VirtualStack16\\");
                    DensityGrid.SaveFile(DataPath + "VirtualStack16\\VStack.tif", 16);
                }

                if (Directory.Exists(TempPath + "MIP\\") == false)
                    Directory.CreateDirectory(TempPath + "MIP\\");
                else
                {
                    string[] OldFrames = Directory.GetFiles(TempPath + "MIP\\");
                    foreach (string F in OldFrames)
                    {
                        try
                        {
                            File.Delete(F);
                        }
                        catch { }
                    }
                }
                #endregion
              
                #region Save and Create Movies
                DensityGrid.SaveCross(DataPath + "CrossSections.jpg");

                if (ScriptParams["SaveMIP"].ToLower() == "true")
                {
                    try
                    {
                        Filter = new ImageViewer.PythonScripting.Projection.MakeMIPMovieEffect();
                        if (DensityGrid.Data != null)
                            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DensityGrid.Data, DataPath + "MIP.avi", TempPath);
                        else
                            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DensityGrid.DataWhole, DataPath + "MIP.avi", TempPath);

                        BitmapImage.Save(DataPath + "Forward1.bmp");
                    }
                    catch { }
                }

                if (ScriptParams["SaveCenteringMovie"].ToLower() == "true")
                {
                    try
                    {
                        //Create AVI File From Frames
                        Filter = new ImageViewer.Filters.Files.AVI.CreateAVIFileFromFramesEffect();
                        //#Parameters required: BitmapFilenames as string[], AVI_filename as string
                        BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DataPath + "Centering.avi", TempPath + "CenterMovie\\Frame_", "jpg");
                    }
                    catch { }
                }

              
                #endregion

                #region DoImageComparison

                string VGFile = null;

                //get all the images needed from visiongate recons for comparison
                try
                {
                    Console.WriteLine(dataEnvironment.DataOutFolder);
                    string dirName = Path.GetFileNameWithoutExtension(dataEnvironment.DataOutFolder);
                    Console.WriteLine(dirName);
                    string[] parts = dirName.Split('_');
                    string Prefix = parts[0];
                    string Year = parts[1].Substring(0, 4);
                    string month = parts[1].Substring(4, 2);
                    string day = parts[1].Substring(6, 2);

                    string basePath = "Y:\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_cropped_16bit\\";

                    Console.WriteLine(basePath);

                    

                    if (Directory.Exists(basePath) == true)
                    {
                        string[] Files = Directory.GetFiles(basePath, "*.png");
                        Files = EffectHelps.SortNumberedFiles(Files);
                        VGFile = Files[Files.Length / 2];
                    }
                    else
                    {
                        basePath = "Y:\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_cropped_16bit\\";
                        if (Directory.Exists(basePath) == true)
                        {
                            string[] Files = Directory.GetFiles(basePath, "*.png");
                            Files = EffectHelps.SortNumberedFiles(Files);
                            VGFile = Files[Files.Length / 2];
                        }
                    }
                    if (VGFile != null)
                    {
                        try
                        {
                            Bitmap b = new Bitmap(VGFile);
                            b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            b.Save(dataEnvironment.DataOutFolder + "\\data\\VGExample.png");
                        }
                        catch { }
                    }
                }
                catch { }

                try
                {
                    //todo: cut down the stack based by the imagesize from reconquality check tool.  just need a nice tool
                    if (ScriptParams["CopyStack"].ToLower() == "true")
                    {
                        MathHelpLib.ProjectionFilters.CopyAndCutStackEffect.CopyStack(dataEnvironment.ExperimentFolder + "stack\\000", dataEnvironment.DataOutFolder + "stack", true, dataEnvironment.ProgressLog["IsColor"].ToString().ToLower() == "true");
                    }
                }
                catch { }


                //do the image quality tests
                if (ScriptParams["DoConvolutionQuality"].ToLower() == "true")
                {
                    dataEnvironment.ProgressLog.AddSafe("Position", "ImageQuality");
                    MathHelpLib.ProjectionFilters.ReconQualityCheckTool.CompareProjection(DensityGrid.DataWhole, dataEnvironment.ExperimentFolder, PassData );
                    

                }

                foreach (KeyValuePair<string, object > kvp in PassData)
                {
                    dataEnvironment.ProgressLog.AddSafe(kvp.Key, kvp.Value.ToString());
                }

                dataEnvironment.ProgressLog.AddSafe("ImageType", Path.GetExtension(dataEnvironment.WholeFileList[0]));

                #endregion
            }

            ///the resources should be cleaned up on the calling thread
            ConvolutionFilter.Dispose();
            ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect.Dispose(dataEnvironment);
        }

        #region Batchloop
        private object CriticalSection = new object();
        private List<ImageFile> BatchLoopThroughImagesSave(int IProcessFunction, DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
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
                    image = ProcessImageFBP(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 5)
                    image = ProcessImageMIP(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 6)
                    image = ProcessImageLoad(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 7)
                    image = ProcessBeforeConvolution(dataEnvironment, imageIndex, image);

                string fileout = "";// OutDirectory + OutFilename + String.Format("{0:000}", imageIndex) + OutExtension;

                FileListOut.Add(new ImageFile(imageIndex, fileout));

                ImageDisp.DisplayImage(imageIndex, image);
                dataEnvironment.AllImages[imageIndex] = image;

                if (dataEnvironment.RunningThreaded  == false)
                    ImageViewer.PythonScripting.MacroHelper.DoEvents();
                else if (ImageViewer.PythonScripting.MacroHelper.ErrorOnThread == true)
                    Thread.CurrentThread.Abort();

                Thread.Sleep(0);
            }
            return FileListOut;
        }

        private void BatchLoopThroughImages(int IProcessFunction, DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
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
                    image = ProcessImageFBP(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 5)
                    image = ProcessImageMIP(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 6)
                    image = ProcessImageLoad(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 7)
                    image = ProcessBeforeConvolution(dataEnvironment, imageIndex, image);

                ImageDisp.DisplayImage(imageIndex, image);
                if (dataEnvironment.RunningThreaded  == false)
                    ImageViewer.PythonScripting.MacroHelper.DoEvents();
                else if (ImageViewer.PythonScripting.MacroHelper.ErrorOnThread == true)
                    Thread.CurrentThread.Abort();

                Thread.Sleep(0);
            }
        }
        #endregion

        public void RunScript(Dictionary<string, object> Variables)
        {
            ImageFileListIn = (List<ImageFile>)Variables["ImageFileListIn"];
            ScriptParams = (Dictionary<string, string>)Variables["ScriptParams"];

            TempPath = (string)Variables["TempPath"];
            ImageDisp = (ImageDisplayer)Variables["ImageDisp"];
            DataPath = (string)Variables["DataPath"];
            Executable = (string)Variables["Executable"];
            ExecutablePath = (string)Variables["ExecutablePath"];
            LibraryPath = (string)Variables["LibraryPath"];
            dataEnvironment = (DataEnvironment)Variables["dataEnvironment"];
            GlobalPassData = (ReplaceStringDictionary)Variables["GlobalPassData"];
            //RunningThreaded = (bool)Variables["RunningThreaded"];
            ThreadNumber = (int)Variables["ThreadNumber"];

            FluorImage = null;

            if (ThreadNumber == 0)
            {
                try
                {
                    Dictionary<string, string> Values = EffectHelps.OpenXMLAndGetTags(dataEnvironment.ExperimentFolder + "\\info.xml", new string[] { "Color" });
                    if (Values["Color"].ToLower() == "true")
                        ColorImage = true;
                    else
                        ColorImage = false;

                    dataEnvironment.ProgressLog.AddSafe("IsColor", ColorImage.ToString());
                }
                catch
                {

                    dataEnvironment.ProgressLog.AddSafe("IsColor", "True");
                }
            }

            JoinThreadsTool.JoinThreads(dataEnvironment, "CheckifColor", ThreadNumber);
            //Console.WriteLine("Forcing image to monochrome!!!!!");
            ColorImage =  dataEnvironment.ProgressLog["IsColor"].ToString().ToLower() == "false";

            //do any pre convolution work.  This is where most of the changes should be located
            BatchLoopThroughImagesSave(7, dataEnvironment, ImageFileListIn, ScriptParams);

            PreBatchProcessRecon(dataEnvironment, ImageFileListIn, ScriptParams);
            PostBatchProcess(dataEnvironment, ImageFileListIn, null, ScriptParams);
        }

        public IScript CloneScript()
        {
            return new ScriptShortArray();
        }
    }
}

