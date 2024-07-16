namespace Tomographic_Imaging_2
{
    partial class EasyGUI
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
            this.components = new System.ComponentModel.Container();
            this.bDataFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bTopLevelBrowse = new System.Windows.Forms.Button();
            this.lDataDirectories = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.bDumpQueue = new System.Windows.Forms.Button();
            this.bQueueDir = new System.Windows.Forms.Button();
            this.bWholeRecon = new System.Windows.Forms.Button();
            this.cWatchInput = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bOutputToSurveyor = new System.Windows.Forms.Button();
            this.tVGdrive = new System.Windows.Forms.TextBox();
            this.bShowStack = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.bFocusValue = new System.Windows.Forms.Button();
            this.bIntensity = new System.Windows.Forms.Button();
            this.lTextSummary = new System.Windows.Forms.RichTextBox();
            this.bFlyThrough = new System.Windows.Forms.Button();
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.nCutOff = new System.Windows.Forms.NumericUpDown();
            this.nPaddedSize = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lFilters = new System.Windows.Forms.ListBox();
            this.bRunSelectedPreProc = new System.Windows.Forms.Button();
            this.bCreateScript = new System.Windows.Forms.Button();
            this.bEditScript = new System.Windows.Forms.Button();
            this.bRunPreprocess = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.profileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.pImagePreview = new System.Windows.Forms.PictureBox();
            this.filewatchPreprocess = new System.IO.FileSystemWatcher();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timerReconBacklog = new System.Windows.Forms.Timer(this.components);
            this.bLoadRaw = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bPauseBatch = new System.Windows.Forms.Button();
            this.bFixReconQual = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.bCopy = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.bCopyOutput = new System.Windows.Forms.TextBox();
            this.bCopySource = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.lProcessProgress = new System.Windows.Forms.Label();
            this.tOutPath = new System.Windows.Forms.TextBox();
            this.tArchiveFolder = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.bImageEditor = new System.Windows.Forms.Button();
            this.aPreProcess = new Tomographic_Imaging_2.AdvancedSelectBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pPPLast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pPP0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pReconstruction)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nCutOff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPaddedSize)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pImagePreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filewatchPreprocess)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // bDataFolder
            // 
            this.bDataFolder.Location = new System.Drawing.Point(6, 32);
            this.bDataFolder.Name = "bDataFolder";
            this.bDataFolder.Size = new System.Drawing.Size(282, 20);
            this.bDataFolder.TabIndex = 0;
            this.bDataFolder.Text = "C:\\Development\\CellCT\\DataIn";
            this.bDataFolder.TextChanged += new System.EventHandler(this.bDataFolder_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Top Level ImageData Input Folder";
            // 
            // bTopLevelBrowse
            // 
            this.bTopLevelBrowse.Location = new System.Drawing.Point(294, 32);
            this.bTopLevelBrowse.Name = "bTopLevelBrowse";
            this.bTopLevelBrowse.Size = new System.Drawing.Size(77, 20);
            this.bTopLevelBrowse.TabIndex = 2;
            this.bTopLevelBrowse.Text = "Browse";
            this.bTopLevelBrowse.UseVisualStyleBackColor = true;
            this.bTopLevelBrowse.Click += new System.EventHandler(this.bTopLevelBrowse_Click);
            // 
            // lDataDirectories
            // 
            this.lDataDirectories.FormattingEnabled = true;
            this.lDataDirectories.Location = new System.Drawing.Point(6, 80);
            this.lDataDirectories.Name = "lDataDirectories";
            this.lDataDirectories.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lDataDirectories.Size = new System.Drawing.Size(282, 290);
            this.lDataDirectories.TabIndex = 3;
            this.lDataDirectories.SelectedIndexChanged += new System.EventHandler(this.lDataDirectories_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "ImageData Directories";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.bDumpQueue);
            this.groupBox1.Controls.Add(this.bQueueDir);
            this.groupBox1.Controls.Add(this.bWholeRecon);
            this.groupBox1.Controls.Add(this.cWatchInput);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.bDataFolder);
            this.groupBox1.Controls.Add(this.bTopLevelBrowse);
            this.groupBox1.Controls.Add(this.lDataDirectories);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 388);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(294, 233);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(80, 30);
            this.button4.TabIndex = 16;
            this.button4.Text = "Fast Queue";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // bDumpQueue
            // 
            this.bDumpQueue.Location = new System.Drawing.Point(294, 301);
            this.bDumpQueue.Name = "bDumpQueue";
            this.bDumpQueue.Size = new System.Drawing.Size(80, 30);
            this.bDumpQueue.TabIndex = 15;
            this.bDumpQueue.Text = "Start Queue";
            this.bDumpQueue.UseVisualStyleBackColor = true;
            this.bDumpQueue.Click += new System.EventHandler(this.bDumpQueue_Click);
            // 
            // bQueueDir
            // 
            this.bQueueDir.Location = new System.Drawing.Point(294, 265);
            this.bQueueDir.Name = "bQueueDir";
            this.bQueueDir.Size = new System.Drawing.Size(80, 30);
            this.bQueueDir.TabIndex = 14;
            this.bQueueDir.Text = "Queue";
            this.bQueueDir.UseVisualStyleBackColor = true;
            this.bQueueDir.Click += new System.EventHandler(this.bQueueDir_Click);
            // 
            // bWholeRecon
            // 
            this.bWholeRecon.Location = new System.Drawing.Point(294, 138);
            this.bWholeRecon.Name = "bWholeRecon";
            this.bWholeRecon.Size = new System.Drawing.Size(80, 30);
            this.bWholeRecon.TabIndex = 12;
            this.bWholeRecon.Text = "Process";
            this.bWholeRecon.UseVisualStyleBackColor = true;
            this.bWholeRecon.Click += new System.EventHandler(this.bWholeRecon_Click);
            // 
            // cWatchInput
            // 
            this.cWatchInput.Location = new System.Drawing.Point(294, 81);
            this.cWatchInput.Name = "cWatchInput";
            this.cWatchInput.Size = new System.Drawing.Size(77, 48);
            this.cWatchInput.TabIndex = 11;
            this.cWatchInput.Text = "Watch For Input";
            this.cWatchInput.UseVisualStyleBackColor = true;
            this.cWatchInput.CheckedChanged += new System.EventHandler(this.cWatchInput_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(294, 176);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 38);
            this.button1.TabIndex = 13;
            this.button1.Text = "Seperate Process";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            this.fileSystemWatcher1.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher1_Changed);
            this.fileSystemWatcher1.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher1_Created);
            this.fileSystemWatcher1.Deleted += new System.IO.FileSystemEventHandler(this.fileSystemWatcher1_Deleted);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.bOutputToSurveyor);
            this.groupBox3.Controls.Add(this.tVGdrive);
            this.groupBox3.Controls.Add(this.bShowStack);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.bFocusValue);
            this.groupBox3.Controls.Add(this.bIntensity);
            this.groupBox3.Controls.Add(this.lTextSummary);
            this.groupBox3.Controls.Add(this.bFlyThrough);
            this.groupBox3.Controls.Add(this.bShowCenteringTool);
            this.groupBox3.Controls.Add(this.progressBar1);
            this.groupBox3.Controls.Add(this.pPPLast);
            this.groupBox3.Controls.Add(this.pPP0);
            this.groupBox3.Controls.Add(this.pReconstruction);
            this.groupBox3.Controls.Add(this.Summary);
            this.groupBox3.Controls.Add(this.bDataView);
            this.groupBox3.Controls.Add(this.bMIP);
            this.groupBox3.Controls.Add(this.bCenter);
            this.groupBox3.Controls.Add(this.bBackground);
            this.groupBox3.Location = new System.Drawing.Point(907, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(369, 461);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Summary";
            // 
            // bOutputToSurveyor
            // 
            this.bOutputToSurveyor.Location = new System.Drawing.Point(169, 409);
            this.bOutputToSurveyor.Name = "bOutputToSurveyor";
            this.bOutputToSurveyor.Size = new System.Drawing.Size(119, 23);
            this.bOutputToSurveyor.TabIndex = 22;
            this.bOutputToSurveyor.Text = "Output to Surveyor";
            this.bOutputToSurveyor.UseVisualStyleBackColor = true;
            this.bOutputToSurveyor.Click += new System.EventHandler(this.bOutputToSurveyor_Click);
            // 
            // tVGdrive
            // 
            this.tVGdrive.Location = new System.Drawing.Point(13, 409);
            this.tVGdrive.Name = "tVGdrive";
            this.tVGdrive.Size = new System.Drawing.Size(144, 20);
            this.tVGdrive.TabIndex = 21;
            this.tVGdrive.Text = "C:\\";
            // 
            // bShowStack
            // 
            this.bShowStack.Enabled = false;
            this.bShowStack.Location = new System.Drawing.Point(206, 80);
            this.bShowStack.Name = "bShowStack";
            this.bShowStack.Size = new System.Drawing.Size(94, 41);
            this.bShowStack.TabIndex = 15;
            this.bShowStack.Text = "Stack";
            this.bShowStack.UseVisualStyleBackColor = true;
            this.bShowStack.Click += new System.EventHandler(this.bShowStack_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 393);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Visiongate Output Drive";
            // 
            // bFocusValue
            // 
            this.bFocusValue.Enabled = false;
            this.bFocusValue.Location = new System.Drawing.Point(106, 81);
            this.bFocusValue.Name = "bFocusValue";
            this.bFocusValue.Size = new System.Drawing.Size(94, 41);
            this.bFocusValue.TabIndex = 14;
            this.bFocusValue.Text = "Focus Value";
            this.bFocusValue.UseVisualStyleBackColor = true;
            this.bFocusValue.Click += new System.EventHandler(this.bFocusValue_Click);
            // 
            // bIntensity
            // 
            this.bIntensity.Enabled = false;
            this.bIntensity.Location = new System.Drawing.Point(6, 80);
            this.bIntensity.Name = "bIntensity";
            this.bIntensity.Size = new System.Drawing.Size(94, 41);
            this.bIntensity.TabIndex = 13;
            this.bIntensity.Text = "Integrated Intensity";
            this.bIntensity.UseVisualStyleBackColor = true;
            this.bIntensity.Click += new System.EventHandler(this.bIntensity_Click);
            // 
            // lTextSummary
            // 
            this.lTextSummary.Location = new System.Drawing.Point(10, 233);
            this.lTextSummary.Name = "lTextSummary";
            this.lTextSummary.Size = new System.Drawing.Size(353, 121);
            this.lTextSummary.TabIndex = 12;
            this.lTextSummary.Text = "";
            // 
            // bFlyThrough
            // 
            this.bFlyThrough.Enabled = false;
            this.bFlyThrough.Location = new System.Drawing.Point(106, 50);
            this.bFlyThrough.Name = "bFlyThrough";
            this.bFlyThrough.Size = new System.Drawing.Size(94, 25);
            this.bFlyThrough.TabIndex = 11;
            this.bFlyThrough.Text = "Fly Through";
            this.bFlyThrough.UseVisualStyleBackColor = true;
            // 
            // bShowCenteringTool
            // 
            this.bShowCenteringTool.Enabled = false;
            this.bShowCenteringTool.Location = new System.Drawing.Point(106, 19);
            this.bShowCenteringTool.Name = "bShowCenteringTool";
            this.bShowCenteringTool.Size = new System.Drawing.Size(94, 25);
            this.bShowCenteringTool.TabIndex = 10;
            this.bShowCenteringTool.Text = "Centering Tool";
            this.bShowCenteringTool.UseVisualStyleBackColor = true;
            this.bShowCenteringTool.Click += new System.EventHandler(this.bShowCenteringTool_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(9, 360);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(354, 20);
            this.progressBar1.TabIndex = 9;
            // 
            // pPPLast
            // 
            this.pPPLast.Location = new System.Drawing.Point(214, 127);
            this.pPPLast.Name = "pPPLast";
            this.pPPLast.Size = new System.Drawing.Size(96, 87);
            this.pPPLast.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pPPLast.TabIndex = 8;
            this.pPPLast.TabStop = false;
            this.pPPLast.Click += new System.EventHandler(this.pPPLast_Click);
            // 
            // pPP0
            // 
            this.pPP0.Location = new System.Drawing.Point(112, 127);
            this.pPP0.Name = "pPP0";
            this.pPP0.Size = new System.Drawing.Size(96, 87);
            this.pPP0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pPP0.TabIndex = 7;
            this.pPP0.TabStop = false;
            this.pPP0.Click += new System.EventHandler(this.pPP0_Click);
            // 
            // pReconstruction
            // 
            this.pReconstruction.Location = new System.Drawing.Point(10, 127);
            this.pReconstruction.Name = "pReconstruction";
            this.pReconstruction.Size = new System.Drawing.Size(96, 87);
            this.pReconstruction.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pReconstruction.TabIndex = 6;
            this.pReconstruction.TabStop = false;
            this.pReconstruction.Click += new System.EventHandler(this.pReconstruction_Click);
            // 
            // Summary
            // 
            this.Summary.AutoSize = true;
            this.Summary.Location = new System.Drawing.Point(6, 217);
            this.Summary.Name = "Summary";
            this.Summary.Size = new System.Drawing.Size(50, 13);
            this.Summary.TabIndex = 5;
            this.Summary.Text = "Summary";
            // 
            // bDataView
            // 
            this.bDataView.Enabled = false;
            this.bDataView.Location = new System.Drawing.Point(6, 50);
            this.bDataView.Name = "bDataView";
            this.bDataView.Size = new System.Drawing.Size(94, 25);
            this.bDataView.TabIndex = 3;
            this.bDataView.Text = "Data View";
            this.bDataView.UseVisualStyleBackColor = true;
            this.bDataView.Click += new System.EventHandler(this.bDataView_Click);
            // 
            // bMIP
            // 
            this.bMIP.Enabled = false;
            this.bMIP.Location = new System.Drawing.Point(206, 50);
            this.bMIP.Name = "bMIP";
            this.bMIP.Size = new System.Drawing.Size(94, 25);
            this.bMIP.TabIndex = 2;
            this.bMIP.Text = "MIP";
            this.bMIP.UseVisualStyleBackColor = true;
            this.bMIP.Click += new System.EventHandler(this.bMIP_Click);
            // 
            // bCenter
            // 
            this.bCenter.Enabled = false;
            this.bCenter.Location = new System.Drawing.Point(206, 19);
            this.bCenter.Name = "bCenter";
            this.bCenter.Size = new System.Drawing.Size(94, 25);
            this.bCenter.TabIndex = 1;
            this.bCenter.Text = "Centering Movie";
            this.bCenter.UseVisualStyleBackColor = true;
            this.bCenter.Click += new System.EventHandler(this.bCenter_Click);
            // 
            // bBackground
            // 
            this.bBackground.Enabled = false;
            this.bBackground.Location = new System.Drawing.Point(6, 19);
            this.bBackground.Name = "bBackground";
            this.bBackground.Size = new System.Drawing.Size(94, 25);
            this.bBackground.TabIndex = 0;
            this.bBackground.Text = "Background";
            this.bBackground.UseVisualStyleBackColor = true;
            this.bBackground.Click += new System.EventHandler(this.bBackground_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox4.Controls.Add(this.nCutOff);
            this.groupBox4.Controls.Add(this.nPaddedSize);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.lFilters);
            this.groupBox4.Controls.Add(this.bRunSelectedPreProc);
            this.groupBox4.Controls.Add(this.bCreateScript);
            this.groupBox4.Controls.Add(this.bEditScript);
            this.groupBox4.Controls.Add(this.bRunPreprocess);
            this.groupBox4.Controls.Add(this.aPreProcess);
            this.groupBox4.Location = new System.Drawing.Point(12, 470);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(774, 295);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ImageData Processing";
            // 
            // nCutOff
            // 
            this.nCutOff.Location = new System.Drawing.Point(586, 255);
            this.nCutOff.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nCutOff.Name = "nCutOff";
            this.nCutOff.Size = new System.Drawing.Size(82, 20);
            this.nCutOff.TabIndex = 20;
            this.nCutOff.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.nCutOff.Visible = false;
            // 
            // nPaddedSize
            // 
            this.nPaddedSize.Location = new System.Drawing.Point(465, 255);
            this.nPaddedSize.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nPaddedSize.Name = "nPaddedSize";
            this.nPaddedSize.Size = new System.Drawing.Size(94, 20);
            this.nPaddedSize.TabIndex = 19;
            this.nPaddedSize.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(583, 236);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Cut off Frequency";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(462, 236);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Padded Size";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(462, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Filter Info";
            // 
            // lFilters
            // 
            this.lFilters.FormattingEnabled = true;
            this.lFilters.Location = new System.Drawing.Point(462, 30);
            this.lFilters.Name = "lFilters";
            this.lFilters.Size = new System.Drawing.Size(215, 199);
            this.lFilters.TabIndex = 15;
            // 
            // bRunSelectedPreProc
            // 
            this.bRunSelectedPreProc.Enabled = false;
            this.bRunSelectedPreProc.Location = new System.Drawing.Point(691, 73);
            this.bRunSelectedPreProc.Name = "bRunSelectedPreProc";
            this.bRunSelectedPreProc.Size = new System.Drawing.Size(77, 48);
            this.bRunSelectedPreProc.TabIndex = 14;
            this.bRunSelectedPreProc.Text = "Run Selected Script";
            this.bRunSelectedPreProc.UseVisualStyleBackColor = true;
            this.bRunSelectedPreProc.Click += new System.EventHandler(this.bRunSelectedPreProc_Click);
            // 
            // bCreateScript
            // 
            this.bCreateScript.Location = new System.Drawing.Point(691, 179);
            this.bCreateScript.Name = "bCreateScript";
            this.bCreateScript.Size = new System.Drawing.Size(77, 37);
            this.bCreateScript.TabIndex = 13;
            this.bCreateScript.Text = "Create Script";
            this.bCreateScript.UseVisualStyleBackColor = true;
            this.bCreateScript.Click += new System.EventHandler(this.bCreateScript_Click);
            // 
            // bEditScript
            // 
            this.bEditScript.Enabled = false;
            this.bEditScript.Location = new System.Drawing.Point(691, 222);
            this.bEditScript.Name = "bEditScript";
            this.bEditScript.Size = new System.Drawing.Size(77, 37);
            this.bEditScript.TabIndex = 12;
            this.bEditScript.Text = "Edit Script";
            this.bEditScript.UseVisualStyleBackColor = true;
            this.bEditScript.Click += new System.EventHandler(this.bEditScript_Click);
            // 
            // bRunPreprocess
            // 
            this.bRunPreprocess.Enabled = false;
            this.bRunPreprocess.Location = new System.Drawing.Point(691, 30);
            this.bRunPreprocess.Name = "bRunPreprocess";
            this.bRunPreprocess.Size = new System.Drawing.Size(77, 37);
            this.bRunPreprocess.TabIndex = 11;
            this.bRunPreprocess.Text = "Run Preprocess";
            this.bRunPreprocess.UseVisualStyleBackColor = true;
            this.bRunPreprocess.Click += new System.EventHandler(this.bRunPreprocess_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.profileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1291, 24);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // profileToolStripMenuItem
            // 
            this.profileToolStripMenuItem.Name = "profileToolStripMenuItem";
            this.profileToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.profileToolStripMenuItem.Text = "Profile";
            // 
            // pImagePreview
            // 
            this.pImagePreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pImagePreview.Location = new System.Drawing.Point(395, 27);
            this.pImagePreview.Name = "pImagePreview";
            this.pImagePreview.Size = new System.Drawing.Size(506, 437);
            this.pImagePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pImagePreview.TabIndex = 16;
            this.pImagePreview.TabStop = false;
            // 
            // filewatchPreprocess
            // 
            this.filewatchPreprocess.EnableRaisingEvents = true;
            this.filewatchPreprocess.SynchronizingObject = this;
            this.filewatchPreprocess.Created += new System.IO.FileSystemEventHandler(this.filewatchPreprocess_Created);
            this.filewatchPreprocess.Deleted += new System.IO.FileSystemEventHandler(this.filewatchPreprocess_Deleted);
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timerReconBacklog
            // 
            this.timerReconBacklog.Enabled = true;
            this.timerReconBacklog.Tick += new System.EventHandler(this.timerReconBacklog_Tick);
            // 
            // bLoadRaw
            // 
            this.bLoadRaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bLoadRaw.Location = new System.Drawing.Point(12, 771);
            this.bLoadRaw.Name = "bLoadRaw";
            this.bLoadRaw.Size = new System.Drawing.Size(90, 21);
            this.bLoadRaw.TabIndex = 19;
            this.bLoadRaw.Text = "Load Raw";
            this.bLoadRaw.UseVisualStyleBackColor = true;
            this.bLoadRaw.Click += new System.EventHandler(this.bLoadRaw_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(275, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(80, 38);
            this.button2.TabIndex = 14;
            this.button2.Text = "Batch Process";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(275, 63);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(80, 38);
            this.button3.TabIndex = 20;
            this.button3.Text = "Clean Up Backgrounds";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bPauseBatch);
            this.groupBox2.Controls.Add(this.bFixReconQual);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.groupBox5);
            this.groupBox2.Controls.Add(this.lProcessProgress);
            this.groupBox2.Controls.Add(this.tOutPath);
            this.groupBox2.Controls.Add(this.tArchiveFolder);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Location = new System.Drawing.Point(788, 497);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(488, 268);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Archives";
            // 
            // bPauseBatch
            // 
            this.bPauseBatch.Location = new System.Drawing.Point(389, 46);
            this.bPauseBatch.Name = "bPauseBatch";
            this.bPauseBatch.Size = new System.Drawing.Size(80, 38);
            this.bPauseBatch.TabIndex = 28;
            this.bPauseBatch.Text = "Pause";
            this.bPauseBatch.UseVisualStyleBackColor = true;
            this.bPauseBatch.Click += new System.EventHandler(this.bPauseBatch_Click);
            // 
            // bFixReconQual
            // 
            this.bFixReconQual.Location = new System.Drawing.Point(407, 215);
            this.bFixReconQual.Name = "bFixReconQual";
            this.bFixReconQual.Size = new System.Drawing.Size(75, 42);
            this.bFixReconQual.TabIndex = 27;
            this.bFixReconQual.Text = "Fix Recon Qual";
            this.bFixReconQual.UseVisualStyleBackColor = true;
            this.bFixReconQual.Click += new System.EventHandler(this.bFixReconQual_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 60);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "Output Folder";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.bCopy);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.bCopyOutput);
            this.groupBox5.Controls.Add(this.bCopySource);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Location = new System.Drawing.Point(10, 147);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(350, 115);
            this.groupBox5.TabIndex = 25;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Copy";
            // 
            // bCopy
            // 
            this.bCopy.Location = new System.Drawing.Point(263, 28);
            this.bCopy.Name = "bCopy";
            this.bCopy.Size = new System.Drawing.Size(79, 27);
            this.bCopy.TabIndex = 29;
            this.bCopy.Text = "Copy";
            this.bCopy.UseVisualStyleBackColor = true;
            this.bCopy.Click += new System.EventHandler(this.bCopy_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(11, 59);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(38, 13);
            this.label10.TabIndex = 28;
            this.label10.Text = "Target";
            // 
            // bCopyOutput
            // 
            this.bCopyOutput.Location = new System.Drawing.Point(14, 76);
            this.bCopyOutput.Name = "bCopyOutput";
            this.bCopyOutput.Size = new System.Drawing.Size(238, 20);
            this.bCopyOutput.TabIndex = 27;
            this.bCopyOutput.Text = "h:\\Processed\\";
            // 
            // bCopySource
            // 
            this.bCopySource.Location = new System.Drawing.Point(14, 36);
            this.bCopySource.Name = "bCopySource";
            this.bCopySource.Size = new System.Drawing.Size(238, 20);
            this.bCopySource.TabIndex = 27;
            this.bCopySource.Text = "c:\\processed\\";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Source";
            // 
            // lProcessProgress
            // 
            this.lProcessProgress.AutoSize = true;
            this.lProcessProgress.Location = new System.Drawing.Point(12, 104);
            this.lProcessProgress.Name = "lProcessProgress";
            this.lProcessProgress.Size = new System.Drawing.Size(31, 13);
            this.lProcessProgress.TabIndex = 24;
            this.lProcessProgress.Text = "____";
            // 
            // tOutPath
            // 
            this.tOutPath.Location = new System.Drawing.Point(10, 75);
            this.tOutPath.Name = "tOutPath";
            this.tOutPath.Size = new System.Drawing.Size(238, 20);
            this.tOutPath.TabIndex = 23;
            this.tOutPath.Text = "C:\\processed6\\";
            // 
            // tArchiveFolder
            // 
            this.tArchiveFolder.Location = new System.Drawing.Point(10, 36);
            this.tArchiveFolder.Name = "tArchiveFolder";
            this.tArchiveFolder.Size = new System.Drawing.Size(238, 20);
            this.tArchiveFolder.TabIndex = 22;
            this.tArchiveFolder.Text = "f:\\";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Base Folder";
            // 
            // bImageEditor
            // 
            this.bImageEditor.Location = new System.Drawing.Point(108, 769);
            this.bImageEditor.Name = "bImageEditor";
            this.bImageEditor.Size = new System.Drawing.Size(75, 23);
            this.bImageEditor.TabIndex = 22;
            this.bImageEditor.Text = "Image Editor";
            this.bImageEditor.UseVisualStyleBackColor = true;
            this.bImageEditor.Click += new System.EventHandler(this.ImageEditor_Click);
            // 
            // aPreProcess
            // 
            this.aPreProcess.FirstBoxLabel = "Available Scripts";
            this.aPreProcess.Location = new System.Drawing.Point(9, 19);
            this.aPreProcess.Name = "aPreProcess";
            this.aPreProcess.SecondBoxLabel = "Preprocessing Script";
            this.aPreProcess.Size = new System.Drawing.Size(454, 256);
            this.aPreProcess.TabIndex = 0;
            this.aPreProcess.FirstBoxSelected += new Tomographic_Imaging_2.AdvancedSelectBox.BoxSelectedEvent(this.aPreProcess_FirstBoxSelected);
            this.aPreProcess.SecondBoxSelected += new Tomographic_Imaging_2.AdvancedSelectBox.BoxSelectedEvent(this.aPreProcess_SecondBoxSelected);
            // 
            // EasyGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1291, 793);
            this.Controls.Add(this.bImageEditor);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.pImagePreview);
            this.Controls.Add(this.bLoadRaw);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(798, 554);
            this.Name = "EasyGUI";
            this.Text = "EasyGUI";
            this.Load += new System.EventHandler(this.EasyGUI_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pPPLast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pPP0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pReconstruction)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nCutOff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPaddedSize)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pImagePreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filewatchPreprocess)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox bDataFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bTopLevelBrowse;
        private System.Windows.Forms.ListBox lDataDirectories;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cWatchInput;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private AdvancedSelectBox aPreProcess;
        private System.Windows.Forms.Button bDataView;
        private System.Windows.Forms.Button bMIP;
        private System.Windows.Forms.Button bCenter;
        private System.Windows.Forms.Button bBackground;
        private System.Windows.Forms.Label Summary;
        private System.Windows.Forms.PictureBox pPPLast;
        private System.Windows.Forms.PictureBox pPP0;
        private System.Windows.Forms.PictureBox pReconstruction;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem profileToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button bRunPreprocess;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button bEditScript;
        private System.Windows.Forms.PictureBox pImagePreview;
        private System.Windows.Forms.Button bCreateScript;
        private System.Windows.Forms.Button bRunSelectedPreProc;
        private System.Windows.Forms.Button bShowCenteringTool;
        private System.Windows.Forms.Button bFlyThrough;
        private System.IO.FileSystemWatcher filewatchPreprocess;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timerReconBacklog;
        private System.Windows.Forms.RichTextBox lTextSummary;
        private System.Windows.Forms.Button bWholeRecon;
        private System.Windows.Forms.Button bIntensity;
        private System.Windows.Forms.Button bFocusValue;
        private System.Windows.Forms.Button bShowStack;
        private System.Windows.Forms.Button bLoadRaw;
        private System.Windows.Forms.Button bOutputToSurveyor;
        private System.Windows.Forms.TextBox tVGdrive;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.NumericUpDown nCutOff;
        private System.Windows.Forms.NumericUpDown nPaddedSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lFilters;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tArchiveFolder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button bDumpQueue;
        private System.Windows.Forms.Button bQueueDir;
        private System.Windows.Forms.Button bImageEditor;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox tOutPath;
        private System.Windows.Forms.Label lProcessProgress;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button bCopy;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox bCopyOutput;
        private System.Windows.Forms.TextBox bCopySource;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button bFixReconQual;
        private System.Windows.Forms.Button bPauseBatch;
    }
}