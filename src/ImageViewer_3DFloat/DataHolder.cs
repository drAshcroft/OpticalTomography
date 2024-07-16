using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using ImageViewer.Filters;
using ImageViewer3D._3DItems;
using MathHelpLib;

namespace ImageViewer3D
{
    public class DataHolder
    {
        private float[, ,] mData;

        public DataHolder(float[, ,] Data)
        {
            mData = Data;


        }

        public DataHolder(double[, ,] Data)
        {
            mData = new float[Data.GetLength(0), Data.GetLength(1), Data.GetLength(2)];
            for (int i = 0; i < Data.GetLength(0); i++)
                for (int j = 0; j < Data.GetLength(1); j++)
                    for (int k = 0; k < Data.GetLength(2); k++)
                        mData[i, j, k] = (float)Data[i, j, k];

        }

        public float[, ,] Data
        {
            get { return mData; }
            set { mData = value; }
        }

        public int Width
        {
            get { return mData.GetLength(0); }
        }

        public int Height
        {
            get { return mData.GetLength(1); }
        }

        public int Depth
        {
            get { return mData.GetLength(2); }
        }

        /// <summary>
        /// Experimental
        /// </summary>
        /// <param name="SliceNumber"></param>
        /// <param name="ViewBounds"></param>
        /// <param name="MinContrast"></param>
        /// <param name="MaxContrast"></param>
        /// <returns></returns>
        private Bitmap GetSliceX(int SliceNumber, Rectangle ViewBounds, double MinContrast, double MaxContrast)
        {
            if (ViewBounds.Width == 0) ViewBounds.Width = 1;
            if (ViewBounds.Height == 0) ViewBounds.Height = 1;
            Bitmap bOut = new Bitmap(ViewBounds.Width, ViewBounds.Height, PixelFormat.Format32bppRgb);
            BitmapData bmd = bOut.LockBits(new Rectangle(0, 0, ViewBounds.Width, ViewBounds.Height), ImageLockMode.WriteOnly, bOut.PixelFormat);

            double iLength = MaxContrast - MinContrast;

            int iWidth = ViewBounds.Width;
            int iHeight = ViewBounds.Height;

            double g;
            byte g2;

            unsafe
            {
                byte* bits;
                int dataY = 0;
                for (int y = ViewBounds.Y; y < ViewBounds.Bottom; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (dataY) * bmd.Stride);
                    for (int x = ViewBounds.X; x < ViewBounds.Right; x++)
                    {
                        g = (255d * (mData[x, y, SliceNumber] - MinContrast) / iLength);
                        if (g > 255) g = 255;
                        if (g < 0) g = 0;
                        g2 = (byte)g;

                        bits = (byte*)scanline;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;

                    }
                    dataY++;
                }
            }

            bOut.UnlockBits(bmd);
            return bOut;
        }

        /// <summary>
        /// Experimental
        /// </summary>
        /// <param name="SliceNumber"></param>
        /// <param name="ViewBounds"></param>
        /// <param name="MinContrast"></param>
        /// <param name="MaxContrast"></param>
        /// <returns></returns>
        private Bitmap GetSliceY(int SliceNumber, Rectangle ViewBounds, double MinContrast, double MaxContrast)
        {
            if (ViewBounds.Width == 0) ViewBounds.Width = 1;
            if (ViewBounds.Height == 0) ViewBounds.Height = 1;
            Bitmap bOut = new Bitmap(ViewBounds.Width, ViewBounds.Height, PixelFormat.Format32bppRgb);
            BitmapData bmd = bOut.LockBits(new Rectangle(0, 0, ViewBounds.Width, ViewBounds.Height), ImageLockMode.WriteOnly, bOut.PixelFormat);

            double iLength = MaxContrast - MinContrast;

            int iWidth = ViewBounds.Width;
            int iHeight = ViewBounds.Height;

            double g;
            byte g2;

            unsafe
            {
                byte* bits;
                int dataY = 0;
                for (int y = ViewBounds.Y; y < ViewBounds.Bottom; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (dataY) * bmd.Stride);
                    for (int x = ViewBounds.X; x < ViewBounds.Right; x++)
                    {
                        g = (255d * (mData[SliceNumber, y, x] - MinContrast) / iLength);
                        if (g > 255) g = 255;
                        if (g < 0) g = 0;
                        g2 = (byte)g;

                        bits = (byte*)scanline;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;

                    }
                }
            }

            bOut.UnlockBits(bmd);
            return bOut;
        }
        /// <summary>
        /// Experimental
        /// </summary>
        /// <param name="SliceNumber"></param>
        /// <param name="ViewBounds"></param>
        /// <param name="MinContrast"></param>
        /// <param name="MaxContrast"></param>
        /// <returns></returns>
        private Bitmap GetSliceZ(int SliceNumber, Rectangle ViewBounds, double MinContrast, double MaxContrast)
        {
            if (ViewBounds.Width == 0) ViewBounds.Width = 1;
            if (ViewBounds.Height == 0) ViewBounds.Height = 1;
            Bitmap bOut = new Bitmap(ViewBounds.Width, ViewBounds.Height, PixelFormat.Format32bppRgb);
            BitmapData bmd = bOut.LockBits(new Rectangle(0, 0, ViewBounds.Width, ViewBounds.Height), ImageLockMode.WriteOnly, bOut.PixelFormat);

            double iLength = MaxContrast - MinContrast;

            int iWidth = ViewBounds.Width;
            int iHeight = ViewBounds.Height;

            double g;
            byte g2;

            unsafe
            {
                byte* bits;
                int dataY = 0;
                for (int y = ViewBounds.Y; y < ViewBounds.Bottom; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (dataY) * bmd.Stride);
                    for (int x = ViewBounds.X; x < ViewBounds.Right; x++)
                    {
                        g = (255d * (mData[x, SliceNumber, y] - MinContrast) / iLength);
                        if (g > 255) g = 255;
                        if (g < 0) g = 0;
                        g2 = (byte)g;

                        bits = (byte*)scanline;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;

                    }
                }
            }

            bOut.UnlockBits(bmd);
            return bOut;
        }


        private Bitmap GetSliceX(int SliceNumber, double MinContrast, double MaxContrast)
        {
            int iWidth = mData.GetLength(0);
            int iHeight = mData.GetLength(1);

            Bitmap bOut = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = bOut.LockBits(new Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.WriteOnly, bOut.PixelFormat);

            double iLength = MaxContrast - MinContrast;

            double g;
            byte g2;

            unsafe
            {
                byte* bits;
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                    for (int x = 0; x < iWidth; x++)
                    {
                        g = (255d * (mData[x, y, SliceNumber] - MinContrast) / iLength);
                        if (g > 255) g = 255;
                        if (g < 0) g = 0;
                        g2 = (byte)g;

                        bits = (byte*)scanline;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;

                    }
                }
            }

            bOut.UnlockBits(bmd);
            return bOut;
        }

        private Bitmap GetSliceY(int SliceNumber, double MinContrast, double MaxContrast)
        {
            int iWidth = mData.GetLength(2);
            int iHeight = mData.GetLength(1);

            Bitmap bOut = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = bOut.LockBits(new Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.WriteOnly, bOut.PixelFormat);

            double iLength = MaxContrast - MinContrast;

            double g;
            byte g2;

            unsafe
            {
                byte* bits;
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                    for (int x = 0; x < iWidth; x++)
                    {
                        g = (255d * (mData[SliceNumber, y, x] - MinContrast) / iLength);
                        if (g > 255) g = 255;
                        if (g < 0) g = 0;
                        g2 = (byte)g;

                        bits = (byte*)scanline;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;

                    }
                }
            }

            bOut.UnlockBits(bmd);
            return bOut;
        }

        private Bitmap GetSliceZ(int SliceNumber, double MinContrast, double MaxContrast)
        {
            int iWidth = mData.GetLength(0);
            int iHeight = mData.GetLength(2);

            Bitmap bOut = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = bOut.LockBits(new Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.WriteOnly, bOut.PixelFormat);
            double iLength = MaxContrast - MinContrast;

            double g;
            byte g2;

            unsafe
            {
                byte* bits;
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                    for (int x = 0; x < iWidth; x++)
                    {
                        g = (255d * (mData[x, SliceNumber, y] - MinContrast) / iLength);
                        if (g > 255) g = 255;
                        if (g < 0) g = 0;
                        g2 = (byte)g;

                        bits = (byte*)scanline;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;

                    }
                }
            }

            bOut.UnlockBits(bmd);
            return bOut;
        }


        public Bitmap GetSlice(int Axis, int SliceNumber, Rectangle ViewBounds, double MinContrast, double MaxContrast)
        {
            if (Axis == 0)
            {
                return GetSliceX(SliceNumber, ViewBounds, MinContrast, MaxContrast);
            }
            else if (Axis == 1)
            {
                return GetSliceY(SliceNumber, ViewBounds, MinContrast, MaxContrast);
            }
            else
            {
                return GetSliceZ(SliceNumber, ViewBounds, MinContrast, MaxContrast);
            }

        }

        public Bitmap GetSlice(int Axis, int SliceNumber, double MinContrast, double MaxContrast)
        {
            if (Axis == 0)
            {
                return GetSliceX(SliceNumber, MinContrast, MaxContrast);
            }
            else if (Axis == 1)
            {
                return GetSliceY(SliceNumber, MinContrast, MaxContrast);
            }
            else
            {
                return GetSliceZ(SliceNumber, MinContrast, MaxContrast);
            }

        }


        private double[,] GetSliceXDouble(int SliceNumber, Rectangle ViewBounds)
        {
            if (ViewBounds.Width == 0) ViewBounds.Width = 1;
            if (ViewBounds.Height == 0) ViewBounds.Height = 1;
            double[,] bOut = new double[ViewBounds.Width, ViewBounds.Height];

            int iWidth = ViewBounds.Width;
            int iHeight = ViewBounds.Height;

            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    bOut[x, y] = mData[x, y, SliceNumber];
                }
            }
            return bOut;
        }

        private double[,] GetSliceYDouble(int SliceNumber, Rectangle ViewBounds)
        {
            if (ViewBounds.Width == 0) ViewBounds.Width = 1;
            if (ViewBounds.Height == 0) ViewBounds.Height = 1;
            double[,] bOut = new double[ViewBounds.Width, ViewBounds.Height];

            int iWidth = ViewBounds.Width;
            int iHeight = ViewBounds.Height;

            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    bOut[x, y] = mData[SliceNumber, x, y];
                }
            }

            return bOut;
        }

        private double[,] GetSliceZDouble(int SliceNumber, Rectangle ViewBounds)
        {
            if (ViewBounds.Width == 0) ViewBounds.Width = 1;
            if (ViewBounds.Height == 0) ViewBounds.Height = 1;
            double[,] bOut = new double[ViewBounds.Width, ViewBounds.Height];

            int iWidth = ViewBounds.Width;
            int iHeight = ViewBounds.Height;

            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    bOut[x, y] = mData[y, SliceNumber, x];
                }
            }

            return bOut;
        }

        public double[,] GetSliceDouble(int Axis, int SliceNumber, Rectangle ViewBounds)
        {
            if (Axis == 0)
            {
                return GetSliceXDouble(SliceNumber, ViewBounds);
            }
            else if (Axis == 1)
            {
                return GetSliceZDouble(SliceNumber, ViewBounds);
            }
            else
            {
                return GetSliceYDouble(SliceNumber, ViewBounds);
            }

        }


        private double[] GetProfileX(int SliceNumber, Point p1, Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            double LineLength = Math.Sqrt(dx * dx + dy * dy);
            int Length = (int)Math.Truncate(LineLength);

            double[] LineValues = new double[Length];
            double x, y;
            int xI, yI;
            double StepSize = 1d / LineLength;

            int cc = 0;

            try
            {
                for (double u = 0; u < 1; u += StepSize)
                {
                    x = dx * u + p2.X;
                    y = dy * u + p2.Y;
                    xI = (int)Math.Round(x);
                    yI = (int)Math.Round(y);
                    LineValues[cc] = mData[xI, yI, SliceNumber];
                    cc++;
                }
            }
            catch { }
            return LineValues;
        }

        private double[] GetProfileY(int SliceNumber, Point p1, Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            double LineLength = Math.Sqrt(dx * dx + dy * dy);
            int Length = (int)Math.Truncate(LineLength);
            //System.Diagnostics.Debug.Print(Length.ToString());
            double[] LineValues = new double[Length];
            double x, y;
            int xI, yI;
            double StepSize = 1d / LineLength;

            int cc = 0;
            try
            {
                for (double u = 0; u < 1; u += StepSize)
                {
                    x = dx * u + p2.X;
                    y = dy * u + p2.Y;
                    xI = (int)Math.Floor(x);
                    yI = (int)Math.Floor(y);
                    LineValues[cc] += mData[xI, SliceNumber, yI];
                    cc++;
                }
            }
            catch { }


            return LineValues;
        }

        private double[] GetProfileZ(int SliceNumber, Point p1, Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            double LineLength = Math.Sqrt(dx * dx + dy * dy);
            int Length = (int)Math.Truncate(LineLength);

            double[] LineValues = new double[Length];
            double x, y;
            int xI, yI;
            double StepSize = 1d / LineLength;

            int cc = 0;
            try
            {
                for (double u = 0; u < 1; u += StepSize)
                {
                    x = dx * u + p2.X;
                    y = dy * u + p2.Y;
                    xI = (int)Math.Floor(x);
                    yI = (int)Math.Floor(y);
                    LineValues[cc] = mData[SliceNumber, yI, xI];

                    cc++;
                }
            }
            catch { }
            if (dx > 10)
                System.Diagnostics.Debug.Print("");
            return LineValues;
        }


        private double GetPointX(int SliceNumber, Point p1)
        {
            int x, y;
            try
            {
                x = p1.X;
                y = p1.Y;
                return mData[x, y, SliceNumber];
            }
            catch { }
            return 0;
        }

        private double GetPointY(int SliceNumber, Point p1)
        {
            int x, y;
            try
            {

                x = p1.X;
                y = p1.Y;
                return mData[x, SliceNumber, y];
            }
            catch { }
            return 0;
        }

        private double GetPointZ(int SliceNumber, Point p1)
        {
            int x, y;
            try
            {
                x = p1.X;
                y = p1.Y;

                return mData[SliceNumber, y, x];

            }
            catch { }
            return 0;
        }

        public double[] GetProfile(int Axis, int SliceNumber, Point P1, Point P2)
        {
            if (Axis == 0)
            {
                return GetProfileX(SliceNumber, P1, P2);
            }
            else if (Axis == 1)
            {
                return GetProfileZ(SliceNumber, P1, P2);
            }
            else
            {
                return GetProfileY(SliceNumber, P1, P2);
            }

        }

        public double GetProint(int Axis, int SliceNumber, Point P1)
        {
            if (Axis == 0)
            {
                return GetPointX(SliceNumber, P1);
            }
            else if (Axis == 1)
            {
                return GetPointZ(SliceNumber, P1);
            }
            else
            {
                return GetPointY(SliceNumber, P1);
            }

        }

        public double[] GetProfile(Point3D P1, Point3D P2)
        {
            throw new Exception("Not yet Implimented");
        }

    }
}
