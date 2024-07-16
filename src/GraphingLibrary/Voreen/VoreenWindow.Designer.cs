﻿namespace GraphingLibrary
{
    partial class VoreenWindow
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
            this.bQuadView = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bQuadView
            // 
            this.bQuadView.Location = new System.Drawing.Point(20, 14);
            this.bQuadView.Name = "bQuadView";
            this.bQuadView.Size = new System.Drawing.Size(160, 42);
            this.bQuadView.TabIndex = 0;
            this.bQuadView.Text = "Quad View";
            this.bQuadView.UseVisualStyleBackColor = true;
            this.bQuadView.Click += new System.EventHandler(this.bQuadView_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(20, 62);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(160, 42);
            this.button1.TabIndex = 1;
            this.button1.Text = "Slice View";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(186, 14);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(160, 42);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cut View";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(186, 62);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(160, 42);
            this.button3.TabIndex = 3;
            this.button3.Text = "Plane Clip View";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(101, 110);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(160, 42);
            this.button4.TabIndex = 4;
            this.button4.Text = "Exploded View";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // VoreenWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 164);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.bQuadView);
            this.Name = "VoreenWindow";
            this.Text = "VoreenWindow";
            this.Load += new System.EventHandler(this.VoreenWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bQuadView;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}