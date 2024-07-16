using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tomographic_Imaging_2
{
    public partial class AdvancedSelectBox : UserControl
    {
        public delegate void BoxSelectedEvent();
        public event BoxSelectedEvent FirstBoxSelected;
        public event BoxSelectedEvent SecondBoxSelected;
        public AdvancedSelectBox()
        {
            InitializeComponent();
        }

        public string FirstBoxLabel
        {
            get { return label5.Text; }
            set { label5.Text = value; }
        }
        public string SecondBoxLabel
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }
        public ListBox FirstBox
        {
            get { return listBox1; }
        }
        public ListBox SecondBox
        {
            get { return listBox2; }
        }

        public string SecondBoxSelectedItem
        {
            get
            {
                for (int i = 0; i < listBox2.Items.Count; i++)
                {
                    if (listBox2.GetSelected(i) == true)
                        return listBox2.Items[i].ToString();
                }
                return listBox2.Items[0].ToString();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            object o= listBox1.SelectedItem;
            bAccept.Enabled = true;
            for (int x = 0; x < listBox2.Items.Count; x++)
            {
                // Determine if the item is selected.
                if (listBox2.GetSelected(x) == true)
                    // Deselect all items that are selected.
                    listBox2.SetSelected(x, false);
            }
            bReject.Enabled = false;
            bMoveUp.Enabled = false;
            bMoveDown.Enabled = false;
            listBox1.SelectedItem = o;
            if (FirstBoxSelected != null)
                FirstBoxSelected();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            object o = listBox2.SelectedItem;
            bAccept.Enabled = false ;
            for (int x = 0; x < listBox1.Items.Count; x++)
            {
                // Determine if the item is selected.
                if (listBox1.GetSelected(x) == true)
                    // Deselect all items that are selected.
                    listBox1.SetSelected(x, false);
            }
            bReject.Enabled = true ;
            bMoveUp.Enabled = true ;
            bMoveDown.Enabled = true ;
            listBox2.SelectedItem = o;
            if (SecondBoxSelected != null)
                SecondBoxSelected();
        }

        private void bAccept_Click(object sender, EventArgs e)
        {
            listBox2.Items.Add(listBox1.SelectedItem);

        }

        private void bReject_Click(object sender, EventArgs e)
        {
            listBox2.Items.Remove(listBox2.SelectedItem);
        }

        private void bMoveUp_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItems.Count > 0)
            {
                object selected = listBox2.SelectedItem;
                int indx = listBox2.Items.IndexOf(selected);
                int totl = listBox2.Items.Count;

                if (indx == 0)
                {
                    listBox2.Items.Remove(selected);
                    listBox2.Items.Insert(totl - 1, selected);
                    listBox2.SetSelected(totl - 1, true);
                }
                else
                {
                    listBox2.Items.Remove(selected);
                    listBox2.Items.Insert(indx - 1, selected);
                    listBox2.SetSelected(indx - 1, true);
                }
            }
        }

        private void bMoveDown_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                object selected = listBox1.SelectedItem;
                int indx = listBox1.Items.IndexOf(selected);
                int totl = listBox1.Items.Count;

                if (indx == totl - 1)
                {
                    listBox1.Items.Remove(selected);
                    listBox1.Items.Insert(0, selected);
                    listBox1.SetSelected(0, true);
                }
                else
                {
                    listBox1.Items.Remove(selected);
                    listBox1.Items.Insert(indx + 1, selected);
                    listBox1.SetSelected(indx + 1, true);
                }
            }
        }

    }
}
