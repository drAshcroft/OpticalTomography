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

namespace ImageViewer.Filters.Effects.Artistic
{
    public class JitterEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Jitter"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Artistic"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
        /// <summary>
        /// Nice artistic effect that adds noise to an image
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">No Parameters</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            Jitter Filter = new Jitter();
            return EffectHelps.FixImageFormat(Filter.Apply(EffectHelps.ConvertToBitmap ( SourceImage) ));
        }

        public static ImageHolder Jitter(ImageHolder SourceImage)
        {
            Jitter Filter = new Jitter();
            return EffectHelps.FixImageFormat(Filter.Apply(EffectHelps.ConvertToBitmap(SourceImage)));
        }

        public static Bitmap Jitter(Bitmap SourceImage)
        {
            Jitter Filter = new Jitter();
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
