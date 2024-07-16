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
using MathHelpLib;


namespace ImageViewer.Filters.Effects.Morphology
{
    public partial class ErosionTool : aEffectForm
    {
        public ErosionTool()
            : base()
        {
            SetParameters(new string[] { "Size" }, new int[] { 3 }, new int[] { 25 });
        }
        public override string EffectName { get { return "Erosion Filter"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Morphology"; } }
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
        /// Converts image to a grayscale to apply the morphology filter.  The size must be odd and larger than 3
        /// </summary>
        /// <param name="dataEnvironment"></param>
        /// <param name="SourceImage"></param>
        /// <param name="PassData"></param>
        /// <param name="Parameters">Size of kernal must be larger than 3 and odd (int)</param>
        /// <returns></returns>
        protected override object doEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            int value = (int)mFilterToken[0];
            if (value < 3) value = 3;
            if (value % 2 == 0) value++;
            mFilterToken[0] = (int)value;

            Bitmap holding = Grayscale.CommonAlgorithms.RMY.Apply(EffectHelps.ConvertToBitmap(SourceImage));
            short[,] SE = StructuringElement.CreateSECircle((byte)value );
            holding = RunEffectImpl(holding, SE);
            return EffectHelps.FixImageFormat(holding);
        }



        public virtual Bitmap RunEffectImpl(Bitmap SourceImage, short[,] StructureElement)
        {
           
            Erosion Filter = new AForge.Imaging.Filters.Erosion(StructureElement);
            Bitmap holding = Filter.Apply(SourceImage);
            return holding;

        }
        /// <summary>
        /// Converts image to a grayscale to apply the morphology filter.  The size must be odd and larger than 3
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="Size">Must be odd and larger than 3</param>
        /// <returns></returns>
        public static ImageHolder Erosion(ImageHolder SourceImage, int Size)
        {
            short[,] StructureElement = StructuringElement.CreateSECircle((byte)Size);
            AForge.Imaging.Filters.Erosion Filter = new AForge.Imaging.Filters.Erosion(StructureElement);
            Bitmap holding = Filter.Apply(SourceImage.ToBitmap());
            return new ImageHolder(holding);
        }
        /// <summary>
        /// Converts image to a grayscale to apply the morphology filter.  The size must be odd and larger than 3
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="Size">Must be odd and larger than 3</param>
        /// <returns></returns>
        public static Bitmap Erosion(Bitmap SourceImage, int Size)
        {
            short[,] StructureElement = StructuringElement.CreateSECircle((byte)Size);
            AForge.Imaging.Filters.Erosion Filter = new AForge.Imaging.Filters.Erosion(StructureElement);
            Bitmap holding = Filter.Apply(SourceImage);
            return holding;
        }


    }
}
