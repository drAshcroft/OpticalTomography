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


#include "FBP_Projection.hpp"


int FBP_Projection::setupFBP_Projection()
{
  
	return SDK_SUCCESS;
}


int FBP_Projection::genBinaryImage()
{
	cl_int status = CL_SUCCESS;

	/*
	 * Have a look at the available platforms and pick either
	 * the AMD one if available or a reasonable default.
	 */
	cl_uint numPlatforms;
	cl_platform_id platform = NULL;
	status = clGetPlatformIDs(0, NULL, &numPlatforms);
	if(!sampleCommon->checkVal(status,
							   CL_SUCCESS,
							   "clGetPlatformIDs failed."))
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

		char platformName[100];
		for (unsigned i = 0; i < numPlatforms; ++i) 
		{
			status = clGetPlatformInfo(platforms[i],
									   CL_PLATFORM_VENDOR,
									   sizeof(platformName),
									   platformName,
									   NULL);

			if(!sampleCommon->checkVal(status,
									   CL_SUCCESS,
									   "clGetPlatformInfo failed."))
			{
				return SDK_FAILURE;
			}

			platform = platforms[i];
			if (!strcmp(platformName, "Advanced Micro Devices, Inc.")) 
			{
				break;
			}
		}
		std::cout << "Platform found : " << platformName << "\n";
		delete[] platforms;
	}

	if(NULL == platform)
	{
		sampleCommon->error("NULL platform found so Exiting Application.");
		return SDK_FAILURE;
	}

	/*
	 * If we could find our platform, use it. Otherwise use just available platform.
	 */
	cl_context_properties cps[5] = 
	{
		CL_CONTEXT_PLATFORM, 
		(cl_context_properties)platform, 
		CL_CONTEXT_OFFLINE_DEVICES_AMD,
		(cl_context_properties)1,
		0
	};

	context = clCreateContextFromType(cps,
									  CL_DEVICE_TYPE_ALL,
									  NULL,
									  NULL,
									  &status);

	if(!sampleCommon->checkVal(status,
							   CL_SUCCESS,
							   "clCreateContextFromType failed."))
	{
		return SDK_FAILURE;
	}

	/* create a CL program using the kernel source */
	streamsdk::SDKFile kernelFile;
	std::string kernelPath = sampleCommon->getPath();
	kernelPath.append("FBP_Projection_Kernels.cl");
	if(!kernelFile.open(kernelPath.c_str()))
	{
		std::cout << "Failed to load kernel file : " << kernelPath << std::endl;
		return SDK_FAILURE;
	}
	const char * source = kernelFile.source().c_str();
	size_t sourceSize[] = {strlen(source)};
	program = clCreateProgramWithSource(context,
										1,
										&source,
										sourceSize,
										&status);
	if(!sampleCommon->checkVal(status,
							   CL_SUCCESS,
							   "clCreateProgramWithSource failed."))
	{
		return SDK_FAILURE;
	}
	
	std::string flagsStr = std::string("");

	// Get additional options
	if(isComplierFlagsSpecified())
	{
		streamsdk::SDKFile flagsFile;
		std::string flagsPath = sampleCommon->getPath();
		flagsPath.append(flags.c_str());
		if(!flagsFile.open(flagsPath.c_str()))
		{
			std::cout << "Failed to load flags file: " << flagsPath << std::endl;
			return SDK_FAILURE;
		}
		flagsFile.replaceNewlineWithSpaces();
		const char * flags = flagsFile.source().c_str();
		flagsStr.append(flags);
	}

	if(flagsStr.size() != 0)
		std::cout << "Build Options are : " << flagsStr.c_str() << std::endl;


	/* create a cl program executable for all the devices specified */
	status = clBuildProgram(program,
							0,
							NULL,
							flagsStr.c_str(),
							NULL,
							NULL);
	sampleCommon->checkVal(status,
						CL_SUCCESS,
						"clBuildProgram failed.");
	size_t numDevices;
	status = clGetProgramInfo(program, 
						   CL_PROGRAM_NUM_DEVICES,
						   sizeof(numDevices),
						   &numDevices,
						   NULL );
	if(!sampleCommon->checkVal(status,
							   CL_SUCCESS,
							   "clGetProgramInfo(CL_PROGRAM_NUM_DEVICES) failed."))
	{
		return SDK_FAILURE;
	}

	std::cout << "Number of devices found : " << numDevices << "\n\n";
	devices = (cl_device_id *)malloc( sizeof(cl_device_id) * numDevices );
	if(devices == NULL)
	{
		sampleCommon->error("Failed to allocate host memory.(devices)");
		return SDK_FAILURE;
	}
	/* grab the handles to all of the devices in the program. */
	status = clGetProgramInfo(program, 
							  CL_PROGRAM_DEVICES, 
							  sizeof(cl_device_id) * numDevices,
							  devices,
							  NULL );
	if(!sampleCommon->checkVal(status,
							   CL_SUCCESS,
							   "clGetProgramInfo(CL_PROGRAM_DEVICES) failed."))
	{
		return SDK_FAILURE;
	}


	/* figure out the sizes of each of the binaries. */
	size_t *binarySizes = (size_t*)malloc( sizeof(size_t) * numDevices );
	if(devices == NULL)
	{
		sampleCommon->error("Failed to allocate host memory.(binarySizes)");
		return SDK_FAILURE;
	}
	
	status = clGetProgramInfo(program, 
							  CL_PROGRAM_BINARY_SIZES,
							  sizeof(size_t) * numDevices, 
							  binarySizes, NULL);
	if(!sampleCommon->checkVal(status,
							   CL_SUCCESS,
							   "clGetProgramInfo(CL_PROGRAM_BINARY_SIZES) failed."))
	{
		return SDK_FAILURE;
	}

	size_t i = 0;
	/* copy over all of the generated binaries. */
	char **binaries = (char **)malloc( sizeof(char *) * numDevices );
	if(binaries == NULL)
	{
		sampleCommon->error("Failed to allocate host memory.(binaries)");
		return SDK_FAILURE;
	}

	for(i = 0; i < numDevices; i++)
	{
		if(binarySizes[i] != 0)
		{
			binaries[i] = (char *)malloc( sizeof(char) * binarySizes[i]);
			if(binaries[i] == NULL)
			{
				sampleCommon->error("Failed to allocate host memory.(binaries[i])");
				return SDK_FAILURE;
			}
		}
		else
		{
			binaries[i] = NULL;
		}
	}
	status = clGetProgramInfo(program, 
							  CL_PROGRAM_BINARIES,
							  sizeof(char *) * numDevices, 
							  binaries, 
							  NULL);
	if(!sampleCommon->checkVal(status,
							   CL_SUCCESS,
							   "clGetProgramInfo(CL_PROGRAM_BINARIES) failed."))
	{
		return SDK_FAILURE;
	}

	/* dump out each binary into its own separate file. */
	for(i = 0; i < numDevices; i++)
	{
		char fileName[100];
		sprintf(fileName, "%s.%d", dumpBinary.c_str(), (int)i);
		if(binarySizes[i] != 0)
		{
			char deviceName[1024];
			status = clGetDeviceInfo(devices[i], 
									 CL_DEVICE_NAME, 
									 sizeof(deviceName),
									 deviceName, 
									 NULL);
			if(!sampleCommon->checkVal(status,
									   CL_SUCCESS,
									   "clGetDeviceInfo(CL_DEVICE_NAME) failed."))
			{
				return SDK_FAILURE;
			}

			printf( "%s binary kernel: %s\n", deviceName, fileName);
			streamsdk::SDKFile BinaryFile;
			if(!BinaryFile.writeBinaryToFile(fileName, 
											 binaries[i], 
											 binarySizes[i]))
			{
				std::cout << "Failed to load kernel file : " << fileName << std::endl;
				return SDK_FAILURE;
			}
		}
		else
		{
			printf("Skipping %s since there is no binary data to write!\n",
					fileName);
		}
	}

	// Release all resouces and memory
	for(i = 0; i < numDevices; i++)
	{
		if(binaries[i] != NULL)
		{
			free(binaries[i]);
			binaries[i] = NULL;
		}
	}

	if(binaries != NULL)
	{
		free(binaries);
		binaries = NULL;
	}

	if(binarySizes != NULL)
	{
		free(binarySizes);
		binarySizes = NULL;
	}

	if(devices != NULL)
	{
		free(devices);
		devices = NULL;
	}

	status = clReleaseProgram(program);
	if(!sampleCommon->checkVal(status,
							   CL_SUCCESS,
							   "clReleaseProgram failed."))
	{
		return SDK_FAILURE;
	}

	status = clReleaseContext(context);
	if(!sampleCommon->checkVal(status,
							   CL_SUCCESS,
							   "clReleaseContext failed."))
	{
		return SDK_FAILURE;
	}

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
	if(!sampleCommon->checkVal(status,
		CL_SUCCESS,
		"clGetPlatformIDs failed."))
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
		sampleCommon->error("NULL platform found so Exiting Application.");
		return SDK_FAILURE;
	}

	// Display available devices.
	if(!sampleCommon->displayDevices(platform, dType))
	{
		sampleCommon->error("sampleCommon::displayDevices() failed");
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
		sampleCommon->error("sampleCommon::validateDeviceId() failed");
		return SDK_FAILURE;
	}

	/* Now allocate memory for device list based on the size we got earlier */
	devices = (cl_device_id *)malloc(deviceListSize);
	if(devices == NULL)
	{
		sampleCommon->error("Failed to allocate memory (devices).");
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
	globalThreads[2]=CubeDepth;
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

	/*
	// create 3D array and copy data to device
	cl_image_format volume_format;
	volume_format.image_channel_order = CL_INTENSITY;
	volume_format.image_channel_data_type =CL_FLOAT;

	d_volumeArray = clCreateImage3D(context, CL_MEM_READ_WRITE | CL_MEM_USE_HOST_PTR , &volume_format, 
	CubeWidth, CubeHeight, CubeDepth,
	(CubeWidth*4), (CubeWidth*CubeHeight*4),
	Cube, &status);

	if(!sampleCommon->checkVal(
	status,
	CL_SUCCESS,
	"clCreateImage3D failed."))
	return SDK_FAILURE;



	// set image and sampler args

	*/


	d_volumeArray = clCreateBuffer(
		context, 
		CL_MEM_READ_WRITE | CL_MEM_USE_HOST_PTR,
		sizeof(cl_double) * CubeWidth*CubeHeight*CubeDepth,
		Cube, 
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

	std::cout << width << "  " << height << "\r";

	cl_uint2 inputDimensions = {width, height};
	cl_uint4 cubeDimensions = {CubeWidth,CubeHeight,CubeDepth,0};

	std::cout << CubeWidth << CubeHeight << CubeDepth << "\r";

	status = clSetKernelArg( kernel, 2, sizeof(cl_uint2), (void *)&inputDimensions);
	if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (inputDim)"))
		return SDK_FAILURE;


	status = clSetKernelArg(kernel,  3, sizeof(cl_uint4), (void *)&cubeDimensions);
	if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (cubeDIM)"))
		return SDK_FAILURE;

	/*
	status = clSetKernelArg( kernel, 4, sizeof(cl_float2), (void *)&UP);
	if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (ZInfo)"))
	return SDK_FAILURE;


	status = clSetKernelArg( kernel, 5, sizeof(cl_float2), (void *)&Across);
	if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (ZInfo)"))
	return SDK_FAILURE;
	*/


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
	if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (UP)"))
		return SDK_FAILURE;


	status = clSetKernelArg( kernel, 5, sizeof(cl_float4), (void *)&Across);
	if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (Across)"))
		return SDK_FAILURE;

	status = clSetKernelArg( kernel, 6, sizeof(cl_double), (void *)&IntensityFactor);
	if(!sampleCommon->checkVal(status, CL_SUCCESS,"clSetKernelArg failed. (IntensityFactor)"))
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

	if (0==Cube)
		return 0;

	/* Enqueue readBuffer*/
	/*status = clEnqueueReadImage(
				commandQueue,
				d_volumeArray,
				CL_TRUE,
				origin,
				globalThreads,
				(CubeWidth*4),
				(CubeWidth*CubeHeight*4),
				Cube,
				0,
				NULL,
				&events[1]);*/

	 /* Enqueue readBuffer*/
    status = clEnqueueReadBuffer(
                commandQueue,
                d_volumeArray,
                CL_TRUE,
                0,
               (CubeWidth*CubeHeight*CubeDepth*sizeof(cl_double )),
                Cube,
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
	double* Image, int ImageWidth,int ImageHeight, double* Cube, int CubeWidth,int CubeHeight, int CubeDepth)
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

	if (CubeWidth>250)
	{
		clFBP_Projection->SetDeviceType("cpu");
	}
	else
	{
		clFBP_Projection->SetDeviceType("gpu");
	}

	 std::cout << "Running Setup";

	 if(clFBP_Projection->setup()!=SDK_SUCCESS)
	 {
		std::cout << "Convolution Failed at setup";
		clFBP_Projection=NULL;
	 }

	return clFBP_Projection;
}


extern "C" __declspec( dllexport ) int RunFBP(FBP_Projection* clFBP_Projection,double* Image, float IntensityFactor, float uX, float uY, float uZ, float aX, float aY, float aZ)
{
	
	clFBP_Projection->input =Image;
		

	cl_float4 UP = { uX,uY,uZ };
	cl_float4 Across = { aX,aY,aZ };

	clFBP_Projection->UP=UP;
	clFBP_Projection->Across=Across;
	clFBP_Projection->IntensityFactor = IntensityFactor;
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