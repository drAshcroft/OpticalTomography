using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kitware.VTK;
using System.Runtime.InteropServices;

namespace ProcessRecons
{
    public partial class vtkForm : Form
    {

        private Kitware.VTK.RenderWindowControl renderWindowControl1;

        public vtkForm()
        {
            InitializeComponent();


            this.renderWindowControl1 = new Kitware.VTK.RenderWindowControl();
            this.renderWindowControl1.AddTestActors = false;
            this.renderWindowControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.renderWindowControl1.Location = new System.Drawing.Point(3, 3);
            this.renderWindowControl1.Name = "renderWindowControl1";
            this.renderWindowControl1.Size = new System.Drawing.Size(this.Width , this.Height );
            this.renderWindowControl1.Dock = DockStyle.Fill;
            this.renderWindowControl1.TabIndex = 0;
            this.renderWindowControl1.TestText = null;
            this.renderWindowControl1.Load += new EventHandler(renderWindowControl1_Load);

           // splitContainer1.Panel2.Controls.Add(this.renderWindowControl1);
            this.tableLayoutPanel1.Controls.Add(this.renderWindowControl1, 0,0);
            //this.Controls.Add(this.renderWindowControl1);
        }

        unsafe void renderWindowControl1_Load(object sender, EventArgs e)
        {
           
        }

        vtkColorTransferFunction ctf = vtkColorTransferFunction.New();
        vtkPiecewiseFunction spwf = vtkPiecewiseFunction.New();
        vtkPiecewiseFunction gpwf = vtkPiecewiseFunction.New();
        int ScaleVTK = 255;
        vtkRenderer ren1;
        vtkRenderWindow renWin;
        vtkRenderWindowInteractor iren;
        vtkVolume vol = vtkVolume.New();

        unsafe void LoadData()
        {
            int Width = Data.GetLength(2);
            int Height = Data.GetLength(1);
            int NumSlices = Data.GetLength(0);

            vtkStructuredPoints sp = new vtkStructuredPoints();
            sp.SetExtent(0, Width, 0, Height, 0, NumSlices);
            sp.SetOrigin(0, 0, 0);
            sp.SetDimensions(Width, Height, NumSlices);
            sp.SetSpacing(1.0, 1.0, 1.0);
            sp.SetScalarTypeToUnsignedShort();
            sp.SetNumberOfScalarComponents(1);

            ushort* volptr = (ushort*)sp.GetScalarPointer();
          
            fixed (ushort* pData = Data)
            {
                CopyMemory(volptr, pData, (uint)Buffer.ByteLength(Data));
            }

            Data = null;

            // Create the RenderWindow, Renderer and both Actors
            ren1 = renderWindowControl1.RenderWindow.GetRenderers().GetFirstRenderer();
            renWin = vtkRenderWindow.New();
            renWin.AddRenderer(ren1);
            iren = vtkRenderWindowInteractor.New();
            iren.SetRenderWindow(renWin);


            // Add the actors to the renderer, set the background and size
            ren1.SetBackground(0, 0, 0);
            renWin.SetSize(300, 300);

            vtkFixedPointVolumeRayCastMapper texMapper = vtkFixedPointVolumeRayCastMapper.New();
          
           
          
            //Go through the visulizatin pipeline
            texMapper.SetInput(sp);

            
            //Set the color curve for the volume
            ctf.AddHSVPoint(0, .67, .07, 1);
            ctf.AddHSVPoint(94 * ScaleVTK, .67, .07, 1);
            ctf.AddHSVPoint(139 * ScaleVTK, 0, 0, 0);
            ctf.AddHSVPoint(160 * ScaleVTK, .28, .047, 1);
            ctf.AddHSVPoint(254 * ScaleVTK, .38, .013, 1);

            //Set the opacity curve for the volume
            spwf.AddPoint(0 * ScaleVTK, 0);
            //spwf.AddPoint(151 * Scale, .1);
            spwf.AddPoint(255 * ScaleVTK, .2);

            dragGraph1.InsertControlPoint(new PointF(0f, 0f));
            dragGraph1.InsertControlPoint(new PointF(1f,.2f));

            //Set the gradient curve for the volume
            gpwf.AddPoint(0 * ScaleVTK, .2);
            //gpwf.AddPoint(10 * Scale, .2);
            gpwf.AddPoint(25 * ScaleVTK, .2);

            vol.GetProperty().SetColor(ctf);
            vol.GetProperty().SetScalarOpacity(spwf);
            vol.GetProperty().SetGradientOpacity(gpwf);

            vol.SetMapper(texMapper);

            //Go through the Graphics Pipeline
            ren1.AddVolume(vol);


           
        }

        
       

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        static unsafe  extern void CopyMemory(ushort* Destination, ushort* Source, uint Length);

        ushort[, ,] Data;
        public unsafe  void OpenVisualization(string Filename)
        {
             Data=MathHelpLib.MathHelpsFileLoader.OpenDensityDataInt(Filename);
             LoadData();
        }

        private void dragGraph1_DrawPointAdded(int index, PointF newPoint)
        {
            spwf.RemoveAllPoints();
            foreach (PointF p in dragGraph1.ControlPoints)
                spwf.AddPoint(p.X * 255 * ScaleVTK, p.Y);
            spwf.Update();
            renderWindowControl1.Refresh();
        }

        private void dragGraph1_DrawPointMoved(int index, PointF newValue)
        {
            spwf.RemoveAllPoints();
            foreach (PointF p in dragGraph1.ControlPoints)
                spwf.AddPoint(p.X * 255 * ScaleVTK, p.Y);
            spwf.Update();
            renderWindowControl1.Refresh();
        }

        private void bRotateAndSave_Click(object sender, EventArgs e)
        {

          vtkWindowToImageFilter windowToImage = vtkWindowToImageFilter.New();

            windowToImage.SetInput(renWin);
            vtkJPEGWriter writer = vtkJPEGWriter.New();

            writer.SetInputConnection(windowToImage.GetOutputPort());
            writer.SetFileName("image.tif");

            for (int i = 0; i < 360; i += 5)
            {
                vol.RotateZ(5);

                windowToImage.Update();
    
    renWin.Render();
    writer.Write();
              
            }
        }
    }
}
