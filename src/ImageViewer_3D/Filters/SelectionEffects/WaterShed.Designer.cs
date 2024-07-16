namespace ImageViewer.Filters.SelectionEffects
{
    partial class WaterShedTool
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
            this.sHue = new MediaSlider.MediaSlider();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // sHue
            // 
            this.sHue.Animated = false;
            this.sHue.AnimationSize = 0.2F;
            this.sHue.AnimationSpeed = MediaSlider.MediaSlider.AnimateSpeed.Normal;
            this.sHue.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.sHue.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.sHue.AutoSize = true;
            this.sHue.BackColor = System.Drawing.Color.Black;
            this.sHue.BackgroundImage = null;
            this.sHue.ButtonAccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.sHue.ButtonBorderColor = System.Drawing.Color.Black;
            this.sHue.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.sHue.ButtonCornerRadius = ((uint)(2u));
            this.sHue.ButtonSize = new System.Drawing.Size(12, 24);
            this.sHue.ButtonStyle = MediaSlider.MediaSlider.ButtonType.GlassOverlap;
            this.sHue.ContextMenuStrip = null;
            this.sHue.LargeChange = 2;
            this.sHue.Location = new System.Drawing.Point(446, 30);
            this.sHue.Margin = new System.Windows.Forms.Padding(0);
            this.sHue.Maximum = 255;
            this.sHue.Minimum = 0;
            this.sHue.Name = "sHue";
            this.sHue.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.sHue.ShowButtonOnHover = false;
            this.sHue.Size = new System.Drawing.Size(289, 30);
            this.sHue.SliderFlyOut = MediaSlider.MediaSlider.FlyOutStyle.None;
            this.sHue.SmallChange = 1;
            this.sHue.SmoothScrolling = false;
            this.sHue.TabIndex = 22;
            this.sHue.TickColor = System.Drawing.Color.DarkGray;
            this.sHue.TickStyle = System.Windows.Forms.TickStyle.BottomRight;
            this.sHue.TickType = MediaSlider.MediaSlider.TickMode.Composite;
            this.sHue.TrackBorderColor = System.Drawing.Color.White;
            this.sHue.TrackDepth = 6;
            this.sHue.TrackFillColor = System.Drawing.Color.Transparent;
            this.sHue.TrackProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(101)))), ((int)(((byte)(188)))));
            this.sHue.TrackShadow = false;
            this.sHue.TrackShadowColor = System.Drawing.Color.DarkGray;
            this.sHue.TrackStyle = MediaSlider.MediaSlider.TrackType.Progress;
            this.sHue.Value = 125;
            this.sHue.ValueChanged += new MediaSlider.MediaSlider.ValueChangedDelegate(this.sBrightness_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(464, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Threshold";
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
            // ThresholdTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(744, 457);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sHue);
            this.Name = "ThresholdTool";
            this.Text = "ContrastTool";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MediaSlider.MediaSlider sHue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}