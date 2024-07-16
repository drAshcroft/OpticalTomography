﻿namespace Dehydrate
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
            this.nDeathMinutes = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nNumRunning = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.bStop = new System.Windows.Forms.Button();
            this.bPause = new System.Windows.Forms.Button();
            this.bStart = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.tOutFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button8 = new System.Windows.Forms.Button();
            this.tWatchFolder = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.uConsole1 = new DoRecons.uConsole();
            ((System.ComponentModel.ISupportInitialize)(this.nDeathMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nNumRunning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // nDeathMinutes
            // 
            this.nDeathMinutes.Location = new System.Drawing.Point(383, 81);
            this.nDeathMinutes.Name = "nDeathMinutes";
            this.nDeathMinutes.Size = new System.Drawing.Size(120, 20);
            this.nDeathMinutes.TabIndex = 49;
            this.nDeathMinutes.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(383, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 48;
            this.label2.Text = "Minutes To Kill";
            // 
            // nNumRunning
            // 
            this.nNumRunning.Location = new System.Drawing.Point(383, 38);
            this.nNumRunning.Name = "nNumRunning";
            this.nNumRunning.Size = new System.Drawing.Size(120, 20);
            this.nNumRunning.TabIndex = 47;
            this.nNumRunning.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(380, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 46;
            this.label1.Text = "Number Running";
            // 
            // bStop
            // 
            this.bStop.BackColor = System.Drawing.Color.Red;
            this.bStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bStop.Location = new System.Drawing.Point(344, 210);
            this.bStop.Name = "bStop";
            this.bStop.Size = new System.Drawing.Size(123, 47);
            this.bStop.TabIndex = 45;
            this.bStop.Text = "Stop";
            this.bStop.UseVisualStyleBackColor = false;
            this.bStop.Visible = false;
            // 
            // bPause
            // 
            this.bPause.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.bPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bPause.Location = new System.Drawing.Point(215, 210);
            this.bPause.Name = "bPause";
            this.bPause.Size = new System.Drawing.Size(123, 47);
            this.bPause.TabIndex = 44;
            this.bPause.Text = "Pause";
            this.bPause.UseVisualStyleBackColor = false;
            // 
            // bStart
            // 
            this.bStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bStart.Location = new System.Drawing.Point(86, 210);
            this.bStart.Name = "bStart";
            this.bStart.Size = new System.Drawing.Size(123, 47);
            this.bStart.TabIndex = 43;
            this.bStart.Text = "Start";
            this.bStart.UseVisualStyleBackColor = false;
            this.bStart.Click += new System.EventHandler(this.bStart_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(267, 93);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(42, 20);
            this.button7.TabIndex = 42;
            this.button7.Text = "...";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // tOutFolder
            // 
            this.tOutFolder.Location = new System.Drawing.Point(12, 93);
            this.tOutFolder.Name = "tOutFolder";
            this.tOutFolder.Size = new System.Drawing.Size(248, 20);
            this.tOutFolder.TabIndex = 41;
            this.tOutFolder.Text = "c:\\dehydrated";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 40;
            this.label3.Text = "Output Folder";
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(267, 21);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(42, 20);
            this.button8.TabIndex = 39;
            this.button8.Text = "...";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // tWatchFolder
            // 
            this.tWatchFolder.Location = new System.Drawing.Point(12, 21);
            this.tWatchFolder.Name = "tWatchFolder";
            this.tWatchFolder.Size = new System.Drawing.Size(248, 20);
            this.tWatchFolder.TabIndex = 38;
            this.tWatchFolder.Text = "e:\\";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 37;
            this.label4.Text = "Watch Folder";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(15, 295);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(913, 604);
            this.pictureBox1.TabIndex = 50;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(638, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(290, 277);
            this.pictureBox2.TabIndex = 51;
            this.pictureBox2.TabStop = false;
            // 
            // uConsole1
            // 
            this.uConsole1.Location = new System.Drawing.Point(934, 12);
            this.uConsole1.Name = "uConsole1";
            this.uConsole1.Size = new System.Drawing.Size(675, 887);
            this.uConsole1.TabIndex = 52;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1620, 911);
            this.Controls.Add(this.uConsole1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.nDeathMinutes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nNumRunning);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bStop);
            this.Controls.Add(this.bPause);
            this.Controls.Add(this.bStart);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.tOutFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.tWatchFolder);
            this.Controls.Add(this.label4);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.nDeathMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nNumRunning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nDeathMinutes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nNumRunning;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bStop;
        private System.Windows.Forms.Button bPause;
        private System.Windows.Forms.Button bStart;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TextBox tOutFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TextBox tWatchFolder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private DoRecons.uConsole uConsole1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}
