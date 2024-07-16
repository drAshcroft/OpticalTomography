using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathHelpLib.PhysicalArrays
{
    /// <summary>
    /// Used for the little too complicated this calls for the physical array.  
    /// </summary>
    public class IndexedRange
    {
        public PhysicalArrayRank RangeRank;
        public int  StartXI, EndXI;
        public int  StartYI, EndYI;
        public int  StartZI, EndZI;

        public IndexedRange(int  StartX, int  EndX)
        {
            this.StartXI = StartX;
            this.EndXI = EndX;
            this.StartYI = -1;
            this.EndYI = -1;
            this.StartZI = -1;
            this.EndZI = -1;
            RangeRank = PhysicalArrayRank.Array1D;
        }

        public IndexedRange(int  StartX, int  EndX, int  StartY, int  EndY)
        {
            this.StartXI = StartX;
            this.EndXI = EndX;
            this.StartYI = StartY;
            this.EndYI = EndY;
            this.StartZI = -1;
            this.EndZI = -1;
            RangeRank = PhysicalArrayRank.Array2D;
        }

        public IndexedRange(int  StartX, int  EndX, int  StartY, int  EndY, int  StartZ, int  EndZ)
        {
            this.StartXI = StartX;
            this.EndXI = EndX;
            this.StartYI = StartY;
            this.EndYI = EndY;
            this.StartZI = StartZ;
            this.EndZI = EndZ ;
            RangeRank = PhysicalArrayRank.Array3D;
        }

        /// <summary>
        /// converts the current indexed range into numbers for the physicalarray
        /// </summary>
        /// <param name="PhysicalStart"></param>
        /// <param name="PhysicalEnd"></param>
        /// <param name="PhysicalStep"></param>
        /// <param name="StartRange"></param>
        /// <param name="EndRange"></param>
        public void ConvertIndexToRange(double[] PhysicalStart, double[] PhysicalEnd, double[] PhysicalStep, out double[] StartRange, out double[] EndRange)
        {
            StartRange = new double [3];
            EndRange = new double [3];
            StartRange[0] = StartXI * PhysicalStep[0] + PhysicalStart[0];
            EndRange[0] = EndXI * PhysicalStep[0] + PhysicalStart[0];

            StartRange[1] = StartYI * PhysicalStep[1] + PhysicalStart[1];
            EndRange[1] = EndYI * PhysicalStep[1] + PhysicalStart[1];

            StartRange[2] = StartZI * PhysicalStep[2] + PhysicalStart[2];
            EndRange[2] = EndZI * PhysicalStep[2] + PhysicalStart[2];
        }
    }
}
