using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MathHelpLib.Convolution
{
    public class AverageConvolutionKernal : IConvolutionKernal
    {
        int[] mRank;
        public AverageConvolutionKernal( int[] Rank)
        {
            mRank = Rank;
        }
        public AverageConvolutionKernal(int Rank)
        {
            mRank =new int[]{ Rank,Rank };
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
            int R = Rank[0];
            double sum = 0;
            ///sum all the elements and get the average
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    sum += LineStarts[j][i];
                }
            }
            return sum / (double)(Rank[0] * Rank[1]);
        }

        public unsafe Int32 RunKernal(Int32*[] LineStarts)
        {
            int R = Rank[0];
            double sumB = 0,sumR=0,sumG=0;
            ///sum all the elements and get the average, seperating the three channels
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    sumB += *((byte*)LineStarts[j] + 4* i);
                }
            }
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    sumG += *((byte*)LineStarts[j] + 4 * i+1);
                }
            }
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    sumR += *((byte*)LineStarts[j] + 4 * i+2);
                }
            }
            double denom= (double)(Rank[0] * Rank[1]);
            int  bsumB = (int )( sumB /denom );
            int  bsumG =(int ) (sumG /denom);
            int  bsumR= (int )( sumR/denom);

            return Color.FromArgb(bsumR, bsumG, bsumB).ToArgb();
        }

      /*  public unsafe byte RunKernal(byte*[] LineStarts, int Channels)
        {
            int R = Rank[0];
            double sum = 0;
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    sum += *(LineStarts[j] + i * Channels);
                }
            }
            return (byte)(sum / (double)(Rank[0] * Rank[1]));
        }
        */
        public double RunKernal(double[,] NeighBors)
        {
            double sum = 0;
            ///sum all the elements and get the average
            for (int i = 0; i < NeighBors.GetLength(0); i++)
            {
                for (int j = 0; j < NeighBors.GetLength(1); j++)
                {
                    sum += NeighBors[i, j];
                }
            }
            return sum / (double)NeighBors.Length;
        }

       /* public Int32 RunKernal(Int32[,] NeighBors)
        {
            double sum = 0;
            for (int i = 0; i < NeighBors.GetLength(0); i++)
            {
                for (int j = 0; j < NeighBors.GetLength(1); j++)
                {
                    sum += NeighBors[i, j];
                }
            }
            return (Int32)(sum / (double)NeighBors.Length);
        }
        public byte RunKernal(byte[,] NeighBors)
        {
            double sum = 0;
            for (int i = 0; i < NeighBors.GetLength(0); i++)
            {
                for (int j = 0; j < NeighBors.GetLength(1); j++)
                {
                    sum += NeighBors[i, j];
                }
            }
            return (byte)(sum / (double)NeighBors.Length);
        }
        */
    }
}
