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
    public class FastBlackWhiteRatioTool : aEffectNoForm
    {
        public override string EffectName { get { return "Get approximate Ratio of Black Versus White"; } }
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


        public static double PercentImage(object Image)
        {
            if (Image.GetType() == typeof(Bitmap))
            {
                return PercentImage((Bitmap)Image);
            }
            else if (Image.GetType() == typeof(ImageHolder))
            {
                return PercentImage((ImageHolder)Image);
            }

            return 1;
        }

        public static double PercentImage(ImageHolder Image)
        {
            double sumWhite = 0;
            double sumBlack = 0;
           

            int iWidth = Image.Width;
            int iHeight = Image.Height;

            int NChannels = Image.NChannels;

           
                unsafe
                {
                    fixed (float* pFromBase = Image.ImageData)
                    {
                        float* pFrom = pFromBase;
                        int Length = Image.ImageData.Length;
                        int Step = 1;
                        if (Length > 200)
                            Step = (int)Math.Truncate(Length / 200d);
                        for (int i = 0; i < Length; i += Step)
                        {
                            if (*pFrom < 50)
                                sumBlack++;
                            else
                                sumWhite++;
                            pFrom += Step;
                        }

                    }
                }

                return sumBlack / (sumBlack + sumWhite);
        }

        public static double PercentImage(Bitmap Image)
        {
            int iWidth = Image.Width;
            int iHeight = Image.Height;
            double sumWhite = 0;
            double sumBlack = 0;
           
            BitmapData bmd = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), ImageLockMode.WriteOnly, Image.PixelFormat);

            double g1, g2, g3;
            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = 0; y < iHeight; y+=25)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x+=25)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];

                            if (g1<50)
                                sumBlack++;
                            else
                                sumWhite++;
                            scanline++;
                        }
                    }
                }
                else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    for (int y = 0; y < iHeight; y+=25)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x+=25)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            if (g1 < 50)
                                sumBlack++;
                            else
                                sumWhite++;
                            scanline += 3;
                        }
                    }
                }
                else if (bmd.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    for (int y = 0; y < iHeight; y += 25)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x += 25)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            if (g1 < 50)
                                sumBlack++;
                            else
                                sumWhite++;
                            scanline++;
                        }
                    }
                }
            }
            Image.UnlockBits(bmd);
            return sumBlack / (sumBlack + sumWhite);
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
