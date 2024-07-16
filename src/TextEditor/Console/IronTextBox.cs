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


namespace IronPythonEditor.Console
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(IronTextBox))]
    [DesignerAttribute(typeof(IronTextBoxControl))]
    internal class IronTextBox : TextBox
    {
        #region IronTextBox members
        /// <summary>
        /// Default prompt text.
        /// </summary>
        private string prompt = ">>>";

        /// <summary>
        /// Used for storing commands.
        /// </summary>
        private IronPythonEditor.Console.CommandHistory commandHistory = new CommandHistory();

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Holds temporary defFunc lines.
        /// </summary>
        public System.Text.StringBuilder defStmtBuilder = new System.Text.StringBuilder();

        /// <summary>
        /// StringCollection of all MiscDirs
        /// </summary>
        public static StringCollection scMisc = new StringCollection();

        /// <summary>
        /// StringCollection of all Python24Dirs
        /// </summary>
        public static StringCollection scPython24 = new StringCollection();

        /// <summary>
        /// StringCollection of all IronPythonDirs
        /// </summary>
        public static StringCollection scIronPython = new StringCollection();

        /// <summary>
        /// Intellisense ToolTip.
        /// </summary>
        System.Windows.Forms.ToolTip intellisense = new System.Windows.Forms.ToolTip();

        /// <summary>
        /// True if currently processing raw_text()
        /// </summary>
        public static Boolean IsRawInput = false;

        /// <summary>
        /// Hold raw_input prompt by user
        /// </summary>
        public string rawprompt = "";

        #endregion IronTextBox members

        internal IronTextBox()
        {
            InitializeComponent();
            printPrompt();

            // Set up the delays for the ToolTip.
            intellisense.AutoPopDelay = 1000;
            intellisense.InitialDelay = 100;
            intellisense.ReshowDelay = 100;
            // Force the ToolTip text to be displayed whether or not the form is active.
            intellisense.ShowAlways = true;

            this.Controls.Add(AutoCompleteListBox);
            AutoCompleteListBox.DoubleClick += new EventHandler(AutoCompleteListBox_DoubleClick);
            AutoCompleteListBox.KeyDown += new KeyEventHandler(AutoCompleteListBox_KeyDown);
            AutoCompleteListBox.LostFocus += new EventHandler(AutoCompleteListBox_LostFocus);
            AutoCompleteListBox.Visible = false;
             
        }

        #region Overrides
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

        /// <summary>
        /// Overridden to protect against deletion of contents
        /// cutting the text and deleting it from the context menu
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0302: //WM_PASTE
                case 0x0300: //WM_CUT
                case 0x000C: //WM_SETTEXT
                    if (!IsCaretAtWritablePosition())
                        MoveCaretToEndOfText();
                    break;
                case 0x0303: //WM_CLEAR
                    return;
            }
            base.WndProc(ref m);
        }
        #endregion Overrides

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // consoleTextBox
            // 
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Location = new Point(0, 0);
            this.MaxLength = 0;
            this.Multiline = true;
            this.Name = "consoleTextBox";
            this.AcceptsTab = true;
            this.AcceptsReturn = true;    //for TextBox use
            this.ScrollBars=ScrollBars.Both ;   //for TextBox use
           // this.ScrollBars = RichTextBoxScrollBars.Both;   //for RichTextBox use
            this.Size = new Size(400, 176);
            this.TabIndex = 0;
            this.Text = "";
            this.KeyPress += new KeyPressEventHandler(this.consoleTextBox_KeyPress);
            this.KeyDown += new KeyEventHandler(ConsoleControl_KeyDown);
            this.MouseUp += new MouseEventHandler(IronTextBox_MouseUp);

            // 
            // IronTextBoxControl
            // 
            this.Name = "IronTextBox";
            this.Size = new Size(400, 176);
            this.ResumeLayout(false);

        }

       
        #endregion

        #region IronTextBox Base Methods
        /// <summary>
        /// Sends the prompt to the IronTextBox
        /// </summary>
        public void printPrompt()
        {
            string currentText = this.Text;

            //add newline if it does not exist
            if ((currentText.Length != 0) && (currentText[currentText.Length - 1] != '\n'))
                printLine();

            //add the prompt
            this.AddText(prompt);
        }

        /// <summary>
        /// Sends a newline character to the IronTextBox
        /// </summary>
        public void printLine()
        {
            this.AddText(System.Environment.NewLine);
        }

        /// <summary>
        /// Returns currentline's text string
        /// </summary>
        /// <returns>Returns currentline's text string</returns>
        public string GetTextAtPrompt()
        {
            if (GetCurrentLine() != "")
                return GetCurrentLine().Substring(prompt.Length);
            else
            {
                string mystring = (string)this.Lines.GetValue(this.Lines.Length - 2);
                if (mystring.Trim() == "")
                    return "";
                else 
                    return mystring.Substring(prompt.Length);
            }
        }

        /// <summary>
        /// Add a command to IronTextBox command history.
        /// </summary>
        /// <param name="currentCommand">IronTextBox command line</param>
        public void AddcommandHistory(string currentCommand)
        {
            commandHistory.Add(currentCommand);
        }

        /// <summary>
        /// Returns true if Keys.Enter
        /// </summary>
        /// <param name="key">Keys</param>
        /// <returns>Returns true if Keys.Enter</returns>
        private bool IsTerminatorKey(System.Windows.Forms.Keys key)
        {
            return key == Keys.Enter;
        }

        /// <summary>
        /// Returns true if (char)13 '\r'
        /// </summary>
        /// <param name="keyChar">char of keypressed</param>
        /// <returns>Returns true if (char)13 '\r'</returns>
        private bool IsTerminatorKey(char keyChar)
        {
            return ((int)keyChar) == 13;
        }

        /// <summary>
        /// Returns the current line, including prompt.
        /// </summary>
        /// <returns>Returns the current line, including prompt.</returns>
        private string GetCurrentLine()
        {
            if (this.Lines.Length > 0)
            {
                return (string)this.Lines.GetValue(this.Lines.GetLength(0) - 1);
            }
            else
                return "";
        }

        /// <summary>
        /// Replaces the text at the current prompt.
        /// </summary>
        /// <param name="text">new text to replace old text.</param>
        private void ReplaceTextAtPrompt(string text)
        {
            string currentLine = GetCurrentLine();
            int charactersAfterPrompt = currentLine.Length - prompt.Length;

            if (charactersAfterPrompt == 0)
                this.AddText(text);
            else
            {
                this.Select(this.TextLength - charactersAfterPrompt, charactersAfterPrompt);
                this.SelectedText = text;
            }
        }

        /// <summary>
        /// Returns true if caret is positioned on the currentline.
        /// </summary>
        /// <returns>Returns true if caret is positioned on the currentline.</returns>
        private bool IsCaretAtCurrentLine()
        {
            return this.TextLength - this.SelectionStart <= GetCurrentLine().Length;
        }

        /// <summary>
        /// Adds text to the IronTextBox
        /// </summary>
        /// <param name="text">text to be added</param>
        private void AddText(string text)
        {
            //Optional////////////
            scollection.Add(text);  //Optional
            //this.Text = StringCollecttostring(scollection); //Optional
            //////////////////////

            this.Enabled = false;
            this.Text += text;
            MoveCaretToEndOfText();
            this.Enabled = true;
            this.Focus();
            this.Update();
        }

        public char LastCharactor()
        {
            if (this.Text =="")
                return ' ';
            else 
                return this.Text[this.Text.Length - 1];
        }
        /// <summary>
        /// Returns a string retrieved from a StringCollection.
        /// </summary>
        /// <param name="inCol">StringCollection to be searched.</param>
        public string StringCollecttostring(System.Collections.Specialized.StringCollection inCol)
        {
            string value = "";
            System.Collections.Specialized.StringEnumerator myEnumerator = inCol.GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                value += myEnumerator.Current;
            }

            return value;
        }

        /// <summary>
        /// Move caret to the end of the current text.
        /// </summary>
        public void MoveCaretToEndOfText()
        {
            this.SelectionStart = this.TextLength;
            this.ScrollToCaret();
        }

        /// <summary>
        /// Returns true is the caret is just before the current prompt.
        /// </summary>
        /// <returns></returns>
        private bool IsCaretJustBeforePrompt()
        {
            return IsCaretAtCurrentLine() && GetCurrentCaretColumnPosition() == prompt.Length;
        }

        /// <summary>
        /// Returns the column position. Useful for selections.
        /// </summary>
        /// <returns></returns>
        private int GetCurrentCaretColumnPosition()
        {
            string currentLine = GetCurrentLine();
            int currentCaretPosition = this.SelectionStart;
            return (currentCaretPosition - this.TextLength + currentLine.Length);
        }

        /// <summary>
        /// Is the caret at a writable position.
        /// </summary>
        /// <returns></returns>
        private bool IsCaretAtWritablePosition()
        {
            return IsCaretAtCurrentLine() && GetCurrentCaretColumnPosition() >= prompt.Length;
        }

        /// <summary>
        /// Sets the text of the prompt.  Default is ">>>"
        /// </summary>
        /// <param name="val">string of new prompt</param>
        public void SetPromptText(string val)
        {
            string currentLine = GetCurrentLine();
            this.Select(0, prompt.Length);
            this.SelectedText = val;
            prompt = val;
        }

        /// <summary>
        /// Gets and sets the IronTextBox prompt.
        /// </summary>
        public string Prompt
        {
            get { return prompt; }
            set { SetPromptText(value); }
        }

        /// <summary>
        /// Returns the string array of the command history. 
        /// </summary>
        /// <returns></returns>
        public string[] GetCommandHistory()
        {
            return commandHistory.GetCommandHistory();
        }

        /// <summary>
        /// Adds text to the IronTextBox.
        /// </summary>
        /// <param name="text"></param>
        public void WriteText(string text)
        {
            this.AddText(text);
        }

        #region IronTextBox Events
        /// <summary>
        /// Handle KeyPress events here.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">KeyPressEventArgs</param>
        private void consoleTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            //If current key is a backspace and is just before prompt, then stay put!
            if (e.KeyChar == (char)8 && IsCaretJustBeforePrompt())
            {
                e.Handled = true;
                return;
            }

            //If current key is enter
            if (IsTerminatorKey(e.KeyChar))
            {
                //**ANY CHANGES HERE MUST ALSO BE COPIED TO SimEnter()**
                e.Handled = true;
                string currentCommand = GetTextAtPrompt();

                if (currentCommand.Length >0 &&  currentCommand[currentCommand.Length-1] == '\'')
                {
                    printPrompt();
                    return;
                }
                //Optional: add the command to the stringcollection
                scollection.Add(currentCommand);
                ///////////////////////////////////////////////////

                //If it is not an empty command, then "fire" the command
                if (currentCommand.Length != 0 && this.defStmtBuilder.Length == 0 && !IsRawInput)
                {
                    if (!currentCommand.Trim().Contains("raw_input"))
                        printLine();
                    ((IronPythonEditor.Console.IronTextBoxControl)this.Parent).FireCommandEntered(currentCommand);
                    commandHistory.Add(currentCommand);
                }

                //if we are doing a def statement (currentCommand.EndsWith(":"))
                if (this.defStmtBuilder.Length != 0)
                {
                    if (currentCommand.EndsWith(":"))
                    {
                        //we are in the first line of a def, it has already printed to console
                        
                        //autoindent the current autoindent value
                        //int asize = Parser.GetNextAutoIndentSize(this.defStmtBuilder.ToString()+"\r\n", 4);

                        //don't printPrompt();
                        ReplaceTextAtPrompt("..." + CreateIndentstring(4));
                        e.Handled = true;
                        return;

                    }
                    else//We are past the first line, and are indenting or ending a def
                    {
                        this.defStmtBuilder.Append(currentCommand + "\r\n");

                        //if it is an empty command let's see if we just finished a def statement
                        if (currentCommand.Trim().Equals(""))
                        {
                            ((IronPythonEditor.Console.IronTextBoxControl)this.Parent).FireCommandEntered(this.defStmtBuilder.ToString().Trim());
                            commandHistory.Add(this.defStmtBuilder.ToString());

                            //we just finished a def so clear the defbuilder
                            this.defStmtBuilder = this.defStmtBuilder.Remove(0, this.defStmtBuilder.Length);
                        }
                        else
                        {
                            //don't printPrompt();
                            AddText("\r\n..." + CreateIndentstring(4));
                            e.Handled = true;
                            return;
                        }
                    }
                }

                //raw_input support...
                if (currentCommand.Trim().Contains("raw_input("))
                {
                    IsRawInput = true;

                    //Note: if raw_input is in quotes this will not work
                    //fyi: IronPython.Modules.Builtin.RawInput();
                    //remove the "\r\n" from IPEWrapper
                    this.Text = this.Text.Remove(this.Text.Length - "\r\n".Length, "\r\n".Length);
                    rawprompt = (string)this.Lines.GetValue(this.Lines.Length - 1);
                    MoveCaretToEndOfText();

                    //AddText(temp);
                    e.Handled = true;
                    return;
                }
                                
                if(IsRawInput)
                {
                    string rawcommand = (string)this.Lines.GetValue(this.Lines.Length - 2);
                    rawcommand = rawcommand.Replace(Prompt, "");
                    string tempprompt = (string)this.Lines.GetValue(this.Lines.Length - 1);

                    ScriptEngine  iptemp = Python.CreateEngine();

                    //examine to see what type of raw_input
                    if (rawcommand.Trim().Equals("raw_input()"))
                    {
                        IsRawInput = false;
                    }
                    else// s = raw_input('--> ')
                    {
                        IsRawInput = false;
                        rawprompt = "";
                        e.Handled = true;
                        printPrompt();
                        MoveCaretToEndOfText();
                        return;
                    }
                    

                }

                //if(GetTextAtPrompt().Trim().Equals(""))
                    printPrompt();
            }


            /*
            // Handle backspace and stringcollection to help the commandhistory accuracy and debugging.
            if (e.KeyChar == (char)8 && (GetStringCollectValue(scollection, scollection.Count - 1).Length == 1) && commandHistory.LastCommand.Contains(GetStringCollectValue(scollection, scollection.Count - 1)))
            {
                scollection.RemoveAt(scollection.Count - 1);
            }*/

        }

        /// <summary>
        /// Build a string of returning spaces for indenting
        /// </summary>
        /// <param name="indentsize"></param>
        /// <returns></returns>
        public string CreateIndentstring(int indentsize)
        {
            string r = "";
            for (int i = 0; i < indentsize; i++)
            {
                r += " ";
            }
            return r;
        }

       
       public event AutoCompletionListFillEvent AutoCompleteListFill;

        /// <summary>
        /// KeyEvent control for staying inside the currentline and autocomplete features
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">KeyEventArgs</param>
        private void ConsoleControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            
            // If the caret is anywhere else, set it back when a key is pressed.
            if (!IsCaretAtWritablePosition() && !(e.Control || IsTerminatorKey(e.KeyCode)))
            {
                MoveCaretToEndOfText();
            }

            // Prevent caret from moving before the prompt
            if (e.KeyCode == System.Windows.Forms.Keys.Left && IsCaretJustBeforePrompt() || e.KeyCode == System.Windows.Forms.Keys.Back && IsCaretJustBeforePrompt())
            {
                e.Handled = true;
            }

            if (AutoCompleteListBox.Visible == true)
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Down)
                {
                    AutoCompleteListBox.Focus();
                    try
                    {
                        AutoCompleteListBox.SelectedIndex++;
                    }
                    catch { }
                }
                else if (e.KeyCode == System.Windows.Forms.Keys.Up)
                {
                    AutoCompleteListBox.Focus();
                    try
                    {
                        AutoCompleteListBox.SelectedIndex--;
                    }
                    catch { }
                }
                else if (e.KeyCode == System.Windows.Forms.Keys.Right)
                {
                    AutoCompleteListBox.Visible = false;
                }
                else if (!(e.KeyValue == 16 || e.KeyValue==8 || (e.KeyValue >= 'a' && e.KeyValue <= 'z') || (e.KeyValue >= 'A' && e.KeyValue <= 'Z')))
                {
                    AutoCompleteListBox.Visible = false;
                }
                if ((e.KeyValue >= 'a' && e.KeyValue <= 'z') || (e.KeyValue >= 'A' && e.KeyValue <= 'Z'))
                {
                    SearchString += (char) e.KeyValue;
                    int index = AutoCompleteListBox.FindString(SearchString);
                    AutoCompleteListBox.SelectedIndex = index;
                }
                if (e.KeyValue == 8)
                {
                    try
                    {
                        SearchString = SearchString.Substring(0, SearchString.Length - 1);
                    }
                    catch { }
                }
                
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Down)
            {
                if (commandHistory.DoesNextCommandExist())
                {
                    ReplaceTextAtPrompt(commandHistory.GetNextCommand());
                }
                e.Handled = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {
                if (commandHistory.DoesPreviousCommandExist())
                {
                    ReplaceTextAtPrompt(commandHistory.GetPreviousCommand());
                }
                e.Handled = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Right)
            {
                // Performs command completion
                string currentTextAtPrompt = GetTextAtPrompt();
                string lastCommand = commandHistory.LastCommand;

                //If the last command is not null and no text at the current prompt or lastcommand starts with the currenttext at the current prompt,
                //then autofill because the right arrow key was pressed.
                if (lastCommand != null && (currentTextAtPrompt.Length == 0 || lastCommand.StartsWith(currentTextAtPrompt)))
                {
                    if (lastCommand.Length > currentTextAtPrompt.Length)
                    {
                        int i = scollection.Count;
                        //scollection.Insert(scollection.Count, lastCommand[currentTextAtPrompt.Length].ToString());
                        this.AddText(lastCommand[currentTextAtPrompt.Length].ToString());
                    }
                }
            }

            if (e.KeyValue == 190)
            {
                #region Dot Pressed
                DotTypeAutoComplete = true;
                string Text = GetTextAtPrompt();
                string Previous = Text.Substring(0, Text.Length - (this.Text.Length - this.SelectionStart));

                string lPrev = Previous.ToLower();
                int WordCut = 0;
                for (int i = lPrev.Length - 1; i > 0; i--)
                {
                    if (!(lPrev[i] >= 'a' && lPrev[i] <= 'z'))
                    {
                        WordCut = i;
                        break;
                    }
                }
                if (WordCut != 0)
                    Previous = Previous.Substring(WordCut + 1, Previous.Length - WordCut - 1);
                string After = Text.Substring(this.Text.Length - this.SelectionStart);
                if (AutoCompleteListFill != null)
                {
                    AutoCompleteItems = AutoCompleteListFill(Previous);
                    if (AutoCompleteItems != null)
                    {
                        AutoCompleteListBox.Width = (int)((double)this.Width / 3d);
                        AutoCompleteListBox.Height = this.Height;
                        AutoCompleteListBox.Items.Clear();
                        AutoCompleteListBox.Items.AddRange(AutoCompleteItems);

                        AutoCompleteListBox.BringToFront();
                        AutoCompleteListBox.Visible = true;
                        AutoCompleteListBox.Location = new Point((int)((double)this.Width * 2d / 3d - 20), 0);
                        SearchString = "";
                       
                    }
                }
                #endregion
            }

            if ((e.KeyValue >= 'a' && e.KeyValue <= 'z') || (e.KeyValue >= 'A' && e.KeyValue <= 'Z') && AutoCompleteListBox.Visible==false )
            {
                #region Key Pressed
                DotTypeAutoComplete = false;
                string Text = GetTextAtPrompt();
                
                if (AutoCompleteListFill != null)
                {
                    AutoCompleteItems = AutoCompleteListFill("");
                    if (AutoCompleteItems != null)
                    {
                        AutoCompleteListBox.Width = (int)((double)this.Width / 3d);
                        AutoCompleteListBox.Height = this.Height;
                        AutoCompleteListBox.Items.Clear();
                        AutoCompleteListBox.Items.AddRange(AutoCompleteItems);

                        AutoCompleteListBox.BringToFront();
                        AutoCompleteListBox.Visible = true;
                        AutoCompleteListBox.Location = new Point((int)((double)this.Width * 2d / 3d - 20), 0);
                        SearchString = "" + (char)e.KeyValue ;

                    }
                }
                #endregion
            }
        }

        void IronTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            AutoCompleteListBox.Visible = false;
        }
        bool DotTypeAutoComplete = true;
        string SearchString = "";
        string[] AutoCompleteItems = null;
        ListBox AutoCompleteListBox = new ListBox();
        void AutoCompleteListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                AutoCompleteListBox_DoubleClick(sender, e);
            }
        }

        void AutoCompleteListBox_LostFocus(object sender, EventArgs e)
        {
            AutoCompleteListBox.Visible = false;
        }

        void AutoCompleteListBox_DoubleClick(object sender, EventArgs e)
        {
            if (DotTypeAutoComplete)
            {
                string Text = GetTextAtPrompt();

                string lPrev = Text.ToLower();
                int WordCut = this.SelectionStart;
                for (int i = lPrev.Length - 1; i > 0; i--)
                {
                    if (lPrev[i] == '.')
                    {
                        WordCut = i;
                        break;
                    }
                }

                Text = Text.Insert(WordCut + 1, AutoCompleteListBox.SelectedItem.ToString());
                ReplaceTextAtPrompt(Text);
            }
            else
            {
                string Text = GetTextAtPrompt();
                string lPrev = Text.ToLower();
                int WordCut = 0;
                for (int i = lPrev.Length - 1; i > 0; i--)
                {
                    if (!(lPrev[i] >= 'a' && lPrev[i] <= 'z'))
                    {
                        WordCut = i;
                        break;
                    }
                }
                Text=  Text.Remove(WordCut,Text.Length - (this.Text.Length- this.SelectionStart) - WordCut);
                Text = Text.Insert(WordCut, AutoCompleteListBox.SelectedItem.ToString());
                ReplaceTextAtPrompt(Text);
            }
        }

        void AutoCompleteListBox_Click(object sender, EventArgs e)
        {
            
        }

        #endregion IronTextBox Events

        #endregion IronTextBox Base Methods

        #region IronTextBox IronPython Support
        /// <summary>
        /// Stores input commands from IronTextBox
        /// </summary>
        StringCollection input = new StringCollection();

        /// <summary>
        /// Stores output generated from IronPython
        /// </summary>
        StringCollection output = new StringCollection();

        #endregion IronTextBox IronPython Support

        #region StringCollection support
        /// <summary>
        /// Commands and strings from IronTextBox.AddText() gets stored here
        /// Status: Currently not used 3/12/06 11:16am
        /// </summary>
        System.Collections.Specialized.StringCollection scollection = new System.Collections.Specialized.StringCollection();

        /// <summary>
        /// Returns a string retrieved from a StringCollection.
        /// </summary>
        /// <param name="inCol">StringCollection to be searched.</param>
        /// <param name="index">index of StringCollection to retrieve.</param>
        public string GetStringCollectValue(System.Collections.Specialized.StringCollection inCol, int index)
        {
            string value = "";
            int count = 0;
            System.Collections.Specialized.StringEnumerator myEnumerator = inCol.GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                if (index == count)
                {
                    value = myEnumerator.Current;
                }

                count = count + 1;
            }

            return value;
        }
        #endregion StringCollection support
    }
  
}
