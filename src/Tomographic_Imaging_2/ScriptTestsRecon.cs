using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ImageViewer;
using ImageViewer.PythonScripting;
using ImageViewer.Filters;
using ImageViewer.Filters.Blobs;
using MathHelpLib;

namespace Tomographic_Imaging_2
{
    public class ScriptTestsRecon
    {
       
        private ImageViewer.Filters.ReplaceStringDictionary GlobalPassData;
        private string DataPath;
        private ImageDisplayer ImageDisp;
     


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



        private ProjectionObject DoRecon(List<aProjectionSlice> Slices, double[] impulse, Dictionary<string, object> ParamDictionary)
        {
            aProjectionSlice apS = Slices[0];
            int Width = apS.Projection.GetLength(Axis.XAxis);
            int Height = apS.Projection.GetLength(Axis.YAxis);

            int PaddedSize = 0;
            if (Width > Height)
            {
                PaddedSize = (int)MathHelps.NearestPowerOf2(Width);
            }
            else
            {
                PaddedSize = (int)MathHelps.NearestPowerOf2(Height);
            }

            ProjectionObject projectionObject = ScriptingInterface.scriptingInterface.CreateProjectionObject(apS);
            projectionObject.DoBackProjection_ThreadedMediumMemory(PaddedSize, Slices.ToArray(), impulse, ConvolutionMethod.Convolution1D);
            print("Done Projecting");
            return projectionObject;
        }

        //this is called once before all the images are looped through
        //it is good for opening array lists and log files
        private List<aProjectionSlice> LoadSlices(List<ImageFile> ImageFileList, Dictionary<string, object> ParamDictionary)
        {
            double SliceDegree = 360.0 / len(ImageFileList);
            List<aProjectionSlice> Slices = new List<aProjectionSlice>();
            ImageHolder testImage = ImagingTools.Load_Bitmap(ImageFileList[0].Filename);
            double dWidth = testImage.Width;
            double dHeight = testImage.Height;
            ImageDisp.DisplayImage(0, testImage);
            for (int CurrentImage = 0; CurrentImage < ImageFileList.Count; CurrentImage++)
            {
                print(ImageFileList[CurrentImage].Filename);
                ProjectionSliceFile psf = new ProjectionSliceFile();
                psf.PersistDataInMemory = Wow.Is64BitProcess;
                psf.LoadFile(ImageFileList[CurrentImage].Filename, SliceDegree * CurrentImage, 2.0 * dHeight / dWidth, 2.0, false);
                print(CurrentImage);
                ImageDisp.DisplayImage(0, psf.Projection.MakeBitmap());
                Slices.Add(psf);
            }
            return Slices;
        }

        //this is the main section.  It calls process, loops through all the images, and then calls 
        //post process.  
        public void RunRecon(Dictionary<string, object> Variables)
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

            List<aProjectionSlice> SliceList = LoadSlices(ImageFileListIn, ScriptParams);

            double[] impulse = Filtering.SincRealSpaceFilter(512, 1);

            ProjectionObject  projectionObject = DoRecon(SliceList, impulse, ScriptParams);

            print("doSave");
            projectionObject.SaveDensityData(DataPath + "ProjectionObject.raw");

            projectionObject.DoMIPProjection_OneSlice(0).Save(DataPath + "Forward1.bmp");
        }
    }
}
