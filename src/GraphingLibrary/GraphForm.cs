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
    public partial class GraphForm : Form
    {
        public GraphForm()
        {
            InitializeComponent();
        }
        private delegate void SetDataDelegate(PhysicalArray PhysArray);
        public void SetData(PhysicalArray PhysArray)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetDataDelegate(SetData), PhysArray);
            }
            else 
                mathGraph1.GraphData(PhysArray);
        }

        private delegate void SetDataDelegateArray(PhysicalArray PhysArray);
        public void SetData(PhysicalArray[] PhysArray)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetDataDelegateArray(SetData), PhysArray);
            }
            else
                mathGraph1.GraphData(PhysArray);
        }

        private delegate void SetDataDelegate1D(double[] PhysArray);
        public void SetData(double[] PhysArray)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetDataDelegate1D(SetData), PhysArray);
            }
            else
                mathGraph1.GraphData(PhysArray);
        }

        private delegate void SetDataDelegateImage(Bitmap  Image);
        public void SetData(Bitmap  Image)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetDataDelegateImage(SetData), Image);
            }
            else
                mathGraph1.GraphData(Image );
        }

        private delegate void SetDataDelegate2D(double[,] PhysArray);
        public void SetData(double[,] PhysArray)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetDataDelegate2D(SetData), PhysArray);
            }
            else
                mathGraph1.GraphData(PhysArray);
        }

        private delegate void SetDataDelegate2DList(List<double[,]> PhysArray);
        public void SetData(List<double[,]> PhysArray)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetDataDelegate2DList(SetData), PhysArray);
            }
            else
                mathGraph1.GraphData(PhysArray);
        }

        private delegate void  CaptionGraphDelegate(string NewCaption);
        public string CaptionGraph
        {
            set 
            {
                if (InvokeRequired )
                    BeginInvoke(new CaptionGraphDelegate(SetCaption),value  );
                else 
                    Text = value; 
            }
        }
        private void  SetCaption(String NewCaption)
        {
            Text = NewCaption;
        }
        public delegate void GraphFormClosedEvent(string GraphName);
        public event GraphFormClosedEvent  GraphFormClosed;
        private void GraphForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (GraphFormClosed != null)
                GraphFormClosed(Text  );
        }

      


        /*ImageAdjustor Adjustor;
        private void mathGraph1_ImageTypeGraphShown(bool IsImageGraph, double ContrastMin, double ContrastMax)
        {
            if (IsImageGraph)
            {
                if (Adjustor == null || Adjustor.IsDisposed)
                {
                    Adjustor = new ImageAdjustor();
                    Adjustor.Show(dockPanel1, DockState.DockRightAutoHide);
                }
            }
            else
            {
                if (Adjustor != null)
                {
                    Adjustor.Close();
                    Adjustor = null;
                }
            }

        }*/
    }
}
