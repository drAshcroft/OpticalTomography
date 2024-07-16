using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;

namespace Tomographic_Imaging_2.ImageManipulation
{
    public partial class EasyImageViewer : DockContent 
    {
        public EasyImageViewer()
        {
            InitializeComponent();
            MenuStrip ViewerMenu = viewerControl1.MyMenu;
            if (ViewerMenu != null)
            {
                ViewerMenu.Location = new System.Drawing.Point(0, 0);
                ViewerMenu.Name = "menuStrip1";
                ViewerMenu.Size = new System.Drawing.Size(1214, 24);
                ViewerMenu.TabIndex = 3;
                ViewerMenu.Text = "";
                this.Controls.Add(ViewerMenu);
                this.MainMenuStrip = ViewerMenu;
                ViewerMenu.PerformLayout();
                ViewerMenu.BringToFront();
                ViewerMenu.Visible = true;
                viewerControl1.Top = 25;
                viewerControl1.Height = viewerControl1.Height - 25;
            }
        }
        public void ShowScript(string ScriptName)
        {
            if (ScriptName.Trim() == "")
            {
                viewerControl1.ShowTextScript ("");
            }
            else
            {
                string filename = Path.GetDirectoryName(Application.ExecutablePath) + "\\scripts\\" + ScriptName;
                viewerControl1.ShowFileScript(filename);
            }
        }
        public void SetImage(Bitmap Image)
        {
            viewerControl1.SetImage(Image);
        }
    }
}
