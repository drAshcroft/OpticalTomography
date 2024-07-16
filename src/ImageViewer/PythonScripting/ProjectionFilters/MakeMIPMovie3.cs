using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ImageViewer.Filters;
using MathHelpLib;
using System.Runtime.InteropServices;
using MathHelpLib.ImageProcessing;
using System.Threading;
using Emgu.CV;
using System.Threading.Tasks;
using Emgu.CV.Structure;

namespace ImageViewer.PythonScripting.Projection
{
    public class MakeMIPMovie3Effect : aEffectNoForm
    {

        public override string EffectName { get { return "Make MIP Movie"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Projection"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        static object SaveFileLock = new object();


        public static ImageHolder MakeFalseColorMovie(double[, ,] Channel1, double[, ,] Channel2, Color Color1, Color Color2, string TempFolder, string MovieFilename)
        {

            Bitmap MIP;
            //if (dataEnvironment.RunningOnGPU)
            //   MIP = DoMIPProjectionGPU(MovieFilename, tempFilename, DensityGrid);
            //else
            MIP = DoMIPProjection(MovieFilename, TempFolder, Channel1, Channel2);
            return new ImageHolder(MIP);
        }



        /// <summary>
        /// Takes the volume and projects does a maximum intensity projection(MIP) on the desired axis.  The image can then be saved if needed
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">Recon Volume as physicalarray, double[,,] or double[][,];  MovieFilename as string; tempFile Directory as string</param>
        /// <returns>returns imageholder if the image is not saved</returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mPassData = PassData;

            mFilterToken = Parameters;
            string MovieFilename = (string)Parameters[1];
            string tempFilename = (string)Parameters[2];

            ImageHolder OutImage = null;
            if (mFilterToken[0].GetType() == typeof(PhysicalArray))
            {
                PhysicalArray DensityGrid = (PhysicalArray)mFilterToken[0];
                Bitmap MIP;
                if (DensityGrid.GetLength(Axis.XAxis) < 250)
                    MIP = DoMIPProjectionGPU(MovieFilename, tempFilename, DensityGrid.ReferenceDataDouble);
                else
                    MIP = DoMIPProjection(MovieFilename, tempFilename, DensityGrid.ReferenceDataDouble,5)[0];
                OutImage = new ImageHolder(MIP);
            }
            else if (mFilterToken[0].GetType() == typeof(double[, ,]))
            {
                double[, ,] DensityGrid = (double[, ,])mFilterToken[0];
                Bitmap MIP;
                //if (dataEnvironment.RunningOnGPU)
                //   MIP = DoMIPProjectionGPU(MovieFilename, tempFilename, DensityGrid);
                //else
                MIP = DoMIPProjection(MovieFilename, tempFilename, DensityGrid,5)[0];
                OutImage = new ImageHolder(MIP);
            }
            else if (mFilterToken[0].GetType() == typeof(float[, ,]))
            {
                float[, ,] DensityGrid = (float[, ,])mFilterToken[0];
                Bitmap MIP;
                //if (dataEnvironment.RunningOnGPU)
                //   MIP = DoMIPProjectionGPU(MovieFilename, tempFilename, DensityGrid);
                //else
                MIP = DoMIPProjection(MovieFilename, tempFilename, DensityGrid, 5)[0];
                OutImage = new ImageHolder(MIP);
            }
            else if (mFilterToken[0].GetType() == typeof(double[][,]))
            {
                double[][,] DensityGrid = (double[][,])mFilterToken[0];
                Bitmap MIP = DoMIPProjection(MovieFilename, tempFilename, DensityGrid);
                OutImage = new ImageHolder(MIP);
            }


            // ImagingTools.CreateMovieFFMPEG(MovieFilename, tempFilename + "mip\\mip", "jpg");
            return OutImage;
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Convolution|PhysicalArray", "SliceAngle|double(Radians)", "optional OutFilename|string" }; }
        }

        [DllImport("VolumeOut.dll")]
        private static unsafe extern IntPtr CreateVolumeOut(string KernalName, string ConvolutionCode, string FlagStr,
            double* Image, int ImageWidth, int ImageHeight, double* Cube, int CubeWidth, int CubeHeight, int CubeDepth);

        [DllImport("VolumeOut.dll")]
        private static unsafe extern int RunVolumeOut(IntPtr Projector, double* Image, float uX, float uY, float uZ, float aX, float aY, float aZ);

        [DllImport("VolumeOut.dll")]
        private static unsafe extern int getVolumeOutData(IntPtr Projector);

        [DllImport("VolumeOut.dll")]
        private static unsafe extern int CloseVolumeOut(IntPtr Projector);

        #region MIPProgram
        static string MIPProgram = @"
#pragma OPENCL EXTENSION cl_amd_fp64 : enable 

double cubicInterpolate (double p[4], double u) {
	return p[1] + 0.5 * u*(p[2] - p[0] + u*(2.0*p[0] - 5.0*p[1] + 4.0*p[2] - p[3] + u*(3.0*(p[1] - p[2]) + p[3] - p[0])));
}

double bicubicInterpolate (double p[4][4], double uX, double uY) {
	double arr[4];
	arr[0] = cubicInterpolate(p[0], uY);
	arr[1] = cubicInterpolate(p[1], uY);
	arr[2] = cubicInterpolate(p[2],uY);
	arr[3] = cubicInterpolate(p[3],uY);
	return cubicInterpolate(arr, uX);
}

__kernel void simpleFBP( __global  double * volume,
                         __global  double * input,
                         const     uint2  inputDimensions,
                         const     uint4  cubeDimensions,
						 const     float4 UP,
                         const     float4 Across
                         )
 {

            uint iWidth = inputDimensions.x;
            uint iHeight = inputDimensions.y;

            int hiWidth = (int)(iWidth / 2);
            int hiHeight = (int)(iHeight / 2);

            uint cWidth = cubeDimensions.x;
            uint cHeight = cubeDimensions.y;
            uint cDepth = cubeDimensions.y;

            int hcWidth = (int)(cWidth / 2);
            int hcHeight = (int)(cHeight / 2);
            int hcDepth = (int)(cDepth / 2);

            uint SliceSize = cHeight * cDepth;

            uint xI =( get_global_id(0));
            uint yI =(  get_global_id(1));
            uint zI =(  get_global_id(2));

            uint cTmp = xI + yI * cWidth + zI * SliceSize;
            uint iTmp00;

            int xII =(int)xI - hcWidth;
            int yII =(int) yI - hcHeight;
            int zII = (int)zI - hcDepth;

            double  x, y;
            
            //calculate the position of the current voxel on the image
            

            x = UP.x * xII + UP.y *yII +UP.z *zII  + hiWidth ;
            y = Across.x * xII + Across.y * yII + Across.z * zII+ hiHeight ;

             if (x>0 && x<iWidth)
             if (y>0 && y<iHeight)
            {
                x = round(x);
                y = round(y);

                if (x>0 && x<iWidth-1)
                if (y>0 && y<iHeight-1)
                {
					iTmp00 = (uint)( x + y * iWidth);
                    V1 = input[iTmp00];
                    if (  volume[cTmp]<V1) volume[cTmp]=V1;
                }
            }

}

";

        #endregion
        bool ProgramBuild = false;

        static object CriticalSectionLock = new object();

        #region GPU
        /// <summary>
        /// Takes a recon volume and does a MIP projection from the desired axis around the Z axis
        /// </summary>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians"></param>
        /// <returns></returns>
        public unsafe Bitmap DoMIPProjectionGPU(string MovieFilename, string tempFolder, double[, ,] DensityGrid)
        {
            lock (CriticalSectionLock)
            {
                if (Directory.Exists(tempFolder + "\\MIP\\") == false)
                    Directory.CreateDirectory(tempFolder + "\\MIP\\");
                else
                {
                    Directory.Delete(tempFolder + "\\MIP\\", true);
                    Directory.CreateDirectory(tempFolder + "\\MIP\\");
                }

                int CubeX, CubeY, CubeZ;
                //calculate the direction vectors needed to do the mip, these are defaults that assume that the rotation should be around the z axis and the fast scan axis is the one that matters
                CubeX = DensityGrid.GetLength(0);
                CubeY = DensityGrid.GetLength(1);
                CubeZ = DensityGrid.GetLength(2);

                // FreeImageAPI.FreeImageBitmap fib = null;
                Bitmap b = null;
                double[,] Slice = new double[CubeX, CubeZ];
                fixed (double* pImage = Slice)
                {
                    IntPtr Program = IntPtr.Zero;
                    GCHandle CubeHandle;
                    IntPtr CubeAddress;
                    long CubeLength;
                    double[, ,] DataCube;

                    if (ProgramBuild == false)
                    {
                        DataCube = DensityGrid;
                        CubeHandle = GCHandle.Alloc(DataCube, GCHandleType.Pinned);
                        CubeAddress = CubeHandle.AddrOfPinnedObject();
                        CubeLength = DataCube.Length;

                        Program = CreateVolumeOut("MIPProjection", MIPProgram, "", pImage, Slice.GetLength(0), Slice.GetLength(1), (double*)CubeAddress, CubeX, CubeY, CubeZ);

                        int cc = 0;
                        for (double i = 0; i < 360; i += 5)
                        {
                            double Radians = (double)i / 180d * Math.PI;
                            DoBackProjection_OneSlice(Program, Slice, 1, 1, DataCube, Radians, Axis2D.YAxis);
                            getVolumeOutData(Program);
                            b = Slice.MakeBitmap();
                            // fib = new FreeImageAPI.FreeImageBitmap();
                            // fib.Save(tempFolder + "\\MIP\\MIP" + string.Format("{0:000}", cc) + ".jpg");
                            cc++;
                        }
                    }
                    CloseVolumeOut(Program);
                }

                return b;
            }
        }

        /// <summary>
        /// calculates the needed vectors to turn the requested angle into projection vectors
        /// </summary>
        /// <param name="Slice"></param>
        /// <param name="PaintingWidth">Physical dimension of the pseudo projection (vs the recon size)</param>
        /// <param name="PaintingHeight">Physical dimension of the pseudo projection (vs the recon size)</param>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians">projection angle in radians</param>
        /// <param name="ConvolutionAxis">Usually set to YAxis. This is the direction that carries the rotation, or the fast axis for the convolution</param>
        private static void DoBackProjection_OneSlice(IntPtr Program, double[,] Slice, double PaintingWidth, double PaintingHeight, double[, ,] DensityGrid, double AngleRadians, Axis2D ConvolutionAxis)
        {
            Axis RotationAxis = Axis.ZAxis;

            Point3D vRotationAxis = new Point3D();
            Point3D axis = new Point3D();

            if (RotationAxis == Axis.XAxis)
            {
                vRotationAxis = new Point3D(1, 0, 0);
                axis = new Point3D(0, 1, 0);
            }
            else if (RotationAxis == Axis.YAxis)
            {
                vRotationAxis = new Point3D(0, 1, 0);
                axis = new Point3D(0, 0, 1);
            }
            else if (RotationAxis == Axis.ZAxis)
            {
                vRotationAxis = new Point3D(0, 0, 1);
                axis = new Point3D(0, 1, 0);
            }

            double angle = AngleRadians;

            Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, angle);

            SmearArray2D(Program, DensityGrid, 2, 2, Slice, PaintingWidth, PaintingHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));
        }

        private static unsafe void SmearArray2D(IntPtr Program, double[, ,] DensityGrid, float GridWidth, float GridHeight, double[,] PaintingArray, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
        {
            //used to keep track of which slices have finished.  this allows the threads to all work on different arrays of the projection object
            bool[] SliceFinished = new bool[DensityGrid.GetLength(0)];

            //make sure all the vectors are the right size
            Direction.Normalize();
            FastScanDirection.Normalize();
            //determine all the important planes in the 3D space and all the step
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            ///get the existing recon data.  the jagged array is used so the data can be checked out by each thread
            double[, ,] mDataDouble = DensityGrid;

            //get all the dimensions
            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LZ = mDataDouble.GetLength(0);

            int LOutX = PaintingArray.GetLength(1);
            int LOutY = PaintingArray.GetLength(0);

            float sX = -1 * GridWidth / 2f;// mPhysicalStart[2];
            float sY = -1 * GridWidth / 2f;// mPhysicalStart[1];
            float sZ = -1 * GridHeight / 2f;// mPhysicalStart[0];

            float sOutX = (float)(PaintingWidth / -2f); //PaintingArray.PhysicalStart(Axis.XAxis);
            float sOutY = (float)(PaintingHeight / -2f); //PaintingArray.PhysicalStart(Axis.YAxis);

            float stepOutX = (float)(PaintingWidth / PaintingArray.GetLength(1)); //PaintingArray.PhysicalStep(Axis.XAxis);
            float stepOutY = (float)(PaintingHeight / PaintingArray.GetLength(0)); //PaintingArray.PhysicalStep(Axis.YAxis);

            float stepX = 2f / LX;// mPhysicalStep[2];
            float stepY = 2f / LY;//mPhysicalStep[1];
            float stepZ = 2f / LZ;// mPhysicalStep[0];

            float bXY, bXX, bYY, bYX;
            float bXZ, bYZ;

            //a lot of the coeffiences can be calculated outside the loop saving time and computation
            bXX = (float)(normalX.Y * stepY / stepOutX);
            bXY = (float)(normalX.X * stepX / stepOutX);
            bXZ = (float)(normalX.Z * stepZ / stepOutX);

            bYX = (float)(normalY.Y * stepY / stepOutY);
            bYY = (float)(normalY.X * stepX / stepOutY);
            bYZ = (float)(normalY.Z * stepX / stepOutY);


            fixed (double* pImage = PaintingArray)
            {
                RunVolumeOut(Program, pImage, bXX, bXY, bXZ, bYX, bYY, bYZ);
            }

            mDataDouble = null;

        }
        #endregion

        public unsafe static Bitmap[] DoMIPProjection(string MovieFilename, string tempFolder, float [, ,] DensityGrid, double AngleStepSize)
        {
            lock (CriticalSectionLock)
            {
                if (tempFolder != "")
                {
                    if (Directory.Exists(tempFolder + "\\MIP\\") == false)
                        Directory.CreateDirectory(tempFolder + "\\MIP\\");
                    else
                    {
                        //   Directory.Delete(tempFolder + "\\MIP\\", true);
                        //   Directory.CreateDirectory(tempFolder + "\\MIP\\");
                    }
                }

                int CubeX, CubeY, CubeZ;
                //calculate the direction vectors needed to do the mip, these are defaults that assume that the rotation should be around the z axis and the fast scan axis is the one that matters
                CubeX = DensityGrid.GetLength(0);
                CubeY = DensityGrid.GetLength(1);
                CubeZ = DensityGrid.GetLength(2);



                int cc = 0;
                List<Thread> Threads = new List<Thread>();
                // int AngleStepSize = 5;
                object FibLock = new object();


                Bitmap[] Images = new Bitmap[(int)(360d / AngleStepSize)];

                for (int i = 0; i < Images.Length; i++)
                {
                    Threads.Add(new Thread(delegate(object Vars)
                    {
                        double[,] Slice;
                        // FreeImageAPI.FreeImageBitmap fib = null;
                        int index = (int)(Vars);
                        Console.WriteLine(index + "  " + index);
                        double Radians = 2 * Math.PI - (double)index * AngleStepSize / 180d * Math.PI;
                        Slice = MakeMIPProjectionEffect.DoMIPProjection_OneSlice(DensityGrid, Radians);

                        Images[index] = Slice.MakeBitmap24FlipHorizonal();
                        /*                    lock (FibLock)
                                            {
                                                fib = new FreeImageAPI.FreeImageBitmap(Slice.MakeBitmap());
                                                fib.Save(tempFolder + "\\MIP\\MIP" + string.Format("{0:000}", index) + ".jpg");
                                            }*/
                    }
                    )
                    );
                }


                Thread CleanUp = new Thread(delegate()
                {
                    int Index = 0;
                    VideoWriter VW = null;
                    int nWidth = 100;
                    while (Index < Images.Length)
                    {
                        while (Images[Index] == null)
                        {
                            Thread.Sleep(100);
                        }

                        if (Index == 0)
                        {
                            nWidth = (int)(16 * (Math.Floor((double)Images[Index].Width / 16d) + 1));
                            VW = new VideoWriter(MovieFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 33, nWidth, Images[Index].Height, true);
                        }

                        Thread.Sleep(100);

                        Bitmap temp = new Bitmap(nWidth, Images[Index].Height, PixelFormat.Format32bppRgb);
                        Graphics g = Graphics.FromImage(temp);

                        g.DrawImage(Images[Index], Point.Empty);

                        var frame = new Emgu.CV.Image<Bgr, byte>(temp);

                        VW.WriteFrame<Bgr, byte>(frame);

                        Index++;
                    }

                    VW.Dispose();
                });

                for (int i = 0; i < Threads.Count; i++)
                {
                    Threads[i].Start(i);
                }

                if (MovieFilename != "")
                    CleanUp.Start();

                foreach (Thread t in Threads)
                    t.Join();

                if (MovieFilename != "")
                    CleanUp.Join();


                //double[,] SliceO;
                //double RadiansO = 0;
                //SliceO = MakeMIPProjectionEffect.DoMIPProjection_OneSlice(DensityGrid, RadiansO);
                return Images;
            }
        }


        public unsafe static Bitmap[] DoMIPProjection(string MovieFilename, string tempFolder, double[, ,] DensityGrid, double AngleStepSize)
        {
            lock (CriticalSectionLock)
            {
                if (tempFolder != "")
                {
                    if (Directory.Exists(tempFolder + "\\MIP\\") == false)
                        Directory.CreateDirectory(tempFolder + "\\MIP\\");
                    else
                    {
                        //   Directory.Delete(tempFolder + "\\MIP\\", true);
                        //   Directory.CreateDirectory(tempFolder + "\\MIP\\");
                    }
                }

                int CubeX, CubeY, CubeZ;
                //calculate the direction vectors needed to do the mip, these are defaults that assume that the rotation should be around the z axis and the fast scan axis is the one that matters
                CubeX = DensityGrid.GetLength(0);
                CubeY = DensityGrid.GetLength(1);
                CubeZ = DensityGrid.GetLength(2);



                int cc = 0;
                List<Thread> Threads = new List<Thread>();
               // int AngleStepSize = 5;
                object FibLock = new object();


                Bitmap[] Images = new Bitmap[(int)(360d / AngleStepSize)];

                for (int i = 0; i < Images.Length;i++ )
                {
                    Threads.Add(new Thread(delegate(object Vars)
                {
                    double[,] Slice;
                    // FreeImageAPI.FreeImageBitmap fib = null;
                    int index = (int)(Vars);
                    Console.WriteLine(index  + "  " + index);
                    double Radians = 2*Math.PI- (double)index *AngleStepSize/180d*Math.PI;
                    Slice = MakeMIPProjectionEffect.DoMIPProjection_OneSlice(DensityGrid, Radians);

                    Images[index] = Slice.MakeBitmap24FlipHorizonal();
                    /*                    lock (FibLock)
                                        {
                                            fib = new FreeImageAPI.FreeImageBitmap(Slice.MakeBitmap());
                                            fib.Save(tempFolder + "\\MIP\\MIP" + string.Format("{0:000}", index) + ".jpg");
                                        }*/
                }
                    )
                    );
                }


                Thread CleanUp = new Thread(delegate()
                    {
                        int Index = 0;
                        VideoWriter VW = null;
                        int nWidth=100;
                        while (Index < Images.Length)
                        {
                            while (Images[Index] == null)
                            {
                                Thread.Sleep(100);
                            }

                            if (Index == 0)
                            {
                                nWidth =  (int)(16 * (Math.Floor((double)Images[Index].Width / 16d) + 1));
                                VW = new VideoWriter(MovieFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 33, nWidth, Images[Index].Height, true);
                            }

                            Thread.Sleep(100);

                            Bitmap temp = new Bitmap(nWidth, Images[Index].Height, PixelFormat.Format32bppRgb);
                            Graphics g = Graphics.FromImage(temp);

                            g.DrawImage(Images[Index], Point.Empty);

                            var frame = new Emgu.CV.Image<Bgr, byte>(temp);

                            VW.WriteFrame<Bgr, byte>(frame);

                            Index++;
                        }

                        VW.Dispose();
                    });

                for (int  i = 0; i < Threads.Count;i++ )
                {
                    Threads[i].Start(i);
                }

                if (MovieFilename !="")
                    CleanUp.Start();

                foreach (Thread t in Threads)
                    t.Join();

                if (MovieFilename != "")
                    CleanUp.Join();


                //double[,] SliceO;
                //double RadiansO = 0;
                //SliceO = MakeMIPProjectionEffect.DoMIPProjection_OneSlice(DensityGrid, RadiansO);
                return Images;
            }
        }

        public unsafe static Bitmap[] DoForwardProjection(string MovieFilename, string tempFolder, double[, ,] DensityGrid, double AngleStepSize)
        {
            lock (CriticalSectionLock)
            {
                if (tempFolder != "")
                {
                    if (Directory.Exists(tempFolder + "\\MIP\\") == false)
                        Directory.CreateDirectory(tempFolder + "\\MIP\\");
                    else
                    {
                        //   Directory.Delete(tempFolder + "\\MIP\\", true);
                        //   Directory.CreateDirectory(tempFolder + "\\MIP\\");
                    }
                }

                int CubeX, CubeY, CubeZ;
                //calculate the direction vectors needed to do the mip, these are defaults that assume that the rotation should be around the z axis and the fast scan axis is the one that matters
                CubeX = DensityGrid.GetLength(0);
                CubeY = DensityGrid.GetLength(1);
                CubeZ = DensityGrid.GetLength(2);



                int cc = 0;
                List<Thread> Threads = new List<Thread>();
                // int AngleStepSize = 5;
                object FibLock = new object();


                Bitmap[] Images = new Bitmap[(int)(360d / AngleStepSize)];

                for (int i = 0; i < Images.Length; i++)
                {
                    Threads.Add(new Thread(delegate(object Vars)
                    {
                        double[,] Slice;
                        // FreeImageAPI.FreeImageBitmap fib = null;
                        int index = (int)(Vars);
                        Console.WriteLine(index + "  " + index);
                        double Radians = 2 * Math.PI - (double)index * AngleStepSize / 180d * Math.PI;
                        Slice = MakeMIPProjectionEffect.DoForwardProjection_OneSlice (DensityGrid, Radians);

                        Images[index] = Slice.MakeBitmap24FlipHorizonal();
                        /*                    lock (FibLock)
                                            {
                                                fib = new FreeImageAPI.FreeImageBitmap(Slice.MakeBitmap());
                                                fib.Save(tempFolder + "\\MIP\\MIP" + string.Format("{0:000}", index) + ".jpg");
                                            }*/
                    }
                    )
                    );
                }


                Thread CleanUp = new Thread(delegate()
                {
                    int Index = 0;
                    VideoWriter VW = null;
                    int nWidth = 100;
                    while (Index < Images.Length)
                    {
                        while (Images[Index] == null)
                        {
                            Thread.Sleep(100);
                        }

                        if (Index == 0)
                        {
                            nWidth = (int)(16 * (Math.Floor((double)Images[Index].Width / 16d) + 1));
                            VW = new VideoWriter(MovieFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 33, nWidth, Images[Index].Height, true);
                        }

                        Thread.Sleep(100);

                        Bitmap temp = new Bitmap(nWidth, Images[Index].Height, PixelFormat.Format32bppRgb);
                        Graphics g = Graphics.FromImage(temp);

                        g.DrawImage(Images[Index], Point.Empty);

                        var frame = new Emgu.CV.Image<Bgr, byte>(temp);

                        VW.WriteFrame<Bgr, byte>(frame);

                        Index++;
                    }

                    VW.Dispose();
                });

                for (int i = 0; i < Threads.Count; i++)
                {
                    Threads[i].Start(i);
                }

                if (MovieFilename != "")
                    CleanUp.Start();

                foreach (Thread t in Threads)
                    t.Join();

                if (MovieFilename != "")
                    CleanUp.Join();


                //double[,] SliceO;
                //double RadiansO = 0;
                //SliceO = MakeMIPProjectionEffect.DoMIPProjection_OneSlice(DensityGrid, RadiansO);
                return Images;
            }
        }


        private class WorkToken
        {
            public double[, ,] Data;
            public Bitmap[] Images;
            public int ImageIndex;
            public double AngleStepSize;
            public int OutputOffset;

            public WorkToken(double[, ,] data, Bitmap[] Images, int ImageIndex, double AngleStepSize, int OutputImageOffset)
            {
                this.Data = data;
                this.Images = Images;
                this.ImageIndex = ImageIndex;
                this.AngleStepSize = AngleStepSize;
                this.OutputOffset = OutputImageOffset;
            }
        }

        private class MergeToken
        {
            public Bitmap[] Images;
            public int ImageIndex;
            public int nAngles;
            public MergeToken(Bitmap[] images, int imageIndex, int nAngles)
            {
                Images = images;
                ImageIndex = imageIndex;
                this.nAngles = nAngles;
            }
        }

        private static void GetMip(object workToken)
        {
            WorkToken token = (WorkToken)workToken;


            int j = token.ImageIndex;
            //  int index = (int)(j / token.AngleStepSize);

            double Radians = (double)(j * token.AngleStepSize) / 180d * Math.PI;
            double[,] Slice1 = MakeMIPProjectionEffect.DoMIPProjection_OneSlice(token.Data, Radians);


            Bitmap b1 = Slice1.MakeBitmapCorner();

            token.Images[j + token.OutputOffset] = new Bitmap(b1);
        }

        private static void JoinImages(object mergeToken)
        {
            try
            {
                MergeToken mt = (MergeToken)mergeToken;
                int index = mt.ImageIndex;
                Bitmap[] Images = mt.Images;

                Images[index + mt.nAngles * 2] = MathImageHelps.MergeBitmaps(Images[index + mt.nAngles], Images[index], 0, 0);
            }
            catch { }
        }

        public unsafe static Bitmap DoMIPProjection(string MovieFilename, string tempFolder, double[, ,] DensityGrid, double[, ,] Grid2)
        {
            lock (CriticalSectionLock)
            {
                if (Directory.Exists(tempFolder + "\\MIP\\") == false)
                    Directory.CreateDirectory(tempFolder + "\\MIP\\");
                else
                {
                    //   Directory.Delete(tempFolder + "\\MIP\\", true);
                    //   Directory.CreateDirectory(tempFolder + "\\MIP\\");
                }

                int CubeX, CubeY, CubeZ;
                //calculate the direction vectors needed to do the mip, these are defaults that assume that the rotation should be around the z axis and the fast scan axis is the one that matters
                CubeX = DensityGrid.GetLength(0);
                CubeY = DensityGrid.GetLength(1);
                CubeZ = DensityGrid.GetLength(2);

                int cc = 0;
                List<Thread> Threads = new List<Thread>();
                double AngleStepSize = 5;
                object FibLock = new object();

                int nAngles = (int)(360d / (double)AngleStepSize);
                Bitmap[] Images = new Bitmap[nAngles * 3];

                WorkToken[] wtokens = new WorkToken[nAngles];
                WorkToken[] wTokens2 = new WorkToken[nAngles];
                MergeToken[] mergeTokens = new MergeToken[nAngles];
                for (int i = 0; i < nAngles; i++)
                {
                    wtokens[i] = new WorkToken(DensityGrid, Images, i, AngleStepSize, 0);

                    wTokens2[i] = new WorkToken(Grid2, Images, i, AngleStepSize, nAngles);

                    mergeTokens[i] = new MergeToken(Images, i, nAngles);
                }


                Thread CleanUp = new Thread(delegate()
                {
                    int Index = 0;
                    VideoWriter VW = null;

                    while (Index < Images.Length)
                    {
                        while (Images[Index] == null)
                        {
                            Thread.Sleep(100);
                        }

                        if (Index == 0)
                            VW = new VideoWriter(MovieFilename, CvInvoke.CV_FOURCC('M', 'J', 'P', 'G'), 33, Images[Index].Height, Images[Index].Width, true);

                        Thread.Sleep(100);

                        var frame = new Emgu.CV.Image<Bgra, byte>(Images[Index]);

                        VW.WriteFrame<Bgra, byte>(frame);


                        Index++;
                    }

                    VW.Dispose();
                });


                CleanUp.Start();
                Parallel.ForEach<WorkToken>(wTokens2, token => GetMip(token));

                Parallel.ForEach<WorkToken>(wtokens, token => GetMip(token));

                Parallel.ForEach<MergeToken>(mergeTokens, token => JoinImages(token));

                CleanUp.Join();


                double[,] SliceO;
                double RadiansO = 0;
                SliceO = MakeMIPProjectionEffect.DoMIPProjection_OneSlice(DensityGrid, RadiansO);
                return SliceO.MakeBitmap();
            }
        }

        public unsafe Bitmap DoMIPProjection(string MovieFilename, string tempFolder, PhysicalArray DensityGrid)
        {
            lock (CriticalSectionLock)
            {
                if (Directory.Exists(tempFolder + "\\MIP\\") == false)
                    Directory.CreateDirectory(tempFolder + "\\MIP\\");
                else
                {
                    Directory.Delete(tempFolder + "\\MIP\\", true);
                    Directory.CreateDirectory(tempFolder + "\\MIP\\");
                }

                double[,] Slice;
                Bitmap b = null;
                int cc = 0;
                for (double i = 0; i < 360; i += 5)
                {
                    double Radians = (double)i / 180d * Math.PI;
                    Slice = (double[,])MakeMIPProjectionEffect.DoMIPProjection_OneSlice(DensityGrid, Radians).ActualData2D;

                    b = Slice.MakeBitmap();
                    b.Save(tempFolder + "\\MIP\\MIP" + string.Format("{0:000}", cc) + ".jpg");
                    cc++;
                }

                return b;
            }
        }

        public unsafe Bitmap DoMIPProjection(string MovieFilename, string tempFolder, double[][,] DensityGrid)
        {
            lock (CriticalSectionLock)
            {
                if (Directory.Exists(tempFolder + "\\MIP\\") == false)
                    Directory.CreateDirectory(tempFolder + "\\MIP\\");
                else
                {
                    Directory.Delete(tempFolder + "\\MIP\\", true);
                    Directory.CreateDirectory(tempFolder + "\\MIP\\");
                }


                double[,] Slice = null;

                Thread[] AllThreads = new Thread[360 / 5];
                Bitmap[] AllFrames = new Bitmap[AllThreads.Length];

                for (int i = 0; i < AllThreads.Length; i++)
                {
                    AllThreads[i] = new Thread(delegate(object Vars)
               {
                   int index = (int)Vars;
                   double Radians = (i * 5d) / 180d * Math.PI;
                   Slice = MakeMIPProjectionEffect.DoMIPProjection_OneSlice(DensityGrid, Radians);

                   AllFrames[index] = (Slice.MakeBitmap());
               });
                    AllThreads[i].Start(i);

                }

                foreach (Thread t in AllThreads)
                    t.Join();

                try
                {
                    ImagingTools.CreateAVIVideo(MovieFilename, AllFrames);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                foreach (Bitmap b in AllFrames)
                    b.Dispose();

                return Slice.MakeBitmap();
            }
        }

    }
}