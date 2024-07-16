using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageViewer3D.Convolution
{
    
    public interface IConvolutionKernal3D
    {
        /// <summary>
        /// Size of the convolution Kernal,  This can be any value supported by this filter used as
        /// 3x3 {3,3}, must be less than value specified in max Rank
        /// </summary>
        int[] Rank { get; }
        int[] MaxRank { get; }
        unsafe float RunKernal(float*[] LineStarts);
        float RunKernal(float[,] NeighBors);
        
    }
}
