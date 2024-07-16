using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace DoRecons
{
    public class ProcessHolder
    {

        public string Path;
        public string TempPath;
        public System.Diagnostics.Stopwatch sw = new Stopwatch();
        public Process ScriptRunner;
        public TimeSpan MaxTime = new TimeSpan(0, 30, 0);
        public ProcessHolder(Process scriptRunner,string TempPath)
        {
            this.TempPath = TempPath;
            ScriptRunner = scriptRunner;
            sw.Start();
        }
        public bool CheckTime()
        {
            try
            {
                if (sw.Elapsed > MaxTime)
                {
                    Console.WriteLine("Killed overlong script");
                    ScriptRunner.Kill();
                    //Directory.Delete(TempPath, true);
                    return false;
                }
                if (ScriptRunner.HasExited == true)
                {
                   // Directory.Delete(TempPath, true);
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public string YearMonthAndDay
        {
            get
            {
                return "";
            }
        }
    }
}
