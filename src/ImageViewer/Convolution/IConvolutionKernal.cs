using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageViewer.Convolution
{
    
    public interface IConvolutionKernal
    {
        /// <summary>
        /// Size of the convolution Kernal,  This can be any value supported by this filter used as
        /// 3x3 {3,3}, must be less than value specified in max Rank
        /// </summary>
        int[] Rank { get; }

        /// <summary>
        /// The largest rank that the kernal can use
        /// </summary>
        int[] MaxRank { get; }


        /// <summary>
        /// Convolution for intensities.  
        /// </summary>
        /// <param name="LineStarts">The beginning (pointer) of the convolution rows.  Rank is used to get this information</param>
        /// <returns></returns>
        unsafe double RunKernal(double *[] LineStarts);

        /// <summary>
        /// Convolution for RGB values.
        /// </summary>
        /// <param name="LineStarts">The beginning(pointer) of the convolution rows.  Rank is used to get this information</param>
        /// <returns></returns>
        unsafe Int32 RunKernal(Int32*[] LineStarts);
        

        /// <summary>
        /// Convolution for intensities
        /// </summary>
        /// <param name="NeighBors">Neighbors of the current pixel (values) </param>
        /// <returns></returns>
        double RunKernal(double[,] NeighBors);
        
    }
}
