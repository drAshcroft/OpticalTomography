using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MathHelpLib;

namespace Tomographic_Imaging_2.VisionTools
{
    public static class LineAngleReporter
    {

        public static VisionHelper.IEdgeFound[][] RegionSeperator(List<VisionHelper.IEdgeFound[]> AllImagesFoundEdges, Rectangle SearchArea)
        {
            int Length = AllImagesFoundEdges.Count;

            VisionHelper.IEdgeFound[][] Edges = new VisionHelper.IEdgeFound[Length][];

            //first work through the list too seperate the lines into their correct quadrants
            for (int fI = 0; fI < Length; fI++)
            {
                VisionHelper.IEdgeFound[] FoundEdges = AllImagesFoundEdges[fI];
                List<VisionHelper.IEdgeFound> GoodEdges1 = new List<VisionHelper.IEdgeFound>();
                foreach (VisionHelper.IEdgeFound ief in FoundEdges)
                {
                    if ((ief.CenterX > SearchArea.X) && (ief.CenterX < (SearchArea.X + SearchArea.Width)))
                        if ((ief.CenterY > SearchArea.Y) && (ief.CenterY < (SearchArea.Y + SearchArea.Height)))
                        {
                            GoodEdges1.Add((VisionHelper.IEdgeFound)ief);
                        }
                }
                Edges[fI] = GoodEdges1.ToArray();
            }

            return Edges;
        }
        public static VisionHelper.IEdgeFound[][][] QuadrantSeperator(List<VisionHelper.IEdgeFound[]> AllImagesFoundEdges, Rectangle SearchArea)
        {
            int Length = AllImagesFoundEdges.Count;

            VisionHelper.IEdgeFound[][][] Edges = new VisionHelper.IEdgeFound[4][][];

            int hWidth = SearchArea.Width / 2;
            int hHeight = SearchArea.Height / 2;
            Rectangle Quad1 = new Rectangle(SearchArea.X, SearchArea.Y, hWidth, hHeight);
            Rectangle Quad2 = new Rectangle(SearchArea.X + hWidth, SearchArea.Y, hWidth, hHeight);
            Rectangle Quad3 = new Rectangle(SearchArea.X, SearchArea.Y + hHeight, hWidth, hHeight);
            Rectangle Quad4 = new Rectangle(SearchArea.X + hWidth, SearchArea.Y + hHeight, hWidth, hHeight);
            Edges[0] = RegionSeperator(AllImagesFoundEdges, Quad1);
            Edges[1] = RegionSeperator(AllImagesFoundEdges, Quad2);
            Edges[2] = RegionSeperator(AllImagesFoundEdges, Quad3);
            Edges[3] = RegionSeperator(AllImagesFoundEdges, Quad4);

            return Edges;
        }
        public static VisionHelper.IEdgeFound[][][] SideSeperator(List<VisionHelper.IEdgeFound[]> AllImagesFoundEdges, Rectangle SearchArea)
        {
            int Length = AllImagesFoundEdges.Count;

            VisionHelper.IEdgeFound[][][] Edges = new VisionHelper.IEdgeFound[4][][];
            int hWidth = SearchArea.Width / 2;
            int hHeight = SearchArea.Height / 2;
            int Width = SearchArea.Width;
            int Height = SearchArea.Height;
            Rectangle Quad1 = new Rectangle(SearchArea.X, SearchArea.Y, hWidth, Height);

            Rectangle Quad2 = new Rectangle(SearchArea.X + hWidth, SearchArea.Y, hWidth, Height);

            Rectangle Quad3 = new Rectangle(SearchArea.X, SearchArea.Y, Width, hHeight);
            Rectangle Quad4 = new Rectangle(SearchArea.X, SearchArea.Y + hHeight, Width, hHeight);
            Edges[0] = RegionSeperator(AllImagesFoundEdges, Quad1);
            Edges[1] = RegionSeperator(AllImagesFoundEdges, Quad2);
            Edges[2] = RegionSeperator(AllImagesFoundEdges, Quad3);
            Edges[3] = RegionSeperator(AllImagesFoundEdges, Quad4);

            return Edges;
        }

        public static double[,] CalculateAngles( VisionHelper.IEdgeFound[][] Edges)
        {
            List<PointD> Angles = new List<PointD>();
            //find an appropriate value for the first step
            double lAngle = 0;
            bool Found;
            for (int i = 0; i < Edges.Length; i++)
            {
                Found = false;
                foreach (VisionHelper.LineFound lf in Edges[i])
                {
                    double angle = Math.Atan2(lf.Y2 - lf.Y1, lf.X2 - lf.X1);
                    if (angle < 0) angle = 2 * Math.PI + angle;
                    if (angle < 3 && Found == false)
                    {
                        lAngle = angle + Math.PI / 2d;
                        Found = true;
                        break;
                    }
                }
                if (Found == true)
                    break;
            }

            //now step through the real data to produce the graph
            for (int i = 0; i < Edges.Length; i++)
            {
                Found = false;
                foreach (VisionHelper.LineFound lf in Edges[i])
                {
                    double angle = Math.Atan2(lf.Y2 - lf.Y1, lf.X2 - lf.X1);
                    if (angle < 0) angle = 2 * Math.PI + angle;
                    if (angle < 3 )
                    {
                        lAngle = angle + Math.PI / 2d;
                        Angles.Add(new PointD(i, lAngle ));
                        Found = true;
                    }
                }
                if (Found == false)
                {
                    Angles.Add(new PointD(i,lAngle ));
                }
            }

            double[,] outArray = new double[2, Angles.Count];
            //now format the data into a graphable form
            for (int i = 0; i < Angles.Count; i++)
            {
                outArray[0, i] = Angles[i].X;
                outArray[1, i] = Angles[i].Y;
            }
            return outArray;
        }

        public static double[,] MakeSingleValued(double[,] array)
        {
            int Length =array.GetLength(1);
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            
            for (int i = 0; i < Length ; i++)
            {
                if (array[0, i] < minX) minX = array[0, i];
                if (array[0, i] > maxX) maxX = array[0, i];
            }
            int[] PointCount = new int[(int)(1+maxX - minX)];
            double[,] OutArray = new double[2,1+(int)( maxX - minX)];

            for (int i = 0; i < Length; i++)
            {
                
                OutArray[1,(int)( array[0,i]-minX)] += array[1, i];
                PointCount[(int)(array[0, i] - minX)]++;
            }
            for (int i = 0; i < OutArray.GetLength(1); i++)
            {
                OutArray[0, i] = i + minX;
                if (PointCount[i]!=0)
                    OutArray[1, i] = OutArray[1, i] / PointCount[i]; 
            }
            return OutArray ;
        }
    }
}
