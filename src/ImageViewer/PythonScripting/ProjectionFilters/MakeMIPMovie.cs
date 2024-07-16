using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ImageViewer.Filters;
using MathHelpLib;



namespace ImageViewer.PythonScripting.Projection
{
    public class MakeMIPMovie : aEffectNoForm
    {
        public override string EffectName { get { return "Make MIP Movie"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Projection"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
            ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mPassData = PassData;

            mFilterToken = Parameters;

            PhysicalArray DensityGrid = (PhysicalArray)mFilterToken[0];
            string BaseFilename = (string)mFilterToken[1];
            string OutPath=Path.GetDirectoryName( Application.ExecutablePath ) + "\\temp\\";
            string[] Frames=new string[360];
            PhysicalArray MIP=null;

            for (int i = 0; i < 360; i++)
            {
                double Angle = (double)i / 180d * Math.PI;
                MIP = DoMIPProjection_OneSlice(DensityGrid, Angle);
                Frames[i] = OutPath + "Frame" + i.ToString() + ".bmp";
                MIP.SaveData(Frames[i]);
            }

            ImagingTools.CreateAVIVideo(BaseFilename, Frames);

            return new ImageHolder((double[,]) MIP.ActualData2D);
        }

        public override object[] DefaultProperties
        {
            get { return null; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Convolution|PhysicalArray",  "MovieFilename|string" }; }
        }

        public PhysicalArray  DoMIPProjection_OneSlice(PhysicalArray DensityGrid, double AngleRadians)
        {
            PhysicalArray SliceProjection;
            MathHelpLib.Axis RotationAxis = MathHelpLib.Axis.ZAxis;
            if (DensityGrid.ArrayRank == PhysicalArrayRank.Array2D)
            {
                throw new Exception("Not Yet Implimented");
            }
            else
            {
                Point3D vRotationAxis = new Point3D();
                Point3D axis = new Point3D();

                if (RotationAxis == MathHelpLib.Axis.XAxis)
                {
                    vRotationAxis = new Point3D(1, 0, 0);
                    axis = new Point3D(0, 1, 0);
                }
                else if (RotationAxis == MathHelpLib.Axis.YAxis)
                {
                    vRotationAxis = new Point3D(0, 1, 0);
                    axis = new Point3D(0, 0, 1);
                }
                else if (RotationAxis == MathHelpLib.Axis.ZAxis)
                {
                    vRotationAxis = new Point3D(0, 0, 1);
                    axis = new Point3D(0, 1, 0);
                }

                Point3D vec = Point3D.RotateAroundAxis(axis, vRotationAxis, AngleRadians);

                SliceProjection = DensityGrid.ProjectMIP(vec, Point3D.CrossProduct(vec, vRotationAxis));
                return SliceProjection;
            }
        }
        

     
    }
}
