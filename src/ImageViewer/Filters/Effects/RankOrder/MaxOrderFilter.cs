using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using MathHelpLib;
using MathHelpLib.Convolution;

namespace ImageViewer.Filters.Effects.RankOrder
{
    public partial class MaxFilterTool : aEffectForm
    {
        public MaxFilterTool():base()
        {
            SetParameters(new string[] { "Radius" }, new int[] { 3 }, new int[] { 25 });
          
        }
        public override string EffectName { get { return "Max Filter"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Rank Order Statistical"; } }
        public override int OrderSuggestion { get { return 5; } }

        public override object[] DefaultProperties
        {
            get { return new object[] {3 }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "Filter_Radius|int" }; }
        }

        protected override object doEffect(DataEnvironment dataEnvironment, object SourceImage, ReplaceStringDictionary PassData, params object[] Parameters)
        {
            
            mFilterToken = Parameters;
            if (mFilterToken == null)
                mFilterToken = DefaultProperties;
                int value = (int)Parameters[0];
                if (value < 3) value = 3;
                if (value % 2 == 0) value++;
                mFilterToken[0] = (int)value;
                MaxKernal vak = new MaxKernal((int)mFilterToken[0]);
                if (SourceImage.GetType() == typeof(Bitmap))
                {
                    return ConvolutionFilterImplented.ConvolutionFilter((Bitmap)SourceImage, vak);
                }
                else if (SourceImage.GetType() == typeof(ImageHolder))
                {
                    return ConvolutionFilterImplented.ConvolutionFilter((ImageHolder)SourceImage, vak);
                }
               
                return null;
        }
        
    }
}
