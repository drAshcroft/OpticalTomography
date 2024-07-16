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
    public class CopyTopTwo : aEffectNoForm3D
    {
        public override string EffectName { get { return "Copy Top Two"; } }
        public override string EffectMenu { get { return "Edit"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 1; } }

        public override DataHolder DoEffect(DataEnvironment3D dataEnvironment, DataHolder SourceImage, ReplaceStringDictionary PassData, params object[] Parameters)
        {
            Bitmap FirstImage = dataEnvironment.Screens[0].ScreenBackBuffer;
            Bitmap SecImage = dataEnvironment.Screens[1].ScreenBackBuffer;

            Bitmap wholeImage = new Bitmap(2*FirstImage.Width, FirstImage.Height);
            Graphics g = Graphics.FromImage(wholeImage);
            g.DrawImage(FirstImage, new Point(0, 0));
            g.DrawImage(SecImage, new Point(FirstImage.Width, 0));
            g = null;
            Clipboard.SetImage(wholeImage);
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
