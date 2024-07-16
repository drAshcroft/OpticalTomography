using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer3D.Tools
{
    public abstract class aDrawingTool3D
    {
        protected ScreenProperties3D mScreenProperties;
        protected Rectangle mSelectionBox;

        public abstract string GetToolName();
        public abstract string GetToolTip();
        public virtual Bitmap GetToolImage()
        {
            return new Bitmap(24, 24);
        }

        public ScreenProperties3D ScreenProperties
        {
            set  {  mScreenProperties=value; }
        }

        public void MouseDown(ScreenProperties3D screenProperties, Point DownPoint, MouseEventArgs e)
        {
            mScreenProperties = screenProperties;
            MouseDownImpl(screenProperties, DownPoint, e);
        }
        public void MouseMove(Point MovePoint, MouseEventArgs e)
        {
            MouseMoveImpl(MovePoint, e);
        }
        public void MouseUp(Point MovePoint, MouseEventArgs e)
        {
            MouseUpImpl(MovePoint, e);
            NotifyOfSelection();
        }

        protected abstract void NotifyOfSelection();

        public void KeyDown(KeyEventArgs e)
        {
            KeyDownImpl(e);
            NotifyOfSelection();
        }
        public void KeyUp(KeyEventArgs e)
        {
            KeyUpImpl(e);
            NotifyOfSelection();
        }
        public void KeyPress(KeyPressEventArgs e)
        {
            KeyPressImpl(e);
            NotifyOfSelection();
        }

        protected virtual void KeyDownImpl(KeyEventArgs e)
        {

        }
        protected virtual void KeyUpImpl(KeyEventArgs e)
        {

        }
        protected virtual void KeyPressImpl(KeyPressEventArgs e)
        {

        }

        protected abstract void MouseDownImpl(ScreenProperties3D screenProperties, Point DownPoint, MouseEventArgs e);
        protected abstract void MouseMoveImpl(Point MovePoint, MouseEventArgs e);
        protected abstract void MouseUpImpl(Point MovePoint, MouseEventArgs e);

        protected Point PutPointInBounds(Point pDown)
        {
            if (pDown.X < 0)
                pDown.X = 0;
            if (pDown.X > mScreenProperties.ScreenCoords.Width)
                pDown.X = mScreenProperties.ScreenCoords.Width;
            if (pDown.Y < 0)
                pDown.Y = 0;
            if (pDown.Y > mScreenProperties.ScreenCoords.Height)
                pDown.Y = mScreenProperties.ScreenCoords.Height;
            return pDown;
        }
        protected Rectangle ChangeToImageCoords(Rectangle Selection)
        {
            if (mScreenProperties.DataEnvironment.Zooming == false)
            {
                Rectangle ViewBox = mScreenProperties.GetViewRectangle();

                //prevent illegal moves
                #region TooBig
                //Too big
                if (Selection.X < ViewBox.Left)
                {
                    Selection.X = ViewBox.X;
                    Selection.Width = ViewBox.Width;
                }
                if (Selection.Y < ViewBox.Top )
                {
                    Selection.Y = ViewBox.Y;
                    Selection.Height = ViewBox.Height;
                }
                //too little
                if (Selection.Right < ViewBox.Left)
                {
                    Selection.X = ViewBox.X;
                    Selection.Width = ViewBox.Width;
                }
                if (Selection.Right > ViewBox.Right)
                    Selection.Width = ViewBox.Right - Selection.X;


                if (Selection.Bottom < ViewBox.Top)
                {
                    Selection.Y = ViewBox.Y;
                    Selection.Height = ViewBox.Height;
                }
                if (Selection.Bottom > ViewBox.Bottom)
                    Selection.Height = ViewBox.Bottom - Selection.Y-1;
                #endregion
                //now check if just one corner is out
                #region Corners
                if (Selection.Left < ViewBox.Left && Selection.Right > ViewBox.Left)
                {
                    Selection.Width = Selection.Right - ViewBox.Left;
                    Selection.X = ViewBox.Left;
                }
                if (Selection.Top < ViewBox.Top && Selection.Bottom > ViewBox.Top)
                {
                    Selection.Height = Selection.Bottom - ViewBox.Top;
                    Selection.Y = ViewBox.Top;
                }

                if (Selection.Top >= ViewBox.Top && Selection.Bottom > ViewBox.Bottom)
                {
                    Selection.Height = ViewBox.Bottom - Selection.Top;
                }
                if (Selection.Right >= ViewBox.Right && Selection.Left > ViewBox.Left)
                {
                    Selection.Width = ViewBox.Right - Selection.X;
                }
                #endregion
                return Selection;
            }
            else
            {
                Rectangle ViewBox = mScreenProperties.GetViewRectangle();
                double ScreenWidth = mScreenProperties.ScreenFrontBuffer.Width;
                double ScreenHeight = mScreenProperties.ScreenFrontBuffer.Height;


                double cX1 = (double)Selection.X / (double)ScreenWidth;
                double cY1 = (double)Selection.Y / (double)ScreenHeight;
                double cX2 = (double)Selection.Right / (double)ScreenWidth;
                double cY2 = (double)Selection.Bottom / (double)ScreenHeight;

                if (cX1 < 0) cX1 = 0;
                if (cX1 > 1) cX1 = 1;

                if (cX2 < 0) cX2 = 0;
                if (cX2 > 1) cX2 = 1;

                if (cY1 < 0) cX1 = 0;
                if (cY1 > 1) cX1 = 1;

                if (cY2 < 0) cY2 = 0;
                if (cY2 > 1) cY2 = 1;

                double t = 0;
                if (cX1 > cX2)
                {
                    t = cX2;
                    cX2 = cX1;
                    cX1 = t;
                }
                if (cY1 > cY2)
                {
                    t = cY2;
                    cY2 = cY1;
                    cY1 = t;
                }

                cX1 = cX1 * ViewBox.Width + ViewBox.Left;
                cX2 = cX2 * ViewBox.Width + ViewBox.Left;

                cY1 = cY1 * ViewBox.Height + ViewBox.Top ;
                cY2 = cY2 * ViewBox.Height + ViewBox.Top;

                return Rectangle.FromLTRB((int)cX1,(int) cY1,(int) cX2,(int) cY2);
            }

        }

        protected virtual void SetSelectionBox(Point pDown, Point pCurrent)
        {
            int left, right, top, bottom;
            if (pDown.X < pCurrent.X)
            {
                left = pDown.X;
                right = pCurrent.X;
            }
            else
            {
                right = pDown.X;
                left = pCurrent.X;
            }
            if (pDown.Y < pCurrent.Y)
            {
                top = pDown.Y;
                bottom = pCurrent.Y;
            }
            else
            {
                top = pCurrent.Y;
                bottom = pDown.Y;
            }
            mSelectionBox.X = left;
            mSelectionBox.Y = top;
            mSelectionBox.Width = right - left;
            mSelectionBox.Height = bottom - top;
        }

        protected virtual void ClearSelectionBox(Rectangle SelectionBox)
        {
            try
            {
               
                SelectionBox.Inflate(2, 2);
                Graphics g = Graphics.FromImage(mScreenProperties.ScreenFrontBuffer);
                g.DrawImage(mScreenProperties.ScreenBackBuffer, SelectionBox, SelectionBox, GraphicsUnit.Pixel);
                mScreenProperties.ForceImageDisplay();
            }
            catch { }
        }

        protected virtual void ClearSelectionBox(Rectangle SelectionBox, int Padding)
        {
            SelectionBox.Inflate(Math.Abs(Padding), Math.Abs(Padding));
            Graphics g = Graphics.FromImage(mScreenProperties.ScreenFrontBuffer);
            g.DrawImage(mScreenProperties.ScreenBackBuffer, SelectionBox, SelectionBox, GraphicsUnit.Pixel);
            mScreenProperties.ForceImageDisplay();
        }

        public void EraseSelection()
        {
            try
            {
                ClearSelectionBox(mSelectionBox);
            }
            catch { }
        }
        protected virtual void DrawSelectionBox(Rectangle SelectionBox)
        {
            if (!(SelectionBox == null || mScreenProperties == null || SelectionBox.Width == 0 || SelectionBox.Height == 0))
            {
                Graphics g = Graphics.FromImage(mScreenProperties.ScreenFrontBuffer);
                g.DrawRectangle(Pens.Red, mSelectionBox);
                mScreenProperties.ForceImageDisplay();
            }
        }

        public abstract void RedrawSelection();

    }
}
