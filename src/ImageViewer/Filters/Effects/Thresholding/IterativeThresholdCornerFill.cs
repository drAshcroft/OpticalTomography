using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using MathHelpLib;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MathHelpLib.ImageProcessing;

namespace ImageViewer.Filters.Thresholding
{
    public class IterativeThresholdCornerFillEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Iterative Threshold With Corner Filling"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Thresholding"; } }

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(EffectHelps.ConvertToBitmap( SourceImage) );
            AForge.Imaging.Filters.IterativeThreshold Filter = new IterativeThreshold();
            holding =Filter.Apply(holding);

            FloodFiller ff = new FloodFiller();
            ff.FillColor = 255;
            ff.FloodFill(holding, new Point(holding.Width/2, 1));
            ff.FloodFill(holding,new Point (holding.Width /2,holding.Height-1));
            return holding;
        }


        public static ImageHolder IterativeThreshold(ImageHolder SourceImage)
        {
            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(EffectHelps.ConvertToBitmap(SourceImage));
            AForge.Imaging.Filters.IterativeThreshold Filter = new IterativeThreshold();
            holding = Filter.Apply(holding);

            FloodFiller ff = new FloodFiller();
            ff.FillColor = 255;
            ff.FloodFill(holding, new Point(holding.Width / 2, 1));
            ff.FloodFill(holding, new Point(holding.Width / 2, holding.Height - 1));
            

            return EffectHelps.FixImageFormat((holding));
        }

        public static Bitmap IterativeThreshold(Bitmap SourceImage)
        {
            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(EffectHelps.ConvertToBitmap(SourceImage));
            AForge.Imaging.Filters.IterativeThreshold Filter = new IterativeThreshold();
            holding = Filter.Apply(holding);

            FloodFiller ff = new FloodFiller();
            ff.FillColor = 255;
            ff.FloodFill(holding, new Point(holding.Width / 2, 1));
            ff.FloodFill(holding, new Point(holding.Width / 2, holding.Height - 1));
            return holding;
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] {""}; }
        }
       
    }



}
