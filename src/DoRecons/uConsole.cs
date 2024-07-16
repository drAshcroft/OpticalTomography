using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoRecons
{
    public partial class uConsole : UserControl
    {
        
        public uConsole()
        {
            InitializeComponent();
        }

        List<string> Messages = new List<string>();
        public void AddLine(string Message)
        {
            Messages.Add(Message);
            if (Messages.Count > 50)
                Messages.RemoveAt(0);
            richTextBox1.Lines = Messages.ToArray();
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
           // richTextBox1.Focus();
        }
    }
}
