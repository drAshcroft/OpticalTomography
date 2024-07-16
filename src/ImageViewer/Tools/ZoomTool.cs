using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer.Tools
{
    public class ZoomTool : ROITool
    {
        public override string GetToolName()
        {
            return "Zoom";
        }
        public override string GetToolTip()
        {
            return "Zooms into a section of the graph,  Use right button to zoom out";
        }

        protected override void MouseUpImpl(Point MovePoint, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                base.MouseUpImpl(MovePoint, e);
                HandleSelectionDone(mSelectionBox);
            }
            if (e.Button == MouseButtons.Right)
            {
                mScreenProperties.VirtualPictureBox = new Rectangle(0, 0, mScreenProperties.OriginalSize.Width, mScreenProperties.OriginalSize.Height);
            }
            mScreenProperties.RedrawBuffers();
        }

        private void HandleSelectionDone(Rectangle SelectionBox)
        {
            //rescale the points to be inside the original coordinates
            double x1 = mScreenProperties.ConvertScreenToImageX((double)SelectionBox.Left);
            double x2 = mScreenProperties.ConvertScreenToImageX((double)SelectionBox.Right);
            double y1 = mScreenProperties.ConvertScreenToImageY((double)SelectionBox.Top);
            double y2 = mScreenProperties.ConvertScreenToImageY((double)SelectionBox.Bottom);


            double Width = (x2 - x1);
            double Height = (y2 - y1);

            double MagX = (double)mScreenProperties.ScreenCoords.Width / (double)Width;
            double MagY = (double)mScreenProperties.ScreenCoords.Height / (double)Height;

            Rectangle mZoomBox;
            if (mScreenProperties.ProportionalZoom)
            {
                double aspect = (double)mScreenProperties.OriginalSize.Width / (double)mScreenProperties.OriginalSize.Height;
                if (mScreenProperties.ScreenCoords.Width > mScreenProperties.ScreenCoords.Height)
                {
                    MagY = MagX;
                    mZoomBox = new Rectangle((int)x1, (int)y1, (int)Width, (int)(Width / aspect));
                }
                else
                {
                    MagX = MagY;
                    mZoomBox = new Rectangle((int)x1, (int)y1, (int)(Height * aspect), (int)(Height));
                }
            }
            else
            {
                mZoomBox = new Rectangle((int)x1, (int)y1, (int)(x2 - x1), (int)(y2 - y1));
            }

            double left = mZoomBox.X * MagX;
            double top = mZoomBox.Y * MagY;

            mScreenProperties.VirtualPictureBox.Width = (int)(mScreenProperties.OriginalSize.Width * MagX);
            mScreenProperties.VirtualPictureBox.Height = (int)(mScreenProperties.OriginalSize.Height * MagY);
            mScreenProperties.VirtualPictureBox.X = (int)(-1 * left);
            mScreenProperties.VirtualPictureBox.Y = (int)(-1 * top);

        }

        protected override void NotifyOfSelection()
        {
            mScreenProperties.NotifyOfSelection(null);
        }

        public override void RedrawSelection()
        {
            //base.RedrawSelection();
        }
    }
}
