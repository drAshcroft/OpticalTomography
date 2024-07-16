using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer.Tools
{
    public class CircleTool: aDrawingTool
    {
        #region Labeling
        public override string GetToolName()
        {
            return "Circle Select";
        }
        public override string GetToolTip()
        {
            return "Selected a region of the image, use + to enlarge outer circle, - to shrink outer circle, * to enlarge inner circle, / to shrink inner circle.";
        }
        #endregion

        protected Point pDown = new Point();
        protected int mOverCircleAdd = 0;
        protected int mInnerCircleAdd = 0;

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
                DrawCircleSelectionBox();
            }
        }

        protected override void MouseUpImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ClearSelectionBox(mSelectionBox);
                Point pUp = PutPointInBounds(new Point(e.X, e.Y));

                SetSelectionBox(pDown, pUp);
                HandleSelectionDone();
            }
        }

        protected override void KeyPressImpl(KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '+':
                    mOverCircleAdd++;
                    break;
                case '-':
                    mOverCircleAdd--;
                    break;
                case '*':
                    mInnerCircleAdd++;
                    break;
                case '/':
                    mInnerCircleAdd--;
                    break;
            }
            ClearSelectionBox(mSelectionBox);
            DrawCircleSelectionBox();

        }

        protected override void ClearSelectionBox(Rectangle SelectionBox)
        {
            if (mOverCircleAdd > 0)
                base.ClearSelectionBox(SelectionBox, mOverCircleAdd + 2);
            else
                base.ClearSelectionBox(SelectionBox);
        }
        private void HandleSelectionDone()
        {
            ClearSelectionBox(mSelectionBox);
            DrawCircleSelectionBox();
        }

        protected virtual  void DrawCircleSelectionBox()
        {
            Graphics g = Graphics.FromImage(mScreenProperties.ScreenFrontBuffer);

            Rectangle tempR = new Rectangle(mSelectionBox.X, mSelectionBox.Y, mSelectionBox.Width, mSelectionBox.Height);
            tempR.Inflate(mOverCircleAdd, mOverCircleAdd);
            g.DrawEllipse(Pens.Red, tempR);

            tempR = new Rectangle(mSelectionBox.X, mSelectionBox.Y, mSelectionBox.Width, mSelectionBox.Height);
            tempR.Inflate(mInnerCircleAdd, mInnerCircleAdd);
            g.DrawEllipse(Pens.Red, tempR);

            int MidX = mSelectionBox.X + mSelectionBox.Width / 2;
            int MidY = mSelectionBox.Y + mSelectionBox.Height / 2;
            g.DrawLine(Pens.Red, new Point(MidX, MidY - 3), new Point(MidX, MidY + 3));
            g.DrawLine(Pens.Red, new Point(MidX - 3, MidY), new Point(MidX + 3, MidY));

            mScreenProperties.ForceImageDisplay();
        }

        protected override void NotifyOfSelection()
        {
            mScreenProperties.NotifyOfSelection(
                new Selections.CircleSelection(
                    mScreenProperties.ConvertRectangleFromScreenToImage( mSelectionBox ), 
                    mScreenProperties.ConvertWidthFromScreenToImage( mInnerCircleAdd ), 
                    mScreenProperties.ConvertWidthFromScreenToImage( mOverCircleAdd ), 
                    ((PictureDisplay)mScreenProperties.PictureBox).Index));
        }
        public override void RedrawSelection()
        {
            DrawCircleSelectionBox();
        }
    }
    
}
