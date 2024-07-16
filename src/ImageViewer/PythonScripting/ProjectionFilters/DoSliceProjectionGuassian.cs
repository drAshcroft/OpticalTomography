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
    public class DoSliceBackProjectionGaussianEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Project Slice Through Object (Guassian)"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Projection"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
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
                DoBackProjection_OneSlice(Slice, DensityGrid, Angle, Axis2D.YAxis);
            }
            else if (mFilterToken[1].GetType() == typeof(ProjectionArrayObject))
            {
                double[,] Slice = (double[,])mFilterToken[0];
                ProjectionArrayObject DensityGrid = (ProjectionArrayObject)mFilterToken[1];
                double Angle = EffectHelps.ConvertToDouble(mFilterToken[2]);

                //gpu run will return the result.  if anything goes wrong, then do the normal cpu work
                DoBackProjection_OneSlice(Slice, 1, 1, DensityGrid, Angle, Axis2D.YAxis);
            }
            else if (mFilterToken[0].GetType() == typeof(float[, ,]))
            {
               /* if (DSBPToken.GPUError == false && File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\ProjectFBPImage.dll") == true)
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
                }*/
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
            //AngleRadians = 0;
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

            if (DensityGrid.Data != null)
                SmearArray2D(AngleRadians, DensityGrid, Slice, PaintingWidth, PaintingHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));
            else
                SmearArray2DWhole(AngleRadians, DensityGrid, Slice, PaintingWidth, PaintingHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));
        }

        static Random rnd = new Random();
        static object FindLock = new object();
        private static void SmearArray2D(double AngleRadians, ProjectionArrayObject DensityGrid, double[,] PaintingArray, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
        {

            //make sure all the vectors are the right size
            Direction.Normalize();
            FastScanDirection.Normalize();
            //determine all the important planes in the 3D space and all the step
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            ///get the existing recon data.  the jagged array is used so the data can be checked out by each thread
            double[][,] mDataDouble = DensityGrid.Data;
            double r = Math.Sqrt(2 * .5 * .5);
            double[] LUT = new double[50];
            #region Look up table
            double[] p_X = new double[4];
            double[] p_Y = new double[4];


            p_X[0] = Math.Cos(Math.PI / 4d + AngleRadians) * r;
            p_Y[0] = Math.Sin(Math.PI / 4d + AngleRadians) * r;

            p_X[1] = Math.Cos(Math.PI * 3d / 4d + AngleRadians) * r;
            p_Y[1] = Math.Sin(Math.PI * 3d / 4d + AngleRadians) * r;

            p_X[2] = Math.Cos(Math.PI * 5d / 4d + AngleRadians) * r;
            p_Y[2] = Math.Sin(Math.PI * 5d / 4d + AngleRadians) * r;

            p_X[3] = Math.Cos(Math.PI * 7d / 4d + AngleRadians) * r;
            p_Y[3] = Math.Sin(Math.PI * 7d / 4d + AngleRadians) * r;

            double[] o_X = new double[4];
            double[] o_Y = new double[4];
            double d;
            const double step = 1.2 / 50d;
            int cc = 0;
            for (double u2 = 0; u2 < 1.2; u2 += step)
            {
                double l1_X = -1.2;
                double l1_Y = u2;

                double l2_X = 1.2;
                double l2_Y = u2;

                for (int j = 0; j < 4; j++)
                {
                    int j2 = (j + 1) % 4;
                    d = (l1_X - l2_X) * (p_Y[j] - p_Y[j2]) - (l1_Y - l2_Y) * (p_X[j] - p_X[j2]);
                    if (d == 0)
                    {
                        o_X[j] = 1000;
                        o_Y[j] = 10000;
                    }
                    else
                    {
                        o_X[j] = ((l1_X * l2_Y - l1_Y * l2_X) * (p_X[j] - p_X[j2]) - (l1_X - l2_X) * (p_X[j] * p_Y[j2] - p_Y[j] * p_X[j2])) / d;
                        o_Y[j] = ((l1_X * l2_Y - l1_Y * l2_X) * (p_Y[j] - p_Y[j2]) - (l1_Y - l2_Y) * (p_X[j] * p_Y[j2] - p_Y[j] * p_X[j2])) / d;
                    }
                }

                double mX1 = 0, mY1 = 0, mX2 = 0, mY2;
                double minP = 1000;
                double minN = 1000;
                for (int j = 0; j < 4; j++)
                {
                    d = Math.Abs(o_X[j]);
                    if (o_X[j] < 0 && d < minN)
                    {
                        mX2 = o_X[j];
                        mY2 = o_Y[j];
                        minN = d;
                    }
                    if (o_X[j] > 0 && d < minP)
                    {
                        mX1 = o_X[j];
                        mY1 = o_Y[j];
                        minP = d;
                    }
                }
                /* double min2 = 1000;
                 for (int j = 0; j < 4; j++)
                 {
                     d=Math.Abs(o_X[j]) ;
                     if (d < min2 && d>min)
                     {
                         mX2 = o_X[j];
                         mY2 = o_Y[j];
                         min2 = d;
                     }
                 }*/
                LUT[cc] = Math.Abs(mX1 - mX2) * Math.Exp(-1 * u2 * u2 * 3);
                cc++;
            }
            #endregion



            #region constants
            //get all the dimensions
            int LI = mDataDouble[0].GetLength(1);
            int LJ = mDataDouble[0].GetLength(0);
            int LK = mDataDouble.GetLength(0);

            int LsI = PaintingArray.GetLength(1);
            int LsJ = PaintingArray.GetLength(0);
            int LsI_1 = LsI - 1;
            int LsJ_1 = LsJ - 1;

            double halfI, halfJ, halfK, halfIs, halfJs;
            double VolToSliceX, VolToSliceY, VolToSliceZ;

            halfI = (double)LI / 2;
            halfJ = (double)LJ / 2;
            halfK = (double)LK / 2;

            halfIs = (double)LsI / 2;
            halfJs = (double)LsJ / 2;

            VolToSliceX = (DensityGrid.XMax - DensityGrid.XMin) / LI * (double)PaintingArray.GetLength(0) / (2 * PaintingWidth);
            VolToSliceY = (DensityGrid.YMax - DensityGrid.YMin) / LJ * (double)PaintingArray.GetLength(0) / (2 * PaintingWidth);
            VolToSliceZ = 1;

            int K_ = 0;
            int FinishedCount = 0;
            #endregion

            double sX, sY, sK, sdotI, u;
            int lower_sI, lower_sJ;
            bool SliceFound = false;
            int LUT_Index;
            unsafe
            {
                //we make the math unchecked to make it go faster.  there seem to be enough checks beforehand
                unchecked
                {
                    //for (int zI = 0; zI < LZ; zI++)
                    // while (FinishedCount < LK)
                    {
                        #region Find open slice
                        //there is a lot of thread contention, so look through the whole stack and find an open slice, and work on it
                        SliceFound = false;
                        int StartI = (new Random(DateTime.Now.Millisecond)).Next(LK);
                        for (int i = 0; i < LK; i++)
                        {
                            //lock (FindLock)
                            {
                                //  if (DensityGrid.LockIndicator[i] == false)
                                {
                                    K_ = (i + StartI) % LK;
                                    DensityGrid.LockIndicator[K_] = true;
                                    FinishedCount++;
                                    SliceFound = true;
                                    //  break;
                                }
                            }

                        #endregion

                            if (SliceFound == true)
                            {
                                //indicate that the thread is locked
                                lock (DensityGrid.LockArray[K_])
                                {
                                    #region Process slice
                                    fixed (double* mipData = mDataDouble[K_])
                                    {
                                        double* POut;
                                        for (int J_ = 0; J_ < LJ; J_++)
                                        {
                                            //tranform to slice index coords
                                            sY = (J_ - halfI) * VolToSliceY;
                                            for (int I_ = 0; I_ < LI; I_++)
                                            {

                                                //tranform to slice index coords
                                                sX = (I_ - halfI) * VolToSliceX;

                                                sdotI = sX * FastScanDirection.X + sY * FastScanDirection.Y + halfIs;
                                                //make sure that we are still in the recon volumn
                                                if (sdotI > 0 && sdotI < LsI_1)
                                                    if (K_ > 0 && K_ < LsJ_1)
                                                    {
                                                        POut = (double*)mipData + I_ * LI + J_;
                                                        lower_sI = (int)Math.Floor(sdotI);
                                                        u = sdotI - lower_sI;
                                                        lower_sJ = K_;//(int)Math.Floor(K_);

                                                        LUT_Index = (int)(u / 1.2);
                                                        if (LUT_Index > 49)
                                                            LUT_Index = 49;
                                                        //*POut = PaintingArray[lower_sI, lower_sJ];
                                                        DensityGrid.Data[K_][I_, J_] += PaintingArray[lower_sJ, lower_sI] * LUT[LUT_Index]
                                                                                + PaintingArray[lower_sJ, lower_sI + 1] * LUT[49 - LUT_Index];
                                                    }
                                            }
                                        }
                                    }

                                    #endregion

                                }
                            }
                            //release the programatic handle to 
                            DensityGrid.LockIndicator[K_] = false;
                        }
                    }
                }
                //SmearArray2DQueue = null;
            }
        }

        private static void SmearArray2DWhole(double AngleRadians, ProjectionArrayObject DensityGrid, double[,] PaintingArray, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
        {

            //make sure all the vectors are the right size
            Direction.Normalize();
            FastScanDirection.Normalize();
            //determine all the important planes in the 3D space and all the step
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            ///get the existing recon data.  the jagged array is used so the data can be checked out by each thread
            double[, ,] mDataDouble = DensityGrid.DataWhole;
            double r = Math.Sqrt(2 * .5 * .5);
            double[] LUT = new double[50];
            #region Look up table
            double[] p_X = new double[4];
            double[] p_Y = new double[4];


            p_X[0] = Math.Cos(Math.PI / 4d + AngleRadians) * r;
            p_Y[0] = Math.Sin(Math.PI / 4d + AngleRadians) * r;

            p_X[1] = Math.Cos(Math.PI * 3d / 4d + AngleRadians) * r;
            p_Y[1] = Math.Sin(Math.PI * 3d / 4d + AngleRadians) * r;

            p_X[2] = Math.Cos(Math.PI * 5d / 4d + AngleRadians) * r;
            p_Y[2] = Math.Sin(Math.PI * 5d / 4d + AngleRadians) * r;

            p_X[3] = Math.Cos(Math.PI * 7d / 4d + AngleRadians) * r;
            p_Y[3] = Math.Sin(Math.PI * 7d / 4d + AngleRadians) * r;

            double[] o_X = new double[4];
            double[] o_Y = new double[4];
            double d;
            const double step = 1.2 / 50d;
            int cc = 0;
            for (double u2 = 0; u2 < 1.2; u2 += step)
            {
                double l1_X = -1.2;
                double l1_Y = u2;

                double l2_X = 1.2;
                double l2_Y = u2;

                for (int j = 0; j < 4; j++)
                {
                    int j2 = (j + 1) % 4;
                    d = (l1_X - l2_X) * (p_Y[j] - p_Y[j2]) - (l1_Y - l2_Y) * (p_X[j] - p_X[j2]);
                    if (d == 0)
                    {
                        o_X[j] = 1000;
                        o_Y[j] = 10000;
                    }
                    else
                    {
                        o_X[j] = ((l1_X * l2_Y - l1_Y * l2_X) * (p_X[j] - p_X[j2]) - (l1_X - l2_X) * (p_X[j] * p_Y[j2] - p_Y[j] * p_X[j2])) / d;
                        o_Y[j] = ((l1_X * l2_Y - l1_Y * l2_X) * (p_Y[j] - p_Y[j2]) - (l1_Y - l2_Y) * (p_X[j] * p_Y[j2] - p_Y[j] * p_X[j2])) / d;
                    }
                }

                double mX1 = 0, mY1 = 0, mX2 = 0, mY2;
                double minP = 1000;
                double minN = 1000;
                for (int j = 0; j < 4; j++)
                {
                    d = Math.Abs(o_X[j]);
                    if (o_X[j] < 0 && d < minN)
                    {
                        mX2 = o_X[j];
                        mY2 = o_Y[j];
                        minN = d;
                    }
                    if (o_X[j] > 0 && d < minP)
                    {
                        mX1 = o_X[j];
                        mY1 = o_Y[j];
                        minP = d;
                    }
                }
                /* double min2 = 1000;
                 for (int j = 0; j < 4; j++)
                 {
                     d=Math.Abs(o_X[j]) ;
                     if (d < min2 && d>min)
                     {
                         mX2 = o_X[j];
                         mY2 = o_Y[j];
                         min2 = d;
                     }
                 }*/
                LUT[cc] = Math.Abs(mX1 - mX2) * Math.Exp(-1 * u2 * u2 * 3);
                cc++;
            }
            #endregion


            #region constants
            //get all the dimensions
            int LI = mDataDouble.GetLength(2);
            int LJ = mDataDouble.GetLength(1);
            int LK = mDataDouble.GetLength(0);

            int LsI = PaintingArray.GetLength(1);
            int LsJ = PaintingArray.GetLength(0);
            int LsI_1 = LsI - 1;
            int LsJ_1 = LsJ - 1;

            double halfI, halfJ, halfK, halfIs, halfJs;
            double VolToSliceX, VolToSliceY, VolToSliceZ;

            halfI = (double)LI / 2;
            halfJ = (double)LJ / 2;
            halfK = (double)LK / 2;

            halfIs = (double)LsI / 2;
            halfJs = (double)LsJ / 2;

            VolToSliceX = (DensityGrid.XMax - DensityGrid.XMin) / LI * (double)PaintingArray.GetLength(0) / (2 * PaintingWidth);
            VolToSliceY = (DensityGrid.YMax - DensityGrid.YMin) / LJ * (double)PaintingArray.GetLength(0) / (2 * PaintingWidth);
            VolToSliceZ = 1;

            int K_ = 0;
            int FinishedCount = 0;
            #endregion

            double sX, sY, sK, sdotI, u;
            int lower_sI, lower_sJ;
            bool SliceFound = false;
            int LUT_Index;
            unsafe
            {
                //we make the math unchecked to make it go faster.  there seem to be enough checks beforehand
                unchecked
                {
                    fixed (double* mipDataWhole = mDataDouble)
                    {
                        //for (int zI = 0; zI < LZ; zI++)
                        // while (FinishedCount < LK)
                        {
                            #region Find open slice
                            //there is a lot of thread contention, so look through the whole stack and find an open slice, and work on it
                            SliceFound = false;
                            int StartI = (new Random(DateTime.Now.Millisecond)).Next(LK);
                            for (int i = 0; i < LK; i++)
                            {
                                //lock (FindLock)
                                {
                                    //  if (DensityGrid.LockIndicator[i] == false)
                                    {
                                        K_ = (i + StartI) % LK;
                                        DensityGrid.LockIndicator[K_] = true;
                                        FinishedCount++;
                                        SliceFound = true;
                                        //  break;
                                    }
                                }

                            #endregion

                                if (SliceFound == true)
                                {
                                    //indicate that the thread is locked
                                    lock (DensityGrid.LockArray[K_])
                                    {
                                        #region Process slice
                                        double* mipData = mipDataWhole + K_ * LI * LJ; //mDataDouble[K_]
                                        {
                                            double* POut;
                                            for (int J_ = 0; J_ < LJ; J_++)
                                            {
                                                //tranform to slice index coords
                                                sY = (J_ - halfI) * VolToSliceY;
                                                for (int I_ = 0; I_ < LI; I_++)
                                                {

                                                    //tranform to slice index coords
                                                    sX = (I_ - halfI) * VolToSliceX;

                                                    sdotI = sX * FastScanDirection.X + sY * FastScanDirection.Y + halfIs;
                                                    //make sure that we are still in the recon volumn
                                                    if (sdotI > 0 && sdotI < LsI_1)
                                                        if (K_ > 0 && K_ < LsJ_1)
                                                        {
                                                            POut = (double*)mipData + I_ * LI + J_;
                                                            lower_sI = (int)Math.Floor(sdotI);
                                                            u = sdotI - lower_sI;
                                                            lower_sJ = K_;//(int)Math.Floor(K_);

                                                            LUT_Index = (int)(u / 1.2);
                                                            if (LUT_Index > 49)
                                                                LUT_Index = 49;
                                                            //*POut = PaintingArray[lower_sI, lower_sJ];
                                                            mDataDouble[K_, I_, J_] += PaintingArray[lower_sJ, lower_sI] * LUT[LUT_Index]
                                                                                    + PaintingArray[lower_sJ, lower_sI + 1] * LUT[49 - LUT_Index];
                                                        }
                                                }
                                            }
                                        }

                                        #endregion

                                    }
                                }
                                //release the programatic handle to 
                                DensityGrid.LockIndicator[K_] = false;
                            }
                        }
                    }
                }
                //SmearArray2DQueue = null;
            }



        }
 
      
    }
}
