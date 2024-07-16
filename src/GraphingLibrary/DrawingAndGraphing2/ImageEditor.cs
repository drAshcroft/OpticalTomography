using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageViewer.Filters;
using ImageViewer;
using MathHelpLib.DrawingAndGraphing;
using MathHelpLib.ImageProcessing;
using MathHelpLib;

namespace GraphingLib.DrawingAndGraphing
{
    public partial class ImageEditor : UserControl, IGraphControl
    {
        public ImageEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// creates the default imageeditor menu structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageEditor_Load(object sender, EventArgs e)
        {
            if (viewerControl1.MyMenu != null)
            {
                this.Controls.Add(viewerControl1.MyMenu);
                viewerControl1.MyMenu.Visible = true;
                viewerControl1.MyMenu.BringToFront();
                viewerControl1.Top = viewerControl1.MyMenu.Height;
                viewerControl1.Height = this.Height - viewerControl1.MyMenu.Height ;

                CreateDefaultMenu(viewerControl1.MyMenu);
            }
        }

        /// <summary>
        /// Creates the change graph type menu for alternative dataviews
        /// </summary>
        /// <param name="MainMenu"></param>
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

        /// <summary>
        /// Event handlers for changing the graph type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string menuText = ((ToolStripItem)sender).Text;
                if (mParent != null)
                {
                    if (mData.GetType() == typeof(PhysicalArray))
                    {
                        switch (menuText)
                        {
                            case "Line Graph":
                                mParent.GraphData((PhysicalArray)mData, MathGraphTypes.Graph1D_Line);
                                break;
                            case "Scatter Graph":
                                mParent.GraphData((PhysicalArray)mData, MathGraphTypes.Graph1D_Scatter);
                                break;
                            case "Bar Graph":
                                mParent.GraphData((PhysicalArray)mData, MathGraphTypes.Graph1D_Bar);
                                break;
                            case "2D Contour":
                                mParent.GraphData((PhysicalArray)mData, MathGraphTypes.Graph2D_contour);
                                break;
                            case "image Viewer":
                                mParent.GraphData((PhysicalArray)mData, MathGraphTypes.Graph2D_ImageEditor);
                                break;
                            case "3D Multi-Angle Viewer":
                                mParent.GraphData((PhysicalArray)mData, MathGraphTypes.Graph3D_Slices_MultiAngleEditor);
                                break;
                            case "3D Iso Surface Viewer":
                                mParent.GraphData((PhysicalArray)mData, MathGraphTypes.Graph3D_Viewer);
                                break;
                        }
                    }
                    else
                    {
                        switch (menuText)
                        {
                            case "Line Graph":
                                mParent.GraphData((double[,])mData, MathGraphTypes.Graph1D_Line);
                                break;
                            case "Scatter Graph":
                                mParent.GraphData((double[,])mData, MathGraphTypes.Graph1D_Scatter);
                                break;
                            case "Bar Graph":
                                mParent.GraphData((double[,])mData, MathGraphTypes.Graph1D_Bar);
                                break;
                            case "2D Contour":
                                mParent.GraphData((double[,])mData, MathGraphTypes.Graph2D_contour);
                                break;
                            case "image Viewer":
                                mParent.GraphData((double[,])mData, MathGraphTypes.Graph2D_ImageEditor);
                                break;
                            case "3D Multi-Angle Viewer":
                                mParent.GraphData((double[,])mData, MathGraphTypes.Graph3D_Slices_MultiAngleEditor);
                                break;
                            case "3D Iso Surface Viewer":
                                mParent.GraphData((double[,])mData, MathGraphTypes.Graph3D_Viewer);
                                break;
                        }
                    }
                }
            }
            catch { Console.Beep(); }
        }


        public bool AdjustableImage
        {
            get { return true; }
        }

        /// <summary>
        /// keeps a reference to the data for graph changes
        /// </summary>
        private object mData;

        /// <summary>
        /// passes a reference to the image to the image viewer
        /// </summary>
        /// <param name="image"></param>
        public void SetData(Bitmap image)
        {
            viewerControl1.SetImage(image);
            this.Invalidate();
            this.Refresh();
            Application.DoEvents();
        }

        /// <summary>
        /// converts 2D physical array to image 
        /// </summary>
        /// <param name="PhysArray"></param>
        public void SetData(PhysicalArray PhysArray)
        {
            viewerControl1.SetImage(PhysArray.MakeBitmap());
            mData = PhysArray;
            this.Invalidate();
            this.Refresh();
            Application.DoEvents();
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="array"></param>
        public void SetData(List<double[,]> array)
        {
            throw new Exception("Not yet Implemented");
        }
        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="array"></param>
        public void SetData(PhysicalArray[] PhysArrays)
        {
            throw new Exception("2D ImageData cannot be displayed in volume graph");
        }

       /// <summary>
       /// Converts 2D array to intensity image
       /// </summary>
       /// <param name="ValueArray"></param>
        public void SetData(double[,] ValueArray)
        {
            viewerControl1.SetImage(ValueArray.MakeBitmap());
            mData = ValueArray;
            this.Invalidate();
            this.Refresh();
            Application.DoEvents();
        }

        /// <summary>
        /// Does not work for this data type
        /// </summary>
        /// <param name="ValueArray"></param>
        public void SetData(double[,,] ValueArray)
        {
            throw new Exception("Datatype not supported");
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

        /*  private void viewerControl1_SelectionPerformed(global::ImageViewer.ISelection Selection)
          {
              double[,] TestData = mData;

              if (Selection is ImageViewer.Selections.ProfileSelection)
              {
                  ImageViewer.Selections.ProfileSelection ps = (ImageViewer.Selections.ProfileSelection)Selection;
                  double[] Profile = TestData.GetProfileLine(ps.P1, ps.P2);
                  Graph1D.GraphLine(this.zedgraphcontrol, Profile.MakeGraphableArray(0, 1), "Pixel", "Intensity");
                  rtStats.Text = "Total Length = " + Profile.Length.ToString() + " Pixels \n";
              }
              else
              {
                  double[] Intensitities = LUTTool.ClipImage(TestData, Selection);
                  Graph1D.GraphBar(this.zedgraphcontrol, LUTTool.MakeHistogramIndexed(Intensitities, 100), "Intensity", "");
                  rtStats.Text = "Total Area = " + Intensitities.Length.ToString() + " Pixels \n";

              }

          }*/

    }
}
