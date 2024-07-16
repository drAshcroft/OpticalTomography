using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VisionCentering
{
    public partial class _3dView : Form
    {
        public _3dView()
        {
            InitializeComponent();
        }

        public void SetData(double[,,] DisplayImage)
        {
            viewerControl3D1.SetImage(DisplayImage);
        }
    }
}
