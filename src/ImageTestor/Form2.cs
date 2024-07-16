using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ImageTestor
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Show();
            Application.DoEvents();
            if (viewerControl3D1.MyMenu != null)
            {
                this.MainMenuStrip = viewerControl3D1.MyMenu;
                this.Controls.Add(viewerControl3D1.MyMenu);
                viewerControl3D1.MyMenu.BringToFront();
                viewerControl3D1.MyMenu.Visible = true;
                viewerControl3D1.Top = 24;
            }


        }

        [DllImport("ProjectFBPImage.dll")]
        private static unsafe extern IntPtr CreateFBP(string KernalName, string ConvolutionCode, string FlagStr,
            float* Image, int ImageWidth, int ImageHeight, float* Cube, int CubeWidth, int CubeHeight, int CubeDepth);

        [DllImport("ProjectFBPImage.dll")]
        private static unsafe extern int RunFBP(IntPtr Convoluter, float* Image);

        [DllImport("ProjectFBPImage.dll")]
        private static unsafe extern int CloseFBP(IntPtr Convoluter);

        #region FBPCode
        string FBP = @"
__kernel void simpleFBP(__global  float  * output,
                                __global  float  * input,
                                const     uint2  inputDimensions,
                                const     uint4  cubeDimensions)
{
    uint tid   = get_global_id(0);
    
    uint width  = inputDimensions.x;
    uint height = inputDimensions.y;
  
	uint cubeWidth  = cubeDimensions.x;
    uint cubeHeight = cubeDimensions.y;  
    uint cubeDepth = cubeDimensions.z;
	
	uint cubePlaneSize = cubeWidth*cubeHeight;

    //get the location within the cube
	uint z = tid/cubePlaneSize;
	uint CurPlane =tid- z*cubePlaneSize;
    uint x = CurPlane %cubeWidth;
    uint y = CurPlane /cubeWidth;

  if (tid>output[0])
		output[0] = tid;
}
";
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            float[, ,] data = new float[25, 25,100];
              double d;
            int Half = data.GetLength(0) / 2;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    for (int m = 0; m < data.GetLength(2); m++)
                    {
                        //  d = 255*( Math.Abs(Math.Sin(Math.Sqrt((i - Half) * (i - Half) + (j - Half) * (j - Half) + (m - Half) * (m - Half)) / Half * 2 * Math.PI)));
                        d = m - 100;
                        data[i, j, m] = -1*m*m+50;// Math.Exp(-1 * d / 100d);
                    }
                }
            }

            
            viewerControl3D1.SetImage(data);
        }
    }
}
