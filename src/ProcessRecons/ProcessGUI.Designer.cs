namespace Tomographic_Imaging_2
{
    partial class ProcessGUI
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.bDataFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bTopLevelBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbLabeledDrive = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.LMachine = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lYear = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lMonth = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lDay = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label16 = new System.Windows.Forms.Label();
            this.tDataInputFolder = new System.Windows.Forms.TextBox();
            this.tBrowseDataFolder = new System.Windows.Forms.Button();
            this.tVGDrive = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tUserName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.lTextSummary = new System.Windows.Forms.RichTextBox();
            this.Summary = new System.Windows.Forms.Label();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.bBackground = new System.Windows.Forms.Button();
            this.bCenter = new System.Windows.Forms.Button();
            this.bMIP = new System.Windows.Forms.Button();
            this.bDataView = new System.Windows.Forms.Button();
            this.bFlyThrough = new System.Windows.Forms.Button();
            this.bIntensity = new System.Windows.Forms.Button();
            this.bFocusValue = new System.Windows.Forms.Button();
            this.bShowStack = new System.Windows.Forms.Button();
            this.bDataviewVG = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.bShowFancy = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pProj4 = new System.Windows.Forms.PictureBox();
            this.pProj3 = new System.Windows.Forms.PictureBox();
            this.pProj1 = new System.Windows.Forms.PictureBox();
            this.pProj2 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pPP0 = new System.Windows.Forms.PictureBox();
            this.ppQuarter = new System.Windows.Forms.PictureBox();
            this.pPPLast = new System.Windows.Forms.PictureBox();
            this.ppHalf = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.pCentering = new System.Windows.Forms.PictureBox();
            this.pReconX = new System.Windows.Forms.PictureBox();
            this.pREcon2 = new System.Windows.Forms.PictureBox();
            this.pMIP = new System.Windows.Forms.PictureBox();
            this.pStack = new System.Windows.Forms.PictureBox();
            this.pReconMine = new System.Windows.Forms.PictureBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.pReconVG = new System.Windows.Forms.PictureBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lVG = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.pp3Quarters = new System.Windows.Forms.PictureBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.profileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lDataDirectories = new System.Windows.Forms.DataGridView();
            this.Dataset_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReconSucceeded = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Registration_Quality = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Cell_Staining = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Focus_Quality = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Interfering_Object = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Good_Cell = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.TooClose = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Interesting = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Recon_Quality = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Noise = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Rings = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Run_Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Background = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Number_Quality = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timerCenterMovie = new System.Windows.Forms.Timer(this.components);
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pProj4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pProj3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pProj1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pProj2)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pPP0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ppQuarter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pPPLast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ppHalf)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pCentering)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pReconX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pREcon2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pMIP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pStack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pReconMine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pReconVG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pp3Quarters)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lDataDirectories)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // bDataFolder
            // 
            this.bDataFolder.Location = new System.Drawing.Point(6, 20);
            this.bDataFolder.Name = "bDataFolder";
            this.bDataFolder.Size = new System.Drawing.Size(188, 20);
            this.bDataFolder.TabIndex = 0;
            this.bDataFolder.Text = "V:\\ASU_Recon";
            this.bDataFolder.TextChanged += new System.EventHandler(this.bDataFolder_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Top Level ImageData Input Folder";
            // 
            // bTopLevelBrowse
            // 
            this.bTopLevelBrowse.Location = new System.Drawing.Point(200, 20);
            this.bTopLevelBrowse.Name = "bTopLevelBrowse";
            this.bTopLevelBrowse.Size = new System.Drawing.Size(77, 20);
            this.bTopLevelBrowse.TabIndex = 2;
            this.bTopLevelBrowse.Text = "Browse";
            this.bTopLevelBrowse.UseVisualStyleBackColor = true;
            this.bTopLevelBrowse.Click += new System.EventHandler(this.bTopLevelBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(395, 27);
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
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.tbLabeledDrive);
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Controls.Add(this.tVGDrive);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.tUserName);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.lTextSummary);
            this.groupBox1.Controls.Add(this.Summary);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(309, 809);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(82, 549);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "Labeled";
            // 
            // tbLabeledDrive
            // 
            this.tbLabeledDrive.Location = new System.Drawing.Point(85, 565);
            this.tbLabeledDrive.Name = "tbLabeledDrive";
            this.tbLabeledDrive.Size = new System.Drawing.Size(54, 20);
            this.tbLabeledDrive.TabIndex = 34;
            this.tbLabeledDrive.Text = "W:\\";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(6, 16);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(287, 491);
            this.tabControl1.TabIndex = 33;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.bDataFolder);
            this.tabPage1.Controls.Add(this.LMachine);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.lYear);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.lMonth);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.lDay);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.bTopLevelBrowse);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(279, 465);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Structure Load";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // LMachine
            // 
            this.LMachine.FormattingEnabled = true;
            this.LMachine.Location = new System.Drawing.Point(6, 57);
            this.LMachine.Name = "LMachine";
            this.LMachine.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.LMachine.Size = new System.Drawing.Size(188, 43);
            this.LMachine.TabIndex = 5;
            this.LMachine.SelectedIndexChanged += new System.EventHandler(this.LMachine_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Machine";
            // 
            // lYear
            // 
            this.lYear.FormattingEnabled = true;
            this.lYear.Location = new System.Drawing.Point(6, 129);
            this.lYear.Name = "lYear";
            this.lYear.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lYear.Size = new System.Drawing.Size(188, 69);
            this.lYear.TabIndex = 7;
            this.lYear.SelectedIndexChanged += new System.EventHandler(this.lYear_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Year";
            // 
            // lMonth
            // 
            this.lMonth.FormattingEnabled = true;
            this.lMonth.Location = new System.Drawing.Point(6, 222);
            this.lMonth.Name = "lMonth";
            this.lMonth.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lMonth.Size = new System.Drawing.Size(188, 134);
            this.lMonth.TabIndex = 9;
            this.lMonth.SelectedIndexChanged += new System.EventHandler(this.lMonth_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 201);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Month";
            // 
            // lDay
            // 
            this.lDay.FormattingEnabled = true;
            this.lDay.Location = new System.Drawing.Point(6, 376);
            this.lDay.Name = "lDay";
            this.lDay.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lDay.Size = new System.Drawing.Size(188, 82);
            this.lDay.TabIndex = 11;
            this.lDay.SelectedIndexChanged += new System.EventHandler(this.lDay_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 360);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Day";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.tDataInputFolder);
            this.tabPage2.Controls.Add(this.tBrowseDataFolder);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(279, 465);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Folder Load";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 18);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(89, 13);
            this.label16.TabIndex = 4;
            this.label16.Text = "Data Input Folder";
            // 
            // tDataInputFolder
            // 
            this.tDataInputFolder.Location = new System.Drawing.Point(3, 34);
            this.tDataInputFolder.Name = "tDataInputFolder";
            this.tDataInputFolder.Size = new System.Drawing.Size(188, 20);
            this.tDataInputFolder.TabIndex = 3;
            this.tDataInputFolder.Text = "V:\\ASU_Recon\\viveks_Ne";
            this.tDataInputFolder.TextChanged += new System.EventHandler(this.tDataInputFolder_TextChanged);
            // 
            // tBrowseDataFolder
            // 
            this.tBrowseDataFolder.Location = new System.Drawing.Point(197, 34);
            this.tBrowseDataFolder.Name = "tBrowseDataFolder";
            this.tBrowseDataFolder.Size = new System.Drawing.Size(77, 20);
            this.tBrowseDataFolder.TabIndex = 5;
            this.tBrowseDataFolder.Text = "Browse";
            this.tBrowseDataFolder.UseVisualStyleBackColor = true;
            this.tBrowseDataFolder.Click += new System.EventHandler(this.tBrowseDataFolder_Click);
            // 
            // tVGDrive
            // 
            this.tVGDrive.Location = new System.Drawing.Point(10, 565);
            this.tVGDrive.Name = "tVGDrive";
            this.tVGDrive.Size = new System.Drawing.Size(54, 20);
            this.tVGDrive.TabIndex = 27;
            this.tVGDrive.Text = "Y:\\";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 549);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(57, 13);
            this.label13.TabIndex = 26;
            this.label13.Text = "Processed";
            // 
            // tUserName
            // 
            this.tUserName.Location = new System.Drawing.Point(10, 526);
            this.tUserName.Name = "tUserName";
            this.tUserName.Size = new System.Drawing.Size(178, 20);
            this.tUserName.TabIndex = 25;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 510);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(29, 13);
            this.label12.TabIndex = 24;
            this.label12.Text = "User";
            // 
            // lTextSummary
            // 
            this.lTextSummary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lTextSummary.Location = new System.Drawing.Point(9, 619);
            this.lTextSummary.Name = "lTextSummary";
            this.lTextSummary.Size = new System.Drawing.Size(284, 184);
            this.lTextSummary.TabIndex = 12;
            this.lTextSummary.Text = "";
            // 
            // Summary
            // 
            this.Summary.AutoSize = true;
            this.Summary.Location = new System.Drawing.Point(7, 595);
            this.Summary.Name = "Summary";
            this.Summary.Size = new System.Drawing.Size(50, 13);
            this.Summary.TabIndex = 5;
            this.Summary.Text = "Summary";
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
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.tableLayoutPanel4);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.pReconVG);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.lVG);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.pp3Quarters);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1116, 411);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Summary";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 310F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel4.Controls.Add(this.groupBox5, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.groupBox4, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel3, 2, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.22066F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 68.77934F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1100, 380);
            this.tableLayoutPanel4.TabIndex = 41;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.bBackground);
            this.groupBox5.Controls.Add(this.bCenter);
            this.groupBox5.Controls.Add(this.bMIP);
            this.groupBox5.Controls.Add(this.bDataView);
            this.groupBox5.Controls.Add(this.bFlyThrough);
            this.groupBox5.Controls.Add(this.bIntensity);
            this.groupBox5.Controls.Add(this.bFocusValue);
            this.groupBox5.Controls.Add(this.bShowStack);
            this.groupBox5.Controls.Add(this.bDataviewVG);
            this.groupBox5.Controls.Add(this.button1);
            this.groupBox5.Controls.Add(this.bShowFancy);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(3, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(304, 112);
            this.groupBox5.TabIndex = 39;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Controls";
            // 
            // bBackground
            // 
            this.bBackground.Enabled = false;
            this.bBackground.Location = new System.Drawing.Point(6, 23);
            this.bBackground.Name = "bBackground";
            this.bBackground.Size = new System.Drawing.Size(94, 25);
            this.bBackground.TabIndex = 0;
            this.bBackground.Text = "Background";
            this.bBackground.UseVisualStyleBackColor = true;
            this.bBackground.Click += new System.EventHandler(this.bBackground_Click);
            // 
            // bCenter
            // 
            this.bCenter.Enabled = false;
            this.bCenter.Location = new System.Drawing.Point(106, 23);
            this.bCenter.Name = "bCenter";
            this.bCenter.Size = new System.Drawing.Size(94, 25);
            this.bCenter.TabIndex = 1;
            this.bCenter.Text = "Centering Movie";
            this.bCenter.UseVisualStyleBackColor = true;
            this.bCenter.Click += new System.EventHandler(this.bCenter_Click);
            // 
            // bMIP
            // 
            this.bMIP.Enabled = false;
            this.bMIP.Location = new System.Drawing.Point(206, 23);
            this.bMIP.Name = "bMIP";
            this.bMIP.Size = new System.Drawing.Size(94, 25);
            this.bMIP.TabIndex = 2;
            this.bMIP.Text = "MIP";
            this.bMIP.UseVisualStyleBackColor = true;
            this.bMIP.Click += new System.EventHandler(this.bMIP_Click);
            // 
            // bDataView
            // 
            this.bDataView.Enabled = false;
            this.bDataView.Location = new System.Drawing.Point(6, 54);
            this.bDataView.Name = "bDataView";
            this.bDataView.Size = new System.Drawing.Size(94, 25);
            this.bDataView.TabIndex = 3;
            this.bDataView.Text = "Data View";
            this.bDataView.UseVisualStyleBackColor = true;
            this.bDataView.Click += new System.EventHandler(this.bDataView_Click);
            // 
            // bFlyThrough
            // 
            this.bFlyThrough.Enabled = false;
            this.bFlyThrough.Location = new System.Drawing.Point(206, 54);
            this.bFlyThrough.Name = "bFlyThrough";
            this.bFlyThrough.Size = new System.Drawing.Size(94, 25);
            this.bFlyThrough.TabIndex = 11;
            this.bFlyThrough.Text = "Fly Through";
            this.bFlyThrough.UseVisualStyleBackColor = true;
            this.bFlyThrough.Click += new System.EventHandler(this.bFlyThrough_Click);
            // 
            // bIntensity
            // 
            this.bIntensity.Enabled = false;
            this.bIntensity.Location = new System.Drawing.Point(6, 84);
            this.bIntensity.Name = "bIntensity";
            this.bIntensity.Size = new System.Drawing.Size(94, 41);
            this.bIntensity.TabIndex = 13;
            this.bIntensity.Text = "Integrated Intensity";
            this.bIntensity.UseVisualStyleBackColor = true;
            this.bIntensity.Click += new System.EventHandler(this.bIntensity_Click);
            // 
            // bFocusValue
            // 
            this.bFocusValue.Enabled = false;
            this.bFocusValue.Location = new System.Drawing.Point(106, 85);
            this.bFocusValue.Name = "bFocusValue";
            this.bFocusValue.Size = new System.Drawing.Size(94, 41);
            this.bFocusValue.TabIndex = 14;
            this.bFocusValue.Text = "Focus Value";
            this.bFocusValue.UseVisualStyleBackColor = true;
            this.bFocusValue.Click += new System.EventHandler(this.bFocusValue_Click);
            // 
            // bShowStack
            // 
            this.bShowStack.Location = new System.Drawing.Point(206, 84);
            this.bShowStack.Name = "bShowStack";
            this.bShowStack.Size = new System.Drawing.Size(94, 41);
            this.bShowStack.TabIndex = 15;
            this.bShowStack.Text = "Stack";
            this.bShowStack.UseVisualStyleBackColor = true;
            this.bShowStack.Click += new System.EventHandler(this.bShowStack_Click);
            // 
            // bDataviewVG
            // 
            this.bDataviewVG.Location = new System.Drawing.Point(106, 54);
            this.bDataviewVG.Name = "bDataviewVG";
            this.bDataviewVG.Size = new System.Drawing.Size(94, 25);
            this.bDataviewVG.TabIndex = 25;
            this.bDataviewVG.Text = "Data ViewVG";
            this.bDataviewVG.UseVisualStyleBackColor = true;
            this.bDataviewVG.Click += new System.EventHandler(this.bDataviewVG_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(106, 129);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 38);
            this.button1.TabIndex = 27;
            this.button1.Text = "Output Back";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // bShowFancy
            // 
            this.bShowFancy.Location = new System.Drawing.Point(206, 129);
            this.bShowFancy.Name = "bShowFancy";
            this.bShowFancy.Size = new System.Drawing.Size(94, 38);
            this.bShowFancy.TabIndex = 26;
            this.bShowFancy.Text = "Fancy Viewer";
            this.bShowFancy.UseVisualStyleBackColor = true;
            this.bShowFancy.Click += new System.EventHandler(this.bShowFancy_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.SetColumnSpan(this.groupBox4, 2);
            this.groupBox4.Controls.Add(this.tableLayoutPanel1);
            this.groupBox4.Location = new System.Drawing.Point(3, 121);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(620, 256);
            this.groupBox4.TabIndex = 37;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Debris Check";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.pProj4, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.pProj3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.pProj1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pProj2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(614, 237);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // pProj4
            // 
            this.pProj4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pProj4.Location = new System.Drawing.Point(462, 3);
            this.pProj4.Name = "pProj4";
            this.pProj4.Size = new System.Drawing.Size(149, 231);
            this.pProj4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pProj4.TabIndex = 32;
            this.pProj4.TabStop = false;
            // 
            // pProj3
            // 
            this.pProj3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pProj3.Location = new System.Drawing.Point(309, 3);
            this.pProj3.Name = "pProj3";
            this.pProj3.Size = new System.Drawing.Size(147, 231);
            this.pProj3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pProj3.TabIndex = 31;
            this.pProj3.TabStop = false;
            // 
            // pProj1
            // 
            this.pProj1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pProj1.Location = new System.Drawing.Point(3, 3);
            this.pProj1.Name = "pProj1";
            this.pProj1.Size = new System.Drawing.Size(147, 231);
            this.pProj1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pProj1.TabIndex = 29;
            this.pProj1.TabStop = false;
            // 
            // pProj2
            // 
            this.pProj2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pProj2.Location = new System.Drawing.Point(156, 3);
            this.pProj2.Name = "pProj2";
            this.pProj2.Size = new System.Drawing.Size(147, 231);
            this.pProj2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pProj2.TabIndex = 30;
            this.pProj2.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.pPP0, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ppQuarter, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.pPPLast, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.ppHalf, 1, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(313, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(310, 112);
            this.tableLayoutPanel2.TabIndex = 38;
            // 
            // pPP0
            // 
            this.pPP0.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pPP0.Location = new System.Drawing.Point(3, 3);
            this.pPP0.Name = "pPP0";
            this.pPP0.Size = new System.Drawing.Size(149, 50);
            this.pPP0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pPP0.TabIndex = 7;
            this.pPP0.TabStop = false;
            this.pPP0.Click += new System.EventHandler(this.pPP0_Click);
            // 
            // ppQuarter
            // 
            this.ppQuarter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ppQuarter.Location = new System.Drawing.Point(158, 3);
            this.ppQuarter.Name = "ppQuarter";
            this.ppQuarter.Size = new System.Drawing.Size(149, 50);
            this.ppQuarter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ppQuarter.TabIndex = 16;
            this.ppQuarter.TabStop = false;
            // 
            // pPPLast
            // 
            this.pPPLast.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pPPLast.Location = new System.Drawing.Point(3, 59);
            this.pPPLast.Name = "pPPLast";
            this.pPPLast.Size = new System.Drawing.Size(149, 50);
            this.pPPLast.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pPPLast.TabIndex = 8;
            this.pPPLast.TabStop = false;
            this.pPPLast.Click += new System.EventHandler(this.pPPLast_Click);
            // 
            // ppHalf
            // 
            this.ppHalf.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ppHalf.Location = new System.Drawing.Point(158, 59);
            this.ppHalf.Name = "ppHalf";
            this.ppHalf.Size = new System.Drawing.Size(149, 50);
            this.ppHalf.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ppHalf.TabIndex = 17;
            this.ppHalf.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel3.Controls.Add(this.pCentering, 2, 3);
            this.tableLayoutPanel3.Controls.Add(this.pReconX, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.pREcon2, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.pMIP, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.pStack, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.pReconMine, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.label8, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label18, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label19, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.label20, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.label21, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label22, 2, 2);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(629, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel4.SetRowSpan(this.tableLayoutPanel3, 2);
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(468, 374);
            this.tableLayoutPanel3.TabIndex = 40;
            // 
            // pCentering
            // 
            this.pCentering.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pCentering.Location = new System.Drawing.Point(314, 205);
            this.pCentering.Name = "pCentering";
            this.pCentering.Size = new System.Drawing.Size(151, 166);
            this.pCentering.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pCentering.TabIndex = 36;
            this.pCentering.TabStop = false;
            // 
            // pReconX
            // 
            this.pReconX.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pReconX.Location = new System.Drawing.Point(3, 18);
            this.pReconX.Name = "pReconX";
            this.pReconX.Size = new System.Drawing.Size(149, 166);
            this.pReconX.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pReconX.TabIndex = 25;
            this.pReconX.TabStop = false;
            // 
            // pREcon2
            // 
            this.pREcon2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pREcon2.Location = new System.Drawing.Point(158, 18);
            this.pREcon2.Name = "pREcon2";
            this.pREcon2.Size = new System.Drawing.Size(150, 166);
            this.pREcon2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pREcon2.TabIndex = 33;
            this.pREcon2.TabStop = false;
            // 
            // pMIP
            // 
            this.pMIP.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pMIP.Location = new System.Drawing.Point(314, 18);
            this.pMIP.Name = "pMIP";
            this.pMIP.Size = new System.Drawing.Size(151, 166);
            this.pMIP.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pMIP.TabIndex = 20;
            this.pMIP.TabStop = false;
            // 
            // pStack
            // 
            this.pStack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pStack.Location = new System.Drawing.Point(158, 205);
            this.pStack.Name = "pStack";
            this.pStack.Size = new System.Drawing.Size(150, 166);
            this.pStack.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pStack.TabIndex = 26;
            this.pStack.TabStop = false;
            // 
            // pReconMine
            // 
            this.pReconMine.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pReconMine.Location = new System.Drawing.Point(3, 205);
            this.pReconMine.Name = "pReconMine";
            this.pReconMine.Size = new System.Drawing.Size(149, 166);
            this.pReconMine.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pReconMine.TabIndex = 37;
            this.pReconMine.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "Axial Slice";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(158, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(52, 13);
            this.label18.TabIndex = 39;
            this.label18.Text = "Top Slice";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(314, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(26, 13);
            this.label19.TabIndex = 40;
            this.label19.Text = "MIP";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 187);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(54, 13);
            this.label20.TabIndex = 41;
            this.label20.Text = "Side Slice";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(158, 187);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(73, 13);
            this.label21.TabIndex = 42;
            this.label21.Text = "VG Axial Slice";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(314, 187);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(101, 13);
            this.label22.TabIndex = 43;
            this.label22.Text = "Centering PP Movie";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(646, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(119, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "Recon Views (spin axis)";
            // 
            // pReconVG
            // 
            this.pReconVG.Location = new System.Drawing.Point(142, 38);
            this.pReconVG.Name = "pReconVG";
            this.pReconVG.Size = new System.Drawing.Size(192, 178);
            this.pReconVG.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pReconVG.TabIndex = 19;
            this.pReconVG.TabStop = false;
            this.pReconVG.Visible = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(835, 95);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(39, 13);
            this.label17.TabIndex = 34;
            this.label17.Text = "Recon";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(1069, 411);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(35, 13);
            this.label15.TabIndex = 28;
            this.label15.Text = "Stack";
            this.label15.Visible = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(628, 95);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(39, 13);
            this.label14.TabIndex = 27;
            this.label14.Text = "Recon";
            // 
            // lVG
            // 
            this.lVG.AutoSize = true;
            this.lVG.Location = new System.Drawing.Point(1050, 250);
            this.lVG.Name = "lVG";
            this.lVG.Size = new System.Drawing.Size(54, 13);
            this.lVG.TabIndex = 24;
            this.lVG.Text = "VGRecon";
            this.lVG.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(326, 6);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(26, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "PPs";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1031, 95);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(26, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "MIP";
            // 
            // pp3Quarters
            // 
            this.pp3Quarters.Location = new System.Drawing.Point(311, 468);
            this.pp3Quarters.Name = "pp3Quarters";
            this.pp3Quarters.Size = new System.Drawing.Size(169, 137);
            this.pp3Quarters.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pp3Quarters.TabIndex = 18;
            this.pp3Quarters.TabStop = false;
            this.pp3Quarters.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressBar1.Location = new System.Drawing.Point(12, 842);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(283, 20);
            this.progressBar1.TabIndex = 9;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.profileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1444, 24);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // profileToolStripMenuItem
            // 
            this.profileToolStripMenuItem.Name = "profileToolStripMenuItem";
            this.profileToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.profileToolStripMenuItem.Text = "Profile";
            // 
            // timer1
            // 
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lDataDirectories
            // 
            this.lDataDirectories.AllowUserToAddRows = false;
            this.lDataDirectories.AllowUserToDeleteRows = false;
            this.lDataDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.lDataDirectories.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.lDataDirectories.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lDataDirectories.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Dataset_Name,
            this.ReconSucceeded,
            this.Registration_Quality,
            this.Cell_Staining,
            this.Focus_Quality,
            this.Interfering_Object,
            this.Good_Cell,
            this.TooClose,
            this.Interesting,
            this.Comments,
            this.Recon_Quality,
            this.Noise,
            this.Rings,
            this.Run_Time,
            this.Background,
            this.Number_Quality});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.lDataDirectories.DefaultCellStyle = dataGridViewCellStyle2;
            this.lDataDirectories.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.lDataDirectories.Location = new System.Drawing.Point(6, 15);
            this.lDataDirectories.MultiSelect = false;
            this.lDataDirectories.Name = "lDataDirectories";
            this.lDataDirectories.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.lDataDirectories.Size = new System.Drawing.Size(1104, 391);
            this.lDataDirectories.TabIndex = 19;
            this.lDataDirectories.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.lDataDirectories_CellValueChanged);
            this.lDataDirectories.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.lDataDirectories_DataError);
            this.lDataDirectories.SelectionChanged += new System.EventHandler(this.lDataDirectories_SelectionChanged);
            this.lDataDirectories.Leave += new System.EventHandler(this.lDataDirectories_Leave);
            // 
            // Dataset_Name
            // 
            this.Dataset_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Dataset_Name.HeaderText = "Dataset Name";
            this.Dataset_Name.Name = "Dataset_Name";
            this.Dataset_Name.ReadOnly = true;
            this.Dataset_Name.Width = 92;
            // 
            // ReconSucceeded
            // 
            this.ReconSucceeded.HeaderText = "Recon Succeeded";
            this.ReconSucceeded.Name = "ReconSucceeded";
            this.ReconSucceeded.ReadOnly = true;
            // 
            // Registration_Quality
            // 
            this.Registration_Quality.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Registration_Quality.HeaderText = "Registration Quality";
            this.Registration_Quality.Items.AddRange(new object[] {
            "OK",
            "Bad",
            "Questionable",
            "-"});
            this.Registration_Quality.Name = "Registration_Quality";
            this.Registration_Quality.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Registration_Quality.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Cell_Staining
            // 
            this.Cell_Staining.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cell_Staining.HeaderText = "Cell Staining";
            this.Cell_Staining.Items.AddRange(new object[] {
            "-",
            "OK",
            "Bad",
            "Questionable"});
            this.Cell_Staining.Name = "Cell_Staining";
            this.Cell_Staining.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Cell_Staining.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Focus_Quality
            // 
            this.Focus_Quality.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Focus_Quality.HeaderText = "Focus Quality";
            this.Focus_Quality.Items.AddRange(new object[] {
            "-",
            "OK",
            "Bad",
            "Questionable"});
            this.Focus_Quality.Name = "Focus_Quality";
            this.Focus_Quality.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Focus_Quality.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Interfering_Object
            // 
            this.Interfering_Object.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Interfering_Object.HeaderText = "Interfering Object";
            this.Interfering_Object.Items.AddRange(new object[] {
            "-",
            "Yes",
            "No"});
            this.Interfering_Object.Name = "Interfering_Object";
            this.Interfering_Object.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Interfering_Object.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Good_Cell
            // 
            this.Good_Cell.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Good_Cell.HeaderText = "Good Cell";
            this.Good_Cell.Items.AddRange(new object[] {
            "-",
            "Yes",
            "No"});
            this.Good_Cell.Name = "Good_Cell";
            this.Good_Cell.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Good_Cell.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // TooClose
            // 
            this.TooClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TooClose.HeaderText = "Too Close To Edge";
            this.TooClose.Items.AddRange(new object[] {
            "-",
            "Yes",
            "No"});
            this.TooClose.Name = "TooClose";
            this.TooClose.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.TooClose.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Interesting
            // 
            this.Interesting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Interesting.HeaderText = "Interesting Cell";
            this.Interesting.Items.AddRange(new object[] {
            "No",
            "Yes",
            "Mitotic"});
            this.Interesting.Name = "Interesting";
            this.Interesting.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Interesting.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Comments
            // 
            this.Comments.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.Width = 81;
            // 
            // Recon_Quality
            // 
            this.Recon_Quality.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Recon_Quality.HeaderText = "ReconQuality";
            this.Recon_Quality.Items.AddRange(new object[] {
            "-",
            "Good",
            "Bad"});
            this.Recon_Quality.Name = "Recon_Quality";
            this.Recon_Quality.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Recon_Quality.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Noise
            // 
            this.Noise.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Noise.HeaderText = "Noise";
            this.Noise.Items.AddRange(new object[] {
            "No",
            "Yes"});
            this.Noise.Name = "Noise";
            this.Noise.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Noise.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Rings
            // 
            this.Rings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Rings.HeaderText = "Rings";
            this.Rings.Items.AddRange(new object[] {
            "No",
            "Yes"});
            this.Rings.Name = "Rings";
            this.Rings.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Rings.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Run_Time
            // 
            this.Run_Time.HeaderText = "Run Time";
            this.Run_Time.Name = "Run_Time";
            this.Run_Time.ReadOnly = true;
            // 
            // Background
            // 
            this.Background.HeaderText = "Background";
            this.Background.Name = "Background";
            this.Background.ReadOnly = true;
            // 
            // Number_Quality
            // 
            this.Number_Quality.HeaderText = "Number Quality";
            this.Number_Quality.Name = "Number_Quality";
            // 
            // timerCenterMovie
            // 
            this.timerCenterMovie.Enabled = true;
            this.timerCenterMovie.Interval = 1000;
            this.timerCenterMovie.Tick += new System.EventHandler(this.timerCenterMovie_Tick);
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.lDataDirectories);
            this.groupBox6.Location = new System.Drawing.Point(3, 420);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(1116, 412);
            this.groupBox6.TabIndex = 34;
            this.groupBox6.TabStop = false;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.groupBox3, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.groupBox6, 0, 1);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(327, 27);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1122, 835);
            this.tableLayoutPanel5.TabIndex = 35;
            // 
            // ProcessGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1444, 874);
            this.Controls.Add(this.tableLayoutPanel5);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(1192, 726);
            this.Name = "ProcessGUI";
            this.Text = "EasyGUI";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProcessGUI_FormClosing);
            this.Load += new System.EventHandler(this.EasyGUI_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pProj4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pProj3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pProj1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pProj2)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pPP0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ppQuarter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pPPLast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ppHalf)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pCentering)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pReconX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pREcon2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pMIP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pStack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pReconMine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pReconVG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pp3Quarters)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lDataDirectories)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox bDataFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bTopLevelBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button bDataView;
        private System.Windows.Forms.Button bMIP;
        private System.Windows.Forms.Button bCenter;
        private System.Windows.Forms.Button bBackground;
        private System.Windows.Forms.Label Summary;
        private System.Windows.Forms.PictureBox pPPLast;
        private System.Windows.Forms.PictureBox pPP0;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem profileToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button bFlyThrough;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.RichTextBox lTextSummary;
        private System.Windows.Forms.Button bIntensity;
        private System.Windows.Forms.Button bFocusValue;
        private System.Windows.Forms.Button bShowStack;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox lDay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lMonth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lYear;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox LMachine;
        private System.Windows.Forms.DataGridView lDataDirectories;
        private System.Windows.Forms.PictureBox pMIP;
        private System.Windows.Forms.PictureBox pReconVG;
        private System.Windows.Forms.PictureBox pp3Quarters;
        private System.Windows.Forms.PictureBox ppHalf;
        private System.Windows.Forms.PictureBox ppQuarter;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.PictureBox pStack;
        private System.Windows.Forms.PictureBox pReconX;
        private System.Windows.Forms.TextBox tUserName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button bDataviewVG;
        private System.Windows.Forms.TextBox tVGDrive;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button bShowFancy;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox tDataInputFolder;
        private System.Windows.Forms.Button tBrowseDataFolder;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridViewTextBoxColumn CenteringQuality2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReconAverage;
        private System.Windows.Forms.PictureBox pProj3;
        private System.Windows.Forms.PictureBox pProj2;
        private System.Windows.Forms.PictureBox pProj1;
        private System.Windows.Forms.PictureBox pProj4;
        private System.Windows.Forms.Label lVG;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.PictureBox pREcon2;
        private System.Windows.Forms.Timer timerCenterMovie;
        private System.Windows.Forms.PictureBox pCentering;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.PictureBox pReconMine;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbLabeledDrive;
        private System.Windows.Forms.DataGridViewTextBoxColumn Dataset_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReconSucceeded;
        private System.Windows.Forms.DataGridViewComboBoxColumn Registration_Quality;
        private System.Windows.Forms.DataGridViewComboBoxColumn Cell_Staining;
        private System.Windows.Forms.DataGridViewComboBoxColumn Focus_Quality;
        private System.Windows.Forms.DataGridViewComboBoxColumn Interfering_Object;
        private System.Windows.Forms.DataGridViewComboBoxColumn Good_Cell;
        private System.Windows.Forms.DataGridViewComboBoxColumn TooClose;
        private System.Windows.Forms.DataGridViewComboBoxColumn Interesting;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewComboBoxColumn Recon_Quality;
        private System.Windows.Forms.DataGridViewComboBoxColumn Noise;
        private System.Windows.Forms.DataGridViewComboBoxColumn Rings;
        private System.Windows.Forms.DataGridViewTextBoxColumn Run_Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Background;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number_Quality;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
    }
}