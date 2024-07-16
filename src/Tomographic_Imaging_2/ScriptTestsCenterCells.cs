using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ImageViewer;
using ImageViewer.PythonScripting;
using ImageViewer.Filters;
using ImageViewer.Filters.Blobs;

namespace Tomographic_Imaging_2
{
    public class ScriptTestsCenterCellls
    {
     
        private int CellSize = 170;
        private int tCellSize = 0;
        private int CellHalf = 85;
        private ImageViewer.Filters.ReplaceStringDictionary GlobalPassData;
        private string DataPath;
        private ImageDisplayer ImageDisp;
        private double[] CellPosX;
        private double[] CellPosY;


        List<ImageFile> ImageFileListIn;
        Dictionary<string, object> ScriptParams;
        string ImageOutPath;
        string ImageOutExten;
        string ImageOutFileName;
        string TempPath;
        string Executable;
        string ExecutablePath;
        string LibraryPath;
        DataEnvironment dataEnvironment;
        List<ImageHolder> AllImages;
        bool UseGlobalImageList;
        bool RunningThreaded;
        int ThisThreadId;

        int MasterThreadID;

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



        private ImageHolder ProcessImageClip(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;

            Rectangle CellArea = new Rectangle((int)Math.Truncate(CellPosX[ImageNumber] - CellHalf), (int)Math.Truncate(CellPosY[ImageNumber] - CellHalf), CellSize, CellSize);

            //Clip Image to New Image
            IEffect Filter = new ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect();
            //Parameters required: Clip Bounds as Rectangle
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, CellArea);

            //Invert Contrast
            Filter = new ImageViewer.Filters.Adjustments.InvertEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

            if (CellSize > 200)
            {
                //Down Sample Image
                Filter = new ImageViewer.Filters.Adjustments.downSampleEffect();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            }

            //Focus Value of Image
            Filter = new ImageViewer.PythonScripting.Statistics.FocusValueTool();
            //Parameters required: Image as image
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, BitmapImage);
            PassData = Filter.PassData;

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValue", ImageNumber, PassData["FocusValue"]);

            return BitmapImage;
        }
        private ImageHolder ProcessImageFind(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage)
        {
            ReplaceStringDictionary PassData = GlobalPassData;

            //Jitter
            IEffect Filter = new ImageViewer.Filters.Effects.Artistic.JitterEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

            //Iterative Threshold
            Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);

            //WaterShed
            Filter = new ImageViewer.Filters.Blobs.WaterShedTool();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
            //Data out of type :
            PassData = Filter.PassData;

            //Get Biggest Blob
            Filter = new ImageViewer.Filters.Blobs.GetBiggestBlob();
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, PassData["Blobs"], true);
            //Data out of type :
            PassData = Filter.PassData;

            BlobDescription Rect = (BlobDescription)PassData["MaxBlob"];

            int x = Rect.CenterOfGravity.X;
            int y = Rect.CenterOfGravity.Y;
            print(x.ToString() + "," + y.ToString());

            tCellSize = tCellSize + Rect.BlobBounds.Width;
            tCellSize = tCellSize + Rect.BlobBounds.Height;

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_Positions", ImageNumber, x);

            //Add Array Point
            Filter = new ImageViewer.PythonScripting.Arrays.AddPointArrayTool();
            //Parameters required: ArrayName as string, datapoint as double
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_Positions", ImageNumber, y);

            return BitmapImage;
        }
        //this is called once before all the images are looped through
        //it is good for opening array lists and log files
        private void PreBatchProcess(DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary)
        {
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;

            //Create Global Array
            IEffect Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_Positions", DataPath + "X_Positions");

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_Positions", DataPath + "Y_Positions");

            BatchLoopThroughImages(1, dataEnvironment, ImageFileList, ParamDictionary);


            print(tCellSize.ToString());
            print(len(ImageFileList).ToString());
            tCellSize = (tCellSize / len(ImageFileList)) / 2;
            CellSize = tCellSize * 2;
            CellHalf = CellSize / 2;
            print("Cell Size" + CellSize.ToString());

            //Center Cells
            Filter = new ImageViewer.Filters.CenterCellsTool2Form();
            //Parameters required: Bitmap_Filenames as string[], X_Positions as int[], Y_Positions as int[], SmoothingTypeX as string, X_Smooth_Param as int, SmootingTypeY as string, Y_Smooth_Param as int, ShowForm as string, CutSize as Size, OptionalOutputDir as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, ImageFileList, 100, "X_Positions", "Y_Positions", "MovingAverage", 5, "MovingAverage", 5, "NoShow", new Size(170, 170), RunningThreaded, ThisThreadId);
            //Data out of type :
            PassData = Filter.PassData;

            if (MasterThreadID == ThisThreadId)
            {
                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "X_Positions", DataPath + "X_Positions");

                //Save Whole Array
                Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "Y_Positions", DataPath + "Y_Positions");
            }
            CellPosX = (double[])PassData["CorrectedX"];

            CellPosY = (double[])PassData["CorrectedY"];

            //Create Global Array
            Filter = new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool();
            //Parameters required: Array_Name as string, Array_Filename as string
            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValue", DataPath + "FocusValue");

            //Macro Add Point
            //return 0;
        }

        //this is called once after the whole set has been processed
        private void PostBatchProcess(DataEnvironment dataEnvironment, List<ImageFile> FirstImageFileList, List<ImageFile> OutFileList, Dictionary<string, object> ParamDictionary)
        {
            //this is just to make the script functions easier, if needed convert to an image from ImageFileList
            ImageHolder BitmapImage = new ImageHolder(new Bitmap(10, 10));
            ReplaceStringDictionary PassData = GlobalPassData;

            //Create AVI File From Frames
          //  IEffect Filter = new ImageViewer.Filters.Files.AVI.CreateAVIFileFromFramesEffect();
            //Parameters required: BitmapFilenames as string[], AVI_filename as string
//            BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, OutFileList, DataPath + "Centering.avi", RunningThreaded, ThisThreadId);

            if (MasterThreadID == ThisThreadId)
            {
                ImageHolder image = ImagingTools.Load_Bitmap(OutFileList[0].Filename);
                ImagingTools.Save_Bitmap(DataPath + "FirstPP.bmp", image);

                image = ImagingTools.Load_Bitmap(OutFileList[len(OutFileList) / 2].Filename);
                ImagingTools.Save_Bitmap(DataPath + "HalfPP.bmp", image);

                image = ImagingTools.Load_Bitmap(OutFileList[len(OutFileList) - 1].Filename);
                ImagingTools.Save_Bitmap(DataPath + "lastPP.bmp", image);

                //Save Whole Array
                IEffect Filter = new ImageViewer.PythonScripting.Arrays.SaveWholeArrayTool();
                //Parameters required:  as 
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, "FocusValue", DataPath + "FocusValue");
            }
            //return 0;
        }

        private List<ImageFile> BatchLoopThroughImagesSave(int IProcessFunction, DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary, string OutDirectory, string OutFilename, string OutExtension)
        {
            List<ImageFile> FileListOut = new List<ImageFile>();
            int imageIndex;
            for (int CurrentParticle = 0; CurrentParticle < ImageFileList.Count; CurrentParticle++)
            {
                //print(ImageFileList[CurrentParticle])
                imageIndex = ImageFileList[CurrentParticle].Index;
                ImageHolder image;
                if (UseGlobalImageList == false)
                    image = ImagingTools.Load_Bitmap(ImageFileList[CurrentParticle].Filename);
                else
                    image = AllImages[imageIndex];

                if (IProcessFunction == 0)
                    image = ProcessImageClip(dataEnvironment, imageIndex, image);
                else
                    image = ProcessImageFind(dataEnvironment, imageIndex, image);

                ImageViewer.PythonScripting.MacroHelper.DoEvents();
                string fileout = OutDirectory + OutFilename + String.Format("{0:000}", imageIndex) + OutExtension;
                //print(fileout)
                FileListOut.Add(new ImageFile(imageIndex, fileout));// .append(fileout);
                if (UseGlobalImageList == false)
                    ImagingTools.Save_Bitmap(fileout, image);
                else
                {
                    AllImages.RemoveAt(CurrentParticle);
                    AllImages.Insert(CurrentParticle, image);
                }
                ImageDisp.DisplayImage(imageIndex, image);
                ImageViewer.PythonScripting.MacroHelper.DoEvents();
            }
            return FileListOut;
        }

        private void  BatchLoopThroughImages(int IProcessFunction, DataEnvironment dataEnvironment, List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary)
        {
            int imageIndex;
            for (int CurrentParticle = 0; CurrentParticle < ImageFileList.Count; CurrentParticle++)
            {
                imageIndex = ImageFileList[CurrentParticle].Index;
                ImageHolder image;
                if (UseGlobalImageList == false)
                    image = ImagingTools.Load_Bitmap(ImageFileList[CurrentParticle].Filename);
                else
                    image = AllImages[imageIndex];

                if (IProcessFunction == 0)
                    image = ProcessImageClip(dataEnvironment, imageIndex, image);
                else
                    image = ProcessImageFind(dataEnvironment, imageIndex, image);
                
                ImageDisp.DisplayImage(imageIndex, image);
                ImageViewer.PythonScripting.MacroHelper.DoEvents();
            }
        }
        //this is the main section.  It calls process, loops through all the images, and then calls 
        //post process.  



        public void CenterCells(Dictionary<string, object> Variables)
        {
            ImageFileListIn = (List<ImageFile>)Variables["ImageFileListIn"];
            ScriptParams = (Dictionary<string, object>)Variables["ScriptParams"];
            ImageOutPath = (string)Variables["ImageOutPath"];
            ImageOutExten = (string)Variables["ImageOutExten"];
            ImageOutFileName = (string)Variables["ImageOutFileName"];
            TempPath = (string)Variables["TempPath"];
            ImageDisp = (ImageDisplayer)Variables["ImageDisp"];
            DataPath = (string)Variables["DataPath"];
            Executable = (string)Variables["Executable"];
            ExecutablePath = (string)Variables["ExecutablePath"];
            LibraryPath = (string)Variables["LibraryPath"];
            dataEnvironment = (DataEnvironment)Variables["dataEnvironment"];
            GlobalPassData = (ReplaceStringDictionary)Variables["GlobalPassData"];

            RunningThreaded = (bool)Variables["RunningThreaded"];
            ThisThreadId = (int)Variables["ThisThreadId"];
            MasterThreadID = (int)Variables["MasterThreadID"];

            AllImages = (List<ImageHolder>)Variables["AllImages"];
            UseGlobalImageList = (bool)Variables["UseGlobalImageList"];

            PreBatchProcess(dataEnvironment, ImageFileListIn, ScriptParams);

            List<ImageFile> OutFileList = BatchLoopThroughImagesSave(0, dataEnvironment, ImageFileListIn, ScriptParams, ImageOutPath, ImageOutFileName, ImageOutExten);
            //OutFileList = BatchLoopThroughImagesSave(ProcessImageClip,dataEnvironment, ImageFileListIn, ScriptParams, ImageOutPath, ImageOutFileName, ImageOutExten)

            PostBatchProcess(dataEnvironment, ImageFileListIn, OutFileList, ScriptParams);
        }
    }
}
