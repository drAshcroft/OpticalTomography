using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GraphingLib
{
    public partial class Light3DView : Form
    {
        public Light3DView()
        {
            InitializeComponent();
        }

        public void SetData(string DataFilename)
        {
            viewerControl3D1.SetImage(DataFilename);
        }
    }
}
