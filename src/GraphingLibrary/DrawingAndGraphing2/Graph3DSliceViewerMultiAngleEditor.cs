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
using MathHelpLib.DrawingAndGraphing;
using MathHelpLib;
namespace GraphingLib.DrawingAndGraphing
{
    public partial class Graph3DSliceViewerMultiAngleEditor : UserControl, IGraphControl
    {
        double[, ,] mGraphData;
        public bool AdjustableImage
        {
            get { return true; }
        }

        public bool Zooming
        {
            set { viewerControl3D1.Zooming = value; }
            get { return viewerControl3D1.Zooming;}

        }
        /// <summary>
        /// loads the control and attempts to initialize the 3D viewer
        /// </summary>
        public Graph3DSliceViewerMultiAngleEditor()
        {
            InitializeComponent();

            if (viewerControl3D1.MyMenu != null)
            {
                this.Controls.Add(viewerControl3D1.MyMenu);
                viewerControl3D1.MyMenu.Visible = true;
                viewerControl3D1.MyMenu.BringToFront();
                viewerControl3D1.Top = viewerControl3D1.MyMenu.Height;
                viewerControl3D1.Height = this.Height - viewerControl3D1.MyMenu.Height;

                CreateDefaultMenu(viewerControl3D1.MyMenu);
            }
            viewerControl3D1_Resize();
        }

        /// <summary>
        /// Makes sure that the memory is cleaned out from the 3D viewer to prevent leaks.
        /// </summary>
        public void FormClosing()
        {
            viewerControl3D1.FormClosing();
        }

        /// <summary>
        /// Creates the change graph type menu for alternative dataviews
        /// </summary>
        /// <param name="MainMenu"></param>
        private void CreateDefaultMenu(MenuStrip MainMenu)
        {

            //all of this is just copied from the designer code
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
                    switch (menuText)
                    {
                        case "Line Graph":
                            mParent.GraphData(mGraphData, MathGraphTypes.Graph1D_Line);
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
            }
            catch { Console.Beep(); }
        }

        /// <summary>
        /// Creates a copy of the data from a physical array and then sets it to the viewer control
        /// </summary>
        /// <param name="PhysArray"></param>
        public void SetData(PhysicalArray PhysArray)
        {
            SetData((double[, ,])PhysArray.ReferenceDataDouble);
            viewerControl3D1_Resize();
        }

        /// <summary>
        /// copies reference of the data to viewer control
        /// </summary>
        /// <param name="GraphData"></param>
        public void SetData(double[, ,] GraphData)
        {

            mGraphData = GraphData;
            viewerControl3D1.SetImage(GraphData);
            viewerControl3D1_Resize();
        }

        /// <summary>
        /// copies reference of the data to viewer control
        /// </summary>
        /// <param name="GraphData"></param>
        public void SetData(float[, ,] GraphData)
        {
            viewerControl3D1.SetImage(GraphData);
            viewerControl3D1_Resize();
        }

        /// <summary>
        /// Cannot handle this data type
        /// </summary>
        /// <param name="ValueArray"></param>
        public void SetData(double[,] ValueArray)
        {
            throw new Exception("2D ImageData cannot be displayed in volume graph");
        }
        /// <summary>
        /// Cannot handle this data type
        /// </summary>
        /// <param name="ValueArray"></param>
        public void SetData(List<double[,]> array)
        {
            throw new Exception("2D ImageData cannot be displayed in volume graph");
        }
        /// <summary>
        /// Cannot handle this data type
        /// </summary>
        /// <param name="ValueArray"></param>
        public void SetData(PhysicalArray[] PhysArrays)
        {
            throw new Exception("2D ImageData cannot be displayed in volume graph");
        }

        public void SetData(string ImageFilename)
        {
            viewerControl3D1.SetImage(ImageFilename);
            viewerControl3D1_Resize();
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

        void viewerControl3D1_Resize(object sender, EventArgs e)
        {
            viewerControl3D1.ViewerControl_Resize(this, e);
        }

        void viewerControl3D1_Resize()
        {
            viewerControl3D1.ViewerControl_Resize(this, EventArgs.Empty);
        }
    }

}