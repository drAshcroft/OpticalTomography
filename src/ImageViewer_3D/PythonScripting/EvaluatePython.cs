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
using Logger;
using IronPython.Runtime.Exceptions;

namespace ImageViewer3D.PythonScripting
{
    internal class EvaluatePython
    {
        cTextWriter ctw = new cTextWriter();
        public static  ScriptEngine engine;
        public static  ScriptScope scope;

        static EvaluatePython()
        {
            engine = Python.CreateEngine();
            scope = engine.CreateScope();
        }
        public EvaluatePython()
        {
            
        }

        public void SetOutputPath()
        {
            try
            {
                engine.Runtime.IO.SetOutput( new IPEStreamWrapper( ) , Encoding.UTF8 );
                Console.SetOut(ctw);
            }
            catch { }
        }

        public void AddVariables(Dictionary<string, object> Variables)
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

        public string evaluate(string code)
        {
            scope.SetVariable("Console", ctw);
            scope.SetVariable("Executable", Application.ExecutablePath);
            scope.SetVariable("ExecutablePath",Path.GetDirectoryName( Application.ExecutablePath) + "\\");
            scope.SetVariable("LibraryPath", Path.GetDirectoryName(Application.ExecutablePath) + @"\ImageViewer3Dr.dll");
            ScriptSource source=null;
            
            try
            {
                source = engine.CreateScriptSourceFromString(code, SourceCodeKind.Statements );
                source.Execute(scope);
            }
            catch (Microsoft.Scripting.SyntaxErrorException ex)
            {
                string err = ex.Message + "\nLine:" + ex.Line + "\nColumn" + ex.Column + "\n" + ex.SourceCode;
                LoggerForm.LogMessage(err);
                return err;
            }
            catch (Exception ex)
            {
                string pythonerror="";
                try
                {
                    pythonerror  = engine.GetService<ExceptionOperations>().FormatException(ex);
                }
                catch { }
                
                string err = pythonerror + "\n\n" + ex.Message;
                LoggerForm.LogMessage(ex.Message);
                if (ex.InnerException != null)
                {
                    err += ex.InnerException.Message;
                    LoggerForm.LogMessage(ex.InnerException.Message);
                }
                err += "\n\n" +  ex.StackTrace;
                

                return err;
            }
           
           // Func<PythonTuple> exc_info = engine.Operations.GetMember<Func<PythonTuple>>(engine.GetSysModule(), "exc_info");
           // TraceBack tb = (TraceBack)exc_info()[2];
            return "";
        }
    }
}
