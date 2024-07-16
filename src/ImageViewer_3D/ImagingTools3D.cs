using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using ImageViewer3D.Filters;
using AviFile;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using ImageViewer;
using ImageViewer.Filters;

namespace ImageViewer3D
{
    public static class ImagingTools3D
    {
        public unsafe static void MaxMinArray(this double[, ,] array, out double Max, out double Min)
        {
            Max = double.MinValue;
            Min = double.MaxValue;

            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (Max < *pAOut) Max = *pAOut;
                    if (Min > *pAOut) Min = *pAOut;
                    pAOut++;
                }
            }
        }

        public unsafe static double MaxArray(this double[, ,] array)
        {
            double max = double.MinValue;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (max < *pAOut) max = *pAOut;
                    pAOut++;
                }
            }
            return max;
        }
        public unsafe static double MinArray(this double[, ,] array)
        {
            double min = double.MaxValue;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (min > *pAOut) min = *pAOut;
                    pAOut++;
                }
            }
            return min;
        }

        public static double[,] SliceZAxis(this double[, ,] ImageArray, int SliceIndex)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);

            double[,] OutArray = new double[iWidth, iHeight];
            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    OutArray[x, y] = ImageArray[x, y, SliceIndex];
                }
            }
            return OutArray;
        }

        public static Bitmap MakeBitmap(this double[,] ImageArray, double MinContrast, double MaxContrast)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = MaxContrast;
            double iMin = MinContrast;

            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                byte* bits;
                double g;
                byte g2;
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        bits = (byte*)scanline;
                        g = (255d * (ImageArray[x, y] - iMin) / iLength);
                        if (g < 0) g = 0;
                        if (g > 255) g = 255;
                        g2 = (byte)g;
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

        public static  Bitmap MakeBitmap(this double[,] ImageArray)
        {
            int iWidth = ImageArray.GetLength(1);
            int iHeight = ImageArray.GetLength(0);
            double iMax = -10000;
            double iMin = 10000;

            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            double  g;
            byte g2;
            unsafe
            {
                fixed (double* pDataBase = ImageArray)
                {

                    double * pData = pDataBase;
                    for (int i = 0; i < ImageArray.Length; i++)
                    {
                        if (iMax < *pData) iMax = *pData;
                        if (iMin > *pData) iMin = *pData;
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
                            g = (255d * (*pData - iMin) / iLength);
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

        public static double[, ,] OpenVolumnData(string Filename)
        {
            string Extension = Path.GetExtension(Filename).ToLower();
            if (Path.GetExtension(Filename).ToLower() == ".raw")
            {
                #region Open Raw
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);
                int sizeX, sizeY, sizeZ;

                Reader.ReadInt32();

                sizeX = Reader.ReadInt32();
                sizeY = Reader.ReadInt32();
                sizeZ = Reader.ReadInt32();

                //stuff used for bounds
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();

                double[, ,] mDensityGrid = new double[sizeX, sizeY, sizeZ];

                byte[] buffer = new byte[sizeX * sizeY * sizeZ * sizeof(double)];
                Reader.Read(buffer, 0, buffer.Length);

                Buffer.BlockCopy(buffer, 0, mDensityGrid, 0, Buffer.ByteLength(mDensityGrid));

                Reader.Close();
                BinaryFile.Close();
                return mDensityGrid;
                #endregion
            }
            else if (Path.GetExtension(Filename).ToLower() == ".bin")
            {
                #region Open Bin
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);

                long nPoints = (int)(BinaryFile.Length / 8d);
                nPoints = (long)Math.Pow((double)nPoints, (1d / 3d));

                int sizeX, sizeY, sizeZ;

                sizeX = (int)nPoints;
                sizeY = (int)nPoints;
                sizeZ = (int)nPoints;

                double[, ,] mDensityGrid = new double[sizeX, sizeY, sizeZ];

                for (int z = 0; z < sizeZ; z++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[x, y, z] = Reader.ReadDouble();
                        }
                    }
                }
                Reader.Close();
                BinaryFile.Close();
                return mDensityGrid;
                #endregion
            }
            else if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                #region Open 2D image
                int sizeX, sizeY;

                Bitmap b = new Bitmap(Filename);
                sizeX = b.Width;
                sizeY = b.Height;

                double[, ,] mDensityGrid = new double[sizeX, sizeY, 1];

                double[,] Data = ImagingTools.ConvertToDoubleArray(b, false);
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        mDensityGrid[x, y, 0] = Data[x, y];
                    }
                }
                return mDensityGrid;
                #endregion
            }
            return null;
        }

        public static double[, ,] OpenVolumnData(string[] Filenames)
        {
            if (Filenames.Length == 1)
                return OpenVolumnData(Filenames[0]);
            string Extension = Path.GetExtension(Filenames[0]).ToLower();
            if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                Filenames = EffectHelps.SortNumberedFiles(Filenames);

                int sizeX, sizeY, sizeZ;

                Bitmap b = new Bitmap(Filenames[0]);
                sizeX = b.Width;
                sizeY = b.Height;
                sizeZ = Filenames.Length;

                double[, ,] mDensityGrid = new double[sizeX, sizeY, sizeZ];

                for (int z = 0; z < sizeZ; z++)
                {
                    b = new Bitmap(Filenames[z]);
                    double[,] Data = ImagingTools.ConvertToDoubleArray(b, false);
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[x, y, z] = Data[x, y];
                        }
                    }
                }
                return mDensityGrid;
            }
            return null;
        }

        public static void SaveVolumnData(double[, ,] mDensityGrid, string Filename)
        {
            string Extension = Path.GetExtension(Filename).ToLower();
            if (Extension == "." || Extension == "")
            {
                Extension = ".raw";
            }
            if (Extension == ".raw")
            {
                #region SaveRawFile
                //why do I do this????
                string outFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename) + ".raw";
                FileStream BinaryFile = new FileStream(outFile, FileMode.Create, FileAccess.Write);
                BinaryWriter Writer = new BinaryWriter(BinaryFile);

                Writer.Write((Int32)3);

                Writer.Write((Int32)mDensityGrid.GetLength(2));
                Writer.Write((Int32)mDensityGrid.GetLength(1));
                Writer.Write((Int32)mDensityGrid.GetLength(0));

                Writer.Write((double)-1);
                Writer.Write((double)1);

                Writer.Write((double)-1);
                Writer.Write((double)1);

                Writer.Write((double)-1);
                Writer.Write((double)1);

                for (int z = 0; z < mDensityGrid.GetLength(0); z++)
                {
                    for (int y = 0; y < mDensityGrid.GetLength(1); y++)
                    {
                        for (int x = 0; x < mDensityGrid.GetLength(2); x++)
                        {
                            Writer.Write((double)mDensityGrid[x, y, z]);
                        }
                    }
                }
                Writer.Close();
                BinaryFile.Close();
                #endregion
            }
            else if (Extension == ".bin")
            {
                #region Save Bin
                FileStream BinaryFile = new FileStream(Filename, FileMode.Create, FileAccess.Write);
                BinaryWriter Writer = new BinaryWriter(BinaryFile);

                for (int z = 0; z < mDensityGrid.GetLength(0); z++)
                {
                    for (int y = 0; y < mDensityGrid.GetLength(1); y++)
                    {
                        for (int x = 0; x < mDensityGrid.GetLength(2); x++)
                        {
                            Writer.Write((double)mDensityGrid[x, y, z]);
                        }
                    }
                }
                Writer.Close();
                BinaryFile.Close();
                #endregion
            }
            else if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                #region Save as Images
                //todo make it so 1D arrays are outputed as bitmaps
                if (mDensityGrid.GetLength(0) == 1)
                {
                    throw new Exception("Not yet implimented");
                }
                else if (mDensityGrid.GetLength(1) == 1)
                {
                    throw new Exception("Not yet implimented");
                }
                else if (mDensityGrid.GetLength(2) == 1)
                {
                    throw new Exception("Not yet implimented");
                }
                else
                {
                    int ZSlices = mDensityGrid.GetLength(0);
                    string outFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename);

                    for (int z = 0; z < ZSlices; z++)
                    {
                        double[,] Slice = mDensityGrid.SliceZAxis(z);
                        Bitmap b = Slice.MakeBitmap();
                        b.Save(outFile + z.ToString() + Extension);
                    }
                }
                #endregion
            }
        }

        public static double[] ConvertGrayscaleSelectionToLinear(double[,] SourceImage, ISelection3D Selection)
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
    }
}
