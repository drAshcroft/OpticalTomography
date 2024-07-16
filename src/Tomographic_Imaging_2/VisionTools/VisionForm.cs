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
    public partial class VisionForm : WeifenLuo.WinFormsUI.Docking.DockContent 
    {
        public VisionForm()
        {
            InitializeComponent();
            tFileSelector.ValueChanged += new EventHandler(tFileSelector_ValueChanged);
        }

        private string[] FileNames;
        private Bitmap SeekImage;

        public void SetFilenames(string[] ImageFiles)
        {
            FileNames = ImageFiles;
            tFileSelector.Visible = true;
            tFileSelector.Maximum = ImageFiles.Length - 1;
           // tFileSelector.Value = 11;
                Bitmap b = new Bitmap(FileNames[tFileSelector.Value]);
                imageFinder1.PerformFind(b);
        }

        public void SetSeekImage(Bitmap SeekImage)
        {
            this.SeekImage = SeekImage;
            //imageFinder1.LoadSeekImage(SeekImage);
            if (FileNames != null)
            {
                Bitmap b = new Bitmap(FileNames[tFileSelector.Value]);
                imageFinder1.PerformFind(b);
            }
        }
        private void tFileSelector_ValueChanged(object sender, EventArgs e)
        {
                Bitmap b = new Bitmap(FileNames[tFileSelector.Value]);
                imageFinder1.PerformFind(b);
        }

       
        public VisionHelper.IEdgeFound [] GetEdges(string Filename)
        {
            Bitmap b = new Bitmap(Filename );
            return  imageFinder1.PerformFind(b);
        }
        public VisionHelper.IEdgeFound[] GetEdges(Bitmap image)
        {
            return imageFinder1.PerformFind(image );
        }

        public Bitmap ProcessedImage
        {
            get { return imageFinder1.ProcessedImage; }
        }
        #region FormHandler
        public event EventHandler VisionFormUpdate;

        private bool mUpdated = false;
        /// <summary>
        /// This works like a labview control,  when it is checked it then switches to false to prepare for the next round.  
        /// It is the callers responsibility to get the selection information.
        /// </summary>
        public bool VisionFormUpdated
        {
            get
            {
                if (mUpdated == true)
                {
                    mUpdated = false;
                    return true;
                }
                return false;
            }
        }
        public bool SelectionsUpdated
        {
            get
            {
                if (mUpdated == true)
                {
                    mUpdated = false;
                    return true;
                }
                return false;
            }
        }
        private void bUpdate_Click(object sender, EventArgs e)
        {
            mUpdated = true;
            if (VisionFormUpdate != null)
                VisionFormUpdate(sender, e);
        }
        private void bClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void WaitForInput(string Prompt)
        {
            this.Show();
            while (SelectionsUpdated == false)
                Application.DoEvents();

        }

        #endregion
    }
}
