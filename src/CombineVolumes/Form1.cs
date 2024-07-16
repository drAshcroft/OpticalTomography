using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathHelpLib;
using System.Drawing.Imaging;

namespace CombineVolumes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        PhysicalArray pa1;
        PhysicalArray pa2;

        Bitmap[] Slices1 = null;
        Bitmap[] Slices2 = null;

        private void button1_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                pa1 = PhysicalArray.OpenDensityData(openFileDialog1.FileName);
                Slices1 = pa1.ShowCross();
                ShowData();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                pa2 = PhysicalArray.OpenDensityData(openFileDialog1.FileName);
                Slices2 = pa2.ShowCross();
                ShowData();
            }
        }

        private void ShowData()
        {

            Bitmap b1 = new Bitmap(Slices1[0]);
            Bitmap b2 = new Bitmap(Slices1[1]);
            Bitmap b3 = new Bitmap(Slices1[2]);

            if (Slices2 != null)
            {
                b1 = MergeBitmaps(b1, Slices2[0], hScrollBar1.Value * -1, hScrollBar2.Value * -1);
                b2 = MergeBitmaps(b2, Slices2[1], hScrollBar2.Value * -1, vScrollBar1.Value * -1);
                b3 = MergeBitmaps(b3, Slices2[2], hScrollBar1.Value * -1, vScrollBar1.Value * -1);
            }

            pictureBox1.Image = b1;
            pictureBox2.Image = b2;
            pictureBox3.Image = b3;

            pictureBox1.Invalidate();
            pictureBox2.Invalidate();
            pictureBox3.Invalidate();

        }

        private Bitmap MergeBitmaps(Bitmap b1, Bitmap b2, int dx, int dy)
        {
            int iWidth = b1.Width;
            int iHeight = b1.Height;

            BitmapData bmd = b1.LockBits(new Rectangle(0, 0, b1.Width, b1.Height), ImageLockMode.WriteOnly, b1.PixelFormat);
            BitmapData bmd2 = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height), ImageLockMode.WriteOnly, b2.PixelFormat);

            unsafe
            {
                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    int cy = dy;
                    for (int y = 0; y < iHeight; y++)
                    {
                        Int32* scanline1 = (Int32*)((byte*)bmd.Scan0 + y * bmd.Stride);
                        Int32* scanline2 = (Int32*)((byte*)bmd2.Scan0 + (cy) * bmd2.Stride) + dx;

                        int cx = dx;
                        for (int x = 0; x < iWidth; x++)
                        {
                            if (cy > 0 && cy < b2.Height)
                            {
                                if (cx > 0 && cx < b2.Width)
                                {
                                    byte* bits1 = (byte*)scanline1;
                                    byte* bits2 = (byte*)scanline2;
                                    bits1[0] = (byte)((bits1[0]) / 2);
                                    bits1[1] = (byte)((bits1[1] + bits2[1]) / 2);
                                    bits1[2] = (byte)((bits2[2]) / 2);
                                    scanline1++;
                                    scanline2++;
                                }
                            }
                            cx++;
                        }

                        cy++;
                    }
                }
            }

            b1.UnlockBits(bmd);
            b2.UnlockBits(bmd2);
            return b1;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ShowData();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ShowData();
        }

        private void vScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            ShowData();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            ShowData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PhysicalArray ca = new PhysicalArray(pa1);
            ca.CombineWithPhysicalArray(pa2, hScrollBar2.Value * -1, hScrollBar1.Value * -1, vScrollBar1.Value * -1);

            Bitmap[] Slices3 = ca.ShowCross();

            pictureBox1.Image = Slices3[0];
            pictureBox2.Image = Slices3[1];
            pictureBox3.Image = Slices3[2];

            pictureBox1.Invalidate();
            pictureBox2.Invalidate();
            pictureBox3.Invalidate();
        }

        private void vScrollBar1_Scroll_1(object sender, ScrollEventArgs e)
        {
            ShowData();
        }

        private void hScrollBar1_Scroll_1(object sender, ScrollEventArgs e)
        {
            ShowData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PhysicalArray ca = new PhysicalArray(pa2);
            ca.ShiftPhysicalArray ( hScrollBar2.Value * -1, hScrollBar1.Value * -1, vScrollBar1.Value * -1);

            ImageViewer.PythonScripting.Projection.MakeMIPMovieEffect.DoMIPProjection("C:\\Development\\CombinedMovieNew.avi", "C:\\Development", (double[, ,])pa1.ActualData3D, (double[, ,])ca.ActualData3D);

        }
    }
}
