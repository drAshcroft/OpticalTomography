using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kitware.VTK;
using System.Drawing;
using MathHelpLib.ImageProcessing;
using MathHelpLib;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace MathHelpLib._3DStuff
{
    public class _3DInterpolation
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        static unsafe extern void CopyMemory(ushort* Destination, ushort* Source, uint Length);

        public static float[, ,] GetVolumeFromPolyData(vtkPolyData OriginalData, vtkPolyData newPolydata)
        {
            double[] boundscf = OriginalData.GetBounds();
            double[] boundsPolyData = newPolydata.GetBounds();

            double[] cfCenter = OriginalData.GetCenter();
            double[] polydataCenter = newPolydata.GetCenter();

            double mx = (boundsPolyData[1] - boundsPolyData[0]) / (boundscf[1] - boundscf[0]);
            double bx = boundsPolyData[0] - boundscf[0] * mx;

            double my = (boundsPolyData[3] - boundsPolyData[2]) / (boundscf[3] - boundscf[2]);
            double by = boundsPolyData[2] - boundscf[2] * my;

            double mz = (boundsPolyData[5] - boundsPolyData[4]) / (boundscf[5] - boundscf[4]);
            double bz = boundsPolyData[4] - boundscf[4] * mz;

            double startm = (boundsPolyData[4] - bz) / mz;
            double finishm = (boundsPolyData[5] - bz) / mz;

            double stepsizem = Math.Abs((boundscf[5] - boundscf[4]) / (boundsPolyData[5] - boundsPolyData[4]));
            double bla = (finishm - startm) / stepsizem + 1;

            float[, ,] volume = new float[100, 100, 500];
            Bitmap b = new Bitmap(100, 100, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.Black);
            int s = 0;
            for (double m = startm; m < finishm; m += stepsizem)
            {
                vtkPlane plane = vtkPlane.New();
                double[] origin = newPolydata.GetBounds();

                plane.SetOrigin(boundscf[0], boundscf[2], m);
                plane.SetNormal(0, 0, 1);

                // Create cutter
                vtkCutter cutter = vtkCutter.New();
                cutter.SetCutFunction(plane);
                cutter.SetInput(OriginalData);

                cutter.Update();

                vtkPolyDataMapper cutterMapper = vtkPolyDataMapper.New();
                cutterMapper.SetInputConnection(cutter.GetOutputPort());
                g.Clear(Color.Black);
                long nCell = cutter.GetOutput().GetNumberOfCells();
                for (int i = 0; i < nCell; i++)
                {
                    vtkPoints pts = cutter.GetOutput().GetCell(i).GetPoints();
                    double[] point1 = pts.GetPoint(0);
                    double[] point2 = pts.GetPoint(1);
                    point1[0] = point1[0] * mx + bx;
                    point1[1] = point1[1] * my + by;

                    point2[0] = point2[0] * mx + bx;
                    point2[1] = point2[1] * my + by;

                    g.DrawLine(Pens.White, new Point((int)point1[0], (int)point1[1]), new Point((int)point2[0], (int)point2[1]));

                }
                // b.Save(@"c:\temp\test\test" + s.ToString() + ".bmp");

                // MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray
                float[,] image = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToFloatArray(b, false);
                float[,] image2 = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToFloatArray(b, false);

                for (int j = 0; j < image.GetLength(0); j++)
                {
                    int control = 1;
                    for (int k = 0; k < image.GetLength(1); k++)
                    {
                        if (image[j, k] == 255)
                        {
                            control *= (-1);
                            while (k < image.GetLength(1) - 1 && image[j, k + 1] == 255) k++;

                        }
                        if (control == -1)
                        {
                            image[j, k] = 255;
                        }
                    }
                }

                //Bitmap first = image.MakeBitmap();

                //first.Save(@"c:\temp\first.bmp");

                for (int j = 0; j < image2.GetLength(1); j++)
                {
                    int control = 1;
                    for (int k = 0; k < image2.GetLength(0); k++)
                    {
                        if (image2[k, j] == 255)
                        {
                            control *= (-1);
                            while (k < image2.GetLength(1) - 1 && image2[k + 1, j] == 255) k++;

                        }
                        if (control == -1)
                        {
                            image2[k, j] = 255;

                        }

                    }
                }
                // Bitmap second = image2.MakeBitmap();

                // second.Save(@"c:\temp\second.bmp");

                for (int j = 0; j < image.GetLength(0); j++)
                {

                    for (int k = 0; k < image.GetLength(1); k++)
                    {
                        if (image[j, k] != image2[j, k])
                        {

                            image[j, k] = 0;
                        }

                    }
                }
                //  Bitmap c = image.MakeBitmap();
                //  c.Save(@"c:\temp\Gaussian\bla" + s.ToString() + ".bmp");
                for (int j = 0; j < image.GetLength(0); j++)
                {

                    for (int k = 0; k < image.GetLength(1); k++)
                    {
                        volume[j, k, s] = image[j, k];
                    }
                }

                s++;
            }
            //b.Save(@"c:\temp\test2.bmp");



            double area = 0;
            for (int i = 0; i < volume.GetLength(0); i++)
            {
                for (int j = 0; j < volume.GetLength(1); j++)
                {
                    for (int k = 0; k < volume.GetLength(2); k++)
                    {
                        if (volume[i, j, k] == 255)
                            area++;

                    }
                }
            }

            return volume;
        }

        public static void GetVTKVolume(vtkPolyData polydata, out double Volume, out double SurfaceArea)
        {
            //polydata = surface.GetOutput();
            var p = polydata.GetPoints();
            for (int i = 0; i < p.GetNumberOfPoints(); i++)
            {
                double[] pD = p.GetPoint(i);
                // System.Diagnostics.Debug.Print(pD[0] + ", " + pD[1] + ", " + pD[2]);
            }


            for (int i = 0; i < polydata.GetNumberOfCells(); i++)
            {
                vtkIdList ids = vtkIdList.New();
                polydata.GetCellPoints(i, ids);
                for (int j = 0; j < ids.GetNumberOfIds(); j++)
                {
                    //  System.Diagnostics.Debug.Write(ids.GetId(j).ToString() + " ");
                }
                // System.Diagnostics.Debug.Print("");
            }

            vtkTriangleFilter triangle = vtkTriangleFilter.New();

            triangle.SetInput(polydata);

            triangle.Update();

            vtkMassProperties massProperty = vtkMassProperties.New();

            massProperty.SetInput(triangle.GetOutput());

            massProperty.Update();

            Volume = massProperty.GetVolume();
            SurfaceArea = massProperty.GetSurfaceArea();
        }


        private static vtkPolyData RotatePolyMeshAroundCenter(vtkPolyData original, double Angle, double X, double Y, double Z)
        {
            //get information about the original
            double[] bounds;
            vtkPolyData contours = original;
            bounds = contours.GetBounds();
            double[] center;
            center = contours.GetCenter();



            vtkTransformPolyDataFilter transformFilter = vtkTransformPolyDataFilter.New();
            transformFilter.SetInput(original);
            vtkTransform transform = vtkTransform.New();
            transformFilter.SetTransform(transform);
            transform.Translate(-center[0], -center[1], -center[2]);
            transformFilter.Update();


            vtkTransform transform2 = vtkTransform.New();
            //transform->RotateWXYZ(double angle, double x, double y, double z);
            transform2.RotateWXYZ(Angle, X, Y, Z);

            vtkTransformPolyDataFilter transformFilter2 = vtkTransformPolyDataFilter.New();

            transformFilter2.SetTransform(transform2);
            transformFilter2.SetInput(transformFilter.GetOutput());
            transformFilter2.Update();



            vtkTransformPolyDataFilter transformFilter3 = vtkTransformPolyDataFilter.New();
            transformFilter3.SetInput(transformFilter2.GetOutput());
            vtkTransform transform3 = vtkTransform.New();
            transformFilter3.SetTransform(transform3);
            transform3.Translate(center[0], center[1], center[2]);
            transformFilter3.Update();


            vtkPolyData output = transformFilter3.GetOutput();

            double[] outCenter = output.GetCenter();
            double[] outBounds = output.GetBounds();

            int w = (int)outBounds[0];
            return output;

        }


        public static void IsoSurface(float[, ,] DataCube, float Threshold)
        {
            vtkStructuredPoints sp = new vtkStructuredPoints();
            sp.SetExtent(0, DataCube.GetLength(2), 0, DataCube.GetLength(1), 0, DataCube.GetLength(0));
            sp.SetOrigin(0, 0, 0);
            sp.SetDimensions(DataCube.GetLength(2), DataCube.GetLength(1), DataCube.GetLength(0));
            sp.SetSpacing(1.0, 1.0, 1.0);
            sp.SetScalarTypeToFloat();
            sp.SetNumberOfScalarComponents(1);
            //sp.AllocateScalars();
            unsafe
            {
                float* volptr = (float*)sp.GetScalarPointer();
                fixed (float* pData = DataCube)
                {
                    CopyMemory((ushort*)(void*)volptr, (ushort*)(void*)pData, (uint)Buffer.ByteLength(DataCube));
                }
            }

            DataCube = null;

            vtkContourFilter Marching = new vtkContourFilter();
            Marching.SetInput(sp);
            Marching.SetValue(0, Threshold);
            Marching.Update();

            vtkSmoothPolyDataFilter smoothFilter = vtkSmoothPolyDataFilter.New();
            smoothFilter.SetInputConnection(Marching.GetOutputPort());
            smoothFilter.SetNumberOfIterations(5);
            smoothFilter.SetRelaxationFactor(0.5);
            smoothFilter.Update();

            FromPolyData(smoothFilter.GetOutput(), ref DataCube);
        }

        public static void GetCurvature(ref float[, ,] DataCube, float Threshold, out double ave, out double max, out double min, out double sd, out long[] Bins, out double  rangeMin)
        {
            vtkStructuredPoints sp = new vtkStructuredPoints();
            sp.SetExtent(0, DataCube.GetLength(2), 0, DataCube.GetLength(1), 0, DataCube.GetLength(0));
            sp.SetOrigin(0, 0, 0);
            sp.SetDimensions(DataCube.GetLength(2), DataCube.GetLength(1), DataCube.GetLength(0));
            sp.SetSpacing(1.0, 1.0, 1.0);
            sp.SetScalarTypeToFloat();
            sp.SetNumberOfScalarComponents(1);
            //sp.AllocateScalars();
            unsafe
            {
                float* volptr = (float*)sp.GetScalarPointer();
                fixed (float* pData = DataCube)
                {
                    CopyMemory((ushort*)(void*)volptr, (ushort*)(void*)pData, (uint)Buffer.ByteLength(DataCube));
                }
            }

           // DataCube = null;

            vtkContourFilter Marching = new vtkContourFilter();
            Marching.SetInput(sp);
            Marching.SetValue(0, Threshold);
            Marching.Update();

            vtkCurvatures curves = new vtkCurvatures();
            curves.SetInput(Marching.GetOutput());

            curves.SetCurvatureTypeToMaximum();
            curves.Update();
            vtkDoubleArray  scales =(vtkDoubleArray) curves.GetOutput().GetPointData().GetScalars();

            double[] data = new double[scales.GetNumberOfTuples()];
           
            for (int i=0;i<scales.GetNumberOfTuples();i++)
                data[i]=scales.GetValue(i);

            double [] range = curves.GetOutput().GetScalarRange();

            
            max = range.MaxArray();
            min = range.MinArray();
            ave = range.AverageArray();

            max = data.MaxArray();
            min = data.MinArray();
            ave = data.AverageArray();
            sd = data.StandardDeviation();


            double binSize = 10;
             rangeMin = Math.Floor(min / binSize) * binSize;
            double rangeMax = Math.Ceiling(max / binSize) * binSize;

            int nBins = (int)((rangeMax - rangeMin) / binSize);

            Bins = new long[nBins];

            for (int i = 0; i < data.Length; i++)
            {
                int BinN = (int)((data[i] - rangeMin) / (rangeMax - rangeMin) * nBins);

                if (BinN > 0 && BinN < nBins)
                {
                    Bins[BinN]++;
                }
            }
           
        }

        private static vtkPolyData RescalePolyMesh(vtkPolyData original, vtkPolyData Finished)
        {

            //get information about the original
            double[] bounds;
            vtkPolyData contours = original;
            bounds = contours.GetBounds();
            double[] center;
            center = contours.GetCenter();


            // Rescale the output back into world coordinates and center it
            //
            double[] scaleCenter;
            scaleCenter = Finished.GetCenter();
            double[] scaleBounds;
            scaleBounds = Finished.GetBounds();


            vtkTransformPolyDataFilter transformFilter = vtkTransformPolyDataFilter.New();
            transformFilter.SetInput(Finished);
            vtkTransform transform = vtkTransform.New();
            transformFilter.SetTransform(transform);
            transform.Translate(-scaleCenter[0], -scaleCenter[1], -scaleCenter[2]);
            transformFilter.Update();


            vtkTransformPolyDataFilter transformFilter2 = vtkTransformPolyDataFilter.New();
            transformFilter2.SetInput(transformFilter.GetOutput());
            vtkTransform transform2 = vtkTransform.New();
            transformFilter2.SetTransform(transform2);
            double scaleX = (bounds[1] - bounds[0]) / (scaleBounds[1] - scaleBounds[0]);
            double scaleY = (bounds[3] - bounds[2]) / (scaleBounds[3] - scaleBounds[2]);
            double scaleZ = (bounds[5] - bounds[4]) / (scaleBounds[5] - scaleBounds[4]);
            transform2.Scale(scaleX, scaleY, scaleZ);
            transformFilter2.Update();



            vtkTransformPolyDataFilter transformFilter3 = vtkTransformPolyDataFilter.New();
            transformFilter3.SetInput(transformFilter2.GetOutput());
            vtkTransform transform3 = vtkTransform.New();
            transformFilter3.SetTransform(transform3);
            transform3.Translate(center[0], center[1], center[2]);
            transformFilter3.Update();


            vtkPolyData output = transformFilter3.GetOutput();

            double[] outCenter = output.GetCenter();
            double[] outBounds = output.GetBounds();

            int w = (int)outBounds[0];
            return output;
        }

        private static void FromPolyData(vtkPolyData pd, string Filename)
        {

            vtkImageData whiteImage = vtkImageData.New();
            double[] bounds = pd.GetBounds();

            double[] spacing = new double[] { 1, 1, 1 };
            whiteImage.SetSpacing(1, 1, 1);

            // compute dimensions
            int[] dim = new int[3];
            for (int i = 0; i < 3; i++)
            {
                dim[i] = (int)(Math.Ceiling((bounds[i * 2 + 1] - bounds[i * 2]) / spacing[i]));
            }
            whiteImage.SetDimensions(dim[0], dim[1], dim[2]);
            whiteImage.SetExtent(0, dim[0] - 1, 0, dim[1] - 1, 0, dim[2] - 1);

            double[] origin = new double[3];
            origin[0] = bounds[0] + spacing[0] / 2;
            origin[1] = bounds[2] + spacing[1] / 2;
            origin[2] = bounds[4] + spacing[2] / 2;
            whiteImage.SetOrigin(origin[0], origin[1], origin[2]);

            // fill the image with foreground voxels:
            byte inval = 255;
            byte outval = 0;
            long count = whiteImage.GetNumberOfPoints();
            for (long i = 0; i < count; ++i)
            {
                whiteImage.GetPointData().GetScalars().SetTuple1(i, inval);
            }

            // polygonal data - . image stencil:
            vtkPolyDataToImageStencil pol2stenc =
              vtkPolyDataToImageStencil.New();

            pol2stenc.SetInput(pd);

            pol2stenc.SetOutputOrigin(origin[0], origin[1], origin[2]);
            pol2stenc.SetOutputSpacing(spacing[0], spacing[1], spacing[2]);
            int[] extent = whiteImage.GetExtent();
            pol2stenc.SetOutputWholeExtent(extent[0], extent[1], extent[2], extent[3], extent[4], extent[5]);
            pol2stenc.Update();

            // cut the corresponding white image and set the background:
            vtkImageStencil imgstenc =
              vtkImageStencil.New();

            imgstenc.SetInput(whiteImage);
            imgstenc.SetStencil(pol2stenc.GetOutput());

            imgstenc.ReverseStencilOff();
            imgstenc.SetBackgroundValue(outval);
            imgstenc.Update();

            vtkMetaImageWriter writer =
              vtkMetaImageWriter.New();
            writer.SetFileName("SphereVolume.mhd");

            writer.SetInput(imgstenc.GetOutput());

            writer.Write();

        }
        private static void FromPolyData(vtkPolyData newPolydata, ref float[, ,] volume)
        {
            Bitmap b = new Bitmap(volume.GetLength(1), volume.GetLength(2));
            Graphics g = Graphics.FromImage(b);
            for (int m = 0; m < volume.GetLength(0); m += 1)
            {
                vtkPlane plane = vtkPlane.New();
                double[] origin = newPolydata.GetBounds();
                System.Diagnostics.Debug.Print(origin[0].ToString());
                int Axis = 0;

                if (Axis == 0)
                {
                    plane.SetOrigin(m, 0, 0);
                    plane.SetNormal(1, 0, 0);
                }
                if (Axis == 2)
                {
                    plane.SetOrigin(0, 0, m);
                    plane.SetNormal(0, 0, 1);

                }

                // Create cutter
                vtkCutter cutter = vtkCutter.New();
                cutter.SetCutFunction(plane);
                cutter.SetInput(newPolydata);

                cutter.Update();
                long nCell = cutter.GetOutput().GetNumberOfCells();
                if (nCell > 0)
                {
                    g.Clear(Color.White);

                    Pen p = new Pen(Color.Black, 2);
                    double lx = 0;
                    double ly = 0;


                   /* long nPoints = cutter.GetOutput().GetNumberOfPoints();

                    var points =cutter.GetOutput().GetPoints();
                    double[] LastPoint = points.GetPoint(0);
                    for (int i = 1; i < nPoints; i++)
                    {
                        double[] point2 = points.GetPoint(i);
                        if (LastPoint[0] > 1 && point2[0] > 1)
                        {
                            g.DrawLine(p, new Point((int)LastPoint[1], (int)LastPoint[2]), new Point((int)point2[1], (int)point2[2]));
                        }
                        LastPoint = point2;
                    }*/
                    
                    for (int i = 0; i < nCell; i++)
                    {
                        vtkPoints pts = cutter.GetOutput().GetCell(i).GetPoints();
                        double[] point1 = pts.GetPoint(0);
                        double[] point2 = pts.GetPoint(1);
                        if (Axis == 0)
                        {
                            if (point1[0] > 1 && point2[0] > 1)
                            {
                                g.DrawLine(p, new Point((int)point1[1], (int)point1[2]), new Point((int)point2[1], (int)point2[2]));
                            }
                        }
                        else if (Axis == 2)
                        {
                            if (point1[0] > 1 && point2[0] > 1)
                            {
                                g.DrawLine(p, new Point((int)point1[0], (int)point1[1]), new Point((int)point2[0], (int)point2[1]));
                            }
                        }
                    }


                    FloodFiller ff = new FloodFiller();
                    ff.FillColor = Color.Black.ToArgb();
                    ff.FloodFill(b, new Point(0, 0));

                    float[,] image = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToFloatArray(b, false);

                    for (int j = 0; j < volume.GetLength(1); j++)
                    {
                        for (int k = 0; k < volume.GetLength(2); k++)
                        {
                            volume[m, j, k] = image[j, k];
                        }
                    }
                }
            }

        }

        public static void Delaunay(List<Point3D[]> Points3D, ref float[, ,] volume, out object GraphicsObject)
        {
            vtkPoints Points = vtkPoints.New();
            Point3D lP = Points3D[0][0];
            //   double lz = lP.Z;
            Points.InsertNextPoint(lP.X, lP.Y, lP.Z);
            double d = 0;
            for (int j = 0; j < Points3D.Count; j++)
            {
                Point3D[] tPoints = Points3D[j];
                for (int i = 0; i < tPoints.Length; i++)
                {
                    Point3D p = tPoints[i];
                    d = p.Distance(lP);
                    if (d > 2)
                    {
                        Points.InsertNextPoint(p.X, p.Y, p.Z);
                        lP = p;
                        //     lz = p.Z;
                    }

                }
            }

            vtkPolyData profile = vtkPolyData.New();
            profile.SetPoints(Points);

            //vtkCleanPolyData cleaner = vtkCleanPolyData.New();
            //cleaner.SetInput(oprofile);
            //cleaner.SetTolerance(.1);
            //cleaner.Update();
            //vtkPolyData profile = cleaner.GetOutput();

            //# Delaunay3D is used to triangulate the points.0The Tolerance is the
            //# distance that nearly coincident points are merged
            //# together.0(Delaunay does better if points are well spaced.) The
            //# alpha value is the radius of circumcircles, circumspheres.0Any mesh
            //# entity whose circumcircle is smaller than this value is output.
            vtkDelaunay3D delny = vtkDelaunay3D.New();
            delny.SetInput(profile);
            delny.SetTolerance(5);
            delny.SetAlpha(25);
            //delny.BoundingTriangulationOff();

            delny.Update();

            /* //# Shrink the result to help see it better.
             vtkShrinkFilter shrink = vtkShrinkFilter.New();
             shrink.SetInputConnection(delny.GetOutputPort());
             shrink.SetShrinkFactor(1);*/

            //vtkDataSetMapper map = vtkDataSetMapper.New();
            //map.SetInputConnection(delny.GetOutputPort());

            vtkExtractUnstructuredGrid pEXGrid = vtkExtractUnstructuredGrid.New();
            pEXGrid.SetInput(delny.GetOutput());
            pEXGrid.CellClippingOff();
            pEXGrid.SetCellMinimum(3);
            pEXGrid.SetCellMaximum(23);

            vtkDataSetSurfaceFilter surfaceFilter = vtkDataSetSurfaceFilter.New();

            surfaceFilter.SetInput(pEXGrid.GetOutput());
            surfaceFilter.Update();

            vtkFillHolesFilter fillHolesFilter = vtkFillHolesFilter.New();
            fillHolesFilter.SetInput(surfaceFilter.GetOutput());

            fillHolesFilter.Update();

            vtkPolyData polydata = fillHolesFilter.GetOutput();

            /* vtkTriangleFilter triangle = vtkTriangleFilter.New();
             triangle.SetInput(polydata);
             triangle.Update();
             vtkMassProperties massProperty = vtkMassProperties.New();
             massProperty.SetInput(triangle.GetOutput());
             massProperty.Update();
             double vol = massProperty.GetVolume();
             vol = vol + 1;
             System.Diagnostics.Debug.Print(vol.ToString());
            vtkDataSetMapper map = vtkDataSetMapper.New();
            map.SetInputConnection(delny.GetOutputPort());

            vtkActor triangulation = vtkActor.New();
            triangulation.SetMapper(map);
            triangulation.GetProperty().SetColor(1, 0, 0);*/

            //Go through the Graphics Pipeline
            //this.Renderer2.AddVolume(triangulation);*/


            GraphicsObject = RescalePolyMesh(profile, polydata);
            FromPolyData((vtkPolyData)GraphicsObject, ref volume);

            /*GraphicsObject = polydata;

            return GetVolumeFromPolyData(profile, polydata);*/
        }
        /*
        public void GaussianSplatEasy(vtkPoints Points)
        {

            vtkPolyData profile = vtkPolyData.New();
            profile.SetPoints(Points);

            //# Delaunay3D is used to triangulate the points.0The Tolerance is the
            //# distance that nearly coincident points are merged
            //# together.0(Delaunay does better if points are well spaced.) The
            //# alpha value is the radius of circumcircles, circumspheres.0Any mesh
            //# entity whose circumcircle is smaller than this value is output.
            vtkDelaunay3D delny = vtkDelaunay3D.New();
            delny.SetInput(profile);
            delny.SetTolerance(0.00001);
            delny.SetAlpha(10);
            delny.BoundingTriangulationOff();

            delny.Update();

            //# Shrink the result to help see it better.
            vtkShrinkFilter shrink = vtkShrinkFilter.New();
            shrink.SetInputConnection(delny.GetOutputPort());
            shrink.SetShrinkFactor(1);

            vtkDataSetMapper map = vtkDataSetMapper.New();
            map.SetInputConnection(shrink.GetOutputPort());

            //vtkDataSetMapper map = vtkDataSetMapper.New();
            //map.SetInputConnection(delny.GetOutputPort());

            vtkExtractUnstructuredGrid pEXGrid = vtkExtractUnstructuredGrid.New();
            pEXGrid.SetInput(delny.GetOutput());
            pEXGrid.CellClippingOff();
            pEXGrid.SetCellMinimum(3);
            pEXGrid.SetCellMaximum(23);

            vtkDataSetSurfaceFilter surfaceFilter = vtkDataSetSurfaceFilter.New();

            surfaceFilter.SetInput(pEXGrid.GetOutput());
            surfaceFilter.Update();

            vtkPolyData polydata = surfaceFilter.GetOutput();

            vtkTriangleFilter triangle = vtkTriangleFilter.New();

            triangle.SetInput(polydata);

            triangle.Update();

            vtkMassProperties massProperty = vtkMassProperties.New();

            massProperty.SetInput(triangle.GetOutput());

            massProperty.Update();

            double vol = massProperty.GetVolume();


            vol = vol + 1;

            System.Diagnostics.Debug.Print(vol.ToString());


            vtkActor triangulation = vtkActor.New();
            triangulation.SetMapper(map);
            triangulation.GetProperty().SetColor(1, 0, 0);


            //Go through the Graphics Pipeline
            //this.Renderer2.AddVolume(triangulation);


        }
        */
        public static void GaussianSplat(List<Point3D[]> Points3D, ref float[, ,] volume, out object GraphicsObject)
        {

            vtkPoints Points = vtkPoints.New();
            for (int j = 0; j < Points3D.Count; j++)
            {
                Point3D[] tPoints = Points3D[j];
                for (int i = 0; i < tPoints.Length; i++)
                {
                    Point3D p = tPoints[i];
                    Points.InsertNextPoint(p.X, p.Y, p.Z);
                }
            }

            vtkPolyData profile = vtkPolyData.New();
            profile.SetPoints(Points);
            profile.Update();

            vtkGaussianSplatter GaussianSplatter = vtkGaussianSplatter.New();

            GaussianSplatter.SetInput(profile);
            GaussianSplatter.SetSampleDimensions(25, 25, 25);
            GaussianSplatter.SetRadius(.5);
            GaussianSplatter.ScalarWarpingOff();
            GaussianSplatter.Update();

            vtkContourFilter surface = vtkContourFilter.New();
            surface.SetInputConnection(GaussianSplatter.GetOutputPort());
            surface.SetValue(0, 0.1);
            surface.Update();

            vtkDataSetSurfaceFilter surfaceFilter = vtkDataSetSurfaceFilter.New();
            surfaceFilter.SetInput(surface.GetOutput());
            surfaceFilter.Update();

            vtkPolyData polydata = vtkPolyData.New();
            polydata = surfaceFilter.GetOutput();
            polydata.Update();
            /*   vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
               mapper.SetInputConnection(surfaceFilter.GetOutputPort());
            

               vtkActor triangulation = vtkActor.New();
               triangulation.SetMapper(mapper);
               triangulation.GetProperty().SetColor(1, 0, 0);
               mapper.Update();*/


            //Go through the Graphics Pipeline
            //this.Renderer2.AddVolume(triangulation);


            GraphicsObject = RescalePolyMesh(profile, polydata);
            FromPolyData((vtkPolyData)GraphicsObject, ref volume);

            /*GraphicsObject = polydata;
            return GetVolumeFromPolyData(profile, polydata);*/
        }

        public static void SurfaceFromUnorganized(List<Point3D[]> Points3D, ref float[, ,] volume, out object GraphicsObject)
        {
            vtkPoints Points = vtkPoints.New();
            Point3D lP = Points3D[0][0];
            double lz = lP.Z;
            Points.InsertNextPoint(lP.X, lP.Y, lP.Z);
            double d = 0;
            for (int j = 0; j < Points3D.Count; j++)
            {
                Point3D[] tPoints = Points3D[j];
                for (int i = 0; i < tPoints.Length; i++)
                {
                    Point3D p = tPoints[i];
                    d = p.Distance(lP);
                    if (d > 2)
                    {
                        Points.InsertNextPoint(p.X, p.Y, p.Z);
                        lP = p;
                        lz = p.Z;
                    }

                }
            }

            vtkPolyData polydata = vtkPolyData.New();
            polydata.SetPoints(Points);

         

            vtkSurfaceReconstructionFilter surf = vtkSurfaceReconstructionFilter.New();
            surf.SetNeighborhoodSize(50);
            surf.SetInput(polydata);
            surf.Update();

            vtkContourFilter cf = vtkContourFilter.New();
            cf.SetInputConnection(surf.GetOutputPort());
            cf.SetValue(0, 0.0);
            cf.Update();

            /* vtkDataSetSurfaceFilter surface = new vtkDataSetSurfaceFilter();
             surface.SetInput(cf.GetOutput());
             surface.Update();*/

            /* //# Shrink the result to help see it better.
             vtkShrinkFilter shrink = vtkShrinkFilter.New();
             shrink.SetInputConnection(cf.GetOutputPort());
             shrink.SetShrinkFactor(1);*/

            /* vtkDataSetMapper map = vtkDataSetMapper.New();
             map.SetInputConnection(shrink.GetOutputPort());*/

            /* vtkExtractUnstructuredGrid pEXGrid = vtkExtractUnstructuredGrid.New();
             pEXGrid.SetInput(cf.GetOutput());
             pEXGrid.CellClippingOff();
             pEXGrid.SetCellMinimum(3);
             pEXGrid.SetCellMaximum(23);*/

            vtkDataSetSurfaceFilter surfaceFilter = vtkDataSetSurfaceFilter.New();
            surfaceFilter.SetInput(cf.GetOutput());
            surfaceFilter.Update();

            /* vtkActor triangulation = vtkActor.New();
             triangulation.SetMapper(map);
             triangulation.GetProperty().SetColor(1, 0, 0);
            //Go through the Graphics Pipeline
            //this.Renderer2.AddVolume(triangulation);

            GraphicsObject = surfaceFilter.GetOutput(); //triangulation;

            return GetVolumeFromPolyData(polydata, surfaceFilter.GetOutput());*/
            GraphicsObject = RescalePolyMesh(polydata, surfaceFilter.GetOutput());
            FromPolyData((vtkPolyData)GraphicsObject, ref volume);
        }

        public static void MarchingCubes(List<Point3D[]> Points3D, ushort[, ,] DataCube, Dictionary<int, int> Remapping, out object GraphicsObject, ref float[, ,] volume)
        {

            vtkPoints OPoints = vtkPoints.New();
            for (int j = 0; j < Points3D.Count; j++)
            {
                Point3D[] tPoints = Points3D[j];
                for (int i = 0; i < tPoints.Length; i++)
                {
                    Point3D p = tPoints[i];
                    OPoints.InsertNextPoint(p.X, p.Y, p.Z);
                }
            }
            vtkPolyData oPoly = vtkPolyData.New();
            oPoly.SetPoints(OPoints);
            oPoly.Update();

            vtkImageData sp = vtkImageData.New();

            sp.SetDimensions(DataCube.GetLength(2), DataCube.GetLength(1), DataCube.GetLength(0));
            sp.SetNumberOfScalarComponents(1);
            sp.SetScalarTypeToUnsignedShort();
            unsafe
            {
                ushort* volptr = (ushort*)sp.GetScalarPointer();
                fixed (ushort* pData = DataCube)
                {
                    CopyMemory(volptr, pData, (uint)Buffer.ByteLength(DataCube));
                }
            }
            DataCube = null;

            vtkImageGaussianSmooth gs = vtkImageGaussianSmooth.New();
            gs.SetInput(sp);
            gs.SetDimensionality(3);
            gs.SetRadiusFactors(1, 1, 1);
            gs.Update();

            //vtkMarchingCubes Marching = vtkMarchingCubes.New();
            vtkContourFilter Marching = new vtkContourFilter();
            Marching.ComputeNormalsOn();
            Marching.ComputeGradientsOn();
            Marching.ComputeScalarsOn();
            Marching.SetInput(gs.GetOutput());
            Marching.SetValue(0, 150);
            Marching.Update();


            vtkSmoothPolyDataFilter smoothFilter = vtkSmoothPolyDataFilter.New();
            smoothFilter.SetInputConnection(Marching.GetOutputPort());
            smoothFilter.SetNumberOfIterations(5);
            smoothFilter.SetRelaxationFactor(0.5);
            smoothFilter.Update();


            int MaxValue = 0;
            foreach (KeyValuePair<int, int> kvp in Remapping)
            {
                if (kvp.Key > MaxValue) MaxValue = kvp.Key;
            }

            vtkPolyData FinalPoints = RescalePolyMesh(oPoly, smoothFilter.GetOutput());
            long nPoints = FinalPoints.GetNumberOfPoints();
            vtkPoints Points = vtkPoints.New();
            for (long j = 0; j < nPoints; j++)
            {
                double[] p = FinalPoints.GetPoint(j);
                int z = (int)Math.Floor(p[2]);
                double z0 = 0;

                if (z < 0) z = 0;

                if (z >= MaxValue)
                {
                    z0 = Remapping[MaxValue - 1];
                }
                else
                {
                    z0 = Remapping[z];
                }
                double z1;
                if (z >= MaxValue)
                {
                    z1 = (z0 - Remapping[MaxValue - 1]) + z0;
                }
                else
                {
                    z1 = Remapping[z + 1];
                }

                double u = (p[2] - z) * (z1 - z0) + z0;
                p[2] = u;
                Points.InsertNextPoint(p[0], p[1], p[2]);
            }
            FinalPoints.SetPoints(Points);

            //  FinalPoints = RotatePolyMeshAroundCenter( FinalPoints,90,0,1,0);

            GraphicsObject = FinalPoints;// RescalePolyMesh(profile, polydata);
            FromPolyData(FinalPoints, ref volume);

            //GraphicsObject = FinalPoints;

            //return GetVolumeFromPolyData(FinalPoints, FinalPoints);
        }

        public static void IsoContour(List<Point3D[]> Points3D, ref float[, ,] volume, out object GraphicsObject)
        {
            //# Define a Single Cube
            vtkFloatArray Scalars = new vtkFloatArray();
            vtkPoints AllPoints = new vtkPoints();
            int cc = 0;
            vtkUnstructuredGrid Grid = new vtkUnstructuredGrid();
            // Grid.Allocate(100, 100);

            cc = 0;
            for (int i = 0; i < Points3D.Count; i++)
            {

                vtkIdList Ids = new vtkIdList();
                for (int k = 0; k < Points3D[i].Length; k++)
                {
                    Scalars.InsertNextValue(1.0f);
                    AllPoints.InsertNextPoint(Points3D[i][k].X, Points3D[i][k].Y, Points3D[i][k].Z);
                    Ids.InsertNextId(cc);
                    cc++;
                }
                Grid.InsertNextCell(3, Ids);
            }
            Grid.SetPoints(AllPoints);
            Grid.GetPointData().SetScalars(Scalars);


            vtkContourFilter Contour = new vtkContourFilter();
            Contour.SetInput(Grid);
            Contour.SetValue(0, 0.5);
            Contour.Update();

            vtkDataSetSurfaceFilter surfaceFilter = vtkDataSetSurfaceFilter.New();
            surfaceFilter.SetInput(Contour.GetOutput());
            surfaceFilter.Update();

            /* vtkActor triangulation = vtkActor.New();
             triangulation.SetMapper(map);
             triangulation.GetProperty().SetColor(1, 0, 0);*/
            //Go through the Graphics Pipeline
            //this.Renderer2.AddVolume(triangulation);

            //GraphicsObject = Contour.GetOutput();
            /* vtkPolyDataMapper boneMapper = vtkPolyDataMapper.New();
             boneMapper.SetInputConnection(Contour.GetOutputPort());
             boneMapper.ScalarVisibilityOff();
             vtkActor bone = vtkActor.New();
             bone.SetMapper(boneMapper);*/

            //Go through the Graphics Pipeline
            // this.Renderer2.AddVolume(bone);

            vtkPolyData polydata = vtkPolyData.New();
            polydata.SetPoints(AllPoints);

            // return GetVolumeFromPolyData(polydata, surfaceFilter.GetOutput());*/

            GraphicsObject = RescalePolyMesh(polydata, surfaceFilter.GetOutput());
            FromPolyData((vtkPolyData)GraphicsObject, ref volume);
        }

        public static void FromContour(List<Point3D[]> Points, ref float[, ,] volume, out object GraphicsObject)
        {
            // Create the data: a series of discs representing the intersections of x-y planes through a unit sphere
            // centered at 0, 0, 0
            //
            int numDivisions = Points.Count;
            double sphereRadius = 1.0;
            double zmin = -0.9 * sphereRadius;
            double zmax = 0.9 * sphereRadius;

            // Append all the discs into one polydata
            //
            vtkAppendPolyData appendFilter = vtkAppendPolyData.New();

            double dx = 0, dy = 0, dz = 0;
            long cc = 0;
            for (int i = 0; i < numDivisions; ++i)
            {
                vtkPolyData circle = vtkPolyData.New();
                CreateCircle(Points[i], ref circle);
                appendFilter.AddInput(circle);
                if (i < numDivisions - 1)
                {
                    for (int j = 0; j < Points[i].Length && j < Points[i + 1].Length; j++)
                    {
                        dx += Points[i][j].X - Points[i + 1][j].X;
                        dy += Points[i][j].Y - Points[i + 1][j].Y;
                        dz += Points[i][j].Z - Points[i + 1][j].Z;
                        cc++;
                    }
                }
            }

            dx = Math.Abs(dx / cc * 1.2);
            dy = Math.Abs(dy / cc * 1.2);
            dz = Math.Abs(dz / cc);

            double deltaz = 1;

            if ((appendFilter.GetNumberOfInputConnections(0) == 0))
            {

            }

            appendFilter.Update();

            // Convert to ijk coordinates for the contour to surface filter
            //
            double[] bounds = new double[6];
            vtkPolyData contours = appendFilter.GetOutput();
            bounds = contours.GetBounds();
            double[] origin = new double[] { bounds[0], bounds[2], bounds[4] };
            //double[] spacing = new double[] { dx, dy, dz };
            double[] spacing = new double[] { 1, 1, 1 };// bounds[1] - bounds[0], bounds[3] - bounds[2], bounds[5] - bounds[4] };

            /*new double[] { (bounds[1] - bounds[0]) / 40,
                    (bounds[3] - bounds[2]) / 40,
                     deltaz };*/

            vtkPolyData poly = vtkPolyData.New();
            vtkPoints points = vtkPoints.New();
            vtkPoints contourPoints = contours.GetPoints();
            long numPoints = contourPoints.GetNumberOfPoints();
            points.SetNumberOfPoints(numPoints);
            for (int i = 0; i < numPoints; ++i)
            {
                double[] pt;
                pt = contourPoints.GetPoint(i);
                pt[0] = (int)((pt[0] - origin[0]) / spacing[0] + 0.5);
                pt[1] = (int)((pt[1] - origin[1]) / spacing[1] + 0.5);
                pt[2] = (int)((pt[2] - origin[2]) / spacing[2] + 0.5);
                points.SetPoint(i, pt[0], pt[1], pt[2]);
            }
            poly.SetPolys(contours.GetPolys());
            poly.SetPoints(points);

            // Create the contour to surface filter
            //
            vtkVoxelContoursToSurfaceFilter contoursToSurface = vtkVoxelContoursToSurfaceFilter.New();
            // contoursToSurface.SetInput(poly);
            contoursToSurface.SetInput(contours);
            // contoursToSurface.SetSpacing(spacing[0], spacing[1], spacing[2]);
            contoursToSurface.Update();

            // Rescale the output back into world coordinates and center it
            //
            double[] scaleCenter;
            scaleCenter = contoursToSurface.GetOutput().GetCenter();
            double[] scaleBounds;
            scaleBounds = contoursToSurface.GetOutput().GetBounds();
            double[] center;
            center = contours.GetCenter();

            vtkTransformPolyDataFilter transformFilter = vtkTransformPolyDataFilter.New();
            transformFilter.SetInputConnection(contoursToSurface.GetOutputPort());
            vtkTransform transform = vtkTransform.New();
            transformFilter.SetTransform(transform);
            transform.Translate(-scaleCenter[0], -scaleCenter[1], -scaleCenter[2]);
            transform.Scale(
              (bounds[1] - bounds[0]) / (scaleBounds[1] - scaleBounds[0]),
              (bounds[3] - bounds[2]) / (scaleBounds[3] - scaleBounds[2]),
              (bounds[5] - bounds[4]) / (scaleBounds[5] - scaleBounds[4]));
            transform.Translate(center[0], center[1], center[2]);

            // Visualize the contours
            //
            vtkPolyDataMapper contoursMapper = vtkPolyDataMapper.New();
            contoursMapper.SetInput(contours);
            contoursMapper.ScalarVisibilityOff();

            vtkActor contoursActor = vtkActor.New();
            contoursActor.SetMapper(contoursMapper);
            contoursActor.GetProperty().SetRepresentationToWireframe();
            contoursActor.GetProperty().ShadingOff();
            contoursMapper.Update();
            /*// Visualize the surface
            //
            vtkPolyDataMapper surfaceMapper = vtkPolyDataMapper.New();
            surfaceMapper.SetInputConnection( transformFilter.GetOutputPort() );
            surfaceMapper.ScalarVisibilityOff();
            surfaceMapper.ImmediateModeRenderingOn();
 
            vtkActor surfaceActor = vtkActor.New();
            surfaceActor.SetMapper( surfaceMapper );
            surfaceActor.GetProperty().SetRepresentationToWireframe();
            surfaceActor.GetProperty().ShadingOff();*/

            //GraphicsObject = contoursToSurface.GetOutput();
            GraphicsObject = transformFilter.GetOutput();
            // vtkPolyData polydata = appendFilter.GetOutput();
            // polydata.SetPoints(appendFilter.GetOutput());

            //FromPolyData(contoursToSurface.GetOutput(), ref volume);
            FromPolyData(transformFilter.GetOutput(), ref volume);
            //return GetVolumeFromPolyData(polydata, transformFilter.GetOutput());
        }

        private static void CreateCircle(Point3D[] Points, ref vtkPolyData polyData)
        {
            vtkPoints points = vtkPoints.New();
            vtkCellArray cells = vtkCellArray.New();

            int resolution = Points.Length;
            points.SetNumberOfPoints(resolution);
            cells.Allocate(1, resolution);
            cells.InsertNextCell(resolution);

            for (int i = 0; i < resolution; ++i)
            {

                double x = Points[i].X;
                double y = Points[i].Y;
                double z = Points[i].Z;
                points.SetPoint(i, x, y, z);
                cells.InsertCellPoint(i);
            }

            polyData.Initialize();
            polyData.SetPolys(cells);
            polyData.SetPoints(points);
        }
    }
}
