using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace IronPythonEditor.TextEditor
{
    public class cTextWriter : TextWriter
    {
        public delegate void PythonSpeaksEvent(string Message);
        public event PythonSpeaksEvent PythonSpeaks;
        public cTextWriter()
        {
        }
        
        public override void WriteLine(string value)
        {
            if (PythonSpeaks != null)
                PythonSpeaks(value);
           // LoggerForm.LogMessage(value);
        }
        public override void Write(string value)
        {
            if (PythonSpeaks != null)
                PythonSpeaks(value);

           // LoggerForm.LogMessage(value);
        }
        public override void Write(object value)
        {
            if (PythonSpeaks != null)
                PythonSpeaks(value.ToString());

           // LoggerForm.LogMessage(value.ToString());
        }
        public override void WriteLine(object value)
        {
            if (PythonSpeaks != null)
                PythonSpeaks(value.ToString());

          //  LoggerForm.LogMessage(value.ToString());
        }
        public void Print(object value)
        {
            if (PythonSpeaks != null)
                PythonSpeaks(value.ToString());

           // LoggerForm.LogMessage(value.ToString());
        }
        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
