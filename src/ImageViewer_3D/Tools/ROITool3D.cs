using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer3D.Tools
{
    public class ROITool3D : aDrawingTool3D
    {
        #region Labeling
        public override string GetToolName()
        {
            return "ROI";
        }
        public override string GetToolTip()
        {
            return "Selected a region of the image";
        }
        #endregion

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
                ClearSelectionBox(mSelectionBox);
                Point pUp = PutPointInBounds(new Point(e.X, e.Y));

                SetSelectionBox(pDown, pUp);
                HandleSelectionDone(mSelectionBox);
            }
        }

        protected override void NotifyOfSelection()
        {
            Selections.ROISelection3D roi = new Selections.ROISelection3D(
                mScreenProperties.ConvertScreenToImage(mSelectionBox),
                ((PictureDisplay3DSlice)mScreenProperties.PictureBox).Index,
                mScreenProperties.SliceIndex ,
                mScreenProperties.Axis
                );
            mScreenProperties.DataEnvironment.NotifyOfSelection(roi);
        }

        private void HandleSelectionDone(Rectangle SelectionBox)
        {
            DrawSelectionBox(mSelectionBox);
        }

        public override void RedrawSelection()
        {
            DrawSelectionBox(mSelectionBox);
        }
    }
}
