using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MathHelpLib
{
    public partial class ImageViewer : UserControl,IGraphControl 
    {
        public ImageViewer()
        {
            InitializeComponent();
        }
        public void SetData(PhysicalArray PhysArray)
        {
            pictureBox1.Image = PhysArray.MakeBitmap();
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
            this.Invalidate();
            this.Refresh();
            Application.DoEvents();
        }
    }
}
