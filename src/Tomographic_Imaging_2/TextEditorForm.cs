using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Tomographic_Imaging_2
{
    public partial class TextEditorForm : DockContent
    {
        public void  LoadScriptFile(string Filename)
        {
          //  textEditorForm1.OpenFile(Filename);
        }

        public TextEditorForm()
        {
            InitializeComponent();

          //  mPythonEngine = new IronPythonEditor.EvaluatePython();
          //  mPythonEngine.PythonSpeaks += new IronPythonEditor.TextEditor.cTextWriter.PythonSpeaksEvent(mPythonEngine_PythonSpeaks);
        }

       /* public TextEditorForm()
        {
            InitializeComponent();

         //   mPythonEngine = PythonEngine;
          //  mPythonEngine.PythonSpeaks += new IronPythonEditor.TextEditor.cTextWriter.PythonSpeaksEvent(mPythonEngine_PythonSpeaks);
        }*/

        private void TextEditorForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //handle the motion of the mouse to produce a verticle splitter control
               // textEditorForm1.Height = e.Y - 5;
               // ironPythonConsole1.Top = e.Y + 5;
               // ironPythonConsole1.Height = this.Height - (e.Y + 5);
            }
        }
       // private IronPythonEditor.EvaluatePython mPythonEngine;

        private void TextEditorForm_Load(object sender, EventArgs e)
        {
            //textEditorForm1.LoadAndInitializeEditor(mPythonEngine, ironPythonConsole1);
           // ironPythonConsole1.InitializeControl();

            string StartProgram =
@"import clr
clr.AddReferenceToFileAndPath(ExecutablePath + 'MathHelpLib.dll')
clr.AddReferenceToFileAndPath(ExecutablePath + 'ImageViewer.dll')
clr.AddReferenceToFileAndPath(Executable)
from MathHelpLib import *
from TomographicLibs import *
from ImageViewer import *
clr.AddReference('System.Drawing')
from System import *
from System.Drawing import *
from System.Drawing.Imaging import *";
           // ironPythonConsole1.SetProgram(StartProgram);
           // ironPythonConsole1.SetVariable("ScriptingInterface", ScriptingInterface.scriptingInterface);
        }

        void mPythonEngine_PythonSpeaks(string Message)
        {
           // ironPythonConsole1.LogMessageRecieved(Message);
        }

        private void ironPythonConsole1_PythonVarAdded(string VarName, object Var)
        {
            ScriptingInterface.scriptingInterface.MakeVariableVisible(VarName, Var);
        }

        private void ironPythonConsole1_PythonVarRemoved(string VarName)
        {
            ScriptingInterface.scriptingInterface.RemoveVisibleVariable(VarName);
        }
    }
}
