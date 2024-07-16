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
    public partial class FakeMoviePlayer : Form
    {
        public FakeMoviePlayer()
        {
            InitializeComponent();
        }
        private string[] Filenames;
        private int FrameIndex = 0;
        public void SetFilenames(string[] FrameFilenames, bool StartPlay)
        {
            tFrame.Maximum = FrameFilenames.Length;
            Filenames = FrameFilenames;
            FrameIndex = 0;
            pictureBox1.Image = new Bitmap(FrameFilenames[0]);
            pictureBox1.Invalidate();
            if (StartPlay) timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            FrameIndex = (FrameIndex + 1) % Filenames.Length;
            Bitmap t = new Bitmap(Filenames[FrameIndex]);
            if (t.Width == pictureBox1.Image.Width && t.Height == pictureBox1.Image.Height)
            {
                pictureBox1.Image = t;
                pictureBox1.Invalidate();
            }
            Application.DoEvents();
        }

        private void bPlay_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }

        private void FakeMoviePlayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Enabled = false;
        }

        private void tFrame_ValueChanged(object sender, EventArgs e)
        {
           
            Bitmap t = new Bitmap(Filenames[tFrame.Value ]);
            if (t.Width == pictureBox1.Image.Width && t.Height == pictureBox1.Image.Height)
            {
                pictureBox1.Image = t;
                pictureBox1.Invalidate();
            }
            Application.DoEvents();
        }

      
    }
}
