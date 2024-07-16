namespace DoRecons
{
    partial class TwoVolume
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.bColor1 = new System.Windows.Forms.Button();
            this.bColor2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tVol1 = new System.Windows.Forms.TrackBar();
            this.tVol2 = new System.Windows.Forms.TrackBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.bProduce = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.tVol1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tVol2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(959, 34);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Open Volumes";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bColor1
            // 
            this.bColor1.BackColor = System.Drawing.Color.Red;
            this.bColor1.Location = new System.Drawing.Point(968, 91);
            this.bColor1.Name = "bColor1";
            this.bColor1.Size = new System.Drawing.Size(47, 40);
            this.bColor1.TabIndex = 1;
            this.bColor1.UseVisualStyleBackColor = false;
            this.bColor1.Click += new System.EventHandler(this.bColor1_Click);
            // 
            // bColor2
            // 
            this.bColor2.BackColor = System.Drawing.Color.White;
            this.bColor2.Location = new System.Drawing.Point(968, 167);
            this.bColor2.Name = "bColor2";
            this.bColor2.Size = new System.Drawing.Size(47, 40);
            this.bColor2.TabIndex = 2;
            this.bColor2.UseVisualStyleBackColor = false;
            this.bColor2.Click += new System.EventHandler(this.bColor2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(965, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Color 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(965, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Color 2";
            // 
            // tVol1
            // 
            this.tVol1.Location = new System.Drawing.Point(407, 737);
            this.tVol1.Name = "tVol1";
            this.tVol1.Size = new System.Drawing.Size(627, 45);
            this.tVol1.TabIndex = 5;
            this.tVol1.Value = 10;
            this.tVol1.ValueChanged += new System.EventHandler(this.tVol1_ValueChanged);
            // 
            // tVol2
            // 
            this.tVol2.Location = new System.Drawing.Point(407, 834);
            this.tVol2.Name = "tVol2";
            this.tVol2.Size = new System.Drawing.Size(627, 45);
            this.tVol2.TabIndex = 6;
            this.tVol2.Value = 10;
            this.tVol2.ValueChanged += new System.EventHandler(this.tVol2_ValueChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(15, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(257, 264);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(278, 20);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(257, 264);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(15, 290);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(257, 264);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 9;
            this.pictureBox3.TabStop = false;
            // 
            // bProduce
            // 
            this.bProduce.Location = new System.Drawing.Point(959, 260);
            this.bProduce.Name = "bProduce";
            this.bProduce.Size = new System.Drawing.Size(75, 23);
            this.bProduce.TabIndex = 10;
            this.bProduce.Text = "Produce";
            this.bProduce.UseVisualStyleBackColor = true;
            this.bProduce.Click += new System.EventHandler(this.bProduce_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(407, 495);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(627, 45);
            this.trackBar1.TabIndex = 11;
            this.trackBar1.Value = 20;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // TwoVolume
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1130, 924);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.bProduce);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tVol2);
            this.Controls.Add(this.tVol1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bColor2);
            this.Controls.Add(this.bColor1);
            this.Controls.Add(this.button1);
            this.Name = "TwoVolume";
            this.Text = "TwoVolume";
            ((System.ComponentModel.ISupportInitialize)(this.tVol1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tVol2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button bColor1;
        private System.Windows.Forms.Button bColor2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar tVol1;
        private System.Windows.Forms.TrackBar tVol2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Button bProduce;
        private System.Windows.Forms.TrackBar trackBar1;
    }
}