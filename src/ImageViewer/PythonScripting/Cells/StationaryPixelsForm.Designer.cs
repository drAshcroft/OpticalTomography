namespace ImageViewer.Filters
{
    partial class StationaryPixelsForm
    {

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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbMask = new System.Windows.Forms.RadioButton();
            this.rbStrips = new System.Windows.Forms.RadioButton();
            this.rbTopAndbottom = new System.Windows.Forms.RadioButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbDivideOne = new System.Windows.Forms.RadioButton();
            this.rbDivide = new System.Windows.Forms.RadioButton();
            this.rbSubtraction = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.zedgraphcontrol = new ZedGraph.ZedGraphControl();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(976, 539);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(67, 35);
            this.button1.TabIndex = 25;
            this.button1.Text = "Done";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(399, 273);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 26;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.rbMask);
            this.groupBox1.Controls.Add(this.rbStrips);
            this.groupBox1.Controls.Add(this.rbTopAndbottom);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(3, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(217, 121);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Moving Pixel Method";
            // 
            // rbMask
            // 
            this.rbMask.AutoSize = true;
            this.rbMask.Location = new System.Drawing.Point(11, 88);
            this.rbMask.Name = "rbMask";
            this.rbMask.Size = new System.Drawing.Size(51, 17);
            this.rbMask.TabIndex = 2;
            this.rbMask.TabStop = true;
            this.rbMask.Text = "Mask";
            this.rbMask.UseVisualStyleBackColor = true;
            this.rbMask.CheckedChanged += new System.EventHandler(this.rbTrigX_CheckedChanged);
            // 
            // rbStrips
            // 
            this.rbStrips.AutoSize = true;
            this.rbStrips.Location = new System.Drawing.Point(11, 51);
            this.rbStrips.Name = "rbStrips";
            this.rbStrips.Size = new System.Drawing.Size(51, 17);
            this.rbStrips.TabIndex = 1;
            this.rbStrips.TabStop = true;
            this.rbStrips.Text = "Strips";
            this.rbStrips.UseVisualStyleBackColor = true;
            this.rbStrips.CheckedChanged += new System.EventHandler(this.rbPolynomialX_CheckedChanged);
            // 
            // rbTopAndbottom
            // 
            this.rbTopAndbottom.AutoSize = true;
            this.rbTopAndbottom.Checked = true;
            this.rbTopAndbottom.Location = new System.Drawing.Point(11, 19);
            this.rbTopAndbottom.Name = "rbTopAndbottom";
            this.rbTopAndbottom.Size = new System.Drawing.Size(101, 17);
            this.rbTopAndbottom.TabIndex = 0;
            this.rbTopAndbottom.TabStop = true;
            this.rbTopAndbottom.Text = "Top And bottom";
            this.rbTopAndbottom.UseVisualStyleBackColor = true;
            this.rbTopAndbottom.CheckedChanged += new System.EventHandler(this.rbMovingAverageX_CheckedChanged);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.trackBar1);
            this.panel1.Location = new System.Drawing.Point(430, 326);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(604, 207);
            this.panel1.TabIndex = 6;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(487, 107);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(74, 35);
            this.button2.TabIndex = 7;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.rbDivideOne);
            this.groupBox2.Controls.Add(this.rbDivide);
            this.groupBox2.Controls.Add(this.rbSubtraction);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(235, 66);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(217, 121);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Background Subtracktion";
            // 
            // rbDivideOne
            // 
            this.rbDivideOne.AutoSize = true;
            this.rbDivideOne.Location = new System.Drawing.Point(11, 88);
            this.rbDivideOne.Name = "rbDivideOne";
            this.rbDivideOne.Size = new System.Drawing.Size(67, 17);
            this.rbDivideOne.TabIndex = 2;
            this.rbDivideOne.Text = "1+Divide";
            this.rbDivideOne.UseVisualStyleBackColor = true;
            this.rbDivideOne.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // rbDivide
            // 
            this.rbDivide.AutoSize = true;
            this.rbDivide.Checked = true;
            this.rbDivide.Location = new System.Drawing.Point(11, 51);
            this.rbDivide.Name = "rbDivide";
            this.rbDivide.Size = new System.Drawing.Size(55, 17);
            this.rbDivide.TabIndex = 1;
            this.rbDivide.TabStop = true;
            this.rbDivide.Text = "Divide";
            this.rbDivide.UseVisualStyleBackColor = true;
            this.rbDivide.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // rbSubtraction
            // 
            this.rbSubtraction.AutoSize = true;
            this.rbSubtraction.Location = new System.Drawing.Point(11, 19);
            this.rbSubtraction.Name = "rbSubtraction";
            this.rbSubtraction.Size = new System.Drawing.Size(65, 17);
            this.rbSubtraction.TabIndex = 0;
            this.rbSubtraction.Text = "Subtract";
            this.rbSubtraction.UseVisualStyleBackColor = true;
            this.rbSubtraction.CheckedChanged += new System.EventHandler(this.rbSubtraction_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(18, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Background Strength";
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.Location = new System.Drawing.Point(21, 31);
            this.trackBar1.Maximum = 500;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(541, 45);
            this.trackBar1.TabIndex = 3;
            this.trackBar1.Value = 100;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // zedgraphcontrol
            // 
            this.zedgraphcontrol.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zedgraphcontrol.Location = new System.Drawing.Point(430, 12);
            this.zedgraphcontrol.Name = "zedgraphcontrol";
            this.zedgraphcontrol.ScrollGrace = 0;
            this.zedgraphcontrol.ScrollMaxX = 0;
            this.zedgraphcontrol.ScrollMaxY = 0;
            this.zedgraphcontrol.ScrollMaxY2 = 0;
            this.zedgraphcontrol.ScrollMinX = 0;
            this.zedgraphcontrol.ScrollMinY = 0;
            this.zedgraphcontrol.ScrollMinY2 = 0;
            this.zedgraphcontrol.Size = new System.Drawing.Size(601, 308);
            this.zedgraphcontrol.TabIndex = 1;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(12, 294);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(399, 273);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 27;
            this.pictureBox2.TabStop = false;
            // 
            // StationaryPixelsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1055, 586);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.zedgraphcontrol);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button1);
            this.Name = "StationaryPixelsForm";
            this.Text = "Stationary Pixels";
            this.Resize += new System.EventHandler(this.CenterCellsTool2Form_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Button button1;
        protected System.Windows.Forms.PictureBox pictureBox1;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.RadioButton rbStrips;
        private System.Windows.Forms.RadioButton rbTopAndbottom;
        private System.Windows.Forms.RadioButton rbMask;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBar1;
        private ZedGraph.ZedGraphControl zedgraphcontrol;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbDivideOne;
        private System.Windows.Forms.RadioButton rbDivide;
        private System.Windows.Forms.RadioButton rbSubtraction;
        protected System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button button2;
    }
}