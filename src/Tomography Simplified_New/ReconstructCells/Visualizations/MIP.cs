﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageProcessing._3D;
using System.Threading.Tasks;
using System.Drawing;

namespace ReconstructCells.Visualizations
{
    public class MIP : Tomography.ReconTemplate
    {
        int nForwardProjections = 40;
        bool SaveMovie = false ;
        string MovieFilename = "";
        double angleStep = 0;

        Image<Gray, float>[] Projections;

        #region Properties

        #region Set
        public void  setNumberProjections(int nProjections)
        {
            nForwardProjections = nProjections;
        }
        public void setFileName(string filename)
        {
            SaveMovie = true;
            MovieFilename = filename;
        }
        #endregion

        #region Get
        public Image<Gray, float>[] getProjections()
        {
            return Projections;
        }

        #endregion

        #endregion


        #region Code
        private void BatchForwardProjectM(int imageNumber)
        {
            double Angle = imageNumber * angleStep;
            var image = DoForwardProjection_OneSlice(1, 1, Angle, Axis2D.YAxis);
            //var image = DoMIPProjection_OneSlice( Angle) ;

            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {
                    if (image.Data[y, x, 0] < 0)
                        image.Data[y, x, 0] = 0;
                }


            Projections[imageNumber] = image;
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
        private Image<Gray, float> DoForwardProjection_OneSlice(double PaintingWidth, double PaintingHeight, double AngleRadians, Axis2D ConvolutionAxis)
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

            //AngleRadians += Math.PI / 2;

            Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, AngleRadians);

            Image<Gray, float> slice = new Image<Gray, float>(DensityGrid.GetLength(0), DensityGrid.GetLength(0));

            UnSmearArray2DWhole(AngleRadians, ref slice, PaintingWidth, PaintingHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));

            return slice;
        }

        private void UnSmearArray2DWhole(double AngleRadians, ref Image<Gray, float> Slice, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
        {
            // var b= DensityGrid.ShowCross();
            // var i = b.Length;

            float[, ,] PaintingArray = Slice.Data;

            //make sure all the vectors are the right size
            Direction.Normalize();
            FastScanDirection.Normalize();
            //determine all the important planes in the 3D space and all the step
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            ///get the existing recon data.  the jagged array is used so the data can be checked out by each thread

            double r = Math.Sqrt(2 * .5 * .5);

            #region constants
            //get all the dimensions
            int LI = DensityGrid.GetLength(2);
            int LJ = DensityGrid.GetLength(1);
            int LK = DensityGrid.GetLength(0);

            int LsI = Slice.Width;
            int LsJ = Slice.Height;
            int LsI_1 = LsI - 1;
            int LsJ_1 = LsJ - 1;

            double halfI, halfJ, halfK, halfIs, halfJs;
            double VolToSliceX, VolToSliceY, VolToSliceZ;

            halfI = (double)LI / 2;
            halfJ = (double)LJ / 2;
            halfK = (double)LK / 2;

            halfIs = (double)LsI / 2;
            halfJs = (double)LsJ / 2;

            VolToSliceX = (xMax - xMin) / LI * (double)Slice.Height / (PaintingWidth);
            VolToSliceY = (yMax - yMin) / LJ * (double)Slice.Height / (PaintingWidth);
            VolToSliceZ = (zMax - zMin) / LK * (double)Slice.Width / (PaintingHeight);

            #endregion

            double sX, sY;
            int iX, iY;
            float uX, uY;
            float value;
            // unsafe
            {
                //we make the math unchecked to make it go faster.  there seem to be enough checks beforehand
                //  unchecked
                {
                    for (int K_ = 0; K_ < LsJ; K_++)
                    {
                        bool OutofGrid = false;
                        for (int I_ = 0; I_ < LsI; I_++)
                        {
                            for (int s = -1; s <= 1; s += 2)
                            {
                                sX = (I_ - halfIs) * FastScanDirection.X + halfI;
                                sY = (I_ - halfIs) * FastScanDirection.Y + halfJ;

                                for (int J_ = 0; J_ < LsJ; J_++)
                                {
                                    sX += Direction.X * s;
                                    sY += Direction.Y * s;

                                    if (sX < 1 || sY < 1 || sX >= LsI_1 || sY >= LsJ_1)
                                    {
                                        OutofGrid = false;
                                        break;
                                    }
                                    else
                                    {
                                        iX = (int)Math.Floor(sX);
                                        iY = (int)Math.Floor(sY);

                                        uX = (float)(sX - iX);
                                        uY = (float)(sY - iY);

                                        // value = DensityGrid[K_, iY, iX];

                                        value = DensityGrid[K_, iY, iX] * (1 - uX) * (1 - uY) +
                                            DensityGrid[K_, iY, iX + 1] * (uX) * (1 - uY) +
                                            DensityGrid[K_, iY + 1, iX] * (1 - uX) * (uY) +
                                            DensityGrid[K_, iY + 1, iX + 1] * (uX) * (uY);
                                        if (value > PaintingArray[I_, K_, 0])
                                            PaintingArray[I_, K_, 0] = value;

                                    }
                                }

                            }
                            if (OutofGrid)
                                break;
                        }
                    }
                }
            }
        }


        private Image<Gray, float>[] DoForwardProjections()
        {
            angleStep = 2 * Math.PI / nForwardProjections;

            Projections = new Image<Gray, float>[nForwardProjections];

            int GridSize = DensityGrid.GetLength(0);

            //now that the cell positions are estimated, fill in all the other cells
            // ParallelOptions po = new ParallelOptions();
            //po.MaxDegreeOfParallelism = Environment.ProcessorCount - 1;

            int numberOfImages = nForwardProjections;

            Parallel.For(0, numberOfImages, Program.threadingParallelOptions, x => BatchForwardProjectM(x));

            if (SaveMovie)
            {
                ImageProcessing._2D.MovieMaker.CreateAVIVideoEMGU(MovieFilename, Projections, 1);
            }

            return Projections;
        }
        #endregion


        protected override void RunNodeImpl()
        {
            DensityGrid = mPassData.DensityGrid;
            DoForwardProjections();
        }

    }
}
