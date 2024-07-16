using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MathHelpLib.ProjectionFilters;
using MathHelpLib;
using System.IO;
using MathHelpLib.ImageProcessing;

namespace ImageTestor
{
    static class Program
    {


        private static  void ConvertFile(ImageViewer.DataEnvironment dataEnvironment)
        {
            float[, ,] ReducedDensityGrid = ProjectionArrayObject.LoadMultipleFiles(dataEnvironment.DataOutFolder + "ProjectionObject0.dat", dataEnvironment.DataOutFolder + "ProjectionObject1.dat", 400);

            double CutValue = -500;
            //  if (DensityGrid.Data != null)
            //     ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder.DoRemoveFBPCylinder(ref DensityGrid.Data);
            //else
            CutValue = ImageViewer3D.Filters.Adjustments.RemoveFBPCylinder.DoRemoveFBPCylinder(ref ReducedDensityGrid);

            //if (DensityGrid.Data != null)
            //   ImageViewer3D.PythonScripting.CellCT.NormalizeFBPVolumeEffect.NormalizeFBPVolume(ref DensityGrid.Data, dataEnvironment.AllImages.Count, true);
            //else
            ImageViewer3D.PythonScripting.CellCT.NormalizeFBPVolumeEffect.NormalizeFBPVolume(ref ReducedDensityGrid, dataEnvironment.AllImages.Count, true, (float)CutValue);


            MathHelpsFileLoader.Save_Tiff_Stack(dataEnvironment.DataOutFolder + "ProjectionObject.tif", ReducedDensityGrid);

            if (Directory.Exists(dataEnvironment.TempPath + "MIP\\") == false)
                Directory.CreateDirectory(dataEnvironment.TempPath + "MIP\\");
            else
            {
                string[] OldFrames = Directory.GetFiles(dataEnvironment.TempPath + "MIP\\");
                foreach (string F in OldFrames)
                {
                    try
                    {
                        File.Delete(F);
                    }
                    catch { }
                }
            }



            MathHelpsFileLoader.SaveCross(dataEnvironment.DataOutFolder + "CrossSections.jpg", ReducedDensityGrid);


            ImageViewer.PythonScripting.Projection.MakeMIPMovie3Effect Filter = new ImageViewer.PythonScripting.Projection.MakeMIPMovie3Effect();
            //  if (DensityGrid.Data != null)
            //      BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, DensityGrid.Data, dataEnvironment.DataOutFolder + "MIP.avi", dataEnvironment.TempPath);
            //  else
            ImageHolder BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, null, null, ReducedDensityGrid, dataEnvironment.DataOutFolder + "MIP.avi", dataEnvironment.TempPath);

            BitmapImage.Save(dataEnvironment.DataOutFolder + "Forward1.bmp");
           

        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            string[] Folders = Directory.GetDirectories(@"C:\Development\CellCT\DataIN");//@"V:\ASU_Recon\cct001\201206\01");

            foreach (string dir in Folders)
            {




                string DataIn =dir + "\\"; // @"V:\ASU_Recon\cct001\201206\01\cct001_20120601_111837\";
                string DataOut = dir + "\\data\\"; // @"V:\ASU_Recon\cct001\201206\01\cct001_20120601_111837\data\";
                string TempDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\temp";

                ImageViewer.DataEnvironment dataEnvironment = new ImageViewer.DataEnvironment();
                dataEnvironment.WholeFileList = new List<string>();
                dataEnvironment.WholeFileList.AddRange(new string[500]);
                dataEnvironment.ThreadsRunning = null;
                dataEnvironment.Screen = null;
                dataEnvironment.AllImages = new ImageLibrary(new string[500], true, TempDirectory,false);
                dataEnvironment.NumberOfRunningThreads = 1;
                dataEnvironment.ExperimentFolder = DataIn;
                dataEnvironment.DataOutFolder = DataOut;

                //  dataEnvironment.ProgressLog = new ReplaceChatStringDictionary(Path.GetFileNameWithoutExtension(DataOut), Port);
                if (File.Exists(dataEnvironment.DataOutFolder + "ProjectionObject0.dat"))
                {
                    try
                    {
                        ConvertFile(dataEnvironment);
                    }
                    catch { }
                }
            }

            /*
            double[] Data = new double[512];
            double[] impulse = MathHelpLib.Filtering.Han_RS_RadonFilter(512, 512,256d);
            double R = 100;
            for (double i = 256 - R; i < 256 + R; i++)
            {
                Data[(int)i] = 2 * Math.Sqrt(Math.Pow(R, 2) - Math.Pow((i - 256), 2));
            }
            double sumDAta = Data.Sum();
            double sumImpulse = impulse.Sum();
            double maxDAta = Data.Max();
            double maxImpulse = impulse.Max();

            double test =Math.Sqrt( maxDAta * maxImpulse);
            double[] result = new double[512];

            unsafe
            {
                fixed (double* pData = Data)
                {
                    fixed (double* pImpulse = impulse)
                    {
                        fixed (double* pOut = result)
                        {
                            ImageViewer.PythonScripting.Projection.Convolution1D.ConvoluteChop(pData, Data.Length, pImpulse, impulse.Length, pOut);
                        }
                    }
                }
            }

            test = result[256];
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] > 0)
                    Data[i] = 1;
                else
                    Data[i] = 0;

               
            }

            double[] results2 = new double[512];

            unsafe
            {
                fixed (double* pData = Data)
                {
                    fixed (double* pImpulse = impulse)
                    {
                        fixed (double* pOut = results2)
                        {
                            ImageViewer.PythonScripting.Projection.Convolution1D.ConvoluteChop(pData, Data.Length, pImpulse, impulse.Length, pOut);
                        }
                    }
                }
            }

            for (int i = 0; i < result.Length; i++)
            {
                if (results2[i] > 0)
                    result[i] /= results2[i];
                else
                    result[i] = 0;

            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new formDragGraph());
             * 
             * */
        }
    }
}
