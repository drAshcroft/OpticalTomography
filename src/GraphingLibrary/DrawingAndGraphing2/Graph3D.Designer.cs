namespace MathHelpLib.DrawingAndGraphing
{
    partial class Graph3D
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.bAddThresholder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cThreshold = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TopHatSelector = new System.Windows.Forms.PictureBox();
            this.graph3DThreshold1 = new MathHelpLib.DrawingAndGraphing.Graph3DThreshold();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TopHatSelector)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(375, 448);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.bAddThresholder);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.cThreshold);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.TopHatSelector);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer1.Size = new System.Drawing.Size(661, 448);
            this.splitContainer1.SplitterDistance = 282;
            this.splitContainer1.TabIndex = 1;
            // 
            // bAddThresholder
            // 
            this.bAddThresholder.Location = new System.Drawing.Point(177, 96);
            this.bAddThresholder.Name = "bAddThresholder";
            this.bAddThresholder.Size = new System.Drawing.Size(102, 23);
            this.bAddThresholder.TabIndex = 1;
            this.bAddThresholder.Text = "Add Thresholder";
            this.bAddThresholder.UseVisualStyleBackColor = true;
            this.bAddThresholder.Click += new System.EventHandler(this.bAddThresholder_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Thresholds";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(6, 121);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(273, 324);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.graph3DThreshold1);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(243, 249);
            this.panel2.TabIndex = 0;
            // 
            // cThreshold
            // 
            this.cThreshold.AutoSize = true;
            this.cThreshold.Location = new System.Drawing.Point(6, 8);
            this.cThreshold.Name = "cThreshold";
            this.cThreshold.Size = new System.Drawing.Size(105, 17);
            this.cThreshold.TabIndex = 2;
            this.cThreshold.Text = "Threshold image";
            this.cThreshold.UseVisualStyleBackColor = true;
            this.cThreshold.CheckedChanged += new System.EventHandler(this.cThreshold_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Intensity";
            // 
            // TopHatSelector
            // 
            this.TopHatSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TopHatSelector.BackColor = System.Drawing.Color.Black;
            this.TopHatSelector.Location = new System.Drawing.Point(3, 44);
            this.TopHatSelector.Name = "TopHatSelector";
            this.TopHatSelector.Size = new System.Drawing.Size(277, 50);
            this.TopHatSelector.TabIndex = 0;
            this.TopHatSelector.TabStop = false;
            this.TopHatSelector.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TopHatSelector_MouseMove);
            this.TopHatSelector.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TopHatSelector_MouseDown);
            this.TopHatSelector.Paint += new System.Windows.Forms.PaintEventHandler(this.TopHatSelector_Paint);
            this.TopHatSelector.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TopHatSelector_MouseUp);
            // 
            // graph3DThreshold1
            // 
            this.graph3DThreshold1.Location = new System.Drawing.Point(3, 3);
            this.graph3DThreshold1.Name = "graph3DThreshold1";
            this.graph3DThreshold1.Size = new System.Drawing.Size(237, 125);
            this.graph3DThreshold1.TabIndex = 0;
            this.graph3DThreshold1.ColorClicked += new System.EventHandler(this.graph3DThreshold1_ColorClicked);
            // 
            // Graph3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Graph3D";
            this.Size = new System.Drawing.Size(661, 448);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Graph3D_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TopHatSelector)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox TopHatSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cThreshold;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private Graph3DThreshold graph3DThreshold1;
        private System.Windows.Forms.Button bAddThresholder;
    }

}