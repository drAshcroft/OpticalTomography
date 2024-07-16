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

namespace ImageViewer.Filters.Effects
{
    public partial class SimpleThresholdTool : aEffectForm
    {
        public SimpleThresholdTool()
            : base()
        {
            SetParameters(new string[] { "Threshold" }, new int[] { 0 }, new int[] { 255 });
        }
        public override string EffectName { get { return "Threshold image"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Thresholding"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
        public override object[] DefaultProperties
        {
            get { return new object[] { 200 }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Threshold|int" }; }
        }

        private static float  MaxBgr = float.MaxValue;

        protected override object doEffect(ImageViewer.DataEnvironment dataEnvironment, object SourceImage, ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            double color = EffectHelps.ConvertToDouble(mFilterToken[0]);

            double ThresholdV = color;
            Bitmap holding = null;
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                holding = ImagingTools.ThresholdImage((Bitmap)SourceImage, (int)ThresholdV);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                ImageHolder ih = ((ImageHolder)SourceImage);
                ih.ThresholdBinaryWhite((float)ThresholdV);
                holding = ih.ToBitmap();
            }
            return holding;
        }

        public static  ImageHolder Threshold(ImageHolder SourceImage, float ThresholdV)
        {
            ImageHolder ih = ((ImageHolder)SourceImage);
            ih.ThresholdBinaryWhite((float)ThresholdV);
            return ih;
        }

        public static  Bitmap Threshold(Bitmap SourceImage, int ThresholdV)
        {
            Bitmap holding = ImagingTools.ThresholdImage(SourceImage, ThresholdV);
            return holding;
        }

    }
}
