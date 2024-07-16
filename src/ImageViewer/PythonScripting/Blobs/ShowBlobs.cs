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
    public partial class ShowBlobsTool : aEffectForm
    {
        public ShowBlobsTool()
            : base()
        {

        }
        public override string EffectName { get { return "Show Blobs"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Blobs"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 25;
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
            get { return null; }
        }

        public override  string[] ParameterList
        {
            get { return new string[]{"|"}; }
        }

        /// <summary>
        /// makes the transparent sheet with a square where all the blobs are on an image.
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData">optional contains a BlobDescription or BlobDescription[] in the key "Blobs"</param>
        /// <param name="Parameters">optional contains a BlobDescription or BlobDescription[]</param>
        /// <returns></returns>
        protected override object doEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mDataEnvironment = dataEnvironment ;
            mFilterToken = Parameters ;

            if (mFilterToken == null)
                mFilterToken = DefaultProperties;

            Bitmap holding = EffectHelps.ConvertToBitmap( SourceImage) ;
            BlobDescription[] Blobs=null;
            if (mPassData == null || (mPassData.ContainsKey("Blobs") == false) || mPassData["Blobs"].GetType() != typeof(BlobDescription[]))
            {
                if (Parameters == null || Parameters[0].GetType() != typeof(BlobDescription[]) || Parameters[0].GetType() != typeof(BlobDescription))
                {
                    if (Parameters[0].GetType() == typeof(BlobDescription))
                        Blobs = new BlobDescription[] { (BlobDescription)Parameters[0] };
                    else
                        Blobs = (BlobDescription[])Parameters[0];
                }
                else 
                    throw new Exception("You must run 'Get Blob Descriptions' before you run this filter");
            }
            else
            {
                
                if (mPassData["Blobs"].GetType() == typeof(BlobDescription))
                    Blobs = new BlobDescription[] { (BlobDescription)mPassData["BlobDescription"] };
                else
                    Blobs = (BlobDescription[])mPassData["Blobs"];
               
            }
            Graphics g = Graphics.FromImage(holding);
            for (int i = 0; i < Blobs.Length; i++)
            {
                g.DrawRectangle(Pens.Red, Blobs[i].BlobBounds);
                g.DrawLine(Pens.Red, new Point(Blobs[i].CenterOfGravity.X - 10, Blobs[i].CenterOfGravity.Y), new Point(Blobs[i].CenterOfGravity.X + 10, Blobs[i].CenterOfGravity.Y));
                g.DrawLine(Pens.Red, new Point(Blobs[i].CenterOfGravity.X, Blobs[i].CenterOfGravity.Y - 10), new Point(Blobs[i].CenterOfGravity.X, Blobs[i].CenterOfGravity.Y + 10));
            }
            return new ImageHolder (holding);

        }
    }
}
