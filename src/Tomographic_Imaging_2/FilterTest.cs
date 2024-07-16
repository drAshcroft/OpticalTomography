using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathHelpLib;

namespace Tomographic_Imaging_2
{
    public partial class FilterTest : Form
    {
        public FilterTest()
        {
            InitializeComponent();
        }
        PhysicalArray Image;
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
            double[] Impulse = Filtering.Sinc_RS_RadonFilter  (512,512,trackBar1.Value, 1);
            PhysicalArray Convoluted= Image.ConvoluteChop1D(Axis.XAxis, Impulse);

            Convoluted.TruncateDataInPlace(Axis.XAxis, -1, 1);
            Convoluted.TruncateDataInPlace(Axis.YAxis, -1, 1);

            pictureBox1.Image =  Convoluted.MakeBitmap();
            pictureBox1.Invalidate();
            Application.DoEvents();
        }

        private void FilterTest_Load(object sender, EventArgs e)
        {
            double[,] RawData = MathHelpLib.ImageProcessing.MathImageHelps.LoadStandardImage_Intensity(
                @"C:\Development\CellCT\DataIn\Flo1RawPP_cct001_20100511_150225\data\CorrectedPPs000.bmp",false );
            Image = new PhysicalArray(RawData, new double[] { -1, -1, -1 }, new double[] { 1, 1, 1 });
            Image = Image.ZeroPad_DataCentered(Axis.XAxis, 512);
            Image = Image.ZeroPad_DataCentered(Axis.YAxis, 512);
        }
    }
}
