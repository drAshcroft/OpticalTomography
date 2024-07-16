using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using MathHelpLib;

namespace ImageViewer.Filters
{
    public abstract partial class aEffectForm : Form, IEffect
    {
        #region Internal Vars
        public aEffectForm()
        {

            InitializeComponent();

        }

        /// <summary>
        /// contains the parameters sent over from python.  this would be nicer, but we do not know what python will send accross
        /// </summary>
        protected object[] mFilterToken;

        /// <summary>
        /// Contains a copy of the global data environment.  
        /// </summary>
        protected DataEnvironment mDataEnvironment;

        /// <summary>
        /// This is used to avoid calculting an image again when the done button is clicked.
        /// </summary>
        protected ImageHolder mScratchImage;

        #endregion

        #region Sliders

        /// <summary>
        /// there can be a number of sliders that are automatically handled for the filters.  This contains a description of these
        /// </summary>
        protected class ParamSlider
        {
            public TrackBar trackBar;
            public Label label;
            public string ParamName;

            public ParamSlider(string paramName, TrackBar trackBar, Label label)
            {
                this.trackBar = trackBar;
                this.label = label;
                this.ParamName = paramName;
                trackBar.ValueChanged += new EventHandler(trackBar_ValueChanged);
            }

            void trackBar_ValueChanged(object sender, EventArgs e)
            {
                label.Text = ParamName + " : " + trackBar.Value.ToString();
            }

            public void ShowValue(int newValue)
            {
                trackBar.Value = newValue;
                label.Text = ParamName + " : " + newValue.ToString();
            }

            public void Redraw()
            {
                trackBar.Invalidate();
                trackBar.Refresh();
            }
        }

        int currentY = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ParamName">Name that will be shown on the screen</param>
        /// <param name="Max">Maximum value of the slider</param>
        /// <param name="Min">Minimum value of the slider</param>
        /// <returns></returns>
        private ParamSlider CreateLabelAndSlider(string ParamName, int Min, int Max)
        {
            System.Windows.Forms.Label label1 = new Label();
            TrackBar trackBar1 = new TrackBar();

            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(trackBar1);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = System.Drawing.Color.White;

            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(35, 13);
            label1.TabIndex = 27;
            label1.Text = ParamName;


            ///////private code to make it work
            label1.Location = new System.Drawing.Point(0, currentY);
            currentY += label1.Height;


            trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                         | System.Windows.Forms.AnchorStyles.Right)));
            trackBar1.Location = new System.Drawing.Point(6, 81);
            trackBar1.Minimum = Min;
            trackBar1.Maximum = Max;
            trackBar1.Name = ParamName;
            trackBar1.Size = new System.Drawing.Size(158, 45);
            trackBar1.TabIndex = 27;


            ////Private code to make it work
            trackBar1.Location = new System.Drawing.Point(0, currentY);
            trackBar1.ValueChanged += new EventHandler(trackBar1_ValueChanged);
            trackBar1.Width = splitContainer1.Panel2.Width - 30;

            currentY += trackBar1.Height + 5;

            label1.Visible = true;
            trackBar1.Visible = true;

            label1.BringToFront();
            trackBar1.BringToFront();

            return new ParamSlider(ParamName, trackBar1, label1);
        }

        /// <summary>
        /// Determines which slider sent the value and updates the correct filtertoken
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            object[] pars = new object[Sliders.Count];
            int i = 0;
            foreach (KeyValuePair<string, ParamSlider> kvp in Sliders)
            {
                try
                {
                    mFilterToken[i] = kvp.Value.trackBar.Value;
                }
                catch
                {
                    for (int j = 0; j < mFilterToken.Length; j++)
                        pars[j] = mFilterToken[j];
                    mFilterToken = pars;
                    mFilterToken[i] = kvp.Value.trackBar.Value;
                }
                i++;

            }
            DoRun();
        }

        /// <summary>
        /// A dictionary with all the parameter sliders.  This should not be accessed, but values should be taken from the mFilterToken array
        /// </summary>
        protected Dictionary<string, ParamSlider> Sliders = new Dictionary<string, ParamSlider>();
        /// <summary>
        /// Creates the slider array
        /// </summary>
        /// <param name="Names"></param>
        /// <param name="Mins"></param>
        /// <param name="Maxs"></param>
        protected void SetParameters(string[] Names, int[] Mins, int[] Maxs)
        {
            splitContainer1.Panel2Collapsed = false;
            pictureDisplay1.Width = this.Width - 250;
            for (int i = 0; i < Names.Length; i++)
            {
                Sliders.Add(Names[i],
                    CreateLabelAndSlider(Names[i], Mins[i], Maxs[i]));
            }
        }

        #endregion



        #region MenuStuff

        /// <summary>
        /// Name displayed on the menu
        /// </summary>
        public abstract string EffectName { get; }

        /// <summary>
        /// Menu that this effect is classified under
        /// </summary>
        public abstract string EffectMenu { get; }

        /// <summary>
        /// Sub menu for this effect
        /// </summary>
        public abstract string EffectSubMenu { get; }

        /// <summary>
        /// Suggestion of location of this effect on the submenu
        /// </summary>
        public virtual int OrderSuggestion
        {
            get { return 1; }
        }

        #endregion

        /// <summary>
        /// An object array that holds all the default properties for the given effect, in the order desired.
        /// </summary>
        public abstract object[] DefaultProperties { get; }

        /// <summary>
        /// Names of the properties.  Only the default list needs to be listed
        /// </summary>
        public abstract string[] ParameterList { get; }


        /// <summary>
        /// The needed effect.  It should always return an imageholder
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        protected abstract object doEffect(
            DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData,
            params object[] Parameters);

        /// <summary>
        /// Interacts with the menu system and with python to pull calls and format them to the needed types
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public virtual object DoEffect(
            DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData,
            params object[] Parameters)
        {

            //set up all the persistant properties for the UI to work
            mDataEnvironment = dataEnvironment;

            mFilterToken = Parameters;

            if (mFilterToken == null)
                mFilterToken = DefaultProperties;

            mPassData = PassData;

            pictureDisplay1.dataEnvironment = mDataEnvironment;

            if (SourceImage.GetType() == typeof(Bitmap))
            {
                mScratchImage = new ImageHolder((Bitmap)SourceImage);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                mScratchImage = ((ImageHolder)SourceImage).Copy();
            }

            //pause if the UI is displayed
            if (this.Visible == true)
            {
                //show the effect on the screen
                DoRun();

                while (this.Visible == true)
                    Application.DoEvents();
            }

            //perform the actual processing and then head out
            return doEffect(dataEnvironment, SourceImage, PassData, mFilterToken);

        }

        #region PassData
        public virtual bool PassesPassData
        { get { return false; } }
        public virtual string PassDataDescription
        {
            get { return ""; }
        }
        protected ReplaceStringDictionary mPassData;
        public ReplaceStringDictionary PassData
        {
            get { return mPassData; }
            set { mPassData = value; }
        }
        #endregion

        public virtual string getMacroString()
        {
            return EffectHelps.FormatMacroString(this, mFilterToken);
        }


        #region Visuals
        public virtual void ShowInterface(IWin32Window Owner)
        {
            this.Show(Owner);
            this.Visible = true;
            Application.DoEvents();
            foreach (ParamSlider ms in Sliders.Values)
            {
                ms.Redraw();
            }
        }

        protected virtual void button1_Click(object sender, EventArgs e)
        {

            this.Hide();
        }

        protected virtual void DoRun()
        {
            pictureDisplay1.SetImage(doEffect(mDataEnvironment, mScratchImage, mPassData, mFilterToken));
            pictureDisplay1.Invalidate();
        }
        #endregion


    }
}
