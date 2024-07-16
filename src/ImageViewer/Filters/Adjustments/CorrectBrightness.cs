using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace ImageViewer.Filters.Adjustments
{
    public class CorrectBrightnessEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Correct Brightness"; } }
        public override string EffectMenu { get { return "Adjustment"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 10; } }

        
        protected override Bitmap doEffect(Bitmap SourceImage, IEffectToken FilterToken)
        {
            BrightnessCorrection Filter = new BrightnessCorrection(.5);
            return EffectHelps.FixImageFormat(Filter.Apply(SourceImage));
        }
        public override IEffectToken CreateToken(params object[] TokenValues)
        {
            mFilterToken = new GeneralToken();
            mFilterToken.Parameters = TokenValues;
            return mFilterToken ;
        }
        public override IEffectToken DefaultToken 
        {
            get
            {
                return CreateToken(null);
            }
        }

        
           
        
        
    }
}
