using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ImageViewer.Filters;
using MathHelpLib;
using System.Threading;
using MathHelpLib.ProjectionFilters;



namespace ImageViewer.PythonScripting.Projection
{
    public class CreateFilteredBackProjectionEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Create Filtered BackProjection Object"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Projection"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        static object CriticalSectionLock = new object();
        static object CriticalSectionLock2 = new object();

        public class CreateProjectionObjectToken : IEffectToken
        {
            public string TokenName()
            {
                return this.ToString();
            }
            public object[] DensityGrid = null;
        }



        // public static PhysicalArray[] mDensityGrid = null;
        //  public static ProjectionArrayObject[] mDensityGrid = null;

        /// <summary>
        /// Creates a physical array to host the recon data
        /// </summary>
        /// <param name="TotalReconObjects">should be 1 in most cases.  allows multiple cores to be built to help avoid thread contension</param>
        /// <param name="nCols">Both the width and depth of the recon cube</param>
        /// <param name="nRows">Number of z levels of recon cube.  index height</param>
        /// <param name="PhysicalWidth"></param>
        /// <param name="PhysicalHeight"></param>
        /// <returns></returns>
        public static PhysicalArray CreatePhysicalArray(int TotalReconObjects, int nCols, int nRows, double PhysicalWidth, double PhysicalHeight,CreateProjectionObjectToken cpot )
        {
            lock (CriticalSectionLock2)
            {
                if (cpot. DensityGrid == null)
                {
                    cpot.DensityGrid = new PhysicalArray[TotalReconObjects];
                }

                int CurrentCore = Thread.CurrentThread.ManagedThreadId % TotalReconObjects;

                if (cpot.DensityGrid[CurrentCore] == null)
                {
                    cpot.DensityGrid[CurrentCore] = new PhysicalArray(nCols, nCols, nRows,
                             PhysicalWidth / -2d, PhysicalWidth / 2d,
                             PhysicalWidth / -2d, PhysicalWidth / 2d,
                             PhysicalHeight / -2d, PhysicalHeight / 2d);
                }

                return (PhysicalArray)cpot.DensityGrid[CurrentCore];
            }
        }

        /// <summary>
        /// Creates a projectionarrayobject to host the recon data
        /// </summary>
        /// <param name="TotalReconObjects">should be 1 in most cases.  allows multiple cores to be built to help avoid thread contension</param>
        /// <param name="nCols">Both the width and depth of the recon cube</param>
        /// <param name="nRows">Number of z levels of recon cube.  index height</param>
        /// <param name="PhysicalWidth"></param>
        /// <param name="PhysicalHeight"></param>
        /// <returns></returns>
        public static ProjectionArrayObject CreateProjectionArrayObject(bool UsingGPU, int TotalReconObjects, int nCols, int nRows, double PhysicalWidth, double PhysicalHeight, CreateProjectionObjectToken cpot)
        {
            lock (CriticalSectionLock2)
            {
                if (cpot.DensityGrid == null)
                {
                    cpot.DensityGrid = new ProjectionArrayObject[TotalReconObjects];
                }

                int CurrentCore = 0;// Thread.CurrentThread.ManagedThreadId % TotalReconObjects;

                if (cpot.DensityGrid[CurrentCore] == null)
                {

                    if (UsingGPU /*&& (nCols + nRows) < 500*/)
                    {
                        cpot.DensityGrid[CurrentCore] = new ProjectionArrayObject(true, nCols, nCols, nRows,
                             PhysicalWidth / -2d, PhysicalWidth / 2d,
                             PhysicalWidth / -2d, PhysicalWidth / 2d,
                             PhysicalHeight / -2d, PhysicalHeight / 2d);
                    }
                    else
                    {
                        cpot.DensityGrid[CurrentCore] = new ProjectionArrayObject(nCols, nCols, nRows,
                                PhysicalWidth / -2d, PhysicalWidth / 2d,
                                PhysicalWidth / -2d, PhysicalWidth / 2d,
                                PhysicalHeight / -2d, PhysicalHeight / 2d);
                    }
                }

                return (ProjectionArrayObject)cpot.DensityGrid[CurrentCore];
            }
        }

        public static ProjectionArrayObject CreateProjectionArrayObject( int nCols, int nRows, int ZStart,int ZEnd, double PhysicalWidth, double PhysicalHeight)
        {
            lock (CriticalSectionLock2)
            {
                ProjectionArrayObject DensityGrid;

                DensityGrid = new ProjectionArrayObject( nCols, nCols, nRows,
                     PhysicalWidth / -2d, PhysicalWidth / 2d,
                     PhysicalWidth / -2d, PhysicalWidth / 2d,
                     PhysicalHeight / -2d, PhysicalHeight / 2d,ZStart,ZEnd);
                  

                return DensityGrid;
            }
        }


        /// <summary>
        /// Creates an object to build the recon into.  This can be a physical array or a projectionarrayobject
        /// Multiple objects can be created to help with thread contension
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">Multiple versions A. Slice as Imageholder or double[,] or double[,,]; 2. slice physical width 3. slice physical height  4. Number of cores 5. Create PhysicalArray
        /// B. 1. num Cols 2. num Rows 3. slice physical width 4. slice physical height 5. Number of cores 6. Create PhysicalArray. 7. Using GPU  </param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mPassData = PassData;

            mFilterToken = Parameters;

            lock (CriticalSectionLock)
            {

                CreateProjectionObjectToken cpot = null;// new CreateProjectionObjectToken();
                if (dataEnvironment.EffectTokens.ContainsKey("CreateProjectionArray") == true)
                    cpot = (CreateProjectionObjectToken)dataEnvironment.EffectTokens["CreateProjectionArray"];
                else
                {
                    cpot = new CreateProjectionObjectToken();
                    dataEnvironment.EffectTokens.Add("CreateProjectionArray", cpot);
                }

                int NCores = 1;
                int nCols = 256, nZCols = 256;
                double dWidth = 2, dHeight = 2;
                bool createPhysicalArray = false;
                bool UsingGPU = true;

                if (mFilterToken[0].GetType() == typeof(ImageHolder))
                {
                    nCols = ((ImageHolder)mFilterToken[0]).Width;
                    nZCols = ((ImageHolder)mFilterToken[0]).Height;
                    dWidth = EffectHelps.ConvertToDouble(mFilterToken[1]);
                    dHeight = EffectHelps.ConvertToDouble(mFilterToken[2]);
                    NCores = (int)mFilterToken[3];

                    createPhysicalArray = (bool)mFilterToken[4];
                    UsingGPU = (bool)mFilterToken[5];
                }
                else if (mFilterToken[0].GetType() == typeof(double[,]))
                {
                    nCols = ((double[,])mFilterToken[0]).GetLength(1);
                    nZCols = ((double[,])mFilterToken[0]).GetLength(0);
                    dWidth = EffectHelps.ConvertToDouble(mFilterToken[1]);
                    dHeight = EffectHelps.ConvertToDouble(mFilterToken[2]);
                    NCores = (int)mFilterToken[3];
                    createPhysicalArray = (bool)mFilterToken[4];
                    UsingGPU = (bool)mFilterToken[5];
                }
                else if (mFilterToken[0].GetType() == typeof(double[, ,]))
                {
                    nCols = ((double[, ,])mFilterToken[0]).GetLength(1);
                    nZCols = ((double[, ,])mFilterToken[0]).GetLength(0);
                    dWidth = EffectHelps.ConvertToDouble(mFilterToken[1]);
                    dHeight = EffectHelps.ConvertToDouble(mFilterToken[2]);
                    NCores = (int)mFilterToken[3];
                    createPhysicalArray = (bool)mFilterToken[4];
                    UsingGPU = (bool)mFilterToken[5];
                }
                else if (mFilterToken[0].GetType() == typeof(int))
                {
                    nCols = (int)mFilterToken[0];
                    nZCols = (int)mFilterToken[1];
                    dWidth = EffectHelps.ConvertToDouble(mFilterToken[2]);
                    dHeight = EffectHelps.ConvertToDouble(mFilterToken[3]);
                    NCores = (int)mFilterToken[4];
                    createPhysicalArray = (bool)mFilterToken[5];
                    UsingGPU = (bool)mFilterToken[6];
                }

                UsingGPU = true;
                if (createPhysicalArray)
                    mPassData.AddSafe("FBJObject", CreatePhysicalArray(NCores, nCols, nZCols, dWidth, dHeight,cpot ));
                else
                    mPassData.AddSafe("FBJObject", CreateProjectionArrayObject(UsingGPU, NCores, nCols, nZCols, dWidth, dHeight,cpot ));


                return SourceImage;
            }
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "FirstSlice(0 Degrees)|ImageHolder or double[,,]", "SliceWidth|double", "SliceHeight|double", "NumberOfCores|int", "CreatePhysicalArray|bool" }; }
        }
    }
}
