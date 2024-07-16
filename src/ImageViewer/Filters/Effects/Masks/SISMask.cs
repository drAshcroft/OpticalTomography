using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace ImageViewer.Filters.Masks
{
    public class SISMaskEffect : aEffectNoForm
    {
        public override string EffectName { get { return "SIS Mask"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Masks"; } }



        /// <summary>
        /// Calcs a threshold with SIS method.  Everything above that is the original image, and everything below that is black
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            Bitmap bSource = EffectHelps.ConvertToBitmap(SourceImage);
            Bitmap b = Grayscale.CommonAlgorithms.RMY.Apply(bSource);
            AForge.Imaging.Filters.SISThreshold Filter = new SISThreshold();
            b = Filter.Apply(b);
            // Filter.ThresholdValue  = Filter.CalculateThreshold(holding , new Rectangle(0, 0, SourceImage.Width, SourceImage.Height));
            Bitmap bOut = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppRgb);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            BitmapData bmDataSource = bSource.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
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
                        p += 1;
                        pOut += 4;
                        ps += 4;
                    }
                }
            }

            b.UnlockBits(bmData);
            bOut.UnlockBits(bmDataOut);
            bSource.UnlockBits(bmDataSource);
            // Graphics.FromImage(bOut).Clear(Color.Blue);
            return EffectHelps.FixImageFormat( bOut);

        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }


    }
}
