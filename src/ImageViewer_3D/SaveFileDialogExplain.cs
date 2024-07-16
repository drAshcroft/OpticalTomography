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
    public partial class SaveFileDialogExplain : FileDialogControlBase
    {
        public SaveFileDialogExplain(string InstructionText)
        {
            InitializeComponent();
            FileDlgType = Win32Types.FileDialogType.SaveFileDlg;
            FileDlgCheckFileExists = false;
            label2.Text = InstructionText;
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
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = mFilter;
            MSDialog = sfd;
            base.FileDlgInitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
           // if (Environment.OSVersion.Version.Major < 6)
           //     MSDialog.SetPlaces(new object[] { @"c:\", (int)Places.MyComputer, (int)Places.Favorites, (int)Places.Printers, (int)Places.Fonts, });
            base.OnPrepareMSDialog();
            sfd.Filter = mFilter;
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
                 if (pbxPreview.Image != null)
                     pbxPreview.Image.Dispose();

                 try
                 {
                     FileInfo fi = new FileInfo(filePath);
                     pbxPreview.Image = Bitmap.FromFile(filePath);
                     lblSizeValue.Text = (fi.Length / 1024).ToString() + "KB";
                     lblColorsValue.Text = GetColorsCountFromImage(pbxPreview.Image);
                     lblFormatValue.Text = GetFormatFromImage(pbxPreview.Image);
                     FileDlgEnableOkBtn = true;
                 }
                 catch (Exception) { FileDlgEnableOkBtn = false; }
             }
             else
             {
                 if (pbxPreview.Image != null)
                     pbxPreview.Image.Dispose();
                 pbxPreview.Image = null;
             }*/
        }

        #endregion

        public string InstructionText
        {
            set { label2.Text = value; }
        }
    }
}
