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
using DoRecons;

namespace Dehydrate
{
    public partial class Form1 : Form
    {
        private List<ProcessHolder> ProcessHolderList = new List<ProcessHolder>();

        public Form1()
        {
            InitializeComponent();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
           // mPauseBatch = false;
           // mStopBatch = false;

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
                    {
                        Attempted++;
                      /*  if (this.Text != "Cleaning missed stuff")
                        {
                            Directory.Delete(basePath, true);
                        }*/
                    }

                    if ((Directory.Exists(basePath + "\\dehydrated") == false || File.Exists(basePath + "\\dehydrated\\center499.png") == false) && (File.Exists( AllDirs[i] + "\\pp\\499.png")==true || File.Exists( AllDirs[i] + "\\pp\\499.ivg")==true))
                    {
                        pPath = Path.GetFileNameWithoutExtension(AllDirs[i]);
                        if (pPath.Length > 10 && pPath.Substring(0, 2) == "cc")
                        {
                            Selected.Enqueue(AllDirs[i]);
                        }

                      /*  if (Directory.Exists(basePath) == true)
                        {
                            if (this.Text != "Cleaning missed stuff")
                            {
                                Directory.Delete(basePath, true);
                            }
                        }*/
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
               
                try
                {
                    if ( ProcessHolderList.Count < 3)
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

                        Dictionary<string, string> Properties = new Dictionary<string, string>();
                        Properties.Add("TempDirectory", Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + dirName);
                        Properties.Add("DataDirectory", DirPath);
                        Properties.Add("DataOut", basePath);
                        Properties.Add("port", "1100");
                        Properties.Add("WaitFiles", true.ToString());
                        Properties.Add("Dehydrate", true.ToString());
                        Properties.Add("GlobalFlatten", true.ToString());
                        Properties.Add("BackgroundSubMethod", "TopAndBottom");
                        Properties.Add("COGMethod", "Threshold");
                        Properties.Add("FlatMethod", "plane");
                        string[] dArgs = new string [Properties.Count *2];
                        int cc=0;
                        string Args = "";
                        foreach (KeyValuePair<string, string> kvp in Properties)
                        {
                            Args += "\"" + kvp.Key.Trim() + "\" \"" + kvp.Value.ToString() + "\" ";
                            dArgs[cc]=kvp.Key.Trim();
                            dArgs[cc+1]=kvp.Value.ToString();
                            cc+=2;
                        }

                        System.Diagnostics.Debug.Print(Args);

                        Process ScriptRunner = new Process();
                        ScriptRunner.StartInfo.WindowStyle = ProcessWindowStyle.Minimized ;
                        ScriptRunner.StartInfo.FileName ="DoRecons.exe";
                        ScriptRunner.StartInfo.Arguments = Args;
                        ScriptRunner.Start();

                        ProcessHolder ph = new ProcessHolder(ScriptRunner, Properties["TempDirectory"]);
                        ph.Path = DirPath;
                        ProcessHolderList.Add(ph);

                        //DoRecons.Program.MainScriptRunner(dArgs);
                        sw.Restart();

                        uConsole1.AddLine(Selected.Count + "/" + StartFolders + "    " + Math.Round((1 - Selected.Count / (double)StartFolders) * 100) + "%");

                        //while (sw.Elapsed < maxWait && ScriptRunner.HasExited == false)
                         //   Application.DoEvents();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }

                try
                {
                    for (int i = 0; i < ProcessHolderList.Count; i++)
                    {
                        if (ProcessHolderList[i].CheckTime() == false)
                        {

                            ProcessHolderList.RemoveAt(i);
                            Application.DoEvents();
                        }
                    }
                }
                catch { }
                Application.DoEvents();
            }

            if (this.Text != "Cleaning missed stuff")
            {
                this.Text = "Cleaning missed stuff";
                bStart_Click(this, e);
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
