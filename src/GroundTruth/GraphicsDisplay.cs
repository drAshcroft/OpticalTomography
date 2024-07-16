using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageViewer3D.Meshes;
using MogreTest;
using MathHelpLib;

namespace GroundTruth
{
    public partial class GraphicsDisplay : Form
    {
        public GraphicsDisplay()
        {
            InitializeComponent();
        }
        protected OgreWindow mogreWin;
        public void ShowGraphics( MarchingCubes marchingCubes )
        {
            mogreWin = new OgreWindow(new Point(100, 30), pictureBox1.Handle);
            mogreWin.InitMogre();

            mogreWin.CreateMesh("Sphere1", marchingCubes.VertexList, marchingCubes.TriangleIndexs);

        }
    }
}
