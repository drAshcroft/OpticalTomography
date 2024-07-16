using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using MathHelpLib;

namespace ImageViewer.Filters.Effects.RankOrder
{
    public partial class Convolution1DFilterTool : aEffectForm
    {

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bTest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pPreview;
        private System.Windows.Forms.Button blocalFinished;

        public Convolution1DFilterTool()
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


        public override string EffectName { get { return "Convolution 1D Filter"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Rank Order Statistical"; } }
        public override int OrderSuggestion { get { return 1; } }


        public override object[] DefaultProperties
        {
            get { return new object[] { 0, new double[10] }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Axis|int(0 means X, 1 means Y)", "Kernal|double[]" }; }
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


            double[, ,] DataOut=null;
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                if ((int)mFilterToken[0] == 0)
                    DataOut = ConvoluteChopX(new ImageHolder((Bitmap)SourceImage), Kernal);
                else
                    DataOut =  ConvoluteChopY(new ImageHolder((Bitmap)SourceImage), Kernal);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                if ((int)mFilterToken[0] == 0)
                    DataOut = ConvoluteChopX((ImageHolder)SourceImage, Kernal);
                else
                    DataOut=  ConvoluteChopY((ImageHolder)SourceImage, Kernal);
            }

            mPassData.AddSafe("ConvolutionData", DataOut);

            if (mFilterToken.Length < 3 || (bool)mFilterToken[3] == false)
            {
                return new ImageHolder(DataOut);
            }
            else
                return SourceImage;
        }

        private double[,,] ConvoluteChopY(ImageHolder image, double[] Kernal)
        {
            double[, ,] DataOut = new double[image.Height, image.Width, image.NChannels];

            unsafe
            {
                fixed (float* pData = image.ImageData)
                {
                    fixed (double* pKernal = Kernal)
                    {
                        fixed (double* pDataOut = DataOut)
                        {
                            for (int i = 0; i < image.NChannels; i++)
                            {
                                for (int x = 0; x < image.Width ; x++)
                                {
                                    float* pIn = pData + x*image.NChannels +i;
                                    double* pOut = pDataOut + x * image.NChannels + i;
                                    ConvoluteChop(pIn, image.Height   , pKernal, Kernal.Length, pOut,image.Width * image.NChannels);

                                }
                            }
                        }
                    }
                }
            }

            return DataOut;
        }


        private double[,,] ConvoluteChopX(ImageHolder image, double[] Kernal)
        {
            double[, ,] DataOut = new double[image.Height, image.Width, image.NChannels];

            unsafe
            {
                fixed (float* pData = image.ImageData)
                {
                    fixed (double* pKernal = Kernal)
                    {
                        fixed (double* pDataOut = DataOut)
                        {
                            for (int i = 0; i < image.NChannels; i++)
                            {
                                for (int y = 0; y < image.Height; y++)
                                {
                                    float* pIn = pData + y * image.Width * image.NChannels + i;
                                    double* pOut = pDataOut + y * image.Width * image.NChannels + i;
                                    ConvoluteChop(pIn, image.Width, pKernal, Kernal.Length, pOut, image.NChannels);

                                }
                            }
                        }
                    }
                }
            }

            return DataOut;
        }



        private static unsafe void ConvoluteChop(float* Array1, int Length1, double* pImpulse, int LImpulse, double* pArrayOut, int Stride)
        {

            int LengthWhole = Length1 + LImpulse;

            int StartI = (int)Math.Truncate((double)LengthWhole / 2d - (double)Length1 / 2d);
            int EndI = (int)Math.Truncate((double)LengthWhole / 2d + (double)Length1 / 2d);
            if (EndI - StartI > Length1)
                EndI--;
            int sI, eI;

            double p1;
            double* p2;
            double* pOut;
            //double ValueOut = 0;
            for (int i = 0; i < Length1; i++)
            {
                p1 = Array1[i * Stride];
                sI = StartI - i;
                eI = EndI - i;
                if (eI > LImpulse) eI = LImpulse;
                if (sI < 0) sI = 0;
                if (sI < eI)
                {
                    p2 = pImpulse + sI;
                    pOut = pArrayOut;// +CurrentParticle + sI - StartI;
                    for (int j = sI; j < eI; j++)
                    {
                        *pOut += p1 * (*p2);
                        // ValueOut = *pOut;
                        //   System.Diagnostics.Debug.Print(ValueOut.ToString());
                        pOut += Stride;
                        p2++;
                    }
                    // System.Diagnostics.Debug.Print(ValueOut.ToString());
                    //  System.Diagnostics.Debug.Print("");
                }
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
