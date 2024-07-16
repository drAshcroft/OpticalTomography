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

namespace ImageViewer.Filters.Masks
{
    public partial class FeatherMaskTool : aEffectForm
    {
        public FeatherMaskTool()
            : base()
        {
            SetParameters(new string[] { "Threshold", "Border Increase", "Feather Size" }, new int[] { 0, 0, 0 }, new int[] { 255, 25, 25 });
        }
        public override string EffectName { get { return "Feather Mask"; } }
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
            get { return new object[] {200,2}; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "Threshold|int","Feather_Size|int"}; }
        }
        private Bitmap DoFeathering(Bitmap Source, int Size, double Sigma)
        {
            GaussianBlur blur = new GaussianBlur(Sigma, Size);

            Bitmap holding = new Bitmap(Source.Width, Source.Height, PixelFormat.Format24bppRgb);
            Graphics.FromImage(holding).DrawImage(Source, 0, 0);
            return blur.Apply(holding);
        }
        private Bitmap DoIterativeMask(Bitmap source)
        {
            Bitmap b = Grayscale.CommonAlgorithms.RMY.Apply(source );
            AForge.Imaging.Filters.IterativeThreshold Filter = new IterativeThreshold();
            b = Filter.Apply(b);
            return b;
        }

        private Bitmap DoOtsuMask(Bitmap source)
        {
            Bitmap b = Grayscale.CommonAlgorithms.RMY.Apply(source);
            AForge.Imaging.Filters.OtsuThreshold  Filter = new OtsuThreshold();
            b = Filter.Apply(b);
            return b;
        }
        private Bitmap DoSISMask(Bitmap source)
        {
            Bitmap b = Grayscale.CommonAlgorithms.RMY.Apply(source);
            AForge.Imaging.Filters.SISThreshold  Filter = new SISThreshold();
            b = Filter.Apply(b);
            return b;
        }

        protected override Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> doEffect(
       DataEnvironment dataEnvironment, Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> SourceImage,
       Dictionary<string, object> PassData,
       params object[] Parameters)
        {
            if (FilterToken == null)
                FilterToken = DefaultToken;

            int Threshold = (int)FilterToken.Parameters[0];
            Bitmap b;
            if (Threshold == -10)
                b = DoIterativeMask(SourceImage);
            else if (Threshold == -20)
                b = DoSISMask(SourceImage);
            else if (Threshold == -30)
                b = DoOtsuMask(SourceImage);
            else
                b = ImagingTools.ThresholdImage(SourceImage, (int)FilterToken.Parameters[0]);

            Bitmap EnlargedMask = DoFeathering(b, (int)FilterToken.Parameters[1], (double)(int)FilterToken.Parameters[1]);
            EnlargedMask = ImagingTools.ThresholdImage(EnlargedMask, 254);
            Bitmap FeatherMask = DoFeathering(EnlargedMask, (int)FilterToken.Parameters[2], (double)(int)FilterToken.Parameters[2]);
           
            // Filter.ThresholdValue  = Filter.CalculateThreshold(holding , new Rectangle(0, 0, SourceImage.Width, SourceImage.Height));
            Bitmap bOut = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppArgb);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmDataMask = EnlargedMask.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            BitmapData bmDataFeather = FeatherMask.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmDataSource = SourceImage.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte Intensity;
            unsafe
            {
                for (int y = 0; y < b.Height; ++y)
                {
                    byte* pMask = (byte*)(void*)bmDataMask.Scan0 + bmDataMask.Stride * y;
                    byte* pFeather = (byte*)(void*)bmDataFeather.Scan0 + bmDataFeather.Stride * y;
                    byte* pSource = (byte*)(void*)bmDataSource.Scan0 + bmDataSource.Stride * y;
                    byte* pOut = (byte*)(void*)bmDataOut.Scan0 + bmDataOut.Stride * y;
                    for (int x = 0; x < b.Width; ++x)
                    {
                        Intensity = pMask[0];

                        //if the mask is zero, then just pass the data through
                     /*   if (Intensity == 0)
                        {
                            *((Int32*)pOut) = *((Int32*)pSource);
                            // *pOut[0] = pSource[0];
                            pOut[1] = pSource[1];
                            pOut[2] = pSource[2];
                            pOut[3] = (byte)(pSource[3]);* /
                        }
                        else */
                        //taper the data off based on the feathering
                        {
                            double alpha = ( *pFeather)/255d;
                            pOut[0] = (byte)(pSource[0] * alpha);
                            pOut[1] = (byte)(pSource[0] * alpha);
                            pOut[2] = (byte)(pSource[0] * alpha);
                            pOut[3] = (byte)(pSource[3]);
                        }
                        pMask += 4;
                        pOut += 4;
                        pSource += 4;
                        pFeather += 4;
                    }
                }
            }

            EnlargedMask.UnlockBits(bmDataMask);
            FeatherMask.UnlockBits(bmDataFeather);
            bOut.UnlockBits(bmDataOut);
            SourceImage.UnlockBits(bmDataSource);
            // Graphics.FromImage(bOut).Clear(Color.Blue);
            return bOut;
        }
       
    }
}
