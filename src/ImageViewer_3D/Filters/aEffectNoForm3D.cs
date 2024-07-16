using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ImageViewer.Filters;

namespace ImageViewer3D.Filters
{
    public abstract class aEffectNoForm3D : IEffect3D
    {
        #region InternalVariables
            protected object[] mFilterToken;
            protected DataEnvironment3D mDataEnvironment;

        #endregion

            #region MenuStuff
            public abstract string EffectName { get; }
        public abstract string EffectMenu { get; }
        public abstract string EffectSubMenu { get; }
        public virtual int OrderSuggestion
        {
            get { return 1; }
        }
            #endregion

        #region PassData
        public virtual bool PassesPassData
        { get { return false; } }
        public virtual string PassDataDescription { get { return ""; } }

        protected ReplaceStringDictionary mPassData;
        public ReplaceStringDictionary PassData
        {
            get { return mPassData; }
            set { mPassData = value; }
        }
        #endregion

        public abstract object[] DefaultProperties { get; }
        public abstract  string[] ParameterList { get ; }

        public abstract DataHolder DoEffect(
           DataEnvironment3D dataEnvironment, DataHolder SourceImage,
           ReplaceStringDictionary PassData,
           params object[] Parameters);


        public virtual void ShowInterface(IWin32Window Owner)
        {

        }
        public virtual string getMacroString()
        {
            return EffectHelps3D.FormatMacroString(this,mFilterToken );
        }
       
    }
}
