using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Tomographic_Imaging_2;
using Microsoft.Office.Interop.Word;
using System.Threading;
using MathHelpLib;

namespace WordExporter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] Directories = Directory.GetDirectories(@"V:\ASU_Recon\viveks_RT24");
            lDataDirectories.Items.Clear();
            for (int i = 0; i < Directories.Length; i++)
                lDataDirectories.Items.Add(Path.GetFileName(Directories[i]));

        }

        private string GetDataInFolder()
        {
            //build the file structure
            string pPath;

            pPath = @"V:\ASU_Recon\viveks_RT24\";
            return pPath;
        }
        private string GetExperimentFolder()
        {
            string pPath = GetDataInFolder() + lDataDirectories.SelectedItem.ToString() + "\\";
            string DataPath = Path.GetDirectoryName(pPath) + "\\";
            return DataPath;
        }
        private string GetDataFolder()
        {
            //build the file structure

            return GetExperimentFolder() + "Data\\";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> Selected = new List<string>();
            for (int i = 0; i < lDataDirectories.Items.Count; i++)
            {
                //  if (lDataDirectories.GetSelected(i) == true)
                {
                    string pPath = GetDataInFolder() + (string)lDataDirectories.Items[i] + "\\";
                    Selected.Add(pPath);
                }
            }


            object oMissing = System.Reflection.Missing.Value;
            object missing = System.Reflection.Missing.Value;
            object Visible = true;
            object start1 = 0;
            object end1 = 0;

            Microsoft.Office.Interop.Word._Application oWord;
            Microsoft.Office.Interop.Word._Document oDoc;
            oWord = new Microsoft.Office.Interop.Word.Application();
            oWord.Visible = true;
            oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
                ref oMissing, ref oMissing);

            Range rng = oDoc.Range(ref start1, ref oMissing);

            oWord.Visible = true;

            foreach (string experiment in Selected)
            {
                // throw new Exception("needs to be fixed");

                try
                {

                    Bitmap[] Views = new Bitmap[5];
                  //  Views[3] = new Bitmap(experiment + "data\\CrossSections_X5.jpg");
                    string[] viewFiles= Directory.GetFiles(experiment+"\\data", "CrossSections_X*.jpg");
                    for (int i=0;i<viewFiles.Length && i<4;i++)
                        Views[i] = new Bitmap(viewFiles[i]);
                    //try
                    //{
                    //    Views[0] = new Bitmap(experiment + "data\\CrossSections_X_512_.jpg");
                    //}
                    //catch { }

                    //try
                    //{
                    //    Views[1] = new Bitmap(experiment + "data\\CrossSections_X_it_512_.jpg");
                    //}
                    //catch { }

                    //try
                    //{
                    //    Views[2] = new Bitmap(experiment + "data\\CrossSections_X__it_TIK.jpg");
                    //}
                    //catch { }
                  
                    //try
                    //{
                    //   // Views[3] = new Bitmap(experiment + "data\\CrossSections_X__wo_kal.jpg");
                    //}
                    //catch { }
                    try
                    {
                        Bitmap v = new Bitmap(experiment + "data\\vgExample.png");

                        int bigger = (v.Width > v.Height ? v.Width : v.Height);
                        Bitmap m3 = new Bitmap(bigger, bigger);
                        Graphics g2 = Graphics.FromImage(m3);


                        if (v.Width > v.Height)
                        {
                            //g2.DrawImage(v, new System.Drawing.Point(0, (bigger - v.Height) / 2));
                            g2.DrawImage(v, new RectangleF(0, (bigger - v.Height) / 2, bigger, v.Height), new RectangleF(0, 0, v.Width, v.Height), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            //g2.DrawImage(v, new System.Drawing.Point((bigger - v.Width) / 2, 0));
                            g2.DrawImage(v, new RectangleF((bigger - v.Width) / 2, 0, v.Width, v.Height), new RectangleF(0, 0, v.Width, v.Height), GraphicsUnit.Pixel);
                        }

                        Bitmap b2 = new Bitmap(Views[0]);
                        g2 = Graphics.FromImage(b2);

                        if (bigger > b2.Width)
                        {
                            int offset = (bigger - b2.Width) / 2;
                            g2.DrawImage(m3, new RectangleF(0, 0, b2.Width, b2.Height), new RectangleF(offset, offset, b2.Width, b2.Height), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            int offset = (b2.Width - bigger) / 2;
                            g2.DrawImage(m3, new RectangleF(offset, offset, v.Width, v.Height), new RectangleF(0, 0, v.Width, v.Height), GraphicsUnit.Pixel);
                        }


                        ImageHolder im = new ImageHolder(b2);

                        

                        Views[4] = im.ToBitmap();
                    }
                    catch
                    {
                    }

                    Bitmap wholeImage = new Bitmap(5 * Views[0].Width, Views[0].Height);
                    Graphics g = Graphics.FromImage(wholeImage);

                    int x = 0;
                    for (int i = 0; i < Views.Length; i++)
                    {
                        if (Views[i] != null)
                        {
                            g.DrawImage(Views[i], new System.Drawing.Point(x, 0));
                        }
                            x += Views[0].Width;
                        
                    }

                    g = null;
                    //wholeImage = Views[1];
                    // wholeImage = ImageViewer.ImagingTools.Invert(wholeImage);

                    pictureBox1.Image = wholeImage;
                    pictureBox1.Invalidate();
                    System.Windows.Forms.Application.DoEvents();

                    wholeImage.Save(@"C:\temp\temp.bmp");


                    Thread.Sleep(300);
                    try
                    {
                        rng.InsertBefore(experiment);
                        rng.InlineShapes.AddPicture(@"C:\temp\temp.bmp", ref missing, ref missing, ref missing);
                        // rng.InlineShapes.AddPicture(@"C:\Development\CellCT\temp2.bmp", ref missing, ref missing, ref missing);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);

                    }
                }
                catch { }
            }

            /* object filename = @"c:\MyWord.doc";
             oDoc.SaveAs(ref filename, ref missing, ref missing, ref missing, ref missing, ref missing,
             ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);*/

        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<string> Selected = new List<string>();
            for (int i = 0; i < lDataDirectories.Items.Count; i++)
            {
                if (lDataDirectories.GetSelected(i) == true)
                {
                    string pPath = GetDataInFolder() + (string)lDataDirectories.Items[i] + "\\";
                    Selected.Add(pPath);
                }
            }


            object oMissing = System.Reflection.Missing.Value;
            object missing = System.Reflection.Missing.Value;
            object Visible = true;
            object start1 = 0;
            object end1 = 0;

            Microsoft.Office.Interop.Word._Application oWord;
            Microsoft.Office.Interop.Word._Document oDoc;
            oWord = new Microsoft.Office.Interop.Word.Application();
            oWord.Visible = true;
            oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
                ref oMissing, ref oMissing);

            Range rng = oDoc.Range(ref start1, ref oMissing);

            oWord.Visible = true;

            string[] Files = Directory.GetFiles(Selected[0] + "\\data\\", "*.cct");
            foreach (string file in Files)
            {
                try
                {
                    PhysicalArray pa = PhysicalArray.OpenDensityData(file);


                    if (pa.ArrayRank == PhysicalArrayRank.Array3D && pa.GetLength(MathHelpLib.Axis.ZAxis) > 2)
                    {

                        double[, ,] Data = (double[, ,])pa.ActualData3D;

                        viewerControl3D1.SetImage(Data);
                        pa = null;
                        Bitmap[] Views = viewerControl3D1.GetVisibleImages();

                        Bitmap wholeImage = new Bitmap(Views[0].Width, Views[0].Height);
                        Graphics g = Graphics.FromImage(wholeImage);
                        g.DrawImage(Views[0], new System.Drawing.Point(0, 0));
                        g.DrawImage(Views[1], new System.Drawing.Point(Views[0].Width, 0));
                        g = null;
                        wholeImage = Views[1];
                        // wholeImage = ImageViewer.ImagingTools.Invert(wholeImage);

                        pictureBox1.Image = wholeImage;
                        pictureBox1.Invalidate();
                        System.Windows.Forms.Application.DoEvents();

                        wholeImage.Save(@"C:\Development\CellCT\temp.bmp");

                        // viewerControl3D1.SetContrastAndBrightness(.709, .906);
                        Views = viewerControl3D1.GetVisibleImages();
                        //Views[1] = ImageViewer.ImagingTools.Invert(Views[1]);
                        Graphics g2 = Graphics.FromImage(Views[2]);
                        System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 8);
                        Brush drawBrush = new SolidBrush(Color.Red);

                        g2.DrawString(Path.GetFileNameWithoutExtension(file), drawFont, drawBrush, new PointF(0, 20));
                        Views[2].Save(@"C:\Development\CellCT\temp2.bmp");
                        Thread.Sleep(300);
                        try
                        {
                            //rng.InsertBefore(experiment);
                            //rng.InlineShapes.AddPicture(@"C:\Development\CellCT\temp.bmp", ref missing, ref missing, ref missing);
                            rng.InlineShapes.AddPicture(@"C:\Development\CellCT\temp2.bmp", ref missing, ref missing, ref missing);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);

                        }
                    }
                }
                catch { }
            }

            /* object filename = @"c:\MyWord.doc";
             oDoc.SaveAs(ref filename, ref missing, ref missing, ref missing, ref missing, ref missing,
             ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);*/
        }
    }
}
