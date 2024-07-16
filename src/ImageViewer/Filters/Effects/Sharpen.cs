using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using MathHelpLib;



namespace ImageViewer.Filters.Effects
{
    public class SharpenEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Sharpen"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            double [,] kernalF = new double [,] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
            MathHelpLib.Convolution.ValueArrayKernal vak = new MathHelpLib.Convolution.ValueArrayKernal(kernalF);
               
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                ImageHolder outI = new ImageHolder((Bitmap)SourceImage).Convolution(vak);
                return outI;
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                ImageHolder outI = ((ImageHolder)SourceImage).Convolution(vak );
                return outI;
            }
           
            return null;
        }


        public override object[] DefaultProperties
        {
            get { return null; }
        }
        public override string[] ParameterList
        {
            get { return new string[] { "" }; }
        }
    }
}
