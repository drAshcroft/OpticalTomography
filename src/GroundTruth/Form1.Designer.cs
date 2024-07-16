namespace GroundTruth
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lPosition = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.clear = new System.Windows.Forms.Button();
            this.AddContour = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.steps = new System.Windows.Forms.NumericUpDown();
            this.lViewAxis = new System.Windows.Forms.ListBox();
            this.bNextCell = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lContourNames = new System.Windows.Forms.ListBox();
            this.Next = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.previous = new System.Windows.Forms.Button();
            this.viewerControl3D1 = new ImageViewer3D.ViewerControl3D();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lCells = new System.Windows.Forms.ListBox();
            this.bBrowse = new System.Windows.Forms.Button();
            this.tUserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openContoursToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveContoursToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveInterpolationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.linearTrianglesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linearLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.marchingCubesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.unorganizedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.delaunayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guassianSplatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoAllContoursToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instructionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.marchingCubesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linearTrianglesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.linearLinesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.marchingCubesToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.unorganizedPointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.delaunayToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.gaussianSplatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.timer1 = new System.Windows.Forms.Timer();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.steps)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1289, 797);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPage1.Controls.Add(this.lPosition);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.viewerControl3D1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1281, 771);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Normal";
            // 
            // lPosition
            // 
            this.lPosition.AutoSize = true;
            this.lPosition.Location = new System.Drawing.Point(936, 10);
            this.lPosition.Name = "lPosition";
            this.lPosition.Size = new System.Drawing.Size(56, 13);
            this.lPosition.TabIndex = 7;
            this.lPosition.Text = "Position: 0";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.clear);
            this.panel1.Controls.Add(this.AddContour);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.steps);
            this.panel1.Controls.Add(this.lViewAxis);
            this.panel1.Controls.Add(this.bNextCell);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.lContourNames);
            this.panel1.Controls.Add(this.Next);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.previous);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1198, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(80, 765);
            this.panel1.TabIndex = 6;
            // 
            // clear
            // 
            this.clear.Location = new System.Drawing.Point(4, 496);
            this.clear.Name = "clear";
            this.clear.Size = new System.Drawing.Size(70, 34);
            this.clear.TabIndex = 16;
            this.clear.Text = "Clear All Contours";
            this.clear.UseVisualStyleBackColor = true;
            this.clear.Click += new System.EventHandler(this.clear_Click);
            // 
            // AddContour
            // 
            this.AddContour.Location = new System.Drawing.Point(4, 455);
            this.AddContour.Name = "AddContour";
            this.AddContour.Size = new System.Drawing.Size(70, 34);
            this.AddContour.TabIndex = 15;
            this.AddContour.Text = "Add Contour";
            this.AddContour.UseVisualStyleBackColor = true;
            this.AddContour.Click += new System.EventHandler(this.AddContour_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 224);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Select View";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 408);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Slices To Skip";
            // 
            // steps
            // 
            this.steps.Location = new System.Drawing.Point(5, 429);
            this.steps.Name = "steps";
            this.steps.Size = new System.Drawing.Size(43, 20);
            this.steps.TabIndex = 1;
            this.steps.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // lViewAxis
            // 
            this.lViewAxis.FormattingEnabled = true;
            this.lViewAxis.Items.AddRange(new object[] {
            "All",
            "View 1",
            "View 2",
            "View 3"});
            this.lViewAxis.Location = new System.Drawing.Point(5, 240);
            this.lViewAxis.Name = "lViewAxis";
            this.lViewAxis.Size = new System.Drawing.Size(69, 56);
            this.lViewAxis.TabIndex = 13;
            this.lViewAxis.SelectedIndexChanged += new System.EventHandler(this.lViewBox_SelectedIndexChanged);
            // 
            // bNextCell
            // 
            this.bNextCell.Enabled = false;
            this.bNextCell.Location = new System.Drawing.Point(3, 553);
            this.bNextCell.Name = "bNextCell";
            this.bNextCell.Size = new System.Drawing.Size(71, 30);
            this.bNextCell.TabIndex = 12;
            this.bNextCell.Text = "Next Cell";
            this.bNextCell.UseVisualStyleBackColor = true;
            this.bNextCell.Click += new System.EventHandler(this.bNextCell_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 185);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 36);
            this.button1.TabIndex = 11;
            this.button1.Text = "Unlock Axis";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.UnlockAxis_Click);
            // 
            // lContourNames
            // 
            this.lContourNames.Enabled = false;
            this.lContourNames.FormattingEnabled = true;
            this.lContourNames.Items.AddRange(new object[] {
            "Cell Membrane",
            "Nucleus",
            "Nucleolus 1",
            "Nucleolus 2",
            "Nucleolus 3",
            "Nucleolus 4",
            "Nucleolus 5",
            "Nucleolus 6",
            "Nucleolus 7",
            "Nucleolus 8",
            "Nucleolus 9",
            "Nucleolus 10",
            "Nucleolus 11",
            "Nucleolus 12"});
            this.lContourNames.Location = new System.Drawing.Point(0, 19);
            this.lContourNames.Name = "lContourNames";
            this.lContourNames.Size = new System.Drawing.Size(77, 160);
            this.lContourNames.TabIndex = 3;
            this.lContourNames.SelectedIndexChanged += new System.EventHandler(this.lContourNames_SelectedIndexChanged);
            // 
            // Next
            // 
            this.Next.Enabled = false;
            this.Next.Location = new System.Drawing.Point(4, 357);
            this.Next.Name = "Next";
            this.Next.Size = new System.Drawing.Size(70, 34);
            this.Next.TabIndex = 5;
            this.Next.Text = "Start";
            this.Next.UseVisualStyleBackColor = true;
            this.Next.Click += new System.EventHandler(this.Next_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Current Contour:";
            // 
            // previous
            // 
            this.previous.Enabled = false;
            this.previous.Location = new System.Drawing.Point(4, 317);
            this.previous.Name = "previous";
            this.previous.Size = new System.Drawing.Size(70, 34);
            this.previous.TabIndex = 4;
            this.previous.Text = "Former Slice";
            this.previous.UseVisualStyleBackColor = true;
            this.previous.Click += new System.EventHandler(this.previous_Click);
            // 
            // viewerControl3D1
            // 
            this.viewerControl3D1.ActiveDrawingTool = null;
            this.viewerControl3D1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewerControl3D1.AutoScroll = true;
            this.viewerControl3D1.DrawingToolsVisible = true;
            this.viewerControl3D1.ExtraControl = null;
            this.viewerControl3D1.Location = new System.Drawing.Point(3, 6);
            this.viewerControl3D1.Name = "viewerControl3D1";
            this.viewerControl3D1.ProportionalZooming = true;
            this.viewerControl3D1.SelectedArea = null;
            this.viewerControl3D1.Size = new System.Drawing.Size(1189, 757);
            this.viewerControl3D1.SliceIndexX = 0;
            this.viewerControl3D1.SliceIndexY = 0;
            this.viewerControl3D1.SliceIndexZ = 0;
            this.viewerControl3D1.TabIndex = 1;
            this.viewerControl3D1.Zooming = false;
            this.viewerControl3D1.ZoomInterpolationMethod = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.viewerControl3D1.AxisUpdated += new ImageViewer3D.ViewerControl3D.AxisIndexUpdatedEvent(this.viewerControl3D1_AxisUpdated);
            this.viewerControl3D1.SelectionPerformed += new ImageViewer3D.SelectionPerformedEvent(this.viewerControl3D1_SelectionPerformed);
            this.viewerControl3D1.Load += new System.EventHandler(this.viewerControl3D1_Load);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lCells);
            this.tabPage2.Controls.Add(this.bBrowse);
            this.tabPage2.Controls.Add(this.tUserName);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.textBox1);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1281, 771);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Advanced";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lCells
            // 
            this.lCells.FormattingEnabled = true;
            this.lCells.Location = new System.Drawing.Point(336, 86);
            this.lCells.Name = "lCells";
            this.lCells.Size = new System.Drawing.Size(570, 485);
            this.lCells.TabIndex = 5;
            this.lCells.SelectedIndexChanged += new System.EventHandler(this.lCells_SelectedIndexChanged);
            // 
            // bBrowse
            // 
            this.bBrowse.Location = new System.Drawing.Point(564, 48);
            this.bBrowse.Name = "bBrowse";
            this.bBrowse.Size = new System.Drawing.Size(75, 23);
            this.bBrowse.TabIndex = 4;
            this.bBrowse.Text = "...";
            this.bBrowse.UseVisualStyleBackColor = true;
            this.bBrowse.Click += new System.EventHandler(this.bBrowse_Click);
            // 
            // tUserName
            // 
            this.tUserName.Location = new System.Drawing.Point(12, 102);
            this.tUserName.Name = "tUserName";
            this.tUserName.Size = new System.Drawing.Size(137, 20);
            this.tUserName.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "User Name";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(336, 48);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(221, 20);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "S:\\Research\\Cell CT\\Evaluation";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(333, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Top Folder";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.instructionsToolStripMenuItem,
            this.marchingCubesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1289, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openContoursToolStripMenuItem,
            this.saveContoursToolStripMenuItem,
            this.saveInterpolationToolStripMenuItem,
            this.redoAllContoursToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.openToolStripMenuItem.Text = "Open Volume";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openContoursToolStripMenuItem
            // 
            this.openContoursToolStripMenuItem.Name = "openContoursToolStripMenuItem";
            this.openContoursToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.openContoursToolStripMenuItem.Text = "Open Contours";
            this.openContoursToolStripMenuItem.Click += new System.EventHandler(this.openContoursToolStripMenuItem_Click);
            // 
            // saveContoursToolStripMenuItem
            // 
            this.saveContoursToolStripMenuItem.Name = "saveContoursToolStripMenuItem";
            this.saveContoursToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.saveContoursToolStripMenuItem.Text = "Save Contours";
            this.saveContoursToolStripMenuItem.Click += new System.EventHandler(this.saveContoursToolStripMenuItem_Click);
            // 
            // saveInterpolationToolStripMenuItem
            // 
            this.saveInterpolationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.linearTrianglesToolStripMenuItem,
            this.linearLinesToolStripMenuItem,
            this.marchingCubesToolStripMenuItem1,
            this.unorganizedToolStripMenuItem,
            this.delaunayToolStripMenuItem,
            this.guassianSplatToolStripMenuItem});
            this.saveInterpolationToolStripMenuItem.Name = "saveInterpolationToolStripMenuItem";
            this.saveInterpolationToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.saveInterpolationToolStripMenuItem.Text = "Save Interpolation";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(164, 22);
            this.toolStripMenuItem1.Text = "All Interpolations";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.AllInterpolationsMenuItem_Click);
            // 
            // linearTrianglesToolStripMenuItem
            // 
            this.linearTrianglesToolStripMenuItem.Name = "linearTrianglesToolStripMenuItem";
            this.linearTrianglesToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.linearTrianglesToolStripMenuItem.Text = "Linear Triangles";
            this.linearTrianglesToolStripMenuItem.Click += new System.EventHandler(this.linearTrianglesToolStripMenuItem_Click);
            // 
            // linearLinesToolStripMenuItem
            // 
            this.linearLinesToolStripMenuItem.Name = "linearLinesToolStripMenuItem";
            this.linearLinesToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.linearLinesToolStripMenuItem.Text = "Linear Lines";
            this.linearLinesToolStripMenuItem.Click += new System.EventHandler(this.linearLinesToolStripMenuItem_Click);
            // 
            // marchingCubesToolStripMenuItem1
            // 
            this.marchingCubesToolStripMenuItem1.Name = "marchingCubesToolStripMenuItem1";
            this.marchingCubesToolStripMenuItem1.Size = new System.Drawing.Size(164, 22);
            this.marchingCubesToolStripMenuItem1.Text = "Marching Cubes";
            this.marchingCubesToolStripMenuItem1.Click += new System.EventHandler(this.marchingCubesToolStripMenuItem1_Click_1);
            // 
            // unorganizedToolStripMenuItem
            // 
            this.unorganizedToolStripMenuItem.Name = "unorganizedToolStripMenuItem";
            this.unorganizedToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.unorganizedToolStripMenuItem.Text = "Unorganized";
            this.unorganizedToolStripMenuItem.Click += new System.EventHandler(this.unorganizedToolStripMenuItem_Click_1);
            // 
            // delaunayToolStripMenuItem
            // 
            this.delaunayToolStripMenuItem.Name = "delaunayToolStripMenuItem";
            this.delaunayToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.delaunayToolStripMenuItem.Text = "Delaunay";
            this.delaunayToolStripMenuItem.Click += new System.EventHandler(this.delaunayToolStripMenuItem_Click_1);
            // 
            // guassianSplatToolStripMenuItem
            // 
            this.guassianSplatToolStripMenuItem.Name = "guassianSplatToolStripMenuItem";
            this.guassianSplatToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.guassianSplatToolStripMenuItem.Text = "Guassian Splat";
            this.guassianSplatToolStripMenuItem.Click += new System.EventHandler(this.guassianSplatToolStripMenuItem_Click);
            // 
            // redoAllContoursToolStripMenuItem
            // 
            this.redoAllContoursToolStripMenuItem.Name = "redoAllContoursToolStripMenuItem";
            this.redoAllContoursToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.redoAllContoursToolStripMenuItem.Text = "Redo All Contours";
            this.redoAllContoursToolStripMenuItem.Click += new System.EventHandler(this.redoAllContoursToolStripMenuItem_Click);
            // 
            // instructionsToolStripMenuItem
            // 
            this.instructionsToolStripMenuItem.Name = "instructionsToolStripMenuItem";
            this.instructionsToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.instructionsToolStripMenuItem.Text = "Instructions";
            // 
            // marchingCubesToolStripMenuItem
            // 
            this.marchingCubesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.linearTrianglesToolStripMenuItem1,
            this.linearLinesToolStripMenuItem1,
            this.marchingCubesToolStripMenuItem2,
            this.unorganizedPointsToolStripMenuItem,
            this.delaunayToolStripMenuItem1,
            this.gaussianSplatToolStripMenuItem});
            this.marchingCubesToolStripMenuItem.Name = "marchingCubesToolStripMenuItem";
            this.marchingCubesToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.marchingCubesToolStripMenuItem.Text = "Interpolation";
            this.marchingCubesToolStripMenuItem.Click += new System.EventHandler(this.marchingCubesToolStripMenuItem_Click);
            // 
            // linearTrianglesToolStripMenuItem1
            // 
            this.linearTrianglesToolStripMenuItem1.Name = "linearTrianglesToolStripMenuItem1";
            this.linearTrianglesToolStripMenuItem1.Size = new System.Drawing.Size(177, 22);
            this.linearTrianglesToolStripMenuItem1.Text = "Linear Triangles";
            this.linearTrianglesToolStripMenuItem1.Click += new System.EventHandler(this.linearTrianglesToolStripMenuItem1_Click);
            // 
            // linearLinesToolStripMenuItem1
            // 
            this.linearLinesToolStripMenuItem1.Name = "linearLinesToolStripMenuItem1";
            this.linearLinesToolStripMenuItem1.Size = new System.Drawing.Size(177, 22);
            this.linearLinesToolStripMenuItem1.Text = "Linear Lines";
            this.linearLinesToolStripMenuItem1.Click += new System.EventHandler(this.linearLinesToolStripMenuItem1_Click);
            // 
            // marchingCubesToolStripMenuItem2
            // 
            this.marchingCubesToolStripMenuItem2.Name = "marchingCubesToolStripMenuItem2";
            this.marchingCubesToolStripMenuItem2.Size = new System.Drawing.Size(177, 22);
            this.marchingCubesToolStripMenuItem2.Text = "Marching Cubes";
            this.marchingCubesToolStripMenuItem2.Click += new System.EventHandler(this.marchingCubesToolStripMenuItem2_Click);
            // 
            // unorganizedPointsToolStripMenuItem
            // 
            this.unorganizedPointsToolStripMenuItem.Name = "unorganizedPointsToolStripMenuItem";
            this.unorganizedPointsToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.unorganizedPointsToolStripMenuItem.Text = "Unorganized Points";
            this.unorganizedPointsToolStripMenuItem.Click += new System.EventHandler(this.unorganizedPointsToolStripMenuItem_Click);
            // 
            // delaunayToolStripMenuItem1
            // 
            this.delaunayToolStripMenuItem1.Name = "delaunayToolStripMenuItem1";
            this.delaunayToolStripMenuItem1.Size = new System.Drawing.Size(177, 22);
            this.delaunayToolStripMenuItem1.Text = "Delaunay";
            this.delaunayToolStripMenuItem1.Click += new System.EventHandler(this.delaunayToolStripMenuItem1_Click);
            // 
            // gaussianSplatToolStripMenuItem
            // 
            this.gaussianSplatToolStripMenuItem.Name = "gaussianSplatToolStripMenuItem";
            this.gaussianSplatToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.gaussianSplatToolStripMenuItem.Text = "Gaussian Splat";
            this.gaussianSplatToolStripMenuItem.Click += new System.EventHandler(this.gaussianSplatToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1289, 821);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.Text = "Ground Truth";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.steps)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openContoursToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveContoursToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox lContourNames;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button previous;
        private ImageViewer3D.ViewerControl3D viewerControl3D1;
        private System.Windows.Forms.NumericUpDown steps;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox tUserName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox lCells;
        private System.Windows.Forms.Button bBrowse;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bNextCell;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lViewAxis;
        private System.Windows.Forms.ToolStripMenuItem instructionsToolStripMenuItem;
        private System.Windows.Forms.Button AddContour;
        private System.Windows.Forms.Button Next;
        private System.Windows.Forms.Button clear;
        private System.Windows.Forms.ToolStripMenuItem marchingCubesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveInterpolationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem linearTrianglesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem linearLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem marchingCubesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem unorganizedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem delaunayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guassianSplatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem linearTrianglesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem linearLinesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem marchingCubesToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem unorganizedPointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem delaunayToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem gaussianSplatToolStripMenuItem;
        private System.Windows.Forms.Label lPosition;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem redoAllContoursToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
    }
}

