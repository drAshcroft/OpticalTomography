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
    public class SaveWholeArrayTool : aEffectNoForm
    {
        public override string EffectName { get { return "Save Whole Array"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Arrays"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 20;
            }
        }

       

        public override bool PassesPassData
        {
            get
            {
                return false ;
            }
        }

        /// <summary>
        /// Write global array to a file
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">arrayname as string; filename as string</param>
        /// <returns></returns>
        public override object  DoEffect(DataEnvironment dataEnvironment, object SourceImage, 
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            mDataEnvironment = dataEnvironment;
            string Filename="";
            if (mFilterToken == null)
            {
                mFilterToken = DefaultProperties;
                string ArrayName = "DefaultArray";
                InputBox.ShowDialog("Save array", "Enter name of array", ref ArrayName);
                Filename = ArrayName;
                InputBox.ShowDialog("Save Array", "Enter Filename", ref Filename);
                mFilterToken[0] = ArrayName;
                mFilterToken[1] = Filename;
            }

            Filename =(string) mFilterToken[1];

            string aName = (string)mFilterToken[0];

          

            //try
            {
                double[] listData = CreateGlobalArrayTool.GetGlobalArray(aName,dataEnvironment ).CopyOutArray;
                if (Filename.Substring(1, 1).ToLower() != ":")
                    Filename = Path.GetDirectoryName(Application.ExecutablePath) + "\\Data\\" + Filename;


                // Write the string to a file.
                System.IO.StreamWriter file = new System.IO.StreamWriter(Filename);
                for (int i = 0; i < listData.Length; i++)
                    file.WriteLine(listData[i]);

                file.Close();
            }
            //catch { }
            return SourceImage;
        }

        public override object[] DefaultProperties
        {

            get {  return new object[] 
                {"DefaultArray","DefaultArray"}; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }

      
    }
}

 