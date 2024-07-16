using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageViewer;
using System.Drawing;
using MathHelpLib;
using System.Threading.Tasks;

namespace DoRecons
{
    class OldReconstruction
    {

        DataEnvironment dataEnvironment;
        MathHelpLib.ProjectionFilters.ProjectionArrayObject DensityGrid;
        Rectangle ReconCutDownRect;


        private void DoSlice(int ImageNumber)
        {
            double[,] Slice = Scripts.FBPScripts.DoSliceConvolution(dataEnvironment, dataEnvironment.AllImages[ImageNumber], DensityGrid, ReconCutDownRect);

            double AngleRadians = 2d * Math.PI / (double)dataEnvironment.AllImages.Count * ImageNumber;
            ImageHolder BitmapImage = dataEnvironment.AllImages[ImageNumber];

            Bitmap b = MathHelpLib.ImageProcessing.MathImageHelps.MakeBitmap(Slice); int w = b.Width;

            ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2 Filter = new ImageViewer.PythonScripting.Projection.DoSliceBackProjectionSiddonEffect2();
           // Console.WriteLine(ImageNumber.ToString());
            Filter.DoEffect(dataEnvironment, BitmapImage, null, Slice, DensityGrid, AngleRadians);
        }

        public void FBPReconstruction(DataEnvironment dataEnvironment, MathHelpLib.ProjectionFilters.ProjectionArrayObject DensityGrid, Rectangle ReconCutDownRect)
        {
            this.dataEnvironment = dataEnvironment;
            this.DensityGrid = DensityGrid;
            this.ReconCutDownRect = ReconCutDownRect;

            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = Environment.ProcessorCount - 1;

            int numberOfImages = dataEnvironment.AllImages.Count;

            Parallel.For(0, numberOfImages, po, x => DoSlice(x));
        }

    }
}
