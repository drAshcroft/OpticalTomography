using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageViewer.Selections
{
    public class CircleAndLineSelection:CircleSelection 
    {
        protected Point mLineEnd;
        public CircleAndLineSelection(Rectangle selectionBounds, int PaddingInner, int PaddingOuter, Point LineEnd,int WindowIndex)
            : base(selectionBounds, PaddingInner, PaddingOuter, WindowIndex )
        {
            mLineEnd = LineEnd;
        }

        public Point LineEnd
        {
            get { return mLineEnd; }
        }
        public Point LineBegin
        {
            get { return ImageCenter; }
        }
        public override object Clone()
        {
            return new CircleAndLineSelection(SelectionBounds, mPaddingInner, mPaddingOuter, mLineEnd, mWindowIndex);
        }

        public override void BringToZero()
        {
            mLineEnd.X -= mSelectionBox.X;
            mLineEnd.Y -= mSelectionBox.Y;
            base.BringToZero();
        }
    }
}
