using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace ImageViewer.Filters.Effects.Artistic
{
    public class OilPaintingEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Oil Paint"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Artistic"; } }
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
            OilPainting Filter = new OilPainting();
            return EffectHelps.FixImageFormat(Filter.Apply(EffectHelps.ConvertToBitmap( SourceImage) ));
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
