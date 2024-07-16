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

namespace ImageViewer3D.Filters.Analysis
{
    public partial class ProfileTool : aEffectForm3D
    {
        #region Setup Form
        private System.Windows.Forms.Button bFinished;
        private ZedGraph.ZedGraphControl zedgraphcontrol;
        public ProfileTool()
            : base()
        {
            pictureDisplay1.Visible = false;

            pInitializeComponent();

            splitContainer1.Visible = false;
            button1.Visible = false;
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void pInitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.bFinished = new System.Windows.Forms.Button();
            this.zedgraphcontrol = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // Finished
            // 
            this.bFinished.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bFinished.Location = new System.Drawing.Point(673, 430);
            this.bFinished.Name = "Finished";
            this.bFinished.Size = new System.Drawing.Size(67, 35);
            this.bFinished.TabIndex = 25;
            this.bFinished.Text = "Done";
            this.bFinished.UseVisualStyleBackColor = true;
            this.bFinished.Click += new System.EventHandler(this.button1_Click);
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
            this.Controls.Add(this.bFinished);
            this.Name = "ContrastTool";
            this.Text = "ContrastTool";
            this.ResumeLayout(false);

        }
        #endregion

        public override string EffectName { get { return "Profile Tool"; } }
        public override string EffectMenu { get { return "Analysis"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 10; } }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }

        protected override ImageViewer3D.DataHolder doEffect(ImageViewer3D.DataEnvironment3D dataEnvironment, ImageViewer3D.DataHolder SourceImage, ImageViewer.Filters.ReplaceStringDictionary PassData, params object[] Parameters)
        {
            
            mDataEnvironment = dataEnvironment;
            mParameters = Parameters;


            mDataEnvironment.SelectionPerformed += new DataEnvironment3D.SelectionPerfomedEventExtended(mDataEnvironment_SelectionPerformed);

            mDataEnvironment_SelectionPerformed(mDataEnvironment, mDataEnvironment.ActiveSelection);
          
            return SourceImage;
        }

        void mDataEnvironment_SelectionPerformed(DataEnvironment3D SourceImage, ISelection3D SelectionIn)
        {
            
            try
            {
                if (SelectionIn != null)
                {
                    ISelection3D Selection = (ISelection3D)SelectionIn.Clone();
                    ImageViewer3D.Selections.ProfileSelection3D ps = (ImageViewer3D.Selections.ProfileSelection3D)Selection;
                    double[] Profile = SourceImage.OriginalData.GetProfile(ps.SelectionAxis, ps.SelectionSliceNumber, ps.P2, ps.P1);

                    GraphLine(Profile);
                }
            }
            catch { }
        }
        public void GraphLine(double[] Data)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedgraphcontrol.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "Pixels";
            myPane.YAxis.Title.Text = "Intensities";

            // Make up some mDataDouble points based on the Sine function
            PointPairList list = new PointPairList();
            for (int i = 0; i < Data.Length ; i++)
            {
                if (Data[i]!=0)
                    list.Add(i, Data[i]);
            }

            myPane.CurveList.Clear();

            // Generate offsetX red curve with diamond symbols, and "Alpha" in the legend
            LineItem myCurve = myPane.AddCurve("",
                list, Color.Red, SymbolType.None);



            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters

            zedgraphcontrol.AxisChange();

            zedgraphcontrol.Invalidate();

        }

    }
}
