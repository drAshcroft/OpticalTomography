namespace Tomographic_Imaging_2
{
    partial class VisionForm
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
            this.bClose = new System.Windows.Forms.Button();
            this.bUpdate = new System.Windows.Forms.Button();
            this.tFileSelector = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.imageFinder1 = new VisionHelper.EdgeFinder();
            ((System.ComponentModel.ISupportInitialize)(this.tFileSelector)).BeginInit();
            this.SuspendLayout();
            // 
            // bClose
            // 
            this.bClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bClose.Location = new System.Drawing.Point(743, 570);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(110, 45);
            this.bClose.TabIndex = 9;
            this.bClose.Text = "Close";
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            // 
            // bUpdate
            // 
            this.bUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bUpdate.Location = new System.Drawing.Point(627, 570);
            this.bUpdate.Name = "bUpdate";
            this.bUpdate.Size = new System.Drawing.Size(110, 45);
            this.bUpdate.TabIndex = 8;
            this.bUpdate.Text = "Update";
            this.bUpdate.UseVisualStyleBackColor = true;
            this.bUpdate.Click += new System.EventHandler(this.bUpdate_Click);
            // 
            // tFileSelector
            // 
            this.tFileSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tFileSelector.Location = new System.Drawing.Point(12, 570);
            this.tFileSelector.Name = "tFileSelector";
            this.tFileSelector.Size = new System.Drawing.Size(609, 45);
            this.tFileSelector.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 554);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "File";
            // 
            // imageFinder1
            // 
            this.imageFinder1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.imageFinder1.Location = new System.Drawing.Point(12, 12);
            this.imageFinder1.Name = "imageFinder1";
            this.imageFinder1.Size = new System.Drawing.Size(841, 539);
            this.imageFinder1.TabIndex = 0;
            // 
            // VisionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(865, 616);
            this.Controls.Add(this.bClose);
            this.Controls.Add(this.bUpdate);
            this.Controls.Add(this.tFileSelector);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.imageFinder1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "VisionForm";
            this.Text = "VisionForm";
            ((System.ComponentModel.ISupportInitialize)(this.tFileSelector)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VisionHelper.EdgeFinder imageFinder1;
        private System.Windows.Forms.Button bClose;
        private System.Windows.Forms.Button bUpdate;
        private System.Windows.Forms.TrackBar tFileSelector;
        private System.Windows.Forms.Label label1;
    }
}