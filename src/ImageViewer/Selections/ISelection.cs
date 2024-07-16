using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageViewer
{
    public  interface ISelection:ICloneable 
    {
        /// <summary>
        /// Outer bounds for the selection. 
        /// </summary>
        Rectangle SelectionBounds { get; }


        Point ImageCenter { get; }

        /// <summary>
        /// Returns true if the testpoint is within the selection.  This is helpful for complicated shape selections.
        /// </summary>
        /// <param name="TestPoint"></param>
        /// <returns></returns>
        bool PointInSelection(Point TestPoint);

        /// <summary>
        /// The index of the display box on multi display box.
        /// </summary>
        int WindowIndex { get; set; }

        /// <summary>
        /// This procedure sets the upper x and y to zero 
        /// while bringing along all the control points with it.
        /// </summary>
        void BringToZero();
    }
}
