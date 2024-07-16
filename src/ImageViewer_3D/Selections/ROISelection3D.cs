using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageViewer3D.Selections
{
    public class ROISelection3D:ISelection3D 
    {
        protected Rectangle  mSelectionBox;
        protected int mSliceNumber;
        protected int mSliceAxis;
        public ROISelection3D(Rectangle selectionBounds, int WindowIndex ,int SliceNumber,int SliceAxis)
        {
            mSelectionBox = new Rectangle(selectionBounds.X, selectionBounds.Y, selectionBounds.Width, selectionBounds.Height);
            mWindowIndex = WindowIndex;
            mSliceAxis = SliceAxis;
            mSliceNumber = SliceNumber;
        }

        public int SelectionAxis
        {
            get { return mSliceAxis ;}
        }
        public int SelectionSliceNumber
        {
            get { return mSliceNumber; }
        }
        public Rectangle SelectionBounds
        {
            get { return mSelectionBox; }
        }
        public Point ImageCenter
        {
            get
            {
                return new Point((int)(mSelectionBox.X + (double)mSelectionBox.Width / 2d), (int)(mSelectionBox.Y + (double)mSelectionBox.Height / 2d));
            }
        }
        public virtual bool PointInSelection(Point TestPoint)
        {
          
            if (mSelectionBox.X <= TestPoint.X && TestPoint.X <= mSelectionBox.Right)
                if (mSelectionBox.Y <= TestPoint.Y && TestPoint.Y <= mSelectionBox.Bottom)
                    return true;
            return false;
        }

        protected int mWindowIndex;
        public int WindowIndex
        {
            get { return mWindowIndex; }
            set { mWindowIndex = value; }
        }

        public virtual object Clone()
        {
            return new ROISelection3D(mSelectionBox, mWindowIndex, mSliceNumber,mSliceAxis );
        }
        public virtual void BringToZero()
        {
            mSelectionBox.X = 0;
            mSelectionBox.Y = 0;
        }
    }
}
