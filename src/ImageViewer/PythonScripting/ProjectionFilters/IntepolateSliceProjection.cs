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
    public class InterpolateSliceEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Expand Slice For Projection"; } }
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
        /// streches out a projection based on its angle and the recon grid.
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData">returns "ExpandedArray"</param>
        /// <param name="Parameters">pseudoprojection as physical array or double[,]; densitygrid as physical array or ProjectionArrayObject; angle as radians</param>
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
                double Angle = (double)mFilterToken[2];
              
                mPassData.AddSafe("ExpandedArray", DoInterpolation(Slice,DensityGrid,Angle,20,InterpolationMethod.Cosine ));
            }
            else if (mFilterToken[1].GetType() == typeof(double[, ,]))
            {
                double[,] Slice = (double[,])mFilterToken[0];
                double[, ,] DensityGrid = (double[, ,])mFilterToken[1];
                double Angle = (double)mFilterToken[2];

                mPassData.AddSafe("ExpandedArray", DoInterpolation(Slice,2,2, DensityGrid,2,2, Angle, 20, InterpolationMethod.Cosine));

            }
            else if (mFilterToken[1].GetType() == typeof(double[][,]))
            {
                double[,] Slice = (double[,])mFilterToken[0];
                double[][,] DensityGrid = (double[][,])mFilterToken[1];
                double Angle = (double)mFilterToken[2];

                mPassData.AddSafe("ExpandedArray", DoInterpolation(Slice,2,2, DensityGrid,2,2, Angle, 20, InterpolationMethod.Cosine));

            }
            else if (mFilterToken[1].GetType() == typeof(ProjectionArrayObject))
            {
                ProjectionArrayObject PAO = (ProjectionArrayObject)mFilterToken[1];
                if (PAO.Data ==null )
                {
                    double[,] Slice = (double[,])mFilterToken[0];
                    double[, ,] DensityGrid = PAO.DataWhole;
                    double Angle = (double)mFilterToken[2];

                    mPassData.AddSafe("ExpandedArray", DoInterpolation(Slice, 2, 2, DensityGrid, 2, 2, Angle, 20, InterpolationMethod.Cosine));
                }
                else
                {
                    double[,] Slice = (double[,])mFilterToken[0];
                    double[][,] DensityGrid = PAO.Data;
                    double Angle = (double)mFilterToken[2];

                    mPassData.AddSafe("ExpandedArray", DoInterpolation(Slice, 2, 2, DensityGrid, 2, 2, Angle, 20, InterpolationMethod.Cosine));
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

        public enum InterpolationMethod
        {
            Linear,
            Cosine,
            Cubic,
            Haskel

        }

        /// <summary>
        /// Stretchs out a pseudo projection on the x axis by a value determined by maxexpansion and the recon grid
        /// </summary>
        /// <param name="Slice"></param>
        /// <param name="DensityGrid"></param>
        /// <param name="Angle"></param>
        /// <param name="maxExpansion"></param>
        /// <param name="interpolationMethod"></param>
        /// <returns></returns>
        public static PhysicalArray DoInterpolation(PhysicalArray Slice, PhysicalArray DensityGrid, double Angle, double maxExpansion, InterpolationMethod interpolationMethod)
        {
            double MaxPossibleExpansion = 4 * MaxExpansion(Slice, DensityGrid, Angle, Axis2D.YAxis);
            if (MaxPossibleExpansion > maxExpansion )
                MaxPossibleExpansion = maxExpansion ;

            if (MaxPossibleExpansion > 1)
                Slice = ExpandArray(Slice, MaxPossibleExpansion,interpolationMethod );
            return Slice;
        }

        /// <summary>
        /// Stretchs out a pseudo projection on the x axis by a value determined by maxexpansion and the recon grid
        /// </summary>
        /// <param name="Slice"></param>
        /// <param name="DensityGrid"></param>
        /// <param name="Angle"></param>
        /// <param name="maxExpansion"></param>
        /// <param name="interpolationMethod"></param>
        /// <returns></returns>
        public static double[,] DoInterpolation(double[,] Slice,double SliceWidth,double SliceHeight, double[, ,] DensityGrid,double gridWidth, double gridHeight, double Angle, double maxExpansion, InterpolationMethod interpolationMethod)
        {
            double MaxPossibleExpansion = 4 * MaxExpansion(Slice,SliceWidth,SliceHeight , DensityGrid,gridWidth,gridHeight, Angle, Axis2D.YAxis);
            if (MaxPossibleExpansion > maxExpansion)
                MaxPossibleExpansion = maxExpansion;

            if (MaxPossibleExpansion > 1)
                Slice = ExpandArray(Slice, MaxPossibleExpansion, interpolationMethod);
            return Slice;
        }

        /// <summary>
        /// Stretchs out a pseudo projection on the x axis by a value determined by maxexpansion and the recon grid
        /// </summary>
        /// <param name="Slice"></param>
        /// <param name="DensityGrid"></param>
        /// <param name="Angle"></param>
        /// <param name="maxExpansion"></param>
        /// <param name="interpolationMethod"></param>
        /// <returns></returns>
        public static double[,] DoInterpolation(double[,] Slice,double SliceWidth,double SliceHeight, double[][,] DensityGrid,double gridWidth, double gridHeight, double Angle, double maxExpansion, InterpolationMethod interpolationMethod)
        {
            double MaxPossibleExpansion = 4 * MaxExpansion(Slice, SliceWidth, SliceHeight, DensityGrid, gridWidth, gridHeight, Angle, Axis2D.YAxis);
            if (MaxPossibleExpansion > maxExpansion)
                MaxPossibleExpansion = maxExpansion;

            if (MaxPossibleExpansion > 1)
                Slice = ExpandArray(Slice, MaxPossibleExpansion, interpolationMethod);
            return Slice;
        }

        /// <summary>
        /// Stretchs a image out on the fast scan axis by the desired expansion factor.  Some interpolation methods are a lot faster than others
        /// </summary>
        /// <param name="SourceArray"></param>
        /// <param name="ExpansionFactor"></param>
        /// <param name="interpolationMethod"></param>
        /// <returns></returns>
        private static PhysicalArray ExpandArray(PhysicalArray SourceArray, double ExpansionFactor, InterpolationMethod interpolationMethod)
        {
            double[, ,] SourceData = SourceArray.ReferenceDataDouble;
            //build the final grid
            PhysicalArray ExpandedArray = new PhysicalArray(
                SourceArray.GetLength(Axis.XAxis), (int)(SourceArray.GetLength(Axis.YAxis) * ExpansionFactor),
                SourceArray.PhysicalStart(Axis.XAxis), SourceArray.PhysicalEnd(Axis.XAxis),
                SourceArray.PhysicalStart(Axis.YAxis), SourceArray.PhysicalEnd(Axis.YAxis));

            
            //get the data
            double[, ,] OutData = ExpandedArray.ReferenceDataDouble;

            //determine the expansion amount
            double StepX = (double)OutData.GetLength(2) / SourceData.GetLength(2);
            StepX = 1 / StepX;

            switch (interpolationMethod)
            {
                case InterpolationMethod.Linear:
                    {
                        #region Linear
                        //determine the coeff for expansion once
                        double[] mu2 = new double[OutData.GetLength(2)];
                        int[] Xs = new int[OutData.GetLength(2)];
                        double mu;
                        for (int i = 0; i < OutData.GetLength(2); i++)
                        {
                            mu = (i * StepX) % 1;
                            mu2[i] = mu;
                            Xs[i] = (int)Math.Truncate(i * StepX);

                        }
                        //now apply across the whole image
                        for (int i = 0; i < OutData.GetLength(2) - 1; i++)
                        {
                            for (int j = 0; j < OutData.GetLength(1); j++)
                            {
                                try
                                {
                                    OutData[0, j, i] = SourceData[0, j, Xs[i]] * (1 - mu2[i]) + SourceData[0, j, Xs[i] + 1] * (mu2[i]);
                                }
                                catch
                                {
                                    if (i < 4)
                                        OutData[0, j, i] = SourceData[0, j, 0];
                                    else
                                        OutData[0, j, i] = SourceData[0, j, SourceData.GetLength(2) - 1];
                                }
                            }
                        }
                        return ExpandedArray;
                        #endregion
                    }
                case InterpolationMethod.Cosine:
                    {
                        #region Cosine
                        //calc the coeff for the top line
                        double[] mu2 = new double[OutData.GetLength(2)];
                        int[] Xs = new int[OutData.GetLength(2)];
                        double mu;
                        double y;
                        int SourceLength = SourceData.GetLength(2) - 2;
                        for (int i = 0; i < OutData.GetLength(2); i++)
                        {
                            mu = (i * StepX) % 1;
                            mu2[i] = (1 - Math.Cos(mu * Math.PI)) / 2;
                            Xs[i] = (int)Math.Truncate(i * StepX);

                        }
                        int x;
                        //apply these through the whole image
                        for (int i = 0; i < OutData.GetLength(2) - 1; i++)
                        {
                            for (int j = 0; j < OutData.GetLength(1); j++)
                            {

                                x = Xs[i];
                                if (x < SourceLength)
                                {
                                    mu = mu2[i];
                                    y = SourceData[0, j, x] * (1 - mu) + SourceData[0, j, x + 1] * (mu); ;
                                    OutData[0, j, i] = y;
                                }
                                else
                                    OutData[0, j, i] = SourceData[0, j, SourceLength + 1];

                            }
                        }
                        return ExpandedArray;
                        #endregion
                    }
                case InterpolationMethod.Cubic:
                    {
                        #region Cubic
                        //not much to calc here, but do what we can
                        double[] mu2 = new double[OutData.GetLength(2)];
                        int[] Xs = new int[OutData.GetLength(2)];
                        double mu;
                        for (int i = 0; i < OutData.GetLength(2); i++)
                        {
                            mu = (i * StepX) % 1;
                            mu2[i] = mu;
                            Xs[i] = (int)Math.Truncate(i * StepX);

                        }

                        //do the nice cubic splines
                        double a0, a1, a2, a3;
                        double y0, y1, y2, y3;
                        for (int i = 0; i < OutData.GetLength(2); i++)
                        {
                            for (int j = 0; j < OutData.GetLength(1); j++)
                            {
                                try
                                {
                                    y0 = SourceData[0, j, Xs[i] - 1];
                                    y1 = SourceData[0, j, Xs[i]];
                                    y2 = SourceData[0, j, Xs[i] + 1];
                                    y3 = SourceData[0, j, Xs[i] + 2];

                                    a0 = y3 - y2 - y0 + y1;
                                    a1 = y0 - y1 - a0;
                                    a2 = y2 - y0;
                                    a3 = y1;

                                    OutData[0, j, i] = a0 * mu2[i] * mu2[i] * mu2[i] + a1 * mu2[i] * mu2[i] + a2 * mu2[i] + a3;
                                }
                                catch
                                {
                                    if (i < 4)
                                        OutData[0, j, i] = SourceData[0, j, 0];
                                    else
                                        OutData[0, j, i] = SourceData[0, j, SourceData.GetLength(2) - 1];
                                }
                            }
                        }
                        return ExpandedArray;
                        #endregion
                    }
                case InterpolationMethod.Haskel:
                    {
                        #region Haskel
                        double[] mu2 = new double[OutData.GetLength(2)];
                        int[] Xs = new int[OutData.GetLength(2)];
                        double mu;
                        for (int i = 0; i < OutData.GetLength(2); i++)
                        {
                            mu = (i * StepX) % 1;
                            mu2[i] = mu;
                            Xs[i] = (int)Math.Truncate(i * StepX);

                        }
                        double a0, a1, a2, a3;
                        double y0, y1, y2, y3;
                        double m0, m1, mu_2, mu3;
                        double bias = 0, tension = 0;//both range between -1 and 1
                        for (int i = 0; i < OutData.GetLength(2); i++)
                        {
                            for (int j = 0; j < OutData.GetLength(1); j++)
                            {
                                try
                                {
                                    y0 = SourceData[0, j, Xs[i] - 1];
                                    y1 = SourceData[0, j, Xs[i]];
                                    y2 = SourceData[0, j, Xs[i] + 1];
                                    y3 = SourceData[0, j, Xs[i] + 2];

                                    mu_2 = mu2[i] * mu2[i];
                                    mu3 = mu_2 * mu2[i];
                                    m0 = (y1 - y0) * (1 + bias) * (1 - tension) / 2;
                                    m0 += (y2 - y1) * (1 - bias) * (1 - tension) / 2;
                                    m1 = (y2 - y1) * (1 + bias) * (1 - tension) / 2;
                                    m1 += (y3 - y2) * (1 - bias) * (1 - tension) / 2;


                                    a0 = 2 * mu3 - 3 * mu_2 + 1;
                                    a1 = mu3 - 2 * mu_2 + mu2[i];
                                    a2 = mu3 - mu_2;
                                    a3 = -2 * mu3 + 3 * mu_2;

                                    OutData[0, j, i] = a0 * y1 + a1 * m0 + a2 * m1 + a3 * y2;
                                }
                                catch
                                {
                                    if (i < 4)
                                        OutData[0, j, i] = SourceData[0, j, 0];
                                    else
                                        OutData[0, j, i] = SourceData[0, j, SourceData.GetLength(2) - 1];
                                }
                            }
                        }
                        return ExpandedArray;
                        #endregion
                    }
            }
            return null;
        }

        /// <summary>
        /// Calculates all the needed angles and then passes the information to smear array to find the grid spacing needed for this method
        /// </summary>
        /// <param name="Slice"></param>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians"></param>
        /// <param name="ConvolutionAxis"></param>
        /// <returns></returns>
        private static double MaxExpansion(PhysicalArray Slice, PhysicalArray DensityGrid, double AngleRadians, Axis2D ConvolutionAxis)
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


            return 1 / SmearArray(DensityGrid, Slice, vec, Point3D.CrossProduct(vec, vRotationAxis));

        }

        /// <summary>
        /// checks the meshing of the image and the grid on just one plane.  the minimum interpolation spacing is then determined
        /// </summary>
        /// <param name="DensityGrid"></param>
        /// <param name="PaintingArray"></param>
        /// <param name="Direction"></param>
        /// <param name="FastScanDirection"></param>
        /// <returns></returns>
        private static double SmearArray(PhysicalArray DensityGrid, PhysicalArray PaintingArray, Point3D Direction, Point3D FastScanDirection)
        {
            List<double> Coords = new List<double>();

            Direction.Normalize();
            FastScanDirection.Normalize();
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            int LX = DensityGrid.GetLength(Axis.XAxis);
            int LY = DensityGrid.GetLength(Axis.YAxis);
            int LZ = DensityGrid.GetLength(Axis.ZAxis);

            int LOutX = PaintingArray.GetLength(Axis.XAxis) - 1;
            int LOutY = PaintingArray.GetLength(Axis.YAxis) - 1;

            double sX = DensityGrid.PhysicalStart(Axis.XAxis);//mPhysicalStart[2];
            double sY = DensityGrid.PhysicalStart(Axis.YAxis);//mPhysicalStart[1];
            double sZ = DensityGrid.PhysicalStart(Axis.ZAxis);//mPhysicalStart[0];

            double sOutX = PaintingArray.PhysicalStart(Axis.XAxis);
            double sOutY = PaintingArray.PhysicalStart(Axis.YAxis);
            double stepOutX = PaintingArray.PhysicalStep(Axis.XAxis);
            double stepOutY = PaintingArray.PhysicalStep(Axis.YAxis);

            double stepX = DensityGrid.PhysicalStep(Axis.XAxis);//mPhysicalStep[2];
            double stepY = DensityGrid.PhysicalStep(Axis.YAxis);//mPhysicalStep[1];
            double stepZ = DensityGrid.PhysicalStep(Axis.ZAxis);//mPhysicalStep[0];

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

            aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY; ;
            bZY = normalY.Z / stepOutY;

            // double uX, uY, sum, Xu, Yu;
            ///this works by defining a plane normalX normalY that is placed under the screen.  Then each voxel in the cube is 
            ///projected by defining the vector R from the origin to the voxel.  the projection coordinates are then determined 
            ///by taking the dot product of the normals and R.

            int zOffest = DensityGrid.GetLength(Axis.XAxis) * DensityGrid.GetLength(Axis.YAxis);
            int yOffset = DensityGrid.GetLength(Axis.XAxis);
            //int xL, yL;
            for (int zI = LZ / 2; zI < LZ / 2 + 1; zI++)
            {
                z = (zI * stepZ + sZ);
                aX = bZX * z + aZX;
                aY = bZY * z + aZY;
                tX1 = aX;
                tY1 = aY;
                for (int yI = 0; yI < LY; yI++)
                {
                    tX1 += bXY;
                    tY1 += bYY;

                    tXI = tX1;
                    tYI = tY1;
                    for (int xI = 0; xI < LX; xI++)
                    {
                        tXI += bXX;
                        tYI += bYX;

                        if (tXI > 0 && tXI < LOutX)
                            if (tYI > 0 && tYI < LOutY)
                            {
                                //xL = (int)Math.Floor(tXI);
                                //yL = (int)Math.Floor(tYI);
                                Coords.Add(tXI);
                            }
                    }
                }
            }

            ///take all the recorded x coords, sort them and then find the minimum distance between the coords
            Coords.Sort();
            double MinDist = double.MaxValue;
            double lCoord = Coords[0];
            double d;
            for (int i = 1; i < Coords.Count; i++)
            {
                d = Coords[i] - lCoord;
                if (d > 0 && d < MinDist) MinDist = d;
                lCoord = Coords[i];
            }
            return MinDist;
        }

        /// <summary>
        /// Stretchs a image out on the fast scan axis by the desired expansion factor.  Some interpolation methods are a lot faster than others
        /// </summary>
        /// <param name="SourceArray"></param>
        /// <param name="ExpansionFactor"></param>
        /// <param name="interpolationMethod"></param>
        /// <returns></returns>
        private static double[,] ExpandArray(double[,] SourceArray, double ExpansionFactor, InterpolationMethod interpolationMethod)
        {
            double[,] SourceData = SourceArray;
            double[,] ExpandedArray = new double[SourceArray.GetLength(0), (int)(SourceArray.GetLength(0) * ExpansionFactor)];

            double StepX = (double)ExpandedArray.GetLength(1) / SourceData.GetLength(1);
            StepX = 1 / StepX;

            switch (interpolationMethod)
            {
                case InterpolationMethod.Linear:
                    {
                        #region Linear
                        double[] mu2 = new double[ExpandedArray.GetLength(1)];
                        int[] Xs = new int[ExpandedArray.GetLength(1)];
                        double mu;
                        for (int i = 0; i < ExpandedArray.GetLength(1); i++)
                        {
                            mu = (i * StepX) % 1;
                            mu2[i] = mu;
                            Xs[i] = (int)Math.Truncate(i * StepX);

                        }
                        for (int i = 0; i < ExpandedArray.GetLength(1) - 1; i++)
                        {
                            for (int j = 0; j < ExpandedArray.GetLength(0); j++)
                            {
                                try
                                {
                                    ExpandedArray[j, i] = SourceData[j, Xs[i]] * (1 - mu2[i]) + SourceData[j, Xs[i] + 1] * (mu2[i]);
                                }
                                catch
                                {
                                    if (i < 4)
                                        ExpandedArray[j, i] = SourceData[j, 0];
                                    else
                                        ExpandedArray[j, i] = SourceData[j, SourceData.GetLength(1) - 1];
                                }
                            }
                        }
                        return ExpandedArray;
                        #endregion
                    }
                case InterpolationMethod.Cosine:
                    {
                        #region Cosine

                        double[] mu2 = new double[ExpandedArray.GetLength(1)];
                        int[] Xs = new int[ExpandedArray.GetLength(1)];
                        double mu;
                        double y;
                        int SourceLength = SourceData.GetLength(1) - 2;
                        for (int i = 0; i < ExpandedArray.GetLength(1); i++)
                        {
                            mu = (i * StepX) % 1;
                            mu2[i] = (1 - Math.Cos(mu * Math.PI)) / 2;
                            Xs[i] = (int)Math.Truncate(i * StepX);

                        }
                        int x;
                        for (int i = 0; i < ExpandedArray.GetLength(1) - 1; i++)
                        {
                            for (int j = 0; j < ExpandedArray.GetLength(0); j++)
                            {

                                x = Xs[i];
                                if (x < SourceLength)
                                {
                                    mu = mu2[i];
                                    y = SourceData[j, x] * (1 - mu) + SourceData[j, x + 1] * (mu); ;
                                    ExpandedArray[j, i] = y;
                                }
                                else
                                    ExpandedArray[j, i] = SourceData[j, SourceLength + 1];

                            }
                        }
                        return ExpandedArray;
                        #endregion
                    }
                case InterpolationMethod.Cubic:
                    {
                        #region Cubic
                        double[] mu2 = new double[ExpandedArray.GetLength(1)];
                        int[] Xs = new int[ExpandedArray.GetLength(1)];
                        double mu;
                        for (int i = 0; i < ExpandedArray.GetLength(1); i++)
                        {
                            mu = (i * StepX) % 1;
                            mu2[i] = mu;
                            Xs[i] = (int)Math.Truncate(i * StepX);

                        }
                        double a0, a1, a2, a3;
                        double y0, y1, y2, y3;
                        for (int i = 0; i < ExpandedArray.GetLength(1); i++)
                        {
                            for (int j = 0; j < ExpandedArray.GetLength(0); j++)
                            {
                                try
                                {
                                    y0 = SourceData[j, Xs[i] - 1];
                                    y1 = SourceData[j, Xs[i]];
                                    y2 = SourceData[j, Xs[i] + 1];
                                    y3 = SourceData[j, Xs[i] + 2];

                                    a0 = y3 - y2 - y0 + y1;
                                    a1 = y0 - y1 - a0;
                                    a2 = y2 - y0;
                                    a3 = y1;

                                    ExpandedArray[j, i] = a0 * mu2[i] * mu2[i] * mu2[i] + a1 * mu2[i] * mu2[i] + a2 * mu2[i] + a3;
                                }
                                catch
                                {
                                    if (i < 4)
                                        ExpandedArray[j, i] = SourceData[j, 0];
                                    else
                                        ExpandedArray[j, i] = SourceData[j, SourceData.GetLength(1) - 1];
                                }
                            }
                        }
                        return ExpandedArray;
                        #endregion
                    }
                case InterpolationMethod.Haskel:
                    {
                        #region Haskel
                        double[] mu2 = new double[ExpandedArray.GetLength(1)];
                        int[] Xs = new int[ExpandedArray.GetLength(1)];
                        double mu;
                        for (int i = 0; i < ExpandedArray.GetLength(1); i++)
                        {
                            mu = (i * StepX) % 1;
                            mu2[i] = mu;
                            Xs[i] = (int)Math.Truncate(i * StepX);

                        }
                        double a0, a1, a2, a3;
                        double y0, y1, y2, y3;
                        double m0, m1, mu_2, mu3;
                        double bias = 0, tension = 0;//both range between -1 and 1
                        for (int i = 0; i < ExpandedArray.GetLength(1); i++)
                        {
                            for (int j = 0; j < ExpandedArray.GetLength(0); j++)
                            {
                                try
                                {
                                    y0 = SourceData[j, Xs[i] - 1];
                                    y1 = SourceData[j, Xs[i]];
                                    y2 = SourceData[j, Xs[i] + 1];
                                    y3 = SourceData[j, Xs[i] + 2];

                                    mu_2 = mu2[i] * mu2[i];
                                    mu3 = mu_2 * mu2[i];
                                    m0 = (y1 - y0) * (1 + bias) * (1 - tension) / 2;
                                    m0 += (y2 - y1) * (1 - bias) * (1 - tension) / 2;
                                    m1 = (y2 - y1) * (1 + bias) * (1 - tension) / 2;
                                    m1 += (y3 - y2) * (1 - bias) * (1 - tension) / 2;


                                    a0 = 2 * mu3 - 3 * mu_2 + 1;
                                    a1 = mu3 - 2 * mu_2 + mu2[i];
                                    a2 = mu3 - mu_2;
                                    a3 = -2 * mu3 + 3 * mu_2;

                                    ExpandedArray[j, i] = a0 * y1 + a1 * m0 + a2 * m1 + a3 * y2;
                                }
                                catch
                                {
                                    if (i < 4)
                                        ExpandedArray[j, i] = SourceData[j, 0];
                                    else
                                        ExpandedArray[j, i] = SourceData[j, SourceData.GetLength(1) - 1];
                                }
                            }
                        }
                        return ExpandedArray;
                        #endregion
                    }
            }
            return null;
        }

        /// <summary>
        /// Calculates all the needed angles and then passes the information to smear array to find the grid spacing needed for this method
        /// </summary>
        /// <param name="Slice"></param>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians"></param>
        /// <param name="ConvolutionAxis"></param>
        /// <returns></returns>
        private static double MaxExpansion(double[,] Slice, double SliceWidth, double SliceHeight, double[, ,] DensityGrid, double GridWidth, double GridHeight, double AngleRadians, Axis2D ConvolutionAxis)
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


            return 1 / SmearArray(DensityGrid,GridWidth,GridHeight , Slice,SliceWidth,SliceHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));

        }

        /// <summary>
        /// checks the meshing of the image and the grid on just one plane.  the minimum interpolation spacing is then determined
        /// </summary>
        /// <param name="DensityGrid"></param>
        /// <param name="PaintingArray"></param>
        /// <param name="Direction"></param>
        /// <param name="FastScanDirection"></param>
        /// <returns></returns>
        private static double SmearArray(double[, ,] DensityGrid, double GridWidth, double GridHeight, double[,] PaintingArray, double SliceWidth, double SliceHeight, Point3D Direction, Point3D FastScanDirection)
        {
            List<double> Coords = new List<double>();

            Direction.Normalize();
            FastScanDirection.Normalize();
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            int LX = DensityGrid.GetLength(2);
            int LY = DensityGrid.GetLength(1);
            int LZ = DensityGrid.GetLength(0);

            int LOutX = PaintingArray.GetLength(1) - 1;
            int LOutY = PaintingArray.GetLength(0) - 1;

            double sX =GridWidth/-2d;//mPhysicalStart[2];
            double sY = GridWidth / -2d;//mPhysicalStart[1];
            double sZ = GridHeight / -2d;//mPhysicalStart[0];

            double sOutX = SliceWidth/-2d;
            double sOutY = SliceHeight/-2d;

            double stepOutX = SliceWidth  / PaintingArray.GetLength(1);
            double stepOutY =SliceHeight  / PaintingArray.GetLength(0);

            double stepX = GridWidth  / LX;
            double stepY = GridWidth  / LY;
            double stepZ = GridHeight  / LZ;

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

            aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY; ;
            bZY = normalY.Z / stepOutY;

            // double uX, uY, sum, Xu, Yu;
            ///this works by defining a plane normalX normalY that is placed under the screen.  Then each voxel in the cube is 
            ///projected by defining the vector R from the origin to the voxel.  the projection coordinates are then determined 
            ///by taking the dot product of the normals and R.

            int zOffest = DensityGrid.GetLength(2) * DensityGrid.GetLength(1);
            int yOffset = DensityGrid.GetLength(2);
            //int xL, yL;
            for (int zI = LZ / 2; zI < LZ / 2 + 1; zI++)
            {
                z = (zI * stepZ + sZ);
                aX = bZX * z + aZX;
                aY = bZY * z + aZY;
                tX1 = aX;
                tY1 = aY;
                for (int yI = 0; yI < LY; yI++)
                {
                    tX1 += bXY;
                    tY1 += bYY;

                    tXI = tX1;
                    tYI = tY1;
                    for (int xI = 0; xI < LX; xI++)
                    {
                        tXI += bXX;
                        tYI += bYX;

                        if (tXI > 0 && tXI < LOutX)
                            if (tYI > 0 && tYI < LOutY)
                            {
                                //xL = (int)Math.Floor(tXI);
                                //yL = (int)Math.Floor(tYI);
                                Coords.Add(tXI);
                            }
                    }
                }
            }
            ///take all the recorded x coords, sort them and then find the minimum distance between the coords
            Coords.Sort();
            double MinDist = double.MaxValue;
            double lCoord = Coords[0];
            double d;
            for (int i = 1; i < Coords.Count; i++)
            {
                d = Coords[i] - lCoord;
                if (d > 0 && d < MinDist) MinDist = d;
                lCoord = Coords[i];
            }
            return MinDist;
        }

        /// <summary>
        /// Calculates all the needed angles and then passes the information to smear array to find the grid spacing needed for this method
        /// </summary>
        /// <param name="Slice"></param>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians"></param>
        /// <param name="ConvolutionAxis"></param>
        /// <returns></returns>
        private static double MaxExpansion(double[,] Slice,double SliceWidth,double SliceHeight, double[][,] DensityGrid,double GridWidth, double GridHeight, double AngleRadians, Axis2D ConvolutionAxis)
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


            return 1 / SmearArray(DensityGrid,GridWidth,GridHeight, Slice,SliceWidth,SliceHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));

        }

        /// <summary>
        /// checks the meshing of the image and the grid on just one plane.  the minimum interpolation spacing is then determined
        /// </summary>
        /// <param name="DensityGrid"></param>
        /// <param name="PaintingArray"></param>
        /// <param name="Direction"></param>
        /// <param name="FastScanDirection"></param>
        /// <returns></returns>
        private static double SmearArray(double[][,] DensityGrid, double GridWidth, double GridHeight, double[,] PaintingArray, double SliceWidth, double SliceHeight, Point3D Direction, Point3D FastScanDirection)
        {
            List<double> Coords = new List<double>();

            Direction.Normalize();
            FastScanDirection.Normalize();
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            int LX = DensityGrid[0].GetLength(1);
            int LY = DensityGrid[0].GetLength(0);
            int LZ = DensityGrid.GetLength(0);

            int LOutX = PaintingArray.GetLength(1) - 1;
            int LOutY = PaintingArray.GetLength(0) - 1;

            double sX = GridWidth / -2d;//mPhysicalStart[2];
            double sY = GridWidth / -2d;//mPhysicalStart[1];
            double sZ = GridHeight / -2d;//mPhysicalStart[0];

            double sOutX = SliceWidth / -2d;
            double sOutY = SliceHeight / -2d;

            double stepOutX = SliceWidth / PaintingArray.GetLength(1);
            double stepOutY = SliceHeight / PaintingArray.GetLength(0);

            double stepX = GridWidth / LX;
            double stepY = GridWidth / LY;
            double stepZ = GridHeight / LZ;

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

            aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY; ;
            bZY = normalY.Z / stepOutY;

            // double uX, uY, sum, Xu, Yu;
            ///this works by defining a plane normalX normalY that is placed under the screen.  Then each voxel in the cube is 
            ///projected by defining the vector R from the origin to the voxel.  the projection coordinates are then determined 
            ///by taking the dot product of the normals and R.

            int zOffest = LX * LY;
            int yOffset = LX;
            //int xL, yL;
            for (int zI = LZ / 2; zI < LZ / 2 + 1; zI++)
            {
                z = (zI * stepZ + sZ);
                aX = bZX * z + aZX;
                aY = bZY * z + aZY;
                tX1 = aX;
                tY1 = aY;
                for (int yI = 0; yI < LY; yI++)
                {
                    tX1 += bXY;
                    tY1 += bYY;

                    tXI = tX1;
                    tYI = tY1;
                    for (int xI = 0; xI < LX; xI++)
                    {
                        tXI += bXX;
                        tYI += bYX;

                        if (tXI > 0 && tXI < LOutX)
                            if (tYI > 0 && tYI < LOutY)
                            {
                                //xL = (int)Math.Floor(tXI);
                                //yL = (int)Math.Floor(tYI);
                                Coords.Add(tXI);
                            }
                    }
                }
            }
            ///take all the recorded x coords, sort them and then find the minimum distance between the coords
            Coords.Sort();
            double MinDist = double.MaxValue;
            double lCoord = Coords[0];
            double d;
            for (int i = 1; i < Coords.Count; i++)
            {
                d = Coords[i] - lCoord;
                if (d > 0 && d < MinDist) MinDist = d;
                lCoord = Coords[i];
            }
            return MinDist;
        }

    }
}
