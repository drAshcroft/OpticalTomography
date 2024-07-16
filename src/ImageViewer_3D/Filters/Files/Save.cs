using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer3D.Filters.Files
{
    public class SaveEffect : aEffectNoForm3D
    {
        public override string EffectName { get { return "Save Projection File"; } }
        public override string EffectMenu { get { return "File"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion
        {
            get
            {
                return 10;
            }
        }

        public override DataHolder DoEffect(DataEnvironment3D dataEnvironment, DataHolder SourceImage, ImageViewer.Filters.ReplaceStringDictionary PassData, params object[] Parameters)
        {
            
            mFilterToken = Parameters;
            if (mFilterToken == null || mFilterToken[0] == null || (string)mFilterToken[0] == "DefaultFile.raw")
            {
                System.Windows.Forms.SaveFileDialog sfd = new SaveFileDialog();
                sfd.ShowDialog(mOwner);
                mFilterToken = new string[] { sfd.FileName };
            }

            string Filename = (string)mFilterToken[0];
           
            ImagingTools3D.SaveVolumnData( SourceImage.Data ,Filename);

            return SourceImage;
        }
        public override object[] DefaultProperties
        {
            get { return new object[]{ "DefaultFile.raw"}; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "Filename|string" }; }
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
