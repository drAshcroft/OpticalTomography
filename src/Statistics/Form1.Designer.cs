namespace Statistics
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.tStorageDir = new System.Windows.Forms.TextBox();
            this.lDataDirectories = new System.Windows.Forms.DataGridView();
            this.Dataset_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReconSucceeded = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bLoadData = new System.Windows.Forms.Button();
            this.bCopy = new System.Windows.Forms.Button();
            this.bExcel = new System.Windows.Forms.Button();
            this.tExcelFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bAll = new System.Windows.Forms.Button();
            this.lExcelDone = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.lDataDirectories)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 138);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Base Data Folder";
            // 
            // tStorageDir
            // 
            this.tStorageDir.Location = new System.Drawing.Point(15, 159);
            this.tStorageDir.Name = "tStorageDir";
            this.tStorageDir.Size = new System.Drawing.Size(159, 20);
            this.tStorageDir.TabIndex = 1;
            this.tStorageDir.Text = "E:\\cct001";
            // 
            // lDataDirectories
            // 
            this.lDataDirectories.AllowUserToAddRows = false;
            this.lDataDirectories.AllowUserToDeleteRows = false;
            this.lDataDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.lDataDirectories.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.lDataDirectories.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lDataDirectories.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Dataset_Name,
            this.ReconSucceeded});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.lDataDirectories.DefaultCellStyle = dataGridViewCellStyle8;
            this.lDataDirectories.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.lDataDirectories.Location = new System.Drawing.Point(192, 9);
            this.lDataDirectories.MultiSelect = false;
            this.lDataDirectories.Name = "lDataDirectories";
            this.lDataDirectories.ReadOnly = true;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.lDataDirectories.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.lDataDirectories.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.lDataDirectories.Size = new System.Drawing.Size(1175, 923);
            this.lDataDirectories.TabIndex = 20;
            // 
            // Dataset_Name
            // 
            this.Dataset_Name.HeaderText = "Dataset Name";
            this.Dataset_Name.Name = "Dataset_Name";
            this.Dataset_Name.ReadOnly = true;
            // 
            // ReconSucceeded
            // 
            this.ReconSucceeded.HeaderText = "Recon Succeeded";
            this.ReconSucceeded.Name = "ReconSucceeded";
            this.ReconSucceeded.ReadOnly = true;
            // 
            // bLoadData
            // 
            this.bLoadData.Location = new System.Drawing.Point(78, 192);
            this.bLoadData.Name = "bLoadData";
            this.bLoadData.Size = new System.Drawing.Size(93, 26);
            this.bLoadData.TabIndex = 21;
            this.bLoadData.Text = "Load";
            this.bLoadData.UseVisualStyleBackColor = true;
            this.bLoadData.Click += new System.EventHandler(this.bLoadData_Click);
            // 
            // bCopy
            // 
            this.bCopy.Location = new System.Drawing.Point(47, 788);
            this.bCopy.Name = "bCopy";
            this.bCopy.Size = new System.Drawing.Size(110, 37);
            this.bCopy.TabIndex = 22;
            this.bCopy.Text = "Copy";
            this.bCopy.UseVisualStyleBackColor = true;
            this.bCopy.Click += new System.EventHandler(this.bCopy_Click);
            // 
            // bExcel
            // 
            this.bExcel.Location = new System.Drawing.Point(78, 69);
            this.bExcel.Name = "bExcel";
            this.bExcel.Size = new System.Drawing.Size(93, 26);
            this.bExcel.TabIndex = 23;
            this.bExcel.Text = "Read Excel";
            this.bExcel.UseVisualStyleBackColor = true;
            this.bExcel.Click += new System.EventHandler(this.bExcel_Click);
            // 
            // tExcelFile
            // 
            this.tExcelFile.Location = new System.Drawing.Point(13, 43);
            this.tExcelFile.Name = "tExcelFile";
            this.tExcelFile.Size = new System.Drawing.Size(158, 20);
            this.tExcelFile.TabIndex = 24;
            this.tExcelFile.Text = "C:\\Development\\Unsorted_data-collection-log.xls";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Excel File";
            // 
            // bAll
            // 
            this.bAll.Location = new System.Drawing.Point(12, 275);
            this.bAll.Name = "bAll";
            this.bAll.Size = new System.Drawing.Size(159, 26);
            this.bAll.TabIndex = 26;
            this.bAll.Text = "Read Excel and Data";
            this.bAll.UseVisualStyleBackColor = true;
            this.bAll.Click += new System.EventHandler(this.bAll_Click);
            // 
            // lExcelDone
            // 
            this.lExcelDone.AutoSize = true;
            this.lExcelDone.Location = new System.Drawing.Point(76, 98);
            this.lExcelDone.Name = "lExcelDone";
            this.lExcelDone.Size = new System.Drawing.Size(19, 13);
            this.lExcelDone.TabIndex = 27;
            this.lExcelDone.Text = "__";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(47, 831);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 37);
            this.button1.TabIndex = 28;
            this.button1.Text = "Copy";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1379, 944);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lExcelDone);
            this.Controls.Add(this.bAll);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tExcelFile);
            this.Controls.Add(this.bExcel);
            this.Controls.Add(this.bCopy);
            this.Controls.Add(this.bLoadData);
            this.Controls.Add(this.lDataDirectories);
            this.Controls.Add(this.tStorageDir);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.lDataDirectories)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tStorageDir;
        private System.Windows.Forms.DataGridView lDataDirectories;
        private System.Windows.Forms.DataGridViewTextBoxColumn Dataset_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReconSucceeded;
        private System.Windows.Forms.Button bLoadData;
        private System.Windows.Forms.Button bCopy;
        private System.Windows.Forms.Button bExcel;
        private System.Windows.Forms.TextBox tExcelFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bAll;
        private System.Windows.Forms.Label lExcelDone;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

