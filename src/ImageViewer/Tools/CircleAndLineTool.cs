using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer.Tools
{
    public class CircleAndLineTool : CircleTool
    {
        #region Labeling
        public override string GetToolName()
        {
            return "CircleAndLine Select";
        }
        public override string GetToolTip()
        {
            return "Selected the major axis, use + to enlarge outer circle, - to shrink outer circle, * to enlarge inner circle, / to shrink inner circle.  Use right mouse button to select a line from the center of the circle";
        }
        #endregion

        protected Rectangle mCircleSelectBox;
        protected Point pOriginalDown;
        protected Point pLineEnd = new Point(-1, -1);
        protected Point pCircleEnd;
        protected override void MouseDownImpl(ScreenProperties screenProperties, Point DownPoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pDown = new Point(e.X, e.Y);
                pOriginalDown = new Point(e.X, e.Y);
                mSelectionBox = new Rectangle();
            }
            else
                mCircleSelectBox = new Rectangle();
        }

        protected override void MouseMoveImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ClearSelectionBox(mSelectionBox);
                pCircleEnd = new Point(e.X, e.Y);
                SetSelectionBox();
                DrawCircleSelectionBox();
            }
            else if (e.Button == MouseButtons.Right)
            {
                ClearSelectionBox(mSelectionBox);
                pLineEnd = new Point(e.X, e.Y);
                SetSelectionBox();
                DrawCircleSelectionBox();
            }
        }

        protected override void MouseUpImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ClearSelectionBox(mSelectionBox);
                pCircleEnd = new Point(e.X, e.Y);
                SetSelectionBox();
                HandleSelectionDone();
            }
            else if (e.Button == MouseButtons.Right)
            {
                ClearSelectionBox(mSelectionBox);
                pLineEnd = new Point(e.X, e.Y);
                SetSelectionBox();
                DrawCircleSelectionBox();
                HandleSelectionDone();
            }
        }
        protected Point mCenter;
        protected void SetSelectionBox()
        {
            double Radius = Math.Sqrt(Math.Pow((pOriginalDown.X - pCircleEnd.X), 2) + Math.Pow((pOriginalDown.Y - pCircleEnd.Y), 2)) / 2d;
            double Cx = (pOriginalDown.X + pCircleEnd.X) / 2;
            double Cy = (pOriginalDown.Y + pCircleEnd.Y) / 2;
            Point pLeft = new Point((int)(Cx - Radius), (int)(Cy - Radius));
            mCenter = new Point((int)Cx, (int)Cy);
            mCircleSelectBox = new Rectangle(pLeft.X, pLeft.Y, (int)(2 * Radius), (int)(2 * Radius));

            if (pLineEnd.X != -1)
            {
                int left, right, top, bottom;
                if (Cx < pLineEnd.X)
                {
                    left = (int)Cx;
                    right = pLineEnd.X;
                }
                else
                {
                    right = (int)Cx;
                    left = pLineEnd.X;
                }
                if (Cy < pLineEnd.Y)
                {
                    top = (int)Cy;
                    bottom = pLineEnd.Y;
                }
                else
                {
                    top = pLineEnd.Y;
                    bottom = (int)Cy;
                }

                Rectangle mLineSelectionBox = new Rectangle();
                mLineSelectionBox.X = left;
                mLineSelectionBox.Y = top;
                mLineSelectionBox.Width = right - left;
                mLineSelectionBox.Height = bottom - top;
                mSelectionBox = Rectangle.Union(mLineSelectionBox, mCircleSelectBox);
            }
            else
            {
                mSelectionBox = mCircleSelectBox;
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

        protected override void DrawCircleSelectionBox()
        {
            Graphics g = Graphics.FromImage(mScreenProperties.ScreenFrontBuffer);

            Rectangle tempR = new Rectangle(mCircleSelectBox.X, mCircleSelectBox.Y, mCircleSelectBox.Width, mCircleSelectBox.Height);
            tempR.Inflate(mOverCircleAdd, mOverCircleAdd);
            g.DrawEllipse(Pens.Red, tempR);

            tempR = new Rectangle(mCircleSelectBox.X, mCircleSelectBox.Y, mCircleSelectBox.Width, mCircleSelectBox.Height);
            tempR.Inflate(mInnerCircleAdd, mInnerCircleAdd);
            g.DrawEllipse(Pens.Red, tempR);

            int MidX = mCenter.X;
            int MidY = mCenter.Y;
            g.DrawLine(Pens.Red, new Point(MidX, MidY - 3), new Point(MidX, MidY + 3));
            g.DrawLine(Pens.Red, new Point(MidX - 3, MidY), new Point(MidX + 3, MidY));


            if (pLineEnd.X != -1)
            {
                g.DrawLine(Pens.Red, mCenter, pLineEnd);
            }

            mScreenProperties.ForceImageDisplay();
        }

        protected override void NotifyOfSelection()
        {
            mScreenProperties.NotifyOfSelection(
                new Selections.CircleAndLineSelection(
                    mScreenProperties.ConvertRectangleFromScreenToImage( mSelectionBox), 
                    mScreenProperties.ConvertWidthFromScreenToImage( mInnerCircleAdd),
                    mScreenProperties.ConvertWidthFromScreenToImage( mOverCircleAdd), 
                    mScreenProperties.ConvertPointFromScreenToImage(  pLineEnd), 
                    ((PictureDisplay)mScreenProperties.PictureBox).Index)
            );
        }

       
    }

}
