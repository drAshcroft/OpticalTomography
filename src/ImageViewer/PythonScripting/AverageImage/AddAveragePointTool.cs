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

namespace ImageViewer.PythonScripting.AverageImage
{
    public class AddPointAverageTool : aEffectNoForm
    {
        public override string EffectName { get { return "Add Average Point"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Average Images"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        public override bool PassesPassData
        {
            get
            {
                return false;
            }
        }
        private static object CriticalSectionLock = new object();




        /// <summary>
        /// Adds an image or part of an image to an rolling average
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">global average name; image index as int; bounds of the average as rectangle</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            if (Parameters == null)
            {
                mFilterToken = DefaultProperties;
                string ArrayName = "DefaultArray";
                InputBox.ShowDialog("Add point to array", "Enter name of array", ref ArrayName);
                mFilterToken[0] = ArrayName;
            }
            string aName = (string)mFilterToken[0];
            int ImageIndex = (int)mFilterToken[1];
            

          //  try
            {
                if (mFilterToken[2].GetType() == typeof(Rectangle))
                {
                    AveragePartImage(aName, (Rectangle)mFilterToken[2], mFilterToken[3],dataEnvironment );
                }
                else
                {
                    AverageWholeImage(aName, mFilterToken[2],dataEnvironment );
                }
            }
           // catch (Exception Exception)
            {
              //  System.Diagnostics.Debug.Print(Exception.Message);
            }
            return SourceImage;
        }



        public static  object AveragePartImage(string aName, Rectangle Bounds, object image, DataEnvironment dataEnvironment)
        {
            Rectangle imageBounds = ImagingTools.GetImageBounds(image);
            if (Bounds.Width > imageBounds.Width)
                Bounds.Width = imageBounds.Width;
            if (Bounds.Height > imageBounds.Height)
                Bounds.Height = imageBounds.Height;

            double[,] data;

            lock (CriticalSectionLock)
            {
                ImageViewer.PythonScripting.AverageImage.CreateGlobalAverageTool.GlobalAverageDescription gad = CreateGlobalAverageTool.GetGlobalArray(aName, dataEnvironment);// ScriptGlobalAverages[aName];

                data = gad.Data;

                //if this is the first reference then load up the image and then return
                if (data == null)
                {
                    data = new double[Bounds.Height, Bounds.Width];
                    gad.Bounds = Bounds;
                    gad.Data = data;
                    gad.Locks = new object[data.GetLength(1)];
                    for (int i = 0; i < gad.Locks.Length; i++)
                        gad.Locks[i] = new object();
                }
                gad.PointCount++;


                AddToDoubleArray(image, ref gad.Data,gad.Bounds , gad.Locks);
            }

            return null;
        }

        public static  void AddToDoubleArray(object b, ref double[,] ExistData, Rectangle Bounds, object[] Locks)
        {
            if (b.GetType() == typeof(Bitmap))
            {
                AddToDoubleArray((Bitmap)b, ref ExistData, Bounds, Locks);
            }
            else if (b.GetType() == typeof(ImageHolder))
            {
                AddToDoubleArray((ImageHolder)b, ref ExistData, Bounds, Locks);
            }
            else if (b.GetType() == typeof(ImageHolder))
            {
                AddToDoubleArray((ImageHolder)b, ref ExistData, Bounds, Locks);
            }
        }

        public static  void AddToDoubleArray(ImageHolder b, ref double[,] ExistData, Rectangle Bounds, object[] Locks)
        {
            //double[,] ImageArray = new double[Image.Height, Image.Width];

            float [, ,] bmd = b.ImageData;
            int cX = 0, cY = 0;
            for (int x = Bounds.X; x < Bounds.Right; x++)
            {
                lock (Locks[cX])
                {
                    cY = 0;
                    for (int y = Bounds.Y; y < Bounds.Bottom; y++)
                    {
                        ExistData[cY, cX] += bmd[y, x, 0];
                        cY++;
                    }
                }
                cX++;
            }
        }

        public static  void AddToDoubleArray(Bitmap b, ref double[,] ExistData, Rectangle Bounds, object[] Locks)
        {

            //double[,] ImageArray = new double[Image.Width, Image.Height];

            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

            double g1, g2, g3;
            unsafe
            {
                for (int x = 0; x < Bounds.Width; x++)
                {
                    lock (Locks[x])
                    {
                        for (int y = 0; y < Bounds.Height; y++)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y + Bounds.Top) * bmd.Stride) + Bounds.Left + x;

                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            ExistData[y, x] += (g1 + g2 + g3) / 3d;
                        }
                    }
                }
            }
            b.UnlockBits(bmd);
        }

        private object AverageWholeImage(string aName, object image, DataEnvironment dataEnvironment)
        {
            double[,] data;
            ImageViewer.PythonScripting.AverageImage.CreateGlobalAverageTool.GlobalAverageDescription gad = CreateGlobalAverageTool.GetGlobalArray(aName, dataEnvironment);// ScriptGlobalAverages[aName];
            lock (CriticalSectionLock)
            {
                data = gad.Data;

                //if this is the first reference then load up the image and then return
                if (data == null)
                {
                    data = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(mFilterToken[2], false);
                    gad.Data = data;
                    gad.Locks = new object[data.GetLength(0)];
                    for (int i = 0; i < gad.Locks.Length; i++)
                        gad.Locks[i] = new object();
                    gad.PointCount = 1;
                    return null;
                }
            }

            double[,] nData = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(image, false);
            object[] Locks = gad.Locks;

            //it is easier to lock the data by row than to do anything more complicated.  
            //loose a little to the locking, but it should be ok with the number of processors
            for (int i = 0; i < data.GetLength(0); i++)
            {
                lock (gad.Locks[i])
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        data[i, j] += nData[i, j];
                    }
                }
            }
            gad.Data = data;
            gad.PointCount++;

            return null;
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { "DefaultArray", 0, 0d }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "ArrayName|string", "ImageIndex", "datapoint|double" }; }
        }

    }
}
