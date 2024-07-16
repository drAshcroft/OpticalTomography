using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using ImageViewer.Filters;
using System.Threading;

namespace ImageViewer.PythonScripting.Arrays
{
    /// <summary>
    /// Allows all the threads to create an array together without steping all over each other
    /// createglobalarray must be called before this is called
    /// </summary>
    public class AddPointArrayTool : aEffectNoForm
    {
        public override string EffectName { get { return "Add Array Point"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Arrays"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
       
        public override bool PassesPassData
        {
            get
            {
                return false;
            }
        }

        public static void AddPoint( DataEnvironment dataEnvironment, string ArrayName, int ImageIndex, object value)
        {
            lock (CriticalSectionLock)
            {
                ((ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GlobalArrayTokens)dataEnvironment.EffectTokens["CreateGlobalArray"]).
                   ScriptGlobalArrays[ArrayName][ImageIndex] = EffectHelps.ConvertToDouble(value);
            }
        }

        private static  object CriticalSectionLock = new object();
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            if (Parameters == null)
            {
                mFilterToken = DefaultProperties;
                string ArrayName = "DefaultArray";
                InputBox.ShowDialog("Add point to array", "Enter name of array", ref ArrayName);
                mFilterToken[0] = ArrayName;
            }
            string aName = (string)mFilterToken[0];
            int ImageIndex = (int)mFilterToken[1];

            lock (CriticalSectionLock)
            {
                ((ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GlobalArrayTokens)dataEnvironment.EffectTokens["CreateGlobalArray"]).
                   ScriptGlobalArrays[aName][ImageIndex]= EffectHelps.ConvertToDouble(mFilterToken[2]);
            }
            return SourceImage ;
        }
        public override object[] DefaultProperties
        {
            get { return new object[] { "DefaultArray",0, 0d }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "ArrayName|string","ImageIndex","datapoint|double" }; }
        }

    }
}
