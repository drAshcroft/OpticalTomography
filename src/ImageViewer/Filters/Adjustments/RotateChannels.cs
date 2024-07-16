using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace ImageViewer.Filters.Adjustments
{
    public class RotateChannelsEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Rotate Color Channels"; } }
        public override string EffectMenu { get { return "Adjustment"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 30; } }

        public override Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> DoEffect(
        DataEnvironment dataEnvironment, Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> SourceImage,
        Dictionary<string, object> PassData,
        params object[] Parameters)
        {
            RotateChannels Filter = new RotateChannels();
            return EffectHelps.FixImageFormat(Filter.Apply(SourceImage));
        
        }
        public override object[] DefaultProperties
        {
            get { return new object[] { null }; }
        }

        public override string ParameterList
        {
            get { return new string[] { "|" }; }
        }
           
    }
}
