﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ImageViewer.Convolution;
using ImageViewer.Filters;
using MathHelpLib;
using System.Runtime.InteropServices;
using System.IO;

namespace ImageViewer.PythonScripting.Projection
{
    public partial class Convolution1DFilterArrayTool : aEffectForm
    {

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bTest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pPreview;
        private System.Windows.Forms.Button blocalFinished;

        public Convolution1DFilterArrayTool()
            : base()
        {
            /*
            pInitializeComponent();
            splitContainer1.Visible = false;
            button1.Visible = false;

           // dataGridView1.RowCount = 11;
           // dataGridView1.ColumnCount = 11;

            blocalFinished.Click += new EventHandler(blocalFinished_Click);
            bTest.Click += new EventHandler(bTest_Click);*/
        }

        void blocalFinished_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void pInitializeComponent()
        {

            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.bTest = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.pPreview = new System.Windows.Forms.PictureBox();
            this.blocalFinished = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(589, 49);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(287, 234);
            this.dataGridView1.TabIndex = 27;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(589, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Kernal";
            // 
            // bTest
            // 
            this.bTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bTest.Location = new System.Drawing.Point(801, 289);
            this.bTest.Name = "bTest";
            this.bTest.Size = new System.Drawing.Size(75, 23);
            this.bTest.TabIndex = 29;
            this.bTest.Text = "Test Filter";
            this.bTest.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(589, 317);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(211, 38);
            this.label2.TabIndex = 30;
            this.label2.Text = "Kernals must be square and have odd number rank for function to work";
            // 
            // pPreview
            // 
            this.pPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pPreview.Location = new System.Drawing.Point(12, 12);
            this.pPreview.Name = "pPreview";
            this.pPreview.Size = new System.Drawing.Size(571, 586);
            this.pPreview.TabIndex = 31;
            this.pPreview.TabStop = false;
            // 
            // blocalFinished
            // 
            this.blocalFinished.Location = new System.Drawing.Point(801, 571);
            this.blocalFinished.Name = "blocalFinished";
            this.blocalFinished.Size = new System.Drawing.Size(75, 27);
            this.blocalFinished.TabIndex = 32;
            this.blocalFinished.Text = "Finished";
            this.blocalFinished.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(888, 606);
            this.ControlBox = false;
            this.Controls.Add(this.blocalFinished);
            this.Controls.Add(this.pPreview);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bTest);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "ContrastTool";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        public override string EffectName { get { return "Convolution 1D Filter With Arrays"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Rank Order Statistical"; } }
        public override int OrderSuggestion { get { return 1; } }

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

        [DllImport("SimpleConvolution.dll")]
        private static unsafe extern int CreateConvoluter(string KernalName, string ConvolutionCode, string CompilationFlags, float* OutputImage, float* Image, int ImageWidth, int ImageHeight, float* Filter, int FilterWidth, int FilterHeight);

        [DllImport("SimpleConvolution.dll")]
        private static unsafe extern int RunConvoluter(float* Image);

        [DllImport("SimpleConvolution.dll")]
        private static unsafe extern int RunConvoluterInputOutput(float* Image, float* output);

        [DllImport("SimpleConvolution.dll")]
        private static unsafe extern int CloseConvoluter();

        ~Convolution1DFilterArrayTool()
        {
            try
            {
                //CloseConvoluter();
            }
            catch { }
        }

        protected override object doEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            mPassData = PassData;
            if (mFilterToken == null)
            {
                mFilterToken = DefaultProperties;
            }

            mDataEnvironment = dataEnvironment;

            double[] Kernal = (double[])mFilterToken[1];

            double[,] DataOut = null;
           // GPUError = true;
            if (GPUError == false && File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\simpleConvolution.dll") == true)
            {
                lock (CriticalSectionLock)
                {
                    float[,] DataIn = null;
                    int OriginalWidth = 0;
                    int OriginalHeight = 0;

                    if (SourceImage.GetType() == typeof(Bitmap))
                    {
                        Bitmap SourceI = (Bitmap)SourceImage;
                        if ((int)mFilterToken[0] == 0)
                        {
                            OriginalWidth = SourceI.Width;
                            OriginalHeight = SourceI.Height;

                            DataIn = ImagingTools.ConvertToFloatArrayPowerOf2(SourceI, false);
                        }
                        else
                        {
                            OriginalWidth = SourceI.Height;
                            OriginalHeight = SourceI.Width;
                            DataIn = ImagingTools.ConvertToFloatArrayPowerOf2(SourceI, true);
                        }
                    }
                    else if (SourceImage.GetType() == typeof(ImageHolder))
                    {
                        ImageHolder SourceI = (ImageHolder)SourceImage;
                        if ((int)mFilterToken[0] == 0)
                        {
                            OriginalWidth = SourceI.Width;
                            OriginalHeight = SourceI.Height;
                            DataIn = ImagingTools.ConvertToFloatArrayPowerOf2(SourceI, false);
                        }
                        else
                        {
                            OriginalWidth = SourceI.Height;
                            OriginalHeight = SourceI.Width;
                            DataIn = ImagingTools.ConvertToFloatArrayPowerOf2(SourceI, true);
                        }
                    }

                    DataOut = ConvolveGPU(DataIn, Kernal, OriginalWidth, OriginalHeight);
                }
            }
            else
            {
                double[,] DataIn = null;
                if (SourceImage.GetType() == typeof(Bitmap))
                {
                    if ((int)mFilterToken[0] == 0)
                        DataIn = ImagingTools.ConvertToDoubleArray((Bitmap)SourceImage, false);
                    else
                        DataIn = ImagingTools.ConvertToDoubleArray((Bitmap)SourceImage, true);
                }
                else if (SourceImage.GetType() == typeof(ImageHolder))
                {
                    if ((int)mFilterToken[0] == 0)
                        DataIn = ImagingTools.ConvertToDoubleArray(((ImageHolder)SourceImage), false);
                    else
                        DataIn = ImagingTools.ConvertToDoubleArray((ImageHolder)SourceImage, true);
                }
                DataOut = Convolve(DataIn, Kernal);
            }

            try
            {
                mPassData.AddSafe("ConvolutionData", DataOut);
            }
            catch { }
            if ((bool)mFilterToken[2] == true)
            {
                return new ImageHolder(DataOut);
            }
            else
                return SourceImage;
        }

        static bool GPUError = false;
        static bool ProgramCreated = false;
        static float[,] OutImage = null;

        static float[] Mask;
        static GCHandle MaskHandle;
        static IntPtr MaskAddress;

        static GCHandle OutHandle;
        static IntPtr OutAddress;

        static object CriticalSectionLock = new object();
        private unsafe double[,] ConvolveGPU(float[,] DataIn, double[] Kernal, int OriginalWidth, int OriginalHeight)
        {

            //set up the convoluter
            if (ProgramCreated == false)
            {
                OutImage = new float[DataIn.GetLength(0), DataIn.GetLength(1)];
                OutHandle = GCHandle.Alloc(OutImage, GCHandleType.Pinned);

                Mask = new float[Kernal.GetLength(0)];
                for (int i = 0; i < Mask.Length; i++)
                    Mask[i] = (float)Kernal[i];
                MaskHandle = GCHandle.Alloc(Mask, GCHandleType.Pinned);
                try
                {
                    OutAddress = OutHandle.AddrOfPinnedObject();
                    // Get the address of the data array
                    MaskAddress = MaskHandle.AddrOfPinnedObject();
                    fixed (float* pImage = DataIn)
                    {
                        CreateConvoluter("simpleConvolution", ConvolutionProgram, "", (float*)OutAddress, pImage, DataIn.GetLength(0), DataIn.GetLength(1), (float*)MaskAddress, Mask.GetLength(0), 1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("GPUConvolution\r" + ex.Message);
                    GPUError = true;
                }
                ProgramCreated = true;
            }


            int OffsetX = (int)((DataIn.GetLength(0) - OriginalHeight) / 2d);
            int OffsetY = (int)((DataIn.GetLength(1) - OriginalWidth) / 2d);

            double[,] DataOut = new double[OriginalHeight, OriginalWidth];
            int Width = DataIn.GetLength(0);

            int oWidth = DataOut.GetLength(1);
            int oHeight = DataOut.GetLength(0);
            //run the convolution.   Setting up the program is expensive, so we only do it once.
            fixed (float* pImage = DataIn)
            {
                RunConvoluter(pImage);

                fixed (double* pDataOut = DataOut)
                {
                    for (int y = 0; y < oHeight; y++)
                    {
                        float* pConvImage = (float*)OutAddress + (y + OffsetX) * Width + OffsetY;

                        double* pOut = pDataOut + y * oWidth;

                        for (int x = 0; x < oWidth; x++)
                        {
                            *pOut = *pConvImage;
                            pOut++;
                            pConvImage++;
                        }
                    }
                }
            }
            return DataOut;

        }

        #region ConvolutionProgram
        string ConvolutionProgram = @"


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
    int left  =(x - vstep);
    int right=(x + vstep); 
    
	/*
		 * initializing wighted sum value
		 */
	float sumFX = 0;

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

";

        #endregion
        private unsafe double[,] Convolve(double[,] DataIn, double[] Kernal)
        {
            double[,] ArrayOut = new double[DataIn.GetLength(0), DataIn.GetLength(1)];
            fixed (double* pArrayIn = DataIn)
            {
                fixed (double* pKernal = Kernal)
                {
                    fixed (double* pOut = ArrayOut)
                    {
                        // ConvolveX(pArrayIn, pOut, DataIn.GetLength(0), DataIn.GetLength(1), pKernal, Kernal.Length);
                        int Width = DataIn.GetLength(0);
                        int Height = DataIn.GetLength(1);
                        for (int i = 0; i < Height; i++)
                        {
                            ConvoluteChop(pArrayIn + i * Width, Width, pKernal, Kernal.Length, pOut + i * Width);
                        }

                    }
                }
            }
            return ArrayOut;
        }

        public static unsafe void ConvoluteChop(double* Array1, int Length1, double* pImpulse, int Length2, double* pArrayOut)
        {

            int LengthWhole = Length1 + Length2;

            int StartI = (int)Math.Truncate((double)LengthWhole / 2d - (double)Length1 / 2d);
            int EndI = (int)Math.Truncate((double)LengthWhole / 2d + (double)Length1 / 2d);
            if (EndI - StartI > Length1)
                EndI--;
            int sI, eI;


            double p1;
            double* p2;
            double* pOut;

            unchecked
            {
                for (int i = 0; i < Length1; i++)
                {
                    p1 = Array1[i];
                    sI = StartI - i;
                    eI = EndI - i;
                    if (eI > Length2) eI = Length2;
                    if (sI < 0) sI = 0;
                    if (sI < eI)
                    {
                        p2 = pImpulse + sI;
                        pOut = pArrayOut + i + sI - StartI;
                        for (int j = sI; j < eI; j++)
                        {
                            *pOut += p1 * (*p2);
                            pOut++;
                            p2++;
                        }
                    }
                }
            }
        }


        ///////////////////////////////////////////////////////////////////////////////
        // double precision float version
        ///////////////////////////////////////////////////////////////////////////////
        private unsafe void ConvolveX(double* ArrayIn, double* ArrayOut, int dataSizeX, int dataSizeY, double* kernelX, int kSizeX)
        {
            int i, j, k, m;

            // intermediate data buffer
            double* inPtr;                         // working pointers
            double* tmpPtr;                       // working pointers
            int kCenter, kOffset, endIndex;                 // kernel indice



            // covolve horizontal direction ///////////////////////

            // find center position of kernel (half of kernel size)
            kCenter = kSizeX >> 1;                          // center index of kernel array
            endIndex = dataSizeX - kCenter;                 // index for full kernel convolution

            // init working pointers
            inPtr = ArrayIn;
            tmpPtr = ArrayOut;                                   // store intermediate results from 1D horizontal convolution

            // start horizontal convolution (x-direction)
            for (i = 0; i < dataSizeY; ++i)                    // number of rows
            {

                kOffset = 0;                                // starting index of partial kernel varies for each sample

                // COLUMN FROM index=0 TO index=kCenter-1
                for (j = 0; j < kCenter; ++j)
                {
                    *tmpPtr = 0;                            // init to 0 before accumulation

                    for (k = kCenter + kOffset, m = 0; k >= 0; --k, ++m) // convolve with partial of kernel
                    {
                        *tmpPtr += *(inPtr + m) * kernelX[k];
                    }
                    ++tmpPtr;                               // next output
                    ++kOffset;                              // increase starting index of kernel
                }

                // COLUMN FROM index=kCenter TO index=(dataSizeX-kCenter-1)
                for (j = kCenter; j < endIndex; ++j)
                {
                    *tmpPtr = 0;                            // init to 0 before accumulate

                    for (k = kSizeX - 1, m = 0; k >= 0; --k, ++m)  // full kernel
                    {
                        *tmpPtr += *(inPtr + m) * kernelX[k];
                    }
                    ++inPtr;                                // next input
                    ++tmpPtr;                               // next output
                }

                kOffset = 1;                                // ending index of partial kernel varies for each sample

                // COLUMN FROM index=(dataSizeX-kCenter) TO index=(dataSizeX-1)
                for (j = endIndex; j < dataSizeX; ++j)
                {
                    *tmpPtr = 0;                            // init to 0 before accumulation

                    for (k = kSizeX - 1, m = 0; k >= kOffset; --k, ++m)   // convolve with partial of kernel
                    {
                        *tmpPtr += *(inPtr + m) * kernelX[k];
                    }
                    ++inPtr;                                // next input
                    ++tmpPtr;                               // next output
                    ++kOffset;                              // increase ending index of partial kernel
                }

                inPtr += kCenter;                           // next row
            }

        }


        private void bTest_Click(object sender, EventArgs e)
        {
            int KernalSize = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value == null || dataGridView1.Rows[i].Cells[0].Value.ToString() == "")
                {
                    KernalSize = i;
                    break;
                }
            }
            double[,] Kernal = new double[KernalSize, KernalSize];

            for (int i = 0; i < Kernal.GetLength(0); i++)
                for (int j = 0; j < Kernal.GetLength(1); j++)
                {
                    string value = dataGridView1.Rows[j].Cells[i].Value.ToString();
                    double item = 0;
                    double.TryParse(value, out item);
                    Kernal[i, j] = item;
                }
            mFilterToken[0] = KernalSize;
            mFilterToken[1] = Kernal;
            DoRun();
        }

    }
}