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
    public class InterpolateSliceArrayEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Expand Array Slice For Projection"; } }
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
            double Angle = (double)mFilterToken[2];

            double MaxPossibleExpansion = MaxExpansion(Slice,DensityGrid.Width,DensityGrid.Height , DensityGrid, Angle/*, Axis2D.YAxis*/);
            if (MaxPossibleExpansion > 10)
                MaxPossibleExpansion = 10;

            if (Math.Abs( MaxPossibleExpansion-1)>.1)
                Slice = ExpandArrayY(Slice, MaxPossibleExpansion, ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.InterpolationMethod.Linear );

             mPassData.AddSafe("ExpandedArray", Slice);

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

        public double[,] ExpandArrayY(double[,] SourceArray, double ExpansionFactor, ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.InterpolationMethod interpolationMethod)
        {
            double[,] SourceData = SourceArray;
            double[,] OutData = new double[(int)(ExpansionFactor * SourceArray.GetLength(0)), SourceArray.GetLength(1)];

            double StepY = (double)OutData.GetLength(0) / SourceData.GetLength(0);
            StepY = 1 / StepY;

            switch (interpolationMethod)
            {
                case ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.InterpolationMethod.Linear:
                    {
                        #region Linear
                        double[] mu2 = new double[OutData.GetLength(0)];
                        int[] Ys = new int[OutData.GetLength(0)];
                        double mu;
                        for (int i = 0; i < OutData.GetLength(0); i++)
                        {
                            mu = (i * StepY) % 1;
                            mu2[i] = mu;
                            Ys[i] = (int)Math.Truncate(i * StepY);

                        }
                        for (int i = 0; i < OutData.GetLength(1) ; i++)
                        {
                            for (int j = 0; j < OutData.GetLength(0)-1; j++)
                            {
                                try
                                {
                                    OutData[j, i] = SourceData[Ys[ j], i] * (1 - mu2[j]) + SourceData[Ys[j]+1 ,i] * (mu2[j]);
                                }
                                catch
                                {
                                    if (j < 4)
                                        OutData[j, i] = SourceData[0, i];
                                    else
                                        OutData[j, i] = SourceData[ SourceData.GetLength(0) - 1,i];
                                }
                            }
                        }
                        return OutData;
                        #endregion
                    }
                case ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.InterpolationMethod.Cosine:
                    {
                        #region Cosine

                        double[] mu2 = new double[OutData.GetLength(1)];
                        int[] Ys = new int[OutData.GetLength(1)];
                        double mu;
                        double y;
                        int SourceLength = SourceData.GetLength(1) - 2;
                        for (int i = 0; i < OutData.GetLength(1); i++)
                        {
                            mu = (i * StepY) % 1;
                            mu2[i] = (1 - Math.Cos(mu * Math.PI)) / 2;
                            Ys[i] = (int)Math.Truncate(i * StepY);

                        }
                        int x;
                        for (int i = 0; i < OutData.GetLength(1) ; i++)
                        {
                            for (int j = 0; j < OutData.GetLength(0)-1; j++)
                            {

                                x = Ys[j];
                                if (x < SourceLength)
                                {
                                    mu = mu2[j];
                                    y = SourceData[x, i] * (1 - mu) + SourceData[ x + 1,i] * (mu); ;
                                    OutData[j, i] = y;
                                }
                                else
                                    OutData[j, i] = SourceData[SourceLength+1, i];

                            }
                        }
                        return OutData;
                        #endregion
                    }
                case ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.InterpolationMethod.Cubic:
                    {
                        #region Cubic
                        double [] mu=new double[OutData.GetLength(1)];
                        double[] mu2 = new double[OutData.GetLength(1)];
                        double[] mu3 = new double[OutData.GetLength(1)];
                        int[] Ys = new int[OutData.GetLength(1)];

                        for (int i = 0; i < OutData.GetLength(1); i++)
                        {
                            mu[i] = (i * StepY) % 1;
                            mu2[i] = mu[i]*mu[i];
                            mu3[i] = mu2[i] * mu[i];
                            Ys[i] = (int)Math.Truncate(i * StepY);
                        }

                        double a0, a1, a2, a3;
                        double y0, y1, y2, y3;
                        int Y;
                        for (int i = 0; i < OutData.GetLength(1); i++)
                        {
                            for (int j = 0; j < OutData.GetLength(0); j++)
                            {
                                try
                                {
                                    Y = Ys[j];
                                    y0 = SourceData[Y-1, i];
                                    y1 = SourceData[Y, i];
                                    y2 = SourceData[Y+1, i];
                                    y3 = SourceData[Y+2, i];

                                    a0 = y3 - y2 - y0 + y1;
                                    a1 = y0 - y1 - a0;
                                    a2 = y2 - y0;
                                    a3 = y1;

                                    OutData[j, i] = a0 * mu3[j] + a1 * mu2[j]  + a2 * mu[j] + a3;
                                }
                                catch
                                {
                                    if (j < 4)
                                        OutData[j, i] = SourceData[0,i];
                                    else
                                        OutData[j, i] = SourceData[ SourceData.GetLength(0) - 1,i];
                                }
                            }
                        }
                        return OutData;
                        #endregion
                    }
                case ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.InterpolationMethod.Haskel:
                    {
                        #region Haskel
                        double[] mu = new double[OutData.GetLength(1)];
                        double[] mu2 = new double[OutData.GetLength(1)];
                        double[] mu3 = new double[OutData.GetLength(1)];
                        int[] Ys = new int[OutData.GetLength(1)];

                        for (int i = 0; i < OutData.GetLength(1); i++)
                        {
                            mu[i] = (i * StepY) % 1;
                            mu2[i] = mu[i] * mu[i];
                            mu3[i] = mu2[i] * mu[i];
                            Ys[i] = (int)Math.Truncate(i * StepY);
                        }

                        double a0, a1, a2, a3;
                        double y0, y1, y2, y3;
                        double m0, m1;
                        double bias = 0, tension = 0;//both range between -1 and 1
                        int Y;
                        for (int i = 0; i < OutData.GetLength(1); i++)
                        {
                            for (int j = 0; j < OutData.GetLength(0); j++)
                            {
                                try
                                {
                                    Y = Ys[j];
                                    y0 = SourceData[Y - 1,i];
                                    y1 = SourceData[Y,i];
                                    y2 = SourceData[Y + 1,i];
                                    y3 = SourceData[Y + 2,i];

                                   
                                    m0 = (y1 - y0) * (1 + bias) * (1 - tension) / 2;
                                    m0 += (y2 - y1) * (1 - bias) * (1 - tension) / 2;
                                    m1 = (y2 - y1) * (1 + bias) * (1 - tension) / 2;
                                    m1 += (y3 - y2) * (1 - bias) * (1 - tension) / 2;


                                    a0 = 2 * mu3[j] - 3 * mu2[j] + 1;
                                    a1 = mu3[j] - 2 * mu2[j] + mu[j];
                                    a2 = mu3[j] - mu2[j];
                                    a3 = -2 * mu3[j] + 3 * mu2[j];

                                    OutData[j, i] = a0 * y1 + a1 * m0 + a2 * m1 + a3 * y2;
                                }
                                catch
                                {
                                    if (j < 4)
                                        OutData[j, i] = SourceData[0,i];
                                    else
                                        OutData[j, i] = SourceData[ SourceData.GetLength(0) - 1,i];
                                }
                            }
                        }
                        return OutData;
                        #endregion
                    }
            }
            return null;
        }


        public double[,] ExpandArrayX(double[,] SourceArray, double ExpansionFactor, ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.InterpolationMethod interpolationMethod)
        {
            double[,] SourceData = SourceArray;
            double[,] OutData = new double[SourceArray.GetLength(0), (int)(ExpansionFactor * SourceArray.GetLength(1))];

            double StepX = (double)OutData.GetLength(0) / SourceData.GetLength(0);
            StepX = 1 / StepX;

            switch (interpolationMethod)
            {
                case ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.InterpolationMethod.Linear:
                    {
                        #region Linear
                        double[] mu2 = new double[OutData.GetLength(1)];
                        int[] Xs = new int[OutData.GetLength(1)];
                        double mu;
                        for (int i = 0; i < OutData.GetLength(1); i++)
                        {
                            mu = (i * StepX) % 1;
                            mu2[i] = mu;
                            Xs[i] = (int)Math.Truncate(i * StepX);

                        }
                        for (int i = 0; i < OutData.GetLength(1) - 1; i++)
                        {
                            for (int j = 0; j < OutData.GetLength(0); j++)
                            {
                                try
                                {
                                    OutData[j, i] = SourceData[j, Xs[i]] * (1 - mu2[i]) + SourceData[j, Xs[i] + 1] * (mu2[i]);
                                }
                                catch
                                {
                                    if (i < 4)
                                        OutData[j, i] = SourceData[j, 0];
                                    else
                                        OutData[j, i] = SourceData[j, SourceData.GetLength(2) - 1];
                                }
                            }
                        }
                        return OutData;
                        #endregion
                    }
                case ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.InterpolationMethod.Cosine:
                    {
                        #region Cosine

                        double[] mu2 = new double[OutData.GetLength(1)];
                        int[] Xs = new int[OutData.GetLength(1)];
                        double mu;
                        double y;
                        int SourceLength = SourceData.GetLength(1) - 2;
                        for (int i = 0; i < OutData.GetLength(1); i++)
                        {
                            mu = (i * StepX) % 1;
                            mu2[i] = (1 - Math.Cos(mu * Math.PI)) / 2;
                            Xs[i] = (int)Math.Truncate(i * StepX);

                        }
                        int x;
                        for (int i = 0; i < OutData.GetLength(1) - 1; i++)
                        {
                            for (int j = 0; j < OutData.GetLength(0); j++)
                            {

                                x = Xs[i];
                                if (x < SourceLength)
                                {
                                    mu = mu2[i];
                                    y = SourceData[j, x] * (1 - mu) + SourceData[j, x + 1] * (mu); ;
                                    OutData[j, i] = y;
                                }
                                else
                                    OutData[j, i] = SourceData[j, SourceLength + 1];

                            }
                        }
                        return OutData;
                        #endregion
                    }
                case ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.InterpolationMethod.Cubic:
                    {
                        #region Cubic
                        double[] mu2 = new double[OutData.GetLength(1)];
                        int[] Xs = new int[OutData.GetLength(1)];
                        double mu;
                        for (int i = 0; i < OutData.GetLength(1); i++)
                        {
                            mu = (i * StepX) % 1;
                            mu2[i] = mu;
                            Xs[i] = (int)Math.Truncate(i * StepX);

                        }
                        double a0, a1, a2, a3;
                        double y0, y1, y2, y3;
                        for (int i = 0; i < OutData.GetLength(1); i++)
                        {
                            for (int j = 0; j < OutData.GetLength(0); j++)
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

                                    OutData[j, i] = a0 * mu2[i] * mu2[i] * mu2[i] + a1 * mu2[i] * mu2[i] + a2 * mu2[i] + a3;
                                }
                                catch
                                {
                                    if (i < 4)
                                        OutData[j, i] = SourceData[j, 0];
                                    else
                                        OutData[j, i] = SourceData[j, SourceData.GetLength(1) - 1];
                                }
                            }
                        }
                        return OutData;
                        #endregion
                    }
                case ImageViewer.PythonScripting.Projection.InterpolateSliceEffect.InterpolationMethod.Haskel:
                    {
                        #region Haskel
                        double[] mu2 = new double[OutData.GetLength(1)];
                        int[] Xs = new int[OutData.GetLength(1)];
                        double mu;
                        for (int i = 0; i < OutData.GetLength(1); i++)
                        {
                            mu = (i * StepX) % 1;
                            mu2[i] = mu;
                            Xs[i] = (int)Math.Truncate(i * StepX);

                        }
                        double a0, a1, a2, a3;
                        double y0, y1, y2, y3;
                        double m0, m1, mu_2, mu3;
                        double bias = 0, tension = 0;//both range between -1 and 1
                        for (int i = 0; i < OutData.GetLength(1); i++)
                        {
                            for (int j = 0; j < OutData.GetLength(0); j++)
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

                                    OutData[j, i] = a0 * y1 + a1 * m0 + a2 * m1 + a3 * y2;
                                }
                                catch
                                {
                                    if (i < 4)
                                        OutData[j, i] = SourceData[j, 0];
                                    else
                                        OutData[j, i] = SourceData[j, SourceData.GetLength(1) - 1];
                                }
                            }
                        }
                        return OutData;
                        #endregion
                    }
            }
            return null;
        }

        public double MaxExpansion(double[,] Slice, double SliceWidth, double SliceHeight, ProjectionArrayObject DensityGrid, double AngleRadians)
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


            return 1 / TestSmearArray(DensityGrid, Slice, SliceWidth, SliceHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));

        }

        private double TestSmearArray(ProjectionArrayObject DensityGrid, double[,] PaintingArray, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
        {
            double[][,] mDataDouble = DensityGrid.Data;
            double[] XCoords;

            Direction.Normalize();
            FastScanDirection.Normalize();
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LZ = mDataDouble.GetLength(0);

            int LOutX = PaintingArray.GetLength(1); //PaintingArray.GetLength(Axis.XAxis);
            int LOutY = PaintingArray.GetLength(0);

            double sX = DensityGrid.XMin;// .PhysicalStart(Axis.XAxis);
            double sY = DensityGrid.YMin;//.PhysicalStart(Axis.YAxis);
            double sZ = DensityGrid.ZMin;//.PhysicalStart(Axis.ZAxis);

            double sOutX = -1 * (PaintingWidth / 2d); //PaintingArray.PhysicalStart(Axis.XAxis);
            double sOutY = -1 * (PaintingHeight / 2d); //PaintingArray.PhysicalStart(Axis.YAxis);
            double stepOutX = (PaintingWidth / LOutX); //PaintingArray.PhysicalStep(Axis.XAxis);
            double stepOutY = (PaintingHeight / LOutY);//PaintingArray.PhysicalStep(Axis.YAxis);

            double stepX = (DensityGrid.XMax - DensityGrid.XMin) / mDataDouble.GetLength(2); // DensityGrid.PhysicalStep(Axis.XAxis);
            double stepY = (DensityGrid.YMax - DensityGrid.YMin) / mDataDouble.GetLength(1); //DensityGrid.PhysicalStep(Axis.YAxis);
            double stepZ = (DensityGrid.ZMax - DensityGrid.ZMin) / mDataDouble.GetLength(0); //DensityGrid.PhysicalStep(Axis.ZAxis);

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


            double[] Normalization = new double[mDataDouble.GetLength(0)];

            List<double> Coords = new List<double>();
            unsafe
            {
                int zOffest = LZ / 2;
                int yOffset = mDataDouble.GetLength(2);
                for (int zI = zOffest; zI < zOffest + 1; zI++)
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
                                    //*POutX += PaintingArray.mDataDouble[0, (int)tYI, (int)tXI];
                                    Coords.Add(tX1);
                                }
                        }

                    }
                }
            }

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
