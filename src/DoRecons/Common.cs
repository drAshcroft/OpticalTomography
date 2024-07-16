using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Tomographic_Imaging_2;

namespace DoRecons
{
    public class Common
    {
        static  UdpClient udpClient;
        public static int StartNetworkListener(ReconForm mainForm, int port)
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

                    while (mainForm.IsDisposed == false)
                    {
                        try
                        {
                            //IPEndPoint object will allow us to read datagrams sent from any source.
                            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);

                            // Blocks until a message returns on this socket from a remote host.
                            Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                            string returnData = Encoding.ASCII.GetString(receiveBytes);

                            // Uses the IPEndPoint object to determine which of these two hosts responded.
                            mainForm.NetworkMessageRecieved(returnData.ToString());
                            try
                            {
                                Console.WriteLine(returnData.ToString());
                            }
                            catch { }
                        }
                        catch { }
                    }
                    udpClient.Close();
                });

                MonitorTreads.Start();
            }
            return port;
        }

        static string DictName;
        static IPEndPoint RemoteIpEndPoint;
        static System.Net.Sockets.UdpClient udpClientB;

        public static void StartNetworkWriter(string dictName, int Port)
        {
            try
            {
                DictName = dictName;
                udpClientB = new System.Net.Sockets.UdpClient();
                RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
                // Sends a message to the host to which you have connected.
                Byte[] sendBytes = Encoding.ASCII.GetBytes(DictName + "Is anybody there?");

                // Sends a message to a different host using optional hostname and port parameters.
                udpClientB.Send(sendBytes, sendBytes.Length, RemoteIpEndPoint);
            }
            catch { }
        }

        private static object CriticalSectionLock = new object();
        public static void SendNetworkPacket(string Key, object Value)
        {
            lock (CriticalSectionLock)
            {
                bool NewValue = true;
                if (udpClientB != null && NewValue == true)
                {
                    System.Diagnostics.Debug.Print(DictName + "|" + Key + " = " + Value.ToString());
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(DictName + "|" + Key + " = " + Value.ToString());
                    // Sends a message to a different host using optional hostname and port parameters.
                    udpClientB.Send(sendBytes, sendBytes.Length, RemoteIpEndPoint);
                }
                Console.WriteLine(Key + " = " + Value);
            }
        }

    }
}
