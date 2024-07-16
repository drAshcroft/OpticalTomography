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
    public class BaseScriptSingle2 : IScript
    {
        public string GetName()
        {
            return "BaseScriptSingle";
        }

        public virtual IScript CloneScript()
        {
            return new BaseScriptSingle2();
        }


        ImageHolder background = null;

        #region global values
        protected Rectangle FirstCellGuess = Rectangle.Empty;

        protected CellPositions CellPositionGuesses;

        protected double[,] BackgroundMask;

        protected Rectangle CellWanderArea;
        protected int CellWanderMargin = 120;

        protected int RoughCellSize = 170;
        protected int RoughCellHalf = 85;

        protected int FineCellSize = 170;
        protected int FineCellHalf = 85;

        protected int ReconCellSize = 170;
        protected int ReconCellHalf = 85;

        protected bool LopsidedCell = false;

        protected bool FluorImage = false;
        protected bool ColorImage = false;

        protected bool CellAreadyFound = false;

        protected DataEnvironment dataEnvironment;

        protected ProjectionArrayObject DensityGrid = null;
        protected Rectangle ReconCutDownRect;


        protected RoughFindCell CellFinder = new RoughFindCell();

        protected object DehyrateLock = new object();
        protected bool SaveDehydrated = false;


        protected bool ExistingBackgroundOnly = true;
        protected bool TopAndBottomMask = true;

        protected List<ImageFile> ImageFileListIn;
        #endregion

        #region Process Individual Files

        public virtual ImageHolder ProcessImageLoad(int ImageNumber)
        {
            try
            {
                ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];

                // dataEnvironment.ProgressLog.AddSafe("Num Loaded Channels", BitmapImage.NChannels);

                //make sure there is only one channel to work with
                if (BitmapImage.NChannels > 1)
                {
                    BitmapImage = ImageViewer.Filters.Adjustments.GrayScaleEffectChannel.GrayScaleFromChannel(BitmapImage, 1);
                }

              /*  if (ColorImage == true)
                {
                    BitmapImage = MathHelpsFileLoader.FixVisionGateImage(BitmapImage, 2);
                }*/

                if (FluorImage == true)
                {
                    //because the 6 are preloaded, this results in 2 image flips.  to avoid another memory copy, just dont invert those that are already inverted.
                    if (AlreadyLoaded.Contains(ImageNumber) == false)
                    {
                        //Invert Contrast
                        BitmapImage.InvertMax();
                    }
                    // BitmapImage = ImageViewer.Filters.Effects.RankOrder.MedianFilterTool.MedianFilter(BitmapImage, 5);
                }

                ImageHolder findBackground = null;

                if (dataEnvironment.ScriptParams["GlobalFlatten"].ToLower() == "true")
                {
                    //alters the image inside
                    ImageViewer.Filters.Effects.Flattening.FlattenEdges1DErrorCorrected.FlattenImageEdgesGlobal(BitmapImage);

                    if (background != null)
                    {
                        findBackground = BitmapImage.Clone();
                        findBackground = ImageViewer.Filters.Effects.Flattening.DivideImage.DivideOneImageByAnother(findBackground, background, new Rectangle(0, 0, BitmapImage.Width, BitmapImage.Height));
                        Bitmap b = findBackground.ToBitmap();
                        int w = b.Width;
                    }
                    else
                        findBackground = BitmapImage.Clone();
                }
                else
                    findBackground = BitmapImage.Clone();

                if (FluorImage == true)
                {
                    findBackground = ImageViewer.Filters.Effects.RankOrder.MedianFilterTool.MedianFilter(findBackground, 5);
                }

                //creates a new image and then messes with the new image, so it does not affect this one.
                CellFinder.ProcessImageFindRough(dataEnvironment, ImageNumber, findBackground, CellPositionGuesses, 2, CellWanderArea, FirstCellGuess);

                return BitmapImage;
            }
            catch (Exception ex)
            {
                string errorMessage = PythonHelps.FormatException(ex);
                dataEnvironment.ProgressLog.AddSafe("Error Message", errorMessage);
                dataEnvironment.ProgressLog.AddSafe("Imageload", "error");

                throw ex;
            }
        }

        public virtual ImageHolder ProcessImageDivide(int ImageNumber)
        {

            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];
            // Bitmap b = BitmapImage.ToBitmap();
            //  int w = b.Width;
            // BitmapImage.Save(@"c:\temp\before.tif");
            try
            {
                int cellHalf = (int)(RoughCellSize);
                Rectangle CellArea = new Rectangle((int)(CellPositionGuesses.X_Positions[ImageNumber] - cellHalf), (int)(CellPositionGuesses.Y_Positions[ImageNumber] - cellHalf), 2 * cellHalf, 2 * cellHalf);


                Rectangle CellAreaPadded = new Rectangle(CellArea.Location, CellArea.Size);
                CellAreaPadded.Inflate((int)(.4 * CellArea.Width), (int)(.4 * CellArea.Height));

                //    b = MathHelpLib.ImageProcessing.MathImageHelps.MakeBitmap(BackgroundMask);

                ImageViewer.Filters.Effects.Flattening.DivideImage.DivideOneImageByAnother(BitmapImage, BackgroundMask, CellAreaPadded);

                //  b = BitmapImage.ToBitmap();
                //    w = b.Width;
                return BitmapImage;
            }
            catch (Exception ex)
            {
                dataEnvironment.ProgressLog.AddSafe("ImageDivide", "error");
                string errorMessage = PythonHelps.FormatException(ex);
                dataEnvironment.ProgressLog.AddSafe("Error Message", errorMessage);
                throw ex;
            }
        }

        public virtual ImageHolder ProcessImageFindFine(int ImageNumber)
        {
            try
            {
                ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];

                int Xo = (int)Math.Truncate(CellPositionGuesses.X_Positions[ImageNumber] - RoughCellHalf);
                int Yo = (int)Math.Truncate(CellPositionGuesses.Y_Positions[ImageNumber] - RoughCellHalf);

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
                    string errorMessage = PythonHelps.FormatException(ex);
                    dataEnvironment.ProgressLog.AddSafe("Error Message", errorMessage);
                }

                double AveX = 0;
                double AveY = 0;

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
                        if (dataEnvironment.ScriptParams["COGMethod"] == "Threshold")
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
                        string errorMessage = PythonHelps.FormatException(ex);
                        dataEnvironment.ProgressLog.AddSafe("Error Message", errorMessage);
                    }
                }

                try
                {
                    //figure out the average and offset the data after the clip
                    AveX = Xo + AveX / (double)NumTestRuns;
                    AveY = Yo + AveY / (double)NumTestRuns;

                    if (MaxBlob.BlobBounds.Width > MaxBlob.BlobBounds.Height)
                        CellPositionGuesses.CellSizes[ImageNumber] = MaxBlob.BlobBounds.Width;
                    else
                        CellPositionGuesses.CellSizes[ImageNumber] = MaxBlob.BlobBounds.Height;

                    if (Math.Abs(1 - MaxBlob.BlobBounds.Width / (double)MaxBlob.BlobBounds.Height) > .1)
                        LopsidedCell = true;

                    if (Math.Abs(CellPositionGuesses.X_Positions[ImageNumber] - AveX) < 10)
                        CellPositionGuesses.X_Positions[ImageNumber] = AveX;

                    if (Math.Abs(CellPositionGuesses.Y_Positions[ImageNumber] - AveY) < 10)
                        CellPositionGuesses.Y_Positions[ImageNumber] = AveY;

                }
                catch (Exception ex)
                {
                    string errorMessage = PythonHelps.FormatException(ex);
                    dataEnvironment.ProgressLog.AddSafe("Error Message", errorMessage);

                }
                return new ImageHolder(bImage2);
            }
            catch (Exception ex)
            {
                dataEnvironment.ProgressLog.AddSafe("Imagefine", "error");
                string errorMessage = PythonHelps.FormatException(ex);
                dataEnvironment.ProgressLog.AddSafe("Error Message", errorMessage);
                throw ex;
            }
        }

        public virtual ImageHolder ProcessImageClip(int ImageNumber)
        {
            try
            {
                ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];
                // Bitmap b1 = BitmapImage.ToBitmap();
                Rectangle CellArea = new Rectangle((int)Math.Truncate(CellPositionGuesses.X_Positions[ImageNumber] - FineCellHalf), (int)Math.Truncate(CellPositionGuesses.Y_Positions[ImageNumber] - FineCellHalf), FineCellSize, FineCellSize);

                // if (ImageNumber == 495)
                //    System.Diagnostics.Debug.Print(" "); 

                BitmapImage = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(BitmapImage, CellArea);

                //  if (ImageNumber == 495)
                //      System.Diagnostics.Debug.Print(" "); 

                //  int w2 = b1.Width;
                BitmapImage.InvertMax();

                // Bitmap test = BitmapImage.ToBitmap();
                // int w = test.Width;

                //  test = BitmapImage.ToBitmap();
                //  w = test.Width;

                BitmapImage = ImageViewer.Filters.Effects.Flattening.FlattenEdges1D.FlattenImageEdgesConstant(BitmapImage);
                //  Bitmap test2 = BitmapImage.ToBitmap();
                //  int w2 = test2.Width;

                // test = BitmapImage.ToBitmap();
                // w = test.Width;

                if (dataEnvironment.ScriptParams.ContainsKey("dehydrateFolder") == true)
                {
                    string DehydrateFolder = dataEnvironment.ScriptParams["dehydrateFolder"];

                    BitmapImage.Save(DehydrateFolder + "\\centered" + string.Format("{0:000}.tif", ImageNumber));
                }

                // test = BitmapImage.ToBitmap();
                // w= test.Width;
                //  int w2 = b1.Width;

                // BitmapImage.Save(@"C:\Development\CellCT\testimages\image" + ImageNumber.ToString() + ".bmp");
                // if (dataEnvironment.ScriptParams["FlatMethod"].ToLower() == "plane")
                {
                    //  BitmapImage = ImageViewer.Filters.Effects.Flattening.FlattenEdges.FlattenImageEdges(BitmapImage);
                }


                if (dataEnvironment.ScriptParams["FlatMethod"].ToLower() == "curve")
                {
                    //   BitmapImage = ImageViewer.Filters.Effects.Flattening.FlattenEdges1D.FlattenImageEdgesMedianPoly(BitmapImage,15);
                    //BitmapImage = ImageViewer.Filters.Effects.Flattening.FlattenEdges1D.FlattenImageEdgesConstant(BitmapImage, 15);
                }
                //  test = BitmapImage.ToBitmap();
                //  w = test.Width;
                double CenterVal = 0, X, Y;
                //get focus value of the clipped cell
                double FocusValue = FocusValueTool.FocusValueF4(BitmapImage, out CenterVal, out X, out Y);
                CellPositionGuesses.X_Positions[ImageNumber] = CellPositionGuesses.X_Positions[ImageNumber] - FineCellHalf + X;
                CellPositionGuesses.Y_Positions[ImageNumber] = CellPositionGuesses.Y_Positions[ImageNumber] - FineCellHalf + Y;



                //if (ImageNumber == 495)
                //     System.Diagnostics.Debug.Print(" "); 

                CellPositionGuesses.FocusValue[ImageNumber] = FocusValue;
                CellPositionGuesses.CenterQuality[ImageNumber] = CenterVal * 100;
                //get and idea of the cell staining for normalization and quality checks
                CellPositionGuesses.CellStain[ImageNumber] = BitmapImage.ImageData.AverageArrayWithThreshold(5000);
                return BitmapImage;
            }
            catch (Exception ex)
            {
                dataEnvironment.ProgressLog.AddSafe("Imageclip", "error");
                MessageBox.Show(ex.StackTrace);
                throw ex;
            }
        }

        public virtual ImageHolder ProcessBeforeConvolution(int ImageNumber)
        {
            try
            {
                ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];

                if (dataEnvironment.ScriptParams["SaveCenteredImage"].ToLower() == "true")
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

                string Method = dataEnvironment.ScriptParams["PreprocessingMethod"];
                int Radius = (int)EffectHelps.ConvertToDouble(dataEnvironment.ScriptParams["ProprocessingRadius"]);

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
                dataEnvironment.ProgressLog.AddSafe("Image before convolution", "error");
                MessageBox.Show(ex.StackTrace);
                throw ex;
            }
        }

        bool[] FrameAlreadyProjected = null;
        public virtual ImageHolder ProcessImageFBP(int ImageNumber)
        {
            try
            {
                //if (FrameAlreadyProjected[ImageNumber])
                //    return dataEnvironment.AllImages[ImageNumber];

                ImageHolder BitmapImage;
                int nRuns = 1;
                int MirrorIndex = ((ImageNumber + dataEnvironment.AllImages.Count / 2) % dataEnvironment.AllImages.Count);
                double MirrorValue = CellPositionGuesses.FocusValue[MirrorIndex];

                BitmapImage = dataEnvironment.AllImages[ImageNumber];
                double w = 1;
                //double F1 =  focusValueMax- Math.Abs(CellPositionGuesses.FocusValue[ImageNumber] - focusValAverage);
                //double F2 = focusValueMax - Math.Abs(MirrorValue - focusValAverage);
                double F1 = Math.Sqrt(Math.Abs(CellPositionGuesses.FocusValue[ImageNumber] - focusValAverage));
                double F2 = Math.Sqrt(focusValueMax - Math.Abs(MirrorValue - focusValAverage));
                if (F1 >= 1)
                    F1 = 1 / F1;
                else
                    F1 = 1;

                if (F2 >= 1)
                    F2 = 1 / F2;
                else
                    F2 = 1;

                w = 2 * F1 / (F1 + F2);
                double w2 = 2 * F2 / (F1 + F2);
                CellPositionGuesses.FocusValue[ImageNumber] = (CellPositionGuesses.FocusValue[ImageNumber] * F1 + CellPositionGuesses.FocusValue[MirrorIndex] * F2) / (F1 + F2);

                if (w < .25) return BitmapImage;
                if (w2 < .25) w = 2;

                double[,] Slice = Scripts.FBPScripts.DoSliceConvolution(dataEnvironment, dataEnvironment.AllImages[ImageNumber], DensityGrid, ReconCutDownRect);
                if (w != 1)
                {
                    Slice.MultiplyInPlace(w);
                }

                double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * ImageNumber;

                for (int j = 0; j < nRuns; j++)
                {
                    if (DensityGrid.ReconInterpolationMethod == "Linear")
                    {
                        dataEnvironment.RunningOnGPU = false;
                        if (dataEnvironment.RunningOnGPU == false)
                        {
                            Slice = ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.DoInterpolation(Slice, 1, 1, DensityGrid.DataWhole, 1, 1, AngleRadians, 20, ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.InterpolationMethod.Cosine);
                        }

                        ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect();
                        Filter.DoEffect(dataEnvironment, BitmapImage, null, Slice, DensityGrid, AngleRadians);
                    }
                    else if (DensityGrid.ReconInterpolationMethod == "Siddon")
                    {
                        //  Bitmap b = MathHelpLib.ImageProcessing.MathImageHelps.MakeBitmap(Slice);
                        ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2 Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2();
                        Filter.DoEffect(dataEnvironment, BitmapImage, null, Slice, DensityGrid, AngleRadians);
                    }
                    else if (DensityGrid.ReconInterpolationMethod == "Gaussian")
                    {
                        ImageViewer.PythonScripting.Projection.DoSliceBackProjectionGaussianEffect Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionGaussianEffect();
                        Filter.DoEffect(dataEnvironment, BitmapImage, null, Slice, DensityGrid, AngleRadians);
                    }
                }
                Console.WriteLine(ImageNumber);

                return new ImageHolder(Slice);
            }
            catch (Exception ex)
            {
                dataEnvironment.ProgressLog.AddSafe("fbp", "error");
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
                return dataEnvironment.AllImages[ImageNumber];
                //  throw ex;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////  Cell Finding and preproessing
        List<int> AlreadyLoaded = new List<int>();
        #region  Find Cell and Cell size
        protected void FindFirstCell()
        {
            if (FirstCellGuess == Rectangle.Empty)
            {
                FirstCellGuess = new Rectangle(0, 0, dataEnvironment.AllImages[0].Width, dataEnvironment.AllImages[1].Height);
            }
            ImageHolder ih = dataEnvironment.AllImages[1];

            int[] Indexs = new int[12];
            for (int i = 0; i < Indexs.Length; i++)
            {
                Indexs[i] = (int)(dataEnvironment.AllImages.Count * (double)i / (double)Indexs.Length);

                ProcessImageLoad(Indexs[i]);

                //  AlreadyLoaded.Add(Indexs[i]);
            }

            int roughCellSize, junk;
            try
            {
                CellWanderArea = CellFinder.FindBestCellCenter(dataEnvironment, Indexs, FirstCellGuess, false, ref CellPositionGuesses, out roughCellSize);
            }
            catch
            {
                PythonHelps.OpenXMLAndProjectionLocations(dataEnvironment.ScriptParams["vgfolder"] + "\\PPDetailReport.xml", CellPositionGuesses);
            }


            //if (dataEnvironment.ScriptParams.ContainsKey("Centering") && dataEnvironment.ScriptParams["Centering"].ToLower() == "bad")
            PythonHelps.OpenXMLAndProjectionLocations(dataEnvironment.ScriptParams["vgfolder"] + "\\PPDetailReport.xml", CellPositionGuesses);

        }

        protected virtual void FindCell()
        {
            if (CellAreadyFound == false)
                FindFirstCell();

            dataEnvironment.ProgressLog.AddSafe("Prep", "Load Image");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList

            dataEnvironment.ProgressLog.AddSafe("Position", "starting process Rough");
            PythonHelps.BatchLoopThroughImages("LoadImages", this, dataEnvironment, true);

            CellFinder.SaveBlobs(dataEnvironment);

            RoughCellSize = (int)(PickTheRightCell());
            RoughCellHalf = (int)(RoughCellSize / 2d);
        }

        protected virtual double PickTheRightCell()
        {

            double tCellSize = CellPositionGuesses.CellSizes[0];

            List<double> tCellSizes = new List<double>(CellPositionGuesses.CellSizes.Length);
            tCellSizes.AddRange(CellPositionGuesses.CellSizes);
            tCellSizes.Sort();
            tCellSize = tCellSizes[(int)(tCellSizes.Count * 3d / 4d)];

            int[] Indexs = new int[dataEnvironment.AllImages.Count];
            int ttCellSize, junk;
            for (int i = 0; i < Indexs.Length; i++)
            {
                Indexs[i] = i;
            }
            bool Error = false;
            try
            {
                CellWanderArea = CellFinder.FindBestCellCenter(dataEnvironment, Indexs, FirstCellGuess, true, ref CellPositionGuesses, out  ttCellSize);
                tCellSize = 1.05 * ttCellSize;
            }
            catch
            {
                Error = true;
            }

            Error = true;
            if (Error || (dataEnvironment.ScriptParams.ContainsKey("Centering") && dataEnvironment.ScriptParams["Centering"].ToLower() == "bad"))
            {
                dataEnvironment.ProgressLog.AddSafe("positionHelp", "True");
                List<int> cellSizes = new List<int>();
                for (int i = 0; i < CellPositionGuesses.CellSizes.Length; i++)
                {
                    cellSizes.Add((int)CellPositionGuesses.CellSizes[i]);
                }
                cellSizes.Sort();
                tCellSize = cellSizes[cellSizes.Count / 2] * 1.5;

                PythonHelps.OpenXMLAndProjectionLocations(dataEnvironment.ScriptParams["vgfolder"] + "\\PPDetailReport.xml", CellPositionGuesses);
                //tCellSize = 300;// GetCellBounds();
                // tCellSize = FirstCellGuess.Width / 2.25;// GetCellBounds();
            }

            return tCellSize;
        }

        protected int GetCellBounds()
        {
            int Index;
            int IndexsLength = 12;
            int MaxX = int.MinValue, MaxY = int.MinValue;
            for (int i = 0; i < IndexsLength; i++)
            {
                Index = (int)(dataEnvironment.AllImages.Count * (double)i / (double)IndexsLength);

                try
                {
                    int CellLength = (int)(RoughCellSize);
                    Rectangle CellArea = new Rectangle((int)Math.Truncate(CellPositionGuesses.X_Positions[Index] - CellLength), (int)Math.Truncate(CellPositionGuesses.Y_Positions[Index] - CellLength),
                        CellLength / 2, CellLength / 2);

                    // if (ImageNumber == 495)
                    //    System.Diagnostics.Debug.Print(" "); 
                    var BitmapImage = CenterCellsTool2Form.ClipImage(dataEnvironment, Index, CellPositionGuesses.X_Positions, CellPositionGuesses.Y_Positions, RoughCellSize / 2, RoughCellSize);

                    var bImage2 = BitmapImage.ToMaskBitmap(5000);

                    BlobDescription Blobs = ImageViewer.Filters.Blobs.CenterOfGravityTool.DoCOGCenter(bImage2)[0];


                    if (MaxX < Blobs.BlobBounds.Width) MaxX = Blobs.BlobBounds.Width;
                    if (MaxY < Blobs.BlobBounds.Height) MaxY = Blobs.BlobBounds.Height;
                }
                catch { }
            }



            if (MaxX > MaxY)
                ReconCellSize = MaxX;
            else
                ReconCellSize = MaxY;

            return (int)(ReconCellSize * 1.3 + 20);


        }

        protected void GetCenteredCellBounds()
        {
            int Index;
            int IndexsLength = 12;
            int MinX = int.MaxValue, MaxX = int.MinValue, MinY = int.MaxValue, MaxY = int.MinValue;
            for (int i = 0; i < IndexsLength; i++)
            {
                Index = (int)(dataEnvironment.AllImages.Count * (double)i / (double)IndexsLength);

                try
                {
                    var bImage2 = dataEnvironment.AllImages[Index].ToMaskBitmap(5000);

                    BlobDescription Blobs = ImageViewer.Filters.Blobs.CenterOfGravityTool.DoCOGCenter(bImage2)[0];

                    if (MinX > Blobs.BlobBounds.X) MinX = Blobs.BlobBounds.X;
                    if (MinY > Blobs.BlobBounds.Y) MinY = Blobs.BlobBounds.Y;
                    if (MaxX < Blobs.BlobBounds.Right) MaxX = Blobs.BlobBounds.Right;
                    if (MaxY < Blobs.BlobBounds.Bottom) MaxY = Blobs.BlobBounds.Bottom;
                }
                catch { }
            }

            int width, height;

            if (Math.Abs(FineCellHalf - MinX) > Math.Abs(FineCellHalf - MaxX))
                width = 2 * Math.Abs(FineCellHalf - MinX);
            else
                width = 2 * Math.Abs(FineCellHalf - MaxX);

            if (Math.Abs(FineCellHalf - MinY) > Math.Abs(FineCellHalf - MaxY))
                height = 2 * Math.Abs(FineCellHalf - MinY);
            else
                height = 2 * Math.Abs(FineCellHalf - MaxY);

            if (width > height)
                ReconCellSize = width;
            else
                ReconCellSize = height;

            ReconCellSize = (int)(ReconCellSize * 1.3 + 20);

            ReconCellHalf = ReconCellSize / 2;
        }

        #endregion

        /// ////////////////////////////////////////////////////    Finding the background 

        #region Remove Background
        protected virtual void FindBackground(double CellSize)
        {
            ImageViewer.Filters.StationaryPixelsForm Filter = null;
            ReplaceStringDictionary PassData = new ReplaceStringDictionary();
            ImageHolder BitmapImage = null;
            bool BackgroundHelp = false;

            dataEnvironment.ProgressLog.AddSafe("CellSize", RoughCellSize);
            try
            {
                TopAndBottomMask = false;
                bool MedianMask = false;
                if (File.Exists(Path.GetDirectoryName(dataEnvironment.ScriptParams["BackFolder"]) + "\\Background.tif") == true)
                {
                    dataEnvironment.ProgressLog.AddSafe("backgroundRemovalMethod", "load background");
                    BackgroundHelp = true;
                    PassData.AddSafe("BackgroundMask",
                        MathHelpsFileLoader.Load_Bitmap(Path.GetDirectoryName(dataEnvironment.ScriptParams["BackFolder"]) + "\\Background.tif").ToDataIntensityDouble());
                }
                else if (TopAndBottomMask)
                {
                    dataEnvironment.ProgressLog.AddSafe("backgroundRemovalMethod", "TopAndBottomMask");
                    Filter = new ImageViewer.Filters.StationaryPixelsForm(true);
                    BitmapImage = Filter.FindStationaryPixels(dataEnvironment, PassData, 100, dataEnvironment.ScriptParams["BackgroundSubMethod"],
                        CellPositionGuesses.X_Positions, CellPositionGuesses.Y_Positions, false, "Divide", 0, (int)(RoughCellSize + 50), true, ImageFileListIn);
                }
                else if (MedianMask)
                {
                    dataEnvironment.ProgressLog.AddSafe("backgroundRemovalMethod", "Median Background");
                    BackgroundMask = ImageViewer.Filters.StationaryPixelsForm.MedianBackground(dataEnvironment, CellPositionGuesses.X_Positions, CellPositionGuesses.Y_Positions,/*100*/ (int)(RoughCellSize + 25), false);

                    //  ImageViewer.Filters.Effects.Flattening.FlattenEdges1DErrorCorrected.FlattenImageEdgesGlobal(BackgroundMask);

                    PassData.AddSafe("BackgroundMask", BackgroundMask);
                }
                else
                {
                    dataEnvironment.ProgressLog.AddSafe("backgroundRemovalMethod", "Average Background");
                    BackgroundMask = ImageViewer.Filters.StationaryPixelsForm.AverageBackground(dataEnvironment, CellPositionGuesses.X_Positions, CellPositionGuesses.Y_Positions,/*100*/ (int)(RoughCellSize + 25), false);

                    //  ImageViewer.Filters.Effects.Flattening.FlattenEdges1DErrorCorrected.FlattenImageEdgesGlobal(BackgroundMask);

                    PassData.AddSafe("BackgroundMask", BackgroundMask);
                }
            }
            catch (Exception ex)
            {
                dataEnvironment.ProgressLog.AddSafe("backgroundRemovalMethod", "unable to create");
                throw (ex);
            }

            dataEnvironment.ProgressLog.AddSafe("Position", "saving stationary pixels");

            #region Save background info
            BackgroundMask = (double[,])PassData["BackgroundMask"];


            if (BackgroundHelp == false)
            {

                if (BitmapImage != null)
                {
                    Bitmap b = BitmapImage.ToBitmap();
                    MathHelpsFileLoader.Save_Bitmap(dataEnvironment.DataOutFolder + "Background.bmp", b);
                }
                try
                {
                    MathHelpsFileLoader.Save_Raw(dataEnvironment.ScriptParams["BackFolder"], BackgroundMask);

                }
                catch { }
            }

            double maxBlobs = PythonHelps.max(CellPositionGuesses.NumBlobs);
            dataEnvironment.ProgressLog.AddSafe("NumberOfCells", maxBlobs.ToString());
            #endregion



            CellWanderMargin = RoughCellSize;

            PythonHelps.print("Rough Cell Size" + RoughCellSize.ToString());

            int XMin = (int)(PythonHelps.min(CellPositionGuesses.X_Positions) - CellWanderMargin);
            int YMin = (int)(PythonHelps.min(CellPositionGuesses.Y_Positions) - CellWanderMargin);
            int XMax = (int)(PythonHelps.max(CellPositionGuesses.X_Positions) + CellWanderMargin);
            int YMax = (int)(PythonHelps.max(CellPositionGuesses.Y_Positions) + CellWanderMargin);

            CellWanderArea = new Rectangle(XMin, YMin, XMax - XMin, YMax - YMin);

            try
            {
                Rectangle examples = new Rectangle(XMin, 0, XMax - XMin, dataEnvironment.AllImages[1].Height);
                ImagingTools.ClipImage(dataEnvironment.AllImages[0].ToBitmap(), examples).Save(dataEnvironment.DataOutFolder + "projection1.jpg");
                ImagingTools.ClipImage(dataEnvironment.AllImages[(int)(dataEnvironment.AllImages.Count / 4d)].ToBitmap(), examples).Save(dataEnvironment.DataOutFolder + "projection2.jpg");
                ImagingTools.ClipImage(dataEnvironment.AllImages[(int)(dataEnvironment.AllImages.Count / 2d)].ToBitmap(), examples).Save(dataEnvironment.DataOutFolder + "projection3.jpg");
                ImagingTools.ClipImage(dataEnvironment.AllImages[(int)(dataEnvironment.AllImages.Count * 3d / 4d)].ToBitmap(), examples).Save(dataEnvironment.DataOutFolder + "projection4.jpg");
            }
            catch { }

            PythonHelps.print(CellWanderArea);
        }

        protected virtual void RemoveBackground()
        {
            dataEnvironment.ProgressLog.AddSafe("Position", "starting stationary pixels");

            FindBackground(RoughCellSize);

            dataEnvironment.ProgressLog.AddSafe("Position", "Dividing away background");

            PythonHelps.BatchLoopThroughImages("Divide", this, dataEnvironment, true);

            dataEnvironment.ProgressLog.AddSafe("Divide", "Divide Succeeded");

        }
        #endregion
        //*****************************************************   Final Centering and clipping

        double focusValueSD = 1000000;
        double focusValAverage = 0;
        double focusValueMax = 0;
        protected virtual void PreBatchProcessCenter()
        {
            ReplaceStringDictionary PassData = new ReplaceStringDictionary();
            dataEnvironment.ProgressLog.AddSafe("Prep", "Center");

            double tCellSize = RoughCellSize;


            Bitmap b = dataEnvironment.AllImages[495].ToBitmap();
            int w = b.Width;

            if (false)
            {
                GoodFeaturesToTrackFinder.GoodFeatureCenter(dataEnvironment, RoughCellSize, CellPositionGuesses.X_Positions, CellPositionGuesses.Y_Positions);

                if (LopsidedCell)
                {
                    tCellSize = CellPositionGuesses.CellSizes[0];
                    for (int i = 0; i < CellPositionGuesses.CellSizes.Length; i++)
                    {
                        if (CellPositionGuesses.CellSizes[i] > tCellSize) tCellSize = CellPositionGuesses.CellSizes[i];
                    }
                }
                else
                {
                    List<int> cellSizes = new List<int>();
                    for (int i = 0; i < CellPositionGuesses.CellSizes.Length; i++)
                    {
                        cellSizes.Add((int)CellPositionGuesses.CellSizes[i]);
                    }
                    cellSizes.Sort();
                    tCellSize = cellSizes[cellSizes.Count / 2];
                }

            }
            else if (false)
            {
                PythonHelps.BatchLoopThroughImages("FindFine", this, dataEnvironment, false);
                for (int i = 0; i < CellPositionGuesses.CellSizes.Length; i++)
                {
                    CellPositionGuesses.CellSizes[i] = CellPositionGuesses.CellSizes[i] / 1.5;
                }

                if (LopsidedCell)
                {
                    tCellSize = CellPositionGuesses.CellSizes[0];
                    for (int i = 0; i < CellPositionGuesses.CellSizes.Length; i++)
                    {
                        if (CellPositionGuesses.CellSizes[i] > tCellSize) tCellSize = CellPositionGuesses.CellSizes[i];
                    }
                }
                else
                {
                    List<int> cellSizes = new List<int>();
                    for (int i = 0; i < CellPositionGuesses.CellSizes.Length; i++)
                    {
                        cellSizes.Add((int)CellPositionGuesses.CellSizes[i]);
                    }
                    cellSizes.Sort();
                    tCellSize = cellSizes[cellSizes.Count / 2];
                }

            }
            else
            {

                PythonHelps.BatchLoopThroughImages("FindFine", this, dataEnvironment, false);
                for (int i = 0; i < CellPositionGuesses.CellSizes.Length; i++)
                {
                    CellPositionGuesses.CellSizes[i] = CellPositionGuesses.CellSizes[i] / 1.5;
                }

                if (LopsidedCell)
                {
                    tCellSize = CellPositionGuesses.CellSizes[0];
                    for (int i = 0; i < CellPositionGuesses.CellSizes.Length; i++)
                    {
                        if (CellPositionGuesses.CellSizes[i] > tCellSize) tCellSize = CellPositionGuesses.CellSizes[i];
                    }
                }
                else
                {
                    List<int> cellSizes = new List<int>();
                    for (int i = 0; i < CellPositionGuesses.CellSizes.Length; i++)
                    {
                        cellSizes.Add((int)CellPositionGuesses.CellSizes[i]);
                    }
                    cellSizes.Sort();
                    tCellSize = cellSizes[cellSizes.Count / 2];
                }

                tCellSize = ImageViewer.Filters.CenterCellsTool2Form.ReconCenter(dataEnvironment, RoughCellSize, CellPositionGuesses.X_Positions, CellPositionGuesses.Y_Positions);
                //    GC.Collect();
                tCellSize = GetCellBounds();
            }

            b = dataEnvironment.AllImages[495].ToBitmap();
            w = b.Width;



            using (System.IO.StreamWriter file = new System.IO.StreamWriter(dataEnvironment.DataOutFolder + "CentersBefore.csv"))
            {
                file.WriteLine(RoughCellSize);
                for (int i = 0; i < CellPositionGuesses.X_Positions.Length; i++)
                {

                    file.WriteLine(CellPositionGuesses.X_Positions[i] + ", " + CellPositionGuesses.Y_Positions[i] + "," + CellPositionGuesses.CellSizes[i].ToString());
                }
            }

            ReconCellSize = (int)(tCellSize * 1.1);
            FineCellSize = (int)(ReconCellSize + 50);

            ReconCellHalf = ReconCellSize / 2;
            FineCellHalf = FineCellSize / 2;

            bool OutOfRange = false;


            for (int i = 0; i < CellPositionGuesses.X_Positions.Length; i++)
            {
                if ((CellPositionGuesses.Y_Positions[i] - ReconCellHalf) < 0 || (CellPositionGuesses.Y_Positions[i] + ReconCellHalf) > dataEnvironment.AllImages[1].Height)
                    OutOfRange = true;
                if ((CellPositionGuesses.X_Positions[i] - ReconCellHalf) < 0 || (CellPositionGuesses.X_Positions[i] + ReconCellHalf) > dataEnvironment.AllImages[1].Width)
                    OutOfRange = true;

            }

            dataEnvironment.ProgressLog.AddSafe("OutOfImageRange", OutOfRange.ToString());

            dataEnvironment.ProgressLog.AddSafe("CenteringQualityActual", "100%");
            dataEnvironment.ProgressLog.AddSafe("CenteringQuality", "100%");


            dataEnvironment.ProgressLog.AddSafe("Position", "Clipping");
            PythonHelps.BatchLoopThroughImages("ClipImages", this, dataEnvironment, true);
            dataEnvironment.ProgressLog.AddSafe("Clipping", "Images Clipped");

            CellFinder.SaveCenters(dataEnvironment, CellPositionGuesses.X_Positions, CellPositionGuesses.Y_Positions);


            GetCenteredCellBounds();

            //save the variation of the focus value to help select the best angle
            double intensity = PythonHelps.Average(CellPositionGuesses.CellStain);

            dataEnvironment.ProgressLog.AddSafe("Cell Staining Average", intensity);
            dataEnvironment.ProgressLog.AddSafe("Cell Staining SD", PythonHelps.Stdev(CellPositionGuesses.CellStain));// PythonHelps.max(CellPositionGuesses.CellStain) - PythonHelps.min(CellPositionGuesses.CellStain));
            dataEnvironment.ProgressLog.AddSafe("Cell Staining Variance", PythonHelps.max(CellPositionGuesses.CellStain) - PythonHelps.min(CellPositionGuesses.CellStain));


            dataEnvironment.ProgressLog.AddSafe("Center Quality", PythonHelps.Average(CellPositionGuesses.CenterQuality));
            dataEnvironment.ProgressLog.AddSafe("Center Quality SD", PythonHelps.Stdev(CellPositionGuesses.CenterQuality));// PythonHelps.max(CellPositionGuesses.CellStain) - PythonHelps.min(CellPositionGuesses.CellStain));
            dataEnvironment.ProgressLog.AddSafe("Center Quality Variance", PythonHelps.max(CellPositionGuesses.CenterQuality) - PythonHelps.min(CellPositionGuesses.CenterQuality));


            focusValAverage = 0;
            for (int i = 0; i < CellPositionGuesses.FocusValue.Length; i++)
            {
                CellPositionGuesses.FocusValue[i] /= intensity;
                focusValAverage += CellPositionGuesses.FocusValue[i];
                if (CellPositionGuesses.FocusValue[i] > focusValueMax)
                    focusValueMax = CellPositionGuesses.FocusValue[i];
            }

            focusValAverage /= CellPositionGuesses.FocusValue.Length;
            focusValueSD = PythonHelps.Stdev(CellPositionGuesses.FocusValue);

            double sd = PythonHelps.Stdev(CellPositionGuesses.FocusValue) / focusValAverage * 100;
            double csd = PythonHelps.Average(CellPositionGuesses.CenterQuality);

            dataEnvironment.ProgressLog.AddSafe("Focus Original", PythonHelps.Average(CellPositionGuesses.FocusValue));
            dataEnvironment.ProgressLog.AddSafe("Focus Original SD", PythonHelps.Stdev(CellPositionGuesses.FocusValue));// PythonHelps.max(CellPositionGuesses.CellStain) - PythonHelps.min(CellPositionGuesses.CellStain));
            dataEnvironment.ProgressLog.AddSafe("Focus Original Variance", PythonHelps.max(CellPositionGuesses.FocusValue) - PythonHelps.min(CellPositionGuesses.FocusValue));

            if (dataEnvironment.ScriptParams["SaveCenteringMovie"].ToLower() == "true")
            {
                ImagingTools.CreateAVIVideo(dataEnvironment.DataOutFolder + "Centering.avi", dataEnvironment.AllImages, 10);
            }

            if (sd > 20)
            {
                dataEnvironment.ProgressLog.AddSafe("FocusFilter", "Focus Value is out of range, Questionable Focus");
                // throw new Exception("Bad Focus");
            }
            if (csd > 7)
            {
                dataEnvironment.ProgressLog.AddSafe("CenteringFilter", "Centering Value is out of range, Bad Centering");
                if (!(dataEnvironment.ScriptParams.ContainsKey("Centering") && dataEnvironment.ScriptParams["Centering"].ToLower() == "bad"))
                    throw new Exception("Bad Centering");
            }
        }

        float[, ,] ReducedDensityGrid = null;

        protected virtual void PreBatchProcessReconGPU()
        {
            GC.Collect();
            dataEnvironment.ProgressLog.AddSafe("Prep", "Recon");
            //create a densitygrid if this is the first run, otherwise just pull the already created grid
            dataEnvironment.ProgressLog.AddSafe("Position", "Getting Impulse");

            ReconCutDownRect = new Rectangle((int)((FineCellSize - ReconCellSize) / 2), (int)((FineCellSize - ReconCellSize) / 2), ReconCellSize, ReconCellSize);

            int FilterWidth = EffectHelps.ConvertToInt(dataEnvironment.ScriptParams["FBPResolution"]);//, EffectHelps.ConvertToInt(ParamDictionary["FBPResolution"])
            double[] impulse = Filtering.GetRealSpaceFilter(dataEnvironment.ScriptParams["FBPWindow"], FilterWidth, FilterWidth, (double)FilterWidth / 2d);
            float[] fImpulse = new float[impulse.Length];
            for (int i = 0; i < impulse.Length; i++)
                fImpulse[i] = (float)impulse[i];


            ReducedDensityGrid = new float[ReconCellSize, ReconCellSize, ReconCellSize];

            dataEnvironment.ProgressLog.AddSafe("Position", "GPU Convolution/Recon");
            string[] Platforms = MathHelpLib.Recon.DoSliceBackProjectionSiddonEffectGPU.AvailablePlatforms();
            MathHelpLib.Recon.DoSliceBackProjectionSiddonEffectGPU.SetupGPU(Platforms[0], 0);

            MathHelpLib.Recon.DoSliceBackProjectionSiddonEffectGPU.ConvolveAndProject(dataEnvironment.AllImages, fImpulse, ref ReducedDensityGrid);


            GC.Collect();
            dataEnvironment.ProgressLog.AddSafe("Recon", "Recon Completed");

        }

        protected virtual void PreBatchProcessRecon0()
        {
            GC.Collect();
            dataEnvironment.ProgressLog.AddSafe("Prep", "Recon");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList

            ReplaceStringDictionary PassData = new ReplaceStringDictionary();

            ImageHolder BitmapImage = dataEnvironment.AllImages[1];
            //create a densitygrid if this is the first run, otherwise just pull the already created grid
            IEffect Filter;

            Filter = new ImageViewer.PythonScripting.Projection.CreateFilteredBackProjectionEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ReconCellSize, ReconCellSize, 2, 2, 1, false, true);
            DensityGrid = (ProjectionArrayObject)Filter.PassData["FBJObject"];


            dataEnvironment.ProgressLog.AddSafe("Position", "Getting Impulse");

            //cut the recon cell out of the middle of the clipping
            ReconCutDownRect = new Rectangle((int)((FineCellSize - ReconCellSize) / 2), (int)((FineCellSize - ReconCellSize) / 2), ReconCellSize, ReconCellSize);

            int FilterWidth = EffectHelps.ConvertToInt(dataEnvironment.ScriptParams["FBPResolution"]);//, EffectHelps.ConvertToInt(ParamDictionary["FBPResolution"])
            DensityGrid.impulse = Filtering.GetRealSpaceFilter(dataEnvironment.ScriptParams["FBPWindow"], FilterWidth, FilterWidth, (double)FilterWidth / 2d);
            DensityGrid.DesiredMethod = ConvolutionMethod.Convolution1D;
            DensityGrid.ConvolutionFilter = new ImageViewer.PythonScripting.Projection.Convolution1D();
            DensityGrid.ReconInterpolationMethod = dataEnvironment.ScriptParams["InterpolationMethod"];


            dataEnvironment.ProgressLog.AddSafe("Position", "Doing Convolution");
            PythonHelps.BatchLoopThroughImages("DoFBPProjection", this, dataEnvironment, false);


            ReducedDensityGrid = DensityGrid.ToFloat();

            DensityGrid.DataWhole = null;
            DensityGrid = null;
            GC.Collect();
            dataEnvironment.ProgressLog.AddSafe("Recon", "Recon Completed");

        }

        protected virtual void PreBatchProcessRecon()
        {
            try
            {
                GC.Collect();
                dataEnvironment.ProgressLog.AddSafe("Prep", "Recon");
                //this is just to make the script functions easier, if needed convert to an image from ImageFileList

                ReplaceStringDictionary PassData = new ReplaceStringDictionary();

                ImageHolder BitmapImage = dataEnvironment.AllImages[1];
                //create a densitygrid if this is the first run, otherwise just pull the already created grid
                IEffect Filter;

                // Filter = new ImageViewer.PythonScripting.Projection.CreateFilteredBackProjectionEffect();
                // BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ReconCellSize, ReconCellSize, 2, 2, 1, false, true);
                // DensityGrid = (ProjectionArrayObject)Filter.PassData["FBJObject"];

                if (ReconCellSize % 2 == 1) ReconCellSize++;
                if (FineCellSize % 2 == 1) FineCellSize++;
                if (ReconCellSize > FineCellSize) ReconCellSize = (int)(FineCellSize * .9);
                ReconCellHalf = ReconCellSize / 2;

                FrameAlreadyProjected = new bool[dataEnvironment.AllImages.Count];
                // if (FineCellSize <700)
                {
                    PreBatchProcessRecon0();
                    return;
                }

                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                        DensityGrid = ImageViewer.PythonScripting.Projection.CreateFilteredBackProjectionEffect.CreateProjectionArrayObject(ReconCellSize, ReconCellSize, 0, ReconCellSize / 2, 2, 2);
                    else
                        DensityGrid = ImageViewer.PythonScripting.Projection.CreateFilteredBackProjectionEffect.CreateProjectionArrayObject(ReconCellSize, ReconCellSize, ReconCellSize / 2, ReconCellSize, 2, 2);

                    dataEnvironment.ProgressLog.AddSafe("Position", "Getting Impulse");

                    ReconCutDownRect = new Rectangle((int)((FineCellSize - ReconCellSize) / 2), (int)((FineCellSize - ReconCellSize) / 2), ReconCellSize, ReconCellSize);

                    int FilterWidth = EffectHelps.ConvertToInt(dataEnvironment.ScriptParams["FBPResolution"]);//, EffectHelps.ConvertToInt(ParamDictionary["FBPResolution"])
                    DensityGrid.impulse = Filtering.GetRealSpaceFilter(dataEnvironment.ScriptParams["FBPWindow"], FilterWidth, FilterWidth, (double)FilterWidth / 2d);
                    DensityGrid.DesiredMethod = ConvolutionMethod.Convolution1D;
                    DensityGrid.ConvolutionFilter = new ImageViewer.PythonScripting.Projection.Convolution1D();
                    DensityGrid.ReconInterpolationMethod = dataEnvironment.ScriptParams["InterpolationMethod"];


                    dataEnvironment.ProgressLog.AddSafe("Position", "Doing Convolution");
                    PythonHelps.BatchLoopThroughImages("DoFBPProjection", this, dataEnvironment, false);

                    DensityGrid.SaveFileRaw(dataEnvironment.DataOutFolder + "ProjectionObject" + i + ".raw", ProjectionArrayObject.RawFileTypes.Float32);
                    //  MathHelpsFileLoader.Save_Tiff_Stack(dataEnvironment.DataOutFolder + "ProjectionObject" + i + ".tif", DensityGrid.DataWhole);
                    DensityGrid = null;
                    GC.Collect();
                }

                ReducedDensityGrid = ProjectionArrayObject.LoadMultipleFiles(dataEnvironment.DataOutFolder + "ProjectionObject0.dat", dataEnvironment.DataOutFolder + "ProjectionObject1.dat", 400);

                /*/ReducedDensityGrid = ProjectionArrayObject.LoadMultipleFiles(dataEnvironment.DataOutFolder + "ProjectionObject0.dat", dataEnvironment.DataOutFolder + "ProjectionObject1.dat");
                //ReducedDensityGrid = new float[200, 200, 200];
                Random rnd = new Random();
                for (int i = 0; i < 200; i++)
                    for (int j = 0; j < 200; j++)
                        for (int k = 0; k < 200; k++)
                            ReducedDensityGrid[i, j, k] = (float)(1000 * rnd.NextDouble());
                */
                dataEnvironment.ProgressLog.AddSafe("Recon", "Recon Completed");
                //GC.Collect();*//
            }
            catch (Exception ex)
            {
                dataEnvironment.ProgressLog.AddSafe("Recon ERror", ex.Message);
            }
        }

        #region Post Process
        ImageHolder StackImage = null;
        // string CurvatureBins = "";
        protected virtual void NormalizedImage()
        {
            double CutValue = -500;
            //  if (DensityGrid.Data != null)
            //     ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder.DoRemoveFBPCylinder(ref DensityGrid.Data);
            //else
            CutValue = ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder.DoRemoveFBPCylinder(ref ReducedDensityGrid);
            dataEnvironment.ProgressLog.AddSafe("Cylinder Boundry", CutValue.ToString());
            //if (DensityGrid.Data != null)
            //   ImageViewer3D.PythonScripting.CellCT.NormalizeFBPVolumeEffect.NormalizeFBPVolume(ref DensityGrid.Data, dataEnvironment.AllImages.Count, true);
            //else
            float cValue = (float)CutValue * 2;
            ImageViewer3D.PythonScripting.CellCT.NormalizeFBPVolumeEffect.NormalizeFBPVolume(ref ReducedDensityGrid, dataEnvironment.AllImages.Count, true, cValue);

            try
            {
                double max, min, ave, sd, rangeMin;
                long[] Curvatures;
                MathHelpLib._3DStuff._3DInterpolation.GetCurvature(ref ReducedDensityGrid, cValue * 3, out ave, out max, out min, out sd, out Curvatures, out rangeMin);

                dataEnvironment.ProgressLog.AddSafe("Curve SD", sd.ToString());
                dataEnvironment.ProgressLog.AddSafe("Curve Max", max.ToString());
                dataEnvironment.ProgressLog.AddSafe("Curve Min", min.ToString());
                dataEnvironment.ProgressLog.AddSafe("Curve Ave", ave.ToString());

                dataEnvironment.ProgressLog.AddSafe("Curve RangeMin", rangeMin.ToString());
                string junk = "";
                for (int i = 0; i < Curvatures.Length; i++)
                    junk += Curvatures[i] + ", ";
                //   CurvatureBins = junk;

            }
            catch { }

            double stackF4 = 1;
            double onAxisF4 = 1;
            try
            {
                // ImageHolder StackImage = null;
                string stackPath = dataEnvironment.ScriptParams["StackFolder"];
                if (File.Exists(stackPath + "\\000_0000m.ivg"))
                    StackImage = MathHelpsFileLoader.LoadIVGFile(stackPath + "\\000_0000m.ivg");
                if (File.Exists(stackPath + "\\000_0000m.png"))
                    StackImage = MathHelpsFileLoader.Load_Bitmap(stackPath + "\\000_0000m.png");

                if (ColorImage == true)
                {
                    StackImage = MathHelpsFileLoader.FixVisionGateImage(StackImage, 2);
                }

                ImageViewer.Filters.Effects.Flattening.FlattenEdges1DErrorCorrected.FlattenImageEdgesGlobal(StackImage);

                dataEnvironment.AllImages[1] = StackImage;
                dataEnvironment.AllImages[1] = ProcessImageDivide(1);
                FineCellHalf = ReconCellHalf;
                FineCellSize = ReconCellSize;
                StackImage = ProcessImageClip(1);

                Point CellCenter;
                //if (FirstCellGuess == Rectangle.Empty)
                CellCenter = new Point((int)CellPositionGuesses.X_Positions[1], (int)CellPositionGuesses.Y_Positions[1]);
                //  else 
                //     CellCenter = new Point(FirstCellGuess.X,FirstCellGuess.Y );

                string[] s = PythonHelps.EvaluateRecon(ReducedDensityGrid, StackImage, out stackF4, out onAxisF4);
                dataEnvironment.ProgressLog.AddSafe("Eval Coef", s[0]);
                dataEnvironment.ProgressLog.AddSafe("Eval Factor", s[1]);
                dataEnvironment.ProgressLog.AddSafe("Eval F4", s[2]);
                dataEnvironment.ProgressLog.AddSafe("Eval Cross Coorelation", s[3]);
                dataEnvironment.ProgressLog.AddSafe("Stack F4", s[4]);
                dataEnvironment.ProgressLog.AddSafe("ReconVsStack", s[5]);
            }
            catch (Exception ex)
            {

                dataEnvironment.ProgressLog.AddSafe("Eval Error", PythonHelps.FormatException(ex));
                // System.Diagnostics.Debug.Print("");
            }


            try
            {
                string VGFolder = (string)dataEnvironment.ScriptParams["vgfolder"];

                if (Directory.Exists(VGFolder + "\\500pp\\recon_cropped_16bit") == true)
                    VGFolder = VGFolder + "\\500pp\\recon_cropped_16bit";
                else
                    VGFolder = VGFolder + "\\500pp\\recon_cropped_8bit";

                string[] files = Directory.GetFiles(VGFolder);

                ImageHolder ih = new ImageHolder(files[files.Length / 2]);
                Bitmap b = ih.ToBitmap();
                double[,] image = ih.ToDataIntensityDouble();


                double FV = FocusValueTool.FocusValueF4(image);

                dataEnvironment.ProgressLog.AddSafe("VG_VsStack", (FV / stackF4).ToString());



                dataEnvironment.ProgressLog.AddSafe("VG_VsASU", (onAxisF4 / FV).ToString());
            }
            catch { }
        }

        protected virtual void SaveVolume()
        {
            if (dataEnvironment.ScriptParams["SaveAsCCT"].ToLower() == "true")
            {
                MathHelpsFileLoader.Save_Tiff_Stack(dataEnvironment.DataOutFolder + "ProjectionObject.tif", ReducedDensityGrid);
                //  DensityGrid.SaveFile(dataEnvironment.DataOutFolder + "ProjectionObject.tif");
            }

            /*if (dataEnvironment.ScriptParams["SaveAsRawDouble"].ToLower() == "true")
            {
                DensityGrid.SaveFileRaw(dataEnvironment.DataOutFolder + "ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.Float32);
            }
            if (dataEnvironment.ScriptParams["SaveAsRawFloat"].ToLower() == "true")
            {
                DensityGrid.SaveFileRaw(dataEnvironment.DataOutFolder + "ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.Float32);
            }
            if (dataEnvironment.ScriptParams["SaveAsRawInt"].ToLower() == "true")
            {
                DensityGrid.SaveFileRaw(dataEnvironment.DataOutFolder + "ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.UInt16);
            }*/
            if (dataEnvironment.ScriptParams["Save8Bit"].ToLower() == "true")
            {
                if (Directory.Exists(dataEnvironment.DataOutFolder + "VirtualStack8\\") == false)
                    Directory.CreateDirectory(dataEnvironment.DataOutFolder + "VirtualStack8\\");
                else
                {
                    Directory.Delete(dataEnvironment.DataOutFolder + "VirtualStack8\\", true);
                    Directory.CreateDirectory(dataEnvironment.DataOutFolder + "VirtualStack8\\");
                }
                MathHelpsFileLoader.Save_Tiff_VirtualStack(dataEnvironment.DataOutFolder + "VirtualStack8\\VStack.tif", ReducedDensityGrid, -20, 8);
            }
            if (dataEnvironment.ScriptParams["Save16Bit"].ToLower() == "true")
            {
                if (Directory.Exists(dataEnvironment.DataOutFolder + "VirtualStack16\\") == false)
                    Directory.CreateDirectory(dataEnvironment.DataOutFolder + "VirtualStack16\\");
                else
                {
                    Directory.Delete(dataEnvironment.DataOutFolder + "VirtualStack16\\", true);
                    Directory.CreateDirectory(dataEnvironment.DataOutFolder + "VirtualStack16\\");
                }
                MathHelpsFileLoader.Save_Tiff_VirtualStack(dataEnvironment.DataOutFolder + "VirtualStack16\\VStack.tif", ReducedDensityGrid, -20, 16);
            }

            if (Directory.Exists(dataEnvironment.TempPath + "MIP\\") == false)
                Directory.CreateDirectory(dataEnvironment.TempPath + "MIP\\");
            else
            {
                string[] OldFrames = Directory.GetFiles(dataEnvironment.TempPath + "MIP\\");
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
            ReplaceStringDictionary PassData = dataEnvironment.PassData;
            IEffect Filter;

            try
            {
                double Ave = PythonHelps.Average(CellPositionGuesses.FocusValue);
                dataEnvironment.ProgressLog.AddSafe("FocusValueAverage", Ave.ToString());
                dataEnvironment.ProgressLog.AddSafe("FocusValueSD", (PythonHelps.Stdev(CellPositionGuesses.FocusValue, Ave) / Ave * 100d).ToString());
                dataEnvironment.ProgressLog.AddSafe("FocusValueMax", PythonHelps.max(CellPositionGuesses.FocusValue).ToString());
                dataEnvironment.ProgressLog.AddSafe("FocusValueMin", PythonHelps.min(CellPositionGuesses.FocusValue).ToString());

                dataEnvironment.ProgressLog.AddSafe("FocusValue", Ave.ToString());
                dataEnvironment.ProgressLog.AddSafe("FocusVar", ((PythonHelps.max(CellPositionGuesses.FocusValue) - PythonHelps.min(CellPositionGuesses.FocusValue)) / Ave).ToString());

                //************************************************  Focus Values ***************************************************************************

            }
            catch { }
        }

        protected virtual void SaveExamples()
        {
            #region SaveExamples
            try
            {
                ImageHolder image = dataEnvironment.AllImages[0];
                MathHelpsFileLoader.Save_Bitmap(dataEnvironment.DataOutFolder + "FirstPP.bmp", image);
            }
            catch { }

            try
            {
                MathHelpsFileLoader.Save_Bitmap(dataEnvironment.DataOutFolder + "Stack.bmp", StackImage);
            }
            catch { }

            try
            {
                ImageHolder image = dataEnvironment.AllImages[dataEnvironment.AllImages.Count / 4];
                MathHelpsFileLoader.Save_Bitmap(dataEnvironment.DataOutFolder + "QuarterPP.bmp", image);
            }
            catch { }

            try
            {
                ImageHolder image = dataEnvironment.AllImages[(int)(dataEnvironment.AllImages.Count * 5d / 8d)];
                MathHelpsFileLoader.Save_Bitmap(dataEnvironment.DataOutFolder + "HalfPP.bmp", image);
            }
            catch { }

            try
            {
                ImageHolder image = dataEnvironment.AllImages[(int)(dataEnvironment.AllImages.Count * 1d / 8d)];
                MathHelpsFileLoader.Save_Bitmap(dataEnvironment.DataOutFolder + "LastQuarterPP.bmp", image);
            }
            catch { }

            try
            {
                ImageHolder image = dataEnvironment.AllImages[dataEnvironment.AllImages.Count - 1];
                MathHelpsFileLoader.Save_Bitmap(dataEnvironment.DataOutFolder + "lastPP.bmp", image);
            }
            catch { }


            MathHelpsFileLoader.SaveCross(dataEnvironment.DataOutFolder + "CrossSections.jpg", ReducedDensityGrid);

            //DensityGrid.SaveCross(dataEnvironment.DataOutFolder + "CrossSections.jpg");
            #endregion

        }

        protected virtual void SaveMIP()
        {
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = dataEnvironment.PassData;


            if (dataEnvironment.ScriptParams["SaveMIP"].ToLower() == "true")
            {
                try
                {
                    ImageViewer.PythonScripting.Projection.MakeMIPMovie3Effect Filter = new ImageViewer.PythonScripting.Projection.MakeMIPMovie3Effect();
                    //  if (DensityGrid.Data != null)
                    //      BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DensityGrid.Data, dataEnvironment.DataOutFolder + "MIP.avi", dataEnvironment.TempPath);
                    //  else
                    BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ReducedDensityGrid, dataEnvironment.DataOutFolder + "MIP.avi", dataEnvironment.TempPath);

                    BitmapImage.Save(dataEnvironment.DataOutFolder + "Forward1.bmp");
                }
                catch { }
            }
        }

        protected virtual void MoveStack()
        {
            try
            {
                //todo: cut down the stack based by the imagesize from reconquality check tool.  just need a nice tool
                if (dataEnvironment.ScriptParams["CopyStack"].ToLower() == "true")
                {
                    MathHelpLib.ProjectionFilters.CopyAndCutStackEffect.CopyStack(dataEnvironment.ExperimentFolder + "stack\\000", dataEnvironment.DataOutFolder + "stack", true, dataEnvironment.ProgressLog["IsColor"].ToString().ToLower() == "true");
                }
            }
            catch { }

        }

        protected virtual void DoQualityChecks()
        {
            //do the image quality tests
            if (dataEnvironment.ScriptParams["DoConvolutionQuality"].ToLower() == "true")
            {
                try
                {
                    dataEnvironment.ProgressLog.AddSafe("Position", "ImageQuality");
                    //  MathHelpLib.ProjectionFilters.ReconQualityCheckTool.CompareProjection(DensityGrid.DataWhole, dataEnvironment.ExperimentFolder, null);
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
            ReplaceStringDictionary PassData = dataEnvironment.PassData;
            IEffect Filter;

            NormalizedImage();

            ////////////////////////////////////////////save the volume////////////////////////////////////////////
            SaveVolume();
            ///////////////////////////////////////////////Save the example images //////////////////////////////
            SaveExamples();
            //************************************************  Focus Values ***************************************************************************
            SaveArrays();
            /////////////////////////////////////////////////////////////Save movies//////////////////////////////////////////
            SaveMIP();

            MoveStack();

            DoQualityChecks();

            foreach (KeyValuePair<string, object> kvp in PassData)
            {
                dataEnvironment.ProgressLog.AddSafe(kvp.Key, kvp.Value.ToString());
            }

            dataEnvironment.ProgressLog.AddSafe("ImageType", Path.GetExtension(dataEnvironment.WholeFileList[0]));

            dataEnvironment.ProgressLog.AddSafe("Background", "New_Background");
            ///the resources should be cleaned up on the calling thread
            // DensityGrid.ConvolutionFilter.Dispose();
            ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect.Dispose(dataEnvironment);

            try
            {
                // dataEnvironment.ProgressLog.AddSafe("Curvatures", CurvatureBins);
            }
            catch { }
        }
        #endregion

        protected virtual void DoRun(Dictionary<string, object> Variables)
        {
            background = null;

            /*
            ImageHolder background0 = dataEnvironment.AllImages[0];
            ImageHolder background1 = dataEnvironment.AllImages[(int)(dataEnvironment.AllImages.Count * 1d / 4d)];
            ImageHolder background2 = dataEnvironment.AllImages[(int)(dataEnvironment.AllImages.Count * 2d / 4d)];
            ImageHolder background3 = dataEnvironment.AllImages[(int)(dataEnvironment.AllImages.Count * 3d / 4d)];

            background = ImageHolder.CombineByBrightest(background0, background1, background2, background3);

            background.ConvertToGrayScaleAverage();
            background.NormalizeToValue(1);*/

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
            PythonHelps.BatchLoopThroughImages("BeforeConvolution", this, dataEnvironment, true);

            PreBatchProcessRecon();
            PostBatchProcess();
        }

        public void RunScript(Dictionary<string, object> Variables)
        {
            ImageFileListIn = (List<ImageFile>)Variables["ImageFileListIn"];

            dataEnvironment = (DataEnvironment)Variables["dataEnvironment"];

            dataEnvironment.TempPath = (string)Variables["TempPath"];
            dataEnvironment.ImageDisp = (ImageDisplayer)Variables["ImageDisp"];
            dataEnvironment.DataOutFolder = (string)Variables["DataPath"];
            dataEnvironment.Executable = (string)Variables["Executable"];
            dataEnvironment.ExecutablePath = (string)Variables["ExecutablePath"];


            dataEnvironment.PassData = (ReplaceStringDictionary)Variables["GlobalPassData"];
            //RunningThreaded = (bool)Variables["RunningThreaded"];

            FluorImage = (((string)Variables["FluorCell"]).ToLower() == "true");

            dataEnvironment.ScriptParams = (Dictionary<string, string>)Variables["ScriptParams"];


            CellPositionGuesses = new CellPositions(dataEnvironment.AllImages.Count);

            if (0 == 0)
            {
                try
                {
                    Dictionary<string, string> Values = EffectHelps.OpenXMLAndGetTags(dataEnvironment.ExperimentFolder + "\\info.xml", new string[] { "Color", "SPECIMEN/CellXPos", "SPECIMEN/CellYPos", "SPECIMEN/BoxWidth", "SPECIMEN/BoxHeight" });
                    if (Values["Color"].ToLower() == "true")
                        ColorImage = true;
                    else
                        ColorImage = false;
                    dataEnvironment.ProgressLog.AddSafe("IsColor", ColorImage.ToString());

                    
                    int x = int.Parse(Values["SPECIMEN/CellXPos"]);
                    int y = int.Parse(Values["SPECIMEN/CellYPos"]);
                    int w = int.Parse(Values["SPECIMEN/BoxWidth"]);
                    int h = int.Parse(Values["SPECIMEN/BoxHeight"]);

                    if (ColorImage)
                    {
                        x /= 2; y /= 2; w /= 2; h /= 2;

                    }

                    FirstCellGuess = new Rectangle(x, y, w, h);

                }
                catch
                {

                    //dataEnvironment.ProgressLog.AddSafe("IsColor", "True");
                }
            }

            DoRun(Variables);
        }

    }
}

