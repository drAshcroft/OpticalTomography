using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

using ImageViewer3D.Meshes;
using ImageViewer3D._3DItems;

namespace ImageViewer3D
{
    public class PictureDisplay3DSlice : PictureBox
    {
        public delegate void ImageUpdatedEvent();
        public event ImageUpdatedEvent ImageUpdated;
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
            set { mScreenProperties = value; }
        }

        public void SetImage()
        {
            try
            {
                this.Image = mScreenProperties.ScreenFrontBuffer;
                this.Invalidate();
               
            }
            catch { }
        }

        private Bitmap mUnzoomedImage;
        public Bitmap UnZoomedImage
        {
            get { return mUnzoomedImage; }
            set
            {
                mUnzoomedImage = value;

                if (ImageUpdated != null)
                    ImageUpdated();
            }
        }

        public void SetImage(Bitmap OriginalImage)
        {

            try
            {

                this.Image = OriginalImage;
                this.Invalidate();
                
            }
            catch { }
        }

        private Color mBorderColor = Color.LightGray;
        public Color BorderColor
        {
            get { return mBorderColor; }
            set { mBorderColor = value; }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            /* if (this.BackColor == Color.LightGray)
                 ControlPaint.DrawBorder(pe.Graphics, pe.ClipRectangle, Color.Gray, ButtonBorderStyle.Outset );
             else
                 ControlPaint.DrawBorder(pe.Graphics, pe.ClipRectangle, mBorderColor, ButtonBorderStyle.Outset);*/
        }



    }
}
