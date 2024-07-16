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
using MathHelpLib;

namespace ImageViewer.PythonScripting.Statistics
{
    public class FocusValueTool : aEffectNoForm
    {
        public override string EffectName { get { return "Focus Value of Image"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Statistics"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        public override bool PassesPassData
        {
            get
            {
                return false;
            }
        }
        private static object CriticalSectionLock = new object();

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            mPassData = PassData;
            if (mFilterToken == null)
                mFilterToken = DefaultProperties;

            //try
            {
                ImageHolder GrayImage = null;
               
                if (SourceImage.GetType() == typeof(Bitmap))
                {
                    GrayImage = (new ImageHolder((Bitmap)SourceImage));
                }
                else if (SourceImage.GetType() == typeof(ImageHolder))
                {
                    GrayImage = ((ImageHolder)SourceImage);
                    GrayImage.ConvertToGrayScaleAverage();
                }
                double ave = GrayImage.FocusValue();
                mPassData.AddSafe("FocusValue", ave);
            }
            //catch (Exception Exception)
            {
              //  mPassData.AddSafe("FocusValue", 0);
              //  System.Diagnostics.Debug.Print(Exception.Message);
            }
            return SourceImage;
        }


        public override object[] DefaultProperties
        {
            get { return new object[] { new Bitmap(1, 1) }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Image|image" }; }
        }

    }
}
