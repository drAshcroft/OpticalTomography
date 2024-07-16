using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using MathHelpLib;
using Emgu.CV.Structure;

namespace ImageViewer.Filters.Adjustments
{
    public class downSampleEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Down Sample Image"; } }
        public override string EffectMenu { get { return "Adjustment"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 21; } }

        /// <summary>
        /// down samples the image by looking at the 4 nearest neighbors.  you get an image that is 1/2 the size
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">No parameters</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {

            ImageHolder source = EffectHelps.FixImageFormat(SourceImage);

            return DownSampleImage(source);
        }

        /// <summary>
        /// down samples the image by looking at the 4 nearest neighbors.  you get an image that is 1/2 the size
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns></returns>
        public static ImageHolder DownSampleImage(ImageHolder SourceImage)
        {
            Bitmap b = SourceImage.ToBitmap();

            ImageHolder outImage = new ImageHolder(SourceImage.Width / 2, SourceImage.Height / 2, SourceImage.NChannels);

            float[, ,] src = SourceImage.ImageData;
            float[, ,] dst = outImage.ImageData;

            for (int n = 0; n < outImage.NChannels; n++)
                for (int i = 0; i < dst.GetLength(0); i++)
                {
                    for (int j = 0; j < dst.GetLength(1); j++)
                    {
                        dst[i, j, n] = (src[i * 2, j * 2, n] + src[i * 2 + 1, j * 2, n] + src[i * 2, j * 2 + 1, n] + src[i * 2 + 1, j * 2 + 1, n]) / 4f;
                    }
                }

            b = outImage.ToBitmap();
            int w = b.Width;
            return outImage;
        }

        public static ImageHolder DownSampleImage(ImageHolder SourceImage, double Scale)
        {
            Emgu.CV.Image<Gray, Single> source = new Emgu.CV.Image<Gray, float>(SourceImage.ImageData);
            //return new ImageHolder(source.Resize(Scale, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Rotate(-90,new Gray(0)));
            return new ImageHolder(source.Resize(Scale, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR));
        }

        public static ImageHolder UpSampleImage(ImageHolder SourceImage, double Scale)
        {
            Emgu.CV.Image<Gray, Single> source = new Emgu.CV.Image<Gray, float>(SourceImage.ImageData);
            
            return new ImageHolder(source.Resize(Scale , Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR));
        }

        public static Bitmap  UpSampleImage(Bitmap SourceImage, double Scale)
        {
            Emgu.CV.Image<Gray, Single> source = new Emgu.CV.Image<Gray, float>(SourceImage);
            source.Resize(Scale, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            return source.ToBitmap();
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "" }; }
        }


    }
}
