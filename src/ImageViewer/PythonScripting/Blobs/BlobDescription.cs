using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageViewer.Filters.Blobs
{
    public class BlobDescription
    {
        public int BlobIndex;
        private Rectangle mBounds;
        private Point Center;
        
        /// <summary>
        /// The complete bounds of the blob as defined by the threshold used.   
        /// </summary>
        public Rectangle BlobBounds
        {
            get { return mBounds ;}
        }

        /// <summary>
        /// The center of gravity of the blob defined by the threshold.  
        /// </summary>
        public Point CenterOfGravity
        {
            get { return Center ; }
        }

        /// <summary>
        /// Index can either be the order that the blob is found or the 
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Bounds"></param>
        /// <param name="Center"></param>
        public BlobDescription(int Index, Rectangle Bounds, Point Center)
        {
            BlobIndex = Index;
            mBounds = Bounds;
            this.Center = Center;
        }
    }
}
