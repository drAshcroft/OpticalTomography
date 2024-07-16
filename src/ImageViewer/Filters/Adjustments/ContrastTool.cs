using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MathHelpLib;

namespace ImageViewer.Filters.Adjustments
{
    public class ContrastTool : aEffectForm
    {
        public ContrastTool()
            : base()
        {
            SetParameters(new string[] { "Brightness", "Contrast" }, new int[] { 0, 0 }, new int[] { 100, 100 });
        }

        public override string EffectName { get { return "Contrast and Brightness"; } }
        public override string EffectMenu { get { return "Adjustment"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 1; } }


        public override object[] DefaultProperties
        {
            get { return new object[] { 0, 125 }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Contrast|int", "Brightness|int" }; }
        }

        /// <summary>
        /// takes the two values and passes them on to the environment display.  In this way the data can be adjusted non destructively.
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">Contrast and Brightness as values between 0 and 100</param>
        /// <returns></returns>
        protected override object doEffect(ImageViewer.DataEnvironment dataEnvironment, object SourceImage, ReplaceStringDictionary PassData, params object[] Parameters)
        {
            double MaxPossible = int.MaxValue;
            double MinPossible = int.MinValue;

            if (SourceImage.GetType() == typeof(Bitmap))
            {
                Bitmap b = (Bitmap)SourceImage;
                double[,] A = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(b, false);
                MaxPossible = A.MaxArray();
                MinPossible = A.MinArray();
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                ImageHolder b = (ImageHolder)SourceImage;
                double[,] A = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(b, false);
                MaxPossible = A.MaxArray();
                MinPossible = A.MinArray();
            }

            double Contrast = EffectHelps.ConvertToDouble(mFilterToken[0]) / 100d;
            double Brightness = EffectHelps.ConvertToDouble(mFilterToken[1]) / 100d;

            double MidPoint = (Brightness) * (MaxPossible - MinPossible) + MinPossible;
            double Width = (Contrast) * (MaxPossible - MinPossible);

            double minContrast = MidPoint - Width;
            double maxContrast = MidPoint + Width;

            if (minContrast < 0) minContrast = 0;
            if (maxContrast < 0) maxContrast = 0;
            if (minContrast > ushort.MaxValue) minContrast = ushort.MaxValue;
            if (maxContrast > ushort.MaxValue) maxContrast = ushort.MaxValue;


            dataEnvironment.MaxContrast = (ushort)(maxContrast);
            dataEnvironment.MinContrast = (ushort)(minContrast);
            pictureDisplay1.dataEnvironment.MaxContrast = (ushort)(maxContrast);
            pictureDisplay1.dataEnvironment.MinContrast = (ushort)(minContrast);
            pictureDisplay1.Invalidate();

            return SourceImage;
        }

        /// <summary>
        /// Adjusts the contrast and brightness of the DataEnvironment, based on the current image
        /// </summary>
        /// <param name="CurrentImage"></param>
        /// <param name="Contrast"></param>
        /// <param name="Brightness"></param>
        /// <param name="dataEnvironment"></param>
        public static void SetContrastAndBrightness(ImageHolder CurrentImage, double Contrast, double Brightness, DataEnvironment dataEnvironment)
        {
            double MaxPossible = int.MaxValue;
            double MinPossible = int.MinValue;



            double[,] A = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(CurrentImage, false);
            MaxPossible = A.MaxArray();
            MinPossible = A.MinArray();



            double MidPoint = (Brightness) * (MaxPossible - MinPossible) + MinPossible;
            double Width = (Contrast) * (MaxPossible - MinPossible);

            double minContrast = MidPoint - Width;
            double maxContrast = MidPoint + Width;

            if (minContrast < 0) minContrast = 0;
            if (maxContrast < 0) maxContrast = 0;
            if (minContrast > ushort.MaxValue) minContrast = ushort.MaxValue;
            if (maxContrast > ushort.MaxValue) maxContrast = ushort.MaxValue;


            dataEnvironment.MaxContrast = (ushort)(maxContrast);
            dataEnvironment.MinContrast = (ushort)(minContrast);

        }

        /// <summary>
        /// Adjusts the contrast and brightness of the DataEnvironment, based on the current image
        /// </summary>
        /// <param name="CurrentImage"></param>
        /// <param name="Contrast"></param>
        /// <param name="Brightness"></param>
        /// <param name="dataEnvironment"></param>
        public static void SetContrastAndBrightness(Bitmap CurrentImage, double Contrast, double Brightness, DataEnvironment dataEnvironment)
        {
            double MaxPossible = int.MaxValue;
            double MinPossible = int.MinValue;


            Bitmap b = CurrentImage;
            double[,] A = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(b, false);
            MaxPossible = A.MaxArray();
            MinPossible = A.MinArray();

            double MidPoint = (Brightness) * (MaxPossible - MinPossible) + MinPossible;
            double Width = (Contrast) * (MaxPossible - MinPossible);

            double minContrast = MidPoint - Width;
            double maxContrast = MidPoint + Width;

            if (minContrast < 0) minContrast = 0;
            if (maxContrast < 0) maxContrast = 0;
            if (minContrast > ushort.MaxValue) minContrast = ushort.MaxValue;
            if (maxContrast > ushort.MaxValue) maxContrast = ushort.MaxValue;


            dataEnvironment.MaxContrast = (ushort)(maxContrast);
            dataEnvironment.MinContrast = (ushort)(minContrast);
        }


    }
}
