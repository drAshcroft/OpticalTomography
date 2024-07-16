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

using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoRecons
{
    public class BaseScriptLarge : IScript
    {
        public string GetName()
        {
            return "BaseScriptSingle";
        }
        public virtual IScript CloneScript()
        {
            return new BaseScriptLarge();
        }

        protected double[] X_PositionsB;
        protected double[] Y_PositionsB;
        protected double[] X_Positions;
        protected double[] Y_Positions;
        protected double[] NumBlobs;
        protected double[] CellSizes;
        protected double[] FocusValues;
        protected double[] CellStain;

        protected double[,] BackgroundMask;
        protected Rectangle CellWanderArea;
        protected int CellWanderMargin = 120;

        protected ImageViewer.Filters.ReplaceStringDictionary GlobalPassData;
        // protected int tCellSize = 0;

        protected int RoughCellSize = 170;
        protected int RoughCellHalf = 85;

        protected int FineCellSize = 170;
        protected int FineCellHalf = 85;

        protected int ReconCellSize = 170;
        protected int ReconCellHalf = 85;

        protected bool FluorImage = true;
        protected bool LopsidedCell = false;
        protected bool CellAreadyFound = false;
        protected bool BlobsFound = false;
        #region global values

        protected string DataPath;
        protected ImageDisplayer ImageDisp;
        protected List<ImageFile> ImageFileListIn;
        protected Dictionary<string, string> ScriptParams;

        protected string TempPath;
        protected string Executable;
        protected string ExecutablePath;
        protected string LibraryPath;
        protected DataEnvironment dataEnvironment;

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

        #region Process Individual Files
        protected virtual ImageHolder ProcessImageLoad(int ImageNumber)
        {
            try
            {
                ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];

                dataEnvironment.ProgressLog.AddSafe("Num Loaded Channels", BitmapImage.NChannels);

                //make sure there is only one channel to work with
                if (BitmapImage.NChannels > 1)
                {
                    BitmapImage = ImageViewer.Filters.Adjustments.GrayScaleEffectChannel.GrayScaleFromChannel(BitmapImage, 1);
                }

                if (ColorImage == true)
                {
                    BitmapImage = MathHelpsFileLoader.FixVisionGateImage(BitmapImage, 2);
                }

                if (FluorImage == true)
                {
                    //Invert Contrast
                    BitmapImage = ImageViewer.Filters.Adjustments.InvertEffect.InvertImage(BitmapImage);
                }

                if (FluorImage == false)
                {
                    if (ScriptParams["GlobalFlatten"].ToLower() == "true")
                    {
                        BitmapImage = ImageViewer.Filters.Effects.Flattening.FlattenEdges1DErrorCorrected.FlattenImageEdges(BitmapImage);
                        //BitmapImage = ImageViewer.Filters.Effects.Flattening.FlattenEdges1D.FlattenImageAreaMedianPoly(BitmapImage, 9);
                    }
                }

                //creates a new image and then messes with the new image, so it does not affect this one.
                if (CellAreadyFound == false && BlobsFound == false)
                    ProcessImageFindRough(ImageNumber, BitmapImage, 2);

                return BitmapImage;
            }
            catch (Exception ex)
            {

                dataEnvironment.ProgressLog.AddSafe("Imageload", "error");
                MessageBox.Show(ex.StackTrace);
                throw ex;
            }
        }

        protected virtual ImageHolder ProcessImageFindRough(int ImageNumber, ImageHolder BitmapImage, int Decimate)
        {
            try
            {
                try
                {
                    //Clip Image to New Image
                    BitmapImage = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(BitmapImage, CellWanderArea);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show(ex.StackTrace);
                }
                /* ImageHolder Clipped = BitmapImage.Clone();
                 Bitmap test = Clipped.ToBitmap();
                 int h=test.Width;*/
                int Expander = Decimate;
                #region Shrinker
                if (FluorImage == true)
                    Expander = 2;
                else
                    Expander = 2;

                //this creates a new image and nothing else effects the original
                if (Expander == 2)
                {
                    //reduce size
                    BitmapImage = ImageViewer.Filters.Adjustments.downSampleEffect.DownSampleImage(BitmapImage);
                }
                else if (Expander == 4)
                {
                    BitmapImage = ImageViewer.Filters.Adjustments.downSampleEffect.DownSampleImage(BitmapImage);
                    BitmapImage = ImageViewer.Filters.Adjustments.downSampleEffect.DownSampleImage(BitmapImage);
                }
                #endregion

                #region Find Cell
                ImageHolder ThreshImage;
                BlobDescription[] Blobs = null;
                BlobDescription MaxBlob = null;
                try
                {
                    //SIS Threshold
                    //Filter =new  ImageViewer.Filters.Thresholding.SISThresholdEffect();
                    //BitmapImage =(ImageHolder ) Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                    if (FluorImage == true)
                    {
                        ThreshImage = ImageViewer.Filters.Thresholding.OtsuThresholdEffect.OtsuThreshold(BitmapImage);
                    }
                    else
                    {
                        ThreshImage = ImageViewer.Filters.Thresholding.IterativeThresholdEffect.IterativeThreshold(BitmapImage);
                    }

                    Blobs = ImageViewer.Filters.Blobs.WaterShedTool.DoWatershed(ThreshImage);
                    MaxBlob = ImageViewer.Filters.Blobs.GetBiggestCenterBlob.SortBlobsCenter(Blobs, ThreshImage.Width, ThreshImage.Height, 25, 300, out Blobs);
                }
                catch
                { }

                //try again to find the blob
                if (MaxBlob == null)
                {
                    ThreshImage = ImageViewer.Filters.Thresholding.OtsuThresholdEffect.OtsuThreshold(BitmapImage);
                    Blobs = ImageViewer.Filters.Blobs.WaterShedTool.DoWatershed(ThreshImage);
                    MaxBlob = ImageViewer.Filters.Blobs.GetBiggestCenterBlob.SortBlobsCenter(Blobs, ThreshImage.Width, ThreshImage.Height, 25, 300, out Blobs);
                }
                #endregion

                #region Process Location

                int x;
                int y;
                if (MaxBlob != null)
                {
                    try
                    {
                        BlobDescription Rect = MaxBlob;
                        x = Rect.CenterOfGravity.X * Expander;
                        y = Rect.CenterOfGravity.Y * Expander;

                        if (Rect.BlobBounds.Width > Rect.BlobBounds.Height)
                            CellSizes[ImageNumber] = Rect.BlobBounds.Width * Expander;
                        else
                            CellSizes[ImageNumber] = Rect.BlobBounds.Height * Expander;
                        //CellSizes[ImageNumber] = (Rect.BlobBounds.Width * Expander + Rect.BlobBounds.Height * Expander) / 2d;
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
                CenterCellsTool2Form.StoreBlobLocation(dataEnvironment, ImageNumber, (BlobDescription[])Blobs, Expander, CellWanderArea);

                X_PositionsB[ImageNumber] = x + CellWanderArea.Left;
                Y_PositionsB[ImageNumber] = y + CellWanderArea.Top;
                NumBlobs[ImageNumber] = Blobs.Length;

                return BitmapImage;

            }
            catch (Exception ex)
            {

                dataEnvironment.ProgressLog.AddSafe("Imagerough", "error");
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
                throw ex;
            }
        }

        protected virtual ImageHolder ProcessImageFindFine(int ImageNumber)
        {
            try
            {
                ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];

                int Xo = (int)Math.Truncate(X_PositionsB[ImageNumber] - RoughCellHalf);
                int Yo = (int)Math.Truncate(Y_PositionsB[ImageNumber] - RoughCellHalf);

                Rectangle CellArea = new Rectangle(Xo, Yo, RoughCellSize, RoughCellSize);

                CellArea.Inflate(20, 20);
                Xo = CellArea.X;
                Yo = CellArea.Y;

                try
                {
                    //Clip Image to New Image
                    BitmapImage = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(BitmapImage, CellArea);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show(ex.StackTrace);
                }

                double AveX = 0;
                double AveY = 0;

                /*  ImageHolder test = ProcessImageFindRough(ImageNumber, BitmapImage, 1);
                  X_Positions[ImageNumber] =Xo+ X_PositionsB[ImageNumber] ;
                  Y_Positions[ImageNumber] = Yo+ Y_PositionsB[ImageNumber];
                  return test;*/

                if (FluorImage == true)
                {
                    BitmapImage = ImageViewer.Filters.Adjustments.InvertEffect.InvertImage(BitmapImage);
                }

                Bitmap OriginalTests = BitmapImage.ToBitmap();

                int NumTestRuns = 4;
                Bitmap bImage = null;
                Bitmap bImage2 = null;
                BlobDescription MaxBlob = null;

                // ImageViewer.Filters.Thresholding.IterativeThresholdCornerFillEffect Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdCornerFillEffect();

                for (int i = 0; i < NumTestRuns; i++)
                {
                    try
                    {
                        //  bImage =(Bitmap) (new Bitmap(OriginalTests)).Clone();
                        bImage = (new Bitmap(OriginalTests));

                        bImage = ImageViewer.Filters.Effects.Artistic.JitterEffect.Jitter(bImage);
                        bImage = ImageViewer.Filters.Effects.Blurs.GaussianBlurTool.DoGuassSmooth(bImage, 5);
                        //  bImage2 = ImageViewer.Filters.Thresholding.IterativeThresholdEffect.IterativeThreshold(bImage);
                        bImage2 = ImageViewer.Filters.Thresholding.IterativeThresholdCornerFillEffect.IterativeThreshold(bImage);


                        BlobDescription[] Blobs = null;
                        if (ScriptParams["COGMethod"] == "Threshold")
                        {
                            Blobs = ImageViewer.Filters.Blobs.CenterOfGravityTool.DoCOG(bImage2);
                            //Blobs = ImageViewer.Filters.Blobs.CenterOfGravityTool.DoCOGCenter(bImage2);
                        }
                        else
                        {
                            //Blobs = ImageViewer.Filters.Blobs.CenterOfGravityIntensityTool.DoCOGCenter(OriginalTests, bImage2);
                            Blobs = ImageViewer.Filters.Blobs.CenterOfGravityIntensityTool.DoCOG(OriginalTests, bImage2);
                        }

                        MaxBlob = Blobs[0]; //ImageViewer.Filters.Blobs.GetBiggestBlob.GetBiggest(Blobs);

                        int x = MaxBlob.CenterOfGravity.X;
                        int y = MaxBlob.CenterOfGravity.Y;

                        AveX += x;
                        AveY += y;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        MessageBox.Show(ex.StackTrace);
                    }
                }

                try
                {
                    //figure out the average and offset the data after the clip
                    AveX = Xo + AveX / (double)NumTestRuns;
                    AveY = Yo + AveY / (double)NumTestRuns;

                    if (MaxBlob.BlobBounds.Width > MaxBlob.BlobBounds.Height)
                        CellSizes[ImageNumber] = MaxBlob.BlobBounds.Width;
                    else
                        CellSizes[ImageNumber] = MaxBlob.BlobBounds.Height;

                    if (Math.Abs(1 - MaxBlob.BlobBounds.Width / (double)MaxBlob.BlobBounds.Height) > .1)
                        LopsidedCell = true;

                    if (Math.Abs(X_Positions[ImageNumber] - AveX) < 10)
                        X_Positions[ImageNumber] = AveX;

                    if (Math.Abs(Y_Positions[ImageNumber] - AveY) < 10)
                        Y_Positions[ImageNumber] = AveY;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show(ex.StackTrace);

                }
                return new ImageHolder(bImage2);
            }
            catch (Exception ex)
            {
                dataEnvironment.ProgressLog.AddSafe("Imagefine", "error");
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
                throw ex;
            }
        }

        protected virtual ImageHolder ProcessImageDivide(int ImageNumber)
        {

            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];
            try
            {
                Rectangle CellArea = new Rectangle((int)(X_PositionsB[ImageNumber] - RoughCellHalf), (int)(Y_PositionsB[ImageNumber] - RoughCellHalf), RoughCellSize, RoughCellSize);

                if (FluorImage == false)
                {
                    Rectangle CellAreaPadded = new Rectangle(CellArea.Location, CellArea.Size);
                    CellAreaPadded.Inflate(CellArea.Width, CellArea.Height);

                    ImageViewer.Filters.Effects.Flattening.DivideImage.DivideOneImageByAnother(BitmapImage, BackgroundMask, CellAreaPadded);
                }
                else
                {
                    BitmapImage = ImageViewer.Filters.Adjustments.InvertEffect.InvertImage(BitmapImage);
                }


                return BitmapImage;
            }
            catch (Exception ex)
            {
                dataEnvironment.ProgressLog.AddSafe("IamgeDivide", "error");
                string[] lines = ex.StackTrace.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                dataEnvironment.ProgressLog.AddSafe("Debug", lines[0].ToString());
                MessageBox.Show(ex.StackTrace);
                throw ex;
            }
        }

        protected virtual ImageHolder ProcessImageClip(int ImageNumber)
        {
            try
            {
                ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];
                //  Bitmap b1 = BitmapImage.ToBitmap();
                Rectangle CellArea = new Rectangle((int)Math.Truncate(X_Positions[ImageNumber] - FineCellHalf), (int)Math.Truncate(Y_Positions[ImageNumber] - FineCellHalf), FineCellSize, FineCellSize);

                BitmapImage = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(BitmapImage, CellArea);
                //  Bitmap test = BitmapImage.ToBitmap();
                //  int w= test.Width;
                //  int w2 = b1.Width;
                BitmapImage = ImageViewer.Filters.Adjustments.InvertEffect.InvertImage(BitmapImage);

                // BitmapImage.Save(@"C:\Development\CellCT\testimages\image" + ImageNumber.ToString() + ".bmp");
                if (ScriptParams["FlatMethod"].ToLower() == "plane")
                {
                    BitmapImage = ImageViewer.Filters.Effects.Flattening.FlattenEdges.FlattenImageEdges(BitmapImage);
                }
                if (ScriptParams["FlatMethod"].ToLower() == "curve")
                {
                    // BitmapImage = ImageViewer.Filters.Effects.Flattening.FlattenEdges1D.FlattenImageEdgesMedianPoly(BitmapImage,15);
                    BitmapImage = ImageViewer.Filters.Effects.Flattening.FlattenEdges1D.FlattenImageEdgesMedianMedian(BitmapImage, 15);
                }

                #region Focus Value

                double FocusValue = FocusValueTool.FocusValueF4(BitmapImage);
                FocusValues[ImageNumber] = FocusValue;

                CellStain[ImageNumber] = BitmapImage.ImageData.AverageArrayWithThreshold(100);
                #endregion

                //  Console.WriteLine("After5 " + BitmapImage.Max());

                if (FluorImage == true)
                {
                    BitmapImage = ImageViewer.Filters.Adjustments.InvertEffect.InvertImage(BitmapImage);
                }
                // Console.WriteLine("After6 " + BitmapImage.Max());
                return BitmapImage;
            }
            catch (Exception ex)
            {
                dataEnvironment.ProgressLog.AddSafe("Imageclip", "error");
                MessageBox.Show(ex.StackTrace);
                throw ex;
            }
        }

        protected virtual ImageHolder ProcessBeforeConvolution(int ImageNumber)
        {
            try
            {
                ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];

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
                int Radius = (int)EffectHelps.ConvertToDouble(ScriptParams["ProprocessingRadius"]);

                if (Method == "median")
                {
                    BitmapImage = ImageViewer.Filters.Effects.RankOrder.MedianFilterTool.MedianFilter(BitmapImage, Radius);
                }
                else if (Method == "average")
                {
                    BitmapImage = ImageViewer.Filters.Effects.RankOrder.AverageFilterTool.AverageFilter(BitmapImage, Radius);
                }
                else if (Method == "alphatrimmed")
                {
                    BitmapImage = ImageViewer.Filters.Effects.RankOrder.AlphaTrimmedMeanFilterTool.AlphaTrimmedFilter(BitmapImage, Radius, 50);
                }
                else if (Method == "opening")
                {
                    BitmapImage = ImageViewer.Filters.Effects.Morphology.OpeningTool.Opening(BitmapImage, Radius);
                }
                else if (Method == "closing")
                {
                    BitmapImage = ImageViewer.Filters.Effects.Morphology.ClosingTool.Closing(BitmapImage, Radius);
                }

                return BitmapImage;
            }
            catch (Exception ex)
            {
                dataEnvironment.ProgressLog.AddSafe("Imagebeforeconvolution", "error");
                MessageBox.Show(ex.StackTrace);
                throw ex;
            }
        }

        bool DoDoubleProjection = false;
        protected virtual ImageHolder ProcessConvolution(int ImageNumber)
        {
            try
            {
                double[,] data = DoSliceConvolution(dataEnvironment.AllImages[ImageNumber]);

                double Angle = (double)ImageNumber / dataEnvironment.AllImages.Count * Math.PI * 2;
                if (DoDoubleProjection)
                    ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.DoOneDoubleProjection(data, Angle);
                else
                    ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.DoOneProjection(data, Angle);
                return dataEnvironment.AllImages[ImageNumber];
            }
            catch (Exception ex)
            {
                dataEnvironment.ProgressLog.AddSafe("Convolution", "error");
                MessageBox.Show(ex.StackTrace);
                throw ex;
            }
        }

        #region FBP
        protected string ReconInterpolationMethod = "Siddon";
        protected virtual double[,] DoSliceConvolution(ImageHolder BitmapImage)
        {
            double[,] Slice = null;
            if (ConvolutionMethod.Convolution1D == DesiredMethod)
            {
                Slice = ConvolutionFilter.DoConvolution(dataEnvironment, BitmapImage, impulse, 2, 8);
            }
            else
            {
                BitmapImage = ImageViewer.PythonScripting.Programming_Tools.PadImageToNewEffect.PadImage(BitmapImage, new Rectangle(0, 0, 256, 256));
            }


            if (ScriptParams["FBPMedian"].ToLower() == "true")
            {
                Slice = ImageViewer.Filters.Effects.RankOrder.MedianFilterTool.MedianFilter(Slice, 3);
            }

            Slice = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(Slice, ReconCutDownRect);
            return Slice;//.RotateArray();
            //return Slice;
        }

        protected Rectangle ReconCutDownRect;
        protected virtual ImageHolder ProcessImageFBP(int ImageNumber)
        {
            try
            {
                ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];

                double[,] Slice = DoSliceConvolution(BitmapImage);

                double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * ImageNumber;


                if (ReconInterpolationMethod == "Linear")
                {
                    dataEnvironment.RunningOnGPU = false;
                    if (dataEnvironment.RunningOnGPU == false)
                    {
                        Slice = ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.DoInterpolation(Slice, 1, 1, DensityGrid.DataWhole, 1, 1, AngleRadians, 20, ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.InterpolationMethod.Cosine);
                    }

                    ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect();
                    Filter.DoEffect(dataEnvironment, BitmapImage, null, Slice, DensityGrid, AngleRadians);
                }
                else if (ReconInterpolationMethod == "Siddon")
                {
                    Bitmap b = MathHelpLib.ImageProcessing.MathImageHelps.MakeBitmap(Slice);
                    ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2 Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2();
                    Filter.DoEffect(dataEnvironment, BitmapImage, null, Slice, DensityGrid, AngleRadians);
                }
                else if (ReconInterpolationMethod == "Gaussian")
                {
                    ImageViewer.PythonScripting.Projection.DoSliceBackProjectionGaussianEffect Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionGaussianEffect();
                    Filter.DoEffect(dataEnvironment, BitmapImage, null, Slice, DensityGrid, AngleRadians);
                }

                return new ImageHolder(Slice);
            }
            catch (Exception ex)
            {
                dataEnvironment.ProgressLog.AddSafe("fbp", "error");
                MessageBox.Show(ex.StackTrace);
                throw ex;
            }
        }
        #endregion

        #endregion


        //////////////////////////////////////////////////////////////  Cell Finding and preproessing
        #region  Find Cell and Cell size
        protected void FindFirstCell()
        {
            // if (File.Exists(dataEnvironment.ExperimentFolder + "\\info.xml") == true)
            {
                //   CellWanderArea = DoRecons.PythonHelps.GetCellAreaFromInfoFile(dataEnvironment.ExperimentFolder + "\\info.xml", dataEnvironment.AllImages[1].Width, dataEnvironment.AllImages[1].Height);
            }
            //else
            {
                int[] Indexs = new int[6];
                for (int i = 0; i < Indexs.Length; i++)
                {
                    Indexs[i] = (int)(dataEnvironment.AllImages.Count * (double)i / (double)Indexs.Length);
                    ProcessImageLoad(Indexs[i]);
                }
                CellWanderArea = CenterCellsTool2Form.FindBestCellCenter(dataEnvironment, Indexs);
                if (CellWanderArea.X < 100 || CellWanderArea.Width > 900)
                {
                    int width = dataEnvironment.AllImages[1].Width;
                    int height = dataEnvironment.AllImages[1].Height;
                    CellWanderArea = new Rectangle((int)(width * .42), 0, (int)(width * .16), height);

                }

            }

        }

        protected virtual void FindCell()
        {
           // if (CellAreadyFound == false)
                FindFirstCell();

            dataEnvironment.ProgressLog.AddSafe("Prep", "Load Image");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList

            dataEnvironment.ProgressLog.AddSafe("Position", "starting process Rough");
            BatchLoopThroughImagesSave("LoadImages");

            //if (BlobsFound == false)
           //     ImageViewer.Filters.CenterCellsTool2Form.SaveBlobs(dataEnvironment);

            RoughCellSize = (int)(PickTheRightCell() * 1.2);
            RoughCellHalf = (int)(RoughCellSize / 2d);

            /*  if (CellAreadyFound)
              {
                  X_PositionsB = X_Positions;
                  Y_PositionsB = Y_Positions;
              }*/
        }

        protected virtual double PickTheRightCell()
        {
           
                double tCellSize = CellSizes[0];
                for (int i = 0; i < CellSizes.Length; i++)
                {
                    if (tCellSize < CellSizes[i]) tCellSize = CellSizes[i];// tCellSize += CellSizes[i];
                }
                //tCellSize /= CellSizes.Length;

                //Dictionary<string, double[]> NewLines = ImageViewer.Filters.CenterCellsTool2Form.PreFitLines2(dataEnvironment, X_PositionsB, Y_PositionsB, out tCellSize);

                //if (CellAreadyFound == false)
                {
                    /*    for (int i = 0; i < X_Positions.Length; i++)
                        {
                            X_Positions[i] = X_PositionsB[i];
                            Y_Positions[i] = Y_PositionsB[i];
                        }*/
                  //  X_Positions = NewLines["UpdatedX"];
                   // Y_Positions = NewLines["UpdatedY"];
                }
                //else
                {
                  //  X_PositionsB = new double[dataEnvironment.AllImages.Count];
                   // Y_PositionsB = new double[dataEnvironment.AllImages.Count];
                    for (int i = 0; i < X_Positions.Length; i++)
                    {
                        X_Positions[i] = X_PositionsB[i];
                        Y_Positions[i] = Y_PositionsB[i];
                    }
                }

                return tCellSize;
           
        }
        #endregion

        /// ////////////////////////////////////////////////////    Finding the background 

        protected object DehyrateLock = new object();
        protected bool SaveDehydrated = false;

        #region Remove Background
        protected bool ExistingBackgroundOnly = true;
        protected bool TopAndBottomMask = true;
        protected virtual void FindBackground(double CellSize)
        {
            ImageViewer.Filters.StationaryPixelsForm Filter = null;
            ReplaceStringDictionary PassData = new ReplaceStringDictionary();
            ImageHolder BitmapImage = null;
            bool BackgroundHelp = false;
            try
            {

                if (File.Exists(Path.GetDirectoryName(ScriptParams["BackFolder"]) + "\\Background.tif") == true && ExistingBackgroundOnly)
                {//Background.tif
                    // MessageBox.Show("Found background");
                    dataEnvironment.ProgressLog.AddSafe("backgroundRemovalMethod", "load background");
                    BackgroundHelp = true;
                    PassData.AddSafe("BackgroundMask",
                        MathHelpsFileLoader.Load_Bitmap(Path.GetDirectoryName(ScriptParams["BackFolder"]) + "\\Background.tif").ToDataIntensityDouble());
                }
                else if (ExistingBackgroundOnly)
                {
                    // MessageBox.Show("no background");
                    dataEnvironment.ProgressLog.AddSafe("backgroundRemovalMethod", "TopAndBottomMask");

                    Filter = new ImageViewer.Filters.StationaryPixelsForm(true);
                    BitmapImage = Filter.FindStationaryPixels(dataEnvironment, PassData, 100, ScriptParams["BackgroundSubMethod"], X_PositionsB, Y_PositionsB, false, "Divide", ThreadNumber, (int)(CellSize * 1.05), true, ImageFileListIn);
                }
                else
                {
                    // MessageBox.Show("wrong spot");
                    if (TopAndBottomMask)
                    {
                        dataEnvironment.ProgressLog.AddSafe("backgroundRemovalMethod", "TopAndBottomMask");
                        Filter = new ImageViewer.Filters.StationaryPixelsForm(true);
                        BitmapImage = Filter.FindStationaryPixels(dataEnvironment, PassData, 100, ScriptParams["BackgroundSubMethod"], X_PositionsB, Y_PositionsB, false, "Divide", ThreadNumber, 100, true, ImageFileListIn);
                        //BitmapImage = (ImageHolder)Filter.DoEffect2(dataEnvironment, null, GlobalPassData, 100, ScriptParams["BackgroundSubMethod"], X_PositionsB, Y_PositionsB, false, "Divide", RoughCellSize, true);
                    }
                    else
                    {
                        dataEnvironment.ProgressLog.AddSafe("backgroundRemovalMethod", "Median Background");
                        // if (ScriptParams["StrictBackground"].ToLower() == "true")
                        {
                            BitmapImage = ImageViewer.Filters.StationaryPixelsForm.AverageBackground(dataEnvironment, X_PositionsB, Y_PositionsB,/*100*/ (int)(CellSize * 1.05), false);
                        }
                        //  else
                        //     BitmapImage = ImageViewer.Filters.StationaryPixelsForm.MedianBackground(dataEnvironment, X_PositionsB, Y_PositionsB, (int)(CellSize * 1.05), true);

                        PassData.AddSafe("BackgroundMask", BitmapImage.ToDataIntensityDouble());
                    }
                }
            }
            catch (Exception ex)
            {
                /*
                try
                {
                    throw ex;
                    //fluor images just dont divide clean
                    if (FluorImage == false)
                    {
                        BackgroundHelp = true;
                        // MessageBox.Show(Path.GetDirectoryName(ScriptParams["BackFolder"]) + "\\Background.tif");
                        if (File.Exists(Path.GetDirectoryName(ScriptParams["BackFolder"]) + "\\Background.tif") == true)
                        {//Background.tif

                            PassData.AddSafe("BackgroundMask",
                                MathHelpsFileLoader.Load_Bitmap(Path.GetDirectoryName(ScriptParams["BackFolder"]) + "\\Background.tif").ToDataIntensityDouble());
                        }
                        else
                            throw ex;
                    }
                    else
                    {
                    }
                }
                catch (Exception ex2)
                {
                    /*  MessageBox.Show(ex2.Message);
                      MessageBox.Show(Path.GetDirectoryName(ScriptParams["BackFolder"]) + "\\Background.tif" + "\n" + ex2.Message + "\n" + ex2.StackTrace);
                    dataEnvironment.ProgressLog.AddSafe("backerror", Path.GetDirectoryName(ScriptParams["BackFolder"]) + "\\Background.tif" + "\n" + ex2.Message + "\n" + ex2.StackTrace);
                    throw new Exception(Path.GetDirectoryName(ScriptParams["BackFolder"]) + "\\Background.tif" + "\n" + ex2.Message + "\n" + ex2.StackTrace);

                }*/

                double[,] Back = new double[dataEnvironment.AllImages[1].Width,dataEnvironment.AllImages[1].Height ];
                for (int i=0;i<Back.GetLength(0);i++)
                    for (int j=0;j<Back.GetLength(1);j++)
                        Back[i,j]=1;

                PassData.AddSafe("BackgroundMask", Back);
                BitmapImage = new ImageHolder(Back);
            }

            dataEnvironment.ProgressLog.AddSafe("Position", "saving stationary pixels");

            #region Save background info

            if (FluorImage == false)
            {
                MathHelpsFileLoader.Save_Raw(DataPath + "Background.tif", (double[,])PassData["BackgroundMask"]);
            }

            if (BackgroundHelp == false)
            {
                Bitmap b = BitmapImage.ToBitmap();
                MathHelpsFileLoader.Save_Bitmap(DataPath + "Background.bmp", b);// BitmapImage);
                try
                {
                    MathHelpsFileLoader.Save_Raw(ScriptParams["BackFolder"], (double[,])PassData["BackgroundMask"]);
                }
                catch { }
            }

            BackgroundMask = (double[,])PassData["BackgroundMask"];
            double maxBlobs = max(NumBlobs);
            dataEnvironment.ProgressLog.AddSafe("NumberOfCells", maxBlobs.ToString());
            #endregion



            //RoughCellSize = (int)((int)PassData["MaxCellSize"]);
            //RoughCellHalf = RoughCellSize / 2;
            CellWanderMargin = RoughCellSize;

            print("Rough Cell Size" + RoughCellSize.ToString());

            int XMin = (int)(min(X_PositionsB) - CellWanderMargin);
            int YMin = (int)(min(Y_PositionsB) - CellWanderMargin);
            int XMax = (int)(max(X_PositionsB) + CellWanderMargin);
            int YMax = (int)(max(Y_PositionsB) + CellWanderMargin);

            CellWanderArea = new Rectangle(XMin, YMin, XMax - XMin, YMax - YMin);

            try
            {

                ImagingTools.ClipImage(dataEnvironment.AllImages[0].ToBitmap(), CellWanderArea).Save(DataPath + "projection1.jpg");
                ImagingTools.ClipImage(dataEnvironment.AllImages[(int)(dataEnvironment.AllImages.Count / 4d)].ToBitmap(), CellWanderArea).Save(DataPath + "projection2.jpg");
                ImagingTools.ClipImage(dataEnvironment.AllImages[(int)(dataEnvironment.AllImages.Count / 2d)].ToBitmap(), CellWanderArea).Save(DataPath + "projection3.jpg");
                ImagingTools.ClipImage(dataEnvironment.AllImages[(int)(dataEnvironment.AllImages.Count * 3d / 4d)].ToBitmap(), CellWanderArea).Save(DataPath + "projection4.jpg");
            }
            catch { }

            print(CellWanderArea);
            //  return;
        }

        protected virtual void RemoveBackground()
        {
            dataEnvironment.ProgressLog.AddSafe("Position", "starting stationary pixels");

            FindBackground(RoughCellSize);

            //  return;
            dataEnvironment.ProgressLog.AddSafe("Position", "Dividing away background");

            BatchLoopThroughImagesSave("Divide");

            dataEnvironment.ProgressLog.AddSafe("Divide", "Divide Succeeded");

        }
        #endregion
        //*****************************************************   Final Centering and clipping


        protected virtual void PreBatchProcessCenter()
        {
            ReplaceStringDictionary PassData = new ReplaceStringDictionary();
            dataEnvironment.ProgressLog.AddSafe("Prep", "Center");

           // ImageViewer.Filters.CenterCellsTool2Form.ReconCenter(dataEnvironment, RoughCellSize, X_Positions, Y_Positions);

            BatchLoopThroughImages("FindFine");

            double tCellSize = RoughCellSize;
            if (LopsidedCell)
            {
                tCellSize = CellSizes[0];
                for (int i = 0; i < CellSizes.Length; i++)
                {
                    if (CellSizes[i] > tCellSize) tCellSize = CellSizes[i];
                }
            }
            else
            {
                tCellSize = CellSizes[0];
                for (int i = 1; i < CellSizes.Length; i++)
                {
                    tCellSize += CellSizes[i];
                }
                tCellSize /= CellSizes.Length;
            }

            FineCellSize = (int)(tCellSize * 1.5) + 20;
            FineCellHalf = FineCellSize / 2;

            ReconCellSize = (int)(.8 * FineCellSize);// (int)(tCellSize * 1.3) + 20;
            ReconCellHalf = ReconCellSize / 2;


            dataEnvironment.ProgressLog.AddSafe("Position", "Centering Fit");
            //Center Cells
            ImageViewer.Filters.CenterCellsTool2Form Filter = new ImageViewer.Filters.CenterCellsTool2Form();

            //Parameters required: Bitmap_Filenames as string[], X_Positions as int[], Y_Positions as int[], SmoothingTypeX as string, X_Smooth_Param as int, SmootingTypeY as string, Y_Smooth_Param as int, ShowForm as string, CutSize as Size, OptionalOutputDir as string
            // Filter.DoEffect2(dataEnvironment, null, GlobalPassData, 100, X_Positions, Y_Positions, "MovingAverage", 5, "MovingAverage", 5, false, new Size(FineCellSize, FineCellSize), true, TempPath);
            //Data out of type :
            //  PassData = Filter.PassData;
            //

            dataEnvironment.ProgressLog.AddSafe("Centering", "Centering Line Created");
            try
            {
                dataEnvironment.ProgressLog.AddSafe("CenteringQualityActual", PassData["CenterAccuracyActual"].ToString() + "%");
            }
            catch { }
            dataEnvironment.ProgressLog.AddSafe("CenteringQuality", "100%"/* PassData["CenterAccuracy"].ToString() + "%"*/);


            //  X_Positions = (double[])PassData["CorrectedX"];
            // Y_Positions = (double[])PassData["CorrectedY"];

            ImageViewer.Filters.CenterCellsTool2Form.SaveCenters(dataEnvironment, X_Positions, Y_Positions);
            //  return;

            dataEnvironment.ProgressLog.AddSafe("Position", "Clipping");

            BatchLoopThroughImagesSave("ClipImages");

            dataEnvironment.ProgressLog.AddSafe("Clipping", "Images Clipped");

            dataEnvironment.ProgressLog.AddSafe("Cell Staining Average", Average(CellStain));
            dataEnvironment.ProgressLog.AddSafe("Cell Staining Variance", max(CellStain) - min(CellStain));

            if (ScriptParams["SaveCenteringMovie"].ToLower() == "true")
            {
                ImagingTools.CreateAVIVideo(DataPath + "Centering.avi", dataEnvironment.AllImages, 10);
            }
        }

        protected ImageViewer.PythonScripting.Projection.Convolution1D ConvolutionFilter;

        protected virtual void PreBatchProcessRecon()
        {
            GC.Collect();
            dataEnvironment.ProgressLog.AddSafe("Prep", "Recon");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList

            ReplaceStringDictionary PassData = new ReplaceStringDictionary();

            dataEnvironment.ProgressLog.AddSafe("Position", "Getting Impulse");

            int FilterWidth = EffectHelps.ConvertToInt(ScriptParams["FBPResolution"]);//, EffectHelps.ConvertToInt(ParamDictionary["FBPResolution"])
            impulse = Filtering.GetRealSpaceFilter(ScriptParams["FBPWindow"], FilterWidth, FilterWidth, (double)FilterWidth / 2d);


            ImageHolder BitmapImage = dataEnvironment.AllImages[1];

            //create a densitygrid if this is the first run, otherwise just pull the already created grid
            IEffect Filter;
            Filter = new ImageViewer.PythonScripting.Projection.CreateFilteredBackProjectionEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ReconCellSize, ReconCellSize, 2, 2, 1, false, true);
            DensityGrid = (ProjectionArrayObject)Filter.PassData["FBJObject"];


            ReconCutDownRect = new Rectangle((int)((FineCellSize - ReconCellSize) / 2), (int)((FineCellSize - ReconCellSize) / 2), ReconCellSize, ReconCellSize);

            dataEnvironment.ProgressLog.AddSafe("Position", "Doing Convolution");

            DesiredMethod = ConvolutionMethod.Convolution1D;

            ConvolutionFilter = new ImageViewer.PythonScripting.Projection.Convolution1D();

            ReconInterpolationMethod = ScriptParams["InterpolationMethod"];

            string[] Platforms = ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.AvailablePlatforms();

            // if (DensityGrid.DataWhole.GetLength(0) > 10)
            {
                BatchLoopThroughImages("DoFBPProjection");

                //ProcessImageFBP(0);
                /*DoDoubleProjection = true;
                ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.SetupDoubleGPU(Platforms[0]);
                ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.SetDouble3DBuffer(DensityGrid.DataWhole);

                ProcessConvolution(0);
              // BatchLoopThroughImagesSave("Convolution");
                //ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.SetImpulse(impulse);
                //ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.DoProjections(dataEnvironment, DensityGrid.DataWhole);
                ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.ReadResultDouble( DensityGrid.DataWhole);*/
            }
            /*   else
               {
                   DoDoubleProjection = false;
                   ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.SetupGPU(Platforms[0], 0);
                   ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.Set3DBuffer(DensityGrid.DataWhole);
                   BatchLoopThroughImagesSave("Convolution");
                   //ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.SetImpulse(impulse);
                   //ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.DoProjections(dataEnvironment, DensityGrid.DataWhole);
                   ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.ReadResult(ref DensityGrid.DataWhole);
               }*/

            dataEnvironment.ProgressLog.AddSafe("Recon", "Recon Completed");

            dataEnvironment.ProgressLog.AddSafe("Position", "Creating Mip DIR");

            GC.Collect();
        }

        #region Post Process
        protected virtual void WaitForFinish()
        {
            //Join Threads
            ImageViewer.PythonScripting.Threads.JoinThreadsTool.JoinThreads(dataEnvironment, "FinishEverything", ThreadNumber);
            dataEnvironment.ProgressLog.AddSafe("Debug", "Reached home" + Thread.CurrentThread.ManagedThreadId.ToString());
        }

        protected virtual void NormalizedImage()
        {
            double CutValue = -500;
            if (DensityGrid.Data != null)
                ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder.DoRemoveFBPCylinder(ref DensityGrid.Data);
            else
                CutValue = ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder.DoRemoveFBPCylinder(ref DensityGrid.DataWhole);

            if (DensityGrid.Data != null)
                ImageViewer3D.PythonScripting.CellCT.NormalizeFBPVolumeEffect.NormalizeFBPVolume(ref DensityGrid.Data, dataEnvironment.AllImages.Count, true);
            else
                ImageViewer3D.PythonScripting.CellCT.NormalizeFBPVolumeEffect.NormalizeFBPVolume(ref DensityGrid.DataWhole, dataEnvironment.AllImages.Count, true, CutValue);

            //dataEnvironment.ProgressLog.AddSafe("Volume Average", average);

            ///There is an ungly ring around the array.  This needs to be removed



        }

        protected virtual void SaveVolume()
        {
            /*if (Directory.Exists(DataPath + "VirtualStack\\") == false)
            {
                Directory.CreateDirectory(DataPath + "VirtualStack\\");
            }
            if (ScriptParams["SaveAsCCT"].ToLower() == "true")
            {
                DensityGrid.SaveFile(DataPath + "VirtualStack\\ProjectionObject.tif");
            }*/
            if (ScriptParams["SaveAsCCT"].ToLower() == "true")
            {

                MathHelpsFileLoader.Save_Tiff_Stack(DataPath + "ProjectionObject.tif", DensityGrid.DataWhole);
                DensityGrid.SaveFile(DataPath + "ProjectionObject.tif");
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
                else
                {
                    Directory.Delete(DataPath + "VirtualStack8\\", true);
                    Directory.CreateDirectory(DataPath + "VirtualStack8\\");
                }
                MathHelpsFileLoader.Save_Tiff_VirtualStack(DataPath + "VirtualStack8\\VStack.tif", DensityGrid.DataWhole, -20, 8);
                //DensityGrid.SaveFile(DataPath + "VirtualStack8\\VStack.tif", 8);
            }
            if (ScriptParams["Save16Bit"].ToLower() == "true")
            {
                if (Directory.Exists(DataPath + "VirtualStack16\\") == false)
                    Directory.CreateDirectory(DataPath + "VirtualStack16\\");
                else
                {
                    Directory.Delete(DataPath + "VirtualStack16\\", true);
                    Directory.CreateDirectory(DataPath + "VirtualStack16\\");
                }
                MathHelpsFileLoader.Save_Tiff_VirtualStack(DataPath + "VirtualStack16\\VStack.tif", DensityGrid.DataWhole, -20, 16);
                //DensityGrid.SaveFile(DataPath + "VirtualStack16\\ProjectionObject.tif");
                //DensityGrid.SaveFile(DataPath + "VirtualStack16\\VStack.tif", 16);
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
                double Ave = Average(FocusValues);
                dataEnvironment.ProgressLog.AddSafe("FocusValueAverage", Ave.ToString());
                dataEnvironment.ProgressLog.AddSafe("FocusValueSD", (Stdev(FocusValues, Ave) / Ave * 100d).ToString());
                dataEnvironment.ProgressLog.AddSafe("FocusValueMax", max(FocusValues).ToString());
                dataEnvironment.ProgressLog.AddSafe("FocusValueMin", min(FocusValues).ToString());

                dataEnvironment.ProgressLog.AddSafe("FocusValue", Ave.ToString());
                dataEnvironment.ProgressLog.AddSafe("FocusVar", ((max(FocusValues) - min(FocusValues)) / Ave).ToString());
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


            if (ScriptParams["SaveMIP"].ToLower() == "true")
            {
                try
                {
                    ImageViewer.PythonScripting.Projection.MakeMIPMovie3Effect Filter = new ImageViewer.PythonScripting.Projection.MakeMIPMovie3Effect();
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
            /*
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;
            if (ScriptParams["SaveCenteringMovie"].ToLower() == "true")
            {
                try
                {
                    ImageViewer.ImagingTools.CreateAVIVideoEMGU(DataPath + "Centering.avi", Directory.GetFiles(TempPath + "CenterMovie", "*.jpg"));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            }*/
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
            //do the image quality tests
            if (ScriptParams["DoConvolutionQuality"].ToLower() == "true")
            {
                try
                {
                    dataEnvironment.ProgressLog.AddSafe("Position", "ImageQuality");
                    MathHelpLib.ProjectionFilters.ReconQualityCheckTool.CompareProjection(DensityGrid.DataWhole, dataEnvironment.ExperimentFolder, null);
                }
                catch { }
            }
        }

        //this is called once after the whole set has been processed
        protected virtual void PostBatchProcess()
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Post");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            WaitForFinish();

            //allow the gpu to definitely finish
            Thread.Sleep(2000);
            //  if (ThreadNumber == 0)
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
                SaveVolume();
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
            dataEnvironment.ProgressLog.AddSafe("Background", "New_Background");
            ///the resources should be cleaned up on the calling thread
            ConvolutionFilter.Dispose();
            ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect.Dispose(dataEnvironment);
        }
        #endregion

        #region Batchloop
        private object CriticalSection = new object();

        #region BatchSave
        protected void BatchSaveFine(int ImageIndex)
        {
            ImageHolder ih = ProcessImageFindFine(ImageIndex);
            dataEnvironment.AllImages[ImageIndex] = ih;
            ImageDisp.DisplayImage(ImageIndex, ih);
        }


        protected void BatchSaveLoad(int ImageIndex)
        {
            ImageHolder ih = ProcessImageLoad(ImageIndex);

            dataEnvironment.AllImages[ImageIndex] = ih;
            ImageDisp.DisplayImage(ImageIndex, ih);
        }

        protected void BatchSaveDivide(int ImageIndex)
        {
            ImageHolder ih = ProcessImageDivide(ImageIndex);

            dataEnvironment.AllImages[ImageIndex] = ih;
            ImageDisp.DisplayImage(ImageIndex, ih);
        }

        protected void BatchSaveClip(int ImageIndex)
        {
            ImageHolder ih = ProcessImageClip(ImageIndex);

            dataEnvironment.AllImages[ImageIndex] = ih;
            ImageDisp.DisplayImage(ImageIndex, ih);
        }

        protected void BatchSaveFBP(int ImageIndex)
        {
            ImageHolder ih = ProcessImageFBP(ImageIndex);

            dataEnvironment.AllImages[ImageIndex] = ih;
            ImageDisp.DisplayImage(ImageIndex, ih);
        }

        protected void BatchSaveBeforeConvolution(int ImageIndex)
        {
            ImageHolder ih = ProcessBeforeConvolution(ImageIndex);

            dataEnvironment.AllImages[ImageIndex] = ih;
            ImageDisp.DisplayImage(ImageIndex, ih);
        }

        protected void BatchSaveConvolution(int ImageIndex)
        {
            ImageHolder ih = ProcessConvolution(ImageIndex);
            dataEnvironment.AllImages[ImageIndex] = ih;
            ImageDisp.DisplayImage(ImageIndex, ih);
        }


        protected void BatchLoopThroughImagesSave(string IProcessFunction)
        {
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = Environment.ProcessorCount - 1;

            if (IProcessFunction == "FindFine")
                Parallel.For(0, ImageFileListIn.Count, po, x => BatchSaveFine(x));
            else if (IProcessFunction == "Divide")
                Parallel.For(0, ImageFileListIn.Count, po, x => BatchSaveDivide(x));
            else if (IProcessFunction == "ClipImages")
                Parallel.For(0, ImageFileListIn.Count, po, x => BatchSaveClip(x));
            else if (IProcessFunction == "DoFBPProjection")
                Parallel.For(0, ImageFileListIn.Count, po, x => BatchSaveFBP(x));
            else if (IProcessFunction == "LoadImages")
                Parallel.For(0, ImageFileListIn.Count, po, x => BatchSaveLoad(x));
            else if (IProcessFunction == "BeforeConvolution")
                Parallel.For(0, ImageFileListIn.Count, po, x => BatchSaveBeforeConvolution(x));
            else if (IProcessFunction == "Convolution")
                Parallel.For(0, ImageFileListIn.Count, po, x => BatchSaveConvolution(x));

            Thread.Sleep(0);

        }
        #endregion

        #region BatchLoop

        protected void BatchFine(int ImageIndex)
        {
            ImageDisp.DisplayImage(ImageIndex, ProcessImageFindFine(ImageIndex));
        }


        protected void BatchLoad(int ImageIndex)
        {
            ImageDisp.DisplayImage(ImageIndex, ProcessImageLoad(ImageIndex));
        }

        protected void BatchDivide(int ImageIndex)
        {
            ImageDisp.DisplayImage(ImageIndex, ProcessImageDivide(ImageIndex));
        }

        protected void BatchClip(int ImageIndex)
        {
            ImageDisp.DisplayImage(ImageIndex, ProcessImageClip(ImageIndex));
        }

        protected void BatchFBP(int ImageIndex)
        {
            ImageDisp.DisplayImage(ImageIndex, ProcessImageFBP(ImageIndex));
        }

        protected void BatchBeforeConvolution(int ImageIndex)
        {
            ImageDisp.DisplayImage(ImageIndex, ProcessBeforeConvolution(ImageIndex));
        }

        protected void BatchLoopThroughImages(string IProcessFunction)
        {
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = Environment.ProcessorCount - 1;
            if (IProcessFunction == "FindFine")
                Parallel.For(0, ImageFileListIn.Count, po, x => BatchFine(x));
            else if (IProcessFunction == "Divide")
                Parallel.For(0, ImageFileListIn.Count, po, x => BatchDivide(x));
            else if (IProcessFunction == "ClipImages")
                Parallel.For(0, ImageFileListIn.Count, po, x => BatchClip(x));
            else if (IProcessFunction == "DoFBPProjection")
                Parallel.For(0, ImageFileListIn.Count, po, x => BatchFBP(x));
            else if (IProcessFunction == "LoadImages")
                Parallel.For(0, ImageFileListIn.Count, po, x => BatchLoad(x));
            else if (IProcessFunction == "BeforeConvolution")
                Parallel.For(0, ImageFileListIn.Count, po, x => BatchBeforeConvolution(x));

            Thread.Sleep(0);
        }
        #endregion
        #endregion

        protected virtual void DoRun(Dictionary<string, object> Variables)
        {
            /*
            BlobsFound = ImageViewer.Filters.CenterCellsTool2Form.OpenBlobs(dataEnvironment, out X_PositionsB, out Y_PositionsB, out CellSizes);
            CellAreadyFound = ImageViewer.Filters.CenterCellsTool2Form.OpenCenters(dataEnvironment, out X_Positions, out Y_Positions, out RoughCellSize);
            */

            //ColorImage = false;

            //format loaded images (i.e. select only one channel) //now done inside find rough
            //  BatchLoopThroughImagesSave(6, dataEnvironment, ImageFileListIn, ScriptParams);

            CellWanderArea = new Rectangle(0, 0, dataEnvironment.AllImages[1].Width, dataEnvironment.AllImages[1].Height);


            dataEnvironment.ProgressLog.AddSafe("Run Time", DateTime.Now.ToString());

            if ((Variables.ContainsKey("LoadPreProcessed") != true || (string)Variables["LoadPreProcessed"] == "False"))
            {

                FindCell();
                RemoveBackground();
                PreBatchProcessCenter();
            }
            // return;
            //do any pre convolution work.  This is where most of the changes should be located
            BatchLoopThroughImagesSave("BeforeConvolution");

            PreBatchProcessRecon();
            PostBatchProcess();
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

            X_PositionsB = new double[ImageFileListIn.Count];
            Y_PositionsB = new double[ImageFileListIn.Count];
            X_Positions = new double[ImageFileListIn.Count];
            Y_Positions = new double[ImageFileListIn.Count];
            NumBlobs = new double[ImageFileListIn.Count];
            CellSizes = new double[ImageFileListIn.Count];
            FocusValues = new double[ImageFileListIn.Count];
            CellStain = new double[ImageFileListIn.Count];

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


            //Console.WriteLine("Forcing image to monochrome!!!!!");
            ColorImage = dataEnvironment.ProgressLog["IsColor"].ToString().ToLower() == "true";
            DoRun(Variables);
        }

       
    }
}

