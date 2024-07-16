using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MathHelpLib.ImageProcessing
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

        /// <summary>
        /// anything bigger than this is difficult
        /// </summary>
        public int[] MaxRank
        {
            get { return new int[]{25,25} ; }
        }

        public unsafe double RunKernal(double*[] LineStarts)
        {
            int R = Rank[0];
            double sum = 0;
            ///add the data and do the average
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
            //average the data from each of the RGB components
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

       /* public unsafe byte RunKernal(byte*[] LineStarts, int Channels)
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
        }*/

        public double RunKernal(double[,] NeighBors)
        {
            //just use the .net call to average the data
            return NeighBors.AverageArray();
        }
      /*  public Int32 RunKernal(Int32[,] NeighBors)
        {
            return (Int32)NeighBors.AverageArray();
        }
        public byte RunKernal(byte[,] NeighBors)
        {
            return (byte)NeighBors.AverageArray();
        }*/

    }
}
