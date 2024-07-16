using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MathHelpLib;
using System.IO;
using System.Threading;
using MathHelpLib.ImageProcessing;
using ImageViewer;
using System.Windows.Forms;

namespace Tomographic_Imaging_2
{
    public class ProjectionObject
    {
        protected PhysicalArray mDensityGrid;

        protected List<aProjectionSlice> mAllSlices = new List<aProjectionSlice>();

        protected double mPhysicalXWidth;
        protected double mPhysicalYWidth;
        protected double mPhysicalHeight;

        public PhysicalArray ProjectionData
        {
            get { return mDensityGrid; }
        }

        public void ClearGrid(bool ProjectionExtendsToCorners, double PhysicalWidth, double PhysicalHeight, int nCols, int nRows)
        {
            mDensityGrid = new PhysicalArray(nCols, nRows, PhysicalWidth / -2d, PhysicalWidth / 2d, PhysicalHeight / -2d, PhysicalHeight / 2d);

            mAllSlices = new List<aProjectionSlice>();

            this.mPhysicalXWidth = PhysicalWidth;
            this.mPhysicalHeight = PhysicalHeight;

            if (ProjectionExtendsToCorners)
            {
                mProjectionStartX = -1 * Math.Sqrt(Math.Pow(PhysicalWidth / 2, 2) + Math.Pow(PhysicalHeight / 2, 2));
                mProjectionEndX = mProjectionStartX * -1;
            }
            else
            {
                mProjectionStartX = -1 * PhysicalWidth / 2;
                mProjectionEndX = PhysicalWidth / 2;
            }

            double stepX = PhysicalWidth / nCols;
            double stepY = PhysicalHeight / nRows;
            if (stepX < stepY)
                mProjectionStepX = stepX;
            else
                mProjectionStepX = stepY;
            m_nProjectionSteps = (int)((mProjectionEndX - mProjectionStartX) / mProjectionStepX);
        }

        public void ClearGrid(bool ProjectionExtendsToCorners, double PhysicalXWidth, double PhysicalYWidth, double PhysicalHeight, int nCols, int nRows, int nZCols)
        {

            //FFTGrid = new complex[nCols, nRows, nZCols];
            mDensityGrid = new PhysicalArray(nCols, nRows, nZCols,
                PhysicalXWidth / -2d, PhysicalXWidth / 2d,
                PhysicalYWidth / -2d, PhysicalYWidth / 2d,
                PhysicalHeight / -2d, PhysicalHeight / 2d);// new double[nCols, nRows, nZCols];

            mAllSlices = new List<aProjectionSlice>();

            this.mPhysicalXWidth = PhysicalXWidth;
            this.mPhysicalYWidth = PhysicalYWidth;
            this.mPhysicalHeight = PhysicalHeight;

            if (ProjectionExtendsToCorners)
            {
                mProjectionStartX = -1 * Math.Sqrt(Math.Pow(PhysicalXWidth / 2d, 2) + PhysicalYWidth * PhysicalYWidth / 4d + Math.Pow(PhysicalHeight / 2d, 2));
                mProjectionEndX = mProjectionStartX * -1;
            }
            else
            {
                mProjectionStartX = -1 * PhysicalXWidth / 2;
                mProjectionEndX = PhysicalXWidth / 2;
            }

            double stepX = PhysicalXWidth / nCols;
            double stepY = PhysicalHeight / nRows;
            if (stepX < stepY)
                mProjectionStepX = stepX;
            else
                mProjectionStepX = stepY;
            m_nProjectionSteps = (int)((mProjectionEndX - mProjectionStartX) / mProjectionStepX);
        }

        #region SliceAndDice
        /// <summary>
        /// Adds offsetX slice to the collection.  Nothing is done with the mDataDouble
        /// </summary>
        /// <param name="aProjectionSlice"></param>
        public void AddSlice(aProjectionSlice ProjectionSlice)
        {
            mAllSlices.Add(ProjectionSlice);
        }
        public void AddSlicesRange(aProjectionSlice[] ProjectionSlices)
        {
            mAllSlices.AddRange(ProjectionSlices);
        }
        public void AddSlicesRange(List<aProjectionSlice> ProjectionSlices)
        {
            mAllSlices.AddRange(ProjectionSlices);
        }
        public int NumberOfSlices
        {
            get { return mAllSlices.Count; }
        }
        #endregion

        #region SimulationCode
        protected double mProjectionStartX;
        protected double mProjectionEndX;
        protected double mProjectionStepX;
        protected int m_nProjectionSteps;

        public aProjectionSlice CreateSimulatedProjection(double AngleDegrees)
        {
            if (mDensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
                return CreateForwardProjection1D(AngleDegrees);
            else
                return CreateForwardProjection2D(Axis.XAxis, AngleDegrees);
        }


        #region GeometricShapes

        /// <summary>
        /// Adds offsetX ellipse to density grid.  
        /// </summary>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="MajorAxis"></param>
        /// <param name="MinorAxis"></param>
        /// <param name="Rotation">Rotation in degrees</param>
        /// <param name="DensityChange"></param>
        public virtual void AddEllipse2D(double CenterX, double CenterY, double MajorAxis, double MinorAxis, double RotationAngle, double DensityChange)
        {
            double Rotation = RotationAngle / 180d * Math.PI;
            double cX = (mPhysicalXWidth / 2) / (mDensityGrid.GetLength(Axis.XAxis) / 2);
            double cY = (mPhysicalHeight / 2) / (mDensityGrid.GetLength(Axis.YAxis) / 2);
            int LX = mDensityGrid.GetLength(Axis.XAxis);
            int LY = mDensityGrid.GetLength(Axis.YAxis);

            for (int i = 0; i < LX; i++)
            {
                double x = cX * (i - LX / 2d);
                for (int j = 0; j < LY; j++)
                {
                    double y = cY * (j - LY / 2d);
                    if (MathHelps.IsInsideEllipse(x, y, CenterX, CenterY, MajorAxis, MinorAxis, Rotation))
                    {
                        mDensityGrid[i, j] = (double)mDensityGrid[i, j] + DensityChange;
                    }
                }
            }
        }

        /// <summary>
        /// Adds offsetX rectangle to density grid.  
        /// </summary>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="MajorAxis"></param>
        /// <param name="MinorAxis"></param>
        /// <param name="Rotation">Rotation in degrees</param>
        /// <param name="DensityChange"></param>
        public virtual void AddRectangle2D(double CenterX, double CenterY, double MajorAxis, double MinorAxis, double RotationAngle, double DensityChange)
        {
            double Rotation = RotationAngle / 180d * Math.PI;
            double cX = (mPhysicalXWidth / 2) / (mDensityGrid.GetLength(Axis.XAxis) / 2);
            double cY = (mPhysicalHeight / 2) / (mDensityGrid.GetLength(Axis.YAxis) / 2);
            for (int i = 0; i < mDensityGrid.GetLength(0); i++)
            {
                double x = cX * (i - mDensityGrid.GetLength(Axis.XAxis) / 2);
                for (int j = 0; j < mDensityGrid.GetLength(Axis.YAxis); j++)
                {
                    double y = cY * (j - mDensityGrid.GetLength(Axis.YAxis) / 2);
                    if (MathHelps.IsInsideRectangle(x, y, CenterX, CenterY, MajorAxis, MinorAxis, Rotation))
                    {
                        mDensityGrid[i, j] = (double)mDensityGrid[i, j] + DensityChange;
                    }
                }
            }
        }

        /// <summary>
        /// Adds offsetX rectangle to density grid.  
        /// </summary>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="MajorAxis"></param>
        /// <param name="MinorAxis"></param>
        /// <param name="Rotation">Rotation in degrees</param>
        /// <param name="DensityChange"></param>
        public virtual void AddRectangle2D(double MajorAxis, double MinorAxis, double DensityChange)
        {

            int sX = (int)((mDensityGrid.GetLength(Axis.XAxis) / 2) - mDensityGrid.GetLength(Axis.XAxis) / 2 * MajorAxis);
            int eX = (int)((mDensityGrid.GetLength(Axis.XAxis) / 2) + mDensityGrid.GetLength(Axis.XAxis) / 2 * MajorAxis);
            int sY = (int)((mDensityGrid.GetLength(Axis.YAxis) / 2) - mDensityGrid.GetLength(Axis.YAxis) / 2 * MinorAxis);
            int eY = (int)((mDensityGrid.GetLength(Axis.YAxis) / 2) + mDensityGrid.GetLength(Axis.YAxis) / 2 * MinorAxis);

            for (int i = sX; i < eX; i++)
            {
                for (int j = sY; j < eY; j++)
                {
                    mDensityGrid[i, j] = (double)mDensityGrid[i, j] + DensityChange;
                }
            }
        }

        /// <summary>
        /// Adds offsetX ellipse to density grid.  
        /// </summary>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="MajorAxis"></param>
        /// <param name="MinorAxis"></param>
        /// <param name="Rotation">Rotation in degrees</param>
        /// <param name="DensityChange"></param>
        public virtual void AddEllipse3D(double CenterX, double CenterY, double CenterZ, double MajorAxis, double MinorAxis, double RotationAngle, double DensityChange)
        {
            double Rotation = RotationAngle / 180d * Math.PI;
            double cX = (mPhysicalXWidth) / ((double)mDensityGrid.GetLength(Axis.XAxis));
            double cY = (mPhysicalYWidth) / ((double)mDensityGrid.GetLength(Axis.YAxis));
            double cZ = (mPhysicalHeight) / ((double)mDensityGrid.GetLength(Axis.ZAxis));

            double aX = Math.Sqrt(Math.Pow(MajorAxis * Math.Cos(Rotation), 2) + Math.Pow(MinorAxis * Math.Sin(Rotation), 2));
            double aY = Math.Sqrt(Math.Pow(MajorAxis * Math.Cos(Rotation + Math.PI / 2), 2) + Math.Pow(MinorAxis * Math.Sin(Rotation + Math.PI / 2), 2));
            double aZ = MinorAxis;

            double hX, hY, hZ;
            hX = mDensityGrid.GetLength(Axis.XAxis) / 2;
            hY = mDensityGrid.GetLength(Axis.YAxis) / 2;
            hZ = mDensityGrid.GetLength(Axis.ZAxis) / 2;


            int Sx = (int)(hX + (CenterX - aX) / cX - 1);
            int Ex = (int)(hX + (CenterX + aX) / cX + 1);
            int Sy = (int)(hY + (CenterY - aY) / cY - 1);
            int Ey = (int)(hY + (CenterY + aY) / cY + 1);
            int Sz = (int)(hZ + (CenterZ - aZ) / cZ - 1);
            int Ez = (int)(hZ + (CenterZ + aZ) / cZ + 1);

            double c = Math.Cos(-1 * Rotation);
            double s = Math.Sin(-1 * Rotation);

            double x, y, z, x1, y1;
            double offsetX, offsetY, ccX, ssX, offsetZ;

            offsetX = -1 * (cX * hX + CenterX);
            offsetY = -1 * (cY * hY + CenterY);
            offsetZ = -1 * (cZ * hZ + CenterZ);

            for (int i = Sx; i < Ex; i++)
            {
                x = cX * i + offsetX;
                ccX = c * x;
                ssX = s * x;
                for (int j = Sy; j < Ey; j++)
                {
                    y = cY * j + offsetY;
                    x1 = (ccX - s * y) / MajorAxis;
                    y1 = (ssX + c * y) / MinorAxis;

                    x1 = x1 * x1;
                    y1 = y1 * y1;

                    for (int m = Sz; m <= Ez; m++)
                    {
                        z = (cZ * m + offsetZ) / MinorAxis;
                        if (x1 + y1 + z * z < 1)
                        {
                            mDensityGrid[i, j, m] = (double)mDensityGrid[i, j, m] + DensityChange;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds offsetX rectangle to density grid.  
        /// </summary>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="MajorAxis"></param>
        /// <param name="MinorAxis"></param>
        /// <param name="Rotation">Rotation in degrees</param>
        /// <param name="DensityChange"></param>
        public virtual void AddRectangle3D(double CenterX, double CenterY, double MajorAxis, double MinorAxis, double RotationAngle, double DensityChange)
        {
            double Rotation = RotationAngle / 180d * Math.PI;
            double cX = (mPhysicalXWidth / 2) / (mDensityGrid.GetLength(Axis.XAxis) / 2);
            double cY = (mPhysicalYWidth / 2) / (mDensityGrid.GetLength(Axis.YAxis) / 2);
            double cZ = (mPhysicalHeight / 2) / (mDensityGrid.GetLength(Axis.ZAxis) / 2);
            for (int i = 0; i < mDensityGrid.GetLength(0); i++)
            {
                double x = cX * (i - mDensityGrid.GetLength(Axis.XAxis) / 2);
                for (int j = 0; j < mDensityGrid.GetLength(Axis.YAxis); j++)
                {
                    double y = cY * (j - mDensityGrid.GetLength(Axis.YAxis) / 2);
                    for (int m = 0; m < mDensityGrid.GetLength(Axis.ZAxis); m++)
                    {
                        double z = cZ * (m - mDensityGrid.GetLength(Axis.ZAxis));
                        if (Math3DHelps.IsInsideRectangle(x, y, z, CenterX, CenterY, 0, MajorAxis, MinorAxis, Rotation))
                        {
                            mDensityGrid[i, j, m] = (double)mDensityGrid[i, j, m] + DensityChange;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds offsetX rectangle to density grid.  
        /// </summary>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="MajorAxis"></param>
        /// <param name="MinorAxis"></param>
        /// <param name="Rotation">Rotation in degrees</param>
        /// <param name="DensityChange"></param>
        public virtual void AddRectangle3D(double MajorAxis, double MinorAxis, double DensityChange)
        {

            int sX = (int)((mDensityGrid.GetLength(Axis.XAxis) / 2) - mDensityGrid.GetLength(Axis.XAxis) / 2 * MajorAxis);
            int eX = (int)((mDensityGrid.GetLength(Axis.XAxis) / 2) + mDensityGrid.GetLength(Axis.XAxis) / 2 * MajorAxis);
            int sY = (int)((mDensityGrid.GetLength(Axis.YAxis) / 2) - mDensityGrid.GetLength(Axis.YAxis) / 2 * MinorAxis);
            int eY = (int)((mDensityGrid.GetLength(Axis.YAxis) / 2) + mDensityGrid.GetLength(Axis.YAxis) / 2 * MinorAxis);

            int sZ = (int)((mDensityGrid.GetLength(Axis.ZAxis) / 2) - mDensityGrid.GetLength(Axis.ZAxis) / 2 * MinorAxis);
            int eZ = (int)((mDensityGrid.GetLength(Axis.ZAxis) / 2) + mDensityGrid.GetLength(Axis.ZAxis) / 2 * MinorAxis);

            for (int i = sX; i < eX; i++)
            {
                for (int j = sY; j < eY; j++)
                {
                    for (int m = sZ; m < eZ; m++)
                    {
                        mDensityGrid[i, j, m] = (double)mDensityGrid[i, j, m] + DensityChange;
                    }
                }
            }
        }


        /// <summary>
        /// Creates the phantom based on the work of shepp and logan
        /// </summary>
        public void CreateShepAndLogan()
        {
            if (mDensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
                CreateShepAndLogan2D();
            else
                CreateShepAndLogan3D();
        }

        /// <summary>
        /// Creates the phantom based on the work of shepp and logan
        /// </summary>
        private void CreateShepAndLogan2D()
        {
            AddEllipse2D(0, 0, .92, .69, 90, 2);
            AddEllipse2D(0, -0.0184, .874, .6624, 90, -0.98);
            AddEllipse2D(.22, 0, .31, .11, 72, -.2);
            AddEllipse2D(-.22, 0, .41, .16, 108, -.2);
            AddEllipse2D(0, .35, .25, .21, 90, .1);
            AddEllipse2D(0, .01, .046, .046, 0, .1);
            AddEllipse2D(0, -.01, .046, .046, 0, .1);
            AddEllipse2D(-.08, -.605, .046, .023, 0, .1);
            AddEllipse2D(0, -.605, .023, .023, 0, .1);
            AddEllipse2D(.06, -.605, .046, .023, 90, .1);

        }

        /// <summary>
        /// Creates the phantom based on the work of shepp and logan
        /// </summary>
        private void CreateShepAndLogan3D()
        {

            AddEllipse3D(0, 0, 0, .92, .92, 0, 2);
            /* AddEllipse3D(0, 0, 0, .92, .69, 90, 2);
             AddEllipse3D(0, -0.0184, 0, .874, .6624, 90, -0.98);
             AddEllipse3D(.22, 0, 0, .31, .11, 72, -.2);
             AddEllipse3D(-.22, 0, 0, .41, .16, 108, -.2);
             AddEllipse3D(0, .35, 0, .25, .21, 90, .1);
             AddEllipse3D(0, .01, 0, .046, .046, 0, .1);
             AddEllipse3D(0, -.01, 0, .046, .046, 0, .1);
             AddEllipse3D(-.08, -.605, 0, .046, .023, 0, .1);
             AddEllipse3D(0, -.605, 0, .023, .023, 0, .1);
             AddEllipse3D(.06, -.605, 0, .046, .023, 90, .1);
             */
        }

        #endregion
        #endregion

        #region BackProjection

        public void DoBackProjection_OneSlice(int PaddedSize, int SliceNumber, Axis2D ConvolutionAxis, ConvolutionMethod convolutionMethod)
        {
            Axis RotationAxis = Axis.ZAxis;
            if (mDensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
            {
                Point3D vec;
                Point3D axis;

                axis = new Point3D(0, 0, 1);

                int i = SliceNumber;

                double rx = Math.Cos(mAllSlices[i].Angle);
                double ry = Math.Sin(mAllSlices[i].Angle);

                vec = new Point3D(ry, rx, 0);

                mDensityGrid.SmearArray(mAllSlices[i].BackProjection, vec, Point3D.CrossProduct(vec, axis));

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

                int i = SliceNumber;
                double angle = mAllSlices[i].Angle;

                Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, angle);

                mDensityGrid.SmearArray(mAllSlices[i].BackProjection, vec, Point3D.CrossProduct(vec, vRotationAxis));
            }

        }

        public void DoBackProjection_OneSlice(PhysicalArray SliceProjection, double AngleRadians)
        {
            Axis RotationAxis = Axis.ZAxis;
            if (mDensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
            {
                Point3D vec;
                Point3D axis;

                axis = new Point3D(0, 0, 1);

                double rx = Math.Cos(AngleRadians);
                double ry = Math.Sin(AngleRadians);

                vec = new Point3D(ry, rx, 0);

                mDensityGrid.SmearArray(SliceProjection, vec, Point3D.CrossProduct(vec, axis));

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

                Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, AngleRadians);

                //mDensityGrid.SmearArray(SliceProjection , vec, Point3D.CrossProduct(vec, vRotationAxis));
                mDensityGrid.SmearArrayInterpolate(SliceProjection, vec, Point3D.CrossProduct(vec, vRotationAxis));
            }

        }


        public void DoBackProjection_OneSlice(int PaddedSize, aProjectionSlice Slice, Axis2D ConvolutionAxis, ConvolutionMethod convolutionMethod)
        {
            Axis RotationAxis = Axis.ZAxis;
            if (mDensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
            {
                Point3D vec;
                Point3D axis;

                axis = new Point3D(0, 0, 1);


                double rx = Math.Cos(Slice.Angle);
                double ry = Math.Sin(Slice.Angle);

                vec = new Point3D(ry, rx, 0);

                mDensityGrid.SmearArray(Slice.BackProjection, vec, Point3D.CrossProduct(vec, axis));

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

                double angle = Slice.Angle;

                Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, angle);

                mDensityGrid.SmearArrayInterpolate(Slice.BackProjection, vec, Point3D.CrossProduct(vec, vRotationAxis));
            }

        }

        public void DoBackProjection_OneSlice(int PaddedSize, aProjectionSlice Slice, Axis2D ConvolutionAxis, int StartZIndex, int EndZIndex, ConvolutionMethod convolutionMethod)
        {
            Axis RotationAxis = Axis.ZAxis;
            if (mDensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
            {
                Point3D vec;
                Point3D axis;

                axis = new Point3D(0, 0, 1);


                double rx = Math.Cos(Slice.Angle);
                double ry = Math.Sin(Slice.Angle);

                vec = new Point3D(ry, rx, 0);

                mDensityGrid.SmearArray(Slice.BackProjection, vec, Point3D.CrossProduct(vec, axis));

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

                double angle = Slice.Angle;

                Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, angle);

                mDensityGrid.SmearArray(Slice.BackProjection, vec, Point3D.CrossProduct(vec, vRotationAxis));
            }

        }

        public void DoBackProjection_AllSlices(int PaddedSize, Axis2D ConvolutionAxis, bool CreateThreads, ConvolutionMethod convolutionMethod)
        {
            for (int i = 0; i < mAllSlices.Count; i++)
            {
                if (mAllSlices[i].BackProjectionPerformed == false)
                    throw new Exception("Backprojections must have been performed before this function is called");
            }

            Axis RotationAxis = Axis.ZAxis;
            if (mDensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
            {
                #region 2DProjection
                Point3D vec;
                Point3D axis;

                axis = new Point3D(0, 0, 1);

                for (int i = 0; i < mAllSlices.Count; i++)
                {
                    double rx = Math.Cos(mAllSlices[i].Angle);
                    double ry = Math.Sin(mAllSlices[i].Angle);

                    vec = new Point3D(ry, rx, 0);
                    Point3D FastScan;
                    if (ConvolutionAxis == Axis2D.XAxis)
                        FastScan = Point3D.CrossProduct(vec, axis);
                    else
                        FastScan = new Point3D(0, 0, 1);


                    mDensityGrid.SmearArray(mAllSlices[i].BackProjection, vec, FastScan);
                }
                #endregion
            }
            else
            {
                #region 3DProjection
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

                for (int i = 0; i < mAllSlices.Count; i++)
                {
                    double angle = mAllSlices[i].Angle;

                    Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, angle);
                    Point3D FastScan;

                    if (ConvolutionAxis == Axis2D.XAxis)
                        FastScan = Point3D.CrossProduct(vec, vRotationAxis);
                    else
                        FastScan = new Point3D(0, 0, 1);

                    if (CreateThreads)
                    {
                        int ProcCount = Environment.ProcessorCount;
                        double NZs = ((double)mDensityGrid.GetLength(Axis.ZAxis) / (double)ProcCount);
                        Thread[] Proj_Thread = new Thread[ProcCount];
                        Queue<int> Indexs = new Queue<int>();

                        for (int j = 0; j < ProcCount; j++)
                        {
                            Indexs.Enqueue(j);
                        }
                        for (int j = 0; j < ProcCount; j++)
                        {
                            Proj_Thread[j] = new Thread(delegate()
                             {
                                 int jj = Indexs.Dequeue();
                                 mDensityGrid.SmearArrayInterpolate(mAllSlices[i].BackProjection, vec, FastScan);
                             }
                            );
                            Proj_Thread[j].Start();
                        }
                        for (int j = 0; j < ProcCount; j++)
                        {
                            Proj_Thread[j].Join();
                        }
                    }
                    else
                        mDensityGrid.SmearArrayInterpolate(mAllSlices[i].BackProjection, vec, FastScan);
                }
                #endregion
            }

        }

        public void DoBackProjection_AllSlices(int PaddedSize, double[] impulse, ConvolutionMethod convolutionMethod)
        {
            Axis2D ConvolutionAxis = Axis2D.XAxis;
            // Axis RotationAxis = Axis.ZAxis;
            for (int i = 0; i < mAllSlices.Count; i++)
            {
                if (mAllSlices[i].BackProjectionPerformed == false)
                    mAllSlices[i].DoBackProjection(impulse, PaddedSize, convolutionMethod);
            }
            DoBackProjection_AllSlices(PaddedSize, ConvolutionAxis, false, convolutionMethod);
        }


        private void DoBackProjection_ThreadedMediumMemory1D(int PaddedSize, aProjectionSlice[] SliceArray, double[] impulse, ConvolutionMethod convolutionMethod)
        {
            //1D projections are not parallelized yet

            foreach (aProjectionSlice Slice in SliceArray)
            {
                Slice.PersistDataInMemory = true;
                Slice.DoBackProjection(impulse, PaddedSize, convolutionMethod);
                AddSlice(Slice);
            }
            DoBackProjection_AllSlices(PaddedSize, Axis2D.XAxis, false, convolutionMethod);
            return;

        }

        private void DoBackProjection_ThreadedMediumMemory2D(int PaddedSize, aProjectionSlice[] SliceArray, double[,] impulse, ConvolutionMethod convolutionMethod)
        {
            int ProcCount = Environment.ProcessorCount / 2;

            Queue<aProjectionSlice> SliceQueue = new Queue<aProjectionSlice>();
            for (int i = 0; i < SliceArray.Length; i++)
            {
                SliceQueue.Enqueue(SliceArray[i]);
            }

            Thread[] BackProject = new Thread[ProcCount];

            for (int i = 0; i < ProcCount; i++)
            {
                BackProject[i] = new Thread(delegate()
                {
                    aProjectionSlice psf = null;
                    while (SliceQueue.Count > 0)
                    {
                        psf = SliceQueue.Dequeue();
                        psf.PersistDataInMemory = true;
                        psf.DoBackProjection(impulse, PaddedSize, convolutionMethod);
                        ScriptingInterface.scriptingInterface.CreateGraph("BackProjection", psf.BackProjection);
                        DoBackProjection_OneSlice(PaddedSize, psf, Axis2D.XAxis, convolutionMethod);
                        psf.PersistDataInMemory = false;
                    }
                    ScriptingInterface.scriptingInterface.SetProjectionSlice(psf);
                });
            }

            for (int Proc = 0; Proc < ProcCount; Proc++)
            {
                BackProject[Proc].Start();
            }

            int ThreadsFinished = 0;
            while (ThreadsFinished < ProcCount)
            {
                ThreadsFinished = 0;
                for (int i = 0; i < ProcCount; i++)
                {
                    if (BackProject[i].ThreadState == ThreadState.Stopped)
                        ThreadsFinished++;
                }
                Application.DoEvents();
            }
            mDensityGrid = mDensityGrid / (SliceArray.Length * mDensityGrid.GetLength(Axis.XAxis));

        }

        private void DoBackProjection_ThreadedMediumMemory2D(int PaddedSize, aProjectionSlice[] SliceArray, double[] impulse, ConvolutionMethod convolutionMethod)
        {
            int ProcCount = 2 * Environment.ProcessorCount;


            Queue<aProjectionSlice> SliceQueue = new Queue<aProjectionSlice>();
            for (int i = 0; i < SliceArray.Length; i++)
            {
                SliceQueue.Enqueue(SliceArray[i]);
            }

            Thread[] BackProject = new Thread[ProcCount];

            for (int i = 0; i < ProcCount; i++)
            {
                BackProject[i] = new Thread(delegate()
                        {
                            aProjectionSlice psf = null;
                            while (SliceQueue.Count > 0)
                            {
                                psf = SliceQueue.Dequeue();
                                Console.WriteLine(Thread.CurrentThread.ManagedThreadId + "\t" + psf.Angle.ToString());
                                if (psf.PersistDataInMemory == true)
                                {
                                    psf.DoBackProjection(impulse, PaddedSize, convolutionMethod);
                                    DoBackProjection_OneSlice(PaddedSize, psf, Axis2D.XAxis, convolutionMethod);
                                }
                                else
                                {
                                    psf.PersistDataInMemory = true;
                                    psf.DoBackProjection(impulse, PaddedSize, convolutionMethod);
                                     ScriptingInterface.scriptingInterface.CreateGraph("BackProjection", psf.BackProjection);
                                    DoBackProjection_OneSlice(PaddedSize, psf, Axis2D.XAxis, convolutionMethod);
                                    psf.PersistDataInMemory = false;
                                }
                            }
                            //ScriptingInterface.scriptingInterface.SetProjectionSlice(psf);
                        });
            }

            for (int Proc = 0; Proc < ProcCount; Proc++)
            {
                BackProject[Proc].Start();
            }

            int ThreadsFinished = 0;
            //try
            {
                while (ThreadsFinished < ProcCount)
                {
                    ThreadsFinished = 0;
                    for (int i = 0; i < ProcCount; i++)
                    {
                        if (BackProject[i].ThreadState == ThreadState.Stopped)
                            ThreadsFinished++;
                    }
                    Application.DoEvents();
                }
            }
            //catch (Exception ex)
            {
                //Logger.LoggerForm.LogErrorMessage(ex);
                //throw new Exception(ex.InnerException.Message + ex.InnerException.StackTrace);
            }

        }
        public void DoBackProjection_ThreadedMediumMemory(int PaddedSize, aProjectionSlice[] SliceArray, double[] impulse, ConvolutionMethod convolutionMethod)
        {
            if (SliceArray[0].ProjectionRank == PhysicalArrayRank.Array1D)
                DoBackProjection_ThreadedMediumMemory1D(PaddedSize, SliceArray, impulse, convolutionMethod);
            else
                DoBackProjection_ThreadedMediumMemory2D(PaddedSize, SliceArray, impulse, convolutionMethod);
        }
        public void DoBackProjection_ThreadedMediumMemory(int PaddedSize, aProjectionSlice[] SliceArray, double[,] impulse, ConvolutionMethod convolutionMethod)
        {
            if (SliceArray[0].ProjectionRank == PhysicalArrayRank.Array1D)
                throw new Exception("You must have 2D projections to use a 2D convolution impulse response.");
            else
                DoBackProjection_ThreadedMediumMemory2D(PaddedSize, SliceArray, impulse, convolutionMethod);
        }

        public void DoBackProjection_ThreadedMediumMemory(int PaddedSize, IronPython.Runtime.List SliceArray, double[] impulse, ConvolutionMethod convolutionMethod)
        {

            aProjectionSlice[] Slices = new aProjectionSlice[SliceArray.Count];
            for (int i = 0; i < SliceArray.Count; i++)
                Slices[i] = (aProjectionSlice)SliceArray[i];

            if (Slices[0].ProjectionRank == PhysicalArrayRank.Array1D)
                DoBackProjection_ThreadedMediumMemory1D(PaddedSize, Slices, impulse, convolutionMethod);
            else
                DoBackProjection_ThreadedMediumMemory2D(PaddedSize, Slices, impulse, convolutionMethod);
        }
        public void DoBackProjection_ThreadedMediumMemory(int PaddedSize, IronPython.Runtime.List SliceArray, double[,] impulse, ConvolutionMethod convolutionMethod)
        {
            aProjectionSlice[] Slices = new aProjectionSlice[SliceArray.Count];
            for (int i = 0; i < SliceArray.Count; i++)
                Slices[i] = (aProjectionSlice)SliceArray[i];

            if (Slices[0].ProjectionRank == PhysicalArrayRank.Array1D)
                throw new Exception("You must have 2D projections to use a 2D convolution impulse response.");
            else
                DoBackProjection_ThreadedMediumMemory2D(PaddedSize, Slices, impulse, convolutionMethod);
        }
        #endregion

        #region ForwardProjection
        public Bitmap DoMIPProjection_OneSlice(double AngleRadians)
        {
            PhysicalArray SliceProjection;
            MathHelpLib.Axis RotationAxis = MathHelpLib.Axis.ZAxis;
            if (mDensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
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

                SliceProjection = mDensityGrid.ProjectMIP(vec, Point3D.CrossProduct(vec, vRotationAxis));
                return SliceProjection.MakeBitmap();
            }
        }


        public virtual aProjectionSlice CreateForwardProjection1D(double AngleDegrees)
        {
            double rAngle = AngleDegrees / 180d * Math.PI;

            double sTheta = Math.Sin(rAngle);
            double cTheta = Math.Cos(rAngle);

            aProjectionSlice ps = new ProjectionSliceMemory();
            ps.Angle = rAngle;
            PhysicalArray tArray = ps.Projection;

            throw new Exception("No Longer Implimented");

           // mDensityGrid.UnSmearArray2DInterpolate(ref tArray, new Point3D(sTheta, cTheta, 0), new Point3D(-1 * cTheta, sTheta, 0));

           // return ps;
        }

        public virtual aProjectionSlice CreateForwardProjection2D(Axis DesiredRotationAxis, double AngleDegrees)
        {
            double rAngle = AngleDegrees / 180d * Math.PI;

            Point3D vRotationAxis = new Point3D();
            Point3D axis = new Point3D();

            if (DesiredRotationAxis == Axis.XAxis)
            {
                vRotationAxis = new Point3D(1, 0, 0);
                axis = new Point3D(0, 1, 0);
            }
            else if (DesiredRotationAxis == Axis.YAxis)
            {
                vRotationAxis = new Point3D(0, 1, 0);
                axis = new Point3D(0, 0, 1);
            }
            else if (DesiredRotationAxis == Axis.ZAxis)
            {
                vRotationAxis = new Point3D(0, 0, 1);
                axis = new Point3D(0, 1, 0);
            }

            Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, rAngle);
            aProjectionSlice ps = new ProjectionSliceMemory();
            ps.Angle = rAngle;
            PhysicalArray tArray = ps.Projection;
            mDensityGrid.UnSmearArray2DInterpolate(ref tArray, vec, Point3D.CrossProduct(vec, vRotationAxis));

            return ps;
        }


        public virtual void CreateForwardProjectionFromCube(ref PhysicalArray ForwardProjection, double AngleRadians)
        {
            Axis RotationAxis = Axis.ZAxis;
            if (mDensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
            {
                throw new Exception("Not yet implimented");

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

                Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, AngleRadians);

                mDensityGrid.UnSmearArray2DInterpolate(ref ForwardProjection, vec, Point3D.CrossProduct(vec, vRotationAxis));
            }

        }
        #endregion

        #region ART
        Random rnd = new Random();
        public void DoARTInitialized(int NumberIterations)
        {
            int gap = (int)((double)mAllSlices.Count / 5d);
            for (int Iteration = 0; Iteration < NumberIterations; Iteration++)
            {
                double correctionFactor = -1 * (1 - (double)Iteration / (double)NumberIterations);
                for (int i = 0; i < mAllSlices.Count; i++)
                {
                    int slice = (i * gap + i + Iteration) % mAllSlices.Count;// (int)(mAllSlices.Count * rnd.NextDouble());
                    double angle = mAllSlices[slice].Angle;

                    //show the back projection
                    ScriptingInterface.scriptingInterface.CreateGraph("Back", mAllSlices[slice].Projection);

                    //DoBackProjection_OneSlice(mAllSlices[slice].Projection, angle);

                    PhysicalArray FProjection = new PhysicalArray(mAllSlices[slice].Projection, PhysicalArrayType.DoubleArray);
                    //Create the forward projection
                    //aProjectionSlice ps= CreateForwardProjectionUnscaled  (Axis.ZAxis, angle ,mAllSlices[slice].Projection  );
                    CreateForwardProjectionFromCube(ref FProjection, angle);

                    ScriptingInterface.scriptingInterface.CreateGraph("Forward", FProjection);

                    //get the difference
                    FProjection.SubtractCenteredInPlace(mAllSlices[slice].Projection);
                    PhysicalArray correction = correctionFactor * FProjection;
                    //double sumCorrection =(double) correction.Sum();
                    //System.Diagnostics.Debug.Print(sumCorrection.ToString()+ "\n" + FProjection.ReferenceDataDouble.AverageArray ().ToString());
                    if (Iteration > 1)
                    {
                        double[, ,] data = correction.ReferenceDataDouble;
                        double[, ,] dataOut = new double[data.GetLength(0), data.GetLength(1), data.GetLength(2)];
                        for (int y = 0; y < data.GetLength(1); y++)
                        {
                            dataOut[0, y, 0] = data[0, y, 0];
                            dataOut[0, y, data.GetLength(2) - 1] = data[0, y, data.GetLength(2) - 1];
                            for (int x = 1; x < data.GetLength(2) - 1; x++)
                            {
                                if (data[0, y, x + 1] > data[0, y, x - 1])
                                {
                                    if (data[0, y, x] > data[0, y, x + 1])
                                        dataOut[0, y, x] = data[0, y, x + 1];
                                    else
                                        dataOut[0, y, x] = data[0, y, x];
                                }
                                else
                                {
                                    if (data[0, y, x] > data[0, y, x - 1])
                                        dataOut[0, y, x] = data[0, y, x - 1];
                                    else
                                        dataOut[0, y, x] = data[0, y, x];

                                }
                                //dataOut[0, y, x] = (data[0, y , x+1] + data[0, y, x] + data[0, y, x-1]) / 3d;
                            }
                        }
                        correction.ReferenceDataDouble = dataOut;
                    }
                    ScriptingInterface.scriptingInterface.CreateGraph("Sub", correction);

                    //do the correction
                    DoBackProjection_OneSlice(correction, angle);
                    Application.DoEvents();
                }
            }
        }
        #endregion

        #region Files
        public void SaveDensityData(string Filename)
        {
            string Extension = Path.GetExtension(Filename).ToLower();
            if (Extension == "." || Extension == "")
            {
                Extension = ".cct";
            }
            if (Extension == ".cct")
            {
                #region SaveRawFile
                string outFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename) + ".cct";
                FileStream BinaryFile = new FileStream(outFile, FileMode.Create, FileAccess.Write);
                BinaryWriter Writer = new BinaryWriter(BinaryFile);

                Writer.Write((Int32)mDensityGrid.ArrayRank);

                Writer.Write((Int32)mDensityGrid.GetLength(Axis.XAxis));
                Writer.Write((Int32)mDensityGrid.GetLength(Axis.YAxis));
                Writer.Write((Int32)mDensityGrid.GetLength(Axis.ZAxis));

                Writer.Write((double)mDensityGrid.PhysicalStart(Axis.XAxis));
                Writer.Write((double)mDensityGrid.PhysicalEnd(Axis.XAxis));

                Writer.Write((double)mDensityGrid.PhysicalStart(Axis.YAxis));
                Writer.Write((double)mDensityGrid.PhysicalEnd(Axis.YAxis));

                Writer.Write((double)mDensityGrid.PhysicalStart(Axis.ZAxis));
                Writer.Write((double)mDensityGrid.PhysicalEnd(Axis.ZAxis));

                for (int z = 0; z < mDensityGrid.GetLength(Axis.ZAxis); z++)
                {
                    for (int y = 0; y < mDensityGrid.GetLength(Axis.YAxis); y++)
                    {
                        for (int x = 0; x < mDensityGrid.GetLength(Axis.XAxis); x++)
                        {
                            Writer.Write((double)mDensityGrid[x, y, z]);
                        }
                    }
                }
                Writer.Close();
                BinaryFile.Close();
                #endregion
            }
            else if (Extension == ".bin")
            {
                #region Save Bin
                FileStream BinaryFile = new FileStream(Filename, FileMode.Create, FileAccess.Write);
                BinaryWriter Writer = new BinaryWriter(BinaryFile);

                for (int z = 0; z < mDensityGrid.GetLength(Axis.ZAxis); z++)
                {
                    for (int y = 0; y < mDensityGrid.GetLength(Axis.YAxis); y++)
                    {
                        for (int x = 0; x < mDensityGrid.GetLength(Axis.XAxis); x++)
                        {
                            Writer.Write((double)mDensityGrid[x, y, z]);
                        }
                    }
                }
                Writer.Close();
                BinaryFile.Close();
                #endregion
            }
            else if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                #region Save as Images
                if (mDensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
                {
                    Bitmap b = mDensityGrid.MakeBitmap();
                    b.Save(Filename);
                }
                else
                {
                    int ZSlices = mDensityGrid.GetLength(Axis.ZAxis);
                    string outFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename);

                    for (int z = 0; z < ZSlices; z++)
                    {
                        double[,] Slice = (double[,])mDensityGrid[true, null, null, z, z];
                        Bitmap b = Slice.MakeBitmap();
                        b.Save(outFile + string.Format("_{0:000}", z) + Extension);
                    }
                }
                #endregion
            }
        }
        public void OpenDensityData(string Filename)
        {
            string Extension = Path.GetExtension(Filename).ToLower();
            if (Path.GetExtension(Filename).ToLower() == ".cct")
            {
                #region Open Raw
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);
                int sizeX, sizeY, sizeZ;

                PhysicalArrayRank ArrayRank = (PhysicalArrayRank)Reader.ReadInt32();

                sizeX = Reader.ReadInt32();
                sizeY = Reader.ReadInt32();
                sizeZ = Reader.ReadInt32();

                mDensityGrid = new PhysicalArray(sizeX, sizeY, sizeZ,
                    Reader.ReadDouble(), Reader.ReadDouble(),
                    Reader.ReadDouble(), Reader.ReadDouble(),
                    Reader.ReadDouble(), Reader.ReadDouble()
                    );

                byte[] buffer = new byte[sizeX * sizeY * sizeZ * sizeof(double)];
                Reader.Read(buffer, 0, buffer.Length);

                mDensityGrid.CopyInDoubleArray(buffer);
              
                Reader.Close();
                BinaryFile.Close();
                #endregion
            }
            else if (Path.GetExtension(Filename).ToLower() == ".bin")
            {
                #region Open Bin
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);

                long nPoints = (int)(BinaryFile.Length / 8d);
                nPoints = (long)Math.Pow((double)nPoints, (1d / 3d));

                int sizeX, sizeY, sizeZ;

                sizeX = (int)nPoints;
                sizeY = (int)nPoints;
                sizeZ = (int)nPoints;

                mDensityGrid = new PhysicalArray(sizeX, sizeY, sizeZ,
                    -1, 1, -1, 1, -1, 1
                    );

                for (int z = 0; z < sizeZ; z++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[x, y, z] = Reader.ReadDouble();
                        }
                    }
                }
                Reader.Close();
                BinaryFile.Close();
                #endregion
            }
            else if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                #region Open 2D image
                int sizeX, sizeY;

                Bitmap b = new Bitmap(Filename);
                sizeX = b.Width;
                sizeY = b.Height;
                mDensityGrid = new PhysicalArray(sizeX, sizeY, -1, 1, -1, 1);


                double[,] Data = MathImageHelps.ConvertToDoubleArray(b, false);
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        mDensityGrid[x, y] = Data[x, y];
                    }
                }

                #endregion
            }
        }
        public void OpenDensityData(string[] Filenames)
        {
            string Extension = Path.GetExtension(Filenames[0]).ToLower();
            if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                Filenames = MathStringHelps.SortNumberedFiles(Filenames);

                int sizeX, sizeY, sizeZ;

                Bitmap b = new Bitmap(Filenames[0]);
                sizeX = b.Width;
                sizeY = b.Height;
                sizeZ = Filenames.Length;
                mDensityGrid = new PhysicalArray(sizeX, sizeY, sizeZ, -1, 1, -1, 1, -1, 1);

                for (int z = 0; z < sizeZ; z++)
                {
                    b = new Bitmap(Filenames[z]);
                    double[,] Data = MathImageHelps.ConvertToDoubleArray(b, false);
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[x, y, z] = Data[x, y];
                        }
                    }
                }
            }
            else if (Extension == ".ivg" )
            {
                Filenames = MathStringHelps.SortNumberedFiles(Filenames);

                int sizeX, sizeY, sizeZ;

                ImageHolder b = MathHelpsFileLoader.LoadIVGFile(Filenames[0]);
                sizeX = b.Width;
                sizeY = b.Height;
                sizeZ = Filenames.Length;
                mDensityGrid = new PhysicalArray(sizeX, sizeY, sizeZ, -1, 1, -1, 1, -1, 1);

                for (int z = 0; z < sizeZ; z++)
                {
                    b = MathHelpsFileLoader.LoadIVGFile(Filenames[z]);
                    double[,] Data = MathImageHelps. ConvertToDoubleArray (b, false);
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[y,x, z] = Data[y,x];
                        }
                    }
                }

            }
        }
        #endregion
    }
}
