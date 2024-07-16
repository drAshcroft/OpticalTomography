using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu;
using ImageViewer;
using MathHelpLib;
using System.Drawing;
using Emgu.CV.Structure;

namespace DoRecons.Scripts
{
    class GoodFeaturesToTrackFinder
    {
        public static void GoodFeatureCenter(DataEnvironment dataEnvironment, int CellSize, double[] X_Positions, double[] Y_Positions)
        {
            if (CellSize > 200) CellSize = 200;
            CellSize *= 2;
            int CellHalf = CellSize / 2;

            PointF[] Centroids = new PointF[dataEnvironment.AllImages.Count];
            for (int i = 0; i < dataEnvironment.AllImages.Count; i++)
            {
                ImageHolder example = ReconCellFinder.ClipImage(dataEnvironment, i, X_Positions, Y_Positions, CellHalf, CellSize);
                Emgu.CV.Image<Gray, float> exampleE = new Emgu.CV.Image<Gray, float>(example.ImageData);
                PointF[][] features =  exampleE.GoodFeaturesToTrack(25, .7, 10, 15);

                int cc = 0;
                for (int j = 0; j < features.Length; j++)
                {
                    for (int k = 0; k < features[j].Length; k++)
                    {
                        Centroids[i].X += features[j][k].X;
                        Centroids[i].Y += features[j][k].Y;
                        cc++;
                    }
                }
                Centroids[i].X /= cc;
                Centroids[i].Y /= cc;
            }

            for (int i = 0; i < dataEnvironment.AllImages.Count; i++)
            {
                X_Positions[i] += Centroids[i].X - Centroids[0].X;
                Y_Positions[i] += Centroids[i].Y - Centroids[0].Y;
            }
        }

    }
}
