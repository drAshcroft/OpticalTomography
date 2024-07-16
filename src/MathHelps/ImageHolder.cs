﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using MathHelpLib.Convolution;

namespace MathHelpLib
{
    public class ImageHolder : IDisposable
    {
        private int mNChannels;
        private Rectangle? mROI = null;
        //private Bitmap mInformationOverlay;
        private float[, ,] mImageData;

        #region Basic Picture Info
        public float[, ,] ImageData
        {
            get { return mImageData; }
            set
            {
                int mWidth = Width;
                int mHeight = Height;

                mImageData = value;
                mNChannels = mImageData.GetLength(2);

                if (mROI == null || mWidth != mImageData.GetLength(0) || mHeight != mImageData.GetLength(1))
                {
                    ResetROI();
                }


            }
        }

        public void ExactDataCopyInto(float[, ,] NewData)
        {
            Buffer.BlockCopy(NewData, 0, mImageData, 0, Buffer.ByteLength(NewData));
        }

        public int Width
        {
            get { return mImageData.GetLength(1); }
        }
        public int Height
        {
            get { return mImageData.GetLength(0); }
        }
        public int NChannels
        {
            get { return mNChannels; }
        }
        #endregion

        /// <summary>
        /// transparent bitmap that will contain boxes and such to display how the work is doing
        /// </summary>
        /* public Bitmap InformationOverLay
         {
             get { return mInformationOverlay; }
             set { mInformationOverlay = value; }
         }*/

        #region FileOperations
        public void Save(string Filename)
        {
            MathHelpLib.MathHelpsFileLoader.Save_Bitmap(Filename, this);
        }
        public void Open(string Filename)
        {
            ImageHolder ih = MathHelpLib.MathHelpsFileLoader.Load_Bitmap(Filename);
            mImageData = ih.ImageData;
            ih = null;
        }
        /* public void OpenOverlay(string Filename)
         {
             mInformationOverlay = new Bitmap(Filename);
         }
         public void SaveOverlay(string Filename)
         {
             mInformationOverlay.Save(Filename);
         }*/
        #endregion

        #region ROIs
        public Rectangle ROI
        {
            get
            {
                if (mROI == null)
                    return new Rectangle(0, 0, Width, Height);
                else
                    return (Rectangle)mROI;
            }
            set { mROI = value; }
        }

        public void ResetROI()
        {
            mROI = new Rectangle(0, 0, Width, Height);
        }
        #endregion

        #region Constructors
        public ImageHolder(string Filename)
        {
            ImageHolder ih = MathHelpLib.MathHelpsFileLoader.Load_Bitmap(Filename);

            mImageData = ih.ImageData;
            mNChannels = ih.mNChannels;

            mROI = new Rectangle(0, 0, ih.Width, ih.Height);
            ih = null;
            //  mInformationOverlay = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            // Graphics g = Graphics.FromImage(mInformationOverlay);
            // g.Clear(Color.Transparent);
        }

        public ImageHolder(Bitmap OriginalImage)
        {
            int mWidth = OriginalImage.Height;
            int mHeight = OriginalImage.Width;
            mNChannels = 3;
            mImageData = new float[mWidth, mHeight, mNChannels];

            BitmapData bmd = OriginalImage.LockBits(new Rectangle(0, 0, OriginalImage.Width, OriginalImage.Height), ImageLockMode.WriteOnly, OriginalImage.PixelFormat);

            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = 0; y < mWidth; y++)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < mHeight; x++)
                        {
                            byte* bits = (byte*)scanline;
                            mImageData[y, x, 0] = bits[0];
                            mImageData[y, x, 1] = bits[1];
                            mImageData[y, x, 2] = bits[2];
                            scanline++;
                        }
                    }
                }
                else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    for (int y = 0; y < mWidth; y++)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < mHeight; x++)
                        {
                            byte* bits = (byte*)scanline;
                            mImageData[y, x, 0] = bits[0];
                            mImageData[y, x, 1] = bits[1];
                            mImageData[y, x, 2] = bits[2];

                            scanline += 3;
                        }
                    }

                }
                else if (bmd.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    for (int y = 0; y < mWidth; y++)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < mHeight; x++)
                        {
                            byte* bits = (byte*)scanline;
                            mImageData[y, x, 0] = *bits;
                            mImageData[y, x, 1] = *bits;
                            mImageData[y, x, 2] = *bits;

                            scanline++;
                        }
                    }

                }
            }
            OriginalImage.UnlockBits(bmd);

            mROI = new Rectangle(0, 0, mWidth, mHeight);
            //mInformationOverlay = new Bitmap(mWidth, mHeight, PixelFormat.Format32bppArgb);
            // Graphics g = Graphics.FromImage(mInformationOverlay);
            // g.Clear(Color.Transparent);

        }

        public ImageHolder(Emgu.CV.Image<Emgu.CV.Structure.Gray, float> OriginalImage)
        {
            int mWidth = OriginalImage.Height;
            int mHeight = OriginalImage.Width;
            mNChannels = OriginalImage.NumberOfChannels;
            mImageData = new float[mWidth, mHeight, mNChannels];

            Size size = OriginalImage.Size;

            Buffer.BlockCopy(OriginalImage.Data, 0, mImageData, 0, Buffer.ByteLength(mImageData));

        }

        public ImageHolder(Emgu.CV.Image<Emgu.CV.Structure.Bgr, float> OriginalImage, int Channel)
        {
            int mWidth = OriginalImage.Height;
            int mHeight = OriginalImage.Width;
            mNChannels = 1;
            mImageData = new float[mWidth, mHeight, mNChannels];

            float[, ,] oImage = OriginalImage.Data;
            for (int i = 0; i < mWidth; i++)
            {
                for (int j = 0; j < mHeight; j++)
                {
                    mImageData[i, j, 0] = oImage[i, j, Channel];
                }
            }
        }

        public ImageHolder(Bitmap OriginalImage, int Channel)
        {
            int mWidth = OriginalImage.Height;
            int mHeight = OriginalImage.Width;
            mNChannels = 1;
            mImageData = new float[mWidth, mHeight, mNChannels];

            BitmapData bmd = OriginalImage.LockBits(new Rectangle(0, 0, OriginalImage.Width, OriginalImage.Height), ImageLockMode.WriteOnly, OriginalImage.PixelFormat);

            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = 0; y < mWidth; y++)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < mHeight; x++)
                        {
                            byte* bits = (byte*)scanline;
                            mImageData[y, x, 0] = bits[Channel];
                            scanline++;
                        }
                    }
                }
                else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    for (int y = 0; y < mWidth; y++)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < mHeight; x++)
                        {
                            byte* bits = (byte*)scanline;
                            mImageData[y, x, 0] = bits[Channel];

                            scanline += 3;
                        }
                    }

                }
                else if (bmd.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    for (int y = 0; y < mWidth; y++)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < mHeight; x++)
                        {
                            byte* bits = (byte*)scanline;
                            mImageData[y, x, 0] = *bits;
                            scanline++;
                        }
                    }

                }
            }
            OriginalImage.UnlockBits(bmd);

            mROI = new Rectangle(0, 0, mWidth, mHeight);
            //mInformationOverlay = new Bitmap(mWidth, mHeight, PixelFormat.Format32bppArgb);
            // Graphics g = Graphics.FromImage(mInformationOverlay);
            // g.Clear(Color.Transparent);

        }
        public ImageHolder(int Width, int Height)
        {
            mNChannels = 3;

            mImageData = new float[Height, Width, mNChannels];
            ResetROI();
            //  mInformationOverlay = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            //  Graphics g = Graphics.FromImage(mInformationOverlay);
            //  g.Clear(Color.Transparent);

        }
        public ImageHolder(int Width, int Height, int NChannels)
        {

            mNChannels = NChannels;

            mImageData = new float[Height, Width, mNChannels];
            ResetROI();
            //  mInformationOverlay = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            //  Graphics g = Graphics.FromImage(mInformationOverlay);
            //  g.Clear(Color.Transparent);

        }
        public ImageHolder(double[,] Intensities)
        {
            mImageData = new float[Intensities.GetLength(0), Intensities.GetLength(1), 1];

            for (int i = 0; i < Intensities.GetLength(0); i++)
            {
                for (int j = 0; j < Intensities.GetLength(1); j++)
                {
                    mImageData[i, j, 0] = (float)Intensities[i, j];
                }
            }
            mNChannels = 1;
            ResetROI();
            //mInformationOverlay = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            // Graphics g = Graphics.FromImage(mInformationOverlay);
            // g.Clear(Color.Transparent);
        }


        public ImageHolder(float[,,] Intensities, Rectangle CutRegion)
        {
            mImageData = new float[CutRegion.Width, CutRegion.Height, 1];

            int ccX = 0, ccY = 0;
            for (int i = CutRegion.Left; i < CutRegion.Right; i++)
            {
                for (int j = CutRegion.Top; j < CutRegion.Bottom; j++)
                {
                    mImageData[ccX, ccY, 0] = (float)Intensities[i, j,0];
                    ccY++;
                }
                ccX++;
            }
            mNChannels = 1;
            ResetROI();
        }

        public ImageHolder(float[,] Intensities)
        {
            mImageData = new float[Intensities.GetLength(0), Intensities.GetLength(1), 1];

            Buffer.BlockCopy(Intensities, 0, mImageData, 0, Buffer.ByteLength(Intensities));

            mNChannels = 1;
            ResetROI();
            //   mInformationOverlay = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            //   Graphics g = Graphics.FromImage(mInformationOverlay);
            //  g.Clear(Color.Transparent);
        }

        public ImageHolder(double[, ,] Intensities)
        {
            mImageData = new float[Intensities.GetLength(0), Intensities.GetLength(1), Intensities.GetLength(2)];
            for (int i = 0; i < Intensities.GetLength(0); i++)
            {
                for (int j = 0; j < Intensities.GetLength(1); j++)
                {
                    for (int k = 0; k < Intensities.GetLength(2); k++)
                    {
                        mImageData[i, j, k] = (float)Intensities[i, j, k];
                    }
                }
            }

            mNChannels = Intensities.GetLength(2);
            ResetROI();
            //   mInformationOverlay = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            //   Graphics g = Graphics.FromImage(mInformationOverlay);
            //   g.Clear(Color.Transparent);
        }

        public void Dispose()
        {
            mImageData = null;
        }

        public ImageHolder Clone()
        {
            ImageHolder newImage = new ImageHolder(this.Width, this.Height, this.mNChannels);
            newImage.mROI = this.mROI;
            Buffer.BlockCopy(mImageData, 0, newImage.mImageData, 0, Buffer.ByteLength(mImageData));
            return newImage;
        }
        #endregion

        // Declare the Windows API function CopyMemory
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr pDst, IntPtr pSrc, System.Int32 ByteLen);

        #region Copys
        public void DrawUnscaled(ImageHolder Copyee, int X, int Y)
        {
            int tWidth;
            int tHeight;
            if (X + Copyee.Width > Width)
                tWidth = Width - X;
            else
                tWidth = Copyee.Width;

            if (Y + Copyee.Height > Height)
                tHeight = Height - Y;
            else
                tHeight = Copyee.Height;

            for (int n = 0; n < Copyee.NChannels; n++)
            {
                for (int i = 0; i < tWidth; i++)
                {
                    for (int j = 0; j < tHeight; j++)
                    {
                        mImageData[Y + j, X + i, n] = Copyee.ImageData[j, i, n];
                    }
                }
            }
        }
        public void CopyTo(ImageHolder Copyee)
        {
            float[, ,] CopyTo = Copyee.mImageData;

            if (Copyee.mROI.Value.Width == mROI.Value.Width && Copyee.mROI.Value.Height == mROI.Value.Height)
            {
                Rectangle ROI = (Rectangle)mROI;
                Rectangle ROIout = Copyee.ROI;
                int xLength = 4 * ROI.Width;
                int FromStride = mImageData.GetLength(0);
                int FromStart = ROI.X;

                int ToStride = CopyTo.GetLength(0);
                int ToStart = Copyee.ROI.X;


                unsafe
                {
                    fixed (float* pToBase = CopyTo)
                    {
                        fixed (float* pFromBase = mImageData)
                        {
                            //straight over copy
                            if (mNChannels == Copyee.NChannels)
                            {
                                for (int channel = 0; channel < mNChannels; channel++)
                                {
                                    int cYout = ROIout.Y;
                                    int cYIn = ROI.Y;
                                    for (int y = 0; y < ROI.Height; y++)
                                    {
                                        int cXout = ROIout.X;
                                        int cXin = ROI.X;
                                        for (int x = 0; x < ROI.Width; x++)
                                        {
                                            CopyTo[cYout, cXout, channel] = mImageData[cYIn, cXin, channel];
                                            cXin++;
                                            cXout++;
                                        }
                                        cYout++;
                                        cYIn++;
                                    }
                                }
                            }
                            //copy from grayscale to color
                            if (mNChannels == 1 && Copyee.mNChannels > 1)
                            {
                                for (int channel = 0; channel < Copyee.mNChannels; channel++)
                                {
                                    int cYout = ROIout.Y;
                                    int cYIn = ROI.Y;
                                    for (int y = 0; y < ROI.Height; y++)
                                    {
                                        int cXout = ROIout.X;
                                        int cXin = ROI.X;
                                        for (int x = 0; x < ROI.Width; x++)
                                        {
                                            CopyTo[cYout, cXout, channel] = mImageData[cYIn, cXin, 0];
                                            cXin++;
                                            cXout++;
                                        }
                                        cYout++;
                                        cYIn++;
                                    }
                                }

                            }
                            //copy from color to grayscale
                            else
                            {
                                int cYout = ROIout.Y;
                                int cYIn = ROI.Y;
                                double Intensity;
                                for (int y = 0; y < ROI.Height; y++)
                                {
                                    int cXout = ROIout.X;
                                    int cXin = ROI.X;
                                    for (int x = 0; x < ROI.Width; x++)
                                    {
                                        Intensity = 0;
                                        for (int i = 0; i < mNChannels; i++)
                                            Intensity += mImageData[cYIn, cXin, i];

                                        CopyTo[cYout, cXout, 0] = (float)(Intensity / mNChannels);
                                        cXin++;
                                        cXout++;
                                    }
                                    cYout++;
                                    cYIn++;
                                }


                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Does not support copying selected areas of different sizes");
            }
        }

        public ImageHolder Copy()
        {
            ImageHolder Clone = new ImageHolder(this.Width, this.Height, this.NChannels);
            Buffer.BlockCopy(mImageData, 0, Clone.mImageData, 0, mImageData.Length * 4);
            return Clone;
        }

        public ImageHolder Copy(Rectangle ROI)
        {
            if (ROI.Width == 0 || ROI.Height == 0)
                return new ImageHolder(1, 1, 1);
            ImageHolder ih = new ImageHolder(ROI.Width, ROI.Height, mNChannels);
            ih.ROI = new Rectangle(0, 0, ROI.Width, ROI.Height);
            Rectangle oldRectangle = ROI;
            mROI = ROI;
            CopyTo(ih);
            ih.ResetROI();
            ROI = oldRectangle;
            return ih;
        }

        #endregion

        #region Transforms
        public void Clear(float ClearValue)
        {
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {
                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {
                        *pData = ClearValue;
                        pData++;
                    }
                }
            }
        }


        #endregion

        #region Conversions

        public Emgu.CV.Image<Emgu.CV.Structure.Gray, float> ToEMGU()
        {
            return  new Emgu.CV.Image<Emgu.CV.Structure.Gray, float>(ImageData);
        }

        public void ConvertToGrayScaleAverage()
        {
            if (NChannels == 1)
                return;
            else if (NChannels == 2)
            {
                #region 2Channels
                float[, ,] ImageDataN = new float[Height, Width, 1];
                unsafe
                {
                    fixed (float* pFromBase = mImageData)
                    {
                        fixed (float* pToBase = ImageDataN)
                        {
                            float* pFrom = pFromBase;
                            float* pTo = pToBase;
                            for (int i = 0; i < mImageData.Length; i++)
                            {
                                *pTo = (*pFrom + *(pFrom + 1)) / 2f;
                                pTo++;
                                pFrom += 2;
                            }
                        }
                    }
                }
                mNChannels = 1;
                mImageData = ImageDataN;
                #endregion
            }
            if (NChannels == 3)
            {
                #region 3Channels
                float[, ,] ImageDataN = new float[Height, Width, 1];
                unsafe
                {
                    fixed (float* pFromBase = mImageData)
                    {
                        fixed (float* pToBase = ImageDataN)
                        {
                            float* pFrom = pFromBase;
                            float* pTo = pToBase;
                            for (int i = 0; i < mImageData.Length; i += 3)
                            {
                                *pTo = (*pFrom + *(pFrom + 1) + *(pFrom + 2)) / 3f;
                                pTo++;
                                pFrom += 3;
                            }
                        }
                    }
                }
                mNChannels = 1;
                mImageData = ImageDataN;
                #endregion
            }
            else
                throw new Exception("Not Implimented for " + NChannels.ToString() + " channels");
        }

        //  static object ConvertLock=new object() ;
        public void ConvertToGrayScaleChannel(int OnlyChannel)
        {

            if (NChannels == 1)
                return;
            else if (NChannels == 2)
            {
                #region 2Channels
                float[, ,] ImageDataN = new float[Height, Width, 1];
                unsafe
                {
                    fixed (float* pFromBase = mImageData)
                    {
                        fixed (float* pToBase = ImageDataN)
                        {
                            float* pFrom = pFromBase + OnlyChannel;
                            float* pTo = pToBase;
                            for (int i = 0; i < mImageData.Length; i++)
                            {
                                *pTo = *pFrom;
                                pTo++;
                                pFrom += 2;
                            }
                        }
                    }
                }
                mNChannels = 1;
                mImageData = ImageDataN;
                #endregion
            }
            if (NChannels == 3)
            {
                #region 3Channels
                //some kind of compiler error here.  not sure if it still applies
                float[, ,] ImageDataN = new float[Height + 0, Width + 0, 1];
                unsafe
                {
                    fixed (float* pFromBase = mImageData)
                    {
                        fixed (float* pToBase = ImageDataN)
                        {
                            float* pFrom = pFromBase + OnlyChannel;
                            float* pTo = pToBase;
                            for (int i = 0; i < mImageData.Length; i += 3)
                            {
                                *pTo = *pFrom;
                                pTo++;
                                pFrom += 3;
                            }
                        }
                    }
                }
                mNChannels = 1;
                mImageData = ImageDataN;
                #endregion
            }
            else
                throw new Exception("Not Implimented for " + NChannels.ToString() + " channels");

        }

        public float[,] ToDataIntensity()
        {

            if (NChannels == 1)
            {
                float[,] OutArray = new float[Height, Width];
                Buffer.BlockCopy(mImageData, 0, OutArray, 0, Buffer.ByteLength(OutArray));
                return OutArray;
            }
            else if (NChannels == 2)
            {
                #region 2Channels
                float[,] ImageDataN = new float[Height, Width];
                unsafe
                {
                    fixed (float* pFromBase = mImageData)
                    {
                        fixed (float* pToBase = ImageDataN)
                        {
                            float* pFrom = pFromBase;
                            float* pTo = pToBase;
                            for (int i = 0; i < mImageData.Length; i++)
                            {
                                *pTo = (*pFrom + *(pFrom + 1)) / 2f;
                                pTo++;
                                pFrom += 2;
                            }
                        }
                    }
                }
                return ImageDataN;
                #endregion
            }
            if (NChannels == 3)
            {
                #region 3Channels
                float[,] ImageDataN = new float[Height, Width];
                unsafe
                {
                    fixed (float* pFromBase = mImageData)
                    {
                        fixed (float* pToBase = ImageDataN)
                        {
                            float* pFrom = pFromBase;
                            float* pTo = pToBase;
                            for (int i = 0; i < mImageData.Length; i += 3)
                            {
                                *pTo = (*pFrom + *(pFrom + 1) + *(pFrom + 2)) / 3f;
                                pTo++;
                                pFrom += 3;
                            }
                        }
                    }
                }
                return ImageDataN;
                #endregion
            }
            else
                throw new Exception("Not Implimented for " + NChannels.ToString() + " channels");

        }

        public double[,] ToDataIntensityDouble()
        {

            if (NChannels == 1)
            {
                double[,] ImageDataN = new double[Height, Width];
                unsafe
                {
                    fixed (float* pFromBase = mImageData)
                    {
                        fixed (double* pToBase = ImageDataN)
                        {
                            float* pFrom = pFromBase;
                            double* pTo = pToBase;
                            for (int i = 0; i < mImageData.Length; i++)
                            {
                                *pTo = *pFrom;
                                pTo++;
                                pFrom++;
                            }
                        }
                    }
                }
                return ImageDataN;
            }
            else if (NChannels == 2)
            {
                #region 2Channels
                double[,] ImageDataN = new double[Height, Width];
                unsafe
                {
                    fixed (float* pFromBase = mImageData)
                    {
                        fixed (double* pToBase = ImageDataN)
                        {
                            float* pFrom = pFromBase;
                            double* pTo = pToBase;
                            for (int i = 0; i < mImageData.Length; i++)
                            {
                                *pTo = (*pFrom + *(pFrom + 1)) / 2f;
                                pTo++;
                                pFrom += 2;
                            }
                        }
                    }
                }
                return ImageDataN;
                #endregion
            }
            if (NChannels == 3)
            {
                #region 3Channels
                double[,] ImageDataN = new double[Height, Width];
                unsafe
                {
                    fixed (float* pFromBase = mImageData)
                    {
                        fixed (double* pToBase = ImageDataN)
                        {
                            float* pFrom = pFromBase;
                            double* pTo = pToBase;
                            for (int i = 0; i < mImageData.Length; i += 3)
                            {
                                *pTo = (*pFrom + *(pFrom + 1) + *(pFrom + 2)) / 3f;
                                pTo++;
                                pFrom += 3;
                            }
                        }
                    }
                }
                return ImageDataN;
                #endregion
            }
            else
                throw new Exception("Not Implimented for " + NChannels.ToString() + " channels");

        }

        public static ImageHolder MergeToColor(ImageHolder Red, ImageHolder Green, ImageHolder Blue)
        {
            if (
                (Red.Width != Green.Width || Red.Height != Green.Height) ||
                (Red.Width != Blue.Width || Red.Height != Blue.Height) ||
                (Blue.Width != Green.Width || Blue.Height != Green.Height)
                )
            {
                throw new Exception("Merge To color: Images are not the same size");

            }

            ImageHolder ih = new ImageHolder(Red.Width, Red.Height, 3);
            unsafe
            {
                fixed (float* pToBase = ih.mImageData)
                {
                    fixed (float* pFromRedBase = Red.mImageData)
                    {
                        fixed (float* pFromGreenBase = Green.mImageData)
                        {
                            fixed (float* pFromBlueBase = Blue.mImageData)
                            {
                                float* pFromRed = pFromRedBase;
                                float* pFromGreen = pFromGreenBase;
                                float* pFromBlue = pFromBlueBase;
                                float* pTo = pToBase;
                                for (int i = 0; i < ih.mImageData.Length; i++)
                                {
                                    *pTo = *pFromRed;
                                    pTo++;
                                    *pTo = *pFromGreen;
                                    pTo++;
                                    *pTo = *pFromBlue;
                                    pTo++;
                                    pFromRed++;
                                    pFromGreen++;
                                    pFromBlue++;
                                }
                            }
                        }
                    }
                }
            }
            return ih;
        }

        public Bitmap BitmapImage
        {
            get { return ToBitmap(); }
        }

        #region MakeBitmap without contrast

        public Bitmap MakeFalseColorBitmap()
        {
            int iWidth = Width;
            int iHeight = Height;
            double iMax = -10000;
            double iMin = 10000;

            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            Int32 g;
            byte g2;
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {

                    float* pData = pDataBase;
                    BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

                    if (NChannels == 1)
                    {
                        for (int y = 0; y < iHeight; y++)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                            pData = pDataBase + y * iWidth;
                            for (int x = 0; x < iWidth; x++)
                            {
                                Int32* bits = (Int32*)scanline;
                                g = (Int32)(*pData);
                                if (g < 0) g = 0;
                                *bits = g;
                                scanline++;
                                pData++;
                            }
                        }
                    }
                    else if (NChannels == 3)
                    {
                        /*for (int y = 0; y < iHeight; y++)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                            pData = pDataBase + y * iWidth * 3;
                            for (int x = 0; x < iWidth; x++)
                            {
                                byte* bits = (byte*)scanline;
                                g = (int)(255d * ((*pData + *(pData + 1) + *(pData + 2)) / 3d - iMin) / iLength);
                                if (g >= 255) g = 255;
                                if (g < 0) g = 0;
                                g2 = (byte)g;
                                bits[0] = g2;
                                bits[1] = g2;
                                bits[2] = g2;
                                scanline += 3;
                                pData++;
                            }
                        }
                        */
                    }

                    b.UnlockBits(bmd);
                }
            }
            return b;
        }

        /// <summary>
        /// Set minContrast =0 and max contrast to either 255 or int16.maxvalue
        /// </summary>
        /// <returns></returns>
        private Bitmap MakeFixedBitmapGrayScale()
        {
            int iWidth = Width;
            int iHeight = Height;
            double iMax = -10000;
            double iMin = 10000;

            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            int g;
            byte g2;
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {

                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {
                        if (iMax < *pData) iMax = *pData;
                        if (iMin > *pData) iMin = *pData;
                        pData++;
                    }

                    iMin = 0;
                    if (iMax > 255)
                        iMax = UInt16.MaxValue;
                    else
                        iMax = 255;


                    double iLength = iMax - iMin;

                    BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

                    if (NChannels == 1)
                    {
                        for (int y = 0; y < iHeight; y++)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                            pData = pDataBase + y * iWidth;
                            for (int x = 0; x < iWidth; x++)
                            {
                                byte* bits = (byte*)scanline;
                                g = (int)(255d * (*pData - iMin) / iLength);
                                if (g >= 255) g = 255;
                                if (g < 0) g = 0;
                                g2 = (byte)g;
                                bits[0] = g2;
                                bits[1] = g2;
                                bits[2] = g2;
                                scanline++;
                                pData++;
                            }
                        }
                    }
                    else if (NChannels == 3)
                    {
                        for (int y = 0; y < iHeight; y++)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                            pData = pDataBase + y * iWidth * 3;
                            for (int x = 0; x < iWidth; x++)
                            {
                                byte* bits = (byte*)scanline;
                                g = (int)(255d * ((*pData + *(pData + 1) + *(pData + 2)) / 3d - iMin) / iLength);
                                if (g >= 255) g = 255;
                                if (g < 0) g = 0;
                                g2 = (byte)g;
                                bits[0] = g2;
                                bits[1] = g2;
                                bits[2] = g2;
                                scanline += 3;
                                pData++;
                            }
                        }

                    }

                    b.UnlockBits(bmd);
                }
            }

            return b;

        }

        private Bitmap MakeBitmapGrayScale()
        {
            int iWidth = Width;
            int iHeight = Height;
            double iMax = -10000;
            double iMin = 10000;

            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            int g;
            byte g2;
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {

                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {
                        if (iMax < *pData) iMax = *pData;
                        if (iMin > *pData) iMin = *pData;
                        pData++;
                    }
                    double iLength = iMax - iMin;

                    BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

                    if (NChannels == 1)
                    {
                        for (int y = 0; y < iHeight; y++)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                            pData = pDataBase + y * iWidth;
                            for (int x = 0; x < iWidth; x++)
                            {
                                byte* bits = (byte*)scanline;
                                g = (int)(255d * (*pData - iMin) / iLength);
                                if (g >= 255) g = 255;
                                if (g < 0) g = 0;
                                g2 = (byte)g;
                                bits[0] = g2;
                                bits[1] = g2;
                                bits[2] = g2;
                                scanline++;
                                pData++;
                            }
                        }
                    }
                    else if (NChannels == 3)
                    {
                        for (int y = 0; y < iHeight; y++)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                            pData = pDataBase + y * iWidth * 3;
                            for (int x = 0; x < iWidth; x++)
                            {
                                byte* bits = (byte*)scanline;
                                g = (int)(255d * ((*pData + *(pData + 1) + *(pData + 2)) / 3d - iMin) / iLength);
                                if (g >= 255) g = 255;
                                if (g < 0) g = 0;
                                g2 = (byte)g;
                                bits[0] = g2;
                                bits[1] = g2;
                                bits[2] = g2;
                                scanline += 3;
                                pData++;
                            }
                        }

                    }

                    b.UnlockBits(bmd);
                }
            }

            return b;

        }

        private Bitmap MakeBitmapColor()
        {
            int iWidth = Width;
            int iHeight = Height;

            double iMax = -10000;
            double iMin = 10000;
            double iLength = 1;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            int g;
            byte g2;
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {

                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i += 4)
                    {
                        if (iMax < *pData) iMax = *pData;
                        if (iMin > *pData) iMin = *pData;
                        pData += 4;
                    }
                    iLength = iMax - iMin;


                    BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

                    for (int y = 0; y < iHeight; y++)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                        pData = pDataBase + y * iWidth * 3;
                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;

                            //convert the red
                            g = (int)(255d * (*pData - iMin) / iLength);
                            if (g >= 255) g = 255;
                            if (g < 0) g = 0;
                            g2 = (byte)g;
                            bits[0] = g2;
                            pData++;

                            //convert the green
                            g = (int)(255d * (*pData - iMin) / iLength);
                            if (g >= 255) g = 255;
                            if (g < 0) g = 0;
                            g2 = (byte)g;
                            bits[1] = g2;
                            pData++;

                            //convert the blue
                            g = (int)(255d * (*pData - iMin) / iLength);
                            if (g >= 255) g = 255;
                            if (g < 0) g = 0;
                            g2 = (byte)g;
                            bits[2] = g2;
                            pData++;

                            scanline++;

                        }
                    }
                    b.UnlockBits(bmd);
                }
            }

            return b;

        }

        public Bitmap ToBitmap()
        {
            Bitmap b;
            if (NChannels == 1)
                b = MakeBitmapGrayScale();
            else
                b = MakeBitmapColor();
            // Graphics g = Graphics.FromImage(b);
            // g.DrawImage(mInformationOverlay, new Point(0, 0));
            return b;
        }

        public Bitmap ToMaskBitmap(float Threshold)
        {

            int iWidth = Width;
            int iHeight = Height;
            double iMax = -10000;
            double iMin = 10000;

            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            float g;
            byte g2;
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {

                    float* pData = pDataBase;
                    BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

                    if (NChannels == 1)
                    {
                        for (int y = 0; y < iHeight; y++)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                            pData = pDataBase + y * iWidth;
                            for (int x = 0; x < iWidth; x++)
                            {
                                byte* bits = (byte*)scanline;
                                g = *pData;
                                if (g >= Threshold)
                                    g2 = 255;
                                else
                                    g2 = 0;

                                bits[0] = g2;
                                bits[1] = g2;
                                bits[2] = g2;
                                scanline++;
                                pData++;
                            }
                        }
                    }
                    else if (NChannels == 3)
                    {
                        for (int y = 0; y < iHeight; y++)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                            pData = pDataBase + y * iWidth * 3;
                            for (int x = 0; x < iWidth; x++)
                            {
                                byte* bits = (byte*)scanline;
                                g = (*pData + *(pData + 1) + *(pData + 2)) / 3f;
                                if (g >= Threshold)
                                    g2 = 255;
                                else
                                    g2 = 0;
                                bits[0] = g2;
                                bits[1] = g2;
                                bits[2] = g2;
                                scanline += 3;
                                pData++;
                            }
                        }

                    }

                    b.UnlockBits(bmd);
                }
            }

            return b;
        }


        /// <summary>
        /// Makes a bitmap with min intensity =0 and max intensity = 255 or int16.maxvalue
        /// </summary>
        /// <returns></returns>
        public Bitmap ToEqualIntensityBitmap()
        {
            Bitmap b;

            b = MakeFixedBitmapGrayScale();

            return b;
        }
        #endregion

        #region Make bitmap with contrast

        private Bitmap MakeBitmapGrayScale(float MinContrast, float MaxContrast)
        {
            int iWidth = Width;
            int iHeight = Height;
            double iMax = MaxContrast;
            double iMin = MinContrast;

            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            int g;
            byte g2;
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {

                    float* pData = pDataBase;

                    double iLength = iMax - iMin;

                    BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

                    for (int y = 0; y < iHeight; y++)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                        pData = pDataBase + y * iWidth;
                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g = (int)(255d * (*pData - iMin) / iLength);
                            if (g > 255) g = 255;
                            if (g < 0) g = 0;
                            g2 = (byte)g;
                            bits[0] = g2;
                            bits[1] = g2;
                            bits[2] = g2;
                            scanline++;
                            pData++;
                        }
                    }
                    b.UnlockBits(bmd);
                }
            }

            return b;

        }

        private Bitmap MakeBitmapColor(float MinContrast, float MaxContrast)
        {
            int iWidth = Width;
            int iHeight = Height;

            double iMax = MaxContrast;
            double iMin = MinContrast;
            double iLength = 1;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            int g;
            byte g2;
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {

                    float* pData = pDataBase;

                    iLength = iMax - iMin;


                    BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

                    for (int y = 0; y < iHeight; y++)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                        pData = pDataBase + y * iWidth * 3;
                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;

                            //convert the red
                            g = (int)(255d * (*pData - iMin) / iLength);
                            if (g > 255) g = 255;
                            if (g < 0) g = 0;
                            g2 = (byte)g;
                            bits[0] = g2;
                            pData++;

                            //convert the green
                            g = (int)(255d * (*pData - iMin) / iLength);
                            if (g > 255) g = 255;
                            if (g < 0) g = 0;
                            g2 = (byte)g;
                            bits[1] = g2;
                            pData++;

                            //convert the blue
                            g = (int)(255d * (*pData - iMin) / iLength);
                            if (g > 255) g = 255;
                            if (g < 0) g = 0;
                            g2 = (byte)g;
                            bits[2] = g2;
                            pData++;

                            scanline++;

                        }
                    }
                    b.UnlockBits(bmd);
                }
            }

            return b;

        }

        private Bitmap MakeBitmapGrayScale(float MinContrast)
        {
            int iWidth = Width;
            int iHeight = Height;
            double iMin = MinContrast;
            double iMax = 0;
            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            int g;
            byte g2;
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {

                    float* pData = pDataBase;

                    for (int i = 0; i < mImageData.Length; i++)
                    {
                        if (iMax < *pData) iMax = *pData;
                        pData++;
                    }

                    double iLength = iMax - iMin;

                    BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

                    for (int y = 0; y < iHeight; y++)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                        pData = pDataBase + y * iWidth;
                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g = (int)(255d * (*pData - iMin) / iLength);
                            if (g > 255) g = 255;
                            if (g < 0)
                                g = 0;
                            g2 = (byte)g;
                            bits[0] = g2;
                            bits[1] = g2;
                            bits[2] = g2;
                            scanline++;
                            pData++;
                        }
                    }
                    b.UnlockBits(bmd);
                }
            }

            return b;

        }


        public Bitmap ToBitmap(float MinContrast, float MaxContrast)
        {
            Bitmap b;
            if (NChannels == 1)
                b = MakeBitmapGrayScale(MinContrast, MaxContrast);
            else
                b = MakeBitmapColor(MinContrast, MaxContrast);

            // Graphics g = Graphics.FromImage(b);
            // g.DrawImage(mInformationOverlay, new Point(0, 0));

            return b;
        }

        public Bitmap ToBitmap(float MinContrast)
        {
            Bitmap b;
            if (NChannels == 1)
                b = MakeBitmapGrayScale(MinContrast);
            else
                throw new Exception("Not yet implimented");

            // Graphics g = Graphics.FromImage(b);
            // g.DrawImage(mInformationOverlay, new Point(0, 0));

            return b;
        }
        #endregion
        #endregion

        #region Math

        public void NormalizeToValue(float DesiredMaxValue)
        {
            float divisor = (float)Max();
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {
                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {

                        *pData = (float)(*pData / divisor * DesiredMaxValue);
                        if (*pData < .1)
                            *pData = .1f;
                        pData++;
                    }
                }
            }
        }

        public ImageHolder Convolution(Convolution.IConvolutionKernal kernal)
        {
            return ConvolutionFilterImplented.ConvolutionFilter(this, kernal);
        }

        public void ThresholdBinaryWhite(float ThresholdValue)
        {
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {
                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {
                        if (*pData > ThresholdValue)
                            *pData = float.MaxValue;
                        else
                            *pData = 0;
                        pData++;
                    }
                }
            }
        }
        public void ThresholdBinaryBlack(float ThresholdValue)
        {
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {
                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {
                        if (*pData < ThresholdValue)
                            *pData = float.MaxValue;
                        else
                            *pData = 0;
                        pData++;
                    }
                }
            }
        }

        public double Max()
        {
            return ImageData.MaxArray();
        }
        public double Min()
        {
            return ImageData.MinArray();
        }
        public void Invert()
        {

            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {
                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {

                        *pData = -1 * (*pData);
                        pData++;
                    }
                }
            }
        }
        public void InvertMax()
        {
            const float max = Int16.MaxValue - 1;// float.MaxValue - 1;
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {
                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {

                        *pData = max - (*pData);
                        pData++;
                    }
                }
            }
        }

        public void InvertZero()
        {
            float max = mImageData.MaxArray();
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {
                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {

                        *pData = max - (*pData);
                        pData++;
                    }
                }
            }
        }

        public float GetAverage()
        {
            double sum = 0;
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {
                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {
                        sum += *pData;
                        pData++;
                    }
                }
            }
            sum /= mImageData.Length;
            return (float)sum;
        }

        public float GetMax()
        {
            double Max = 0;
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {
                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {
                        if (Max < *pData) Max = *pData;
                        pData++;
                    }
                }
            }
            return (float)Max;
        }

        public void  Mul(float mult)
        {
           
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {
                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {
                        *pData *= mult;
                        pData++;
                    }
                }
            }
          
        }

        public static ImageHolder CombineByBrightest(ImageHolder image0, ImageHolder image1, ImageHolder image2, ImageHolder image3)
        {
            ImageHolder ih = new ImageHolder(image0.Width, image0.Height, image0.NChannels);

            unsafe
            {
                fixed (float* pDataBase0 = image0.mImageData)
                {
                    fixed (float* pDataBase1 = image1.mImageData)
                    {
                        fixed (float* pDataBase2 = image2.mImageData)
                        {
                            fixed (float* pDataBase3 = image3.mImageData)
                            {
                                fixed (float* pDataOutBase = ih.mImageData)
                                {
                                    float* pData0 = pDataBase0;
                                    float* pData1 = pDataBase1;
                                    float* pData2 = pDataBase2;
                                    float* pData3 = pDataBase3;
                                    float* pOut = pDataOutBase;
                                    float maxValue = 0;
                                    for (int i = 0; i < image0.mImageData.Length; i++)
                                    {
                                        maxValue = *pData0;
                                        if (*pData1 > maxValue) maxValue = *pData1;
                                        if (*pData2 > maxValue) maxValue = *pData2;

                                        if (*pData3 > maxValue) maxValue = *pData3;

                                        *pOut = maxValue;

                                        pData0++;
                                        pData1++;
                                        pData2++;
                                        pData3++;
                                        pOut++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ih;

        }

        public static ImageHolder operator /(ImageHolder x, double y)
        {
            ImageHolder ih = new ImageHolder(x.Width, x.Height, x.NChannels);
            unsafe
            {
                fixed (float* pDataBase = x.mImageData)
                {
                    fixed (float* pDataOutBase = ih.mImageData)
                    {
                        float* pData = pDataBase;
                        float* pOut = pDataOutBase;
                        for (int i = 0; i < x.mImageData.Length; i++)
                        {
                            *pOut = (float)(*pData / y);
                            pData++;
                            pOut++;
                        }
                    }
                }
            }
            return ih;
        }

        public static ImageHolder operator *(ImageHolder x, double y)
        {
            ImageHolder ih = new ImageHolder(x.Width, x.Height, x.NChannels);
            unsafe
            {
                fixed (float* pDataBase = x.mImageData)
                {
                    fixed (float* pDataOutBase = ih.mImageData)
                    {
                        float* pData = pDataBase;
                        float* pOut = pDataOutBase;
                        for (int i = 0; i < x.mImageData.Length; i++)
                        {
                            *pOut = (float)(*pData * y);
                            pData++;
                            pOut++;
                        }
                    }
                }
            }
            return ih;
        }


        public static ImageHolder operator +(ImageHolder x, ImageHolder y)
        {
            ImageHolder ih = new ImageHolder(x.Width, x.Height, x.NChannels);
            if (x.Width != y.Width || x.Height != y.Height || x.NChannels != y.NChannels)
            {
                throw new Exception("Images must be the same size");
            }
            unsafe
            {
                fixed (float* pDataBase = x.mImageData)
                {
                    fixed (float* pAddBase = y.mImageData)
                    {
                        fixed (float* pDataOutBase = ih.mImageData)
                        {
                            float* pData = pDataBase;
                            float* pOut = pDataOutBase;
                            float* pAdd = pAddBase;
                            for (int i = 0; i < x.mImageData.Length; i++)
                            {
                                *pOut = (float)(*pData + *pAdd);
                                pData++;
                                pAdd++;
                                pOut++;
                            }
                        }
                    }
                }
            }
            return ih;
        }

        public void Add(ImageHolder x)
        {
            if (x.Width != this.Width || x.Height != this.Height || x.NChannels != this.NChannels)
            {
                throw new Exception("Images must be the same size");
            }
            unsafe
            {
                fixed (float* pDataBase = x.mImageData)
                {
                    fixed (float* pAddBase = this.mImageData)
                    {
                        fixed (float* pDataOutBase = this.mImageData)
                        {
                            float* pData = pDataBase;
                            float* pOut = pDataOutBase;
                            float* pAdd = pAddBase;
                            for (int i = 0; i < x.mImageData.Length; i++)
                            {
                                *pOut = (float)(*pData + *pAdd);
                                pData++;
                                pAdd++;
                                pOut++;
                            }
                        }
                    }
                }
            }
        }

        public static void AddToDouble(ref double[,] BaseArray, ImageHolder image)
        {
            if (BaseArray.GetLength(1) != image.Width || BaseArray.GetLength(0) != image.Height || 1 != image.NChannels)
            {
                throw new Exception("Images must be the same size");
            }

            float[, ,] yData = image.ImageData;
            for (int x = 0; x < BaseArray.GetLength(0); x++)
                for (int y = 0; y < BaseArray.GetLength(1); y++)
                {
                    BaseArray[x, y] += yData[x, y, 0];
                }
        }

        public void DivideImage(double divisor)
        {
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {
                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {

                        *pData = (float)(*pData / divisor);
                        pData++;
                    }
                }
            }
        }

        public void ImproveMinimum(double Factor)
        {
            float min = (float)Min();
            float max = (float)Max();
            min = (float)((max - min) * Factor + min);
            unsafe
            {
                fixed (float* pDataBase = mImageData)
                {
                    float* pData = pDataBase;
                    for (int i = 0; i < mImageData.Length; i++)
                    {

                        *pData = (float)(*pData - min);
                        if (*pData < 0) *pData = 0;
                        pData++;
                    }
                }
            }
        }

        public double FocusValue()
        {
            double[,] xSobel = new double[,] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
            ValueArrayKernal vak = new ValueArrayKernal(xSobel);
            ImageHolder xIH = ConvolutionFilterImplented.ConvolutionFilter(this, vak);

            double sum = 0;
            unsafe
            {
                fixed (float* pXBase = xIH.mImageData)
                {
                    float* pX = pXBase;
                    for (int i = 0; i < xIH.mImageData.Length; i++)
                    {
                        sum += (*pX) * (*pX);
                    }

                }
            }
            return sum / (double)xIH.mImageData.Length;
        }

        public void Rotate90()
        {
            float[, ,] NewArray = new float[Width, Height, mNChannels];


            /*   for (int channel = 0; channel < mNChannels; mNChannels++)
               {
                   for (int y = 0; y < Height; y++)
                   {
                       for (int x = 0; x < Width; x++)
                       {
                           NewArray[x, y, channel] = mImageData[y, x, channel];
                       }
                   }
               }
               mImageData = NewArray;*/

            int Stride = Width * mNChannels;
            unsafe
            {
                fixed (float* pFromBase = mImageData)
                {
                    fixed (float* pToBase = NewArray)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            float* pFrom = pFromBase + y * Stride;
                            float* pTo = pToBase + y * mNChannels;
                            for (int x = 0; x < Width; x++)
                            {
                                *pTo = *pFrom;
                                pTo += Stride;
                                pFrom += NChannels;
                            }
                        }
                    }
                }
            }
            mImageData = NewArray;
        }

        public ImageHolder Rotate(double Degrees, float BackgroundColor)
        {
            throw new Exception("Not implemented yet");
        }
        #endregion

        #region Profiling
        public float[] SampleOnLine(Point p1, Point p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double LineLength = Math.Sqrt(dx * dx + dy * dy);
            int Length = (int)Math.Truncate(LineLength);

            float[] LineValues = new float[Length];
            double x, y;
            int xI, yI;
            double StepSize = 1d / LineLength;
            float nInputs = 4 * NChannels;

            try
            {
                for (int channel = 0; channel < mNChannels; channel++)
                {
                    int cc = 0;

                    for (double u = 0; u < 1; u += StepSize)
                    {

                        x = dx * u + p1.X;
                        y = dy * u + p1.Y;
                        xI = (int)Math.Round(x);
                        yI = (int)Math.Round(y);
                        LineValues[cc] += mImageData[yI, xI, channel];

                        cc++;

                    }
                }
            }
            catch { }
            return LineValues;
        }

        #endregion
    }
}