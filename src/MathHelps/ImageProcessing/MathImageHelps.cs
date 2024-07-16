using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace MathHelpLib.ImageProcessing
{
    public static class MathImageHelps
    {


        #region FlattenImages

        public static Bitmap FlattenImageEdges(Bitmap image)
        {
            Bitmap bOut = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppRgb);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);


            ImageToFakeArrayGrayscale array = new ImageToFakeArrayGrayscale(bmData.Scan0, bmData.Stride, image.Width, image.Height);
            ImageToFakeArrayGrayscale outImage = new ImageToFakeArrayGrayscale(bmDataOut.Scan0, bmDataOut.Stride, image.Width, image.Height);

            double[,] outArray = new double[image.Width, image.Height];

            double[,] MatrixX = new double[3, 3];
            double[,] MatrixY = new double[1, 3];
            double[,] Edges = new double[3, array.GetLength(0) * 4 + array.GetLength(1) * 4];
            double val = 0;
            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);

            int cc = 0;
            //Get the top edge
            for (int i = 0; i < array.GetLength(0); i++)
            {
                Edges[0, cc] = i;
                Edges[1, cc] = 0;
                Edges[2, cc] = array[i, 0];
                cc++;

                Edges[0, cc] = i;
                Edges[1, cc] = 1;
                Edges[2, cc] = array[i, 1];
                cc++;
            }
            //get the bottom edge
            for (int i = 0; i < array.GetLength(0); i++)
            {
                Edges[0, cc] = i;
                Edges[1, cc] = (arrayHeight - 2);
                Edges[2, cc] = array[i, (arrayHeight - 2)];
                cc++;

                Edges[0, cc] = i;
                Edges[1, cc] = (arrayHeight - 2);
                Edges[2, cc] = array[i, (arrayHeight - 2)];
                cc++;
            }
            //get the right edge 
            for (int i = 0; i < arrayHeight; i++)
            {
                Edges[0, cc] = 0;
                Edges[1, cc] = i;
                Edges[2, cc] = array[0, i];
                cc++;

                Edges[0, cc] = 1;
                Edges[1, cc] = i;
                Edges[2, cc] = array[1, i];
                cc++;
            }
            //get the left edge
            for (int i = 0; i < arrayHeight; i++)
            {
                Edges[0, cc] = arrayWidth - 1;
                Edges[1, cc] = i;
                Edges[2, cc] = array[arrayWidth - 1, i];
                cc++;

                Edges[0, cc] = arrayWidth - 2;
                Edges[1, cc] = i;
                Edges[2, cc] = array[arrayWidth - 2, i];
                cc++;
            }

            //build the matrix for the least squares
            for (int i = 0; i < cc; i++)
            {
                MatrixY[0, 0] += Edges[2, i]; //[v]
                MatrixY[0, 1] += Edges[2, i] * Edges[0, i];//[v*x]
                MatrixY[0, 2] += Edges[2, i] * Edges[1, i]; //[v*y]

                MatrixX[0, 0]++; //N
                MatrixX[0, 1] += Edges[0, i]; //X
                MatrixX[0, 2] += Edges[1, i];//Y
                MatrixX[1, 1] += Edges[0, i] * Edges[0, i];//X^2
                MatrixX[1, 2] += Edges[0, i] * Edges[1, i];//X*Y
                MatrixX[2, 2] += Edges[1, i] * Edges[1, i];//Y^2
            }

            MatrixX[1, 0] = MatrixX[0, 1];
            MatrixX[2, 0] = MatrixX[0, 2];
            MatrixX[2, 1] = MatrixX[1, 2];

            MathNet.Numerics.LinearAlgebra.Matrix m = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixX);
            MathNet.Numerics.LinearAlgebra.Matrix t = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixX);
            MathNet.Numerics.LinearAlgebra.Matrix Ys = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixY);
            t.Transpose();
            Ys.Transpose();

            MathNet.Numerics.LinearAlgebra.Matrix tempM = t * m;
            MathNet.Numerics.LinearAlgebra.Matrix invertM = tempM.Inverse();

            tempM = (invertM * t) * Ys;

            double Max = double.MinValue, Min = double.MaxValue;
            double A = tempM[0, 0], B = tempM[1, 0], C = tempM[2, 0];

            for (int i = 0; i < arrayHeight; i++)
            {
                for (int j = 0; j < arrayWidth; j++)
                {
                    outArray[j, i] = array[j, i] - (A + B * j + C * i);
                    if (outArray[j, i] > Max) Max = outArray[j, i];
                    if (outArray[j, i] < Min) Min = outArray[j, i];
                }
            }
            for (int i = 0; i < arrayHeight; i++)
            {
                for (int j = 0; j < arrayWidth; j++)
                {
                    byte Gray = (byte)(255 * (outArray[j, i] - Min) / (Max - Min));
                    outImage.SetGray(j, i, Gray);
                }
            }

            image.UnlockBits(bmData);
            bOut.UnlockBits(bmDataOut);

            return bOut;
        }
        /*
        public static ImageHolder FlattenImageEdges(ImageHolder image)
        {

            ImageHolder imageOut = image.CopyBlank();

            ushort[, ,] array = image.ImageData;
            ushort[, ,] outImage = imageOut.ImageData;

            double[,] outArray = new double[image.Width, image.Height];

            double[,] MatrixX = new double[3, 3];
            double[,] MatrixY = new double[1, 3];
            double[,] Edges = new double[3, array.GetLength(0) * 4 + array.GetLength(1) * 4];
            double val = 0;
            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);

            int cc = 0;
            //Get the top edge
            for (int i = 0; i < array.GetLength(0); i++)
            {
                Edges[0, cc] = i;
                Edges[1, cc] = 0;
                Edges[2, cc] = array[0, i, 0];
                cc++;

                Edges[0, cc] = i;
                Edges[1, cc] = 1;
                Edges[2, cc] = array[0, i, 1];
                cc++;
            }
            //get the bottom edge
            for (int i = 0; i < array.GetLength(0); i++)
            {
                Edges[0, cc] = i;
                Edges[1, cc] = (arrayHeight - 2);
                Edges[2, cc] = array[0, i, (arrayHeight - 2)];
                cc++;

                Edges[0, cc] = i;
                Edges[1, cc] = (arrayHeight - 2);
                Edges[2, cc] = array[0, i, (arrayHeight - 2)];
                cc++;
            }
            //get the right edge 
            for (int i = 0; i < arrayHeight; i++)
            {
                Edges[0, cc] = 0;
                Edges[1, cc] = i;
                Edges[2, cc] = array[0, 0, i];
                cc++;

                Edges[0, cc] = 1;
                Edges[1, cc] = i;
                Edges[2, cc] = array[0, 1, i];
                cc++;
            }
            //get the left edge
            for (int i = 0; i < arrayHeight; i++)
            {
                Edges[0, cc] = arrayWidth - 1;
                Edges[1, cc] = i;
                Edges[2, cc] = array[0, arrayWidth - 1, i];
                cc++;

                Edges[0, cc] = arrayWidth - 2;
                Edges[1, cc] = i;
                Edges[2, cc] = array[0, arrayWidth - 2, i];
                cc++;
            }

            //build the matrix for the least squares
            for (int i = 0; i < cc; i++)
            {
                MatrixY[0, 0] += Edges[2, i]; //[v]
                MatrixY[0, 1] += Edges[2, i] * Edges[0, i];//[v*x]
                MatrixY[0, 2] += Edges[2, i] * Edges[1, i]; //[v*y]

                MatrixX[0, 0]++; //N
                MatrixX[0, 1] += Edges[0, i]; //X
                MatrixX[0, 2] += Edges[1, i];//Y
                MatrixX[1, 1] += Edges[0, i] * Edges[0, i];//X^2
                MatrixX[1, 2] += Edges[0, i] * Edges[1, i];//X*Y
                MatrixX[2, 2] += Edges[1, i] * Edges[1, i];//Y^2
            }

            MatrixX[1, 0] = MatrixX[0, 1];
            MatrixX[2, 0] = MatrixX[0, 2];
            MatrixX[2, 1] = MatrixX[1, 2];

            MathNet.Numerics.LinearAlgebra.Matrix m = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixX);
            MathNet.Numerics.LinearAlgebra.Matrix t = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixX);
            MathNet.Numerics.LinearAlgebra.Matrix Ys = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixY);
            t.Transpose();
            Ys.Transpose();

            MathNet.Numerics.LinearAlgebra.Matrix tempM = t * m;
            MathNet.Numerics.LinearAlgebra.Matrix invertM = tempM.Inverse();

            tempM = (invertM * t) * Ys;

            double Max = double.MinValue, Min = double.MaxValue;
            double A = tempM[0, 0], B = tempM[1, 0], C = tempM[2, 0];

            for (int i = 0; i < arrayHeight; i++)
            {
                for (int j = 0; j < arrayWidth; j++)
                {
                    outArray[j, i] = array[0, j, i] - (A + B * j + C * i);
                    if (outArray[j, i] > Max) Max = outArray[j, i];
                    if (outArray[j, i] < Min) Min = outArray[j, i];
                }
            }
            for (int i = 0; i < arrayHeight; i++)
            {
                for (int j = 0; j < arrayWidth; j++)
                {
                    byte Gray = (byte)(255 * (outArray[j, i] - Min) / (Max - Min));
                    outImage[0, j, i] = Gray;
                }
            }

            return imageOut;
        }
        */
        public static ImageHolder FlattenImageEdges(ImageHolder image)
        {

            ImageHolder imageOut = new ImageHolder(image.Width, image.Height, image.NChannels);

            float[, ,] array = image.ImageData;
            float[, ,] outImage = imageOut.ImageData;

            float[,] outArray = new float[image.Height, image.Width];

            double[,] MatrixX = new double[3, 3];
            double[,] MatrixY = new double[1, 3];
            double[,] Edges = new double[3, array.GetLength(0) * 4 + array.GetLength(1) * 4];
            double val = 0;
            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);

            int cc = 0;
            //Get the top edge
            for (int i = 0; i < array.GetLength(0); i++)
            {
                Edges[0, cc] = i;
                Edges[1, cc] = 0;
                Edges[2, cc] = array[i, 0, 0];
                cc++;

                Edges[0, cc] = i;
                Edges[1, cc] = 1;
                Edges[2, cc] = array[i, 1, 0];
                cc++;
            }
            //get the bottom edge
            for (int i = 0; i < array.GetLength(0); i++)
            {
                Edges[0, cc] = i;
                Edges[1, cc] = (arrayHeight - 2);
                Edges[2, cc] = array[i, (arrayHeight - 2), 0];
                cc++;

                Edges[0, cc] = i;
                Edges[1, cc] = (arrayHeight - 2);
                Edges[2, cc] = array[i, (arrayHeight - 2), 0];
                cc++;
            }
            //get the right edge 
            for (int i = 0; i < arrayHeight; i++)
            {
                Edges[0, cc] = 0;
                Edges[1, cc] = i;
                Edges[2, cc] = array[0, i, 0];
                cc++;

                Edges[0, cc] = 1;
                Edges[1, cc] = i;
                Edges[2, cc] = array[1, i, 0];
                cc++;
            }
            //get the left edge
            for (int i = 0; i < arrayHeight; i++)
            {
                Edges[0, cc] = arrayWidth - 1;
                Edges[1, cc] = i;
                Edges[2, cc] = array[arrayWidth - 1, i, 0];
                cc++;

                Edges[0, cc] = arrayWidth - 2;
                Edges[1, cc] = i;
                Edges[2, cc] = array[arrayWidth - 2, i, 0];
                cc++;
            }

            //build the matrix for the least squares
            for (int i = 0; i < cc; i++)
            {
                MatrixY[0, 0] += Edges[2, i]; //[v]
                MatrixY[0, 1] += Edges[2, i] * Edges[0, i];//[v*x]
                MatrixY[0, 2] += Edges[2, i] * Edges[1, i]; //[v*y]

                MatrixX[0, 0]++; //N
                MatrixX[0, 1] += Edges[0, i]; //X
                MatrixX[0, 2] += Edges[1, i];//Y
                MatrixX[1, 1] += Edges[0, i] * Edges[0, i];//X^2
                MatrixX[1, 2] += Edges[0, i] * Edges[1, i];//X*Y
                MatrixX[2, 2] += Edges[1, i] * Edges[1, i];//Y^2
            }

            MatrixX[1, 0] = MatrixX[0, 1];
            MatrixX[2, 0] = MatrixX[0, 2];
            MatrixX[2, 1] = MatrixX[1, 2];

            MathNet.Numerics.LinearAlgebra.Matrix m = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixX);
            MathNet.Numerics.LinearAlgebra.Matrix t = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixX);
            MathNet.Numerics.LinearAlgebra.Matrix Ys = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixY);
            t.Transpose();
            Ys.Transpose();

            MathNet.Numerics.LinearAlgebra.Matrix tempM = t * m;
            MathNet.Numerics.LinearAlgebra.Matrix invertM = tempM.Inverse();

            tempM = (invertM * t) * Ys;

            float Max = float.MinValue, Min = float.MaxValue;
            float A = (float)tempM[0, 0], B = (float)tempM[1, 0], C = (float)tempM[2, 0];

            for (int i = 0; i < arrayHeight; i++)
            {
                for (int j = 0; j < arrayWidth; j++)
                {
                    outArray[j, i] = array[j, i, 0] - (A + B * j + C * i);
                    // if (outArray[j, i] > Max) Max = outArray[j, i];
                    if (outArray[j, i] < Min) Min = outArray[j, i];
                }
            }

            for (int i = 0; i < arrayHeight; i++)
            {
                for (int j = 0; j < arrayWidth; j++)
                {
                    // byte Gray = (byte)(255 * (outArray[j, i] - Min) / (Max - Min));
                    outImage[j, i, 0] = (outArray[j, i] - Min);
                }
            }

            return imageOut;
        }


        public static double[,] FlattenImageEdges(double[,] array)
        {

            double[,] outArray = new double[array.GetLength(0), array.GetLength(1)];

            double[,] MatrixX = new double[3, 3];
            double[,] MatrixY = new double[1, 3];
            double[,] Edges = new double[3, array.GetLength(0) * 4 + array.GetLength(1) * 4];

            int arrayHeight = array.GetLength(1);
            int arrayWidth = array.GetLength(0);

            int cc = 0;
            //Get the top edge
            for (int i = 0; i < array.GetLength(0); i++)
            {
                Edges[0, cc] = i;
                Edges[1, cc] = 0;
                Edges[2, cc] = array[i, 0];
                cc++;

                Edges[0, cc] = i;
                Edges[1, cc] = 1;
                Edges[2, cc] = array[i, 1];
                cc++;
            }
            //get the bottom edge
            for (int i = 0; i < array.GetLength(0); i++)
            {
                Edges[0, cc] = i;
                Edges[1, cc] = (arrayHeight - 2);
                Edges[2, cc] = array[i, (arrayHeight - 2)];
                cc++;

                Edges[0, cc] = i;
                Edges[1, cc] = (arrayHeight - 2);
                Edges[2, cc] = array[i, (arrayHeight - 2)];
                cc++;
            }
            //get the right edge 
            for (int i = 0; i < arrayHeight; i++)
            {
                Edges[0, cc] = 0;
                Edges[1, cc] = i;
                Edges[2, cc] = array[0, i];
                cc++;

                Edges[0, cc] = 1;
                Edges[1, cc] = i;
                Edges[2, cc] = array[1, i];
                cc++;
            }
            //get the left edge
            for (int i = 0; i < arrayHeight; i++)
            {
                Edges[0, cc] = arrayWidth - 1;
                Edges[1, cc] = i;
                Edges[2, cc] = array[arrayWidth - 1, i];
                cc++;

                Edges[0, cc] = arrayWidth - 2;
                Edges[1, cc] = i;
                Edges[2, cc] = array[arrayWidth - 2, i];
                cc++;
            }

            //build the matrix for the least squares
            for (int i = 0; i < cc; i++)
            {
                MatrixY[0, 0] += Edges[2, i]; //[v]
                MatrixY[0, 1] += Edges[2, i] * Edges[0, i];//[v*x]
                MatrixY[0, 2] += Edges[2, i] * Edges[1, i]; //[v*y]

                MatrixX[0, 0]++; //N
                MatrixX[0, 1] += Edges[0, i]; //X
                MatrixX[0, 2] += Edges[1, i];//Y
                MatrixX[1, 1] += Edges[0, i] * Edges[0, i];//X^2
                MatrixX[1, 2] += Edges[0, i] * Edges[1, i];//X*Y
                MatrixX[2, 2] += Edges[1, i] * Edges[1, i];//Y^2
            }

            MatrixX[1, 0] = MatrixX[0, 1];
            MatrixX[2, 0] = MatrixX[0, 2];
            MatrixX[2, 1] = MatrixX[1, 2];

            MathNet.Numerics.LinearAlgebra.Matrix m = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixX);
            MathNet.Numerics.LinearAlgebra.Matrix t = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixX);
            MathNet.Numerics.LinearAlgebra.Matrix Ys = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixY);
            t.Transpose();
            Ys.Transpose();

            MathNet.Numerics.LinearAlgebra.Matrix tempM = t * m;
            MathNet.Numerics.LinearAlgebra.Matrix invertM = tempM.Inverse();

            tempM = (invertM * t) * Ys;

            double Max = double.MinValue, Min = double.MaxValue;
            double A = tempM[0, 0], B = tempM[1, 0], C = tempM[2, 0];

            for (int i = 0; i < arrayHeight; i++)
            {
                for (int j = 0; j < arrayWidth; j++)
                {
                    outArray[j, i] = array[j, i] - (A + B * j + C * i);
                    if (outArray[j, i] > Max) Max = outArray[j, i];
                    if (outArray[j, i] < Min) Min = outArray[j, i];
                }
            }
            return outArray;
        }

        /*   /// <summary>
           /// Flattens a image by looking at all the edge pixels and then performing a plane subtraction
           /// </summary>
           /// <param name="array"></param>
           /// <returns></returns>
           public static double[,] FlattenImageEdges(this double[,] array)
           {
               double[,] outArray = new double[array.GetLength(0), array.GetLength(1)];

               double[,] MatrixX = new double[3, 3];
               double[,] MatrixY = new double[1, 3];
               double[,] Edges = new double[3, array.GetLength(0) * 4 + array.GetLength(1) * 4];

               int arrayHeight = array.GetLength(1);
               int arrayWidth = array.GetLength(0);

               int cc = 0;
               //Get the top edge
               for (int i = 0; i < array.GetLength(0); i++)
               {
                   Edges[0, cc] = i;
                   Edges[1, cc] = 0;
                   Edges[2, cc] = array[i, 0];
                   cc++;

                   Edges[0, cc] = i;
                   Edges[1, cc] = 1;
                   Edges[2, cc] = array[i, 1];
                   cc++;
               }
               //get the bottom edge
               for (int i = 0; i < array.GetLength(0); i++)
               {
                   Edges[0, cc] = i;
                   Edges[1, cc] = (arrayHeight - 2);
                   Edges[2, cc] = array[i, (arrayHeight - 2)];
                   cc++;

                   Edges[0, cc] = i;
                   Edges[1, cc] = (arrayHeight - 2);
                   Edges[2, cc] = array[i, (arrayHeight - 2)];
                   cc++;
               }
               //get the right edge 
               for (int i = 0; i < arrayHeight; i++)
               {
                   Edges[0, cc] = 0;
                   Edges[1, cc] = i;
                   Edges[2, cc] = array[0, i];
                   cc++;

                   Edges[0, cc] = 1;
                   Edges[1, cc] = i;
                   Edges[2, cc] = array[1, i];
                   cc++;
               }
               //get the left edge
               for (int i = 0; i < arrayHeight; i++)
               {
                   Edges[0, cc] = arrayWidth - 1;
                   Edges[1, cc] = i;
                   Edges[2, cc] = array[arrayWidth - 1, i];
                   cc++;

                   Edges[0, cc] = arrayWidth - 2;
                   Edges[1, cc] = i;
                   Edges[2, cc] = array[arrayWidth - 2, i];
                   cc++;
               }

               //build the matrix for the least squares
               for (int i = 0; i < cc; i++)
               {
                   MatrixY[0, 0] += Edges[2, i]; //[v]
                   MatrixY[0, 1] += Edges[2, i] * Edges[0, i];//[v*x]
                   MatrixY[0, 2] += Edges[2, i] * Edges[1, i]; //[v*y]

                   MatrixX[0, 0]++; //N
                   MatrixX[0, 1] += Edges[0, i]; //X
                   MatrixX[0, 2] += Edges[1, i];//Y
                   MatrixX[1, 1] += Edges[0, i] * Edges[0, i];//X^2
                   MatrixX[1, 2] += Edges[0, i] * Edges[1, i];//X*Y
                   MatrixX[2, 2] += Edges[1, i] * Edges[1, i];//Y^2
               }

               MatrixX[1, 0] = MatrixX[0, 1];
               MatrixX[2, 0] = MatrixX[0, 2];
               MatrixX[2, 1] = MatrixX[1, 2];
               //do the least squares stuff
               MathNet.Numerics.LinearAlgebra.Matrix m = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixX);
               MathNet.Numerics.LinearAlgebra.Matrix t = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixX);
               MathNet.Numerics.LinearAlgebra.Matrix Ys = MathNet.Numerics.LinearAlgebra.Matrix.Create(MatrixY);
               t.Transpose();
               Ys.Transpose();

               MathNet.Numerics.LinearAlgebra.Matrix tempM = t * m;
               MathNet.Numerics.LinearAlgebra.Matrix invertM = tempM.Inverse();

               //get the coefficents for the plane
               tempM = (invertM * t) * Ys;

               double Max = double.MinValue, Min = double.MaxValue;
               double A = tempM[0, 0], B = tempM[1, 0], C = tempM[2, 0];

               //subtract off the plane from the data
               for (int i = 0; i < arrayHeight; i++)
               {
                   for (int j = 0; j < arrayWidth; j++)
                   {
                       outArray[j, i] = array[j, i] - (A + B * j + C * i);
                       if (outArray[j, i] > Max) Max = outArray[j, i];
                       if (outArray[j, i] < Min) Min = outArray[j, i];
                   }
               }


               return outArray;
           }*/

        #endregion

        //todo: make these load with freeimage so they can use more formats

        /// <summary>
        /// Uses .net to load a standard image type.  cannot load 16 bit images
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="Rotate90"></param>
        /// <returns></returns>
        public static Bitmap LoadBitmap(string Filename, bool Rotate90)
        {
            Bitmap b = new Bitmap(Filename);
            if (Rotate90)
                b.RotateFlip(RotateFlipType.Rotate90FlipNone);
            return b;
        }

        /// <summary>
        /// Uses .net to load a bitmap, then converts the 3 channels into an intensity value (uses average value)
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="Rotate90"></param>
        /// <returns></returns>
        public static double[,] LoadStandardImage_Intensity(string Filename, bool Rotate90)
        {
            Bitmap b = new Bitmap(Filename);
            if (Rotate90)
                b.RotateFlip(RotateFlipType.Rotate90FlipNone);

            int iWidth = b.Width;
            int iHeight = b.Height;

            double[,] ImageArray = new double[iWidth, iHeight];

            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

            double g1, g2, g3;
            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            //average the intensities to set the pixel value
                            ImageArray[x, y] = (double)(g1 + g2 + g3) / 3d;
                            scanline++;
                        }
                    }
                }
                else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            ImageArray[x, y] = (double)(g1 + g2 + g3) / 3d;
                            scanline += 3;
                        }
                    }

                }
            }
            b.UnlockBits(bmd);
            return ImageArray;
        }

        /// <summary>
        /// converts a bitmap to a double array using the average of the pixel values
        /// </summary>
        /// <param name="b"></param>
        /// <param name="Rotate90"></param>
        /// <returns></returns>
        /* public static double[,] ConvertToDoubleArray(this Bitmap b, bool Rotate90)
         {
             if (Rotate90)
                 b.RotateFlip(RotateFlipType.Rotate90FlipNone);
             int iWidth = b.Width;
             int iHeight = b.Height;

             double[,] ImageArray = new double[iWidth , iHeight];

             BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

             double g1, g2, g3;
             unsafe
             {

                 if (bmd.Stride / (double)bmd.Width == 4)
                 {
                     for (int y = 0; y < iHeight; y++)
                     {
                         Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                         for (int x = 0; x < iWidth; x++)
                         {
                             byte* bits = (byte*)scanline;
                             g1 = bits[0];
                             g2 = bits[1];
                             g3 = bits[2];

                             ImageArray[x, y] = (g1 + g2  + g3)/3d;
                             scanline++;
                         }
                     }
                 }
                 else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                 {
                     for (int y =0; y < iHeight; y++)
                     {
                         byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);

                         for (int x = 0; x < iWidth; x++)
                         {
                             byte* bits = (byte*)scanline;
                             g1 = bits[0];
                             g2 = bits[1];
                             g3 = bits[2];

                             ImageArray[x, y] = (g1  + g2  + g3)/3d;
                             scanline += 3;
                         }
                     }

                 }
             }
             b.UnlockBits(bmd);
             return ImageArray;
         }*/

        /// <summary>
        /// returns the sum of the intensities in an image
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Int64 SumImage(this Bitmap b)
        {

            int iWidth = b.Width;
            int iHeight = b.Height;

            Int64 ImageArraySum = 0;

            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);

            double g1, g2, g3;
            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            ImageArraySum += (Int64)((g1 + g2 + g3) / 3d);
                            scanline++;
                        }
                    }
                }
                else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            ImageArraySum += (Int64)((g1 + g2 + g3) / 3d);
                            scanline += 3;
                        }
                    }

                }
            }
            b.UnlockBits(bmd);
            return (Int64)ImageArraySum;
        }

        /// <summary>
        /// loads an image using the standard .net api and then converts it to a 32 bit RGB pixel format for easier processing
        /// </summary>
        /// <param name="Filename"></param>
        /// <returns></returns>
        public static Bitmap LoadBitmap_convert_to_32RGB(string Filename)
        {
            Bitmap orig = new Bitmap(Filename);
            Bitmap clone = new Bitmap(orig.Width, orig.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(orig, new Rectangle(0, 0, clone.Width, clone.Height));
            }
            orig.Dispose();
            return clone;
        }

        /// <summary>
        /// This function will rotate the image, clipping the parts that do not fit
        /// </summary>
        /// <param name="b"></param>
        /// <param name="Degrees"></param>
        /// <returns></returns>
        public static Bitmap rotateImage(Bitmap b, float Degrees)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(returnBitmap);
            //move rotation point to center of image
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
            //rotate
            g.RotateTransform(Degrees);
            //move image back
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            //draw passed in image onto graphics object
            g.DrawImage(b, new Point(0, 0));
            return returnBitmap;
        }

        /// <summary>
        /// This function will rotate the image, clipping the parts that do not fit
        /// </summary>
        /// <param name="b"></param>
        /// <param name="Degrees"></param>
        /// <returns></returns>
        public static Bitmap rotateImage(Bitmap b, float Degrees, Color BackColor)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(returnBitmap);

            g.Clear(BackColor);
            //move rotation point to center of image
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
            //rotate
            g.RotateTransform(Degrees);
            //move image back
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            //draw passed in image onto graphics object
            g.DrawImage(b, new Point(0, 0));
            return returnBitmap;
        }

        /// <summary>
        /// cut a section of the image from original image
        /// </summary>
        /// <param name="b"></param>
        /// <param name="clippingRegion"></param>
        /// <returns></returns>
        public static Bitmap ClipImage(this Bitmap b, Rectangle clippingRegion)
        {
            Bitmap b2 = new Bitmap(clippingRegion.Width, clippingRegion.Height, PixelFormat.Format32bppRgb);
            Graphics.FromImage(b2).DrawImage(b, new Rectangle(0, 0, clippingRegion.Width, clippingRegion.Height), clippingRegion, GraphicsUnit.Pixel);
            return b2;
        }

        /// <summary>
        /// takes a masked image and calculates the center of gravity of the non black pixels
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns></returns>
        public static Point FindCentroid(ref Bitmap SourceImage)
        {
            double[,] ImageArray = MathImageHelps.ConvertToDoubleArray(SourceImage, false);

            double SumX = 0;
            double SumY = 0;
            double CountX = 0;
            double CountY = 0;
            double Weight = 1;
            for (int i = 0; i < ImageArray.GetLength(0); i++)
            {
                for (int j = 0; j < ImageArray.GetLength(1); j++)
                {
                    Weight = (ImageArray[i, j]);
                    if (Weight != 0)
                    {
                        SumX += i;
                        SumY += j;
                        CountX += 1;
                        CountY += 1;
                    }
                }
            }
            double CenterX = SumX / CountX;
            double CenterY = SumY / CountY;
            SourceImage = ImageArray.MakeBitmap();
            return new Point((int)CenterX, (int)CenterY);
        }

        /// <summary>
        /// performs a center of gravity of an image using the intensities of the pixels
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="InvertIntensities">false if bright things are the most important, true if dark things are the most important</param>
        /// <returns></returns>
        public static Bitmap CenterImage(Bitmap SourceImage, bool InvertIntensities)
        {
            double[,] ImageArray = MathImageHelps.ConvertToDoubleArray(SourceImage, false);
            double Max = ImageArray.MaxArray();

            double SumX = 0;
            double SumY = 0;
            double CountX = 0;
            double CountY = 0;
            double Weight = 1;
            if (InvertIntensities == false)
            {
                for (int i = 0; i < ImageArray.GetLength(0); i++)
                {
                    for (int j = 0; j < ImageArray.GetLength(1); j++)
                    {
                        Weight = (ImageArray[i, j]);
                        SumX += i * Weight;
                        SumY += j * Weight;
                        CountX += Weight;
                        CountY += Weight;
                    }
                }
            }
            else
            {
                double max = ImageArray.MaxArray();
                for (int i = 0; i < ImageArray.GetLength(0); i++)
                {
                    for (int j = 0; j < ImageArray.GetLength(1); j++)
                    {
                        Weight = max - (ImageArray[i, j]);
                        SumX += i * Weight;
                        SumY += j * Weight;
                        CountX += Weight;
                        CountY += Weight;
                    }
                }
            }
            double CenterX = SourceImage.Width / 2d - SumX / CountX;
            double CenterY = SourceImage.Height / 2d - SumY / CountY;
            Rectangle sRect = new Rectangle(0, 0, SourceImage.Width, SourceImage.Height);
            Rectangle Moved = new Rectangle((int)(CenterX), (int)(CenterY), SourceImage.Width, SourceImage.Height);
            Moved = Rectangle.Intersect(Moved, sRect);
            Bitmap b2 = new Bitmap(SourceImage.Width, SourceImage.Height, PixelFormat.Format32bppRgb);
            Graphics g = Graphics.FromImage(b2);
            g.DrawImage(SourceImage, new Rectangle(0, 0, Moved.Width, Moved.Height), Moved, GraphicsUnit.Pixel);
            return b2;
        }

        /// <summary>
        /// Threshold the image using a percent of the histogram
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="ThresholdPercent">0 to 1</param>
        /// <returns></returns>
        public static Bitmap ThresholdImage(Bitmap SourceImage, double ThresholdPercent)
        {
            double[,] ImageArray = MathImageHelps.ConvertToDoubleArray(SourceImage, false);
            double Max = ImageArray.MaxArray();
            double Min = ImageArray.MinArray();
            double threshold = (Max - Min) * ThresholdPercent + Min;
            for (int i = 0; i < ImageArray.GetLength(0); i++)
            {
                for (int j = 0; j < ImageArray.GetLength(1); j++)
                {
                    if (ImageArray[i, j] < threshold)
                        ImageArray[i, j] = 100;
                    else
                        ImageArray[i, j] = 0;
                }
            }
            return ImageArray.MakeBitmap();
        }

        /// <summary>
        /// Auto threshold image,  not sure if it works
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns></returns>
        public static Bitmap ThresholdImage(Bitmap SourceImage)
        {
            double[,] ImageArray = MathImageHelps.ConvertToDoubleArray(SourceImage, false);
            double Max = ImageArray.MaxArray();
            double Min = ImageArray.MinArray();
            double ThresholdPercent = .5;
            //figure out threshold
            double SumBelow = 0, CountBelow = 0;
            double SumAbove = 0, CountAbove = 0;
            double threshold;

            for (int t = 0; t < 2; t++)
            {
                threshold = (Max - Min) * ThresholdPercent + Min;
                for (int i = 0; i < ImageArray.GetLength(0); i += 4)
                {
                    for (int j = 0; j < ImageArray.GetLength(1); j += 4)
                    {
                        if (ImageArray[i, j] > threshold)
                        {
                            SumAbove += ImageArray[i, j];
                            CountAbove++;
                        }
                        else
                        {
                            SumBelow += ImageArray[i, j];
                            CountBelow++;
                        }
                    }
                }
                SumBelow /= CountBelow;
                SumAbove /= CountAbove;
                SumAbove = (SumAbove + SumBelow) / 2d;
                ThresholdPercent = (SumAbove - Min) / (Max - Min);
            }

            //now do the thresholding
            threshold = (Max - Min) * ThresholdPercent + Min;
            for (int i = 0; i < ImageArray.GetLength(0); i++)
            {
                for (int j = 0; j < ImageArray.GetLength(1); j++)
                {
                    if (ImageArray[i, j] < threshold)
                        ImageArray[i, j] = 100;
                    else
                        ImageArray[i, j] = 0;
                }
            }
            return ImageArray.MakeBitmap();
        }

        /*  /// <summary>
          /// Performs realspace convolution of image and given kernal, returns image of the same size
          /// </summary>
          /// <param name="SourceImage"></param>
          /// <param name="Kernal"></param>
          /// <returns></returns>
          public static Bitmap ConvolutionFilter(Bitmap SourceImage, ImageViewer.Convolution.IConvolutionKernal Kernal)
          {
              int iWidth = SourceImage.Width;
              int iHeight = SourceImage.Height;

              //create the output image, the images is not allowed to grow
              Bitmap bOut = new Bitmap(SourceImage.Width, SourceImage.Height, SourceImage.PixelFormat);
              BitmapData bmd = SourceImage.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height), ImageLockMode.WriteOnly, SourceImage.PixelFormat);
              BitmapData bmdOut = bOut.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height), ImageLockMode.WriteOnly, SourceImage.PixelFormat);

              ///determine how far back to start the linestarts
              int StepBack = (int)Math.Floor(Kernal.Rank[0] / 2d);
              int LineBack = (int)Math.Truncate(Kernal.Rank[1] / -2d);
              int LineForward = (int)Math.Floor(Kernal.Rank[1] / 2d);
              int cc = 0;
              unsafe
              {
                  double Factor = (double)Int32.MaxValue / (double)(iHeight + iWidth);
                  Int32*[] LineStarts = new Int32*[Kernal.Rank[1]];
                  if (bmd.Stride / (double)bmd.Width == 4)
                  {
                      for (int y = Kernal.Rank[1]; y < iHeight - Kernal.Rank[1]; y++)
                      {
                          Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);
                          Int32* scanlineOut = (Int32*)((byte*)bmdOut.Scan0 + (y) * bmdOut.Stride);
                          scanline += StepBack;
                          scanlineOut += StepBack;
                          //todo: The edge pixels are not correctly handled in this code
                          for (int x = StepBack; x < iWidth - StepBack; x++)
                          {
                              cc = 0;
                              //get the starts of the kernal array.  this only covers the lines that are affected by the convolution for this filter
                              for (int line = LineBack; line <= LineForward; line++)
                              {
                                  LineStarts[cc] = (scanline - StepBack) + bmd.Stride / 4 * line;
                                  cc++;
                              }
                              *scanlineOut =  Kernal.RunKernal(LineStarts);
                              scanline++;
                              scanlineOut++;
                          }
                      }
                  }
                  else
                      throw new Exception("Only works for 32 bit images.  Please convert your image");
              }
              SourceImage.UnlockBits(bmd);
              bOut.UnlockBits(bmdOut);
              return bOut;
          }
          */


        /// <summary>
        /// return the pixels between the start and end points
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns>RGB values for each pixel</returns>
        public static Int32[] GetProfileLine(this Bitmap SourceImage, Point p1, Point p2)
        {
            int iWidth = SourceImage.Width;
            int iHeight = SourceImage.Height;
            int Length = (int)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            Int32[] Profile = new Int32[Length];

            BitmapData bmd = SourceImage.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height), ImageLockMode.WriteOnly, SourceImage.PixelFormat);

            double tx = (double)(p2.X - p1.X) / (double)Length;
            double ty = (double)(p2.Y - p1.Y) / (double)Length;
            double sX = p1.X;
            double sY = p1.Y;
            int x = 0, y = 0;

            unsafe
            {
                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int t = 0; t < Length; t++)
                    {
                        y = (int)Math.Round(sY);
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);
                        x = (int)Math.Round(sX);
                        Profile[t] = scanline[x];
                        sY += ty;
                        sX += tx;
                    }
                }
                else
                    throw new Exception("Only works for 32 bit images.  Please convert your image");

            }
            SourceImage.UnlockBits(bmd);
            return Profile;
        }

        /// <summary>
        /// converts a line of RGB values into intensities by adding the values of each
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static double[] ConvertToIntensity(Int32[] Data)
        {
            double[] outArray = new double[Data.Length];
            for (int i = 0; i < Data.Length; i++)
            {
                Color color = Color.FromArgb(Data[i]);
                outArray[i] = color.R + color.G + color.B;
            }
            return outArray;
        }




        #region Image Coversions


        public static ImageHolder ConvertToGrayScaleImage(double[,] Data, double IntensityScale)
        {
            ImageHolder bOut = new ImageHolder(Data.GetLength(1), Data.GetLength(0), 1);

            float[, ,] DataOut = bOut.ImageData;

            ushort Intensity;
            unsafe
            {
                fixed (double* pData = Data)
                {
                    fixed (float* pDataOut = DataOut)
                    {
                        double* pIn = pData;
                        float* pOut = pDataOut;
                        for (int i = 0; i < Data.Length; i++)
                        {
                            *pOut = (float)(*pIn * IntensityScale);
                            pOut++;
                            pIn++;
                        }
                    }
                }
            }
            /*   for (int X = 0; X < ImageData.GetLength(0); X++)
               {
                   for (int Y = 0; Y < ImageData.GetLength(1); Y++)
                   {
                       Intensity = (ushort)(ImageData[X, Y] * IntensityScale);
                       DataOut[X,Y,0] = Intensity;
                   
                   }
               }*/
            return bOut;
        }


        public static double[,] ConvertToDoubleArray(object Image, bool Rotate90)
        {
            if (Image.GetType() == typeof(Bitmap))
            {
                return ConvertToDoubleArray((Bitmap)Image, Rotate90);
            }
            else if (Image.GetType() == typeof(ImageHolder))
            {
                return ConvertToDoubleArray((ImageHolder)Image, Rotate90);
            }

            return null;
        }

        public static double[,] ConvertToDoubleArray(ImageHolder Image, bool Rotate90, Rectangle Region)
        {
            double[,] ImageArray;
            if (Rotate90 == false)
            {
                int iWidth = Image.Width;
                int iHeight = Image.Height;

                ImageArray = new double[Region.Width, Region.Height];
                int NChannels = Image.NChannels;

                if (NChannels == 1)
                {
                    #region 1Channels
                    for (int i = 0; i < Region.Width; i++)
                        for (int j = 0; j < Region.Height; j++)
                        {
                            ImageArray[i, j] = Image.ImageData[Region.Y + j, Region.X + i, 0];
                        }

                    #endregion
                }
                else
                    throw new Exception("Not Implimented for " + NChannels.ToString() + " channels");
            }
            else
            {
                throw new Exception("Not Implimented for ");
            }
            return ImageArray;
        }

        public static double[,] ConvertToDoubleArray(ImageHolder Image, bool Rotate90)
        {
            double[,] ImageArray;
            if (Rotate90 == false)
            {
                int iWidth = Image.Width;
                int iHeight = Image.Height;

                ImageArray = new double[iHeight, iWidth];
                int NChannels = Image.NChannels;

                if (NChannels == 1)
                {
                    #region 1Channels

                    unsafe
                    {
                        fixed (float* pFromBase = Image.ImageData)
                        {
                            fixed (double* pToBase = ImageArray)
                            {
                                float* pFrom = pFromBase;
                                double* pTo = pToBase;
                                int Length = Image.ImageData.Length;
                                for (int i = 0; i < Length; i += 1)
                                {
                                    *pTo = *pFrom;
                                    pTo++;
                                    pFrom++;
                                }
                            }
                        }
                    }
                    #endregion

                }
                else if (NChannels == 3)
                {
                    #region 3Channels

                    unsafe
                    {
                        fixed (float* pFromBase = Image.ImageData)
                        {
                            fixed (double* pToBase = ImageArray)
                            {
                                float* pFrom = pFromBase;
                                double* pTo = pToBase;
                                int Length = Image.ImageData.Length;
                                for (int i = 0; i < Length; i += 3)
                                {
                                    *pTo = (*pFrom + *(pFrom + 1) + *(pFrom + 2)) / 3f;
                                    pTo++;
                                    pFrom += 3;
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                    throw new Exception("Not Implimented for " + NChannels.ToString() + " channels");
            }
            else
            {

                float[, ,] OutArray = Image.ImageData;
                ImageArray = new double[Image.Width, Image.Height];
                int NChannels = Image.NChannels;

                if (NChannels == 1)
                {
                    #region 1Channels

                    for (int x = 0; x < ImageArray.GetLength(0); x++)
                    {
                        for (int y = 0; y < ImageArray.GetLength(1); y++)
                        {
                            ImageArray[x, y] = OutArray[y, x, 0];
                        }
                    }
                    #endregion

                }

                else
                    throw new Exception("Not Implimented for " + NChannels.ToString() + " channels");
            }
            return ImageArray;
        }

        public static double[,] ConvertToDoubleArray(Bitmap Image, bool Rotate90)
        {
            if (Rotate90)
                Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            int iWidth = Image.Width;
            int iHeight = Image.Height;

            double[,] ImageArray = new double[iWidth, iHeight];

            BitmapData bmd = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), ImageLockMode.WriteOnly, Image.PixelFormat);

            double g1, g2, g3;
            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            ImageArray[x, y] = (g1 + g2 + g3) / 3d;
                            scanline++;
                        }
                    }
                }
                else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            ImageArray[x, y] = (g1 + g2 + g3) / 3d;
                            scanline += 3;
                        }
                    }

                }
            }
            Image.UnlockBits(bmd);
            return ImageArray;
        }


        public static double[,] ConvertToDoubleArrayPowerOf2(object Image, bool Rotate90)
        {
            if (Image.GetType() == typeof(Bitmap))
            {
                return ConvertToDoubleArrayPowerOf2((Bitmap)Image, Rotate90);
            }
            else if (Image.GetType() == typeof(ImageHolder))
            {
                return ConvertToDoubleArrayPowerOf2((ImageHolder)Image, Rotate90);
            }

            return null;
        }

        public static double[,] ConvertToDoubleArrayPowerOf2(ImageHolder Image, bool Rotate90)
        {
            double[,] ImageArray;
            if (Rotate90 == false)
            {
                int iWidth = Image.Width;
                int iHeight = Image.Height;

                int newWidth = (int)NearestPowerOf2(iWidth);
                int newHeight = (int)NearestPowerOf2(iHeight);

                int OffsetX = (int)((newWidth - iWidth) / 2d);
                int OffsetY = (int)((newHeight - iHeight) / 2d);

                ImageArray = new double[newHeight, newWidth];
                int NChannels = Image.NChannels;

                if (NChannels == 1)
                {
                    #region 1Channels

                    unsafe
                    {
                        fixed (float* pFromBase = Image.ImageData)
                        {
                            fixed (double* pToBase = ImageArray)
                            {
                                for (int y = 0; y < iHeight; y++)
                                {
                                    float* pFrom = pFromBase + y * iWidth;
                                    double* pTo = pToBase + (y + OffsetY) * newWidth + OffsetX;
                                    for (int x = 0; x < iWidth; x++)
                                    {
                                        *pTo = *pFrom;
                                        pTo++;
                                        pFrom++;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (NChannels == 2)
                {
                    #region 2Channels

                    unsafe
                    {
                        fixed (float* pFromBase = Image.ImageData)
                        {
                            fixed (double* pToBase = ImageArray)
                            {
                                for (int y = 0; y < iHeight; y++)
                                {
                                    float* pFrom = pFromBase + y * iWidth * 2;
                                    double* pTo = pToBase + (y + OffsetY) * newWidth + OffsetX;
                                    for (int x = 0; x < iWidth; x++)
                                    {
                                        *pTo = *pFrom + *(pFrom + 1);
                                        pTo++;
                                        pFrom += 2;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (NChannels == 3)
                {
                    #region 3Channels

                    unsafe
                    {
                        fixed (float* pFromBase = Image.ImageData)
                        {
                            fixed (double* pToBase = ImageArray)
                            {
                                for (int y = 0; y < iHeight; y++)
                                {
                                    float* pFrom = pFromBase + y * iWidth * 3;
                                    double* pTo = pToBase + (y + OffsetY) * newWidth + OffsetX;
                                    // float* pTo = pToBase + (y+OffsetY  ) * newWidth ;
                                    for (int x = 0; x < iWidth; x++)
                                    {
                                        *pTo = *pFrom + *(pFrom + 1) + *(pFrom + 2);
                                        pTo++;
                                        pFrom += 3;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                    throw new Exception("Not Implimented for " + NChannels.ToString() + " channels");
            }
            else
            {
                int iWidth = Image.Width;
                int iHeight = Image.Height;

                int newWidth = (int)NearestPowerOf2(iWidth);
                int newHeight = (int)NearestPowerOf2(iHeight);

                int OffsetX = (int)((newWidth - iWidth) / 2d);
                int OffsetY = (int)((newHeight - iHeight) / 2d);

                ImageArray = new double[newWidth, newHeight];

                int NChannels = Image.NChannels;

                if (NChannels == 1)
                {
                    #region 1Channels

                    float[, ,] FromArray = Image.ImageData;

                    for (int y = 0; y < iHeight; y++)
                    {
                        for (int x = 0; x < iWidth; x++)
                        {
                            ImageArray[x + OffsetX, y + OffsetY] = FromArray[y, x, 0];
                        }
                    }
                    #endregion
                }
                else if (NChannels == 3)
                {
                    #region 3Channels

                    float[, ,] FromArray = Image.ImageData;

                    for (int y = 0; y < iHeight; y++)
                    {
                        for (int x = 0; x < iWidth; x++)
                        {
                            ImageArray[x + OffsetX, y + OffsetY] = FromArray[y, x, 0] + FromArray[y, x, 1] + FromArray[y, x, 2];
                        }
                    }

                    #endregion
                }
                else
                    throw new Exception("Not Implimented for " + NChannels.ToString() + " channels");
            }
            return ImageArray;
        }

        public static double[,] ConvertToDoubleArrayPowerOf2(Bitmap Image, bool Rotate90)
        {
            if (Rotate90)
                Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            int iWidth = Image.Width;
            int iHeight = Image.Height;

            int newWidth = (int)NearestPowerOf2(iWidth);
            int newHeight = (int)NearestPowerOf2(iHeight);

            int OffsetX = (int)((newWidth - iWidth) / 2d);
            int OffsetY = (int)((newHeight - iHeight) / -2d);

            double[,] ImageArray = new double[newHeight, newWidth];

            BitmapData bmd = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), ImageLockMode.WriteOnly, Image.PixelFormat);

            float g1, g2, g3;
            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            ImageArray[x + OffsetX, y + OffsetY] = (g1 + g2 + g3) / 3d;
                            scanline++;
                        }
                    }
                }
                else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            ImageArray[x + OffsetX, y + OffsetY] = (g1 + g2 + g3) / 3d;
                            scanline += 3;
                        }
                    }

                }
            }
            Image.UnlockBits(bmd);
            return ImageArray;
        }


        public static float[,] ConvertToFloatArray(object Image, bool Rotate90)
        {
            if (Image.GetType() == typeof(Bitmap))
            {
                return ConvertToFloatArray((Bitmap)Image, Rotate90);
            }
            else if (Image.GetType() == typeof(ImageHolder))
            {
                return ConvertToFloatArray((ImageHolder)Image, Rotate90);
            }
            else if (Image.GetType() == typeof(ImageHolder))
            {
                return ConvertToFloatArray((ImageHolder)Image, Rotate90);
            }
            return null;
        }

        public static float[,] ConvertToFloatArray(ImageHolder Image, bool Rotate90)
        {
            float[,] ImageArray;
            if (Rotate90 == false)
            {
                int iWidth = Image.Width;
                int iHeight = Image.Height;

                ImageArray = new float[iHeight, iWidth];
                int NChannels = Image.NChannels;

                if (NChannels == 1)
                {
                    #region 1Channels

                    unsafe
                    {
                        fixed (float* pFromBase = Image.ImageData)
                        {
                            fixed (float* pToBase = ImageArray)
                            {
                                float* pFrom = pFromBase;
                                float* pTo = pToBase;
                                int Length = Image.ImageData.Length;
                                for (int i = 0; i < Length; i += 1)
                                {
                                    *pTo = *pFrom;
                                    pTo++;
                                    pFrom++;
                                }
                            }
                        }
                    }
                    #endregion

                }
                else if (NChannels == 3)
                {
                    #region 3Channels

                    unsafe
                    {
                        fixed (float* pFromBase = Image.ImageData)
                        {
                            fixed (float* pToBase = ImageArray)
                            {
                                float* pFrom = pFromBase;
                                float* pTo = pToBase;
                                int Length = Image.ImageData.Length;
                                for (int i = 0; i < Length; i += 3)
                                {
                                    *pTo = (*pFrom + *(pFrom + 1) + *(pFrom + 2)) / 3f;
                                    pTo++;
                                    pFrom += 3;
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                    throw new Exception("Not Implimented for " + NChannels.ToString() + " channels");
            }
            else
            {
                int iWidth = Image.Height;
                int iHeight = Image.Width;

                float[, ,] OutArray = Image.ImageData;
                ImageArray = new float[iHeight, iWidth];
                int NChannels = Image.NChannels;

                if (NChannels == 1)
                {
                    #region 1Channels

                    for (int x = 0; x < iWidth; x++)
                    {
                        for (int y = 0; y < iHeight; y++)
                        {
                            ImageArray[x, y] = OutArray[y, x, 0];
                        }
                    }
                    #endregion

                }

                else
                    throw new Exception("Not Implimented for " + NChannels.ToString() + " channels");
            }
            return ImageArray;
        }

        public static float[,] ConvertToFloatArray(Bitmap Image, bool Rotate90)
        {
            if (Rotate90)
                Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            int iWidth = Image.Width;
            int iHeight = Image.Height;

            float[,] ImageArray = new float[iWidth, iHeight];

            BitmapData bmd = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), ImageLockMode.WriteOnly, Image.PixelFormat);

            float g1, g2, g3;
            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            ImageArray[x, y] = (g1 + g2 + g3) / 3f;
                            scanline++;
                        }
                    }
                }
                else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            ImageArray[x, y] = (g1 + g2 + g3) / 3f;
                            scanline += 3;
                        }
                    }

                }
            }
            Image.UnlockBits(bmd);
            return ImageArray;
        }


        public static float[,] ConvertToFloatArrayPowerOf2(object Image, bool Rotate90)
        {
            if (Image.GetType() == typeof(Bitmap))
            {
                return ConvertToFloatArrayPowerOf2((Bitmap)Image, Rotate90);
            }
            else if (Image.GetType() == typeof(ImageHolder))
            {
                return ConvertToFloatArrayPowerOf2((ImageHolder)Image, Rotate90);
            }

            return null;
        }

        public static float[,] ConvertToFloatArrayPowerOf2(ImageHolder Image, bool Rotate90)
        {
            float[,] ImageArray;
            if (Rotate90 == false)
            {
                int iWidth = Image.Width;
                int iHeight = Image.Height;

                int newWidth = (int)NearestPowerOf2(iWidth);
                int newHeight = (int)NearestPowerOf2(iHeight);

                int OffsetX = (int)((newWidth - iWidth) / 2d);
                int OffsetY = (int)((newHeight - iHeight) / 2d);

                ImageArray = new float[newHeight, newWidth];
                int NChannels = Image.NChannels;

                if (NChannels == 1)
                {
                    #region 1Channels

                    unsafe
                    {
                        fixed (float* pFromBase = Image.ImageData)
                        {
                            fixed (float* pToBase = ImageArray)
                            {
                                for (int y = 0; y < iHeight; y++)
                                {
                                    float* pFrom = pFromBase + y * iWidth;
                                    float* pTo = pToBase + (y + OffsetY) * newWidth + OffsetX;
                                    for (int x = 0; x < iWidth; x++)
                                    {
                                        *pTo = *pFrom;
                                        pTo++;
                                        pFrom++;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (NChannels == 2)
                {
                    #region 2Channels

                    unsafe
                    {
                        fixed (float* pFromBase = Image.ImageData)
                        {
                            fixed (float* pToBase = ImageArray)
                            {
                                for (int y = 0; y < iHeight; y++)
                                {
                                    float* pFrom = pFromBase + y * iWidth * 2;
                                    float* pTo = pToBase + (y + OffsetY) * newWidth + OffsetX;
                                    for (int x = 0; x < iWidth; x++)
                                    {
                                        *pTo = *pFrom + *(pFrom + 1);
                                        pTo++;
                                        pFrom += 2;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (NChannels == 3)
                {
                    #region 3Channels

                    unsafe
                    {
                        fixed (float* pFromBase = Image.ImageData)
                        {
                            fixed (float* pToBase = ImageArray)
                            {
                                for (int y = 0; y < iHeight; y++)
                                {
                                    float* pFrom = pFromBase + y * iWidth * 3;
                                    float* pTo = pToBase + (y + OffsetY) * newWidth + OffsetX;
                                    // float* pTo = pToBase + (y+OffsetY  ) * newWidth ;
                                    for (int x = 0; x < iWidth; x++)
                                    {
                                        *pTo = *pFrom + *(pFrom + 1) + *(pFrom + 2);
                                        pTo++;
                                        pFrom += 3;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                    throw new Exception("Not Implimented for " + NChannels.ToString() + " channels");
            }
            else
            {
                int iWidth = Image.Width;
                int iHeight = Image.Height;

                int newWidth = (int)NearestPowerOf2(iWidth);
                int newHeight = (int)NearestPowerOf2(iHeight);

                int OffsetX = (int)((newWidth - iWidth) / 2d);
                int OffsetY = (int)((newHeight - iHeight) / 2d);

                ImageArray = new float[newWidth, newHeight];

                int NChannels = Image.NChannels;

                if (NChannels == 1)
                {
                    #region 1Channels

                    float[, ,] FromArray = Image.ImageData;

                    for (int y = 0; y < iHeight; y++)
                    {
                        for (int x = 0; x < iWidth; x++)
                        {
                            ImageArray[x + OffsetX, y + OffsetY] = FromArray[y, x, 0];
                        }
                    }
                    #endregion
                }
                else if (NChannels == 3)
                {
                    #region 3Channels

                    float[, ,] FromArray = Image.ImageData;

                    for (int y = 0; y < iHeight; y++)
                    {
                        for (int x = 0; x < iWidth; x++)
                        {
                            ImageArray[x + OffsetX, y + OffsetY] = FromArray[y, x, 0] + FromArray[y, x, 1] + FromArray[y, x, 2];
                        }
                    }

                    #endregion
                }
                else
                    throw new Exception("Not Implimented for " + NChannels.ToString() + " channels");
            }
            return ImageArray;
        }

        public static float[,] ConvertToFloatArrayPowerOf2(Bitmap Image, bool Rotate90)
        {
            if (Rotate90)
                Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            int iWidth = Image.Width;
            int iHeight = Image.Height;

            int newWidth = (int)NearestPowerOf2(iWidth);
            int newHeight = (int)NearestPowerOf2(iHeight);

            int OffsetX = (int)((newWidth - iWidth) / 2d);
            int OffsetY = (int)((newHeight - iHeight) / -2d);

            float[,] ImageArray = new float[newHeight, newWidth];

            BitmapData bmd = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), ImageLockMode.WriteOnly, Image.PixelFormat);

            float g1, g2, g3;
            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            ImageArray[x + OffsetX, y + OffsetY] = (g1 + g2 + g3) / 3f;
                            scanline++;
                        }
                    }
                }
                else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + y * bmd.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;
                            g1 = bits[0];
                            g2 = bits[1];
                            g3 = bits[2];

                            ImageArray[x + OffsetX, y + OffsetY] = (g1 + g2 + g3) / 3f;
                            scanline += 3;
                        }
                    }

                }
            }
            Image.UnlockBits(bmd);
            return ImageArray;
        }

        public static double NearestPowerOf2(int testNumber)
        {
            double denom = Math.Log(testNumber) / Math.Log(2);
            if (denom - Math.Floor(denom) == 0)
                return testNumber;
            else
                return Math.Pow(2, Math.Floor(denom) + 1);
        }

        public static Bitmap ConvertToBitmap(this double[,] ImageArray)
        {
            return MakeBitmap(ImageArray);
        }


        public static Bitmap MergeBitmaps(float[,] b1, float[,] b2, Color color1, Color color2, float intensity1, float intensity2, float minPecent)
        {
            int iWidth = b1.GetLength(0);
            int iHeight = b1.GetLength(1);

            float iMax1 = -10000;
            float iMin1 = 10000;

            float iMax2 = -10000;
            float iMin2 = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax1 < b1[i, j]) iMax1 = b1[i, j];
                    if (iMin1 > b1[i, j] && b1[i, j] != 0) iMin1 = b1[i, j];

                    if (iMax2 < b2[i, j]) iMax2 = b2[i, j];
                    if (iMin2 > b2[i, j] && b2[i, j] != 0) iMin2 = b2[i, j];
                }
            float iLength1 = iMax1 - iMin1;
            float iLength2 = iMax2 - iMin2;
            iMin1 = iMin1 + iLength1 * minPecent;
            iMin2 = iMin2 + iLength2 * minPecent;

            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);

            BitmapData bOut = b.LockBits(new Rectangle(0, 0, iWidth, iHeight), ImageLockMode.WriteOnly, b.PixelFormat);

            float[] col1 = new float[] { color1.R / 255f, color1.G / 255f, color1.B / 255f };
            float[] col2 = new float[] { color2.R / 255f, color2.G / 255f, color2.B / 255f };

            float w1 = 1;// (col1[2] + col2[2]);
            float w2 = 1;// (col1[1] + col2[1]);
            float w3 = 1;// (col1[0] + col2[0]);

            if (w1 == 0) w1 = 1;
            if (w2 == 0) w2 = 1;
            if (w3 == 0) w3 = 1;
            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanOut = (Int32*)((byte*)bOut.Scan0 + (y) * bOut.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bitsOut = (byte*)scanOut;

                        float bits1 = 254f * (b1[x, y] - iMin1) / iLength1;
                        float bits2 = 254f * (b2[x, y] - iMin2) / iLength2;
                        if (bits1 < 0) bits1 = 0;
                        if (bits2 < 0) bits2 = 0;
                        if (b1[x,y]==0) bits1 =0;
                        if (b2[x,y]==0) bits2 =0;

                        bitsOut[0] = (byte)((intensity1 * bits1 * col1[2] + intensity2 * bits2 * col2[2]) / w1);
                        bitsOut[1] = (byte)((intensity1 * bits1 * col1[1] + intensity2 * bits2 * col2[1]) / w2);
                        bitsOut[2] = (byte)((intensity1 * bits1 * col1[0] + intensity2 * bits2 * col2[0]) / w3);

                        scanOut++;
                    }
                }
            }

            b.UnlockBits(bOut);

            return b;
        }


        public static Bitmap MergeBitmaps(Bitmap b1, Bitmap b2, int dx, int dy)
        {
            int iWidth = b1.Width;
            int iHeight = b1.Height;

            Bitmap b = new Bitmap(b1);

            BitmapData bOut = b.LockBits(new Rectangle(0, 0, b1.Width, b1.Height), ImageLockMode.WriteOnly, b1.PixelFormat);

            BitmapData bmd = b1.LockBits(new Rectangle(0, 0, b1.Width, b1.Height), ImageLockMode.WriteOnly, b1.PixelFormat);
            BitmapData bmd2 = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height), ImageLockMode.WriteOnly, b2.PixelFormat);

            unsafe
            {
                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    int cy = dy;
                    for (int y = 0; y < iHeight; y++)
                    {
                        Int32* scanline1 = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);
                        Int32* scanline2 = (Int32*)((byte*)bmd2.Scan0 + (cy) * bmd2.Stride) + dx;

                        Int32* scanOut = (Int32*)((byte*)bOut.Scan0 + (y) * bmd2.Stride);

                        int cx = dx;
                        for (int x = 0; x < iWidth; x++)
                        {
                            if (cy > 0 && cy < b2.Height)
                            {
                                if (cx > 0 && cx < b2.Width)
                                {
                                    byte* bitsOut = (byte*)scanOut;

                                    byte* bits1 = (byte*)scanline1;
                                    byte* bits2 = (byte*)scanline2;
                                    bitsOut[0] = (byte)((bits1[0]));
                                    bitsOut[1] = (byte)(0);
                                    bitsOut[2] = (byte)((bits2[2]));
                                    scanline1++;
                                    scanline2++;

                                    scanOut++;
                                }
                            }
                            cx++;
                        }

                        cy++;
                    }
                }
            }

            b1.UnlockBits(bmd);
            b2.UnlockBits(bmd2);
            b.UnlockBits(bOut);

            return b;
        }

        /// <summary>
        /// Converts offsetX 2D array to offsetX intensity bitmap
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <returns></returns>
        public static Bitmap MakeBitmap(this double[,] ImageArray)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[i, j]) iMax = ImageArray[i, j];
                    if (iMin > ImageArray[i, j] && ImageArray[i, j] != 0) iMin = ImageArray[i, j];
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }


        /// <summary>
        /// Converts offsetX 2D array to offsetX intensity bitmap
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <returns></returns>
        public static Bitmap MakeBitmap24FlipHorizonal(this double[,] ImageArray)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[i, j]) iMax = ImageArray[i, j];
                    if (iMin > ImageArray[i, j]) iMin = ImageArray[i, j];
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            int fWidth = iWidth - 1;
            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    byte* scanline = (byte*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[fWidth - x, y] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline += 3;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }

        /// <summary>
        /// Converts offsetX 2D array to offsetX intensity bitmap
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <returns></returns>
        public static Bitmap MakeBitmap24(this double[,] ImageArray)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[i, j]) iMax = ImageArray[i, j];
                    if (iMin > ImageArray[i, j]) iMin = ImageArray[i, j];
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    byte* scanline = (byte*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline += 3;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }

        /// <summary>
        /// Converts offsetX 2D array to offsetX intensity bitmap
        /// Uses the corners to set the min of the bitmap
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <returns></returns>
        public static Bitmap MakeBitmapCorner(this double[,] ImageArray)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[i, j]) iMax = ImageArray[i, j];
                    if (iMin > ImageArray[i, j]) iMin = ImageArray[i, j];
                }

            iMin = ImageArray[5, 5];

            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }

        public static Bitmap MakeBitmap(this double[,] ImageArray, double MinContrast, double MaxContrast)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = MaxContrast;
            double iMin = MinContrast;
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }

        public static Bitmap MakeBitmap(this float[,] ImageArray, float MinContrast, float MaxContrast)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = MaxContrast;
            double iMin = MinContrast;
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }

        public static Bitmap MakeBitmap(this float[,] ImageArray)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[i, j]) iMax = ImageArray[i, j];
                    if (iMin > ImageArray[i, j]) iMin = ImageArray[i, j];
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }
        /*public static Bitmap MakeBitmap(this float[,] ImageArray, double  MinContrast, float MaxContrast)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = MaxContrast;
            double iMin = MinContrast;
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = 255;// g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }*/

        /// <summary>
        /// Converts to intensity bitmap
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <returns></returns>
        public static Bitmap MakeBitmap(this Int32[,] ImageArray)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[i, j]) iMax = ImageArray[i, j];
                    if (iMin > ImageArray[i, j]) iMin = ImageArray[i, j];
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, y] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }
        #endregion
    }
}

