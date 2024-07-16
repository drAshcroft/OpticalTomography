using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageViewer.Selections
{
    public class CircleSelection:ROISelection 
    {
        protected int mPaddingInner=0, mPaddingOuter=0;
      
        public CircleSelection(Rectangle selectionBounds, int PaddingInner, int PaddingOuter, int WindowIndex ):base(selectionBounds, WindowIndex )
        {
            mPaddingInner = PaddingInner;
            mPaddingOuter = PaddingOuter;
        }
        public Rectangle InnerCircleBounds
        {
            get 
            {
                Rectangle tempR = new Rectangle(mSelectionBox.X, mSelectionBox.Y, mSelectionBox.Width, mSelectionBox.Height);
                tempR.Inflate(mPaddingInner, mPaddingInner);
                return tempR;
            }
        }
        public Rectangle OuterCircleBounds
        {
            get 
            {
                Rectangle tempR = new Rectangle(mSelectionBox.X, mSelectionBox.Y, mSelectionBox.Width, mSelectionBox.Height);
                tempR.Inflate(mPaddingOuter, mPaddingOuter);
                return tempR;
            }
        }
        public double InnerRadius
        {
            get 
            {
                if (mSelectionBox.Width < mSelectionBox.Height)
                    return mSelectionBox.Height/2 + mPaddingInner;
                else
                    return mSelectionBox.Width/2 + mPaddingInner;
            }
        }
        public double OuterRadius
        {
            get 
            {
                if (mSelectionBox.Width < mSelectionBox.Height)
                    return mSelectionBox.Height/2 + mPaddingOuter;
                else
                    return mSelectionBox.Width/2 + mPaddingOuter;

            }
        }
        
        public override   bool PointInSelection(Point TestPoint)
        {
            double R2 = OuterRadius * OuterRadius;
            Point center = ImageCenter;
            double x = TestPoint.X - center.X;
            double y = TestPoint.Y - center.Y;

            double r = x * x + y * y;
            if (InnerRadius == OuterRadius)
            {
                if (r < R2) return true;
            }
            else
            {
                if (InnerRadius*InnerRadius  <= r && r <= R2 )
                    return true;
            }
            return false;

        }

        public override object Clone()
        {
            return new CircleSelection(SelectionBounds, mPaddingInner, mPaddingOuter, mWindowIndex);
        }

        public override void BringToZero()
        {
            base.BringToZero();
        }
    }
}
