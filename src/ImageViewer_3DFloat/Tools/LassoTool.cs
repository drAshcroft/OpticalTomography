using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer3D.Tools
{
    public class LassoSelectTool : aDrawingTool3D
    {
        #region Labeling
        public override string GetToolName()
        {
            return "Lasso";
        }

        public override string GetToolTip()
        {
            return "Selected a region of the image";
        }

        #endregion

        Point current = new Point();
        Point last = new Point();
        Point First = new Point();


        private List<Point> list = new List<Point>();
        private List<Point[]> inactive = new List<Point[]>();


        /// <summary>
        /// Draws the active curve. Points must be in unscale coords
        /// </summary>
        public List<PointF> DrawnCurve
        {
            get
            {
                if (mScreenProperties.Zooming)
                {
                    double ZX = mScreenProperties.ZoomX;
                    double ZY = mScreenProperties.ZoomY;
                    double oX = mScreenProperties.ViewX;
                    double oY = mScreenProperties.ViewY;

                    List<PointF> OutPoints = new List<PointF>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        OutPoints.Add(new PointF((float)(list[i].X / ZX + oX), (float)(list[i].Y / ZY + oY)));
                    }
                    return OutPoints;
                }
                else
                {
                    List<PointF> OutPoints = new List<PointF>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        OutPoints.Add(new PointF((float)(list[i].X ), (float)(list[i].Y)));
                    }
                    return OutPoints;
                }
            }

            set
            {
                if (mScreenProperties.Zooming)
                {
                    double ZX = mScreenProperties.ZoomX;
                    double ZY = mScreenProperties.ZoomY;
                    double oX = mScreenProperties.ViewX;
                    double oY = mScreenProperties.ViewY;

                    list.Clear();
                    List<PointF> InPoints = value;

                    if (value != null)
                    {
                        for (int i = 0; i < InPoints.Count; i++)
                        {
                            list.Add(new Point((int)((InPoints[i].X - oX) * ZX), (int)((InPoints[i].Y - oY) * ZY)));
                        }
                    }
                }
                else
                {
                    list.Clear();
                    List<PointF> InPoints = value;

                    if (value != null)
                    {
                        for (int i = 0; i < InPoints.Count; i++)
                        {
                            list.Add(new Point((int)(InPoints[i].X), (int)(InPoints[i].Y )));
                        }
                    }
                }
            }
        }

        public void InactiveClear()
        {
            inactive.Clear();
        }

        public void InactiveAdd(PointF[] curve)
        {
            Point[] OutCurve = null;
            if (mScreenProperties.Zooming)
            {
                double ZX = mScreenProperties.ZoomX;
                double ZY = mScreenProperties.ZoomY;
                double oX = mScreenProperties.ViewX;
                double oY = mScreenProperties.ViewY;

                OutCurve = new Point[curve.Length];
               
                for (int i = 0; i < curve.Length; i++)
                {
                    OutCurve[i]= new Point((int)((curve[i].X - oX) * ZX), (int)((curve[i].Y - oY) * ZY));
                }
            }
            else
            {
                OutCurve = new Point[curve.Length];

                for (int i = 0; i < curve.Length; i++)
                {
                    OutCurve[i] = new Point((int)(curve[i].X ), (int)(curve[i].Y ));
                }
                
            }

            inactive.Add(OutCurve);
        }


        public void SetOnAxis(ScreenProperties3D screen)
        {
            mScreenProperties = screen;
            mSelectionBox = new Rectangle(0, 0, 1000, 1000);
        }

        protected override void MouseDownImpl(ScreenProperties3D screenProperties, Point DownPoint, System.Windows.Forms.MouseEventArgs e)
        {
            First = new Point(e.X, e.Y);
            current = First;
            last = First;
            mSelectionBox = new Rectangle(First, new Size(3, 3));

            list = new List<Point>();
            list.Add(First);

        }


        protected override void MouseMoveImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                last = current;
                current = new Point(e.X, e.Y);
                SetSelectionBox(last, current);
                DrawSelectionBox(mSelectionBox);
                try
                {
                    list.Add(current);
                }
                catch { }
            }
        }

        protected override void DrawSelectionBox(Rectangle SelectionBox)
        {
            if (mScreenProperties != null)
            {
                int i;
                Graphics g = Graphics.FromImage(mScreenProperties.ScreenFrontBuffer);
                for (i = 0; i < list.Count - 1; i++)
                {
                    //g.DrawLine(Pens.Red, last, current);
                    g.DrawLine(Pens.Red, list[i], list[i + 1]);
                }
                if (inactive != null)
                {
                    int j;
                    for (j = 0; j < inactive.Count; j++)
                        for (i = 0; i < inactive[j].Length - 1; i++)
                        {
                            g.DrawLine(Pens.Yellow, inactive[j][i], inactive[j][i + 1]);
                        }
                }

                mScreenProperties.ForceImageDisplay();
            }
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
                last = First;
                // ClearSelectionBox(mSelectionBox);
                //Point pUp = PutPointInBounds(new Point(e.X, e.Y));
                current = PutPointInBounds(new Point(e.X, e.Y));
                list.Add(last);
                //SetSelectionBox(First, pUp);
                SetSelectionBox(First, current);
                //current = pUp;
                //DrawSelectionBox(mSelectionBox);
                HandleSelectionDone(mSelectionBox);

            }
        }

        protected override void NotifyOfSelection()
        {
            Selections.LassoSelection3D roi = new Selections.LassoSelection3D(
                mScreenProperties.ConvertScreenToImage(mSelectionBox),
                ((PictureDisplay3DSlice)mScreenProperties.PictureBox).Index,
                mScreenProperties.SliceIndex,
                mScreenProperties.Axis, DrawnCurve );

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
    /*
    public class ROITool3DBeatriz : aDrawingTool3D
    {
        #region Labeling
        public override string GetToolName()
        {
            return "Lasso";
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
    */
}
