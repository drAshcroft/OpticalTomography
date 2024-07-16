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
using ImageViewer.Filters;

namespace ImageViewer.Filters.Circles
{
    public partial class GetCircleEdgeTool : aEffectForm
    {
        public GetCircleEdgeTool()
            : base()
        {

        }
        public override string EffectName { get { return "Get Circle Edge From Data"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Circles"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 10;
            }
        }
       
        public override bool PassesPassData
        {
            get
            {
                return true;
            }
        }
        public override object[] DefaultProperties
        {
            get { return new object[] { null }; }
        }

        public override string ParameterList
        {
            get { return new string[] { "|" }; }
        }

        protected override Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> doEffect(
       DataEnvironment dataEnvironment, Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> SourceImage,
       Dictionary<string, object> PassData,
       params object[] Parameters)
        {
            mSourceImages = SourceImage;
            mFilterToken = FilterToken;

            if (mFilterToken == null)
                mFilterToken = DefaultToken;


            Bitmap ShowImage = SourceImage[0].ActiveSelectedImage;
            mScratchImage = new Bitmap(ShowImage.Width, ShowImage.Height, PixelFormat.Format32bppArgb);
            Graphics.FromImage(mScratchImage).DrawImage(ShowImage,
                new Rectangle(0, 0, mScratchImage.Width, mScratchImage.Height),
                new Rectangle(0, 0, ShowImage.Width, ShowImage.Height), GraphicsUnit.Pixel);


            DoRun();


            while (this.Visible == true)
                Application.DoEvents();


            Bitmap b = (Bitmap)SourceImage.Clone();

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            double CenterX = (double)b.Width / 2d;
            double CenterY = (double)b.Height / 2d;
            double dx, dy, r;
            double AverageR = 0, countAR = 0;
            double Intensity, lIntensity = 1;
            double AverageAngle = 0, AngleWeight = 0;

            unsafe
            {
              
                for (int y = 15; y < b.Height-15; ++y)
                {
                    byte* p = (byte*)(void*)bmData.Scan0 + bmData.Stride * y;
                    for (int x = 15; x < b.Width-15; ++x)
                    {
                        Intensity = ((double)(p[0] + p[1] + p[2]) / 3d);
                        if (Intensity == 0)
                        {
                            dx = x - CenterX;
                            dy = y - CenterY;
                            r = Math.Sqrt(dx * dx + dy * dy);
                            AverageR += r;
                            countAR++;
                        }
                        p += 4;
                    }

                }


                AverageR /= countAR;
                AverageR *= 1.05;

                
                double angle = 0;
                AverageAngle = 0;
                countAR = 0;
                for (int y = 15; y < b.Height - 15; ++y)
                {
                    byte* p = (byte*)(void*)bmData.Scan0 + bmData.Stride * y;
                    for (int x = 15; x < b.Width - 15; ++x)
                    {
                            dx = x - CenterX;
                            dy = y - CenterY;
                            r = Math.Sqrt(dx * dx + dy * dy);
                            if (r < AverageR)
                            {
                                if (p[0] + p[1] + p[2] == 0)
                                {
                                    angle = Math.Atan( Math.Abs(dy / dx));
                                    if (dx < 0 && dy > 0)
                                        angle += Math.PI / 2d;
                                    if (dx < 0 && dy < 0)
                                        angle += Math.PI;
                                    if (dx > 0 && dy < 0)
                                        angle += Math.PI * 3d / 2d;
                                    AverageAngle += angle;
                                    countAR++;
                                }

                            }
                        p += 4;
                    }

                }
                AverageAngle = (AverageAngle / countAR) /Math.PI *180d;

            }

            //AverageAngle /= AngleWeight;

            b.UnlockBits(bmData);
            Rectangle AverageRrect = new Rectangle((int)(CenterX - AverageR), (int)(CenterY - AverageR), (int)(2 * AverageR), (int)(2 * AverageR));

            Graphics g = Graphics.FromImage(b);
            //g.DrawEllipse(Pens.Red, AverageRrect);

            g.DrawLine(Pens.Red, new Point((int)CenterX, (int)CenterY),
                new Point((int)(CenterX + AverageR * Math.Cos(AverageAngle / 180 * Math.PI)), (int)(CenterY - AverageR * Math.Sin(AverageAngle / 180 * Math.PI))));
            mPassData = AverageAngle;
            //System.Diagnostics.Debug.Print(AverageAngle.ToString());
            return b;

        }
    }
}
