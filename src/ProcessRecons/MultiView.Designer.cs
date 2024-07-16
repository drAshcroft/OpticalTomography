namespace ProcessRecons
{
    partial class MultiView
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
            this.lDataDirectories = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.bBrowse = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.numNumCols = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.bLoad = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.viewerControl3D1 = new ImageViewer3D.ViewerControl3D();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumCols)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lDataDirectories
            // 
            this.lDataDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lDataDirectories.FormattingEnabled = true;
            this.lDataDirectories.Location = new System.Drawing.Point(3, 31);
            this.lDataDirectories.Name = "lDataDirectories";
            this.lDataDirectories.ScrollAlwaysVisible = true;
            this.lDataDirectories.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lDataDirectories.Size = new System.Drawing.Size(290, 823);
            this.lDataDirectories.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(4, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(250, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // bBrowse
            // 
            this.bBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bBrowse.Location = new System.Drawing.Point(260, 3);
            this.bBrowse.Name = "bBrowse";
            this.bBrowse.Size = new System.Drawing.Size(33, 23);
            this.bBrowse.TabIndex = 2;
            this.bBrowse.Text = "...";
            this.bBrowse.UseVisualStyleBackColor = true;
            this.bBrowse.Click += new System.EventHandler(this.bBrowse_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1233, 954);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1233, 817);
            this.panel2.TabIndex = 0;
            // 
            // numNumCols
            // 
            this.numNumCols.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numNumCols.Location = new System.Drawing.Point(4, 931);
            this.numNumCols.Name = "numNumCols";
            this.numNumCols.Size = new System.Drawing.Size(140, 20);
            this.numNumCols.TabIndex = 4;
            this.numNumCols.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numNumCols.ValueChanged += new System.EventHandler(this.numNumCols_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 913);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Num Cols";
            // 
            // bLoad
            // 
            this.bLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bLoad.Location = new System.Drawing.Point(3, 876);
            this.bLoad.Name = "bLoad";
            this.bLoad.Size = new System.Drawing.Size(95, 28);
            this.bLoad.TabIndex = 6;
            this.bLoad.Text = "Load";
            this.bLoad.UseVisualStyleBackColor = true;
            this.bLoad.Click += new System.EventHandler(this.bLoad_Click);
            // 
            // viewerControl3D1
            // 
            this.viewerControl3D1.ActiveDrawingTool = null;
            this.viewerControl3D1.AutoScroll = true;
            this.viewerControl3D1.DrawingToolsVisible = true;
            this.viewerControl3D1.ExtraControl = null;
            this.viewerControl3D1.Location = new System.Drawing.Point(1450, 97);
            this.viewerControl3D1.Name = "viewerControl3D1";
            this.viewerControl3D1.ProportionalZooming = true;
            this.viewerControl3D1.Size = new System.Drawing.Size(962, 720);
            this.viewerControl3D1.SliceIndexX = 0;
            this.viewerControl3D1.SliceIndexY = 0;
            this.viewerControl3D1.SliceIndexZ = 0;
            this.viewerControl3D1.TabIndex = 7;
            this.viewerControl3D1.Visible = false;
            this.viewerControl3D1.ZoomInterpolationMethod = System.Drawing.Drawing2D.InterpolationMode.Default;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panel3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel1);
            this.splitContainer2.Size = new System.Drawing.Size(1533, 954);
            this.splitContainer2.SplitterDistance = 296;
            this.splitContainer2.TabIndex = 8;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lDataDirectories);
            this.panel3.Controls.Add(this.textBox1);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.bLoad);
            this.panel3.Controls.Add(this.numNumCols);
            this.panel3.Controls.Add(this.bBrowse);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(296, 954);
            this.panel3.TabIndex = 9;
            // 
            // MultiView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1533, 954);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.viewerControl3D1);
            this.Name = "MultiView";
            this.Text = "MultiView";
            this.Load += new System.EventHandler(this.MultiView_Load);
            this.ResizeEnd += new System.EventHandler(this.MultiView_ResizeEnd);
            this.Resize += new System.EventHandler(this.MultiView_Resize);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numNumCols)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lDataDirectories;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button bBrowse;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.NumericUpDown numNumCols;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bLoad;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private ImageViewer3D.ViewerControl3D viewerControl3D1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel3;
        
    }
}