using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageViewer.Filters;
using System.IO;
using MathHelpLib;
using ImageViewer;
using MathHelpLib.ImageProcessing;

namespace FixStack
{
    public partial class FormVisionGateImage : Form
    {
        public FormVisionGateImage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string STorage = textBox1.Text;

            //List<string> DirAdds = 
            List<string> BadDirs = new List<string>();
            List<string> GoodDirs = new List<string>();
            bool FirstDir = true;
            string[] AllDirs;

            BadDirs.Add(STorage);
            while (FirstDir == true && BadDirs.Count > 0)
            {
                try
                {
                    string[] Dirs = Directory.GetDirectories(BadDirs[0]);
                    if (Dirs.Length > 0)
                    {
                        if (Dirs[0].Contains("cct") == true && Path.GetFileName(Dirs[0]).Length > 6)
                            GoodDirs.AddRange(Dirs);
                        else
                            BadDirs.AddRange(Dirs);
                    }
                }
                catch { }
                BadDirs.RemoveAt(0);
            }
            AllDirs = GoodDirs.ToArray();

            Dictionary<string, ReplaceStringDictionary> AllData = new Dictionary<string, ReplaceStringDictionary>();
            for (int i = 0; i < AllDirs.Length; i++)
            {
                try
                {
                    string pPath = AllDirs[i].Replace("\"", "").Replace("'", "");

                    if (pPath.EndsWith("\\") == false)
                        pPath += "\\";

                    string dirName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(pPath));
                    string[] parts = dirName.Split('_');
                    string Prefix = parts[0];
                    string Year = parts[1].Substring(0, 4);
                    string month = parts[1].Substring(4, 2);
                    string day = parts[1].Substring(6, 2);

                    string basePath = textBox1.Text + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;

                    label1.Text = month + " / " + day + "/" + Year;
                    Application.DoEvents();
                    if (Directory.Exists(basePath) == true)
                    {
                        string[] Files = Directory.GetFiles(basePath + "\\stack\\");

                        if (Path.GetExtension(Files[0]) == ".png")
                        {

                            ImageHolder ih= MathHelpsFileLoader. Load_Bitmap(Files[0],0);

                            double [,] image= MathHelpLib.ImageProcessing.MathImageHelps. ConvertToDoubleArray(ih, false);

                            double[,] smallimage = new double[64, 64];
                            for (int x=0;x<64;x++)
                                for (int y = 0; y < 64; y++)
                                {
                                    smallimage[x, y] = image[x, y];
                                }

                            pictureBox1.Image = image.MakeBitmap();// MakeBitmap(image);
                            pictureBox1.Invalidate();

                            image = smallimage;

                            complex[,] image2 = MathHelpLib.MathFFTHelps.FFTreal2complex(image);

                            image= image2.ConvertToDoubleReal();

                            image.MultiplyInPlace(image);
                            image = MathHelpLib.MathFFTHelps.MakeFFTHumanReadable(image);

                            image.LogInPlaceErrorlessImage();

                            pictureBox2.Image = image. MakeBitmap();
                            pictureBox2.Invalidate();
                            Application.DoEvents();
                        }

                    }
                    else
                    {

                    }
                     
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            }
        }
    }
}
