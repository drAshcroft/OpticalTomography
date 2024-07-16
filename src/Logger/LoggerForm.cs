using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Logger
{
    public delegate void LogMessageRecievedEvent(string Message);
    public class LoggerForm
    {
        public static event LogMessageRecievedEvent LogMessageRecieved;

        public static bool LogFullError = false;
        public static bool LogStackTrace = false;
        public static bool LogTimeStamp =false ;
        public static bool LogCallingFunction = false ;
        public static bool LogExceptionType = true;
        public static string MonitorFormatString = "Monitor Update Rate = {0} ms";

        public static Form mOwner;
        /// <summary>
        /// Log a message to the main monitor form.  Static so it can be called by using the type anywhere in the code
        /// </summary>
        /// <param name="Message"></param>
        public static void LogMessage(string Message)
        {
            if (LogMessageRecieved != null)
            {
                if (LogCallingFunction)
                {
                    System.Diagnostics.StackFrame stackframe =
                    new System.Diagnostics.StackFrame(1, true);

                    Message = stackframe.GetMethod().ReflectedType.Name
                        + "."
                        + stackframe.GetMethod().Name + "  :: " + Message;
                }
                LogMessageRecieved("\n" + Message);
            }
            else
                System.Diagnostics.Debug.Print(Message);
        }

        public static void LogErrorMessage(Exception ex)
        {
            if (LogMessageRecieved != null)
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

                LogMessageRecieved("\n" + ErrorMessage);
            }
            else
                System.Diagnostics.Debug.Print(ex.Message );

        }

        public static void LogErrorMessage(string ErrorMessage)
        {
            if (LogCallingFunction && LogMessageRecieved!=null)
            {
                System.Diagnostics.StackFrame stackframe =
                new System.Diagnostics.StackFrame(1, true);

                ErrorMessage = stackframe.GetMethod().ReflectedType.Name
                    + "."
                    + stackframe.GetMethod().Name + "  :: " + ErrorMessage;
            }
            else
                System.Diagnostics.Debug.Print(ErrorMessage);

            if (LogMessageRecieved != null)
                LogMessageRecieved("\n" + ErrorMessage);
            else
                System.Diagnostics.Debug.Print("\n" + ErrorMessage);
        }

        public static void LogMonitorRate(long milliSec)
        {
            if (LogMessageRecieved != null)
            {
                LogMessageRecieved("\n" + milliSec.ToString());
            }
            else
                System.Diagnostics.Debug.Print(milliSec.ToString());

        }
    }
}
