using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer3D.Tools
{
    public class ZoomTool3D : ROITool3D
    {
        public override string GetToolName()
        {
            return "Zoom";
        }
        public override string GetToolTip()
        {
            return "Zooms into a section of the graph,  Use right button to zoom out";
        }

        protected override void MouseUpImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                base.MouseUpImpl(MovePoint, e);
                HandleSelectionDone(mSelectionBox);
            }
            if (e.Button == MouseButtons.Right)
            {
                mScreenProperties.DataEnvironment.Zooming = false;
                mScreenProperties.DataEnvironment.ResetZoom();
            }
            mScreenProperties.RedrawBuffers();
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
