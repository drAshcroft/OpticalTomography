using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace LocateCell
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] Files = Directory.GetFiles(@"C:\AndrewMovie","*.jpg");

            pictureBox1.Image = new Bitmap(Files[0]);
            pictureBox1.Invalidate();
            
           
        }
    }
}
