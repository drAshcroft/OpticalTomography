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
    public class ReadWholeAverageTool : aEffectNoForm
    {
        public override string EffectName { get { return "Read Whole Array"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Average Images"; } }

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
        public override string PassDataDescription
        {
            get
            {
                return "Returns array double[,]|| PassData['WholeAverageArray']";
            }
        }

        /// <summary>
        /// pulls the image from the global averages
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">global average name as string; </param>
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


            //try
            {
                double[,] data = CreateGlobalAverageTool.GetGlobalArray(aName, dataEnvironment).Data;// ScriptGlobalAverages[aName].Data;
                double[,] data2 = new double[data.GetLength(0), data.GetLength(1)];
                double cc = CreateGlobalAverageTool.GetGlobalArray(aName, dataEnvironment).PointCount;
                double sum=0;
                for (int i = 0; i < data.GetLength(0); i++)
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        data2[i, j] = data[i, j] / cc;
                        sum += (data2[i, j]);
                    }
                sum = sum / (data.Length );
                mPassData.AddSafe("WholeAverageArray", data2 );
                mPassData.AddSafe("WholeAverageValue", sum);
                return new ImageHolder(  MathHelpLib.ImageProcessing.MathImageHelps  .ConvertToBitmap(data2));
            }
           // catch { }
          //  return SourceImage;
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { "DefaultArray"}; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "ArrayName|string" }; }
        }

    }
}
