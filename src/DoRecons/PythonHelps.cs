using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using MathHelpLib;
using ImageViewer;
using ImageViewer.Filters;
using ImageViewer.PythonScripting;
using System.Windows.Forms;
using MathHelpLib.ProjectionFilters;
using System.Drawing;
using System.Threading.Tasks;
using MathHelpLib.ImageProcessing;
using System.Xml;

namespace DoRecons
{
    public class PythonHelps
    {

        public static void OpenXMLAndProjectionLocations(string Filename, CellPositions positions)
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(Filename);

            XmlNodeList nodes = xmlDoc.SelectNodes("DataSet/PseudoProjection/BoundingBox");
            // nodes = xmlDoc.GetElementsByTagName("PseudoProjection");

            if (nodes.Count > 0)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    XmlNode node = nodes[i];

                    int Bottom = int.Parse(node.Attributes["bottom"].Value.ToString());
                    int Top = int.Parse(node.Attributes["top"].Value.ToString());
                    int Left = int.Parse(node.Attributes["left"].Value.ToString());
                    int Right = int.Parse(node.Attributes["right"].Value.ToString());

                    positions.X_Positions[i] = (Left + Right) / 2;
                    positions.Y_Positions[i] = (Top + Bottom) / 2;

                    positions.CellSizes[i] = Math.Abs(Left - Right);

                }
            }

        }


        #region Batchloop
        private object CriticalSection = new object();
        private static DataEnvironment sDataEnvironment;
        private static BaseScriptSingle2 sScript;
        private static bool sSaveResult = true;

        public static void BatchSaveFine(int ImageIndex)
        {
            ImageHolder ih = sScript.ProcessImageFindFine(ImageIndex);
            if (sSaveResult)
                sDataEnvironment.AllImages[ImageIndex] = ih;
            sDataEnvironment.ImageDisp.DisplayImage(ImageIndex, ih);
        }

        public static void BatchSaveLoad(int ImageIndex)
        {
            ImageHolder ih = sScript.ProcessImageLoad(ImageIndex);
            if (sSaveResult)
                sDataEnvironment.AllImages[ImageIndex] = ih;
            sDataEnvironment.ImageDisp.DisplayImage(ImageIndex, ih);
        }

        public static void BatchSaveDivide(int ImageIndex)
        {
            ImageHolder ih = sScript.ProcessImageDivide(ImageIndex);
            if (sSaveResult)
                sDataEnvironment.AllImages[ImageIndex] = ih;
            sDataEnvironment.ImageDisp.DisplayImage(ImageIndex, ih);
        }

        public static void BatchSaveClip(int ImageIndex)
        {
            ImageHolder ih = sScript.ProcessImageClip(ImageIndex);
            if (sSaveResult)
                sDataEnvironment.AllImages[ImageIndex] = ih;
            sDataEnvironment.ImageDisp.DisplayImage(ImageIndex, ih);
        }

        public static void BatchSaveFBP(int ImageIndex)
        {
            ImageHolder ih = sScript.ProcessImageFBP(ImageIndex);
            if (sSaveResult)
                sDataEnvironment.AllImages[ImageIndex] = ih;
            sDataEnvironment.ImageDisp.DisplayImage(ImageIndex, ih);
        }

        public static void BatchSaveBeforeConvolution(int ImageIndex)
        {
            ImageHolder ih = sScript.ProcessBeforeConvolution(ImageIndex);
            if (sSaveResult)
                sDataEnvironment.AllImages[ImageIndex] = ih;
            sDataEnvironment.ImageDisp.DisplayImage(ImageIndex, ih);
        }

        public static void BatchSaveConvolution(int ImageIndex)
        {
            ImageHolder ih = null;// sScript.ProcessConvolution(ImageIndex);
            if (sSaveResult)
                sDataEnvironment.AllImages[ImageIndex] = ih;
            sDataEnvironment.ImageDisp.DisplayImage(ImageIndex, ih);
        }


        public static void BatchLoopThroughImages(string IProcessFunction, BaseScriptSingle2 baseScript, DataEnvironment dataEnvironment, bool SaveOutput)
        {
            sDataEnvironment = dataEnvironment;
            sScript = baseScript;
            sSaveResult = SaveOutput;

            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = Environment.ProcessorCount - 1;

            int numberOfImages = dataEnvironment.AllImages.Count;

            if (IProcessFunction == "FindFine")
                Parallel.For(0, numberOfImages, po, x => BatchSaveFine(x));
            else if (IProcessFunction == "Divide")
                Parallel.For(0, numberOfImages, po, x => BatchSaveDivide(x));
            else if (IProcessFunction == "ClipImages")
                Parallel.For(0, numberOfImages, po, x => BatchSaveClip(x));
            else if (IProcessFunction == "DoFBPProjection")
                Parallel.For(0, numberOfImages, po, x => BatchSaveFBP(x));
            else if (IProcessFunction == "LoadImages")
                Parallel.For(0, numberOfImages, po, x => BatchSaveLoad(x));
            else if (IProcessFunction == "BeforeConvolution")
                Parallel.For(0, numberOfImages, po, x => BatchSaveBeforeConvolution(x));
            else if (IProcessFunction == "Convolution")
                Parallel.For(0, numberOfImages, po, x => BatchSaveConvolution(x));

            Thread.Sleep(0);
        }
        #endregion

        public static void print(object message)
        {
            Console.WriteLine(message.ToString());
            //  dataEnvironment.ProgressLog.AddSafe("Debug", message.ToString());
        }
        public static int len(Array array)
        {
            return array.Length;
        }
        public static int len(System.Collections.IList array)
        {
            return array.Count;
        }
        public static double min(double[] array)
        {
            return array.Min();
        }
        public static double max(double[] array)
        {
            return array.Max();
        }
        public static double Average(double[] array)
        {

            return array.Average();
        }
        public static double Stdev(double[] array)
        {
            double ave = array.Average();
            double sum = 0;
            double d = 0;
            for (int i = 0; i < array.Length; i++)
            {
                d = array[i] - ave;
                sum += d * d;
            }
            sum = Math.Sqrt(sum / array.Length);
            return sum;
        }
        public static double Stdev(double[] array, double Average)
        {
            double ave = Average;
            double sum = 0;
            double d = 0;
            for (int i = 0; i < array.Length; i++)
            {
                d = array[i] - ave;
                sum += d * d;
            }
            sum = Math.Sqrt(sum) / array.Length;
            return sum;
        }

        public static Rectangle GetCellAreaFromInfoFile(string Filename, int ImageWidth, int ImageHeight)
        {
            using (StreamReader sr = new StreamReader(Filename))
            {
                int CellXPos = 0;
                int BoxWidth = 0;

                String line = sr.ReadToEnd();
                string[] lines = line.Split('>', '<');
                // Read and display lines from the file until the end of
                // the file is reached.
                for (int i = 0; i < lines.Length; i++)
                {
                    line = lines[i];
                    if (line.Contains("CellXPos") == true && CellXPos == 0)
                    {
                        CellXPos = int.Parse(lines[i + 1]);
                    }

                    if (line.Contains("BoxWidth") == true && BoxWidth == 0)
                    {
                        BoxWidth = int.Parse(lines[i + 1]);
                    }

                    if (CellXPos != 0 && BoxWidth != 0)
                    {
                        BoxWidth = (int)(BoxWidth * 1.5);
                        return new Rectangle((int)(CellXPos - 1.5 * BoxWidth), 0, BoxWidth * 2, ImageHeight);
                    }
                }
            }

            return new Rectangle(0, 0, ImageWidth, ImageHeight);
        }

        public static Dictionary<string, object> SetupVariables(string ExperimentPath, string DataPath)
        {
            Dictionary<string, object> Variables = new Dictionary<string, object>();


            string TempDirectory = ExperimentPath + "\\temp\\data";


            while (Directory.Exists(DataPath) == false)
                Thread.Sleep(100);

            string[] Images = null;
            while (Images == null || Images.Length == 0)
            {
                Images = Directory.GetFiles(DataPath);
            }
            string[] Sorted = MathStringHelps.SortNumberedFiles(Images);
            string exten = Path.GetExtension(Images[0]);



            List<string> ImagesIn = new List<string>();

            foreach (string s in Sorted)
            {
                string exten2 = Path.GetExtension(s).ToLower();
                if (exten2 == ".ivg" || exten2 == ".png" || exten2 == ".bmp" || exten2 == ".cct" || exten2 == ".tiff" || exten2 == ".tif")
                    ImagesIn.Add(s);
            }

            ImageViewer.DataEnvironment dataEnvironment = new ImageViewer.DataEnvironment();
            dataEnvironment.WholeFileList = new List<string>();
            dataEnvironment.WholeFileList.AddRange(ImagesIn);
            dataEnvironment.ThreadsRunning = null;
            dataEnvironment.Screen = null;
            dataEnvironment.ProcCount = Environment.ProcessorCount;
            dataEnvironment.AllImages = new ImageLibrary(ImagesIn.ToArray(), true, TempDirectory,false );
            dataEnvironment.NumberOfRunningThreads = dataEnvironment.ProcCount;
            dataEnvironment.ExperimentFolder = ExperimentPath;
            dataEnvironment.DataOutFolder = ExperimentPath + "\\data";
            dataEnvironment.ProgressLog = new ReplaceChatStringDictionary(Path.GetFileNameWithoutExtension(dataEnvironment.DataOutFolder), 1100);
            dataEnvironment.FluorImage = true;

            Display display = new Display();
            display.Show();
            display.BringToFront();
            display.WindowState = FormWindowState.Normal;
            display.Caption = ExperimentPath;


            Variables.Add("TempPath", TempDirectory);
            Variables.Add("ImageDisp", new ImageDisplayer(display.picture));
            Variables.Add("DataPath", ExperimentPath + "\\data");
            Variables.Add("Executable", Application.ExecutablePath);
            Variables.Add("ExecutablePath", Path.GetDirectoryName(Application.ExecutablePath) + "\\");
            Variables.Add("LibraryPath", Path.GetDirectoryName(Application.ExecutablePath) + "\\ImageViewer.dll");
            Variables.Add("dataEnvironment", dataEnvironment);
            Variables.Add("GlobalPassData", new ImageViewer.Filters.ReplaceStringDictionary());
            Variables.Add("ImagesInMemory", true);
            Variables.Add("PhysicalImages", true);
            Variables.Add("RunningThreaded", true);



            Dictionary<string, string> ScriptParams = new Dictionary<string, string>();
            ScriptParams.Add("GlobalFlatten", "true");
            ScriptParams.Add("BackgroundSubMethod", "TopAndBottom");
            ScriptParams.Add("COGMethod", "Threshold");
            ScriptParams.Add("FlatMethod", "plane");

            ScriptParams.Add("PreprocessingMethod", "median");
            ScriptParams.Add("ProprocessingRadius", "3");
            ScriptParams.Add("FBPWindow", "Han");
            ScriptParams.Add("FBPResolution", "512");
            ScriptParams.Add("FBPMedian", "False");

            ScriptParams.Add("SaveAsCCT", "True");
            ScriptParams.Add("SaveAsRawDouble", "False");
            ScriptParams.Add("SaveAsRawFloat", "False");
            ScriptParams.Add("SaveAsRawInt", "False");
            ScriptParams.Add("Save8Bit", "False");
            ScriptParams.Add("Save16Bit", "False");

            ScriptParams.Add("SaveMIP", "True");
            ScriptParams.Add("SaveCenteringMovie", "True");
            ScriptParams.Add("CopyStack", "False");
            ScriptParams.Add("DoConvolutionQuality", "False");
            Variables.Add("ScriptParams", ScriptParams);

            return Variables;
        }

        public static bool IsColorImage(DataEnvironment dataEnvironment)
        {
            bool ColorImage;
            try
            {
                Dictionary<string, string> Values = EffectHelps.OpenXMLAndGetTags(dataEnvironment.ExperimentFolder + "\\info.xml", new string[] { "Color" });
                if (Values["Color"].ToLower() == "true")
                    ColorImage = true;
                else
                    ColorImage = false;

                dataEnvironment.ProgressLog.AddSafe("IsColor", ColorImage.ToString());
            }
            catch
            {
                dataEnvironment.ProgressLog.AddSafe("IsColor", "True");
            }

            //Console.WriteLine("Forcing image to monochrome!!!!!");
            return dataEnvironment.ProgressLog["IsColor"].ToString().ToLower() == "true";

        }

        /* public static XYThreadPool StartThreadPool(int MinThreads)
         {
             XYThreadPool myPool2 = new XYThreadPool();
             myPool2.StartThreadPool(MinThreads, MinThreads+10);
             return myPool2;
            
         }
         public static void InsertThreadpoolWorkItem(XYThreadPool threadpool,string sName,Delegate Method, bool StoreOutput, params object[] Params )
         {
             threadpool.InsertWorkItem(sName, Method, Params, StoreOutput);
            // Thread t = new Thread(new ParameterizedThreadStart (
         }*/

        public static void Save3DVolume(string DataPath, ProjectionArrayObject DensityGrid, Dictionary<string, string> ScriptParams)
        {
            if (ScriptParams["SaveAsCCT"].ToLower() == "true")
            {
                DensityGrid.SaveFile(DataPath + "\\ProjectionObject.cct");
            }
            if (ScriptParams["SaveAsRawDouble"].ToLower() == "true")
            {
                DensityGrid.SaveFileRaw(DataPath + "\\ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.Float32);
            }
            if (ScriptParams["SaveAsRawFloat"].ToLower() == "true")
            {
                DensityGrid.SaveFileRaw(DataPath + "\\ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.Float32);
            }
            if (ScriptParams["SaveAsRawInt"].ToLower() == "true")
            {
                DensityGrid.SaveFileRaw(DataPath + "\\ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.UInt16);
            }
            if (ScriptParams["Save8Bit"].ToLower() == "true")
            {
                if (Directory.Exists(DataPath + "\\VirtualStack8\\") == false)
                    Directory.CreateDirectory(DataPath + "\\VirtualStack8\\");
                DensityGrid.SaveFile(DataPath + "\\VirtualStack8\\VStack.tif", 8);
            }
            if (ScriptParams["Save16Bit"].ToLower() == "true")
            {
                if (Directory.Exists(DataPath + "\\VirtualStack16\\") == false)
                    Directory.CreateDirectory(DataPath + "\\VirtualStack16\\");
                DensityGrid.SaveFile(DataPath + "\\VirtualStack16\\VStack.tif", 16);
            }

        }


        public static void SaveMIPMovie(string DataPath, string TempPath, ProjectionArrayObject DensityGrid, DataEnvironment dataEnvironment)
        {
            string[] OldFrames;
            if (Directory.Exists(TempPath + "\\MIP\\") == false)
                Directory.CreateDirectory(TempPath + "\\MIP\\");
            else
            {
                OldFrames = Directory.GetFiles(TempPath + "\\MIP\\");
                foreach (string F in OldFrames)
                {
                    try
                    {
                        File.Delete(F);
                    }
                    catch { }
                }
            }

            try
            {
                ImageHolder BitmapImage = null;
                ImageViewer.PythonScripting.Projection.MakeMIPMovie3Effect Filter = new ImageViewer.PythonScripting.Projection.MakeMIPMovie3Effect();
                if (DensityGrid.Data != null)
                    BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, null, DensityGrid.Data, DataPath + "\\MIP.avi", TempPath);
                else
                    BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, null, DensityGrid.DataWhole, DataPath + "\\MIP.avi", TempPath);

                BitmapImage.Save(DataPath + "\\Forward1.bmp");
            }
            catch { }

            OldFrames = Directory.GetFiles(TempPath + "\\MIP\\");
            foreach (string F in OldFrames)
            {
                try
                {
                    File.Delete(F);
                }
                catch { }
            }

            Directory.Delete(TempPath + "\\MIP\\");
        }

        public static void SaveCenteringMovie(string DataPath, string TempPath, ProjectionArrayObject DensityGrid, DataEnvironment dataEnvironment)
        {
            ImageHolder BitmapImage = null;
            try
            {
                //Create AVI File From Frames
                //ImageViewer.Filters.Files.AVI.CreateAVIFileFromFramesEffect Filter = new ImageViewer.Filters.Files.AVI.CreateAVIFileFromFramesEffect();
                //#Parameters required: BitmapFilenames as string[], AVI_filename as string
                // BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, null, DataPath + "\\Centering.avi", TempPath + "\\CenterMovie\\Frame_", "jpg");
            }
            catch { }

        }

        public static void GetExampleVisionGateImages(string StorageDrive, DataEnvironment dataEnvironment)
        {
            string VGFile = null;

            //get all the images needed from visiongate recons for comparison
            try
            {
                string dirName = Path.GetFileNameWithoutExtension(dataEnvironment.ExperimentFolder);
                string[] parts = dirName.Split('_');
                string Prefix = parts[0];
                string Year = parts[1].Substring(0, 4);
                string month = parts[1].Substring(4, 2);
                string day = parts[1].Substring(6, 2);

                string basePath = StorageDrive + "\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_cropped_16bit\\";

                Console.WriteLine(basePath);



                if (Directory.Exists(basePath) == true)
                {
                    string[] Files = Directory.GetFiles(basePath, "*.png");
                    Files = EffectHelps.SortNumberedFiles(Files);
                    VGFile = Files[Files.Length / 2];
                }
                else
                {
                    basePath = "Y:\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_cropped_16bit\\";
                    if (Directory.Exists(basePath) == true)
                    {
                        string[] Files = Directory.GetFiles(basePath, "*.png");
                        Files = EffectHelps.SortNumberedFiles(Files);
                        VGFile = Files[Files.Length / 2];
                    }
                }
                if (VGFile != null)
                {
                    try
                    {
                        Bitmap b = new Bitmap(VGFile);
                        b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        b.Save(dataEnvironment.DataOutFolder + "\\VGExample.png");
                    }
                    catch { }
                }
            }
            catch { }
        }
        public static void DoEvents()
        {
            Application.DoEvents();
        }

        public static ImageHolder ClipandFlattenImage(float[, ,] DataCube, double X, double Y, ImageHolder BitmapImage, double[,] BackgroundMask)
        {
            int cellHalf = (int)(DataCube.GetLength(0) / 2);
            Rectangle CellArea = new Rectangle((int)(X - cellHalf), (int)(Y - cellHalf), 2 * cellHalf, 2 * cellHalf);

            Rectangle CellAreaPadded = new Rectangle(CellArea.Location, CellArea.Size);
            CellAreaPadded.Inflate((int)(.4 * CellArea.Width), (int)(.4 * CellArea.Height));

            ImageViewer.Filters.Effects.Flattening.DivideImage.DivideOneImageByAnother(BitmapImage, BackgroundMask, CellAreaPadded);

            double FineCellHalf = cellHalf;
            int FineCellSize = cellHalf * 2;

            CellArea = new Rectangle((int)Math.Truncate(X - FineCellHalf), (int)Math.Truncate(Y - FineCellHalf), FineCellSize, FineCellSize);
            BitmapImage = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(BitmapImage, CellArea);

            BitmapImage.InvertMax();

            return  ImageViewer.Filters.Effects.Flattening.FlattenEdges1D.FlattenImageEdgesConstant(BitmapImage);
        }


        /// <summary>
        /// takes a volume and saves the three axis cross
        /// </summary>
        /// <param name="Filename"></param>
        public static string[] EvaluateRecon(float[, ,] DataWhole, ImageHolder stackImage, out double stackF4, out double onAxisF4)
        {


            int ZSlices = DataWhole.GetLength(0);
            int XSlices = DataWhole.GetLength(1);
            int YSlices = DataWhole.GetLength(2);

            double[,] Slice = new double[XSlices, YSlices];
            for (int y = 0; y < YSlices; y++)
            {
                for (int x = 0; x < XSlices; x++)
                {
                    Slice[x, y] = DataWhole[ZSlices / 2, x, y];
                }
            }
            double[,] SliceX = new double[XSlices, ZSlices];
            for (int z = 0; z < ZSlices; z++)
            {
                for (int x = 0; x < XSlices; x++)
                {
                    SliceX[x, z] = DataWhole[z, x, YSlices / 2];
                }
            }
            double[,] SliceY = new double[YSlices, ZSlices];
            for (int z = 0; z < ZSlices; z++)
            {
                for (int y = 0; y < YSlices; y++)
                {
                    SliceY[y, z] = DataWhole[z, XSlices / 2, y];
                }
            }


            double sumSlice = Slice.SumArray();
            string[] s = new string[6];

            double maxFreqZ;
            double maxFreqX;
            double maxFreqY;
            double[] fZ;
            double[] fY;
            double[] fX;

            try
            {
                fZ = MathFFTHelps.QualityByPowerSpectrum(Slice, out maxFreqZ);
                fY = MathFFTHelps.QualityByPowerSpectrum(SliceX, out maxFreqY);
                fX = MathFFTHelps.QualityByPowerSpectrum(SliceY, out maxFreqX);
                fZ = MathHelpLib.MathArrayHelps.AddToArray(fZ, fY);
                fZ = MathHelpLib.MathArrayHelps.AddToArray(fZ, fX);

                s[0] = "";
                for (int i = 0; i < fZ.Length; i++)
                    s[0] += (fZ[i] / 3d / sumSlice + ",");

                s[1] = (fZ[fZ.Length - 2] / fZ[0]).ToString();
            }
            catch (Exception ex)
            {
                // System.Diagnostics.Debug.Print(ex.Message);

            }

            stackF4 = 1;
            onAxisF4 = 1;
            double FV = 0;
            try
            {
                if (stackImage != null)
                {

                    double[,] stackD = stackImage.ToDataIntensityDouble();
                    FV = FocusValueTool.FocusValueF4(stackD);
                    onAxisF4 = FocusValueTool.FocusValueF4(Slice);
                    //Bitmap b = Slice.MakeBitmap();
                    // int w = b.Width;
                    double[,] Map = MathFFTHelps.CrossCorrelationFFTNormalized(SliceX, stackD);
                    //Bitmap b = stackD.MakeBitmap();
                    // Bitmap b2 = SliceX.MakeBitmap();
                    //  int w = b.Width + b2.Width;
                    double ImageVal = Map.MaxArray();
                    s[3] = ImageVal.ToString();

                    stackF4 = FV;
                    s[4] = FV.ToString();
                }
            }
            catch { }

            double FocusValue = 0;

            try
            {
                FocusValue = FocusValueTool.FocusValueF4(Slice);
                FocusValue += FocusValueTool.FocusValueF4(SliceX);
                FocusValue += FocusValueTool.FocusValueF4(SliceY);
            }
            catch { }

            FocusValue /= 3;
            s[2] = (FocusValue).ToString();

            if (FV != 0)
                s[5] = (FocusValue / FV).ToString();
            return s;
        }



        public static string FormatException(Exception ex)
        {
            string[] parts = ex.StackTrace.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string message = ex.Message.Replace("\n", " ") + " --- " + parts[0] + "---" + parts[1];
            message = message.Replace("<", " ");
            message = message.Replace(">", " ");
            message = message.Replace(",", " ");
            return message;
        }
    }
    public class CellPositions
    {
        public double[] X_Positions;
        public double[] Y_Positions;
        public double[] CellSizes;
        public double[] FocusValue;
        public double[] NumBlobs;
        public double[] CellStain;
        public double[] CenterQuality;
        public CellPositions(int Images)
        {
            X_Positions = new double[Images];
            Y_Positions = new double[Images];
            CellSizes = new double[Images];
            FocusValue = new double[Images];
            NumBlobs = new double[Images];
            CellStain = new double[Images];
            CenterQuality = new double[Images];
        }
    }

}
