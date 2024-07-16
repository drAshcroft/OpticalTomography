using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using MathHelpLib;

namespace ImageViewer.PythonScripting
{
    public class ImageDisplayer
    {
        DataEnvironment mScreens = null;
        PictureBox mPictureBox;
        public ImageDisplayer(PictureBox pictureBox)
        {
            mPictureBox = pictureBox;
        }
        public ImageDisplayer(DataEnvironment Screens)
        {
            mScreens = Screens;
        }

        private delegate void DisplayBitmapImage(int index, Bitmap newImage);
        private delegate void DisplayImageHolder(int index, ImageHolder newImage);

        public void DisplayImage(int index, ImageHolder newImage)
        {
            if (index % 5 == 0)
            {

                if (mPictureBox.InvokeRequired)
                {
                    mPictureBox.BeginInvoke(new DisplayImageHolder(DisplayImage), index, newImage);
                }
                else
                {

                    try
                    {
                        if (mScreens != null)
                        {
                            mScreens.Screen.ActiveSelectedImage = (newImage);
                        }
                        if (mPictureBox != null)
                        {
                            lock (mPictureBox)
                            {
                                mPictureBox.Image = newImage.ToBitmap();
                                mPictureBox.Invalidate();

                            }
                        }
                    }
                    catch { }
                   // Application.DoEvents();
                }
            }
        }


        public void DisplayImage(int index, Bitmap newImage)
        {
            if (index % 5 == 0)
            {
                if (mPictureBox.InvokeRequired)
                {
                    mPictureBox.BeginInvoke(new DisplayBitmapImage(DisplayImage), index, newImage);
                }
                else
                {
                    try
                    {
                        if (mScreens != null)
                            mScreens.Screen.ActiveSelectedImage = new ImageHolder(newImage);

                        if (mPictureBox != null)
                        {
                            lock (mPictureBox)
                            {
                                mPictureBox.Image = newImage;
                                mPictureBox.Invalidate();

                            }
                        }
                    }
                    catch { }
                   // Application.DoEvents();
                }
            }
        }

    }
}
