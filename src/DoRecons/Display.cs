using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoRecons
{
    public partial class Display : Form
    {
        public Display()
        {
            InitializeComponent();
        }
        public PictureBox picture
        {
            get { return pictureBox1; }
        }
        public string Caption
        {
            set { this.Text = value; }
        }
    }
}
