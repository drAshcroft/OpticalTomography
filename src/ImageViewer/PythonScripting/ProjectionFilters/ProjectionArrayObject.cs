using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using ImageViewer;
using MathHelpLib.ImageProcessing;

namespace MathHelpLib.ProjectionFilters
{
    public class ProjectionArrayObject
    {

        public static float[, ,] LoadMultipleFiles(string File0, string File1, int MaxSize)
        {

            float[, ,] outArray = new float[MaxSize, MaxSize, MaxSize];

            float[, ,] Part1 = MathHelpsFileLoader.OpenDensityDataFloat(File0);

            double Convert = (double)(MaxSize - 2) / (double)Part1.GetLength(1);


            int I, J, K;
            for (int i = 0; i < Part1.GetLength(0); i++)
            {
                I = (int)(i * Convert);
                for (int j = 0; j < Part1.GetLength(1); j++)
                {
                    J = (int)(j * Convert);
                    for (int k = 0; k < Part1.GetLength(2); k++)
                    {

                        outArray[I, J, (int)(k * Convert)] = Part1[i, j, k];
                    }
                }
            }

            int offset = Part1.GetLength(0);

            Part1 = null;

            float[, ,] Part2 = MathHelpsFileLoader.OpenDensityDataFloat(File1);

            for (int i = 0; i < Part2.GetLength(0); i++)
            {
                I = (int)((i + offset) * Convert);
                for (int j = 0; j < Part2.GetLength(1); j++)
                {
                    J = (int)(j * Convert);
                    for (int k = 0; k < Part2.GetLength(2); k++)
                    {

                        outArray[I, J, (int)(k * Convert)] = Part2[i, j, k];
                    }
                }
            }


            Part1 = null;
            Part2 = null;
            return outArray;

        }


        public static float[, ,] LoadMultipleFiles(string File0, string File1)
        {
            float[, ,] Part1 = MathHelpsFileLoader.OpenDensityDataFloat(File0);
            float[, ,] Part2 = MathHelpsFileLoader.OpenDensityDataFloat(File1);

            float[, ,] outArray = new float[Part1.GetLength(0) + Part2.GetLength(0), Part1.GetLength(1), Part1.GetLength(2)];

            Buffer.BlockCopy(Part1, 0, outArray, 0, Buffer.ByteLength(Part1));
            Buffer.BlockCopy(Part2, 0, outArray, Buffer.ByteLength(Part1), Buffer.ByteLength(Part2));

            Part1 = null;
            Part2 = null;
            return outArray;

        }

        public double[] impulse = null;
        public ConvolutionMethod DesiredMethod = ConvolutionMethod.Convolution1D;
        public ImageViewer.PythonScripting.Projection.Convolution1D ConvolutionFilter = new ImageViewer.PythonScripting.Projection.Convolution1D();
        public string ReconInterpolationMethod = "Siddon";

        /// <summary>
        /// Contains the data in a format that allows slices to be pulled from the whole and worked on by each thread
        /// data is organised so the first index is the z of the reconstruction, or along the rotation axis
        /// </summary>
        public double[][,] Data;
        public double[, ,] DataWhole;


        /// <summary>
        /// Used to keep all the threads properly locked
        /// </summary>
        public object[] LockArray;

        /// <summary>
        /// Used to help distribute the working threads amoung all the slices.  the lock indicator is used to do a non blocking check if the 
        /// slice is already in use
        /// </summary>
        public bool[] LockIndicator;

        //These are the physical dimensions of the recon object.  They are very useful for properly scaling the projections against the recon
        public double XMin = -1;
        public double XMax = 1;

        public double YMin = -1;
        public double YMax = 1;

        public double ZMin = -1;
        public double ZMax = 1;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="XMin"></param>
        /// <param name="XMax"></param>
        /// <param name="YMin"></param>
        /// <param name="YMax"></param>
        /// <param name="ZMin"></param>
        /// <param name="ZMax"></param>
        public ProjectionArrayObject(double[, ,] Data, double XMin, double XMax, double YMin, double YMax, double ZMin, double ZMax)
        {
            //if (Float == true)
            {
                this.ZStart = 0;
                this.ZEnd = DataWhole.GetLength(0);
                this.ZWhole = DataWhole.GetLength(0);
                DataWhole = Data;
                LockArray = new object[DataWhole.GetLength(0)];

                for (int i = 0; i < DataWhole.GetLength(0); i++)
                {
                    LockArray[i] = new object();
                }

                LockIndicator = new bool[DataWhole.GetLength(0)];

                this.XMin = XMin;
                this.XMax = XMax;

                this.YMin = YMin;
                this.YMax = YMax;

                this.ZMin = ZMin;
                this.ZMax = ZMax;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nCols">number of slices along the z direction</param>
        /// <param name="nRows">number of slices along the y direction</param>
        /// <param name="nSteps">number of slices along the x direction</param>
        /// <param name="XMin"></param>
        /// <param name="XMax"></param>
        /// <param name="YMin"></param>
        /// <param name="YMax"></param>
        /// <param name="ZMin"></param>
        /// <param name="ZMax"></param>
        public ProjectionArrayObject(int nCols, int nRows, int nSteps, double XMin, double XMax, double YMin, double YMax, double ZMin, double ZMax)
        {
            this.ZStart = 0;
            this.ZEnd = nSteps;
            this.ZWhole = nSteps;
            Data = new double[nCols][,];
            LockArray = new object[nCols];

            for (int i = 0; i < nCols; i++)
            {
                Data[i] = new double[nRows, nSteps];
                LockArray[i] = new object();
            }


            LockIndicator = new bool[nCols];

            this.XMin = XMin;
            this.XMax = XMax;

            this.YMin = YMin;
            this.YMax = YMax;

            this.ZMin = ZMin;
            this.ZMax = ZMax;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Float">just a fake parameter, set to true</param>
        /// <param name="nCols">number of slices along the z direction</param>
        /// <param name="nRows">number of slices along the y direction</param>
        /// <param name="nSteps">number of slices along the x direction</param>
        /// <param name="XMin"></param>
        /// <param name="XMax"></param>
        /// <param name="YMin"></param>
        /// <param name="YMax"></param>
        /// <param name="ZMin"></param>
        /// <param name="ZMax"></param>
        public ProjectionArrayObject(bool Float, int nCols, int nRows, int nSteps, double XMin, double XMax, double YMin, double YMax, double ZMin, double ZMax)
        {
            if (Float == true)
            {
                this.ZStart = 0;
                this.ZEnd = nSteps;
                this.ZWhole = nSteps;

                DataWhole = new double[nCols, nRows, nSteps];
                LockArray = new object[nCols];

                for (int i = 0; i < nCols; i++)
                {
                    LockArray[i] = new object();
                }

                LockIndicator = new bool[nCols];

                this.XMin = XMin;
                this.XMax = XMax;

                this.YMin = YMin;
                this.YMax = YMax;

                this.ZMin = ZMin;
                this.ZMax = ZMax;
            }
        }

        public int ZStart;
        public int ZEnd;
        public int ZWhole;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nCols">number of slices along the z direction</param>
        /// <param name="nRows">number of slices along the y direction</param>
        /// <param name="nSteps">number of slices along the x direction</param>
        /// <param name="XMin"></param>
        /// <param name="XMax"></param>
        /// <param name="YMin"></param>
        /// <param name="YMax"></param>
        /// <param name="ZMin"></param>
        /// <param name="ZMax"></param>
        public ProjectionArrayObject(int nCols, int nRows, int nSteps, double XMin, double XMax, double YMin, double YMax, double ZMin, double ZMax, int ZStart, int ZEnd)
        {
            this.ZStart = ZStart;
            this.ZEnd = ZEnd;
            this.ZWhole = nSteps;

            DataWhole = new double[(ZEnd - ZStart), nCols, nRows];
            LockArray = new object[nCols];

            for (int i = 0; i < nCols; i++)
            {
                LockArray[i] = new object();
            }

            LockIndicator = new bool[nCols];

            this.XMin = XMin;
            this.XMax = XMax;

            this.YMin = YMin;
            this.YMax = YMax;

            this.ZMin = ZMin;
            this.ZMax = ZMax;
        }


        /// <summary>
        /// Physical width on the X axis
        /// </summary>
        public double Width
        {
            get
            {
                return XMax - XMin;
            }
        }
        /// <summary>
        /// Physical width on the Y axis
        /// </summary>
        public double Height
        {
            get
            {
                return YMax - YMin;
            }
        }
        /// <summary>
        /// Physical width on the Z axis
        /// </summary>
        public double Depth
        {
            get
            {
                return ZMax - ZMin;
            }
        }

        /// <summary>
        /// Saves 3 images from the volume, using the slices at the center of each axis
        /// </summary>
        /// <param name="Filename"></param>
        public void SaveCross(string Filename)
        {
            if (Data != null)
                SaveCrossJagged(Filename);
            else
                SaveCrossNormal(Filename);
        }

        private void SaveCrossJagged(string Filename)
        {
            string Extension = Path.GetExtension(Filename).ToLower();
            if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                #region Save as Images

                int ZSlices = Data.GetLength(0);
                int XSlices = Data[0].GetLength(0);
                int YSlices = Data[0].GetLength(1);

                string outFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename);


                {
                    double[,] Slice = (double[,])Data[ZSlices / 2];
                    Bitmap b = Slice.MakeBitmap();
                    b.Save(outFile + "_Z" + Extension);
                }

                {
                    double[,] SliceX = new double[XSlices, ZSlices];
                    for (int z = 0; z < ZSlices; z++)
                    {
                        for (int x = 0; x < XSlices; x++)
                        {
                            SliceX[x, z] = Data[z][x, YSlices / 2];
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
                            SliceY[y, z] = Data[z][XSlices / 2, y];
                        }
                    }
                    Bitmap b = SliceY.MakeBitmap();
                    b.Save(outFile + "_Y" + Extension);
                }
                #endregion
            }

        }

        private void SaveCrossNormal(string Filename)
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

        public Bitmap[] ShowCross()
        {
            #region Save as Images

            int ZSlices = DataWhole.GetLength(0);
            int XSlices = DataWhole.GetLength(1);
            int YSlices = DataWhole.GetLength(2);

            Bitmap[] b = new Bitmap[3];

            double[,] Slice = new double[XSlices, YSlices];
            for (int y = 0; y < YSlices; y++)
            {
                for (int x = 0; x < XSlices; x++)
                {
                    Slice[x, y] = DataWhole[ZSlices / 2, x, y];
                }
            }
            b[0] = Slice.MakeBitmap();

            double[,] SliceX = new double[XSlices, ZSlices];
            for (int z = 0; z < ZSlices; z++)
            {
                for (int x = 0; x < XSlices; x++)
                {
                    SliceX[x, z] = DataWhole[z, x, YSlices / 2];
                }
            }
            b[1] = SliceX.MakeBitmap();
            double[,] SliceY = new double[YSlices, ZSlices];
            for (int z = 0; z < ZSlices; z++)
            {
                for (int y = 0; y < YSlices; y++)
                {
                    SliceY[y, z] = DataWhole[z, XSlices / 2, y];
                }
            }
            b[2] = SliceY.MakeBitmap();
            #endregion
            return b;
        }


        /// <summary>
        /// saves the recon volume as either a monolithic file or 
        /// as a stack of images depending on the selected file type
        /// </summary>
        /// <param name="Filename"></param>
        public void SaveFile(string Filename)
        {
            if (Data != null)
                SaveFileJagged(Filename);
            else
                SaveFileNormal(Filename);
        }

        /// <summary>
        /// saves the recon volume as either a monolithic file or 
        /// as a stack of images depending on the selected file type
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="BitDepth">for pngs, and tiffs this specifies the saved file depth</param>
        public void SaveFile(string Filename, int BitDepth)
        {

            if (Data != null)
                SaveFileJagged(Filename, BitDepth);
            else
                SaveFileNormal(Filename, BitDepth);
        }



        /// <summary>
        /// saves the recon volume as either a monolithic file or 
        /// as a stack of images depending on the selected file type
        /// </summary>
        /// <param name="Filename"></param>
        private void SaveFileJagged(string Filename)
        {
            MathHelpsFileLoader.Save_Raw(Filename, Data);
        }

        /// <summary>
        /// saves the recon volume as either a monolithic file or 
        /// as a stack of images depending on the selected file type
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="BitDepth">for pngs, and tiffs this specifies the saved file depth</param>
        private void SaveFileJagged(string Filename, int BitDepth)
        {
            MathHelpsFileLoader.Save_Raw(Filename, Data, BitDepth);
        }

        /// <summary>
        /// saves the recon volume as either a monolithic file or 
        /// as a stack of images depending on the selected file type
        /// </summary>
        /// <param name="Filename"></param>
        private void SaveFileNormal(string Filename)
        {
            MathHelpsFileLoader.Save_Raw(Filename, DataWhole);
        }


        public enum RawFileTypes
        {
            UInt16, Float32
        }

        /// <summary>
        /// saves the recon volume as either a monolithic file or 
        /// as a stack of images depending on the selected file type
        /// </summary>
        /// <param name="Filename"></param>
        public void SaveFileRaw(string Filename, RawFileTypes fileType)
        {
            //todo: make it possible to save stack as a tiff
            string Extension = Path.GetExtension(Filename).ToLower();
            if (Extension == "." || Extension == "")
            {
                Extension = ".raw";
            }

            if (DataWhole != null)
            {
                #region Saving datawhole
                double max1 = DataWhole.MaxArray();
                double min1 = DataWhole.MinArray();

                if (Extension == ".raw")
                {
                    #region SaveRawFile

                    string HeaderFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename) + ".dat";

                    using (StreamWriter outfile = new StreamWriter(HeaderFile))
                    {
                        outfile.WriteLine("ObjectFileName:\t" + Path.GetFileName(Filename));
                        outfile.WriteLine("Resolution:\t" + DataWhole.GetLength(0) + " " + DataWhole.GetLength(1) + " " + DataWhole.GetLength(2));
                        outfile.WriteLine("SliceThickness:\t1 1 1");
                        if (fileType == RawFileTypes.UInt16)
                            outfile.WriteLine("Format:\tUSHORT");
                        else
                            outfile.WriteLine("Format:\tFLOAT");
                        outfile.WriteLine("ObjectModel:\tI");
                    }

                    FileStream BinaryFile = new FileStream(Filename, FileMode.Create, FileAccess.Write);
                    BinaryWriter Writer = new BinaryWriter(BinaryFile);

                    if (fileType == RawFileTypes.Float32)
                    {

                        double length = float.MaxValue / (max1 - min1);
                        for (int z = 0; z < DataWhole.GetLength(0); z++)
                        {
                            for (int y = 0; y < DataWhole.GetLength(1); y++)
                            {
                                for (int x = 0; x < DataWhole.GetLength(2); x++)
                                {
                                    //Writer.Write((float)((DataWhole[z, y, x] - min1) * length));
                                    Writer.Write((float)(DataWhole[z, y, x]));
                                }
                            }
                        }
                    }
                    else
                    {

                        double length = ushort.MaxValue / (max1 - min1);
                        for (int z = 0; z < DataWhole.GetLength(0); z++)
                        {
                            for (int y = 0; y < DataWhole.GetLength(1); y++)
                            {
                                for (int x = 0; x < DataWhole.GetLength(2); x++)
                                {
                                    Writer.Write((ushort)((DataWhole[z, y, x] - min1) * length));
                                }
                            }
                        }
                    }

                    Writer.Close();
                    BinaryFile.Close();
                    #endregion
                }
                else
                    SaveFile(Filename);
                #endregion
            }
            else
            {
                #region Saving jagged array

                double max1 = Data.MaxArray();
                double min1 = Data.MinArray();

                if (Extension == ".raw")
                {
                    #region SaveRawFile

                    string HeaderFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename) + ".dat";

                    using (StreamWriter outfile = new StreamWriter(HeaderFile))
                    {
                        outfile.WriteLine("ObjectFileName:\t" + Path.GetFileName(Filename));
                        outfile.WriteLine("Resolution:\t" + Data[0].GetLength(1) + " " + Data[0].GetLength(0) + " " + Data.Length);
                        outfile.WriteLine("SliceThickness:\t1 1 1");

                        if (fileType == RawFileTypes.UInt16)
                            outfile.WriteLine("Format:\tUSHORT");
                        else
                            outfile.WriteLine("Format:\tFLOAT");

                        outfile.WriteLine("ObjectModel:\tI");
                    }

                    FileStream BinaryFile = new FileStream(Filename, FileMode.Create, FileAccess.Write);
                    BinaryWriter Writer = new BinaryWriter(BinaryFile);

                    if (fileType == RawFileTypes.Float32)
                    {

                        double length = float.MaxValue / (max1 - min1);
                        for (int z = 0; z < Data.Length; z++)
                        {
                            for (int y = 0; y < Data[0].GetLength(0); y++)
                            {
                                for (int x = 0; x < Data[0].GetLength(1); x++)
                                {
                                    Writer.Write((float)((Data[z][y, x] - min1) * length));
                                }
                            }
                        }
                    }
                    else
                    {

                        double length = ushort.MaxValue / (max1 - min1);
                        for (int z = 0; z < Data.Length; z++)
                        {
                            for (int y = 0; y < Data[0].GetLength(0); y++)
                            {
                                for (int x = 0; x < Data[0].GetLength(1); x++)
                                {
                                    Writer.Write((ushort)((Data[z][y, x] - min1) * length));
                                }
                            }
                        }
                    }

                    Writer.Close();
                    BinaryFile.Close();
                    #endregion
                }
                else
                    SaveFile(Filename);
                #endregion

            }

        }

        /// <summary>
        /// saves the recon volume as either a monolithic file or 
        /// as a stack of images depending on the selected file type
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="BitDepth">for pngs, and tiffs this specifies the saved file depth</param>
        private void SaveFileNormal(string Filename, int BitDepth)
        {
            MathHelpsFileLoader.Save_Raw(Filename, DataWhole);
        }

        public static ProjectionArrayObject OpenDensityData(string Filename)
        {
            return new ProjectionArrayObject(MathHelpsFileLoader.OpenDensityData(Filename), -1, 1, -1, 1, -1, 1);
        }

        public static ProjectionArrayObject OpenDensityData(string[] Filenames)
        {
            return new ProjectionArrayObject(MathHelpsFileLoader.OpenDensityData(Filenames), -1, 1, -1, 1, -1, 1);
        }


        public float[, ,] ToFloat()
        {
            float[, ,] outArray = new float[DataWhole.GetLength(0), DataWhole.GetLength(1), DataWhole.GetLength(2)];

            for (int i = 0; i < DataWhole.GetLength(0); i++)
                for (int j = 0; j < DataWhole.GetLength(0); j++)
                    for (int k = 0; k < DataWhole.GetLength(0); k++)
                        outArray[i, j, k] = (float)DataWhole[i, j, k];
            return outArray;

        }


        /// <summary>
        /// Allows two projectionarrayobjects to be added together and the result is saved in this 
        /// object
        /// </summary>
        /// <param name="Adder"></param>
        public void AddInPlace(ProjectionArrayObject Adder)
        {
            unsafe
            {
                unchecked
                {
                    for (int z = 0; z < Data.GetLength(0); z++)
                    {
                        fixed (double* pBase = Data[z])
                        {
                            fixed (double* pAdd = Adder.Data[z])
                            {
                                double* pOut = pBase;
                                double* pIn = pAdd;
                                for (int i = 0; i < Data.Length; i++)
                                {
                                    *pOut += *pIn;
                                    pOut++;
                                    pIn++;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
