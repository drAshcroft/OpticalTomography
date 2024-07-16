using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageViewer.Filters.SelectionEffects
{
    public class BlobDescription
    {
        public int BlobIndex;
        private Rectangle mBounds;
        private Point Center;
        public Rectangle BlobBounds
        {
            get { return mBounds ;}
        }
        public Point CenterOfGravity
        {
            get { return Center ; }
        }
        public BlobDescription(int Index, Rectangle Bounds, Point Center)
        {
            BlobIndex = Index;
            mBounds = Bounds;
            this.Center = Center;
        }
    }
}
