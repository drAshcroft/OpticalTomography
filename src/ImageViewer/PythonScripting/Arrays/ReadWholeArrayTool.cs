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

namespace ImageViewer.PythonScripting.Arrays
{
    public class ReadWholeArrayTool : aEffectNoForm
    {
        public override string EffectName { get { return "Read Whole Array"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Arrays"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 15;
            }
        }



        public override bool PassesPassData
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Pulls data from the global array into the specific thread
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mPassData = PassData;
            mFilterToken = Parameters;
            if (mFilterToken == null)
            {
                mFilterToken = DefaultProperties;
                string ArrayName = "DefaultArray";
                InputBox.ShowDialog("Read array", "Enter name of array", ref ArrayName);
                mFilterToken[0] = ArrayName;
            }

            string aName = (string)mFilterToken[0];


            mPassData.AddSafe("WholeArray", CreateGlobalArrayTool.GetGlobalArray(aName,dataEnvironment ).CopyOutArray );
            return SourceImage;
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { "DefaultArray" }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "ArrayName|string" }; }
        }

    }
}
