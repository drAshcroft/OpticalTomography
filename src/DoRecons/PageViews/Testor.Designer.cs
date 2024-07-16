namespace DoRecons.PageViews
{
    partial class Testor
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
            this.lDataFolders = new System.Windows.Forms.ListBox();
            this.bStart = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.tDataFolder = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.bShowStack = new System.Windows.Forms.Button();
            this.bFocusValue = new System.Windows.Forms.Button();
            this.bIntensity = new System.Windows.Forms.Button();
            this.lTextSummary = new System.Windows.Forms.RichTextBox();
            this.bFancyView = new System.Windows.Forms.Button();
            this.bShowCenteringTool = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.pPPLast = new System.Windows.Forms.PictureBox();
            this.pPP0 = new System.Windows.Forms.PictureBox();
            this.pReconstruction = new System.Windows.Forms.PictureBox();
            this.Summary = new System.Windows.Forms.Label();
            this.bDataView = new System.Windows.Forms.Button();
            this.bMIP = new System.Windows.Forms.Button();
            this.bCenter = new System.Windows.Forms.Button();
            this.bBackground = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.bTestScript = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.uConsole1 = new DoRecons.uConsole();
            ((System.ComponentModel.ISupportInitialize)(this.pPPLast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pPP0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pReconstruction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lDataFolders
            // 
            this.lDataFolders.FormattingEnabled = true;
            this.lDataFolders.Location = new System.Drawing.Point(9, 46);
            this.lDataFolders.Name = "lDataFolders";
            this.lDataFolders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lDataFolders.Size = new System.Drawing.Size(248, 329);
            this.lDataFolders.TabIndex = 36;
            this.lDataFolders.SelectedIndexChanged += new System.EventHandler(this.lDataFolders_SelectedIndexChanged);
            // 
            // bStart
            // 
            this.bStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bStart.Location = new System.Drawing.Point(587, 194);
            this.bStart.Name = "bStart";
            this.bStart.Size = new System.Drawing.Size(123, 47);
            this.bStart.TabIndex = 34;
            this.bStart.Text = "Start";
            this.bStart.UseVisualStyleBackColor = false;
            this.bStart.Click += new System.EventHandler(this.bStart_Click);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(219, 20);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(38, 20);
            this.button14.TabIndex = 33;
            this.button14.Text = "...";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // tDataFolder
            // 
            this.tDataFolder.Location = new System.Drawing.Point(9, 20);
            this.tDataFolder.Name = "tDataFolder";
            this.tDataFolder.Size = new System.Drawing.Size(204, 20);
            this.tDataFolder.TabIndex = 32;
            this.tDataFolder.Text = "C:\\Development\\CellCT\\DataIN";
            this.tDataFolder.TextChanged += new System.EventHandler(this.tDataFolder_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Watch Folder";
            // 
            // bShowStack
            // 
            this.bShowStack.Enabled = false;
            this.bShowStack.Location = new System.Drawing.Point(472, 107);
            this.bShowStack.Name = "bShowStack";
            this.bShowStack.Size = new System.Drawing.Size(94, 41);
            this.bShowStack.TabIndex = 51;
            this.bShowStack.Text = "Stack";
            this.bShowStack.UseVisualStyleBackColor = true;
            this.bShowStack.Click += new System.EventHandler(this.bShowStack_Click);
            // 
            // bFocusValue
            // 
            this.bFocusValue.Enabled = false;
            this.bFocusValue.Location = new System.Drawing.Point(370, 107);
            this.bFocusValue.Name = "bFocusValue";
            this.bFocusValue.Size = new System.Drawing.Size(94, 41);
            this.bFocusValue.TabIndex = 50;
            this.bFocusValue.Text = "Focus Value";
            this.bFocusValue.UseVisualStyleBackColor = true;
            this.bFocusValue.Click += new System.EventHandler(this.bFocusValue_Click);
            // 
            // bIntensity
            // 
            this.bIntensity.Enabled = false;
            this.bIntensity.Location = new System.Drawing.Point(268, 107);
            this.bIntensity.Name = "bIntensity";
            this.bIntensity.Size = new System.Drawing.Size(94, 41);
            this.bIntensity.TabIndex = 49;
            this.bIntensity.Text = "Integrated Intensity";
            this.bIntensity.UseVisualStyleBackColor = true;
            this.bIntensity.Click += new System.EventHandler(this.bIntensity_Click);
            // 
            // lTextSummary
            // 
            this.lTextSummary.Location = new System.Drawing.Point(268, 260);
            this.lTextSummary.Name = "lTextSummary";
            this.lTextSummary.Size = new System.Drawing.Size(442, 121);
            this.lTextSummary.TabIndex = 48;
            this.lTextSummary.Text = "";
            // 
            // bFancyView
            // 
            this.bFancyView.Enabled = false;
            this.bFancyView.Location = new System.Drawing.Point(370, 76);
            this.bFancyView.Name = "bFancyView";
            this.bFancyView.Size = new System.Drawing.Size(94, 25);
            this.bFancyView.TabIndex = 47;
            this.bFancyView.Text = "Fancy View";
            this.bFancyView.UseVisualStyleBackColor = true;
            this.bFancyView.Click += new System.EventHandler(this.bFancyView_Click);
            // 
            // bShowCenteringTool
            // 
            this.bShowCenteringTool.Enabled = false;
            this.bShowCenteringTool.Location = new System.Drawing.Point(370, 45);
            this.bShowCenteringTool.Name = "bShowCenteringTool";
            this.bShowCenteringTool.Size = new System.Drawing.Size(94, 25);
            this.bShowCenteringTool.TabIndex = 46;
            this.bShowCenteringTool.Text = "Centering Tool";
            this.bShowCenteringTool.UseVisualStyleBackColor = true;
            this.bShowCenteringTool.Click += new System.EventHandler(this.bShowCenteringTool_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(10, 387);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(700, 20);
            this.progressBar1.TabIndex = 45;
            // 
            // pPPLast
            // 
            this.pPPLast.Location = new System.Drawing.Point(472, 154);
            this.pPPLast.Name = "pPPLast";
            this.pPPLast.Size = new System.Drawing.Size(96, 87);
            this.pPPLast.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pPPLast.TabIndex = 44;
            this.pPPLast.TabStop = false;
            this.pPPLast.Click += new System.EventHandler(this.pPPLast_Click);
            // 
            // pPP0
            // 
            this.pPP0.Location = new System.Drawing.Point(370, 154);
            this.pPP0.Name = "pPP0";
            this.pPP0.Size = new System.Drawing.Size(96, 87);
            this.pPP0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pPP0.TabIndex = 43;
            this.pPP0.TabStop = false;
            this.pPP0.Click += new System.EventHandler(this.pPP0_Click);
            // 
            // pReconstruction
            // 
            this.pReconstruction.Location = new System.Drawing.Point(268, 154);
            this.pReconstruction.Name = "pReconstruction";
            this.pReconstruction.Size = new System.Drawing.Size(96, 87);
            this.pReconstruction.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pReconstruction.TabIndex = 42;
            this.pReconstruction.TabStop = false;
            this.pReconstruction.Click += new System.EventHandler(this.pReconstruction_Click);
            // 
            // Summary
            // 
            this.Summary.AutoSize = true;
            this.Summary.Location = new System.Drawing.Point(264, 244);
            this.Summary.Name = "Summary";
            this.Summary.Size = new System.Drawing.Size(50, 13);
            this.Summary.TabIndex = 41;
            this.Summary.Text = "Summary";
            // 
            // bDataView
            // 
            this.bDataView.Enabled = false;
            this.bDataView.Location = new System.Drawing.Point(268, 77);
            this.bDataView.Name = "bDataView";
            this.bDataView.Size = new System.Drawing.Size(94, 25);
            this.bDataView.TabIndex = 40;
            this.bDataView.Text = "Data View";
            this.bDataView.UseVisualStyleBackColor = true;
            this.bDataView.Click += new System.EventHandler(this.bDataView_Click);
            // 
            // bMIP
            // 
            this.bMIP.Enabled = false;
            this.bMIP.Location = new System.Drawing.Point(472, 77);
            this.bMIP.Name = "bMIP";
            this.bMIP.Size = new System.Drawing.Size(94, 25);
            this.bMIP.TabIndex = 39;
            this.bMIP.Text = "MIP";
            this.bMIP.UseVisualStyleBackColor = true;
            this.bMIP.Click += new System.EventHandler(this.bMIP_Click);
            // 
            // bCenter
            // 
            this.bCenter.Enabled = false;
            this.bCenter.Location = new System.Drawing.Point(472, 46);
            this.bCenter.Name = "bCenter";
            this.bCenter.Size = new System.Drawing.Size(94, 25);
            this.bCenter.TabIndex = 38;
            this.bCenter.Text = "Centering Movie";
            this.bCenter.UseVisualStyleBackColor = true;
            this.bCenter.Click += new System.EventHandler(this.bCenter_Click);
            // 
            // bBackground
            // 
            this.bBackground.Enabled = false;
            this.bBackground.Location = new System.Drawing.Point(268, 46);
            this.bBackground.Name = "bBackground";
            this.bBackground.Size = new System.Drawing.Size(94, 25);
            this.bBackground.TabIndex = 37;
            this.bBackground.Text = "Background";
            this.bBackground.UseVisualStyleBackColor = true;
            this.bBackground.Click += new System.EventHandler(this.bBackground_Click);
            // 
            // bTestScript
            // 
            this.bTestScript.Location = new System.Drawing.Point(597, 152);
            this.bTestScript.Name = "bTestScript";
            this.bTestScript.Size = new System.Drawing.Size(103, 36);
            this.bTestScript.TabIndex = 54;
            this.bTestScript.Text = "Test Script";
            this.bTestScript.UseVisualStyleBackColor = true;
            this.bTestScript.Click += new System.EventHandler(this.bTestScript_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(572, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(140, 132);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 55;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(370, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 23);
            this.button1.TabIndex = 56;
            this.button1.Text = "VisionGate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // uConsole1
            // 
            this.uConsole1.Location = new System.Drawing.Point(9, 413);
            this.uConsole1.Name = "uConsole1";
            this.uConsole1.Size = new System.Drawing.Size(701, 525);
            this.uConsole1.TabIndex = 53;
            // 
            // Testor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.bTestScript);
            this.Controls.Add(this.uConsole1);
            this.Controls.Add(this.bShowStack);
            this.Controls.Add(this.bFocusValue);
            this.Controls.Add(this.bIntensity);
            this.Controls.Add(this.lTextSummary);
            this.Controls.Add(this.bFancyView);
            this.Controls.Add(this.bShowCenteringTool);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.pPPLast);
            this.Controls.Add(this.pPP0);
            this.Controls.Add(this.pReconstruction);
            this.Controls.Add(this.Summary);
            this.Controls.Add(this.bDataView);
            this.Controls.Add(this.bMIP);
            this.Controls.Add(this.bCenter);
            this.Controls.Add(this.bBackground);
            this.Controls.Add(this.lDataFolders);
            this.Controls.Add(this.bStart);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.tDataFolder);
            this.Controls.Add(this.label6);
            this.Name = "Testor";
            this.Size = new System.Drawing.Size(727, 948);
            this.Load += new System.EventHandler(this.Testor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pPPLast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pPP0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pReconstruction)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lDataFolders;
        private System.Windows.Forms.Button bStart;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.TextBox tDataFolder;
        private System.Windows.Forms.Label label6;
       
        private System.Windows.Forms.Button bShowStack;
        private System.Windows.Forms.Button bFocusValue;
        private System.Windows.Forms.Button bIntensity;
        private System.Windows.Forms.RichTextBox lTextSummary;
        private System.Windows.Forms.Button bFancyView;
        private System.Windows.Forms.Button bShowCenteringTool;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.PictureBox pPPLast;
        private System.Windows.Forms.PictureBox pPP0;
        private System.Windows.Forms.PictureBox pReconstruction;
        private System.Windows.Forms.Label Summary;
        private System.Windows.Forms.Button bDataView;
        private System.Windows.Forms.Button bMIP;
        private System.Windows.Forms.Button bCenter;
        private System.Windows.Forms.Button bBackground;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private uConsole uConsole1;
        private System.Windows.Forms.Button bTestScript;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
    }
}
