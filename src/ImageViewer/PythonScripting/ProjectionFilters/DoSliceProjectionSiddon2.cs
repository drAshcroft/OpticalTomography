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
using MathHelpLib.ProjectionFilters;
using System.Runtime.InteropServices;
using System.Threading;

using Cloo;


namespace ImageViewer.PythonScripting.Projection
{
    public class DoSliceBackProjectionSiddonEffect2 : aEffectNoForm
    {
        public override string EffectName { get { return "Project Slice Through Object (Siddon)"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Projection"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }


        public static void RunRecon()
        {
            string[] Platforms = ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.AvailablePlatforms();
            ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.SetupGPU(Platforms[0],0);
            double[, ,] Data = new double[100, 100, 100];
            Data[0, 0, 0] = 5;
            ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.Set3DBuffer(Data);
            ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.DoProjections(null, Data);
            ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.ReadResult(ref Data);

        }



        #region GPU Code
        static string FBPcode = @"

#pragma OPENCL EXTENSION cl_amd_fp64 : enable 

__kernel void simpleConvolution(  __global double  * output,
                                  __global double  * input,
                                  __global double  * mask,
                                  const    uint  inputDimensionsX,
                                  const    uint  inputDimensionsY,
                                  const    uint  maskDimensions)
{
    uint tid   = get_global_id(0);
    
    uint width  = inputDimensionsX;
    //uint height = inputDimensions.y;
    
    uint x      = tid%width;
    uint y      = tid/width;
    
    uint maskWidth  = maskDimensions;
    //uint maskHeight = maskDimensions.y;
    
    uint vstep = (maskWidth  -1)/2;
    //uint hstep = (maskHeight -1)/2;
    
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


__kernel void simpleFBP( __global  double * volume,
                         __global  double * input,
                         const     uint  inputDimensionsX,
						const     uint  inputDimensionsY,
                         const     uint  cubeDimensionsX,
						const     uint  cubeDimensionsY,
						const     uint  cubeDimensionsZ,
						 const     float UPX,
						const     float UPY,
						const     float UPZ,
                         const     float AcrossX,
						const     float AcrossY,
						const     float AcrossZ,
                         const     double IntensityFactor
     
                         )
 {
			
            uint2 inputDimensions =(uint2)(inputDimensionsX, inputDimensionsY);
		    uint4 cubeDimensions =(uint4)(cubeDimensionsX,cubeDimensionsY,cubeDimensionsZ,0);
            float4 UP =(float4)(UPX,UPY,UPZ,0);
            float4 Across =(float4)(AcrossX,AcrossY,AcrossZ,0);

            uint iWidth = inputDimensions.x;
            uint iHeight = inputDimensions.y;

            int hiWidth = (int)(iWidth / 2);
            int hiHeight = (int)(iHeight / 2);

            float4 cDim =(float4)(  cubeDimensions.x,  cubeDimensions.y,  cubeDimensions.z, 0);
            float4 hcDim= cDim/2;

            //int hcWidth = (int)(cWidth / 2);
            //int hcHeight = (int)(cHeight / 2);
            //int hcDepth = (int)(cDepth / 2);

            uint SliceSize = cDim.y*cDim.z;// cHeight * cDepth;

            float4 pixelposition = (float4)( get_global_id(0), get_global_id(1),  get_global_id(2), 0);

            uint cTmp = dot((float4)(1, cDim.x, SliceSize, 0), pixelposition);// xI + yI * cWidth + zI * SliceSize;
            uint iTmp00,iTmp01,iTmp10,iTmp11;

            float4 xII= pixelposition - hcDim;
            //int xII =(int)xI - hcWidth;
            //int yII =(int) yI - hcHeight;
            //int zII = (int)zI - hcDepth;

            double  x, y,rX,rY,uX,uY,V1,V2;
            
           

           
            x = dot(UP,xII) + hiWidth; //UP.x * xII + UP.y *yII +UP.z *zII  + hiWidth ;
            y = dot(Across, xII) +hiHeight; // Across.x * xII + Across.y * yII + Across.z * zII+ hiHeight ;

            if (x>0 && x<iWidth && y>0 && y<iHeight)
            {
                rX = floor(x);
                rY = floor(y);
                if (x>1 && x<iWidth-1 && y>1 && y<iHeight-1)
                {
                    uY=x-rX;
                    uX=y-rY;
					
					iTmp00 = (uint)(rX + rY * iWidth);
                    iTmp10 = iTmp00+1;
                    iTmp01 =(uint)( rX + (rY-1) * iWidth);
                    iTmp11 = iTmp10+1;
					
                    V1 = input[iTmp00]*uY+input[iTmp01]*(1-uY);
                    V2 = input[iTmp10]*uY+input[iTmp11]*(1-uY);
              
                    volume[cTmp] +=  (V1*uX+V2*(1-uX))*IntensityFactor;

                }
            }
}


";

        #endregion

        #region GPU Code2
        static string FBPcode2 = @"
#pragma OPENCL EXTENSION cl_amd_fp64 : enable 

__kernel void doubleFBP( __global  double * volume,
                         __global  double * input,
                         const     uint  inputDimensionsX,
						 const     uint  inputDimensionsY,
                         const     uint  cubeDimensionsX,
						 const     uint  cubeDimensionsY,
						 const     uint  cubeDimensionsZ,
						 const     float UPX,
						 const     float UPY,
						 const     float UPZ,
                         const     float AcrossX,
						 const     float AcrossY,
						 const     float AcrossZ,
                         const     double IntensityFactor,
                         const     uint ZOffset
                         )
 {
			
            uint2 inputDimensions =(uint2)(inputDimensionsX, inputDimensionsY);
		    uint4 cubeDimensions =(uint4)(cubeDimensionsX,cubeDimensionsY,cubeDimensionsZ,0);

            float4 UP =(float4)(UPX,UPY,UPZ,0);
            float4 Across =(float4)(AcrossX,AcrossY,AcrossZ,0);

            uint iWidth = inputDimensions.x;
            uint iHeight = inputDimensions.y;

            int hiWidth = (int)(iWidth / 2);
            int hiHeight = (int)(iHeight / 2);

            float4 cDim =(float4)(  cubeDimensions.x,  cubeDimensions.y,  cubeDimensions.z, 0);
            float4 hcDim= cDim/2;

            //int hcWidth = (int)(cWidth / 2);
            //int hcHeight = (int)(cHeight / 2);
            //int hcDepth = (int)(cDepth / 2);

            uint SliceSize = cDim.y*cDim.x;// cHeight * cDepth;

            float4 pixelposition = (float4)( get_global_id(0), get_global_id(1),  get_global_id(2), 0);

            uint cTmp = dot((float4)(1, cDim.x, SliceSize, 0), pixelposition);// xI + yI * cWidth + zI * SliceSize;
            uint iTmp00,iTmp01,iTmp10,iTmp11;

            pixelposition.z += ZOffset;

            float4 xII= pixelposition - hcDim;

            double  x, y,rX,rY,uX,uY,V1,V2;
           
            x = dot(UP,xII) + hiWidth; 
            y = dot(Across, xII) +hiHeight; 

            if (x>0 && x<iWidth && y>0 && y<iHeight)
            {
                rX = floor(x);
                rY = floor(y);
                if (x>1 && x<iWidth-1 && y>1 && y<iHeight-1)
                {
                    uY=x-rX;
                    uX=y-rY;
					
					iTmp00 = (uint)(rX + rY * iWidth);
                    iTmp10 = iTmp00+1;
                    iTmp01 =(uint)( rX + (rY-1) * iWidth);
                    iTmp11 = iTmp10+1;
					
                    V1 = input[iTmp00]*uY+input[iTmp01]*(1-uY);
                    V2 = input[iTmp10]*uY+input[iTmp11]*(1-uY);
              
                    volume[cTmp] += pixelposition.z;//  (V1*uX+V2*(1-uX))*IntensityFactor;

                }
            }
}
";
        #endregion
        static IList<ComputeDevice> devices;
        static ComputePlatform platform;
        static ComputeContext context;
        static ComputeContext context2;
        static ComputeKernel kernel;
        static ComputeKernel kernel2;
        static ComputeKernel kernelConvolve;
        static ComputeBuffer<double> ComputeDataVolume;
        static ComputeBuffer<double> ComputeDataVolume2;
        static ComputeBuffer<double> ComputeImpulse;
        static ComputeCommandQueue commands;
        static ComputeCommandQueue commands2;
        static ComputeEventList eventList;
        static ComputeEventList eventList2;
        static ComputeProgram program;
        static ComputeProgram program2;

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


        public static bool SetupDoubleGPU(string Platform)
        {
            bool ret = BuildProgram(Platform, FBPcode2, 0, out context, out program);
            ret = ret && BuildProgram(Platform, FBPcode2, 0, out context2, out  program2);

            if (ret == true)
            {
                // Create the kernel function and set its arguments.
                kernel = program.CreateKernel("doubleFBP");
                kernel2 = program2.CreateKernel("doubleFBP");

                // Create the event wait list. An event list is not really needed for this example but it is important to see how it works.
                // Note that events (like everything else) consume OpenCL resources and creating a lot of them may slow down execution.
                // For this reason their use should be avoided if possible.
                eventList = new ComputeEventList();
                eventList2 = new ComputeEventList();

                // Create the command queue. This is used to control kernel execution and manage read/write/copy operations.
                commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);
                commands2 = new ComputeCommandQueue(context2, context2.Devices[0], ComputeCommandQueueFlags.None);

                return true;
            }
            return ret;
        }


        public static bool SetupGPU(string Platform, int device )
        {
            bool ret = BuildProgram(Platform, FBPcode, device, out context, out program);

            if (ret == true)
            {
                // Create the kernel function and set its arguments.
                kernel = program.CreateKernel("simpleFBP");

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

        public static void SetImpulse(double[] impulse)
        {
            ComputeImpulse = new ComputeBuffer<double>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, impulse);
            kernelConvolve.SetMemoryArgument(2, ComputeImpulse);
            kernelConvolve.SetValueArgument<uint>(5, (uint)impulse.Length);
        }


        private static double[, ,] mData = null;

        private static double[, ,] mFData;
        private static double[, ,] mSData;
        private static IntPtr mFirstHalf;
        private static IntPtr mSecondHalf;
        private static GCHandle mhData;
        private static int mZCut;
        public static void SetDouble3DBuffer(double[, ,] Data)
        {
            mZCut = (Data.GetLength(0) / 2);
            //int  FirstHalf = Data.GetLength(2) * Data.GetLength(1) * mZCut ;
            //int SecondLength = Data.Length - FirstHalf;

            //mhData = GCHandle.Alloc(Data, GCHandleType.Pinned);
            //mFirstHalf = mhData.AddrOfPinnedObject();
            //mSecondHalf = mFirstHalf + FirstHalf ;

           // mFData = new double[Data.GetLength(0), Data.GetLength(1), mZCut ];
            mSData = new double[Data.GetLength(0), Data.GetLength(1), Data.GetLength(2)-mZCut];

            /*ComputeDataVolume = new ComputeBuffer<double>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, mFData);
            kernel.SetMemoryArgument(0, ComputeDataVolume);
            kernel.SetValueArgument<uint>(4, (uint)Data.GetLength(0));
            kernel.SetValueArgument<uint>(5, (uint)Data.GetLength(1));
            kernel.SetValueArgument<uint>(6, (uint)Data.GetLength(2));*/

            ComputeDataVolume2 = new ComputeBuffer<double>(context2, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, mSData);
            kernel2.SetMemoryArgument(0, ComputeDataVolume2);
            kernel2.SetValueArgument<uint>(4, (uint)Data.GetLength(0));
            kernel2.SetValueArgument<uint>(5, (uint)Data.GetLength(1));
            kernel2.SetValueArgument<uint>(6, (uint)(Data.GetLength(2)));

            mData = Data;
        }

        public static void Set3DBuffer(double[, ,] Data)
        {
            ComputeDataVolume = new ComputeBuffer<double>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, Data);
            kernel.SetMemoryArgument(0, ComputeDataVolume);

            kernel.SetValueArgument<uint>(4, (uint)Data.GetLength(0));
            kernel.SetValueArgument<uint>(5, (uint)Data.GetLength(1));
            kernel.SetValueArgument<uint>(6, (uint)Data.GetLength(2));

            mData = Data;
        }

        static object ProjectionLock = new object();

        public static void DoOneDoubleProjection(double[,] Image, double Angle)
        {
            Axis RotationAxis = Axis.ZAxis;

            Point3D vRotationAxis = new Point3D();
            Point3D axis = new Point3D();

            if (RotationAxis == Axis.XAxis)
            {
                vRotationAxis = new Point3D(1, 0, 0);
                axis = new Point3D(0, 1, 0);
            }
            else if (RotationAxis == Axis.YAxis)
            {
                vRotationAxis = new Point3D(0, 1, 0);
                axis = new Point3D(0, 0, 1);
            }
            else if (RotationAxis == Axis.ZAxis)
            {
                vRotationAxis = new Point3D(0, 0, 1);
                axis = new Point3D(0, 1, 0);
            }

            Point3D Direction = Point3D.RotateAroundAxis(axis, vRotationAxis, Angle);
            Point3D FastScanDirection = Point3D.CrossProduct(Direction, vRotationAxis);
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);
            Direction.Normalize();
            SlowScanAxis.Normalize();


            lock (ProjectionLock)
            {
                // Create the input buffers and fill them with data from the arrays.
                // Access modifiers should match those in a kernel.
                // CopyHostPointer means the buffer should be filled with the data provided in the last argument.
              /*  ComputeBuffer<double> a = new ComputeBuffer<double>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, Image);
                kernel.SetMemoryArgument(1, a);
                kernel.SetValueArgument<uint>(2, (uint)Image.GetLength(0));
                kernel.SetValueArgument<uint>(3, (uint)Image.GetLength(1));

                kernel.SetValueArgument<float>(7, (float)vRotationAxis.X);
                kernel.SetValueArgument<float>(8, (float)vRotationAxis.Y);
                kernel.SetValueArgument<float>(9, (float)vRotationAxis.Z);

                kernel.SetValueArgument<float>(10, (float)FastScanDirection.X);
                kernel.SetValueArgument<float>(11, (float)FastScanDirection.Y);
                kernel.SetValueArgument<float>(12, (float)FastScanDirection.Z);

                kernel.SetValueArgument<double>(13, (double)1);
                kernel.SetValueArgument<uint>(14, 0);
                //long WorkGroupSize = kernel.GetWorkGroupSize(devices[0]);

                // Execute the kernel "count" times. After this call returns, "eventList" will contain an event associated with this command.
                // If eventList == null or typeof(eventList) == ReadOnlyCollection<ComputeEventBase>, a new event will not be created.
                commands.Execute(kernel, null, new long[] { mData.GetLength(0), mData.GetLength(1), mZCut}, null, eventList);

                eventList.Wait();
               
                a.Dispose();*/
                // Create the input buffers and fill them with data from the arrays.
                // Access modifiers should match those in a kernel.
                // CopyHostPointer means the buffer should be filled with the data provided in the last argument.
                ComputeBuffer<double> a2 = new ComputeBuffer<double>(context2, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, Image);
                kernel2.SetMemoryArgument(1, a2);
                kernel2.SetValueArgument<uint>(2, (uint)Image.GetLength(0));
                kernel2.SetValueArgument<uint>(3, (uint)Image.GetLength(1));

                kernel2.SetValueArgument<float>(7, (float)vRotationAxis.X);
                kernel2.SetValueArgument<float>(8, (float)vRotationAxis.Y);
                kernel2.SetValueArgument<float>(9, (float)vRotationAxis.Z);

                kernel2.SetValueArgument<float>(10, (float)FastScanDirection.X);
                kernel2.SetValueArgument<float>(11, (float)FastScanDirection.Y);
                kernel2.SetValueArgument<float>(12, (float)FastScanDirection.Z);

                kernel2.SetValueArgument<double>(13, (double)1);
                kernel2.SetValueArgument<uint>(14, (uint)mZCut);
                //long WorkGroupSize = kernel.GetWorkGroupSize(devices[0]);

                // Execute the kernel "count" times. After this call returns, "eventList" will contain an event associated with this command.
                // If eventList == null or typeof(eventList) == ReadOnlyCollection<ComputeEventBase>, a new event will not be created.
                commands2.Execute(kernel2, null, new long[] { mData.GetLength(2), mData.GetLength(1), mData.GetLength(2) - mZCut -1}, null, eventList2);

                // 1) Wait for the events in the list to finish,
                
                eventList2.Wait();
               
                a2.Dispose();
            }
            GC.Collect();
        }


        public static void DoOneProjection(double[,] Image, double Angle)
        {
            Axis RotationAxis = Axis.ZAxis;

            Point3D vRotationAxis = new Point3D();
            Point3D axis = new Point3D();

            if (RotationAxis == Axis.XAxis)
            {
                vRotationAxis = new Point3D(1, 0, 0);
                axis = new Point3D(0, 1, 0);
            }
            else if (RotationAxis == Axis.YAxis)
            {
                vRotationAxis = new Point3D(0, 1, 0);
                axis = new Point3D(0, 0, 1);
            }
            else if (RotationAxis == Axis.ZAxis)
            {
                vRotationAxis = new Point3D(0, 0, 1);
                axis = new Point3D(0, 1, 0);
            }

            lock (ProjectionLock)
            {
                // Create the input buffers and fill them with data from the arrays.
                // Access modifiers should match those in a kernel.
                // CopyHostPointer means the buffer should be filled with the data provided in the last argument.
                ComputeBuffer<double> a = new ComputeBuffer<double>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, Image);

                kernel.SetMemoryArgument(1, a);

                kernel.SetValueArgument<uint>(2, (uint)Image.GetLength(0));
                kernel.SetValueArgument<uint>(3, (uint)Image.GetLength(1));

                Point3D Direction = Point3D.RotateAroundAxis(axis, vRotationAxis, Angle);

                Point3D FastScanDirection = Point3D.CrossProduct(Direction, vRotationAxis);

                Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

                Direction.Normalize();
                SlowScanAxis.Normalize();

                kernel.SetValueArgument<float>(7, (float)vRotationAxis.X);
                kernel.SetValueArgument<float>(8, (float)vRotationAxis.Y);
                kernel.SetValueArgument<float>(9, (float)vRotationAxis.Z);

                kernel.SetValueArgument<float>(10, (float)FastScanDirection.X);
                kernel.SetValueArgument<float>(11, (float)FastScanDirection.Y);
                kernel.SetValueArgument<float>(12, (float)FastScanDirection.Z);

                kernel.SetValueArgument<double>(13, (double)1);

                long WorkGroupSize = kernel.GetWorkGroupSize(devices[0]);


                // Execute the kernel "count" times. After this call returns, "eventList" will contain an event associated with this command.
                // If eventList == null or typeof(eventList) == ReadOnlyCollection<ComputeEventBase>, a new event will not be created.
                commands.Execute(kernel, null, new long[] { mData.GetLength(2), mData.GetLength(1), mData.GetLength(0) }, null, eventList);

                // 1) Wait for the events in the list to finish,
                eventList.Wait();
                a.Dispose();
            }
            GC.Collect();
        }

        public static void DoProjections(DataEnvironment dataEnvironment, double[, ,] Data)
        {
            Axis RotationAxis = Axis.ZAxis;

            Point3D vRotationAxis = new Point3D();
            Point3D axis = new Point3D();

            if (RotationAxis == Axis.XAxis)
            {
                vRotationAxis = new Point3D(1, 0, 0);
                axis = new Point3D(0, 1, 0);
            }
            else if (RotationAxis == Axis.YAxis)
            {
                vRotationAxis = new Point3D(0, 1, 0);
                axis = new Point3D(0, 0, 1);
            }
            else if (RotationAxis == Axis.ZAxis)
            {
                vRotationAxis = new Point3D(0, 0, 1);
                axis = new Point3D(0, 1, 0);
            }
            /*
            kernelConvolve.SetValueArgument<uint>(3,(uint) dataEnvironment.AllImages[1].Width);
            kernelConvolve.SetValueArgument<uint>(4, (uint)dataEnvironment.AllImages[1].Height);

            float[][,] OutData = new float[dataEnvironment.AllImages.Count][,];

            float[,] BufferIn = new float[dataEnvironment.AllImages[1].ImageData.GetLength(0), dataEnvironment.AllImages[1].ImageData.GetLength(1)];
            float[,] BufferOut = new float[dataEnvironment.AllImages[1].ImageData.GetLength(0), dataEnvironment.AllImages[1].ImageData.GetLength(1)];
            ComputeBuffer<float> tempImage = new ComputeBuffer<float>(context, ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.CopyHostPointer, BufferOut);
            kernelConvolve.SetMemoryArgument(0, tempImage);

            for (int i = 0; i <1;i++)// dataEnvironment.AllImages.Count; i++)
            {
                float[, ,] Image = dataEnvironment.AllImages[i].ImageData;
                Buffer.BlockCopy(Image,0,BufferIn,0,Buffer.ByteLength(BufferIn));
                ComputeBuffer<float> a = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, BufferIn);
                kernelConvolve.SetMemoryArgument(1, a);
                // Execute the kernel "count" times. After this call returns, "eventList" will contain an event associated with this command.
                // If eventList == null or typeof(eventList) == ReadOnlyCollection<ComputeEventBase>, a new event will not be created.
                commands.Execute(kernelConvolve, null, new long[] { Image.Length }, null, eventList);
                // 1) Wait for the events in the list to finish,
                eventList.Wait();
                OutData[i] = new float[BufferOut.GetLength(0), BufferOut.GetLength(1)];
                commands.ReadFromBuffer(tempImage, ref OutData[i], true, new SysIntX2(0,  0), new SysIntX2(0,0), new SysIntX2(BufferOut.GetLength(0), BufferOut.GetLength(1)), eventList);
                a = null;
                GC.Collect();
            }

            commands.Finish();

            Bitmap b = dataEnvironment.AllImages[1].ToBitmap();


            b = MathHelpLib.ImageProcessing.MathImageHelps.MakeBitmap(OutData[0]);

            System.Diagnostics.Debug.Print(b.ToString());)*/

            for (int i = 0; i < dataEnvironment.AllImages.Count; i++)
            {
                float[, ,] Image = dataEnvironment.AllImages[i].ImageData;

                // Create the input buffers and fill them with data from the arrays.
                // Access modifiers should match those in a kernel.
                // CopyHostPointer means the buffer should be filled with the data provided in the last argument.
                ComputeBuffer<float> a = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, Image);

                kernel.SetMemoryArgument(1, a);

                kernel.SetValueArgument<uint>(2, (uint)Image.GetLength(0));
                kernel.SetValueArgument<uint>(3, (uint)Image.GetLength(1));


                double angle = (double)i / (double)dataEnvironment.AllImages.Count * Math.PI * 2;

                Point3D Direction = Point3D.RotateAroundAxis(axis, vRotationAxis, angle);

                Point3D FastScanDirection = Point3D.CrossProduct(Direction, vRotationAxis);

                Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

                Direction.Normalize();
                SlowScanAxis.Normalize();

                kernel.SetValueArgument<float>(7, (float)vRotationAxis.X);
                kernel.SetValueArgument<float>(8, (float)vRotationAxis.Y);
                kernel.SetValueArgument<float>(9, (float)vRotationAxis.Z);

                kernel.SetValueArgument<float>(10, (float)FastScanDirection.X);
                kernel.SetValueArgument<float>(11, (float)FastScanDirection.Y);
                kernel.SetValueArgument<float>(12, (float)FastScanDirection.Z);

                kernel.SetValueArgument<double>(13, (double)1);

                long WorkGroupSize = kernel.GetWorkGroupSize(devices[0]);


                // Execute the kernel "count" times. After this call returns, "eventList" will contain an event associated with this command.
                // If eventList == null or typeof(eventList) == ReadOnlyCollection<ComputeEventBase>, a new event will not be created.
                commands.Execute(kernel, null, new long[] { Data.GetLength(2), Data.GetLength(1), Data.GetLength(0) }, null, eventList);

                // 1) Wait for the events in the list to finish,
                eventList.Wait();
                a.Dispose();
                GC.Collect();
            }
            // 2) Or simply use

            //commands.Finish();
        }

        public static void ReadResult(ref double[, ,] Data)
        {
            // A blocking "ReadFromBuffer" (if 3rd argument is true) will wait for itself and any previous commands
            // in the command queue or eventList to finish execution. Otherwise an explicit wait for all the opencl commands 
            // to finish has to be issued before "arrC" can be used. 
            // This explicit synchronization can be achieved in two ways:

            // Read back the results. If the command-queue has out-of-order execution enabled (default is off), ReadFromBuffer 
            // will not execute until any previous events in eventList (in our case only eventList[0]) are marked as complete 
            // by OpenCL. By default the command-queue will execute the commands in the same order as they are issued from the host.
            // eventList will contain two events after this method returns.

            commands.ReadFromBuffer(ComputeDataVolume, ref Data, true, new SysIntX3(0, 0, 0), new SysIntX3(0, 0, 0), new SysIntX3(Data.GetLength(2), Data.GetLength(1), Data.GetLength(0)), eventList);
            //commands.ReadFromBuffer(b, ref Data, true, eventList);
            // double d = Data.SumArray();
        }

        public static void ReadResultDouble(double[, ,] Data)
        {
            // A blocking "ReadFromBuffer" (if 3rd argument is true) will wait for itself and any previous commands
            // in the command queue or eventList to finish execution. Otherwise an explicit wait for all the opencl commands 
            // to finish has to be issued before "arrC" can be used. 
            // This explicit synchronization can be achieved in two ways:

            // Read back the results. If the command-queue has out-of-order execution enabled (default is off), ReadFromBuffer 
            // will not execute until any previous events in eventList (in our case only eventList[0]) are marked as complete 
            // by OpenCL. By default the command-queue will execute the commands in the same order as they are issued from the host.
            // eventList will contain two events after this method returns.
           // unsafe
            {
             //   fixed (double* first = mFData)
                {
               //     commands.ReadFromBuffer(ComputeDataVolume, (IntPtr)first, true, 0, mFData.LongLength, eventList);
                   // commands.ReadFromBuffer(ComputeDataVolume, ref Data, true, new SysIntX3(0, 0, 0), new SysIntX3(0, 0, 0), new SysIntX3(Data.GetLength(0), Data.GetLength(1), mZCut), eventList);
                }
            }
           // Thread.Sleep(1000);
            commands2.ReadFromBuffer(ComputeDataVolume2, ref Data, true, new SysIntX3(0, 0, 0), new SysIntX3(0, 0, mZCut ), new SysIntX3(mSData.GetLength(0), mSData.GetLength(1), mSData.GetLength(2)), eventList2);
            /*
                Thread t2 = new Thread(delegate()
                    {

                       
                    });

                t1.Start();
                Thread.Sleep(1000);
                t1.Join();
                t2.Start();
                Thread.Sleep(1000);
                t2.Join();*/

            // Thread t1 = new Thread(delegate()
             /*    {
                     for (int i = 0; i < Data.GetLength(0); i++)
                         for (int j = 0; j < Data.GetLength(1); j++)
                             for (int k = 0; k < mZCut; k++)
                                 Data[i, j, k] = mFData[i, j, k];
               /*  }
             );

             Thread t2 = new Thread(delegate()
             {
                 int Zcut2 = Data.GetLength(2) - mZCut;
                 for (int i = 0; i < Data.GetLength(0); i++)
                     for (int j = 0; j < Data.GetLength(1); j++)
                         for (int k = 0; k < Zcut2; k++)
                             Data[i, j, k +mZCut ] = mSData[i, j, k];
             }
             );

             t1.Start();
             t2.Start();

             t1.Join(); 
             t2.Join();*/
            //commands.ReadFromBuffer(ComputeDataVolume, ref Data, true, new SysIntX3(0, 0, 0), new SysIntX3(0, 0, 0), new SysIntX3(Data.GetLength(0), Data.GetLength(1), mZCut), eventList);
            //commands2.ReadFromBuffer(ComputeDataVolume2, ref Data, true, new SysIntX3(0, 0, 0), new SysIntX3(0, 0, mZCut), new SysIntX3(Data.GetLength(0), Data.GetLength(1), Data.GetLength(2)- mZCut), eventList2);

            // mhData.Free();
            //commands.ReadFromBuffer(b, ref Data, true, eventList);
            // double d = Data.SumArray();
        }

        /// <summary>
        /// This is the core routine for filtered back projection.  This back projects the desired array into the recon volume
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">pseudo projection in either physical array or double[,]; recon grid in either physical array or projectionarrayobject; projection angle (rotation around z)</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mPassData = PassData;

            mFilterToken = Parameters;

            if (mFilterToken[0].GetType() == typeof(PhysicalArray))
            {
                PhysicalArray Slice = (PhysicalArray)mFilterToken[0];
                PhysicalArray DensityGrid = (PhysicalArray)mFilterToken[1];
                double Angle = EffectHelps.ConvertToDouble(mFilterToken[2]);
                DoBackProjection_OneSlice(Slice, DensityGrid, Angle, Axis2D.YAxis);
            }
            else if (mFilterToken[1].GetType() == typeof(ProjectionArrayObject))
            {
                double[,] Slice = (double[,])mFilterToken[0];
                ProjectionArrayObject DensityGrid = (ProjectionArrayObject)mFilterToken[1];
                double Angle = EffectHelps.ConvertToDouble(mFilterToken[2]);

                Bitmap b = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToBitmap(Slice);
                int w = b.Width;
                //gpu run will return the result.  if anything goes wrong, then do the normal cpu work
                DoBackProjection_OneSlice(Slice, 1, 1, DensityGrid, Angle, Axis2D.YAxis);
            }
            else if (mFilterToken[0].GetType() == typeof(float[, ,]))
            {
                /*  if (DSBPToken.GPUError == false && File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\ProjectFBPImage.dll") == true)
                  {
                      float[,] DataIn = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToFloatArray((double[,])mFilterToken[0], false);
                      float[, ,] DensityGrid = (float[, ,])mFilterToken[1];
                      double Angle = EffectHelps.ConvertToDouble(mFilterToken[2]);
                      try
                      {
                          //run the GPU convolution
                          throw new Exception("Not implimented");
                          //   BackProjectGPU(DataIn, DensityGrid, Angle);
                      }
                      catch { DSBPToken.GPUError = true; }
                  }*/
            }
            return SourceImage;
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Convolution|PhysicalArray", "ProjectionObject|PhysicalArray", "SliceAngle|double(Radians)" }; }
        }

        /// <summary>
        /// calculates the needed vectors to turn the requested angle into projection vectors
        /// </summary>
        /// <param name="Slice"></param>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians"></param>
        /// <param name="ConvolutionAxis">Usually set to YAxis. This is the direction that carries the rotation, or the fast axis for the convolution</param>
        public static void DoBackProjection_OneSlice(PhysicalArray Slice, PhysicalArray DensityGrid, double AngleRadians, Axis2D ConvolutionAxis)
        {
            Axis RotationAxis = Axis.ZAxis;
            if (DensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
            {
                Point3D vec;
                Point3D axis;

                axis = new Point3D(0, 0, 1);


                double rx = Math.Cos(AngleRadians);
                double ry = Math.Sin(AngleRadians);

                vec = new Point3D(ry, rx, 0);

                DensityGrid.SmearArray(Slice, vec, Point3D.CrossProduct(vec, axis));

            }
            else
            {
                Point3D vRotationAxis = new Point3D();
                Point3D axis = new Point3D();

                if (RotationAxis == Axis.XAxis)
                {
                    vRotationAxis = new Point3D(1, 0, 0);
                    axis = new Point3D(0, 1, 0);
                }
                else if (RotationAxis == Axis.YAxis)
                {
                    vRotationAxis = new Point3D(0, 1, 0);
                    axis = new Point3D(0, 0, 1);
                }
                else if (RotationAxis == Axis.ZAxis)
                {
                    vRotationAxis = new Point3D(0, 0, 1);
                    axis = new Point3D(0, 1, 0);
                }

                double angle = AngleRadians;

                Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, angle);

                // DensityGrid.SmearArrayInterpolate1D (Slice, vec, Point3D.CrossProduct(vec, vRotationAxis),PhysicalArray.InterpolationMethod.Cubic );
                DensityGrid.SmearArray(Slice, vec, Point3D.CrossProduct(vec, vRotationAxis));
            }

        }

        /// <summary>
        /// calculates the needed vectors to turn the requested angle into projection vectors
        /// </summary>
        /// <param name="Slice"></param>
        /// <param name="PaintingWidth">Physical dimension of the pseudo projection (vs the recon size)</param>
        /// <param name="PaintingHeight">Physical dimension of the pseudo projection (vs the recon size)</param>
        /// <param name="DensityGrid"></param>
        /// <param name="AngleRadians"></param>
        /// <param name="ConvolutionAxis">Usually set to YAxis. This is the direction that carries the rotation, or the fast axis for the convolution</param>
        public static void DoBackProjection_OneSlice(double[,] Slice, double PaintingWidth, double PaintingHeight, ProjectionArrayObject DensityGrid, double AngleRadians, Axis2D ConvolutionAxis)
        {
            //AngleRadians = 0;
            Axis RotationAxis = Axis.ZAxis;

            Point3D vRotationAxis = new Point3D();
            Point3D axis = new Point3D();

            if (RotationAxis == Axis.XAxis)
            {
                vRotationAxis = new Point3D(1, 0, 0);
                axis = new Point3D(0, 1, 0);
            }
            else if (RotationAxis == Axis.YAxis)
            {
                vRotationAxis = new Point3D(0, 1, 0);
                axis = new Point3D(0, 0, 1);
            }
            else if (RotationAxis == Axis.ZAxis)
            {
                vRotationAxis = new Point3D(0, 0, 1);
                axis = new Point3D(0, 1, 0);
            }

            double angle = AngleRadians;

            Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, angle);

            if (DensityGrid.Data != null)
                SmearArray2D(AngleRadians, DensityGrid, Slice, PaintingWidth, PaintingHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));
            else
            {
               // if (DensityGrid.ZStart == 0 && DensityGrid.ZEnd == DensityGrid.ZWhole)
                    SmearArray2DWhole(AngleRadians, DensityGrid, Slice, PaintingWidth, PaintingHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));
              //  else
              //      SmearArray2DWhole(AngleRadians, DensityGrid,DensityGrid.ZStart,DensityGrid.ZEnd,DensityGrid.ZWhole, Slice, PaintingWidth, PaintingHeight, vec, Point3D.CrossProduct(vec, vRotationAxis));
            }
        }

      
        static Random rnd = new Random();
        static object FindLock = new object();
        private static void SmearArray2D(double AngleRadians, ProjectionArrayObject DensityGrid, double[,] PaintingArray, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
        {

            //make sure all the vectors are the right size
            Direction.Normalize();
            FastScanDirection.Normalize();
            //determine all the important planes in the 3D space and all the step
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            ///get the existing recon data.  the jagged array is used so the data can be checked out by each thread
            double[][,] mDataDouble = DensityGrid.Data;
            double r = Math.Sqrt(2 * .5 * .5);
            double[] LUT = new double[50];
            #region Look up table
            double[] p_X = new double[4];
            double[] p_Y = new double[4];


            p_X[0] = Math.Cos(Math.PI / 4d + AngleRadians) * r;
            p_Y[0] = Math.Sin(Math.PI / 4d + AngleRadians) * r;

            p_X[1] = Math.Cos(Math.PI * 3d / 4d + AngleRadians) * r;
            p_Y[1] = Math.Sin(Math.PI * 3d / 4d + AngleRadians) * r;

            p_X[2] = Math.Cos(Math.PI * 5d / 4d + AngleRadians) * r;
            p_Y[2] = Math.Sin(Math.PI * 5d / 4d + AngleRadians) * r;

            p_X[3] = Math.Cos(Math.PI * 7d / 4d + AngleRadians) * r;
            p_Y[3] = Math.Sin(Math.PI * 7d / 4d + AngleRadians) * r;

            double[] o_X = new double[4];
            double[] o_Y = new double[4];
            double d;
            const double step = 1.2 / 50d;
            int cc = 0;
            for (double u2 = 0; u2 < 1.2; u2 += step)
            {
                double l1_X = -1.2;
                double l1_Y = u2;

                double l2_X = 1.2;
                double l2_Y = u2;

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
                /* double min2 = 1000;
                 for (int j = 0; j < 4; j++)
                 {
                     d=Math.Abs(o_X[j]) ;
                     if (d < min2 && d>min)
                     {
                         mX2 = o_X[j];
                         mY2 = o_Y[j];
                         min2 = d;
                     }
                 }*/
                LUT[cc] = Math.Abs(mX1 - mX2);
                cc++;
            }
            #endregion
            #region constants
            //get all the dimensions
            int LI = mDataDouble[0].GetLength(1);
            int LJ = mDataDouble[0].GetLength(0);
            int LK = mDataDouble.GetLength(0);

            int LsI = PaintingArray.GetLength(1);
            int LsJ = PaintingArray.GetLength(0);
            int LsI_1 = LsI - 1;
            int LsJ_1 = LsJ - 1;

            double halfI, halfJ, halfK, halfIs, halfJs;
            double VolToSliceX, VolToSliceY, VolToSliceZ;

            halfI = (double)LI / 2;
            halfJ = (double)LJ / 2;
            halfK = (double)LK / 2;

            halfIs = (double)LsI / 2;
            halfJs = (double)LsJ / 2;

            VolToSliceX = (DensityGrid.XMax - DensityGrid.XMin) / LI * (double)PaintingArray.GetLength(0) / (2 * PaintingWidth);
            VolToSliceY = (DensityGrid.YMax - DensityGrid.YMin) / LJ * (double)PaintingArray.GetLength(0) / (2 * PaintingWidth);
            VolToSliceZ = 1;

            int K_ = 0;
            int FinishedCount = 0;
            #endregion

            double sX, sY, sK, sdotI, u;
            int lower_sI, lower_sJ;
            bool SliceFound = false;
            int LUT_Index;
            unsafe
            {
                //we make the math unchecked to make it go faster.  there seem to be enough checks beforehand
                unchecked
                {
                    //for (int zI = 0; zI < LZ; zI++)
                    // while (FinishedCount < LK)
                    {
                        #region Find open slice
                        //there is a lot of thread contention, so look through the whole stack and find an open slice, and work on it
                        SliceFound = false;
                        int StartI = (new Random(DateTime.Now.Millisecond)).Next(LK);
                        for (int i = 0; i < LK; i++)
                        {
                            //lock (FindLock)
                            {
                                //  if (DensityGrid.LockIndicator[i] == false)
                                {
                                    K_ = (i + StartI) % LK;
                                    DensityGrid.LockIndicator[K_] = true;
                                    FinishedCount++;
                                    SliceFound = true;
                                    //  break;
                                }
                            }

                        #endregion

                            if (SliceFound == true)
                            {
                                //indicate that the thread is locked
                                lock (DensityGrid.LockArray[K_])
                                {
                                    #region Process slice
                                    fixed (double* mipData = mDataDouble[K_])
                                    {
                                        double* POut;
                                        for (int J_ = 0; J_ < LJ; J_++)
                                        {
                                            //tranform to slice index coords
                                            sY = (J_ - halfI) * VolToSliceY;
                                            for (int I_ = 0; I_ < LI; I_++)
                                            {

                                                //tranform to slice index coords
                                                sX = (I_ - halfI) * VolToSliceX;

                                                sdotI = sX * FastScanDirection.X + sY * FastScanDirection.Y + halfIs;
                                                //make sure that we are still in the recon volumn
                                                if (sdotI > 0 && sdotI < LsI_1)
                                                    if (K_ > 0 && K_ < LsJ_1)
                                                    {
                                                        POut = (double*)mipData + I_ * LI + J_;
                                                        lower_sI = (int)Math.Floor(sdotI);
                                                        u = sdotI - lower_sI;
                                                        lower_sJ = K_;//(int)Math.Floor(K_);

                                                        LUT_Index = (int)(u / 1.2);
                                                        if (LUT_Index > 49)
                                                            LUT_Index = 49;
                                                        //*POut = PaintingArray[lower_sI, lower_sJ];
                                                        DensityGrid.Data[K_][I_, J_] += PaintingArray[lower_sJ, lower_sI] * LUT[LUT_Index]
                                                                                + PaintingArray[lower_sJ, lower_sI + 1] * LUT[49 - LUT_Index];
                                                    }
                                            }
                                        }
                                    }

                                    #endregion

                                }
                            }
                            //release the programatic handle to 
                            DensityGrid.LockIndicator[K_] = false;
                        }
                    }
                }
                //SmearArray2DQueue = null;
            }
        }

        private static void SmearArray2DWhole(double AngleRadians, ProjectionArrayObject DensityGrid, double[,] PaintingArray, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
        {

            //make sure all the vectors are the right size
            Direction.Normalize();
            FastScanDirection.Normalize();
            //determine all the important planes in the 3D space and all the step
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            ///get the existing recon data.  the jagged array is used so the data can be checked out by each thread
            double[, ,] mDataDouble = DensityGrid.DataWhole;
            double r = Math.Sqrt(2 * .5 * .5);
            double[] LUT = new double[50];
            #region Look up table
            double[] p_X = new double[4];
            double[] p_Y = new double[4];


            p_X[0] = Math.Cos(Math.PI / 4d + AngleRadians) * r;
            p_Y[0] = Math.Sin(Math.PI / 4d + AngleRadians) * r;

            p_X[1] = Math.Cos(Math.PI * 3d / 4d + AngleRadians) * r;
            p_Y[1] = Math.Sin(Math.PI * 3d / 4d + AngleRadians) * r;

            p_X[2] = Math.Cos(Math.PI * 5d / 4d + AngleRadians) * r;
            p_Y[2] = Math.Sin(Math.PI * 5d / 4d + AngleRadians) * r;

            p_X[3] = Math.Cos(Math.PI * 7d / 4d + AngleRadians) * r;
            p_Y[3] = Math.Sin(Math.PI * 7d / 4d + AngleRadians) * r;

            double[] o_X = new double[4];
            double[] o_Y = new double[4];
            double d;
            const double step = 1.2 / 50d;
            int cc = 0;
            for (double u2 = 0; u2 < 1.2; u2 += step)
            {
                double l1_X = -1.2;
                double l1_Y = u2;

                double l2_X = 1.2;
                double l2_Y = u2;

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
                /* double min2 = 1000;
                 for (int j = 0; j < 4; j++)
                 {
                     d=Math.Abs(o_X[j]) ;
                     if (d < min2 && d>min)
                     {
                         mX2 = o_X[j];
                         mY2 = o_Y[j];
                         min2 = d;
                     }
                 }*/
                LUT[cc] =  Math.Abs(mX1 - mX2);
                cc++;
            }
            #endregion
            #region constants
            //get all the dimensions
            int LI = mDataDouble.GetLength(2);
            int LJ = mDataDouble.GetLength(1);
            int LK = mDataDouble.GetLength(0);

            int LsI = PaintingArray.GetLength(1);
            int LsJ = PaintingArray.GetLength(0);
            int LsI_1 = LsI - 1;
            int LsJ_1 = LsJ - 1;

            double halfI, halfJ, halfK, halfIs, halfJs;
            double VolToSliceX, VolToSliceY, VolToSliceZ;

            halfI = (double)LI / 2;
            halfJ = (double)LJ / 2;
            halfK = (double)LK / 2;

            halfIs = (double)LsI / 2;
            halfJs = (double)LsJ / 2;

            VolToSliceX = (DensityGrid.XMax - DensityGrid.XMin) / LI * (double)PaintingArray.GetLength(0) / (2 * PaintingWidth);
            VolToSliceY = (DensityGrid.YMax - DensityGrid.YMin) / LJ * (double)PaintingArray.GetLength(0) / (2 * PaintingWidth);
            VolToSliceZ = (DensityGrid.ZMax - DensityGrid.ZMin) / LK * (double)PaintingArray.GetLength(1) / (2 * PaintingHeight);

            int K_ = 0;
            int FinishedCount = 0;
            #endregion

            double sX, sY, sK, sdotI, u;
            int lower_sI, lower_sJ;
            bool SliceFound = false;
            int LUT_Index;
            unsafe
            {
                //we make the math unchecked to make it go faster.  there seem to be enough checks beforehand
                unchecked
                {
                    fixed (double* mipDataWhole = mDataDouble)
                    {
                        //for (int zI = 0; zI < LZ; zI++)
                        // while (FinishedCount < LK)
                        {
                            #region Find open slice
                            //there is a lot of thread contention, so look through the whole stack and find an open slice, and work on it
                            SliceFound = false;
                            int StartI = (new Random(DateTime.Now.Millisecond)).Next(LK);
                            for (int i = 0; i < LK; i++)
                            {
                                //lock (FindLock)
                                {
                                    //  if (DensityGrid.LockIndicator[i] == false)
                                    {
                                        K_ = (i + StartI) % LK;
                                        DensityGrid.LockIndicator[K_] = true;
                                        FinishedCount++;
                                        SliceFound = true;
                                        //  break;
                                    }
                                }

                            #endregion

                                if (SliceFound == true)
                                {
                                    //indicate that the thread is locked
                                    lock (DensityGrid.LockArray[K_])
                                    {
                                        #region Process slice
                                        double* mipData = mipDataWhole + K_ * LI * LJ; //mDataDouble[K_]
                                        {
                                            double* POut;
                                            for (int J_ = 0; J_ < LJ; J_++)
                                            {
                                                //tranform to slice index coords
                                                sY = (J_ - halfI) * VolToSliceY;
                                                for (int I_ = 0; I_ < LI; I_++)
                                                {

                                                    //tranform to slice index coords
                                                    sX = (I_ - halfI) * VolToSliceX;

                                                    sdotI = sX * FastScanDirection.X + sY * FastScanDirection.Y + halfIs;
                                                    //make sure that we are still in the recon volumn
                                                    if (sdotI > 0 && sdotI < LsI_1)
                                                        if (K_ > 0 && K_ < LsJ_1)
                                                        {
                                                            POut = (double*)mipData + I_ * LI + J_;
                                                            lower_sI = (int)Math.Floor(sdotI);
                                                            u = sdotI - lower_sI;
                                                            lower_sJ =(int)( (K_-halfK) * VolToSliceZ + halfJs);//(int)Math.Floor(K_);

                                                            LUT_Index = (int)(u / 1.2);
                                                            if (LUT_Index > 49)
                                                                LUT_Index = 49;
                                                            //*POut = PaintingArray[lower_sI, lower_sJ];
                                                           // mDataDouble[K_, J_, I_] += PaintingArray[lower_sJ, lower_sI] * LUT[LUT_Index]*(1-u)
                                                           //                         + PaintingArray[lower_sJ, lower_sI + 1] * LUT[49 - LUT_Index]*u;

                                                            mDataDouble[K_, J_, I_] += PaintingArray[lower_sJ, lower_sI] * (1 - u)
                                                                                   + PaintingArray[lower_sJ, lower_sI + 1] *  u;
                                                        }
                                                }
                                            }
                                        }

                                        #endregion

                                    }
                                }
                                //release the programatic handle to 
                                DensityGrid.LockIndicator[K_] = false;
                            }
                        }
                    }
                }
                //SmearArray2DQueue = null;
            }



        }


        private static void SmearArray2DWhole(double AngleRadians, ProjectionArrayObject DensityGrid, int ZStart,int ZEnd, int ZWhole, double[,] PaintingArray, double PaintingWidth, double PaintingHeight, Point3D Direction, Point3D FastScanDirection)
        {

            //make sure all the vectors are the right size
            Direction.Normalize();
            FastScanDirection.Normalize();
            //determine all the important planes in the 3D space and all the step
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            ///get the existing recon data.  the jagged array is used so the data can be checked out by each thread
            double[, ,] mDataDouble = DensityGrid.DataWhole;
            double r = Math.Sqrt(2 * .5 * .5);
            double[] LUT = new double[50];
            #region Look up table
            double[] p_X = new double[4];
            double[] p_Y = new double[4];


            p_X[0] = Math.Cos(Math.PI / 4d + AngleRadians) * r;
            p_Y[0] = Math.Sin(Math.PI / 4d + AngleRadians) * r;

            p_X[1] = Math.Cos(Math.PI * 3d / 4d + AngleRadians) * r;
            p_Y[1] = Math.Sin(Math.PI * 3d / 4d + AngleRadians) * r;

            p_X[2] = Math.Cos(Math.PI * 5d / 4d + AngleRadians) * r;
            p_Y[2] = Math.Sin(Math.PI * 5d / 4d + AngleRadians) * r;

            p_X[3] = Math.Cos(Math.PI * 7d / 4d + AngleRadians) * r;
            p_Y[3] = Math.Sin(Math.PI * 7d / 4d + AngleRadians) * r;

            double[] o_X = new double[4];
            double[] o_Y = new double[4];
            double d;
            const double step = 1.2 / 50d;
            int cc = 0;
            for (double u2 = 0; u2 < 1.2; u2 += step)
            {
                double l1_X = -1.2;
                double l1_Y = u2;

                double l2_X = 1.2;
                double l2_Y = u2;

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
                /* double min2 = 1000;
                 for (int j = 0; j < 4; j++)
                 {
                     d=Math.Abs(o_X[j]) ;
                     if (d < min2 && d>min)
                     {
                         mX2 = o_X[j];
                         mY2 = o_Y[j];
                         min2 = d;
                     }
                 }*/
                LUT[cc] = Math.Abs(mX1 - mX2);
                cc++;
            }
            #endregion

            #region constants
            //get all the dimensions
            int LI = mDataDouble.GetLength(2);
            int LJ = mDataDouble.GetLength(1);
            int LK = ZWhole;// mDataDouble.GetLength(0);

            int LsI = PaintingArray.GetLength(1);
            int LsJ = PaintingArray.GetLength(0);
            int LsI_1 = LsI - 1;
            int LsJ_1 = LsJ - 1;

            double halfI, halfJ, halfK, halfIs, halfJs;
            double VolToSliceX, VolToSliceY, VolToSliceZ;

            halfI = (double)LI / 2;
            halfJ = (double)LJ / 2;
            halfK = (double)LK / 2;

            halfIs = (double)LsI / 2;
            halfJs = (double)LsJ / 2;

            VolToSliceX = (DensityGrid.XMax - DensityGrid.XMin) / LI * (double)PaintingArray.GetLength(0) / (2 * PaintingWidth);
            VolToSliceY = (DensityGrid.YMax - DensityGrid.YMin) / LJ * (double)PaintingArray.GetLength(0) / (2 * PaintingWidth);
            VolToSliceZ = (DensityGrid.ZMax - DensityGrid.ZMin) / LK * (double)PaintingArray.GetLength(1) / (2 * PaintingHeight);

            int K_ = 0;
            int FinishedCount = 0;
            #endregion

            double sX, sY, sK, sdotI, u;
            int lower_sI, lower_sJ;
            bool SliceFound = false;
            int LUT_Index;
            unsafe
            {
                //we make the math unchecked to make it go faster.  there seem to be enough checks beforehand
                unchecked
                {
                    fixed (double* mipDataWhole = mDataDouble)
                    {
                        //for (int zI = 0; zI < LZ; zI++)
                        // while (FinishedCount < LK)
                        {
                            #region Find open slice
                            //there is a lot of thread contention, so look through the whole stack and find an open slice, and work on it
                            SliceFound = false;
                            int StartI = (new Random(DateTime.Now.Millisecond)).Next(LK);
                            for (int i = ZStart; i < ZEnd; i++)
                            {
                                //lock (FindLock)
                                {
                                    //  if (DensityGrid.LockIndicator[i] == false)
                                    {
                                        K_ = (i + StartI) % (ZEnd-ZStart)+ZStart;
                                        DensityGrid.LockIndicator[K_] = true;
                                        FinishedCount++;
                                        SliceFound = true;
                                        //  break;
                                    }
                                }

                            #endregion

                                if (SliceFound == true)
                                {
                                    //indicate that the thread is locked
                                    lock (DensityGrid.LockArray[K_])
                                    {
                                        #region Process slice
                                        double* mipData = mipDataWhole + K_ * LI * LJ; //mDataDouble[K_]
                                        {
                                            double* POut;
                                            for (int J_ = 0; J_ < LJ; J_++)
                                            {
                                                //tranform to slice index coords
                                                sY = (J_ - halfI) * VolToSliceY;
                                                for (int I_ = 0; I_ < LI; I_++)
                                                {

                                                    //tranform to slice index coords
                                                    sX = (I_ - halfI) * VolToSliceX;

                                                    sdotI = sX * FastScanDirection.X + sY * FastScanDirection.Y + halfIs;
                                                    //make sure that we are still in the recon volumn
                                                    if (sdotI > 0 && sdotI < LsI_1)
                                                        if (K_ > 0 && K_ < LsJ_1)
                                                        {
                                                            try
                                                            {
                                                               // POut = (double*)mipData + I_ * LI + J_;
                                                                lower_sI = (int)Math.Floor(sdotI);
                                                                u = sdotI - lower_sI;
                                                                lower_sJ = (int)((K_ - halfK) * VolToSliceZ + halfJs);//(int)Math.Floor(K_);

                                                                LUT_Index = (int)(u / 1.2);
                                                                if (LUT_Index > 49)
                                                                    LUT_Index = 49;

                                                                //*POut = PaintingArray[lower_sI, lower_sJ];
                                                                mDataDouble[K_-ZStart, J_, I_] += PaintingArray[lower_sJ, lower_sI] * LUT[LUT_Index]
                                                                                        + PaintingArray[lower_sJ, lower_sI + 1] * LUT[49 - LUT_Index];
                                                            }
                                                            catch(Exception ex0)
                                                            {
                                                                System.Diagnostics.Debug.Print(ex0.Message);
                                                            }
                                                        }
                                                }
                                            }
                                        }

                                        #endregion

                                    }
                                }
                                //release the programatic handle to 
                                DensityGrid.LockIndicator[K_] = false;
                            }
                        }
                    }
                }
                //SmearArray2DQueue = null;
            }



        }
   
    }
}
