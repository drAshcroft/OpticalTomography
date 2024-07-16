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
    public partial class FormRemoveEmpty        : Form
    {
        public FormRemoveEmpty()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string STorage = tOutPath.Text;

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
                    pPath = AllDirs[i];


                    if ((File.Exists(pPath + "\\stack\\000_0000m.png") == false && File.Exists(pPath + "\\Data\\ProjectionObject.cct") == false))
                    {
                            Selected.Enqueue(AllDirs[i]);
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
               
                Directory.Delete(DirPath,true  );
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
