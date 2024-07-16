using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace GraphingLib
{
    public partial class StackExplorer : Form
    {
        public StackExplorer()
        {
            InitializeComponent();
        }

        string[] StackNames = null;

        public void OpenVGExperimentFolder(string StackFolder)
        {
            try
            {
               
                string[] Files = Directory.GetFiles(StackFolder);

                List<string> forwards = new List<string>();
                List<string> backs = new List<string>();
                string MidPoint = "";

                for (int i = 0; i < Files.Length; i++)
                {
                    string filename = Path.GetFileNameWithoutExtension(Files[i]);
                    if (filename.EndsWith("n") == true)
                    {
                        backs.Add(Files[i]);
                    }
                    else if (filename.EndsWith("p") == true)
                        forwards.Add(Files[i]);
                    else if (filename.EndsWith("m") == true)
                        MidPoint = Files[i];
                }

                backs.Sort();
                forwards.Sort();

                string[] SortedFiles = new string[backs.Count + 1 + forwards.Count];
                int cc = 0;
                for (int i = 0; i < backs.Count; i++)
                {
                    SortedFiles[cc] = backs[backs.Count - 1 - i];
                    cc++;
                }
                SortedFiles[cc] = MidPoint;
                cc++;
                for (int i = 0; i < forwards.Count; i++)
                {
                    SortedFiles[cc] = forwards[i];
                    cc++;
                }

                StackSelector.Maximum = SortedFiles.Length;
                StackNames = SortedFiles;
                StackSelector.Value = SortedFiles.Length / 2;
            }
            catch { }
        }

        public void OpenExperimentFolder(string ExperimentFolder)
        {
            try
            {
                string StackFolder = ExperimentFolder + "Stack\\";
                string[] Files = Directory.GetFiles(StackFolder);

                List<string> forwards = new List<string>();
                List<string> backs = new List<string>();
                string MidPoint = "";

                for (int i = 0; i < Files.Length; i++)
                {
                    string filename = Path.GetFileNameWithoutExtension(Files[i]);
                    if (filename.EndsWith("n") == true)
                    {
                        backs.Add(Files[i]);
                    }
                    else if (filename.EndsWith("p") == true)
                        forwards.Add(Files[i]);
                    else if (filename.EndsWith("m") == true)
                        MidPoint = Files[i];
                }

                backs.Sort();
                forwards.Sort();

                string[] SortedFiles = new string[backs.Count + 1 + forwards.Count];
                int cc = 0;
                for (int i = 0; i < backs.Count; i++)
                {
                    SortedFiles[cc] = backs[backs.Count - 1 - i];
                    cc++;
                }
                SortedFiles[cc] = MidPoint;
                cc++;
                for (int i = 0; i < forwards.Count; i++)
                {
                    SortedFiles[cc] = forwards[i];
                    cc++;
                }

                StackSelector.Maximum = SortedFiles.Length;
                StackNames = SortedFiles;
                StackSelector.Value = SortedFiles.Length / 2;
            }
            catch { }
        }

        private void StackSelector_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int FrameNumber =0;
                if (rEven.Checked == true)
                    FrameNumber = StackSelector.Value * 2;
                else
                    FrameNumber = StackSelector.Value * 2 + 1;

                string ImageFilename = StackNames[StackSelector.Value];
                pictureBox1.Image = MathHelpLib.MathHelpsFileLoader.FixVisionGateImage(MathHelpLib.MathHelpsFileLoader.Load_Bitmap(ImageFilename), 2).ToBitmap();
                pictureBox1.Invalidate();
            }
            catch { }
            Application.DoEvents();
        }

        private void rEven_CheckedChanged(object sender, EventArgs e)
        {
            StackSelector.Maximum = StackNames.Length / 2;
        }

        private void rOdd_CheckedChanged(object sender, EventArgs e)
        {
            StackSelector.Maximum = StackNames.Length / 2;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            StackSelector.Maximum = StackNames.Length ;
        }


    }
}
