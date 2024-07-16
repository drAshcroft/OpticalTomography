using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathHelpLib.DrawingAndGraphing;
using MathHelpLib.ImageProcessing;
using MathHelpLib;

namespace GraphingLib.DrawingAndGraphing
{
    public partial class Graph3DSliceViewer : UserControl,IGraphControl 
    {
        double[, ,] mGraphData;

        /// <summary>
        /// Doesnt seem to do anything
        /// </summary>
        public bool AdjustableImage
        {
            get { return true ; }
        }


        public Graph3DSliceViewer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// takes a 3D physical array and creates a copy of the 3D data within for use in the slice viewer
        /// </summary>
        /// <param name="PhysArray"></param>
        public void SetData(PhysicalArray PhysArray)
        {
            SetData((double[,,])PhysArray.ReferenceDataDouble );
        }

        /// <summary>
        /// Pulls the 3D data, does not make a copy, but keeps a reference to the 3D data
        /// </summary>
        /// <param name="GraphData"></param>
        public void SetData(double[, ,] GraphData)
        {
            mGraphData = GraphData;
            ShowSlice(50);
        }

        /// <summary>
        /// not allowed with this graph type
        /// </summary>
        /// <param name="ValueArray"></param>
        public void SetData(double[,] ValueArray)
        {
            throw new Exception("2D ImageData cannot be displayed in volume graph");
        }
        /// <summary>
        /// not allowed with this graph type
        /// </summary>
        /// <param name="ValueArray"></param>
        public void SetData(List<double[,]> array)
        {
            throw new Exception("2D ImageData cannot be displayed in volume graph");
        }
        /// <summary>
        /// not allowed with this graph type
        /// </summary>
        /// <param name="ValueArray"></param>
        public void SetData(PhysicalArray[] PhysArrays)
        {
            throw new Exception("2D ImageData cannot be displayed in volume graph");
        }

        /// <summary>
        /// Pulls a slice from the 3D volume to show on the screen
        /// </summary>
        /// <param name="SlicePercent">A Value Between 0 and 100</param>
        public void ShowSlice(int SlicePercent)
        {
            Bitmap b = mGraphData.MakeBitmap((int)((double)mGraphData.GetLength(2) * (double)SlicePercent / 100d));
            pictureBox1.Image = b;
            pictureBox1.Invalidate();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            ShowSlice(trackBar1.Value);
        }

        MathGraph mParent;
        /// <summary>
        /// Used to signal a graph change to the parent control.  Used if the user wants to use a different graph.
        /// </summary>
        /// <param name="Parent"></param>
        public void SetParentControl(MathGraph Parent)
        {
            mParent = Parent;
        }
    }

}