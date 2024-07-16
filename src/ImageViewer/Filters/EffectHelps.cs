using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using MathHelpLib;
using MathHelpLib.ImageProcessing;
namespace ImageViewer.Filters
{
    public class EffectHelps
    {

        

        /// <summary>
        /// Deletes all files inside a folder, and the folder
        /// </summary>
        /// <param name="Folder"></param>
        public static void ClearTempFolder(string Folder)
        {
            Directory.Delete(Folder, true);
        }

        /// <summary>
        /// Converts values returned from python into a double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ConvertToDouble(object value)
        {
            if (value.GetType() == typeof(int))
                return (double)(int)value;
            if (value.GetType() == typeof(Single))
                return (double)(Single)value;
            if (value.GetType() == typeof(double))
                return (double)value;
            if (value.GetType() == typeof(decimal))
                return (double)(decimal)value;
            if (value.GetType() == typeof(string))
            {
                double d;
                double.TryParse((string)value, out d);
                return d;
            }
            return (double)(ValueType)value;
        }

        /// <summary>
        /// Converts values returned from python into a double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int  ConvertToInt(object value)
        {
            if (value.GetType() == typeof(int))
                return (int)value;
            if (value.GetType() == typeof(Single))
                return (int)(Single)value;
            if (value.GetType() == typeof(double))
                return (int )value;
            if (value.GetType() == typeof(decimal))
                return (int )(decimal)value;
            if (value.GetType() == typeof(string))
            {
                int  d;
                int .TryParse((string)value, out d);
                return d;
            }
            return (int )(ValueType)value;
        }

        private static object CriticalSectionLock_convert = new object();

        /// <summary>
        /// Converts a number of types into a bitmap
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns></returns>
        public static Bitmap ConvertToBitmap(object SourceImage)
        {
            // lock (CriticalSectionLock_convert)
            {
                if (SourceImage.GetType() == typeof(Bitmap))
                {
                    return (Bitmap)SourceImage;
                }
                else if (SourceImage.GetType() == typeof(ImageHolder))
                {
                    return ((ImageHolder)SourceImage).ToBitmap();
                }
                else if (SourceImage.GetType() == typeof(double[,]))
                {
                    return  ((double[,])SourceImage).MakeBitmap();
                }
                else if (SourceImage.GetType() == typeof(float [,]))
                {
                  //  return ImagingTools.ConvertToBitmap((float[,])SourceImage);
                }

                return null;
            }
        }

        private static object CriticalSectionLock_FixImage = new object();

        /// <summary>
        /// Converts the incoming datatype to a imageholder
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns></returns>
        public static ImageHolder FixImageFormat(object SourceImage)
        {

            if (SourceImage != null)
            {
                //   lock (CriticalSectionLock_FixImage)
                {
                    if (SourceImage.GetType() == typeof(Bitmap))
                    {
                        return new ImageHolder((Bitmap)SourceImage);
                    }
                    else if (SourceImage.GetType() == typeof(ImageHolder))
                    {
                        return (ImageHolder)SourceImage;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Used to make the parameter list pretty and take the info provided and turn it into a python script
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        private static string FormatParamList(IEffect effect, object[] Parameters)
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

        private static string DescribeParameters(IEffect effect)
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

        public static string FormatMacroString(IEffect effect, object[] Parameters)
        {
            string outstring = "#" + effect.EffectName + "\n";
            outstring += "Filter =" + effect.GetType().ToString().Replace("ImageViewer.", "") + "()\n";
            bool hasParams = (Parameters != null);

            if (hasParams)
            {
                outstring += "#Parameters required: " + DescribeParameters(effect) + "\n";
                outstring += "BitmapImage=Filter.DoEffect(dataEnvironment,BitmapImage, PassData, " + FormatParamList(effect, Parameters) + ")\n";
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

        public static string[] SortNumberedFiles(string[] UnsortedFilenames)
        {
            return  MathStringHelps. SortNumberedFiles(UnsortedFilenames);
        }


        private static object StaticXMLBlock = new object();
        /// <summary>
        /// gets the requests tages from an xml document
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="NodePaths">the needed nodes</param>
        /// <returns></returns>
        public static Dictionary<string, string> OpenXMLAndGetTags(string Filename, string[] NodePaths)
        {
            lock (StaticXMLBlock)
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(Filename);

                Dictionary<string, string> values = new Dictionary<string, string>();
                for (int i = 0; i < NodePaths.Length; i++)
                {
                    XmlNodeList nodes = xmlDoc.SelectNodes(NodePaths[i]);
                    if (nodes.Count == 0)
                        nodes = xmlDoc.GetElementsByTagName(NodePaths[i]);

                    if (nodes.Count > 0)
                    {
                        XmlNode node = nodes[0];
                        if (node.Value != null)
                        {
                            Console.WriteLine(node.Name + ", " + node.Value.ToString());
                            values.Add(NodePaths[i], node.Value.ToString());
                        }
                        else
                        {
                            try
                            {
                                Console.WriteLine(node.Name + ", " + node.InnerText .ToString());
                                values.Add(NodePaths[i], node.InnerText .ToString());
                            }
                            catch { }
                        }
                    }
                }

                return values;
            }
        }


       
    }
}
