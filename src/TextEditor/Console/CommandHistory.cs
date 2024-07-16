
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
    internal class CommandHistory
    {
        private int currentPosn;
        private string lastCommand;
        private ArrayList commandHistory = new ArrayList();

        internal CommandHistory()
        {
        }

        internal void Add(string command)
        {
            if (command != lastCommand)
            {
                commandHistory.Add(command);
                lastCommand = command;
                currentPosn = commandHistory.Count;
            }
        }

        internal bool DoesPreviousCommandExist()
        {
            return currentPosn > 0;
        }

        internal bool DoesNextCommandExist()
        {
            return currentPosn < commandHistory.Count - 1;
        }

        internal string GetPreviousCommand()
        {
            lastCommand = (string)commandHistory[--currentPosn];
            return lastCommand;
        }

        internal string GetNextCommand()
        {
            lastCommand = (string)commandHistory[++currentPosn];
            return LastCommand;
        }

        internal string LastCommand
        {
            get { return lastCommand; }
        }

        internal string[] GetCommandHistory()
        {
            return (string[])commandHistory.ToArray(typeof(string));
        }
    }
}
