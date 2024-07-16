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
    public partial class CenterOfGravityTool : aEffectNoForm
    {
        public CenterOfGravityTool()
        {

        }
        public override string EffectName { get { return "Whole image Center Of Gravity"; } }
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
        /// <param name="SourceImage"></param>
        /// <returns></returns>
        public static BlobDescription[] DoCOG(Bitmap SourceImage)
        {


            // GDI+ still lies to us - the return format is BGR, NOT RGB.
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

            int MaxX = int.MinValue;
            int MinX = int.MaxValue;
            int MaxY = int.MinValue;
            int MinY = int.MaxValue;

           

            unsafe
            {
                for (int y = 0; y < SourceImage.Height; ++y)
                {
                    Int32* p = (Int32*)((byte*)(void*)bmData.Scan0 + bmData.Stride * y);
                    for (int x = 0; x < SourceImage.Width; ++x)
                    {
                        //the alpha channel always causes problems. so remove it from the colors
                        curColor = (*p) & 0x00FFFFFF;
                        
                        //if this pixel is black then add it to the average
                        if (curColor == 0)
                        {
                            CC++;
                            SumX += x;
                            SumY += y;
                            if (y > MaxY) MaxY = y;
                            if (y < MinY) MinY = y;
                            if (x > MaxX) MaxX = x;
                            if (x < MinX) MinX = x;
                        }
                        p++;
                    }
                }

                Blobs.Add(new BlobDescription(0, Rectangle.FromLTRB(MinX, MinY, MaxX, MaxY), new Point((int)(SumX / CC), (int)(SumY / CC))));
            }

            SourceImage.UnlockBits(bmData);
            return Blobs.ToArray();

        }

        /// <summary>
        /// takes an image and then returns the center of gravity.  You must threshold the image yourself before calling this function.
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns></returns>
        public static BlobDescription[] DoCOGCenter(Bitmap SourceImage)
        {

            FloodFiller ff = new FloodFiller();
            ff.FillStyle = FloodFillStyle.Linear;

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = SourceImage.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);


            int HalfX = SourceImage.Width / 2;
            int HalfY = SourceImage.Height / 2;


            ff.FillColor = 5;
            try
            {
                ff.FloodFill(bmData, new Point(HalfX, HalfY));
            }
            catch { }

            Int32 Black = Color.Black.ToArgb();
            Int32 White = Color.White.ToArgb();
            Int32 Red = Color.Red.ToArgb();
            Int32 curColor = 0;
            List<BlobDescription> Blobs = new List<BlobDescription>();

            double SumX = 0;
            double SumY = 0;
            double CC = 0;

            int MaxX = int.MinValue;
            int MinX = int.MaxValue;
            int MaxY = int.MinValue;
            int MinY = int.MaxValue;

            unsafe
            {
                for (int y = 0; y < SourceImage.Height; ++y)
                {
                    Int32* p = (Int32*)((byte*)(void*)bmData.Scan0 + bmData.Stride * y);
                    for (int x = 0; x < SourceImage.Width; ++x)
                    {
                        //the alpha channel always causes problems. so remove it from the colors
                        curColor = (*p) & 0x00FFFFFF;

                        //if this pixel is black then add it to the average
                        if (curColor == 5)
                        {
                            CC++;
                            SumX += x;
                            SumY += y;
                            if (y > MaxY) MaxY = y;
                            if (y < MinY) MinY = y;
                            if (x > MaxX) MaxX = x;
                            if (x < MinX) MinX = x;
                        }
                        p++;
                    }
                }

                Blobs.Add(new BlobDescription(0, Rectangle.FromLTRB(MinX, MinY, MaxX, MaxY), new Point((int)(SumX / CC), (int)(SumY / CC))));
            }

            SourceImage.UnlockBits(bmData);
            return Blobs.ToArray();

        }

        /// <summary>
        /// takes an image and then returns the center of gravity.  You must threshold the image yourself before calling this function.
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns></returns>
        public static BlobDescription[] DoCOG(ImageHolder SourceImage)
        {
            double[,] image = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false);

            double curColor = 0;
            List<BlobDescription> Blobs = new List<BlobDescription>();

            double SumX = 0;
            double SumY = 0;
            double CC = 0;

            int MaxX = int.MinValue;
            int MinX = int.MaxValue;
            int MaxY = int.MinValue;
            int MinY = int.MaxValue;

            int Width = image.GetLength(0);
            int Height = image.GetLength(1);

            int  x, y;
            unsafe
            {
                fixed (double* pImage = image)
                {
                    for (int i = 0; i < image.Length; i++)
                    //mark all the blobs by watersheding
                    {
                        curColor = pImage[i];
                        //if the pixel is zero(or almost zero) takes its value into the average
                        if (curColor < 1)
                        {
                            //get the coords in the image
                            x = i % Width;
                            y = i / Height;
                            CC++;
                            SumX += x;
                            SumY += y;
                            if (y > MaxY) MaxY = y;
                            if (y < MinY) MinY = y;
                            if (x > MaxX) MaxX = x;
                            if (x < MinX) MinX = x;
                        }
                    }
                }
            }

            Blobs.Add(new BlobDescription(0, Rectangle.FromLTRB(MinX, MinY, MaxX, MaxY), new Point((int)(SumX / CC), (int)(SumY / CC))));


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
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                holding = (Bitmap)SourceImage;
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {

                holding = EffectHelps.ConvertToBitmap(SourceImage);
            }


            BlobDescription[] Blobs = DoCOG(holding);

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
