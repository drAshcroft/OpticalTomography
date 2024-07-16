using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

using MathHelpLib;




namespace MathHelpLib.Recon
{
    public class DoARTSliceProjectionEffect 
    {

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

        public static void DoForwardProjection_OneSlice(ref double[,] Slice, double PaintingWidth, double PaintingHeight, ProjectionArrayObject DensityGrid, double AngleRadians, Axis2D ConvolutionAxis)
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

            UnSmearArray2D(DensityGrid, ref  Slice, PaintingWidth, PaintingHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));
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

            //a lot of the coeffiences can be calculated outside the loop saving time and computation
            bXX = normalX.Y * stepY / stepOutX;
            bXY = normalX.X * stepX / stepOutX;

            bYX = normalY.Y * stepY / stepOutY;
            bYY = normalY.X * stepX / stepOutY;

            aZX = (normalX.Y * sY - sOutX + normalX.X * sX) / stepOutX;
            bZX = normalX.Z / stepOutX;

            aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY;
            bZY = normalY.Z / stepOutY;


            int X, Y;
            unsafe
            {
                //we make the math unchecked to make it go faster.  there seem to be enough checks beforehand
                unchecked
                {
                    int zOffest = LY * LX;
                    int yOffset = LX;

                    for (int zI = 0; zI < LZ; zI++)
                    {
                            //when all the slices are processed jump out
                            //indicate that this slice has been processed.
                            SliceFinished[zI] = true;

                            fixed (double* mipData = mDataDouble[zI])
                            {

                                double* POut = (double*)mipData;
                                double* POutZ = POut;
                                double* POutY;
                                double* POutX;
                                double uX, uY, vX, vY, V1, V2, V;
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
                                        if (tXI > 0 && tXI < LOutX)
                                            if (tYI > 0 && tYI < LOutY)
                                            {
                                                Y = (int)Math.Round(tYI);
                                                X = (int)Math.Round(tXI);
                                                if (X < LOutX - 1)
                                                    if (Y < LOutY - 1)
                                                    {
                                                        //distribute out the pixel to the contributing pixels
                                                        uX = tXI - X;
                                                        uY = tYI - Y;
                                                        vX = 1 - uX;
                                                        vY = 1 - uY;

                                                        V1 = PaintingArray[Y, X];// *uX + PaintingArray[Y, X + 1] * vX;
                                                       // V2 = PaintingArray[Y + 1, X] * uX + PaintingArray[Y + 1, X + 1] * vX;

                                                        *POutX += V1;// *uX + V2 * vX;
                                                    }
                                            }
                                        POutX++;
                                    }
                                    POutY += yOffset;

                                }
                                POutZ += zOffest;
                            }

                        //release the programatic handle to 
                        DensityGrid.LockIndicator[zI] = false;
                    }
                }
                //SmearArray2DQueue = null;
            }
        }

        private static void UnSmearArray2D(ProjectionArrayObject DensityGrid, ref  double[,] UnPaintedArray, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
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

            double[,] PixelCount = new double[UnPaintedArray.GetLength(0), UnPaintedArray.GetLength(1)];

            //get all the dimensions
            int LX = mDataDouble[0].GetLength(1);
            int LY = mDataDouble[0].GetLength(0);
            int LZ = mDataDouble.GetLength(0);

            int LOutX = UnPaintedArray.GetLength(1);
            int LOutY = UnPaintedArray.GetLength(0);

            double sX = DensityGrid.XMin;// mPhysicalStart[2];
            double sY = DensityGrid.YMin;// mPhysicalStart[1];
            double sZ = DensityGrid.ZMin;// mPhysicalStart[0];

            double sOutX = PaintingWidth / -2d; //PaintingArray.PhysicalStart(Axis.XAxis);
            double sOutY = PaintingHeight / -2d; //PaintingArray.PhysicalStart(Axis.YAxis);

            double stepOutX = PaintingWidth / UnPaintedArray.GetLength(1); //PaintingArray.PhysicalStep(Axis.XAxis);
            double stepOutY = PaintingHeight / UnPaintedArray.GetLength(0); //PaintingArray.PhysicalStep(Axis.YAxis);

            double stepX = DensityGrid.Width / LX;// mPhysicalStep[2];
            double stepY = DensityGrid.Height / LY;//mPhysicalStep[1];
            double stepZ = DensityGrid.Depth / LZ;// mPhysicalStep[0];

            double z;
            double tXI, tYI, tX1, tY1;

            double aX, aY, bXY, bXX, bYY, bYX;
            double aZX, bZX, aZY, bZY;

            //a lot of the coeffiences can be calculated outside the loop saving time and computation
            bXX = normalX.Y * stepY / stepOutX;
            bXY = normalX.X * stepX / stepOutX;

            bYX = normalY.Y * stepY / stepOutY;
            bYY = normalY.X * stepX / stepOutY;

            aZX = (normalX.Y * sY - sOutX + normalX.X * sX) / stepOutX;
            bZX = normalX.Z / stepOutX;

            aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY;
            bZY = normalY.Z / stepOutY;


            int X, Y;
            unsafe
            {
                //we make the math unchecked to make it go faster.  there seem to be enough checks beforehand
                unchecked
                {
                    int zOffest = LY * LX;
                    int yOffset = LX;

                    for (int zI = 0; zI < LZ; zI++)
                    {
                            //indicate that this slice has been processed.
                            SliceFinished[zI] = true;
                            fixed (double* mipData = mDataDouble[zI])
                            {

                                double* POut = (double*)mipData;
                                double* POutZ = POut;
                                double* POutY;
                                double* POutX;
                                double uX, uY, vX, vY, V1, V2;
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
                                        if (tXI > 0 && tXI < LOutX)
                                            if (tYI > 0 && tYI < LOutY)
                                            {
                                                Y = (int)Math.Round(tYI);
                                                X = (int)Math.Round(tXI);
                                                if (X < LOutX - 1)
                                                    if (Y < LOutY - 1)
                                                    {
                                                        //distribute out the pixel to the contributing pixels
                                                        uX = tXI - X;
                                                        uY = tYI - Y;
                                                        vX = 1 - uX;
                                                        vY = 1 - uY;
                                                        V1 = *POutX * 1;// uX;
                                                        V2 = *POutX * vX;

                                                        //these are needed to normalize the thing
                                                        PixelCount[Y, X] += 1;// uX* uY;
                                                        /*PixelCount[Y, X + 1] += vX * uY;
                                                        PixelCount[Y + 1, X] += uX * vY;
                                                        PixelCount[Y + 1, X + 1] += vX * vY;*/

                                                        //record the values
                                                        UnPaintedArray[Y, X] += V1;
                                                        /* UnPaintedArray[Y, X + 1] += V2 * uY;
                                                         UnPaintedArray[Y + 1, X] += V1 * vY;
                                                         UnPaintedArray[Y + 1, X + 1] += V2 * vY;*/

                                                    }
                                            }
                                        POutX++;
                                    }
                                    POutY += yOffset;

                                }
                                POutZ += zOffest;

                        }

                        //release the programatic handle to 
                        DensityGrid.LockIndicator[zI] = false;
                    }
                }
                //SmearArray2DQueue = null;
            }


            //normalize the count
         /*   unsafe
            {
                unchecked
                {
                    fixed (double* pFinal = UnPaintedArray)
                    fixed (double* pCount = PixelCount)
                    {
                        double* pout = pFinal;
                        double* pC = pCount;
                        for (int i = 0; i < UnPaintedArray.Length; i++)
                        {
                            *pout /= (*pC+1);
                            pout++;
                            pC++;
                        }
                    }
                }
            }*/






        }

    }
}
