using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using MathHelpLib;
using ZedGraph;
using System.IO;

namespace ImageViewer.Filters
{
    public class FlyThroughTool : aEffectForm
    {
        Timer timer;
        public FlyThroughTool()
            : base()
        {
            SetParameters(new string[] { "First Slice", "Last Slice" }, new int[] { 0, 0 }, new int[] { 100, 100 });

            timer = new Timer();
            timer.Enabled = false;
            timer.Interval = 33;
            timer.Tick += new EventHandler(timer_Tick);

            Button button2 = new Button();
            button2.Click += new EventHandler(button2_Click);

            splitContainer1.Panel2.Controls.Add(button2);

            button2.Left = 0;
            button2.Top = (int)((double)splitContainer1.Panel2.Height / 2d);
            button2.Visible = true;
            button2.BringToFront();
            button2.Size = new System.Drawing.Size(67, 55);
            button2.TabIndex = 25;
            button2.Text = "Save Video";
            button2.UseVisualStyleBackColor = true;
        }

        void button2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            int EndIndex = (int)(EffectHelps.ConvertToDouble(mFilterToken.Parameters[1]) / 100d * mData.GetLength(0));
            int StartIndex = (int)(EffectHelps.ConvertToDouble(mFilterToken.Parameters[0]) / 100d * mData.GetLength(0));
            string tempDir = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\FlyThrough\\";

            if (Directory.Exists(tempDir) == false)
                Directory.CreateDirectory(tempDir);
            else
            {
                string[] Files = Directory.GetFiles(tempDir);
                foreach (string s in Files)
                    File.Delete(s);
            }

            SaveFileDialog sfd = new SaveFileDialog();
            DialogResult ret = sfd.ShowDialog();
            if (ret == DialogResult.OK)
            {
                List<string> FrameNames = new List<string>();
                for (currentIndex = StartIndex; currentIndex < EndIndex; currentIndex++)
                {
                    Bitmap bmp = mData.SliceZAxis(currentIndex).MakeBitmap(MinContrast, MaxContrast);
                    string frameName = tempDir + "Image" + string.Format("{0:0000}", currentIndex) + ".bmp";
                    FrameNames.Add(frameName);
                    bmp.Save(frameName);
                    pictureBox1.Image = bmp;
                    pictureBox1.Invalidate();
                }

                currentIndex = 0;


                ImagingTools.CreateAVIVideo(sfd.FileName, FrameNames.ToArray());
            }
            MessageBox.Show("File Generation is complete", "");
            string[] FilesD = Directory.GetFiles(tempDir);
            foreach (string s in FilesD)
                File.Delete(s);
            timer.Enabled = true;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            DoRun();
        }
        public event RefreshDataStoreEvent RefreshDataStore;


        double[, ,] mData;
        double[,] mSlice;
        double MaxValue = 0;
        double MinValue = 0;
        double MaxContrast = 0;
        double MinContrast = 1000;

        public override string EffectName { get { return "3D Fly Through"; } }
        public override string EffectMenu { get { return "3D Effects"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 1; } }

        public override IEffectToken DefaultToken { get { return CreateToken(0, 100); } }

        public override string RunEffect(ScreenProperties[] SourceImage, IEffectToken FilterToken)
        {
            mFilterToken = FilterToken;
            if (mFilterToken == null)
                mFilterToken = DefaultToken;

            timer.Enabled = true;

            double minContrast = 0, maxContrast = 1;

            if (RefreshDataStore != null)
                RefreshDataStore(out mData, out mSlice, out  minContrast, out maxContrast);

            mSourceImages = SourceImage;

            MaxValue = mData.MaxArray();
            MinValue = mData.MinArray();

            MaxContrast = maxContrast * (MaxValue - MinValue) + MinValue;
            MinContrast = minContrast * (MaxValue - MinValue) + MinValue;

            currentIndex = (int)(EffectHelps.ConvertToDouble(mFilterToken.Parameters[0]) / 100d * mData.GetLength(0));

            string outString = base.RunEffect(SourceImage, FilterToken); ;

            return outString;
        }

        int currentIndex;
        protected override Bitmap doEffect(Bitmap SourceImage, IEffectToken FilterToken)
        {
            Bitmap holding = mData.SliceZAxis(currentIndex).MakeBitmap(MinContrast, MaxContrast);
            currentIndex++;
            if (currentIndex >= (EffectHelps.ConvertToDouble(FilterToken.Parameters[1]) / 100d * mData.GetLength(0)))
                currentIndex = (int)(EffectHelps.ConvertToDouble(FilterToken.Parameters[0]) / 100d * mData.GetLength(0));
            return EffectHelps.FixImageFormat(holding);
        }
        public override IEffectToken CreateToken(params object[] TokenValues)
        {
            IEffectToken ContrastToken = new GeneralToken();
            ContrastToken.Parameters = new object[2];
            ContrastToken.Parameters[0] = EffectHelps.ConvertToDouble(TokenValues[0]);
            ContrastToken.Parameters[1] = EffectHelps.ConvertToDouble(TokenValues[1]);
            return ContrastToken;
        }
    }
}
