using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathHelpLib.DrawingAndGraphing
{
    /// <summary>
    /// Used to select the correct graphing control.  
    /// </summary>
    public enum MathGraphTypes
    {
       Unknown, 
       Graph1D_Line,Graph1D_Scatter, Graph1D_Bar,
       Graph2D_contour, Graph2D_Image, Graph2D_ImageEditor,
       Graph3D_Viewer, Graph3D_Slices,Graph3D_Slices_MultiAngle,
       Graph3D_Slices_MultiAngleEditor
    }
}
