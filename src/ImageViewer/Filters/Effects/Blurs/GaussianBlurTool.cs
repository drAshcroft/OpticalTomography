using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using MathHelpLib;
using MathHelpLib.Convolution;


namespace ImageViewer.Filters.Effects.Blurs
{
    public partial class GaussianBlurTool : aEffectForm
    {
        public GaussianBlurTool()
            : base()
        {
            SetParameters(new string[] { "Radius" }, new int[] { 3 }, new int[] { 21 });
        }
        public override string EffectName { get { return "Gaussian Blur"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Blurs"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }
        public override object[] DefaultProperties
        {
            get { return new object[] { 3 }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Circle Radius|int" }; }
        }

        /// <summary>
        /// Creates the proper Guass shaped filter for 1D filters and seperable filters
        /// </summary>
        /// <param name="Size">Size in pixels</param>
        /// <returns></returns>
        public static float[] Create1DGuassFilter(int Size)
        {
            //   Allocate window buffer
            float[] Weights = new float[Size];
            //   Window half
            int Half = Size >> 1;
            //   Central weight
            Weights[Half] = 1.0f;
            //   The rest of weights
            for (int Weight = 1; Weight < Half + 1; ++Weight)
            {
                //   Support point
                double x = 3.0 * (double)Weight / (double)Half;
                //   Corresponding symmetric weights
                Weights[Half - Weight] = Weights[Half + Weight] = (float)Math.Exp(-x * x / 2.0);
            }
            //   Weight sum
            float k = 0.0f;
            for (int Weight = 0; Weight < Size; ++Weight)
                k += Weights[Weight];
            //   Weight scaling
            for (int Weight = 0; Weight < Size; ++Weight)
                Weights[Weight] /= k;
            return Weights;
        }

        /// <summary>
        /// Uses seperable property of filter to make 2D guassian array
        /// </summary>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static double[,] Create2DGuassFilter(int Size)
        {
            double[,] V1 = new double[1, Size];
            double[,] V2 = new double[1, Size];
            ///guassian filters are seperable, so just mulitply 2 1D images
            float[] Filter1D = Create1DGuassFilter(Size);
            for (int i = 0; i < Size; i++)
            {
                V1[0, i] = Filter1D[i];
                V2[0, i] = Filter1D[i];
            }

            V2 = BasicMatrix.Transpose(V2);
            double[,] Filter = BasicMatrix.Multiply(V1, V2); ;
            return Filter;
        }

        static MathHelpLib.Convolution.ValueArrayKernal vak = null;

        /// <summary>
        /// Applies a guassian filter to the data
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">integer indicating the rank of the filter must be odd</param>
        /// <returns></returns>
        protected override object doEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            int value = (int)mFilterToken[0];
            if (value < 3) value = 3;
            if (value % 2 == 0) value++;
            mFilterToken[0] = value;

            if (vak == null || vak.Rank[0] != value)
                vak = new MathHelpLib.Convolution.ValueArrayKernal(Create2DGuassFilter(value));

            if (SourceImage.GetType() == typeof(Bitmap))
            {
                ImageHolder outI = new ImageHolder((Bitmap)SourceImage).Convolution(vak);
                return outI;
            }
            else if (SourceImage.GetType() == typeof(ImageHolder))
            {
                ImageHolder outI = ((ImageHolder)SourceImage).Convolution(vak);
                return outI;
            }

            return null;
        }

        /// <summary>
        /// Performs guassian smoothing 
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="FilterSize">must be odd larger than 3</param>
        /// <returns></returns>
        public static ImageHolder DoGuassSmooth(ImageHolder SourceImage, int FilterSize)
        {
            int value = FilterSize;
            if (value < 3) value = 3;
            if (value % 2 == 0) value++;

            if (vak == null || vak.Rank[0] != value)
                vak = new MathHelpLib.Convolution.ValueArrayKernal(Create2DGuassFilter(value));


            return SourceImage.Convolution(vak);

        }

        /// <summary>
        /// Performs guassian smoothing 
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="FilterSize">must be odd larger than 3</param>
        /// <returns></returns>
        public static Bitmap  DoGuassSmooth(Bitmap  SourceImage, int FilterSize)
        {
            int value = FilterSize;
            if (value < 3) value = 3;
            if (value % 2 == 0) value++;

            if (vak == null || vak.Rank[0] != value)
                vak = new MathHelpLib.Convolution.ValueArrayKernal(Create2DGuassFilter(value));


            return ConvolutionFilterImplented.ConvolutionFilter(SourceImage, vak);

        }
    

    }
}
