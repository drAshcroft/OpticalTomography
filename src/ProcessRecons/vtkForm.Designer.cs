namespace ProcessRecons
{
    partial class vtkForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dragGraph1 = new GraphingLibrary.DragGraph();
            this.bRotateAndSave = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.dragGraph1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.bRotateAndSave, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 76.13333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.86667F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(975, 962);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // dragGraph1
            // 
            this.dragGraph1.BackColor = System.Drawing.Color.White;
            this.dragGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dragGraph1.Location = new System.Drawing.Point(3, 735);
            this.dragGraph1.Name = "dragGraph1";
            this.dragGraph1.Size = new System.Drawing.Size(969, 224);
            this.dragGraph1.TabIndex = 0;
            this.dragGraph1.DrawPointAdded += new GraphingLibrary.DragGraph.DragPointAddedEvent(this.dragGraph1_DrawPointAdded);
            this.dragGraph1.DrawPointMoved += new GraphingLibrary.DragGraph.DrawPointMovedEvent(this.dragGraph1_DrawPointMoved);
            // 
            // bRotateAndSave
            // 
            this.bRotateAndSave.Location = new System.Drawing.Point(3, 3);
            this.bRotateAndSave.Name = "bRotateAndSave";
            this.bRotateAndSave.Size = new System.Drawing.Size(75, 35);
            this.bRotateAndSave.TabIndex = 1;
            this.bRotateAndSave.Text = "Rotate and Save";
            this.bRotateAndSave.UseVisualStyleBackColor = true;
            this.bRotateAndSave.Click += new System.EventHandler(this.bRotateAndSave_Click);
            // 
            // vtkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 962);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "vtkForm";
            this.Text = "vtkForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private GraphingLibrary.DragGraph dragGraph1;
        private System.Windows.Forms.Button bRotateAndSave;

    }
}