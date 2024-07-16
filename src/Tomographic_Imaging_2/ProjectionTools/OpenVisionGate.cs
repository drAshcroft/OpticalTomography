using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using MathHelpLib;
using System.IO;

namespace Tomographic_Imaging_2
{
    class OpenVisionGate
    {
        public static void FixVisionGateImage(string Filename)
        {
            Bitmap b = new Bitmap(Filename);
            int iWidth = b.Width;
            int iHeight = b.Height;

            Bitmap b2 = new Bitmap(iWidth / 2, iHeight / 2, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
            BitmapData bmdOut = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height), ImageLockMode.WriteOnly, b2.PixelFormat);

            unsafe
            {

                if (bmd.Stride / (double)bmd.Width == 4)
                {
                        int ccY = 0;
                        for (int y = 0; y < iHeight; y += 2)
                        {
                            Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y ) * bmd.Stride);
                            Int32* scanlineOut = (Int32*)((byte*)bmdOut.Scan0 + (ccY) * bmdOut.Stride);
                            for (int x = 0; x < iWidth; x += 2)
                            {
                                *scanlineOut  = *scanline;
                                scanline += 2;
                                scanlineOut ++;
                            }
                            ccY++;
                        }
                    
                }
                else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    for (int y = 0; y < iHeight; y++)
                    {
                        byte* scanline = ((byte*)bmd.Scan0 + (y) * bmd.Stride);

                        for (int x = 0; x < iWidth; x++)
                        {
                            byte* bits = (byte*)scanline;

                            scanline += 3;
                        }
                    }

                }
            }
            b.UnlockBits(bmd);
            b2.UnlockBits(bmdOut);
            b2.Save(Path.GetDirectoryName(Filename) + "\\New" + Path.GetFileName(Filename));
            b.Dispose();
            b2.Dispose();
            b2 = null;
            b=null;
        }
    }
}
