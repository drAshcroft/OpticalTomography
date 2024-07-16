namespace DoRecons
{
    partial class ReconForm
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
            this.netWorkWatcher1 = new DoRecons.PageViews.NetWorkWatcher();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.batchProcessor1 = new DoRecons.PageViews.BatchProcessor();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.testor1 = new DoRecons.PageViews.Testor();
            this.reconWorkFlow1 = new DoRecons.ReconWorkFlow();
            this.bShowCustomScript = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(736, 1011);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.netWorkWatcher1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(728, 985);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Network";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // netWorkWatcher1
            // 
            this.netWorkWatcher1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.netWorkWatcher1.Location = new System.Drawing.Point(3, 3);
            this.netWorkWatcher1.Name = "netWorkWatcher1";
            this.netWorkWatcher1.Size = new System.Drawing.Size(722, 979);
            this.netWorkWatcher1.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.batchProcessor1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(728, 985);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Batch Process";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // batchProcessor1
            // 
            this.batchProcessor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.batchProcessor1.Location = new System.Drawing.Point(0, 0);
            this.batchProcessor1.Name = "batchProcessor1";
            this.batchProcessor1.Size = new System.Drawing.Size(693, 985);
            this.batchProcessor1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.testor1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(728, 985);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Test";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // testor1
            // 
            this.testor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testor1.Location = new System.Drawing.Point(3, 3);
            this.testor1.Name = "testor1";
            this.testor1.Size = new System.Drawing.Size(722, 979);
            this.testor1.TabIndex = 0;
            this.testor1.Load += new System.EventHandler(this.testor1_Load);
            // 
            // reconWorkFlow1
            // 
            this.reconWorkFlow1.Location = new System.Drawing.Point(742, 20);
            this.reconWorkFlow1.Name = "reconWorkFlow1";
            this.reconWorkFlow1.Size = new System.Drawing.Size(637, 979);
            this.reconWorkFlow1.TabIndex = 2;
            this.reconWorkFlow1.TabStop = false;
            // 
            // bShowCustomScript
            // 
            this.bShowCustomScript.Location = new System.Drawing.Point(1204, 900);
            this.bShowCustomScript.Name = "bShowCustomScript";
            this.bShowCustomScript.Size = new System.Drawing.Size(163, 41);
            this.bShowCustomScript.TabIndex = 3;
            this.bShowCustomScript.Text = "Custom Script";
            this.bShowCustomScript.UseVisualStyleBackColor = true;
            this.bShowCustomScript.Click += new System.EventHandler(this.bShowCustomScript_Click);
            // 
            // ReconForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1404, 1011);
            this.Controls.Add(this.bShowCustomScript);
            this.Controls.Add(this.reconWorkFlow1);
            this.Controls.Add(this.tabControl1);
            this.Name = "ReconForm";
            this.Text = "ReconForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ReconForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ReconForm_FormClosed);
            this.Load += new System.EventHandler(this.ReconForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage2;
        
        private PageViews.Testor testor1;
        private PageViews.NetWorkWatcher netWorkWatcher1;
        private PageViews.BatchProcessor batchProcessor1;
        private ReconWorkFlow reconWorkFlow1;
        private System.Windows.Forms.Button bShowCustomScript;
       
    }
}