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

namespace ImageViewer.Filters.Edit
{
    public class Cut : Copy
    {
        public override string EffectName
        {
            get
            {
                return "Cut";
            }
        }
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
            //first get a system bitmap
            Bitmap b = EffectHelps.ConvertToBitmap(SourceImage);
            Clipboard.SetImage(b);
            //then return a blank image
            return new ImageHolder(b.Width, b.Height);
        }
    }
}
