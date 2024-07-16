using GraphingLib.DrawingAndGraphing;
namespace GraphingLib
{
    partial class GraphForm3D
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphForm3D));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bSave = new System.Windows.Forms.Button();
            this.graph3DSliceViewerMultiAngleEditor1 = new GraphingLib.DrawingAndGraphing.Graph3DSliceViewerMultiAngleEditor();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(952, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(513, 337);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(1036, 406);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 3;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // graph3DSliceViewerMultiAngleEditor1
            // 
            this.graph3DSliceViewerMultiAngleEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.graph3DSliceViewerMultiAngleEditor1.Location = new System.Drawing.Point(1, 1);
            this.graph3DSliceViewerMultiAngleEditor1.Name = "graph3DSliceViewerMultiAngleEditor1";
            this.graph3DSliceViewerMultiAngleEditor1.Size = new System.Drawing.Size(945, 758);
            this.graph3DSliceViewerMultiAngleEditor1.TabIndex = 4;
            // 
            // GraphForm3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1468, 771);
            this.Controls.Add(this.graph3DSliceViewerMultiAngleEditor1);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.pictureBox1);
            this.Name = "GraphForm3D";
            this.Text = "GraphForm3D";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GraphForm3D_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button bSave;
        private Graph3DSliceViewerMultiAngleEditor graph3DSliceViewerMultiAngleEditor1;
    }
}