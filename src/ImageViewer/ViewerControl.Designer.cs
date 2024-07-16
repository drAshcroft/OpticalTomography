namespace ImageViewer
{
    partial class ViewerControl
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureDisplay1 = new ImageViewer.PictureDisplay();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.pictureDisplay1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(796, 555);
            this.panel1.TabIndex = 1;
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            this.panel1.SizeChanged += new System.EventHandler(this.panel1_SizeChanged);
            // 
            // pictureDisplay1
            // 
            this.pictureDisplay1.BackColor = System.Drawing.Color.Black;
            this.pictureDisplay1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureDisplay1.Index = 0;
            this.pictureDisplay1.Location = new System.Drawing.Point(0, 0);
            this.pictureDisplay1.Name = "pictureDisplay1";
            this.pictureDisplay1.ScreenProperties = null;
            this.pictureDisplay1.Size = new System.Drawing.Size(792, 551);
            this.pictureDisplay1.TabIndex = 0;
            this.pictureDisplay1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Location = new System.Drawing.Point(802, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(97, 508);
            this.panel2.TabIndex = 2;
            // 
            // ViewerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ViewerControl";
            this.Size = new System.Drawing.Size(899, 558);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ViewerControl_KeyUp);
            this.Resize += new System.EventHandler(this.ViewerControl_Resize);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ViewerControl_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewerControl_KeyDown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureDisplay1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private PictureDisplay pictureDisplay1;
    }
}
