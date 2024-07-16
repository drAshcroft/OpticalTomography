using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace MathHelpLib
{
    public partial class Graph1DZ : ZedGraphControl ,IGraphControl 
    {
        public Graph1DZ()
        {
            InitializeComponent();
        }
        public void ClearData()
        {
            GraphPane myPane=this.GraphPane ;
            myPane.CurveList.Clear();
        }
        public void SetData(PhysicalArray PhysArray)
        {
           double[,] Graph= PhysArray.MakeGraphableArray();
           GraphLine(Graph);
        }

        public void SetData(double[] array)
        {
            double[,] Graph = array.MakeGraphableArray(0,1);
            GraphLine(Graph);
        }

        
        public void GraphLine(double[,] Data)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = this.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "Distance";
            myPane.YAxis.Title.Text = "Absorption";

            // Make up some mDataDouble points based on the Sine function
            PointPairList list = new PointPairList();
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                double x = Data[0, i];
                double y = Data[1, i];
                list.Add(x, y);
            }

            myPane.CurveList.Clear();

            // Generate offsetX red curve with diamond symbols, and "Alpha" in the legend
            LineItem myCurve = myPane.AddCurve("Alpha",
                list, Color.Red, SymbolType.None);
            // Fill the symbols with white
            myCurve.Symbol.Fill = new Fill(Color.White);

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
            this.IsShowHScrollBar = true;
            this.IsShowVScrollBar = true;
            this.IsAutoScrollRange = true;


            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            this.AxisChange();
            // Make sure the Graph gets redrawn
            this.Invalidate();
            this.Refresh();
        }

    }

}