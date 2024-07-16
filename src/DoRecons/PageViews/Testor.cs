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
using MathHelpLib;
using ImageViewer.Filters;
using GraphingLib;
using System.Threading;
using MathHelpLib.CurveFitting;

namespace DoRecons.PageViews
{
    public partial class Testor : UserControl
    {
        public Testor()
        {
            InitializeComponent();
        }
        public ReconWorkFlow reconWorkFlow1;
        private void tDataFolder_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string[] DirectoryNames = Directory.GetDirectories(tDataFolder.Text);
                lDataFolders.Items.Clear();
                foreach (string s in DirectoryNames)
                    lDataFolders.Items.Add(Path.GetFileNameWithoutExtension(s));
            }
            catch { }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
            {
                tDataFolder.Text = folderBrowserDialog1.SelectedPath;
                tDataFolder_TextChanged(this, EventArgs.Empty);
            }
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            if (lDataFolders.SelectedIndex < 0)
                return;

            for (int ListI = 0; ListI < lDataFolders.Items.Count; ListI++)
            {
                if (lDataFolders.GetSelected(ListI) == true)
                {
                    // try
                    {
                        Dictionary<string, string> Properties = reconWorkFlow1.SaveGUI();

                        string basePath;

                        string DataPath1 = tDataFolder.Text;
                        if (DataPath1.EndsWith("\\") == false) DataPath1 = DataPath1 + "\\";
                        string DataPath = DataPath1 + /* lDataFolders.SelectedItem*/ lDataFolders.Items[ListI].ToString();

                        string dirName = "test";
                        try
                        {
                            dirName = Path.GetFileNameWithoutExtension(DataPath);
                            string[] parts = dirName.Split('_');
                            string Prefix = parts[0];
                            string Year = parts[1].Substring(0, 4);
                            string month = parts[1].Substring(4, 2);
                            string day = parts[1].Substring(6, 2);

                            basePath = DataPath1 + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;
                        }
                        catch
                        {
                            basePath = DataPath;
                        }

                        Properties.Add("StrictBackground", false.ToString());
                        Properties.Add("TempDirectory", Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + dirName);
                        Properties.Add("DataDirectory", DataPath);
                        Properties.Add("DataOut", DataPath);
                        Properties.Add("port", Program.Port.ToString());



                        Properties.Add("Centering", "Bad");
                        /*  Properties.Add("SecondCell","True");
                          Properties.Add("OriginalDataPath", @"Y:\Fluor\cct001\201203\15\cct001_20120315_084709");*/

                        if (Properties["LoadPreProcessed"] == "True" && Directory.Exists(DataPath + "\\dehydrated") != true)
                        {
                            Properties.Remove("LoadPreProcessed");
                            Properties.Add("LoadPreProcessed", "False");
                        }

                        //Properties.Add("Rehydrate", Directory.Exists(DataPath + "\\dehydrated").ToString());
                        // Properties.Add("Rehydrate",false.ToString());


                        //   Properties.Add("Dehydrate", true.ToString());


                        string[] Args = new string[Properties.Count * 2];
                        int cc = 0;
                        foreach (KeyValuePair<string, string> kvp in Properties)
                        {
                            Args[cc] = kvp.Key;
                            Args[cc + 1] = kvp.Value;
                            cc += 2;
                        }

                        Program.MainScriptRunner(Args);
                    }
                    // catch { }
                }
            }
        }


        public void NetworkMessage(string message)
        {
            uConsole1.AddLine(message);
        }

        public Dictionary<string, string> SaveReconProperties()
        {
            return reconWorkFlow1.SaveGUI();
        }
        public void SetReconProperties(Dictionary<string, string> Props)
        {
            reconWorkFlow1.SetupControl(Props);
        }

        private void Testor_Load(object sender, EventArgs e)
        {
            tDataFolder_TextChanged(this, EventArgs.Empty);
        }


        private string GetExperimentFolder()
        {
            string DataPath1 = tDataFolder.Text;
            if (DataPath1.EndsWith("\\") == false) DataPath1 = DataPath1 + "\\";
            string DataPath = DataPath1 + lDataFolders.SelectedItem.ToString();

            return DataPath;
        }

        private string GetDataFolder()
        {
            string DataPath1 = tDataFolder.Text;
            if (DataPath1.EndsWith("\\") == false) DataPath1 = DataPath1 + "\\";
            string DataPath = DataPath1 + lDataFolders.SelectedItem.ToString();

            return DataPath + "\\Data\\";
        }


        private void lDataFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            string DataPath1 = tDataFolder.Text;
            if (DataPath1.EndsWith("\\") == false) DataPath1 = DataPath1 + "\\";

            string DataPath = "";
            for (int i = 0; i < lDataFolders.Items.Count; i++)
                if (lDataFolders.GetSelected(i) == true)
                    DataPath = DataPath1 + lDataFolders.Items[i].ToString();

            if (DataPath == "")
                return;

            string PPPath = DataPath + "\\pp\\";
            string Path = DataPath + "\\Data\\";



            //prevent random errors by cloning the image data
            try
            {
                pPP0.Image = new Bitmap(new Bitmap(Path + "FirstPP.bmp"));
            }
            catch
            {
                if (Directory.Exists(PPPath))
                {
                    string[] Files = Directory.GetFiles(PPPath);
                    pPP0.Image = MathHelpsFileLoader.Load_Bitmap(Files[0]).ToBitmap();
                }
                else
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
            bCenter.Enabled = File.Exists(Path + "Centering.avi") || File.Exists(DataPath + @"\Data\temp\CenterMovie\Frame_000.bmp");
            bDataView.Enabled = File.Exists(Path + "projectionobject.raw") || File.Exists(Path + "projectionobject.cct") || File.Exists(Path + "projectionobject.tif");
            bFancyView.Enabled = File.Exists(Path + "projectionobject.dat");
            bMIP.Enabled = File.Exists(Path + "Mip.avi");
            bIntensity.Enabled = File.Exists(Path + "BGIntensityAverage");
            bFocusValue.Enabled = File.Exists(Path + "FocusValue");
            bShowStack.Enabled = File.Exists(DataPath + @"\STACK\000\000_0000m.png");
            try
            {
                lTextSummary.Text = System.IO.File.ReadAllText(Path + "Comments.txt");
            }
            catch
            {
                lTextSummary.Text = "";
            }
        }

        private void bDataView_Click(object sender, EventArgs e)
        {

            if (File.Exists(GetDataFolder() + "ProjectionObject.tif "))
            {
                GraphForm3D gf3d = new GraphForm3D();
                gf3d.Show(this);
                gf3d.SetData(GetDataFolder() + "ProjectionObject.tif");

            }
            else
                if (File.Exists(GetDataFolder() + "ProjectionObject.cct"))
                {
                    PhysicalArray pa = PhysicalArray.OpenDensityData(GetDataFolder() + "ProjectionObject.cct");

                    GraphForm3D gf3d = new GraphForm3D();
                    gf3d.Show(this);
                    gf3d.SetData(pa);
                    pa = null;
                }
                else if (File.Exists(GetDataFolder() + "ProjectionObject.dat"))
                {
                    PhysicalArray pa = PhysicalArray.OpenDensityData(GetDataFolder() + "ProjectionObject.dat");

                    GraphForm3D gf3d = new GraphForm3D();
                    gf3d.Show(this);
                    gf3d.SetData(pa);
                    pa = null;
                }


        }

        private void bFancyView_Click(object sender, EventArgs e)
        {
            if (File.Exists(GetDataFolder() + "ProjectionObject.dat"))
            {
                ReconForm.SetVoreenData(GetDataFolder() + "ProjectionObject.dat");
            }
        }

        private void bBackground_Click(object sender, EventArgs e)
        {
            try
            {

                string DataPath = GetDataFolder();
                ScriptingInterface.scriptingInterface.CreateGraph("Background", MathHelpLib.MathHelpsFileLoader.Load_Bitmap(DataPath + "Background.bmp").ToBitmap());
            }
            catch { }
        }

        private void bShowCenteringTool_Click(object sender, EventArgs e)
        {

        }

        private void bCenter_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(GetDataFolder() + "Centering.avi"))
                {

                    ScriptingInterface.scriptingInterface.CreateGraph("Centering", GetDataFolder() + "Centering.avi", lDataFolders.SelectedItem.ToString());

                }
            }
            catch { }
        }

        private void bMIP_Click(object sender, EventArgs e)
        {
            try
            {

                ScriptingInterface.scriptingInterface.CreateGraph("MIP", GetDataFolder() + "mip.avi", lDataFolders.SelectedItem.ToString());


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

        private void bShowStack_Click(object sender, EventArgs e)
        {
            StackExplorer se = new StackExplorer();
            se.OpenExperimentFolder(GetExperimentFolder());
            se.Show();
        }

        private void pReconstruction_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptingInterface.scriptingInterface.CreateGraph("Recon", (Bitmap)pReconstruction.Image);
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

        private void bTestScript_Click(object sender, EventArgs e)
        {

            float[] test = new float[100];
            test.MedianSmooth(11);

            return;

            TwoVolume tv = new TwoVolume();
            tv.Show();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] filters = Enum.GetNames(typeof(Filtering.FilterTypes));



            if (lDataFolders.SelectedIndex < 0)
                return;
            Dictionary<string, string> Properties = reconWorkFlow1.SaveGUI();


            string DataPath1 = tDataFolder.Text;
            if (DataPath1.EndsWith("\\") == false) DataPath1 = DataPath1 + "\\";
            string DataPath = DataPath1 + lDataFolders.SelectedItem.ToString();

            string dirName = Path.GetFileNameWithoutExtension(DataPath);
            string[] parts = dirName.Split('_');
            string Prefix = parts[0];
            string Year = parts[1].Substring(0, 4);
            string month = parts[1].Substring(4, 2);
            string day = parts[1].Substring(6, 2);

            string basePath = DataPath1 + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;

            Properties.Add("TempDirectory", DataPath + "\\temp");

            Properties.Add("DataDirectory", DataPath);
            Properties.Add("DataOut", DataPath);
            Properties.Add("port", Program.Port.ToString());




            string[] ppm = filters;//{ "none", "median", "average", "alphatrimmed", "opening", "closing"};

            for (int i = 0; i < ppm.Length; i++)
            {
                string[] Args = new string[Properties.Count * 2];
                int cc = 0;

                foreach (KeyValuePair<string, string> kvp in Properties)
                {
                    Args[cc] = kvp.Key;
                    if (kvp.Key == "FBPWindow")
                        Args[cc + 1] = ppm[i];

                    else
                        Args[cc + 1] = kvp.Value;

                    cc += 2;
                }

                Program.MainScriptRunner(Args);

                try
                {
                    File.Move(DataPath + "\\data\\projectionobject.cct", DataPath + "\\data\\projectionobject1024_MaskandIntensity_Curve_" + ppm[i] + ".cct");
                }
                catch { }

            }

            /*for (int i = 0; i < filters.Length / 2; i++)
            {
                string[] Args = new string[Properties.Count * 2];
                int cc = 0;

                foreach (KeyValuePair<string, string> kvp in Properties)
                {
                    Args[cc] = kvp.Key;
                    if (kvp.Key == "FBPWindow")
                        Args[cc + 1] = filters[i];
                    else if ("FBPResolution" == kvp.Key)
                        Args[cc + 1] = "1024";
                    else
                        Args[cc + 1] = kvp.Value;

                    cc += 2;
                }

                Program.MainScriptRunner(Args);

                try
                {
                    File.Move(DataPath + "\\data\\projectionobject.cct", DataPath + "\\data\\projectionobject1024_MaskandIntensity_Curve_" + filters[i] + ".cct");
                }
                catch { }
            }
        */


        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                string ProcessedDrive = "y:\\";
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
                    basePath = ProcessedDrive + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_8_bit\\";
                    if (Directory.Exists(basePath) == false)
                    {
                        basePath = ProcessedDrive + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_cropped_16bit\\";
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
                gf3d.Show(this);
                gf3d.SetData(PhysicalArray.OpenDensityData(Slices));
                // gf3d.Text = lDataDirectories.SelectedRows[0].Cells[0].Value.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Beep();
            }
        }






    }
}
