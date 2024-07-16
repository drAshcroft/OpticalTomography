using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Tomographic_Imaging_2
{
    
    public partial class ImageSelector : DockContent 
    {
        private ImageSelection.ImageStatistics ImageStatisicsForm;

        public ImageSelector()
        {
            InitializeComponent();
            ImageStatisicsForm = new Tomographic_Imaging_2.ImageSelection.ImageStatistics();
            // TomographicMainForm.ShowDockContent(ImageStatisicsForm);
            ImageStatisicsForm.Show(this.dockPanel1, DockState.DockRightAutoHide);
        }
        
        #region Filehandling

        private string[] mFileNames;
        public void SetFilenames(string[] ImageFiles)
        {
            mFileNames = ImageFiles;
            tFileSelector.Visible = true;
            tFileSelector.Maximum = ImageFiles.Length-1;

            Bitmap b = new Bitmap(mFileNames[tFileSelector.Value]);
            viewerControl1.SetImage(b);
            tFileSelector.Visible = true;
        }
        public void SetFilenames(string ImageFile)
        {
            mFileNames = new string[]{ ImageFile};
            tFileSelector.Visible = false;
            tFileSelector.Value = 0;
            //tFileSelector.Maximum = ImageFiles.LengthX - 1;

            Bitmap b = new Bitmap(mFileNames[tFileSelector.Value]);
            viewerControl1.SetImage(b);
        }

        public void SetBitmap(Bitmap showBitmap)
        {
            viewerControl1.SetImage(showBitmap);
        }

        private void tFileSelector_ValueChanged(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(mFileNames[tFileSelector.Value ]);
            viewerControl1.SetImage(b);
        }

        #endregion

        #region SelectionHandling

        public ImageViewer.Tools.aDrawingTool SelectionTool
        {
            get { return viewerControl1.ActiveDrawingTool ; }
            set { viewerControl1.ActiveDrawingTool = value; }
        }

        private ImageViewer.ISelection mActiveSelection;
        public Bitmap ActiveImage
        {
            get { return viewerControl1.ActiveImage; }
            set { viewerControl1.SetImage(value); }
        }

        public ImageViewer.ISelection ActiveSelection
        {
            get { return mActiveSelection; }
        }

        private void viewerControl1_SelectionPerformed(ImageViewer.ISelection Selection)
        {
            mActiveSelection = Selection;
            if (cShowStatistics.Checked)
            {
                if (ImageStatisicsForm == null || ImageStatisicsForm.IsDisposed)
                {
                    ImageStatisicsForm = new Tomographic_Imaging_2.ImageSelection.ImageStatistics();
                   // TomographicMainForm.ShowDockContent(ImageStatisicsForm);
                    ImageStatisicsForm.Show(this.dockPanel1, DockState.DockRightAutoHide);
                }
               // ImageStatisicsForm.Show();
                ImageStatisicsForm.ShowStatistics(Selection, ActiveImage);

            }
        }

        #endregion

        #region FormUpdater

        public event EventHandler ImageSelectorUpdated;

        private bool mUpdated = false;

        /// <summary>
        /// This works like a labview control,  when it is checked it then switches to false to prepare for the next round.  
        /// It is the callers responsibility to get the selection information.
        /// </summary>
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
            if (ImageSelectorUpdated != null)
                ImageSelectorUpdated(sender, e);
        }
        private void bClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void WaitForInput(string Prompt)
        {
            label2.Text = Prompt;
            viewerControl1.DrawingToolsVisible = true;
            this.Show();
            while (SelectionsUpdated == false)
                Application.DoEvents();

        }
        public void WaitForInput(string Prompt, ImageViewer.Tools.aDrawingTool SelectionTool)
        {
            label2.Text = Prompt;
            viewerControl1.DrawingToolsVisible = false;
            this.SelectionTool = SelectionTool;
            this.Show();
            while (SelectionsUpdated == false)
                Application.DoEvents();

        }

        #endregion

        #region Events

        private void ImageSelector_ResizeEnd(object sender, EventArgs e)
        {
            viewerControl1.IMAQViewerControl_Resize(sender , e);
        }
        private void viewerControl1_ToolTipUpdate(string ToolTip)
        {
            toolStripStatusLabel1.Text = ToolTip;
        }
        #endregion

        private void viewerControl1_MouseMoving(object sender, MouseEventArgs e)
        {
            label3.Text = "( " + e.X + ", " + e.Y + ")";
        }
       
    }
}
