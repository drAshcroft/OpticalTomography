using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathHelpLib;
using ImageViewer;
using System.Drawing;

namespace DoRecons.Scripts
{
    public class FBPScripts
    {
        public static double[,] DoSliceConvolution(DataEnvironment dataEnvironment, ImageHolder BitmapImage, MathHelpLib.ProjectionFilters.ProjectionArrayObject reconObject, Rectangle reconCutDownRect)
        {
            double[,] Slice = null;
            if (ConvolutionMethod.Convolution1D == reconObject.DesiredMethod)
            {
                Slice = reconObject.ConvolutionFilter.DoConvolution(dataEnvironment, BitmapImage, reconObject.impulse, 2, 8);
            }
            else
            {
                BitmapImage = ImageViewer.PythonScripting.Programming_Tools.PadImageToNewEffect.PadImage(BitmapImage, new Rectangle(0, 0, 256, 256));
            }

            if (dataEnvironment.ScriptParams["FBPMedian"].ToLower() == "true")
            {
                Slice = ImageViewer.Filters.Effects.RankOrder.MedianFilterTool.MedianFilter(Slice, 3);
            }

            Slice = ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect.ClipImage(Slice, reconCutDownRect);
            return Slice;//.RotateArray();
            //return Slice;
        }



    }
}
