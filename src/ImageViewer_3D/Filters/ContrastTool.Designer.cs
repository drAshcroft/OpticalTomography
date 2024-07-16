namespace ImageViewer.Filters
{
    partial class ContrastTool
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
            this.sContrast = new MediaSlider.MediaSlider();
            this.sBrightness = new MediaSlider.MediaSlider();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // sContrast
            // 
            this.sContrast.Animated = false;
            this.sContrast.AnimationSize = 0.2F;
            this.sContrast.AnimationSpeed = MediaSlider.MediaSlider.AnimateSpeed.Normal;
            this.sContrast.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.sContrast.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.sContrast.AutoSize = true;
            this.sContrast.BackColor = System.Drawing.Color.Black;
            this.sContrast.BackgroundImage = null;
            this.sContrast.ButtonAccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.sContrast.ButtonBorderColor = System.Drawing.Color.Black;
            this.sContrast.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.sContrast.ButtonCornerRadius = ((uint)(2u));
            this.sContrast.ButtonSize = new System.Drawing.Size(12, 24);
            this.sContrast.ButtonStyle = MediaSlider.MediaSlider.ButtonType.GlassOverlap;
            this.sContrast.ContextMenuStrip = null;
            this.sContrast.LargeChange = 2;
            this.sContrast.Location = new System.Drawing.Point(446, 99);
            this.sContrast.Margin = new System.Windows.Forms.Padding(0);
            this.sContrast.Maximum = 50;
            this.sContrast.Minimum = -50;
            this.sContrast.Name = "sContrast";
            this.sContrast.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.sContrast.ShowButtonOnHover = false;
            this.sContrast.Size = new System.Drawing.Size(289, 30);
            this.sContrast.SliderFlyOut = MediaSlider.MediaSlider.FlyOutStyle.None;
            this.sContrast.SmallChange = 1;
            this.sContrast.SmoothScrolling = false;
            this.sContrast.TabIndex = 21;
            this.sContrast.TickColor = System.Drawing.Color.DarkGray;
            this.sContrast.TickStyle = System.Windows.Forms.TickStyle.BottomRight;
            this.sContrast.TickType = MediaSlider.MediaSlider.TickMode.Composite;
            this.sContrast.TrackBorderColor = System.Drawing.Color.White;
            this.sContrast.TrackDepth = 6;
            this.sContrast.TrackFillColor = System.Drawing.Color.Transparent;
            this.sContrast.TrackProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(101)))), ((int)(((byte)(188)))));
            this.sContrast.TrackShadow = false;
            this.sContrast.TrackShadowColor = System.Drawing.Color.DarkGray;
            this.sContrast.TrackStyle = MediaSlider.MediaSlider.TrackType.Progress;
            this.sContrast.Value = 0;
            this.sContrast.ValueChanged += new MediaSlider.MediaSlider.ValueChangedDelegate(this.sContrast_ValueChanged);
            // 
            // sBrightness
            // 
            this.sBrightness.Animated = false;
            this.sBrightness.AnimationSize = 0.2F;
            this.sBrightness.AnimationSpeed = MediaSlider.MediaSlider.AnimateSpeed.Normal;
            this.sBrightness.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.sBrightness.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.sBrightness.AutoSize = true;
            this.sBrightness.BackColor = System.Drawing.Color.Black;
            this.sBrightness.BackgroundImage = null;
            this.sBrightness.ButtonAccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.sBrightness.ButtonBorderColor = System.Drawing.Color.Black;
            this.sBrightness.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.sBrightness.ButtonCornerRadius = ((uint)(2u));
            this.sBrightness.ButtonSize = new System.Drawing.Size(12, 24);
            this.sBrightness.ButtonStyle = MediaSlider.MediaSlider.ButtonType.GlassOverlap;
            this.sBrightness.ContextMenuStrip = null;
            this.sBrightness.LargeChange = 2;
            this.sBrightness.Location = new System.Drawing.Point(446, 30);
            this.sBrightness.Margin = new System.Windows.Forms.Padding(0);
            this.sBrightness.Maximum = 50;
            this.sBrightness.Minimum = -50;
            this.sBrightness.Name = "sBrightness";
            this.sBrightness.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.sBrightness.ShowButtonOnHover = false;
            this.sBrightness.Size = new System.Drawing.Size(289, 30);
            this.sBrightness.SliderFlyOut = MediaSlider.MediaSlider.FlyOutStyle.None;
            this.sBrightness.SmallChange = 1;
            this.sBrightness.SmoothScrolling = false;
            this.sBrightness.TabIndex = 22;
            this.sBrightness.TickColor = System.Drawing.Color.DarkGray;
            this.sBrightness.TickStyle = System.Windows.Forms.TickStyle.BottomRight;
            this.sBrightness.TickType = MediaSlider.MediaSlider.TickMode.Composite;
            this.sBrightness.TrackBorderColor = System.Drawing.Color.White;
            this.sBrightness.TrackDepth = 6;
            this.sBrightness.TrackFillColor = System.Drawing.Color.Transparent;
            this.sBrightness.TrackProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(101)))), ((int)(((byte)(188)))));
            this.sBrightness.TrackShadow = false;
            this.sBrightness.TrackShadowColor = System.Drawing.Color.DarkGray;
            this.sBrightness.TrackStyle = MediaSlider.MediaSlider.TrackType.Progress;
            this.sBrightness.Value = 0;
            this.sBrightness.ValueChanged += new MediaSlider.MediaSlider.ValueChangedDelegate(this.sBrightness_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(464, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Brightness";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(464, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Contrast";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(668, 413);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(67, 35);
            this.button1.TabIndex = 25;
            this.button1.Text = "Done";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(427, 427);
            this.pictureBox1.TabIndex = 26;
            this.pictureBox1.TabStop = false;
            // 
            // ContrastTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(744, 457);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sBrightness);
            this.Controls.Add(this.sContrast);
            this.Name = "ContrastTool";
            this.Text = "ContrastTool";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MediaSlider.MediaSlider sContrast;
        private MediaSlider.MediaSlider sBrightness;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}