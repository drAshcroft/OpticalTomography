using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ProcessRecons
{
    public partial class DataViewControl : Form
    {
        public DataViewControl()
        {
            InitializeComponent();
        }

        [DllImport("voreenve.dll")]
        private static extern void ShowVoreenve(bool ShowInterface);

        private void bHide_Click(object sender, EventArgs e)
        {
            ShowVoreenve(false);
        }
    }
}
