using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;


namespace VoreenWindow
{
    public class CommonNetwork
    {
        public delegate void NetworkMessageRecievedEvent(string Message);

        public event NetworkMessageRecievedEvent NetworkMessageRecieved;

        public bool Quit = false;
        UdpClient udpClient;
        public  int StartNetworkListener(Form Host, int port)
        {
            if (udpClient == null)
            {
                //check if this port is already being used
                while (udpClient == null)
                {
                    try
                    {
                        udpClient = new UdpClient(port);
                    }
                    catch
                    {
                        port++;
                    }
                }
                Thread MonitorTreads = new Thread(delegate(object Vars)
                {
                    while (Host.IsDisposed == false && Host.Visible==true && Quit==false )
                    {
                        //IPEndPoint object will allow us to read datagrams sent from any source.
                        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);

                        // Blocks until a message returns on this socket from a remote host.
                        Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                        string returnData = Encoding.ASCII.GetString(receiveBytes);

                        // Uses the IPEndPoint object to determine which of these two hosts responded.
                        if (NetworkMessageRecieved!=null)
                            NetworkMessageRecieved(returnData.ToString());
                        Console.WriteLine(returnData.ToString());
                    }
                    udpClient.Close();
                });

                MonitorTreads.Start();
            }
            return port;
        }

        string DictName;
        IPEndPoint RemoteIpEndPoint;
        System.Net.Sockets.UdpClient udpClientB;

        public void StartNetworkWriter(string dictName, int Port)
        {
            DictName = dictName;
            udpClientB = new System.Net.Sockets.UdpClient();
            RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
            // Sends a message to the host to which you have connected.
            Byte[] sendBytes = Encoding.ASCII.GetBytes(DictName + "Is anybody there?");

            // Sends a message to a different host using optional hostname and port parameters.
            udpClientB.Send(sendBytes, sendBytes.Length, RemoteIpEndPoint);
        }

        private object CriticalSectionLock = new object();
        public void SendNetworkPacket(string Key, object Value)
        {
            lock (CriticalSectionLock)
            {
                bool NewValue = true;
                if (udpClientB != null && NewValue == true)
                {
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(DictName + "|" + Key + " = " + Value.ToString());
                    // Sends a message to a different host using optional hostname and port parameters.
                    udpClientB.Send(sendBytes, sendBytes.Length, RemoteIpEndPoint);
                }
                Console.WriteLine(Key + " = " + Value);
            }
        }

    }
}
