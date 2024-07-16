using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MathHelpLib;
using MathHelpLib.ImageProcessing;
using MathHelpLib.PhysicalArrays;

namespace DoRecons
{
    public partial class TwoVolume : Form
    {
        public TwoVolume()
        {
            InitializeComponent();
        }

        float[, ,] Vol1;
        float[, ,] Vol2;


         float[,] X1;
         float[,] X2;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();

            bColor1.BackColor = Color.FromArgb(0, 0, 255);
            bColor2.BackColor = Color.FromArgb(255, 0, 0);
           /* of.ShowDialog();
            string Path1 = of.FileName;
           // of.ShowDialog();
            string Path2 = of.FileName;*/


            string Path1 = @"Y:\Fluor\cct001\201203\15\cct001_20120315_084134\Data\ProjectionObject.cct";
            string Path2 = @"Y:\Fluor\cct001\201203\15\cct001_20120315_084709\Data\ProjectionObject.cct";


            Vol1=  MathHelpLib.MathHelpsFileLoader.OpenDensityDataFloat(Path1);
            ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder.DoRemoveFBPCylinder(ref Vol1);
            Vol2=  MathHelpLib.MathHelpsFileLoader.OpenDensityDataFloat(Path2);
             ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder.DoRemoveFBPCylinder(ref Vol2);

             X1= CleanImage( Vol1.SliceXAxis(Vol1.GetLength(0) / 2));
             X2 =CleanImage( Vol2.SliceXAxis(Vol1.GetLength(0) / 2));

             float MinPercent = trackBar1.Value / (float)trackBar1.Maximum;
             Bitmap b = MathImageHelps.MergeBitmaps(X1, X2, bColor1.BackColor, bColor2.BackColor, tVol1.Value / 10f, tVol2.Value / 10f, MinPercent);
             pictureBox1.Image = b;

            pictureBox1.Invalidate();
            pictureBox1.Refresh();
        }

        private float[,] CleanImage(float[,] fin)
        {
            for (int i = 0; i < fin.GetLength(0); i++)
                for (int j = 0; j < fin.GetLength(1); j++)
                    if (fin[i, j] == 43.6717262)
                        fin[i, j] = 0;

            return fin;
        }

        private void bColor1_Click(object sender, EventArgs e)
        {
            float MinPercent = trackBar1.Value / (float)trackBar1.Maximum;
            colorDialog1.ShowDialog();
            bColor1.BackColor = colorDialog1.Color;
            pictureBox1.Image = MathImageHelps.MergeBitmaps(X1, X2, bColor1.BackColor, bColor2.BackColor, tVol1.Value / 10f, tVol2.Value / 10f, MinPercent);

            pictureBox1.Invalidate();
            pictureBox1.Refresh();
        }

        private void bColor2_Click(object sender, EventArgs e)
        {
            float MinPercent = trackBar1.Value / (float)trackBar1.Maximum;
            colorDialog1.ShowDialog();
            bColor2.BackColor = colorDialog1.Color;
            pictureBox1.Image = MathImageHelps.MergeBitmaps(X1, X2, bColor1.BackColor, bColor2.BackColor, tVol1.Value / 10f, tVol2.Value / 10f, MinPercent);

            pictureBox1.Invalidate();
            pictureBox1.Refresh();
        }

        private void tVol1_ValueChanged(object sender, EventArgs e)
        {
            float MinPercent = trackBar1.Value / (float)trackBar1.Maximum;
            pictureBox1.Image = MathImageHelps.MergeBitmaps(X1, X2, bColor1.BackColor, bColor2.BackColor, tVol1.Value / 10f, tVol2.Value / 10f, MinPercent);

            pictureBox1.Invalidate();
            pictureBox1.Refresh();
        }

        private void tVol2_ValueChanged(object sender, EventArgs e)
        {
            float MinPercent = trackBar1.Value / (float)trackBar1.Maximum;
            pictureBox1.Image = MathImageHelps.MergeBitmaps(X1, X2, bColor1.BackColor, bColor2.BackColor, tVol1.Value / 10f, tVol2.Value / 10f, MinPercent);

            pictureBox1.Invalidate();
            pictureBox1.Refresh();
        }

        private void bProduce_Click(object sender, EventArgs e)
        {
            float MinPercent = trackBar1.Value / (float)trackBar1.Maximum;
            X1 = CleanImage(Vol1.SliceXAxis(Vol1.GetLength(0) / 2));
            X2 = CleanImage(Vol2.SliceXAxis(Vol1.GetLength(0) / 2));

            Bitmap b = MathImageHelps.MergeBitmaps(X1, X2, bColor1.BackColor, bColor2.BackColor, tVol1.Value / 10f, tVol2.Value / 10f, MinPercent);
            pictureBox3.Image = b;
            pictureBox3.Invalidate();
            pictureBox3.Refresh();



            X1 = CleanImage(Vol1.SliceYAxis(Vol1.GetLength(0) / 2));
            X2 = CleanImage(Vol2.SliceYAxis(Vol1.GetLength(0) / 2));

            b = MathImageHelps.MergeBitmaps(X1, X2, bColor1.BackColor, bColor2.BackColor, tVol1.Value / 10f, tVol2.Value / 10f, MinPercent);
            pictureBox2.Image = b;
            pictureBox2.Invalidate();
            pictureBox2.Refresh();


            X1 = CleanImage(Vol1.SliceZAxis(Vol1.GetLength(0) / 2));
            X2 = CleanImage(Vol2.SliceZAxis(Vol1.GetLength(0) / 2));

            b = MathImageHelps.MergeBitmaps(X1, X2, bColor1.BackColor, bColor2.BackColor, tVol1.Value / 10f, tVol2.Value / 10f, MinPercent);
            pictureBox1.Image = b;
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float MinPercent = trackBar1.Value / (float)trackBar1.Maximum;

        }

         
    }
}
