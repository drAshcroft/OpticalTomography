using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GraphingLib._3DStuff
{
    public partial class PseudoCube : UserControl
    {
        public PseudoCube()
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
                    mImageX = new Bitmap(value);
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
                    mImageY.RotateFlip(RotateFlipType.Rotate90FlipY);
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
                    mImageZ = new Bitmap(value);
                    this.Invalidate();
                }
            }
        }

        private double XPercent=.5, YPercent=.5, ZPercent=.5;
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
        protected override void OnPaint(PaintEventArgs pea)
        {
            DrawCubePlanes();
        }
        Point A, B, C, a, b, Z;
        private void DrawCubePlanes()
        {
            Graphics g;

            int y = (int)(60d / 170d * (double)this.Height);
            int c = (int)((double)this.Height * 3d / 8d);
            int x = (int)((double)this.Width * 3d / 8d);
            A = new Point(10, c);
            B = new Point(this.Width - x - 10, c);
            C = new Point(10, this.Height - 10);

            a = new Point(A.X + x, A.Y - y);
            b = new Point(B.X + x, B.Y - y);
            Z = new Point(B.X, C.Y);

            Point[] p3Fro = { A, B, C };
            Point[] p3Top = { a, b, A };
            Point[] p3Rig = { B, b, Z };

            Bitmap bm = new Bitmap(B.X + x+10, C.Y + y+10);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            if (mImageX != null)
            {
                Image im1 = mImageX;
                g.DrawImage(im1, p3Fro);
            }

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
            int X = (int)((B.X - A.X) * XPercent + A.X);
            g.DrawLine(Pens.Green, new Point(X, A.Y), new Point(X, C.Y));
            {
                double y1 = (C.Y - B.Y) * YPercent + B.Y;
                double minY = C.Y - (B.Y - b.Y);
                double y2 = (minY - a.Y) * YPercent + a.Y;
                g.DrawLine(Pens.Blue, new Point(B.X, (int)y1), new Point(b.X, (int)y2));
            }
            {
                
             //   g.DrawLine(Pens.Purple , Test1 , Test2);
                
            }


            double yZ = (1-ZPercent) * (A.Y - a.Y)+a.Y ;
            double x1 = (A.X - a.X) * (1-ZPercent) + a.X;
            double x2 = (B.X - b.X) * (1-ZPercent) + b.X;
            g.DrawLine(Pens.Yellow, new Point((int)x1, (int)yZ), new Point((int)x2, (int)yZ));

            g = Graphics.FromHwnd(this.Handle);

            g.DrawImage(bm, 1, 1);

            g.Dispose();
        }
        private int ActiveAxis = 0;
        private void PseudoCube_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //check if mouse is in front panel
                if (e.X > A.X && e.X < B.X)
                {
                    if (e.Y > A.Y && e.Y < C.Y)
                    {
                        XPercent = ((double)e.X - A.X) / (double)(B.X - A.X);
                        if (XAxisMoved != null)
                            XAxisMoved(XPercent);
                        this.Invalidate();
                        ActiveAxis = 1;
                        return;
                    }
                }
                //check if mouse is in right panel
                if (e.X > B.X && e.X < b.X)
                {
                    double x = e.X - B.X;
                    double u1 = 1 - x / (double)(b.X - B.X);

                    double y1 = (B.Y - b.Y) * u1 + b.Y;
                    double minY = Z.Y - (B.Y - b.Y);
                    double y2 = (B.Y - b.Y) * u1 + minY;
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
                if (e.Y < B.Y && e.Y > a.Y)
                {
                    double y = e.Y - a.Y;
                    double u1 = 1 - (y) / (double)(A.Y - a.Y);
                    double x1 = (a.X - A.X) * u1 + A.X;
                    double x2 = (b.X - B.X) * u1 + B.X;
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
            if (e.Button == MouseButtons.Right)
            {
                //check if mouse is in front panel
                if (e.X > A.X && e.X < B.X && ActiveAxis==1)
                {
                    if (e.Y > A.Y && e.Y < C.Y)
                    {
                        XPercent = ((double)e.X-A.X) / (double)(B.X - A.X);
                        if (XAxisMoved != null)
                            XAxisMoved(XPercent );
                        this.Invalidate();
                        return;
                    }
                }
                //check if mouse is in right panel
                if (e.X > B.X && e.X < b.X && ActiveAxis==2)
                {
                    double x = e.X - B.X;
                    double u1 =1- x / (double)(b.X - B.X);

                    double y1 = (B.Y - b.Y) * u1  + b.Y;
                    double minY = Z.Y - (B.Y - b.Y);
                    double y2 = (B.Y - b.Y) * u1 + minY;
                  //  Test1 = new Point(e.X,(int) y1);
                  //  Test2 = new Point(e.X, (int)y2);
                    if (e.Y > y1 && e.Y < y2)
                    {
                        YPercent = (e.Y-y1)/(y2-y1) ;
                        if (YAxisMoved != null)
                            YAxisMoved(YPercent );
                        this.Invalidate();
                        return;
                    }
                }

                //check if mouse is in top panel
                if (e.Y < B.Y && e.Y > a.Y && ActiveAxis==3)
                {
                    double y = e.Y - a.Y;
                    double u1 = 1-(y) / (double)(A.Y - a.Y);
                    double x1 = (a.X - A.X) * u1 + A.X;
                    double x2= (b.X - B.X ) * u1 + B.X ;
                    if (e.X > x1 && e.X < x2)
                    {
                        ZPercent = (double)u1;
                        if (ZAxisMoved != null)
                            ZAxisMoved( ZPercent );
                        this.Invalidate();
                        return;
                    }
                }

            }
        }

        private void PseudoCube_MouseUp(object sender, MouseEventArgs e)
        {

        }
    }
}
