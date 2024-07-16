using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using ImageViewer.PythonScripting;
using ImageViewer;
using MathHelpLib.ProjectionFilters;
using MathHelpLib;
using ImageViewer.Filters;
using ImageViewer.Filters.Blobs;
using System.Threading;
using ImageViewer.PythonScripting.Threads;
using System.IO;
using System.Drawing.Imaging;
using DoRecons.Scripts;

namespace Dehydrate
{
    class DehydrateScript:IScript 
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

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType == "image/jpeg")
                    ici = codec;
            }

            EncoderParameters ep = new EncoderParameters();
            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)50);

            if (ColorImage == true)
            {
                BitmapImage = MathHelpsFileLoader.FixVisionGateImage(BitmapImage, 2);

            }

            BitmapImage.ToEqualIntensityBitmap().Save(dataEnvironment.DataOutFolder + "Dehydrated\\Whole" + string.Format("{0:000}", ImageNumber) + ".jpg", ici, ep);

            ProcessImageFindRough(dataEnvironment, ImageNumber, BitmapImage);

        /*    // try
            {
                //divide off the background curvature
                if (ScriptParams["GlobalFlatten"].ToLower() == "true")
                {
                    Filter = new ImageViewer.Filters.Effects.Flattening.FlattenEdges1DErrorCorrected();
                    BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                }
            }
            // catch { }*/

            return BitmapImage;
        }

        private ImageHolder ProcessImageFindRough(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ImageViewer.Filters.ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            if (FluorImage == null)
            {
                //Iterative Threshold
                Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdEffect();
                object BitmapImage2 = Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

                Filter = new ImageViewer.PythonScripting.Statistics.FastBlackWhiteRatioTool();
                Filter.DoEffect(dataEnvironment, BitmapImage, PassData, BitmapImage2);
                double Percent = (double)Filter.PassData["PercentBlack"];
                dataEnvironment.ProgressLog.AddSafe("PercentBlack", Percent);
                FluorImage = (Percent > .2);
            }
            FluorImage = false;

            int Expander = 2;

            if (FluorImage.Value == true)
            {
                //Invert Contrast
                Filter = new ImageViewer.Filters.Adjustments.InvertEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                Expander = 2;
            }

            if (Expander == 2)
            {
                //reduce size
                Filter = new ImageViewer.Filters.Adjustments.downSampleEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            }
            else if (Expander == 4)
            {
                //reduce size
                Filter = new ImageViewer.Filters.Adjustments.downSampleEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

                //reduce size
                Filter = new ImageViewer.Filters.Adjustments.downSampleEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            }

            object ThreshImage;
            try
            {
                //SIS Threshold
                //Filter =new  ImageViewer.Filters.Thresholding.SISThresholdEffect();
                //BitmapImage =(ImageHolder ) Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                if (FluorImage.Value == true)
                {
                    //Otsu Threshold
                    Filter = new ImageViewer.Filters.Thresholding.OtsuThresholdEffect();
                    ThreshImage = Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                }
                else
                {
                    //Iterative Threshold
                    Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdEffect();
                    ThreshImage = Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                }

                //WaterShed
                Filter = new ImageViewer.Filters.Blobs.WaterShedTool();
                Filter.DoEffect(dataEnvironment, ThreshImage, PassData);
                //Data out of type :
                PassData = Filter.PassData;

                //Get Biggest Blob
                Filter = new ImageViewer.Filters.Blobs.GetBiggestCenterBlob();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, PassData["Blobs"], false);
                //Data out of type :
                PassData = Filter.PassData;
            }
            catch
            { }



            if (PassData.ContainsKey("MaxBlob") == false)
            {

                //Otsu Threshold
                Filter = new ImageViewer.Filters.Thresholding.OtsuThresholdEffect();
                ThreshImage = Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

                //WaterShed
                Filter = new ImageViewer.Filters.Blobs.WaterShedTool();
                Filter.DoEffect(dataEnvironment, ThreshImage, PassData);
                //Data out of type :
                PassData = Filter.PassData;

                //Get Biggest Blob
                Filter = new ImageViewer.Filters.Blobs.GetBiggestCenterBlob();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, PassData["Blobs"], false);
                //Data out of type :
                PassData = Filter.PassData;
            }

            int x;
            int y;
            if (PassData.ContainsKey("MaxBlob") == true)
            {
                try
                {
                    BlobDescription Rect = (BlobDescription)PassData["MaxBlob"];
                    x = Rect.CenterOfGravity.X * Expander;
                    y = Rect.CenterOfGravity.Y * Expander;
                    //print((Rect.BlobBounds.Width*2).ToString() +"," + (2*Rect.BlobBounds.Height).ToString())
                    //print(x.ToString() + "," + y.ToString());

                    tCellSize = tCellSize + Rect.BlobBounds.Width * Expander;
                    tCellSize = tCellSize + Rect.BlobBounds.Height * Expander;
                    ntCellSize++;
                }
                catch
                {
                    x = int.MinValue;
                    y = int.MinValue;
                }
            }
            else
            {
                x = int.MinValue;
                y = int.MinValue;
            }


            try
            {
                CenterCellsTool2Form.StoreBlobLocation(dataEnvironment, ImageNumber, (List<BlobDescription>)PassData["Blobs"], Expander);
            }
            catch
            {
                try
                {
                    CenterCellsTool2Form.StoreBlobLocation(dataEnvironment, ImageNumber, (BlobDescription[])PassData["Blobs"], Expander);
                }
                catch { }
            }

            try
            {
                //Add Array Point
                Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
                //Parameters required: ArrayName as string, datapoint as double
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB", ImageNumber, x);

            }
            catch
            {
                dataEnvironment.ProgressLog.AddSafe("Debug", "Error adding");
            }

            try
            {
                //Add Array Point
                Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
                //Parameters required: ArrayName as string, datapoint as double
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_PositionsB", ImageNumber, y);
            }
            catch
            {
                dataEnvironment.ProgressLog.AddSafe("Debug", "");
            }

            try
            {
                //Add Array Point
                Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
                //Parameters required: ArrayName as string, datapoint as double
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "NumBlobs", ImageNumber, PassData["NumBigBlobs"]);
            }
            catch
            {
                dataEnvironment.ProgressLog.AddSafe("Debug", "");
            }
            // dataEnvironment.ProgressLog.AddSafe("imagenumber", ImageNumber.ToString());
            return BitmapImage;
        }

        private ImageHolder ProcessImageClip(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;

            Rectangle CellArea = new Rectangle((int)Math.Truncate(XData [ImageNumber] - CellHalf), (int)Math.Truncate(YData[ImageNumber] - CellHalf), CellSize, CellSize);


            //Clip Image to New Image
            IEffect Filter = new ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect();
            //Parameters required: Clip Bounds as Rectangle
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, CellArea);
           
            BitmapImage.ToEqualIntensityBitmap().Save(dataEnvironment.DataOutFolder + "Dehydrated\\Center" + string.Format("{0:000}", ImageNumber) + ".png");
         
            return BitmapImage;
        }

        private void PreBatchProcessFindAndClip(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, string> ParamDictionary)
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Divide");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ImageViewer.Filters.ReplaceStringDictionary PassData = GlobalPassData;

            IEffect Filter;
            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB", DataPath + "X_PositionsB", dataEnvironment.AllImages.Count);

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_PositionsB", DataPath + "Y_PositionsB", dataEnvironment.AllImages.Count);

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "NumBlobs", DataPath + "NumBlobs", dataEnvironment.AllImages.Count);

            dataEnvironment.ProgressLog.AddSafe("Position", "starting process Rough");
            //BatchLoopThroughImages(0, dataEnvironment, ImageFileList, ParamDictionary);
            BatchLoopThroughImagesSave(6, dataEnvironment, ImageFileListIn, ScriptParams);
            //print(tCellSize.ToString());
            // print(len(ImageFileList).ToString());
            tCellSize = (tCellSize / len(ImageFileList)) / 2;


            ImageViewer.Filters.CenterCellsTool2Form.PreFitLines(dataEnvironment, "X_PositionsB", "Y_PositionsB", ThreadNumber);

            dataEnvironment.ProgressLog.AddSafe("Position", "starting stationary pixels");
            //Stationary Pixels
            try
            {
                Filter = new ImageViewer.Filters.StationaryPixelsForm();
                //Parameters required: BitmapFiles as string[],  as , X_Positions as double[], Y_Positions as double[], ShowForm as bool, SubtractMethod as string
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData,
                    100, "TopAndBottom", "X_PositionsB", "Y_PositionsB", false, "Divide", ThreadNumber, tCellSize, true, ImageFileList);
                //Data out of type :
                PassData = Filter.PassData;
            }
            catch
            {
                BitmapImage = null;
            }

            CellSize = (int)((int)PassData["MaxCellSize"] * 2.2);
            CellHalf = CellSize / 2;
            CellWanderMargin = CellSize;
            print("Cell Size" + CellSize.ToString());


            dataEnvironment.ProgressLog.AddSafe("Position", "saving stationary pixels");
            if ((dataEnvironment.NumberOfRunningThreads > 1 && ThreadNumber == 1) || (dataEnvironment.NumberOfRunningThreads <= 1 && ThreadNumber == 0))
            {
                try
                {
                    MathHelpsFileLoader.Save_Bitmap(DataPath + "Background.bmp", BitmapImage);
                }
                catch { }

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB", DataPath + "X_PositionsB");

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_PositionsB", DataPath + "Y_PositionsB");

                try
                {
                    MathHelpsFileLoader.Save_Raw(DataPath + "Background.cct", (double[,])PassData["BackgroundMask"]);
                }
                catch { }

                //Read Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.ReadWholeArrayTool();
                //Parameters required: ArrayName as string
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "NumBlobs");
                //Data out of type :
                PassData = Filter.PassData;

                double[] nBlobs = (double[])PassData["WholeArray"];
                double maxBlobs = max(nBlobs);
                dataEnvironment.ProgressLog.AddSafe("NumberOfCells", maxBlobs.ToString());
            }

            //BackgroundMask = (double[,])PassData["BackgroundMask"];

            //Read Whole Array
            Filter = new ImageViewer.PythonScripting.Arrays.ReadWholeArrayTool();
            //Parameters required: ArrayName as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_PositionsB");
            //Data out of type :
            PassData = Filter.PassData;

            XData = (double[])PassData["WholeArray"];

            //Read Whole Array
            Filter = new ImageViewer.PythonScripting.Arrays.ReadWholeArrayTool();
            //Parameters required: ArrayName as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_PositionsB");
            //Data out of type :
            PassData = Filter.PassData;

            YData = (double[])PassData["WholeArray"];

            int XMin = (int)(min(XData) - CellWanderMargin);
            int YMin = (int)(min(YData) - CellWanderMargin);
            int XMax = (int)(max(XData) + CellWanderMargin);
            int YMax = (int)(max(YData) + CellWanderMargin);

            CellWanderArea = new Rectangle(XMin, YMin, XMax - XMin, YMax - YMin);

            print(CellWanderArea);

            dataEnvironment.ProgressLog.AddSafe("Position", "Dividing away background");
            //now do the divide
            BatchLoopThroughImagesSave(3, dataEnvironment, ImageFileListIn, ScriptParams);
            // BatchLoopThroughImages(2, dataEnvironment, ImageFileListIn, ScriptParams );

            dataEnvironment.ProgressLog.AddSafe("Divide", "Divide Succeeded");

        }

       
        //this is called once after the whole set has been processed
        private void PostBatchProcess(DataEnvironment dataEnvironment, List<ImageFile> FirstImageFileList, List<ImageFile> OutFileList, Dictionary<string, string> ParamDictionary)
        {
            dataEnvironment.ProgressLog.AddSafe("Prep", "Post");
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;


          

            dataEnvironment.ProgressLog.AddSafe("Debug", "Reached home" + Thread.CurrentThread.ManagedThreadId.ToString());



            if (ThreadNumber == 0)
            {
                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = null;
                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.MimeType == "image/jpeg")
                        ici = codec;
                }

                EncoderParameters ep = new EncoderParameters();
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)50);

                string[] AllFiles =  Directory.GetFiles(dataEnvironment.ExperimentFolder + "stack\\000\\");

                for (int i = 0; i < AllFiles.Length; i++)
                {
                    BitmapImage = MathHelpsFileLoader.Load_Bitmap(AllFiles[i]);
                    if (ColorImage == true)
                    {
                        BitmapImage = MathHelpsFileLoader.FixVisionGateImage(BitmapImage, 2);
                    }
                    BitmapImage.ToEqualIntensityBitmap().Save(dataEnvironment.DataOutFolder + "Dehydrated\\Stack\\stack" + string.Format("{0:000}", i) + ".jpg", ici, ep);
                }

            }
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

                if (IProcessFunction == 0)
                    image = ProcessImageFindRough(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 3)
                    image = ProcessImageClip(dataEnvironment, imageIndex, image);
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

                if (IProcessFunction == 0)
                    image = ProcessImageFindRough(dataEnvironment, imageIndex, image);
                else if (IProcessFunction == 3)
                    image = ProcessImageClip(dataEnvironment, imageIndex, image);
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

                try
                {
                    Directory.CreateDirectory(dataEnvironment.DataOutFolder + "Dehydrated\\");
                }
                catch { }
                try
                {
                    Directory.CreateDirectory(dataEnvironment.DataOutFolder + "Dehydrated\\Stack\\");
                }
                catch { }
            }

            JoinThreadsTool.JoinThreads(dataEnvironment, "CheckifColor", ThreadNumber);
            //Console.WriteLine("Forcing image to monochrome!!!!!");
            ColorImage = dataEnvironment.ProgressLog["IsColor"].ToString().ToLower() == "true";


            PreBatchProcessFindAndClip(dataEnvironment, ImageFileListIn, ScriptParams);
            
            PostBatchProcess(dataEnvironment, ImageFileListIn, null, ScriptParams);
        }
        public IScript CloneScript()
        {
            return new DehydrateScript();
        }
    }
}
