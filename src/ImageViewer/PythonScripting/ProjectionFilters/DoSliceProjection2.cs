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
using MathHelpLib.ProjectionFilters;
using System.Runtime.InteropServices;
using System.Threading;



namespace ImageViewer.PythonScripting.Projection
{
    public class DoSliceBackProjectionEffect2 : aEffectNoForm
    {
        public override string EffectName { get { return "Project Slice Through Object (Two GPU)"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Projection"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        public class DoSliceBPartialToken : DoSliceBackProjectionEffect.DoSliceBPToken
        {
            public IntPtr Proj1 = IntPtr.Zero;
            public IntPtr Proj2 = IntPtr.Zero;

          /*  public string TokenName()
            {
                return this.ToString();
            }
            public bool GPUError = false;

           

            public GCHandle CubeHandle;
            public IntPtr CubeAddress;
            public long CubeLength;
            public int CubeX, CubeY, CubeZ;
            public double[, ,] DataCube;

            public int CallingThread;*/
        }

        /// <summary>
        /// This is the core routine for filtered back projection.  This back projects the desired array into the recon volume
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">pseudo projection in either physical array or double[,]; recon grid in either physical array or projectionarrayobject; projection angle (rotation around z)</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mPassData = PassData;

            mFilterToken = Parameters;

            if (mFilterToken[0].GetType() == typeof(PhysicalArray))
            {
                PhysicalArray Slice = (PhysicalArray)mFilterToken[0];
                PhysicalArray DensityGrid = (PhysicalArray)mFilterToken[1];
                double Angle = EffectHelps.ConvertToDouble(mFilterToken[2]);

                DoSliceBackProjectionEffect.DoBackProjection_OneSlice(Slice, DensityGrid, Angle, Axis2D.YAxis);
            }
            else if (mFilterToken[1].GetType() == typeof(ProjectionArrayObject))
            {
                double[,] Slice = (double[,])mFilterToken[0];
                ProjectionArrayObject DensityGrid = (ProjectionArrayObject)mFilterToken[1];
                double Angle = EffectHelps.ConvertToDouble(mFilterToken[2]);

                if (dataEnvironment.RunningOnGPU)
                {
                    lock (CriticalSectionLockGPU)
                    {
                        if (DSBPToken == null)
                        {
                            if (dataEnvironment.EffectTokens.ContainsKey("DoSliceProjection") == true)
                                DSBPToken = (DoSliceBPartialToken)dataEnvironment.EffectTokens["DoSliceProjection"];
                            else
                            {
                                DSBPToken = new DoSliceBPartialToken();
                                dataEnvironment.EffectTokens.Add("DoSliceProjection", DSBPToken);
                            }
                        }
                    }

                    if (DensityGrid.Data == null)
                    {
                        try
                        {
                            //run the GPU convolution
                            BackProjectGPU(Slice, DensityGrid.DataWhole, Angle, DSBPToken);
                            return SourceImage;
                        }
                        catch
                        {
                            dataEnvironment.RunningOnGPU = false;
                            DSBPToken.GPUError = true;
                        }
                    }
                }

                DoSliceBackProjectionEffect.DoBackProjection_OneSlice(Slice, DensityGrid.Width, DensityGrid.Height, DensityGrid, Angle, Axis2D.YAxis);
            }
            else if (mFilterToken[0].GetType() == typeof(float[, ,]))
            {
                if (DSBPToken.GPUError == false && File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\ProjectFBPImage.dll") == true)
                {
                    float[,] DataIn = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToFloatArray((double[,])mFilterToken[0], false);
                    float[, ,] DensityGrid = (float[, ,])mFilterToken[1];
                    double Angle = EffectHelps.ConvertToDouble(mFilterToken[2]);
                    try
                    {
                        //run the GPU convolution
                        throw new Exception("Not implimented");
                        //   BackProjectGPU(DataIn, DensityGrid, Angle);
                    }
                    catch { DSBPToken.GPUError = true; }
                }
            }
            return SourceImage;
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Convolution|PhysicalArray", "ProjectionObject|PhysicalArray", "SliceAngle|double(Radians)" }; }
        }

        #region GPU projection
        //creating the GPU program is expensive, so only create it one time, only need to pass the kernal over once, and it only seems to 
        //work if the output is not changed, so always use the correct output buffer

        static object CriticalSectionLockGPU = new object();
        static object CriticalSectionLockGPU2 = new object();

        /*
       */

        DoSliceBPartialToken DSBPToken = null;

        [DllImport("Project_Partial_FBPImage.dll")]
        private static unsafe extern IntPtr CreateFBP(string KernalName, string ConvolutionCode, string FlagStr,
            double* Image, int ImageWidth, int ImageHeight, double* Cube, int CubeWidth, int CubeHeight, int CubeDepth, int ZStart, int ZLength);

        [DllImport("Project_Partial_FBPImage.dll")]
        private static unsafe extern int RunFBP(IntPtr Projector, double* Image, float uX, float uY, float uZ, float aX, float aY, float aZ);

        [DllImport("Project_Partial_FBPImage.dll")]
        private static unsafe extern int getFBPData(IntPtr Projector);

        [DllImport("Project_Partial_FBPImage.dll")]
        private static unsafe extern int CloseFBP(IntPtr Projector);

        #region FBPCode
        static string FBPcode = @"
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
/*
double linearInterpolate (double p[2], double u) {
	return p[0]*u+p[1]*(1-u);
}

double biLinearInterpolate (double p[2][2], double uX, double uY) {
	double arr[2];
	arr[0] = linearInterpolate(p[0], uY);
	arr[1] = linearInterpolate(p[1], uY);
	return linearInterpolate(arr, uX);
}*/

void DO_Projection(     __global  double * volume,
                        __global  double * input,
                         const     uint2  inputDimensions,
                         const     uint4  cubeDimensions,
						 const     float4 UP,
                         const     float4 Across,
                         const     uint2 ZInterval,
                         uint xI,
						 uint yI,
						 uint zI
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

          

            uint cTmp = xI + yI * cWidth + zI * SliceSize;
            uint iTmp00,iTmp01,iTmp10,iTmp11;

            int xII =(int)xI - hcWidth;
            int yII =(int) yI - hcHeight;
            int zII = (int)(zI + ZInterval.x) - hcDepth;//places this z in the correct spot within the image, since the cube z position is adjusted to zero)

            double  x, y,rX,rY,uX,uY,V1,V2;
            
            //calculate the position of the current voxel on the image
            

            x = UP.x * xII + UP.y *yII +UP.z *zII  + hiWidth ;
            y = Across.x * xII + Across.y * yII + Across.z * zII+ hiHeight ;

             if (x>0 && x<iWidth && y>0 && y<iHeight)
            {
               // rX = floor(x);
               // rY = floor(y);
                 rX = round(x);
                 rY = round(y);
                if (x>1 && x<iWidth-1 && y>1 && y<iHeight-1)
                {
                    /*uY=x-rX;
                    uX=y-rY;
					
					iTmp00 = (uint)(rX + rY * iWidth);
                    iTmp10 = iTmp00+1;
                    iTmp01 =(uint)( rX + (rY-1) * iWidth);
                    iTmp11 = iTmp10+1;
					
                    V1 = input[iTmp00]*uY+input[iTmp01]*(1-uY);
                    V2 = input[iTmp10]*uY+input[iTmp11]*(1-uY);
              
                    volume[cTmp] +=  V1*uX+V2*(1-uX);*/

                   
                    iTmp00 = (uint)(rX + rY * iWidth);
                    volume[cTmp] +=input[iTmp00];
                }
                /*else if (x>=1 && x<iWidth && y>=0 && y<iHeight)
                {
                    iTmp00 = (uint)(rX + rY * iWidth);
                    volume[cTmp] +=input[iTmp00];
                }*/
            }
}
__kernel void simpleFBP( __global  double * volume,
                         __global  double * input,
                         const     uint2  inputDimensions,
                         const     uint4  cubeDimensions,
						 const     float4 UP,
                         const     float4 Across,
                         const     uint2 ZInterval,
                         const     uint KernalWorkSize
                         )
 {

            uint xI =( get_global_id(0));
            uint yI =(  get_global_id(1));
            uint zI =(  get_global_id(2));
			
			DO_Projection(volume,input, inputDimensions,cubeDimensions,UP,Across,ZInterval,xI,yI,zI);
}
";

        #endregion


        public static void Dispose(DataEnvironment dataEnvironment)
        {
            lock (CriticalSectionLockGPU)
            {
                DoSliceBPartialToken DSBPToken = null;
                if (DSBPToken == null)
                {
                    if (dataEnvironment.EffectTokens.ContainsKey("DoSliceProjection") == true)
                        DSBPToken = (DoSliceBPartialToken)dataEnvironment.EffectTokens["DoSliceProjection"];
                    else
                    {
                        DSBPToken = new DoSliceBPartialToken();
                        dataEnvironment.EffectTokens.Add("DoSliceProjection", DSBPToken);
                    }
                }

                if (DSBPToken.CallingThread == Thread.CurrentThread.ManagedThreadId)
                {
                    try
                    {
                        //   CloseFBP(DSBPToken.Proj);
                    }
                    catch { }
                }

            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Projection"></param>
        /// <param name="Volume"></param>
        /// <param name="Angle">Projection angle in radians</param>
        public static unsafe void BackProjectGPU(double[,] Projection, double[, ,] Volume, double Angle, DoSliceBPartialToken DSBPToken)
        {
            lock (CriticalSectionLockGPU)
            {
                if (DSBPToken.Proj1 == IntPtr.Zero)
                {
                    DSBPToken.CallingThread = Thread.CurrentThread.ManagedThreadId;
                    DSBPToken.CubeHandle = GCHandle.Alloc(Volume, GCHandleType.Pinned);
                    DSBPToken.CubeAddress = DSBPToken.CubeHandle.AddrOfPinnedObject();
                    DSBPToken.CubeX = Volume.GetLength(0);
                    DSBPToken.CubeY = Volume.GetLength(1);
                    DSBPToken.CubeZ = Volume.GetLength(2);
                    DSBPToken.CubeLength = Volume.Length;
                    DSBPToken.DataCube = Volume;

                    //gpu cannot handle a cube larger than 250
                    if (DSBPToken.CubeZ > 700)
                    {
                        int StartZ = DSBPToken.CubeZ / 2;
                        fixed (double* pImage = Projection)
                        {
                            DSBPToken.Proj1 = CreateFBP("simpleFBP", FBPcode, " ",
                                pImage, Projection.GetLength(0), Projection.GetLength(1),
                                (double*)DSBPToken.CubeAddress, DSBPToken.CubeZ, DSBPToken.CubeY, DSBPToken.CubeX, StartZ, DSBPToken.CubeZ / 2 - 1);

                            DSBPToken.Proj2 = CreateFBP("simpleFBP", FBPcode, " ",
                                pImage, Projection.GetLength(0), Projection.GetLength(1),
                                (double*)DSBPToken.CubeAddress, DSBPToken.CubeZ, DSBPToken.CubeY, DSBPToken.CubeX, 0, StartZ);

                        }
                    }
                    else
                    {
                        int StartZ = DSBPToken.CubeZ / 2;
                        fixed (double* pImage = Projection)
                        {
                            DSBPToken.Proj1 = CreateFBP("simpleFBP", FBPcode, " ",
                                pImage, Projection.GetLength(0), Projection.GetLength(1),
                                (double*)DSBPToken.CubeAddress, DSBPToken.CubeZ, DSBPToken.CubeY, DSBPToken.CubeX, 0, DSBPToken.CubeZ);

                            DSBPToken.Proj2 = IntPtr.Zero;

                        }
                    }
                }
            }

            DoBackProjection_OneSlice(Projection, 2, 2, Volume, Angle, Axis2D.YAxis, DSBPToken);
        }

        public static unsafe double[, ,] ReadReconVolume(DataEnvironment dataEnvironment)
        {

            DoSliceBPartialToken DSBPToken = null;
            lock (CriticalSectionLockGPU)
            {
                if (dataEnvironment.EffectTokens.ContainsKey("DoSliceProjection") == true)
                    DSBPToken = (DoSliceBPartialToken)dataEnvironment.EffectTokens["DoSliceProjection"];
                else
                {
                    DSBPToken = new DoSliceBPartialToken();
                    dataEnvironment.EffectTokens.Add("DoSliceProjection", DSBPToken);
                }
            }

            getFBPData(DSBPToken.Proj1);

            //   CloseFBP(DSBPToken.Proj);
            DSBPToken.Proj1 = IntPtr.Zero;

            if (DSBPToken.Proj2 != IntPtr.Zero)
            {
                getFBPData(DSBPToken.Proj2);

                //   CloseFBP(DSBPToken.Proj);
                DSBPToken.Proj2 = IntPtr.Zero;
            }
            DSBPToken.CubeHandle.Free();
            DSBPToken.CubeAddress = IntPtr.Zero;
            return DSBPToken.DataCube;
        }

        public static unsafe double[, ,] CopyReconVolume(DataEnvironment dataEnvironment)
        {
            DoSliceBPartialToken DSBPToken = null;
            lock (CriticalSectionLockGPU)
            {
                if (dataEnvironment.EffectTokens.ContainsKey("DoSliceProjection") == true)
                    DSBPToken = (DoSliceBPartialToken)dataEnvironment.EffectTokens["DoSliceProjection"];
                else
                {
                    DSBPToken = new DoSliceBPartialToken();
                    dataEnvironment.EffectTokens.Add("DoSliceProjection", DSBPToken);
                }
            }


            getFBPData(DSBPToken.Proj1);
            if (DSBPToken.Proj2 != IntPtr.Zero)
            {
                getFBPData(DSBPToken.Proj2);
            }

            double[, ,] DataOut = new double[DSBPToken.CubeX, DSBPToken.CubeY, DSBPToken.CubeZ];
            unchecked
            {
                fixed (double* pDAtaOut = DataOut)
                {
                    double* pOut = pDAtaOut;
                    double* pIn = (double*)DSBPToken.CubeAddress;
                    double V = 0;
                    for (int i = 0; i < DSBPToken.CubeLength; i++)
                    {
                        V = *pIn;
                        if (double.IsInfinity(V) == false && double.IsNaN(V) == false)
                            *pOut = V;
                        pOut++;
                        pIn++;
                    }
                }
            }



            CloseFBP(DSBPToken.Proj1);
            DSBPToken.Proj1 = IntPtr.Zero;
            if (DSBPToken.Proj2 != IntPtr.Zero)
            {
                CloseFBP(DSBPToken.Proj2);
                DSBPToken.Proj2 = IntPtr.Zero;
            }
            DSBPToken.CubeHandle.Free();
            DSBPToken.CubeAddress = IntPtr.Zero;
            return DataOut;
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
        private static void DoBackProjection_OneSlice(double[,] Slice, double PaintingWidth, double PaintingHeight, double[, ,] DensityGrid, double AngleRadians, Axis2D ConvolutionAxis, DoSliceBPartialToken DSBPToken)
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

            SmearArray2D(DensityGrid, 2, 2, Slice, PaintingWidth, PaintingHeight, vec, Point3D.CrossProduct(vec, vRotationAxis), DSBPToken);
        }

        private static unsafe void SmearArray2D(double[, ,] DensityGrid, float GridWidth, float GridHeight, double[,] PaintingArray, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection, DoSliceBPartialToken DSBPToken)
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

            lock (CriticalSectionLockGPU2)
            {
                fixed (double* pImage = PaintingArray)
                {
                    RunFBP(DSBPToken.Proj1, pImage, bXX, bXY, bXZ, bYX, bYY, bYZ);
                    if (DSBPToken.Proj2 != IntPtr.Zero)
                    {
                        RunFBP(DSBPToken.Proj2, pImage, bXX, bXY, bXZ, bYX, bYY, bYZ);
                    }
                }
            }
            mDataDouble = null;

        }

        #endregion

          /// <summary>
        /// calculates the needed vectors to turn the requested angle into projection vectors
        /// </summary>
        /// <param name="Slice"></param>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians"></param>
        /// <param name="ConvolutionAxis">Usually set to YAxis. This is the direction that carries the rotation, or the fast axis for the convolution</param>
        public static void DoBackProjection_OneSlice(PhysicalArray Slice, PhysicalArray DensityGrid, double AngleRadians, Axis2D ConvolutionAxis)
        {
            Axis RotationAxis = Axis.ZAxis;
            if (DensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
            {
                Point3D vec;
                Point3D axis;

                axis = new Point3D(0, 0, 1);


                double rx = Math.Cos(AngleRadians);
                double ry = Math.Sin(AngleRadians);

                vec = new Point3D(ry, rx, 0);

                DensityGrid.SmearArray(Slice, vec, Point3D.CrossProduct(vec, axis));

            }
            else
            {
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

                // DensityGrid.SmearArrayInterpolate1D (Slice, vec, Point3D.CrossProduct(vec, vRotationAxis),PhysicalArray.InterpolationMethod.Cubic );
                DensityGrid.SmearArray(Slice, vec, Point3D.CrossProduct(vec, vRotationAxis));
            }

        }

        /// <summary>
        /// calculates the needed vectors to turn the requested angle into projection vectors
        /// </summary>
        /// <param name="Slice"></param>
        /// <param name="PaintingWidth">Physical dimension of the pseudo projection (vs the recon size)</param>
        /// <param name="PaintingHeight">Physical dimension of the pseudo projection (vs the recon size)</param>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians"></param>
        /// <param name="ConvolutionAxis">Usually set to YAxis. This is the direction that carries the rotation, or the fast axis for the convolution</param>
        public static void DoBackProjection_OneSlice(double[,] Slice, double PaintingWidth, double PaintingHeight, ProjectionArrayObject DensityGrid, double AngleRadians, Axis2D ConvolutionAxis)
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

            SmearArray2D(DensityGrid, Slice, PaintingWidth, PaintingHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));
        }

        static Random rnd = new Random();

        private static void SmearArray2D(ProjectionArrayObject DensityGrid, double[,] PaintingArray, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
        {
            //used to keep track of which slices have finished.  this allows the threads to all work on different arrays of the projection object
            bool[] SliceFinished = new bool[DensityGrid.Data.GetLength(0)];

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
            double[][,] mDataDouble = DensityGrid.Data;

            //get all the dimensions
            int LX = mDataDouble[0].GetLength(1);
            int LY = mDataDouble[0].GetLength(0);
            int LZ = mDataDouble.GetLength(0);

            int LOutX = PaintingArray.GetLength(1);
            int LOutY = PaintingArray.GetLength(0);

            double sX = DensityGrid.XMin;// mPhysicalStart[2];
            double sY = DensityGrid.YMin;// mPhysicalStart[1];
            double sZ = DensityGrid.ZMin;// mPhysicalStart[0];

            double sOutX = PaintingWidth / -2d; //PaintingArray.PhysicalStart(Axis.XAxis);
            double sOutY = PaintingHeight / -2d; //PaintingArray.PhysicalStart(Axis.YAxis);

            double stepOutX = PaintingWidth / PaintingArray.GetLength(1); //PaintingArray.PhysicalStep(Axis.XAxis);
            double stepOutY = PaintingHeight / PaintingArray.GetLength(0); //PaintingArray.PhysicalStep(Axis.YAxis);

            double stepX = DensityGrid.Width / LX;// mPhysicalStep[2];
            double stepY = DensityGrid.Height / LY;//mPhysicalStep[1];
            double stepZ = DensityGrid.Depth / LZ;// mPhysicalStep[0];

            double z;
            double tXI, tYI, tX1, tY1;

            double aX, aY, bXY, bXX, bYY, bYX;
            double aZX, bZX, aZY, bZY;

            double uX, uY;
            int rX, rY;
            uint iTmp00, iTmp10, iTmp01, iTmp11;
            double V1, V2;

            //a lot of the coeffiences can be calculated outside the loop saving time and computation
            bXX = normalX.Y * stepY / stepOutX;
            bXY = normalX.X * stepX / stepOutX;

            bYX = normalY.Y * stepY / stepOutY;
            bYY = normalY.X * stepX / stepOutY;

            aZX = (normalX.Y * sY - sOutX + normalX.X * sX) / stepOutX;
            bZX = normalX.Z / stepOutX;

            aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY;
            bZY = normalY.Z / stepOutY;

            int LY_1 = LOutY - 1;
            int LX_1 = LOutX - 1;

            int X, Y;
            unsafe
            {
                //we make the math unchecked to make it go faster.  there seem to be enough checks beforehand
                unchecked
                {
                    int zOffest = LY * LX;
                    int yOffset = LX;

                    int zI = 0;
                    int FinishedCount = 0;
                    //for (int zI = 0; zI < LZ; zI++)
                    while (FinishedCount < LZ)
                    {

                        //there is a lot of thread contention, so look through the whole stack and find an open slice, and work on it
                        for (int i = 0; i < LZ; i++)
                        {
                            if (DensityGrid.LockIndicator[i] == false && SliceFinished[i] == false)
                            {
                                zI = i;
                                break;
                            }
                        }

                        //indicate that the thread is locked
                        DensityGrid.LockIndicator[zI] = true;
                        lock (DensityGrid.LockArray[zI])
                        {
                            //when all the slices are processed jump out
                            FinishedCount++;
                            //indicate that this slice has been processed.
                            SliceFinished[zI] = true;

                            fixed (double* mipData = mDataDouble[zI])
                            {

                                double* POut = (double*)mipData;
                                double* POutZ = POut;
                                double* POutY;
                                double* POutX;
                                //int zI = SmearArray2DQueue.Dequeue();

                                //calculate the position of the current voxel on the image
                                z = (zI * stepZ + sZ);
                                aX = bZX * z + aZX;
                                aY = bZY * z + aZY;
                                tX1 = aX;
                                tY1 = aY;
                                POutY = POutZ;
                                for (int yI = 0; yI < LY; yI++)
                                {
                                    //calculate the position of the current voxel on the image
                                    tX1 += bXY;
                                    tY1 += bYY;

                                    tXI = tX1;
                                    tYI = tY1;
                                    POutX = POutY;
                                    for (int xI = 0; xI < LX; xI++)
                                    {
                                        ///this complicated number of steps ensures that there are only adds in this loop and the loop above, trying to speed up this
                                        ///routine
                                        tXI += bXX;
                                        tYI += bYX;

                                        //make sure that we are still in the recon volumn
                                        if (tXI > 0)
                                            if (tYI > 0)
                                            {
                                                rY = (int)Math.Floor(tYI);
                                                rX = (int)Math.Floor(tXI);
                                                if (rX < LX_1)
                                                {
                                                    if (rY < LY_1)
                                                    {

                                                        uX = 1 - (tXI - rX);
                                                        uY = 1 - (tYI - rY);

                                                        V1 = PaintingArray[rY, rX] * uY + PaintingArray[rY + 1, rX] * (1 - uY);
                                                        V2 = PaintingArray[rY, rX + 1] * uY + PaintingArray[rY + 1, rX + 1] * (1 - uY);

                                                        *POutX += V1 * uX + V2 * (1 - uX);
                                                    }
                                                    else if (rY == LY_1)
                                                    {
                                                        // Console.WriteLine(rX + " " + rY + " " + LX_1 + " " + LY_1 );
                                                        *POutX += PaintingArray[rY, rX];
                                                    }
                                                }
                                                else if (rX == LX_1 && rY < LOutY)
                                                {
                                                    *POutX += PaintingArray[rY, rX];
                                                }
                                            }
                                        POutX++;
                                    }
                                    POutY += yOffset;

                                }
                                POutZ += zOffest;
                            }

                        }
                        //release the programatic handle to 
                        DensityGrid.LockIndicator[zI] = false;
                    }
                }
                //SmearArray2DQueue = null;
            }
        }
        
    }

}