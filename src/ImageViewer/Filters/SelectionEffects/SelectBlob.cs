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

namespace ImageViewer.Filters.SelectionEffects
{
    public class SelectBlobEffect : IEffect
    {
        public string EffectName { get { return "Select Blob"; } }
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

        public IEffectToken CurrentProperties { get { return mFilterToken; } }

        IEffectToken mFilterToken;

        public void RunEffect(ScreenProperties SourceImage, IEffectToken FilterToken)
        {
            ScreenProperties[] sp = { SourceImage };
            RunEffect(sp, FilterToken);
        }
        public string RunEffect(ScreenProperties[] SourceImage, IEffectToken FilterToken)
        {
            for (int i = 0; i < SourceImage.Length; i++)
            {
                SourceImage[i].ActiveSelectedImage = RunEffect(SourceImage[i],SourceImage[i].ActiveSelectedImage, FilterToken);
            }
            string MacroString = EffectHelps.FormatParameterlessMacroString("PassData = Filter.PassData", this) + "\n";
            return MacroString;
        }
        

        /// <summary>
        /// This function can only be called when there is only one blob in the pass data.  Either you must select one with the 
        /// script or use get Biggest blob.
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="FilterToken"></param>
        /// <returns></returns>
        public Bitmap RunEffect(ScreenProperties screenProperties, Bitmap SourceImage, IEffectToken FilterToken)
        {

            if (mPassData.GetType() != typeof(BlobDescription))
                throw new Exception("Please select one blob before this filter");
            screenProperties.NotifyOfSelection(
                new Selections.ROISelection(((BlobDescription)mPassData).BlobBounds, ((PictureDisplay) screenProperties.PictureBox).Index ) );

            return (Bitmap)SourceImage.Clone();
        }
        public void Show(IWin32Window owner)
        {
            mFilterToken = new GeneralToken();
            mFilterToken.Parameters = new object[1];
        }
    }
}
