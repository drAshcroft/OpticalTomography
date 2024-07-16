using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathHelpLib;
using MathHelpLib._3DStuff;


namespace GroundTruth
{
    public class TriangleProxy
    {
        Point3D[] Points;
        int[] Indexs;
        int StartIndex;
        public bool TriangleFixed=false;
        int[] Indexs3;

        public Point3D this[int index]
        {
            get { return Points[Indexs[index + StartIndex]]; }
        }

        public TriangleProxy(ref Point3D[] Points,ref int[] Indexs, int StartIndex)
        {
            this.Points = Points;
            this.Indexs = Indexs;
            this.StartIndex = StartIndex;
            this.Indexs3 = new int[3];
            Indexs3[0] = Indexs[StartIndex];
            Indexs3[1] = Indexs[StartIndex + 1];
            Indexs3[2] = Indexs[StartIndex + 2];

        }

        public bool ContainsIndex(int Index)
        {
            if (Index == Indexs[StartIndex])
                return true;
            if (Index == Indexs[StartIndex+1])
                return true;
            if (Index == Indexs[StartIndex+2])
                return true;
            //when you do "return" does it automatically do break?
            return false;
        }


        public void FlipNormal()
        {
            int t = Indexs[StartIndex];
            Indexs[StartIndex] = Indexs[StartIndex + 1];
            Indexs[StartIndex + 1]=t;
        }
        public static void FixTriangles(List<TriangleProxy> Triangles)
        {
            Triangles[0].TriangleFixed = true;
            FixTrianglesStep2(Triangles, 0);

            for (int i = 0; i < Triangles.Count; i++)
            {
                if (Triangles[i].TriangleFixed == false)
                {
                    Triangles[i].TriangleFixed = true;
                    FixTrianglesStep2(Triangles, i);
                }
            }
        
        }

        public static void FixTrianglesStep2(List<TriangleProxy> Triangles, int TriangleIndex)
        {
            
            //Point3D a = Triangles[TriangleIndex].Points[0];
            //Point3D b = Triangles[TriangleIndex].Points[1];
            //Point3D c = Triangles[TriangleIndex].Points[2];
            int[] Indexes = new int[3];
            Indexes = Triangles[TriangleIndex].Indexs3;
            List <int> NextStdTriangles = new List<int>();


            for (int i = 0; i < Triangles.Count; i++)
            {

                if (Triangles[i].TriangleFixed == false)
                {
                    if (Triangles[i].ContainsIndex(Indexes[0]) ==true && Triangles[i].ContainsIndex(Indexes[1]) == true)
                    {
                        int index0 = Triangles[i].Indexs[0];
                        int index1 = Triangles[i].Indexs[1];
                        int index2 = Triangles[i].Indexs[2];

                        Triangles[i].Indexs[0] = Indexes[1];
                        Triangles[i].Indexs[1] = Indexes[0];

                        if (index0 != Indexes[0] && index0 != Indexes[1])
                            Triangles[i].Indexs[2] = index0;

                        if (index1 != Indexes[0] && index1 != Indexes[1])
                            Triangles[i].Indexs[2] = index1;

                        if (index2 != Indexes[0] && index2 != Indexes[1])
                            Triangles[i].Indexs[2] = index2;
                        
                        Triangles[i].TriangleFixed = true;

                        NextStdTriangles.Add(i);


                    }
                    else if (Triangles[i].ContainsIndex(Indexes[0]) == true && Triangles[i].ContainsIndex(Indexes[2]) == true)
                    {

                        int index0 = Triangles[i].Indexs[0];
                        int index1 = Triangles[i].Indexs[1];
                        int index2 = Triangles[i].Indexs[2];

                        Triangles[i].Indexs[0] = Indexes[0];
                        Triangles[i].Indexs[1] = Indexes[2];

                        if (index0 != Indexes[0] && index0 != Indexes[2])
                            Triangles[i].Indexs[2] = index0;

                        if (index1 != Indexes[0] && index1 != Indexes[2])
                            Triangles[i].Indexs[2] = index1;

                        if (index2 != Indexes[0] && index2 != Indexes[2])
                            Triangles[i].Indexs[2] = index2;

                        Triangles[i].TriangleFixed = true;

                        NextStdTriangles.Add(i);
                    }
                    else if (Triangles[i].ContainsIndex(Indexes[2]) == true && Triangles[i].ContainsIndex(Indexes[1]) == true)
                    {
                        int index0 = Triangles[i].Indexs[0];
                        int index1 = Triangles[i].Indexs[1];
                        int index2 = Triangles[i].Indexs[2];

                        Triangles[i].Indexs[0] = Indexes[1];
                        Triangles[i].Indexs[1] = Indexes[2];

                        if (index0 != Indexes[2] && index0 != Indexes[1])
                            Triangles[i].Indexs[2] = index0;

                        if (index1 != Indexes[2] && index1 != Indexes[1])
                            Triangles[i].Indexs[2] = index1;

                        if (index2 != Indexes[2] && index2 != Indexes[1])
                            Triangles[i].Indexs[2] = index2;

                        Triangles[i].TriangleFixed = true;

                        NextStdTriangles.Add(i);
                    
                    }
                    

                }
            }

            for (int i = 0; i < NextStdTriangles.Count; i++)
                FixTrianglesStep3(Triangles, NextStdTriangles[i]);


           
        
        }

        public static void FixTrianglesStep3(List<TriangleProxy> Triangles, int TriangleIndex)
        {

            //Point3D a = Triangles[TriangleIndex].Points[0];
            //Point3D b = Triangles[TriangleIndex].Points[1];
            //Point3D c = Triangles[TriangleIndex].Points[2];
            int[] Indexes = new int[3];
            Indexes = Triangles[TriangleIndex].Indexs3;
            List<int> NextStdTriangles = new List<int>();


            for (int i = 0; i < Triangles.Count; i++)
            {

                if (Triangles[i].TriangleFixed == false)
                {
                    if (Triangles[i].ContainsIndex(Indexes[0]) == true && Triangles[i].ContainsIndex(Indexes[1]) == true)
                    {
                        int index0 = Triangles[i].Indexs[0];
                        int index1 = Triangles[i].Indexs[1];
                        int index2 = Triangles[i].Indexs[2];

                        Triangles[i].Indexs[0] = Indexes[1];
                        Triangles[i].Indexs[1] = Indexes[0];

                        if (index0 != Indexes[0] && index0 != Indexes[1])
                            Triangles[i].Indexs[2] = index0;

                        if (index1 != Indexes[0] && index1 != Indexes[1])
                            Triangles[i].Indexs[2] = index1;

                        if (index2 != Indexes[0] && index2 != Indexes[1])
                            Triangles[i].Indexs[2] = index2;

                        Triangles[i].TriangleFixed = true;

                        NextStdTriangles.Add(i);


                    }
                    else if (Triangles[i].ContainsIndex(Indexes[0]) == true && Triangles[i].ContainsIndex(Indexes[2]) == true)
                    {

                        int index0 = Triangles[i].Indexs[0];
                        int index1 = Triangles[i].Indexs[1];
                        int index2 = Triangles[i].Indexs[2];

                        Triangles[i].Indexs[0] = Indexes[0];
                        Triangles[i].Indexs[1] = Indexes[2];

                        if (index0 != Indexes[0] && index0 != Indexes[2])
                            Triangles[i].Indexs[2] = index0;

                        if (index1 != Indexes[0] && index1 != Indexes[2])
                            Triangles[i].Indexs[2] = index1;

                        if (index2 != Indexes[0] && index2 != Indexes[2])
                            Triangles[i].Indexs[2] = index2;

                        Triangles[i].TriangleFixed = true;

                        NextStdTriangles.Add(i);
                    }
                    else if (Triangles[i].ContainsIndex(Indexes[2]) == true && Triangles[i].ContainsIndex(Indexes[1]) == true)
                    {
                        int index0 = Triangles[i].Indexs[0];
                        int index1 = Triangles[i].Indexs[1];
                        int index2 = Triangles[i].Indexs[2];

                        Triangles[i].Indexs[0] = Indexes[1];
                        Triangles[i].Indexs[1] = Indexes[2];

                        if (index0 != Indexes[2] && index0 != Indexes[1])
                            Triangles[i].Indexs[2] = index0;

                        if (index1 != Indexes[2] && index1 != Indexes[1])
                            Triangles[i].Indexs[2] = index1;

                        if (index2 != Indexes[2] && index2 != Indexes[1])
                            Triangles[i].Indexs[2] = index2;

                        Triangles[i].TriangleFixed = true;

                        NextStdTriangles.Add(i);

                    }


                }
            }

            for (int i = 0; i < NextStdTriangles.Count; i++)
                FixTrianglesStep2(Triangles, NextStdTriangles[i]);

        }

        public static double TriangleArea(Point3D p1, Point3D p2, Point3D p3)
        {
            return (((p1 - p2).CrossProduct(p1 - p3)).Abs()) / 2;

        }
        public static double SurfaceArea(MarchingCubes Cell)
        {

            double SurfaceArea = 0;



            for (int i = 0; i < Cell.TriangleIndexs.Length - 2; i = i + 3)
            {
                int a = Cell.TriangleIndexs[i];
                int b = Cell.TriangleIndexs[i + 1];
                int c = Cell.TriangleIndexs[i + 2];

                SurfaceArea += TriangleArea(Cell.VertexList[a], Cell.VertexList[b], Cell.VertexList[c]);

            }

            return SurfaceArea;

        }
        public double VolumeWithOrigen(MarchingCubes Cell)
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
