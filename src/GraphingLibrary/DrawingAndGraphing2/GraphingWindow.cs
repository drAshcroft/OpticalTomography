using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathHelpLib;

namespace GraphingLib.DrawingAndGraphing
{
    public partial class GraphingWindow : Form
    {
        public GraphingWindow()
        {
            InitializeComponent();
        }

        public void SetData(PhysicalArray PhysArray)
        {
            mathGraph1.GraphData(PhysArray);
        }
    }
}
