using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ImageViewer3D.Tools;
namespace ImageViewer3D
{
    public class ScreenProperties3D
    {
        private PictureDisplay3DSlice mPictureBox;
        private ViewerControl3D mViewerControl;
        private DataEnvironment3D mDataEnvironment;
        private int mAxis;
        private int mSlice = 0;

        public ScreenProperties3D(DataEnvironment3D dataEnvironment, PictureDisplay3DSlice DisplayBox, ViewerControl3D viewerControl, int Axis)
        {
            this.mAxis = Axis;
            mDataEnvironment = dataEnvironment;
            mPictureBox = DisplayBox;
            mViewerControl = viewerControl;
        }

        public PictureDisplay3DSlice PictureBox
        {
            get { return mPictureBox; }
            set { mPictureBox = value; }
        }

        public DataEnvironment3D DataEnvironment
        {
            get { return mDataEnvironment; }
        }

        public int SliceIndex
        {
            get { return mSlice; }
            set
            {
                mSlice = value;
                RedrawBuffers();
            }
        }

        public int Axis
        {
            get { return mAxis; }
        }
        /// <summary>
        /// Pushes a ROI selection onto the screen and into the registers.
        /// </summary>
        /// <param name="Tool">The desired selection tool</param>
        /// <param name="DesiredSelection">The outer bounds of the selection.  For fancier tools, the other information must be provided
        /// before calling this function</param>
        public void SimulateROISelection(aDrawingTool3D Tool, Rectangle DesiredSelection)
        {
            MouseEventArgs mea = new MouseEventArgs(MouseButtons.Left, 1, DesiredSelection.X, DesiredSelection.Y, 0);
            Tool.MouseDown(this, new Point(DesiredSelection.X, DesiredSelection.Y), mea);
            mea = new MouseEventArgs(MouseButtons.Left, 1, DesiredSelection.Right, DesiredSelection.Bottom, 0);
            Tool.MouseMove(new Point(DesiredSelection.Right, DesiredSelection.Bottom), mea);
            Tool.MouseUp(new Point(DesiredSelection.Right, DesiredSelection.Bottom), mea);
        }

        /// <summary>
        /// reads in the reported mousecoords and returns the screen coords
        /// </summary>
        /// <param name="MouseCoords">Coords reported by a mouseevent</param>
        /// <returns></returns>
        public Point MouseCoordsToScreenCoords(Point MouseCoords)
        {
            return mPictureBox.PointToScreen(MouseCoords);
        }

        /// <summary>
        /// This is the dimensions of the picturebox that is the eventual display of the picture.  
        /// </summary>
        public Rectangle ScreenCoords
        {
            get
            {
                return mPictureBox.Bounds;
            }
        }

        #region Buffers
        /// <summary>
        /// A Copy of the original blown up as it should be for the current zoom.  You should not change this image unless it
        /// is a copy from the original image
        /// </summary>
        public Bitmap ScreenBackBuffer;

        /// <summary>
        /// This is the actual drawing surface.  It should be the only surface that is changed for drawing selection boxes
        /// </summary>
        public Bitmap ScreenFrontBuffer;



        #endregion

        #region Screen Drawing Code
        /// <summary>
        /// Cleans out the back buffers to make sure that the desired image is the only image
        /// </summary>
        public void CleanseScreen()
        {
            Graphics g = Graphics.FromImage(ScreenFrontBuffer);
            g.InterpolationMode = mDataEnvironment.InterpolationMode;
            g.DrawImage(ScreenBackBuffer, ScreenCoords, ScreenCoords, GraphicsUnit.Pixel);
            mPictureBox.Invalidate();
        }
        public Rectangle GetViewRectangle()
        {
            return mDataEnvironment.ViewBox.AxisBounds(mAxis);
        }

        public void SetViewBounds(Rectangle NewBounds)
        {
            mDataEnvironment.SetViewBoxSide(mAxis, NewBounds);
        }

        Random rnd = new Random();
        /// <summary>
        /// This call should be used after a zoom or a grab to redraw all the buffers
        /// </summary>
        public void RedrawBuffers()
        {
            if (mDataEnvironment != null)
            {
                Rectangle ViewBox = GetViewRectangle();
                Bitmap OriginalImage = mDataEnvironment.GetSlice(mAxis, mSlice);

                if (ScreenBackBuffer == null)
                {
                    ScreenBackBuffer = new Bitmap(mPictureBox.Width, mPictureBox.Height, PixelFormat.Format32bppRgb);
                }
                if (ScreenFrontBuffer == null)
                {
                    ScreenBackBuffer = new Bitmap(mPictureBox.Width, mPictureBox.Height, PixelFormat.Format32bppRgb);
                }

                if (mDataEnvironment.Zooming == true)
                {
                    //draw the original image zoomed and panned as required
                    Graphics g2 = Graphics.FromImage(ScreenFrontBuffer);
                    g2.Clear(Color.Black);
                    g2.InterpolationMode = mDataEnvironment.InterpolationMode;
                    g2.DrawImage(OriginalImage, new Rectangle(0, 0, ScreenCoords.Width, ScreenCoords.Height), ViewBox, GraphicsUnit.Pixel);

                    ScreenBackBuffer = new Bitmap(ScreenFrontBuffer);

                    mPictureBox.UnZoomedImage = ScreenBackBuffer;
                }
                else
                {
                    //draw the original image zoomed and panned as required
                    Graphics g2 = Graphics.FromImage(ScreenFrontBuffer);
                    g2.Clear(Color.Black);
                    g2.InterpolationMode = mDataEnvironment.InterpolationMode;
                    g2.DrawImage(OriginalImage, new Point(0, 0));

                    ScreenBackBuffer = new Bitmap(ScreenFrontBuffer);
                    mPictureBox.UnZoomedImage = OriginalImage;
                }
                //force a redraw of the image.
                mPictureBox.Image = ScreenFrontBuffer;
                mPictureBox.Invalidate();
                mPictureBox.Refresh();

                mViewerControl.RedrawToolSelection();
            }
        }

        /// <summary>
        /// Forces the displays to redraw the buffered images
        /// </summary>
        public void ForceImageDisplay()
        {
            mPictureBox.SetImage();

        }
        #endregion

        #region Convertions

        public int  ConvertScreenToImageX(double X)
        {
             Rectangle ViewBox = GetViewRectangle();
             if (mDataEnvironment.Zooming)
             {
                 return (int)((X / ScreenCoords.Width) * ViewBox.Width + ViewBox.X);
             }
             else
             {
                 return (int)(X+ViewBox.X);
             }
        }

        public int  ConvertScreenToImageY(double Y)
        {
            Rectangle ViewBox = GetViewRectangle();
            if (mDataEnvironment.Zooming)
            {
                return (int)( (Y / ScreenCoords.Height) * ViewBox.Height + ViewBox.Y);
            }
            else
            {
                return (int)( Y + ViewBox.Y);
            }
        }

        public Rectangle ConvertScreenToImage(Rectangle ScreenCoords)
        {
            //rescale the points to be inside the original coordinates
            double x1 = ConvertScreenToImageX((double)ScreenCoords.Left);
            double x2 = ConvertScreenToImageX((double)ScreenCoords.Right);
            double y1 = ConvertScreenToImageY((double)ScreenCoords.Top);
            double y2 = ConvertScreenToImageY((double)ScreenCoords.Bottom);

            return new Rectangle((int)x1, (int)y1, (int)(x2 - x1), (int)(y2 - y1));

        }
        public Point ConvertScreenToImage(Point ScreenCoords)
        {
            //rescale the points to be inside the original coordinates
            double x1 = ConvertScreenToImageX((double)ScreenCoords.X);
            double y1 = ConvertScreenToImageY((double)ScreenCoords.Y);

            return new Point((int)x1, (int)y1);
        }

        /// <summary>
        /// Used when the image is zoomed.  the drawing tools use the screen coords, but effects use image coords.
        /// </summary>
        /// <param name="ScreenRect">The coords used on the screen</param>
        /// <returns></returns>
        public int ConvertWidthFromScreenToImage(double ScreenWidth)
        {
            Rectangle ViewBox = GetViewRectangle();
            double w = (double)ViewBox.Width / (double)ScreenCoords.Width * ScreenWidth;
            return (int)w;
        }

        #endregion

    }
}
