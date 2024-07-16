using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Tomographic_Imaging_2;
using System.Threading;

namespace ProcessRecons
{
    public partial class MultiView : Form
    {
        public MultiView()
        {
            InitializeComponent();
            textBox1.Text = @"C:\processed";
            textBox1_TextChanged(this, EventArgs.Empty);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox1.Text) == true)
            {
                try
                {
                    string[] Directories = Directory.GetDirectories(textBox1.Text, "cct*",SearchOption.AllDirectories);
                   
                    lDataDirectories.Items.Clear();
                   
                    for (int i = 0; i < Directories.Length; i++)
                    {
                        if (Directories[i].Length > 6)
                        {
                            //string dirName = Path.GetFileNameWithoutExtension(Directories[i]);
                            string dirName = Directories[i];
                            lDataDirectories.Items.Add(dirName);
                        }
                    }
                }
                catch { }
            }
        }

        private void bBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private string GetDataInFolder()
        {
            //build the file structure
            string pPath;

            pPath = textBox1.Text ;
            return pPath;
        }
        private string GetExperimentFolder()
        {
            string pPath = GetDataInFolder() + lDataDirectories.SelectedItem.ToString() + "\\";
            string DataPath = Path.GetDirectoryName(pPath) + "\\";
            return DataPath;
        }
        private string GetDataFolder()
        {
            //build the file structure

            return GetExperimentFolder() + "Data\\";
        }
        private PictureBox CreateWindow()
        {
            PictureBox  pictureBox1 = new System.Windows.Forms.PictureBox();
            pictureBox1.Location = new System.Drawing.Point(21, 46);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(236, 247);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.panel2.Controls.Add(pictureBox1);

            return pictureBox1;
        }

        private List<PictureBox> Windows = new List<PictureBox>();
        private void bLoad_Click(object sender, EventArgs e)
        {
            List<string> Selected = new List<string>();
            for (int i = 0; i < lDataDirectories.Items.Count; i++)
            {
                if (lDataDirectories.GetSelected(i) == true)
                {
                    string pPath =  (string)lDataDirectories.Items[i] + "\\";
                    Selected.Add(pPath);
                }
            }

            foreach (PictureBox p in Windows)
                panel2.Controls.Remove(p);

            Windows.Clear();

            foreach (string experiment in Selected)
            {
                try
                {

                    float [,,] data= MathHelpLib.MathHelpsFileLoader.OpenDensityDataFloat(experiment + @"data\ProjectionObject.cct");

                    viewerControl3D1.SetImage(data);
                 
                    Bitmap[] Views = viewerControl3D1.GetVisibleImages();

                    Bitmap wholeImage = new Bitmap(Views[0].Width, Views[0].Height);
                    Graphics g = Graphics.FromImage(wholeImage);
                    g.DrawImage(Views[1], new System.Drawing.Point(0, 0));

                    String drawString = Path.GetDirectoryName(experiment);
                    // Create font and brush.
                    Font drawFont = new Font("Arial", 8);
                    SolidBrush drawBrush = new SolidBrush(Color.Red);
                    // Create point for upper-left corner of drawing.
                    float x = 0F;
                    float y = 15F;
                    // Set format of string.
                    StringFormat drawFormat = new StringFormat();
                    drawFormat.FormatFlags = StringFormatFlags.NoClip;
                    g.DrawString(drawString, drawFont, drawBrush, new Point(0, 12));
                    g = null;

                    PictureBox pictureBox1 = CreateWindow();
                    pictureBox1.Image =wholeImage;
                    pictureBox1.Invalidate();
                    System.Windows.Forms.Application.DoEvents();
                    Windows.Add(pictureBox1);
                    Application.DoEvents();
                    ArrangeWindows();
                }
                catch { }
            }

            ArrangeWindows();
        }

        private void ArrangeWindows()
        {
            int Width =(int)( panel2.Width / numNumCols.Value);
            int nCols = (int)numNumCols.Value;
            int Row=0;
            for (int i = 0; i < Windows.Count; i++)
            {
                Row =(int)Math.Floor( (double)i / nCols);
                int col = i % nCols;

                Windows[i].Left = col * Width;
                Windows[i].Top = Row * Width;
                Windows[i].Width = Width;
                Windows[i].Height = Width;
            }
            panel2.Height = (Row+1) * Width;
        }

        private void numNumCols_ValueChanged(object sender, EventArgs e)
        {
            ArrangeWindows();
            Application.DoEvents();
        }

        private void MultiView_Load(object sender, EventArgs e)
        {

        }

        private void MultiView_ResizeEnd(object sender, EventArgs e)
        {
           // ArrangeWindows();
           
        }

        private void MultiView_Resize(object sender, EventArgs e)
        {
            ArrangeWindows();
        }
    }
}
