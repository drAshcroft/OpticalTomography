using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using MathHelpLib;
using ImageViewer.Filters;
using System.Threading;
using ImageViewer.PythonScripting;
using Tomographic_Imaging_2.ImageManipulation;
using ImageViewer;
using System.Xml;
using System.Diagnostics;
using GraphingLib;

namespace Tomographic_Imaging_2
{
    public partial class ProcessGUI : DockContent
    {
        public ProcessGUI()
        {
            InitializeComponent();
        }

        #region Procedures


        #region FolderMakers
        private string GetDataInFolder()
        {
            //build the file structure
            string pPath;
            if (bDataFolder.Text.EndsWith("\\") == true)
                pPath = bDataFolder.Text;
            else
                pPath = bDataFolder.Text + "\\";

            string SelectedDay = (string)lDay.SelectedItem;
            string SelectedMonth = (string)lMonth.SelectedItem;
            string SelectedYear = (string)lYear.SelectedItem;
            string Machine = (string)LMachine.SelectedItem;

            pPath += Machine + "\\" + SelectedYear + SelectedMonth + "\\" + SelectedDay + "\\";
            return pPath;
        }
        private string GetExperimentFolder()
        {
            string pPath = GetDataInFolder() + lDataDirectories.SelectedRows[0].Cells[0].Value.ToString() + "\\";
            string DataPath = Path.GetDirectoryName(pPath) + "\\";
            return DataPath;
        }
        private string GetDataFolder()
        {
            //build the file structure
            return GetExperimentFolder() + "Data\\";
        }
        #endregion


        //multiple folders may be loaded in at the same time.  these need to be run 1 at a time to avoid overloading the 
        //system.  The queue provides a simple way to do this
        Queue<string> ReconDirectoryBacklog = new Queue<string>();
        private bool ReconRunning = false;
        private bool ProcessRunning = false;
        private bool WholeReconRunning = false;
        #endregion

        #region DataIn
        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            if (bDataFolder.Text.Trim() != "")
            {
                bDataFolder_TextChanged(this, EventArgs.Empty);
            }

        }

        private void fileSystemWatcher1_Created(object sender, FileSystemEventArgs e)
        {
            fileSystemWatcher1_Changed(sender, e);
        }

        private void fileSystemWatcher1_Deleted(object sender, FileSystemEventArgs e)
        {
            fileSystemWatcher1_Changed(sender, e);
        }

        private void bTopLevelBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult ret = folderBrowserDialog1.ShowDialog();
                if (ret == DialogResult.OK)
                {
                    bDataFolder.Text = folderBrowserDialog1.SelectedPath;
                    bDataFolder_TextChanged(this, EventArgs.Empty);
                }
            }
            catch { }
        }

       


        #endregion

        private void EasyGUI_Load(object sender, EventArgs e)
        {
            //set up the scripting interface
            ScriptingInterface sScriptingInterface = new ScriptingInterface();
            sScriptingInterface.MainForm = this;

            string ScriptDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\Scripts\\";
            string ReconScriptDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\ReconScripts\\";

            bDataFolder_TextChanged(this, EventArgs.Empty);

            FillRows();
        }


        string CurrentParentFolder = "";
        string[] AvailableFolders = null;
        bool[] Processed = null;

        private void FillRows()
        {

            Thread FillMonitor = new Thread(delegate(object Vars)
                {
                    bool DoSleep = true;
                    while (this.IsDisposed == false)
                    {
                        DoSleep = true;
                        try
                        {
                            if (Processed != null)
                            {
                                for (int i = 0; i < Processed.Length; i++)
                                {
                                    try
                                    {
                                        if (Processed[i] == false)
                                        {

                                            DoRow(i, CurrentParentFolder + AvailableFolders[i] + "\\");
                                            DoSleep = false;
                                        }
                                    }
                                    catch { }
                                    Processed[i] = true;
                                    if (DoSleep)
                                        Thread.Sleep(100);

                                }
                            }
                        }
                        catch { }
                        Thread.Sleep(1000);
                    }
                }
            );

            FillMonitor.Start();

        }

        #region Processing
        private void filewatchPreprocess_Created(object sender, FileSystemEventArgs e)
        {
            string ScriptDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\Scripts\\";

            string[] Scripts = Directory.GetFiles(ScriptDirectory);

            for (int i = 0; i < Scripts.Length; i++)
                Scripts[i] = Path.GetFileName(Scripts[i]);


        }

        private void filewatchPreprocess_Deleted(object sender, FileSystemEventArgs e)
        {
            filewatchPreprocess_Created(sender, e);
        }




        #endregion

        #region Summary

        private void bFocusValue_Click(object sender, EventArgs e)
        {
            try
            {
                ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool FileReader = new ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool();
                ReplaceStringDictionary passdata = new ReplaceStringDictionary();
                FileReader.DoEffect(null, null, passdata, "FocusValue", GetDataFolder() + "FocusValue");
                double[] FocusValue = (double[])passdata["Array"];

                ScriptingInterface.scriptingInterface.CreateGraph("Focus Value", FocusValue, "Frame Number", "Focus Value");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Beep();
            }
        }


        private void bIntensity_Click(object sender, EventArgs e)
        {
            try
            {
                ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool FileReader = new ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool();
                ReplaceStringDictionary passdata = new ReplaceStringDictionary();
                FileReader.DoEffect(null, null, passdata, "BGIntensityAverage", GetDataFolder() + "BGIntensityAverage");
                double[] BGAverage = (double[])passdata["Array"];

                double ave = BGAverage.Average();
                FileReader.DoEffect(null, null, passdata, "BGIntensityAverage", GetDataFolder() + "BGIntensitySD");
                double[] BGSD = (double[])passdata["Array"];
                BGSD.DivideInPlace(ave);
                BGSD.MultiplyInPlace(100);
                ScriptingInterface.scriptingInterface.CreateGraph("Background Average Values", BGAverage, "Frame Number", "Intensity");
                ScriptingInterface.scriptingInterface.CreateGraph("Background Variations", BGSD, "Frame Number", "% Deviation from Average Image");

                FileReader.DoEffect(null, null, passdata, "FGIntensityDivide", GetDataFolder() + "FGIntensityDivide");
                double[] FGAverageDivide = (double[])passdata["Array"];

                FileReader.DoEffect(null, null, passdata, "FGIntensityNoDivide", GetDataFolder() + "FGIntensityNoDivide");
                double[] FGAverageNoDivide = (double[])passdata["Array"];

                List<double[]> FGValues = new List<double[]>();
                FGValues.Add(FGAverageDivide);
                FGValues.Add(FGAverageNoDivide);

                ScriptingInterface.scriptingInterface.CreateGraph("Integrated PP Values", FGValues, "Frame Number", "Intensity");


                FGAverageDivide.DivideInPlaceErrorless(BGAverage);
                FGAverageDivide.MultiplyInPlace(ave);

                FGAverageNoDivide.DivideInPlaceErrorless(BGAverage);
                FGAverageNoDivide.MultiplyInPlace(ave);

                ScriptingInterface.scriptingInterface.CreateGraph("Integrated PP Values (Normalized)", FGValues, "Frame Number", "Intensity");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Beep();
            }
        }


        private void bBackground_Click(object sender, EventArgs e)
        {
            //build the file structure
            try
            {

                string DataPath = GetDataFolder();
                ScriptingInterface.scriptingInterface.CreateGraph("Background", MathHelpsFileLoader.Load_Bitmap(DataPath + "Background.bmp").ToBitmap());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Beep();
            }

        }

        private void bCenter_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptingInterface.scriptingInterface.CreateGraph("Centering", GetDataFolder() + "Centering.avi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Beep();
            }
        }

        private void pPP0_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptingInterface.scriptingInterface.CreateGraph("First Projection", (Bitmap)pPP0.Image);
            }
            catch { }
        }

        private void pPPLast_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptingInterface.scriptingInterface.CreateGraph("Half Projection", (Bitmap)pPPLast.Image);
            }
            catch { }
        }

        private void bShowCenteringTool_Click(object sender, EventArgs e)
        {
            ImageHolder BitmapImage = new ImageHolder(1, 1);
            ReplaceStringDictionary PassData = new ReplaceStringDictionary();
            DataEnvironment dataEnvironment = new DataEnvironment();

            //Read Whole Array
            ImageViewer.Filters.IEffect Filter = new ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_Positions", GetDataFolder() + "X_Positions");
            //ImageData out of type :System.Double[]
            double[] XData = (double[])Filter.PassData["Array"];

            //Read Whole Array
            Filter = new ImageViewer.PythonScripting.Arrays.ReadTestArrayFromFileTool();

            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_Positions", GetDataFolder() + "Y_Positions");
            //ImageData out of type :System.Double[]
            double[] YData = (double[])Filter.PassData["Array"];

            //build the file structure
            string pPath;
            if (bDataFolder.Text.EndsWith("\\") == true)
                pPath = bDataFolder.Text;
            else
                pPath = bDataFolder.Text + "\\";

            pPath += lDataDirectories.SelectedRows[0].Cells[0].Value.ToString() + "\\";
            string tppPath = Path.GetDirectoryName(pPath) + "\\PP\\";
            //temp0PPs000.bmp
            // string filter = "temp0PPs*.*";
            string[] PPs = Directory.GetFiles(tppPath);
            string[] Sorted = MathStringHelps.SortNumberedFiles(PPs);
            List<string> ImagesIn = new List<string>();
            ImagesIn.AddRange(Sorted);

            //Center Cells
            Filter = new ImageViewer.Filters.CenterCellsTool2Form();

            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ImagesIn, 100, "X_Positions", "Y_Positions", "MovingAverage", 5, "MovingAverage", 5, "Show", new Size(170, 170), false, 0);
        }

        private void bDataView_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(GetDataFolder() + "ProjectionObject.cct"))
                {
                    ProjectionObject po = new ProjectionObject();
                    po.OpenDensityData(GetDataFolder() + "ProjectionObject.cct");

                    GraphForm3D gf3d = new GraphForm3D();
                    gf3d.Show(this);
                    gf3d.SetData(po.ProjectionData);
                    po = null;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Beep();
            }
        }

        private void bDataviewVG_Click(object sender, EventArgs e)
        {
            //try
            {
                string ProcessedDrive = tVGDrive.Text;
                string DataIn = GetExperimentFolder();// GetDataInFolder();
                Console.WriteLine(DataIn);
                string dirName = Path.GetFileNameWithoutExtension(DataIn);
                if (dirName == "")
                    dirName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(DataIn));
                Console.WriteLine(dirName);
                string[] parts = dirName.Split('_');
                string Prefix = parts[0];
                string Year = parts[1].Substring(0, 4);
                string month = parts[1].Substring(4, 2);
                string day = parts[1].Substring(6, 2);

                string basePath = ProcessedDrive + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_cropped_8bit\\";
                string[] Slices = Directory.GetFiles(basePath);
                List<string> GoodFiles = new List<string>();
                foreach (string s in Slices)
                    if (Path.GetExtension(s).ToLower().Trim() != ".vvi")
                        GoodFiles.Add(s);

                Slices = EffectHelps.SortNumberedFiles(GoodFiles.ToArray());

                GraphForm3D gf3d = new GraphForm3D();
                gf3d.Show(this);
                gf3d.SetData(PhysicalArray.OpenDensityData(Slices  ));
            }
            /*catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Beep();
            }*/
        }

        
        #endregion

        private void bLoadRaw_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                ProjectionObject po = new ProjectionObject();
                if (openFileDialog1.FileNames.Length == 1)
                {
                    po.OpenDensityData(openFileDialog1.FileName);
                }
                else
                {
                    po.OpenDensityData(openFileDialog1.FileNames);
                }
                ScriptingInterface.scriptingInterface.MakeVariableVisible(Path.GetFileNameWithoutExtension(openFileDialog1.FileName), po);
                ScriptingInterface.scriptingInterface.CreateGraph(Path.GetFileNameWithoutExtension(openFileDialog1.FileName), po.ProjectionData);
            }
            openFileDialog1.Multiselect = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pPath;
            List<string> Selected = new List<string>();
            for (int i = 0; i < lDataDirectories.Rows.Count; i++)
            {
                if (lDataDirectories.Rows[i].Selected == true)
                {
                    pPath = GetDataInFolder() + (string)lDataDirectories.Rows[i].Cells[0].ToString() + "\\";
                    Selected.Add(pPath);
                }
            }

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            TimeSpan maxWait = new TimeSpan(0, 10, 0);
            foreach (string p in Selected)
            {
                Process ScriptRunner = new Process();

                ScriptRunner.StartInfo.FileName = @"C:\Development\CellCT\Tomographic_Imaging_MDL\ScriptRunner\bin\Debug\ScriptRunner.exe";
                ScriptRunner.StartInfo.Arguments = "\"" + p + "\"";

                sw.Restart();
                ScriptRunner.Start();

                while (sw.Elapsed < maxWait && ScriptRunner.HasExited == false)
                {
                    Application.DoEvents();
                }

                try
                {
                    if (ScriptRunner.HasExited == false)
                        ScriptRunner.Kill();
                }
                catch { }
            }
        }

        private class ProcessHolder
        {
            public System.Diagnostics.Stopwatch sw = new Stopwatch();
            public Process ScriptRunner;
            public TimeSpan MaxTime = new TimeSpan(0, 20, 0);
            public ProcessHolder(Process scriptRunner)
            {
                ScriptRunner = scriptRunner;
                sw.Start();
            }
            public bool CheckTime()
            {
                try
                {
                    if (sw.Elapsed > MaxTime)
                    {
                        ScriptRunner.Kill();
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        private List<ProcessHolder> ProcessHolderList = new List<ProcessHolder>();
        private void button2_Click(object sender, EventArgs e)
        {
            string STorage = @"G:\storage\";

            string[] AllDirs = Directory.GetDirectories(STorage, "cc*.*", SearchOption.AllDirectories);

            string pPath;
            Queue<string> Selected = new Queue<string>();

            for (int i = 0; i < AllDirs.Length; i++)
            {
                try
                {
                    /*  pPath = AllDirs[i].Replace("\"", "").Replace("'", "");

                      if (pPath.EndsWith("\\") == false)
                          pPath += "\\";

                      string dirName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(pPath));
                      string[] parts = dirName.Split('_');
                      string Prefix = parts[0];
                      string Year = parts[1].Substring(0, 4);
                      string month = parts[1].Substring(4, 2);
                      string day = parts[1].Substring(6, 2);

                      string basePath = "c:\\Processed\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\";


                      //"G:\\storage\\cct002\\201012\\07\\cct002_20101207_150552"
                      if (Directory.Exists(basePath) == false)
                      {*/
                    pPath = Path.GetFileNameWithoutExtension(AllDirs[i]);
                    if (pPath.Length > 10 && pPath.Substring(0, 2) == "cc")
                    {
                        Selected.Enqueue(AllDirs[i]);
                    }
                    /*}
                    else
                    {
                        pPath = "";
                    }*/
                }
                catch { }

            }

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            TimeSpan maxWait = new TimeSpan(0, 1, 45);
            while (Selected.Count > 0)// (string p in Selected)
            {
                Process ScriptRunner = new Process();

                ScriptRunner.StartInfo.FileName = @"C:\Development\CellCT\runtime\ScriptRunner.exe";
                ScriptRunner.StartInfo.Arguments = "\"" + Selected.Dequeue() + "\"";
                ScriptRunner.Start();

                ProcessHolderList.Add(new ProcessHolder(ScriptRunner));

                sw.Restart();
                while (sw.Elapsed < maxWait && ScriptRunner.HasExited == false)
                    Application.DoEvents();
            }
        }

        private void timerProcess_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < ProcessHolderList.Count; i++)
            {
                if (ProcessHolderList[i].CheckTime() == false)
                    ProcessHolderList.RemoveAt(i);

            }
        }

        private void bDataFolder_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(bDataFolder.Text) == true)
            {
                try
                {
                    string[] Directories = Directory.GetDirectories(bDataFolder.Text);
                    LMachine.Items.Clear();
                    for (int i = 0; i < Directories.Length; i++)
                    {
                        string machine = Path.GetFileName(Directories[i]);
                        if (machine.Contains("cct"))
                            LMachine.Items.Add(machine );
                    }
                    LMachine.SetSelected(0, true);
                }
                catch { }
            }
        }

        private void LMachine_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string Machine = (string)LMachine.SelectedItem;
                string[] Directories = Directory.GetDirectories(bDataFolder.Text);

                lYear.Items.Clear();
                for (int i = 0; i < Directories.Length; i++)
                {
                    if (Machine == Path.GetFileName(Directories[i]))
                    {
                        string[] Years = Directory.GetDirectories(Directories[i]);
                        for (int j = 0; j < Years.Length; j++)
                        {
                            string YearAndMonth = Path.GetFileName(Years[j]);
                            string Year = YearAndMonth.Substring(0, 4);
                            bool YearAllreadyThere = false;
                            for (int k = 0; k < lYear.Items.Count; k++)
                            {
                                if ((string)lYear.Items[k] == Year)
                                    YearAllreadyThere = true;

                            }
                            if (YearAllreadyThere == false)
                                lYear.Items.Add(Year);
                        }
                    }
                }
                lYear.SetSelected(0, true);
            }
            catch { Console.Beep(); }
        }

        private void lYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string SelectedYear = (string)lYear.SelectedItem;
                string Machine = (string)LMachine.SelectedItem;
                string[] Directories = Directory.GetDirectories(bDataFolder.Text);

                lMonth.Items.Clear();
                for (int i = 0; i < Directories.Length; i++)
                {
                    if (Machine == Path.GetFileName(Directories[i]))
                    {
                        string[] Years = Directory.GetDirectories(Directories[i]);
                        for (int j = 0; j < Years.Length; j++)
                        {
                            string YearAndMonth = Path.GetFileName(Years[j]);
                            string Year = YearAndMonth.Substring(0, 4);
                            if (Year == SelectedYear)
                            {
                                string month = YearAndMonth.Substring(4, 2);
                                lMonth.Items.Add(month);
                            }
                        }
                    }
                }
                lMonth.SetSelected(0, true);
            }
            catch { Console.Beep(); }
        }

        private void lMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string SelectedMonth = (string)lMonth.SelectedItem;
                string SelectedYear = (string)lYear.SelectedItem;
                string Machine = (string)LMachine.SelectedItem;

                lDay.Items.Clear();
                string[] Directories = Directory.GetDirectories(bDataFolder.Text + "\\" + Machine + "\\" + SelectedYear + SelectedMonth + "\\");
                for (int i = 0; i < Directories.Length; i++)
                {
                    string dirName = Path.GetFileNameWithoutExtension(Directories[i]);
                    lDay.Items.Add(dirName);
                }
                lDay.SetSelected(0, true);
            }
            catch { Console.Beep(); }
        }

        private void lDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string SelectedDay = (string)lDay.SelectedItem;
                string SelectedMonth = (string)lMonth.SelectedItem;
                string SelectedYear = (string)lYear.SelectedItem;
                string Machine = (string)LMachine.SelectedItem;

                lDataDirectories.Rows.Clear();
                //lDataDirectories.Items.Clear();
                string[] Directories = Directory.GetDirectories(bDataFolder.Text + "\\" + Machine + "\\" + SelectedYear + SelectedMonth + "\\" + SelectedDay + "\\");

                Processed = null;
                AvailableFolders = new string[Directories.Length];
                for (int i = 0; i < Directories.Length; i++)
                {
                    string dirName = Path.GetFileNameWithoutExtension(Directories[i]);
                    lDataDirectories.Rows.Add(dirName, "", "", "");
                    AvailableFolders[i] = dirName;
                }
                lDataDirectories.Rows[0].Selected = true;
                lDataDirectories.CurrentCell = lDataDirectories.Rows[0].Cells[0];
                lDataDirInfos = 0;

                CurrentParentFolder = GetDataInFolder();
                Processed = new bool[AvailableFolders.Length];
            }
            catch { Console.Beep(); }
        }

        private void bMIP_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptingInterface.scriptingInterface.CreateGraph("MIP", GetDataFolder() + "mip.avi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Beep();
            }
        }

        static string LastSelectionFolder = "";
        private void lDataDirectories_SelectionChanged(object sender, EventArgs e)
        {

            try
            {
                if (LastSelectionFolder != "")
                {
                    String line;
                    string userImageQuality = (string)lUserReconQuality.SelectedItem;
                    for (int i = 0; i < lUserReconQuality.Items.Count; i++)
                    {
                        if (lUserReconQuality.GetSelected(i) == true)
                        {
                            userImageQuality = (string)lUserReconQuality.Items[i];
                            break;
                        }
                    }

                    string userCellQuality = (string)lUserCellQuality.SelectedItem;
                    for (int i = 0; i < lUserCellQuality.Items.Count; i++)
                    {
                        if (lUserCellQuality.GetSelected(i) == true)
                        {
                            userCellQuality = (string)lUserCellQuality.Items[i];
                            break;
                        }
                    }

                    string Stain = cbStainIntensity.Checked.ToString();
                    string Motion = cbMotionREgistration.Checked.ToString();
                    string Noise = cbBackgroundNoise.Checked.ToString();
                    string Focus = cbFocus.Checked.ToString();

                    line = "UserImageQuality==" + userImageQuality + "\r\n";
                    line += "UserCellQuality==" + userCellQuality + "\r\n";
                    line += "Stain==" + Stain + "\r\n";
                    line += "Motion==" + Motion + "\r\n";
                    line += "Noise==" + Noise + "\r\n";
                    line += "Focus==" + Focus + "\r\n";
                    line += "User== User_" + tUserName.Text + "\r\n";
                    line += "Error==" + cbError.Checked.ToString() + "\r\n";  
                    line+="InterestingCell==" + cbInterestingCell.Checked.ToString() + "\r\n";
                    line += "InterferingObject==" +cbInterferingObject.Checked.ToString() + "\r\n"; 
                    line +="MitoticCell==" +cbMitoticCell .Checked.ToString() + "\r\n";
                    // Write the string to a file.
                    System.IO.StreamWriter file = new System.IO.StreamWriter(LastSelectionFolder);
                    file.WriteLine(line);

                    file.Close();
                }

            }
            catch
            {


            }

            string DataPath;
            try
            {
                DataPath = GetDataFolder();
                string PPPath = GetExperimentFolder() + "PP\\";



                //prevent random errors by cloning the image data
                try
                {
                    pPP0.Image = new Bitmap(new Bitmap(DataPath + "FirstPP.bmp"));
                }
                catch
                {
                    pPP0.Image = new Bitmap(1, 1);
                }

                try
                {
                    ppQuarter.Image = new Bitmap(new Bitmap(DataPath + "quarterPP.bmp"));
                }
                catch
                {
                    ppQuarter.Image = new Bitmap(1, 1);
                }

                try
                {
                    ppHalf.Image = new Bitmap(new Bitmap(DataPath + "halfPP.bmp"));
                }
                catch
                {
                    ppHalf.Image = new Bitmap(1, 1);
                }

                try
                {
                    pp3Quarters.Image = new Bitmap(new Bitmap(DataPath + "LastQuarterPP.bmp"));
                }
                catch
                {
                    pp3Quarters.Image = new Bitmap(1, 1);
                }

                try
                {
                    pPPLast.Image = new Bitmap(new Bitmap(DataPath + "lastPP.bmp"));
                }
                catch
                {
                    pPPLast.Image = new Bitmap(1, 1);
                }




                try
                {
                    pReconMine.Image = new Bitmap(new Bitmap(DataPath + "CrossSections_Z.jpg"));
                }
                catch
                {
                    pReconMine.Image = new Bitmap(1, 1);
                }

                try
                {

                    pReconVG.Image = new Bitmap(new Bitmap(DataPath + "VGExample.png"));
                 
                }
                catch
                {
                    pReconVG.Image = new Bitmap(1, 1);
                }

                try
                {
                    pReconX.Image = new Bitmap(new Bitmap(DataPath + "CrossSections_X.jpg"));

                }
                catch
                {
                    pReconX.Image = new Bitmap(1, 1);
                }

                try
                {
                    Bitmap b= new Bitmap(DataPath + "StackExample.bmp");
                    b.RotateFlip(RotateFlipType.Rotate270FlipNone );
                    pStack.Image = new Bitmap(b );

                }
                catch
                {
                    pStack.Image = new Bitmap(1, 1);
                }

                try
                {
                    pMIP.Image = new Bitmap(new Bitmap(DataPath + "Forward1.bmp"));
                }
                catch
                {
                    pMIP.Image = new Bitmap(1, 1);
                }



                try
                {
                    lTextSummary.Text = System.IO.File.ReadAllText(DataPath + "Comments.txt");
                }
                catch
                {
                    lTextSummary.Text = "";
                }
                //force a redraw
                pPP0.Invalidate();
                pPPLast.Invalidate();
                pReconMine.Invalidate();


                bBackground.Enabled = File.Exists(DataPath + "Background.bmp");
                // bShowCenteringTool.Enabled = File.Exists(DataPath + "x_Positions");
                bCenter.Enabled = File.Exists(DataPath + "Centering.avi");
                bDataView.Enabled = File.Exists(DataPath + "projectionobject.cct");
                bFlyThrough.Enabled = File.Exists(DataPath + "flyThrough.avi");
                bMIP.Enabled = File.Exists(DataPath + "Mip.avi");
                bIntensity.Enabled = File.Exists(DataPath + "BGIntensityAverage");
                bFocusValue.Enabled = File.Exists(DataPath + "FocusValue");

                try
                {
                    String line;
                    using (StreamReader sr = new StreamReader(DataPath + "UserComments.txt"))
                    {
                        line = sr.ReadToEnd();
                    }

                    line = line.ToLower();
                    string[] Parts = line.Split(new string[] { "\r\n", "\r", "\n", "==" }, StringSplitOptions.RemoveEmptyEntries);

                    ReplaceStringDictionary Values = new ReplaceStringDictionary();

                    for (int i = 0; i < Parts.Length; i += 2)
                    {
                        Values.AddSafe(Parts[i], Parts[i + 1]);
                    }

                    string userImageQuality = "Choose";
                    try
                    {

                        userImageQuality = ((string)Values["userimagequality"]).ToLower();
                        for (int i = 0; i < lUserReconQuality.Items.Count; i++)
                        {
                            if (((string)lUserReconQuality.Items[i]).ToLower() == userImageQuality)
                            {
                                lUserReconQuality.SetSelected(i, true);
                                break;
                            }
                        }
                    }
                    catch
                    {
                        userImageQuality = "Choose";
                        lUserReconQuality.SetSelected(0, true);
                    }

                    string userCellQuality = "Choose";
                    try
                    {

                        userCellQuality = ((string)Values["usercellquality"]).ToLower();
                        for (int i = 0; i < lUserCellQuality.Items.Count; i++)
                        {
                            if (((string)lUserCellQuality.Items[i]).ToLower() == userCellQuality)
                            {
                                lUserCellQuality.SetSelected(i, true);
                                break;
                            }
                        }
                    }
                    catch
                    {
                        userCellQuality = "Choose";
                        lUserCellQuality.SetSelected(0, true);
                    }


                    string Stain = "false";
                    string Motion = "false";
                    string Noise = "false";
                    string Focus = "false";
                    string Error ="false";
                    string InterestingCell ="false";
                    string InterferingObject = "false";
                    string MitoticCell = "false";

                    try { Stain = (string)Values["stain"]; }
                    catch { }

                    try { Motion = (string)Values["motion"]; }
                    catch { }

                    try { Noise = (string)Values["noise"]; }
                    catch { }

                    try { Focus = (string)Values["focus"]; }
                    catch { }

                    try { Error = (string)Values["error"]; }
                    catch { }

                    try { InterestingCell  = (string)Values["interestingcell"]; }
                    catch { }

                    try { InterferingObject = (string)Values["interferingobject"]; }
                    catch { }

                    try { MitoticCell = (string)Values["mitoticcell"]; }
                    catch { }

                    cbBackgroundNoise.Checked = (Noise != "false");
                    cbFocus.Checked = (Focus != "false");
                    cbMotionREgistration.Checked = (Motion != "false");
                    cbStainIntensity.Checked = (Stain != "false");
                    cbError.Checked = (Error != "false");
                    cbInterestingCell.Checked=(InterestingCell !="false" );
                    cbInterferingObject.Checked = (InterferingObject != "false");
                    cbMitoticCell.Checked = (MitoticCell != "false");

                }
                catch
                {
                    lUserReconQuality.SetSelected(0, true);
                    lUserCellQuality.SetSelected(0, true);
                    cbBackgroundNoise.Checked = false;
                    cbFocus.Checked = false;
                    cbMotionREgistration.Checked = false;
                    cbStainIntensity.Checked = false;
                }

                LastSelectionFolder = DataPath + "UserComments.txt";
            }
            catch
            {
                LastSelectionFolder = "";
            }

        }

        private delegate void DoRowEvent(int Row, ReplaceStringDictionary Values);

        private void DoRow(int Row, ReplaceStringDictionary Values)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new DoRowEvent(DoRow), Row, Values);
            }
            else
            {
                string NumberOfCells = (string)Values["numberofcells"];
                string CenteringQuality = (string)Values["centeringquality"];
                string ReconSucceeded = (string)Values["recon"];
                string FocusValue = "0";
                string FocusVar = "0";
                string[] FocusParts = ((string)Values["focusvalue"]).Split(',');
                FocusValue = (string)FocusParts[0];
                FocusVar = (string)FocusParts[1];
                string ImageQuality = (String)Values["imagequality"];
                string ImageType = (string)Values["imagetype"];


                lDataDirectories.Rows[Row].Cells[1].Value = ReconSucceeded;
                lDataDirectories.Rows[Row].Cells[2].Value = (string)Values["backgroundsubtraction"];
                lDataDirectories.Rows[Row].Cells[3].Value = CenteringQuality;
                lDataDirectories.Rows[Row].Cells[4].Value = FocusValue;
                lDataDirectories.Rows[Row].Cells[5].Value = FocusVar;
                lDataDirectories.Rows[Row].Cells[6].Value = NumberOfCells;
                lDataDirectories.Rows[Row].Cells[7].Value = ImageType;
                lDataDirectories.Rows[Row].Cells[8].Value = ImageQuality;
            }
        }

        private void DoRow(int Row, string FilePath)
        {
            string DataPath = FilePath + "data\\";
            if (File.Exists(DataPath + "comments.txt") == false)
            {
                lDataDirectories.Rows[Row].Cells[1].Value = "No Data";
                return;
            }

            ReplaceStringDictionary Values = DoRow(FilePath);

            DoRow(Row, Values);
        }

        private ReplaceStringDictionary DoRow(string FilePath)
        {
            ReplaceStringDictionary Values = new ReplaceStringDictionary();
            string DataPath = FilePath + "data\\";
            if (File.Exists(DataPath + "comments.txt") == false)
            {
                Values.AddSafe("recon", "No Data");
                return Values;
            }

            String line;
            using (StreamReader sr = new StreamReader(DataPath + "comments.txt"))
            {
                line = sr.ReadToEnd();
            }

            line = line.Replace("\r", "").Replace("\n", "").ToLower();
            string[] Parts = line.Split(new string[] { "<", "/>", ">" }, StringSplitOptions.RemoveEmptyEntries);



            for (int i = 0; i < Parts.Length; i += 2)
            {
                Values.AddSafe(Parts[i], Parts[i + 1]);
            }
            string NumberOfCells = "0";
            try
            {
                NumberOfCells = (string)Values["numberofcells"];
            }
            catch { }
            Values.AddSafe("numberofcells", NumberOfCells);

            string CenteringQuality = "0";
            try
            {
                CenteringQuality = (string)Values["centeringquality"];
            }
            catch { }
            Values.AddSafe("centeringquality", CenteringQuality);

            string ReconSucceeded = "False";
            try
            {
                ReconSucceeded = (string)Values["recon"];
            }
            catch { }

            if (ReconSucceeded == "False")
            {
                if (File.Exists(DataPath + "ProjectionObject.cct") == true)
                {
                    ReconSucceeded = "False but succeeded";
                }
                else
                    ReconSucceeded = "False";
            }
            Values.AddSafe("recon", ReconSucceeded);


            string FocusValue = "0";
            string FocusVar = "0";
            try
            {
                string[] FocusParts = ((string)Values["focusvalue"]).Split(',');
                FocusValue = (string)FocusParts[0];
                FocusVar = (string)FocusParts[1];
            }
            catch { }
            Values.AddSafe("focusvalue", FocusValue + "," + FocusVar);

            string ImageQuality = "0";
            try
            {
                ImageQuality = (String)Values["imagequality"];
            }
            catch
            { }
            Values.AddSafe("imagequality", ImageQuality);


            string ImageType = "RGB";
            try
            {
                if ((string)Values["imagetype"] == ".png")
                    ImageType = "RGB";
                else
                    ImageType = "Monochrome";
            }
            catch { }
            Values["imagetype"] = ImageType;

            string BackgroundSubtraction = "No Info";
            try
            {
                BackgroundSubtraction = (String)Values["backgroundsubtraction"];
            }
            catch { }
            Values.AddSafe("backgroundsubtraction", BackgroundSubtraction);

            return Values;

        }

        int lDataDirInfos = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (lDataDirInfos < lDataDirectories.Rows.Count)
                {

                    string pPath = GetDataInFolder() + lDataDirectories.Rows[lDataDirInfos].Cells[0].Value + "\\";
                    string DataPath = Path.GetDirectoryName(pPath) + "\\";

                    DoRow(lDataDirInfos, pPath);
                    lDataDirInfos++;
                }
            }
            catch { }
            Application.DoEvents();
        }

        private void bShowStack_Click(object sender, EventArgs e)
        {
            try
            {
                StackExplorer se = new StackExplorer();
                se.OpenExperimentFolder(GetExperimentFolder());
                se.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Beep();
            }
        }

       
        private void bFlyThrough_Click(object sender, EventArgs e)
        {
            Console.Beep();
        }

        private void lDataDirectories_Leave(object sender, EventArgs e)
        {
            lDataDirectories_SelectionChanged(this, EventArgs.Empty);
        }

        private void lDataDirectories_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            lDataDirectories_SelectionChanged(this, EventArgs.Empty);
        }

        private void ProcessGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            lDataDirectories_SelectionChanged(this, EventArgs.Empty);
        }

       


    }
}
