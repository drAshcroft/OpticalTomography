using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace ImageViewer.Filters.Edit
{
    public class DeSelectAll
        : aEffectNoForm
    {
        public override string EffectName { get { return "Deselect All"; } }
        public override string EffectMenu { get { return "Edit"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion
        {
            get
            {
                return 25;
            }
        }
       
        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] {""}; }
        }

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mDataEnvironment = dataEnvironment;
            mDataEnvironment.Screen .NotifyOfSelection(null);
            string outstring = "#Edit Function\n";
            outstring += "Filter = " + this.GetType().ToString() + "()/n"
            + "holding=Filter.RunEffect(holding1,null) ";
            return null ;
        }



        public void Show(IWin32Window owner)
        {
           
        }
    }
}
