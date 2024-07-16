namespace ImageTestor
{
    partial class formDragGraph
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
            this.dragGraph1 = new GraphingLibrary.DragGraph();
            this.SuspendLayout();
            // 
            // dragGraph1
            // 
            this.dragGraph1.BackColor = System.Drawing.Color.White;
            this.dragGraph1.Location = new System.Drawing.Point(32, 45);
            this.dragGraph1.Name = "dragGraph1";
            this.dragGraph1.Size = new System.Drawing.Size(981, 378);
            this.dragGraph1.TabIndex = 0;
            // 
            // formDragGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1071, 480);
            this.Controls.Add(this.dragGraph1);
            this.Name = "formDragGraph";
            this.Text = "formDragGraph";
            this.Load += new System.EventHandler(this.formDragGraph_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private GraphingLibrary.DragGraph dragGraph1;
    }
}