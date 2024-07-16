using MathHelpLib.DrawingAndGraphing;
using GraphingLib.DrawingAndGraphing;
namespace GraphingLib
{
    partial class GraphForm
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
           
            this.mathGraph1 = new MathGraph();
            this.SuspendLayout();
            // 
            // mathGraph1
            // 
            this.mathGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mathGraph1.Location = new System.Drawing.Point(0, 0);
            this.mathGraph1.Name = "mathGraph1";
            this.mathGraph1.Size = new System.Drawing.Size(970, 737);
            this.mathGraph1.TabIndex = 0;
          
            // 
            // GraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(970, 737);
            this.Controls.Add(this.mathGraph1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "GraphForm";
            this.Text = "GraphForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GraphForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private MathGraph mathGraph1;
    }
}