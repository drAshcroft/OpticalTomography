using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomographic_Imaging_2
{
    public class ProjectionToken
    {
        public int ProcessorSlice;
        public int ProcessorCount;
        public double[] impulse;
        public aProjectionSlice psf;
        public ProjectionObject po;
        public int[] FinishedConvolutionFlags;
        public int[] FinishedProcessing;
    }
}
