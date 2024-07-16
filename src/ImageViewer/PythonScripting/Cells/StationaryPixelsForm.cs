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
using System.Threading;
using System.IO;
using MathHelpLib;
using MathHelpLib.ImageProcessing;
using ImageViewer.PythonScripting.Threads;
using ImageViewer.PythonScripting;
using ImageViewer.Filters.Effects.Flattening;

namespace ImageViewer.Filters
{
    public partial class StationaryPixelsForm : Form, IEffect
    {

        public string EffectName { get { return "Stationary Pixels"; } }
        public string EffectMenu { get { return "Macros"; } }
        public string EffectSubMenu { get { return "Cell Tools"; } }
        public int OrderSuggestion { get { return 1; } }

        public object[] DefaultProperties
        {
            get { return new object[] { new string[] { "" }, 0, "TopAndBottom", new double[] { 0, 1, 2, 3 }, new double[] { 0, 1, 2, 3 }, true, "Divide", false, 0, 170, false }; }
        }

        public string[] ParameterList
        {
            get
            {
                return new string[] { "BitmapFiles|string[]", "strength|int", "FindMethod|string",
                                    "X_Positions|double[]", "Y_Positions|double[]", 
                                    "ShowForm|bool", "SubtractMethod|string","Threaded|bool","ThreadID|int, MaxCellSizeLength|int" , "UseLastStable|bool"
                };
            }
        }




        #region Lines
        string[] ImageFilenames;
        double[] X_Positions;
        double[] Y_Positions;

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

        void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            mFilterToken[1] = trackBar1.Value;
            DoRun();
        }

        public virtual string getMacroString()
        {
            return EffectHelps.FormatMacroString(this, mFilterToken);
        }

        public StationaryPixelsForm()
        {
            InitializeComponent();
        }

        public StationaryPixelsForm(bool NoForm)
        {
            if (!NoForm)
                InitializeComponent();
        }

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
            get { return ""; }
        }


        private void SetupForm(bool Threaded)
        {

            mScratchImage = new ImageHolder(pictureBox1.Width, pictureBox1.Height);

            pictureBox1.Image = mScratchImage.ToBitmap();



            if ((ShowForm) == true)
            {
                timer1.Enabled = true;
                this.Show();
            }

            if (ImageFilenames == null || ImageFilenames[0] == "")
            {
                return;
            }

            // this has to be called last or it will screw up the token
            ///////////////////////////////////////////////////////////
            if ((SubtractionMethod).ToLower() == "divide")
                rbDivide.Checked = true;
            else if ((SubtractionMethod).ToLower() == "divideone")
                rbDivideOne.Checked = true;
            else
                rbSubtraction.Checked = true;

            trackBar1.Value = Strength;

            string Method = (MaskFindMethod).ToLower();
            if (Method == "mask")
                rbMask.Checked = true;
            else if (Method == "strip")
                rbStrips.Checked = true;
            else
                rbTopAndbottom.Checked = true;

            timer1.Enabled = false;
        }

        protected object[] mFilterToken;


        private int ThreadID;

        static double[,] StableMask;

        //static bool MaskCreated = false;
        public class RemoveBackgroundToken : IEffectToken
        {
            public string TokenName()
            {
                return this.ToString();
            }

            public double[,] Mask = null;
            public ImageHolder OutImage = null;
            public int MaxCellSize = 110;

            public double[,] Average = null;
            public double[,] AverageTop = null;
            public double[,] AverageBottom = null;
            public double BottomCount = 0;
            public double TopCount = 0;
            public object MaskCriticalSection = new object();
            public object MaskCriticalSection2 = new object();
            public object[] LineLocks = null;
            public double HalfPoint = 0;
            public bool PostProcessingDone = false;
            public object TopLock = new object();
            public object BottomLock = new object();

            public System.Diagnostics.Stopwatch mStopWatch = new System.Diagnostics.Stopwatch();
        }

        RemoveBackgroundToken removeToken;

        static object CriticalSectionLock = new object();


        int Strength;
        string MaskFindMethod;
        string XArrayName, YArrayName;
        int ThreadNumber;
        int CellSize;
        bool ShowForm;
        string SubtractionMethod;
        bool UseStableBackground;
        int[] IndexArray = null;


        public static double[,] MedianBackground(DataEnvironment dataEnvironment, double[] XArray, double[] YArray, int CellSize, bool FixBlackSpot)
        {
            try
            {
                ImageHolder example = dataEnvironment.AllImages[0];
                //return example;
                int width = example.Width;
                int height = example.Height;

                double[,] arrayOut = new double[height, width];

                //float[] PixelVals = new float[dataEnvironment.AllImages.Count];

                Rectangle[] CellBounds = new Rectangle[dataEnvironment.AllImages.Count];
                CellSize += 25;
                int halfCell = (int)(CellSize / 2d);

                Rectangle intersection = new Rectangle((int)(XArray[0] - halfCell), (int)(YArray[0] - halfCell), CellSize, CellSize);
                for (int i = 0; i < dataEnvironment.AllImages.Count; i++)
                {
                    Rectangle r = new Rectangle((int)(XArray[i] - halfCell), (int)(YArray[i] - halfCell), CellSize, CellSize);
                    intersection = Rectangle.Intersect(intersection, r);
                }

                if (intersection.Height != 0)
                    throw new Exception("Unable to get background");

                double d, MaxValue = double.MinValue;
                float val;
                int nImages = 40;
                int step = (int)(dataEnvironment.AllImages.Count / (float)nImages);
                List<float>[,] PixelList = new List<float>[height, width];// (new float[20]);

                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        PixelList[i, j] = new List<float>(new float[nImages]);

                Thread[] Ts = new Thread[nImages];
                for (int k = 0; k < nImages; k++)
                {
                    Ts[k] = new Thread(delegate(object imageIndex)
                        {
                            int ImageIndex = (int)imageIndex;
                            int cc = 0;
                            int index = 0;
                            float[, ,] data = dataEnvironment.AllImages[ImageIndex * step].ImageData;

                            //now go through each pixel to get its sorted list
                            for (int i = 0; i < height; i++)
                            {
                                for (int j = 0; j < width; j++)
                                {
                                    PixelList[i, j][ImageIndex] = data[i, j, 0];
                                }
                            }
                        });
                    Ts[k].Start(k);
                }

                for (int i = 0; i < Ts.Length; i++)
                    Ts[i].Join();


                for (int k = 0; k < nImages; k++)
                {
                    Ts[k] = new Thread(delegate(object imageSection)
                        {
                            int ImageIndex = (int)imageSection;
                            int cc = 0;
                            int index = 0;
                            float[, ,] data = dataEnvironment.AllImages[ImageIndex].ImageData;
                            //now go through each pixel to get its sorted list
                            int startI = (int)(height / (double)nImages * ImageIndex);
                            int endI = (int)(height / (double)nImages * (ImageIndex + 1));
                            if (endI > height) endI = height;
                            int Target = (int)(nImages*3f/4f); 
                            for (int i = startI; i < endI; i++)
                            {
                                for (int j = 0; j < width; j++)
                                {
                                    List<float> pl = PixelList[i, j];
                                    pl.Sort();
                                    arrayOut[i, j] = (pl[Target + 1] + pl[Target ] + pl[Target - 1] + pl[Target - 2]) / 4f;// (pl[nImages - 1] + pl[nImages - 2] + pl[nImages - 3] + pl[nImages - 4]) / 4f;
                                }
                            }
                        });
                    Ts[k].Start(k);
                }
                for (int i = 0; i < Ts.Length; i++)
                    Ts[i].Join();


                FlattenEdges1DErrorCorrected.FlattenImageEdgesGlobalO(arrayOut);


                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        d = arrayOut[i, j];
                        if (d > MaxValue) MaxValue = d;
                    }
                }

                MathHelpLib.MathArrayHelps.DivideInPlace(arrayOut, MaxValue);


                return arrayOut;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get background" + ex.Message);
            }
        }

        public static double[,] AverageBackground(DataEnvironment dataEnvironment, double[] XArray, double[] YArray, int CellSize, bool FixBlackSpot)
        {
            try
            {
                ImageHolder example = dataEnvironment.AllImages[0];
                //return example;
                int width = example.Width;
                int height = example.Height;

                double[,] arrayOut = new double[height, width];

                //float[] PixelVals = new float[dataEnvironment.AllImages.Count];

                Rectangle[] CellBounds = new Rectangle[dataEnvironment.AllImages.Count];
                CellSize += 25;
                int halfCell = (int)(CellSize / 2d);

                Rectangle intersection = new Rectangle((int)(XArray[0] - halfCell), (int)(YArray[0] - halfCell), CellSize, CellSize);
                for (int i = 0; i < dataEnvironment.AllImages.Count; i++)
                {
                    Rectangle r = new Rectangle((int)(XArray[i] - halfCell), (int)(YArray[i] - halfCell), CellSize, CellSize);
                    intersection = Rectangle.Intersect(intersection, r);
                }

                if (intersection.Height != 0)
                    throw new Exception("Unable to get background");

                double d, MaxValue = double.MinValue;
                float val;
                int nImages = 40;
                int step = (int)(dataEnvironment.AllImages.Count / (float)nImages);
                List<float>[,] PixelList = new List<float>[height, width];// (new float[20]);

                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        PixelList[i, j] = new List<float>(new float[nImages]);

                Thread[] Ts = new Thread[nImages];
                for (int k = 0; k < nImages; k++)
                {
                    Ts[k] = new Thread(delegate(object imageIndex)
                    {
                        int ImageIndex = (int)imageIndex;
                        int cc = 0;
                        int index = 0;
                        float[, ,] data = dataEnvironment.AllImages[ImageIndex * step].ImageData;

                        //now go through each pixel to get its sorted list
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                PixelList[i, j][ImageIndex] = data[i, j, 0];
                            }
                        }
                    });
                    Ts[k].Start(k);
                }

                for (int i = 0; i < Ts.Length; i++)
                    Ts[i].Join();


                for (int k = 0; k < nImages; k++)
                {
                    Ts[k] = new Thread(delegate(object imageSection)
                    {
                        int ImageIndex = (int)imageSection;
                        int cc = 0;
                        int index = 0;
                        float[, ,] data = dataEnvironment.AllImages[ImageIndex].ImageData;
                        //now go through each pixel to get its sorted list
                        int startI = (int)(height / (double)nImages * ImageIndex);
                        int endI = (int)(height / (double)nImages * (ImageIndex + 1));
                        if (endI > height) endI = height;
                        int Target = (int)(nImages * 1f / 2f);
                        for (int i = startI; i < endI; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                List<float> pl = PixelList[i, j];
                                pl.Sort();
                                
                                arrayOut[i, j] = (pl[Target + 1] + pl[Target] + pl[Target - 1] + pl[Target - 2]) / 4f;// (pl[nImages - 1] + pl[nImages - 2] + pl[nImages - 3] + pl[nImages - 4]) / 4f;
                            }
                        }
                    });
                    Ts[k].Start(k);
                }
                for (int i = 0; i < Ts.Length; i++)
                    Ts[i].Join();


                FlattenEdges1DErrorCorrected.FlattenImageEdgesGlobalO(arrayOut);


                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        d = arrayOut[i, j];
                        if (d > MaxValue) MaxValue = d;
                    }
                }

                MathHelpLib.MathArrayHelps.DivideInPlace(arrayOut, MaxValue);


                return arrayOut;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get background" + ex.Message);
            }
        }


        // Filter.DoEffect2(dataEnvironment, BitmapImage, GlobalPassData, 100,BackgroundMethod , X_PositionsB2, Y_PositionsB, False, "Divide",  tCellSize, True)
        public static ImageHolder AverageBackgroundO(DataEnvironment dataEnvironment, double[] XArray, double[] YArray, int CellSize, bool FixBlackSpot)
        {

            ImageHolder example = dataEnvironment.AllImages[0];
            //return example;
            ImageHolder ImageOut = new ImageHolder(example.Width, example.Height, 1);

            //float[] PixelVals = new float[dataEnvironment.AllImages.Count];

            Rectangle[] CellBounds = new Rectangle[dataEnvironment.AllImages.Count];

            int halfCell = (int)(CellSize / 2d);

            for (int i = 0; i < dataEnvironment.AllImages.Count; i++)
            {
                CellBounds[i] = new Rectangle((int)(XArray[i] - halfCell), (int)(YArray[i] - halfCell), CellSize, CellSize);
            }

            double MaxValue = double.MinValue;
            float val;
            long[,] Count = new long[example.ImageData.GetLength(0), example.ImageData.GetLength(1)];
            double[,] sum = new double[example.ImageData.GetLength(0), example.ImageData.GetLength(1)];

            for (int k = 0; k < dataEnvironment.AllImages.Count; k++)
            {
                float[, ,] data = dataEnvironment.AllImages[k].ImageData;
                for (int i = 0; i < example.ImageData.GetLength(0); i++)
                    for (int j = 0; j < example.ImageData.GetLength(1); j++)
                    {
                        Point Loc = new Point(j, i);

                        if (CellBounds[k].Contains(Loc) == false)
                        {
                            sum[i, j] += data[i, j, 0];
                            Count[i, j]++;
                        }
                    }
            }


            bool BlackSpot = false;
            int MinBlackJ = int.MaxValue;
            int MaxBlackJ = int.MinValue;
            int MinBlackI = int.MaxValue;
            int MaxBlackI = int.MinValue;

            float[, ,] dataOut = ImageOut.ImageData;
            for (int i = 0; i < example.ImageData.GetLength(0); i++)
                for (int j = 0; j < example.ImageData.GetLength(1); j++)
                {
                    try
                    {
                        if (Count[i, j] != 0)
                        {
                            val = (float)(sum[i, j] / Count[i, j]); ;
                            dataOut[i, j, 0] = val;
                            if (val > MaxValue) MaxValue = val;
                        }
                        else
                        {
                            BlackSpot = true;
                            if (i < MinBlackI) MinBlackI = i;
                            if (i > MaxBlackI) MaxBlackI = i;

                            if (j < MinBlackJ) MinBlackJ = j;
                            if (j > MaxBlackJ) MaxBlackJ = j;

                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(ex.Message);
                    }
                }

            if (BlackSpot == true)
            {
                if (FixBlackSpot && false)
                {
                    int MinI = MinBlackI - 1;
                    int MinJ = MinBlackJ - 1;
                    for (int i = MinBlackI; i < MaxBlackI + 1; i++)
                        for (int j = MinBlackJ; j < MaxBlackJ + 1; j++)
                        {
                            float w1 = 1f / (j - MinJ);
                            float w2 = 1f / (i - MinI);
                            float w3 = 1f / (MaxBlackJ + 1 - j);
                            float w4 = 1f / (MaxBlackI + 1 - i);
                            val = dataOut[MaxBlackI + 1, j, 0];
                            //  dataOut[i, j, 0] = (dataOut[MinI, j, 0] * w2 + dataOut[MaxBlackI + 1, j, 0] * w4 + dataOut[i, MinJ, 0] * w1 + dataOut[i, MaxBlackJ + 1, 0] * w3) / (w1 + w2 + w3 + w4);
                            dataOut[i, j, 0] = (dataOut[MinI, j, 0] * w2 + dataOut[MaxBlackI + 1, j, 0] * w4) / (w2 + w4);
                        }
                }
                else
                    throw new Exception("Could not form background");
            }

            /*           if (Count > 0)
                        {
                            val =(float)( sum / Count);
                            if (val > MaxValue) MaxValue = val;
                            ImageOut.ImageData[i, j, 0] = val;
                        }
                        else
                            ImageOut.ImageData[i, j, 0] = 0;*/


            MathHelpLib.MathArrayHelps.DivideInPlace(ImageOut.ImageData, (float)MaxValue);


            return ImageOut;
        }

        public ImageHolder FindStationaryPixels(DataEnvironment dataEnvironment, ReplaceStringDictionary PassData, int Strength, string MaskFindMethod, double[] XArray, double[] YArray,
            bool ShowForm, string SubtractionMethod, int ThreadID, int CellSize, bool UseStableBackground, List<ImageFile> imagesNames)
        {
            mPassData = PassData;
            mDataEnvironment = dataEnvironment;


            X_Positions = XArray;
            Y_Positions = YArray;

            IndexArray = new int[imagesNames.Count];
            for (int i = 0; i < imagesNames.Count; i++)
                IndexArray[i] = imagesNames[i].Index;


            lock (CriticalSectionLock)
            {


                if (dataEnvironment.EffectTokens.ContainsKey("StationaryPixels") == true)
                    removeToken = (RemoveBackgroundToken)dataEnvironment.EffectTokens["StationaryPixels"];
                else
                {
                    removeToken = new RemoveBackgroundToken();
                    dataEnvironment.EffectTokens.Add("StationaryPixels", removeToken);
                    try
                    {
                        SetupForm(dataEnvironment.RunningThreaded);
                    }
                    catch { }
                }


                if (CellSize > removeToken.MaxCellSize) removeToken.MaxCellSize = (int)CellSize;
                Console.WriteLine("CellSize is : " + removeToken.MaxCellSize.ToString());
            }

            if (mFilterToken == null)
            {
                mFilterToken = DefaultProperties;
            }




            ///get a list of the filenames
            ImageFilenames = mDataEnvironment.WholeFileList.ToArray();

            //make the mask
            // try
            {
                CreateMaskThreaded(IndexArray, Y_Positions, BackGroundSubtractionMethod.TopAndBottom);
            }
            /*  catch (Exception ex)
              {
                  mPassData.AddSafe("MaxCellSize", removeToken.MaxCellSize);
                  JoinThreadsTool.ReleaseAllJoins(dataEnvironment);
                  mPassData.AddSafe("BackgroundMask", removeToken.Mask);
                  throw ex;
              }*/
            mPassData.AddSafe("MaxCellSize", removeToken.MaxCellSize);
            mPassData.AddSafe("BackgroundMask", removeToken.Mask);


            return removeToken.OutImage;

        }

        public object DoEffect(DataEnvironment dataEnvironment, object SourceImage, ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            ThreadID = 0;

            Strength = (int)Parameters[0];
            MaskFindMethod = (string)Parameters[1];
            XArrayName = (string)Parameters[2];
            YArrayName = (string)Parameters[3];
            ShowForm = (bool)Parameters[4];
            SubtractionMethod = (string)Parameters[5];
            ThreadID = (int)Parameters[6];
            CellSize = (int)Parameters[7];
            UseStableBackground = (bool)Parameters[8];
            List<ImageFile> imagesNames = (List<ImageFile>)Parameters[9];


            mPassData = PassData;
            mFilterToken = Parameters;
            mDataEnvironment = dataEnvironment;


            X_Positions = ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GetGlobalArray(XArrayName, dataEnvironment).CopyOutArray;
            Y_Positions = ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GetGlobalArray(YArrayName, dataEnvironment).CopyOutArray;


            return FindStationaryPixels(dataEnvironment, PassData, Strength, MaskFindMethod, X_Positions, Y_Positions, ShowForm, SubtractionMethod, ThreadID, CellSize, UseStableBackground, imagesNames);


        }

        public object DoEffect2(DataEnvironment dataEnvironment, object SourceImage, ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            mPassData = PassData;
            mDataEnvironment = dataEnvironment;

            ThreadID = 0;

            Strength = (int)Parameters[0];
            MaskFindMethod = (string)Parameters[1];
            X_Positions = (double[])Parameters[2];
            Y_Positions = (double[])Parameters[3];
            ShowForm = (bool)Parameters[4];
            SubtractionMethod = (string)Parameters[5];
            CellSize = (int)Parameters[6];
            UseStableBackground = (bool)Parameters[7];


            ///get a list of the filenames
            ImageFilenames = mDataEnvironment.WholeFileList.ToArray();

            IndexArray = new int[ImageFilenames.Length];
            for (int i = 0; i < ImageFilenames.Length; i++)
                IndexArray[i] = i;// imagesNames[i].Index;



            lock (CriticalSectionLock)
            {
                if (dataEnvironment.EffectTokens.ContainsKey("StationaryPixels") == true)
                    removeToken = (RemoveBackgroundToken)dataEnvironment.EffectTokens["StationaryPixels"];
                else
                {
                    removeToken = new RemoveBackgroundToken();
                    dataEnvironment.EffectTokens.Add("StationaryPixels", removeToken);
                    try
                    {
                        SetupForm(dataEnvironment.RunningThreaded);
                    }
                    catch { }
                }


                if (CellSize > removeToken.MaxCellSize) removeToken.MaxCellSize = (int)CellSize;
                Console.WriteLine("CellSize is : " + removeToken.MaxCellSize.ToString());
            }

            if (mFilterToken == null)
            {
                mFilterToken = DefaultProperties;
            }

            #region Threaded

            //make the mask
            try
            {
                CreateMask(IndexArray, Y_Positions, BackGroundSubtractionMethod.TopAndBottom);
            }
            catch (Exception ex)
            {
                mPassData.AddSafe("MaxCellSize", removeToken.MaxCellSize);
                JoinThreadsTool.ReleaseAllJoins(dataEnvironment);
                mPassData.AddSafe("BackgroundMask", removeToken.Mask);
                return removeToken.OutImage;
                throw ex;
            }

            mPassData.AddSafe("MaxCellSize", removeToken.MaxCellSize);
            mPassData.AddSafe("BackgroundMask", removeToken.Mask);

            return removeToken.OutImage;
            #endregion
        }

        private void MakeMask()
        {
            if (removeToken.Mask == null)
            {
                CreateMask();
            }

            pictureBox1.Image = MathImageHelps.MakeBitmap(removeToken.Mask);
            // timer1.Enabled = true;

            if ((ShowForm) == true)
            {
                pictureBox2.Image = SubtractBackGround(ImageFilenames[0]);
            }

            pictureBox1.Invalidate();
            pictureBox2.Invalidate();

        }

        private Bitmap SubtractBackGround(string ForeGroundFile)
        {
            double[,] ForeGround = MathHelpsFileLoader.LoadStandardImage_Intensity(ForeGroundFile, false);
            double[,] OutArray = new double[ForeGround.GetLength(0), ForeGround.GetLength(1)];
            double Factor = (double)trackBar1.Value / 100d;
            if (rbSubtraction.Checked == true)
            {
                for (int i = 0; i < ForeGround.GetLength(0); i++)
                    for (int j = 0; j < ForeGround.GetLength(1); j++)
                        OutArray[i, j] = ForeGround[i, j] - removeToken.Mask[i, j] * Factor;
            }
            if (rbDivide.Checked == true)
            {

                for (int i = 0; i < ForeGround.GetLength(0); i++)
                    for (int j = 0; j < ForeGround.GetLength(1); j++)
                    {
                        OutArray[i, j] = ForeGround[i, j] / (removeToken.Mask[i, j] * Factor);
                    }
            }
            if (rbDivideOne.Checked == true)
            {
                Factor = Factor / 255;
                for (int i = 0; i < ForeGround.GetLength(0); i++)
                    for (int j = 0; j < ForeGround.GetLength(1); j++)
                    {
                        OutArray[i, j] = ForeGround[i, j] / (removeToken.Mask[i, j] * Factor + 1);
                    }
            }
            return MathImageHelps.MakeBitmap(OutArray);
        }

        private void GetTopAndBottomByMargin(int[] Indexs, double[] YValues, int TopHalfPoint, double Margin)
        {
            float[,] TestImage;
            double d = 0;
            for (int i = 0; i < Indexs.Length; i++)
            {
                if (Y_Positions[Indexs[i]] - ((double)removeToken.MaxCellSize * Margin) > removeToken.HalfPoint)
                {
                    lock (removeToken.BottomLock)
                    {
                        removeToken.BottomCount++;

                        TestImage = mDataEnvironment.AllImages[Indexs[i]].ToDataIntensity();
                        for (int y = 0; y < removeToken.HalfPoint + 5; y++)
                        {
                            for (int x = 0; x < TestImage.GetLength(1); x++)
                            {
                                d = TestImage[y, x];
                                removeToken.AverageBottom[y, x] += d;
                            }
                        }
                    }
                }
                else if (Y_Positions[Indexs[i]] + ((double)removeToken.MaxCellSize * Margin) <= removeToken.HalfPoint)
                {
                    lock (removeToken.TopLock)
                    {
                        removeToken.TopCount++;

                        TestImage = mDataEnvironment.AllImages[Indexs[i]].ToDataIntensity();
                        for (int y = TopHalfPoint; y < TestImage.GetLength(0); y++)
                        {
                            for (int x = 0; x < TestImage.GetLength(1); x++)
                            {
                                d = TestImage[y, x];
                                removeToken.AverageTop[y - TopHalfPoint, x] += d;
                            }
                        }
                    }
                }
            }
        }


        private void GetTopAndBottomByPosition(double[] YValues, int TopHalfPoint)
        {
            float[,] TestImage;
            double MaxValue = double.MinValue, MinValue = double.MaxValue;
            int MaxPos = 0, MinPos = 0;
            for (int i = 10; i < YValues.Length - 10; i++)
            {
                if (YValues[i] > MaxValue)
                {
                    MaxValue = YValues[i];
                    MaxPos = i;
                }
                if (YValues[i] < MinValue)
                {
                    MinValue = YValues[i];
                    MinPos = i;
                }
            }


            double d = 0;
            for (int i = 0; i < mDataEnvironment.AllImages.Count; i++)
            {
                if (Math.Abs(i - MaxPos) < 5)
                {
                    lock (removeToken.BottomLock)
                    {
                        removeToken.BottomCount++;

                        TestImage = mDataEnvironment.AllImages[i].ToDataIntensity();
                        for (int y = 0; y < removeToken.HalfPoint + 5; y++)
                        {
                            for (int x = 0; x < TestImage.GetLength(1); x++)
                            {
                                d = TestImage[y, x];
                                removeToken.AverageBottom[y, x] += d;
                            }
                        }

                    }
                }
                else if (Math.Abs(i - MinPos) < 5)
                {
                    lock (removeToken.TopLock)
                    {
                        removeToken.TopCount++;

                        TestImage = mDataEnvironment.AllImages[i].ToDataIntensity();
                        for (int y = TopHalfPoint; y < TestImage.GetLength(0); y++)
                        {
                            for (int x = 0; x < TestImage.GetLength(1); x++)
                            {
                                d = TestImage[y, x];
                                removeToken.AverageTop[y - TopHalfPoint, x] += d;
                            }
                        }



                    }
                }
            }

        }

        private bool HandleNoBackground(RemoveBackgroundToken removeBackGround, double[] YValues, int TopHalfPoint)
        {
            string AllBackground;
            if (mDataEnvironment.DataOutFolder.EndsWith("\\"))
                AllBackground = Directory.GetParent(Directory.GetParent(mDataEnvironment.DataOutFolder).FullName).FullName + "\\AllBackground.cct";
            else
                AllBackground = Directory.GetParent(mDataEnvironment.DataOutFolder).FullName + "\\AllBackground.cct";
            //string AllBackground = mDataEnvironment.DataOutFolder + "\\AllBackground.cct";
            // MessageBox.Show(AllBackground);
            try
            {
                //if the previous background mask matches, then use this mask.  This assumes that the data was taken the same day as the previous run
                if (UseStableBackground == true && StableMask != null && StableMask.GetLength(0) == removeToken.Average.GetLength(0) && StableMask.GetLength(1) == removeToken.Average.GetLength(1))
                {
                    #region PRevious

                    // MessageBox.Show("stable");
                    mDataEnvironment.ProgressLog.AddSafe("BackgroundSubtractionError", "Using previous background");
                    removeToken.Mask = new double[removeToken.Average.GetLength(0), removeToken.Average.GetLength(1)];
                    Buffer.BlockCopy(StableMask, 0, removeToken.Mask, 0, Buffer.ByteLength(removeToken.Mask));
                    removeToken.OutImage = new ImageHolder(removeToken.Mask);
                    removeToken.Average = null;
                    removeToken.HalfPoint = 0;
                    removeToken.PostProcessingDone = true;
                    return false;
                    #endregion
                }
                else if (mDataEnvironment.ExperimentFolder != null && mDataEnvironment.ExperimentFolder != "" && File.Exists(AllBackground) == true)
                {
                    #region LoadFromFile
                    try
                    {
                        //  MessageBox.Show("all");
                        mDataEnvironment.ProgressLog.AddSafe("BackgroundSubtractionError", "Using all average background");
                        ImageHolder background = MathHelpLib.MathHelpsFileLoader.Load_Raw(AllBackground);
                        removeToken.Mask = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(background, false);
                        removeToken.OutImage = background;
                        //   MessageBox.Show("backgroundloaded");
                        removeToken.Average = null;
                        removeToken.HalfPoint = 0;
                        removeToken.PostProcessingDone = true;

                    }
                    catch (Exception ex)
                    {
                        //  MessageBox.Show(ex.Message);
                        throw ex;
                    }
                    return false;
                    #endregion
                }
                else
                {
                    #region NoBackground
                    mDataEnvironment.ProgressLog.AddSafe("BackgroundSubtractionError", "Background Subtraction close");

                    removeToken.TopCount = 0;
                    removeToken.BottomCount = 0;


                    removeToken.AverageBottom = new double[removeToken.AverageBottom.GetLength(0), removeToken.AverageBottom.GetLength(1)];
                    removeToken.AverageTop = new double[removeToken.AverageTop.GetLength(0), removeToken.AverageTop.GetLength(1)];

                    int[] Indexs = new int[YValues.Length];
                    for (int i = 0; i < Indexs.Length; i++)
                        Indexs[i] = i;



                    GetTopAndBottomByMargin(Indexs, YValues, TopHalfPoint, .65);

                    //GetTopAndBottomByPosition( YValues, TopHalfPoint );

                    if (!(removeToken.BottomCount == 0 || removeToken.TopCount == 0))
                    {

                        return true;
                    }
                    else
                    {
                        #region NoBackgroundAtAll
                        // MessageBox.Show("None");
                        mDataEnvironment.ProgressLog.AddSafe("BackgroundSubtraction", "Background Subtraction failed");
                        mDataEnvironment.ProgressLog.AddSafe("BackgroundSubtractionError", "Background Subtraction failed");
                        for (int y = 0; y < removeToken.Average.GetLength(0); y++)
                            for (int x = 0; x < removeToken.Average.GetLength(1); x++)
                            {
                                removeToken.Average[y, x] = 1;
                            }
                        //  removeToken.Average = null;

                        removeToken.Mask = removeToken.Average;
                        removeToken.OutImage = new ImageHolder(removeToken.Average);
                        //  removeToken.Average = null;
                        removeToken.HalfPoint = 0;
                        removeToken.PostProcessingDone = true;

                        throw new Exception("Background subtraction failed");
                        #endregion

                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
                throw ex;
            }
        }

        private void DivideAndSeamTopandBottom(int TopHalfPoint)
        {
            double d;
            //get the intensity along the same line from the top and bottom in order to normalize the two
            double TopInten = 0, BottomInten = 0;
            for (int x = 0; x < removeToken.Average.GetLength(1); x++)
            {
                BottomInten += removeToken.AverageBottom[(int)removeToken.HalfPoint, x];
                TopInten += removeToken.AverageTop[5, x];
            }
            BottomInten = BottomInten / removeToken.BottomCount;
            TopInten = TopInten / removeToken.TopCount;

            double BottomChange = TopInten / BottomInten;

            for (int y = 0; y < removeToken.HalfPoint /*- 5*/; y++)
                for (int x = 0; x < removeToken.Average.GetLength(1); x++)
                {
                    removeToken.Average[y, x] = BottomChange * removeToken.AverageBottom[y, x] / removeToken.BottomCount;
                }

            for (int y = (int)removeToken.HalfPoint /*+ 5*/; y < removeToken.Average.GetLength(0); y++)
            {
                int y2 = (y - TopHalfPoint);
                for (int x = 0; x < removeToken.Average.GetLength(1); x++)
                {
                    d = removeToken.AverageTop[y2, x] / removeToken.TopCount;
                    removeToken.Average[y, x] = d;
                }
            }


            double cc = 0;
            double c = 0;
            for (int y = (int)removeToken.HalfPoint - 5; y < removeToken.HalfPoint + 5; y++)
            {
                int y2 = (y - TopHalfPoint);
                c = 1d / (1 + Math.Exp(-1 * (cc - 5) * 1));
                for (int x = 0; x < removeToken.Average.GetLength(1); x++)
                {
                    removeToken.Average[y, x] = (((1 - c) * removeToken.AverageBottom[y, x] / removeToken.BottomCount * BottomChange + (c) * removeToken.AverageTop[y2, x] / removeToken.TopCount) /*+ Average[y, x]*/);///2d;
                }
                c += .1;
                cc++;
            }


            //make sure there are no zeros, (screw up the division)
            double MaxValue = 0;
            double MinValue = 0;
            for (int x = 0; x < removeToken.Average.GetLength(0); x++)
                for (int y = (int)0; y < removeToken.Average.GetLength(1); y++)
                {
                    if (removeToken.Average[x, y] < MinValue) MinValue = removeToken.Average[x, y];
                    if (removeToken.Average[x, y] > MaxValue) MaxValue = removeToken.Average[x, y];
                    if (removeToken.Average[x, y] == 0) removeToken.Average[x, y] = .001;
                }
            double Range = MaxValue - MinValue;
            //normalize mask to one
            for (int x = 0; x < removeToken.Average.GetLength(0); x++)
                for (int y = (int)0; y < removeToken.Average.GetLength(1); y++)
                {
                    // removeToken.Average[x, y] = (removeToken.Average[x, y]-MinValue) / Range + .001;
                    removeToken.Average[x, y] = removeToken.Average[x, y] / MaxValue;
                }

            removeToken.Mask = removeToken.Average;
        }

        private void CreateMaskTopAndBottom(float[,] TestImage, int[] Indexs, double[] YValues, bool Threaded)
        {
            #region Top and Bottom


            int TopHalfPoint = 0;
            int SeamHalf = 5;
            #region Initialize
            lock (removeToken.MaskCriticalSection2)
            {
                if (removeToken.HalfPoint == 0)
                {
                    removeToken.BottomCount = 0;
                    removeToken.TopCount = 0;
                    removeToken.HalfPoint = 0;
                    double cc = 0;

                    double MaxY = double.MinValue;
                    double MinY = double.MaxValue;

                    for (int i = 0; i < Y_Positions.Length; i++)
                    {
                        if (Y_Positions[i] != 0)
                        {
                            removeToken.HalfPoint += Y_Positions[i];
                            if (Y_Positions[i] > MaxY) MaxY = Y_Positions[i];
                            if (Y_Positions[i] < MinY) MinY = Y_Positions[i];
                            cc++;
                        }
                    }
                    removeToken.HalfPoint = Math.Round(removeToken.HalfPoint / (double)cc);

                    removeToken.AverageTop = new double[(int)(TestImage.GetLength(0) - removeToken.HalfPoint + SeamHalf), TestImage.GetLength(1)];
                    removeToken.AverageBottom = new double[(int)(removeToken.HalfPoint + SeamHalf), TestImage.GetLength(1)];
                }
            }
            #endregion

            TopHalfPoint = (int)(removeToken.HalfPoint - SeamHalf);

            //find those cells that clear the center line by a comfortable margin
            GetTopAndBottomByMargin(Indexs, YValues, TopHalfPoint, .8);

            if (Threaded)
                JoinThreadsTool.JoinThreads(mDataEnvironment, "Stationary2", ThreadID);

            if (ThreadID == 0)
            {
                bool SkipProcess = false;
                //if one of the counts does not get updated, then there is a major problem.  Use the cleanup options
                if (removeToken.TopCount == 0 || removeToken.BottomCount == 0)
                {
                    SkipProcess = (HandleNoBackground(removeToken, YValues, TopHalfPoint) == false);
                }

                if (SkipProcess == false)
                {
                    DivideAndSeamTopandBottom(TopHalfPoint);


                    if (UseStableBackground == true)
                    {
                        if (StableMask == null || StableMask.GetLength(0) != removeToken.Mask.GetLength(0) || StableMask.GetLength(1) != removeToken.Mask.GetLength(1))
                            StableMask = new double[removeToken.Mask.GetLength(0), removeToken.Mask.GetLength(1)];

                        Buffer.BlockCopy(removeToken.Mask, 0, StableMask, 0, Buffer.ByteLength(removeToken.Mask));
                    }

                    removeToken.OutImage = new ImageHolder(removeToken.Average);
                    removeToken.Average = null;
                    removeToken.HalfPoint = 0;
                    removeToken.PostProcessingDone = true;
                    mDataEnvironment.ProgressLog.AddSafe("BackgroundSubtraction", "Background mask created");
                }
            }

            if (Threaded)
                JoinThreadsTool.JoinThreads(mDataEnvironment, "StationaryOutTB", ThreadID);

            #endregion

        }

        public enum BackGroundSubtractionMethod
        {
            TopAndBottom, Strips, Mask

        }

        private void CreateMaskThreaded(int[] Indexs, double[] YValues, BackGroundSubtractionMethod backgroundSubtractionMethod)
        {
            float[,] TestImage = null;
            ///get an example image to initialize everything 
            TestImage = mDataEnvironment.AllImages[Indexs[0]].ToDataIntensity();

            ///if this is the first thread, clean up the vars to make sure they are the correct sizes.
            lock (removeToken.MaskCriticalSection)
            {
                if (removeToken.Average == null)
                {
                    removeToken.Average = new double[TestImage.GetLength(0), TestImage.GetLength(1)];
                    removeToken.LineLocks = new object[TestImage.GetLength(0)];
                    for (int i = 0; i < removeToken.LineLocks.Length; i++)
                        removeToken.LineLocks[i] = new object();
                    removeToken.PostProcessingDone = false;
                }
            }

            if (backgroundSubtractionMethod == BackGroundSubtractionMethod.TopAndBottom)
            {
                CreateMaskTopAndBottom(TestImage, Indexs, YValues, true);
            }
            else if (backgroundSubtractionMethod == BackGroundSubtractionMethod.Strips)
            {
                #region Strips
                #endregion
            }
            else if (backgroundSubtractionMethod == BackGroundSubtractionMethod.Mask)
            {

            }
        }

        private void CreateMask(int[] Indexs, double[] YValues, BackGroundSubtractionMethod backgroundSubtractionMethod)
        {
            float[,] TestImage = null;
            ///get an example image to initialize everything 
            TestImage = mDataEnvironment.AllImages[Indexs[0]].ToDataIntensity();

            ///if this is the first thread, clean up the vars to make sure they are the correct sizes.
            lock (removeToken.MaskCriticalSection)
            {
                if (removeToken.Average == null)
                {
                    removeToken.Average = new double[TestImage.GetLength(0), TestImage.GetLength(1)];
                    removeToken.LineLocks = new object[TestImage.GetLength(0)];
                    for (int i = 0; i < removeToken.LineLocks.Length; i++)
                        removeToken.LineLocks[i] = new object();
                    removeToken.PostProcessingDone = false;
                }
            }

            if (backgroundSubtractionMethod == BackGroundSubtractionMethod.TopAndBottom)
            {
                CreateMaskTopAndBottom(TestImage, Indexs, YValues, false);
            }
            else if (backgroundSubtractionMethod == BackGroundSubtractionMethod.Strips)
            {
                #region Strips
                #endregion
            }
            else if (backgroundSubtractionMethod == BackGroundSubtractionMethod.Mask)
            {

            }
        }

        private void CreateMask()
        {
            double[,] TestImage = MathHelpsFileLoader.LoadStandardImage_Intensity(ImageFilenames[0], false);
            double[,] Average = null;

            if (rbTopAndbottom.Checked == true)
            {
                #region Top and Bottom

                Average = new double[TestImage.GetLength(0), TestImage.GetLength(1)];
                //double HalfPoint = (double)TestImage.GetLength(1) / 2d;
                double HalfPoint = 0;
                for (int i = 0; i < Y_Positions.Length; i++)
                    HalfPoint += Y_Positions[i];
                HalfPoint = Math.Round(HalfPoint / (double)Y_Positions.Length);
                // HalfPoint = TestImage.GetLength(0) / 2;
                double TopCount = 0, BottomCount = 0;
                for (int i = 0; i < ImageFilenames.Length; i += 3)
                {
                    if (Y_Positions[i] - ((double)removeToken.MaxCellSize * .75) > HalfPoint)
                    {
                        BottomCount++;
                        TestImage = MathHelpsFileLoader.LoadStandardImage_Intensity(ImageFilenames[i], false);
                        for (int y = 0; y < HalfPoint; y++)
                            for (int x = 0; x < TestImage.GetLength(1); x++)
                            {
                                Average[y, x] += TestImage[y, x];
                            }
                    }
                    else if (Y_Positions[i] + ((double)removeToken.MaxCellSize * .75) <= HalfPoint)
                    {
                        TopCount++;
                        TestImage = MathHelpsFileLoader.LoadStandardImage_Intensity(ImageFilenames[i], false);
                        for (int y = (int)HalfPoint; y < TestImage.GetLength(0); y++)
                            for (int x = 0; x < TestImage.GetLength(1); x++)
                            {
                                Average[y, x] += TestImage[y, x];
                            }
                    }
                }

                if (TopCount == 0 || BottomCount == 0)
                {
                    for (int y = 0; y < TestImage.GetLength(0); y++)
                        for (int x = 0; x < TestImage.GetLength(1); x++)
                        {
                            Average[y, x] = 1;
                        }
                    removeToken.Mask = Average;
                    return;
                }
                for (int y = 0; y < HalfPoint; y++)
                    for (int x = 0; x < TestImage.GetLength(1); x++)
                    {
                        Average[y, x] /= BottomCount;
                    }

                for (int y = (int)HalfPoint; y < TestImage.GetLength(0); y++)
                    for (int x = 0; x < TestImage.GetLength(1); x++)
                    {
                        Average[y, x] /= TopCount;
                    }


                #endregion
            }
            else if (rbStrips.Checked == true)
            {
                #region Strips

                Average = new double[TestImage.GetLength(0), TestImage.GetLength(1)];
                double[] LineCounts = new double[TestImage.GetLength(1)];

                for (int i = 0; i < ImageFilenames.Length; i++)
                {
                    TestImage = MathHelpsFileLoader.LoadStandardImage_Intensity(ImageFilenames[i], false);
                    for (int y = 0; y < TestImage.GetLength(0); y++)
                    {
                        if (Math.Abs(y - Y_Positions[i]) > 150)
                        {
                            LineCounts[y]++;
                            for (int x = 0; x < TestImage.GetLength(1); x++)
                            {
                                Average[y, x] += TestImage[y, x];
                            }
                        }
                    }
                }


                for (int x = 0; x < TestImage.GetLength(1); x++)
                    for (int y = 0; y < TestImage.GetLength(0); y++)
                    {
                        Average[y, x] /= LineCounts[y];
                    }


                #endregion
            }
            else if (rbMask.Checked == true)
            {

            }


            //make sure there are no zeros, (screw up the division)
            double MaxValue = 0;
            for (int x = 0; x < TestImage.GetLength(0); x++)
                for (int y = (int)0; y < TestImage.GetLength(1); y++)
                {
                    if (Average[x, y] == 0)
                        Average[x, y] = .001;
                    if (Average[x, y] > MaxValue) MaxValue = Average[x, y];
                }
            //normalize mask to one
            for (int x = 0; x < TestImage.GetLength(0); x++)
                for (int y = (int)0; y < TestImage.GetLength(1); y++)
                {
                    Average[x, y] /= MaxValue;
                }
            removeToken.Mask = Average;
        }

        protected DataEnvironment mDataEnvironment;
        protected ImageHolder mScratchImage;


        public virtual void ShowInterface(IWin32Window Owner)
        {
            this.Show(Owner);
            Application.DoEvents();
            // mFilterToken = DefaultToken;
        }

        protected virtual void button1_Click(object sender, EventArgs e)
        {

            this.Hide();
        }

        protected virtual void DoRun()
        {
            MakeMask();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        #region events
        private void rbMovingAverageX_CheckedChanged(object sender, EventArgs e)
        {
            mFilterToken[2] = "TopAndBottom";
            removeToken.Mask = null;
            DoRun();
        }

        private void rbPolynomialX_CheckedChanged(object sender, EventArgs e)
        {
            mFilterToken[2] = "Strip";
            removeToken.Mask = null;
            DoRun();
        }

        private void rbTrigX_CheckedChanged(object sender, EventArgs e)
        {
            mFilterToken[2] = "Mask";
            removeToken.Mask = null;
            DoRun();
        }

        private void rbMovingAverageY_CheckedChanged(object sender, EventArgs e)
        {
            DoRun();
        }

        private void rbPolynomialY_CheckedChanged(object sender, EventArgs e)
        {
            DoRun();
        }

        private void rbTrigY_CheckedChanged(object sender, EventArgs e)
        {
            DoRun();
        }

        private void nudPeriodY_ValueChanged(object sender, EventArgs e)
        {
            DoRun();
        }

        private void nudOrderY_ValueChanged(object sender, EventArgs e)
        {
            DoRun();
        }

        private void nudPeriodX_ValueChanged(object sender, EventArgs e)
        {
            removeToken.Mask = null;
            DoRun();
        }

        private void nudOrderX_ValueChanged(object sender, EventArgs e)
        {
            DoRun();
        }

        private void rbSubtraction_CheckedChanged(object sender, EventArgs e)
        {
            mFilterToken[5] = "Subtract";
            DoRun();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            mFilterToken[5] = "Divide";
            DoRun();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            mFilterToken[5] = "DivideOne";
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
            string OutPath = @"C:\Development\CellCT\Flo1RawPP_cct001_20100511_150225\BackGroundCorrection\";
            for (int i = 0; i < ImageFilenames.Length; i++)
            {
                Bitmap b = SubtractBackGround(ImageFilenames[i]);
                b.Save(OutPath + "bgCorrection" + string.Format("{0:00}", i) + ".bmp");
            }
        }


    }
}
