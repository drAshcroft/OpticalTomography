using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageViewer3D.Filters;
using System.IO;
using System.Threading;
using ImageViewer.PythonScripting;
using ImageViewer;
using ImageViewer.Filters;

namespace ImageViewer3D.PythonScripting
{
    public partial class MacroRecorder3D : Form,IEffect3D 
    {

        #region formSetup

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem stopRecordingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startRecordingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem batchProcessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem threadedBatchProcessToolStripMenuItem;

       
        private System.Windows.Forms.SplitContainer splitContainer1;

        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.startRecordingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopRecordingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.threadedBatchProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 1);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
           
            this.splitContainer1.Size = new System.Drawing.Size(931, 321);
            this.splitContainer1.SplitterDistance = 723;
            this.splitContainer1.TabIndex = 3;
            // 
            // startRecordingToolStripMenuItem
            // 
            this.startRecordingToolStripMenuItem.Name = "startRecordingToolStripMenuItem";
            this.startRecordingToolStripMenuItem.Size = new System.Drawing.Size(100, 20);
            this.startRecordingToolStripMenuItem.Text = "Start Recording";
            this.startRecordingToolStripMenuItem.Click += new System.EventHandler(this.startRecordingToolStripMenuItem_Click);
            // 
            // stopRecordingToolStripMenuItem
            // 
            this.stopRecordingToolStripMenuItem.Name = "stopRecordingToolStripMenuItem";
            this.stopRecordingToolStripMenuItem.Size = new System.Drawing.Size(100, 20);
            this.stopRecordingToolStripMenuItem.Text = "Stop Recording";
            this.stopRecordingToolStripMenuItem.Click += new System.EventHandler(this.stopRecordingToolStripMenuItem_Click);
            // 
            // batchProcessToolStripMenuItem
            // 
            this.batchProcessToolStripMenuItem.Name = "batchProcessToolStripMenuItem";
            this.batchProcessToolStripMenuItem.Size = new System.Drawing.Size(92, 20);
            this.batchProcessToolStripMenuItem.Text = "Batch Process";
            this.batchProcessToolStripMenuItem.Click += new System.EventHandler(this.batchProcessToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // threadedBatchProcessToolStripMenuItem
            // 
            this.threadedBatchProcessToolStripMenuItem.Name = "threadedBatchProcessToolStripMenuItem";
            this.threadedBatchProcessToolStripMenuItem.Size = new System.Drawing.Size(145, 20);
            this.threadedBatchProcessToolStripMenuItem.Text = "Threaded Batch Process";
            this.threadedBatchProcessToolStripMenuItem.Click += new System.EventHandler(this.threadedBatchProcessToolStripMenuItem_Click);
            // 
            // textEditorForm1
            // 
           
            // 
            // MacroRecorder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 485);
            this.Controls.Add(this.splitContainer1);
            
            this.Name = "MacroRecorder";
            this.Text = "Macro Recorder";
            this.Load += new System.EventHandler(this.MacroRecorder_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        #region MenuStuff
        public string EffectName { get { return "Record Macro"; } }
        public string EffectMenu { get { return "Macros"; } }
        public string EffectSubMenu { get { return ""; } }
        public int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        public object[] DefaultProperties
        {
            get { return new object[] { Header + "\n\n" + Footer }; }
        }

        public string[] ParameterList
        {
            get { return new string[] { "Script_Text|string" }; }
        }
        #endregion

        #region Internal Vars


        protected object[] mFilterToken;
        protected DataEnvironment3D mDataEnvironment;
        protected Bitmap  mScratchImage;
        protected ImageViewer.Filters.ReplaceStringDictionary mPassData;
        #endregion

        #region effect stuff

        protected object doEffect(ImageViewer3D.DataEnvironment3D dataEnvironment, object SourceImage, ImageViewer.Filters.ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mDataEnvironment = dataEnvironment;
            MacroRecorder3D mr = new MacroRecorder3D();
            mr.ShowInterface(mOwner);
            mr.Show(mOwner);
            mr.doSetup(dataEnvironment, Parameters);
            return null;
        }

        private void doSetup(DataEnvironment3D dataEnvironment, object[] MacroToken)
        {
            mDataEnvironment = dataEnvironment;
            mFilterToken = MacroToken;
            if (mFilterToken == null)
            {
                mFilterToken = DefaultProperties;
            }

            if (textEditorForm1.Text.Trim() == "")
            {
                textEditorForm1.Text = Header + "\n\n" + Footer;
            }
            this.Text = Path.GetFileNameWithoutExtension((string)MacroToken[0]);
            if (((bool)mFilterToken[1]) == true)
            {
                textEditorForm1.OpenFile((string)mFilterToken[0]);
            }
            else
                textEditorForm1.Text = (string)mFilterToken[0];

            try
            {
                myViewer = dataEnvironment.ViewerControl;
                EventListener = new MacroLineGeneratedEvent(MacroLineGenerated);
                dataEnvironment.RegisterMacroListener(EventListener);
            }
            catch { }
        }

        IWin32Window mOwner;
        public void ShowInterface(IWin32Window Owner)
        {
            mOwner = Owner;
        }

        public DataHolder DoEffect(DataEnvironment3D dataEnvironment, DataHolder SourceImage, ImageViewer.Filters.ReplaceStringDictionary PassData, params object[] Parameters)
        {
           
            //set up all the persistant properties for the UI to work
            mDataEnvironment = dataEnvironment;
            mFilterToken = Parameters;
            if (mFilterToken == null)
                mFilterToken = DefaultProperties;
            mPassData = PassData;
            doEffect(dataEnvironment, SourceImage, PassData, Parameters);
            return null;
        }

        public string getMacroString()
        {
            return "";
        }

        public virtual bool PassesPassData
        { get { return false; } }
        public virtual string PassDataDescription
        {
            get { return ""; }
        }
        
        public ImageViewer.Filters.ReplaceStringDictionary PassData
        {
            get { return mPassData; }
            set { mPassData = value; }
        }

        #endregion

        ViewerControl3D myViewer;

        #region Initialize
        public MacroRecorder3D()
            : base()
        {
            InitializeComponent();

            textEditorForm1.AddMenuItem(startRecordingToolStripMenuItem);
            textEditorForm1.AddMenuItem(stopRecordingToolStripMenuItem);
            textEditorForm1.AddMenuItem(batchProcessToolStripMenuItem);
            textEditorForm1.AddMenuItem(threadedBatchProcessToolStripMenuItem);

            Header = @"
def ProcessImage(dataEnvironment, ImageNumber, BitmapImage):
    PassData=GlobalPassData";

            Footer = @"
    #Macro Add Point
    return BitmapImage

#this is called once before all the images are looped through
#it is good for opening array lists and log files
def PreBatchProcess(dataEnvironment, ImageFileList, ParamDictionary):
    #this is just to make the script functions easier, if needed convert to an image from ImageFileList
    BitmapImage=Bitmap(10,10)
	PassData=GlobalPassData
    
    return 0

#this is called once after the whole set has been processed
def PostBatchProcess(dataEnvironment, FirstImageFileList, OutFileList, ParamDictionary):
    #this is just to make the script functions easier, if needed convert to an image from ImageFileList
    BitmapImage=Bitmap(10,10)    
	PassData=GlobalPassData
    
    return 0

def BatchLoopThroughImagesSave(IProcessFunction, dataEnvironment, ImageFileList, ParamDictionary, OutDirectory, OutFilename, OutExtension):
    FileListOut=[]
    imageIndex =StartImageIndex
    for CurrentParticle in range(ImageFileList.Count):
        print(ImageFileList[CurrentParticle])
        image = ImagingTools.Load_Bitmap(ImageFileList[CurrentParticle].Filename)
        imageIndex=ImageFileList[CurrentParticle].Index
        image = IProcessFunction(dataEnvironment, imageIndex, image)
        PythonScripting.MacroHelper.DoEvents()
        fileout = OutDirectory + OutFilename + String.Format('{0:000}',  imageIndex) + OutExtension
        print(fileout)
        FileListOut.append(fileout)
        ImagingTools.Save_Bitmap(fileout,image)
        ImageDisp.DisplayVolume(imageIndex,image)
        if PythonScripting.MacroHelper.DoEvents()==True:
            exit()
    return FileListOut

def BatchLoopThroughImages(IProcessFunction, dataEnvironment, ImageFileList, ParamDictionary):
    FileListOut=[]
    imageIndex=StartImageIndex
    for CurrentParticle in range(ImageFileList.Count):
        print(ImageFileList[CurrentParticle])
        image = ImagingTools.Load_Bitmap(ImageFileList[CurrentParticle].Filename)
        imageIndex=ImageFileList[CurrentParticle].Index
        image = IProcessFunction(dataEnvironment,imageIndex,image)
        FileListOut.append(ImageFileList[CurrentParticle])
        ImageDisp.DisplayVolume(imageIndex,image)
        if PythonScripting.MacroHelper.DoEvents()==True:
            exit()
    return FileListOut

#this is the main section.  It calls process, loops through all the images, and then calls 
#post process.  
import clr
clr.AddReference('System.Drawing')
from System import *
from System.Drawing import *
from System.Drawing.Imaging import *
clr.AddReferenceToFileAndPath(LibraryPath)
from ImageViewer3DFloat import *

PreBatchProcess(dataEnvironment, ImageFileListIn, ScriptParams )

OutFileList = BatchLoopThroughImagesSave(ProcessImage,dataEnvironment, ImageFileListIn, ScriptParams, ImageOutPath, ImageOutFileName, ImageOutExten)

PostBatchProcess(dataEnvironment, ImageFileListIn, OutFileList, ScriptParams)";

            Header = Tabbify(Header);
            Footer = Tabbify(Footer);


            this.ControlBox = true;
        }
        string Tabbify(string Code)
        {
            return Code.Replace("    ", "\t");
        }
        string Header;
        string Footer;
        #endregion

        #region AddLines
        MacroLineGeneratedEvent EventListener;
        bool AcceptInput = true;
        void MacroLineGenerated(string Macroline)
        {
            if (AcceptInput)
                AddLine(Macroline);
        }

        List<string> MacroItems = new List<string>();
        public void ClearAll()
        {
            MacroItems = new List<string>();
            textEditorForm1.Text = "";
        }
        public void AddLine(string Text)
        {
            MacroItems.Add(Text);
            string newLines = "";
            string[] Lines = Text.Split('\n');
            for (int j = 0; j < Lines.Length; j++)
                newLines += "\t" + Lines[j] + "\n";

            string test = textEditorForm1.Text;
            //check to make sure that add point still exists
            if (test.Contains("#Macro Add Point") == true)
            {
                newLines += "\t#Macro Add Point";
                if (newLines.StartsWith("\t") == true)
                    newLines = newLines.Substring(1, newLines.Length - 1);
                test = test.Replace("#Macro Add Point", newLines);
            }
            else //otherwise just add  it to the end
            {
                if (test.Contains("\treturn BitmapImage") == true)
                {
                    newLines += "\treturn BitmapImage";
                    test = test.Replace("\treturn BitmapImage", newLines);
                }
                else
                    test += newLines + "\n\t#Macro Add Point\n\treturn BitmapImage\n";
            }
            textEditorForm1.Text = test;

        }
        #endregion

        #region MenuItems

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //handle the motion of the mouse to produce a verticle splitter control
                splitContainer1.Height = e.Y - 5;
                ironPythonConsole1.Top = e.Y + 5;
                ironPythonConsole1.Height = this.Height - (e.Y + 5);
            }
        }

        private void stopRecordingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AcceptInput = false;
            mDataEnvironment.RemoveMacroListener(EventListener);
        }

        private void startRecordingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AcceptInput = true;
            // mDataEnvironment[0].RegisterMacroListener(EventListener);
        }


        #endregion


       // private IronPythonEditor.EvaluatePython mPythonEngine = new EvaluatePython();
        private void batchProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MacroHelper3D.ErrorOnThread = false;
            OpenFileDialogExplain ofd = new OpenFileDialogExplain("Please choose all the input files for the batch processing", true);

            SaveFileDialogExplain sfd = new SaveFileDialogExplain(
"Please choose a directory and filename for the output.  Files will be outputed with an added index, i.e.  If you give c:\\test.bmp, the program will output c;\\test0.bmp, c:\\test1.bmp ...");
            //DialogResult ret = ofd.ShowDialog();
            DialogResult ret = DialogResult.OK;
            if (ret == DialogResult.OK)
            {
               // EvaluatePython ep = new EvaluatePython();
              //  ep.PythonSpeaks += new IronPythonEditor.TextEditor.cTextWriter.PythonSpeaksEvent(ep_PythonSpeaks);

                List<ImageFile> ImagesIn = new List<ImageFile>();

                ImageDisplayer3D id = new ImageDisplayer3D(mDataEnvironment);
                string[] filenames = Directory.GetFiles(@"C:\Development\CellCT\DataIn\cct001_20110426_085008\pp\");
                //string[] filenames = ofd.MSDialog.FileNames;
                string[] Sorted = MathHelpLib.MathStringHelps . SortNumberedFiles(filenames);
                for (int i = 0; i < Sorted.Length; i++)
                    ImagesIn.Add(new ImageFile(i, Sorted[i]));
                mDataEnvironment.WholeFileList = new List<string>();
                mDataEnvironment.WholeFileList.AddRange(Sorted);
                Dictionary<string, object> ScriptParams = new Dictionary<string, object>();
                string DirOut = @"C:\Development\CellCT\DataIn\cct001_20110426_085008\data\temp\temp.bmp";
                //ret = sfd.ShowDialog();
                if (ret == DialogResult.OK)
                {
                    //clear out the arrays so they are new each time the script is run
                   // if (PythonScripting.Arrays.CreateGlobalArrayTool.ScriptGlobalArrays != null)
                    //    PythonScripting.Arrays.CreateGlobalArrayTool.ScriptGlobalArrays.Clear();

                    string ImageOutPath = Path.GetDirectoryName(DirOut) + "\\";
                    string ImageOutFilename = Path.GetFileNameWithoutExtension(DirOut);
                    string TempDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\Temp\\";
                    string ImageExten = Path.GetExtension(DirOut);
                    if (ImageExten == "") ImageExten = ".bmp";

                    //prepare the variable list for the script
                    Dictionary<string, object> Variables = new Dictionary<string, object>();
                    Variables.Add("ImageFileListIn", ImagesIn);
                    Variables.Add("ScriptParams", ScriptParams);
                    Variables.Add("ImageOutPath", ImageOutPath);
                    Variables.Add("ImageOutExten", ImageExten);
                    Variables.Add("ImageOutFileName", ImageOutFilename);
                    Variables.Add("TempPath", TempDirectory);
                    Variables.Add("ImageDisp", id);
                    Variables.Add("dataEnvironment", mDataEnvironment);
                    Variables.Add("GlobalPassData", new ReplaceStringDictionary());
                    Variables.Add("DataPath", TempDirectory);
                    Variables.Add("ThisThreadId", Thread.CurrentThread.ManagedThreadId);
                    Variables.Add("RunningThreaded", false);
                    Variables.Add("MasterThreadID", Thread.CurrentThread.ManagedThreadId);
                    Variables.Add("StartImageIndex", 0);
                    Variables.Add("LibraryPath", Path.GetDirectoryName(Application.ExecutablePath ) + "\\ImageViewer3DFloat.dll");
                    //ep.AddVariables(Variables);

                    string code = @"";
                    try
                    {
                       // ep.evaluate(textEditorForm1.Text + "\n\n" + Tabbify(code));
                        MessageBox.Show("Batch Processing is finished");
                    }
                   /* catch (PythonException pe)
                    {
                        ironPythonConsole1.LogMessageRecieved(pe.PythonErrorTraceBack );
                        textEditorForm1.SetCaretPosition(pe.LineNumber, pe.ColNumber);
                    }*/
                    catch (Exception ex)
                    {
                        ironPythonConsole1.LogMessageRecieved(ex.Message);
                        if (ex.InnerException != null)
                            ironPythonConsole1.LogMessageRecieved(ex.InnerException.Message);
                    }
                }
            }
        }

        private void threadedBatchProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MacroHelper3D.ErrorOnThread = false;
            OpenFileDialogExplain ofd = new OpenFileDialogExplain("Please choose all the input files for the batch processing", true);

            SaveFileDialogExplain sfd = new SaveFileDialogExplain(
"Please choose a directory and filename for the output.  Files will be outputed with an added index, i.e.  If you give c:\\test.bmp, the program will output c;\\test0.bmp, c:\\test1.bmp ...");
            //DialogResult ret = ofd.ShowDialog();
            DialogResult ret = DialogResult.OK;
            if (ret == DialogResult.OK)
            {
               // EvaluatePython ep = new EvaluatePython();
               // ep.PythonSpeaks += new IronPythonEditor.TextEditor.cTextWriter.PythonSpeaksEvent(ep_PythonSpeaks);
                List<ImageFile> ImagesIn = new List<ImageFile>();

                ImageDisplayer3D id = new ImageDisplayer3D(mDataEnvironment);
                string[] filenames = Directory.GetFiles(@"C:\Development\CellCT\DataIn\cct001_20110426_085008\data\temp\");
                // string[] filenames = Directory.GetFiles(@"C:\Development\CellCT\DataIn\cct001_20110426_085008\PP\");
                //string[] filenames = ofd.MSDialog.FileNames;
                string[] Sorted = MathHelpLib.MathStringHelps.SortNumberedFiles(filenames);
                for (int i = 0; i < Sorted.Length; i++)
                    ImagesIn.Add(new ImageFile(i, Sorted[i]));

                //a list of the original files is loaded here so that the global functions can get at it.  I would prefer that this is 
                //done in a different way as it is sloppy.  
                mDataEnvironment.WholeFileList = new List<string>();
                mDataEnvironment.WholeFileList.AddRange(Sorted);

                Dictionary<string, object> ScriptParams = new Dictionary<string, object>();

                //ret = sfd.ShowDialog();
                if (ret == DialogResult.OK)
                {
                    //clear out the arrays so they are new each time the script is run
                    //if (PythonScripting.Arrays.CreateGlobalArrayTool.ScriptGlobalArrays != null)
                    //    PythonScripting.Arrays.CreateGlobalArrayTool.ScriptGlobalArrays.Clear();

                    string DirOut = @"C:\Development\CellCT\DataIn\cct001_20110426_085008\data\temp.bmp";
                    // string DirOut = @"C:\Development\CellCT\DataIn\cct001_20110426_085008\data\temp\temp.bmp";
                    // string DirOut = sfd.MSDialog.FileName;
                    string ImageOutPath = Path.GetDirectoryName(DirOut) + "\\";
                    string ImageOutFilename = Path.GetFileNameWithoutExtension(DirOut);
                    string TempDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\Temp\\";
                    string ImageExten = Path.GetExtension(DirOut);
                    if (ImageExten == "") ImageExten = ".bmp";

                    int ProcCount = Environment.ProcessorCount;
                    Thread[] ProcessThreads = new Thread[ProcCount];
                    int nFiles = (int)((double)ImagesIn.Count / (double)ProcCount);
                    mDataEnvironment.ThreadsRunning = new List<int>();

                    //break up the file list to each thread.  
                    List<ImageFile>[] tImagesIn = new List<ImageFile>[ProcCount];
                    for (int i = 0; i < ProcCount; i++)
                        tImagesIn[i] = new List<ImageFile>();
                    for (int i = 0; i < ImagesIn.Count; i++)
                        tImagesIn[i % ProcCount].Add(ImagesIn[i]);

                    //this is the first thread.  Any filter that works on the whole file list will be run from this thread
                    int MasterThreadId = 0;
                    Dictionary<string, object>[] threadVars = new Dictionary<string, object>[ProcCount];
                    for (int i = 0; i < ProcCount; i++)
                    {
                        //prepare the variable list for the script
                        Dictionary<string, object> Variables = new Dictionary<string, object>();

                        //ironpython can work threaded if each thread has its own scope.  may still be a little unstable
                        string code = textEditorForm1.Text + "\n\n";
                        ProcessThreads[i] = new Thread(delegate(object Vars)
                        {
                            try
                            {
                               // ep.evaluate(ep.CreateScope((Dictionary<string, object>)Vars), code);
                            }
                            catch //(Exception ex)
                            {
                                MacroHelper3D.ErrorOnThread = true;
                            }

                        });

                        Variables.Add("ImageFileListIn", tImagesIn[i]);
                        Variables.Add("ScriptParams", ScriptParams);
                        Variables.Add("ImageOutPath", ImageOutPath);
                        Variables.Add("ImageOutExten", ImageExten);
                        Variables.Add("ImageOutFileName", ImageOutFilename);
                        Variables.Add("TempPath", TempDirectory);
                        Variables.Add("ImageDisp", id);
                        Variables.Add("dataEnvironment", mDataEnvironment);
                        Variables.Add("GlobalPassData", new ReplaceStringDictionary());
                        Variables.Add("DataPath", TempDirectory);
                        Variables.Add("RunningThreaded", true);
                        Variables.Add("StartImageIndex", 0);
                        Variables.Add("ThisThreadId", ProcessThreads[i].ManagedThreadId);
                        Variables.Add("LibraryPath", Path.GetDirectoryName(Application.ExecutablePath) + "\\ImageViewer3DFloat.dll");

                        //the master thread should just be the first one.  It has no physical meaning, just helps to organize 
                        //single threaded operations
                        if (i == 0)
                        {
                            MasterThreadId = ProcessThreads[0].ManagedThreadId;
                            Variables.Add("MasterThreadID", MasterThreadId);
                            mDataEnvironment.MasterThreadId = MasterThreadId;
                        }
                        else
                        {
                            Variables.Add("MasterThreadID", MasterThreadId);
                        }
                        mDataEnvironment.ThreadsRunning.Add(ProcessThreads[i].ManagedThreadId);

                        threadVars[i] = Variables;
                    }

                    for (int i = 0; i < ProcCount; i++)
                    {
                        Dictionary<string, object> Variables = threadVars[i];
                        ProcessThreads[i].Start(Variables);
                    }

                    //do the join without locking up the UI thread
                    int ThreadsFinished = 0;
                    //try
                    {
                        while (ThreadsFinished < ProcCount)
                        {
                            ThreadsFinished = 0;
                            for (int i = 0; i < ProcCount; i++)
                            {
                                if (ProcessThreads[i].ThreadState == ThreadState.Stopped)
                                    ThreadsFinished++;
                            }
                            Application.DoEvents();
                        }
                    }

                    MessageBox.Show("Batch Processing is finished");
                }
            }
        }

        void ep_PythonSpeaks(string Message)
        {
            ironPythonConsole1.LogMessageRecieved(Message);
        }

        private void MacroRecorder_Load(object sender, EventArgs e)
        {
           // textEditorForm1.LoadAndInitializeEditor(mPythonEngine, ironPythonConsole1);
            ironPythonConsole1.InitializeControl();
            string StartProgram =
@"import clr
clr.AddReferenceToFileAndPath(ExecutablePath + 'MathHelpLib.dll')
clr.AddReferenceToFileAndPath(ExecutablePath + 'ImageViewer3DFloat.dll')
clr.AddReferenceToFileAndPath(Executable)
from MathHelpLib import *
from TomographicLibs import *
from ImageViewer3DFloat import *
clr.AddReference('System.Drawing')
from System import *
from System.Drawing import *
from System.Drawing.Imaging import *";
            ironPythonConsole1.SetProgram(StartProgram);
            
        }

        private void ironPythonConsole1_PythonVarAdded(string VarName, object Var)
        {
            variableWindow1.UpdateVarDisplay(VarName, Var);
        }

        private void ironPythonConsole1_PythonVarRemoved(string VarName)
        {
            variableWindow1.RemoveVariable(VarName);
        }
    }
}
