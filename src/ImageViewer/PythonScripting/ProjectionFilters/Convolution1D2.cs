using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ImageViewer.Filters;
using MathHelpLib;
using System.Runtime.InteropServices;



namespace ImageViewer.PythonScripting.Projection
{
    public class Convolution1D : aEffectNoForm
    {
        public override string EffectName { get { return "Convolution 1D"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Rank Order Statistical"; } }
        public override int OrderSuggestion { get { return 1; } }

        #region DLL Imports
        [DllImport("SimpleConvolution.dll")]
        private static unsafe extern IntPtr CreateConvoluter(string KernalName, string ConvolutionCode, string CompilationFlags,
            double* OutputImage, double* Image, int ImageWidth, int ImageHeight, double* Filter, int FilterWidth, int FilterHeight);

        [DllImport("SimpleConvolution.dll")]
        private static unsafe extern int RunConvoluter(IntPtr Convoluter, double* Image);

        [DllImport("SimpleConvolution.dll")]
        private static unsafe extern int RunConvoluterInputOutput(IntPtr Convoluter, double* Image, double* output);

        [DllImport("SimpleConvolution.dll")]
        private static unsafe extern int CloseConvoluter(IntPtr Convoluter);
        #endregion
        
        ~Convolution1D()
        {

        }

        public void Dispose()
        {
            try
            {
                if (GPUToken != null)
                {
                    if (GPUToken.Convoluter != IntPtr.Zero)
                    {
                        // CloseConvoluter(GPUToken.Convoluter);
                    }

                    GPUToken.MaskHandle.Free();
                    GPUToken.MaskAddress = IntPtr.Zero;

                    GPUToken.OutHandle.Free();
                    GPUToken.OutAddress = IntPtr.Zero;
                }
                GPUToken = null;
            }
            catch { }

        }

        GPUConvolutionToken GPUToken = null;

        /// <summary>
        /// Does the convolution, first tries with GPU then with CPU
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="Kernal"></param>
        /// <param name="ConvolutionAxis">int(0 means X, 1 means Y)</param>
        /// <param name="CutDown" > Amount to clip off the egdes of the image </param>
        /// <returns></returns>
        public double[,] DoConvolution(Bitmap SourceImage, double[] Kernal, int ConvolutionAxis, double CutDown)
        {
            double[,] DataIn = null;
            int OriginalWidth = 0;
            int OriginalHeight = 0;

            if (mDataEnvironment.RunningOnGPU == true)
            {
                lock (CriticalSectionLock)
                {
                    if (GPUToken == null)
                    {
                        if (mDataEnvironment.EffectTokens.ContainsKey("1DConvolution") == true)
                            GPUToken = (GPUConvolutionToken)mDataEnvironment.EffectTokens["1DConvolution"];
                        else
                        {
                            GPUToken = new GPUConvolutionToken();
                            mDataEnvironment.EffectTokens.Add("1DConvolution", GPUToken);
                        }
                    }
                }
                //test the GPU first to see if it works
                if (GPUToken.GPUError == false && File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\simpleConvolution.dll") == true)
                {

                    if (ConvolutionAxis == 0)
                    {
                        OriginalWidth = (int)(SourceImage.Width * CutDown);
                        OriginalHeight = (int)(SourceImage.Height * CutDown);
                        DataIn = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArrayPowerOf2(SourceImage, false);
                    }
                    else
                    {
                        OriginalWidth = (int)(SourceImage.Height * CutDown);
                        OriginalHeight = (int)(SourceImage.Width * CutDown);
                        DataIn = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArrayPowerOf2(SourceImage, true);
                    }

                    try
                    {
                        //run the GPU convolution
                        return ConvolveGPU(DataIn, Kernal, OriginalWidth, OriginalHeight);
                    }
                    catch
                    {
                        GPUToken.GPUError = true;
                        mDataEnvironment.RunningOnGPU = false;
                    }
                }
            }
            //do the convolution on the cpu
            double[,] DataInDouble;
            if (DataIn == null)
            {
                if (ConvolutionAxis == 0)
                    DataInDouble = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, false);
                else
                    DataInDouble = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, true);
            }
            else
            {
                DataInDouble = new double[DataIn.GetLength(0), DataIn.GetLength(1)];

                Buffer.BlockCopy(DataIn, 0, DataInDouble, 0, Buffer.ByteLength(DataIn));
                // DataIn.CopyTo(DataInDouble, 0);
            }
            //do the convolution
            return Convolve(DataInDouble, Kernal);
        }
        /// <summary>
        /// Does the convolution, first tries with GPU then with CPU
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="Kernal"></param>
        /// <param name="ConvolutionAxis">int(0 means X, 1 means Y)</param>
        /// /// <param name="CutDown" > Amount to clip off the egdes of the image </param>
        /// <returns></returns>
        public double[,] DoConvolution(DataEnvironment dataEnvironment, ImageHolder SourceImage, double[] Kernal, int ConvolutionAxis, double CutDown)
        {
            mDataEnvironment = dataEnvironment;
            double[,] DataIn = null;
            int OriginalWidth = 0;
            int OriginalHeight = 0;

            lock (CriticalSectionLock)
            {
                if (GPUToken == null)
                {
                    if (mDataEnvironment.EffectTokens.ContainsKey("1DConvolution") == true)
                        GPUToken = (GPUConvolutionToken)mDataEnvironment.EffectTokens["1DConvolution"];
                    else
                    {
                        GPUToken = new GPUConvolutionToken();
                        mDataEnvironment.EffectTokens.Add("1DConvolution", GPUToken);
                    }
                }
            }
            GPUToken.GPUError = true;
            //test the GPU first to see if it works
            if (GPUToken.GPUError == false && File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\simpleConvolution.dll") == true)
            {
                #region Gpu Convolution
                if (ConvolutionAxis == 0)
                {
                    OriginalWidth = (int)(SourceImage.Width * CutDown);
                    OriginalHeight = (int)(SourceImage.Height * CutDown);
                    DataIn = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArrayPowerOf2(SourceImage, false);// ImagingTools.ConvertToFloatArrayPowerOf2(SourceImage, false);
                }
                else
                {
                    OriginalWidth = (int)(SourceImage.Height * CutDown);
                    OriginalHeight = (int)(SourceImage.Width * CutDown);
                    DataIn = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArrayPowerOf2(SourceImage, true);// ImagingTools.ConvertToFloatArrayPowerOf2(SourceImage, true);
                }

                try
                {
                    //run the GPU convolution
                    double[,] d= ConvolveGPU(DataIn, Kernal, OriginalWidth, OriginalHeight);
                    if (d == null)
                    {
                        GPUToken.GPUError = true;
                        mDataEnvironment.RunningOnGPU = false;
                        return DoNormalConvolution(Kernal,ConvolutionAxis,SourceImage);
                    }
                    return d;
                }
                catch
                {
                    GPUToken.GPUError = true;
                    mDataEnvironment.RunningOnGPU = false;
                }
                #endregion
            }

            return DoNormalConvolution(Kernal, ConvolutionAxis, SourceImage);
        }


        


        private double[,] DoNormalConvolution(double[] Kernal, int ConvolutionAxis, ImageHolder SourceImage)
        {
            double[,] DataInDouble;
                if (ConvolutionAxis == 0)
                    DataInDouble = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray((SourceImage), false);
                else
                    DataInDouble = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(SourceImage, true);
            //do the convolution
            return Convolve(DataInDouble, Kernal);
        }

        public float[,]  DoNormalConvolution(float[] Kernal, int ConvolutionAxis, ref ImageHolder SourceImage, Rectangle CutRegion)
        {
            
            float [,] Convolved =   Convolve(SourceImage.ImageData, Kernal);

            //do the convolution
            return Convolved;
        }


        /// <summary>
        /// Does the convolution of source image.  tries GPU first, then if this fails then works 
        /// with CPU.  
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">"Axis|int(0 means X, 1 means Y)", "Kernal|double[]", "ShowConvolution|bool", "return physical array|bool","PhysicalHeight|double", "PhysicalHeight|double" </param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
             mPassData = PassData;
            if (mFilterToken == null)
            {
                mFilterToken = DefaultProperties;
            }

            mDataEnvironment = dataEnvironment;

            double[] Kernal = (double[])mFilterToken[1];

            double[,] DataOut = null;

            // GPUError = true;
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                DataOut = DoConvolution((Bitmap)SourceImage, Kernal, (int)mFilterToken[0], .95);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
               // Console.WriteLine("MaxIntensity "+ ((ImageHolder)SourceImage).ImageData.MaxArray().ToString());
                DataOut = DoConvolution(dataEnvironment, (ImageHolder)SourceImage, Kernal, (int)mFilterToken[0], .95);
            }


            //if the user wants a physical array, return physical array with the desired dimensions
            if ((bool)mFilterToken[3] == true)
            {
                #region Physical array
                double dWidth = EffectHelps.ConvertToDouble(mFilterToken[4]) / 2d;
                double dHeight = EffectHelps.ConvertToDouble(mFilterToken[5]) / 2d;

                PhysicalArray PhysOut;
                if ((int)mFilterToken[0] == 0)
                    PhysOut = new PhysicalArray(DataOut, -1 * dWidth, dWidth, -1 * dHeight, dHeight, true);
                else
                    PhysOut = new PhysicalArray(DataOut, -1 * dWidth, dWidth, -1 * dHeight, dHeight, false);

                PassData.AddSafe("ConvolutionData", PhysOut);

                if ((bool)mFilterToken[2] == true)
                {
                    return new ImageHolder(PhysOut.MakeBitmap());
                }
                else
                    return SourceImage;
                #endregion
            }
            else
            {
                #region normal array
                if (DataOut == null)
                    System.Diagnostics.Debug.Print("");

                PassData.AddSafe("ConvolutionData", DataOut);

                if ((bool)mFilterToken[2] == true)
                {
                    return new ImageHolder(DataOut);
                }
                else
                    return SourceImage;
                #endregion
            }
        }

        #region GPU convolution
        //creating the GPU program is expensive, so only create it one time, only need to pass the kernal over once, and it only seems to 
        //work if the output is not changed, so always use the correct output buffer

        /*
        static bool ProgramCreated = false;
        static float[,] OutImage = null;

        static float[] Mask;
        static GCHandle MaskHandle;
        static IntPtr MaskAddress;

        static GCHandle OutHandle;
        static IntPtr OutAddress;
        */
        static object CriticalSectionLock = new object();

        // private class ActualGPUConvoluter
        // {

        public class GPUConvolutionToken : IEffectToken
        {
            public string TokenName()
            {
                return this.ToString();
            }

            public bool GPUError = false;
            public bool ProgramCreated = false;
            public double[,] OutImage = null;

            public double[] Mask;

            public IntPtr Convoluter;

            public GCHandle MaskHandle;
            public IntPtr MaskAddress;

            public GCHandle OutHandle;
            public IntPtr OutAddress;
        }
        /*public bool UsingGPU
        {
            get { return !GPUError; }

        }*/

        public unsafe double[,] ConvolveGPU(double[,] DataIn, double[] Kernal, int OriginalWidth, int OriginalHeight)
        {
            lock (CriticalSectionLock)
            {
                //set up the convoluter
                if (GPUToken.ProgramCreated == false)
                {

                    GPUToken.OutImage = new double[DataIn.GetLength(0), DataIn.GetLength(1)];
                    GPUToken.OutHandle = GCHandle.Alloc(GPUToken.OutImage, GCHandleType.Pinned);

                    GPUToken.Mask = Kernal;
                    GPUToken.MaskHandle = GCHandle.Alloc(GPUToken.Mask, GCHandleType.Pinned);
                    try
                    {
                        GPUToken.OutAddress = GPUToken.OutHandle.AddrOfPinnedObject();
                        // Get the address of the data array
                        GPUToken.MaskAddress = GPUToken.MaskHandle.AddrOfPinnedObject();
                        fixed (double* pImage = DataIn)
                        {
                            //we reuse the mask and output buffers to improve speed and because the output buffer cannot be changed
                            GPUToken.Convoluter = CreateConvoluter("simpleConvolution", ConvolutionProgram, " ", (double*)GPUToken.OutAddress, pImage, DataIn.GetLength(0), DataIn.GetLength(1), (double*)GPUToken.MaskAddress, GPUToken.Mask.GetLength(0), 1);
                            if (GPUToken.Convoluter == IntPtr.Zero)
                            {
                                GPUToken.GPUError = true;
                                mDataEnvironment.RunningOnGPU = false;
                                throw new Exception("GPU not functioning");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("GPUConvolution\r" + ex.Message);
                        GPUToken.GPUError = true;
                        mDataEnvironment.RunningOnGPU = false;
                    }
                    GPUToken.ProgramCreated = true;

                }


                int OffsetX = (int)((DataIn.GetLength(0) - OriginalHeight) / 2d);
                int OffsetY = (int)((DataIn.GetLength(1) - OriginalWidth) / 2d);

                double[,] DataOut = new double[OriginalHeight, OriginalWidth];
                int Width = DataIn.GetLength(0);

                int oWidth = DataOut.GetLength(1);
                int oHeight = DataOut.GetLength(0);
                //run the convolution.   Setting up the program is expensive, so we only do it once above, and just over and over again, reusing the output 
                //buffer.
                fixed (double* pImage = DataIn)
                {
                    RunConvoluter(GPUToken.Convoluter, pImage);

                    //only copy the range that matches the size of the original image
                    fixed (double* pDataOut = DataOut)
                    {
                        for (int y = 0; y < oHeight; y++)
                        {
                            double* pConvImage = (double*)GPUToken.OutAddress + (y + OffsetX) * Width + OffsetY;

                            double* pOut = pDataOut + y * oWidth;

                            for (int x = 0; x < oWidth; x++)
                            {
                                *pOut = *pConvImage;
                                pOut++;
                                pConvImage++;
                            }
                        }
                    }
                }
                double d = DataOut.SumArray();
                Console.Write(d);
                if (double.IsNaN(d))
                    Console.Write(d);
                return DataOut;
                //return OutImage;
            }
        }

        #region ConvolutionProgram
        //simple program to do 1D convolution on the fast scan axis.  should be optimised more
        static string ConvolutionProgram = @"

#pragma OPENCL EXTENSION cl_amd_fp64 : enable 
__kernel void simpleConvolution(__global  double  * output,
                                __global  double  * input,
                                __global  double  * mask,
                                const     uint2  inputDimensions,
                                const     uint2  maskDimensions)
{
    uint tid   = get_global_id(0);
    
    uint width  = inputDimensions.x;
    uint height = inputDimensions.y;
    
    uint x      = tid%width;
    uint y      = tid/width;
    
    uint maskWidth  = maskDimensions.x;
    uint maskHeight = maskDimensions.y;
    
    uint vstep = (maskWidth  -1)/2;
    uint hstep = (maskHeight -1)/2;
    
    /*
     * find the left, right, top and bottom indices such that
     * the indices do not go beyond image boundaires
     */
    int left  =(x - vstep);
    int right=(x + vstep); 
    
	/*
		 * initializing wighted sum value
		 */
	double sumFX = 0;

	if (left >=0 && right <width)
	{
////////////////////////////////////////////////////Main Area/////////////////////////////////////////////
		uint maskIndex =0;
		uint index = y * width;
		for(uint i = left; i <= right; ++i)
		{
				sumFX += (input[index+i] * mask[maskIndex]);
				maskIndex=maskIndex+1;
	    }  
   }
   else if (left<0 && right <width)
   {
////////////////////////////////////////////////////get left apron/////////////////////////////////////////////

		uint index     = y * width;
		uint maskIndex =-1*left;
        
	   for(uint i = 0; i <= right; ++i)
       {
				sumFX += (input[index+i] * mask[maskIndex]);
				maskIndex=maskIndex+1;
	   }
   }
   else if (left>0 && right >=width)
   {
/////////////////////////////////////////get right apron///////////////////////////////////////////////////////
		uint index     = y * width;
		uint maskIndex =0;
		for(uint i = left; i < width; ++i)
		{    
				sumFX += (input[index+i] * mask[maskIndex]);
				maskIndex =maskIndex+1;
	   }
   }
   else 
   {
/////////////////////////////////////////get both aprons///////////////////////////////////////////////////////
		uint index     = y * width;
    	uint maskIndex =-1*left;

        for(uint i = 0; i < width; ++i)
		{
				sumFX += (input[index+i] * mask[maskIndex]);
				maskIndex =maskIndex+1;
	   }
   }

    output[tid] = sumFX;
}

";

        #endregion
        #endregion
        //  }
        #region NormalConvolution
        /// <summary>
        /// queues the lines of the input image
        /// </summary>
        /// <param name="DataIn"></param>
        /// <param name="Kernal"></param>
        /// <returns></returns>
        public static unsafe double[,] Convolve(double[,] DataIn, double[] Kernal)
        {
            double[,] ArrayOut = new double[DataIn.GetLength(0), DataIn.GetLength(1)];

           // double kernalSum = Kernal.Sum();

            fixed (double* pArrayIn = DataIn)
            {
                fixed (double* pKernal = Kernal)
                {
                    fixed (double* pOut = ArrayOut)
                    {
                        // ConvolveX(pArrayIn, pOut, DataIn.GetLength(0), DataIn.GetLength(1), pKernal, Kernal.Length);
                        int Width = DataIn.GetLength(0);
                        int Height = DataIn.GetLength(1);
                        for (int i = 0; i < Height; i++)
                        {
                            ConvoluteChop(pArrayIn + i * Width, Width, pKernal, Kernal.Length, pOut + i * Width);
                        }

                        //long Area = Width * Height;
                      /*  double* ppOut = pOut;
                        for (long i = 0; i < Area; i++)
                        {
                            (*ppOut) /= kernalSum;
                            ppOut++;
                        }*/
                    }
                }
            }
            return ArrayOut;
        }

        /// <summary>
        /// queues the lines of the input image
        /// </summary>
        /// <param name="DataIn"></param>
        /// <param name="Kernal"></param>
        /// <returns></returns>
        public static unsafe float[, ] Convolve(float[, ,] DataIn, float[] Kernal)
        {
            float[,] ArrayOut = new float[DataIn.GetLength(0), DataIn.GetLength(1)];

            // double kernalSum = Kernal.Sum();

            fixed (float* pArrayIn = DataIn)
            {
                fixed (float* pKernal = Kernal)
                {
                    fixed (float* pOut = ArrayOut)
                    {
                        // ConvolveX(pArrayIn, pOut, DataIn.GetLength(0), DataIn.GetLength(1), pKernal, Kernal.Length);
                        int Width = DataIn.GetLength(0);
                        int Height = DataIn.GetLength(1);
                        for (int i = 0; i < Height; i++)
                        {
                            ConvoluteChop(pArrayIn + i * Width, Width, pKernal, Kernal.Length, pOut + i * Width);
                        }

                        //long Area = Width * Height;
                        /*  double* ppOut = pOut;
                          for (long i = 0; i < Area; i++)
                          {
                              (*ppOut) /= kernalSum;
                              ppOut++;
                          }*/
                    }
                }
            }
            return ArrayOut;
        }

        /// <summary>
        /// queues the lines of the input image
        /// </summary>
        /// <param name="DataIn"></param>
        /// <param name="Kernal"></param>
        /// <returns></returns>
        public static unsafe float [,] Convolve(float [,] DataIn, double[] Kernal)
        {
            float[,] ArrayOut = new float[DataIn.GetLength(0), DataIn.GetLength(1)];

            // double kernalSum = Kernal.Sum();

            fixed (float* pArrayIn = DataIn)
            {
                fixed (double* pKernal = Kernal)
                {
                    fixed (float* pOut = ArrayOut)
                    {
                        // ConvolveX(pArrayIn, pOut, DataIn.GetLength(0), DataIn.GetLength(1), pKernal, Kernal.Length);
                        int Width = DataIn.GetLength(0);
                        int Height = DataIn.GetLength(1);
                        for (int i = 0; i < Height; i++)
                        {
                            ConvoluteChop(pArrayIn + i * Width, Width, pKernal, Kernal.Length, pOut + i * Width);
                        }

                        //long Area = Width * Height;
                        /*  double* ppOut = pOut;
                          for (long i = 0; i < Area; i++)
                          {
                              (*ppOut) /= kernalSum;
                              ppOut++;
                          }*/
                    }
                }
            }
            return ArrayOut;
        }


        /// <summary>
        /// queues the lines of the input image
        /// </summary>
        /// <param name="DataIn"></param>
        /// <param name="Kernal"></param>
        /// <returns></returns>
        public static unsafe double[,] Convolve(float[, ,] DataIn, double[] Kernal)
        {
            double[,] ArrayOut = new double[DataIn.GetLength(0), DataIn.GetLength(1)];

            // double kernalSum = Kernal.Sum();

            fixed (float* pArrayIn = DataIn)
            {
                fixed (double* pKernal = Kernal)
                {
                    fixed (double* pOut = ArrayOut)
                    {
                        // ConvolveX(pArrayIn, pOut, DataIn.GetLength(0), DataIn.GetLength(1), pKernal, Kernal.Length);
                        int Width = DataIn.GetLength(0);
                        int Height = DataIn.GetLength(1);
                        for (int i = 0; i < Height; i++)
                        {
                            ConvoluteChop(pArrayIn + i * Width, Width, pKernal, Kernal.Length, pOut + i * Width);
                        }

                        //long Area = Width * Height;
                        /*  double* ppOut = pOut;
                          for (long i = 0; i < Area; i++)
                          {
                              (*ppOut) /= kernalSum;
                              ppOut++;
                          }*/
                    }
                }
            }
            return ArrayOut;
        }

        /// <summary>
        /// Does the convolution along the fast memory access direction
        /// </summary>
        /// <param name="Array1"></param>
        /// <param name="Length1"></param>
        /// <param name="pImpulse"></param>
        /// <param name="Length2"></param>
        /// <param name="pArrayOut"></param>
        public static unsafe void ConvoluteChop(float* Array1, int Length1, double* pImpulse, int Length2, float* pArrayOut)
        {

            int LengthWhole = Length1 + Length2;

            int StartI = (int)Math.Truncate((double)LengthWhole / 2d - (double)Length1 / 2d);
            int EndI = (int)Math.Truncate((double)LengthWhole / 2d + (double)Length1 / 2d);
            if (EndI - StartI > Length1)
                EndI--;
            int sI, eI;


            double p1;
            double* p2;
            float* pOut;

            unchecked
            {
                for (int i = 0; i < Length1; i++)
                {
                    p1 = Array1[i];
                    sI = StartI - i;
                    eI = EndI - i;
                    if (eI > Length2) eI = Length2;
                    if (sI < 0) sI = 0;
                    if (sI < eI)
                    {
                        p2 = pImpulse + sI;
                        pOut = pArrayOut + i + sI - StartI;
                        for (int j = sI; j < eI; j++)
                        {
                            *pOut += (float)(p1 * (*p2));
                            pOut++;
                            p2++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does the convolution along the fast memory access direction
        /// </summary>
        /// <param name="Array1"></param>
        /// <param name="Length1"></param>
        /// <param name="pImpulse"></param>
        /// <param name="Length2"></param>
        /// <param name="pArrayOut"></param>
        public static unsafe void ConvoluteChop(float* Array1, int Length1, double* pImpulse, int Length2, double* pArrayOut)
        {

            int LengthWhole = Length1 + Length2;

            int StartI = (int)Math.Truncate((double)LengthWhole / 2d - (double)Length1 / 2d);
            int EndI = (int)Math.Truncate((double)LengthWhole / 2d + (double)Length1 / 2d);
            if (EndI - StartI > Length1)
                EndI--;
            int sI, eI;


            double p1;
            double* p2;
            double* pOut;

            unchecked
            {
                for (int i = 0; i < Length1; i++)
                {
                    p1 = Array1[i];
                    sI = StartI - i;
                    eI = EndI - i;
                    if (eI > Length2) eI = Length2;
                    if (sI < 0) sI = 0;
                    if (sI < eI)
                    {
                        p2 = pImpulse + sI;
                        pOut = pArrayOut + i + sI - StartI;
                        for (int j = sI; j < eI; j++)
                        {
                            *pOut += (p1 * (*p2));
                            pOut++;
                            p2++;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// queues the lines of the input image
        /// </summary>
        /// <param name="DataIn"></param>
        /// <param name="Kernal"></param>
        /// <returns></returns>
        public static unsafe double[,] Convolve(double[,,] DataIn, double[] Kernal)
        {
            double[,] ArrayOut = new double[DataIn.GetLength(0), DataIn.GetLength(1)];

            // double kernalSum = Kernal.Sum();

            fixed (double* pArrayIn = DataIn)
            {
                fixed (double* pKernal = Kernal)
                {
                    fixed (double* pOut = ArrayOut)
                    {
                        // ConvolveX(pArrayIn, pOut, DataIn.GetLength(0), DataIn.GetLength(1), pKernal, Kernal.Length);
                        int Width = DataIn.GetLength(0);
                        int Height = DataIn.GetLength(1);
                        for (int i = 0; i < Height; i++)
                        {
                            ConvoluteChop(pArrayIn + i * Width, Width, pKernal, Kernal.Length, pOut + i * Width);
                        }

                        //long Area = Width * Height;
                        /*  double* ppOut = pOut;
                          for (long i = 0; i < Area; i++)
                          {
                              (*ppOut) /= kernalSum;
                              ppOut++;
                          }*/
                    }
                }
            }
            return ArrayOut;
        }


        /// <summary>
        /// Does the convolution along the fast memory access direction
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="pImpulse"></param>
        /// <param name="impulseWidth"></param>
        /// <param name="pArrayOut"></param>
        public static unsafe void ConvoluteChop(double* image, int width, double* pImpulse, int impulseWidth, double* pArrayOut)
        {

            int LengthWhole = width + impulseWidth;

            int StartI = (int)Math.Truncate((double)LengthWhole / 2d - (double)width / 2d);
            int EndI = (int)Math.Truncate((double)LengthWhole / 2d + (double)width / 2d);
            if (EndI - StartI > width)
                EndI--;
            int sI, eI;


            double p1;
            double* p2;
            double* pOut;

            unchecked
            {
                for (int x = 0; x < width; x++)
                {
                    p1 = image[x];
                    sI = StartI - x;
                    eI = EndI - x;
                    if (eI > impulseWidth) eI = impulseWidth;
                    if (sI < 0) sI = 0;
                    if (sI < eI)
                    {
                        p2 = pImpulse + sI;
                        pOut = pArrayOut + x + sI - StartI;
                        for (int j = sI; j < eI; j++)
                        {
                            *pOut += p1 * (*p2);
                            pOut++;
                            p2++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does the convolution along the fast memory access direction
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="pImpulse"></param>
        /// <param name="impulseWidth"></param>
        /// <param name="pArrayOut"></param>
        public static unsafe void ConvoluteChop(float* image, int width, float* pImpulse, int impulseWidth, float* pArrayOut)
        {

            int LengthWhole = width + impulseWidth;

            int StartI = (int)Math.Truncate((double)LengthWhole / 2d - (double)width / 2d);
            int EndI = (int)Math.Truncate((double)LengthWhole / 2d + (double)width / 2d);
            if (EndI - StartI > width)
                EndI--;
            int sI, eI;


            float p1;
            float* p2;
            float* pOut;

            unchecked
            {
                for (int x = 0; x < width; x++)
                {
                    p1 = image[x];
                    sI = StartI - x;
                    eI = EndI - x;
                    if (eI > impulseWidth) eI = impulseWidth;
                    if (sI < 0) sI = 0;
                    if (sI < eI)
                    {
                        p2 = pImpulse + sI;
                        pOut = pArrayOut + x + sI - StartI;
                        for (int j = sI; j < eI; j++)
                        {
                            *pOut += p1 * (*p2);
                            pOut++;
                            p2++;
                        }
                    }
                }
            }
        }


        ///////////////////////////////////////////////////////////////////////////////
        // double precision float version
        ///////////////////////////////////////////////////////////////////////////////
        private unsafe void ConvolveX(double* ArrayIn, double* ArrayOut, int dataSizeX, int dataSizeY, double* kernelX, int kSizeX)
        {
            int i, j, k, m;

            // intermediate data buffer
            double* inPtr;                         // working pointers
            double* tmpPtr;                       // working pointers
            int kCenter, kOffset, endIndex;                 // kernel indice



            // covolve horizontal direction ///////////////////////

            // find center position of kernel (half of kernel size)
            kCenter = kSizeX >> 1;                          // center index of kernel array
            endIndex = dataSizeX - kCenter;                 // index for full kernel convolution

            // init working pointers
            inPtr = ArrayIn;
            tmpPtr = ArrayOut;                                   // store intermediate results from 1D horizontal convolution

            unchecked
            {
                // start horizontal convolution (x-direction)
                for (i = 0; i < dataSizeY; ++i)                    // number of rows
                {

                    kOffset = 0;                                // starting index of partial kernel varies for each sample

                    // COLUMN FROM index=0 TO index=kCenter-1
                    for (j = 0; j < kCenter; ++j)
                    {
                        *tmpPtr = 0;                            // init to 0 before accumulation

                        for (k = kCenter + kOffset, m = 0; k >= 0; --k, ++m) // convolve with partial of kernel
                        {
                            *tmpPtr += *(inPtr + m) * kernelX[k];
                        }
                        ++tmpPtr;                               // next output
                        ++kOffset;                              // increase starting index of kernel
                    }

                    // COLUMN FROM index=kCenter TO index=(dataSizeX-kCenter-1)
                    for (j = kCenter; j < endIndex; ++j)
                    {
                        *tmpPtr = 0;                            // init to 0 before accumulate

                        for (k = kSizeX - 1, m = 0; k >= 0; --k, ++m)  // full kernel
                        {
                            *tmpPtr += *(inPtr + m) * kernelX[k];
                        }
                        ++inPtr;                                // next input
                        ++tmpPtr;                               // next output
                    }

                    kOffset = 1;                                // ending index of partial kernel varies for each sample

                    // COLUMN FROM index=(dataSizeX-kCenter) TO index=(dataSizeX-1)
                    for (j = endIndex; j < dataSizeX; ++j)
                    {
                        *tmpPtr = 0;                            // init to 0 before accumulation

                        for (k = kSizeX - 1, m = 0; k >= kOffset; --k, ++m)   // convolve with partial of kernel
                        {
                            *tmpPtr += *(inPtr + m) * kernelX[k];
                        }
                        ++inPtr;                                // next input
                        ++tmpPtr;                               // next output
                        ++kOffset;                              // increase ending index of partial kernel
                    }

                    inPtr += kCenter;                           // next row
                }
            }

        }
        #endregion

        #region Menu Junk
        public override string PassDataDescription
        {
            get
            {
                return "PhysicalArray  returned with convolution data in PassData[\"ConvolutionData\"";
            }
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { 0, new double[10], 2, 2 }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Axis|int(0 means X, 1 means Y)", "Kernal|double[]", "ShowConvolution|bool", "return physical array|bool", "PhysicalHeight|double", "PhysicalHeight|double" }; }
        }
        #endregion

    }
}
