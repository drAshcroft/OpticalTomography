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
using System.IO;

namespace ImageViewer.Filters.Adjustments
{
    public partial class BatchRotateImageTool : aEffectForm
    {
        Timer timer;
        public BatchRotateImageTool()
            : base()
        {
            SetParameters(new string[] { "Scale", "Base Rotation" }, new int[] { 0, 0 }, new int[] { 300, 360 });
            timer = new Timer();
            timer.Interval = 30;
            timer.Tick += new EventHandler(timer_Tick);
        }
        int ImageIndex = 0;
        void timer_Tick(object sender, EventArgs e)
        {
            DisplayImage();
            ImageIndex++;
            Application.DoEvents();
        }

        private Bitmap  DisplayImage()
        {
            if (ImageNames != null)
            {
                Bitmap holding = null;
                ImageIndex = ImageIndex % ImageNames.Length;
                holding = ImagingTools.RotateImage(new Bitmap(ImageNames[ImageIndex]), (float)(Angles[ImageIndex] - 180));

                int centerX = holding.Width / 2;
                int centerY = holding.Height / 2;
                int hWidth = 70;
                int hHeight = 70;
                holding = ImagingTools.ClipImage(holding, new Rectangle(centerX - hWidth, centerY - hHeight, hWidth * 2, hHeight * 2));
                pictureBox1.Image = holding;
                pictureBox1.Invalidate();
                return holding;
            }
            else
                return new Bitmap(10, 10);
        }
        public override string EffectName { get { return "Batch Rotate Image"; } }
        public override string EffectMenu { get { return "Adjustment"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 30; } }

        public override object[] DefaultProperties
        {
            get { return new object[] { 100,0 }; }
        }

        public override string ParameterList
        {
            get { return new string[] { "Circle Radius|int","Angle:double" }; }
        }

        public override string RunEffect(ScreenProperties SourceImage, IEffectToken FilterToken)
        {
            mSourceImages = SourceImage;
            mFilterToken = FilterToken;
            if (mFilterToken == null)
                mFilterToken = DefaultToken;

            mScratchImage = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format32bppArgb);

            Bitmap ShowImage = SourceImage.ActiveSelectedImage;
            if (ShowImage != null)
            {
                Graphics.FromImage(mScratchImage).DrawImage(ShowImage,
                    new Rectangle(0, 0, mScratchImage.Width, mScratchImage.Height),
                    new Rectangle(0, 0, ShowImage.Width, ShowImage.Height), GraphicsUnit.Pixel);
            }

            SetupAngles();

            DoRun();
            timer.Enabled = true; 
            while (this.Visible == true)
                Application.DoEvents();

            return getMacroString();
        }

        double[] BaseAngles;
        double[] Angles;
        string[] ImageNames;
        string PathOut;
        string Fileout;
        string exten;
        private void SetupAngles()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            // ofd.ShowDialog();
            // string[] ImageNames =  ofd.FileNames;
            ImageNames = Directory.GetFiles(@"C:\Development\CellCT\Beads\Corrected - Copy\");

            // ofd.ShowDialog();
            // string AngleNames = ofd.FileName;
            string AngleNames = @"C:\Development\CellCT\Beads\angles.txt";
            double[]  Angles = new double[ImageNames.Length];
            string line;

            System.IO.StreamReader file =
                   new System.IO.StreamReader(AngleNames);
            try
            {
                // Read the file and display it line by line.
                
                int cc = 0;
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Trim() != "")
                    {
                        double.TryParse(line, out Angles[cc]);
                        cc++;
                    }
                }
            }
            catch { }
            finally { file.Close(); }

            //remove the jerkers
            for (int i = 1; i < Angles.Length - 1; i++)
            {
                if (Math.Abs(Angles[i] - Angles[i - 1]) > 7 && Math.Abs(Angles[i] - Angles[i + 1]) > 7)
                    Angles[i] = (Angles[i - 1] + Angles[i + 1]) / 2;
            }

            //smooth the data
            double[] AnglesOut = new double[Angles.Length];
            for (int i = 0; i < 3; i++)
                AnglesOut[i] = Angles[i];
            for (int i = 3; i < Angles.Length - 3; i++)
            {
                for (int j = i - 2; j < i + 3; j++)
                    AnglesOut[i] += Angles[j];
                AnglesOut[i] /= 5;
            }
            for (int i = Angles.Length - 3; i < Angles.Length; i++)
                AnglesOut[i] = Angles[i];

            Angles = AnglesOut;

            SaveFileDialog sfd = new SaveFileDialog();
            // sfd.ShowDialog();
            // string Filename = sfd.FileName;
            string Filename = @"C:\Development\CellCT\Beads\Corrected\pic.bmp";

            PathOut = Path.GetDirectoryName(Filename);
            Fileout = Path.GetFileNameWithoutExtension(Filename);
            exten = Path.GetExtension(Filename);

            BaseAngles = Angles;
        }

        protected override Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> doEffect(
        DataEnvironment dataEnvironment, Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> SourceImage,
        Dictionary<string, object> PassData,
        params object[] Parameters)
        {
            if (FilterToken == null)
                FilterToken = DefaultToken;

            double Scale = EffectHelps.ConvertToDouble(FilterToken.Parameters[0]) / 100d;
            double AngleAdd = EffectHelps.ConvertToDouble(FilterToken.Parameters[1]);


            Angles = new double[BaseAngles.Length];
            double AveAngle = 0;
            for (int i = 0; i < Angles.Length; i++)
            {
                AveAngle += BaseAngles[i];
            }
            AveAngle /= Angles.Length;
            for (int i = 0; i < Angles.Length; i++)
            {
                Angles[i] = (BaseAngles[i] - AveAngle) * Scale + AveAngle + AngleAdd;
            }

            Bitmap holding = null;
            holding = DisplayImage();
            ImageIndex++;

            return EffectHelps.FixImageFormat(holding);
        }
        public override IEffectToken CreateToken(params object[] TokenValues)
        {
            mFilterToken = new GeneralToken();
            mFilterToken.Parameters = new object[2];
            mFilterToken.Parameters[0] = EffectHelps.ConvertToDouble(TokenValues[0]);
            mFilterToken.Parameters[1] = EffectHelps.ConvertToDouble(TokenValues[1]);
            return mFilterToken;
        }

        protected override void button1_Click(object sender, EventArgs e)
        {
            Bitmap holding = null;
            for (int i = 0; i < ImageNames.Length; i += 1)
            {
                holding = ImagingTools.RotateImage(new Bitmap(ImageNames[i]), (float)(Angles[i] - 180));
                holding.Save(PathOut + "\\" + Fileout + string.Format("{0:000}", i) + exten);
                pictureBox1.Image = holding;
                pictureBox1.Invalidate();
                Application.DoEvents();
            }
            timer.Enabled = false;
            timer = null;
            this.Close();
        }
    }
}
