using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using MathHelpLib._3DStuff;


namespace MathHelpLib.DrawingAndGraphing
{
    public partial class Graph3D : UserControl, IGraphControl
    {
        public bool AdjustableImage
        {
            get { return false; }
        }
        public Graph3D()
        {
            InitializeComponent();
        }
        protected OgreWindow mogreWin;
        private Graph3DThreshold lastThresholder = null;
        System.Threading.Timer timer1;
        System.Threading.Timer timer2;
        double[, ,] NormData;
        public void SetData(double[, ,] Data)
        {
            if (mogreWin == null)
                StartGraph();

            MarchingCubes cubes = new MarchingCubes();
            cubes.CreateSurface(Data, 1, (int)(Data.GetLength(0) / 4d), (int)(Data.GetLength(1) / 4d), (int)(Data.GetLength(2) / 4d));
            cubes.RotateY = Math.PI / 2d;
            cubes.CenterAndNormalizePoints(25);

            try
            {
                // ShowMesh("Sphere1", cubes.VertexList, cubes.TriangleIndexs, cubes.CreateVertexNormals());
            }
            catch
            {

            }
            mogreWin.CreateCloud(Data.GetLength(0), Data.GetLength(1), Data.GetLength(2), 175f, 64);
            NormData = Data.DivideToArray(Data.MaxArray());


            ThresholdList.Add(new Thresholds(.66, .1, 0, 1, TopHatSelector, Color.Blue));

            graph3DThreshold1.SetThresholder(ThresholdList[0]);
            lastThresholder = graph3DThreshold1;

            mogreWin.GenerateCloud(NormData);
            mogreWin.Paint();

            timer1 = new System.Threading.Timer(new System.Threading.TimerCallback(timer1_Tick), new object(), 300, 300);
            timer2 = new System.Threading.Timer(new System.Threading.TimerCallback(timer2_Tick), new object(), 300, 300);
        }

        public void SetData(PhysicalArray PhysArray)
        {
            MarchingCubes cubes = new MarchingCubes();
            double[, ,] Data = (double[, ,])PhysArray.ActualData3D;
            SetData(Data);
        }

        public void SetData(double[,] ValueArray)
        {
            throw new Exception("2D ImageData cannot be displayed in volume graph");
        }

        public void SetData(List<double[,]> array)
        {
            throw new Exception("2D ImageData cannot be displayed in volume graph");
        }

        public void SetData(PhysicalArray[] PhysArrays)
        {
            throw new Exception("2D ImageData cannot be displayed in volume graph");
        }

        public void StartGraph()
        {
            mogreWin = new OgreWindow(new Point(100, 30), pictureBox1.Handle);
            mogreWin.InitMogre();
        }

        public void ShowMesh(string name, Point3D[] Vertexs, int[] TriangleIndices, Point3D[] Normals)
        {
            if (mogreWin == null)
                StartGraph();

            mogreWin.CreateMesh(name, Vertexs, TriangleIndices, Normals);
            mogreWin.Paint();
        }

        MathGraph mParent;
        public void SetParentControl(MathGraph Parent)
        {
            mParent = Parent;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (mogreWin != null)
                mogreWin.Paint();
        }

        private void Graph3D_Paint(object sender, PaintEventArgs e)
        {
            if (mogreWin != null)
                mogreWin.Paint();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (mogreWin != null)
                mogreWin.Paint();
            //base.OnPaint(e);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        #region Thresholder

        List<Thresholds> ThresholdList = new List<Thresholds>();
        /// <summary>
        /// Assumes that the thresholdlist has already been sorted and that the covered regions do not overlap
        /// </summary>
        private void DrawThresholder()
        {
            ThresholdList.Sort();
            Rectangle fillrect;
            Bitmap ScreenB = new Bitmap(TopHatSelector.Width, TopHatSelector.Height, PixelFormat.Format32bppArgb);
            int HalfHeight = ScreenB.Height / 2;
            Graphics G = Graphics.FromImage(ScreenB);
            G.Clear(Color.Black);
            if (ThresholdList.Count > 0)
            {
                int i = 0;
                //get the first edge
                G.FillRectangle(Brushes.Gray, new Rectangle(0, 0, ThresholdList[0].LeftScreenEdge, ScreenB.Height));
                //now fill in the middle
                for (i = 1; i < ThresholdList.Count; i++)
                {
                    fillrect = new Rectangle(ThresholdList[i - 1].RightScreenEdge, 0, ThresholdList[i].LeftScreenEdge - ThresholdList[i - 1].RightScreenEdge, ScreenB.Height);
                    if (fillrect.Width > 0)
                        G.FillRectangle(Brushes.Gray, fillrect);
                }
                //and the the right edge
                fillrect = new Rectangle(ThresholdList[i - 1].RightScreenEdge, 0, ScreenB.Width - ThresholdList[i - 1].RightScreenEdge, ScreenB.Height);
                if (fillrect.Width > 0)
                    G.FillRectangle(Brushes.Gray, fillrect);

                for (i = 0; i < ThresholdList.Count; i++)
                {
                    G.FillEllipse(Brushes.Yellow, new Rectangle(ThresholdList[i].LeftScreenEdge - 3, HalfHeight - 3, 6, 6));
                    G.FillEllipse(Brushes.Red, new Rectangle(ThresholdList[i].MiddleScreen - 3, HalfHeight - 3, 6, 6));
                    G.FillEllipse(Brushes.Yellow, new Rectangle(ThresholdList[i].RightScreenEdge - 3, HalfHeight - 3, 6, 6));
                }

            }
            TopHatSelector.Image = ScreenB;
            TopHatSelector.Invalidate();
        }

        Thresholds mThresholdsAbove = null;
        Thresholds mThresholdsBelow = null;
        Thresholds mSelectedThreshold = null;
        bool mWidthSelector = false;
        double mMiddleValue;
        private void TopHatSelector_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < ThresholdList.Count; i++)
            {
                if (Math.Abs(ThresholdList[i].LeftScreenEdge - e.X) < 4 || Math.Abs(ThresholdList[i].RightScreenEdge - e.X) < 4)
                {
                    mSelectedThreshold = ThresholdList[i];

                    if (i > 0)
                        mThresholdsBelow = ThresholdList[i - 1];
                    if (i < ThresholdList.Count - 1)
                        mThresholdsAbove = ThresholdList[i + 1];

                    mMiddleValue = mSelectedThreshold.Middle;
                    mWidthSelector = true;
                    break;
                }
                else if (Math.Abs(ThresholdList[i].MiddleScreen - e.X) < 4)
                {
                    mSelectedThreshold = ThresholdList[i];
                    mMiddleValue = mSelectedThreshold.MiddleScreen;
                    mWidthSelector = false;
                    break;
                }

            }

        }

        private bool Waiting = false;
        private bool NewValueAvail = false;
        private void TopHatSelector_MouseMove(object sender, MouseEventArgs e)
        {

            if (mSelectedThreshold != null && e.Button == MouseButtons.Left)
            {
                if (mWidthSelector == true)
                {
                    double nWidth = Math.Abs(mSelectedThreshold.ScreenToIntensity(e.X) - mMiddleValue);
                    mSelectedThreshold.HalfWidth = nWidth;
                }
                else
                {
                    double newMiddle = mSelectedThreshold.ScreenToIntensity(e.X);
                    mSelectedThreshold.Middle = newMiddle;
                }
                DrawThresholder();
                NewValueAvail = true;
            }
        }

        private void timer2_Tick(object state)
        {
            if (Waiting == false && NewValueAvail ==true )
            {
                Waiting = true;
                NewValueAvail = false;
                mogreWin.GenerateCloudUnthreaded (NormData, ThresholdList);
                Waiting = false;
            }
        }

        private void TopHatSelector_MouseUp(object sender, MouseEventArgs e)
        {
            mThresholdsAbove = null;
            mThresholdsBelow = null;
            mSelectedThreshold = null;
        }

        private void cThreshold_CheckedChanged(object sender, EventArgs e)
        {
            if (cThreshold.Checked)
            {
                DrawThresholder();
                mogreWin.GenerateCloud(NormData, ThresholdList);
               
            }
        }

        private delegate void paintRequest();
        private void timer1_Tick(object State)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new paintRequest(PaintRequested));
            }
            else 
            {
                PaintRequested();
            }
        }
        private void PaintRequested()
        {
            if (mogreWin != null)
                mogreWin.Paint();
         
        }
        #endregion

        private void bAddThresholder_Click(object sender, EventArgs e)
        {
            Thresholds t = new Thresholds(.5, .1, 0, 1, TopHatSelector, Color.Blue);
            ThresholdList.Add(t);

            Graph3DThreshold graph3DThreshold1 = new Graph3DThreshold(t);
            graph3DThreshold1.SetThresholder(t);
            graph3DThreshold1.Location = new System.Drawing.Point(3, lastThresholder.Top + lastThresholder.Height + 5);
            graph3DThreshold1.Name = "graph3DThreshold1";
            graph3DThreshold1.Size = new System.Drawing.Size(237, 125);
            graph3DThreshold1.TabIndex = 0;
            graph3DThreshold1.ColorClicked += new EventHandler(graph3DThreshold1_ColorClicked);
            this.panel2.Controls.Add(graph3DThreshold1);
            lastThresholder = graph3DThreshold1;
            panel2.Height = lastThresholder.Top + lastThresholder.Height + 5;
            DrawThresholder();
            if (mogreWin != null)
                mogreWin.GenerateCloud(NormData, ThresholdList);

        }

        private void graph3DThreshold1_ColorClicked(object sender, EventArgs e)
        {
            if (mogreWin != null)
                mogreWin.GenerateCloud(NormData, ThresholdList);
        }

        private void TopHatSelector_Paint(object sender, PaintEventArgs e)
        {
            //DrawThresholder();
        }




    }
}