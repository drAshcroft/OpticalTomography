using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer3D.Tools
{
    public class ProfileTool3D : aDrawingTool3D
    {
        public override string GetToolName()
        {
            return "Profile";
        }
        public override string GetToolTip()
        {
            return "Gets the profile of the data that the line overlays";
        }

        Point pDown;
        Point pUp;
        protected override void MouseDownImpl(ScreenProperties3D screenProperties, Point DownPoint, System.Windows.Forms.MouseEventArgs e)
        {
            pDown = DownPoint;
        }
        protected override void MouseMoveImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ClearSelectionBox(mSelectionBox);
                SetSelectionBox(pDown, new Point(e.X, e.Y));
                DrawProfileLine(new Point(e.X, e.Y));
                pUp = new Point(e.X, e.Y);
                NotifyOfSelection();
            }
        }
        protected override void MouseUpImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ClearSelectionBox(mSelectionBox);
                // Point pUp = PutPointInBounds(new Point(e.X, e.Y));
                pUp = new Point(e.X, e.Y);
                DrawProfileLine(pUp);
            }
        }
        private void DrawProfileLine(Point CurrentPoint)
        {
            Graphics g = Graphics.FromImage(mScreenProperties.ScreenFrontBuffer);
            g.DrawLine(Pens.Red, pDown, CurrentPoint);
            mScreenProperties.ForceImageDisplay();
        }

        protected override void NotifyOfSelection()
        {
            mScreenProperties.DataEnvironment.NotifyOfSelection(
                new Selections.ProfileSelection3D(
                    mScreenProperties.ConvertScreenToImage(mSelectionBox),
                    mScreenProperties.ConvertScreenToImage(pDown),
                    mScreenProperties.ConvertScreenToImage(pUp),
                    ((PictureDisplay3DSlice)mScreenProperties.PictureBox).Index,
                    mScreenProperties.Axis,
                    mScreenProperties.SliceIndex
                    )
                    );
        }

        public override void RedrawSelection()
        {
            DrawProfileLine(pUp);
        }
    }
}
