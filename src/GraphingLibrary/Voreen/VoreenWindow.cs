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

namespace GraphingLibrary
{
    public partial class VoreenWindow : Form
    {
        [DllImport("voreenve.dll")]
        private static extern int CreateVoreenve(int argC, string[] Args);

        [DllImport("voreenve.dll")]
        private static extern void ShowVoreenve(bool ShowInterface);

        [DllImport("voreenve.dll")]
        private static extern void OpenWorkspace(string Filename);

        [DllImport("voreenve.dll")]
        private static extern void OpenVolume(string Filename);

        [DllImport("voreenve.dll")]
        private static extern void openNetwork(string Filename);

        [DllImport("voreenve.dll")]
        private static extern void saveNetwork(string Path, string Filename);

        public VoreenWindow()
        {
            InitializeComponent();
        }

        private string DataPath = "";

        public void ShowData(string Filename)
        {
            DataPath = Filename;

            string ExecPath= Path.GetDirectoryName(Application.ExecutablePath)+ "\\voreen";
            File.Copy(ExecPath + "\\quadview.vws", ExecPath + "\\Show.vws");

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(ExecPath + "\\Show.vws");
            XmlNodeList nodeList = xDoc.GetElementsByTagName("Origin");

            nodeList[0].Attributes["filename"].Value = Filename;

            xDoc.Save(ExecPath + "\\Show.vws");

            try
            {
                CreateVoreenve(0, new string[] { "" });
            }
            catch { }

            OpenWorkspace(ExecPath + "\\Show.vws");
        }

        private void LoadWorkspace(string Workspace)
        {
            string ExecPath = Path.GetDirectoryName(Application.ExecutablePath);
            File.Copy(ExecPath + "\\" + Workspace, ExecPath + "\\Show.vws");

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(ExecPath + "\\Show.vws");
            XmlNodeList nodeList = xDoc.GetElementsByTagName("Origin");

            nodeList[0].Attributes["filename"].Value = DataPath;

            xDoc.Save(ExecPath + "\\Show.vws");

            OpenWorkspace(ExecPath + "\\Show.vws");
        }

        private void VoreenWindow_Load(object sender, EventArgs e)
        {

        }

        private void bQuadView_Click(object sender, EventArgs e)
        {
            LoadWorkspace("quadview.vws");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadWorkspace("fancy.vws");
        }
    }
}
