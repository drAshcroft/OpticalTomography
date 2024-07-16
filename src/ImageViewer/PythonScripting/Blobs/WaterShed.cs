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
using MathHelpLib;

namespace ImageViewer.Filters.Blobs
{
    public partial class WaterShedAndThresholdTool : aEffectForm
    {
        public WaterShedAndThresholdTool()
            : base()
        {
            SetParameters(new string[] { "Threshold" }, new int[] { 0 }, new int[] { 255 });
        }
        public override string EffectName { get { return "WaterShed And Threshold"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Blobs"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
       
        public override bool PassesPassData
        {
            get
            {
                return true;
            }
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { 200 }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "Threshold|int" }; }
        }

        /// <summary>
        /// Performs a watershed on an image that has been already thresholded.  returns all the blobs in an image
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        protected override object doEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            double ThresholdV = (double)(int)Parameters[0];

            Bitmap holding = null;
            if (SourceImage.GetType() == typeof(Bitmap))
            {
                holding = ImagingTools.ThresholdImage((Bitmap)SourceImage, (int)ThresholdV);
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                ImageHolder ih = ((ImageHolder)SourceImage);
                ih.ThresholdBinaryWhite((float)ThresholdV);
                holding = ih.ToBitmap();
            }

            //the routine only works on bitmaps right now.  should be updated to work on imageholders
            BlobDescription[] Blobs = WaterShedTool.DoWatershed(holding);

            mPassData.AddSafe ("BlobDescription", Blobs );

            return SourceImage ;
        }
        

        private Bitmap ShowBlobs(Bitmap SourceImage )
        {
            try
            {
                Bitmap holding = (Bitmap)SourceImage.Clone();

                BlobDescription[] Blobs;
                if (mPassData.GetType() == typeof(BlobDescription))
                    Blobs = new BlobDescription[] { (BlobDescription)mPassData["BlobDescription"] };
                else
                    Blobs = (BlobDescription[])mPassData["BlobDescription"];
                Graphics g = Graphics.FromImage(holding);
                for (int i = 0; i < Blobs.Length; i++)
                {
                    g.DrawRectangle(Pens.Red, Blobs[i].BlobBounds);
                    g.DrawLine(Pens.Red, new Point(Blobs[i].CenterOfGravity.X - 10, Blobs[i].CenterOfGravity.Y), new Point(Blobs[i].CenterOfGravity.X + 10, Blobs[i].CenterOfGravity.Y));
                    g.DrawLine(Pens.Red, new Point(Blobs[i].CenterOfGravity.X, Blobs[i].CenterOfGravity.Y - 10), new Point(Blobs[i].CenterOfGravity.X, Blobs[i].CenterOfGravity.Y + 10));
                }

                return holding;
            }
            catch
            {
                return SourceImage;
            }

        }

        protected override  void DoRun()
        {
            
        }

    }
}
