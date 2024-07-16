namespace GraphingLib.DrawingAndGraphing
{
    partial class Graph3DSliceViewerMultiAngleEditor
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
            this.viewerControl3D1 = new ImageViewer3D.ViewerControl3D();
            this.SuspendLayout();
            // 
            // viewerControl3D1
            // 
            this.viewerControl3D1.ActiveDrawingTool = null;
            this.viewerControl3D1.AutoScroll = true;
            this.viewerControl3D1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewerControl3D1.DrawingToolsVisible = true;
            this.viewerControl3D1.ExtraControl = null;
            this.viewerControl3D1.Location = new System.Drawing.Point(0, 0);
            this.viewerControl3D1.Name = "viewerControl3D1";
            this.viewerControl3D1.ProportionalZooming = true;
            this.viewerControl3D1.Size = new System.Drawing.Size(828, 691);
            this.viewerControl3D1.SliceIndexX = 0;
            this.viewerControl3D1.SliceIndexY = 0;
            this.viewerControl3D1.SliceIndexZ = 0;
            this.viewerControl3D1.TabIndex = 0;
            this.viewerControl3D1.ZoomInterpolationMethod = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.viewerControl3D1.Resize += new System.EventHandler(this.viewerControl3D1_Resize);
            // 
            // Graph3DSliceViewerMultiAngleEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.viewerControl3D1);
            this.Name = "Graph3DSliceViewerMultiAngleEditor";
            this.Size = new System.Drawing.Size(828, 691);
            this.ResumeLayout(false);

        }

        #endregion

        private ImageViewer3D.ViewerControl3D viewerControl3D1;


    }

}