using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MathHelpLib;
using WeifenLuo.WinFormsUI.Docking;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using MathHelpLib.ImageProcessing;
using MathHelpLib.DrawingAndGraphing;
//using AForge.Imaging;
using Logger;
using System.Reflection;
using Tomographic_Imaging_2.ImageManipulation;
using ImageViewer;
using MathHelpLib.ImageProcessing;

namespace Tomographic_Imaging_2
{
    public partial class TomographicMainForm : Form
    {
        public TomographicMainForm()
        {
            InitializeComponent();
            sMainForm = this;
        }

        private static ScriptingInterface sScriptingInterface;
        private static TomographicMainForm sMainForm;

        public static bool isInitialized
        {
            get
            {
                return (sMainForm != null);
            }
        }

        public static ScriptingInterface GetScriptingInterface()
        {
            return sScriptingInterface;
        }

        public static void ShowDockContent(DockContent form)
        {
            form.Show(sMainForm.dockPanel1, DockState.Float);
        }
        public static void ShowDockContent(DockContent form, DockState Location)
        {
            form.Show(sMainForm.dockPanel1, Location);
        }

        private void TomographicMainForm_Load(object sender, EventArgs e)
        {

            //if this is the start up form create scripting interface, otherwise 
            //just use the already created interface.
            if (ScriptingInterface.scriptingInterface != null)
                sScriptingInterface = ScriptingInterface.scriptingInterface;
            else
            {
                sScriptingInterface = new ScriptingInterface();
                sScriptingInterface.MainForm = this;
            }

            //create the GUI
            SimulationControls simControls = new SimulationControls();
            simControls.Show(dockPanel1, DockState.DockRightAutoHide);

            TextEditorForm tef = new TextEditorForm();
            tef.Show(dockPanel1, DockState.Document);

            VariableWindowForm varWin = new VariableWindowForm();
            varWin.Show(dockPanel1, DockState.DockRight);
            
        }

        #region Test_Runs
        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #region Run_Reconstructions

        private void run3DReconstructionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectionObject po;
            List<aProjectionSlice> Slices = (List<aProjectionSlice>)ScriptingInterface.scriptingInterface.VisibleVariables["ProjectionSlices"];
            if (Slices[0].ProjectionRank == PhysicalArrayRank.Array2D)
            {
                po = ScriptingInterface.scriptingInterface.CreateProjectionObject(256, 2, 2, 2);
            }
            else
            {
                po = ScriptingInterface.scriptingInterface.CreateProjectionObject(256, 2, 2);
            }

            double[,] impulse = Filtering.SincRealSpaceFilter2D(256, 1);
            ScriptingInterface.scriptingInterface.MakeVariableVisible("ProjectionOject", po);
            ScriptingInterface.scriptingInterface.MakeVariableVisible("impulse", impulse);

            aProjectionSlice apS = Slices[0];
            int Width = apS.Projection.GetLength(Axis.XAxis);
            int Height = apS.Projection.GetLength(Axis.YAxis);
            int PaddedSize;
            if (Width > Height)
                PaddedSize = (int)MathHelps.NearestPowerOf2(2 * Width);
            else
                PaddedSize = (int)MathHelps.NearestPowerOf2(2 * Height);

            po.DoBackProjection_ThreadedMediumMemory(PaddedSize, Slices.ToArray(), impulse, ConvolutionMethod.ConvolutionRealSpaceFilterFFT);

            ScriptingInterface.scriptingInterface.CreateGraph("ProjectionOutput", po.ProjectionData);
        }
        #endregion

        private void loadImageAsSliceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string Filename = "C:\\Users\\bashcAdmin\\Documents\\Visual Studio 2008\\Projects\\Flo1CorrectedPP_cct001_20100511_150225\\corrpp_blue005.png";
            DialogResult ret = openFileDialog1.ShowDialog();
            string Filename = openFileDialog1.FileName;
            if (ret == DialogResult.OK)
            {

                ProjectionSliceFile psf = new ProjectionSliceFile();
                psf.PersistDataInMemory = true;
                psf.LoadFile(Filename, 0, 2, 2, true);
                ScriptingInterface.scriptingInterface.CreateGraph("ImageTest", psf.Projection);

                double[,] impulse = Filtering.SincRealSpaceFilter2D(512, 1);
                ////////////////////////////Test Realspace 2D convolution/////////////////////////////////////

                psf.DoBackProjection(impulse, 512, ConvolutionMethod.ConvolutionRealSpaceFilterFFT);
                ScriptingInterface.scriptingInterface.CreateGraph("Backprojection", psf.BackProjection);
            }
        }


        #region ImagesForReconstruction
        private void loadImagesForReconstructionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                string[] FileNames = openFileDialog1.FileNames;
                double SliceDegree = 360 / (double)FileNames.Length;
                Queue<aProjectionSlice> SliceQueue = new Queue<aProjectionSlice>();
                Bitmap test = new Bitmap(FileNames[0]);
                for (int i = 0; i < FileNames.Length; i++)
                {
                    ProjectionSliceFile psf = new ProjectionSliceFile();
                    psf.PersistDataInMemory = false;
                    psf.LoadFile(FileNames[i], SliceDegree * i, 2 * 1.42 * test.Height / test.Width, 2 * 1.42, true);

                    ScriptingInterface.scriptingInterface.CreateGraph("ImageTest", psf.Projection);
                    ScriptingInterface.scriptingInterface.SetProjectionSlice(psf);
                    SliceQueue.Enqueue(psf);
                }

                ProjectionObject po = ScriptingInterface.scriptingInterface.CreateProjectionObject(170, 2, 2, 2);

                double[] impulse = Filtering.Ramachandran_Lakshminarayanan_RS_RadonFilter (256, 2.0 / 256.0);

                ScriptingInterface.scriptingInterface.MakeVariableVisible("ProjectionOject", po);
                List<aProjectionSlice> Slices = (List<aProjectionSlice>)ScriptingInterface.scriptingInterface.VisibleVariables["ProjectionSlices"];

                int PaddedSize;
                if (test.Width > test.Height)
                    PaddedSize = (int)MathHelps.NearestPowerOf2(2 * test.Width);
                else
                    PaddedSize = (int)MathHelps.NearestPowerOf2(2 * test.Height);


                Thread[] BackProject = new Thread[Environment.ProcessorCount];
                for (int i = 0; i < Environment.ProcessorCount; i++)
                {
                    BackProject[i] = new Thread(delegate()
                        {
                            while (SliceQueue.Count > 0)
                            {
                                aProjectionSlice ps = SliceQueue.Dequeue();
                                ps.DoBackProjection(impulse, PaddedSize, ConvolutionMethod.ConvolutionRealSpaceFilterFFT);
                                po.AddSlice(ps);
                            }
                        }
                    );
                    BackProject[i].Start();
                }

                for (int i = 0; i < BackProject.Length; i++)
                {
                    BackProject[i].Join();
                }

                po.DoBackProjection_AllSlices(PaddedSize, Axis2D.XAxis, true, ConvolutionMethod.ConvolutionRealSpaceFilterFFT);
                ScriptingInterface.scriptingInterface.CreateGraph("ProjectionOutput", po.ProjectionData);
            }
        }
        private void loadImagesForReconstructionNotThreadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                ProjectionObject po = ScriptingInterface.scriptingInterface.CreateProjectionObject(170, 2, 2, 2);
                string[] FileNames = openFileDialog1.FileNames;
                double SliceDegree = 360 / (double)FileNames.Length;
                //double[,] impulse = Filtering.RectangularRealSpaceFilter2D(512, 1000, 1);
                double[,] impulse = Filtering.SincRealSpaceFilter2D(512, 1);
                //double[] impulse = Filtering.Ramachandran_Lakshminarayanan_RealSpacefilter(512, 1); 
                Bitmap test = new Bitmap(FileNames[0]);
                int PaddedSize;
                if (test.Width > test.Height)
                    PaddedSize = (int)MathHelps.NearestPowerOf2(2 * test.Width);
                else
                    PaddedSize = (int)MathHelps.NearestPowerOf2(2 * test.Height);

                ConvolutionMethod convolutionMethod = ConvolutionMethod.ConvolutionRealSpaceFilterFFT;
                for (int i = 0; i < FileNames.Length; i++)
                {
                    ProjectionSliceFile psf = new ProjectionSliceFile();
                    psf.PersistDataInMemory = false;
                    psf.LoadFile(FileNames[i], SliceDegree * i, 2 *  test.Height / test.Width, 2 , false);

                    ScriptingInterface.scriptingInterface.CreateGraph("ImageTest", psf.Projection);
                    ScriptingInterface.scriptingInterface.SetProjectionSlice(psf);
                    ScriptingInterface.scriptingInterface.CreateGraph("BackProjection", psf.DoBackProjection(impulse, PaddedSize, convolutionMethod  ));
                    po.AddSlice(psf);
                }

                ScriptingInterface.scriptingInterface.MakeVariableVisible("ProjectionOject", po);

                po.DoBackProjection_AllSlices(PaddedSize, Axis2D.XAxis, false, convolutionMethod );
                ScriptingInterface.scriptingInterface.CreateGraph("ProjectionOutput", po.ProjectionData);
            }
        }
       
     
        private void loadImagesForReconstruction2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                string[] FileNames = openFileDialog1.FileNames;
                FileNames = MathStringHelps.SortNumberedFiles(FileNames);
                double SliceDegree = 360d / (double)FileNames.Length;// *10.5;

                ProjectionObject po = ScriptingInterface.scriptingInterface.CreateProjectionObject(170, 2, 2, 2);

                // double[] impulse = Filtering.GuassianApproximation_SeperableRealSpaceFilter(512,1d/ 100d);
                double[] impulse = Filtering.Ramachandran_Lakshminarayanan_RS_RadonFilter(512, 1);
               // double[] impulse = Filtering.Shepp_Logan_RealSpacefilter(512, 1d/100d);

               // double[] impulse = Filtering.Sinc_RS_RadonFilter (512,512, 1);
               // double[,] impulse = Filtering.SincRealSpaceFilter2D(512, 1);

               // double[] impulse = Filtering.Sinc_RS_RadonFilter (512,512, 1);
                //double[,] impulse = Filtering.SincRealSpaceFilter2D(512, 1);


                ScriptingInterface.scriptingInterface.CreateGraph("Impulse Function", impulse, "X", "Intensity");

                ScriptingInterface.scriptingInterface.MakeVariableVisible("ProjectionOject", po);

                Bitmap b = new Bitmap(FileNames[0]);
                double dWidth = b.Width;
                double dHeight = b.Height;
                b = null;
                aProjectionSlice[] Slices = new aProjectionSlice[FileNames.Length];
                string TempFilePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\";
               
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                //double MaxValue = 0;
                List<aProjectionSlice> SliceList = new List<aProjectionSlice>();
                for (int i = 0; i < FileNames.Length; i+=1)
                {
                    ProjectionSliceFile psf = new ProjectionSliceFile();
                    psf.PersistDataInMemory = false;
                    psf.LoadFile(FileNames[i], SliceDegree * i, 2d * dHeight / dWidth, 2d,false);
                  
                    SliceList.Add( psf);
                    ScriptingInterface.scriptingInterface.CreateGraph("ImageTest", psf.Projection);
                }
                Slices = SliceList.ToArray();

                aProjectionSlice apS = Slices[0];
                int Width = apS.Projection.GetLength(Axis.XAxis);
                int Height = apS.Projection.GetLength(Axis.YAxis);
                int PaddedSize;
                if (Width > Height)
                    PaddedSize = (int)MathHelps.NearestPowerOf2(Width);
                else
                    PaddedSize = (int)MathHelps.NearestPowerOf2(Height);


                po.DoBackProjection_ThreadedMediumMemory(PaddedSize, Slices, impulse, ConvolutionMethod.ConvolutionRealSpaceGPU );

               // po.ProjectionData.NormalizeSlices(Axis.ZAxis);
                long runtime = sw.ElapsedMilliseconds;
                LoggerForm.LogMessage("Completed in " + runtime.ToString() + "ms or " + ((double)runtime / 1000d).ToString());
                sw.Stop();

                ScriptingInterface.scriptingInterface.CreateGraph("ProjectionOutput", po.ProjectionData);
            }
        }

        private void loadImagesForReconstructionChoseAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                string[] FileNames = openFileDialog1.FileNames;
                FileNames = MathStringHelps.SortNumberedFiles(FileNames);

                ImageSelector selector = new ImageSelector();
                selector.SetFilenames(FileNames);
                Bitmap b3 = MathImageHelps.LoadBitmap(FileNames[0], false);
                selector.SetBitmap(b3);
                selector.WaitForInput("Please select the interesting projection area", new ImageViewer.Tools.ROITool());
                ImageViewer.Selections.ROISelection ROI = (ImageViewer.Selections.ROISelection)selector.ActiveSelection;

                int CenterPixel = (int)Math.Truncate((double)b3.Height / 2d + .5);
                int ROIWidth = (int)Math.Truncate((double)ROI.SelectionBounds.Height / 2d);
                Rectangle ClipRectangle = Rectangle.FromLTRB(ROI.SelectionBounds.Left, CenterPixel - ROIWidth, ROI.SelectionBounds.Right, CenterPixel + ROIWidth);
                double SliceDegree = 360d / (double)FileNames.Length;

                ProjectionObject po = ScriptingInterface.scriptingInterface.CreateProjectionObject(170, 2, 2, 2);

                //double[] impulse = Filtering.Ramachandran_Lakshminarayanan_RealSpacefilter(512, 2.0 / 256.0);
                double[,] impulse = Filtering.SincRealSpaceFilter2D(512, 1);
                //double[,] impulse = Filtering.Ramachandran_Lakshminarayanan_RealSpacefilter2D(512,1);

                ScriptingInterface.scriptingInterface.MakeVariableVisible("ProjectionOject", po);

                string TempFilePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\";
                aProjectionSlice[] Slices = new aProjectionSlice[FileNames.Length];
                string fileout;

                double dWidth = ClipRectangle.Width;
                double dHeight = ClipRectangle.Height;

                for (int i = 0; i < FileNames.Length; i++)
                {
                    Bitmap b = MathImageHelps.LoadBitmap(FileNames[i], false);
                    Bitmap b2 = b.ClipImage(ClipRectangle);

                    double[,] ImageArray = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray( b2,false);
                    ImageArray.InvertArrayValuesInPlace();
                    ImageArray = MathImageHelps.FlattenImageEdges(ImageArray);
                    b2 = ImageArray.MakeBitmap();

                    fileout = TempFilePath + Path.GetFileName(FileNames[i]);
                    //Image.Save(fileout);
                    b2.Save(fileout);
                    b2 = null;
                    ProjectionSliceFile psf = new ProjectionSliceFile();
                    psf.PersistDataInMemory = false;
                    psf.LoadFile(fileout, SliceDegree * i, 2d * dHeight / dWidth, 2d, false);
                    Slices[i] = psf;

                    ScriptingInterface.scriptingInterface.CreateGraph("ImageTest", psf.Projection);
                }

                aProjectionSlice apS = Slices[0];
                int Width = apS.Projection.GetLength(Axis.XAxis);
                int Height = apS.Projection.GetLength(Axis.YAxis);
                int PaddedSize;
                if (Width > Height)
                    PaddedSize = (int)MathHelps.NearestPowerOf2(2 * Width);
                else
                    PaddedSize = (int)MathHelps.NearestPowerOf2(2 * Height);


                po.DoBackProjection_ThreadedMediumMemory(PaddedSize, Slices, impulse, ConvolutionMethod.ConvolutionRealSpaceFilterFFT);

                ScriptingInterface.scriptingInterface.CreateGraph("ProjectionOutput", po.ProjectionData);
            }
        }
        #endregion
       
        #region TestConvolutions
        private void testConvolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] Data = new double[256];
            double half = Data.Length / 2d;
            double Radius = Data.Length / 4d;
            Radius *= Radius;
            for (int i = 0; i < Data.Length; i++)
            {
                if ((i - half) * (i - half) < Radius)
                {
                    Data[i] = Math.Sqrt(Radius - (i - half) * (i - half));
                }
            }

            double[] impulse = Filtering.Ramachandran_Lakshminarayanan_RS_RadonFilter(512, 2.0 / 256.0);


            #region Test The Filters
            double[] impulseFFT = Filtering.Rectangular_RS_RadonFilter (512,512, 1000, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("ImpulseFFT", impulseFFT, "X", "Y");

            double[] TestFFT1 = Filtering.ConvoluteChop(Data, impulseFFT);
            double[] TestFFT2 = Filtering.ConvoluteChop(Data, impulse);
            List<double[,]> Lines = new List<double[,]>();
            Lines.Add(Data.Normalize1DArray().MakeGraphableArray(-1 * half, 1));
            Lines.Add(TestFFT1.Normalize1DArray().MakeGraphableArray(TestFFT1.Length / -2d, 1));
            Lines.Add(TestFFT2.Normalize1DArray().MakeGraphableArray(TestFFT2.Length / -2d, 1));
            ScriptingInterface.scriptingInterface.CreateGraph("TestFFT", Lines, "X", "", "Y");

            TestFFT1.Normalize1DArrayInPlace();
            TestFFT2.Normalize1DArrayInPlace();
            double[] TestSubtract = TestFFT1.DivideInPlaceErrorless(TestFFT2);
            Lines = new List<double[,]>();
            Lines.Add(Data.Normalize1DArray().MakeGraphableArray(-1 * half, 1));
            Lines.Add(TestFFT1.Normalize1DArray().MakeGraphableArray(TestFFT1.Length / -2d, 1));
            Lines.Add(TestFFT2.Normalize1DArray().MakeGraphableArray(TestFFT2.Length / -2d, 1));
            Lines.Add(TestSubtract.Normalize1DArray().MakeGraphableArray(TestSubtract.Length / -2d, 1));
            ScriptingInterface.scriptingInterface.CreateGraph("TestSubtract", Lines, "X", "", "Y");
            #endregion

            Lines = new List<double[,]>();
            Lines.Add(Data.Normalize1DArray().MakeGraphableArray(-1 * half, 1));
            Lines.Add(impulse.Normalize1DArray().MakeGraphableArray(impulse.Length / -2d, 1));
            ScriptingInterface.scriptingInterface.CreateGraph("ImageData", Lines, "X", "", "Y");

            double[] ConvolutionFull = null;
            double[] ConvolutionChoped = null;
            double[] ConvolutionFFT = null;

            //Do a convolution along this axis
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (int i = 0; i < 500; i++)
            {
                ConvolutionFull = Filtering.Convolute(Data, impulse);
            }
            long Elapsed = sw.ElapsedMilliseconds;
            LoggerForm.LogMessage("Convolution Full Finished in " + Elapsed + " ms");
            sw.Reset();

            sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (int i = 0; i < 500; i++)
            {
                ConvolutionChoped = Filtering.ConvoluteChop(Data, impulse);
            }
            Elapsed = sw.ElapsedMilliseconds;
            LoggerForm.LogMessage("Convolution Chopped Finished in " + Elapsed + " ms");
            sw.Reset();

            sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (int i = 0; i < 500; i++)
            {
                ConvolutionFFT = Filtering.ConvoluteFFT(Data, impulse, true, false);
            }
            Elapsed = sw.ElapsedMilliseconds;
            LoggerForm.LogMessage("Convolution FFTcomplex2complex Finished in " + Elapsed + " ms");
            sw.Reset();

            Lines.Clear();
            Lines.Add(ConvolutionFull.MakeGraphableArray(ConvolutionFull.Length / -2, 1));
            Lines.Add(ConvolutionChoped.MakeGraphableArray(ConvolutionChoped.Length / -2, 1));
            Lines.Add(ConvolutionFFT.MakeGraphableArray(ConvolutionFFT.Length / -2, 1));

            ScriptingInterface.scriptingInterface.CreateGraph("Convolution 1", Lines, "X", "", "Y");

            double[] SubFullChopped = new double[Data.Length];
            double[] SubFFTChopped = new double[Data.Length];
            double[] SubFullFFT = new double[Data.Length];

            int OffsetX = (ConvolutionFull.Length - Data.Length) / 2;
            for (int i = 0; i < Data.Length; i++)
            {
                SubFullChopped[i] = ConvolutionFull[i + OffsetX] - ConvolutionChoped[i];
            }
            OffsetX = (ConvolutionFFT.Length - Data.Length) / 2;
            for (int i = 0; i < Data.Length; i++)
            {
                SubFFTChopped[i] = ConvolutionFFT[i + OffsetX] - ConvolutionChoped[i];
            }

            double[] tSubFullFFT = new double[ConvolutionFFT.Length];
            OffsetX = (ConvolutionFull.Length - ConvolutionFFT.Length) / 2;
            for (int i = 0; i < ConvolutionFFT.Length; i++)
            {
                tSubFullFFT[i] = ConvolutionFull[i + OffsetX] - ConvolutionFFT[i];
            }

            OffsetX = (tSubFullFFT.Length - Data.Length) / 2;
            for (int i = 0; i < Data.Length; i++)
            {
                SubFullFFT[i] = tSubFullFFT[i + OffsetX];
            }


            Lines.Clear();
            Lines.Add(SubFullChopped.MakeGraphableArray(SubFullChopped.Length / -2, 1));
            Lines.Add(SubFFTChopped.MakeGraphableArray(SubFFTChopped.Length / -2, 1));
            Lines.Add(SubFullFFT.MakeGraphableArray(SubFullFFT.Length / -2, 1));

            ScriptingInterface.scriptingInterface.CreateGraph("Convolution Subtract", Lines, "X", "", "Y");
        }

        private void testFFTConvolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] Data = new double[256];
            double half = Data.Length / 2d;
            double Radius = Data.Length / 4d;
            Radius *= Radius;
            for (int i = 0; i < Data.Length; i++)
            {
                if ((i - half) * (i - half) < Radius)
                {
                    Data[i] = Math.Sqrt(Radius - (i - half) * (i - half));
                }
            }

            double[] impulse = Filtering.Ramachandran_Lakshminarayanan_RS_RadonFilter(512, 2.0 / 256.0);

            List<double[,]> Lines = new List<double[,]>();
            Lines.Add(Data.Normalize1DArray().MakeGraphableArray(-1 * half, 1));
            Lines.Add(impulse.Normalize1DArray().MakeGraphableArray(impulse.Length / -2d, 1));
            ScriptingInterface.scriptingInterface.CreateGraph("ImageData", Lines, "X", "", "Y");

            #region FFTConvolutions
            double[] Filter = Filtering.RectangularFSProjectionFilter(512,512, 1000, 1);
            Filter = Filtering.ConvoluteFFT(Data, Filter, false, true);
            ScriptingInterface.scriptingInterface.CreateGraph("Rectangular", Filter, "X", "Y");

            Filter = Filtering.HammingFSProjectionFilter(512, 512, 1);
            Filter = Filtering.ConvoluteFFT(Data, Filter, false, true);
            ScriptingInterface.scriptingInterface.CreateGraph("Hamming", Filter, "X", "Y");

            Filter = Filtering.HanFSProjectionFilter(512, 512, 1);
            Filter = Filtering.ConvoluteFFT(Data, Filter, false, true);
            ScriptingInterface.scriptingInterface.CreateGraph("Hann", Filter, "X", "Y");

            Filter = Filtering.TukeyFSProjectionFilter(512, 512, .5, 1);
            Filter = Filtering.ConvoluteFFT(Data, Filter, false, true);
            ScriptingInterface.scriptingInterface.CreateGraph("tukey", Filter, "X", "Y");

            Filter = Filtering.CosineFSProjectionFilter(512, 512, 1);
            Filter = Filtering.ConvoluteFFT(Data, Filter, false, true);
            ScriptingInterface.scriptingInterface.CreateGraph("Cosine", Filter, "X", "Y");

            Filter = Filtering.LanczosFSProjectionFilter(512, 512, 1);
            Filter = Filtering.ConvoluteFFT(Data, Filter, false, true);
            ScriptingInterface.scriptingInterface.CreateGraph("Lanczos", Filter, "X", "Y");

            Filter = Filtering.SincFSProjectionFilter(512, 512, 1);
            Filter = Filtering.ConvoluteFFT(Data, Filter, false, true);
            ScriptingInterface.scriptingInterface.CreateGraph("Sinc", Filter, "X", "Y");

            Filter = Filtering.TriangularFSProjectionFilter(512, 512, 1);
            Filter = Filtering.ConvoluteFFT(Data, Filter, false, true);
            ScriptingInterface.scriptingInterface.CreateGraph("Triangular", Filter, "X", "Y");

            Filter = Filtering.GaussianFSProjectionFilter(512, 512, .5, 1);
            Filter = Filtering.ConvoluteFFT(Data, Filter, false, true);
            ScriptingInterface.scriptingInterface.CreateGraph("Guassian", Filter, "X", "Y");

            Filter = Filtering.BartlettFSProjectionFilter(512, 512, 1);
            Filter = Filtering.ConvoluteFFT(Data, Filter, false, true);
            ScriptingInterface.scriptingInterface.CreateGraph("Bartlett", Filter, "X", "Y");

            Filter = Filtering.BartlettHannFSProjectionFilter(512, 512, 1);
            Filter = Filtering.ConvoluteFFT(Data, Filter, false, true);
            ScriptingInterface.scriptingInterface.CreateGraph("BartlettHann", Filter, "X", "Y");

            Filter = Filtering.BlackmanFSProjectionFilter(512, 512, 1);
            Filter = Filtering.ConvoluteFFT(Data, Filter, false, true);
            ScriptingInterface.scriptingInterface.CreateGraph("Blackmann", Filter, "X", "Y");

            #endregion
            #region RealSpaceConvolutions

            Filter = Filtering.Rectangular_RS_RadonFilter(512, 512, 1000, 1);
            Filter = Filtering.ConvoluteChop(Data, Filter);
            ScriptingInterface.scriptingInterface.CreateGraph("RectangularReal", Filter, "X", "Y");

            Filter = Filtering.Hamming_RS_RadonFilter(512, 512, 1);
            Filter = Filtering.ConvoluteChop(Data, Filter);
            ScriptingInterface.scriptingInterface.CreateGraph("HammingReal", Filter, "X", "Y");

            Filter = Filtering.Han_RS_RadonFilter(512, 512, 1);
            Filter = Filtering.ConvoluteChop(Data, Filter);
            ScriptingInterface.scriptingInterface.CreateGraph("HannReal", Filter, "X", "Y");

            Filter = Filtering.Tukey_RS_RadonFilter(512, 512, .5, 1);
            Filter = Filtering.ConvoluteChop(Data, Filter);
            ScriptingInterface.scriptingInterface.CreateGraph("tukeyReal", Filter, "X", "Y");

            Filter = Filtering.Cosine_RS_RadonFilter(512, 512, 1);
            Filter = Filtering.ConvoluteChop(Data, Filter);
            ScriptingInterface.scriptingInterface.CreateGraph("CosineReal", Filter, "X", "Y");

            Filter = Filtering.Lanczos_RS_RadonFilter(512, 512, 1);
            Filter = Filtering.ConvoluteChop(Data, Filter);
            ScriptingInterface.scriptingInterface.CreateGraph("LanczosReal", Filter, "X", "Y");

            Filter = Filtering.Sinc_RS_RadonFilter(512, 512, 1);
            Filter = Filtering.ConvoluteChop(Data, Filter);
            ScriptingInterface.scriptingInterface.CreateGraph("SincReal", Filter, "X", "Y");

            Filter = Filtering.Triangular_RS_RadonFilter(512, 512, 1);
            Filter = Filtering.ConvoluteChop(Data, Filter);
            ScriptingInterface.scriptingInterface.CreateGraph("TriangularReal", Filter, "X", "Y");

            Filter = Filtering.Gaussian_RS_RadonFilter(512, 512, .5, 1);
            Filter = Filtering.ConvoluteChop(Data, Filter);
            ScriptingInterface.scriptingInterface.CreateGraph("GuassianReal", Filter, "X", "Y");

            Filter = Filtering.Bartlett_RS_RadonFilter(512, 512, 1);
            Filter = Filtering.ConvoluteChop(Data, Filter);
            ScriptingInterface.scriptingInterface.CreateGraph("BartlettReal", Filter, "X", "Y");

            Filter = Filtering.BartlettHann_RS_RadonFilter(512, 512, 1);
            Filter = Filtering.ConvoluteChop(Data, Filter);
            ScriptingInterface.scriptingInterface.CreateGraph("BartlettHannReal", Filter, "X", "Y");

            Filter = Filtering.Blackman_RS_RadonFilter(512, 512, 1);
            Filter = Filtering.ConvoluteChop(Data, Filter);
            ScriptingInterface.scriptingInterface.CreateGraph("BlackmannReal", Filter, "X", "Y");
            #endregion
        }

        private void testConvolution2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] Data = new double[128, 128];
            double half = Data.GetLength(0) / 2d;
            double Radius = half / 2d;
            double r, x, y;
            Radius *= Radius;
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                x = i - half;
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    y = j - half;
                    r = x * x + y * y;
                    if (r < Radius)
                    {
                        Data[i, j] = Math.Sqrt(Radius - r);
                    }
                }
            }
            ScriptingInterface.scriptingInterface.CreateGraph("ImageData", Data, "X", "", "Y");

            double[] impulse1D = Filtering.Rectangular_RS_RadonFilter (64,64, 10000, 1);
            double[,] impulse = Filtering.RectangularRealSpaceFilter2D (64, 10000, 1); //Filtering.Ramachandran_Lakshminarayanan_RealSpacefilter2D(512, 2.0 / 256.0);
            ScriptingInterface.scriptingInterface.CreateGraph("Impulse 1D", impulse1D, "X", "");
            ScriptingInterface.scriptingInterface.CreateGraph("DataImpulse", impulse.LogErrorless(), "X", "", "Y");
            /*
            ///////Test Physical Arrays//////////////////////////////////
            PhysicalArray dataPA = new PhysicalArray(ImageData, -1, 1, -1, 1, true);
            ScriptingInterface.scriptingInterface.CreateGraph("ImageData Physical Array", dataPA);

            //double[,] oArray= 
            dataPA = dataPA.ConvoluteFFT2D(Axis.XAxis, Axis.YAxis, impulse, true);
            //ScriptingInterface.scriptingInterface.CreateGraph("DataPAFullSize", oArray, "X", "", "Y");
            ScriptingInterface.scriptingInterface.CreateGraph("ImageData PA_ConvolutionFFT", dataPA);

            dataPA = new PhysicalArray(ImageData, -1, 1, -1, 1, true);
            //dataPA= dataPA.ConvoluteChopSeperable(Axis.XAxis, Axis.YAxis, impulse1D);
            dataPA = dataPA.ConvoluteChop1D(Axis.XAxis, impulse1D);
            ScriptingInterface.scriptingInterface.CreateGraph("DataPA_ConvolutionX", dataPA);

            dataPA = new PhysicalArray(ImageData, -1, 1, -1, 1, true);
            dataPA = dataPA.ConvoluteChop1D(Axis.YAxis, impulse1D);
            ScriptingInterface.scriptingInterface.CreateGraph("DataPA_ConvolutionY", dataPA);

            dataPA = new PhysicalArray(ImageData, -1, 1, -1, 1, true);
            dataPA = dataPA.ConvoluteChopSeperable(Axis.XAxis, Axis.YAxis, impulse1D);
            ScriptingInterface.scriptingInterface.CreateGraph("DataPA_Convolution Seperable", dataPA);


            dataPA = new PhysicalArray(ImageData, -1, 1, -1, 1, true);
            dataPA = dataPA.ConvoluteReal2D(Axis.XAxis, Axis.YAxis, impulse);
            ScriptingInterface.scriptingInterface.CreateGraph("DataPA_Convolution Real", dataPA);
            return;
            ////////////Test real space convolutions
              */
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            double[,] ConvolutionSeperable = MathHelps.ConvoluteChopSeperable(Data, impulse1D);
            ScriptingInterface.scriptingInterface.CreateGraph("ImageData Convolution Seperable", ConvolutionSeperable, "X", "Y", "Intensity");
            long Elapsed = sw.ElapsedMilliseconds;
            LoggerForm.LogMessage("Convolution seperable Finished in " + Elapsed + " ms");


            Data = new double[256, 256];
            half = Data.GetLength(0) / 2d;
            Radius = half / 2d;
            Radius *= Radius;
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                x = i - half;
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    y = j - half;
                    r = x * x + y * y;
                    if (r < Radius)
                    {
                        Data[i, j] = Math.Sqrt(Radius - r);
                    }
                }
            }

            impulse = Filtering.RectangularRealSpaceFilter2D(256, 10000, 1); //Filtering.Ramachandran_Lakshminarayanan_RealSpacefilter2D(512, 2.0 / 256.0);
         

            ///////////////Test FFT Convolutions////////////////////
            double[,] ConvolutionFFT = null;
            sw.Reset();
            sw.Start();
            ConvolutionFFT = Filtering.ConvoluteFFT(Data, impulse, true);
            ConvolutionFFT.DivideInPlace(Data.GetLength(0) * Data.GetLength(1));
            Elapsed = sw.ElapsedMilliseconds;
            LoggerForm.LogMessage("Convolution FFTcomplex2complex Finished in " + Elapsed + " ms");
            sw.Reset();
            ScriptingInterface.scriptingInterface.CreateGraph("Convolution FFT Results", ConvolutionFFT.ReducePaddedData(Data.GetLength(0), Data.GetLength(1)), "X", "", "Y");

            ConvolutionFFT.DivideInPlace(ConvolutionFFT.AverageArray());

            double[,] ConvolutionSub = ConvolutionFFT;//.SubtractFromArray(ConvolutionReal);
            ScriptingInterface.scriptingInterface.CreateGraph("Convolution subtract", ConvolutionSub, "X", "Y", "Intensity");

            // LoggerForm.LogMessage("Sum Real " + ConvolutionReal.AverageArray());
            LoggerForm.LogMessage("Sum FFT " + ConvolutionFFT.AverageArray());
            LoggerForm.LogMessage("Total Difference" + ConvolutionSub.AverageArray());

            double[] Line = ConvolutionSub.GetXLine(ConvolutionSub.GetLength(1) / 2);
            ScriptingInterface.scriptingInterface.CreateGraph("Subtraction Profile", Line, "X", "Y");

        }
        private void testWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] Filter = Filtering.RectangularWindow (512,512, 1000, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Rectangular", Filter, "X", "Y");

            Filter = Filtering.HammingWindow(100,100);
            ScriptingInterface.scriptingInterface.CreateGraph("Hamming", Filter, "X", "Y");

            Filter = Filtering.HanWindow(100, 100);
            ScriptingInterface.scriptingInterface.CreateGraph("Hann", Filter, "X", "Y");

            Filter = Filtering.TukeyWindow(100, 100, .5);
            ScriptingInterface.scriptingInterface.CreateGraph("tukey", Filter, "X", "Y");

            Filter = Filtering.CosineWindow(100, 100);
            ScriptingInterface.scriptingInterface.CreateGraph("Cosine", Filter, "X", "Y");

            Filter = Filtering.LanczosWindow(100, 100);
            ScriptingInterface.scriptingInterface.CreateGraph("Lanczos", Filter, "X", "Y");

            Filter = Filtering.SincWindow(100, 100);
            ScriptingInterface.scriptingInterface.CreateGraph("Sinc", Filter, "X", "Y");

            Filter = Filtering.TriangularWindow(100, 100);
            ScriptingInterface.scriptingInterface.CreateGraph("Triangular", Filter, "X", "Y");

            Filter = Filtering.GaussianWindow(100, 100, .5);
            ScriptingInterface.scriptingInterface.CreateGraph("Guassian", Filter, "X", "Y");

            Filter = Filtering.BartlettWindow(100, 100);
            ScriptingInterface.scriptingInterface.CreateGraph("Bartlett", Filter, "X", "Y");

            Filter = Filtering.BartlettHannWindow(100, 100);
            ScriptingInterface.scriptingInterface.CreateGraph("BartlettHann", Filter, "X", "Y");

            Filter = Filtering.BlackmanWindow(100, 100);
            ScriptingInterface.scriptingInterface.CreateGraph("Blackmann", Filter, "X", "Y");

            Filter = Filtering.NuttallWindow(100, 100);
            ScriptingInterface.scriptingInterface.CreateGraph("Nuttall", Filter, "X", "Y");

            Filter = Filtering.BlackMan_HarrisWindow(100, 100);
            ScriptingInterface.scriptingInterface.CreateGraph("BlackMan_Harris", Filter, "X", "Y");

            Filter = Filtering.BlackMan_NuttallWindow(100, 100);
            ScriptingInterface.scriptingInterface.CreateGraph("BlackMan_Nuttall", Filter, "X", "Y");

            Filter = Filtering.FlatTopWindow(100, 100);
            ScriptingInterface.scriptingInterface.CreateGraph("FlatTop", Filter, "X", "Y");

        }
        private void test2DWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] Filter = Filtering.RectangularWindow2D(512, 1000, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Rectangular", Filter, "X", "Y");

            Filter = Filtering.HammingWindow2D(100);
            ScriptingInterface.scriptingInterface.CreateGraph("Hamming", Filter, "X", "Y");

            Filter = Filtering.HanWindow2D(100);
            ScriptingInterface.scriptingInterface.CreateGraph("Hann", Filter, "X", "Y");

            Filter = Filtering.TukeyWindow2D(100, .5);
            ScriptingInterface.scriptingInterface.CreateGraph("tukey", Filter, "X", "Y");

            Filter = Filtering.CosineWindow2D(100);
            ScriptingInterface.scriptingInterface.CreateGraph("Cosine", Filter, "X", "Y");

            Filter = Filtering.LanczosWindow2D(100);
            ScriptingInterface.scriptingInterface.CreateGraph("Lanczos", Filter, "X", "Y");

            Filter = Filtering.SincWindow2D(100);
            ScriptingInterface.scriptingInterface.CreateGraph("Sinc", Filter, "X", "Y");

            Filter = Filtering.TriangularWindow2D(100);
            ScriptingInterface.scriptingInterface.CreateGraph("Triangular", Filter, "X", "Y");

            Filter = Filtering.GaussianWindow2D(100, .5);
            ScriptingInterface.scriptingInterface.CreateGraph("Guassian", Filter, "X", "Y");

            Filter = Filtering.BartlettWindow2D(100);
            ScriptingInterface.scriptingInterface.CreateGraph("Bartlett", Filter, "X", "Y");

            Filter = Filtering.BartlettHannWindow2D(100);
            ScriptingInterface.scriptingInterface.CreateGraph("BartlettHann", Filter, "X", "Y");

            Filter = Filtering.BlackmanWindow2D(100);
            ScriptingInterface.scriptingInterface.CreateGraph("Blackmann", Filter, "X", "Y");

        }
        private void testFiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {

            double[] Filter = Filtering.RectangularFSProjectionFilter(512,512, 1000, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Rectangular", Filter, "X", "Y");

            Filter = Filtering.HammingFSProjectionFilter(100,100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Hamming", Filter, "X", "Y");

            Filter = Filtering.HanFSProjectionFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Hann", Filter, "X", "Y");

            Filter = Filtering.TukeyFSProjectionFilter(100, 100, .5, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("tukey", Filter, "X", "Y");

            Filter = Filtering.CosineFSProjectionFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Cosine", Filter, "X", "Y");

            Filter = Filtering.LanczosFSProjectionFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Lanczos", Filter, "X", "Y");

            Filter = Filtering.SincFSProjectionFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Sinc", Filter, "X", "Y");

            Filter = Filtering.TriangularFSProjectionFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Triangular", Filter, "X", "Y");

            Filter = Filtering.GaussianFSProjectionFilter(100, 100, .5, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Guassian", Filter, "X", "Y");

            Filter = Filtering.BartlettFSProjectionFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Bartlett", Filter, "X", "Y");

            Filter = Filtering.BartlettHannFSProjectionFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("BartlettHann", Filter, "X", "Y");

            Filter = Filtering.BlackmanFSProjectionFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Blackmann", Filter, "X", "Y");

            Filter = Filtering.NuttallFSProjectionFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Nuttall", Filter, "X", "Y");

            Filter = Filtering.BlackMan_HarrisFSProjectionFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("BlackMan_Harris", Filter, "X", "Y");

            Filter = Filtering.BlackMan_NuttallFSProjectionFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("BlackMan_Nuttall", Filter, "X", "Y");

            Filter = Filtering.FlatTopFSProjectionFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("FlatTop", Filter, "X", "Y");
        }
        private void test2DFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] Filter = Filtering.RectangularProjectionFilter2D(512, 1000, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Rectangular", Filter, "X", "Y");

            Filter = Filtering.HammingProjectionFilter2D(100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Hamming", Filter, "X", "Y");

            Filter = Filtering.HanProjectionFilter2D(100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Hann", Filter, "X", "Y");

            Filter = Filtering.TukeyProjectionFilter2D(100, .5, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("tukey", Filter, "X", "Y");

            Filter = Filtering.CosineProjectionFilter2D(100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Cosine", Filter, "X", "Y");

            Filter = Filtering.LanczosProjectionFilter2D(100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Lanczos", Filter, "X", "Y");

            Filter = Filtering.SincProjectionFilter2D(100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Sinc", Filter, "X", "Y");

            Filter = Filtering.TriangularProjectionFilter2D(100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Triangular", Filter, "X", "Y");

            Filter = Filtering.GaussianProjectionFilter2D(100, .5, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Guassian", Filter, "X", "Y");

            Filter = Filtering.BartlettProjectionFilter2D(100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Bartlett", Filter, "X", "Y");

            Filter = Filtering.BartlettHannProjectionFilter2D(100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("BartlettHann", Filter, "X", "Y");

            Filter = Filtering.BlackmanProjectionFilter2D(100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Blackmann", Filter, "X", "Y");

        }


        private void testConvolutionImpulseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] Filter = Filtering.Rectangular_RS_RadonFilter(100,100, 10, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Rectangular", Filter, "X", "Y");

            Filter = Filtering.Hamming_RS_RadonFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Hamming", Filter, "X", "Y");

            Filter = Filtering.Han_RS_RadonFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Hann", Filter, "X", "Y");

            Filter = Filtering.Tukey_RS_RadonFilter(100, 100, .5, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("tukey", Filter, "X", "Y");

            Filter = Filtering.Cosine_RS_RadonFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Cosine", Filter, "X", "Y");

            Filter = Filtering.Lanczos_RS_RadonFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Lanczos", Filter, "X", "Y");

            Filter = Filtering.Sinc_RS_RadonFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Sinc", Filter, "X", "Y");

            Filter = Filtering.Triangular_RS_RadonFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Triangular", Filter, "X", "Y");

            Filter = Filtering.Gaussian_RS_RadonFilter(100, 100, .5, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Guassian", Filter, "X", "Y");

            Filter = Filtering.Bartlett_RS_RadonFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Bartlett", Filter, "X", "Y");

            Filter = Filtering.BartlettHann_RS_RadonFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("BartlettHann", Filter, "X", "Y");

            Filter = Filtering.Blackman_RS_RadonFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Blackmann", Filter, "X", "Y");

            Filter = Filtering.Nuttall_RS_RadonFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Nuttall", Filter, "X", "Y");

            Filter = Filtering.BlackMan_Nuttall_RS_RadonFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Blackman Nuttall", Filter, "X", "Y");

            Filter = Filtering.BlackMan_Harris_RS_RadonFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Blackman Harris", Filter, "X", "Y");

            Filter = Filtering.FlatTop_RS_RadonFilter(100, 100, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Flat Top", Filter, "X", "Y");

        }

        private void test2DConvolutionFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] Filter = Filtering.RectangularRealSpaceFilter2D(256, 1000, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Rectangular", Filter, "X", "Y");

            Filter = Filtering.HammingRealSpaceFilter2D(512, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Hamming", Filter.LogErrorless(), "X", "Y");

            Filter = Filtering.HanRealSpaceFilter2D(512, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Hann", Filter.LogErrorless(), "X", "Y");

            Filter = Filtering.TukeyRealSpaceFilter2D(512, .5, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("tukey", Filter.LogErrorless(), "X", "Y");

            Filter = Filtering.CosineRealSpaceFilter2D(512, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Cosine", Filter.LogErrorless(), "X", "Y");

            Filter = Filtering.LanczosRealSpaceFilter2D(512, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Lanczos", Filter.LogErrorless(), "X", "Y");

            Filter = Filtering.SincRealSpaceFilter2D(512, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Sinc", Filter.LogErrorless(), "X", "Y");

            Filter = Filtering.TriangularRealSpaceFilter2D(512, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Triangular", Filter.LogErrorless(), "X", "Y");

            Filter = Filtering.GaussianRealSpaceFilter2D(512, .5, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Guassian", Filter.LogErrorless(), "X", "Y");

            Filter = Filtering.BartlettRealSpaceFilter2D(512, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Bartlett", Filter.LogErrorless(), "X", "Y");

            Filter = Filtering.BartlettHannRealSpaceFilter2D(512, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("BartlettHann", Filter.LogErrorless(), "X", "Y");

            Filter = Filtering.BlackmanRealSpaceFilter2D(512, 1);
            ScriptingInterface.scriptingInterface.CreateGraph("Blackmann", Filter.LogErrorless(), "X", "Y");
        }

        #endregion

        private void testVariableWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectionObject po = new ProjectionObject();
            po.ClearGrid(true, 2, 2, 256, 256);
            ScriptingInterface.scriptingInterface.MakeVariableVisible("Projection Object 2D", po);
            po = new ProjectionObject();
            po.ClearGrid(true, 2, 2, 2, 256, 256, 256);
            ScriptingInterface.scriptingInterface.MakeVariableVisible("Projection Object 3D", po);

            double A = 25.05;
            ScriptingInterface.scriptingInterface.MakeVariableVisible("A", A);

            string tString = "Hello World";
            ScriptingInterface.scriptingInterface.MakeVariableVisible("TestString", tString);

            double[,] TestArray = new double[2, 10];
            for (int i = 0; i < TestArray.GetLength(1); i++)
            {
                TestArray[0, i] = i;
                TestArray[1, i] = i;
            }
            ScriptingInterface.scriptingInterface.MakeVariableVisible("TestArray", TestArray);

        }

        
        #endregion

        #region FileHandling
        private void saveProjectionObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ret = saveFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                foreach (object o in ScriptingInterface.scriptingInterface.VisibleVariables.Values)
                {
                    try
                    {
                        ProjectionObject po = (ProjectionObject)o;
                        po.SaveDensityData(saveFileDialog1.FileName);
                        return;
                    }
                    catch { }
                }
            }
        }

        private void openProjectionObjectToolStripMenuItem_Click(object sender, EventArgs e)
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
                ScriptingInterface.scriptingInterface.MakeVariableVisible( Path.GetFileNameWithoutExtension( openFileDialog1.FileName) , po);
                ScriptingInterface.scriptingInterface.CreateGraph(Path.GetFileNameWithoutExtension(openFileDialog1.FileName), po.ProjectionData);
            }
            openFileDialog1.Multiselect = false;
        }

        private void fixViToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                string[] FileNames =// Directory.GetFiles(@"C:\Users\bashcAdmin\Documents\Visual Studio 2008\Projects\Flo1RawPP_cct001_20100511_150225\PP\");
                    openFileDialog1.FileNames;
                FileNames = MathStringHelps.SortNumberedFiles(FileNames);

                if (FileNames == null || FileNames.Length == 0)
                {
                    FileNames = openFileDialog1.FileNames;
                    for (int i = 0; i < FileNames.Length; i++)
                    {
                        Application.DoEvents();
                        OpenVisionGate.FixVisionGateImage(FileNames[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < FileNames.Length; i++)
                    {
                        Application.DoEvents();
                        OpenVisionGate.FixVisionGateImage(FileNames[i]);
                    }
                }
                MessageBox.Show("Finished Processing", "");
            }
            openFileDialog1.Multiselect = false;

        }
        #endregion

        #region Wonky Images

       /* private void edgeTrackingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = DialogResult.OK;// openFileDialog1.ShowDialog();
            //DialogResult ret =  openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {


                string[] unsortedFilenames = Directory.GetFiles("C:\\Users\\bashcAdmin\\Desktop\\New folder (2)\\", "*.png");/// openFileDialog1.mFileNames;
                //string[] unsortedFilenames = Directory.GetFiles(@"C:\Users\bashcAdmin\Documents\Visual Studio 2008\Projects\Tomographic_Imaging_MDL\Tomographic_Imaging_2\bin\Debug\TEMP\", "*.png");/// openFileDialog1.mFileNames;
                string[] Filenames = MathStringHelps.SortNumberedFiles(unsortedFilenames);

                ImageSelector imageSelector = new ImageSelector();
                TomographicMainForm.ShowDockContent(imageSelector);
                imageSelector.SetFilenames(Filenames);

                imageSelector.WaitForInput("Select important feature");
                Rectangle TopHalf = imageSelector.ActiveSelection.SelectionBounds;

                VisionForm vf = new VisionForm();
                TomographicMainForm.ShowDockContent(vf);
                vf.SetFilenames(Filenames);
                vf.WaitForInput("Please set the canny properties correctly");

                Bitmap bb = new Bitmap(Filenames[0]);

                List<VisionHelper.IEdgeFound[]> Edges = new List<VisionHelper.IEdgeFound[]>();
                for (int i = 0; i < Filenames.Length; i++)
                {

                    Bitmap b = new Bitmap(Filenames[i]);
                    Bitmap b2 = b;
                    VisionHelper.IEdgeFound[] FoundEdges = vf.GetEdges(b2);
                    string Filename = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\Edged" + i.ToString() + ".png";
                    vf.ProcessedImage.Save(Filename);
                    Application.DoEvents();
                    Edges.Add(FoundEdges);
                }

                imageSelector.WaitForInput("Select desired edge region");
                VisionHelper.IEdgeFound[][] EdgesFound = VisionTools.LineAngleReporter.RegionSeperator(Edges, TopHalf);

                double[,] AllAngles = VisionTools.LineAngleReporter.CalculateAngles(EdgesFound);

                ScriptingInterface.scriptingInterface.CreateGraph("Quadrants", AllAngles, "Filenumber", "", "Angle(Radians)");

                double[,] Angles = VisionTools.LineAngleReporter.MakeSingleValued(AllAngles);

                ScriptingInterface.scriptingInterface.CreateGraph("QuadrantsClean", Angles, "Filenumber", "", "Angle(Radians)");


                //double[,] Angles = new double[2, 100];
                double[] linearAngles = new double[Angles.GetLength(1)];
                for (int i = 0; i < Angles.GetLength(1); i++)
                {
                    linearAngles[i] = Angles[1, i];
                }

                double[] Smoothed = new double[linearAngles.Length];
                int Count = 0;
                int Period = 5;
                for (int i = 0; i < linearAngles.Length; i++)
                {
                    Count = 0;
                    for (int j = i - Period; j < i + Period; j++)
                    {
                        try
                        {
                            Smoothed[i] += linearAngles[j];
                            Count++;
                        }
                        catch { }
                    }
                    if (Count > 0)
                        Smoothed[i] = Smoothed[i] / Count;
                }
                List<double[,]> LinesA = new List<double[,]>();
                LinesA.Add(linearAngles.MakeGraphableArray(0, 1));
                LinesA.Add(Smoothed.MakeGraphableArray(0, 1));
                ScriptingInterface.scriptingInterface.CreateGraph("SmoothedData", LinesA, "Filenumber", "", "Angle");


                complex[] fftC = MathFFTHelps.FFTreal2complex(linearAngles);
                complex[] fftC2 = new complex[fftC.Length];
                ScriptingInterface.scriptingInterface.CreateGraph("FFTcomplex2complex", fftC.ConvertToDoubleMagnitude(), "", "");

                string sFreq = UseableInputBox.Show("Desired Frequency").Wait();
                int Freq = 10;
                int.TryParse(sFreq, out Freq);
                for (int i = 0; i < fftC.Length; i++)
                {
                    if (i == Freq || i == 0)
                        fftC2[i] = fftC[i];
                }

                bool Finished = false;

                double[] fftAngles = MathFFTHelps.iFFTcomplex2real(fftC2);

                fftAngles = fftAngles.DivideToArray((double)fftAngles.Length);

                List<double[,]> Lines = new List<double[,]>();
                Lines.Add(linearAngles.MakeGraphableArray(0, 1));
                Lines.Add(fftAngles.MakeGraphableArray(0, 1));
                ScriptingInterface.scriptingInterface.CreateGraph("FFTAngles", Lines, "", "", "");

                string tempPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\";

                double midPointSmooth = Smoothed.Average();
                double midPointLA = linearAngles.Average();
                double sumLA = 0;
                double sumS = 0;
                for (int i = 0; i < linearAngles.Length; i++)
                {
                    sumLA += Math.Pow((linearAngles[i] - midPointLA), 2);
                    sumS += Math.Pow((Smoothed[i] - midPointSmooth), 2);
                }

                double ScaleFactor = Math.Sqrt(sumLA / sumS);

                double[] UseAngles = new double[fftAngles.Length];

                while (!Finished)
                {
                    string retS = UseableInputBox.Show("Desired Scale").Wait();
                    if (retS == "")
                    {
                        Finished = true;
                        break;
                    }
                    double.TryParse(retS, out ScaleFactor);
                    for (int i = 0; i < fftAngles.Length; i++)
                    {
                        UseAngles[i] = (Smoothed[i] - midPointLA) * ScaleFactor + midPointLA;
                    }
                    for (int i = 0; i < Filenames.Length; i++)
                    {
                        Bitmap b = MathImageHelps.rotateImage(new Bitmap(Filenames[i]), (float)((Math.PI - UseAngles[i]) / Math.PI * 180));
                        Bitmap b2 = b.ClipImage(TopHalf);
                        imageSelector.SetBitmap(b2);
                        Application.DoEvents();
                    }
                }
                for (int i = 0; i < Filenames.Length; i++)
                {
                    Bitmap b = MathImageHelps.rotateImage(new Bitmap(Filenames[i]), (float)((Math.PI - UseAngles[i]) / Math.PI * 180));
                    Bitmap b2 = b.ClipImage(TopHalf);
                    imageSelector.SetBitmap(b2);
                    b2.Save(tempPath + Path.GetFileName(Filenames[i]));
                    Application.DoEvents();
                }
            }
        }

        private void momentOfInertiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            //DialogResult ret = DialogResult.OK;// openFileDialog1.ShowDialog();
            DialogResult ret = openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {

                //string[] unsortedFilenames = Directory.GetFiles(@"C:\Users\bashcAdmin\Desktop\New folder (2)\");
                string[] unsortedFilenames = openFileDialog1.FileNames;
                //string[] unsortedFilenames = Directory.GetFiles(@"C:\Users\bashcAdmin\Documents\Visual Studio 2008\Projects\Tomographic_Imaging_MDL\Tomographic_Imaging_2\bin\Debug\TEMP\", "*.png");/// openFileDialog1.mFileNames;
                string[] Filenames = MathStringHelps.SortNumberedFiles(unsortedFilenames);

                ImageSelector imageSelector = new ImageSelector();
                TomographicMainForm.ShowDockContent(imageSelector);
                imageSelector.SetFilenames(Filenames);
                imageSelector.WaitForInput("Please select circle with interesting features");

                ImageViewer.ISelection CircleSelection = imageSelector.ActiveSelection;
                //new MathImageViewer.Selections.CircleSelection(new Rectangle(278, 203, 81, 81), -7, 7);
                System.Diagnostics.Debug.Print(CircleSelection.SelectionBounds.X.ToString());
                System.Diagnostics.Debug.Print(CircleSelection.SelectionBounds.Y.ToString());
                System.Diagnostics.Debug.Print(CircleSelection.SelectionBounds.Width.ToString());
                System.Diagnostics.Debug.Print(CircleSelection.SelectionBounds.Height.ToString());
                System.Diagnostics.Debug.Print(((ImageViewer.Selections.CircleSelection)CircleSelection).InnerRadius.ToString());
                System.Diagnostics.Debug.Print(((ImageViewer.Selections.CircleSelection)CircleSelection).OuterRadius.ToString());


                double X0 = CircleSelection.SelectionBounds.X;
                double X1 = CircleSelection.SelectionBounds.Right;
                double Y0 = CircleSelection.SelectionBounds.Y;
                double Y1 = CircleSelection.SelectionBounds.Bottom;
                double CX = CircleSelection.ImageCenter.X;
                double CY = CircleSelection.ImageCenter.Y;
                double Angle;

                double[] Modes2 = new double[Filenames.Length];

                double[] Maxs2 = new double[Filenames.Length];
                double[,] Slosh = new double[360, Filenames.Length];
                for (int i = 0; i < Filenames.Length; i++)
                {
                    double[] Intensity = new double[360];
                    double[] Count = new double[360];
                    double[,] image = MathImageHelps.LoadStandardImage_Intensity(Filenames[i], false);

                    #region Find Angular Distribution
                    for (int x = (int)X0; x < X1; x++)
                        for (int y = (int)Y0; y < Y1; y++)
                        {
                            if (CircleSelection.PointInSelection(new Point(x, y)))
                            {
                                double rx = x - CX;
                                double ry = y - CY;
                                double r2 = rx * rx + ry * ry;
                                Angle = MathHelps.GetAngleDegrees(rx, ry);
                                Intensity[(int)Angle] += image[x, y];// *r2;
                                Count[(int)Angle] += 1;// r2;
                                if (Angle > 90 && Angle < 180)
                                    image[x, y] = 0;
                            }
                        }
                    #endregion

                    #region Normalize Angular Distribution (correct for being a box or oval instead of a circle)
                    for (int m = 0; m < Intensity.Length; m++)
                    {
                        try
                        {
                            if (Count[m] != 0)
                                Intensity[m] = Intensity[m] / Count[m];
                            else
                                Intensity[m] = Intensity[m - 1];
                        }
                        catch { }
                    }
                    ScriptingInterface.scriptingInterface.CreateGraph("Angular Distribution", Intensity, "Angle", "Intensity");
                    Application.DoEvents();
                    #endregion

                    for (int A = 0; A < 360; A++)
                        Slosh[A, i] = Intensity[A];

                }
                imageSelector.SetBitmap(Slosh.MakeBitmap());
                imageSelector.WaitForInput("Select region that moves");
                ImageViewer.Selections.ROISelection Roi = (ImageViewer.Selections.ROISelection)imageSelector.ActiveSelection;
                //new MathImageViewer.Selections.ROISelection(new Rectangle(57,4,83,269));
                int StartAngle = Roi.SelectionBounds.X;
                int EndAngle = Roi.SelectionBounds.X + Roi.SelectionBounds.Width;

                for (int i = 0; i < Filenames.Length; i++)
                {
                    #region get the mode and the intersesting part of the angles
                    int cc = 0;

                    double mode2 = 0; double Divisor2 = 0;
                    for (int a = StartAngle; a < EndAngle; a++)
                    {
                        mode2 += Slosh[a, i] * (a);
                        Divisor2 += Slosh[a, i];
                        cc++;
                    }

                    double max = 0;
                    double MaxAngle = 0;
                    for (int a = StartAngle; a < EndAngle; a++)
                    {
                        //Interesting[cc] = Intensity[a];
                        mode2 += Slosh[a, i] * (a);
                        Divisor2 += Slosh[a, i];

                    }
                    for (int a = StartAngle; a < EndAngle; a++)
                    {
                        if (Slosh[a, i] > max)
                        {
                            max = Slosh[a, i];
                            MaxAngle = a;
                        }
                    }

                    #endregion


                    Modes2[i] = mode2 / Divisor2;
                    Maxs2[i] = MaxAngle;

                }

                complex[] FFT = MathFFTHelps.FFT(Modes2);
                int Cutoff = 40;
                for (int i = Cutoff; i < FFT.Length - Cutoff; i++)
                    FFT[i] = new complex(0, 0);
                double[] smoothedFFT = MathFFTHelps.iFFT(FFT);
                smoothedFFT.DivideInPlace(FFT.Length);

                complex[] FFT2 = MathFFTHelps.FFT(Maxs2);
                ScriptingInterface.scriptingInterface.CreateGraph("FFT", FFT2.ConvertToDoubleMagnitude(), "W", "Intensity");
                for (int i = (int)(Cutoff / 3d); i < FFT2.Length - Cutoff / 3d; i++)
                    FFT2[i] = new complex(0, 0);
                double[] smoothedFFT2 = MathFFTHelps.iFFT(FFT2);
                smoothedFFT2.DivideInPlace(FFT2.Length);

                List<double[,]> Lines = new List<double[,]>();
                Lines.Add(Modes2.MakeGraphableArray(0, 1));
                Lines.Add(smoothedFFT.MakeGraphableArray(0, 1));
                Lines.Add(smoothedFFT2.MakeGraphableArray(0, 1));
                Lines.Add(Maxs2.MakeGraphableArray(0, 1));
                ScriptingInterface.scriptingInterface.CreateGraph("Angular Motion", Lines, "Filenumber", "", "Angle");


                double[] SmoothedChoice = smoothedFFT;

                double ave = SmoothedChoice.Average();
                SmoothedChoice.SubtractInPlace(ave);
                SmoothedChoice.MultiplyInPlace(2);
                SmoothedChoice.SubtractInPlace(ave);

                SmoothedChoice.AddInPlace(smoothedFFT2);

                ScriptingInterface.scriptingInterface.CreateGraph("Final Adjustment", SmoothedChoice, "X", "Y");
                int NewSize = (int)(CircleSelection.SelectionBounds.Width * 1);
                Rectangle rect = CircleSelection.SelectionBounds;
                rect.Inflate(NewSize, NewSize);
                string tempPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\";
                for (int i = 0; i < Filenames.Length; i++)
                {
                    float AngleShift = (float)(180 - SmoothedChoice[i]);
                    Bitmap b = MathImageHelps.rotateImage(new Bitmap(Filenames[i]), AngleShift);
                    Bitmap b2 = b.ClipImage(rect);
                    imageSelector.SetBitmap(b2);
                    b2.Save(tempPath + Path.GetFileName(Filenames[i]));
                    Application.DoEvents();
                }
            }
        }

        private void determineRollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = DialogResult.OK;// openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {


                string[] unsortedFilenames = Directory.GetFiles(
@"C:\Users\bashcAdmin\Documents\Visual Studio 2008\Projects\Tomographic_Imaging_MDL\Tomographic_Imaging_2\bin\Debug\TEMP", "*.png");/// openFileDialog1.mFileNames;

                string[] Filenames = MathStringHelps.SortNumberedFiles(unsortedFilenames);

                ImageSelector imageSelector = new ImageSelector();
                TomographicMainForm.ShowDockContent(imageSelector);
                imageSelector.SetFilenames(Filenames);
                imageSelector.SetBitmap(new Bitmap(Filenames[Filenames.Length / 2]));
                imageSelector.WaitForInput("Please set rotation axis");

                ImageViewer.Selections.CircleAndLineSelection CircleLineSelection = (ImageViewer.Selections.CircleAndLineSelection)imageSelector.ActiveSelection;
                double Angle2 = 180 - MathHelps.GetAngleDegrees(CircleLineSelection.LineEnd.X - CircleLineSelection.ImageCenter.X, CircleLineSelection.LineEnd.Y - CircleLineSelection.ImageCenter.Y);


                Bitmap b = MathImageHelps.rotateImage(new Bitmap(Filenames[Filenames.Length / 2]), (float)(Angle2));
                imageSelector.SetBitmap(b);
                imageSelector.WaitForInput("Please draw rotation detection line");
                ImageViewer.Selections.ProfileSelection ProfileSelection = (ImageViewer.Selections.ProfileSelection)imageSelector.ActiveSelection;

                double PixelLength = Math.Sqrt(Math.Pow(ProfileSelection.P1.X - ProfileSelection.P2.X, 2) + Math.Pow(ProfileSelection.P2.Y - ProfileSelection.P1.Y, 2));
                double sx = ProfileSelection.P1.X;
                double sy = ProfileSelection.P1.Y;

                double dx = (ProfileSelection.P2.X - ProfileSelection.P1.X) / PixelLength;
                double dy = (ProfileSelection.P2.Y - ProfileSelection.P1.Y) / PixelLength;

                double[,] Patterns = new double[(int)PixelLength, Filenames.Length];
                PixelLength--;
                for (int i = 0; i < Filenames.Length; i++)
                {
                    Bitmap b2 = MathImageHelps.rotateImage(new Bitmap(Filenames[i]), (float)Angle2);
                    double[,] Intensity = b2.ConvertToDoubleArray(false);
                    imageSelector.SetBitmap(b2);
                    for (int p = 0; p < PixelLength; p++)
                    {
                        Patterns[p, i] = Intensity[(int)(sx + dx * p), (int)(sy + dy * p)];
                    }
                    Application.DoEvents();
                }
                b = Patterns.MakeBitmap();
                imageSelector.SetBitmap(b);
                imageSelector.WaitForInput("Please chose a repeating line for the FFT");

                ProfileSelection = (ImageViewer.Selections.ProfileSelection)imageSelector.ActiveSelection;

                PixelLength = Math.Sqrt(Math.Pow(ProfileSelection.P1.X - ProfileSelection.P2.X, 2) + Math.Pow(ProfileSelection.P2.Y - ProfileSelection.P1.Y, 2));
                sx = ProfileSelection.P1.X;
                sy = ProfileSelection.P1.Y;

                dx = (ProfileSelection.P2.X - ProfileSelection.P1.X) / PixelLength;
                dy = (ProfileSelection.P2.Y - ProfileSelection.P1.Y) / PixelLength;

                double[] Pattern = new double[(int)PixelLength];
                PixelLength--;
                for (int p = 0; p < PixelLength; p++)
                {
                    Pattern[p] = Patterns[(int)(sx + dx * p), (int)(sy + dy * p)];
                }
                complex[] FFT = MathFFTHelps.FFT(Pattern);
                ScriptingInterface.scriptingInterface.CreateGraph("FFT", FFT.ConvertToDoubleMagnitude(), "W", "Intensity");
            }
        }
        private void simpleRotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = DialogResult.OK;// openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                string[] unsortedFilenames = Directory.GetFiles(
@"C:\Users\bashcAdmin\Documents\Visual Studio 2008\Projects\Tomographic_Imaging_MDL\Tomographic_Imaging_2\bin\Debug\TEMP", "p*.png");/// openFileDialog1.mFileNames;

                string[] Filenames = MathStringHelps.SortNumberedFiles(unsortedFilenames);

                ImageSelector imageSelector = new ImageSelector();
                TomographicMainForm.ShowDockContent(imageSelector);
                imageSelector.SetFilenames(Filenames);
                imageSelector.SetBitmap(new Bitmap(Filenames[Filenames.Length / 2]));
                imageSelector.WaitForInput("Please set rotation axis");

                ImageViewer.Selections.CircleAndLineSelection CircleLineSelection = (ImageViewer.Selections.CircleAndLineSelection)imageSelector.ActiveSelection;
                double Angle2 = 180 - MathHelps.GetAngleDegrees(CircleLineSelection.LineEnd.X - CircleLineSelection.ImageCenter.X, CircleLineSelection.LineEnd.Y - CircleLineSelection.ImageCenter.Y);

                string tempPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\";
                for (int i = 0; i < Filenames.Length; i++)
                {
                    Bitmap b2 = MathImageHelps.rotateImage(new Bitmap(Filenames[i]), (float)Angle2);
                    imageSelector.SetBitmap(b2);
                    Application.DoEvents();
                    b2.Save(tempPath + "R" + Path.GetFileName(Filenames[i]));

                }
            }
        }
        private void edgeTrackingInertiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = DialogResult.OK;// openFileDialog1.ShowDialog();
            //DialogResult ret =  openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                string[] unsortedFilenames = Directory.GetFiles("C:\\Users\\bashcAdmin\\Desktop\\New folder (2)\\", "*.png");/// openFileDialog1.mFileNames;
                //string[] unsortedFilenames = Directory.GetFiles(@"C:\Users\bashcAdmin\Documents\Visual Studio 2008\Projects\Tomographic_Imaging_MDL\Tomographic_Imaging_2\bin\Debug\TEMP\", "*.png");/// openFileDialog1.mFileNames;
                string[] Filenames = MathStringHelps.SortNumberedFiles(unsortedFilenames);

                ImageSelector imageSelector = new ImageSelector();
                TomographicMainForm.ShowDockContent(imageSelector);
                imageSelector.SetFilenames(Filenames);

                imageSelector.WaitForInput("Select important feature");
                Rectangle TopHalf = imageSelector.ActiveSelection.SelectionBounds;

                VisionForm vf = new VisionForm();
                TomographicMainForm.ShowDockContent(vf);
                vf.SetFilenames(Filenames);
                vf.WaitForInput("Please set the canny properties correctly");

                Bitmap bb = new Bitmap(Filenames[0]);

                List<VisionHelper.IEdgeFound[]> Edges = new List<VisionHelper.IEdgeFound[]>();
                for (int i = 0; i < Filenames.Length; i++)
                {
                    Bitmap b = new Bitmap(Filenames[i]);
                    Bitmap b2 = b;
                    VisionHelper.IEdgeFound[] FoundEdges = vf.GetEdges(b2);
                    string Filename = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\Edged" + i.ToString() + ".png";
                    vf.ProcessedImage.Save(Filename);
                    Application.DoEvents();
                    Edges.Add(FoundEdges);
                }

                imageSelector.WaitForInput("Select desired edge region");
                VisionHelper.IEdgeFound[][] EdgesFound = VisionTools.LineAngleReporter.RegionSeperator(Edges, TopHalf);

                double[,] AllAngles = VisionTools.LineAngleReporter.CalculateAngles(EdgesFound);

                ScriptingInterface.scriptingInterface.CreateGraph("Quadrants", AllAngles, "Filenumber", "", "Angle(Radians)");

                double[,] Angles = VisionTools.LineAngleReporter.MakeSingleValued(AllAngles);

                ScriptingInterface.scriptingInterface.CreateGraph("QuadrantsClean", Angles, "Filenumber", "", "Angle(Radians)");


                //double[,] Angles = new double[2, 100];
                double[] linearAngles = new double[Angles.GetLength(1)];
                for (int i = 0; i < Angles.GetLength(1); i++)
                {
                    linearAngles[i] = Angles[1, i];
                }

                double[] Smoothed = new double[linearAngles.Length];
                int Count = 0;
                int Period = 5;
                for (int i = 0; i < linearAngles.Length; i++)
                {
                    Count = 0;
                    for (int j = i - Period; j < i + Period; j++)
                    {
                        try
                        {
                            Smoothed[i] += linearAngles[j];
                            Count++;
                        }
                        catch { }
                    }
                    if (Count > 0)
                        Smoothed[i] = Smoothed[i] / Count;
                }
                List<double[,]> LinesA = new List<double[,]>();
                LinesA.Add(linearAngles.MakeGraphableArray(0, 1));
                LinesA.Add(Smoothed.MakeGraphableArray(0, 1));
                ScriptingInterface.scriptingInterface.CreateGraph("SmoothedData", LinesA, "Filenumber", "", "Angle");


                complex[] fftC = MathFFTHelps.FFTreal2complex(linearAngles);
                complex[] fftC2 = new complex[fftC.Length];
                ScriptingInterface.scriptingInterface.CreateGraph("FFTcomplex2complex", fftC.ConvertToDoubleMagnitude(), "", "");

                string sFreq = UseableInputBox.Show("Desired Frequency").Wait();
                int Freq = 10;
                int.TryParse(sFreq, out Freq);
                for (int i = 0; i < fftC.Length; i++)
                {
                    if (i == Freq || i == 0)
                        fftC2[i] = fftC[i];
                }

                bool Finished = false;

                double[] fftAngles = MathFFTHelps.iFFTcomplex2real(fftC2);

                fftAngles = fftAngles.DivideToArray((double)fftAngles.Length);

                List<double[,]> Lines = new List<double[,]>();
                Lines.Add(linearAngles.MakeGraphableArray(0, 1));
                Lines.Add(fftAngles.MakeGraphableArray(0, 1));
                ScriptingInterface.scriptingInterface.CreateGraph("FFTAngles", Lines, "", "", "");

                string tempPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\";

                double midPointSmooth = Smoothed.Average();
                double midPointLA = linearAngles.Average();
                double sumLA = 0;
                double sumS = 0;
                for (int i = 0; i < linearAngles.Length; i++)
                {
                    sumLA += Math.Pow((linearAngles[i] - midPointLA), 2);
                    sumS += Math.Pow((Smoothed[i] - midPointSmooth), 2);
                }

                double ScaleFactor = Math.Sqrt(sumLA / sumS);

                double[] UseAngles = new double[fftAngles.Length];

                while (!Finished)
                {
                    string retS = UseableInputBox.Show("Desired Scale").Wait();
                    if (retS == "")
                    {
                        Finished = true;
                        break;
                    }
                    double.TryParse(retS, out ScaleFactor);
                    for (int i = 0; i < fftAngles.Length; i++)
                    {
                        UseAngles[i] = (Smoothed[i] - midPointLA) * ScaleFactor + midPointLA;
                    }
                    for (int i = 0; i < Filenames.Length; i++)
                    {
                        Bitmap b = MathImageHelps.rotateImage(new Bitmap(Filenames[i]), (float)((Math.PI - UseAngles[i]) / Math.PI * 180));
                        Bitmap b2 = b.ClipImage(TopHalf);
                        imageSelector.SetBitmap(b2);
                        Application.DoEvents();
                    }
                }
                for (int i = 0; i < Filenames.Length; i++)
                {
                    Bitmap b = MathImageHelps.rotateImage(new Bitmap(Filenames[i]), (float)((Math.PI - UseAngles[i]) / Math.PI * 180));
                    Bitmap b2 = b.ClipImage(TopHalf);
                    imageSelector.SetBitmap(b2);
                    b2.Save(tempPath + Path.GetFileName(Filenames[i]));
                    Application.DoEvents();
                }
            }
        }*/
        #endregion

        private void closeGraphWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptingInterface.scriptingInterface.CloseAllGraphWindows();
        }

      

        private void testYConvolutionChopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] Data = new double[450, 251];
            for (int j = 0; j < Data.GetLength(0); j++)
                for (int m = 0; m < Data.GetLength(1); m++)
                {
                    if (j > 100 && j < 200 && m > 100 && m < 200)
                        Data[j, m] = 255;
                }
            PhysicalArray pa = new PhysicalArray(Data, -1, 1, -1, 1, false);
            ScriptingInterface.scriptingInterface.CreateGraph("ImageData", pa);
            //pa.SaveData(@"C:\Development\CellCT\Tomographic_Imaging_MDL\test.bmp");
            double[] impulse = Filtering.GuassianApproximation_SeperableRealSpaceFilter(512,512, 1d / 100d);
            ScriptingInterface.scriptingInterface.CreateGraph("Impulse", impulse, "X", "y");
            PhysicalArray ca = pa.ConvoluteChopSeperable(Axis.XAxis, Axis.YAxis, impulse);
            ScriptingInterface.scriptingInterface.CreateGraph("Convoluted", ca);

        }

        private void loadImageAndConvoluteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // DialogResult ret = openFileDialog1.ShowDialog();
            // if (ret == DialogResult.OK)
            {
                //string[] FileNames = openFileDialog1.FileNames;
                string[] FileNames = { "C:\\Development\\CellCT\\Flo1CorrectedPP_cct001_20100511_150225\\corrpp_blue000.png" };
                FileNames = MathStringHelps.SortNumberedFiles(FileNames);

                ImageSelector selector = new ImageSelector();
                selector.SetFilenames(FileNames);
                //selector.Show();
                Bitmap b3 = MathImageHelps.LoadBitmap(FileNames[0], false);
                selector.SetBitmap(b3);

                double dWidth = b3.Width;
                double dHeight = b3.Height;
                selector.WaitForInput("Please select the interesting projection area", new ImageViewer.Tools.ROITool());

                ImageViewer.Selections.ROISelection ROI = (ImageViewer.Selections.ROISelection)selector.ActiveSelection;
                //ImageViewer.Selections.ROISelection ROI = new ImageViewer.Selections.ROISelection(new Rectangle(53, 150, 139, 147));

                Bitmap b4 = b3.ClipImage(ROI.SelectionBounds);

                double[,] ImageArray =  MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(b4,false);
                ImageArray.InvertArrayValuesInPlace();
                ImageArray = MathImageHelps.FlattenImageEdges(ImageArray);
                //double[,] impulse = Filtering.RectangularRealSpaceFilter2D(128, 10000, 1); //Filtering.Ramachandran_Lakshminarayanan_RealSpacefilter2D(512, 2.0 / 256.0);
                double[,] impulse = Filtering.SincRealSpaceFilter2D(512, 1);

                PhysicalArray dataPA = new PhysicalArray(ImageArray, -1, 1, -1, 1, true);
                ScriptingInterface.scriptingInterface.CreateGraph("Real ImageData", dataPA);
                // dataPA = dataPA.ConvoluteReal2D(Axis.XAxis, Axis.YAxis, impulse);
                ScriptingInterface.scriptingInterface.CreateGraph("DataPA_Convolution Real", dataPA);

                //double[,] oArray= 
                dataPA = dataPA.ConvoluteFFT2D(Axis.XAxis, Axis.YAxis, impulse, true);
                //ScriptingInterface.scriptingInterface.CreateGraph("DataPAFullSize", oArray, "X", "", "Y");
                ScriptingInterface.scriptingInterface.CreateGraph("ImageData PA_ConvolutionFFT", dataPA);
            }
        }

        private void profileLineTestorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[, ,] TestMatrix = new double[100, 110, 95];
            for (int i = 0; i < TestMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < TestMatrix.GetLength(1); j++)
                    for (int m = 0; m < TestMatrix.GetLength(2); m++)
                        TestMatrix[i, j, m] = i + j + m;
            }
            ScriptingInterface.scriptingInterface.CreateGraph("TestProfile", TestMatrix, "", "", "", "");
            TestMatrix = null;
        }

        private void projectionProcessorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EasyImageViewer eiv = new EasyImageViewer();
            TomographicMainForm.ShowDockContent(eiv);
        }

        private void checkAllPaddingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = @"C:\Development\CellCT\Flo1RawPP_cct001_20100511_150225\Corrected\Corrected0.bmp";
            double[,] impulse = Filtering.SincRealSpaceFilter2D(256, 1);

            ScriptingInterface.scriptingInterface.CreateGraph("Impulse Function", impulse, "X", "Intensity");
            Bitmap test = new Bitmap(filename);

            int dWidth = test.Width;
            int dHeight = test.Height;
            int PaddedSize;
            if (dWidth > dHeight)
                PaddedSize = (int)MathHelps.NearestPowerOf2(2 * dWidth);
            else
                PaddedSize = (int)MathHelps.NearestPowerOf2(2 * dHeight);

            ProjectionSliceFile psf = new ProjectionSliceFile();
            psf.PersistDataInMemory = true;
            psf.LoadFile(filename, 0, 2d * dHeight / dWidth, 2d, false);

            ScriptingInterface.scriptingInterface.CreateGraph("Original", psf.Projection);
            PhysicalArray ProjectionT = psf.Projection.ZeroPad_DataCentered(Axis.XAxis, PaddedSize);
            ProjectionT = ProjectionT.ZeroPad_DataCentered(Axis.YAxis, PaddedSize);

            ScriptingInterface.scriptingInterface.CreateGraph("Padded", ProjectionT);

            PhysicalArray tBackProjection = ProjectionT.ConvoluteFFT2D(Axis.XAxis, Axis.YAxis, impulse, true);

            ScriptingInterface.scriptingInterface.CreateGraph("Convoluted", tBackProjection);

            tBackProjection.TruncateDataInPlace(Axis.XAxis, psf.Projection.PhysicalStart(Axis.XAxis), psf.Projection.PhysicalEnd(Axis.XAxis));
            ScriptingInterface.scriptingInterface.CreateGraph("ChoppedX", tBackProjection);

            tBackProjection.TruncateDataInPlace(Axis.YAxis, psf.Projection.PhysicalStart(Axis.YAxis), psf.Projection.PhysicalEnd(Axis.YAxis));

            ScriptingInterface.scriptingInterface.CreateGraph("Chopped", tBackProjection);
        }

        
        
        private void checkProjectionIntegratedSumsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                string[] FileNames = openFileDialog1.FileNames;
                FileNames = MathStringHelps.SortNumberedFiles(FileNames);
                double SliceDegree = 360d / (double)FileNames.Length;
                double[,] Datas = new double[2, FileNames.Length];
                for (int i = 0; i < FileNames.Length; i++)
                {
                    Datas[0, i] = i * SliceDegree;
                    Datas[1, i] = MathHelpLib.ImageProcessing.MathImageHelps.SumImage(new Bitmap(FileNames[i]));
                    toolStripProgressBar1.Value = (int)((double)(i + 1) / (double)FileNames.Length * 100d);
                    Application.DoEvents();
                }
                ScriptingInterface.scriptingInterface.CreateGraph("Intensity Graph", Datas, "Projection Angle", "Average Intensity");
            }
        }

        private void checkNormalizationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                string[] FileNames = openFileDialog1.FileNames;
                FileNames = MathStringHelps.SortNumberedFiles(FileNames);
                double SliceDegree = 360d / (double)FileNames.Length;

                Bitmap b = new Bitmap(FileNames[0]);
                double dWidth = b.Width;
                double dHeight = b.Height;

                double[,] Datas = new double[2, FileNames.Length];
                for (int i = 0; i < FileNames.Length; i++)
                {
                    ProjectionSliceFile psf = new ProjectionSliceFile();

                    psf.PersistDataInMemory = false;
                    psf.LoadFile(FileNames[i], SliceDegree * i, 2d * dHeight / dWidth, 2d, false);
                    PhysicalArray pa = psf.Projection;
                    double[, ,] array = pa.ReferenceDataDouble;
                    array.NormalizeArraySumInPlace(400);
                    psf.Projection = pa;

                    Datas[0, i] = SliceDegree * i;
                    Datas[1, i] = psf.Projection.ReferenceDataDouble.SumArray() / (double)(array.GetLength(0) * array.GetLength(1));
                    ScriptingInterface.scriptingInterface.CreateGraph("ImageTest", psf.Projection);
                }
                ScriptingInterface.scriptingInterface.CreateGraph("Normalized Images", Datas, "Angles", "Intensity");

            }
        }

        private void sARTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            DialogResult ret = openFileDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                string[] FileNames = openFileDialog1.FileNames;
                FileNames = MathStringHelps.SortNumberedFiles(FileNames);
                double SliceDegree = 360d / (double)FileNames.Length;

                double[,] impulse = Filtering.SincRealSpaceFilter2D(256, 1);

                ScriptingInterface.scriptingInterface.CreateGraph("Impulse Function", impulse, "X", "Intensity");
                
                Bitmap b = new Bitmap(FileNames[0]);
                double dWidth = b.Width;
                double dHeight = b.Height;
                b = null;
                List<aProjectionSlice> Slices = new List<aProjectionSlice> ();
                string TempFilePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\";
                
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
               
                for (int i = 0; i < FileNames.Length; i+=1)
                {
                    ProjectionSliceFile psf = new ProjectionSliceFile();
                    psf.PersistDataInMemory = false;
                    psf.LoadFile(FileNames[i], SliceDegree * i, 2d * dHeight / dWidth/*Math.Sqrt(2)*/, 2d/*Math.Sqrt(2)*/, false);
                    Slices.Add ( psf);
                    ScriptingInterface.scriptingInterface.CreateGraph("ImageTest", psf.Projection);
                }

                ProjectionObject po = ScriptingInterface.scriptingInterface.CreateProjectionObject((int)(dWidth*1) , 2, 2, 2);
                ScriptingInterface.scriptingInterface.MakeVariableVisible("ProjectionOject", po);

                aProjectionSlice apS = Slices[0];
                int Width = apS.Projection.GetLength(Axis.XAxis);
                int Height = apS.Projection.GetLength(Axis.YAxis);
                int PaddedSize;
                if (Width > Height)
                    PaddedSize = (int)MathHelps.NearestPowerOf2(Width);
                else
                    PaddedSize = (int)MathHelps.NearestPowerOf2(Height);

                //po.DoBackProjection_ThreadedMediumMemory(PaddedSize, Slices.ToArray(), impulse, ConvolutionMethod.NoConvolution );
                long runtime = sw.ElapsedMilliseconds;
                LoggerForm.LogMessage("Completed in " + runtime.ToString() + "ms or " + ((double)runtime / 1000d).ToString());
                sw.Stop();

               // ScriptingInterface.scriptingInterface.CreateGraph("ProjectionOutput", po.ProjectionData);
                po.AddSlicesRange(Slices);
                po.DoARTInitialized(3);

               ScriptingInterface.scriptingInterface.CreateGraph("ProjectionOutput", po.ProjectionData);

            }
        }

        private void createPhantomToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

     

        private void gPUConvolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float[,] Data = new float[256, 256];
            double half = Data.GetLength(0) / 2d;
            double Radius = Data.GetLength(0) / 4d;
            Radius *= Radius;
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    if ((i - half) * (i - half) + (j - half) * (j - half) < Radius)
                    {
                        Data[i, j] = (float)10;// Math.Sqrt(Radius - (i - half) * (i - half));
                    }
                }
            }

            ScriptingInterface.scriptingInterface.CreateGraph("Before", Data.MakeBitmap());

            double[] impulse = Filtering.Ramachandran_Lakshminarayanan_RS_RadonFilter(512, 1);
            float[] fImpulse = impulse.ConvertToFloat();
            ScriptingInterface.scriptingInterface.CreateGraph("ImpulseFFT", impulse, "X", "Y");

       
        }

        private void fFTTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] D = new double[100, 100];
            Random rnd = new Random();
            for (int i = 0; i < D.GetLength(0); i++)
                for (int j = 0; j < D.GetLength(1); j++)
                    D[i, j] = rnd.NextDouble();



            D = MathHelpLib.MathFFTHelps.FFTreal2real(D);
         
            ScriptingInterface.scriptingInterface.CreateGraph("Random", D, "X", "Y", "Z");
            int MinR = D.GetLength(0) / 4;
            double sum = 0;
            double cc = 0;
            for (int i = 0; i < D.GetLength(0) / 2; i++)
                for (int j = 0; j < D.GetLength(1) / 2; j++)
                {

                    if ((i + j) > MinR)
                    {
                        sum += D[i, j];
                        cc++;
                    }
                }
        }

        private void beatrizToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] FileNames2 = Directory.GetFiles(@"C:\Development\CellCT\DataIN\cct001_20090916_115617\STACK\000");
            string junk="";

            string[] Files = new string[FileNames2.Length];

            double m = Math.Floor((double)(FileNames2.Length / 2));
            int r = 0;
            while (r < FileNames2.Length)
            {
                Files[(int)m] = FileNames2[r];
                r++;
                m += Math.Pow((-1), r + 1) * r;
            }

            foreach (string file in Files)
                junk += Path.GetFileName( file) + ",";
            Clipboard.SetText(junk);
        }

        








    }
}