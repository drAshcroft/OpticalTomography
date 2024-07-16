namespace Tomographic_Imaging_2
{
    partial class ImageSelector
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
            this.label1 = new System.Windows.Forms.Label();
            this.tFileSelector = new System.Windows.Forms.TrackBar();
            this.bUpdate = new System.Windows.Forms.Button();
            this.bClose = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.viewerControl1 = new ImageViewer.ViewerControl();
            this.label3 = new System.Windows.Forms.Label();
            this.cShowStatistics = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.tFileSelector)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-3, 529);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "File";
            // 
            // tFileSelector
            // 
            this.tFileSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tFileSelector.Location = new System.Drawing.Point(0, 545);
            this.tFileSelector.Name = "tFileSelector";
            this.tFileSelector.Size = new System.Drawing.Size(622, 45);
            this.tFileSelector.TabIndex = 3;
            this.tFileSelector.Visible = false;
            this.tFileSelector.ValueChanged += new System.EventHandler(this.tFileSelector_ValueChanged);
            // 
            // bUpdate
            // 
            this.bUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bUpdate.Location = new System.Drawing.Point(628, 547);
            this.bUpdate.Name = "bUpdate";
            this.bUpdate.Size = new System.Drawing.Size(110, 30);
            this.bUpdate.TabIndex = 4;
            this.bUpdate.Text = "Update";
            this.bUpdate.UseVisualStyleBackColor = true;
            this.bUpdate.Click += new System.EventHandler(this.bUpdate_Click);
            // 
            // bClose
            // 
            this.bClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bClose.Location = new System.Drawing.Point(744, 548);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(110, 30);
            this.bClose.TabIndex = 5;
            this.bClose.Text = "Close";
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label2.Location = new System.Drawing.Point(26, 499);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(840, 43);
            this.label2.TabIndex = 7;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 580);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(912, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // viewerControl1
            // 
            this.viewerControl1.ActiveDrawingTool = null;
            this.viewerControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.viewerControl1.AutoScroll = true;
            this.viewerControl1.DrawingToolsVisible = true;
            this.viewerControl1.Location = new System.Drawing.Point(0, 0);
            this.viewerControl1.Name = "viewerControl1";
            this.viewerControl1.ProportionalZooming = true;
            this.viewerControl1.Size = new System.Drawing.Size(911, 480);
            this.viewerControl1.TabIndex = 6;
            this.viewerControl1.ZoomInterpolationMethod = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.viewerControl1.SelectionPerformed += new ImageViewer.SelectionPerformedEvent(this.viewerControl1_SelectionPerformed);
            this.viewerControl1.MouseMoving += new System.Windows.Forms.MouseEventHandler(this.viewerControl1_MouseMoving);
            this.viewerControl1.ToolTipUpdate += new ImageViewer.ToolTipUpdateEvent(this.viewerControl1_ToolTipUpdate);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 483);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "(0,0)";
            // 
            // cShowStatistics
            // 
            this.cShowStatistics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cShowStatistics.AutoSize = true;
            this.cShowStatistics.Location = new System.Drawing.Point(82, 482);
            this.cShowStatistics.Name = "cShowStatistics";
            this.cShowStatistics.Size = new System.Drawing.Size(98, 17);
            this.cShowStatistics.TabIndex = 10;
            this.cShowStatistics.Text = "Show Statistics";
            this.cShowStatistics.UseVisualStyleBackColor = true;
            // 
            // ImageSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 602);
            this.Controls.Add(this.cShowStatistics);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bClose);
            this.Controls.Add(this.bUpdate);
            this.Controls.Add(this.tFileSelector);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.viewerControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ImageSelector";
            this.Text = "ImageSelector";
            this.ResizeEnd += new System.EventHandler(this.ImageSelector_ResizeEnd);
            ((System.ComponentModel.ISupportInitialize)(this.tFileSelector)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar tFileSelector;
        private System.Windows.Forms.Button bUpdate;
        private System.Windows.Forms.Button bClose;
        private ImageViewer.ViewerControl viewerControl1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cShowStatistics;
       
    }
}