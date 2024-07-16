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
    public partial class FormFixEndings : Form
    {
        public FormFixEndings()
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

                    string basePath = tArchiveFolder .Text + "\\" + Year + month + "\\" + day + "\\" + dirName;

                    if (Directory.Exists(basePath) == true)
                        Attempted++;

                    
                    string  dataPath = basePath + "\\Data\\ProjectionObject.raw";

                    if (File.Exists(dataPath) == true)
                    {
                        PhysicalArray pa = PhysicalArray.OpenDensityData(dataPath);
                        ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder filter = new ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder();
                        filter.DoEffect(null, null, null, pa.ReferenceDataDouble );
                        string Outpath = Path.GetDirectoryName(dataPath) + "\\" + Path.GetFileNameWithoutExtension(dataPath) + ".cct";
                        pa.SaveData(Outpath);
                    }

                    dataPath = basePath + "\\Data\\ProjectionObject.cct";

                    if (File.Exists(dataPath) == true)
                    {
                        PhysicalArray pa = PhysicalArray.OpenDensityData(dataPath);
                        ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder filter = new ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder();
                        filter.DoEffect(null, null, null, pa.ReferenceDataDouble);
                        string Outpath = Path.GetDirectoryName(dataPath) + "\\" + Path.GetFileNameWithoutExtension(dataPath) + ".cct";
                        pa.SaveData(Outpath);
                    }
                    


                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }

            }
           

            MessageBox.Show("Finished");

        }
    }
}
