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

namespace ImageViewer.Filters.Effects.Flattening
{
    public partial class AverageImagesTool : aEffectForm
    {
        public AverageImagesTool()
            : base()
        {

        }

        public override string EffectName { get { return "Average Images Tool"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Flattening"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 25;
            }
        }
       
        public override bool PassesPassData
        {
            get
            {
                return true  ;
            }
        }
        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }

        /// <summary>
        /// Takes a list of filenames and adds them together to make an average
        /// </summary>
        /// <param name="Filenames"></param>
        /// <returns></returns>
        public static ImageHolder AverageImages(string[] Filenames)
        {
           

            double[,] AveImage =  MathHelpsFileLoader.LoadStandardImage_Intensity(Filenames[0], false);
            double[,] instantImage;
            for (int i = 1; i < Filenames.Length; i++)
            {
                instantImage =  MathHelpsFileLoader.LoadStandardImage_Intensity(Filenames[i], false);
                for (int x = 0; x < AveImage.GetLength(0); x++)
                    for (int y = 0; y < AveImage.GetLength(1); y++)
                        AveImage[x, y] += instantImage[x, y];
               
            }
            double count = Filenames.Length;
            double Max = 0;
            for (int x = 0; x < AveImage.GetLength(0); x++)
                for (int y = 0; y < AveImage.GetLength(1); y++)
                {
                    AveImage[x, y] /= count;
                    if (AveImage[x, y] > Max) Max = AveImage[x, y];
                }

            return new ImageHolder(AveImage);
        }
       
        /// <summary>
        /// averages together a number of images that have been named in a string array to produce the average
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">string[] of filenames</param>
        /// <returns></returns>
        protected  override object  doEffect(DataEnvironment dataEnvironment, object SourceImage, 
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            if (mFilterToken == null || mFilterToken[0] == null)
            {
                System.Windows.Forms.OpenFileDialog sfd = new OpenFileDialog();

                //sfd.Filter = "AVI Files (*.avi) | *.avi";
                sfd.Multiselect = true;
                sfd.ShowDialog();
                mFilterToken = DefaultProperties;
                mFilterToken[0]= sfd.FileNames;
            }

            string[] Filename = (string[])mFilterToken[0];

            double[,] AveImage = MathHelpsFileLoader.LoadStandardImage_Intensity(Filename[0], false);
            double[,] instantImage;
            for (int i = 1; i < Filename.Length; i++)
            {
                instantImage = MathHelpsFileLoader. LoadStandardImage_Intensity(Filename[i], false);
                for (int x = 0; x < AveImage.GetLength(0); x++)
                    for (int y = 0; y < AveImage.GetLength(1); y++)
                        AveImage[x, y] += instantImage[x, y];
                if (i % 10 == 0)
                {
                    pictureDisplay1.Image = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToGrayScaleImage(AveImage, 1d / (double)i).ToBitmap();
                    pictureDisplay1.Invalidate();
                    Application.DoEvents();
                }
            }
            double count = Filename.Length;
            double Max=0;
            for (int x = 0; x < AveImage.GetLength(0); x++)
                for (int y = 0; y < AveImage.GetLength(1); y++)
                {
                    AveImage[x, y] /= count;
                    if (AveImage[x,y] > Max) Max = AveImage[x, y];
                }

            PassData.Add("AverageImage", AveImage);

            ImageHolder holding = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToGrayScaleImage(AveImage, 255d / Max);
            pictureDisplay1.Image = holding.ToBitmap();
            pictureDisplay1.Invalidate();
            return holding;
        }


       

       
    }
}
