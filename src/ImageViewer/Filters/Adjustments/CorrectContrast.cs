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
    public class CorrectContrastEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Correct Contrast"; } }
        public override string EffectMenu { get { return "Adjustment"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 11; } }
        protected override Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> doEffect(Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> SourceImage, IEffectToken FilterToken)
        {
            ContrastCorrection Filter = new ContrastCorrection(.5);
            return EffectHelps.FixImageFormat(Filter.Apply(SourceImage));
        }
        public override IEffectToken CreateToken(params object[] TokenValues)
        {
            mFilterToken = new GeneralToken();
            mFilterToken.Parameters = TokenValues;
            return mFilterToken;
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
