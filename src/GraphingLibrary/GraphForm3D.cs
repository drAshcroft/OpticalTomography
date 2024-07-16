using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathHelpLib;

namespace GraphingLib
{
    public partial class GraphForm3D : Form
    {
        public GraphForm3D()
        {
            InitializeComponent();
        }

        public bool Zooming
        {
            set { graph3DSliceViewerMultiAngleEditor1.Zooming = value; }

        }

        private delegate void SetDataDelegateString(string Filename);
        public void SetData(string  Filename)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetDataDelegateString(SetData), Filename);
            }
            else
                graph3DSliceViewerMultiAngleEditor1.SetData(Filename);
               // mathGraph1.GraphData(Filename);
        }

        PhysicalArray Data=null;

        private delegate void SetDataDelegate(PhysicalArray PhysArray);
        public void SetData(PhysicalArray PhysArray)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetDataDelegate(SetData), PhysArray);
            }
            else
            {
                //mathGraph1.GraphData(PhysArray);
                graph3DSliceViewerMultiAngleEditor1.SetData(PhysArray);
                Data = PhysArray;
            }
        }

        private delegate void SetDataDelegateF(float[, ,] PhysArray);
        public void SetData(float[,,] PhysArray)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetDataDelegateF(SetData), PhysArray);
            }
            else
            {
                //mathGraph1.GraphData(PhysArray);
                graph3DSliceViewerMultiAngleEditor1.SetData(PhysArray);
               
            }
        }
      

        private delegate void CaptionGraphDelegate(string NewCaption);
        public string CaptionGraph
        {
            set
            {
                if (InvokeRequired)
                    BeginInvoke(new CaptionGraphDelegate(SetCaption), value);
                else
                    Text = value;
            }
        }
        private void SetCaption(String NewCaption)
        {
            Text = NewCaption;
        }
        public delegate void GraphFormClosedEvent(string GraphName);
        public event GraphFormClosedEvent GraphFormClosed;
        private void GraphForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (GraphFormClosed != null)
                    GraphFormClosed(Text);
            }
            catch { }
        }

        private void GraphForm3D_FormClosing(object sender, FormClosingEventArgs e)
        {
            graph3DSliceViewerMultiAngleEditor1.FormClosing();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (DialogResult.OK == sfd.ShowDialog())
                Data.SaveData(sfd.FileName);
        }

      


    }
}
