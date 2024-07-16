/* ============================================================

Copyright (c) 2009 Advanced Micro Devices, Inc.  All rights reserved.
 
Redistribution and use of this material is permitted under the following 
conditions:
 
Redistributions must retain the above copyright notice and all terms of this 
license.
 
In no event shall anyone redistributing or accessing or using this material 
commence or participate in any arbitration or legal action relating to this 
material against Advanced Micro Devices, Inc. or any copyright holders or 
contributors. The foregoing shall survive any expiration or termination of 
this license or any agreement or access or use related to this material. 

ANY BREACH OF ANY TERM OF THIS LICENSE SHALL RESULT IN THE IMMEDIATE REVOCATION 
OF ALL RIGHTS TO REDISTRIBUTE, ACCESS OR USE THIS MATERIAL.

THIS MATERIAL IS PROVIDED BY ADVANCED MICRO DEVICES, INC. AND ANY COPYRIGHT 
HOLDERS AND CONTRIBUTORS "AS IS" IN ITS CURRENT CONDITION AND WITHOUT ANY 
REPRESENTATIONS, GUARANTEE, OR WARRANTY OF ANY KIND OR IN ANY WAY RELATED TO 
SUPPORT, INDEMNITY, ERROR FREE OR UNINTERRUPTED OPERA TION, OR THAT IT IS FREE 
FROM DEFECTS OR VIRUSES.  ALL OBLIGATIONS ARE HEREBY DISCLAIMED - WHETHER 
EXPRESS, IMPLIED, OR STATUTORY - INCLUDING, BUT NOT LIMITED TO, ANY IMPLIED 
WARRANTIES OF TITLE, MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, 
ACCURACY, COMPLETENESS, OPERABILITY, QUALITY OF SERVICE, OR NON-INFRINGEMENT. 
IN NO EVENT SHALL ADVANCED MICRO DEVICES, INC. OR ANY COPYRIGHT HOLDERS OR 
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, PUNITIVE,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT
OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, REVENUE, DATA, OR PROFITS; OR 
BUSINESS INTERRUPTION) HOWEVER CAUSED OR BASED ON ANY THEORY OF LIABILITY 
ARISING IN ANY WAY RELATED TO THIS MATERIAL, EVEN IF ADVISED OF THE POSSIBILITY 
OF SUCH DAMAGE. THE ENTIRE AND AGGREGATE LIABILITY OF ADVANCED MICRO DEVICES, 
INC. AND ANY COPYRIGHT HOLDERS AND CONTRIBUTORS SHALL NOT EXCEED TEN DOLLARS 
(US $10.00). ANYONE REDISTRIBUTING OR ACCESSING OR USING THIS MATERIAL ACCEPTS 
THIS ALLOCATION OF RISK AND AGREES TO RELEASE ADVANCED MICRO DEVICES, INC. AND 
ANY COPYRIGHT HOLDERS AND CONTRIBUTORS FROM ANY AND ALL LIABILITIES, 
OBLIGATIONS, CLAIMS, OR DEMANDS IN EXCESS OF TEN DOLLARS (US $10.00). THE 
FOREGOING ARE ESSENTIAL TERMS OF THIS LICENSE AND, IF ANY OF THESE TERMS ARE 
CONSTRUED AS UNENFORCEABLE, FAIL IN ESSENTIAL PURPOSE, OR BECOME VOID OR 
DETRIMENTAL TO ADVANCED MICRO DEVICES, INC. OR ANY COPYRIGHT HOLDERS OR 
CONTRIBUTORS FOR ANY REASON, THEN ALL RIGHTS TO REDISTRIBUTE, ACCESS OR USE 
THIS MATERIAL SHALL TERMINATE IMMEDIATELY. MOREOVER, THE FOREGOING SHALL 
SURVIVE ANY EXPIRATION OR TERMINATION OF THIS LICENSE OR ANY AGREEMENT OR 
ACCESS OR USE RELATED TO THIS MATERIAL.

NOTICE IS HEREBY PROVIDED, AND BY REDISTRIBUTING OR ACCESSING OR USING THIS 
MATERIAL SUCH NOTICE IS ACKNOWLEDGED, THAT THIS MATERIAL MAY BE SUBJECT TO 
RESTRICTIONS UNDER THE LAWS AND REGULATIONS OF THE UNITED STATES OR OTHER 
COUNTRIES, WHICH INCLUDE BUT ARE NOT LIMITED TO, U.S. EXPORT CONTROL LAWS SUCH 
AS THE EXPORT ADMINISTRATION REGULATIONS AND NATIONAL SECURITY CONTROLS AS 
DEFINED THEREUNDER, AS WELL AS STATE DEPARTMENT CONTROLS UNDER THE U.S. 
MUNITIONS LIST. THIS MATERIAL MAY NOT BE USED, RELEASED, TRANSFERRED, IMPORTED,
EXPORTED AND/OR RE-EXPORTED IN ANY MANNER PROHIBITED UNDER ANY APPLICABLE LAWS, 
INCLUDING U.S. EXPORT CONTROL LAWS REGARDING SPECIFICALLY DESIGNATED PERSONS, 
COUNTRIES AND NATIONALS OF COUNTRIES SUBJECT TO NATIONAL SECURITY CONTROLS. 
MOREOVER, THE FOREGOING SHALL SURVIVE ANY EXPIRATION OR TERMINATION OF ANY 
LICENSE OR AGREEMENT OR ACCESS OR USE RELATED TO THIS MATERIAL.

NOTICE REGARDING THE U.S. GOVERNMENT AND DOD AGENCIES: This material is 
provided with "RESTRICTED RIGHTS" and/or "LIMITED RIGHTS" as applicable to 
computer software and technical data, respectively. Use, duplication, 
distribution or disclosure by the U.S. Government and/or DOD agencies is 
subject to the full extent of restrictions in all applicable regulations, 
including those found at FAR52.227 and DFARS252.227 et seq. and any successor 
regulations thereof. Use of this material by the U.S. Government and/or DOD 
agencies is acknowledgment of the proprietary rights of any copyright holders 
and contributors, including those of Advanced Micro Devices, Inc., as well as 
the provisions of FAR52.227-14 through 23 regarding privately developed and/or 
commercial computer software.

This license forms the entire agreement regarding the subject matter hereof and 
supersedes all proposals and prior discussions and writings between the parties 
with respect thereto. This license does not affect any ownership, rights, title,
or interest in, or relating to, this material. No terms of this license can be 
modified or waived, and no breach of this license can be excused, unless done 
so in a writing signed by all affected parties. Each term of this license is 
separately enforceable. If any term of this license is determined to be or 
becomes unenforceable or illegal, such term shall be reformed to the minimum 
extent necessary in order for this license to remain in effect in accordance 
with its terms as modified by such reformation. This license shall be governed 
by and construed in accordance with the laws of the State of Texas without 
regard to rules on conflicts of law of any state or jurisdiction or the United 
Nations Convention on the International Sale of Goods. All disputes arising out 
of this license shall be subject to the jurisdiction of the federal and state 
courts in Austin, Texas, and all defenses are hereby waived concerning personal 
jurisdiction and venue of these courts.

============================================================ */


#include "FBP_Partial_Projection.hpp"


int FBP_Projection::setupFBP_Projection()
{
  
	return SDK_SUCCESS;
}


int FBP_Projection::genBinaryImage()
{
	cl_int status = CL_SUCCESS;

	
	return SDK_SUCCESS;
}


int FBP_Projection::setupCL(void)
{
	cl_int status = 0;
	size_t deviceListSize;

	cl_device_type dType;
	
	if(deviceType.compare("cpu") == 0)
	{
		dType = CL_DEVICE_TYPE_CPU;
	}
	else //deviceType = "gpu" 
	{
		dType = CL_DEVICE_TYPE_GPU;
		if(isThereGPU() == false)
		{
			std::cout << "GPU not found. Falling back to CPU device" << std::endl;
			dType = CL_DEVICE_TYPE_CPU;
		}
	}

	/*
	 * Have a look at the available platforms and pick either
	 * the AMD one if available or a reasonable default.
	 */

	cl_uint numPlatforms;
	cl_platform_id platform = NULL;
	status = clGetPlatformIDs(0, NULL, &numPlatforms);
	if(!sampleCommon->checkVal(status, CL_SUCCESS,  "clGetPlatformIDs failed."))
	{
		return SDK_FAILURE;
	}
	if (0 < numPlatforms) 
	{
		cl_platform_id* platforms = new cl_platform_id[numPlatforms];
		status = clGetPlatformIDs(numPlatforms, platforms, NULL);
		if(!sampleCommon->checkVal(status,
								   CL_SUCCESS,
								   "clGetPlatformIDs failed."))
		{
			return SDK_FAILURE;
		}

		if(isPlatformEnabled())
		{
			platform = platforms[platformId];
		}
		else
		{
			for (unsigned i = 0; i < numPlatforms; ++i) 
			{
				char pbuf[100];
				status = clGetPlatformInfo(platforms[i],
										   CL_PLATFORM_VENDOR,
										   sizeof(pbuf),
										   pbuf,
										   NULL);

				if(!sampleCommon->checkVal(status,
										   CL_SUCCESS,
										   "clGetPlatformInfo failed."))
				{
					return SDK_FAILURE;
				}

				platform = platforms[i];
				if (!strcmp(pbuf, "Advanced Micro Devices, Inc.")) 
				{
					break;
				}
			}
		}
		delete[] platforms;
	}

	if(NULL == platform)
	{
		std::cout<<("NULL platform found so Exiting Application.");
		return SDK_FAILURE;
	}

	// Display available devices.
	if(!sampleCommon->displayDevices(platform, dType))
	{
		std::cout<<("sampleCommon::displayDevices() failed");
		return SDK_FAILURE;
	}

	/*
	 * If we could find our platform, use it. Otherwise use just available platform.
	 */
	 cl_context_properties cps[3] = 
	{
		CL_CONTEXT_PLATFORM, 
		(cl_context_properties)platform, 
		0
	};

	context = clCreateContextFromType(
				  cps,
				  dType,
				  NULL,
				  NULL,
				  &status);

	if(!sampleCommon->checkVal(status, 
				CL_SUCCESS,
				"clCreateContextFromType failed."))
		return SDK_FAILURE;

	/* First, get the size of device list data */
	status = clGetContextInfo(
			context, 
			CL_CONTEXT_DEVICES, 
			0, 
			NULL, 
			&deviceListSize);
	if(!sampleCommon->checkVal(
				status, 
				CL_SUCCESS,
				"clGetContextInfo failed."))
		return SDK_FAILURE;

	int deviceCount = (int)(deviceListSize / sizeof(cl_device_id));
	if(!sampleCommon->validateDeviceId(deviceId, deviceCount))
	{
		std::cout<<("sampleCommon::validateDeviceId() failed");
		return SDK_FAILURE;
	}

	/* Now allocate memory for device list based on the size we got earlier */
	devices = (cl_device_id *)malloc(deviceListSize);
	if(devices == NULL)
	{
		std::cout<<("Failed to allocate memory (devices).");
		return SDK_FAILURE;
	}

	/* Now, get the device list data */
	status = clGetContextInfo(
			context, 
			CL_CONTEXT_DEVICES, 
			deviceListSize, 
			devices, 
			NULL);
	if(!sampleCommon->checkVal(
				status,
				CL_SUCCESS, 
				"clGetGetContextInfo failed."))
		return SDK_FAILURE;

	{
		/* The block is to move the declaration of prop closer to its use */
		cl_command_queue_properties prop = 0;
		commandQueue = clCreateCommandQueue(
				context, 
				devices[deviceId], 
				prop, 
				&status);
		if(!sampleCommon->checkVal(
					status,
					0,
					"clCreateCommandQueue failed."))
			return SDK_FAILURE;
	}

   
 

	 const char * source = ConvolutionCode.c_str();
		size_t sourceSize[] = { strlen(source) };

		program = clCreateProgramWithSource(context,
											1,
											&source,
											sourceSize,
											&status);

	if(!sampleCommon->checkVal(
			status,
			CL_SUCCESS,
			"clCreateProgramWithSource failed."))
		return SDK_FAILURE;

   

	if(flagsStr.size() != 0)
		std::cout << "Build Options are : " << flagsStr.c_str() << std::endl;

	

	/* create a cl program executable for all the devices specified */
	status = clBuildProgram(program, 
							1, 
							&devices[deviceId], 
							flagsStr.c_str(), 
							NULL, 
							NULL);

	if(!sampleCommon->checkVal(
				status,
				CL_SUCCESS,
				"clBuildProgram failed."))
		return SDK_FAILURE;

	/* get a kernel object handle for a kernel with the given name */
	kernel = clCreateKernel(program , KernalName.c_str() /*"FBP_Projection"*/, &status);
	if(!sampleCommon->checkVal(
				status,
				CL_SUCCESS,
				"clCreateKernel failed."))
		return SDK_FAILURE;

	globalThreads[0] = CubeWidth;
	globalThreads[1]= CubeHeight;
	globalThreads[2]= CubeZLength;
	localThreads[0]  = 256;

	/* Check group size against kernelWorkGroupSize */
	status = clGetKernelWorkGroupInfo(kernel,
									  devices[deviceId],
									  CL_KERNEL_WORK_GROUP_SIZE,
									  sizeof(size_t),
									  &kernelWorkGroupSize,
									  0);
	if(!sampleCommon->checkVal(
						status,
						CL_SUCCESS, 
						"clGetKernelWorkGroupInfo failed."))
	{
		return SDK_FAILURE;
	}

	 if((cl_uint)(localThreads[0]) > kernelWorkGroupSize)
	{
		if(!quiet)
		{
			std::cout << "Out of Resources!" << std::endl;
			std::cout << "Group Size specified : "<< localThreads[0] 
				<< std::endl;
			std::cout << "Max Group Size supported on the kernel : " 
				<< kernelWorkGroupSize <<std::endl;
			std::cout <<"Changing the group size to " << kernelWorkGroupSize 
				<< std::endl;
		}

		localThreads[0] = kernelWorkGroupSize;
	}


	  std::cout << CubeWidth << "  " << CubeHeight << "  " << CubeDepth << "\r";

	/////////////////////////////////////////////////////////////////////////////////////////
		d_volumeArray = clCreateBuffer(
			context, 
			CL_MEM_READ_WRITE | CL_MEM_USE_HOST_PTR,
			sizeof(cl_double) * CubeWidth*CubeHeight*CubeZLength,
			Cube+CubeWidth*CubeHeight*CubeZStart, 
			&status);

	if(!sampleCommon->checkVal(
				status,
				CL_SUCCESS,
				"clCreateBuffer failed. (outputBuffer)"))
		return SDK_FAILURE;

  
	status = clSetKernelArg(kernel, 0, sizeof(cl_mem), (void *) &d_volumeArray);
		
		if(!sampleCommon->checkVal(
				status,
				CL_SUCCESS,
				"clCreateSampler failed."))
	return SDK_FAILURE;
		/////////////////////////////////////////////////////////////////////////////////////////


   std::cout << width << "  " << height << "\r";

	cl_uint2 inputDimensions = {width, height};
	

	std::cout << CubeWidth << CubeHeight << CubeDepth << "\r";

	status = clSetKernelArg( kernel, 2, sizeof(cl_uint2), (void *)&inputDimensions);
   if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (inputDim)"))
		return SDK_FAILURE;

   /////////////////////////////////////////////////////////////////////////////////////////

   cl_uint4 cubeDimensions = {CubeWidth,CubeHeight,CubeDepth,0};

   status = clSetKernelArg(kernel,  3, sizeof(cl_uint4), (void *)&cubeDimensions);
   if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (cubeDIM)"))
		return SDK_FAILURE;

   cl_uint2 ZStartLength = {CubeZStart, CubeZLength};

   status = clSetKernelArg(kernel,  6, sizeof(cl_uint2), (void *)&ZStartLength);
   if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (cubeZ Length)"))
		return SDK_FAILURE;

   cl_uint kws=kernelWorkGroupSize;
   status = clSetKernelArg(kernel,  7, sizeof(cl_uint), (void *)&kws);
   if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (workgroupsize)"))
		return SDK_FAILURE;

   
	return SDK_SUCCESS;
}

int FBP_Projection::resetInput()
{
	cl_int status = 0;

	if (inputBuffer!=0)
		status = clReleaseMemObject(inputBuffer);

   if(!sampleCommon->checkVal(
	  status,
	  CL_SUCCESS,
	  "clReleaseMemObject failed."))
	  return SDK_FAILURE;

	inputBuffer = clCreateBuffer( context, CL_MEM_READ_ONLY | CL_MEM_USE_HOST_PTR, sizeof(cl_double ) * width * height, input, &status);
	if(!sampleCommon->checkVal( status, CL_SUCCESS, "clCreateBuffer failed. (inputBuffer)"))
		return SDK_FAILURE;

   status = clSetKernelArg( kernel, 1, sizeof(cl_mem), (void *)&inputBuffer);
   if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (outputBuffer)"))
		return SDK_FAILURE;


    status = clSetKernelArg( kernel, 4, sizeof(cl_float4), (void *)&UP);
   if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (ZInfo)"))
		return SDK_FAILURE;


	 status = clSetKernelArg( kernel, 5, sizeof(cl_float4), (void *)&Across);
   if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (ZInfo)"))
		return SDK_FAILURE;

  

	return SDK_SUCCESS;
}


int FBP_Projection::runCLKernels(void)
{
	cl_int   status;
	cl_event events[2];
	
	/* 
	 * Enqueue a kernel run call.
	 */
	status = clEnqueueNDRangeKernel(
			commandQueue,
			kernel,
			3,
			NULL,
			globalThreads,
			0,
			0,
			NULL,
			&events[0]);

	if(!sampleCommon->checkVal(
				status,
				CL_SUCCESS,
				"clEnqueueNDRangeKernel failed."))
		return SDK_FAILURE;


	/* wait for the kernel call to finish execution */
	status = clWaitForEvents(1, &events[0]);
	if(!sampleCommon->checkVal(
				status,
				CL_SUCCESS,
				"clWaitForEvents failed."))
		return SDK_FAILURE;

	return SDK_SUCCESS;
}

int FBP_Projection::getCLData(void)
{
	cl_int   status;
	cl_event events[2];
	size_t origin[3] ={0,0,0};

	if (Cube ==0)
		return 0;

	 /* Enqueue readBuffer*/
	status = clEnqueueReadBuffer(
				commandQueue,
				d_volumeArray,
				CL_TRUE,
				0,
			   (sizeof(cl_double) * CubeWidth*CubeHeight*CubeZLength),
				Cube+CubeWidth*CubeHeight*CubeZStart,
				0,
				NULL,
				&events[1]);

	if(!sampleCommon->checkVal(
			status,
			CL_SUCCESS,
			"clEnqueueReadBuffer failed."))
		return SDK_FAILURE;
	
	/* Wait for the read buffer to finish execution */
	status = clWaitForEvents(1, &events[1]);
	if(!sampleCommon->checkVal(
			status,
			CL_SUCCESS,
			"clWaitForEvents failed."))
		return SDK_FAILURE;
	
	clReleaseEvent(events[1]);
	return SDK_SUCCESS;
}

int FBP_Projection::initialize()
{
  
   return SDK_SUCCESS;
}

int FBP_Projection::setup()
{
	std::cout << "setupSimpleConvoltuion";

   if(setupFBP_Projection()!=SDK_SUCCESS)
	  return SDK_FAILURE;
	
   
	 std::cout << "setupCL";
	if(setupCL()!=SDK_SUCCESS)
	  return SDK_FAILURE;

	 std::cout << "CLDone";
 
   return SDK_SUCCESS;
}


int FBP_Projection::run()
{
	std::cout << "Executing kernel for " << iterations << 
		" iterations" << std::endl;
	std::cout << "-------------------------------------------" << std::endl;

	for(int i = 0; i < iterations; i++)
	{
		/* Arguments are set and execution call is enqueued on command buffer */
		if(runCLKernels()!=SDK_SUCCESS)
			return SDK_FAILURE;
	}

	return SDK_SUCCESS;
}

int FBP_Projection::verifyResults()
{
	if(verify)
	{
	  
	}

	return SDK_SUCCESS;
}

void FBP_Projection::printStats()
{
	std::string strArray[5] = {"Width", "Height", "mask Size", "Time(sec)", "KernelTime(sec)"};
	std::string stats[5];

	totalTime = setupTime + totalKernelTime;

	stats[0]  = sampleCommon->toString(width    , std::dec);
	stats[1]  = sampleCommon->toString(height   , std::dec);
	stats[2]  = sampleCommon->toString(CubeWidth, std::dec);
	stats[3]  = sampleCommon->toString(totalTime, std::dec);
	stats[4]  = sampleCommon->toString(totalKernelTime, std::dec);
	
	this->SDKSample::printStats(strArray, stats, 5);
}

int FBP_Projection::cleanup()
{
	/* Releases OpenCL resources (Context, Memory etc.) */
	cl_int status;

	status = clReleaseKernel(kernel);
	if(!sampleCommon->checkVal(
		status,
		CL_SUCCESS,
		"clReleaseKernel failed."))
		return SDK_FAILURE;

	status = clReleaseProgram(program);
	if(!sampleCommon->checkVal(
		status,
		CL_SUCCESS,
		"clReleaseProgram failed."))
		return SDK_FAILURE;

	if (inputBuffer !=0)
		status = clReleaseMemObject(inputBuffer);
   
	status = clReleaseCommandQueue(commandQueue);
	 if(!sampleCommon->checkVal(
		status,
		CL_SUCCESS,
		"clReleaseCommandQueue failed."))
		return SDK_FAILURE;

	status = clReleaseContext(context);
   if(!sampleCommon->checkVal(
		 status,
		 CL_SUCCESS,
		 "clReleaseContext failed."))
	  return SDK_FAILURE;

	/* release program resources (input memory etc.) */
	if(input) 
	   input=0;

   if(Cube) 
		Cube=0;
	
	if(devices)
		free(devices);

   return SDK_SUCCESS;
}

int main(int argc, char * argv[])
{
   FBP_Projection clFBP_Projection("OpenCL Simple Convolution");

  
   if(clFBP_Projection.initialize()!=SDK_SUCCESS)
	   return SDK_FAILURE;
   if(!clFBP_Projection.parseCommandLine(argc, argv))
	  return SDK_FAILURE;

	if(clFBP_Projection.isDumpBinaryEnabled())
	{
		return clFBP_Projection.genBinaryImage();
	}
	else
	{
	   if(clFBP_Projection.setup()!=SDK_SUCCESS)
		  return SDK_FAILURE;
	   if(clFBP_Projection.run()!=SDK_SUCCESS)
		  return SDK_FAILURE;
	   if(clFBP_Projection.verifyResults()!=SDK_SUCCESS)
		  return SDK_FAILURE;
	   if(clFBP_Projection.cleanup()!=SDK_SUCCESS)
		  return SDK_FAILURE;
	   clFBP_Projection.printStats();
	}

	return SDK_SUCCESS;
}

extern "C" __declspec( dllexport ) FBP_Projection* CreateFBP(char* KernalName, char* ConvolutionCode, char* FlagStr, 
	double* Image, int ImageWidth,int ImageHeight, double* Cube, int CubeWidth,int CubeHeight, int CubeDepth,int  ZStart, int ZLength)
{

	 std::cout << KernalName;
	  std::cout << ConvolutionCode;

	FBP_Projection* clFBP_Projection;

	clFBP_Projection=new  FBP_Projection("OpenCL Simple Convolution");
	
	clFBP_Projection->ConvolutionCode=std::string(ConvolutionCode);
	clFBP_Projection->flagsStr = std::string(FlagStr);
	clFBP_Projection->KernalName = std::string(KernalName);


	 std::cout << clFBP_Projection->ConvolutionCode;
	  std::cout <<clFBP_Projection->KernalName;

	clFBP_Projection->height =ImageHeight;
	clFBP_Projection->width =ImageWidth;
	clFBP_Projection->input =Image;

	clFBP_Projection->Cube=Cube;
	clFBP_Projection->CubeWidth=CubeWidth;
	clFBP_Projection->CubeHeight =CubeHeight;
	clFBP_Projection->CubeDepth =CubeDepth;
	clFBP_Projection->CubeZStart=ZStart;
	clFBP_Projection->CubeZLength=ZLength;
	
    clFBP_Projection->SetDeviceType("gpu");

	 std::cout << "Running Setup";

	 if(clFBP_Projection->setup()!=SDK_SUCCESS)
	 {
		std::cout << "Convolution Failed at setup";
		clFBP_Projection=NULL;
	 }

	return clFBP_Projection;
}


extern "C" __declspec( dllexport ) int RunFBP(FBP_Projection* clFBP_Projection,double* Image, float uX, float uY, float uZ, float aX, float aY, float aZ)
{
	
	clFBP_Projection->input =Image;
		

	cl_float4 UP = { uX,uY,uZ };
	cl_float4 Across = { aX,aY,aZ };

	clFBP_Projection->UP=UP;
	clFBP_Projection->Across=Across;

	clFBP_Projection->resetInput();

	if(clFBP_Projection->run()!=SDK_SUCCESS)
		  return SDK_FAILURE;

	return SDK_SUCCESS;
}


extern "C" __declspec( dllexport ) int getFBPData(FBP_Projection* clFBP_Projection)
{

	if(clFBP_Projection->getCLData()!=SDK_SUCCESS)
		  return SDK_FAILURE;

	return SDK_SUCCESS;
}



extern "C" __declspec( dllexport ) int CloseFBP(FBP_Projection* clFBP_Projection)
{
	 if(clFBP_Projection->cleanup()!=SDK_SUCCESS)
		  return SDK_FAILURE;

	 clFBP_Projection->printStats();
	 return SDK_SUCCESS;
}