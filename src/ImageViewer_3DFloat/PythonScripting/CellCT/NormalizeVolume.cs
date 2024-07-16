using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using ImageViewer.Filters;
using ImageViewer3D.Filters;

namespace ImageViewer3D.PythonScripting.CellCT
{
    public class NormalizeFBPVolumeEffect : aEffectNoForm3D
    {
        public override string EffectName { get { return "Normalize Volume"; } }
        public override string EffectMenu { get { return "Cell CT"; } }
        public override string EffectSubMenu { get { return "FBP"; } }
        public override int OrderSuggestion { get { return 1; } }


        public static void NormalizeFBPVolume(ref DataHolder SourceImage, int nProjections, bool ClipZeros)
        {
           // if (nProjections != 500)
            {
                float factor = nProjections / 500f;// *SourceImage.Width;
                unsafe
                {
                    fixed (float* pSource = SourceImage.Data)
                    {
                        float* pOut = pSource;
                        long length = SourceImage.Data.LongLength;
                        if (ClipZeros)
                        {
                            for (int i = 0; i < length; i++)
                            {
                                if (*pOut > 0)
                                {
                                    *pOut /= factor;
                                }
                                else
                                {
                                    *pOut = 0;
                                }
                                pOut++;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < length; i++)
                            {
                                *pOut /= factor;
                                pOut++;
                            }
                        }
                    }
                }
            }
        }

        public static void NormalizeFBPVolume(ref double[, ,] SourceImage, int nProjections, bool ClipZeros,double CutValue)
        {
           // if (nProjections != 500)
            {
                double factor = nProjections / 500f;// *SourceImage.GetLength(0);
                double ave = 0;
                double cc = 0;
                unsafe
                {
                    fixed (double* pSource = SourceImage)
                    {
                        double* pOut = pSource;
                        if (ClipZeros)
                        {
                            for (int i = 0; i < SourceImage.LongLength; i++)
                            {

                                *pOut /= factor;
                                if (*pOut < CutValue )
                                    *pOut = CutValue;

                                if (*pOut > 5)
                                {
                                    ave += *pOut;
                                    cc++;
                                }
                                pOut++;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < SourceImage.LongLength; i++)
                            {
                                *pOut /= factor;
                                if (*pOut > 0)
                                {
                                    ave += *pOut;
                                    cc++;
                                }
                                pOut++;
                            }
                        }
                    }
                }
                //return ave / cc;
            }
        }


        public static void NormalizeFBPVolume(ref float[, ,] SourceImage, int nProjections, bool ClipZeros,  float CutValue)
        {
            // if (nProjections != 500)
            {
                float factor = nProjections / 500f;// *SourceImage.GetLength(0);
                double ave = 0;
                double cc = 0;
                unsafe
                {
                    fixed (float* pSource = SourceImage)
                    {
                        float* pOut = pSource;
                        if (ClipZeros)
                        {
                            for (int i = 0; i < SourceImage.LongLength; i++)
                            {

                                *pOut =(*pOut)/ factor-CutValue;
                                if (*pOut < 0)
                                    *pOut = 0;

                               /* if (*pOut > 5)
                                {
                                    ave += *pOut;
                                    cc++;
                                }*/
                                pOut++;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < SourceImage.LongLength; i++)
                            {
                                *pOut = (*pOut) / factor - CutValue;
                                if (*pOut < 0)
                                    *pOut = 0;

                                /* if (*pOut > 5)
                                 {
                                     ave += *pOut;
                                     cc++;
                                 }*/
                                pOut++;
                            }
                        }
                    }
                }
              
            }
        }

        public static void NormalizeFBPVolume(ref double[][,] SourceImage, int nProjections, bool ClipZeros)
        {
            if (nProjections != 500)
            {
                double factor = nProjections / 500f;// *SourceImage.GetLength(0);
                double cutoff = 0;// 7 * factor;
                unsafe
                {
                    for (int j = 0; j < SourceImage.Length; j++)
                    {
                        fixed (double* pSource = SourceImage[j])
                        {
                            double* pOut = pSource;
                            long length = SourceImage[j].LongLength;
                            if (ClipZeros)
                            {
                                for (int i = 0; i < length; i++)
                                {
                                    if (*pOut > cutoff)
                                    {
                                        *pOut /= factor;
                                    }
                                    else
                                        *pOut = 0;
                                    pOut++;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < length; i++)
                                {
                                    *pOut /= factor;
                                    pOut++;
                                }
                            }

                        }
                    }
                }
            }
        }

        public override DataHolder DoEffect(DataEnvironment3D dataEnvironment, DataHolder SourceImage, ReplaceStringDictionary PassData, params object[] Parameters)
        {
            if (SourceImage != null)
            {
                NormalizeFBPVolume(ref SourceImage, (int)Parameters[0], (Parameters.Length > 1 && (bool)Parameters[1] == true));

                return SourceImage;
            }
            else if (Parameters[0].GetType() == typeof(double[, ,]))
            {
                double[, ,] Data = (double[, ,])Parameters[0];
                NormalizeFBPVolume(ref Data, (int)Parameters[1], (Parameters.Length > 2 && (bool)Parameters[2] == true),-500);
                return new DataHolder(Data);
            }
            else if (Parameters.GetType() == typeof(double[][,]) || Parameters[0].GetType() == typeof(double[][,]))
            {
                double[][,] Data;
                if (Parameters.GetType() == typeof(double[][,]))
                    Data = (double[][,])Parameters;
                else
                    Data = (double[][,])Parameters[0];
                NormalizeFBPVolume(ref Data, (int)Parameters[1], (Parameters.Length > 2 && (bool)Parameters[2] == true));
                return null;
            }
            else
                return SourceImage;
        }
        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "" }; }
        }



    }
}
