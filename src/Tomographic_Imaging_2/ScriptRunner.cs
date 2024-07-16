using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageViewer;
using ImageViewer.PythonScripting;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ImageViewer.Filters;

namespace Tomographic_Imaging_2
{
    public class ScriptRunner
    {
       

        public static void RunScripts(PictureBox pImagePreview, string ScriptName, IScript ScriptToRun, List<string> ImagesIn, string ExperimentPath, string DataPath, bool RunThreaded, Dictionary<string, object> ScriptParams, int Port)
        {
            MacroHelper.ErrorOnThread = false;
            ImageDisplayer id = new ImageDisplayer(pImagePreview);

            object CriticalSectionObject = new object();
            //only the last one should end up in the nice folder

            string TempDirectory = Path.GetDirectoryName(DataPath) + "\\temp\\";
            string ImageExten = ".bmp";
            if (ImageExten == "") ImageExten = ".bmp";

         

            int ProcCount;
            //RunThreaded = false;
            if (RunThreaded)
                ProcCount =6;// Environment.ProcessorCount;
            else
                ProcCount = 1;

            System.IO.StreamWriter LogFile = new System.IO.StreamWriter(DataPath + "Comments.txt");

            ImageViewer.DataEnvironment dataEnvironment = new ImageViewer.DataEnvironment();
            dataEnvironment.WholeFileList = new List<string>();
            dataEnvironment.WholeFileList.AddRange(ImagesIn);
            dataEnvironment.ThreadsRunning = null;
            dataEnvironment.Screen = null;
            dataEnvironment.AllImages = new ImageLibrary(ImagesIn.ToArray(), true, TempDirectory);
            dataEnvironment.NumberOfRunningThreads = ProcCount;
            dataEnvironment.ExperimentFolder = ExperimentPath;
            dataEnvironment.DataOutFolder = Directory.GetParent(Directory.GetParent(DataPath).FullName).FullName;
            dataEnvironment.ProgressLog = new ReplaceChatStringDictionary(Path.GetFileNameWithoutExtension( Path.GetDirectoryName(ExperimentPath)),Port );

            Thread[] ProcessThreads = new Thread[ProcCount];
            dataEnvironment.ThreadsRunning = new List<int>();

            //break up the file list to each thread.  
            List<ImageFile>[] tImagesIn = new List<ImageFile>[ProcCount];

            for (int m = 0; m < ProcCount; m++)
                tImagesIn[m] = new List<ImageFile>();
            for (int m = 0; m < ImagesIn.Count; m++)
                tImagesIn[m % ProcCount].Add(new ImageFile(m, ImagesIn[m]));

            //this is the first thread.  Any filter that works on the whole file list will be run from this thread

            Dictionary<string, object>[] threadVars = new Dictionary<string, object>[ProcCount];

            bool ErrorThrown = false;
            string ErrorMessage = "";
            for (int m = 0; m < ProcCount; m++)
            {

                ProcessThreads[m] = new Thread(delegate(object Vars)
                {
                    try
                    {
                        Dictionary<string, object> sVariables = (Dictionary<string, object>)Vars;
                        IScript tests = ScriptToRun.CloneScript();
                        tests.RunScript(sVariables);
                    }
                    catch (Exception ex)
                    {
                        lock (CriticalSectionObject)
                        {
                            if (ErrorMessage == "")
                            {
                                ErrorMessage = ex.Message + "\n" + ex.StackTrace;
                                System.Diagnostics.Debug.Print(ex.StackTrace);
                                System.Diagnostics.Debug.Print("");
                                MacroHelper.ErrorOnThread = true;
                                ErrorThrown = true;
                                Console.WriteLine(ErrorMessage);
                                dataEnvironment.ProgressLog.AddSafe("Error", ErrorMessage);
                            }
                        }
                    }

                });
            }

            for (int m = 0; m < ProcCount; m++)
            {
                //prepare the variable list for the script
                Dictionary<string, object> Variables = new Dictionary<string, object>();
                Variables.Add("Scriptname", ScriptName);
                Variables.Add("ImageFileListIn", tImagesIn[m]);
                Variables.Add("ScriptParams", ScriptParams);
                Variables.Add("TempPath", TempDirectory);
                Variables.Add("ImageDisp", id);
                Variables.Add("DataPath", DataPath);
                Variables.Add("Executable", Application.ExecutablePath);
                Variables.Add("ExecutablePath", Path.GetDirectoryName(Application.ExecutablePath) + "\\");
                Variables.Add("LibraryPath", Path.GetDirectoryName(Application.ExecutablePath) + "\\ImageViewer.dll");
                Variables.Add("dataEnvironment", dataEnvironment);
                Variables.Add("GlobalPassData", new ImageViewer.Filters.ReplaceStringDictionary());
                Variables.Add("ImagesInMemory", true);
                Variables.Add("PhysicalImages", true);
                Variables.Add("RunningThreaded", true);
                Variables.Add("ThreadNumber", m);

                dataEnvironment.ThreadsRunning.Add(m);//ProcessThreads[m].ManagedThreadId);
                threadVars[m] = Variables;
            }
            dataEnvironment.ProgressLog.AddSafe("Starting", "True");
            for (int m = 0; m < ProcCount; m++)
            {
                Dictionary<string, object> Variables = threadVars[m];
                ProcessThreads[m].Start(Variables);
            }

            //do the join without locking up the UI thread
            int ThreadsFinished = 0;
            //try
            {
                while (ThreadsFinished < ProcCount)
                {
                    ThreadsFinished = 0;
                    for (int m = 0; m < ProcCount; m++)
                    {
                        if (ProcessThreads[m].ThreadState == ThreadState.Stopped)
                            ThreadsFinished++;
                    }
                    Application.DoEvents();
                }
            }

            dataEnvironment.ProgressLog.AddSafe("Finished All Threads", "true");

            LogFile.Close();

            using (System.IO.StreamWriter file = new StreamWriter(DataPath + "comments.txt", false))
            {
                foreach (KeyValuePair<string, object> kvp in dataEnvironment.ProgressLog)
                {
                    file.WriteLine("<" + kvp.Key + "><" + kvp.Value + "/>");
                }

                if (ErrorThrown)
                    file.WriteLine("<ErrorMessage><" + ErrorMessage + "/>");
                else
                    file.WriteLine("<ErrorMessage><Succeeded/>");
            }

            

            

            if (ErrorThrown)
                throw new Exception(ErrorMessage);


        }

       
    }
}
