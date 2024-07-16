using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageViewer3D
{
    public partial class hArrowSlider : UserControl
    {
        public delegate void ValueChangedEvent(double newValue);
        public event ValueChangedEvent ValueChanged;
        public hArrowSlider()
        {
            InitializeComponent();
            mMaxPixel = this.Width;
        }

        private double mMaximum = 100;
        private double mMinimum = 0;
        private double mCurrentValue = 50;
        private Color mSliderColor = Color.Blue;

        private int mMinPixel = 0;
        private int mMaxPixel;
        private bool mCustomPixelRange = false;
        public int MinPixel
        {
            get { return mMinPixel; }
            set
            {
                mMinPixel = value;
                mCustomPixelRange = true;
            }
        }

        /// <summary>
        /// set should only be used when resizing is no longer desired.  
        /// </summary>
        public int MaxPixel
        {
            get { return mMaxPixel; }
            set
            {
                mMaxPixel = value;
                mCustomPixelRange = true;
            }
        }

        public double Maximum
        {
            get { return mMaximum; }
            set { mMaximum = value; }
        }
        public double Minimum
        {
            get { return mMinimum; }
            set { mMinimum = value; }
        }
        public double Value
        {
            get { return mCurrentValue; }
            set { mCurrentValue = value; }
        }
        public Color SliderColor
        {
            get { return mSliderColor; }
            set { mSliderColor = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(mSliderColor), new Rectangle((int)((Value - Minimum) / (Maximum-Minimum) * (double)(mMaxPixel - mMinPixel) + mMinPixel) - 2, 0, 5, this.Height));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
            if (mCustomPixelRange == false)
                mMaxPixel = this.Width;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mCurrentValue = (Maximum - Minimum) * ((double)e.X - mMinPixel) / (double)(mMaxPixel - mMinPixel) + Minimum;
                if (mCurrentValue > Maximum)
                    mCurrentValue = Maximum;
                if (mCurrentValue < Minimum)
                    mCurrentValue = Minimum;
                if (ValueChanged != null)
                    ValueChanged(mCurrentValue);

                this.Invalidate();
            }
            base.OnMouseMove(e);
        }
    }
}
