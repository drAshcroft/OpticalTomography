using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AForge.Imaging.Filters;
using MathHelpLib;

namespace ImageViewer.Filters.Circles
{
    public class HoughCircleFinder: aEffectNoForm
    {
        public override string EffectName { get { return "HoughCircleFinder"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Circles"; } }
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

        public static ImageHolder HoughCircle(ImageHolder SourceImage)
        {
            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(EffectHelps.ConvertToBitmap(SourceImage));
            AForge.Imaging.Filters.OtsuThreshold Filter = new OtsuThreshold();
            // Filter.ThresholdValue  = Filter.CalculateThreshold(holding , new Rectangle(0, 0, SourceImage.Width, SourceImage.Height));

            return EffectHelps.FixImageFormat(Filter.Apply(holding));
        }

        public static Bitmap HoughCircle(Bitmap SourceImage)
        {
            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(SourceImage);
            AForge.Imaging.Filters.OtsuThreshold Filter = new OtsuThreshold();
            // Filter.ThresholdValue  = Filter.CalculateThreshold(holding , new Rectangle(0, 0, SourceImage.Width, SourceImage.Height));

            return Filter.Apply(holding);
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
