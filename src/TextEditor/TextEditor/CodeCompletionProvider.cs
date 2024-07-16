using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

using System.Windows.Forms;
using ICSharpCode.TextEditor;
using System.Collections;

namespace IronPythonEditor.TextEditor
{
    public class CodeCompletionProvider : ICompletionDataProvider
    {
        TextEditorForm  mainForm;

        public CodeCompletionProvider(TextEditorForm mainForm)
        {
            this.mainForm = mainForm;
        }

        public ImageList ImageList
        {
            get
            {
                return mainForm.CodeCompletionImages;
            }
        }

        public string PreSelection
        {
            get
            {
                return null;
            }
        }

        public int DefaultIndex
        {
            get
            {
                return -1;
            }
        }

        public CompletionDataProviderKeyResult ProcessKey(char key)
        {
            if (char.IsLetterOrDigit(key) || key == '_')
            {
                return CompletionDataProviderKeyResult.NormalKey;
            }
            else
            {
                // key triggers insertion of selected items
                return CompletionDataProviderKeyResult.InsertionKey;
            }
        }

        /// <summary>
        /// Called when entry should be inserted. Forward to the insertion action of the completion data.
        /// </summary>
        public bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
        {
            textArea.Caret.Position = textArea.Document.OffsetToPosition(insertionOffset);
            return data.InsertAction(textArea, key);
        }

        public ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
        {
            
            string[] Lines = textArea.MotherTextEditorControl.Text.Split('\n');
            string subLine = Lines[textArea.Caret.Line].Substring(0, textArea.Caret.Column);
            string[] parts = subLine.Split(' ', '=');
            string[] Reference = parts[parts.Length - 1].Split('.');

            List<ICompletionData> resultList = new List<ICompletionData>();

            List<string> Names = mainForm.AutoCompleteList[Reference[Reference.Length - 1]];
               
            foreach(string n in Names )
                resultList.Add(new DefaultCompletionData(n,n  ,0));
            
            return resultList.ToArray();
        }


      
    }
}
