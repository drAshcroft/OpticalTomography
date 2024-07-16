namespace Rehydrate
{
    partial class Form1
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
            this.bStart = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.tOutFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button8 = new System.Windows.Forms.Button();
            this.tWatchFolder = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.uConsole1 = new DoRecons.uConsole();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // bStart
            // 
            this.bStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bStart.Location = new System.Drawing.Point(329, 30);
            this.bStart.Name = "bStart";
            this.bStart.Size = new System.Drawing.Size(123, 47);
            this.bStart.TabIndex = 51;
            this.bStart.Text = "Start";
            this.bStart.UseVisualStyleBackColor = false;
            this.bStart.Click += new System.EventHandler(this.bStart_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(267, 102);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(42, 20);
            this.button7.TabIndex = 50;
            this.button7.Text = "...";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // tOutFolder
            // 
            this.tOutFolder.Location = new System.Drawing.Point(12, 102);
            this.tOutFolder.Name = "tOutFolder";
            this.tOutFolder.Size = new System.Drawing.Size(248, 20);
            this.tOutFolder.TabIndex = 49;
            this.tOutFolder.Text = "c:\\rehydrated";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 48;
            this.label3.Text = "Output Folder";
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(267, 30);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(42, 20);
            this.button8.TabIndex = 47;
            this.button8.Text = "...";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // tWatchFolder
            // 
            this.tWatchFolder.Location = new System.Drawing.Point(12, 30);
            this.tWatchFolder.Name = "tWatchFolder";
            this.tWatchFolder.Size = new System.Drawing.Size(248, 20);
            this.tWatchFolder.TabIndex = 46;
            this.tWatchFolder.Text = "C:\\dehydrated\\cct001\\201012\\05";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 45;
            this.label4.Text = "Watch Folder";
            // 
            // uConsole1
            // 
            this.uConsole1.Location = new System.Drawing.Point(956, 13);
            this.uConsole1.Name = "uConsole1";
            this.uConsole1.Size = new System.Drawing.Size(404, 576);
            this.uConsole1.TabIndex = 53;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(16, 136);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(919, 452);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 54;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1372, 601);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.uConsole1);
            this.Controls.Add(this.bStart);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.tOutFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.tWatchFolder);
            this.Controls.Add(this.label4);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bStart;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TextBox tOutFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TextBox tWatchFolder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private DoRecons.uConsole uConsole1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

