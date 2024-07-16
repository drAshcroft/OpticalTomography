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
using MathHelpLib;

namespace ImageViewer.PythonScripting.AverageImage
{
    public class CreateGlobalAverageTool : aEffectNoForm
    {
        public override string EffectName { get { return "Create Global Average"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Average Images"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 1;
            }
        }

        public override bool PassesPassData
        {
            get
            {
                return false;
            }
        }

        public class GlobalAverageDescription
        {
            /// <summary>
            /// contains the bounds that are used for this average from the main image, if bounds are performed
            /// </summary>
            public Rectangle Bounds;
            public string Filename;
            public double[,] Data;
            public object[] Locks;
            public int PointCount;
            public GlobalAverageDescription(string Filename, double[,] Data)
            {
                this.Filename = Filename;
                this.Data = Data;
            }
        }


        public static bool GlobalAverageCreated(string arrayName, DataEnvironment dataEnvironment)
        {
            if (dataEnvironment.EffectTokens.ContainsKey("CreateGlobalAverage") == true)
            {
                return ((GlobalAverageTokens)dataEnvironment.EffectTokens["CreateGlobalAverage"]).ScriptGlobalAverages.ContainsKey(arrayName);
            }
            else
                return false;
        }

        public static GlobalAverageDescription GetGlobalArray(string arrayName, DataEnvironment dataEnvironment)
        {

            return ((GlobalAverageTokens)dataEnvironment.EffectTokens["CreateGlobalAverage"]).ScriptGlobalAverages[arrayName];
        }

        public static void AddSafe_Array(string arrayName, DataEnvironment dataEnvironment, GlobalAverageDescription gad)
        {
            ((GlobalAverageTokens)dataEnvironment.EffectTokens["CreateGlobalAverage"]).ScriptGlobalAverages.AddSafe(arrayName, gad);

        }


        public class GlobalAverageTokens : IEffectToken
        {
            public string TokenName()
            {
                return this.ToString();
            }

            /// <summary>
            /// A dictionary of all the global arrays and their keys
            /// </summary>
            public DictionaryThreadSafe<string, GlobalAverageDescription> ScriptGlobalAverages = new DictionaryThreadSafe<string, GlobalAverageDescription>();
        }

        /// <summary>
        /// contains the averages for each names global average
        /// </summary>
       // public static Dictionary<string, GlobalAverageDescription> ScriptGlobalAverages;

        private static object CriticalSectionLock = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">name of average; defaultfilename; total number of images</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mDataEnvironment = dataEnvironment;
            mFilterToken = Parameters;
            if (mFilterToken == null)
            {
                mFilterToken = DefaultProperties;
                string ArrayName = "DefaultAverage";
                InputBox.ShowDialog("Create Average", "Enter name of average", ref ArrayName);
                mFilterToken[0] = ArrayName;
            }

            if (mFilterToken.Length < 2)
            {
                object[] pars = new object[2];
                pars[0] = mFilterToken[0];
                pars[1] = mFilterToken[0];
                mFilterToken = pars;
            }

            lock (CriticalSectionLock)
            {
                GlobalAverageTokens gat;
                if (mDataEnvironment.EffectTokens.ContainsKey("CreateGlobalAverage") == true)
                    gat = (GlobalAverageTokens)mDataEnvironment.EffectTokens["CreateGlobalAverage"];
                else
                {
                    gat = new GlobalAverageTokens();
                    mDataEnvironment.EffectTokens.Add("CreateGlobalAverage", gat);
                }


                string aName = (string)mFilterToken[0];
                string afilePath = (string)mFilterToken[1];

                if (afilePath.Trim() == "" || afilePath == aName)
                {
                    afilePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Data\\" + aName;
                    if (Directory.Exists(Path.GetDirectoryName(afilePath)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(afilePath));
                    }
                }

                if (gat.ScriptGlobalAverages.ContainsKey(aName) == true)
                {
                    return SourceImage;
                }
                else
                {
                    double[,] tList = null;
                    if (mFilterToken.Length > 2 && mFilterToken[2] != null)
                    {
                        tList = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(mFilterToken[2], false);
                        GlobalAverageDescription gad = new GlobalAverageDescription(afilePath, tList);

                        gad.Locks = new object[tList.GetLength(0)];
                        for (int i = 0; i < gad.Locks.Length; i++)
                            gad.Locks[i] = new object();

                        gad.PointCount = 1;
                        gat. ScriptGlobalAverages.Add(aName, gad);
                    }
                    else
                        gat.ScriptGlobalAverages.Add(aName, new GlobalAverageDescription(afilePath, tList));

                    return SourceImage;
                }
            }
        }
        public override object[] DefaultProperties
        {
            get { return new object[] { "DefaultArray", "", null }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Array_Name|string", "Array_Filename|string" }; }
        }
    }
}
