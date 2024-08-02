using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Transformations;

namespace MathHelpLib
{
    public class FFTMathNetLib:IFFTPluginHandler 
    {
        private RealFourierTransformation rft = new RealFourierTransformation(TransformationConvention.Default);
        private ComplexFourierTransformation cft = new ComplexFourierTransformation(TransformationConvention.Default);

        #region Actual FFTCalls
       
        /// These are the codes to actually talk to the math.net implimentation, not just manipulate the data
       
        private void FFTreal(double[] array, out double[] OutReal, out double[] OutImag)
        {
            rft.TransformForward(array, out OutReal, out OutImag);
        }
        private void iFFTreal(double[] arrayReal, double[] arrayImag, out double[] OutReal)
        {
            rft.TransformBackward(arrayReal, arrayImag, out OutReal);
        }

        private void FFTreal(double[] array, int DimX, int DimY, out double[] OutReal, out double[] OutImag)
        {
            rft.TransformForward(array, out OutReal, out OutImag, DimX, DimY);
        }
        private void iFFTreal(double[] arrayReal, double[] arrayImag, int DimX, int DimY, out double[] OutReal)
        {
            rft.TransformBackward(arrayReal, arrayImag, out OutReal, DimX, DimY);
        }

        private void FFTcomplex(ref double[] arrayRealImag)
        {
            cft.TransformForward(arrayRealImag);
        }
        private void iFFTcomplex(ref double[] arrayRealImag)
        {
            cft.TransformBackward(arrayRealImag);
        }
        private void FFTcomplex(ref double[] arrayRealImag, int DimX, int DimY)
        {
            cft.TransformForward(arrayRealImag, DimX, DimY);
        }
        private void iFFTcomplex(ref double[] arrayRealImag, int DimX, int DimY)
        {
            cft.TransformBackward(arrayRealImag, DimX, DimY);
        }

        #endregion

       
        /// Temporary arrays to change the data types to fit the requested format
        /// since most calls are done repeatedly with the same size arrays, we use these to allow a little more speed
      //  Math.net requires that all arrays are 1D 
        private double[] TempArrayComplex = null;
        private double[] TempArrayReal = null;
        private double[] TempArrayImag = null;

        #region ArrayFormatting

        private void CopyDoubleArrayToComplex(double[,] array)
        {
            //avoid reinitializing the temparray
            if (TempArrayComplex == null || TempArrayComplex.Length != array.Length * 2)
            {
                TempArrayComplex = new double[array.Length * 2];
            }
            unsafe
            {
                //the complex format is just real imag real image
                fixed (double* pTempArray = TempArrayComplex)
                {
                    fixed (double* pArray = array)
                    {
                        double* pIn = pArray;
                        double* pOut = pTempArray;
                        for (int i = 0; i < array.Length; i++)
                        {
                            *pOut = *pIn;
                            *(pOut + 1) = 0;
                            pOut += 2;
                            pIn++;
                        }
                    }
                }
            }
        }
        private void CopyDoubleArrayToReal(double[,] array)
        {
            //avoid reinitializing the temparray
            if (TempArrayReal == null || TempArrayReal.Length != array.Length)
            {
                TempArrayReal = new double[array.Length];
                TempArrayImag = new double[array.Length];
            }
            unsafe
            {
                fixed (double* pTempArray = TempArrayReal)
                {
                    fixed (double* pArray = array)
                    {
                        double* pIn = pArray;
                        double* pOut = pTempArray;
                        for (int i = 0; i < array.Length; i++)
                        {
                            *pOut = *pIn;
                            pOut++;
                            pIn++;
                        }
                    }
                }
            }
        }
        private void CopyDoubleArrayTo2D(double[] array, ref  double[,] OutArray)
        {
            unsafe
            {
                fixed (double* pOutArray = OutArray)
                {
                    fixed (double* pArray = array)
                    {
                        double* pIn = pArray;
                        double* pOut = pOutArray;
                        for (int i = 0; i < array.Length; i++)
                        {
                            *pOut = *pIn;
                            pOut++;
                            pIn++;
                        }
                    }
                }
            }
        }

        private void CopyDoubleArrayToComplex(double[] array)
        {
            //avoid reinitializing the temparray
            if (TempArrayComplex == null || TempArrayComplex.Length != array.Length * 2)
            {
                TempArrayComplex = new double[array.Length * 2];
            }
            unsafe
            {
                //the complex format is just real imag real image
                fixed (double* pTempArray = TempArrayComplex)
                {
                    fixed (double* pArray = array)
                    {
                        double* pIn = pArray;
                        double* pOut = pTempArray;
                        for (int i = 0; i < array.Length; i++)
                        {
                            *pOut = *pIn;
                            *(pOut + 1) = 0;
                            pOut += 2;
                            pIn++;
                        }
                    }
                }
            }
        }
        private void CopyDoubleArrayToReal(double[] array)
        {
            //avoid reinitializing the temparray
            if (TempArrayReal == null || TempArrayReal.Length != array.Length)
            {
                TempArrayReal = new double[array.Length];
                TempArrayImag = new double[array.Length];
            }
            unsafe
            {
                fixed (double* pTempArray = TempArrayReal)
                {
                    fixed (double* pArray = array)
                    {
                        double* pIn = pArray;
                        double* pOut = pTempArray;
                        for (int i = 0; i < array.Length; i++)
                        {
                            *pOut = *pIn;
                            pOut++;
                            pIn++;
                        }
                    }
                }
            }
        }
        private void CopyDoubleArray(double[] array, ref  double[] OutArray)
        {
            unsafe
            {
                fixed (double* pOutArray = OutArray)
                {
                    fixed (double* pArray = array)
                    {
                        double* pIn = pArray;
                        double* pOut = pOutArray;
                        for (int i = 0; i < array.Length; i++)
                        {
                            *pOut = *pIn;
                            pOut++;
                            pIn++;
                        }
                    }
                }
            }
        }

        private unsafe void CopyDoubleArrayToComplex(double* pArray, int Length)
        {
            //avoid reinitializing the temparray
            if (TempArrayComplex == null || TempArrayComplex.Length != Length * 2)
            {
                TempArrayComplex = new double[Length * 2];
            }
            unsafe
            {
                //the complex format is just real imag real image
                fixed (double* pTempArray = TempArrayComplex)
                {
                    double* pIn = pArray;
                    double* pOut = pTempArray;
                    for (int i = 0; i < Length; i++)
                    {
                        *pOut = *pIn;
                        *(pOut + 1) = 0;
                        pOut += 2;
                        pIn++;
                    }

                }
            }
        }
        private unsafe void CopyDoubleArrayToReal(double* pArray, int Length)
        {
            //avoid reinitializing the temparray
            if (TempArrayReal == null || TempArrayReal.Length != Length)
            {
                TempArrayReal = new double[Length];
                TempArrayImag = new double[Length];
            }
            unsafe
            {
                fixed (double* pTempArray = TempArrayReal)
                {

                    double* pIn = pArray;
                    double* pOut = pTempArray;
                    for (int i = 0; i < Length; i++)
                    {
                        *pOut = *pIn;
                        pOut++;
                        pIn++;
                    }

                }
            }
        }
        private unsafe void CopyDoubleArray(double* pArray, int Length, double* pOutArray)
        {
            unsafe
            {

                double* pIn = pArray;
                double* pOut = pOutArray;
                for (int i = 0; i < Length; i++)
                {
                    *pOut = *pIn;
                    pOut++;
                    pIn++;
                }

            }
        }


        private void CopyComplexArrayToReal(complex[,] array)
        {
            //avoid reinitializing the temparray
            if (TempArrayReal == null || TempArrayReal.Length != array.Length)
            {
                TempArrayReal = new double[array.Length];
                TempArrayImag = new double[array.Length];
            }
            unsafe
            {
                //the complex format is just real imag real image
                fixed (double* pTempArray = TempArrayReal)
                {
                    fixed (complex* pArray = array)
                    {
                        double* pIn = (double*)pArray;
                        double* pOut = pTempArray;
                        for (int i = 0; i < array.Length; i++)
                        {
                            *pOut = *pIn;
                            pOut++;
                            pIn += 2;
                        }
                    }
                }
            }
        }
        private void CopyComplexArrayToImag(complex[,] array)
        {
            //avoid reinitializing the temparray
            if (TempArrayReal == null || TempArrayReal.Length != array.Length)
            {
                TempArrayReal = new double[array.Length];
                TempArrayImag = new double[array.Length];
            }
            unsafe
            {
                //the complex format is just real imag real image
                fixed (double* pTempArray = TempArrayReal)
                {
                    fixed (complex* pArray = array)
                    {
                        double* pIn = (double*)pArray + 1;
                        double* pOut = pTempArray + 1;
                        for (int i = 0; i < array.Length - 1; i++)
                        {
                            *pOut = *pIn;
                            pOut++;
                            pIn += 2;
                        }
                    }
                }
            }
        }
        private void CopyComplexArrayToRealAndImage(complex[,] array)
        {
            //avoid reinitializing the temparray
            if (TempArrayReal == null || TempArrayReal.Length != array.Length)
            {
                TempArrayReal = new double[array.Length];
                TempArrayImag = new double[array.Length];
            }
            unsafe
            {
                fixed (double* pTempArrayR = TempArrayReal)
                {
                    fixed (double* pTempArrayI = TempArrayImag)
                    {
                        fixed (complex* pArray = array)
                        {
                            double* pIn = (double*)pArray;
                            double* pOutR = pTempArrayR;
                            double* pOutI = pTempArrayI;
                            for (int i = 0; i < array.Length; i++)
                            {
                                *pOutR = *pIn;
                                pIn++;
                                *pOutI = *pIn;
                                pOutR++;
                                pOutI++;
                                pIn++;
                            }
                        }
                    }
                }
            }
        }
        private void CopyComplexArrayToRealAndImage(complex[] array)
        {
            //avoid reinitializing the temparray
            if (TempArrayReal == null || TempArrayReal.Length != array.Length)
            {
                TempArrayReal = new double[array.Length];
                TempArrayImag = new double[array.Length];
            }
            unsafe
            {
                fixed (double* pTempArrayR = TempArrayReal)
                {
                    fixed (double* pTempArrayI = TempArrayImag)
                    {
                        fixed (complex* pArray = array)
                        {
                            double* pIn = (double*)pArray;
                            double* pOutR = pTempArrayR;
                            double* pOutI = pTempArrayI;
                            for (int i = 0; i < array.Length; i++)
                            {
                                *pOutR = *pIn;
                                pIn++;
                                *pOutI = *pIn;
                                pOutR++;
                                pOutI++;
                                pIn++;
                            }
                        }
                    }
                }
            }
        }
        private unsafe void CopyComplexArrayToRealAndImage(complex* pArray, int Length)
        {
            //avoid reinitializing the temparray
            if (TempArrayReal == null || TempArrayReal.Length != Length)
            {
                TempArrayReal = new double[Length];
                TempArrayImag = new double[Length];
            }
            unsafe
            {
                fixed (double* pTempArrayR = TempArrayReal)
                {
                    fixed (double* pTempArrayI = TempArrayImag)
                    {

                        double* pIn = (double*)pArray;
                        double* pOutR = pTempArrayR;
                        double* pOutI = pTempArrayI;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOutR = *pIn;
                            pIn++;
                            *pOutI = *pIn;
                            pOutR++;
                            pOutI++;
                            pIn++;
                        }
                    }
                }
            }
        }


        private unsafe void CopyRealToDoubleArray(double* pOutArray)
        {
            unsafe
            {
                fixed (double* pArray = TempArrayReal)
                {
                    double* pIn = pArray;
                    double* pOut = pOutArray;
                    for (int i = 0; i < TempArrayReal.Length; i++)
                    {
                        *pOut = *pIn;
                        pOut++;
                        pIn++;
                    }
                }
            }
        }

        private void CopyRealAndImageToComplexArray(ref complex[] array)
        {
            unsafe
            {
                fixed (double* pTempArrayR = TempArrayReal)
                {
                    fixed (double* pTempArrayI = TempArrayImag)
                    {
                        fixed (complex* pArray = array)
                        {
                            double* pIn = (double*)pArray;
                            double* pOutR = pTempArrayR;
                            double* pOutI = pTempArrayI;
                            for (int i = 0; i < array.Length; i++)
                            {
                                *pIn = *pOutR;
                                pIn++;
                                *pIn = *pOutI;
                                pOutR++;
                                pOutI++;
                                pIn++;
                            }
                        }
                    }
                }
            }
        }
        private void CopyRealAndImageToComplexArray(ref complex[,] array)
        {
            unsafe
            {
                fixed (double* pTempArrayR = TempArrayReal)
                {
                    fixed (double* pTempArrayI = TempArrayImag)
                    {
                        fixed (complex* pArray = array)
                        {
                            double* pIn = (double*)pArray;
                            double* pOutR = pTempArrayR;
                            double* pOutI = pTempArrayI;
                            for (int i = 0; i < array.Length; i++)
                            {
                                *pIn = *pOutR;
                                pIn++;
                                *pIn = *pOutI;
                                pOutR++;
                                pOutI++;
                                pIn++;
                            }
                        }
                    }
                }
            }
        }
        private unsafe void CopyRealAndImageToComplexArray(complex* pArray, int Length)
        {

            {
                fixed (double* pTempArrayR = TempArrayReal)
                {
                    fixed (double* pTempArrayI = TempArrayImag)
                    {

                        {
                            double* pIn = (double*)pArray;
                            double* pOutR = pTempArrayR;
                            double* pOutI = pTempArrayI;
                            for (int i = 0; i < Length; i++)
                            {
                                *pIn = *pOutR;
                                pIn++;
                                *pIn = *pOutI;
                                pOutR++;
                                pOutI++;
                                pIn++;
                            }
                        }
                    }
                }
            }
        }


        private void CopyComplexArrayToComplex(complex[,] array)
        {
            //remember that temparraycomplex is double array
            //avoid reinitializing the temparray
            if (TempArrayComplex == null || TempArrayComplex.Length != array.Length * 2)
            {
                TempArrayComplex = new double[array.Length * 2];
            }
            unsafe
            {
                fixed (double* pTempArray = TempArrayComplex)
                {
                    fixed (complex* pArray = array)
                    {
                        complex* pIn = pArray;
                        complex* pOut = (complex*)pTempArray;
                        for (int i = 0; i < array.Length; i++)
                        {
                            *pOut = *pIn;
                            pOut++;
                            pIn++;
                        }
                    }
                }
            }
        }
        private void CopyComplexArrayToComplex(complex[] array)
        {
            //avoid reinitializing the temparray
            if (TempArrayComplex == null || TempArrayComplex.Length != array.Length * 2)
            {
                TempArrayComplex = new double[array.Length * 2];
            }
            unsafe
            {
                fixed (double* pTempArray = TempArrayComplex)
                {
                    fixed (complex* pArray = array)
                    {
                        double* pIn = (double*)pArray;
                        double* pOut = pTempArray;
                        for (int i = 0; i < array.Length * 2; i++)
                        {
                            *pOut = *pIn;
                            pOut++;
                            pIn++;
                        }
                    }
                }
            }
        }
        private void CopyComplexArrayTo2D(complex[] array, ref  complex[,] OutArray)
        {
            unsafe
            {
                fixed (complex* pOutArray = OutArray)
                {
                    fixed (complex* pArray = array)
                    {
                        complex* pIn = pArray;
                        complex* pOut = pOutArray;
                        for (int i = 0; i < array.Length; i++)
                        {
                            *pOut = *pIn;
                            pOut++;
                            pIn++;
                        }
                    }
                }
            }
        }
        private unsafe void CopyComplexArrayToComplex(complex* pArray, int Length)
        {
            //avoid reinitializing the temparray
            if (TempArrayComplex == null || TempArrayComplex.Length != Length * 2)
            {
                TempArrayComplex = new double[Length * 2];
            }
            unsafe
            {
                //the complex format is just real imag real image
                fixed (double* pTempArray = TempArrayComplex)
                {
                    double* pIn = (double*)pArray;
                    double* pOut = pTempArray;
                    for (int i = 0; i < Length ; i++)
                    {
                        *pOut = *pIn;
                        pOut ++;
                        pIn++;
                    }

                }
            }
        }
        private unsafe void CopyComplexArrayToReal(complex* pArray, int Length)
        {
            //avoid reinitializing the temparray
            if (TempArrayReal == null || TempArrayReal.Length != Length)
            {
                TempArrayReal = new double[Length];
                TempArrayImag = new double[Length];
            }
            unsafe
            {
                fixed (double* pTempArray = TempArrayReal)
                {
                    //the complex format is just real imag real image
                    double* pIn = (double*)pArray;
                    double* pOut = pTempArray;
                    for (int i = 0; i < Length; i++)
                    {
                        *pOut = *pIn;
                        *(pOut + 1) = 0;
                        pOut++;
                        pIn += 2;
                    }

                }
            }
        }
        private unsafe void CopyComplexArray(complex* pArray, int Length, complex* pOutArray)
        {
            unsafe
            {

                complex* pIn = pArray;
                complex* pOut = pOutArray;
                for (int i = 0; i < Length; i++)
                {
                    *pOut = *pIn;
                    pOut++;
                    pIn++;
                }

            }
        }
        private unsafe void CopyComplexArray(double* pArray, int Length, complex* pOutArray)
        {
            unsafe
            {

                double* pIn = pArray;
                double* pOut = (double*)pOutArray;
                for (int i = 0; i < Length * 2; i++)
                {
                    *pOut = *pIn;
                    pOut++;
                    pIn++;
                }

            }
        }
        private unsafe void CopyComplexArray(double[] array, int Length, complex* pOutArray)
        {
            unsafe
            {
                fixed (double* pArray = array)
                {
                    double* pIn = pArray;
                    double* pOut = (double*)pOutArray;
                    for (int i = 0; i < Length * 2; i++)
                    {
                        *pOut = *pIn;
                        pOut++;
                        pIn++;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Not allowed with this lib
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public double[] FFTreal2real(double[] array)
        {
            throw new Exception("This library does not impliment a real to real fft transformation");
        }
        /// <summary>
        /// Not allowed with this lib
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public unsafe void FFTreal2real(double* pArray, int Length, double* pArrayOut)
        {
            throw new Exception("This library does not impliment a real to real fft transformation");
        }
        /// <summary>
        /// Not allowed with this lib
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public double[,] FFTreal2real(double[,] array)
        {
            throw new Exception("This library does not impliment a real to real fft transformation");
        }
        /// <summary>
        /// Not allowed with this lib
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public unsafe void FFTreal2real(double* pArray, int LengthX,int LengthY, double* pArrayOut)
        {
            throw new Exception("This library does not impliment a real to real fft transformation");
        }


        /// <summary>
        /// Takes complex data, does the inverse tranformation and only allows reals
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public double[] iFFTcomplex2real(complex[] array)
        {
            double[] OutArray = new double[array.Length];
            CopyComplexArrayToRealAndImage(array);
            iFFTreal(TempArrayReal, TempArrayImag, out TempArrayReal);
            Buffer.BlockCopy(TempArrayReal, 0, OutArray, 0, array.Length * 8);
            return OutArray;
        }
        /// <summary>
        /// Takes complex data, does the inverse tranformation and only allows reals
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public double[,] iFFTcomplex2real(complex[,] array)
        {
            double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
            CopyComplexArrayToRealAndImage(array);
            iFFTreal(TempArrayReal, TempArrayImag, array.GetLength(0), array.GetLength(1), out TempArrayReal);
            Buffer.BlockCopy(TempArrayReal, 0, OutArray, 0, array.Length * 8);
            return OutArray;
        }
        /// <summary>
        /// Takes complex data, does the inverse tranformation and only allows reals
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public unsafe void iFFTcomplex2real(complex* pArray, int Length, double* pArrayOut)
        {

            CopyComplexArrayToRealAndImage(pArray, Length);
            iFFTreal(TempArrayReal, TempArrayImag, out TempArrayReal);
            CopyRealToDoubleArray(pArrayOut);
        }
        /// <summary>
        /// Takes complex data, does the inverse tranformation and only allows reals
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public unsafe void iFFTcomplex2real(complex* pArray, int LengthX, int LengthY, double* pArrayOut)
        {
            CopyComplexArrayToRealAndImage(pArray, LengthX * LengthY);
            iFFTreal(TempArrayReal, TempArrayImag, LengthX, LengthY, out TempArrayReal);
            CopyRealToDoubleArray(pArrayOut);
        }

        /// <summary>
        /// performs fft on real data
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public complex[] FFTreal2complex(double[] array)
        {
            complex[] OutArray = new complex[array.Length];
            FFTreal(array, out  TempArrayReal, out  TempArrayImag);
            CopyRealAndImageToComplexArray(ref OutArray);
            return OutArray;
        }
        double[] Linear = null;
        /// <summary>
        /// performs fft on real data
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public complex[,] FFTreal2complex(double[,] array)
        {
            complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
            if (Linear == null || Linear.Length != array.Length)
                Linear = new double[array.Length];
            unsafe
            {
                fixed (double* pArray = array)
                {
                    fixed (double* pInArray = Linear)
                    {
                        double* pOut = pArray;
                        double* pIn = pInArray;

                        for (int i = 0; i < array.Length; i++)
                        {
                            *pIn = *pOut;
                            pIn++;
                            pOut++;
                        }
                    }
                }
            }
            FFTreal(Linear, array.GetLength(0), array.GetLength(1), out TempArrayReal, out TempArrayImag);
            CopyRealAndImageToComplexArray(ref OutArray);
            return OutArray;
        }
        /// <summary>
        /// performs fft on real data
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public unsafe void FFTreal2complex(double* pArray, int Length, complex* pArrayOut)
        {
            if (Linear == null || Linear.Length != Length)
                Linear = new double[Length];
            unsafe
            {

                fixed (double* pInArray = Linear)
                {
                    double* pOut = pArray;
                    double* pIn = pInArray;

                    for (int i = 0; i < Length; i++)
                    {
                        *pIn = *pOut;
                        pIn++;
                        pOut++;
                    }

                }
            }
            FFTreal(Linear, out  TempArrayReal, out TempArrayImag);
            CopyRealAndImageToComplexArray(pArrayOut, Length);
        }
        /// <summary>
        /// performs fft on real data
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public unsafe void FFTreal2complex(double* pArray, int LengthX, int LengthY, complex* pArrayOut)
        {
            if (Linear == null || Linear.Length != LengthX*LengthY )
                Linear = new double[LengthX*LengthY];
            unsafe
            {
                
                    fixed (double* pInArray = Linear)
                    {
                        double* pOut = pArray;
                        double* pIn = pInArray;

                        for (int i = 0; i < LengthX*LengthY; i++)
                        {
                            *pIn = *pOut;
                            pIn++;
                            pOut++;
                        }
                    }
              
            }
            FFTreal(Linear, LengthX, LengthY, out  TempArrayReal, out  TempArrayImag);
            CopyRealAndImageToComplexArray(pArrayOut, LengthX * LengthY);
        }


        public double[] iFFTreal2real(double[] array)
        {
            CopyDoubleArrayToReal(array);
            iFFTreal(TempArrayReal, TempArrayImag, out TempArrayReal);
            CopyDoubleArray(TempArrayReal, ref array);
            return array;
        }
        public double[,] iFFTreal2real(double[,] array)
        {
            CopyDoubleArrayToReal(array);
            iFFTreal(TempArrayReal, TempArrayImag, array.GetLength(0), array.GetLength(1), out TempArrayReal);
            CopyDoubleArrayTo2D(TempArrayReal, ref array);
            return array;
        }
        public unsafe void iFFTreal2real(double* pArray, int Length, double* pArrayOut)
        {
            CopyDoubleArrayToReal(pArray, Length);
            iFFTreal(TempArrayReal, TempArrayImag, out TempArrayReal);
            CopyDoubleArray(pArray, Length, pArrayOut);
        }
        public unsafe void iFFTreal2real(double* pArray, int LengthX, int LengthY, double* pArrayOut)
        {
            CopyDoubleArrayToReal(pArray, LengthX * LengthY);
            iFFTreal(TempArrayReal, TempArrayImag, LengthX, LengthY, out TempArrayReal);
            CopyDoubleArray(pArray, LengthX * LengthY, pArrayOut);
        }


        public complex[] FFTcomplex2complex(complex[] array)
        {
            CopyComplexArrayToComplex(array);
            FFTcomplex(ref TempArrayComplex);
            Buffer.BlockCopy(TempArrayComplex, 0, array, 0, array.Length * 16);
            return array;
        }
        public complex[,] FFTcomplex2complex(complex[,] array)
        {
            CopyComplexArrayToComplex(array);
            FFTcomplex(ref TempArrayComplex);
            Buffer.BlockCopy(TempArrayComplex, 0, array, 0, array.Length * 16);
            return array;
        }
        public unsafe void FFTcomplex2complex(complex* pArray, int Length, complex* pArrayOut)
        {
            CopyComplexArrayToComplex(pArray, Length);
            FFTcomplex(ref TempArrayComplex);
            CopyComplexArray(TempArrayComplex, Length, pArrayOut);
        }
        public unsafe void FFTcomplex2complex(complex* pArray, int LengthX, int LengthY, complex* pArrayOut)
        {
            CopyComplexArrayToComplex(pArray, LengthY * LengthX);
            FFTcomplex(ref TempArrayComplex);
            CopyComplexArray(TempArrayComplex, LengthX * LengthY, pArrayOut);
        }

        public complex[] iFFTcomplex2complex(complex[] array)
        {
            CopyComplexArrayToComplex(array);
            iFFTcomplex(ref TempArrayComplex);
            Buffer.BlockCopy(TempArrayComplex, 0, array, 0, array.Length * 16);
            return array;
        }
        public complex[,] iFFTcomplex2complex(complex[,] array)
        {
            CopyComplexArrayToComplex(array);
            iFFTcomplex(ref TempArrayComplex);
            Buffer.BlockCopy(TempArrayComplex, 0, array, 0, array.Length * 16);
            return array;
        }
        public unsafe void iFFTcomplex2complex(complex* pArray, int Length, complex* pArrayOut)
        {
            CopyComplexArrayToComplex(pArray, Length);
            iFFTcomplex(ref TempArrayComplex);
            CopyComplexArray(TempArrayComplex, Length, pArrayOut);
        }
        public unsafe void iFFTcomplex2complex(complex* pArray, int LengthX, int LengthY, complex* pArrayOut)
        {
            CopyComplexArrayToComplex(pArray, LengthY * LengthX);
            iFFTcomplex(ref TempArrayComplex);
            CopyComplexArray(TempArrayComplex, LengthX * LengthY, pArrayOut);
        }
    }
}
