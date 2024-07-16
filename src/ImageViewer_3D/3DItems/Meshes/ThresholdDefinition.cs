using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer3D.Meshes
{
    public class Thresholds:IComparable <Thresholds>
    {
        private PictureBox  mScreencontrol;
        private double mMiddle;
        private double mHWidth;
        private double mMin, mMax, mLower,mUpper;
        private Color mColor;
        private byte mRed, mGreen, mBlue;

        public int CompareTo(Thresholds  other)
        {
            return mMiddle.CompareTo(other.mMiddle );
        }

        public Thresholds(double Middle, double HalfWidth, double MinValue, double MaxValue, PictureBox  ScreenControl, Color color)
        {
            mScreencontrol = ScreenControl;
            mMiddle = Middle;
            mHWidth = HalfWidth;
            mLower = mMiddle - mHWidth;
            mUpper = mMiddle + mHWidth;
            mMin = MinValue;
            mMax = MaxValue;
            mColor = color;
            mRed = mColor.R;
            mBlue = mColor.B;
            mGreen = mColor.G;
        }
        public Color PixelColor
        {
            get { return mColor; }
            set 
            { 
                mColor = value;
                mRed = mColor.R;
                mBlue = mColor.B;
                mGreen = mColor.G;
            }
        }
        public byte Red
        {
            get { return mRed; }
        }
        public byte Blue
        {
            get { return mBlue; }
        }
        public byte Green
        {
            get { return mGreen; }
        }

        public int LeftScreenEdge
        {
            get { return (int)(((mMiddle - mHWidth) - mMin) / (mMax - mMin) * mScreencontrol.Width ); }
        }
        public int RightScreenEdge
        {
            get { return (int)(((mMiddle + mHWidth) - mMin) / (mMax - mMin) * mScreencontrol.Width ); }
        }
        public int MiddleScreen
        {
            get { return (int)(((mMiddle) - mMin) / (mMax - mMin) * mScreencontrol.Width ); }
        }
        public double ScreenToIntensity(int ScreenValue)
        {
            return (double)ScreenValue /(double) mScreencontrol.Width  * (mMax - mMin) + mMin;
        }
        public double HalfWidth
        {
            set 
            { 
                mHWidth = value;
                mLower = mMiddle - mHWidth;
                mUpper = mMiddle + mHWidth;

            }
            get { return mHWidth; }
        }
        public double Middle
        {
            set 
            { 
                mMiddle = value;
                mLower = mMiddle - mHWidth;
                mUpper = mMiddle + mHWidth;

            }
            get { return mMiddle; }
        }

        public bool IsInRange(double IntensityValue)
        {
            if (IntensityValue > mLower && IntensityValue < mUpper)
            {
                return true;
            }
            else
                return false;
        }
    }
}
