using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ImageTestor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Show();
            Application.DoEvents();
            // Bitmap mDataBitmap = new Bitmap(@"C:\Users\Public\Pictures\Sample Pictures\desert.jpg");
             //Bitmap b = new Bitmap(@"C:\Development\CellCT\oFlo1RawPP_cct001_20100511_150225\PP\New000.png");
            // Bitmap Image = new Bitmap(@"U:\Research\Brian\CellCT datasets\Absorption\Cell_PP_ivg_cct004_20101209_090107\PP\000.ivg");
            //  Bitmap mDataBitmap = new Bitmap(@"C:\Users\basch\Documents\Visual Studio 2008\Projects\Flo1CorrectedPP_cct001_20100511_150225\corrpp_blue000.png");
            //  viewerControl1.SetImage(b);
            if (viewerControl1.MyMenu != null)
            {
                this.MainMenuStrip = viewerControl1.MyMenu;
                this.Controls.Add(viewerControl1.MyMenu);
                viewerControl1.MyMenu.BringToFront();
                viewerControl1.MyMenu.Visible = true;
                viewerControl1.Top = this.MainMenuStrip.Height;
            }
            //string filename = @"C:\Development\CellCT\Tomographic_Imaging_MDL\Tomographic_Imaging_2\bin\x86\Debug\Scripts\CenterCells3_abs.py";
           // string filename = @"C:\Development\CellCT\Tomographic_Imaging_MDL\Tomographic_Imaging_2\bin\x86\Debug\Scripts\RemoveBackground3_abs.py";
            //string filename = @"C:\Development\CellCT\Tomographic_Imaging_MDL\Tomographic_Imaging_2\bin\x86\Debug\Scripts\ReRemoveBackground.py";

          //  viewerControl1.ShowFileScript(filename);
          
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
