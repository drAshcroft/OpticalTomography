using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphingLib.DrawingAndGraphing;

namespace MathHelpLib.DrawingAndGraphing
{
    public interface IGraphControl
    {
        //Ways for getting data into the different controls,  just throw an error if your graph does not handle the particular type.
        //Other datatypes can be custom added into each datacontrol, this is just for the generalized interface
        void SetData(PhysicalArray PhysArray);
        void SetData(double[,] ValueArray);
        void SetData(double[,,] ValueArray);
        void SetData(List<double[,]> ValueArray);
        void SetData(PhysicalArray[] PhysArrays);

        /// <summary>
        /// Appears to be useless
        /// </summary>
        bool AdjustableImage
        {
            get;
        }

        /// <summary>
        /// Determines if the graphing usercontrol is visible
        /// </summary>
        bool Visible
        {
            get;
            set;
        }

        /// <summary>
        /// Width of graphing usercontrol
        /// </summary>
        int Width
        {
            get;
            set;
        }

        /// <summary>
        /// Height of graphing usercontrol
        /// </summary>
        int Height
        {
            get;
            set;
        }

        /// <summary>
        /// Used for requesting the parent control or form switch to a different graph type.  leave null if this is not possible
        /// </summary>
        /// <param name="Parent"></param>
        void SetParentControl(MathGraph Parent);
    }
}
