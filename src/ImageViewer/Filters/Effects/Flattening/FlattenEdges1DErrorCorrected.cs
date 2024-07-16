using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using MathHelpLib;
using MathHelpLib.ImageProcessing;
using Emgu.CV.Structure;

namespace ImageViewer.Filters.Effects.Flattening
{
    public class FlattenEdges1DErrorCorrected : aEffectNoForm
    {
        public override string EffectName { get { return "Flatten Edges 1D Error Corrected"; } }
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
                return FlattenEdges1DErrorCorrected.FlattenImageEdgesBoxCar(new ImageHolder((Bitmap)SourceImage));
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                return FlattenEdges1DErrorCorrected.FlattenImageEdgesBoxCar((ImageHolder)SourceImage);
            }
            return null;
        }

        public static void FlattenImageEdgesGlobal(ImageHolder image)
        {
            float[, ,] array = image.ImageData;
            int count = 30;
            float[] samples = new float[count];
            float[] edge = new float[array.GetLength(0)];
            int step = (int)Math.Floor(array.GetLength(1) / (float)count) - 1;
            List<float> lsamp = new List<float>(samples);

            int X = 0, LL = 0;

            int Cut = (int)(count * 4f / 5f);
            //get the values for the first half
            try
            {
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    X = 0;
                    for (LL = 0; LL < count; LL++)
                    {
                        lsamp[LL] = array[i, X, 0];
                        X += step;
                    }

                    lsamp.Sort();
                    edge[i] = (lsamp[Cut - 2] + lsamp[Cut - 3] + lsamp[Cut - 4] + lsamp[Cut - 5]) / 4;
                }

                double offset1 = edge[1];

                int FirstMaxI = 0;
                int SecondMaxI = edge.Length;

                float firstMax = 0;
                float secondMax = 0;

                double diff;
                for (int i = 6; i < 200; i++)
                {
                    diff=Math.Abs( edge[i+6] - edge[i-6]);
                    if (diff > firstMax)
                    {
                        firstMax =(float) diff;
                        FirstMaxI = i;
                    }
                }

                for (int i = edge.Length -200; i < edge.Length-6; i++)
                {
                    diff = Math.Abs(edge[i + 6] - edge[i - 6]);
                    if (diff > secondMax)
                    {
                        secondMax =(float) diff;
                        SecondMaxI = i;
                    }
                }

                if (firstMax < 400)
                    FirstMaxI = 0;
                if (secondMax < 400)
                    SecondMaxI = edge.Length;


                float[] edgeOut = new float[edge.Length];
                double Sum2 = 0;
                float MaxEdge = 0;
                double[] coeff;

                //curve fit the first section
                int cc = 0;
                double[,] EdgesSmooth1 = new double[2, FirstMaxI];
                #region first section
                try
                {
                    for (int i = 0; i < FirstMaxI; i++)
                    {
                        EdgesSmooth1[0, cc] = i;
                        EdgesSmooth1[1, cc] = edge[i] - offset1;
                        cc++;
                    }


                    if (EdgesSmooth1.GetLength(1) > 18)
                    {
                        MathHelpLib.CurveFitting.MathCurveFits.LinearRegressionPoly(EdgesSmooth1, 5, out coeff);

                        for (int i = 0; i < FirstMaxI; i++)
                        {
                            Sum2 = offset1;
                            for (int n = 0; n < coeff.Length; n++)
                                Sum2 += Math.Pow(i, n) * coeff[n];

                            //  System.Diagnostics.Debug.Print(edge[i] + "\t" + Sum2);
                            edgeOut[i] = (float)Sum2;
                            if (edgeOut[i] > MaxEdge) MaxEdge = edgeOut[i];
                        }
                    }
                    else
                    {
                        if (FirstMaxI > 1)
                        {
                            double ave = 0;
                            for (int i = 0; i < FirstMaxI; i++)
                            {
                                ave += EdgesSmooth1[1, i];
                            }

                            ave /= (FirstMaxI);
                            ave += offset1;
                            for (int i = 0; i < FirstMaxI; i++)
                            {

                                edgeOut[i] = (float)ave;
                                if (edgeOut[i] > MaxEdge) MaxEdge = edgeOut[i];
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
                #endregion

                #region Middle Section
                  try
                {
                    //curve fit the middle
                    cc = 0;
                    EdgesSmooth1 = new double[2, SecondMaxI - FirstMaxI];
                    for (int i = FirstMaxI; i < SecondMaxI; i++)
                    {
                        EdgesSmooth1[0, cc] = i;
                        EdgesSmooth1[1, cc] = edge[i] - offset1;
                        cc++;
                    }

                    if (EdgesSmooth1.GetLength(1) > 18)
                    {
                        MathHelpLib.CurveFitting.MathCurveFits.LinearRegressionPoly(EdgesSmooth1, 16, out coeff);

                        for (int i = FirstMaxI; i < SecondMaxI; i++)
                        {
                            Sum2 = offset1;
                            for (int n = 0; n < coeff.Length; n++)
                                Sum2 += Math.Pow(i, n) * coeff[n];

                            //  System.Diagnostics.Debug.Print(edge[i] + "\t" + Sum2);
                            edgeOut[i] = (float)Sum2;
                            if (edgeOut[i] > MaxEdge) MaxEdge = edgeOut[i];
                        }
                    }
                    else
                    {
                        if (SecondMaxI - FirstMaxI > 1)
                        {
                            double ave = 0;
                            for (int i = 0; i < EdgesSmooth1.GetLength(1); i++)
                            {
                                ave += EdgesSmooth1[1, i];
                            }
                            ave /= (SecondMaxI - FirstMaxI);
                            ave += offset1;
                            for (int i = FirstMaxI; i < SecondMaxI; i++)
                            {

                                edgeOut[i] = (float)ave;
                                if (edgeOut[i] > MaxEdge) MaxEdge = edgeOut[i];
                            }
                        }

                    }
                }
                  catch 
                  {

                      System.Diagnostics.Debug.Print(" ");
                  }
                #endregion

                #region End Section
                 try
                {
                    //curve fit the end
                    cc = 0;
                    EdgesSmooth1 = new double[2, edge.Length - SecondMaxI];
                    for (int i = SecondMaxI; i < edge.Length; i++)
                    {
                        EdgesSmooth1[0, cc] = i;
                        EdgesSmooth1[1, cc] = edge[i] - offset1;
                        cc++;
                    }

                    if (EdgesSmooth1.GetLength(1) > 18)
                    {
                        MathHelpLib.CurveFitting.MathCurveFits.LinearRegressionPoly(EdgesSmooth1, 5, out coeff);

                        for (int i = SecondMaxI; i < edge.Length; i++)
                        {
                            Sum2 = offset1;
                            for (int n = 0; n < coeff.Length; n++)
                                Sum2 += Math.Pow(i, n) * coeff[n];

                            //  System.Diagnostics.Debug.Print(edge[i] + "\t" + Sum2);
                            edgeOut[i] = (float)Sum2;
                            if (edgeOut[i] > MaxEdge) MaxEdge = edgeOut[i];
                        }
                    }
                    else
                    {
                        if (edge.Length - SecondMaxI > 1)
                        {
                            double ave = 0;
                            for (int i = 0; i < EdgesSmooth1.GetLength(1); i++)
                            {
                                ave += EdgesSmooth1[1, i];
                            }
                            ave /= (edge.Length - SecondMaxI);
                            ave += offset1;
                            for (int i = SecondMaxI; i < edge.Length; i++)
                            {
                                edgeOut[i] = (float)ave;
                                if (edgeOut[i] > MaxEdge) MaxEdge = edgeOut[i];
                            }
                        }
                    }
                }
                 catch {
                     System.Diagnostics.Debug.Print(" ");
                 }
                #endregion

                //check for nan
                if (MaxEdge != MaxEdge)
                {
                    FlattenImageEdgesGlobalO(image);
                    return;
                }

                for (int i = 0; i < edgeOut.Length; i++)
                {
                   // System.Diagnostics.Debug.Print(edge[i] + "\t" + edgeOut[i]);
                    edgeOut[i] /= MaxEdge;
                    //edge2[i] /= MaxEdge;
                }

                int arrayHeight = array.GetLength(0);
                int arrayWidth = array.GetLength(1);
                double sum = 0;
                unchecked
                {
                    float Offset = (float)Math.Pow(2, 16);
                    for (int jj = 0; jj < arrayHeight; jj++)
                    {
                        for (int i = 0; i < arrayWidth; i++)
                        {
                            //  float u = (i-image.Width/4)
                            array[jj, i, 0] = array[jj, i, 0] / edgeOut[jj];// +Offset;
                            //   sum += array[jj, i, 0];
                        }
                    }
                }


                //  sum /= (arrayHeight * arrayWidth);
                //  if (sum == 0 || double.IsNaN(sum) || double.IsInfinity(sum) || (sum<50000) || sum >70000)
                //     System.Diagnostics.Debug.Print(" ");


                //  System.Diagnostics.Debug.Print(sum.ToString());
            }
            catch (Exception ex)
            {
                FlattenImageEdgesGlobalO(image);
                // MessageBox.Show(ex.Message + "\n" + image.Height + "\n" + image.Width + "\n" + X + "\n" + LL + "\n" + lsamp.Count + "\n" + edge.Length + "\n" + ex.StackTrace );

            }

            // Bitmap b = image.ToBitmap();
            // int w = b.Width;
            //  return imageOut;
        }


        public static void FlattenImageEdgesGlobalOO(ImageHolder image)
        {
            float[, ,] array = image.ImageData;
            int count = 30;
            float[] samples = new float[count];
            float[] edge = new float[array.GetLength(0)];
            int step = (int)Math.Floor(array.GetLength(1) / (float)count) - 1;
            List<float> lsamp = new List<float>(samples);

            int X = 0, LL = 0;

            int Cut = (int)(count * 4f / 5f);
            //get the values for the first half
            try
            {
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    X = 0;
                    for (LL = 0; LL < count; LL++)
                    {
                        lsamp[LL] = array[i, X, 0];
                        X += step;
                    }

                    lsamp.Sort();
                    edge[i] = (lsamp[Cut - 2] + lsamp[Cut - 3] + lsamp[Cut - 4] + lsamp[Cut - 5]) / 4;
                }

                double offset1 = edge[1];

                int FirstMaxI = 0;
                int SecondMaxI = edge.Length;

                float firstMax = 0;
                float secondMax = 0;

                for (int i = 0; i < edge.Length / 2; i++)
                {
                    if (edge[i] > firstMax)
                    {
                        firstMax = edge[i];
                        FirstMaxI = i;
                    }
                }

                for (int i = edge.Length / 2; i < edge.Length; i++)
                {
                    if (edge[i] > secondMax)
                    {
                        secondMax = edge[i];
                        SecondMaxI = i;
                    }
                }

                float[] edgeOut = new float[edge.Length];
                double Sum2 = 0;
                float MaxEdge = 0;
                double[] coeff;

                //curve fit the first section
                int cc = 0;
                double[,] EdgesSmooth1 = new double[2, FirstMaxI];
                #region first section
                try
                {
                    for (int i = 0; i < FirstMaxI; i++)
                    {
                        EdgesSmooth1[0, cc] = i;
                        EdgesSmooth1[1, cc] = edge[i] - offset1;
                        cc++;
                    }


                    if (EdgesSmooth1.GetLength(1) > 18)
                    {
                        MathHelpLib.CurveFitting.MathCurveFits.LinearRegressionPoly(EdgesSmooth1, 5, out coeff);

                        for (int i = 0; i < FirstMaxI; i++)
                        {
                            Sum2 = offset1;
                            for (int n = 0; n < coeff.Length; n++)
                                Sum2 += Math.Pow(i, n) * coeff[n];

                            //  System.Diagnostics.Debug.Print(edge[i] + "\t" + Sum2);
                            edgeOut[i] = (float)Sum2;
                            if (edgeOut[i] > MaxEdge) MaxEdge = edgeOut[i];
                        }
                    }
                    else
                    {
                        if (FirstMaxI > 1)
                        {
                            double ave = 0;
                            for (int i = 0; i < FirstMaxI; i++)
                            {
                                ave += EdgesSmooth1[1, i];
                            }

                            ave /= (FirstMaxI);
                            ave += offset1;
                            for (int i = 0; i < FirstMaxI; i++)
                            {

                                edgeOut[i] = (float)ave;
                                if (edgeOut[i] > MaxEdge) MaxEdge = edgeOut[i];
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
                #endregion

                #region Middle Section
                //  try
                {
                    //curve fit the middle
                    cc = 0;
                    EdgesSmooth1 = new double[2, SecondMaxI - FirstMaxI];
                    for (int i = FirstMaxI; i < SecondMaxI; i++)
                    {
                        EdgesSmooth1[0, cc] = i;
                        EdgesSmooth1[1, cc] = edge[i] - offset1;
                        cc++;
                    }

                    if (EdgesSmooth1.GetLength(1) > 18)
                    {
                        MathHelpLib.CurveFitting.MathCurveFits.LinearRegressionPoly(EdgesSmooth1, 16, out coeff);

                        for (int i = FirstMaxI; i < SecondMaxI; i++)
                        {
                            Sum2 = offset1;
                            for (int n = 0; n < coeff.Length; n++)
                                Sum2 += Math.Pow(i, n) * coeff[n];

                            //  System.Diagnostics.Debug.Print(edge[i] + "\t" + Sum2);
                            edgeOut[i] = (float)Sum2;
                            if (edgeOut[i] > MaxEdge) MaxEdge = edgeOut[i];
                        }
                    }
                    else
                    {
                        if (SecondMaxI - FirstMaxI > 1)
                        {
                            double ave = 0;
                            for (int i = FirstMaxI; i < SecondMaxI; i++)
                            {
                                ave += EdgesSmooth1[1, i];
                            }
                            ave /= (SecondMaxI - FirstMaxI);
                            ave += offset1;
                            for (int i = FirstMaxI; i < SecondMaxI; i++)
                            {

                                edgeOut[i] = (float)ave;
                                if (edgeOut[i] > MaxEdge) MaxEdge = edgeOut[i];
                            }
                        }

                    }
                }
                //  catch { }
                #endregion

                #region End Section
                // try
                {
                    //curve fit the end
                    cc = 0;
                    EdgesSmooth1 = new double[2, edge.Length - SecondMaxI];
                    for (int i = SecondMaxI; i < edge.Length; i++)
                    {
                        EdgesSmooth1[0, cc] = i;
                        EdgesSmooth1[1, cc] = edge[i] - offset1;
                        cc++;
                    }

                    if (EdgesSmooth1.GetLength(1) > 18)
                    {
                        MathHelpLib.CurveFitting.MathCurveFits.LinearRegressionPoly(EdgesSmooth1, 5, out coeff);

                        for (int i = SecondMaxI; i < edge.Length; i++)
                        {
                            Sum2 = offset1;
                            for (int n = 0; n < coeff.Length; n++)
                                Sum2 += Math.Pow(i, n) * coeff[n];

                            //  System.Diagnostics.Debug.Print(edge[i] + "\t" + Sum2);
                            edgeOut[i] = (float)Sum2;
                            if (edgeOut[i] > MaxEdge) MaxEdge = edgeOut[i];
                        }
                    }
                    else
                    {
                        if (edge.Length - SecondMaxI > 1)
                        {
                            double ave = 0;
                            for (int i = SecondMaxI; i < edge.Length; i++)
                            {
                                ave += EdgesSmooth1[1, i];
                            }
                            ave /= (edge.Length - SecondMaxI);
                            ave += offset1;
                            for (int i = SecondMaxI; i < edge.Length; i++)
                            {
                                edgeOut[i] = (float)ave;
                                if (edgeOut[i] > MaxEdge) MaxEdge = edgeOut[i];
                            }
                        }
                    }
                }
                // catch { }
                #endregion

                //check for nan
                if (MaxEdge != MaxEdge)
                {
                    FlattenImageEdgesGlobalO(image);
                    return;
                }

                for (int i = 0; i < edgeOut.Length; i++)
                {
                          System.Diagnostics.Debug.Print(edge[i] + "\t" + edgeOut[i]);
                    edgeOut[i] /= MaxEdge;
                    //edge2[i] /= MaxEdge;
                }

                int arrayHeight = array.GetLength(0);
                int arrayWidth = array.GetLength(1);
                double sum = 0;
                unchecked
                {
                    float Offset = (float)Math.Pow(2, 16);
                    for (int jj = 0; jj < arrayHeight; jj++)
                    {
                        for (int i = 0; i < arrayWidth; i++)
                        {
                            //  float u = (i-image.Width/4)
                            array[jj, i, 0] = array[jj, i, 0] / edgeOut[jj];// +Offset;
                            //   sum += array[jj, i, 0];
                        }
                    }
                }


                //  sum /= (arrayHeight * arrayWidth);
                //  if (sum == 0 || double.IsNaN(sum) || double.IsInfinity(sum) || (sum<50000) || sum >70000)
                //     System.Diagnostics.Debug.Print(" ");


                //  System.Diagnostics.Debug.Print(sum.ToString());
            }
            catch (Exception ex)
            {
                FlattenImageEdgesGlobalO(image);
                // MessageBox.Show(ex.Message + "\n" + image.Height + "\n" + image.Width + "\n" + X + "\n" + LL + "\n" + lsamp.Count + "\n" + edge.Length + "\n" + ex.StackTrace );

            }

            // Bitmap b = image.ToBitmap();
            // int w = b.Width;
            //  return imageOut;
        }

        public static void FlattenImageEdgesGlobalO(ImageHolder image)
        {
            float[, ,] array = image.ImageData;
            int count = 30;
            float[] samples = new float[count];
            float[] edge = new float[image.Height];
            int step = (int)Math.Floor(image.Width / (float)count);
            List<float> lsamp = new List<float>(samples);

            //get the values for the first half
            for (int i = 0; i < image.Height; i++)
            {
                int X = 0;
                for (int j = 0; j < count; j++)
                {
                    lsamp[j] = array[i, X, 0];
                    X += step;
                }

                lsamp.Sort();
                edge[i] = (lsamp[count - 1] + lsamp[count - 2] + lsamp[count - 3] + lsamp[count - 4]) / 4;
            }
            double offset1 = edge[1];
            double[,] EdgesSmooth1 = new double[2, image.Height];
            for (int i = 0; i < image.Height; i++)
            {
                EdgesSmooth1[0, i] = i;
                EdgesSmooth1[1, i] = edge[i] - offset1;
            }
            double[] coeff1;
            MathHelpLib.CurveFitting.MathCurveFits.LinearRegressionPoly(EdgesSmooth1, 15, out coeff1);




            double Sum2 = 0;
            float MaxEdge = 0;
            for (int i = 0; i < image.Height; i++)
            {
                Sum2 = offset1;
                for (int n = 0; n < coeff1.Length; n++)
                    Sum2 += Math.Pow(i, n) * coeff1[n];

                System.Diagnostics.Debug.Print(edge[i] + "\t" + Sum2);
                edge[i] = (float)Sum2;
                if (edge[i] > MaxEdge) MaxEdge = edge[i];
            }




            for (int i = 0; i < edge.Length; i++)
            {
                edge[i] /= MaxEdge;
                //edge2[i] /= MaxEdge;
            }

            int arrayHeight = image.Height;
            int arrayWidth = image.Width;
            unchecked
            {
                float Offset = (float)Math.Pow(2, 16);
                for (int jj = 0; jj < arrayHeight; jj++)
                {
                    for (int i = 0; i < arrayWidth; i++)
                    {
                        //  float u = (i-image.Width/4)
                        array[jj, i, 0] = array[jj, i, 0] / edge[jj];// +Offset;
                    }
                }
            }

            // Bitmap b = image.ToBitmap();
            // int w = b.Width;
            //  return imageOut;
        }


        public static void FlattenImageEdgesGlobalO(double[,] array)
        {
           
            int count = 30;
            double[] samples = new double[count];
            double[] edge = new double[array.GetLength(0)];
            int step = (int)Math.Floor(array.GetLength(1) / (float)count)-1;
            List<double> lsamp = new List<double>(samples);

            //get the values for the first half
            for (int i = 0; i < array.GetLength(0); i++)
            {
                int X = 0;
                for (int j = 0; j < count; j++)
                {
                    lsamp[j] = array[i, X];
                    X += step;
                }

                lsamp.Sort();
                edge[i] = (lsamp[count - 1] + lsamp[count - 2] + lsamp[count - 3] + lsamp[count - 4]) / 4;
            }
            double offset1 = edge[1];
            double[,] EdgesSmooth1 = new double[2, array.GetLength(0)];
            for (int i = 0; i <array.GetLength(0); i++)
            {
                EdgesSmooth1[0, i] = i;
                EdgesSmooth1[1, i] = edge[i] - offset1;
            }
            double[] coeff1;
            MathHelpLib.CurveFitting.MathCurveFits.LinearRegressionPoly(EdgesSmooth1, 25, out coeff1);




            double Sum2 = 0;
            double MaxEdge = 0;
            string junk = "";
           
            for (int i = 0; i < array.GetLength(0); i++)
            {
                Sum2 = offset1;
                for (int n = 0; n < coeff1.Length; n++)
                    Sum2 += Math.Pow(i, n) * coeff1[n];

               // junk +=edge[i] + "\t" + Sum2 + "\n";
                edge[i] = (float)Sum2;
                if (edge[i] > MaxEdge) MaxEdge = edge[i];
            }
           // System.Diagnostics.Debug.Print(junk);



            for (int i = 0; i < edge.Length; i++)
            {
                edge[i] /= MaxEdge;
                //edge2[i] /= MaxEdge;
            }

            int arrayHeight = array.GetLength(0);
            int arrayWidth = array.GetLength(1);
            unchecked
            {
                float Offset = (float)Math.Pow(2, 16);
                for (int jj = 0; jj < arrayHeight; jj++)
                {
                    for (int i = 0; i < arrayWidth; i++)
                    {
                        //  float u = (i-image.Width/4)
                        array[jj, i] = array[jj, i] / edge[jj];// +Offset;
                    }
                }
            }

            // Bitmap b = image.ToBitmap();
            // int w = b.Width;
            //  return imageOut;
        }



        public static ImageHolder FlattenImageEdges(ImageHolder image)
        {

            //  image.Save(@"C:\Development\test.bmp");
            ImageHolder imageOut = new ImageHolder(image.Width, image.Height, 1);

            float[, ,] array = image.ImageData;
            float[, ,] outImage = imageOut.ImageData;

            // double[,] outArray = new double[image.Height, image.Width];

            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);
            int ArrayBottom = arrayHeight - 1;


            double[,] Edges = new double[2, arrayWidth];
            //get the right edge 
            for (int i = 0; i < arrayWidth; i++)
            {
                double Sum = 0;
                int cc = 0;

                for (int j = 0; j < 10; j++)
                {
                    Sum += array[i, ArrayBottom - j, 0];
                    cc++;
                }
                Edges[1, i] = Sum / cc;
                Edges[0, i] = i;
            }

            //get the left edge 
            for (int i = 0; i < arrayWidth; i++)
            {
                double Sum = 0;
                int cc = 0;

                for (int j = 0; j < 10; j++)
                {
                    Sum += array[i, j, 0];
                    cc++;
                }
                Sum /= cc;
                if (Sum > Edges[1, i]) Edges[1, i] = Sum;
            }

            //get the middle left edge 

            int StartMid = (int)(ArrayBottom * .25);
            for (int i = 0; i < arrayWidth; i++)
            {
                double Sum = 0;
                int cc = 0;

                for (int j = 0; j < 10; j++)
                {
                    Sum += array[i, StartMid + j, 0];
                    cc++;
                }
                Sum /= cc;
                if (Sum > Edges[1, i]) Edges[1, i] = Sum;
            }


            StartMid = (int)(ArrayBottom * .75);
            for (int i = 0; i < arrayWidth; i++)
            {
                double Sum = 0;
                int cc = 0;

                for (int j = 0; j < 10; j++)
                {
                    Sum += array[i, StartMid + j, 0];
                    cc++;
                }
                Sum /= cc;
                if (Sum > Edges[1, i]) Edges[1, i] = Sum;
            }

            float[] EdgesSmooth = new float[arrayWidth];

            double[] coeff;
            MathHelpLib.CurveFitting.MathCurveFits.LinearRegressionPoly(Edges, 9, out coeff);

            coeff[0] = 0;
            double Sum2 = 0;
            for (int i = 0; i < arrayWidth; i++)
            {
                Sum2 = 0;
                for (int n = 0; n < coeff.Length; n++)
                    Sum2 += Math.Pow(i, n) * coeff[n];
                EdgesSmooth[i] = (float)Sum2;

            }

            Sum2 /= arrayHeight;
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

        private static double[,] GetLine(float[, ,] Array, int StartIndex, int ArrayWidth, int ArrayHeight, int Depth, double[,] Edges, ref  double[] Count, float[, ,] Mask)
        {
            int cBlacks = 0;
            for (int i = 0; i < ArrayWidth; i++)
            {
                if (Mask[i, StartIndex + 1, 0] == 0)
                {
                    cBlacks++;
                }
            }

            if (cBlacks > 75)
                return Edges;

            for (int i = 0; i < ArrayWidth; i++)
            {
                double Sum = 0;
                int cc = 0;
                for (int j = 0; j < Depth; j++)
                {
                    if (Mask[i, StartIndex + j, 0] > 0)
                    {
                        Sum += Array[i, StartIndex + j, 0];
                        cc++;
                    }
                }

                Edges[1, i] += Sum;
                Count[i] += cc;
            }
            return Edges;
        }
        public static ImageHolder FlattenImageEdgesBoxCar(ImageHolder image)
        {
            ImageHolder mask = ImageViewer.Filters.Thresholding.IterativeThresholdEffect.IterativeThreshold(image);

            //  image.Save(@"C:\Development\test.bmp");
            ImageHolder imageOut = new ImageHolder(image.Width, image.Height, 1);

            float[, ,] array = image.ImageData;
            float[, ,] outImage = imageOut.ImageData;
            float[, ,] maskArray = mask.ImageData;

            // double[,] outArray = new double[image.Height, image.Width];

            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);
            int ArrayBottom = arrayHeight - 1;
            const int Depth = 30;

            double[,] Edges = new double[2, arrayWidth];
            double[] Count = new double[arrayWidth];

            Edges = GetLine(array, 0, arrayWidth, arrayHeight, Depth, Edges, ref  Count, maskArray);
            Edges = GetLine(array, (int)(arrayHeight * .1), arrayWidth, arrayHeight, Depth, Edges, ref  Count, maskArray);
            Edges = GetLine(array, (int)(arrayHeight * .2), arrayWidth, arrayHeight, Depth, Edges, ref  Count, maskArray);
            Edges = GetLine(array, (int)(arrayHeight * .3), arrayWidth, arrayHeight, Depth, Edges, ref  Count, maskArray);
            Edges = GetLine(array, (int)(arrayHeight * .4), arrayWidth, arrayHeight, Depth, Edges, ref  Count, maskArray);
            Edges = GetLine(array, (int)(arrayHeight * .5), arrayWidth, arrayHeight, Depth, Edges, ref  Count, maskArray);
            Edges = GetLine(array, (int)(arrayHeight * .6), arrayWidth, arrayHeight, Depth, Edges, ref  Count, maskArray);
            Edges = GetLine(array, (int)(arrayHeight * .7), arrayWidth, arrayHeight, Depth, Edges, ref  Count, maskArray);
            Edges = GetLine(array, (int)(arrayHeight * .8), arrayWidth, arrayHeight, Depth, Edges, ref  Count, maskArray);
            Edges = GetLine(array, (int)(arrayHeight * .9), arrayWidth, arrayHeight, Depth, Edges, ref  Count, maskArray);

            for (int i = 0; i < Count.Length; i++)
                Edges[1, i] /= Count[i];


            float[] EdgesSmooth = new float[arrayWidth];

            double ave = 0;
            for (int i = 0; i < 5; i++)
                EdgesSmooth[i] = (float)Edges[1, i];
            for (int i = 5; i < arrayWidth - 5; i++)
            {
                double Sum2 = 0;

                for (int n = i - 5; n < i + 5; n++)
                {
                    Sum2 += Edges[1, n];

                }
                ave += Edges[1, i];

                EdgesSmooth[i] = (float)Sum2 / 10f;

            }
            for (int i = arrayWidth - 5; i < arrayWidth; i++)
                EdgesSmooth[i] = (float)Edges[1, i];

            float avef = (float)(ave / (arrayWidth - 10));
            for (int i = 0; i < arrayWidth; i++)
                EdgesSmooth[i] -= avef;

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
