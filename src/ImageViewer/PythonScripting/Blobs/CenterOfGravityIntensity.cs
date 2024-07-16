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
using MathHelpLib;
using MathHelpLib.ImageProcessing;

namespace ImageViewer.Filters.Blobs
{
    public partial class CenterOfGravityIntensityTool : aEffectNoForm
    {
        public CenterOfGravityIntensityTool()
        {

        }
        public override string EffectName { get { return "Whole image Center Of Gravity, using pixel intensity"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Blobs"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 100;
            }
        }

        public override bool PassesPassData
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// takes an image and then returns the center of gravity.  You must threshold the image yourself before calling this function.
        /// </summary>
        /// <param name="ThresholdedImage"></param>
        /// <returns></returns>
        public static BlobDescription[] DoCOG(Bitmap SourceImage, Bitmap ThresholdedImage)
        {


            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmThreshold = ThresholdedImage.LockBits(new Rectangle(0, 0, ThresholdedImage.Width, ThresholdedImage.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            BitmapData bmData = SourceImage.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            Int32 Black = Color.Black.ToArgb();
            Int32 White = Color.White.ToArgb();
            Int32 Red = Color.Red.ToArgb();
            Int32 curColor = 0;
            List<BlobDescription> Blobs = new List<BlobDescription>();

            double SumX = 0;
            double SumY = 0;
            double CC = 0;
            double Intensity = 0;
            double SumIntensity = 0;
            int MaxX = int.MinValue;
            int MinX = int.MaxValue;
            int MaxY = int.MinValue;
            int MinY = int.MaxValue;

            unsafe
            {
                //mark all the blobs by watersheding
                for (int y = 0; y < ThresholdedImage.Height; ++y)
                {
                    Int32* p = (Int32*)((byte*)(void*)bmThreshold.Scan0 + bmThreshold.Stride * y);

                    Int32* pD = (Int32*)((byte*)(void*)bmData.Scan0 + bmData.Stride * y);

                    for (int x = 0; x < ThresholdedImage.Width; ++x)
                    {
                        //the alpha channel always causes problems. so remove it from the colors
                        curColor = (*p) & 0x00FFFFFF;
                        
                        //if this pixel is black then add it to the average
                        if (curColor == 0)
                        {

                            Intensity = 255d - (double)((*pD & 0x0000FF00) >> 8 + (*pD & 0x000000FF) + (*pD & 0x00FF0000)>>16)/3d;  
                            CC++;
                            SumX += x*Intensity ;
                            SumY += y*Intensity ;
                            if (y > MaxY) MaxY = y;
                            if (y < MinY) MinY = y;
                            if (x > MaxX) MaxX = x;
                            if (x < MinX) MinX = x;
                            SumIntensity += Intensity;
                        }
                        p++;
                    }
                }

                double xx = SumX / SumIntensity;
                double yy = SumY / SumIntensity;

                Blobs.Add(new BlobDescription(0, Rectangle.FromLTRB(MinX, MinY, MaxX, MaxY), new Point((int)(Math.Round(xx)), (int)Math.Round(yy))));
            }
            SourceImage.UnlockBits(bmData);
            ThresholdedImage.UnlockBits(bmThreshold);
            return Blobs.ToArray();

        }


        /// <summary>
        /// takes an image and then returns the center of gravity.  You must threshold the image yourself before calling this function.
        /// </summary>
        /// <param name="ThresholdedImage"></param>
        /// <returns></returns>
        public static BlobDescription[] DoCOGCenter(Bitmap SourceImage, Bitmap ThresholdedImage)
        {
            FloodFiller ff = new FloodFiller();
            ff.FillStyle = FloodFillStyle.Linear;

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmThreshold = ThresholdedImage.LockBits(new Rectangle(0, 0, ThresholdedImage.Width, ThresholdedImage.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);


            //fill the center to mark the cell (easy watershedding)
            int HalfX = SourceImage.Width / 2;
            int HalfY = SourceImage.Height / 2;


            ff.FillColor = 5;
            try
            {
                ff.FloodFill(bmThreshold, new Point(HalfX, HalfY));
            }
            catch { }


            BitmapData bmData = SourceImage.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            Int32 Black = Color.Black.ToArgb();
            Int32 White = Color.White.ToArgb();
            Int32 Red = Color.Red.ToArgb();
            Int32 curColor = 0;
            List<BlobDescription> Blobs = new List<BlobDescription>();

            double SumX = 0;
            double SumY = 0;
            double CC = 0;
            double Intensity = 0;
            double SumIntensity = 0;
            int MaxX = int.MinValue;
            int MinX = int.MaxValue;
            int MaxY = int.MinValue;
            int MinY = int.MaxValue;

            unsafe
            {
                //mark all the blobs by watersheding
                for (int y = 0; y < ThresholdedImage.Height; ++y)
                {
                    Int32* p = (Int32*)((byte*)(void*)bmThreshold.Scan0 + bmThreshold.Stride * y);

                    Int32* pD = (Int32*)((byte*)(void*)bmData.Scan0 + bmData.Stride * y);

                    for (int x = 0; x < ThresholdedImage.Width; ++x)
                    {
                        //the alpha channel always causes problems. so remove it from the colors
                        curColor = (*p) & 0x00FFFFFF;

                        //if this pixel is black then add it to the average
                        if (curColor == 5)
                        {

                            Intensity = 255d - (double)((*pD & 0x0000FF00) >> 8 + (*pD & 0x000000FF) + (*pD & 0x00FF0000) >> 16) / 3d;
                            CC++;
                            SumX += x * Intensity;
                            SumY += y * Intensity;
                            if (y > MaxY) MaxY = y;
                            if (y < MinY) MinY = y;
                            if (x > MaxX) MaxX = x;
                            if (x < MinX) MinX = x;
                            SumIntensity += Intensity;
                        }
                        p++;
                    }
                }

                double xx = SumX / SumIntensity;
                double yy = SumY / SumIntensity;

                Blobs.Add(new BlobDescription(0, Rectangle.FromLTRB(MinX, MinY, MaxX, MaxY), new Point((int)(Math.Round(xx)), (int)Math.Round(yy))));
            }

            SourceImage.UnlockBits(bmData);
            ThresholdedImage.UnlockBits(bmThreshold);
            return Blobs.ToArray();

        }

        /// <summary>
        /// takes an image and then returns the center of gravity.  You must threshold the image yourself before calling this function.
        /// </summary>
        /// <param name="ThresholdImage"></param>
        /// <returns></returns>
        public static BlobDescription[] DoCOG(ImageHolder SourceImage, ImageHolder ThresholdImage)
        {

            float[, ,] SourceData = SourceImage.ImageData;
            float[, ,] ThresholdData = ThresholdImage.ImageData;

            float MaxI = SourceData.MaxArray();
            float MinI = SourceData.MinArray();

            double curColor = 0;
            List<BlobDescription> Blobs = new List<BlobDescription>();

            double SumX = 0;
            double SumY = 0;
            double CC = 0;

            int MaxX = int.MinValue;
            int MinX = int.MaxValue;
            int MaxY = int.MinValue;
            int MinY = int.MaxValue;
            double Intensity, SumIntensity=0;
            int Width = ThresholdData.GetLength(0);
            int Height = ThresholdData.GetLength(1);

            int  x, y;
            int NChanThresh = ThresholdImage.NChannels;
            int NChanSource = SourceImage. NChannels;
            long Length = SourceData.Length;
            unsafe
            {
                fixed (float* pThresh = ThresholdData)
                {
                    fixed (float* pSource = SourceData)
                    {
                        float* PT = pThresh;
                        float* pSourceImage = pSource;

                        for (int i = 0; i < Length; i++)
                        //mark all the blobs by watersheding
                        {
                            curColor = *PT;
                            //if the pixel is zero(or almost zero) takes its value into the average
                            if (curColor < 1)
                            {
                                Intensity =Math.Sqrt( (pSource[i]-MaxI )/(MinI -MaxI ));
                                SumIntensity += Intensity;
                                //get the coords in the image
                                x = i % Width;
                                y = i / Width;
                                CC++;
                                SumX += x*Intensity ;
                                SumY += y*Intensity ;
                                if (y > MaxY) MaxY = y;
                                if (y < MinY) MinY = y;
                                if (x > MaxX) MaxX = x;
                                if (x < MinX) MinX = x;
                            }
                            PT += NChanThresh;
                            pSourceImage += NChanSource;
                        }
                    }
                }
            }

            double xx = SumX / SumIntensity;
            double yy = SumY / SumIntensity;
            Blobs.Add(new BlobDescription(0, Rectangle.FromLTRB(MinX, MinY, MaxX, MaxY), new Point((int)Math.Round (xx ), (int)Math.Round (yy))));


            return Blobs.ToArray();

        }

        /// <summary>
        /// takes an image and then returns the center of gravity.  You must threshold the image yourself before calling this function.
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">No Parameters</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            if (mFilterToken == null)
                mFilterToken = DefaultProperties;
            mPassData = PassData;

            Bitmap holding = null;
            Bitmap Source = null;

            BlobDescription[] Blobs=null;
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                Source  = (Bitmap)SourceImage;
                holding = (Bitmap)Parameters[0];
                Blobs = DoCOG(Source, holding);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                ImageHolder Mask;
                if (Parameters[0].GetType() == typeof(ImageHolder))
                    Mask = (ImageHolder)Parameters[0];
                else
                    Mask = new ImageHolder((Bitmap)Parameters[0]);
                Blobs = DoCOG((ImageHolder)SourceImage,Mask );
            }



            mPassData.AddSafe("Blobs", Blobs);

            return SourceImage;
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }





    }
}
