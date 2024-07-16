using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace MathHelpLib
{
    public static class MathArrayHelps
    {
        #region ArrayMath




        /// <summary>
        /// returns the max of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe double MaxArray(this double[, ,] array)
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

        /// <summary>
        /// returns the max of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe float MaxArray(this float[, ,] array)
        {
            float max = float.MinValue;
            fixed (float* pArray = array)
            {
                float* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (max < *pAOut)
                        max = *pAOut;
                    pAOut++;
                }
            }
            return max;
        }

        /// <summary>
        /// returns the max of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe float AverageArray(this float[, ,] array)
        {
            double max = 0;
            fixed (float* pArray = array)
            {
                float* pAOut = pArray;
                for (int i = 0; i < array.Length; i++)
                {
                    max += *pAOut;
                    pAOut++;
                }
            }
            return (float)(max / (double)(array.LongLength));
        }


        /// <summary>
        /// returns the max of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe float AverageArrayWithThreshold(this float[, ,] array, float Threshold)
        {
            double max = 0;
            long CC = 0;
            fixed (float* pArray = array)
            {
                float* pAOut = pArray;
                for (int i = 0; i < array.Length; i++)
                {
                    if ((*pAOut) > Threshold)
                    {
                        max += *pAOut;
                        CC++;
                    }
                    pAOut++;
                }
            }
            CC++;
            return (float)(max / (double)(CC));
        }

        /// <summary>
        /// returns the max of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe double MaxArray(this double[][,] array)
        {
            double max = double.MinValue;
            for (int l = 0; l < array.Length; l++)
                fixed (double* pArray = array[l])
                {
                    double* pAOut = pArray;
                    int Length = array[l].Length;
                    for (int i = 0; i < Length; i++)
                    {
                        if (max < *pAOut)
                            max = *pAOut;
                        pAOut++;
                    }
                }
            return max;
        }

        /// <summary>
        /// returns the min of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe double MinArray(this double[][,] array)
        {
            double min = double.MaxValue;
            for (int l = 0; l < array.Length; l++)
                fixed (double* pArray = array[l])
                {
                    double* pAOut = pArray;
                    int Length = array[l].Length;
                    for (int i = 0; i < Length; i++)
                    {
                        if (min > *pAOut)
                            min = *pAOut;
                        pAOut++;
                    }
                }
            return min;
        }

        /// <summary>
        /// returns the min of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe float MinArray(this float[][,] array)
        {
            float min = float.MaxValue;
            for (int l = 0; l < array.Length; l++)
                fixed (float* pArray = array[l])
                {
                    float* pAOut = pArray;
                    int Length = array[l].Length;
                    for (int i = 0; i < Length; i++)
                    {
                        if (min > *pAOut)
                            min = *pAOut;
                        pAOut++;
                    }
                }
            return min;
        }

        /// <summary>
        /// returns the min of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe double MinArray(this double[, ,] array)
        {
            double min = double.MaxValue;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (min > *pAOut) min = *pAOut;
                    pAOut++;
                }
            }
            return min;
        }

        /// <summary>
        /// returns the min of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe float MinArray(this float[, ,] array)
        {
            float min = float.MaxValue;
            fixed (float* pArray = array)
            {
                float* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (min > *pAOut) min = *pAOut;
                    pAOut++;
                }
            }
            return min;
        }

        #endregion



        public static double[,] RotateArray(this double[,] array)
        {
            double[,] OutArray = new double[array.GetLength(1), array.GetLength(0)];

            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                    OutArray[j, i] = array[i, j];
            return OutArray;
        }

        /// <summary>
        /// unpads a bigger array, using the center as the fixed point and cuttting away the edges
        /// </summary>
        /// <param name="array"></param>
        /// <param name="NewLength"></param>
        /// <returns></returns>
        public static double[] CenterShortenArray(this double[] array, int NewLength)
        {
            double[] ArrayOut2 = new double[NewLength];
            int cc = 0;
            int Length2 = array.Length / 2 + NewLength / 2;
            for (int i = (int)(array.Length / 2 - NewLength / 2); i < Length2; i++)
            {
                ArrayOut2[cc] = array[i];
                cc++;
            }
            return ArrayOut2;
        }

        /// <summary>
        /// unpads a bigger array, using the center as the fixed point and cuttting away the edges
        /// </summary>
        /// <param name="array"></param>
        /// <param name="NewLength"></param>
        /// <returns></returns>
        public static double[,] CenterShortenArray(this double[,] array, int NewLength)
        {
            if ((array.GetLength(0) == NewLength && array.GetLength(1) == NewLength) || array.GetLength(0) < NewLength || array.GetLength(1) < NewLength)
                return array;
            double[,] ArrayOut2 = new double[NewLength, NewLength];
            int cc = 0;
            int cc2 = 0;
            int Length2 = array.GetLength(1) / 2 + NewLength / 2;
            for (int i = (int)(array.GetLength(1) / 2 - NewLength / 2); i < Length2; i++)
            {
                cc2 = 0;
                for (int j = (int)(array.GetLength(1) / 2 - NewLength / 2); j < Length2; j++)
                {
                    ArrayOut2[cc, cc2] = array[i, j];
                    cc2++;
                }
                cc++;
            }
            return ArrayOut2;
        }

        /// <summary>
        /// Pads out an array, with the zeros going on the outer edges and the array remaining in the center
        /// </summary>
        /// <param name="array"></param>
        /// <param name="NewLength"></param>
        /// <returns></returns>
        public static double[] ZeroPadArray(this double[] array, int NewLength)
        {
            double[] ArrayOut2 = new double[NewLength];
            int cc = 0;
            int Length2 = array.Length / 2 + NewLength / 2;
            for (int i = (int)(NewLength / 2 - array.Length / 2); i < Length2; i++)
            {
                ArrayOut2[i] = array[cc];
                cc++;
            }
            return ArrayOut2;
        }

        /// <summary>
        /// Pads out an array, with the zeros going on the outer edges and the array remaining in the center. only pads on the second index
        /// </summary>
        /// <param name="array"></param>
        /// <param name="NewLength"></param>
        /// <returns></returns>
        public static double[,] ZeroPadArray1D(this double[,] array, int NewLength)
        {
            double[,] ArrayOut2 = new double[2, NewLength];
            int cc = 0;
            int Length2 = array.GetLength(1) / 2 + NewLength / 2;
            for (int i = (int)(NewLength / 2 - array.GetLength(1) / 2); i < Length2; i++)
            {
                ArrayOut2[0, i] = array[0, cc];
                ArrayOut2[1, i] = array[1, cc];
                cc++;
            }
            return ArrayOut2;
        }

        /// <summary>
        /// Pads out an array, with the zeros going on the outer edges and the array remaining in the center
        /// </summary>
        /// <param name="array"></param>
        /// <param name="NewLength"></param>
        /// <returns></returns>
        public static double[,] ZeroPadArray2D(this double[,] array, int NewLength)
        {
            double[,] ArrayOut2 = new double[NewLength, NewLength];
            int cc = 0;
            int OffSetX = (NewLength - array.GetLength(0)) / 2;
            int OffSetY = (NewLength - array.GetLength(1)) / 2;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    ArrayOut2[i + OffSetX, j + OffSetY] = array[i, j];
                    cc++;
                }
            }
            return ArrayOut2;
        }

        public static double[,] ToDouble(this float[, ,] array)
        {
            double[,] arrayout = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    arrayout[i, j] = array[i, j, 0];
                }
            }
            return arrayout;
        }
        /// <summary>
        /// Pads out an array, with the zeros going on the outer edges and the array remaining in the center
        /// </summary>
        /// <param name="array"></param>
        /// <param name="NewLength"></param>
        /// <returns></returns>
        public static double[,] ZeroPadArray2D(this float[, ,] array, int NewLength)
        {
            double[,] ArrayOut2 = new double[NewLength, NewLength];
            int cc = 0;
            int OffSetX = (NewLength - array.GetLength(0)) / 2;
            int OffSetY = (NewLength - array.GetLength(1)) / 2;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    ArrayOut2[i + OffSetX, j + OffSetY] = array[i, j,0];
                    cc++;
                }
            }
            return ArrayOut2;
        }


        /// <summary>
        /// cuts an array in half by sending half the elements to one array and the other half to the other.  Uses every other value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Array1"></param>
        /// <param name="Array2"></param>
        public static void DecimateArray(this double[] array, ref double[] Array1, ref double[] Array2)
        {
            int cc = 0;
            for (int i = 0; i < array.Length; i += 2)
            {
                Array1[cc] = array[i];
                cc++;
            }

            cc = 0;
            for (int i = 1; i < array.Length; i += 2)
            {
                Array2[cc] = array[i];
                cc++;
            }
        }

        /// <summary>
        /// Cuts out everyother value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Array1"></param>
        public static void DecimateArray(this double[] array, ref double[] Array1)
        {
            int cc = 0;
            for (int i = 0; i < array.Length; i += 2)
            {
                Array1[cc] = array[i];
                cc++;
            }
        }

        /// <summary>
        /// cuts down an array by pulling out numbered values
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Period">The repeat period of the kept values</param>
        /// <returns></returns>
        public static double[] DecimateArray(this double[] array, int Period)
        {
            double[] array1 = new double[(int)(array.Length / Period)];
            int cc = 0;
            for (int i = 0; i < array.Length; i += Period)
            {
                array1[cc] = array[i];
                cc++;
            }
            return array1;
        }

        /// <summary>
        /// performs simple box car filtering of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Period">number of values that are used for the filter</param>
        /// <returns></returns>
        public static double[] SmoothArrayboxCar(this double[] array, int Period)
        {
            double[] Smoothed = new double[array.Length];
            int Count = 0;

            for (int i = 0; i < array.Length; i++)
            {
                Count = 0;
                for (int j = i - Period; j < i + Period; j++)
                {
                    try
                    {
                        Smoothed[i] += array[j];
                        Count++;
                    }
                    catch { }
                }
                if (Count > 0)
                    Smoothed[i] = Smoothed[i] / Count;
            }
            return Smoothed;
        }

        /// <summary>
        /// Normalizes the second rank of the given array to 1 or -1 as the greatest value
        /// </summary>
        /// <param name="array"></param>
        public static void NormalizeGraphable2DArrayInPlace(this double[,] array)
        {
            double Max = double.MinValue;
            for (int i = 0; i < array.GetLength(1); i++)
            {
                if (Math.Abs(array[1, i]) > Max) Max = Math.Abs(array[1, i]);
            }
            for (int i = 0; i < array.GetLength(1); i++)
            {
                array[1, i] = array[1, i] / Max;
            }
        }

        /// <summary>
        /// Changes the average of an array to the new targetvalue
        /// </summary>
        /// <param name="array"></param>
        /// <param name="TargetValue"></param>
        /// <returns></returns>
        public static double[,] NormalizeArraySumInPlace(this double[,] array, double TargetValue)
        {
            ///get the conversion from the old value to the new value
            double sum = array.SumArray() / (double)(array.GetLength(0) * array.GetLength(1));
            double norm = TargetValue / sum;
            ///go through all the elements changing by this value
            unsafe
            {
                fixed (double* pArray = array)
                {
                    double* ppArray = pArray;
                    for (int i = 0; i < array.Length; i++)
                    {
                        *ppArray = (*ppArray) * norm;
                        ppArray++;
                    }
                }
            }
            return array;
        }

        /// <summary>
        /// changes the average value of the array to a new targetvalue
        /// </summary>
        /// <param name="array"></param>
        /// <param name="TargetValue"></param>
        /// <returns></returns>
        public static double[, ,] NormalizeArraySumInPlace(this double[, ,] array, double TargetValue)
        {
            double sum = array.SumArray() / (double)(array.GetLength(0) * array.GetLength(1));
            double norm = TargetValue / sum;

            unsafe
            {
                fixed (double* pArray = array)
                {
                    double* ppArray = pArray;
                    for (int i = 0; i < array.Length; i++)
                    {
                        *ppArray = (*ppArray) * norm;
                        ppArray++;
                    }
                }
            }
            return array;
        }

        /// <summary>
        /// Normalizes the array to 1 or -1 as the greatest value
        /// </summary>
        /// <param name="array"></param>
        public static double[] Normalize1DArrayInPlace(this double[] array)
        {
            double Max = double.MinValue;
            for (int i = 0; i < array.Length; i++)
            {
                if (Math.Abs(array[i]) > Max) Max = Math.Abs(array[i]);
            }
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i] / Max;
            }
            return array;
        }


        /// <summary>
        /// takes and copies the source array, creating a new array with 1 or -1 as the greatest value
        /// </summary>
        /// <param name="array"></param>
        public static double[] Normalize1DArray(this double[] array)
        {
            double Max = double.MinValue;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > Max) Max = array[i];
            }
            double[] OutArray = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                OutArray[i] = array[i] / Max;
            }
            return OutArray;
        }


        /// <summary>
        /// Adds the specified value to all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="yShift"></param>
        public static void ShiftArrayInPlace(this double[] array, double yShift)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i] + yShift;
            }
        }

        /// <summary>
        /// Adds the specified yvalue to all the elements of the second dimension and the 
        /// specifed xvalue to all the elements of the 1st dimension (Used for graphind 1D arrays that have ranges)
        /// </summary>
        /// <param name="array"></param>
        /// <param name="yShift"></param>
        /// <param name="xShift"></param>
        public static void ShiftArrayInPlace(this double[,] array, double yShift, double xShift)
        {
            for (int i = 0; i < array.GetLength(1); i++)
            {
                array[1, i] += yShift;
                array[0, i] += xShift;
            }
        }

        #region ArrayArithmetic
        /// <summary>
        /// Adjusts the value of the given array by the given value
        /// </summary>
        public static void AddInPlace(this double[] array, double addValue)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] += addValue;
            }
        }

        /// <summary>
        /// creates a new array that is the sum of the given array and the second value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static double[] AddToArray(this double[] array, double addValue)
        {
            double[] OutArray = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                OutArray[i] = array[i] + addValue;
            }
            return OutArray;
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value
        /// </summary>
        public static void SubtractInPlace(this double[] array, double addValue)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] -= addValue;
            }
        }

        /// <summary>
        /// creates a new array that is the sum of the given array and the second value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static double[] SubtractFromArray(this double[] array, double addValue)
        {
            double[] OutArray = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                OutArray[i] = array[i] - addValue;
            }
            return OutArray;
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value
        /// </summary>
        public static void MultiplyInPlace(this double[] array, double Multiplicant)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] *= Multiplicant;
            }
        }

        /// <summary>
        /// creates a new array that is the sum of the given array and the second value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static double[] MultiplyToArray(this double[] array, double Multiplicant)
        {
            double[] OutArray = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                OutArray[i] = array[i] * Multiplicant;
            }
            return OutArray;
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value
        /// </summary>
        public static void DivideInPlace(this double[] array, double Divisor)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] /= Divisor;
            }
        }

        /// <summary>
        /// creates a new array that is the sum of the given array and the second value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static double[] DivideToArray(this double[] array, double Divisor)
        {
            double[] OutArray = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                OutArray[i] = array[i] / Divisor;
            }
            return OutArray;
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value
        /// </summary>
        public static void AddInPlace(this double[] array, double[] addValue)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] += addValue[i];
            }
        }

        /// <summary>
        /// creates a new array that is the sum of the given array and the second value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static double[] AddToArray(this double[] array, double[] addValue)
        {
            double[] OutArray = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                OutArray[i] = array[i] + addValue[i];
            }
            return OutArray;
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value
        /// </summary>
        public static void SubtractInPlace(this double[] array, double[] addValue)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] -= addValue[i];
            }
        }

        /// <summary>
        /// creates a new array that is the sum of the given array and the second value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static double[] SubtractFromArray(this double[] array, double[] addValue)
        {
            double[] OutArray = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                OutArray[i] = array[i] - addValue[i];
            }
            return OutArray;
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value, there is no error checking
        /// </summary>
        public static void MultiplyInPlace(this double[] array, double[] Multiplicant)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] *= Multiplicant[i];
            }
        }

        /// <summary>
        /// creates a new array that is the sum of the given array and the second value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static double[] MultiplyToArray(this double[] array, double[] Multiplicant)
        {
            double[] OutArray = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                OutArray[i] = array[i] * Multiplicant[i];
            }
            return OutArray;
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value, there is no error checking
        /// </summary>
        public static void DivideInPlace(this double[] array, double[] Divisor)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] /= Divisor[i];
            }
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value, there is no error checking
        /// </summary>
        public static double[] DivideInPlaceErrorless(this double[] array, double[] Divisor)
        {
            for (int i = 0; i < array.Length; i++)
            {
                try
                {
                    array[i] /= Divisor[i];
                }
                catch
                {
                    try
                    {
                        array[i] = array[i - 1];
                    }
                    catch { }
                }
            }
            return array;
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value, there is no error checking
        /// </summary>
        public static void LogInPlaceErrorless(this double[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                try
                {
                    array[i] = Math.Log(array[i]);
                }
                catch
                {
                    try
                    {
                        array[i] = array[i - 1];
                    }
                    catch { array[i] = 0; }
                }
            }
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value, there is no error checking
        /// </summary>
        public static void LogInPlaceErrorless(this double[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    try
                    {
                        array[i, j] = Math.Log(array[i, j]);
                    }
                    catch
                    {
                        try
                        {
                            array[i, j] = array[i - 1, j];
                        }
                        catch { array[i, j] = 0; }
                    }
                }
            }
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value, there is no error checking
        /// </summary>
        public static void LogInPlaceErrorlessImage(this double[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (array[i, j] == 0)
                    {
                        array[i, j] = 0;
                    }
                    else
                    {
                        try
                        {
                            array[i, j] = Math.Log(array[i, j]);
                        }
                        catch
                        {
                            try
                            {
                                array[i, j] = array[i - 1, j];
                            }
                            catch { array[i, j] = 0; }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// creates a new array that is the sum of the given array and the second value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static double[,] LogErrorless(this double[,] array)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    try
                    {
                        OutArray[i, j] = Math.Log(array[i, j]);
                    }
                    catch
                    {
                        try
                        {
                            OutArray[i, j] = array[i - 1, j];
                        }
                        catch { OutArray[i, j] = 0; }
                    }
                }
            }
            return OutArray;
        }

        /// <summary>
        /// creates a new array that is the sum of the given array and the second value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static double[] DivideToArray(this double[] array, double[] Divisor)
        {
            double[] OutArray = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                OutArray[i] = array[i] / Divisor[i];
            }
            return OutArray;
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value, there is no error checking
        /// </summary>
        public static void AddInPlace(this double[,] array, double addValue)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] += addValue;
                }
            }
        }

        /// <summary>
        /// creates a new array that is the sum of the given array and the second value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static double[,] AddToArray(this double[,] array, double addValue)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    OutArray[i, j] = array[i, j] + addValue;
                }
            }
            return OutArray;
        }

        /// <summary>
        /// performs cast on whole array to convert from double to float
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static float[] ConvertToFloat(this double[] array)
        {
            float[] OutArray = new float[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                OutArray[i] = (float)array[i];
            }
            return OutArray;
        }

        /// <summary>
        /// performs cast on whole array to convert from double to float
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static float[,] ConvertToFloat(this double[,] array)
        {
            float[,] OutArray = new float[array.GetLength(0), array.GetLength(1)];
            unsafe
            {
                fixed (float* pOut = OutArray)
                {
                    fixed (double* pIn = array)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            pOut[i] = (float)pIn[i];
                        }
                    }
                }
            }
            return OutArray;
        }

        /// <summary>
        /// performs cast on whole array to convert from double to float
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static float[, ,] ConvertToFloat(this double[, ,] array)
        {
            float[, ,] OutArray = new float[array.GetLength(0), array.GetLength(1), array.GetLength(2)];
            unsafe
            {
                fixed (float* pOut = OutArray)
                {
                    fixed (double* pIn = array)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            pOut[i] = (float)pIn[i];
                        }
                    }
                }
            }
            return OutArray;
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value, there is no error checking
        /// </summary>
        public static void SubtractInPlace(this double[,] array, double addValue)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] -= addValue;
                }
            }
        }

        /// <summary>
        /// creates a new array that is the sum of the given array and the second value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static double[,] SubtractFromArray(this double[,] array, double addValue)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    OutArray[i, j] = array[i, j] - addValue;
                }
            }
            return OutArray;
        }

        /// <summary>
        /// Adjusts the value of the given array by the given value, there is no error checking
        /// </summary>
        public static void MultiplyInPlace(this double[,] array, double Multiplicant)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] *= Multiplicant;
                }
            }
        }

        /// <summary>
        /// creates a new array that is the sum of the given array and the second value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static double[,] MultiplyToArray(this double[,] array, double Multiplicant)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    OutArray[i, j] = array[i, j] * Multiplicant;
                }
            }
            return OutArray;
        }

        /// <summary>
        /// returns the sum of all the elements in an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double SumArray(this double[] array)
        {
            double sum = 0;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                sum += array[i];
            }
            return sum;
        }
        /// <summary>
        /// returns the sum of all the elements in an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double SumArray(this double[,] array)
        {
            double sum = 0;
            unsafe
            {
                fixed (double* pArray = array)
                {
                    double* ppArray = pArray;
                    for (int i = 0; i < array.Length; i++)
                    {
                        sum += *ppArray;
                        ppArray++;
                    }
                }
            }
            return sum;
        }

        /// <summary>
        /// returns the sum of all the elements in an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe double SumArray(this double[, ,] array)
        {
            double sum = 0;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    sum += *pAOut;

                    pAOut++;
                }
            }
            return sum;
        }
        /*
                /// <summary>
                /// returns the max of all the elements of an array
                /// </summary>
                /// <param name="array"></param>
                /// <returns></returns>
                public static unsafe double MaxArray(this double[, ,] array)
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
                /// <summary>
                /// returns the min of all the elements of an array
                /// </summary>
                /// <param name="array"></param>
                /// <returns></returns>
                public static unsafe double MinArray(this double[, ,] array)
                {
                    double min = double.MaxValue;
                    fixed (double* pArray = array)
                    {
                        double* pAOut = pArray;

                        for (int i = 0; i < array.Length; i++)
                        {
                            if (min > *pAOut) min = *pAOut;
                            pAOut++;
                        }
                    }
                    return min;
                }

                /// <summary>
                /// returns the max of all the elements of an array
                /// </summary>
                /// <param name="array"></param>
                /// <returns></returns>
                public static unsafe double MaxArray(this double[,] array)
                {
                    double max = double.MinValue;
                    fixed (double* pArray = array)
                    {
                        double* pAOut = pArray;

                        for (int i = 0; i < array.Length; i++)
                        {
                            if (max < *pAOut) max = *pAOut;
                            pAOut++;
                        }
                    }
                    return max;
                }

                /// <summary>
                /// returns the min of all the elements of an array
                /// </summary>
                /// <param name="array"></param>
                /// <returns></returns>
                public static unsafe double MinArray(this double[,] array)
                {
                    double min = double.MaxValue;
                    fixed (double* pArray = array)
                    {
                        double* pAOut = pArray;

                        for (int i = 0; i < array.Length; i++)
                        {
                            if (min > *pAOut) min = *pAOut;
                            pAOut++;
                        }
                    }
                    return min;
                }
                */
        /// <summary>
        /// returns the stdev of the array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe double StandardDeviation(this double[] array)
        {
            double average = array.Average();
            double sum = 0, x = 0;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    x = ((*pAOut) - average);
                    sum += x * x;
                    pAOut++;
                }
            }

            return Math.Sqrt(sum / (array.Length - 1));
        }

        /// <summary>
        /// returns the max of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe double MaxArray(this double[] array)
        {
            double max = double.MinValue;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (max < *pAOut) max = *pAOut;
                    pAOut++;
                }
            }
            return max;
        }
        /// <summary>
        /// returns the min of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe double MinArray(this double[] array)
        {
            double min = double.MaxValue;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (min > *pAOut) min = *pAOut;
                    pAOut++;
                }
            }
            return min;
        }


        /// <summary>
        /// returns the max of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe double MaxArray(this double[,] array)
        {
            double max = double.MinValue;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (max < *pAOut) max = *pAOut;
                    pAOut++;
                }
            }
            return max;
        }

        /// <summary>
        /// returns the max of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe double MaxArray(this double[,] array,out int X, out int Y)
        {
            double max = double.MinValue;
            long MaxI = 0;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (max < *pAOut)
                    {
                        max = *pAOut;
                        MaxI = i;
                    }
                    pAOut++;
                }
            }

            Y = (int)(MaxI / (double)array.GetLength(0));
            X = (int)(MaxI % array.GetLength(0));

            return max;
        }


        /// <summary>
        /// returns the min of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe float MinArray(this float[] array)
        {
            float min = float.MaxValue;
            fixed (float* pArray = array)
            {
                float* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (min > *pAOut) min = *pAOut;
                    pAOut++;
                }
            }
            return min;
        }


        /// <summary>
        /// returns the max of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe float MaxArray(this float[,] array)
        {
            float max = float.MinValue;
            fixed (float* pArray = array)
            {
                float* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (max < *pAOut) max = *pAOut;
                    pAOut++;
                }
            }
            return max;
        }


        /// <summary>
        /// returns the max of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        /* public static unsafe float MaxArray(this float[, ,] array)
         {
             float max = float.MinValue;
             fixed (float* pArray = array)
             {
                 float* pAOut = pArray;

                 for (int i = 0; i < array.Length; i++)
                 {
                     if (max < *pAOut) max = *pAOut;
                     pAOut++;
                 }
             }
             return max;
         }

         /// <summary>
         /// returns the max of all the elements of an array
         /// </summary>
         /// <param name="array"></param>
         /// <returns></returns>
         public static unsafe float MinArray(this float[, ,] array)
         {
             float min = float.MaxValue;
             fixed (float* pArray = array)
             {
                 float* pAOut = pArray;

                 for (int i = 0; i < array.Length; i++)
                 {
                     if (min > *pAOut) min = *pAOut;
                     pAOut++;
                 }
             }
             return min;
         }*/

        /// <summary>
        /// returns the min of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe double MinArray(this double[,] array)
        {
            double min = double.MaxValue;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (min > *pAOut) min = *pAOut;
                    pAOut++;
                }
            }
            return min;
        }

        /// <summary>
        /// returns the min of all the elements of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static unsafe float MinArray(this float[,] array)
        {
            float min = float.MaxValue;
            fixed (float* pArray = array)
            {
                float* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    if (min > *pAOut) min = *pAOut;
                    pAOut++;
                }
            }
            return min;
        }


        /// <summary>
        /// Scales an array while keeping the same average
        /// </summary>
        /// <param name="array"></param>
        /// <param name="ScaleFactor"></param>
        /// <returns></returns>
        public static unsafe double[] RescaleArrayInPlace(this double[] array, double ScaleFactor)
        {
            double average = array.Average();
            double x;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    x = ((*pAOut) - average) * ScaleFactor;
                    *pAOut = x + average;
                    pAOut++;
                }
            }
            return array;
        }


        public static double AverageArray(this double[] array)
        {
            double sum = 0;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                sum += array[i];
            }
            return sum / (double)array.Length;
        }

        public static double AverageArray(this double[,] array)
        {
            double sum = 0;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    sum += array[i, j];
                }
            }
            return sum / (double)array.Length;
        }

        public static double AverageArray(this Int32[,] array)
        {
            double sum = 0;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    sum += array[i, j];
                }
            }
            return sum / (double)array.Length;
        }
        public static double AverageArray(this byte[,] array)
        {
            double sum = 0;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    sum += array[i, j];
                }
            }
            return sum / (double)array.Length;
        }

        public static unsafe double AverageArray(this double[, ,] array)
        {
            double sum = 0;
            fixed (double* pArray = array)
            {
                double* pAOut = pArray;

                for (int i = 0; i < array.Length; i++)
                {
                    sum += *pAOut;
                    pAOut++;
                }
            }
            return sum / (double)array.Length;
        }

        /// <summary>
        /// performs a operation on every elements in the array, no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        public static void DivideInPlace(this double[,] array, double Divisor)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] /= Divisor;
                }
            }
        }

        /// <summary>
        /// performs a operation on every elements in the array, no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        public static void DivideInPlace(this float[, ,] array, float Divisor)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    for (int k = 0; k < array.GetLength(2); k++)
                        array[i, j, k] /= Divisor;
                }
            }
        }

        /// <summary>
        /// Creates a new array using the original array and the value.  each element is considered individually. There is no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        /// <returns></returns>
        public static double[,] DivideToArray(this double[,] array, double Divisor)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            unsafe
            {
                fixed (double* pIn = array)
                {
                    fixed (double* pOut = OutArray)
                    {
                        double* PIn = pIn, POut = pOut;
                        for (int i = 0; i < array.Length; i++)
                        {
                            *PIn = (*POut) / Divisor;
                            PIn++;
                            POut++;
                        }
                    }
                }
            }
            return OutArray;
        }

        /// <summary>
        /// performs a operation on every elements in the array, no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        public static double[, ,] DivideToArray(this double[, ,] array, double Divisor)
        {
            double[, ,] OutArray = new double[array.GetLength(0), array.GetLength(1), array.GetLength(2)];
            unsafe
            {
                fixed (double* pIn = array)
                {
                    fixed (double* pOut = OutArray)
                    {
                        double* PIn = pIn, POut = pOut;
                        for (int i = 0; i < array.Length; i++)
                        {
                            *POut = (*PIn) / Divisor;
                            PIn++;
                            POut++;
                        }
                    }
                }
            }
            return OutArray;
        }

        /// <summary>
        /// performs a operation on every elements in the array, no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        public static void AddInPlace(this double[,] array, double[,] addValue)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] += addValue[i, j];
                }
            }
        }

        /// <summary>
        /// Creates a new array using the original array and the value.  each element is considered individually. There is no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        /// <returns></returns>
        public static double[,] AddToArray(this double[,] array, double[,] addValue)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    OutArray[i, j] = array[i, j] + addValue[i, j];
                }
            }
            return OutArray;
        }

        public static void SubtractInPlace(this double[,] array, double[,] addValue)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] -= addValue[i, j];
                }
            }
        }

        /// <summary>
        /// Creates a new array using the original array and the value.  each element is considered individually. There is no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        /// <returns></returns>
        public static double[,] SubtractFromArray(this double[,] array, double[,] addValue)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    OutArray[i, j] = array[i, j] - addValue[i, j];
                }
            }
            return OutArray;
        }
        /// <summary>
        /// performs a operation on every elements in the array, no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        public static void MultiplyInPlace(this double[,] array, double[,] Multiplicant)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] *= Multiplicant[i, j];
                }
            }
        }
        /// <summary>
        /// Creates a new array using the original array and the value.  each element is considered individually. There is no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        /// <returns></returns>
        public static double[,] MultiplyToArray(this double[,] array, double[,] Multiplicant)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    OutArray[i, j] = array[i, j] * Multiplicant[i, j];
                }
            }
            return OutArray;
        }
        /// <summary>
        /// performs a operation on every elements in the array, no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        public static void DivideInPlace(this double[,] array, double[,] Divisor)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] /= Divisor[i, j];
                }
            }
        }
        /// <summary>
        /// Creates a new array using the original array and the value.  each element is considered individually. There is no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        /// <returns></returns>
        public static double[,] DivideToArray(this double[,] array, double[,] Divisor)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    OutArray[i, j] = array[i, j] / Divisor[i, j];
                }
            }
            return OutArray;
        }

        /// <summary>
        /// performs a operation on every elements in the array, no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        public static void AddInPlace(this double[,] array, double[] addValue)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] += addValue[i];
                }
            }
        }
        /// <summary>
        /// Creates a new array using the original array and the value.  each element is considered individually. There is no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        /// <returns></returns>
        public static double[,] AddToArray(this double[,] array, double[] addValue)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    OutArray[i, j] = array[i, j] + addValue[i];
                }
            }
            return OutArray;
        }
        /// <summary>
        /// performs a operation on every elements in the array, no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        public static void SubtractInPlace(this double[,] array, double[] addValue)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] -= addValue[i];
                }
            }
        }
        /// <summary>
        /// Creates a new array using the original array and the value.  each element is considered individually. There is no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        /// <returns></returns>
        public static double[,] SubtractFromArray(this double[,] array, double[] addValue)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    OutArray[i, j] = array[i, j] - addValue[i];
                }
            }
            return OutArray;
        }
        /// <summary>
        /// performs a operation on every elements in the array, no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        public static void MultiplyInPlace(this double[,] array, double[] Multiplicant)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] *= Multiplicant[i];
                }
            }
        }
        /// <summary>
        /// Creates a new array using the original array and the value.  each element is considered individually. There is no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        /// <returns></returns>
        public static double[,] MultiplyToArray(this double[,] array, double[] Multiplicant)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    OutArray[i, j] = array[i, j] * Multiplicant[i];
                }
            }
            return OutArray;
        }
        /// <summary>
        /// performs a operation on every elements in the array, no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        public static void DivideInPlace(this double[,] array, double[] Divisor)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] /= Divisor[i];
                }
            }
        }
        /// <summary>
        /// Creates a new array using the original array and the value.  each element is considered individually. There is no error checking
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Divisor"></param>
        /// <returns></returns>
        public static double[,] DivideToArray(this double[,] array, double[] Divisor)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    OutArray[i, j] = array[i, j] / Divisor[i];
                }
            }
            return OutArray;
        }
        #endregion

        /// <summary>
        /// Pulls a array from the given array with the dimensions of the given ROI
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startX"></param>
        /// <param name="endX"></param>
        /// <param name="startY"></param>
        /// <param name="endY"></param>
        /// <returns></returns>
        public static double[,] SubArray(this double[,] array, int startX, int endX, int startY, int endY)
        {
            double[,] OutArray = new double[endX - startX, endY - startY];
            int cX = 0, cy = 0;
            for (int x = startX; x < endX; x++)
            {
                cy = 0;
                for (int y = startY; y < endY; y++)
                {
                    OutArray[cX, cy] = array[x, y];
                    cy++;
                }
                cX++;
            }
            return OutArray;
        }

        /// <summary>
        /// Pulls a smaller array from the given array with size given by width and height, centered on the center of the original array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="WidthX"></param>
        /// <param name="WidthY"></param>
        /// <returns></returns>
        public static double[,] CutFromCenter(this double[,] array, int WidthX, int WidthY)
        {
            int startX, endX, startY, endY;
            int halfX = array.GetLength(0) / 2;
            int halfY = array.GetLength(1) / 2;

            startX = halfX - WidthX;
            startY = halfY - WidthY;
            endX = halfX + WidthX;
            endY = halfY + WidthY;
            return SubArray(array, startX, endX, startY, endY);
        }

        /// <summary>
        /// Pulls a smaller array from the given array with size given by width and height, centered on the center of the original array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="NewLengthX"></param>
        /// <param name="NewLengthY"></param>
        /// <returns></returns>
        public static double[,] ReducePaddedData(this double[,] array, int NewLengthX, int NewLengthY)
        {
            double[,] NewArray = new double[NewLengthX, NewLengthY];

            int OffsetX = (int)((array.GetLength(0) - NewLengthX) / 2d);
            int OffsetY = (int)((array.GetLength(1) - NewLengthY) / 2d);

            for (int i = 0; i < NewArray.GetLength(0); i++)
            {
                for (int j = 0; j < NewArray.GetLength(1); j++)
                {
                    NewArray[i, j] = array[i + OffsetX, j + OffsetY];
                }
            }
            return NewArray;
        }

        /// <summary>
        /// gets one line from a 2 array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="YIndex"></param>
        /// <returns></returns>
        public static double[] GetXLine(this double[,] array, int YIndex)
        {
            double[] OutArray = new double[array.GetLength(0)];
            for (int i = 0; i < array.GetLength(0); i++)
                OutArray[i] = array[i, YIndex];
            return OutArray;
        }
        /// <summary>
        /// gets one line from a 2 array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="YIndex"></param>
        /// <returns></returns>
        public static double[] GetYLine(this double[,] array, int XIndex)
        {
            double[] OutArray = new double[array.GetLength(1)];
            for (int i = 0; i < array.GetLength(1); i++)
                OutArray[i] = array[XIndex, i];
            return OutArray;
        }

        /// <summary>
        /// Inverts an array using the max value of the array, a new array is created in the process
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[,] InvertArrayValues(this double[,] array)
        {
            double Max = array.MaxArray();
            double Min = array.MinArray();

            double[,] outArray = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    outArray[i, j] = Max - array[i, j];
                }
            }
            return outArray;
        }

        /// <summary>
        /// Inverts an array using the max value of the array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[,] InvertArrayValuesInPlace(this double[,] array)
        {
            double Max = array.MaxArray();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = Max - array[i, j];
                }
            }
            return array;
        }
        /// <summary>
        /// Inverts an array using the given value for the max
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[,] InvertArrayValuesInPlace(this double[,] array, double MaxValue)
        {
            double Max = MaxValue;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = Max - array[i, j];
                }
            }
            return array;
        }

        /// <summary>
        /// pulls a plane from a 3D array with 1st dim constant
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <param name="SliceIndex">the value to be constant (first dim of array)</param>
        /// <returns></returns>
        public static double[,] SliceXAxis(this double[, ,] ImageArray, int SliceIndex)
        {
            int iWidth = ImageArray.GetLength(2);
            int iHeight = ImageArray.GetLength(1);
            double[,] OutArray = new double[iWidth, iHeight];


            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    OutArray[x, y] = ImageArray[SliceIndex, y, x];
                }
            }
            return OutArray;
        }

        /// <summary>
        /// pulls a plane from a 3D array with 2st dim constant
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <param name="SliceIndex">the value to be constant (2nd dim of array)</param>
        /// <returns></returns>
        public static double[,] SliceYAxis(this double[, ,] ImageArray, int SliceIndex)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(2);

            double[,] OutArray = new double[iWidth, iHeight];

            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    OutArray[x, y] = ImageArray[x, SliceIndex, y];
                }
            }
            return OutArray;
        }
        /// <summary>
        /// pulls a plane from a 3D array with 3st dim constant
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <param name="SliceIndex">the value to be constant (3nd dim of array)</param>
        /// <returns></returns>
        public static double[,] SliceZAxis(this double[, ,] ImageArray, int SliceIndex)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);

            double[,] OutArray = new double[iWidth, iHeight];
            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    OutArray[x, y] = ImageArray[x, y, SliceIndex];
                }
            }
            return OutArray;
        }

        /// <summary>
        /// pulls a plane from a 3D array with 1st dim constant
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <param name="SliceIndex">the value to be constant (first dim of array)</param>
        /// <returns></returns>
        public static float[,] SliceXAxis(this float[, ,] ImageArray, int SliceIndex)
        {
            int iWidth = ImageArray.GetLength(2);
            int iHeight = ImageArray.GetLength(1);
            float[,] OutArray = new float[iWidth, iHeight];


            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    OutArray[x, y] = ImageArray[SliceIndex, y, x];
                }
            }
            return OutArray;
        }

        /// <summary>
        /// pulls a plane from a 3D array with 2st dim constant
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <param name="SliceIndex">the value to be constant (2nd dim of array)</param>
        /// <returns></returns>
        public static float[,] SliceYAxis(this float[, ,] ImageArray, int SliceIndex)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(2);

            float[,] OutArray = new float[iWidth, iHeight];

            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    OutArray[x, y] = ImageArray[x, SliceIndex, y];
                }
            }
            return OutArray;
        }
        /// <summary>
        /// pulls a plane from a 3D array with 3st dim constant
        /// </summary>
        /// <param name="ImageArray"></param>
        /// <param name="SliceIndex">the value to be constant (3nd dim of array)</param>
        /// <returns></returns>
        public static float[,] SliceZAxis(this float[, ,] ImageArray, int SliceIndex)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);

            float[,] OutArray = new float[iWidth, iHeight];
            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    OutArray[x, y] = ImageArray[x, y, SliceIndex];
                }
            }
            return OutArray;
        }

        /// <summary>
        /// pulls a series of pixels from the line defined by p1 to p2.  Values are not interpolated
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double[] GetProfileLine(this double[,] SourceImage, Point p1, Point p2)
        {
            int iWidth = SourceImage.GetLength(0);
            int iHeight = SourceImage.GetLength(1);

            int Length = (int)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            double[] Profile = new double[Length];

            double tx = (double)(p2.X - p1.X) / (double)Length;
            double ty = (double)(p2.Y - p1.Y) / (double)Length;
            double sX = p1.X;
            double sY = p1.Y;
            int x = 0, y = 0;

            //todo: clip the line at the boundries of the array
            try
            {
                unsafe
                {
                    for (int t = 0; t < Length; t++)
                    {
                        y = (int)Math.Round(sY);
                        x = (int)Math.Round(sX);
                        Profile[t] = SourceImage[x, y];
                        sY += ty;
                        sX += tx;
                    }
                }
            }
            catch { }
            return Profile;
        }

        /// <summary>
        /// saves a array into the raw format for saving double arays
        /// </summary>
        /// <param name="array"></param>
        /// <param name="Filename"></param>
        public static void SaveArrayAsRaw(this double[,] array, string Filename)
        {
            FileStream BinaryFile = new FileStream(Filename, FileMode.Create, FileAccess.Write);
            BinaryWriter Writer = new BinaryWriter(BinaryFile);

            Writer.Write((Int32)PhysicalArrayRank.Array2D);

            Writer.Write((Int32)array.GetLength(0));
            Writer.Write((Int32)array.GetLength(1));
            Writer.Write((Int32)1);

            Writer.Write((double)0);
            Writer.Write((double)0);

            Writer.Write((double)0);
            Writer.Write((double)0);

            Writer.Write((double)0);
            Writer.Write((double)0);

            for (int z = 0; z < 1; z++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    for (int x = 0; x < array.GetLength(0); x++)
                    {
                        Writer.Write((double)array[x, y]);
                    }
                }
            }
            Writer.Close();
            BinaryFile.Close();
        }
    }
}
