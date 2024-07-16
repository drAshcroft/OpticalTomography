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
using ImageViewer3D.Convolution;

namespace ImageViewer3D.Filters.Effects.RankOrder
{
    public partial class MedianFilterTool3D : aEffectForm3D
    {
        public MedianFilterTool3D()
            : base()
        {
            SetParameters(new string[] { "Radius" }, new int[] { 3 }, new int[] { 25 });

        }
        public override string EffectName { get { return "Median Filter"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Rank Order Statistical"; } }
        public override int OrderSuggestion { get { return 5; } }

        public override object[] DefaultProperties
        {
            get { return new object[] { 3 }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Filter_Radius|int" }; }
        }

        protected override DataHolder doEffect(
            DataEnvironment3D dataEnvironment, DataHolder SourceImage,
            ImageViewer.Filters.ReplaceStringDictionary PassData,
            params object[] Parameters)
        {

            mParameters = Parameters;
            if (mParameters == null)
                mParameters = DefaultProperties;
            int value = (int)Parameters[0];
            if (value < 3) value = 3;
            if (value % 2 == 0) value++;
            mParameters[0] = (int)value;
            MedianKernal3D vak = new MedianKernal3D((int)mParameters[0]);


            if (SourceImage != null)
                return ConvolutionFilterImplented3D.ConvolutionFilter(SourceImage, vak);
            else
                return new DataHolder(ConvolutionFilterImplented3D.ConvolutionFilter((float[, ,])mParameters[1], vak));


        }


    }
}
