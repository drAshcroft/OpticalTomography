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
    public class SetRectangleTool : aEffectNoForm
    {
        public override string EffectName { get { return "Set Rectangle Size"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Programming"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 10;
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
            try
            {
                mFilterToken = Parameters;
                Rectangle orig = (Rectangle)mFilterToken[0];

                if (mFilterToken == null)
                    mFilterToken = DefaultProperties;
                try
                {
                    Size target = (Size)mFilterToken[0];
                    int centerX = (int)(orig.Left + (double)orig.Width / 2d);
                    int centerY = (int)(orig.Top + (double)orig.Height / 2d);
                    int HalfX = (int)((double)target.Width / 2d);
                    int HalfY = (int)((double)target.Height / 2d);
                    mPassData.AddSafe("Bounds", Rectangle.FromLTRB(centerX - HalfX, centerY - HalfY, centerX + HalfX, centerY + HalfY));
                }
                catch
                {
                    mPassData.AddSafe("Bounds", (Size)DefaultProperties[0]);
                }
            }
            catch { }
            return SourceImage;
        }
        public override object[] DefaultProperties
        {
            get { return new object[] { new Size(100, 100) }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Rectangle_Size|Size" }; }
        }



    }
}
