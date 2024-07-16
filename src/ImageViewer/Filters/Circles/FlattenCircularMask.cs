using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace ImageViewer.Filters.Circles
{
    public class FlattenCircularMaskEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Flatten Circular Mask"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Circles"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 30;
            }
        }


        public override Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> DoEffect(
                DataEnvironment dataEnvironment, Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> SourceImage,
                Dictionary<string, object> PassData,
                params object[] Parameters)
        {
            mFilterToken = FilterToken;
            mSourceImages = SourceImage;

            ISelection selection = SourceImage[0].ActiveSelection;

            for (int i = 0; i < SourceImage.Length; i++)
            {
                mSourceImages[i].NotifyOfSelection(null);
            }


            if (selection != null)
            {
                mFilterToken.Parameters[0] = selection.SelectionBounds;
            }

            for (int i = 0; i < SourceImage.Length; i++)
            {
                SourceImage[i].ActiveSelectedImage = RunEffect(SourceImage[i].ActiveSelectedImage, mFilterToken);
            }

            for (int i = 0; i < SourceImage.Length; i++)
            {
                mSourceImages[i].NotifyOfSelection(selection);
            }

            if (FilterToken == null)
                FilterToken = DefaultToken;

            if (mPassData != null && mPassData.GetType() == typeof(Rectangle))
                FilterToken.Parameters[0] = mPassData;

            Rectangle Bounds = (Rectangle)FilterToken.Parameters[0];
            // Filter.ThresholdValue  = Filter.CalculateThreshold(holding , new Rectangle(0, 0, SourceImage.Width, SourceImage.Height));
            Bitmap bOut = new Bitmap(SourceImage.Width, SourceImage.Height, PixelFormat.Format32bppArgb);
            BitmapData bmDataSource = SourceImage.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte Intensity;
            double centerX = Bounds.X + (double)Bounds.Width / 2d;
            double centerY = Bounds.Y + (double)Bounds.Height / 2d;
            double R = ((double)Bounds.Width / 2d), r, dx, dy;
            R = R * R;
            unsafe
            {
                double MinR = double.MaxValue;
                double MinDr = 0;
                double MinAverage = double.MaxValue;
                double cR=0;
                double AverageIntensity = 0, cc = 0;
                double MinIntensity = double.MaxValue;
                double II;
                for (int dR = -4; dR < 4; dR++)
                {
                    cR = Math.Pow( Math.Sqrt(R) + dR,2);
                    AverageIntensity = 0; cc = 0;
                    MinIntensity = double.MaxValue;
                    
                    for (int y = 0; y < SourceImage.Height; ++y)
                    {
                        byte* ps = (byte*)(void*)bmDataSource.Scan0 + bmDataSource.Stride * y;
                        for (int x = 0; x < SourceImage.Width; ++x)
                        {
                            dx = x - centerX;
                            dy = y - centerY;
                            r = (dx * dx + dy * dy);
                            if ((r - cR) < 1)
                            {
                                II = ((double)ps[0] + ps[1] + ps[2]) / 3d;
                                AverageIntensity += II;
                                if (II < MinIntensity) MinIntensity = II;
                                cc++;
                            }
                            ps += 4;
                        }
                    }
                    if (cc > 0)
                        AverageIntensity /= cc;
                    if (MinIntensity < MinAverage)
                    {
                        MinAverage = MinIntensity;
                        MinR = cR;
                        MinDr = dR;
                    }
                }
                R =  MinR  ;
                AverageIntensity = MinAverage;
                AverageIntensity = MinIntensity ;
                for (int y = 0; y < SourceImage.Height; ++y)
                {
                    byte* ps = (byte*)(void*)bmDataSource.Scan0 + bmDataSource.Stride * y;
                    byte* pOut = (byte*)(void*)bmDataOut.Scan0 + bmDataOut.Stride * y;
                    for (int x = 0; x < SourceImage.Width; ++x)
                    {
                        dx = x - centerX;
                        dy = y - centerY;
                        r = (dx * dx + dy * dy);
                        if (r > R)
                            Intensity = 0;
                        else
                            Intensity = 1;

                        if (Intensity == 1)
                        {
                            pOut[0] = (byte)(ps[0] );
                            pOut[1] = (byte)(ps[1] );
                            pOut[2] = (byte)(ps[2] );
                            pOut[3] = (byte)(ps[3]);
                        }
                        else
                        {
                            pOut[0] = (byte)AverageIntensity;
                            pOut[1] = (byte)AverageIntensity;
                            pOut[2] = (byte)AverageIntensity;
                            pOut[3] = (byte)(ps[3]);
                        }
                        pOut += 4;
                        ps += 4;
                    }
                }
            }

            SourceImage.UnlockBits(bmDataSource);
            bOut.UnlockBits(bmDataOut);
            // Graphics.FromImage(bOut).Clear(Color.Blue);
            return bOut;

        }

        public override object[] DefaultProperties
        {
            get { return new object[] { new Rectangle(0, 0, 10, 10) }; }
        }

        public override string ParameterList
        {
            get { return new string[] { "Circle_Bounds|Rectangle" }; }
        }


    }
}
