using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using MathHelpLib;

using System.Runtime.InteropServices;
using System.Threading;

using Cloo;
using MathHelpLib.ImageProcessing;


namespace MathHelpLib.Recon
{
    public class DoSliceBackProjectionSiddonEffectGPU
    {

        public static void RunRecon(ImageLibrary allImages)
        {
            string[] Platforms = AvailablePlatforms();
            SetupGPU(Platforms[0], 0);
            float[, ,] Data = new float[100, 100, 100];
            Data[0, 0, 0] = 5;
            //Set3DBuffer(Data);
            //DoProjections(allImages, Data);
            // ReadResult(ref Data);
        }


        #region GPU Code
        public static string FBProjectionCode = @"
 __kernel void rotateConvolution(  
		__global float  * imageOut,
		__constant float  * pImpulse,
		const uint  impulseWidth,
		const uint2  inputDimensions,
		__global float  * imageIn
)
{
    uint tid   = get_global_id(0);
    
    uint width  = inputDimensions.x;
    uint height = inputDimensions.y;
    
    uint x = tid%width;
    uint y = tid/width;
    
    uint ImpulseIND=0;
    int StartJ = y -impulseWidth/2 ;
    int EndJ = y +impulseWidth/2 ;

    if (StartJ <0)
    {
      StartJ=0;
      ImpulseIND=impulseWidth/2- y;
    }
    if (EndJ >height) EndJ=height-1;

    uint memIND=x + StartJ*width;
 
    float sum =0;
    for (int j = StartJ; j <EndJ; j++)
    {        
         sum = sum+ imageIn[memIND]* pImpulse[ImpulseIND];
		 ImpulseIND++; 
         memIND +=width;
    }

     uint outid = x*width+y;
    imageOut[outid]=sum;
}


__kernel void simpleConvolution(  
		__global float  * imageOut,
		__constant float  * pImpulse,
		const uint  impulseWidth,
		const uint2  inputDimensions,
		__global float  * imageIn
)
{
    uint tid   = get_global_id(0);
    
    uint width  = inputDimensions.x;
    //uint height = inputDimensions.y;
    
    uint x = tid%width;
    uint y = tid/width;
    
    uint ImpulseIND=0;
    int StartI = x -impulseWidth/2 ;
    int EndI = x +impulseWidth/2 ;

    if (StartI <0)
    {
      StartI=0;
      ImpulseIND=impulseWidth/2- x;
    }
    if (EndI >width) EndI=width-1;

    StartI=y*width+ StartI;
    EndI=y*width+ EndI+1;
 
    float sum =0;
    for (int j = StartI; j <EndI; j++)
    {
         sum = sum+ imageIn[j]* pImpulse[ImpulseIND];
		 ImpulseIND++; 
    }
    uint outid = x*width+y;
    imageOut[outid]=sum;
}

__kernel void FBProjection(  __global float  * cube,
                             
                             const    uint4  cubeDimensions,
						     const    uint2  imageDimensions,
							__global float  * imageIn,
                             const    float4 FastScanDirection,
							 __global float * LUT )
{

			uint tid   = get_global_id(0);
    
            uint LI = cubeDimensions.x;
            uint LJ = cubeDimensions.y;
            uint LK = cubeDimensions.z;

            uint LsI = imageDimensions.x;
            uint LsJ = imageDimensions.y;

            uint sliceSize = LI*LJ;
			uint K_ = tid /sliceSize;
            uint remander = tid % sliceSize;
            uint I_ = remander % LI;
            uint J_ = remander / LI;
            

            float sX, sY, sdotI, u;
            int lower_sI, lower_sJ;
            
            int LUT_Index;
            int LsI_1 = LsI - 1;
            
            float halfI,  halfK, halfIs, halfJs;
            halfI = (float)LI / 2;
            halfK = (float)LK / 2;

            halfIs = (float)LsI / 2;
            halfJs = (float)LsJ / 2;

            //tranform to slice index coords
            sY = (J_ - halfI);
            sX = (I_ - halfI);

            sdotI = sX * FastScanDirection.X + sY * FastScanDirection.Y + halfIs;
           //make sure that we are still in the recon volumn
            if (sdotI > 0 && sdotI < LsI_1)
            {                        
                 lower_sI = floor(sdotI);
                 u = sdotI - lower_sI;
                 lower_sJ = ((K_ - halfK)+ halfJs);
                 LUT_Index = (int)(u / 1.2f);
                 if (LUT_Index > 49)
                     LUT_Index = 49;

                 uint paintIndex = lower_sJ*LsI + lower_sI;
                 cube[tid] += imageIn[paintIndex] * LUT[LUT_Index] + imageIn[paintIndex + 1] * LUT[49 - LUT_Index];
            }
        
}
";
        public static string ConvolutionCode = @"
 
";

        #endregion

        static IList<ComputeDevice> devices;
        static ComputePlatform platform;
        static ComputeContext context;
        static ComputeKernel kernelFBP;
        static ComputeKernel kernelConvolve;
        static ComputeBuffer<float> ComputeDataVolume;
        static ComputeBuffer<float> ComputeImpulse;
        static ComputeCommandQueue commands;
        static ComputeEventList eventList;
        static ComputeProgram program;

        public static string[] AvailablePlatforms()
        {
            // Populate OpenCL Platform ComboBox
            string[] availablePlatforms = new string[ComputePlatform.Platforms.Count];
            for (int i = 0; i < availablePlatforms.Length; i++)
                availablePlatforms[i] = ComputePlatform.Platforms[i].Name;

            return availablePlatforms;
        }

        private static bool BuildProgram(string Platform, string ProgramSource, int Device, out  ComputeContext context, out ComputeProgram program)
        {
            context = null;
            program = null;
            devices = new List<ComputeDevice>();

            if (ComputePlatform.Platforms.Count == 0)
                return false;

            foreach (ComputePlatform cp in ComputePlatform.Platforms)
            {
                if (cp.Name == Platform)
                    platform = cp;
            }

            devices.Add(platform.Devices[Device]);

            if (devices.Count == 0)
                return false;

            ComputeContextPropertyList properties = new ComputeContextPropertyList(platform);
            context = new ComputeContext(devices, properties, null, IntPtr.Zero);


            // Create and build the opencl program.
            program = new ComputeProgram(context, ProgramSource);
            program.Build(null, null, null, IntPtr.Zero);
            return true;
        }

        public static bool SetupGPU(string Platform, int device)
        {
            bool ret = BuildProgram(Platform, ConvolutionCode + "\n" + FBProjectionCode, device, out context, out program);

            if (ret == true)
            {
                // Create the kernel function and set its arguments.
                kernelConvolve = program.CreateKernel("rotateConvolution");
                kernelFBP = program.CreateKernel("FBProjection");
                // Create the event wait list. An event list is not really needed for this example but it is important to see how it works.
                // Note that events (like everything else) consume OpenCL resources and creating a lot of them may slow down execution.
                // For this reason their use should be avoided if possible.
                eventList = new ComputeEventList();

                // Create the command queue. This is used to control kernel execution and manage read/write/copy operations.
                commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);

                return true;
            }
            return ret;
        }

        public static void SetImpulse(float[] impulse)
        {
            ComputeImpulse = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, impulse);
            kernelConvolve.SetMemoryArgument(2, ComputeImpulse);
            kernelConvolve.SetValueArgument<uint>(4, (uint)impulse.Length);
        }

        public static float[,] ConvolveImage(float[,] ImageData)
        {
            float[,] imageDataOut = new float[ImageData.GetLength(0), ImageData.GetLength(1)];
            unsafe
            {

                fixed (float* pImage = ImageData)
                {
                    fixed (float* pImageOut = imageDataOut)
                    {
                        ComputeBuffer<float> a = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, ImageData.LongLength, (IntPtr)pImage);

                        ComputeBuffer<float> ImageOutB = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, ImageData.LongLength, (IntPtr)pImageOut);

                        kernelConvolve.SetMemoryArgument(0, ImageOutB);
                        kernelConvolve.SetMemoryArgument(1, a);

                        uint[] dims = new uint[] { (uint)ImageData.GetLength(0), (uint)ImageData.GetLength(1) };
                        fixed (uint* pDims = dims)
                        {
                            kernelConvolve.SetArgument(3, (IntPtr)(sizeof(uint) * 2), (IntPtr)pDims);
                            //  kernelConvolve.SetValueArgument<uint>(3, (uint)ImageData.GetLength(0));
                            // kernelConvolve.SetValueArgument<uint>(4, (uint)ImageData.GetLength(1));


                            long WorkGroupSize = kernelConvolve.GetWorkGroupSize(devices[0]);


                            // Execute the kernel "count" times. After this call returns, "eventList" will contain an event associated with this command.
                            // If eventList == null or typeof(eventList) == ReadOnlyCollection<ComputeEventBase>, a new event will not be created.
                            commands.Execute(kernelConvolve, null, new long[] { ImageData.LongLength }, null, eventList);

                            // 1) Wait for the events in the list to finish,
                            eventList.Wait();

                            commands.ReadFromBuffer(ImageOutB, (IntPtr)pImageOut, true, 0, ImageData.LongLength, eventList);
                            a.Dispose();
                        }
                    }
                }
            }
            return imageDataOut;
        }

        public static void ConvolveAndProject(ImageHolder allImages, float[] impulse, ref  float[, ,] volume)
        {
            ComputeImpulse = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, impulse);

            long WorkGroupSize = kernelConvolve.GetWorkGroupSize(devices[0]);

            float[, ,] ImageData = allImages.ImageData;

            float[,] imageDataOut = new float[ImageData.GetLength(0), ImageData.GetLength(1)];
            unsafe
            {
                fixed (float* pImage = ImageData, pImageOut = imageDataOut, pCube = volume)
                {
                    uint[] imageDims = new uint[] { (uint)ImageData.GetLength(0), (uint)ImageData.GetLength(1) };
                    uint[] cubeDims = new uint[] { (uint)volume.GetLength(0), (uint)volume.GetLength(1), (uint)volume.GetLength(2), 0 };
                    fixed (uint* pImageDims = imageDims, pVolDims = cubeDims)
                    {
                        /*__global float  * imageOut,
                            __constant float  * pImpulse,
                            const uint  impulseWidth,
                            const uint2  inputDimensions,*/
                        ComputeBuffer<float> ImageOutB = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, ImageData.LongLength, (IntPtr)pImageOut);
                        kernelConvolve.SetMemoryArgument(0, ImageOutB);
                        kernelConvolve.SetMemoryArgument(1, ComputeImpulse);
                        kernelConvolve.SetValueArgument<uint>(2, (uint)impulse.Length);
                        kernelConvolve.SetArgument(3, (IntPtr)(sizeof(uint) * 2), (IntPtr)pImageDims);

                        /*__global float  * cube,
                         const    uint4  cubeDimensions,
                         const    uint2  imageDimensions,
                           __global float  * imageIn,
                        */
                        ComputeDataVolume = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, volume.LongLength, (IntPtr)pCube);
                        kernelFBP.SetMemoryArgument(0, ComputeDataVolume);
                        kernelFBP.SetArgument(1, (IntPtr)(sizeof(uint) * 4), (IntPtr)pVolDims);
                        kernelFBP.SetArgument(2, (IntPtr)(sizeof(uint) * 2), (IntPtr)pImageDims);
                        kernelFBP.SetMemoryArgument(3, ImageOutB);


                        //input the image into the convolution kernal
                        ComputeBuffer<float> inputImageBuffer = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, ImageData.LongLength, (IntPtr)pImage);
                        kernelConvolve.SetMemoryArgument(4, inputImageBuffer);

                        double AngleRadians = 0;
                        float[] fastScanDirection;
                        float[] LUT;
                        CreateLUT(AngleRadians, out fastScanDirection, out LUT);
                        ComputeBuffer<float> LUTBuffer = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, LUT);
                        kernelFBP.SetMemoryArgument(5, LUTBuffer);

                        // for (int i = 0; i < 500; i++)
                        {

                            AngleRadians = 0 / 500f * 360f / 180f * Math.PI;

                            CreateLUT(AngleRadians, out fastScanDirection, out LUT);

                            fixed (float* pFS_Dir = fastScanDirection)
                            {
                                kernelFBP.SetArgument(4, (IntPtr)(sizeof(uint) * 4), (IntPtr)pFS_Dir);


                                /* float[, ,] Data2 = allImages[i].ImageData;
                                 commands.WriteToBuffer(Data2, inputImageBuffer, false, new SysIntX3(0, 0, 0), new SysIntX3(0, 0, 0),
                                     new SysIntX3(Data2.GetLength(0), Data2.GetLength(1), Data2.GetLength(2)), eventList);*/

                                commands.WriteToBuffer(LUT, LUTBuffer, false, null);

                                //eventList = new ComputeEventList();
                                //now that all the memory is finally set up, run the kernals
                                commands.Execute(kernelConvolve, null, new long[] { ImageData.LongLength }, null, eventList);

                                // eventList.Wait();

                                //commands.ReadFromBuffer(ImageOutB, (IntPtr)pImageOut, true,0,imageDataOut.LongLength, null);


                                // Bitmap bb = imageDataOut.MakeBitmap();

                                // int bbb = bb.Width;
                                //now that all the memory is finally set up, run the kernals
                                commands.Execute(kernelFBP, null, new long[] { volume.LongLength }, null, eventList);

                                // 1) Wait for the events in the list to finish,

                                eventList.Wait();

                                eventList = null;
                            }
                        }
                    }
                    commands.ReadFromBuffer(ComputeDataVolume, (IntPtr)pCube, true, 0, volume.LongLength, null);
                }
            }
        }

        private struct uint2
        {
            public uint x;
            public uint y;
            public uint2(int x, int y)
            {
                this.x = (uint)x;
                this.y = (uint)y;
            }
            public uint2(uint[] x)
            {
                this.x = (uint)x[0];
                this.y = (uint)x[1];
            }
        }

        private struct uint4
        {
            public uint x;
            public uint y;
            public uint z;
            public uint w;
            public uint4(int x, int y,int z, int w)
            {
                this.x = (uint)x;
                this.y = (uint)y;
                this.z = (uint)z;
                this.w = (uint)w;
            }
             public uint4(uint[] x)
            {
                this.x = (uint)x[0];
                this.y = (uint)x[1];
                this.z = (uint)x[2];
                this.w = (uint)x[3];
            }
        }

        private static unsafe void rotateConvolution(float* imageOut, float* pImpulse, int impulseWidth, uint2 inputDimensions, float* imageIn, uint tid)
        {

            int width = (int)inputDimensions.x;
            int height = (int)inputDimensions.y;

            int x = (int)(tid % width);
            int y = (int)(tid / width);

            int ImpulseIND = 0;
            int StartJ = (y - impulseWidth / 2);
            int EndJ = (y + impulseWidth / 2);

            if (StartJ < 0)
            {
                StartJ = 0;
                ImpulseIND = impulseWidth / 2 - y;
            }
            if (EndJ > height) EndJ = (height - 1);

            int memIND = x + StartJ * width;

            float sum = 0;
            for (int j = StartJ; j < EndJ; j++)
            {
                sum = sum + imageIn[memIND] * pImpulse[ImpulseIND];
                ImpulseIND++;
                memIND += width;
            }

            uint outid = (uint)(x * height + y);
            imageOut[outid] = sum;
        }

        public static void ConvolveAndProjectSim(ImageLibrary allImages, float[] impulse, ref  float[, ,] volume, int ZStart, int ZLength, out float[,] imageDataOut)
        {
           

            float[, ,] ImageData = allImages[0].ImageData;

            /*float[,]*/ imageDataOut = new float[ZLength,ImageData.GetLength(1)];
            unsafe
            {
                fixed (float* pImage = ImageData, pImageOut = imageDataOut, pCube = volume,pImpulse=impulse)
                {
                    uint[] imageDims = new uint[] { (uint)ImageData.GetLength(1), (uint)ZLength };
                    uint[] cubeDims = new uint[] { (uint)volume.GetLength(2), (uint)volume.GetLength(1), (uint)ZLength, 0 };
                    fixed (uint* pImageDims = imageDims, pVolDims = cubeDims )
                    {
                      
                        double AngleRadians = 0;
                        float[] fastScanDirection;
                        float[] LUT;
                        CreateLUT(AngleRadians, out fastScanDirection, out LUT);
                      
                       // for (int i = 0; i < 500; i += 25)
                        int i = 0;
                        {

                            AngleRadians = i / 500f * 360f / 180f * Math.PI;

                            CreateLUT(AngleRadians, out fastScanDirection, out LUT);
                            float[, ,] Data2 = allImages[i].ImageData;

                            fixed (float* pFS_Dir = fastScanDirection, pNextImage = Data2)
                            {

                                for (uint j = 0; j < imageDataOut.LongLength; j++)
                                    rotateConvolution(pImageOut, pImpulse, impulse.Length, new uint2(imageDims), pImage + imageDataOut.GetLength(1) * ZStart, j);

                               
                                
                                Console.WriteLine(i);
                                // eventList = null;
                            }
                        }
                    }
                    
                }
            }
        }


        public static void ConvolveAndProject(ImageLibrary allImages, float[] impulse, ref  float[, ,] volume, int ZStart, int ZLength)
        {
            ComputeImpulse = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, impulse);

            long WorkGroupSize = kernelConvolve.GetWorkGroupSize(devices[0]);

            float[, ,] ImageData = allImages[0].ImageData;

            float[,] imageDataOut = new float[ZLength, ImageData.GetLength(1)];
            unsafe
            {
                fixed (float* pImage = ImageData, pImageOut = imageDataOut, pCube = volume)
                {
                    uint[] imageDims = new uint[] { (uint)ImageData.GetLength(1), (uint)ZLength };
                    uint[] cubeDims = new uint[] { (uint)volume.GetLength(2), (uint)volume.GetLength(1), (uint)ZLength, 0 };
                    fixed (uint* pImageDims = imageDims, pVolDims = cubeDims)
                    {
                        /*__global float  * imageOut,
                            __constant float  * pImpulse,
                            const uint  impulseWidth,
                            const uint2  inputDimensions,*/
                        ComputeBuffer<float> ImageOutB = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, imageDataOut.LongLength, (IntPtr)(pImageOut));
                        kernelConvolve.SetMemoryArgument(0, ImageOutB);
                        kernelConvolve.SetMemoryArgument(1, ComputeImpulse);
                        kernelConvolve.SetValueArgument<uint>(2, (uint)impulse.Length);
                        kernelConvolve.SetArgument(3, (IntPtr)(sizeof(uint) * 2), (IntPtr)pImageDims);

                        /*__global float  * cube,
                         const    uint4  cubeDimensions,
                         const    uint2  imageDimensions,
                           __global float  * imageIn,
                        */
                        ComputeDataVolume = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, volume.LongLength, (IntPtr)pCube);
                        kernelFBP.SetMemoryArgument(0, ComputeDataVolume);
                        kernelFBP.SetArgument(1, (IntPtr)(sizeof(uint) * 4), (IntPtr)pVolDims);
                        kernelFBP.SetArgument(2, (IntPtr)(sizeof(uint) * 2), (IntPtr)pImageDims);
                        kernelFBP.SetMemoryArgument(3, ImageOutB);


                        //input the image into the convolution kernal
                        ComputeBuffer<float> inputImageBuffer = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, imageDataOut.LongLength, (IntPtr)(pImage + imageDataOut.GetLength(1) * ZStart));
                        kernelConvolve.SetMemoryArgument(4, inputImageBuffer);

                        double AngleRadians = 0;
                        float[] fastScanDirection;
                        float[] LUT;
                        CreateLUT(AngleRadians, out fastScanDirection, out LUT);
                        ComputeBuffer<float> LUTBuffer = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, LUT);
                        kernelFBP.SetMemoryArgument(5, LUTBuffer);

                        for (int i = 0; i < 500; i += 25)
                        {

                            AngleRadians = i / 500f * 360f / 180f * Math.PI;

                            CreateLUT(AngleRadians, out fastScanDirection, out LUT);
                            float[, ,] Data2 = allImages[i].ImageData;

                            fixed (float* pFS_Dir = fastScanDirection, pNextImage = Data2)
                            {
                                //  eventList = new ComputeEventList();

                                kernelFBP.SetArgument(4, (IntPtr)(sizeof(uint) * 4), (IntPtr)pFS_Dir);

                                commands.WriteToBuffer(Data2, inputImageBuffer, false, new SysIntX3(ZStart, 0, 0), new SysIntX3(0, 0, 0),
                                    new SysIntX3(ZLength, Data2.GetLength(1), Data2.GetLength(2)), eventList);

                                //commands.WriteToBuffer(Data2, inputImageBuffer, false, new SysIntX3(0, 0, 0), new SysIntX3(0, 0, 0),
                                //    new SysIntX3(Data2.GetLength(0), Data2.GetLength(1), Data2.GetLength(2)), eventList);

                                commands.WriteToBuffer(LUT, LUTBuffer, false, eventList);
                                //}


                                //now that all the memory is finally set up, run the kernals
                                commands.Execute(kernelConvolve, null, new long[] { imageDataOut.LongLength }, null, eventList);

                                //now that all the memory is finally set up, run the kernals
                                commands.Execute(kernelFBP, null, new long[] { volume.LongLength }, null, eventList);

                                // 1) Wait for the events in the list to finish,

                                eventList.Wait();

                                Console.WriteLine(i);
                                // eventList = null;
                            }
                        }
                    }
                    commands.ReadFromBuffer(ComputeDataVolume, (IntPtr)pCube, true, 0, volume.LongLength, null);
                }
            }
        }

        public static void ConvolveAndProject(ImageLibrary allImages, float[] impulse, ref  float[, ,] volume)
        {
            ComputeImpulse = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, impulse);

            long WorkGroupSize = kernelConvolve.GetWorkGroupSize(devices[0]);

            float[, ,] ImageData = allImages[0].ImageData;

            float[,] imageDataOut = new float[ImageData.GetLength(0), ImageData.GetLength(1)];
            unsafe
            {
                fixed (float* pImage = ImageData, pImageOut = imageDataOut, pCube = volume)
                {
                    uint[] imageDims = new uint[] { (uint)ImageData.GetLength(0), (uint)ImageData.GetLength(1) };
                    uint[] cubeDims = new uint[] { (uint)volume.GetLength(0), (uint)volume.GetLength(1), (uint)volume.GetLength(2), 0 };
                    fixed (uint* pImageDims = imageDims, pVolDims = cubeDims)
                    {
                        /*__global float  * imageOut,
                            __constant float  * pImpulse,
                            const uint  impulseWidth,
                            const uint2  inputDimensions,*/
                        ComputeBuffer<float> ImageOutB = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, ImageData.LongLength, (IntPtr)pImageOut);
                        kernelConvolve.SetMemoryArgument(0, ImageOutB);
                        kernelConvolve.SetMemoryArgument(1, ComputeImpulse);
                        kernelConvolve.SetValueArgument<uint>(2, (uint)impulse.Length);
                        kernelConvolve.SetArgument(3, (IntPtr)(sizeof(uint) * 2), (IntPtr)pImageDims);

                        /*__global float  * cube,
                         const    uint4  cubeDimensions,
                         const    uint2  imageDimensions,
                           __global float  * imageIn,
                        */
                        ComputeDataVolume = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, volume.LongLength, (IntPtr)pCube);
                        kernelFBP.SetMemoryArgument(0, ComputeDataVolume);
                        kernelFBP.SetArgument(1, (IntPtr)(sizeof(uint) * 4), (IntPtr)pVolDims);
                        kernelFBP.SetArgument(2, (IntPtr)(sizeof(uint) * 2), (IntPtr)pImageDims);
                        kernelFBP.SetMemoryArgument(3, ImageOutB);


                        //input the image into the convolution kernal
                        ComputeBuffer<float> inputImageBuffer = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, ImageData.LongLength, (IntPtr)pImage);
                        kernelConvolve.SetMemoryArgument(4, inputImageBuffer);

                        double AngleRadians = 0;
                        float[] fastScanDirection;
                        float[] LUT;
                        CreateLUT(AngleRadians, out fastScanDirection, out LUT);
                        ComputeBuffer<float> LUTBuffer = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, LUT);
                        kernelFBP.SetMemoryArgument(5, LUTBuffer);

                        for (int i = 0; i < 500; i += 25)
                        {

                            AngleRadians = i / 500f * 360f / 180f * Math.PI;

                            CreateLUT(AngleRadians, out fastScanDirection, out LUT);

                            fixed (float* pFS_Dir = fastScanDirection)
                            {
                                //  eventList = new ComputeEventList();

                                kernelFBP.SetArgument(4, (IntPtr)(sizeof(uint) * 4), (IntPtr)pFS_Dir);

                                float[, ,] Data2 = allImages[i].ImageData;
                                commands.WriteToBuffer(Data2, inputImageBuffer, false, new SysIntX3(0, 0, 0), new SysIntX3(0, 0, 0),
                                    new SysIntX3(Data2.GetLength(0), Data2.GetLength(1), Data2.GetLength(2)), eventList);

                                commands.WriteToBuffer(LUT, LUTBuffer, false, eventList);
                                //}


                                //now that all the memory is finally set up, run the kernals
                                commands.Execute(kernelConvolve, null, new long[] { ImageData.LongLength }, null, eventList);

                                //now that all the memory is finally set up, run the kernals
                                commands.Execute(kernelFBP, null, new long[] { volume.LongLength }, null, eventList);

                                // 1) Wait for the events in the list to finish,

                                eventList.Wait();

                                Console.WriteLine(i);
                                // eventList = null;
                            }
                        }
                    }
                    commands.ReadFromBuffer(ComputeDataVolume, (IntPtr)pCube, true, 0, volume.LongLength, null);
                }
            }
        }

        private static void CreateLUT(double angleRadians, out float[] fastScanDirection, out float[] LUT)
        {
            Point3D vRotationAxis;
            Point3D axis;
            vRotationAxis = new Point3D(0, 0, 1);
            axis = new Point3D(0, 1, 0);
            Point3D Direction = Point3D.RotateAroundAxis(axis, vRotationAxis, angleRadians);
            Point3D FastScanDirection = Point3D.CrossProduct(Direction, vRotationAxis);


            //make sure all the vectors are the right size
            Direction.Normalize();
            FastScanDirection.Normalize();
            //determine all the important planes in the 3D space and all the step
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            ///get the existing recon data.  the jagged array is used so the data can be checked out by each thread

            float r = (float)Math.Sqrt(2 * .5 * .5);
            LUT = new float[50];
            #region Look up table
            float[] p_X = new float[4];
            float[] p_Y = new float[4];


            p_X[0] = (float)Math.Cos(Math.PI / 4d + angleRadians) * r;
            p_Y[0] = (float)Math.Sin(Math.PI / 4d + angleRadians) * r;

            p_X[1] = (float)Math.Cos(Math.PI * 3d / 4d + angleRadians) * r;
            p_Y[1] = (float)Math.Sin(Math.PI * 3d / 4d + angleRadians) * r;

            p_X[2] = (float)Math.Cos(Math.PI * 5d / 4d + angleRadians) * r;
            p_Y[2] = (float)Math.Sin(Math.PI * 5d / 4d + angleRadians) * r;

            p_X[3] = (float)Math.Cos(Math.PI * 7d / 4d + angleRadians) * r;
            p_Y[3] = (float)Math.Sin(Math.PI * 7d / 4d + angleRadians) * r;

            float[] o_X = new float[4];
            float[] o_Y = new float[4];
            float d;
            const float step = 1.2f / 50f;
            int cc = 0;
            for (float u2 = 0; u2 < 1.2; u2 += step)
            {
                float l1_X = -1.2f;
                float l1_Y = u2;

                float l2_X = 1.2f;
                float l2_Y = u2;

                for (int j = 0; j < 4; j++)
                {
                    int j2 = (j + 1) % 4;
                    d = (l1_X - l2_X) * (p_Y[j] - p_Y[j2]) - (l1_Y - l2_Y) * (p_X[j] - p_X[j2]);
                    if (d == 0)
                    {
                        o_X[j] = 1000;
                        o_Y[j] = 10000;
                    }
                    else
                    {
                        o_X[j] = ((l1_X * l2_Y - l1_Y * l2_X) * (p_X[j] - p_X[j2]) - (l1_X - l2_X) * (p_X[j] * p_Y[j2] - p_Y[j] * p_X[j2])) / d;
                        o_Y[j] = ((l1_X * l2_Y - l1_Y * l2_X) * (p_Y[j] - p_Y[j2]) - (l1_Y - l2_Y) * (p_X[j] * p_Y[j2] - p_Y[j] * p_X[j2])) / d;
                    }
                }

                double mX1 = 0, mY1 = 0, mX2 = 0, mY2;
                double minP = 1000;
                double minN = 1000;
                for (int j = 0; j < 4; j++)
                {
                    d = Math.Abs(o_X[j]);
                    if (o_X[j] < 0 && d < minN)
                    {
                        mX2 = o_X[j];
                        mY2 = o_Y[j];
                        minN = d;
                    }
                    if (o_X[j] > 0 && d < minP)
                    {
                        mX1 = o_X[j];
                        mY1 = o_Y[j];
                        minP = d;
                    }
                }

                LUT[cc] = (float)Math.Abs(mX1 - mX2);
                cc++;
            }
            #endregion

            fastScanDirection = new float[] { (float)FastScanDirection.X, (float)FastScanDirection.Y, (float)FastScanDirection.Z, 0f };
        }

        public static void ReadResult(ref float[, ,] Data)
        {
            // A blocking "ReadFromBuffer" (if 3rd argument is true) will wait for itself and any previous commands
            // in the command queue or eventList to finish execution. Otherwise an explicit wait for all the opencl commands 
            // to finish has to be issued before "arrC" can be used. 
            // This explicit synchronization can be achieved in two ways:

            // Read back the results. If the command-queue has out-of-order execution enabled (default is off), ReadFromBuffer 
            // will not execute until any previous events in eventList (in our case only eventList[0]) are marked as complete 
            // by OpenCL. By default the command-queue will execute the commands in the same order as they are issued from the host.
            // eventList will contain two events after this method returns.

            //  commands.ReadFromBuffer(ComputeDataVolume, ref Data, true, new SysIntX3(0, 0, 0), new SysIntX3(0, 0, 0), new SysIntX3(Data.GetLength(2), Data.GetLength(1), Data.GetLength(0)), eventList);
            //commands.ReadFromBuffer(b, ref Data, true, eventList);
            // double d = Data.SumArray();
        }



    }

}
