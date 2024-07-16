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
using MathHelpLib;

namespace ImageViewer.Filters.Blobs
{
    public class GetBiggestCenterBlob : aEffectNoForm
    {
        public override string EffectName { get { return "Get Biggest Center Blob"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Blobs"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 20;
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
        /// Finds the biggest blob from the list, also checking if the blob is in the center (horizontal) of the image and if it touches the edge(this is rejected)
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData">list used if there is an item named "Blobs"</param>
        /// <param name="Parameters">accepts and arrray of BlobDescriptions or an list of BlobDescriptions.  Parameter 2 indicates if you wish to have a rectangle drawn on the source image</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mPassData = PassData;

            int Width = 0;
            int Height = 0;
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                Width = ((Bitmap)SourceImage).Width;
                Height = ((Bitmap)SourceImage).Height;
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                Width = ((ImageHolder)SourceImage).Width;
                Height = ((ImageHolder)SourceImage).Height;
            }

            if (Parameters != null && Parameters.Length > 0)
            {
                if (Parameters[0].GetType() == typeof(BlobDescription[]))
                {
                    SortBlobDescriptions(((BlobDescription[])Parameters[0]), Width, Height);
                }
                else if (Parameters[0].GetType() == typeof(List<BlobDescription>))
                {
                    SortBlobDescriptions(((List<BlobDescription>)Parameters[0]).ToArray(), Width, Height);
                }
            }
            else if (mPassData.ContainsKey("Blobs") == true)
            {
                SortBlobDescriptions(((BlobDescription[])mPassData["Blobs"]), Width, Height);
            }
            else
                throw new Exception("Please run watershed filter before this filter");

            if (mPassData.ContainsKey("MaxBlob") == true && Parameters != null && (Parameters.Length >= 2 && ((bool)Parameters[1]) == true))
            {
                BlobDescription Mblob = (BlobDescription)mPassData["MaxBlob"];
                if (SourceImage.GetType() == typeof(Bitmap))
                {
                    Graphics g = Graphics.FromImage((Bitmap)SourceImage);
                    g.DrawRectangle(Pens.Red, Mblob.BlobBounds);
                    g.DrawEllipse(Pens.Red, new Rectangle(Mblob.CenterOfGravity, new Size(3, 3)));
                    g = null;
                }
                else if (SourceImage.GetType() == typeof(ImageHolder))
                {
                   /* Graphics g = Graphics.FromImage(((ImageHolder)SourceImage).InformationOverLay);
                    g.DrawRectangle(Pens.Red, Mblob.BlobBounds);
                    g.DrawEllipse(Pens.Red, new Rectangle(Mblob.CenterOfGravity, new Size(3, 3)));
                    g = null;*/
                }
            }

            mPassData.AddSafe("NumBigBlobs", (double)NumBigBlobs);
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

        public static BlobDescription SortBlobsCenter(BlobDescription[] Blobs, int Width, int Height, int MinSize, int MaxSize, out BlobDescription[] FilteredBlobs)
        {
            NumBigBlobs = 0;
            //chose the most centered blob that does not touch the edges of the image
            double half = Width / 2d;
            double MaxArea = double.MinValue;
            BlobDescription MaxBlob = null;
            double area;
            double x, Radius;

            int MinWidth = MinSize;
           // if (Width == 800)
            //    MinWidth = 50;

            List<BlobDescription> TestBlobs = new List<BlobDescription>();
            for (int i = 0; i < Blobs.Length; i++)
            {
                BlobDescription blob1 = Blobs[i];
                bool Found = false;
                for (int j = i + 1; j < Blobs.Length; j++)
                {
                    if (Blobs[j].BlobBounds.Left < blob1.CenterOfGravity.X && Blobs[j].BlobBounds.Right > blob1.CenterOfGravity.X && Blobs[j].CenterOfGravity.Y > blob1.CenterOfGravity.Y)
                    {
                        if (Blobs[j].BlobBounds.Width > MinWidth && Blobs[j].BlobBounds.Height > MinWidth &&
                            Blobs[j].BlobBounds.Height < MaxSize && Blobs[j].BlobBounds.Height < MaxSize)
                        {
                           // if (Math.Abs(Blobs[j].CenterOfGravity.X - half) < 200)
                            {
                                Found = true;
                                break;
                            }
                        }
                    }
                }
                if (!Found)
                {
                    TestBlobs.Add(blob1);
                }
            }

            Blobs = TestBlobs.ToArray();
            TestBlobs.Clear();
            for (int i = 0; i < Blobs.Length; i++)
            {
                BlobDescription blob = Blobs[i];

                //check if it is not touching the border
                if (blob.BlobBounds.Left > 25 && blob.BlobBounds.Right < Width - 25)
                {
                    // if (blob.BlobBounds.Bottom < Height - 100 &&  blob.BlobBounds.Top > 100)
                    {
                        x =Math.Abs (Blobs[i].CenterOfGravity.X - half);
                        Radius = Math.Exp(-1 * x * x / Width);
                        area = Blobs[i].BlobBounds.Height * Blobs[i].BlobBounds.Width;
                       // if (Blobs[i].BlobBounds.Width > MinWidth && Blobs[i].BlobBounds.Height > MinWidth)
                        {
                            if (area > 500)
                                NumBigBlobs++;
                            area = area * Radius;
                            if (area > MaxArea)
                            {
                                MaxBlob = Blobs[i];
                                MaxArea = area;
                            }

                            if (x < 200)
                                TestBlobs.Add(blob);
                        }
                    }
                }
            }

            FilteredBlobs = TestBlobs.ToArray();
            return MaxBlob;
        }


        public static BlobDescription SortBlobsCenter(BlobDescription[] Blobs, int Width, int Height, int MinSize, int MaxSize, out BlobDescription[] FilteredBlobs, int XCenter)
        {
            NumBigBlobs = 0;
            //chose the most centered blob that does not touch the edges of the image
            double half = XCenter;
            double MaxArea = double.MinValue;
            BlobDescription MaxBlob = null;
            double area;
            double x, Radius;

            int MinWidth = MinSize;
            // if (Width == 800)
            //    MinWidth = 50;

            List<BlobDescription> TestBlobs = new List<BlobDescription>();
            for (int i = 0; i < Blobs.Length; i++)
            {
                BlobDescription blob1 = Blobs[i];
                bool Found = false;
                for (int j = i + 1; j < Blobs.Length; j++)
                {
                    if (Blobs[j].BlobBounds.Left < blob1.CenterOfGravity.X && Blobs[j].BlobBounds.Right > blob1.CenterOfGravity.X && Blobs[j].CenterOfGravity.Y > blob1.CenterOfGravity.Y)
                    {
                        if (Blobs[j].BlobBounds.Width > MinWidth && Blobs[j].BlobBounds.Height > MinWidth &&
                            Blobs[j].BlobBounds.Height < MaxSize && Blobs[j].BlobBounds.Height < MaxSize)
                        {
                            // if (Math.Abs(Blobs[j].CenterOfGravity.X - half) < 200)
                            {
                                Found = true;
                                break;
                            }
                        }
                    }
                }
                if (!Found)
                {
                    TestBlobs.Add(blob1);
                }
            }

            Blobs = TestBlobs.ToArray();
            TestBlobs.Clear();
            for (int i = 0; i < Blobs.Length; i++)
            {
                BlobDescription blob = Blobs[i];

                //check if it is not touching the border
             //   if (blob.BlobBounds.Left > 25 && blob.BlobBounds.Right < Width - 25)
                {
                    // if (blob.BlobBounds.Bottom < Height - 100 &&  blob.BlobBounds.Top > 100)
                    {
                        x = Math.Abs(Blobs[i].CenterOfGravity.X - half);
                        Radius = Math.Exp(-1 * x * x / Width);
                        area = Blobs[i].BlobBounds.Height * Blobs[i].BlobBounds.Width;
                        // if (Blobs[i].BlobBounds.Width > MinWidth && Blobs[i].BlobBounds.Height > MinWidth)
                        if (Blobs[i].BlobBounds.X >0 && Blobs[i].BlobBounds.Right <Width)
                        {
                            if (area > 500)
                                NumBigBlobs++;
                            area = area * Radius;
                            if (area > MaxArea)
                            {
                                MaxBlob = Blobs[i];
                                MaxArea = area;
                            }

                            //if (x < 200)
                                TestBlobs.Add(blob);
                        }
                    }
                }
            }

            FilteredBlobs = TestBlobs.ToArray();
            return MaxBlob;
        }


        public static BlobDescription SortBlobs(BlobDescription[] Blobs, int Width, int Height)
        {
            NumBigBlobs = 0;
            //chose the most centered blob that does not touch the edges of the image
            double half = Width / 2d;
            double MaxArea = double.MinValue;
            BlobDescription MaxBlob = null;
            double area;
            double x, Radius;

            int MinWidth = 10;
            if (Width == 800)
                MinWidth = 50;

            List<BlobDescription> TestBlobs = new List<BlobDescription>();
            for (int i = 0; i < Blobs.Length; i++)
            {
                BlobDescription blob1 = Blobs[i];
                bool Found = false;
                for (int j = i + 1; j < Blobs.Length; j++)
                {
                    if (Blobs[j].BlobBounds.Left < blob1.CenterOfGravity.X && Blobs[j].BlobBounds.Right > blob1.CenterOfGravity.X && Blobs[j].CenterOfGravity.Y > blob1.CenterOfGravity.Y)
                    {
                        Found = true;
                        break;
                    }
                }
                if (!Found)
                {
                    TestBlobs.Add(blob1);
                }
            }


            Blobs = TestBlobs.ToArray();

            for (int i = 0; i < Blobs.Length; i++)
            {
                BlobDescription blob = Blobs[i];

                //check if it is not touching the border
                if (blob.BlobBounds.Left > 25 && blob.BlobBounds.Right < Width - 25)
                {
                    // if (blob.BlobBounds.Bottom < Height - 100 &&  blob.BlobBounds.Top > 100)
                    {
                        x = (Blobs[i].CenterOfGravity.X - half);
                        Radius = Math.Exp(-1 * x * x / Width);
                        area = Blobs[i].BlobBounds.Height * Blobs[i].BlobBounds.Width;
                        if (Blobs[i].BlobBounds.Width > MinWidth && Blobs[i].BlobBounds.Height > MinWidth)
                        {
                            if (area > 500)
                                NumBigBlobs++;
                            area = area * Radius;
                            if (area > MaxArea)
                            {
                                MaxBlob = Blobs[i];
                                MaxArea = area;
                            }
                        }
                    }
                }
            }
            //if it cannot be found go ahead and try for a smaller blob
            if (MaxBlob == null)
            {
                for (int i = 0; i < Blobs.Length; i++)
                {
                    BlobDescription blob = Blobs[i];
                    if (blob.BlobBounds.Left > 5 && blob.BlobBounds.Right < Width - 5)
                    {
                        // if (blob.BlobBounds.Bottom < Height - 100 &&  blob.BlobBounds.Top > 100)
                        {
                            x = (Blobs[i].CenterOfGravity.X - half);
                            Radius = Math.Exp(-1 * x * x / Width);
                            area = Blobs[i].BlobBounds.Height * Blobs[i].BlobBounds.Width;
                            {
                                if (area > 500)
                                    NumBigBlobs++;
                                area = area * Radius;
                                if (area > MaxArea)
                                {
                                    MaxBlob = Blobs[i];
                                    MaxArea = area;
                                }
                            }
                        }
                    }
                }
            }
            return MaxBlob;
        }

        public static BlobDescription SortBlobs(BlobDescription[] Blobs, ImageHolder SingleImage)
        {

            int Width = SingleImage.Width;
            int Height = SingleImage.Height;

            return SortBlobs(Blobs, Width, Height);

        }

        private static int NumBigBlobs = 0;
        private void SortBlobDescriptions(BlobDescription[] Blobs, int Width, int Height)
        {
            NumBigBlobs = 0;
            mPassData.AddSafe("MaxBlob", SortBlobs(Blobs,Width,Height) );
        }


    }
}
