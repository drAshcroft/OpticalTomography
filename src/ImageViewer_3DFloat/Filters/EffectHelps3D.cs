using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.IO;

namespace ImageViewer3D.Filters
{
    public class EffectHelps3D
    {
      
        private static string FormatParamList(IEffect3D effect, object[] Parameters)
        {
            List<string> ParamList = new List<string>();
            for (int i = 0; i < Parameters.Length; i++)
            {
                string PValue;
                Type t;
                if (Parameters[i] == null)
                {
                    PValue = "None";
                    t = null;
                }
                else
                {
                    PValue = Parameters[i].ToString();
                    t = Parameters[i].GetType();
                }


                if (t == typeof(double[,]))
                {
                    double[,] pValue = (double[,])Parameters[i];
                    string junk = "[";
                    int m;
                    for (m = 0; m < pValue.GetLength(0); m++)
                    {
                        junk += "[";
                        int n;
                        for (n = 0; n < pValue.GetLength(1) - 1; n++)
                        {
                            junk += pValue[m, n].ToString() + ",";
                        }
                        junk += pValue[m, n].ToString() + "],";
                    }
                    junk = junk.Substring(0, junk.Length - 2) + "]";
                    ParamList.Add(junk);
                }
                else if (t == typeof(Rectangle))
                {
                    Rectangle rect = (Rectangle)Parameters[i];
                    ParamList.Add("Rectangle(" + rect.Left + "," + rect.Top + "," + rect.Width + "," + rect.Height + ")");
                }
                else if (t == typeof(Size))
                {
                    Size rect = (Size)Parameters[i];
                    ParamList.Add("Size(" + rect.Width + "," + rect.Height + ")");
                }
                else if (t == typeof(string))
                {
                    string s = (string)Parameters[i];
                    s = "\"" + s + "\"";
                    s = s.Replace("\\", "\\\\");
                    ParamList.Add(s);
                }
                else
                    ParamList.Add(PValue);
            }
            string outString = "";
            for (int i = 0; i < ParamList.Count - 1; i++)
                outString += ParamList[i].ToString() + ",";
            outString += ParamList[ParamList.Count - 1];
            return outString;
        }

        private static string DescribeParameters(IEffect3D effect)
        {
            string[] ParamDesc = effect.ParameterList;
            string junk = "";
            string[] parts;
            int i = 0;
            try
            {
                for (i = 0; i < ParamDesc.Length - 1; i++)
                {
                    parts = ParamDesc[i].Split('|');
                    junk += parts[0] + " as " + parts[1] + ", ";
                }
            
            parts = ParamDesc[i].Split('|');
            junk += parts[0] + " as " + parts[1];
            }
            catch { }
            return junk;
        }

        public static string FormatMacroString(IEffect3D effect, object[] Parameters)
        {
            string outstring = "#" + effect.EffectName + "\n";
            outstring += "Filter =" + effect.GetType().ToString().Replace("ImageViewer3DFloat.", "") + "()\n";
            bool hasParams = (Parameters != null);
            
            if (hasParams)
            {
                outstring += "#Parameters required: " + DescribeParameters(effect) + "\n";
                outstring += "BitmapImage=Filter.DoEffect(dataEnvironment,BitmapImage, PassData, " + FormatParamList(effect, Parameters) +")\n";
            }
            else
                outstring += "BitmapImage=Filter.DoEffect(dataEnvironment,BitmapImage, PassData)\n";

            if (effect.PassesPassData)
            {
                try
                {
                    outstring += "#ImageData out of type :" + effect.PassDataDescription +
                               "\nPassData = Filter.PassData\n";
                }
                catch
                {
                    outstring += "PassData = Filter.PassData\n";
                }
            }

            return outstring;
        }

    }
}
