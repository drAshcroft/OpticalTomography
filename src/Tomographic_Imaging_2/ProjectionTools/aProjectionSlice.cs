using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathHelpLib;
using System.IO;
using System.Drawing;
using MathHelpLib.ImageProcessing;

namespace Tomographic_Imaging_2
{
    
    public abstract class aProjectionSlice:IDisposable 
    {
        public double Angle;
        protected  double mPhysicalXwidth;
        protected double mPhysicalYWidth;
        protected bool  mPersist = false;
        protected PhysicalArray mBackProjection;

        /// <summary>
        /// Provides a chech for the more expensive slices like file slice,  a check can be performed without loading the backprojection data
        /// </summary>
        protected bool mBackProjectionPerformed;

        public abstract PhysicalArrayRank ProjectionRank
        {
            get;
        }

        public abstract  void Dispose();
        
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

        public virtual  double PhysicalStartX
        {
            get { return -.5 * mPhysicalXwidth; }
        }

        public virtual  double PhysicalEndX
        {
            get { return .5 * mPhysicalXwidth; }
        }

        public virtual  double PhysicalStartY
        {
            get { return -.5 * mPhysicalYWidth; }
        }

        public virtual  double PhysicalEndY
        {
            get { return .5 * mPhysicalYWidth; }
        }

        public virtual  bool PersistDataInMemory
        {
            get { return mPersist; }
            set { mPersist = value; }
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
            this.Projection = new PhysicalArray(Projection, PhysicalStartX, PhysicalEndX, PhysicalStartY, PhysicalEndY,true );
        }

        public void SetProjection(double[,] Projection, double[] PhysicalStart, double[] PhysicalEnd)
        {
            this.Projection = new PhysicalArray(Projection, PhysicalStart, PhysicalEnd);
        }
        /// <summary>
        /// this would be an expensive operation to call repeatedly.  Make offsetX local copy to do the math
        /// you must call DoBackProjection_AllSlices for this to have offsetX value
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
        /// <param name="impulseFreqSpace"></param>
        /// <returns></returns>
        public PhysicalArray DoBackProjection(double[] impulse,int ZeroPaddedSize,ConvolutionMethod convolutionMethod )
        {
            PhysicalArray ProjectionT = null;
            if (Projection.ArrayRank == PhysicalArrayRank.Array1D)
                ProjectionT = Projection.ZeroPad_DataCentered(Axis.XAxis, ZeroPaddedSize);
            else
            {
               // ProjectionT = Projection.ZeroPad_DataCentered(Axis.XAxis, ZeroPaddedSize);
                //ProjectionT =  ProjectionT.ZeroPad_DataCentered(Axis.YAxis, ZeroPaddedSize);
            }
            ProjectionT = DoBackProjection(Projection, impulse ,convolutionMethod );

            if (Projection.ArrayRank == PhysicalArrayRank.Array1D)
                ProjectionT.TruncateDataInPlace(Axis.XAxis , Projection.PhysicalStart(Axis.XAxis ), Projection.PhysicalEnd(Axis.YAxis ));
            else 
            {
               // ProjectionT.TruncateDataInPlace(Axis.XAxis , Projection.PhysicalStart(Axis.XAxis ), Projection.PhysicalEnd(Axis.XAxis ));
                //ProjectionT.TruncateDataInPlace(Axis.YAxis , Projection.PhysicalStart(Axis.YAxis ), Projection.PhysicalEnd(Axis.YAxis ));
            }

            if (mPersist)
                this.mBackProjection = ProjectionT;
            return ProjectionT;
        }

        /// <summary>
        /// Creates the backprojecting slice
        /// </summary>
        /// <param name="impulseFreqSpace"></param>
        /// <returns></returns>
        public PhysicalArray DoBackProjection(double[,] impulse,int ZeroPaddedSize, ConvolutionMethod convolutionMethod)
        {
            PhysicalArray ProjectionT = null;
            string junk = Projection.GetLength(Axis.XAxis).ToString() + "," + Projection.GetLength(Axis.YAxis).ToString() + "," + Projection.GetLength(Axis.ZAxis).ToString() ;
            if (Projection.ArrayRank == PhysicalArrayRank.Array1D)
                ProjectionT = Projection.ZeroPad_DataCentered(Axis.XAxis, ZeroPaddedSize);
            else
            {
                ProjectionT = Projection.ZeroPad_DataCentered(Axis.XAxis, ZeroPaddedSize);
                ProjectionT= ProjectionT.ZeroPad_DataCentered(Axis.YAxis, ZeroPaddedSize);
            }
            
            ProjectionT= DoBackProjection(ProjectionT, impulse, convolutionMethod);

            if (Projection.ArrayRank == PhysicalArrayRank.Array1D)
                ProjectionT.TruncateDataInPlace(Axis.XAxis, Projection.PhysicalStart(Axis.XAxis), Projection.PhysicalEnd(Axis.YAxis));
            else
            {
                ProjectionT.TruncateDataInPlace(Axis.XAxis, Projection.PhysicalStart(Axis.XAxis), Projection.PhysicalEnd(Axis.XAxis));
                ProjectionT.TruncateDataInPlace(Axis.YAxis, Projection.PhysicalStart(Axis.YAxis), Projection.PhysicalEnd(Axis.YAxis));
            }

            junk += "," + ProjectionT.GetLength(Axis.XAxis).ToString() + "," + ProjectionT.GetLength(Axis.YAxis).ToString() + "," + ProjectionT.GetLength(Axis.ZAxis).ToString();

            System.Diagnostics.Debug.Print(junk);
            if (mPersist)
                this.mBackProjection = ProjectionT;
            return ProjectionT;

        }

        protected abstract PhysicalArray DoBackProjection(PhysicalArray ProjectionT, double[] impulse, ConvolutionMethod convolutionMethod);
        protected abstract PhysicalArray DoBackProjection(PhysicalArray ProjectionT, double[,] impulse,  ConvolutionMethod convolutionMethod);
      
        #region FileActions
        protected void SaveSliceData(PhysicalArray SliceData,  string Filename)
        {
            string extention=Path.GetExtension(Filename).ToLower();
            if ( extention  == ".cct")
            {
                #region RawFile 
                FileStream BinaryFile = new FileStream(Filename, FileMode.Create, FileAccess.Write);
                BinaryWriter Writer = new BinaryWriter(BinaryFile);

                Writer.Write((Int32)SliceData.ArrayRank);

                Writer.Write((Int32)SliceData.GetLength(Axis.XAxis));
                Writer.Write((Int32)SliceData.GetLength(Axis.YAxis));
                Writer.Write((Int32)SliceData.GetLength(Axis.ZAxis));

                Writer.Write((double)SliceData.PhysicalStart(Axis.XAxis));
                Writer.Write((double)SliceData.PhysicalEnd(Axis.XAxis));

                Writer.Write((double)SliceData.PhysicalStart(Axis.YAxis));
                Writer.Write((double)SliceData.PhysicalEnd(Axis.YAxis));

                Writer.Write((double)SliceData.PhysicalStart(Axis.ZAxis));
                Writer.Write((double)SliceData.PhysicalEnd(Axis.ZAxis));

                for (int z = 0; z < SliceData.GetLength(Axis.ZAxis); z++)
                {
                    for (int y = 0; y < SliceData.GetLength(Axis.YAxis); y++)
                    {
                        for (int x = 0; x < SliceData.GetLength(Axis.XAxis); x++)
                        {
                            Writer.Write((double)SliceData[x, y, z]);
                        }
                    }
                }
                Writer.Close();
                BinaryFile.Close();
                #endregion
            }
            else if (extention == ".bmp" || extention == ".gif" || 
                extention == ".jpeg" || extention == ".png" || extention == ".tiff" || extention == ".tif" || 
                extention == ".jpg")
            {
                Bitmap b = SliceData.MakeBitmap();
                b.Save(Filename);
            }


        }

        protected PhysicalArray OpenSliceData(string Filename,bool RotateImage90Degrees)
        {
            string extention = Path.GetExtension(Filename).ToLower();
            if (extention  == ".cct")
            {
                #region RawFile
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);
                int sizeX, sizeY, sizeZ;

                PhysicalArrayRank ArrayRank = (PhysicalArrayRank)Reader.ReadInt32();

                sizeX = Reader.ReadInt32();
                sizeY = Reader.ReadInt32();
                sizeZ = Reader.ReadInt32();



                PhysicalArray  DensityGrid = new PhysicalArray(sizeX, sizeY, sizeZ,
                    Reader.ReadDouble(), Reader.ReadDouble(),
                    Reader.ReadDouble(), Reader.ReadDouble(),
                    Reader.ReadDouble(), Reader.ReadDouble()
                    );

                DensityGrid.ArrayRank = ArrayRank;
                for (int z = 0; z < sizeZ; z++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            DensityGrid[x, y, z] = Reader.ReadDouble();
                        }
                    }
                }
                Reader.Close();
                BinaryFile.Close();
                return DensityGrid;
                #endregion
            }
            if (extention == ".bmp" || extention == ".gif" ||
                extention == ".jpeg" || extention == ".png" || extention == ".tiff" || extention == ".tif" ||
                extention == ".jpg")
            {
                
                
                double[,] ImageArray=  MathImageHelps.LoadStandardImage_Intensity(Filename,RotateImage90Degrees     );
                return new PhysicalArray(ImageArray, PhysicalStartX ,PhysicalEndX , PhysicalStartY , PhysicalEndY ,false  );
            }
            return null;
        }
        #endregion
    }
}
