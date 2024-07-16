using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer.Tools
{
    public abstract  class aDrawingTool
    {
        protected ScreenProperties mScreenProperties;
        protected Rectangle mSelectionBox;

        public abstract  string GetToolName();
        public abstract string GetToolTip();
        public virtual Bitmap GetToolImage()
        {
            return new Bitmap(24, 24);
        }

        public void MouseDown(ScreenProperties screenProperties, Point DownPoint, MouseEventArgs e)
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

        protected abstract void  NotifyOfSelection();

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

        protected abstract void MouseDownImpl(ScreenProperties screenProperties, Point DownPoint, MouseEventArgs e);
        protected abstract void MouseMoveImpl(Point MovePoint, MouseEventArgs e);
        protected abstract void MouseUpImpl(Point MovePoint, MouseEventArgs e);

        protected Point PutPointInBounds(Point pDown)
        {
            if (pDown.X < 0)
                pDown.X = 0;
            if (pDown.X > mScreenProperties.ScreenCoords .Width)
                pDown.X = mScreenProperties.ScreenCoords.Width;
            if (pDown.Y < 0)
                pDown.Y = 0;
            if (pDown.Y > mScreenProperties.ScreenCoords.Height)
                pDown.Y = mScreenProperties.ScreenCoords.Height;
            return pDown;
        }
        protected virtual  void SetSelectionBox(Point pDown, Point pCurrent)
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
        public abstract void  RedrawSelection();

    }
}
