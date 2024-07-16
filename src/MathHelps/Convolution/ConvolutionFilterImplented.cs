using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace MathHelpLib.Convolution
{
    public class ConvolutionFilterImplented
    {
        /// <summary>
        /// Does the hard work of convoluting the image.  
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="Kernal">Desired convolution kernal.  </param>
        /// <returns></returns>
        public static Bitmap ConvolutionFilter(Bitmap SourceImage, IConvolutionKernal Kernal)
        {
            int iWidth = SourceImage.Width;
            int iHeight = SourceImage.Height;

            double[,] bInData = MathHelpLib.ImageProcessing.MathImageHelps. ConvertToDoubleArray(SourceImage, false);
            double[,] bOutData = new double[iHeight, iWidth];

            int StepBack = (int)Math.Floor(Kernal.Rank[0] / 2d);
            int LineBack = (int)Math.Truncate(Kernal.Rank[1] / -2d);
            int LineForward = (int)Math.Floor(Kernal.Rank[1] / 2d);
            int cc = 0;
            unsafe
            {
                double Factor = (double)Int32.MaxValue / (double)(iHeight + iWidth);
                double*[] LineStarts = new double*[Kernal.Rank[1]];
                fixed (double* pbIn = bInData)
                {
                    fixed (double* pbOut = bOutData)
                    {
                        for (int y = Kernal.Rank[1]; y < iHeight - Kernal.Rank[1]; y++)
                        {
                            double* scanline = pbIn + (y) * iWidth;
                            double* scanlineOut = pbOut + y * iWidth;
                            scanline += StepBack;
                            scanlineOut += StepBack;
                            for (int x = StepBack; x < iWidth - StepBack; x++)
                            {
                                cc = 0;
                                for (int line = LineBack; line <= LineForward; line++)
                                {
                                    LineStarts[cc] = (scanline - StepBack) + iWidth * line;
                                    cc++;
                                }
                                *scanlineOut = Kernal.RunKernal(LineStarts);
                                scanline++;
                                scanlineOut++;
                            }
                        }
                    }
                }
            }
            return MathHelpLib.ImageProcessing.MathImageHelps.ConvertToBitmap(bOutData);
        }

        /// <summary>
        /// Does the hard work of convoluting the image.  
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="Kernal">Desired convolution kernal.  </param>
        /// <returns></returns>
        public static ImageHolder ConvolutionFilter(ImageHolder SourceImage, IConvolutionKernal Kernal)
        {


            double[,] bInData = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false);
            int iWidth = bInData.GetLength(1);
            int iHeight = bInData.GetLength(0);

            double[,] bOutData = new double[iHeight, iWidth];

            int StepBack = (int)Math.Floor(Kernal.Rank[0] / 2d);
            int LineBack = (int)Math.Truncate(Kernal.Rank[1] / -2d);
            int LineForward = (int)Math.Floor(Kernal.Rank[1] / 2d);
            int cc = 0;
            unsafe
            {
                double Factor = (double)Int32.MaxValue / (double)(iHeight + iWidth);
                double*[] LineStarts = new double*[Kernal.Rank[1]];
                fixed (double* pbIn = bInData)
                {
                    fixed (double* pbOut = bOutData)
                    {
                        //do the main section
                        for (int y = -1 * LineBack; y < iHeight - LineForward; y++)
                        {
                            double* scanline = pbIn + (y) * iWidth;
                            double* scanlineOut = pbOut + y * iWidth;
                            scanline += StepBack;
                            scanlineOut += StepBack;
                            for (int x = StepBack; x < iWidth - StepBack; x++)
                            {
                                cc = 0;
                                for (int line = LineBack; line <= LineForward; line++)
                                {
                                    LineStarts[cc] = (scanline - StepBack) + iWidth * line;
                                    cc++;
                                }
                                *scanlineOut = Kernal.RunKernal(LineStarts);
                                scanline++;
                                scanlineOut++;
                            }
                        }

                        //do the top 
                        for (int y = 0; y < -1 * LineBack; y++)
                        {
                            double* scanline = pbIn + (y) * iWidth;
                            double* scanlineOut = pbOut + y * iWidth;
                            scanline += StepBack;
                            scanlineOut += StepBack;
                            for (int x = StepBack; x < iWidth - StepBack; x++)
                            {
                                cc = 0;
                                for (int line = LineBack; line <= LineForward; line++)
                                {
                                    if ((line + y) < 0)
                                        LineStarts[cc] = (scanline - StepBack) + iWidth * 0;
                                    else
                                        LineStarts[cc] = (scanline - StepBack) + iWidth * line;
                                    cc++;
                                }
                                *scanlineOut = Kernal.RunKernal(LineStarts);
                                scanline++;
                                scanlineOut++;
                            }
                        }

                        //do the bottom
                        for (int y = iHeight - LineForward; y < iHeight; y++)
                        {
                            double* scanline = pbIn + (y) * iWidth;
                            double* scanlineOut = pbOut + y * iWidth;
                            scanline += StepBack;
                            scanlineOut += StepBack;
                            for (int x = StepBack; x < iWidth - StepBack; x++)
                            {
                                cc = 0;
                                for (int line = LineBack; line <= LineForward; line++)
                                {
                                    if ((line + y) >= iHeight)
                                        LineStarts[cc] = (scanline - StepBack) + iWidth * (iHeight - y - 1);
                                    else
                                        LineStarts[cc] = (scanline - StepBack) + iWidth * line;
                                    cc++;
                                }
                                *scanlineOut = Kernal.RunKernal(LineStarts);
                                scanline++;
                                scanlineOut++;
                            }
                        }


                        double[,] DummyArray = new double[Kernal.Rank[0], Kernal.Rank[1]];
                        int ccX, ccY;
                        int xx, yy;
                        fixed (double* pDummy = DummyArray)
                        {
                            for (int i = 0; i < Kernal.Rank[0]; i++)
                                LineStarts[i] = pDummy + i * Kernal.Rank[1];


                            //do the Left and Right
                            for (int y = 0; y < iHeight; y++)
                            {
                                double* scanline = pbIn + (y) * iWidth;
                                double* scanlineOut = pbOut + y * iWidth;
                                //left side
                                for (int x = 0; x < StepBack; x++)
                                {
                                    ccX = 0;
                                    for (int i = x - StepBack; i <= x + StepBack; i++)
                                    {
                                        ccY = 0;
                                        for (int j = y + LineBack; j <= y + LineForward; j++)
                                        {
                                            if (i < 0)
                                                xx = 0;
                                            else
                                                xx = i;
                                            if (j < 0)
                                                yy = 0;
                                            else
                                                yy = j;
                                            if (yy >= iHeight) yy = iHeight - 1;
                                            DummyArray[ccX, ccY] = bInData[yy, xx];
                                            ccY++;
                                        }
                                        ccX++;
                                    }
                                    *scanlineOut = Kernal.RunKernal(LineStarts);
                                    scanline++;
                                    scanlineOut++;
                                }

                                scanline = pbIn + (y) * iWidth + iWidth - StepBack;
                                scanlineOut = pbOut + y * iWidth + iWidth - StepBack;
                                //right side
                                for (int x = iWidth - StepBack; x < iWidth; x++)
                                {
                                    ccX = 0;
                                    for (int i = x - StepBack; i <= x + StepBack; i++)
                                    {
                                        ccY = 0;
                                        for (int j = y + LineBack; j <= y + LineForward; j++)
                                        {
                                            if (i < 0)
                                                xx = 0;
                                            else
                                                xx = i;
                                            if (xx >= iWidth) xx = iWidth - 1;

                                            if (j < 0)
                                                yy = 0;
                                            else
                                                yy = j;
                                            if (yy >= iHeight) yy = iHeight - 1;
                                            DummyArray[ccX, ccY] = bInData[yy, xx];

                                            ccY++;
                                        }
                                        ccX++;
                                    }
                                    *scanlineOut = Kernal.RunKernal(LineStarts);
                                    scanline++;
                                    scanlineOut++;
                                }

                            }

                        }
                    }
                }
            }
            return MathHelpLib.ImageProcessing.MathImageHelps.ConvertToGrayScaleImage(bOutData, 1);
        }

        /// <summary>
        /// Performs realspace convolution of image and given kernal, returns image of the same size
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="Kernal"></param>
        /// <returns></returns>
        public static double[,] ConvolutionFilter(double[,] SourceImage, Convolution.IConvolutionKernal Kernal)
        {
            int iWidth = SourceImage.GetLength(0);
            int iHeight = SourceImage.GetLength(1);

            double[,] OutArray = new double[iWidth, iHeight];

            int StepBack = (int)Math.Floor(Kernal.Rank[0] / 2d);
            int LineBack = (int)Math.Truncate(Kernal.Rank[1] / -2d);
            int LineForward = (int)Math.Floor(Kernal.Rank[1] / 2d);
            int cc = 0;
            unsafe
            {
                double*[] LineStarts = new double*[Kernal.Rank[1]];
                fixed (double* pInArray = SourceImage)
                {
                    fixed (double* pOutArray = OutArray)
                    {
                        for (int y = Kernal.Rank[1]; y < iHeight - Kernal.Rank[1]; y++)
                        {
                            double* scanline = pInArray + y * iHeight;
                            double* scanlineOut = pOutArray + y * iHeight;
                            scanline += StepBack;
                            scanlineOut += StepBack;
                            for (int x = StepBack; x < iWidth - StepBack; x++)
                            {
                                cc = 0;
                                //get the starts of the kernal array.  this only covers the lines that are affected by the convolution for this filter
                                for (int line = LineBack; line <= LineForward; line++)
                                {
                                    LineStarts[cc] = (scanline - StepBack) + iHeight * line;
                                    cc++;
                                }
                                *scanlineOut = Kernal.RunKernal(LineStarts);
                                //OutArray[x,y] = Kernal.RunKernal(LineStarts);
                                scanline++;
                                scanlineOut++;
                            }
                        }
                    }
                }
            }
            return OutArray;
        }
    }
}
