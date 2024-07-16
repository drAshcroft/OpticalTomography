namespace Tomographic_Imaging_2
{
    partial class SimulationControls
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
            this.b3DPhantom = new System.Windows.Forms.Button();
            this.bDoSaveProjections = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nSlices = new System.Windows.Forms.NumericUpDown();
            this.bCreatePhantom = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nGridSize = new System.Windows.Forms.NumericUpDown();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.bLoadPhantom = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.bDoProjections = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nSlices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nGridSize)).BeginInit();
            this.SuspendLayout();
            // 
            // b3DPhantom
            // 
            this.b3DPhantom.Location = new System.Drawing.Point(141, 51);
            this.b3DPhantom.Name = "b3DPhantom";
            this.b3DPhantom.Size = new System.Drawing.Size(123, 40);
            this.b3DPhantom.TabIndex = 24;
            this.b3DPhantom.Text = "Create 3D Phantom";
            this.b3DPhantom.UseVisualStyleBackColor = true;
            this.b3DPhantom.Click += new System.EventHandler(this.b3DPhantom_Click);
            // 
            // bDoSaveProjections
            // 
            this.bDoSaveProjections.Location = new System.Drawing.Point(141, 161);
            this.bDoSaveProjections.Name = "bDoSaveProjections";
            this.bDoSaveProjections.Size = new System.Drawing.Size(123, 35);
            this.bDoSaveProjections.TabIndex = 23;
            this.bDoSaveProjections.Text = "Save Projections";
            this.bDoSaveProjections.UseVisualStyleBackColor = true;
            this.bDoSaveProjections.Click += new System.EventHandler(this.bDoSaveProjections_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Number Of Slices";
            // 
            // nSlices
            // 
            this.nSlices.Location = new System.Drawing.Point(12, 135);
            this.nSlices.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nSlices.Name = "nSlices";
            this.nSlices.Size = new System.Drawing.Size(123, 20);
            this.nSlices.TabIndex = 21;
            this.nSlices.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            // 
            // bCreatePhantom
            // 
            this.bCreatePhantom.Location = new System.Drawing.Point(12, 51);
            this.bCreatePhantom.Name = "bCreatePhantom";
            this.bCreatePhantom.Size = new System.Drawing.Size(123, 40);
            this.bCreatePhantom.TabIndex = 20;
            this.bCreatePhantom.Text = "Create 2D Phantom";
            this.bCreatePhantom.UseVisualStyleBackColor = true;
            this.bCreatePhantom.Click += new System.EventHandler(this.bCreatePhantom_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Grid Size";
            // 
            // nGridSize
            // 
            this.nGridSize.Location = new System.Drawing.Point(15, 25);
            this.nGridSize.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.nGridSize.Name = "nGridSize";
            this.nGridSize.Size = new System.Drawing.Size(123, 20);
            this.nGridSize.TabIndex = 26;
            this.nGridSize.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 213);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(252, 23);
            this.progressBar1.TabIndex = 27;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 242);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 38);
            this.button1.TabIndex = 28;
            this.button1.Text = "Create Math Sphere";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bLoadPhantom
            // 
            this.bLoadPhantom.Location = new System.Drawing.Point(141, 97);
            this.bLoadPhantom.Name = "bLoadPhantom";
            this.bLoadPhantom.Size = new System.Drawing.Size(123, 35);
            this.bLoadPhantom.TabIndex = 29;
            this.bLoadPhantom.Text = "Load Phantom";
            this.bLoadPhantom.UseVisualStyleBackColor = true;
            this.bLoadPhantom.Click += new System.EventHandler(this.bLoadPhantom_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // bDoProjections
            // 
            this.bDoProjections.Location = new System.Drawing.Point(12, 161);
            this.bDoProjections.Name = "bDoProjections";
            this.bDoProjections.Size = new System.Drawing.Size(123, 35);
            this.bDoProjections.TabIndex = 30;
            this.bDoProjections.Text = "Do Projections";
            this.bDoProjections.UseVisualStyleBackColor = true;
            this.bDoProjections.Click += new System.EventHandler(this.bDoProjections_Click);
            // 
            // SimulationControls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 603);
            this.Controls.Add(this.bDoProjections);
            this.Controls.Add(this.bLoadPhantom);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.nGridSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.b3DPhantom);
            this.Controls.Add(this.bDoSaveProjections);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nSlices);
            this.Controls.Add(this.bCreatePhantom);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SimulationControls";
            this.TabText = "Simulation Controls";
            this.Text = "Simulation Controls";
            this.Load += new System.EventHandler(this.SimulationControls_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nSlices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nGridSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button b3DPhantom;
        private System.Windows.Forms.Button bDoSaveProjections;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nSlices;
        private System.Windows.Forms.Button bCreatePhantom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nGridSize;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button bLoadPhantom;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button bDoProjections;
    }
}