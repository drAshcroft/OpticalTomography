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

namespace ImageViewer.Filters.SelectionEffects
{
    public class GetBlobDescriptions : IEffect
    {
        public string EffectName { get { return "Get Blob Descriptions"; } }
        public string EffectMenu { get { return "Selections"; } }
        public string EffectSubMenu { get { return ""; } }
        public int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        object mPassData = null;
        public object PassData
        {
            get { return mPassData; }
            set { mPassData = value; }
        }

        public IEffectToken CurrentProperties { get { return mFilterToken; } }

        IEffectToken mFilterToken;

        public void RunEffect(ScreenProperties SourceImage, IEffectToken FilterToken)
        {
            ScreenProperties[] sp = { SourceImage };
            RunEffect(sp, FilterToken);
        }
        public string RunEffect(ScreenProperties[] SourceImage, IEffectToken FilterToken)
        {
            for (int i = 0; i < SourceImage.Length; i++)
            {
                SourceImage[i].ActiveSelectedImage = RunEffect(SourceImage[i],SourceImage[i].ActiveSelectedImage, FilterToken);
            }
            string MacroString = EffectHelps.FormatParameterlessMacroString("PassData = Filter.PassData",  this) + "\n";
            MacroString += "PassData = Filter.PassData\n";
            return MacroString;
        }

        /// <summary>
        /// these functions must be called after watershed has been called.  they require a number that is passed from 
        /// watershed that indicates the number of particles.  Errors will be thrown if this is not followed
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="FilterToken"></param>
        /// <returns></returns>
        public Bitmap RunEffect(ScreenProperties screenProperties, Bitmap SourceImage, IEffectToken FilterToken)
        {
            if (mPassData.GetType() != typeof(Int32))
                throw new Exception("You must run the watershed algorythm for this function to work");
            Int32 ParticleCount = (Int32)mPassData;

            Bitmap holding = SourceImage;
            FloodFiller ff = new FloodFiller();
            ff.FillStyle = FloodFillStyle.Linear;

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = holding.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Int32 Black = Color.Black.ToArgb();
            Int32 White = Color.White.ToArgb();
            Int32 Red = Color.Red.ToArgb();


            List<BlobDescription> Blobs = new List<BlobDescription>();

            unsafe
            {
                for (int i = 0; i < ParticleCount; i++)
                {
                    int MaxX = int.MinValue;
                    int MinX = int.MaxValue;
                    int MaxY = int.MinValue;
                    int MinY = int.MaxValue;
                    int CenterX = -1;
                    int CenterY = -1;
                    int MaxIndex = 0;
                    double SumX = 0;
                    double SumY = 0;
                    double Count = 0;
                    for (int y = 0; y < SourceImage.Height; ++y)
                    {
                        Int32* p = (Int32*)((byte*)(void*)bmData.Scan0 + bmData.Stride * y);
                        for (int x = 0; x < SourceImage.Width; ++x)
                        {
                            if (*p == MaxIndex)
                            {
                                SumX += x;
                                SumY += y;
                                Count++;
                                if (x > MaxX) MaxX = x;
                                if (x < MinX) MinX = x;
                                if (y > MaxY) MaxY = y;
                                if (y < MinY) MinY = y;
                            }
                            p++;
                        }
                    }
                    if (Count > 0)
                    {
                        CenterX = (int)Math.Truncate(SumX / Count);
                        CenterY = (int)Math.Truncate(SumY / Count);

                        Blobs.Add( new BlobDescription(i, Rectangle.FromLTRB(MinX, MinY, MaxX, MaxY), new Point(CenterX, CenterY)));
                    }
                }
            }

            holding.UnlockBits(bmData);

            mPassData = Blobs.ToArray();
            return (Bitmap)SourceImage.Clone();
        }

        public void Show(IWin32Window owner)
        {
            mFilterToken = new GeneralToken();
            mFilterToken.Parameters = new object[1];
        }
    }
}
