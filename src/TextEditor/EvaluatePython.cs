using System;
using System.Collections.Generic;
using System.Text;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using IronPython.Runtime.Exceptions;
using IronPythonEditor.TextEditor;


namespace IronPythonEditor
{
    public class EvaluatePython
    {
        public event cTextWriter.PythonSpeaksEvent PythonSpeaks;

        cTextWriter ctw = new cTextWriter();
        public  ScriptEngine engine;
        public  ScriptScope scope;

        //static EvaluatePython()
        //{}

        public EvaluatePython()
        {
            engine = Python.CreateEngine();
            scope = engine.CreateScope();
            ctw.PythonSpeaks += new cTextWriter.PythonSpeaksEvent(ctw_PythonSpeaks);

        }

        void ctw_PythonSpeaks(string Message)
        {
            if (PythonSpeaks != null)
                PythonSpeaks(Message);
        }

        public void SetOutputPath()
        {
            try
            {
                engine.Runtime.IO.SetOutput(new IPEStreamWrapper(), Encoding.UTF8);
               // Console.SetOut(ctw);
            }
            catch { }
        }

        public ScriptScope CreateScope(Dictionary<string, object> Variables)
        {
            ScriptScope scope = engine.CreateScope();
            AddVariables(scope, Variables);
            return scope;
        }

        /// <summary>
        /// Use Universal scope
        /// </summary>
        /// <param name="Variables"></param>
        public void AddVariables(Dictionary<string, object> Variables)
        {
            AddVariables(scope, Variables);
        }
        public void AddVariables(ScriptScope scope, Dictionary<string, object> Variables)
        {
            foreach (KeyValuePair<string, object> kvp in Variables)
            {
                try
                {
                    scope.SetVariable(kvp.Key, kvp.Value);
                }
                catch { }
            }

        }
        /// <summary>
        /// Use universal scope
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public void  evaluate(string code)
        {
             evaluate(scope, code);
        }

        /// <summary>
        /// Use specified scope
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public void evaluate(ScriptScope scope, string code)
        {
            scope.SetVariable("Console", ctw);
            scope.SetVariable("Executable", Application.ExecutablePath);
            scope.SetVariable("ExecutablePath", Path.GetDirectoryName(Application.ExecutablePath) + "\\");
            //scope.SetVariable("LibraryPath", Path.GetDirectoryName(Application.ExecutablePath) + "\\ImageViewer.dll");
            ScriptSource source = null;

            try
            {
                source = engine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
                source.Execute(scope);
            }
            catch (Microsoft.Scripting.SyntaxErrorException ex)
            {
                throw new PythonException(ex);
            }
            catch (Exception ex)
            {
                string pythonerror = "";
                try
                {
                    pythonerror = engine.GetService<ExceptionOperations>().FormatException(ex);
                }
                catch { }

                PythonException pe = new PythonException();
                pe.PythonErrorTraceBack = pythonerror;

                pe.Message = ex.Message;

                throw pe;

                /*LoggerForm.LogMessage(ex.Message);
                if (ex.InnerException != null)
                {
                    err += ex.InnerException.Message;
                    LoggerForm.LogMessage(ex.InnerException.Message);
                }
                //err += "\n\n" +  ex.StackTrace;
                */

                //return err;
            }
        }
    }
}
