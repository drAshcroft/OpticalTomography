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
using MathHelpLib;

namespace ImageViewer.PythonScripting.Programming_Tools
{
    public class PadImageToNewEffect : aEffectNoForm
    {
        public override string EffectName { get { return "Pad image to New image"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Programming"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 45;
            }
        }


        public override bool PassesPassData
        {
            get
            {
                return false;
            }
        }

        public static ImageHolder PadImage(ImageHolder SourceImage, Rectangle ClipRegion)
        {
            ImageHolder source = (ImageHolder)SourceImage;
            ImageHolder Dest = new ImageHolder(ClipRegion.Width, ClipRegion.Height, source.NChannels);

            Dest.ROI = new Rectangle((int)Math.Truncate((double)(Dest.Width - source.Width) / 2d), (int)Math.Truncate((double)(Dest.Height - source.Height) / 2d), source.Width, source.Height);
            source.ROI = new Rectangle(0, 0, source.Width, source.Height);
            try
            {
                source.CopyTo(Dest);
            }
            catch
            {
                throw new Exception("New size is smaller than existing image.  Please specify a larger size");
            }
            return Dest;
        }

        public static Bitmap PadImage(Bitmap source, Rectangle ClipRegion)
        {
            Bitmap Dest = new Bitmap(ClipRegion.Width, ClipRegion.Height, PixelFormat.Format32bppRgb);
            Graphics g = Graphics.FromImage(Dest);
            g.DrawImage(source, new Point((int)Math.Truncate((double)(Dest.Width - source.Width) / 2d), (int)Math.Truncate((double)(Dest.Height - source.Height) / 2d)));
            g = null;
            return (Dest);
        }

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;
            mDataEnvironment = dataEnvironment;
            if (Parameters == null)
            {
                mFilterToken = DefaultProperties;
            }


            Rectangle clippingRegion;
            if (mFilterToken[0] is Rectangle)
            {
                clippingRegion = (Rectangle)mFilterToken[0];


                if (SourceImage.GetType() == typeof(Bitmap))
                {
                    return PadImage((Bitmap)SourceImage, clippingRegion);
                }
                else if (SourceImage.GetType() == typeof(ImageHolder))
                {
                    return PadImage((ImageHolder)SourceImage, clippingRegion);
                }

                return SourceImage;
            }
            else
                return SourceImage;
        }
        public override object[] DefaultProperties
        {
            get
            {
                //poor mans clone
                return new object[] { Rectangle.Inflate(mDataEnvironment.Screen.ScreenCoords, 0, 0) };
            }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "NewSize|Rectangle" }; }
        }




    }
}
