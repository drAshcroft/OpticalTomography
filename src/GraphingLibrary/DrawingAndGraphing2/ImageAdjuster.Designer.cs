namespace MathHelpLib.DrawingAndGraphing
{
    partial class ImageAdjuster
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
            this.histogram1 = new AForge.Controls.Histogram();
            this.colorSlider1 = new AForge.Controls.ColorSlider();
            this.SuspendLayout();
            // 
            // histogram1
            // 
            this.histogram1.Location = new System.Drawing.Point(3, 3);
            this.histogram1.Name = "histogram1";
            this.histogram1.Size = new System.Drawing.Size(204, 184);
            this.histogram1.TabIndex = 0;
            this.histogram1.Text = "histogram1";
            this.histogram1.Values = null;
            // 
            // colorSlider1
            // 
            this.colorSlider1.Location = new System.Drawing.Point(3, 193);
            this.colorSlider1.Name = "colorSlider1";
            this.colorSlider1.Size = new System.Drawing.Size(204, 23);
            this.colorSlider1.TabIndex = 3;
            this.colorSlider1.Text = "LUTMaxMin";
            this.colorSlider1.Type = AForge.Controls.ColorSlider.ColorSliderType.InnerGradient;
            // 
            // ImageAdjuster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.colorSlider1);
            this.Controls.Add(this.histogram1);
            this.Name = "ImageAdjuster";
            this.Size = new System.Drawing.Size(403, 387);
            this.ResumeLayout(false);

        }

        #endregion

        private AForge.Controls.Histogram histogram1;
        private AForge.Controls.ColorSlider colorSlider1;
    }
}
