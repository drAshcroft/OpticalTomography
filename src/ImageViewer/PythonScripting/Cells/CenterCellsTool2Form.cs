using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using ZedGraph;
using System.IO;
using System.Threading;
using ImageViewer.PythonScripting;
using MathHelpLib;
using ImageViewer.PythonScripting.Threads;
using MathHelpLib.ProjectionFilters;
using ImageViewer.PythonScripting.Projection;
using ImageViewer.Filters.Blobs;

namespace ImageViewer.Filters
{
    public partial class CenterCellsTool2Form : Form, IEffect
    {
        #region menu
        public string EffectName { get { return "Center Cells"; } }
        public string EffectMenu { get { return "Macros"; } }
        public string EffectSubMenu { get { return "Cell Tools"; } }
        public int OrderSuggestion { get { return 1; } }

        #endregion

        #region Lines
        public class CorrectedLines
        {
            private double[] mCorrectedX;
            private double[] mCorrectedY;

            public double[] CorrectedX
            {
                get { return mCorrectedX; }
            }
            public double[] CorrectedY
            {
                get { return mCorrectedY; }
            }
            public CorrectedLines(double[] CorrectedX, double[] CorrectedY)
            {
                mCorrectedX = CorrectedX;
                mCorrectedY = CorrectedY;
            }

        }

        string[] ImageFilenames;
        double[] X_Positions;
        double[] Y_Positions;

        double[] Use_X;
        double[] Use_Y;

        int ImageIndex;

        private void CreateGraph(double[,] Data)
        {
            GraphPane myPane = zedgraphcontrol.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "Intensity";
            myPane.YAxis.Title.Text = "Count";

            // Make up some data points from the Sine function
            PointPairList list = new PointPairList();
            for (int x = 0; x < Data.GetLength(1); x++)
            {
                list.Add(Data[0, x], Data[1, x]);
            }
            myPane.CurveList.Clear();
            // Generate a blue curve with circle symbols, and "My Curve 2" in the legend
            LineItem myCurve = myPane.AddCurve("", list, Color.Blue, SymbolType.None);
            // Fill the area under the curve with a white-red gradient at 45 degrees
            myCurve.Line.Fill = new Fill(Color.Red);

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White);

            // Fill the pane background with a color gradient
            myPane.Fill = new Fill(Color.White);

            // Calculate the Axis Scale Ranges
            zedgraphcontrol.AxisChange();
            zedgraphcontrol.Invalidate();
        }

        public void GraphLine(double[] DataX, double[] DataY)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedgraphcontrol.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "Frame";
            myPane.YAxis.Title.Text = "Position";

            // Make up some mDataDouble points based on the Sine function
            PointPairList listX = new PointPairList();
            PointPairList listY = new PointPairList();
            for (int i = 0; i < DataX.Length - 1; i++)
            {
                listX.Add(i, DataX[i]);
                listY.Add(i, DataY[i]);
            }

            myPane.CurveList.Clear();

            // Generate offsetX red curve with diamond symbols, and "Alpha" in the legend
            LineItem myCurveX = myPane.AddCurve("", listX, Color.Red, SymbolType.None);
            LineItem myCurveY = myPane.AddCurve("", listY, Color.Blue, SymbolType.None);

            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters

            zedgraphcontrol.AxisChange();

            zedgraphcontrol.Invalidate();

        }
        public void GraphLine(double[] DataX, double[] DataY, double[,] FitX, double[,] FitY)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedgraphcontrol.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "Frame";
            myPane.YAxis.Title.Text = "Position";

            // Make up some mDataDouble points based on the Sine function
            PointPairList listX = new PointPairList();
            PointPairList listY = new PointPairList();
            PointPairList fX = new PointPairList();
            PointPairList fY = new PointPairList();

            for (int i = 0; i < DataX.Length - 1; i++)
            {
                listX.Add(i, DataX[i]);
                listY.Add(i, DataY[i]);
                fX.Add(i, FitX[1, i]);
                fY.Add(i, FitY[1, i]);
            }

            myPane.CurveList.Clear();

            // Generate offsetX red curve with diamond symbols, and "Alpha" in the legend
            LineItem myCurveX = myPane.AddCurve("", listX, Color.Red, SymbolType.None);
            LineItem myCurveY = myPane.AddCurve("", listY, Color.Blue, SymbolType.None);
            LineItem myFitY = myPane.AddCurve("", fX, Color.Black, SymbolType.None);
            LineItem myFitX = myPane.AddCurve("", fY, Color.Black, SymbolType.None);

            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters

            zedgraphcontrol.AxisChange();

            zedgraphcontrol.Invalidate();

        }
        #endregion

        public string getMacroString()
        {
            return EffectHelps.FormatMacroString(this, mFilterToken);
        }

        void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (mFilterToken == null)
                mFilterToken = DefaultProperties;
            mFilterToken[1] = trackBar1.Value;
            DoRun();
        }

        public CenterCellsTool2Form()
        {
            InitializeComponent();
        }

        #region passdata
        public virtual bool PassesPassData
        { get { return true; } }

        protected ReplaceStringDictionary mPassData;

        public ReplaceStringDictionary PassData
        {
            get { return mPassData; }
            set { mPassData = value; }
        }

        public string PassDataDescription
        {
            get { return "PassData['CorrectedX']=double[] of corrected X coords\nPassData['CorrectedY']=double[] of corrected Y coords"; }
        }
        #endregion


        public static ImageHolder ClipImage(DataEnvironment dataEnvironment, int ImageI, double[] X_Positions, double[] Y_Positions, int CellHalf, int CellSize)
        {
            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageI];

            // Bitmap b1 = BitmapImage.ToBitmap();
            Rectangle CellArea = new Rectangle((int)Math.Truncate(X_Positions[ImageI] - CellHalf), (int)Math.Truncate(Y_Positions[ImageI] - CellHalf), CellSize, CellSize);

            BitmapImage = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(BitmapImage, CellArea);
            BitmapImage = ImageViewer.Filters.Adjustments.InvertEffect.InvertImage(BitmapImage);


            BitmapImage.ImproveMinimum(.1);

            // Bitmap b2 = BitmapImage.ToBitmap();

            //  int w = b1.Width + b2.Width;

            return BitmapImage;
        }

        public static ImageHolder ClipImagePeriodic(ImageHolder BitmapImage, double[] X_Positions, double[] Y_Positions, int CellHalf, int CellSize)
        {

            int ImageI = 0;
            // Bitmap b1 = BitmapImage.ToBitmap();
            Rectangle CellArea = new Rectangle((int)Math.Truncate(X_Positions[ImageI] - CellHalf), (int)Math.Truncate(Y_Positions[ImageI] - CellHalf), CellSize, CellSize);



            if (CellArea.Top > 0 && CellArea.Bottom < BitmapImage.Height)
            {
                BitmapImage = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(BitmapImage, CellArea);
            }
            else if (CellArea.Top < 0)
            {
                ImageHolder iOut = new ImageHolder(CellSize, CellSize, 1);
                unchecked
                {
                    //copy the correct data into the correct spot
                    int startY = -1 * CellArea.Top;
                    for (int y = 0; y < CellArea.Bottom; y++)
                    {
                        int startX = 0;
                        for (int x = CellArea.Left; x < CellArea.Right; x++)
                        {
                            iOut.ImageData[startY, startX, 0] = BitmapImage.ImageData[y, x, 0];
                            startX++;
                        }
                        startY++;
                    }
                    int endY = -1 * CellArea.Top;
                    startY = 0;
                    //grab data from other side of the image to pad the open space
                    for (int y = CellArea.Bottom; y < (CellArea.Bottom + endY); y++)
                    {
                        int startX = 0;
                        for (int x = CellArea.Left; x < CellArea.Right; x++)
                        {
                            iOut.ImageData[startY, startX, 0] = BitmapImage.ImageData[y, x, 0];
                            startX++;
                        }
                        startY++;
                    }
                }
                BitmapImage = iOut;
                Bitmap b = BitmapImage.ToBitmap();
                int w = b.Width;
            }
            else if (CellArea.Bottom > BitmapImage.Height)
            {
                unchecked
                {
                    ImageHolder iOut = new ImageHolder(CellSize, CellSize, 1);
                    //copy the correct data into the correct spot
                    int startY = 0;
                    for (int y = CellArea.Top; y < BitmapImage.Height; y++)
                    {
                        int startX = 0;
                        for (int x = CellArea.Left; x < CellArea.Right; x++)
                        {
                            iOut.ImageData[startY, startX, 0] = BitmapImage.ImageData[y, x, 0];
                            startX++;
                        }
                        startY++;
                    }
                    int endY = CellArea.Bottom - BitmapImage.Height;

                    //grab data from other side of the image to pad the open space
                    for (int y = CellArea.Top - endY; y < CellArea.Top; y++)
                    {
                        int startX = 0;
                        for (int x = CellArea.Left; x < CellArea.Right; x++)
                        {
                            iOut.ImageData[startY, startX, 0] = BitmapImage.ImageData[y, x, 0];
                            startX++;
                        }
                        startY++;
                    }

                    BitmapImage = iOut;
                    Bitmap b = BitmapImage.ToBitmap();
                    int w = b.Width;
                }

            }

            BitmapImage = ImageViewer.Filters.Adjustments.InvertEffect.InvertImage(BitmapImage);


            BitmapImage.ImproveMinimum(.1);

            // Bitmap b2 = BitmapImage.ToBitmap();

            //  int w = b1.Width + b2.Width;

            return BitmapImage;
        }

        public static ImageHolder ClipImagePeriodic(DataEnvironment dataEnvironment, int ImageI, double[] X_Positions, double[] Y_Positions, int CellHalf, int CellSize)
        {
            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageI];

            // Bitmap b1 = BitmapImage.ToBitmap();
            Rectangle CellArea = new Rectangle((int)Math.Truncate(X_Positions[ImageI] - CellHalf), (int)Math.Truncate(Y_Positions[ImageI] - CellHalf), CellSize, CellSize);



            if (CellArea.Top > 0 && CellArea.Bottom < BitmapImage.Height)
            {
                BitmapImage = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(BitmapImage, CellArea);
            }
            else if (CellArea.Top < 0)
            {
                ImageHolder iOut = new ImageHolder(CellSize, CellSize, 1);
                unchecked
                {
                    //copy the correct data into the correct spot
                    int startY = -1 * CellArea.Top;
                    for (int y = 0; y < CellArea.Bottom; y++)
                    {
                        int startX = 0;
                        for (int x = CellArea.Left; x < CellArea.Right; x++)
                        {
                            iOut.ImageData[startY, startX, 0] = BitmapImage.ImageData[y, x, 0];
                            startX++;
                        }
                        startY++;
                    }

                    double average = 0;
                    double std = 0;
                    double cc = 0;
                    for (int x = CellArea.Left; x < CellArea.Right; x++)
                    {
                        average += BitmapImage.ImageData[CellArea.Bottom, x, 0];
                        cc++;
                    }
                    average = average / cc;
                    double d = 0;
                    for (int x = CellArea.Left; x < CellArea.Right; x++)
                    {
                        d = (BitmapImage.ImageData[CellArea.Bottom, x, 0] - average);
                        std = d * d;
                    }

                    std = Math.Sqrt(std / cc) * 4;

                    Random rnd = new Random();
                    int endY = -1 * CellArea.Top;
                    startY = 0;
                    //grab data from other side of the image to pad the open space
                    for (int y = CellArea.Bottom; y < (CellArea.Bottom + endY); y++)
                    {
                        int startX = 0;
                        for (int x = CellArea.Left; x < CellArea.Right; x++)
                        {
                            iOut.ImageData[startY, startX, 0] = (float)(average + std * (.5 - rnd.NextDouble()));
                            startX++;
                        }
                        startY++;
                    }
                }
                //  BitmapImage = iOut;
                //  Bitmap b = BitmapImage.ToBitmap();
                // int w = b.Width;
            }
            else if (CellArea.Bottom > BitmapImage.Height)
            {
                unchecked
                {
                    ImageHolder iOut = new ImageHolder(CellSize, CellSize, 1);
                    //copy the correct data into the correct spot
                    int startY = 0;
                    for (int y = CellArea.Top; y < BitmapImage.Height; y++)
                    {
                        int startX = 0;
                        for (int x = CellArea.Left; x < CellArea.Right; x++)
                        {
                            iOut.ImageData[startY, startX, 0] = BitmapImage.ImageData[y, x, 0];
                            startX++;
                        }
                        startY++;
                    }
                    int endY = CellArea.Bottom - BitmapImage.Height;

                    double average = 0;
                    double std = 0;
                    double cc = 0;
                    for (int x = CellArea.Left; x < CellArea.Right; x++)
                    {
                        average += BitmapImage.ImageData[CellArea.Top, x, 0];
                        cc++;
                    }
                    average = average / cc;
                    double d = 0;
                    for (int x = CellArea.Left; x < CellArea.Right; x++)
                    {
                        d = (BitmapImage.ImageData[CellArea.Top, x, 0] - average);
                        std = d * d;
                    }

                    std = Math.Sqrt(std / cc) * 4;

                    Random rnd = new Random();

                    //grab data from other side of the image to pad the open space
                    for (int y = CellArea.Top - endY; y < CellArea.Top; y++)
                    {
                        int startX = 0;
                        for (int x = CellArea.Left; x < CellArea.Right; x++)
                        {
                            iOut.ImageData[startY, startX, 0] = (float)(average + std * (.5 - rnd.NextDouble()));
                            startX++;
                        }
                        startY++;
                    }

                    // BitmapImage = iOut;
                    // Bitmap b = BitmapImage.ToBitmap();
                    // int w = b.Width;
                }

            }

            BitmapImage = ImageViewer.Filters.Adjustments.InvertEffect.InvertImage(BitmapImage);


            BitmapImage.ImproveMinimum(.1);

            // Bitmap b2 = BitmapImage.ToBitmap();

            //  int w = b1.Width + b2.Width;

            return BitmapImage;
        }


        public static double[,] DoSliceConvolution(DataEnvironment dataEnvironment, ImageHolder BitmapImage, MathHelpLib.ProjectionFilters.ProjectionArrayObject reconObject)
        {
            double[,] Slice = null;

            Slice = reconObject.ConvolutionFilter.DoConvolution(dataEnvironment, BitmapImage, reconObject.impulse, 2, 8);

            return Slice;//.RotateArray();
            //return Slice;
        }

        private static ProjectionArrayObject CreateTempRecon(DataEnvironment dataEnvironment, int CellSize, double[] X_Positions, double[] Y_Positions, double DownSampleScale, int nSlices)
        {
            int CellHalf = CellSize / 2;

            ProjectionArrayObject DensityGrid = new ProjectionArrayObject(true, (int)(CellSize * DownSampleScale + .5), (int)(CellSize * DownSampleScale + .5), (int)(CellSize * DownSampleScale + .5), -1, 1, -1, 1, -1, 1);

            // ConvolutionMethod DesiredMethod = ConvolutionMethod.Convolution1D;

            ImageViewer.PythonScripting.Projection.Convolution1D ConvolutionFilter = new ImageViewer.PythonScripting.Projection.Convolution1D();

            int FilterWidth = 256;//, EffectHelps.ConvertToInt(ParamDictionary["FBPResolution"])
            double[] impulse = Filtering.GetRealSpaceFilter("Han", FilterWidth, FilterWidth, (double)FilterWidth / 2d);

            Thread[] Recons = new Thread[nSlices];
            for (int i = 0; i < Recons.Length; i++)
            {
                Recons[i] = new Thread(delegate(object ImageN)
                {
                    try
                    {
                        int ImageI = (int)ImageN;
                        ImageHolder BitmapImage = ClipImage(dataEnvironment, ImageI, X_Positions, Y_Positions, CellHalf, CellSize);
                        if (DownSampleScale != 1)
                            BitmapImage = ImageViewer.Filters.Adjustments.downSampleEffect.DownSampleImage(BitmapImage, DownSampleScale);
                        //  Bitmap b = BitmapImage.ToBitmap();
                        //  int w = b.Width;


                        //double[,] Slice = BitmapImage.ToDataIntensityDouble();
                        double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * ImageI;

                        double[,] Slice = DensityGrid.ConvolutionFilter.DoConvolution(dataEnvironment, BitmapImage, impulse, 2, 8);

                        ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2 FilterR = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2();
                        FilterR.DoEffect(dataEnvironment, null, null, Slice, DensityGrid, AngleRadians);
                        Console.Write(ImageI + " ");
                    }
                    catch { }
                });
            }

            double Gap = (double)(dataEnvironment.AllImages.Count) / (double)Recons.Length;
            for (int i = 0; i < Recons.Length; i++)
            {
                Recons[i].Start((int)(i * Gap));
            }

            for (int i = 0; i < Recons.Length; i++)
                Recons[i].Join();

            return DensityGrid;
        }

        public static double ReconCenter(DataEnvironment dataEnvironment, int CellSize, double[] X_Positions, double[] Y_Positions)
        {
            int tCellSize = (int)(CellSize * 1.3);
            // if (CellSize > 320) CellSize = 320;
            //CellSize *= 2;
            int CellHalf = tCellSize / 2;

            double DownSampleScale;
            if (tCellSize > 400)
                DownSampleScale = .25;
            else if (tCellSize > 300)
                DownSampleScale = .5;
            else
                DownSampleScale = .7;

            int nReconSlices = 41;

            //make a mock up reconstruction to create a very steady template.  
            ProjectionArrayObject DensityGrid = CreateTempRecon(dataEnvironment, tCellSize, X_Positions, Y_Positions, DownSampleScale, nReconSlices);

            //clean off the nasty ring
            DoRemoveFBPCylinder(ref DensityGrid.DataWhole);

            // create the forward projection for comparison
            double AngleStep = 360d / nReconSlices;
            Bitmap[] Ideals = MakeMIPMovie3Effect.DoForwardProjection("", "", DensityGrid.DataWhole, AngleStep);
            //  Bitmap[] Ideals = MakeMIPMovie3Effect.DoMIPProjection("", "", DensityGrid.DataWhole, AngleStep);
            for (int i = 0; i < Ideals.Length; i++)
            {
                //    Ideals[i].Save("c:\\temp\\m" + i.ToString() + ".bmp");
            }
            //clear out the memory
            DensityGrid.DataWhole = null;
            DensityGrid = null;
            GC.Collect();

            //clean up the forward projections to make them ideal
            ImageHolder[] IdealsH = new ImageHolder[Ideals.Length];
            for (int i = 0; i < Ideals.Length; i++)
            {
                IdealsH[i] = new ImageHolder(Ideals[i], 0);
                if (DownSampleScale != 1)
                    IdealsH[i] = ImageViewer.Filters.Adjustments.downSampleEffect.UpSampleImage(IdealsH[i], 1d / DownSampleScale);
                IdealsH[i].ImproveMinimum(.2);


                // IdealsH[i] = ImageViewer.Filters.Effects.Blurs.GaussianBlurTool.DoGuassSmooth(IdealsH[i], 5);
            }

            for (int i = 0; i < Ideals.Length; i++)
            {
                //    IdealsH[i].ToBitmap().Save("c:\\temp\\m" + i.ToString() + ".bmp");
            }

            double[][] FixedPositions = InterpolatedPhaseCorrelation(dataEnvironment, IdealsH, tCellSize, X_Positions, Y_Positions, AngleStep);

            return EstimateCellSize(dataEnvironment, CellSize, FixedPositions[0], FixedPositions[1]);
        }


        private static double[][] InterpolatedPhaseCorrelation(DataEnvironment dataEnvironment, ImageHolder[] IdealsH, int CellSize, double[] X_Positions, double[] Y_Positions, double AngleStepDegrees)
        {
            //now use the limited set to create a full set of images, then use phase correlation to line the images up.  
            // to be implemented, subpixel alignment.
            Thread[] PPs = new Thread[dataEnvironment.AllImages.Count];
            double AngleStep = AngleStepDegrees / 180d * Math.PI;
            for (int j = 0; j < dataEnvironment.AllImages.Count; j++)
            {
                PPs[j] = new Thread(delegate(object index)
                {
                    try
                    {
                        int i = (int)index;

                        double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * i + 2d * Math.PI / 4d;
                        int ImageIndex = ((int)Math.Floor(AngleRadians / AngleStep)) % IdealsH.Length;
                        int Image2 = ((int)Math.Floor(AngleRadians / AngleStep) + 1) % IdealsH.Length;
                        double u = Math.Abs((AngleRadians - Math.Floor(AngleRadians / AngleStep) * AngleStep) / AngleStep);

                        //create the interpolated image
                        ImageHolder CompareImage = ImagingTools.BlendImages(IdealsH[ImageIndex], IdealsH[Image2], (float)u);

                        //cut out the test image
                        ImageHolder originalImage = ClipImage(dataEnvironment, i, X_Positions, Y_Positions, CellSize / 2, CellSize);

                        PointF shift = MathHelpLib.MathFFTHelps.FindShift(originalImage.ImageData, CompareImage.ImageData);

                        X_Positions[i] += shift.X;
                        Y_Positions[i] += shift.Y;

                        Console.Write(i + " ");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                    }
                });
            }

            //start each batch of pps in limited amounts to prevent memory errors
            int cc = 0;
            int sCC = 0;
            int step = 50;
            for (int i = 0; i < dataEnvironment.AllImages.Count; i += step)
            {
                sCC = cc;
                for (int j = 0; j < step; j++)
                {
                    if (cc < dataEnvironment.AllImages.Count)
                    {
                        PPs[cc].Start(cc);
                        cc++;
                    }
                }
                for (int j = sCC; j < cc; j++)
                {
                    PPs[j].Join();
                }
                // GC.Collect();
            }

            //clean up the last little bit of PPs
            for (; cc < dataEnvironment.AllImages.Count; cc++)
            {
                PPs[cc].Start(cc);

            }

            for (int i = 0; i < PPs.Length; i++)
                PPs[i].Join();

            double[][] OutArrays = new double[][] { X_Positions, Y_Positions };

            return OutArrays;
        }

        private static double EstimateCellSize(DataEnvironment dataEnvironment, int CellSize, double[] X_Positions, double[] Y_Positions)
        {
            int nSamples = 15;

            List<int> tCellSize = new List<int>();
            for (int i = 0; i < dataEnvironment.AllImages.Count; i += (dataEnvironment.AllImages.Count / nSamples))
            {
                ImageHolder BitmapImage = ClipImage(dataEnvironment, i, X_Positions, Y_Positions, CellSize / 2, CellSize);
                BitmapImage = ImageViewer.Filters.Thresholding.IterativeThresholdEffect.IterativeThreshold(BitmapImage);
                BlobDescription[] Blobs = null;
                Blobs = ImageViewer.Filters.Blobs.CenterOfGravityTool.DoCOG(BitmapImage);

                if (Blobs[0].BlobBounds.Width > Blobs[0].BlobBounds.Height)
                    tCellSize.Add(Blobs[0].BlobBounds.Width);
                else
                    tCellSize.Add(Blobs[0].BlobBounds.Height);
            }

            tCellSize.Sort();

            return tCellSize[tCellSize.Count - 4];
        }


        public static void ReconCenterFullSize(DataEnvironment dataEnvironment, int CellSize, double[] X_Positions, double[] Y_Positions)
        {
            if (CellSize > 320) CellSize = 320;
            //CellSize *= 2;
            int CellHalf = CellSize / 2;

            ProjectionArrayObject DensityGrid = new ProjectionArrayObject(true, CellSize, CellSize, CellSize, -1, 1, -1, 1, -1, 1);

            // ConvolutionMethod DesiredMethod = ConvolutionMethod.Convolution1D;

            ImageViewer.PythonScripting.Projection.Convolution1D ConvolutionFilter = new ImageViewer.PythonScripting.Projection.Convolution1D();

            int FilterWidth = 256;//, EffectHelps.ConvertToInt(ParamDictionary["FBPResolution"])
            double[] impulse = Filtering.GetRealSpaceFilter("Han", FilterWidth, FilterWidth, (double)FilterWidth / 2d);

            Thread[] Recons = new Thread[31];
            for (int i = 0; i < Recons.Length; i++)
            {
                Recons[i] = new Thread(delegate(object ImageN)
                    {
                        try
                        {
                            int ImageI = (int)ImageN;
                            ImageHolder BitmapImage = ClipImage(dataEnvironment, ImageI, X_Positions, Y_Positions, CellHalf, CellSize);

                            double[,] Slice = null;
                            Slice = ConvolutionFilter.DoConvolution(dataEnvironment, BitmapImage, impulse, 2, 8);
                            // Slice = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(Slice, ReconCutDownRect);
                            double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * ImageI;
                            Bitmap bbi = MathHelpLib.ImageProcessing.MathImageHelps.MakeBitmap(Slice);
                            int ghj = bbi.Width;
                            ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2 FilterR = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2();
                            FilterR.DoEffect(dataEnvironment, null, null, Slice, DensityGrid, AngleRadians);
                        }
                        catch { }
                    });
            }

            double Gap = (double)(dataEnvironment.AllImages.Count) / (double)Recons.Length;
            for (int i = 0; i < Recons.Length; i++)
            {
                Recons[i].Start((int)(i * Gap));
            }

            for (int i = 0; i < Recons.Length; i++)
                Recons[i].Join();

            DoRemoveFBPCylinder(ref DensityGrid.DataWhole);

            double AngleStep = 360d / Recons.Length;

            Bitmap[] Ideals = MakeMIPMovie3Effect.DoMIPProjection("", "", DensityGrid.DataWhole, AngleStep);


            DensityGrid.DataWhole = null;
            DensityGrid = null;
            GC.Collect();
            ImageHolder[] IdealsH = new ImageHolder[Ideals.Length];
            for (int i = 0; i < Ideals.Length; i++)
            {
                IdealsH[i] = new ImageHolder(Ideals[i]);
                IdealsH[i].ImproveMinimum(.2);
                IdealsH[i] = ImageViewer.Filters.Effects.Blurs.GaussianBlurTool.DoGuassSmooth(IdealsH[i], 5);
            }

            for (int i = 0; i < Ideals.Length; i++)
            {
                //  IdealsH[i].ToBitmap().Save("c:\\temp\\m" + i.ToString() + ".bmp");
            }

            Thread[] PPs = new Thread[dataEnvironment.AllImages.Count];

            AngleStep = AngleStep / 180d * Math.PI;

            for (int j = 0; j < dataEnvironment.AllImages.Count; j++)
            {
                PPs[j] = new Thread(delegate(object index)
                    {
                        try
                        {
                            int i = (int)index;

                            double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * i + 2d * Math.PI / 4d;
                            int ImageIndex = ((int)Math.Floor(AngleRadians / AngleStep)) % IdealsH.Length;
                            int Image2 = ((int)Math.Floor(AngleRadians / AngleStep) + 1) % IdealsH.Length;
                            double u = Math.Abs((AngleRadians - Math.Floor(AngleRadians / AngleStep) * AngleStep) / AngleStep);

                            ImageHolder CompareImage = ImagingTools.BlendImages(IdealsH[ImageIndex], IdealsH[Image2], (float)u);

                            ImageHolder originalImage = ClipImage(dataEnvironment, i, X_Positions, Y_Positions, CellHalf, CellSize);

                            if (i % 30 == 0)
                            {
                                /*   // originalImage.ImproveMinimum(.1);
                                   Bitmap b1 = CompareImage.ToBitmap();
                                   Bitmap b2 = originalImage.ToBitmap();

                                   b1.Save("c:\\temp\\m" + i.ToString() + ".bmp");
                                   b2.Save("c:\\temp\\mC" + i.ToString() + ".bmp");*/
                            }
                            PointF shift = MathHelpLib.MathFFTHelps.FindShift(originalImage.ImageData, CompareImage.ImageData);

                            //  if (Math.Abs(shift.X) < 10 && Math.Abs(shift.Y) < 10)
                            {
                                X_Positions[i] += shift.X;
                                Y_Positions[i] += shift.Y;
                            }

                            //int gh = b1.Width + b2.Width;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                        }
                    });
            }
            int cc = 0;
            int sCC = 0;
            for (int i = 0; i < dataEnvironment.AllImages.Count; i += 50)
            {
                sCC = cc;
                for (int j = 0; j < 30; j++)
                {
                    PPs[cc].Start(cc);
                    cc++;
                }
                for (int j = sCC; j < cc; j++)
                {
                    PPs[j].Join();
                }
                GC.Collect();
            }
            for (; cc < dataEnvironment.AllImages.Count; cc++)
            {
                PPs[cc].Start(cc);
            }

            for (int i = 0; i < PPs.Length; i++)
                PPs[i].Join();

        }

        public static double DoRemoveFBPCylinder(ref double[, ,] Data)
        {
            double RX = Data.GetLength(0) / 2d - 2;
            double RY = Data.GetLength(1) / 2d - 2;
            double RZ = Data.GetLength(2) / 2d - 2;
            double HalfI = Data.GetLength(0) / 2d;
            double HalfJ = Data.GetLength(1) / 2d;
            double HalfK = Data.GetLength(2) / 2d;

            double x;
            double y;
            double z;

            double min = double.MaxValue;
            double val;
            double sum = 0, count = 0;
            List<double> medianList = new List<double>();
            for (int i = 0; i < Data.GetLength(0); i++)
                for (int j = 0; j < Data.GetLength(1); j++)
                    for (int k = 0; k < Data.GetLength(2); k++)
                    {
                        x = (i - HalfI);
                        y = RX * (j - HalfJ) / RY;
                        z = RX * (k - HalfK) / RZ;
                        if (RX - (x * x + y * y) < 2)
                        {
                            val = Data[i, j, k];
                            if (val < min) min = val;
                            sum += val;
                            medianList.Add(val);
                            count++;
                        }
                    }

            medianList.Sort();
            sum = medianList[(int)(medianList.Count * 5f / 8f)];
            // sum /= count;
            if (sum < -500) sum = -500;
            // sum = min;
            sum = 0;
            //  double sum = -500;
            for (int i = 0; i < Data.GetLength(0); i++)
                for (int j = 0; j < Data.GetLength(1); j++)
                    for (int k = 0; k < Data.GetLength(2); k++)
                    {
                        x = (i - HalfI) / RX;
                        y = (j - HalfJ) / RY;
                        z = (k - HalfK) / RZ;
                        if ((x * x + y * y + z * z) > 1 || Data[i, j, k] < 0)
                        {
                            Data[i, j, k] = sum;
                        }
                    }

            return sum;
        }

        protected object[] mFilterToken;
        public object[] DefaultProperties
        {
            get
            {
                return new object[] {new string[] { "" }, 0,
                    new double[] { 0, 1, 2, 3 },
                    new double[] { 0, 1, 2, 3 },
                    "MovingAverage", 25,
                    "Trig", 0,
                    "Show",
                    new Size(170, 170),
                    Path.GetDirectoryName(Application.ExecutablePath) + "\\TEMP\\tempimage.bmp",false,0,false,""  };
            }
        }
        public string[] ParameterList
        {
            get
            {
                return new string[] 
            { 
                "Bitmap_Filenames|string[]","X_Positions|int[]","Y_Positions|int[]",
                "SmoothingTypeX|string","X_Smooth_Param|int","SmootingTypeY|string",
                "Y_Smooth_Param|int","ShowForm|string","CutSize|Size","OptionalOutputDir|string",
                "Threaded|bool","ThreadID|int","CreateMovie|bool","TempDataDir|string"
            };
            }
        }

        static object CriticalSectionLock = new object();

        public class CenterCellsToken : IEffectToken
        {
            public string TokenName()
            {
                return this.ToString();
            }
            /// <summary>
            /// holds the corrected data
            /// </summary>
            public double[] CXData;
            public double[] CYData;

            /// <summary>
            /// determines the cellsize by pulling the cellsize from each thread and then 
            /// combining them together
            /// </summary>
            public Size CellSize = new Size(170, 170);

            public System.Diagnostics.Stopwatch mStopWatch;
            public double FitQuality = 0;
            public bool MakeMovie = false;

            public string TempDataDir = "";
        }

        CenterCellsToken cct = null;


        double SnapStrength;
        string XArrayName, YArrayName;
        string XRemoveMethod;
        decimal XRemoveParam;
        string YRemoveMethod;
        decimal YRemoveParam;
        bool ShowForm;
        Size CellSize;
        int ThreadID;
        bool MakeMovie;
        string TempFolder;
        List<ImageFile> ImageList;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters"> 
        /// "X_Positions|int[]"
        /// "Y_Positions|int[]",
        /// "SmoothingTypeX|string"
        /// "X_Smooth_Param|int"
        /// "SmootingTypeY|string",
        /// "Y_Smooth_Param|int",
        /// "ShowForm|string",
        /// "CutSize|Size",
        /// "OptionalOutputDir|string",
        /// "ThreadID|int",
        /// "CreateMovie|bool",
        /// "TempDataDir|string"</param>
        /// <returns></returns>
        public object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
           ReplaceStringDictionary PassData, params object[] Parameters)
        {

            mFilterToken = Parameters;

            SnapStrength = (int)Parameters[0];
            XArrayName = (string)Parameters[1];
            YArrayName = (string)Parameters[2];
            XRemoveMethod = (string)Parameters[3];
            XRemoveParam = (int)Parameters[4];
            YRemoveMethod = (string)Parameters[5];
            YRemoveParam = (int)Parameters[6];
            ShowForm = (bool)Parameters[7];
            CellSize = (Size)Parameters[8];
            ThreadID = (int)Parameters[9];
            MakeMovie = (bool)Parameters[10];
            TempFolder = (string)Parameters[11];

            ImageList = (List<ImageFile>)Parameters[12];

            lock (CriticalSectionLock)
            {
                ///check if this is the first pass
                if (cct == null)
                {


                    if (dataEnvironment.EffectTokens.ContainsKey("CenterCellsTool") == true)
                        cct = (CenterCellsToken)dataEnvironment.EffectTokens["CenterCellsTool"];
                    else
                    {
                        cct = new CenterCellsToken();

                        cct.CellSize = new Size(170, 170);
                        cct.mStopWatch = new System.Diagnostics.Stopwatch();
                        cct.mStopWatch.Reset();
                        cct.mStopWatch.Start();

                        dataEnvironment.EffectTokens.Add("CenterCellsTool", cct);
                    }

                    //put all the variables to their correct values

                    Console.WriteLine("CenteringSTarted");
                }


                Size tCellSize = CellSize;
                if (tCellSize.Width > cct.CellSize.Width)
                    cct.CellSize.Width = tCellSize.Width;
                if (tCellSize.Height > cct.CellSize.Height)
                    cct.CellSize.Height = tCellSize.Height;
            }

            JoinThreadsTool.JoinThreads(dataEnvironment, "CenterCells0", ThreadID);

            mPassData = PassData;

            mDataEnvironment = dataEnvironment;

            //only one thread is alble to do all the work, so pull that thread and let all the others wait
            if (dataEnvironment.RunningThreaded == false || (ThreadID == 0))
            {
                try
                {
                    cct.MakeMovie = MakeMovie;
                    cct.TempDataDir = TempFolder;
                }
                catch
                {
                    cct.MakeMovie = false;
                }


                ImageFilenames = mDataEnvironment.WholeFileList.ToArray();

                ///either load in the array that is passed, or pull the correct global array
                X_Positions = ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GetGlobalArray(XArrayName, dataEnvironment).CopyOutArray;

                ///either load in the array that is passed, or pull the correct global array
                Y_Positions = ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GetGlobalArray(YArrayName, dataEnvironment).CopyOutArray;

                ///really dont think this works much anymore
                if (ShowForm)
                    this.Show();

                //determine the centering fit method
                // this has to be called last or it will screw up the token
                if (XRemoveMethod == "movingaverage")
                {
                    rbMovingAverageX.Checked = true;
                    nudPeriodX.Value = (decimal)XRemoveParam;
                }
                else if ((XRemoveMethod).ToLower() == "polynomial")
                {
                    rbPolynomialX.Checked = true;
                    nudOrderX.Value = (decimal)XRemoveParam;
                }
                else if ((XRemoveMethod).ToLower() == "trig")
                {
                    rbTrigX.Checked = true;
                }

                if ((YRemoveMethod).ToLower() == "movingaverage")
                {
                    rbMovingAverageY.Checked = true;
                    nudPeriodY.Value = (decimal)YRemoveParam;
                }
                else if ((YRemoveMethod).ToLower() == "polynomial")
                {
                    rbPolynomialY.Checked = true;
                    nudOrderY.Value = (decimal)YRemoveParam;
                }
                else if ((YRemoveMethod).ToLower() == "trig")
                {
                    rbTrigY.Checked = true;
                }

                trackBar1.Value = (int)SnapStrength;

                //do the actual fitting
                FitLines();

                if (ShowForm)
                {
                    timer1.Enabled = true;
                    while (this.Visible == true)
                        Application.DoEvents();
                }


                this.Hide();

                //check the quality of the fit against a sign and the actual data
                cct.FitQuality = CheckFit();


                Console.WriteLine("Done Centering MasterThread");
                //return SourceImage;
            }

            JoinThreadsTool.JoinThreads(dataEnvironment, "CenterCellsFinal", ThreadID);



            if (cct.MakeMovie)
            {
                /*  //make sure the file structure is correct
                  lock (CriticalSectionLock)
                  {
                      if (Directory.Exists(cct.TempDataDir + "\\CenterMovie\\") == false)
                          Directory.CreateDirectory(cct.TempDataDir + "\\CenterMovie\\");
                      else
                      {
                          string[] OldFrames = Directory.GetFiles(cct.TempDataDir + "\\CenterMovie\\");
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

                  //now send out the images
                  List<ImageFile> MyImages = ImageList;
                  for (int i = 0; i < MyImages.Count; i++)
                  {
                      if (MyImages[i].Index % 2 == 0)
                      {
                          Bitmap b = CenterImage(MyImages[i]);
                          FreeImageAPI.FreeImageBitmap fib = new FreeImageAPI.FreeImageBitmap(b);
                          fib.Save(cct.TempDataDir + "\\CenterMovie\\Frame" + string.Format("_{0:000}", MyImages[i].Index / 2) + ".jpg");
                      }
                  }*/
            }

            mPassData.AddSafe("CorrectedX", cct.CXData);
            mPassData.AddSafe("CorrectedY", cct.CYData);
            mPassData.AddSafe("CenterAccuracy", cct.FitQuality);

            return SourceImage;
        }

        public object DoEffect2(DataEnvironment dataEnvironment, object SourceImage,
           ReplaceStringDictionary PassData, params object[] Parameters)
        {

            mFilterToken = Parameters;

            SnapStrength = (int)Parameters[0];
            X_Positions = (double[])Parameters[1];
            Y_Positions = (double[])Parameters[2];
            XRemoveMethod = (string)Parameters[3];
            XRemoveParam = (int)Parameters[4];
            YRemoveMethod = (string)Parameters[5];
            YRemoveParam = (int)Parameters[6];
            ShowForm = (bool)Parameters[7];
            CellSize = (Size)Parameters[8];
            MakeMovie = (bool)Parameters[9];
            TempFolder = (string)Parameters[10];


            lock (CriticalSectionLock)
            {
                ///check if this is the first pass
                if (cct == null)
                {


                    if (dataEnvironment.EffectTokens.ContainsKey("CenterCellsTool") == true)
                        cct = (CenterCellsToken)dataEnvironment.EffectTokens["CenterCellsTool"];
                    else
                    {
                        cct = new CenterCellsToken();

                        cct.CellSize = new Size(170, 170);
                        cct.mStopWatch = new System.Diagnostics.Stopwatch();
                        cct.mStopWatch.Reset();
                        cct.mStopWatch.Start();

                        dataEnvironment.EffectTokens.Add("CenterCellsTool", cct);
                    }

                    //put all the variables to their correct values

                    Console.WriteLine("CenteringSTarted");
                }


                Size tCellSize = CellSize;
                if (tCellSize.Width > cct.CellSize.Width)
                    cct.CellSize.Width = tCellSize.Width;
                if (tCellSize.Height > cct.CellSize.Height)
                    cct.CellSize.Height = tCellSize.Height;
            }

            mPassData = PassData;

            mDataEnvironment = dataEnvironment;

            //only one thread is alble to do all the work, so pull that thread and let all the others wait
            if (dataEnvironment.RunningThreaded == false || (ThreadID == 0))
            {
                try
                {
                    cct.MakeMovie = MakeMovie;
                    cct.TempDataDir = TempFolder;
                }
                catch
                {
                    cct.MakeMovie = false;
                }


                ImageFilenames = mDataEnvironment.WholeFileList.ToArray();



                ///really dont think this works much anymore
                if (ShowForm)
                    this.Show();

                //determine the centering fit method
                // this has to be called last or it will screw up the token
                if (XRemoveMethod == "movingaverage")
                {
                    rbMovingAverageX.Checked = true;
                    nudPeriodX.Value = (decimal)XRemoveParam;
                }
                else if ((XRemoveMethod).ToLower() == "polynomial")
                {
                    rbPolynomialX.Checked = true;
                    nudOrderX.Value = (decimal)XRemoveParam;
                }
                else if ((XRemoveMethod).ToLower() == "trig")
                {
                    rbTrigX.Checked = true;
                }

                if ((YRemoveMethod).ToLower() == "movingaverage")
                {
                    rbMovingAverageY.Checked = true;
                    nudPeriodY.Value = (decimal)YRemoveParam;
                }
                else if ((YRemoveMethod).ToLower() == "polynomial")
                {
                    rbPolynomialY.Checked = true;
                    nudOrderY.Value = (decimal)YRemoveParam;
                }
                else if ((YRemoveMethod).ToLower() == "trig")
                {
                    rbTrigY.Checked = true;
                }

                trackBar1.Value = (int)SnapStrength;

                //do the actual fitting
                FitLines();

                if (ShowForm)
                {
                    timer1.Enabled = true;
                    while (this.Visible == true)
                        Application.DoEvents();
                }


                this.Hide();

                //check the quality of the fit against a sign and the actual data
                cct.FitQuality = CheckFit();


                Console.WriteLine("Done Centering MasterThread");
                //return SourceImage;
            }

            JoinThreadsTool.JoinThreads(dataEnvironment, "CenterCellsFinal", ThreadID);



            if (cct.MakeMovie)
            {
                /* //make sure the file structure is correct
                 lock (CriticalSectionLock)
                 {
                     if (Directory.Exists(cct.TempDataDir + "\\CenterMovie\\") == false)
                         Directory.CreateDirectory(cct.TempDataDir + "\\CenterMovie\\");
                     else
                     {
                         string[] OldFrames = Directory.GetFiles(cct.TempDataDir + "\\CenterMovie\\");
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

                 //now send out the images
                 //List<ImageFile> MyImages = ImageList;
                 List<string> Filenames = dataEnvironment.WholeFileList;
                 for (int i = 0; i < Filenames.Count; i++)
                 {
                     if (i % 5 == 0)
                     {
                         Bitmap b = CenterImage( new ImageFile(i, Filenames[i]));
                         b.Save(cct.TempDataDir + "\\CenterMovie\\Frame" + string.Format("_{0:000}", i / 2) + ".jpg");
                         //FreeImageAPI.FreeImageBitmap fib = new FreeImageAPI.FreeImageBitmap(b);
                         //fib.Save(cct.TempDataDir + "\\CenterMovie\\Frame" + string.Format("_{0:000}", i / 2) + ".jpg");
                     }
                 }*/
            }

            mPassData.AddSafe("CorrectedX", cct.CXData);
            mPassData.AddSafe("CorrectedY", cct.CYData);
            mPassData.AddSafe("CenterAccuracy", cct.FitQuality);

            return SourceImage;
        }
        /// <summary>
        /// put the files in the correct possition then clip them off to make a nice movie
        /// </summary>
        /// <param name="CurrentImage"></param>
        /// <returns></returns>
        private Bitmap CenterImage(ImageFile CurrentImage)
        {
            Bitmap b = mDataEnvironment.AllImages[CurrentImage.Index].ToBitmap();

            mScratchImage = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppRgb);

            Graphics g = Graphics.FromImage(mScratchImage);
            g.Clear(Color.White);
            int X = (int)(mScratchImage.Width / 2 - cct.CXData[CurrentImage.Index]);
            int Y = (int)(mScratchImage.Height / 2 - cct.CYData[CurrentImage.Index]);

            g.DrawImageUnscaled(b, new Point(X, Y));

            int MidX = mScratchImage.Width / 2;
            int MidY = mScratchImage.Height / 2;

            //draw a cross at the center
            g.DrawLine(Pens.Blue, new Point(MidX, MidY - 20), new Point(MidX, MidY + 20));
            g.DrawLine(Pens.Blue, new Point(MidX - 20, MidY), new Point(MidX + 20, MidY));

            Font f = new System.Drawing.Font(FontFamily.GenericMonospace, 14);

            g.DrawString(string.Format("{0:000} ", CurrentImage.Index), f, Brushes.Black, new PointF(0, 20));
            g.Dispose();

            Bitmap ClippedBitmap = new Bitmap(cct.CellSize.Width, cct.CellSize.Width);// mCellWanderArea.Height);
            g = Graphics.FromImage(ClippedBitmap);
            g.Clear(Color.White);
            g.DrawImageUnscaled(mScratchImage, new Point(-1 * (int)(mScratchImage.Width / 2d - cct.CellSize.Width / 2d), -1 * (int)(mScratchImage.Height / 2d - cct.CellSize.Width / 2d)));


            b = new Bitmap(ClippedBitmap.Width / 2, ClippedBitmap.Height / 2);
            g = Graphics.FromImage(b);
            g.DrawImage(ClippedBitmap, new Rectangle(0, 0, b.Width, b.Height), new Rectangle(0, 0, ClippedBitmap.Width, ClippedBitmap.Height), GraphicsUnit.Pixel);
            return b;
        }



        /// <summary>
        /// The bounds of where the cell wanders.  This is used to clip the images down later
        /// </summary>
        private static Rectangle mCellWanderArea = Rectangle.Empty;

        public class PreFitToken : IEffectToken
        {
            public string TokenName()
            {
                return this.ToString();
            }

        }

        private class Lane
        {
            public Rectangle LaneBounds;
            public double LaneFocus;
            public double Average;
            public List<CellF4> Widths;
            public bool inLane(Point p)
            {
                return LaneBounds.Contains(p);
            }
        }

        private class CellF4
        {
            public Rectangle cellBounds;
            public double F4;
            public Point CellCenter;
            public int ImageN;
        }

        private static int MySortFunction(CellF4 obj1, CellF4 obj2)
        {
            return obj1.cellBounds.Width.CompareTo(obj2.cellBounds.Width);
        }

        // the problem is that hthere can be multiple cells and debry in the field of view.  
        public static Rectangle FindBestCellCenter(DataEnvironment dataEnvironment, int[] Indexs)
        {
            //double[][] focuses = new double[Indexs.Length][];
            List<Blobs.BlobDescription[]> Condensed = new List<BlobDescription[]>();
            int Height = dataEnvironment.AllImages[0].Height;
            int Width = dataEnvironment.AllImages[0].Width;
            List<CellF4> Cells = new List<CellF4>();

            //get all the focus scores and then put all the cells in a list for easy processing
            int MaxLanes = 0;
            for (int ImageN = 0; ImageN < Indexs.Length; ImageN++)
            {
                if (mBlobs.Length > Indexs[ImageN] && mBlobs[Indexs[ImageN]] != null)
                {
                    Condensed.Add(mBlobs[Indexs[ImageN]]);
                    ImageHolder ih = dataEnvironment.AllImages[Indexs[ImageN]];
                    // double[]f4=new double[Condensed[ImageN].Length];
                    CellF4 cell = new CellF4();
                    int ClosestValue = int.MaxValue;
                    for (int i = 0; i < Condensed[ImageN].Length; i++)
                    {
                        Blobs.BlobDescription Single = Condensed[ImageN][i];
                        //  if (Single.BlobBounds.Width > 20 && Single.BlobBounds.Height > 20 &&
                        //      Single.BlobBounds.Width < 300 && Single.BlobBounds.Height < 300)

                        if (Math.Abs(Single.CenterOfGravity.X - ih.Width / 2d) < ClosestValue)
                        {
                            ClosestValue = (int)Math.Abs(Single.CenterOfGravity.X - ih.Width / 2d);
                            cell.cellBounds = Single.BlobBounds;
                            cell.CellCenter = Single.CenterOfGravity;
                            cell.F4 = MathHelpLib.ProjectionFilters.FocusValueTool.FocusValueF4(ih, Single.BlobBounds) / (Single.BlobBounds.Width * Single.BlobBounds.Height);
                            cell.ImageN = ImageN;

                        }
                    }
                    Cells.Add(cell);
                }
            }

            Lane[] Lanes = new Lane[1];
            Cells.Sort(new Comparison<CellF4>(MySortFunction));
            //put all the cells into lanes
            for (int i = 0; i < Cells.Count; i++)
            {
                CellF4 cell = Cells[i];
                bool found = false;
                for (int j = 0; j < Lanes.Length; j++)
                {
                    if (Lanes[j] != null)
                    {
                        Lanes[j].LaneFocus += cell.F4;
                        Lanes[j].Widths.Add(cell);
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    for (int j = 0; j < Lanes.Length; j++)
                    {
                        if (Lanes[j] == null)
                        {
                            Lanes[j] = new Lane();
                            double increase = 1.2;
                            Lanes[j].LaneBounds = new Rectangle((int)(cell.CellCenter.X - increase * cell.cellBounds.Width), 0, (int)(increase * 2 * cell.cellBounds.Width), Height);
                            Lanes[j].Widths = new List<CellF4>();
                            Lanes[j].Widths.Add(cell);
                            break;
                        }
                    }
                }
            }

            //remove lanes that touch the sides.
            for (int i = 0; i < Lanes.Length; i++)
            {
                if (Lanes[i] != null)
                {
                    if (Lanes[i].LaneBounds.X < 50)
                    {
                        Lanes[i] = null;
                    }
                    else if (Lanes[i].LaneBounds.Right > Width - 100)
                    {

                        Lanes[i] = null;
                    }
                }
            }


            //find that statistic for each lane
            double MaxF4 = 0, MinWidthVariation = double.MinValue, MaxNumber = 0;
            int MaxF4I = 0, MinWidthVariationI = 0, MaxNumberI = 0;
            for (int i = 0; i < Lanes.Length; i++)
            {
                if (Lanes[i] != null)
                {
                    if (Lanes[i].Widths.Count > MaxNumber)
                    {
                        MaxNumber = Lanes[i].Widths.Count;
                        MaxNumberI = i;
                    }
                    double f4 = Lanes[i].LaneFocus / Lanes[i].Widths.Count;
                    if (f4 > MaxF4)
                    {
                        MaxF4 = f4;
                        MaxF4I = i;
                    }

                    double AverageWidth = 0;
                    for (int j = 0; j < Lanes[i].Widths.Count; j++)
                    {
                        AverageWidth += Lanes[i].Widths[j].cellBounds.Width;
                    }
                    AverageWidth = AverageWidth / Lanes[i].Widths.Count;
                    double SD = 0;
                    for (int j = 0; j < Lanes[i].Widths.Count; j++)
                    {
                        SD += Math.Pow(AverageWidth - Lanes[i].Widths[j].cellBounds.Width, 2);
                    }
                    SD = SD / Lanes[i].Widths.Count;

                    if (SD < MinWidthVariation)
                    {
                        MinWidthVariation = SD;
                        MinWidthVariationI = i;
                    }

                    Lanes[i].Average = AverageWidth;
                }
            }
            //decide which lane is best 
            Lane BestLane = Lanes[0];

            if (BestLane == null)
            {
                int CloseValue = Width;
                CellF4 cell = Cells[0];
                for (int i = 0; i < Cells.Count; i++)
                {
                    if (Math.Abs(Cells[i].CellCenter.X - Width / 2d) < CloseValue)
                    {
                        CloseValue = (int)Math.Abs(Cells[i].CellCenter.X - Width / 2d);
                        cell = Cells[i];
                    }
                }



                BestLane = new Lane();
                double increase = 1.2;
                BestLane.LaneBounds = new Rectangle((int)(cell.CellCenter.X - increase * cell.cellBounds.Width), 0, (int)(increase * 2 * cell.cellBounds.Width), Height);
                BestLane.Widths = new List<CellF4>();
                BestLane.Widths.Add(cell);
                BestLane.Average = cell.cellBounds.Width;

            }

            //Blobs.BlobDescription Single2 = mBlobs[ImageNumber][MaxI];
            //  return new Rectangle((int)(Single2.CenterOfGravity.X - Single2.BlobBounds.Width * 4), 0, (int)(Single2.BlobBounds.Width * 2 * 4), Image.Height);
            double AveX = 0;
            for (int i = 0; i < BestLane.Widths.Count; i++)
                AveX += BestLane.Widths[i].CellCenter.X;

            AveX = AveX / BestLane.Widths.Count;

            if (BestLane.Average < 50) BestLane.Average = 100;
            double Increase = 3;
            return new Rectangle((int)(AveX - BestLane.Average * Increase), BestLane.LaneBounds.Top, (int)(BestLane.Average * 2 * Increase), BestLane.LaneBounds.Height);
        }


        // the problem is that hthere can be multiple cells and debry in the field of view.  
        public static Rectangle FindBestCell(DataEnvironment dataEnvironment, int[] Indexs)
        {
            //double[][] focuses = new double[Indexs.Length][];
            Blobs.BlobDescription[][] Condensed = new Blobs.BlobDescription[Indexs.Length][];
            int Height = dataEnvironment.AllImages[0].Height;
            int Width = dataEnvironment.AllImages[0].Width;
            List<CellF4> Cells = new List<CellF4>();

            //get all the focus scores and then put all the cells in a list for easy processing
            int MaxLanes = 0;
            for (int ImageN = 0; ImageN < Indexs.Length; ImageN++)
            {
                Condensed[ImageN] = mBlobs[Indexs[ImageN]];
                ImageHolder ih = dataEnvironment.AllImages[Indexs[ImageN]];
                // double[]f4=new double[Condensed[ImageN].Length];
                for (int i = 0; i < Condensed[ImageN].Length; i++)
                {
                    Blobs.BlobDescription Single = Condensed[ImageN][i];
                    if (Single.BlobBounds.Width > 20 && Single.BlobBounds.Height > 20 &&
                        Single.BlobBounds.Width < 300 && Single.BlobBounds.Height < 400)
                    {
                        CellF4 cell = new CellF4();
                        cell.cellBounds = Single.BlobBounds;
                        cell.CellCenter = Single.CenterOfGravity;
                        cell.F4 = MathHelpLib.ProjectionFilters.FocusValueTool.FocusValueF4(ih, Single.BlobBounds) / (Single.BlobBounds.Width * Single.BlobBounds.Height);
                        cell.ImageN = ImageN;
                        Cells.Add(cell);
                    }
                }
                // focuses[ImageN] = f4;

                if (Condensed[ImageN].Length > MaxLanes) MaxLanes = Condensed[ImageN].Length;
            }

            Lane[] Lanes = new Lane[MaxLanes];
            Cells.Sort(new Comparison<CellF4>(MySortFunction));
            //put all the cells into lanes
            for (int i = 0; i < Cells.Count; i++)
            {
                CellF4 cell = Cells[i];
                bool found = false;
                for (int j = 0; j < Lanes.Length; j++)
                {
                    if (Lanes[j] != null && Lanes[j].inLane(cell.CellCenter))
                    {
                        Lanes[j].LaneFocus += cell.F4;
                        Lanes[j].Widths.Add(cell);
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    for (int j = 0; j < Lanes.Length; j++)
                    {
                        if (Lanes[j] == null)
                        {
                            Lanes[j] = new Lane();
                            double increase = 1.2;
                            Lanes[j].LaneBounds = new Rectangle((int)(cell.CellCenter.X - increase * cell.cellBounds.Width), 0, (int)(increase * 2 * cell.cellBounds.Width), Height);
                            Lanes[j].Widths = new List<CellF4>();
                            Lanes[j].Widths.Add(cell);
                            break;
                        }
                    }
                }
            }

            //remove lanes that touch the sides.
            for (int i = 0; i < Lanes.Length; i++)
            {
                if (Lanes[i] != null)
                {
                    if (Lanes[i].LaneBounds.X < 50)
                    {
                        Lanes[i] = null;
                    }
                    else if (Lanes[i].LaneBounds.Right > Width - 100)
                    {

                        Lanes[i] = null;
                    }
                }
            }


            //find that statistic for each lane
            double MaxF4 = 0, MinWidthVariation = double.MinValue, MaxNumber = 0;
            int MaxF4I = 0, MinWidthVariationI = 0, MaxNumberI = 0;
            for (int i = 0; i < Lanes.Length; i++)
            {
                if (Lanes[i] != null)
                {
                    if (Lanes[i].Widths.Count > MaxNumber)
                    {
                        MaxNumber = Lanes[i].Widths.Count;
                        MaxNumberI = i;
                    }
                    double f4 = Lanes[i].LaneFocus / Lanes[i].Widths.Count;
                    if (f4 > MaxF4)
                    {
                        MaxF4 = f4;
                        MaxF4I = i;
                    }

                    double AverageWidth = 0;
                    for (int j = 0; j < Lanes[i].Widths.Count; j++)
                    {
                        AverageWidth += Lanes[i].Widths[j].cellBounds.Width;
                    }
                    AverageWidth = AverageWidth / Lanes[i].Widths.Count;
                    double SD = 0;
                    for (int j = 0; j < Lanes[i].Widths.Count; j++)
                    {
                        SD += Math.Pow(AverageWidth - Lanes[i].Widths[j].cellBounds.Width, 2);
                    }
                    SD = SD / Lanes[i].Widths.Count;

                    if (SD < MinWidthVariation)
                    {
                        MinWidthVariation = SD;
                        MinWidthVariationI = i;
                    }

                    Lanes[i].Average = AverageWidth;
                }
            }
            //decide which lane is best 
            Lane BestLane = null;
            try
            {
                if (MaxNumberI == MaxF4I && MinWidthVariationI == MaxNumberI)
                    BestLane = Lanes[MaxNumberI];
                else if (MaxF4I == MinWidthVariationI)
                    BestLane = Lanes[MaxF4I];
                else if (MaxNumberI == MinWidthVariationI)
                    BestLane = Lanes[MaxNumberI];
                else
                    BestLane = Lanes[MaxF4I];
            }
            catch { }

            if (BestLane == null)
            {
                int CloseValue = Width;
                CellF4 cell = Cells[0];
                for (int i = 0; i < Cells.Count; i++)
                {
                    if (Math.Abs(Cells[i].CellCenter.X - Width / 2d) < CloseValue)
                    {
                        CloseValue = (int)Math.Abs(Cells[i].CellCenter.X - Width / 2d);
                        cell = Cells[i];
                    }
                }

                BestLane = new Lane();
                double increase = 1.2;
                BestLane.LaneBounds = new Rectangle((int)(cell.CellCenter.X - increase * cell.cellBounds.Width), 0, (int)(increase * 2 * cell.cellBounds.Width), Height);
                BestLane.Widths = new List<CellF4>();
                BestLane.Widths.Add(cell);
                BestLane.Average = cell.cellBounds.Width;
            }

            //Blobs.BlobDescription Single2 = mBlobs[ImageNumber][MaxI];
            //  return new Rectangle((int)(Single2.CenterOfGravity.X - Single2.BlobBounds.Width * 4), 0, (int)(Single2.BlobBounds.Width * 2 * 4), Image.Height);
            double AveX = 0;
            for (int i = 0; i < BestLane.Widths.Count; i++)
                AveX += BestLane.Widths[i].CellCenter.X;

            AveX = AveX / BestLane.Widths.Count;

            double Increase = 4;
            return new Rectangle((int)(AveX - BestLane.Average * Increase), BestLane.LaneBounds.Top, (int)(BestLane.Average * 2 * Increase), BestLane.LaneBounds.Height);
        }


        public static Rectangle FindBestCell(int ImageNumber, ImageHolder Image)
        {
            if (mBlobs[ImageNumber].Length == 1)
            {
                Blobs.BlobDescription Single = mBlobs[ImageNumber][0];
                return new Rectangle((int)(Single.CenterOfGravity.X - Single.BlobBounds.Width * 4), 0, (int)(Single.BlobBounds.Width * 2 * 4), Image.Height);
            }

            double MaxF4 = double.MinValue;
            int MaxI = 0;
            for (int i = 0; i < mBlobs[ImageNumber].Length; i++)
            {
                Blobs.BlobDescription Single = mBlobs[ImageNumber][i];
                if (Single.BlobBounds.Width > 40 && Single.BlobBounds.Height > 40 &&
                    Single.BlobBounds.Width < 300 && Single.BlobBounds.Height < 400)
                {
                    double j = MathHelpLib.ProjectionFilters.FocusValueTool.FocusValueF4(Image, Single.BlobBounds);
                    if (j > MaxF4)
                    {
                        MaxF4 = j;
                        MaxI = i;
                    }
                }
            }

            Blobs.BlobDescription Single2 = mBlobs[ImageNumber][MaxI];
            return new Rectangle((int)(Single2.CenterOfGravity.X - Single2.BlobBounds.Width * 4), 0, (int)(Single2.BlobBounds.Width * 2 * 4), Image.Height);

        }

        private static object StoreBlobsLockObject = new object();
        private static Blobs.BlobDescription[][] mBlobs = null;
        public static void StoreBlobLocation(DataEnvironment dataEnviroment, int ImageNumber, List<Blobs.BlobDescription> AllFoundBlobs, double Expander)
        {
            lock (StoreBlobsLockObject)
            {
                if (mBlobs == null)
                    mBlobs = new Filters.Blobs.BlobDescription[dataEnviroment.AllImages.Count][];
            }
            Blobs.BlobDescription[] Blobs = new Filters.Blobs.BlobDescription[AllFoundBlobs.Count];
            for (int i = 0; i < Blobs.Length; i++)
            {
                Blobs.BlobDescription bd = new Filters.Blobs.BlobDescription(AllFoundBlobs[i].BlobIndex, AllFoundBlobs[i].BlobBounds, new Point((int)(AllFoundBlobs[i].CenterOfGravity.X * Expander), (int)(AllFoundBlobs[i].CenterOfGravity.Y * Expander)));
                Blobs[i] = bd;
            }

            mBlobs[ImageNumber] = Blobs;
        }

        private static Rectangle sCellWanderArea;
        public static void StoreBlobLocation(DataEnvironment dataEnviroment, int ImageNumber, Blobs.BlobDescription[] AllFoundBlobs, double Expander, Rectangle CellWanderArea)
        {
            sCellWanderArea = CellWanderArea;
            lock (StoreBlobsLockObject)
            {
                if (mBlobs == null)
                    mBlobs = new Filters.Blobs.BlobDescription[dataEnviroment.AllImages.Count][];
            }

            Blobs.BlobDescription[] Blobs = new Filters.Blobs.BlobDescription[AllFoundBlobs.Length];
            for (int i = 0; i < Blobs.Length; i++)
            {
                Blobs.BlobDescription bd = new Filters.Blobs.BlobDescription(AllFoundBlobs[i].BlobIndex, AllFoundBlobs[i].BlobBounds, new Point((int)(AllFoundBlobs[i].CenterOfGravity.X * Expander),
                    (int)(AllFoundBlobs[i].CenterOfGravity.Y * Expander)));
                Blobs[i] = bd;
            }

            mBlobs[ImageNumber] = Blobs;
        }

        public static void StoreBlobLocation(DataEnvironment dataEnviroment, int ImageNumber, Blobs.BlobDescription[] AllFoundBlobs, double Expander)
        {
            lock (StoreBlobsLockObject)
            {
                if (mBlobs == null)
                    mBlobs = new Filters.Blobs.BlobDescription[dataEnviroment.AllImages.Count][];
            }

            Blobs.BlobDescription[] Blobs = new Filters.Blobs.BlobDescription[AllFoundBlobs.Length];
            for (int i = 0; i < Blobs.Length; i++)
            {
                Blobs.BlobDescription bd = new Filters.Blobs.BlobDescription(AllFoundBlobs[i].BlobIndex, AllFoundBlobs[i].BlobBounds, new Point((int)(AllFoundBlobs[i].CenterOfGravity.X * Expander), (int)(AllFoundBlobs[i].CenterOfGravity.Y * Expander)));
                Blobs[i] = bd;
            }

            mBlobs[ImageNumber] = Blobs;
        }
        private class BlobLocation
        {
            public int ImageN;
            public double X;
            public double Y;
            public double Area;

            public BlobLocation(Blobs.BlobDescription blob, int imageNumber)
            {
                X = blob.CenterOfGravity.X;
                Y = blob.CenterOfGravity.Y;
                Area = blob.BlobBounds.Width * blob.BlobBounds.Height;
                ImageN = imageNumber;
            }
        }

        private static object FitLinesCriticalSection = new object();
        public static void PreFitLines(DataEnvironment dataEnvironment, string XArrayName, string YArrayName, int ThreadID)
        {

            JoinThreadsTool.JoinThreads(dataEnvironment, "PreFitLines", ThreadID);

            lock (FitLinesCriticalSection)
            {
                if (dataEnvironment.EffectTokens.ContainsKey("PreFitLines") == false)
                {

                    //this is a workaround to use the same procedure for the python version and the c# version.  
                    //The original version is in the comments below

                    ///either load in the array that is passed, or pull the correct global array
                    double[] X_Positions = ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GetGlobalArray(XArrayName, dataEnvironment).CopyOutArray;

                    ///either load in the array that is passed, or pull the correct global array
                    double[] Y_Positions = ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GetGlobalArray(YArrayName, dataEnvironment).CopyOutArray;

                    Dictionary<string, double[]> Results = PreFitLines(dataEnvironment, X_Positions, Y_Positions);

                    ///either load in the array that is passed, or pull the correct global array
                    ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GetGlobalArray(XArrayName, dataEnvironment).CopyInArray = Results["UpdatedX"];

                    ///either load in the array that is passed, or pull the correct global array
                    ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GetGlobalArray(YArrayName, dataEnvironment).CopyInArray = Results["UpdatedY"];


                }
            }
        }

        public static void SaveBlobs(DataEnvironment dataEnvironment)
        {
            if (mBlobs != null && mBlobs.Length > 0)
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(dataEnvironment.DataOutFolder + "Blobs.txt");
                for (int i = 0; i < mBlobs.Length; i++)
                {
                    for (int j = 0; j < mBlobs[i].Length; j++)
                    {
                        Point p = new Point(mBlobs[i][j].CenterOfGravity.X + sCellWanderArea.X, mBlobs[i][j].CenterOfGravity.Y + sCellWanderArea.Y);
                        Rectangle r = new Rectangle(mBlobs[i][j].BlobBounds.X + sCellWanderArea.X, mBlobs[i][j].BlobBounds.Y + sCellWanderArea.Y, mBlobs[i][j].BlobBounds.Width, mBlobs[i][j].BlobBounds.Height);
                        file.WriteLine(i.ToString() + "| " + j.ToString() + "| " + p.ToString() + "| " + r.ToString());
                    }
                }
                file.Close();
            }
        }

        public static bool OpenBlobs(DataEnvironment dataEnvironment, out double[] X_Positions, out double[] Y_Positions, out double[] CellSizes)
        {
            X_Positions = new double[dataEnvironment.AllImages.Count];
            Y_Positions = new double[dataEnvironment.AllImages.Count];
            CellSizes = new double[dataEnvironment.AllImages.Count];
            return false;
            if (File.Exists(dataEnvironment.DataOutFolder + "Blobs.txt"))
            {
                try
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(dataEnvironment.DataOutFolder + "Blobs.txt");
                    string FileS = file.ReadToEnd();
                    file.Close();

                    string[] Lines = FileS.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    mBlobs = new Blobs.BlobDescription[dataEnvironment.AllImages.Count][];
                    X_Positions = new double[dataEnvironment.AllImages.Count];
                    Y_Positions = new double[dataEnvironment.AllImages.Count];
                    CellSizes = new double[dataEnvironment.AllImages.Count];
                    List<Blobs.BlobDescription>[] BlobList = new List<Blobs.BlobDescription>[dataEnvironment.AllImages.Count];

                    for (int i = 0; i < Lines.Length; i++)
                    {
                        int ImageN, BlobN;
                        Rectangle blobBounds;
                        Point com;

                        GetBlobInfo(Lines[i], out ImageN, out BlobN, out blobBounds, out com);

                        if (BlobList[ImageN] == null)
                            BlobList[ImageN] = new List<Blobs.BlobDescription>();

                        BlobList[ImageN].Add(new Blobs.BlobDescription(ImageN, blobBounds, com));
                    }

                    for (int i = 0; i < mBlobs.Length; i++)
                    {
                        if (BlobList[i] != null)
                        {
                            mBlobs[i] = BlobList[i].ToArray();

                            var MaxBlob = ImageViewer.Filters.Blobs.GetBiggestCenterBlob.SortBlobs(mBlobs[i], dataEnvironment.AllImages[0]);

                            // CenterCellsTool2Form.StoreBlobLocation(dataEnvironment, i, (BlobDescription[])mBlobs[i], 1, CellWanderArea);

                            X_Positions[i] = MaxBlob.CenterOfGravity.X;
                            Y_Positions[i] = MaxBlob.CenterOfGravity.Y;
                            CellSizes[i] = (MaxBlob.BlobBounds.Width + MaxBlob.BlobBounds.Height) / 2;
                        }
                    }
                    return true;
                }
                catch
                {
                    X_Positions = new double[dataEnvironment.AllImages.Count];
                    Y_Positions = new double[dataEnvironment.AllImages.Count];
                    CellSizes = new double[dataEnvironment.AllImages.Count];
                    return false;
                }
            }
            else
            {
                X_Positions = new double[dataEnvironment.AllImages.Count];
                Y_Positions = new double[dataEnvironment.AllImages.Count];
                CellSizes = new double[dataEnvironment.AllImages.Count];
                return false;
            }

        }

        public static void SaveCenters(DataEnvironment dataEnvironment, double[] X_Positions, double[] Y_Positions)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(dataEnvironment.DataOutFolder + "Centers.csv", false);
            for (int i = 0; i < X_Positions.Length; i++)
            {
                file.WriteLine(i.ToString() + ", " + X_Positions[i].ToString() + ", " + Y_Positions[i].ToString());
            }
            file.Close();
        }


        public static bool OpenCenters(DataEnvironment dataEnvironment, out double[] X_Positions, out double[] Y_Positions, out int CellSize)
        {
            X_Positions = new double[500];
            Y_Positions = new double[500];
            CellSize = 250;
            if (File.Exists(dataEnvironment.DataOutFolder + "Centers.csv") == false)
                return false;

            System.IO.StreamReader file = new System.IO.StreamReader(dataEnvironment.DataOutFolder + "Centers.csv");
            string FileAll = file.ReadToEnd();
            string[] Lines = FileAll.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int lastIndex = -1;
            for (int i = 0; i < Lines.Length; i++)
            {
                string[] parts = Lines[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                int index = int.Parse(parts[0]);
                if (lastIndex == -1)
                    lastIndex = index;
                X_Positions[index] = double.Parse(parts[1]);
                Y_Positions[index] = double.Parse(parts[2]);
            }

            //deal with missing values
            if (X_Positions[0] == 0 && Y_Positions[0] == 0)
            {
                X_Positions[0] = X_Positions[lastIndex];
                Y_Positions[0] = Y_Positions[lastIndex];
            }
            for (int i = 1; i < Lines.Length; i++)
            {
                if (X_Positions[i] == 0 && Y_Positions[i] == 0)
                {
                    X_Positions[i] = X_Positions[lastIndex];
                    Y_Positions[i] = Y_Positions[lastIndex];
                }
                else
                    lastIndex = i;
            }


            //find the cellsize
            if (File.Exists(dataEnvironment.DataOutFolder + "blobs.txt"))
            {
                double tCellSize = 0;
                double cc = 0;
                file = new System.IO.StreamReader(dataEnvironment.DataOutFolder + "blobs.txt");
                FileAll = file.ReadToEnd();
                Lines = FileAll.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                // 0| 0| {X=104,Y=602}| {X=0,Y=0,Width=224,Height=449}
                for (int i = 0; i < Lines.Length; i++)
                {
                    string[] Parts = Lines[i].Split(new string[] { "|", "{", "}", "=", ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                    double W = double.Parse(Parts[11]);
                    double H = double.Parse(Parts[13]);

                    if (W > 40 && W < 300 && H > 40 && H < 300)
                    {
                        tCellSize += (W + H);
                        cc += 2;
                    }
                }
                CellSize = (int)(tCellSize / cc * 2.2);
            }

            file.Close();
            return true;
        }

        private static void GetBlobInfo(string Line, out int ImageN, out int blobN, out Rectangle blobBounds, out Point centerOfMass)
        {
            string[] Parts = Line.Split(new string[] { "|", "{", "}", "=", ",", " " }, StringSplitOptions.RemoveEmptyEntries);

            int X = int.Parse(Parts[7]);
            int Y = int.Parse(Parts[9]);
            int W = int.Parse(Parts[11]);
            int H = int.Parse(Parts[13]);

            int cX = int.Parse(Parts[3]);
            int cY = int.Parse(Parts[5]);

            centerOfMass = new Point(cX, cY);
            blobBounds = new Rectangle(X, Y, W, H);

            ImageN = int.Parse(Parts[0]);
            blobN = int.Parse(Parts[1]);
        }

        public static Dictionary<string, double[]> PreFitLines2(DataEnvironment dataEnvironment, double[] X_Positions, double[] Y_Positions, out double CellSize)
        {
            PreFitToken jtt = new PreFitToken();
            dataEnvironment.EffectTokens.AddSafe("PreFitLines", jtt);

            double[,] XData = null;
            double[,] YData = null;
            double Percent = 0;

            double WholeP = dataEnvironment.AllImages[0].Width;

            if (mBlobs != null)
            {
                try
                {
                    List<Blobs.BlobDescription> BestBlobs = new List<Blobs.BlobDescription>();
                    //chage the variable from blobdescription to bloblocation
                    double Average = 0;
                    double cc2 = 0;
                    double Width = 0;
                    for (int i = 0; i < mBlobs.Length; i++)
                    {
                        if (mBlobs[i] != null)
                        {
                            for (int j = 0; j < mBlobs[i].Length; j++)
                            {
                                Average += mBlobs[i][j].CenterOfGravity.X;
                                Width += mBlobs[i][j].BlobBounds.Width;
                                cc2++;
                            }
                        }
                    }
                    Average = Average / cc2;
                    //the cell area was previously centered with the "right cell"
                    Average = sCellWanderArea.Width / 2d;

                    Width = Width / cc2 / 2;
                    if (mBlobs[0] != null)
                    {
                        Blobs.BlobDescription LastBlob = mBlobs[0][0];
                        for (int i = 0; i < mBlobs.Length; i++)
                        {
                            if (mBlobs[i] != null)
                            {
                                if (mBlobs[i].Length == 1)
                                {
                                    mBlobs[i][0].BlobIndex = i;
                                    BestBlobs.Add(mBlobs[i][0]);
                                    LastBlob = mBlobs[i][0];
                                }
                                else
                                {
                                    double d;
                                    double Distance = double.MaxValue;
                                    int BestDistance = 0;
                                    for (int j = 0; j < mBlobs[i].Length; j++)
                                    {
                                        if (i == 0)
                                            d = Math.Abs(mBlobs[i][j].CenterOfGravity.X - Average);
                                        else
                                            d = Math.Abs(mBlobs[i][j].BlobBounds.Width - LastBlob.BlobBounds.Width);
                                        if (d < Distance)
                                            if (mBlobs[i][j].BlobBounds.Width > 25)
                                            {
                                                BestDistance = j;
                                                Distance = d;
                                            }
                                    }
                                    mBlobs[i][BestDistance].BlobIndex = i;
                                    BestBlobs.Add(mBlobs[i][BestDistance]);
                                    LastBlob = mBlobs[i][BestDistance];
                                }
                            }
                        }
                    }
                    int cc = 0;
                    XData = new double[2, BestBlobs.Count + 20];
                    YData = new double[2, BestBlobs.Count + 20];
                    int endImage = dataEnvironment.AllImages.Count;

                    double tCellSize = 0;
                    int ccCell = 0;

                    for (int i = BestBlobs.Count - 10; i < BestBlobs.Count; i++)
                    {
                        XData[0, cc] = BestBlobs[i].BlobIndex - endImage;
                        XData[1, cc] = BestBlobs[i].CenterOfGravity.X;


                        YData[0, cc] = BestBlobs[i].BlobIndex - endImage;
                        YData[1, cc] = BestBlobs[i].CenterOfGravity.Y;
                        cc++;
                    }
                    for (int i = 0; i < BestBlobs.Count; i++)
                    {
                        XData[0, cc] = BestBlobs[i].BlobIndex;
                        XData[1, cc] = BestBlobs[i].CenterOfGravity.X;


                        YData[0, cc] = BestBlobs[i].BlobIndex;
                        YData[1, cc] = BestBlobs[i].CenterOfGravity.Y;

                        tCellSize += BestBlobs[i].BlobBounds.Width + BestBlobs[i].BlobBounds.Height;
                        ccCell++;
                        cc++;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        XData[0, cc] = BestBlobs[i].BlobIndex + endImage;
                        XData[1, cc] = BestBlobs[i].CenterOfGravity.X;


                        YData[0, cc] = BestBlobs[i].BlobIndex + endImage;
                        YData[1, cc] = BestBlobs[i].CenterOfGravity.Y;
                        cc++;
                    }

                    CellSize = (tCellSize / ccCell);

                    double[,] FitLineX = null;
                    double[,] FitLineY = null;

                    FitLineX = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFitIndex(XData, 7, dataEnvironment.AllImages.Count);
                    FitLineY = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFitIndex(YData, 7, dataEnvironment.AllImages.Count);

                    //FitLineY = MathHelpLib.MathHelps.TrigFitIndex(YData, dataEnvironment.AllImages.Count);


                    double[] LineX = new double[dataEnvironment.AllImages.Count];
                    double[] LineY = new double[dataEnvironment.AllImages.Count];

                    for (int i = 0; i < dataEnvironment.AllImages.Count; i++)
                    {
                        LineX[i] = FitLineX[1, i] + sCellWanderArea.Left;
                        LineY[i] = FitLineY[1, i] + sCellWanderArea.Top;
                    }

                    Dictionary<string, double[]> OutArray = new Dictionary<string, double[]>();

                    OutArray.Add("UpdatedX", LineX);
                    OutArray.Add("UpdatedY", LineY);

                    return OutArray;
                }
                catch
                {
                    Dictionary<string, double[]> OutArray = new Dictionary<string, double[]>();
                    OutArray.Add("UpdatedX", X_Positions);
                    OutArray.Add("UpdatedY", Y_Positions);
                    CellSize = 300;
                    return OutArray;

                }
            }
            else
            {
                Dictionary<string, double[]> OutArray = new Dictionary<string, double[]>();
                OutArray.Add("UpdatedX", X_Positions);
                OutArray.Add("UpdatedY", Y_Positions);
                CellSize = 170;
                return OutArray;
            }
        }



        public static Dictionary<string, double[]> PreFitLines(DataEnvironment dataEnvironment, double[] X_Positions, double[] Y_Positions)
        {
            PreFitToken jtt = new PreFitToken();
            dataEnvironment.EffectTokens.AddSafe("PreFitLines", jtt);

            double[,] XData = null;
            double[,] YData = null;
            double Percent = 0;

            double WholeP = dataEnvironment.AllImages[0].Width;

            if (mBlobs != null)
            {
                //chage the variable from blobdescription to bloblocation

                //organize all the cells that are found in the first frame into the strings
                List<List<BlobLocation>> Strings = new List<List<BlobLocation>>();
                foreach (Blobs.BlobDescription bd in mBlobs[0])
                {
                    if (bd.BlobBounds.Left > 100 && bd.BlobBounds.Right < WholeP - 100)
                    {
                        if (bd.BlobBounds.Width > 30 && bd.BlobBounds.Height > 30)
                        {
                            List<BlobLocation> individualString = new List<BlobLocation>();
                            individualString.Add(new BlobLocation(bd, 0));
                            Strings.Add(individualString);
                        }
                    }
                }


                //now search through all the found blob locations, placing those cells that are close
                //to each other into the appropriate string.
                //
                //if no appropriate string is found, then start a new string.
                bool NoneFound;
                for (int imageN = 1; imageN < mBlobs.Length; imageN++)
                {
                    foreach (Blobs.BlobDescription bd in mBlobs[imageN])
                    {
                        if (bd.BlobBounds.Left > 100 && bd.BlobBounds.Right < WholeP - 100)
                        {
                            if (bd.BlobBounds.Width > 30 && bd.BlobBounds.Height > 30)
                            {
                                NoneFound = true;
                                for (int j = 0; j < Strings.Count; j++)
                                {
                                    BlobLocation LastPoint = Strings[j][Strings[j].Count - 1];
                                    if ((Math.Abs(LastPoint.X - bd.CenterOfGravity.X) + Math.Abs(LastPoint.Y - bd.CenterOfGravity.Y) + Math.Abs(LastPoint.ImageN - imageN)) < 40)
                                    {
                                        List<BlobLocation> individualString = Strings[j];
                                        individualString.Add(new BlobLocation(bd, imageN));
                                        Strings.RemoveAt(j);
                                        Strings.Add(individualString);
                                        NoneFound = false;
                                    }
                                }

                                if (NoneFound)
                                {
                                    List<BlobLocation> individualString = new List<BlobLocation>();
                                    individualString.Add(new BlobLocation(bd, imageN));
                                    Strings.Add(individualString);
                                }
                            }
                        }
                    }
                }

                List<BlobLocation> SelectedString = null;
                int MaxC = 0;
                int MaxI = 0;
                int MaxSize = 0;
                double HalfP = dataEnvironment.AllImages[0].Width / 2;
                double x;
                double Radius;
                for (int i = 0; i < Strings.Count; i++)
                {
                    if (Strings[i].Count == dataEnvironment.AllImages.Count)
                    {
                        double tMaxSize = 0;
                        for (int j = 0; j < Strings[i].Count; j++)
                        {
                            x = (Strings[i][j].X - HalfP);
                            Radius = Math.Exp(-1 * x * x / HalfP / 2);

                            tMaxSize += Strings[i][j].Area * Radius;
                        }
                        if (tMaxSize > MaxSize)
                        {
                            MaxSize = (int)tMaxSize;
                            SelectedString = Strings[i];
                        }
                    }
                    if (MaxC < Strings[i].Count)
                    {
                        MaxC = Strings[i].Count;
                        MaxI = i;
                    }
                }

                try
                {
                    if (SelectedString == null)
                    {
                        SelectedString = Strings[MaxI];
                    }

                    Percent = (double)SelectedString.Count / dataEnvironment.AllImages.Count - .01;
                    XData = new double[2, SelectedString.Count + 20];
                    YData = new double[2, SelectedString.Count + 20];
                    int cc = 0;
                    int endImage = SelectedString[SelectedString.Count - 1].ImageN;
                    for (int i = SelectedString.Count - 10; i < SelectedString.Count; i++)
                    {
                        XData[0, cc] = SelectedString[i].ImageN - endImage;
                        XData[1, cc] = SelectedString[i].X;


                        YData[0, cc] = SelectedString[i].ImageN - endImage;
                        YData[1, cc] = SelectedString[i].Y;
                        cc++;
                    }
                    for (int i = 0; i < SelectedString.Count; i++)
                    {
                        XData[0, cc] = SelectedString[i].ImageN;
                        XData[1, cc] = SelectedString[i].X;


                        YData[0, cc] = SelectedString[i].ImageN;
                        YData[1, cc] = SelectedString[i].Y;
                        cc++;
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        XData[0, cc] = SelectedString[i].ImageN + endImage;
                        XData[1, cc] = SelectedString[i].X;


                        YData[0, cc] = SelectedString[i].ImageN + endImage;
                        YData[1, cc] = SelectedString[i].Y;
                        cc++;
                    }
                    int j = 0;
                    j++;
                }
                catch { }
            }

            if (XData == null)
            {

                ///either load in the array that is passed, or pull the correct global array

                int GoodPoints = 0, BadPoints = 0;
                for (int i = 0; i < X_Positions.Length; i++)
                {
                    if (X_Positions[i] < -3000)
                        BadPoints++;
                    else
                        GoodPoints++;
                }
                Percent = (double)GoodPoints / (double)(GoodPoints + BadPoints);

                dataEnvironment.ProgressLog.AddSafe("FindQuality", Percent.ToString());
                ///put the data in the correct format for the proceedures
                XData = new double[2, GoodPoints + 20];
                YData = new double[2, GoodPoints + 20];

                int cc = 0;
                for (int i = X_Positions.Length - 10; i < X_Positions.Length; i++)
                {
                    if (X_Positions[i] > -3000)
                    {
                        XData[0, cc] = i - X_Positions.Length;
                        YData[0, cc] = i - X_Positions.Length;
                        XData[1, cc] = X_Positions[i];
                        YData[1, cc] = Y_Positions[i];
                        cc++;
                    }
                }

                for (int i = 0; i < X_Positions.Length; i++)
                {
                    if (X_Positions[i] > -3000)
                    {
                        XData[0, cc] = i;
                        YData[0, cc] = i;
                        XData[1, cc] = X_Positions[i];
                        YData[1, cc] = Y_Positions[i];
                        cc++;
                    }
                }

                for (int i = 0; i < 10; i++)
                {
                    if (X_Positions[i] > -3000)
                    {
                        XData[0, cc] = X_Positions.Length + i - 1;
                        YData[0, cc] = X_Positions.Length + i - 1;
                        XData[1, cc] = X_Positions[i];
                        YData[1, cc] = Y_Positions[i];
                        cc++;
                    }
                }
                int j = 0;
                j++;
            }

            double[,] FitLineX = null;
            double[,] FitLineY = null;
            //do the fitting
            if (Percent == 1)
            {
                // FitLineX = MathHelpLib.MathHelps.FitMovingAverage(XData, 5);
                // FitLineY = MathHelpLib.MathHelps.FitMovingAverage(YData, 5);
                FitLineX = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFitIndex(XData, 7, dataEnvironment.AllImages.Count);
                FitLineY = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFitIndex(YData, 7, dataEnvironment.AllImages.Count);
            }
            else if (Percent > .85)
            {
                // FitLineX = MathHelpLib.MathHelps.TrigFitIndex(XData, dataEnvironment.AllImages.Count);
                FitLineX = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFitIndex(XData, 7, dataEnvironment.AllImages.Count);
                FitLineY = MathHelpLib.CurveFitting.MathCurveFits.TrigFitIndex(YData, dataEnvironment.AllImages.Count);
            }
            else
            {
                FitLineX = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFitIndex(XData, 7, dataEnvironment.AllImages.Count);
                FitLineY = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFitIndex(YData, 7, dataEnvironment.AllImages.Count);

            }

            double[] LineX = new double[dataEnvironment.AllImages.Count];
            double[] LineY = new double[dataEnvironment.AllImages.Count];

            for (int i = 0; i < dataEnvironment.AllImages.Count; i++)
            {
                LineX[i] = FitLineX[1, i];
                LineY[i] = FitLineY[1, i];
            }

            Dictionary<string, double[]> OutArray = new Dictionary<string, double[]>();

            OutArray.Add("UpdatedX", LineX);
            OutArray.Add("UpdatedY", LineY);

            return OutArray;

        }
        /// <summary>
        /// Take the center locations and then smooth the data out a little to help mis identification
        /// </summary>
        private void FitLines()
        {
            ///put the data in the correct format for the proceedures
            double[,] XData = new double[2, X_Positions.Length];
            double[,] YData = new double[2, Y_Positions.Length];
            for (int i = 0; i < X_Positions.Length; i++)
            {
                XData[0, i] = i;
                YData[0, i] = i;
                XData[1, i] = X_Positions[i];
                YData[1, i] = Y_Positions[i];
            }

            double[,] FitLineX = null;
            double[,] FitLineY = null;
            //do the fitting
            if (rbMovingAverageX.Checked == true)
            {
                FitLineX = MathHelpLib.CurveFitting.MathCurveFits.FitMovingAverage(XData, (int)nudPeriodX.Value);
            }
            if (rbPolynomialX.Checked == true)
            {
                FitLineX = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFit(XData, (int)nudOrderX.Value);
            }
            if (rbTrigX.Checked == true)
            {
                FitLineX = MathHelpLib.CurveFitting.MathCurveFits.TrigFit(XData);
            }

            if (rbMovingAverageY.Checked == true)
            {
                FitLineY = MathHelpLib.CurveFitting.MathCurveFits.FitMovingAverage(YData, (int)nudPeriodY.Value);
            }
            if (rbPolynomialY.Checked == true)
            {
                FitLineY = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFit(YData, (int)nudOrderY.Value);
            }
            if (rbTrigY.Checked == true)
            {
                FitLineY = MathHelpLib.CurveFitting.MathCurveFits.TrigFit(YData);
            }

            Use_X = new double[X_Positions.Length];
            Use_Y = new double[Y_Positions.Length];

            double Mixing = (double)trackBar1.Value / 100d;

            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;
            //combine the fit with the current data with some kind of mix.  usually 100% fit works best
            for (int i = 0; i < X_Positions.Length; i++)
            {
                Use_X[i] = (1 - Mixing) * X_Positions[i] + Mixing * FitLineX[1, i];
                Use_Y[i] = (1 - Mixing) * Y_Positions[i] + Mixing * FitLineY[1, i];

                if (Use_X[i] > maxX) maxX = (int)Use_X[i];
                if (Use_X[i] < minX) minX = (int)Use_X[i];

                if (Use_Y[i] > maxY) maxY = (int)Use_Y[i];
                if (Use_Y[i] < minY) minY = (int)Use_Y[i];
            }

            ///label the cell wander area
            mCellWanderArea = Rectangle.FromLTRB(minX - cct.CellSize.Width, minY - cct.CellSize.Height, maxX + cct.CellSize.Width, maxY + cct.CellSize.Height);

            //draw the current positions
            if (X_Positions != null && Y_Positions != null)
            {
                GraphLine(X_Positions, Y_Positions, FitLineX, FitLineY);
            }
            mPassData.AddSafe("CorrectedX", Use_X);
            mPassData.AddSafe("CorrectedY", Use_Y);

            //send the data up to the static variable for everyone to use
            cct.CXData = Use_X;
            cct.CYData = Use_Y;
        }

        /// <summary>
        /// compares the actual data with a sign and the fitted values
        /// </summary>
        /// <returns></returns>
        private double CheckFit()
        {
            ///correct the format of the data on the x axis
            double[,] YData = new double[2, cct.CYData.Length];
            for (int i = 0; i < X_Positions.Length; i++)
            {
                YData[0, i] = i;
                YData[1, i] = cct.CYData[i];
            }

            ///do a sine fit
            double[,] FitLineY = null;
            FitLineY = MathHelpLib.CurveFitting.MathCurveFits.TrigFit(YData);

            //compare the actual data to the curve fit, sine fit
            double sum = 0;
            double sumAFit = 0;
            double d = 0;
            int minY = int.MaxValue, maxY = int.MinValue;
            for (int i = 0; i < X_Positions.Length; i++)
            {
                d = (Y_Positions[i] - FitLineY[1, i]);
                sum += d * d;
                if (Y_Positions[i] > maxY) maxY = (int)Y_Positions[i];
                if (Y_Positions[i] < minY) minY = (int)Y_Positions[i];

                d = (Y_Positions[i] - cct.CYData[i]);
                sumAFit += d * d;

            }

            sum = Math.Round(100 - (sum / X_Positions.Length) / (maxY - minY) * 100d, 2);
            sumAFit = 100 - (sumAFit / X_Positions.Length) / (maxY - minY) * 100d;

            mPassData.AddSafe("CenterAccuracyActual", sumAFit);
            mPassData.AddSafe("CenterAccuracy", sum);
            return sum;
        }

        protected DataEnvironment mDataEnvironment;

        protected Bitmap mScratchImage;

        public virtual void ShowInterface(IWin32Window Owner)
        {
            this.Show(Owner);
            Application.DoEvents();

            mFilterToken = null;

        }

        protected virtual void button1_Click(object sender, EventArgs e)
        {

            this.Hide();
        }

        protected virtual void DoRun()
        {
            FitLines();
        }

        /// <summary>
        /// used to animate the curve fit for the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ImageFilenames != null && ImageFilenames[0] != "")
            {
                try
                {
                    if (mScratchImage == null)
                    {
                        mScratchImage = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format32bppRgb);
                    }

                    ImageIndex += 2;
                    if (ImageIndex >= ImageFilenames.Length) ImageIndex = 0;
                    Bitmap b = MathHelpLib.MathHelpsFileLoader.Load_Bitmap(ImageFilenames[ImageIndex]).ToBitmap();

                    Graphics g = Graphics.FromImage(mScratchImage);

                    int X = (int)(mScratchImage.Width / 2 - Use_X[ImageIndex]);
                    int Y = (int)(mScratchImage.Height / 2 - Use_Y[ImageIndex]);

                    g.DrawImageUnscaled(b, new Point(X, Y));
                    //  g.DrawImage(b, new Rectangle(X, Y, mCellWanderArea.Width, mCellWanderArea.Height), mCellWanderArea, GraphicsUnit.Pixel);

                    int MidX = mScratchImage.Width / 2;
                    int MidY = mScratchImage.Height / 2;

                    g.DrawLine(Pens.Blue, new Point(MidX, MidY - 10), new Point(MidX, MidY + 10));
                    g.DrawLine(Pens.Blue, new Point(MidX - 10, MidY), new Point(MidX + 10, MidY));
                    g.Dispose();

                    Bitmap ClippedBitmap = new Bitmap(cct.CellSize.Width * 2, mCellWanderArea.Height);
                    g = Graphics.FromImage(ClippedBitmap);
                    g.DrawImageUnscaled(mScratchImage, new Point(-1 * (int)(mScratchImage.Width / 2d - cct.CellSize.Width * 1), 0));

                    // Graphics  g = Graphics.FromImage(b);
                    //g.DrawRectangle(Pens.Red, (int)Math.Truncate( Use_X[ImageIndex])-85,(int)Math.Truncate( Use_Y[ImageIndex])-85, 170, 170);
                    // catch { }
                    pictureBox1.Image = ClippedBitmap;

                    pictureBox1.Invalidate();
                }
                catch { }
            }
        }

        #region events
        private void rbMovingAverageX_CheckedChanged(object sender, EventArgs e)
        {
            mFilterToken[4] = "MovingAverage";
            mFilterToken[5] = nudPeriodX.Value;

            DoRun();
        }

        private void rbPolynomialX_CheckedChanged(object sender, EventArgs e)
        {
            mFilterToken[4] = "Polynomial";
            mFilterToken[5] = nudOrderX.Value;
            DoRun();
        }

        private void rbTrigX_CheckedChanged(object sender, EventArgs e)
        {
            mFilterToken[4] = "Trig";
            mFilterToken[5] = 0;
            DoRun();
        }

        private void rbMovingAverageY_CheckedChanged(object sender, EventArgs e)
        {
            mFilterToken[6] = "MovingAverage";
            mFilterToken[7] = nudPeriodY.Value;
            DoRun();
        }

        private void rbPolynomialY_CheckedChanged(object sender, EventArgs e)
        {
            mFilterToken[6] = "Polynomial";
            mFilterToken[7] = nudOrderY.Value;
            DoRun();
        }

        private void rbTrigY_CheckedChanged(object sender, EventArgs e)
        {
            mFilterToken[6] = "Trig";
            mFilterToken[7] = 0;
            DoRun();
        }

        private void nudPeriodY_ValueChanged(object sender, EventArgs e)
        {
            mFilterToken[7] = nudPeriodY.Value;
            DoRun();
        }

        private void nudOrderY_ValueChanged(object sender, EventArgs e)
        {
            mFilterToken[7] = nudOrderY.Value;
            DoRun();
        }

        private void nudPeriodX_ValueChanged(object sender, EventArgs e)
        {
            mFilterToken[5] = nudPeriodX.Value;
            DoRun();
        }

        private void nudOrderX_ValueChanged(object sender, EventArgs e)
        {
            mFilterToken[5] = nudOrderX.Value;
            DoRun();
        }
        #endregion

        private void CenterCellsTool2Form_Resize(object sender, EventArgs e)
        {
            groupBox1.Width = panel1.Width / 2 - 5;
            groupBox2.Width = panel1.Width / 2 - 5;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            Application.DoEvents();
            SaveFileDialogExplain sfd = new SaveFileDialogExplain("Please indicate the directory and filename pattern for outputting the cut centers");
            DialogResult ret = sfd.ShowDialog();
            if (ret == DialogResult.OK)
            {
                string FilePathOut = Path.GetDirectoryName(sfd.MSDialog.FileName) + "\\";
                string FileName = FilePathOut + Path.GetFileNameWithoutExtension(sfd.MSDialog.FileName);
                string Exten = Path.GetExtension(sfd.MSDialog.FileName);

                Size CutSize = new Size((int)numCutWidth.Value, (int)nudCutHeight.Value);
                Bitmap ScratchImage = new Bitmap(2 * CutSize.Width, 2 * CutSize.Height, PixelFormat.Format32bppRgb);

                int hHalf = ScratchImage.Height / 2;
                int hWidth = ScratchImage.Width / 2;


                Rectangle CutRect = new Rectangle((hWidth - CutSize.Width / 2), hHalf - CutSize.Height / 2, CutSize.Width, CutSize.Height);
                Rectangle DisplayRect = new Rectangle((hWidth - CutSize.Width / 2), hHalf - CutSize.Height / 2, CutSize.Width, CutSize.Height);
                DisplayRect.Inflate(3, 3);
                string[] NewFilenames = new string[ImageFilenames.Length];
                for (int i = 0; i < ImageFilenames.Length; i++)
                {
                    Bitmap b = new Bitmap(ImageFilenames[i]);
                    Graphics g = Graphics.FromImage(ScratchImage);
                    // try
                    {
                        g.Clear(Color.White);
                        g.DrawImage(b,
                            new Point(
                                (int)((double)ScratchImage.Width / 2d - Use_X[i]),
                                (int)((double)ScratchImage.Height / 2d - Use_Y[i])));
                        //g.DrawRectangle(Pens.Red, DisplayRect);
                        pictureBox1.Image = ScratchImage;
                        pictureBox1.Invalidate();
                        Application.DoEvents();

                    }
                    // catch { }
                    g = null;
                    b = ImagingTools.ClipImageExactCopy(ScratchImage, CutRect);
                    NewFilenames[i] = FileName + string.Format("{0:000}", i) + Exten;
                    b.Save(NewFilenames[i]);
                    b = null;
                }
            }
            timer1.Enabled = true;
        }

        private void bLoadX_Click(object sender, EventArgs e)
        {
            try
            {
                IEffect filter = new ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool();

                filter.DoEffect(mDataEnvironment, null, mPassData, tbXArrayName.Text, tbXArrayName.Text);

                mFilterToken[2] = filter.PassData;

                DoEffect(mDataEnvironment, null, mPassData, mFilterToken);
                DoRun();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bLoadYArray_Click(object sender, EventArgs e)
        {
            try
            {
                IEffect filter = new ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool();

                filter.DoEffect(mDataEnvironment, null, mPassData, tbYArrayName.Text, tbYArrayName.Text);

                mFilterToken[2] = filter.PassData;

                DoEffect(mDataEnvironment, null, mPassData, mFilterToken);
                DoRun();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            mScratchImage = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format32bppRgb);
        }
    }
}
