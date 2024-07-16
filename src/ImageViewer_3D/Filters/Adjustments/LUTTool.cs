using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ZedGraph;

namespace ImageViewer3D.Filters.Adjustments
{
    public partial class LUTTool : aEffectForm3D
    {
        private ZedGraph.ZedGraphControl zedgraphcontrol;
        private Button Finished;
        private PictureBox Preview;
        private SplitContainer splitContainerLoc;
        private System.Windows.Forms.Label ulabel;
        private System.Windows.Forms.Label llabel;
        private TrackBar trackBar2;
        private TrackBar trackBar1;

        private void pInitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Finished = new System.Windows.Forms.Button();
            this.zedgraphcontrol = new ZedGraph.ZedGraphControl();
            this.Preview = new System.Windows.Forms.PictureBox();
            this.splitContainerLoc = new System.Windows.Forms.SplitContainer();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.llabel = new System.Windows.Forms.Label();
            this.ulabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Preview)).BeginInit();
            this.splitContainerLoc.Panel1.SuspendLayout();
            this.splitContainerLoc.Panel2.SuspendLayout();
            this.splitContainerLoc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            this.SuspendLayout();
            // 
            // Finished
            // 
            this.Finished.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Finished.Location = new System.Drawing.Point(1015, 578);
            this.Finished.Name = "Finished";
            this.Finished.Size = new System.Drawing.Size(67, 35);
            this.Finished.TabIndex = 25;
            this.Finished.Text = "Done";
            this.Finished.UseVisualStyleBackColor = true;
            // 
            // zedgraphcontrol
            // 
            this.zedgraphcontrol.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zedgraphcontrol.Location = new System.Drawing.Point(3, 3);
            this.zedgraphcontrol.Name = "zedgraphcontrol";
            this.zedgraphcontrol.ScrollGrace = 0;
            this.zedgraphcontrol.ScrollMaxX = 0;
            this.zedgraphcontrol.ScrollMaxY = 0;
            this.zedgraphcontrol.ScrollMaxY2 = 0;
            this.zedgraphcontrol.ScrollMinX = 0;
            this.zedgraphcontrol.ScrollMinY = 0;
            this.zedgraphcontrol.ScrollMinY2 = 0;
            this.zedgraphcontrol.Size = new System.Drawing.Size(711, 394);
            this.zedgraphcontrol.TabIndex = 26;
            // 
            // Preview
            // 
            this.Preview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Preview.Location = new System.Drawing.Point(0, 0);
            this.Preview.Name = "Preview";
            this.Preview.Size = new System.Drawing.Size(360, 571);
            this.Preview.TabIndex = 27;
            this.Preview.TabStop = false;
            // 
            // splitContainerLoc
            // 
            this.splitContainerLoc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerLoc.Location = new System.Drawing.Point(1, 1);
            this.splitContainerLoc.Name = "splitContainerLoc";
            // 
            // splitContainerLoc.Panel1
            // 
            this.splitContainerLoc.Panel1.Controls.Add(this.Preview);
            // 
            // splitContainerLoc.Panel2
            // 
            this.splitContainerLoc.Panel2.Controls.Add(this.ulabel);
            this.splitContainerLoc.Panel2.Controls.Add(this.llabel);
            this.splitContainerLoc.Panel2.Controls.Add(this.trackBar2);
            this.splitContainerLoc.Panel2.Controls.Add(this.trackBar1);
            this.splitContainerLoc.Panel2.Controls.Add(this.zedgraphcontrol);
            this.splitContainerLoc.Size = new System.Drawing.Size(1081, 571);
            this.splitContainerLoc.SplitterDistance = 360;
            this.splitContainerLoc.TabIndex = 28;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(75, 419);
            this.trackBar1.Maximum = 255;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(615, 45);
            this.trackBar1.TabIndex = 29;
            this.trackBar1.Value = 80;
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(75, 486);
            this.trackBar2.Maximum = 255;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(615, 45);
            this.trackBar2.TabIndex = 80;
            this.trackBar2.Value = 200;
            // 
            // label1
            // 
            this.ulabel.AutoSize = true;
            this.ulabel.ForeColor = System.Drawing.Color.White;
            this.ulabel.Location = new System.Drawing.Point(95, 470);
            this.ulabel.Name = "label1";
            this.ulabel.Size = new System.Drawing.Size(60, 13);
            this.ulabel.TabIndex = 31;
            this.ulabel.Text = "Upper Limit";
            // 
            // label2
            // 
            this.llabel.AutoSize = true;
            this.llabel.ForeColor = System.Drawing.Color.White;
            this.llabel.Location = new System.Drawing.Point(99, 403);
            this.llabel.Name = "label2";
            this.llabel.Size = new System.Drawing.Size(56, 13);
            this.llabel.TabIndex = 32;
            this.llabel.Text = "Lower limit";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1091, 622);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainerLoc);
            this.Controls.Add(this.Finished);
            this.Name = "Form1";
            this.Text = "ContrastTool";
            ((System.ComponentModel.ISupportInitialize)(this.Preview)).EndInit();
            this.splitContainerLoc.Panel1.ResumeLayout(false);
            this.splitContainerLoc.Panel2.ResumeLayout(false);
            this.splitContainerLoc.Panel2.PerformLayout();
            this.splitContainerLoc.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            this.ResumeLayout(false);

        }
        public LUTTool()
            : base()
        {

            base.splitContainer1.Visible = false;
            button1.Visible = false;

            pInitializeComponent();

            Sliders.Add("Lower Limit", new ParamSlider("Lower Limit", trackBar1, llabel));
            Sliders.Add("Upper Limit", new ParamSlider("Upper Limit", trackBar2, ulabel));
            trackBar1.ValueChanged += new EventHandler(trackBar2_ValueChanged);
            trackBar2.ValueChanged += new EventHandler(trackBar2_ValueChanged);

            Finished.Click += new EventHandler(Finished_Click);
        }

        void Finished_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            object[] pars = new object[Sliders.Count];
            int i = 0;
            foreach (KeyValuePair<string, ParamSlider> kvp in Sliders)
            {
                double pVal = (double)kvp.Value.trackBar.Value / 255d * (MaxValue - MinValue) + MinValue;
                pars[i] = pVal;
                i++;
            }

            mDataEnvironment.MinContrast  = (double)pars[0];
            mDataEnvironment.MaxContrast = (double)pars[1];
           // mDataEnvironment.RedrawBuffers();
            mParameters = pars;
            DoRun();
        }

        public override string EffectName { get { return "LUT Adjustment"; } }
        public override string EffectMenu { get { return "Adjustment"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 1; } }

        DataHolder  mData;
       // double[,] mSlice;
        double MaxValue = 0;
        double MinValue = 0;

        public override string[] ParameterList
        {
            get { return new string[] { "mMinContrast|double", "mMaxContrast|double" }; }
        }
        public override object[] DefaultProperties
        {
            get { return new object[]{ .25, .75 }; }
        }
        protected override DataHolder doEffect(DataEnvironment3D dataEnvironment, DataHolder SourceImage, ImageViewer.Filters.ReplaceStringDictionary PassData, params object[] Parameters)
        {
            try
            {
                mDataEnvironment = dataEnvironment;
                mParameters = Parameters;
                if (mParameters == null)
                {
                    mParameters = DefaultProperties;
                    mParameters[0] = dataEnvironment.MinContrast;
                    mParameters[1] = dataEnvironment.MaxContrast;
                }
                else
                {
                    dataEnvironment.MinContrast = (double)mParameters[0];
                    dataEnvironment.MaxContrast = (double)mParameters[1];
                    dataEnvironment.RedrawBuffers();
                }

                mData = SourceImage;
                MaxValue = mDataEnvironment.MaxPossibleContrast;
                MinValue = mDataEnvironment.MinPossibleContrast;

                double[] Flattened = ClipImage(mData.Data);
                double[,] Histo = MakeHistogramIndexed(Flattened, 100);
                try
                {
                    CreateGraph(Histo);
                }
                catch { }
            }
            catch { }
            return SourceImage;
        }
        

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


        public static int[] MakeHistogram(double[] Intensities, int NumBins)
        {
            double max = Intensities.Max();// MaxArray(Intensities);
            double min = Intensities.Min();
            double step = (max - min) / (double)NumBins;
            int[] OutArray = new int[NumBins + 1];
            if (step != 0)
            {
                for (int i = 0; i < Intensities.Length; i++)
                {
                    int index = (int)Math.Truncate((Intensities[i] - min) / step);
                    OutArray[index]++;
                }
            }
            return OutArray;
        }

        public static double[,] MakeHistogramIndexed(double[] Intensities, int NumBins)
        {
            double max = Intensities.Max();// MaxArray(Intensities);
            double min = Intensities.Min();
            double step = (max - min) / (double)NumBins;
            if (step == 0) step = 1;
            double[,] OutArray = new double[2, NumBins + 1];
            for (int i = 0; i < Intensities.Length; i++)
            {
                int index = (int)Math.Truncate((Intensities[i] - min) / step);
                OutArray[1, index]++;
            }
            for (int i = 0; i < OutArray.GetLength(1); i++)
                OutArray[0, i] = min + step * i;
            return OutArray;
        }

        
        public static double[] ClipImage(Bitmap SourceImage, ImageViewer.ISelection Selection)
        {
            int iWidth = SourceImage.Width;
            int iHeight = SourceImage.Height;

            double[] ImageArray = new double[Selection.SelectionBounds.Width * Selection.SelectionBounds.Height];

            BitmapData bmd = SourceImage.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height), ImageLockMode.WriteOnly, SourceImage.PixelFormat);

            double g1, g2, g3;
            long t = 0;
            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = Selection.SelectionBounds.Top; y < Selection.SelectionBounds.Bottom; y += 2)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                        for (int x = Selection.SelectionBounds.Left; x < Selection.SelectionBounds.Right; x += 2)
                        {
                            if (Selection.PointInSelection(new Point(x, y)) == true)
                            {
                                byte* bits = (byte*)(scanline + x);
                                g1 = bits[0];
                                g2 = bits[1];
                                g3 = bits[2];

                                ImageArray[t] = (g1 + g2 + g3) / 3d;

                                t++;
                            }
                        }
                    }
                }
                else
                    throw new Exception("Does not support image formats other than 32 bits.  Please convert the image");
            }
            SourceImage.UnlockBits(bmd);
            if (t < ImageArray.Length)
            {
                double[] ActualLength = new double[t];
                Buffer.BlockCopy(ImageArray, 0, ActualLength, 0, (int)(t * sizeof(double)));
                return ActualLength;
            }
            else
                return ImageArray;
        }

        public static double[] ClipImage(double[, ,] Data)
        {
            int step = 20;
            double[] ImageArray = new double[(int)((double)Data.Length / (double)step)];

            long t = 0;
            long Length = Data.Length - step * 2;
            unsafe
            {
                fixed (double* pArray = Data)
                {
                    double* pIn = pArray;
                    for (int i = 0; i < Length; i += step)
                    {
                        ImageArray[t] = *pIn;
                        t++;
                        pIn += step;
                    }
                }
            }

            return ImageArray;
        }

        public static double[] ClipImage(float [, ,] Data)
        {
            int step = 20;
            double[] ImageArray = new double[(int)((double)Data.Length / (double)step)];

            long t = 0;
            long Length = Data.Length - step * 2;
            unsafe
            {
                fixed (float * pArray = Data)
                {
                    float * pIn = pArray;
                    for (int i = 0; i < Length; i += step)
                    {
                        ImageArray[t] = *pIn;
                        t++;
                        pIn += step;
                    }
                }
            }

            return ImageArray;
        }

        public static double[] ClipImage(double[,] Data)
        {
            int step = 20;
            double[] ImageArray = new double[(int)((double)Data.Length / (double)step)];

            long t = 0;
            long Length = Data.Length - step * 2;
            unsafe
            {
                fixed (double* pArray = Data)
                {
                    double* pIn = pArray;
                    for (int i = 0; i < Length; i += step)
                    {
                        ImageArray[t] = *pIn;
                        t++;
                        pIn += step;
                    }
                }
            }

            return ImageArray;
        }

        public static double[] ClipImage(double[,] Data, ImageViewer.ISelection Selection)
        {
            int iWidth = Data.GetLength(0);
            int iHeight = Data.GetLength(1);

            double[] ImageArray = new double[Selection.SelectionBounds.Width * Selection.SelectionBounds.Height];

            long t = 0;
            unsafe
            {
                for (int y = Selection.SelectionBounds.Top; y < Selection.SelectionBounds.Bottom; y += 2)
                {
                    for (int x = Selection.SelectionBounds.Left; x < Selection.SelectionBounds.Right; x += 2)
                    {
                        if (x >= 0 && y >= 0)
                        {
                            if (x < Data.GetLength(0) && y < Data.GetLength(1))
                            {
                                if (Selection.PointInSelection(new Point(x, y)) == true)
                                {
                                    ImageArray[t] = Data[x, y];
                                    t++;
                                }
                            }
                        }
                    }
                }
            }
            if (t < ImageArray.Length)
            {
                double[] ActualLength = new double[t];
                Buffer.BlockCopy(ImageArray, 0, ActualLength, 0, (int)(t * sizeof(double)));
                return ActualLength;
            }
            else
                return ImageArray;
        }

        private void GraphBar(double[,] Data, string xAxisName, string YAxisName)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = this.zedgraphcontrol.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = xAxisName;
            myPane.YAxis.Title.Text = YAxisName;

            // Make up some mDataDouble points based on the Sine function
            PointPairList list = new PointPairList();
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                double x = Data[0, i];
                double y = Data[1, i];
                list.Add(x, y);
            }

            myPane.CurveList.Clear();

            // create the curves
            BarItem myCurve = myPane.AddBar("", list, Color.Red);

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;

            // Make the Y axis scale red
            myPane.YAxis.Scale.FontSpec.FontColor = Color.Red;
            myPane.YAxis.Title.FontSpec.FontColor = Color.Red;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside;

            // Enable scrollbars if needed
            this.zedgraphcontrol.IsShowHScrollBar = true;
            this.zedgraphcontrol.IsShowVScrollBar = true;
            this.zedgraphcontrol.IsAutoScrollRange = true;


            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            this.zedgraphcontrol.AxisChange();
            this.zedgraphcontrol.Invalidate();
            // Make sure the Graph gets redrawn
            this.Invalidate();
            this.Refresh();
        }

        private Bitmap MakeBitmap(double[,] ImageArray, double MaxIntensity, double MinIntensity)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = MaxIntensity;
            double iMin = MinIntensity;

            double iLength = iMax - iMin;
            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);
            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y] - iMin) / iLength);
                        if (g > 255) g = 255;
                        if (g < 0) g = 0;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }
       
    }
}
