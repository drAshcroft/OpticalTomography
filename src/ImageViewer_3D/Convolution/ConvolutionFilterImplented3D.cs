using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageViewer3D.Convolution
{
    public class ConvolutionFilterImplented3D
    {
        public static double[,,] ConvolutionFilter(double[,,] SourceImage, IConvolutionKernal3D Kernal)
        {
            double[, ,] bInData = SourceImage;
            int iWidth = bInData.GetLength(2);
            int iHeight = bInData.GetLength(1);
            int iDepth = bInData.GetLength(0);

            double[, ,] bOutData = new double[iHeight, iWidth, iDepth];

            int StepBack = (int)Math.Floor((Kernal.Rank[0] - 1) / 2d);
            int LineBack = (int)Math.Truncate(Kernal.Rank[1] / -2d);
            int LineForward = (int)Math.Floor(Kernal.Rank[1] / 2d);
            int cc = 0;
            unsafe
            {
                double*[] LineStarts = new double*[Kernal.Rank[1] * Kernal.Rank[2]];

                fixed (double* pbIn = bInData)
                {
                    fixed (double* pbOut = bOutData)
                    {
                        for (int z = StepBack; z < iDepth - StepBack; z++)
                        {
                            //do the main section
                            for (int y = StepBack; y < iHeight - StepBack; y++)
                            {
                                //find the first x line 
                                double* scanline = pbIn + (y) * iWidth + z * iWidth * iHeight;
                                double* scanlineOut = pbOut + y * iWidth + z * iWidth * iHeight;
                                //advance over the by the amount of the step
                                scanline += StepBack;
                                scanlineOut += StepBack;
                                //now progress down the line
                                for (int x = StepBack; x < iWidth - StepBack; x++)
                                {
                                    cc = 0;

                                    for (int lineZ = LineBack; lineZ <= LineForward; lineZ++)
                                    {
                                        for (int lineY = LineBack; lineY <= LineForward; lineY++)
                                        {
                                            LineStarts[cc] = (scanline - StepBack) + iWidth * lineY + iWidth * iHeight * lineZ;
                                            cc++;
                                        }
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


            return bOutData;
        }

        public static DataHolder  ConvolutionFilter(DataHolder  SourceImage, IConvolutionKernal3D Kernal)
        {
            double[, ,] bInData = SourceImage.Data;
            int iWidth = bInData.GetLength(2);
            int iHeight = bInData.GetLength(1);
            int iDepth = bInData.GetLength(0);

            double[,,] bOutData = new double[iHeight, iWidth,iDepth ];

            int StepBack = (int)Math.Floor( (Kernal.Rank[0]-1) / 2d);
            int LineBack = (int)Math.Truncate(Kernal.Rank[1] / -2d);
            int LineForward = (int)Math.Floor(Kernal.Rank[1] / 2d);
            int cc = 0;
            unsafe
            {
                double*[] LineStarts = new double*[Kernal.Rank[1]*Kernal.Rank[2]];

                fixed (double* pbIn = bInData)
                {
                    fixed (double* pbOut = bOutData)
                    {
                        for (int z = StepBack; z < iDepth - StepBack; z++)
                        {
                            //do the main section
                            for (int y = StepBack ; y < iHeight - StepBack ; y++)
                            {
                                //find the first x line 
                                double* scanline = pbIn + (y) * iWidth +z*iWidth*iHeight ;
                                double* scanlineOut = pbOut + y * iWidth + z * iWidth * iHeight;
                                //advance over the by the amount of the step
                                scanline += StepBack;
                                scanlineOut += StepBack;
                                //now progress down the line
                                for (int x = StepBack; x < iWidth - StepBack; x++)
                                {
                                    cc = 0;

                                    for (int lineZ = LineBack; lineZ <= LineForward; lineZ++)
                                    {
                                        for (int lineY = LineBack; lineY <= LineForward; lineY++)
                                        {
                                            LineStarts[cc] = (scanline - StepBack) + iWidth * lineY+iWidth*iHeight*lineZ ;
                                            cc++;
                                        }
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

            SourceImage.Data = bOutData;
            return  SourceImage ;
        }

    }
}
