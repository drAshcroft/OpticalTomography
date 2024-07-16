using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using ImageViewer.Filters;

namespace ImageViewer3D.Filters.Edit
{
    public class CopyActiveImage : aEffectNoForm3D
    {
        public override string EffectName { get { return "Copy Active Image"; } }
        public override string EffectMenu { get { return "Edit"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 1; } }

        public override DataHolder DoEffect(DataEnvironment3D dataEnvironment, DataHolder SourceImage, ReplaceStringDictionary PassData, params object[] Parameters)
        {
            Bitmap FirstImage;
            if (dataEnvironment.ActiveSelection!=null && dataEnvironment.ActiveSelection.WindowIndex >= 0 && dataEnvironment.ActiveSelection.WindowIndex < 4)
                FirstImage = dataEnvironment.Screens[dataEnvironment.ActiveSelection.WindowIndex].ScreenBackBuffer;
            else
                FirstImage = dataEnvironment.Screens[0].ScreenBackBuffer;

            Clipboard.SetImage(FirstImage);
            return SourceImage;
        }
        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "" }; }
        }



    }
}
