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

namespace ImageViewer.PythonScripting.Programming_Tools
{
    public class RectangleToSelection
        : aEffectNoForm
    {
        public override string EffectName { get { return "Rectangle To Selection"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Programming"; } }
        public override int OrderSuggestion { get { return 30; } }

        
        public override bool PassesPassData
        {
            get
            {
                return false ;
            }
        }

        public override Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> DoEffect(
        DataEnvironment dataEnvironment, Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> SourceImage,
        Dictionary<string, object> PassData,
        params object[] Parameters)
        {
            Rectangle rect = (Rectangle)mPassData;
            for (int i = 0; i < SourceImage.Length; i++)
            {
                ImageViewer.Tools.ROITool roiT = new ImageViewer.Tools.ROITool();
                SourceImage[i].SimulateROISelection(roiT, rect);
            }
            string outstring =
@"#Edit Function
Filter = " + this.GetType().ToString() + @"() 
holding=Filter.RunEffect(holding1,None) ";
            mFilterToken = FilterToken;
            return SourceImage;
        }
        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "" }; }
        }
       
        
    }
}
