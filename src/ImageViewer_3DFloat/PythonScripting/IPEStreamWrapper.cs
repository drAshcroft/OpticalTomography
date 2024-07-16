using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Logger;

namespace ImageViewer3D.PythonScripting
{
    public delegate void PythonResponse(string text);

    public class IPEStreamWrapper : Stream
    {
        string Output;

        public event PythonResponse PythonTalks;

        public IPEStreamWrapper()
        {
           
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true ; }
        }

        public override long Position
        {
            get
            {
                return 0;
            }
            set
            {
                
            }
        } 
        public override void Flush()
        {
            Output = "";
        }
       
        public override void Write(byte[] buffer, int offset, int count)
        {
            Output += Encoding.UTF8.GetString(buffer , offset, count);
            if (PythonTalks != null)
            {
                PythonTalks(Output);
                Output = "";
            }
            else
            {
                LoggerForm.LogMessage(Output);
                Output = "";
            }
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
