namespace ImageTestor
{
    partial class Form3
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
            this.nextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.correctToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewerControl1 = new ImageViewer.ViewerControl();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nextToolStripMenuItem,
            this.correctToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(650, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // nextToolStripMenuItem
            // 
            this.nextToolStripMenuItem.Name = "nextToolStripMenuItem";
            this.nextToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.nextToolStripMenuItem.Text = "Next";
            this.nextToolStripMenuItem.Click += new System.EventHandler(this.nextToolStripMenuItem_Click);
            // 
            // correctToolStripMenuItem
            // 
            this.correctToolStripMenuItem.Name = "correctToolStripMenuItem";
            this.correctToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.correctToolStripMenuItem.Text = "Correct";
            this.correctToolStripMenuItem.Click += new System.EventHandler(this.correctToolStripMenuItem_Click);
            // 
            // viewerControl1
            // 
            this.viewerControl1.ActiveDrawingTool = null;
            this.viewerControl1.AutoScroll = true;
            this.viewerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewerControl1.DrawingToolsVisible = true;
            this.viewerControl1.ExtraControl = null;
            this.viewerControl1.Location = new System.Drawing.Point(0, 24);
            this.viewerControl1.Name = "viewerControl1";
            this.viewerControl1.ProportionalZooming = true;
            this.viewerControl1.Size = new System.Drawing.Size(650, 314);
            this.viewerControl1.TabIndex = 1;
            this.viewerControl1.ZoomInterpolationMethod = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.viewerControl1.SelectionPerformed += new ImageViewer.SelectionPerformedEvent(this.viewerControl1_SelectionPerformed);
            this.viewerControl1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.viewerControl1_KeyPress);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 338);
            this.Controls.Add(this.viewerControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form3";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form3_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem nextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem correctToolStripMenuItem;
        private ImageViewer.ViewerControl viewerControl1;
    }
}