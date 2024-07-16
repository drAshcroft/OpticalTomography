using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ImageViewer;
using ImageViewer.PythonScripting;
using ImageViewer.Filters;
using ImageViewer.Filters.Blobs;
using System.Threading;
using MathHelpLib;
using System.IO;


namespace Tomographic_Imaging_2
{
    public class ScriptMovies : IScript
    {
        public string GetName()
        {
            return "ScriptCombined";
        }
        private double[,] BackgroundMask;
        private Rectangle CellWanderArea;
        private int CellWanderMargin = 120;
        private double[] XData;
        private double[] YData;
        private int CellSize = 170;
        private int tCellSize = 0;
        private int CellHalf = 85;
        //private double[,] AverageIllumination;
        private ImageViewer.Filters.ReplaceStringDictionary GlobalPassData;
        private double[] CellPosX;
        private double[] CellPosY;

        private bool? FluorImage = null;


        #region global values

        private string DataPath;
        private ImageDisplayer ImageDisp;
        List<ImageFile> ImageFileListIn;
        Dictionary<string, object> ScriptParams;
        // string ImageOutPath;
        // string ImageOutExten;
        // string ImageOutFileName;
        string TempPath;
        string Executable;
        string ExecutablePath;
        string LibraryPath;
        DataEnvironment dataEnvironment;

        bool RunningThreaded;
        int ThreadNumber;

        #endregion

        #region pythonsimulation
        private void print(object message)
        {
            Console.WriteLine(message.ToString());
        }
        private int len(Array array)
        {
            return array.Length;
        }
        private int len(System.Collections.IList array)
        {
            return array.Count;
        }
        private double min(double[] array)
        {
            return array.Min();
        }
        private double max(double[] array)
        {
            return array.Max();
        }
        private double Average(double[] array)
        {

            return array.Average();
        }
        private double Stdev(double[] array)
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
        #endregion

        PhysicalArray DensityGrid = null;
        private ImageHolder ProcessImageMIP(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            if (ImageNumber % 6 == 0)
            {
                Filter = new ImageViewer.PythonScripting.Projection.MakeMIPProjectionEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DensityGrid, (double)ImageNumber / 500d * 2 * Math.PI, TempPath + "MIP\\Frame_" + string.Format("{0:000}.bmp", (int)(ImageNumber / 6)));

            }
            return BitmapImage;
        }
     
        private void PreBatchProcessRecon(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary)
        {
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;
         
            if (Directory.Exists(TempPath + "MIP\\") == false)
                Directory.CreateDirectory(TempPath + "MIP\\");

            BatchLoopThroughImages(5, dataEnvironment, ImageFileList, ParamDictionary);
            dataEnvironment.ProgressLog.AddSafe("MIP", "MIP Images completed");

        }

        //this is called once after the whole set has been processed
        private void PostBatchProcess(DataEnvironment dataEnvironment, List<ImageFile> FirstImageFileList, List<ImageFile> OutFileList, Dictionary<string, object> ParamDictionary)
        {
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            //Join Threads
            Filter = new ImageViewer.PythonScripting.Threads.JoinThreadsTool();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ThreadNumber);

          

            System.Diagnostics.Debug.Print("Reached home" + Thread.CurrentThread.ManagedThreadId.ToString());
            if (ThreadNumber == 0)
            {
                  //Create AVI File From Frames
                 Filter = new ImageViewer.Filters.Files.AVI.CreateAVIFileFromFramesEffect();
                 //#Parameters required: BitmapFilenames as string[], AVI_filename as string
                 BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DataPath + "Centering.avi", TempPath + "CenterMovie\\Frame_", "bmp");

                 EffectHelps.ClearTempFolder(TempPath + "CenterMovie\\");

                 //Create AVI File From Frames
                 Filter = new ImageViewer.Filters.Files.AVI.CreateAVIFileFromFramesEffect();
                 //#Parameters required: BitmapFilenames as string[], AVI_filename as string
                 BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DataPath + "MIP.avi", TempPath + "MIP\\Frame_", "bmp");
                 EffectHelps.ClearTempFolder(TempPath + "MIP\\");
            }
        }

        private object CriticalSection = new object();
        private List<ImageFile> BatchLoopThroughImagesSave(int IProcessFunction, DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary)
        {
            List<ImageFile> FileListOut = new List<ImageFile>();
            int imageIndex = 0;
            for (int CurrentParticle = 0; CurrentParticle < ImageFileList.Count; CurrentParticle++)
            {
                imageIndex = ImageFileList[CurrentParticle].Index;

                ImageHolder image = (ImageHolder)dataEnvironment.AllImages[imageIndex];

             if (IProcessFunction == 5)
                    image = ProcessImageMIP(dataEnvironment, imageIndex, image);

                string fileout = "";// OutDirectory + OutFilename + String.Format("{0:000}", imageIndex) + OutExtension;

                FileListOut.Add(new ImageFile(imageIndex, fileout));

                ImageDisp.DisplayImage(imageIndex, image);
                dataEnvironment.AllImages[imageIndex] = image;

                if (RunningThreaded == false)
                    ImageViewer.PythonScripting.MacroHelper.DoEvents();
                else if (ImageViewer.PythonScripting.MacroHelper.ErrorOnThread == true)
                    Thread.CurrentThread.Abort();

                Thread.Sleep(0);
            }
            return FileListOut;
        }

        private void BatchLoopThroughImages(int IProcessFunction, DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary)
        {
            int imageIndex;
            for (int CurrentParticle = 0; CurrentParticle < ImageFileList.Count; CurrentParticle++)
            {
                //print(ImageFileList[CurrentParticle])
                imageIndex = ImageFileList[CurrentParticle].Index;
                ImageHolder image = (ImageHolder)dataEnvironment.AllImages[imageIndex];

                if (IProcessFunction == 5)
                    image = ProcessImageMIP(dataEnvironment, imageIndex, image);

                ImageDisp.DisplayImage(imageIndex, image);
                if (RunningThreaded == false)
                    ImageViewer.PythonScripting.MacroHelper.DoEvents();
                else if (ImageViewer.PythonScripting.MacroHelper.ErrorOnThread == true)
                    Thread.CurrentThread.Abort();

                Thread.Sleep(0);
            }
        }

        public void RunScript(Dictionary<string, object> Variables)
        {
            ImageFileListIn = (List<ImageFile>)Variables["ImageFileListIn"];
            ScriptParams = (Dictionary<string, object>)Variables["ScriptParams"];

            TempPath = (string)Variables["TempPath"];
            ImageDisp = (ImageDisplayer)Variables["ImageDisp"];
            DataPath = (string)Variables["DataPath"];
            Executable = (string)Variables["Executable"];
            ExecutablePath = (string)Variables["ExecutablePath"];
            LibraryPath = (string)Variables["LibraryPath"];
            dataEnvironment = (DataEnvironment)Variables["dataEnvironment"];
            GlobalPassData = (ReplaceStringDictionary)Variables["GlobalPassData"];
            RunningThreaded = (bool)Variables["RunningThreaded"];
            ThreadNumber = (int)Variables["ThreadNumber"];

            FluorImage = null;

        
                PreBatchProcessRecon(dataEnvironment, ImageFileListIn, ScriptParams);
                PostBatchProcess(dataEnvironment, ImageFileListIn, null, ScriptParams);
            
        }

        public IScript CloneScript()
        {
            return new ScriptMovies();
        }
    }
}

