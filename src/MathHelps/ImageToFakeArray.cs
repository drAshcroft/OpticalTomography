using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace MathHelpLib
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

        BitmapData bmd;
        Bitmap TheImage;

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

        public ImageToFakeArrayGrayscale(Bitmap TheImage)
        {
            this.TheImage = TheImage;
            BitmapData bmd = TheImage.LockBits(new Rectangle(0, 0, TheImage.Width, TheImage.Height), ImageLockMode.ReadWrite, TheImage.PixelFormat);


            Scan0 = (Int32*)(void*)bmd.Scan0 ;
            Stride = bmd.Stride  / 4;
            Dims = new int[2];
            Dims[0] = TheImage.Width ;
            Dims[1] = TheImage.Height ;
        }

        ~ImageToFakeArrayGrayscale()
        {
            ReleaseImage();
        }

        public void ReleaseImage()
        {
            TheImage.UnlockBits(bmd);
        }
    }
}
