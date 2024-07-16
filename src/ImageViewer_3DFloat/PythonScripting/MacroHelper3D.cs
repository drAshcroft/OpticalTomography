using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageViewer3D.PythonScripting
{
    public class MacroHelper3D
    {
        public static bool  ErrorOnThread=false ;
        //returns true if an error has occured on another thread.  This provides a clean way to shut everything down
        public static bool DoEvents()
        {
            Application.DoEvents();
            return ErrorOnThread;
        }
    }
}
