using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MathHelpLib.DrawingAndGraphing;
using MathHelpLib;

namespace GraphingLib.DrawingAndGraphing
{
    public delegate void ImageTypeGraphShownEvent(bool IsImageGraph, double ContrastMin, double ContrastMax);

    public partial class MathGraph : UserControl
    {
        /// <summary>
        /// Reference to current graph control,  handy for changing with need
        /// </summary>
        IGraphControl TheControl;
      

        /// <summary>
        /// Attempts to set the aspect ration for images that should not zoomed
        /// </summary>
        bool MaintainAspectRatio = false;
        double AspectRationYtoX = 1;
        public MathGraph()
        {
            InitializeComponent();
        }

        #region SelectGraphs

        /// <summary>
        /// Do all the needed memory cleaning
        /// </summary>
        public void FormClosing()
        {
            if (TheControl.GetType() == typeof(Graph3DSliceViewerMultiAngleEditor))
                ((Graph3DSliceViewerMultiAngleEditor)TheControl).FormClosing();
        }

        #region dynamic graphing usercontrol creation
        private IGraphControl Create1DGraph()
        {
            if (TheControl != null && TheControl.GetType() != typeof(Graph1D))
            {
                TheControl.Visible = false;
                this.Controls.Remove((Control)TheControl);
                TheControl = null;
            }

            if (TheControl != null)
            {
                ((Graph1D)TheControl).ClearData();
            }
            else
            {
                Graph1D g1d = new Graph1D();
                this.Controls.Add(g1d);
                g1d.Visible = true;
                g1d.Dock = DockStyle.Fill;
                Application.DoEvents();
                TheControl = g1d;
                MaintainAspectRatio = false;
            }
            return TheControl;
        }
        private IGraphControl Create2DContourGraph()
        {
            if (TheControl != null && TheControl.GetType() != typeof(Graph1D))
            {
                TheControl.Visible = false;
                this.Controls.Remove((Control)TheControl);
                TheControl = null;
            }
            return TheControl;
        }
        private IGraphControl Create2DImageGraph()
        {
            if (TheControl != null && TheControl.GetType() != typeof(MathImageViewer))
            {
                TheControl.Visible = false;
                this.Controls.Remove((Control)TheControl);
                TheControl = null;
            }
            if (TheControl == null)
            {
                MathImageViewer g1d = new MathImageViewer();
                this.Controls.Add(g1d);
                g1d.Visible = true;
                TheControl = (IGraphControl)g1d;
                MaintainAspectRatio = true;

                MathGraph_Resize(this, EventArgs.Empty);
            }
            return TheControl;
        }
        private IGraphControl Create2DImageEditorGraph()
        {
            if (TheControl != null && TheControl.GetType() != typeof(DrawingAndGraphing.ImageEditor))
            {
                TheControl.Visible = false;
                this.Controls.Remove((Control)TheControl);
                TheControl = null;
            }
            if (TheControl == null)
            {
                GraphingLib.DrawingAndGraphing.ImageEditor g1d = new GraphingLib.DrawingAndGraphing.ImageEditor();
                this.Controls.Add(g1d);
                g1d.Visible = true;
                TheControl = (IGraphControl)g1d;
                MaintainAspectRatio = true;

                MathGraph_Resize(this, EventArgs.Empty);
            }
            return TheControl;
        }
        private IGraphControl Create3DSliceGraph()
        {
            if (TheControl != null && TheControl.GetType() != typeof(Graph3DSliceViewer))
            {
                TheControl.Visible = false;
                this.Controls.Remove((Control)TheControl);
                TheControl = null;
            }
            if (TheControl == null)
            {
                Graph3DSliceViewer g1d = new Graph3DSliceViewer();
                this.Controls.Add(g1d);
                g1d.Visible = true;
                g1d.Dock = DockStyle.Fill;
                TheControl = g1d;
            }
            return TheControl;
        }
        private IGraphControl Create3DSliceMultiAngleGraphEditor()
        {
            if (TheControl != null && TheControl.GetType() != typeof(Graph3DSliceViewerMultiAngleEditor))
            {
                TheControl.Visible = false;
                this.Controls.Remove((Control)TheControl);
                TheControl = null;
            }
            if (TheControl == null)
            {
                Graph3DSliceViewerMultiAngleEditor g1d = new Graph3DSliceViewerMultiAngleEditor();
                this.Controls.Add(g1d);
                g1d.Visible = true;
                g1d.Dock = DockStyle.Fill;
                TheControl = g1d;
            }
            return TheControl;
        }
        /// <summary>
        /// Doesnt work yet
        /// </summary>
        /// <returns></returns>
        private IGraphControl Create3DGraph()
        {
            /*if (TheControl != null && TheControl.GetType() != typeof(Graph3D))
            {
                TheControl.Visible = false;
                this.Controls.Remove((Control)TheControl);
                TheControl = null;
            }
            if (TheControl == null)
            {
                Graph3D g1d = new Graph3D();
                this.Controls.Add(g1d);
                g1d.Visible = true;
                g1d.Dock = DockStyle.Fill;
                TheControl = g1d;
            }*/
            return TheControl;
        }
        #endregion

        /// <summary>
        /// Parent control to create the appropriate graph type, and set the aspect ratio
        /// </summary>
        /// <param name="GraphType"></param>
        /// <param name="DataWidth"></param>
        /// <param name="DataHeight"></param>
        private void SelectGraph(MathGraphTypes GraphType, int DataWidth, int DataHeight)
        {
            switch (GraphType)
            {
                case MathGraphTypes.Graph1D_Line:
                case MathGraphTypes.Graph1D_Scatter:
                    TheControl = Create1DGraph();
                    break;
                case MathGraphTypes.Graph2D_contour:
                    TheControl = Create2DContourGraph();
                    break;
                case MathGraphTypes.Graph2D_Image:
                    TheControl = Create2DImageGraph();
                    AspectRationYtoX = (double)DataHeight / (double)DataWidth;
                    break;
                case MathGraphTypes.Graph2D_ImageEditor:
                    TheControl = Create2DImageEditorGraph();
                    AspectRationYtoX = (double)DataHeight / (double)DataWidth;
                    break;
                case MathGraphTypes.Graph3D_Slices:
                    TheControl = Create3DSliceGraph();
                    break;
                case MathGraphTypes.Graph3D_Slices_MultiAngle:
                    //TheControl = Create3DSliceMultiAngleGraph();
                    break;
                case MathGraphTypes.Graph3D_Slices_MultiAngleEditor:
                    TheControl = Create3DSliceMultiAngleGraphEditor();
                    break;
                case MathGraphTypes.Graph3D_Viewer:
                    TheControl = Create3DGraph();
                    break;
            }
            TheControl.SetParentControl(this);
        }

        #endregion

        #region GraphSelectors

        /// <summary>
        /// Creates a graph from physical array, first tries suggestedgraphinghint and then sets up a 1D line, a image, or a slice multiview
        /// </summary>
        /// <param name="PhysArray"></param>
        public void GraphData(PhysicalArray PhysArray)
        {
            if (PhysArray == null)
                return;
            if (PhysArray.ArrayInformation.SuggestedGraphingHint != MathGraphTypes.Unknown)
            {
                GraphData(PhysArray, PhysArray.ArrayInformation.SuggestedGraphingHint);
            }
            else if (PhysArray.ArrayRank == PhysicalArrayRank.Array1D)
            {
                GraphData(PhysArray, MathGraphTypes.Graph1D_Line);
            }
            else if (PhysArray.ArrayRank == PhysicalArrayRank.Array2D)
            {
                GraphData(PhysArray, MathGraphTypes.Graph2D_Image);
            }
            else
            {
                GraphData(PhysArray, MathGraphTypes.Graph3D_Slices_MultiAngleEditor);
            }
        }

        /// <summary>
        /// Creates the desired graph type from a physical array.  Errors will be thrown if incompadible graph types are selected
        /// </summary>
        /// <param name="PhysArray"></param>
        /// <param name="GraphType"></param>
        public void GraphData(PhysicalArray PhysArray, MathGraphTypes GraphType)
        {
            if (PhysArray.ArrayRank != PhysicalArrayRank.Array1D)
                SelectGraph(GraphType, PhysArray.GetLength(Axis.XAxis), PhysArray.GetLength(Axis.YAxis));
            else
                SelectGraph(GraphType, 1, 1);
            TheControl.SetData(PhysArray);
        }

        /// <summary>
        /// Creates multiple graphs from physical array.  Used mostly only for multiple 1D lines
        /// </summary>
        /// <param name="PhysArray"></param>
        public void GraphData(PhysicalArray[] PhysArray)
        {
            if (PhysArray == null)
                return;
            if (PhysArray[0].ArrayInformation.SuggestedGraphingHint != MathGraphTypes.Unknown)
            {
                GraphData(PhysArray, PhysArray[0].ArrayInformation.SuggestedGraphingHint);
            }
            else if (PhysArray[0].GetLength(Axis.YAxis) == 2)
            {
                GraphData(PhysArray, MathGraphTypes.Graph1D_Line);
            }
            else
            {
                GraphData(PhysArray, MathGraphTypes.Graph2D_Image);
            }
        }

        /// <summary>
        /// Creates the desired graph type from a physical array.  Errors will be thrown if incompadible graph types are selected
        /// Only 1D lines support this at this time
        /// </summary>
        /// <param name="PhysArray"></param>
        /// <param name="GraphType"></param>
        public void GraphData(PhysicalArray[] PhysArray, MathGraphTypes GraphType)
        {
            SelectGraph(GraphType, PhysArray[0].GetLength(Axis.XAxis), PhysArray[0].GetLength(Axis.YAxis));
            TheControl.SetData(PhysArray);
        }

        /// <summary>
        /// Creates 1D line from array
        /// </summary>
        /// <param name="PhysArray"></param>
        public void GraphData(double[] PhysArray)
        {
            if (PhysArray == null)
                return;
            GraphData(PhysArray, MathGraphTypes.Graph1D_Line);
        }

        /// <summary>
        /// Creates the desired graph type from a physical array.  Errors will be thrown if incompadible graph types are selected
        /// </summary>
        /// <param name="PhysArray"></param>
        /// <param name="GraphType"></param>
        public void GraphData(double[] ValArray, MathGraphTypes GraphType)
        {
            SelectGraph(GraphType, 1, 1);
            TheControl.SetData(ValArray.MakeGraphableArray(0, 1));
        }

        /// <summary>
        /// Creates either a line from a rank 2 array or an image from any other size
        /// </summary>
        /// <param name="PhysArray"></param>
        public void GraphData(double[,] PhysArray)
        {
            if (PhysArray == null)
                return;
            if (PhysArray.GetLength(0) == 2)
            {
                GraphData(PhysArray, MathGraphTypes.Graph1D_Line);
            }
            else
            {
                GraphData(PhysArray, MathGraphTypes.Graph2D_Image);
            }
        }

        /// <summary>
        /// Creates the desired graph type from a physical array.  Errors will be thrown if incompadible graph types are selected
        /// </summary>
        /// <param name="PhysArray"></param>
        /// <param name="GraphType"></param>
        public void GraphData(double[,] PhysArray, MathGraphTypes GraphType)
        {
            SelectGraph(GraphType, PhysArray.GetLength(0), PhysArray.GetLength(1));
            TheControl.SetData(PhysArray);
        }

        /// <summary>
        /// Creates a 3D slice array graph
        /// </summary>
        /// <param name="PhysArray"></param>
        public void GraphData(double[, ,] PhysArray)
        {
            if (PhysArray == null)
                return;
            GraphData(PhysArray, MathGraphTypes.Graph3D_Slices_MultiAngleEditor );
        }

        /// <summary>
        /// Creates the desired graph type from a physical array.  Errors will be thrown if incompadible graph types are selected
        /// </summary>
        /// <param name="PhysArray"></param>
        /// <param name="GraphType"></param>
        public void GraphData(double[,,] PhysArray, MathGraphTypes GraphType)
        {
            SelectGraph(GraphType, PhysArray.GetLength(0), PhysArray.GetLength(1));
            TheControl.SetData(PhysArray);
        }

        /// <summary>
        /// Creates either a line from a rank 2 array or an image from any other size
        /// </summary>
        /// <param name="PhysArray"></param>
        public void GraphData(List<double[,]> PhysArray)
        {
            if (PhysArray == null)
                return;
            if (PhysArray[0].GetLength(0) == 2)
            {
                GraphData(PhysArray, MathGraphTypes.Graph1D_Line);
            }
            else
            {
                GraphData(PhysArray, MathGraphTypes.Graph2D_Image);
            }
        }

        /// <summary>
        /// Creates the desired graph type from a physical array.  Errors will be thrown if incompadible graph types are selected
        /// </summary>
        /// <param name="PhysArray"></param>
        /// <param name="GraphType"></param>
        public void GraphData(List<double[,]> PhysArray, MathGraphTypes GraphType)
        {
            SelectGraph(GraphType, PhysArray[0].GetLength(0), PhysArray[0].GetLength(1));
            TheControl.SetData(PhysArray);
        }

        /// <summary>
        /// Creates an image viewer
        /// </summary>
        /// <param name="Image"></param>
        public void GraphData(Bitmap Image)
        {
            if (Image == null)
                return;
            GraphData(Image, MathGraphTypes.Graph2D_Image);
        }

        /// <summary>
        /// Creates the desired graph type from a physical array.  Errors will be thrown if incompadible graph types are selected
        /// </summary>
        /// <param name="PhysArray"></param>
        /// <param name="GraphType"></param>
        public void GraphData(Bitmap Image, MathGraphTypes GraphType)
        {
            SelectGraph(GraphType, Image.Width, Image.Height);
            if (TheControl.GetType() == typeof(MathImageViewer))
                ((MathImageViewer)TheControl).SetData(Image);
            else 
                ((ImageEditor)TheControl).SetData(Image);
        }

        /// <summary>
        /// Creates the desired graph type from a physical array.  Errors will be thrown if incompadible graph types are selected
        /// </summary>
        /// <param name="PhysArray"></param>
        /// <param name="GraphType"></param>
        public void GraphData(string ImageFileName)
        {
            string exten = Path.GetExtension(ImageFileName ).ToLower();
            if (exten == ".cct" || exten == ".raw" || exten == ".dat" || exten == ".tif")
            {
                SelectGraph(MathGraphTypes.Graph3D_Slices_MultiAngleEditor, 200, 200);
                Application.DoEvents();
                ((Graph3DSliceViewerMultiAngleEditor)TheControl).SetData(ImageFileName);
            }
            else 
            {
                SelectGraph(MathGraphTypes.Graph2D_Image , 200, 200);
                Application.DoEvents();
                ((MathImageViewer)TheControl).SetData(ImageFileName);
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// tries to maintain the correct size ratio so the image looks correct
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MathGraph_Resize(object sender, EventArgs e)
        {
            if (MaintainAspectRatio)
            {
                TheControl.Width = this.Width;
                TheControl.Height = (int)(this.Width * AspectRationYtoX);
            }
        }
        #endregion
    }


}
