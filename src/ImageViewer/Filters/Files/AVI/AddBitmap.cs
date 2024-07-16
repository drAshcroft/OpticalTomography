using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;


namespace ImageViewer.Filters.Files.AVI
{
    /*
    public class AddBitmapToAVIFileEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Add Bitmap to AVI File"; } }
        public override string EffectMenu { get { return "File"; } }
        public override string EffectSubMenu { get { return "AVI"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 15;
            }
        }

        /// <summary>
        /// Adds a bitmap to the AVI created with the CreateAVIFileEffect
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">No Param</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
                    ReplaceStringDictionary PassData, params object[] Parameters)
        {
            ///if there is no file then throw error
            if (CreateAVIFileEffect.aviManager == null)
            {
                throw new Exception("AVI File has not been created");
            }
            //Just add a frame
            if (CreateAVIFileEffect.aviStream!=null)
                CreateAVIFileEffect.aviStream.AddFrame( EffectHelps.ConvertToBitmap(SourceImage) );

            return SourceImage;
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { null }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }
        
        public override void ShowInterface(IWin32Window owner)
        {
         
        }
    }*/
}
