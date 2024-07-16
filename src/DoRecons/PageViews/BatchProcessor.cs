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
using MathHelpLib;
using MathHelpLib.ImageProcessing;
using ImageViewer;

namespace DoRecons.PageViews
{
    public partial class BatchProcessor : UserControl
    {
        private List<ProcessHolder> ProcessHolderList = new List<ProcessHolder>();

        private bool mPauseBatch = false;

        private bool mStopBatch = false;

        public ReconWorkFlow reconWorkFlow1;

        public BatchProcessor()
        {
            InitializeComponent();
        }

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

        private bool HasAttribute(string basePath, string Attribute)
        {
            if (File.Exists((basePath + "\\data\\comments.txt")) == true)
            {
                using (StreamReader sr = new StreamReader(basePath + "\\data\\comments.txt"))
                {
                    String line;
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    line = sr.ReadToEnd();


                    if (line.Contains(Attribute) == true)
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        private void bStart_Click(object sender, EventArgs e)
        {
            //CleanBackgrounds();
            mPauseBatch = false;
            mStopBatch = false;

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


            CreateBackgrounds();

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

                    string dehydratefolder = "e:\\Dehydrated\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;
                    if (Directory.Exists(basePath) == true)
                        Attempted++;

                    string ProjectionFile = basePath + "\\data\\ProjectionObject.tif";

                    if (Directory.Exists(AllDirs[i] + "\\pp\\") && Directory.Exists(AllDirs[i] + "\\stack\\"))
                    {

                        string backgroundpath;
                        //  backgroundpath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + Prefix + "\\" + Year + month + "\\" + day + "\\back_" + dirName + ".tif";
                        backgroundpath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + Prefix + "\\" + Year + month + "\\" + day + "\\background.tif"; ;

                        bool alreadyDone = true;

                        if (File.Exists(ProjectionFile) == false)
                            alreadyDone = false;



                        if (File.Exists((basePath + "\\data\\comments.txt")) == true)
                        {
                            using (StreamReader sr = new StreamReader(basePath + "\\data\\comments.txt"))
                            {
                                String line;
                                // Read and display lines from the file until the end of
                                // the file is reached.
                                line = sr.ReadToEnd();


                                if (line.Contains("Unable to get background") == true || line.Contains("unable to get background") == true
                                    || line.Contains("unable to create") == true)
                                {
                                    alreadyDone = false;
                                }

                                if (line.Contains("<Run Time>") == false)
                                    alreadyDone = false;

                                try
                                {
                                    string[] lines = line.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                                    for (int kk = 0; kk < lines.Length; kk++)
                                    {
                                        line = lines[kk];
                                        bool found = line.Contains("<Run Time>");
                                        if (found)
                                        {
                                            string[] dParts = line.Split(new string[] { "<", "/>", ">" }, StringSplitOptions.RemoveEmptyEntries);
                                            string[] t = dParts[1].Split(new string[] { " ", "/" }, StringSplitOptions.RemoveEmptyEntries);
                                            DateTime dt = new DateTime(int.Parse(t[2]), int.Parse(t[0]), int.Parse(t[1]));
                                            if (dt < (new DateTime(2012, 7, 16)))
                                            {
                                                alreadyDone = false;
                                            }
                                            break;
                                        }
                                    }
                                }
                                catch 
                                {
                                    alreadyDone = false;
                                }
                            }

                            if (HasAttribute(basePath, "Bad Centering"))
                                alreadyDone = false;

                           // if (HasAttribute(basePath, "Bad Focus"))
                           //     alreadyDone = false;

                        }
                        alreadyDone = false;
                        if (alreadyDone == false)
                        {
                            pPath = Path.GetFileNameWithoutExtension(AllDirs[i]);
                            if (pPath.Length > 10 && pPath.Substring(0, 2) == "cc")
                            {
                                // if (Directory.Exists(dehydratefolder )==false)
                                Selected.Enqueue(AllDirs[i]);
                            }
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
            TimeSpan maxWait = new TimeSpan(0, 0, 45);

            int StartFolders = Selected.Count;

            while (Selected.Count > 0 && mStopBatch == false)// (string p in Selected)
            {
                if (mPauseBatch == true)
                {
                    while (mPauseBatch)
                    {
                        Application.DoEvents();
                        Thread.Sleep(10);
                    }
                }
                try
                {
                    if (ProcessHolderList.Count < nNumRunning.Value)
                    {
                        string DirPath = Selected.Dequeue();

                        Dictionary<string, string> Properties = reconWorkFlow1.SaveGUI();

                        uConsole1.AddLine("Starting reconstruction of " + DirPath);

                        string dirName = Path.GetFileNameWithoutExtension(DirPath);
                        string[] parts = dirName.Split('_');
                        string Prefix = parts[0];
                        string Year = parts[1].Substring(0, 4);
                        string month = parts[1].Substring(4, 2);
                        string day = parts[1].Substring(6, 2);


                        string basePath;

                        basePath = Path.Combine(tOutFolder.Text, Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName);


                        if (HasAttribute(basePath, "Bad Centering"))
                            Properties.Add("Centering", "Bad");

                        if (Properties["LoadPreProcessed"] == "True" && Directory.Exists(basePath + "\\dehydrated") != true)
                        {
                            Properties.Remove("LoadPreProcessed");
                            Properties.Add("LoadPreProcessed", "False");
                        }

                        Properties.Add("StrictBackground", false.ToString());
                        Properties.Add("TempDirectory", Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + dirName);
                        Properties.Add("DataDirectory", DirPath);
                        Properties.Add("DataOut", basePath);
                        Properties.Add("port", Program.Port.ToString());
                        Properties.Add("Iteration", TryN);
                        //Properties.Add("WaitFiles", true.ToString());

                        string Args = "";
                        foreach (KeyValuePair<string, string> kvp in Properties)
                            Args += "\"" + kvp.Key.Trim() + "\" \"" + kvp.Value.ToString() + "\" ";
                        System.Diagnostics.Debug.Print(Args);

                        sw.Restart();

                        uConsole1.AddLine(Selected.Count + "/" + StartFolders + "    " + Math.Round((1 - Selected.Count / (double)StartFolders) * 100) + "%");

                        if (false   )
                        {
                            string[] Args2 = new string[Properties.Count * 2];
                            int cc = 0;
                            foreach (KeyValuePair<string, string> kvp in Properties)
                            {
                                Args2[cc] = kvp.Key;
                                Args2[cc + 1] = kvp.Value;
                                cc += 2;
                            }
                            Program.MainScriptRunner(Args2);
                        }
                        else
                        {
                            Process ScriptRunner = new Process();
                            ScriptRunner.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                            ScriptRunner.StartInfo.FileName = Application.ExecutablePath;
                            ScriptRunner.StartInfo.Arguments = Args;
                            ScriptRunner.Start();

                            ProcessHolder ph = new ProcessHolder(ScriptRunner, Properties["TempDirectory"]);
                            ph.Path = DirPath;
                            ProcessHolderList.Add(ph);

                            // while ( ScriptRunner.HasExited == false)
                            //    Application.DoEvents();

                            while (sw.Elapsed < maxWait && ScriptRunner.HasExited == false)
                                Application.DoEvents();
                        }
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

            this.Text = "Cleaning missed stuff";
            // CleanBackgrounds();
            //  MessageBox.Show("All Recons are finished");
        }

        private void CreateBackgrounds()
        {
            string backgroundpath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\cct001";
            if (Directory.Exists(backgroundpath) == false)
                Directory.CreateDirectory(backgroundpath);
            string[] BackFolders = Directory.GetDirectories(backgroundpath, "*.*", SearchOption.AllDirectories);

            bool BackgroundFound = false;
            foreach (string folders in BackFolders)
            {
                if (File.Exists(folders + "\\background.tif") == false)
                {
                    try
                    {
                        double[,] AverageBackGround = null;
                        int cc = 0;
                        string LastFolderName = "";
                        string[] Files = Directory.GetFiles(folders, "*.tif");

                        if (Files.Length == 1)
                            File.Copy(Files[0], folders + "\\background.tif");
                        else if (Files.Length > 0)
                        {
                            List<ImageHolder> images = new List<ImageHolder>();
                            for (int i = 0; i < Files.Length; i++)//string ExperimentFile in Files)
                            {
                                try
                                {
                                    if (Path.GetFileNameWithoutExtension(Files[i]) != "Background")
                                    {
                                        ImageHolder ih = MathHelpsFileLoader.Load_Tiff(Files[i]);
                                        if (i != 0 && (ih.Width == 900 || ih.Width == 450))
                                            images.Add(ih);
                                    }
                                }
                                catch { }
                            }

                            if (images.Count == 0)
                                System.Diagnostics.Debug.Print("");
                            else
                            {
                                int MaxWidth = 0;
                                for (int i = 0; i < images.Count; i++)
                                {
                                    if (images[i].Width > MaxWidth)
                                        MaxWidth = images[i].Width;
                                }

                                for (int i = 0; i < images.Count; i++)
                                {
                                    if (images[i].Width != MaxWidth)
                                        images.RemoveAt(i);
                                }

                                ImageHolder ImageOut = new ImageHolder(images[0].Width, images[0].Height, 1);

                                int iK = 0;
                                List<float> PixelVals = new List<float>(new float[images.Count]);
                                for (int i = 0; i < images[0].ImageData.GetLength(0); i++)
                                    for (int j = 0; j < images[0].ImageData.GetLength(1); j++)
                                    {
                                        try
                                        {
                                            for (int k = 0; k < images.Count; k++)
                                            {
                                                iK = k;
                                                PixelVals[k] = images[k].ImageData[i, j, 0];
                                            }
                                            PixelVals.Sort();
                                            if (PixelVals.Count > 2)
                                                ImageOut.ImageData[i, j, 0] = PixelVals[(int)(PixelVals.Count * 3 / 4d)];
                                            else
                                                ImageOut.ImageData[i, j, 0] = PixelVals[0];
                                        }
                                        catch (Exception ex)
                                        {
                                            System.Diagnostics.Debug.Print(ex.Message);
                                        }
                                        iK++;
                                    }

                                iK += 1;
                                MathHelpsFileLoader.Save_TIFF(folders + " \\Background.tif", ImageOut);
                                BackgroundFound = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(ex.Message);
                    }
                }
            }

        }

        public void CleanBackgrounds()
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
            // string[] AllDirs = Directory.GetDirectories(STorage, "cc*.*", SearchOption.TopDirectoryOnly );
            AllDirs = GoodDirs.ToArray();

            string pPath;
            Queue<string> Selected = new Queue<string>();
            //Dictionary<string, List<string>> ExistingBackgrounds = new Dictionary<string, List<string>>();
            List<string> ExistingBackgrounds = new List<string>();
            int Completed = 0;
            int Attempted = 0;


            for (int i = 0; i < AllDirs.Length; i++)
            {
                try
                {
                    if (mPauseBatch == true)
                    {
                        while (mPauseBatch)
                        {
                            Application.DoEvents();
                            Thread.Sleep(10);
                        }
                    }

                    pPath = AllDirs[i].Replace("\"", "").Replace("'", "");

                    if (pPath.EndsWith("\\") == false)
                        pPath += "\\";

                    string dirName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(pPath));
                    string[] parts = dirName.Split('_');
                    string Prefix = parts[0];
                    string Year = parts[1].Substring(0, 4);
                    string month = parts[1].Substring(4, 2);
                    string day = parts[1].Substring(6, 2);

                    string basePath = Path.Combine(tOutFolder.Text, Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName);
                    string ExperimentFolder = basePath;
                    if (Directory.Exists(basePath) == true)
                        Attempted++;

                    // basePath += "\\Data\\Comments.txt";
                    basePath += "\\Data\\ProjectionObject.tif";

                    if (File.Exists(basePath) == false)
                    {
                        // string userComments = basePath + "\\data\\usercomments.txt";
                        bool DoAdd = true;
                        /* if (File.Exists(userComments) == true)
                         {
                             using (StreamReader sr = new StreamReader("TestFile.txt"))
                             {
                                 String line = sr.ReadToEnd();
                                 if (line.Contains("UserImageQuality==") == true)
                                 {
                                     string[] parts2 = line.Split(new string[]{"\n"}, StringSplitOptions.None);
                                     foreach (string s in parts2 ) 
                                     {
                                         if (s.Contains("UserImageQuality==") == true)
                                         {
                                             if (s.Contains("Choose") == false)
                                                 DoAdd = false;

                                         }
                                     }

                                 }
                             }
                         }*/

                        if (DoAdd)
                        {
                            pPath = Path.GetFileNameWithoutExtension(AllDirs[i]);
                            if (pPath.Length > 10 && pPath.Substring(0, 2) == "cc")
                            {
                                Selected.Enqueue(AllDirs[i]);
                            }
                        }
                    }
                    else
                    {
                        string backpath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + Prefix + "\\" + Year + month + "\\" + day;

                        if (ExistingBackgrounds.Contains(backpath) == false)//.ContainsKey(backpath) == false)
                        {
                            ExistingBackgrounds.Add(backpath);
                            //ckistingBackgrounds.Add(Prefix + Year + month + day, new List<string>());
                        }

                        //ExistingBackgrounds[Prefix + Year + month + day].Add(ExperimentFolder);
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
                if (ProcessHolderList.Count < 4)
                {
                    string DirPath = Selected.Dequeue();

                    Dictionary<string, string> Properties = reconWorkFlow1.SaveGUI();
                    uConsole1.AddLine("Starting reconstruction of " + DirPath);

                    string dirName = Path.GetFileNameWithoutExtension(DirPath);
                    string[] parts = dirName.Split('_');
                    string Prefix = parts[0];
                    string Year = parts[1].Substring(0, 4);
                    string month = parts[1].Substring(4, 2);
                    string day = parts[1].Substring(6, 2);


                    string basePath;
                    basePath = Path.Combine(tOutFolder.Text, Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName);

                    bool BackgroundFound = true;
                    //  Properties.Add("TempDirectory", Path.GetDirectoryName(Application.ExecutablePath) + "\\temp");
                    Properties.Add("DataDirectory", DirPath);
                    Properties.Add("DataOut", basePath);
                    Properties.Add("port", Program.Port.ToString());
                    Properties.Add("WaitFiles", true.ToString());
                    Properties.Add("StrictBackground", BackgroundFound.ToString());

                    if (Properties["LoadPreProcessed"] == "True")
                    {
                        Properties.Remove("LoadPreProcessed");
                        Properties.Add("LoadPreProcessed", "False");
                    }


                    Properties.Add("TempDirectory", Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + dirName);


                    string Args = "";
                    foreach (KeyValuePair<string, string> kvp in Properties)
                        Args += "\"" + kvp.Key.Trim() + "\" \"" + kvp.Value.ToString() + "\" ";
                    System.Diagnostics.Debug.Print(Args);

                    Process ScriptRunner = new Process();
                    ScriptRunner.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    ScriptRunner.StartInfo.CreateNoWindow = false;

                    ScriptRunner.StartInfo.FileName = Application.ExecutablePath;
                    ScriptRunner.StartInfo.Arguments = Args;
                    Clipboard.SetText(Args);
                    ScriptRunner.Start();

                    ProcessHolder ph = new ProcessHolder(ScriptRunner, Properties["TempDirectory"]);
                    ph.Path = DirPath;
                    ProcessHolderList.Add(ph);

                    sw.Restart();
                    uConsole1.AddLine(Selected.Count + "/" + StartFolders + "    " + Math.Round((1 - Selected.Count / (double)StartFolders) * 100) + "%");
                    while (sw.Elapsed < maxWait && ScriptRunner.HasExited == false)
                        Application.DoEvents();
                }

                for (int i = 0; i < ProcessHolderList.Count; i++)
                {
                    if (ProcessHolderList[i].CheckTime() == false)
                        ProcessHolderList.RemoveAt(i);
                }
            }
            // MessageBox.Show("Finished Second Process");*/


        }

        private double[,] GetBackgroundImageOneCell(string ExperimentFolderPath, bool Fussy)
        {
            string DataPath = ExperimentFolderPath + "Data\\";

            if (Fussy == true)
            {
                if (File.Exists(DataPath + "comments.txt") == false)
                {
                    return null;
                }

                String line;
                using (StreamReader sr = new StreamReader(DataPath + "comments.txt"))
                {
                    line = sr.ReadToEnd();
                }

                line = line.ToLower();
                string[] Parts = line.Split(new string[] { "\r", "\n", "<", "/>", ">", "," }, StringSplitOptions.RemoveEmptyEntries);
                string NumberOfCells = "";

                for (int i = 0; i < Parts.Length; i++)
                {
                    if (Parts[i].Contains("numberofcells") == true)
                    {
                        NumberOfCells = Parts[i + 1];
                    }
                }

                if (NumberOfCells.Trim() == "1")
                {
                    double[,] ih = MathHelpsFileLoader.Load_RawToDouble(DataPath + "Background.cct");
                    return ih;
                }
            }
            else
            {
                double[,] ih = MathHelpsFileLoader.Load_RawToDouble(DataPath + "Background.cct");
                return ih;
            }
            return null;
        }

        private void bPause_Click(object sender, EventArgs e)
        {
            mPauseBatch = !mPauseBatch;
        }

        private void bStop_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
            {
                tWatchFolder.Text = folderBrowserDialog1.SelectedPath;

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
            {
                tOutFolder.Text = folderBrowserDialog1.SelectedPath;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] FirstFolders = new string[] { @"C:\TestItems" };

            Filtering.FilterTypes[] DesiredFilters = new Filtering.FilterTypes[] { Filtering.FilterTypes.Han };

            int filterN = 0;
            //for (int filterN = 0; filterN < DesiredFilters.Length; filterN++)
            {
                Filtering.FilterTypes CurFilter = DesiredFilters[filterN];// Filtering.FilterTypes.RamLak;

                reconWorkFlow1.SetFilter(CurFilter);
                foreach (string s in FirstFolders)
                {
                    tWatchFolder.Text = s;
                    bStart_Click(this, EventArgs.Empty);
                }
            }
        }

        private string TryN = "First";
        private void bManyDir_Click(object sender, EventArgs e)
        {
            string DirPath = tWatchFolder.Text;
            List<string> Dirs = new List<string>(Directory.GetDirectories(DirPath));

            Dirs.Sort();
            for (int i = Dirs.Count - 1; i >= 0; i--)
            {
                List<string> subdirs = new List<string>(Directory.GetDirectories(Dirs[i]));
                subdirs.Reverse();
                foreach (string s in subdirs)
                {
                    try
                    {
                        //if (s.Contains("08") == false)
                        {

                            tWatchFolder.Text = s;
                            TryN = "First";
                            bStart_Click(this, e);
                            TryN = "Second";
                            // bStart_Click(this, e);
                        }
                    }
                    catch { }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            //CleanBackgrounds();
            mPauseBatch = false;
            mStopBatch = false;

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

                    string dehydratefolder = "e:\\Dehydrated\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;
                    if (Directory.Exists(basePath) == true)
                        Attempted++;

                    string ProjectionFile = basePath + "\\data\\ProjectionObject.tif";

                    if (Directory.Exists(AllDirs[i] + "\\pp\\") && Directory.Exists(AllDirs[i] + "\\stack\\"))
                    {

                        string backgroundpath;
                        backgroundpath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + Prefix + "\\" + Year + month + "\\" + day + "\\background.tif"; ;

                        bool alreadyDone = true;

                        if (File.Exists(ProjectionFile) == true && File.Exists(basePath + "\\data\\stack.tif") == false)
                        {
                            pPath = Path.GetFileNameWithoutExtension(AllDirs[i]);
                            if (pPath.Length > 10 && pPath.Substring(0, 2) == "cc")
                            {
                                Selected.Enqueue(AllDirs[i]);
                            }
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
            TimeSpan maxWait = new TimeSpan(0, 0, 45);

            int StartFolders = Selected.Count;

            StreamWriter Wholefile = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + "focus scores.csv", true);


            while (Selected.Count > 0 && mStopBatch == false)// (string p in Selected)
            {
                if (mPauseBatch == true)
                {
                    while (mPauseBatch)
                    {
                        Application.DoEvents();
                        Thread.Sleep(10);
                    }
                }
                try
                {
                    if (ProcessHolderList.Count < nNumRunning.Value)
                    {
                        string DirPath = Selected.Dequeue();

                        Dictionary<string, string> Properties = reconWorkFlow1.SaveGUI();

                        uConsole1.AddLine("Starting reconstruction of " + DirPath);

                        string dirName = Path.GetFileNameWithoutExtension(DirPath);
                        string[] parts = dirName.Split('_');
                        string Prefix = parts[0];
                        string Year = parts[1].Substring(0, 4);
                        string month = parts[1].Substring(4, 2);
                        string day = parts[1].Substring(6, 2);


                        string basePath;

                        basePath = Path.Combine(tOutFolder.Text, Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName);


                       // if (HasAttribute(basePath, "Bad Centering"))
                        //    Properties.Add("Centering", "Bad");

                        if (Properties["LoadPreProcessed"] == "True" && Directory.Exists(basePath + "\\dehydrated") != true)
                        {
                            Properties.Remove("LoadPreProcessed");
                            Properties.Add("LoadPreProcessed", "False");
                        }


                        Properties.Add("Centering", "Bad");

                        Properties.Add("StrictBackground", false.ToString());
                        Properties.Add("TempDirectory", Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + dirName);
                        Properties.Add("DataDirectory", DirPath);
                        Properties.Add("DataOut", basePath);
                        Properties.Add("port", Program.Port.ToString());
                        Properties.Add("Iteration", TryN);
                        //Properties.Add("WaitFiles", true.ToString());

                        double stackF4 = 1;
                        double onAxisF4 = 1;
                        try
                        {


                           



                            string backPath;
                            backPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + Prefix + "\\" + Year + month + "\\" + day;

                            backPath += "\\background.tif";
                            Console.WriteLine(backPath);
                            double[,] BackgroundMask = MathHelpsFileLoader.Load_Bitmap(backPath).ToDataIntensityDouble();


                            double X, Y;
                            using (StreamReader sr = new StreamReader(basePath + "\\data\\centers.csv"))
                            {
                                String line = sr.ReadLine();
                                parts = line.Split(',');
                                X = double.Parse(parts[1]);
                                Y = double.Parse(parts[2]);
                            }
                            Console.WriteLine(X + "," + Y);
                            float[, ,] DataCube = MathHelpsFileLoader.OpenDensityDataFloat(basePath + "\\data\\projectionobject.tif");

                            double FineCellHalf = DataCube.GetLength(0) / 2;
                            int FineCellSize = DataCube.GetLength(0);

                            // ImageHolder StackImage = null;
                            string stackPath = DirPath + "\\stack\\000";
                            ImageHolder StackImage = null;

                            string[] files = Directory.GetFiles(stackPath);
                            double[] fValues = new double[files.Length];
                            for (int i = 0; i < files.Length; i++)
                            {
                                if (Path.GetExtension(files[i]) == ".ivg")
                                    StackImage = MathHelpsFileLoader.LoadIVGFile(files[i]);
                                else
                                    StackImage = MathHelpsFileLoader.Load_Bitmap(files[i]);

                                StackImage = PythonHelps.ClipandFlattenImage(DataCube, X, Y, StackImage, BackgroundMask);

                                double[,] stackD = StackImage.ToDataIntensityDouble();
                                fValues[i] = MathHelpLib.ProjectionFilters.FocusValueTool.FocusValueF4(stackD);
                                Application.DoEvents();
                            }

                            int maxFI = 0;
                            double max = 0;
                            for (int i = 0; i < fValues.Length; i++)
                            {
                                if (fValues[i] > max)
                                {
                                    max = fValues[i];
                                    maxFI = i;
                                }
                            }
                            Console.WriteLine(max.ToString());

                            if (Path.GetExtension(files[maxFI]) == ".ivg")
                                StackImage = MathHelpsFileLoader.LoadIVGFile(files[maxFI]);
                            else
                                StackImage = MathHelpsFileLoader.Load_Bitmap(files[maxFI]);


                            ImageViewer.Filters.Effects.Flattening.FlattenEdges1DErrorCorrected.FlattenImageEdgesGlobal(StackImage);


                            ImageHolder BitmapImage = StackImage;

                            BitmapImage = PythonHelps.ClipandFlattenImage(DataCube, X, Y, BitmapImage, BackgroundMask);

                            string[] s = PythonHelps.EvaluateRecon(DataCube, BitmapImage, out stackF4, out onAxisF4);

                            Console.WriteLine(stackF4.ToString());

                            double FV = 0.0001;
                            try
                            {
                                string VGFolder = "y:\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;

                                if (Directory.Exists(VGFolder + "\\500pp\\recon_cropped_16bit") == true)
                                    VGFolder = VGFolder + "\\500pp\\recon_cropped_16bit";
                                else if (Directory.Exists(VGFolder + "\\500pp\\recon_16_bit") == true)
                                    VGFolder = VGFolder + "\\500pp\\recon_16_bit";
                                else if (Directory.Exists(VGFolder + "\\500pp\\recon_8_bit") == true)
                                    VGFolder = VGFolder + "\\500pp\\recon_8_bit";
                                else
                                    VGFolder = VGFolder + "\\500pp\\recon_cropped_8bit";


                               
                                if (Directory.Exists(VGFolder))
                                {
                                    string[] filesV = Directory.GetFiles(VGFolder);
                                    filesV = MathHelpLib.MathStringHelps.SortNumberedFiles(filesV);
                                    if (filesV.Length > 0)
                                    {
                                        ImageHolder ih = new ImageHolder(filesV[filesV.Length / 2 + 1]);
                                        Bitmap b = ih.ToBitmap();
                                        double[,] image = ih.ToDataIntensityDouble();

                                        FV = MathHelpLib.ProjectionFilters.FocusValueTool.FocusValueF4(image);
                                    }
                                }
                            }
                            catch { }

                            using (StreamWriter outfile = new StreamWriter(basePath + "\\data\\correctedEval.csv"))
                            {
                                outfile.WriteLine("Eval Coef," + s[0]);
                                outfile.WriteLine("Eval Factor," + s[1]);
                                outfile.WriteLine("Eval F4," + s[2]);
                                outfile.WriteLine("Eval Cross Coorelation," + s[3]);
                                outfile.WriteLine("Stack F4," + s[4]);
                                outfile.WriteLine("ReconVsStack," + s[5]);
                                outfile.WriteLine("VG_VsStack," + (FV / stackF4).ToString());
                                outfile.WriteLine("VG_VsASU," + (onAxisF4 / FV).ToString());


                                Console.WriteLine("Eval Coef," + s[0]);
                                Console.WriteLine("Eval Factor," + s[1]);
                                Console.WriteLine("Eval F4," + s[2]);
                                Console.WriteLine("Eval Cross Coorelation," + s[3]);
                                Console.WriteLine("Stack F4," + s[4]);
                                Console.WriteLine("ReconVsStack," + s[5]);
                                Console.WriteLine("VG_VsStack," + (FV / stackF4).ToString());
                                Console.WriteLine("VG_VsASU," + (onAxisF4 / FV).ToString());

                            }

                            string jSTring = Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + ",Eval Coef," + s[0] + ",Eval Factor," + s[1] + ",Eval F4," + s[2] + ",Eval Cross Coorelation," + s[3] + ",Stack F4," + s[4] + ",ReconVsStack," + s[5] + ",VG_VsStack," + (FV / stackF4).ToString() + ",VG_VsASU," + (onAxisF4 / FV).ToString();
                            Wholefile.WriteLine(jSTring);

                            Application.DoEvents();

                            try
                            {
                                if (File.Exists(basePath + "\\data\\stack.bmp"))
                                    File.Delete(basePath + "\\data\\stack.bmp");
                                BitmapImage.Save(basePath + "\\data\\stack.tif");
                                Console.WriteLine(basePath + "\\data\\stack.tif");
                            }
                            catch { }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.Print(ex.Message);
                        }
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
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);

                }
                Application.DoEvents();


            }

            Wholefile.Close();
            this.Text = "Cleaning missed stuff";
            // CleanBackgrounds();
            //  MessageBox.Show("All Recons are finished");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string DirPath = tWatchFolder.Text;
            List<string> Dirs = new List<string>(Directory.GetDirectories(DirPath));

            Dirs.Sort();
            for (int i = Dirs.Count - 1; i >= 0; i--)
            {
                List<string> subdirs = new List<string>(Directory.GetDirectories(Dirs[i]));

                foreach (string s in subdirs)
                {
                    try
                    {
                        if (s.Contains("201206") == false)
                        {
                            tWatchFolder.Text = s;
                            try
                            {
                                button2_Click(this, e);
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }
        }

    }
}
