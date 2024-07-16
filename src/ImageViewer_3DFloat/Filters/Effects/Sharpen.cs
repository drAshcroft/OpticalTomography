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

namespace ImageViewer3D.Filters.Effects
{
    public class SharpenEffect : aEffectNoForm3D
    {
        public override string EffectName { get { return "Sharpen"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
        public override DataHolder DoEffect(DataEnvironment3D dataEnvironment, DataHolder SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
           
            return null;
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
