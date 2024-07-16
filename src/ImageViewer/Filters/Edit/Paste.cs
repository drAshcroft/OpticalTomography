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
    public class Paste:Copy 
    {
        public override string EffectName
        {
            get
            {
                return "Paste";
            }
        }
        public override int OrderSuggestion
        {
            get
            {
                return 10;
            }
        }

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
              ReplaceStringDictionary PassData, params object[] Parameters)
        {
            try
            {
                return new ImageHolder((Bitmap)Clipboard.GetImage());
            }
            catch
            {
                return null;
            }
        }
    }
}
