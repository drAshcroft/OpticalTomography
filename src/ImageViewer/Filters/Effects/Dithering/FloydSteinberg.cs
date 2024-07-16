using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace ImageViewer.Filters.Effects.Dithering
{
    public class FloydSteinbergEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Floyd-Steinberg Dithering"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Dithering"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             ReplaceStringDictionary PassData, params object[] Parameters)
        {
            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply( EffectHelps.ConvertToBitmap( SourceImage) );
            FloydSteinbergDithering Filter = new FloydSteinbergDithering();
            return EffectHelps.FixImageFormat(Filter.Apply(holding));
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
