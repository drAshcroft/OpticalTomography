using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace ImageViewer.Filters.Edit
{
    public class Copy
        : aEffectNoForm
    {
        public override string EffectName { get { return "Copy"; } }
        public override string EffectMenu { get { return "Edit"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 1; } }

        public override object   DoEffect(DataEnvironment dataEnvironment, object SourceImage, 
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            //todo: need to provide a custom clipboard format for the data to preserve the 16 bit information
            Clipboard.SetImage( EffectHelps.ConvertToBitmap( SourceImage) );
            return SourceImage;
        }
        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] {""}; }
        }
        
    }
}
