using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageViewer.Tools;
using ImageViewer3D.Tools;

namespace ImageViewer3D
{
    public class ToolbarButton3D:Button 
    {
        private aDrawingTool3D mMyTool;
        public aDrawingTool3D DrawingTool
        {
            get { return mMyTool; }
        }
        public ToolbarButton3D(aDrawingTool3D DrawingTool)
        {
            mMyTool = DrawingTool;
        }
    }
}
