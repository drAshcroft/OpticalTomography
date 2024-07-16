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
    public class DeleteSelection:Copy 
    {
        public override string EffectName
        {
            get
            {
                return "Delete Selection";
            }
        }
        public override int OrderSuggestion
        {
            get
            {
                return 35;
            }
        }

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
              ReplaceStringDictionary PassData, params object[] Parameters)
        {
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                return new ImageHolder(((Bitmap)SourceImage).Width, ((Bitmap)SourceImage).Height);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                ImageHolder ih=((ImageHolder)SourceImage);
                return new ImageHolder(ih.Width,ih.Height,ih.NChannels );
            }
            
            return null;
            
        }
    }
}
