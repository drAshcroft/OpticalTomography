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

namespace MathHelpLib.ProjectionFilters
{
    public class FocusValueTool : aEffectNoForm
    {
        public override string EffectName { get { return "Focus Value of Image"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Statistics"; } }

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

        /// <summary>
        /// Returns a number of focus values to determine the quality of this image
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns>"FocusValue""FocusValueMedian""FocusValueMostProb""FocusValueMax"</returns>
        public static double FocusValueF4(Bitmap SourceImage)
        {
            double[,] GrayImage = null;
            GrayImage = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false);
            return DoCalcF4(GrayImage);
        }

        /// <summary>
        /// Returns a number of focus values to determine the quality of this image
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns>"FocusValue""FocusValueMedian""FocusValueMostProb""FocusValueMax"</returns>
        public static double FocusValueF4(ImageHolder SourceImage, out double DistToCenter, out double X, out double Y)
        {
            if (SourceImage.NChannels > 1)
            {
                double[,] GrayImage = null;
                GrayImage = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false);
                DistToCenter = 0;
                X = 0;
                Y = 0;
                return DoCalcF4(GrayImage);
            }
            else
            {
                double Max = SourceImage.Max();
                double Min = SourceImage.Min();
                int MaxI = SourceImage.Height-2;
                int MaxJ = SourceImage.Width-2;

                double sum1 = 0;
                double sum2 = 0;
                double sumAve = 0;
                double d = 0;
                double cut = Max*1d/3d;
                long cc = 0;
                X = 0;
                Y = 0;
                for (int i = 0; i < MaxI; i++)
                {
                    for (int j = 0; j < MaxJ; j++)
                    {
                        if (SourceImage.ImageData[i, j, 0] > cut)
                        {
                            d = SourceImage.ImageData[i, j, 0];
                            sum1 += d * SourceImage.ImageData[i , j+1, 0];
                            sum2 += d * SourceImage.ImageData[i , j+2, 0];
                            sumAve += d*d;
                            X += i;
                            Y += j;
                            cc++;
                        }
                    }
                }

                if (cc != 0)
                {
                    X /= cc;
                    Y /= cc;
                    DistToCenter = Math.Sqrt((MaxI / 2 - X) * (MaxI / 2 - X) + (MaxJ / 2 - Y) * (MaxJ / 2 - Y)) / MaxI;
                }
                else
                    DistToCenter = 1;

                if (DistToCenter != DistToCenter)
                    DistToCenter = 0;

                sumAve = 1;// sumAve / cc;
               // Bitmap b = SourceImage.ToBitmap();
               // int w = b.Width;
                sum1 /= (sumAve * cc);
                sum2 /= (sumAve * cc);

                double f4 =Math.Abs (sum1 - sum2);
               // if (f4 < 0)
                //    System.Diagnostics.Debug.Print(" ");
                return f4*1000;// *(Max - Min) / Max;
            }
        }

        public static double FocusValueF4(double[,] SourceImage)
        {

                double Max = SourceImage.MaxArray();
                double Min = SourceImage.MinArray();
                int MaxI = SourceImage.GetLength(0) - 2;
                int MaxJ = SourceImage.GetLength(1) - 2;

                double sum1 = 0;
                double sum2 = 0;
                double sumAve = 0;
                double d = 0;
                double cut = (Max-Min) * 1d / 3d + Min;
                long cc = 0;
            
                for (int i = 0; i < MaxI; i++)
                {
                    for (int j = 0; j < MaxJ; j++)
                    {
                        if (SourceImage[i, j] > cut)
                        {
                            d = SourceImage[i, j];
                            sum1 += d * SourceImage[i, j + 1];
                            sum2 += d * SourceImage[i, j + 2];
                            sumAve += d*d;
                          
                            cc++;
                        }
                    }
                }
               
                sumAve =  sumAve / cc;
                if (cc == 0) { sumAve = 1; cc = 1; }
                // Bitmap b = SourceImage.ToBitmap();
                // int w = b.Width;
                sum1 /= (sumAve * cc);
                sum2 /= (sumAve * cc);

                double f4 = Math.Abs(sum1 - sum2);
                // if (f4 < 0)
                //    System.Diagnostics.Debug.Print(" ");
                return f4*1000;// *(Max - Min) / Max;
            
        }


        /// <summary>
        /// Returns a number of focus values to determine the quality of this image
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns>"FocusValue""FocusValueMedian""FocusValueMostProb""FocusValueMax"</returns>
        public static double FocusValueF4(ImageHolder SourceImage, Rectangle Region)
        {
            //if (SourceImage.NChannels > 1)
            {
                double[,] GrayImage = null;
                GrayImage = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false, Region);
                return DoCalcF4(GrayImage);
            }
            //else
            {
           //     throw new Exception("Not yet implemented");
            }
        }

        /// <summary>
        /// The focus values are determined from the fft of the image
        /// </summary>
        /// <param name="GrayImage"></param>
        /// <returns></returns>
        private static double DoCalcF4(double[,] GrayImage)
        {
            double Max = MaxArray(GrayImage);
            double Min = GrayImage.MinArray();
            int MaxI = GrayImage.GetLength(0);
            int MaxJ = GrayImage.GetLength(1) - 2;

            double sum1 = 0;
            double sum2 = 0;
            double d = 0;
            for (int i = 0; i < MaxI; i++)
            {
                for (int j = 0; j < MaxJ; j++)
                {
                    d = GrayImage[i, j];
                    sum1 += d * GrayImage[i, j + 1];
                    sum2 += d * GrayImage[i, j + 2];
                }
            }

            sum1 /= (Max * MaxI * MaxJ);
            sum2 /= (Max * MaxI * MaxJ);


            return (sum1 - sum2) * (Max - Min);
        }

        #region FFt Focus
        /// <summary>
        /// Returns a number of focus values to determine the quality of this image
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns>"FocusValue""FocusValueMedian""FocusValueMostProb""FocusValueMax"</returns>
        public static double[] FocusValueFFT(Bitmap SourceImage)
        {
            double[,] GrayImage = null;
            GrayImage = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false);
            return DoCalcFFT(GrayImage);
        }

        /// <summary>
        /// Returns a number of focus values to determine the quality of this image
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns>"FocusValue""FocusValueMedian""FocusValueMostProb""FocusValueMax"</returns>
        public static double[] FocusValueFFT(ImageHolder SourceImage)
        {
            double[,] GrayImage = null;
            GrayImage = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false);
            return DoCalcFFT(GrayImage);
        }

        /// <summary>
        /// The focus values are determined from the fft of the image
        /// </summary>
        /// <param name="GrayImage"></param>
        /// <returns></returns>
        private static double[] DoCalcFFT(double[,] GrayImage)
        {
            double[] FocusValues = new double[4];

            double Max = MaxArray(GrayImage);
            GrayImage = MathHelpLib.MathFFTHelps.FFTreal2real(GrayImage);
            double MinR = GrayImage.GetLength(0) / 2d * 5d / 8d;

            double sum = 0;
            double cc = 0;
            double d = 0;

            //sum all the upper half of the frequencies
            int HalfX = GrayImage.GetLength(0) / 2;
            int HalfY = GrayImage.GetLength(1) / 2;
            for (int i = 0; i < HalfX; i++)
                for (int j = 0; j < HalfY; j++)
                {

                    if ((i + j) > MinR)
                    {
                        d = GrayImage[i, j];
                        sum += d * d;
                        cc++;
                    }
                }
            FocusValues[0] = 1000 * Math.Sqrt(sum) / Max / cc;

            //get median of the focus values
            List<double> values = new List<double>();
            for (int i = 0; i < HalfX; i += 20)
                for (int j = 0; j < HalfY; j += 20)
                {

                    if ((i + j) > MinR)
                    {
                        d = GrayImage[i, j];
                        values.Add(Math.Abs(d));
                    }
                }
            values.Sort();

            FocusValues[1] = 1000 * values[values.Count / 2] / Max;

            try
            {
                //find the peak of the histogram
                int[] Bins = new int[31];
                double minBin = values[0];
                double MaxBin = values[values.Count - 1];
                double StepBin = (MaxBin - minBin) / 20d;
                for (int i = 0; i < values.Count; i++)
                {
                    try
                    {
                        Bins[(int)((values[i] - minBin) / StepBin)]++;
                    }
                    catch { }
                }
                int MostProbBin = int.MinValue;
                double MostProbValue = 0;
                for (int i = 0; i < Bins.Length; i++)
                {
                    if (Bins[i] > MostProbBin)
                    {
                        MostProbBin = Bins[i];
                        MostProbValue = i * StepBin + minBin;
                    }
                }

                FocusValues[2] = MostProbValue / Max;
                //get the average value
                FocusValues[3] = values[values.Count - 1] / Max;
            }
            catch { }
            return FocusValues;
        }

        #endregion


        /// <summary>
        /// takes the source image and then returns a number of values to evaluate the quality of the pseudo projections
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">none</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            mPassData = PassData;
            if (mFilterToken == null)
                mFilterToken = DefaultProperties;


            double[] FocusValues = null;

            if (SourceImage.GetType() == typeof(Bitmap))
            {
                FocusValues = FocusValueFFT((Bitmap)SourceImage);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                FocusValues = FocusValueFFT((ImageHolder)SourceImage);
            }


            mPassData.AddSafe("FocusValue", FocusValues[0]);

            mPassData.AddSafe("FocusValueMedian", FocusValues[1]);

            mPassData.AddSafe("FocusValueMostProb", FocusValues[2]);

            mPassData.AddSafe("FocusValueMax", FocusValues[3]);

            return SourceImage;
        }

        /// <summary>
        /// get the maximum value for an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe double MaxArray(double[,] array)
        {
            double max = double.MinValue;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (max < *pAOut)
                        max = *pAOut;
                    pAOut++;
                }
            }
            return max;
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
