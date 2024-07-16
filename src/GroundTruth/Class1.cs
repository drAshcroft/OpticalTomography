using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathHelpLib;
using MathHelpLib._3DStuff;



namespace GroundTruth
{

    
   public static class Class1
    {
        public static void Compacting(Surface3D Contour, int size1, int size2)
        {
            List<SurfaceSlice> SlicesWithContours = new List<SurfaceSlice>();

            for (int i = 0; i < Contour.Slices.Count(); i++)
            {
                if (Contour.Slices[i].Contours != null)
                    SlicesWithContours.Add(Contour.Slices[i]);
            }

            int x;
            int y;
            double[, ,] Matrix = new double[SlicesWithContours.Count, size1, size2];

            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < SlicesWithContours.Count; j++)
                {
                    for (int k = 0; k < SlicesWithContours[j].Contours.Count; k++)
                    {
                        for (int l = 0; l < SlicesWithContours[j].Contours[k].Count(); l++)
                        {
                            x =(int) SlicesWithContours[j].Contours[k][l].X;
                            y =(int) SlicesWithContours[j].Contours[k][l].Y;
                            Matrix[i, x, y] = 255;
                            
                        }



                    }

                }

            }

        }
        public static void Filling(double[, ,] Matrix)
        {


            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    int control = 1;
                    for (int k = 0; k < Matrix.GetLength(2); k++)
                    {
                        if (Matrix[i, j, k] == 255)
                        {
                            control *= (-1);
                        }
                        if (control == -1)
                        {
                            Matrix[i, j, k] = 255;
                        }

                    }
                }
            }

        }

        public static double TriangleArea( Point3D p1, Point3D p2, Point3D p3)
    {
        return  (((p1 - p2).CrossProduct(p1 - p3)).Abs())/2;
 
    }
        public static double SurfaceArea (MarchingCubes Cell)
        {

            double SurfaceArea = 0;

            

            for (int i = 0; i < Cell.TriangleIndexs.Length -2; i=i+3)
            {
                int a = Cell.TriangleIndexs[i];
                int b = Cell.TriangleIndexs[i+1];
                int c = Cell.TriangleIndexs[i+2];

                SurfaceArea += TriangleArea(Cell.VertexList[a], Cell.VertexList[b], Cell.VertexList[c]);
            
            }

            return SurfaceArea;
        
        }

        public static double Volume(MarchingCubes Cell)
        {

            double Volume = 0;
            Point3D CenterOfGravity = new Point3D();


            for (int i = 0; i < Cell.VertexList.Length; i++)
            {
                CenterOfGravity.X += Cell.VertexList[i].X;
                CenterOfGravity.Y += Cell.VertexList[i].Y;
                CenterOfGravity.Z += Cell.VertexList[i].Z;

            }

            CenterOfGravity.X /= Cell.VertexList.Length;
            CenterOfGravity.Y /= Cell.VertexList.Length;
            CenterOfGravity.Z /= Cell.VertexList.Length;

            for (int i = 0; i < Cell.TriangleIndexs.Length - 2; i = i + 3)
            {

                int a = Cell.TriangleIndexs[i];
                int b = Cell.TriangleIndexs[i + 1];
                int c = Cell.TriangleIndexs[i + 2];

                    double triangle = TriangleArea(Cell.VertexList[a], Cell.VertexList[b], Cell.VertexList[c]);
                    Point3D CenterOfGravityTriangle = new Point3D();
                    CenterOfGravityTriangle.X = (Cell.VertexList[a].X + Cell.VertexList[b].X + Cell.VertexList[c].X) / 3;
                    CenterOfGravityTriangle.Y = (Cell.VertexList[a].Y + Cell.VertexList[b].Y + Cell.VertexList[c].Y) / 3;
                    CenterOfGravityTriangle.Z = (Cell.VertexList[a].Z + Cell.VertexList[b].Z + Cell.VertexList[c].Z) / 3;
                double height = CenterOfGravity.Distance(CenterOfGravityTriangle);
                    //double height = Math.Sqrt(Math.Pow((CenterOfGravity.X - CenterOfGravityTriangle.X), 2) + Math.Pow((CenterOfGravity.Y - CenterOfGravityTriangle.Y), 2) +
                                                                       // Math.Pow((CenterOfGravity.Z - CenterOfGravityTriangle.Z), 2));
                    double sign = ((Cell.VertexList[b] - Cell.VertexList[a]).CrossProduct(Cell.VertexList[c] - Cell.VertexList[b])).DotProduct(CenterOfGravity);

                    if (sign > 0)
                    {
                        Volume += (triangle * height) / 3;
                    }
                    if (sign < 0)
                    {
                        Volume -= (triangle * height) / 3;
                    }
                
            }
            return Volume;
        }


    }
}
