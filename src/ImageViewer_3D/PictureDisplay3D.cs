using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;


namespace ImageViewer3D
{
    public class PictureDisplay3D : PictureBox
    {
        ScreenProperties3D mScreenProperties;

        _3DItems.OgreWindow OgreWindow;

        private int mIndex;
        public int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }

        public ScreenProperties3D ScreenProperties
        {
            get { return mScreenProperties; }
            set { mScreenProperties = value; }
        }

        public PictureDisplay3D()
        {

        }

        public void StartGraphics()
        {
            OgreWindow = new ImageViewer3D._3DItems.OgreWindow();
            OgreWindow.InitMogre(this.Handle);
        }

        public void LoadVolumeData(double[, ,] Data)
        {
            if (OgreWindow == null)
                StartGraphics();

            // double cI = (double)ImageData.GetLength(0) / (double)ImageData.GetLength(0);
            // double cJ = (double)ImageData.GetLength(1) / (double)ImageData.GetLength(1);
            // double cK = (double)ImageData.GetLength(2) / (double)ImageData.GetLength(2);

            float[, ,] Intensities = new float[Data.GetLength(0), Data.GetLength(1), Data.GetLength(2)];
            float Max = float.MinValue;
            for (int i = 0; i < Intensities.GetLength(0); i++)
                for (int j = 0; j < Intensities.GetLength(1); j++)
                    for (int k = 0; k < Intensities.GetLength(2); k++)
                    {
                        //   Intensities[i, j, k] = (float)ImageData[(int)(cI * i), (int)(cJ * j), (int)(cK * k)];
                        Intensities[i, j, k] = (float)Data[i, j, k];
                        if (Intensities[i, j, k] > Max)
                            Max = Intensities[i, j, k];
                    }
            for (int i = 0; i < Intensities.GetLength(0); i++)
                for (int j = 0; j < Intensities.GetLength(1); j++)
                    for (int k = 0; k < Intensities.GetLength(2); k++)
                        Intensities[i, j, k] = Intensities[i, j, k] / Max;

            OgreWindow.Create3DTextureOpaque("Text3D", Intensities);
            OgreWindow.Create3DMaterial("Text3D", "VolumeMaterial");

            //OgreWindow.CreateVolumeRenderer("Text3D", 100, 50);

            ShowCross();
            OgreWindow.Paint();
        }

        public void ShowCross()
        {
            if (OgreWindow == null)
                StartGraphics();

            object Node = OgreWindow.Create3DNode("Cross");
            OgreWindow.CreateXYPlane("PlaneXY", "VolumeMaterial", Node);
            OgreWindow.CreateXZPlane("PlaneXZ", "VolumeMaterial", Node);
            OgreWindow.CreateYZPlane("PlaneYZ", "VolumeMaterial", Node);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (OgreWindow != null)
                OgreWindow.Paint();
        }
    }
}
