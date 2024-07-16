namespace ProcessRecons
{
    partial class DataViewControl
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
            this.bHide = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bHide
            // 
            this.bHide.Location = new System.Drawing.Point(32, 19);
            this.bHide.Name = "bHide";
            this.bHide.Size = new System.Drawing.Size(140, 59);
            this.bHide.TabIndex = 0;
            this.bHide.Text = "Hide Dataviewer";
            this.bHide.UseVisualStyleBackColor = true;
            this.bHide.Click += new System.EventHandler(this.bHide_Click);
            // 
            // DataViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 260);
            this.Controls.Add(this.bHide);
            this.Name = "DataViewControl";
            this.Text = "DataViewControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bHide;
    }
}