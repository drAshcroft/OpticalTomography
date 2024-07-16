using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathHelpLib;
using GraphingLib.DrawingAndGraphing;

namespace GraphingLib
{
    public partial class IntensityGraph : Form
    {
        public IntensityGraph()
        {
            InitializeComponent();
        }

        private void IntensityGraph_Load(object sender, EventArgs e)
        {
            this.Show();
            Application.DoEvents();
              openFileDialog1.Multiselect = true;
            DialogResult ret = openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                string[] FileNames = openFileDialog1.FileNames;
                FileNames = MathStringHelps.SortNumberedFiles(FileNames);
                double SliceDegree = 360d / (double)FileNames.Length ;
                double[,] Datas = new double[2, FileNames.Length];
                for (int i = 0; i < FileNames.Length; i++)
                {
                    Datas[0, i] = i * SliceDegree;
                    Datas[1, i] = MathHelpLib.ImageProcessing.MathImageHelps.SumImage(new Bitmap(FileNames[i]));
                    progressBar1.Value =(int)((double)( i+1) /(double) FileNames.Length * 100d);
                    Application.DoEvents();
                }
                Graph1D.GraphLine(zedgraphcontrol, Datas, "Projection Angle", "Intensity");
            }
        }
    }
}
