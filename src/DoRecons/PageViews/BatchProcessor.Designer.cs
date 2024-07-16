namespace DoRecons.PageViews
{
    partial class BatchProcessor
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
            this.bStop = new System.Windows.Forms.Button();
            this.bPause = new System.Windows.Forms.Button();
            this.bStart = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.tOutFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button8 = new System.Windows.Forms.Button();
            this.tWatchFolder = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nNumRunning = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nDeathMinutes = new System.Windows.Forms.NumericUpDown();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.bManyDir = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.uConsole1 = new DoRecons.uConsole();
            ((System.ComponentModel.ISupportInitialize)(this.nNumRunning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nDeathMinutes)).BeginInit();
            this.SuspendLayout();
            // 
            // bStop
            // 
            this.bStop.BackColor = System.Drawing.Color.Red;
            this.bStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bStop.Location = new System.Drawing.Point(349, 149);
            this.bStop.Name = "bStop";
            this.bStop.Size = new System.Drawing.Size(123, 47);
            this.bStop.TabIndex = 30;
            this.bStop.Text = "Stop";
            this.bStop.UseVisualStyleBackColor = false;
            this.bStop.Visible = false;
            this.bStop.Click += new System.EventHandler(this.bStop_Click);
            // 
            // bPause
            // 
            this.bPause.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.bPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bPause.Location = new System.Drawing.Point(220, 149);
            this.bPause.Name = "bPause";
            this.bPause.Size = new System.Drawing.Size(123, 47);
            this.bPause.TabIndex = 29;
            this.bPause.Text = "Pause";
            this.bPause.UseVisualStyleBackColor = false;
            this.bPause.Click += new System.EventHandler(this.bPause_Click);
            // 
            // bStart
            // 
            this.bStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bStart.Location = new System.Drawing.Point(91, 149);
            this.bStart.Name = "bStart";
            this.bStart.Size = new System.Drawing.Size(123, 47);
            this.bStart.TabIndex = 27;
            this.bStart.Text = "Start";
            this.bStart.UseVisualStyleBackColor = false;
            this.bStart.Click += new System.EventHandler(this.bStart_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(264, 91);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(42, 20);
            this.button7.TabIndex = 26;
            this.button7.Text = "...";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // tOutFolder
            // 
            this.tOutFolder.Location = new System.Drawing.Point(9, 91);
            this.tOutFolder.Name = "tOutFolder";
            this.tOutFolder.Size = new System.Drawing.Size(248, 20);
            this.tOutFolder.TabIndex = 25;
            this.tOutFolder.Text = "V:\\ASU_Recon";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Output Folder";
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(264, 19);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(42, 20);
            this.button8.TabIndex = 23;
            this.button8.Text = "...";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // tWatchFolder
            // 
            this.tWatchFolder.Location = new System.Drawing.Point(9, 19);
            this.tWatchFolder.Name = "tWatchFolder";
            this.tWatchFolder.Size = new System.Drawing.Size(248, 20);
            this.tWatchFolder.TabIndex = 22;
            this.tWatchFolder.Text = "V:\\Raw PP\\cct001\\Absorption\\201011\\15";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Watch Folder";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(377, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Number Running";
            // 
            // nNumRunning
            // 
            this.nNumRunning.Location = new System.Drawing.Point(380, 36);
            this.nNumRunning.Name = "nNumRunning";
            this.nNumRunning.Size = new System.Drawing.Size(120, 20);
            this.nNumRunning.TabIndex = 34;
            this.nNumRunning.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(380, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "Minutes To Kill";
            // 
            // nDeathMinutes
            // 
            this.nDeathMinutes.Location = new System.Drawing.Point(380, 79);
            this.nDeathMinutes.Name = "nDeathMinutes";
            this.nDeathMinutes.Size = new System.Drawing.Size(120, 20);
            this.nDeathMinutes.TabIndex = 36;
            this.nDeathMinutes.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(621, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 37;
            this.button1.Text = "Vivek\'s Routine";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bManyDir
            // 
            this.bManyDir.Location = new System.Drawing.Point(514, 149);
            this.bManyDir.Name = "bManyDir";
            this.bManyDir.Size = new System.Drawing.Size(121, 47);
            this.bManyDir.TabIndex = 38;
            this.bManyDir.Text = "Backwards Start";
            this.bManyDir.UseVisualStyleBackColor = true;
            this.bManyDir.Click += new System.EventHandler(this.bManyDir_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(605, 62);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(91, 29);
            this.button2.TabIndex = 39;
            this.button2.Text = "Fix Stack";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(605, 97);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(91, 29);
            this.button3.TabIndex = 40;
            this.button3.Text = "Back Stack";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // uConsole1
            // 
            this.uConsole1.Location = new System.Drawing.Point(9, 202);
            this.uConsole1.Name = "uConsole1";
            this.uConsole1.Size = new System.Drawing.Size(675, 731);
            this.uConsole1.TabIndex = 32;
            // 
            // BatchProcessor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.bManyDir);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.nDeathMinutes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nNumRunning);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.uConsole1);
            this.Controls.Add(this.bStop);
            this.Controls.Add(this.bPause);
            this.Controls.Add(this.bStart);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.tOutFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.tWatchFolder);
            this.Controls.Add(this.label4);
            this.Name = "BatchProcessor";
            this.Size = new System.Drawing.Size(696, 944);
            ((System.ComponentModel.ISupportInitialize)(this.nNumRunning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nDeathMinutes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bStop;
        private System.Windows.Forms.Button bPause;
        private System.Windows.Forms.Button bStart;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TextBox tOutFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TextBox tWatchFolder;
        private System.Windows.Forms.Label label4;
        private uConsole uConsole1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nNumRunning;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nDeathMinutes;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button bManyDir;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
      
    }
}
