using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using ImageViewer.Filters;
using MathHelpLib;

namespace ImageViewer.Filters.Blobs
{
    public class GetBiggestBlob : aEffectNoForm
    {
        public override string EffectName { get { return "Get Biggest Blob"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Blobs"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 20;
            }
        }

        public override bool PassesPassData
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Finds the biggest blob from the list
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData">list used if there is an item named "Blobs"</param>
        /// <param name="Parameters">accepts and arrray of BlobDescriptions or an list of BlobDescriptions.  Parameter 2 indicates if you wish to have a rectangle drawn on the source image</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mPassData = PassData;

            BlobDescription MaxBlob = null;
            if (Parameters != null && Parameters.Length >0)
            {
                if (Parameters[0].GetType() == typeof(BlobDescription[]))
                {
                   MaxBlob= SortBlobDescriptions(((BlobDescription[])Parameters[0]));
                }
                else if (Parameters[0].GetType() == typeof(List<BlobDescription>))
                {
                   MaxBlob= SortBlobDescriptions(((List<BlobDescription>)Parameters[0]).ToArray());
                }
            }
            else if (mPassData.ContainsKey("Blobs") == true)
            {
               MaxBlob= SortBlobDescriptions(((BlobDescription[])mPassData["Blobs"]));
            }
            else
                throw new Exception("Please run watershed filter before this filter");

            mPassData.AddSafe("MaxBlob", MaxBlob);

            if (mPassData.ContainsKey("MaxBlob") == true && Parameters!=null && (Parameters.Length >= 2 && ((bool)Parameters[1]) == true))
            {
                BlobDescription Mblob=(BlobDescription) mPassData["MaxBlob"];
                if (SourceImage.GetType() == typeof(Bitmap))
                {
                    Graphics g = Graphics.FromImage((Bitmap)SourceImage);
                    g.DrawRectangle(Pens.Red, Mblob.BlobBounds);
                    g.DrawEllipse(Pens.Red, new Rectangle(Mblob.CenterOfGravity, new Size(3, 3)));
                    g = null;
                }
                else if (SourceImage.GetType() == typeof(ImageHolder))
                {
                   // Graphics g = Graphics.FromImage( ((ImageHolder )SourceImage).InformationOverLay );
                   // g.DrawRectangle(Pens.Red, Mblob.BlobBounds);
                   // g.DrawEllipse(Pens.Red, new Rectangle(Mblob.CenterOfGravity, new Size(3, 3)));
                   // g = null;
                }
            }

            return SourceImage ;
        }

        public static BlobDescription GetBiggest(BlobDescription[] Blobs)
        {
            return SortBlobDescriptions(Blobs);
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }

        /// <summary>
        /// pick out the biggest blob by area.
        /// </summary>
        /// <param name="Blobs"></param>
        private static  BlobDescription SortBlobDescriptions(BlobDescription[] Blobs)
        {
            long MaxArea = 0;
            BlobDescription MaxBlob = null;
            long area;
            for (int i = 0; i < Blobs.Length; i++)
            {
                area = Blobs[i].BlobBounds.Height * Blobs[i].BlobBounds.Width;
                if (area > MaxArea)
                {
                    MaxBlob = Blobs[i];
                    MaxArea = area;
                }
            }

            return MaxBlob;
           
        }


    }
}
