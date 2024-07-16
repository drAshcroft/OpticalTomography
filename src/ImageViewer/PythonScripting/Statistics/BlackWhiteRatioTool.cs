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

namespace ImageViewer.PythonScripting.Statistics
{
    public class BlackWhiteRatioTool : aEffectNoForm
    {
        public override string EffectName { get { return "Get Ratio of Black Versus White"; } }
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

            mPassData.AddSafe("PercentBlack", PercentImage(mFilterToken[0]));

            return SourceImage;
        }



        private double PercentImage(object image)
        {
            double[,] nData = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(image, false);

            double sumWhite = 0, d;
            double sumBlack = 0;
            for (int i = 0; i < nData.GetLength(0); i+=8)
            {
                for (int j = 0; j < nData.GetLength(1); j+=8)
                {
                    d = (nData[i, j]);
                    if (d > 50)
                        sumWhite++;
                    else
                        sumBlack++;
                }
            }

            return sumBlack / (sumWhite + sumBlack);
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { new Bitmap(1, 1) }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "ThresholdedImage|Imageholder" }; }
        }

    }
}
