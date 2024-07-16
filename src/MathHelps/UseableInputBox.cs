using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tomographic_Imaging_2
{
    public partial class UseableInputBox : Form
    {
        public UseableInputBox()
        {
            InitializeComponent();
        }
        public UseableInputBox(string Question)
        {
            InitializeComponent();
            mUpdated = false;
            label1.Text = Question;
        }
        public UseableInputBox(string Question,string DefaultValue)
        {
            InitializeComponent();
            mUpdated = false;
            label1.Text = Question;
            textBox1.Text = DefaultValue;
        }
        private bool mUpdated = false;

        public bool Updated
        {
            get
            {
                if (mUpdated)
                {
                    mUpdated = false;
                    return true;
                }
                return false;
            }
        }

        public static UseableInputBox Show(string Question)
        {
            UseableInputBox uib = new UseableInputBox(Question );
            uib.Show();
            return uib;
        }

        public static UseableInputBox Show(string Question,string DefaultValue)
        {
            UseableInputBox uib = new UseableInputBox(Question,DefaultValue );
            uib.Show();
            return uib;
        }
        public string  Wait()
        {
            while (Updated == false)
            {
                Application.DoEvents();
            }
            string answer = textBox1.Text;
            this.Close();
            return answer;
        }
        private void bDone_Click(object sender, EventArgs e)
        {
            mUpdated = true;
            this.Hide();
        }

        public string Answer
        {
            get
            { return textBox1.Text; }
        }


        private void UseableInputBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            mUpdated = true;
        }

       
    }
}
