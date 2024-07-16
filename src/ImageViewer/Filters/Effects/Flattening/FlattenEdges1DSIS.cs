using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using MathHelpLib;

namespace ImageViewer.Filters.Effects.Flattening
{
    public class FlattenEdges1DSIS : aEffectNoForm
    {
        public override string EffectName { get { return "Flatten Edges 1D SIS Mask"; } }
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
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "" }; }
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
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                return FlattenImageEdges((Bitmap)SourceImage);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                return FlattenImageEdges((ImageHolder)SourceImage);
            }
            return null;
        }

       
        public static ImageHolder FlattenImageEdges(Bitmap SourceImage)
        {
            double[,] Image = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false);
            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(SourceImage);
            AForge.Imaging.Filters.SISThreshold Filter = new SISThreshold();
            Filter.ApplyInPlace (holding );
            double[,] Mask = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(holding, false);
            return new ImageHolder(FlattenEdges1DIterative.FlattenImageEdges(Image, Mask));
        }
        public static ImageHolder FlattenImageEdges(ImageHolder  SourceImage)
        {
            double[,] Image = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false);
            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(EffectHelps.ConvertToBitmap(SourceImage));
            AForge.Imaging.Filters.SISThreshold Filter = new SISThreshold();
            Filter.ApplyInPlace (holding );
            double[,] Mask = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(holding, false);
            return new ImageHolder(FlattenEdges1DIterative.FlattenImageEdges(Image, Mask));
        }

    }
}
