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



namespace ImageViewer.PythonScripting.Projection
{
    public class Convolution1DGPU : aEffectNoForm
    {
        public override string EffectName { get { return "Convolution 1D On GPU"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Rank Order Statistical"; } }
        public override int OrderSuggestion { get { return 1; } }


        static GPUConvolution.GPUConvoluter1D Convoluter = null;
        static int ReferenceCount = 0;
        static object CriticalSectionLock = new object();
        static object CriticalSectionLock2 = new object();


        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            mPassData = PassData;
            double[] Kernal = (double[])mFilterToken[1];
            lock (CriticalSectionLock)
            {
                if (Convoluter == null)
                {
                    Convoluter = new GPUConvolution.GPUConvoluter1D();
                    Convoluter.SetupConvolution( );
                    ReferenceCount = 0;
                }
                ReferenceCount += 1;
            }

           
            if (mFilterToken == null)
            {
                mFilterToken = DefaultProperties;
            }

            mDataEnvironment = dataEnvironment;

           
            float[] fKernal = new float[Kernal.Length];
            for (int i = 0; i < Kernal.Length; i++)
                //for (int j = 0; j < Kernal.Length; j++)
                    fKernal[i] = (float)Kernal[i];

            float[,] DataOut = null;
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                Bitmap b = (Bitmap)SourceImage;
                if ((int)mFilterToken[0] == 0)
                    DataOut = ImagingTools.SliceAndPadZeroArrayToFloat(ImagingTools.ConvertToDoubleArray(b, false), b.Width + fKernal.GetLength(0), b.Height + fKernal.GetLength(1));
                else
                    DataOut = ImagingTools.SliceAndPadZeroArrayToFloat(ImagingTools.ConvertToDoubleArray(b, true), b.Width + fKernal.GetLength(0), b.Height + fKernal.GetLength(1));
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                ImageHolder ih = ((ImageHolder)SourceImage);
                if ((int)mFilterToken[0] == 0)
                {
                    DataOut = ImagingTools.SliceAndPadZeroArrayToFloat(ih, 0, ih.Width + fKernal.GetLength(0), ih.Height + fKernal.GetLength(0));// ImagingTools.ConvertToFloatArray(((ImageHolder)SourceImage), false);
                }
                else
                    DataOut = ImagingTools.SliceAndPadZeroArrayToFloat(ih, 0, ih.Width + fKernal.GetLength(0), ih.Height + fKernal.GetLength(0));// ImagingTools.ConvertToFloatArray((ImageHolder)SourceImage, true);
            }

            DataOut = Convolve(DataOut, fKernal);

            double dWidth = EffectHelps.ConvertToDouble(mFilterToken[3]) / 2d;
            double dHeight = EffectHelps.ConvertToDouble(mFilterToken[4]) / 2d;

            PhysicalArray PhysOut;
            if ((int)mFilterToken[0] == 0)
                PhysOut = new PhysicalArray(DataOut, -1 * dWidth, dWidth, -1 * dHeight, dHeight, true);
            else
                PhysOut = new PhysicalArray(DataOut, -1 * dWidth, dWidth, -1 * dHeight, dHeight, false);

            mPassData.AddSafe("ConvolutionData", PhysOut);

            lock (CriticalSectionLock)
            {
                ReferenceCount--;
                if (ReferenceCount == 0)
                    Convoluter = null;
            }

            if ((bool)mFilterToken[2] == true)
            {
                return new ImageHolder(PhysOut.MakeBitmap());
            }
            else
                return SourceImage;
        }




        private unsafe float[,] Convolve(float[,] DataIn, float[] Kernal)
        {
            float[,] Slice = Convoluter.ConvolutionGPU (DataIn, Kernal);
            return Slice;
        }

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
            get { return new string[] { "Axis|int(0 means X, 1 means Y)", "Kernal|double[]", "ShowConvolution", "PhysicalWidth|double", "PhysicalHeight|double" }; }
        }

    }
}
