using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using MathHelpLib;

namespace ImageViewer.Filters.Files
{
    public partial class NewImage : aEffectForm
    {
        #region Form Setup
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bDone;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.NumericUpDown nWidth;
        private System.Windows.Forms.NumericUpDown nHeight;

        public NewImage()
            : base()
        {
            splitContainer1.Visible = false;
            button1.Visible = false;

            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bDone = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.nWidth = new System.Windows.Forms.NumericUpDown();
            this.nHeight = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Width";
            this.label1.ForeColor = Color.White;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Height";
            this.label2.ForeColor = Color.White;
            // 
            // bDone
            // 
            this.bDone.Location = new System.Drawing.Point(129, 14);
            this.bDone.Name = "bDone";
            this.bDone.Size = new System.Drawing.Size(75, 23);
            this.bDone.TabIndex = 4;
            this.bDone.Text = "Done";
            this.bDone.UseVisualStyleBackColor = true;
            this.bDone.Click += new System.EventHandler(this.bDone_Click);
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(129, 43);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 5;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // nWidth
            // 
            this.nWidth.Location = new System.Drawing.Point(3, 17);
            this.nWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nWidth.Name = "nWidth";
            this.nWidth.Size = new System.Drawing.Size(120, 20);
            this.nWidth.TabIndex = 6;
            this.nWidth.Value = new decimal(new int[] {
            640,
            0,
            0,
            0});
            // 
            // nHeight
            // 
            this.nHeight.Location = new System.Drawing.Point(3, 55);
            this.nHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nHeight.Name = "nHeight";
            this.nHeight.Size = new System.Drawing.Size(120, 20);
            this.nHeight.TabIndex = 7;
            this.nHeight.Value = new decimal(new int[] {
            480,
            0,
            0,
            0});
            // 
            // NewImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 91);
            this.Controls.Add(this.nHeight);
            this.Controls.Add(this.nWidth);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bDone);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "NewImage";
            this.Text = "NewImage";
            ((System.ComponentModel.ISupportInitialize)(this.nWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        public override string EffectName { get { return "New image"; } }
        public override string EffectMenu { get { return "File"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion
        {
            get
            {
                return 1;
            }
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { 640, 480 }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "Width|int", "Height|int" }; }
        }

        protected override object doEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            //if the image is different, all the screens will be updated
            return new ImageHolder((int)Parameters[0], (int)Parameters[1]);
        }


        private void bDone_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }
        private void bCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
