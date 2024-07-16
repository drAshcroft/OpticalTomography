using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer.Filters.Files
{
    public class OpenEffect:aEffectNoForm 
    {
        public override string EffectName { get { return "Open File"; } }
        public override string EffectMenu { get { return "File"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             ReplaceStringDictionary PassData, params object[] Parameters)
        {
            ///If no file has been created, start a new file
            if (Parameters == null)
            {
                OpenFileDialog sfd = new OpenFileDialog();
                DialogResult ret = sfd.ShowDialog();
                if (DialogResult.OK == ret)
                {
                    mFilterToken =  new string[1] { sfd.FileName };
                }
            }
            else
                mFilterToken = Parameters;

           //return the file and all the intialization will be taken care of
            //todo: make this handle opening a file within a ROI work correctly
            try
            {
                string Filename = (string)mFilterToken[0];
                return MathHelpLib.MathHelpsFileLoader. Load_Bitmap(Filename);
            }
            catch
            {
                return SourceImage;
            }
        }
        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }
       

        public override  void ShowInterface(IWin32Window owner)
        {
            //System.Windows.Forms.OpenFileDialog sfd = new OpenFileDialog();
            //sfd.ShowDialog(owner);
            //mFilterToken = new GeneralToken();
            //mFilterToken.Parameters = new string[1] { sfd.FileName };
        }
    }
}
