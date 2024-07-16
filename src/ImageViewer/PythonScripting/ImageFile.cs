using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageViewer.PythonScripting
{
    public class ImageFile
    {
        public int Index;
        public string Filename;
        public ImageFile(int Index, string Filename)
        {
            this.Index = Index;
            this.Filename = Filename;
        }
    }
}
