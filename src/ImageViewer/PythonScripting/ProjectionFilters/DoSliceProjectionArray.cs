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



namespace ImageViewer.PythonScripting.Projection
{
    public class DoSliceBackProjectionEffectArray : aEffectNoForm
    {
        public override string EffectName { get { return "Project Slice Through 3D Array"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Projection"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }


        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mPassData = PassData;

            mFilterToken = Parameters;

            double[,] Slice = (double[,])mFilterToken[0];
            ProjectionArrayObject DensityGrid = (ProjectionArrayObject)mFilterToken[1];
            double Angle = EffectHelps.ConvertToDouble(mFilterToken[2]);

            DoBackProjection_OneSlice(Slice, DensityGrid.Width, DensityGrid.Height, DensityGrid, Angle, Axis2D.YAxis);

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


        public void DoBackProjection_OneSlice(double[,] Slice, double PaintingWidth, double PaintingHeight, ProjectionArrayObject DensityGrid, double AngleRadians, Axis2D ConvolutionAxis)
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

        private void SmearArray2D(ProjectionArrayObject DensityGrid, double[,] PaintingArray, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
        {

            bool[] SliceFinished = new bool[DensityGrid.Data.GetLength(0)];

            Direction.Normalize();
            FastScanDirection.Normalize();
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            double[][,] mDataDouble = DensityGrid.Data;

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
                                                Y = (int)Math.Round(tYI);
                                                X = (int)Math.Round(tXI);
                                                if (X < LOutX)
                                                    if (Y < LOutY)
                                                    {
                                                        *POutX += PaintingArray[Y, X];
                                                    }
                                            }
                                        POutX++;
                                    }
                                    POutY += yOffset;

                                }
                                POutZ += zOffest;
                            }

                        }
                        DensityGrid.LockIndicator[zI] = false;
                    }
                }
                //SmearArray2DQueue = null;
            }
        }


        /* private void SmearArray2DOld(ProjectionArrayObject DensityGrid, double[,] PaintingArray, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
         {
             lock (SmearCriticalSection)
             {
                 if (SafetyArray == null || SafetyArray.Length != DensityGrid.Data.GetLength(0))
                 {
                     SafetyArray = new object[DensityGrid.Data.GetLength(0)];
                     for (int i = 0; i < SafetyArray.Length; i++)
                         SafetyArray[i] = new object();
                 }
             }

             Direction.Normalize();
             FastScanDirection.Normalize();
             Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

             Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
             Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
             normalX.Normalize();
             normalY.Normalize();

             double[, ,] mDataDouble = DensityGrid.Data;

             int LX = mDataDouble.GetLength(2);
             int LY = mDataDouble.GetLength(1);
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

             double stepX = DensityGrid.Width / DensityGrid.Data.GetLength(2);// mPhysicalStep[2];
             double stepY = DensityGrid.Height / DensityGrid.Data.GetLength(1);//mPhysicalStep[1];
             double stepZ = DensityGrid.Depth / DensityGrid.Data.GetLength(0);// mPhysicalStep[0];

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
             int X, Y;
             unsafe
             {
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
                         //int zI = SmearArray2DQueue.Dequeue();
                         lock (SafetyArray[zI])
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
                                             Y = (int)Math.Round(tYI);
                                             X = (int)Math.Round(tXI);
                                             if (X < LOutX)
                                                 if (Y < LOutY)
                                                 {
                                                     *POutX += PaintingArray[Y, X];
                                                 }
                                         }
                                     POutX++;
                                 }
                                 POutY += yOffset;

                             }
                             POutZ += zOffest;
                         }
                     }
                 }
                 //SmearArray2DQueue = null;
             }
         }
         */
    }
}
