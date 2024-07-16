using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;


namespace ImageViewer.Filters
{
    public interface IEffect
    {

        /// <summary>
        /// Name displayed on the menu
        /// </summary>
       string EffectName { get; }

        /// <summary>
        /// Menu that this effect is classified under
        /// </summary>
        string EffectMenu { get; }

        /// <summary>
        /// Sub menu for this effect
        /// </summary>
        string EffectSubMenu { get; }

        /// <summary>
        /// Suggestion of location of this effect on the submenu
        /// </summary>
        int OrderSuggestion
        {
            get;
        }

        /// <summary>
        /// This is used if the filter passes data on the pass data channel
        /// </summary>
        bool PassesPassData { get; }

        /// <summary>
        /// Describes the data that is being passed.  name and datatype
        /// </summary>
        string PassDataDescription { get; }

        /// <summary>
        /// Must take data from the script, if needed add an entry and then pass the dictionary allong.  Failure to do this will result in discomfort for the user
        /// </summary>
        ReplaceStringDictionary PassData
        {
            get;
        }

        /// <summary>
        /// An object array that holds all the default properties for the given effect, in the order desired.
        /// </summary>
        object[] DefaultProperties { get; }

        //shows parameters needed and types
         string[] ParameterList { get; }


        /// <summary>
        /// does the effect.  should always return an imageholder,  needs to accept bitmats and imageholders
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
         object DoEffect(DataEnvironment dataEnvironment, object SourceImage, ReplaceStringDictionary  PassData, params object[] Parameters);
 
        /// <summary>
        /// if this has as form, it should should when htis is called
        /// </summary>
        /// <param name="Owner"></param>
        void ShowInterface(IWin32Window Owner);

        /// <summary>
        /// returns an example string on how to call this effect
        /// </summary>
        /// <returns></returns>
        string getMacroString();
    }
}
