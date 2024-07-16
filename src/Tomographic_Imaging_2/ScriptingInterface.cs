using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathHelpLib;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using System.Drawing;
using Logger;
using System.IO;
using System.Diagnostics;
using GraphingLib;
using System.Runtime.InteropServices;

namespace Tomographic_Imaging_2
{
    public class ScriptingInterface
    {
        public ScriptingInterface()
        {
            scriptingInterface = this;
        }

        public static ScriptingInterface scriptingInterface;

        private List<aProjectionSlice> ProjectionSlices = null;

        public Form MainForm;

        public SimulationControls simulationControlForm;
        public VariableWindowForm variableWindow;

        Dictionary<string, GraphForm> GraphWindows = new Dictionary<string, GraphForm>();
        Dictionary<string, object> VisibleVariable = new Dictionary<string, object>();

        #region GraphHandling
        private GraphForm ActualGraphCreate(string GraphName)
        {
            GraphForm gf = new GraphForm();

            if (TomographicMainForm.isInitialized == true)
                gf.Show(MainForm);
            else
                gf.Show(MainForm);
           // gf.Show(MainForm);

            GraphWindows.Add(GraphName, gf);
            gf.CaptionGraph = GraphName;
            gf.Visible = true;
            Application.DoEvents();
            gf.GraphFormClosed += new GraphForm.GraphFormClosedEvent(gf_GraphFormClosed);
            return gf;
        }

        void gf_GraphFormClosed(string GraphName)
        {
            GraphWindows.Remove(GraphName);
        }

        public void CloseAllGraphWindows()
        {
            for (int i = 0; i < GraphWindows.Count; )
            {
                GraphForm gf = GraphWindows.Values.First<GraphForm>();
                gf.Close();
            }
            GraphWindows.Clear();
        }

        private delegate void CreateGraphDelegate(string GraphName, PhysicalArray Data);
        private delegate void CreateGraphDelegateArray(string GraphName, PhysicalArray[] Data);
        public void CreateGraph(string GraphName, PhysicalArray Data)
        {
            if (MainForm.InvokeRequired)
            {
                MainForm.BeginInvoke(new CreateGraphDelegate(CreateGraph), GraphName, Data);
            }
            else
            {
                if (GraphName == "")
                    throw new Exception("Must include a graph name for later reference");
                if (GraphWindows.ContainsKey(GraphName))
                {
                    Form f = GraphWindows[GraphName];
                    if (f.IsDisposed == false)
                    {
                        UpdateGraph(GraphName, Data);
                        return;
                    }
                    else
                    {
                        GraphWindows.Remove(GraphName);
                    }
                }
                ActualGraphCreate(GraphName).SetData(Data);
            }
        }
        public void UpdateGraph(string GraphName, PhysicalArray Data)
        {
            if (MainForm.InvokeRequired)
            {
                MainForm.BeginInvoke(new CreateGraphDelegate(UpdateGraph), GraphName, Data);
            }
            else
            {
                GraphWindows[GraphName].SetData(Data);
                Application.DoEvents();
            }
        }

        public void CreateGraph(string GraphName, PhysicalArray[] Data)
        {
            if (MainForm.InvokeRequired)
            {
                MainForm.BeginInvoke(new CreateGraphDelegateArray(CreateGraph), GraphName, Data);
            }
            else
            {
                if (GraphName == "")
                    throw new Exception("Must include a graph name for later reference");
                if (GraphWindows.ContainsKey(GraphName))
                {
                    Form f = GraphWindows[GraphName];
                    if (f.IsDisposed == false)
                    {
                        UpdateGraph(GraphName, Data);
                        return;
                    }
                    else
                    {
                        GraphWindows.Remove(GraphName);
                    }
                }
                ActualGraphCreate(GraphName).SetData(Data);
            }
        }
        public void UpdateGraph(string GraphName, PhysicalArray[] Data)
        {
            if (MainForm.InvokeRequired)
            {
                MainForm.BeginInvoke(new CreateGraphDelegateArray(UpdateGraph), GraphName, Data);
            }
            else
            {
                GraphWindows[GraphName].SetData(Data);
                Application.DoEvents();
            }
        }

        private delegate void CreateGraphDelegateImage(string GraphName, Bitmap Data);
        public void CreateGraph(string GraphName, Bitmap Image)
        {
            if (MainForm.InvokeRequired)
            {
                MainForm.BeginInvoke(new CreateGraphDelegateImage(CreateGraph), GraphName, Image);
            }
            else
            {
                if (GraphName == "")
                    throw new Exception("Must include a graph name for later reference");
                if (GraphWindows.ContainsKey(GraphName))
                {
                    Form f = GraphWindows[GraphName];
                    if (f.IsDisposed == false)
                    {
                        UpdateGraph(GraphName, Image);
                        return;
                    }
                    else
                    {
                        GraphWindows.Remove(GraphName);
                    }
                }
                ActualGraphCreate(GraphName).SetData(Image);
            }
        }
        public void UpdateGraph(string GraphName, Bitmap Data)
        {
            if (MainForm.InvokeRequired)
            {
                MainForm.BeginInvoke(new CreateGraphDelegateImage(UpdateGraph), GraphName, Data);
            }
            else
            {
                GraphWindows[GraphName].SetData(Data);
                Application.DoEvents();
            }
        }


        [DllImport("user32.dll", EntryPoint = "SetParent")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

 
      
        public void CreateGraph(string GraphName, string Filename)
        {
         /*   DirectShowLib.Sample.MainForm mf = new DirectShowLib.Sample.MainForm();
            mf.TopMost = true;
            mf.filename = Filename;

            // ScriptingInterface.scriptingInterface.CreateGraph("Centering", GetDataFolder() + "Centering.avi");
            mf.Show();
            Application.DoEvents();
            mf.OpenClip();
            mf.SetCaption(Filename);*/
        }

        public void CreateGraph(string GraphName, string Filename,string Caption)
        {
           /* DirectShowLib.Sample.MainForm mf = new DirectShowLib.Sample.MainForm();
            mf.TopMost = true;
            mf.filename = Filename;

            // ScriptingInterface.scriptingInterface.CreateGraph("Centering", GetDataFolder() + "Centering.avi");
            mf.Show();
            Application.DoEvents();
            mf.OpenClip();
            mf.SetCaption(Caption);*/
        }

        public void CreateGraph(string GraphName, string[] Filenames)
        {
            string exten = Path.GetExtension(Filenames[0]).ToLower();
            if (exten == ".bmp")
            {
                FakeMoviePlayer vp = new FakeMoviePlayer();
                vp.Show(MainForm);
                vp.SetFilenames(Filenames, true);
                Application.DoEvents();
            }
        }

        public void CreateGraph(string GraphName, List<PhysicalArray> Data)
        {
            CreateGraph(GraphName, Data.ToArray());
        }
        public void UpdateGraph(string GraphName, List<PhysicalArray> Data)
        {
            UpdateGraph(GraphName, Data.ToArray());
        }


        public void CreateGraph(string GraphName, double[] Data, string XAxisName, string ValueAxisName)
        {

            PhysicalArray pa = new PhysicalArray(Data, 0, Data.Length);
            pa.ArrayInformation.AxisName_Set(GraphAxis.XAxis, XAxisName);
            pa.ArrayInformation.AxisName_Set(GraphAxis.ValueAxis, ValueAxisName);
            CreateGraph(GraphName, pa);

        }
        public void UpdateGraph(string GraphName, double[] Data, string XAxisName, string ValueAxisName)
        {
            PhysicalArray pa = new PhysicalArray(Data, 0, Data.Length);
            pa.ArrayInformation.AxisName_Set(GraphAxis.XAxis, XAxisName);
            pa.ArrayInformation.AxisName_Set(GraphAxis.ValueAxis, ValueAxisName);

            UpdateGraph(GraphName, pa);
        }
        public void CreateGraph(string GraphName, List<double[]> Data, string XAxisName, string ValueAxisName)
        {

            List<double[,]> NewGraphs = new List<double[,]>();
            foreach (double[] doubledata in Data)
            {
                NewGraphs.Add(doubledata.MakeGraphableArray(0, 1));
            }
            CreateGraph(GraphName, NewGraphs, XAxisName, ValueAxisName);
        }

        public void CreateGraph(string GraphName, double[,] Data, string XAxisName, string ValueAxisName)
        {
            if (Data.GetLength(0) == 2)
            {
                List<double[,]> temp = new List<double[,]>();
                temp.Add(Data);
                CreateGraph(GraphName, temp, XAxisName, "", ValueAxisName);
            }
            else
            {
                CreateGraph(GraphName, Data, XAxisName, "", ValueAxisName);
            }
        }
        public void CreateGraph(string GraphName, double[,] Data, string XAxisName, string YAxisName, string ValueAxisName)
        {
            if (Data == null)
                return;
            if (Data.GetLength(0) == 2)
            {
                List<double[,]> temp = new List<double[,]>();
                temp.Add(Data);
                CreateGraph(GraphName, temp, XAxisName, YAxisName, ValueAxisName);
            }
            else
            {
                PhysicalArray pa = new PhysicalArray(Data, 0, Data.GetLength(0), 0, Data.GetLength(1), false);
                pa.ArrayInformation.AxisName_Set(GraphAxis.XAxis, XAxisName);
                pa.ArrayInformation.AxisName_Set(GraphAxis.YAxis, YAxisName);
                pa.ArrayInformation.AxisName_Set(GraphAxis.ValueAxis, ValueAxisName);
                CreateGraph(GraphName, pa);
            }
        }
        public void UpdateGraph(string GraphName, double[,] Data, string XAxisName, string YAxisName, string ValueAxisName)
        {
            if (Data.GetLength(0) == 2)
            {
                List<double[,]> temp = new List<double[,]>();
                temp.Add(Data);
                UpdateGraph(GraphName, temp, XAxisName, YAxisName, ValueAxisName);
            }
            else
            {
                PhysicalArray pa = new PhysicalArray(Data, 0, Data.GetLength(0), 0, Data.GetLength(1), false);
                pa.ArrayInformation.AxisName_Set(GraphAxis.XAxis, XAxisName);
                pa.ArrayInformation.AxisName_Set(GraphAxis.YAxis, YAxisName);
                pa.ArrayInformation.AxisName_Set(GraphAxis.ValueAxis, ValueAxisName);
                UpdateGraph(GraphName, pa);
            }
        }

        public void CreateGraph(string GraphName, double[, ,] Data, string XAxisName, string YAxisName, string ZAxisName, string ValueAxisName)
        {
            PhysicalArray pa = new PhysicalArray(Data, 0, Data.GetLength(0), 0, Data.GetLength(1), 0, Data.GetLength(2));
            pa.ArrayInformation.AxisName_Set(GraphAxis.XAxis, XAxisName);
            pa.ArrayInformation.AxisName_Set(GraphAxis.YAxis, YAxisName);
            pa.ArrayInformation.AxisName_Set(GraphAxis.ZAxis, ZAxisName);
            pa.ArrayInformation.AxisName_Set(GraphAxis.ValueAxis, ValueAxisName);
            CreateGraph(GraphName, pa);
        }
        public void UpdateGraph(string GraphName, double[, ,] Data, string XAxisName, string YAxisName, string ZAxisName, string ValueAxisName)
        {
            PhysicalArray pa = new PhysicalArray(Data, 0, Data.GetLength(0), 0, Data.GetLength(1), 0, Data.GetLength(2));
            pa.ArrayInformation.AxisName_Set(GraphAxis.XAxis, XAxisName);
            pa.ArrayInformation.AxisName_Set(GraphAxis.YAxis, YAxisName);
            pa.ArrayInformation.AxisName_Set(GraphAxis.ZAxis, ZAxisName);
            pa.ArrayInformation.AxisName_Set(GraphAxis.ValueAxis, ValueAxisName);
            UpdateGraph(GraphName, pa);
        }

        private delegate void CreateGraphDelegateDoubleArray(string GraphName, List<double[,]> Data, string XAxisName, string YAxisName, string ValueAxisName);
        public void CreateGraph(string GraphName, List<double[,]> Data, string XAxisName, string YAxisName, string ValueAxisName)
        {
            if (Data[0].GetLength(0) == 2)
            {
                if (MainForm.InvokeRequired)
                {
                    MainForm.BeginInvoke(new CreateGraphDelegateDoubleArray(CreateGraph), GraphName, Data, XAxisName, YAxisName, ValueAxisName);
                }
                else
                {
                    if (GraphName == "")
                        throw new Exception("Must include a graph name for later reference");

                    if (GraphWindows.ContainsKey(GraphName))
                    {
                        Form f = GraphWindows[GraphName];
                        if (f.IsDisposed == false)
                        {
                            UpdateGraph(GraphName, Data, XAxisName, YAxisName, ValueAxisName);
                            return;
                        }
                        else
                        {
                            GraphWindows.Remove(GraphName);
                        }
                    }
                    ActualGraphCreate(GraphName).SetData(Data);
                }
            }
            else
            {
                PhysicalArray[] OutPhysArrays = new PhysicalArray[Data.Count];
                for (int i = 0; i < Data.Count; i++)
                {
                    PhysicalArray pa = new PhysicalArray(Data[i], 0, Data[i].GetLength(1), 0, Data[i].GetLength(0), false);
                    pa.ArrayInformation.AxisName_Set(GraphAxis.XAxis, XAxisName);
                    pa.ArrayInformation.AxisName_Set(GraphAxis.YAxis, YAxisName);
                    pa.ArrayInformation.AxisName_Set(GraphAxis.ValueAxis, ValueAxisName);
                    OutPhysArrays[i] = pa;
                }
                CreateGraph(GraphName, OutPhysArrays);
            }
        }
        public void CreateGraph(string GraphName, List<double[,]> Data, string XAxisName, string ValueAxisName)
        {
            if (Data[0].GetLength(0) == 2)
            {
                if (MainForm.InvokeRequired)
                {
                    MainForm.BeginInvoke(new CreateGraphDelegateDoubleArray(CreateGraph), GraphName, Data, XAxisName, "", ValueAxisName);
                }
                else
                {
                    if (GraphName == "")
                        throw new Exception("Must include a graph name for later reference");

                    if (GraphWindows.ContainsKey(GraphName))
                    {
                        Form f = GraphWindows[GraphName];
                        if (f.IsDisposed == false)
                        {
                            UpdateGraph(GraphName, Data, XAxisName, "", ValueAxisName);
                            return;
                        }
                        else
                        {
                            GraphWindows.Remove(GraphName);
                        }
                    }
                    ActualGraphCreate(GraphName).SetData(Data);
                }
            }
            else
            {
                PhysicalArray[] OutPhysArrays = new PhysicalArray[Data.Count];
                for (int i = 0; i < Data.Count; i++)
                {
                    PhysicalArray pa = new PhysicalArray(Data[i], 0, Data[i].GetLength(1), 0, Data[i].GetLength(0), false);
                    pa.ArrayInformation.AxisName_Set(GraphAxis.XAxis, XAxisName);
                    pa.ArrayInformation.AxisName_Set(GraphAxis.YAxis, "");
                    pa.ArrayInformation.AxisName_Set(GraphAxis.ValueAxis, ValueAxisName);
                    OutPhysArrays[i] = pa;
                }
                CreateGraph(GraphName, OutPhysArrays);
            }
        }
        public void UpdateGraph(string GraphName, List<double[,]> Data, string XAxisName, string YAxisName, string ValueAxisName)
        {
            if (Data[0].GetLength(0) == 2)
            {
                if (MainForm.InvokeRequired)
                {
                    MainForm.BeginInvoke(new CreateGraphDelegateDoubleArray(UpdateGraph), GraphName, Data, XAxisName, YAxisName, ValueAxisName);
                }
                else
                {

                    GraphWindows[GraphName].SetData(Data);
                    Application.DoEvents();
                }
            }
            else
            {
                PhysicalArray[] OutPhysArrays = new PhysicalArray[Data.Count];
                for (int i = 0; i < Data.Count; i++)
                {
                    PhysicalArray pa = new PhysicalArray(Data[i], 0, Data[i].GetLength(0), 0, Data[i].GetLength(1), false);
                    pa.ArrayInformation.AxisName_Set(GraphAxis.XAxis, XAxisName);
                    pa.ArrayInformation.AxisName_Set(GraphAxis.YAxis, YAxisName);
                    pa.ArrayInformation.AxisName_Set(GraphAxis.ValueAxis, ValueAxisName);
                    OutPhysArrays[i] = pa;
                }
                UpdateGraph(GraphName, OutPhysArrays);
            }
        }
        #endregion

        public void WriteToLog(string Value)
        {
            LoggerForm.LogMessage(Value);
        }

        private delegate void MakeVariableVisibleDelegate(string VarName, object Variable);
        public void MakeVariableVisible(string VarName, object Variable)
        {
            if (MainForm.InvokeRequired)
            {
                MainForm.BeginInvoke(new MakeVariableVisibleDelegate(MakeVariableVisible), VarName, Variable);
            }
            else
            {
                try
                {
                    if (VisibleVariable.ContainsKey(VarName))
                    {
                        VisibleVariable.Remove(VarName);
                        VisibleVariable.Add(VarName, Variable);
                        variableWindow.UpdateVariable(VarName, Variable);
                    }
                    else
                    {
                        VisibleVariable.Add(VarName, Variable);
                        variableWindow.UpdateVarDisplay(VarName, Variable);
                    }
                }
                catch { }
            }
        }
        public Dictionary<string, object> VisibleVariables
        {
            get { return VisibleVariable; }
        }
        public object this[string VariableName]
        {
            get
            {
                try
                {
                    return VisibleVariables[VariableName];
                }
                catch (Exception ex)
                {
                    LoggerForm.LogErrorMessage(ex);
                    return null;
                }
            }
        }
        public void RemoveVisibleVariable(string VarName)
        {
            try
            {
                variableWindow.RemoveVariable(VarName);
                VisibleVariables.Remove(VarName);
            }
            catch (Exception ex)
            {
                LoggerForm.LogErrorMessage(ex);
            }
        }

        public void SetProjectionSlice(aProjectionSlice Slice)
        {
            if (ProjectionSlices == null)
            {
                ProjectionSlices = new List<aProjectionSlice>();
            }
            ProjectionSlices.Add(Slice);
            MakeVariableVisible("ProjectionSlices", ProjectionSlices);
        }

        public ProjectionObject CreateProjectionObject(aProjectionSlice Slice)
        {
            ProjectionObject po = new ProjectionObject();
            int GridSize = Slice.Projection.GetLength(Axis.XAxis);
            po.ClearGrid(true, Slice.PhysicalEndX - Slice.PhysicalStartX, Slice.PhysicalEndY - Slice.PhysicalStartY, Slice.PhysicalEndX - Slice.PhysicalStartX, GridSize, GridSize, GridSize);
            return po;
        }

        public ProjectionObject CreateProjectionObject(int GridSize, double PhysicalWidth, double PhysicalHeight)
        {
            ProjectionObject po = new ProjectionObject();
            po.ClearGrid(true, PhysicalWidth, PhysicalHeight, GridSize, GridSize);
            return po;
        }
        public ProjectionObject CreateProjectionObject(int GridSize, double PhysicalWidth, double PhysicalHeight, double PhysicalZHeight)
        {
            ProjectionObject po = new ProjectionObject();
            po.ClearGrid(true, PhysicalWidth, PhysicalHeight, PhysicalZHeight, GridSize, GridSize, GridSize);
            return po;
        }


        public ProjectionObject CreatePhantom(int GridSize)
        {
            ProjectionObject Phantom = new ProjectionObject();
            Phantom.ClearGrid(true, 2, 2, 2, GridSize, GridSize, GridSize);
            Phantom.CreateShepAndLogan();
            return Phantom;
        }
    }


}
