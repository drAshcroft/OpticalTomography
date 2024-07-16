using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MathHelpLib;
using Tomographic_Imaging_2;

namespace FixStack
{
    public partial class FormCopyStack : Form
    {
        public FormCopyStack()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string STorage = tArchiveFolder.Text;

            #region GetPossibleFolders
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
            // string[] AllDirs = Directory.GetDirectories(STorage, "cc*.*", SearchOption.TopDirectoryOnly );
            AllDirs = GoodDirs.ToArray();
            #endregion

            #region Select undone folders
            string pPath;
            Queue<string> Selected = new Queue<string>();
            Dictionary<string, string> OutputDirs = new Dictionary<string, string>();
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

                    string basePath = tOutPath.Text + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;

                    if (Directory.Exists(basePath) == true)
                        Attempted++;

                    
                    // basePath += "\\Data\\ProjectionObject.cct";

                    if ((File.Exists(basePath + "\\stack\\000_0000m.png") == false || File.Exists(basePath + "\\Data\\CrossSections_X.jpg") == false) && File.Exists(basePath + "\\Data\\ProjectionObject.cct") == true)
                    {
                        pPath = Path.GetFileNameWithoutExtension(AllDirs[i]);
                        if (pPath.Length > 10 && pPath.Substring(0, 2) == "cc")
                        {
                            OutputDirs.Add(AllDirs[i], basePath);
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
            #endregion
            int NumDirs = AllDirs.Length;

            while (Selected.Count > 0)// (string p in Selected)
            {
                string DirPath = Selected.Dequeue();
                string OutPath = OutputDirs[DirPath];

                string StackPath = OutPath + "\\Stack\\";
                if (Directory.Exists(StackPath) == false)
                    Directory.CreateDirectory(StackPath);

                if (Directory.Exists(DirPath + "\\stack\\000\\") && (File.Exists(StackPath + "000_0000m.png") == false && File.Exists(StackPath + "000_0000m.ivg") == false))
                {
                    string[] StackFiles = Directory.GetFiles(DirPath + "\\stack\\000\\");
                    foreach (string file in StackFiles)
                    {
                        try
                        {
                            File.Copy(file, StackPath + Path.GetFileName(file), true);
                        }
                        catch { }
                    }
                }

                string DataPath = OutPath + "\\data\\";
                if (File.Exists(DataPath + "projectionobject.cct") == true && File.Exists(DataPath + "CrossSections_X.jpg")==false  )
                {
                    try
                    {
                        ProjectionObject po = new ProjectionObject();
                        po.OpenDensityData(DataPath + "ProjectionObject.cct");
                        PhysicalArray density = po.ProjectionData;

                        density.SaveCross(DataPath + "CrossSections.jpg");

                        po = null;
                    }
                    catch { }
                }
                try
                {
                    label1.Text = Selected.Count + " / " + NumDirs + " == " + Math.Round(100-((double)Selected.Count / (double)NumDirs) * 100, 2).ToString() + "%";
                }
                catch { }
                Application.DoEvents();
            }

            MessageBox.Show("Finished");

        }
    }
}
