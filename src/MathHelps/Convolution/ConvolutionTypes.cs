using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathHelpLib
{
    public enum ConvolutionMethod
    {
        Convolution1D, ConvolutionRealSpaceSeperable, ConvolutionRealSpaceFilterFFT,
        ConvolutionFrequencySpaceFilterFFT, ConvolutionRealSpace2D, ConvolutionRealSpaceGPU,NoConvolution
    }
}
