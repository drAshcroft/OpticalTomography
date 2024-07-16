using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ImageViewer.Filters
{
    public partial class ContrastTool : Form,IEffect 
    {
        public ContrastTool()
        {
            InitializeComponent();
        }
        public string EffectName { get { return "Contrast and Brightness"; } }
        public string EffectMenu { get { return "Effects"; } }
        public string EffectSubMenu { get { return ""; } }
        public IEffectToken CurrentProperties { get { return mContrastToken; } }

        ScreenProperties[] mSourceImage;
        IEffectToken mContrastToken;
        Bitmap ScratchImage = null;
        Bitmap holding = null;
        public void  RunEffect(ScreenProperties[]  SourceImage, IEffectToken ContrastToken)
        {
            if (ContrastToken == null)
            {
                ContrastToken = new GeneralToken();
                ContrastToken.Parameters = new double[2];
                ContrastToken.Parameters[0] = .5;
                ContrastToken.Parameters[1] = .5;
            }
            
            mSourceImage = SourceImage;
            mContrastToken = ContrastToken;
            ScratchImage = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format32bppArgb );

            Bitmap ShowImage = SourceImage[0].ActiveSelectedImage;
            Graphics.FromImage(ScratchImage).DrawImage(ShowImage , 
                new Rectangle(0, 0, ScratchImage.Width, ScratchImage.Height), 
                new Rectangle(0, 0, ShowImage.Width, ShowImage.Height), GraphicsUnit.Pixel);

            holding = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format32bppArgb);
            DoRun();
        }
        private void DoRun()
        {
            Graphics.FromImage(holding).DrawImage(ScratchImage,0,0);

            if (mContrastToken.Parameters[0]!=0)
                Brightness(holding ,(int) mContrastToken.Parameters[0]);

            if (mContrastToken.Parameters[1]!=0)
                Contrast(holding, (int)mContrastToken.Parameters[1]);

            pictureBox1.Image = holding;
            pictureBox1.Invalidate();
        }
        public static bool Brightness(Bitmap b, int nBrightness)
        {
            if (nBrightness < -255 || nBrightness > 255)
                return false;

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            int nVal = 0;

            unsafe
            {
                for (int y = 0; y < b.Height; ++y)
                {
                    byte* p = (byte*)(void*)Scan0 + bmData.Stride *y ;
                    for (int x = 0; x < b.Width ; ++x)
                    {
                        nVal = (int)(p[0] + nBrightness);
                        if (nVal < 0) nVal = 0;
                        if (nVal > 255) nVal = 255;
                        p[0] = (byte)nVal;

                        nVal = (int)(p[1] + nBrightness);
                        if (nVal < 0) nVal = 0;
                        if (nVal > 255) nVal = 255;
                        p[1] = (byte)nVal;

                        nVal = (int)(p[2] + nBrightness);
                        if (nVal < 0) nVal = 0;
                        if (nVal > 255) nVal = 255;
                        p[2] = (byte)nVal;

                        p+=4;
                    }
                }
            }

            b.UnlockBits(bmData);

            return true;
        }

        public static bool Contrast(Bitmap b, int  nContrast)
        {
            if (nContrast < -100) return false;
            if (nContrast > 100) return false;

            double pixel = 0, contrast = (100.0 + nContrast) / 100.0;

            contrast *= contrast;

            int red, green, blue;

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                for (int y = 0; y < b.Height; ++y)
                {
                    byte* p = (byte*)(void*)Scan0 + bmData.Stride * y;
                    for (int x = 0; x < b.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];

                        pixel = red / 255.0;
                        pixel -= 0.5;
                        pixel *= contrast;
                        pixel += 0.5;
                        pixel *= 255;
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;
                        p[2] = (byte)pixel;

                        pixel = green / 255.0;
                        pixel -= 0.5;
                        pixel *= contrast;
                        pixel += 0.5;
                        pixel *= 255;
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;
                        p[1] = (byte)pixel;

                        pixel = blue / 255.0;
                        pixel -= 0.5;
                        pixel *= contrast;
                        pixel += 0.5;
                        pixel *= 255;
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;
                        p[0] = (byte)pixel;

                        p += 4;
                    }
                }
            }

            b.UnlockBits(bmData);

            return true;
        }
      

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < mSourceImage.Length; i++)
            {
                ScratchImage = mSourceImage[i].ActiveSelectedImage;
                holding = new Bitmap(ScratchImage.Width, ScratchImage.Height, PixelFormat.Format32bppArgb);
                DoRun();
                mSourceImage[i].ActiveSelectedImage = holding;
                mSourceImage[i].RedrawBuffers();
            }
            this.Hide();
        }

        private void sBrightness_ValueChanged(object sender, EventArgs e)
        {
            mContrastToken.Parameters[0] = (double)sBrightness.Value /(double) sBrightness.Maximum * 255;
            DoRun();
        }

        private void sContrast_ValueChanged(object sender, EventArgs e)
        {
            mContrastToken.Parameters[1] =(double) sContrast.Value / (double)sContrast.Maximum * 100;
            DoRun();

        }
    }
}
