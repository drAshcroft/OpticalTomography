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

namespace ImageViewer.Filters.Effects.EdgeDetection
{
    public class SobelEdgeDetectionEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Sobel Edge Detection"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Edge Detection"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        public static Bitmap SobelEdgeDetection(Bitmap SourceImage)
        {
            SobelEdgeDetector Filter = new SobelEdgeDetector();

            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(SourceImage);

            return Filter.Apply(holding);
        }

        public static ImageHolder SobelEdgeDetection(ImageHolder SourceImage)
        {
            SobelEdgeDetector Filter = new SobelEdgeDetector();

            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(EffectHelps.ConvertToBitmap(SourceImage));

            return EffectHelps.FixImageFormat(Filter.Apply(holding));
        }
        /// <summary>
        /// performs a difference edge detection.  
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">No Parameters</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
              ReplaceStringDictionary PassData, params object[] Parameters)
        {
            SobelEdgeDetector Filter = new SobelEdgeDetector();


            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(EffectHelps.ConvertToBitmap(SourceImage));

            return EffectHelps.FixImageFormat(Filter.Apply(holding));

        }
        public override object[] DefaultProperties
        {
            get { return new object[] { null }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }

    }
}
