using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer.Tools
{
    public class CenterROITool :  ROITool 
    {
        #region Labeling
        public override string GetToolName()
        {
            return "ROI Centered";
        }
        public override string GetToolTip()
        {
            return "Selected a region of the image, click to chose center of selection and then drag to set box size";
        }
        #endregion

        Point pDown = new Point();

        protected override void MouseDownImpl(ScreenProperties screenProperties, Point DownPoint, System.Windows.Forms.MouseEventArgs e)
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

        protected override void SetSelectionBox(Point pDown, Point pCurrent)
        {
            int HalfWidth = Math.Abs(pCurrent.X - pDown.X);
            int HalfHeight = Math.Abs(pCurrent.Y - pDown.Y);

            mSelectionBox.X = pDown.X - HalfWidth ;
            mSelectionBox.Y = pDown.Y-HalfHeight ;
            mSelectionBox.Width = HalfWidth*2;
            mSelectionBox.Height = HalfHeight*2;
        }

        protected override  void DrawSelectionBox(Rectangle SelectionBox)
        {
            Graphics g = Graphics.FromImage(mScreenProperties.ScreenFrontBuffer);
            g.DrawRectangle(Pens.Red, mSelectionBox);

            int MidX = mSelectionBox.X + mSelectionBox.Width / 2;
            int MidY = mSelectionBox.Y + mSelectionBox.Height / 2;
            g.DrawLine(Pens.Red, new Point(MidX, MidY - 3), new Point(MidX, MidY + 3));
            g.DrawLine(Pens.Red, new Point(MidX - 3, MidY), new Point(MidX + 3, MidY));

            mScreenProperties.ForceImageDisplay();
        }

        protected override void NotifyOfSelection()
        {
            mScreenProperties.NotifyOfSelection(new Selections.ROISelection( mScreenProperties.ConvertRectangleFromScreenToImage(  mSelectionBox ), ((PictureDisplay)mScreenProperties.PictureBox).Index)); 
        }

        private void HandleSelectionDone(Rectangle SelectionBox)
        {
            DrawSelectionBox(mSelectionBox);
        }
    }
}
