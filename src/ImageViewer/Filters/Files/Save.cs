using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using MathHelpLib;

namespace ImageViewer.Filters.Files
{
    public class SaveEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Save File"; } }
        public override string EffectMenu { get { return "File"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion
        {
            get
            {
                return 10;
            }
        }

        /// <summary>
        /// saves an image 
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">Filename as string</param>
        /// <returns></returns>
        public override object  DoEffect(DataEnvironment dataEnvironment, object SourceImage, 
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            ///get the filename if it is not given
            if (mFilterToken == null || mFilterToken[0] == null)
            {
                System.Windows.Forms.SaveFileDialog sfd = new SaveFileDialog();
                sfd.ShowDialog(mOwner);
                mFilterToken = new string[] { sfd.FileName };
            }

            string Filename = (string)mFilterToken[0];

           //save the image in the ROI
            MathHelpsFileLoader.Save_Bitmap(Filename, SourceImage);
            return SourceImage;
        }
        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }

       
        IWin32Window mOwner;
        public override void ShowInterface(IWin32Window owner)
        {
            mOwner = owner;
            System.Windows.Forms.SaveFileDialog sfd = new SaveFileDialog();
           // sfd.ShowDialog(owner);
           // mFilterToken = new string[1] { sfd.FileName };
        }
    }
}
