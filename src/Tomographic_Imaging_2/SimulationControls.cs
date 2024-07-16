using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathHelpLib;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using ImageViewer;
using MathHelpLib.ImageProcessing;

namespace Tomographic_Imaging_2
{
    public partial class SimulationControls : DockContent
    {
        private ProjectionObject Phantom;

        public SimulationControls()
        {
            InitializeComponent();
        }

        private void bCreatePhantom_Click(object sender, EventArgs e)
        {

            int Size = (int)nGridSize.Value;
            Phantom = new ProjectionObject();
            Phantom.ClearGrid(true, 2, 2, Size, Size);
            Phantom.CreateShepAndLogan();
            ScriptingInterface.scriptingInterface.MakeVariableVisible("Phantom", Phantom);
            ScriptingInterface.scriptingInterface.CreateGraph("SimulationGraph", Phantom.ProjectionData);
        }

        private void b3DPhantom_Click(object sender, EventArgs e)
        {
            int GridSize = (int)nGridSize.Value;
            Phantom = new ProjectionObject();
            Phantom.ClearGrid(true, 2, 2, 2, GridSize, GridSize, GridSize);
            Phantom.CreateShepAndLogan();
            ScriptingInterface.scriptingInterface.MakeVariableVisible("Phantom3D", Phantom);
            ScriptingInterface.scriptingInterface.CreateGraph("SimulationGraph", Phantom.ProjectionData);
        }

        private void bDoSaveProjections_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            DialogResult ret = sfd.ShowDialog();
            if (ret == DialogResult.OK)
            {
                string FileTemp = Path.GetDirectoryName(sfd.FileName) + "\\" + Path.GetFileNameWithoutExtension(sfd.FileName);
                string exten = Path.GetExtension(sfd.FileName);
                double NSlices = (double)nSlices.Value;
                double SliceDegree = 360 / NSlices;
                Random rnd = new Random();
                string junk = "";
                if (Phantom != null)
                {
                    for (int i = 0; i < NSlices; i++)
                    {
                        aProjectionSlice ps = Phantom.CreateSimulatedProjection(i * SliceDegree);


                        //code to provide an artifical jitter
                        double[,] data = (double[,])ps.Projection.ActualData2D;
                        Bitmap b = data.MakeBitmap();
                        /*Bitmap b2 = new Bitmap(Image);
                        int x =(int)(Math.Round  (1-rnd.NextDouble()*2*3));
                        int y=(int)(Math.Round  (1-rnd.NextDouble()*2*3));
                        System.Diagnostics.Debug.Print(x.ToString() + ", " + y.ToString());
                        junk += x.ToString() + ", " + y.ToString() + "\n";
                        Graphics.FromImage(b2).DrawImage(Image,new Point( x,y ));
                        */

                        b.Save(FileTemp + string.Format("{0:0000}", i) + exten);
                        ScriptingInterface.scriptingInterface.CreateGraph("SimulatedProjection", b);

                        progressBar1.Value = (int)((i + 1) / NSlices * 100);
                        Application.DoEvents();
                    }
                  
                }

            }
        }

        private void SimulationControls_Load(object sender, EventArgs e)
        {
            ScriptingInterface.scriptingInterface.simulationControlForm = this;
        }


        private class Circle
        {
            public double x;
            public double y;
            public double z;
            public double R;
            public double DeltaDensity;

            public Circle(double X, double Y, double Z, double r, double Density)
            {
                int size = 170;
                this.x = X * size;
                this.y = Y * size;
                this.z = Z * size;
                this.R = r * size;
                this.DeltaDensity = Density;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            DialogResult ret = sfd.ShowDialog();
            if (ret == DialogResult.OK)
            {
                string FileTemp = Path.GetDirectoryName(sfd.FileName) + "\\" + Path.GetFileNameWithoutExtension(sfd.FileName);
                string exten = Path.GetExtension(sfd.FileName);
                double NSlices = (double)nSlices.Value;
                double SliceDegree = 360d / NSlices / 180d * Math.PI;

                double[,] data = new double[1600, 900];
                double hWidth = data.GetLength(0) / 2;
                double hHeight = data.GetLength(1) / 2;

                List<Circle> Circles = new List<Circle>();
                Circles.Add(new Circle( 0,   0,  0,    .9,     -2));
                Circles.Add(new Circle( 0,  0,0,  .6624, .98));

                Circles.Add(new Circle( .22, 0,   0,   .21,   .02 ));
                Circles.Add(new Circle(-.22, 0,    0,  .21,   .02));

                Circles.Add(new Circle(0, .35, 0, .25, -.01));

                Circles.Add(new Circle( 0,  .1,   0,   .046, -.01));
                Circles.Add(new Circle( 0, -.1,   0,   .046, -.01));

                Circles.Add(new Circle(-.08, -.605, 0, .046, -.01));
                Circles.Add(new Circle(0, -.605,0, .023, -.01));
                Circles.Add(new Circle(.06, -.605, 0, .046, -.01));



                double dx, dy,d;
                for (int i = 0; i < NSlices; i++)
                {
                    for (int xi = 0; xi < data.GetLength(0); xi++)
                    {
                        for (int yi = 0; yi < data.GetLength(1); yi++)
                        {
                            data[xi, yi] = 1000;
                        }
                    }
                    
                    for (int xi = 0; xi < data.GetLength(0); xi++)
                    {
                        for (int yi = 0; yi < data.GetLength(1); yi++)
                        {
                            for (int cir = 0; cir < Circles.Count; cir++)
                            {
                                double x = ((xi - hWidth));
                                double y1 = ((yi - hHeight)- 250*Math.Sin(SliceDegree * i));
                                Circle c = Circles[cir];
                                dx = (x - (c.x - c.y * Math.Sin(SliceDegree * i)));
                                dy= (y1- (c.y ));
                                double r = Math.Sqrt( dx*dx+dy*dy);
                                if (r < c.R)
                                {
                                    d = data[xi, yi] + c.DeltaDensity / c.R * Math.Sqrt(c.R * c.R - r * r);
                                    data[xi, yi] = d;
                                }


                            }
                        }
                    }

                    Bitmap b = data.MakeBitmap();
                    b.Save(FileTemp + string.Format("{0:0000}", i) + exten);
                    //MathHelpLib.MathHelpsFileLoader.Save_Raw(FileTemp + string.Format("{0:0000}", i) + ".ivg",data );
                    ScriptingInterface.scriptingInterface.CreateGraph("SimulationGraph", data, "X", "Y", "Z");
                    progressBar1.Value = (int)((i + 1) / NSlices * 100);
                    Application.DoEvents();
                }

            }
        }

        private void bLoadPhantom_Click(object sender, EventArgs e)
        {

            openFileDialog1.Multiselect = true;
            DialogResult ret = openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                Phantom = new ProjectionObject();
                if (openFileDialog1.FileNames.Length == 1)
                {
                    Phantom.OpenDensityData(openFileDialog1.FileName);
                }
                else
                {
                    Phantom.OpenDensityData(openFileDialog1.FileNames);
                }
                ScriptingInterface.scriptingInterface.MakeVariableVisible("Phantom3D", Phantom);
                ScriptingInterface.scriptingInterface.CreateGraph("SimulationGraph", Phantom.ProjectionData);
            }
            openFileDialog1.Multiselect = false;

        }

        private void bDoProjections_Click(object sender, EventArgs e)
        {
            double NSlices = (double)nSlices.Value;
            double SliceDegree = 360 / NSlices;
            string junk = "";
            if (Phantom != null)
            {
                for (int i = 0; i < NSlices; i++)
                {
                    aProjectionSlice ps = Phantom.CreateSimulatedProjection(i * SliceDegree);
                    ScriptingInterface.scriptingInterface.CreateGraph("SimulatedProjection", ps.Projection );

                    progressBar1.Value = (int)((i + 1) / NSlices * 100);
                    Application.DoEvents();
                    Phantom.AddSlice(ps);
                }
            }

        }



    }
}
