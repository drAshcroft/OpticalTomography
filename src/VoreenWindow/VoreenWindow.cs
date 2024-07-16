using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Threading;

namespace VoreenWindow
{
    public partial class VoreenWindowController : Form
    {
        [DllImport("voreenve.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CreateVoreenve(int argC, string[] Args);

        [DllImport("voreenve.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ShowVoreenve(bool ShowInterface);

        [DllImport("voreenve.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void OpenWorkspace(string Filename);

        [DllImport("voreenve.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void OpenVolume(string Filename);

        [DllImport("voreenve.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void openNetwork(string Filename);

        [DllImport("voreenve.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void saveNetwork(string Path, string Filename);

        [DllImport("voreenve.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VoreenveWindowState(int WindowState);

        public VoreenWindowController()
        {
            InitializeComponent();
        }

        private string DataPath = "";

        public void ShowData(string Filename)
        {
            DataPath = Filename;

            string ExecPath = Path.GetDirectoryName(Application.ExecutablePath);// +"\\voreen";
            File.Copy(ExecPath + "\\quadview.vws", ExecPath + "\\Show.vws",true);

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(ExecPath + "\\Show.vws");
            XmlNodeList nodeList = xDoc.GetElementsByTagName("Origin");

            nodeList[0].Attributes["filename"].Value = Filename;

            xDoc.Save(ExecPath + "\\Show.vws");

            timer1.Enabled = true;
            try
            {
                CreateVoreenve(0, new string[] { "" });
            }
            catch { }

            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {


            timer1.Enabled = false;
            string ExecPath = Path.GetDirectoryName(Application.ExecutablePath);// +"\\voreen";
            OpenWorkspace(ExecPath + "\\Show.vws");
        }

        private void LoadWorkspace(string Workspace)
        {
            string ExecPath = Path.GetDirectoryName(Application.ExecutablePath);
            File.Copy(ExecPath + "\\" + Workspace, ExecPath + "\\Show.vws", true);

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(ExecPath + "\\Show.vws");
            XmlNodeList nodeList = xDoc.GetElementsByTagName("Origin");

            nodeList[0].Attributes["filename"].Value = DataPath;

            xDoc.Save(ExecPath + "\\Show.vws");

            OpenWorkspace(ExecPath + "\\Show.vws");
        }
        CommonNetwork Network;
        private void VoreenWindow_Load(object sender, EventArgs e)
        {
            Network = new CommonNetwork();
            Network.StartNetworkListener(this, 1423);
            Network.NetworkMessageRecieved += new CommonNetwork.NetworkMessageRecievedEvent(Network_NetworkMessageRecieved);

            string ExecPath = Path.GetDirectoryName(Application.ExecutablePath);
            string[] Files = Directory.GetFiles(ExecPath, "*.vws");
            foreach (string s in Files)
            {
                string file = Path.GetFileNameWithoutExtension(s);
                if (file.ToLower() != "show")
                {
                    listBox1.Items.Add(file);
                }
            }
            for (int i = 0; i < listBox1.Items.Count;i++ )
            {
                if (listBox1.Items[i].ToString() == "quadview")
                    listBox1.SetSelected(i, true);
            }
        }

        private delegate void MessageRecieved(string Message);

        void Network_NetworkMessageRecieved(string Message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MessageRecieved(Network_NetworkMessageRecieved), Message);
            }
            else 
            {
                try
                {
                    Console.WriteLine(Message);

                    string[] Parts = Message.Split('|', '=');

                    Console.WriteLine(Parts.Length);
                    string command, data;
                    if (Parts.Length == 2)
                    {
                        command = Parts[0].Trim().ToLower();
                        data = Parts[1].Trim();
                    }
                    else
                    {
                        command = Parts[1].Trim().ToLower();
                        data = Parts[2].Trim();

                    }

                    Console.WriteLine(command);
                    Console.WriteLine(data);
                    switch (command)
                    {
                        case "datafile":
                            Console.WriteLine("datafile");
                            ShowData(data);
                            break;
                        case "workspace":
                            Console.WriteLine("workspace");
                            LoadWorkspace(data);
                            break;
                        case "quit":
                            this.Close();
                            break;
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void bQuadView_Click(object sender, EventArgs e)
        {
            LoadWorkspace("quadview.vws");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadWorkspace("fancy.vws");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Network_NetworkMessageRecieved(@"VoreenWindow|DataFile = C:\Development\CellCT\DataIN\cct001_20090916_115617\Data\ProjectionObject.dat");
           // Network.SendNetworkPacket("DataFile", @"C:\Development\CellCT\DataIN\cct001_20090916_115617\Data\ProjectionObject.dat");
        }

        private void VoreenWindowController_FormClosed(object sender, FormClosedEventArgs e)
        {
            Network.Quit = true;
        }

       

        private void bOpen_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                if (listBox1.GetSelected(i))
                {
                    LoadWorkspace(listBox1.Items[i].ToString() + ".vws");
                }
            }
        }

       
    }
}
