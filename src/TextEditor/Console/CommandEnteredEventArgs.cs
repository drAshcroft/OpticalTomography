
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
   

    /// <summary>
    /// Command argument class.
    /// </summary>
    public class CommandEnteredEventArgs : EventArgs
    {
        string command;
        public CommandEnteredEventArgs(string command)
        {
            this.command = command;
        }

        public string Command
        {
            get { return command; }
        }
    }


}