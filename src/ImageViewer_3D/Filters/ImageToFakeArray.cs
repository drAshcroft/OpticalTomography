using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageViewer3D.Filters
{
    public unsafe class ImageToFakeArrayGrayscale
    {
        private Int32* Scan0;
        private int Stride;
        private int[] Dims;

        public double this[int X, int Y]
        {
            get
            {
                byte* channels = (byte*)(Scan0 + Y * Stride + X);

                return ((double)channels[0] + (double)channels[1] + (double)channels[2]) / 3d;
            }
            set
            {
                *(Scan0 + Y * Stride + X)=(Int32)value ;
            }
        }
        public void SetGray( int X, int Y ,byte GrayScaleValue)
        {
                byte* channels = (byte*)(Scan0 + Y * Stride + X);
                channels[0] = GrayScaleValue;
                channels[1] = GrayScaleValue;
                channels[2] = GrayScaleValue;
                channels[3] = 255;
        }
        public int GetLength(int Dimension)
        {
            return Dims[Dimension];
        }
        public ImageToFakeArrayGrayscale(IntPtr scan0, int stride, int Width,int Height)
        {
            Scan0 = (Int32*)(void*)scan0;
            Stride = stride/4;
            Dims = new int[2];
            Dims[0] = Width;
            Dims[1] = Height;
        }
    }
}
