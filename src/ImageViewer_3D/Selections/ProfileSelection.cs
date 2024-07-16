using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageViewer3D.Selections
{
    public class ProfileSelection:ROISelection 
    {
        Point mPoint1;
        Point mPoint2;
        public ProfileSelection(Rectangle selectionBounds,Point p1,Point p2,int WindowIndex):base(selectionBounds,WindowIndex )
        {
            mPoint1 = p1;
            mPoint2 = p2;
        }
        public Point P1
        {
            get { return mPoint1; }
        }
        public Point P2
        {
            get { return mPoint2; }
        }
        public override object Clone()
        {
            return new ProfileSelection(SelectionBounds, mPoint1, mPoint2, mWindowIndex);
        }

        public override void BringToZero()
        {
            mPoint1.X -= mSelectionBox.X;
            mPoint1.Y -= mSelectionBox.Y;

            mPoint2.X -= mSelectionBox.X;
            mPoint2.Y -= mSelectionBox.Y;
            base.BringToZero();
        }
    }
}
