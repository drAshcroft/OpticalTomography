using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using MathHelpLib;
using MathHelpLib.ImageProcessing;
using MathHelpLib.CurveFitting;

namespace ImageViewer.Filters.Effects.Flattening
{
    public class FlattenEdges1D : aEffectNoForm
    {
        public override string EffectName { get { return "Flatten Edges 1D"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Flattening"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "" }; }
        }

        /// <summary>
        /// Samples all the points on the edge of an image and then subtracts a place obtained by linear regression.
        /// This will create a mostly flat edge 
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">No Parameters</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                return FlattenEdges1D.FlattenImageEdges((Bitmap)SourceImage);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                return FlattenEdges1D.FlattenImageEdges((ImageHolder)SourceImage);
            }
            return null;
        }

        public static ImageHolder FlattenImageEdges(ImageHolder image)
        {

            //  image.Save(@"C:\Development\test.bmp");
            ImageHolder imageOut = new ImageHolder(image.Width, image.Height, image.NChannels);

            float[, ,] array = image.ImageData;
            float[, ,] outImage = imageOut.ImageData;

            // double[,] outArray = new double[image.Height, image.Width];

            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);
            int ArrayBottom = arrayHeight - 1;


            double[,] Edges = new double[2, arrayWidth];
            int j = 0;
            double Sum = 0;
            //get the right edge 
            for (int i = 0; i < arrayWidth; i++)
            {
                for (j = 0; j < 10; j++)
                {
                    Edges[1, i] += array[i, j, 0];
                    Edges[1, i] += array[i, ArrayBottom - j, 0];
                }
                Edges[1, i] /= 2f * j;
                Edges[0, i] = i;

            }

            float[] EdgesSmooth = new float[arrayWidth];

            double[] coeff;
            MathHelpLib.CurveFitting.MathCurveFits.LinearRegressionPoly(Edges, 11, out coeff);


            for (int i = 0; i < arrayWidth; i++)
            {
                Sum = 0;
                for (int n = 0; n < coeff.Length; n++)
                    Sum += Math.Pow(i, n) * coeff[n];
                EdgesSmooth[i] = (float)Sum;

            }


            Sum /= arrayHeight;
            unchecked
            {
                for (int i = 0; i < arrayHeight; i++)
                {
                    for (int jj = 0; jj < arrayWidth; jj++)
                    {
                        outImage[jj, i, 0] = array[jj, i, 0] - EdgesSmooth[jj];
                    }
                }
            }
            return imageOut;
        }

        public static ImageHolder FlattenImageEdgesMedian(ImageHolder image)
        {

            //  image.Save(@"C:\Development\test.bmp");
            ImageHolder imageOut = new ImageHolder(image.Width, image.Height, image.NChannels);

            float[, ,] array = image.ImageData;
            float[, ,] outImage = imageOut.ImageData;

            // double[,] outArray = new double[image.Height, image.Width];

            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);
            int ArrayBottom = arrayHeight - 1;


            List<float>[] Edges = new List<float>[arrayWidth];
            int j = 0;

            //get the right edge 
            for (int i = 0; i < arrayWidth; i++)
            {
                Edges[i] = new List<float>();
                for (j = 0; j < 35; j++)
                {
                    Edges[i].Add(array[i, j, 0]);
                    Edges[i].Add(array[i, ArrayBottom - j, 0]);
                }
                Edges[i].Sort();

            }

            float[] EdgesSmoothT = new float[arrayWidth];

            for (int i = 0; i < arrayWidth; i++)
            {
                EdgesSmoothT[i] = Edges[i][(int)(Edges[i].Count / 2f)];
            }

            float[] EdgesSmooth = new float[arrayWidth];
            EdgesSmooth[0] = EdgesSmoothT[0];
            EdgesSmooth[1] = EdgesSmoothT[1];
            for (int i = 2; i < arrayWidth - 2; i++)
            {
                EdgesSmooth[i] = (EdgesSmoothT[i - 2] + EdgesSmoothT[i - 1] + EdgesSmoothT[i] + EdgesSmoothT[i + 1] + EdgesSmoothT[i + 2]) / 5f;
            }
            EdgesSmooth[arrayWidth - 1] = EdgesSmoothT[arrayWidth - 1];
            EdgesSmooth[arrayWidth - 2] = EdgesSmoothT[arrayWidth - 2];
            unchecked
            {
                for (int jj = 0; jj < arrayWidth; jj++)
                {
                    float edgeval = EdgesSmooth[jj];
                    for (int i = 0; i < arrayHeight; i++)
                    {
                        outImage[jj, i, 0] = array[jj, i, 0] - edgeval;
                        // if (outImage[jj, i, 0] < 0) outImage[jj, i, 0] = 0;
                    }
                }
            }
            return imageOut;
        }

        public static ImageHolder FlattenImageEdgesMedianPoly(ImageHolder image, int PolyOrder)
        {

            //  image.Save(@"C:\Development\test.bmp");
            ImageHolder imageOut = new ImageHolder(image.Width, image.Height, image.NChannels);

            float[, ,] array = image.ImageData;
            float[, ,] outImage = imageOut.ImageData;

            // double[,] outArray = new double[image.Height, image.Width];

            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);
            int ArrayBottom = arrayHeight - 1;


            List<float>[] Edges = new List<float>[arrayWidth];
            int j = 0;

            //get the right edge 
            for (int i = 0; i < arrayWidth; i++)
            {
                Edges[i] = new List<float>();
                for (j = 0; j < 35; j++)
                {
                    Edges[i].Add(array[i, j, 0]);
                    Edges[i].Add(array[i, ArrayBottom - j, 0]);
                }
                Edges[i].Sort();

            }

            double[,] EdgesSmoothT = new double[2, arrayWidth + 20];

            int cc = 0;

            for (int i = 0; i < 10; i++)
            {
                EdgesSmoothT[0, cc] = -10 + i;
                EdgesSmoothT[1, cc] = Edges[0][(int)(Edges[i].Count / 2f)];
                cc++;
            }

            for (int i = 0; i < arrayWidth; i++)
            {
                EdgesSmoothT[0, cc] = i;
                EdgesSmoothT[1, cc] = Edges[i][(int)(Edges[i].Count / 2f)];
                cc++;
            }

            for (int i = 0; i < 10; i++)
            {
                EdgesSmoothT[0, cc] = arrayWidth + i;
                EdgesSmoothT[1, cc] = Edges[arrayWidth - 1][(int)(Edges[i].Count / 2f)];
                cc++;
            }



            double[] coeff;
            MathHelpLib.CurveFitting.MathCurveFits.LinearRegressionPoly(EdgesSmoothT, PolyOrder, out coeff);


            float[] EdgesSmooth = new float[arrayWidth];
            //  coeff[0] = 0;
            double Sum2 = 0;
            for (int i = 0; i < arrayWidth; i++)
            {
                Sum2 = 0;
                for (int n = 0; n < coeff.Length; n++)
                    Sum2 += Math.Pow(i, n) * coeff[n];
                EdgesSmooth[i] = (float)Sum2;
            }

            unchecked
            {
                for (int jj = 0; jj < arrayWidth; jj++)
                {
                    float edgeval = EdgesSmooth[jj];
                    for (int i = 0; i < arrayHeight; i++)
                    {
                        outImage[jj, i, 0] = array[jj, i, 0] - edgeval;
                        // if (outImage[jj, i, 0] < 0) outImage[jj, i, 0] = 0;
                    }
                }
            }
            return imageOut;
        }

        public static ImageHolder FlattenImageEdgesMedianMedian(ImageHolder image, int Order)
        {

            //  image.Save(@"C:\Development\test.bmp");
            ImageHolder imageOut = new ImageHolder(image.Width, image.Height, image.NChannels);

            float[, ,] array = image.ImageData;
            float[, ,] outImage = imageOut.ImageData;

            // double[,] outArray = new double[image.Height, image.Width];

            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);
            int ArrayBottom = arrayHeight - 1;

            List<float>[] Edges = new List<float>[arrayWidth];
            int j = 0;

            //get the right edge 
            for (int i = 0; i < arrayWidth; i++)
            {
                Edges[i] = new List<float>();
                for (j = 0; j < 35; j++)
                {
                    Edges[i].Add(array[i, j, 0]);
                    Edges[i].Add(array[i, ArrayBottom - j, 0]);
                }
                Edges[i].Sort();

            }

            float[] EdgesSmoothT = new float[arrayWidth];

            int cc = 0;
            for (int i = 0; i < arrayWidth; i++)
            {
                EdgesSmoothT[cc] = Edges[i][(int)(Edges[i].Count / 2f)];
                cc++;
            }

            float[] EdgesSmooth = EdgesSmoothT.MedianSmooth(Order);


            unchecked
            {
                for (int jj = 0; jj < arrayWidth; jj++)
                {
                    float edgeval = EdgesSmooth[jj];
                    for (int i = 0; i < arrayHeight; i++)
                    {
                        outImage[jj, i, 0] = array[jj, i, 0] - edgeval;
                        // if (outImage[jj, i, 0] < 0) outImage[jj, i, 0] = 0;
                    }
                }
            }
            return imageOut;
        }

        public static ImageHolder FlattenImageEdgesConstant(ImageHolder image)
        {

            //  image.Save(@"C:\Development\test.bmp");
            ImageHolder imageOut = new ImageHolder(image.Width, image.Height, image.NChannels);

            float[, ,] array = image.ImageData;
            float[, ,] outImage = imageOut.ImageData;

            // double[,] outArray = new double[image.Height, image.Width];

            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);
            int ArrayBottom = arrayHeight - 1;


            double EdgeSum = 0;
            long cc = 0;
            int otherSide = array.GetLength(1) - 1;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                if (array[i, 0, 0] != 0)
                {
                    EdgeSum += array[i, 0, 0];
                    cc++;
                }
                if (array[i, otherSide, 0] != 0)
                {
                    EdgeSum += array[i, otherSide, 0];
                    cc++;
                }

            }

            otherSide = array.GetLength(0) - 1;
            for (int i = 0; i < array.GetLength(1); i++)
            {
                if (array[0, i, 0] != 0)
                {
                    EdgeSum += array[0, i, 0];
                    cc++;
                }
                if (array[otherSide, i, 0] != 0)
                {
                    EdgeSum += array[otherSide, i, 0];
                    cc++;
                }
            }

            float edgeval = (float)(EdgeSum / cc);

            unchecked
            {
                for (int jj = 0; jj < arrayWidth; jj++)
                {
                    for (int i = 0; i < arrayHeight; i++)
                    {
                        outImage[jj, i, 0] = array[jj, i, 0] - edgeval;
                    }
                }
            }
            return imageOut;
        }


        public static ImageHolder FlattenImageAreaMedianPoly(ImageHolder image, int PolyOrder)
        {

            //  image.Save(@"C:\Development\test.bmp");
            ImageHolder imageOut = new ImageHolder(image.Width, image.Height, image.NChannels);

            float[, ,] array = image.ImageData;
            float[, ,] outImage = imageOut.ImageData;

            // double[,] outArray = new double[image.Height, image.Width];

            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);
            int ArrayBottom = arrayHeight - 1;


            List<float>[] Edges = new List<float>[arrayWidth];
            int j = 0;

            //get the right edge 
            for (int i = 0; i < arrayWidth; i++)
            {
                Edges[i] = new List<float>();
                for (j = 0; j < 15; j++)
                {
                    Edges[i].Add(array[i, j, 0]);
                    Edges[i].Add(array[i, ArrayBottom - j, 0]);
                    Edges[i].Add(array[i, (int)(ArrayBottom / 4d - j), 0]);
                    Edges[i].Add(array[i, (int)(3d * ArrayBottom / 4d - j), 0]);
                    Edges[i].Add(array[i, (int)(2d * ArrayBottom / 5d - j), 0]);
                }
                Edges[i].Sort();

            }

            double[,] EdgesSmoothT = new double[2, arrayWidth + 20];

            int cc = 0;

            for (int i = 0; i < 10; i++)
            {
                EdgesSmoothT[0, cc] = -10 + i;
                EdgesSmoothT[1, cc] = Edges[0][(int)(Edges[i].Count / 2f)];
                cc++;
            }

            for (int i = 0; i < arrayWidth; i++)
            {
                EdgesSmoothT[0, cc] = i;
                EdgesSmoothT[1, cc] = Edges[i][(int)(Edges[i].Count / 2f)];
                cc++;
            }

            for (int i = 0; i < 10; i++)
            {
                EdgesSmoothT[0, cc] = arrayWidth + i;
                EdgesSmoothT[1, cc] = Edges[arrayWidth - 1][(int)(Edges[i].Count / 2f)];
                cc++;
            }



            double[] coeff;
            MathHelpLib.CurveFitting.MathCurveFits.LinearRegressionPoly(EdgesSmoothT, PolyOrder, out coeff);


            float[] EdgesSmooth = new float[arrayWidth];
            coeff[0] = 0;
            double Sum2 = 0;
            for (int i = 0; i < arrayWidth; i++)
            {
                Sum2 = 0;
                for (int n = 0; n < coeff.Length; n++)
                    Sum2 += Math.Pow(i, n) * coeff[n];
                EdgesSmooth[i] = (float)Sum2;
            }

            unchecked
            {
                for (int jj = 0; jj < arrayWidth; jj++)
                {
                    float edgeval = EdgesSmooth[jj];
                    for (int i = 0; i < arrayHeight; i++)
                    {
                        outImage[jj, i, 0] = array[jj, i, 0] - edgeval;
                        // if (outImage[jj, i, 0] < 0) outImage[jj, i, 0] = 0;
                    }
                }
            }
            return imageOut;
        }


        public static double[,] FlattenImageEdges(double[,] array)
        {

            double[,] outArray = new double[array.GetLength(0), array.GetLength(1)];

            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);
            int ArrayBottom = arrayHeight - 1;

            double[] Edges = new double[arrayWidth];

            int j = 0;
            double Sum = 0;
            //get the right edge 
            for (int i = 0; i < arrayWidth; i++)
            {
                for (j = 0; j < 10; j++)
                {
                    Edges[i] += array[i, j];
                    Edges[i] += array[i, ArrayBottom - j];
                }
                Edges[i] /= j;
            }

            double[] EdgesSmooth = new double[arrayWidth];
            for (int i = 0; i < arrayWidth; i++)
            {
                int sJ = i - 2;
                int eJ = i + 2;
                if (sJ < 0) sJ = 0;
                if (eJ > arrayWidth) eJ = arrayWidth - 1;
                double BoxSum = 0;
                for (int jj = sJ; jj < eJ; jj++)
                {
                    BoxSum += Edges[jj];
                }
                EdgesSmooth[i] = BoxSum / (eJ - sJ);
                Sum += EdgesSmooth[i];
            }
            Sum /= arrayHeight;

            for (int i = 0; i < arrayHeight; i++)
            {
                for (int jj = 0; jj < arrayWidth; jj++)
                {
                    outArray[jj, i] = array[jj, i] - EdgesSmooth[jj];
                }
            }

            return outArray;
        }

        public static Bitmap FlattenImageEdges(Bitmap SourceImage)
        {
            return FlattenImageEdges(MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false)).MakeBitmap();
        }
    }
}
