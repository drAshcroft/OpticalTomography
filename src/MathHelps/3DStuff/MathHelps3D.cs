using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace MathHelpLib
{

    /// <summary>
    /// Axis specification for 3D arrays and transforms
    /// </summary>
    public enum Axis
    {
        XAxis = 0, YAxis = 1, ZAxis = 2
    }
    /// <summary>
    ///  Axis specification for 2D arrays and transforms
    /// </summary>
    public enum Axis2D
    {
        XAxis = 0, YAxis = 1
    }

    public static class Math3DHelps
    {
        /// <summary>
        /// Rotates around z axis
        /// </summary>
        /// <param name="Angle">rotation in radians</param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point3D RotatePoint(double XYAngle, Point3D point)
        {
            return Point3D.Roll(point, XYAngle);
        }

        /// <summary>
        /// Rotates point around all axis, yaw, pitch, then roll
        /// </summary>
        /// <param name="Angle">rotation in radians</param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point3D RotatePoint(double XYAngle, double YZAngle, double XZangle, Point3D point)
        {
            Point3D temp = Point3D.Yaw(point, XZangle);
            temp.Pitch(YZAngle);
            temp.Roll(XYAngle);
            return temp;
        }

        /// <summary>
        ///Rotates point around all axis, yaw, pitch, then roll
        /// </summary>
        /// <param name="Angle"> Rotation in radians</param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static Point3D RotatePoint(double XYAngle, double YZAngle, double XZAngle, double X, double Y, double Z)
        {

            return RotatePoint(XYAngle, YZAngle, XZAngle, new Point3D(X, Y, Z));
        }

        /// <summary>
        /// Normalizes point so R =1 
        /// </summary>
        /// <param name="inPoint"></param>
        /// <returns></returns>
        public static Point3D NormalizePoint(Point3D inPoint)
        {
            double r = Math.Sqrt(inPoint.X * inPoint.X + inPoint.Y * inPoint.Y + inPoint.Z * inPoint.Z);
            return new Point3D(inPoint.X / r, inPoint.Y / r, inPoint.Z / r);
        }

        /// <summary>
        ///  Normalizes point so R =1 
        /// </summary>
        /// <param name="inPoint"></param>
        /// <returns></returns>
        public static Point3D NormalizePoint(double X, double Y, double Z)
        {
            double r = Math.Sqrt(X * X + Y * Y + Z * Z);
            return new Point3D(X / r, Y / r, Z / r);
        }

        /// <summary>
        /// Returns a true if a point is inside a rotated, offset ellipse
        /// </summary>
        /// <param name="x">check point X</param>
        /// <param name="y">check point X</param>
        /// <param name="z">check point X</param>
        /// <param name="CenterX">center of ellipse X</param>
        /// <param name="CenterY">center of ellipse X</param>
        /// <param name="CenterZ">center of ellipse X</param>
        /// <param name="MajorAxis"></param>
        /// <param name="MinorAxis"></param>
        /// <param name="Rotation">rotation of ellipse around z axis in radian</param>
        /// <returns></returns>
        public static bool IsInsideEllipse(double x, double y, double z, double CenterX, double CenterY, double CenterZ, double MajorAxis, double MinorAxis, double Rotation)
        {
            //translate and rotate the ellipse to 0,0,0 with no rotation and then check the point
            Point3D newPoint = new Point3D(x - CenterX, y - CenterY, z - CenterZ);
            newPoint = RotatePoint(-1 * Rotation, newPoint);

            double xp = Math.Abs(newPoint.X / MajorAxis);
            double yp = Math.Abs(newPoint.Y / MinorAxis);
            double zp = Math.Abs(newPoint.Z / MinorAxis);

            if (xp * xp + yp * yp + zp * zp < 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// returns true is point (x,y) is within the rectangle defined by center (centerX,centerY) with specified axis and rotation
        /// </summary>
        /// <param name="x">check point</param>
        /// <param name="y">check point</param>
        /// <param name="CenterX">center of rectangle</param>
        /// <param name="CenterY">center of rectangle</param>
        /// <param name="MajorAxis">Height of rectangle</param>
        /// <param name="MinorAxis">Width of rectangle</param>
        /// <param name="Rotation">Rotation is in radians around z axis</param>
        /// <returns></returns>
        public static bool IsInsideRectangle(double x, double y, double z, double CenterX, double CenterY, double CenterZ, double MajorAxis, double MinorAxis, double Rotation)
        {
            Point3D newPoint = new Point3D(x - CenterX, y - CenterY, z - CenterZ);
            newPoint = RotatePoint(-1 * Rotation, newPoint);
            double xp = Math.Abs(newPoint.X / MajorAxis);
            double yp = Math.Abs(newPoint.Y / MinorAxis);
            double zp = Math.Abs(newPoint.Z / MinorAxis);

            if (xp <= 1 && yp <= 1 && zp < 1)
                return true;
            else
                return false;

        }

        /*

        /// <summary>
        /// produces a MIP projection assuming that the Z axis is the desired rotation axis and the X axis 
        /// is the fast scan axis
        /// </summary>
        /// <param name="Angle"></param>
        /// <returns></returns>
        public static double[,] MIPProjection(double AngleRadians, double[, ,] DataArray)
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

            Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, AngleRadians);

            Point3D Direction = vec;
            Point3D FastScanDirection = Point3D.CrossProduct(vec, vRotationAxis);

            Direction.Normalize();
            FastScanDirection.Normalize();
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            int LX = DataArray.GetLength(2);
            int LY = DataArray.GetLength(1);
            int LZ = DataArray.GetLength(0);


            double[,] OutArray = new double[DataArray.GetLength(1), DataArray.GetLength(0)];

            int LOutX = OutArray.GetLength(0)-1;
            int LOutY = OutArray.GetLength(1)-1;

            double sX = -1;
            double sY = -1;
            double sZ = -1;

            double sOutX = -1;
            double sOutY = -1;
            double stepOutX = 2d / (double)OutArray.GetLength(0);
            double stepOutY = 2d / (double)OutArray.GetLength(1);

            double stepX = 2d / (double)DataArray.GetLength(2);
            double stepY = 2d / (double)DataArray.GetLength(1);
            double stepZ = 2d / (double)DataArray.GetLength(0);

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

            double uX, uY,  Xu, Yu;
            
            ///this works by defining a plane normalX normalY that is placed under the screen.  Then each voxel in the cube is 
            ///projected by defining the vector R from the origin to the voxel.  the projection coordinates are then determined 
            ///by taking the dot product of the normals and R.
            unsafe
            {
                int zOffest = DataArray.GetLength(2) * DataArray.GetLength(1);
                int yOffset = DataArray.GetLength(2);
                fixed (double* POut = OutArray)
                {
                    double* POutZ = POut;
                    double* POutY;
                    double* POutX;
                    double VoxelVal;
                    int xL, yL;
                    for (int zI = 0; zI < LZ; zI++)
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

                                        xL = (int)tXI;
                                        yL = (int)tYI;
                                        uX = tXI - xL;
                                        uY = tYI - yL;
                                        Xu = (1 - uX);
                                        Yu = (1 - uY);

                                        VoxelVal = (*POutX);
                                        OutArray[ yL, xL] += VoxelVal * Xu * Yu;
                                        OutArray[ yL, xL + 1] += VoxelVal * uX * Yu;
                                        OutArray[ yL + 1, xL] += VoxelVal * Xu * uY;
                                        OutArray[ yL + 1, xL + 1] += VoxelVal * uX * uY;
                                    }
                                POutX++;
                            }
                            POutY += yOffset;
                        }
                        POutZ += zOffest;
                    }
                }
            }
            return OutArray;
        }*/
    }
}