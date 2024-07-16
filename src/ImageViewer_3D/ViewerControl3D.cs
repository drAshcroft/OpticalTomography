using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ImageViewer3D.Tools;
using System.Reflection;
using System.IO;
using ImageViewer3D.Filters;
using ImageViewer.Filters;
using ImageViewer;

namespace ImageViewer3D
{
    public delegate void MacroLineGeneratedEvent(string MacroLine);
    public delegate void ToolTipUpdateEvent(string ToolTip);
    public delegate void SelectionPerformedEvent(ISelection3D Selection);
    public partial class ViewerControl3D : UserControl
    {
        #region Events
        public event SelectionPerformedEvent SelectionPerformed;
        public event ToolTipUpdateEvent ToolTipUpdate;
        public new event EventHandler OnClick;
        #endregion

        #region InternalVars

        private aDrawingTool3D mCurrentTool;
        private aDrawingTool3D mLastTool;

        private ISelection3D mLastSelection;
        private MenuStrip mMenuStrip;

        private PictureDisplay3DSlice[] Displays;
        private ScreenProperties3D[] mScreens;
        private DataEnvironment3D mDataEnvironment;

        private UserControl mExtraControl;

        static aDrawingTool3D[] DrawingToolStore;
        static IEffect3D[] EffectToolStore;
        static string[] ScriptStore;

        HistoryMonitor mHistoryMonitor = new HistoryMonitor();

        #endregion

        #region public_Properties

        public MenuStrip MyMenu
        {
            get { return mMenuStrip; }
        }

        public aDrawingTool3D ActiveDrawingTool
        {
            get { return mCurrentTool; }
            set
            {
                if (mCurrentTool != null)
                    mCurrentTool.EraseSelection();
                mLastTool = null;
                mCurrentTool = value;
            }
        }

        public bool DrawingToolsVisible
        {
            get { return panel2.Visible; }
            set
            {
                /* if (value == false)
                 {
                     panel2.Visible = false;
                     panel1.Width = this.Width;
                 }
                 else
                 {
                     panel2.Visible = true;
                     panel1.Width = this.Width - panel2.Width;
                 }*/

            }
        }


        public ISelection3D SelectedArea
        {
            get { return mLastSelection; }
        }

        public void AddDrawingTool(aDrawingTool3D addTool)
        {
            ToolbarButton3D adtB = new ToolbarButton3D(addTool);
            adtB.Dock = DockStyle.Top;
            adtB.Text = addTool.GetToolName();
            adtB.Click += new EventHandler(adtB_Click);
            adtB.KeyDown += new KeyEventHandler(adtB_KeyDown);
            adtB.KeyPress += new KeyPressEventHandler(adtB_KeyPress);
            adtB.KeyUp += new KeyEventHandler(adtB_KeyUp);
            panel2.Controls.Add(adtB);

            if (addTool.GetType() == typeof(ROITool3D))
                mCurrentTool = addTool;
        }

        public void AddMenuItem(IEffect3D effect)
        {
            if (mMenuStrip == null)
                mMenuStrip = new MenuStrip();

            ToolStripMenuItem mainMenu = (ToolStripMenuItem)FindMenuItem(mMenuStrip, effect.EffectMenu);
            if (mainMenu == null)
            {
                mainMenu = new ToolStripMenuItem(effect.EffectMenu);
                mMenuStrip.Items.Add(mainMenu);
            }

            ToolStripMenuItem subMenu;
            if (effect.EffectSubMenu != "")
            {
                subMenu = (ToolStripMenuItem)FindMenuItem(mainMenu, effect.EffectSubMenu);
                if (subMenu == null)
                {
                    subMenu = new ToolStripMenuItem(effect.EffectSubMenu);
                    mainMenu.DropDownItems.Add(subMenu);

                }
            }
            else
                subMenu = mainMenu;


            EffectMenuItem3D emi = (EffectMenuItem3D)FindMenuItem(subMenu, effect.EffectName);

            if (emi == null)
                emi = new EffectMenuItem3D(effect, effect.EffectName);
            else
                emi.MenuEffect = effect;
            emi.Click += new EventHandler(emi_Click);
            if (subMenu.DropDownItems.Count == 0)
            {
                subMenu.DropDownItems.Add(emi);
            }
            else
            {
                for (int i = 0; i < subMenu.DropDownItems.Count; i++)
                {
                    try
                    {
                        if (subMenu.DropDownItems[i].GetType() == typeof(EffectMenuItem3D))
                        {
                            if (effect.OrderSuggestion <= ((EffectMenuItem3D)subMenu.DropDownItems[i]).MenuEffect.OrderSuggestion)
                            {
                                subMenu.DropDownItems.Insert(i, emi);
                                return;
                            }
                        }
                    }
                    catch { }
                }
            }
            subMenu.DropDownItems.Add(emi);
        }

        public bool ProportionalZooming
        {
            get
            {
                if (Displays != null && mDataEnvironment.OriginalData != null)
                    return mDataEnvironment.ProportionalZoom;
                else
                    return true;
            }
            set
            {
                try
                {
                    mDataEnvironment.ProportionalZoom = value;
                }
                catch { }
            }

        }

        public UserControl ExtraControl
        {
            get
            {
                return mExtraControl;
            }
            set
            {
                if (mExtraControl != null)
                {
                    panel1.Controls.Remove(mExtraControl);
                    mExtraControl = null;
                }

                if (value != null)
                {
                    mExtraControl = value;
                    panel1.Controls.Add(mExtraControl);
                    mExtraControl.Visible = true;
                    mExtraControl.BringToFront();
                }
            }

        }

        public InterpolationMode ZoomInterpolationMethod
        {
            get
            {
                if (Displays != null && mDataEnvironment.OriginalData != null)
                    return mDataEnvironment.InterpolationMode;
                else
                    return InterpolationMode.Default;
            }
            set
            {
                try
                {
                    mDataEnvironment.InterpolationMode = value;
                }
                catch { }
            }
        }

        public void ShowFileScript(string FilePath)
        {
            PythonScripting.MacroRecorder3D mr = new ImageViewer3D.PythonScripting.MacroRecorder3D();

            object[] mMacroToken = new object[] { FilePath, true };

            mr.DoEffect(mDataEnvironment, null, null, mMacroToken);
        }

        public void ShowTextScript(string Script)
        {
            PythonScripting.MacroRecorder3D mr = new ImageViewer3D.PythonScripting.MacroRecorder3D();

            object[] mMacroToken = new object[] { Script, false };

            mr.DoEffect(mDataEnvironment, null, null, mMacroToken);
        }

        private delegate void SetImageDelegate(float[, ,] DisplayImage);
        public void SetImage(float[, ,] DisplayImage)
        {
            if (InvokeRequired)
            {
                this.Invoke(new SetImageDelegate(SetImage), DisplayImage);
            }
            else
            {
                SetUpImage(DisplayImage);

            }
        }
        private void SetUpImage(float[, ,] DisplayVolume)
        {
            if (DisplayVolume != null)
            {
                //cleanse all the existing references of the large 3D volume data
                if (mDataEnvironment.OriginalData != null)
                    mDataEnvironment.OriginalData.Data = null;

                mDataEnvironment.ActiveSelectedImage = new DataHolder(DisplayVolume);

                mDataEnvironment.AutoSetContrast();

                mScreens[0].SliceIndex = mDataEnvironment.FullSize.Width / 2;
                mScreens[1].SliceIndex = mDataEnvironment.FullSize.Height / 2;
                mScreens[2].SliceIndex = mDataEnvironment.FullSize.Depth / 2;
                // pictureDisplay3D1.LoadVolumeData(DisplayVolume);
            }
        }

        private delegate void SetImageDDelegate(double[, ,] DisplayImage);
        public void SetImage(double[, ,] DisplayImage)
        {
            if (InvokeRequired)
            {
                this.Invoke(new SetImageDDelegate(SetImage), DisplayImage);
            }
            else
            {
                SetUpImage(DisplayImage);

            }
        }
        private void SetUpImage(double[, ,] DisplayVolume)
        {
            if (DisplayVolume != null)
            {
                //cleanse all the existing references of the large 3D volume data
                if (mDataEnvironment.OriginalData != null)
                    mDataEnvironment.OriginalData.Data = null;

                mDataEnvironment.ActiveSelectedImage = new DataHolder(DisplayVolume);

                mDataEnvironment.AutoSetContrast();

                mScreens[0].SliceIndex = mDataEnvironment.FullSize.Width / 2;
                mScreens[1].SliceIndex = mDataEnvironment.FullSize.Height / 2;
                mScreens[2].SliceIndex = mDataEnvironment.FullSize.Depth / 2;
                // pictureDisplay3D1.LoadVolumeData(DisplayVolume);
            }
        }

        private delegate void SetImageFDelegate(string[] DisplayImage);
        public void SetImage(string[] DisplayImage)
        {
            if (InvokeRequired)
            {
                this.Invoke(new SetImageFDelegate(SetImage), DisplayImage);
            }
            else
            {
                SetUpImage(DisplayImage);

            }
        }
        private void SetUpImage(string[] DisplayVolume)
        {
            if (DisplayVolume != null)
            {
                SetUpImage(LoadStackImages(DisplayVolume));
            }
        }

        private double[, ,] LoadStackImages(string[] Filenames)
        {
            string Extension = Path.GetExtension(Filenames[0]).ToLower();
            if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                Filenames = EffectHelps.SortNumberedFiles(Filenames);

                int sizeX, sizeY, sizeZ;

                Bitmap b = new Bitmap(Filenames[0]);
                sizeX = b.Width;
                sizeY = b.Height;
                sizeZ = Filenames.Length;
                double[, ,] mDensityGrid = new double[sizeX, sizeY, sizeZ];

                for (int z = 0; z < sizeZ; z++)
                {
                    b = new Bitmap(Filenames[z]);
                    double[,] Data = ImagingTools.ConvertToDoubleArray(b, false);
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[x, y, z] = Data[x, y];
                        }
                    }
                }
                return mDensityGrid;
            }
            else if (Extension == ".ivg")
            {
                Filenames = EffectHelps.SortNumberedFiles(Filenames);

                int sizeX, sizeY, sizeZ;

                ImageViewer.ImageHolder b = ImageViewer.ImagingTools.LoadIVGFile(Filenames[0]);
                sizeX = b.Width;
                sizeY = b.Height;
                sizeZ = Filenames.Length;
                double[, ,] mDensityGrid = new double[sizeX, sizeY, sizeZ];

                for (int z = 0; z < sizeZ; z++)
                {
                    b = ImageViewer.ImagingTools.LoadIVGFile(Filenames[z]);
                    double[,] Data = ImageViewer.ImagingTools.ConvertToDoubleArray(b, false);
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[y, x, z] = Data[y, x];
                        }
                    }
                }
                return mDensityGrid;
            }
            return null;
        }

        public void FormClosing()
        {
            mDataEnvironment.OriginalData.Data = null;
        }
        public event MacroLineGeneratedEvent MacroLineGenerated;

        public Bitmap[] GetVisibleImages()
        {
            Bitmap[] b = new Bitmap[3];
            b[0] = mDataEnvironment.Screens[0].ScreenBackBuffer;
            b[1] = mDataEnvironment.Screens[1].ScreenBackBuffer;
            b[2] = mDataEnvironment.Screens[2].ScreenBackBuffer;
            return b;
        }
        #endregion

        #region Initialize the Control and load tools

        private ToolStripItem FindMenuItem(MenuStrip menu, string ItemName)
        {
            foreach (ToolStripItem tsi in menu.Items)
            {
                if (tsi.Text.ToLower() == ItemName.ToLower())
                {
                    return tsi;
                }
            }
            return null;
        }
        private ToolStripItem FindMenuItem(ToolStripMenuItem menu, string ItemName)
        {
            foreach (ToolStripItem tsi in menu.DropDownItems)
            {
                if (tsi.Text.ToLower() == ItemName.ToLower())
                {
                    return tsi;
                }
            }
            return null;
        }
        public ViewerControl3D()
        {
            InitializeComponent();
            try
            {
                StartControl();
            }
            catch { }
        }

        public void StartControl()
        {
            //placeholder image to make sure that the zoom functions can operate

            Displays = new PictureDisplay3DSlice[3];
            Displays[1] = pictureDisplay1;
            Displays[0] = pictureDisplay3DSlice1;
            Displays[2] = pictureDisplay3DSlice2;

            mDataEnvironment = new DataEnvironment3D(this);
            mScreens = new ScreenProperties3D[3];
            mDataEnvironment.Screens = mScreens;
            for (int i = 0; i < 3; i++)
            {
                mScreens[i] = new ScreenProperties3D(mDataEnvironment, Displays[i], this, i);
                mScreens[i].ScreenFrontBuffer = new Bitmap(Displays[i].Width, Displays[i].Height, PixelFormat.Format32bppRgb);
                mScreens[i].ScreenBackBuffer = new Bitmap(Displays[i].Width, Displays[i].Height, PixelFormat.Format32bppRgb);
                try
                {
                    Displays[i].SetImage();
                }
                catch { }

                Displays[i].ScreenProperties = mScreens[i];
                Displays[i].MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
                Displays[i].MouseMove += new MouseEventHandler(pictureBox1_MouseMove);
                Displays[i].MouseUp += new MouseEventHandler(pictureBox1_MouseUp);


            }
            mDataEnvironment.ContrastChanged += new DataEnvironment3D.ContrastChangedEvent(mDataEnvironment_ContrastChanged);

            crossDisplay1.ScreenXY = mScreens[2];
            crossDisplay1.ScreenXZ = mScreens[0];
            crossDisplay1.ScreenYZ = mScreens[1];

            mMenuStrip = new MenuStrip();
            string[] Menus = MenuStructure.MainMenuStructure();
            for (int i = 0; i < Menus.Length; i++)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(Menus[i]);
                mMenuStrip.Items.Add(tsmi);
                string[] SubMenus = MenuStructure.SubMenuStructure(Menus[i]);
                if (SubMenus != null)
                {
                    for (int j = 0; j < SubMenus.Length; j++)
                    {
                        tsmi.DropDownItems.Add(SubMenus[j]);
                    }
                }
            }

            try
            {
                LoadTools();
            }
            catch { }
            try
            {
                LoadScripts();
            }
            catch { }


            ToolStripMenuItem mainMenu = (ToolStripMenuItem)FindMenuItem(mMenuStrip, "Edit");
            ToolStripMenuItem subMenu = (ToolStripMenuItem)FindMenuItem(mainMenu, "Undo");
            subMenu.Click += new EventHandler(Undo_Click);

            subMenu = (ToolStripMenuItem)FindMenuItem(mainMenu, "Redo");
            subMenu.Click += new EventHandler(Redo_Click);
        }



        void Undo_Click(object sender, EventArgs e)
        {
            //mHistoryMonitor.StepBackMomento(mScreens);
        }
        void Redo_Click(object sender, EventArgs e)
        {
            // mHistoryMonitor.StepForwardMomento(mScreens);
        }

        private void LoadTools()
        {
            if (DrawingToolStore == null)
            {
                List<aDrawingTool3D> drawStore = new List<aDrawingTool3D>();
                List<string> AllFiles = new List<string>();
                List<IEffect3D> effectStore = new List<IEffect3D>();
                string[] Filenames = Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath), "plug*.dll"); // Path.GetDirectoryName(Application.ExecutablePath) + "\\ImageViewer3D.dll";
                AllFiles.AddRange(Filenames);
                AllFiles.Add(Path.GetDirectoryName(Application.ExecutablePath) + "\\ImageViewer3D.dll");
                Filenames = AllFiles.ToArray();
                for (int i = 0; i < Filenames.Length; i++)
                {
                    try
                    {
                        Assembly asm = Assembly.LoadFrom(Filenames[i]);
                        Type[] alltypes = asm.GetTypes();
                        foreach (Type temp in alltypes)
                        {
                            if (temp != typeof(ViewerControl3D) && temp.IsAbstract == false)
                            {
                                try
                                {
                                    ConstructorInfo[] ci = temp.GetConstructors();
                                    aDrawingTool3D aDT = (aDrawingTool3D)ci[0].Invoke(null);

                                    AddDrawingTool(aDT);
                                    drawStore.Add(aDT);
                                }
                                catch { }
                            }

                            if (temp.GetInterface("IEffect3D") != null)
                            {
                                try
                                {
                                    ConstructorInfo[] ci = temp.GetConstructors();
                                    IEffect3D aDT = (IEffect3D)ci[0].Invoke(null);

                                    AddMenuItem(aDT);
                                    effectStore.Add(aDT);
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                }
                EffectToolStore = effectStore.ToArray();
                DrawingToolStore = drawStore.ToArray();
            }
            else
            {
                foreach (aDrawingTool3D aDT in DrawingToolStore)
                    AddDrawingTool(aDT);
                foreach (IEffect3D effect in EffectToolStore)
                    AddMenuItem(effect);
            }
        }
        private void LoadScripts()
        {
            if (ScriptStore == null)
            {
                List<string> scriptStore = new List<string>();
                List<string> AllFiles = new List<string>();
                string[] Filenames = Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath) + "\\scripts", "*.py"); // Path.GetDirectoryName(Application.ExecutablePath) + "\\ImageViewer3D.dll";
                AllFiles.AddRange(Filenames);

                ToolStripMenuItem mainMenu = (ToolStripMenuItem)FindMenuItem(mMenuStrip, "Scripts");
                if (mainMenu == null)
                {
                    mainMenu = new ToolStripMenuItem("Scripts");
                    mMenuStrip.Items.Add(mainMenu);
                }

                foreach (string s in AllFiles)
                {
                    ToolStripItem menuItem = mainMenu.DropDownItems.Add(Path.GetFileNameWithoutExtension(s));
                    menuItem.Click += new EventHandler(menuItem_Click);
                }
                ScriptStore = AllFiles.ToArray();
            }
            else
            {
                ToolStripMenuItem mainMenu = (ToolStripMenuItem)FindMenuItem(mMenuStrip, "Scripts");
                if (mainMenu == null)
                {
                    mainMenu = new ToolStripMenuItem("Scripts");
                    mMenuStrip.Items.Add(mainMenu);
                }

                foreach (string s in ScriptStore)
                {
                    ToolStripItem menuItem = mainMenu.DropDownItems.Add(Path.GetFileNameWithoutExtension(s));
                    menuItem.Click += new EventHandler(menuItem_Click);
                }
            }
        }

        #endregion

        #region ToolHandling

        public void RedrawToolSelection()
        {
            if (mLastTool != null)
            {
                mLastTool.RedrawSelection();
            }
            else if (mCurrentTool != null)
            {
                mCurrentTool.RedrawSelection();
            }
        }

        public void DoSelectionPerformed(ISelection3D Selection)
        {
            if (Selection == null)
            {
                if (mLastTool != null)
                {
                    mLastTool.EraseSelection();
                    mLastTool = null;
                }
                if (mCurrentTool != null)
                {
                    mCurrentTool.EraseSelection();
                }
            }
            if (Selection != null && SelectionPerformed != null)
                SelectionPerformed(Selection);
            mLastSelection = Selection;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // mScreenProperties.CleanseScreen();
            if (mLastTool != null)
            {
                mLastTool.EraseSelection();
                mLastTool = null;
            }
            if (mCurrentTool != null)
            {

                mCurrentTool.EraseSelection();

                mCurrentTool.MouseDown(((PictureDisplay3DSlice)sender).ScreenProperties, new Point(e.X, e.Y), e);
            }
        }
        public event MouseEventHandler MouseMoving;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //notify the users of this control of mouse interactions
            if (MouseMoving != null)
                MouseMoving(sender, e);


            if (mCurrentTool != null)
                mCurrentTool.MouseMove(new Point(e.X, e.Y), e);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mCurrentTool != null)
                mCurrentTool.MouseUp(new Point(e.X, e.Y), e);
        }

        private void ViewerControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (mCurrentTool != null)
                mCurrentTool.KeyDown(e);
        }
        private void ViewerControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (mCurrentTool != null)
                mCurrentTool.KeyUp(e);
        }
        private void ViewerControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (mCurrentTool != null)
                mCurrentTool.KeyPress(e);
        }
        #endregion

        #region Events
        void adtB_KeyUp(object sender, KeyEventArgs e)
        {
            ViewerControl_KeyUp(sender, e);
        }

        void adtB_KeyPress(object sender, KeyPressEventArgs e)
        {
            ViewerControl_KeyPress(sender, e);
        }

        void adtB_KeyDown(object sender, KeyEventArgs e)
        {
            ViewerControl_KeyDown(sender, e);
        }

        void adtB_Click(object sender, EventArgs e)
        {
            mLastTool = mCurrentTool;
            mCurrentTool = ((ToolbarButton3D)sender).DrawingTool;
            if (ToolTipUpdate != null)
                ToolTipUpdate(mCurrentTool.GetToolTip());
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (OnClick != null)
                OnClick(sender, e);
        }


        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            ViewerControl_Resize(this, e);
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            ViewerControl_Resize(this, e);
        }
        public void ViewerControl_Resize(object sender, EventArgs e)
        {
            try
            {
                if (Width > 0)
                {
                    for (int i = 0; i < Displays.Length; i++)
                    {
                        Displays[i].ScreenProperties.ScreenFrontBuffer = new Bitmap(Displays[i].Width, Displays[i].Height, PixelFormat.Format32bppRgb);
                        Displays[i].SetImage();
                        Displays[i].ScreenProperties.ScreenBackBuffer = new Bitmap(Displays[i].Width, Displays[i].Height, PixelFormat.Format32bppRgb);
                        Displays[i].ScreenProperties.RedrawBuffers();
                    }
                }
            }
            catch { }
        }

        ReplaceStringDictionary mPassData = new ReplaceStringDictionary();
        void emi_Click(object sender, EventArgs e)
        {
            IEffect3D effect = ((EffectMenuItem3D)sender).MenuEffect;
            try
            {
                effect.ShowInterface(this.Parent);
                ((aEffectForm3D)effect).Visible = true;
            }
            catch { }

            ///Leave a history momento so the effect can be removed
            // mHistoryMonitor.PushMomento(effect.EffectName, mScreens);

            mDataEnvironment.ActiveSelectedImage = effect.DoEffect(mDataEnvironment, mDataEnvironment.ActiveSelectedImage, mPassData, null);
            if (effect.PassData != null)
            {
                if (effect.PassesPassData == true)
                    mPassData = effect.PassData;
            }
            if (MacroLineGenerated != null)
                MacroLineGenerated(effect.getMacroString());
        }

        void menuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            string filename = Path.GetDirectoryName(Application.ExecutablePath) + "\\scripts\\" + tsi.Text + ".py";

            StreamReader streamReader = new StreamReader(filename);
            string text = streamReader.ReadToEnd();
            streamReader.Close();

            PythonScripting.MacroRecorder3D mr = new ImageViewer3D.PythonScripting.MacroRecorder3D();
            // mr.Show(this.Parent );

            ///Leave a history momento so the effect can be removed
            // mHistoryMonitor.PushMomento(tsi.Text, mScreens);

            object[] mMacroToken = new object[] { text };

            mr.DoEffect(mDataEnvironment, null, mPassData, mMacroToken);
        }

        private bool ProgramChangeSlice = false;
        private void crossDisplay1_XAxisMoved(double Percent)
        {
            try
            {
                if (ProgramChangeSlice == false)
                    mScreens[2].SliceIndex = (int)(Percent * (mDataEnvironment.FullSize.Width - 1));
            }
            catch { }
        }

        private void crossDisplay1_YAxisMoved(double Percent)
        {
            try
            {
                if (ProgramChangeSlice == false)
                    mScreens[0].SliceIndex = (int)(Percent * (mDataEnvironment.FullSize.Height - 1));
            }
            catch { }
        }

        private void crossDisplay1_ZAxisMoved(double Percent)
        {
            try
            {
                if (ProgramChangeSlice == false)
                    mScreens[1].SliceIndex = (int)((1 - Percent) * (mDataEnvironment.FullSize.Depth - 1));
            }
            catch { }
        }

        public int SliceIndexX
        {
            get { return mScreens[2].SliceIndex; }
            set {
                ProgramChangeSlice = true;
                mScreens[2].SliceIndex = value;
                crossDisplay1.SlicePositionX = value / (mDataEnvironment.FullSize.Width - 1);
                ProgramChangeSlice = false;
            }
        }

        public int SliceIndexY
        {
            get { return mScreens[0].SliceIndex; }
            set {
                ProgramChangeSlice = true;
                mScreens[0].SliceIndex = value;
                crossDisplay1.SlicePositionX = value / (mDataEnvironment.FullSize.Height  - 1);
                ProgramChangeSlice = false;
            }
        }

        public int SliceIndexZ
        {
            get { return mScreens[1].SliceIndex; }
            set {
                ProgramChangeSlice = true;
                mScreens[1].SliceIndex = value;
                crossDisplay1.SlicePositionX = value / (mDataEnvironment.FullSize.Depth  - 1);
                ProgramChangeSlice = false;
            }
        }



        #endregion


        #region ContrastAndBrightness

        /// <summary>
        /// Sets the image brightness and contrast
        /// </summary>
        /// <param name="Contrast">value from 0.0-1.0 percent contrast</param>
        /// <param name="Brightness">value from 0.0-1.0 percent brightness</param>
        public void SetContrastAndBrightness(double Contrast, double Brightness)
        {
            tContrast.Value = (int)(Contrast * 1000d);
            tBrightness.Value = (int)(Brightness * 1000d);

            tContrast_Scroll(this, EventArgs.Empty);
            tBrightness_Scroll(this, EventArgs.Empty);
        }

        private bool mProgramChange = false;
        private void tContrast_Scroll(object sender, EventArgs e)
        {
            if (mProgramChange == false)
            {
                double Min = mDataEnvironment.MaxPossibleContrast;
                double Max = mDataEnvironment.MinPossibleContrast;

                double Average = (1 - (double)tBrightness.Value / 1000d) * (Max - Min) + Min;
                double WholeWidth = (mDataEnvironment.MaxPossibleContrast - mDataEnvironment.MinPossibleContrast);
                mDataEnvironment.MinContrast = Average - WholeWidth * (1 - (double)tContrast.Value / 1000d);
                mDataEnvironment.MaxContrast = Average + WholeWidth * (1 - (double)tContrast.Value / 1000d);
            }
        }

        private void tBrightness_Scroll(object sender, EventArgs e)
        {
            if (mProgramChange == false)
            {
                double Min = mDataEnvironment.MaxPossibleContrast;
                double Max = mDataEnvironment.MinPossibleContrast;

                double Average = (1 - (double)tBrightness.Value / 1000d) * (Max - Min) + Min;
                double WholeWidth = (mDataEnvironment.MaxPossibleContrast - mDataEnvironment.MinPossibleContrast);
                mDataEnvironment.MinContrast = Average - WholeWidth * (1 - (double)tContrast.Value / 1000d);
                mDataEnvironment.MaxContrast = Average + WholeWidth * (1 - (double)tContrast.Value / 1000d);
            }
        }

        void mDataEnvironment_ContrastChanged(double MinContrast, double MaxContrast)
        {
            // mProgramChange = true;
            try
            {
                tContrast.Value = (int)((MaxContrast - MinContrast) / (mDataEnvironment.MaxPossibleContrast - mDataEnvironment.MinPossibleContrast) * 1000);
                tBrightness.Value = (int)(((MaxContrast + MinContrast) / 2d) / (mDataEnvironment.MaxPossibleContrast + mDataEnvironment.MinPossibleContrast) * 1000);

                tContrast_Scroll(this, EventArgs.Empty);
                tBrightness_Scroll(this, EventArgs.Empty);
            }
            catch { }
            mProgramChange = false;
        }
        #endregion


    }
}
