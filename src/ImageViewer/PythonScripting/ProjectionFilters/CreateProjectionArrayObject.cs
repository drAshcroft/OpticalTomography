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
    public class CreateFilteredBackProjectionArrayEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Create Filtered BackProjection Array Object"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Projection"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        public static void ClearMem()
        {
            mDensityGrid = null;

        }

        static object CriticalSectionLock = new object();
        public  static ProjectionArrayObject [] mDensityGrid = null;
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mPassData = PassData;

            mFilterToken = Parameters;

            lock (CriticalSectionLock)
            {
                int NCores=1;
                int nCols = 256, nZCols = 256;
                double dWidth = 2, dHeight = 2;

                if (mFilterToken[0].GetType() == typeof(ImageHolder))
                {
                    nCols = ((ImageHolder)mFilterToken[0]).Width;
                    nZCols = ((ImageHolder)mFilterToken[0]).Height;
                    dWidth = EffectHelps.ConvertToDouble(mFilterToken[1]);
                    dHeight = EffectHelps.ConvertToDouble(mFilterToken[2]);
                    NCores = (int)mFilterToken[3];
                }
                else if (mFilterToken[0].GetType() == typeof(double[,]))
                {
                    nCols = ((double[,])mFilterToken[0]).GetLength(1);
                    nZCols = ((double[,])mFilterToken[0]).GetLength(0);
                    dWidth = EffectHelps.ConvertToDouble(mFilterToken[1]);
                    dHeight = EffectHelps.ConvertToDouble(mFilterToken[2]);
                    NCores = (int)mFilterToken[3];
                }
                else if (mFilterToken[0].GetType() == typeof(double[, ,]))
                {
                    nCols = ((double[, ,])mFilterToken[0]).GetLength(1);
                    nZCols = ((double[, ,])mFilterToken[0]).GetLength(0);
                    dWidth = EffectHelps.ConvertToDouble(mFilterToken[1]);
                    dHeight = EffectHelps.ConvertToDouble(mFilterToken[2]);
                    NCores = (int)mFilterToken[3];
                }
                else if (mFilterToken[0].GetType() == typeof(int))
                {
                    nCols = (int)mFilterToken[0];
                    nZCols = (int)mFilterToken[1];
                    dWidth = EffectHelps.ConvertToDouble(mFilterToken[2]);
                    dHeight = EffectHelps.ConvertToDouble(mFilterToken[3]);
                    NCores = (int)mFilterToken[4];
                }


                if (mDensityGrid == null)
                {
                    mDensityGrid = new ProjectionArrayObject [NCores ];
                }

                int CurrentCore = Thread.CurrentThread.ManagedThreadId % NCores ;

                if (mDensityGrid [CurrentCore ]==null )
                {
                    mDensityGrid[CurrentCore] = new ProjectionArrayObject(nCols, nCols, nZCols,
                         dWidth / -2d, dWidth / 2d,
                         dWidth / -2d, dWidth / 2d,
                         dHeight / -2d, dHeight / 2d);
                }

                mPassData.AddSafe("FBJObject", mDensityGrid[CurrentCore]);
                return SourceImage;
            }
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "FirstSlice(0 Degrees)|ImageHolder or double[,,]", "SliceWidth|double", "SliceHeight|double", "NumberOfCores|int" }; }
        }
    }
}
