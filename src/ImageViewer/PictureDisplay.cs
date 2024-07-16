using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using MathHelpLib;

namespace ImageViewer
{
    public class PictureDisplay : PictureBox
    {
        ScreenProperties mScreenProperties;
        DataEnvironment mDataEnvironment;
        private int mIndex;
        public int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }
        public ScreenProperties ScreenProperties
        {
            get { return mScreenProperties; }
            set { mScreenProperties = value; }
        }
        public DataEnvironment dataEnvironment
        {
            get { return mDataEnvironment; }
            set
            {
                mDataEnvironment = value;
                if (dataEnvironment !=null)
                    mScreenProperties = dataEnvironment.Screen;
            }
        }

        private Bitmap mImage;
        private ImageHolder mImageGray;
        private ImageHolder mImageColor;


        private delegate void SetImageDelegate(object image);
        public void SetImage(object image)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new SetImageDelegate(SetImage), image);
            }
            else
            {
                if (image.GetType() == typeof(Bitmap))
                {
                    Image = (Bitmap)image;
                }
                else if (image.GetType() == typeof(ImageHolder))
                {
                    Image = ((ImageHolder)image).ToBitmap();
                }
                else if (image.GetType() == typeof(ImageHolder))
                {
                    Image = ((ImageHolder)image).ToBitmap();
                }
                else
                    throw new Exception("image format not supported");

                try
                {
                    if (dataEnvironment != null)
                    {
                        if (mImage != null)
                        {
                            PaintBitmap(mImage);
                        }
                        else if (mImageColor != null)
                        {
                            PaintBitmap(mImageColor);
                        }
                        else if (mImageGray != null)
                        {
                            PaintBitmap(mImageGray);
                        }
                    }
                }
                catch { }
            }
        }

        public new Image Image
        {
            get { return mImage; }
            set
            {

                mImage = (Bitmap)value;
                mImageGray = null;
                mImageColor = null;
            }
        }

        public ImageHolder ImageGray
        {
            get { return mImageGray; }
            set
            {
                mImageGray = value;
                mImageColor = null;
                mImage = null;

            }
        }

        public ImageHolder ImageColor
        {
            get { return mImageColor; }
            set
            {
                mImageColor = value;
                mImage = null;
                mImageGray = null;
            }
        }
        private void PaintBitmap(Bitmap image)
        {
            Bitmap bOut;

            if (!(dataEnvironment.MinContrast == 0 && dataEnvironment.MaxContrast == 0))
            {
                #region contrast known
                bOut = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppRgb);
                // GDI+ still lies to us - the return format is BGR, NOT RGB.
                BitmapData bmData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

                byte MaxContrast = (byte)((double)dataEnvironment.MaxContrast * (double)byte.MaxValue / (double)ushort.MaxValue);
                byte MinContrast = (byte)((double)dataEnvironment.MinContrast * (double)byte.MaxValue / (double)ushort.MaxValue);
                double length = MaxContrast - MinContrast;
                double Gray=0;
                byte Intensity;
                unsafe
                {
                    for (int y = 0; y < image.Height; ++y)
                    {
                        byte* p = (byte*)(void*)bmData.Scan0 + bmData.Stride * y;
                        byte* pOut = (byte*)(void*)bmDataOut.Scan0 + bmDataOut.Stride * y;
                        for (int x = 0; x < image.Width; ++x)
                        {
                            Gray  = ((double)(p[0] - MinContrast) / length * 255);
                            if (Gray >255)Gray = 255;
                            if (Gray <0) Gray =0;
                            pOut[0] = (byte)Gray;

                            Gray = ((double)(p[1] - MinContrast) / length * 255);
                            if (Gray > 255) Gray = 255;
                            if (Gray < 0) Gray = 0;
                            pOut[1] = (byte)Gray;

                            Gray = ((double)(p[2] - MinContrast) / length * 255);
                            if (Gray > 255) Gray = 255;
                            if (Gray < 0) Gray = 0;
                            pOut[2] = (byte)Gray;
                            
                            pOut[3] = 255;
                            p += 4;
                            pOut += 4;
                        }
                    }
                }
                image.UnlockBits(bmData);
                bOut.UnlockBits(bmDataOut);
                // Graphics.FromImage(bOut).Clear(Color.Blue);
                base.Image = bOut;
                #endregion
            }
            else
            {
                base.Image = image;
            }
        }

        private void PaintBitmap(ImageHolder image)
        {
            Bitmap bOut = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppRgb);

            float [, ,] bmData = image.ImageData;
            BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            if (!(dataEnvironment.MinContrast == 0 && dataEnvironment.MaxContrast == 0))
            {
                #region Contrast known
                ushort MaxContrast = dataEnvironment.MaxContrast;
                ushort MinContrast = dataEnvironment.MinContrast;
                double length = MaxContrast - MinContrast;
                double dIntensity;
                byte Intensity;
                unsafe
                {
                    fixed (float * pData = bmData)
                    {
                        for (int y = 0; y < image.Height; ++y)
                        {
                            float * p = pData + image.Width * y;
                            byte* pOut = (byte*)(void*)bmDataOut.Scan0 + bmDataOut.Stride * y;
                            for (int x = 0; x < image.Width; ++x)
                            {
                                dIntensity = (double)(*p - MinContrast) / length * 255;
                                if (dIntensity > 255)
                                    Intensity = 255;
                                else if (dIntensity < 0)
                                    Intensity = 0;
                                else
                                    Intensity = (byte)dIntensity;

                                *pOut = Intensity;
                                pOut++;
                                *pOut = Intensity;
                                pOut++;
                                *pOut = Intensity;
                                pOut++;
                                *pOut = 255;
                                pOut++;
                                p++;
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region Contrast unknown
                unsafe
                {
                    fixed (float * pData = bmData)
                    {
                        float * pMax = pData;
                        float  MaxContrast = 0;
                        float  MinContrast = float .MaxValue;
                        for (int i = 0; i < bmData.Length; i++)
                        {
                            if (*pMax > MaxContrast) MaxContrast = *pMax;
                            if (*pMax < MinContrast) MinContrast = *pMax;
                            pMax++;
                        }

                        double length = MaxContrast - MinContrast;
                        double dIntensity;
                        byte Intensity;

                        for (int y = 0; y < image.Height; ++y)
                        {
                            float * p = pData + image.Width * y;
                            byte* pOut = (byte*)(void*)bmDataOut.Scan0 + bmDataOut.Stride * y;
                            for (int x = 0; x < image.Width; ++x)
                            {
                                dIntensity = (double)(*p - MinContrast) / length * 255;
                                if (dIntensity > 255)
                                    Intensity = 255;
                                else if (dIntensity < 0)
                                    Intensity = 0;
                                else
                                    Intensity = (byte)dIntensity;

                                *pOut = Intensity;
                                pOut++;
                                *pOut = Intensity;
                                pOut++;
                                *pOut = Intensity;
                                pOut++;
                                *pOut = 255;
                                pOut++;
                                p++;
                            }
                        }
                    }
                }
                #endregion
            }
            bOut.UnlockBits(bmDataOut);
            // Graphics.FromImage(bOut).Clear(Color.Blue);
            base.Image = bOut;
        }
       
    }
}
