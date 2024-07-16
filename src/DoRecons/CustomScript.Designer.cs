namespace DoRecons
{
    partial class CustomScript
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
            this.matlabInterface1 = new IronPythonEditor.MatlabInterface();
            this.SuspendLayout();
            // 
            // matlabInterface1
            // 
            this.matlabInterface1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.matlabInterface1.Location = new System.Drawing.Point(0, 0);
            this.matlabInterface1.Name = "matlabInterface1";
            this.matlabInterface1.Size = new System.Drawing.Size(1226, 927);
            this.matlabInterface1.TabIndex = 0;
            // 
            // CustomScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1226, 927);
            this.Controls.Add(this.matlabInterface1);
            this.Name = "CustomScript";
            this.Text = "CustomScript";
            this.ResumeLayout(false);

        }

        #endregion

        private IronPythonEditor.MatlabInterface matlabInterface1;

    }
}