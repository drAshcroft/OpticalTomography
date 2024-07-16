using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using MathHelpLib;

namespace ImageViewer.Filters.Effects.Flattening
{
    public class FlattenEdges : aEffectNoForm
    {
        public override string EffectName { get { return "Flatten Edges"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Flattening"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
        public override object[] DefaultProperties
        {
            get { return null ; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { ""}; }
        }

        /// <summary>
        /// Samples all the points on the edge of an image and then subtracts a place obtained by linear regression.
        /// This will create a mostly flat edge 
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">No Parameters</param>
        /// <returns></returns>
        public override object  DoEffect(DataEnvironment dataEnvironment, object SourceImage, 
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                return MathHelpLib.ImageProcessing.MathImageHelps.FlattenImageEdges((Bitmap)SourceImage);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                return MathHelpLib.ImageProcessing.MathImageHelps.FlattenImageEdges((ImageHolder)SourceImage);
            }
            return null;
        }

        public static ImageHolder FlattenImageEdges(ImageHolder SourceImage)
        {
            return MathHelpLib.ImageProcessing.MathImageHelps.FlattenImageEdges(SourceImage);
        }

        public static Bitmap FlattenImageEdges(Bitmap SourceImage)
        {
            return MathHelpLib.ImageProcessing.MathImageHelps.FlattenImageEdges(SourceImage);
        }
    }
}
