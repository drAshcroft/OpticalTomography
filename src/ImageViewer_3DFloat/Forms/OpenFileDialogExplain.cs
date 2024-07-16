using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FileDialogExtenders;

namespace ImageViewer3D
{
    public partial class OpenFileDialogExplain : FileDialogControlBase
    {
        private bool mMultiSelect = true;
        public OpenFileDialogExplain(string InstructionText, bool Multiselect)
        {
            InitializeComponent();
            label2.Text = InstructionText;
            mMultiSelect = Multiselect;

        }
        private string mFilter;
        public string Filter
        {
            get { return mFilter; }
            set { mFilter = value; }
        }


        #region Overrides
        protected override void OnPrepareMSDialog()
        {
            OpenFileDialog ofd= new OpenFileDialog();
            ofd.Multiselect=mMultiSelect ;
            ofd.Filter = mFilter;

            MSDialog = ofd;
            
            base.FileDlgInitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (Environment.OSVersion.Version.Major < 6)
                MSDialog.SetPlaces(new object[] { @"c:\", (int)Places.MyComputer, (int)Places.Favorites, (int)Places.Printers, (int)Places.Fonts, });

            base.OnPrepareMSDialog();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1807:AvoidUnnecessaryStringCreation", MessageId = "filePath")]
        private void MyOpenFileDialogControl_FileNameChanged(IWin32Window sender, string filePath)
        {
           /* if (filePath.ToLower().EndsWith(".bmp") ||
                filePath.ToLower().EndsWith(".jpg") ||
                filePath.ToLower().EndsWith(".png") ||
                filePath.ToLower().EndsWith(".tif") ||
                filePath.ToLower().EndsWith(".gif"))
            {
                if (pbxPreview.image != null)
                    pbxPreview.image.Dispose();

                try
                {
                    FileInfo fi = new FileInfo(filePath);
                    pbxPreview.image = Bitmap.FromFile(filePath);
                    lblSizeValue.Text = (fi.Length / 1024).ToString() + "KB";
                    lblColorsValue.Text = GetColorsCountFromImage(pbxPreview.image);
                    lblFormatValue.Text = GetFormatFromImage(pbxPreview.image);
                    FileDlgEnableOkBtn = true;
                }
                catch (Exception) { FileDlgEnableOkBtn = false; }
            }
            else
            {
                if (pbxPreview.image != null)
                    pbxPreview.image.Dispose();
                pbxPreview.image = null;
            }*/
        }

        #endregion

        public string InstructionText
        {
            set { label2.Text = value; }
        }
    }
}
