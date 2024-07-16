using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using MathHelpLib;
using MathHelpLib.DrawingAndGraphing;

namespace GraphingLib.DrawingAndGraphing
{
    public partial class Graph1D : UserControl ,IGraphControl 
    {
        public bool AdjustableImage
        {
            get { return false; }
        }
        public Graph1D()
        {
            InitializeComponent();
        }

        /// <summary>
        /// standard list of colors for making graph lines
        /// </summary>
        private  static Color[] Colors = { Color.Red ,Color.Blue,Color.Black,Color.Green,Color.Yellow,Color.Brown,Color.BlueViolet ,Color.Purple,Color.MintCream };

        #region AcceptData
        /// <summary>
        /// clears all data from the graph
        /// </summary>
        public void ClearData()
        {
            GraphPane myPane=this.zedgraphcontrol .GraphPane ;
            myPane.CurveList.Clear();
        }

        /// <summary>
        /// Pulls the data from a 1D physical array and graphs it with the axis names in the physical array
        /// </summary>
        /// <param name="PhysArray"></param>
        public void SetData(PhysicalArray PhysArray)
        {
           double[,] Graph= PhysArray.MakeGraphableArray();
           GraphData(PhysArray.ArrayInformation.SuggestedGraphingHint , Graph, PhysArray.ArrayInformation.AxisName(GraphAxis.XAxis), PhysArray.ArrayInformation.AxisName(GraphAxis.ValueAxis));
        }

        /// <summary>
        /// Pulls the data from a linear array and graphs it with no axis names
        /// </summary>
        /// <param name="array"></param>
        public void SetData(double[] array)
        {
            double[,] Graph = array.MakeGraphableArray(0,1);
            GraphData(MathGraphTypes.Unknown , Graph,"","");
        }

        /// <summary>
        /// Pulls the data from a linear array and graphs it with no axis names
        /// </summary>
        /// <param name="array"></param>
        public void SetData(int[] array)
        {
            double[,] Graph = array.MakeGraphableArray(0, 1);
            GraphData(MathGraphTypes.Unknown, Graph, "", "");
        }

        /// <summary>
        /// Pulls the data from a linear array and graphs it with no axis names
        /// </summary>
        /// <param name="array"></param>
        public void SetData(double[,] array)
        {
            GraphData(MathGraphTypes.Unknown, array, "", "");
        }

        /// <summary>
        /// Pulls the data from a list of  linear arrays and graphs each line
        /// </summary>
        /// <param name="array"></param>
        public void SetData(List<double[,]> array)
        {
            GraphData(MathGraphTypes.Unknown, array[0], "", "");
            for (int i = 1; i < array.Count; i++)
                GraphExtraData(MathGraphTypes.Unknown, array[i], "", "");
        }

        /// <summary>
        /// Graphs the 1D data using the selected graph type and the given names
        /// </summary>
        /// <param name="GraphType">this can be MathGraphTypes.Graph1D_Line, MathGraphTypes.Graph1D_Scatter, MathGraphTypes.Graph1D_Bar</param>
        /// <param name="Data">1D data, x in first col, y in second col</param>
        /// <param name="XAxisName"></param>
        /// <param name="YAxisName"></param>
        public void SetData(MathGraphTypes GraphType, double[,] Data,string XAxisName,string YAxisName)
        {
            GraphData(GraphType, Data, XAxisName, YAxisName);
        }

        /// <summary>
        /// Graphs multiple 1D data using the selected graph type and the given names
        /// </summary>
        /// <param name="GraphType">this can be MathGraphTypes.Graph1D_Line, MathGraphTypes.Graph1D_Scatter, MathGraphTypes.Graph1D_Bar</param>
        /// <param name="Data">lsit of 1D data, x in first col, y in second col</param>
        /// <param name="XAxisName"></param>
        /// <param name="YAxisName"></param>
        public void SetData(MathGraphTypes GraphType, List< double[,]> Data, string XAxisName, string YAxisName)
        {
            GraphData(GraphType, Data[0], XAxisName, YAxisName);
            for (int i = 1; i < Data.Count; i++)
                GraphExtraData(GraphType, Data[i], XAxisName, YAxisName);
        }

       /// <summary>
       /// graphs a list of physical arrays, each with its own color
       /// </summary>
       /// <param name="PhysArrays"></param>
        public void SetData(PhysicalArray[] PhysArrays)
        {
            double[,] Graph = PhysArrays[0].MakeGraphableArray();
            GraphData(PhysArrays[0].ArrayInformation.SuggestedGraphingHint, Graph, PhysArrays[0].ArrayInformation.AxisName(GraphAxis.XAxis), PhysArrays[0].ArrayInformation.AxisName(GraphAxis.ValueAxis));
            for (int i = 1; i < PhysArrays.Length; i++)
            {
                Graph = PhysArrays[i].MakeGraphableArray();
                GraphExtraData(PhysArrays[i].ArrayInformation.SuggestedGraphingHint, Graph, PhysArrays[i].ArrayInformation.AxisName(GraphAxis.XAxis), PhysArrays[i].ArrayInformation.AxisName(GraphAxis.ValueAxis));
            }
        }

        /// <summary>
        /// not supported with this graph type
        /// </summary>
        /// <param name="ValueArray"></param>
        public void SetData(double[, ,] ValueArray)
        {
            throw new Exception("Datatype not supported");
        }
        #endregion

        #region Do Graphing
        /// <summary>
        /// General purpose graphing routing.  Clears graph before starting
        /// </summary>
        /// <param name="GraphType">this can be MathGraphTypes.Graph1D_Line, MathGraphTypes.Graph1D_Scatter, MathGraphTypes.Graph1D_Bar</param>
        /// <param name="Data"></param>
        /// <param name="XAxisname"></param>
        /// <param name="YAxisname"></param>
        public void GraphData(MathGraphTypes GraphType, double[,] Data, string XAxisname, string YAxisname)
        {
            try
            {
                switch (GraphType)
                {
                    case MathGraphTypes.Unknown:
                    case MathGraphTypes.Graph1D_Line:
                        GraphLine( this.zedgraphcontrol  , Data, XAxisname, YAxisname);
                        break;
                    case MathGraphTypes.Graph1D_Scatter:
                        GraphScatter(this.zedgraphcontrol, Data, XAxisname, YAxisname);
                        break;
                    case MathGraphTypes.Graph1D_Bar:
                        GraphBar(this.zedgraphcontrol, Data, XAxisname, YAxisname);
                        break;
                    default:
                        GraphLine(this.zedgraphcontrol, Data, XAxisname, YAxisname);
                        break;
                }
            }
            catch { }
        }

        /// <summary>
        /// General purpose graphing routing.  Adds another line to the graph, without removing the others
        /// </summary>
        /// <param name="GraphType">this can be MathGraphTypes.Graph1D_Line, MathGraphTypes.Graph1D_Scatter, MathGraphTypes.Graph1D_Bar</param>
        /// <param name="Data"></param>
        /// <param name="XAxisname"></param>
        /// <param name="YAxisname"></param>
        public void GraphExtraData(MathGraphTypes GraphType, double[,] Data, string XAxisname, string YAxisname)
        {
            switch (GraphType)
            {
                case MathGraphTypes.Unknown:
                case MathGraphTypes.Graph1D_Line:
                    GraphExtraLine(this.zedgraphcontrol, Data);
                    break;
                case MathGraphTypes.Graph1D_Scatter:
                    GraphExtraScatter(this.zedgraphcontrol, Data);
                    break;
                case MathGraphTypes.Graph1D_Bar:
                    GraphExtraBar(this.zedgraphcontrol, Data);
                    break;

                default:
                    GraphLine(this.zedgraphcontrol, Data, XAxisname, YAxisname);
                    break;
            }
        }

        #region zedgraph routings
        /// <summary>
        /// zedgraph routing to add data to a graph
        /// </summary>
        /// <param name="ZedGraph"></param>
        /// <param name="Data"></param>
        /// <param name="xAxisName"></param>
        /// <param name="YAxisName"></param>
        public static void GraphLine(ZedGraphControl ZedGraph, double[,] Data, string xAxisName, string YAxisName)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = ZedGraph.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = xAxisName +"";
            myPane.YAxis.Title.Text = YAxisName +"";

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
            ZedGraph.IsShowHScrollBar = true;
            ZedGraph.IsShowVScrollBar = true;
            ZedGraph.IsAutoScrollRange = true;


            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            try
            {
                ZedGraph.AxisChange();
            }
            catch { }
            ZedGraph.Invalidate();
            
        }
        /// <summary>
        /// zedgraph routing to add data to a graph
        /// </summary>
        /// <param name="ZedGraph"></param>
        /// <param name="Data"></param>
        /// <param name="xAxisName"></param>
        /// <param name="YAxisName"></param>
        public static void GraphExtraLine(ZedGraphControl ZedGraph, double[,] Data)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = ZedGraph.GraphPane;

            // Make up some mDataDouble points based on the Sine function
            PointPairList list = new PointPairList();
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                double x = Data[0, i];
                double y = Data[1, i];
                list.Add(x, y);
            }

            // Generate offsetX red curve with diamond symbols, and "Alpha" in the legend
            LineItem myCurve = myPane.AddCurve("Alpha",
                list, Colors[ myPane.CurveList.Count % Colors.Length ] , SymbolType.None);
            // Fill the symbols with white
            myCurve.Symbol.Fill = new Fill(Color.White);

            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            ZedGraph.AxisChange();
            ZedGraph.Invalidate();
            // Make sure the Graph gets redrawn
           
        }
        /// <summary>
        /// zedgraph routing to add data to a graph
        /// </summary>
        /// <param name="ZedGraph"></param>
        /// <param name="Data"></param>
        /// <param name="xAxisName"></param>
        /// <param name="YAxisName"></param>
        public static void GraphScatter(ZedGraphControl ZedGraph, double[,] Data, string xAxisName, string YAxisName)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = ZedGraph.GraphPane;

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

            LineItem myCurve = myPane.AddCurve("", list, Color.Red, SymbolType.Diamond);
            // Don't display the line (This makes a scatter plot)
            myCurve.Line.IsVisible = false;
            // Hide the symbol outline
            myCurve.Symbol.Border.IsVisible = false;
            // Fill the symbol interior with color
            myCurve.Symbol.Fill = new Fill(Color.Red);



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
            ZedGraph.IsShowHScrollBar = true;
            ZedGraph.IsShowVScrollBar = true;
            ZedGraph.IsAutoScrollRange = true;


            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            ZedGraph.AxisChange();
            ZedGraph.Invalidate();
           
        }
        /// <summary>
        /// zedgraph routing to add data to a graph
        /// </summary>
        /// <param name="ZedGraph"></param>
        /// <param name="Data"></param>
        /// <param name="xAxisName"></param>
        /// <param name="YAxisName"></param>
        public static void GraphExtraScatter(ZedGraphControl ZedGraph, double[,] Data)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = ZedGraph.GraphPane;

            // Make up some mDataDouble points based on the Sine function
            PointPairList list = new PointPairList();
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                double x = Data[0, i];
                double y = Data[1, i];
                list.Add(x, y);
            }

            Color ScatterColor = Colors[myPane.CurveList.Count % Colors.Length];
            // Generate offsetX red curve with diamond symbols, and "Alpha" in the legend
            LineItem myCurve = myPane.AddCurve("", list, ScatterColor , SymbolType.Circle );

            myCurve.Symbol.Size = 2;
            // Don't display the line (This makes a scatter plot)
            myCurve.Line.IsVisible = false;
            // Hide the symbol outline
            myCurve.Symbol.Border.IsVisible = false;
            // Fill the symbol interior with color
            myCurve.Symbol.Fill = new Fill(ScatterColor );

            
            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            ZedGraph.AxisChange();
            ZedGraph.Invalidate();
            // Make sure the Graph gets redrawn
          
        }
        /// <summary>
        /// zedgraph routing to add data to a graph
        /// </summary>
        /// <param name="ZedGraph"></param>
        /// <param name="Data"></param>
        /// <param name="xAxisName"></param>
        /// <param name="YAxisName"></param>
        public static void GraphBar(ZedGraphControl ZedGraph, double[,] Data, string xAxisName, string YAxisName)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = ZedGraph.GraphPane;

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
            ZedGraph.IsShowHScrollBar = true;
            ZedGraph.IsShowVScrollBar = true;
            ZedGraph.IsAutoScrollRange = true;


            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            ZedGraph.AxisChange();
            ZedGraph.Invalidate();
           
        }
        /// <summary>
        /// zedgraph routing to add data to a graph
        /// </summary>
        /// <param name="ZedGraph"></param>
        /// <param name="Data"></param>
        /// <param name="xAxisName"></param>
        /// <param name="YAxisName"></param>
        public static void GraphExtraBar(ZedGraphControl ZedGraph, double[,] Data)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = ZedGraph.GraphPane;

            // Make up some mDataDouble points based on the Sine function
            PointPairList list = new PointPairList();
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                double x = Data[0, i];
                double y = Data[1, i];
                list.Add(x, y);
            }

            Color ScatterColor = Colors[myPane.CurveList.Count % Colors.Length];
            // create the curves
            BarItem myCurve = myPane.AddBar("", list, ScatterColor );


            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            ZedGraph.AxisChange();
            ZedGraph.Invalidate();
           
        }
        #endregion
        #endregion

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