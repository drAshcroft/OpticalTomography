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
using System.Net;
using System.Net.Sockets;
using ImageViewer;
using GraphingLib;
using MathHelpLib.ImageProcessing;

namespace Tomographic_Imaging_2
{
    public partial class EasyGUI : DockContent
    {
        public EasyGUI()
        {
            InitializeComponent();
        }

        #region Procedures

        private string GetDesiredFilter()
        {
            for (int i = 0; i < lFilters.Items.Count; i++)
            {

                if (lFilters.GetSelected(i) == true)
                {
                    return lFilters.Items[i].ToString();
                }
            }
            return lFilters.Items[0].ToString();
        }
        private string GetDataInFolder()
        {
            //build the file structure
            string pPath;
            if (bDataFolder.Text.EndsWith("\\") == true)
                pPath = bDataFolder.Text;
            else
                pPath = bDataFolder.Text + "\\";
            return pPath;
        }
        private string GetExperimentFolder()
        {
            string pPath = GetDataInFolder() + lDataDirectories.SelectedItem.ToString() + "\\";
            string DataPath = Path.GetDirectoryName(pPath) + "\\";
            return DataPath;
        }
        private string GetDataFolder()
        {
            //build the file structure

            return GetExperimentFolder() + "Data\\";
        }

        private void RunPreProcessScripts(string DataDirectory, string Filtername, string ScriptName, int PaddedLength, int CutLength)
        {

            Filtering.FilterTypes FilterType = (Filtering.FilterTypes)Enum.Parse(typeof(Filtering.FilterTypes), Filtername);

            Dictionary<string, object> ScriptParams = new Dictionary<string, object>();
            ScriptParams.Add("ImpulseFilter", Filtering.GetRealSpaceFilter(FilterType, PaddedLength, CutLength, 1));

            //build the file structure
            string pPath = DataDirectory;

            string tempPath = Path.GetDirectoryName(pPath) + "\\Data\\temp\\";
            if (Directory.Exists(tempPath) == false)
                Directory.CreateDirectory(tempPath);

            string DataPath = Path.GetDirectoryName(pPath) + "\\Data\\";
            if (Directory.Exists(DataPath) == false)
                Directory.CreateDirectory(DataPath);


            string Script = ScriptName.ToLower();

            IScript ProgScript = null;
            if (Script == "scriptcombined")
                ProgScript = new ScriptCombinedArray();
            else if (Script == "scriptmovies")
                ProgScript = new ScriptMovies();

            try
            {
                List<string> ImagesIn = new List<string>();


                string[] PPs = Directory.GetFiles(pPath + "PP\\");
                string[] Sorted = MathStringHelps.SortNumberedFiles(PPs);
                ImagesIn.AddRange(Sorted);


                ScriptRunner.RunScripts(pImagePreview, aPreProcess.SecondBoxSelectedItem, ProgScript, ImagesIn, pPath, DataPath, true, ScriptParams, 1100);
            }
            catch (Exception ex)
            { }

            string[] eraseFiles = Directory.GetFiles(tempPath);


            lDataDirectories_SelectedIndexChanged(this, EventArgs.Empty);
        }



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
            if (cWatchInput.Checked)
            {
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    ReconDirectoryBacklog.Enqueue(e.Name.ToString());
                }
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
            DialogResult ret = folderBrowserDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                bDataFolder.Text = folderBrowserDialog1.SelectedPath;
                bDataFolder_TextChanged(this, EventArgs.Empty);
            }

        }


        private void bDataFolder_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(bDataFolder.Text) == true)
            {
                string SelectedPath = (string)lDataDirectories.SelectedItem;

                string[] Directories = Directory.GetDirectories(bDataFolder.Text);
                lDataDirectories.Items.Clear();
                for (int i = 0; i < Directories.Length; i++)
                    lDataDirectories.Items.Add(Path.GetFileName(Directories[i]));

                if (SelectedPath != null)
                {
                    for (int i = 0; i < lDataDirectories.Items.Count; i++)
                    {
                        if (SelectedPath == lDataDirectories.Items[i].ToString())
                        {
                            lDataDirectories.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                    lDataDirectories.SelectedIndex = 0;
                fileSystemWatcher1.Path = bDataFolder.Text;
            }
        }

        private void cWatchInput_CheckedChanged(object sender, EventArgs e)
        {
            fileSystemWatcher1.EnableRaisingEvents = cWatchInput.Checked;
            //  timerReconBacklog.Enabled = cWatchInput.Checked;
        }


        private void lDataDirectories_SelectedIndexChanged(object sender, EventArgs e)
        {
            bRunPreprocess.Enabled = true;

            try
            {
                //load one bitmap from the selected folder
                string Path;
                if (bDataFolder.Text.EndsWith("\\") == true)
                    Path = bDataFolder.Text;
                else
                    Path = bDataFolder.Text + "\\";

                string PPPath = Path + lDataDirectories.SelectedItem + "\\PP\\";



                Path += lDataDirectories.SelectedItem + "\\Data\\";

                string[] Files = Directory.GetFiles(PPPath);
                Files = MathStringHelps.SortNumberedFiles(Files);

                try
                {
                    pImagePreview.Image = MathHelpsFileLoader. Load_Bitmap(Files[0]).ToBitmap();
                }
                catch { }

                //prevent random errors by cloning the image data
                try
                {
                    pPP0.Image = new Bitmap(new Bitmap(Path + "FirstPP.bmp"));
                }
                catch
                {
                    pPP0.Image = new Bitmap(1, 1);
                }

                try
                {
                    pPPLast.Image = new Bitmap(new Bitmap(Path + "HalfPP.bmp"));
                }
                catch
                {
                    pPPLast.Image = new Bitmap(1, 1);
                }
                try
                {
                    pReconstruction.Image = new Bitmap(new Bitmap(Path + "Forward1.bmp"));
                }
                catch
                {
                    pReconstruction.Image = new Bitmap(1, 1);
                }
                //force a redraw
                pPP0.Invalidate();
                pPPLast.Invalidate();
                pReconstruction.Invalidate();


                bBackground.Enabled = File.Exists(Path + "Background.bmp");
                bShowCenteringTool.Enabled = File.Exists(Path + "x_Positions");
                bCenter.Enabled = File.Exists(Path + "Centering.avi") || File.Exists(GetExperimentFolder() + @"\Data\temp\CenterMovie\Frame_000.bmp");
                bDataView.Enabled = File.Exists(Path + "projectionobject.raw");
                bFlyThrough.Enabled = File.Exists(Path + "flyThrough.avi");
                bMIP.Enabled = File.Exists(Path + "Mip.avi");
                bIntensity.Enabled = File.Exists(Path + "BGIntensityAverage");
                bFocusValue.Enabled = File.Exists(Path + "FocusValue");
                bShowStack.Enabled = File.Exists(GetExperimentFolder() + @"\STACK\000\000_0000m.png");
                try
                {
                    lTextSummary.Text = System.IO.File.ReadAllText(Path + "Comments.txt");
                }
                catch
                {
                    lTextSummary.Text = "";
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

            string[] Scripts = Directory.GetFiles(ScriptDirectory);
            string[] ScriptRecons = Directory.GetFiles(ReconScriptDirectory);

            for (int i = 0; i < Scripts.Length; i++)
                Scripts[i] = Path.GetFileName(Scripts[i]);

            for (int i = 0; i < ScriptRecons.Length; i++)
                ScriptRecons[i] = Path.GetFileName(ScriptRecons[i]);


            aPreProcess.FirstBox.Items.Add("ScriptCenter");
            aPreProcess.FirstBox.Items.Add("ScriptCombined");
            aPreProcess.FirstBox.Items.Add("ScriptRecon");
            aPreProcess.FirstBox.Items.Add("ScriptMovies");

            aPreProcess.FirstBox.Items.AddRange(Scripts);

            if (bDataFolder.Text.Trim() != "")
            {
                bDataFolder_TextChanged(this, EventArgs.Empty);
            }

            aPreProcess.SecondBox.Items.Add("ScriptCombined");

            filewatchPreprocess.Path = ScriptDirectory;

            string[] filters = Enum.GetNames(typeof(Filtering.FilterTypes));
            for (int i = 0; i < filters.Length; i++)
                lFilters.Items.Add(filters[i]);

            lFilters.SetSelected(0, true);
        }

        #region Processing
        private void filewatchPreprocess_Created(object sender, FileSystemEventArgs e)
        {
            string ScriptDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\Scripts\\";

            string[] Scripts = Directory.GetFiles(ScriptDirectory);

            for (int i = 0; i < Scripts.Length; i++)
                Scripts[i] = Path.GetFileName(Scripts[i]);

            aPreProcess.FirstBox.Items.Clear();
            aPreProcess.FirstBox.Items.AddRange(Scripts);
        }

        private void filewatchPreprocess_Deleted(object sender, FileSystemEventArgs e)
        {
            filewatchPreprocess_Created(sender, e);
        }

        private void bEditScript_Click(object sender, EventArgs e)
        {
            string ScriptName = (string)aPreProcess.FirstBox.SelectedItem;
            if (ScriptName == null)
                ScriptName = (string)aPreProcess.SecondBox.SelectedItem;
            if (ScriptName != null)
            {

                ///get one image from the dataset for a test
                string Path;
                if (bDataFolder.Text.EndsWith("\\") == true)
                    Path = bDataFolder.Text;
                else
                    Path = bDataFolder.Text + "\\";
                Path += lDataDirectories.SelectedItem + "\\PP\\";

                try
                {
                    string[] bitmaps = Directory.GetFiles(Path);
                    Path = bitmaps[0];
                }
                catch
                {
                    Path = "";
                }

                //now show the script editor, image editor, and selected script
                EasyImageViewer eiv = new EasyImageViewer();
                eiv.Show();

                try
                {
                    // if (Path != "")
                    //eiv.SetImage(ImageViewer.ImagingTools.Load_Bitmap(Path));
                }
                catch { }
                eiv.ShowScript(ScriptName);
            }
            else
                MessageBox.Show("Please select a script to edit", "No preprocess script");
        }

        private void bCreateScript_Click(object sender, EventArgs e)
        {

            EasyImageViewer eiv = new EasyImageViewer();
            eiv.Show();

            //load one bitmap from the selected folder
            string Path;
            if (bDataFolder.Text.EndsWith("\\") == true)
                Path = bDataFolder.Text;
            else
                Path = bDataFolder.Text + "\\";
            Path += lDataDirectories.SelectedItem + "\\PP\\";

            try
            {
                string[] bitmaps = Directory.GetFiles(Path);
                Path = bitmaps[0];
                //now show imageditor with script editor
                //eiv.SetImage(ImageViewer.ImagingTools.Load_Bitmap(Path));
            }
            catch { }

            eiv.ShowScript("");
        }

        private void bRunSelectedPreProc_Click(object sender, EventArgs e)
        {

            RunPreProcessScripts(GetExperimentFolder(), GetDesiredFilter(), aPreProcess.SecondBox.Items[0].ToString(), (int)(nPaddedSize.Value), (int)nCutOff.Value);

        }

        private void bRunPreprocess_Click(object sender, EventArgs e)
        {
            ProcessRunning = true;
            timer1.Enabled = true;
            RunPreProcessScripts(GetExperimentFolder(), GetDesiredFilter(), aPreProcess.SecondBox.Items[0].ToString(), (int)(nPaddedSize.Value), (int)nCutOff.Value);
            timer1.Enabled = false;
            progressBar1.Value = progressBar1.Maximum;
            ProcessRunning = false;

        }

        private void aPreProcess_FirstBoxSelected()
        {
            bRunSelectedPreProc.Enabled = false;
            bEditScript.Enabled = false;
        }

        private void aPreProcess_SecondBoxSelected()
        {
            bRunSelectedPreProc.Enabled = true;
            bEditScript.Enabled = true;
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
            catch { }
        }


        private void bIntensity_Click(object sender, EventArgs e)
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


        private void bBackground_Click(object sender, EventArgs e)
        {
            //build the file structure
            try
            {
                string pPath;
                if (bDataFolder.Text.EndsWith("\\") == true)
                    pPath = bDataFolder.Text;
                else
                    pPath = bDataFolder.Text + "\\";

                pPath += lDataDirectories.SelectedItem.ToString() + "\\";
                string DataPath = Path.GetDirectoryName(pPath) + "\\Data\\";
                ScriptingInterface.scriptingInterface.CreateGraph("Background", MathHelpsFileLoader.Load_Bitmap(DataPath + "Background.bmp").ToBitmap());
            }
            catch { }

        }

        private void bCenter_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(GetDataFolder() + "Centering.avi"))
                {
                    ScriptingInterface.scriptingInterface.CreateGraph("Centering", GetDataFolder() + "Centering.avi");

                }
                else if (File.Exists(GetExperimentFolder() + @"\Data\temp\CenterMovie\Frame_000.bmp"))
                {
                    try
                    {
                        ScriptingInterface.scriptingInterface.CreateGraph("Centering", Directory.GetFiles(GetExperimentFolder() + @"\data\temp\CenterMovie\"));
                    }
                    catch { }
                }
            }
            catch { }
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

            pPath += lDataDirectories.SelectedItem.ToString() + "\\";
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
          //  try
            {
                /*if (File.Exists(GetDataFolder() + "ProjectionObject.cct"))
                {
                    ProjectionObject po = new ProjectionObject();
                    po.OpenDensityData(GetDataFolder() + "ProjectionObject.cct");

                    GraphForm3D gf3d = new GraphForm3D();
                    gf3d.Show(this);
                    gf3d.SetData(po.ProjectionData);
                    po = null;
                }*/

                if (File.Exists(GetDataFolder() + "ProjectionObject.dat") )
                {
                  

                    PhysicalArray pa=  PhysicalArray.OpenDensityData(GetDataFolder() + "ProjectionObject.dat");

                    GraphForm3D gf3d = new GraphForm3D();
                    gf3d.Show(this);
                    gf3d.SetData(pa);
                   pa  = null;
                }

            }
           // catch { }
        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = (progressBar1.Value + 1) % progressBar1.Maximum;
        }

        private void timerReconBacklog_Tick(object sender, EventArgs e)
        {
            if (!(WholeReconRunning == true || ReconRunning == true || ProcessRunning == true))
            {
                if (ReconDirectoryBacklog.Count > 0)
                {
                    WholeReconRunning = true;
                    string ExperimentFolderName = ReconDirectoryBacklog.Dequeue();
                    if (ExperimentFolderName != null && ExperimentFolderName != "")
                    {
                        string[] parts = ExperimentFolderName.Split(new string[] { "||" }, StringSplitOptions.None);
                        for (int j = 0; j < lDataDirectories.Items.Count; j++)
                        {
                            if (Path.GetDirectoryName((string)lDataDirectories.Items[j]) == Path.GetDirectoryName(parts[0]))
                                lDataDirectories.SetSelected(j, true);
                            else
                                lDataDirectories.SetSelected(j, false);
                        }

                        try
                        {
                            ProcessRunning = true;
                            timer1.Enabled = true;

                            int Padded = 512, Cutoff = 512;
                            int.TryParse(parts[2], out Padded);
                            int.TryParse(parts[3], out Cutoff);
                            RunPreProcessScripts(parts[0], parts[1], parts[4], Padded, Cutoff);
                        }
                        catch
                        {

                        }
                        timer1.Enabled = false;
                        progressBar1.Value = progressBar1.Maximum;
                        ProcessRunning = false;

                    }
                    WholeReconRunning = false;
                }
            }

        }

        private void bWholeRecon_Click(object sender, EventArgs e)
        {
            string pPath;
            List<string> Selected = new List<string>();
            for (int i = 0; i < lDataDirectories.Items.Count; i++)
            {
                if (lDataDirectories.GetSelected(i) == true)
                {
                    pPath = GetDataInFolder() + (string)lDataDirectories.Items[i] + "\\";
                    Selected.Add(pPath + "||" + GetDesiredFilter() + "||" + (nPaddedSize.Value).ToString() + "||" + (nPaddedSize.Value).ToString() + "||" + aPreProcess.SecondBox.Items[0].ToString());
                }
            }

            foreach (string p in Selected)
                ReconDirectoryBacklog.Enqueue(p);
        }


        private Queue<string> LittleQueue = new Queue<string>();
        private void bQueueDir_Click(object sender, EventArgs e)
        {
            string pPath;

            for (int i = 0; i < lDataDirectories.Items.Count; i++)
            {
                if (lDataDirectories.GetSelected(i) == true)
                {
                    pPath = GetDataInFolder() + (string)lDataDirectories.Items[i] + "\\";
                    LittleQueue.Enqueue(pPath + "||" + GetDesiredFilter() + "||" + (nPaddedSize.Value).ToString() + "||" + (nPaddedSize.Value).ToString() + "||" + aPreProcess.SecondBox.Items[0].ToString());
                }
            }
        }

        private void bDumpQueue_Click(object sender, EventArgs e)
        {
            foreach (string p in LittleQueue)
                ReconDirectoryBacklog.Enqueue(p);
        }

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


        #region Visiongate
        private void bOutputToSurveyor_Click(object sender, EventArgs e)
        {
            string OutputPath = tVGdrive.Text;
            string ExperimentFolder = GetExperimentFolder();
            ExperimentFolder = Path.GetFileName(ExperimentFolder.Substring(0, ExperimentFolder.Length - 2));

            string[] Parts = ExperimentFolder.Split('_');
            string FolderPrefix = Parts[0] + "A";
            string MonthAndYear = Parts[1].Substring(0, 6);
            string Day = Parts[1].Substring(6, Parts[1].Length - 6);

            ExperimentFolder = FolderPrefix + "_" + MonthAndYear + Day + "_" + Parts[2];
            OutputPath += "" + FolderPrefix + "\\" + MonthAndYear + "\\" + Day + "\\" + ExperimentFolder + "\\";

            string ExampleDir = Path.GetDirectoryName(Application.ExecutablePath) + "\\ExampleVG\\";
            //create the directory structure
            if (Directory.Exists(OutputPath) == false)
            {
                Directory.CreateDirectory(OutputPath);
                string[] Dirs = Directory.GetDirectories(ExampleDir, "*.*", SearchOption.AllDirectories);
                foreach (string DirName in Dirs)
                {
                    Directory.CreateDirectory(DirName.Replace(ExampleDir, OutputPath));
                }
            }

            string[] ExampleFiles = Directory.GetFiles(ExampleDir, "*.*", SearchOption.AllDirectories);
            foreach (string filename in ExampleFiles)
            {
                string outFile = filename.Replace(ExampleDir, OutputPath);
                File.Copy(filename, outFile, true);
            }

            string[] StackFiles = Directory.GetFiles(GetExperimentFolder() + "Stack\\000\\");
            foreach (string file in StackFiles)
                File.Copy(file, OutputPath + "fixedStackImages\\" + Path.GetFileName(file), true);

            ProjectionObject po = new ProjectionObject();
            po.OpenDensityData(GetDataFolder() + "ProjectionObject.cct");
            po.SaveDensityData(OutputPath + "500PP\\recon_cropped_8bit\\reconCrop8bit.png");
            po.SaveDensityData(OutputPath + "500PP\\recon_cropped_16bit\\reconCrop16bit.png");

            Bitmap b = new Bitmap(GetDataFolder() + "FirstPP.bmp");
            b.Save(OutputPath + "500pp\\cropped000.png");
            int CellWidth = b.Width;
            int CellHeight = b.Height;

            b = new Bitmap(GetDataFolder() + "HalfPP.bmp");
            b.Save(OutputPath + "500pp\\cropped249.png");

            b = new Bitmap(GetDataFolder() + "background.bmp");
            b.Save(OutputPath + "500pp\\noise.png");

            b = new Bitmap(GetDataFolder() + "forward1.bmp");
            b.Save(OutputPath + "500pp\\volume_thumbnail.jpg");
            b.Save(OutputPath + "500pp\\visualization_preview.jpg");

            File.Copy(GetDataFolder() + "mip.avi", OutputPath + "500pp\\visualization_" + ExperimentFolder + ".avi", true);

            File.Copy(GetDataFolder() + "centering.avi", OutputPath + "corrppmovie.avi", true);

            string ReconVV =
@"<VVOpenWizard Version=""1.8""
        Spacing=""0.0734 0.0734 0.0734""
        Origin=""0 0 0""
        DistanceUnits=""Microns""
        ScalarType=""3""
        WholeExtent=" + string.Format("\"{0} {1} {2} {3} {4} {5}\"", 0, po.ProjectionData.GetLength(Axis.XAxis), 0, po.ProjectionData.GetLength(Axis.YAxis), 0, po.ProjectionData.GetLength(Axis.ZAxis)) + @" 
        NumberOfScalarComponents=""1""
        IndependentComponents=""1""
        FileOrientation=""10 2 0""
        BigEndianFlag=""0""
        FilePattern=""reconCrop8bit_%03d.png""
        FileDimensionality=""2""/>";

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(OutputPath + "500PP\\recon_cropped_8bit\\reconCrop8bit_000.png.vvi"))
            {
                file.WriteLine(ReconVV);
                file.Close();
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(OutputPath + "500PP\\recon_cropped_8bit\\reconCrop16bit_000.png.vvi"))
            {
                file.WriteLine(ReconVV);
                file.Close();
            }

            ReplaceStringDictionary FileMessages = new ReplaceStringDictionary();
            using (System.IO.StreamReader file = new System.IO.StreamReader(GetDataFolder() + "Comments.txt"))
            {
                string[] Lines = file.ReadToEnd().Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < Lines.Length; i++)
                {

                    string[] parts = Lines[i].Split(new string[] { "<", "/>", ">" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1)
                        FileMessages.AddSafe(parts[0], parts[1]);
                }
            }



            //Here is the variable with which you assign a new value to the attribute
            string newValue = string.Empty;
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(OutputPath + "MotionControlReport.xml");

            XmlNode node = xmlDoc.SelectSingleNode("DataSet/ReconstructionSizeX");
            node.Attributes[0].Value = po.ProjectionData.GetLength(Axis.XAxis).ToString();

            node = xmlDoc.SelectSingleNode("DataSet/ReconstructionSizeY");
            node.Attributes[0].Value = po.ProjectionData.GetLength(Axis.YAxis).ToString();

            node = xmlDoc.SelectSingleNode("DataSet/ReconstructionSizeZ");
            node.Attributes[0].Value = po.ProjectionData.GetLength(Axis.ZAxis).ToString();


            node = xmlDoc.SelectSingleNode("DataSet/RegistrationCenterX");
            node.Attributes[0].Value = (po.ProjectionData.GetLength(Axis.XAxis) / 2).ToString();

            node = xmlDoc.SelectSingleNode("DataSet/RegistrationCenterY");
            node.Attributes[0].Value = (po.ProjectionData.GetLength(Axis.YAxis) / 2).ToString();

            node = xmlDoc.SelectSingleNode("DataSet/RegistrationCenterZ");
            node.Attributes[0].Value = (po.ProjectionData.GetLength(Axis.ZAxis) / 2).ToString();


            node = xmlDoc.SelectSingleNode("DataSet/ObjectSizeX");
            node.Attributes[0].Value = CellWidth.ToString();

            node = xmlDoc.SelectSingleNode("DataSet/ObjectSizeY");
            node.Attributes[0].Value = CellWidth.ToString();

            node = xmlDoc.SelectSingleNode("DataSet/ObjectSizeZ");
            node.Attributes[0].Value = CellHeight.ToString();

            node = xmlDoc.SelectSingleNode("DataSet/Status");
            node.Attributes[0].Value = FileMessages["ErrorMessage"].ToString();
            if (FileMessages["ErrorMessage"].ToString() == "Succeeded")
                node.Attributes[1].Value = "Succeeded";
            else
                node.Attributes[1].Value = "Failed";

            node = xmlDoc.SelectSingleNode("DataSet/NoiseCorrection");
            node.Attributes["incompleteNoiseCorrection"].Value = (FileMessages["ErrorMessage"].ToString() == "Background mask created").ToString();


            xmlDoc.Save(OutputPath + "MotionControlReport.xml");

            //xmlFile is the path of your file to be modified

            //Here is the variable with which you assign a new value to the attribute

            xmlDoc = new XmlDocument();

            xmlDoc.Load(OutputPath + "500pp\\NoiseCorrectionReport.xml");

            node = xmlDoc.SelectSingleNode("DataSet/Status");
            if (FileMessages["ErrorMessage"].ToString() == "Background mask created")
            {
                node.Attributes["message"].Value = "Noise image generated successfully";
                node.Attributes["value"].Value = "Succeeded";
            }
            else
            {
                node.Attributes["message"].Value = "Bad Noise Image";
                node.Attributes["value"].Value = "Failed";
            }



            xmlDoc.Save(OutputPath + "MotionControlReport.xml");


        }





        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            string pPath;
            List<string> Selected = new List<string>();
            for (int i = 0; i < lDataDirectories.Items.Count; i++)
            {
                if (lDataDirectories.GetSelected(i) == true)
                {
                    pPath = GetDataInFolder() + (string)lDataDirectories.Items[i] + "\\";
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
                ScriptRunner.StartInfo.Arguments = "\"" + p + "\" \"" + GetDesiredFilter() + "\" " + nPaddedSize.Value.ToString() + " " + nCutOff.Value.ToString() + " " + aPreProcess.SecondBoxSelectedItem;

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
            public string Path;
            public System.Diagnostics.Stopwatch sw = new Stopwatch();
            public Process ScriptRunner;
            public TimeSpan MaxTime = new TimeSpan(0, 30, 0);
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
                        Console.WriteLine("Killed overlong script");
                        ScriptRunner.Kill();
                        return false;
                    }
                    if (ScriptRunner.HasExited == true)
                        return false;
                }
                catch
                {
                    return false;
                }
                return true;
            }
            public string YearMonthAndDay
            {
                get
                {
                    return "";
                }
            }
        }

        UdpClient udpClient;
        private int StartNetworkListener(int port)
        {
            if (udpClient == null)
            {
                //check if this port is already being used
                while (udpClient == null)
                {
                    try
                    {
                        udpClient = new UdpClient(port);
                    }
                    catch
                    {
                        port++;
                    }
                }
                Thread MonitorTreads = new Thread(delegate(object Vars)
                    {
                        while (this.IsDisposed == false)
                        {
                            //IPEndPoint object will allow us to read datagrams sent from any source.
                            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);

                            // Blocks until a message returns on this socket from a remote host.
                            Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                            string returnData = Encoding.ASCII.GetString(receiveBytes);

                            // Uses the IPEndPoint object to determine which of these two hosts responded.
                            Console.WriteLine(/*RemoteIpEndPoint.Address.ToString() + ":" + RemoteIpEndPoint.Port.ToString() + " --- " +*/ returnData.ToString());
                        }
                        udpClient.Close();
                    });

                MonitorTreads.Start();
            }
            return port;
        }

        private bool mPauseBatch = false;
        private List<ProcessHolder> ProcessHolderList = new List<ProcessHolder>();
        private void button2_Click(object sender, EventArgs e)
        {
            int CommunicationPort = StartNetworkListener(1100);

            string STorage = tArchiveFolder.Text;

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


                    if (Directory.Exists(basePath) == false && Directory.Exists(AllDirs[i] + "\\pp\\") && Directory.Exists(AllDirs[i] + "\\stack\\"))
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
                    if (ProcessHolderList.Count < 3)
                    {
                        string DirPath = Selected.Dequeue();
                        Process ScriptRunner = new Process();
                        ScriptRunner.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

                        //ScriptRunner.StartInfo.FileName = @"C:\Development\CellCT\runtime - Copy\ScriptRunner.exe";
                        ScriptRunner.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + @"\ScriptRunner.exe";

                        //ScriptRunner.StartInfo.Arguments = "\"" + DirPath + "\" \"" + GetDesiredFilter() + "\" " + nPaddedSize.Value.ToString() + " " + nCutOff.Value.ToString() + " " + aPreProcess.SecondBoxSelectedItem;
                        string DataPath = tOutPath.Text;
                        if (DataPath.EndsWith("\\") == true)
                            DataPath = DataPath.Substring(0, DataPath.Length - 1);
                        ScriptRunner.StartInfo.Arguments = "\"" + DirPath + "\" \"Han\" " + nPaddedSize.Value.ToString() + " " + nCutOff.Value.ToString() + " " + aPreProcess.SecondBoxSelectedItem + " " + "\"" + DataPath + "\" " + CommunicationPort;

                        ScriptRunner.Start();

                        ProcessHolder ph = new ProcessHolder(ScriptRunner);
                        ph.Path = DirPath;
                        ProcessHolderList.Add(ph);

                        sw.Restart();

                        lProcessProgress.Text = Selected.Count + "/" + StartFolders + "    " + Math.Round((1 - Selected.Count / (double)StartFolders) * 100) + "%";

                        while (sw.Elapsed < maxWait && ScriptRunner.HasExited == false)
                            Application.DoEvents();
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
                            ProcessHolderList.RemoveAt(i);
                    }
                }
                catch { }
                Application.DoEvents();
            }

            this.Text = "Cleaning missed stuff";
            button3_Click(this, EventArgs.Empty);
            MessageBox.Show("All Recons are finished");
        }

        private void bPauseBatch_Click(object sender, EventArgs e)
        {
            mPauseBatch = !mPauseBatch;
        }


        private void bMIP_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptingInterface.scriptingInterface.CreateGraph("MIP", GetDataFolder() + "mip.avi");
            }
            catch { }
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

        private void button3_Click(object sender, EventArgs e)
        {
            int CommunicationPort = StartNetworkListener(1100);
            //string STorage = @"G:\storage\cct002\201012\06\";
            string STorage = tArchiveFolder.Text;

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
            Dictionary<string, List<string>> ExistingBackgrounds = new Dictionary<string, List<string>>();
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

                    string basePath = tOutPath.Text + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;
                    string ExperimentFolder = basePath;
                    if (Directory.Exists(basePath) == true)
                        Attempted++;

                    // basePath += "\\Data\\Comments.txt";
                    basePath += "\\Data\\ProjectionObject.cct";

                    if (File.Exists(basePath) == false)
                    {
                        pPath = Path.GetFileNameWithoutExtension(AllDirs[i]);
                        if (pPath.Length > 10 && pPath.Substring(0, 2) == "cc")
                        {
                            Selected.Enqueue(AllDirs[i]);
                        }
                    }
                    else
                    {
                        if (ExistingBackgrounds.ContainsKey(Prefix + Year + month + day) == false)
                        {
                            ExistingBackgrounds.Add(Prefix + Year + month + day, new List<string>());
                        }

                        ExistingBackgrounds[Prefix + Year + month + day].Add(ExperimentFolder);
                        pPath = "";
                        Completed++;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }

            }


            foreach (List<string> folders in ExistingBackgrounds.Values)
            {
                double[,] AverageBackGround = null;
                int cc = 0;
                string LastFolderName = "";
                foreach (string ExperimentFolder in folders)
                {
                    try
                    {
                        double[,] ih = GetBackgroundImageOneCell(ExperimentFolder + "\\", true);
                        if (ih != null)
                        {
                            if (AverageBackGround == null)
                            {
                                AverageBackGround = ih;
                            }
                            else
                                AverageBackGround.AddInPlace(ih);
                            cc++;
                        }
                        LastFolderName = ExperimentFolder;
                    }
                    catch { }
                }
                //if there were backgrounds, but nothing comes through, try again less picky about the backgrounds
                if (AverageBackGround == null)
                {
                    foreach (string ExperimentFolder in folders)
                    {
                        try
                        {
                            double[,] ih = GetBackgroundImageOneCell(ExperimentFolder + "\\", false);
                            if (ih != null)
                            {
                                if (AverageBackGround == null)
                                {
                                    AverageBackGround = ih;
                                }
                                else
                                    AverageBackGround.AddInPlace(ih);
                                cc++;
                            }
                            LastFolderName = ExperimentFolder;
                        }
                        catch { }
                    }

                }
                if (AverageBackGround != null)
                {
                    AverageBackGround.DivideInPlace(cc);
                    MathHelpsFileLoader.Save_Raw(Directory.GetParent(LastFolderName).FullName + "\\AllBackground.cct", AverageBackGround);
                    AverageBackGround.MakeBitmap().Save(Directory.GetParent(LastFolderName).FullName + "\\AllBackground.bmp");
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
                    Process ScriptRunner = new Process();

                    ScriptRunner.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + @"\ScriptRunner.exe";
                    string DataPath = tOutPath.Text;
                    if (DataPath.EndsWith("\\") == true)
                        DataPath = DataPath.Substring(0, DataPath.Length - 1);

                    ScriptRunner.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    ScriptRunner.StartInfo.Arguments = "\"" + DirPath + "\" \"Han\" " + nPaddedSize.Value.ToString() + " " + nCutOff.Value.ToString() + " " + aPreProcess.SecondBoxSelectedItem + " " + "\"" + DataPath + "\" " + CommunicationPort;
                    ScriptRunner.Start();

                    ProcessHolder ph = new ProcessHolder(ScriptRunner);
                    ph.Path = DirPath;
                    ProcessHolderList.Add(ph);

                    sw.Restart();
                    lProcessProgress.Text = Selected.Count + "/" + StartFolders + "    " + Math.Round((1 - Selected.Count / (double)StartFolders) * 100) + "%";
                    while (sw.Elapsed < maxWait && ScriptRunner.HasExited == false)
                        Application.DoEvents();
                }

                for (int i = 0; i < ProcessHolderList.Count; i++)
                {
                    if (ProcessHolderList[i].CheckTime() == false)
                        ProcessHolderList.RemoveAt(i);
                }
            }
            MessageBox.Show("Finished Second Process");
        }

        private void bShowStack_Click(object sender, EventArgs e)
        {
            StackExplorer se = new StackExplorer();
            se.OpenExperimentFolder(GetExperimentFolder());
            se.Show();
        }

        private void ImageEditor_Click(object sender, EventArgs e)
        {
            EasyImageViewer eiv = new EasyImageViewer();
            eiv.Show();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string pPath;

            for (int i = 0; i < lDataDirectories.Items.Count; i++)
            {
                if (lDataDirectories.GetSelected(i) == true)
                {
                    string dirName = (string)lDataDirectories.Items[i];
                    dirName = dirName.Replace("cct001_20110301_103547", "");

                    pPath = GetDataInFolder() + (string)lDataDirectories.Items[i] + "\\";
                    if (pPath.Contains("3DMedian") == true)
                    {
                        string filtername = dirName.Replace("3DMedian", "");
                        if (filtername == "RamLak") filtername = "Rectangular";
                        if (filtername == "ShepLogan") filtername = "Sinc";
                        LittleQueue.Enqueue(pPath + "||" + filtername + "||" + (512).ToString() + "||" + (512).ToString() + "||scriptrecon");
                    }
                    else if (pPath.Contains("Median") == true)
                    {
                        string filtername = dirName.Replace("Median", "");
                        if (filtername == "RamLak") filtername = "Rectangular";
                        if (filtername == "ShepLogan") filtername = "Sinc";
                        LittleQueue.Enqueue(pPath + "||" + filtername + "||" + (512).ToString() + "||" + (512).ToString() + "||scriptcenter");
                    }
                    else if (pPath.Contains("Low") == true)
                    {
                        string filtername = dirName.Replace("Low", "");
                        if (filtername == "RamLak") filtername = "Rectangular";
                        if (filtername == "ShepLogan") filtername = "Sinc";
                        LittleQueue.Enqueue(pPath + "||" + filtername + "||" + (170).ToString() + "||" + (170).ToString() + "||scriptcombined");

                    }
                    else
                    {
                        string filtername = dirName.Replace("NO", "");
                        if (filtername == "RamLak") filtername = "Rectangular";
                        if (filtername == "ShepLogan") filtername = "Sinc";
                        LittleQueue.Enqueue(pPath + "||" + filtername + "||" + (512).ToString() + "||" + (512).ToString() + "||scriptcombined");

                    }
                }
            }

        }

        private void bCopy_Click(object sender, EventArgs e)
        {
            string STorage = bCopySource.Text;

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
            int Completed = 0;
            int Attempted = 0;
            for (int i = 0; i < AllDirs.Length; i++)
            {

                pPath = AllDirs[i];
                string Outpath = pPath.Replace(bCopySource.Text, bCopyOutput.Text);



                if (Directory.Exists(Outpath) == false || (File.Exists(pPath + "\\data\\projectionoutput.cct") == true && File.Exists(Outpath + "\\data\\projectionoutput.cct") == false))
                {
                    //Directory.(pPath, Outpath);
                    CopyAll(new DirectoryInfo(pPath), new DirectoryInfo(Outpath));
                    Completed++;
                }
                else
                {
                    pPath = "";

                }


            }
            Console.WriteLine(Completed);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it’s new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        #region FixQual
        private void bFixReconQual_Click(object sender, EventArgs e)
        {
            string STorage = tOutPath.Text;

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

            for (int i = 1; i < AllDirs.Length; i += 2)
            {
                try
                {
                    string pPath = AllDirs[i].Replace("\"", "").Replace("'", "");

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
                        if (File.Exists(basePath + " \\data\\projectionobject.cct") == true/* && File.Exists(basePath + " \\data\\correctedQualityScore.txt") == false*/)
                        {
                            lProcessProgress.Text = i + " / " + AllDirs.Length + " == " + (100d * (double)i / (double)AllDirs.Length).ToString();
                            double[, ,] densityData = OpenDensityData(basePath + "\\data\\projectionobject.cct");
                            double[,] Slice = new double[densityData.GetLength(2), densityData.GetLength(1)];

                            int HX = densityData.GetLength(0) / 2;
                            int HY = densityData.GetLength(1) / 2;
                            int HZ = densityData.GetLength(2) / 2;

                            for (int x = 0; x < Slice.GetLength(0); x++)
                                for (int y = 0; y < Slice.GetLength(1); y++)
                                {
                                    Slice[Slice.GetLength(0) - x - 1, y] = densityData[HX, y, x];
                                }

                        /*    double[,] slice2 = new double[Slice.GetLength(0) / 2, Slice.GetLength(1) / 2];
                            for (int x = 0; x < slice2.GetLength(0); x++)
                            {
                                try
                                {
                                    for (int y = 0; y < slice2.GetLength(1); y++)
                                    {
                                        slice2[x, y] = Slice[x * 2, y * 2];
                                    }
                                }
                                catch { }
                            }
                            Slice = slice2;*/

                            pImagePreview.Image = Slice.MakeBitmap();
                            //  Slice.MakeBitmap().Save(basePath + "\\data\\reconslice.bmp");
                            pImagePreview.Invalidate();
                            Application.DoEvents();

                            Bitmap BestMatch = null;
                            double[] Fits = MathHelpLib.ProjectionFilters.ReconQualityCheckTool.ImageQuality(Slice,true, basePath + "\\stack", false, 0, 1, null);

                            string line = "";

                            line += "RightQualityFull==" + Fits[0].ToString() + "\r\n";
                            line += "RightQualityValueLowerHalf==" + Fits[1].ToString() + "\r\n";
                            line += "RightQualityValueUpperHalf==" + Fits[2].ToString() + "\r\n";
                            line += "RightQualityValueLowerThird==" + Fits[3].ToString() + "\r\n";
                            line += "RightQualityValueUpperQuarter==" + Fits[4].ToString() + "\r\n";
                            line += "RightQualityValueLowerThirdReo==" + Fits[5].ToString() + "\r\n";
                            line += "RightQualityValueUpperQuarterReo==" + Fits[6].ToString() + "\r\n";

                            line += "RightQualityValueFreq==" + Fits[7].ToString() + "\r\n";
                            line += "RightQualityValueLowerHalfFreq==" + Fits[8].ToString() + "\r\n";
                            line += "RightQualityValueUpperHalfFreq==" + Fits[9].ToString() + "\r\n";
                            line += "RightQualityValueLowerThirdFreq==" + Fits[10].ToString() + "\r\n";
                            line += "RightQualityValueUpperQuarterFreq==" + Fits[11].ToString() + "\r\n";
                            line += "RightQualityValueLowerThirdReoFreq==" + Fits[12].ToString() + "\r\n";
                            line += "RightQualityValueUpperQuarterReoFreq==" + Fits[13].ToString() + "\r\n";

                            try
                            {
                                System.IO.StreamWriter file = new System.IO.StreamWriter(basePath + "\\data\\correctedQualityScore.txt");
                                file.WriteLine(line);

                                file.Close();
                            }
                            catch { }
                            if (BestMatch != null)
                            {
                                ScriptingInterface.scriptingInterface.CreateGraph("BestMatch", BestMatch);
                                ScriptingInterface.scriptingInterface.CreateGraph("Recon", (Bitmap)pImagePreview.Image);
                            }
                        }
                        else
                        {

                        }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }

            }
        }

        public double[, ,] OpenDensityData(string Filename)
        {
            string Extension = Path.GetExtension(Filename).ToLower();
            if (Path.GetExtension(Filename).ToLower() == ".cct")
            {
                #region Open Raw
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);
                int sizeX, sizeY, sizeZ;

                int ArrayRank = Reader.ReadInt32();

                sizeX = Reader.ReadInt32();
                sizeY = Reader.ReadInt32();
                sizeZ = Reader.ReadInt32();

                double[, ,] mDensityGrid = new double[sizeZ, sizeY, sizeX];

                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();
                Reader.ReadDouble();


                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int z = 0; z < sizeZ; z++)
                        {
                            mDensityGrid[z, y, x] = Reader.ReadDouble();
                        }
                    }
                }

                Reader.Close();
                BinaryFile.Close();
                return mDensityGrid;
                #endregion
            }
            else if (Path.GetExtension(Filename).ToLower() == ".bin")
            {
                #region Open Bin

                #endregion
            }
            return null;

        }



        #endregion

        private void pReconstruction_Click(object sender, EventArgs e)
        {

        }

      










    }
}
