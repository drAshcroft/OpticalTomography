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
    public class GrayScaleEffectChannel : aEffectNoForm
    {
        public override string EffectName { get { return "Convert to Grayscale by Channel"; } }
        public override string EffectMenu { get { return "Adjustment"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 20; } }

        /// <summary>
        /// Pulls one channel from an image to make a grayscale.  Red is 0, Green is 1, and Blue is 2
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">color channel that is desired</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {

            int Channel = (int)Parameters[0];
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                ImageHolder ih = (new ImageHolder((Bitmap)SourceImage));
                ih.ConvertToGrayScaleChannel(Channel);
                return ih;
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                ((ImageHolder)SourceImage).ConvertToGrayScaleChannel(Channel);
                return SourceImage;
            }
            return null;
        }

        /// <summary>
        /// Pulls one channel from an image to make a grayscale.  Red is 0, Green is 1, and Blue is 2
        /// </summary>
        /// <param name="image"></param>
        /// <param name="Channel"> Red is 0, Green is 1, and Blue is 2</param>
        /// <returns></returns>
        public static ImageHolder GrayScaleFromChannel(ImageHolder image, int Channel)
        {
            image.ConvertToGrayScaleChannel(Channel);
            return image;
        }

        /// <summary>
        ///  Pulls one channel from an image to make a grayscale.  Red is 0, Green is 1, and Blue is 2
        /// </summary>
        /// <param name="image"></param>
        /// <param name="Channel"> Red is 0, Green is 1, and Blue is 2</param>
        /// <returns></returns>
        public static ImageHolder GrayScaleFromChannel(Bitmap  image, int Channel)
        {
            ImageHolder ih = (new ImageHolder(image));
            ih.ConvertToGrayScaleChannel(Channel);
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
