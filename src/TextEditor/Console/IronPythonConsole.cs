using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using IronPythonEditor.TextEditor;

namespace IronPythonEditor.Console
{
    public partial class IronPythonConsole : UserControl
    {
        private EvaluatePython mPythonEngine;
        public delegate void PythonVarAddedEvent(string VarName, object Var);
        public delegate void PythonVarRemovedEvent(string VarName);

        public event PythonVarAddedEvent PythonVarAdded;
        public event PythonVarRemovedEvent PythonVarRemoved;

        public IronPythonConsole()
        {
            InitializeComponent();
            // LoggerForm.LogMessageRecieved += new LogMessageRecievedEvent(LoggerForm_LogMessageRecieved);
            // InitializeControl();
        }

        public IronPythonConsole(EvaluatePython PythonEngine)
        {
            InitializeComponent();
            // LoggerForm.LogMessageRecieved += new LogMessageRecievedEvent(LoggerForm_LogMessageRecieved);
            // InitializeControl();
            SetEngine(PythonEngine);
        }

        public void LogMessageRecieved(string Message)
        {
            ironTextBoxControl1.WriteLine(Message + "  '");
        }

        public void SetVariable(string VariableName, object Variable)
        {
            ironTextBoxControl1.scope.SetVariable(VariableName, Variable);
        }
        public void SetProgram(string Program)
        {
            string[] Lines = Program.Split(new string[] { "\n\r", "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in Lines)
            {
                ironTextBoxControl1.WriteText(line);
                ironTextBoxControl1.SimEnter();

            }
        }

        public void SetEngine(EvaluatePython ep)
        {
            mPythonEngine = ep;
            cTextWriter ctw = new cTextWriter();
            ctw.PythonSpeaks += new cTextWriter.PythonSpeaksEvent(ctw_PythonSpeaks);
            ironTextBoxControl1.scope = ep.scope;
            ironTextBoxControl1.engine = ep.engine ;

            ironTextBoxControl1.scope.SetVariable("Console", ctw);
            ironTextBoxControl1.scope.SetVariable("Executable", Application.ExecutablePath);
            ironTextBoxControl1.scope.SetVariable("ExecutablePath", Path.GetDirectoryName(Application.ExecutablePath) + "\\");

            timer1.Enabled = true;
        }

        public void InitializeControl()
        {
            
        }

        void ctw_PythonSpeaks(string Message)
        {
            LogMessageRecieved(Message);
        }

        List<string> Ignores = new List<string>();
        List<string> lVarNames = new List<string>();
        private void timer1_Tick(object sender, EventArgs e)
        {
            List<string> varNames = new List<string>();
            varNames.AddRange(mPythonEngine.scope.GetVariableNames());
            for (int i = 0; i < Ignores.Count; i++)
            {
                try
                {
                    varNames.Remove(Ignores[i]);
                }
                catch { }
            }

            if (PythonVarAdded != null)
            {
                for (int i = 0; i < varNames.Count; i++)
                {
                    if (lVarNames.Contains(varNames[i]) == false)
                    {
                        object var;
                        mPythonEngine.scope.TryGetVariable(varNames[i], out var);
                        if (var != null)
                        {
                            Type t = var.GetType();
                            if (t != typeof(IronPython.Runtime.Types.PythonType))
                            {
                                if (t != typeof(Microsoft.Scripting.Actions.NamespaceTracker))
                                    if (t != typeof(Microsoft.Scripting.Actions.TypeGroup))
                                        if (t != typeof(IronPython.Runtime.PythonDictionary))
                                            if (t != typeof(IronPython.Runtime.PythonModule))
                                                PythonVarAdded(varNames[i], var);
                                //ScriptingInterface.scriptingInterface.MakeVariableVisible(varNames[i], var);*/
                            }
                            lVarNames.Add(varNames[i]);
                        }
                    }
                }

                if (PythonVarRemoved != null)
                {
                    List<string> tVarNames = new List<string>();
                    for (int i = 0; i < lVarNames.Count; i++)
                    {
                        if (varNames.Contains(lVarNames[i]) == true)
                            tVarNames.Add(lVarNames[i]);
                        else
                            PythonVarRemoved(lVarNames[i]);
                        //ScriptingInterface.scriptingInterface.RemoveVisibleVariable(lVarNames[i]);
                    }
                    lVarNames = tVarNames;
                }
            }

        }

        private string[] GetIronPythonTypes()
        {
            string[] types = new List<string>(mPythonEngine.scope.GetVariableNames()).ToArray();
            // sort methods by name
            Array.Sort(types,
                    delegate(string methodInfo1, string methodInfo2)
                    { return methodInfo1.CompareTo(methodInfo2); });

            // write method names
            return types;
        }

        private string[] ironTextBoxControl1_AutoCompletionListFill(string ObjectName)
        {
            if (ObjectName == "")
                return GetIronPythonTypes();
            object var;
            List<string> Methods = new List<string>();
            List<string> AllInfo = new List<string>();


            mPythonEngine.scope.TryGetVariable(ObjectName, out var);
            if (var == null)
                return null;
            Type tt = var.GetType();
            FieldInfo[] fieldInfos = tt.GetFields();

            for (int i = 0; i < fieldInfos.Length; i++)
                AllInfo.Add(fieldInfos[i].Name);

            // get all public static methods of MyClass type
            MethodInfo[] methodInfos = tt.GetMethods();

            for (int i = 0; i < methodInfos.Length; i++)
            {
                string Params = "";
                foreach (ParameterInfo pi in methodInfos[i].GetParameters())
                {
                    string pp = pi.ParameterType.ToString();
                    pp = pp.Replace("System.", "");
                    Params += pp + " " + pi.Name + ", ";
                }
                if (Params.Length > 0)
                    Params = Params.Substring(0, Params.Length - 2);
                Params = methodInfos[i].Name + "(" + Params + ")";
                AllInfo.Add(Params);
            }

            string[] AllStrings = AllInfo.ToArray();

            // sort methods by name
            Array.Sort(AllStrings,
                    delegate(string methodInfo1, string methodInfo2)
                    { return methodInfo1.CompareTo(methodInfo2); });

            // write method names
            return AllStrings;

        }


    }
}
