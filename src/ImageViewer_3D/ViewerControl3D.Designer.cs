namespace ImageViewer3D
{
    partial class ViewerControl3D
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureDisplay3DSlice2 = new ImageViewer3D.PictureDisplay3DSlice();
            this.pictureDisplay3DSlice1 = new ImageViewer3D.PictureDisplay3DSlice();
            this.pictureDisplay1 = new ImageViewer3D.PictureDisplay3DSlice();
            this.crossDisplay1 = new ImageViewer3D.CrossDisplay();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tBrightness = new System.Windows.Forms.TrackBar();
            this.tContrast = new System.Windows.Forms.TrackBar();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay3DSlice2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay3DSlice1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay1)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tContrast)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Location = new System.Drawing.Point(0, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1103, 973);
            this.panel1.TabIndex = 1;
            this.panel1.SizeChanged += new System.EventHandler(this.panel1_SizeChanged);
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.pictureDisplay3DSlice2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pictureDisplay3DSlice1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureDisplay1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.crossDisplay1, 2, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1099, 969);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // pictureDisplay3DSlice2
            // 
            this.pictureDisplay3DSlice2.BackColor = System.Drawing.Color.Black;
            this.pictureDisplay3DSlice2.BorderColor = System.Drawing.Color.LightGray;
            this.pictureDisplay3DSlice2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureDisplay3DSlice2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureDisplay3DSlice2.Index = 0;
            this.pictureDisplay3DSlice2.Location = new System.Drawing.Point(3, 492);
            this.pictureDisplay3DSlice2.Name = "pictureDisplay3DSlice2";
            this.pictureDisplay3DSlice2.ScreenProperties = null;
            this.pictureDisplay3DSlice2.Size = new System.Drawing.Size(538, 474);
            this.pictureDisplay3DSlice2.TabIndex = 2;
            this.pictureDisplay3DSlice2.TabStop = false;
            this.pictureDisplay3DSlice2.UnZoomedImage = null;
            // 
            // pictureDisplay3DSlice1
            // 
            this.pictureDisplay3DSlice1.BackColor = System.Drawing.Color.Black;
            this.pictureDisplay3DSlice1.BorderColor = System.Drawing.Color.Blue;
            this.pictureDisplay3DSlice1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureDisplay3DSlice1.Index = 0;
            this.pictureDisplay3DSlice1.Location = new System.Drawing.Point(557, 3);
            this.pictureDisplay3DSlice1.Name = "pictureDisplay3DSlice1";
            this.pictureDisplay3DSlice1.ScreenProperties = null;
            this.pictureDisplay3DSlice1.Size = new System.Drawing.Size(539, 473);
            this.pictureDisplay3DSlice1.TabIndex = 1;
            this.pictureDisplay3DSlice1.TabStop = false;
            this.pictureDisplay3DSlice1.UnZoomedImage = null;
            // 
            // pictureDisplay1
            // 
            this.pictureDisplay1.BackColor = System.Drawing.Color.Black;
            this.pictureDisplay1.BorderColor = System.Drawing.Color.LightGray;
            this.pictureDisplay1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureDisplay1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureDisplay1.Index = 0;
            this.pictureDisplay1.Location = new System.Drawing.Point(3, 3);
            this.pictureDisplay1.Name = "pictureDisplay1";
            this.pictureDisplay1.ScreenProperties = null;
            this.pictureDisplay1.Size = new System.Drawing.Size(538, 473);
            this.pictureDisplay1.TabIndex = 0;
            this.pictureDisplay1.TabStop = false;
            this.pictureDisplay1.UnZoomedImage = null;
            // 
            // crossDisplay1
            // 
            this.crossDisplay1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.crossDisplay1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crossDisplay1.ImageX = null;
            this.crossDisplay1.ImageY = null;
            this.crossDisplay1.ImageZ = null;
            this.crossDisplay1.Location = new System.Drawing.Point(557, 492);
            this.crossDisplay1.Name = "crossDisplay1";
            this.crossDisplay1.Size = new System.Drawing.Size(539, 474);
            this.crossDisplay1.TabIndex = 3;
            this.crossDisplay1.YAxisMoved += new ImageViewer3D.CrossDisplay.YAxisMoviedEvent(this.crossDisplay1_YAxisMoved);
            this.crossDisplay1.XAxisMoved += new ImageViewer3D.CrossDisplay.XAxisMoviedEvent(this.crossDisplay1_XAxisMoved);
            this.crossDisplay1.ZAxisMoved += new ImageViewer3D.CrossDisplay.ZAxisMoviedEvent(this.crossDisplay1_ZAxisMoved);
          
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Location = new System.Drawing.Point(1109, 33);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(97, 971);
            this.panel2.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.tBrightness);
            this.panel3.Controls.Add(this.tContrast);
            this.panel3.Location = new System.Drawing.Point(1212, 33);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(107, 970);
            this.panel3.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Brightness";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Contrast";
            // 
            // tBrightness
            // 
            this.tBrightness.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tBrightness.Location = new System.Drawing.Point(50, 21);
            this.tBrightness.Maximum = 1000;
            this.tBrightness.Name = "tBrightness";
            this.tBrightness.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tBrightness.Size = new System.Drawing.Size(45, 946);
            this.tBrightness.TabIndex = 1;
            this.tBrightness.Value = 500;
            this.tBrightness.Scroll += new System.EventHandler(this.tBrightness_Scroll);
            // 
            // tContrast
            // 
            this.tContrast.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tContrast.Location = new System.Drawing.Point(3, 21);
            this.tContrast.Maximum = 1000;
            this.tContrast.Name = "tContrast";
            this.tContrast.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tContrast.Size = new System.Drawing.Size(45, 946);
            this.tContrast.TabIndex = 0;
            this.tContrast.Value = 500;
            this.tContrast.Scroll += new System.EventHandler(this.tContrast_Scroll);
            // 
            // ViewerControl3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ViewerControl3D";
            this.Size = new System.Drawing.Size(1319, 1009);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewerControl_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ViewerControl_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ViewerControl_KeyUp);
            this.Resize += new System.EventHandler(this.ViewerControl_Resize);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay3DSlice2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay3DSlice1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tContrast)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private PictureDisplay3DSlice pictureDisplay1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private PictureDisplay3DSlice pictureDisplay3DSlice2;
        private PictureDisplay3DSlice pictureDisplay3DSlice1;
        private CrossDisplay crossDisplay1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar tBrightness;
        private System.Windows.Forms.TrackBar tContrast;
    }
}
