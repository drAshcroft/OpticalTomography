using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

using ImageViewer.Filters;
using ImageViewer;
using MathHelpLib;
using MathHelpLib.ProjectionFilters;
using ImageViewer.Filters.Blobs;
using System.Threading;

namespace MathHelpLib.ProjectionFilters
{
    public class CopyAndCutStackEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Move Stack to new location chopping down to cell"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Cell Tools"; } }

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
        /// Copies the stack from one location to another cutting with the same technique used to find the center cell in normal recons
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">Source Stack Directory as string, Destination Stack Directory as string; Cut stack images as bool; bool if image needs to be visongatefixed</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mDataEnvironment = dataEnvironment;
            mFilterToken = Parameters;
            mPassData = PassData;
            if (mFilterToken == null)
                mFilterToken = DefaultProperties;

            string SrcDir = (string)mFilterToken[0];
            string dstDir = (string)mFilterToken[1];
            bool CutStack = (bool)mFilterToken[2];
            bool ColorImage = (bool)mFilterToken[3];

            CopyStack(SrcDir, dstDir, CutStack, ColorImage);


            return SourceImage;
        }


        public static void CopyStack(string SrcStackDir, string DstStackDir, bool CutStack, bool ColorImage)
        {
            string StackFolder = SrcStackDir;

            string[] FileNames = Directory.GetFiles(StackFolder, "*.bmp");

            if (FileNames == null || FileNames.Length < 2)
                FileNames = Directory.GetFiles(StackFolder, "*.png");

            if (FileNames == null || FileNames.Length < 2)
                FileNames = Directory.GetFiles(StackFolder, "*.jpeg");

            if (FileNames == null || FileNames.Length < 2)
                FileNames = Directory.GetFiles(StackFolder, "*.ivg");
            Array.Sort(FileNames);


            double m = Math.Floor((double)(FileNames.Length / 2));
            int r = 0;

            string[] Files = new string[FileNames.Length];

            while (r < FileNames.Length)
            {
                Files[(int)m] = FileNames[r];
                r++;
                m += Math.Pow((-1), r + 1) * r;
            }

            ImageHolder BitmapImage = MathHelpsFileLoader.Load_Bitmap(Files[Files.Length / 2]);


            if (ColorImage == true)
            {
                BitmapImage = MathHelpsFileLoader.FixVisionGateImage(BitmapImage, 3);
            }

            Rectangle CellArea;
            if (CutStack)
            {
                ImageViewer.Filters.ReplaceStringDictionary PassData = new ReplaceStringDictionary();
                IEffect Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(null, BitmapImage, PassData);

                //WaterShed
                Filter = new ImageViewer.Filters.Blobs.WaterShedTool();
                BitmapImage = (ImageHolder)Filter.DoEffect(null, BitmapImage, PassData);
                //Data out of type :
                PassData = Filter.PassData;

                //Get Biggest Blob
                Filter = new ImageViewer.Filters.Blobs.GetBiggestCenterBlob();
                BitmapImage = (ImageHolder)Filter.DoEffect(null, BitmapImage, PassData, PassData["Blobs"], false);
                //Data out of type :
                PassData = Filter.PassData;
                BlobDescription Rect = (BlobDescription)PassData["MaxBlob"];
                int x = Rect.CenterOfGravity.X;
                int y = Rect.CenterOfGravity.Y;

                int CellSize = (int)(Rect.BlobBounds.Width * 2.5);
                double CellHalf = CellSize / 2;


                CellArea = new Rectangle((int)Math.Truncate(x - CellHalf), (int)Math.Truncate(y - CellHalf), CellSize, CellSize);
            }
            else
                CellArea = new Rectangle(0, 0, BitmapImage.Width, BitmapImage.Height);


            Thread[] Threads = new Thread[Files.Length];

            for (int q = 0; q < Files.Length; q++)
            {
                Threads[q] = new Thread(delegate(object Index)
                    {
                        int Index2 = (int)Index;
                        if (CutStack == false)
                        {
                            File.Copy(Files[Index2], DstStackDir + "\\" + Path.GetFileName(Files[Index2]), true);
                        }
                        else
                        {
                            ImageHolder a = MathHelpLib.MathHelpsFileLoader.Load_Bitmap(Files[Index2]);

                            if (ColorImage == true)
                            {
                                a = MathHelpLib.MathHelpsFileLoader.FixVisionGateImage(a, 2);
                            }

                            try
                            {
                                //Clip Image to New Image
                                ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect Filter = new ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect();
                                //Parameters required: Clip Bounds as Rectangle
                                a = (ImageHolder)Filter.DoEffect(null, a, null, CellArea);
                                a.Invert();
                                a.Save(DstStackDir + "\\" + Path.GetFileName(Files[Index2]));
                            }
                            catch { }
                        }
                    }
                );
                Threads[q].Start(q);
            }


            foreach (Thread t in Threads)
                t.Join();

        }


        public override object[] DefaultProperties
        {
            get { return new object[] { new Bitmap(1, 1) }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Image|image" }; }
        }

    }
}
