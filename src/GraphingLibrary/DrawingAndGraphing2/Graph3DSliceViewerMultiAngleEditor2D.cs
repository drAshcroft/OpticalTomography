using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageViewer.Filters;
using ZedGraph;
namespace MathHelpLib.DrawingAndGraphing
{
    public partial class Graph3DSliceViewerMultiAngleEditor2D : UserControl, IGraphControl
    {
        double[, ,] mGraphData;
        

        double SliceXPercent;
        double SliceYPercent;
        double SliceZPercent;
        
        double mMaxContrast = 0;
        double mMinContrast = 0;
        double mMaxData = 0;
        double mMinData = 0;
        MathHelpLib._3DStuff.CrossDisplay pseudoCube1;

       /* global::ImageViewer.Filters.LUTTool mLUTTool = new global::ImageViewer.Filters.LUTTool();
        global::ImageViewer.Filters.FlyThroughTool mFlyThrough = new FlyThroughTool();
        global::ImageViewer.Filters.MIPTool mMIPTool = new MIPTool();
        global::ImageViewer.Filters.SaveStackTool mSaveStack = new SaveStackTool();
        */
        public bool AdjustableImage
        {
            get { return true; }
        }

        public Graph3DSliceViewerMultiAngleEditor2D()
        {
            InitializeComponent();
            SliceXPercent = .5;
            SliceYPercent = .5;
            SliceZPercent = .5;


            pseudoCube1 = new MathHelpLib._3DStuff.CrossDisplay();
            viewerControl1.ExtraControl = pseudoCube1;

            this.pseudoCube1.Location = new System.Drawing.Point(0, 0);
            this.pseudoCube1.Name = "pseudoCube1";
            this.pseudoCube1.Size = new System.Drawing.Size(336, 289);
            this.pseudoCube1.TabIndex = 4;
            this.pseudoCube1.XAxisMoved += new MathHelpLib._3DStuff.CrossDisplay.XAxisMoviedEvent(this.pseudoCube1_XAxisMoved);
            this.pseudoCube1.ZAxisMoved += new MathHelpLib._3DStuff.CrossDisplay.ZAxisMoviedEvent(this.pseudoCube1_ZAxisMoved);
            this.pseudoCube1.YAxisMoved += new MathHelpLib._3DStuff.CrossDisplay.YAxisMoviedEvent(this.pseudoCube1_YAxisMoved);

            if (viewerControl1.MyMenu != null)
            {
                this.Controls.Add(viewerControl1.MyMenu);
                viewerControl1.MyMenu.Visible = true;
                viewerControl1.MyMenu.BringToFront();
                viewerControl1.Top = viewerControl1.MyMenu.Height;
                viewerControl1.Height = this.Height - viewerControl1.MyMenu.Height  ;

                CreateDefaultMenu(viewerControl1.MyMenu);
            }

           /* mLUTTool.RefreshDataStore += new global::ImageViewer.Filters.RefreshDataStoreEvent(mLUTTool_RefreshDataStore);
            mLUTTool.UpdateContrast += new global::ImageViewer.Filters.UpdateContrastEvent(mLUTTool_UpdateContrast);
            mFlyThrough.RefreshDataStore += new RefreshDataStoreEvent(mLUTTool_RefreshDataStore);
            mMIPTool.RefreshDataStore += new RefreshDataStoreEvent(mLUTTool_RefreshDataStore);
            mSaveStack.RefreshDataStore += new RefreshDataStoreEvent(mLUTTool_RefreshDataStore);
            global::ImageViewer.Filters.MnIPTool minIp = new MnIPTool();

            minIp.RefreshDataStore += new RefreshDataStoreEvent(mLUTTool_RefreshDataStore);

            viewerControl1.AddMenuItem(mLUTTool);
            viewerControl1.AddMenuItem(mFlyThrough);
            viewerControl1.AddMenuItem(mMIPTool);
            viewerControl1.AddMenuItem(mSaveStack);
            viewerControl1.AddMenuItem(minIp);*/
        }

        

        private void CreateDefaultMenu(MenuStrip MainMenu)
        {
            System.Windows.Forms.ToolStripMenuItem changeGraphToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem lineGraphToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem scatterGraphToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem barGraphToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem dContourToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem imageViewerToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem dMultiAngleViewerToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem dIsoSurfaceViewerToolStripMenuItem;

            changeGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            lineGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            scatterGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            barGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            dContourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            imageViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            dMultiAngleViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            dIsoSurfaceViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
             changeGraphToolStripMenuItem
            });
            // 
            // changeGraphToolStripMenuItem
            // 
            changeGraphToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
             lineGraphToolStripMenuItem,
             scatterGraphToolStripMenuItem,
             barGraphToolStripMenuItem,
             dContourToolStripMenuItem,
             imageViewerToolStripMenuItem,
             dMultiAngleViewerToolStripMenuItem,
             dIsoSurfaceViewerToolStripMenuItem});
            changeGraphToolStripMenuItem.Name = "changeGraphToolStripMenuItem";
            changeGraphToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            changeGraphToolStripMenuItem.Text = "Change Graph";
            // 
            // lineGraphToolStripMenuItem
            // 
            lineGraphToolStripMenuItem.Name = "lineGraphToolStripMenuItem";
            lineGraphToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            lineGraphToolStripMenuItem.Text = "Line Graph";
            lineGraphToolStripMenuItem.Click += new EventHandler(ToolStripMenuItem_Click);
            // 
            // scatterGraphToolStripMenuItem
            // 
            scatterGraphToolStripMenuItem.Name = "scatterGraphToolStripMenuItem";
            scatterGraphToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            scatterGraphToolStripMenuItem.Text = "Scatter Graph";
            scatterGraphToolStripMenuItem.Click += new EventHandler(ToolStripMenuItem_Click);
            // 
            // barGraphToolStripMenuItem
            // 
            barGraphToolStripMenuItem.Name = "barGraphToolStripMenuItem";
            barGraphToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            barGraphToolStripMenuItem.Text = "Bar Graph";
            barGraphToolStripMenuItem.Click += new EventHandler(ToolStripMenuItem_Click);
            // 
            // dContourToolStripMenuItem
            // 
            dContourToolStripMenuItem.Name = "dContourToolStripMenuItem";
            dContourToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            dContourToolStripMenuItem.Text = "2D Contour";
            dContourToolStripMenuItem.Click += new EventHandler(ToolStripMenuItem_Click);
            // 
            // imageViewerToolStripMenuItem
            // 
            imageViewerToolStripMenuItem.Name = "imageViewerToolStripMenuItem";
            imageViewerToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            imageViewerToolStripMenuItem.Text = "image Viewer";
            imageViewerToolStripMenuItem.Click += new EventHandler(ToolStripMenuItem_Click);
            // 
            // dMultiAngleViewerToolStripMenuItem
            // 
            dMultiAngleViewerToolStripMenuItem.Name = "dMultiAngleViewerToolStripMenuItem";
            dMultiAngleViewerToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            dMultiAngleViewerToolStripMenuItem.Text = "3D Multi-Angle Viewer";
            dMultiAngleViewerToolStripMenuItem.Click += new EventHandler(ToolStripMenuItem_Click);
            // 
            // dIsoSurfaceViewerToolStripMenuItem
            // 
            dIsoSurfaceViewerToolStripMenuItem.Name = "dIsoSurfaceViewerToolStripMenuItem";
            dIsoSurfaceViewerToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            dIsoSurfaceViewerToolStripMenuItem.Text = "3D Iso Surface Viewer";
            dIsoSurfaceViewerToolStripMenuItem.Click += new EventHandler(ToolStripMenuItem_Click);
        }

        void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string menuText = ((ToolStripItem)sender).Text;
            
                switch (menuText)
                {
                    case "Line Graph":
                        mParent.GraphData(mGraphData , MathGraphTypes.Graph1D_Line);
                        break;
                    case "Scatter Graph":
                        mParent.GraphData(mGraphData, MathGraphTypes.Graph1D_Scatter);
                        break;
                    case "Bar Graph":
                        mParent.GraphData(mGraphData, MathGraphTypes.Graph1D_Bar);
                        break;
                    case "2D Contour":
                        mParent.GraphData(mGraphData, MathGraphTypes.Graph2D_contour);
                        break;
                    case "image Viewer":
                        mParent.GraphData(mGraphData, MathGraphTypes.Graph2D_ImageEditor);
                        break;
                    case "3D Multi-Angle Viewer":
                        mParent.GraphData(mGraphData, MathGraphTypes.Graph3D_Slices_MultiAngleEditor);
                        break;
                    case "3D Iso Surface Viewer":
                        mParent.GraphData(mGraphData, MathGraphTypes.Graph3D_Viewer);
                        break;
                }
            
        }


        public void SetData(PhysicalArray PhysArray)
        {
            SetData((double[, ,])PhysArray.ReferenceDataDouble);
        }

        public void SetData(double[, ,] GraphData)
        {

            mGraphData = GraphData;
            double maxContrast = GraphData.MaxArray();
            double minContrast = GraphData.MinArray();
            mMaxData = maxContrast;
            mMinData = minContrast;
            double MidContrast, stdev;

            ContrastArray(GraphData, out MidContrast, out stdev);
            mMaxContrast = MidContrast + 2*stdev;
            mMinContrast = MidContrast -  stdev;

            ShowSlice();
        }

        public unsafe void ContrastArray(double[, ,] array, out double Average, out double StDev)
        {
            double sum = 0;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;
                double cc = 0;
                for (int i = 0; i < array.Length; i += 5)
                {
                    if (*pAOut > 0)
                    {
                        sum += *pAOut;
                        cc++;
                    }
                    pAOut++;
                }
                double ave = sum / cc;
                double x;
                sum = 0; cc = 0;
                for (int i = 1; i < array.Length; i += 5)
                {
                    if (*pAOut > 0)
                    {
                        x = (*pAOut - ave);
                        sum += x * x;
                        cc++;
                    }
                    pAOut++;
                }
                Average = ave;
                StDev = Math.Sqrt(sum / cc);
            }

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

        double[][,] Slices = new double[3][,];
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

            Bitmap[] Angles = new Bitmap[3];

            Slices[0] = mGraphData.SliceXAxis((int)((double)(mGraphData.GetLength(0) - 1) * SliceXPercent));
            Slices[2] = mGraphData.SliceYAxis((int)((double)(mGraphData.GetLength(1) - 1) * SliceYPercent));
            Slices[1] = mGraphData.SliceZAxis((int)((double)(mGraphData.GetLength(2) - 1) * SliceZPercent));

            Angles[1] = Slices[1].MakeBitmap(mMinContrast, mMaxContrast);
            Angles[0] = Slices[0].MakeBitmap(mMinContrast, mMaxContrast);
            Angles[2] = Slices[2].MakeBitmap(mMinContrast, mMaxContrast);

           // viewerControl1.SetImage(Angles);

            pseudoCube1.ImageX = Angles[0];
            pseudoCube1.ImageY = Angles[2];
            pseudoCube1.ImageZ = Angles[1];

            pseudoCube1.Invalidate();
        }

        #region Events
        private void Graph3DSliceViewerMultiAngle_Resize(object sender, EventArgs e)
        {
            int HWidth = this.Width / 2;
            int HHeight = this.Height / 2;
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

        void mLUTTool_UpdateContrast(double MinContrast, double MaxContrast)
        {
            mMaxContrast = MaxContrast;
            mMinContrast = MinContrast;
            ShowSlice();
        }

        void mLUTTool_RefreshDataStore(out double[, ,] MyData, out double[,] Slice,out double  minContrastPercent,out double  maxContrastPercent)
        {
            MyData = mGraphData;
            Slice = MyData.SliceXAxis((int)((double)(mGraphData.GetLength(0) - 1) * SliceXPercent));
            minContrastPercent = ( mMinContrast-mMinData)/(mMaxData-mMinData) ;
            maxContrastPercent = (mMaxContrast - mMinData) / (mMaxData - mMinData);
        }
        #endregion

        MathGraph mParent;
        public void SetParentControl(MathGraph Parent)
        {
            mParent = Parent;
        }


       
    }

}