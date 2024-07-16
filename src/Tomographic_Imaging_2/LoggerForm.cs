using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Threading;

    public partial class LoggerForm :DockContent 
    {
        private delegate void  LogMessageDelegate(string LogMessage);
        public LoggerForm()
        {
            InitializeComponent();
            Logger = this;
        }

        public static bool LogFullError = false;
        public static bool LogStackTrace = false;
        public static bool LogTimeStamp = true;
        public static bool LogCallingFunction = true;
        public static bool LogExceptionType = true;
        public static string MonitorFormatString = "Monitor Update Rate = {0} ms";
        private static LoggerForm Logger;
        /// <summary>
        /// Log a message to the main monitor form.  Static so it can be called by using the type anywhere in the code
        /// </summary>
        /// <param name="Message"></param>
        public static void LogMessage(string Message)
        {
            if (Logger != null)
            {
                if (LogCallingFunction)
                {
                    System.Diagnostics.StackFrame stackframe =
                    new System.Diagnostics.StackFrame(1, true);

                    Message = stackframe.GetMethod().ReflectedType.Name
                        + "."
                        + stackframe.GetMethod().Name + "  :: " + Message;
                }
                Logger.logMessage(Message);
            }
        }

        public static void LogErrorMessage(Exception ex)
        {
            if (Logger != null)
            {
                string ErrorMessage = ex.Message + "\n";
                if (ex.InnerException != null)
                    ErrorMessage += ex.InnerException.Message + "\n";

                if (LogStackTrace == true)
                    ErrorMessage += ex.StackTrace;

                if (LogCallingFunction)
                {
                    System.Diagnostics.StackFrame stackframe =
                    new System.Diagnostics.StackFrame(1, true);

                    ErrorMessage = stackframe.GetMethod().ReflectedType.Name
                        + "."
                        + stackframe.GetMethod().Name + "  :: " + ErrorMessage;
                }

                if (LogExceptionType)
                {
                    ErrorMessage += ex.GetType().ToString();
                }

                Logger.logErrorMessage(ErrorMessage);
            }
        }

        public static void LogErrorMessage(string ErrorMessage)
        {
            if (LogCallingFunction)
            {
                System.Diagnostics.StackFrame stackframe =
                new System.Diagnostics.StackFrame(1, true);

                ErrorMessage = stackframe.GetMethod().ReflectedType.Name
                    + "."
                    + stackframe.GetMethod().Name + "  :: " + ErrorMessage;
            }
            Logger.logErrorMessage(ErrorMessage);
        }

        public static void LogMonitorRate(long  milliSec)
        {
            if (Logger != null)
            {
                Logger.logMonitorRate(milliSec);
            }
        }

       
        private void logMonitorRate(long milliSec)
        {
            if (InvokeRequired)
                this.BeginInvoke(new Action<long>(logMonitorRate), milliSec);
            else
            {
                try
                {
                    monitorUpdateRateMsToolStripMenuItem.Text = string.Format( MonitorFormatString,milliSec ) ;
                }
                catch { }
            }
        }

        private void DisplayError(string ErrorMessage)
        {
            MessageBox.Show(Logger, ErrorMessage, "Cell Automation Platform Error");
        }

        private void logErrorMessage(string ErrorMessage)
        {
            if (InvokeRequired)
                this.Invoke(new Action<string>(DisplayError), ErrorMessage);
            else
            {
                DisplayError(ErrorMessage);

                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(50);
                    Application.DoEvents();
                }
                logMessage(ErrorMessage);
            }
        }

        private void logMessage(string Message)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new LogMessageDelegate(LogMessage), Message);
            }
            else
            {
                try
                {
                    string LogText = "";
                    if (LogTimeStamp)
                    {
                        DateTime now = DateTime.Now;
                        LogText  += string.Format(" {0:00}:{1:00}:{2:00}:{3:000} :: ", now.Hour, now.Minute, now.Second, now.Millisecond);
                    }
                    LogText  += Message + "\n";
                    richTextBox1.Text += LogText;
                    richTextBox1.Select(richTextBox1.Text.Length, 1);
                    richTextBox1.ScrollToCaret();
                    System.Diagnostics.Trace.Write(LogText);
                }
                catch { }
            }
        }
        

        private void saveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Filter = "Text File (*.txt) | (*.txt) | All files (*.*) | (*.*)";
                DialogResult ret = saveFileDialog1.ShowDialog();
                if (DialogResult.OK == ret)
                {
                    richTextBox1.SaveFile(saveFileDialog1.FileName);
                }
            }
            catch { }
        }

       
            
       
    }

