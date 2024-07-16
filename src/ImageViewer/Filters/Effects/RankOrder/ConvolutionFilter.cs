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
using MathHelpLib.Convolution;
using MathHelpLib;

namespace ImageViewer.Filters.Effects.RankOrder
{
    public partial class ConvolutionFilterTool : aEffectForm
    {

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bTest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pPreview;
        private System.Windows.Forms.Button blocalFinished;

        public ConvolutionFilterTool()
            : base()
        {
            pInitializeComponent();
            splitContainer1.Visible = false;
            button1.Visible = false;

            dataGridView1.RowCount = 11;
            dataGridView1.ColumnCount = 11;

            blocalFinished.Click += new EventHandler(blocalFinished_Click);
            bTest.Click += new EventHandler(bTest_Click);
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


        public override string EffectName { get { return "Convolution Filter"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Rank Order Statistical"; } }
        public override int OrderSuggestion { get { return 1; } }

       

        public override object[] DefaultProperties
        {
            get { return new object[] { 3, new double[3, 3] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } } }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "Kernal Size Radius|int","Kernal|double[,]" }; }
        }

        protected override object doEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            if (mFilterToken == null)
            {
                mFilterToken = DefaultProperties;
            }

            mDataEnvironment = dataEnvironment;

            double[,] Kernal = (double[,])mFilterToken[1];
            for (int i = 0; i < Kernal.GetLength(0); i++)
                for (int j = 0; j < Kernal.GetLength(1); j++)
                    dataGridView1.Rows[j].Cells[i].Value = Kernal[i, j];

            int value = (int)mFilterToken[0];
            if (value < 3) value = 3;
            if (value % 2 == 0) value++;
            mFilterToken[0] = (int)value;
           
            ValueArrayKernal vak = new ValueArrayKernal((double[,])mFilterToken[1]);

            if (SourceImage.GetType() == typeof(Bitmap))
            {
                return ConvolutionFilterImplented.ConvolutionFilter((Bitmap)SourceImage, vak);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                return ConvolutionFilterImplented.ConvolutionFilter((ImageHolder)SourceImage, vak);
            }
           
            return null;
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
