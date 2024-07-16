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
    public partial class formDragGraph : Form
    {
        public formDragGraph()
        {
            InitializeComponent();
        }

        private void formDragGraph_Load(object sender, EventArgs e)
        {
            dragGraph1.InsertControlPoint(new PointF(0, 0));
            dragGraph1.InsertControlPoint(new PointF(1, 1));
        }
    }
}
