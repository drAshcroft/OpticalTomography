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
    public partial class WaterShedTool :  aEffectNoForm
    {
        public WaterShedTool()
        {
            
        }
        public override string EffectName { get { return "WaterShed"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Blobs"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 0;
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
        /// walks through each pixel of an image and assignes the value to the various blobs. (black spots should be the blobs)
        /// </summary>
        /// <param name="holding"></param>
        /// <returns></returns>
        public static  BlobDescription[] DoWatershed(Bitmap holding)
        {
            FloodFiller ff = new FloodFiller();
            ff.FillStyle = FloodFillStyle.Linear;

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = holding.LockBits(new Rectangle(0, 0, holding.Width, holding.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            Random rnd = new Random();
            Int32 ParticleCount = 0;
            Int32 Black = Color.Black.ToArgb();
            Int32 White = Color.White.ToArgb();
            Int32 Red = Color.Red.ToArgb();
            Int32 curColor = 0;
            List<BlobDescription> Blobs = new List<BlobDescription>();
            unsafe
            {
                unchecked
                {
                    //mark all the blobs by watersheding
                    for (int y = 0; y < holding.Height; ++y)
                    {
                        Int32* p = (Int32*)((byte*)(void*)bmData.Scan0 + bmData.Stride * y);
                        for (int x = 0; x < holding.Width; ++x)
                        {
                            ///the alpha channel screws things up, so just block it off
                            curColor = (*p) & 0x00FFFFFF;
                            //only look at the black spots
                            if (curColor == 0)
                            {
                                ParticleCount++;
                                ff.FillColor = ParticleCount;
                                try
                                {
                                    ff.FloodFill(bmData, new Point(x, y));
                                }
                                catch { }
                            }
                            p++;
                        }
                    }
                }
                int[] MaxX = new int[ParticleCount];
                int[] MinX = new int[ParticleCount];
                int[] MaxY = new int[ParticleCount];
                int[] MinY = new int[ParticleCount];
                int[] CenterX = new int[ParticleCount];
                int[] CenterY = new int[ParticleCount];
                double[] SumX = new double[ParticleCount];
                double[] SumY = new double[ParticleCount];
                double[] Count = new double[ParticleCount];

                for (int CurrentParticle = 0; CurrentParticle < ParticleCount; CurrentParticle++)
                {
                    MaxX[CurrentParticle] = int.MinValue;
                    MinX[CurrentParticle] = int.MaxValue;
                    MaxY[CurrentParticle] = int.MinValue;
                    MinY[CurrentParticle] = int.MaxValue;
                    CenterX[CurrentParticle] = -1;
                    CenterY[CurrentParticle] = -1;
                    SumX[CurrentParticle] = 0;
                    SumY[CurrentParticle] = 0;
                    Count[CurrentParticle] = 0;
                }

                unchecked
                {
                    //now pull the counts of each of the blobs
                    ///get the size of each of the blobs
                    int Bin;
                    for (int y = 0; y < holding.Height; ++y)
                    {
                        Int32* p = (Int32*)((byte*)(void*)bmData.Scan0 + bmData.Stride * y);
                        for (int x = 0; x < holding.Width; ++x)
                        {
                            curColor = (*p) & 0x00FFFFFF;
                            if (curColor != 0 && curColor != 16777215)
                            {
                                Bin = *p;

                                if (Bin > 0)
                                {
                                    Bin--;
                                    SumX[Bin] += x;
                                    SumY[Bin] += y;
                                    Count[Bin]++;
                                    if (x > MaxX[Bin]) MaxX[Bin] = x;
                                    if (x < MinX[Bin]) MinX[Bin] = x;
                                    if (y > MaxY[Bin]) MaxY[Bin] = y;
                                    if (y < MinY[Bin]) MinY[Bin] = y;
                                }
                            }
                            p++;
                        }
                    }
                }
                //last put them into nice packages with a sort that they have to be larger than 10 pixels
                for (int i = 0; i < ParticleCount; i++)
                {
                    if (Count[i] > 10 )
                    {
                        CenterX[i] = (int)Math.Truncate(SumX[i] / Count[i]);
                        CenterY[i] = (int)Math.Truncate(SumY[i] / Count[i]);
                        Rectangle r =  Rectangle.FromLTRB(MinX[i], MinY[i], MaxX[i], MaxY[i]);
                        if (r.Width>5 && r.Height>5)
                            Blobs.Add(new BlobDescription(i,r, new Point(CenterX[i], CenterY[i])));
                    }
                }
            }
            for (int i=0;i<Blobs.Count;i++)
                System.Diagnostics.Debug.Print("Blobs = " + Blobs[i].BlobBounds);
            holding.UnlockBits(bmData);
            return Blobs.ToArray();
        }


        public static BlobDescription[] DoWatershed(ImageHolder holding)
        {
            return DoWatershed(EffectHelps.ConvertToBitmap(holding));
        }
        /// <summary>
        /// accepts a thresholded image and extracts each blob from the image
        /// 
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">No parameters</param>
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
               
                holding = EffectHelps.ConvertToBitmap( SourceImage);
            }
           

            BlobDescription[] Blobs = DoWatershed(holding);

            mPassData.AddSafe("Blobs", Blobs);

            return  null  ;
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
