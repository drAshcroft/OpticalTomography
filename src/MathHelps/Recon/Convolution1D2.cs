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



namespace MathHelpLib.Recon
{
    public class Convolution1D 
    {
        ~Convolution1D()
        {

        }


        /// <summary>
        /// Does the convolution, first tries with GPU then with CPU
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="Kernal"></param>
        /// <param name="ConvolutionAxis">int(0 means X, 1 means Y)</param>
        /// <param name="CutDown" > Amount to clip off the egdes of the image </param>
        /// <returns></returns>
        public double[,] DoConvolution(Bitmap SourceImage, double[] Kernal, int ConvolutionAxis, double CutDown)
        {
            double[,] DataIn = null;
            int OriginalWidth = 0;
            int OriginalHeight = 0;

           
            //do the convolution on the cpu
            double[,] DataInDouble;
            if (DataIn == null)
            {
                if (ConvolutionAxis == 0)
                    DataInDouble = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false);
                else
                    DataInDouble = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, true);
            }
            else
            {
                DataInDouble = new double[DataIn.GetLength(0), DataIn.GetLength(1)];

                Buffer.BlockCopy(DataIn, 0, DataInDouble, 0, Buffer.ByteLength(DataIn));
                // DataIn.CopyTo(DataInDouble, 0);
            }
            //do the convolution
            return Convolve(DataInDouble, Kernal);
        }


        private double[,] DoNormalConvolution(double[] Kernal, int ConvolutionAxis, ImageHolder SourceImage)
        {
            double[,] DataInDouble;
                if (ConvolutionAxis == 0)
                    DataInDouble = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray((SourceImage), false);
                else
                    DataInDouble = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, true);
            //do the convolution
            return Convolve(DataInDouble, Kernal);
        }

       

        

      
        //  }
        #region NormalConvolution
        /// <summary>
        /// queues the lines of the input image
        /// </summary>
        /// <param name="DataIn"></param>
        /// <param name="Kernal"></param>
        /// <returns></returns>
        public static unsafe double[,] Convolve(double[,] DataIn, double[] Kernal)
        {
            double[,] ArrayOut = new double[DataIn.GetLength(0), DataIn.GetLength(1)];

           // double kernalSum = Kernal.Sum();

            fixed (double* pArrayIn = DataIn)
            {
                fixed (double* pKernal = Kernal)
                {
                    fixed (double* pOut = ArrayOut)
                    {
                        // ConvolveX(pArrayIn, pOut, DataIn.GetLength(0), DataIn.GetLength(1), pKernal, Kernal.Length);
                        int Width = DataIn.GetLength(0);
                        int Height = DataIn.GetLength(1);
                        for (int i = 0; i < Height; i++)
                        {
                            ConvoluteChop(pArrayIn + i * Width, Width, pKernal, Kernal.Length, pOut + i * Width);
                        }

                        //long Area = Width * Height;
                      /*  double* ppOut = pOut;
                        for (long i = 0; i < Area; i++)
                        {
                            (*ppOut) /= kernalSum;
                            ppOut++;
                        }*/
                    }
                }
            }
            return ArrayOut;
        }

        /// <summary>
        /// queues the lines of the input image
        /// </summary>
        /// <param name="DataIn"></param>
        /// <param name="Kernal"></param>
        /// <returns></returns>
        public static unsafe float [,] Convolve(float [,] DataIn, double[] Kernal)
        {
            float[,] ArrayOut = new float[DataIn.GetLength(0), DataIn.GetLength(1)];

            // double kernalSum = Kernal.Sum();

            fixed (float* pArrayIn = DataIn)
            {
                fixed (double* pKernal = Kernal)
                {
                    fixed (float* pOut = ArrayOut)
                    {
                        // ConvolveX(pArrayIn, pOut, DataIn.GetLength(0), DataIn.GetLength(1), pKernal, Kernal.Length);
                        int Width = DataIn.GetLength(0);
                        int Height = DataIn.GetLength(1);
                        for (int i = 0; i < Height; i++)
                        {
                            ConvoluteChop(pArrayIn + i * Width, Width, pKernal, Kernal.Length, pOut + i * Width);
                        }

                        //long Area = Width * Height;
                        /*  double* ppOut = pOut;
                          for (long i = 0; i < Area; i++)
                          {
                              (*ppOut) /= kernalSum;
                              ppOut++;
                          }*/
                    }
                }
            }
            return ArrayOut;
        }


        /// <summary>
        /// queues the lines of the input image
        /// </summary>
        /// <param name="DataIn"></param>
        /// <param name="Kernal"></param>
        /// <returns></returns>
        public static unsafe float[,] Convolve(float[,,] DataIn, double[] Kernal)
        {
            float[,] ArrayOut = new float[DataIn.GetLength(0), DataIn.GetLength(1)];

            // double kernalSum = Kernal.Sum();

            fixed (float* pArrayIn = DataIn)
            {
                fixed (double* pKernal = Kernal)
                {
                    fixed (float* pOut = ArrayOut)
                    {
                        // ConvolveX(pArrayIn, pOut, DataIn.GetLength(0), DataIn.GetLength(1), pKernal, Kernal.Length);
                        int Width = DataIn.GetLength(0);
                        int Height = DataIn.GetLength(1);
                        for (int i = 0; i < Height; i++)
                        {
                            ConvoluteChop(pArrayIn + i * Width, Width, pKernal, Kernal.Length, pOut + i * Width);
                        }

                        //long Area = Width * Height;
                        /*  double* ppOut = pOut;
                          for (long i = 0; i < Area; i++)
                          {
                              (*ppOut) /= kernalSum;
                              ppOut++;
                          }*/
                    }
                }
            }
            return ArrayOut;
        }

        /// <summary>
        /// Does the convolution along the fast memory access direction
        /// </summary>
        /// <param name="Array1"></param>
        /// <param name="Length1"></param>
        /// <param name="pImpulse"></param>
        /// <param name="Length2"></param>
        /// <param name="pArrayOut"></param>
        public static unsafe void ConvoluteChop(float* Array1, int Length1, double* pImpulse, int Length2, float* pArrayOut)
        {

            int LengthWhole = Length1 + Length2;

            int StartI = (int)Math.Truncate((double)LengthWhole / 2d - (double)Length1 / 2d);
            int EndI = (int)Math.Truncate((double)LengthWhole / 2d + (double)Length1 / 2d);
            if (EndI - StartI > Length1)
                EndI--;
            int sI, eI;


            double p1;
            double* p2;
            float* pOut;

            unchecked
            {
                for (int i = 0; i < Length1; i++)
                {
                    p1 = Array1[i];
                    sI = StartI - i;
                    eI = EndI - i;
                    if (eI > Length2) eI = Length2;
                    if (sI < 0) sI = 0;
                    if (sI < eI)
                    {
                        p2 = pImpulse + sI;
                        pOut = pArrayOut + i + sI - StartI;
                        for (int j = sI; j < eI; j++)
                        {
                            *pOut += (float)(p1 * (*p2));
                            pOut++;
                            p2++;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// queues the lines of the input image
        /// </summary>
        /// <param name="DataIn"></param>
        /// <param name="Kernal"></param>
        /// <returns></returns>
        public static unsafe double[,] Convolve(double[,,] DataIn, double[] Kernal)
        {
            double[,] ArrayOut = new double[DataIn.GetLength(0), DataIn.GetLength(1)];

            // double kernalSum = Kernal.Sum();

            fixed (double* pArrayIn = DataIn)
            {
                fixed (double* pKernal = Kernal)
                {
                    fixed (double* pOut = ArrayOut)
                    {
                        // ConvolveX(pArrayIn, pOut, DataIn.GetLength(0), DataIn.GetLength(1), pKernal, Kernal.Length);
                        int Width = DataIn.GetLength(0);
                        int Height = DataIn.GetLength(1);
                        for (int i = 0; i < Height; i++)
                        {
                            ConvoluteChop(pArrayIn + i * Width, Width, pKernal, Kernal.Length, pOut + i * Width);
                        }

                        //long Area = Width * Height;
                        /*  double* ppOut = pOut;
                          for (long i = 0; i < Area; i++)
                          {
                              (*ppOut) /= kernalSum;
                              ppOut++;
                          }*/
                    }
                }
            }
            return ArrayOut;
        }


        /// <summary>
        /// Does the convolution along the fast memory access direction
        /// </summary>
        /// <param name="Array1"></param>
        /// <param name="Length1"></param>
        /// <param name="pImpulse"></param>
        /// <param name="Length2"></param>
        /// <param name="pArrayOut"></param>
        public static unsafe void ConvoluteChop(double* Array1, int Length1, double* pImpulse, int Length2, double* pArrayOut)
        {

            int LengthWhole = Length1 + Length2;

            int StartI = (int)Math.Truncate((double)LengthWhole / 2d - (double)Length1 / 2d);
            int EndI = (int)Math.Truncate((double)LengthWhole / 2d + (double)Length1 / 2d);
            if (EndI - StartI > Length1)
                EndI--;
            int sI, eI;


            double p1;
            double* p2;
            double* pOut;

            unchecked
            {
                for (int i = 0; i < Length1; i++)
                {
                    p1 = Array1[i];
                    sI = StartI - i;
                    eI = EndI - i;
                    if (eI > Length2) eI = Length2;
                    if (sI < 0) sI = 0;
                    if (sI < eI)
                    {
                        p2 = pImpulse + sI;
                        pOut = pArrayOut + i + sI - StartI;
                        for (int j = sI; j < eI; j++)
                        {
                            *pOut += p1 * (*p2);
                            pOut++;
                            p2++;
                        }
                    }
                }
            }
        }


        ///////////////////////////////////////////////////////////////////////////////
        // double precision float version
        ///////////////////////////////////////////////////////////////////////////////
        private unsafe void ConvolveX(double* ArrayIn, double* ArrayOut, int dataSizeX, int dataSizeY, double* kernelX, int kSizeX)
        {
            int i, j, k, m;

            // intermediate data buffer
            double* inPtr;                         // working pointers
            double* tmpPtr;                       // working pointers
            int kCenter, kOffset, endIndex;                 // kernel indice



            // covolve horizontal direction ///////////////////////

            // find center position of kernel (half of kernel size)
            kCenter = kSizeX >> 1;                          // center index of kernel array
            endIndex = dataSizeX - kCenter;                 // index for full kernel convolution

            // init working pointers
            inPtr = ArrayIn;
            tmpPtr = ArrayOut;                                   // store intermediate results from 1D horizontal convolution

            unchecked
            {
                // start horizontal convolution (x-direction)
                for (i = 0; i < dataSizeY; ++i)                    // number of rows
                {

                    kOffset = 0;                                // starting index of partial kernel varies for each sample

                    // COLUMN FROM index=0 TO index=kCenter-1
                    for (j = 0; j < kCenter; ++j)
                    {
                        *tmpPtr = 0;                            // init to 0 before accumulation

                        for (k = kCenter + kOffset, m = 0; k >= 0; --k, ++m) // convolve with partial of kernel
                        {
                            *tmpPtr += *(inPtr + m) * kernelX[k];
                        }
                        ++tmpPtr;                               // next output
                        ++kOffset;                              // increase starting index of kernel
                    }

                    // COLUMN FROM index=kCenter TO index=(dataSizeX-kCenter-1)
                    for (j = kCenter; j < endIndex; ++j)
                    {
                        *tmpPtr = 0;                            // init to 0 before accumulate

                        for (k = kSizeX - 1, m = 0; k >= 0; --k, ++m)  // full kernel
                        {
                            *tmpPtr += *(inPtr + m) * kernelX[k];
                        }
                        ++inPtr;                                // next input
                        ++tmpPtr;                               // next output
                    }

                    kOffset = 1;                                // ending index of partial kernel varies for each sample

                    // COLUMN FROM index=(dataSizeX-kCenter) TO index=(dataSizeX-1)
                    for (j = endIndex; j < dataSizeX; ++j)
                    {
                        *tmpPtr = 0;                            // init to 0 before accumulation

                        for (k = kSizeX - 1, m = 0; k >= kOffset; --k, ++m)   // convolve with partial of kernel
                        {
                            *tmpPtr += *(inPtr + m) * kernelX[k];
                        }
                        ++inPtr;                                // next input
                        ++tmpPtr;                               // next output
                        ++kOffset;                              // increase ending index of partial kernel
                    }

                    inPtr += kCenter;                           // next row
                }
            }

        }
        #endregion

     

    }
}
