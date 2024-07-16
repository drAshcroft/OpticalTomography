using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using Tomographic_Imaging_2;
using GraphingLib;

namespace DoRecons
{

    public partial class ReconForm : Form
    {
        public ReconForm()
        {
            InitializeComponent();
            testor1.reconWorkFlow1 = reconWorkFlow1;
            batchProcessor1.reconWorkFlow1 = reconWorkFlow1;
            netWorkWatcher1.reconWorkFlow1 = reconWorkFlow1;
            netWorkWatcher1.SetReconProperties(null);
            ThisHandle = this.Handle ;
           
        }


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr child, IntPtr newParent);

        public static IntPtr ThisHandle;
        static Process FancyViewer = null;
        public static  void SetVoreenData(string Filename)
        {

            if (FancyViewer == null || FancyViewer.HasExited )
            {
                FancyViewer = new Process();
                FancyViewer.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                FancyViewer.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\VoreenWindow.exe";
                FancyViewer.StartInfo.Arguments = "";
                FancyViewer.Start();
            }

            Thread.Sleep(400);
            Common.StartNetworkWriter("VoreenWindow", 1423);
            Common.SendNetworkPacket("DataFile", Filename);

            IntPtr processWindowHandle = FancyViewer.MainWindowHandle; // .GetProcessById(procId).MainWindowHandle;
            SetParent(processWindowHandle, ThisHandle  );
        }


        private delegate void MessageRecieved(string Message);
        public void NetworkMessageRecieved(string Message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MessageRecieved(NetworkMessageRecieved), Message);
            }
            else
            {
                if (LastTab == 0)
                    netWorkWatcher1.NetworkMessage(Message);
                else if (LastTab == 1)
                    batchProcessor1.NetworkMessage(Message);
                else
                    testor1.NetworkMessage(Message);
            }

        }


        private int LastTab = 0;
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> Props;
            if (LastTab == 0)
                Props = netWorkWatcher1.SaveReconProperties();
            else if (LastTab == 1)
                Props = batchProcessor1.SaveReconProperties();
            else
                Props = testor1.SaveReconProperties();

            LastTab = tabControl1.SelectedIndex;

            if (LastTab == 0)
                netWorkWatcher1.SetReconProperties(Props);
            else if (LastTab == 1)
                batchProcessor1.SetReconProperties(Props);
            else
                testor1.SetReconProperties(Props);
        }

        private void ReconForm_Load(object sender, EventArgs e)
        {

            ScriptingInterface sScriptingInterface = new ScriptingInterface();
            sScriptingInterface.MainForm = this;

            //Directory.Delete(Path.GetDirectoryName(Application.ExecutablePath) + "\\temp", true);

            if (File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + @"\DefaultSettings.txt") == true)
            {
                string line;
                using (StreamReader sr = new StreamReader(Path.GetDirectoryName(Application.ExecutablePath) + @"\DefaultSettings.txt"))
                {
                    line = sr.ReadToEnd();
                }
                string[] args = line.Split(new string[] { "\" \"" }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<string, string> dArgs = new Dictionary<string, string>();

                for (int i = 0; i < args.Length; i += 2)
                {
                    try
                    {
                        System.Diagnostics.Debug.Print(args[i] + "," + args[i + 1]);
                        dArgs.Add(args[i].Replace("\"",""), args[i + 1]);
                    }
                    catch { }
                }

                if (LastTab == 0)
                    netWorkWatcher1.SetReconProperties(dArgs);
                else if (LastTab == 1)
                    batchProcessor1.SetReconProperties(dArgs);
                else
                    testor1.SetReconProperties(dArgs);
            }
        }

        private void ReconForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dictionary<string, string> Props;
            if (LastTab == 0)
                Props = netWorkWatcher1.SaveReconProperties();
            else if (LastTab == 1)
                Props = batchProcessor1.SaveReconProperties();
            else
                Props = testor1.SaveReconProperties();



            string Args = "";
            foreach (KeyValuePair<string, string> kvp in Props)
                Args += "\"" + kvp.Key.Trim() + "\" \"" + kvp.Value.ToString() + "\" ";

            using (StreamWriter outfile = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + @"\DefaultSettings.txt"))
            {
                outfile.Write(Args);
            }


        }

        private void ReconForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Common.SendNetworkPacket("quit", "true");
            }
            catch { }
        }

        private void bShowCustomScript_Click(object sender, EventArgs e)
        {
           // CustomScript cs = new CustomScript();
           // cs.Show();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ImageViewer.ImagingTools.CreateAVIVideoEMGU(@"C:\Development\CellCT\DataIN\cct001_20090916_115617\data\Centering2.avi", Directory.GetFiles(@"C:\Development\CellCT\DataIN\cct001_20090916_115617\temp\CenterMovie", "*.jpg"));
            //ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2.RunRecon();
            //MathHelpLib.MathHelpsFileLoader.Load_Bitmap(@"C:\Development\CellCT\DataIN\cct001_20090916_115617\PP\001.png");
            MathHelpLib.MathHelpsFileLoader.Load_Tiff(@"C:\19\cct001_20120319_090339\temp\cct001\201203\19\Background.tif");
        }

        private void testor1_Load(object sender, EventArgs e)
        {

        }

       

    }
}
