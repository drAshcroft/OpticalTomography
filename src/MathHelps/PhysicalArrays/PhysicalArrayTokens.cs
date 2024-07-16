using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathHelpLib.DrawingAndGraphing;

namespace MathHelpLib
{
    public enum GraphAxis
    {
        XAxis = 0, YAxis = 1, ZAxis = 2, ValueAxis = 3
    }

    /// <summary>
    /// Class to help correctly graph the data in the physical array
    /// </summary>
    public class PhysicalArrayTokens : ICloneable
    {
        private string[] AxisNames = new string[4];
        private string[] AxisUnits = new string[4];
        public MathGraphTypes SuggestedGraphingHint = MathGraphTypes.Unknown;
        private bool[] mFFTSpace = new bool[3];
        private bool[] mMachineReadableFFT = new bool[3];

        public void AxisName_Set(GraphAxis DesiredAxis, string NewName)
        {
            AxisNames[(int)DesiredAxis] = NewName;
        }
        public string AxisName(GraphAxis DesiredAxis)
        {
            return AxisNames[(int)DesiredAxis];
        }

        public void FFTSpace_Set(GraphAxis DesiredAxis, bool Value)
        {
            mFFTSpace[(int)DesiredAxis] = Value;
        }
        public bool FFTSpace(GraphAxis DesiredAxis)
        {
            return mFFTSpace[(int)DesiredAxis];
        }

        public void MachineReadableFFT_Set(GraphAxis DesiredAxis, bool Value)
        {
            mMachineReadableFFT[(int)DesiredAxis] = Value;
        }
        public bool MachineReadableFFT(GraphAxis DesiredAxis)
        {
            return mMachineReadableFFT[(int)DesiredAxis];
        }

        public object Clone()
        {
            PhysicalArrayTokens pat = new PhysicalArrayTokens();
            for (int i = 0; i < AxisNames.Length; i++)
            {
                pat.AxisNames[i] = AxisNames[i];
                pat.AxisUnits[i] = AxisUnits[i];
            }
            for (int i = 0; i < mFFTSpace.Length; i++)
            {
                pat.mFFTSpace[i] = mFFTSpace[i];
                pat.mMachineReadableFFT[i] = mMachineReadableFFT[i];
            }

            pat.SuggestedGraphingHint = SuggestedGraphingHint;

            return pat;
        }
    }
}
