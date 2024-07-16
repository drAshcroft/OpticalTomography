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
    public partial class AlphaTrimmedMeanFilterTool : aEffectForm
    {
        public AlphaTrimmedMeanFilterTool()
            : base()
        {
            SetParameters(new string[] { "Radius","alpha" }, new int[] { 3,0 }, new int[] { 25,50 });
        }
        public override string EffectName { get { return "Alpha Trimmed Mean Filter"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Rank Order Statistical"; } }
        public override int OrderSuggestion { get { return 5; } }


        public override object[] DefaultProperties
        {
            get { return new object[] { 5,25 }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Average_Radius|int","alpha(percent)|int" }; }
        }

        public static ImageHolder AlphaTrimmedFilter(ImageHolder SourceImage, int Radius, int alpha)
        {
            if (Radius < 3) Radius = 3;
            if (Radius % 2 == 0) Radius++;
            ATMConvolutionKernal vak = new ATMConvolutionKernal(Radius, alpha );
            return ConvolutionFilterImplented.ConvolutionFilter(SourceImage, vak);
        }

        public static Bitmap AlphaTrimmedFilter(Bitmap SourceImage, int Radius, int alpha)
        {
            if (Radius < 3) Radius = 3;
            if (Radius % 2 == 0) Radius++;
            ATMConvolutionKernal vak = new ATMConvolutionKernal(Radius, alpha);
            return ConvolutionFilterImplented.ConvolutionFilter(SourceImage, vak);
        }

        public static double[,] AlphaTrimmedFilter(double[,] SourceImage, int Radius, int alpha)
        {
            if (Radius < 3) Radius = 3;
            if (Radius % 2 == 0) Radius++;
            ATMConvolutionKernal vak = new ATMConvolutionKernal(Radius, alpha);
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
            int alpha = (int)Parameters[1];
            ATMConvolutionKernal vak = new ATMConvolutionKernal((int)mFilterToken[0], alpha);

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
