﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Imaging.Filters;
using System.Drawing;
using MathHelpLib;

namespace ImageViewer.Filters.Effects.Morphology
{
    public class DilultionTool:ErosionTool 
    {
        public override string EffectName
        {
            get
            {
                return "Dilation Filter";
            }
        }
        public override System.Drawing.Bitmap RunEffectImpl(System.Drawing.Bitmap SourceImage, short[,] StructureElement)
        {
            Dilatation  Filter = new AForge.Imaging.Filters.Dilatation (StructureElement);
            Bitmap holding = Filter.Apply(SourceImage );
            return holding;
        }

        /// <summary>
        /// Converts image to a grayscale to apply the morphology filter.  The size must be odd and larger than 3
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="Size">Must be odd and larger than 3</param>
        /// <returns></returns>
        public static ImageHolder Dilatation(ImageHolder SourceImage, int Size)
        {
            short[,] StructureElement = StructuringElement.CreateSECircle((byte)Size);
            AForge.Imaging.Filters.Dilatation Filter = new AForge.Imaging.Filters.Dilatation(StructureElement);
            Bitmap holding = Filter.Apply(SourceImage.ToBitmap());
            return new ImageHolder(holding);
        }
        /// <summary>
        /// Converts image to a grayscale to apply the morphology filter.  The size must be odd and larger than 3
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="Size">Must be odd and larger than 3</param>
        /// <returns></returns>
        public static Bitmap Dilatation(Bitmap SourceImage, int Size)
        {
            short[,] StructureElement = StructuringElement.CreateSECircle((byte)Size);
            AForge.Imaging.Filters.Dilatation Filter = new AForge.Imaging.Filters.Dilatation(StructureElement);
            Bitmap holding = Filter.Apply(SourceImage);
            return holding;
        }

    }
}