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

namespace ImageViewer3D.Filters
{
    public abstract partial class aEffectForm3D : Form, IEffect3D
    {
        #region Internal Vars
        public aEffectForm3D()
        {
           
                InitializeComponent();
           
        }

        protected object[] mParameters;
        protected DataEnvironment3D mDataEnvironment;
        protected DataHolder mSourceImage;
        protected Bitmap  mScratchImage;

        #endregion

        #region Sliders
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

        void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            object[] pars = new object[Sliders.Count];
            int i = 0;
            foreach (KeyValuePair<string, ParamSlider> kvp in Sliders)
            {
                try
                {
                    mParameters[i] = kvp.Value.trackBar.Value;
                }
                catch
                {
                    for (int j = 0; j < mParameters.Length; j++)
                        pars[j] = mParameters[j];
                    mParameters = pars;
                    mParameters[i] = kvp.Value.trackBar.Value;
                }
                i++;

            }
            DoRun();
        }

        protected Dictionary<string, ParamSlider> Sliders = new Dictionary<string, ParamSlider>();
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
        public abstract string EffectName { get; }
        public abstract string EffectMenu { get; }
        public abstract string EffectSubMenu { get; }
        public virtual int OrderSuggestion
        {
            get { return 1; }
        }
        #endregion


        public abstract object[] DefaultProperties { get; }
        public abstract string[] ParameterList { get; }
        protected abstract DataHolder doEffect(
            DataEnvironment3D dataEnvironment, DataHolder SourceImage,
            ImageViewer.Filters.ReplaceStringDictionary PassData,
            params object[] Parameters);

        public virtual DataHolder DoEffect(
            DataEnvironment3D dataEnvironment, DataHolder SourceImage,
            ImageViewer.Filters.ReplaceStringDictionary PassData,
            params object[] Parameters)
        {
            //set up all the persistant properties for the UI to work
            mDataEnvironment = dataEnvironment;
            mParameters = Parameters;
            if (mParameters == null)
                mParameters = DefaultProperties;
            mPassData = PassData;
            mSourceImage = SourceImage;
           
            //pause if the UI is displayed
            if (this.Visible == true)
            {
                //show the effect on the screen
                DoRun();

                while (this.Visible == true)
                    Application.DoEvents();
            }

            //perform the actual processing and then head out
            return doEffect(dataEnvironment, SourceImage, PassData, mParameters);
           
        }

        #region PassData
        public virtual bool PassesPassData
        { get { return false; } }
        public virtual string PassDataDescription
        {
            get { return ""; }
        }
        protected ImageViewer.Filters.ReplaceStringDictionary mPassData;
        public ImageViewer.Filters.ReplaceStringDictionary PassData
        {
            get { return mPassData; }
            set { mPassData = value; }
        }
        #endregion

        public virtual string getMacroString()
        {
            return EffectHelps3D.FormatMacroString(this, mParameters );
        }


        #region Visuals
        public virtual void ShowInterface(IWin32Window Owner)
        {
            this.Show(Owner);
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
            doEffect(mDataEnvironment, mSourceImage, mPassData, mParameters);
        }
        #endregion


    }
}
