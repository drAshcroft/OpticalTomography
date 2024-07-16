using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using MathHelpLib.ImageProcessing;
using MathHelpLib;
using System.Drawing.Imaging;

namespace Tomographic_Imaging_2.ImageSelection
{
    public partial class ImageStatistics : DockContent 
    {
        public ImageStatistics()
        {
            InitializeComponent();
        }
        public void ShowStatistics(ImageViewer.ISelection Selection, Bitmap ActiveImage)
        {
            if (Selection == null)
                return;
            if (Selection is ImageViewer.Selections.ProfileSelection)
            {
                panel1.Visible = true;
                panel1.BringToFront();
                panel2.Visible = false;
                ImageViewer.Selections.ProfileSelection ps= (ImageViewer.Selections.ProfileSelection)Selection;
                int [] Profile=  ActiveImage.GetProfileLine(ps.P1, ps.P2);
                graph1D1.SetData( MathImageHelps.ConvertToIntensity( Profile) );
                graph1D1.Visible = true;
            }
            else
            {
                panel1.Visible = false;
                panel2.Visible = true;
                panel2.BringToFront();
                double[] Red, Green, Blue;
                ClipImage(ActiveImage, Selection, out Red, out Green, out Blue);
                int [] LRed, LGreen, LBlue;
                LRed = MakeHistogram(Red, 100);
                LBlue = MakeHistogram(Blue, 100);
                LGreen = MakeHistogram(Green, 100);
                
                histogram1.Values = LRed ;
                histogram2.Values = LGreen;
                histogram3.Values = LBlue;

                ColorImageStatisticsDescription statDesc = new ColorImageStatisticsDescription(MathImageHelps.ClipImage(ActiveImage,Selection.SelectionBounds ));
                // show statistics
                propertyGrid.SelectedObject = statDesc;
                propertyGrid.ExpandAllGridItems();
                //int[] Histogram = MakeHistogram(ClipImage(ActiveImage, Selection),100);
                /*
                double[] Red,Green,Blue;
                ClipImage(ActiveImage, Selection,out Red, out Green, out Blue );
                double[,] LRed,LGreen,LBlue;
                LRed = MakeHistogram(Red, 25);
                LBlue = MakeHistogram(Blue, 25);
                LGreen = MakeHistogram(Green, 25);
                List<double[,]> Lines = new List<double[,]>();
                Lines.Add(LRed);
                Lines.Add(LBlue);
                Lines.Add(LGreen);
                graph1D1.SetData(MathGraphTypes.Graph1D_Bar, Lines, "Intensity", "");
                double[,] data = MakeHistogram(ClipImage(ActiveImage, Selection), 30);
                graph1D1.SetData(MathGraphTypes.Graph1D_Bar, data, "Intensity", "");
                graph1D1.Invalidate();*/
            }
        }
        private int [] MakeHistogram(double[] Intensities, int NumBins)
        {
            double max = Intensities.MaxArray();
            double min = Intensities.MinArray();
            double step = (max - min) / (double)NumBins;
            int [] OutArray = new int [NumBins + 1];
            if (step != 0)
            {
                for (int i = 0; i < Intensities.Length; i++)
                {
                    int index = (int)Math.Truncate((Intensities[i] - min) / step);
                    OutArray[index]++;
                }
            }
            return OutArray;
        }

        private double[,] MakeHistogramIndexed(double[] Intensities, int NumBins)
        {
            double max = Intensities.MaxArray();
            double min = Intensities.MinArray();
            double step = (max - min) / (double)NumBins;
            double[,] OutArray = new double[2, NumBins+1];
            for (int i = 0; i < Intensities.Length; i++)
            {
                int index =(int) Math.Truncate  ((Intensities[i]-min)/step );
                OutArray[1,index]++;
            }
            for (int i = 0; i < OutArray.GetLength(1); i++)
                OutArray[0, i] = min + step * i;
            return OutArray;
        }
        private void ClipImage(Bitmap SourceImage, ImageViewer.ISelection Selection,out double[] Red, out double[] Green, out double[] Blue)
        {
            int iWidth = SourceImage.Width;
            int iHeight = SourceImage.Height;

            double[] ImageArrayR = new double[Selection.SelectionBounds.Width * Selection.SelectionBounds.Height];
            double[] ImageArrayG = new double[Selection.SelectionBounds.Width * Selection.SelectionBounds.Height];
            double[] ImageArrayB = new double[Selection.SelectionBounds.Width * Selection.SelectionBounds.Height];

            BitmapData bmd = SourceImage.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height), ImageLockMode.WriteOnly, SourceImage.PixelFormat);

            double g1, g2, g3;
            long t = 0;
            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = Selection.SelectionBounds.Top; y < Selection.SelectionBounds.Bottom; y += 2)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                        for (int x = Selection.SelectionBounds.Left; x < Selection.SelectionBounds.Right; x += 2)
                        {
                            if (Selection.PointInSelection(new Point(x, y)) == true)
                            {
                                byte* bits = (byte*)(scanline + x);
                                g1 = bits[0];
                                g2 = bits[1];
                                g3 = bits[2];

                                ImageArrayR[t] = g1;
                                ImageArrayG[t] = g2 ;
                                ImageArrayB[t] =g3 ;

                                t++;
                            }
                        }
                    }
                }
                else
                    throw new Exception("Does not support image formats other than 32 bits.  Please convert the image");
            }
            SourceImage.UnlockBits(bmd);
            if (t < ImageArrayR.Length)
            {
                Red = new double[t];
                Buffer.BlockCopy(ImageArrayR, 0, Red, 0, (int)(t * sizeof(double)));
                Green = new double[t];
                Buffer.BlockCopy(ImageArrayG, 0, Green, 0, (int)(t * sizeof(double)));
                Blue = new double[t];
                Buffer.BlockCopy(ImageArrayB, 0, Blue, 0, (int)(t * sizeof(double)));
            }
            else
            {
                Red = ImageArrayR;
                Blue = ImageArrayB;
                Green = ImageArrayG;
            }
        }
       
        private double[] ClipImage(Bitmap SourceImage, ImageViewer.ISelection Selection)
        {
            int iWidth = SourceImage.Width;
            int iHeight = SourceImage.Height;

            double [] ImageArray = new double [Selection.SelectionBounds.Width*Selection.SelectionBounds.Height  ];

            BitmapData bmd = SourceImage.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height), ImageLockMode.WriteOnly, SourceImage.PixelFormat);

            double g1, g2, g3;
            long t=0;
            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                    for (int y = Selection.SelectionBounds.Top; y < Selection.SelectionBounds.Bottom ; y+=2)
                    {
                        Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                        for (int x = Selection.SelectionBounds.Left ; x < Selection.SelectionBounds.Right ; x+=2)
                        {
                            if (Selection.PointInSelection(new Point(x, y)) == true)
                            {
                                byte* bits = (byte*)(scanline+x);
                                g1 = bits[0];
                                g2 = bits[1];
                                g3 = bits[2];

                                ImageArray[t] = Math.Sqrt(g1 * g1 + g2 * g2 + g3 * g3);
                               
                                t++;
                            }
                        }
                    }
                }
                else
                    throw new Exception("Does not support image formats other than 32 bits.  Please convert the image");
            }
            SourceImage.UnlockBits(bmd);
            if (t < ImageArray.Length)
            {
                double[] ActualLength = new double[t];
                Buffer.BlockCopy(ImageArray, 0, ActualLength, 0,(int) (t * sizeof(double)));
                return ActualLength;
            }
            else 
                return ImageArray;
        }
       
    }
}
