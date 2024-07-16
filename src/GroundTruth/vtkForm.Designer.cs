namespace GroundTruth
{
    partial class vtkForm
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
            this._3D_View1 = new GraphingLibrary.DrawingAndGraphing2._3D_View();
            this.SuspendLayout();
            // 
            // _3D_View1
            // 
            this._3D_View1.Dock = System.Windows.Forms.DockStyle.Fill;
            this._3D_View1.Location = new System.Drawing.Point(0, 0);
            this._3D_View1.Name = "_3D_View1";
            this._3D_View1.Size = new System.Drawing.Size(996, 831);
            this._3D_View1.TabIndex = 0;
            // 
            // vtkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(996, 831);
            this.Controls.Add(this._3D_View1);
            this.Name = "vtkForm";
            this.Text = "vtkForm";
            this.ResumeLayout(false);

        }

        #endregion

        private GraphingLibrary.DrawingAndGraphing2._3D_View _3D_View1;
    }
}