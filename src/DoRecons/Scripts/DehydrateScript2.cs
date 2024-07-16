using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ImageViewer;
using ImageViewer.PythonScripting;
using ImageViewer.Filters;
using ImageViewer.Filters.Blobs;
using System.Threading;
using MathHelpLib;
using System.IO;
using MathHelpLib.ProjectionFilters;
using ImageViewer.PythonScripting.Threads;
using Tomographic_Imaging_2;
using System.Drawing.Imaging;
using DoRecons.Scripts;

namespace DoRecons
{
    public class DehydrateScript2 :BaseScript
    {
        protected override ImageHolder ProcessImageLoad(DataEnvironment dataEnvironment, int ImageNumber)
        {
            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

            PassData.AddSafe("Num Loaded Channels", BitmapImage.NChannels);

            //make sure there is only one channel to work with
            if (BitmapImage.NChannels > 1)
            {
                Filter = new ImageViewer.Filters.Adjustments.GrayScaleEffectChannel();
                BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData, 1);
            }

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType == "image/jpeg")
                    ici = codec;
            }

            EncoderParameters ep = new EncoderParameters();
            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)50);

            if (ColorImage == true)
            {
                BitmapImage = MathHelpsFileLoader.FixVisionGateImage(BitmapImage, 2);
            }
            lock (LockDir)
            {
                if (Directory.Exists(dataEnvironment.DataOutFolder + "Dehydrated") == false)
                    Directory.CreateDirectory(dataEnvironment.DataOutFolder + "Dehydrated");
            }
            BitmapImage.MakeFalseColorBitmap().Save(dataEnvironment.DataOutFolder + "Dehydrated\\Whole" + string.Format("{0:000}", ImageNumber) + ".jpg", ici, ep);

            ProcessImageFindRough(dataEnvironment, ImageNumber, BitmapImage);

            if (FluorImage == false)
            {
                // try
                {
                    //divide off the background curvature
                    if (ScriptParams["GlobalFlatten"].ToLower() == "true")
                    {
                        Filter = new ImageViewer.Filters.Effects.Flattening.FlattenEdges1DErrorCorrected();
                        BitmapImage = (ImageHolder)Filter.DoEffect(dataEnvironment, BitmapImage, PassData);
                    }
                }
                // catch { }
            }
            return BitmapImage;
        }

        static object LockDir = new object();
        protected override ImageHolder ProcessBeforeConvolution(DataEnvironment dataEnvironment, int ImageNumber)
        {
            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];
            ReplaceStringDictionary PassData = GlobalPassData;
            IEffect Filter;

        
            BitmapImage.Save(dataEnvironment.DataOutFolder + "Dehydrated\\" + string.Format("{0:000}", ImageNumber) + ".cct");

            //BitmapImage.ToEqualIntensityBitmap().Save(dataEnvironment.DataOutFolder + "Dehydrated\\Center" + string.Format("{0:000}", ImageNumber) + ".png");
            return BitmapImage;
        }

        protected override void DoRun(Dictionary<string, object> Variables)
        {

          //  FluorImage = false;
            //ColorImage = false;

            //format loaded images (i.e. select only one channel) //now done inside find rough
            //  BatchLoopThroughImagesSave(6, dataEnvironment, ImageFileListIn, ScriptParams);

            PreBatchProcessBackgroundDivide(dataEnvironment, ImageFileListIn, ScriptParams);
            PreBatchProcessCenter(dataEnvironment, ImageFileListIn, ScriptParams);

            //do any pre convolution work.  This is where most of the changes should be located
            BatchLoopThroughImagesSave(7, dataEnvironment, ImageFileListIn, ScriptParams);

        }
       

        public override IScript CloneScript()
        {
            return new DehydrateScript2();
        }
    }
}

