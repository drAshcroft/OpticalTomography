using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace MathHelpLib.DrawingAndGraphing
{
    public partial class Graph3DSliceViewerMultiAngle : UserControl, IGraphControl
    {
        double[, ,] mGraphData;
        double SliceXPercent;
        double SliceYPercent;
        double SliceZPercent;
        double maxContrast = 0;
        double minContrast = 0;
        double tmaxContrast = 0;
        double tminContrast = 0;

        public bool AdjustableImage
        {
            get { return true ; }
        }

        public Graph3DSliceViewerMultiAngle()
        {
            InitializeComponent();
            SliceXPercent = .5;
            SliceYPercent = .5;
            SliceZPercent = .5;
        }

        public void SetData(PhysicalArray PhysArray)
        {
            SetGraphData((double[, ,])PhysArray.ReferenceDataDouble);
        }

        public void SetGraphData(double[, ,] GraphData)
        {
           
            mGraphData = GraphData;
            maxContrast = GraphData.MaxArray();
            minContrast = GraphData.MinArray();
            double MidContrast, stdev;
           // MidContrast = GraphData.AverageArray();
            ContrastArray(GraphData, out MidContrast, out stdev);
            tmaxContrast = MidContrast + stdev;
            tminContrast = MidContrast - stdev;

            ShowSlice();
        }

        public unsafe void ContrastArray(double[, ,] array,out double Average, out double StDev)
        {
            double sum = 0;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i+=5)
                {
                    sum += *pAOut;
                    pAOut++;
                }
                double ave=sum / (double)array.Length*5;
                double x ;
                sum=0;
                for (int i = 1; i < array.Length; i+=5)
                {
                    x= (*pAOut-ave);
                    sum += x*x;
                    pAOut++;
                }
                Average=ave;
                StDev = Math.Sqrt( sum  / array.Length*5); 
            }
            
        }

        public void SetData(double[,] ValueArray)
        {
            throw new Exception("2D Data cannot be displayed in volume graph");
        }
        public void SetData(List<double[,]> array)
        {
            throw new Exception("2D Data cannot be displayed in volume graph");
        }
        public void SetData(PhysicalArray[] PhysArrays)
        {
            throw new Exception("2D Data cannot be displayed in volume graph");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SlicePercent">A Value Between 0 and 100</param>
        public void ShowSlice()
        {
            if (SliceXPercent < 0)
                SliceXPercent = 0;
            if (SliceXPercent > 1)
                SliceXPercent = 1;
            if (SliceYPercent < 0)
                SliceYPercent = 0;
            if (SliceYPercent > 1)
                SliceYPercent = 1;
            if (SliceZPercent < 0)
                SliceZPercent = 0;
            if (SliceZPercent > 1)
                SliceZPercent = 1;

            Bitmap bZ = mGraphData.MakeBitmap(Axis.ZAxis, (int)((double)(mGraphData.GetLength(2)-1) * SliceZPercent), tminContrast, tmaxContrast);
            pictureBox1.Image = bZ;

            Bitmap bX = mGraphData.MakeBitmap(Axis.XAxis, (int)((double)(mGraphData.GetLength(0)-1) * SliceXPercent), tminContrast, tmaxContrast);
            pictureBox2.Image = bX;

            Bitmap  bY = mGraphData.MakeBitmap(Axis.YAxis, (int)((double)(mGraphData.GetLength(1)-1) * SliceYPercent), tminContrast, tmaxContrast);
            pictureBox3.Image = bY;

            //b = mGraphData.MakeBitmap(Axis.ZAxis, (int)((double)mGraphData.GetLength(2) * SliceZPercent));
            //pictureBox4.Image = b;
            pseudoCube1.ImageX = bX;
            pseudoCube1.ImageY=bY;
            pseudoCube1.ImageZ = bZ;


            pictureBox1.Invalidate();
            pictureBox2.Invalidate();
            pictureBox3.Invalidate();
            pseudoCube1.Invalidate();

        }

        #region Events

        private void Graph3DSliceViewerMultiAngle_Resize(object sender, EventArgs e)
        {
            int HWidth = this.Width / 2;
            int HHeight = this.Height / 2;

            pictureBox1.Top = 0;
            pictureBox1.Left = 0;
            pictureBox1.Height = HHeight;
            pictureBox1.Width = HWidth;

            pictureBox2.Top = 0;
            pictureBox2.Left = HWidth;
            pictureBox2.Height = HHeight;
            pictureBox2.Width = HWidth;

            pictureBox3.Top = HHeight;
            pictureBox3.Left = 0;
            pictureBox3.Height = HHeight;
            pictureBox3.Width = HWidth;

            pictureBox4.Top = HHeight;
            pictureBox4.Left = HWidth;
            pictureBox4.Height = HHeight;
            pictureBox4.Width = HWidth;


        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                SliceXPercent = e.X / (double)pictureBox1.Width;
                SliceYPercent = e.Y / (double)pictureBox1.Height;
                ShowSlice();
            }
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                SliceZPercent = 1 - e.X / (double)pictureBox1.Width;
                SliceYPercent = 1 - e.Y / (double)pictureBox1.Height;
                ShowSlice();
            }
        }

        private void pictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                SliceXPercent = e.X / (double)pictureBox1.Width;
                SliceZPercent = e.Y / (double)pictureBox1.Height;
                ShowSlice();
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            double MidContrast = (maxContrast + minContrast) / 2;
            
            //mMinContrast = minContrast  + Math.Abs((MidContrast - minContrast)) * (double)trackBar1.Value / 100d;
            ShowSlice();
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            double MidContrast = (maxContrast + minContrast) / 2;

            //mMaxContrast = maxContrast + Math.Abs((MidContrast - maxContrast)) * (double)trackBar2.Value / 100d;
            ShowSlice();

        }

        private void pseudoCube1_XAxisMoved(double Percent)
        {
            SliceZPercent = Percent;
            ShowSlice();
        }

        private void pseudoCube1_YAxisMoved(double Percent)
        {
            SliceYPercent = Percent;
            ShowSlice();
        }

        private void pseudoCube1_ZAxisMoved(double Percent)
        {
            SliceXPercent = Percent;
            ShowSlice();
            
        }
        #endregion


        MathGraph mParent;
        public void SetParentControl(MathGraph Parent)
        {
            mParent = Parent;
        }
    }

}