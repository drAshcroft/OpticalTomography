using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;

using System.Threading;
using ImageViewer.PythonScripting;

namespace ImageViewer.Filters.Files.AVI
{/*
    public class CreateAVIFileFromFramesEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Create AVI File From Frames"; } }
        public override string EffectMenu { get { return "File"; } }
        public override string EffectSubMenu { get { return "AVI"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 1;
            }
        }

        public static AviManager aviManager = null;
        public static VideoStream aviStream = null;
        public static string OpenFilename = "";
        public static void StartAVIStreams(Bitmap FirstImage)
        {
            if (OpenFilename != "")
            {
                aviManager = new AviManager(OpenFilename, false);
                aviStream = aviManager.AddVideoStream(false, 33, FirstImage);
            }
        }

        private string[] ParamToList(object oParam)
        {
            string[] bitmaps;
            if (oParam.GetType().ToString().Contains("System.Collections.Generic.List") == true)
            {
                if (oParam.GetType() == typeof(List<ImageFile>))
                {
                    List<ImageFile> files = (List<ImageFile>)oParam;
                    bitmaps = new string[files.Count];
                    for (int i = 0; i < files.Count; i++)
                        bitmaps[files[i].Index] = files[i].Filename;
                }
                else
                    bitmaps = ((List<string>)oParam).ToArray();
            }
            else if (oParam.GetType() == typeof(IronPython.Runtime.List))
            {
                IronPython.Runtime.List pythonList = (IronPython.Runtime.List)oParam;
                bitmaps = new string[pythonList.Count];
                for (int i = 0; i < pythonList.Count; i++)
                    bitmaps[i] = (string)pythonList[i];
            }
            else
                bitmaps = (string[])oParam;
            return bitmaps;
        }


        /// <summary>
        /// Creates an AVI file that can have bitmaps added in.  The bitmaps must all be 
        /// the same size.  There can only be one avi file open at a time
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">AVI Filename, director with frames (constant filename then extension);image library; or framefilenames as string[]</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
                    ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;

            if (mFilterToken == null)
            {
                mFilterToken = DefaultProperties;

                OpenFileDialogExplain ofd = new OpenFileDialogExplain("Please select the frames that you wish to have made into a movie", true);
                DialogResult ret = ofd.ShowDialog();
                if (ret == DialogResult.OK)
                {
                    SaveFileDialogExplain sfd = new SaveFileDialogExplain("Please indicate the desired movie file name");
                    sfd.Filter = "AVI file *.AVI| *.AVI";
                    ret = sfd.ShowDialog();

                    mFilterToken[0] = ofd.MSDialog.FileNames;
                    if (ret == DialogResult.OK)
                    {
                        mFilterToken[1] = sfd.MSDialog.FileName;
                        //ImagingTools.CreateAVIVideo(sfd.MSDialog.FileName, ofd.MSDialog.FileNames);
                    }
                    //      MessageBox.Show("File Generation is complete", "");
                }
            }

            string Filename = (string)mFilterToken[0];
            if (File.Exists(Filename) == true)
                File.Delete(Filename);

            if (mFilterToken[1].GetType() == typeof(string))
            {
               
                string framename= (string)mFilterToken[1];
                string exten =(string)mFilterToken[2];

                string[] Bitmaps = Directory.GetFiles( Path.GetDirectoryName(framename ), Path.GetFileNameWithoutExtension(framename ) + "*." + exten );
                Bitmaps = EffectHelps.SortNumberedFiles(Bitmaps );
                OpenFilename = Filename;

                ImagingTools.CreateAVIVideo(Filename, Bitmaps);


               // ImagingTools.CreateMovieFFMPEG(Filename, framename, exten);
                return SourceImage;

            }
            if (mFilterToken[1].GetType() == typeof(string[]))
            {
                string[] bitmaps;

                bitmaps = EffectHelps.SortNumberedFiles((string[])mFilterToken [1]);

                OpenFilename = Filename;
                ImagingTools.CreateAVIVideo(Filename, bitmaps);
                return SourceImage;
            }
            else if (mFilterToken[1].GetType() == typeof(ImageLibrary))
            {
                ImagingTools.CreateAVIVideo(Filename, (ImageLibrary)mFilterToken[1]);
            }
            return SourceImage;
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { new string[1], "DefaultFilename.avi", false }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "BitmapFilenames|string[]", "AVI_filename|string", "Threaded|bool", "ThreadID|int" }; }
        }

        public override void ShowInterface(IWin32Window owner)
        {

        }
    }*/
}
