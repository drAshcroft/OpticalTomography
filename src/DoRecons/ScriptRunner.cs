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
using DoRecons.Scripts;
using MathHelpLib.ImageProcessing;

namespace DoRecons
{
    public class ScriptRunner
    {
       // IronPythonEditor.EvaluatePython ep = new IronPythonEditor.EvaluatePython();

        /// <summary>
        /// Runs a threaded reconstruction.  ScriptParams must contain TempDirectory, DataDirectory ,  and DataOut(path to desired output folder)
        /// </summary>
        /// <param name="pImagePreview"></param>
        /// <param name="RunThreaded"></param>
        /// <param name="ImagesIn"></param>
        /// <param name="ScriptToRun"></param>
        /// <param name="ScriptParams"></param>
        /// <param name="Port"></param>
        public static void RunScripts(PictureBox pImagePreview, bool RunThreaded,string[] ImagesIn, IScript ScriptToRun, Dictionary<string,string  > ScriptParams, int Port)
        {
            MacroHelper.ErrorOnThread = false;
            ImageDisplayer id = new ImageDisplayer(pImagePreview);

            object CriticalSectionObject = new object();
            //only the last one should end up in the nice folder

            string TempDirectory = ScriptParams["TempDirectory"] + "\\";
            string DataIn = ScriptParams["DataDirectory"] + "\\";
            string DataOut = ScriptParams["DataOut"] + "\\";
            string ImageExten = ".bmp";
            if (ImageExten == "") ImageExten = ".bmp";

          

            int ProcCount;

            if (RunThreaded)
                ProcCount = 1;//(int)(Environment.ProcessorCount / 2d);
            else
                ProcCount = 1;


           


            System.IO.StreamWriter LogFile = new System.IO.StreamWriter(DataOut + "Comments.txt");

            ImageViewer.DataEnvironment dataEnvironment = new ImageViewer.DataEnvironment();
            dataEnvironment.WholeFileList = new List<string>();
            dataEnvironment.WholeFileList.AddRange(ImagesIn);
            dataEnvironment.ThreadsRunning = null;
            dataEnvironment.Screen = null;
           
            dataEnvironment.NumberOfRunningThreads = ProcCount;
            dataEnvironment.ExperimentFolder = DataIn;
            dataEnvironment.DataOutFolder = DataOut;
            dataEnvironment.ProgressLog = new ReplaceChatStringDictionary(Path.GetFileNameWithoutExtension( DataOut  ),Port );


            Dictionary<string, string> Values = EffectHelps.OpenXMLAndGetTags(dataEnvironment.ExperimentFolder + "\\info.xml", new string[] { "Color", "SPECIMEN/CellXPos", "SPECIMEN/CellYPos", "SPECIMEN/BoxWidth", "SPECIMEN/BoxHeight" });
            bool ColorImage = false;
            if (Values["Color"].ToLower() == "true")
                ColorImage = true;
            else
                ColorImage = false;

            dataEnvironment.AllImages = new ImageLibrary(ImagesIn.ToArray(), true, TempDirectory,ColorImage);

            Thread[] ProcessThreads = new Thread[ProcCount];
            dataEnvironment.ThreadsRunning = new List<int>();

            dataEnvironment.RunningThreaded  = RunThreaded;

            //break up the file list to each thread.  
            List<ImageFile>[] tImagesIn = new List<ImageFile>[ProcCount];

            for (int m = 0; m < ProcCount; m++)
                tImagesIn[m] = new List<ImageFile>();
            for (int m = 0; m < ImagesIn.Length ; m++)
                tImagesIn[m % ProcCount].Add(new ImageFile(m, ImagesIn[m]));

            //this is the first thread.  Any filter that works on the whole file list will be run from this thread

            Dictionary<string, object>[] threadVars = new Dictionary<string, object>[ProcCount];
            for (int m = 0; m < ProcCount; m++)
            {
                //prepare the variable list for the script
                Dictionary<string, object> Variables = new Dictionary<string, object>();
                Variables.Add("Scriptname", ScriptToRun.GetName());
                Variables.Add("ImageFileListIn", tImagesIn[m]);
                Variables.Add("ScriptParams", ScriptParams);
                Variables.Add("TempPath", TempDirectory);
                Variables.Add("ImageDisp", id);
                Variables.Add("DataPath", DataOut + "data\\");
                Variables.Add("Executable", Application.ExecutablePath);
                Variables.Add("ExecutablePath", Path.GetDirectoryName(Application.ExecutablePath) + "\\");
                Variables.Add("LibraryPath", Path.GetDirectoryName(Application.ExecutablePath) + "\\ImageViewer.dll");
                Variables.Add("dataEnvironment", dataEnvironment);
                Variables.Add("GlobalPassData", new ImageViewer.Filters.ReplaceStringDictionary());
                Variables.Add("ImagesInMemory", true);
                Variables.Add("PhysicalImages", true);
                Variables.Add("RunningThreaded", true);
                Variables.Add("ThreadNumber", m);

                foreach (KeyValuePair<string, string> kvp in ScriptParams)
                    Variables.Add(kvp.Key, kvp.Value);

                dataEnvironment.ThreadsRunning.Add(m);//ProcessThreads[m].ManagedThreadId);
                threadVars[m] = Variables;

            }

            bool ErrorThrown = false;
            string ErrorMessage = "";
            for (int m = 0; m < ProcCount; m++)
            {
               // Dictionary<string, object> Vars=threadVars[m];
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
                            try
                            {
                                if (ErrorMessage == "")
                                {
                                   // MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                                    ErrorMessage = PythonHelps.FormatException(ex);
                                    //ErrorMessage = ex.Message + "\n\n\n" + ex.StackTrace;
                                    System.Diagnostics.Debug.Print(ex.StackTrace);
                                    System.Diagnostics.Debug.Print("");
                                    MacroHelper.ErrorOnThread = true;
                                    ErrorThrown = true;
                                    Console.WriteLine(ErrorMessage);
                                    try
                                    {
                                        dataEnvironment.ProgressLog.AddSafe("Error", ErrorMessage);
                                    }
                                    catch { }
                                    //kill all the other threads (usually they are stuck in a join)
                                    for (int i = 0; i < ProcessThreads.Length; i++)
                                    {
                                        if (Thread.CurrentThread.ManagedThreadId != ProcessThreads[i].ManagedThreadId)
                                            ProcessThreads[i].Abort();
                                    }
                                    Thread.Sleep(20000);
                                }
                            }
                            catch { }
                        }
                    }

                }
                );
            }

            dataEnvironment.ProgressLog.AddSafe("Starting", "True");
            
            for (int m = 0; m < ProcCount; m++)
            {
                Dictionary<string, object> Variables = threadVars[m];
                ProcessThreads[m].Start(Variables);
                //IScript tests = ScriptToRun.CloneScript();
                //tests.RunScript(Variables);
            }

            
          

            //do the join without locking up the UI thread
            int ThreadsFinished = 0;
           // try
            {
                while (ThreadsFinished < ProcCount && ErrorThrown ==false )
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

            try
            {
                dataEnvironment.ProgressLog.AddSafe("Finished All Threads", "true");
            }
            catch { }

            LogFile.Close();

            try
            {
                using (System.IO.StreamWriter file = new StreamWriter(DataOut + "data\\comments.txt", false))
                {
                    foreach (KeyValuePair<string, object> kvp in dataEnvironment.ProgressLog)
                    {
                        file.WriteLine("<" + kvp.Key + "><" + kvp.Value + "/>");
                    }

                    if (ErrorThrown)
                        file.WriteLine("<ErrorMessage><" + ErrorMessage.Replace("<", "").Replace(">", "") + "/>");
                    else
                        file.WriteLine("<ErrorMessage><Succeeded/>");
                }
            }
            catch { }

            if (ErrorThrown)
                throw new Exception(ErrorMessage);
        }

       

      /*  private static void RunScriptsIron(int ScriptIndex, int length, string ScriptName, string Script, List<string> ImagesIn, string pPath, string DataPath, bool RunThreaded)
        {
            MacroHelper.ErrorOnThread = false;
            //ImageDisplayer id = new ImageDisplayer(pImagePreview);

            Dictionary<string, object> ScriptParams = new Dictionary<string, object>();
            string ImageOutPath;
            string ImageOutFilename;

            object CriticalSectionObject = new object();
            //only the last one should end up in the nice folder
            if (ScriptIndex == length - 1)
            {
                ImageOutPath = Path.GetDirectoryName(pPath) + "\\ProcessedPPs\\";
                ImageOutFilename = "CorrectedPPs";
            }
            else
            {
                ImageOutPath = Path.GetDirectoryName(pPath) + "\\ProcessedPPs\\temp\\";
                ImageOutFilename = "temp" + ScriptIndex.ToString() + "PPs";
            }

            string TempDirectory = Path.GetDirectoryName(pPath) + "\\ProcessedPPs\\temp\\";
            string ImageExten = ".bmp";
            if (ImageExten == "") ImageExten = ".bmp";

           

            ImageViewer.DataEnvironment dataEnvironment = new ImageViewer.DataEnvironment();
            dataEnvironment.WholeFileList = new List<string>();
            dataEnvironment.WholeFileList.AddRange(ImagesIn);
            dataEnvironment.ThreadsRunning = null;
            dataEnvironment.Screen = null;

            int ProcCount;

            if (RunThreaded)
                ProcCount = 2 * Environment.ProcessorCount;
            else
                ProcCount = 1;

            Thread[] ProcessThreads = new Thread[ProcCount];
            dataEnvironment.ThreadsRunning = new List<int>();

            //break up the file list to each thread.  
            List<ImageFile>[] tImagesIn = new List<ImageFile>[ProcCount];
            for (int m = 0; m < ProcCount; m++)
                tImagesIn[m] = new List<ImageFile>();
            for (int m = 0; m < ImagesIn.Count; m++)
                tImagesIn[m % ProcCount].Add(new ImageFile(m, ImagesIn[m]));

            //this is the first thread.  Any filter that works on the whole file list will be run from this thread
            int MasterThreadId = 0;
            Dictionary<string, object>[] threadVars = new Dictionary<string, object>[ProcCount];

            StreamReader streamReader = new StreamReader(Script);
            string ScriptText = streamReader.ReadToEnd();
            streamReader.Close();

            bool ErrorThrown = false;

            for (int m = 0; m < ProcCount; m++)
            {
                //ironpython can work threaded if each thread has its own scope.  may still be a little unstable
                ProcessThreads[m] = new Thread(delegate(object Vars)
                {
                    try
                    {
                        //  ep.evaluate(ep.CreateScope((Dictionary<string, object>)Vars), ScriptText);
                    }
                    catch (IronPythonEditor.TextEditor.PythonException pex)
                    {
                        lock (CriticalSectionObject)
                        {
                            if (MacroHelper.ErrorOnThread == false)
                            {
                                MacroHelper.ErrorOnThread = true;
                                Thread.Sleep(100);
                                Common.SendNetworkPacket  ("Error in Script", pex.PythonErrorTraceBack);
                                ErrorThrown = true;
                                using (System.IO.StreamWriter file = new System.IO.StreamWriter(DataPath + "Comments.txt"))
                                {
                                    file.WriteLine(ScriptName + "\n\n" + pex.PythonErrorTraceBack);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (CriticalSectionObject)
                        {
                            if (MacroHelper.ErrorOnThread == false)
                            {
                                MacroHelper.ErrorOnThread = true;
                                Thread.Sleep(100);
                                Common.SendNetworkPacket("Error in Script", ex.Message);
                                ErrorThrown = true;
                                using (System.IO.StreamWriter file = new System.IO.StreamWriter(DataPath + "Comments.txt"))
                                {
                                    file.WriteLine(ScriptName + "\n\n" + ex.Message);
                                }
                            }
                        }
                    }
                });

                //prepare the variable list for the script
                Dictionary<string, object> Variables = new Dictionary<string, object>();
                Variables.Add("ImageFileListIn", tImagesIn[m]);
                Variables.Add("ScriptParams", ScriptParams);
                Variables.Add("ImageOutPath", ImageOutPath);
                Variables.Add("ImageOutExten", ImageExten);
                Variables.Add("ImageOutFileName", ImageOutFilename);
                Variables.Add("TempPath", TempDirectory);
                //Variables.Add("ImageDisp", id);
                Variables.Add("DataPath", DataPath);
                Variables.Add("Executable", Application.ExecutablePath);
                Variables.Add("ExecutablePath", Path.GetDirectoryName(Application.ExecutablePath) + "\\");
                Variables.Add("LibraryPath", Path.GetDirectoryName(Application.ExecutablePath) + "\\ImageViewer.dll");
                Variables.Add("dataEnvironment", dataEnvironment);
                Variables.Add("GlobalPassData", new ImageViewer.Filters.ReplaceStringDictionary());

                Variables.Add("RunningThreaded", true);
                Variables.Add("StartImageIndex", 0);
                Variables.Add("ThisThreadId", ProcessThreads[m].ManagedThreadId);

                //the master thread should just be the first one.  It has no physical meaning, just helps to organize 
                //single threaded operations
                if (m == 0)
                {
                    MasterThreadId = ProcessThreads[0].ManagedThreadId;
                    Variables.Add("MasterThreadID", MasterThreadId);
                    // dataEnvironment.MasterThreadId = MasterThreadId;
                }
                else
                {
                    Variables.Add("MasterThreadID", MasterThreadId);
                }
                dataEnvironment.ThreadsRunning.Add(ProcessThreads[m].ManagedThreadId);

                threadVars[m] = Variables;
            }

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

            if (ErrorThrown)
                throw new Exception();

        }*/
    }
}
