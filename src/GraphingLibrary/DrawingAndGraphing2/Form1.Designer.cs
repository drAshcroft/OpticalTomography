namespace MathHelpLib.DrawingAndGraphing
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.changeGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scatterGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dContourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dMultiAngleViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dIsoSurfaceViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeGraphToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(284, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // changeGraphToolStripMenuItem
            // 
            this.changeGraphToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lineGraphToolStripMenuItem,
            this.scatterGraphToolStripMenuItem,
            this.barGraphToolStripMenuItem,
            this.dContourToolStripMenuItem,
            this.imageViewerToolStripMenuItem,
            this.dMultiAngleViewerToolStripMenuItem,
            this.dIsoSurfaceViewerToolStripMenuItem});
            this.changeGraphToolStripMenuItem.Name = "changeGraphToolStripMenuItem";
            this.changeGraphToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.changeGraphToolStripMenuItem.Text = "Change Graph";
            // 
            // lineGraphToolStripMenuItem
            // 
            this.lineGraphToolStripMenuItem.Name = "lineGraphToolStripMenuItem";
            this.lineGraphToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.lineGraphToolStripMenuItem.Text = "Line Graph";
            // 
            // scatterGraphToolStripMenuItem
            // 
            this.scatterGraphToolStripMenuItem.Name = "scatterGraphToolStripMenuItem";
            this.scatterGraphToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.scatterGraphToolStripMenuItem.Text = "Scatter Graph";
            // 
            // barGraphToolStripMenuItem
            // 
            this.barGraphToolStripMenuItem.Name = "barGraphToolStripMenuItem";
            this.barGraphToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.barGraphToolStripMenuItem.Text = "Bar Graph";
            // 
            // dContourToolStripMenuItem
            // 
            this.dContourToolStripMenuItem.Name = "dContourToolStripMenuItem";
            this.dContourToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.dContourToolStripMenuItem.Text = "2D Contour";
            // 
            // imageViewerToolStripMenuItem
            // 
            this.imageViewerToolStripMenuItem.Name = "imageViewerToolStripMenuItem";
            this.imageViewerToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.imageViewerToolStripMenuItem.Text = "image Viewer";
            // 
            // dMultiAngleViewerToolStripMenuItem
            // 
            this.dMultiAngleViewerToolStripMenuItem.Name = "dMultiAngleViewerToolStripMenuItem";
            this.dMultiAngleViewerToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.dMultiAngleViewerToolStripMenuItem.Text = "3D Multi-Angle Viewer";
            // 
            // dIsoSurfaceViewerToolStripMenuItem
            // 
            this.dIsoSurfaceViewerToolStripMenuItem.Name = "dIsoSurfaceViewerToolStripMenuItem";
            this.dIsoSurfaceViewerToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.dIsoSurfaceViewerToolStripMenuItem.Text = "3D Iso Surface Viewer";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem changeGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lineGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scatterGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem barGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dContourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageViewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dMultiAngleViewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dIsoSurfaceViewerToolStripMenuItem;
    }
}