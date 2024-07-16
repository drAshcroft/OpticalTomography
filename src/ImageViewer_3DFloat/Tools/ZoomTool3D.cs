using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer3D.Tools
{
    public class ZoomTool3D : aDrawingTool3D
    {
        public override string GetToolName()
        {
            return "Zoom";
        }
        public override string GetToolTip()
        {
            return "Zooms into a section of the graph,  Use right button to zoom out";
        }
        Point pDown = new Point();

        protected override void MouseDownImpl(ScreenProperties3D screenProperties, Point DownPoint, System.Windows.Forms.MouseEventArgs e)
        {
            pDown = new Point(e.X, e.Y);
            mSelectionBox = new Rectangle();
        }

        protected override void MouseMoveImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ClearSelectionBox(mSelectionBox);
                SetSelectionBox(pDown, new Point(e.X, e.Y));
                DrawSelectionBox(mSelectionBox);
            }
        }



        protected override void MouseUpImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseUpImpl2(MovePoint, e);
                HandleSelectionDone(mSelectionBox);
            }
            if (e.Button == MouseButtons.Right)
            {
                mScreenProperties.DataEnvironment.Zooming = false;
                mScreenProperties.DataEnvironment.ResetZoom();
            }
            mScreenProperties.RedrawBuffers();
        }
        protected void MouseUpImpl2(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ClearSelectionBox(mSelectionBox);
                Point pUp = PutPointInBounds(new Point(e.X, e.Y));

                SetSelectionBox(pDown, pUp);
                HandleSelectionDone2(mSelectionBox);
            }
        }

        private void HandleSelectionDone2(Rectangle SelectionBox)
        {
            DrawSelectionBox(mSelectionBox);
        }

        private void HandleSelectionDone(Rectangle SelectionBox)
        {
           
            Rectangle NewView = ChangeToImageCoords(SelectionBox);
           
                
            mScreenProperties.DataEnvironment.Zooming = true;
            mScreenProperties.SetViewBounds(NewView);
        }

        protected override void NotifyOfSelection()
        {
            mScreenProperties.DataEnvironment.NotifyOfSelection(null);
        }

        public override void RedrawSelection()
        {
            //base.RedrawSelection();
        }
    }
}
