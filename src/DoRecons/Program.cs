using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using Tomographic_Imaging_2;
using System.IO;
using MathHelpLib;
using System.Threading;
using DoRecons.Scripts;
using MathHelpLib.Recon;
using MathHelpLib;
using MathHelpLib.ImageProcessing;
using System.Drawing;
using ImageViewer;
using ImageViewer.Filters;

namespace DoRecons
{
    public class Program
    {


        static void TestRecons()
        {

            DataEnvironment dataEnvironment = new DataEnvironment();

            string[] TestImageFiles = Directory.GetFiles(@"C:\Dehydrated\cct001\201202\10\cct001_20120210_160444");

            string[] ImagesIn = MathHelpLib.MathStringHelps.SortNumberedFiles(TestImageFiles);
            string TempDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp";

            dataEnvironment.AllImages = new ImageLibrary(ImagesIn, true, TempDirectory,false);


            for (int i = 0; i < dataEnvironment.AllImages.Count; i++)
            {
                int w = dataEnvironment.AllImages[i].Width;// = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(dataEnvironment.AllImages[i], new Rectangle(100,100,100,100));
                var image = dataEnvironment.AllImages[i].ToEMGU();
                dataEnvironment.AllImages[i] = new ImageHolder(image.PyrDown().PyrDown());
            }


            dataEnvironment.ScriptParams = new Dictionary<string, string>();
            dataEnvironment.ScriptParams.Add("FBPMedian", "False");

            ImageViewer.PythonScripting.Projection.TemplateReconstruction tr = new ImageViewer.PythonScripting.Projection.TemplateReconstruction();

            ReplaceStringDictionary PassData = new ReplaceStringDictionary();

            ImageHolder BitmapImage = dataEnvironment.AllImages[1];

            int ReconCellSize = BitmapImage.Width;

            IEffect Filter;

            Filter = new ImageViewer.PythonScripting.Projection.CreateFilteredBackProjectionEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ReconCellSize, ReconCellSize, 2, 2, 1, false, true);
            MathHelpLib.ProjectionFilters.ProjectionArrayObject DensityGrid = (MathHelpLib.ProjectionFilters.ProjectionArrayObject)Filter.PassData["FBJObject"];


            //cut the recon cell out of the middle of the clipping
            //ReconCutDownRect = new Rectangle((int)((FineCellSize - ReconCellSize) / 2), (int)((FineCellSize - ReconCellSize) / 2), ReconCellSize, ReconCellSize);

            int FilterWidth = 256;
            DensityGrid.impulse = Filtering.GetRealSpaceFilter("Han", FilterWidth, FilterWidth, (double)FilterWidth / 2d);
            DensityGrid.DesiredMethod = ConvolutionMethod.Convolution1D;
            //DensityGrid.ConvolutionFilter = new ImageViewer.PythonScripting.Projection.Convolution1D();

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();
            Console.WriteLine(sw.Elapsed.ToString());
            //  tr.FBPReconstruction(dataEnvironment, DensityGrid);
            Console.WriteLine(sw.Elapsed.ToString());

            sw.Stop();
            Bitmap[] b = DensityGrid.ShowCross();

            System.Diagnostics.Debug.Print(b[0].Width.ToString());


            DensityGrid.SaveCross(@"C:\temp\test.bmp");


            Filter = new ImageViewer.PythonScripting.Projection.CreateFilteredBackProjectionEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ReconCellSize, ReconCellSize, 2, 2, 1, false, true);
            DensityGrid = (MathHelpLib.ProjectionFilters.ProjectionArrayObject)Filter.PassData["FBJObject"];


            //cut the recon cell out of the middle of the clipping
            //ReconCutDownRect = new Rectangle((int)((FineCellSize - ReconCellSize) / 2), (int)((FineCellSize - ReconCellSize) / 2), ReconCellSize, ReconCellSize);


            DensityGrid.impulse = Filtering.GetRealSpaceFilter("Han", FilterWidth, FilterWidth, (double)FilterWidth / 2d);
            DensityGrid.DesiredMethod = ConvolutionMethod.Convolution1D;
            //DensityGrid.ConvolutionFilter = new ImageViewer.PythonScripting.Projection.Convolution1D();


            OldReconstruction or = new OldReconstruction();

            sw.Reset();
            sw.Start();
            Console.WriteLine("Old-" + sw.Elapsed.ToString());
            or.FBPReconstruction(dataEnvironment, DensityGrid, new Rectangle(0, 0, ReconCellSize, ReconCellSize));

            Console.WriteLine("Old-" + sw.Elapsed.ToString());

            sw.Stop();
            b = DensityGrid.ShowCross();

            System.Diagnostics.Debug.Print("Old-" + b[0].Width.ToString());


            DensityGrid.SaveCross(@"C:\temp\testOLD.bmp");

        }

        [STAThread]
        static void Main(string[] args)
        {
            //TestRecons();            return;
            if (args.Length > 0)
                MainScriptRunner(args);
            else
                MainReconForm();

        }

        public static int Port;
        static void MainReconForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ReconForm rf = new ReconForm();
            Random rnd = new Random(DateTime.Now.Millisecond);
            Port = 1100 + (int)Math.Round((double)rnd.Next(255));
            try
            {
                Common.StartNetworkListener(rf, Port);
            }
            catch
            {
                Common.StartNetworkListener(rf, Port + 5);
            }


            // Application.Run(new ProcessGUI ());
            Application.Run(rf);
        }


        public static void MainScriptRunner(string[] args)
        {
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            string DataPath = "";
            Display display = new Display();
            Dictionary<string, string> dArgs = new Dictionary<string, string>();
            string pPath = "";
            try
            {


                for (int i = 0; i < args.Length; i += 2)
                {
                    try
                    {
                        System.Diagnostics.Debug.Print(args[i] + "," + args[i + 1]);
                        dArgs.Add(args[i], args[i + 1]);
                    }
                    catch { }
                }


                int.TryParse(dArgs["port"], out Port);

                Console.WriteLine(dArgs["DataDirectory"]);
                display.Show();
                display.BringToFront();
                display.WindowState = FormWindowState.Normal;
                display.Caption = dArgs["DataDirectory"];

                string Foldername = Path.GetDirectoryName(dArgs["DataDirectory"]);


                Common.StartNetworkWriter(Foldername, Port);

                pPath = dArgs["DataOut"];

                if (pPath.EndsWith("\\") == false)
                    pPath += "\\";

                Common.SendNetworkPacket("", pPath);

                string tempPath = dArgs["TempDirectory"];
                if (Directory.Exists(tempPath) == false)
                    Directory.CreateDirectory(tempPath);

                DataPath = pPath + "Data\\";
                if (Directory.Exists(DataPath) == false)
                    Directory.CreateDirectory(DataPath);

                string StackPath = pPath + "Stack\\";
                Common.SendNetworkPacket("StackPath", StackPath);
                if (Directory.Exists(StackPath) == false)
                    Directory.CreateDirectory(StackPath);


                Common.SendNetworkPacket("NumArgs", dArgs.Count.ToString());


                IScript MyScript = null;

                string ExperimentFolder = dArgs["DataDirectory"] + "\\PP\\";
                string stackFolder = dArgs["DataDirectory"] + "\\stack\\000";

                if (dArgs.ContainsKey("Dehydrate") == true)
                {
                    MyScript = new DehydrateScript2();
                }
                else if (dArgs.ContainsKey("SecondCell") == true && dArgs["SecondCell"] == "True")
                {
                    MyScript = new SecondCell();
                }
                else
                {
                    MyScript = new BaseScriptSingle2();
                }

                Console.WriteLine(MyScript.ToString());
                //
                // ExperimentFolder = dArgs["DataDirectory"] + "\\samantha\\";
                // ExperimentFolder = dArgs["DataDirectory"] + "\\PP\\";

                string dirName = Path.GetFileNameWithoutExtension(dArgs["DataDirectory"]);
                string[] parts = dirName.Split('_');
                string Prefix = parts[0];
                string Year = parts[1].Substring(0, 4);
                string month = parts[1].Substring(4, 2);
                string day = parts[1].Substring(6, 2);

                string basePath;
                basePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + Prefix + "\\" + Year + month + "\\" + day;

                if (Directory.Exists(basePath) == false)
                    Directory.CreateDirectory(basePath);

                dArgs.Add("BackFolder", basePath + "\\back_" + dirName + ".tif");

                string dehydratefolder = "c:\\Dehydrated\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;

                if (Directory.Exists(dehydratefolder))
                    Directory.Delete(dehydratefolder, true);
                Directory.CreateDirectory(dehydratefolder);
                dArgs.Add("dehydrateFolder", dehydratefolder);

                string[] Images = null;
                if (dArgs.ContainsKey("LoadPreProcessed") == false || dArgs["LoadPreProcessed"] == "False")
                {
                    while (Directory.Exists(ExperimentFolder) == false)
                        Thread.Sleep(100);

                    while (Images == null || Images.Length == 0)
                    {
                        Images = Directory.GetFiles(ExperimentFolder);
                    }
                }
                else
                {
                    while (Images == null || Images.Length == 0)
                    {
                        Images = Directory.GetFiles(dArgs["DataDirectory"] + "\\dehydrated", "*.cct");
                    }
                }

                string[] Sorted = MathStringHelps.SortNumberedFiles(Images);
                string exten = Path.GetExtension(Images[0]);


                string VGFolder = "y:\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;

                dArgs.Add("vgfolder", VGFolder);

                // dArgs.Add("processedfolder", "y:\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName);

                dArgs.Add("StackFolder", stackFolder);
                try
                {
                    List<string> ImagesIn = new List<string>();
                    if (dArgs.ContainsKey("WaitFiles") == true && dArgs["WaitFiles"].ToLower() == "true")
                    {
                        for (int i = 1; i < 500; i++)
                            ImagesIn.Add(ExperimentFolder + string.Format("{0:000}{1}", i, exten));
                    }
                    else
                    {
                        foreach (string s in Sorted)
                        {
                            string exten2 = Path.GetExtension(s).ToLower();
                            if (exten2 == ".ivg" || exten2 == ".png" || exten2 == ".bmp" || exten2 == ".cct" || exten2 == ".tiff" || exten2 == ".tif")
                                ImagesIn.Add(s);
                        }
                    }

                    //  ImagesIn.Clear();
                    //  ImagesIn.AddRange(Directory.GetFiles(dehydratefolder));

                    Console.WriteLine("Running script");
                    ScriptRunner.RunScripts(display.picture, true, ImagesIn.ToArray(), MyScript, dArgs, Port);

                }
                catch (Exception ex)
                {
                    Console.Beep();
                    if (Directory.Exists(DataPath) == false)
                        Directory.CreateDirectory(DataPath);

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(DataPath + "Comments.txt", true))
                    {

                        file.WriteLine("<ErrorMessageOut><" + ex.Message + "/>");
                    }

                    Console.WriteLine(ex.Message);
                    if (ex.InnerException != null)
                        Console.WriteLine(ex.InnerException.Message);
                    // Console.ReadKey();
                }

                try
                {
                    Console.WriteLine("Cleaning temp files");
                    string[] eraseFiles = Directory.GetFiles(tempPath);
                }
                catch { }
            }
            catch (Exception ex)
            {
                //     Exception ex = null;
                Console.Beep();
                if (Directory.Exists(DataPath) == false)
                    Directory.CreateDirectory(DataPath);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(DataPath + "Comments.txt", true))
                {
                    file.WriteLine("<ErrorMessageOut><" + ex.Message + "/>");
                }
                try
                {
                    Common.SendNetworkPacket("StackPath", ex.Message);
                }
                catch { }
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);

                // Console.ReadKey();
            }

            if (dArgs.ContainsKey("MoveFiles") == true && dArgs["MoveFiles"].ToLower() == "true" && pPath != "")
            {
                try
                {
                    /*  Console.WriteLine("Moving Files");
                      string SourceDir = dArgs["DataDirectory"];

                      string[] Files = Directory.GetFiles(SourceDir, "*.*", SearchOption.AllDirectories);
                    
                      for (int i = 0; i < Files.Length; i++)
                      {
                          try
                          {
                              string OutFile = pPath + Files[i].Replace(SourceDir + "\\", " ").Trim();
                              string dir = Path.GetDirectoryName(OutFile);
                              if (Directory.Exists(dir) == false)
                                  Directory.CreateDirectory(dir);

                              File.Move(Files[i], OutFile);
                              Console.WriteLine(Files[i]);
                          }
                          catch { }
                      }

                    //  Directory.Delete(SourceDir, true);*/
                }
                catch (Exception ex)
                {
                    Common.SendNetworkPacket("Move Error", ex.Message);
                }
            }
            display.Close();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.Beep();
            Console.Beep();
            Console.Beep();

            Exception ex = (Exception)e.ExceptionObject;
            Console.WriteLine(ex.Message);
            if (ex.InnerException != null)
                Console.WriteLine(ex.InnerException.Message);
            //Console.ReadKey();
        }

    }
}
