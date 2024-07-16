using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace ImageViewer.Filters
{
    /// <summary>
    /// a dictionary that allows an key to be added with the same name as a previous key.
    /// The old key will be replaced
    /// </summary>
    public class ReplaceStringDictionary : Dictionary<string, object>
    {
        private object CriticalSectionLock = new object();
        public void AddSafe(string Key, object Value)
        {
            lock (CriticalSectionLock)
            {
                if (this.ContainsKey(Key) == false)
                    this.Add(Key, Value);
                else
                {
                    this.Remove(Key);
                    this.Add(Key, Value);
                }
            }
        }
    }

    /// <summary>
    /// a dictionary that allows an key to be added with the same name as a previous key.
    /// The old key will be replaced
    /// </summary>

    public class ReplaceFileStringDictionary : Dictionary<string, object>
    {
        System.IO.StreamWriter LogFile;
        public ReplaceFileStringDictionary(System.IO.StreamWriter LogFile)
        {
            this.LogFile = LogFile;
        }
        private object CriticalSectionLock = new object();
        public new void Add(string Key, object Value)
        {
            lock (CriticalSectionLock)
            {
                if (base.ContainsKey(Key) == false)
                    base.Add(Key, Value);
                else
                {
                    base.Remove(Key);
                    base.Add(Key, Value);
                }
                LogFile.WriteLine("<" + Key + "><" + Value + "/>");
            }

        }

        public void AddSafe(string Key, object Value)
        {
            lock (CriticalSectionLock)
            {
                if (this.ContainsKey(Key) == false)
                    this.Add(Key, Value);
                else
                {
                    this.Remove(Key);
                    this.Add(Key, Value);
                }
                LogFile.WriteLine("<" + Key + "><" + Value + "/>");
            }
        }
    }

    /// <summary>
    /// a dictionary that allows an key to be added with the same name as a previous key.
    /// The old key will be replaced
    /// </summary>

    public class ReplaceChatStringDictionary : Dictionary<string, object>
    {
        string DictName;
        IPEndPoint RemoteIpEndPoint;
        System.Net.Sockets.UdpClient udpClientB;
       
        public ReplaceChatStringDictionary(string DictName,int Port)
        {
            this.DictName = DictName;
            udpClientB = new System.Net.Sockets.UdpClient();
            RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port );
            // Sends a message to the host to which you have connected.
            Byte[] sendBytes = Encoding.ASCII.GetBytes(DictName + "Is anybody there?");

            // Sends a message to a different host using optional hostname and port parameters.
            udpClientB.Send(sendBytes, sendBytes.Length, RemoteIpEndPoint);
        }
        private object CriticalSectionLock = new object();
        public new void Add(string Key, object Value)
        {
            lock (CriticalSectionLock)
            {
                bool NewValue = true ;
                if (base.ContainsKey(Key) == false)
                    base.Add(Key, Value);
                else
                {
                    if (this[Key] != Value)
                    {
                        base.Remove(Key);
                        base.Add(Key, Value);
                    }
                    else
                        NewValue = false;
                }

                if (udpClientB != null && NewValue ==true )
                {
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(DictName + ":" + Key + " = " + Value.ToString());

                    // Sends a message to a different host using optional hostname and port parameters.
                    udpClientB.Send(sendBytes, sendBytes.Length, RemoteIpEndPoint);
                }
                if (NewValue)
                    Console.WriteLine(Key + " = " + Value);
            }

        }

        public void AddSafe(string Key, object Value)
        {
            lock (CriticalSectionLock)
            {
                bool NewValue = true;
                if (base.ContainsKey(Key) == false)
                    base.Add(Key, Value);
                else
                {
                    if (this[Key] != Value)
                    {
                        base.Remove(Key);
                        base.Add(Key, Value);
                    }
                    else
                        NewValue = false;
                }

                if (udpClientB != null && NewValue == true)
                {
                    try
                    {
                        Byte[] sendBytes = Encoding.ASCII.GetBytes(DictName + ":" + Key + " = " + Value.ToString());

                        // Sends a message to a different host using optional hostname and port parameters.
                        udpClientB.Send(sendBytes, sendBytes.Length, RemoteIpEndPoint);
                    }
                    catch { }
                }
                try
                {
                    if (NewValue)
                        Console.WriteLine(Key + " = " + Value);
                }
                catch { }
            }
        }

    }
    /// <summary>
    /// a dictionary that allows an key to be added with the same name as a previous key.
    /// The old key will be replaced
    /// </summary>

    public class ReplaceIntDictionary : Dictionary<int, object>
    {
        private object CriticalSectionLock = new object();
        public void AddSafe(int Key, object Value)
        {
            lock (CriticalSectionLock)
            {
                if (this.ContainsKey(Key) == false)
                    this.Add(Key, Value);
                else
                {
                    this.Remove(Key);
                    this.Add(Key, Value);
                }
            }
        }
    }
}
