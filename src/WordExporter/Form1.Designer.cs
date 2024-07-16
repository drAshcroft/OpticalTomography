namespace WordExporter
{
    partial class Form1
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
            this.lDataDirectories = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.viewerControl3D1 = new ImageViewer3D.ViewerControl3D();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lDataDirectories
            // 
            this.lDataDirectories.FormattingEnabled = true;
            this.lDataDirectories.Location = new System.Drawing.Point(45, 44);
            this.lDataDirectories.Name = "lDataDirectories";
            this.lDataDirectories.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lDataDirectories.Size = new System.Drawing.Size(282, 290);
            this.lDataDirectories.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(354, 51);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(111, 40);
            this.button1.TabIndex = 5;
            this.button1.Text = "Compare Experiments";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // viewerControl3D1
            // 
            this.viewerControl3D1.ActiveDrawingTool = null;
            this.viewerControl3D1.AutoScroll = true;
            this.viewerControl3D1.DrawingToolsVisible = true;
            this.viewerControl3D1.ExtraControl = null;
            this.viewerControl3D1.Location = new System.Drawing.Point(372, 122);
            this.viewerControl3D1.Name = "viewerControl3D1";
            this.viewerControl3D1.ProportionalZooming = true;
            this.viewerControl3D1.Size = new System.Drawing.Size(1236, 895);
            this.viewerControl3D1.SliceIndexX = 0;
            this.viewerControl3D1.SliceIndexY = 0;
            this.viewerControl3D1.SliceIndexZ = 0;
            this.viewerControl3D1.TabIndex = 6;
            this.viewerControl3D1.ZoomInterpolationMethod = System.Drawing.Drawing2D.InterpolationMode.Default;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(13, 377);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(307, 228);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(505, 51);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(111, 40);
            this.button2.TabIndex = 8;
            this.button2.Text = "Compare CCT";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1667, 1052);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.viewerControl3D1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lDataDirectories);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lDataDirectories;
        private System.Windows.Forms.Button button1;
        private ImageViewer3D.ViewerControl3D viewerControl3D1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button2;
    }
}

