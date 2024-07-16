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
using MathHelpLib;

namespace ImageViewer.Filters.Analysis
{
    public partial class HistogramTool : aEffectForm  
    {
        private ZedGraph.ZedGraphControl zedgraphcontrol;
        private Button Finished;
        public HistogramTool():base()
        {
            splitContainer1.Visible = false;
            button1.Visible = false;

            this.components = new System.ComponentModel.Container();
            this.Finished = new System.Windows.Forms.Button();
            this.zedgraphcontrol = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // Finished
            // 
            this.Finished.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Finished.Location = new System.Drawing.Point(673, 430);
            this.Finished.Name = "Finished";
            this.Finished.Size = new System.Drawing.Size(67, 35);
            this.Finished.TabIndex = 25;
            this.Finished.Text = "Done";
            this.Finished.UseVisualStyleBackColor = true;
            this.Finished.Click += new System.EventHandler(this.button1_Click);
            // 
            // zedgraphcontrol
            // 
            this.zedgraphcontrol.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zedgraphcontrol.Location = new System.Drawing.Point(8, 8);
            this.zedgraphcontrol.Name = "zedgraphcontrol";
            this.zedgraphcontrol.ScrollGrace = 0;
            this.zedgraphcontrol.ScrollMaxX = 0;
            this.zedgraphcontrol.ScrollMaxY = 0;
            this.zedgraphcontrol.ScrollMaxY2 = 0;
            this.zedgraphcontrol.ScrollMinX = 0;
            this.zedgraphcontrol.ScrollMinY = 0;
            this.zedgraphcontrol.ScrollMinY2 = 0;
            this.zedgraphcontrol.Size = new System.Drawing.Size(732, 416);
            this.zedgraphcontrol.TabIndex = 26;
            // 
            // ContrastTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(749, 474);
            this.ControlBox = false;
            this.Controls.Add(this.zedgraphcontrol);
            this.Controls.Add(this.Finished);
            this.Name = "ContrastTool";
            this.Text = "ContrastTool";
            this.ResumeLayout(false);
           

        }
        public override string EffectName { get { return "Histogram Tool"; } }
        public override string EffectMenu { get { return "Analysis"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 1; } }

        public override object[] DefaultProperties
        {
            get { return new object[] { null }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }
        protected override object doEffect(ImageViewer.DataEnvironment dataEnvironment, object SourceImage, ReplaceStringDictionary PassData, params object[] Parameters)
        {
            

            mDataEnvironment = dataEnvironment;
            mFilterToken = Parameters;

            try
            {
                Bitmap ShowImage = null;// mDataEnvironment.Screen.ActiveSelectedImage.Bitmap;

                ISelection Selection = (ISelection)mDataEnvironment.Screen.ActiveSelection.Clone();
                Selection.BringToZero();

                double[] ArrayInfo = ImagingTools.ConvertGrayscaleSelectionToLinear(ShowImage, Selection);
                double[,] HistoData = ImagingTools.MakeHistogramIndexed(ArrayInfo);

                CreateGraph(HistoData);
            }
            catch { }

            mDataEnvironment.Screen.SelectionPerformed += new ScreenProperties.SelectionPerfomedEventExtended(HistogramTool_SelectionPerformed);
            
            return SourceImage;
        }

        void HistogramTool_SelectionPerformed(ScreenProperties SourceImage, ISelection SelectionIn)
        {
            try
            {
                ImageHolder ShowImage = (ImageHolder)SourceImage.ActiveSelectedImage;
                ISelection Selection = (ISelection)SelectionIn.Clone();
                Selection.BringToZero();
                double[] ArrayInfo = ImagingTools.ConvertGrayscaleSelectionToLinear(ShowImage);
                double[,] HistoData = ImagingTools.MakeHistogramIndexed(ArrayInfo);
                CreateGraph(HistoData);
            }
            catch { }
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
                list.Add(Data[0,x], Data[1,x]);
            }
            myPane.CurveList.Clear();
            // Generate a blue curve with circle symbols, and "My Curve 2" in the legend
            LineItem myCurve = myPane.AddCurve("", list, Color.Blue, SymbolType.None );
            // Fill the area under the curve with a white-red gradient at 45 degrees
            myCurve.Line.Fill = new Fill(Color.Red);

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White);

            // Fill the pane background with a color gradient
            myPane.Fill = new Fill(Color.White);

            // Calculate the Axis Scale Ranges
            zedgraphcontrol . AxisChange();
            zedgraphcontrol.Invalidate();
        }

        
    }
}
