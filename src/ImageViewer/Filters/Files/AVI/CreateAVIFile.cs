using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;


namespace ImageViewer.Filters.Files.AVI
{
    /*
    public class CreateAVIFileEffect : aEffectNoForm 
    {
        public override string EffectName { get { return "Create AVI File"; } }
        public override string EffectMenu { get { return "File"; } }
        public override string EffectSubMenu { get { return "AVI"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
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


        /// <summary>
        /// Creates an AVI file that can have bitmaps added in.  The bitmaps must all be 
        /// the same size.  There can only be one file open at a time
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">AVI Filename</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
                    ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;

            if (mFilterToken == null ||  mFilterToken[0]==null)
            {
                System.Windows.Forms.SaveFileDialog sfd = new SaveFileDialog();
                
                sfd.Filter = "AVI Files (*.avi) | *.avi";
                sfd.ShowDialog();
                mFilterToken = new string[1] { sfd.FileName };
            }

            string Filename = (string)mFilterToken [0];

            OpenFilename = Filename;    
     
            return SourceImage;
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { null }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }
     

        public override  void ShowInterface(IWin32Window owner)
        {
        }
    }

*/
}
