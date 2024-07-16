﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageProcessing._3D;
using System.Drawing;
using MathLibrary;
using ImageProcessing;
using MathLibrary.FFT;

namespace ReconstructCells.Tomography
{
    public class obdSIRTRecon : ReconTemplate
    {
        #region Properties
        private bool Realign = false;
        #region Set
        public void setRealign(bool realign)
        {
            Realign = realign;
        }
        #endregion

        #region Get


        #endregion

        #endregion

        #region Code



        private float[, ,] SecondGrid;
        private float[, ,] ErrorGrid;
        private float[, ,] AlphaWeighting;

        const double P = 1.1;
        
        const double sigma = .1;
        double factor = 1 / P / Math.Pow(sigma, P) / 26;
        const double alpha = 100;
        float calmFactor = .001f;
        float errorWeight = (float)(1.0 / 50);
        double MaxR = 1;
        int batchStartIndex = 0;
        int batchSkip = 1;


        //  int[,] Mask;
        Random rnd = new Random();

        #region Parallel Math


        private double I(double delta)
        {
            delta = Math.Abs(delta);
            return Math.Pow(delta, P) + P * P * Math.Pow(delta, P - 1);

        }

        private double MRFRegularization(double centerValue, int x, int y, int z)
        {
            int x0 = x - 1;
            int y0 = y - 1;
            int z0 = z - 1;

            int x1 = x + 1;
            int y1 = y + 1;
            int z1 = z + 1;

            //get the corners
            double sum = I(DensityGrid[x0, y0, z0] - centerValue);
            sum += I(DensityGrid[x0, y1, z0] - centerValue);
            sum += I(DensityGrid[x0, y0, z1] - centerValue);
            sum += I(DensityGrid[x0, y1, z1] - centerValue);

            sum += I(DensityGrid[x1, y0, z0] - centerValue);
            sum += I(DensityGrid[x1, y1, z0] - centerValue);
            sum += I(DensityGrid[x1, y0, z1] - centerValue);
            sum += I(DensityGrid[x1, y1, z1] - centerValue);
            sum *= r;

            //then get the sides
            sum += I(DensityGrid[x0, y, z] - centerValue);
            sum += I(DensityGrid[x, y0, z] - centerValue);
            sum += I(DensityGrid[x, y, z0] - centerValue);
            sum += I(DensityGrid[x1, y, z] - centerValue);
            sum += I(DensityGrid[x, y1, z] - centerValue);
            sum += I(DensityGrid[x, y, z1] - centerValue);

            sum = factor * sum;

            return sum;
        }

        private void BatchAddAndZero(int sliceNumber)
        {
            float error, d;
            for (int x = 0; x < DensityGrid.GetLength(1); x++)
                for (int y = 0; y < DensityGrid.GetLength(2); y++)
                {
                    if (MaxR > (x - halfI) * (x - halfI) + (y - halfJ) * (y - halfJ) + (sliceNumber - halfK) * (sliceNumber - halfK))
                    {
                        {
                            error = ErrorGrid[sliceNumber, x, y] * errorWeight;
                            d = DensityGrid[sliceNumber, x, y] + error;

                            if (d < 0) d = 0;

                            SecondGrid[sliceNumber, x, y] = d;
                        }
                    }
                    else
                        SecondGrid[sliceNumber, x, y] = 0;

                    ErrorGrid[sliceNumber, x, y] = 0;
                    AlphaWeighting[sliceNumber, x, y] = 0;
                }
        }

        private void BatchMoveArray(int sliceNumber)
        {

            for (int x = 0; x < DensityGrid.GetLength(1); x++)
                for (int y = 0; y < DensityGrid.GetLength(2); y++)
                {
                    DensityGrid[sliceNumber, x, y] = SecondGrid[sliceNumber, x, y];
                }
        }

        #endregion

        #region Statistical


        private void BatchStatProject(int imageNumber)
        {
            //imageNumber *= 20;
            //imageNumber = (imageNumber * 100 + imageNumber) % Library.Count;
            imageNumber = (batchStartIndex + imageNumber * batchSkip) % Library.Count;

            double Angle = 2 * Math.PI * (double)imageNumber / Library.Count;
            DoSIRTProjection_OneSlice(Library[imageNumber], Angle, Axis2D.YAxis);
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
        private void DoSIRTProjection_OneSlice(Image<Gray, float> Slice, double AngleRadians, Axis2D ConvolutionAxis)
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

            Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, AngleRadians);

            SmearArray2DWhole(AngleRadians, Slice, vec, Point3D.CrossProduct(vec, vRotationAxis));
        }

        private void SmearArray2DWhole(double AngleRadians, Image<Gray, float> Slice, Point3D Direction, Point3D FastScanDirection)
        {
            float[, ,] PaintingArray = Slice.Data;
            Image<Gray, float> diffImage = Slice.CopyBlank();
            float[, ,] difference = diffImage.Data;
            float[] counts = new float[diffImage.Width];
            //make sure all the vectors are the right size
            Direction.Normalize();
            FastScanDirection.Normalize();
            //determine all the important planes in the 3D space and all the step
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            int K_ = 0;
            int FinishedCount = 0;
            double sX, sY, sdotI;
            float val;
            float u;
            int lower_sI, lower_sJ;

            unsafe
            {
                //we make the math unchecked to make it go faster.  there seem to be enough checks beforehand
                unchecked
                {
                    //there is a lot of thread contention, so look through the whole stack and find an open slice, and work on it

                    int StartI = 0;// (int)halfK;// (new Random(DateTime.Now.Millisecond)).Next(LK);

                    #region Calculate all the weights
                    float[,] Weights = new float[DensityGrid.GetLength(1), DensityGrid.GetLength(2)];
                    int[,] Coord = new int[DensityGrid.GetLength(1), DensityGrid.GetLength(2)];
                    for (int J_ = 0; J_ < LJ; J_++)
                    {
                        //tranform to slice index coords
                        sY = (J_ - halfI) * VolToSliceY;
                        for (int I_ = 0; I_ < LI; I_++)
                        {

                            //tranform to slice index coords
                            sX = (I_ - halfI) * VolToSliceX;

                            sdotI = halfIs + (sX * FastScanDirection.X + sY * FastScanDirection.Y);
                            //make sure that we are still in the recon volumn
                            if (sdotI > 0 && sdotI < LsI_1)
                            {
                                lower_sI = (int)Math.Floor(sdotI);
                                u = (float)(sdotI - lower_sI);
                                Weights[J_, I_] = u;
                                Coord[J_, I_] = lower_sI;
                                counts[lower_sI] += (1 - u);
                                counts[lower_sI + 1] += (u);
                            }
                            else
                                Weights[J_, I_] = -1;
                        }
                    }
                    #endregion


                    #region UnSmear slice
                    for (K_ = 0; K_ < LK; K_++)
                    {

                        for (int J_ = 0; J_ < LJ; J_++)
                        {

                            for (int I_ = 0; I_ < LI; I_++)
                            {
                                if (Weights[J_, I_] != -1)
                                {
                                    lower_sI = Coord[J_, I_];
                                    u = Weights[J_, I_];
                                    val = DensityGrid[K_, J_, I_];

                                    difference[lower_sI, K_, 0] += val * (1 - u);
                                    difference[lower_sI + 1, K_, 0] += val * u;
                                }
                            }
                        }
                    }
                    #endregion

                    int halfIndex = (int)halfJ;


                    #region DoDifference

                    for (int x = 0; x < LsJ; x++)
                    {
                        float dd, v;
                        for (int y = 0; y < LsI; y++)
                        {
                            dd = difference[y, x, 0];
                            v = PaintingArray[y, x, 0];
                            difference[y, x, 0] = calmFactor * (v - dd);
                        }
                    }

                    #endregion

                    double d = difference.MaxArray();

                    float df, df2;
                    df =(float)d;
                    df = 0;
                    StartI = rnd.Next(LK);
                    for (int i = 0; i < LK; i++)
                    {
                        K_ = (i + StartI) % LK;
                        LockIndicator[K_] = true;
                        FinishedCount++;


                        //indicate that the thread is locked
                        lock (LockArray[K_])
                        {
                            lower_sJ = K_;

                            #region Smear Slice


                            for (int J_ = 0; J_ < LJ; J_++)
                            {
                                for (int I_ = 0; I_ < LI; I_++)
                                {
                                    if (Weights[J_, I_] != -1)
                                    {
                                        lower_sI = Coord[J_, I_];
                                        u = Weights[J_, I_];

                                        df = difference[lower_sI + 1, lower_sJ, 0];
                                        df2 = difference[lower_sI, lower_sJ, 0];

                                        ErrorGrid[K_, J_, I_] += df2 * (1 - u) + df * (u);
                                        AlphaWeighting[K_, J_, I_] += u * u + (1 - u) * (1 - u);
                                    }
                                }
                            }

                            #endregion
                        }
                        LockIndicator[K_] = false;
                    }

                    // ErrorGrid.SaveCross(@"c:\temp\error.bmp");

                }
            }
        }
        #endregion


        OnDemandImageLibrary realLib;
        protected override void RunNodeImpl()
        {
            Library = mPassData.Library;
            DensityGrid = mPassData.DensityGrid;
            DoProjections();
        }

        int ProjectionNumber = 0;
        public float[, ,] DoProjections()
        {

          //  Library = new OnDemandImageLibrary(@"c:\temp\deconv", true, @"c:\temp", false);
            //for (int i = 0; i < Library.Count; i++)
            //    Library[i] = new Image<Gray, float>(samp2(Library[i].Data, DensityGrid.GetLength(0) + 1));

            // BatchDeconv(9);
            int GridSize = Library[0].Width;

            SetConstants(new Image<Gray, float>(samp2(Library[1].Data, DensityGrid.GetLength(0) + 1)));

            // ParallelOptions po = new ParallelOptions();
            //po.MaxDegreeOfParallelism = Environment.ProcessorCount;

            int numberOfImages = Library.Count;

            // CleanProjections();
            realLib = new OnDemandImageLibrary(Library);


           // Parallel.For(0, (int)(numberOfImages), Program.threadingParallelOptions, x => BatchDeconv(x));

            //if (Realign)
            //{
            //    Registration.AlignByRecon ar = new Registration.AlignByRecon();
            //    ar.SetInput(mPassData);
            //    ar.setNumberOfProjections(125);
            //    ar.setScale(1);
            //    ar.RunNode();
            //}
            Library.SaveImages(@"c:\temp\deconv\image.tif");
            for (int i = 0; i < Library.Count; i++)
                Library[i] = new Image<Gray, float>(samp2(Library[i].Data, DensityGrid.GetLength(0) + 1));


            //  return null;
            Mask = new int[DensityGrid.GetLength(1), DensityGrid.GetLength(2)];

            MaxR = DensityGrid.GetLength(1) / 2;
            MaxR *= MaxR;

            for (int j = 0; j < DensityGrid.GetLength(1); j++)
                for (int k = 0; k < DensityGrid.GetLength(2); k++)
                {
                    double y = (j - HalfJ) / RY;
                    double z = (k - HalfK) / RZ;
                    if ((y * y + z * z) > 1)
                    {
                        Mask[j, k] = 1;
                    }
                }

            for (int j = 0; j < DensityGrid.GetLength(1); j++)
                for (int k = 0; k < DensityGrid.GetLength(2); k++)
                    for (int n = 0; n < DensityGrid.GetLength(0); n++)
                        if (DensityGrid[n, j, k] < 0)
                            DensityGrid[n, j, k] = 0;

            var b = DensityGrid.ShowCross();

            b[0] = Mask.MakeBitmap();

            ErrorGrid = new float[DensityGrid.GetLength(0), DensityGrid.GetLength(1), DensityGrid.GetLength(2)];
            AlphaWeighting = new float[DensityGrid.GetLength(0), DensityGrid.GetLength(1), DensityGrid.GetLength(2)];
            SecondGrid = new float[DensityGrid.GetLength(0), DensityGrid.GetLength(1), DensityGrid.GetLength(2)];

            ProjectionNumber++;
            int nIterations = 4;

            // BatchSIRTProject(0);
            double m = 0;
            int hx = DensityGrid.GetLength(1) / 2;
            for (int i = 0; i < DensityGrid.GetLength(0); i++)
                m += DensityGrid[i, hx, hx];

            double mP = Library[1].Data[Library[1].Height / 2, Library[1].Width / 2, 0];

            DensityGrid.DivideInPlace((float)(m / mP));

            for (int j = 0; j < nIterations; j++)
            {
                calmFactor = .5f/(j+1);// 3f / DensityGrid.GetLength(0);

                batchStartIndex = j;

                Parallel.For(0, (int)(numberOfImages / (double)batchSkip), Program.threadingParallelOptions, x => BatchStatProject(x));

                float sum = ErrorGrid.AverageArrayWithThreshold(-500);
                float s2 = ErrorGrid.MinArray();
                float s3 = ErrorGrid.MaxArray();

                errorWeight = (float)((20 * batchSkip / (numberOfImages)) / Math.Sqrt(j + 2));
                sum += s2 + s3;
                Parallel.For(0, (int)DensityGrid.GetLength(0), Program.threadingParallelOptions, x => BatchAddAndZero(x));

                double m2 = DensityGrid.MaxArray();
                double m3 = SecondGrid.MaxArray();

                float[, ,] tempGrid = DensityGrid;

                DensityGrid = SecondGrid;

                SecondGrid = tempGrid;

                ProjectionNumber++;
                m = m + m2 + m3;
            }


            return DensityGrid;
        }
        #endregion



        #region SR

        private void BatchDeconv(int imageNumber)
        {
            Library[imageNumber] = DeconvolveImage(Library[imageNumber], 2, 10, 2);
        }

        private Image<Gray, float> DeconvolveImage(Image<Gray, float> image, int nMacroIter, int nPSFIter, int nImageIter)
        {
            Image<Gray, float> PSF = new Image<Gray, float>(100, 100);
            PSF = PSF.Add(new Gray(1d / Math.Sqrt(PSF.Width)));

            Image<Gray, float> psfBounds = PSF.CopyBlank();
            int hW = PSF.Width / 2;
            double R;
            double sigma = 3;
            sigma = 2 / sigma / sigma;
            for (int xx = 0; xx < psfBounds.Width; xx++)
                for (int yy = 0; yy < psfBounds.Height; yy++)
                {
                    R = (Math.Pow(xx - hW, 2) + Math.Pow(yy - hW, 2));
                    psfBounds.Data[yy, xx, 0] = (float)(1 * Math.Exp(-1 * (R * sigma)));
                }

            Image<Gray, float> X = null;
            float[, ,] temp;

            for (int i = 0; i < nMacroIter; i++)
            {
                temp = psfBounds.Copy().Data;
                temp.NormalizeArray();
                X = obd2(X, image.Data, temp, new int[] { nPSFIter, nImageIter }, 1, false, psfBounds.Data);
            }
            X = new Image<Gray, float>(samp2(X.Data, image.Width));
            return X;
        }

        #region matlab fakes
        private int[] size(float[, ,] x)
        {
            return new int[] { x.GetLength(0), x.GetLength(1) };
        }
        private int floor(float x)
        {
            return (int)Math.Floor(x);
        }

        private int ceil(float x)
        {
            return (int)Math.Ceiling(x);
        }

        private int floor(double x)
        {
            return (int)Math.Floor(x);
        }
        private void error(object o)
        {
            Program.WriteLine(o.ToString());

        }
        private int isempty(object o)
        {
            if (o == null)
                return 1;
            else
                return 0;
        }
        private float[, ,] zeros(int[] size)
        {
            return new float[size[0], size[1], 1];
        }
        private float[, ,] zeros(int size)
        {
            return new float[size, size, 1];
        }
        private float[, ,] zeros(double size)
        {
            return new float[(int)size, (int)size, 1];
        }
        #endregion
        private Image<Gray, float> obd2(Image<Gray, float> x, float[, ,] y, float[, ,] f, int[] maxiter, double srf, bool regularize, float[, ,] PSFBounds)
        {

            double sf = f.GetLength(0);
            var sy = size(y);            //size of blurred image
            if (srf >= 1)
            {
                sy[0] = floor(srf * sy[0]);
                sy[1] = floor(srf * sy[1]);
                sf = floor(srf * sf);
            }
            else
            {
                if (srf < 1)
                    error("superresolution factor must be one or larger");
            }

            if (x != null)
            {
                var sx = size(x.Data);

                //estimate PSF with multiplicative updates
                obd_updatePSF(ref f, x.Data, y, maxiter[1], srf, regularize, PSFBounds);
                double fSum = f.SumArray();
                f.DivideInPlace((float)fSum);
                Image<Gray, float> t = new Image<Gray, float>(f);
                int w2 = t.Width;
                //f.NormalizeArray();
                x.Data.MultiplyInPlace((float)fSum);
            }
            else
            {
                //if ((isempty(f) == 1))
                {
                    f = zeros(sf);
                    int sf2 = (int)Math.Ceiling(sf / 2);
                    f[sf2, sf2, 0] = 1;   //a delta peak

                }

                var sx = sy[0] + sf - 1;

                //  var xData = pos(cnv2tp(f, y, srf));
                float[, ,] xData = pos(samp2(y, (int)sx));

                x = new Image<Gray, float>(xData);
                // f = origPSF.CopyThis();
                // f.NormalizeArray();
                return x;
            }

            float[, ,] data = x.Data;

            //improve true image x with multiplicative updates
            obd_update(ref data, f, y, maxiter[1], srf);

            var fY = y.MaxArray();
            var fX = x.Data.MaxArray();

            var w = fY - fX;

            return x;
        }


        //%%%%%%%%%%%%%%%%
        private void obd_updatePSF(ref float[, ,] f, float[, ,] x, float[, ,] y, int iters, double srf, bool regularize, float[, ,] psfBounds)
        {
            //depending on the value of sf the roles of f and x can be swapped
            var sf = size(f);
            var sy = size(y);    //for srf > 1, the low resolution y



            float[, ,] factor = zeros(f.GetLength(0));

            float tol = (float)(1e-10);

            if (regularize)
            {
                for (int i = 0; i < iters / 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        var ytmp = pos(cnv2(x, f, sy[0]));
                        // ytmp = ytmp .* m;                 //deal with clipping
                        var nom = pos(cnv2tp(x, y, srf));
                        var denom = pos(cnv2tp(x, ytmp, srf));
                        nom.AddToArray(tol);
                        denom.AddToArray(tol);
                        factor.DivideToArray(nom, denom);
                        //  factor = reshape(factor, sf);
                        //  imagesc(factor);drawnow;
                        f.MultiplyInPlace(factor);
                    }

                    for (int xx = 0; xx < psfBounds.GetLength(0); xx++)
                        for (int yy = 0; yy < psfBounds.GetLength(1); yy++)
                        {
                            if (f[xx, yy, 0] > psfBounds[xx, yy, 0])
                                f[xx, yy, 0] = psfBounds[xx, yy, 0];

                        }
                }

            }
            else
            {
                for (int i = 0; i < iters; i++)
                {
                    var ytmp = pos(cnv2(x, f, sy[0]));
                    // ytmp = ytmp .* m;                 //deal with clipping
                    var nom = pos(cnv2tp(x, y, srf));
                    var denom = pos(cnv2tp(x, ytmp, srf));
                    nom.AddToArray(tol);
                    denom.AddToArray(tol);
                    factor.DivideToArray(nom, denom);
                    //  factor = reshape(factor, sf);
                    //  imagesc(factor);drawnow;
                    f.MultiplyInPlace(factor);
                }
            }

        }

        //%%%%%%%%%%%%%%%%
        private void obd_update(ref float[, ,] x1, float[, ,] f, float[, ,] y, int iters, double srf)
        {
            //depending on the value of sf the roles of f and x can be swapped
            var sf = size(x1);
            var sy = size(y);    //for srf > 1, the low resolution y



            float[, ,] factor = zeros(x1.GetLength(0));

            float tol = (float)(1e-10);
            for (int i = 0; i < iters; i++)
            {
                var ytmp = pos(cnv2(f, x1, sy[0]));
                // ytmp = ytmp .* m;                 //deal with clipping
                var nom = pos(cnv2tp(f, y, srf));
                var denom = pos(cnv2tp(f, ytmp, srf));
                nom.AddToArray(tol);
                denom.AddToArray(tol);
                factor.DivideToArray(nom, denom);
                //  factor = reshape(factor, sf);
                //  imagesc(factor);drawnow;
                x1.MultiplyInPlace(factor);
            }

        }

        //%%%%%%%%%%%%%%%%%
        private float[, ,] pos(float[, ,] x)
        {
            x.EnforcePositive();
            return x;
        }

        //%%%%%%%%%%%%%%%%%
        private float[, ,] cnv2slice(float[, ,] A, int[] i, int[] j)
        {
            float[, ,] aN = new float[i[1] - i[0], j[1], j[0]];
            int x = 0, y = 0;
            for (int I = i[0]; I < i[1]; I++)
            {
                for (int J = j[0]; J < j[1]; J++)
                {
                    aN[x, y, 0] = A[I, J, 0];
                    y++;
                }
                x++;
            }
            return aN;
        }



        //%%%%%%%%%%%%%%%%%
        private /*y*/ float[, ,] cnv2(float[, ,] x, float[, ,] f, int sy)
        {
            var sx = size(x);
            var sf = size(f);
            float[, ,] y = null;
            if (sx[0] >= sf[0]) //x is larger or equal to f
            {
                //perform convolution in Fourier space
                // y = ifft2(fft2(x) .* fft2(f, sx(1), sx(2)));
                y = MathFFTHelps.FFT_cnv2(x, f);
                //Image<Gray, float> t = new Image<Gray, float>(y);
                //  int w = t.Width;
                //y = cnv2slice(y, sf(1):sx(1), sf(2):sx(2));
            }
            else
            {
                if (sx[0] <= sf[0])  //x is smaller or equal than f
                {
                    y = cnv2(f, x, sy);
                }
                else
                    error("[cnv2.m] x must be at least as large as f or vice versa.");
            }

            if (sy > size(y)[0])
                error("[cnv2.m] size missmatch");


            if ((sy < size(y)[0]))
            {
                y = samp2(y, sy);   //downsample
                //  Image<Gray, float> t = new Image<Gray, float>(y);
                //  int w = t.Width;
            }
            return y;
        }

        //%%%%%%%%%%%%%%%%%
        private /* f*/ float[, ,] cnv2tp(float[, ,] x, float[, ,] y, double srf)
        {
            var sx = size(x);
            float[, ,] y2 = y;
            if (srf > 1)
            {
                y2 = samp2(y, floor(srf * size(y)[0]));    //upsample
            }
            var sy = size(y)[0];

            int sf;
            float[, ,] f = null;
            //perform the linear convolution in Fourier space
            if (sx[0] >= sy)
            {
                sf = sx[0] - sy + 1;
                f = MathFFTHelps.conv2(x, y, sf); //ifft2(conj(fft2(x)).*fft2(cnv2pad(y, sf)));
                //  Image<Gray, float> t = new Image<Gray, float>(f);
                //  int w = t.Width;
                // f = cnv2slice(tmp, 1:sf(1), 1:sf(2));
            }
            else
            {
                if (sx[0] <= sy)
                {
                    sf = sy + sx[0] - 1;
                    f = MathFFTHelps.CrossCorrelationFFT(y, x, sf);
                    //f = ifft2(conj(fft2(x, sf(1), sf(2))).*fft2(cnv2pad(y, sx), sf(1), sf(2)));}
                }
                else  //x and y are incomparable
                    error("[cnv2.m] x must be at least as large as y or vice versa.");
            }
            //f = real(f);
            return f;
        }



        //        //%%%%%%%%%%%%%
        //        private /* B =*/float[, ,] cnv2pad(float[, ,] A, int sf){
        ////PAD with zeros from the top-left
        //i = sf(1);  j = sf(2);
        //[rA, cA] = size(A);
        //float[,,] B = zeros(rA+i-1, cA+j-1);
        //B(i:end, j:end) = A;
        //return    B;}

        //%%%%%%%%%%%%
        private float[, ,] samp2(float[, ,] x, int sy)
        {

            float[, ,] arrayOutf = new float[sy, sy, 1];
            int startI = (int)(Math.Floor((x.GetLength(0) - sy) / 2d));

            if (startI >= 0)
            {
                for (int i = 0; i < arrayOutf.GetLength(0); i++)
                {
                    for (int j = 0; j < arrayOutf.GetLength(1); j++)
                    {
                        arrayOutf[i, j, 0] = (float)(x[i + startI, j + startI, 0]);
                    }
                }

                return arrayOutf;
            }
            else
            {
                for (int i = 0; i < x.GetLength(0); i++)
                {
                    for (int j = 0; j < x.GetLength(1); j++)
                    {
                        arrayOutf[i - startI, j - startI, 0] = (float)(x[i, j, 0]);
                    }
                }

                return arrayOutf;

            }
            //sx = size(x);
            // downsample by factor srf
            //y = sampmat(sy(1), sx(1)) * x * sampmat(sy(2), sx(2))';
            // return null;
        }

        //%%%%%%%%%%%%%
        private double[,] sampmat(int m, int n)
        {
            return null;
            //D = kron(speye(m), ones(n, 1)') * kron(speye(n), ones(m, 1))/m;
        }
        //%%%%% } OF obd.m %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



        #endregion
    }
}

