using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using MathHelpLib;

using System.Runtime.InteropServices;
using System.Threading;



namespace MathHelpLib.Recon
{
    public class DoSliceBackProjectionEffect 
    {
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

            SmearArray2D(DensityGrid, Slice, PaintingWidth, PaintingHeight, AngleRadians, vec, Point3D.CrossProduct(vec, vRotationAxis));
        }

        static Random rnd = new Random();

        private static void SmearArray2D(ProjectionArrayObject DensityGrid, double[,] PaintingArray, double PaintingWidth, double PaintingHeight, double AngleRadians, Point3D Direction, Point3D FastScanDirection)
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


            //determine the travel path of the ray through the grid point
            AngleRadians = ((180d / Math.PI * AngleRadians) % 90) * Math.PI / 180d;
            double side1 = Math.Abs(Math.Cos(AngleRadians));
            if (side1 > .5)
                side1 = Math.Cos(Math.PI / 2 - AngleRadians);
            side1 =1/ Math.Sqrt(side1 * side1 + .5 * .5);

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

                                                        *POutX += (V1 * uX + V2 * (1 - uX))*side1;
                                                    }
                                                    else if (rY == LY_1)
                                                    {
                                                        // Console.WriteLine(rX + " " + rY + " " + LX_1 + " " + LY_1 );
                                                        *POutX += PaintingArray[rY, rX]*side1;
                                                    }
                                                }
                                                else if (rX == LX_1 && rY < LOutY)
                                                {
                                                    *POutX += PaintingArray[rY, rX]*side1;
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
