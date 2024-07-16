using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer3D.Tools
{
    public class GrabTool3D:aDrawingTool3D
    {
        #region Labeling Info
        public override string GetToolName()
        {
            return "Grab";
        }
        public override string GetToolTip()
        {
            return "Moves the image after the image has been zoomed";
        }
        #endregion

        Point mDownPoint;
        Point pOriginalCorner;

        protected override void MouseDownImpl(ScreenProperties3D screenProperties, Point DownPoint, System.Windows.Forms.MouseEventArgs e)
        {
            mDownPoint = mScreenProperties.MouseCoordsToScreenCoords(DownPoint  );
            pOriginalCorner = new Point(mScreenProperties.DataEnvironment.ViewBox.X  , mScreenProperties.DataEnvironment.ViewBox.Y  );
        }
        protected override void MouseMoveImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point p2 = mScreenProperties.MouseCoordsToScreenCoords(MovePoint);
                p2.X = p2.X - mDownPoint.X;
                p2.Y = p2.Y - mDownPoint.Y;
                MoveGrabBox(p2);
            }
        }
        protected override void MouseUpImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            
        }
        private void MoveGrabBox(Point ChangePosition)
        {

            int Left = pOriginalCorner.X + ChangePosition.X;
            int Top = pOriginalCorner.Y + ChangePosition.Y;

            if (mScreenProperties.ScreenCoords.Width - Left > mScreenProperties.DataEnvironment.ViewBox.Width)
                Left = mScreenProperties.ScreenCoords.Width - mScreenProperties.DataEnvironment.ViewBox.Width;

            if (mScreenProperties.ScreenCoords.Height - Top > mScreenProperties.DataEnvironment.ViewBox.Height)
                Top = -1 * (mScreenProperties.DataEnvironment.ViewBox.Height - mScreenProperties.ScreenCoords.Height);


            if (Left > 0)
                Left = 0;
            if (Top > 0)
                Top = 0;

            mScreenProperties.DataEnvironment.ViewBox.X = Left;
            mScreenProperties.DataEnvironment.ViewBox.Y = Top;

            mScreenProperties.RedrawBuffers();
        }

        protected override void NotifyOfSelection()
        {
            mScreenProperties.DataEnvironment .NotifyOfSelection(null);
        }

        public override void RedrawSelection()
        {
            
        }
    }
}
