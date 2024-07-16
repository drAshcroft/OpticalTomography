namespace IronPythonEditor.Console
{
    partial class IronPythonConsole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IronPythonConsole));
            this.ironTextBoxControl1 = new IronPythonEditor.Console.IronTextBoxControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // ironTextBoxControl1
            // 
            this.ironTextBoxControl1.ConsoleTextBackColor = System.Drawing.Color.White;
            this.ironTextBoxControl1.ConsoleTextFont = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ironTextBoxControl1.ConsoleTextForeColor = System.Drawing.SystemColors.WindowText;
            this.ironTextBoxControl1.defBuilder = ((System.Text.StringBuilder)(resources.GetObject("ironTextBoxControl1.defBuilder")));
            this.ironTextBoxControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ironTextBoxControl1.Location = new System.Drawing.Point(0, 0);
            this.ironTextBoxControl1.Name = "ironTextBoxControl1";
            this.ironTextBoxControl1.Prompt = ">>>";
            this.ironTextBoxControl1.Size = new System.Drawing.Size(284, 262);
            this.ironTextBoxControl1.TabIndex = 5;
            this.ironTextBoxControl1.AutoCompletionListFill += new IronPythonEditor.Console.AutoCompletionListFillEvent(this.ironTextBoxControl1_AutoCompletionListFill);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // IronPythonConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.ironTextBoxControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "IronPythonConsole";
            this.Text = "IronPythonConsole";
            this.ResumeLayout(false);

        }

        #endregion

        private IronPythonEditor.Console.IronTextBoxControl ironTextBoxControl1;
        private System.Windows.Forms.Timer timer1;
    }
}