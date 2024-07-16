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
using ZedGraph;
using System.IO;

namespace ImageViewer.Filters
{
    public class MnIPTool : aEffectForm
    {
        Timer timer;
        public MnIPTool()
            : base()
        {
            timer = new Timer();
            timer.Enabled = false;
            timer.Interval = 33;
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
                for (int i = 0; i < 360; i++)
                {
                    currentIndex = i;
                    Bitmap bmp = doEffect(null, null);
                    string frameName = tempDir + "Image" + string.Format("{0:0000}", currentIndex) + ".bmp";
                    FrameNames.Add(frameName);
                    bmp.Save(frameName);
                    pictureBox1.Image = bmp;
                    pictureBox1.Invalidate();
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

        void timer_Tick(object sender, EventArgs e)
        {
            DoRun();
        }
        public event RefreshDataStoreEvent RefreshDataStore;


        PhysicalArray mPData;
        double[,] mSlice;
        double MaxValue = 0;
        double MinValue = 0;
        double MaxContrast = 0;
        double MinContrast = 1000;

        public override string EffectName { get { return "MinIP Video"; } }
        public override string EffectMenu { get { return "3D Effects"; } }
        public override string EffectSubMenu { get { return ""; } }
        public override int OrderSuggestion { get { return 1; } }

        public override IEffectToken DefaultToken { get { return CreateToken(); } }

        public override string RunEffect(ScreenProperties[] SourceImage, IEffectToken FilterToken)
        {
            mFilterToken = FilterToken;
            if (mFilterToken == null)
                mFilterToken = DefaultToken;

            timer.Enabled = true;

            double minContrast = 0, maxContrast = 1;
            double[, ,] mData = null;

            if (RefreshDataStore != null)
                RefreshDataStore(out mData, out mSlice, out  minContrast, out maxContrast);

            mPData = new PhysicalArray(mData, new double[] { -1, -1, -1 }, new double[] { 1, 1, 1 }, false, PhysicalArrayRank.Array3D);
            Projection = new PhysicalArray(mData.GetLength(1), mData.GetLength(0), -1, 1, -1, 1);
            mSourceImages = SourceImage;

            MaxValue = mData.MaxArray();
            MinValue = mData.MinArray();

            MaxContrast = maxContrast * (MaxValue - MinValue) + MinValue;
            MinContrast = minContrast * (MaxValue - MinValue) + MinValue;

            currentIndex = 0;

            string outString = base.RunEffect(SourceImage, FilterToken); ;

            return outString;
        }
        PhysicalArray Projection;
        int currentIndex;
        protected override Bitmap doEffect(Bitmap SourceImage, IEffectToken FilterToken)
        {
            //System.Diagnostics.Debug.Print(mPData.Sum().ToString());
            DoMIPProjection_OneSlice(ref Projection, (double)currentIndex / 180d * Math.PI);
            //System.Diagnostics.Debug.Print(Projection.Sum().ToString());
            Bitmap holding = Projection.MakeBitmap();

            currentIndex++;
            if (currentIndex > 360) currentIndex = 0;
            Projection.Clear();
            return EffectHelps.FixImageFormat(holding);
        }
        public override IEffectToken CreateToken(params object[] TokenValues)
        {
            IEffectToken ContrastToken = new GeneralToken();
            ContrastToken.Parameters = null;

            return ContrastToken;
        }


        public void DoMIPProjection_OneSlice(ref PhysicalArray SliceProjection, double AngleRadians)
        {
            MathHelpLib.Axis RotationAxis = MathHelpLib.Axis.ZAxis;
            if (mPData.ArrayRank == PhysicalArrayRank.Array2D)
            {
                Point3D vec;
                Point3D axis;

                axis = new Point3D(0, 0, 1);

                double rx = Math.Cos(AngleRadians);
                double ry = Math.Sin(AngleRadians);

                vec = new Point3D(ry, rx, 0);

                mPData.SmearArray(SliceProjection, vec, Point3D.CrossProduct(vec, axis));

            }
            else
            {
                Point3D vRotationAxis = new Point3D();
                Point3D axis = new Point3D();

                if (RotationAxis == MathHelpLib.Axis.XAxis)
                {
                    vRotationAxis = new Point3D(1, 0, 0);
                    axis = new Point3D(0, 1, 0);
                }
                else if (RotationAxis == MathHelpLib.Axis.YAxis)
                {
                    vRotationAxis = new Point3D(0, 1, 0);
                    axis = new Point3D(0, 0, 1);
                }
                else if (RotationAxis == MathHelpLib.Axis.ZAxis)
                {
                    vRotationAxis = new Point3D(0, 0, 1);
                    axis = new Point3D(0, 1, 0);
                }

                Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, AngleRadians);

                //mDensityGrid.SmearArray(SliceProjection , vec, Point3D.CrossProduct(vec, vRotationAxis));
                //mPData.SmearArrayInterpolate(SliceProjection, vec, Point3D.CrossProduct(vec, vRotationAxis));
                //SliceProjection =  mPData.ProjectArrayInterpolate(vec, Point3D.CrossProduct(vec, vRotationAxis));
                SliceProjection = mPData.ProjectMnIP(vec, Point3D.CrossProduct(vec, vRotationAxis));
                //mPData.MIPProjection(ref SliceProjection, vec, Point3D.CrossProduct(vec, vRotationAxis));
                //System.Diagnostics.Debug.Print(SliceProjection.Sum().ToString());
            }

        }
    }
}
