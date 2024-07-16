using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IronPythonEditor.VariableWindow
{
    public class VariableNode:TreeNode 
    {
        public object Variable;

        public VariableNode(string Text) : base(Text) { }
    }
}
