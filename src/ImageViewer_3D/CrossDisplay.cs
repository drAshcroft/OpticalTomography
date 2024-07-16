using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageViewer3D
{
    public partial class CrossDisplay : UserControl
    {
        public CrossDisplay()
        {
            InitializeComponent();
        }
        private Bitmap mImageX;
        private Bitmap mImageY;
        private Bitmap mImageZ;

        public Bitmap ImageX
        {
            get { return mImageX; }
            set
            {
                if (value != null)
                {
                    mImageX = new Bitmap( value);
                    mImageX.RotateFlip(RotateFlipType.Rotate270FlipY   );
                    this.Invalidate();
                }
            }
        }
        public Bitmap ImageY
        {
            get { return mImageY; }
            set
            {
                if (value != null)
                {
                    mImageY = new Bitmap(value);
                    mImageY.RotateFlip(RotateFlipType.Rotate90FlipX  );
                    this.Invalidate();
                }
            }
        }
        public Bitmap ImageZ
        {
            get { return mImageZ; }
            set
            {
                if (value != null)
                {
                    mImageZ = new Bitmap( value );
                    mImageZ.RotateFlip(RotateFlipType.RotateNoneFlipX );
                    this.Invalidate();
                }
            }
        }

        public double SlicePositionX
        {
            set
            {
                XPercent = value;
                this.Invalidate();
            }
            get
            {
                return XPercent;
            }
        }
        public double SlicePositionY
        {
            set
            {
                ZPercent = value;
                this.Invalidate();
            }
            get
            {
                return ZPercent;
            }
        }
        public double SlicePositionZ
        {
            set
            {
                ZPercent = value;
                this.Invalidate();
            }
            get
            {
                return ZPercent;
            }
        }

        private ScreenProperties3D mScreenXY;
        private ScreenProperties3D mScreenXZ;
        private ScreenProperties3D mScreenYZ;

        public ScreenProperties3D ScreenXY
        {
            set
            {
                value.PictureBox.ImageUpdated += new PictureDisplay3DSlice.ImageUpdatedEvent(PictureBox_ImageUpdatedX);
                mScreenXY = value;
            }
        }

        void PictureBox_ImageUpdatedX()
        {
            ImageZ = (Bitmap)mScreenXY.PictureBox.UnZoomedImage ;
        }
        public ScreenProperties3D ScreenXZ
        {
            set
            {
                value.PictureBox.ImageUpdated += new PictureDisplay3DSlice.ImageUpdatedEvent(PictureBox_ImageUpdatedY);
                mScreenXZ = value;
            }
        }

        void PictureBox_ImageUpdatedY()
        {
            ImageY = (Bitmap)mScreenXZ.PictureBox.UnZoomedImage;
        }
        public ScreenProperties3D ScreenYZ
        {
            set
            {
                value.PictureBox.ImageUpdated += new PictureDisplay3DSlice.ImageUpdatedEvent(PictureBox_ImageUpdatedZ);
                mScreenYZ = value;
            }
        }

        void PictureBox_ImageUpdatedZ()
        {
            ImageX = (Bitmap)mScreenYZ.PictureBox.UnZoomedImage;
        }

        private double XPercent = .5, YPercent = .5, ZPercent = .5;
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
        protected override void OnPaint(PaintEventArgs pea)
        {
            CubeDBuf();
        }
        Point UpperLeft, MidCornerPoint, BottomLeft, uppermostLeft, uppermostRight, BottomRight;
        private void CubeDBuf()
        {
            Graphics g;

            int y = (int)(60d / 170d * (double)this.Height);
            int c = (int)((double)this.Height * 3d / 8d);
            int x = (int)((double)this.Width * 3d / 8d);
            UpperLeft = new Point(10, c);
            MidCornerPoint = new Point(this.Width - x - 10, c);
            BottomLeft = new Point(10, this.Height - 10);

            uppermostLeft = new Point(UpperLeft.X + x, UpperLeft.Y - y);
            uppermostRight = new Point(MidCornerPoint.X + x, MidCornerPoint.Y - y);
            BottomRight = new Point(MidCornerPoint.X, BottomLeft.Y);

            Point SideRight = new Point(uppermostRight.X, BottomRight.Y - (MidCornerPoint.Y - uppermostRight.Y));

            int xxx =-1* (int)((UpperLeft.X - uppermostLeft.X) * ZPercent);
            int yyy =-1* (int)((UpperLeft.Y - uppermostLeft.Y) * ZPercent);
            Point[] p3Fro = { new Point( UpperLeft.X +xxx ,UpperLeft.Y +yyy ),
                              new Point (   MidCornerPoint.X+xxx ,MidCornerPoint.Y+yyy) ,
                              new Point(  BottomLeft.X+xxx,BottomLeft.Y+yyy) };

            int yy = (int)((BottomLeft.Y - UpperLeft.Y) * YPercent);
            Point[] p3Top = { new Point( uppermostLeft.X, uppermostLeft.Y+yy),
                              new Point(   uppermostRight.X, uppermostRight.Y+yy) ,
                              new Point( UpperLeft.X, UpperLeft.Y + yy ) };

            int xx = (int)((MidCornerPoint.X - UpperLeft.X) * (1-XPercent));
            Point[] p3Rig = { new Point(MidCornerPoint.X -xx,MidCornerPoint.Y )    ,
                              new Point(  uppermostRight.X -xx,uppermostRight.Y),
                              new Point( BottomRight.X-xx,BottomRight.Y) };


            xxx = -1 * (int)((UpperLeft.X - uppermostLeft.X) *(1- ZPercent));
            yyy = -1 * (int)((UpperLeft.Y - uppermostLeft.Y) *(1- ZPercent));
            Point[] p3RigF = { new Point(MidCornerPoint.X -xx,MidCornerPoint.Y )    ,
                              new Point(  uppermostRight.X -xx-xxx,uppermostRight.Y-yyy  ),
                              new Point( BottomRight.X-xx,BottomRight.Y) };


            Point[] p3TopHalfL = { new Point( uppermostLeft.X-xxx, uppermostLeft.Y+yy-yyy ),
                              new Point(   uppermostRight.X-xxx-xx, uppermostRight.Y+yy-yyy) ,
                              new Point( UpperLeft.X, UpperLeft.Y + yy ) };

            xx = (int)((MidCornerPoint.X - UpperLeft.X) * ( XPercent));
            Point[] p3TopHalfR = { new Point( uppermostLeft.X-xxx+xx, uppermostLeft.Y+yy-yyy ),
                              new Point(   uppermostRight.X-xxx, uppermostRight.Y+yy-yyy) ,
                              new Point( UpperLeft.X+xx, UpperLeft.Y + yy ) };


            /* Point[] p3Fro = { UpperLeft, MidCornerPoint, BottomLeft };
             Point[] p3Top = { uppermostLeft, uppermostRight, UpperLeft };
             Point[] p3Rig = { MidCornerPoint, uppermostRight, BottomRight };*/


            Bitmap bm = new Bitmap(MidCornerPoint.X + x + 10, BottomLeft.Y + y + 10);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);

            //            g.DrawEllipse(Pens.Red,new Rectangle(SideRight.X -3,SideRight.Y -3,6,6));

            
            if (mImageY != null)
            {
                Image im3 = mImageY;
                g.DrawImage(im3, p3Top);
            }
            if (mImageZ != null)
            {
                Image im2 = mImageZ;
                g.DrawImage(im2, p3Rig);
            }

            if (mImageX != null)
            {
                Image im1 = mImageX;
                g.DrawImage(im1, p3Fro);
            }

            if (mImageY != null)
            {
                Image im3 = mImageY;
                g.DrawImage(im3, p3TopHalfL, new Rectangle(0, (int)(im3.Height *(1- ZPercent)),
                    (int)(im3.Width * XPercent), (int)(im3.Height * (ZPercent))), GraphicsUnit.Pixel);
            }

            if (mImageZ != null)
            {
                Image im2 = mImageZ;
                g.DrawImage(im2, p3RigF, new Rectangle(0,0,(int)(im2.Width*ZPercent),im2.Height ),GraphicsUnit.Pixel );
            }

            if (mImageY != null)
            {
                Image im3 = mImageY;
                g.DrawImage(im3, p3TopHalfR,
                    new Rectangle((int)(im3.Width * XPercent), 
                        (int)(im3.Height *(1- ZPercent)),
                    (int)(im3.Width * (1-XPercent)), 
                    (int)(im3.Height * ( ZPercent))),
                    GraphicsUnit.Pixel);
            }

            int X = (int)((MidCornerPoint.X - UpperLeft.X) * XPercent + UpperLeft.X);
            g.DrawLine(new Pen(Color.Green ,4), new Point(X, UpperLeft.Y), new Point(X, BottomLeft.Y));

            double y1 = (BottomLeft.Y - MidCornerPoint.Y) * YPercent + MidCornerPoint.Y;
            double minY = BottomLeft.Y - (MidCornerPoint.Y - uppermostRight.Y);
            double y2 = (minY - uppermostLeft.Y) * YPercent + uppermostLeft.Y;
            g.DrawLine(new Pen(Color.Blue,4), new Point(MidCornerPoint.X, (int)y1), new Point(uppermostRight.X, (int)y2));


            double yZ = (1 - ZPercent) * (UpperLeft.Y - uppermostLeft.Y) + uppermostLeft.Y;
            double x1 = (UpperLeft.X - uppermostLeft.X) * (1 - ZPercent) + uppermostLeft.X;
            double x2 = (MidCornerPoint.X - uppermostRight.X) * (1 - ZPercent) + uppermostRight.X;
            g.DrawLine(new Pen(Color.Yellow,4), new Point((int)x1, (int)yZ), new Point((int)x2, (int)yZ));

            g = Graphics.FromHwnd(this.Handle);

            g.DrawImage(bm, 1, 1);

            g.Dispose();
        }
        private int ActiveAxis = 0;
        private void PseudoCube_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //check if mouse is in front panel
                if (e.X > UpperLeft.X && e.X < MidCornerPoint.X)
                {
                    if (e.Y > UpperLeft.Y && e.Y < BottomLeft.Y)
                    {
                        XPercent = ((double)e.X - UpperLeft.X) / (double)(MidCornerPoint.X - UpperLeft.X);
                        if (XAxisMoved != null)
                            XAxisMoved(XPercent);
                        this.Invalidate();
                        ActiveAxis = 1;
                        return;
                    }
                }
                //check if mouse is in right panel
                if (e.X > MidCornerPoint.X && e.X < uppermostRight.X)
                {
                    double x = e.X - MidCornerPoint.X;
                    double u1 = 1 - x / (double)(uppermostRight.X - MidCornerPoint.X);

                    double y1 = (MidCornerPoint.Y - uppermostRight.Y) * u1 + uppermostRight.Y;
                    double minY = BottomRight.Y - (MidCornerPoint.Y - uppermostRight.Y);
                    double y2 = (MidCornerPoint.Y - uppermostRight.Y) * u1 + minY;
                    //  Test1 = new Point(e.X,(int) y1);
                    //  Test2 = new Point(e.X, (int)y2);
                    if (e.Y > y1 && e.Y < y2)
                    {
                        YPercent = (e.Y - y1) / (y2 - y1);
                        if (YAxisMoved != null)
                            YAxisMoved(YPercent);
                        this.Invalidate();
                        ActiveAxis = 2;
                        return;
                    }
                }

                //check if mouse is in top panel
                if (e.Y < MidCornerPoint.Y && e.Y > uppermostLeft.Y)
                {
                    double y = e.Y - uppermostLeft.Y;
                    double u1 = 1 - (y) / (double)(UpperLeft.Y - uppermostLeft.Y);
                    double x1 = (uppermostLeft.X - UpperLeft.X) * u1 + UpperLeft.X;
                    double x2 = (uppermostRight.X - MidCornerPoint.X) * u1 + MidCornerPoint.X;
                    if (e.X > x1 && e.X < x2)
                    {
                        ZPercent = (double)u1;
                        if (ZAxisMoved != null)
                            ZAxisMoved(ZPercent);
                        this.Invalidate();
                        ActiveAxis = 3;
                        return;
                    }
                }

            }
        }
        public delegate void XAxisMoviedEvent(double Percent);
        public delegate void YAxisMoviedEvent(double Percent);
        public delegate void ZAxisMoviedEvent(double Percent);
        public event YAxisMoviedEvent YAxisMoved;
        public event XAxisMoviedEvent XAxisMoved;
        public event ZAxisMoviedEvent ZAxisMoved;
        private void PseudoCube_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    //check if mouse is in front panel
                    //if (e.X > UpperLeft.X && e.X < MidCornerPoint.X && ActiveAxis == 1)
                    if (ActiveAxis == 1)
                    {
                        double dXPercent = ((double)e.X - UpperLeft.X) / (double)(MidCornerPoint.X - UpperLeft.X);
                        if (dXPercent >= 0 && dXPercent <= 1)
                        {
                            XPercent = ((double)e.X - UpperLeft.X) / (double)(MidCornerPoint.X - UpperLeft.X);
                            if (XAxisMoved != null)
                                XAxisMoved(XPercent);
                            this.Invalidate();
                            return;
                        }
                    }
                    //check if mouse is in right panel
                    //if (e.X > MidCornerPoint.X && e.X < uppermostRight.X && ActiveAxis == 2)
                    if (ActiveAxis == 2)
                    {
                        double x = e.X - MidCornerPoint.X;
                        double u1 = 1 - x / (double)(uppermostRight.X - MidCornerPoint.X);

                        double y1 = (MidCornerPoint.Y - uppermostRight.Y) * u1 + uppermostRight.Y;
                        double minY = BottomRight.Y - (MidCornerPoint.Y - uppermostRight.Y);
                        double y2 = (MidCornerPoint.Y - uppermostRight.Y) * u1 + minY;
                        //  Test1 = new Point(e.X,(int) y1);
                        //  Test2 = new Point(e.X, (int)y2);
                        double dYPercent = (e.Y - y1) / (y2 - y1);
                        if (dYPercent >= 0 && dYPercent <= 1)
                        {
                            YPercent = (e.Y - y1) / (y2 - y1);
                            if (YAxisMoved != null)
                                YAxisMoved(YPercent);
                            this.Invalidate();
                            return;
                        }
                    }

                    //check if mouse is in top panel
                    //if (e.Y < MidCornerPoint.Y && e.Y > uppermostLeft.Y && ActiveAxis == 3)
                    if (ActiveAxis == 3)
                    {
                        double y = e.Y - uppermostLeft.Y;
                        double u1 = 1 - (y) / (double)(UpperLeft.Y - uppermostLeft.Y);
                        double x1 = (uppermostLeft.X - UpperLeft.X) * u1 + UpperLeft.X;
                        double x2 = (uppermostRight.X - MidCornerPoint.X) * u1 + MidCornerPoint.X;

                        if (u1 >= 0 && u1 <= 1)
                        {
                            ZPercent = (double)u1;
                            if (ZAxisMoved != null)
                                ZAxisMoved(ZPercent);
                            this.Invalidate();
                            return;
                        }
                    }

                }
            }
            catch { }
        }

        private void PseudoCube_MouseUp(object sender, MouseEventArgs e)
        {
            ActiveAxis = 0;
        }
    }
}
