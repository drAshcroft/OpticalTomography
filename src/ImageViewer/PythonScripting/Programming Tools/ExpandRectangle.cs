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
    public class ExpandRectangleTool : aEffectNoForm
    {
        public override string EffectName { get { return "Expand Rectangle"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Programming"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 15;
            }
        }
       
        public override bool PassesPassData
        {
            get
            {
                return true;
            }
        }

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;

            if (mFilterToken == null)
                mFilterToken = DefaultProperties;
            
            try
            {
                Size target = (Size)mFilterToken[1];
                Rectangle orig = (Rectangle)mFilterToken[0];
                orig.Inflate(target);
                mPassData.AddSafe("Bounds", orig);
            }
            catch
            {
                
            }
            return null; 
        }
        public override object[] DefaultProperties
        {
            get { return new object[] { new Size(100,100)}; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "Expand_amount|Size" }; }
        }
       
    }
}
