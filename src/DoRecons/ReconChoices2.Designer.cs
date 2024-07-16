namespace DoRecons
{
    partial class ReconChoices2
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rPPNone = new System.Windows.Forms.RadioButton();
            this.nPPRadius = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.rPPClosing = new System.Windows.Forms.RadioButton();
            this.rPPOpening = new System.Windows.Forms.RadioButton();
            this.rPPAlphaTrimmed = new System.Windows.Forms.RadioButton();
            this.rPPAverage = new System.Windows.Forms.RadioButton();
            this.rPPMedian = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.cSaveMIP = new System.Windows.Forms.CheckBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.rVolumeRawCCT = new System.Windows.Forms.RadioButton();
            this.rVolumeRawInt = new System.Windows.Forms.RadioButton();
            this.rVolumeRawFloat = new System.Windows.Forms.RadioButton();
            this.rVolumeRawDouble = new System.Windows.Forms.RadioButton();
            this.cSaveVolume = new System.Windows.Forms.CheckBox();
            this.cSave16bit = new System.Windows.Forms.CheckBox();
            this.cSave8bit = new System.Windows.Forms.CheckBox();
            this.cSaveCenteringMovie = new System.Windows.Forms.CheckBox();
            this.cSaveCenteredImages = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.nARTAngle = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.rART = new System.Windows.Forms.RadioButton();
            this.rFBP = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cFBPMedian = new System.Windows.Forms.CheckBox();
            this.nFBPResolution = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lFBPWindowing = new System.Windows.Forms.ListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rCenteringMask = new System.Windows.Forms.RadioButton();
            this.rCenteringStrips = new System.Windows.Forms.RadioButton();
            this.rCenteringTop = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rMaskCOG = new System.Windows.Forms.RadioButton();
            this.rThresholdCOG = new System.Windows.Forms.RadioButton();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nPPRadius)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nARTAngle)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nFBPResolution)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rPPNone);
            this.groupBox2.Controls.Add(this.nPPRadius);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.rPPClosing);
            this.groupBox2.Controls.Add(this.rPPOpening);
            this.groupBox2.Controls.Add(this.rPPAlphaTrimmed);
            this.groupBox2.Controls.Add(this.rPPAverage);
            this.groupBox2.Controls.Add(this.rPPMedian);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(18, 246);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(285, 170);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Preprocessing";
            // 
            // rPPNone
            // 
            this.rPPNone.AutoSize = true;
            this.rPPNone.Location = new System.Drawing.Point(7, 20);
            this.rPPNone.Name = "rPPNone";
            this.rPPNone.Size = new System.Drawing.Size(55, 19);
            this.rPPNone.TabIndex = 7;
            this.rPPNone.Text = "None";
            this.rPPNone.UseVisualStyleBackColor = true;
            // 
            // nPPRadius
            // 
            this.nPPRadius.Location = new System.Drawing.Point(154, 55);
            this.nPPRadius.Name = "nPPRadius";
            this.nPPRadius.Size = new System.Drawing.Size(120, 21);
            this.nPPRadius.TabIndex = 6;
            this.nPPRadius.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(151, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Radius";
            // 
            // rPPClosing
            // 
            this.rPPClosing.AutoSize = true;
            this.rPPClosing.Location = new System.Drawing.Point(7, 139);
            this.rPPClosing.Name = "rPPClosing";
            this.rPPClosing.Size = new System.Drawing.Size(66, 19);
            this.rPPClosing.TabIndex = 4;
            this.rPPClosing.Text = "Closing";
            this.rPPClosing.UseVisualStyleBackColor = true;
            // 
            // rPPOpening
            // 
            this.rPPOpening.AutoSize = true;
            this.rPPOpening.Location = new System.Drawing.Point(7, 116);
            this.rPPOpening.Name = "rPPOpening";
            this.rPPOpening.Size = new System.Drawing.Size(72, 19);
            this.rPPOpening.TabIndex = 3;
            this.rPPOpening.Text = "Opening";
            this.rPPOpening.UseVisualStyleBackColor = true;
            // 
            // rPPAlphaTrimmed
            // 
            this.rPPAlphaTrimmed.AutoSize = true;
            this.rPPAlphaTrimmed.Cursor = System.Windows.Forms.Cursors.Default;
            this.rPPAlphaTrimmed.Location = new System.Drawing.Point(7, 92);
            this.rPPAlphaTrimmed.Name = "rPPAlphaTrimmed";
            this.rPPAlphaTrimmed.Size = new System.Drawing.Size(109, 19);
            this.rPPAlphaTrimmed.TabIndex = 2;
            this.rPPAlphaTrimmed.Text = "Alpha Trimmed";
            this.rPPAlphaTrimmed.UseVisualStyleBackColor = true;
            // 
            // rPPAverage
            // 
            this.rPPAverage.AutoSize = true;
            this.rPPAverage.Location = new System.Drawing.Point(7, 68);
            this.rPPAverage.Name = "rPPAverage";
            this.rPPAverage.Size = new System.Drawing.Size(69, 19);
            this.rPPAverage.TabIndex = 1;
            this.rPPAverage.Text = "Average";
            this.rPPAverage.UseVisualStyleBackColor = true;
            // 
            // rPPMedian
            // 
            this.rPPMedian.AutoSize = true;
            this.rPPMedian.Checked = true;
            this.rPPMedian.Location = new System.Drawing.Point(7, 44);
            this.rPPMedian.Name = "rPPMedian";
            this.rPPMedian.Size = new System.Drawing.Size(97, 19);
            this.rPPMedian.TabIndex = 0;
            this.rPPMedian.TabStop = true;
            this.rPPMedian.Text = "Median Filter";
            this.rPPMedian.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox8);
            this.groupBox3.Controls.Add(this.groupBox6);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(958, 1025);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Recon Workflow";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.cSaveMIP);
            this.groupBox8.Controls.Add(this.groupBox9);
            this.groupBox8.Controls.Add(this.cSaveVolume);
            this.groupBox8.Controls.Add(this.cSave16bit);
            this.groupBox8.Controls.Add(this.cSave8bit);
            this.groupBox8.Controls.Add(this.cSaveCenteringMovie);
            this.groupBox8.Controls.Add(this.cSaveCenteredImages);
            this.groupBox8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox8.Location = new System.Drawing.Point(309, 22);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(327, 903);
            this.groupBox8.TabIndex = 17;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Reporting";
            // 
            // cSaveMIP
            // 
            this.cSaveMIP.AutoSize = true;
            this.cSaveMIP.Checked = true;
            this.cSaveMIP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cSaveMIP.Location = new System.Drawing.Point(19, 317);
            this.cSaveMIP.Name = "cSaveMIP";
            this.cSaveMIP.Size = new System.Drawing.Size(123, 19);
            this.cSaveMIP.TabIndex = 20;
            this.cSaveMIP.Text = "Create MIP Movie";
            this.cSaveMIP.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.rVolumeRawCCT);
            this.groupBox9.Controls.Add(this.rVolumeRawInt);
            this.groupBox9.Controls.Add(this.rVolumeRawFloat);
            this.groupBox9.Controls.Add(this.rVolumeRawDouble);
            this.groupBox9.Location = new System.Drawing.Point(42, 169);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(253, 129);
            this.groupBox9.TabIndex = 19;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Volume Format";
            // 
            // rVolumeRawCCT
            // 
            this.rVolumeRawCCT.AutoSize = true;
            this.rVolumeRawCCT.Location = new System.Drawing.Point(7, 99);
            this.rVolumeRawCCT.Name = "rVolumeRawCCT";
            this.rVolumeRawCCT.Size = new System.Drawing.Size(97, 19);
            this.rVolumeRawCCT.TabIndex = 3;
            this.rVolumeRawCCT.Text = "CCT (double)";
            this.rVolumeRawCCT.UseVisualStyleBackColor = true;
            // 
            // rVolumeRawInt
            // 
            this.rVolumeRawInt.AutoSize = true;
            this.rVolumeRawInt.Location = new System.Drawing.Point(7, 72);
            this.rVolumeRawInt.Name = "rVolumeRawInt";
            this.rVolumeRawInt.Size = new System.Drawing.Size(159, 19);
            this.rVolumeRawInt.TabIndex = 2;
            this.rVolumeRawInt.Text = "Raw (INT with clamping)";
            this.rVolumeRawInt.UseVisualStyleBackColor = true;
            // 
            // rVolumeRawFloat
            // 
            this.rVolumeRawFloat.AutoSize = true;
            this.rVolumeRawFloat.Location = new System.Drawing.Point(7, 46);
            this.rVolumeRawFloat.Name = "rVolumeRawFloat";
            this.rVolumeRawFloat.Size = new System.Drawing.Size(76, 19);
            this.rVolumeRawFloat.TabIndex = 1;
            this.rVolumeRawFloat.Text = "Raw float";
            this.rVolumeRawFloat.UseVisualStyleBackColor = true;
            // 
            // rVolumeRawDouble
            // 
            this.rVolumeRawDouble.AutoSize = true;
            this.rVolumeRawDouble.Checked = true;
            this.rVolumeRawDouble.Location = new System.Drawing.Point(7, 21);
            this.rVolumeRawDouble.Name = "rVolumeRawDouble";
            this.rVolumeRawDouble.Size = new System.Drawing.Size(91, 19);
            this.rVolumeRawDouble.TabIndex = 0;
            this.rVolumeRawDouble.TabStop = true;
            this.rVolumeRawDouble.Text = "Raw double";
            this.rVolumeRawDouble.UseVisualStyleBackColor = true;
            // 
            // cSaveVolume
            // 
            this.cSaveVolume.AutoSize = true;
            this.cSaveVolume.Checked = true;
            this.cSaveVolume.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cSaveVolume.Location = new System.Drawing.Point(19, 140);
            this.cSaveVolume.Name = "cSaveVolume";
            this.cSaveVolume.Size = new System.Drawing.Size(98, 19);
            this.cSaveVolume.TabIndex = 18;
            this.cSaveVolume.Text = "Save Volume";
            this.cSaveVolume.UseVisualStyleBackColor = true;
            // 
            // cSave16bit
            // 
            this.cSave16bit.AutoSize = true;
            this.cSave16bit.Location = new System.Drawing.Point(19, 115);
            this.cSave16bit.Name = "cSave16bit";
            this.cSave16bit.Size = new System.Drawing.Size(163, 19);
            this.cSave16bit.TabIndex = 17;
            this.cSave16bit.Text = "Save 16 bit Stack Images";
            this.cSave16bit.UseVisualStyleBackColor = true;
            // 
            // cSave8bit
            // 
            this.cSave8bit.AutoSize = true;
            this.cSave8bit.Location = new System.Drawing.Point(19, 90);
            this.cSave8bit.Name = "cSave8bit";
            this.cSave8bit.Size = new System.Drawing.Size(156, 19);
            this.cSave8bit.TabIndex = 16;
            this.cSave8bit.Text = "Save 8 bit Stack Images";
            this.cSave8bit.UseVisualStyleBackColor = true;
            // 
            // cSaveCenteringMovie
            // 
            this.cSaveCenteringMovie.AutoSize = true;
            this.cSaveCenteringMovie.Checked = true;
            this.cSaveCenteringMovie.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cSaveCenteringMovie.Location = new System.Drawing.Point(19, 46);
            this.cSaveCenteringMovie.Name = "cSaveCenteringMovie";
            this.cSaveCenteringMovie.Size = new System.Drawing.Size(156, 19);
            this.cSaveCenteringMovie.TabIndex = 15;
            this.cSaveCenteringMovie.Text = "Save Registration Video";
            this.cSaveCenteringMovie.UseVisualStyleBackColor = true;
            // 
            // cSaveCenteredImages
            // 
            this.cSaveCenteredImages.AutoSize = true;
            this.cSaveCenteredImages.Checked = true;
            this.cSaveCenteredImages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cSaveCenteredImages.Location = new System.Drawing.Point(19, 20);
            this.cSaveCenteredImages.Name = "cSaveCenteredImages";
            this.cSaveCenteredImages.Size = new System.Drawing.Size(150, 19);
            this.cSaveCenteredImages.TabIndex = 14;
            this.cSaveCenteredImages.Text = "Save Centered Images";
            this.cSaveCenteredImages.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.groupBox7);
            this.groupBox6.Controls.Add(this.rART);
            this.groupBox6.Controls.Add(this.rFBP);
            this.groupBox6.Controls.Add(this.groupBox5);
            this.groupBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.Location = new System.Drawing.Point(18, 422);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(285, 503);
            this.groupBox6.TabIndex = 16;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Reconstruction";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.nARTAngle);
            this.groupBox7.Controls.Add(this.label4);
            this.groupBox7.Location = new System.Drawing.Point(26, 421);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(247, 70);
            this.groupBox7.TabIndex = 18;
            this.groupBox7.TabStop = false;
            // 
            // nARTAngle
            // 
            this.nARTAngle.Location = new System.Drawing.Point(8, 32);
            this.nARTAngle.Name = "nARTAngle";
            this.nARTAngle.Size = new System.Drawing.Size(234, 21);
            this.nARTAngle.TabIndex = 1;
            this.nARTAngle.Value = new decimal(new int[] {
            72,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(157, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "Seperation Angle (degrees)";
            // 
            // rART
            // 
            this.rART.AutoSize = true;
            this.rART.Location = new System.Drawing.Point(7, 391);
            this.rART.Name = "rART";
            this.rART.Size = new System.Drawing.Size(48, 19);
            this.rART.TabIndex = 17;
            this.rART.Text = "ART";
            this.rART.UseVisualStyleBackColor = true;
            // 
            // rFBP
            // 
            this.rFBP.AutoSize = true;
            this.rFBP.Checked = true;
            this.rFBP.Location = new System.Drawing.Point(7, 30);
            this.rFBP.Name = "rFBP";
            this.rFBP.Size = new System.Drawing.Size(154, 19);
            this.rFBP.TabIndex = 16;
            this.rFBP.TabStop = true;
            this.rFBP.Text = "Filtered Back Projection";
            this.rFBP.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cFBPMedian);
            this.groupBox5.Controls.Add(this.nFBPResolution);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.lFBPWindowing);
            this.groupBox5.Location = new System.Drawing.Point(26, 53);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(248, 327);
            this.groupBox5.TabIndex = 15;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Filtering/Convolution";
            // 
            // cFBPMedian
            // 
            this.cFBPMedian.AutoSize = true;
            this.cFBPMedian.Location = new System.Drawing.Point(8, 304);
            this.cFBPMedian.Name = "cFBPMedian";
            this.cFBPMedian.Size = new System.Drawing.Size(172, 19);
            this.cFBPMedian.TabIndex = 10;
            this.cFBPMedian.Text = "Post Filtering Median Filter";
            this.cFBPMedian.UseVisualStyleBackColor = true;
            // 
            // nFBPResolution
            // 
            this.nFBPResolution.Location = new System.Drawing.Point(8, 37);
            this.nFBPResolution.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nFBPResolution.Name = "nFBPResolution";
            this.nFBPResolution.Size = new System.Drawing.Size(234, 21);
            this.nFBPResolution.TabIndex = 9;
            this.nFBPResolution.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Resolution";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Windowing Filter";
            // 
            // lFBPWindowing
            // 
            this.lFBPWindowing.FormattingEnabled = true;
            this.lFBPWindowing.ItemHeight = 15;
            this.lFBPWindowing.Location = new System.Drawing.Point(8, 86);
            this.lFBPWindowing.Name = "lFBPWindowing";
            this.lFBPWindowing.Size = new System.Drawing.Size(234, 199);
            this.lFBPWindowing.TabIndex = 6;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rCenteringMask);
            this.groupBox4.Controls.Add(this.rCenteringStrips);
            this.groupBox4.Controls.Add(this.rCenteringTop);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(18, 126);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(285, 112);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Background Subtraction";
            // 
            // rCenteringMask
            // 
            this.rCenteringMask.AutoSize = true;
            this.rCenteringMask.Location = new System.Drawing.Point(7, 68);
            this.rCenteringMask.Name = "rCenteringMask";
            this.rCenteringMask.Size = new System.Drawing.Size(55, 19);
            this.rCenteringMask.TabIndex = 2;
            this.rCenteringMask.Text = "Mask";
            this.rCenteringMask.UseVisualStyleBackColor = true;
            // 
            // rCenteringStrips
            // 
            this.rCenteringStrips.AutoSize = true;
            this.rCenteringStrips.Location = new System.Drawing.Point(7, 44);
            this.rCenteringStrips.Name = "rCenteringStrips";
            this.rCenteringStrips.Size = new System.Drawing.Size(56, 19);
            this.rCenteringStrips.TabIndex = 1;
            this.rCenteringStrips.Text = "Strips";
            this.rCenteringStrips.UseVisualStyleBackColor = true;
            // 
            // rCenteringTop
            // 
            this.rCenteringTop.AutoSize = true;
            this.rCenteringTop.Checked = true;
            this.rCenteringTop.Location = new System.Drawing.Point(7, 20);
            this.rCenteringTop.Name = "rCenteringTop";
            this.rCenteringTop.Size = new System.Drawing.Size(112, 19);
            this.rCenteringTop.TabIndex = 0;
            this.rCenteringTop.TabStop = true;
            this.rCenteringTop.Text = "Top and Bottom";
            this.rCenteringTop.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rMaskCOG);
            this.groupBox1.Controls.Add(this.rThresholdCOG);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(18, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(285, 93);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Center Of Gravity";
            // 
            // rMaskCOG
            // 
            this.rMaskCOG.AutoSize = true;
            this.rMaskCOG.Location = new System.Drawing.Point(7, 50);
            this.rMaskCOG.Name = "rMaskCOG";
            this.rMaskCOG.Size = new System.Drawing.Size(153, 19);
            this.rMaskCOG.TabIndex = 1;
            this.rMaskCOG.Text = "By Mask and Intensities";
            this.rMaskCOG.UseVisualStyleBackColor = true;
            // 
            // rThresholdCOG
            // 
            this.rThresholdCOG.AutoSize = true;
            this.rThresholdCOG.Checked = true;
            this.rThresholdCOG.Location = new System.Drawing.Point(7, 20);
            this.rThresholdCOG.Name = "rThresholdCOG";
            this.rThresholdCOG.Size = new System.Drawing.Size(123, 19);
            this.rThresholdCOG.TabIndex = 0;
            this.rThresholdCOG.TabStop = true;
            this.rThresholdCOG.Text = "By Threshold Only";
            this.rThresholdCOG.UseVisualStyleBackColor = true;
            // 
            // ReconChoices2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Name = "ReconChoices2";
            this.Size = new System.Drawing.Size(958, 1025);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nPPRadius)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nARTAngle)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nFBPResolution)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rPPNone;
        private System.Windows.Forms.NumericUpDown nPPRadius;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rPPClosing;
        private System.Windows.Forms.RadioButton rPPOpening;
        private System.Windows.Forms.RadioButton rPPAlphaTrimmed;
        private System.Windows.Forms.RadioButton rPPAverage;
        private System.Windows.Forms.RadioButton rPPMedian;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.CheckBox cSaveMIP;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.RadioButton rVolumeRawCCT;
        private System.Windows.Forms.RadioButton rVolumeRawInt;
        private System.Windows.Forms.RadioButton rVolumeRawFloat;
        private System.Windows.Forms.RadioButton rVolumeRawDouble;
        private System.Windows.Forms.CheckBox cSaveVolume;
        private System.Windows.Forms.CheckBox cSave16bit;
        private System.Windows.Forms.CheckBox cSave8bit;
        private System.Windows.Forms.CheckBox cSaveCenteringMovie;
        private System.Windows.Forms.CheckBox cSaveCenteredImages;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.NumericUpDown nARTAngle;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rART;
        private System.Windows.Forms.RadioButton rFBP;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cFBPMedian;
        private System.Windows.Forms.NumericUpDown nFBPResolution;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lFBPWindowing;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rCenteringMask;
        private System.Windows.Forms.RadioButton rCenteringStrips;
        private System.Windows.Forms.RadioButton rCenteringTop;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rMaskCOG;
        private System.Windows.Forms.RadioButton rThresholdCOG;
    }
}
