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
using MathHelpLib;

namespace ImageViewer.PythonScripting.Programming_Tools
{
    public class PassDataToImageEffect : aEffectNoForm
    {
        public override string EffectName { get { return "PassData to Active image"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Programming"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 2;
            }
        }
       
        public override bool PassesPassData
        {
            get
            {
                return false ;
            }
        }

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
              ReplaceStringDictionary PassData, params object[] Parameters)
        {
            if (Parameters!=null)
            {
                if (Parameters[0].GetType ()==typeof(ImageHolder))
                {
                    return (ImageHolder)Parameters[0];
                }
                else if (Parameters[0].GetType ()==typeof(Bitmap ))
                {
                    return new ImageHolder((Bitmap)Parameters[0]);
                }
            }
                //if we make it to hear, it must not be in the parameter list,  try the passdata
            if (mPassData.ContainsKey("OrigImage")==true)
            {
                if (mPassData["OrigImage"].GetType() == typeof(ImageHolder))
                    return (ImageHolder)mPassData["OrigImage"];
            }
             
            return null;
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
#PassData to Active image
Filter =PythonScripting.Programming_Tools.PassDataToImageEffect()
#Accepts data of type: System.Drawing.Bitmap
if type(PassData).__name__ =='Bitmap':
    Filter.PassData=PassData
else:
    try:
        Filter.PassData = OriginalImage
    except:
        print 'No data to restore'
BitmapImage=Filter.RunEffect(BitmapImage,None)";

        }
        
    }
}
