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
    public class BaseScript : IScript
    {
        public string GetName()
        {
            return "ScriptCombined";
        }
        protected double[,] BackgroundMask;
        protected Rectangle CellWanderArea;
        protected int CellWanderMargin = 120;
        protected double[] XData;
        protected double[] YData;
        protected int CellSize = 170;
        protected int tCellSize = 0;
        protected int ntCellSize = 0;
        protected int CellHalf = 85;
        //private double[,] AverageIllumination;
        protected ImageViewer.Filters.ReplaceStringDictionary GlobalPassData;
        protected double[] CellPosX;
        protected double[] CellPosY;

        protected bool FluorImage = true;


        #region global values

        protected string DataPath;
        protected ImageDisplayer ImageDisp;
        protected List<ImageFile> ImageFileListIn;
        protected Dictionary<string, string> ScriptParams;
        // string ImageOutPath;
        // string ImageOutExten;
        // string ImageOutFileName;
        protected string TempPath;
        protected string Executable;
        protected string ExecutablePath;
        protected string LibraryPath;
        protected DataEnvironment dataEnvironment;

        // bool RunningThreaded;
        protected int ThreadNumber;
        protected bool ColorImage = true;
        #endregion

        #region pythonsimulation
        protected void print(object message)
        {
            Console.WriteLine(message.ToString());
            //  dataEnvironment.ProgressLog.AddSafe("Debug", message.ToString());
        }
        protected int len(Array array)
        {
            return array.Length;
        }
        protected int len(System.Collections.IList array)
        {
            return array.Count;
        }
        protected double min(double[] array)
        {
            return array.Min();
        }
        protected double max(double[] array)
        {
            return array.Max();
        }
        protected double Average(double[] array)
        {

            return array.Average();
        }
        protected double Stdev(double[] array)
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
        protected double Stdev(double[] array, double Average)
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

        protected double[] impulse = null;
        protected ConvolutionMethod DesiredMethod;
        protected ProjectionArrayObject DensityGrid = null;


        protected virtual ImageHolder ProcessImageLoad(DataEnvironment dataEnvironment, int ImageNumber)
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

            if (FluorImage == true)
            {
                //Invert Contrast
                Filter = new ImageViewer.Filters.Adjustments.InvertEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            }



            //creates a new image and then messes with the new image, so it does not affect this one.
            ProcessImageFindRough(dataEnvironment, ImageNumber, BitmapImage);

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

        protected virtual ImageHolder ProcessImageFindRough(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {

            ImageViewer.Filters.ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            /* if (FluorImage == null)
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
             //FluorImage = true;*/

            int Expander = 2;
            #region Shrinker
            if (FluorImage == true)
                Expander = 2;
            else
                Expander = 2;

            //this creates a new image and nothing else effects the original
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
            #endregion

            #region Find Cell
            object ThreshImage;
            try
            {
                //SIS Threshold
                //Filter =new  ImageViewer.Filters.Thresholding.SISThresholdEffect();
                //BitmapImage =(ImageHolder ) Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                if (FluorImage == true)
                {
                    //Otsu Threshold
                    Filter = new ImageViewer.Filters.Thresholding.OtsuThresholdEffect();
                    ThreshImage = Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
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
            { }



            if (PassData.ContainsKey("MaxBlob") == false)
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
            #endregion

            #region Process Location

            int x;
            int y;
            if (PassData.ContainsKey("MaxBlob") == true)
            {
                try
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
                catch
                {
                    x = int.MinValue;
                    y = int.MinValue;
                }
            }
            else
            {
                x = int.MinValue;
                y = int.MinValue;
            }

            #endregion

            object blobs = PassData["Blobs"];
            if (blobs.GetType() == typeof(List<BlobDescription>))
            {
                CenterCellsTool2Form.StoreBlobLocation(dataEnvironment, ImageNumber, (List<BlobDescription>)blobs, Expander);
            }
            else
            {
                CenterCellsTool2Form.StoreBlobLocation(dataEnvironment, ImageNumber, (BlobDescription[])blobs, Expander);
            }

            #region Save
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


            #endregion
            // dataEnvironment.ProgressLog.AddSafe("imagenumber", ImageNumber.ToString());
            // Console.WriteLine("RoughMax " + BitmapImage.Max());
            // Console.WriteLine("Rough Width : " + BitmapImage.Width.ToString());
            return BitmapImage;
        }

        protected virtual ImageHolder ProcessImageFindFine(DataEnvironment dataEnvironment, int ImageNumber)
        {

            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];//.Clone();
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            //Console.WriteLine("Before " + BitmapImage.Max());
            // Console.WriteLine("Before Width : " + BitmapImage.Width.ToString());

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


            if (FluorImage == true)
            {
                //Invert Contrast
                Filter = new ImageViewer.Filters.Adjustments.InvertEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

            }

            ImageHolder OriginalTests = BitmapImage;

            BlobDescription Rect = null;
            int NumTestRuns = 4;
            object BitmapImage2 = null;
            for (int i = 0; i < NumTestRuns; i++)
            {
                BitmapImage = OriginalTests.Copy();
                //Jitter
                Filter = new ImageViewer.Filters.Effects.Artistic.JitterEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

                //Iterative Threshold
                Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdCornerFillEffect();
                BitmapImage2 = Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

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
            // Console.WriteLine("After " + dataEnvironment.AllImages[ImageNumber].Max());
            // Console.WriteLine("After Width : " + dataEnvironment.AllImages[ImageNumber].Width.ToString());
            return EffectHelps.FixImageFormat(BitmapImage2);
        }

        protected virtual ImageHolder ProcessImageDivide(DataEnvironment dataEnvironment, int ImageNumber)
        {
            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];
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

        protected virtual ImageHolder ProcessImageClip(DataEnvironment dataEnvironment, int ImageNumber)
        {
            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];
            ReplaceStringDictionary PassData = GlobalPassData;

            // Console.WriteLine("Before " + BitmapImage.Max());
            // Console.WriteLine("Before Width : " + BitmapImage.Width.ToString());

            Rectangle CellArea = new Rectangle((int)Math.Truncate(CellPosX[ImageNumber] - CellHalf), (int)Math.Truncate(CellPosY[ImageNumber] - CellHalf), CellSize, CellSize);


            //Clip Image to New Image
            IEffect Filter = new ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect();
            //Parameters required: Clip Bounds as Rectangle
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, CellArea);

            //  Console.WriteLine("After1 " + BitmapImage.Max());

            //Invert Contrast
            Filter = new ImageViewer.Filters.Adjustments.InvertEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            // Console.WriteLine("After2 " + BitmapImage.Max());

            // BitmapImage.Save(@"C:\Development\CellCT\testimages\image" + ImageNumber.ToString() + ".bmp");
            if (ScriptParams["FlatMethod"].ToLower() == "plane")
            {
                Filter = new ImageViewer.Filters.Effects.Flattening.FlattenEdges();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, CellArea);
            }
            else if (ScriptParams["FlatMethod"].ToLower() == "curve")
            {
                BitmapImage = ImageViewer.Filters.Effects.Flattening.FlattenEdges1D.FlattenImageEdges(BitmapImage);
            }

            //   Console.WriteLine("After3 " + BitmapImage.Max());
            if (CellSize > 200)
            {
                //reduce size
                // Filter = new ImageViewer.Filters.Adjustments.downSampleEffect();
                // BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            }

            //Focus Value of Image




            #region Focus Value
            double d;
            double FocusValue = 0;// FocusValueTool.FocusValueF4(BitmapImage, out d);
            ImageViewer.PythonScripting.Arrays.AddPointArrayTool.AddPoint(dataEnvironment, "FocusValue", ImageNumber, FocusValue);

            #endregion

            //  Console.WriteLine("After5 " + BitmapImage.Max());

            if (FluorImage == true)
            {
                //Invert Contrast
                Filter = new ImageViewer.Filters.Adjustments.InvertEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

            }
            // Console.WriteLine("After6 " + BitmapImage.Max());
            return BitmapImage;
        }

        protected virtual double[,] DoSliceConvolution(ImageHolder BitmapImage, ReplaceStringDictionary PassData)
        {
            IEffect Filter;
            double[,] Slice = null;
            if (ConvolutionMethod.Convolution1D == DesiredMethod)
            {
                BitmapImage = (ImageHolder)ConvolutionFilter.DoEffect(dataEnvironment, BitmapImage, PassData, 1, impulse, true, false, 2, 2);
                try
                {
                    if (ConvolutionFilter.PassData.ContainsKey("ConvolutionData") == false)
                        System.Diagnostics.Debug.Print("");
                    Slice = (double[,])ConvolutionFilter.PassData["ConvolutionData"];
                }
                catch
                {
                    // System.Diagnostics.Debug.Print("Convolution error");
                    Console.WriteLine("Convolution error");
                }
            }
            else
            {
                //Pad Image to New Image
                Filter = new ImageViewer.PythonScripting.Programming_Tools.PadImageToNewEffect();
                //Parameters required: Clip Bounds as Rectangle
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, new Rectangle(0, 0, 256, 256));
            }


            if (ScriptParams["FBPMedian"].ToLower() == "true")
            {
                Filter = new ImageViewer.Filters.Effects.RankOrder.MedianFilterTool();
                Slice = (double[,])Filter.DoEffect(dataEnvironment, Slice, PassData, 3);
            }

            return Slice;
        }

        protected static object DehyrateLock = new object();

        protected static bool SaveDehydrated = false;
        protected virtual ImageHolder ProcessBeforeConvolution(DataEnvironment dataEnvironment, int ImageNumber)
        {
            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            if (ScriptParams["SaveCenteredImage"].ToLower() == "true")
            {
                lock (DehyrateLock)
                {
                    if (Directory.Exists(dataEnvironment.DataOutFolder + "Dehydrated") == false)
                    {
                        Directory.CreateDirectory(dataEnvironment.DataOutFolder + "Dehydrated");
                        SaveDehydrated = true;
                    }
                }

                if (SaveDehydrated)
                    BitmapImage.Save(dataEnvironment.DataOutFolder + "Dehydrated\\" + string.Format("{0:000}", ImageNumber) + ".cct");
            }



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

            return BitmapImage;
        }

        /* protected virtual ImageHolder ProcessImageFBPSiddon(DataEnvironment dataEnvironment, int ImageNumber)
         {
             ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];
           
             ReplaceStringDictionary PassData = GlobalPassData;
             IEffect Filter;

             double[,] Slice = DoSliceConvolution(BitmapImage, PassData);

             double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * ImageNumber;
             Console.WriteLine(ImageNumber + " project");
             Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect();
             BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, Slice, DensityGrid, AngleRadians);

             return new ImageHolder(Slice);
         }

         protected virtual ImageHolder ProcessImageFBPGaussian(DataEnvironment dataEnvironment, int ImageNumber)
         {
             Console.WriteLine(ImageNumber.ToString());
             ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];
             ReplaceStringDictionary PassData = GlobalPassData;
             IEffect Filter;
             dataEnvironment.RunningOnGPU = false;
             double[,] Slice = null;
             if (ConvolutionMethod.Convolution1D == DesiredMethod)
             {
                 BitmapImage = (ImageHolder)ConvolutionFilter.DoEffect(dataEnvironment, BitmapImage, PassData, 1, impulse, true, false, 2, 2);
                 try
                 {
                     if (ConvolutionFilter.PassData.ContainsKey("ConvolutionData") == false)
                         System.Diagnostics.Debug.Print("");
                     Slice = (double[,])ConvolutionFilter.PassData["ConvolutionData"];
                 }
                 catch
                 {
                     // System.Diagnostics.Debug.Print("Convolution error");
                     Console.WriteLine("Convolution error");
                 }
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
                 BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, (int)(Slice.GetLength(0)), (int)(Slice.GetLength(1)), 2, 2, 1, false, dataEnvironment.RunningOnGPU);

                 DensityGrid = (ProjectionArrayObject)Filter.PassData["FBJObject"];
             }

             if (ScriptParams["FBPMedian"].ToLower() == "true")
             {
                 Filter = new ImageViewer.Filters.Effects.RankOrder.MedianFilterTool();
                 Slice = (double[,])Filter.DoEffect(dataEnvironment, Slice, PassData, 3);
             }

             double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * ImageNumber;
             Console.WriteLine(ImageNumber + " project");
             Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionGaussianEffect();
             BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, Slice, DensityGrid, AngleRadians);

             return new ImageHolder(Slice);
         }*/


        protected string ReconInterpolationMethod = "Linear";
        protected virtual ImageHolder ProcessImageFBP(DataEnvironment dataEnvironment, int ImageNumber)
        {
            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];

            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            double[,] Slice = DoSliceConvolution(BitmapImage, PassData);

            double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * ImageNumber;


            if (ReconInterpolationMethod == "Linear")
            {
                if (dataEnvironment.RunningOnGPU == false)
                {
                    Filter = new ImageViewer.PythonScripting.Projection.InterpolateSliceEffect();
                    BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, Slice, DensityGrid, AngleRadians);
                    Slice = (double[,])Filter.PassData["ExpandedArray"];
                }

                Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, Slice, DensityGrid, AngleRadians);

            }
            else if (ReconInterpolationMethod == "Siddon")
            {
                Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, Slice, DensityGrid, AngleRadians);
            }
            else if (ReconInterpolationMethod == "Gaussian")
            {
                Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionGaussianEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, Slice, DensityGrid, AngleRadians);
            }

            return new ImageHolder(Slice);
            // return BitmapImage;
        }

        protected virtual ImageHolder ProcessImageMIP(DataEnvironment dataEnvironment, int ImageNumber)
        {
            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            if (ImageNumber % 6 == 0)
            {
                Filter = new ImageViewer.PythonScripting.Projection.MakeMIPProjectionEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DensityGrid.Data, (double)ImageNumber / 500d * 2 * Math.PI, TempPath + "MIP\\Frame_" + string.Format("{0:000}.bmp", (int)(ImageNumber / 6)));

            }
            return BitmapImage;
        }

        protected virtual void PreBatchProcessBackgroundDivide(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Divide");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ImageViewer.Filters.ReplaceStringDictionary PassData = GlobalPassData;

            IEffect Filter;
            #region Create Arrays
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


            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "CellSize", DataPath + "CellSize", dataEnvironment.AllImages.Count);

            #endregion


            dataEnvironment.ProgressLog.AddSafe("Position", "starting process Rough");
            //BatchLoopThroughImages(0, dataEnvironment, ImageFileList, ParamDictionary);
            BatchLoopThroughImagesSave(6, dataEnvironment, ImageFileListIn, ScriptParams);
            //print(tCellSize.ToString());
            // print(len(ImageFileList).ToString());
            tCellSize = (tCellSize / len(ImageFileList)) / 2;

            ImageViewer.Filters.CenterCellsTool2Form.PreFitLines(dataEnvironment, "X_PositionsB", "Y_PositionsB", ThreadNumber);

            dataEnvironment.ProgressLog.AddSafe("Position", "starting stationary pixels");
            //Stationary Pixels

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

                try
                {
                    //Add Array Point
                    Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
                    //Parameters required: ArrayName as string, datapoint as double
                    BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "CellSize", 0, (int)((int)PassData["MaxCellSize"]));
                }
                catch
                {
                    dataEnvironment.ProgressLog.AddSafe("Debug", "");
                }

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "CellSize", DataPath + "CellSize");


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

        protected virtual void PreBatchProcessCenter(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Center");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;

            #region Create Arrays
            //Create Global Array
            IEffect Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_Positions", DataPath + "X_Positions", dataEnvironment.AllImages.Count);

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_Positions", DataPath + "Y_Positions", dataEnvironment.AllImages.Count);
            #endregion

            dataEnvironment.ProgressLog.AddSafe("Position", "Fine");
            BatchLoopThroughImages(1, dataEnvironment, ImageFileList, ParamDictionary);

            /* print(tCellSize.ToString());
             print(len(ImageFileList).ToString());
             tCellSize = (tCellSize / len(ImageFileList)) / 2;*/
            if (FluorImage == true)
            {
                CellSize = (int)(CellSize * 1.2);
            }
            else
            {
                CellSize = (int)(CellSize * 1.7);
            }

            dataEnvironment.ProgressLog.AddSafe("CellSize ", CellSize.ToString());

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


            #endregion

            dataEnvironment.ProgressLog.AddSafe("Position", "Clipping");
            List<ImageFile> OutFileList = BatchLoopThroughImagesSave(3, dataEnvironment, ImageFileListIn, ScriptParams);

            dataEnvironment.ProgressLog.AddSafe("Clipping", "Images Clipped");

        }

        protected object CreateGridObject = new object();
        protected ImageViewer.PythonScripting.Projection.Convolution1D ConvolutionFilter;
        protected virtual void PreBatchProcessRecon(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            GC.Collect();

            dataEnvironment.ProgressLog.AddSafe("Prep", "Recon");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage;// = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;

            dataEnvironment.ProgressLog.AddSafe("Position", "Getting Impulse");

            int FilterWidth = EffectHelps.ConvertToInt(ParamDictionary["FBPResolution"]);//, EffectHelps.ConvertToInt(ParamDictionary["FBPResolution"])
            impulse = Filtering.GetRealSpaceFilter(ParamDictionary["FBPWindow"], FilterWidth, FilterWidth, (double)FilterWidth / 2d);


            BitmapImage = dataEnvironment.AllImages[1];

            lock (CreateGridObject)
            {
                dataEnvironment.ProgressLog.AddSafe("recon", "ImageSize-" + BitmapImage.Width.ToString());
                //create a densitygrid if this is the first run, otherwise just pull the already created grid
                IEffect Filter;
                Filter = new ImageViewer.PythonScripting.Projection.CreateFilteredBackProjectionEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, (int)(BitmapImage.Width), (int)(BitmapImage.Height), 2, 2, 1, false, true);
                DensityGrid = (ProjectionArrayObject)Filter.PassData["FBJObject"];
            }

            dataEnvironment.ProgressLog.AddSafe("Position", "Doing Convolution");

            DesiredMethod = ConvolutionMethod.Convolution1D;

            ConvolutionFilter = new ImageViewer.PythonScripting.Projection.Convolution1D();

            ReconInterpolationMethod = ScriptParams["InterpolationMethod"];

            BatchLoopThroughImages(4, dataEnvironment, ImageFileList, ParamDictionary);

            dataEnvironment.ProgressLog.AddSafe("Recon", "Recon Completed");

            dataEnvironment.ProgressLog.AddSafe("Position", "Creating Mip DIR");

        }

        #region Saving everything
        protected virtual void WaitForFinish()
        {
            //Join Threads
            ImageViewer.PythonScripting.Threads.JoinThreadsTool.JoinThreads(dataEnvironment, "FinishEverything", ThreadNumber);
            dataEnvironment.ProgressLog.AddSafe("Debug", "Reached home" + Thread.CurrentThread.ManagedThreadId.ToString());
        }

        protected virtual void NormalizedImage()
        {
            if (DensityGrid.Data != null)
                ImageViewer3D.PythonScripting.CellCT.NormalizeFBPVolumeEffect.NormalizeFBPVolume(ref DensityGrid.Data, dataEnvironment.AllImages.Count, true);
            else
                ImageViewer3D.PythonScripting.CellCT.NormalizeFBPVolumeEffect.NormalizeFBPVolume(ref DensityGrid.DataWhole, dataEnvironment.AllImages.Count, true,-500);


            ///There is an ungly ring around the array.  This needs to be removed

            if (DensityGrid.Data != null)
                ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder.DoRemoveFBPCylinder(ref DensityGrid.Data);
            else
                ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder.DoRemoveFBPCylinder(ref DensityGrid.DataWhole);

        }

        protected virtual void SaveInfo()
        {
            if (ScriptParams["SaveAsCCT"].ToLower() == "true")
            {
                DensityGrid.SaveFile(DataPath + "ProjectionObject.cct");
            }
            if (ScriptParams["SaveAsRawDouble"].ToLower() == "true")
            {
                DensityGrid.SaveFileRaw(DataPath + "ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.Float32);
            }
            if (ScriptParams["SaveAsRawFloat"].ToLower() == "true")
            {
                DensityGrid.SaveFileRaw(DataPath + "ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.Float32);
            }
            if (ScriptParams["SaveAsRawInt"].ToLower() == "true")
            {
                DensityGrid.SaveFileRaw(DataPath + "ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.UInt16);
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

        }

        protected virtual void SaveArrays()
        {
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            #region SaveArrays
            try
            {
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
                double Ave = Average(Focus);
                dataEnvironment.ProgressLog.AddSafe("FocusValueAverage", Ave.ToString());
                dataEnvironment.ProgressLog.AddSafe("FocusValueSD", (Stdev(Focus, Ave) / Ave * 100d).ToString());
                dataEnvironment.ProgressLog.AddSafe("FocusValueMax", max(Focus).ToString());
                dataEnvironment.ProgressLog.AddSafe("FocusValueMin", min(Focus).ToString());


            #endregion
                //************************************************  Focus Values ***************************************************************************
                #region Calc Intensity Values

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
            }
            catch { }
                #endregion
        }

        protected virtual void SaveExamples()
        {
            #region SaveExamples
            try
            {
                ImageHolder image = dataEnvironment.AllImages[0];
                MathHelpsFileLoader.Save_Bitmap(DataPath + "FirstPP.bmp", image);
            }
            catch { }

            try
            {
                ImageHolder image = dataEnvironment.AllImages[dataEnvironment.AllImages.Count / 4];
                MathHelpsFileLoader.Save_Bitmap(DataPath + "QuarterPP.bmp", image);
            }
            catch { }

            try
            {
                ImageHolder image = dataEnvironment.AllImages[dataEnvironment.AllImages.Count / 2];
                MathHelpsFileLoader.Save_Bitmap(DataPath + "HalfPP.bmp", image);
            }
            catch { }

            try
            {
                ImageHolder image = dataEnvironment.AllImages[dataEnvironment.AllImages.Count * 3 / 4];
                MathHelpsFileLoader.Save_Bitmap(DataPath + "LastQuarterPP.bmp", image);
            }
            catch { }

            try
            {
                ImageHolder image = dataEnvironment.AllImages[dataEnvironment.AllImages.Count - 1];
                MathHelpsFileLoader.Save_Bitmap(DataPath + "lastPP.bmp", image);
            }
            catch { }
            DensityGrid.SaveCross(DataPath + "CrossSections.jpg");
            #endregion

        }

        protected virtual void SaveMIP()
        {
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            if (ScriptParams["SaveMIP"].ToLower() == "true")
            {
                try
                {
                    Filter = new ImageViewer.PythonScripting.Projection.MakeMIPMovie3Effect();
                    if (DensityGrid.Data != null)
                        BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DensityGrid.Data, DataPath + "MIP.avi", TempPath);
                    else
                        BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DensityGrid.DataWhole, DataPath + "MIP.avi", TempPath);

                    BitmapImage.Save(DataPath + "Forward1.bmp");
                }
                catch { }
            }
        }

        protected virtual void SaveCenteringMovie()
        {
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;
            if (ScriptParams["SaveCenteringMovie"].ToLower() == "true")
            {
                try
                {
                    ImageViewer.ImagingTools.CreateAVIVideo(DataPath + "Centering.avi", Directory.GetFiles(TempPath + "CenterMovie", "*.jpg"));
                }
                catch { }
            }
        }

        protected virtual string GetVGImage()
        {
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
            return VGFile;
        }

        protected virtual void MoveStack()
        {
            try
            {
                //todo: cut down the stack based by the imagesize from reconquality check tool.  just need a nice tool
                if (ScriptParams["CopyStack"].ToLower() == "true")
                {
                    MathHelpLib.ProjectionFilters.CopyAndCutStackEffect.CopyStack(dataEnvironment.ExperimentFolder + "stack\\000", dataEnvironment.DataOutFolder + "stack", true, dataEnvironment.ProgressLog["IsColor"].ToString().ToLower() == "true");
                }
            }
            catch { }

        }

        protected virtual void DoQualityChecks()
        {
            ReplaceStringDictionary PassData = GlobalPassData;
            //do the image quality tests
            if (ScriptParams["DoConvolutionQuality"].ToLower() == "true")
            {
                try
                {
                    dataEnvironment.ProgressLog.AddSafe("Position", "ImageQuality");
                    MathHelpLib.ProjectionFilters.ReconQualityCheckTool.CompareProjection(DensityGrid.DataWhole, dataEnvironment.ExperimentFolder, PassData);
                }
                catch { }
            }
        }

        //this is called once after the whole set has been processed
        protected virtual void PostBatchProcess(DataEnvironment dataEnvironment, List<ImageFile> FirstImageFileList, List<ImageFile> OutFileList, Dictionary<string, string> ParamDictionary)
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Post");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            WaitForFinish();

            //allow the gpu to definitely finish
            Thread.Sleep(2000);
            if (ThreadNumber == 0)
            {
                ///the data is still on the card if we have been using the gpu.  Pull the data down and then save it
                /*if (dataEnvironment.RunningOnGPU == true && DensityGrid.DataWhole != null)
                {
                    DensityGrid.DataWhole = ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect.ReadReconVolume(dataEnvironment);
                }*/

                NormalizedImage();



                /*   double[,] Slice = new double[DensityGrid.Data[0].GetLength(0),DensityGrid.Data[0].GetLength(0)];
                   ImageViewer.PythonScripting.Projection.DoARTSliceProjectionEffect.DoForwardProjection_OneSlice(ref Slice,1,1,DensityGrid,0,Axis2D.YAxis);

                   double AllAverage = DensityGrid.Data[(int)(DensityGrid.Data.Length / 2d)].AverageArray();
                   double AverageFront = Slice.AverageArray();
                   double SliceAverage =dataEnvironment.AllImages[0].GetAverage();


                   double mAA = DensityGrid.Data[(int)(DensityGrid.Data.Length / 2d)].MaxArray();
                   double mAF = Slice.MaxArray();
                   double mSSA = dataEnvironment.AllImages[0].Max();*/


                ////////////////////////////////////////////save the volume////////////////////////////////////////////
                SaveInfo();
                ///////////////////////////////////////////////Save the example images //////////////////////////////
                SaveExamples();
                //************************************************  Focus Values ***************************************************************************
                SaveArrays();
                /////////////////////////////////////////////////////////////Save movies//////////////////////////////////////////
                SaveMIP();

                SaveCenteringMovie();


                string VGImage = GetVGImage();

                MoveStack();

                DoQualityChecks();

                foreach (KeyValuePair<string, object> kvp in PassData)
                {
                    dataEnvironment.ProgressLog.AddSafe(kvp.Key, kvp.Value.ToString());
                }

                dataEnvironment.ProgressLog.AddSafe("ImageType", Path.GetExtension(dataEnvironment.WholeFileList[0]));

            }

            ///the resources should be cleaned up on the calling thread
            ConvolutionFilter.Dispose();
            ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect.Dispose(dataEnvironment);
        }
        #endregion

        #region Batchloop
        private object CriticalSection = new object();
        protected List<ImageFile> BatchLoopThroughImagesSave(int IProcessFunction, DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            List<ImageFile> FileListOut = new List<ImageFile>();
            int imageIndex = 0;
            for (int CurrentParticle = 0; CurrentParticle < ImageFileList.Count; CurrentParticle++)
            {
                imageIndex = ImageFileList[CurrentParticle].Index;

                ImageHolder image = null;// = (ImageHolder)dataEnvironment.AllImages[imageIndex];

                if (IProcessFunction == 0)
                    image = ProcessImageFindRough(dataEnvironment, imageIndex, (ImageHolder)dataEnvironment.AllImages[imageIndex]);
                else if (IProcessFunction == 1)
                    image = ProcessImageFindFine(dataEnvironment, imageIndex);
                else if (IProcessFunction == 2)
                    image = ProcessImageDivide(dataEnvironment, imageIndex);
                else if (IProcessFunction == 3)
                    image = ProcessImageClip(dataEnvironment, imageIndex);
                else if (IProcessFunction == 4)
                    image = ProcessImageFBP(dataEnvironment, imageIndex);
                else if (IProcessFunction == 5)
                    image = ProcessImageMIP(dataEnvironment, imageIndex);
                else if (IProcessFunction == 6)
                    image = ProcessImageLoad(dataEnvironment, imageIndex);
                else if (IProcessFunction == 7)
                    image = ProcessBeforeConvolution(dataEnvironment, imageIndex);

                string fileout = "";// OutDirectory + OutFilename + String.Format("{0:000}", imageIndex) + OutExtension;

                FileListOut.Add(new ImageFile(imageIndex, fileout));

                ImageDisp.DisplayImage(imageIndex, image);
                dataEnvironment.AllImages[imageIndex] = image;

                if (dataEnvironment.RunningThreaded == false)
                    ImageViewer.PythonScripting.MacroHelper.DoEvents();
                else if (ImageViewer.PythonScripting.MacroHelper.ErrorOnThread == true)
                    Thread.CurrentThread.Abort();

                Thread.Sleep(0);
            }
            return FileListOut;
        }

        protected void BatchLoopThroughImages(int IProcessFunction, DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            int imageIndex;
            for (int CurrentParticle = 0; CurrentParticle < ImageFileList.Count; CurrentParticle++)
            {
                //print(ImageFileList[CurrentParticle])
                imageIndex = ImageFileList[CurrentParticle].Index;
                ImageHolder image = null;// = (ImageHolder)dataEnvironment.AllImages[imageIndex];

                if (IProcessFunction == 0)
                    image = ProcessImageFindRough(dataEnvironment, imageIndex, (ImageHolder)dataEnvironment.AllImages[imageIndex]);
                else if (IProcessFunction == 1)
                    image = ProcessImageFindFine(dataEnvironment, imageIndex);
                else if (IProcessFunction == 2)
                    image = ProcessImageDivide(dataEnvironment, imageIndex);
                else if (IProcessFunction == 3)
                    image = ProcessImageClip(dataEnvironment, imageIndex);
                else if (IProcessFunction == 4)
                    image = ProcessImageFBP(dataEnvironment, imageIndex);
                else if (IProcessFunction == 5)
                    image = ProcessImageMIP(dataEnvironment, imageIndex);
                else if (IProcessFunction == 6)
                    image = ProcessImageLoad(dataEnvironment, imageIndex);
                else if (IProcessFunction == 7)
                    image = ProcessBeforeConvolution(dataEnvironment, imageIndex);

                ImageDisp.DisplayImage(imageIndex, image);
                if (dataEnvironment.RunningThreaded == false)
                    ImageViewer.PythonScripting.MacroHelper.DoEvents();
                else if (ImageViewer.PythonScripting.MacroHelper.ErrorOnThread == true)
                    Thread.CurrentThread.Abort();

                Thread.Sleep(0);
            }
        }
        #endregion

        protected virtual void DoRun(Dictionary<string, object> Variables)
        {
            //ColorImage = false;

            //format loaded images (i.e. select only one channel) //now done inside find rough
            //  BatchLoopThroughImagesSave(6, dataEnvironment, ImageFileListIn, ScriptParams);

            if ((Variables.ContainsKey("LoadPreProcessed") != true || (string)Variables["LoadPreProcessed"] == "False"))
            {
                PreBatchProcessBackgroundDivide(dataEnvironment, ImageFileListIn, ScriptParams);
                PreBatchProcessCenter(dataEnvironment, ImageFileListIn, ScriptParams);
            }

            //do any pre convolution work.  This is where most of the changes should be located
            BatchLoopThroughImagesSave(7, dataEnvironment, ImageFileListIn, ScriptParams);

            PreBatchProcessRecon(dataEnvironment, ImageFileListIn, ScriptParams);
            PostBatchProcess(dataEnvironment, ImageFileListIn, null, ScriptParams);
        }

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
            FluorImage = ((string)Variables["FluorCell"] == "True");


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
            ColorImage = dataEnvironment.ProgressLog["IsColor"].ToString().ToLower() == "true";
            DoRun(Variables);
        }

        public virtual IScript CloneScript()
        {
            return new BaseScript();
        }
    }
}

