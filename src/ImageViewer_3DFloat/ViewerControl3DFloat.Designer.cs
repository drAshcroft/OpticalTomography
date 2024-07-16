
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
            this.crossDisplay1 = new ImageViewer3D.CrossDisplay();
            this.panel4 = new System.Windows.Forms.Panel();
            this.pictureDisplay1 = new ImageViewer3D.PictureDisplay3DSlice();
            this.panel5 = new System.Windows.Forms.Panel();
            this.pictureDisplay3DSlice2 = new ImageViewer3D.PictureDisplay3DSlice();
            this.panel6 = new System.Windows.Forms.Panel();
            this.pictureDisplay3DSlice1 = new ImageViewer3D.PictureDisplay3DSlice();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tBrightness = new System.Windows.Forms.TrackBar();
            this.tContrast = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.LIntensity = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay1)).BeginInit();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay3DSlice2)).BeginInit();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay3DSlice1)).BeginInit();
            this.panel2.SuspendLayout();
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
            this.panel1.Location = new System.Drawing.Point(0, 54);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1031, 952);
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
            this.tableLayoutPanel1.Controls.Add(this.crossDisplay1, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel5, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel6, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1027, 948);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // crossDisplay1
            // 
            this.crossDisplay1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.crossDisplay1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crossDisplay1.ImageX = null;
            this.crossDisplay1.ImageY = null;
            this.crossDisplay1.ImageZ = null;
            this.crossDisplay1.Location = new System.Drawing.Point(521, 482);
            this.crossDisplay1.Name = "crossDisplay1";
            this.crossDisplay1.Size = new System.Drawing.Size(503, 463);
            this.crossDisplay1.SlicePositionX = 0.5D;
            this.crossDisplay1.SlicePositionY = 0.5D;
            this.crossDisplay1.SlicePositionZ = 0.5D;
            this.crossDisplay1.TabIndex = 3;
            this.crossDisplay1.YAxisMoved += new ImageViewer3D.CrossDisplay.YAxisMoviedEvent(this.crossDisplay1_YAxisMoved);
            this.crossDisplay1.XAxisMoved += new ImageViewer3D.CrossDisplay.XAxisMoviedEvent(this.crossDisplay1_XAxisMoved);
            this.crossDisplay1.ZAxisMoved += new ImageViewer3D.CrossDisplay.ZAxisMoviedEvent(this.crossDisplay1_ZAxisMoved);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Yellow;
            this.panel4.Controls.Add(this.pictureDisplay1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(502, 463);
            this.panel4.TabIndex = 4;
            // 
            // pictureDisplay1
            // 
            this.pictureDisplay1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureDisplay1.BackColor = System.Drawing.Color.Black;
            this.pictureDisplay1.BorderColor = System.Drawing.Color.Yellow;
            this.pictureDisplay1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureDisplay1.Index = 0;
            this.pictureDisplay1.Location = new System.Drawing.Point(3, 3);
            this.pictureDisplay1.Name = "pictureDisplay1";
            this.pictureDisplay1.ScreenProperties = null;
            this.pictureDisplay1.Size = new System.Drawing.Size(496, 457);
            this.pictureDisplay1.TabIndex = 0;
            this.pictureDisplay1.TabStop = false;
            this.pictureDisplay1.UnZoomedImage = null;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Lime;
            this.panel5.Controls.Add(this.pictureDisplay3DSlice2);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(3, 482);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(502, 463);
            this.panel5.TabIndex = 5;
            // 
            // pictureDisplay3DSlice2
            // 
            this.pictureDisplay3DSlice2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureDisplay3DSlice2.BackColor = System.Drawing.Color.Black;
            this.pictureDisplay3DSlice2.BorderColor = System.Drawing.Color.LightGray;
            this.pictureDisplay3DSlice2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureDisplay3DSlice2.Index = 0;
            this.pictureDisplay3DSlice2.Location = new System.Drawing.Point(3, 3);
            this.pictureDisplay3DSlice2.Name = "pictureDisplay3DSlice2";
            this.pictureDisplay3DSlice2.ScreenProperties = null;
            this.pictureDisplay3DSlice2.Size = new System.Drawing.Size(496, 457);
            this.pictureDisplay3DSlice2.TabIndex = 2;
            this.pictureDisplay3DSlice2.TabStop = false;
            this.pictureDisplay3DSlice2.UnZoomedImage = null;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.Blue;
            this.panel6.Controls.Add(this.pictureDisplay3DSlice1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(521, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(503, 463);
            this.panel6.TabIndex = 6;
            // 
            // pictureDisplay3DSlice1
            // 
            this.pictureDisplay3DSlice1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureDisplay3DSlice1.BackColor = System.Drawing.Color.Black;
            this.pictureDisplay3DSlice1.BorderColor = System.Drawing.Color.Blue;
            this.pictureDisplay3DSlice1.Index = 0;
            this.pictureDisplay3DSlice1.Location = new System.Drawing.Point(3, 3);
            this.pictureDisplay3DSlice1.Name = "pictureDisplay3DSlice1";
            this.pictureDisplay3DSlice1.ScreenProperties = null;
            this.pictureDisplay3DSlice1.Size = new System.Drawing.Size(497, 457);
            this.pictureDisplay3DSlice1.TabIndex = 1;
            this.pictureDisplay3DSlice1.TabStop = false;
            this.pictureDisplay3DSlice1.UnZoomedImage = null;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.LIntensity);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Location = new System.Drawing.Point(1034, 33);
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
            this.panel3.Location = new System.Drawing.Point(1137, 33);
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(523, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "X - Y Axis";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.label4.Location = new System.Drawing.Point(3, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "Z - Y Axis";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 927);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Intensity";
            // 
            // LIntensity
            // 
            this.LIntensity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LIntensity.AutoSize = true;
            this.LIntensity.Location = new System.Drawing.Point(3, 952);
            this.LIntensity.Name = "LIntensity";
            this.LIntensity.Size = new System.Drawing.Size(13, 13);
            this.LIntensity.TabIndex = 1;
            this.LIntensity.Text = "_";
            // 
            // ViewerControl3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ViewerControl3D";
            this.Size = new System.Drawing.Size(1244, 1009);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewerControl_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ViewerControl_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ViewerControl_KeyUp);
            this.Resize += new System.EventHandler(this.ViewerControl_Resize);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay1)).EndInit();
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay3DSlice2)).EndInit();
            this.panel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay3DSlice1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tContrast)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label LIntensity;
        private System.Windows.Forms.Label label5;
    }
}
