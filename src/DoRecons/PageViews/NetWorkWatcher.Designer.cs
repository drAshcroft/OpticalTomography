namespace DoRecons.PageViews
{
    partial class NetWorkWatcher
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
            this.bStopWatch = new System.Windows.Forms.Button();
            this.bStartWatch = new System.Windows.Forms.Button();
            this.bBrowseOutput = new System.Windows.Forms.Button();
            this.tOutFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bBrowseWatch = new System.Windows.Forms.Button();
            this.tWatchFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.uConsole1 = new DoRecons.uConsole();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // bStopWatch
            // 
            this.bStopWatch.BackColor = System.Drawing.Color.Red;
            this.bStopWatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bStopWatch.Location = new System.Drawing.Point(239, 139);
            this.bStopWatch.Name = "bStopWatch";
            this.bStopWatch.Size = new System.Drawing.Size(188, 70);
            this.bStopWatch.TabIndex = 19;
            this.bStopWatch.Text = "Stop Watching";
            this.bStopWatch.UseVisualStyleBackColor = false;
            this.bStopWatch.Click += new System.EventHandler(this.bStopWatch_Click);
            // 
            // bStartWatch
            // 
            this.bStartWatch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bStartWatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bStartWatch.Location = new System.Drawing.Point(45, 139);
            this.bStartWatch.Name = "bStartWatch";
            this.bStartWatch.Size = new System.Drawing.Size(188, 70);
            this.bStartWatch.TabIndex = 17;
            this.bStartWatch.Text = "Start Watching";
            this.bStartWatch.UseVisualStyleBackColor = false;
            this.bStartWatch.Click += new System.EventHandler(this.bStartWatch_Click);
            // 
            // bBrowseOutput
            // 
            this.bBrowseOutput.Location = new System.Drawing.Point(264, 101);
            this.bBrowseOutput.Name = "bBrowseOutput";
            this.bBrowseOutput.Size = new System.Drawing.Size(42, 20);
            this.bBrowseOutput.TabIndex = 16;
            this.bBrowseOutput.Text = "...";
            this.bBrowseOutput.UseVisualStyleBackColor = true;
            this.bBrowseOutput.Click += new System.EventHandler(this.bBrowseOutput_Click);
            // 
            // tOutFolder
            // 
            this.tOutFolder.Location = new System.Drawing.Point(9, 101);
            this.tOutFolder.Name = "tOutFolder";
            this.tOutFolder.Size = new System.Drawing.Size(248, 20);
            this.tOutFolder.TabIndex = 15;
            this.tOutFolder.Text = "V:\\ASU_Recon";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Output Folder";
            // 
            // bBrowseWatch
            // 
            this.bBrowseWatch.Location = new System.Drawing.Point(264, 29);
            this.bBrowseWatch.Name = "bBrowseWatch";
            this.bBrowseWatch.Size = new System.Drawing.Size(42, 20);
            this.bBrowseWatch.TabIndex = 13;
            this.bBrowseWatch.Text = "...";
            this.bBrowseWatch.UseVisualStyleBackColor = true;
            this.bBrowseWatch.Click += new System.EventHandler(this.bBrowseWatch_Click);
            // 
            // tWatchFolder
            // 
            this.tWatchFolder.Location = new System.Drawing.Point(9, 29);
            this.tWatchFolder.Name = "tWatchFolder";
            this.tWatchFolder.Size = new System.Drawing.Size(248, 20);
            this.tWatchFolder.TabIndex = 12;
            this.tWatchFolder.Text = "x:\\";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Watch Folder";
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.IncludeSubdirectories = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            this.fileSystemWatcher1.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher1_Created);
            // 
            // uConsole1
            // 
            this.uConsole1.Location = new System.Drawing.Point(3, 215);
            this.uConsole1.Name = "uConsole1";
            this.uConsole1.Size = new System.Drawing.Size(681, 721);
            this.uConsole1.TabIndex = 21;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(605, 47);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(54, 38);
            this.button1.TabIndex = 22;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // NetWorkWatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.uConsole1);
            this.Controls.Add(this.bStopWatch);
            this.Controls.Add(this.bStartWatch);
            this.Controls.Add(this.bBrowseOutput);
            this.Controls.Add(this.tOutFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bBrowseWatch);
            this.Controls.Add(this.tWatchFolder);
            this.Controls.Add(this.label1);
            this.Name = "NetWorkWatcher";
            this.Size = new System.Drawing.Size(702, 946);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bStopWatch;
        private System.Windows.Forms.Button bStartWatch;
        private System.Windows.Forms.Button bBrowseOutput;
        private System.Windows.Forms.TextBox tOutFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bBrowseWatch;
        private System.Windows.Forms.TextBox tWatchFolder;
        private System.Windows.Forms.Label label1;
       
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private uConsole uConsole1;
        private System.Windows.Forms.Button button1;
    }
}
