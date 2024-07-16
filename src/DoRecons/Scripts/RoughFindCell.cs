using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ImageViewer;
using MathHelpLib;
using ImageViewer.PythonScripting;
using ImageViewer.PythonScripting.Threads;
using MathHelpLib.ProjectionFilters;
using ImageViewer.PythonScripting.Projection;
using ImageViewer.Filters.Blobs;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace DoRecons.Scripts
{

    public class RoughFindCell
    {
        object StoreBlobsLockObject = new object();
        private BlobDescription[][] mBlobs = null;

        public void StoreBlobLocation(DataEnvironment dataEnviroment, int ImageNumber, BlobDescription[] AllFoundBlobs, double Expander, Rectangle CellWanderArea)
        {
            lock (StoreBlobsLockObject)
            {
                if (mBlobs == null)
                    mBlobs = new BlobDescription[dataEnviroment.AllImages.Count][];
            }

            BlobDescription[] Blobs = new BlobDescription[AllFoundBlobs.Length];

            for (int i = 0; i < Blobs.Length; i++)
            {
                //expand the info and then move it to image coords
                Point origP = AllFoundBlobs[i].CenterOfGravity;
                Point p = new Point((int)(origP.X * Expander + CellWanderArea.X), (int)(origP.Y * Expander + CellWanderArea.Y));
                Rectangle orignal = AllFoundBlobs[i].BlobBounds;
                Rectangle r = new Rectangle((int)(orignal.X * Expander + CellWanderArea.X), (int)(orignal.Y * Expander + CellWanderArea.Y), (int)(orignal.Width * Expander), (int)(orignal.Height * Expander));
                BlobDescription bd = new BlobDescription(AllFoundBlobs[i].BlobIndex, r, p);
                Blobs[i] = bd;
            }

            mBlobs[ImageNumber] = Blobs;
        }


        private bool FirstFound = false;
        private int RoughCellSize;

        private int FindBlobs(ImageHolder BitmapImage, CellPositions RoughLocations, DataEnvironment dataEnvironment, int ImageNumber, Rectangle findArea, int Decimate, Rectangle SuggestedFindPosition)
        {
            #region Find Cell
            ImageHolder ThreshImage = null;
            BlobDescription[] Blobs = null;
            BlobDescription MaxBlob = null;


           // Bitmap b = BitmapImage.ToBitmap();
           // int w = b.Width;

            int MidPoint = BitmapImage.Width / 2;
            if (SuggestedFindPosition != Rectangle.Empty)
            {
                MidPoint = SuggestedFindPosition.X;
            }

            try
            {
                ThreshImage = ImageViewer.Filters.Thresholding.IterativeThresholdEffect.IterativeThreshold(BitmapImage);
                Blobs = ImageViewer.Filters.Blobs.WaterShedTool.DoWatershed(ThreshImage);
                MaxBlob = ImageViewer.Filters.Blobs.GetBiggestCenterBlob.SortBlobsCenter(Blobs, ThreshImage.Width, ThreshImage.Height, 25, 400, out Blobs, MidPoint);
            }
            catch
            { }

            //try again to find the blob
            if (MaxBlob == null)
            {
                ThreshImage = ImageViewer.Filters.Thresholding.OtsuThresholdEffect.OtsuThreshold(BitmapImage);
                Blobs = ImageViewer.Filters.Blobs.WaterShedTool.DoWatershed(ThreshImage);
                MaxBlob = ImageViewer.Filters.Blobs.GetBiggestCenterBlob.SortBlobsCenter(Blobs, ThreshImage.Width, ThreshImage.Height, 25, 400, out Blobs, MidPoint);
            }

          //  b = ThreshImage.ToBitmap();
           // w = b.Width;
            #endregion

            #region Process Location

            int x;
            int y;
            if (MaxBlob != null)
            {
                try
                {
                    BlobDescription Rect = MaxBlob;
                    x = Rect.CenterOfGravity.X * Decimate;
                    y = Rect.CenterOfGravity.Y * Decimate;

                    if (Rect.BlobBounds.Width > Rect.BlobBounds.Height)
                        RoughLocations.CellSizes[ImageNumber] = Rect.BlobBounds.Width * Decimate;
                    else
                        RoughLocations.CellSizes[ImageNumber] = Rect.BlobBounds.Height * Decimate;
                    //CellSizes[ImageNumber] = (Rect.BlobBounds.Width * Expander + Rect.BlobBounds.Height * Expander) / 2d;
                }
                catch
                {
                    //indicate that the cell was not found
                    x = int.MinValue;
                    y = int.MinValue;
                }
            }
            else
            {
                //indicate that the cell was not found
                x = int.MinValue;
                y = int.MinValue;
            }

            #endregion

            this.StoreBlobLocation(dataEnvironment, ImageNumber, (BlobDescription[])Blobs, Decimate, findArea);

            if (x != int.MinValue)
            {
                RoughLocations.X_Positions[ImageNumber] = x + findArea.Left;
                RoughLocations.Y_Positions[ImageNumber] = y + findArea.Top;
                RoughLocations.NumBlobs[ImageNumber] = Blobs.Length;
            }
            else
            {
                RoughLocations.X_Positions[ImageNumber] = x;
                RoughLocations.Y_Positions[ImageNumber] = y;
                RoughLocations.NumBlobs[ImageNumber] = 0;
            }

            return x;
        }

        private ImageHolder FindSuggestedRough(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage, CellPositions RoughLocations)
        {
            try
            {
                int halfWidth = (int)(RoughLocations.CellSizes[ImageNumber] / 2);
                Rectangle findArea = new Rectangle((int)RoughLocations.X_Positions[ImageNumber] - 20 - halfWidth, (int)(RoughLocations.Y_Positions[ImageNumber] - halfWidth), 2 * halfWidth + 40, 2 * halfWidth);
                try
                {

                    findArea.Inflate(40, 40);
                    BitmapImage = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(BitmapImage, findArea);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show(ex.StackTrace);
                }

                Bitmap b = BitmapImage.ToBitmap();

                int w = b.Width;

                int x = FindBlobs(BitmapImage, RoughLocations, dataEnvironment, ImageNumber, findArea, 1, findArea);


                if (x != int.MinValue)
                    return BitmapImage;
                else
                    return null;

            }
            catch (Exception ex)
            {

                dataEnvironment.ProgressLog.AddSafe("Imagerough", "error");
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
                throw ex;
            }

        }

        public ImageHolder ProcessImageFindRough(DataEnvironment dataEnvironment, int ImageNumber, ImageHolder BitmapImage, CellPositions RoughLocations, int Decimate, Rectangle CellWanderArea, Rectangle SuggestedLocation)
        {
            // Bitmap b = BitmapImage.ToBitmap();
            ImageHolder suggestedImage = null;
            if (FirstFound)
                suggestedImage = FindSuggestedRough(dataEnvironment, ImageNumber, BitmapImage, RoughLocations);

            if (suggestedImage != null)
                return suggestedImage;


            try
            {
                try
                {
                    //   b = BitmapImage.ToBitmap();
                    BitmapImage = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(BitmapImage, CellWanderArea);
                    //   b = BitmapImage.ToBitmap();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show(ex.StackTrace);
                }

                //this creates a new image and nothing else effects the original
                if (FirstFound == true)
                    Decimate = 1;
                if (Decimate == 2)
                {
                    //  b = BitmapImage.ToBitmap();
                    //reduce size
                    BitmapImage = ImageViewer.Filters.Adjustments.downSampleEffect.DownSampleImage(BitmapImage);

                }
                else if (Decimate == 4)
                {
                    BitmapImage = ImageViewer.Filters.Adjustments.downSampleEffect.DownSampleImage(BitmapImage);
                    BitmapImage = ImageViewer.Filters.Adjustments.downSampleEffect.DownSampleImage(BitmapImage);

                }
                Rectangle adjustedSuggestedLocation;
                if (SuggestedLocation.Width == CellWanderArea.Width)
                {
                    adjustedSuggestedLocation = CellWanderArea;

                }
                else
                {
                    adjustedSuggestedLocation = new Rectangle((SuggestedLocation.X - 200) / Decimate, CellWanderArea.Top, (SuggestedLocation.Width + 400) / Decimate, CellWanderArea.Height / Decimate);
                }
                FindBlobs(BitmapImage, RoughLocations, dataEnvironment, ImageNumber, CellWanderArea, Decimate, adjustedSuggestedLocation);

                return BitmapImage;

            }
            catch (Exception ex)
            {

                dataEnvironment.ProgressLog.AddSafe("Imagerough", "error");
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
                throw ex;
            }
        }

        // the problem is that hthere can be multiple cells and debry in the field of view.  
        public Rectangle FindBestCellCenter(DataEnvironment dataEnvironment, int[] Indexs, Rectangle firstGuess, bool EstimateCellSize, ref  CellPositions RoughLocations, out int roughCellSize)
        {
            BlobDescription[] BestCells = new BlobDescription[Indexs.Length];

            int MidPoint;
            try
            {
                MidPoint = dataEnvironment.AllImages[1].Width / 2;
            }
            catch
            {
                ImageHolder ih = dataEnvironment.AllImages[1];
                Thread.Sleep(100);
                MidPoint = ih.Width / 2;
            }
            int MinWidth = 75;
            Point lastLocation = Point.Empty;
            if (firstGuess != Rectangle.Empty)
            {
                MidPoint = firstGuess.X;// -firstGuess.Width / 2;
                MinWidth = (int)(firstGuess.Width * .8);
                lastLocation = new Point(firstGuess.X, firstGuess.Y);
            }

            int MinX = dataEnvironment.AllImages[1].Width, MaxX = 0, minY = dataEnvironment.AllImages[1].Height, maxY = 0;

            Console.WriteLine(MidPoint.ToString());

            List<int> Widths = new List<int>();
            //find the cell in each index by checking if it is close to the center or the vg defined center
            int goodCount = 0;
            for (int i = 0; i < Indexs.Length; i++)
            {
                int imageIndex = Indexs[i];
                if (mBlobs[imageIndex] != null)
                {
                    int MinDif = int.MaxValue;
                    if (mBlobs[imageIndex].Length > 0 && mBlobs[imageIndex][0] != null)
                    {
                        BlobDescription bestBlob = mBlobs[imageIndex][0];
                        string junk2 = "";
                        for (int bN = 0; bN < mBlobs[imageIndex].Length; bN++)
                        {
                            int distance;
                            if (lastLocation != Point.Empty)
                                distance = (int)Math.Sqrt(Math.Pow(mBlobs[imageIndex][bN].CenterOfGravity.X - lastLocation.X, 2) + Math.Pow(mBlobs[imageIndex][bN].CenterOfGravity.Y - lastLocation.Y, 2));
                            else
                                distance = Math.Abs(MidPoint - mBlobs[imageIndex][bN].CenterOfGravity.X);

                            if (distance < MinDif && (mBlobs[imageIndex][bN].BlobBounds.Width) > MinWidth)
                            {
                                MinDif = distance;
                                bestBlob = mBlobs[imageIndex][bN];
                            }
                            junk2 += (mBlobs[imageIndex][bN].BlobBounds.ToString()+ " \n");
                        }

                        if (i<12)
                            Console.WriteLine(junk2 + "---\n" + bestBlob.BlobBounds.ToString() + "\n\n");

                        lastLocation.X = bestBlob.CenterOfGravity.X;
                        lastLocation.Y = bestBlob.CenterOfGravity.Y;
                        // Bitmap bb = dataEnvironment.AllImages[Indexs[i]].ToBitmap();
                        //   Graphics g = Graphics.FromImage(bb);
                        //  g.DrawRectangle(Pens.Red, bestBlob.BlobBounds);

                        //int www = bb.Width;
                        BestCells[i] = bestBlob;

                        

                        Widths.Add(bestBlob.BlobBounds.Width);


                        int mX = bestBlob.CenterOfGravity.X - bestBlob.BlobBounds.Width / 2;
                        int mY = bestBlob.CenterOfGravity.Y - bestBlob.BlobBounds.Height / 2;
                        int Mx = bestBlob.CenterOfGravity.X + bestBlob.BlobBounds.Width / 2;
                        int My = bestBlob.CenterOfGravity.Y + bestBlob.BlobBounds.Height / 2;

                        if (mX < MinX)
                            MinX = mX;
                        if (mY < minY)
                            minY = mY;
                        if (Mx > MaxX)
                            MaxX = Mx;
                        if (My > maxY)
                            maxY = My;
                        goodCount++;
                    }
                }
            }
            if (goodCount == 0)
                throw new Exception("Unable to find any cells");
            Widths.Sort();
            int likelyWidth = Widths[(int)(Widths.Count / 4d * 3d)];

           // if (firstGuess != Rectangle.Empty)
           //     likelyWidth = firstGuess.Width/2;

            #region OldMethod
            /*
            double[,] XData = new double[2, goodCount * 3];
            double[,] YData = new double[2, goodCount * 3];
            int cc = 0;
            for (int i = 0; i < Indexs.Length; i++)
            {
                if (BestCells[i] != null)
                {
                    XData[0, cc] = i - Indexs.Length;
                    XData[1, cc] = BestCells[i].CenterOfGravity.X;


                    YData[0, cc] = i - Indexs.Length;
                    YData[1, cc] = BestCells[i].CenterOfGravity.Y;
                    cc++;
                }
            }

            for (int i = 0; i < Indexs.Length; i++)
            {
                if (BestCells[i] != null)
                {
                    XData[0, cc] = i;
                    XData[1, cc] = BestCells[i].CenterOfGravity.X;


                    YData[0, cc] = i;
                    YData[1, cc] = BestCells[i].CenterOfGravity.Y;
                    cc++;
                }
            }

            for (int i = 0; i < Indexs.Length; i++)
            {
                if (BestCells[i] != null)
                {
                    XData[0, cc] = i + Indexs.Length;
                    XData[1, cc] = BestCells[i].CenterOfGravity.X;


                    YData[0, cc] = i + Indexs.Length;
                    YData[1, cc] = BestCells[i].CenterOfGravity.Y;
                    cc++;
                }
            }*/
            #endregion

            List<double> FrameNumbers = new List<double>();
            List<double> X = new List<double>();
            List<double> Y = new List<double>();

            int cc = -1 * Indexs.Length;
            for (int i = 0; i < Indexs.Length * 3; i++)
            {
                if (BestCells[(i % Indexs.Length)] != null && BestCells[(i % Indexs.Length)].CenterOfGravity.X > 1)
                {
                    FrameNumbers.Add(cc);
                    X.Add(BestCells[(i % Indexs.Length)].CenterOfGravity.X);
                    Y.Add(BestCells[(i % Indexs.Length)].CenterOfGravity.Y);
                }
                cc++;
            }

            double[,] XData = new double[2, FrameNumbers.Count];
            double[,] YData = new double[2, FrameNumbers.Count];
            double StepSize = dataEnvironment.AllImages.Count / (double)Indexs.Length;
            string junk = "";
            for (int i = 0; i < FrameNumbers.Count; i++)
            {
                XData[0, i] = StepSize * FrameNumbers[i];
                YData[0, i] = StepSize * FrameNumbers[i];

                XData[1, i] = X[i];
                YData[1, i] = Y[i];
                junk += XData[0, i] + "\t" + XData[1, i] + "\t" + YData[0, i] + " \t" + YData[1, i] + "\n";
            }
            double[,] FitLineX;
            double[,] FitLineY;


            /* using (System.IO.StreamWriter file = new System.IO.StreamWriter(dataEnvironment.DataOutFolder + "FirstFinds.csv"))
             {
                 file.WriteLine(RoughCellSize);
                 for (int i = 0; i < XData.GetLength(1); i++)
                 {
                     file.WriteLine(XData[0,i] + ", " + XData[1,i] + "," + YData[1,i]);
                 }
             }*/


            FitLineY = MathHelpLib.CurveFitting.MathCurveFits.TrigFitIndexFixedFrequency(YData, dataEnvironment.AllImages.Count, 2 * Math.PI / dataEnvironment.AllImages.Count);//.PolynomialFitIndex(YData, 10, dataEnvironment.AllImages.Count); //PolynomialFitIndex(YData, 6, dataEnvironment.AllImages.Count);

            //remove the worst point and try again
            int WorstI = 0;
            double WorstV = 0;
            double worstValue = 0;
            for (int i = 0; i < Indexs.Length; i++)
            {
                double diff = Math.Abs(YData[1, i] - FitLineY[1, Indexs[i]]);
                if (diff > WorstV)
                {
                    WorstI = i;
                    WorstV = diff;
                    worstValue = YData[1, i];
                }
            }

            if (WorstV > 30)
            {
                double[,] XData2 = new double[2, XData.GetLength(1) - 3];
                double[,] YData2 = new double[2, YData.GetLength(1) - 3];
                cc = 0;
                for (int i = 0; i < XData.GetLength(1); i++)
                {
                    if (Math.Abs(YData[1, i] - worstValue) > .01)
                    {
                        YData2[0, cc] = YData[0, i];
                        YData2[1, cc] = YData[1, i];

                        XData2[0, cc] = XData[0, i];
                        XData2[1, cc] = XData[1, i];
                        cc++;
                    }
                }

                //FitLineX = MathHelpLib.CurveFitting.MathCurveFits.TrigFitIndexFixedFrequency(XData2, dataEnvironment.AllImages.Count, 2 * Math.PI / dataEnvironment.AllImages.Count);// .PolynomialFitIndex(XData, 10, dataEnvironment.AllImages.Count);
                //FitLineX = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFitIndex(XData, 5, dataEnvironment.AllImages.Count);
                FitLineY = MathHelpLib.CurveFitting.MathCurveFits.TrigFitIndexFixedFrequency(YData2, dataEnvironment.AllImages.Count, 2 * Math.PI / dataEnvironment.AllImages.Count);//.PolynomialFitIndex(YData, 10, dataEnvironment.AllImages.Count); //PolynomialFitIndex(YData, 6, dataEnvironment.AllImages.Count);


                XData = XData2;
                YData = YData2;
            }

            // List<KeyValuePair<double,double>> knownSamples = new List<KeyValuePair<double,double>>(); 
            // for (int I=0;I<XData.GetLength(1);I++)
            //    knownSamples.Add( new KeyValuePair<double,double>(XData[0,I],XData[1,I]));

            FitLineX = new double[2, dataEnvironment.AllImages.Count];
            for (int I = 0; I < dataEnvironment.AllImages.Count; I++)
            {
                for (int J = 0; J < XData.GetLength(1) - 1; J++)
                {
                    if (I >= XData[0, J] && I < XData[0, J + 1])
                    {
                        double start = XData[1, J];
                        double end = XData[1, J + 1];
                        double amount = (I - XData[0, J]) / (XData[0, J + 1] - XData[0, J]);
                        amount = (amount * amount) * (3f - (2f * amount));

                        FitLineX[1, I] = (start + ((end - start) * amount));
                        break;
                    }
                }
            }

            //FitLineX = MathHelpLib.CurveFitting.MathCurveFits.TrigFitIndexFixedFrequency(XData, dataEnvironment.AllImages.Count, 2 * Math.PI / dataEnvironment.AllImages.Count);// .PolynomialFitIndex(XData, 10, dataEnvironment.AllImages.Count);
            // FitLineX = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFitIndex(XData, 10, dataEnvironment.AllImages.Count);

            #region debug test

            //            FitLineX = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFitIndex(XData, 10, dataEnvironment.AllImages.Count);
            //          FitLineY = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFitIndex(YData, 10, dataEnvironment.AllImages.Count); //PolynomialFitIndex(YData, 6, dataEnvironment.AllImages.Count);

            /*   string junk = "";

               //   for (int i = 0; i < XData.GetLength(1); i++)
                //  {
                  //    junk += "XData[0, i]=" + XData[0, i] + "; YData[0, i]=" + XData[0, i ] +"; XData[1, i]=" + XData[1, i] + "; YData[1, i]=" + YData[1,i] + ";\n";
                  //}
               junk += "\n";
               junk += "\n";
               for (int i = 0; i < XData.GetLength(1); i++)
               {
                   junk += XData[0, i] + "\t" + XData[1, i] + "\t" + YData[1, i] + "\n";
               }
               junk += "\n"; junk += "\n";
               for (int i = 0; i < FitLineX.GetLength(1); i++)
               {
                   junk += FitLineX[0, i] + "\t" + FitLineX[1, i] + "\t" + FitLineY[1, i] + "\n";
               }
               //Clipboard.SetText(junk);

               System.Diagnostics.Debug.Print(junk);*/
            #endregion


            double minX = double.MaxValue;
            double maxX = double.MinValue;
            for (int i = 0; i < XData.GetLength(1); i++)
            {
                if (minX > XData[1, i]) minX = XData[1, i];
                if (maxX < XData[1, i]) maxX = XData[1, i];
            }

            if (EstimateCellSize)
            {
                for (int i = 0; i < dataEnvironment.AllImages.Count; i++)
                {
                    RoughLocations.X_Positions[i] = FitLineX[1, i];
                    RoughLocations.Y_Positions[i] = FitLineY[1, i];
                    RoughLocations.CellSizes[i] = BestCells[i].BlobBounds.Width;

                   // junk += i + "\t" + RoughLocations.X_Positions[i] + "\t" + RoughLocations.Y_Positions[i] + "\n";
                }
            }
            else
            {
                for (int i = 0; i < dataEnvironment.AllImages.Count; i++)
                {
                    RoughLocations.X_Positions[i] = FitLineX[1, i];
                    RoughLocations.Y_Positions[i] = FitLineY[1, i];
                    RoughLocations.CellSizes[i] = likelyWidth * 2;

                   // junk += i + "\t" + RoughLocations.X_Positions[i] + "\t" + RoughLocations.Y_Positions[i] + "\n";
                }
            }

           // System.Diagnostics.Debug.Print(junk);

            Rectangle cellBounds = new Rectangle((int)minX, minY, (int)(maxX - minX), maxY - minY);

            cellBounds.Inflate((int)(cellBounds.Height / 3d), (int)(cellBounds.Height / 3d));
            if (cellBounds.Y < 0) cellBounds.Y = 0;
            if (cellBounds.Bottom > dataEnvironment.AllImages[1].Height) cellBounds.Height = dataEnvironment.AllImages[1].Height - cellBounds.Y;


            FirstFound = true;
            RoughCellSize = likelyWidth * 2;
            roughCellSize = RoughCellSize;
            return cellBounds;
        }

        public static double SpLine(List<KeyValuePair<double, double>> knownSamples, double z)
        {

            int np = knownSamples.Count;

            if (np > 1)
            {

                double[] a = new double[np];

                double x1;

                double x2;

                double y;

                double[] h = new double[np];

                for (int i = 1; i <= np - 1; i++)
                {

                    h[i] = knownSamples[i].Key - knownSamples[i - 1].Key;

                }

                if (np > 2)
                {

                    double[] sub = new double[np - 1];

                    double[] diag = new double[np - 1];

                    double[] sup = new double[np - 1];

                    for (int i = 1; i <= np - 2; i++)
                    {

                        diag[i] = (h[i] + h[i + 1]) / 3;

                        sup[i] = h[i + 1] / 6;

                        sub[i] = h[i] / 6;

                        a[i] = (knownSamples[i + 1].Value - knownSamples[i].Value) / h[i + 1] -

                               (knownSamples[i].Value - knownSamples[i - 1].Value) / h[i];

                    }

                    // SolveTridiag is a support function, see Marco Roello's original code

                    // for more information at

                    // http://www.codeproject.com/useritems/SplineInterpolation.asp

                    solveTridiag(sub, diag, sup, ref a, np - 2);

                }



                int gap = 0;

                double previous = double.MinValue;

                // At the end of this iteration, "gap" will contain the index of the interval

                // between two known values, which contains the unknown z, and "previous" will

                // contain the biggest z value among the known samples, left of the unknown z

                for (int i = 0; i < knownSamples.Count; i++)
                {

                    if (knownSamples[i].Key < z && knownSamples[i].Key > previous)
                    {

                        previous = knownSamples[i].Key;

                        gap = i + 1;

                    }

                }

                x1 = z - previous;

                x2 = h[gap] - x1;

                y = ((-a[gap - 1] / 6 * (x2 + h[gap]) * x1 + knownSamples[gap - 1].Value) * x2 +

                    (-a[gap] / 6 * (x1 + h[gap]) * x2 + knownSamples[gap].Value) * x1) / h[gap];

                return y;

            }

            return 0;

        }

        private static void solveTridiag(double[] sub, double[] diag, double[] sup, ref double[] b, int n)
        {
            /*                  solve linear system with tridiagonal n by n matrix a
                                using Gaussian elimination *without* pivoting
                                where   a(i,i-1) = sub[i]  for 2<=i<=n
                                        a(i,i)   = diag[i] for 1<=i<=n
                                        a(i,i+1) = sup[i]  for 1<=i<=n-1
                                (the values sub[1], sup[n] are ignored)
                                right hand side vector b[1:n] is overwritten with solution 
                                NOTE: 1...n is used in all arrays, 0 is unused */
            int i;
            /*                  factorization and forward substitution */
            for (i = 2; i <= n; i++)
            {
                sub[i] = sub[i] / diag[i - 1];
                diag[i] = diag[i] - sub[i] * sup[i - 1];
                b[i] = b[i] - sub[i] * b[i - 1];
            }
            b[n] = b[n] / diag[n];
            for (i = n - 1; i >= 1; i--)
            {
                b[i] = (b[i] - sup[i] * b[i + 1]) / diag[i];
            }
        }

        private class BlobLocation
        {
            public int ImageN;
            public double X;
            public double Y;
            public double Area;

            public BlobLocation(BlobDescription blob, int imageNumber)
            {
                X = blob.CenterOfGravity.X;
                Y = blob.CenterOfGravity.Y;
                Area = blob.BlobBounds.Width * blob.BlobBounds.Height;
                ImageN = imageNumber;
            }
        }

        private static object FitLinesCriticalSection = new object();

        private void GetBlobInfo(string Line, out int ImageN, out int blobN, out Rectangle blobBounds, out Point centerOfMass)
        {
            string[] Parts = Line.Split(new string[] { "|", "{", "}", "=", ",", " " }, StringSplitOptions.RemoveEmptyEntries);

            int X = int.Parse(Parts[7]);
            int Y = int.Parse(Parts[9]);
            int W = int.Parse(Parts[11]);
            int H = int.Parse(Parts[13]);

            int cX = int.Parse(Parts[3]);
            int cY = int.Parse(Parts[5]);

            centerOfMass = new Point(cX, cY);
            blobBounds = new Rectangle(X, Y, W, H);

            ImageN = int.Parse(Parts[0]);
            blobN = int.Parse(Parts[1]);
        }

        public void PreFitLines2(DataEnvironment dataEnvironment, out  double[] X_Positions, out double[] Y_Positions)
        {


            X_Positions = new double[dataEnvironment.AllImages.Count];
            Y_Positions = new double[dataEnvironment.AllImages.Count];

            double[,] XData = null;
            double[,] YData = null;

            X_Positions[0] = X_Positions[1];
            Y_Positions[0] = Y_Positions[1];
            X_Positions[X_Positions.Length - 1] = X_Positions[X_Positions.Length - 2];
            Y_Positions[X_Positions.Length - 1] = Y_Positions[X_Positions.Length - 2];

            double WholeP = dataEnvironment.AllImages[0].Width;
            string junk = "";


            int endImage = dataEnvironment.AllImages.Count - 1;

            double[] YFiltered = new double[Y_Positions.Length];

            //do median filtering to remove spikes
            List<double> xVals = new List<double>(new double[11]);
            int cc = 0;
            for (int i = 0; i < 5; i++)
                YFiltered[i] = Y_Positions[i];

            for (int i = 5; i < Y_Positions.Length - 5; i++)
            {
                int start = i - 5;
                int end = i + 5;
                cc = 0;
                for (int j = start; j < end; j++)
                {
                    if (double.IsNaN(Y_Positions[j]) == false)
                        xVals[cc] = Y_Positions[j];
                    cc++;
                }
                xVals.Sort();
                YFiltered[i] = xVals[xVals.Count / 2];
            }

            for (int i = Y_Positions.Length - 5; i < Y_Positions.Length; i++)
                YFiltered[i] = Y_Positions[i];

            for (int i = 0; i < X_Positions.Length; i++)
            {
                junk += X_Positions[i] + "\t" + Y_Positions[i] + "\t" + YFiltered[i] + "\n";
            }

            List<double> FrameNumber = new List<double>();
            List<double> X = new List<double>();
            List<double> Y = new List<double>();


            cc = -1 * X_Positions.Length;

            for (int i = 0; i < X_Positions.Length * 3; i++)
            {
                if (X_Positions[(i % X_Positions.Length)] > 0 && Y_Positions[(i % X_Positions.Length)] > 0)
                {
                    FrameNumber.Add(cc);
                    X.Add(X_Positions[(i % X_Positions.Length)]);
                    Y.Add(YFiltered[(i % X_Positions.Length)]);
                }
                cc++;

            }

            cc = 0;
            XData = new double[2, FrameNumber.Count];
            YData = new double[2, FrameNumber.Count];

            for (int i = 0; i < FrameNumber.Count; i++)
            {
                XData[0, i] = FrameNumber[i];
                YData[0, i] = FrameNumber[i];
                XData[1, i] = X[i];
                YData[1, i] = Y[i];

            }

            double[,] FitLineX = null;
            double[,] FitLineY = null;

            // FitLineX = MathHelpLib.CurveFitting.MathCurveFits.TrigFitIndexFixedFrequency(XData, dataEnvironment.AllImages.Count, 2 * Math.PI / dataEnvironment.AllImages.Count);
            //FitLineX = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFitIndex(XData, 11, dataEnvironment.AllImages.Count);
            FitLineY = MathHelpLib.CurveFitting.MathCurveFits.TrigFitIndexFixedFrequency(YData, dataEnvironment.AllImages.Count, 2 * Math.PI / dataEnvironment.AllImages.Count);

            FitLineX = new double[2, X_Positions.Length];


            //do median filtering to remove spikes from the x column
            xVals = new List<double>(new double[11]);
            cc = 0;
            for (int i = 0; i < 5; i++)
                FitLineX[1, i] = X_Positions[i];

            for (int i = 5; i < X_Positions.Length - 5; i++)
            {
                int start = i - 5;
                int end = i + 5;
                cc = 0;
                for (int j = start; j < end; j++)
                {
                    if (double.IsNaN(X_Positions[j]) == false && X_Positions[j] > 0)
                        xVals[cc] = X_Positions[j];
                    cc++;
                }
                xVals.Sort();
                FitLineX[1, i] = xVals[xVals.Count / 2];
            }

            for (int i = X_Positions.Length - 5; i < X_Positions.Length; i++)
                FitLineX[1, i] = X_Positions[i];

            //FitLineY = MathHelpLib.CurveFitting.MathCurveFits.PolynomialFitIndex(YData, 7, dataEnvironment.AllImages.Count);



            /* for (int i = 0; i < XData.GetLength(1); i++)
             {
                 junk += "XData[0, i]=" + XData[0, i] + "; YData[0, i]=" + XData[0, i] + "; XData[1, i]=" + XData[1, i] + "; YData[1, i]=" + YData[1, i] + ";\n";
             }
            junk += "\n";
            junk += "\n";
            // for (int i = 0; i < XData.GetLength(1); i++)
            {
                //     junk += XData[0, i] + "\t" + XData[1, i] + "\t" + YData[1, i] + "\n";
            }
            junk += "\n"; junk += "\n";
            for (int i = 0; i < FitLineX.GetLength(1); i++)
            {
                junk += FitLineX[0, i] + "\t" + FitLineX[1, i] + "\t" + FitLineY[1, i] + "\n";
            }
            //Clipboard.SetText(junk);

            System.Diagnostics.Debug.Print(junk);*/

            for (int i = 0; i < dataEnvironment.AllImages.Count; i++)
            {
                X_Positions[i] = FitLineX[1, i];
                Y_Positions[i] = FitLineY[1, i];
            }
            X_Positions[0] = X_Positions[1];
            Y_Positions[0] = Y_Positions[1];
        }

        #region File Operations
        public void SaveBlobs(DataEnvironment dataEnvironment)
        {
            if (mBlobs != null && mBlobs.Length > 0)
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(dataEnvironment.DataOutFolder + "Blobs.txt");
                for (int i = 0; i < mBlobs.Length; i++)
                {
                    for (int j = 0; j < mBlobs[i].Length; j++)
                    {
                        Point p = new Point(mBlobs[i][j].CenterOfGravity.X, mBlobs[i][j].CenterOfGravity.Y);
                        Rectangle r = new Rectangle(mBlobs[i][j].BlobBounds.X, mBlobs[i][j].BlobBounds.Y, mBlobs[i][j].BlobBounds.Width, mBlobs[i][j].BlobBounds.Height);
                        file.WriteLine(i.ToString() + "| " + j.ToString() + "| " + p.ToString() + "| " + r.ToString());
                    }
                }
                file.Close();
            }
        }

        public bool OpenBlobs(DataEnvironment dataEnvironment, out double[] X_Positions, out double[] Y_Positions, out double[] CellSizes)
        {

            if (File.Exists(dataEnvironment.DataOutFolder + "Blobs.txt"))
            {
                try
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(dataEnvironment.DataOutFolder + "Blobs.txt");
                    string FileS = file.ReadToEnd();
                    file.Close();

                    string[] Lines = FileS.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    mBlobs = new BlobDescription[dataEnvironment.AllImages.Count][];
                    X_Positions = new double[dataEnvironment.AllImages.Count];
                    Y_Positions = new double[dataEnvironment.AllImages.Count];
                    CellSizes = new double[dataEnvironment.AllImages.Count];
                    List<BlobDescription>[] BlobList = new List<BlobDescription>[dataEnvironment.AllImages.Count];

                    for (int i = 0; i < Lines.Length; i++)
                    {
                        int ImageN, BlobN;
                        Rectangle blobBounds;
                        Point com;

                        GetBlobInfo(Lines[i], out ImageN, out BlobN, out blobBounds, out com);

                        if (BlobList[ImageN] == null)
                            BlobList[ImageN] = new List<BlobDescription>();

                        BlobList[ImageN].Add(new BlobDescription(ImageN, blobBounds, com));
                    }

                    for (int i = 0; i < mBlobs.Length; i++)
                    {
                        if (BlobList[i] != null)
                        {
                            mBlobs[i] = BlobList[i].ToArray();

                            var MaxBlob = GetBiggestCenterBlob.SortBlobs(mBlobs[i], dataEnvironment.AllImages[0]);

                            // CenterCellsTool2Form.StoreBlobLocation(dataEnvironment, i, (BlobDescription[])mBlobs[i], 1, CellWanderArea);

                            X_Positions[i] = MaxBlob.CenterOfGravity.X;
                            Y_Positions[i] = MaxBlob.CenterOfGravity.Y;
                            CellSizes[i] = (MaxBlob.BlobBounds.Width + MaxBlob.BlobBounds.Height) / 2;
                        }
                    }
                    return true;
                }
                catch
                {
                    X_Positions = new double[dataEnvironment.AllImages.Count];
                    Y_Positions = new double[dataEnvironment.AllImages.Count];
                    CellSizes = new double[dataEnvironment.AllImages.Count];
                    return false;
                }
            }
            else
            {
                X_Positions = new double[dataEnvironment.AllImages.Count];
                Y_Positions = new double[dataEnvironment.AllImages.Count];
                CellSizes = new double[dataEnvironment.AllImages.Count];
                return false;
            }

        }

        public void SaveCenters(DataEnvironment dataEnvironment, double[] X_Positions, double[] Y_Positions)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(dataEnvironment.DataOutFolder + "Centers.csv", false);
            for (int i = 0; i < X_Positions.Length; i++)
            {
                file.WriteLine(i.ToString() + ", " + X_Positions[i].ToString() + ", " + Y_Positions[i].ToString());
            }
            file.Close();
        }

        public bool OpenCenters(DataEnvironment dataEnvironment, out double[] X_Positions, out double[] Y_Positions, out int CellSize)
        {
            X_Positions = new double[500];
            Y_Positions = new double[500];
            CellSize = 250;
            if (File.Exists(dataEnvironment.DataOutFolder + "Centers.csv") == false)
                return false;

            System.IO.StreamReader file = new System.IO.StreamReader(dataEnvironment.DataOutFolder + "Centers.csv");
            string FileAll = file.ReadToEnd();
            string[] Lines = FileAll.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int lastIndex = -1;
            for (int i = 0; i < Lines.Length; i++)
            {
                string[] parts = Lines[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                int index = int.Parse(parts[0]);
                if (lastIndex == -1)
                    lastIndex = index;
                X_Positions[index] = double.Parse(parts[1]);
                Y_Positions[index] = double.Parse(parts[2]);
            }

            //deal with missing values
            if (X_Positions[0] == 0 && Y_Positions[0] == 0)
            {
                X_Positions[0] = X_Positions[lastIndex];
                Y_Positions[0] = Y_Positions[lastIndex];
            }
            for (int i = 1; i < Lines.Length; i++)
            {
                if (X_Positions[i] == 0 && Y_Positions[i] == 0)
                {
                    X_Positions[i] = X_Positions[lastIndex];
                    Y_Positions[i] = Y_Positions[lastIndex];
                }
                else
                    lastIndex = i;
            }


            //find the cellsize
            if (File.Exists(dataEnvironment.DataOutFolder + "blobs.txt"))
            {
                double tCellSize = 0;
                double cc = 0;
                file = new System.IO.StreamReader(dataEnvironment.DataOutFolder + "blobs.txt");
                FileAll = file.ReadToEnd();
                Lines = FileAll.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                // 0| 0| {X=104,Y=602}| {X=0,Y=0,Width=224,Height=449}
                for (int i = 0; i < Lines.Length; i++)
                {
                    string[] Parts = Lines[i].Split(new string[] { "|", "{", "}", "=", ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                    double W = double.Parse(Parts[11]);
                    double H = double.Parse(Parts[13]);

                    if (W > 40 && W < 300 && H > 40 && H < 300)
                    {
                        tCellSize += (W + H);
                        cc += 2;
                    }
                }
                CellSize = (int)(tCellSize / cc * 2.2);
            }

            file.Close();
            return true;
        }

        #endregion
    }
}
