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
    public partial class ShowBlobsTool : Form, IEffect
    {
        public ShowBlobsTool()
        {
            InitializeComponent();
        }
        public string EffectName { get { return "Show Blobs"; } }
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
            Bitmap ShowImage = SourceImage[0].ActiveSelectedImage;
            ScratchImage = new Bitmap(ShowImage.Width, ShowImage.Height, PixelFormat.Format32bppArgb);

            
            Graphics.FromImage(ScratchImage).DrawImage(ShowImage,
                new Rectangle(0, 0, ScratchImage.Width, ScratchImage.Height),
                new Rectangle(0, 0, ShowImage.Width, ShowImage.Height), GraphicsUnit.Pixel);

            holding = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format32bppArgb);
            DoRun();

            while (this.Visible == true)
                Application.DoEvents();

            return EffectHelps.FormatParameterlessMacroString("Filter.PassData=PassData", this);
        }

        public Bitmap RunEffect(ScreenProperties screenProperties, Bitmap SourceImage, IEffectToken ThresholdToken)
        {
            holding = (Bitmap)SourceImage.Clone();

            if (mPassData == null || !(mPassData.GetType() == typeof(BlobDescription[]) || mPassData.GetType() == typeof(BlobDescription)))
                throw new Exception("You must run 'Get Blob Descriptions' before you run this filter");
            else
            {
                BlobDescription[] Blobs;
                if (mPassData.GetType() == typeof(BlobDescription))
                    Blobs = new BlobDescription[] { (BlobDescription)mPassData };
                else
                    Blobs = (BlobDescription[])mPassData;
                Graphics g = Graphics.FromImage(holding);
                for (int i = 0; i < Blobs.Length; i++)
                {
                    g.DrawRectangle(Pens.Red, Blobs[i].BlobBounds);
                    g.DrawLine(Pens.Red, new Point(Blobs[i].CenterOfGravity.X - 10, Blobs[i].CenterOfGravity.Y), new Point(Blobs[i].CenterOfGravity.X + 10, Blobs[i].CenterOfGravity.Y));
                    g.DrawLine(Pens.Red, new Point(Blobs[i].CenterOfGravity.X, Blobs[i].CenterOfGravity.Y - 10), new Point(Blobs[i].CenterOfGravity.X, Blobs[i].CenterOfGravity.Y + 10));
                }
            }

            return holding;
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



    }
}
