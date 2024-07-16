using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer.Tools
{
    public class ProfileTool:aDrawingTool 
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
        protected override void MouseDownImpl(ScreenProperties screenProperties, Point DownPoint, System.Windows.Forms.MouseEventArgs e)
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
            mScreenProperties.NotifyOfSelection(
                new Selections.ProfileSelection(
                    mScreenProperties.ConvertRectangleFromScreenToImage( mSelectionBox ), 
                    mScreenProperties.ConvertPointFromScreenToImage( pDown ), 
                    mScreenProperties.ConvertPointFromScreenToImage( pUp ), 
                    ((PictureDisplay)mScreenProperties.PictureBox).Index));
        }

        public override void RedrawSelection()
        {
            DrawProfileLine( pUp );
        }
    }
}
