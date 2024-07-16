using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

using ZedGraph;
using System.IO;
using ImageViewer.Filters;
using ImageViewer;
using MathHelpLib.ImageProcessing;

namespace ImageViewer3D.Filters.Files
{
    public class FlyThroughTool : aEffectForm3D
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
            int EndIndex = (int)(EffectHelps.ConvertToDouble(mParameters[1]) / 100d * mSourceImage.Data.GetLength(0));
            int StartIndex = (int)(EffectHelps.ConvertToDouble(mParameters[0]) / 100d * mSourceImage.Data.GetLength(0));
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
                    float [,] dt= mSourceImage.Data.SliceZAxis(currentIndex);
                    Bitmap bmp =dt.MakeBitmap(MinContrast, MaxContrast);
                    string frameName = tempDir + "Image" + string.Format("{0:0000}", currentIndex) + ".bmp";
                    FrameNames.Add(frameName);
                    bmp.Save(frameName);
                    pictureDisplay1.Image = bmp;
                    pictureDisplay1.Invalidate();
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
            timer.Enabled = false;
            Bitmap holding = mSourceImage.Data.SliceZAxis(currentIndex).MakeBitmap(MinContrast, MaxContrast);
            currentIndex++;
            if (currentIndex >= (EffectHelps.ConvertToDouble(mParameters[1]) / 100d * mSourceImage.Data.GetLength(0)))
                currentIndex = (int)(EffectHelps.ConvertToDouble(mParameters[0]) / 100d * mSourceImage.Data.GetLength(0));
            pictureDisplay1.Image = holding;
            pictureDisplay1.Invalidate();
            Application.DoEvents();
            timer.Enabled = true;
        }
        
        float MaxContrast = 0;
        float MinContrast = 1000;

        public override string EffectName { get { return "3D Fly Through"; } }
        public override string EffectMenu { get { return "File"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 1; } }

        //public override IEffectToken DefaultToken { get { return CreateToken(0, 100); } }
        public override object[] DefaultProperties
        {
            get { return new object[]{0,100}; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "FirstSlice(Percent)|int", "LastSlice(Percent)|int" }; }
        }

        protected override ImageViewer3D.DataHolder doEffect(ImageViewer3D.DataEnvironment3D dataEnvironment, ImageViewer3D.DataHolder SourceImage, ImageViewer.Filters.ReplaceStringDictionary PassData, params object[] Parameters)
        {
            
            mParameters =Parameters ;
            if (mParameters == null)
                mParameters = DefaultProperties;

            timer.Enabled = true;

            mSourceImage = SourceImage;

            MaxContrast = (float)dataEnvironment.MaxContrast;
            MinContrast =(float) dataEnvironment.MinContrast;

            currentIndex = (int)(EffectHelps.ConvertToDouble(mParameters[0]) / 100d * mSourceImage.Data.GetLength(0));

            return SourceImage;
        }

        int currentIndex;
    }
}
