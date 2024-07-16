using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageViewer3D.PythonScripting
{
    public class ImageDisplayer3D
    {
        DataEnvironment3D mScreens;
        public ImageDisplayer3D(DataEnvironment3D Screens)
        {
            mScreens = Screens;
        }

        public void DisplayImage(int index, DataHolder  newImage)
        {
            try
            {
                mScreens.ActiveSelectedImage = (newImage);
            }
            catch { }
        }

        public void DisplayImage(int index, float[,,] newImage)
        {
            try
            {
                mScreens.ActiveSelectedImage = new DataHolder(newImage);
            }
            catch { }
        }

        public void DisplayImage(int index, double[,,] newImage)
        {
            try
            {

                mScreens.ActiveSelectedImage =new DataHolder (newImage);
            }
            catch { }
        }

    }
}
