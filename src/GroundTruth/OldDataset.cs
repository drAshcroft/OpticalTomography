using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MathHelpLib;

namespace GroundTruthOld
{
    [Serializable ]
    public struct Dataset
    {
        public string Filename;

        public string User;

        public int Num_3DContours;

        public List <Contour3D> Contours;

        public int Width, Depth, Height;

        public Rectangle ViewBox;
        public Rectangle ScreenBox;
        public bool Zooming;
       
        public Dataset(string Filename, string user)
        {
            this.Filename = Filename;
            this.User = user;
            this.Contours = new List<Contour3D>();
            Num_3DContours = 0;

            ViewBox = new Rectangle();
            ScreenBox = new Rectangle();
            Width = 0;
            Height = 0;
            Depth = 0;
            Zooming = false;
        }

        public void AddContour(string Contour3DName)
        {
            Contours.Add(new Contour3D(Contour3DName));
            this.Num_3DContours = Contours.Count;
        }
    }

    [Serializable]
    public struct Contour3D
    {
        public string ContourName;
        public int Num_of_Slices;
        public List< SingleSlice> SingleSlice;

        public Contour3D(string name)
        {
            this.ContourName = name;
            this.Num_of_Slices = 0;
            this.SingleSlice = new List<SingleSlice>();
        }
        
        public void Reset()
        {
            this.SingleSlice = new List<SingleSlice>();
        }
        
    }
   

    [Serializable]
    public struct SingleSlice
    {
        public int PlaneAxis;
        public int SliceNumber;
        public List<Point[]> Vertixes;
        
        public SingleSlice(int slicenumber)
        {
            this.SliceNumber = slicenumber;
            this.PlaneAxis = 0;
            this.Vertixes = new List<Point[]>();
        }
    }
}