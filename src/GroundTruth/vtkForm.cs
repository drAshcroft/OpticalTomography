using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GroundTruth
{
    public partial class vtkForm : Form
    {
        public vtkForm()
        {
            InitializeComponent();
        }

        public GraphingLibrary.DrawingAndGraphing2._3D_View GetViewer()
        {
            return _3D_View1;
        }
    }

 
}
