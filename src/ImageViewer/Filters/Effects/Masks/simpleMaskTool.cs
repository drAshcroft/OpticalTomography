using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using MathHelpLib;

namespace ImageViewer.Filters.Masks
{
    public partial class SimpleThresholdTool : aEffectForm
    {
        public SimpleThresholdTool():base()
        {
            SetParameters(new string[] { "Threshold" }, new int[] { 0 }, new int[] { 255 });
        }
        public override string EffectName { get { return "Threshold Mask"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Masks"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
        public override object[] DefaultProperties
        {
            get { return new object[] {200 }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "Threshold|int" }; }
        }

        /// <summary>
        /// Takes a pixel value.  Everything above that is the original image, and everything below that is black
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        protected override object doEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;

           ///make a copy so we can manipulate the image
            Bitmap source = EffectHelps.ConvertToBitmap(SourceImage);
            //get the threshold
            Bitmap  b = ImagingTools.ThresholdImage(source, (int)mFilterToken[0]);
            

         
            Bitmap bOut = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppRgb);

         
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            BitmapData bmDataSource = source.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            byte Intensity;
            unsafe
            {
                for (int y = 0; y < b.Height; ++y)
                {
                    byte* p = (byte*)(void*)bmData.Scan0 + bmData.Stride * y;
                    byte* ps = (byte*)(void*)bmDataSource.Scan0 + bmDataSource.Stride * y;
                    byte* pOut = (byte*)(void*)bmDataOut.Scan0 + bmDataOut.Stride * y;
                    for (int x = 0; x < b.Width; ++x)
                    {
                        ///where the threshold is black, pass the image through to the new image
                        Intensity = p[0];
                        if (Intensity == 0)
                        {
                            pOut[0] = ps[0];
                            pOut[1] = ps[1];
                            pOut[2] = ps[2];
                            pOut[3] = (byte)(ps[3]);
                        }
                        else
                        {
                            pOut[0] = 0;
                            pOut[1] = 0;
                            pOut[2] = 0;
                            pOut[3] = (byte)(ps[3]);
                        }
                        p += 4;
                        pOut += 4;
                        ps += 4;
                    }
                }
            }

            b.UnlockBits(bmData);
            bOut.UnlockBits(bmDataOut);
            source.UnlockBits(bmDataSource);
            // Graphics.FromImage(bOut).Clear(Color.Blue);
            return new ImageHolder(bOut);
        }

        /// <summary>
        /// Thresholds an image, then returns those parts of the image that are brighter than the threshold
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Threshold"></param>
        /// <returns></returns>
        private static ImageHolder SimpleThresholdMask(ImageHolder Image, int Threshold)
        {
            ///make a copy so we can manipulate the image
            Bitmap source = Image.ToBitmap();
            //get the threshold
            Bitmap b = ImagingTools.ThresholdImage(source, Threshold  );

            Bitmap bOut = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppRgb);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            BitmapData bmDataSource = source.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            byte Intensity;
            unsafe
            {
                for (int y = 0; y < b.Height; ++y)
                {
                    byte* p = (byte*)(void*)bmData.Scan0 + bmData.Stride * y;
                    byte* ps = (byte*)(void*)bmDataSource.Scan0 + bmDataSource.Stride * y;
                    byte* pOut = (byte*)(void*)bmDataOut.Scan0 + bmDataOut.Stride * y;
                    for (int x = 0; x < b.Width; ++x)
                    {
                        Intensity = p[0];
                        ///where the threshold is black, pass the image through to the new image
                        if (Intensity == 0)
                        {
                            pOut[0] = ps[0];
                            pOut[1] = ps[1];
                            pOut[2] = ps[2];
                            pOut[3] = (byte)(ps[3]);
                        }
                        else
                        {
                            pOut[0] = 0;
                            pOut[1] = 0;
                            pOut[2] = 0;
                            pOut[3] = (byte)(ps[3]);
                        }
                        p += 4;
                        pOut += 4;
                        ps += 4;
                    }
                }
            }

            b.UnlockBits(bmData);
            bOut.UnlockBits(bmDataOut);
            source.UnlockBits(bmDataSource);
            // Graphics.FromImage(bOut).Clear(Color.Blue);
            return new ImageHolder(bOut);
        }

        /// <summary>
        /// Thresholds an image, then returns those parts of the image that are brighter than the threshold
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Threshold"></param>
        /// <returns></returns>
        private static Bitmap SimpleThresholdMask(Bitmap source, int Threshold)
        {
            ///make a copy so we can manipulate the image
            Bitmap b = ImagingTools.ThresholdImage(source, Threshold);

            //get the threshold
            Bitmap bOut = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppRgb);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            BitmapData bmDataSource = source.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            byte Intensity;
            unsafe
            {
                for (int y = 0; y < b.Height; ++y)
                {
                    byte* p = (byte*)(void*)bmData.Scan0 + bmData.Stride * y;
                    byte* ps = (byte*)(void*)bmDataSource.Scan0 + bmDataSource.Stride * y;
                    byte* pOut = (byte*)(void*)bmDataOut.Scan0 + bmDataOut.Stride * y;
                    for (int x = 0; x < b.Width; ++x)
                    {
                        Intensity = p[0];
                        ///where the threshold is black, pass the image through to the new image
                        if (Intensity == 0)
                        {
                            pOut[0] = ps[0];
                            pOut[1] = ps[1];
                            pOut[2] = ps[2];
                            pOut[3] = (byte)(ps[3]);
                        }
                        else
                        {
                            pOut[0] = 0;
                            pOut[1] = 0;
                            pOut[2] = 0;
                            pOut[3] = (byte)(ps[3]);
                        }
                        p += 4;
                        pOut += 4;
                        ps += 4;
                    }
                }
            }

            b.UnlockBits(bmData);
            bOut.UnlockBits(bmDataOut);
            source.UnlockBits(bmDataSource);
            // Graphics.FromImage(bOut).Clear(Color.Blue);
            return (bOut);
        }
        
    }
}
