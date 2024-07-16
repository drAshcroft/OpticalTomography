using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using MathHelpLib;

namespace ImageViewer.Filters.Effects.Flattening
{
    public class FlattenEdges1DIterative : aEffectNoForm
    {
        public override string EffectName { get { return "Flatten Edges 1D Iterative Mask"; } }
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
                return FlattenImageEdges((Bitmap)SourceImage);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                return FlattenImageEdges((ImageHolder)SourceImage);
            }
            return null;
        }

        public static double[,] FlattenImageEdges(double[,] array,double[,] Mask)
        {

            double[,] outArray = new double[array.GetLength(0), array.GetLength(1)];


            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);
            int ArrayBottom = arrayHeight - 1;

            double[,] Edges = new double[2,arrayWidth];

            int cc = 0;
            int j = 0;
            int gap = 10;
            bool  AllPixels = false  ;
            double Sum = 0;


            while (AllPixels==false )
            {
                AllPixels = true;
                for (int i = 0; i < arrayWidth; i++)
                {
                    cc = 0;
                    for (j = 0; j < gap ; j++)
                    {
                        if (Mask[j, i] > 0)
                        {
                            Edges[1,i] += array[i,j];
                            cc++;
                        }
                        if (Mask[j, i] > 0)
                        {
                            Edges[1,i] += array[i,ArrayBottom - j];
                            cc++;
                        }
                    }
                    if (cc == 0)
                    {
                        AllPixels = false;
                        gap = gap + 5;
                        break;
                    }
                    Edges[0, i] = i;
                    Edges[1,i] /= cc;
                }
                if (gap > (arrayWidth /2))
                    throw new Exception("Unable to flatten edges");
            }

            float[] EdgesSmooth = new float[arrayWidth];

            double[] coeff;
            MathHelpLib.CurveFitting.MathCurveFits.LinearRegressionPoly(Edges, 9, out coeff);


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
                        outArray[jj, i] = array[jj, i] - EdgesSmooth[jj];
                    }
                }
            }
            return outArray;
        }

        public static ImageHolder FlattenImageEdges(Bitmap SourceImage)
        {
            double[,] Image = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false);
            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(SourceImage);
            AForge.Imaging.Filters.IterativeThreshold Filter = new IterativeThreshold();
            Filter.ApplyInPlace (holding );
            double[,] Mask = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(holding, false);
            return  new ImageHolder( FlattenImageEdges(Image, Mask  ));
        }
        public static ImageHolder FlattenImageEdges(ImageHolder  SourceImage)
        {
            double[,] Image = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false);
            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(EffectHelps.ConvertToBitmap(SourceImage));
            AForge.Imaging.Filters.IterativeThreshold Filter = new IterativeThreshold();
          
            Filter.ApplyInPlace (holding );
            double[,] Mask = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(holding, false);
            return  new ImageHolder(   FlattenImageEdges(Image, Mask  ));
        }

    }
}
