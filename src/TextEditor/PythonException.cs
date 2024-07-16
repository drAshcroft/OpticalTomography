using System;
using System.Collections.Generic;
using System.Text;

namespace IronPythonEditor.TextEditor
{
    public class PythonException : Exception
    {

        public PythonException()
        {

        }

        public PythonException(Microsoft.Scripting.SyntaxErrorException ex)
        {
            mLineNumber = ex.Line;
            mColNumber = ex.Column;
            mMessage = ex.Message;

        }


        string mTraceBack;
        public string PythonErrorTraceBack
        {
            get { return mTraceBack; }
            set
            {
                mTraceBack = value;
                string[] Parts = mTraceBack.Split(',', '\n', '\r');
                mColNumber = 0;
                mLineNumber = 0;
                foreach (string s in Parts)
                {
                    if (s.Trim().StartsWith("line") == true)
                    {
                        string j=s.Remove(0, 5);

                        int.TryParse(j, out mLineNumber);
                    }
                }
            }
        }

        private string mMessage;
        public new  string Message
        {
            get
            {
                return mMessage;
            }
            set
            {
                mMessage = value;
            }
        }
        private int mLineNumber;
        private int mColNumber;

        public int LineNumber
        {
            get { return mLineNumber; }
            set { mLineNumber = value; }
        }
        public int ColNumber
        {
            get { return mColNumber; }
            set { mColNumber = value; }
        }

    }
}
