using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

using MathHelpLib.CurveFitting;
using MathHelpLib;
using Emgu.CV.Structure;

namespace DoRecons.PageViews
{
    public partial class NetWorkWatcher : UserControl
    {
        public NetWorkWatcher()
        {
            InitializeComponent();
        }

        public ReconWorkFlow reconWorkFlow1;

        public Dictionary<string, string> SaveReconProperties()
        {
            return reconWorkFlow1.SaveGUI();
        }
        public void SetReconProperties(Dictionary<string, string> Props)
        {
            reconWorkFlow1.SetupControl(Props);
        }

        public void NetworkMessage(string message)
        {
            uConsole1.AddLine(message);
        }

        List<string> AlreadyCaught = new List<string>();
        private void fileSystemWatcher1_Created(object sender, System.IO.FileSystemEventArgs e)
        {

            // uConsole1.AddLine ("Creation detected : " +  e.FullPath);
            string junk = e.FullPath;
            ProcessFolder(junk);
        }

        private void ProcessFolder(string Foldername)
        {
            Dictionary<string, string> Properties = reconWorkFlow1.SaveGUI();


            string[] Parts = Foldername.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);

            string dir = Path.GetDirectoryName(Foldername);
            string ExperimentFolder = "";

            string[] dirParts = Foldername.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            bool NotFound = true;
            for (int i = 0; i < dirParts.Length; i++)
            {
                ExperimentFolder += dirParts[i];
                if (dirParts[i].StartsWith("cct") == true)
                {
                    NotFound = false;
                    break;
                }
                ExperimentFolder += "\\";
            }

            if (Directory.Exists(ExperimentFolder + "\\pp") == true)
            {


                if (NotFound == false)
                {
                    string DataPath = ExperimentFolder;
                    if (Directory.Exists(DataPath + "\\pp") == true)
                    {
                        if (AlreadyCaught.Contains(DataPath) == false)
                        {
                            AlreadyCaught.Add(DataPath);

                            uConsole1.AddLine("Starting reconstruction of " + DataPath);

                            string dirName = Path.GetFileNameWithoutExtension(DataPath);
                            string[] parts = dirName.Split('_');
                            string Prefix = parts[0];
                            string Year = parts[1].Substring(0, 4);
                            string month = parts[1].Substring(4, 2);
                            string day = parts[1].Substring(6, 2);

                            string basePath;

                            if (tOutFolder.Text.EndsWith("\\"))
                                basePath = tOutFolder.Text + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;
                            else
                                basePath = tOutFolder.Text + "\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;




                            if (Properties["LoadPreProcessed"] == "True" && Directory.Exists(basePath + "\\dehydrated") != true)
                            {
                                Properties.Remove("LoadPreProcessed");
                                Properties.Add("LoadPreProcessed", "False");
                            }

                            Properties.Add("StrictBackground", false.ToString());
                            Properties.Add("TempDirectory", Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + dirName);
                            Properties.Add("DataDirectory", DataPath);
                            Properties.Add("DataOut", basePath);
                            Properties.Add("port", Program.Port.ToString());


                            // Properties.Add("MoveFiles", true.ToString());
                            string Args = "";
                            foreach (KeyValuePair<string, string> kvp in Properties)
                                Args += "\"" + kvp.Key.Trim() + "\" \"" + kvp.Value.ToString() + "\" ";
                            System.Diagnostics.Debug.Print(Args);

                            Process ScriptRunner = new Process();
                            ScriptRunner.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                            ScriptRunner.StartInfo.FileName = Application.ExecutablePath;
                            ScriptRunner.StartInfo.Arguments = Args;
                            ScriptRunner.Start();


                            /*   string[] Args = new string[Properties.Count * 2];
                               int cc = 0;
                               foreach (KeyValuePair<string, string> kvp in Properties)
                               {
                                   Args[cc] = kvp.Key;
                                   Args[cc + 1] = kvp.Value;
                                   cc += 2;
                               }

                               Program.MainScriptRunner(Args);*/

                        }
                    }
                }
            }

        }

        private void bStartWatch_Click(object sender, EventArgs e)
        {
            tWatchFolder.Enabled = false;
            tOutFolder.Enabled = false;
            fileSystemWatcher1.Path = tWatchFolder.Text;
            fileSystemWatcher1.EnableRaisingEvents = true;

            uConsole1.AddLine("Watching folder: " + tWatchFolder.Text);

            List<string> AlreadyDone = new List<string>();

            while (bStop == false)
            {
                string[] folders = Directory.GetDirectories(tWatchFolder.Text);

                for (int i = 0; i < folders.Length; i++)
                {
                    if (AlreadyDone.Contains(folders[i]) == false)
                    {
                        AlreadyDone.Add(folders[i]);
                        ProcessFolder(folders[i]);
                        // for (int min = 0; min < 6; min++)
                        {
                            //  for (int sec = 0; sec < 60; sec++)
                            {
                                //     for (int milli = 0; milli < 1000; milli += 80)
                                {
                                    Thread.Sleep(80);
                                    Application.DoEvents();
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(100);
                Application.DoEvents();
            }
        }
        private bool bStop = false;
        private void bStopWatch_Click(object sender, EventArgs e)
        {
            bStop = true;
            tWatchFolder.Enabled = true;
            tOutFolder.Enabled = true;
            fileSystemWatcher1.EnableRaisingEvents = false;
        }

        private void bBrowseWatch_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tWatchFolder.Text = folderBrowserDialog1.SelectedPath;

            }
        }

        private void bBrowseOutput_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tOutFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }


        private void MoveDirectory(string SourceDirectory, string DestDirectory)
        {

            foreach (string dir in Directory.GetDirectories(SourceDirectory))
            {
                string newDest = DestDirectory + "\\" + Path.GetFileName(dir);
                if (Directory.Exists (newDest )==false )
                    Directory.CreateDirectory (newDest );
                
                MoveDirectory(dir, newDest );
            }

            foreach (string file in Directory.GetFiles(SourceDirectory))
            {
                string newFile = DestDirectory +"\\" + Path.GetFileName(file);
               // System.Diagnostics.Debug.Print(file);
               // System.Diagnostics.Debug.Print(newFile);
                if (File.Exists (newFile )==false )
                    File.Move(file, newFile);
            }

        }

        private void MoveDirectory(Tuple<string, string> info)
        {
            MoveDirectory(info.Item1, info.Item2);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            //string[] dirs = Directory.GetDirectories(@"V:\Raw PP\cct001\Fluorescence\cct001","*.*",SearchOption.AllDirectories);

            // string target = @"V:\Raw PP\cct001\Fluorescence\Cleaned";

           // string[] dirs = Directory.GetDirectories(@"X:\Rejected_Datasets");
            List<string> dirs = new List<string>();

            //string[]  maindirs = Directory.GetDirectories(@"V:\Raw PP\cct001\Fluorescence\cct001") ;
            //// string target = @"V:\Raw PP\cct001\Fluorescence\Cleaned";
            //foreach (string s in maindirs)
            //{
            //    string[] dir2 = Directory.GetDirectories(s);
            //    foreach (string s2 in dir2)
            //    {
            //        dirs.AddRange (Directory.GetDirectories(s2));
            //    }
            //}
            //dirs.Clear();
            dirs.AddRange(Directory.GetDirectories(@"X:\Rejected_Datasets"));
          //  dirs.AddRange(Directory.GetDirectories(@"V:\Raw PP\cct001\Fluorescence\empties"));
           

            foreach (string s in dirs)
            {
                string filename = Path.GetFileNameWithoutExtension(s);

                if (filename.Length > 8 && filename.Substring(0, 3) == "cct")
                {
                    try
                    {

                        string dirName = Path.GetFileNameWithoutExtension(s);
                        string[] parts = dirName.Split('_');
                        string Prefix = parts[0];
                        string Year = parts[1].Substring(0, 4);
                        string month = parts[1].Substring(4, 2);
                        string day = parts[1].Substring(6, 2);

                        string basePath = @"V:\Raw PP\cct001\Fluorescence\" + Prefix + "\\" + Year + month + "\\" + day;
                        string target = @"V:\Raw PP\cct001\Fluorescence\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;



                        if (Directory.Exists(s + "\\pp") == true)
                        {
                            string[] files = Directory.GetFiles(s + "\\pp");
                            if (files.Length > 400)
                            {

                                ImageHolder ih = null;
                                ih = new ImageHolder(files[10]);

                                if (!(ih.Width == 512 || ih.Width == 1024))
                                {
                                    basePath = @"V:\Raw PP\cct001\Fluorescence\absorb rejects\" + Prefix + "\\" + Year + month + "\\" + day;
                                    target = @"V:\Raw PP\cct001\Fluorescence\absorb rejects\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;


                                    //   if (Directory.Exists(target) == false)
                                    {
                                        if (Directory.Exists(basePath) == false)
                                            Directory.CreateDirectory(basePath);
                                        MoveDirectory(s, target);
                                    }
                                    Directory.Delete(s, true);
                                }
                                else
                                {

                                    if (Directory.Exists(basePath) == false)
                                        Directory.CreateDirectory(basePath);

                                    Thread t = new Thread(delegate() 
                                        { 
                                            MoveDirectory(s, target); 
                                        });
                                    t.Start();
                                    Directory.Delete(s, true);
                                }
                            }
                            else
                            {
                                basePath = @"V:\Raw PP\cct001\Fluorescence\empties\" + Prefix + "\\" + Year + month + "\\" + day;
                                target = @"V:\Raw PP\cct001\Fluorescence\empties\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;


                                //  if (Directory.Exists(target) == false)
                                // {
                                if (Directory.Exists(basePath) == false)
                                    Directory.CreateDirectory(basePath);

                                MoveDirectory(s, target);
                                Directory.Delete(s, true);
                                //}
                            }
                        }
                        else
                        {

                            if (Directory.Exists(s + "\\pp") == true)
                            {
                                basePath = @"V:\Raw PP\cct001\Fluorescence\empties\" + Prefix + "\\" + Year + month + "\\" + day;
                                target = @"V:\Raw PP\cct001\Fluorescence\empties\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;


                                //if (Directory.Exists(target) == false)
                                {
                                    if (Directory.Exists(basePath) == false)
                                        Directory.CreateDirectory(basePath);

                                    MoveDirectory(s, target);
                                }
                                Directory.Delete(s, true);
                            }
                            else if (Directory.Exists(s + "\\data") == true)
                            {
                                basePath = @"V:\Raw PP\cct001\Fluorescence\Processed\" + Prefix + "\\" + Year + month + "\\" + day;
                                target = @"V:\Raw PP\cct001\Fluorescence\Processed\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;


                                //if (Directory.Exists(target) == false)
                                {
                                    if (Directory.Exists(basePath) == false)
                                        Directory.CreateDirectory(basePath);

                                    MoveDirectory(s, target);
                                }
                                Directory.Delete(s, true);
                            }
                            else
                            {
                                basePath = @"V:\Raw PP\cct001\Fluorescence\empties\" + Prefix + "\\" + Year + month + "\\" + day;
                                target = @"V:\Raw PP\cct001\Fluorescence\empties\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;


                                //if (Directory.Exists(target) == false)
                                {
                                    if (Directory.Exists(basePath) == false)
                                        Directory.CreateDirectory(basePath);

                                    MoveDirectory(s, target);
                                }
                                Directory.Delete(s, true);
                            }
                        }
                    }
                    catch(Exception ex) {
                        System.Diagnostics.Debug.Print(ex.Message);
                    }


                    }
              
            }


            /* foreach (string s in dirs)
             {
                 string filename = Path.GetFileNameWithoutExtension(s);

                 if (s.Contains("EPC"))
                     System.Diagnostics.Debug.Print("");
                 if (filename.Contains("cct001") == true && filename.Length > 16)
                 {

                     if (Directory.Exists(s + "\\pp") == true)
                     {
                         try
                         {
                             Directory.Move(s, target + "\\" + filename);
                         }
                         catch { }
                     }

                 }
             }*/

            /* PythonHelps.OpenXMLAndProjectionLocations(@"Y:\cct001\201112\02\cct001_20111202_111558\PPDetailReport.xml",null);

             return;
             float[, ,] f = MathHelpLib.MathHelpsFileLoader.OpenDensityDataFloat(@"C:\Development\CellCT\DataIN\cct001_20120629_103335\Data\ProjectionObject.tif");
            //double ave, max, min, sd;
            //long[] Curvatures;
            //MathHelpLib._3DStuff._3DInterpolation.GetCurvature(ref f,30000, out ave, out max, out min, out sd, out Curvatures, out sd);
             GraphingLib.vtkForm graphForm = new GraphingLib.vtkForm();
             graphForm.Show();
             Application.DoEvents();
           //  graphForm.GetViewer().ShowVolumeData(f);
             graphForm.GetViewer().IsoSurface(f, 60000, false, Color.Red);
          //   PythonHelps.EvaluateRecon(f, "C:\\Development\\CellCT\\DataIN\\cct001_20120319_084253\\stack\\000", new Point(754,	673), 352);*/

        }
    }
}
