using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ImageViewer.Convolution;

namespace ImageViewer.Filters.Effects.RankOrder
{
    public class MedianKernal:IConvolutionKernal 
    {
        int[] mRank; 
        double[] supp;
        double[] suppR;
        double[] suppG;
        double[] suppB;
        public MedianKernal( int[] Rank)
        {
            mRank = Rank;
            supp = new double[mRank[0] * mRank[1]];
            suppR = new double[mRank[0] * mRank[1]];
            suppG = new double[mRank[0] * mRank[1]];
            suppB = new double[mRank[0] * mRank[1]];
        }
        public MedianKernal(int Rank)
        {
            mRank =new int[]{ Rank,Rank };
            supp = new double[mRank[0] * mRank[1]];
            suppR = new double[mRank[0] * mRank[1]];
            suppG = new double[mRank[0] * mRank[1]];
            suppB = new double[mRank[0] * mRank[1]];
        }
        public int[] Rank
        {
            get { return  mRank ; }
        }
        public int[] MaxRank
        {
            get { return mRank ; }
        }

        public unsafe double RunKernal(double*[] LineStarts)
        {
            
            int t = 0;
            
            for (int j = 0; j < Rank[1]; ++j)
            {
                for (int i = 0; i < Rank[0]; ++i)
                {

                    supp[t] = LineStarts[j][i];
                    t++;
                }
            }

            Array.Sort(supp);
            return supp[((Rank[0] * Rank[1] - 1) / 2)];
        }

        public unsafe Int32 RunKernal(Int32*[] LineStarts)
        {
            
            int t = 0;
           

            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    suppB[t] = *((byte*)LineStarts[j] + 4 * i);
                    t++;
                }
            }
            t = 0;
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    suppG[t] = *((byte*)LineStarts[j] + 4 * i+1);
                    t++;
                }
            }

            t = 0;
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    suppR[t] = *((byte*)LineStarts[j] + 4 * i+2);
                    t++;
                }
            }
            

            Array.Sort(suppB);
            Array.Sort(suppR);
            Array.Sort(suppG);
            t= ((Rank[0] * Rank[1] - 1) / 2);
            return Color.FromArgb((int)suppR[t], (int)suppG[t], (int)suppB[t]).ToArgb();
        }

        public unsafe byte RunKernal(byte*[] LineStarts, int Channels)
        {
            byte [] supp = new byte [Rank[0] * Rank[1]];
            int t = 0;
            int number = 0;
            for (int j = 0; j < Rank[1]; ++j)
            {
                for (int i = 0; i < Rank[0]; ++i)
                {

                    supp[t] = *(LineStarts[j] + i*Channels);
                    t++;
                    ++number;
                }
            }
            if (number == 0)
                return 0;

            Array.Sort(supp);
            return supp[((Rank[0] * Rank[1] - 1) / 2)];

        }

        public double RunKernal(double[,] NeighBors)
        {
            double[] supp = new double[Rank[0] * Rank[1]];
            int t = 0;
            int number = 0;
            for (int j = 0; j < Rank[1]; ++j)
            {
                for (int i = 0; i < Rank[0]; ++i)
                {

                    supp[t] = NeighBors [i, j];
                    t++;
                    ++number;
                }
            }
            if (number == 0)
                return 0;

            Array.Sort(supp);
            return supp[((Rank[0] * Rank[1] - 1) / 2)];
        }
        public Int32 RunKernal(Int32[,] NeighBors)
        {
            Int32[] supp = new Int32[Rank[0] * Rank[1]];
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
        public byte RunKernal(byte[,] NeighBors)
        {
            byte [] supp = new byte [Rank[0] * Rank[1]];
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

        public int median(int[,] input, int Rank)
        {
            int[] supp = new int[Rank * Rank];
            int t = 0;
            int number = 0;
            for (int j = 0; j < Rank; ++j)
            {
                for (int i = 0; i < Rank; ++i)
                {
                   
                        supp[t] = input[ i, j];
                        t++;
                        ++number;
                }
            }
            if (number == 0)
                return 0;

            Array.Sort(supp);
            return supp[((Rank * Rank - 1) / 2)];
        }
    }
}
