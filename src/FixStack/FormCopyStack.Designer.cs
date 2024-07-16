namespace FixStack
{
    partial class FormCopyStack
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
            this.button1 = new System.Windows.Forms.Button();
            this.tOutPath = new System.Windows.Forms.TextBox();
            this.tArchiveFolder = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(52, 164);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(176, 86);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tOutPath
            // 
            this.tOutPath.Location = new System.Drawing.Point(12, 78);
            this.tOutPath.Name = "tOutPath";
            this.tOutPath.Size = new System.Drawing.Size(238, 20);
            this.tOutPath.TabIndex = 26;
            this.tOutPath.Text = "c:\\processed5\\";
            // 
            // tArchiveFolder
            // 
            this.tArchiveFolder.Location = new System.Drawing.Point(12, 51);
            this.tArchiveFolder.Name = "tArchiveFolder";
            this.tArchiveFolder.Size = new System.Drawing.Size(238, 20);
            this.tArchiveFolder.TabIndex = 25;
            this.tArchiveFolder.Text = "g:\\";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 24;
            this.label7.Text = "Base Folder";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tOutPath);
            this.Controls.Add(this.tArchiveFolder);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tOutPath;
        private System.Windows.Forms.TextBox tArchiveFolder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
    }
}

