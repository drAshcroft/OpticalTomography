using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomographic_Imaging_2
{
    public enum ConvolutionMethod
    {
        Convolution1D, ConvolutionRealSpaceSeperable, ConvolutionRealSpaceFilterFFT,
        ConvolutionFrequencySpaceFilterFFT, ConvolutionRealSpace2D, ConvolutionRealSpaceGPU,NoConvolution
    }
}
