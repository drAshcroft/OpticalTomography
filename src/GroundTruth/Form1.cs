using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageViewer3D.Tools;
using Tomographic_Imaging_2;
using System.IO;
using MathHelpLib;
using System.Threading;
using ImageViewer.Filters;
using ImageViewer.PythonScripting;
using ImageViewer;
using System.Xml;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using MathHelpLib.ImageProcessing;
using GroundTruth;
using System.Xml.Serialization;
using ImageViewer3D.Selections;
using GraphingLib;

namespace GroundTruth
{
    public partial class Form1 : Form
    {
        Dataset AllContours;
        int CurrentContour;
        private int CurrentSliceIndex;

        int mSelectedAxis = -1;

        private int Width = 0;
        private int Height = 0;
        private int Depth = 0;
        private int bAddContour = 0;

        private string CurrentFileDirectory;

        // public double[, ,] loadedvolume;

        public Form1()
        {
            InitializeComponent();
            lViewAxis.SetSelected(0, true);
            instructionsToolStripMenuItem.Text = "Use File/Open Volume to load image.";
        }

        string args;
        public Form1(string args)
        {

            InitializeComponent();
            this.args = args;
            timer1.Enabled = true;
        }

        private Rectangle ZoomBox = new Rectangle();
        private Rectangle ScreenBox = new Rectangle();

        private float[, ,] VolumeData = null;

        private Dataset ConvertXML(string Filename)
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(GroundTruthOld.Dataset));

            // Create a new file stream for reading the XML file
            FileStream ReadFileStream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read);

            // Load the object saved above by using the Deserialize function
            GroundTruthOld.Dataset OldContours = (GroundTruthOld.Dataset)SerializerObj.Deserialize(ReadFileStream);

            // Cleanup
            ReadFileStream.Close();


            Dataset Contours = new Dataset(OldContours.Filename, OldContours.User);
            Contours.Depth = OldContours.Depth;
            Contours.Filename = OldContours.Filename;
            Contours.Height = OldContours.Height;
            Contours.ScreenBox = OldContours.ScreenBox;
            Contours.User = OldContours.User;
            Contours.ViewBox = OldContours.ViewBox;
            Contours.Width = OldContours.Width;
            Contours.Zooming = false;

            int zoomX = Contours.ScreenBox.Width;
            int zoomY = Contours.ScreenBox.Height;

            double unZoomX = 1;// (double)TheContours.Depth / (double)zoomX;
            double unZoomY = 1;// (double)TheContours.Height / (double)zoomY;

            unZoomX = (double)Contours.Depth / (double)zoomX;
            unZoomY = (double)Contours.Height / (double)zoomY;

            for (int i = 0; i < OldContours.Contours.Count; i++)
            {
                GroundTruthOld.Contour3D oldC = OldContours.Contours[i];
                Surface3D surf = new Surface3D(oldC.ContourName);
                surf.Num_of_Slices = oldC.Num_of_Slices;
                surf.SurfaceName = oldC.ContourName;
                for (int j = 0; j < oldC.SingleSlice.Count; j++)
                {
                    GroundTruthOld.SingleSlice ssOld = oldC.SingleSlice[j];
                    SurfaceSlice ssN = new SurfaceSlice(ssOld.SliceNumber);
                    ssN.PlaneAxis = ssOld.PlaneAxis;
                    ssN.SliceNumber = ssOld.SliceNumber;
                    for (int k = 0; k < ssOld.Vertixes.Count; k++)
                    {
                        PointF[] Points = new PointF[ssOld.Vertixes[k].Length];
                        for (int m = 0; m < Points.Length; m++)
                            Points[m] = new PointF((float)(ssOld.Vertixes[k][m].X * unZoomX), (float)(unZoomY * ssOld.Vertixes[k][m].Y));
                        ssN.Contours.Add(Points);
                    }
                    surf.Slices.Add(ssN);
                }
                Contours.Surfaces.Add(surf);
            }

            return Contours;
        }

        private Dataset OpenContour(string Filename)
        {
            string xmlpath = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename) + ".xml";
            if (Path.GetExtension(Filename).ToLower() == ".xml")
            {
                return ConvertXML(Filename);
            }
            else if (File.Exists(Filename) == false && File.Exists(xmlpath) == true)
            {
                return ConvertXML(xmlpath);
            }
            else
            {
                XmlSerializer SerializerObj = new XmlSerializer(typeof(Dataset));

                // Create a new file stream for reading the XML file
                FileStream ReadFileStream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read);

                // Load the object saved above by using the Deserialize function
                Dataset Contours = (Dataset)SerializerObj.Deserialize(ReadFileStream);

                // Cleanup
                ReadFileStream.Close();
                return Contours;
            }

        }

        private void OpenDataSet(string Filename)
        {

            string[] dirParts;
            if (File.Exists(Filename))
                dirParts = Path.GetDirectoryName(Filename).Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            else
                dirParts = Filename.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);

            bool NotFound = true;
            CurrentFileDirectory = "";
            for (int i = 0; i < dirParts.Length; i++)
            {
                CurrentFileDirectory += dirParts[i];
                if (dirParts[i].StartsWith("cct") == true)
                {
                    NotFound = false;
                    break;
                }
                CurrentFileDirectory += "\\";
            }
            //CurrentFileDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(FolderName)));
            string ContourName = "";
            if (Directory.Exists(CurrentFileDirectory + "\\data") == true)
            {
                ContourName = CurrentFileDirectory + "\\data\\CellContours.CHF";
            }
            else if (File.Exists(CurrentFileDirectory + "\\500PP\\CellContours.CHF"))
            {
                ContourName = CurrentFileDirectory + "\\500PP\\CellContours.CHF";
            }
            else if (File.Exists(CurrentFileDirectory + "\\CellContours.CHF"))
            {
                ContourName = CurrentFileDirectory + "\\CellContours.CHF";
            }
            else if (Directory.Exists(CurrentFileDirectory + "\\data") == true)
            {
                ContourName = CurrentFileDirectory + "\\data\\CellContours.xml";
            }
            else if (File.Exists(CurrentFileDirectory + "\\500PP\\CellContours.xml"))
            {
                ContourName = CurrentFileDirectory + "\\500PP\\CellContours.xml";
            }
            else if (File.Exists(CurrentFileDirectory + "\\CellContours.xml"))
            {
                ContourName = CurrentFileDirectory + "\\CellContours.xml";
            }

            if (Directory.Exists(Filename) == true || Path.GetExtension(Filename).ToLower() == ".chf" || Path.GetExtension(Filename).ToLower() == ".xml")
            {
                if (Directory.Exists(CurrentFileDirectory + "\\data") == true)
                {
                    Filename = CurrentFileDirectory + "\\data\\ProjectionObject.cct";
                }
                else if (Directory.Exists(CurrentFileDirectory + "\\500PP\\recon_cropped_8bit") == true)
                {
                    Filename = CurrentFileDirectory + "\\500PP\\recon_cropped_8bit\\reconCrop8bit_000.png";
                }
                else if (Directory.Exists(CurrentFileDirectory + "\\500PP\\recon_8bit") == true)
                {
                    Filename = CurrentFileDirectory + "\\500PP\\recon_8bit\\recon8bit_000.png";
                }
            }


            viewerControl3D1.ActiveDrawingTool = new LassoSelectTool();
            viewerControl3D1.ClearOldData();

            if (Filename.IndexOf(".cct") != -1)
                VolumeData = MathHelpsFileLoader.OpenDensityDataFloat(Filename);
            else
            {
                string[] files = Directory.GetFiles(Path.GetDirectoryName(Filename), "*.png");
                if (files.Length == 0)
                    files = Directory.GetFiles(Path.GetDirectoryName(Filename), "*.tif");

                VolumeData = MathHelpsFileLoader.OpenDensityDataFloat(files);
            }
            Width = VolumeData.GetLength(2);
            Height = VolumeData.GetLength(1);
            Depth = VolumeData.GetLength(0);

            viewerControl3D1.SelectedArea = null;
            viewerControl3D1.SetImage(VolumeData);

            mSelectedAxis = -1;

            viewerControl3D1.SliceIndexX = (int)(Width / 2d);
            viewerControl3D1.SliceIndexY = (int)(Height / 2d);
            viewerControl3D1.SliceIndexZ = (int)(Depth / 2d);


            if (File.Exists(ContourName))
            {
                /* XmlSerializer SerializerObj = new XmlSerializer(typeof(Dataset));

                 // Create a new file stream for reading the XML file
                 FileStream ReadFileStream = new FileStream(ContourName, FileMode.Open, FileAccess.Read, FileShare.Read);

                 // Load the object saved above by using the Deserialize function
                 AllContours = (Dataset)SerializerObj.Deserialize(ReadFileStream);

                 // Cleanup
                 ReadFileStream.Close();*/
                AllContours = OpenContour(ContourName);
                AllContours.Filename = Filename;
                if (AllContours.Zooming == true)
                {
                    for (int CurrentContour = 0; CurrentContour < AllContours.Surfaces.Count; CurrentContour++)
                    {
                        for (int CurrentSliceIndex = 0; CurrentSliceIndex < AllContours.Surfaces[CurrentContour].Slices.Count; CurrentSliceIndex++)
                        {
                            SurfaceSlice ssn = AllContours.Surfaces[CurrentContour].Slices[CurrentSliceIndex];
                            for (int slice = 0; slice < ssn.Contours.Count; slice++)
                            {
                                int zoomX = AllContours.ScreenBox.Width;
                                int zoomY = AllContours.ScreenBox.Height;

                                double unZoomX = (double)AllContours.Depth / (double)zoomX;
                                double unZoomY = (double)AllContours.Height / (double)zoomY;

                                PointF[] points = ssn.Contours[slice];
                                PointF[] OutPoints = new PointF[points.Length];
                                for (int i = 0; i < points.Length; i++)
                                {
                                    OutPoints[i].X = (int)(points[i].X * unZoomX);
                                    OutPoints[i].Y = (int)(points[i].Y * unZoomY);
                                }
                                ssn.Contours.RemoveAt(slice);
                                ssn.Contours.Insert(slice, OutPoints);
                            }
                        }
                        //AllContours.Contours[CurrentContour].SingleSlice.Reverse();
                    }
                    AllContours.Zooming = false;
                }


                int BiggestContour = 0;
                int BiggestSlice = 0;
                int selectedAxis = 0;
                int t = 0;
                for (int i = 0; i < AllContours.Surfaces.Count; i++)
                    for (int j = 0; j < AllContours.Surfaces[i].Slices.Count; j++)
                    {
                        t = AllContours.Surfaces[i].Slices[j].PlaneAxis;
                        if (t > selectedAxis) selectedAxis = t;

                        if (AllContours.Surfaces[i].Slices[j].Contours != null && AllContours.Surfaces[i].Slices[j].Contours.Count > 0)
                        {
                            BiggestContour = i;
                            if (BiggestSlice == 0)
                                BiggestSlice = j;
                        }
                    }


                if (selectedAxis == 1)
                    lViewAxis.SetSelected(1, true);
                else if (selectedAxis == 2)
                    lViewAxis.SetSelected(2, true);
                else if (selectedAxis == 0)
                    lViewAxis.SetSelected(3, true);

                lViewBox_SelectedIndexChanged(this, EventArgs.Empty);

                Next_Click(this, EventArgs.Empty);

                lContourNames.SetSelected(BiggestContour, true);
                lContourNames_SelectedIndexChanged(this, EventArgs.Empty);

                if (selectedAxis == 2 && BiggestSlice < Width)
                {
                    viewerControl3D1.SliceIndexY = BiggestSlice;
                    CurrentSliceIndex = viewerControl3D1.SliceIndexY;
                }
                else if (selectedAxis == 0 && BiggestSlice < Height)
                {
                    viewerControl3D1.SliceIndexX = BiggestSlice;
                    CurrentSliceIndex = viewerControl3D1.SliceIndexX;
                }
                else if (selectedAxis == 1 && BiggestSlice < Depth)
                {
                    viewerControl3D1.SliceIndexZ = BiggestSlice;
                    CurrentSliceIndex = viewerControl3D1.SliceIndexZ;
                }
            }
            else
            {
                AllContours = new Dataset(Filename, tUserName.Text);
                for (int i = 0; i < lContourNames.Items.Count; i++)
                    AllContours.AddSurface((string)lContourNames.Items[i]);

                AllContours.Depth = Depth;
                AllContours.Width = Width;
                AllContours.Height = Height;
                lContourNames.SetSelected(0, true);
                lContourNames_SelectedIndexChanged(this, EventArgs.Empty);
            }

            //set the caption
            Text = Path.GetDirectoryName(Filename);
        }

        private void SaveDataSet(string FileName)
        {

            AllContours.User = tUserName.Text;
            AllContours.ScreenBox = new Rectangle(0, 0, ScreenBox.Width, ScreenBox.Height);
            AllContours.ViewBox = ZoomBox;
            AllContours.Zooming = false;// viewerControl3D1.Zooming;

            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(Dataset));

            System.IO.StreamWriter file = new System.IO.StreamWriter(FileName);
            writer.Serialize(file, AllContours);
            file.Close();

            //marchingCubesToolStripMenuItem_Click(this, EventArgs.Empty);

        }

        private void LoadCellList(string FolderName)
        {
            string STorage = FolderName;

            //List<string> DirAdds = 
            List<string> BadDirs = new List<string>();
            List<string> GoodDirs = new List<string>();
            bool FirstDir = true;
            string[] AllDirs;

            BadDirs.Add(STorage);
            while (FirstDir == true && BadDirs.Count > 0)
            {
                try
                {
                    string[] Dirs = Directory.GetDirectories(BadDirs[0]);
                    if (Dirs.Length > 0)
                    {
                        if (Dirs[0].Contains("cct") == true && Path.GetFileName(Dirs[0]).Length > 6)
                            GoodDirs.AddRange(Dirs);
                        else
                            BadDirs.AddRange(Dirs);
                    }
                }
                catch { }
                BadDirs.RemoveAt(0);
            }
            // string[] AllDirs = Directory.GetDirectories(STorage, "cc*.*", SearchOption.TopDirectoryOnly );
            AllDirs = GoodDirs.ToArray();
            string pPath;
            Queue<string> Selected = new Queue<string>();
            int Completed = 0;
            int Attempted = 0;
            lCells.Items.Clear();


            const bool SearchTarget = false;
            for (int i = 0; i < AllDirs.Length; i++)
            {
                try
                {
                    string folder = AllDirs[i];
                    if (File.Exists(folder + "\\data\\ProjectionObject.cct") == true
                        && File.Exists(folder + "\\data\\CellContours.CHF") == SearchTarget
                         && File.Exists(folder + "\\data\\CellContours.xml") == SearchTarget)
                    {
                        lCells.Items.Add(folder);
                    }
                    else if (File.Exists(folder + "\\data\\ProjectionObject.cct") == true
                        && File.Exists(folder + "\\data\\CellContours.CHF") == SearchTarget
                        && File.Exists(folder + "\\data\\CellContours.xml") == SearchTarget)
                    {
                        lCells.Items.Add(folder);
                    }
                    else if (Directory.Exists(folder + "\\500PP") == true
                         && File.Exists(folder + "\\CellContours.CHF") == SearchTarget
                        && File.Exists(folder + "\\CellContours.xml") == SearchTarget)
                    {
                        lCells.Items.Add(folder);
                    }
                    else if (Directory.Exists(folder + "\\500PP") == true
                         && File.Exists(folder + "\\550pp\\CellContours.CHF") == SearchTarget
                        && File.Exists(folder + "\\550pp\\CellContours.xml") == SearchTarget)
                    {
                        lCells.Items.Add(folder);
                    }
                    else
                    {
                        pPath = "";
                        Completed++;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }

            }

            CellLoaded = false;
            lCells.SetSelected(0, true);
            if (CellLoaded == false)
                lCells_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private bool CellLoaded = false;

        private void DrawSelections()
        {
            try
            {
                ImageViewer3D.Tools.LassoSelectTool lst = (ImageViewer3D.Tools.LassoSelectTool)viewerControl3D1.ActiveDrawingTool;
                try
                {
                    SurfaceSlice ssn = AllContours.Surfaces[CurrentContour].Slices[CurrentSliceIndex];

                    //pull out the active slice and draw it on the screen.  coords have to be unscaled
                    int last = ssn.Contours.Count - 1;
                    if (last >= 0)
                    {
                        double unZoomX = 1;
                        double unZoomY = 1;
                        if (AllContours.Zooming == true)
                        {
                            int zoomX = AllContours.ScreenBox.Width;
                            int zoomY = AllContours.ScreenBox.Height;

                            unZoomX = (double)AllContours.Depth / (double)zoomX;
                            unZoomY = (double)AllContours.Height / (double)zoomY;

                            PointF[] points = ssn.Contours[last];
                            PointF[] OutPoints = new PointF[points.Length];
                            for (int i = 0; i < points.Length; i++)
                            {
                                OutPoints[i].X = (int)(points[i].X * unZoomX);
                                OutPoints[i].Y = (int)(points[i].Y * unZoomY);
                            }

                            lst.DrawnCurve = new List<PointF>(OutPoints);
                        }
                        else
                            lst.DrawnCurve = new List<PointF>(ssn.Contours[last]);
                    }
                    else //just clear off the drawing
                        lst.DrawnCurve = null;


                    lst.InactiveClear();


                    for (int i = 0; i < ssn.Contours.Count - 1; i++)
                    {
                        if (AllContours.Zooming == true)
                        {
                            int zoomX = AllContours.ScreenBox.Width;
                            int zoomY = AllContours.ScreenBox.Height;

                            double unZoomX = (double)AllContours.Depth / (double)zoomX;
                            double unZoomY = (double)AllContours.Height / (double)zoomY;

                            PointF[] points = ssn.Contours[i];
                            PointF[] OutPoints = new PointF[points.Length];
                            for (int j = 0; j < points.Length; j++)
                            {
                                OutPoints[j].X = (int)(points[j].X * unZoomX);
                                OutPoints[j].Y = (int)(points[j].Y * unZoomY);
                            }

                            lst.InactiveAdd((OutPoints));
                        }
                        else
                            lst.InactiveAdd(ssn.Contours[i]);
                    }

                }
                catch
                {
                    try
                    {
                        lst.DrawnCurve = null;
                        lst.InactiveClear();
                    }
                    catch { }
                }
            }
            catch { }
        }

        #region Events


        #region Form Events
        private void Form1_Load(object sender, EventArgs e)
        {
            viewerControl3D1.Zooming = true;
            viewerControl3D1.ActiveDrawingTool = (new ImageViewer3D.Tools.LassoSelectTool());
            //  if (Directory.Exists(textBox1.Text))
            //     textBox1_TextChanged(this, EventArgs.Empty);
            /*   this.Show();
               Application.DoEvents();

               OpenDataSet("S:\\Research\\Cell CT\\Evaluation\\Miranda\\ConciseEvaluation\\VG\\cct001_20110120_104112\\CellContours.CHF");
                   lViewAxis.Enabled = true;
                   Next.Enabled = true;
                   instructionsToolStripMenuItem.Text = "Chose one view that you like, then click start.";
                   AllInterpolationsMenuItem_Click(this, EventArgs.Empty);*/
        }

        private void viewerControl3D1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //  bNextCell_Click(this, EventArgs.Empty);
            }
            catch { }
        }
        #endregion

        #region MenuEvents

        #region File
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ret = openFileDialog1.ShowDialog();

            if (ret == DialogResult.OK)
            {
                OpenDataSet(openFileDialog1.FileName);
                lViewAxis.Enabled = true;
                Next.Enabled = true;
                instructionsToolStripMenuItem.Text = "Chose one view that you like, then click start.";
            }
        }

        private void saveContoursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(CurrentFileDirectory + "\\data") == true)
                SaveDataSet(CurrentFileDirectory + "\\data\\cellcontours.CHF");
            else
                SaveDataSet(CurrentFileDirectory + "\\cellcontours.CHF");
        }



        private void openContoursToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //DialogResult ret = openFileDialog1.ShowDialog();

            //if (ret == DialogResult.OK)
            {
                AllContours = OpenContour(@"S:\Research\Cell CT\Evaluation\Kat\For B\fcopycct001_20110614_132108\CellContours.xml");
              //  AllContours = OpenContour(openFileDialog1.FileName);

                Width = AllContours.Width;
                Height = AllContours.Height;
                Depth = AllContours.Depth;
            }

        }
        #endregion

        #region Interpolation Events

        #region Save Interpolation
        private void linearTrianglesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FromContours(InterpolationType.VoxelContours, AllContours, false);
        }

        private void linearLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FromContours(InterpolationType.LinearInterpolation, AllContours, false);
        }

        private void marchingCubesToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            FromContours(InterpolationType.MarchingCubes, AllContours, false);
        }

        private void unorganizedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FromContours(InterpolationType.FromUnorganized, AllContours, false);
        }

        private void delaunayToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FromContours(InterpolationType.Delaunay, AllContours, false);
        }

        private void guassianSplatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FromContours(InterpolationType.GaussianSplat, AllContours, false);
        }
        #endregion

        #region View Interpolation
        private void marchingCubesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void doAllInterpolationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunInterpolations(AllContours);
            MessageBox.Show("Done");
        }


        private void linearTrianglesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FromContours(InterpolationType.VoxelContours, AllContours, true);
        }

        private void linearLinesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FromContours(InterpolationType.LinearInterpolation, AllContours, true);
        }

        private void marchingCubesToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            FromContours(InterpolationType.MarchingCubes, AllContours, true);
        }

        private void unorganizedPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FromContours(InterpolationType.FromUnorganized, AllContours, true);
        }

        private void delaunayToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FromContours(InterpolationType.Delaunay, AllContours, true);
        }

        private void gaussianSplatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FromContours(InterpolationType.GaussianSplat, AllContours, true);
        }

        #endregion
        #endregion

        #endregion

        #region Contour Events
        private void viewerControl3D1_SelectionPerformed(ImageViewer3D.ISelection3D Selection)
        {
            if (Selection.GetType() == typeof(LassoSelection3D))
            {
                if (mSelectedAxis != Selection.SelectionAxis)
                    return;

                ZoomBox = viewerControl3D1.GetScreen(mSelectedAxis).GetViewRectangle();
                ScreenBox = viewerControl3D1.GetScreen(mSelectedAxis).ScreenCoords;

                ImageViewer3D.Selections.LassoSelection3D lasso = (ImageViewer3D.Selections.LassoSelection3D)Selection;

                SurfaceSlice ssn;
                //ssn = AllContours.Contours[CurrentContour].SingleSlice[CurrentSliceIndex / (int)steps.Value];
                ssn = AllContours.Surfaces[CurrentContour].Slices[CurrentSliceIndex];

                ssn.PlaneAxis = lasso.SelectionAxis;

                //only add a new contour if the add contour button has been pushed, otherwise jsut replace the data
                if (bAddContour == 1 || ssn.Contours.Count == 0)
                {
                    ssn.Contours.Add(new PointF[lasso.Lasso.ToArray().Length]);
                }

                int numb = ssn.Contours.Count - 1;
                ssn.Contours[numb] = lasso.Lasso.ToArray();
                ssn.SliceNumber = CurrentSliceIndex;
                bAddContour = 0;

                //AllContours.Contours[CurrentContour].SingleSlice[CurrentSliceIndex / (int)steps.Value] = ssn;
                AllContours.Surfaces[CurrentContour].Slices[CurrentSliceIndex] = ssn;
                // CurrentContour.SingleSlice[CurrentSliceIndex].Vertixes.AddRange(lasso.list);

                DrawSelections();
            }
        }

        private void lContourNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < lContourNames.Items.Count; i++)
            {
                if (lContourNames.GetSelected(i) == true)
                {
                    CurrentContour = i;
                    if (mSelectedAxis == 2)
                    {
                        viewerControl3D1.SliceIndexX = 0;
                        CurrentSliceIndex = 0;
                    }
                    else if (mSelectedAxis == 0)
                    {
                        viewerControl3D1.SliceIndexY = 0;
                        CurrentSliceIndex = 0;
                    }
                    else if (mSelectedAxis == 1)
                    {
                        viewerControl3D1.SliceIndexZ = 0;
                        CurrentSliceIndex = 0;
                    }

                    //  if (AllContours.Contours[CurrentContour].SingleSlice.Count == 0)
                    //    AllContours.Contours[CurrentContour].SingleSlice.Add(new SingleSlice(CurrentSliceIndex));
                    break;
                }
            }
        }

        private void previous_Click(object sender, EventArgs e)
        {
            try
            {
                int newValue = CurrentSliceIndex - (int)steps.Value;
                if (newValue < 0)
                    newValue = CurrentSliceIndex;
                lPosition.Text = "Position: " + newValue.ToString();
                if (mSelectedAxis == 0)
                {
                    viewerControl3D1.SliceIndexY = newValue;
                    CurrentSliceIndex = viewerControl3D1.SliceIndexY;
                }
                else if (mSelectedAxis == 2)
                {
                    viewerControl3D1.SliceIndexX = newValue;
                    CurrentSliceIndex = viewerControl3D1.SliceIndexX;
                }
                else
                {
                    viewerControl3D1.SliceIndexZ = newValue;
                    CurrentSliceIndex = viewerControl3D1.SliceIndexZ;
                }

                DrawSelections();
            }
            catch { }

            viewerControl3D1.RedrawBuffers();
        }

        private void Next_Click(object sender, EventArgs e)
        {
            previous.Enabled = true;
            #region Setup Views
            if (mSelectedAxis == -1)
            {
                if (lViewAxis.GetSelected(0) == true)
                {
                    MessageBox.Show("Please Select just one view to do the drawing");
                    return;
                }
                else if (lViewAxis.GetSelected(1) == true)
                    mSelectedAxis = 1;
                else if (lViewAxis.GetSelected(2) == true)
                    mSelectedAxis = 2;
                else if (lViewAxis.GetSelected(3) == true)
                    mSelectedAxis = 0;

                if (mSelectedAxis == 0)
                {
                    viewerControl3D1.SliceIndexY = 0;
                    CurrentSliceIndex = 0;
                }
                else if (mSelectedAxis == 2)
                {
                    viewerControl3D1.SliceIndexX = 0;
                    CurrentSliceIndex = 0;
                }
                else
                {
                    viewerControl3D1.SliceIndexZ = 0;
                    CurrentSliceIndex = 0;
                }
                // viewerControl3D1.collapseDisplay(mSelectedAxis);
                Next.Enabled = true;
                lContourNames.Enabled = true;
                bNextCell.Enabled = true;
                Next.Text = "Next Slice";
                instructionsToolStripMenuItem.Text = "Click next until you see the desired feature.  Then draw the contour.  Click next to draw next.";
            }
            else
            {
                string OutName = "";
                if (Directory.Exists(CurrentFileDirectory + "\\data") == true)
                    OutName = CurrentFileDirectory + "\\data\\CellContours.CHF";
                else
                    OutName = CurrentFileDirectory + "\\500PP\\CellContours.CHF";
                //  if (OutName != "")
                //      SaveDataSet(OutName);
            }
            #endregion
            //make sure that slices have been added for this contour
            #region Populate slices
            if (mSelectedAxis == 0)
            {
                if (AllContours.Surfaces[CurrentContour].Slices.Count < Width)
                {
                    AllContours.Surfaces[CurrentContour].Reset(Width);
                    /*for (int j = 0; AllContours.Surfaces[CurrentContour].Slices.Length < Width; j++)
                    {
                        AllContours.Surfaces[CurrentContour].Slices.Add(new SurfaceSlice(j));
                    }*/
                }
            }
            else if (mSelectedAxis == 2)
            {
                if (AllContours.Surfaces[CurrentContour].Slices.Count < Height)
                {
                    AllContours.Surfaces[CurrentContour].Reset(Height);
                    /*
                    for (int j = 0; AllContours.Surfaces[CurrentContour].Slices.Length < Height; j++)
                    {
                        AllContours.Surfaces[CurrentContour].Slices.Add(new SurfaceSlice(j));
                    }*/
                }
            }
            else
            {
                if (AllContours.Surfaces[CurrentContour].Slices.Count < Depth)
                {
                    AllContours.Surfaces[CurrentContour].Reset(Depth);
                    /*
                    for (int j = 0; AllContours.Surfaces[CurrentContour].Slices.Length < Depth; j++)
                    {
                        AllContours.Surfaces[CurrentContour].Slices.Add(new SurfaceSlice(j));
                    }*/
                }
            }
            #endregion

            try
            {
                int NewValue = CurrentSliceIndex + (int)steps.Value; ;
                if (mSelectedAxis == 0 && NewValue < Width)
                {
                    viewerControl3D1.SliceIndexY = NewValue;
                    CurrentSliceIndex = viewerControl3D1.SliceIndexY;
                    lPosition.Text = "Position: " + NewValue.ToString();
                }
                else if (mSelectedAxis == 2 && NewValue < Height)
                {
                    viewerControl3D1.SliceIndexX = NewValue;
                    CurrentSliceIndex = viewerControl3D1.SliceIndexX;
                    lPosition.Text = "Position: " + NewValue.ToString();
                }
                else if (mSelectedAxis == 1 && NewValue < Depth)
                {
                    viewerControl3D1.SliceIndexZ = NewValue;
                    CurrentSliceIndex = viewerControl3D1.SliceIndexZ;
                    lPosition.Text = "Position: " + NewValue.ToString();
                }

                DrawSelections();

                viewerControl3D1.RedrawBuffers();
            }
            catch { }
        }



        private void AddContour_Click(object sender, EventArgs e)
        {

            bAddContour = 1;

            //List<Point[]> PreviousContours = new List<Point[]>();
            /*for (int i = 0; i < AllContours.Contours[CurrentContour].SingleSlice[CurrentSliceIndex].Vertixes.Count - 1; i++)
            {
                PreviousContours.Add(AllContours.Contours[CurrentContour].SingleSlice[CurrentSliceIndex].Vertixes[i]);
            }*/
            //PreviousContours.AddRange(AllContours.Contours[CurrentContour].SingleSlice[CurrentSliceIndex].Vertixes);
            //viewerControl3D1.ActiveDrawingTool = (new ImageViewer3D.Tools.LassoSelectTool());

            ImageViewer3D.Tools.LassoSelectTool lst = (ImageViewer3D.Tools.LassoSelectTool)viewerControl3D1.ActiveDrawingTool;

            lst.InactiveClear();
            foreach (PointF[] p in AllContours.Surfaces[CurrentContour].Slices[CurrentSliceIndex].Contours)
                lst.InactiveAdd(p);
        }

        private void clear_Click(object sender, EventArgs e)
        {
            AllContours.Surfaces[CurrentContour].Slices[CurrentSliceIndex].Contours.Clear();
            ImageViewer3D.Tools.LassoSelectTool lst = (ImageViewer3D.Tools.LassoSelectTool)viewerControl3D1.ActiveDrawingTool;
            lst.InactiveClear();
            lst.DrawnCurve = null;

            if (mSelectedAxis == 2)
            {
                viewerControl3D1.SliceIndexY = CurrentSliceIndex;
            }
            else if (mSelectedAxis == 0)
            {
                viewerControl3D1.SliceIndexX = CurrentSliceIndex;
            }

            else if (mSelectedAxis == 1)
            {
                viewerControl3D1.SliceIndexZ = CurrentSliceIndex;

            }
        }

        private void bDeleteCurve_Click(object sender, EventArgs e)
        {
            AllContours.Surfaces[CurrentContour].Slices[CurrentSliceIndex].Contours.Clear();
            ImageViewer3D.Tools.LassoSelectTool lst = (ImageViewer3D.Tools.LassoSelectTool)viewerControl3D1.ActiveDrawingTool;
            lst.DrawnCurve = null;
            lst.InactiveClear();

            viewerControl3D1.RedrawBuffers();
        }
        #endregion

        #region View Events
        private void lViewBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < lViewAxis.Items.Count; i++)
            {
                if (lViewAxis.GetSelected(i) == true)
                {
                    viewerControl3D1.CollapseDisplay(i - 1);
                }
            }
        }

        private int viewerControl3D1_AxisUpdated(int Axis, int NewValue)
        {
            lPosition.Text = "Position: " + NewValue.ToString();
            CurrentSliceIndex = NewValue;
            return CurrentSliceIndex;
        }

        private void UnlockAxis_Click(object sender, EventArgs e)
        {
            mSelectedAxis = -1;
            //  AllContours.Surfaces[CurrentContour].Reset();
        }
        #endregion

        #region FileEvents
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox1.Text))
            {
                LoadCellList(textBox1.Text);
            }
        }

        private void lCells_SelectedIndexChanged(object sender, EventArgs e)
        {
            CellLoaded = true;
            int selected = 0;// lCells.SelectedIndex;
            for (int i = 0; i < lCells.Items.Count; i++)
            {
                if (lCells.GetSelected(i) == true)
                    selected = i;
            }
            if (File.Exists(lCells.Items[selected].ToString() + "\\data\\ProjectionObject.cct") == true)
            {
                OpenDataSet(lCells.Items[selected].ToString() + "\\data\\ProjectionObject.cct");
                lViewAxis.Enabled = true;
                Next.Enabled = true;
                instructionsToolStripMenuItem.Text = "Chose one view that you like, then click start.";
            }
            else if (File.Exists(lCells.Items[selected].ToString() + "\\data\\ProjectionObject.raw") == true)
            {
                OpenDataSet(lCells.Items[selected].ToString() + "\\data\\ProjectionObject.raw");
                lViewAxis.Enabled = true;
                Next.Enabled = true;
                instructionsToolStripMenuItem.Text = "Chose one view that you like, then click start.";
            }
            else if (File.Exists(lCells.Items[selected].ToString() + "\\500PP\\recon_8_bit\\recon8bit_000.png") == true)
            {
                OpenDataSet(lCells.Items[selected].ToString() + "\\500PP\\recon_8_bit\\");
                lViewAxis.Enabled = true;
                Next.Enabled = true;
                instructionsToolStripMenuItem.Text = "Chose one view that you like, then click start.";
            }
            else if (File.Exists(lCells.Items[selected].ToString() + "\\500PP\\recon_cropped_8bit\\reconCrop8bit_000.png") == true)
            {
                OpenDataSet(lCells.Items[selected].ToString() + "\\500PP\\recon_cropped_8bit\\");
                lViewAxis.Enabled = true;
                Next.Enabled = true;
                instructionsToolStripMenuItem.Text = "Chose one view that you like, then click start.";
            }
            else if (Directory.Exists(lCells.Items[selected].ToString() + "\\500PP") == true)
            {
                if (File.Exists(lCells.Items[selected].ToString() + "\\500PP\\CELLCONTOURS.CHF"))
                    OpenDataSet(lCells.Items[selected].ToString() + "\\500PP");
                else if (File.Exists(lCells.Items[selected].ToString() + "\\CELLCONTOURS.CHF"))
                    OpenDataSet(lCells.Items[selected].ToString());
                lViewAxis.Enabled = true;
                Next.Enabled = true;
                instructionsToolStripMenuItem.Text = "Chose one view that you like, then click start.";
            }
        }

        private void bNextCell_Click(object sender, EventArgs e)
        {
            previous.Enabled = false;
            Next.Enabled = false;
            Next.Text = "Start";
            lContourNames.Enabled = false;
            mSelectedAxis = -1;

            int i;
            string CurrentName = CurrentFileDirectory, OutName;
            /* for (i = 0; i < lCells.Items.Count; i++)
                 if (lCells.GetSelected(i) == true)
                 {
                     CurrentName = lCells.Items[i].ToString();
                     break;
                 }*/
            if (Directory.Exists(CurrentName + "\\data") == true)
                OutName = CurrentName + "\\data\\CellContours.CHF";
            else
                OutName = CurrentName + "\\CellContours.CHF";

            SaveDataSet(OutName);
            RunInterpolations(AllContours);
            viewerControl3D1.ClearOldData();

            for (i = 0; i < lCells.Items.Count; i++)
            {
                if (lCells.GetSelected(i) == true)
                {
                    try
                    {
                        lCells.SetSelected(i + 1, true);
                        if (File.Exists(lCells.Items[i + 1].ToString() + "\\data\\ProjectionObject.cct") == true)
                            OpenDataSet(lCells.Items[i + 1].ToString() + "\\data\\ProjectionObject.cct");
                        else if (File.Exists(lCells.Items[i + 1].ToString() + "\\data\\ProjectionObject.raw") == true)
                            OpenDataSet(lCells.Items[i + 1].ToString() + "\\data\\ProjectionObject.raw");
                        else if (File.Exists(lCells.Items[i + 1].ToString() + "\\500PP\\recon_cropped_8bit\\reconCrop8bit_000.png") == true)
                            OpenDataSet(lCells.Items[i + 1].ToString() + "\\500PP\\recon_cropped_8bit\\");
                        break;
                    }
                    catch { }
                }
            }

            viewerControl3D1.CollapseDisplay(-1);
            mSelectedAxis = -1;
            // listBox1.SetSelected(0, true);
            Next.Enabled = true;


        }

        private void bBrowse_Click(object sender, EventArgs e)
        {

            folderBrowserDialog1.SelectedPath = textBox1.Text;
            if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
            {
                string[] parts = folderBrowserDialog1.SelectedPath.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);

                if (parts[parts.Length - 1].Substring(0, 3) == "cct")
                {
                    string searchFolder = parts[parts.Length - 1];
                    textBox1.Text = Path.GetDirectoryName(folderBrowserDialog1.SelectedPath);
                    for (int i = 0; i < lCells.Items.Count; i++)
                        if (lCells.Items[i] == searchFolder)
                            lCells.SetSelected(i, true);
                }
                else
                    textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        #endregion

        #endregion

        #region Interpolation


        private void RunInterpolations(Dataset Contours)
        {
            // Thread t = new Thread(delegate(object Vars)
            {
                //try
                {
                    FromContours(InterpolationType.VoxelContours, Contours, false);
                }
                //catch { }
                //try
                {
                    FromContours(InterpolationType.LinearInterpolation, Contours, false);
                }
                // catch { }
                // try
                {
                    FromContours(InterpolationType.MarchingCubes, Contours, false);
                }
                // catch { }
                // try
                {
                    FromContours(InterpolationType.Delaunay, Contours, false);
                }
                // catch { }
                // try
                {
                    FromContours(InterpolationType.GaussianSplat, Contours, false);
                }
                //  catch { }
                //  try
                {
                    FromContours(InterpolationType.IsoSurface, Contours, false);
                }
                //  catch { }

            }
            MessageBox.Show("Interpolation is done");
            //);
            //t.Start();

        }
        private void DoEvaluation(double[, ,] DataCube)
        {
            //homework

        }
        /*
        private void GaussianSplat()
        {
/*
            vtkForm graphics = new vtkForm();
            graphics.Show();

            int nPoints = 0;
            for (int IContour = 0; IContour < 1/*AllContours.Contours.Count; IContour++)
            {
               // List<Point3D> Points = new List<Point3D>();
                int zoomX = AllContours.ScreenBox.Width;
                int zoomY = AllContours.ScreenBox.Height;

                double unZoomX = (double)AllContours.Depth / (double)zoomX;
                double unZoomY = (double)AllContours.Height / (double)zoomY;


                nPoints = 0;
                List<Point3D[]> Contours = ContoursToPoint3D(IContour, ref  nPoints, unZoomX, unZoomY);

               /* for (int i = 0; i < AllContours.Contours[0].SingleSlice.Count; i++)
                {
                    for (int j = 0; j < AllContours.Contours[0].SingleSlice[i].Vertixes.Count; j++)
                    {
                        for (int k = 0; k < AllContours.Contours[0].SingleSlice[i].Vertixes[j].Length; k = k + 3)
                        {
                            int x = (int)(AllContours.Contours[0].SingleSlice[i].Vertixes[j][k].X * unZoomX);
                            int y = (int)(AllContours.Contours[0].SingleSlice[i].Vertixes[j][k].Y * unZoomY);
                            Points.Add(new Point3D(x, y, i));
                        }
                    }
                }

                //MathHelpLib._3DStuff._3DInterpolation interpolation;
                object GraphicsObject;
                float[, ,] DataGrid = MathHelpLib._3DStuff._3DInterpolation.GaussianSplat(Points.ToArray(), out GraphicsObject);

                //EvaluationMetrics ... (datagrid)
                graphics.GetViewer().ShowGraphicsObject(GraphicsObject);

                string SaveDir = Path.GetDirectoryName(AllContours.Filename) + "\\interpolationCellWall_GaussianSplat\\Contour" + IContour;
                if (Directory.Exists(SaveDir) == false) Directory.CreateDirectory(SaveDir);

                MathHelpsFileLoader.SaveData(SaveDir + "\\mask.bmp", DataGrid);

                DataGrid = null;

                GC.Collect();
            }
        }

        private void Delaunay()
        {

            vtkForm graphics = new vtkForm();
            graphics.Show();


            for (int IContour = 0; IContour < 1/*AllContours.Contours.Count; IContour++)
            {
                List<Point3D> Points = new List<Point3D>();


                int zoomX = AllContours.ScreenBox.Width;
                int zoomY = AllContours.ScreenBox.Height;

                double unZoomX = (double)AllContours.Depth / (double)zoomX;
                double unZoomY = (double)AllContours.Height / (double)zoomY;


                for (int i = 0; i < AllContours.Contours[0].SingleSlice.Count; i++)
                {

                    for (int j = 0; j < AllContours.Contours[0].SingleSlice[i].Vertixes.Count; j++)
                    {
                        for (int k = 0; k < AllContours.Contours[0].SingleSlice[i].Vertixes[j].Length; k = k + 3)
                        {

                            int x = (int)(AllContours.Contours[0].SingleSlice[i].Vertixes[j][k].X * unZoomX);
                            int y = (int)(AllContours.Contours[0].SingleSlice[i].Vertixes[j][k].Y * unZoomY);

                            Points.Add(new Point3D(x, y, i));
                        }

                    }

                }

                //MathHelpLib._3DStuff._3DInterpolation interpolation;
                object GraphicsObject;
                float[, ,] DataGrid = MathHelpLib._3DStuff._3DInterpolation.Delaunay(Points.ToArray(), out GraphicsObject);

                //EvaluationMetrics ... (datagrid)
                graphics.GetViewer().ShowGraphicsObject(GraphicsObject);

                string SaveDir = Path.GetDirectoryName(AllContours.Filename) + "\\interpolationCellWall_Delauny\\Contour" + IContour;
                if (Directory.Exists(SaveDir) == false) Directory.CreateDirectory(SaveDir);

                MathHelpsFileLoader.SaveData(SaveDir + "\\mask.bmp", DataGrid);
                DataGrid = null;

                GC.Collect();
            }
        }

        private void SurfaceFromUnorganized()
        {

            vtkForm graphics = new vtkForm();
            graphics.Show();


            for (int IContour = 0; IContour < 1/*AllContours.Contours.Count; IContour++)
            {
            List<Point3D> Points = new List<Point3D>();


            int zoomX = AllContours.ScreenBox.Width;
            int zoomY = AllContours.ScreenBox.Height;

            double unZoomX = (double)AllContours.Depth / (double)zoomX;
            double unZoomY = (double)AllContours.Height / (double)zoomY;


            for (int i = 0; i < AllContours.Contours[0].SingleSlice.Count; i++)
            {

                for (int j = 0; j < AllContours.Contours[0].SingleSlice[i].Vertixes.Count; j++)
                {
                    for (int k = 0; k < AllContours.Contours[0].SingleSlice[i].Vertixes[j].Length; k = k + 3)
                    {

                        int x = (int)(AllContours.Contours[0].SingleSlice[i].Vertixes[j][k].X * unZoomX);
                        int y = (int)(AllContours.Contours[0].SingleSlice[i].Vertixes[j][k].Y * unZoomY);

                        Points.Add(new Point3D(x, y, i));
                    }
                }
            }

            //MathHelpLib._3DStuff._3DInterpolation interpolation;
            object GraphicsObject;
            float[, ,] DataGrid = MathHelpLib._3DStuff._3DInterpolation.SurfaceFromUnorganized(Points.ToArray(), out GraphicsObject);

            //EvaluationMetrics ... (datagrid)
            graphics.GetViewer().ShowGraphicsObject(GraphicsObject);

            string SaveDir = Path.GetDirectoryName(AllContours.Filename) + "\\interpolationCellWall_SurfaceFromUnorganized\\Contour" + IContour;
            if (Directory.Exists(SaveDir) == false) Directory.CreateDirectory(SaveDir);

            MathHelpsFileLoader.SaveData(SaveDir + "\\mask.bmp", DataGrid);
                 DataGrid = null;

                GC.Collect();
            }
        }

        /// <summary>
        /// obsolete and buggy
        /// </summary>
        private void MarchingCubes()
        {
            int zoomX = AllContours.ScreenBox.Width;
            int zoomY = AllContours.ScreenBox.Height;

            double unZoomX = (double)AllContours.Depth / (double)zoomX;
            double unZoomY = (double)AllContours.Height / (double)zoomY;

            List<SingleSlice> SlicesWithContours = new List<SingleSlice>();
            for (int Contour = 0; Contour < AllContours.Contours.Count; Contour++)
            {

                List<int> SliceIndex = new List<int>();
                for (int i = 0; i < AllContours.Contours[Contour].SingleSlice.Count - 1 && i < AllContours.Width; i++)
                {
                    if (AllContours.Contours[Contour].SingleSlice[i].Vertixes.Count > 0)
                    {
                        SlicesWithContours.Add(AllContours.Contours[Contour].SingleSlice[i]);
                        SliceIndex.Add(i);
                    }
                }
            }

            double[, ,] DataCube = new double[AllContours.Width, AllContours.Height, SlicesWithContours.Count];
            //all the slices in the array
            for (int i = 0; i < SlicesWithContours.Count; i++)
            {

                Bitmap b = new Bitmap(AllContours.Width, AllContours.Height);
                Graphics g = Graphics.FromImage(b);

                g.Clear(Color.Black);

                SingleSlice sSingle = SlicesWithContours[i];

                //draw all shapes on this slice
                for (int j = 0; j < sSingle.Vertixes.Count; j++)
                {
                    GraphicsPath gp = new GraphicsPath();
                    gp.StartFigure();
                    //draw the shape
                    for (int k = 100; k < sSingle.Vertixes[j].Length; k++)
                    {
                        // Create an open figure
                        Point point1 = new Point();
                        point1.X = (int)(sSingle.Vertixes[j][k - 100].X * unZoomX);
                        point1.Y = (int)(sSingle.Vertixes[j][k - 100].Y * unZoomY);

                        Point point2 = new Point();
                        point2.X = (int)(sSingle.Vertixes[j][k].X * unZoomX);
                        point2.Y = (int)(sSingle.Vertixes[j][k].Y * unZoomY);

                        gp.AddLine(point1.X, point1.Y, point2.X, point2.Y);
                    }
                    gp.CloseFigure();
                    g.FillPath(Brushes.White, gp);
                    gp.Dispose();
                }

                double[,] imageData = MathImageHelps.ConvertToDoubleArray(b, false);
                //b.Save(@"c:\temp\MC2\mc" + i.ToString() + ".bmp");
                b.Dispose();

                for (int x = 0; x < imageData.GetLength(0); x++)
                    for (int y = 0; y < imageData.GetLength(1); y++)
                        DataCube[x, y, i] = imageData[x, y];
            }

            MathHelpLib._3DStuff.MarchingCubes mc = new MathHelpLib._3DStuff.MarchingCubes();
            mc.CreateSurface(DataCube, 1, (int)((double)DataCube.GetLength(0) / 3d), (int)((double)DataCube.GetLength(1) / 3d), (int)((double)DataCube.GetLength(2) / 3d));

            Point3D[] VertexList = mc.VertexList;
            int[] TriangleIndexs = mc.TriangleIndexs;

            double[][] Points = new double[VertexList.Length][];
            for (int i = 0; i < VertexList.Length; i++)
            {
                double[] d = new double[3];
                d[0] = mc.VertexList[i].X;
                d[1] = mc.VertexList[i].Y;
                d[2] = mc.VertexList[i].Z;
                Points[i] = d;
            }

            vtkForm graphics = new vtkForm();
            graphics.Show();
            graphics.GetViewer().LoadPolyMesh(Points, mc.TriangleIndexs);
        }*/

        private void MarchingCubesVTK(Dataset TheContours, bool ShowResultNoSave)
        {
            int zoomX = TheContours.ScreenBox.Width;
            int zoomY = TheContours.ScreenBox.Height;

            double unZoomX = 1;// (double)TheContours.Depth / (double)zoomX;
            double unZoomY = 1;// (double)TheContours.Height / (double)zoomY;

            if (TheContours.Zooming)
            {
                unZoomX = (double)TheContours.Depth / (double)zoomX;
                unZoomY = (double)TheContours.Height / (double)zoomY;
            }

            vtkForm graphics = null;
            if (ShowResultNoSave)
            {
                graphics = new vtkForm();
                graphics.Show();
            }
            float[, ,] volume = new float[TheContours.Width, TheContours.Height, TheContours.Depth];
            for (int Contour = 0; Contour < TheContours.Surfaces.Count; Contour++)
            {
                string SaveDir = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(TheContours.Filename))) + "\\interpolationMarching\\Contour" + Contour.ToString();

                List<SurfaceSlice> SlicesWithContours = new List<SurfaceSlice>();
                Dictionary<int, int> SliceIndex = new Dictionary<int, int>();
                int cc = 0;
                for (int i = 1; i < TheContours.Surfaces[Contour].Slices.Count - 1 && i < TheContours.Width; i++)
                {
                    if (TheContours.Surfaces[Contour].Slices[i].Contours.Count > 0)
                    {
                        SlicesWithContours.Add(TheContours.Surfaces[Contour].Slices[i]);
                        SliceIndex.Add(cc + 5, i);
                        cc++;
                    }
                }

                if (SlicesWithContours.Count > 0)
                {
                    List<Point3D[]> AllPoints = new List<Point3D[]>();
                    cc = 0;
                    //  new float[TheContours.Depth , TheContours.Height , TheContours.Width + 40]
                    // ushort[, ,] DataCube = new ushort[SlicesWithContours.Count + 10, TheContours.Width, TheContours.Height];
                    ushort[, ,] DataCube = new ushort[SlicesWithContours.Count + 10, TheContours.Depth, TheContours.Height];
                    //all the slices in the array
                    for (int i = 0; i < SlicesWithContours.Count; i++)
                    {
                        Bitmap b = new Bitmap(TheContours.Height, TheContours.Depth);
                        #region Draw Contours

                        Graphics g = Graphics.FromImage(b);

                        g.Clear(Color.Black);

                        SurfaceSlice sSingle = SlicesWithContours[i];

                        List<Point3D> points = new List<Point3D>();
                        //draw all shapes on this slice
                        for (int j = 0; j < sSingle.Contours.Count; j++)
                        {
                            GraphicsPath gp = new GraphicsPath();
                            gp.StartFigure();
                            //draw the shape
                            for (int k = 1; k < sSingle.Contours[j].Length; k++)
                            {
                                // Create an open figure
                                Point point1 = new Point();
                                point1.X = (int)(sSingle.Contours[j][k - 1].X * unZoomX);
                                point1.Y = (int)(sSingle.Contours[j][k - 1].Y * unZoomY);

                                Point point2 = new Point();
                                point2.X = (int)(sSingle.Contours[j][k].X * unZoomX);
                                point2.Y = (int)(sSingle.Contours[j][k].Y * unZoomY);

                                gp.AddLine(point1.X, point1.Y, point2.X, point2.Y);
                                points.Add(new Point3D(point1.X, point1.Y, i + 5));
                            }
                            gp.CloseFigure();
                            g.FillPath(Brushes.White, gp);
                            gp.Dispose();
                        }
                        AllPoints.Add(points.ToArray());

                        double[,] imageData = MathImageHelps.ConvertToDoubleArray(b, false);
                        // b.Save(@"c:\temp\MC2\mc" + i.ToString() + ".bmp");
                        b.Dispose();
                        #endregion
                        for (int x = 0; x < imageData.GetLength(0); x++)
                            for (int y = 0; y < imageData.GetLength(1); y++)
                                DataCube[i + 5, y, x] = (ushort)imageData[x, y];
                    }


                    //MathHelpLib._3DStuff._3DInterpolation interpolation;
                    object GraphicsObject;
                    MathHelpLib._3DStuff._3DInterpolation.MarchingCubes(AllPoints, DataCube, SliceIndex, out GraphicsObject, ref volume);

                    //graphics.GetViewer().IsoSurface(volume, 150,false );
                    if (ShowResultNoSave)
                    {
                        //EvaluationMetrics ... (datagrid)
                        graphics.GetViewer().ShowGraphicsObject(GraphicsObject);
                    }
                    else
                    {
                        //string SaveDir = Path.GetDirectoryName(TheContours.Filename) + "\\interpolation_MatchingCubesVTK\\Contour" + Contour;

                        try
                        {
                            if (Directory.Exists(SaveDir) == true)
                                Directory.Delete(SaveDir, true);
                        }
                        catch { }
                        try
                        {
                            Directory.CreateDirectory(SaveDir);
                        }
                        catch { }
                        MathHelpsFileLoader.Save_Raw(SaveDir + "\\mask.bmp", volume);
                    }

                    DataCube = null;
                    GC.Collect();
                }
            }
        }


        /*
        private void IsoSurface()
        {

            vtkForm graphics = new vtkForm();
            graphics.Show();

            for (int IContour = 0; IContour < 1/*AllContours.Contours.Count; IContour++)
            {


                int zoomX = AllContours.ScreenBox.Width;
                int zoomY = AllContours.ScreenBox.Height;

                double unZoomX = (double)AllContours.Depth / (double)zoomX;
                double unZoomY = (double)AllContours.Height / (double)zoomY;

                List<Point3D[]> Contours = new List<Point3D[]>();

                for (int i = 0; i < AllContours.Contours[0].SingleSlice.Count; i++)
                {
                    for (int j = 0; j < AllContours.Contours[0].SingleSlice[i].Vertixes.Count; j++)
                    {
                        List<Point3D> Points = new List<Point3D>();// Point3D[AllContours.Contours[0].SingleSlice[i].Vertixes[j].Length];
                        int cc = 0;
                        for (int k = 0; k < AllContours.Contours[0].SingleSlice[i].Vertixes[j].Length; k = k + 3)
                        {
                            int x = (int)(AllContours.Contours[0].SingleSlice[i].Vertixes[j][k].X * unZoomX);
                            int y = (int)(AllContours.Contours[0].SingleSlice[i].Vertixes[j][k].Y * unZoomY);
                            Points.Add(new Point3D(x, y, i));
                        }
                        Contours.Add(Points.ToArray());
                    }
                }

                //MathHelpLib._3DStuff._3DInterpolation interpolation;
                object GraphicsObject;
                float[, ,] DataGrid = MathHelpLib._3DStuff._3DInterpolation.IsoContour(Contours, out GraphicsObject);

                //EvaluationMetrics ... (datagrid)
                graphics.GetViewer().ShowGraphicsObject(GraphicsObject);

                string SaveDir = Path.GetDirectoryName(AllContours.Filename) + "\\interpolationCellWall_IsoSurface\\Contour" + 0;
                if (Directory.Exists(SaveDir) == false) Directory.CreateDirectory(SaveDir);

                MathHelpsFileLoader.SaveData(SaveDir + "\\mask.bmp", DataGrid);

                DataGrid = null;
                GC.Collect();
            }
        }*/

        private List<Point3D[]> ContoursToPoint3D(Dataset AllContours, int ContourIndex, ref int nPoints, double unZoomX, double unZoomY)
        {
            List<Point3D[]> Contours = new List<Point3D[]>();
            if (AllContours.Surfaces[ContourIndex].Slices.Count > 0)
            {
                //if (AllContours.Surfaces[ContourIndex].Slices[0].PlaneAxis == 1)
                {
                    #region XAxis
                    for (int i = 0; i < AllContours.Surfaces[ContourIndex].Slices.Count; i++)
                    {
                        for (int j = 0; j < AllContours.Surfaces[ContourIndex].Slices[i].Contours.Count; j++)
                        {
                            List<Point3D> Points = new List<Point3D>(AllContours.Surfaces[ContourIndex].Slices[i].Contours[j].Length);
                            for (int k = 0; k < AllContours.Surfaces[ContourIndex].Slices[i].Contours[j].Length; k++)
                            {
                                float z = AllContours.Surfaces[ContourIndex].Slices[i].SliceNumber;
                                float x = (float)(AllContours.Surfaces[ContourIndex].Slices[i].Contours[j][k].X * unZoomX);
                                float y = (float)(AllContours.Surfaces[ContourIndex].Slices[i].Contours[j][k].Y * unZoomY);
                                Points.Add(new Point3D(x, y, z));
                                nPoints++;
                            }
                            Contours.Add(Points.ToArray());
                        }
                    }
                    #endregion
                }
                /*
                else if (AllContours.Surfaces[ContourIndex].Slices[0].PlaneAxis == 2)
                {
                    #region YAxis
                    for (int i = 0; i < AllContours.Surfaces[ContourIndex].Slices.Count; i++)
                    {
                        for (int j = 0; j < AllContours.Surfaces[ContourIndex].Slices[i].Contours.Count; j++)
                        {
                            List<Point3D> Points = new List<Point3D>(AllContours.Surfaces[ContourIndex].Slices[i].Contours[j].Length);
                            for (int k = 0; k < AllContours.Surfaces[ContourIndex].Slices[i].Contours[j].Length; k++)
                            {
                                float z = AllContours.Surfaces[ContourIndex].Slices[i].SliceNumber;
                                float x = (float)(AllContours.Surfaces[ContourIndex].Slices[i].Contours[j][k].X * unZoomX);
                                float y = (float)(AllContours.Surfaces[ContourIndex].Slices[i].Contours[j][k].Y * unZoomY);
                                Points.Add(new Point3D(x, z, y));
                                nPoints++;
                            }
                            Contours.Add(Points.ToArray());
                        }
                    }
                    #endregion
                }
                else
                {
                    #region ZAxis
                    for (int i = 0; i < AllContours.Surfaces[ContourIndex].Slices.Count; i++)
                    {
                        for (int j = 0; j < AllContours.Surfaces[ContourIndex].Slices[i].Contours.Count; j++)
                        {
                            List<Point3D> Points = new List<Point3D>(AllContours.Surfaces[ContourIndex].Slices[i].Contours[j].Length);
                            for (int k = 0; k < AllContours.Surfaces[ContourIndex].Slices[i].Contours[j].Length; k++)
                            {
                                float z = AllContours.Surfaces[ContourIndex].Slices[i].SliceNumber;
                                float x = (float)(AllContours.Surfaces[ContourIndex].Slices[i].Contours[j][k].X * unZoomX);
                                float y = (float)(AllContours.Surfaces[ContourIndex].Slices[i].Contours[j][k].Y * unZoomY);
                                Points.Add(new Point3D(z, y, x));
                                nPoints++;
                            }
                            Contours.Add(Points.ToArray());
                        }
                    }
                    #endregion
                }*/
            }
            return Contours;
        }

        private enum InterpolationType
        {
            VoxelContours, GaussianSplat, Delaunay, FromUnorganized, MarchingCubes, IsoSurface, LinearInterpolation
        }

        private void FromContours(InterpolationType IType, Dataset TheContours, bool ShowResultNoSave)
        {
            // try
            {
                if (IType == InterpolationType.MarchingCubes)
                {
                    MarchingCubesVTK(TheContours, ShowResultNoSave);
                    return;
                }
                if (IType == InterpolationType.LinearInterpolation)
                {
                    LinearInterpolation(TheContours, ShowResultNoSave);
                    return;
                }

                vtkForm graphics = null;
                if (ShowResultNoSave)
                {
                    graphics = new vtkForm();
                    graphics.Show();
                }

                float[, ,] volume = new float[TheContours.Width, TheContours.Height, TheContours.Depth];
                int nPoints = 0;
                for (int SurfaceI = 0; SurfaceI < TheContours.Surfaces.Count; SurfaceI++)
                {
                    string SaveDir = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(TheContours.Filename))) + "\\interpolation" + IType.ToString().Replace('.', '_') + "\\Contour" + SurfaceI.ToString();

                    int zoomX = TheContours.ScreenBox.Width;
                    int zoomY = TheContours.ScreenBox.Height;

                    double unZoomX = 1;// (double)TheContours.Depth / (double)zoomX;
                    double unZoomY = 1;// (double)TheContours.Height / (double)zoomY;

                    if (TheContours.Zooming)
                    {
                        unZoomX = (double)TheContours.Depth / (double)zoomX;
                        unZoomY = (double)TheContours.Height / (double)zoomY;
                    }

                    nPoints = 0;
                    List<Point3D[]> Contours = ContoursToPoint3D(TheContours, SurfaceI, ref  nPoints, unZoomX, unZoomY);

                    if (nPoints > 0)
                    {
                        //MathHelpLib._3DStuff._3DInterpolation interpolation;
                        object GraphicsObject = null;

                        if (IType == InterpolationType.VoxelContours)
                            MathHelpLib._3DStuff._3DInterpolation.FromContour(Contours, ref volume, out GraphicsObject);
                        else if (IType == InterpolationType.GaussianSplat)
                            MathHelpLib._3DStuff._3DInterpolation.GaussianSplat(Contours, ref volume, out GraphicsObject);
                        else if (IType == InterpolationType.Delaunay)
                            MathHelpLib._3DStuff._3DInterpolation.Delaunay(Contours, ref volume, out GraphicsObject);
                        else if (IType == InterpolationType.FromUnorganized)
                            MathHelpLib._3DStuff._3DInterpolation.SurfaceFromUnorganized(Contours, ref volume, out GraphicsObject);
                        else if (IType == InterpolationType.IsoSurface)
                            MathHelpLib._3DStuff._3DInterpolation.IsoContour(Contours, ref volume, out GraphicsObject);

                        if (graphics != null && volume != null)
                            graphics.GetViewer().IsoSurface(volume, 150, false, Colors[SurfaceI % Colors.Length]);
                        //EvaluationMetrics ... (datagrid)
                        if (ShowResultNoSave)
                        {
                            graphics.GetViewer().ShowGraphicsObject(GraphicsObject);
                        }
                        else
                        {

                            if (Directory.Exists(SaveDir) == true)
                                Directory.Delete(SaveDir, true);

                            Directory.CreateDirectory(SaveDir);
                            MathHelpsFileLoader.Save_Raw(SaveDir + "\\mask.bmp", volume);
                        }
                    }
                    Contours = null;
                    GC.Collect();
                }
            }
            //  catch (Exception ex)
            {
                //     MessageBox.Show(IType.ToString() + " failed\n" + ex.Message);

            }
        }

        Color[] Colors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Cyan, Color.Magenta, Color.MediumAquamarine, Color.MintCream, Color.PaleTurquoise };
        private void LinearInterpolation(Dataset TheContours, bool ShowResultNoSave)
        {
            int zoomX = TheContours.ScreenBox.Width;
            int zoomY = TheContours.ScreenBox.Height;

            double unZoomX = 1;// (double)TheContours.Depth / (double)zoomX;
            double unZoomY = 1;// (double)TheContours.Height / (double)zoomY;

            if (TheContours.Zooming)
            {
                unZoomX = (double)TheContours.Depth / (double)zoomX;
                unZoomY = (double)TheContours.Height / (double)zoomY;
            }


            vtkForm graphics;// = null;
            // if (ShowResultNoSave)
            {
                graphics = new vtkForm();
                graphics.Show();
            }


            // return;


            for (int SurfaceI = 0; SurfaceI < TheContours.Surfaces.Count; SurfaceI++)
            {

                int nContours = 0;
                for (int i = 0; i < TheContours.Surfaces[SurfaceI].Slices.Count - 1 && i < TheContours.Width; i++)
                {
                    for (int j = 0; j < TheContours.Surfaces[SurfaceI].Slices.Count; j++)
                        nContours += TheContours.Surfaces[SurfaceI].Slices[j].Contours.Count;
                }

                if (nContours > 0)
                {
                    //string SaveDir = Path.GetDirectoryName(TheContours.Filename) + "\\interpolation_Linear\\Contour" + Contour;
                    string SaveDir = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(TheContours.Filename))) + "\\interpolationLinear\\Contour" + SurfaceI.ToString();
                    if (Directory.Exists(SaveDir) == true)
                        //Directory.Delete(SaveDir, true);
                        // 
                        Directory.CreateDirectory(SaveDir);


                    SurfaceSlice[] TestSlices = TheContours.Surfaces[SurfaceI].Slices.ToArray();
                    /* for (int k = 0; k < TestSlices.Length; k++)
                         if (k % 10 != 0)
                             TestSlices[k] = new SurfaceSlice(TestSlices[k].SliceNumber);*/

                    int PlaneAxis = -1;
                    for (int i = 0; i < TheContours.Surfaces[SurfaceI].Slices.Count - 1 && i < TheContours.Width; i++)
                    {
                        SurfaceSlice currentSlice = TestSlices[i];
                        if (currentSlice.Contours.Count > 0)
                        {
                            PlaneAxis = TheContours.Surfaces[SurfaceI].Slices[i].PlaneAxis;
                            break;
                        }
                    }
                    //create the cube for all the contours to be placed inside
                    SurfaceSlice[] Slices = null;

                    if (PlaneAxis == 2)
                    {
                        Slices = new SurfaceSlice[VolumeData.GetLength(1)];
                    }
                    else if (PlaneAxis == 0)
                    {
                        Slices = new SurfaceSlice[VolumeData.GetLength(2)];
                    }
                    else
                        Slices = new SurfaceSlice[VolumeData.GetLength(0)];

                    // return;
                    for (int i = 0; i < Slices.Length; i++)
                    {
                        Slices[i] = new SurfaceSlice(i);
                    }

                    SurfaceSlice LastSlice = new SurfaceSlice();
                    LastSlice.PlaneAxis = -1;

                    for (int i = 0; i < TheContours.Surfaces[SurfaceI].Slices.Count - 1 && i < TheContours.Width; i++)
                    {
                        SurfaceSlice currentSlice = TestSlices[i];

                        #region DoInterpolation
                        //if the contour is drawn then just put it in the right place
                        if (currentSlice.Contours.Count > 0)
                        {
                            Slices[TheContours.Surfaces[SurfaceI].Slices[i].SliceNumber] = TheContours.Surfaces[SurfaceI].Slices[i];
                            LastSlice = TheContours.Surfaces[SurfaceI].Slices[i];
                        }
                        //if it is not and at least one slice has been drawn, start interpolating
                        else if (LastSlice.PlaneAxis != -1 && currentSlice.Contours.Count == 0)
                        {
                            //find the next drawn on  slice
                            SurfaceSlice NextSlice = new SurfaceSlice();
                            bool found = false;
                            int nextSliceIndex = i + 1;
                            for (; nextSliceIndex < TheContours.Surfaces[SurfaceI].Slices.Count; nextSliceIndex++)
                            {
                                //if (TheContours.Surfaces[Contour].Slices[nextSliceIndex].Contours.Count > 0)
                                if (TestSlices[nextSliceIndex].Contours.Count > 0)
                                {
                                    //NextSlice = TheContours.Surfaces[Contour].Slices[nextSliceIndex];
                                    NextSlice = TestSlices[nextSliceIndex];
                                    found = true;
                                    break;
                                }
                            }
                            //if a next slice is found then interpolate (if last slice was last drawn slice then errors happen
                            if (found)
                            {
                                #region Get more complicated slice
                                SurfaceSlice MoreComplicated = new SurfaceSlice(), LessComplicated = new SurfaceSlice();
                                if (LastSlice.Contours.Count > NextSlice.Contours.Count)
                                {
                                    MoreComplicated = LastSlice;
                                    LessComplicated = NextSlice;
                                }
                                else if (LastSlice.Contours.Count < NextSlice.Contours.Count)
                                {
                                    LessComplicated = LastSlice;
                                    MoreComplicated = NextSlice;
                                }
                                else
                                {
                                    if (LastSlice.Contours[0].Length > NextSlice.Contours[0].Length)
                                    {
                                        MoreComplicated = LastSlice;
                                        LessComplicated = NextSlice;
                                    }
                                    else
                                    {
                                        LessComplicated = LastSlice;
                                        MoreComplicated = NextSlice;

                                    }
                                }
                                #endregion

                                #region Get All Target points
                                int nPoints = 0;
                                for (int lcI = 0; lcI < LessComplicated.Contours.Count; lcI++)
                                {
                                    nPoints += LessComplicated.Contours[lcI].Length;
                                }
                                Point[] second = new Point[nPoints];
                                int mm = 0;
                                for (int lcI = 0; lcI < LessComplicated.Contours.Count; lcI++)
                                {
                                    for (int k = 0; k < LessComplicated.Contours[lcI].Length; k++)
                                    {
                                        second[mm] = new Point((int)LessComplicated.Contours[lcI][k].X, (int)LessComplicated.Contours[lcI][k].Y);
                                        mm++;
                                    }
                                }
                                #endregion


                                PointF[][] InterpolatedPoints = new PointF[(int)Math.Abs(LessComplicated.SliceNumber - MoreComplicated.SliceNumber)][];

                                for (int mcI = 0; mcI < MoreComplicated.Contours.Count; mcI++)
                                {
                                    #region Load all points for the current slice and  contour
                                    Point[] first = new Point[MoreComplicated.Contours[mcI].Length];
                                    int m = 0;
                                    for (int k = 0; k < MoreComplicated.Contours[mcI].Length; k++)
                                    {
                                        first[m] = new Point((int)MoreComplicated.Contours[mcI][k].X, (int)MoreComplicated.Contours[mcI][k].Y);
                                        m++;
                                    }
                                    #endregion

                                    //load up a target interpolation for all the intervening slices
                                    for (int k = 0; k < InterpolatedPoints.Length; k++)
                                    {
                                        InterpolatedPoints[k] = new PointF[first.Length];
                                    }

                                    //set up the need slice number interpolations
                                    double min = MoreComplicated.SliceNumber;
                                    double max = LessComplicated.SliceNumber;
                                    double length = max - min;

                                    //this is the actual bottom position in the slice array
                                    int ZeroPosition = (int)min;
                                    if (min > max)
                                        ZeroPosition = (int)max;

                                    for (int j = 0; j < first.Length; j++)
                                    {
                                        #region Find minimum distance
                                        double minDistance = Math.Sqrt((first[j].X - second[0].X) * (first[j].X - second[0].X) +
                                            (first[j].Y - second[0].Y) * (first[j].Y - second[0].Y));
                                        Point closestPoint = second[0];

                                        for (int k = 1; k < second.Length; k++)
                                        {
                                            double currentDistance = Math.Sqrt((first[j].X - second[k].X) * (first[j].X - second[k].X) +
                                                (first[j].Y - second[k].Y) * (first[j].Y - second[k].Y));

                                            if (currentDistance < minDistance)
                                            {
                                                minDistance = currentDistance;
                                                closestPoint = second[k];
                                            }
                                        }
                                        #endregion

                                        #region Interpolate through all the slices
                                        //first and last are already draw curves so they are not interpolated
                                        for (int k = 1; k < InterpolatedPoints.Length; k++)
                                        {
                                            double u = (double)k / (double)InterpolatedPoints.Length;
                                            PointF interpolated = new PointF();
                                            interpolated.X = (float)((closestPoint.X - first[j].X) * u + first[j].X);
                                            interpolated.Y = (float)((closestPoint.Y - first[j].Y) * u + first[j].Y);
                                            int index = (int)((u * length + min) - ZeroPosition);
                                            InterpolatedPoints[index][j] = interpolated;
                                        }
                                        #endregion
                                    }

                                    //add all the interpolated points
                                    for (int k = 1; k < InterpolatedPoints.Length; k++)
                                    {
                                        Slices[ZeroPosition + k].Contours.Add(InterpolatedPoints[k]);

                                    }
                                }
                            }
                        }
                        #endregion
                    }

                    float[, ,] DataCube;


                    DataCube = new float[TheContours.Depth, TheContours.Height, TheContours.Width];//TheContours.Depth, TheContours.Height, TheContours.Width];
                    //all the slices in the array
                    for (int i = 0; i < Slices.Length; i++)
                    {
                        #region DrawSlices
                        Bitmap b = null;
                        if (PlaneAxis == 2)
                            b = new Bitmap(VolumeData.GetLength(0), VolumeData.GetLength(2));
                        else if (PlaneAxis == 0)
                            b = new Bitmap(VolumeData.GetLength(0), VolumeData.GetLength(1));
                        else
                            b = new Bitmap(VolumeData.GetLength(2), VolumeData.GetLength(1));

                        Graphics g = Graphics.FromImage(b);

                        g.Clear(Color.Black);

                        SurfaceSlice sSingle = Slices[i];

                        //draw all shapes on this slice
                        if (sSingle.Contours != null || sSingle.Contours.Count > 0)
                        {
                            for (int j = 0; j < sSingle.Contours.Count; j++)
                            {
                                GraphicsPath gp = new GraphicsPath();
                                gp.StartFigure();
                                //draw the shape
                                for (int k = 1; k < sSingle.Contours[j].Length; k++)
                                {
                                    // Create an open figure
                                    Point point1 = new Point();
                                    point1.X = (int)(sSingle.Contours[j][k - 1].X * unZoomX);
                                    point1.Y = (int)(sSingle.Contours[j][k - 1].Y * unZoomY);

                                    Point point2 = new Point();
                                    point2.X = (int)(sSingle.Contours[j][k].X * unZoomX);
                                    point2.Y = (int)(sSingle.Contours[j][k].Y * unZoomY);

                                    gp.AddLine(point1.X, point1.Y, point2.X, point2.Y);
                                }
                                gp.CloseFigure();
                                g.FillPath(Brushes.White, gp);
                                gp.Dispose();
                            }
                        }
                        else
                            System.Diagnostics.Debug.Print("Problem");

                        double[,] imageData = MathImageHelps.ConvertToDoubleArray(b, false);

                        b.Dispose();
                        #endregion
                        System.Diagnostics.Debug.Write(Slices[i].SliceNumber.ToString() + " ");
                        if (PlaneAxis == 2)
                        {
                            for (int x = 0; x < imageData.GetLength(0); x++)
                                for (int y = 0; y < imageData.GetLength(1); y++)
                                    DataCube[x, Slices[i].SliceNumber, y] = (ushort)imageData[x, y];
                        }
                        else if (PlaneAxis == 0)
                        {
                            for (int x = 0; x < imageData.GetLength(0); x++)
                                for (int y = 0; y < imageData.GetLength(1); y++)
                                    DataCube[x, y, Slices[i].SliceNumber] = (ushort)imageData[x, y];
                        }
                        else
                        {
                            for (int x = 0; x < imageData.GetLength(0); x++)
                                for (int y = 0; y < imageData.GetLength(1); y++)
                                    DataCube[Slices[i].SliceNumber, y, x] = (ushort)imageData[x, y];
                        }

                        //DataCube[y, x, Slices[i].SliceNumber] = (ushort)imageData[x, y];

                    }

                    //graphics.GetViewer().IsoSurface(VolumeData,50, true);
                    graphics.GetViewer().ShowVolumeData(VolumeData);

                    //   TheContours.Width, TheContours.Height, TheContours.Depth
                    if (!ShowResultNoSave)
                    {
                        if (Directory.Exists(SaveDir) == true)
                            Directory.Delete(SaveDir, true);
                        Directory.CreateDirectory(SaveDir);
                        MathHelpsFileLoader.Save_Raw(SaveDir + "\\mask" + ".bmp", DataCube);
                        //     b.Save(SaveDir + "\\mask" + string.Format("{0:000}", i) + ".bmp");
                    }

                    //  if (ShowResultNoSave)
                    //  if (graphics != null && DataCube != null)

                    graphics.GetViewer().IsoSurface(DataCube, 150, true, Colors[SurfaceI % Colors.Length]);

                    DataCube = null;
                }
                GC.Collect();
            }
        }


        #endregion

        private void AllInterpolationsMenuItem_Click(object sender, EventArgs e)
        {
            var Contours = AllContours;

            vtkForm graphics = null;
            //   graphics = new vtkForm();
            //  graphics.Show();
            float max = VolumeData.MaxArray();
            float min = VolumeData.MinArray();

            // graphics.GetViewer().IsoSurface(VolumeData, 150,true );
            //  FromContours(InterpolationType.VoxelContours, Contours, true);

            //catch { }
            //try
            {
          //      FromContours(InterpolationType.LinearInterpolation, Contours, false);
            }
            // catch { }
          //  return;
            Application.DoEvents();
            GC.Collect();
            try
            {
               // FromContours(InterpolationType.MarchingCubes, Contours, false);
            }
            catch { }
            Application.DoEvents();
            GC.Collect();
            try
            {
                FromContours(InterpolationType.GaussianSplat, Contours, false);
            }
            catch { }
            Application.DoEvents();
            GC.Collect();
            try
            {
                FromContours(InterpolationType.Delaunay, Contours, false);
            }
            catch { }
            Application.DoEvents();
            GC.Collect();

            try
            {
                FromContours(InterpolationType.FromUnorganized, Contours, false);
            }
            catch { }
            Application.DoEvents();
            GC.Collect();

            try
            {
                // FromContours(InterpolationType.IsoSurface, Contours, false);
            }
            catch { }
        }

        public void SaveAllInterpolations(string Filename)
        {

            /*  List<string> BadDirs = new List<string>();
              List<string> GoodDirs = new List<string>();
              bool FirstDir = true;
              string[] AllDirs;

              BadDirs.Add(@"S:\Research\Cell CT\Ground Truth contours");
              while (FirstDir == true && BadDirs.Count > 0)
              {
                  try
                  {
                      string[] Dirs = Directory.GetDirectories(BadDirs[0]);
                      if (Dirs.Length > 0)
                      {
                          if (Dirs[0].Contains("cct") == true && Path.GetFileName(Dirs[0]).Length > 6)
                          {
                              for (int i = 0; i < Dirs.Length; i++)
                              {
                                  string[] files = Directory.GetFiles(Dirs[i], "CellContours.*");
                                  if (files.Length>0)
                                      GoodDirs.AddRange(files);
                              }
                          }
                          else
                              BadDirs.AddRange(Dirs);
                      }
                  }
                  catch { }
                  BadDirs.RemoveAt(0);
              }


              foreach (string s in GoodDirs)
              {
                  System.Diagnostics.Debug.Print(s);*/
            try
            {
               // OpenDataSet(s);
                OpenDataSet(Filename);
                lViewAxis.Enabled = true;
                Next.Enabled = true;
                instructionsToolStripMenuItem.Text = "Chose one view that you like, then click start.";
                AllInterpolationsMenuItem_Click(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("");
            }
            //}
        }
        private void redoAllContoursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //  string[] Files = Directory.GetFiles(@"S:\Research\Cell CT\Evaluation\", "*.chf", SearchOption.AllDirectories);

            //List<string> DirAdds = 
            List<string> BadDirs = new List<string>();
            List<string> GoodDirs = new List<string>();
            bool FirstDir = true;
            string[] AllDirs;

            BadDirs.Add(@"S:\Research\Cell CT\Ground Truth contours");
            while (FirstDir == true && BadDirs.Count > 0)
            {
                try
                {
                    string[] Dirs = Directory.GetDirectories(BadDirs[0]);
                    if (Dirs.Length > 0)
                    {
                        if (Dirs[0].Contains("cct") == true && Path.GetFileName(Dirs[0]).Length > 6)
                        {
                            for (int i = 0; i < Dirs.Length; i++)
                            {
                                string[] files = Directory.GetFiles(Dirs[i], "CellContours.*");
                                GoodDirs.AddRange(files);
                            }
                        }
                        else
                            BadDirs.AddRange(Dirs);
                    }
                }
                catch { }
                BadDirs.RemoveAt(0);
            }


           // foreach (string s in GoodDirs)
            {
                // if (Directory.Exists(Path.GetDirectoryName(s) + "\\interpolationIsoSurface") == false)
                {
                    string s2 = @"S:\Research\Cell CT\Ground Truth contours\Kat\ConciseEvaluation\VG\cct001_20120103_151207";
                    Process ScriptRunner = new Process();
                    ScriptRunner.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    ScriptRunner.StartInfo.FileName = Application.ExecutablePath;
                    ScriptRunner.StartInfo.Arguments = "\"" + s2 + "\"";
                  //  ScriptRunner.Start();

                    Stopwatch sw = new Stopwatch();
                    sw.Reset();
                    sw.Start();
                  //  while (sw.Elapsed.Minutes < 1)
                    {
                   //     Application.DoEvents();
                     //   Thread.Sleep(100);
                    }
                    SaveAllInterpolations(s2);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            lViewAxis.SetSelected(0, true);
            instructionsToolStripMenuItem.Text = "Use File/Open Volume to load image.";
            SaveAllInterpolations(args);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.Elapsed.Minutes < 5)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
            this.Close();

        }


    }
}
