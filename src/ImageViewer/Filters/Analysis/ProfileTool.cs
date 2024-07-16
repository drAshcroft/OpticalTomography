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
using System.Threading;

namespace ImageViewer.Filters.Analysis
{
    public partial class ProfileTool : aEffectForm
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

        public override string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }

        ~ProfileTool()
        {
            try
            {
                if (t != null)
                    t.Abort();
            }
            catch { }
        }
        private Thread t;
        protected override object doEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mDataEnvironment = dataEnvironment;
            mFilterToken = Parameters;


            mDataEnvironment.Screen.SelectionPerformed += new ScreenProperties.SelectionPerfomedEventExtended(ProfileTool_SelectionPerformed);

            ProfileTool_SelectionPerformed(mDataEnvironment.Screen, mDataEnvironment.Screen.ActiveSelection);

            t = new Thread(new ThreadStart(GetProfileLine));
            t.Start();

            return SourceImage;
        }

        delegate void GraphLineEvent(double[] PixelValues);

        private class ProfileRequest
        {
            public bool Handled = true;
            public ImageHolder ShowImage;
            public Point p1;
            public Point p2;
        }
        ProfileRequest CurrentProfileRequest = new ProfileRequest();
        void GetProfileLine()
        {
            while (true)
            {
                try
                {

                    if (CurrentProfileRequest.Handled == false)
                    {
                        float[] Profile = CurrentProfileRequest.ShowImage.SampleOnLine(CurrentProfileRequest.p1, CurrentProfileRequest.p2);
                        
                        CurrentProfileRequest.Handled = true;
                        double[] PixelVals = ImagingTools.ConvertToIntensity(Profile);

                        this.Invoke(new GraphLineEvent(GraphLine), (PixelVals));

                    }
                }
                catch { }

                Thread.Sleep(100);
            }
        }

        void ProfileTool_SelectionPerformed(ScreenProperties SourceImage, ISelection SelectionIn)
        {
            try
            {
                ImageHolder ShowImage = (ImageHolder)SourceImage.OriginalImage;

                ISelection Selection = (ISelection)SourceImage.ActiveSelection;//.Clone();
               // Selection.BringToZero();

                ImageViewer.Selections.ProfileSelection ps = (ImageViewer.Selections.ProfileSelection)Selection;

                CurrentProfileRequest.ShowImage = ShowImage;
                CurrentProfileRequest.p1 = ps.P1;
                CurrentProfileRequest.p2 = ps.P2;
                System.Diagnostics.Debug.Print(ps.P1.ToString() + " " + ps.P2.ToString());
                CurrentProfileRequest.Handled = false;

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
            for (int i = 0; i < Data.Length - 1; i++)
            {
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
