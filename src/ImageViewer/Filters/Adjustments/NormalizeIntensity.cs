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

namespace ImageViewer.Filters.Adjustments
{
    public class NormalizeIntensityEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Normalize Intensity by Average"; } }
        public override string EffectMenu { get { return "Adjustment"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 12; } }

        /// <summary>
        ///  Attempts to normalize the intensity of an image by it's average intensity.  If a target is not provided, it will just divide by the average
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">The target value(ushort)</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            if (Parameters == null || Parameters[0] == null)
                Parameters = DefaultProperties;
            //ImageHolder bOut = SourceImage.CopyBlank();
            double NormTarget = EffectHelps.ConvertToDouble(Parameters[0]);

            ImageHolder tImage=null;
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                tImage= new ImageHolder((Bitmap)SourceImage);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                tImage = ((ImageHolder)SourceImage);
            }

            float AverageVal = (tImage).GetAverage();
            double convert = NormTarget / AverageVal;

            //normalize with the average
            return (tImage / convert);
        }

        /// <summary>
        /// Attempts to normalize the intensity of an image by it's average intensity.  If a target is not provided, it will just divide by the average
        /// </summary>
        /// <param name="image"></param>
        /// <param name="NormalizeToTarget"></param>
        /// <param name="Target"></param>
        /// <returns></returns>
        public static ImageHolder NormalizeImageByAverage(ImageHolder image, bool NormalizeToTarget, double Target)
        {

            float AverageVal = (image).GetAverage();
            if (NormalizeToTarget)
            {
                double convert = Target / AverageVal;

                //normalize with the average
                return (image / convert);
            }
            else
            {
                return image / AverageVal;
            }
        }

        /// <summary>
        ///  Attempts to normalize the intensity of an image by it's average intensity.  If a target is not provided, it will just divide by the average
        /// </summary>
        /// <param name="image"></param>
        /// <param name="NormalizeToTarget"></param>
        /// <param name="Target"></param>
        /// <returns></returns>
        public static ImageHolder NormalizeImageByAverage(Bitmap image, bool NormalizeToTarget, double Target)
        {
            ImageHolder tImage = new ImageHolder((Bitmap)image);
            float AverageVal = (tImage).GetAverage();
            if (NormalizeToTarget)
            {
                double convert = Target / AverageVal;

                //normalize with the average
                return (tImage / convert);
            }
            else
            {
                return tImage / AverageVal;
            }
        }


        public override object[] DefaultProperties
        {
            get { return new object[]{ 255 }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[]{ "Normalization_Target|ushort"}; }
        }

    }
}
