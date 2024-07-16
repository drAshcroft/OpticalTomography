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

namespace ImageViewer.PythonScripting.Programming_Tools
{
    public class ClipImageToNewEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Clip image to New image"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Programming"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 45;
            }
        }

        
        public override bool PassesPassData
        {
            get
            {
                return false;
            }
        }

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            mDataEnvironment = dataEnvironment;
            if (Parameters == null)
            {
                mFilterToken = DefaultProperties ;
            }

            Rectangle clippingRegion;
            if (mFilterToken[0] is Rectangle)
            {
                clippingRegion = (Rectangle)mFilterToken[0];

                if (SourceImage.GetType() == typeof(Bitmap))
                {
                    return EffectHelps.FixImageFormat( ImagingTools.ClipImageExactCopy((Bitmap)SourceImage, clippingRegion));
                }
                else if (SourceImage.GetType() == typeof(ImageHolder))
                {
                    return ImagingTools.ClipImageExactCopy((ImageHolder)SourceImage , clippingRegion,0);
                }
               
                return SourceImage ;
            }
            else
                return SourceImage ;
        }

        public static Bitmap ClipImage(Bitmap SourceImage, Rectangle ClipRectangle)
        {
            return ImagingTools.ClipImageExactCopy(SourceImage, ClipRectangle);
        }

        public static ImageHolder ClipImage(ImageHolder SourceImage, Rectangle ClipRectangle)
        {
            return ImagingTools.ClipImageExactCopy(SourceImage, ClipRectangle, 0);
        }

        public static double[,] ClipImage(double[,] SourceImage, Rectangle ClipRectangle)
        {
            return ImagingTools.ClipImageExactCopy(SourceImage, ClipRectangle);
        }

        public override object[] DefaultProperties
        {
            get 
            { 
                //poor mans clone
                return new object[] 
                   { Rectangle.Inflate( mDataEnvironment.Screen.ScreenCoords,0,0) }; 
            }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "Clip Bounds|Rectangle" }; }
        }
        



    }
}
