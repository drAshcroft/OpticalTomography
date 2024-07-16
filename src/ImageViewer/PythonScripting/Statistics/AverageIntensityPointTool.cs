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

namespace ImageViewer.PythonScripting.AverageImage
{
    public class AverageIntensityPointTool : aEffectNoForm
    {
        public override string EffectName { get { return "Average Intensity of Image"; } }
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
                if (mFilterToken[0].GetType() == typeof(Rectangle))
                {
                    mPassData.AddSafe("AverageIntensity", AverageDoubleArray(mFilterToken[1], (Rectangle)mFilterToken[0]));
                }
                else
                {
                    mPassData.AddSafe("AverageIntensity", AverageImage(mFilterToken[0]));
                }
            }
            //catch (Exception Exception)
            {
              //  System.Diagnostics.Debug.Print(Exception.Message);
            }
            return SourceImage;
        }

       

        public double AverageDoubleArray(object b, Rectangle Bounds)
        {
            if (Bounds.X<0)
            {
                Bounds.Height += Bounds.X;
                Bounds.X = 0;
            }
            if (Bounds.Y < 0)
            {
                Bounds.Width += Bounds.Y;
                Bounds.Y = 0;
            }
            
            if (b.GetType() == typeof(Bitmap))
            {

                return AverageDoubleArrayP((Bitmap)b, Bounds);
            }
            else if (b.GetType() == typeof(ImageHolder))
            {
                return AverageDoubleArrayP((ImageHolder)b, Bounds);
            }
            
            return 1000;
        }

        private double AverageDoubleArrayP(ImageHolder b, Rectangle Bounds)
        {
            float [, ,] bmd = b.ImageData;
            int cX = 0, cY = 0;
            double d;
            double sum = 0;
            double cc = 0;
                                try
                    {

            for (int x = Bounds.X; x < Bounds.Right; x++)
            {
                cY = 0;
                for (int y = Bounds.Y; y < Bounds.Bottom; y++)
                {
                        sum += bmd[y, x, 0];
                        cc++;
                        cY++;
                }
                cX++;
            }
                    }
                                catch
                                {
                                }

            sum = sum / (cc);
            return sum;
        }

        private double AverageDoubleArrayP(Bitmap b, Rectangle Bounds)
        {

            //double[,] ImageArray = new double[Image.Width, Image.Height];

            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

            double sum = 0, d;
            double g1, g2, g3;
            unsafe
            {
                for (int x = 0; x < Bounds.Width; x++)
                {
                    for (int y = 0; y < Bounds.Height; y++)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y + Bounds.Top) * bmd.Stride) + Bounds.Left + x;

                        byte* bits = (byte*)scanline;
                        g1 = bits[0];
                        g2 = bits[1];
                        g3 = bits[2];

                        d = ((g1 + g2 + g3) / 3d);
                        sum += d;
                    }
                }
            }
            b.UnlockBits(bmd);
            sum = sum / (Bounds.Width * Bounds.Height);
            return sum;
        }

        private double AverageImage(object image)
        {
            double[,] nData = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(image, false);

            double sum = 0, d;
            for (int i = 0; i < nData.GetLength(0); i++)
            {
                for (int j = 0; j < nData.GetLength(1); j++)
                {
                    d = (nData[i, j]);
                    sum += d;
                }
            }

            return sum / (nData.GetLength(0) * nData.GetLength(1));
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { new Bitmap(1, 1), new Rectangle(0, 0, 2, 2), new Bitmap(1, 1) }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "AverageArray|string", "CompareImage or Bounds|Image or Rectange", "CompareImage|Image" }; }
        }

    }
}
