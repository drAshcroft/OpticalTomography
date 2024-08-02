using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using ImageViewer.Filters;

using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using MathHelpLib;
using Emgu.CV;
using Emgu.CV.Structure;
using MathHelpLib.ImageProcessing;

namespace ImageViewer
{
    public static class ImagingTools
    {

        public static Rectangle GetImageBounds(object Image)
        {
            if (Image.GetType() == typeof(Bitmap))
            {
                Bitmap image = (Bitmap)Image;
                return new Rectangle(0, 0, image.Width, image.Height);
            }
            else if (Image.GetType() == typeof(ImageHolder))
            {
                ImageHolder image = (ImageHolder)Image;
                return new Rectangle(0, 0, image.Width, image.Height);
            }

            return Rectangle.Empty;
        }

        public static double[] ConvertGrayscaleSelectionToLinear(Bitmap SourceImage, ImageViewer.ISelection Selection)
        {
            int iWidth = SourceImage.Width;
            int iHeight = SourceImage.Height;

            double[] ImageArray = new double[Selection.SelectionBounds.Width * Selection.SelectionBounds.Height];

            BitmapData bmd = SourceImage.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height), ImageLockMode.WriteOnly, SourceImage.PixelFormat);

            double g1, g2, g3;
            long t = 0;
            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = Selection.SelectionBounds.Top; y < Selection.SelectionBounds.Bottom; y += 2)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                        for (int x = Selection.SelectionBounds.Left; x < Selection.SelectionBounds.Right; x += 2)
                        {
                            if (Selection.PointInSelection(new Point(x, y)) == true)
                            {
                                byte* bits = (byte*)(scanline + x);
                                g1 = bits[0];
                                g2 = bits[1];
                                g3 = bits[2];

                                ImageArray[t] = (g1 + g2 + g3) / 3d;

                                t++;
                            }
                        }
                    }
                }
                else
                    throw new Exception("Does not support image formats other than 32 bits.  Please convert the image");
            }
            SourceImage.UnlockBits(bmd);
            if (t < ImageArray.Length)
            {
                double[] ActualLength = new double[t];
                Buffer.BlockCopy(ImageArray, 0, ActualLength, 0, (int)(t * sizeof(double)));
                return ActualLength;
            }
            else
                return ImageArray;
        }

        public static double[] ConvertGrayscaleSelectionToLinear(ImageHolder SourceImage, ImageViewer.ISelection Selection)
        {
            int iWidth = SourceImage.Width;
            int iHeight = SourceImage.Height;

            double[] ImageArray = new double[Selection.SelectionBounds.Width * Selection.SelectionBounds.Height];

            float[, ,] bmd = SourceImage.ImageData;

            double g1, g2, g3;
            long t = 0;


            for (int y = Selection.SelectionBounds.Top; y < Selection.SelectionBounds.Bottom; y += 2)
            {
                for (int x = Selection.SelectionBounds.Left; x < Selection.SelectionBounds.Right; x += 2)
                {
                    if (Selection.PointInSelection(new Point(x, y)) == true)
                    {
                        g1 = bmd[0, x, y];
                        g2 = bmd[1, x, y];
                        g3 = bmd[2, x, y];

                        ImageArray[t] = (g1 + g2 + g3) / 3d;

                        t++;
                    }
                }
            }


            if (t < ImageArray.Length)
            {
                double[] ActualLength = new double[t];
                Buffer.BlockCopy(ImageArray, 0, ActualLength, 0, (int)(t * sizeof(double)));
                return ActualLength;
            }
            else
                return ImageArray;
        }

        public static double[] ConvertGrayscaleSelectionToLinear(ImageHolder SourceImage)
        {
            int iWidth = SourceImage.Width;
            int iHeight = SourceImage.Height;

            double[] ImageArray = new double[SourceImage.Width * SourceImage.Height];

            float[, ,] bmd = SourceImage.ImageData;

            double g1, g2, g3;
            long t = 0;
            double N = SourceImage.NChannels;
            for (int n = 0; n < SourceImage.NChannels; n++)
            {
                for (int y = 0; y < SourceImage.Height; y += 2)
                {
                    for (int x = 0; x < SourceImage.Width; x += 2)
                    {
                        g1 = bmd[y, x, n];
                        ImageArray[t] += g1 / N;
                        t++;
                    }
                }
            }


            if (t < ImageArray.Length)
            {
                double[] ActualLength = new double[t];
                Buffer.BlockCopy(ImageArray, 0, ActualLength, 0, (int)(t * sizeof(double)));
                return ActualLength;
            }
            else
                return ImageArray;
        }

        public static double[] ConvertGrayscaleSelectionToLinear(double[,] SourceImage, ImageViewer.ISelection Selection)
        {
            int iWidth = SourceImage.GetLength(0);
            int iHeight = SourceImage.GetLength(1);

            double[] ImageArray = new double[Selection.SelectionBounds.Width * Selection.SelectionBounds.Height];

            double[,] bmd = SourceImage;

            double g1;
            long t = 0;


            for (int y = Selection.SelectionBounds.Top; y < Selection.SelectionBounds.Bottom; y += 2)
            {
                for (int x = Selection.SelectionBounds.Left; x < Selection.SelectionBounds.Right; x += 2)
                {
                    if (Selection.PointInSelection(new Point(x, y)) == true)
                    {
                        g1 = bmd[x, y];
                        ImageArray[t] = g1;
                        t++;
                    }
                }
            }


            if (t < ImageArray.Length)
            {
                double[] ActualLength = new double[t];
                Buffer.BlockCopy(ImageArray, 0, ActualLength, 0, (int)(t * sizeof(double)));
                return ActualLength;
            }
            else
                return ImageArray;
        }



        #region Histograms
        public static int[] MakeHistogram(double[] Intensities, int NumBins)
        {
            double max = Intensities.MaxArray();
            double min = Intensities.MinArray();
            double step = (max - min) / (double)NumBins;
            int[] OutArray = new int[NumBins + 1];
            if (step != 0)
            {
                for (int i = 0; i < Intensities.Length; i++)
                {
                    int index = (int)Math.Truncate((Intensities[i] - min) / step);
                    OutArray[index]++;
                }
            }
            return OutArray;
        }

        public static double[,] MakeHistogramIndexed(double[] Intensities, int NumBins)
        {
            double max = Intensities.MaxArray();
            double min = Intensities.MinArray();
            double step = (max - min) / (double)NumBins;
            double[,] OutArray = new double[2, NumBins + 1];
            for (int i = 0; i < Intensities.Length; i++)
            {
                int index = (int)Math.Truncate((Intensities[i] - min) / step);
                OutArray[1, index]++;
            }
            for (int i = 0; i < OutArray.GetLength(1); i++)
                OutArray[0, i] = min + step * i;
            return OutArray;
        }

        public static double[,] MakeHistogramIndexed(double[] Intensities)
        {
            double max = Intensities.MaxArray();
            double min = Intensities.MinArray();
            int NumBins = (int)Math.Floor(Intensities.Length / 5d);
            double step = (max - min) / (double)NumBins;
            double[,] OutArray = new double[2, NumBins + 1];
            for (int i = 0; i < Intensities.Length; i++)
            {
                int index = (int)Math.Truncate((Intensities[i] - min) / step);
                OutArray[1, index]++;
            }
            for (int i = 0; i < OutArray.GetLength(1); i++)
                OutArray[0, i] = min + step * i;
            return OutArray;
        }

        public static void ConvertSelectionToLinear(Bitmap SourceImage, ImageViewer.ISelection Selection, out double[] Red, out double[] Green, out double[] Blue)
        {
            int iWidth = SourceImage.Width;
            int iHeight = SourceImage.Height;

            double[] ImageArrayR = new double[Selection.SelectionBounds.Width * Selection.SelectionBounds.Height];
            double[] ImageArrayG = new double[Selection.SelectionBounds.Width * Selection.SelectionBounds.Height];
            double[] ImageArrayB = new double[Selection.SelectionBounds.Width * Selection.SelectionBounds.Height];

            BitmapData bmd = SourceImage.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height), ImageLockMode.WriteOnly, SourceImage.PixelFormat);

            double g1, g2, g3;
            long t = 0;
            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = Selection.SelectionBounds.Top; y < Selection.SelectionBounds.Bottom; y += 2)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                        for (int x = Selection.SelectionBounds.Left; x < Selection.SelectionBounds.Right; x += 2)
                        {
                            if (Selection.PointInSelection(new Point(x, y)) == true)
                            {
                                byte* bits = (byte*)(scanline + x);
                                g1 = bits[0];
                                g2 = bits[1];
                                g3 = bits[2];

                                ImageArrayR[t] = g1;
                                ImageArrayG[t] = g2;
                                ImageArrayB[t] = g3;

                                t++;
                            }
                        }
                    }
                }
                else
                    throw new Exception("Does not support image formats other than 32 bits.  Please convert the image");
            }
            SourceImage.UnlockBits(bmd);
            if (t < ImageArrayR.Length)
            {
                Red = new double[t];
                Buffer.BlockCopy(ImageArrayR, 0, Red, 0, (int)(t * sizeof(double)));
                Green = new double[t];
                Buffer.BlockCopy(ImageArrayG, 0, Green, 0, (int)(t * sizeof(double)));
                Blue = new double[t];
                Buffer.BlockCopy(ImageArrayB, 0, Blue, 0, (int)(t * sizeof(double)));
            }
            else
            {
                Red = ImageArrayR;
                Blue = ImageArrayB;
                Green = ImageArrayG;
            }
        }

        #endregion





        #region Image DataTransfer
        public static bool OverWriteImage(Bitmap bIn, Bitmap bFrom, Point dest)
        {

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmDataIn = bIn.LockBits(new Rectangle(0, 0, bIn.Width, bIn.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            BitmapData bmDataFrom = bFrom.LockBits(new Rectangle(0, 0, bFrom.Width, bFrom.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            int strideIn = bmDataIn.Stride;
            System.IntPtr Scan0In = bmDataIn.Scan0;

            unsafe
            {
                for (int y = 0; y < bFrom.Height; ++y)
                {
                    Int32* pIn = (Int32*)((byte*)(void*)Scan0In + bmDataIn.Stride * (y + dest.Y) + dest.X * 4);
                    Int32* pFrom = (Int32*)((byte*)(void*)bmDataFrom.Scan0 + bmDataFrom.Stride * y);
                    for (int x = 0; x < bFrom.Width; ++x)
                    {
                        *pIn = *pFrom;
                        pIn++;
                        pFrom++;
                    }
                }
            }

            bIn.UnlockBits(bmDataIn);
            bFrom.UnlockBits(bmDataFrom);

            return true;
        }

        public static Bitmap ClipImage(Bitmap b, Rectangle clippingRegion)
        {
            int Width = b.Width;
            int Height = b.Height;
            Rectangle bR = new Rectangle(0, 0, b.Width, b.Height);
            
            Bitmap b2 = new Bitmap(clippingRegion.Width, clippingRegion.Height, PixelFormat.Format32bppRgb);
            Graphics g = Graphics.FromImage(b2);

            clippingRegion.Intersect(bR);
            g.DrawImage(b, new Rectangle(0, 0, clippingRegion.Width, clippingRegion.Height), clippingRegion, GraphicsUnit.Pixel);
            return b2;
        }

        public static Bitmap ClipImageExactCopy(Bitmap bFrom, Rectangle src)
        {
            Bitmap bIn = new Bitmap(src.Width, src.Height, bFrom.PixelFormat);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmDataIn = bIn.LockBits(new Rectangle(0, 0, bIn.Width, bIn.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            BitmapData bmDataFrom = bFrom.LockBits(new Rectangle(0, 0, bFrom.Width, bFrom.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            int strideIn = bmDataIn.Stride;
            System.IntPtr Scan0In = bmDataIn.Scan0;

            unsafe
            {
                for (int y = 0; y < bIn.Height; ++y)
                {
                    Int32* pIn = (Int32*)((byte*)(void*)Scan0In + bmDataIn.Stride * (y));
                    Int32* pFrom = (Int32*)((byte*)(void*)bmDataFrom.Scan0 + bmDataFrom.Stride * (y + src.Y)) + src.X;
                    for (int x = 0; x < bIn.Width; ++x)
                    {
                        *pIn = *pFrom;
                        pIn++;
                        pFrom++;
                    }
                }
            }

            bIn.UnlockBits(bmDataIn);
            bFrom.UnlockBits(bmDataFrom);

            return bIn;
        }


        public static ImageHolder ClipImageExactCopy(ImageHolder bFrom, Rectangle src, int Depth)
        {

            if (src.X == 0 && src.Y == 0 & src.Width == bFrom.Width && src.Height == bFrom.Height)
            {
                return bFrom.Clone();
            }

            ImageHolder ImageOut = new ImageHolder(src.Width, src.Height, bFrom.NChannels);

            int X = src.X, Y = src.Y;
            int Width = src.Width, Height = src.Height;
            bool RangeFixed = false;
            bool TopFixed = false;
            bool BottomFixed = false;
            bool RightFixed = false;
            bool LeftFixed = false;

            float[, ,] Data = bFrom.ImageData;
            float[, ,] DataOut = ImageOut.ImageData;
            double R = 0;
            //first check to see if this fits into the existing image (bfrom)
            if (Y < 0)
            {
                TopFixed = true;
                RangeFixed = true;
            }
            if (X < 0)
            {
                LeftFixed = true;
                RangeFixed = true;
            }
            if (Height + Y > bFrom.Height)
            {
                BottomFixed = true;
                RangeFixed = true;
            }
            if (Width + X > bFrom.Width)
            {
                RightFixed = true;
                RangeFixed = true;
            }

            if (RangeFixed)
            {
                List<double> vals = new List<double>();
                double v = 0;
                int cc = 0;
                R = 0;
                int tx = X;
                int ty = Y;

                int ex = src.Right;
                int ey = src.Bottom;

                if (X < 0)
                    tx = 0;
                if (Y < 0)
                    ty = 0;

                if (ex > Data.GetLength(1))
                    ex = Data.GetLength(1);

                if (ey > Data.GetLength(0))
                    ey = Data.GetLength(0);

                if (TopFixed)
                {
                    for (int i = tx; i < ex; i++)
                    {
                        v = Data[0, i, 0];
                        vals.Add(v);
                        R += v;
                        cc++;
                    }
                }

                if (BottomFixed)
                {
                    int Top = Data.GetLength(0) - 1;
                    for (int i = tx; i < ex; i++)
                    {
                        v = Data[Top, i, 0];
                        vals.Add(v);
                        R += v;
                        cc++;
                    }
                }

                if (LeftFixed)
                {
                    for (int i = ty; i < ey; i++)
                    {
                        v = Data[i, 0, 0];
                        vals.Add(v);
                        R += v;
                        cc++;
                    }
                }

                if (RightFixed)
                {
                    int RightMost = Data.GetLength(1) - 1;
                    for (int i = ty; i < ey; i++)
                    {
                        v = Data[i, RightMost, 0];
                        vals.Add(v);
                        R += v;
                        cc++;
                    }
                }

                if (vals.Count > 0)
                {
                    vals.Sort();
                    R = vals[vals.Count / 2];
                }
                else if (cc > 0)
                    R = R / cc;
                else
                    R = 60000;
                ImageOut.Clear((float)R);
            }

            float ROut = (float)R;
            int Xout = 0, Yout;
            unchecked
            {
                for (int i = src.Left; i < src.Right; i++)
                {
                    Yout = 0;
                    if (i > 0 && i < Data.GetLength(1))
                    {
                        for (int j = src.Top; j < src.Bottom; j++)
                        {
                            if (j >= 0 && j < Data.GetLength(0))
                            {
                                DataOut[Yout, Xout, 0] = Data[j, i, 0];
                            }
                            else
                            {
                                DataOut[Yout, Xout, 0] = ROut;
                            }
                            Yout++;

                        }
                    }
                    else
                    {
                        for (int j = 0; j < src.Height; j++)
                        {
                            DataOut[j, Xout, 0] = ROut;
                            Yout++;
                        }
                    }
                    Xout++;

                }
            }
            return ImageOut;
        }


        public static ImageHolder ClipImageExactCopyO(ImageHolder bFrom, Rectangle src, int Depth)
        {
            // try
            {
                ImageHolder ImageOut = new ImageHolder(src.Width, src.Height, bFrom.NChannels);

                int X = src.X, Y = src.Y;
                int Width = src.Width, Height = src.Height;
                bool RangeFixed = false;
                float[, ,] Data = bFrom.ImageData;
                double R = 0;
                //first check to see if this fits into the existing image (bfrom)
                if (Y < 0)
                {
                    Y = 0;
                    Height = src.Bottom - Y;
                    RangeFixed = true;
                }
                if (X < 0)
                {
                    X = 0;
                    Width = src.Right - X;
                    RangeFixed = true;
                }
                if (Height + Y > bFrom.Height)
                {
                    Height = bFrom.Height - Y - 1;

                    RangeFixed = true;
                }
                if (Width + X > bFrom.Width)
                {
                    Width = bFrom.Width - X - 1;
                    RangeFixed = true;
                }


                int Xo = 0, Yo = 0;

                if (src.Top < 0)
                {
                    Yo = -1 * src.Top;
                    RangeFixed = true;
                }
                if (src.Left < 0)
                {
                    Xo = -1 * src.Left;
                    RangeFixed = true;
                }

                if (RangeFixed)
                {
                    List<double> vals = new List<double>();
                    double v = 0;
                    int cc = 0;
                    R = 0;
                    for (int i = X; i < Width; i++)
                    {
                        v = Data[0, i, 0];
                        vals.Add(v);
                        R += v;
                        cc++;
                    }
                    int Top = Data.GetLength(0) - 1;
                    for (int i = X; i < Width; i++)
                    {
                        v = Data[Top, i, 0];
                        vals.Add(v);
                        R += v;
                        cc++;
                    }

                    if (vals.Count > 0)
                    {
                        vals.Sort();
                        R = vals[vals.Count / 2];
                    }
                    else
                        R = R / cc;
                    ImageOut.Clear((float)R);
                }

                {
                    ImageOut.ROI = new Rectangle(Xo, Yo, Width, Height);
                    bFrom.ROI = new Rectangle(X, Y, Width, Height);

                    bFrom.CopyTo(ImageOut);
                    bFrom.ResetROI();
                }

                return ImageOut;

            }
            /* catch (Exception ex)
             {
                 if (ex.Message == "OpenCV: src.depth() == dst.depth() && src.size == dst.size")
                 {
                     try
                     {
                         if (Depth < 3)
                             return ClipImageExactCopy(bFrom, src, Depth + 1);
                         else
                             throw new Exception("Unable to clip");
                     }
                     catch (Exception ex2)
                     {
                         System.Diagnostics.Debug.Print(ex2.Message);
                     }
                 }
                 System.Diagnostics.Debug.Print(ex.Message);
             }*/
            return null;
        }

        public static double[,] ClipImageExactCopy(double[,] bFrom, Rectangle src)
        {
            // try
            {
                double[,] ImageOut = new double[src.Width, src.Height];

                int X = src.X, Y = src.Y;
                int Width = src.Width, Height = src.Height;
                bool RangeFixed = false;


                //first check to see if this fits into the existing image (bfrom)
                if (Y < 0)
                {
                    Y = 0;
                    Height = src.Bottom - Y;
                    RangeFixed = true;
                }
                if (X < 0)
                {
                    X = 0;
                    Width = src.Right - X;
                    RangeFixed = true;
                }
                if (Height + Y > bFrom.GetLength(0))
                {
                    Height = bFrom.GetLength(0) - Y - 1;
                    RangeFixed = true;
                }
                if (Width + X > bFrom.GetLength(1))
                {
                    Width = bFrom.GetLength(1) - X - 1;
                    RangeFixed = true;
                }


                int Xo = 0, Yo = 0;

                if (src.Top < 0)
                {
                    Yo = -1 * src.Top;
                    RangeFixed = true;
                }
                if (src.Left < 0)
                {
                    Xo = -1 * src.Left;
                    RangeFixed = true;
                }

                if (RangeFixed)
                {
                    double R = 0;
                    for (int i = 0; i < bFrom.GetLength(1); i++)
                    {
                        R += bFrom[0, i];
                    }
                    R /= bFrom.GetLength(1);

                }

                //ImageOut.ROI = new Rectangle(Xo, Yo, Width, Height);
                //bFrom.ROI = new Rectangle(X, Y, Width, Height);

                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < Height; j++)
                    {
                        ImageOut[j, i] = bFrom[Y + j, X + i];
                    }


                return ImageOut;

            }
            /* catch (Exception ex)
             {
                 if (ex.Message == "OpenCV: src.depth() == dst.depth() && src.size == dst.size")
                 {
                     try
                     {
                         if (Depth < 3)
                             return ClipImageExactCopy(bFrom, src, Depth + 1);
                         else
                             throw new Exception("Unable to clip");
                     }
                     catch (Exception ex2)
                     {
                         System.Diagnostics.Debug.Print(ex2.Message);
                     }
                 }
                 System.Diagnostics.Debug.Print(ex.Message);
             }*/
            return null;
        }

        #endregion

        #region ImageMath

        public static bool Brightness(Bitmap b, int nBrightness)
        {
            if (nBrightness < -255 || nBrightness > 255)
                return false;

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            int nVal = 0;

            unsafe
            {
                for (int y = 0; y < b.Height; ++y)
                {
                    byte* p = (byte*)(void*)Scan0 + bmData.Stride * y;
                    for (int x = 0; x < b.Width; ++x)
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

                        p += 4;
                    }
                }
            }

            b.UnlockBits(bmData);

            return true;
        }

        public static bool Contrast(Bitmap b, int nContrast)
        {
            if (nContrast < -100) return false;
            if (nContrast > 100) return false;

            double pixel = 0, contrast = (100.0 + nContrast) / 100.0;

            contrast *= contrast;

            int red, green, blue;

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

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

        public static Bitmap Invert(Bitmap b)
        {
            Bitmap bOut = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppRgb);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            unsafe
            {
                for (int y = 0; y < b.Height; ++y)
                {
                    byte* p = (byte*)(void*)bmData.Scan0 + bmData.Stride * y;
                    byte* pOut = (byte*)(void*)bmDataOut.Scan0 + bmDataOut.Stride * y;
                    for (int x = 0; x < b.Width; ++x)
                    {
                        pOut[0] = (byte)(255 - p[0]);
                        pOut[1] = (byte)(255 - p[1]);
                        pOut[2] = (byte)(255 - p[2]);
                        pOut[3] = (byte)(p[3]);
                        p += 4;
                        pOut += 4;
                    }
                }
            }

            b.UnlockBits(bmData);
            bOut.UnlockBits(bmDataOut);
            // Graphics.FromImage(bOut).Clear(Color.Blue);
            return bOut;
        }

        public static Bitmap GrayScale(Bitmap b)
        {
            Bitmap bOut = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppRgb);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            byte Intensity;
            unsafe
            {
                for (int y = 0; y < b.Height; ++y)
                {
                    byte* p = (byte*)(void*)bmData.Scan0 + bmData.Stride * y;
                    byte* pOut = (byte*)(void*)bmDataOut.Scan0 + bmDataOut.Stride * y;
                    for (int x = 0; x < b.Width; ++x)
                    {
                        Intensity = (byte)((double)(p[0] + p[1] + p[2]) / 3d);
                        pOut[0] = Intensity;
                        pOut[1] = Intensity;
                        pOut[2] = Intensity;
                        pOut[3] = (byte)(p[3]);
                        p += 4;
                        pOut += 4;
                    }
                }
            }

            b.UnlockBits(bmData);
            bOut.UnlockBits(bmDataOut);
            // Graphics.FromImage(bOut).Clear(Color.Blue);
            return bOut;
        }

        public static Bitmap ThresholdImage(Bitmap image, Int32 AverageIntensityThreshold)
        {
            Bitmap bOut = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppRgb);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            Int32 averageValue;
            Int32 Threshold = 3 * AverageIntensityThreshold;
            unsafe
            {
                for (int y = 0; y < image.Height; ++y)
                {
                    byte* p = (byte*)(void*)bmData.Scan0 + bmData.Stride * y;
                    Int32* pOut = (Int32*)((byte*)(void*)bmDataOut.Scan0 + bmDataOut.Stride * y);
                    for (int x = 0; x < image.Width; ++x)
                    {
                        averageValue = p[0] + p[1] + p[2];
                        if (averageValue > Threshold)
                        {
                            *pOut = Color.White.ToArgb();
                        }
                        else
                            *pOut = Color.Black.ToArgb();
                        p += 4;
                        pOut++;
                    }
                }
            }

            image.UnlockBits(bmData);
            bOut.UnlockBits(bmDataOut);
            // Graphics.FromImage(bOut).Clear(Color.Blue);
            return bOut;
        }

        public static Bitmap ThresholdImage(Bitmap image, Int32 AverageIntensityThreshold, out int NumPixels)
        {
            Bitmap bOut = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppRgb);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            Int32 averageValue;
            Int32 Threshold = 3 * AverageIntensityThreshold;
            NumPixels = 0;
            unsafe
            {
                for (int y = 0; y < image.Height; ++y)
                {
                    byte* p = (byte*)(void*)bmData.Scan0 + bmData.Stride * y;
                    Int32* pOut = (Int32*)((byte*)(void*)bmDataOut.Scan0 + bmDataOut.Stride * y);
                    for (int x = 0; x < image.Width; ++x)
                    {
                        averageValue = p[0] + p[1] + p[2];
                        if (averageValue > Threshold)
                        {
                            *pOut = Color.White.ToArgb();
                        }
                        else
                        {
                            *pOut = Color.Black.ToArgb();
                            NumPixels++;
                        }
                        p += 4;
                        pOut++;
                    }
                }
            }

            image.UnlockBits(bmData);
            bOut.UnlockBits(bmDataOut);
            // Graphics.FromImage(bOut).Clear(Color.Blue);
            return bOut;
        }

        public static Bitmap RotateImage(Bitmap b, float Degrees)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(returnBitmap);
            //move rotation point to center of image
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
            //rotate
            g.RotateTransform(Degrees);
            //move image back
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            //draw passed in image onto graphics object
            g.DrawImage(b, new Point(0, 0));
            return returnBitmap;
        }

        public static float[,] SliceAndPadZeroArrayToFloat(ImageHolder InImage, int Channel, int PaddingX, int PaddingY)
        {
            float[, ,] InArray = InImage.ImageData;

            float[,] OutArray = null;

            OutArray = new float[PaddingX, PaddingY];
            int OffsetX = (int)((InArray.GetLength(0) - PaddingX) / -2d);
            int OffsetY = (int)((InArray.GetLength(1) - PaddingY) / -2d);

            for (int i = 0; i < InArray.GetLength(0); i++)
            {
                for (int j = 0; j < InArray.GetLength(1); j++)
                {
                    OutArray[i + OffsetX, j + OffsetY] = (float)InArray[i, j, Channel];
                }
            }

            return OutArray;
        }

        public static float[,] SliceAndPadZeroArrayToFloat(double[,] InArray, int PaddingX, int PaddingY)
        {

            float[,] OutArray = null;

            OutArray = new float[PaddingX, PaddingY];
            int OffsetX = (int)((InArray.GetLength(0) - PaddingX) / -2d);
            int OffsetY = (int)((InArray.GetLength(1) - PaddingY) / -2d);

            for (int i = 0; i < InArray.GetLength(0); i++)
            {
                for (int j = 0; j < InArray.GetLength(1); j++)
                {
                    OutArray[i + OffsetX, j + OffsetY] = (float)InArray[i, j];
                }
            }

            return OutArray;
        }

        public static Bitmap CombineImages(Bitmap FirstChannel, Color FirstChannelColor, Bitmap SecondChannel, Color SecondChannelColor)
        {
            int iWidth = FirstChannel.Width;
            int iHeight = FirstChannel.Height;

            Bitmap bOut = new Bitmap(FirstChannel.Width, FirstChannel.Height, PixelFormat.Format32bppRgb);

            BitmapData bmd = FirstChannel.LockBits(new Rectangle(0, 0, FirstChannel.Width, FirstChannel.Height), ImageLockMode.WriteOnly, FirstChannel.PixelFormat);
            BitmapData bmd2 = SecondChannel.LockBits(new Rectangle(0, 0, SecondChannel.Width, SecondChannel.Height), ImageLockMode.WriteOnly, SecondChannel.PixelFormat);
            BitmapData bmdOut = bOut.LockBits(new Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.WriteOnly, bOut.PixelFormat);


            double R1, G1, B1;
            double R2, G2, B2;

            R1 = FirstChannelColor.R;
            G1 = FirstChannelColor.G;
            B1 = FirstChannelColor.B;

            R2 = SecondChannelColor.R;
            G2 = SecondChannelColor.G;
            B2 = SecondChannelColor.B;

            double g1, g2;
            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);
                        byte* scanline2 = ((byte*)bmd2.Scan0 + y * bmd2.Stride);
                        byte* scanlineOut = ((byte*)bmdOut.Scan0 + y * bmdOut.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            g1 = (*scanline) / 255d;
                            g2 = (*scanline2) / 255d;

                            *scanlineOut = (byte)(g1 * B1 + g2 * B2);
                            *(scanlineOut + 1) = (byte)(g1 * G1 + g2 * G2);
                            *(scanlineOut + 2) = (byte)(g1 * R1 + g2 * R2);

                            scanline += 4;
                            scanline2 += 4;
                            scanlineOut += 4;
                        }
                    }
                }
            }
            FirstChannel.UnlockBits(bmd);
            SecondChannel.UnlockBits(bmd2);
            bOut.UnlockBits(bmdOut);
            return bOut;

        }

        #endregion



        #region Profiles
        /// <summary>
        /// used with emgu data, second dimension is color channel
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static double[] ConvertToIntensity(ushort[,] Data)
        {
            double[] outArray = new double[Data.GetLength(1)];
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                outArray[i] = (double)(Data[0, i] + Data[1, i] + Data[2, i]) / 3d;
            }
            return outArray;
        }

        public static double[] ConvertToIntensity(float[] Data)
        {
            double[] outArray = new double[Data.Length];
            for (int i = 0; i < Data.Length; i++)
            {
                outArray[i] = (double)Data[i];
            }
            return outArray;
        }


        /// <summary>
        /// Designed to convert RGBA data to intensities
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static double[] ConvertToIntensity(Int32[] Data)
        {
            double[] outArray = new double[Data.Length];
            for (int i = 0; i < Data.Length; i++)
            {
                Color color = Color.FromArgb(Data[i]);
                outArray[i] = (double)(color.R + color.G + color.B) / 3d;
            }
            return outArray;
        }

        public static Int32[] GetProfileLine(this Bitmap SourceImage, Point p1, Point p2)
        {
            int iWidth = SourceImage.Width;
            int iHeight = SourceImage.Height;
            int Length = (int)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            Int32[] Profile = new Int32[Length];

            BitmapData bmd = SourceImage.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height), ImageLockMode.WriteOnly, SourceImage.PixelFormat);

            double tx = (double)(p2.X - p1.X) / (double)Length;
            double ty = (double)(p2.Y - p1.Y) / (double)Length;
            double sX = p1.X;
            double sY = p1.Y;
            int x = 0, y = 0;

            unsafe
            {
                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int t = 0; t < Length; t++)
                    {
                        y = (int)Math.Round(sY);
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);
                        x = (int)Math.Round(sX);
                        Profile[t] = scanline[x];
                        sY += ty;
                        sX += tx;
                    }
                }
                else
                    throw new Exception("Only works for 32 bit images.  Please convert your image");

            }
            SourceImage.UnlockBits(bmd);
            return Profile;
        }
        #endregion

        private static void CreateMovieFFMPEG(string MovieFilename, string FramePattern, string FrameExtension)
        {

            if (File.Exists(MovieFilename) == true)
                File.Delete(MovieFilename);

            Process FFmpeg = new Process();
            FFmpeg.StartInfo.UseShellExecute = false;
            FFmpeg.StartInfo.RedirectStandardError = true;
            FFmpeg.StartInfo.RedirectStandardOutput = true;
            FFmpeg.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg.exe";
            FFmpeg.StartInfo.Arguments = string.Format("-c:v mjpeg -i {0}%03d.{1} \"{2}\"", FramePattern, FrameExtension, MovieFilename);
            System.Diagnostics.Debug.Print(FFmpeg.StartInfo.FileName + " " + FFmpeg.StartInfo.Arguments);
            FFmpeg.StartInfo.CreateNoWindow = true;

            FFmpeg.Start();

            //  FFmpeg.WaitForExit();
            /*   //Create the output and streamreader to get the output
               string output = null; StreamReader srOutput = null;

               //get the output
               srOutput = FFmpeg.StandardError;

               //now put it in a string
               output = srOutput.ReadToEnd();

               System.Diagnostics.Debug.Print(output);*/

            // FFmpeg.Close();
        }

        private static void CreateAVIVideoFFMPEG(string AVIFilename, string[] Frames)
        {
            if (File.Exists(AVIFilename) == true)
                File.Delete(AVIFilename);

            string FramePattern = Path.GetFileNameWithoutExtension(Frames[0]);
            string FrameExtension = Path.GetExtension(Frames[0]);

            Process FFmpeg = new Process();
            FFmpeg.StartInfo.UseShellExecute = false;
            FFmpeg.StartInfo.RedirectStandardError = true;
            FFmpeg.StartInfo.RedirectStandardOutput = true;
            FFmpeg.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg.exe";
            FFmpeg.StartInfo.Arguments = string.Format("-c:v mjpeg -i {0}%03d.{1} \"{2}\"", FramePattern, FrameExtension, AVIFilename);
            System.Diagnostics.Debug.Print(FFmpeg.StartInfo.FileName + " " + FFmpeg.StartInfo.Arguments);
            FFmpeg.StartInfo.CreateNoWindow = true;

            FFmpeg.Start();
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private static Random rnd = new Random(DateTime.Now.Millisecond);

        private static void CreateAVIVideoFFMPEG(string AVIFilename, ImageLibrary Frames)
        {
            if (File.Exists(AVIFilename) == true)
                File.Delete(AVIFilename);

            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            //  bmp1.Save(@"c:\TestPhotoQualityFifty.jpg", jgpEncoder, myEncoderParameters);

            string TempFolder = Path.GetTempPath() + "\\" + rnd.Next(1000).ToString();

            Directory.CreateDirectory(TempFolder);

            for (int i = 0; i < Frames.Count; i++)
            {
                Frames[i].ToBitmap().Save(TempFolder + "\\" + string.Format("I{0:00000}.jpg", i), jgpEncoder, myEncoderParameters);
            }

            string FramePattern = "I";
            string FrameExtension = "jpg";

            Process FFmpeg = new Process();
            FFmpeg.StartInfo.UseShellExecute = false;
            FFmpeg.StartInfo.RedirectStandardError = true;
            FFmpeg.StartInfo.RedirectStandardOutput = true;
            FFmpeg.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg.exe";
            FFmpeg.StartInfo.Arguments = string.Format("-c:v mjpeg -i {0}%03d.{1} \"{2}\"", FramePattern, FrameExtension, AVIFilename);
            System.Diagnostics.Debug.Print(FFmpeg.StartInfo.FileName + " " + FFmpeg.StartInfo.Arguments);
            FFmpeg.StartInfo.CreateNoWindow = true;

            FFmpeg.Start();

            FFmpeg.WaitForExit();


            Directory.Delete(TempFolder, true);
        }
        private static void CreateAVIVideoFFMPEG(string AVIFilename, ImageLibrary Frames, int SkipFrames)
        {
            if (File.Exists(AVIFilename) == true)
                File.Delete(AVIFilename);

            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            //  bmp1.Save(@"c:\TestPhotoQualityFifty.jpg", jgpEncoder, myEncoderParameters);

            string TempFolder = Path.GetTempPath() + "\\" + rnd.Next(1000).ToString();

            Directory.CreateDirectory(TempFolder);

            int cc = 0;
            for (int i = 0; i < Frames.Count; i += SkipFrames)
            {
                Frames[i].ToBitmap().Save(TempFolder + "\\" + string.Format("I{0:00000}.jpg", cc), jgpEncoder, myEncoderParameters);
                cc++;
            }

            string FramePattern = "I";
            string FrameExtension = "jpg";

            Process FFmpeg = new Process();
            FFmpeg.StartInfo.UseShellExecute = false;
            FFmpeg.StartInfo.RedirectStandardError = true;
            FFmpeg.StartInfo.RedirectStandardOutput = true;
            FFmpeg.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg.exe";
            FFmpeg.StartInfo.Arguments = string.Format("-c:v mjpeg -i {0}%03d.{1} \"{2}\"", FramePattern, FrameExtension, AVIFilename);
            System.Diagnostics.Debug.Print(FFmpeg.StartInfo.FileName + " " + FFmpeg.StartInfo.Arguments);
            FFmpeg.StartInfo.CreateNoWindow = true;

            FFmpeg.Start();

            FFmpeg.WaitForExit();


            Directory.Delete(TempFolder, true);
        }
        private static void CreateAVIVideoFFMPEG(string AVIFilename, Bitmap[] Frames)
        {
            if (File.Exists(AVIFilename) == true)
                File.Delete(AVIFilename);

            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            //  bmp1.Save(@"c:\TestPhotoQualityFifty.jpg", jgpEncoder, myEncoderParameters);

            string TempFolder = Path.GetTempPath() + "\\" + rnd.Next(1000).ToString();

            Directory.CreateDirectory(TempFolder);

            for (int i = 0; i < Frames.Length; i++)
            {
                Frames[i].Save(TempFolder + "\\" + string.Format("I{0:00000}.jpg", i), jgpEncoder, myEncoderParameters);
            }

            string FramePattern = "I";
            string FrameExtension = "jpg";

            Process FFmpeg = new Process();
            FFmpeg.StartInfo.UseShellExecute = false;
            FFmpeg.StartInfo.RedirectStandardError = true;
            FFmpeg.StartInfo.RedirectStandardOutput = true;
            FFmpeg.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg.exe";
            FFmpeg.StartInfo.Arguments = string.Format("-c:v mjpeg -i {0}%03d.{1} \"{2}\"", FramePattern, FrameExtension, AVIFilename);
            System.Diagnostics.Debug.Print(FFmpeg.StartInfo.FileName + " " + FFmpeg.StartInfo.Arguments);
            FFmpeg.StartInfo.CreateNoWindow = true;

            FFmpeg.Start();

            FFmpeg.WaitForExit();


            Directory.Delete(TempFolder, true);
        }


        public static void CreateAVIVideo(string AVIFilename, string[] Frames)
        {
            CreateAVIVideoEMGU(AVIFilename, Frames);
            /*
            Bitmap bmp = new Bitmap(Frames[0]);
            VideoWriter VW = new VideoWriter(AVIFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 15 , bmp.Height, bmp.Width, true);
            int count = 0;
            for (int n = 1; n < Frames.Length; n++)
            {
                if (Frames[n].Trim().Length > 0)
                {
                    var frame = new Emgu.CV.Image<Bgr, byte>(Frames[n]);
                    VW.WriteFrame<Bgr, byte>(frame);
                    count++;
                }
            }
            VW.Dispose();*/
        }
        public static void CreateAVIVideo(string AVIFilename, ImageLibrary Frames)
        {
            CreateAVIVideoEMGU(AVIFilename, Frames);
            /*
            Bitmap bmp = Frames[0].ToBitmap();
            VideoWriter VW = new VideoWriter(AVIFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 15, bmp.Height, bmp.Width, true);

            Bitmap bitmap;
            int count = 0;
            for (int n = 1; n < Frames.Count; n++)
            {
                if (Frames[n] != null)
                {
                    bitmap = Frames[n].ToBitmap();
                    var frame = new Emgu.CV.Image<Bgr, byte>(bitmap);
                    VW.WriteFrame<Bgr, byte>(frame);
                    bitmap.Dispose();
                    count++;
                }
            }

            VW.Dispose();*/
        }
        public static void CreateAVIVideo(string AVIFilename, ImageLibrary Frames, int SkipFrames)
        {
            CreateAVIVideoEMGU(AVIFilename, Frames, SkipFrames);
            /*
            Bitmap bmp = Frames[0].ToBitmap();
            VideoWriter VW = new VideoWriter(AVIFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 15, bmp.Height, bmp.Width, true);

            Bitmap bitmap;
            int count = 0;
            for (int n = 1; n < Frames.Count; n++)
            {
                if (Frames[n] != null)
                {
                    bitmap = Frames[n].ToBitmap();
                    var frame = new Emgu.CV.Image<Bgr, byte>(bitmap);
                    VW.WriteFrame<Bgr, byte>(frame);
                    bitmap.Dispose();
                    count++;
                }
            }

            VW.Dispose();*/
        }
        public static void CreateAVIVideo(string AVIFilename, Bitmap[] Frames)
        {
            CreateAVIVideoEMGU(AVIFilename, Frames);
            /*
            Bitmap bmp = Frames[0];
            VideoWriter VW = new VideoWriter(AVIFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 15, bmp.Height, bmp.Width, true);

            Bitmap bitmap;
            int count = 0;
            for (int n = 1; n < Frames.Length; n++)
            {
                if (Frames[n] != null)
                {
                    bitmap = Frames[n];
                    var frame = new Emgu.CV.Image<Bgr, byte>(bitmap);
                    VW.WriteFrame<Bgr, byte>(frame);
                    count++;
                }
            }
            VW.Dispose();*/
        }


        public static Bitmap ConvertBitmapTo24(this Bitmap bitmap)
        {
            Bitmap temp = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(temp);

            g.DrawImage(bitmap, Point.Empty);
            return temp;
        }


        public static Bitmap ConvertBitmapTo32(this Bitmap bitmap)
        {
            if (!(bitmap.PixelFormat == PixelFormat.Format32bppArgb || bitmap.PixelFormat == PixelFormat.Format32bppPArgb || bitmap.PixelFormat == PixelFormat.Format32bppRgb))
            {
                Bitmap temp = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppRgb);
                Graphics g = Graphics.FromImage(temp);

                g.DrawImage(bitmap, Point.Empty);
                return temp;
            }
            else
                return bitmap;
        }

        private static void CreateAVIVideoEMGU(string AVIFilename, string[] Frames)
        {
            Bitmap bitmap = new Bitmap(Frames[1]);

            int nWidth = (int)(16 * (Math.Floor((double)bitmap.Width / 16d) + 1));

            VideoWriter VW = new VideoWriter(AVIFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 15, nWidth, bitmap.Height, true);


            int count = 0;
            for (int n = 1; n < Frames.Length; n++)
            {
                if (Frames[n].Trim().Length > 0)
                {
                    try
                    {
                        bitmap = new Bitmap(Frames[n]);
                        Bitmap temp = new Bitmap(nWidth, bitmap.Height, PixelFormat.Format32bppRgb);
                        Graphics g = Graphics.FromImage(temp);

                        g.DrawImage(bitmap, Point.Empty);

                        //bitmap =  Frames[n].ToBitmap();
                        var frame = new Emgu.CV.Image<Bgr, byte>(temp);



                        VW.WriteFrame<Bgr, byte>(frame);
                        count++;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(ex.Message);
                    }
                }
            }
            VW.Dispose();
        }

        private static void CreateAVIVideoEMGUInvoke(string AVIFilename, string[] Frames)
        {
            Bitmap bitmap = ConvertBitmapTo32(new Bitmap(Frames[1]));

            BitmapData bmd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

            int Stride = bmd.Stride;

            bitmap.UnlockBits(bmd);

            //  VideoWriter VW = null;

            // VW = new VideoWriter(AVIFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 15, bitmap.Width  , bitmap.Height,true );

            System.Drawing.Size size = new System.Drawing.Size(bitmap.Width / 2, bitmap.Height);
            // unsafe
            {
                //   fixed (int* psize = size)
                {
                    IntPtr _ptr = CvInvoke.cvCreateVideoWriter(AVIFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 15, size, 1);

                    int count = 0;
                    for (int n = 1; n < Frames.Length; n++)
                    {
                        if (Frames[n].Trim().Length > 0)
                        {
                            try
                            {
                                FileInfo file = new FileInfo(Frames[n]);
                                IntPtr ptr = CvInvoke.cvLoadImage(file.FullName, Emgu.CV.CvEnum.LOAD_IMAGE_TYPE.CV_LOAD_IMAGE_ANYCOLOR | Emgu.CV.CvEnum.LOAD_IMAGE_TYPE.CV_LOAD_IMAGE_ANYDEPTH);
                                if (ptr == IntPtr.Zero)
                                    throw new NullReferenceException(String.Format("Unable to load image from file \"{0}\".", file.FullName));

                                CvInvoke.cvWriteFrame(_ptr, ptr);
                                //VW.WriteFrame<Bgr, byte>(frame);
                                count++;
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.Print(ex.Message);
                            }
                        }
                    }
                    CvInvoke.cvReleaseVideoWriter(ref _ptr);
                }
            }
            System.Diagnostics.Debug.Print(size.ToString());
            // VW.Dispose();
        }
        private static void CreateAVIVideoEMGU(string AVIFilename, ImageLibrary Frames)
        {

            ImageHolder bmp = Frames[0];

            int nWidth = (int)(16 * (Math.Floor((double)bmp.Width / 16d) + 1));

            VideoWriter VW = null;

            VW = new VideoWriter(AVIFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 15, bmp.Height, bmp.Width, true);

            Bitmap bitmap;
            int count = 0;
            for (int n = 1; n < Frames.Count; n++)
            {
                if (Frames[n] != null)
                {
                    bitmap = Frames[n].ToBitmap();
                    Bitmap temp = new Bitmap(nWidth, bitmap.Height, PixelFormat.Format32bppRgb);
                    Graphics g = Graphics.FromImage(temp);

                    g.DrawImage(bitmap, Point.Empty);

                    var frame = new Emgu.CV.Image<Bgr, byte>(temp);


                    VW.WriteFrame<Bgr, byte>(frame);
                    bitmap.Dispose();
                    count++;
                }
            }

            VW.Dispose();
        }

        /// <summary>
        /// Creates a compressed AVI file from the frames
        /// </summary>
        /// <param name="AVIFilename"></param>
        /// <param name="Frames"></param>
        /// <param name="SkipFrames">The number of frames to skip between adds</param>
        private static void CreateAVIVideoEMGU(string AVIFilename, ImageLibrary Frames, int SkipFrames)
        {

            ImageHolder bmp = Frames[0];

            int nWidth = (int)(16 * (Math.Floor((double)bmp.Width / 16d) + 1));

            VideoWriter VW = null;

            VW = new VideoWriter(AVIFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 15, bmp.Height, bmp.Width, true);

            Bitmap bitmap;
            int count = 0;
            for (int n = 1; n < Frames.Count; n += SkipFrames)
            {
                if (Frames[n] != null)
                {
                    bitmap = Frames[n].ToBitmap(0);
                    Bitmap temp = new Bitmap(nWidth, bitmap.Height, PixelFormat.Format32bppRgb);
                    Graphics g = Graphics.FromImage(temp);

                    g.DrawImage(bitmap, Point.Empty);

                    var frame = new Emgu.CV.Image<Bgr, byte>(temp);


                    VW.WriteFrame<Bgr, byte>(frame);
                    bitmap.Dispose();
                    count++;
                }
            }

            VW.Dispose();
        }

        private static void CreateAVIVideoEMGU(string AVIFilename, Bitmap[] Frames)
        {

            Bitmap bmp = Frames[0];

            int nWidth = (int)(16 * (Math.Floor((double)bmp.Width / 16d) + 1));

            VideoWriter VW = new VideoWriter(AVIFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 15, bmp.Height, bmp.Width, true);

            Bitmap bitmap;
            int count = 0;
            for (int n = 1; n < Frames.Length; n++)
            {
                if (Frames[n] != null)
                {
                    bitmap = Frames[n];

                    Bitmap temp = new Bitmap(nWidth, bitmap.Height, PixelFormat.Format32bppRgb);
                    Graphics g = Graphics.FromImage(temp);

                    g.DrawImage(bitmap, Point.Empty);

                    var frame = new Emgu.CV.Image<Bgr, byte>(temp);

                    if (bmp.Width != frame.Width || bmp.Height != frame.Height)
                        System.Diagnostics.Debug.Print("");


                    VW.WriteFrame<Bgr, byte>(frame);
                    bitmap.Dispose();
                    count++;
                }
            }

            VW.Dispose();
        }


        /// <summary>
        /// does a weighted blend of two images.  Blendfactor 0 is 100% image1, 1 is 100% image2
        /// </summary>
        /// <param name="image1"></param>
        /// <param name="image2"></param>
        /// <param name="BlendFactor"></param>
        /// <returns></returns>
        public static ImageHolder BlendImages(ImageHolder image1, ImageHolder image2, float BlendFactor)
        {
            ImageHolder iOut = new ImageHolder(image1.Width, image1.Height, 1);
            float[, ,] dOut = iOut.ImageData;
            float[, ,] i1 = image1.ImageData;
            float[, ,] i2 = image2.ImageData;

            int MaxJ = image1.Height - 1;
            for (int i = 0; i < image1.Width; i++)
            {
                for (int j = 0; j < image1.Height; j++)
                {
                    dOut[j, i, 0] = i1[MaxJ- j, i, 0] * (1 - BlendFactor) + i2[MaxJ- j, i, 0] * BlendFactor;

                }
            }
            return iOut;
        }
    }
}
