using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer.Tools
{
    public class CircleLineTool: CircleTool 
    {
        #region Labeling
        public override string GetToolName()
        {
            return "LCircle Line Select";
        }
        public override string GetToolTip()
        {
            return "Selected the major axis, use + to enlarge outer circle, - to shrink outer circle, * to enlarge inner circle, / to shrink inner circle.";
        }
        #endregion

        protected Point pOriginalDown;
        protected override void MouseDownImpl(ScreenProperties screenProperties, Point DownPoint, System.Windows.Forms.MouseEventArgs e)
        {
            pDown = new Point(e.X, e.Y);
            pOriginalDown = new Point(e.X, e.Y);
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
                SetSelectionBox(pDown, new Point(e.X,e.Y));
                HandleSelectionDone();
            }
        }

        protected override void SetSelectionBox(Point pDown, Point pCurrent)
        {
            double Radius = Math.Sqrt(Math.Pow((pOriginalDown.X - pCurrent.X), 2) + Math.Pow((pOriginalDown.Y - pCurrent.Y), 2)) / 2d;
            double Cx = (pOriginalDown.X + pCurrent.X) / 2;
            double Cy = (pOriginalDown.Y + pCurrent.Y) / 2;
            Point  pLeft = new Point((int)(Cx - Radius), (int)(Cy - Radius));

            mSelectionBox = new Rectangle(pLeft.X, pLeft.Y, (int)(2 * Radius), (int)(2 * Radius));
            
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

        protected override  void DrawCircleSelectionBox()
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

    }
    
}
