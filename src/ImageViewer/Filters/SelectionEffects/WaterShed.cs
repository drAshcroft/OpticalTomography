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
using ImageViewer.Filters;

namespace ImageViewer.Filters.SelectionEffects
{
    public partial class WaterShedTool : Form, IEffect
    {
        public WaterShedTool()
        {
            InitializeComponent();
        }
        public string EffectName { get { return "Threshold and WaterShed"; } }
        public string EffectMenu { get { return "Selections"; } }
        public string EffectSubMenu { get { return ""; } }
        public int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
        object mPassData = null;
        public object PassData
        {
            get { return mPassData; }
            set { mPassData = value; }
        }
        public IEffectToken CurrentProperties { get { return mThresholdToken; } }

        ScreenProperties[] mSourceImage;
        IEffectToken mThresholdToken;
        Bitmap ScratchImage = null;
        Bitmap holding = null;
        public void RunEffect(ScreenProperties SourceImage, IEffectToken ThresholdToken)
        {
            ScreenProperties[] sp = { SourceImage };
            RunEffect(sp, ThresholdToken);
        }
        public string RunEffect(ScreenProperties[] SourceImage, IEffectToken ThresholdToken)
        {
            if (ThresholdToken == null)
            {
                ThresholdToken = new GeneralToken();
                ThresholdToken.Parameters = new object[1];
                ThresholdToken.Parameters[0] = 0;
            }

            mSourceImage = SourceImage;
            mThresholdToken = ThresholdToken;
            ScratchImage = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format32bppArgb);

            Bitmap ShowImage = SourceImage[0].ActiveSelectedImage;
            Graphics.FromImage(ScratchImage).DrawImage(ShowImage,
                new Rectangle(0, 0, ScratchImage.Width, ScratchImage.Height),
                new Rectangle(0, 0, ShowImage.Width, ShowImage.Height), GraphicsUnit.Pixel);

            holding = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format32bppArgb);
            DoRun();

            while (this.Visible == true)
                Application.DoEvents();

            return EffectHelps.FormatMacroString(this) + "\nPassData=Filter.PassData\n" ;
        }

        public Bitmap RunEffect(ScreenProperties screenProperties, Bitmap SourceImage, IEffectToken ThresholdToken)
        {

            holding = ThresholdImage(SourceImage, (int)ThresholdToken.Parameters[0]);
            FloodFiller ff = new FloodFiller();
            ff.FillStyle = FloodFillStyle.Linear;

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = holding.LockBits(new Rectangle(0, 0, SourceImage.Width, SourceImage.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Random rnd = new Random();
            Int32 ParticleCount = 0;
            Int32 Black = Color.Black.ToArgb();
            Int32 White = Color.White.ToArgb();
            Int32 Red = Color.Red.ToArgb();
           
            unsafe
            {
                for (int y = 0; y < SourceImage.Height; ++y)
                {
                    Int32* p = (Int32*)((byte*)(void*)bmData.Scan0 + bmData.Stride * y);
                    for (int x = 0; x < SourceImage.Width; ++x)
                    {
                        if (*p == Black)
                        {
                            ParticleCount++;
                            ff.FillColor = Color.FromArgb(ParticleCount);
                            ff.FloodFill(bmData, new Point(x, y));
                        }

                        p++;
                    }
                }
            }

            holding.UnlockBits(bmData);
            mPassData = ParticleCount;
            //if (CenterX!=-1)
             //   Graphics.FromImage(holding).DrawRectangle(Pens.Red,Rectangle.FromLTRB(CenterX-5,CenterY-5,CenterX+5,CenterY+5));
            return holding;
        }

        private Bitmap ThresholdImage(Bitmap image, Int32 AverageIntensityThreshold)
        {
            Bitmap bOut = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmDataOut = bOut.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Int32 averageValue;
            Int32 Threshold = 3 * AverageIntensityThreshold;
            unsafe
            {
                for (int y = 0; y < image.Height; ++y)
                {
                    byte* p = (byte*)(void*)bmData.Scan0 + bmData.Stride * y;
                    Int32* pOut = (Int32*)((byte*)(void*)bmDataOut.Scan0 + bmDataOut.Stride * y);
                    for (int x = 0; x < image.Width; ++x)
                    {
                        averageValue = p[0] + p[1] + p[2];
                        if (averageValue > Threshold)
                        {
                            *pOut = Color.White.ToArgb();
                        }
                        else
                            *pOut = Color.Black.ToArgb();
                        p += 4;
                        pOut++;
                    }
                }
            }

            image.UnlockBits(bmData);
            bOut.UnlockBits(bmDataOut);
            // Graphics.FromImage(bOut).Clear(Color.Blue);
            return bOut;
        }

        private void DoRun()
        {
            pictureBox1.Image = RunEffect(mSourceImage[0],ScratchImage, mThresholdToken);
            pictureBox1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < mSourceImage.Length; i++)
            {
                mSourceImage[i].ActiveSelectedImage = RunEffect(mSourceImage[i],mSourceImage[i].ActiveSelectedImage, mThresholdToken);
                mSourceImage[i].RedrawBuffers();
            }
            this.Hide();
        }

        private void sBrightness_ValueChanged(object sender, EventArgs e)
        {
            mThresholdToken.Parameters[0] = (int)sHue.Value;
            DoRun();
        }

    }
}
