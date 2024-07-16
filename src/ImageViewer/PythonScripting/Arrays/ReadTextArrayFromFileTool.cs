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
    /// <summary>
    /// this loads a array from a text file and makes it accesible to all threads
    /// 
    /// </summary>
    public class ReadTestArrayFromFileTool : aEffectNoForm
    {
        public override string EffectName { get { return "Read Text Array from File"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Arrays"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 25;
            }
        }



        public override bool PassesPassData
        {
            get
            {
                return true;
            }
        }

        private static object LockFile = new object();
        /// <summary>
        /// Loads a text file into a global array for all threads
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">array name as string; filename as string</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            mPassData = PassData;
            mDataEnvironment = dataEnvironment;
            string Filename = "";
            string ArrayName = "DefaultArray";
            if (mFilterToken == null)
            {
                mFilterToken = DefaultProperties;

                InputBox.ShowDialog("Open array", "Enter name of array", ref ArrayName);
                Filename = ArrayName;
                InputBox.ShowDialog("Open Array", "Enter Filename", ref Filename);
                mFilterToken[0] = ArrayName;
                mFilterToken[1] = Filename;
            }

            Filename = (string)mFilterToken[1];
            string aName = (string)mFilterToken[0];

            try
            {
                if (CreateGlobalArrayTool.GlobalArrayCreated(aName,dataEnvironment)==false )
                {
                    CreateGlobalArrayTool cga = new CreateGlobalArrayTool();
                    cga.DoEffect(dataEnvironment, SourceImage, PassData, aName, "");
                }
            }
            catch { }

            if (Filename == null || Filename == "" || Filename.Substring(1, 1).ToLower() != ":")
            {
                string tFilename = Path.GetDirectoryName(Application.ExecutablePath) + "\\Data\\" + Filename;
                if (File.Exists(tFilename) == false)
                {
                    tFilename = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\" + Filename;
                }
                Filename = tFilename;

            }


            lock (LockFile)
            {
                if (File.Exists(Filename) == false)
                {
                    throw new Exception("Cannot Find file");
                }

                List<double> listData = new List<double>();
                using (StreamReader sr = new StreamReader(Filename))
                {
                    String line;
                    int i = 0;
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        double dataPoint;
                        double.TryParse(line, out dataPoint);
                        listData.Add(dataPoint);
                        i++;
                    }
                }

                CreateGlobalArrayTool.AddSafe_Array(aName, dataEnvironment,
                    new ImageViewer.PythonScripting.Arrays.CreateGlobalArrayTool.GlobalArrayDescription(Filename, listData.ToArray()));

                mPassData.AddSafe("Array", listData.ToArray());
            }

            return SourceImage;
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { "DefaultArray", "Filename" }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "ArrayName|string", "ArrayFileName|string" }; }
        }

    }
}
