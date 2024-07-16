﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using ImageViewer.Filters;

namespace ImageViewer.Filters.Blobs
{
    public class GetBlobDescriptions : aEffectNoForm
    {
        public override string EffectName { get { return "Get Blob Descriptions"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Blobs"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
        public override bool AcceptsPassData
        {
            get
            {
                return true;
            }
        }
        public override string AcceptsPassDataTypes
        {
            get
            {
                return typeof(int ).ToString();
            }
        }
        public override bool PassesPassData
        {
            get
            {
                return true;
            }
        }
        protected override Bitmap doEffect(Bitmap SourceImage, IEffectToken FilterToken)
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
        public override IEffectToken CreateToken(params object[] TokenValues)
        {
            mFilterToken = new GeneralToken();
            mFilterToken.Parameters = TokenValues;
            return mFilterToken ;
        }
        public override IEffectToken DefaultToken 
        {
            get
            {
                return CreateToken(null);
            }
        }
           
        
    }
}
