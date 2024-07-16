using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageViewer.Filters;
using ImageViewer.Filters.Blobs;
using System.Drawing.Imaging;
using System.Threading.Tasks;
namespace VisionCentering
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }


        private PointF OpticalFlow(Image<Gray, Byte> imgA, Image<Gray, Byte> imgB, int MAX_CORNERS)
        {
            // System.Threading.Tasks.Parallel.For(

            Size img_sz = imgA.Size;
            int win_size = 15;

            // Get the features for tracking
            Image<Bgr, byte> eig_image = new Image<Bgr, byte>(img_sz);
            Image<Bgr, byte> tmp_image = new Image<Bgr, byte>(img_sz);

            int corner_count = MAX_CORNERS;

            PointF[][] cornersA = imgA.GoodFeaturesToTrack(MAX_CORNERS, .05, 5, 3);
            //cvGoodFeaturesToTrack( imgA, eig_image, tmp_image, cornersA, &corner_count,
            //	0.05, 5.0, 0, 3, 0, 0.04 );
            MCvTermCriteria criteria = new MCvTermCriteria(20, .03);
            criteria.type = Emgu.CV.CvEnum.TERMCRIT.CV_TERMCRIT_ITER | Emgu.CV.CvEnum.TERMCRIT.CV_TERMCRIT_EPS;
            imgA.FindCornerSubPix(cornersA, new System.Drawing.Size(win_size, win_size), new System.Drawing.Size(-1, -1), criteria);
            //cvFindCornerSubPix( imgA, cornersA, corner_count, cvSize( win_size, win_size ),
            //	cvSize( -1, -1 ), cvTermCriteria( CV_TERMCRIT_ITER | CV_TERMCRIT_EPS, 20, 0.03 ) );

            int cc = 0;
            for (int i = 0; i < cornersA.Length; i++)
            {
                cc += cornersA[i].Length;
            }
            PointF[] FixedCorners = new PointF[cc];
            cc = 0;
            for (int i = 0; i < cornersA.Length; i++)
            {
                for (int j = 0; j < cornersA[i].Length; j++)
                {
                    FixedCorners[cc] = cornersA[i][j];
                    cc++;
                }
            }

            // Call Lucas Kanade algorithm
            byte[] features_found = new byte[MAX_CORNERS];
            float[] feature_errors = new float[MAX_CORNERS];

            Size pyr_sz = new System.Drawing.Size(imgA.Width + 8, imgB.Height / 3);
            Image<Bgr, byte> pyrA = new Image<Bgr, byte>(pyr_sz);
            Image<Bgr, byte> pyrB = new Image<Bgr, byte>(pyr_sz);

            PointF[] cornersB = new PointF[MAX_CORNERS];

            Emgu.CV.OpticalFlow.PyrLK(imgA, imgB, FixedCorners, new Size(win_size, win_size), 5, new MCvTermCriteria(20, .03), out cornersB, out features_found, out feature_errors);

            //  cvCalcOpticalFlowPyrLK( imgA, imgB, pyrA, pyrB, cornersA, cornersB, corner_count,  
            //	cvSize( win_size, win_size ), 5, features_found, feature_errors,
            //	 cvTermCriteria( CV_TERMCRIT_ITER | CV_TERMCRIT_EPS, 20, 0.3 ), 0 );
            PointF[] Vecs = new PointF[FixedCorners.Length];
            float aveX = 0, aveY = 0;
            for (int i = 0; i < FixedCorners.Length; i++)
            {
                PointF Vec = new PointF(FixedCorners[i].X - cornersB[i].X, FixedCorners[i].Y - cornersB[i].Y);
                Vecs[i] = Vec;
                aveX += Vec.X;
                aveY += Vec.Y;
            }
            aveX /= FixedCorners.Length;
            aveY /= FixedCorners.Length;

            // Make an image of the results
            return new PointF(aveX, aveY);
        }


        private PointF OpticalFlow(Bitmap frame1, Bitmap frame2, int MAX_CORNERS)
        {
            // System.Threading.Tasks.Parallel.For(

            Image<Gray, Byte> imgA =
               new Image<Gray, byte>(frame1);
            Image<Gray, Byte> imgB =
               new Image<Gray, byte>(frame2);


            Size img_sz = imgA.Size;
            int win_size = 15;



            // Get the features for tracking
            Image<Bgr, byte> eig_image = new Image<Bgr, byte>(img_sz);
            Image<Bgr, byte> tmp_image = new Image<Bgr, byte>(img_sz);

            int corner_count = MAX_CORNERS;

            PointF[][] cornersA = imgA.GoodFeaturesToTrack(MAX_CORNERS, .05, 5, 3);
            //cvGoodFeaturesToTrack( imgA, eig_image, tmp_image, cornersA, &corner_count,
            //	0.05, 5.0, 0, 3, 0, 0.04 );
            MCvTermCriteria criteria = new MCvTermCriteria(20, .03);
            criteria.type = Emgu.CV.CvEnum.TERMCRIT.CV_TERMCRIT_ITER | Emgu.CV.CvEnum.TERMCRIT.CV_TERMCRIT_EPS;
            imgA.FindCornerSubPix(cornersA, new System.Drawing.Size(win_size, win_size), new System.Drawing.Size(-1, -1), criteria);
            //cvFindCornerSubPix( imgA, cornersA, corner_count, cvSize( win_size, win_size ),
            //	cvSize( -1, -1 ), cvTermCriteria( CV_TERMCRIT_ITER | CV_TERMCRIT_EPS, 20, 0.03 ) );

            int cc = 0;
            for (int i = 0; i < cornersA.Length; i++)
            {
                cc += cornersA[i].Length;
            }
            PointF[] FixedCorners = new PointF[cc];
            cc = 0;
            for (int i = 0; i < cornersA.Length; i++)
            {
                for (int j = 0; j < cornersA[i].Length; j++)
                {
                    FixedCorners[cc] = cornersA[i][j];
                    cc++;
                }
            }

            // Call Lucas Kanade algorithm
            byte[] features_found = new byte[MAX_CORNERS];
            float[] feature_errors = new float[MAX_CORNERS];

            Size pyr_sz = new System.Drawing.Size(imgA.Width + 8, imgB.Height / 3);
            Image<Bgr, byte> pyrA = new Image<Bgr, byte>(pyr_sz);
            Image<Bgr, byte> pyrB = new Image<Bgr, byte>(pyr_sz);

            PointF[] cornersB = new PointF[MAX_CORNERS];

            Emgu.CV.OpticalFlow.PyrLK(imgA, imgB, FixedCorners, new Size(win_size, win_size), 5, new MCvTermCriteria(20, .03), out cornersB, out features_found, out feature_errors);

            //  cvCalcOpticalFlowPyrLK( imgA, imgB, pyrA, pyrB, cornersA, cornersB, corner_count,  
            //	cvSize( win_size, win_size ), 5, features_found, feature_errors,
            //	 cvTermCriteria( CV_TERMCRIT_ITER | CV_TERMCRIT_EPS, 20, 0.3 ), 0 );
            PointF[] Vecs = new PointF[FixedCorners.Length];
            float aveX = 0, aveY = 0;
            for (int i = 0; i < FixedCorners.Length; i++)
            {
                PointF Vec = new PointF(FixedCorners[i].X - cornersB[i].X, FixedCorners[i].Y - cornersB[i].Y);
                Vecs[i] = Vec;
                aveX += Vec.X;
                aveY += Vec.Y;
            }
            aveX /= FixedCorners.Length;
            aveY /= FixedCorners.Length;

            // Make an image of the results
            return new PointF(aveX, aveY);
        }

        private CircleF[] FindBiggestCircle(Bitmap b)
        {

            //Load the image from file and resize it for display
            Image<Bgr, Byte> img =
               new Image<Bgr, byte>(b);// .Resize(400, 400, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR, true);

            //Convert the image to grayscale and filter out the noise
            Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

            gray = gray.Not();
            Gray cannyThreshold = new Gray(30);
            // Gray cannyThresholdLinking = new Gray(120);
            Gray circleAccumulatorThreshold = new Gray(300);
            CircleF[] circles = gray.HoughCircles(
                cannyThreshold,
                circleAccumulatorThreshold,
                4.0, //Resolution of the accumulator used to detect centers of the circles
                15.0, //min distance 
                (int)(b.Width / 2d * .6), //min radius
                (int)(b.Width / 2d * .85) //max radius
                )[0]; //Get the circles from the first channel

            int MaxIndex = -1;
            double Area = 0;
            for (int i = 0; i < circles.Length; i++)
            {
                if (circles[i].Area > Area)
                {
                    Area = circles[i].Area;
                    MaxIndex = i;
                }
            }

            return circles;
        }
        Emgu.CV.Capture input_video;

        const int nFrame =45;
        Bitmap[] Frames = new Bitmap[nFrame];
        Image<Gray, byte>[] FramesEMGU = new Image<Gray, byte>[nFrame];
        Point[] FrameCenters = new Point[nFrame];
        Rectangle CellSize;
        private void button1_Click(object sender, EventArgs e)
        {
            hScrollBar1.Maximum = nFrame+5;
            input_video = new Emgu.CV.Capture(@"C:\AndrewMovie\2MHz 1.5V Before High.avi");
            for (int i = 0; i < Frames.Length; i++)
            {
                Image<Gray, byte> frame = input_video.QueryGrayFrame();
                FramesEMGU[i] = frame;
                Bitmap b = frame.ToBitmap();
                Frames[i] = b;
            }
            viewerControl1.SetImage(Frames[0]);
            input_video = null;
        }

        private void viewerControl1_SelectionPerformed(ImageViewer.ISelection Selection)
        {
            CellSize = new Rectangle(0, 0, Selection.SelectionBounds.Width, Selection.SelectionBounds.Height);
            FrameCenters[hScrollBar1.Value] = new Point(Selection.ImageCenter.X, Selection.ImageCenter.Y);
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            viewerControl1.SetImage(Frames[hScrollBar1.Value]);
            label1.Text = hScrollBar1.Value.ToString();
        }

        double[, ,] DensityGrid;
        double[, ,] VisitCount;

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Frames.Length; i++)
                Frames[i].Dispose();
            Frames = null;
            GC.Collect();
            //input_video = new Emgu.CV.Capture(@"C:\AndrewMovie\2MHz 1.5V Before High.avi");

            IEffect Filter;
            ReplaceStringDictionary PassData = new ReplaceStringDictionary();

            Bitmap lastB = null;
            int cc = 0;
            PointF AllTurnDir = new PointF(0, 0);
            int TurnCount = 0;
            CellSize.Inflate(50, 50);
            Rectangle CutRegion = CellSize;
            CutRect = CellSize;
            DensityGrid = new double[CutRect.Width, CutRect.Width, CutRect.Width];
            VisitCount = new double[CutRect.Width, CutRect.Width, CutRect.Width];

            int TrueFrames = int.Parse( textBox1.Text );

            double[][,] Radiuss = new double[CutRect.Height / 2][,];
            for (int i = 0; i < Radiuss.Length; i++)
            {
                Radiuss[i] = new double[CutRect.Width, TrueFrames];
            }
            double ProjAngle = 0;
            Current = new Point(105, 104);

            List<double[]> XPoints = new List<double[]>();
            List<double[]> YPoints = new List<double[]>();

            for (int i = 0; i < FrameCenters.Length; i++)
            {
                if ((FrameCenters[i].X != 0 && FrameCenters[i].Y != 0))
                {
                    XPoints.Add(new double[] { i, FrameCenters[i].X });
                    YPoints.Add(new double[] { i, FrameCenters[i].Y });
                }
            }

            if (FrameCenters[FrameCenters.GetLength(0) - 1].X == 0 && FrameCenters[FrameCenters.GetLength(0) - 1].Y == 0)
            {
                XPoints.Add(new double[] { FrameCenters.GetLength(0) - 1, XPoints[XPoints.Count - 1][1] });
                YPoints.Add(new double[] { FrameCenters.GetLength(0) - 1, YPoints[YPoints.Count - 1][1] });
            }

            double[,] XData = new double[XPoints.Count, 2];
            double[,] YData = new double[YPoints.Count, 2];

            for (int i = 0; i < XPoints.Count; i++)
            {
                XData[i, 0] = XPoints[i][0];
                XData[i, 1] = XPoints[i][1];

                YData[i, 0] = YPoints[i][0];
                YData[i, 1] = YPoints[i][1];
            }

            double[] FitLineX = MathHelpLib.MathHelps.CubicInterpolationIndex(XData);
            double[] FitLineY = MathHelpLib.MathHelps.CubicInterpolationIndex(YData);

            /*double[] FitLineX = new double[FrameCenters.Length];
            double[] FitLineY = new double[FrameCenters.Length];

            for (int i = 0; i < FrameCenters.Length; i++)
            {
                FitLineX[i] = FrameCenters[0].X;
                FitLineY[i] = FrameCenters[0].Y;
            }*/

            int HalfWidth = CellSize.Width / 2;
            int HalfHeight = CellSize.Height / 2;

            for (int FrameN = 0; FrameN < TrueFrames/2; FrameN++)
            {
                Image<Gray, byte> frame = FramesEMGU[FrameN];// input_video.QueryGrayFrame();
                frame.ROI = new Rectangle((int)(FitLineX[FrameN] - HalfWidth), (int)(FitLineY[FrameN] - HalfHeight), CellSize.Width, CellSize.Height);

                Bitmap ClippedFrame = frame.ToBitmap();

                float Angle = (float)MathHelpLib.MathHelps.GetAngleDegrees(Current.X, Current.Y);

                Bitmap RotatedFrame = MathHelpLib.ImageProcessing.MathImageHelps.rotateImage(ClippedFrame, -1 * Angle, Color.Black);

                double[,] bImage = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(RotatedFrame, false);

                int MidY = bImage.GetLength(1) / 2;
                for (int xx = 0; xx < bImage.GetLength(0); xx++)
                {

                   /* for (int R = 0; R < MidY; R++)
                    {
                        if (R > 0)
                            Radiuss[R][xx, FrameN] += bImage[xx, R + MidY];
                    }*/

                    for (int yy = 0; yy < bImage.GetLength(1); yy++)
                    {
                        int R = yy - MidY;
                        try
                        {
                            if (R >= 0)
                                Radiuss[R][xx, FrameN] = bImage[xx, yy];
                            else if (FrameN + TrueFrames / 2 < TrueFrames)
                                Radiuss[-1 * R - 1][xx, FrameN + (TrueFrames + 1) / 2] = bImage[xx, yy];
                        }
                        catch { }
                    }
                }

                try
                {
                    viewerControl1.SetImage(RotatedFrame);
                }
                catch { }

                ClippedFrame.Dispose();
                RotatedFrame.Dispose();
                cc = cc + 1;

                ProjAngle += 2 * Math.PI / TrueFrames;
                frame.Dispose();
                frame = null;
                GC.Collect();
            }

            // return;

            for (int R = 1; R < Radiuss.Length; R++)
            {
                double[,] ImageData = Radiuss[R];

                Bitmap b = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToBitmap(ImageData);

                //stretch the bitmap using all the nice drawing machinery
                Bitmap b1 = new Bitmap(b, new Size(b.Width, (int)(361)));

                if (R == 20)
                {
                    viewerControl1.SetImage(b1);
                    Application.DoEvents();
                }

                ImageData = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(b1, false);
                Radiuss[R] = ImageData;
            }


            for (int i = 0; i < DensityGrid.GetLength(0); i++)
            {
                for (int j = 0; j < DensityGrid.GetLength(1); j++)
                    for (int k = 0; k < DensityGrid.GetLength(2); k++)
                    {
                        double R = Math.Round(Math.Sqrt(Math.Pow(j - DensityGrid.GetLength(1) / 2, 2) + Math.Pow(k - DensityGrid.GetLength(2) / 2, 2)));
                        double Angle = Math.Round(MathHelpLib.MathHelps.GetAngleDegrees(j - DensityGrid.GetLength(1) / 2, k - DensityGrid.GetLength(2) / 2));
                        if (R > 0 && R < Radiuss.Length)
                        {
                            double[,] ImageData = Radiuss[(int)R];
                            DensityGrid[i, j, k] = ImageData[i, (int)Angle];
                        }
                    }
            }

            AllTurnDir = new PointF(AllTurnDir.X / TurnCount, AllTurnDir.Y / TurnCount);

        }


        private void button2_ClickO(object sender, EventArgs e)
        {

            input_video = new Emgu.CV.Capture(@"C:\AndrewMovie\2MHz 1.5V Before High.avi");

            IEffect Filter;
            ReplaceStringDictionary PassData = new ReplaceStringDictionary();

            Bitmap lastB = null;
            int cc = 0;
            PointF AllTurnDir = new PointF(0, 0);
            int TurnCount = 0;
            Rectangle CutRegion = new Rectangle(CutRect.X, CutRect.Y, CutRect.Width, CutRect.Height);
            DensityGrid = new double[CutRect.Width, CutRect.Width, CutRect.Width];
            VisitCount = new double[CutRect.Width, CutRect.Width, CutRect.Width];

            int TrueFrames = 71;

            double[][,] Radiuss = new double[CutRect.Height / 2][,];
            for (int i = 0; i < Radiuss.Length; i++)
            {
                Radiuss[i] = new double[CutRect.Width, TrueFrames];
            }
            double ProjAngle = 0;
            Current = new Point(105, 104);

            //get the current frame and then pickout the first set of features needed for the optical flow
            Image<Gray, byte> Originalframe = input_video.QueryGrayFrame();
            PointF[][] LFeatures = Originalframe.GoodFeaturesToTrack(5, .01, .01, 10);

            Bitmap bClean = Originalframe.ToBitmap();

            bClean = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(bClean, CutRegion);

            double[,] OriginalImage = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(bClean, false);

            Bitmap b = ImageViewer.Filters.Thresholding.OtsuThresholdEffect.OtsuThreshold(bClean);

            b = ImageViewer.Filters.Effects.Morphology.ErosionTool.Erosion(b, 5);
            Image<Gray, byte> lframe = Originalframe;
            lframe.ROI = CutRegion;
            for (int FrameN = 0; FrameN < TrueFrames; FrameN++)
            {
                Image<Gray, byte> frame = input_video.QueryGrayFrame();
                bClean = frame.ToBitmap();
                bClean = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(bClean, CutRegion);

                double[,] ThisImage = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(bClean, false);
                double[,] ConvolutionImage = MathHelpLib.MathFFTHelps.CrossCorrelationFFT(OriginalImage, ThisImage);

                int MaxI = 0;
                double MaxIV = -10000000;
                double sum = 0;
                int MaxJ = 0;
                for (int i = 0; i < ConvolutionImage.GetLength(0); i++)
                {
                    for (int j = 0; j < ConvolutionImage.GetLength(1); j++)
                    {
                        if (ConvolutionImage[i, j] > MaxIV)
                        {
                            MaxIV = ConvolutionImage[i, j];
                            MaxI = i;
                            MaxJ = j;
                        }
                        sum += ThisImage[i, j];
                    }
                }




                frame.ROI = CutRegion;
                //lframe.ROI = CutRegion;
                PointF Overlap = OpticalFlow(frame, lframe, 5);

                PointF[] OriginalPoints = new PointF[] { new PointF(0, 0), new PointF(1, 0), new PointF(0, 1) };
                PointF[] NewPoints = new PointF[] { new PointF(Overlap.X, Overlap.Y), new PointF(1 + Overlap.X, Overlap.Y), new PointF(Overlap.X, 1 + Overlap.Y) };
                var tmatrix = Emgu.CV.CameraCalibration.GetAffineTransform(NewPoints, OriginalPoints);
                frame = frame.WarpAffine<double>(tmatrix, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC, Emgu.CV.CvEnum.WARP.CV_WARP_FILL_OUTLIERS, new Gray(0));
                lframe.Dispose();
                lframe = null;
                lframe = frame;

                // frame.ROI = new Rectangle(0,0,frame.Width,frame.Height);
                b = frame.ToBitmap();
                // b = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(b, CutRegion);
                /*pictureBox2.Image = b;
                pictureBox2.Invalidate();
                Application.DoEvents();*/




                b = ImageViewer.Filters.Thresholding.OtsuThresholdEffect.OtsuThreshold(bClean);
                b = ImageViewer.Filters.Effects.Morphology.ErosionTool.Erosion(b, 5);

                /* pictureBox2.Image = b;
                 pictureBox2.Invalidate();
                 Application.DoEvents();*/


                Filter = new ImageViewer.Filters.Blobs.WaterShedTool();
                Filter.DoEffect(null, b, PassData);
                //Data out of type :
                PassData = Filter.PassData;

                //Get Biggest Blob
                Filter = new ImageViewer.Filters.Blobs.GetBiggestBlob();
                Filter.DoEffect(null, null, PassData, PassData["Blobs"], false);
                //Data out of type :
                PassData = Filter.PassData;

                int x = 0;
                int y = 0;
                if (PassData.ContainsKey("MaxBlob") == true)
                {
                    BlobDescription Rect = (BlobDescription)PassData["MaxBlob"];
                    x = Rect.CenterOfGravity.X;
                    y = Rect.CenterOfGravity.Y;
                }

                Bitmap b2 = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppRgb);
                Graphics g2 = Graphics.FromImage(b2);
                // g2.DrawImageUnscaled(bClean, new Point(x - b.Width / 2, y - b.Height / 2));
                g2.DrawImageUnscaled(bClean, new Point(0, 0));
                b.Dispose();
                b = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(b2, new Rectangle(10, 10, b2.Width - 20, b2.Height - 20));

                float Angle = (float)MathHelpLib.MathHelps.GetAngleDegrees(Current.X, Current.Y);

                b = MathHelpLib.ImageProcessing.MathImageHelps.rotateImage(b, -1 * Angle, Color.Black);

                double[,] bImage = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(b, false);

                int MidY = bImage.GetLength(1) / 2;
                for (int xx = 0; xx < bImage.GetLength(0); xx++)
                {
                    for (int yy = 0; yy < bImage.GetLength(1); yy++)
                    {
                        int R = yy - MidY;

                        if (R > 0)
                            Radiuss[R][xx, FrameN] += bImage[xx, yy];
                        //else
                        //  Radiuss[-1 * R][xx, (FrameN+36) % 72] +=  bImage[xx, yy];

                        /* // if (R > 0)
                          {
                              int aX = xx;
                              int aY = (int)(Math.Round(Math.Cos(ProjAngle) * R)) + DensityGrid.GetLength(1) / 2-1;
                              int aZ = (int)(Math.Round(Math.Sin(ProjAngle) * R)) + DensityGrid.GetLength(2) / 2-1;

                              DensityGrid[aX, aY, aZ] = bImage[xx, yy];
                              VisitCount[aX, aY, aZ] = 1;
                          }*/
                    }
                }
                try
                {
                    b.SetPixel(x, y, Color.Red);
                    viewerControl1.SetImage(b);
                }
                catch { }
                b.Dispose();
                cc = cc + 1;

                ProjAngle += 2 * Math.PI / TrueFrames;
                GC.Collect();
            }

            // return;

            for (int R = 1; R < Radiuss.Length; R++)
            {
                double[,] ImageData = Radiuss[R];
                /* int HalfJ = (ImageData.GetLength(1)+1)/2;

                 double[,] ImageData2 = new double[ImageData.GetLength(0), ImageData.GetLength(1)];

                 for (int i=0;i<ImageData.GetLength(0);i++)
                     for (int j = 0; j < ImageData.GetLength(1); j++)
                     {
                         ImageData2[i, j] = ImageData[i,(j + HalfJ) % ImageData.GetLength(1)]; 
                     }*/

                b = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToBitmap(ImageData);
                Bitmap b1 = new Bitmap(b, new Size(b.Width, (int)(361)));

                //b = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToBitmap(ImageData2);
                //Bitmap b2 = new Bitmap(b,new Size(b.Width ,(int)( 361 ) ));

                if (R == 20)
                {
                    //pictureBox1.Image = b1;
                    Application.DoEvents();
                }

                ImageData = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(b1, false);
                //ImageData2 =MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(b2, false);



                /*  for (int i=0;i<ImageData.GetLength(0);i++)
                    for (int j = 0; j < 90; j++)
                    {
                        ImageData[i,j]=ImageData2[i,j];
                    }

                  for (int i = 0; i < ImageData.GetLength(0); i++)
                      for (int j = 270; j < 361; j++)
                      {
                          ImageData[i, j] = ImageData2[i, j];
                      }*/


                Radiuss[R] = ImageData;
            }


            for (int i = 0; i < DensityGrid.GetLength(0); i++)
            {
                for (int j = 0; j < DensityGrid.GetLength(1); j++)
                    for (int k = 0; k < DensityGrid.GetLength(2); k++)
                    {
                        double R = Math.Round(Math.Sqrt(Math.Pow(j - DensityGrid.GetLength(1) / 2, 2) + Math.Pow(k - DensityGrid.GetLength(2) / 2, 2)));
                        double Angle = Math.Round(MathHelpLib.MathHelps.GetAngleDegrees(j - DensityGrid.GetLength(1) / 2, k - DensityGrid.GetLength(2) / 2));
                        if (R > 0 && R < Radiuss.Length)
                        {
                            double[,] ImageData = Radiuss[(int)R];
                            DensityGrid[i, j, k] = ImageData[i, (int)Angle];
                        }
                    }
            }

            /* for (int i = 0; i < DensityGrid.GetLength(0); i++)
                 for (int j = 0; j < DensityGrid.GetLength(1); j++)
                     for (int k = 0; k < DensityGrid.GetLength(2); k++)
                         DensityGrid[i, j, k] /= VisitCount[i, j, k];
           */

            AllTurnDir = new PointF(AllTurnDir.X / TurnCount, AllTurnDir.Y / TurnCount);

        }

        Point Down;
        Point Current = new Point(1, 0);
        Rectangle CutRect;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Down = new Point(e.X, e.Y);
            //  Current = new Point(e.X - pictureBox1.Width / 2, e.Y - pictureBox1.Height / 2);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {


                }
                catch { }

            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int MinX, MinY, Width, Height;
                if (Down.X < e.X)
                    MinX = Down.X;
                else
                    MinX = e.X;
                if (Down.Y < e.Y)
                    MinY = Down.Y;
                else
                    MinY = e.Y;
                CutRect = new Rectangle(MinX, MinY, Math.Abs(e.X - Down.X), Math.Abs(e.Y - Down.Y));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _3dView viewer = new _3dView();

            viewer.SetData(DensityGrid);
            viewer.Show();
        }




    }
}
