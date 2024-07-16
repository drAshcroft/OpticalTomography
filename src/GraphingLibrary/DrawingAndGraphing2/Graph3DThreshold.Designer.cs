namespace MathHelpLib.DrawingAndGraphing
{
    partial class Graph3DThreshold
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lMiddle = new System.Windows.Forms.Label();
            this.lWidth = new System.Windows.Forms.Label();
            this.bColor = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Display Color";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Middle";
            // 
            // lMiddle
            // 
            this.lMiddle.AutoSize = true;
            this.lMiddle.Location = new System.Drawing.Point(13, 17);
            this.lMiddle.Name = "lMiddle";
            this.lMiddle.Size = new System.Drawing.Size(35, 13);
            this.lMiddle.TabIndex = 3;
            this.lMiddle.Text = "label4";
            // 
            // lWidth
            // 
            this.lWidth.AutoSize = true;
            this.lWidth.Location = new System.Drawing.Point(13, 57);
            this.lWidth.Name = "lWidth";
            this.lWidth.Size = new System.Drawing.Size(35, 13);
            this.lWidth.TabIndex = 4;
            this.lWidth.Text = "label4";
            // 
            // bColor
            // 
            this.bColor.Location = new System.Drawing.Point(16, 95);
            this.bColor.Name = "bColor";
            this.bColor.Size = new System.Drawing.Size(63, 23);
            this.bColor.TabIndex = 5;
            this.bColor.Text = "Color";
            this.bColor.UseVisualStyleBackColor = true;
            this.bColor.Click += new System.EventHandler(this.bColor_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Graph3DThreshold
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bColor);
            this.Controls.Add(this.lWidth);
            this.Controls.Add(this.lMiddle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Graph3DThreshold";
            this.Size = new System.Drawing.Size(105, 125);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lMiddle;
        private System.Windows.Forms.Label lWidth;
        private System.Windows.Forms.Button bColor;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Timer timer1;
    }
}
