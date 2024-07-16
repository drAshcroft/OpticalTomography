using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using MathHelpLib;

namespace ImageViewer.Filters.Effects.Flattening
{
    public class DivideImage : aEffectNoForm
    {
        public override string EffectName { get { return "Divide image"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Flattening"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        /// Divides one image with another image of the same size
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">dividing image as bitmap or imageholder; optional rectanglular bounds as rectangle</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            // try
            {
                mPassData = PassData;
                double[,] Data;
                double[,] divisor = null;

                if (Parameters != null && Parameters[0] != null)
                {
                    if (Parameters[0].GetType() == typeof(ImageHolder))
                        divisor = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray((ImageHolder)Parameters[0], false);
                    else if (Parameters[0].GetType() == typeof(double[,]))
                        divisor = (double[,])Parameters[0];
                }
                else
                {
                    if (mPassData.ContainsKey("Background") && mPassData["Background"].GetType() == typeof(ImageHolder))
                    {
                        divisor = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray((ImageHolder)mPassData["Background"], false);
                    }
                    else if (mPassData.ContainsKey("Background") && mPassData["Background"].GetType() == typeof(double[,]))
                    {
                        divisor = (double[,])mPassData["Background"];
                    }
                }
                if (divisor != null)
                {
                    if (Parameters.Length > 1 && Parameters[1] != null)
                    {
                        Rectangle Bounds = (Rectangle)Parameters[1];

                        ImageHolder DataH;
                        if (SourceImage.GetType() == typeof(Bitmap))
                            DataH = new ImageHolder((Bitmap)SourceImage);
                        else
                            DataH = (ImageHolder)SourceImage;

                        return DivideOneImageByAnother(DataH, divisor, Bounds);
                    }
                    else
                    {
                        Data = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false);

                        return DivideOneImageByAnother(Data, divisor);
                    }
                }
                else
                    return SourceImage;
            }
            // catch(Exception ex)
            {
                //   System.Diagnostics.Debug.Print(ex.Message);
                // return SourceImage   ;
            }
        }

        /// <summary>
        /// Takes the top image and divides by the second image.  images must be the same size.  you are responsible for error checking
        /// </summary>
        /// <param name="Numerator"></param>
        /// <param name="Denominator"></param>
        /// <returns></returns>
        public static ImageHolder DivideOneImageByAnother(Bitmap  Numerator, ImageHolder Denominator)
        {
            ImageHolder DataH;

            DataH = new ImageHolder(Numerator);
            double[,] divisor = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(Denominator, false);
            return DivideOneImageByAnother(DataH, divisor);
        }
        /// <summary>
        /// Takes the top image and divides by the second image.  images must be the same size.  you are responsible for error checking
        /// </summary>
        /// <param name="Numerator"></param>
        /// <param name="Denominator"></param>
        /// <returns></returns>
        public static ImageHolder DivideOneImageByAnother(Bitmap Numerator, Bitmap  Denominator)
        {
            ImageHolder DataH;

            DataH = new ImageHolder(Numerator);
            double[,] divisor = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(Denominator, false);
            return DivideOneImageByAnother(DataH, divisor);
        }
        /// <summary>
        /// Takes the top image and divides by the second image.  images must be the same size.  you are responsible for error checking
        /// </summary>
        /// <param name="Numerator"></param>
        /// <param name="Denominator"></param>
        /// <returns></returns>
        public static ImageHolder DivideOneImageByAnother(ImageHolder Numerator, ImageHolder Denominator)
        {
            double[,] divisor = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(Denominator, false);
            return DivideOneImageByAnother(Numerator, divisor);
        }
        /// <summary>
        /// Takes the top image and divides by the second image.  images must be the same size.  you are responsible for error checking
        /// </summary>
        /// <param name="Numerator"></param>
        /// <param name="Denominator"></param>
        /// <returns></returns>
        public static ImageHolder DivideOneImageByAnother(ImageHolder Numerator, double[,] divisor)
        {

            double[,] Data = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(Numerator, false);

            return DivideOneImageByAnother(Data, divisor);
        }
        /// <summary>
        /// Takes the top image and divides by the second image.  images must be the same size.  you are responsible for error checking
        /// </summary>
        /// <param name="Numerator"></param>
        /// <param name="Denominator"></param>
        /// <returns></returns>
        public static ImageHolder DivideOneImageByAnother(double[,] Numerator, double[,] divisor)
        {

            if (Numerator.GetLength(0) != divisor.GetLength(0) || Numerator.GetLength(1) != divisor.GetLength(1))
                throw new Exception("Images are not the same size");

            double[,] Data = Numerator;

            ImageHolder bOut = new ImageHolder(Data.GetLength(1), Data.GetLength(0), 1);

            float[, ,] DataOut = bOut.ImageData;
            unsafe
            {
                fixed (float* pOut = DataOut)
                {
                    fixed (double* pNum = Data)
                    {
                        fixed (double* pDiv = divisor)
                        {
                            for (int i = 0; i < Data.Length; i++)
                            {
                                ///move through every pixel in the image, dividing one by the other and placing it into the resultant array
                                pOut[i] = (float)(pNum[i] / pDiv[i]);
                            }
                        }
                    }
                }
            }

            return bOut;
        }


        /// <summary>
        /// Divides only the area within the bounds command by the second image
        /// The numerator will be altered
        /// </summary>
        /// <param name="Numerator"></param>
        /// <param name="Denominator"></param>
        /// <param name="Bounds"></param>
        /// <returns></returns>
        public static ImageHolder DivideOneImageByAnother(Bitmap  Numerator, ImageHolder Denominator, Rectangle Bounds)
        {
            ImageHolder DataH;

            DataH = new ImageHolder(Numerator);
            double[,] divisor = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(Denominator, false);
            return DivideOneImageByAnother(DataH, divisor, Bounds);
        }
        /// <summary>
        /// Divides only the area within the bounds command by the second image
        /// The numerator will be altered
        /// </summary>
        /// <param name="Numerator"></param>
        /// <param name="Denominator"></param>
        /// <param name="Bounds"></param>
        /// <returns></returns>
        public static ImageHolder DivideOneImageByAnother(Bitmap Numerator, Bitmap  Denominator, Rectangle Bounds)
        {
            ImageHolder DataH;

            DataH = new ImageHolder(Numerator);
            double[,] divisor = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(Denominator, false);
            return DivideOneImageByAnother(DataH, divisor, Bounds);
        }
        /// <summary>
        /// Divides only the area within the bounds command by the second image
        /// The numerator will be altered
        /// </summary>
        /// <param name="Numerator"></param>
        /// <param name="Denominator"></param>
        /// <param name="Bounds"></param>
        /// <returns></returns>
        public static ImageHolder DivideOneImageByAnother(ImageHolder Numerator, ImageHolder Denominator, Rectangle Bounds)
        {
            double[,] divisor = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(Denominator, false);
            return DivideOneImageByAnother(Numerator, divisor, Bounds);
        }
        /// <summary>
        /// Divides only the area within the bounds command by the second image
        /// The numerator will be altered
        /// </summary>
        /// <param name="Numerator"></param>
        /// <param name="Denominator"></param>
        /// <param name="Bounds"></param>
        /// <returns></returns>
        public static ImageHolder DivideOneImageByAnother(ImageHolder Numerator, double[,] divisor, Rectangle Bounds)
        {
            ImageHolder DataH = Numerator;
            float[, ,] imageData = DataH.ImageData;
           
            //make sure that the bounds fall within the image. 
            //if they do not, alter them so that they land where they should with the outer edges at their original location
            if (Bounds.Top < 0)
            {
                int Bottom = Bounds.Bottom;
                Bounds.Y = 0;
                Bounds.Height = Bottom;
            }
            if (Bounds.Left < 0)
            {
                int right = Bounds.Right;
                Bounds.X = 0;
                Bounds.Width = right;
            }
            if (Bounds.Bottom > DataH.Height)
                Bounds.Height = DataH.Height - Bounds.Y;

            if (Bounds.Right > DataH.Width)
                Bounds.Width = DataH.Width - Bounds.X;

            ///do the division within the bounded section
            if (DataH.NChannels == 1)
            {
                if (imageData.GetLength(0) == divisor.GetLength(1))
                {
                    int iW = imageData.GetLength(1), iH = imageData.GetLength(0);
                    int dW = divisor.GetLength(0), dH = divisor.GetLength(1);
                   // try
                    {
                        for (int x = Bounds.Left; x < Bounds.Right; x++)
                            for (int y = Bounds.Top; y < Bounds.Bottom; y++)
                            {

                                imageData[y, x, 0] = (float)(imageData[y, x, 0] / divisor[x, y]);
                            }
                    }
                  //  catch
                    {
                   //     System.Diagnostics.Debug.Print("");
                    }
                }
                else
                {
                  //  try
                    {
                    for (int x = Bounds.Left; x < Bounds.Right; x++)
                        for (int y = Bounds.Top; y < Bounds.Bottom; y++)
                        {
                            imageData[y, x, 0] = (float)(imageData[y, x, 0] / divisor[y, x]);
                        }
                    }
                  //  catch
                    {
                   //     System.Diagnostics.Debug.Print("");
                    }
                }
            }
            else
            {
                for (int n = 0; n < DataH.NChannels; n++)
                    for (int x = Bounds.Left; x < Bounds.Right; x++)
                        for (int y = Bounds.Top; y < Bounds.Top; y++)
                        {
                            imageData[y, x, n] = (float)(imageData[y, x, n] / divisor[y, x]);
                        }
            }

            return DataH;

            
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
