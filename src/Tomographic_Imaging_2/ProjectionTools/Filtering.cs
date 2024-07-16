using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathHelpLib;


namespace Tomographic_Imaging_2
{
    public class Filtering
    {
        private static object CriticalSectionLock = new object();
        #region Conversions
        public static double[] ConvertFilterToRealSpace(double[] Filter, double PhysicalStepSize)
        {
            double[] OutArray = MathFFTHelps.iFFTreal2real(Filter);
            OutArray.MultiplyInPlace(PhysicalStepSize / (double)Filter.Length);
            return OutArray;
        }
        public static double[] ConvertFilterToRealSpace(complex[] Filter, double PhysicalStepSize)
        {
            lock (CriticalSectionLock)
            {
                double[] OutArray = MathFFTHelps.iFFTcomplex2real(Filter);
                OutArray.MultiplyInPlace(PhysicalStepSize / (double)Filter.Length);

                return OutArray;
            }
        }
        public static double[] ConvertWindowToRadonProjectionFilter(double[] Window, double PhysicalStepSize)
        {
            double[] OutArray = new double[Window.Length];
            int CutPoint = Window.Length;
            double HalfPoint = CutPoint / 2d;
            for (int i = 0; i < CutPoint; i++)
            {
                double w = (i - HalfPoint) / PhysicalStepSize;
                OutArray[i] = Window[i] * Math.Abs(w);
            }

            return OutArray.MakeFFTMachineReadable();
        }
        public static double[] ConvertWindowToRealSpaceRadon(double[] Window, double PhysicalStepSize)
        {

            complex[] OutArray = new complex[Window.Length];
            int CutPoint = Window.Length;
            double HalfPoint = CutPoint / 2d;
            for (int i = 0; i < CutPoint; i++)
            {
                double w = (i - HalfPoint) / PhysicalStepSize;
                OutArray[i] = new complex(Window[i] * Math.Abs(w), 0);
            }
            OutArray = OutArray.MakeFFTMachineReadable();
            return ConvertFilterToRealSpace(OutArray, PhysicalStepSize).MakeFFTHumanReadable();
        }

        public static double[,] ConvertFilterToRealSpace(double[,] Filter, double PhysicalStepSize)
        {
            double[,] OutArray = MathFFTHelps.iFFTreal2real(Filter);
            OutArray.MultiplyInPlace(PhysicalStepSize / (double)Filter.Length);
            return OutArray;
        }
        public static double[,] ConvertFilterToRealSpace(complex[,] Filter, double PhysicalStepSize)
        {
            double[,] OutArray = MathFFTHelps.iFFTcomplex2complex(Filter.MakeFFTMachineReadable()).ConvertToDoubleReal();
            OutArray.MultiplyInPlace(PhysicalStepSize / (double)Filter.Length);
            return OutArray.MakeFFTHumanReadable();
        }
        public static double[,] ConvertWindowToRadonProjectionFilter(double[,] Window, double PhysicalStepSize)
        {
            double[,] OutArray = new double[Window.GetLength(0), Window.GetLength(1)];
            double nPointsX = Window.GetLength(0);
            double HalfPointX = nPointsX / 2d;
            double nPointsY = Window.GetLength(1);
            double HalfPointY = nPointsY / 2d;
            double x, y, w;
            for (int i = 0; i < nPointsX; i++)
            {
                for (int j = 0; j < nPointsY; j++)
                {
                    x = (i - HalfPointX) / PhysicalStepSize;
                    y = (j - HalfPointY) / PhysicalStepSize;
                    w = Math.Sqrt(x * x + y * y);
                    OutArray[i, j] = Window[i, j] * Math.Abs(w);
                }
            }
            return OutArray;
        }
        public static double[,] ConvertWindowToRealSpaceRadonFilter(double[,] Window, double PhysicalStepSize)
        {
            complex[,] OutArray = new complex[Window.GetLength(0), Window.GetLength(1)];
            double nPointsX = Window.GetLength(0);
            double HalfPointX = nPointsX / 2d;
            double nPointsY = Window.GetLength(1);
            double HalfPointY = nPointsY / 2d;
            double x, y, w;
            for (int i = 0; i < nPointsX; i++)
            {
                for (int j = 0; j < nPointsY; j++)
                {
                    x = (i - HalfPointX) / PhysicalStepSize;
                    y = (j - HalfPointY) / PhysicalStepSize;
                    w = Math.Sqrt(x * x + y * y);
                    OutArray[i, j] = new complex(Window[i, j] * w, 0);
                }
            }
            OutArray[(int)HalfPointX, (int)HalfPointY] = new complex(1000, 0);


            return ConvertFilterToRealSpace(OutArray, PhysicalStepSize);
        }
        #endregion

        #region 1D
        #region Window_Function



        public static double[] RectangularWindow(int nPoints, int CutPoint, double FrequencyCutoff, double PhysicalStepSize)
        {
            double[] impulse = new double[nPoints];

            int Lower = (int)Math.Round((nPoints - CutPoint) / 2d);
            int Upper = (int)Math.Round((nPoints + CutPoint) / 2d);
            for (int i = 0; i < impulse.Length; i++)
            {
                if (i > Lower && i < Upper)
                    impulse[i] = 1;
            }
            return impulse;
        }
        public static double[] HanWindow(int nPoints, int CutPoint)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Floor(nPoints / 2d);
            double Length = nPoints;
            int cc = 0;
            double x;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    x = 2d * Math.PI * i / CutPoint;
                    OutArray[cc] = .5 * (1 + Math.Cos(x));
                }
                else
                    OutArray[cc] = 0;
                cc++;
            }
            return OutArray;
        }
        public static double[] HammingWindow(int nPoints, int CutPoint)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Truncate((nPoints) / 2d);
            double Length = CutPoint;
            int cc = 0;
            double x;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    x = 2d * Math.PI * i / Length;
                    OutArray[cc] = .54 + .46 * Math.Cos(x);
                }
                else
                    OutArray[cc] = 0;

                cc++;
            }
            return OutArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CutPoint"></param>
        /// <param name="alpha">A number somewhere between 0-1</param>
        /// <param name="PhysicalStepSize"></param>
        /// <returns></returns>
        public static double[] TukeyWindow(int nPoints, int CutPoint, double alpha)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Floor((nPoints / 2d));
            double Length = CutPoint;
            int cc = 0;
            double x;

            double ChangeFrequency = alpha * Length / 2d;
            double ChangeFrequency2 = Length * (1 - alpha / 2d);
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    if (cc < ChangeFrequency)
                    {
                        x = Math.PI * (2d * i / (alpha * Length) - 1);
                        OutArray[cc] = .5 * (1 + Math.Cos(x));
                    }
                    else if (cc > ChangeFrequency2)
                    {
                        x = Math.PI * (2d * i / (alpha * Length) - 2d / alpha + 1d);
                        OutArray[cc] = .5 * (1 + Math.Cos(x));
                    }
                    else
                        OutArray[cc] = 1;
                }
                else
                    OutArray[cc] = 0;
                cc++;
            }
            return OutArray;
        }

        public static double[] CosineWindow(int nPoints, int CutPoint)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Floor(nPoints / 2d);
            double Length = CutPoint;
            int cc = 0;
            double x;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    x = Math.PI * (i / Length);
                    OutArray[cc] = Math.Cos(x);
                }
                else
                    OutArray[cc] = 0;
                cc++;
            }
            return OutArray;
        }

        public static double[] LanczosWindow(int nPoints, int CutPoint)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Floor(nPoints / 2d);
            double Length = CutPoint;
            int cc = 0;
            double x;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    x = 2d * i / Length;
                    x = x * Math.PI;
                    if (x != 0)
                        OutArray[cc] = Math.Sin(x) / x;
                    else
                        OutArray[cc] = 1;
                }
                else
                    OutArray[cc] = 0;
                cc++;
            }
            return OutArray;
        }
        public static double[] SincWindow(int nPoints, int CutPoint)
        {
            return LanczosWindow(nPoints, CutPoint);
        }
        public static double[] TriangularWindow(int nPoints, int CutPoint)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Floor(nPoints / 2d);
            double Length = CutPoint;
            int cc = 0;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    OutArray[cc] = 2d / Length * (Length / 2d - Math.Abs(cc - Length / 2d));
                }
                else
                    OutArray[cc] = 0;
                cc++;
            }
            return OutArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CutPoint"></param>
        /// <param name="sigma">Should be a value less than .5</param>
        /// <returns></returns>
        public static double[] GaussianWindow(int nPoints, int CutPoint, double sigma)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Floor(nPoints / 2d);
            double Length = CutPoint;
            int cc = 0;
            double x;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    x = (i) / (sigma * HalfPoint);
                    x = -.5 * x * x;
                    OutArray[cc] = Math.Exp(x);
                }
                else
                    OutArray[cc] = 0;
                cc++;
            }
            return OutArray;
        }

        public static double[] BartlettWindow(int nPoints, int CutPoint)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Floor(nPoints / 2d);
            for (int i = 0; i < CutPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    OutArray[i] = 2d / ((double)CutPoint) * (((double)CutPoint) / 2d - Math.Abs(i - ((double)CutPoint - 1) / 2d));
                }
                else
                    OutArray[i] = 0;
            }
            return OutArray;
        }

        public static double[] BartlettHannWindow(int nPoints, int CutPoint)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Floor(nPoints / 2d);
            double Length = CutPoint;
            int cc = 0;
            double x;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    x = 2d * Math.PI * i / Length;
                    OutArray[cc] = .38 * Math.Cos(x);
                    OutArray[cc] += .62 - .48 * Math.Abs(i / Length);
                }
                else
                    OutArray[cc] = 0;
                cc++;
            }
            return OutArray;
        }

        public static double[] BlackmanWindow(int nPoints, int CutPoint)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Floor(nPoints / 2d);
            double Length = CutPoint;
            int cc = 0;
            double x;
            double alpha = .16;
            double a0 = (1 - alpha) / 2d;
            double a1 = .5;
            double a2 = alpha / 2;

            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    x = 2 * Math.PI * cc / Length;
                    x = (a0 - a1 * Math.Cos(x) + a2 * Math.Cos(2 * x));
                    OutArray[cc] = x;
                }
                else
                    OutArray[cc] = 0;
                cc++;
            }
            return OutArray;
        }

        public static double[] NuttallWindow(int nPoints, int CutPoint)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Floor(nPoints / 2d);
            double Length = CutPoint;
            int cc = 0;
            double x;
            double a0 = .355768;
            double a1 = .487396;
            double a2 = .144232;
            double a3 = .012604;

            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    x = Math.PI * cc / Length;
                    x = (a0 - a1 * Math.Cos(2 * x) + a2 * Math.Cos(4 * x) - a3 * Math.Cos(6 * x));
                    OutArray[cc] = x;
                }
                else
                    OutArray[cc] = 0;
                cc++;
            }
            return OutArray;
        }
        public static double[] BlackMan_HarrisWindow(int nPoints, int CutPoint)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Floor(nPoints / 2d);
            double Length = CutPoint;
            int cc = 0;
            double x;
            double a0 = .35875;
            double a1 = .48829;
            double a2 = .14128;
            double a3 = .01168;

            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    x = Math.PI * cc / Length;
                    x = (a0 - a1 * Math.Cos(2 * x) + a2 * Math.Cos(4 * x) - a3 * Math.Cos(6 * x));
                    OutArray[cc] = x;
                }
                else
                    OutArray[cc] = 0;
                cc++;
            }
            return OutArray;

        }
        public static double[] BlackMan_NuttallWindow(int nPoints, int CutPoint)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Floor(nPoints / 2d);
            double Length = CutPoint;
            int cc = 0;
            double x;
            double a0 = .3636819;
            double a1 = .4891775;
            double a2 = .1365995;
            double a3 = .0106411;

            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    x = Math.PI * cc / Length;
                    x = (a0 - a1 * Math.Cos(2 * x) + a2 * Math.Cos(4 * x) - a3 * Math.Cos(6 * x));
                    OutArray[cc] = x;
                }
                else
                    OutArray[cc] = 0;
                cc++;
            }
            return OutArray;
        }
        public static double[] FlatTopWindow(int nPoints, int CutPoint)
        {
            double[] OutArray = new double[nPoints];
            double HalfPoint = Math.Floor(nPoints / 2d);
            double Length = CutPoint;
            int cc = 0;
            double x;
            double a0 = 1;
            double a1 = 1.93;
            double a2 = 1.29;
            double a3 = .388;
            double a4 = .032;

            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                if (Math.Abs(i) < CutPoint)
                {
                    x = Math.PI * cc / Length;
                    x = (a0 - a1 * Math.Cos(2 * x) + a2 * Math.Cos(4 * x) - a3 * Math.Cos(6 * x) + a4 * Math.Cos(8 * x));
                    OutArray[cc] = x;
                }
                else
                    OutArray[cc] = 0;
                cc++;
            }
            return OutArray;

        }

        #endregion

        #region Projection_Filters
        public static double[] RectangularFSProjectionFilter(int nPoints, int CutPoint, double FrequencyCutoff, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(RectangularWindow(nPoints, CutPoint, FrequencyCutoff, PhysicalStepSize), PhysicalStepSize);
        }
        public static double[] HanFSProjectionFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(HanWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] HammingFSProjectionFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(HammingWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] TukeyFSProjectionFilter(int nPoints, int CutPoint, double alpha, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(TukeyWindow(nPoints, CutPoint, alpha), PhysicalStepSize);
        }
        public static double[] CosineFSProjectionFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(CosineWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] LanczosFSProjectionFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(LanczosWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] SincFSProjectionFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return LanczosFSProjectionFilter(nPoints, CutPoint, PhysicalStepSize);
        }
        public static double[] TriangularFSProjectionFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(TriangularWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] BartlettFSProjectionFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(BartlettWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] GaussianFSProjectionFilter(int nPoints, int CutPoint, double sigma, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(GaussianWindow(nPoints, CutPoint, sigma), PhysicalStepSize);
        }
        public static double[] BartlettHannFSProjectionFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(BartlettHannWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] BlackmanFSProjectionFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(BlackmanWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] NuttallFSProjectionFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(NuttallWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] BlackMan_HarrisFSProjectionFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(BlackMan_HarrisWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] BlackMan_NuttallFSProjectionFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(BlackMan_NuttallWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] FlatTopFSProjectionFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(FlatTopWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        #endregion


        #region RealSpace_Filters
        public enum FilterTypes
        {
            RamLak,SheppLogan,Rectangular,Han,Hamming,Tukey,Cosine,Lanczos,Sinc,Triangular,Barlett,Gaussian,BartlettHann,Blackman,Nuttall,BlackmanHarris,BlackmanNuttall,FlatTop
        }

        public static double[] GetRealSpaceFilter(FilterTypes FilterType, int PaddedSize, int CutPoint, double PhysicalStep)
        {
            switch (FilterType)
            {
                case FilterTypes.Barlett:
                    return Bartlett_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.BartlettHann :
                    return BartlettHann_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Blackman :
                    return Blackman_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.BlackmanHarris :
                    return BlackMan_Harris_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.BlackmanNuttall :
                    return BlackMan_Nuttall_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Cosine :
                    return Cosine_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.FlatTop :
                    return FlatTop_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Gaussian :
                    return Gaussian_RS_RadonFilter(PaddedSize, CutPoint,CutPoint/2d, PhysicalStep);
                case FilterTypes.Hamming :
                    return Hamming_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Han :
                    return Han_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Lanczos :
                    return Lanczos_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Nuttall :
                    return Nuttall_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.RamLak :
                    return Ramachandran_Lakshminarayanan_RS_RadonFilter(PaddedSize, PhysicalStep);
                case FilterTypes.Rectangular :
                    return Rectangular_RS_RadonFilter(PaddedSize, CutPoint, CutPoint, PhysicalStep);
                case FilterTypes.SheppLogan :
                    return Shepp_Logan_RS_RadonFilter(PaddedSize, PhysicalStep);
                case FilterTypes.Sinc :
                    return Sinc_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Triangular :
                    return Triangular_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Tukey :
                    return Tukey_RS_RadonFilter(PaddedSize, CutPoint,0.5, PhysicalStep);
            }
            return Ramachandran_Lakshminarayanan_RS_RadonFilter(PaddedSize, PhysicalStep);
        }
        public static double[] GetRealSpaceFilter(string FilterType, int PaddedSize, int CutPoint, double PhysicalStep)
        {
            Filtering.FilterTypes ft  = (Filtering.FilterTypes)Enum.Parse(typeof(Filtering.FilterTypes), FilterType );
            switch (ft )
            {
                case FilterTypes.Barlett:
                    return Bartlett_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.BartlettHann:
                    return BartlettHann_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Blackman:
                    return Blackman_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.BlackmanHarris:
                    return BlackMan_Harris_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.BlackmanNuttall:
                    return BlackMan_Nuttall_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Cosine:
                    return Cosine_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.FlatTop:
                    return FlatTop_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Gaussian:
                    return Gaussian_RS_RadonFilter(PaddedSize, CutPoint, CutPoint / 2d, PhysicalStep);
                case FilterTypes.Hamming:
                    return Hamming_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Han:
                    return Han_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Lanczos:
                    return Lanczos_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Nuttall:
                    return Nuttall_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.RamLak:
                    return Ramachandran_Lakshminarayanan_RS_RadonFilter(PaddedSize, PhysicalStep);
                case FilterTypes.Rectangular:
                    return Rectangular_RS_RadonFilter(PaddedSize, CutPoint, CutPoint, PhysicalStep);
                case FilterTypes.SheppLogan:
                    return Shepp_Logan_RS_RadonFilter(PaddedSize, PhysicalStep);
                case FilterTypes.Sinc:
                    return Sinc_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Triangular:
                    return Triangular_RS_RadonFilter(PaddedSize, CutPoint, PhysicalStep);
                case FilterTypes.Tukey:
                    return Tukey_RS_RadonFilter(PaddedSize, CutPoint, 0.5, PhysicalStep);
            }
            return Ramachandran_Lakshminarayanan_RS_RadonFilter(PaddedSize, PhysicalStep);
        }


        public static double[] Ramachandran_Lakshminarayanan_RS_RadonFilter(int nPoints, double PhysicalStep)
        {
            double[] impulse = new double[nPoints];
            double tau = PhysicalStep;
            double halfI = impulse.Length / 2;
            double tauP = Math.PI * tau * Math.PI * tau;
            double offsetP;
            for (int i = 0; i < impulse.Length; i++)
            {
                if (i == halfI)
                    impulse[i] = .25 / tau / tau;
                else if ((i % 2) == 0)
                    impulse[i] = 0;
                else
                {
                    offsetP = i - halfI;
                    impulse[i] = -1 / (offsetP * offsetP * tauP);
                }
            }
            return impulse;
        }
        public static double[] Shepp_Logan_RS_RadonFilter(int nPoints, double PhysicalStep)
        {
            double[] impulse = new double[nPoints];
            double tau = PhysicalStep;
            double halfI = impulse.Length / 2;
            double C = 2 / Math.PI / Math.PI * tau;
            double offsetP;
            for (int i = 0; i < impulse.Length; i++)
            {
                if (i == halfI)
                    impulse[i] = C;
                else
                {
                    offsetP = i - halfI;
                    impulse[i] = -1 * C / (4 * offsetP - 1);
                }
            }
            return impulse;
        }
        public static double[] Rectangular_RS_RadonFilter(int nPoints, int CutPoint, double FrequencyCutoff, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(RectangularWindow(nPoints, CutPoint, FrequencyCutoff, PhysicalStepSize), PhysicalStepSize);
        }
        public static double[] Han_RS_RadonFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(HanWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] Hamming_RS_RadonFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(HammingWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] Tukey_RS_RadonFilter(int nPoints, int CutPoint, double alpha, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(TukeyWindow(nPoints, CutPoint, alpha), PhysicalStepSize);
        }
        public static double[] Cosine_RS_RadonFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(CosineWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] Lanczos_RS_RadonFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(LanczosWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] Sinc_RS_RadonFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return Lanczos_RS_RadonFilter(nPoints, CutPoint, PhysicalStepSize);
        }
        public static double[] Sinc_RS_RadonFilter(int nPoints, int CutPoint, int LowPassPoints, double PhysicalStepSize)
        {
            double[] Window = LanczosWindow(nPoints, LowPassPoints);
            Window = Window.ZeroPadArray(CutPoint);
            return ConvertWindowToRealSpaceRadon(Window, PhysicalStepSize);

        }
        public static double[] Triangular_RS_RadonFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(TriangularWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] Bartlett_RS_RadonFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(BartlettWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] Gaussian_RS_RadonFilter(int nPoints, int CutPoint, double sigma, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(GaussianWindow(nPoints, CutPoint, sigma), PhysicalStepSize);
        }
        public static double[] BartlettHann_RS_RadonFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(BartlettHannWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] Blackman_RS_RadonFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(BlackmanWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] Nuttall_RS_RadonFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(NuttallWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] BlackMan_Harris_RS_RadonFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(BlackMan_HarrisWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] BlackMan_Nuttall_RS_RadonFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(BlackMan_NuttallWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        public static double[] FlatTop_RS_RadonFilter(int nPoints, int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadon(FlatTopWindow(nPoints, CutPoint), PhysicalStepSize);
        }
        #endregion
        #endregion

        #region 2D

        public static double[] GuassianApproximation_SeperableRealSpaceFilter(int nPoints, int CutPoint, double Sigma)
        {
            double[] impulse = new double[CutPoint];

            double halfI = (double)CutPoint / 2d;
            double tauP = -2d * Math.PI * Math.PI * Sigma * Sigma;
            double a = Math.Sqrt(2 * Math.PI * Sigma * Sigma);
            double x;
            for (int i = 0; i < CutPoint; i++)
            {
                x = (i - halfI);
                impulse[i] = a * Math.Exp(tauP * x * x);
            }
            return impulse;
        }
        /* public static double[,] Ramachandran_Lakshminarayanan_RealSpacefilterRound2D(int CutPoint, double PhysicalStep)
         {
             double[,] impulse = new double[CutPoint, CutPoint];
             double tau = PhysicalStep;
             double halfI = CutPoint  / 2;
             double tauP = Math.PI * tau * Math.PI * tau;
             double offsetP;
             for (int CurrentParticle = 0; CurrentParticle < impulse.GetLength(0); CurrentParticle++)
             {
                 for (int j = 0; j < impulse.GetLength(1); j++)
                 {
                     double w = Math.Sqrt((CurrentParticle - halfI) * (CurrentParticle - halfI) + (j - halfI) * (j - halfI))+halfI ;
                     if (w == halfI)
                         impulse[CurrentParticle, j] = .25 / tau / tau;
                     else if ((w % 2) == 0)
                         impulse[CurrentParticle, j] = 0;
                     else if ((w % 2) == 1)
                     {
                         offsetP = w - halfI;
                         impulse[CurrentParticle, j] = -1 / (offsetP * offsetP * tauP);
                     }
                     else
                     {
                         offsetP = Math.Abs(w - Math.Truncate(w));
                         impulse[CurrentParticle, j] = -1 / (offsetP * offsetP * tauP) * offsetP;
                     }
                 }
             }
             return impulse;
         }


         public static double[,] Ramachandran_Lakshminarayanan_RealSpacefilter2D(int CutPoint, double PhysicalStep)
         {
             double[,] impulse = new double[CutPoint, CutPoint];
             double tau = PhysicalStep;
             double halfI = CutPoint  / 2;
             double halfJ = CutPoint / 2;
             double tauP = Math.PI * tau * Math.PI * tau;
             double offsetP;
             for (int CurrentParticle = 0; CurrentParticle < CutPoint; CurrentParticle++)
             {
                 for (int j = 0; j < CutPoint; j++)
                 {
                    if (CurrentParticle == halfI)
                         impulse[CurrentParticle, j] = .25 / tau / tau;
                     else if ((CurrentParticle % 2) == 0)
                         impulse[CurrentParticle, j] = 0;
                     else
                     {
                         offsetP =  CurrentParticle - halfI;
                         impulse[CurrentParticle, j] = -1 / (offsetP * offsetP * tauP);
                     }

                    // impulse[Xd, Yi] = 1;
                    if (j == halfJ)
                         impulse[CurrentParticle, j] *= .25 / tau / tau;
                     else if ((j % 2) == 0)
                         impulse[CurrentParticle, j] *= 0;
                     else
                     {
                         offsetP = j - halfI;
                         impulse[CurrentParticle, j] *= -1 / (offsetP * offsetP * tauP);
                     }
                 }
             }
             return impulse;
         }

         public static double[,] Shepp_Logan_RealSpacefilter2D(int CutPoint, double PhysicalStep)
         {
             double[,] impulse = new double[CutPoint, CutPoint];
             double tau = PhysicalStep;
             double halfI = CutPoint  / 2;
             double C = 2 / Math.PI / Math.PI * tau;
             double offsetP;
             for (int CurrentParticle = 0; CurrentParticle < CutPoint; CurrentParticle++)
             {
                 for (int j = 0; j < CutPoint; j++)
                 {
                     if (CurrentParticle == halfI)
                         impulse[CurrentParticle, j] = C;
                     else
                     {
                         offsetP = CurrentParticle - halfI;
                         impulse[CurrentParticle, j] = -1 * C / (4 * offsetP - 1);
                     }
                     if (j == halfI)
                         impulse[CurrentParticle, j] = C;
                     else
                     {
                         offsetP = j - halfI;
                         impulse[CurrentParticle, j] *= -1 * C / (4 * offsetP - 1);
                     }
                 }
             }
             return impulse;
         }
         */
        #region Window_Function
        public static double[,] RectangularWindow2D(int CutPoint, double FrequencyCutoff, double PhysicalStepSize)
        {
            double[,] OutArray = new double[CutPoint, CutPoint];
            double Cutoff = FrequencyCutoff / PhysicalStepSize;
            int HalfPoint = (int)(CutPoint / 2d);
            for (int i = 0; i < CutPoint; i++)
            {
                for (int j = 0; j < CutPoint; j++)
                {
                    double x = i - HalfPoint;
                    double y = i - HalfPoint;
                    double w = Math.Sqrt(x * x + y * y);
                    if (Math.Abs(w) < Cutoff)
                        OutArray[i, j] = 1;
                    else
                        OutArray[i, j] = 0;
                }
            }
            return OutArray;
        }
        public static double[,] HanWindow2D(int CutPoint)
        {
            double[,] OutArray = new double[CutPoint, CutPoint];
            double HalfPoint = (CutPoint / 2d);
            double Length = CutPoint;

            double w;
            double x;
            int ccx = 0;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                int cc = 0;
                for (double j = -HalfPoint; j < HalfPoint; j++)
                {
                    w = Math.Sqrt(i * i + j * j);
                    if ((w) > HalfPoint)
                        OutArray[ccx, cc] = 0;
                    else
                    {

                        x = 2d * Math.PI * w / Length;
                        OutArray[ccx, cc] = .5 * (1 + Math.Cos(x));
                    }
                    cc++;
                }
                ccx++;
            }
            return OutArray;
        }
        public static double[,] HammingWindow2D(int CutPoint)
        {
            double[,] OutArray = new double[CutPoint, CutPoint];
            double HalfPoint = (CutPoint / 2d);
            double Length = CutPoint;
            double w; double x;
            int ccx = 0;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                int cc = 0;
                for (double j = -HalfPoint; j < HalfPoint; j++)
                {
                    w = Math.Sqrt(i * i + j * j);
                    if ((w) > HalfPoint)
                        OutArray[ccx, cc] = 0;
                    else
                    {
                        x = 2d * Math.PI * w / Length;
                        OutArray[ccx, cc] = .54 + .46 * Math.Cos(x);
                    }
                    cc++;
                }
                ccx++;
            }
            return OutArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CutPoint"></param>
        /// <param name="alpha">A number somewhere between 0-1</param>
        /// <param name="PhysicalStepSize"></param>
        /// <returns></returns>
        public static double[,] TukeyWindow2D(int CutPoint, double alpha)
        {
            double[,] OutArray = new double[CutPoint, CutPoint];
            double HalfPoint = (CutPoint / 2d);
            double Length = CutPoint;

            double ChangeFrequency = alpha * Length / 2d;
            double ChangeFrequency2 = Length * (1 - alpha / 2d);

            double w; double x;
            int ccx = 0;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                int cc = 0;
                for (double j = -HalfPoint; j < HalfPoint; j++)
                {
                    w = Math.Sqrt(i * i + j * j);
                    if ((w) > HalfPoint)
                        OutArray[ccx, cc] = 0;
                    else if ((w + HalfPoint) < ChangeFrequency)
                    {
                        x = Math.PI * (2d * w / (alpha * Length) - 1);
                        OutArray[ccx, cc] = .5 * (1 + Math.Cos(x));
                    }
                    else if ((w + HalfPoint) > ChangeFrequency2)
                    {
                        x = Math.PI * (2d * w / (alpha * Length) - 2d / alpha + 1d);
                        OutArray[ccx, cc] = .5 * (1 + Math.Cos(x));
                    }
                    else
                        OutArray[ccx, cc] = 1;
                    cc++;
                }
                ccx++;
            }
            return OutArray;
        }

        public static double[,] CosineWindow2D(int CutPoint)
        {
            double[,] OutArray = new double[CutPoint, CutPoint];
            double HalfPoint = (CutPoint / 2d);
            double Length = CutPoint;
            double w; double x;
            int ccx = 0;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                int cc = 0;
                for (double j = -HalfPoint; j < HalfPoint; j++)
                {
                    w = Math.Sqrt(i * i + j * j);
                    if ((w) > HalfPoint)
                        OutArray[ccx, cc] = 0;
                    else
                    {
                        x = Math.PI * (w / Length);
                        OutArray[ccx, cc] = Math.Cos(x);
                    }
                    cc++;
                }
                ccx++;
            }
            return OutArray;
        }

        public static double[,] LanczosWindow2D(int CutPoint)
        {
            double[,] OutArray = new double[CutPoint, CutPoint];
            double HalfPoint = (CutPoint / 2d);
            double Length = CutPoint;
            double w; double x;
            int ccx = 0;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                int cc = 0;
                for (double j = -HalfPoint; j < HalfPoint; j++)
                {
                    w = Math.Sqrt(i * i + j * j);
                    if ((w) > HalfPoint)
                        OutArray[ccx, cc] = 0;
                    else
                    {
                        x = 2d * w / Length;
                        x = x * Math.PI;
                        if (x != 0)
                            OutArray[ccx, cc] = Math.Sin(x) / x;
                        else
                            OutArray[ccx, cc] = 1;
                    }
                    cc++;
                }
                ccx++;
            }
            return OutArray;
        }
        public static double[,] SincWindow2D(int CutPoint)
        {
            return LanczosWindow2D(CutPoint);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CutPoint"></param>
        /// <param name="sigma">Should be a value less than .5</param>
        /// <returns></returns>
        public static double[,] GaussianWindow2D(int CutPoint, double sigma)
        {
            double[,] OutArray = new double[CutPoint, CutPoint];
            double HalfPoint = (CutPoint / 2d);
            double Length = CutPoint;
            double w; double x;
            int ccx = 0;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                int cc = 0;
                for (double j = -HalfPoint; j < HalfPoint; j++)
                {
                    w = Math.Sqrt(i * i + j * j);
                    if ((w) > HalfPoint)
                        OutArray[ccx, cc] = 0;
                    else
                    {
                        x = (w) / (sigma * HalfPoint);
                        x = -.5 * x * x;
                        OutArray[ccx, cc] = Math.Exp(x);
                    }
                    cc++;
                }
                ccx++;
            }
            return OutArray;
        }

        public static double[,] TriangularWindow2D(int CutPoint)
        {
            double[,] OutArray = new double[CutPoint, CutPoint];
            double HalfPoint = (CutPoint / 2d);
            double Length = CutPoint;
            double w;
            int ccx = 0;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                int cc = 0;
                for (double j = -HalfPoint; j < HalfPoint; j++)
                {
                    w = Math.Sqrt(i * i + j * j);
                    if ((w) > HalfPoint)
                        OutArray[ccx, cc] = 0;
                    else
                    {
                        w += HalfPoint;
                        OutArray[ccx, cc] = 2d / Length * (Length / 2d - Math.Abs(w - Length / 2d));
                    }
                    cc++;
                }
                ccx++;
            }
            return OutArray;
        }
        public static double[,] BartlettHannWindow2D(int CutPoint)
        {
            double[,] OutArray = new double[CutPoint, CutPoint];
            double HalfPoint = (CutPoint / 2d);
            double Length = CutPoint;
            double w; double x;
            int ccx = 0;
            for (double i = -HalfPoint; i < HalfPoint; i++)
            {
                int cc = 0;
                for (double j = -HalfPoint; j < HalfPoint; j++)
                {
                    w = Math.Sqrt(i * i + j * j);
                    if ((w) > HalfPoint)
                        OutArray[ccx, cc] = 0;
                    else
                    {
                        x = 2d * Math.PI * w / Length;
                        OutArray[ccx, cc] = .38 * Math.Cos(x);
                        OutArray[ccx, cc] += .62 - .48 * Math.Abs(w / Length);
                    }
                    cc++;
                }
                ccx++;
            }
            return OutArray;
        }

        public static double[,] BartlettWindow2D(int CutPoint)
        {
            double[,] OutArray = new double[CutPoint, CutPoint];
            double HalfPoint = (CutPoint / 2d);
            double w;
            for (int i = 0; i < CutPoint; i++)
            {
                for (int j = 0; j < CutPoint; j++)
                {
                    w = Math.Sqrt((i - HalfPoint) * (i - HalfPoint) + (j - HalfPoint) * (j - HalfPoint));
                    if ((w) > HalfPoint)
                        OutArray[i, j] = 0;
                    else
                    {
                        w += HalfPoint;
                        OutArray[i, j] = 2d / ((double)CutPoint) * (((double)CutPoint) / 2d - Math.Abs(w - ((double)CutPoint - 1) / 2d));
                    }
                }
            }
            return OutArray;
        }

        public static double[,] BlackmanWindow2D(int CutPoint)
        {
            double[,] OutArray = new double[CutPoint, CutPoint];
            double HalfPoint = (CutPoint / 2d);
            double Length = CutPoint;

            double alpha = .16;
            double a0 = (1 - alpha) / 2d;
            double a1 = .5;
            double a2 = alpha / 2;

            double w; double x;
            for (int i = 0; i < CutPoint; i++)
            {
                for (int j = 0; j < CutPoint; j++)
                {
                    w = Math.Sqrt((i - HalfPoint) * (i - HalfPoint) + (j - HalfPoint) * (j - HalfPoint));
                    if ((w) > HalfPoint)
                        OutArray[i, j] = 0;
                    else
                    {
                        w += HalfPoint;
                        x = 2 * Math.PI * w / Length;
                        x = (a0 - a1 * Math.Cos(x) + a2 * Math.Cos(2 * x));
                        OutArray[i, j] = x;
                    }
                }
            }
            return OutArray;
        }
        #endregion

        #region Projection_Filters
        public static double[,] RectangularProjectionFilter2D(int CutPoint, double FrequencyCutoff, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(RectangularWindow2D(CutPoint, FrequencyCutoff, PhysicalStepSize), PhysicalStepSize);
        }
        public static double[,] HanProjectionFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(HanWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] HammingProjectionFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(HammingWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] TukeyProjectionFilter2D(int CutPoint, double alpha, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(TukeyWindow2D(CutPoint, alpha), PhysicalStepSize);
        }
        public static double[,] CosineProjectionFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(CosineWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] LanczosProjectionFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(LanczosWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] SincProjectionFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return LanczosProjectionFilter2D(CutPoint, PhysicalStepSize);
        }
        public static double[,] TriangularProjectionFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(TriangularWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] BartlettProjectionFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(BartlettWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] GaussianProjectionFilter2D(int CutPoint, double sigma, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(GaussianWindow2D(CutPoint, sigma), PhysicalStepSize);
        }
        public static double[,] BartlettHannProjectionFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(BartlettHannWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] BlackmanProjectionFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRadonProjectionFilter(BlackmanWindow2D(CutPoint), PhysicalStepSize);
        }
        #endregion

        #region RealSpace_Filters
        public static double[,] RectangularRealSpaceFilter2D(int CutPoint, double FrequencyCutoff, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadonFilter(RectangularWindow2D(CutPoint, FrequencyCutoff, PhysicalStepSize), PhysicalStepSize);
        }
        public static double[,] HanRealSpaceFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadonFilter(HanWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] HammingRealSpaceFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadonFilter(HammingWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] TukeyRealSpaceFilter2D(int CutPoint, double alpha, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadonFilter(TukeyWindow2D(CutPoint, alpha), PhysicalStepSize);
        }
        public static double[,] CosineRealSpaceFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadonFilter(CosineWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] LanczosRealSpaceFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadonFilter(LanczosWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] SincRealSpaceFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return LanczosRealSpaceFilter2D(CutPoint, PhysicalStepSize);
        }
        public static double[,] TriangularRealSpaceFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadonFilter(TriangularWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] BartlettRealSpaceFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadonFilter(BartlettWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] GaussianRealSpaceFilter2D(int CutPoint, double sigma, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadonFilter(GaussianWindow2D(CutPoint, sigma), PhysicalStepSize);
        }
        public static double[,] BartlettHannRealSpaceFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadonFilter(BartlettHannWindow2D(CutPoint), PhysicalStepSize);
        }
        public static double[,] BlackmanRealSpaceFilter2D(int CutPoint, double PhysicalStepSize)
        {
            return ConvertWindowToRealSpaceRadonFilter(BlackmanWindow2D(CutPoint), PhysicalStepSize);
        }
        #endregion
        #endregion
    }
}
