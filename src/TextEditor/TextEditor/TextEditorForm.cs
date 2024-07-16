using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System.Linq;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using System.Reflection;

namespace IronPythonEditor.TextEditor
{
    /// <summary>Main form for a multi-file text editor based on 
    /// ICSharpCode.TextEditor.TextEditorControl.</summary>
    public partial class TextEditorForm : UserControl
    {

        TextEditorControl Editor;
        Form ParentForm;

        public TextEditorForm()
        {
            InitializeComponent();
            ParentForm = FindForm();
        }

        public void LoadAndInitializeEditor(EvaluatePython PythonEngine, Console.IronPythonConsole Console)
        {
            mEvalPython = PythonEngine;
            mConsole = Console;
            Editor = (AddNewTextEditor("New file"));
        }

        public override string Text
        {
            get
            {
                return Editor.Text;
            }
            set
            {
                Editor.Text = value;
            }
        }

        public void AddMenuItem(ToolStripMenuItem MenuItem)
        {
            menuStrip1.Items.Add(MenuItem);
        }

        private void SetupEditor(TextEditorControl Editor)
        {

            // HighlightingManager.Manager.AddSyntaxModeFileProvider(
            //                     new FileSyntaxModeProvider("Python"));
            // Now we can set the Highlighting scheme...
            Editor.Document.HighlightingStrategy =
              HighlightingManager.Manager.FindHighlighter("Python");

            // Show or Hide the End of Line Markers...
            Editor.ShowEOLMarkers = false;

            // Show or Hide Invalid Line Markers...
            Editor.ShowInvalidLines = true;

            // Show or Hide a little dot where spaces are...
            Editor.ShowSpaces = true;

            // Show or Hide ">>" where tabs are...
            Editor.ShowTabs = true;

            // Highlight the matching bracket or not...
            Editor.ShowMatchingBracket = true;

            // Show or Hide Line Numbers...
            Editor.ShowLineNumbers = true;

            // Show or Hide a Ruler at the top of the editor...
            Editor.ShowHRuler = false;

            // Show or Hide the vertical line in the text editor...
            Editor.ShowVRuler = true;

            // Enable Code Folding, if enabled, you must set the folding strategy
            Editor.EnableFolding = true;
            Editor.Document.FoldingManager.FoldingStrategy = new RegionFoldingStrategy();
            Editor.Document.FoldingManager.UpdateFoldings(null, null);

            // Editor's font...
            Editor.Font = this.Font;

            // If you want to convert tabs to spaces or not...
            Editor.ConvertTabsToSpaces = true;

            // How many spaces should make up a tab...
            Editor.TabIndent = 4;

            // What column to place the vertical ruler at...
            Editor.VRulerRow = 80;

            // Allow the caret "|" beyong the end of the line or not...
            Editor.AllowCaretBeyondEOL = false;

            // Automatically instert a curly bracket when one is typed or not...
            Editor.TextEditorProperties.AutoInsertCurlyBracket = false;

            //Editor.ActiveTextAreaControl.TextArea.KeyEventHandler += new ICSharpCode.TextEditor.KeyEventHandler(TextArea_KeyEventHandler);

            if (mAutoCompletionList == null)
                CreateAutoCompleteDictionary();

            Editor.Text =
@"
import clr
";

        }

        public void SetText(string text)
        {
            Editor.Text = text;
        }
        public void OpenFile(string Filename)
        {
            OpenFiles(new string[] { Filename });
        }

        #region CodeCompletion
        CodeCompletionWindow codeCompletionWindow;

        bool TextArea_KeyEventHandler(char key)
        {
            if (codeCompletionWindow != null)
            {
                // If completion window is open and wants to handle the key, don't let the text area
                // handle it
                if (codeCompletionWindow.ProcessKeyEvent(key))
                {
                    return true;
                }
            }
            if (key == '.')
            {
                ICompletionDataProvider completionDataProvider = new CodeCompletionProvider(this);

                if (ParentForm == null)
                    ParentForm = FindForm();

                codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(
                    ParentForm,					// The parent window for the completion window
                    Editor, 					// The text editor to show the window for
                    "test",		// Filename - will be passed back to the provider
                    completionDataProvider,		// Provider to get the list of possible completions
                    key							// Key pressed - will be passed to the provider
                );
                if (codeCompletionWindow != null)
                {
                    // ShowCompletionWindow can return null when the provider returns an empty list
                    codeCompletionWindow.Closed += new EventHandler(CloseCodeCompletionWindow);
                }
            }
            return false;
        }

        void CloseCodeCompletionWindow(object sender, EventArgs e)
        {
            if (codeCompletionWindow != null)
            {
                codeCompletionWindow.Closed -= new EventHandler(CloseCodeCompletionWindow);
                codeCompletionWindow.Dispose();
                codeCompletionWindow = null;
            }
        }
        public ImageList CodeCompletionImages
        {
            get { return imageList1; }
        }

        #region CreateAutoCompleteList
        public Dictionary<string, List<string>> AutoCompleteList
        {
            get { return mAutoCompletionList; }
        }

        Dictionary<string, List<string>> mAutoCompletionList = null;
        private void StaticMethods(Type MyClass)
        {
            // get all public static methods of MyClass type
            MethodInfo[] methodInfos = MyClass.GetMethods(BindingFlags.Public |
                                                                  BindingFlags.Static);
            // sort methods by name
            Array.Sort(methodInfos,
                    delegate(MethodInfo methodInfo1, MethodInfo methodInfo2)
                    { return methodInfo1.Name.CompareTo(methodInfo2.Name); });
            List<string> MethodNames = new List<string>();
            // write method names
            foreach (MethodInfo methodInfo in methodInfos)
            {
                string test = methodInfo.ToString();
                MethodNames.Add(methodInfo.ToString());
            }
            mAutoCompletionList.Add(MyClass.ToString(), MethodNames);
        }
        private void CreateAutoCompleteDictionary()
        {
            mAutoCompletionList = new Dictionary<string, List<string>>();

            // StaticMethods(typeof(MathHelpLib.MathHelps));
        }
        #endregion
        #endregion

        #region Code related to File menu

        private void menuFileNew_Click(object sender, EventArgs e)
        {
            Editor = (AddNewTextEditor("New file"));
        }

        /// <summary>This variable holds the settings (whether to show line numbers, 
        /// etc.) that all editor controls share.</summary>
        ITextEditorProperties _editorSettings;

        private TextEditorControl AddNewTextEditor(string title)
        {
            var tab = new TabPage(title);
            TextEditorControl editor = new TextEditorControl();
            editor.Dock = System.Windows.Forms.DockStyle.Fill;
            editor.IsReadOnly = false;
            editor.Document.DocumentChanged +=
                new DocumentEventHandler((sender, e) => { SetModifiedFlag(editor, true); });

            SetupEditor(editor);
            // When a tab page gets the focus, move the focus to the editor control
            // instead when it gets the Enter (focus) event. I use BeginInvoke 
            // because changing the focus directly in the Enter handler doesn't 
            // work.
            tab.Enter +=
                new EventHandler((sender, e) =>
                {
                    var page = ((TabPage)sender);
                    page.BeginInvoke(new Action<TabPage>(p => p.Controls[0].Focus()), page);
                });
            tab.Controls.Add(editor);
            fileTabs.Controls.Add(tab);

            if (_editorSettings == null)
            {
                _editorSettings = editor.TextEditorProperties;
                OnSettingsChanged();
            }
            else
                editor.TextEditorProperties = _editorSettings;

            fileTabs.SelectedTab = tab;
            Editor = editor;
            return editor;
        }

        private void menuFileOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                // Try to open chosen file
                OpenFiles(openFileDialog.FileNames);
        }

        private void OpenFiles(string[] fns)
        {
            // Close default untitled document if it is still empty
            if (fileTabs.TabPages.Count == 1
                && ActiveEditor.Document.TextLength == 0
                && string.IsNullOrEmpty(ActiveEditor.FileName))
                RemoveTextEditor(ActiveEditor);

            // Open file(s)
            foreach (string fn in fns)
            {
                var editor = AddNewTextEditor(Path.GetFileName(fn));
                try
                {
                    editor.LoadFile(fn);
                    // Modified flag is set during loading because the document 
                    // "changes" (from nothing to something). So, clear it again.
                    SetModifiedFlag(editor, false);
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                    RemoveTextEditor(editor);
                    return;
                }

                // ICSharpCode.TextEditor doesn't have any built-in code folding
                // strategies, so I've included a simple one. Apparently, the
                // foldings are not updated automatically, so in this demo the user
                // cannot add or remove folding regions after loading the file.
                editor.Document.FoldingManager.FoldingStrategy = new RegionFoldingStrategy();
                editor.Document.FoldingManager.UpdateFoldings(null, null);
            }
        }

        private void menuFileClose_Click(object sender, EventArgs e)
        {
            if (ActiveEditor != null)
                RemoveTextEditor(ActiveEditor);
        }

        private void RemoveTextEditor(TextEditorControl editor)
        {
            ((TabControl)editor.Parent.Parent).Controls.Remove(editor.Parent);
        }

        private void menuFileSave_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor != null)
                DoSave(editor);
        }

        private bool DoSave(TextEditorControl editor)
        {
            if (string.IsNullOrEmpty(editor.FileName))
                return DoSaveAs(editor);
            else
            {
                try
                {
                    editor.SaveFile(editor.FileName);
                    SetModifiedFlag(editor, false);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                    return false;
                }
            }
        }

        private void menuFileSaveAs_Click(object sender, EventArgs e)
        {
            var editor = ActiveEditor;
            if (editor != null)
                DoSaveAs(editor);
        }

        private bool DoSaveAs(TextEditorControl editor)
        {
            saveFileDialog.FileName = editor.FileName;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    editor.SaveFile(saveFileDialog.FileName);
                    editor.Parent.Text = Path.GetFileName(editor.FileName);
                    SetModifiedFlag(editor, false);

                    // The syntax highlighting strategy doesn't change
                    // automatically, so do it manually.
                    editor.Document.HighlightingStrategy =
                        HighlightingStrategyFactory.CreateHighlightingStrategyForFile(editor.FileName);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
            }
            return false;
        }

        private void menuFileExit_Click(object sender, EventArgs e)
        {
            // this.Close();
        }

        #endregion

        #region Code related to Edit menu

        /// <summary>Performs an action encapsulated in IEditAction.</summary>
        /// <remarks>
        /// There is an implementation of IEditAction for every action that 
        /// the user can invoke using a shortcut key (arrow keys, Ctrl+X, etc.)
        /// The editor control doesn't provide a public funciton to perform one
        /// of these actions directly, so I wrote DoEditAction() based on the
        /// code in TextArea.ExecuteDialogKey(). You can call ExecuteDialogKey
        /// directly, but it is more fragile because it takes a Keys value (e.g.
        /// Keys.Left) instead of the action to perform.
        /// <para/>
        /// Clipboard commands could also be done by calling methods in
        /// editor.ActiveTextAreaControl.TextArea.ClipboardHandler.
        /// </remarks>
        private void DoEditAction(TextEditorControl editor, ICSharpCode.TextEditor.Actions.IEditAction action)
        {
            if (editor != null && action != null)
            {
                var area = editor.ActiveTextAreaControl.TextArea;
                editor.BeginUpdate();
                try
                {
                    lock (editor.Document)
                    {
                        action.Execute(area);
                        if (area.SelectionManager.HasSomethingSelected && area.AutoClearSelection /*&& caretchanged*/)
                        {
                            if (area.Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal)
                            {
                                area.SelectionManager.ClearSelection();
                            }
                        }
                    }
                }
                finally
                {
                    editor.EndUpdate();
                    area.Caret.UpdateCaretPosition();
                }
            }
        }

        private void menuEditCut_Click(object sender, EventArgs e)
        {
            if (HaveSelection())
                DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.Cut());
        }
        private void menuEditCopy_Click(object sender, EventArgs e)
        {
            if (HaveSelection())
                DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.Copy());
        }
        private void menuEditPaste_Click(object sender, EventArgs e)
        {
            DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.Paste());
        }
        private void menuEditDelete_Click(object sender, EventArgs e)
        {
            if (HaveSelection())
                DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.Delete());
        }

        private bool HaveSelection()
        {
            var editor = ActiveEditor;
            return editor != null &&
                editor.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected;
        }

        FindAndReplaceForm _findForm = new FindAndReplaceForm();

        private void menuEditFind_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            _findForm.ShowFor(editor, false);
        }

        private void menuEditReplace_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            _findForm.ShowFor(editor, true);
        }

        private void menuFindAgain_Click(object sender, EventArgs e)
        {
            _findForm.FindNext(true, false,
                string.Format("Search text «{0}» not found.", _findForm.LookFor));
        }
        private void menuFindAgainReverse_Click(object sender, EventArgs e)
        {
            _findForm.FindNext(true, true,
                string.Format("Search text «{0}» not found.", _findForm.LookFor));
        }

        private void menuToggleBookmark_Click(object sender, EventArgs e)
        {
            var editor = ActiveEditor;
            if (editor != null)
            {
                DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.ToggleBookmark());
                editor.IsIconBarVisible = editor.Document.BookmarkManager.Marks.Count > 0;
            }
        }

        private void menuGoToNextBookmark_Click(object sender, EventArgs e)
        {
            DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.GotoNextBookmark
                (bookmark => true));
        }

        private void menuGoToPrevBookmark_Click(object sender, EventArgs e)
        {
            DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.GotoPrevBookmark
                (bookmark => true));
        }

        #endregion

        #region Code related to Options menu

        /// <summary>Toggles whether the editor control is split in two parts.</summary>
        /// <remarks>Exercise for the reader: modify TextEditorControl and
        /// TextAreaControl so it shows a little "splitter stub" like you see in
        /// other apps, that allows the user to split the text editor by dragging
        /// it.</remarks>
        private void menuSplitTextArea_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.Split();
        }

        /// <summary>Show current settings on the Options menu</summary>
        /// <remarks>We don't have to sync settings between the editors because 
        /// they all share the same DefaultTextEditorProperties object.</remarks>
        private void OnSettingsChanged()
        {
            menuShowSpacesTabs.Checked = _editorSettings.ShowSpaces;
            menuShowNewlines.Checked = _editorSettings.ShowEOLMarker;
            menuHighlightCurrentRow.Checked = _editorSettings.LineViewerStyle == LineViewerStyle.FullRow;
            menuBracketMatchingStyle.Checked = _editorSettings.BracketMatchingStyle == BracketMatchingStyle.After;
            menuEnableVirtualSpace.Checked = _editorSettings.AllowCaretBeyondEOL;
            menuShowLineNumbers.Checked = _editorSettings.ShowLineNumbers;
        }

        private void menuShowSpaces_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.ShowSpaces = editor.ShowTabs = !editor.ShowSpaces;
            OnSettingsChanged();
        }
        private void menuShowNewlines_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.ShowEOLMarkers = !editor.ShowEOLMarkers;
            OnSettingsChanged();
        }

        private void menuHighlightCurrentRow_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.LineViewerStyle = editor.LineViewerStyle == LineViewerStyle.None
                ? LineViewerStyle.FullRow : LineViewerStyle.None;
            OnSettingsChanged();
        }

        private void menuBracketMatchingStyle_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.BracketMatchingStyle = editor.BracketMatchingStyle == BracketMatchingStyle.After
                ? BracketMatchingStyle.Before : BracketMatchingStyle.After;
            OnSettingsChanged();
        }

        private void menuEnableVirtualSpace_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.AllowCaretBeyondEOL = !editor.AllowCaretBeyondEOL;
            OnSettingsChanged();
        }

        private void menuShowLineNumbers_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.ShowLineNumbers = !editor.ShowLineNumbers;
            OnSettingsChanged();
        }

        private void menuSetTabSize_Click(object sender, EventArgs e)
        {
            if (ActiveEditor != null)
            {
                string result = InputBox.Show("Specify the desired tab width.", "Tab size", _editorSettings.TabIndent.ToString());
                int value;
                if (result != null && int.TryParse(result, out value) && value.IsInRange(1, 32))
                {
                    ActiveEditor.TabIndent = value;
                }
            }
        }

        private void menuSetFont_Click(object sender, EventArgs e)
        {
            var editor = ActiveEditor;
            if (editor != null)
            {
                fontDialog.Font = editor.Font;
                if (fontDialog.ShowDialog(this) == DialogResult.OK)
                {
                    editor.Font = fontDialog.Font;
                    OnSettingsChanged();
                }
            }
        }

        #endregion

        #region Other stuff



        private void TextEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ask user to save changes
            foreach (var editor in AllEditors)
            {
                if (IsModified(editor))
                {
                    var r = MessageBox.Show(string.Format("Save changes to {0}?", editor.FileName ?? "new file"),
                        "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (r == DialogResult.Cancel)
                        e.Cancel = true;
                    else if (r == DialogResult.Yes)
                        if (!DoSave(editor))
                            e.Cancel = true;
                }
            }
        }

        /// <summary>Returns a list of all editor controls</summary>
        private IEnumerable<TextEditorControl> AllEditors
        {
            get
            {
                return from t in fileTabs.Controls.Cast<TabPage>()
                       from c in t.Controls.OfType<TextEditorControl>()
                       select c;
            }
        }

        /// <summary>Returns the currently displayed editor, or null if none are open</summary>
        private TextEditorControl ActiveEditor
        {
            get
            {
                if (fileTabs.TabPages.Count == 0) return null;
                return fileTabs.SelectedTab.Controls.OfType<TextEditorControl>().FirstOrDefault();
            }
        }

        /// <summary>Gets whether the file in the specified editor is modified.</summary>
        /// <remarks>TextEditorControl doesn't maintain its own internal modified 
        /// flag, so we use the '*' shown after the file name to represent the 
        /// modified state.</remarks>
        private bool IsModified(TextEditorControl editor)
        {
            // TextEditorControl doesn't seem to contain its own 'modified' flag, so 
            // instead we'll treat the "*" on the filename as the modified flag.
            try
            {
                return editor.Parent.Text.EndsWith("*");
            }
            catch
            {
                return true;
            }
        }
        private void SetModifiedFlag(TextEditorControl editor, bool flag)
        {
            if (IsModified(editor) != flag)
            {
                var p = editor.Parent;
                if (IsModified(editor))
                    p.Text = p.Text.Substring(0, p.Text.Length - 1);
                else
                    p.Text += "*";
            }
        }

        /// <summary>We handle DragEnter and DragDrop so users can drop files on the editor.</summary>
        private void TextEditorForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }
        private void TextEditorForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] list = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (list != null)
                OpenFiles(list);
        }

        #endregion

        public void SetCaretPosition(int Line,int Col)
        {
            Editor.ActiveTextAreaControl.Caret.Line =Line ;
            Editor.ActiveTextAreaControl.Caret.Column = Col ;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                // foreach (TextEditorControl editor in Editors)
                {
                    Editor.Document.FoldingManager.FoldingStrategy = new RegionFoldingStrategy();
                    Editor.Document.FoldingManager.UpdateFoldings(null, null);
                }
            }
            catch { }
        }
        EvaluatePython mEvalPython;// = new EvaluatePython();
        Console.IronPythonConsole mConsole;
        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mEvalPython.SetOutputPath();

            try
            {
                mEvalPython.evaluate(Editor.Text);
            }
            catch (PythonException pex)
            {
                Editor.ActiveTextAreaControl.Caret.Line = pex.LineNumber;
                Editor.ActiveTextAreaControl.Caret.Column = pex.ColNumber;
                if (mConsole != null)
                {
                    mConsole.LogMessageRecieved(pex.PythonErrorTraceBack);
                }
            }
            catch (Exception ex)
            {
                if (mConsole != null)
                {
                    mConsole.LogMessageRecieved(ex.Message);
                }
            }

        }

        private void fileTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            Editor = fileTabs.SelectedTab.Controls.OfType<TextEditorControl>().FirstOrDefault();
        }


    }


}