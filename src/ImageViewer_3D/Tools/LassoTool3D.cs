using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer3D.Tools
{
    public class ROITool3DBeatriz : aDrawingTool3D
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

        Point current = new Point();
        Point last = new Point();
        Point First = new Point();
        List<Point> list;// = new List<int>();
        protected override void MouseDownImpl(ScreenProperties3D screenProperties, Point DownPoint, System.Windows.Forms.MouseEventArgs e)
        {
            First = new Point(e.X, e.Y);
            current = First;
            mSelectionBox = new Rectangle(First, new Size(3, 3));
            list.Add(First);
            
        }
        
        
        protected override void MouseMoveImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                last = current;
                current = new Point(e.X, e.Y);
                SetSelectionBox(last,current);
                DrawSelectionBox(mSelectionBox);
               
                list.Add(current);
                
                
            }
        }

        protected override void DrawSelectionBox(Rectangle SelectionBox)
        {
            Graphics g = Graphics.FromImage(mScreenProperties.ScreenFrontBuffer);
            g.DrawLine(Pens.Red, last, current);
            mScreenProperties.ForceImageDisplay();
        }

        protected override void SetSelectionBox(Point pDown, Point pCurrent)
        {
            if (mSelectionBox.Contains(pCurrent) == false)
            {
                if (pCurrent.Y < mSelectionBox.Top)
                {
                    mSelectionBox.Height += (mSelectionBox.Top - pCurrent.Y);
                    mSelectionBox.Y = pCurrent.Y;
                }
                else if (pCurrent.Y > mSelectionBox.Bottom)
                    mSelectionBox.Height = mSelectionBox.Height + (pCurrent.Y - mSelectionBox.Bottom);

                if (pCurrent.X < mSelectionBox.Left)
                {
                    mSelectionBox.Width += (mSelectionBox.Left - pCurrent.X);
                    mSelectionBox.X = pCurrent.X;
                }

                else if (pCurrent.X > mSelectionBox.Right)
                    mSelectionBox.Width += (pCurrent.X - mSelectionBox.Right);

            }
        }

        protected override void MouseUpImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
               // ClearSelectionBox(mSelectionBox);
                Point pUp = PutPointInBounds(new Point(e.X, e.Y));

                SetSelectionBox(First, pUp);
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
