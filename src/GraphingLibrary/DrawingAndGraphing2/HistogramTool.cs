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

        public override IEffectToken DefaultToken
        {
            get
            {
                return CreateToken();
            }
        }
       
        public override IEffectToken CreateToken(params object[] TokenValues)
        {
            mFilterToken = new GeneralToken();
            mFilterToken.Parameters = null;
            return mFilterToken;
        }

        protected override  Bitmap doEffect(Bitmap SourceImage, IEffectToken FilterToken)
        {
            double[] ArrayInfo = ImagingTools.ConvertGrayscaleSelectionToLinear(SourceImage,new Selections.ROISelection(new Rectangle(0,0,SourceImage.Width,SourceImage.Height ),0));
            double[,] HistoData = ImagingTools.MakeHistogramIndexed(ArrayInfo, 100);

            CreateGraph(HistoData);
            return SourceImage;
        }

       
        

        public override  string  RunEffect(ScreenProperties[]  SourceImage, IEffectToken ContrastToken)
        {
            if (ContrastToken == null)
            {
                ContrastToken = new GeneralToken();
                ContrastToken.Parameters = new object[1];
            }
            
            mSourceImages = SourceImage;
            mFilterToken = ContrastToken;

            try
            {
                Bitmap ShowImage = SourceImage[0].ActiveSelectedImage;

                ISelection Selection = (ISelection)SourceImage[0].ActiveSelection.Clone();
                Selection.BringToZero();

                double[] ArrayInfo = ImagingTools.ConvertGrayscaleSelectionToLinear(ShowImage, Selection);
                double[,] HistoData = ImagingTools.MakeHistogramIndexed(ArrayInfo, 100);

                CreateGraph(HistoData);
            }
            catch { }
            for (int i=0;i<SourceImage.Length;i++)
                SourceImage[i].SelectionPerformed += new ScreenProperties.SelectionPerfomedEventExtended(HistogramTool_SelectionPerformed);
            return EffectHelps.FormatMacroString(this);
        }

        void HistogramTool_SelectionPerformed(ScreenProperties SourceImage, ISelection SelectionIn)
        {
            try
            {
                Bitmap ShowImage = SourceImage.ActiveSelectedImage;
                ISelection Selection = (ISelection)SelectionIn.Clone();
                Selection.BringToZero();
                double[] ArrayInfo = ImagingTools.ConvertGrayscaleSelectionToLinear(ShowImage, Selection);
                double[,] HistoData = ImagingTools.MakeHistogramIndexed(ArrayInfo, 255);
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
