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
using MathHelpLib.Convolution;
using MathHelpLib;

namespace ImageViewer.Filters.Effects.RankOrder
{
    public partial class AverageFilterTool : aEffectForm
    {
        public AverageFilterTool()
            : base()
        {
            SetParameters(new string[] { "Radius" }, new int[] { 3 }, new int[] { 25 });
        }
        public override string EffectName { get { return "Average Filter"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Rank Order Statistical"; } }
        public override int OrderSuggestion { get { return 5; } }


        public override object[] DefaultProperties
        {
            get { return new object[] { 5 }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Average_Radius|int" }; }
        }

        public static ImageHolder AverageFilter(ImageHolder SourceImage, int Radius)
        {
            if (Radius < 3) Radius = 3;
            if (Radius % 2 == 0) Radius++;
            AverageConvolutionKernal vak = new AverageConvolutionKernal(Radius);
            return ConvolutionFilterImplented.ConvolutionFilter(SourceImage, vak);
        }

        public static Bitmap AverageFilter(Bitmap SourceImage, int Radius)
        {
            if (Radius < 3) Radius = 3;
            if (Radius % 2 == 0) Radius++;
            AverageConvolutionKernal vak = new AverageConvolutionKernal(Radius);
            return ConvolutionFilterImplented.ConvolutionFilter(SourceImage, vak);
        }

        public static double[,] AverageFilter(double[,] SourceImage, int Radius)
        {
            if (Radius < 3) Radius = 3;
            if (Radius % 2 == 0) Radius++;
            AverageConvolutionKernal vak = new AverageConvolutionKernal(Radius);
            return ConvolutionFilterImplented.ConvolutionFilter(SourceImage, vak);
        }

        protected override object doEffect(DataEnvironment dataEnvironment, object SourceImage,
           ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            if (mFilterToken == null)
                mFilterToken = DefaultProperties;
            int value = (int)Parameters[0];
            if (value < 3) value = 3;
            if (value % 2 == 0) value++;
            mFilterToken[0] = (int)value;

            AverageConvolutionKernal vak = new AverageConvolutionKernal((int)mFilterToken[0]);

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
