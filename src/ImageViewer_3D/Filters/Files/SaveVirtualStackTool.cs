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


namespace ImageViewer3D.Filters.Files
{
    public class SaveVirtualStackTool : aEffectForm3D
    {
        Timer timer;
        public SaveVirtualStackTool()
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
            button2.Text = "Save Tiff";
            button2.UseVisualStyleBackColor = true;
        }

        void button2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            int EndIndex = (int)(EffectHelps.ConvertToDouble(mParameters[1]) / 100d * mSourceImage.Data.GetLength(0));
            int StartIndex = (int)(EffectHelps.ConvertToDouble(mParameters[0]) / 100d * mSourceImage.Data.GetLength(0));


            SaveFileDialog sfd = new SaveFileDialog();
            DialogResult ret = sfd.ShowDialog();

            string PathOut = Path.GetDirectoryName(sfd.FileName);
            string FileOut = Path.GetFileNameWithoutExtension(sfd.FileName);
            string Exten = Path.GetExtension(sfd.FileName);

            if (ret == DialogResult.OK)
            {
                if (Exten == ".tiff" || Exten == ".tif")
                {
                    SaveTIFF(sfd.FileName, StartIndex, EndIndex);
                }
                else
                {
                    for (currentIndex = StartIndex; currentIndex < EndIndex; currentIndex++)
                    {
                        Bitmap bmp = mSourceImage.Data .SliceZAxis(currentIndex).MakeBitmap(MinContrast, MaxContrast);
                        string frameName = PathOut + "\\" + FileOut + string.Format("{0:0000}", currentIndex) + Exten;

                        bmp.Save(frameName);
                        pictureDisplay1.Image = bmp;
                        pictureDisplay1.Invalidate();
                    }
                }
                currentIndex = 0;
            }
            MessageBox.Show("File Generation is complete", "");

            timer.Enabled = true;
        }

        private void SaveTIFF(string FileOut, int StartIndex, int EndIndex)
        {
            //get the codec for tiff files
            ImageCodecInfo info = null;
            foreach (ImageCodecInfo ice in ImageCodecInfo.GetImageEncoders())
                if (ice.MimeType == "image/tiff")
                    info = ice;

            //use the save encoder
            System.Drawing.Imaging.Encoder enc = System.Drawing.Imaging.Encoder.SaveFlag;
            EncoderParameters ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.MultiFrame);

            Bitmap pages = null;
            int frame = 0;
            for (currentIndex = StartIndex; currentIndex < EndIndex; currentIndex++)
            {
                if (currentIndex == StartIndex)
                {
                    pages = mSourceImage.Data.SliceZAxis(currentIndex).MakeBitmap(MinContrast, MaxContrast);
                    //save the first frame
                    pages.Save(FileOut, info, ep);
                }
                else
                {
                    //save the intermediate frames
                    ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.FrameDimensionPage);
                    Bitmap bm = mSourceImage.Data.SliceZAxis(currentIndex).MakeBitmap(MinContrast, MaxContrast);
                    pages.SaveAdd(bm, ep);
                }

                if (currentIndex == EndIndex - 1)
                {
                    //flush and close.
                    ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.Flush);
                    pages.SaveAdd(ep);
                }
                frame++;
            }
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
        
        double MaxContrast = 0;
        double MinContrast = 1000;

        public override string EffectName { get { return "Save Virtual Stack"; } }
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

            MaxContrast = dataEnvironment.MaxContrast;
            MinContrast = dataEnvironment.MinContrast;

            currentIndex = (int)(EffectHelps.ConvertToDouble(mParameters[0]) / 100d * mSourceImage.Data.GetLength(0));

            return SourceImage;
        }

        int currentIndex;
    }
}
