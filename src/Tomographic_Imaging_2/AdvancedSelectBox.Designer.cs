namespace Tomographic_Imaging_2
{
    partial class AdvancedSelectBox
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
            this.bAccept = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bMoveUp = new System.Windows.Forms.Button();
            this.bMoveDown = new System.Windows.Forms.Button();
            this.bReject = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bAccept
            // 
            this.bAccept.Enabled = false;
            this.bAccept.Location = new System.Drawing.Point(167, 79);
            this.bAccept.Name = "bAccept";
            this.bAccept.Size = new System.Drawing.Size(33, 24);
            this.bAccept.TabIndex = 16;
            this.bAccept.Text = ">>";
            this.bAccept.UseVisualStyleBackColor = true;
            this.bAccept.Click += new System.EventHandler(this.bAccept_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "1";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(3, 16);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(158, 225);
            this.listBox1.TabIndex = 14;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(206, 16);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(158, 225);
            this.listBox2.TabIndex = 17;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(203, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "2";
            // 
            // bMoveUp
            // 
            this.bMoveUp.Enabled = false;
            this.bMoveUp.Location = new System.Drawing.Point(370, 79);
            this.bMoveUp.Name = "bMoveUp";
            this.bMoveUp.Size = new System.Drawing.Size(73, 24);
            this.bMoveUp.TabIndex = 19;
            this.bMoveUp.Text = "Move Up";
            this.bMoveUp.UseVisualStyleBackColor = true;
            this.bMoveUp.Click += new System.EventHandler(this.bMoveUp_Click);
            // 
            // bMoveDown
            // 
            this.bMoveDown.Enabled = false;
            this.bMoveDown.Location = new System.Drawing.Point(370, 109);
            this.bMoveDown.Name = "bMoveDown";
            this.bMoveDown.Size = new System.Drawing.Size(73, 24);
            this.bMoveDown.TabIndex = 20;
            this.bMoveDown.Text = "Move Down";
            this.bMoveDown.UseVisualStyleBackColor = true;
            this.bMoveDown.Click += new System.EventHandler(this.bMoveDown_Click);
            // 
            // bReject
            // 
            this.bReject.Enabled = false;
            this.bReject.Location = new System.Drawing.Point(167, 109);
            this.bReject.Name = "bReject";
            this.bReject.Size = new System.Drawing.Size(33, 24);
            this.bReject.TabIndex = 21;
            this.bReject.Text = "<<";
            this.bReject.UseVisualStyleBackColor = true;
            this.bReject.Click += new System.EventHandler(this.bReject_Click);
            // 
            // AdvancedSelectBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bReject);
            this.Controls.Add(this.bMoveDown);
            this.Controls.Add(this.bMoveUp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.bAccept);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.listBox1);
            this.Name = "AdvancedSelectBox";
            this.Size = new System.Drawing.Size(448, 245);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bAccept;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bMoveUp;
        private System.Windows.Forms.Button bMoveDown;
        private System.Windows.Forms.Button bReject;
    }
}
