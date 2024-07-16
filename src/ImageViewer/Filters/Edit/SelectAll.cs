using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace ImageViewer.Filters.Edit
{
    public class SelectAll
        : aEffectNoForm
    {
        public override string EffectName { get { return "Select All"; } }
        public override string EffectMenu { get { return "Edit"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 20; } }

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
                      ReplaceStringDictionary PassData, params object[] Parameters)
        {
            ImageViewer.Tools.ROITool roiT = new ImageViewer.Tools.ROITool();
            dataEnvironment.Screen .SimulateROISelection(roiT,
                new Rectangle(0, 0, dataEnvironment.Screen.ScreenBackBuffer.Width, dataEnvironment.Screen.ScreenBackBuffer.Height));
            
            return SourceImage;
        }
        public override object[] DefaultProperties
        {
            get { return new object[] { null }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }
        
    }
}
