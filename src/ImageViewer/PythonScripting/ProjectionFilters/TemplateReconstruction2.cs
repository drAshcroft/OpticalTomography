using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathHelpLib.ProjectionFilters;
using MathHelpLib;
using System.Threading;
using System.Drawing;

namespace ImageViewer.PythonScripting.Projection
{
    public class TemplateReconstruction2
    {
        private class ColumnInfo
        {
            public Point ColIndex;
            public int PaintIndex;
            public double LeftWeight;
            public double RightWeight;
            public int sliceIndex;
        }
        double[, ,] mDataDouble;
        double[][,] mSlices;

        volatile bool ReconFinished = false;

        Thread[] ThreadPool = new Thread[48];
        Queue<ColumnInfo> ColumnStarts = new Queue<ColumnInfo>();

        object CheckLock = new object();

        private void DoColumn(object threadIndex)
        {
            while (ReconFinished == false)
            {
                //there is a small gap between the if and the dequeue, so we catch the erro and try again
                try
                {
                    ColumnInfo ci = null;
                    lock (CheckLock)
                    {
                        if (ColumnStarts.Count > 0)
                        {
                            ci = ColumnStarts.Dequeue();
                        }
                    }

                    if (ci != null)
                    {
                        double[,] tSlice = mSlices[ci.sliceIndex];
                        // Console.WriteLine(threadIndex.ToString() + " " + ci.ColIndex.ToString());
                        // System.Diagnostics.Debug.Print(threadIndex.ToString() + " " + ci.ColIndex.ToString());
                        int X = ci.ColIndex.X;
                        int Y = ci.ColIndex.Y;
                        int index = ci.PaintIndex;
                        double rW = ci.RightWeight;
                        double lW = ci.LeftWeight;
                        unchecked
                        {
                            for (int Z = 0; Z < mDataDouble.GetLength(0); Z++)
                            {
                                mDataDouble[Z, X, Y] += tSlice[Z, index] * rW + tSlice[Z, index + 1] * lW;
                            }
                        }
                    }
                }


                catch { }
                //Thread.Sleep(1);
            }

        }

        public void FBPReconstruction(DataEnvironment dataEnvironment, MathHelpLib.ProjectionFilters.ProjectionArrayObject DensityGrid)
        {
            mDataDouble = DensityGrid.DataWhole;
            double[] Kernal = DensityGrid.impulse;

            mSlices = new double[dataEnvironment.AllImages.Count][,];

            for (int i = 0; i < ThreadPool.Length; i++)
            {
                ThreadPool[i] = new Thread(DoColumn);
                ThreadPool[i].Start(i);
            }

            Queue<int> SlicesFinished = new Queue<int>();

            Thread Convolution = new Thread(delegate()
                {
                    for (int i = 0; i < dataEnvironment.AllImages.Count; i++)
                    {
                        mSlices[i] = Convolution1D.Convolve(dataEnvironment.AllImages[i].ImageData, Kernal);
                        SlicesFinished.Enqueue(i);
                    }
                }
            );
            Convolution.Start();

            int nSlices = 0;
            while (nSlices<dataEnvironment.AllImages.Count)
            {
                while (SlicesFinished.Count==0)
                    Thread.Sleep(100);

                int i = SlicesFinished.Dequeue();
                DoBackProjection_OneSlice(i, 1, 1, DensityGrid, 2 * Math.PI / (double)dataEnvironment.AllImages.Count * i, Axis2D.YAxis);
                
              //  Console.WriteLine(i.ToString());
                System.Diagnostics.Debug.Print(i.ToString());
                nSlices++;
            }

            while (ColumnStarts.Count > 0)
                Thread.Sleep(100);

            ReconFinished = true;

            for (int i = 0; i < ThreadPool.Length; i++)
            {
                ThreadPool[i].Join();
            }
        }

        private void DoBackProjection_OneSlice(int SliceIndex, double PaintingWidth, double PaintingHeight, ProjectionArrayObject DensityGrid, double AngleRadians, Axis2D ConvolutionAxis)
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

            SmearArray2DWhole(AngleRadians, DensityGrid, SliceIndex, PaintingWidth, PaintingHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));

        }

        private void SmearArray2DWhole(double AngleRadians, ProjectionArrayObject DensityGrid, int SliceIndex, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
        {
            double[,] PaintingArray = mSlices[SliceIndex];

            //make sure all the vectors are the right size
            Direction.Normalize();
            FastScanDirection.Normalize();
            //determine all the important planes in the 3D space and all the step
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            ///get the existing recon data.  the jagged array is used so the data can be checked out by each thread

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
                LUT[cc] = Math.Abs(mX1 - mX2);
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
            VolToSliceZ = (DensityGrid.ZMax - DensityGrid.ZMin) / LK * (double)PaintingArray.GetLength(1) / (2 * PaintingHeight);

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
                            {
                                lower_sI = (int)Math.Floor(sdotI);
                                u = sdotI - lower_sI;
                                lower_sJ = (int)((K_ - halfK) * VolToSliceZ + halfJs);//(int)Math.Floor(K_);

                                LUT_Index = (int)(u / 1.2);
                                if (LUT_Index > 49)
                                    LUT_Index = 49;

                                ColumnInfo ci = new ColumnInfo();
                                ci.ColIndex = new Point(I_, J_);
                                ci.LeftWeight = LUT[49 - LUT_Index]; ;
                                ci.RightWeight = LUT[LUT_Index];
                                ci.PaintIndex = lower_sI;
                                ci.sliceIndex = SliceIndex;

                                while (ColumnStarts.Count > 7000)
                                    Thread.Sleep(100);

                                // Console.WriteLine(ColumnStarts.Count.ToString());
                                lock (CheckLock)
                                {
                                    ColumnStarts.Enqueue(ci);
                                }
                            }
                        }
                    }

                }
            }
        }
        //SmearArray2DQueue = null;
    }




}
