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

namespace ImageViewer.Filters.Blobs
{
    public class ConvertBlobToRectangle : aEffectNoForm
    {
        public override string EffectName { get { return "Convert Blob to Rectangle"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Blobs"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 50;
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
        /// Takes the blob descriptions, and converts the first one into a rectangle, or the maxblob first if it is available.
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">a blob description, a list of blob descriptions, or null if there is a "maxBlob" in passdata</param>
        /// <returns></returns>
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mPassData = PassData;
            if (Parameters != null)
            {
                if (Parameters[0].GetType() == typeof(BlobDescription))
                {
                    mPassData.AddSafe("Bounds", ((BlobDescription)Parameters[0]).BlobBounds);
                }
                else if (Parameters[0].GetType() == typeof(List<BlobDescription>))
                {
                    mPassData.AddSafe("Bounds", ((List< BlobDescription>)Parameters[0])[0].BlobBounds);
                }
                else if (Parameters[0].GetType() == typeof(BlobDescription[]))
                {
                    mPassData.AddSafe("Bounds", ((BlobDescription[])Parameters[0])[0].BlobBounds);
                }
            }
            else if (mPassData.ContainsKey("MaxBlob")==true )
            {
                mPassData.AddSafe("Bounds", ((BlobDescription)mPassData["MaxBlob"]).BlobBounds);
            }
            
            else 
                throw new Exception("Please run watershed filter before this filter");

            return SourceImage ;
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "|" }; }
        }
       
       
    }
}
