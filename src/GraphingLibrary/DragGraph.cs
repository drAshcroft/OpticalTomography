using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GraphingLibrary
{

    public partial class DragGraph : UserControl
    {

        public delegate void DragPointAddedEvent(int index, PointF newPoint);
        public delegate void DrawPointMovedEvent(int index, PointF newValue);

        public event DragPointAddedEvent DrawPointAdded;
        public event DrawPointMovedEvent DrawPointMoved;

        public DragGraph()
        {
            InitializeComponent();
        }


        public void InsertControlPoint(PointF point)
        {
            ControlPoints.Add(point);
        }

        public List<PointF> ControlPoints=new List<PointF>();

        int SelectedPoint = -1;

        int MousePoint = -1;
        private void DragGraph_MouseDown(object sender, MouseEventArgs e)
        {
            PointF e2 = new PointF((float)e.X / this.Width,1- (float)e.Y / this.Height);
            float criticwidth = (8f / this.Width);
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                double R2 = Math.Pow(e2.X - ControlPoints[i].X, 2) + Math.Pow(e2.Y - ControlPoints[i].Y, 2);
                if (R2 < criticwidth)
                {
                    MousePoint = i;
                    break;
                }
            }
            if (MousePoint == -1)
            {
                for (int i = 0; i < ControlPoints.Count - 1; i++)
                {
                    if (e2.X > ControlPoints[i].X && e2.X < ControlPoints[i + 1].X)
                    {
                        ControlPoints.Insert(i+1, new PointF(e2.X, e2.Y));
                        if (DrawPointAdded != null)
                            DrawPointAdded(i + 1, new PointF(e2.X, e2.Y));
                        MousePoint = i+1;
                        break;
                    }
                }
            }
            SelectedPoint = MousePoint;
        }

        private void DragGraph_MouseMove(object sender, MouseEventArgs e)
        {
            if (MousePoint != -1)
            {
                PointF P = ControlPoints[MousePoint];
                PointF e2 = new PointF((float)e.X / this.Width,1- (float)e.Y / this.Height);
                P.X = e2.X;
                P.Y = e2.Y;
                if (P.X > 1) P.X = 1;
                if (P.X < 0) P.X = 0;

                if (P.Y > 1) P.Y = 1;
                if (P.Y < 0) P.Y = 0;

                ControlPoints.RemoveAt(MousePoint);
                ControlPoints.Insert(MousePoint, P);
                this.Invalidate();
               
            }
        }

        private void DragGraph_MouseUp(object sender, MouseEventArgs e)
        {
            if (DrawPointMoved != null)
                DrawPointMoved(MousePoint, ControlPoints[MousePoint]);
            MousePoint = -1;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            if (ControlPoints !=null && ControlPoints.Count > 0)
            {
                Point P1 = new Point((int)(this.Width * ControlPoints[0].X), (int)(this.Height *(1- ControlPoints[0].Y)));
                for (int i = 1; i < ControlPoints.Count; i++)
                {
                    int x = (int)(this.Width * ControlPoints[i].X);
                    int y=this.Height - (int)(this.Height *( ControlPoints[i].Y));
                    Point P2 = new Point(x,   y );
                    g.DrawLine(Pens.Black, P1, P2);
                    g.DrawEllipse(Pens.Red, new Rectangle(x-2,y-2, 4, 4));
                    P1 = new Point(P2.X,P2.Y );
                }
            }
        }

    }
}
