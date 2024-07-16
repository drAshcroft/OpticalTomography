using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathHelpLib;
using System.Drawing;

namespace ImageViewer3D
{
    public class Data3D
    {
        private ScreenProperties3D[] mScreens = new ScreenProperties3D[3];
        private double mMinPixelValue;
        private double mMaxPixelValue;

        private double mContrastMin;
        private double mContrastMax;

        public static double[, ,] mImageData;

        public Data3D(double[,,] Data, ScreenProperties3D[] Screens)
        {
            mImageData = Data;
            mMinPixelValue = Data.MinArray();
            mMaxPixelValue = Data.MaxArray();
            mContrastMin = mMinPixelValue;
            mContrastMax = mMaxPixelValue;
            mScreens = Screens;
            for (int i = 0; i < mScreens.Length; i++)
                mScreens[i].OriginalData = this;
        }

        public double ContrastMin
        {
            get { return mContrastMin; }
            set { mContrastMin = value; }
        }

        public double ContrastMax
        {
            get { return mContrastMax; }
            set { mContrastMax = value; }
        }

        public double[, ,] Data
        {
            get { return mImageData ; }
            set
            {
                mImageData  = value;
            }
        }

        public ScreenProperties3D[] Screens
        {
            get { return mScreens; }
        }

        public void SetGlobalZoom(Axis SourceAxis, Rectangle zoomBox, double MagX, double MagY)
        {
            if (SourceAxis == Axis.ZAxis)
            {
                double left = zoomBox.X * MagX;
                double top = zoomBox.Y * MagY;

                mScreens[2].VirtualPictureBox.Width = (int)(mScreens[2].ViewPictureSize.Width * MagX);
                mScreens[1].VirtualPictureBox.Height = (int)(mScreens[1].ViewPictureSize.Height * MagY);
                mScreens[2].VirtualPictureBox.X = (int)(-1 * left);
                mScreens[1].VirtualPictureBox.Y = (int)(-1 * top);

                mScreens[1].RedrawBuffers();
                mScreens[2].RedrawBuffers();
            }
            else if (SourceAxis == Axis.XAxis)
            {
                double left = zoomBox.X * MagX;
                double top = zoomBox.Y * MagY;

                mScreens[2].VirtualPictureBox.Height  = (int)(mScreens[2].ViewPictureSize.Height  * MagX);
                mScreens[0].VirtualPictureBox.Height   = (int)(mScreens[0].ViewPictureSize.Height   * MagY);
                mScreens[2].VirtualPictureBox.Y = (int)(-1 * top);
                mScreens[0].VirtualPictureBox.Y = (int)(-1 * top );

                mScreens[0].RedrawBuffers();
                mScreens[2].RedrawBuffers();

            }
            else
            {
                double left = zoomBox.X * MagX;
                double top = zoomBox.Y * MagY;

                mScreens[0].VirtualPictureBox.Width = (int)(mScreens[0].ViewPictureSize.Width * MagX);
                mScreens[1].VirtualPictureBox.Height = (int)(mScreens[1].ViewPictureSize.Height * MagY);
                mScreens[0].VirtualPictureBox.X = (int)(-1 * left);
                mScreens[1].VirtualPictureBox.Y = (int)(-1 * top);

                mScreens[1].RedrawBuffers();
                mScreens[0].RedrawBuffers();

            }
        }

        public void ChangeSlice(Axis axis, int Slice)
        {
            if (axis == Axis.ZAxis)
            {
                if (Slice <mImageData.GetLength(2) && Slice>0)
                    Screens[0].SliceIndex = Slice;
            }
            else if (axis == Axis.XAxis)
            {
                if (Slice < mImageData.GetLength(0) && Slice > 0)
                    Screens[1].SliceIndex = Slice;
            }
            else
            {
                if (Slice < mImageData.GetLength(1) && Slice > 0)
                    Screens[2].SliceIndex = Slice;
            }

        }
    }
}
