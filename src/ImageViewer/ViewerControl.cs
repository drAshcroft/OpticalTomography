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
using ImageViewer.Tools;
using System.Reflection;
using System.IO;
using ImageViewer.Filters;
using MathHelpLib;


namespace ImageViewer
{
    public delegate void MacroLineGeneratedEvent(string MacroLine);
    public delegate void ToolTipUpdateEvent(string ToolTip);
    public delegate void SelectionPerformedEvent(ISelection Selection);
    public partial class ViewerControl : UserControl
    {
        #region Events
        public event SelectionPerformedEvent SelectionPerformed;
        public event ToolTipUpdateEvent ToolTipUpdate;
        public new event EventHandler OnClick;
        #endregion

        #region InternalVars

        private aDrawingTool mCurrentTool;
        private aDrawingTool mLastTool;
        //private ScreenProperties[] mScreenProperties;
        private ISelection mLastSelection;

        private MenuStrip mMenuStrip;


        private PictureDisplay Displays;
        private ScreenProperties mScreens;
        private DataEnvironment mDataEnvironment;

        private UserControl mExtraControl;

        static aDrawingTool[] DrawingToolStore;
        static IEffect[] EffectToolStore;
        static string[] ScriptStore;

        HistoryMonitor mHistoryMonitor = new HistoryMonitor();

        #endregion

        #region public_Properties
        internal ScreenProperties ScreenProperties
        {
            get
            {
                return mScreens;
            }
        }

        public MenuStrip MyMenu
        {
            get { return mMenuStrip; }
        }

        public aDrawingTool ActiveDrawingTool
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

        public Bitmap ActiveImage
        {
            get { return Displays.ScreenProperties.OriginalImage.ToBitmap() ; }
        }

        public ISelection SelectedArea
        {
            get { return mLastSelection; }
            set
            {
                mScreens.ActiveSelection = value;
            }
        }

        public void AddDrawingTool(aDrawingTool addTool)
        {
            ToolbarButton adtB = new ToolbarButton(addTool);
            adtB.Dock = DockStyle.Top;
            adtB.Text = addTool.GetToolName();
            adtB.Click += new EventHandler(adtB_Click);
            adtB.KeyDown += new KeyEventHandler(adtB_KeyDown);
            adtB.KeyPress += new KeyPressEventHandler(adtB_KeyPress);
            adtB.KeyUp += new KeyEventHandler(adtB_KeyUp);
            panel2.Controls.Add(adtB);

            if (addTool.GetType() == typeof(ROITool))
                mCurrentTool = addTool;
        }

        public void AddMenuItem(IEffect effect)
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


            EffectMenuItem emi = (EffectMenuItem)FindMenuItem(subMenu, effect.EffectName);

            if (emi == null)
                emi = new EffectMenuItem(effect, effect.EffectName);
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
                        if (subMenu.DropDownItems[i].GetType() == typeof(EffectMenuItem))
                        {
                            if (effect.OrderSuggestion <= ((EffectMenuItem)subMenu.DropDownItems[i]).MenuEffect.OrderSuggestion)
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
                if (Displays != null && Displays.ScreenProperties != null)
                    return Displays.ScreenProperties.ProportionalZoom;
                else
                    return true;
            }
            set
            {
                try
                {
                    Displays.ScreenProperties.ProportionalZoom = value;
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
                if (Displays != null && Displays.ScreenProperties != null)
                    return Displays.ScreenProperties.InterpolationMode;
                else
                    return InterpolationMode.Bilinear;
            }
            set
            {
                try
                {
                    Displays.ScreenProperties.InterpolationMode = value;
                }
                catch { }
            }
        }

        public void ShowFileScript(string FilePath )
        {
            PythonScripting.MacroRecorder mr = new ImageViewer.PythonScripting.MacroRecorder();

            object[] mMacroToken = new object[] { FilePath, true  };

            mr.DoEffect(mDataEnvironment, null, null, mMacroToken);
        }

        public void ShowTextScript(string Script)
        {
            PythonScripting.MacroRecorder mr = new ImageViewer.PythonScripting.MacroRecorder();

            object[] mMacroToken = new object[] { Script , false  };

            mr.DoEffect(mDataEnvironment, null, null, mMacroToken);
        }

        private delegate void SetImageDelegate(Bitmap DisplayImage);
        public void SetImage(Bitmap DisplayImage)
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
        private void SetUpImage(Bitmap DisplayImage)
        {
            if (DisplayImage != null)
            {
                mScreens.ActiveSelection = null;
                mScreens.ActiveSelectedImage = new ImageHolder(DisplayImage);
             
            }
        }
        private delegate void SetImageDelegateIH(ImageHolder DisplayImage);
        public void SetImage(ImageHolder DisplayImage)
        {
            if (InvokeRequired)
            {
                this.Invoke(new SetImageDelegateIH(SetImage), DisplayImage);
            }
            else
            {
                SetUpImage(DisplayImage);

            }
        }
        private void SetUpImage(ImageHolder DisplayImage)
        {
            if (DisplayImage != null)
            {
                mScreens.ActiveSelection = null;
                mScreens.ActiveSelectedImage =DisplayImage;

            }
        }

        public event MacroLineGeneratedEvent MacroLineGenerated;
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
        public ViewerControl()
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
            /*Displays = new PictureDisplay();
            panel1.Controls.Add(Displays);
            Displays.Visible = true;
            Displays.BringToFront();*/
            Displays = pictureDisplay1;
            mDataEnvironment = new DataEnvironment();

            ScreenProperties mScreenProperties = new ScreenProperties(Displays, this);
            mScreenProperties.ScreenFrontBuffer = new Bitmap (Displays.Width, Displays.Height,PixelFormat.Format32bppRgb );
            
            try
            {
                Displays.SetImage(mScreenProperties.ScreenFrontBuffer);
            }
            catch { }
            Displays.ScreenProperties = mScreenProperties;
            Displays.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            Displays.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);
            Displays.MouseUp += new MouseEventHandler(pictureBox1_MouseUp);
            mScreens = mScreenProperties;

           
            mDataEnvironment.Screen = mScreens;
            Displays.dataEnvironment = mDataEnvironment;

            mMenuStrip = new MenuStrip();
            string[] Menus = Filters.MenuStructure.MainMenuStructure();
            for (int i = 0; i < Menus.Length; i++)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(Menus[i]);
                mMenuStrip.Items.Add(tsmi);
                string[] SubMenus = Filters.MenuStructure.SubMenuStructure(Menus[i]);
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
            mHistoryMonitor.StepBackMomento(mScreens);
        }
        void Redo_Click(object sender, EventArgs e)
        {
            mHistoryMonitor.StepForwardMomento(mScreens);
        }

        private void LoadTools()
        {
            if (DrawingToolStore == null)
            {
                List<aDrawingTool> drawStore = new List<aDrawingTool>();
                List<string> AllFiles = new List<string>();
                List<IEffect> effectStore = new List<IEffect>();
                string[] Filenames = Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath), "plug*.dll"); // Path.GetDirectoryName(Application.ExecutablePath) + "\\ImageViewer.dll";
                AllFiles.AddRange(Filenames);
                AllFiles.Add(Path.GetDirectoryName(Application.ExecutablePath) + "\\ImageViewer.dll");
                Filenames = AllFiles.ToArray();
                for (int i = 0; i < Filenames.Length; i++)
                {
                    try
                    {
                        Assembly asm = Assembly.LoadFrom(Filenames[i]);
                        Type[] alltypes = asm.GetTypes();
                        foreach (Type temp in alltypes)
                        {
                            if (temp != typeof(ViewerControl) && temp.IsAbstract == false)
                            {
                                try
                                {
                                    ConstructorInfo[] ci = temp.GetConstructors();
                                    aDrawingTool aDT = (aDrawingTool)ci[0].Invoke(null);

                                    AddDrawingTool(aDT);
                                    drawStore.Add(aDT);
                                }
                                catch { }
                            }

                            if (temp.GetInterface("IEffect") != null)
                            {
                                try
                                {
                                    ConstructorInfo[] ci = temp.GetConstructors();
                                    IEffect aDT = (IEffect)ci[0].Invoke(null);

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
                foreach (aDrawingTool aDT in DrawingToolStore)
                    AddDrawingTool(aDT);
                foreach (IEffect effect in EffectToolStore)
                    AddMenuItem(effect);
            }
        }
        private void LoadScripts()
        {
            if (ScriptStore == null)
            {
                List<string> scriptStore = new List<string>();
                List<string> AllFiles = new List<string>();
                string[] Filenames = Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath) + "\\scripts", "*.py"); // Path.GetDirectoryName(Application.ExecutablePath) + "\\ImageViewer.dll";
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

        public void DoSelectionPerformed(ISelection Selection)
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
                mCurrentTool.MouseDown(((PictureDisplay)sender).ScreenProperties, new Point(e.X, e.Y), e);
            }
        }
        public event MouseEventHandler MouseMoving;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
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
            mCurrentTool = ((ToolbarButton)sender).DrawingTool;
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
                    Displays.ScreenProperties.ScreenFrontBuffer = new Bitmap(Displays.Width, Displays.Height, PixelFormat.Format32bppRgb);
                    Displays.SetImage(Displays.ScreenProperties.ScreenFrontBuffer);
                    Displays.ScreenProperties.ScreenBackBuffer = new Bitmap (Displays.Width, Displays.Height, PixelFormat.Format32bppRgb );
                    Displays.ScreenProperties.RedrawBuffers();
                }
            }
            catch { }
        }

        ReplaceStringDictionary mPassData = new ReplaceStringDictionary();
        void emi_Click(object sender, EventArgs e)
        {
            IEffect effect = ((EffectMenuItem)sender).MenuEffect;
            try
            {
                effect.ShowInterface(this.Parent);
            }
            catch { }

            ///Leave a history momento so the effect can be removed
            mHistoryMonitor.PushMomento(effect.EffectName, mScreens);
            if (mScreens.ActiveSelectedImage != null)
            {
                mScreens.ActiveSelectedImage = effect.DoEffect(mDataEnvironment, mScreens.ActiveSelectedImage, mPassData, null);
                if (effect.PassData != null)
                {
                    if (effect.PassesPassData == true)
                        mPassData = effect.PassData;
                }

                System.Diagnostics.Debug.Print(effect.getMacroString());
                if (MacroLineGenerated != null)
                    MacroLineGenerated(effect.getMacroString());
            }
        }

        void menuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            string filename = Path.GetDirectoryName(Application.ExecutablePath) + "\\scripts\\" + tsi.Text + ".py";

            StreamReader streamReader = new StreamReader(filename);
            string text = streamReader.ReadToEnd();
            streamReader.Close();

            PythonScripting.MacroRecorder mr = new ImageViewer.PythonScripting.MacroRecorder();
            // mr.Show(this.Parent );

            ///Leave a history momento so the effect can be removed
            mHistoryMonitor.PushMomento(tsi.Text, mScreens);

            object[] mMacroToken = new object[] { text };

            mr.DoEffect(mDataEnvironment,null,mPassData, mMacroToken);
        }



        #endregion
    }
}
