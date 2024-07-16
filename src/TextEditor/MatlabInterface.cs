using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IronPythonEditor
{
    public partial class MatlabInterface : UserControl
    {
        private IronPythonEditor.EvaluatePython mPythonEngine;

       

        public void LoadScriptFile(string Filename)
        {
            textEditorForm1.OpenFile(Filename);
        }

        public MatlabInterface()
        {
            InitializeComponent();
            mPythonEngine = new IronPythonEditor.EvaluatePython();
            mPythonEngine.PythonSpeaks += new IronPythonEditor.TextEditor.cTextWriter.PythonSpeaksEvent(mPythonEngine_PythonSpeaks);
            ironPythonConsole1.SetEngine(mPythonEngine);
        }



        public void SetDefaultHeader(string HeaderCode)
        {

            textEditorForm1.Text = HeaderCode;
            ironPythonConsole1.SetProgram(HeaderCode);

        }


        #region Events

        void mPythonEngine_PythonSpeaks(string Message)
        {
            ironPythonConsole1.LogMessageRecieved(Message);
        }

        private void textEditorForm1_Load(object sender, EventArgs e)
        {
            textEditorForm1.LoadAndInitializeEditor(mPythonEngine, ironPythonConsole1);
            ironPythonConsole1.InitializeControl();

           /* string StartProgram = mDefaultCodeHeader;
            ironPythonConsole1.SetProgram(StartProgram);*/
        }

     

        Dictionary<string, object> VisibleVariable = new Dictionary<string, object>();
        private void ironPythonConsole1_PythonVarAdded(string VarName, object Var)
        {
            if (VisibleVariable.ContainsKey(VarName))
            {
                VisibleVariable.Remove(VarName);
                VisibleVariable.Add(VarName, Var);
                variableWindow1.UpdateVariable(VarName, Var);
            }
            else
            {
                VisibleVariable.Add(VarName, Var);
                variableWindow1.UpdateVarDisplay(VarName, Var);
            }
        }

        private void ironPythonConsole1_PythonVarRemoved(string VarName)
        {
            try
            {
                variableWindow1.RemoveVariable(VarName);
                VisibleVariable.Remove(VarName);
            }
            catch (Exception ex)
            {
                //LoggerForm.LogErrorMessage(ex);
            }
        }


        #endregion

    }
}
