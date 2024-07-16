using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ImageViewer.Tools;
using MathHelpLib;

namespace ImageViewer
{
    public class ScreenProperties
    {
        private PictureDisplay mPictureBox;
        private ViewerControl mViewerControl;

        public ViewerControl ViewerControl
        {
            get { return mViewerControl; }
        }
        public ScreenProperties(PictureDisplay DisplayBox, ViewerControl viewerControl)
        {
            mPictureBox = DisplayBox;
            mViewerControl = viewerControl;
        }
        public PictureDisplay PictureBox
        {
            get { return mPictureBox; }
            set { mPictureBox = value; }
        }


        #region Selection handling
        public delegate void SelectionPerfomedEventExtended(ScreenProperties SourceImage, ISelection Selection);
        public event SelectionPerfomedEventExtended SelectionPerformed;

        /// <summary>
        /// Notifies the user of the viewer control that a selection has been performed.
        /// </summary>
        /// <param name="Selection"></param>
        public void NotifyOfSelection(ISelection Selection)
        {
            mActiveSelection = Selection;
            mViewerControl.DoSelectionPerformed(Selection);
            if (SelectionPerformed != null)
            {
                SelectionPerformed(this, Selection);
            }
        }

        /// <summary>
        /// Pushes a ROI selection onto the screen and into the registers.
        /// </summary>
        /// <param name="Tool">The desired selection tool</param>
        /// <param name="DesiredSelection">The outer bounds of the selection.  For fancier tools, the other information must be provided
        /// before calling this function</param>
        public void SimulateROISelection(aDrawingTool Tool, Rectangle DesiredSelection)
        {

            MouseEventArgs mea = new MouseEventArgs(MouseButtons.Left, 1, DesiredSelection.X, DesiredSelection.Y, 0);
            Tool.MouseDown(this, new Point(DesiredSelection.X, DesiredSelection.Y), mea);
            mea = new MouseEventArgs(MouseButtons.Left, 1, DesiredSelection.Right, DesiredSelection.Bottom, 0);
            Tool.MouseMove(new Point(DesiredSelection.Right, DesiredSelection.Bottom), mea);
            Tool.MouseUp(new Point(DesiredSelection.Right, DesiredSelection.Bottom), mea);
        }

        private ISelection mActiveSelection;

        /// <summary>
        /// This contains a reference to last selection performed.  
        /// </summary>
        public ISelection ActiveSelection
        {
            get { return mActiveSelection; }
            set { NotifyOfSelection(value); }
        }

        /// <summary>
        /// Gets/Sets the selected region of the image,  returns/sets the whole image if nothing is selected
        /// </summary>
        public object ActiveSelectedImage
        {
            get
            {
                if (mViewerControl.SelectedArea == null)
                {
                    return OriginalImage;
                }
                else
                {
                    return OriginalImage.Copy(mViewerControl.SelectedArea.SelectionBounds);
                    //return ImagingTools.ClipImageExactCopy(OriginalImage, mViewerControl.SelectedArea.SelectionBounds);
                }
            }
            set
            {
                if (value != null)
                {
                    ImageHolder image = ImageViewer.Filters.EffectHelps.FixImageFormat(value);
                    if (mViewerControl.SelectedArea == null)
                    {
                        OriginalImage = image;
                    }
                    else
                    {
                        //perform a perfect copy if needed
                        if (mViewerControl.SelectedArea.SelectionBounds.Width == image.Width && mViewerControl.SelectedArea.SelectionBounds.Height == image.Height)
                        {
                            OriginalImage.ROI = mViewerControl.SelectedArea.SelectionBounds ;
                            
                            image.CopyTo(OriginalImage);
                        }
                        //else
                            //throw new Exception("Needs to be implimented");
                    }
                    //reset all the buffers
                    RedrawBuffers();
                }
            }

        }
        #endregion

        #region Convertions

        public double ConvertScreenToImageX(double X)
        {
            return (X - VirtualPictureBox.Left) / (double)VirtualPictureBox.Width * (double)OriginalSize.Width;
        }
        public double ConvertScreenToImageY(double Y)
        {
            return (double)(Y - VirtualPictureBox.Top) / (double)VirtualPictureBox.Height * (double)OriginalSize.Height;
        }
        public Rectangle ConvertScreenToImage(Rectangle ScreenCoords)
        {
            //rescale the points to be inside the original coordinates
            double x1 = ConvertScreenToImageX((double)ScreenCoords.Left);//-mVirtualPictureBox.Left ) / (double)mVirtualPictureBox.Width * (double)mOriginalSize.Width;
            double x2 = ConvertScreenToImageX((double)ScreenCoords.Right);// - mVirtualPictureBox.Left) / (double)mVirtualPictureBox.Width * (double)mOriginalSize.Width;
            double y1 = ConvertScreenToImageY((double)ScreenCoords.Top);// - mVirtualPictureBox.Top) / (double)mVirtualPictureBox.Height * (double)mOriginalSize.Height;
            double y2 = ConvertScreenToImageY((double)ScreenCoords.Bottom);// - mVirtualPictureBox.Top) / (double)mVirtualPictureBox.Height * (double)mOriginalSize.Height;

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
        public Point ConvertPointFromScreenToImage(Point ScreenPoint)
        {
            double x = (double)(ScreenPoint.X - VirtualPictureBox.Left) / (double)VirtualPictureBox.Width * OriginalSize.Width;
            double y = (double)(ScreenPoint.Y - VirtualPictureBox.Top) / (double)VirtualPictureBox.Height * OriginalSize.Height;
            return new Point((int)x, (int)y);
        }

        /// <summary>
        /// Used when the image is zoomed.  the drawing tools use the screen coords, but effects use image coords.
        /// </summary>
        /// <param name="ScreenRect">The coords used on the screen</param>
        /// <returns></returns>
        public Rectangle ConvertRectangleFromScreenToImage(Rectangle ScreenRect)
        {
            double x = (double)(ScreenRect.X - VirtualPictureBox.Left) / (double)VirtualPictureBox.Width * OriginalSize.Width;
            double y = (double)(ScreenRect.Y - VirtualPictureBox.Top) / (double)VirtualPictureBox.Height * OriginalSize.Height;

            double w = (double)(ScreenRect.Width) / (double)VirtualPictureBox.Width * OriginalSize.Width;
            double h = (double)(ScreenRect.Height) / (double)VirtualPictureBox.Height * OriginalSize.Height;
            return new Rectangle((int)x, (int)y, (int)w, (int)h);
        }

        /// <summary>
        /// Used when the image is zoomed.  the drawing tools use the screen coords, but effects use image coords.
        /// </summary>
        /// <param name="ScreenRect">The coords used on the screen</param>
        /// <returns></returns>
        public int ConvertWidthFromScreenToImage(double ScreenWidth)
        {
            double w = (double)(ScreenWidth) / (double)VirtualPictureBox.Width * OriginalSize.Width;
            return (int)w;
        }

        #endregion

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

        /// <summary>
        /// This is a copy of the original image.  It should never be changed (without calling redrawbuffers)
        /// </summary>
        public ImageHolder OriginalImage;
        public Rectangle OriginalSize;

        /// <summary>
        /// Describes the size of the image if it was blown up as it would be by a zoom.  This is used to calculate all 
        /// the coordinates
        /// </summary>
        public Rectangle VirtualPictureBox;

        /// <summary>
        /// This is the area on the original image that is being shown on the screen.  a different view than the orginal
        /// can be caused by zooms or grabs.
        /// </summary>
        public Rectangle OriginalViewRectangle
        {
            get
            {
                double x = ConvertScreenToImageX(0);
                double y = ConvertScreenToImageY(0);
                double width = ConvertScreenToImageX((double)ScreenCoords.Width) - x;
                double height = ConvertScreenToImageY((double)ScreenCoords.Height) - y;
                return new Rectangle((int)x, (int)y, (int)width, (int)height);
            }
        }
        #endregion

        #region Zoom Controls
        public bool ProportionalZoom = true;
        public InterpolationMode InterpolationMode = InterpolationMode.Default;
        public bool Zooming
        {
            get
            {
                return ((VirtualPictureBox.Width == OriginalSize.Width) && (VirtualPictureBox.Height == OriginalSize.Height));
            }
        }
        #endregion

        #region Screen Drawing Code
        /// <summary>
        /// Cleans out the back buffers to make sure that the desired image is the only image
        /// </summary>
        public void CleanseScreen()
        {
            Graphics g = Graphics.FromImage(ScreenFrontBuffer);
            g.InterpolationMode = InterpolationMode;
            g.DrawImage(ScreenBackBuffer, ScreenCoords, ScreenCoords, GraphicsUnit.Pixel);
            mPictureBox.Invalidate();
        }

        Random rnd = new Random();
        /// <summary>
        /// This call should be used after a zoom or a grab to redraw all the buffers
        /// </summary>
        public void RedrawBuffers()
        {
            if (OriginalImage != null)
            {
                //reset the original size to mirror the size changes that some effects cause.
                if (VirtualPictureBox.Width == 0 || VirtualPictureBox.Height == 0 || OriginalSize.Width != OriginalImage.Width || OriginalSize.Height != OriginalImage.Height)
                    VirtualPictureBox = new Rectangle(0, 0, OriginalImage.Width, OriginalImage.Height);


                OriginalSize = new Rectangle(0, 0, OriginalImage.Width, OriginalImage.Height);

                Bitmap tOriginalImage = OriginalImage.ToBitmap();
                //draw the original image zoomed and panned as required
                Graphics g2 = Graphics.FromImage(ScreenFrontBuffer);
                g2.Clear(Color.Black);
                g2.InterpolationMode = InterpolationMode;
                Rectangle ViewBox = OriginalViewRectangle;
                g2.DrawImage(tOriginalImage, new Rectangle(0, 0, ScreenCoords.Width, ScreenCoords.Height), ViewBox, GraphicsUnit.Pixel);


                g2 = Graphics.FromImage(ScreenBackBuffer);
                g2.Clear(Color.Black);
                g2.InterpolationMode = InterpolationMode;
                g2.DrawImage(tOriginalImage, new Rectangle(0, 0, ScreenCoords.Width, ScreenCoords.Height), ViewBox, GraphicsUnit.Pixel);

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
            mPictureBox.SetImage(ScreenFrontBuffer);
            mPictureBox.Invalidate();
        }
        #endregion

        #region Macrohelps
        public void RegisterMacroListener(MacroLineGeneratedEvent Target)
        {
            mViewerControl.MacroLineGenerated += new MacroLineGeneratedEvent(Target);
        }
        public void RemoveMacroListener(MacroLineGeneratedEvent Target)
        {
            mViewerControl.MacroLineGenerated -= Target;
        }
        #endregion
    }
}
