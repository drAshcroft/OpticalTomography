using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathHelpLib.PhysicalArrays
{
    /// <summary>
    /// Another overly complicated structure to use with the this ranges of the physical array
    /// </summary>
    public struct PhysicalRange
    {
        public PhysicalArrayRank RangeRank;
        public double StartX, EndX;
        public double StartY, EndY;
        public double StartZ, EndZ;

        public PhysicalRange(double StartX, double EndX)
        {
            this.StartX = StartX;
            this.EndX = EndX;
            this.StartY = -1;
            this.EndY = -1;
            this.StartZ = -1;
            this.EndZ = -1;
            RangeRank = PhysicalArrayRank.Array1D;
        }

        public PhysicalRange(double StartX, double EndX, double StartY, double EndY)
        {
            this.StartX = StartX;
            this.EndX = EndX;
            this.StartY = StartY;
            this.EndY = EndY;
            this.StartZ = -1;
            this.EndZ = -1;
            RangeRank = PhysicalArrayRank.Array2D;
        }

        public PhysicalRange(double StartX, double EndX, double StartY, double EndY,double StartZ,double EndZ)
        {
            this.StartX = StartX;
            this.EndX = EndX;
            this.StartY = StartY;
            this.EndY = EndY;
            this.StartZ = StartZ;
            this.EndZ = EndZ ;
            RangeRank = PhysicalArrayRank.Array3D;
        }

        public void ConvertRangeToIndex(double[] PhysicalStart, double[] PhysicalEnd, double[] PhysicalStep, out int[] StartIndex, out int[] EndIndex)
        {
            StartIndex = new int[3];
            EndIndex = new int[3];
            StartIndex[0]=(int)( (StartX-PhysicalStart[0])/PhysicalStep[0]);
            EndIndex[0] = (int)((EndX - PhysicalStart[0]) / PhysicalStep[0]);

            StartIndex[1] = (int)((StartY - PhysicalStart[1]) / PhysicalStep[1]);
            EndIndex[1] = (int)((EndY - PhysicalStart[1]) / PhysicalStep[1]);

            StartIndex[2] = (int)((StartZ- PhysicalStart[2]) / PhysicalStep[2]);
            EndIndex[2] = (int)((EndZ - PhysicalStart[2]) / PhysicalStep[2]);

        }
    }
}
