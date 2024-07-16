namespace ImageViewer.Filters
{
    partial class CenterCellsTool2Form
    {

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
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.bLoadYArray = new System.Windows.Forms.Button();
            this.tbYArrayName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.bLoadX = new System.Windows.Forms.Button();
            this.tbXArrayName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudOrderX = new System.Windows.Forms.NumericUpDown();
            this.nudPeriodX = new System.Windows.Forms.NumericUpDown();
            this.rbTrigX = new System.Windows.Forms.RadioButton();
            this.rbPolynomialX = new System.Windows.Forms.RadioButton();
            this.rbMovingAverageX = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nudOrderY = new System.Windows.Forms.NumericUpDown();
            this.nudPeriodY = new System.Windows.Forms.NumericUpDown();
            this.rbTrigY = new System.Windows.Forms.RadioButton();
            this.rbPolynomialY = new System.Windows.Forms.RadioButton();
            this.rbMovingAverageY = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.zedgraphcontrol = new ZedGraph.ZedGraphControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.numCutWidth = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.nudCutHeight = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOrderX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPeriodX)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOrderY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPeriodY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCutWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(1688, 785);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(67, 35);
            this.button1.TabIndex = 25;
            this.button1.Text = "Done";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(666, 777);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 26;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Resize += new System.EventHandler(this.pictureBox1_Resize);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(2, 2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Controls.Add(this.zedgraphcontrol);
            this.splitContainer1.Size = new System.Drawing.Size(1753, 777);
            this.splitContainer1.SplitterDistance = 666;
            this.splitContainer1.TabIndex = 27;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.bLoadYArray);
            this.panel1.Controls.Add(this.tbYArrayName);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.bLoadX);
            this.panel1.Controls.Add(this.tbXArrayName);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.trackBar1);
            this.panel1.Location = new System.Drawing.Point(18, 557);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1057, 217);
            this.panel1.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(521, 198);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Y ImageData Array";
            // 
            // bLoadYArray
            // 
            this.bLoadYArray.Location = new System.Drawing.Point(760, 191);
            this.bLoadYArray.Name = "bLoadYArray";
            this.bLoadYArray.Size = new System.Drawing.Size(75, 23);
            this.bLoadYArray.TabIndex = 10;
            this.bLoadYArray.Text = "Load Y ImageData";
            this.bLoadYArray.UseVisualStyleBackColor = true;
            this.bLoadYArray.Click += new System.EventHandler(this.bLoadYArray_Click);
            // 
            // tbYArrayName
            // 
            this.tbYArrayName.Location = new System.Drawing.Point(623, 193);
            this.tbYArrayName.Name = "tbYArrayName";
            this.tbYArrayName.Size = new System.Drawing.Size(131, 20);
            this.tbYArrayName.TabIndex = 9;
            this.tbYArrayName.Text = "Y_Positions";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(12, 193);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "X ImageData Array";
            // 
            // bLoadX
            // 
            this.bLoadX.Location = new System.Drawing.Point(263, 188);
            this.bLoadX.Name = "bLoadX";
            this.bLoadX.Size = new System.Drawing.Size(75, 23);
            this.bLoadX.TabIndex = 7;
            this.bLoadX.Text = "Load X ImageData";
            this.bLoadX.UseVisualStyleBackColor = true;
            this.bLoadX.Click += new System.EventHandler(this.bLoadX_Click);
            // 
            // tbXArrayName
            // 
            this.tbXArrayName.Location = new System.Drawing.Point(114, 190);
            this.tbXArrayName.Name = "tbXArrayName";
            this.tbXArrayName.Size = new System.Drawing.Size(131, 20);
            this.tbXArrayName.TabIndex = 6;
            this.tbXArrayName.Text = "X_Positions";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.nudOrderX);
            this.groupBox1.Controls.Add(this.nudPeriodX);
            this.groupBox1.Controls.Add(this.rbTrigX);
            this.groupBox1.Controls.Add(this.rbPolynomialX);
            this.groupBox1.Controls.Add(this.rbMovingAverageX);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(6, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 121);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "X Curve Fit";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(214, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Order";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(214, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Period";
            // 
            // nudOrderX
            // 
            this.nudOrderX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudOrderX.Location = new System.Drawing.Point(257, 48);
            this.nudOrderX.Name = "nudOrderX";
            this.nudOrderX.Size = new System.Drawing.Size(39, 20);
            this.nudOrderX.TabIndex = 4;
            this.nudOrderX.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudOrderX.ValueChanged += new System.EventHandler(this.nudOrderX_ValueChanged);
            // 
            // nudPeriodX
            // 
            this.nudPeriodX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudPeriodX.Location = new System.Drawing.Point(257, 16);
            this.nudPeriodX.Name = "nudPeriodX";
            this.nudPeriodX.Size = new System.Drawing.Size(39, 20);
            this.nudPeriodX.TabIndex = 3;
            this.nudPeriodX.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudPeriodX.ValueChanged += new System.EventHandler(this.nudPeriodX_ValueChanged);
            // 
            // rbTrigX
            // 
            this.rbTrigX.AutoSize = true;
            this.rbTrigX.Location = new System.Drawing.Point(11, 88);
            this.rbTrigX.Name = "rbTrigX";
            this.rbTrigX.Size = new System.Drawing.Size(43, 17);
            this.rbTrigX.TabIndex = 2;
            this.rbTrigX.TabStop = true;
            this.rbTrigX.Text = "Trig";
            this.rbTrigX.UseVisualStyleBackColor = true;
            this.rbTrigX.CheckedChanged += new System.EventHandler(this.rbTrigX_CheckedChanged);
            // 
            // rbPolynomialX
            // 
            this.rbPolynomialX.AutoSize = true;
            this.rbPolynomialX.Location = new System.Drawing.Point(11, 51);
            this.rbPolynomialX.Name = "rbPolynomialX";
            this.rbPolynomialX.Size = new System.Drawing.Size(75, 17);
            this.rbPolynomialX.TabIndex = 1;
            this.rbPolynomialX.TabStop = true;
            this.rbPolynomialX.Text = "Polynomial";
            this.rbPolynomialX.UseVisualStyleBackColor = true;
            this.rbPolynomialX.CheckedChanged += new System.EventHandler(this.rbPolynomialX_CheckedChanged);
            // 
            // rbMovingAverageX
            // 
            this.rbMovingAverageX.AutoSize = true;
            this.rbMovingAverageX.Checked = true;
            this.rbMovingAverageX.Location = new System.Drawing.Point(11, 19);
            this.rbMovingAverageX.Name = "rbMovingAverageX";
            this.rbMovingAverageX.Size = new System.Drawing.Size(103, 17);
            this.rbMovingAverageX.TabIndex = 0;
            this.rbMovingAverageX.TabStop = true;
            this.rbMovingAverageX.Text = "Moving Average";
            this.rbMovingAverageX.UseVisualStyleBackColor = true;
            this.rbMovingAverageX.CheckedChanged += new System.EventHandler(this.rbMovingAverageX_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.nudOrderY);
            this.groupBox2.Controls.Add(this.nudPeriodY);
            this.groupBox2.Controls.Add(this.rbTrigY);
            this.groupBox2.Controls.Add(this.rbPolynomialY);
            this.groupBox2.Controls.Add(this.rbMovingAverageY);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(608, 66);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(288, 121);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Y Curve Fit";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(198, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Order";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(198, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Period";
            // 
            // nudOrderY
            // 
            this.nudOrderY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudOrderY.Location = new System.Drawing.Point(241, 48);
            this.nudOrderY.Name = "nudOrderY";
            this.nudOrderY.Size = new System.Drawing.Size(39, 20);
            this.nudOrderY.TabIndex = 8;
            this.nudOrderY.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudOrderY.ValueChanged += new System.EventHandler(this.nudOrderY_ValueChanged);
            // 
            // nudPeriodY
            // 
            this.nudPeriodY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudPeriodY.Location = new System.Drawing.Point(241, 16);
            this.nudPeriodY.Name = "nudPeriodY";
            this.nudPeriodY.Size = new System.Drawing.Size(39, 20);
            this.nudPeriodY.TabIndex = 7;
            this.nudPeriodY.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudPeriodY.ValueChanged += new System.EventHandler(this.nudPeriodY_ValueChanged);
            // 
            // rbTrigY
            // 
            this.rbTrigY.AutoSize = true;
            this.rbTrigY.Checked = true;
            this.rbTrigY.Location = new System.Drawing.Point(15, 88);
            this.rbTrigY.Name = "rbTrigY";
            this.rbTrigY.Size = new System.Drawing.Size(43, 17);
            this.rbTrigY.TabIndex = 4;
            this.rbTrigY.TabStop = true;
            this.rbTrigY.Text = "Trig";
            this.rbTrigY.UseVisualStyleBackColor = true;
            this.rbTrigY.CheckedChanged += new System.EventHandler(this.rbTrigY_CheckedChanged);
            // 
            // rbPolynomialY
            // 
            this.rbPolynomialY.AutoSize = true;
            this.rbPolynomialY.Location = new System.Drawing.Point(15, 51);
            this.rbPolynomialY.Name = "rbPolynomialY";
            this.rbPolynomialY.Size = new System.Drawing.Size(75, 17);
            this.rbPolynomialY.TabIndex = 3;
            this.rbPolynomialY.TabStop = true;
            this.rbPolynomialY.Text = "Polynomial";
            this.rbPolynomialY.UseVisualStyleBackColor = true;
            this.rbPolynomialY.CheckedChanged += new System.EventHandler(this.rbPolynomialY_CheckedChanged);
            // 
            // rbMovingAverageY
            // 
            this.rbMovingAverageY.AutoSize = true;
            this.rbMovingAverageY.Location = new System.Drawing.Point(15, 19);
            this.rbMovingAverageY.Name = "rbMovingAverageY";
            this.rbMovingAverageY.Size = new System.Drawing.Size(103, 17);
            this.rbMovingAverageY.TabIndex = 2;
            this.rbMovingAverageY.TabStop = true;
            this.rbMovingAverageY.Text = "Moving Average";
            this.rbMovingAverageY.UseVisualStyleBackColor = true;
            this.rbMovingAverageY.CheckedChanged += new System.EventHandler(this.rbMovingAverageY_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(21, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Curve Fit Effect";
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.Location = new System.Drawing.Point(24, 19);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(994, 45);
            this.trackBar1.TabIndex = 3;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // zedgraphcontrol
            // 
            this.zedgraphcontrol.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zedgraphcontrol.Location = new System.Drawing.Point(18, 10);
            this.zedgraphcontrol.Name = "zedgraphcontrol";
            this.zedgraphcontrol.ScrollGrace = 0D;
            this.zedgraphcontrol.ScrollMaxX = 0D;
            this.zedgraphcontrol.ScrollMaxY = 0D;
            this.zedgraphcontrol.ScrollMaxY2 = 0D;
            this.zedgraphcontrol.ScrollMinX = 0D;
            this.zedgraphcontrol.ScrollMinY = 0D;
            this.zedgraphcontrol.ScrollMinY2 = 0D;
            this.zedgraphcontrol.Size = new System.Drawing.Size(1041, 541);
            this.zedgraphcontrol.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(747, 784);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(106, 37);
            this.button2.TabIndex = 28;
            this.button2.Text = "Cut Centers";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(221, 798);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 13);
            this.label8.TabIndex = 31;
            this.label8.Text = "Cut Width";
            // 
            // numCutWidth
            // 
            this.numCutWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numCutWidth.Location = new System.Drawing.Point(281, 794);
            this.numCutWidth.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.numCutWidth.Name = "numCutWidth";
            this.numCutWidth.Size = new System.Drawing.Size(120, 20);
            this.numCutWidth.TabIndex = 32;
            this.numCutWidth.Value = new decimal(new int[] {
            170,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(407, 798);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 33;
            this.label9.Text = "Cut Height";
            // 
            // nudCutHeight
            // 
            this.nudCutHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudCutHeight.Location = new System.Drawing.Point(470, 794);
            this.nudCutHeight.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.nudCutHeight.Name = "nudCutHeight";
            this.nudCutHeight.Size = new System.Drawing.Size(120, 20);
            this.nudCutHeight.TabIndex = 34;
            this.nudCutHeight.Value = new decimal(new int[] {
            170,
            0,
            0,
            0});
            // 
            // CenterCellsTool2Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1759, 827);
            this.ControlBox = false;
            this.Controls.Add(this.nudCutHeight);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.numCutWidth);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.button1);
            this.Name = "CenterCellsTool2Form";
            this.Text = "ContrastTool";
            this.Resize += new System.EventHandler(this.CenterCellsTool2Form_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOrderX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPeriodX)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOrderY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPeriodY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCutWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Button button1;
        protected System.Windows.Forms.PictureBox pictureBox1;
        protected System.Windows.Forms.SplitContainer splitContainer1;
        private ZedGraph.ZedGraphControl zedgraphcontrol;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.RadioButton rbPolynomialX;
        private System.Windows.Forms.RadioButton rbMovingAverageX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudOrderY;
        private System.Windows.Forms.NumericUpDown nudPeriodY;
        private System.Windows.Forms.RadioButton rbTrigY;
        private System.Windows.Forms.RadioButton rbPolynomialY;
        private System.Windows.Forms.RadioButton rbMovingAverageY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudOrderX;
        private System.Windows.Forms.NumericUpDown nudPeriodX;
        private System.Windows.Forms.RadioButton rbTrigX;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button bLoadX;
        private System.Windows.Forms.TextBox tbXArrayName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button bLoadYArray;
        private System.Windows.Forms.TextBox tbYArrayName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numCutWidth;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nudCutHeight;
    }
}