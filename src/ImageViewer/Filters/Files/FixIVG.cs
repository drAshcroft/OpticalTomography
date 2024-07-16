using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using MathHelpLib;



namespace ImageViewer.Filters.Effects
{
    public class FixVGImageEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Fix Visiongate Image"; } }
        public override string EffectMenu { get { return "File"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {

            if (SourceImage.GetType() == typeof(ImageHolder))
              return    MathHelpLib.MathHelpsFileLoader.FixVisionGateImage((ImageHolder)SourceImage);
            else 
              return   MathHelpLib.MathHelpsFileLoader.FixVisionGateImage((Bitmap)SourceImage);
           
        }


        public override object[] DefaultProperties
        {
            get { return null; }
        }
        public override string[] ParameterList
        {
            get { return new string[] { "" }; }
        }
    }
}
