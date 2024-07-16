using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageViewer.Filters;
using MathHelpLib;
using ImageViewer.PythonScripting;
using MathHelpLib.ImageProcessing;

namespace ImageViewer
{
    public class DataEnvironment
    {
        public ScreenProperties Screen;
        //setting both to zero causes the displaybox to do autocontrast
        public ushort MaxContrast=0;
        public ushort MinContrast=0;
        //public int MasterThreadId = 0;
        public List<int> ThreadsRunning = null;
        public List<string > WholeFileList = null;
        public ImageLibrary  AllImages = null;
        public ReplaceChatStringDictionary ProgressLog;
        public int NumberOfRunningThreads = 0;

        public string PPFolder = "";
        public string StackFolder = "";
        
        public string ExperimentFolder = "";
        public string DataOutFolder = "";
        
        public int ProcCount = 1;

        public bool RunningOnGPU=true ;
        public bool RunningThreaded = true;
        public bool FluorImage = false;
        public DictionaryThreadSafe<string, IEffectToken> EffectTokens=new DictionaryThreadSafe<string,IEffectToken>();

        public  ImageDisplayer ImageDisp;

        public Dictionary<string, string> ScriptParams;

        public ImageViewer.Filters.ReplaceStringDictionary PassData;

        public string TempPath;
        public string Executable;
        public string ExecutablePath;
    }
}
