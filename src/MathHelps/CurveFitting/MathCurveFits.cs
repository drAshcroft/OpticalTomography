using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathHelpLib.CurveFitting
{
    public static  class MathCurveFits
    {
        #region CurveFits
        public static double[,] FitMovingAverage(double[,] ScatterData, int Period)
        {
            double[,] outArray = new double[2, ScatterData.GetLength(1)];
            int H = (int)Math.Truncate((double)Period / 2d);
            if (ScatterData.GetLength(1) > 2 * H)
            {
                for (int i = 0; i < H; i++)
                {
                    outArray[0, i] = ScatterData[0, i];
                    for (int j = 0; j <= 2 * i; j++)
                    {
                        outArray[1, i] += ScatterData[1, j];
                    }
                    outArray[1, i] = outArray[1, i] / (2 * i + 1);
                }
                for (int i = H; i < ScatterData.GetLength(1) - H; i++)
                {
                    outArray[0, i] = ScatterData[0, i];
                    for (int j = i - H; j <= i + H; j++)
                    {
                        outArray[1, i] += ScatterData[1, j];
                    }
                    outArray[1, i] = outArray[1, i] / (2 * H + 1);
                }
                for (int i = ScatterData.GetLength(1) - H; i < ScatterData.GetLength(1); i++)
                {
                    outArray[0, i] = ScatterData[0, i];
                    int m = ScatterData.GetLength(1) - i;
                    for (int j = i; j < ScatterData.GetLength(1); j++)
                    {
                        outArray[1, i] += ScatterData[1, j];
                    }
                    outArray[1, i] = outArray[1, i] / (m);
                }
                return outArray;
            }
            else
                return ScatterData;
        }

        public static double[,] PolynomialFit(double[,] ScatterData, int Order)
        {
            double[,] outArray = new double[2, ScatterData.GetLength(1)];

            double[] coeff;
            LinearRegressionPoly(ScatterData, Order, out coeff);
            MathNet.Numerics.Polynomial p = new MathNet.Numerics.Polynomial(coeff);
            for (int i = 0; i < ScatterData.GetLength(1); i++)
            {
                outArray[0, i] = ScatterData[0, i];
                outArray[1, i] = p.Evaluate(ScatterData[0, i]);
            }
            return outArray;
        }

        public static double[,] TrigFit(double[,] ScatterData)
        {
            double[,] outArray = new double[2, ScatterData.GetLength(1)];

            //produce test data - a is a vecor with exact solution
            double[][] dataPoints;

            double Max = double.MinValue, Min = double.MaxValue;
            for (int i = 0; i < ScatterData.GetLength(1); i += 2)
            {
                if (ScatterData[1, i] > Max) Max = ScatterData[1, i];
                if (ScatterData[1, i] < Min) Min = ScatterData[1, i];
            }


            double[] a = { (Max - Min) / 2, 2 * Math.PI / ScatterData.GetLength(1), 0, (Max + Min) / 2 };
            LMAFunction f = new SineFunction();

            double[] xValues = new double[ScatterData.GetLength(1)];
            double[] yValues = new double[xValues.Length];

            for (int i = 0; i < xValues.Length; i++)
            {
                xValues[i] = ScatterData[0, i];
                yValues[i] = ScatterData[1, i];
            }

            dataPoints = new double[][] { xValues, yValues };


            LMA algorithm = new LMA(f, a, dataPoints, null, new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(4, 4), 1d - 20, 100);

            algorithm.Fit();

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = algorithm.Parameters[i];
            }


            for (int i = 0; i < ScatterData.GetLength(1); i++)
            {
                outArray[0, i] = ScatterData[0, i];
                outArray[1, i] = a[0] * Math.Sin(a[1] * ScatterData[0, i] + a[2]) + a[3];
            }
            return outArray;
        }


        public static double[,] PolynomialFitIndex(double[,] ScatterData, int Order, int OutLength)
        {
            double[,] outArray = new double[2, OutLength];

            double[] coeff;
            LinearRegressionPoly(ScatterData, Order, out coeff);
            MathNet.Numerics.Polynomial p = new MathNet.Numerics.Polynomial(coeff);
            for (int i = 0; i < OutLength; i++)
            {
                outArray[0, i] = i;
                outArray[1, i] = p.Evaluate(i);
            }
            return outArray;
        }

        public static double[,] TrigFitIndex(double[,] ScatterData, int OutLength)
        {
            double[,] outArray = new double[2, OutLength];

            //produce test data - a is a vecor with exact solution
            double[][] dataPoints;

            double length = ScatterData.GetLength(1);
            double offset = 0;
            for (int i = 0; i < ScatterData.GetLength(1); i++)
            {
                offset += ScatterData[1, i];
            }
            offset /= length;

            double MaxI = Math.PI / 2, Max = double.MinValue;
            double RMS = 0, d;
            for (int i = 0; i < ScatterData.GetLength(1); i++)
            {
                d = ScatterData[1, i] - offset;
                RMS += d * d;
                if (d > Max)
                {
                    Max = d;
                    MaxI = ScatterData[0, i];
                }
            }

            double amplitude = Math.Sqrt(2 * RMS / ScatterData.GetLength(1));
            double freq  = 2 * Math.PI / ScatterData.GetLength(1);
            double phase = Math.PI / 2 - freq * MaxI;

            double[] a = { amplitude, freq, phase , offset };
            LMAFunction f = new SineFunction();

            double[] xValues = new double[ScatterData.GetLength(1)];
            double[] yValues = new double[xValues.Length];

            for (int i = 0; i < xValues.Length; i++)
            {
                xValues[i] = ScatterData[0, i];
                yValues[i] = ScatterData[1, i];
            }

            dataPoints = new double[][] { xValues, yValues };


            LMA algorithm = new LMA(f, a, dataPoints, null, new DenseMatrix(4, 4), 1d - 20, 100);

            algorithm.Fit();

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = algorithm.Parameters[i];
            }


            for (int i = 0; i < OutLength; i++)
            {
                outArray[0, i] = i;
                outArray[1, i] = a[0] * Math.Sin(a[1] * i + a[2]) + a[3];
            }
            return outArray;
        }

        public static double[,] TrigFitIndexFixedFrequency(double[,] ScatterData, int OutLength,double Freq)
        {
            double[,] outArray = new double[2, OutLength];

            //produce test data - a is a vecor with exact solution
            double[][] dataPoints;

            double length = ScatterData.GetLength(1);
            double offset = 0;
            for (int i = 0; i < ScatterData.GetLength(1); i ++)
            {
                offset += ScatterData[1, i];
            }
            offset /= length;

            double MaxI=Math.PI/2, Max = double.MinValue;
            double RMS=0,d;
            for (int i = 0; i < ScatterData.GetLength(1); i ++)
            {
                d=ScatterData[1, i] - offset;
                RMS += d*d;
                if (d > Max)
                {
                    Max = d;
                    MaxI = ScatterData[0, i];
                }
            }

            double amplitude = Math.Sqrt(2 * RMS / ScatterData.GetLength(1));

            double phase = Math.PI/2 - Freq * MaxI;

            //double Freq = 2 * Math.PI / ScatterData.GetLength(1);
            double[] a = { amplitude ,  phase, offset};
            SineFunctionFixedFrequency f = new SineFunctionFixedFrequency();
            f.Frequency = Freq;

            double[] xValues = new double[ScatterData.GetLength(1)];
            double[] yValues = new double[xValues.Length];

            for (int i = 0; i < xValues.Length; i++)
            {
                xValues[i] = ScatterData[0, i];
                yValues[i] = ScatterData[1, i];
            }

            dataPoints = new double[][] { xValues, yValues };

            LMA algorithm = new LMA(f, a, dataPoints, null, new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(3, 3), 1d - 20, 100);

            algorithm.Fit();

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = algorithm.Parameters[i];
            }

           // string junk = "";
            for (int i = 0; i < OutLength; i++)
            {
                outArray[0, i] = i;
                outArray[1, i] = a[0] * Math.Sin(f.Frequency * i + a[1]) + a[2];
             //   junk += outArray[1, i] + "\n";
            }
            
            return outArray;
        }

        public static double[] GuassFit(double[,] ScatterData)
        {

            //produce test data - a is a vecor with exact solution
            double[][] dataPoints;

            //get max and min
            double Max = double.MinValue, Min = double.MaxValue;
            double Ave = 0, cc = 0;
            for (int i = 0; i < ScatterData.GetLength(1); i += 2)
            {
                if (ScatterData[1, i] > Max) Max = ScatterData[1, i];
                if (ScatterData[1, i] < Min) Min = ScatterData[1, i];
                Ave = Ave + ScatterData[0, i];
                cc++;
            }
            Ave = Ave / cc;

            double[] a = { Max - Min, Ave, Math.Abs((Ave - ScatterData[0, 0]) / 2d), Min };
            LMAFunction f = new GuassFunction();

            double[] xValues = new double[ScatterData.GetLength(1)];
            double[] yValues = new double[xValues.Length];

            for (int i = 0; i < xValues.Length; i++)
            {
                xValues[i] = ScatterData[0, i];
                yValues[i] = ScatterData[1, i];
            }

            dataPoints = new double[][] { xValues, yValues };


            LMA algorithm = new LMA(f, a, dataPoints, null, new DenseMatrix(4, 4), 1d - 20, 100);

            algorithm.Fit();

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = algorithm.Parameters[i];
            }

            return a;
        }

        public static double[] GuassDiffusionFit(double[,] ScatterData)
        {

            //produce test data - a is a vecor with exact solution
            double[][] dataPoints;

            //get max and min
            double Max = double.MinValue, Min = double.MaxValue;
            double Ave = 0, cc = 0;
            for (int i = 0; i < ScatterData.GetLength(1); i += 2)
            {
                if (ScatterData[1, i] > Max) Max = ScatterData[1, i];
                if (ScatterData[1, i] < Min) Min = ScatterData[1, i];
                Ave = Ave + ScatterData[0, i];
                cc++;
            }
            Ave = Ave / cc;

            double[] a = { 200, 100, 0 };
            LMAFunction f = new GuassDiffusionFunction();

            double[] xValues = new double[ScatterData.GetLength(1)];
            double[] yValues = new double[xValues.Length];

            for (int i = 0; i < xValues.Length; i++)
            {
                xValues[i] = ScatterData[0, i];
                yValues[i] = ScatterData[1, i];
            }

            dataPoints = new double[][] { xValues, yValues };


            LMA algorithm = new LMA(f, a, dataPoints, null, new DenseMatrix(3, 3), 1d - 20, 100);

            algorithm.Fit();

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = algorithm.Parameters[i];
            }

            return a;
        }

        public static void LinearRegressionPoly(double[,] ScatterData, int PolyOrder, out double[] coeff)
        {
            double[,] X = new double[ScatterData.GetLength(1), PolyOrder + 1];
            double[,] Y = new double[1, ScatterData.GetLength(1)];
            for (int i = 0; i < ScatterData.GetLength(1); i++)
            {
                double xV = 1;
                for (int j = 0; j <= PolyOrder; j++)
                {
                    X[i, j] = xV;
                    xV *= ScatterData[0, i];
                }
                Y[0, i] = ScatterData[1, i];
            }


           var  mX = Matrix<double>.Build.DenseOfArray(X);
            var  mY = Matrix<double>.Build.DenseOfArray(Y);
            mY.Transpose();
            var a = mX.Solve(mY);
            //System.Diagnostics.Debug.Print(a.ToString());
            coeff = new double[PolyOrder + 1];
            for (int i = 0; i <= PolyOrder; i++)
            {
                coeff[i] = a[i, 0];
            }

        }

        public static void LinearRegression(double[,] Scatterdata, out double slope, out double intercept)
        {
            double xAvg = 0;
            double yAvg = 0;
            double Length = Scatterdata.GetLength(1);
            for (int i = 0; i < Length; i++)
            {
                xAvg += Scatterdata[0, i];
                yAvg += Scatterdata[1, i];
            }

            xAvg = xAvg / Length;
            yAvg = yAvg / Length;

            double v1 = 0;
            double v2 = 0;

            for (int i = 0; i < Length; i++)
            {
                v1 += (Scatterdata[0, i] - xAvg) * (Scatterdata[1, i] - yAvg);
                v2 += Math.Pow(Scatterdata[0, i] - xAvg, 2);
            }

            slope = v1 / v2;
            intercept = yAvg - slope * xAvg;
        }

        private static double cubicInterpolate(double[] p, double x)
        {
            return p[1] + 0.5 * x * (p[2] - p[0] + x * (2.0 * p[0] - 5.0 * p[1] + 4.0 * p[2] - p[3] + x * (3.0 * (p[1] - p[2]) + p[3] - p[0])));
        }

        public static double[] CubicInterpolationIndex(double[,] ScatterData)
        {
            int nTotalPoints = (int)ScatterData[ScatterData.GetLength(0) - 1, 0] + 1;
            double[] OutPoints = new double[nTotalPoints];

            double[] p = new double[4];

            //handle the edges
            for (int i = 0; i < ScatterData[1, 0]; i++)
            {
                double d = i / ScatterData[1, 0] * (ScatterData[1, 1] - ScatterData[0, 1]) + ScatterData[0, 1];
                OutPoints[i] = d;
            }

            int StartI = (int)ScatterData[ScatterData.GetLength(0) - 2, 0];
            int EndI = (int)ScatterData[ScatterData.GetLength(0) - 1, 0] + 1;
            double StartV = ScatterData[ScatterData.GetLength(0) - 2, 1];
            double EndV = ScatterData[ScatterData.GetLength(0) - 1, 1];
            for (int i = (int)StartI; i < EndI; i++)
            {
                double d = (i - StartI) / (EndI - StartI) * (EndV - StartV) + StartV;
                OutPoints[i] = d;
            }

            //now cycle through the middle
            for (int i = 1; i < ScatterData.GetLength(0) - 2; i++)
            {
                p[0] = ScatterData[i - 1, 1];
                p[1] = ScatterData[i, 1];
                p[2] = ScatterData[i + 1, 1];
                p[3] = ScatterData[i + 2, 1];

                for (int j = (int)ScatterData[i, 0]; j < ScatterData[i + 2, 0]; j++)
                {
                    double d = (j - ScatterData[i, 0]) / (ScatterData[i + 1, 0] - ScatterData[i, 0]);
                    OutPoints[j] = cubicInterpolate(p, d);
                }
            }

            return OutPoints;
        }

        public static float[] MedianSmooth(this float[] array, int Order)
        {
            float[] outArray = new float[array.Length];
            int back = Order / 2;
            List<float> elements = new List<float>(Order);

            outArray[0] = array[0];
            for (int i = 1; i < back; i++)
            {
                elements.Clear();
                for (int j = 0; j < i + i; j++)
                {
                    elements.Add(array[j]);
                }
                elements.Sort();
                outArray[i] = elements[elements.Count / 2];
            }

            elements.Clear();
            for (int i = 0; i < Order; i++)
                elements.Add(0);

            int length = array.Length - back - 1;
            for (int i = back; i < length; i++)
            {
                int cc = 0;
                for (int j = i - back; j < i + back; j++)
                {
                    elements[cc] = array[j];
                    cc++;
                }
                elements.Sort();
                outArray[i] = elements[elements.Count / 2];
            }

            for (int i = length; i < array.Length - 1; i++)
            {
                elements.Clear();
                for (int j = i - back; j < array.Length; j++)
                {
                    elements.Add(array[j]);
                }
                elements.Sort();
                outArray[i] = elements[elements.Count / 2];
            }

            outArray[array.Length - 1] = array[array.Length - 1];
            return outArray;
        }
        #endregion
    }
}
