using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace Tomographic_Imaging_2.TextEditor
{
    /// <summary>
    /// Represents an item in the code completion window.
    /// </summary>
    class CodeCompletionData : DefaultCompletionData, ICompletionData
    {
        
        public CodeCompletionData(string  member)
            : base(member, null, 0)
        {
        }

        int overloads = 0;

        public void AddOverload()
        {
            overloads++;
        }

       /* static int GetMemberImageIndex(IMember member)
        {
            // Missing: different icons for private/public member
            if (member is IMethod)
                return 1;
            if (member is IProperty)
                return 2;
            if (member is IField)
                return 3;
            if (member is IEvent)
                return 6;
            return 3;
        }*/

        /*static int GetClassImageIndex(IClass c)
        {
            switch (c.ClassType)
            {
                case ClassType.Enum:
                    return 4;
                default:
                    return 0;
            }
        }*/

        string description;

        // DefaultCompletionData.Description is not virtual, but we can reimplement
        // the interface to get the same effect as overriding.
        string ICompletionData.Description
        {
            get
            {
                if (description == null)
                {
                   
                    description = "";
                    if (overloads > 1)
                    {
                        description += " (+" + overloads + " overloads)";
                    }
                   
                }
                return description;
            }
        }

        public static string XmlDocumentationToText(string xmlDoc)
        {
            return "";    
        }
    }
}
