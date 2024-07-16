using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageViewer3D
{
    public  class PictureDisplay: PictureBox 
    {
        ScreenProperties3D mScreenProperties;
        private int mIndex;
        public int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }
        public ScreenProperties3D ScreenProperties
        {
            get { return mScreenProperties; }
            set { mScreenProperties = value;  }
        }
        
    }
}
