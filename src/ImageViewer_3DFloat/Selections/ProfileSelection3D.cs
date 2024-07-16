using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageViewer3D.Selections
{
    public class ProfileSelection3D:ROISelection3D 
    {
        Point mPoint1;
        Point mPoint2;
        public ProfileSelection3D(Rectangle selectionBounds,Point p1,Point p2,int WindowIndex,int Axis,int SliceNumber):base(selectionBounds,WindowIndex,SliceNumber,Axis  )
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
            return new ProfileSelection3D(SelectionBounds, mPoint1, mPoint2, mWindowIndex,mSliceAxis ,mSliceNumber );
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
