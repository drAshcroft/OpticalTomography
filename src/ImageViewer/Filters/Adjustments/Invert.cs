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
    public class InvertEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Invert Contrast"; } }
        public override string EffectMenu { get { return "Adjustment"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 20; } }

        /// <summary>
        /// Inverts the colors in an image
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">no parameters</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage, ReplaceStringDictionary PassData, params object[] Parameters)
        {
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                return EffectHelps.FixImageFormat( ImagingTools.Invert((Bitmap)SourceImage) );
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                ImageHolder ih = ((ImageHolder)SourceImage);
                ih.Invert();
                return ih ;
            }
            return null;
        }

        /// <summary>
        /// inverts image in place
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static ImageHolder InvertImage(ImageHolder image)
        {
            image.Invert();
            return image;
        }

        /// <summary>
        /// inverts image and returns a new image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static ImageHolder InvertImage(Bitmap image)
        {
            return EffectHelps.FixImageFormat(ImagingTools.Invert((Bitmap)image));
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
