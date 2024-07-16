using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageViewer.Filters
{
    /// <summary>
    /// Used to take a pointer to an image and make it look like a normal .net array
    /// </summary>
    public unsafe class ImageToFakeArrayGrayscale
    {
        /// <summary>
        /// Scan0 acquired from image
        /// </summary>
        private Int32* Scan0;
        /// <summary>
        /// Number in bytes to the width of each row
        /// </summary>
        private int Stride;

        /// <summary>
        /// Height and Width
        /// </summary>
        private int[] Dims;

        /// <summary>
        /// returns the intensity pixel at this point
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public double this[int X, int Y]
        {
            get
            {
                //get the location of the pixel
                byte* channels = (byte*)(Scan0 + Y * Stride + X);
                ///averte the values to get the average intensity
                return ((double)channels[0] + (double)channels[1] + (double)channels[2]) / 3d;
            }
            /*set
            {
                *(Scan0 + Y * Stride + X)=(Int32)value ;
            }*/
        }
        
        /// <summary>
        /// Sets the value of all three channels to the specified level
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="GrayScaleValue"></param>
        public void SetGray( int X, int Y ,byte GrayScaleValue)
        {
                byte* channels = (byte*)(Scan0 + Y * Stride + X);
                channels[0] = GrayScaleValue;
                channels[1] = GrayScaleValue;
                channels[2] = GrayScaleValue;
                channels[3] = 255;
        }

        /// <summary>
        /// returns the width or height of the image, in the same style as an array
        /// </summary>
        /// <param name="Dimension"></param>
        /// <returns></returns>
        public int GetLength(int Dimension)
        {
            return Dims[Dimension];
        }

        /// <summary>
        /// The image must have been locked to get its pointer information.  
        /// </summary>
        /// <param name="scan0"></param>
        /// <param name="stride"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
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
