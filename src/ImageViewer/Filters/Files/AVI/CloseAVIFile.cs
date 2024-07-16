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
    public class CloseAVIFileEffect:aEffectNoForm 
    {
        public override string EffectName { get { return "Close AVI File"; } }
        public override string EffectMenu { get { return "File"; } }
        public override string EffectSubMenu { get { return "AVI"; } }
        
        public override int OrderSuggestion
        {
            get
            {
                return 25;
            }
        }

        /// <summary>
        /// Closes the currect AVI file
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">No Param</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            try
            {
                CreateAVIFileEffect.aviManager.Close();
                CreateAVIFileEffect.aviManager = null;
                CreateAVIFileEffect.aviStream = null;
            }
            catch { }
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
       

        public override  void ShowInterface(IWin32Window owner)
        {
           
        }
    }*/
}
