using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathHelpLib._3DStuff;

namespace MathHelpLib.DrawingAndGraphing
{
    public partial class Graph3DThreshold : UserControl
    {
        Thresholds mThreshold;

        public Graph3DThreshold()
        {
            InitializeComponent();
        }
        public Graph3DThreshold(Thresholds threshold  )
        {
            InitializeComponent();
            SetThresholder(threshold);
        }

        public void SetThresholder(Thresholds threshold)
        {
            mThreshold = threshold;
            lMiddle.Text = threshold.Middle.ToString();
            lWidth.Text = threshold.HalfWidth.ToString();
            bColor.BackColor = threshold.PixelColor;
            timer1.Enabled = true;
        }

        public event EventHandler ColorClicked;
        private void bColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            mThreshold.PixelColor = colorDialog1.Color;
            bColor.BackColor = mThreshold.PixelColor;
            if (ColorClicked != null)
                ColorClicked(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lMiddle.Text = mThreshold.Middle.ToString();
            lWidth.Text = mThreshold.HalfWidth.ToString();
        }
    }
}
