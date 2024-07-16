using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MathHelpLib;

namespace IVGViewer
{
    public partial class IVG_Viewr : Form
    {
        public IVG_Viewr()
        {
            InitializeComponent();
        }

        public IVG_Viewr(string[]args)
        {
            InitializeComponent();

            if (args != null && args.Length > 0)
            {
                string exten = Path.GetExtension(args[0]).ToLower();
                if (exten == ".cct" || exten == ".raw" || exten == ".dat")
                {
                    PhysicalArray pa= PhysicalArray.OpenDensityData(args[0]);
                    if (pa.ArrayRank == PhysicalArrayRank.Array2D || pa.GetLength(Axis.ZAxis )==1)
                    {
                        viewerControl1.Visible = true;
                        viewerControl3D1.Visible = false;

                        viewerControl1.SetImage(  new ImageHolder((double[,])pa.ActualData2D)  );
                    }
                    else
                    {




                        viewerControl1.Visible = false;
                        viewerControl3D1.Visible = true;

                        pa = null;
                        GC.Collect();
                        if (args[0].Contains("ProjectionObject0.dat") == false)
                            viewerControl3D1.SetImage(args[0]);
                        else
                        {
                            string path = Path.GetDirectoryName(args[0]);
                            float[, ,] data = MathHelpLib.ProjectionFilters.ProjectionArrayObject.LoadMultipleFiles(args[0] , path + "\\ProjectionObject1.dat");
                            viewerControl3D1.SetImage(data);
                        }
                    }
                }
                else
                {
                    viewerControl1.Visible = true;
                    viewerControl3D1.Visible =false;
                 
                    viewerControl1.SetImage(MathHelpLib.MathHelpsFileLoader.Load_Bitmap(args[0]).ToBitmap());
                }
            }
        }

        private void IVG_Viewr_Load(object sender, EventArgs e)
        {
            this.Show();
            Application.DoEvents();
          
            if (viewerControl1.MyMenu != null)
            {
                this.MainMenuStrip = viewerControl1.MyMenu;
                this.Controls.Add(viewerControl1.MyMenu);
                viewerControl1.MyMenu.BringToFront();
                viewerControl1.MyMenu.Visible = true;
                viewerControl1.Top = this.MainMenuStrip.Height;
            }
        }


        string[] DirectoryFiles = null;
        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
            {
                tDirectorySearch.Visible = true;
                DirectoryFiles = Directory.GetFiles(folderBrowserDialog1.SelectedPath);
                DirectoryFiles = MathHelpLib.MathStringHelps.SortNumberedFiles(DirectoryFiles);
                tDirectorySearch.Maximum = DirectoryFiles.Length;
                tDirectorySearch_ValueChanged(this, EventArgs.Empty);
            }
        }

        private void tDirectorySearch_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                viewerControl1.SetImage(MathHelpsFileLoader.Load_Bitmap(DirectoryFiles[tDirectorySearch.Value]));
            }
            catch { }
        }
    }
}
