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
    public partial class GetCircleTool : aEffectForm
    {
        public GetCircleTool()
            : base()
        {

        }
        public override string EffectName { get { return "Get Circle From Data"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Circles"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 1;
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
            get { return null; }
        }

        public override string ParameterList
        {
            get { return ""; }
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


            Bitmap ShowImage = SourceImage.ActiveSelectedImage;
            mScratchImage = new Bitmap(ShowImage.Width, ShowImage.Height, PixelFormat.Format32bppArgb);
            Graphics.FromImage(mScratchImage).DrawImage(ShowImage,
                new Rectangle(0, 0, mScratchImage.Width, mScratchImage.Height),
                new Rectangle(0, 0, ShowImage.Width, ShowImage.Height), GraphicsUnit.Pixel);


            DoRun();


            while (this.Visible == true)
                Application.DoEvents();

          
            Bitmap b = SourceImage;

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            double CenterX = (double)b.Width / 2d;
            double CenterY = (double)b.Height / 2d;
            double dx, dy, r;
            double AverageMaxR=0, AverageR=0,countAMR=0,countAR=0;
            double  Intensity,lIntensity=1;
            unsafe
            {
                List<Point> Edges = new List<Point>();
                for (int y = 5; y < b.Height-5; ++y)
                {
                    byte* p = (byte*)(void*)bmData.Scan0 + bmData.Stride * y;
                    for (int x = 5; x < b.Width-5; ++x)
                    {
                        Intensity = ((double)(p[0]+p[1]+p[2] ) );
                        if ((Intensity == 0 && lIntensity != 0) || (Intensity != 0 && lIntensity == 0))
                        {
                            Edges.Add(new Point(x, y));
                        }
                        lIntensity = Intensity;
                        p += 4;
                    }

                }
                Point[] aEdges = Edges.ToArray();
                double MaxR = 0;
                Point[] Points = new Point[8];
                for (int i = 1; i < aEdges.Length; i++)
                    for (int j = 1; j < aEdges.Length; j++)
                    {
                        dx = aEdges[i].X - aEdges[j].X;
                        dy = aEdges[i].Y - aEdges[j].Y;
                        r = dx * dx + dy * dy;
                        if (r > MaxR)
                        {
                            MaxR = r;
                            for (int m = Points.Length-1; m>=2; m--)
                            {
                                Points[m] = Points[m - 2];
                            }
                            if (i!=0)
                            Points[0] = aEdges [ i];
                            Points[1] =aEdges[ j];
                        }
                    }
                CenterX = 0;
                CenterY = 0;
                for (int m = 0; m < Points.Length; m ++)
                {
                    CenterX += Points[m].X;
                    CenterY += Points[m].Y;
                }
                CenterX /= (Points.Length);
                CenterY /= (Points.Length);

                for (int y = 0; y < b.Height; ++y)
                {
                    byte* p = (byte*)(void*)bmData.Scan0 + bmData.Stride * y;
                    for (int x = 0; x < b.Width; ++x)
                    {
                        Intensity = ((double)(p[0] ));
                        if ((Intensity == 0 && lIntensity!=0) || (Intensity!=0 && lIntensity==0))
                        {
                            dx = x - CenterX;
                            dy = y - CenterY;
                            r = Math.Sqrt(dx * dx + dy * dy);
                            AverageMaxR += r;
                            countAMR++;
                        }
                        if (Intensity == 0)
                        {
                            dx = x - CenterX;
                            dy = y - CenterY;
                            r = Math.Sqrt( dx * dx + dy * dy);
                            AverageR += r;
                            countAR++;
                        }
                        lIntensity = Intensity;
                        p += 4;
                    }

                }
            }
            if (countAMR >0)
                AverageMaxR /= countAMR;
            if (countAR >0)
                AverageR /= countAR;

            b.UnlockBits(bmData);

            Rectangle AverageMax = new Rectangle((int)(CenterX - AverageMaxR),(int)( CenterY - AverageMaxR),(int)( 2 * AverageMaxR),(int)( 2 * AverageMaxR));
            Rectangle AverageRrect = new Rectangle((int)(CenterX - AverageR), (int)(CenterY - AverageR), (int)(2 * AverageR), (int)(2 * AverageR));
            
            Graphics g = Graphics.FromImage(b);
            g.DrawEllipse(Pens.Red, AverageMax);
            g.DrawEllipse(Pens.Red, AverageRrect);

            mPassData = AverageMax;
            return b;

        }
       
    }
}
