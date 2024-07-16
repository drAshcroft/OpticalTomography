namespace GraphingLib
{
    partial class StackExplorer
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
            this.StackSelector = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.rEven = new System.Windows.Forms.RadioButton();
            this.rOdd = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.StackSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // StackSelector
            // 
            this.StackSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StackSelector.Location = new System.Drawing.Point(12, 747);
            this.StackSelector.Name = "StackSelector";
            this.StackSelector.Size = new System.Drawing.Size(1098, 45);
            this.StackSelector.TabIndex = 20;
            this.StackSelector.ValueChanged += new System.EventHandler(this.StackSelector_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 731);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Stack Slice";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(5, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1195, 727);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 22;
            this.pictureBox1.TabStop = false;
            // 
            // rEven
            // 
            this.rEven.AutoSize = true;
            this.rEven.Location = new System.Drawing.Point(1119, 737);
            this.rEven.Name = "rEven";
            this.rEven.Size = new System.Drawing.Size(50, 17);
            this.rEven.TabIndex = 23;
            this.rEven.Text = "Even";
            this.rEven.UseVisualStyleBackColor = true;
            this.rEven.CheckedChanged += new System.EventHandler(this.rEven_CheckedChanged);
            // 
            // rOdd
            // 
            this.rOdd.AutoSize = true;
            this.rOdd.Location = new System.Drawing.Point(1119, 760);
            this.rOdd.Name = "rOdd";
            this.rOdd.Size = new System.Drawing.Size(45, 17);
            this.rOdd.TabIndex = 24;
            this.rOdd.Text = "Odd";
            this.rOdd.UseVisualStyleBackColor = true;
            this.rOdd.CheckedChanged += new System.EventHandler(this.rOdd_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(1170, 760);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(36, 17);
            this.radioButton1.TabIndex = 25;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "All";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // StackExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1216, 804);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.rOdd);
            this.Controls.Add(this.rEven);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.StackSelector);
            this.Name = "StackExplorer";
            this.Text = "StackExplorer";
            ((System.ComponentModel.ISupportInitialize)(this.StackSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar StackSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton rEven;
        private System.Windows.Forms.RadioButton rOdd;
        private System.Windows.Forms.RadioButton radioButton1;
    }
}