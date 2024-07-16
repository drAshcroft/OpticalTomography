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

namespace RejectReview
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] Dirs = Directory.GetDirectories(@"X:\Rejected_Datasets", "pp", SearchOption.AllDirectories);
           for (int i = 0; i < Dirs.Length; i++)
           {
               string ppath = Path.GetDirectoryName(Dirs[i]);
               string[] Parts =Path.GetFileName(ppath).Split('_');
               long Date = long.Parse(Parts[1]);

               if (Date >= 20110418 && Date<20111100)
               {
                   string[] Files = Directory.GetFiles(Dirs[i], "000.*");
                   if (Files != null && Files.Length > 0)
                   {
                       if (Path.GetExtension(Files[0]).ToLower() == ".ivg")
                       {
                           try
                           {
                                ImageHolder ih = MathHelpLib.MathHelpsFileLoader.Load_Bitmap(Files[0]);
                                ih.Save(@"C:\temp\Summary\test_" + Parts[1] + Parts[2] + ".png");
                           }
                           catch
                           { }
                       }
                       else
                       {
                           File.Copy(Files[0], @"C:\temp\Summary\test_" + Parts[1] + Parts[2] +  ".png",true);
                       }
                   }
               }

           }
        }
    }
}
