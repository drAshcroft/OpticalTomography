namespace IVGViewer
{
    partial class IVG_Viewr
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
            this.viewerControl3D1 = new ImageViewer3D.ViewerControl3D();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tDirectorySearch = new System.Windows.Forms.TrackBar();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tDirectorySearch)).BeginInit();
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
            this.viewerControl1.Location = new System.Drawing.Point(4, 28);
            this.viewerControl1.Name = "viewerControl1";
            this.viewerControl1.ProportionalZooming = true;
            this.viewerControl1.SelectedArea = null;
            this.viewerControl1.Size = new System.Drawing.Size(1187, 829);
            this.viewerControl1.TabIndex = 0;
            this.viewerControl1.ZoomInterpolationMethod = System.Drawing.Drawing2D.InterpolationMode.Default;
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
            this.viewerControl3D1.SelectedArea = null;
            this.viewerControl3D1.Size = new System.Drawing.Size(1192, 858);
            this.viewerControl3D1.SliceIndexX = 0;
            this.viewerControl3D1.SliceIndexY = 0;
            this.viewerControl3D1.SliceIndexZ = 0;
            this.viewerControl3D1.TabIndex = 1;
            this.viewerControl3D1.Visible = false;
            this.viewerControl3D1.Zooming = false;
            this.viewerControl3D1.ZoomInterpolationMethod = System.Drawing.Drawing2D.InterpolationMode.Default;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openDirectoryToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1192, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openDirectoryToolStripMenuItem
            // 
            this.openDirectoryToolStripMenuItem.Name = "openDirectoryToolStripMenuItem";
            this.openDirectoryToolStripMenuItem.Size = new System.Drawing.Size(99, 20);
            this.openDirectoryToolStripMenuItem.Text = "Open Directory";
            this.openDirectoryToolStripMenuItem.Click += new System.EventHandler(this.openDirectoryToolStripMenuItem_Click);
            // 
            // tDirectorySearch
            // 
            this.tDirectorySearch.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tDirectorySearch.Location = new System.Drawing.Point(0, 813);
            this.tDirectorySearch.Name = "tDirectorySearch";
            this.tDirectorySearch.Size = new System.Drawing.Size(1192, 45);
            this.tDirectorySearch.TabIndex = 3;
            this.tDirectorySearch.Visible = false;
            this.tDirectorySearch.ValueChanged += new System.EventHandler(this.tDirectorySearch_ValueChanged);
            // 
            // IVG_Viewr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1192, 858);
            this.Controls.Add(this.tDirectorySearch);
            this.Controls.Add(this.viewerControl1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.viewerControl3D1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "IVG_Viewr";
            this.Text = "IVG Viewer";
            this.Load += new System.EventHandler(this.IVG_Viewr_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tDirectorySearch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ImageViewer.ViewerControl viewerControl1;
        private ImageViewer3D.ViewerControl3D viewerControl3D1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openDirectoryToolStripMenuItem;
        private System.Windows.Forms.TrackBar tDirectorySearch;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}

