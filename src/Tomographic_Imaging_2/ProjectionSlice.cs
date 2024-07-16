using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathHelpLib;

namespace Tomographic_Imaging_2
{
    public abstract class aProjectionSlice
    {
        public double Angle;

        /// <summary>
        /// Provides a chech for the more expensive slices like file slice,  a check can be performed without loading the backprojection data
        /// </summary>
        protected bool mBackProjectionPerformed;
        
        public aProjectionSlice()
        { }
        public aProjectionSlice(double Angle, PhysicalArray SliceData)
        {
            Projection = SliceData;
            this.Angle = Angle;
        }

        public bool BackProjectionPerformed
        {
            get { return mBackProjectionPerformed; }
        }

        public abstract   double PhysicalStart
        {
            get;
        }

        public abstract double PhysicalEnd
        {
            get;
        }

        public abstract  double PhysicalStep
        {
            get;
        }

        public abstract PhysicalArray  Projection
        {
            get;
            set;
        }

        public void SetProjection(double[] Projection, double PhysicalStart, double PhysicalEnd)
        {
            this.Projection = new PhysicalArray(Projection, PhysicalStart, PhysicalEnd);
        }

        public void SetProjection(double[,] Projection, double PhysicalStartX, double PhysicalEndX, double PhysicalStartY, double PhysicalEndY)
        {
            this.Projection = new PhysicalArray(Projection, PhysicalStartX, PhysicalEndX, PhysicalStartY, PhysicalEndY);
        }

        public void SetProjection(double[,] Projection, double[] PhysicalStart, double[] PhysicalEnd)
        {
            this.Projection = new PhysicalArray(Projection, PhysicalStart, PhysicalEnd);
        }
        /// <summary>
        /// this would be an expensive operation to call repeatedly.  Make offsetX local copy to do the math
        /// you must call DoBackProjection for this to have offsetX value
        /// </summary>
        public abstract PhysicalArray BackProjection
        {
            get;
            set;
        }

        /// <summary>
        /// Creates the backprojecting slice,  if you wish to add zero padding, you can specify how many times more 
        /// the mDataDouble should be extended
        /// </summary>
        /// <param name="ZeroPadding">n is the number of points passed to the convolution. Useful for ffts</param>
        /// <param name="impulse"></param>
        /// <returns></returns>
        public PhysicalArray DoBackProjection(double[] impulse,int  ZeroPadding )
        {
            PhysicalArray ProjectionT = null;
            ProjectionT = Projection.ZeroPad_DataCentered(Axis.XAxis, ZeroPadding);
            return DoBackProjection(ProjectionT, impulse);
        }

        /// <summary>
        /// Creates the backprojecting slice,  if you wish to add zero padding, you can specify how many times more 
        /// the mDataDouble should be extended
        /// </summary>
        /// <param name="ZeroPadding">1 means that the mDataDouble stays the same length, 2 means twice as long...</param>
        /// <param name="impulse"></param>
        /// <returns></returns>
        public PhysicalArray DoBackProjection(double ZeroPadding, double[] impulse)
        {
            PhysicalArray ProjectionT = null;
            if (ZeroPadding == 1 || ZeroPadding == 0)
                ProjectionT = Projection;
            else
            {
                PhysicalArray BaseArray = Projection;
                ProjectionT = BaseArray.ZeroPad_DataCentered(Axis.XAxis, (int)(BaseArray.GetLength(Axis.XAxis ) * ZeroPadding));
            }

            return DoBackProjection(ProjectionT, impulse);
        }

        /// <summary>
        /// Creates the backprojecting slice
        /// </summary>
        /// <param name="impulse"></param>
        /// <returns></returns>
        public PhysicalArray  DoBackProjection( double[] impulse)
        {
            PhysicalArray ProjectionT = null;
            ProjectionT = Projection;

            return DoBackProjection(ProjectionT, impulse);
        }

        public PhysicalArray DoBackProjection(double[] ProjectionT, double[] impulse)
        {
            double tau = (PhysicalEnd - PhysicalStart) / ((double)ProjectionT.Length);
            PhysicalArray  tBackProjection = new PhysicalArray( MathHelps.ConvoluteChop(ProjectionT, impulse),PhysicalStart,PhysicalEnd) ;
            tBackProjection *= tau;
            BackProjection = tBackProjection;
            return tBackProjection;
        }

        public abstract  PhysicalArray  DoBackProjection(PhysicalArray ProjectionT, double[] impulse);

    }
}
