﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ZedGraph;
using System.IO;
using ImageViewer3D._3DItems;
using ImageViewer;
using MathHelpLib;
using MathHelpLib.ImageProcessing;

namespace ImageViewer3D.Filters.Files
{
    public class CombineMIPTool : aEffectForm3D
    {
        Timer timer;
        double MaxContrast = 255;
        double MinContrast = 0;
        public CombineMIPTool()
            : base()
        {
            timer = new Timer();
            timer.Enabled = false;
            timer.Interval = 100;
            timer.Tick += new EventHandler(timer_Tick);

            Button button2 = new Button();
            button2.Click += new EventHandler(button2_Click);

            splitContainer1.Panel1.Controls.Add(button2);


            button2.Top = (int)((double)splitContainer1.Panel2.Height / 2d);
            button2.Visible = true;
            button2.BringToFront();
            button2.Size = new System.Drawing.Size(67, 55);
            button2.Left = splitContainer1.Panel1.Width - button2.Width - 10;
            button2.TabIndex = 25;
            button2.Text = "Save Video";
            button2.UseVisualStyleBackColor = true;
        }

        void button2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            string tempDir = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp\\_MIP\\";

            if (Directory.Exists(tempDir) == false)
                Directory.CreateDirectory(tempDir);
            else
            {
                string[] Files = Directory.GetFiles(tempDir);
                foreach (string s in Files)
                    File.Delete(s);
            }
            SaveFileDialog sfd = new SaveFileDialog();
            DialogResult ret = sfd.ShowDialog();
            if (ret == DialogResult.OK)
            {

                List<string> FrameNames = new List<string>();
                double[,] ProjectionOut = new double[mSourceImage.Width, mSourceImage.Height];
                for (int i = 0; i < 360; i++)
                {
                    DoMIPProjection_OneSlice(ref Projection, (double)i / 180d * Math.PI, mSourceImage);
                    Bitmap holdingRed = MathImageHelps. MakeBitmap(Projection);//,MinContrast,MaxContrast );

                    DoMIPProjection_OneSlice(ref Projection2, (double)i / 180d * Math.PI, OtherImage);
                    Bitmap holdingGreen = MathImageHelps.MakeBitmap(Projection2);//,MinContrast,MaxContrast );

                    Bitmap bmp = ImagingTools.CombineImages(holdingRed, Color.Red, holdingGreen, Color.Green);

                    string frameName = tempDir + "Image" + string.Format("{0:0000}", i) + ".bmp";
                    FrameNames.Add(frameName);
                    bmp.Save(frameName);
                    pictureDisplay1.Image = bmp;
                    pictureDisplay1.Invalidate();
                    Application.DoEvents();
                }

                currentIndex = 0;

                ImagingTools.CreateAVIVideo(sfd.FileName, FrameNames.ToArray());
            }

            MessageBox.Show("AVI Conversion is Complete", "");
            string[] FilesD = Directory.GetFiles(tempDir);
            foreach (string s in FilesD)
                File.Delete(s);
            timer.Enabled = true;
        }

        float[,] Projection = new float[10, 10];
        float[,] Projection2 = new float[10, 10];
        void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;
            DoMIPProjection_OneSlice(ref Projection, (double)currentIndex / 180d * Math.PI,mSourceImage );
            Bitmap holdingRed = MathImageHelps.MakeBitmap(Projection);//,MinContrast,MaxContrast );

            DoMIPProjection_OneSlice(ref Projection2, (double)currentIndex / 180d * Math.PI, OtherImage );
            Bitmap holdingGreen = MathImageHelps.MakeBitmap(Projection2);//,MinContrast,MaxContrast );

            pictureDisplay1.Image = ImagingTools.CombineImages(holdingRed, Color.Red, holdingGreen, Color.Green);
            pictureDisplay1.Invalidate();
            Application.DoEvents();
            currentIndex += 10;
            if (currentIndex > 360) currentIndex = 0;
            if (this.Visible == true)
                timer.Enabled = true;
        }

        public override string EffectName { get { return "Combined Datasets MIP Video"; } }
        public override string EffectMenu { get { return "File"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 1; } }

        public override object[] DefaultProperties
        {
            get { return new object[] { null }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "" }; }
        }

        DataHolder OtherImage = null;

        protected override ImageViewer3D.DataHolder doEffect(ImageViewer3D.DataEnvironment3D dataEnvironment, ImageViewer3D.DataHolder SourceImage, ImageViewer.Filters.ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mParameters = Parameters;
            if (mParameters == null || mParameters[0] == null || (string)mParameters[0] == "DefaultFile.cct")
            {
                OpenFileDialog sfd = new OpenFileDialog();
                sfd.Multiselect = true;
                DialogResult ret = sfd.ShowDialog();
                if (DialogResult.OK == ret)
                {
                    mParameters = sfd.FileNames;
                }
            }
            else
                mParameters = Parameters;

            timer.Enabled = true;

            mSourceImage = SourceImage;

            MaxContrast = dataEnvironment.MaxContrast;
            MinContrast = dataEnvironment.MinContrast;

            currentIndex = 0;


            try
            {
                //the files can either be a list of bitmaps or just one raw file
                double[, ,] Data;
                if (mParameters[0].GetType() == typeof(string))
                {
                    Data = ImagingTools3D.OpenVolumnData((string)mParameters[0]);
                }
                else
                {
                    string[] Filenames = (string[])mParameters[0];
                    Data = ImagingTools3D.OpenVolumnData(Filenames);
                }

                if (Data != null)
                {
                    OtherImage = new DataHolder(Data);
                }
                else
                    return SourceImage;
            }
            catch
            {
                return SourceImage;
            }
            return SourceImage;
        }

        int currentIndex;

        public static void DoMIPProjection_OneSlice(ref float[,] SliceProjection, double AngleRadians, DataHolder SourceImage)
        {

            Point3D vRotationAxis = new Point3D();
            Point3D axis = new Point3D();

            vRotationAxis = new Point3D(0, 0, 1);
            axis = new Point3D(0, 1, 0);

            Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, AngleRadians);

            SliceProjection = ProjectMIP(SourceImage.Data, vec, Point3D.CrossProduct(vec, vRotationAxis));

        }

        public static float[,] ProjectMIP(float[, ,] mDataDouble, Point3D Direction, Point3D FastScanDirection)
        {
            double[] mPhysicalStart = new double[] { -1, -1, -1 };
            double[] mPhysicalEnd = new double[] { 1, 1, 1 };
            double[] mPhysicalStep = new double[] { 2d / mDataDouble.GetLength(0), 2d / mDataDouble.GetLength(1), 2d / mDataDouble.GetLength(2) };

            double LengthCorner = 0;
            double StepSize = double.MaxValue;
            for (int i = 1; i < 3; i++)
            {
                LengthCorner += mPhysicalStart[i] * mPhysicalStart[i];
                if (mPhysicalStep[i] < StepSize)
                    StepSize = mPhysicalStep[i];
            }
            LengthCorner = Math.Sqrt(LengthCorner) / Math.Sqrt(2);

            float[,] PImage = new float[(int)(2 * LengthCorner / StepSize), (int)(2 * LengthCorner / StepSize)];

            FastScanDirection.Normalize();

            Point3D FastScanAxis = FastScanDirection * StepSize;
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanAxis);
            Point3D Origin = -1 * Direction * LengthCorner - FastScanDirection * LengthCorner - SlowScanAxis / StepSize * LengthCorner;
            Direction.Normalize();
            Direction *= StepSize;

            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LZ = mDataDouble.GetLength(0);
            int LOut = PImage.GetLength(0);

            double sX = mPhysicalStart[2];
            double sY = mPhysicalStart[1];
            double sZ = mPhysicalStart[0];

            double stepX = mPhysicalStep[2];
            double stepY = mPhysicalStep[1];
            double stepZ = mPhysicalStep[0];

            double x1, y1, z1;
            double x2, y2, z2;
            double x3, y3, z3;

            double nXp1, nYp1, nZp1;
            double sum;//, Count;
            double uX, uY, Xu, Yu;
            double val;
            int xL, yL;
            for (int tnXI = 0; tnXI < LOut; tnXI++)
            {
                x1 = Origin.X + FastScanAxis.X * tnXI - mPhysicalStart[2];
                y1 = Origin.Y + FastScanAxis.Y * tnXI - mPhysicalStart[1];
                z1 = Origin.Z + FastScanAxis.Z * tnXI - mPhysicalStart[0];
                double A;//, B, C, D;
                //for (int tnYI = 0; tnYI < LOut; tnYI++)
                for (int tnYI = 0; tnYI < LZ; tnYI++)
                {
                    x2 = x1 + SlowScanAxis.X * tnYI;
                    y2 = y1 + SlowScanAxis.Y * tnYI;
                    z2 = z1 + SlowScanAxis.Z * tnYI;
                    sum = 0;
                    //Count = 0;
                    for (int tI = 0; tI < LOut; tI++)
                    {
                        x3 = (x2 + Direction.X * tI);
                        y3 = (y2 + Direction.Y * tI);
                        z3 = (z2 + Direction.Z * tI);

                        nXp1 = (x3) / stepX;
                        nYp1 = (y3) / stepY;
                        //nZp1 = (z3) / stepZ;
                        nZp1 = tnYI;

                        if ((nXp1 >= 0 && nXp1 < LX))
                            if (nYp1 >= 0 && nYp1 < LY)
                                if (nZp1 >= 0 && nZp1 < LZ)
                                {
                                    if (nXp1 >= LX - 1 || nYp1 >= LY - 1)
                                    {
                                        val = mDataDouble[(int)nZp1, (int)nXp1, (int)nYp1];
                                    }
                                    else
                                    {
                                        //bilinear interpolation
                                        xL = (int)nXp1;
                                        yL = (int)nYp1;
                                        uX = 1 - (nXp1 - xL);
                                        uY = nYp1 - yL;
                                        Xu = (1 - uX);
                                        Yu = (1 - uY);

                                        A = mDataDouble[(int)nZp1, yL, xL];// *Xu * Yu;
                                        //B = 0;// mDataDouble[(int)nZp1, yL, xL + 1] * uX * Yu;
                                        //C = 0;// mDataDouble[(int)nZp1, yL + 1, xL] * Xu * uY;
                                        //D = 0;// mDataDouble[(int)nZp1, yL + 1, xL + 1] * uX * uY;
                                        val = A;
                                        //if (B > val) val=B;
                                        //if (C > val) val = C;
                                        //if (D > val) val = D;

                                    }
                                    if (val > sum) sum = val;

                                }
                    }
                    PImage[tnXI, tnYI] =(float) sum;
                }
            }

            return PImage;
        }
    }
}
