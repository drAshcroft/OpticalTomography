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

using MathHelpLib.ImageProcessing;

namespace ImageViewer.PythonScripting.Projection
{
    public class MakeMIPProjectionEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Make MIP Projection"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Projection"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        object SaveFileLock = new object();

        /// <summary>
        /// Takes the volume and projects does a maximum intensity projection(MIP) on the desired axis.  The image can then be saved if needed
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">Recon Volume as physicalarray, double[,,] or double[][,]; angle in radians; optional savefilename as string</param>
        /// <returns>returns imageholder if the image is not saved</returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mPassData = PassData;

            mFilterToken = Parameters;

            if (mFilterToken[0].GetType() == typeof(PhysicalArray))
            {
                PhysicalArray DensityGrid = (PhysicalArray)mFilterToken[0];
                double Angle = EffectHelps.ConvertToDouble(mFilterToken[1]);

                PhysicalArray MIP = DoMIPProjection_OneSlice(DensityGrid, Angle);

                if (mFilterToken.Length > 2)
                {
                    lock (SaveFileLock)
                    {
                        string Filename = (string)mFilterToken[2];
                        MIP.SaveData(Filename);
                        return SourceImage;
                    }
                }
                else
                    return new ImageHolder((double[,])MIP.ActualData2D);
            }
            else if (mFilterToken[0].GetType() == typeof(double[, ,]))
            {
                double[, ,] DensityGrid = (double[, ,])mFilterToken[0];
                double Angle = EffectHelps.ConvertToDouble(mFilterToken[1]);

                double[,] MIP = DoMIPProjection_OneSlice(DensityGrid, Angle);

                if (mFilterToken.Length > 2)
                {
                    lock (SaveFileLock)
                    {
                        Bitmap b = MIP.MakeBitmap();
                        try
                        {
                            string Filename = (string)mFilterToken[2];
                            b.Save(Filename);
                            return SourceImage;
                        }
                        catch (Exception ex)
                        {
                            string Filename = (string)mFilterToken[2];
                            b.Save(Filename);
                            return SourceImage;

                        }
                    }
                }
                else
                    return new ImageHolder((double[,])MIP);
            }
            else if (mFilterToken[0].GetType() == typeof(double[][,]))
            {
                double[][,] DensityGrid = (double[][,])mFilterToken[0];
                double Angle = EffectHelps.ConvertToDouble(mFilterToken[1]);

                double[,] MIP = DoMIPProjection_OneSlice(DensityGrid, Angle);

                if (mFilterToken.Length > 2)
                {
                    lock (SaveFileLock)
                    {
                        Bitmap b = MIP.MakeBitmap();
                        try
                        {
                            string Filename = (string)mFilterToken[2];
                            b.Save(Filename);
                            return SourceImage;
                        }
                        catch (Exception ex)
                        {
                            string Filename = (string)mFilterToken[2];
                            b.Save(Filename);
                            return SourceImage;

                        }
                    }
                }
                else
                    return new ImageHolder((double[,])MIP);
            }

            return null;
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Convolution|PhysicalArray", "SliceAngle|double(Radians)", "optional OutFilename|string" }; }
        }

        /// <summary>
        /// Takes a recon volume and does a MIP projection from the desired axis around the Z axis
        /// </summary>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians"></param>
        /// <returns></returns>
        public static  PhysicalArray DoMIPProjection_OneSlice(PhysicalArray DensityGrid, double AngleRadians)
        {
            //calculate the direction vectors needed to do the mip, these are defaults that assume that the rotation should be around the z axis and the fast scan axis is the one that matters
            PhysicalArray SliceProjection;
            MathHelpLib.Axis RotationAxis = MathHelpLib.Axis.ZAxis;
            if (DensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
            {
                throw new Exception("Not Yet Implimented");
            }
            else
            {
                Point3D vRotationAxis = new Point3D();
                Point3D axis = new Point3D();

                if (RotationAxis == MathHelpLib.Axis.XAxis)
                {
                    vRotationAxis = new Point3D(1, 0, 0);
                    axis = new Point3D(0, 1, 0);
                }
                else if (RotationAxis == MathHelpLib.Axis.YAxis)
                {
                    vRotationAxis = new Point3D(0, 1, 0);
                    axis = new Point3D(0, 0, 1);
                }
                else if (RotationAxis == MathHelpLib.Axis.ZAxis)
                {
                    vRotationAxis = new Point3D(0, 0, 1);
                    axis = new Point3D(0, 1, 0);
                }

                Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, AngleRadians);

                ///use the nice function do do mips from the Physical Array
                SliceProjection = DensityGrid.ProjectMIP(vec, Point3D.CrossProduct(vec, vRotationAxis));
                return SliceProjection;
            }
        }

        /// <summary>
        /// Takes a recon volume and does a MIP projection from the desired axis around the Z axis
        /// </summary>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians"></param>
        /// <returns></returns>
        public static  double[,] DoMIPProjection_OneSlice(double[, ,] DensityGrid, double AngleRadians)
        {
            //calculate the direction vectors needed to do the mip, these are defaults that assume that the rotation should be around the z axis and the fast scan axis is the one that matters
            double[,] SliceProjection;
            MathHelpLib.Axis RotationAxis = MathHelpLib.Axis.ZAxis;

            Point3D vRotationAxis = new Point3D();
            Point3D axis = new Point3D();

            if (RotationAxis == MathHelpLib.Axis.XAxis)
            {
                vRotationAxis = new Point3D(1, 0, 0);
                axis = new Point3D(0, 1, 0);
            }
            else if (RotationAxis == MathHelpLib.Axis.YAxis)
            {
                vRotationAxis = new Point3D(0, 1, 0);
                axis = new Point3D(0, 0, 1);
            }
            else if (RotationAxis == MathHelpLib.Axis.ZAxis)
            {
                vRotationAxis = new Point3D(0, 0, 1);
                axis = new Point3D(0, 1, 0);
            }

            Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, AngleRadians);

            SliceProjection = ProjectMIP(DensityGrid, vec, Point3D.CrossProduct(vec, vRotationAxis));
            return SliceProjection;

        }

        /// Takes a recon volume and does a MIP projection from the desired axis around the Z axis
        /// </summary>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians"></param>
        /// <returns></returns>
        public static double[,] DoForwardProjection_OneSlice(double[, ,] DensityGrid, double AngleRadians)
        {
            //calculate the direction vectors needed to do the mip, these are defaults that assume that the rotation should be around the z axis and the fast scan axis is the one that matters
            double[,] SliceProjection;
            MathHelpLib.Axis RotationAxis = MathHelpLib.Axis.ZAxis;

            Point3D vRotationAxis = new Point3D();
            Point3D axis = new Point3D();

            if (RotationAxis == MathHelpLib.Axis.XAxis)
            {
                vRotationAxis = new Point3D(1, 0, 0);
                axis = new Point3D(0, 1, 0);
            }
            else if (RotationAxis == MathHelpLib.Axis.YAxis)
            {
                vRotationAxis = new Point3D(0, 1, 0);
                axis = new Point3D(0, 0, 1);
            }
            else if (RotationAxis == MathHelpLib.Axis.ZAxis)
            {
                vRotationAxis = new Point3D(0, 0, 1);
                axis = new Point3D(0, 1, 0);
            }

            Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, AngleRadians);

            SliceProjection =  ForwardProject(DensityGrid, vec, Point3D.CrossProduct(vec, vRotationAxis));
            return SliceProjection;

        }


        /// <summary>
        /// Takes a recon volume and does a MIP projection from the desired axis around the Z axis
        /// </summary>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians"></param>
        /// <returns></returns>
        public static double[,] DoMIPProjection_OneSlice(float [, ,] DensityGrid, double AngleRadians)
        {
            //calculate the direction vectors needed to do the mip, these are defaults that assume that the rotation should be around the z axis and the fast scan axis is the one that matters
            double[,] SliceProjection;
            MathHelpLib.Axis RotationAxis = MathHelpLib.Axis.ZAxis;

            Point3D vRotationAxis = new Point3D();
            Point3D axis = new Point3D();

            if (RotationAxis == MathHelpLib.Axis.XAxis)
            {
                vRotationAxis = new Point3D(1, 0, 0);
                axis = new Point3D(0, 1, 0);
            }
            else if (RotationAxis == MathHelpLib.Axis.YAxis)
            {
                vRotationAxis = new Point3D(0, 1, 0);
                axis = new Point3D(0, 0, 1);
            }
            else if (RotationAxis == MathHelpLib.Axis.ZAxis)
            {
                vRotationAxis = new Point3D(0, 0, 1);
                axis = new Point3D(0, 1, 0);
            }

            Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, AngleRadians);

            SliceProjection = ProjectMIP(DensityGrid, vec, Point3D.CrossProduct(vec, vRotationAxis));
            return SliceProjection;

        }


        static Random rnd = new Random();

        /// <summary>
        /// Do the work of projecting the grid out onto the image
        /// </summary>
        /// <param name="DensityGrid"></param>
        /// <param name="Direction"></param>
        /// <param name="FastScanDirection"></param>
        /// <returns></returns>
        private static  double[,] ProjectMIP(double[, ,] DensityGrid, Point3D Direction, Point3D FastScanDirection)
        {
            try
            {
                double LengthCorner = 0;
                //determine the size of the output image
                int GridWidth = DensityGrid.GetLength(1);
                if (DensityGrid.GetLength(2) > GridWidth) GridWidth = DensityGrid.GetLength(2);

                LengthCorner = Math.Sqrt(2);

                double[,] PaintingArray = new double[(int)(DensityGrid.GetLength(0)), (int)(GridWidth)];

                ///get all the direction vectors correct
                Direction.Normalize();
                FastScanDirection.Normalize();
                Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

                Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
                Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
                normalX.Normalize();
                normalY.Normalize();

                double[, ,] mDataDouble = DensityGrid;

                int LX = mDataDouble.GetLength(2);
                int LY = mDataDouble.GetLength(1);
                int LZ = mDataDouble.GetLength(0);

                int LOutX = PaintingArray.GetLength(1);
                int LOutY = PaintingArray.GetLength(0);
                ///just assume a simple set of sizes for the motion 
                double sX = -1;
                double sY = -1;
                double sZ = -1;

                double sOutX = 2d / -2d; //PaintingArray.PhysicalStart(Axis.XAxis);
                double sOutY = 2d / -2d; //PaintingArray.PhysicalStart(Axis.YAxis);

                double stepOutX = 2d / PaintingArray.GetLength(1); //PaintingArray.PhysicalStep(Axis.XAxis);
                double stepOutY = 2d / PaintingArray.GetLength(0); //PaintingArray.PhysicalStep(Axis.YAxis);

                double stepX = 2d / DensityGrid.GetLength(2);// mPhysicalStep[2];
                double stepY = 2d / DensityGrid.GetLength(1);//mPhysicalStep[1];
                double stepZ = 2d / DensityGrid.GetLength(0);// mPhysicalStep[0];

                double z;
                double tXI, tYI, tX1, tY1;
                ///precalculate as many coeff as possible to avoid math inside the nastly loop
                double aX, aY, bXY, bXX, bYY, bYX;
                double aZX, bZX, aZY, bZY;

                bXX = normalX.Y * stepY / stepOutX;
                bXY = normalX.X * stepX / stepOutX;

                bYX = normalY.Y * stepY / stepOutY;
                bYY = normalY.X * stepX / stepOutY;

                aZX = (normalX.Y * sY - sOutX + normalX.X * sX) / stepOutX;
                bZX = normalX.Z / stepOutX;

                aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY;
                bZY = normalY.Z / stepOutY;

                double[] Normalization = new double[mDataDouble.GetLength(0)];
                double u, value;
                int X, Y;
              
                unchecked
                {
                    unsafe
                    {
                        ///walk through the loop, sending the grid data out to the image
                        ///there is no interpolation for this as the grid data is dense enough that it 
                        ///is rare that there is a miss
                        fixed (double* mipData = mDataDouble)
                        {
                            int zOffest = mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                            int yOffset = mDataDouble.GetLength(2);
                            double* POut = (double*)mipData;
                            double* POutZ = POut;
                            double* POutY;
                            double* POutX;
                            for (int zI = 0; zI < LZ; zI++)
                            // while ( SmearArray2DQueue.Count>0)
                            {

                                z = (zI * stepZ + sZ);
                                aX = bZX * z + aZX;
                                aY = bZY * z + aZY;
                                tX1 = aX;
                                tY1 = aY;
                                POutY = POutZ;
                                for (int yI = 0; yI < LY; yI++)
                                {

                                    tX1 += bXY;
                                    tY1 += bYY;

                                    tXI = tX1;
                                    tYI = tY1;
                                    POutX = POutY;
                                    for (int xI = 0; xI < LX; xI++)
                                    {
                                        tXI += bXX;
                                        tYI += bYX;

                                        if (tXI > 0 && tXI < LOutX)
                                            if (tYI > 0 && tYI < LOutY)
                                            {
                                                // u = tXI % 1;
                                               
                                                Y = (int)Math.Round(tYI);
                                                // if (u>0.5)
                                                X = (int)Math.Round(tXI);
                                                //else
                                                //   X = (int)tXI+1;

                                                if (X > 0 && X < LOutX && Y > 0 && Y < LOutY)
                                                {
                                                    value =/* mDataDouble[zI, yI, xI];//*/ (*POutX);
                                                    if (value > PaintingArray[Y, X])
                                                        PaintingArray[Y, X] = value;

                                                }
                                                // if (tYI+1 <LOutY )
                                                //   PaintingArray[(int)tYI + 1, (int)tXI] += value * (1 - u);
                                            }
                                        POutX++;
                                    }
                                    POutY += yOffset;

                                }
                                POutZ += zOffest;

                            }
                        }
                        //SmearArray2DQueue = null;
                    }
                }
                return PaintingArray;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.StackTrace);
                System.Diagnostics.Debug.Print("");
                return null;
            }
        }


        /// <summary>
        /// Do the work of projecting the grid out onto the image
        /// </summary>
        /// <param name="DensityGrid"></param>
        /// <param name="Direction"></param>
        /// <param name="FastScanDirection"></param>
        /// <returns></returns>
        private static double[,] ForwardProject(double[, ,] DensityGrid, Point3D Direction, Point3D FastScanDirection)
        {
            try
            {
                double LengthCorner = 0;
                //determine the size of the output image
                int GridWidth = DensityGrid.GetLength(1);
                if (DensityGrid.GetLength(2) > GridWidth) GridWidth = DensityGrid.GetLength(2);

                LengthCorner = Math.Sqrt(2);

                double[,] PaintingArray = new double[(int)(DensityGrid.GetLength(0)), (int)(GridWidth)];

                ///get all the direction vectors correct
                Direction.Normalize();
                FastScanDirection.Normalize();
                Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

                Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
                Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
                normalX.Normalize();
                normalY.Normalize();

                double[, ,] mDataDouble = DensityGrid;

                int LX = mDataDouble.GetLength(2);
                int LY = mDataDouble.GetLength(1);
                int LZ = mDataDouble.GetLength(0);

                int LOutX = PaintingArray.GetLength(1);
                int LOutY = PaintingArray.GetLength(0);
                ///just assume a simple set of sizes for the motion 
                double sX = -1;
                double sY = -1;
                double sZ = -1;

                double sOutX = 2d / -2d; //PaintingArray.PhysicalStart(Axis.XAxis);
                double sOutY = 2d / -2d; //PaintingArray.PhysicalStart(Axis.YAxis);

                double stepOutX = 2d / PaintingArray.GetLength(1); //PaintingArray.PhysicalStep(Axis.XAxis);
                double stepOutY = 2d / PaintingArray.GetLength(0); //PaintingArray.PhysicalStep(Axis.YAxis);

                double stepX = 2d / DensityGrid.GetLength(2);// mPhysicalStep[2];
                double stepY = 2d / DensityGrid.GetLength(1);//mPhysicalStep[1];
                double stepZ = 2d / DensityGrid.GetLength(0);// mPhysicalStep[0];

                double z;
                double tXI, tYI, tX1, tY1;
                ///precalculate as many coeff as possible to avoid math inside the nastly loop
                double aX, aY, bXY, bXX, bYY, bYX;
                double aZX, bZX, aZY, bZY;

                bXX = normalX.Y * stepY / stepOutX;
                bXY = normalX.X * stepX / stepOutX;

                bYX = normalY.Y * stepY / stepOutY;
                bYY = normalY.X * stepX / stepOutY;

                aZX = (normalX.Y * sY - sOutX + normalX.X * sX) / stepOutX;
                bZX = normalX.Z / stepOutX;

                aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY;
                bZY = normalY.Z / stepOutY;

                double[] Normalization = new double[mDataDouble.GetLength(0)];
                double u, value;
                int X, Y;

                unchecked
                {
                    unsafe
                    {
                        ///walk through the loop, sending the grid data out to the image
                        ///there is no interpolation for this as the grid data is dense enough that it 
                        ///is rare that there is a miss
                        fixed (double* mipData = mDataDouble)
                        {
                            int zOffest = mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                            int yOffset = mDataDouble.GetLength(2);
                            double* POut = (double*)mipData;
                            double* POutZ = POut;
                            double* POutY;
                            double* POutX;
                            for (int zI = 0; zI < LZ; zI++)
                            // while ( SmearArray2DQueue.Count>0)
                            {

                                z = (zI * stepZ + sZ);
                                aX = bZX * z + aZX;
                                aY = bZY * z + aZY;
                                tX1 = aX;
                                tY1 = aY;
                                POutY = POutZ;
                                for (int yI = 0; yI < LY; yI++)
                                {

                                    tX1 += bXY;
                                    tY1 += bYY;

                                    tXI = tX1;
                                    tYI = tY1;
                                    POutX = POutY;
                                    for (int xI = 0; xI < LX; xI++)
                                    {
                                        tXI += bXX;
                                        tYI += bYX;

                                        if (tXI > 0 && tXI < LOutX)
                                            if (tYI > 0 && tYI < LOutY)
                                            {
                                                // u = tXI % 1;

                                                Y = (int)Math.Round(tYI);
                                                // if (u>0.5)
                                                X = (int)Math.Round(tXI);
                                                //else
                                                //   X = (int)tXI+1;

                                                if (X > 0 && X < LOutX && Y > 0 && Y < LOutY)
                                                {
                                                    value =/* mDataDouble[zI, yI, xI];//*/ (*POutX);
                                                    //if (value > PaintingArray[Y, X])
                                                        PaintingArray[Y, X] += value;

                                                }
                                                // if (tYI+1 <LOutY )
                                                //   PaintingArray[(int)tYI + 1, (int)tXI] += value * (1 - u);
                                            }
                                        POutX++;
                                    }
                                    POutY += yOffset;

                                }
                                POutZ += zOffest;

                            }
                        }
                        //SmearArray2DQueue = null;
                    }
                }
                return PaintingArray;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.StackTrace);
                System.Diagnostics.Debug.Print("");
                return null;
            }
        }


        /// <summary>
        /// Do the work of projecting the grid out onto the image
        /// </summary>
        /// <param name="DensityGrid"></param>
        /// <param name="Direction"></param>
        /// <param name="FastScanDirection"></param>
        /// <returns></returns>
        private static double[,] ProjectMIP(float [, ,] DensityGrid, Point3D Direction, Point3D FastScanDirection)
        {
            try
            {
                double LengthCorner = 0;
                //determine the size of the output image
                int GridWidth = DensityGrid.GetLength(1);
                if (DensityGrid.GetLength(2) > GridWidth) GridWidth = DensityGrid.GetLength(2);

                LengthCorner = Math.Sqrt(2);

                double[,] PaintingArray = new double[(int)(DensityGrid.GetLength(0)), (int)(GridWidth)];

                ///get all the direction vectors correct
                Direction.Normalize();
                FastScanDirection.Normalize();
                Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

                Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
                Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
                normalX.Normalize();
                normalY.Normalize();

                float[, ,] mDataDouble = DensityGrid;

                int LX = mDataDouble.GetLength(2);
                int LY = mDataDouble.GetLength(1);
                int LZ = mDataDouble.GetLength(0);

                int LOutX = PaintingArray.GetLength(1);
                int LOutY = PaintingArray.GetLength(0);
                ///just assume a simple set of sizes for the motion 
                double sX = -1;
                double sY = -1;
                double sZ = -1;

                double sOutX = 2d / -2d; //PaintingArray.PhysicalStart(Axis.XAxis);
                double sOutY = 2d / -2d; //PaintingArray.PhysicalStart(Axis.YAxis);

                double stepOutX = 2d / PaintingArray.GetLength(1); //PaintingArray.PhysicalStep(Axis.XAxis);
                double stepOutY = 2d / PaintingArray.GetLength(0); //PaintingArray.PhysicalStep(Axis.YAxis);

                double stepX = 2d / DensityGrid.GetLength(2);// mPhysicalStep[2];
                double stepY = 2d / DensityGrid.GetLength(1);//mPhysicalStep[1];
                double stepZ = 2d / DensityGrid.GetLength(0);// mPhysicalStep[0];

                double z;
                double tXI, tYI, tX1, tY1;
                ///precalculate as many coeff as possible to avoid math inside the nastly loop
                double aX, aY, bXY, bXX, bYY, bYX;
                double aZX, bZX, aZY, bZY;

                bXX = normalX.Y * stepY / stepOutX;
                bXY = normalX.X * stepX / stepOutX;

                bYX = normalY.Y * stepY / stepOutY;
                bYY = normalY.X * stepX / stepOutY;

                aZX = (normalX.Y * sY - sOutX + normalX.X * sX) / stepOutX;
                bZX = normalX.Z / stepOutX;

                aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY;
                bZY = normalY.Z / stepOutY;

                double[] Normalization = new double[mDataDouble.GetLength(0)];
                double u, value;
                int X, Y;

                unchecked
                {
                    unsafe
                    {
                        ///walk through the loop, sending the grid data out to the image
                        ///there is no interpolation for this as the grid data is dense enough that it 
                        ///is rare that there is a miss
                        fixed (float* mipData = mDataDouble)
                        {
                            int zOffest = mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                            int yOffset = mDataDouble.GetLength(2);
                            float* POut = (float*)mipData;
                            float* POutZ = POut;
                            float* POutY;
                            float* POutX;
                            for (int zI = 0; zI < LZ; zI++)
                            // while ( SmearArray2DQueue.Count>0)
                            {

                                z = (zI * stepZ + sZ);
                                aX = bZX * z + aZX;
                                aY = bZY * z + aZY;
                                tX1 = aX;
                                tY1 = aY;
                                POutY = POutZ;
                                for (int yI = 0; yI < LY; yI++)
                                {

                                    tX1 += bXY;
                                    tY1 += bYY;

                                    tXI = tX1;
                                    tYI = tY1;
                                    POutX = POutY;
                                    for (int xI = 0; xI < LX; xI++)
                                    {
                                        tXI += bXX;
                                        tYI += bYX;

                                        if (tXI > 0 && tXI < LOutX)
                                            if (tYI > 0 && tYI < LOutY)
                                            {
                                                // u = tXI % 1;

                                                Y = (int)Math.Round(tYI);
                                                // if (u>0.5)
                                                X = (int)Math.Round(tXI);
                                                //else
                                                //   X = (int)tXI+1;

                                                if (X > 0 && X < LOutX && Y > 0 && Y < LOutY)
                                                {
                                                    value = (*POutX);
                                                    if (value > PaintingArray[Y, X])
                                                        PaintingArray[Y, X] = value;

                                                }
                                                // if (tYI+1 <LOutY )
                                                //   PaintingArray[(int)tYI + 1, (int)tXI] += value * (1 - u);
                                            }
                                        POutX++;
                                    }
                                    POutY += yOffset;

                                }
                                POutZ += zOffest;

                            }
                        }
                        //SmearArray2DQueue = null;
                    }
                }
                return PaintingArray;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.StackTrace);
                System.Diagnostics.Debug.Print("");
                return null;
            }
        }

        /// <summary>
        /// Takes a recon volume and does a MIP projection from the desired axis around the Z axis
        /// </summary>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians"></param>
        /// <returns></returns>
        public static  double[,] DoMIPProjection_OneSlice(double[][,] DensityGrid, double AngleRadians)
        {
            //calculate the direction vectors needed to do the mip, these are defaults that assume that the rotation should be around the z axis and the fast scan axis is the one that matters
            double[,] SliceProjection;
            MathHelpLib.Axis RotationAxis = MathHelpLib.Axis.ZAxis;

            Point3D vRotationAxis = new Point3D();
            Point3D axis = new Point3D();

            if (RotationAxis == MathHelpLib.Axis.XAxis)
            {
                vRotationAxis = new Point3D(1, 0, 0);
                axis = new Point3D(0, 1, 0);
            }
            else if (RotationAxis == MathHelpLib.Axis.YAxis)
            {
                vRotationAxis = new Point3D(0, 1, 0);
                axis = new Point3D(0, 0, 1);
            }
            else if (RotationAxis == MathHelpLib.Axis.ZAxis)
            {
                vRotationAxis = new Point3D(0, 0, 1);
                axis = new Point3D(0, 1, 0);
            }

            Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, AngleRadians);

            SliceProjection = ProjectMIP(DensityGrid, vec, Point3D.CrossProduct(vec, vRotationAxis));
            return SliceProjection;

        }
        /// <summary>
        /// Do the work of projecting the grid out onto the image
        /// </summary>
        /// <param name="DensityGrid"></param>
        /// <param name="Direction"></param>
        /// <param name="FastScanDirection"></param>
        /// <returns></returns>
        private static  double[,] ProjectMIP(double[][,] DensityGrid, Point3D Direction, Point3D FastScanDirection)
        {
            double LengthCorner = 0;

            int GridWidth = DensityGrid[0].GetLength(0);
            if (DensityGrid[0].GetLength(1) > GridWidth) GridWidth = DensityGrid[0].GetLength(1);

            LengthCorner = Math.Sqrt(2);

            double[,] PaintingArray = new double[(int)(DensityGrid.GetLength(0)), (int)(GridWidth)];

            Direction.Normalize();
            FastScanDirection.Normalize();
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            double[][,] mDataDouble = DensityGrid;

            int LX = mDataDouble[0].GetLength(1);
            int LY = mDataDouble[0].GetLength(0);
            int LZ = mDataDouble.GetLength(0);

            int LOutX = PaintingArray.GetLength(1);
            int LOutY = PaintingArray.GetLength(0);

            double sX = -1;
            double sY = -1;
            double sZ = -1;

            double sOutX = 2d / -2d; //PaintingArray.PhysicalStart(Axis.XAxis);
            double sOutY = 2d / -2d; //PaintingArray.PhysicalStart(Axis.YAxis);

            double stepOutX = 2d / PaintingArray.GetLength(1); //PaintingArray.PhysicalStep(Axis.XAxis);
            double stepOutY = 2d / PaintingArray.GetLength(0); //PaintingArray.PhysicalStep(Axis.YAxis);

            double stepX = 2d / LX;
            double stepY = 2d / LY;
            double stepZ = 2d / LZ;

            double z;
            double tXI, tYI, tX1, tY1;

            double aX, aY, bXY, bXX, bYY, bYX;
            double aZX, bZX, aZY, bZY;

            bXX = normalX.Y * stepY / stepOutX;
            bXY = normalX.X * stepX / stepOutX;

            bYX = normalY.Y * stepY / stepOutY;
            bYY = normalY.X * stepX / stepOutY;

            aZX = (normalX.Y * sY - sOutX + normalX.X * sX) / stepOutX;
            bZX = normalX.Z / stepOutX;

            aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY;
            bZY = normalY.Z / stepOutY;

            double[] Normalization = new double[mDataDouble.GetLength(0)];
            double u, value;
            int X, Y;
            unsafe
            {
                ///walk through the loop, sending the grid data out to the image
                ///there is no interpolation for this as the grid data is dense enough that it 
                ///is rare that there is a miss
                int zOffest = LX * LY;
                int yOffset = LZ;

                for (int zI = 0; zI < LZ; zI++)
                // while ( SmearArray2DQueue.Count>0)
                {
                    fixed (double* mipData = mDataDouble[zI])
                    {
                        double* POut = (double*)mipData;
                        double* POutZ = POut;
                        double* POutY;
                        double* POutX;
                        z = (zI * stepZ + sZ);
                        aX = bZX * z + aZX;
                        aY = bZY * z + aZY;
                        tX1 = aX;
                        tY1 = aY;
                        POutY = POutZ;
                        for (int yI = 0; yI < LY; yI++)
                        {

                            tX1 += bXY;
                            tY1 += bYY;

                            tXI = tX1;
                            tYI = tY1;
                            POutX = POutY;
                            for (int xI = 0; xI < LX; xI++)
                            {
                                tXI += bXX;
                                tYI += bYX;

                                if (tXI > 0 && tXI < LOutX)
                                    if (tYI > 0 && tYI < LOutY)
                                    {
                                        // u = tXI % 1;
                                        value = (*POutX);
                                        Y = (int)Math.Round(tYI);
                                        // if (u>0.5)
                                        X = (int)Math.Round(tXI);
                                        //else
                                        //   X = (int)tXI+1;

                                        if (X < LOutX)
                                        {
                                            if (value > PaintingArray[Y, X])
                                                PaintingArray[Y, X] = value;
                                        }
                                        // if (tYI+1 <LOutY )
                                        //   PaintingArray[(int)tYI + 1, (int)tXI] += value * (1 - u);
                                    }
                                POutX++;
                            }
                            POutY += yOffset;

                        }
                        POutZ += zOffest;

                    }
                }
                //SmearArray2DQueue = null;
            }
            return PaintingArray;
        }

    }
}