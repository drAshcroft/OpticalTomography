﻿

#if DEBUG
// #define TESTING
#endif


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Utilities;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageProcessing._3D;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using MathLibrary;
using ImageProcessing;
using System.Diagnostics;

namespace ReconstructCells.Background
{
    class DivideFlatten_NoDivide : ReconstructNodeTemplate
    {
        Image<Gray, float> BackGround;
        CellLocation[] Locations;
        int CellSize; //determined in code 
        int suggestedCellSize;
        //   float BackgroundAverage;
        OnDemandImageLibrary Library;
        int TrimSize = 10;

        float[] ImageMinValues;


        int Buffer = 30;



        #region Properties

        #region Set

        public void setSuggestedCellSize(int cellSize)
        {
            suggestedCellSize = cellSize;
        }
        public void setBackground(Image<Gray, float> backGround)
        {
            BackGround = backGround;
        }

        #endregion

        #endregion

        #region Code

        private PointF NoisyFinder(Image<Gray, float> image)
        {

            float xx = 0, yy = 0;
            for (int i = 0; i < 5; i++)
            {
                Bitmap b = image.ScaledBitmap;
                MathLibrary.Signal_Processing.ImageFilters.AddJitter(ref b);

                Image<Gray, float> image2 = new Image<Gray, float>(b);

                var moments = image2.GetMoments(false);

                xx += (float)moments.GravityCenter.x;
                yy += (float)moments.GravityCenter.y;
            }

            return new PointF(xx / 5f, yy / 5f);
        }


        private void BatchRemoveBackground(int imageNumber)
        {
            try
            {
                double[] min, max;
                Point[] pm, pM;
                Buffer = 0;

                var image = Library[imageNumber];

                CellLocation cellLocation = new CellLocation(new PointF(image.Width / 2, image.Height / 2), CellSize, imageNumber);
                int minCellX = (int)(cellLocation.CellCenter.X - CellSize / 2 - Buffer);
                int maxCellX = (int)(cellLocation.CellCenter.X + CellSize / 2 + Buffer);
                int minCellY = (int)(cellLocation.CellCenter.Y - CellSize / 2 - Buffer);
                int maxCellY = (int)(cellLocation.CellCenter.Y + CellSize / 2 + Buffer);

                Rectangle ROI = new Rectangle(minCellX, minCellY, maxCellX - minCellX, maxCellY - minCellY);
                int cellHalf = (int)((ROI.Width) / 2d);

                image.ROI = ROI;
                image = image.Copy();
                image.ROI = Rectangle.Empty;

                image.MinMax(out min, out max, out pm, out pM);

                int sizeIncrease = mPassData.DataScaling;
                var image2 = image.Copy();


                #region refine cutting
                PointF p = NoisyFinder(image2);
                int nCellSize = (int)(Math.Floor(((double)ROI.Width * sizeIncrease - TrimSize * sizeIncrease) / 2));

                ROI = new Rectangle((int)(Math.Floor(p.X - nCellSize)), (int)(Math.Floor(p.Y - nCellSize)), 2 * nCellSize, 2 * nCellSize);

                cellLocation.CellCenter.X += (int)(p.X - cellHalf);
                cellLocation.CellCenter.Y += (int)(p.Y - cellHalf);

                // beforeRoughness[imageNumber] = cellLocation.CellCenter.X;
                // afterRoughness[imageNumber] = cellLocation.CellCenter.Y;
                if ((ROI.X >= 0 && ROI.Right < image.Width && ROI.Y >= 0 && ROI.Bottom < image.Height))
                {
                    image = image.Copy(ROI);
                }
                else
                {
                    #region boundry Problems ___ lazy problem
                    Image<Gray, float> nImage = new Image<Gray, float>(2 * nCellSize, 2 * nCellSize);

                    var ROIdest = new Rectangle(0, 0, 2 * nCellSize, 2 * nCellSize);
                    var ROIsrc = new Rectangle(ROI.X, ROI.Y, ROI.Width, ROI.Height);

                    if (ROI.X < 0)
                    {
                        ROIdest.X = -1 * ROI.X;
                        int right = ROI.Right;
                        ROIsrc.X = 0; ROIsrc.Width = right;
                        ROIdest.Width = right;
                    }

                    if (ROI.Y < 0)
                    {
                        ROIdest.Y = -1 * ROI.Y;
                        int bottom = ROI.Bottom;
                        ROIsrc.Y = 0; ROIsrc.Height = bottom;
                        ROIdest.Height = bottom;
                    }

                    if (ROI.Right > image.Width)
                    {
                        ROIsrc.Width = image.Width - ROI.X;
                        ROIdest.Width = ROIsrc.Width;
                    }

                    if (ROI.Bottom > image.Height)
                    {
                        ROIsrc.Height = image.Height - ROI.Y;
                        ROIdest.Height = ROIsrc.Height;
                    }

                    // nImage.ROI = ROIdest;
                    // image.ROI = ROIsrc;

                    //nImage = image.Copy();
                    int xx = ROIdest.X;
                    for (int x = ROIsrc.X; x < ROIsrc.Right; x++)
                    {
                        int yy = ROIdest.Y;
                        for (int y = ROIsrc.Y; y < ROIsrc.Bottom; y++)
                        {
                            nImage.Data[yy, xx, 0] = image.Data[y, x, 0];
                            yy++;
                        }
                        xx++;
                    }

                    //image.CopyTo(nImage);
                    image = nImage;
                    #endregion
                }

                Library[imageNumber] = image;
                #endregion

            }
            catch (Exception ex)
            {
                Program.WriteTagsToLog("divide error", ex.Message + "/n/n/n" + ex.StackTrace);
                Library[imageNumber] = Library[0].CopyBlank();

            }

            //  Library[imageNumber] = image;
        }


        private void RemoveBackgrounds()
        {
            CellSize = Library[11].Width;


           // ParallelOptions po = new ParallelOptions();
          //  po.MaxDegreeOfParallelism = Environment.ProcessorCount - 1;

            int numberOfImages = Library.Count;
            Parallel.For(0, numberOfImages, Program.threadingParallelOptions, x => BatchRemoveBackground(x));
        }

        #endregion

        protected override void RunNodeImpl()
        {
            Library = mPassData.Library;
            Locations = mPassData.Locations;

            ImageMinValues = new float[Library.Count];

            RemoveBackgrounds();


        }
    }
}
