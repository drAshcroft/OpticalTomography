﻿namespace IronPythonEditor
{
    partial class MatlabInterface
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textEditorForm1 = new IronPythonEditor.TextEditor.TextEditorForm();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ironPythonConsole1 = new IronPythonEditor.Console.IronPythonConsole();
            this.variableWindow1 = new IronPythonEditor.VariableWindow.VariableWindow();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.variableWindow1);
            this.splitContainer1.Size = new System.Drawing.Size(1098, 810);
            this.splitContainer1.SplitterDistance = 805;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(805, 810);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textEditorForm1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(797, 784);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Code Editor";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textEditorForm1
            // 
            this.textEditorForm1.AllowDrop = true;
            this.textEditorForm1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditorForm1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textEditorForm1.Location = new System.Drawing.Point(3, 3);
            this.textEditorForm1.Name = "textEditorForm1";
            this.textEditorForm1.Size = new System.Drawing.Size(791, 778);
            this.textEditorForm1.TabIndex = 0;
            this.textEditorForm1.Load += new System.EventHandler(this.textEditorForm1_Load);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ironPythonConsole1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(797, 784);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Interactive Console";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ironPythonConsole1
            // 
            this.ironPythonConsole1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ironPythonConsole1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ironPythonConsole1.Location = new System.Drawing.Point(3, 3);
            this.ironPythonConsole1.Name = "ironPythonConsole1";
            this.ironPythonConsole1.Size = new System.Drawing.Size(791, 778);
            this.ironPythonConsole1.TabIndex = 0;
            this.ironPythonConsole1.PythonVarAdded += new IronPythonEditor.Console.IronPythonConsole.PythonVarAddedEvent(this.ironPythonConsole1_PythonVarAdded);
            this.ironPythonConsole1.PythonVarRemoved += new IronPythonEditor.Console.IronPythonConsole.PythonVarRemovedEvent(this.ironPythonConsole1_PythonVarRemoved);
            // 
            // variableWindow1
            // 
            this.variableWindow1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.variableWindow1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.variableWindow1.Location = new System.Drawing.Point(0, 0);
            this.variableWindow1.Name = "variableWindow1";
            this.variableWindow1.Size = new System.Drawing.Size(289, 810);
            this.variableWindow1.TabIndex = 0;
            // 
            // MatlabInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "MatlabInterface";
            this.Size = new System.Drawing.Size(1098, 810);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private TextEditor.TextEditorForm textEditorForm1;
        private System.Windows.Forms.TabPage tabPage2;
        private Console.IronPythonConsole ironPythonConsole1;
        private VariableWindow.VariableWindow variableWindow1;
    }
}
