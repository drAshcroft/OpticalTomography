using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageTestor
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            double[, ,] temp = new double[100, 100, 100];
            double R = 50 * 50;
            for (int x = 0; x < 256; x++)
                for (int y = 0; y < 256; y++)
                    for (int z = 0; z < 256; z++)
                    {
                        double dx = x - 50;
                        double dy = y - 50;
                        double dz = z - 50;
                        if (dx * dx + dy * dy + dz * dz < R)
                            temp[x, y, z] = x;
                    }
            viewerControl3D1.SetImage(temp);
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }
    }
}
