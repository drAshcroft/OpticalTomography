using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace MathHelpLib
{
    #region Useful_Structs
    public struct PointD
    {
        public double X;
        public double Y;
        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return X.ToString() + "," + Y.ToString();
        }
    }


    #endregion

    public static class MathHelps
    {
       

        #region Make Bitmaps

        /// <summary>
        /// Makes a bitmap from the selected slice from the 3D data
        /// the bitmap is autocontrasted
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <param name="ZIndex"></param>
        /// <returns></returns>
        public static Bitmap MakeBitmap(this double[, ,] ImageArray, int ZIndex)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[i, j, ZIndex]) iMax = ImageArray[i, j, ZIndex];
                    if (iMin > ImageArray[i, j, ZIndex]) iMin = ImageArray[i, j, ZIndex];
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y, ZIndex] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }

        public static Bitmap MakeBitmapPerp(this double[, ,] ImageArray, int ZIndex)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(2);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[i, ZIndex, j]) iMax = ImageArray[i, ZIndex, j];
                    if (iMin > ImageArray[i,ZIndex, j]) iMin = ImageArray[i, ZIndex, j];
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, ZIndex, y] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }

        public static Bitmap MakeBitmapPerpX(this double[, ,] ImageArray, int ZIndex)
        {
            int iWidth = ImageArray.GetLength(1);
            int iHeight = ImageArray.GetLength(2);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[ ZIndex,i, j]) iMax = ImageArray[ ZIndex,i, j];
                    if (iMin > ImageArray[ ZIndex,i, j]) iMin = ImageArray[ ZIndex,i, j];
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[ ZIndex,x, y] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }

        

        /// <summary>
        /// Makes a bitmap from the selected slice from the 3D data
        /// the bitmap is autocontrasted
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <param name="ViewAxis"></param>
        /// <param name="SliceIndex"></param>
        /// <returns></returns>
        public static Bitmap MakeBitmap(this double[, ,] ImageArray, Axis ViewAxis, int SliceIndex)
        {
            if (ViewAxis == Axis.XAxis)
                return MakeBitmapXAxis(ImageArray, SliceIndex);
            else if (ViewAxis == Axis.YAxis)
                return MakeBitmapYAxis(ImageArray, SliceIndex);
            else
                return MakeBitmapZAxis(ImageArray, SliceIndex);
        }
        private static Bitmap MakeBitmapXAxis(this double[, ,] ImageArray, int SliceIndex)
        {
            int iWidth = ImageArray.GetLength(1);
            int iHeight = ImageArray.GetLength(2);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    double Value = ImageArray[SliceIndex, i, j];
                    if (iMax < Value) iMax = Value;
                    if (iMin > Value) iMin = Value;
                }

            return MakeBitmapXAxis(ImageArray, SliceIndex, iMin, iMax);
        }
        private static Bitmap MakeBitmapYAxis(this double[, ,] ImageArray, int SliceIndex)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(2);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    double Value = ImageArray[i, SliceIndex, j];
                    if (iMax < Value) iMax = Value;
                    if (iMin > Value) iMin = Value;
                }

            return MakeBitmapYAxis(ImageArray, SliceIndex, iMin, iMax);
        }
        private static Bitmap MakeBitmapZAxis(this double[, ,] ImageArray, int SliceIndex)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    double Value = ImageArray[i, j, SliceIndex];
                    if (iMax < Value) iMax = Value;
                    if (iMin > Value) iMin = Value;
                }

            return MakeBitmapZAxis(ImageArray, SliceIndex, iMin, iMax);
        }

        /// <summary>
        /// Makes a bitmap from the selected slice from the 3D data 
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <param name="ViewAxis"></param>
        /// <param name="SliceIndex"></param>
        /// <param name="MinContrast">min contrast in pixel value</param>
        /// <param name="MaxContrast">max contrast in pixel value</param>
        /// <returns></returns>
        public static Bitmap MakeBitmap(this double[, ,] ImageArray, Axis ViewAxis, int SliceIndex, double MinContrast, double MaxContrast)
        {
            if (ViewAxis == Axis.XAxis)
                return MakeBitmapXAxis(ImageArray, SliceIndex, MinContrast, MaxContrast);
            else if (ViewAxis == Axis.YAxis)
                return MakeBitmapYAxis(ImageArray, SliceIndex, MinContrast, MaxContrast);
            else
                return MakeBitmapZAxis(ImageArray, SliceIndex, MinContrast, MaxContrast);
        }
        private static Bitmap MakeBitmapXAxis(this double[, ,] ImageArray, int SliceIndex, double MinContrast, double MaxContrast)
        {
            int iWidth = ImageArray.GetLength(2);
            int iHeight = ImageArray.GetLength(1);
            double iMin = MinContrast;
            double iMax = MaxContrast;
            double iLength = iMax - iMin;

            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

            unsafe
            {

                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[SliceIndex, y, x] - iMin) / iLength);
                        if (g > 255) g = 255;
                        if (g < 0) g = 0;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }
        private static Bitmap MakeBitmapYAxis(this double[, ,] ImageArray, int SliceIndex, double MinContrast, double MaxContrast)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(2);
            double iMin = MinContrast;
            double iMax = MaxContrast;

            double iLength = iMax - iMin;

            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

            unsafe
            {

                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, SliceIndex, y] - iMin) / iLength);
                        if (g > 255) g = 255;
                        if (g < 0) g = 0;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }
        private static Bitmap MakeBitmapZAxis(this double[, ,] ImageArray, int SliceIndex, double MinContrast, double MaxContrast)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMin = MinContrast;
            double iMax = MaxContrast;

            double iLength = iMax - iMin;

            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

            unsafe
            {

                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y, SliceIndex] - iMin) / iLength);

                        if (g > 255) g = 255;
                        if (g < 0) g = 0;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }


        /// <summary>
        /// Converts offsetX 2D array to offsetX intensity bitmap
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <returns></returns>
        public static Bitmap MakeBitmap(this complex[,] ImageArray)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[i, j].Abs()) iMax = ImageArray[i, j].Abs();
                    if (iMin > ImageArray[i, j].Abs()) iMin = ImageArray[i, j].Abs();
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y].Abs() - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }

        public static Bitmap MakeBitmap(this complex[, ,] ImageArray, int ZIndex)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[i, j, ZIndex].Abs()) iMax = ImageArray[i, j, ZIndex].Abs();
                    if (iMin > ImageArray[i, j, ZIndex].Abs()) iMin = ImageArray[i, j, ZIndex].Abs();
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y, ZIndex].Abs() - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }
        /// <summary>
        /// Converts offsetX 2D array to offsetX intensity bitmap
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <returns></returns>
        public static Bitmap MakeBitmapReal(this complex[,] ImageArray)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[i, j].real) iMax = ImageArray[i, j].real;
                    if (iMin > ImageArray[i, j].real) iMin = ImageArray[i, j].real;
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y].real - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }

        /// <summary>
        /// Converts offsetX 2D array to offsetX intensity bitmap
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <returns></returns>
        public static Bitmap MakeBitmapImag(this complex[,] ImageArray)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[i, j].imag) iMax = ImageArray[i, j].imag;
                    if (iMin > ImageArray[i, j].imag) iMin = ImageArray[i, j].imag;
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y].imag - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }
        #endregion

        /// <summary>
        /// performs a operation on every elements in the array, no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        public static void DivideInPlaceErrorless(this double[,] array, double[,] Divisor)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    try
                    {
                        array[i, j] /= Divisor[i, j];
                    }
                    catch
                    {
                        array[i, j] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// returns the sum of all the elements in an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double SumArrayCareful(this double[,] array)
        {
            double sum = 0;
            unsafe
            {
                fixed (double* pArray = array)
                {
                    double* ppArray = pArray;
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (double.IsNaN(*ppArray) == false && double.IsInfinity(*ppArray) == false)
                        {
                            sum += *ppArray;
                        }
                        ppArray++;

                    }
                }
            }
            return sum;
        }

        public static double[, ,] LinearScale(double[, ,] Data, double V11, double V12, double V21, double V22)
        {
            double m = (V21 - V22) / (V11 - V12);
            double b = V21 - (m * V11);

            for (int i = 0; i < Data.GetLength(0); i++)
                for (int j = 0; j < Data.GetLength(1); j++)
                    for (int k = 0; k < Data.GetLength(2); k++)
                        Data[i, j, k] = m * Data[i, j, k] + b;
            return Data;
        }

        /// <summary>
        /// Returns the angle in radians for the two offset values
        /// </summary>
        /// <param name="dx">x distance from the center</param>
        /// <param name="dy">y distance from the center</param>
        /// <returns></returns>
        public static double GetAngle(double dx, double dy)
        {
            //atan returns angles from only 2 quadrants.  this makes sure to return the 
            //values from the whole circle
            double Angle = 0;
            if (dx == 0)
            {
                if (dy > 0) Angle = Math.PI / 2d;
                else Angle = Math.PI * 3d / 2d;
            }
            else
            {
                Angle = Math.Atan(Math.Abs(dy / dx));
                if (dx < 0 && dy > 0)
                    Angle = Math.PI - Angle;
                else if (dx < 0 && dy < 0)
                    Angle += Math.PI;
                else if (dx > 0 && dy < 0)
                    Angle = 2 * Math.PI - Angle;
            }
            return Angle;
        }
        /// <summary>
        /// Returns the angle in degrees for the two offset values
        /// </summary>
        /// <param name="dx">x distance from the center</param>
        /// <param name="dy">y distance from the center</param>
        /// <returns></returns>
        public static double GetAngleDegrees(double dx, double dy)
        {
            //atan returns angles from only 2 quadrants.  this makes sure to return the 
            //values from the whole circle
            double Angle = 0;
            if (dx == 0)
            {
                if (dy > 0) Angle = 90;
                else Angle = 270;
            }
            else if (dy == 0)
            {
                if (dx > 0)
                    Angle = 0;
                else
                    Angle = 180;
            }
            else
            {
                Angle = Math.Atan(Math.Abs(dy / dx)) / Math.PI * 180d;
                if (dx < 0 && dy > 0)
                    Angle = 180 - Angle;
                else if (dx < 0 && dy < 0)
                    Angle += 180;
                else if (dx > 0 && dy < 0)
                    Angle = 360 - Angle;
            }
            return Angle;
        }


        /// <summary>
        /// Rotates a point around the origin
        /// </summary>
        /// <param name="Angle">rotation in radians</param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static PointD RotatePoint(double Angle, PointD point)
        {
            return RotatePoint(Angle, point.X, point.Y);
        }

        /// <summary>
        /// Rotates a point around the origin
        /// </summary>
        /// <param name="Angle">rotation in radians</param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static PointD RotatePoint(double Angle, double X, double Y)
        {
            double c = Math.Cos(Angle);
            double s = Math.Sin(Angle);
            return new PointD(c * X - s * Y, s * X + c * Y);
        }



        /// <summary>
        /// Normalizes offsetX point as if it is offsetX vector from (0,0)
        /// </summary>
        /// <param name="inPoint"></param>
        /// <returns></returns>
        public static PointD NormalizePoint(PointD inPoint)
        {
            double r = Math.Sqrt(inPoint.X * inPoint.X + inPoint.Y * inPoint.Y);
            return new PointD(inPoint.X / r, inPoint.Y / r);
        }

        /// <summary>
        /// returns true is point (x,y) is within the ellipse defined by center (centerX,centerY) with specified axis and rotation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="MajorAxis"></param>
        /// <param name="MinorAxis"></param>
        /// <param name="Rotation">Rotation is in radians</param>
        /// <returns></returns>
        public static bool IsInsideEllipse(double x, double y, double CenterX, double CenterY, double MajorAxis, double MinorAxis, double Rotation)
        {
            PointD newPoint = new PointD(x - CenterX, y - CenterY);
            newPoint = RotatePoint(-1 * Rotation, newPoint);
            double xp = newPoint.X / MajorAxis;
            double yp = newPoint.Y / MinorAxis;
            if (xp * xp + yp * yp <= 1)
                return true;
            else
                return false;


        }



        /// <summary>
        /// returns true is point (x,y) is within the ellipse defined by center (centerX,centerY) with specified axis and rotation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="MajorAxis"></param>
        /// <param name="MinorAxis"></param>
        /// <param name="Rotation">Rotation is in radians</param>
        /// <returns></returns>
        public static bool IsInsideRectangle(double x, double y, double CenterX, double CenterY, double MajorAxis, double MinorAxis, double Rotation)
        {
            PointD newPoint = new PointD(x - CenterX, y - CenterY);
            newPoint = RotatePoint(-1 * Rotation, newPoint);
            double xp = Math.Abs(newPoint.X / MajorAxis);
            double yp = Math.Abs(newPoint.Y / MinorAxis);

            if (xp <= 1 && yp <= 1)
                return true;
            else
                return false;


        }


      
        /*
        /// <summary>
        /// returns an array with length equal to the shorter array (results are centered)
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Impulse"></param>
        /// <returns></returns>
        public static double[] ConvoluteChopSlow(double[] Array1, double[] Array2)
        {
            double[] ArrayOut = Convolute(Array1, Array2);

            int Length;

            if (Array1.Length < Array2.Length)
            {
                Length = Array1.Length;
            }
            else
            {
                Length = Array2.Length;
            }
            double[] ArrayOut2 = new double[Length];
            int cc = 0;
            int Length2 = ArrayOut.Length / 2 + Length / 2;
            for (int i = (int)(ArrayOut.Length / 2 - Length / 2); i < Length2; i++)
            {
                ArrayOut2[cc] = ArrayOut[i];
                cc++;
            }

            return ArrayOut2;
        }*/

        /// <summary>
        /// Makes the array graphable by setting up an X axis for the array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="MinX"></param>
        /// <param name="StepX"></param>
        /// <returns></returns>
        public static double[,] MakeGraphableArray(this double[] array, double MinX, double StepX)
        {
            double[,] outArray = new double[2, array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                outArray[0, i] = MinX + StepX * i;
                outArray[1, i] = array[i];
            }
            return outArray;
        }

        /// <summary>
        /// Makes the array graphable by setting up an X axis for the array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="MinX"></param>
        /// <param name="StepX"></param>
        /// <returns></returns>
        public static double[,] MakeGraphableArray(this int[] array, double MinX, double StepX)
        {
            double[,] outArray = new double[2, array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                outArray[0, i] = MinX + StepX * i;
                outArray[1, i] = array[i];
            }
            return outArray;
        }



        /// <summary>
        /// Returns the nearest power of 2 that is equal or greater to the current value
        /// </summary>
        /// <param name="testNumber"></param>
        /// <returns></returns>
        public static double NearestPowerOf2(int testNumber)
        {
            double denom = Math.Log(testNumber) / Math.Log(2);
            if (denom - Math.Floor(denom) == 0)
                return testNumber;
            else
                return Math.Pow(2, Math.Floor(denom) + 1);

        }

        /*  /// <summary>
          /// Performs a linear regression of the 1D data.  
          /// </summary>
          /// <param name="ScatterData"></param>
          /// <param name="PolyOrder">polynomial order of the fitting function</param>
          /// <param name="coeff"></param>
          public static void LinearRegressionPoly(double[,] ScatterData, int PolyOrder, out double[] coeff)
          {
              //set up the linear alegbra matricies
              double[,] X= new double[ScatterData.GetLength(1),PolyOrder+1];
              double[,] Y = new double[1,ScatterData.GetLength(1)];
              for (int i=0;i<ScatterData.GetLength(1);i++)
              {
                  double xV =1;
                  for (int j=0;j<=PolyOrder;j++)
                  {
                      X[i,j]=xV;
                      xV*=ScatterData[0,i];
                  }
                  Y[0,i]=ScatterData[1,i];
              }

              //create the mathnet matrix
              MathNet.Numerics.LinearAlgebra.Matrix mX = MathNet.Numerics.LinearAlgebra.Matrix.Create(X);// new MathNet.Numerics.LinearAlgebra.Matrix(X);
              MathNet.Numerics.LinearAlgebra.Matrix mY = MathNet.Numerics.LinearAlgebra.Matrix.Create(Y);
              //solve the y=a*x equation
              mY.Transpose();
              MathNet.Numerics.LinearAlgebra.Matrix a=  mX.Solve(mY);

              //copy out the regression
              coeff = new double[PolyOrder + 1];
              for (int i = 0; i <= PolyOrder; i++)
              {
                  coeff[i] = a[ i,0];
              }

          }

          /// <summary>
          /// Does a linear regression with a line
          /// </summary>
          /// <param name="Scatterdata"></param>
          /// <param name="slope">resultant slope</param>
          /// <param name="intercept">resultant intercept</param>
          public static void LinearRegression(double[,] Scatterdata, out double slope, out double intercept)
          {
              double xAvg = 0;
              double yAvg = 0;
              double Length = Scatterdata.GetLength(1);
              for (int i = 0; i < Length ; i++)
              {
                  xAvg += Scatterdata[0,i];
                  yAvg += Scatterdata[1,i];
              }

              xAvg = xAvg / Length;
              yAvg = yAvg / Length;

              double v1 = 0;
              double v2 = 0;

              for (int i = 0; i < Length; i++)
              {
                  v1 += (Scatterdata[0,i] - xAvg) * (Scatterdata[1,i] - yAvg);
                  v2 += Math.Pow(Scatterdata[0,i] - xAvg, 2);
              }

              slope  = v1 / v2;
              intercept  = yAvg - slope  * xAvg;
          }*/



        /// <summary>
        /// Performs a seperable convolution on an array, i.e. first in one direction and then on the other axis
        /// </summary>
        /// <param name="Array1"></param>
        /// <param name="Impulse"></param>
        /// <returns></returns>
        public static double[,] ConvoluteChopSeperable(this double[,] Array1, double[] Impulse)
        {
            return Filtering.ConvoluteChopSeperable(Array1, Impulse);
        }
    }

}