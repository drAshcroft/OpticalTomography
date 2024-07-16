using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MathHelpLib;

namespace GroundTruth
{


    [Serializable ]
    public struct Dataset
    {
        public string Filename;
        public string User;
        public int Num_3DSurfaces;
        public int Width, Depth, Height;

        public List <Surface3D> Surfaces;

        public Rectangle ViewBox;
        public Rectangle ScreenBox;
        public bool Zooming;
       
        public Dataset(string Filename, string user)
        {
            this.Filename = Filename;
            this.User = user;
            this.Surfaces = new List<Surface3D>();
            Num_3DSurfaces = 0;

            ViewBox = new Rectangle();
            ScreenBox = new Rectangle();
            Width = 0;
            Height = 0;
            Depth = 0;
            Zooming = false;
        }

        public void AddSurface(string Contour3DName)
        {
            Surfaces.Add(new Surface3D(Contour3DName));
            this.Num_3DSurfaces = Surfaces.Count;
        }
    }

    [Serializable]
    public struct Surface3D
    {
        public string SurfaceName;
        public int Num_of_Slices;
        public List<SurfaceSlice> Slices;

        public Surface3D(string name)
        {
            this.SurfaceName = name;
            this.Num_of_Slices = 0;
            this.Slices = new List<SurfaceSlice>();
        }
        
        public void Reset(int nSlices)
        {
            Num_of_Slices = nSlices;
            for (int i = 0; i < nSlices; i++)
                this.Slices.Add(new SurfaceSlice(i));
        }
        
    }

    [Serializable]
    public struct SurfaceSlice
    {
        public int PlaneAxis;
        public int SliceNumber;
        public List<PointF[]> Contours;
        
        public SurfaceSlice(int slicenumber)
        {
            this.SliceNumber = slicenumber;
            this.PlaneAxis = 0;
            this.Contours = new List<PointF[]>();
        }
        
    }
}