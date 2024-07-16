using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathHelpLib.ImageProcessing
{
    
    public interface IConvolutionKernal
    {
        /// <summary>
        /// Size of the convolution Kernal,  This can be any value supported by this filter used as
        /// 3x3 {3,3}, must be less than value specified in max Rank.  The number always has to be odd
        /// </summary>
        int[] Rank { get; }

        /// <summary>
        /// Maximum size of the kernal.  This is usually just from computational limitations
        /// </summary>
        int[] MaxRank { get; }

        /// <summary>
        /// runs the kernal on the data.  each of the linestarts is a pointer to the begining of row the image data.
        /// advancing the pointer on each of the linestarts is the equivilent of moving through the columns of the data
        /// </summary>
        /// <param name="LineStarts"></param>
        /// <returns></returns>
        unsafe double RunKernal(double *[] LineStarts);

        /// <summary>
        /// runs the kernal on the data.  each of the linestarts is a pointer to the begining of row the image data.
        /// advancing the pointer on each of the linestarts is the equivilent of moving through the columns of the data
        /// In this case, the int32 holds the rgb of the data
        /// </summary>
        /// <param name="LineStarts"></param>
        /// <returns></returns>
        unsafe Int32 RunKernal(Int32*[] LineStarts);
        
        /// <summary>
        /// Holds and array that matches the data under the kernal for this pixel
        /// </summary>
        /// <param name="NeighBors"></param>
        /// <returns></returns>
        double RunKernal(double[,] NeighBors);
        
    }
}
