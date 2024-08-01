
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;//ToolboxItem
using System.Drawing;       //ToolboxBitmap
using IronPython.Runtime;   //PythonDictionary
using IronPython.Hosting;   //PythonEngine
using Microsoft.Scripting;  //ScriptDomainManager
using Microsoft.Scripting.Hosting;
using IronPythonEditor.Console.Paths;
using IronPythonEditor.Console.Utils;


namespace IronPythonEditor.Console
{
   
    namespace Utils
    {
        /// <summary>
        /// 
        /// </summary>
        public class Converts
        {
            /// <summary>
            /// Custom MessageBox call. Excepts some random objects from IronPython and converts to string.
            /// </summary>
            /// <param name="inobject">Output object from IronPython.</param>
            public static void MessageBoxIronPy(Object inobject)
            {
                Type itstype = inobject.GetType();

                switch (itstype.FullName)
                {
                    case "IronPython.Runtime.PythonDictionary":
                        PythonDictionary IPDict = new PythonDictionary();
                        IPDict = (PythonDictionary)inobject;
                        MessageBox.Show(IPDict.ToString());
                        break;
                    case "IronPython.Runtime.List":
                        PythonList IPList = new PythonList();
                        IPList = (PythonList)inobject;
                        MessageBox.Show(IPList.ToString());
                        break;
                    case "System.String":
                        MessageBox.Show(inobject.ToString());
                        break;
                    case "System.Int32":
                        MessageBox.Show(Convert.ToString(inobject));
                        break;
                    case "System.Collections.Specialized.StringCollection":
                        StringCollection IPSC = new StringCollection();
                        IPSC = (StringCollection)inobject;
                        StringEnumerator SCE = IPSC.GetEnumerator();
                        string output = "";
                        while (SCE.MoveNext())
                            output += SCE.Current.ToString();
                        MessageBox.Show(output);
                        break;
                    default:
                        MessageBox.Show(inobject.GetType().ToString() + " not yet implemented.");
                        break;
                }
            }
        }
    }

}