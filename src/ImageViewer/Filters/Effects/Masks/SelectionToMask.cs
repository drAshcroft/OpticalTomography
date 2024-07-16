using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace ImageViewer.Filters.Masks
{
    public class SelectionToMaskEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Selection to Mask"; } }
        public override string EffectMenu { get { return "Effects"; } }
        public override string EffectSubMenu { get { return "Masks"; } }

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             Dictionary<string, object> PassData, params object[] Parameters)
        {
            if (Parameters == null)
                Parameters = DefaultProperties;
            mFilterToken = Parameters;
            mSourceImages = dataEnvironment;

            ISelection selection = mSourceImages.Screen.ActiveSelection;
            if (selection != null)
            {
                mFilterToken[0] = selection.SelectionBounds;
            }
            mSourceImages.Screen.NotifyOfSelection(null);

            Rectangle Bounds = (Rectangle)mFilterToken[0];


            Emgu.CV.Image<Emgu.CV.Structure.Bgr, ushort> bOut = SourceImage.CopyBlank();
            ushort[, ,] bmDataSource = SourceImage.Data;

            ushort[, ,] bmDataOut = bOut.Data;
            byte Intensity;
            for (int y = 0; y < SourceImage.Height; ++y)
            {
                for (int x = 0; x < SourceImage.Width; ++x)
                {
                    if ((x >= Bounds.X && x <= Bounds.Right) && (y >= Bounds.Top && y < Bounds.Bottom))
                        Intensity = 0;
                    else
                        Intensity = 1;

                    bmDataOut[0, x, y] = Intensity;
                    bmDataOut[1, x, y] = Intensity;
                    bmDataOut[2, x, y] = Intensity;
                }
            }

            // Graphics.FromImage(bOut).Clear(Color.Blue);
            return bOut;

        }

        public override object[] DefaultProperties
        {
            get { return new object[] { new Rectangle(0, 0, 10, 10) }; }
        }

        public override  string[] ParameterList
        {
            get { return new string[] { "MaskBounds|Rectangle" }; }
        }

    }
}
