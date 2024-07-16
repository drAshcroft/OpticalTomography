using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathHelpLib;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace Tomographic_Imaging_2
{
    public class ProjectionSliceFile : aProjectionSlice
    {
        string mFilename;
        string mbackProjectionFilename;

        PhysicalArray mProjection = null;

        PhysicalArrayRank mRank = PhysicalArrayRank.Array1D;
        bool mRotateImage;

        public override void Dispose()
        {
            try { mProjection.Dispose(); }
            catch { }
            try { mBackProjection.Dispose(); }
            catch { }
        }


        public override PhysicalArrayRank ProjectionRank
        {
            get
            {
                return mRank;
            }
        }


        public override bool PersistDataInMemory
        {
            get { return mPersist; }
            set
            {
                mPersist = value;
                if (mPersist == false)
                {
                    mProjection = null;
                    mBackProjection = null;
                }

            }
        }

/*        public void ConvertFromImageHolder(ImageHolder image)
        {
            Projection = new PhysicalArray(image, -1, 1, -1, 1);
            mBackProjection = null;
        }*/

        public override PhysicalArray Projection
        {
            get
            {
                if (mPersist == false || this.mProjection == null)
                {
                    PhysicalArray mProjection = OpenSliceData(mFilename, mRotateImage);
                    if (mProjection.PhysicalLength(Axis.XAxis) == 0)
                    {
                        mProjection.PhysicalStartSet(Axis.XAxis, mPhysicalXwidth / -2d);
                        mProjection.PhysicalEndSet(Axis.XAxis, mPhysicalXwidth / 2d);
                        mProjection.PhysicalStartSet(Axis.YAxis, mPhysicalYWidth / -2d);
                        mProjection.PhysicalEndSet(Axis.YAxis, mPhysicalYWidth / 2d);
                    }
                    if (mPersist)
                    {
                        this.mProjection = mProjection;
                    }
                    return mProjection;
                }
                return this.mProjection;
            }

            set
            {
                if (mPersist == false)
                {
                    mRank = value.ArrayRank;
                    mFilename = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + Path.GetFileNameWithoutExtension(mFilename) + ".cct";
                    SaveSliceData(value, mFilename);
                    mRotateImage = false;
                }
                else
                {
                    mProjection = value;
                    mPhysicalXwidth  =mProjection.PhysicalEnd(Axis.XAxis)- mProjection.PhysicalStart(Axis.XAxis);
                    mPhysicalYWidth  = mProjection.PhysicalEnd(Axis.YAxis) - mProjection.PhysicalStart(Axis.YAxis);
                  
                }
            }
        }

        public override PhysicalArray BackProjection
        {
            get
            {
                if (mPersist && mBackProjection != null)
                    return mBackProjection;

                if (mBackProjectionPerformed)
                {
                    return OpenSliceData(mbackProjectionFilename, false);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                mBackProjectionPerformed = true;
                if (mPersist)
                    mBackProjection = value;
                else
                    throw new Exception("Not yet implimented");
            }
        }
        private object CriticalSectionLocker = new object();

        private object Lock1DStaticObject = new object();
        private float[] fImpulse1D = null;

        protected override PhysicalArray DoBackProjection(PhysicalArray ProjectionT, double[] impulse, ConvolutionMethod convolutionMethod)
        {

            double tau;
            PhysicalArray tBackProjection;
            if (ProjectionT.ArrayRank == PhysicalArrayRank.Array1D)
            {
                tau = (PhysicalEndX - PhysicalStartX) / ((double)ProjectionT.Length);
                tBackProjection = ProjectionT.ConvoluteChop1D(Axis.YAxis, impulse);
            }
            else
            {
                tau = (PhysicalEndX - PhysicalStartX) / ((double)ProjectionT.Length);
                tau *= (PhysicalEndY - PhysicalStartY) / ((double)ProjectionT.Length);
                switch (convolutionMethod)
                {
                    case ConvolutionMethod.Convolution1D:
                        tBackProjection = ProjectionT.ConvoluteChop1D(Axis.XAxis, impulse);
                        break;
                    case ConvolutionMethod.ConvolutionFrequencySpaceFilterFFT:
                        //tBackProjection = ProjectionT.ConvoluteFFT2D(Axis.XAxis,Axis.YAxis,
                        throw new Exception("Not yet implemented");
                    // break;
                    case ConvolutionMethod.ConvolutionRealSpaceSeperable:
                        tBackProjection = ProjectionT.ConvoluteChopSeperable(Axis.XAxis, Axis.YAxis, impulse);
                        break;
                    case ConvolutionMethod.NoConvolution:
                        tBackProjection = ProjectionT;
                        tau = 1;
                        break;
                    case ConvolutionMethod.ConvolutionRealSpaceGPU:
                        lock (Lock1DStaticObject)
                        {
                            if (fImpulse1D == null)
                            {
                                fImpulse1D = new float[impulse.GetLength(0)];
                                for (int i = 0; i < impulse.GetLength(0); i++)
                                        fImpulse1D[i] = (float)impulse[i];
                            }
                        }
                        throw new Exception("Not yet implimented");
                        //tBackProjection = ProjectionT.ConvolutionGPU(Axis.XAxis, Axis.YAxis, fImpulse1D);
                        break;
                    default:
                        throw new Exception("Not appropriate for 1D impulse");
                }
            }

            tBackProjection *= tau;

            if (mPersist == false)
            {
                mbackProjectionFilename = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + Path.GetFileNameWithoutExtension(mFilename) + Angle.ToString().Replace('.', '_') + ".cct";
                SaveSliceData(tBackProjection, mbackProjectionFilename);
            }

            mBackProjectionPerformed = true;

            return tBackProjection;
        }


        static float[,] fImpulse;
        static object LockStaticObject = new object();
        protected override PhysicalArray DoBackProjection(PhysicalArray ProjectionT, double[,] impulse, ConvolutionMethod convolutionMethod)
        {

            double tau;
            PhysicalArray tBackProjection;
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
                        tBackProjection = ProjectionT.ConvoluteFFT2D(Axis.XAxis, Axis.YAxis, impulse, true);
                        break;
                    case ConvolutionMethod.ConvolutionFrequencySpaceFilterFFT:
                        tBackProjection = ProjectionT.ConvoluteFFT2D(Axis.XAxis, Axis.YAxis, impulse, false);
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
                       // tBackProjection = ProjectionT.ConvolutionGPU(Axis.XAxis, Axis.YAxis, fImpulse);
                        break;
                    case ConvolutionMethod.ConvolutionRealSpace2D:
                        tBackProjection = ProjectionT.ConvoluteReal2D(Axis.XAxis, Axis.YAxis, impulse);
                        break;
                    case ConvolutionMethod.NoConvolution :
                        tBackProjection = ProjectionT;
                        tau = 1;
                        break;
                    default:
                        throw new Exception("Not appropriate for 1D impulse");
                }
            }

            tBackProjection *= tau;
            // double[,] tflattened =   ImageViewer.ImagingTools.FlattenImageEdges((double[,]) tBackProjection.ActualData2D);
           

            //do not save the data if it is to stay in memory.  This slows down the reconstruction
            if (PersistDataInMemory == false)
            {
                mbackProjectionFilename = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + Path.GetFileNameWithoutExtension(mFilename) + Angle.ToString().Replace('.', '_') + ".cct";
                SaveSliceData(tBackProjection, mbackProjectionFilename);
            }

            mBackProjectionPerformed = true;

            return tBackProjection;
        }


        public void LoadFile(string FilenameWithPath, double AngleDegrees, double PhysicalXSize, double PhysicalYSize, bool RotateImage90Degrees)
        {
            mRotateImage = RotateImage90Degrees;
            Angle = AngleDegrees / 180 * Math.PI;
            mBackProjectionPerformed = false;
            mFilename = FilenameWithPath;
            this.mPhysicalXwidth = PhysicalXSize;
            this.mPhysicalYWidth = PhysicalYSize;
            this.mRank = PhysicalArrayRank.Array2D;
            if (PersistDataInMemory)
            {
                mProjection = Projection;
            }
        }
    }
}
