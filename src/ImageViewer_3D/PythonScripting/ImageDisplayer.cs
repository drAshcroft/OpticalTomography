using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageViewer3D.PythonScripting
{
    public class ImageDisplayer
    {
        ScreenProperties3D[] mScreens;
        public ImageDisplayer(ScreenProperties3D[] Screens)
        {
            mScreens = Screens;
        }
        public void DisplayImage(Bitmap newImage)
        {
            for (int i = 0; i < mScreens.Length; i++)
            {
                try
                {
                   // mScreens[i].ActiveSelectedData = newImage;
                }
                catch { }
            }
        }

    }
}
