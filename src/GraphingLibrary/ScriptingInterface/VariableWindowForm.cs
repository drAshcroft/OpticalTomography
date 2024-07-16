using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MathHelpLib;
//using IronPythonEditor.VariableWindow;

namespace GraphingLib
{
    public partial class VariableWindowForm : Form
    {
        public VariableWindowForm()
        {
            InitializeComponent();
        }

        public void UpdateVariable(string VariableName, object NewVariable)
        {
           // variableWindow1.UpdateVariable(VariableName, NewVariable);
        }

        public void UpdateVarDisplay(string Varname, object Variable)
        {
          //  variableWindow1.UpdateVarDisplay(Varname, Variable);
        }

        public void RemoveVariable(string VariableName)
        {
           // variableWindow1.RemoveVariable(VariableName);
        }

        private bool variableWindow1_NodeOpened(VariableNode  vNode)
        {
            if (vNode.Variable.GetType() == typeof(PhysicalArray))
            {
                #region Physical Array
                try
                {
                    ScriptingInterface.scriptingInterface.CreateGraph(vNode.Text,
                     (PhysicalArray)vNode.Variable);
                }
                catch { }
                #endregion
                return true;
            }
           
            else if (vNode.Variable is Array)
            {
                #region Arrays
                Array array = ((Array)vNode.Variable);
                int Dimensions = array.Rank;
                if (Dimensions == 1)
                    ScriptingInterface.scriptingInterface.CreateGraph(vNode.Text, (double[])vNode.Variable, "X", "Y");
                else if (Dimensions == 2)
                    ScriptingInterface.scriptingInterface.CreateGraph(vNode.Text, (double[,])vNode.Variable, "", "", "");
                else if (Dimensions == 3)
                    ScriptingInterface.scriptingInterface.CreateGraph(vNode.Text, (double[, ,])vNode.Variable, "", "", "", "");
                #endregion
                return true;
            }
            return false;
        }

        private bool variableWindow1_BeforeNodeExpand(VariableNode vNode)
        {
            object Variable = vNode.Variable;
            if (Variable.GetType() == typeof(PhysicalArray))
            {
                #region PhysicalArray
                PhysicalArray pa = (PhysicalArray)Variable;
                vNode.Nodes.Add("Array Rank = " + pa.ArrayRank.ToString());
                vNode.Nodes.Add("ImageData Type = " + pa.ArrayType.ToString());
                vNode.Nodes.Add("Array Size = " + pa.GetLength(Axis.XAxis) + "," + pa.GetLength(Axis.YAxis) + "," + pa.GetLength(Axis.ZAxis));
                return true;
                #endregion
            }
         
            else if (Variable.GetType().ToString().Contains("System.Collections.Generic.List"))
            {
                #region Lists

                System.Collections.IList ilist = Variable as System.Collections.IList;

                vNode.Nodes.Add("Number of Items = " + ilist.Count);
                bool ShowExplore = true;
                for (int i = 0; i < vNode.Nodes.Count; i++)
                {
                    if (vNode.Nodes[i].Text == "Explore Further")
                        ShowExplore = false;
                }
                int cc = 0;
                foreach (object o in ilist)
                {
                    if (o != null)
                    {
                        //variableWindow1. UpdateVarDisplay(vNode, cc.ToString() + " : " + o.GetType().ToString(), o);
                        cc++;
                        if (cc > 5)
                        {
                            if (ShowExplore)
                                vNode.Nodes.Add("Explore Further");
                            break;
                        }
                    }
                }
                return true;
                #endregion
            }
            else if (Variable is Array)
            {
                #region Arrays
                Array array = ((Array)Variable);
                int Dimensions = array.Rank;
                vNode.Nodes.Add("Array Rank = " + Dimensions);
                string Sizes = " Array Size = (";
                for (int i = 0; i < Dimensions - 1; i++)
                {
                    Sizes += array.GetLength(i).ToString() + " X ";
                }
                Sizes += array.GetLength(Dimensions - 1).ToString() + ") ";
                vNode.Nodes.Add(Sizes);

                try
                {
                    vNode.Nodes.Add("Array of Type: " + array.GetValue(0).GetType().ToString());
                }
                catch { }
                return true;
                #endregion
            }
            else if (Variable is ValueType)
            {
                #region ValueTypes
                vNode.Nodes.Add("Value = " + Variable.ToString());
                return true;
                #endregion
            }
            else if (Variable is string)
            {
                #region Strings
                vNode.Nodes.Add("Value = " + Variable.ToString());
                return true;
                #endregion
            }
            else
            {
                return false;
            }
        }

        private void VariableWindowForm_Load(object sender, EventArgs e)
        {
            ScriptingInterface.scriptingInterface.variableWindow = this;
        }
    }


    public class VariableNode : TreeNode
    {
        public object Variable;

        public VariableNode(string Text) : base(Text) { }
    }
}
