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
    public class StandardDeviationPointAverageTool : aEffectNoForm
    {
        public override string EffectName { get { return "Standard Deviation of Image"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Average Images"; } }
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

        /// <summary>
        /// performs the standard deviation of an image against an average image
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            mPassData = PassData;
            if (mFilterToken == null)
                mFilterToken = DefaultProperties;

            double[,] Data = (double[,])mFilterToken[0];
           // try
            {
                if (mFilterToken[1].GetType() == typeof(Rectangle))
                {
                    mPassData.AddSafe("StdDev", StdDevPartImage(Data, (Rectangle)mFilterToken[1], mFilterToken[2]));
                }
                else
                {
                    mPassData.AddSafe("StdDev",StdDevWholeImage(Data, mFilterToken[1]));
                }
            }
           // catch (Exception Exception)
            {
            //    mPassData.AddSafe("StdDev", 0);
             //   System.Diagnostics.Debug.Print(Exception.Message);
            }
            return SourceImage;
        }

        private object StdDevPartImage(double[,] Data, Rectangle Bounds, object image)
        {
            Rectangle imageBounds = ImagingTools.GetImageBounds(image);
            if (Bounds.Width > imageBounds.Width)
                Bounds.Width = imageBounds.Width;
            if (Bounds.Height > imageBounds.Height)
                Bounds.Height = imageBounds.Height;
            return StdDevToDoubleArray(image, ref Data, Bounds);
        }

        public double StdDevToDoubleArray(object b, ref double[,] AverageData, Rectangle Bounds)
        {
            if (b.GetType() == typeof(Bitmap))
            {
                return StdDevToDoubleArray((Bitmap)b, ref AverageData, Bounds);
            }
            else if (b.GetType() == typeof(ImageHolder))
            {
                return StdDevToDoubleArray((ImageHolder)b, ref AverageData, Bounds);
            }
            else if (b.GetType() == typeof(ImageHolder))
            {
                return StdDevToDoubleArray((ImageHolder)b, ref AverageData, Bounds);
            }
            return 1000;
        }

        public double StdDevToDoubleArray(ImageHolder b, ref double[,] ExistData, Rectangle Bounds)
        {
            //double[,] ImageArray = new double[Image.Height, Image.Width];

            float [, ,] bmd = b.ImageData;
            int cX = 0, cY = 0;
            double d;
            double sum = 0;

            for (int x = Bounds.X; x < Bounds.Right; x++)
            {
                cY = 0;
                for (int y = Bounds.Y; y < Bounds.Bottom; y++)
                {
                    try
                    {
                        d = ExistData[cY, cX] - bmd[y, x, 0];
                        sum += (d * d);
                    }
                    catch { }
                    cY++;
                }
                cX++;
            }
            sum = Math.Sqrt(sum / (Bounds.Width * Bounds.Height));
            return sum;
        }


        public double StdDevToDoubleArray(Bitmap b, ref double[,] ExistData, Rectangle Bounds)
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
                        try
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y + Bounds.Top) * bmd.Stride) + Bounds.Left + x;

                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            d = (ExistData[y, x] - (g1 + g2 + g3) / 3d);

                            sum += d * d;
                        }
                        catch { }
                    }
                }
            }
            b.UnlockBits(bmd);
            sum = Math.Sqrt(sum / (Bounds.Width * Bounds.Height));
            return sum;
        }


        private double StdDevWholeImage(double[,] data, object image)
        {
            double[,] nData = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(image, false);

            double sum = 0, d;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    d = (data[i, j] - nData[i, j]);
                    sum += d * d;
                }
            }

            return Math.Sqrt(sum / (data.GetLength(0) * data.GetLength(1)));
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
