using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using ImageViewer.Filters;

namespace ImageViewer.PythonScripting.Programming_Tools
{
    public class CloneImageToPassDataEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Clone image to PassData"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Programming"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 1;
            }
        }
        
        public override bool PassesPassData
        {
            get
            {
                return true;
            }
        }
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             ReplaceStringDictionary PassData, params object[] Parameters)
        {

            mPassData.AddSafe("OrigImage", EffectHelps.FixImageFormat( SourceImage).Copy ());

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
        public override string getMacroString()
        {
            return @"
#Clone image to PassData
Filter =PythonScripting.Programming_Tools.CloneImageToPassDataEffect()
BitmapImage=Filter.RunEffect(BitmapImage,None)
#ImageData out of type :System.Drawing.Bitmap
PassData = Filter.PassData
OriginalImage = Filter.PassData";
        }
    }
}
