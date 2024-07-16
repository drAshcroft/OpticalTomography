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
using MathHelpLib.ProjectionFilters;
using ImageViewer.PythonScripting.Threads;

using DoRecons.Scripts;

namespace DoRecons
{
    public class ScriptSamantha2 : IScript
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
        private int ntCellSize = 0;
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
        Dictionary<string, string> ScriptParams;
        // string ImageOutPath;
        // string ImageOutExten;
        // string ImageOutFileName;
        string TempPath;
        string Executable;
        string ExecutablePath;
        string LibraryPath;
        DataEnvironment dataEnvironment;

        // bool RunningThreaded;
        int ThreadNumber;
        bool ColorImage = true;
        #endregion

        #region pythonsimulation
        private void print(object message)
        {
            Console.WriteLine(message.ToString());
            //  dataEnvironment.ProgressLog.AddSafe("Debug", message.ToString());
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
        private double Stdev(double[] array, double Average)
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
        #endregion

        double[] impulse = null;
        ConvolutionMethod DesiredMethod;
        ProjectionArrayObject DensityGrid = null;


        private ImageHolder ProcessImageLoad(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            PassData.AddSafe("Num Loaded Channels", BitmapImage.NChannels);

            //make sure there is only one channel to work with
            if (BitmapImage.NChannels > 1)
            {
                Filter = new ImageViewer.Filters.Adjustments.GrayScaleEffectChannel();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, 1);
            }


            if (ColorImage == true)
            {
                BitmapImage = MathHelpsFileLoader.FixVisionGateImage(BitmapImage, 2);
            }
            
            return BitmapImage;
        }


        private ImageHolder ProcessImageFBP(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;
            dataEnvironment.RunningOnGPU = false;
            double[,] Slice = null;
            if (ConvolutionMethod.Convolution1D == DesiredMethod)
            {
                int NewSize = (int)(1.5 * CellSize);

                
                BitmapImage = (ImageHolder)ConvolutionFilter.DoEffect(dataEnvironment, BitmapImage, PassData, 1, impulse, true, false, 2, 2);
                Slice = (double[,])ConvolutionFilter.PassData["ConvolutionData"];
            }
            else
            {
                //Pad Image to New Image
                Filter = new ImageViewer.PythonScripting.Programming_Tools.PadImageToNewEffect();
                //Parameters required: Clip Bounds as Rectangle
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, new Rectangle(0, 0, 256, 256));
            }

            if (DensityGrid == null)
            {
                Filter = new ImageViewer.PythonScripting.Projection.CreateFilteredBackProjectionEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, (int)(Slice.GetLength(0)), (int)(Slice.GetLength(1)), 2, 2, 1, false, dataEnvironment.RunningOnGPU);

                DensityGrid = (ProjectionArrayObject)Filter.PassData["FBJObject"];
            }

            if (ScriptParams["FBPMedian"].ToLower() == "true")
            {
                Filter = new ImageViewer.Filters.Effects.RankOrder.MedianFilterTool();
                Slice = (double[,])Filter.DoEffect(dataEnvironment, Slice, PassData, 3);
            }

            double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * ImageNumber;
            if (dataEnvironment.RunningOnGPU == false)
            {
                Filter = new ImageViewer.PythonScripting.Projection.InterpolateSliceEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, Slice, DensityGrid, AngleRadians);
                Slice = (double[,])Filter.PassData["ExpandedArray"];
            }
           
            Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, Slice, DensityGrid, AngleRadians);

            return new ImageHolder(Slice);
            // return BitmapImage;
        }

        ImageViewer.PythonScripting.Projection.Convolution1D ConvolutionFilter;
        private void PreBatchProcessRecon(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Recon");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;

            dataEnvironment.ProgressLog.AddSafe("Position", "Getting Impulse");

            impulse = Filtering.GetRealSpaceFilter(ParamDictionary["FBPWindow"], EffectHelps.ConvertToInt(ParamDictionary["FBPResolution"]), EffectHelps.ConvertToInt(ParamDictionary["FBPResolution"]), 1);

            dataEnvironment.ProgressLog.AddSafe("Position", "Doing Convolution");
            DesiredMethod = ConvolutionMethod.Convolution1D;

            ConvolutionFilter = new ImageViewer.PythonScripting.Projection.Convolution1D();

            BatchLoopThroughImages(4, dataEnvironment, ImageFileList, ParamDictionary);


            dataEnvironment.ProgressLog.AddSafe("Recon", "Recon Completed");

            dataEnvironment.ProgressLog.AddSafe("Position", "Creating Mip DIR");

        }

        //this is called once after the whole set has been processed
        private void PostBatchProcess(DataEnvironment dataEnvironment, List<ImageFile> FirstImageFileList, List<ImageFile> OutFileList, Dictionary<string, string> ParamDictionary)
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Post");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;


            //Join Threads
            ImageViewer.PythonScripting.Threads.JoinThreadsTool.JoinThreads(dataEnvironment, "FinishEverything", ThreadNumber);


            dataEnvironment.ProgressLog.AddSafe("Debug", "Reached home" + Thread.CurrentThread.ManagedThreadId.ToString());



            if (ThreadNumber == 0)
            {
                ///the data is still on the card if we have been using the gpu.  Pull the data down and then save it
                if (dataEnvironment.RunningOnGPU == true && DensityGrid.DataWhole != null)
                {
                    DensityGrid.DataWhole = ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect.ReadReconVolume(dataEnvironment);
                }

               
                ////////////////////////////////////////////save the volume////////////////////////////////////////////
                #region SaveVolume
                if (ScriptParams["SaveAsCCT"].ToLower() == "true")
                {
                    DensityGrid.SaveFile(DataPath + "ProjectionObject.cct");
                }
                if (ScriptParams["SaveAsRawDouble"].ToLower() == "true")
                {
                    DensityGrid.SaveFileRaw(DataPath + "ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.Float32);
                }
                if (ScriptParams["SaveAsRawFloat"].ToLower() == "true")
                {
                    DensityGrid.SaveFileRaw(DataPath + "ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.Float32);
                }
                if (ScriptParams["SaveAsRawInt"].ToLower() == "true")
                {
                    DensityGrid.SaveFileRaw(DataPath + "ProjectionObject.raw", ProjectionArrayObject.RawFileTypes.UInt16);
                }
                if (ScriptParams["Save8Bit"].ToLower() == "true")
                {
                    if (Directory.Exists(DataPath + "VirtualStack8\\") == false)
                        Directory.CreateDirectory(DataPath + "VirtualStack8\\");
                    DensityGrid.SaveFile(DataPath + "VirtualStack8\\VStack.tif", 8);
                }
                if (ScriptParams["Save16Bit"].ToLower() == "true")
                {
                    if (Directory.Exists(DataPath + "VirtualStack16\\") == false)
                        Directory.CreateDirectory(DataPath + "VirtualStack16\\");
                    DensityGrid.SaveFile(DataPath + "VirtualStack16\\VStack.tif", 16);
                }

               
                #endregion
               

                #region DoImageComparison

                string VGFile = null;

                //get all the images needed from visiongate recons for comparison
                try
                {
                    Console.WriteLine(dataEnvironment.DataOutFolder);
                    string dirName = Path.GetFileNameWithoutExtension(dataEnvironment.DataOutFolder);
                    Console.WriteLine(dirName);
                    string[] parts = dirName.Split('_');
                    string Prefix = parts[0];
                    string Year = parts[1].Substring(0, 4);
                    string month = parts[1].Substring(4, 2);
                    string day = parts[1].Substring(6, 2);

                    string basePath = "Y:\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_cropped_16bit\\";

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
                            b.Save(dataEnvironment.DataOutFolder + "\\data\\VGExample.png");
                        }
                        catch { }
                    }
                }
                catch { }

                try
                {
                    //todo: cut down the stack based by the imagesize from reconquality check tool.  just need a nice tool
                    if (ScriptParams["CopyStack"].ToLower() == "true")
                    {
                        MathHelpLib.ProjectionFilters.CopyAndCutStackEffect.CopyStack(dataEnvironment.ExperimentFolder + "stack\\000", dataEnvironment.DataOutFolder + "stack", true, dataEnvironment.ProgressLog["IsColor"].ToString().ToLower() == "true");
                    }
                }
                catch { }


                //do the image quality tests
                if (ScriptParams["DoConvolutionQuality"].ToLower() == "true")
                {
                    try
                    {
                        dataEnvironment.ProgressLog.AddSafe("Position", "ImageQuality");
                        MathHelpLib.ProjectionFilters.ReconQualityCheckTool.CompareProjection(DensityGrid.DataWhole, dataEnvironment.ExperimentFolder, PassData);
                    }
                    catch { }


                }

                foreach (KeyValuePair<string, object> kvp in PassData)
                {
                    dataEnvironment.ProgressLog.AddSafe(kvp.Key, kvp.Value.ToString());
                }

                dataEnvironment.ProgressLog.AddSafe("ImageType", Path.GetExtension(dataEnvironment.WholeFileList[0]));

                #endregion
            }

            ///the resources should be cleaned up on the calling thread
            ConvolutionFilter.Dispose();
            ImageViewer.PythonScripting.Projection.DoSliceBackProjectionEffect.Dispose(dataEnvironment);
        }

        #region Batchloop
        private object CriticalSection = new object();
        private List<ImageFile> BatchLoopThroughImagesSave(int IProcessFunction, DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            List<ImageFile> FileListOut = new List<ImageFile>();
            int imageIndex = 0;
            for (int CurrentParticle = 0; CurrentParticle < ImageFileList.Count; CurrentParticle++)
            {
                imageIndex = ImageFileList[CurrentParticle].Index;

                ImageHolder image = (ImageHolder)dataEnvironment.AllImages[imageIndex];

              
                if (IProcessFunction == 4)
                    image = ProcessImageFBP(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 6)
                    image = ProcessImageLoad(dataEnvironment, imageIndex, image);
                

                string fileout = "";// OutDirectory + OutFilename + String.Format("{0:000}", imageIndex) + OutExtension;

                FileListOut.Add(new ImageFile(imageIndex, fileout));

                ImageDisp.DisplayImage(imageIndex, image);
                dataEnvironment.AllImages[imageIndex] = image;

                if (dataEnvironment.RunningThreaded == false)
                    ImageViewer.PythonScripting.MacroHelper.DoEvents();
                else if (ImageViewer.PythonScripting.MacroHelper.ErrorOnThread == true)
                    Thread.CurrentThread.Abort();

                Thread.Sleep(0);
            }
            return FileListOut;
        }

        private void BatchLoopThroughImages(int IProcessFunction, DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            int imageIndex;
            for (int CurrentParticle = 0; CurrentParticle < ImageFileList.Count; CurrentParticle++)
            {
                //print(ImageFileList[CurrentParticle])
                imageIndex = ImageFileList[CurrentParticle].Index;
                ImageHolder image = (ImageHolder)dataEnvironment.AllImages[imageIndex];

                if (IProcessFunction == 4)
                    image = ProcessImageFBP(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 6)
                    image = ProcessImageLoad(dataEnvironment, imageIndex, image);

                ImageDisp.DisplayImage(imageIndex, image);
                if (dataEnvironment.RunningThreaded == false)
                    ImageViewer.PythonScripting.MacroHelper.DoEvents();
                else if (ImageViewer.PythonScripting.MacroHelper.ErrorOnThread == true)
                    Thread.CurrentThread.Abort();

                Thread.Sleep(0);
            }
        }
        #endregion

        public void RunScript(Dictionary<string, object> Variables)
        {
            ImageFileListIn = (List<ImageFile>)Variables["ImageFileListIn"];
            ScriptParams = (Dictionary<string, string>)Variables["ScriptParams"];

            TempPath = (string)Variables["TempPath"];
            ImageDisp = (ImageDisplayer)Variables["ImageDisp"];
            DataPath = (string)Variables["DataPath"];
            Executable = (string)Variables["Executable"];
            ExecutablePath = (string)Variables["ExecutablePath"];
            LibraryPath = (string)Variables["LibraryPath"];
            dataEnvironment = (DataEnvironment)Variables["dataEnvironment"];
            GlobalPassData = (ReplaceStringDictionary)Variables["GlobalPassData"];
            //RunningThreaded = (bool)Variables["RunningThreaded"];
            ThreadNumber = (int)Variables["ThreadNumber"];

            FluorImage = null;

            if (ThreadNumber == 0)
            {
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
            }

            JoinThreadsTool.JoinThreads(dataEnvironment, "CheckifColor", ThreadNumber);
            //Console.WriteLine("Forcing image to monochrome!!!!!");
            ColorImage = dataEnvironment.ProgressLog["IsColor"].ToString().ToLower() == "true";
            ColorImage = false;

            //load the modified images
            BatchLoopThroughImagesSave(6, dataEnvironment, ImageFileListIn, ScriptParams);
           

            PreBatchProcessRecon(dataEnvironment, ImageFileListIn, ScriptParams);
            PostBatchProcess(dataEnvironment, ImageFileListIn, null, ScriptParams);
        }

        public IScript CloneScript()
        {
            return new ScriptSamantha2();
        }
    }
}

