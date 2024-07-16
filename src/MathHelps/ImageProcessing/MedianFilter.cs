using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MathHelpLib.ImageProcessing
{
    public  class MedianKernal:IConvolutionKernal 
    {
        int[] mRank;
        public MedianKernal( int[] Rank)
        {
            mRank = Rank;
        }
        public MedianKernal(int Rank)
        {
            mRank =new int[]{ Rank,Rank };
        }
        public int[] Rank
        {
            get { return  mRank ; }
        }
        public int[] MaxRank
        {
            get { return new int[] { 25, 25 }; }
        }

        public unsafe double RunKernal(double*[] LineStarts)
        {
            double [] supp = new double [Rank[0] * Rank[1]];
            int t = 0;
            
            //get all the values into an array
            for (int j = 0; j < Rank[1]; ++j)
            {
                for (int i = 0; i < Rank[0]; ++i)
                {

                    supp[t] = LineStarts[j][i];
                    t++;
                }
            }

            //sort the array and then take the median values
            Array.Sort(supp);
            return supp[((Rank[0] * Rank[1] - 1) / 2)];
        }

        public unsafe Int32 RunKernal(Int32*[] LineStarts)
        {
            //get arrays for each of the colors
            double[] suppR = new double[Rank[0] * Rank[1]];
            double[] suppG = new double[Rank[0] * Rank[1]];
            double[] suppB = new double[Rank[0] * Rank[1]];
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
            
            //sort the arrays and take the middle values
            Array.Sort(suppB);
            Array.Sort(suppR);
            Array.Sort(suppG);
            t= ((Rank[0] * Rank[1] - 1) / 2);
            return Color.FromArgb((int)suppR[t], (int)suppG[t], (int)suppB[t]).ToArgb();
        }
        /*
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

        }*/

        public double RunKernal(double[,] NeighBors)
        {
            double[] supp = new double[Rank[0] * Rank[1]];
            int t = 0;
            int number = 0;
            //get the values into a linear array, 
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

            //sort the values and take the middle
            Array.Sort(supp);
            return supp[((Rank[0] * Rank[1] - 1) / 2)];
        }
       /* public Int32 RunKernal(Int32[,] NeighBors)
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
        }*/
    }
}
