using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace IronPythonEditor.VariableWindow
{
    public partial class VariableWindow : UserControl
    {
        public VariableWindow()
        {
            InitializeComponent();
        }


        public delegate bool BeforeNodeExpandEvent(VariableNode vNode);
        public delegate bool NodeOpenedEvent(VariableNode  eNode);

        public event NodeOpenedEvent NodeOpened;
        public event BeforeNodeExpandEvent BeforeNodeExpand;

        private delegate void NoDelegate(string Varname, object Variable);

        public void UpdateVarDisplay(string Varname, object Variable)
        {
            if (InvokeRequired)
                BeginInvoke(new NoDelegate(UpdateVarDisplay), Varname, Variable);
            else
            {
                UpdateVarDisplay(RootNode, Varname, Variable);
                RootNode.Expand();
            }
        }
        public void RemoveVariable(string VariableName)
        {
            try
            {
                RootNode.Nodes.Remove(AllVariables[VariableName]);
                AllVariables.Remove(VariableName);
            }
            catch { }
        }
        public void UpdateVariable(string VariableName, object NewVariable)
        {
            VariableNode vNode = UpdateExistingVariable(RootNode, VariableName, NewVariable);
            try
            {
                AllVariables.Remove(VariableName);
            }
            catch { }
            AllVariables.Add(VariableName, vNode);
        }
        private VariableNode UpdateExistingVariable(TreeNode Parent, string VariableName, object NewVariable)
        {
            VariableNode tnode = new VariableNode(VariableName);

            tnode.Variable = NewVariable;

            int index = 0;
            try
            {
                index = Parent.Nodes.IndexOf(AllVariables[VariableName]);
                Parent.Nodes.Remove(AllVariables[VariableName]);
            }
            catch { }
            Parent.Nodes.Insert(index, tnode);
            tnode.Nodes.Add(new VariableNode("Type :" + NewVariable.GetType().ToString()));
            return tnode;
        }
        private Dictionary<string, VariableNode> AllVariables = new Dictionary<string, VariableNode>();
        public void UpdateVarDisplay(TreeNode Parent, string Name, object Variable)
        {
            if (Variable == null)
                return;
            if (AllVariables.ContainsKey(Name))
            {
                UpdateExistingVariable(Parent, Name, Variable);
            }
            else
            {
                VariableNode tnode = new VariableNode(Name);
                AllVariables.Add(Name, tnode);
                tnode.Variable = Variable;
                Parent.Nodes.Add(tnode);
                tnode.Nodes.Add(new VariableNode("Type :" + Variable.GetType().ToString()));
            }
        }

        TreeNode RootNode;
        private void VariableWindow_Load(object sender, EventArgs e)
        {
            RootNode = treeView1.Nodes.Add("Varibles");
            RootNode.Expand();
        }

        protected virtual void NodeOpenedI(TreeNode eNode)
        {
            if (eNode != null)
            {
                if (eNode.GetType() == typeof(VariableNode))
                {
                    VariableNode vNode = (VariableNode)eNode;
                    if (vNode.Variable != null)
                    {
                        if (NodeOpened == null || NodeOpened(vNode) == false)
                        {

                            if (vNode.Variable is Array)
                            {
                                #region Arrays
                                Array array = ((Array)vNode.Variable);
                                int Dimensions = array.Rank;

                                #endregion
                            }
                            else if (vNode.Text == "Visualize")
                            {
                                #region Visualization
                                try
                                {

                                }
                                catch { }
                                #endregion
                            }
                        }
                    }
                }
            }
        }

        protected virtual void BeforeNodeExpandI(VariableNode vNode)
        {
            object Variable = vNode.Variable;

            vNode.Nodes.Clear();
            vNode.Nodes.Add(new VariableNode("Type :" + Variable.GetType().ToString()));

            if (BeforeNodeExpand == null || BeforeNodeExpand(vNode) == false)
            {

                if (Variable.GetType().ToString().Contains("System.Collections.Generic.List"))
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
                            UpdateVarDisplay(vNode, cc.ToString() + " : " + o.GetType().ToString(), o);
                            cc++;
                            if (cc > 5)
                            {
                                if (ShowExplore)
                                    vNode.Nodes.Add("Explore Further");
                                break;
                            }
                        }
                    }
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
                    #endregion
                }
                else if (Variable is ValueType)
                {
                    #region ValueTypes
                    vNode.Nodes.Add("Value = " + Variable.ToString());
                    #endregion
                }
                else if (Variable is string)
                {
                    #region Strings
                    vNode.Nodes.Add("Value = " + Variable.ToString());
                    #endregion
                }
                else
                {
                    vNode.Nodes.Add(Variable.ToString());

                }
            }
        }


        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            NodeOpenedI(e.Node);

        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.GetType() == typeof(TreeNode))
                return;


            BeforeNodeExpandI((VariableNode)e.Node);
        }
    }
}
