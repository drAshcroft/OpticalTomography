using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CopyToSVN
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string OrigPath = @"C:\Development\CellCT\Tomographic_Imaging_MDL\";
            string[] Files = Directory.GetFiles(@"C:\Development\CellCT\Tomographic_Imaging_MDL\", "*.*", SearchOption.AllDirectories);
             List<string> GoodFiles = new List<string>();
             foreach (string f in Files)
             {
                 if (f.Contains(".svn") == false && f.Contains(@"\obj\") == false)
                 {
                     GoodFiles.Add(f);
                 }
                 else
                 {
                     //System.Diagnostics.Debug.Print("");
                 }
             }
             
             int PathLength = OrigPath.Length;
             foreach (string f in GoodFiles)
             {
                 string f2 = @"C:\Development\CellCT\Tomographic_Imaging\" + f.Remove(0, PathLength);
                 try
                 {
                     File.Copy(f, f2, true);
                 }
                 catch (DirectoryNotFoundException ex)
                 {
                     Directory.CreateDirectory(Path.GetDirectoryName(f2));
                     File.Copy(f, f2, true);
                 }
                 catch 
                 { }
             }
             this.Close();
        }
    }
}
