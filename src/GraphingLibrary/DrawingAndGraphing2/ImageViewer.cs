using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using ImageViewer;
using MathHelpLib.DrawingAndGraphing;
using MathHelpLib;
using MathHelpLib.ImageProcessing;

namespace GraphingLib.DrawingAndGraphing
{
    public class MathImageViewer: PictureBox ,IGraphControl
    {

        object myData=null;

        #region Usercontrols
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem EditImage;
        private ToolStripMenuItem SaveImage;
        private SaveFileDialog saveFileDialog1;
        private System.ComponentModel.IContainer components;
        public MathImageViewer()
            : base()
        {
            InitializeComponent();
            CreateDefaultMenu(contextMenuStrip1 );
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.EditImage = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveImage = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EditImage,
            this.SaveImage});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(135, 48);
            // 
            // EditImage
            // 
            this.EditImage.Name = "EditImage";
            this.EditImage.Size = new System.Drawing.Size(134, 22);
            this.EditImage.Text = "Edit image";
            this.EditImage.Click += new System.EventHandler(this.EditImage_Click);
            // 
            // SaveImage
            // 
            this.SaveImage.Name = "SaveImage";
            this.SaveImage.Size = new System.Drawing.Size(134, 22);
            this.SaveImage.Text = "Save image";
            this.SaveImage.Click += new System.EventHandler(this.SaveImage_Click);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }
        private void SetupContextMenu()
        {
            InitializeComponent();
        }
        private void CreateDefaultMenu(ContextMenuStrip  MainMenu)
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
            if (myData.GetType() == typeof(PhysicalArray))
            {
                #region physical Array
                switch (menuText)
                {
                    case "Line Graph":
                        mParent.GraphData((PhysicalArray)myData, MathGraphTypes.Graph1D_Line);
                        break;
                    case "Scatter Graph":
                        mParent.GraphData((PhysicalArray)myData, MathGraphTypes.Graph1D_Scatter);
                        break;
                    case "Bar Graph":
                        mParent.GraphData((PhysicalArray)myData, MathGraphTypes.Graph1D_Bar);
                        break;
                    case "2D Contour":
                        mParent.GraphData((PhysicalArray)myData, MathGraphTypes.Graph2D_contour);
                        break;
                    case "image Viewer":
                        mParent.GraphData((PhysicalArray)myData, MathGraphTypes.Graph2D_ImageEditor);
                        break;
                    case "3D Multi-Angle Viewer":
                        mParent.GraphData((PhysicalArray)myData, MathGraphTypes.Graph3D_Slices_MultiAngleEditor);
                        break;
                    case "3D Iso Surface Viewer":
                        mParent.GraphData((PhysicalArray)myData, MathGraphTypes.Graph3D_Viewer);
                        break;
                }
                #endregion
            }
            else if (myData.GetType() == typeof(double[,]))
            {
                #region double[,] array
                switch (menuText)
                {
                    case "Line Graph":
                        mParent.GraphData((double[,])myData, MathGraphTypes.Graph1D_Line);
                        break;
                    case "Scatter Graph":
                        mParent.GraphData((double[,])myData, MathGraphTypes.Graph1D_Scatter);
                        break;
                    case "Bar Graph":
                        mParent.GraphData((double[,])myData, MathGraphTypes.Graph1D_Bar);
                        break;
                    case "2D Contour":
                        mParent.GraphData((double[,])myData, MathGraphTypes.Graph2D_contour);
                        break;
                    case "image Viewer":
                        mParent.GraphData((double[,])myData, MathGraphTypes.Graph2D_ImageEditor);
                        break;
                    case "3D Multi-Angle Viewer":
                        mParent.GraphData((double[,])myData, MathGraphTypes.Graph3D_Slices_MultiAngleEditor);
                        break;
                    case "3D Iso Surface Viewer":
                        mParent.GraphData((double[,])myData, MathGraphTypes.Graph3D_Viewer);
                        break;
                }
                #endregion
            }
            else if (myData.GetType() == typeof(Bitmap ))
            {
                #region bitmap
                switch (menuText)
                {
                    case "Line Graph":
                        mParent.GraphData((Bitmap)myData, MathGraphTypes.Graph1D_Line);
                        break;
                    case "Scatter Graph":
                        mParent.GraphData((Bitmap)myData, MathGraphTypes.Graph1D_Scatter);
                        break;
                    case "Bar Graph":
                        mParent.GraphData((Bitmap)myData, MathGraphTypes.Graph1D_Bar);
                        break;
                    case "2D Contour":
                        mParent.GraphData((Bitmap)myData, MathGraphTypes.Graph2D_contour);
                        break;
                    case "image Viewer":
                        mParent.GraphData((Bitmap)myData, MathGraphTypes.Graph2D_ImageEditor);
                        break;
                    case "3D Multi-Angle Viewer":
                        mParent.GraphData((Bitmap )myData, MathGraphTypes.Graph3D_Slices_MultiAngleEditor);
                        break;
                    case "3D Iso Surface Viewer":
                        mParent.GraphData((Bitmap)myData, MathGraphTypes.Graph3D_Viewer);
                        break;
                }
                #endregion
            }
        }
        #endregion

        public bool AdjustableImage
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a bitmap from 2D physical array
        /// </summary>
        /// <param name="PhysArray"></param>
        public void SetData(PhysicalArray PhysArray)
        {
            this.SizeMode = PictureBoxSizeMode.Zoom;
            this.Image = PhysArray.MakeBitmap();
            this.Invalidate();
            this.Refresh();
            Application.DoEvents();
            myData = PhysArray;
        }

        /// <summary>
        /// Not yet implemented
        /// </summary>
        /// <param name="array"></param>
        public void SetData(List<double[,]> array)
        {
            myData = array;
            throw new Exception("Not yet Implemented");
        }

        /// <summary>
        /// Not yet implemented
        /// </summary>
        /// <param name="array"></param>
        public void SetData(PhysicalArray[] PhysArrays)
        {
            myData = PhysArrays;
            throw new Exception("2D ImageData cannot be displayed in image graph");
        }

        /// <summary>
        /// Creates a bitmap from array
        /// </summary>
        /// <param name="ValueArray"></param>
        public void SetData(double[,] ValueArray)
        {
            myData = ValueArray;
            this.SizeMode = PictureBoxSizeMode.Zoom;
            this.Image = ValueArray.MakeBitmap();
            this.Invalidate();
            this.Refresh();
            Application.DoEvents();
        }

        public void SetData(Bitmap Image)
        {
            myData = Image;
            this.SizeMode = PictureBoxSizeMode.Zoom;
            this.Image = Image;
            this.Invalidate();
            this.Refresh();
            Application.DoEvents();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="ValueArray"></param>
        public void SetData(double[, ,] ValueArray)
        {
            throw new Exception("Datatype not supported");
        }


        public void SetData(string ImageFilename)
        {
            SetData(new Bitmap(ImageFilename));
        }

        /// <summary>
        /// Shows the graph change menu
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Right)
            {
                if (contextMenuStrip1 == null)
                    SetupContextMenu();
                contextMenuStrip1.Show(this.PointToScreen(new System.Drawing.Point(e.X, e.Y)));
            }
        }

        /// <summary>
        /// Saves the displayed image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SaveImage_Click(object sender, EventArgs e)
        {
            DialogResult ret = saveFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                this.Image.Save(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Flips over to imageeditor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EditImage_Click(object sender, EventArgs e)
        {
            if (mParent != null)
            {
                if (myData.GetType() == typeof(PhysicalArray))
                    mParent.GraphData((PhysicalArray)myData, MathGraphTypes.Graph2D_ImageEditor);
                else if (myData.GetType() == typeof(double[,]))
                    mParent.GraphData((double[,])myData, MathGraphTypes.Graph2D_ImageEditor);
            }
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
