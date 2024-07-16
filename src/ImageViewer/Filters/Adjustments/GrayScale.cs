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
    public class GrayScaleEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Convert to Grayscale"; } }
        public override string EffectMenu { get { return "Adjustment"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 20; } }

        /// <summary>
        /// Takes color image and makes a grayscale image
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">no parameters</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                ImageHolder ih = (new ImageHolder((Bitmap)SourceImage));
                ih.ConvertToGrayScaleAverage();
                return ih;
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                ((ImageHolder)SourceImage).ConvertToGrayScaleAverage();
                return SourceImage;
            }
            return null;
        }

        public static ImageHolder GrayScaleImage(ImageHolder image)
        {
            image.ConvertToGrayScaleAverage();
            return image;
        }

        public static ImageHolder  GrayScaleImage(Bitmap image)
        {
            ImageHolder ih = (new ImageHolder((Bitmap)image));
            ih.ConvertToGrayScaleAverage();
            return ih;
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "" }; }
        }


    }
}
