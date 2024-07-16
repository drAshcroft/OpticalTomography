using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MathHelpLib.ProjectionFilters;
using MathHelpLib;

namespace ImageTestor
{
    public partial class FormGPU : Form
    {
        public FormGPU()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Show();
            Application.DoEvents();
        }

        [DllImport("ProjectFBPImage.dll")]
        private static unsafe extern IntPtr CreateFBP(string KernalName, string ConvolutionCode, string FlagStr,
            double* Image, int ImageWidth, int ImageHeight, double* Cube, int CubeWidth, int CubeHeight, int CubeDepth);

        [DllImport("ProjectFBPImage.dll")]
        private static unsafe extern int RunFBP(IntPtr Projector, double* Image, float uX, float uY, float uZ, float aX, float aY, float aZ);

        [DllImport("ProjectFBPImage.dll")]
        private static unsafe extern int getFBPData(IntPtr Projector);

        [DllImport("ProjectFBPImage.dll")]
        private static unsafe extern int CloseFBP(IntPtr Projector);

        #region FBPCode
        string FBPcode = @"
#pragma OPENCL EXTENSION cl_amd_fp64 : enable 
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
            uint iTmp;

            int xII =(int)xI - hcWidth;
            int yII =(int) yI - hcHeight;
            int zII = (int)zI - hcDepth;

            float  x, y;
            
            //calculate the position of the current voxel on the image
            

            x = UP.x * xII + UP.y *yII +UP.z *zII  + hiWidth ;
            y = Across.x * xII + Across.y * yII + Across.z * zII+ hiHeight ;

             if (x>0 && x<iWidth)
             if (y>0 && y<iHeight)
            {
                 y = round(y);
                 x = round(x);

                if (x>0 && x<iWidth)
                if (y>0 && y<iHeight)
                {
                    iTmp =(uint)( x + y * iWidth);
                    volume[cTmp] +=  input[iTmp];
                }
            }
volume[cTmp]=3000;
}
";
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            double[, ,] data = new double[250, 250, 250];


            double[,] bImage = new double[data.GetLength(0), data.GetLength(1)];
            for (int i = 0; i < bImage.GetLength(0); i++)
                for (int j = 0; j < bImage.GetLength(1); j++)
                    if (i > bImage.GetLength(0) * .25 && i < bImage.GetLength(0) * .75)
                        if (j > bImage.GetLength(1) * .25 && j < bImage.GetLength(1) * .75)
                            bImage[i, j] = 10;

            unsafe
            {
                fixed (double* pDAta = data)
                {
                    fixed (double* pImage = bImage)
                    {
                        Proj = CreateFBP("simpleFBP", FBPcode, " ", pImage, bImage.GetLength(0), bImage.GetLength(1), pDAta, data.GetLength(2), data.GetLength(1), data.GetLength(0));

                        if (Proj == IntPtr.Zero)
                            throw new Exception("No working");

                        for (int i = 0; i < 500; i += 5)
                            DoBackProjection_OneSlice(bImage, 2, 2, data, (float)i / 180f * Math.PI, Axis2D.YAxis);


                        getFBPData(Proj);

                        CloseFBP(Proj);

                    }
                }
            }
            viewerControl3D1.SetImage(data);

        }



        static IntPtr Proj;

        /// <summary>
        /// calculates the needed vectors to turn the requested angle into projection vectors
        /// </summary>
        /// <param name="Slice"></param>
        /// <param name="PaintingWidth">Physical dimension of the pseudo projection (vs the recon size)</param>
        /// <param name="PaintingHeight">Physical dimension of the pseudo projection (vs the recon size)</param>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians">projection angle in radians</param>
        /// <param name="ConvolutionAxis">Usually set to YAxis. This is the direction that carries the rotation, or the fast axis for the convolution</param>
        public static void DoBackProjection_OneSlice(double[,] Slice, double PaintingWidth, double PaintingHeight, double[, ,] DensityGrid, double AngleRadians, Axis2D ConvolutionAxis)
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

            SmearArray2D(DensityGrid, 2, 2, Slice, PaintingWidth, PaintingHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));
        }



        private static unsafe void SmearArray2D(double[, ,] DensityGrid, float GridWidth, float GridHeight, double[,] PaintingArray, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
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
                RunFBP(Proj, pImage, bXX, bXY, bXZ, bYX, bYY, bYZ);
                /*
                fixed (float* pData = DensityGrid)
                {
                    for (int x = 0; x < LX; x++)
                        for (int y = 0; y < LY; y++)
                            for (int z = 0; z < LZ; z++)
                            {
                                Coords = new uint[] { (uint)x, (uint)y, (uint)z };
                                simpleFBP(pData, pImage, new uint2((uint)LOutX, (uint)LOutY), new uint4((uint)LX, (uint)LY, (uint)LZ, 0), new float4(bXX, bXY, bXZ, 0), new float4(bYX, bYY, bYZ, 0));
                            }
                 //  
                }*/
            }


        }

        private struct uint4
        {
            public uint x;
            public uint y;
            public uint z;
            public uint w;

            public uint4(uint x, uint y, uint z, uint w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }
        }
        private struct float4
        {
            public float x;
            public float y;
            public float z;
            public float w;
            public float4(float x, float y, float z, float w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }

        }
        private struct uint2
        {
            public uint x;
            public uint y;
            public uint2(uint x, uint y)
            {
                this.x = x;
                this.y = y;
            }
        }

        private static uint[] Coords;
        private static uint get_global_id(int index)
        {
            return Coords[index];
        }
        private static float round(float value)
        {
            return (float)Math.Round(value);
        }
        private static unsafe void simpleFBP(
                               float* volume,
                               float* input,
                               uint2 inputDimensions,
                               uint4 cubeDimensions,
                               float4 UP,
                               float4 Across
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

            uint xI = (get_global_id(0));
            uint yI = (get_global_id(1));
            uint zI = (get_global_id(2));

            uint cTmp = xI + yI * cWidth + zI * SliceSize;
            uint iTmp;

            int xII = (int)xI - hcWidth;
            int yII = (int)yI - hcHeight;
            int zII = (int)zI - hcDepth;

            float x, y;

            //calculate the position of the current voxel on the image


            x = UP.x * xII + UP.y * yII + UP.z * zII + hiWidth;
            y = Across.x * xII + Across.y * yII + Across.z * zII + hiHeight;

            if (x > 0 && x < iWidth)
                if (y > 0 && y < iHeight)
                {
                    y = round(y);
                    x = round(x);

                    if (x > 0 && x < iWidth)
                        if (y > 0 && y < iHeight)
                        {
                            iTmp = (uint)(x + y * iWidth);
                            volume[cTmp] = input[iTmp];
                        }
                }

        }

        /* private float DoProjection(float2 stepZ, float2 aX, float4 aXX, float4 bXX)
         {
             int zI, yI, xI;
             float z, x, y;
           
             //int zI = SmearArray2DQueue.Dequeue();

             //calculate the position of the current voxel on the image
             z = (zI * stepZ + sZ);
             aX = bZX * z + aZX;
             aY = bZY * z + aZY;

             ///this complicated number of steps ensures that there are only adds in this loop and the loop above, trying to speed up this
             ///routine
             float tXI =aX+ bXY * yI + bXX * xI;
             float tYI =aY+ bYY * yI+ bYX * xI;

             //make sure that we are still in the recon volumn
             if (tXI > 0 && tXI < LOutX)
                 if (tYI > 0 && tYI < LOutY)
                 {
                     y = Math.Round(tYI);
                     x = Math.Round(tXI);
                     if (x < LOutX)
                         if (y < LOutY)
                         {
                             *POutX += PaintingArray[(int)y, (int)x];
                         }
                 }

         }*/


    }
}
