using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using MathHelpLib;
using Tomographic_Imaging_2;
using System.Runtime.InteropServices;

namespace ScriptRunner
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern Int32 FlashWindowEx(ref FLASHWINFO pwfi);
        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public Int32 dwFlags;
            public UInt32 uCount;
            public Int32 dwTimeout;
        }
        // stop flashing
        static Int32 FLASHW_STOP = 0;

        // flash the window title 
        static Int32 FLASHW_CAPTION = 1;

        // flash the taskbar button
        static Int32 FLASHW_TRAY = 2;

        // 1 | 2
        static Int32 FLASHW_ALL = 3;

        // flash continuously 
        static Int32 FLASHW_TIMER = 4;

        // flash until the window comes to the foreground 
        static Int32 FLASHW_TIMERNOFG = 12;

        public static bool Flash(IntPtr Handle)
        {
            FLASHWINFO fw = new FLASHWINFO();

            fw.cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof(FLASHWINFO)));
            fw.hwnd = Handle;
            fw.dwFlags =FLASHW_ALL  ;
            fw.uCount = UInt32.MaxValue;

            return  FlashWindowEx(ref fw)==0;
        }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            string DataPath = "";
            Display display = new Display();
            try
            {
                Console.WriteLine(args[0]);

               
                display.Show();

                display.Caption = args[0];

                //build the file structure
                string pPath = /*"C:\\Development\\CellCT\\DataIn\\cct001_20101118_085849\\";//*/args[0].Replace("\"", "").Replace("'", "");

                if (pPath.EndsWith("\\") == false)
                    pPath += "\\";

                string dirName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(pPath));
                string[] parts = dirName.Split('_');
                string Prefix = parts[0];
                string Year = parts[1].Substring(0, 4);
                string month = parts[1].Substring(4, 2);
                string day = parts[1].Substring(6, 2);

                string basePath = args[5] + "\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\";

                Console.WriteLine(basePath);

                string tempPath = basePath + "Data\\temp\\";
                if (Directory.Exists(tempPath) == false)
                    Directory.CreateDirectory(tempPath);

                DataPath = basePath + "Data\\";
                if (Directory.Exists(DataPath) == false)
                    Directory.CreateDirectory(DataPath);

                string StackPath= basePath + "Stack\\";
                Console.WriteLine(StackPath);
                if (Directory.Exists(StackPath) == false)
                    Directory.CreateDirectory(StackPath);

                string filePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\scripts\\";

                Console.WriteLine("Has args args == " + (args != null).ToString());
                Console.WriteLine(args.Length.ToString());

                string Filtername = args[1];
                string PaddedLength = args[2];
                string CutLength = args[3];
                string DesiredScript = args[4];

                //string junk = args[5];
                int PadLength = 0;
                int iCutLength = 0;
                int CommunPort = 1100;
                int.TryParse(PaddedLength, out PadLength);
                int.TryParse(CutLength, out iCutLength);
                int.TryParse(args[6], out CommunPort);

                Console.WriteLine("Getting impulse");
                MathHelpLib.Filtering.FilterTypes FilterType = (MathHelpLib.Filtering.FilterTypes)Enum.Parse(typeof(MathHelpLib.Filtering.FilterTypes), (string)Filtername);

                Dictionary<string, object> ScriptParams = new Dictionary<string, object>();
                ScriptParams.Add("ImpulseFilter", MathHelpLib.Filtering.GetRealSpaceFilter(FilterType, PadLength, iCutLength, 1));


                Console.WriteLine(((double[])ScriptParams["ImpulseFilter"]).Length.ToString());

                string Script = DesiredScript.ToLower();
                Console.WriteLine(DesiredScript.ToLower());
                IScript ProgScript = null;

                ProgScript = new ScriptCombinedArray();


                Console.WriteLine(ProgScript.ToString());
                Console.WriteLine(FilterType.ToString());

               try
                {

                    List<string> ImagesIn = new List<string>();
                    //if this is the first run, pull the actual data
                    if (Script == "scriptrecon")
                    {

                    }
                    else
                    {
                        Console.WriteLine("getting file names ");
                        string[] PPs = Directory.GetFiles(pPath + "PP\\");
                        Console.WriteLine("Num Files = " + PPs.Length);
                        string[] Sorted = MathStringHelps.SortNumberedFiles(PPs);
                        Console.WriteLine("Sorted");
                        ImagesIn.AddRange(Sorted);
                    }
                    Console.WriteLine("Running script");
                    Tomographic_Imaging_2.ScriptRunner.RunScripts(display.picture, DesiredScript, ProgScript, ImagesIn, pPath, DataPath, true, ScriptParams,CommunPort );

                    try
                    {
                     /*   Console.WriteLine("Copying Stack");
                        string StackPath = basePath + "Stack\\";
                        if (Directory.Exists(StackPath) == false)
                            Directory.CreateDirectory(StackPath);

                        string[] StackFiles = Directory.GetFiles(pPath + "\\stack\\000\\");
                        foreach (string file in StackFiles)
                            File.Copy(file, StackPath + Path.GetFileName(file), true);
                        */
                    }
                    catch { }

                }
                catch (Exception ex)
               // Exception ex=null;
                {
                    Console.Beep();
                    if (Directory.Exists(DataPath) == false)
                        Directory.CreateDirectory(DataPath);

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(DataPath + "Comments.txt", true))
                    {

                        file.WriteLine("<ErrorMessageOut><" + ex.Message + "/>");
                    }
                    Flash(display.Handle);
                    Console.WriteLine(ex.Message);
                    if (ex.InnerException != null)
                        Console.WriteLine(ex.InnerException.Message);
                   // Console.ReadKey();
                }

                try
                {
                    Console.WriteLine("Cleaning temp files");
                    string[] eraseFiles = Directory.GetFiles(tempPath);
                }
                catch { }
            }
           catch (Exception ex)
               
            {
           //     Exception ex = null;
                Console.Beep();
                if (Directory.Exists(DataPath) == false)
                    Directory.CreateDirectory(DataPath);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(DataPath + "Comments.txt",true ))
                {
                    file.WriteLine("<ErrorMessageOut><" + ex.Message + "/>");
                }
                Flash(display.Handle);
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
               // Console.ReadKey();
            }
            //Console.WriteLine("Done Processing. Press any key");
            //Console.ReadKey();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.Beep();
            Console.Beep();
            Console.Beep();

            Exception ex=(Exception)e.ExceptionObject ;
            Console.WriteLine(  ex.Message);
            if (ex.InnerException != null)
                Console.WriteLine(ex.InnerException.Message);
            Console.ReadKey();
        }
    }
}
