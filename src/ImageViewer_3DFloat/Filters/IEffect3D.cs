using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ImageViewer.Filters;

namespace ImageViewer3D.Filters
{
    public interface IEffect3D
    {

        string EffectName { get; }
        string EffectMenu { get; }
        string EffectSubMenu { get; }
        int OrderSuggestion
        {
            get;
        }

        bool PassesPassData { get; }
        string PassDataDescription { get; }

        ReplaceStringDictionary PassData
        {
            get;
        }

        object[] DefaultProperties { get; }

        //shows parameters needed and types
        string[] ParameterList { get; }
        DataHolder DoEffect(DataEnvironment3D dataEnvironment, DataHolder SourceImage, ReplaceStringDictionary PassData, params object[] Parameters);

        void ShowInterface(IWin32Window Owner);

        string getMacroString();
    }
}
