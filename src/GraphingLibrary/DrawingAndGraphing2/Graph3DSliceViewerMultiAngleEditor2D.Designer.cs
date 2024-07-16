namespace MathHelpLib.DrawingAndGraphing
{
    partial class Graph3DSliceViewerMultiAngleEditor2D
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
            this.viewerControl1 = new ImageViewer.ViewerControl();
            this.SuspendLayout();
            // 
            // viewerControl1
            // 
            this.viewerControl1.ActiveDrawingTool = null;
            this.viewerControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.viewerControl1.AutoScroll = true;
            this.viewerControl1.DrawingToolsVisible = true;
            this.viewerControl1.ExtraControl = null;
            this.viewerControl1.Location = new System.Drawing.Point(0, 34);
            this.viewerControl1.Name = "viewerControl1";
            this.viewerControl1.ProportionalZooming = true;
            this.viewerControl1.Size = new System.Drawing.Size(821, 654);
            this.viewerControl1.TabIndex = 0;
            this.viewerControl1.ZoomInterpolationMethod = System.Drawing.Drawing2D.InterpolationMode.Default;
            // 
            // Graph3DSliceViewerMultiAngleEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.viewerControl1);
            this.Name = "Graph3DSliceViewerMultiAngleEditor";
            this.Size = new System.Drawing.Size(824, 691);
            this.Resize += new System.EventHandler(this.Graph3DSliceViewerMultiAngle_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private global::ImageViewer.ViewerControl viewerControl1;

    }

}