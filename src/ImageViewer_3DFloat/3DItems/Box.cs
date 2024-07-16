using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageViewer3D._3DItems
{
    public class Box
    {
        public int X, Y, Z;
        public int Width, Height, Depth;

        public int Front
        {
            get
            { return Z; }
        }
        public int Back
        {
            get
            {
                return Z + Depth;
            }
        }

        public int Left
        {
            get { return X; }
        }
        public int Right
        {
            get { return X + Width; }
        }

        public int Top
        {
            get { return Y; }
        }
        public int Bottom
        {
            get { return Y + Height; }
        }

        public Rectangle AxisBounds(int Axis)
        {
            if (Axis == 0)
            {
                return new Rectangle(X, Y, Width, Height);
            }
            if (Axis == 1)
            {
                return new Rectangle(Z, Y, Depth, Height);
            }
            if (Axis == 2)
            {
                return new Rectangle(X, Z, Width, Depth);
            }
            return new Rectangle(0,0,0,0);
        }

        public void SetAxisBounds(int Axis, Rectangle Bounds)
        {
            if (Axis == 0)
            {
                X = Bounds.X;
                Y = Bounds.Y;
                Z = Z + Depth / 2;
                Width =Bounds.Width ;
                Height = Bounds.Height;
                Depth =(int)( (Bounds.Width + Bounds.Height) / 2d);
                Z = Z - Depth / 2;
            }
            if (Axis == 1)
            {
                X = X + Width / 2;
                Z = Bounds.X;
                Y = Bounds.Y;
                Width = (int)((Bounds.Width + Bounds.Height) / 2d);
                Depth = Bounds.Width;
                Height = Bounds.Height;
                X = X - Width / 2;
            }
            if (Axis == 2)
            {
                X = Bounds.X;
                Z = Bounds.Y;
                Y = Y + Height / 2;
                Width = Bounds.Width;
                Height = (int)((Bounds.Width + Bounds.Height) / 2d);
                Depth = Bounds.Height;
                Y = Y - Height / 2;
            }
        }
    }
}
