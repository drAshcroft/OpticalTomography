namespace Tomographic_Imaging_2.ImageManipulation
{
    partial class EasyImageViewer
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
            this.viewerControl1 = new ImageViewer.ViewerControl();
            this.SuspendLayout();
            // 
            // viewerControl1
            // 
            this.viewerControl1.ActiveDrawingTool = null;
            this.viewerControl1.AutoScroll = true;
            this.viewerControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.viewerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewerControl1.DrawingToolsVisible = true;
            this.viewerControl1.ExtraControl = null;
            this.viewerControl1.Location = new System.Drawing.Point(0, 0);
            this.viewerControl1.Name = "viewerControl1";
            this.viewerControl1.ProportionalZooming = true;
            this.viewerControl1.Size = new System.Drawing.Size(769, 575);
            this.viewerControl1.TabIndex = 0;
            this.viewerControl1.ZoomInterpolationMethod = System.Drawing.Drawing2D.InterpolationMode.Default;
            // 
            // ImageViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 575);
            this.Controls.Add(this.viewerControl1);
            this.Name = "ImageViewer";
            this.Text = "ImageViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private global::ImageViewer.ViewerControl viewerControl1;
    }
}