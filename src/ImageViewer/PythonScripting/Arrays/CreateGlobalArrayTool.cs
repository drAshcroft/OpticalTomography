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

namespace ImageViewer.PythonScripting.Arrays
{
    /// <summary>
    /// Allows all the threads to create an array together without steping all over each other
    /// </summary>
    public class CreateGlobalArrayTool : aEffectNoForm
    {
        public override string EffectName { get { return "Create Global Array"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Arrays"; } }
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

        /// <summary>
        /// this is a threadsafe array to allow the threads to deposit information
        /// </summary>
        public class GlobalArrayDescription
        {
            /// <summary>
            /// contains the default filename for this array
            /// </summary>
            public string Filename;
            private double[] Data;

            private object CriticalSectionLock = new object();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="Filename">default filename</param>
            /// <param name="Data">data, already dimensioned correctly</param>
            public GlobalArrayDescription(string Filename, double[] Data)
            {
                this.Filename = Filename;
                this.Data = Data;
            }

            /// <summary>
            /// pull the current state of the data item
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public double this[int index]
            {
                get
                {

                    lock (CriticalSectionLock)
                    {
                        return Data[index];
                    }
                }
                set
                {
                    lock (CriticalSectionLock)
                    {
                        Data[index] = value;
                    }
                }
            }

            /// <summary>
            /// pull the current state of the entire array
            /// </summary>
            public double[] CopyOutArray
            {
                get
                {
                    lock (CriticalSectionLock)
                    {
                        double[] CopyOutArray = new double[Data.Length];
                        Buffer.BlockCopy(Data, 0, CopyOutArray, 0, Buffer.ByteLength(Data));
                        return CopyOutArray;
                    }
                }
            }

            /// <summary>
            /// set the current state of the entire array
            /// </summary>
            public double[] CopyInArray
            {
                set
                {
                    lock (CriticalSectionLock)
                    {
                        Data = new double[value.Length];
                        Buffer.BlockCopy(value,0,  Data, 0, Buffer.ByteLength(Data));
                    }
                }
            }
        }



        private static object CriticalSectionLock = new object();


        public class GlobalArrayTokens : IEffectToken
        {
            public string TokenName()
            {
                return this.ToString();
            }

            /// <summary>
            /// A dictionary of all the global arrays and their keys
            /// </summary>
            public DictionaryThreadSafe<string, GlobalArrayDescription> ScriptGlobalArrays = new DictionaryThreadSafe<string, GlobalArrayDescription>();
        }


        public static bool GlobalArrayCreated(string arrayName, DataEnvironment dataEnvironment)
        {
            if (dataEnvironment.EffectTokens.ContainsKey("CreateGlobalArray") == true)
            {
                return ((ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GlobalArrayTokens)dataEnvironment.EffectTokens["CreateGlobalArray"]).ScriptGlobalArrays.ContainsKey(arrayName);
            }
            else
                return false;
        }

        public static GlobalArrayDescription GetGlobalArray(string arrayName, DataEnvironment dataEnvironment)
        {

            return ((ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GlobalArrayTokens)dataEnvironment.EffectTokens["CreateGlobalArray"]).ScriptGlobalArrays[arrayName];
        }

        public static void AddSafe_Array(string arrayName, DataEnvironment dataEnvironment, GlobalArrayDescription gad)
        {
            ((ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GlobalArrayTokens)dataEnvironment.EffectTokens["CreateGlobalArray"]).ScriptGlobalArrays.AddSafe(arrayName, gad);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">arrayname as string;defualt filename as string; number of datapoints </param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {

            mFilterToken = Parameters;
            mDataEnvironment = dataEnvironment;
            if (mFilterToken == null)
            {
                mFilterToken = DefaultProperties;
                string ArrayName = "DefaultArray";
                InputBox.ShowDialog("Create Array", "Enter name of array", ref ArrayName);
                mFilterToken[0] = ArrayName;
            }

            //default values for the rest of the array
            if (mFilterToken.Length < 2)
            {
                object[] pars = new object[3];
                pars[0] = mFilterToken[0];
                ///default filename
                pars[1] = mFilterToken[0];

                pars[2] = 500;
                mFilterToken = pars;
            }

            ///create the array with the correct key and the correct number of spaces
            ///the list and dictionary are not threadsafe, so this is the only method.
            lock (CriticalSectionLock)
            {
                GlobalArrayTokens gat;
                if (mDataEnvironment.EffectTokens.ContainsKey("CreateGlobalArray") == true)
                    gat = (GlobalArrayTokens)mDataEnvironment.EffectTokens["CreateGlobalArray"];
                else
                {
                    gat = new GlobalArrayTokens();
                    mDataEnvironment.EffectTokens.Add("CreateGlobalArray", gat);
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

                if (gat.ScriptGlobalArrays.ContainsKey(aName) == true)
                {
                    return SourceImage;
                }
                else
                {
                    double[] tList = new double[(int)mFilterToken[2]];
                    gat.ScriptGlobalArrays.Add(aName, new GlobalArrayDescription(afilePath, tList));

                    return SourceImage;
                }
            }
        }
        public override object[] DefaultProperties
        {
            get { return new object[] { "DefaultArray", "" }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Array_Name|string", "Array_Filename|string", "Array_Size|int" }; }
        }
    }
}
