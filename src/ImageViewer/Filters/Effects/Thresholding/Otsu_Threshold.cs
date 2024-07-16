using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using MathHelpLib;

namespace ImageViewer.Filters.Thresholding
{
    public class OtsuThresholdEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Otsu Threshold"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Thresholding"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 15;
            }
        }

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
                    ReplaceStringDictionary PassData, params object[] Parameters)
        {
            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply( EffectHelps.ConvertToBitmap( SourceImage) );
            AForge.Imaging.Filters.OtsuThreshold  Filter = new OtsuThreshold ();
            //  Filter.ThresholdValue  = Filter.CalculateThreshold(holding , new Rectangle(0, 0, SourceImage.Width, SourceImage.Height));

            //return EffectHelps.FixImageFormat(Filter.Apply(holding));
            return Filter.Apply(holding);

        }

        public static ImageHolder OtsuThreshold(ImageHolder SourceImage)
        {
            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(EffectHelps.ConvertToBitmap(SourceImage));
            AForge.Imaging.Filters.OtsuThreshold Filter = new OtsuThreshold();
            // Filter.ThresholdValue  = Filter.CalculateThreshold(holding , new Rectangle(0, 0, SourceImage.Width, SourceImage.Height));

            return EffectHelps.FixImageFormat(Filter.Apply(holding));
        }

        public static Bitmap OtsuThreshold(Bitmap SourceImage)
        {
            Bitmap holding;
            if (SourceImage.PixelFormat==PixelFormat.Format8bppIndexed)
                holding=SourceImage;
            else 
                holding= Grayscale.CommonAlgorithms.RMY.Apply(SourceImage);
            AForge.Imaging.Filters.OtsuThreshold Filter = new OtsuThreshold();
            // Filter.ThresholdValue  = Filter.CalculateThreshold(holding , new Rectangle(0, 0, SourceImage.Width, SourceImage.Height));

            return Filter.Apply(SourceImage);
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { null }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }
            
           
        
    }
}
