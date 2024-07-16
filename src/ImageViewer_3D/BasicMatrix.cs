using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageViewer3D
{
    public static class BasicMatrix
    {
        public static float[,] Multiply(float[,] Matrix1, float[,] Matrix2)
        {
            float[,] MatrixOut = new float[Matrix2.GetLength(0), Matrix2.GetLength(1)];
            for (int i = 0; i < MatrixOut.GetLength(0); i++)
            {
                for (int j = 0; j < MatrixOut.GetLength(1); j++)
                {
                    for (int m = 0; m < Matrix1.GetLength(0); m++)
                    {
                        MatrixOut[i, j] += Matrix1[ m, j] * Matrix2[i, m]; 
                    }
                }
            }
            return MatrixOut;
        }

        public static float[,] Transpose(float[,] matrix)
        {
            float[,] a=new float[matrix.GetLength(1),matrix.GetLength(0)];
            for (int i=0;i<matrix.GetLength(0);i++)
                for (int j=0;j<matrix.GetLength(1);j++)
                    a[j,i]=matrix[ i,j];
            return a;
        }

        public static float[,] Invert(float[,] matrix)
        {
            float k = matrix.GetLength(0);
            float d = detrm(matrix,k);
            if (d == 0)
                throw new Exception("Matrix is not invertable");
            else
                return cofact(matrix, k);
        }
        /******************FUNCTION TO FIND THE DETERMINANT OF THE MATRIX************************/
        private static float detrm(float[,] a, float k)
        {
            float s = 1, det = 0;
            float[,] b = new float[a.GetLength(0), a.GetLength(1)];
            int i, j, m, n, c;
            if (k == 1)
            {
                return (a[0, 0]);
            }
            else
            {
                det = 0;
                for (c = 0; c < k; c++)
                {
                    m = 0;
                    n = 0;
                    for (i = 0; i < k; i++)
                    {
                        for (j = 0; j < k; j++)
                        {
                            b[i, j] = 0;
                            if (i != 0 && j != c)
                            {
                                b[m, n] = a[i, j];
                                if (n < (k - 2))
                                    n++;
                                else
                                {
                                    n = 0;
                                    m++;
                                }
                            }
                        }
                    }
                    det = det + s * (a[0, c] * detrm(b, k - 1));
                    s = -1 * s;
                }
            }
            return (det);
        }
        /*******************FUNCTION TO FIND COFACTOR*********************************/
        private static float[,] cofact(float[,] num, float f)
        {
            float[,] b = new float[num.GetLength(0), num.GetLength(1)];
            float[,] fac = new float[num.GetLength(0), num.GetLength(1)];
            int p, q, m, n, i, j;
            for (q = 0; q < f; q++)
            {
                for (p = 0; p < f; p++)
                {
                    m = 0;
                    n = 0;
                    for (i = 0; i < f; i++)
                    {
                        for (j = 0; j < f; j++)
                        {
                            b[i, j] = 0;
                            if (i != q && j != p)
                            {
                                b[m, n] = num[i, j];
                                if (n < (f - 2))
                                    n++;
                                else
                                {
                                    n = 0;
                                    m++;
                                }
                            }
                        }
                    }
                    fac[q, p] = (float)Math.Pow(-1, q + p) * detrm(b, f - 1);
                }
            }
            return Inverse(num, fac, f);
        }
        /*************FUNCTION TO FIND TRANSPOSE AND INVERSE OF A MATRIX**************************/
        private static float[,] Inverse(float[,] num, float[,] fac, float r)
        {
            int i = 0, j = 0;
            float[,] b = new float[num.GetLength(0), num.GetLength(1)];
            float[,] inv = new float[num.GetLength(0), num.GetLength(1)];
            float d;
            for (i = 0; i < r; i++)
            {
                for (j = 0; j < r; j++)
                {
                    b[i, j] = fac[j, i];
                }
            }

            d = detrm(num, r);
            inv[0, 0] = 0;
            for (i = 0; i < r; i++)
            {
                for (j = 0; j < r; j++)
                {
                    inv[i, j] = b[i, j] / d;
                }
            }
            return inv;
        }
    }
}
