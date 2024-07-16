using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
//using BitMiracle.LibTiff.Classic;
using MathHelpLib.ImageProcessing;
using Emgu.CV.Structure;
using BitMiracle.LibTiff.Classic;

namespace MathHelpLib
{
    public static class MathHelpsFileLoader
    {

        #region Save Bitmaps
        public static void Save_Bitmap(string Filename, object Image)
        {
            if (Path.GetExtension(Filename).ToLower() == ".cct")
            {
                if (Image.GetType() == typeof(Bitmap))
                {
                    MathHelpLib.MathHelpsFileLoader.Save_Raw(Filename, (Bitmap)Image);
                }
                else if (Image.GetType() == typeof(ImageHolder))
                {
                    MathHelpLib.MathHelpsFileLoader.Save_Raw(Filename, (ImageHolder)Image);
                }

            }
            else
            {
                if (Path.GetExtension(Filename).ToLower().Contains("tif"))
                {
                    Save_TIFF(Filename,(ImageHolder) Image);
                }
                else
                {
                    if (Image.GetType() == typeof(Bitmap))
                    {
                        if (File.Exists(Filename) == true)
                            File.Delete(Filename);
                        ((Bitmap)Image).Save(Filename);
                    }
                    else if (Image.GetType() == typeof(ImageHolder))
                    {
                        ((ImageHolder)Image).ToBitmap().Save(Filename);
                    }

                    else
                    {
                        throw new Exception("Do not know how to save this image type");
                    }
                }
            }
        }

        #region Raw Data
        public static void Save_Raw(string Filename, double[,] imageData)
        {
            if (Path.GetExtension(Filename).ToLower().Contains("cct") == true)
                System.Diagnostics.Debug.Print("");
            Save_TIFF(Filename, imageData);
        }

        public static void Save_Raw(string Filename, float[,] imageData)
        {
            if (Path.GetExtension(Filename).ToLower().Contains("cct") == true)
                System.Diagnostics.Debug.Print("");
            Save_TIFF(Filename, imageData);
        }

        public static void Save_Raw(string Filename, Bitmap imageData)
        {
            if (Path.GetExtension(Filename).ToLower().Contains("cct") == true)
                System.Diagnostics.Debug.Print("");
            Save_TIFF(Filename, imageData);
        }

        public static void Save_Raw(string Filename, ImageHolder imageData)
        {
            if (Path.GetExtension(Filename).ToLower().Contains("cct") == true)
                System.Diagnostics.Debug.Print("");
            Save_TIFF(Filename, imageData);
        }



        public static void Save_Raw(string Filename, float[, ,] data)
        {

            string extention = Path.GetExtension(Filename).ToLower();
            if (extention == ".cct")
            {
                System.Diagnostics.Debug.Print("");
            }
            else if (extention == ".bmp" || extention == ".gif" || extention == ".jpeg" || extention == ".png" || extention == ".tiff" || extention == ".tif" || extention == ".jpg")
            {
                if (extention == ".png")
                {
                    #region savepng
                    float[, ,] Slice = data;

                    double Min = double.MaxValue;
                    double Max = double.MinValue;
                    for (int z = 0; z < Slice.GetLength(0); z++)
                    {
                        for (int y = 0; y < Slice.GetLength(1); y++)
                        {
                            for (int x = 0; x < Slice.GetLength(2); x++)
                            {
                                if (Slice[z, y, x] > Max) Max = Slice[z, y, x];
                                if (Slice[z, y, x] < Min) Min = Slice[z, y, x];
                            }
                        }
                    }

                    double Scale = ushort.MaxValue / (Max - Min);

                    string ppath = Path.GetDirectoryName(Filename);
                    string pFilename = Path.GetFileNameWithoutExtension(Filename);
                    for (int z = 0; z < Slice.GetLength(0); z++)
                    {
                        Emgu.CV.Image<Gray, UInt16> image = new Emgu.CV.Image<Gray, ushort>(Slice.GetLength(1), Slice.GetLength(2));

                        for (int y = 0; y < Slice.GetLength(1); y++)
                        {
                            for (int x = 0; x < Slice.GetLength(0); x++)
                            {
                                image.Data[x, y, 0] = (UInt16)(Scale * (Slice[z, y, x] - Min));
                            }
                        }
                        image.Save(ppath + "\\" + pFilename + string.Format("{0:000}", z) + extention);
                    }
                    #endregion
                }
                else if (extention == ".tiff" || extention == ".tif")
                {
                    Save_Tiff_Stack(Filename, data);
                }
                else
                {
                    #region AllOthers

                    float[, ,] Slice = data;

                    double Min = double.MaxValue;
                    double Max = double.MinValue;
                    for (int z = 0; z < Slice.GetLength(0); z++)
                    {
                        for (int y = 0; y < Slice.GetLength(1); y++)
                        {
                            for (int x = 0; x < Slice.GetLength(2); x++)
                            {
                                if (Slice[z, y, x] > Max) Max = Slice[z, y, x];
                                if (Slice[z, y, x] < Min) Min = Slice[z, y, x];
                            }
                        }
                    }

                    double Scale = byte.MaxValue / (Max - Min);

                    string ppath = Path.GetDirectoryName(Filename);
                    string pFilename = Path.GetFileNameWithoutExtension(Filename);
                    for (int z = 0; z < Slice.GetLength(0); z++)
                    {
                        Emgu.CV.Image<Gray, byte> image = new Emgu.CV.Image<Gray, byte>(Slice.GetLength(2), Slice.GetLength(1));

                        for (int y = 0; y < Slice.GetLength(1); y++)
                        {
                            for (int x = 0; x < Slice.GetLength(2); x++)
                            {
                                image.Data[y, x, 0] = (byte)(Scale * (Slice[z, y, x] - Min));
                            }
                        }
                        image.Save(ppath + "\\" + pFilename + string.Format("{0:000}", z) + extention);
                    }
                    #endregion
                }
            }
        }

        public static void Save_Raw(string Filename, double[, ,] data)
        {
            string extention = Path.GetExtension(Filename).ToLower();
            if (extention == ".cct")
            {
                System.Diagnostics.Debug.Print("");
            }
            else if (extention == ".bmp" || extention == ".gif" ||
                extention == ".jpeg" || extention == ".png" || extention == ".tiff" || extention == ".tif" ||
                extention == ".jpg")
            {
                if (extention == ".png")
                {
                    #region savepng
                    double[, ,] Slice = data;

                    double Min = double.MaxValue;
                    double Max = double.MinValue;
                    for (int z = 0; z < Slice.GetLength(0); z++)
                    {
                        for (int y = 0; y < Slice.GetLength(1); y++)
                        {
                            for (int x = 0; x < Slice.GetLength(2); x++)
                            {
                                if (Slice[z, y, x] > Max) Max = Slice[z, y, x];
                                if (Slice[z, y, x] < Min) Min = Slice[z, y, x];
                            }
                        }
                    }

                    double Scale = ushort.MaxValue / (Max - Min);

                    string ppath = Path.GetDirectoryName(Filename);
                    string pFilename = Path.GetFileNameWithoutExtension(Filename);
                    for (int z = 0; z < Slice.GetLength(0); z++)
                    {
                        Emgu.CV.Image<Gray, UInt16> image = new Emgu.CV.Image<Gray, ushort>(Slice.GetLength(0), Slice.GetLength(1));

                        for (int y = 0; y < Slice.GetLength(1); y++)
                        {
                            for (int x = 0; x < Slice.GetLength(0); x++)
                            {
                                image.Data[x, y, 0] = (UInt16)(Scale * (Slice[z, y, x] - Min));
                            }
                        }
                        image.Save(ppath + "\\" + pFilename + z.ToString() + extention);
                    }
                    #endregion
                }
                else if (extention == ".tiff" || extention == ".tif")
                {
                    Save_Tiff_Stack(Filename, data);
                }
                else
                {
                    #region AllOthers

                    double[, ,] Slice = data;

                    double Min = double.MaxValue;
                    double Max = double.MinValue;
                    for (int z = 0; z < Slice.GetLength(0); z++)
                    {
                        for (int y = 0; y < Slice.GetLength(1); y++)
                        {
                            for (int x = 0; x < Slice.GetLength(2); x++)
                            {
                                if (Slice[z, y, x] > Max) Max = Slice[z, y, x];
                                if (Slice[z, y, x] < Min) Min = Slice[z, y, x];
                            }
                        }
                    }

                    double Scale = byte.MaxValue / (Max - Min);

                    string ppath = Path.GetDirectoryName(Filename);
                    string pFilename = Path.GetFileNameWithoutExtension(Filename);
                    for (int z = 0; z < Slice.GetLength(0); z++)
                    {
                        Emgu.CV.Image<Gray, byte> image = new Emgu.CV.Image<Gray, byte>(Slice.GetLength(0), Slice.GetLength(1));

                        for (int y = 0; y < Slice.GetLength(1); y++)
                        {
                            for (int x = 0; x < Slice.GetLength(0); x++)
                            {
                                image.Data[x, y, 0] = (byte)(Scale * (Slice[z, y, x] - Min));
                            }
                        }
                        image.Save(ppath + "\\" + pFilename + z.ToString() + extention);
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// saves the recon volume as either a monolithic file or 
        /// as a stack of images depending on the selected file type
        /// </summary>
        /// <param name="Filename"></param>
        public static void Save_Raw(string Filename, double[][,] Data)
        {
            //todo: make it possible to save stack as a tiff
            string Extension = Path.GetExtension(Filename).ToLower();
            if (Extension == "." || Extension == "")
            {
                Extension = ".raw";
            }
            if (Extension == ".raw")
            {
                #region SaveRawFile

                string HeaderFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename) + ".dat";

                using (StreamWriter outfile = new StreamWriter(HeaderFile))
                {
                    /*ObjectFileName:	walnut.raw
                    Resolution:	128 96 114
                    SliceThickness:	1 1 1
                    Format:		USHORT
                    ObjectModel:	I
                     */

                    outfile.WriteLine("ObjectFileName:" + Filename);
                    outfile.WriteLine("Resolution: " + Data[0].GetLength(1) + " " + Data[0].GetLength(0) + " " + Data.GetLength(0));
                    outfile.WriteLine("SliceThickness:	1 1 1");
                    outfile.WriteLine("Format:		USHORT");
                    outfile.WriteLine(" ObjectModel:	I");
                }

                FileStream BinaryFile = new FileStream(Filename, FileMode.Create, FileAccess.Write);
                BinaryWriter Writer = new BinaryWriter(BinaryFile);

                for (int z = 0; z < Data.GetLength(0); z++)
                {
                    for (int y = 0; y < Data[0].GetLength(0); y++)
                    {
                        for (int x = 0; x < Data[0].GetLength(1); x++)
                        {
                            Writer.Write((double)Data[z][y, x]);
                        }
                    }
                }
                Writer.Close();
                BinaryFile.Close();
                #endregion
            }
            else if (Extension == ".cct")
            {
                #region SaveCCTFile
                FileStream BinaryFile = new FileStream(Filename, FileMode.Create, FileAccess.Write);

                #endregion
            }
            else if (Extension == ".bmp" || Extension == ".jpg")
            {
                #region Save as Images

                int ZSlices = Data.GetLength(0);
                string outFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename);

                double Max = double.MinValue;
                double Min = double.MaxValue;
                double max = double.MinValue;
                double min = double.MaxValue;

                for (int z = 0; z < ZSlices; z++)
                {
                    double[,] Slice = (double[,])Data[z];
                    max = Slice.MaxArray();
                    min = Slice.MinArray();
                    if (max > Max) Max = max;
                    if (min < Min) Min = min;
                }

                for (int z = 0; z < ZSlices; z++)
                {
                    double[,] Slice = (double[,])Data[z];
                    Bitmap b = Slice.MakeBitmap(Min, Max);
                    b.Save(outFile + string.Format("_{0:000}", z) + Extension);
                }

                #endregion
            }
            else if (Extension == ".png" || Extension == ".tif")
            {
                #region Save as Images

                int ZSlices = Data.GetLength(0);
                string outFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename);

                double Max = double.MinValue;
                double Min = double.MaxValue;
                double max = double.MinValue;
                double min = double.MaxValue;

                for (int z = 0; z < ZSlices; z++)
                {
                    double[,] Slice = (double[,])Data[z];
                    max = Slice.MaxArray();
                    min = Slice.MinArray();
                    if (max > Max) Max = max;
                    if (min < Min) Min = min;
                }

                double A = UInt16.MaxValue / (Max - Min);

                for (int z = 0; z < ZSlices; z++)
                {
                    double[,] Slice = (double[,])Data[z];
                    Save_Raw(outFile + string.Format("_{0:000}", z) + Extension, Slice);
                    /*                    FreeImageAPI.FreeImageBitmap fib = new FreeImageAPI.FreeImageBitmap(Slice.GetLength(0), Slice.GetLength(1), FreeImageAPI.FREE_IMAGE_TYPE.FIT_UINT16);
                                        unsafe
                                        {
                                            for (int y = 0; y < Slice.GetLength(1); y++)
                                            {
                                                UInt16* pOut = (UInt16*)((byte*)fib.Scan0 + fib.Pitch * y);

                                                for (int x = 0; x < Slice.GetLength(0); x++)
                                                {
                                                    UInt16 Gray = (UInt16)(A * (Slice[x, y] - Min));
                                                }
                                            }
                                        }
                                        fib.Save();*/
                }

                #endregion
            }
            else if (Extension == ".tif" || Extension == ".tiff")
            {
                Save_Tiff_Stack(Filename, Data);
            }
        }
        /// <summary>
        /// saves the recon volume as either a monolithic file or 
        /// as a stack of images depending on the selected file type
        /// </summary>
        /// <param name="Filename"></param>
        public static void Save_Raw(string Filename, float[][,] Data)
        {
            //todo: make it possible to save stack as a tiff
            string Extension = Path.GetExtension(Filename).ToLower();
            if (Extension == "." || Extension == "")
            {
                Extension = ".raw";
            }
            if (Extension == ".raw")
            {
                #region SaveRawFile

                string HeaderFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename) + ".dat";

                using (StreamWriter outfile = new StreamWriter(HeaderFile))
                {
                    /*ObjectFileName:	walnut.raw
                    Resolution:	128 96 114
                    SliceThickness:	1 1 1
                    Format:		USHORT
                    ObjectModel:	I
                     */

                    outfile.WriteLine("ObjectFileName:" + Filename);
                    outfile.WriteLine("Resolution: " + Data[0].GetLength(1) + " " + Data[0].GetLength(0) + " " + Data.GetLength(0));
                    outfile.WriteLine("SliceThickness:	1 1 1");
                    outfile.WriteLine("Format:		USHORT");
                    outfile.WriteLine(" ObjectModel:	I");
                }

                FileStream BinaryFile = new FileStream(Filename, FileMode.Create, FileAccess.Write);
                BinaryWriter Writer = new BinaryWriter(BinaryFile);

                for (int z = 0; z < Data.GetLength(0); z++)
                {
                    for (int y = 0; y < Data[0].GetLength(0); y++)
                    {
                        for (int x = 0; x < Data[0].GetLength(1); x++)
                        {
                            Writer.Write((double)Data[z][y, x]);
                        }
                    }
                }
                Writer.Close();
                BinaryFile.Close();
                #endregion
            }
            else if (Extension == ".cct")
            {
                #region SaveCCTFile
                FileStream BinaryFile = new FileStream(Filename, FileMode.Create, FileAccess.Write);

                #endregion
            }
            else if (Extension == ".bmp" || Extension == ".jpg")
            {
                #region Save as Images

                int ZSlices = Data.GetLength(0);
                string outFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename);

                float Max = float.MinValue;
                float Min = float.MaxValue;
                float max = float.MinValue;
                float min = float.MaxValue;

                for (int z = 0; z < ZSlices; z++)
                {
                    float[,] Slice = (float[,])Data[z];
                    max = Slice.MaxArray();
                    min = Slice.MinArray();
                    if (max > Max) Max = max;
                    if (min < Min) Min = min;
                }

                for (int z = 0; z < ZSlices; z++)
                {
                    float[,] Slice = (float[,])Data[z];
                    Bitmap b = Slice.MakeBitmap(Min, Max);
                    b.Save(outFile + string.Format("_{0:000}", z) + Extension);
                }

                #endregion
            }
            else if (Extension == ".png")
            {
                #region Save as Images

                int ZSlices = Data.GetLength(0);
                string outFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename);

                float Max = float.MinValue;
                float Min = float.MaxValue;
                float max = float.MinValue;
                float min = float.MaxValue;

                for (int z = 0; z < ZSlices; z++)
                {
                    float[,] Slice = (float[,])Data[z];
                    max = Slice.MaxArray();
                    min = Slice.MinArray();
                    if (max > Max) Max = max;
                    if (min < Min) Min = min;
                }

                double A = UInt16.MaxValue / (Max - Min);

                for (int z = 0; z < ZSlices; z++)
                {
                    float[,] Slice = (float[,])Data[z];
                    Save_Raw(outFile + string.Format("_{0:000}", z) + Extension, Slice);
                    /*                    FreeImageAPI.FreeImageBitmap fib = new FreeImageAPI.FreeImageBitmap(Slice.GetLength(0), Slice.GetLength(1), FreeImageAPI.FREE_IMAGE_TYPE.FIT_UINT16);
                                        unsafe
                                        {
                                            for (int y = 0; y < Slice.GetLength(1); y++)
                                            {
                                                UInt16* pOut = (UInt16*)((byte*)fib.Scan0 + fib.Pitch * y);

                                                for (int x = 0; x < Slice.GetLength(0); x++)
                                                {
                                                    UInt16 Gray = (UInt16)(A * (Slice[x, y] - Min));
                                                }
                                            }
                                        }
                                        fib.Save();*/
                }

                #endregion
            }
            else if (Extension == ".tif" || Extension == ".tiff")
            {
                Save_Tiff_Stack(Filename, Data);
            }
        }

        public static void Save_Raw(string Filename, double[][,] Data, int BitDepth)
        {

            string Extension = Path.GetExtension(Filename).ToLower();
            if (Extension == ".png" || Extension == ".tif" && BitDepth == 16)
            {
                #region Save as Images

                int ZSlices = Data.GetLength(0);
                string outFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename);

                double Max = double.MinValue;
                double Min = double.MaxValue;
                double max = double.MinValue;
                double min = double.MaxValue;

                for (int z = 0; z < ZSlices; z++)
                {
                    double[,] Slice = (double[,])Data[z];
                    max = Slice.MaxArray();
                    min = Slice.MinArray();
                    if (max > Max) Max = max;
                    if (min < Min) Min = min;
                }

                double A = (UInt16.MaxValue - 1) / (Max - Min);

                for (int z = 0; z < ZSlices; z++)
                {
                    double[,] Slice = (double[,])Data[z];
                    Save_Raw(outFile + string.Format("_{0:000}", z) + Extension, Slice);
                    /* FreeImageAPI.FreeImageBitmap fib = new FreeImageAPI.FreeImageBitmap(Slice.GetLength(0), Slice.GetLength(1), FreeImageAPI.FREE_IMAGE_TYPE.FIT_UINT16);
                     unsafe
                     {
                         for (int y = 0; y < Slice.GetLength(1); y++)
                         {
                             UInt16* pOut = (UInt16*)((byte*)fib.Scan0 + fib.Stride * y);

                             for (int x = 0; x < Slice.GetLength(0); x++)
                             {
                                 UInt16 Gray = (UInt16)Math.Truncate(A * (Slice[x, y] - Min));
                                 *pOut = Gray;
                                 pOut++;
                             }
                         }
                     }
                     fib.Save(outFile + string.Format("_{0:000}", z) + Extension);*/
                }

                #endregion
            }
            else
            {
                Save_Raw(Filename, Data);
            }
        }
        #endregion

        #region Tiffs

        public static void Save_Tiff_Stack(string Filename, double[, ,] Data)
        {
            int width = Data.GetLength(0);
            int height = Data.GetLength(1);

            using (Tiff output = Tiff.Open(Filename, "w"))
            {
                for (int z = 0; z < Data.GetLength(2); z++)
                {
                    // Emgu.CV.Image<Gray, float> image = new Emgu.CV.Image<Gray, float>(Data.GetLength(1), Data.GetLength(2));


                    output.SetField(TiffTag.IMAGEWIDTH, width);
                    output.SetField(TiffTag.IMAGELENGTH, height);
                    output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                    output.SetField(TiffTag.SAMPLEFORMAT, BitMiracle.LibTiff.Classic.SampleFormat.IEEEFP);
                    output.SetField(TiffTag.BITSPERSAMPLE, 32);
                    output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);

                    output.SetField(TiffTag.ROWSPERSTRIP, output.DefaultStripSize(0));
                    //output.SetField(TiffTag.ROWSPERSTRIP, height);
                    output.SetField(TiffTag.XRESOLUTION, 88.0);
                    output.SetField(TiffTag.YRESOLUTION, 88.0);
                    output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                    output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);

                    if (z % 2 == 0)
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                    else
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISWHITE);

                    //                    output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                    output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                    output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                    // specify that it's a page within the multipage file
                    output.SetField(TiffTag.SUBFILETYPE, FileType.PAGE);
                    // specify the page number
                    output.SetField(TiffTag.PAGENUMBER, z, Data.GetLength(2));

                    float[] samples = new float[width];
                    byte[] buffer = new byte[samples.Length * sizeof(float)];
                    for (int i = 0; i < height; i++)
                    {

                        for (int j = 0; j < width; j++)
                            samples[j] = (float)(Data[j, i, z]);


                        Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                        output.WriteScanline(buffer, i);
                    }
                    output.WriteDirectory();
                }
            }

        }
        public static void Save_Tiff_Stack(string Filename, float[, ,] Data)
        {
            int width = Data.GetLength(0);
            int height = Data.GetLength(1);

            using (Tiff output = Tiff.Open(Filename, "w"))
            {
                for (int z = 0; z < Data.GetLength(2); z++)
                {
                    // Emgu.CV.Image<Gray, float> image = new Emgu.CV.Image<Gray, float>(Data.GetLength(1), Data.GetLength(2));


                    output.SetField(TiffTag.IMAGEWIDTH, width);
                    output.SetField(TiffTag.IMAGELENGTH, height);
                    output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                    output.SetField(TiffTag.SAMPLEFORMAT, BitMiracle.LibTiff.Classic.SampleFormat.IEEEFP);
                    output.SetField(TiffTag.BITSPERSAMPLE, 32);
                    output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);

                    output.SetField(TiffTag.ROWSPERSTRIP, output.DefaultStripSize(0));
                    //output.SetField(TiffTag.ROWSPERSTRIP, height);
                    output.SetField(TiffTag.XRESOLUTION, 88.0);
                    output.SetField(TiffTag.YRESOLUTION, 88.0);
                    output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                    output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);

                    if (z % 2 == 0)
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                    else
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISWHITE);

                    //                    output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                    output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                    output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                    // specify that it's a page within the multipage file
                    output.SetField(TiffTag.SUBFILETYPE, FileType.PAGE);
                    // specify the page number
                    output.SetField(TiffTag.PAGENUMBER, z, Data.GetLength(2));


                    for (int i = 0; i < height; i++)
                    {
                        float[] samples = new float[width];
                        for (int j = 0; j < width; j++)
                            samples[j] = (float)(Data[j, i, z]);

                        byte[] buffer = new byte[samples.Length * sizeof(float)];
                        Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                        output.WriteScanline(buffer, i);
                    }
                    output.WriteDirectory();
                }
            }
        }
        public static void Save_Tiff_Stack(string Filename, double[][,] Data)
        {
            int width = Data[0].GetLength(0);
            int height = Data[0].GetLength(1);

            using (Tiff output = Tiff.Open(Filename, "w"))
            {
                for (int z = 0; z < Data.Length; z++)
                {
                    output.SetField(TiffTag.IMAGEWIDTH, width);
                    output.SetField(TiffTag.IMAGELENGTH, height);
                    output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                    output.SetField(TiffTag.SAMPLEFORMAT, BitMiracle.LibTiff.Classic.SampleFormat.IEEEFP);
                    output.SetField(TiffTag.BITSPERSAMPLE, 32);
                    output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);

                    output.SetField(TiffTag.ROWSPERSTRIP, output.DefaultStripSize(0));
                    output.SetField(TiffTag.XRESOLUTION, 88.0);
                    output.SetField(TiffTag.YRESOLUTION, 88.0);
                    output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                    output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);

                    if (z % 2 == 0)
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                    else
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISWHITE);

                    output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                    output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                    // specify that it's a page within the multipage file
                    output.SetField(TiffTag.SUBFILETYPE, FileType.PAGE);
                    // specify the page number
                    output.SetField(TiffTag.PAGENUMBER, z, Data.GetLength(2));


                    for (int i = 0; i < height; i++)
                    {
                        float[] samples = new float[width];
                        for (int j = 0; j < width; j++)
                            samples[j] = (float)(Data[z][j, i]);

                        byte[] buffer = new byte[samples.Length * sizeof(float)];
                        Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                        output.WriteScanline(buffer, i);
                    }
                    output.WriteDirectory();
                }
            }
        }
        public static void Save_Tiff_Stack(string Filename, float[][,] Data)
        {
            int width = Data[0].GetLength(0);
            int height = Data[0].GetLength(1);

            using (Tiff output = Tiff.Open(Filename, "w"))
            {
                for (int z = 0; z < Data.Length; z++)
                {
                    output.SetField(TiffTag.IMAGEWIDTH, width);
                    output.SetField(TiffTag.IMAGELENGTH, height);
                    output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                    output.SetField(TiffTag.SAMPLEFORMAT, BitMiracle.LibTiff.Classic.SampleFormat.IEEEFP);
                    output.SetField(TiffTag.BITSPERSAMPLE, 32);
                    output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);

                    output.SetField(TiffTag.ROWSPERSTRIP, output.DefaultStripSize(0));
                    output.SetField(TiffTag.XRESOLUTION, 88.0);
                    output.SetField(TiffTag.YRESOLUTION, 88.0);
                    output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                    output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);

                    if (z % 2 == 0)
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                    else
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISWHITE);

                    output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                    output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                    // specify that it's a page within the multipage file
                    output.SetField(TiffTag.SUBFILETYPE, FileType.PAGE);
                    // specify the page number
                    output.SetField(TiffTag.PAGENUMBER, z, Data.GetLength(2));


                    for (int i = 0; i < height; i++)
                    {
                        float[] samples = new float[width];
                        for (int j = 0; j < width; j++)
                            samples[j] = (float)(Data[z][j, i]);

                        byte[] buffer = new byte[samples.Length * sizeof(float)];
                        Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                        output.WriteScanline(buffer, i);
                    }
                    output.WriteDirectory();
                }
            }
        }



        public static void Save_Tiff_VirtualStack(string Filename, double[, ,] Data, double MinValue, int BitDepth)
        {
            int width = Data.GetLength(0);
            int height = Data.GetLength(1);

            double MaxValue = Data.MaxArray();

            double Scale;

            if (BitDepth == 8)
            {
                Scale = byte.MaxValue / (MaxValue - MinValue);
            }
            else
            {
                Scale = Int16.MaxValue / (MaxValue - MinValue);
            }
            string pPath = Path.GetDirectoryName(Filename);
            string pFilename = Path.GetFileNameWithoutExtension(Filename);
            for (int z = 0; z < Data.GetLength(2); z++)
            {

                using (Tiff output = Tiff.Open(pPath + "\\" + pFilename + string.Format("{0:0000}.tif", z), "w"))
                {

                    output.SetField(TiffTag.IMAGEWIDTH, width);
                    output.SetField(TiffTag.IMAGELENGTH, height);
                    output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                    output.SetField(TiffTag.BITSPERSAMPLE, BitDepth);
                    output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                    output.SetField(TiffTag.ROWSPERSTRIP, height);
                    output.SetField(TiffTag.XRESOLUTION, 88.0);
                    output.SetField(TiffTag.YRESOLUTION, 88.0);
                    output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                    output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                    output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                    output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                    output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                    if (BitDepth == 8)
                    {
                        byte[] samples = new byte[width];
                        byte val;
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                val = (byte)(Scale * (Data[i, j, z] - MinValue)); ;
                                if (val < 0) val = 0;

                                samples[j] = val;
                            }

                            output.WriteScanline(samples, i);
                        }
                    }
                    else
                    {
                        byte[] buffer = new byte[width * sizeof(short)];
                        ushort[] samples = new ushort[width];
                        ushort val;
                        for (int i = 0; i < height; i++)
                        {

                            for (int j = 0; j < width; j++)
                            {
                                val = (ushort)(Scale * (Data[i, j, z] - MinValue)); ;
                                if (val < 0) val = 0;

                                samples[j] = val;
                            }

                            Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                            output.WriteScanline(buffer, i);
                        }
                    }
                }
            }

        }
        public static void Save_Tiff_VirtualStack(string Filename, float[, ,] Data, float MinValue, int BitDepth)
        {
            int width = Data.GetLength(0);
            int height = Data.GetLength(1);

            double MaxValue = Data.MaxArray();
            double Scale;

            if (BitDepth == 8)
            {
                Scale = byte.MaxValue / (MaxValue - MinValue);
            }
            else
            {
                Scale = Int16.MaxValue / (MaxValue - MinValue);
            }
            string pPath = Path.GetDirectoryName(Filename);
            string pFilename = Path.GetFileNameWithoutExtension(Filename);
            for (int z = 0; z < Data.GetLength(2); z++)
            {
                using (Tiff output = Tiff.Open(pPath + "\\" + pFilename + string.Format("{0:0000}.tif", z), "w"))
                {

                    output.SetField(TiffTag.IMAGEWIDTH, width);
                    output.SetField(TiffTag.IMAGELENGTH, height);
                    output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                    output.SetField(TiffTag.BITSPERSAMPLE, BitDepth);
                    output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                    output.SetField(TiffTag.ROWSPERSTRIP, height);
                    output.SetField(TiffTag.XRESOLUTION, 88.0);
                    output.SetField(TiffTag.YRESOLUTION, 88.0);
                    output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                    output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                    output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                    output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                    output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                    if (BitDepth == 8)
                    {
                        byte[] samples = new byte[width];
                        byte val;
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                val = (byte)(Scale * (Data[i, j, z] - MinValue)); ;
                                if (val < 0) val = 0;
                                samples[j] = val;
                            }
                            //byte[] buffer = new byte[samples.Length * sizeof(short)];
                            //Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                            output.WriteScanline(samples, i);
                        }
                    }
                    else
                    {
                        byte[] buffer = new byte[width * sizeof(short)];
                        short[] samples = new short[width];
                        short val;
                        for (int i = 0; i < height; i++)
                        {

                            for (int j = 0; j < width; j++)
                            {
                                val = (short)(Scale * (Data[i, j, z] - MinValue)); ;
                                if (val < 0) val = 0;
                                samples[j] = val;
                            }

                            Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                            output.WriteScanline(buffer, i);
                        }
                    }
                }
            }
        }
        public static void Save_Tiff_VirtualStack(string Filename, double[][,] Data, double MinValue, int BitDepth)
        {
            int width = Data.GetLength(0);
            int height = Data.GetLength(1);

            double MaxValue = 0;
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                double mValue = Data[i].MaxArray();
                if (mValue > MaxValue) MaxValue = mValue;
            }
            double Scale;

            if (BitDepth == 8)
            {
                Scale = byte.MaxValue / (MaxValue - MinValue);
            }
            else
            {
                Scale = Int16.MaxValue / (MaxValue - MinValue);
            }
            string pPath = Path.GetDirectoryName(Filename);
            string pFilename = Path.GetFileNameWithoutExtension(Filename);
            for (int z = 0; z < Data.Length; z++)
            {
                using (Tiff output = Tiff.Open(pPath + "\\" + pFilename + string.Format("{0:0000}.tif", z), "w"))
                {

                    output.SetField(TiffTag.IMAGEWIDTH, width);
                    output.SetField(TiffTag.IMAGELENGTH, height);
                    output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                    output.SetField(TiffTag.BITSPERSAMPLE, BitDepth);
                    output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                    output.SetField(TiffTag.ROWSPERSTRIP, height);
                    output.SetField(TiffTag.XRESOLUTION, 88.0);
                    output.SetField(TiffTag.YRESOLUTION, 88.0);
                    output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                    output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                    output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                    output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                    output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                    if (BitDepth == 8)
                    {
                        byte[] samples = new byte[width];
                        byte val;
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                val = (byte)(Scale * (Data[z][i, j] - MinValue)); ;
                                if (val < 0) val = 0;
                                samples[j] = val;
                            }
                            //byte[] buffer = new byte[samples.Length * sizeof(short)];
                            //Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                            output.WriteScanline(samples, i);
                        }
                    }
                    else
                    {
                        byte[] buffer = new byte[width * sizeof(short)];
                        short[] samples = new short[width];
                        short val;
                        for (int i = 0; i < height; i++)
                        {

                            for (int j = 0; j < width; j++)
                            {
                                val = (short)(Scale * (Data[z][i, j] - MinValue)); ;
                                if (val < 0) val = 0;
                                samples[j] = val;
                            }
                            Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                            output.WriteScanline(buffer, i);
                        }
                    }
                }
            }
        }
        public static void Save_Tiff_VirtualStack(string Filename, float[][,] Data, float MinValue, int BitDepth)
        {
            int width = Data.GetLength(0);
            int height = Data.GetLength(1);

            double MaxValue = 0;
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                double mValue = Data[i].MaxArray();
                if (mValue > MaxValue) MaxValue = mValue;
            }
            double Scale;

            if (BitDepth == 8)
            {
                Scale = byte.MaxValue / (MaxValue - MinValue);
            }
            else
            {
                Scale = Int16.MaxValue / (MaxValue - MinValue);
            }

            string pPath = Path.GetDirectoryName(Filename);
            string pFilename = Path.GetFileNameWithoutExtension(Filename);

            for (int z = 0; z < Data.Length; z++)
            {
                using (Tiff output = Tiff.Open(pPath + "\\" + pFilename + string.Format("{0:0000}.tif", z), "w"))
                {

                    output.SetField(TiffTag.IMAGEWIDTH, width);
                    output.SetField(TiffTag.IMAGELENGTH, height);
                    output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                    output.SetField(TiffTag.BITSPERSAMPLE, BitDepth);
                    output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                    output.SetField(TiffTag.ROWSPERSTRIP, height);
                    output.SetField(TiffTag.XRESOLUTION, 88.0);
                    output.SetField(TiffTag.YRESOLUTION, 88.0);
                    output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                    output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                    output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                    output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                    output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                    if (BitDepth == 8)
                    {
                        byte[] samples = new byte[width];
                        byte val;
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                val = (byte)(Scale * (Data[z][i, j] - MinValue)); ;
                                if (val < 0) val = 0;
                                samples[j] = val;
                            }
                            //byte[] buffer = new byte[samples.Length * sizeof(short)];
                            //Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                            output.WriteScanline(samples, i);
                        }
                    }
                    else
                    {
                        byte[] buffer = new byte[width * sizeof(short)];
                        short[] samples = new short[width];
                        short val;
                        for (int i = 0; i < height; i++)
                        {

                            for (int j = 0; j < width; j++)
                            {
                                val = (short)(Scale * (Data[z][i, j] - MinValue)); ;
                                if (val < 0) val = 0;
                                samples[j] = val;
                            }
                            Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                            output.WriteScanline(buffer, i);
                        }
                    }
                }
            }
        }

        public static void Save_TIFF(string Filename, ImageHolder imageData)
        {

            int width = imageData.Width;
            int height = imageData.Height;

            float[, ,] ImageArray = imageData.ImageData;

            using (Tiff output = Tiff.Open(Filename, "w"))
            {
                output.SetField(TiffTag.IMAGEWIDTH, width);
                output.SetField(TiffTag.IMAGELENGTH, height);
                output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                output.SetField(TiffTag.SAMPLEFORMAT, BitMiracle.LibTiff.Classic.SampleFormat.IEEEFP);
                output.SetField(TiffTag.BITSPERSAMPLE, 32);
                output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                output.SetField(TiffTag.ROWSPERSTRIP, height);
                output.SetField(TiffTag.XRESOLUTION, 88.0);
                output.SetField(TiffTag.YRESOLUTION, 88.0);
                output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                for (int i = 0; i < height; i++)
                {
                    float[] samples = new float[width];
                    for (int j = 0; j < width; j++)
                        samples[j] = ImageArray[i,j, 0];

                    byte[] buffer = new byte[samples.Length * sizeof(float)];
                    Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                    output.WriteScanline(buffer, i);
                }
            }
        }

        public static void Save_16bit_TIFF(string Filename, ImageHolder imageData, float Scale, float Min)
        {

            int width = imageData.Width;
            int height = imageData.Height;

            float[, ,] ImageArray = imageData.ImageData;

            if (imageData.NChannels == 1)
            {
                Emgu.CV.Image<Emgu.CV.Structure.Gray, UInt16> image = new Emgu.CV.Image<Gray, ushort>(imageData.Width, imageData.Height);
                UInt16[, ,] samples = image.Data;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                        samples[i, j, 0] = (UInt16)(Scale * (ImageArray[i, j, 0] - Min));
                }
                image.Save(Filename);
            }
            else
            {
                Emgu.CV.Image<Emgu.CV.Structure.Bgr, UInt16> image = new Emgu.CV.Image<Bgr, ushort>(imageData.Width, imageData.Height);
                UInt16[, ,] samples = image.Data;
                for (int nC = 0; nC < 3; nC++)
                {
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                            samples[i, j, nC] = (UInt16)(Scale * (ImageArray[i, j, nC] - Min));
                    }
                }
                image.Save(Filename);
            }


            /*using (Tiff output = Tiff.Open(Filename, "w"))
            {
                output.SetField(TiffTag.IMAGEWIDTH, width);
                output.SetField(TiffTag.IMAGELENGTH, height);
                output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                output.SetField(TiffTag.BITSPERSAMPLE, 16);
                output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                output.SetField(TiffTag.ROWSPERSTRIP, height);
                output.SetField(TiffTag.XRESOLUTION, 88.0);
                output.SetField(TiffTag.YRESOLUTION, 88.0);
                output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                Random random = new Random();
                for (int i = 0; i < height; i++)
                {
                    short[] samples = new short[width];
                    for (int j = 0; j < width; j++)
                        samples[j] = (short)(Scale * (ImageArray[i, j, 0] - Min));

                    byte[] buffer = new byte[samples.Length * sizeof(short)];
                    Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                    output.WriteScanline(buffer, i);
                }
            }*/
        }

        public static void Save_16bit_TIFF(string Filename, Bitmap imageData)
        {
            Emgu.CV.Image<Emgu.CV.Structure.Bgr, UInt16> image = new Emgu.CV.Image<Bgr, ushort>(imageData);

            image.Save(Filename);
        }
        public static void Save_16bit_TIFF(string Filename, double[,] imageData, double Scale, double Min)
        {
            int width = imageData.GetLength(0);
            int height = imageData.GetLength(1);


            Emgu.CV.Image<Emgu.CV.Structure.Gray, UInt16> image = new Emgu.CV.Image<Gray, ushort>(width, height);
            UInt16[, ,] samples = image.Data;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                    samples[i, j, 0] = (UInt16)(Scale * (imageData[i, j] - Min));
            }
            image.Save(Filename);


            /* using (Tiff output = Tiff.Open(Filename, "w"))
             {
                 output.SetField(TiffTag.IMAGEWIDTH, width);
                 output.SetField(TiffTag.IMAGELENGTH, height);
                 output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                 output.SetField(TiffTag.BITSPERSAMPLE, 16);
                 output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                 output.SetField(TiffTag.ROWSPERSTRIP, height);
                 output.SetField(TiffTag.XRESOLUTION, 88.0);
                 output.SetField(TiffTag.YRESOLUTION, 88.0);
                 output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                 output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                 output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                 output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                 output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                 for (int i = 0; i < height; i++)
                 {
                     short[] samples = new short[width];
                     for (int j = 0; j < width; j++)
                         samples[j] = (short)(Scale * (imageData[j, i] - Min));

                     byte[] buffer = new byte[samples.Length * sizeof(short)];
                     Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                     output.WriteScanline(buffer, i);
                 }
             }*/


        }
        public static void Save_16bit_TIFF(string Filename, float[,] imageData, double Scale, double Min)
        {
            int width = imageData.GetLength(0);
            int height = imageData.GetLength(1);


            Emgu.CV.Image<Emgu.CV.Structure.Gray, UInt16> image = new Emgu.CV.Image<Gray, ushort>(width, height);
            UInt16[, ,] samples = image.Data;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                    samples[i, j, 0] = (UInt16)(Scale * (imageData[i, j] - Min));
            }
            image.Save(Filename);


            /* using (Tiff output = Tiff.Open(Filename, "w"))
             {
                 output.SetField(TiffTag.IMAGEWIDTH, width);
                 output.SetField(TiffTag.IMAGELENGTH, height);
                 output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                 output.SetField(TiffTag.BITSPERSAMPLE, 16);
                 output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                 output.SetField(TiffTag.ROWSPERSTRIP, height);
                 output.SetField(TiffTag.XRESOLUTION, 88.0);
                 output.SetField(TiffTag.YRESOLUTION, 88.0);
                 output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                 output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                 output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                 output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                 output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                 for (int i = 0; i < height; i++)
                 {
                     short[] samples = new short[width];
                     for (int j = 0; j < width; j++)
                         samples[j] = (short)(Scale * (imageData[j, i] - Min));

                     byte[] buffer = new byte[samples.Length * sizeof(short)];
                     Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                     output.WriteScanline(buffer, i);
                 }
             }*/
        }

        public static void Save_TIFF(string Filename, double[,] imageData)
        {
            int width = imageData.GetLength(0);
            int height = imageData.GetLength(1);


            using (Tiff output = Tiff.Open(Filename, "w"))
            {
                output.SetField(TiffTag.IMAGEWIDTH, width);
                output.SetField(TiffTag.IMAGELENGTH, height);
                output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                output.SetField(TiffTag.SAMPLEFORMAT, BitMiracle.LibTiff.Classic.SampleFormat.IEEEFP);
                output.SetField(TiffTag.BITSPERSAMPLE, 32);
                output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                output.SetField(TiffTag.ROWSPERSTRIP, height);
                output.SetField(TiffTag.XRESOLUTION, 88.0);
                output.SetField(TiffTag.YRESOLUTION, 88.0);
                output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                for (int i = 0; i < height; i++)
                {
                    float[] samples = new float[width];
                    for (int j = 0; j < width; j++)
                        samples[j] = (float)(imageData[j, i]);

                    byte[] buffer = new byte[samples.Length * sizeof(float)];
                    Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                    output.WriteScanline(buffer, i);
                }
            }
        }

        public static void Save_TIFF(string Filename, float[,] imageData)
        {
            int width = imageData.GetLength(0);
            int height = imageData.GetLength(1);


            using (Tiff output = Tiff.Open(Filename, "w"))
            {
                output.SetField(TiffTag.IMAGEWIDTH, width);
                output.SetField(TiffTag.IMAGELENGTH, height);
                output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                output.SetField(TiffTag.SAMPLEFORMAT, BitMiracle.LibTiff.Classic.SampleFormat.IEEEFP);
                output.SetField(TiffTag.BITSPERSAMPLE, 32);
                output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                output.SetField(TiffTag.ROWSPERSTRIP, height);
                output.SetField(TiffTag.XRESOLUTION, 88.0);
                output.SetField(TiffTag.YRESOLUTION, 88.0);
                output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                for (int i = 0; i < height; i++)
                {
                    float[] samples = new float[width];
                    for (int j = 0; j < width; j++)
                        samples[j] = imageData[j, i];

                    byte[] buffer = new byte[samples.Length * sizeof(float)];
                    Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);
                    output.WriteScanline(buffer, i);
                }
            }
        }

        public static void Save_TIFF(string Filename, Bitmap imageData)
        {
            Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> image = new Emgu.CV.Image<Bgr, byte>(imageData);

            image.Save(Filename);
            /*
            using (Tiff tif = Tiff.Open(Filename, "w"))
            {
                byte[] raster = getImageRasterBytes(imageData, imageData.PixelFormat);
                tif.SetField(TiffTag.IMAGEWIDTH, imageData.Width);
                tif.SetField(TiffTag.IMAGELENGTH, imageData.Height);
                tif.SetField(TiffTag.COMPRESSION, Compression.LZW);
                tif.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);

                tif.SetField(TiffTag.ROWSPERSTRIP, imageData.Height);

                tif.SetField(TiffTag.XRESOLUTION, imageData.HorizontalResolution);
                tif.SetField(TiffTag.YRESOLUTION, imageData.VerticalResolution);

                tif.SetField(TiffTag.BITSPERSAMPLE, 8);
                tif.SetField(TiffTag.SAMPLESPERPIXEL, 3);

                tif.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);

                int stride = raster.Length / imageData.Height;
                convertSamples(raster, imageData.Width, imageData.Height);

                for (int i = 0, offset = 0; i < imageData.Height; i++)
                {
                    tif.WriteScanline(raster, offset, i, 0);
                    offset += stride;
                }
            }*/
        }
        #endregion

        private static byte[] getImageRasterBytes(Bitmap bmp, PixelFormat format)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            byte[] bits = null;

            try
            {
                // Lock the managed memory
                BitmapData bmpdata = bmp.LockBits(rect, ImageLockMode.ReadWrite, format);

                // Declare an array to hold the bytes of the bitmap.
                bits = new byte[bmpdata.Stride * bmpdata.Height];

                // Copy the values into the array.
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, bits, 0, bits.Length);

                // Release managed memory
                bmp.UnlockBits(bmpdata);
            }
            catch
            {
                return null;
            }

            return bits;
        }

        /// <summary>
        /// Converts BGR samples into RGB samples
        /// </summary>
        private static void convertSamples(byte[] data, int width, int height)
        {
            int stride = data.Length / height;
            const int samplesPerPixel = 3;

            for (int y = 0; y < height; y++)
            {
                int offset = stride * y;
                int strideEnd = offset + width * samplesPerPixel;

                for (int i = offset; i < strideEnd; i += samplesPerPixel)
                {
                    byte temp = data[i + 2];
                    data[i + 2] = data[i];
                    data[i] = temp;
                }
            }
        }


        /// <summary>
        /// takes a volume and saves the three axis cross
        /// </summary>
        /// <param name="Filename"></param>
        public static  void SaveCross(string Filename, float[,,] DataWhole)
        {
            string Extension = Path.GetExtension(Filename).ToLower();
            if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                #region Save as Images

                int ZSlices = DataWhole.GetLength(0);
                int XSlices = DataWhole.GetLength(1);
                int YSlices = DataWhole.GetLength(2);

                string outFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename);


                {
                    double[,] Slice = new double[XSlices, YSlices];
                    for (int y = 0; y < YSlices; y++)
                    {
                        for (int x = 0; x < XSlices; x++)
                        {
                            Slice[x, y] = DataWhole[ZSlices / 2, x, y];
                        }
                    }
                    Bitmap b = Slice.MakeBitmap();
                    b.Save(outFile + "_Z" + Extension);
                }

                {
                    double[,] SliceX = new double[XSlices, ZSlices];
                    for (int z = 0; z < ZSlices; z++)
                    {
                        for (int x = 0; x < XSlices; x++)
                        {
                            SliceX[x, z] = DataWhole[z, x, YSlices / 2];
                        }
                    }
                    Bitmap b = SliceX.MakeBitmap();
                    b.Save(outFile + "_X" + Extension);
                }

                {
                    double[,] SliceY = new double[YSlices, ZSlices];
                    for (int z = 0; z < ZSlices; z++)
                    {
                        for (int y = 0; y < YSlices; y++)
                        {
                            SliceY[y, z] = DataWhole[z, XSlices / 2, y];
                        }
                    }
                    Bitmap b = SliceY.MakeBitmap();
                    b.Save(outFile + "_Y" + Extension);
                }
                #endregion
            }

        }

        #endregion

        static object CriticalSectionFIBLoad = new object();

        #region Load Bitmaps

        public static float[,,] Load_TiffStack(string Filename)
        {
            int width=100;
            int height=100;
            int pageCount = 0;
            using (Tiff tif = Tiff.Open(Filename, "r"))
            {
                FieldValue[] value = tif.GetField(TiffTag.IMAGEWIDTH);
                width = value[0].ToInt();

                value = tif.GetField(TiffTag.IMAGELENGTH);
                height = value[0].ToInt();
                
                do
                {
                    ++pageCount;
                } while (tif.ReadDirectory());
            }
            float[, ,] DataCube = new float[width, height, pageCount];

            using (Tiff tif = Tiff.Open(Filename, "r"))
            {
                FieldValue[] value = tif.GetField(TiffTag.SAMPLEFORMAT);

                if (value[0].Value.ToString() == "IEEEFP")
                {
                     int LineWidth = width * sizeof(float);
                        byte[] buffer = new byte[width * sizeof(float)];
                        int LineCount = Buffer.ByteLength(buffer);
                    int pageSize  = LineCount * height ;
                    for (int page = 0; page < pageCount; page++)
                    {
                        for (int i = 0; i < height; i++)
                        {
                            tif.ReadScanline(buffer, i);
                            Buffer.BlockCopy(buffer, 0, DataCube, i * LineWidth + page*pageSize , LineCount);
                        }
                        tif.ReadDirectory();
                    }

                }
                else
                {
                    // Read the image into the memory buffer
                    int[] raster = new int[height * width];

                    if (!tif.ReadRGBAImage(width, height, raster))
                    {
                        System.Windows.Forms.MessageBox.Show("Could not read image");
                        return null;
                    }

                }
            }

            return DataCube;
        }

        public static ImageHolder Load_Tiff(string Filename)
        {
            using (Tiff tif = Tiff.Open(Filename, "r"))
            {
                /*  output.SetField(TiffTag.IMAGEWIDTH, width);
                output.SetField(TiffTag.IMAGELENGTH, height);
                output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                output.SetField(TiffTag.SAMPLEFORMAT, BitMiracle.LibTiff.Classic.SampleFormat.IEEEFP);
                output.SetField(TiffTag.BITSPERSAMPLE, 32);
                output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                output.SetField(TiffTag.ROWSPERSTRIP, height);
                output.SetField(TiffTag.XRESOLUTION, 88.0);
                output.SetField(TiffTag.YRESOLUTION, 88.0);
                output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);*/

                // Find the width and height of the image
                FieldValue[] value = tif.GetField(TiffTag.IMAGEWIDTH);
                int width = value[0].ToInt();

                value = tif.GetField(TiffTag.IMAGELENGTH);
                int height = value[0].ToInt();

                value = tif.GetField(TiffTag.SAMPLEFORMAT);

                if (value[0].Value.ToString()  =="IEEEFP")
                {
                    ImageHolder ih = new ImageHolder(width, height, 1);
                    float[, ,] data = ih.ImageData;
                    int LineWidth =  width * sizeof(float);
                     byte[] buffer = new byte[width * sizeof(float)];
                    int LineCount = Buffer.ByteLength(buffer);
                    for (int i = 0; i < height; i++)
                    {
                        tif.ReadScanline(buffer, i);
                        Buffer.BlockCopy(buffer, 0, data, i*LineWidth , LineCount );
                    }
                   // Bitmap b = ih.ToBitmap();
                  
                    return ih;
                }
                else
                {
                    // Read the image into the memory buffer
                    int[] raster = new int[height * width];

                    if (!tif.ReadRGBAImage(width, height, raster))
                    {
                        System.Windows.Forms.MessageBox.Show("Could not read image");
                        return null;
                    }

                }
            }
            return null;
        }

        static Semaphore FIBLoadLock = new Semaphore(4, 10);

        public static ImageHolder Load_Bitmap(string Filename)
        {
            string exten = Path.GetExtension(Filename).ToLower();
            if (exten == ".ivg")
            {
                ImageHolder ih = LoadIVGFile(Filename);
                return ih;
            }
            else if (exten == ".cct" || exten == ".raw")
            {
                return Load_Raw(Filename);
            }
            else
            {
                if (Path.GetExtension(Filename)=="tif")
                {
                    return Load_Tiff(Filename);

                }
                else
                {
                    Emgu.CV.Image<Gray, float> image = new Emgu.CV.Image<Gray, float>(Filename);

                    image.Rotate(90, new Gray(0));

                    ImageHolder ret = new ImageHolder(image);
                    // Bitmap b= ret.ToBitmap();
                    return ret;
                }
                /*
                FreeImageAPI.FreeImageBitmap fib;
                FIBLoadLock.WaitOne();
                try
                {
                    fib = new FreeImageAPI.FreeImageBitmap(Filename);
                }
                catch (Exception ex)
                {

                    FIBLoadLock.Release();

                    try
                    {
                        Bitmap b = new Bitmap(Filename);
                        return new ImageHolder(b);
                    }
                    catch
                    {
                        throw ex;
                    }
                }

                FIBLoadLock.Release();
                if (fib.ColorDepth == 16)
                    return ConvertFIBtoImageholder(fib);
                else
                {
                    return new ImageHolder((Bitmap)fib);
                }
                 
                 */


            }
        }

        /* private static ImageHolder ConvertFIBtoImageholder(FreeImageAPI.FreeImageBitmap fib)
         {
             int iWidth = fib.Width;
             int iHeight = fib.Height;
             int iNChannels = 1;

             ImageHolder iOut = new ImageHolder(iWidth, iHeight, iNChannels);
             float[, ,] mImageData = iOut.ImageData;

             unsafe
             {

                 for (int y = 0; y < iHeight; y++)
                 {

                     FreeImageAPI.Scanline<UInt16> line = fib.GetScanline<UInt16>(y);

                     for (int x = 0; x < iWidth; x++)
                     {
                         mImageData[y, x, 0] = line[x];
                     }
                 }
             }
             return iOut;
         }*/

        public static ImageHolder Load_Bitmap(string Filename, int Channel)
        {
            string exten = Path.GetExtension(Filename).ToLower();
            if (exten == ".ivg")
            {
                ImageHolder ih = LoadIVGFile(Filename, Channel);
                return ih;
            }
            else if (exten == ".cct")
            {
                return Load_Raw(Filename);
            }
            else
            {
                Emgu.CV.Image<Bgr, float> image = new Emgu.CV.Image<Bgr, float>(Filename);

                ImageHolder ret = new ImageHolder(image, Channel);
                // Bitmap b= ret.ToBitmap();
                return ret;

                /*FreeImageAPI.FreeImageBitmap fib;
                FIBLoadLock.WaitOne();
                fib = new FreeImageAPI.FreeImageBitmap(Filename);
                FIBLoadLock.Release();
                if (fib.ColorDepth == 16)
                    return ConvertFIBtoImageholder(fib);
                else
                {
                    return new ImageHolder((Bitmap)fib);
                }*/
            }
        }

        public static ImageHolder Load_Raw(string Filename)
        {
            FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            BinaryReader Reader = new BinaryReader(BinaryFile);
            BinaryFile.Seek(0, SeekOrigin.Begin);
            int sizeX, sizeY, sizeZ;

            int ArrayRank = Reader.ReadInt32();

            if (ArrayRank != 2)
                MessageBox.Show("Can only display 2D arrays");

            sizeX = Reader.ReadInt32();
            sizeY = Reader.ReadInt32();
            sizeZ = Reader.ReadInt32();

            Reader.ReadDouble();
            Reader.ReadDouble();
            Reader.ReadDouble();
            Reader.ReadDouble();
            Reader.ReadDouble();
            Reader.ReadDouble();

            double[,] image = new double[sizeX, sizeY];


            byte[] buffer = new byte[Buffer.ByteLength(image)];
            Reader.Read(buffer, 0, buffer.Length);

            Buffer.BlockCopy(buffer, 0, image, 0, buffer.Length);

            Reader.Close();
            BinaryFile.Close();
            return MathHelpLib.ImageProcessing.MathImageHelps.ConvertToGrayScaleImage(image, 1);
        }

        public static double[,] Load_RawToDouble(string Filename)
        {
            FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            BinaryReader Reader = new BinaryReader(BinaryFile);
            BinaryFile.Seek(0, SeekOrigin.Begin);
            int sizeX, sizeY, sizeZ;

            int ArrayRank = Reader.ReadInt32();

            if (ArrayRank != 2)
                MessageBox.Show("Can only display 2D arrays");

            sizeX = Reader.ReadInt32();
            sizeY = Reader.ReadInt32();
            sizeZ = Reader.ReadInt32();

            Reader.ReadDouble();
            Reader.ReadDouble();
            Reader.ReadDouble();
            Reader.ReadDouble();
            Reader.ReadDouble();
            Reader.ReadDouble();

            double[,] image = new double[sizeX, sizeY];


            byte[] buffer = new byte[Buffer.ByteLength(image)];
            Reader.Read(buffer, 0, buffer.Length);

            Buffer.BlockCopy(buffer, 0, image, 0, buffer.Length);

            Reader.Close();
            BinaryFile.Close();
            return image;
        }

        public static ImageHolder LoadIVGFileTest(string Filename)
        {
            using (BinaryReader b = new BinaryReader(File.Open(Filename, FileMode.Open)))
            {
                int pos = 0;
                ImageHolder image;//= new Bitmap(width, height, PixelFormat.Format32bppRgb);
                int length = (int)b.BaseStream.Length;
                {
                    int MagicNumber1 = b.ReadInt16();
                    // Image.Close();
                    int MagicNumber2 = b.ReadInt16();

                    if ((MagicNumber1 != 26454) || (MagicNumber2 != 25171))
                    {
                        throw new Exception("Wrong file format");
                    }

                    int width = b.ReadInt16();
                    int height = b.ReadInt16();
                    int bitsPerPixel = b.ReadInt16();

                    b.ReadInt32();
                    b.ReadInt16();
                    b.ReadInt16();
                    b.ReadInt16();

                    double RealBBP = Math.Round((double)((length - 20) / width / height));

                    byte[] Buffer = b.ReadBytes(length - 20);

                    image = new ImageHolder(width, height, 1);
                    float[, ,] bmd = image.ImageData;
                    float Gray;


                    if (RealBBP == 2)
                    {
                        unsafe
                        {
                            fixed (byte* pBuffer = Buffer)
                            {
                                ushort* pIn = (ushort*)pBuffer;

                                for (int y = 0; y < height; y++)
                                {
                                    for (int x = 0; x < width; x++)
                                    {
                                        Gray = *pIn;

                                        bmd[y, x, 0] = Gray;
                                        pIn++;
                                    }
                                }


                                pIn = (ushort*)pBuffer;

                                for (int y = 0; y < height; y += 2)
                                {
                                    for (int x = 0; x < width; x += 2)
                                    {
                                        //bmd[y, x, 0] = Gray;
                                    }
                                }


                            }
                        }
                    }
                    else
                        throw new Exception("That pixelformat is not supported");

                    b.Close();
                }


                return image;
            }
        }

        public static ImageHolder LoadIVGFile(string Filename)
        {
            using (BinaryReader b = new BinaryReader(File.Open(Filename, FileMode.Open)))
            {
                int pos = 0;
                ImageHolder image;//= new Bitmap(width, height, PixelFormat.Format32bppRgb);
                int length = (int)b.BaseStream.Length;
                {
                    int MagicNumber1 = b.ReadInt16();
                    // Image.Close();
                    int MagicNumber2 = b.ReadInt16();

                    if ((MagicNumber1 != 26454) || (MagicNumber2 != 25171))
                    {
                        //  throw new Exception("Wrong file format");
                    }

                    int width = b.ReadInt16();
                    int height = b.ReadInt16();
                    int bitsPerPixel = b.ReadInt16();

                    b.ReadInt32();
                    b.ReadInt16();
                    b.ReadInt16();
                    b.ReadInt16();

                    double RealBBP = Math.Round((double)((length - 20) / width / height));

                    byte[] Buffer = b.ReadBytes(length - 20);

                    image = new ImageHolder(width, height, 1);
                    float[, ,] bmd = image.ImageData;
                    ushort Gray;


                    //  if (RealBBP == 2)
                    {
                        unsafe
                        {
                            fixed (byte* pBuffer = Buffer)
                            {
                                ushort* pIn = (ushort*)pBuffer;
                                ushort temp = 1;
                                byte* ptemp = (byte*)&temp;
                                for (int y = 0; y < height; y++)
                                {
                                    for (int x = 0; x < width; x++)
                                    {
                                        /* byte* t = (byte*)pIn;
                                         *ptemp = *(t + 1);
                                         *(ptemp + 1) = *(t);
                                         Gray = temp;*/
                                        Gray = *pIn;
                                        bmd[y, x, 0] = Gray;
                                        pIn++;
                                    }
                                }
                            }
                        }
                    }
                    //  else
                    //    throw new Exception("That pixelformat is not supported");
                    float max = MathHelpLib.MathArrayHelps.MaxArray(bmd);
                    b.Close();
                }
                return image;
            }
        }

        public static ImageHolder LoadIVGFile(string Filename, int Channel)
        {
            using (BinaryReader b = new BinaryReader(File.Open(Filename, FileMode.Open)))
            {
                int pos = 0;
                ImageHolder image;//= new Bitmap(width, height, PixelFormat.Format32bppRgb);
                int length = (int)b.BaseStream.Length;
                {
                    int MagicNumber1 = b.ReadInt16();
                    // Image.Close();
                    int MagicNumber2 = b.ReadInt16();

                    if ((MagicNumber1 != 26454) || (MagicNumber2 != 25171))
                    {
                        throw new Exception("Wrong file format");
                    }

                    int width = b.ReadInt16();
                    int height = b.ReadInt16();
                    int bitsPerPixel = b.ReadInt16();

                    b.ReadInt32();
                    b.ReadInt16();
                    b.ReadInt16();
                    b.ReadInt16();

                    double RealBBP = Math.Round((double)((length - 20) / width / height));

                    byte[] Buffer = b.ReadBytes(length - 20);

                    image = new ImageHolder(width, height, 1);
                    float[, ,] bmd = image.ImageData;
                    float Gray;


                    if (RealBBP == 2)
                    {
                        unsafe
                        {
                            fixed (byte* pBuffer = Buffer)
                            {
                                ushort* pIn = (ushort*)pBuffer;

                                for (int y = 0; y < height; y++)
                                {
                                    for (int x = 0; x < width; x++)
                                    {
                                        Gray = *pIn;

                                        bmd[y, x, 0] = Gray;
                                        pIn++;
                                    }
                                }
                            }
                        }
                    }
                    else
                        throw new Exception("That pixelformat is not supported");

                    b.Close();
                }
                return image;
            }
        }

        public static Bitmap LoadIVGFileBitmap(string Filename)
        {
            using (BinaryReader b = new BinaryReader(File.Open(Filename, FileMode.Open)))
            {
                int pos = 0;
                Bitmap image;//= new Bitmap(width, height, PixelFormat.Format32bppRgb);
                int length = (int)b.BaseStream.Length;

                {
                    int MagicNumber1 = b.ReadInt16();
                    // Image.Close();
                    int MagicNumber2 = b.ReadInt16();

                    if ((MagicNumber1 != 26454) || (MagicNumber2 != 25171))
                    {
                        throw new Exception("Wrong file format");
                    }

                    int width = b.ReadInt16();
                    int height = b.ReadInt16();
                    int bitsPerPixel = b.ReadInt16();

                    b.ReadInt32();
                    b.ReadInt16();
                    b.ReadInt16();
                    b.ReadInt16();

                    double RealBBP = Math.Round((double)((length - 20) / width / height));

                    byte[] Buffer = b.ReadBytes(length - 20);

                    image = new Bitmap(width, height, PixelFormat.Format32bppRgb);
                    BitmapData bmd = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, image.PixelFormat);
                    int Gray;
                    int Max = 0;

                    if (RealBBP == 2)
                    {
                        unsafe
                        {
                            fixed (byte* pBuffer = Buffer)
                            {
                                Int16* pIn = (Int16*)pBuffer;

                                int NElements = Buffer.Length / 2;
                                for (int i = 0; i < NElements; i++)
                                {
                                    Gray = *pIn;
                                    if (Gray > Max) Max = Gray;
                                    pIn++;
                                }
                                double dMax = (double)Max;

                                pIn = (Int16*)pBuffer;
                                long cc = 0;
                                byte bGray;
                                for (int y = 0; y < height; y++)
                                {
                                    byte* pPixel = ((byte*)bmd.Scan0 + y * bmd.Stride);
                                    for (int x = 0; x < width; x++)
                                    {
                                        Gray = *pIn;

                                        //Gray =(int)( (double) Gray / dMax *255d);
                                        Gray = Gray / 128;
                                        if (Gray > 255) Gray = 255;
                                        bGray = (byte)Gray;
                                        pPixel[0] = bGray;
                                        pPixel[1] = bGray;
                                        pPixel[2] = bGray;
                                        pPixel[3] = 255;
                                        pPixel += 4;
                                        cc++;
                                        pIn++;
                                    }
                                }
                            }
                        }
                    }
                    else
                        throw new Exception("That pixelformat is not supported");
                    // System.Diagnostics.Debug.Print((Int16.MaxValue/127).ToString());
                    image.UnlockBits(bmd);
                    b.Close();
                }
                return image;
            }
        }

        public static double[,] LoadStandardImage_Intensity(string Filename, bool Rotate90)
        {
            double[,] ImageArray;
            if (Path.GetExtension(Filename).ToLower() == ".cct")
                ImageArray = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(Load_Raw(Filename), Rotate90);
            else
                ImageArray = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(Load_Bitmap(Filename), Rotate90);

            return ImageArray;
        }

        public static Bitmap FixVisionGateImage(Bitmap VisionGateImage)
        {

            int iWidth = VisionGateImage.Width;
            int iHeight = VisionGateImage.Height;

            Bitmap b2 = new Bitmap(iWidth / 2, iHeight / 2, PixelFormat.Format32bppRgb);
            BitmapData bmd = VisionGateImage.LockBits(new Rectangle(0, 0, VisionGateImage.Width, VisionGateImage.Height), ImageLockMode.ReadOnly, VisionGateImage.PixelFormat);
            BitmapData bmdOut = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height), ImageLockMode.WriteOnly, b2.PixelFormat);

            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    int ccY = 0;
                    for (int y = 0; y < iHeight; y += 2)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                        Int32* scanlineOut = (Int32*)((byte*)bmdOut.Scan0 + (ccY) * bmdOut.Stride);
                        for (int x = 0; x < iWidth; x += 2)
                        {
                            *scanlineOut = *scanline;
                            scanline += 2;
                            scanlineOut++;
                        }
                        ccY++;
                    }

                }
                else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    throw new Exception("not prepared for 24 bit images");
                }
            }
            VisionGateImage.UnlockBits(bmd);
            b2.UnlockBits(bmdOut);

            return b2;
        }

        public static ImageHolder FixVisionGateImage(ImageHolder VisionGateImage)
        {

            int iWidth = VisionGateImage.Width;
            int iHeight = VisionGateImage.Height;

            ImageHolder b2 = new ImageHolder(iWidth / 2, iHeight / 2, VisionGateImage.NChannels);

            float[, ,] dataOut = VisionGateImage.ImageData;
            float[, ,] DataIn = b2.ImageData;

            int ccY = 0;
            for (int chan = 0; chan < VisionGateImage.NChannels; chan++)
            {
                for (int y = 0; y < iHeight; y += 2)
                {

                    for (int x = 0; x < iWidth; x += 2)
                    {
                        DataIn[y / 2, x / 2, chan] = dataOut[y, x, chan];
                    }
                    ccY++;
                }
            }



            return b2;
        }

        /// <summary>
        /// Converts a visiongate bayer pattern image to a normal grayscale
        /// </summary>
        /// <param name="VisionGateImage"></param>
        /// <param name="Channel">0 pull from red, 1 pull from blue, 2 pull from sum of greens, 3- pull from sum of all channels</param>
        /// <returns></returns>
        public static Bitmap FixVisionGateImage(Bitmap VisionGateImage, int Channel)
        {

            int iWidth = VisionGateImage.Width;
            int iHeight = VisionGateImage.Height;

            Bitmap b2 = new Bitmap(iWidth / 2, iHeight / 2, PixelFormat.Format32bppRgb);
            BitmapData bmd = VisionGateImage.LockBits(new Rectangle(0, 0, VisionGateImage.Width, VisionGateImage.Height), ImageLockMode.ReadOnly, VisionGateImage.PixelFormat);
            BitmapData bmdOut = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height), ImageLockMode.WriteOnly, b2.PixelFormat);


            unsafe
            {
                if (Channel == 0)
                {
                    if (bmd.Stride / (double)bmd.Width == 4)
                    {
                        int ccY = 0;
                        for (int y = 1; y < iHeight; y += 2)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                            Int32* scanlineOut = (Int32*)((byte*)bmdOut.Scan0 + (ccY) * bmdOut.Stride);
                            for (int x = 0; x < iWidth; x += 2)
                            {
                                *scanlineOut = *scanline;
                                scanline += 2;
                                scanlineOut++;
                            }
                            ccY++;
                        }

                    }
                    else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                    {
                        throw new Exception("not prepared for 24 bit images");
                    }
                }
                else if (Channel == 1)
                {
                    if (bmd.Stride / (double)bmd.Width == 4)
                    {
                        int ccY = 0;
                        for (int y = 0; y < iHeight; y += 2)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride) + 1;
                            Int32* scanlineOut = (Int32*)((byte*)bmdOut.Scan0 + (ccY) * bmdOut.Stride);
                            for (int x = 0; x < iWidth; x += 2)
                            {
                                *scanlineOut = *scanline;
                                scanline += 2;
                                scanlineOut++;
                            }
                            ccY++;
                        }

                    }
                    else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                    {
                        throw new Exception("not prepared for 24 bit images");
                    }
                }
                else if (Channel == 2)
                {
                    if (bmd.Stride / (double)bmd.Width == 4)
                    {
                        int ccY = 0;
                        for (int y = 0; y < iHeight; y += 2)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                            Int32* scanlineOut = (Int32*)((byte*)bmdOut.Scan0 + (ccY) * bmdOut.Stride);
                            for (int x = 0; x < iWidth; x += 2)
                            {
                                *scanlineOut = *scanline + *(scanline + bmd.Stride / 4 + 1);
                                scanline += 2;
                                scanlineOut++;
                            }
                            ccY++;
                        }

                    }
                    else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                    {
                        throw new Exception("not prepared for 24 bit images");
                    }
                }
                else if (Channel == 3)
                {
                    if (bmd.Stride / (double)bmd.Width == 4)
                    {
                        int ccY = 0;
                        for (int y = 0; y < iHeight; y += 2)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                            Int32* scanlineOut = (Int32*)((byte*)bmdOut.Scan0 + (ccY) * bmdOut.Stride);
                            for (int x = 0; x < iWidth; x += 2)
                            {
                                *scanlineOut = *scanline + *(scanline + 1) + *(scanline + bmd.Stride / 4 + 1) + *(scanline + bmd.Stride / 4);
                                scanline += 2;
                                scanlineOut++;
                            }
                            ccY++;
                        }
                    }
                    else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                    {
                        throw new Exception("not prepared for 24 bit images");
                    }
                }
            }
            VisionGateImage.UnlockBits(bmd);
            b2.UnlockBits(bmdOut);

            return b2;
        }

        /// <summary>
        /// Converts a visiongate bayer pattern image to a normal grayscale
        /// </summary>
        /// <param name="VisionGateImage"></param>
        /// <param name="Channel">0 pull from red, 1 pull from blue, 2 pull from sum of greens, 3- pull from sum of all channels</param>
        /// <returns></returns>
        public static ImageHolder FixVisionGateImage(ImageHolder VisionGateImage, int Channel)
        {

            int iWidth = VisionGateImage.Width;
            int iHeight = VisionGateImage.Height;

            ImageHolder b2 = new ImageHolder(iWidth / 2, iHeight / 2, 1);

            float[, ,] dataOut = VisionGateImage.ImageData;
            float[, ,] DataIn = b2.ImageData;

            if (Channel == 0)
            {
                int ccY = 0;
                for (int y = 1; y < iHeight; y += 2)
                {

                    for (int x = 0; x < iWidth; x += 2)
                    {
                        DataIn[(y - 1) / 2, x / 2, 0] = dataOut[y, x, 0];
                    }
                    ccY++;
                }
            }
            else if (Channel == 1)
            {
                int ccY = 0;
                for (int y = 0; y < iHeight; y += 2)
                {
                    for (int x = 1; x < iWidth; x += 2)
                    {
                        DataIn[y / 2, (x - 1) / 2, 0] = dataOut[y, x, 0];
                    }
                    ccY++;
                }
            }
            else if (Channel == 2)
            {
                int ccY = 0;
                for (int y = 0; y < iHeight; y += 2)
                {
                    for (int x = 0; x < iWidth; x += 2)
                    {
                        DataIn[y / 2, x / 2, 0] = (dataOut[y, x, 0] + dataOut[y + 1, x + 1, 0]) / 2f;
                    }
                    ccY++;
                }
            }
            else if (Channel == 3)
            {
                int ccY = 0;
                for (int y = 0; y < iHeight; y += 2)
                {
                    for (int x = 0; x < iWidth; x += 2)
                    {
                        DataIn[y / 2, x / 2, 0] = (dataOut[y, x, 0] + dataOut[y + 1, x + 1, 0] + dataOut[y + 1, x, 0] + dataOut[y, x + 1, 0]) / 4f;
                    }
                    ccY++;
                }
            }


            return b2;
        }
        #endregion


        public static double[, ,] OpenDensityData(string[] Filenames)
        {
            string Extension = Path.GetExtension(Filenames[0]).ToLower();
            if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                Filenames = MathStringHelps.SortNumberedFiles(Filenames);

                int sizeX, sizeY, sizeZ;

                Bitmap b = new Bitmap(Filenames[0]);
                sizeX = b.Width;
                sizeY = b.Height;
                sizeZ = Filenames.Length;

                double[, ,] mDensityGrid = new double[sizeX, sizeY, sizeZ];

                double XMin = -1;
                double XMax = 1;

                double YMin = -1;
                double YMax = 1;

                double ZMin = -1;
                double ZMax = 1;

                for (int z = 0; z < sizeZ; z++)
                {
                    b = new Bitmap(Filenames[z]);
                    double[,] Data = MathImageHelps.ConvertToDoubleArray(b, false);
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
            else if (Extension == ".ivg")
            {
                Filenames = MathStringHelps.SortNumberedFiles(Filenames);

                int sizeX, sizeY, sizeZ;

                ImageHolder b = MathHelpLib.MathHelpsFileLoader.LoadIVGFile(Filenames[0]);
                sizeX = b.Width;
                sizeY = b.Height;
                sizeZ = Filenames.Length;
                double[, ,] mDensityGrid = new double[sizeX, sizeY, sizeZ];

                double XMin = -1;
                double XMax = 1;

                double YMin = -1;
                double YMax = 1;

                double ZMin = -1;
                double ZMax = 1;


                for (int z = 0; z < sizeZ; z++)
                {
                    b = MathHelpLib.MathHelpsFileLoader.LoadIVGFile(Filenames[z]);
                    // double[,] Data = MathImageHelps. ConvertToDoubleArray(b, false);
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[z, y, x] = b.ImageData[y, x, 0];// Data[y, x];
                        }
                    }
                }


                return mDensityGrid;
            }
            return null;
        }
        public static double[, ,] OpenDensityData(string Filename)
        {
            string Extension = Path.GetExtension(Filename).ToLower();

            if (Path.GetExtension(Filename).ToLower() == ".raw" && File.Exists(Path.Combine(Path.GetDirectoryName(Filename), Path.GetFileNameWithoutExtension(Filename)) + ".dat") == true)
                Filename = Path.Combine(Path.GetDirectoryName(Filename), Path.GetFileNameWithoutExtension(Filename)) + ".dat";

            if (Path.GetExtension(Filename).ToLower() == ".cct")
            {
                #region Open Raw
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);
                int sizeX, sizeY, sizeZ;

                int ArrayRank = Reader.ReadInt32();

                sizeX = Reader.ReadInt32();
                sizeY = Reader.ReadInt32();
                sizeZ = Reader.ReadInt32();


                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();


                double[, ,] mDensityGrid;
                if ((sizeX > 200 || sizeY > 200 || sizeZ > 200) && (IntPtr.Size != 8))
                {
                    mDensityGrid = new double[200, 200, 200];
                    double cX = 199d / sizeX;
                    double cY = 199d / sizeY;
                    double cZ = 199d / sizeZ;

                    for (int z = 0; z < sizeZ; z++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int x = 0; x < sizeX; x++)
                            {
                                mDensityGrid[(int)(z * cZ), (int)(y * cY), (int)(x * cX)] = Reader.ReadDouble();
                            }
                        }
                    }



                }
                else
                {
                    mDensityGrid = new double[sizeZ, sizeY, sizeX];

                    byte[] buffer = Reader.ReadBytes(mDensityGrid.Length * 8);
                    double[] BufD = new double[mDensityGrid.Length];
                    Buffer.BlockCopy(buffer, 0, BufD, 0, mDensityGrid.Length * 8);
                    buffer = null;
                    unsafe
                    {
                        fixed (double* pDouble = BufD)
                        fixed (double* pData = mDensityGrid)
                        {
                            double* pIn = pDouble;
                            double* pOut = pData;
                            for (int i = 0; i < BufD.Length; i++)
                            {
                                *pOut = *pIn;
                                pOut++;
                                pIn++;
                            }

                        }
                    }
                    /*for (int x = 0; x < sizeX; x++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int z = 0; z < sizeZ; z++)
                            {
                                mDensityGrid[z, y, x] = (float)Reader.ReadDouble();
                            }
                        }
                    }*/

                }

                Reader.Close();
                BinaryFile.Close();
                return mDensityGrid;

                #endregion
            }
            else if (Path.GetExtension(Filename).ToLower() == ".dat")
            {
                #region Open dat
                int DataType = 0;
                int sizeX, sizeY, sizeZ;

                sizeX = 0;
                sizeY = 0;
                sizeZ = 0;
                Dictionary<string, string> Tags = new Dictionary<string, string>();

                using (StreamReader sr = new StreamReader(Filename))
                {

                    String line;
                    string[] parts;
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Trim() != "" && line.Trim().StartsWith("#") == false)
                        {
                            parts = line.Replace("\t", "").Split(':');
                            Tags.Add(parts[0].Trim().ToLower(), parts[1]);
                        }
                    }
                    /*  ObjectFileName:	ProjectionObject.raw
                      Resolution:	220 220 220
                      SliceThickness:	1 1 1
                      Format:	FLOAT
                      ObjectModel:	I*/
                    parts = Tags["resolution"].Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    int.TryParse(parts[0], out sizeX);
                    int.TryParse(parts[1], out sizeY);
                    int.TryParse(parts[2], out sizeZ);

                    if (Tags["format"].ToLower() == "float")
                        DataType = 1;
                    if (Tags["format"].ToLower() == "double")
                        DataType = 2;
                    if (Tags["format"].ToLower() == "ushort")
                        DataType = 3;
                }

                Filename = Path.GetDirectoryName(Filename) + "\\" + Tags["objectfilename"];
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);


                double[, ,] mDensityGrid = new double[sizeX, sizeY, sizeZ];


                if (DataType == 2)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int z = 0; z < sizeZ; z++)
                            {
                                mDensityGrid[x, y, z] = Reader.ReadDouble();
                            }
                        }
                    }
                }
                else if (DataType == 1)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int z = 0; z < sizeZ; z++)
                            {
                                mDensityGrid[x, y, z] = Reader.ReadSingle();
                            }
                        }
                    }
                }
                else if (DataType == 3)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int z = 0; z < sizeZ; z++)
                            {
                                mDensityGrid[x, y, z] = (float)Reader.ReadUInt16();
                            }
                        }
                    }
                }


                Reader.Close();
                BinaryFile.Close();
                return mDensityGrid;
                #endregion
            }
            else if (Path.GetExtension(Filename).ToLower() == ".raw")
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

                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int z = 0; z < sizeZ; z++)
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

            return null;

        }



        public static float[, ,] OpenDensityDataFloat(string Filename)
        {
            string Extension = Path.GetExtension(Filename).ToLower();

            if (Path.GetExtension(Filename).ToLower() == ".raw" && File.Exists(Path.Combine(Path.GetDirectoryName(Filename), Path.GetFileNameWithoutExtension(Filename)) + ".dat") == true)
                Filename = Path.Combine(Path.GetDirectoryName(Filename), Path.GetFileNameWithoutExtension(Filename)) + ".dat";

            if (Path.GetExtension(Filename).ToLower() == ".cct")
            {
                #region Open Raw
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);
                int sizeX, sizeY, sizeZ;

                int ArrayRank = Reader.ReadInt32();

                sizeX = Reader.ReadInt32();
                sizeY = Reader.ReadInt32();
                sizeZ = Reader.ReadInt32();


                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();


                float[, ,] mDensityGrid;
                if ((sizeX > 200 || sizeY > 200 || sizeZ > 200) && (IntPtr.Size != 8))
                {
                    mDensityGrid = new float[200, 200, 200];
                    double cX = 199d / sizeX;
                    double cY = 199d / sizeY;
                    double cZ = 199d / sizeZ;

                    for (int z = 0; z < sizeZ; z++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int x = 0; x < sizeX; x++)
                            {
                                mDensityGrid[(int)(z * cZ), (int)(y * cY), (int)(x * cX)] = (float)Reader.ReadDouble();
                            }
                        }
                    }



                }
                else
                {
                    mDensityGrid = new float[sizeZ, sizeY, sizeX];

                    byte[] buffer = Reader.ReadBytes(mDensityGrid.Length * 8);
                    double[] BufD = new double[mDensityGrid.Length];
                    Buffer.BlockCopy(buffer, 0, BufD, 0, mDensityGrid.Length * 8);
                    buffer = null;
                    unsafe
                    {
                        fixed (double* pDouble = BufD)
                        fixed (float* pData = mDensityGrid)
                        {
                            double* pIn = pDouble;
                            float* pOut = pData;
                            for (int i = 0; i < BufD.Length; i++)
                            {
                                *pOut = (float)*pIn;
                                pOut++;
                                pIn++;
                            }

                        }
                    }
                    /*for (int x = 0; x < sizeX; x++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int z = 0; z < sizeZ; z++)
                            {
                                mDensityGrid[z, y, x] = (float)Reader.ReadDouble();
                            }
                        }
                    }*/

                }

                Reader.Close();
                BinaryFile.Close();
                return mDensityGrid;

                #endregion
            }
            else if (Path.GetExtension(Filename).ToLower() == ".dat")
            {
                #region Open dat
                int DataType = 0;
                int sizeX, sizeY, sizeZ;

                sizeX = 0;
                sizeY = 0;
                sizeZ = 0;
                Dictionary<string, string> Tags = new Dictionary<string, string>();

                using (StreamReader sr = new StreamReader(Filename))
                {

                    String line;
                    string[] parts;
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Trim() != "" && line.Trim().StartsWith("#") == false)
                        {
                            parts = line.Replace("\t", "").Split(':');
                            Tags.Add(parts[0].Trim().ToLower(), parts[1]);
                        }
                    }
                    /*  ObjectFileName:	ProjectionObject.raw
                      Resolution:	220 220 220
                      SliceThickness:	1 1 1
                      Format:	FLOAT
                      ObjectModel:	I*/
                    parts = Tags["resolution"].Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    int.TryParse(parts[0], out sizeX);
                    int.TryParse(parts[1], out sizeY);
                    int.TryParse(parts[2], out sizeZ);

                    if (Tags["format"].ToLower() == "float")
                        DataType = 1;
                    if (Tags["format"].ToLower() == "double")
                        DataType = 2;
                    if (Tags["format"].ToLower() == "ushort")
                        DataType = 3;
                }

                Filename = Path.GetDirectoryName(Filename) + "\\" + Tags["objectfilename"];
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);


                float[, ,] mDensityGrid = new float[sizeX, sizeY, sizeZ];


                if (DataType == 2)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int z = 0; z < sizeZ; z++)
                            {
                                mDensityGrid[x, y, z] = (float)Reader.ReadDouble();
                            }
                        }
                    }
                }
                else if (DataType == 1)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int z = 0; z < sizeZ; z++)
                            {
                                mDensityGrid[x, y, z] = Reader.ReadSingle();
                            }
                        }
                    }
                }
                else if (DataType == 3)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int z = 0; z < sizeZ; z++)
                            {
                                mDensityGrid[x, y, z] = (float)Reader.ReadUInt16();
                            }
                        }
                    }
                }


                Reader.Close();
                BinaryFile.Close();
                return mDensityGrid;
                #endregion
            }
            else if (Path.GetExtension(Filename).ToLower() == ".raw")
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

                float[, ,] mDensityGrid = new float[sizeX, sizeY, sizeZ];

                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int z = 0; z < sizeZ; z++)
                        {
                            mDensityGrid[x, y, z] = (float)Reader.ReadDouble();
                        }
                    }
                }
                Reader.Close();
                BinaryFile.Close();
                return mDensityGrid;
                #endregion
            }
            else if (Path.GetExtension(Filename).ToLower() == ".tif")
            {
                return Load_TiffStack(Filename);
            }

            return null;

        }
        public static float[, ,] OpenDensityDataFloat(string[] Filenames)
        {
            string Extension = Path.GetExtension(Filenames[0]).ToLower();
            if (Extension == ".png" || Extension == ".tif" || Extension == ".tiff")
            {
                Filenames = MathStringHelps.SortNumberedFiles(Filenames);

                int sizeX, sizeY, sizeZ;

                ImageHolder b = new ImageHolder  (Filenames[0]);
                sizeX = b.Width;
                sizeY = b.Height;
                sizeZ = Filenames.Length;

                float[, ,] mDensityGrid = new float[sizeZ, sizeY, sizeX];
                //mDensityGrid = new PhysicalArray(sizeX, sizeY, sizeZ, -1, 1, -1, 1, -1, 1);

                for (int z = 0; z < sizeZ; z++)
                {
                    b = new ImageHolder(Filenames[z]);
                    double[,] Data = MathImageHelps.ConvertToDoubleArray(b, false);
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[z, y, x] = (float)Data[y, x];
                        }
                    }
                    b.Dispose();
                    b = null;
                    GC.Collect();
                }
                return mDensityGrid;

            }
            if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                Filenames = MathStringHelps.SortNumberedFiles(Filenames);

                int sizeX, sizeY, sizeZ;

                Bitmap b = new Bitmap(Filenames[0]);
                sizeX = b.Width;
                sizeY = b.Height;
                sizeZ = Filenames.Length;

                float[, ,] mDensityGrid = new float[sizeZ, sizeY, sizeX];
                //mDensityGrid = new PhysicalArray(sizeX, sizeY, sizeZ, -1, 1, -1, 1, -1, 1);

                for (int z = 0; z < sizeZ; z++)
                {
                    b = new Bitmap(Filenames[z]);
                    double[,] Data = MathImageHelps.ConvertToDoubleArray(b, false);
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[z, y, x] = (float)Data[x, y];
                        }
                    }
                    b.Dispose();
                    b = null;
                    GC.Collect();
                }
                return mDensityGrid;
            }
            else if (Extension == ".ivg")
            {
                Filenames = MathStringHelps.SortNumberedFiles(Filenames);

                int sizeX, sizeY, sizeZ;

                ImageHolder b = MathHelpLib.MathHelpsFileLoader.LoadIVGFile(Filenames[0]);
                sizeX = b.Width;
                sizeY = b.Height;
                sizeZ = Filenames.Length;
                float[, ,] mDensityGrid = new float[sizeZ, sizeY, sizeX];

                for (int z = 0; z < sizeZ; z++)
                {
                    b = MathHelpLib.MathHelpsFileLoader.LoadIVGFile(Filenames[z]);
                    // double[,] Data = MathImageHelps. ConvertToDoubleArray(b, false);
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[z, y, x] = b.ImageData[y, x, 0];// Data[y, x];
                        }
                    }
                }


                return mDensityGrid;
            }
            return null;
        }
        public static ushort[, ,] OpenDensityDataInt(string Filename)
        {
            string Extension = Path.GetExtension(Filename).ToLower();
            if (Path.GetExtension(Filename).ToLower() == ".cct")
            {
                #region Open Raw
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);
                int sizeX, sizeY, sizeZ;

                int ArrayRank = Reader.ReadInt32();

                sizeX = Reader.ReadInt32();
                sizeY = Reader.ReadInt32();
                sizeZ = Reader.ReadInt32();


                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();


                ushort[, ,] mDensityGrid;
                if ((sizeX > 200 || sizeY > 200 || sizeZ > 200) && (IntPtr.Size != 8))
                {
                    mDensityGrid = new ushort[200, 200, 200];
                    double cX = 199d / sizeX;
                    double cY = 199d / sizeY;
                    double cZ = 199d / sizeZ;

                    for (int x = 0; x < sizeX; x++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int z = 0; z < sizeZ; z++)
                            {
                                mDensityGrid[(int)(z * cZ), (int)(y * cY), (int)(x * cX)] = (ushort)Reader.ReadDouble();
                            }
                        }
                    }



                }
                else
                {
                    mDensityGrid = new ushort[sizeZ, sizeY, sizeX];

                    byte[] buffer = Reader.ReadBytes(mDensityGrid.Length * 8);
                    double[] BufD = new double[mDensityGrid.Length];
                    Buffer.BlockCopy(buffer, 0, BufD, 0, mDensityGrid.Length * 8);

                    double Max = double.MinValue;
                    double Min = double.MaxValue;


                    buffer = null;
                    unsafe
                    {
                        fixed (double* pDouble = BufD)
                        {
                            double* pIn = pDouble;
                            for (int i = 0; i < BufD.Length; i++)
                            {
                                if (*pIn > Max) Max = *pIn;
                                if (*pIn < Min) Min = *pIn;
                                pIn++;
                            }
                            double Length = (ushort.MaxValue - 1) / (Max - Min);
                            fixed (ushort* pData = mDensityGrid)
                            {
                                pIn = pDouble;
                                ushort* pOut = pData;
                                for (int i = 0; i < BufD.Length; i++)
                                {
                                    *pOut = (ushort)((*pIn - Min) * Length);
                                    pOut++;
                                    pIn++;
                                }

                            }
                        }
                    }
                    /*for (int x = 0; x < sizeX; x++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int z = 0; z < sizeZ; z++)
                            {
                                mDensityGrid[z, y, x] = (float)Reader.ReadDouble();
                            }
                        }
                    }*/

                }

                Reader.Close();
                BinaryFile.Close();
                return mDensityGrid;

                #endregion
            }
            else if (Path.GetExtension(Filename).ToLower() == ".dat")
            {
                #region Open dat
                int DataType = 0;
                int sizeX, sizeY, sizeZ;

                sizeX = 0;
                sizeY = 0;
                sizeZ = 0;
                Dictionary<string, string> Tags = new Dictionary<string, string>();

                using (StreamReader sr = new StreamReader(Filename))
                {

                    String line;
                    string[] parts;
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Trim() != "" && line.Trim().StartsWith("#") == false)
                        {
                            parts = line.Replace("\t", "").Split(':');
                            Tags.Add(parts[0].Trim().ToLower(), parts[1]);
                        }
                    }
                    /*  ObjectFileName:	ProjectionObject.raw
                      Resolution:	220 220 220
                      SliceThickness:	1 1 1
                      Format:	FLOAT
                      ObjectModel:	I*/
                    parts = Tags["resolution"].Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    int.TryParse(parts[0], out sizeX);
                    int.TryParse(parts[1], out sizeY);
                    int.TryParse(parts[2], out sizeZ);

                    if (Tags["format"].ToLower() == "float")
                        DataType = 1;
                    if (Tags["format"].ToLower() == "double")
                        DataType = 2;
                    if (Tags["format"].ToLower() == "ushort")
                        DataType = 3;
                }

                Filename = Path.GetDirectoryName(Filename) + "\\" + Tags["objectfilename"];
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);


                ushort[, ,] mDensityGrid = new ushort[sizeX, sizeY, sizeZ];


                if (DataType == 2)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int x = 0; x < sizeX; x++)
                            {
                                mDensityGrid[x, y, z] = (ushort)Reader.ReadDouble();
                            }
                        }
                    }
                }
                else if (DataType == 1)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int x = 0; x < sizeX; x++)
                            {
                                mDensityGrid[x, y, z] = (ushort)Reader.ReadSingle();
                            }
                        }
                    }
                }
                else if (DataType == 3)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int x = 0; x < sizeX; x++)
                            {
                                mDensityGrid[x, y, z] = Reader.ReadUInt16();
                            }
                        }
                    }
                }


                Reader.Close();
                BinaryFile.Close();
                return mDensityGrid;
                #endregion
            }
            else if (Path.GetExtension(Filename).ToLower() == ".raw")
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

                ushort[, ,] mDensityGrid = new ushort[sizeX, sizeY, sizeZ];

                for (int z = 0; z < sizeZ; z++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[x, y, z] = (ushort)Reader.ReadDouble();
                        }
                    }
                }
                Reader.Close();
                BinaryFile.Close();
                return mDensityGrid;
                #endregion
            }

            return null;

        }



    }
}
