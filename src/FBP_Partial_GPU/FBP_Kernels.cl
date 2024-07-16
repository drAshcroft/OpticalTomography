#pragma OPENCL EXTENSION cl_amd_fp64 : enable 

double cubicInterpolate (double p[4], double u) {
	return p[1] + 0.5 * u*(p[2] - p[0] + u*(2.0*p[0] - 5.0*p[1] + 4.0*p[2] - p[3] + u*(3.0*(p[1] - p[2]) + p[3] - p[0])));
}

double bicubicInterpolate (double p[4][4], double uX, double uY) {
	double arr[4];
	arr[0] = cubicInterpolate(p[0], uY);
	arr[1] = cubicInterpolate(p[1], uY);
	arr[2] = cubicInterpolate(p[2],uY);
	arr[3] = cubicInterpolate(p[3],uY);
	return cubicInterpolate(arr, uX);
}
/*
double linearInterpolate (double p[2], double u) {
	return p[0]*u+p[1]*(1-u);
}

double biLinearInterpolate (double p[2][2], double uX, double uY) {
	double arr[2];
	arr[0] = linearInterpolate(p[0], uY);
	arr[1] = linearInterpolate(p[1], uY);
	return linearInterpolate(arr, uX);
}*/

__kernel void simpleFBP( __global  double * volume,
                         __global  double * input,
                         const     uint2  inputDimensions,
                         const     uint4  cubeDimensions,
						 const     float4 UP,
                         const     float4 Across
                         )
 {

            uint iWidth = inputDimensions.x;
            uint iHeight = inputDimensions.y;

            int hiWidth = (int)(iWidth / 2);
            int hiHeight = (int)(iHeight / 2);

            uint cWidth = cubeDimensions.x;
            uint cHeight = cubeDimensions.y;
            uint cDepth = cubeDimensions.y;

            int hcWidth = (int)(cWidth / 2);
            int hcHeight = (int)(cHeight / 2);
            int hcDepth = (int)(cDepth / 2);

            uint SliceSize = cHeight * cDepth;

            uint xI =( get_global_id(0));
            uint yI =(  get_global_id(1));
            uint zI =(  get_global_id(2));

            uint cTmp = xI + yI * cWidth + zI * SliceSize;
            uint iTmp00,iTmp01,iTmp10,iTmp11;

            int xII =(int)xI - hcWidth;
            int yII =(int) yI - hcHeight;
            int zII = (int)zI - hcDepth;

            double  x, y,rX,rY,uX,uY,V1,V2;
            
            //calculate the position of the current voxel on the image
            

            x = UP.x * xII + UP.y *yII +UP.z *zII  + hiWidth ;
            y = Across.x * xII + Across.y * yII + Across.z * zII+ hiHeight ;

             if (x>0 && x<iWidth)
             if (y>0 && y<iHeight)
            {
                rX = floor(x);
                rY = floor(y);

                if (x>0 && x<iWidth-1)
                if (y>0 && y<iHeight-1)
                {
                    uX=x-rX;
                    uY=y-rY;
					
					
					iTmp00 = (uint)( x + y * iWidth);
                    iTmp10 = iTmp00+1;
                    iTmp01 =(uint)( x + (y+1) * iWidth);
                    iTmp11 = iTmp10+1;
					
                    V1 = input[iTmp00]*uY+input[iTmp01]*(1-uY);
                    V2 = input[iTmp10]*uY+input[iTmp11]*(1-uY);
              
                    volume[cTmp] +=  V1*uX+V2*(1-uX);
                }
            }

}