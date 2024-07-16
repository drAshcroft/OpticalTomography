using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace ImageViewer.Filters.Adjustments
{
    public partial class RotateHueTool : aEffectForm  
    {
        public RotateHueTool():base()
        {
            SetParameters(new string[] { "Hue" }, new int[] { -100 }, new int[] { 100 });
        }
        public override string EffectName { get { return "Rotate Hue"; } }
        public override string EffectMenu { get { return "Adjustment"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 30; } }


        public override object[] DefaultProperties
        {
            get { return new object[] { 0 }; }
        }

        public override string ParameterList
        {
            get { return new string[] { "Hue_Change|int" }; }
        }

        protected override Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> doEffect(
        DataEnvironment dataEnvironment, Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> SourceImage,
        Dictionary<string, object> PassData,
        params object[] Parameters)
        {
            hue.Hue = (int)FilterToken.Parameters[0];
            Bitmap holding = new Bitmap(SourceImage.Width, SourceImage.Height, PixelFormat.Format24bppRgb);
            Graphics.FromImage(holding).DrawImage(SourceImage, 0, 0);
            return EffectHelps.FixImageFormat(hue.Apply(holding));
        }
       
        HueModifier hue = new HueModifier(0);
       
    }
}
