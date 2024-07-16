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

namespace ImageViewer.Filters.Effects
{
    public partial class PercentPixelsThresholdTool : aEffectForm
    {
        public PercentPixelsThresholdTool():base()
        {
            SetParameters(new string[] { "Percent Of Pixels" }, new int[] { 0 }, new int[] { 100 });
        }
        public override string EffectName { get { return "Percent Pixels Threshold Image"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Thresholding"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
        public override object[] DefaultProperties
        {
            get { return new object[] { 25 }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "Percent_Pixels_Black|int" }; }
        }

        public override object DoEffect(ImageViewer.DataEnvironment dataEnvironment, object SourceImage, System.Collections.Generic.Dictionary<string, object> PassData, params object[] Parameters)
        {
            return base.DoEffect(dataEnvironment, SourceImage, PassData, Parameters);
        }
       
    }
}
