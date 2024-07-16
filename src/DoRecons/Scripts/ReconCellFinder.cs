using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathHelpLib;
using ImageViewer;
using System.Drawing;
using MathHelpLib.ProjectionFilters;
using System.Threading;
using ImageViewer.PythonScripting.Projection;
using System.Windows.Forms;

namespace DoRecons.Scripts
{
    public class ReconCellFinder
    {

        private  static double DoRemoveFBPCylinder(ref double[, ,] Data)
        {
            double RX = Data.GetLength(0) / 2d - 2;
            double RY = Data.GetLength(1) / 2d - 2;
            double RZ = Data.GetLength(2) / 2d - 2;
            double HalfI = Data.GetLength(0) / 2d;
            double HalfJ = Data.GetLength(1) / 2d;
            double HalfK = Data.GetLength(2) / 2d;

            double x;
            double y;
            double z;

            double min = double.MaxValue;
            double val;
            double sum = 0, count = 0;
            List<double> medianList = new List<double>();
            for (int i = 0; i < Data.GetLength(0); i++)
                for (int j = 0; j < Data.GetLength(1); j++)
                    for (int k = 0; k < Data.GetLength(2); k++)
                    {
                        x = (i - HalfI);
                        y = RX * (j - HalfJ) / RY;
                        z = RX * (k - HalfK) / RZ;
                        if (RX - (x * x + y * y) < 2)
                        {
                            val = Data[i, j, k];
                            if (val < min) min = val;
                            sum += val;
                            medianList.Add(val);
                            count++;
                        }
                    }

            medianList.Sort();
            sum = medianList[(int)(medianList.Count * 5f / 8f)];
            // sum /= count;
            if (sum < -500) sum = -500;
            // sum = min;
            sum = 0;
            //  double sum = -500;
            for (int i = 0; i < Data.GetLength(0); i++)
                for (int j = 0; j < Data.GetLength(1); j++)
                    for (int k = 0; k < Data.GetLength(2); k++)
                    {
                        x = (i - HalfI) / RX;
                        y = (j - HalfJ) / RY;
                        z = (k - HalfK) / RZ;
                        if ((x * x + y * y + z * z) > 1 || Data[i, j, k] < 0)
                        {
                            Data[i, j, k] = sum;
                        }
                    }

            return sum;
        }

        public  static ImageHolder ClipImage(DataEnvironment dataEnvironment, int ImageI, double[] X_Positions, double[] Y_Positions, int CellHalf, int CellSize)
        {
            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageI];

            Rectangle CellArea = new Rectangle((int)Math.Truncate(X_Positions[ImageI] - CellHalf), (int)Math.Truncate(Y_Positions[ImageI] - CellHalf), CellSize, CellSize);

            BitmapImage = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(BitmapImage, CellArea);
            BitmapImage = ImageViewer.Filters.Adjustments.InvertEffect.InvertImage(BitmapImage);

            BitmapImage.ImproveMinimum(.1);

            return BitmapImage;
        }

        public static void ReconCenter(DataEnvironment dataEnvironment, int CellSize, double[] X_Positions, double[] Y_Positions)
        {
            if (CellSize > 200) CellSize = 200;
            CellSize *= 2;
            int CellHalf = CellSize / 2;

            ProjectionArrayObject DensityGrid = new ProjectionArrayObject(true, CellSize, CellSize, CellSize, -1, 1, -1, 1, -1, 1);

            // ConvolutionMethod DesiredMethod = ConvolutionMethod.Convolution1D;

            ImageViewer.PythonScripting.Projection.Convolution1D ConvolutionFilter = new ImageViewer.PythonScripting.Projection.Convolution1D();

            int FilterWidth = 256;//, EffectHelps.ConvertToInt(ParamDictionary["FBPResolution"])
            double[] impulse = Filtering.GetRealSpaceFilter("Han", FilterWidth, FilterWidth, (double)FilterWidth / 2d);

            Thread[] Recons = new Thread[31];
            for (int i = 0; i < Recons.Length; i++)
            {
                Recons[i] = new Thread(delegate(object ImageN)
                {
                    try
                    {
                        int ImageI = (int)ImageN;
                        ImageHolder BitmapImage = ClipImage(dataEnvironment, ImageI, X_Positions, Y_Positions, CellHalf, CellSize);

                        double[,] Slice = null;
                        Slice = ConvolutionFilter.DoConvolution(dataEnvironment, BitmapImage, impulse, 2, 8);
                        // Slice = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(Slice, ReconCutDownRect);
                        double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * ImageI;
                        Bitmap bbi = MathHelpLib.ImageProcessing.MathImageHelps.MakeBitmap(Slice);
                        int ghj = bbi.Width;
                        ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2 FilterR = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2();
                        FilterR.DoEffect(dataEnvironment, null, null, Slice, DensityGrid, AngleRadians);
                    }
                    catch { }
                });
            }

            double Gap = (double)(dataEnvironment.AllImages.Count) / (double)Recons.Length;
            for (int i = 0; i < Recons.Length; i++)
            {
                Recons[i].Start((int)(i * Gap));
            }

            for (int i = 0; i < Recons.Length; i++)
                Recons[i].Join();

            DoRemoveFBPCylinder(ref DensityGrid.DataWhole);

            /*  Bitmap br = DensityGrid.DataWhole.MakeBitmap(DensityGrid.DataWhole.GetLength(0) / 2);
              br.Save("c:\\temp\\p1.bmp");
              br = DensityGrid.DataWhole.MakeBitmapPerp(DensityGrid.DataWhole.GetLength(0) / 2);
              br.Save("c:\\temp\\p2.bmp");
              br = DensityGrid.DataWhole.MakeBitmapPerpX(DensityGrid.DataWhole.GetLength(0) / 2);
              br.Save("c:\\temp\\p3.bmp");*/

            double AngleStep = 360d / Recons.Length;

            Bitmap[] Ideals = MakeMIPMovie3Effect.DoMIPProjection("", "", DensityGrid.DataWhole, AngleStep);


            DensityGrid.DataWhole = null;
            DensityGrid = null;
            GC.Collect();
            ImageHolder[] IdealsH = new ImageHolder[Ideals.Length];
            for (int i = 0; i < Ideals.Length; i++)
            {
                IdealsH[i] = new ImageHolder(Ideals[i]);
                IdealsH[i].ImproveMinimum(.2);
                IdealsH[i] = ImageViewer.Filters.Effects.Blurs.GaussianBlurTool.DoGuassSmooth(IdealsH[i], 5);
            }

            for (int i = 0; i < Ideals.Length; i++)
            {
                //  IdealsH[i].ToBitmap().Save("c:\\temp\\m" + i.ToString() + ".bmp");
            }

            Thread[] PPs = new Thread[dataEnvironment.AllImages.Count];

            AngleStep = AngleStep / 180d * Math.PI;

            for (int j = 0; j < dataEnvironment.AllImages.Count; j++)
            {
                PPs[j] = new Thread(delegate(object index)
                {
                    try
                    {
                        int i = (int)index;

                        double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * i + 2d * Math.PI / 4d;
                        int ImageIndex = ((int)Math.Floor(AngleRadians / AngleStep)) % IdealsH.Length;
                        int Image2 = ((int)Math.Floor(AngleRadians / AngleStep) + 1) % IdealsH.Length;
                        double u = Math.Abs((AngleRadians - Math.Floor(AngleRadians / AngleStep) * AngleStep) / AngleStep);

                        ImageHolder CompareImage = ImagingTools.BlendImages(IdealsH[ImageIndex], IdealsH[Image2], (float)u);

                        ImageHolder originalImage = ClipImage(dataEnvironment, i, X_Positions, Y_Positions, CellHalf, CellSize);

                        if (i % 30 == 0)
                        {
                            /*   // originalImage.ImproveMinimum(.1);
                               Bitmap b1 = CompareImage.ToBitmap();
                               Bitmap b2 = originalImage.ToBitmap();

                               b1.Save("c:\\temp\\m" + i.ToString() + ".bmp");
                               b2.Save("c:\\temp\\mC" + i.ToString() + ".bmp");*/
                        }
                        PointF shift = MathHelpLib.MathFFTHelps.FindShift(originalImage.ImageData, CompareImage.ImageData);

                        //  if (Math.Abs(shift.X) < 10 && Math.Abs(shift.Y) < 10)
                        {
                            X_Positions[i] += shift.X;
                            Y_Positions[i] += shift.Y;
                        }

                        //int gh = b1.Width + b2.Width;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                    }
                });
            }
            int cc = 0;
            int sCC = 0;
            for (int i = 0; i < dataEnvironment.AllImages.Count; i += 50)
            {
                sCC = cc;
                for (int j = 0; j < 30; j++)
                {
                    PPs[cc].Start(cc);
                    cc++;
                }
                for (int j = sCC; j < cc; j++)
                {
                    PPs[j].Join();
                }
                GC.Collect();
            }
            for (; cc < dataEnvironment.AllImages.Count; cc++)
            {
                PPs[cc].Start(cc);
            }

            for (int i = 0; i < PPs.Length; i++)
                PPs[i].Join();

        }


    }
}
