using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ImageViewer.Convolution;

namespace ImageViewer.Convolution
{
    public  class MaxKernal:IConvolutionKernal 
    {
        int[] mRank;
       
        public MaxKernal( int[] Rank)
        {
            mRank = Rank;
        }

        public MaxKernal(int Rank)
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
            double M = double.MinValue ;
            //find the max value for the range under the kernal
            for (int j = 0; j < Rank[1]; ++j)
            {
                for (int i = 0; i < Rank[0]; ++i)
                {
                    if (LineStarts[j][i] > M) M = LineStarts[j][i];
                   
                }
            }
            return M;
        }

        public unsafe Int32 RunKernal(Int32*[] LineStarts)
        {
            
            int t = 0;
            int suppB = int.MinValue;
            int suppG = int.MinValue;
            int suppR = int.MinValue;
            //find the max value for each color for the range under the kernal
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    t = *((byte*)LineStarts[j] + 4 * i);
                    if (suppB < t) suppB = t;
                }
            }
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    t = *((byte*)LineStarts[j] + 4 * i+1);
                    if (suppG < t) suppG = t;
                }
            }

            t = 0;
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    t = *((byte*)LineStarts[j] + 4 * i+2);
                    if (suppR < t) suppR = t;
                }
            }
            
            return Color.FromArgb(suppR, suppG, suppB).ToArgb();
        }

        public double RunKernal(double[,] NeighBors)
        {
            return 0;
        }
      
    }
}
