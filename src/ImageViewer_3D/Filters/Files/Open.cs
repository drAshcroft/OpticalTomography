using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer3D.Filters.Files
{
    public class OpenEffect:aEffectNoForm3D
    {
        public override string EffectName { get { return "Open Projection File"; } }
        public override string EffectMenu { get { return "File"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        public override DataHolder DoEffect(DataEnvironment3D dataEnvironment, DataHolder SourceImage, ImageViewer.Filters.ReplaceStringDictionary PassData, params object[] Parameters)
        {

            mFilterToken = Parameters;
            if (mFilterToken == null || mFilterToken[0] == null || (string)mFilterToken[0] == "DefaultFile.raw")
            {
                OpenFileDialog sfd = new OpenFileDialog();
                sfd.Multiselect = true;
                DialogResult ret = sfd.ShowDialog();
                if (DialogResult.OK == ret)
                {
                    
                    mFilterToken =  sfd.FileNames;
                }
            }
            else
                mFilterToken = Parameters;

           
            try
            {
                double[, ,] Data;
                if (mFilterToken[0].GetType() == typeof(string))
                {
                    Data = ImagingTools3D.OpenVolumnData((string)mFilterToken[0]);
                }
                else
                {
                    string[] Filenames = (string[])mFilterToken[0];
                    Data = ImagingTools3D.OpenVolumnData(Filenames);
                }
                if (Data != null)
                {
                    dataEnvironment.OriginalData.Data = null;
                    return new DataHolder(Data);
                }
                else
                    return SourceImage;
            }
            catch
            {
                return SourceImage;
            }
        }
        public override object[] DefaultProperties
        {
            get { return new object[] { "DefaultFile.raw" }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "Filename(array for stack)|" }; }
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
