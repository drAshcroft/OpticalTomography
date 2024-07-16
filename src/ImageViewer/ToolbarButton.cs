using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageViewer.Tools;

namespace ImageViewer
{
    public class ToolbarButton:Button 
    {
        private aDrawingTool mMyTool;
        public aDrawingTool DrawingTool
        {
            get { return mMyTool; }
        }
        public ToolbarButton(aDrawingTool DrawingTool)
        {
            mMyTool = DrawingTool;
        }
    }
}
