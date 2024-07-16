using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathHelpLib;

namespace Tomographic_Imaging_2
{
    public class ProjectionSliceMemory : aProjectionSlice
    {
        PhysicalArray mProjection;

        public override void Dispose()
        {
            mProjection.Dispose();
            mBackProjection.Dispose();
        }

        public override PhysicalArrayRank ProjectionRank
        {
            get { return mProjection.ArrayRank; }
        }

        public override double PhysicalStartX
        {
            get { return mProjection.PhysicalStart(Axis.XAxis); }
        }

        public override double PhysicalEndX
        {
            get { return mProjection.PhysicalEnd(Axis.XAxis); }
        }

        public override double PhysicalStartY
        {
            get { return mProjection.PhysicalStart(Axis.YAxis); }
        }

        public override double PhysicalEndY
        {
            get { return mProjection.PhysicalEnd(Axis.YAxis); }
        }

        public override PhysicalArray Projection
        {
            get { return mProjection; }
            set
            {
                mProjection = value;
                mBackProjection = null;
            }
        }

       

        /// <summary>
        /// this would be an expensive operation to call repeatedly.  Make offsetX local copy to do the math
        /// you must call DoBackProjection_AllSlices for this to have offsetX value
        /// </summary>
        public override PhysicalArray BackProjection
        {
            get
            {

                return mBackProjection;
            }
            set
            {
                mBackProjectionPerformed = true;
                mBackProjection = value;

            }
        }

        protected override PhysicalArray DoBackProjection(PhysicalArray ProjectionT, double[] impulse, ConvolutionMethod convolutionMethod)
        {
            double tau;
            if (ProjectionT.ArrayRank == PhysicalArrayRank.Array1D)
            {
                tau = (PhysicalEndX - PhysicalStartX) / ((double)ProjectionT.Length);
                mBackProjection = ProjectionT.ConvoluteChop1D(Axis.YAxis, impulse);
            }
            else
            {
                tau = (PhysicalEndX - PhysicalStartX) / ((double)ProjectionT.Length);
                tau *= (PhysicalEndY - PhysicalStartY) / ((double)ProjectionT.Length);
                switch (convolutionMethod)
                {
                    case ConvolutionMethod.Convolution1D:
                        mBackProjection = ProjectionT.ConvoluteChop1D(Axis.XAxis, impulse);
                        break;
                    case ConvolutionMethod.ConvolutionFrequencySpaceFilterFFT:
                        //tBackProjection = ProjectionT.ConvoluteFFT2D(Axis.XAxis,Axis.YAxis,
                        throw new Exception("Not yet implemented");
                    // break;
                    case ConvolutionMethod.ConvolutionRealSpaceSeperable:
                        mBackProjection = ProjectionT.ConvoluteChopSeperable(Axis.XAxis, Axis.YAxis, impulse);
                        break;
                    case ConvolutionMethod.NoConvolution:
                        mBackProjection = ProjectionT;
                        tau = 1;
                        break;

                    default:
                        throw new Exception("Not appropriate for 1D impulse");
                }
            }

            mBackProjection *= tau;
            mBackProjectionPerformed = true;
            return mBackProjection;
        }
        static float[,] fImpulse;
        static object LockStaticObject = new object();
        protected override PhysicalArray DoBackProjection(PhysicalArray ProjectionT, double[,] impulse, ConvolutionMethod convolutionMethod)
        {
            double tau;
            if (ProjectionT.ArrayRank == PhysicalArrayRank.Array1D)
            {
                throw new Exception("Only 2D projections can be convoluted with a 2D impulse response");
            }
            else
            {
                tau = (PhysicalEndX - PhysicalStartX) / ((double)ProjectionT.Length);
                tau *= (PhysicalEndY - PhysicalStartY) / ((double)ProjectionT.Length);
                switch (convolutionMethod)
                {
                    case ConvolutionMethod.Convolution1D:
                        throw new Exception("Not appropriate for 2D impulse");
                    //break;
                    case ConvolutionMethod.ConvolutionRealSpaceFilterFFT:
                        mBackProjection = ProjectionT.ConvoluteFFT2D(Axis.XAxis, Axis.YAxis, impulse, true);
                        break;
                    case ConvolutionMethod.ConvolutionFrequencySpaceFilterFFT:
                        mBackProjection = ProjectionT.ConvoluteFFT2D(Axis.XAxis, Axis.YAxis, impulse, false);
                        break;
                    case ConvolutionMethod.ConvolutionRealSpaceSeperable:
                        throw new Exception("Not appropriate for 2D impulse");
                    // break;
                    case ConvolutionMethod.ConvolutionRealSpaceGPU:
                        lock (LockStaticObject)
                        {
                            if (fImpulse == null)
                            {
                                fImpulse = new float[impulse.GetLength(0), impulse.GetLength(1)];
                                for (int i = 0; i < impulse.GetLength(0); i++)
                                    for (int j = 0; j < impulse.GetLength(1); j++)
                                        fImpulse[i, j] = (float)impulse[i, j];
                            }
                        }
                        throw new Exception("not implimented");
                       // mBackProjection = ProjectionT.ConvolutionGPU(Axis.XAxis, Axis.YAxis, fImpulse);
                        break;
                    case ConvolutionMethod.ConvolutionRealSpace2D:
                        throw new Exception("Not Yet implimented");
                    // break;
                    case ConvolutionMethod.NoConvolution:
                        mBackProjection = ProjectionT;
                        tau = 1;
                        break;
                    default:
                        throw new Exception("Not appropriate for 1D impulse");
                }
            }

            mBackProjection *= tau;
            mBackProjectionPerformed = true;
            return mBackProjection;
        }

    }
}
