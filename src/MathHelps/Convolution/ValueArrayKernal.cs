using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MathHelpLib.Convolution
{
    public class ValueArrayKernal: IConvolutionKernal
    {
        int[] mRank;
        double[,] Kernal;
        double Divide;

        /// <summary>
        /// This is a very standard convolution.  just the values of the kernal times the values under the kernal
        /// </summary>
        /// <param name="Kernal"></param>
        public ValueArrayKernal( double[,] Kernal)
        {
            mRank = new int[]{Kernal.GetLength(0), Kernal.GetLength(1)};
            Divide = 0;
            for (int i = 0; i < Kernal.GetLength(0); i++)
                for (int j = 0; j < Kernal.GetLength(1); j++)
                    Divide += Kernal[i, j];
            this.Kernal = Kernal;
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
            //multiply each of the convolution kernals times the pixel values
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    sum += LineStarts[j][i]*Kernal[i,j];
                }
            }
            return sum;
        }

        public unsafe Int32 RunKernal(Int32*[] LineStarts)
        {
            int R = Rank[0];
            double sumB = 0,sumR=0,sumG=0;
            //multiply each of the color values times the convolution kernals
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    sumB += *((byte*)LineStarts[j] + 4* i) *Kernal[i,j];
                }
            }
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    sumG += *((byte*)LineStarts[j] + 4 * i + 1) * Kernal[i, j];
                }
            }
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < LineStarts.Length; j++)
                {
                    sumR += *((byte*)LineStarts[j] + 4 * i + 2) * Kernal[i, j];
                }
            }
            sumB = sumB / Divide;
            sumR = sumR / Divide;
            sumG = sumG / Divide;

            if (sumB < 0) sumB = 0;
            if (sumR < 0) sumR = 0;
            if (sumG < 0) sumG = 0;

            if (sumB > 255) sumB = 255;
            if (sumR > 255) sumR = 255;
            if (sumG > 255) sumG = 255;

            if (double.IsNaN(sumB)) sumB = 0;
            if (double.IsNaN(sumR)) sumR = 0;
            if (double.IsNaN(sumG)) sumG = 0;
            return Color.FromArgb((int)sumR, (int)sumG, (int)sumB).ToArgb();
        }

        public double RunKernal(double[,] NeighBors)
        {
            int R = Rank[0];
            double sum = 0;
            for (int i = 0; i < Rank[0]; i++)
            {
                for (int j = 0; j < NeighBors.Length; j++)
                {
                    sum += NeighBors[i,j ] * Kernal[i, j];
                }
            }
            return sum;
        }
    }
}
