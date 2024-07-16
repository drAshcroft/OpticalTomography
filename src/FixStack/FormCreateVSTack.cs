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
using System.Text.RegularExpressions;

namespace FixStack
{
    public partial class FormCreateVSTack : Form
    {
        public FormCreateVSTack()
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

                    if (Directory.Exists(basePath ) == true)
                    {
                        string VGPath = "Y:\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_cropped_8bit\\";

                        Console.WriteLine(VGPath);

                        string VGFile = null;

                        if (Directory.Exists(VGPath) == true)
                        {
                            string[] Files = Directory.GetFiles(VGPath,"*.png");
                            Files = SortNumberedFiles(Files);
                            VGFile = Files[Files.Length / 2];
                        }
                        else
                        {
                            VGPath = "Y:\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_cropped_16bit\\";
                            if (Directory.Exists(VGPath) == true)
                            {
                                string[] Files = Directory.GetFiles(VGPath, "*.png");
                                Files = SortNumberedFiles(Files);
                                VGFile = Files[Files.Length / 2];
                            }
                        }
                        if (VGFile != null)
                        {
                            //ImageHolder vgExample = ImagingTools.Load_Bitmap(VGFile);
                            //vgExample.Save(DataPath + "VGExample.bmp");
                            //string exten = Path.GetExtension(VGFile);
                            //File.Copy(VGFile, basePath  + "\\data\\VGExample" + exten, true);
                            try
                            {
                                File.Delete(basePath + "\\data\\VGExample.bmp");
                            }
                            catch { }
                            Bitmap b = new Bitmap(VGFile);
                            b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            b.Save(basePath + "\\data\\VGExample.png");
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
           
            MessageBox.Show("Finished");

        }

        public static string[] SortNumberedFiles(string[] UnsortedFilenames)
        {
            Regex objNaturalPattern = new Regex(@"((\b[0-9]+)?\.)?[0-9]+\b");

            //find the minimum value so it will got back in correctly
            int MinFileN = int.MaxValue;
            int BadFiles = 0;
            for (int i = 0; i < UnsortedFilenames.Length; i++)
            {
                MatchCollection mc = objNaturalPattern.Matches(Path.GetFileNameWithoutExtension(UnsortedFilenames[i]));
                if (mc.Count != 0)
                {
                    string Filenumber = mc[0].Value;
                    int iFileNumber = 0;
                    int.TryParse(Filenumber, out iFileNumber);
                    if (iFileNumber < MinFileN) MinFileN = iFileNumber;
                }
                else
                    BadFiles++;
            }

            SortedDictionary<int, string> SortedFiles = new SortedDictionary<int, string>();

            //string[] Filenames = new string[UnsortedFilenames.Length - BadFiles];
            for (int i = 0; i < UnsortedFilenames.Length; i++)
            {
                MatchCollection mc = objNaturalPattern.Matches(Path.GetFileNameWithoutExtension(UnsortedFilenames[i]));
                if (mc.Count != 0)
                {
                    string Filenumber = mc[0].Value;
                    int iFileNumber = 0;
                    int.TryParse(Filenumber, out iFileNumber);
                    //Filenames[iFileNumber - MinFileN] = UnsortedFilenames[i];
                    SortedFiles.Add(iFileNumber, UnsortedFilenames[i]);
                }
            }

            return SortedFiles.Values.ToArray();
        }
    }
}
