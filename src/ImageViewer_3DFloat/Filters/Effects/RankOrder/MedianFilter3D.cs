using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ImageViewer3D.Convolution;

namespace ImageViewer3D.Filters.Effects.RankOrder
{
    public class MedianKernal3D : IConvolutionKernal3D
    {
        int[] mRank;
        float[] supp;
        float[] suppR;
        float[] suppG;
        float[] suppB;
        public MedianKernal3D(int[] Rank)
        {
            mRank = Rank;
            supp = new float[mRank[0] * mRank[1] * mRank[2]];
            suppR = new float[mRank[0] * mRank[1] * mRank[2]];
            suppG = new float[mRank[0] * mRank[1] * mRank[2]];
            suppB = new float[mRank[0] * mRank[1] * mRank[2]];
        }
        public MedianKernal3D(int Rank)
        {
            mRank = new int[] { Rank, Rank, Rank };
            supp = new float[mRank[0] * mRank[1] * mRank[2]];
            suppR = new float[mRank[0] * mRank[1] * mRank[2]];
            suppG = new float[mRank[0] * mRank[1] * mRank[2]];
            suppB = new float[mRank[0] * mRank[1] * mRank[2]];
        }
        public int[] Rank
        {
            get { return mRank; }
        }
        public int[] MaxRank
        {
            get { return mRank; }
        }

        public unsafe float RunKernal(float*[] LineStarts)
        {
            int R = Rank[0];
            int t = 0;
            float* LineStart;
            for (int i = 0; i < LineStarts.Length; ++i)
            {
                LineStart = LineStarts[i];
                for (int j = 0; j < R; ++j)
                {
                    supp[t] = *LineStart;
                    LineStart++;
                    t++;
                }
            }

            Array.Sort(supp);
            return supp[((R * LineStarts.Length - 1) / 2)];
        }

        public float RunKernal(float[,] NeighBors)
        {
            float[] supp = new float[Rank[0] * Rank[1]];
            int t = 0;
            int number = 0;
            for (int j = 0; j < Rank[1]; ++j)
            {
                for (int i = 0; i < Rank[0]; ++i)
                {

                    supp[t] = NeighBors[i, j];
                    t++;
                    ++number;
                }
            }
            if (number == 0)
                return 0;

            Array.Sort(supp);
            return supp[((Rank[0] * Rank[1] - 1) / 2)];
        }
    }
}
