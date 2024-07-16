//---------------------------------------------------------------------------------
//IronTextBox.cs - version 2.0.2.0b
// TextBox control based class designed to be used with Microsoft's IronPython.
// Maybe useful for testing Python scripts with IronPython. 
//WHAT'S NEW: 
//      -Updated License from GNU to Expat/MIT
//      -Tested with IronPython 2.03B
//TO DO:
//      -Fix raw_input support: "s = raw_input('--> ')"
//      -Multiple arg support for "paths" command. eg. "paths -misc -python24"
//      -Intellisense ToolTip
//
//BY DOWNLOADING AND USING, YOU AGREE TO THE FOLLOWING TERMS:
//Copyright (c) 2006-2008 by Joseph P. Socoloski III
//LICENSE
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//the MIT License, given here: <http://www.opensource.org/licenses/mit-license.php> 
//---------------------------------------------------------------------------------

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
using IronPythonEditor.TextEditor;


namespace IronPythonEditor.Console
{
   
    /// <summary>
    /// Summary description for IronTextBoxControl.
    /// </summary>
    public class IronTextBoxControl : UserControl
    {
        #region IronTextBoxControl members
        /// <summary>
        /// Main IronPython ScriptEngine
        /// </summary>
        public ScriptEngine engine;

        /// <summary>
        /// Main IronPython ScriptScope
        /// </summary>
        public ScriptScope scope;

        /// <summary>
        /// The IronTextBox member.
        /// </summary>
        private IronTextBox consoleTextBox;

        /// <summary>
        /// The CommandEntered event
        /// </summary>
        public event EventCommandEntered CommandEntered;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        /// <summary>
        /// Adds def lines one by one.
        /// </summary>
        public StringBuilder defBuilder
        {
            get { return consoleTextBox.defStmtBuilder; }
            set
            {
                if (consoleTextBox != null)
                    consoleTextBox.defStmtBuilder = value;
            }
        }

        /// <summary>
        /// Returns the string array of the command history.
        /// </summary>
        /// <returns></returns>
        public string[] GetCommandHistory()
        {
            return consoleTextBox.GetCommandHistory();
        }

        /// <summary>
        /// Gets and sets console text ForeColor. 
        /// </summary>
        public Color ConsoleTextForeColor
        {
            get { return consoleTextBox != null ? consoleTextBox.ForeColor : Color.Black; }
            set
            {
                if (consoleTextBox != null)
                    consoleTextBox.ForeColor = value;
            }
        }

        /// <summary>
        /// Gets and sets console text BackColor. 
        /// </summary>
        public Color ConsoleTextBackColor
        {
            get { return consoleTextBox != null ? consoleTextBox.BackColor : Color.White; }
            set
            {
                if (consoleTextBox != null)
                    consoleTextBox.BackColor = value;
            }
        }

        /// <summary>
        /// Gets and sets console Font. 
        /// </summary>
        public Font ConsoleTextFont
        {
            get { return consoleTextBox != null ? consoleTextBox.Font : new Font("Lucida Console", 8); }
            set
            {
                if (consoleTextBox != null)
                    consoleTextBox.Font = value;
            }
        }

        /// <summary>
        /// Gets and sets string to be used for the Prompt.
        /// </summary>
        public string Prompt
        {
            get { return consoleTextBox.Prompt; }
            set { consoleTextBox.Prompt = value; }
        }
        #endregion IronTextBoxControl members

        /// <summary>
        /// IronTextBoxControl
        /// </summary>
        public IronTextBoxControl()
        {
            InitializeComponent();

            //Create the ScriptRuntime
            engine = Python.CreateEngine();
            //Create the scope for the ScriptEngine
            scope = engine.CreateScope();

            //IronTextBox's CommandEntered event
            CommandEntered += new IronPythonEditor.Console.EventCommandEntered(irontextboxControl_CommandEntered);

            consoleTextBox.AutoCompleteListFill += new AutoCompletionListFillEvent(consoleTextBox_AutoCompleteListFill);
        }

        public event AutoCompletionListFillEvent AutoCompletionListFill;
        string[] consoleTextBox_AutoCompleteListFill(string ObjectName)
        {
            if (AutoCompletionListFill != null)
                return AutoCompletionListFill(ObjectName);
            else
                return null;
        }

        /// <summary>
        /// Executes the Python file within the IronTextBox environment.
        /// A nice way to quickly get a Python module in CLI to test or use.
        /// </summary>
        /// <param name="pyfile">Python file (.py)</param>
        /// <returns>object</returns>
        object DoIPExecuteFile(string pyfile)
        {
            ScriptSource source = engine.CreateScriptSourceFromFile(pyfile);
            return source.Execute(scope);
        }

        /// <summary>
        /// Executes the code in SourceCodeKind.SingleStatement to fire the command event
        /// Use DoIPEvaluate if you do not wish to fire the command event
        /// </summary>
        /// <param name="pycode">python statement</param>
        /// <returns>object</returns>
        object DoIPExecute(string pycode)
        {
            ScriptSource source = engine.CreateScriptSourceFromString(pycode, SourceCodeKind.SingleStatement);
            return source.Execute(scope);
        }

        /// <summary>
        /// Executes the code in SourceCodeKind.Expression not to fire the command event
        /// Use DoIPExecute if you do wish to fire the command event
        /// </summary>
        /// <param name="pycode">Python expression</param>
        /// <returns>object</returns>
        object DoIPEvaluate(string pycode)
        {
            ScriptSource source = engine.CreateScriptSourceFromString(pycode, SourceCodeKind.Expression);
            return source.Execute(scope);
        }

        /// <summary>
        /// irontextboxControl_CommandEntered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void irontextboxControl_CommandEntered(object sender, IronPythonEditor.Console.CommandEnteredEventArgs e)
        {
            string command = e.Command.TrimEnd();

            engine.Runtime.IO.SetOutput(new IPEStreamWrapper(), engine.Runtime.IO.InputEncoding);

            //Create a temp object to use
           // object tempobject;

            //Begin IronTextBox evaluation if something there....
            if (command != "")
            {
                if (command == "cls")
                    this.Clear();
                else if (command == "history")
                {
                    string[] commands = this.GetCommandHistory();
                    StringBuilder stringBuilder = new StringBuilder(commands.Length);
                    foreach (string s in commands)
                    {
                        stringBuilder.Append(s);
                        stringBuilder.Append(System.Environment.NewLine);
                    }
                    this.WriteText(stringBuilder.ToString());
                }
                else if (command == "help")
                {
                    this.WriteText(GetHelpText());
                }
                else if (command == "newconsole")
                {
                    //consoleTextBox.global_eng = new PythonEngine();
                    this.WriteText("Not currently supported\r\n");
                }
                else if (command.StartsWith("prompt") && command.Length == 6)
                {
                    string[] parts = command.Split(new char[] { '=' });
                    if (parts.Length == 2 && parts[0].Trim() == "prompt")
                        this.Prompt = parts[1].Trim();
                }
                else if (command == "btaf")
                {
                    //btaf = Browse To Append File....
                    //Check to see if sys is loaded
                    if (!DoIPEvaluate("dir()").ToString().Contains("sys"))
                    {
                        consoleTextBox.printPrompt();
                        consoleTextBox.WriteText("import sys");
                        this.SimEnter();
                    }

                    System.Windows.Forms.FolderBrowserDialog ofd = new System.Windows.Forms.FolderBrowserDialog();
                    ofd.SelectedPath = IronPythonEditor.Console.Paths.MiscDirs.vs_Projects;
                    ofd.ShowDialog();
                    consoleTextBox.printPrompt();
                    consoleTextBox.WriteText("sys.path.append(\"" + ofd.SelectedPath + "\")");
                    this.SimEnter();
                }
                else if (command == "runfile")
                {
                    //runfile - Run a .Py file.  Calls OpenFileDialog to PythonEngine.RunFile....
                    //  goodfor debuging .y file within IDE
                    this.Runfile();
                }
                else if (command == "btwfi")
                {
                    //btwfi - Browse To Walk FIle. Calls OpenFileDialog.
                    this.WalkPythonFile();
                }
                else if (command == "rew")
                {
                    //btwfi - Browse To Walk FIle. Calls OpenFileDialog.
                    StringBuilder SBCode = new StringBuilder();
                    this.RewritePyFiletoSB(out SBCode);
                    DoIPExecute(SBCode.ToString()); //transformed object code from a .py
                }
                else if (command.StartsWith("paths"))
                {
                    //Appends all hardcoded common paths stored in UIIronTextBox.Paths
                    //paths [-arg] - [args: -misc, -python24, -ironpython, -all] (-all=default)
                    if (command.Contains(" -"))
                    {
                        string[] splitcommand = command.Split('-');
                        splitcommand[1] = splitcommand[1].Trim();
                        this.ImportPaths(splitcommand[1]);
                    }
                    else
                        this.ImportPaths(command.Trim());

                }
                else if (command.TrimEnd().EndsWith(":") == true)
                {
                    //Need to do a ReadStatement...
                    try
                    {
                        bool isMultiLine = false;
                        int autoIndentSize = 0;
                        int numberOfBlankLines = 0;
                        object ExecWrapper = null;
                        bool result;

                        string line = command;
                        if (line == null)
                        {
                            if (ExecWrapper != null)
                            {
                                //Ops.Call(ExecWrapper, new object[] { null });//not needed for IP2?
                            }
                            result = false;
                        }

                        defBuilder.Append(line);
                        defBuilder.Append("\r\n");

                        bool endOfInput = (line.Length == 0);
                        bool parsingMultiLineString;
                        bool parsingMultiLineCmpdStmt;

                        //old//s = ParsetheText(consoleTextBox.global_eng.Sys, new CompilerContext(), defBuilder.ToString(), endOfInput, out parsingMultiLineString, out isMultiLine);
                        string[] seperators = new string[] { "\r" };
                        string[] allpieces = defBuilder.ToString().Split(seperators, StringSplitOptions.None);

                        if (/*Options.AutoIndentSize != 0 &&*/ line.Trim().Length == 0)
                            numberOfBlankLines++;
                        else
                            numberOfBlankLines = 0;


                        if (allpieces.Length > 1)
                        {
                            // Note that splitting a string literal over multiple lines does not 
                            // constitute a multi-line statement.
                            isMultiLine = true;
                        }

                        //autoIndentSize = Parser.GetNextAutoIndentSize(defBuilder.ToString(), autoIndentSize);//Not needed in IP2?
                        result = true;
                    }
                    catch
                    {
                    }
                }

                else //misc commands...
                {
                    try
                    {
                        DoIPExecute(command);
                        this.WriteText(Environment.NewLine + IPEStreamWrapper.sbOutput.ToString());
                        IPEStreamWrapper.sbOutput = "";
                        //added to fix "rearviewmirror" (IPEStreamWrapper.sbOutput not clearing) bug.
                        //IPEStreamWrapper.sbOutput.Remove(0, IPEStreamWrapper.sbOutput.Length);        //Clear
                    }

                    catch (Exception err)//catch any errors
                    {
                        this.WriteText("\r\nIronTextBoxControl error: " + err.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Displays information about IronTextBox and user's IronPython version.
        /// </summary>
        /// <returns>Returns string information about IronTextBox and user's IronPython version.</returns>
        public string GetHelpText()
        {
            string helpText;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("*******************************************");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("**   IronTextBox version 2.0.2.0b Help   **");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("*******************************************");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("You are using " + engine.LanguageVersion + " (" + engine.LanguageVersion + ")");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("Commands Available:");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("(1) prompt - Changes prompt. Usage: prompt=<desired_prompt>");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("(2) history - prints history of entered commands.");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("(3) cls - Clears the screen.");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("(4) newconsole - Clears the current PythonEngine.");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("(5) btaf - Browse To Append Folder. Calls FolderBrowserDialog.");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("(6) btwfi - Browse To Walk FIle. Calls OpenFileDialog.");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("(7) paths [-arg] - [args: -misc, -python24, -ironpython, -all] (-all=default)");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("(8) rew - Re-Write a Python file into a StringBuilder.(for testing)");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("(9) runfile - Run a .Py file.  Calls OpenFileDialog to PythonEngine.RunFile.");
            stringBuilder.Append(System.Environment.NewLine);
            helpText = stringBuilder.ToString();
            return helpText;
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.consoleTextBox = new IronPythonEditor.Console.IronTextBox();
            this.SuspendLayout();
            // 
            // consoleTextBox
            // 
            //	this.consoleTextBox.AcceptsReturn = true;
            this.consoleTextBox.AcceptsTab = true;
            this.consoleTextBox.BackColor = Color.White;
            this.consoleTextBox.Dock = DockStyle.Fill;
            this.consoleTextBox.Location = new Point(0, 0);
            this.consoleTextBox.Multiline = true;
            this.consoleTextBox.Name = "consoleTextBox";
            this.consoleTextBox.Prompt = ">>>";
            this.consoleTextBox.ScrollBars = ScrollBars.Both; //for TextBox use
           // this.consoleTextBox.ScrollBars = RichTextBoxScrollBars.Both; //for RichTextBox use
            this.consoleTextBox.Font = new Font("Lucida Console", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(0)));
            this.consoleTextBox.Size = new Size(232, 216);
            this.consoleTextBox.TabIndex = 0;
            this.consoleTextBox.Text = "";
            // 
            // IronTextBoxControl
            // 
            this.Controls.Add(this.consoleTextBox);
            this.Name = "IronTextBoxControl";
            this.Size = new Size(232, 216);
            this.ResumeLayout(false);

        }
        #endregion

        #region Overides
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion Overides

        /// <summary>
        /// Run the command.
        /// </summary>
        /// <param name="command">Command line string.</param>
        internal void FireCommandEntered(string command)
        {
            OnCommandEntered(command);
        }

        /// <summary>
        /// Creates new EventCommandEntered event.
        /// </summary>
        /// <param name="command">Command line string.</param>
        protected virtual void OnCommandEntered(string command)
        {
            if (CommandEntered != null)
                CommandEntered(command, new CommandEnteredEventArgs(command));
        }

        /// <summary>
        /// Clear the current text in the IronTextBox.
        /// </summary>
        public void Clear()
        {
            consoleTextBox.Clear();
        }

        /// <summary>
        /// Send text to the IronTextBox.
        /// </summary>
        /// <param name="text"></param>
        public void WriteText(string text)
        {
            consoleTextBox.WriteText(text);
        }

        /// <summary>
        /// Send text to the IronTextBox.
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text)
        {
            if (consoleTextBox.LastCharactor() == '>')
                consoleTextBox.printLine();
            consoleTextBox.WriteText(text);
            consoleTextBox.printLine();

        }

        /// <summary>
        /// Simulate the Enter KeyPress event.
        /// </summary>
        public void SimEnter()
        {
            string currentCommand = consoleTextBox.GetTextAtPrompt();
            consoleTextBox.Focus();

            //Optional: add the command to the stringcollection
            //consoleTextBox.scollection.Add(currentCommand);
            ///////////////////////////////////////////////////

            //If it is not an empty command, then "fire" the command
            if (currentCommand.Length != 0)
            {
                //consoleTextBox.printLine();
                ((IronPythonEditor.Console.IronTextBoxControl)consoleTextBox.Parent).FireCommandEntered(currentCommand);
                consoleTextBox.AddcommandHistory(currentCommand);
            }
            else
            {
                //if it is an empty command let's see if we just finished a def statement
                if (consoleTextBox.defStmtBuilder.Length != 0)
                {
                    ((IronPythonEditor.Console.IronTextBoxControl)consoleTextBox.Parent).FireCommandEntered(consoleTextBox.defStmtBuilder.ToString());
                    consoleTextBox.AddcommandHistory(consoleTextBox.defStmtBuilder.ToString());

                    //we just finished a def so clear the defbuilder
                    consoleTextBox.defStmtBuilder = consoleTextBox.defStmtBuilder.Remove(0, consoleTextBox.defStmtBuilder.Length);
                }
            }
            consoleTextBox.printPrompt();
        }

        /// <summary>
        /// Opens a Python files and reads line by line into IronTextBox.
        /// </summary>
        /// <param name="fullpathfilename">fullpathfilename</param>
        public void WalkPythonFile(string fullpathfilename)
        {
            try
            {
                string filetext = File.ReadAllText(fullpathfilename);
                //tabs create a problem when trying to remove comments
                filetext = filetext.Replace("\t", "    ");

                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StringReader sr = new StringReader(filetext))
                {
                    String line;
                    StringBuilder sb = new StringBuilder();
                    int pos = 0;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        //if the line is a # comment line, or a single line do not add...
                        if (!line.StartsWith("#") && !line.StartsWith("    #") && !line.StartsWith("        #") && !line.StartsWith("            #") && !line.Equals("\r\n") && line != "")
                        {
                            //catch """ comments
                            if (line.StartsWith("\"\"\"") || line.StartsWith("    \"\"\"") || line.StartsWith("        \"\"\"") || line.StartsWith("            \"\"\""))
                            {
                                //the line may also end with """, so if it is not read until end
                                if (!IsSingleCommentLine(line))
                                {
                                    //get to the end of the comments
                                    while (!sr.ReadLine().TrimEnd().EndsWith("\"\"\""))
                                    {
                                        //do nothing
                                    }
                                }

                                //reassign line
                                line = sr.ReadLine();
                            }

                            //the line may also end with """, so if it is not read until end
                            if (!IsSingleCommentLine(line))
                            {
                                //if the line ends with """, then delete """ and read until end
                                if (line.TrimEnd().EndsWith("\"\"\"") && line.IndexOf("\"\"\"") == line.Length - 3)
                                {
                                    //remove """ and reassign line
                                    line = line.Remove(line.Length - 3);

                                    //get to the end of the comments
                                    while (!sr.ReadLine().TrimEnd().EndsWith("\"\"\""))
                                    {
                                        //do nothing
                                    }
                                }
                                //then append line
                                sb.AppendLine(line);

                                consoleTextBox.WriteText(line);
                                SimEnter();
                            }
                        }


                        pos = sb.Length;
                        //if a blank line, enter previous text as a FuncDef
                        if (line == "" && sb.Length != 0)
                        {
                            try //try to find last ""
                            {
                                //consoleTextBox.WriteText(sb.ToString());
                                SimEnter();
                            }
                            catch
                            {

                            }

                        }
                        //consoleTextBox.WriteText(line);

                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                consoleTextBox.WriteText("The file could not be read:");
                consoleTextBox.WriteText(e.Message);
            }

        }

        /// <summary>
        /// Opens a FolderBrowserDialog to load a Python file to read it line by line into IronTextBox.
        /// </summary>
        public void WalkPythonFile()
        {
            try
            {
                //See if sys is imported...
                if (!DoIPEvaluate("dir()").ToString().Contains("sys"))
                {
                    consoleTextBox.printPrompt();
                    consoleTextBox.WriteText("import sys");
                    this.SimEnter();
                }

                //Browse to the file...
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = IronPythonEditor.Console.Paths.MiscDirs.vs_Projects;
                ofd.Filter = "Python files (*.py)|*.py|All files (*.*)|*.*";
                ofd.ShowDialog();

                //Ask the user if they would like to append the path
                string message = "Do you need to append the folder:\r\n" + Path.GetDirectoryName(Path.GetFullPath(ofd.FileName)) + "\r\n\r\nto the PythonEngine?";
                string caption = "Append Folder Path";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(this, message, caption, buttons);
                if (result == DialogResult.Yes)
                {
                    consoleTextBox.printPrompt();
                    consoleTextBox.WriteText("sys.path.append(\"" + Path.GetDirectoryName(Path.GetFullPath(ofd.FileName)) + "\")");
                    this.SimEnter();

                    //Keep asking until No
                    while (result.Equals(DialogResult.Yes))
                    {
                        //Ask the user if more folders are needed to be appended
                        message = "Do you need to append another folder?";
                        result = MessageBox.Show(this, message, caption, buttons);
                        if (result == DialogResult.Yes)
                        {
                            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                            fbd.SelectedPath = Path.GetDirectoryName(Path.GetFullPath(ofd.FileName));
                            fbd.ShowDialog();
                            consoleTextBox.printPrompt();
                            consoleTextBox.WriteText("sys.path.append(\"" + fbd.SelectedPath + "\")");
                            this.SimEnter();
                        }
                    }
                }

                WalkPythonFile(ofd.FileName);
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                consoleTextBox.WriteText("The file could not be read:");
                consoleTextBox.WriteText(e.Message);
            }
        }

        /// <summary>
        /// Run a .Py file.  Calls OpenFileDialog to PythonEngine.RunFile.
        /// </summary>
        public void Runfile()
        {
            try
            {
                //Browse to the file...
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = IronPythonEditor.Console.Paths.MiscDirs.vs_Projects;
                ofd.Filter = "Python files (*.py)|*.py|All files (*.*)|*.*";
                ofd.ShowDialog();

                DoIPExecuteFile(ofd.FileName);

            }
            catch (Exception ex)
            {
                consoleTextBox.WriteText("The file could not be read:");
                consoleTextBox.WriteText(ex.Message);
            }

        }

        /// <summary>
        /// Opens a Python file and reads line by line into a StringBuilder.
        /// </summary>
        /// <param name="sbCode">out StringBuilder</param>
        public void RewritePyFiletoSB(out StringBuilder sbCode)
        {
            StringBuilder sb = new StringBuilder();

            //See if sys is imported...
            if (!DoIPEvaluate("dir()").ToString().Contains("sys"))
            {
                consoleTextBox.printPrompt();
                consoleTextBox.WriteText("import sys");
                this.SimEnter();
            }

            //Browse to the file...
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = IronPythonEditor.Console.Paths.MiscDirs.vs_Projects;
            ofd.Filter = "Python files (*.py)|*.py|All files (*.*)|*.*";
            ofd.ShowDialog();

            try
            {
                string filetext = File.ReadAllText(ofd.FileName);
                //tabs create a problem when trying to remove comments
                filetext = filetext.Replace("\t", "    ");

                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StringReader sr = new StringReader(filetext))
                {
                    String line;
                    int pos = 0;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {

                        /////temp testing
                        /// "        # unpp augmented predicate"
                        /// "        """ "
                        ///if (line == "    def chunk_lemmatised(self,lemmatised_text):")
                        ///{
                        ///    int temp = pos;
                        ///}
                        /////temp testing

                        //if the line is a # comment line, or a single line do not add...
                        if (!line.StartsWith("#") && !line.StartsWith("    #") && !line.StartsWith("        #") && !line.StartsWith("            #") && !line.Equals("\r\n") && line != "")
                        {
                            //catch """ comments
                            if (line.StartsWith("\"\"\"") || line.StartsWith("    \"\"\"") || line.StartsWith("        \"\"\"") || line.StartsWith("            \"\"\""))
                            {
                                //the line may also end with """, so if it is not read until end
                                if (!IsSingleCommentLine(line))
                                {
                                    //get to the end of the comments
                                    while (!sr.ReadLine().TrimEnd().EndsWith("\"\"\""))
                                    {
                                        //do nothing
                                    }
                                }

                                //reassign line
                                line = sr.ReadLine();
                            }

                            //the line may also end with """, so if it is not read until end
                            if (!IsSingleCommentLine(line))
                            {
                                //if the line ends with """, then delete """ and read until end
                                if (line.TrimEnd().EndsWith("\"\"\"") && line.IndexOf("\"\"\"") == line.Length - 3)
                                {
                                    //remove """ and reassign line
                                    line = line.Remove(line.Length - 3);

                                    //get to the end of the comments
                                    while (!sr.ReadLine().TrimEnd().EndsWith("\"\"\""))
                                    {
                                        //do nothing
                                    }
                                }
                                //then append line
                                sb.AppendLine(line);
                            }
                        }
                        pos = sb.Length;
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                consoleTextBox.WriteText("The file could not be read:");
                consoleTextBox.WriteText(e.Message);
            }

            sbCode = sb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true if line begins with #, or begins with """ and endwith """</returns>
        public bool IsSingleCommentLine(string line)
        {
            //Trim the end of the line because sometimes whitespace after """
            line = line.TrimEnd();

            if (line.StartsWith("#") || line.StartsWith("    #") || line.StartsWith("        #") || line.StartsWith("            #") && line != "")
            {
                return true;
            }
            else if (line.StartsWith("\"\"\"") || line.StartsWith("    \"\"\"") || line.StartsWith("        \"\"\"") || line.StartsWith("            \"\"\""))
            {
                if (line.TrimEnd().EndsWith("\"\"\"") && line.IndexOf("\"\"\"") != line.Length - 3)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Returns aa ArrayList from a StringCollection  
        /// </summary>
        /// <param name="StringColin">Incoming StringCollection.</param>
        public ArrayList Convert_StringCollectiontoArrayList(StringCollection StringColin)
        {
            ArrayList newArrayList = new ArrayList();

            StringEnumerator myEnumerator = StringColin.GetEnumerator();
            while (myEnumerator.MoveNext())
                newArrayList.Add(myEnumerator.Current.ToString());

            return newArrayList;
        }

        /// <summary>
        /// ImportPaths
        /// </summary>
        /// <param name="arg"></param>
        public void ImportPaths(string arg)
        {
            StringCollection scMiscDirs = new StringCollection();
            scMiscDirs.Add(IronPythonEditor.Console.Paths.MiscDirs.ConceptNet);
            scMiscDirs.Add(IronPythonEditor.Console.Paths.MiscDirs.montylingua);
            scMiscDirs.Add(IronPythonEditor.Console.Paths.MiscDirs.vs_Projects);
            StringEnumerator SCEMiscDirs = scMiscDirs.GetEnumerator();

            StringCollection scPython24Dirs = new StringCollection();
            scPython24Dirs.Add(IronPythonEditor.Console.Paths.Python24Dirs.Python24_DLLs);
            scPython24Dirs.Add(IronPythonEditor.Console.Paths.Python24Dirs.Python24_Lib);
            scPython24Dirs.Add(IronPythonEditor.Console.Paths.Python24Dirs.Python24_Lib_lib_tk);
            scPython24Dirs.Add(IronPythonEditor.Console.Paths.Python24Dirs.Python24_libs);
            scPython24Dirs.Add(IronPythonEditor.Console.Paths.Python24Dirs.Python24_Tools);
            scPython24Dirs.Add(IronPythonEditor.Console.Paths.Python24Dirs.Python24_Tools_Scripts);
            StringEnumerator SCEPython24Dirs = scPython24Dirs.GetEnumerator();

            StringCollection scIronPythonDirs = new StringCollection();
            scIronPythonDirs.Add(IronPythonEditor.Console.Paths.IronPythonDirs.IronPython_Tutorial);
            //scIronPythonDirs.Add(UIIronTextBox.Paths.IronPythonDirs.Runtime);
            StringEnumerator SCEIronPythonDirs = scIronPythonDirs.GetEnumerator();

            //Create All SC
            StringCollection scAll = new StringCollection();
            while (SCEMiscDirs.MoveNext())
            {
                scAll.Add(SCEMiscDirs.Current);
            }
            while (SCEPython24Dirs.MoveNext())
            {
                scAll.Add(SCEPython24Dirs.Current);
            }
            while (SCEIronPythonDirs.MoveNext())
            {
                scAll.Add(SCEIronPythonDirs.Current);
            }
            StringEnumerator SCEAll = scAll.GetEnumerator();

            //Reset Enums
            SCEMiscDirs.Reset();
            SCEPython24Dirs.Reset();
            SCEIronPythonDirs.Reset();

            //Check to see if sys is loaded
            if (!DoIPEvaluate("dir()").ToString().Contains("sys"))
            {
                consoleTextBox.printPrompt();
                consoleTextBox.WriteText("import sys");
                this.SimEnter();
            }
            else
                consoleTextBox.printPrompt();


            try
            {
                switch (arg)
                {
                    case "misc":
                        {
                            while (SCEMiscDirs.MoveNext())
                            {
                                //consoleTextBox.printPrompt();
                                consoleTextBox.WriteText("sys.path.append(\"" + SCEMiscDirs.Current + "\")");
                                this.SimEnter();
                            }
                            break;
                        }
                    case "python24":
                        {
                            while (SCEPython24Dirs.MoveNext())
                            {
                                //consoleTextBox.printPrompt();
                                consoleTextBox.WriteText("sys.path.append(\"" + SCEPython24Dirs.Current + "\")");
                                this.SimEnter();
                            }
                            break;
                        }
                    case "ironpython":
                        {
                            while (SCEIronPythonDirs.MoveNext())
                            {
                                //consoleTextBox.printPrompt();
                                consoleTextBox.WriteText("sys.path.append(\"" + SCEIronPythonDirs.Current + "\")");
                                this.SimEnter();
                            }
                            break;
                        }
                    case "all":
                        {
                            while (SCEAll.MoveNext())
                            {
                                //consoleTextBox.printPrompt();
                                consoleTextBox.WriteText("sys.path.append(\"" + SCEAll.Current + "\")");
                                this.SimEnter();
                            }
                            break;
                        }
                    case "paths":
                        {
                            while (SCEAll.MoveNext())
                            {
                                //consoleTextBox.printPrompt();
                                consoleTextBox.WriteText("sys.path.append(\"" + SCEAll.Current + "\")");
                                this.SimEnter();
                            }
                            break;
                        }
                    default:
                        consoleTextBox.WriteText("Invalid arg. Only: -misc, -python24, -ironpython, -all");
                        break;
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                consoleTextBox.WriteText("ImportPaths error: ");
                consoleTextBox.WriteText(e.Message);
            }
        }
    }
   }
