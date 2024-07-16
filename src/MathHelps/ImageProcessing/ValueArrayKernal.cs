using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MathHelpLib.ImageProcessing
{
    /// <summary>
    /// used for any numerical kernal.  i.e. sobel filters, guassian filters...
    /// </summary>
    public class ValueArrayKernal: IConvolutionKernal
    {
        int[] mRank;
        double[,] Kernal;
        public ValueArrayKernal( double[,] Kernal)
        {
            mRank = new int[]{Kernal.GetLength(0), Kernal.GetLength(1)};
            this.Kernal = Kernal;
        }
        
        public int[] Rank
        {
            get { return  mRank ; }
        }
        public int[] MaxRank
        {
            get { return new int[] { 100, 100 }; }
        }

        public unsafe double RunKernal(double*[] LineStarts)
        {
            int R = Rank[0];
            double sum = 0;
            //multiple the kernal with the image for this pixel
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
            //multiple the kernal times each of the colors
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
