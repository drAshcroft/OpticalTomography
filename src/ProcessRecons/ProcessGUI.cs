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
using ImageViewer;
using System.Xml;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ProcessRecons;
using GraphingLib;
using VoreenWindow;

namespace Tomographic_Imaging_2
{
    public partial class ProcessGUI : Form
    {
        Dictionary<string, string> Columns = new Dictionary<string, string>();

        public ProcessGUI()
        {
            InitializeComponent();
            ThisHandle = this.Handle;
            NetworkHandler = new CommonNetwork();
            NetworkHandler.StartNetworkWriter("Process", 1423);


            for (int i = 0; i < Cols.Length; i++)
            {
                Columns.Add(colNames[i], Cols[i]);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr child, IntPtr newParent);

        public static IntPtr ThisHandle;

        static CommonNetwork NetworkHandler;


        #region Procedures


        #region FolderMakers

        private string GetVGStack()
        {
            return GetVGFolder() + "stack\\000\\";
        }

        //V:\Raw PP\cct001\Absorption\200912\03\cct001_20091203_145527\STACK\000
        private string GetVGFolder()
        {

            string pPath;
            if (bDataFolder.Text.EndsWith("\\") == true)
                pPath = bDataFolder.Text;
            else
                pPath = bDataFolder.Text + "\\";

            pPath = pPath.Split(Path.VolumeSeparatorChar)[0];

            //build the file structure
            pPath = pPath + @":\raw pp\";


            string SelectedDay = (string)lDay.SelectedItem;
            string SelectedMonth = (string)lMonth.SelectedItem;
            string SelectedYear = (string)lYear.SelectedItem;
            string Machine = (string)LMachine.SelectedItem;

            pPath += Machine + "\\absorption\\" + SelectedYear + SelectedMonth + "\\" + SelectedDay + "\\" + lDataDirectories.Rows[lDataDirectories.SelectedCells[0].RowIndex].Cells[0].Value.ToString() + "\\";
            return pPath;
        }

        private Bitmap GetVGExample()
        {

            string pPath;
            if (bDataFolder.Text.EndsWith("\\") == true)
                pPath = bDataFolder.Text;
            else
                pPath = bDataFolder.Text + "\\";

            pPath = pPath.Split(Path.VolumeSeparatorChar)[0];

            //build the file structure
            pPath = @"y:\";

            var row = lDataDirectories.SelectedCells[0].RowIndex;
            string foldername = lDataDirectories.Rows[row].Cells[0].Value.ToString();

            var parts = foldername.Split('_');
            var Prefix = parts[0];
            var Year = parts[1].Substring(0, 4);
            var month = parts[1].Substring(4, 2);
            var day = parts[1].Substring(6, 2);

            //string SelectedDay = (string)lDay.SelectedItem;
            //string SelectedMonth = (string)lMonth.SelectedItem;
            //string SelectedYear = (string)lYear.SelectedItem;
            //string Machine = (string)LMachine.SelectedItem;

            pPath += Prefix + "\\" + Year + month + "\\" + day + "\\" + foldername + "\\";// lDataDirectories.Rows[lDataDirectories.SelectedCells[0].RowIndex].Cells[0].Value.ToString() + "\\";


            if (Directory.Exists(pPath + @"500PP\recon_cropped_8bit"))
            {
                string[] files = Directory.GetFiles(pPath + @"500PP\recon_cropped_8bit");

                string filename = pPath + @"500PP\recon_cropped_8bit\reconCrop8bit_" + string.Format("{0:000}.png", files.Length / 2);

                return new Bitmap(filename);
            }

            return null;
        }

        private string GetDataInFolder()
        {
            if (tabControl1.SelectedIndex == 0)
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
            else
            {
                string pPath;
                if (tDataInputFolder.Text.EndsWith("\\") == true)
                    pPath = tDataInputFolder.Text;
                else
                    pPath = tDataInputFolder.Text + "\\";
                return pPath;
            }
        }
        private string GetExperimentFolder()
        {
            var row = lDataDirectories.SelectedCells[0].RowIndex;
            string foldername = lDataDirectories.Rows[row].Cells[0].Value.ToString();
            string pPath = GetDataInFolder() + foldername + "\\";
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
            // timerStartVoreene.Enabled = true;

            //set up the scripting interface

            ScriptingInterface sScriptingInterface = new ScriptingInterface();
            sScriptingInterface.MainForm = this;

            string ScriptDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\Scripts\\";
            string ReconScriptDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\ReconScripts\\";


            DriveInfo[] Drives = DriveInfo.GetDrives();

            for (int i = 0; i < Drives.Length; i++)
            {
                try
                {
                    if (Drives[i].VolumeLabel.ToLower().Contains("labeling") == true)
                    {
                        tbLabeledDrive.Text = Drives[i].Name;
                    }
                    else if (Drives[i].VolumeLabel.ToLower().Contains("processed") == true)
                    {
                        tVGDrive.Text = Drives[i].Name;
                    }
                    else if (Drives[i].VolumeLabel.ToLower().Contains("bda_cellct") == true)
                    {
                        bDataFolder.Text = Drives[i].Name + @"ASU_Recon";
                    }
                }
                catch { }
            }
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
                ScriptingInterface.scriptingInterface.CreateGraph("Background - " + lDataDirectories.Rows[lDataDirectories.SelectedCells[0].RowIndex].Cells[0].Value.ToString(), MathHelpsFileLoader.Load_Bitmap(DataPath + "Background.bmp").ToBitmap());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Beep();
            }

        }

        private void bCenter_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptingInterface.scriptingInterface.CreateGraph("Centering", GetDataFolder() + "Centering.avi", lDataDirectories.Rows[lDataDirectories.SelectedCells[0].RowIndex].Cells[0].Value.ToString());
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

            pPath += lDataDirectories.Rows[lDataDirectories.SelectedCells[0].RowIndex].Cells[0].Value.ToString() + "\\";
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

        void showOnMonitor(Form FormToDisplay, int showOnMonitor)
        {
            Screen[] sc;
            sc = Screen.AllScreens;
            //get all the screen width and heights 
            if (showOnMonitor > sc.Length - 1) showOnMonitor = sc.Length - 1;

            FormToDisplay.FormBorderStyle = FormBorderStyle.None;
            FormToDisplay.Left = sc[showOnMonitor].Bounds.Width;
            FormToDisplay.Top = sc[showOnMonitor].Bounds.Height;
            FormToDisplay.StartPosition = FormStartPosition.Manual;
            FormToDisplay.Show();
        }

        private void bDataView_Click(object sender, EventArgs e)
        {
            GraphForm3D gf3d = new GraphForm3D();
            gf3d.Zooming = true;
            try
            {
                string DataFile = GetDataFolder() + "ProjectionObject.dat";
                if (File.Exists(GetDataFolder() + "projectionobject_fbp_16_hc\\image0000.tif"))
                {
                    //C:\temp\testbad\data\projectionobject_fbp_16_512
                    string[] Slices = Directory.GetFiles(GetDataFolder() + "projectionobject_fbp_16_hc");

                    Slices = EffectHelps.SortNumberedFiles(Slices.ToArray());

                    gf3d.Zooming = true;
                    gf3d.Show(this);
                    gf3d.SetData(MathHelpsFileLoader.OpenDensityDataFloat(Slices));  //PhysicalArray.OpenDensityData(Slices));
                    gf3d.Text = lDataDirectories.Rows[lDataDirectories.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
                }
                else if (File.Exists(GetDataFolder() + "ProjectionObject.tif"))
                {

                    gf3d.Show(this);
                    gf3d.SetData(GetDataFolder() + "ProjectionObject.tif");
                    gf3d.Zooming = true;
                    gf3d.Text = lDataDirectories.Rows[lDataDirectories.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
                }
                else if (File.Exists(GetDataFolder() + "ProjectionObject.cct"))
                {

                    gf3d.Show(this);
                    gf3d.SetData(GetDataFolder() + "ProjectionObject.cct");
                    gf3d.Zooming = true;
                    gf3d.Text = lDataDirectories.Rows[lDataDirectories.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
                }
                else if (File.Exists(DataFile))
                {
                    gf3d.Show(this);
                    gf3d.SetData(DataFile);
                    gf3d.Zooming = true;
                    gf3d.Text = lDataDirectories.Rows[lDataDirectories.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
                    /*
                    ShowVoreenve(true);

                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(@"C:\Development\Voreen3\data\workspaces\templateQuad.xml");
                    XmlNodeList nodeList = xDoc.GetElementsByTagName("Origin");

                    nodeList[0].Attributes["filename"].Value = DataFile;

                    xDoc.Save(@"C:\Development\Voreen2008\data\workspaces\Brianview.vws");

                    OpenWorkspace(@"C:\Development\Voreen2008\data\workspaces\Brianview.vws");

                    showOnMonitor(new DataViewControl(), 2);
                     */
                }
                else if (File.Exists(GetDataFolder() + "ProjectionObject.raw"))
                {
                    gf3d.Show(this);
                    gf3d.SetData(GetDataFolder() + "ProjectionObject.raw");
                    gf3d.Zooming = true;
                    gf3d.Text = lDataDirectories.Rows[lDataDirectories.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                Console.Beep();
                gf3d.Close();
            }


            /*ProjectionObject po = new ProjectionObject();
            po.OpenDensityData(GetDataFolder() + "ProjectionObject.cct");

            GraphForm3D gf3d = new GraphForm3D();
            gf3d.Show(this);
            gf3d.SetData(po.ProjectionData);
            po = null;

        }
    }
    catch (OutOfMemoryException ex)
    {
        Light3DView l3D = new Light3DView();
        l3D.SetData(GetDataFolder() + "ProjectionObject.cct");
        l3D.Show(this);
    }*/
        }

        private void bDataviewVG_Click(object sender, EventArgs e)
        {
            try
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

                string basePath = ProcessedDrive + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_16_bit\\";
                if (Directory.Exists(basePath) == false)
                {
                    basePath = ProcessedDrive + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_cropped_16bit\\";
                    if (Directory.Exists(basePath) == false)
                    {
                        basePath = ProcessedDrive + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_8_bit\\";
                        if (Directory.Exists(basePath) == false)
                        {
                            basePath = ProcessedDrive + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_cropped_8bit\\";
                        }
                    }
                }
                string[] Slices = Directory.GetFiles(basePath);
                List<string> GoodFiles = new List<string>();
                foreach (string s in Slices)
                    if (Path.GetExtension(s).ToLower().Trim() != ".vvi")
                        GoodFiles.Add(s);

                Slices = EffectHelps.SortNumberedFiles(GoodFiles.ToArray());

                GraphForm3D gf3d = new GraphForm3D();
                gf3d.Zooming = true;
                gf3d.Show(this);
                gf3d.SetData(MathHelpsFileLoader.OpenDensityDataFloat(Slices));  //PhysicalArray.OpenDensityData(Slices));
                gf3d.Text = lDataDirectories.Rows[lDataDirectories.SelectedCells[0].RowIndex].Cells[0].Value.ToString() + "_VG";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Beep();
            }
        }

        #endregion

        private void bLoadRaw_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                float[, ,] data;

                if (openFileDialog1.FileNames.Length == 1)
                {
                    data = MathHelpLib.MathHelpsFileLoader.OpenDensityDataFloat(openFileDialog1.FileName);

                }
                else
                {
                    data = MathHelpLib.MathHelpsFileLoader.OpenDensityDataFloat(openFileDialog1.FileNames);
                }
                ScriptingInterface.scriptingInterface.MakeVariableVisible(Path.GetFileNameWithoutExtension(openFileDialog1.FileName), data);
                ScriptingInterface.scriptingInterface.CreateGraph(Path.GetFileNameWithoutExtension(openFileDialog1.FileName), data);
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
                            LMachine.Items.Add(machine);
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
                lYear.SetSelected(lYear.Items.Count - 1, true);
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
                lMonth.SetSelected(lMonth.Items.Count - 1, true);
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
                lDay.SetSelected(lDay.Items.Count - 1, true);
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
                ScriptingInterface.scriptingInterface.CreateGraph("MIP", GetDataFolder() + "mip.avi", lDataDirectories.Rows[lDataDirectories.SelectedCells[0].RowIndex].Cells[0].Value.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Beep();
            }
        }


        Emgu.CV.Capture CenteringMovie = null;
        Emgu.CV.Capture MIPMovie = null;

        static string LastSelectionFolder = "";
        static int LastRow = -1;
        private void SaveLastSelection()
        {
            if (valueChangedRow == LastRow)
            {
                String line = "";
                int row = LastRow;
                for (int i = 0; i < Cols.Length; i++)
                {
                    try
                    {
                        string val = lDataDirectories.Rows[row].Cells[colNames[i]].Value.ToString();
                        line += (colNames[i] + "==" + val + "\n");
                    }
                    catch { }
                }

                line += "Username==" +tUserName.Text   +"\n";

                // Write the string to a file.
                System.IO.StreamWriter file = new System.IO.StreamWriter(LastSelectionFolder);
                file.WriteLine(line);

                file.Close();

                if (LastRow != -1)
                {

                    lDataDirectories.Rows[LastRow].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }


        private delegate void ShowImage(Bitmap image);

        private void ShowCenteringImage(Bitmap image)
        {
            if (this.InvokeRequired == true)
            {
                this.BeginInvoke(new ShowImage(ShowCenteringImage), image);
            }
            else
            {
                pCentering.Image = image;
                pCentering.Invalidate();
                //Application.DoEvents();
            }
        }

        private void ShowMIPImage(Bitmap image)
        {
            if (this.InvokeRequired == true)
            {
                this.BeginInvoke(new ShowImage(ShowMIPImage), image);
            }
            else
            {
                pMIP.Image = image;
                pMIP.Invalidate();
                //Application.DoEvents();
            }
        }

        Thread mCenteringMovie = null;
        bool StopCenteringMovie = false;
        private void StartCenteringMovie(string DataPath)
        {
            if (mCenteringMovie != null && mCenteringMovie.IsAlive == true)
            {
                StopCenteringMovie = true;
                while (mCenteringMovie.IsAlive)
                {
                    Application.DoEvents();
                }
                //mCenteringMovie.Join();
            }
            StopCenteringMovie = false;
            try
            {
                mCenteringMovie = new Thread(delegate()
                    {
                        if (CenteringMovie != null)
                        {
                            CenteringMovie.Dispose();
                        }

                        try
                        {
                            if (File.Exists(DataPath + "Centering.avi"))
                                CenteringMovie = new Emgu.CV.Capture(DataPath + "Centering.avi");
                            else
                                CenteringMovie = null;



                            if (MIPMovie != null)
                            {
                                MIPMovie.Dispose();
                            }
                            if (File.Exists(DataPath + "MIP.avi"))
                                MIPMovie = new Emgu.CV.Capture(DataPath + "MIP.avi");
                            else
                                MIPMovie = null;

                            while (StopCenteringMovie == false && this.Visible == true)
                            {
                                if (CenteringMovie != null)
                                {
                                    try
                                    {
                                        ShowCenteringImage(CenteringMovie.QueryFrame().ToBitmap());
                                    }
                                    catch
                                    {
                                        CenteringMovie.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_POS_FRAMES, 0);
                                    }
                                }
                                if (MIPMovie != null)
                                {
                                    try
                                    {
                                        ShowMIPImage(MIPMovie.QueryFrame().ToBitmap());
                                    }
                                    catch
                                    {
                                        MIPMovie.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_POS_FRAMES, 0);
                                    }
                                }
                                Thread.Sleep(100);
                            }
                        }
                        catch { }
                    });
                mCenteringMovie.Start();
            }
            catch
            {
                CenteringMovie = null;
                MIPMovie = null;

                pMIP.Image = new Bitmap(10, 10);
                pCentering.Image = new Bitmap(10, 10);

            }

        }

        string DataPath;
        private void lDataDirectories_SelectionChanged(object sender, EventArgs e)
        {

            try
            {
                if (LastSelectionFolder != "")
                    SaveLastSelection();
            }
            catch
            {
            }


            try
            {
                DataPath = GetDataFolder();
                string PPPath = GetExperimentFolder() + "PP\\";

                StopCenteringMovie = true;
                timerCenterMovie.Enabled = false;
                timerCenterMovie.Enabled = true;

                pCentering.Image = new Bitmap(1, 1);
                pMIP.Image = new Bitmap(1, 1);
                /// StartCenteringMovie(DataPath);

                try
                {
                    pProj1.Image = new Bitmap(new Bitmap(DataPath + "projection1.jpg"));
                }
                catch { pProj1.Image = new Bitmap(10, 10); }

                try
                {
                    pProj2.Image = new Bitmap(new Bitmap(DataPath + "projection2.jpg"));
                }
                catch { pProj2.Image = new Bitmap(10, 10); }


                try
                {
                    pProj3.Image = new Bitmap(new Bitmap(DataPath + "projection3.jpg"));
                }
                catch { pProj3.Image = new Bitmap(10, 10); }


                try
                {
                    pProj4.Image = new Bitmap(new Bitmap(DataPath + "projection4.jpg"));
                }
                catch { pProj4.Image = new Bitmap(10, 10); }


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


                //  CrossSections_X_512_m_

                try
                {
                    pReconMine.Image = new Bitmap(new Bitmap(DataPath + "CrossSections_X___TIK.jpg"));
                }
                catch
                {
                    pReconMine.Image = new Bitmap(1, 1);
                }

                try
                {
                    Bitmap b = new Bitmap(DataPath + "VGExample.png");
                    //  b.RotateFlip(RotateFlipType.RotateNoneFlipXY );
                    // b.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    //ImageHolder intermediate = new ImageHolder(b);
                    pStack.Image = b;// new Bitmap(intermediate.ToBitmap());
                }
                catch
                {
                    pStack.Image = new Bitmap(1, 1);
                }

                try
                {
                    pReconX.Image = new Bitmap(new Bitmap(DataPath + "CrossSections_Y___TIK.jpg"));

                }
                catch
                {
                    pReconX.Image = new Bitmap(1, 1);
                }

                try
                {
                    pREcon2.Image = new Bitmap(new Bitmap(DataPath + "CrossSections_Z___TIK.jpg"));

                }
                catch
                {
                    pREcon2.Image = new Bitmap(1, 1);
                }

                //try
                //{
                //    //Bitmap b;
                //    //if (File.Exists(DataPath + "Stack.bmp"))
                //    //    b = new Bitmap(DataPath + "Stack.bmp");
                //    //else
                //    //    b = MathHelpsFileLoader.Load_Bitmap(DataPath + "Stack.tif").ToBitmap();
                //    //b.RotateFlip(RotateFlipType.Rotate270FlipY);//.Rotate270FlipNone);
                //    //pStack.Image = new Bitmap(b);

                //    var vg =   GetVGExample();

                //    vg.RotateFlip(RotateFlipType.Rotate270FlipY);

                //    pStack.Image = new Bitmap(vg);

                //}
                //catch
                //{
                //    pStack.Image = new Bitmap(1, 1);
                //}

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
                pStack.Invalidate();

                bBackground.Enabled = File.Exists(DataPath + "Background.bmp");
                // bShowCenteringTool.Enabled = File.Exists(DataPath + "x_Positions");
                bCenter.Enabled = File.Exists(DataPath + "Centering.avi");
                bDataView.Enabled = true;// File.Exists(DataPath + "projectionobject.cct") || File.Exists(DataPath + "projectionobject.raw") || File.Exists(DataPath + "projectionobject.tif");
                bFlyThrough.Enabled = File.Exists(DataPath + "flyThrough.avi");
                bMIP.Enabled = File.Exists(DataPath + "Mip.avi");
                bIntensity.Enabled = File.Exists(DataPath + "BGIntensityAverage");
                bFocusValue.Enabled = File.Exists(DataPath + "FocusValue");
                bShowFancy.Enabled = File.Exists(DataPath + "projectionobject.cct");

                LastSelectionFolder = DataPath + "UserGroundTruth.txt";
                LastRow = lDataDirectories.SelectedCells[0].RowIndex;
            }
            catch
            {
                LastSelectionFolder = "";
            }

        }

        private delegate void DoRowEvent(int Row, ReplaceStringDictionary Values);


        string[] Cols = new string[] { "recon", "run time", "centeringquality", "cell staining quality", "focusvalue", "backgroundsubtraction", "InterferingObject", "Comments", "Good Cell", "TooClose", "Interesting", "ReconQuality", "Noise", "Rings", "recon staining" };
        string[] colNames = new string[] { "ReconSucceeded", "Run_Time", "Registration_Quality", "Cell_Staining", "Focus_Quality", "Background", "Interfering_Object", "Comments", "Good_Cell", "TooClose", "Interesting", "Recon_Quality", "Noise", "Rings", "Number_Quality" };

        private void DoRow(int Row, ReplaceStringDictionary Values)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new DoRowEvent(DoRow), Row, Values);
            }
            else
            {
                for (int i = 0; i < Cols.Length; i++)
                {
                    try
                    {
                        string val = (string)Values[Cols[i]];

                        var cell = lDataDirectories.Rows[Row].Cells[colNames[i]];
                        cell.Value = val;
                    }
                    catch { }
                }

                try
                {
                    if (((string)Values["valuesentered"]) == "no")
                        lDataDirectories.Rows[Row].DefaultCellStyle.BackColor = Color.White;
                    else
                        lDataDirectories.Rows[Row].DefaultCellStyle.BackColor = Color.LightGray;

                }
                catch { }
            }
        }

        private void DoRow(int Row, string FilePath)
        {
            string DataPath = FilePath + "data\\";
            if (File.Exists(DataPath + "comments.txt") == false)
            {
                //lDataDirectories.Rows[Row].Cells[1].Value = "No Data";
                return;
            }

            ReplaceStringDictionary Values = DoRow(FilePath);

            DoRow(Row, Values);
        }

        private ReplaceStringDictionary DoRow(string FilePath)
        {
            ReplaceStringDictionary Values = new ReplaceStringDictionary();
            string DataPath = FilePath + "data\\";
            String line;
            string[] Parts;
            Values.AddSafe("ReconQuality", "-");
            try
            {
                if (File.Exists(DataPath + "UserCommentsNew.txt") == true)
                {
                    line = "";
                    using (StreamReader sr = new StreamReader(DataPath + "UserCommentsNew.txt"))
                    {
                        line = sr.ReadToEnd();
                    }


                    Parts = line.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < Parts.Length; i++)
                    {
                        string[] parts2 = Parts[i].Split(new string[] { "==" }, StringSplitOptions.None);
                        Values.AddSafe(Columns[parts2[0]], parts2[1]);
                    }

                    if (((string)Values["recon"]).Trim() != "")
                    {
                        Values.AddSafe("valuesentered", "yes");
                        return Values;
                    }
                }
            }
            catch { }

            if (File.Exists(DataPath + "comments.txt") == false)
            {
                Values.AddSafe("recon", "No Data");
                return Values;
            }


            using (StreamReader sr = new StreamReader(DataPath + "comments.txt"))
            {
                line = sr.ReadToEnd();
            }

            line = line.Replace("\r", "").Replace("\n", "").ToLower();
            Parts = line.Split(new string[] { "<", "/>", ">" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < Parts.Length; i += 2)
            {
                try
                {
                    Values.AddSafe(Parts[i], Parts[i + 1]);
                }
                catch { }
            }

            Values.AddSafe("Noise", "No");
            Values.AddSafe("Rings", "No");

            string outOfImageRange = "-";
            try
            {
                outOfImageRange = (string)Values["outofimagerange"];
                if (outOfImageRange == "True")
                    outOfImageRange = "Yes";
                else
                    outOfImageRange = "No";
            }
            catch { }
            Values.AddSafe("TooClose", outOfImageRange);

            string NumberOfCells = "";
            try
            {
                NumberOfCells = (string)Values["numberofcells"];
            }
            catch { }
            Values.AddSafe("numberofcells", NumberOfCells);

            string CenteringQuality = "-";
            try
            {
                CenteringQuality = (string)Values["center quality"];

                double dCenterQuality = double.Parse(CenteringQuality);


                if (line.Contains("bad centering") == true)
                    CenteringQuality = "Bad";
                else if (dCenterQuality > 4)
                    CenteringQuality = "Questionable";
                else
                    CenteringQuality = "OK";
            }
            catch
            {
                CenteringQuality = "-";
            }
            Values.AddSafe("centeringquality", CenteringQuality);

            string CenteringQuality2 = "";
            try
            {
                CenteringQuality2 = (string)Values["centeringqualityactual"];
            }
            catch { }
            Values.AddSafe("centeringquality2", CenteringQuality2);


            string CellStainingAverage = "";
            Values.AddSafe("cell staining quality", "");
            try
            {
                CellStainingAverage = (string)Values["cell staining average"];

                if (double.Parse(CellStainingAverage) > 10000)
                {
                    Values.AddSafe("cell staining quality", "OK");
                }
                // else
                // {
                //     Values.AddSafe("cell staining quality", "Bad");

                // }
            }
            catch { }
            Values.AddSafe("cell staining average", CellStainingAverage);


            string ReconAverage = "";
            try
            {
                ReconAverage = (string)Values["eval factor"];
                ReconAverage += (", " + (string)Values["eval f4"]);


                ReconAverage = (string)Values["reconvsstack"];
                //  ReconAverage = (string)Values["cylinder boundry"];
                //  ReconAverage = Math.Round(100 * double.Parse(ReconAverage) / double.Parse(CellStainingAverage)).ToString();
            }
            catch { }
            Values.AddSafe("recon staining", ReconAverage);


            string ReconSucceeded = "False";
            try
            {
                ReconSucceeded = (string)Values["recon"];
            }
            catch { }

            if (ReconSucceeded == "False")
            {
                if (File.Exists(DataPath + "ProjectionObject.cct") == true || File.Exists(DataPath + "ProjectionObject.tif") == true)
                {
                    ReconSucceeded = "False but succeeded";
                }
                else
                    ReconSucceeded = "False";
            }
            Values.AddSafe("recon", ReconSucceeded);


            string FocusValue = "-";
            string FocusVar = "";
            try
            {
                if (line.Contains("bad focus") == true)
                {
                    FocusValue = "Bad";
                }
                else if (line.Contains("questionable Focus"))
                    FocusValue = "Questionable";
                else
                {
                    FocusValue = ((string)Values["focusvalue"]);
                    FocusVar = ((string)Values["focusvar"]);


                    double dFocusValue = double.Parse(FocusValue);
                    double dFocusVar = Math.Round(100 * double.Parse((string)Values["focusvaluesd"]) / dFocusValue, 1);
                    if (dFocusVar > 18)
                        FocusValue = "Bad";
                    else if (dFocusVar > 9)
                        FocusValue = "Questionable";
                    else
                        FocusValue = "OK";

                    if (dFocusValue < 4 && FocusValue != "Bad")
                        FocusValue = "Questionable";
                }

            }
            catch
            {
                FocusValue = "-";

            }


            Values.AddSafe("focusvalue", FocusValue);
            Values.AddSafe("focusvar", FocusVar);

            string reconGood = "-";
            try
            {
                string d = (string)Values["reconvsstack"];
                double dd = double.Parse(d);
                if (dd > .8)
                    reconGood = "Good";
                else
                    reconGood = "Bad";
            }
            catch { }
            Values.AddSafe("ReconQuality", reconGood);
            Values.AddSafe("Recon_Quality", reconGood);

            string BackgroundSubtraction = "No Info";
            try
            {
                if (Values.ContainsKey("backgroundsubtraction"))
                    BackgroundSubtraction = (String)Values["backgroundsubtraction"];
                else if (Values.ContainsKey("background"))
                    BackgroundSubtraction = (String)Values["background"];

                if (Values.ContainsKey("backgroundremovalmethod"))
                    BackgroundSubtraction = (String)Values["backgroundremovalmethod"];

            }
            catch { }
            Values.AddSafe("backgroundsubtraction", BackgroundSubtraction);


            Values.AddSafe("InterferingObject", "");

            DataPath = FilePath + "\\data";

            string pPath = FilePath.Replace("\"", "").Replace("'", "");

            if (pPath.EndsWith("\\") == false)
                pPath += "\\";

            string dirName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(pPath));
            string[] parts = dirName.Split('_');
            string Prefix = parts[0];
            string Year = parts[1].Substring(0, 4);
            string month = parts[1].Substring(4, 2);
            string day = parts[1].Substring(6, 2);


            DataPath = Path.Combine(tbLabeledDrive.Text, Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName);
            Values.AddSafe("InterferingObject", "-");
            DataPath = DataPath + "\\MotionControlEvaluation.xml";
            if (File.Exists(DataPath))
            {
                line = "";
                using (StreamReader sr = new StreamReader(DataPath))
                {
                    line = sr.ReadToEnd();
                }
                string[] parts2 = line.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < parts2.Length; i++)
                {
                    if (parts2[i].Contains("Comments"))
                    {
                        string[] parts3 = parts2[i].Split(new string[] { "</", "\">" }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts3.Length > 1)
                        {
                            string Comments = parts3[1];
                            Values.AddSafe("Comments", Comments);
                        }
                    }
                    if (parts2[i].Contains("InterferingObject"))
                    {
                        string[] parts3 = parts2[i].Split(new string[] { "</", "\">" }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts3.Length > 1)
                        {
                            string Comments = parts3[1];
                            Values.AddSafe("InterferingObject", Comments);
                        }
                    }

                    if (parts2[i].Contains("BackgroundDebris"))
                    {
                        string[] parts3 = parts2[i].Split(new string[] { "</", "\">" }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts3.Length > 1)
                        {
                            string Comments = parts3[1];
                            Values.AddSafe("InterferingObject", Comments);
                        }
                    }

                    if (parts2[i].Contains("ObjectOutOfFocus"))
                    {
                        string[] parts3 = parts2[i].Split(new string[] { "</", "\">" }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts3.Length > 1)
                        {
                            string Comments = parts3[1];
                            string cur = ((string)Values["focusvalue"]);
                            if (cur != "OK")
                            {
                                if (Comments == "Yes")
                                    Values.AddSafe("focusvalue", "Bad");
                            }
                        }
                    }
                }
            }

            DataPath = FilePath + "data";
            Values.AddSafe("Good Cell", "-");

            if (File.Exists(DataPath + "\\UserComments.txt") == true)
            {
                line = "";
                using (StreamReader sr = new StreamReader(DataPath + "\\UserComments.txt"))
                {
                    line = sr.ReadToEnd();
                }

                Parts = line.Split(new string[] { "\r\n", "\r", "\n", "==" }, StringSplitOptions.RemoveEmptyEntries);

                bool SomethingDone = false;

                for (int i = 0; i < Parts.Length; i += 2)
                {
                    try
                    {
                        if (Parts[i] == "UserCellQuality")
                        {
                            try
                            {
                                int quality = int.Parse(Parts[i + 1]);
                                if (quality >= 5)
                                    Values.AddSafe("Good Cell", "Yes");
                                else
                                    Values.AddSafe("Good Cell", "No");
                            }
                            catch { }
                        }
                        if (Parts[i] == "Stain")
                        {
                            if (Parts[i + 1] != "False")
                                Values.AddSafe("cell staining average", "Bad");
                        }
                        if (Parts[i] == "Focus")
                        {
                            if (Parts[i + 1] == "True")
                                Values.AddSafe("focusvalue", "Bad");
                        }

                        if (Parts[i] == "TooClose")
                        {
                            if (Parts[i + 1] == "True")
                                Values.AddSafe("TooClose", "Yes");
                        }
                    }
                    catch { }
                }

            }

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
                //se.OpenExperimentFolder(GetExperimentFolder());
                se.OpenVGExperimentFolder(GetVGStack());
                se.Show();
                se.Text = lDataDirectories.Rows[lDataDirectories.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
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

        private void bShowFancy_Click(object sender, EventArgs e)
        {
            if (File.Exists(GetDataFolder() + "ProjectionObject.cct"))
            {
                ProcessRecons.vtkForm vtkForm = new ProcessRecons.vtkForm();
                vtkForm.Show(this);
                vtkForm.OpenVisualization(GetDataFolder() + "ProjectionObject.cct");


                Application.DoEvents();

                // ProcessGUI.SetVoreenData(GetDataFolder() + "ProjectionObject.dat");
            }
        }

        private void tBrowseDataFolder_Click(object sender, EventArgs e)
        {
            try
            {

                DialogResult ret = folderBrowserDialog1.ShowDialog();
                if (ret == DialogResult.OK)
                {
                    tDataInputFolder.Text = folderBrowserDialog1.SelectedPath;

                    //  bDataFolder_TextChanged(this, EventArgs.Empty);
                }

            }
            catch { }
        }

        private void tDataInputFolder_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(tDataInputFolder.Text) == true)
            {
                try
                {
                    string[] Directories = Directory.GetDirectories(tDataInputFolder.Text, "cct*");
                    lDataDirectories.Rows.Clear();

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
                catch { }
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string DataPath = GetDataFolder();
            PhysicalArray pa = PhysicalArray.OpenDensityData(DataPath + "Background.cct");
            pa.SaveData(DataPath + "background.tif");
            ScriptingInterface.scriptingInterface.CreateGraph("test", (double[,])pa.ActualData2D, "x", "y");
            //ScriptingInterface.scriptingInterface.CreateGraph("Background - " + lDataDirectories.SelectedRows[0].Cells[0].Value.ToString(), MathHelpsFileLoader.Load_Bitmap(DataPath + "Background.bmp").ToBitmap());
        }

        private void timerCenterMovie_Tick(object sender, EventArgs e)
        {
            timerCenterMovie.Enabled = false;
            StartCenteringMovie(DataPath);
        }

        int valueChangedRow = -1;
        private void lDataDirectories_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            valueChangedRow = e.RowIndex;
        }

        private void lDataDirectories_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

    }
}
