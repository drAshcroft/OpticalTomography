

__kernel void simpleConvolution(__global  float  * output,
                                __global  float  * input,
                                __global  float  * mask,
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
    int left  =(x - vstep);//   = (x           <  vstep) ? 0         : (x - vstep);
    int right=(x + vstep); //   = ((x + vstep) >= width) ? width - 1 : (x + vstep); 
    
	/*
		 * initializing wighted sum value
		 */
	float sumFX = 0;

	if (left >0 && right <width)
	{
		
		for(uint i = left; i <= right; ++i)
		{
		   uint j=y;   
			{
				/*
				 * performing wighted sum within the mask boundaries
				 */
				uint maskIndex = (j - (y - hstep)) * maskWidth  + (i - (x - vstep));
				uint index     = j                 * width      + i;
            
				sumFX += (input[index] * mask[maskIndex]);
			}
	   }
	   sumFX=1;
   }
   else if (left<0 && right <width)
   {
////////////////////////////////////////////////////get left apron/////////////////////////////////////////////

		uint index     = y * width;
		float leftValue=input[index];
        for(uint i = left; i <= 0; ++i)
		{
		   uint j=y;   
			{
				/*
				 * performing wighted sum within the mask boundaries
				 */
				uint maskIndex = (j - (y - hstep)) * maskWidth  + (i - (x - vstep));
            
				sumFX += leftValue * mask[maskIndex];
			}
	   }


		for(uint i = 0; i <= right; ++i)
		{
		   uint j=y;   
			{
				/*
				 * performing wighted sum within the mask boundaries
				 */
				uint maskIndex = (j - (y - hstep)) * maskWidth  + (i - (x - vstep));
				index     = j * width + i;
            
				sumFX += (input[index] * mask[maskIndex]);
			}
	   }
	   
   }

   else if (left>0 && right >width)
   {
/////////////////////////////////////////get right apron///////////////////////////////////////////////////////
		uint index;

		for(uint i = left; i <= width-1; ++i)
		{
		   uint j=y;   
			{
				/*
				 * performing wighted sum within the mask boundaries
				 */
				uint maskIndex = (j - (y - hstep)) * maskWidth  + (i - (x - vstep));
				index = j * width + i;
            
				sumFX += (input[index] * mask[maskIndex]);
			}
	   }

	   float rightValue=input[index];
    
	   for(uint i = width-1; i <= right; ++i)
	   { 
		   uint j=y;   
			{
				/*
				 * performing wighted sum within the mask boundaries
				 */
				uint maskIndex = (j - (y - hstep)) * maskWidth  + (i - (x - vstep));
            
				sumFX += rightValue * mask[maskIndex];
			}
	   }
	   sumFX=3;
   }
   else if (left>0 && right >width)
   {
/////////////////////////////////////////get both aprons///////////////////////////////////////////////////////
		uint index     = y * width;
		float leftValue=input[index];
        for(uint i = left; i <= 0; ++i)
		{
		   uint j=y;   
			{
				/*
				 * performing wighted sum within the mask boundaries
				 */
				uint maskIndex = (j - (y - hstep)) * maskWidth  + (i - (x - vstep));
            
				sumFX += leftValue * mask[maskIndex];
			}
	   }

		for(uint i = left; i <= width-1; ++i)
		{
		   uint j=y;   
			{
				/*
				 * performing wighted sum within the mask boundaries
				 */
				uint maskIndex = (j - (y - hstep)) * maskWidth  + (i - (x - vstep));
				index = j * width + i;
            
				sumFX += (input[index] * mask[maskIndex]);
			}
	   }

	   float rightValue=input[index];
    
	   for(uint i = width-1; i <= right; ++i)
	   { 
		   uint j=y;   
			{
				/*
				 * performing wighted sum within the mask boundaries
				 */
				uint maskIndex = (j - (y - hstep)) * maskWidth  + (i - (x - vstep));
            
				sumFX += rightValue * mask[maskIndex];
			}
	   }
	   sumFX=4;
   }

    output[tid] = sumFX;
}
