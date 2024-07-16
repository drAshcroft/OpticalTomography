using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Logger;

namespace ImageViewer3D.PythonScripting
{
    public class cTextWriter : TextWriter
    {
        
        public cTextWriter()
        {
        }
        
        public override void WriteLine(string value)
        {
            LoggerForm.LogMessage(value);
        }
        public override void Write(string value)
        {
            LoggerForm.LogMessage(value);
        }
        public override void Write(object value)
        {
            LoggerForm.LogMessage(value.ToString());
        }
        public override void WriteLine(object value)
        {
            LoggerForm.LogMessage(value.ToString());
        }
        public void Print(object value)
        {
            LoggerForm.LogMessage(value.ToString());
        }
        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
