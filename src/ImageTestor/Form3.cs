using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ImageTestor
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        ImageViewer.Selections.CircleAndLineSelection cls;
        private void viewerControl1_SelectionPerformed(ImageViewer.ISelection Selection)
        {
           
             cls = (ImageViewer.Selections.CircleAndLineSelection)Selection;
           
               
            
        }

        private void viewerControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'd')
            {
                FileIndex += 10;
                viewerControl1.SetImage(new Bitmap(filenames[FileIndex]));

            }
        }
        System.IO.StreamWriter file;
        string[] filenames;
        int FileIndex = 2;
        private void Form3_Load(object sender, EventArgs e)
        {
            this.Show();
            Application.DoEvents();

            filenames = MathHelpLib.MathStringHelps.SortNumberedFiles(
               Directory.GetFiles(@"C:\Users\basch\Documents\Visual Studio 2008\Projects\New folder (2)\", "*.png"));

            file = new System.IO.StreamWriter(@"C:\Users\basch\Documents\Visual Studio 2008\Projects\Tracking.txt");

            viewerControl1.SetImage(new Bitmap(filenames[FileIndex]));

        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            file.Close();
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sep = ",";
            FileIndex += 10;
            string junk = FileIndex.ToString() + sep + cls.ImageCenter.X + sep + cls.ImageCenter.Y + sep;
            junk += cls.InnerRadius + sep + cls.LineEnd.X + sep + cls.LineEnd.Y + sep;
            junk += cls.SelectionBounds.Left + sep + cls.SelectionBounds.Top + sep;
            junk += cls.SelectionBounds.Width.ToString() + sep + cls.SelectionBounds.Height.ToString();

            double dx, dy;
            dx = cls.LineEnd.X - cls.ImageCenter.X;
            dy = cls.LineEnd.Y - cls.ImageCenter.Y;
            double angle = Math.Atan(Math.Abs(dy / dx));
            if (dx < 0 && dy > 0)
                angle += Math.PI / 2d;
            if (dx < 0 && dy < 0)
                angle += Math.PI;
            if (dx > 0 && dy < 0)
                angle += Math.PI * 3d / 2d;
            angle = (angle / Math.PI * 180);

            junk += sep + angle.ToString();
            file.WriteLine(junk);

            try
            {
                viewerControl1.SetImage(new Bitmap(filenames[FileIndex]));

            }
            catch {
                file.Close();
                this.Close();
            }
          
        }

        float cubic_interpolate(float y0, float y1, float y2, float y3, float mu)
        {

            float a0, a1, a2, a3, mu2;

            mu2 = mu * mu;
            a0 = y3 - y2 - y0 + y1; //p
            a1 = y0 - y1 - a0;
            a2 = y2 - y0;
            a3 = y1;

            return (a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3);
        }

        private void correctToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string junk = Clipboard.GetText();
            string[] Lines = junk.Split(new string[] { "\n"}, StringSplitOptions.None );
            double[,] DataPoints = new double[2,Lines.Length-1];
            for (int i = 0; i < Lines.Length-1; i++)
            {
                string[] junk2 = Lines[i].Split(new string[] { ",", "\t" }, StringSplitOptions.None);
                double.TryParse(junk2[0],out DataPoints[0, i]);
                double.TryParse(junk2[1], out DataPoints[1, i]); 
            }

            double[] Angles = new double[filenames.Length];
            int MaxAngle = 0;
            string junk3 = "";
            try
            {
                for (int i = 0; i < Angles.Length - 1; i++)
                {
                    int c = (int)Math.Truncate((double)i / 5d);
                    float P0 = (float)DataPoints[1, c];
                    float P1 = (float)DataPoints[1, c + 1];
                    double u = ((double)i % 5) / 5;
                    Angles[i] = (P1 - P0) * u + P0;
                    junk3 += i.ToString() + "," + Angles[i].ToString() + "\n";
                    MaxAngle = i;
                }
            }
            catch { }
           // Clipboard.SetText(junk3);
            System.Diagnostics.Debug.Print(junk3);
            string PathOut = @"C:\Users\basch\Documents\Visual Studio 2008\Projects\CorrectedBeads\";
            for (int i = 0; i < MaxAngle ; i++)
            {
                Bitmap b = new Bitmap(filenames[i]);
                ImageViewer.Filters.Adjustments.RotateImageTool rit = new ImageViewer.Filters.Adjustments.RotateImageTool();
                ImageViewer.Filters.GeneralToken token= new ImageViewer.Filters.GeneralToken();
                token.Parameters = new object[1];
                token.Parameters[0] =-1* Angles[i];
                b=  rit.RunEffect(b, token );
                viewerControl1.SetImage(b);
                Application.DoEvents();
                b.Save(PathOut + "AngleCorrected" + string.Format("{0:000}", i) + ".bmp"); 
            }
        }

    }
}
