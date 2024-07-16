using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tomographic_Imaging_2.ImageManipulation
{
    public partial class VideoPlayer : Form
    {
        public VideoPlayer()
        {
            InitializeComponent();
        }

        public void PlayMovie(string Filename)
        {
            axWindowsMediaPlayer1.URL = Filename ;

        }
    }
}
