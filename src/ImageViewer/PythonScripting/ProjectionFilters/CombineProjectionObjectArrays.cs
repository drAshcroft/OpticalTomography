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



namespace ImageViewer.PythonScripting.Projection
{
    public class CombineProjectionObjectArrays : aEffectNoForm
    {
        public override string EffectName { get { return "Join all Projection Object Arrays"; } }
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
       

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mPassData = PassData;

            mFilterToken = Parameters;

            lock (CriticalSectionLock)
            {

                for (int i = 1; i < CreateFilteredBackProjectionArrayEffect.mDensityGrid.Length; i++)
                {
                    CreateFilteredBackProjectionArrayEffect.mDensityGrid[0].AddInPlace( CreateFilteredBackProjectionArrayEffect.mDensityGrid[i]);
                }



                mPassData["CombinedProjection"] = CreateFilteredBackProjectionArrayEffect.mDensityGrid[0];
                CreateFilteredBackProjectionArrayEffect.mDensityGrid = null;
                return SourceImage;
            }
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "none|none" }; }
        }
    }
}
