using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ImageViewer.Convolution;
using ImageViewer3D.Convolution;

namespace ImageViewer3D.Filters.Effects.RankOrder
{
    public class AverageConvolutionKernal3D : IConvolutionKernal3D
    {
        int[] mRank;
        public AverageConvolutionKernal3D(int[] Rank)
        {
            mRank = Rank;
        }
        public AverageConvolutionKernal3D(int Rank)
        {
            mRank = new int[] { Rank, Rank, Rank };
        }
        public int[] Rank
        {
            get { return mRank; }
        }
        public int[] MaxRank
        {
            get { return mRank; }
        }

        public unsafe double RunKernal(double*[] LineStarts)
        {
            int R = Rank[0];
            double sum = 0;
            double* LineStart;

            for (int j = 0; j < LineStarts.Length; j++)
            {
                LineStart = LineStarts[j];
                for (int i = 0; i < R; i++)
                {
                    sum += *LineStart;
                    LineStart++;
                }
            }
            return sum / (double)(Rank[0]*Rank[1]*Rank[2]);
        }
        public double RunKernal(double[,] NeighBors)
        {
            double sum = 0;
            for (int i = 0; i < NeighBors.GetLength(0); i++)
            {
                for (int j = 0; j < NeighBors.GetLength(1); j++)
                {
                    sum += NeighBors[i, j];
                }
            }
            return sum / (double)NeighBors.Length;
        }



    }
}
