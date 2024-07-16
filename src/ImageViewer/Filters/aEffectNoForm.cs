using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer.Filters
{
    public abstract class aEffectNoForm : IEffect
    {
        #region InternalVariables
        /// <summary>
        /// contains the parameters sent over from python.  this would be nicer, but we do not know what python will send accross
        /// </summary>
        protected object[] mFilterToken;

        /// <summary>
        /// Contains a copy of the global data environment.  
        /// </summary>
        protected DataEnvironment mDataEnvironment;

        #endregion

            #region MenuStuff

            /// <summary>
            /// Name displayed on the menu
            /// </summary>
            public abstract string EffectName { get; }

            /// <summary>
            /// Menu that this effect is classified under
            /// </summary>
            public abstract string EffectMenu { get; }

            /// <summary>
            /// Sub menu for this effect
            /// </summary>
            public abstract string EffectSubMenu { get; }

            /// <summary>
            /// Suggestion of location of this effect on the submenu
            /// </summary>
            public virtual int OrderSuggestion
            {
                get { return 1; }
            }

            #endregion

        #region PassData

        /// <summary>
        /// This is used if the filter passes data on the pass data channel
        /// </summary>
        public virtual bool PassesPassData
        { get { return false; } }

        /// <summary>
        /// Describes the data that is being passed.  name and datatype
        /// </summary>
        public virtual string PassDataDescription { get { return ""; } }


        protected ReplaceStringDictionary mPassData;

        /// <summary>
        /// The returned data from all the types will be here
        /// </summary>
        public ReplaceStringDictionary PassData
        {
            get { return mPassData; }
            set { mPassData = value; }
        }
        #endregion

        /// <summary>
        /// An object array that holds all the default properties for the given effect, in the order desired.
        /// </summary>
        public abstract object[] DefaultProperties { get; }

        /// <summary>
        /// Names of the properties.  Only the default list needs to be listed
        /// </summary>
        public abstract string[] ParameterList { get; }

        /// <summary>
        /// The needed effect.  It should always return an imageholder
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public abstract object  DoEffect(
           DataEnvironment dataEnvironment, object  SourceImage,
           ReplaceStringDictionary PassData,
           params object[] Parameters);

        /// <summary>
        /// Just satisfying the requirement.  there is not interface to show for these effects
        /// </summary>
        /// <param name="Owner"></param>
        public virtual void ShowInterface(IWin32Window Owner)
        {

        }

        /// <summary>
        /// Returns the string that is used to call the effect with python
        /// </summary>
        /// <returns></returns>
        public virtual string getMacroString()
        {
            return EffectHelps.FormatMacroString(this,mFilterToken );
        }
       
    }
}
