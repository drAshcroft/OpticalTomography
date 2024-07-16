using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using MathHelpLib;

namespace Rehydrate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            string STorage = tWatchFolder.Text;

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
            string pPath;
            Queue<string> Selected = new Queue<string>();
            int Completed = 0;
            int Attempted = 0;
            for (int i = 0; i < AllDirs.Length; i++)
            {
                try
                {
                    pPath = AllDirs[i].Replace("\"", "").Replace("'", "");

                    if (pPath.EndsWith("\\") == false)
                        pPath += "\\";

                    string dirName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(pPath));
                    string[] parts = dirName.Split('_');
                    string Prefix = parts[0];
                    string Year = parts[1].Substring(0, 4);
                    string month = parts[1].Substring(4, 2);
                    string day = parts[1].Substring(6, 2);

                    string basePath;
                    basePath = Path.Combine(tOutFolder.Text, Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName);


                    if (Directory.Exists(basePath) == true)
                        Attempted++;

                    if (Directory.Exists(basePath) == false && Directory.Exists(AllDirs[i] + "\\Dehydrated\\"))
                    {
                        pPath = Path.GetFileNameWithoutExtension(AllDirs[i]);
                        if (pPath.Length > 10 && pPath.Substring(0, 2) == "cc")
                        {
                            Selected.Enqueue(AllDirs[i]);
                        }
                    }
                    else
                    {
                        pPath = "";
                        Completed++;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }

            }

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            TimeSpan maxWait = new TimeSpan(0, 1, 0);

            int StartFolders = Selected.Count;

            while (Selected.Count > 0)// (string p in Selected)
            {

              //  try
                {
                   
                        string DirPath = Selected.Dequeue();


                        uConsole1.AddLine("Starting dehydration of " + DirPath);

                        string dirName = Path.GetFileNameWithoutExtension(DirPath);
                        string[] parts = dirName.Split('_');
                        string Prefix = parts[0];
                        string Year = parts[1].Substring(0, 4);
                        string month = parts[1].Substring(4, 2);
                        string day = parts[1].Substring(6, 2);
                        string basePath;

                        basePath = Path.Combine(tOutFolder.Text, Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName);
                        Rehydrate(DirPath, basePath);
                }
               // catch (Exception ex)
                {
               //     System.Diagnostics.Debug.Print(ex.Message);
                }

              
                Application.DoEvents();
            }

            this.Text = "Cleaning missed stuff";
        }

        double[] ReadPositionFile(string PosFile)
        {
            double[] DataPoints;
            using (StreamReader sr = new StreamReader(PosFile))
            {
                String line = sr.ReadToEnd();
                string[] Lines = line.Split(new string[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);

                DataPoints = new double[Lines.Length];

                for (int i = 0; i < Lines.Length; i++)
                {
                    DataPoints[i] = double.Parse(Lines[i]);
                }
            }
            return DataPoints;
        }

        public void Rehydrate(string DirFrom, string DirTo)
        {
            if (File.Exists (DirFrom + "\\data\\X_PositionsB"))
            {
                double[] XCenters=ReadPositionFile(DirFrom + "\\data\\X_PositionsB");
                double[] YCenters =ReadPositionFile(DirFrom + "\\data\\Y_PositionsB");
                try
                {
                    Directory.CreateDirectory(DirTo + "\\pp\\");
                }
                catch { }
                for (int i = 0; i < XCenters.Length; i++)
                {
                    ImageHolder Whole=  MathHelpLib.MathHelpsFileLoader.Load_Bitmap(DirFrom + "\\Dehydrated\\Whole" + string.Format("{0:000}", i) + ".jpg");
                    
                    ImageHolder Cell = MathHelpLib.MathHelpsFileLoader.Load_Bitmap(DirFrom + "\\Dehydrated\\Center" + string.Format("{0:000}", i) + ".png");
                    float MaxCell = Cell.GetMax();
                    if (MaxCell > 255)
                    {
                        Whole = Whole * 255;
                    }
                    Whole.DrawUnscaled(Cell,(int)( XCenters[i] - Cell.Width / 2),(int)( YCenters[i] - Cell.Height / 2));
                    Bitmap b = Whole.ToBitmap();
                    b.Save(DirTo + "\\PP\\" + string.Format("{0:000}", i) + ".png");
                    pictureBox1.Image = b;
                    pictureBox1.Invalidate();
                    Application.DoEvents();

                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tWatchFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
