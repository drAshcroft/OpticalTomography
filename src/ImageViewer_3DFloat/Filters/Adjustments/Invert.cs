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

namespace ImageViewer3D.Filters.Adjustments
{
    public class InvertEffect : aEffectNoForm3D
    {
        public override string EffectName { get { return "Invert Contrast"; } }
        public override string EffectMenu { get { return "Adjustment"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 20; } }

        public override DataHolder DoEffect(DataEnvironment3D dataEnvironment, DataHolder SourceImage, ReplaceStringDictionary PassData, params object[] Parameters)
        {
            unsafe
            {
                fixed (float  * pSource = SourceImage.Data )
                {
                    float  * pOut = pSource;
                    for (int i = 0; i < SourceImage.Data .Length; i++)
                    {
                        *pOut = *pOut * -1;
                        pOut++;
                    }
                }
            }
            return SourceImage;
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
