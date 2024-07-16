using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageViewer3D._3DItems;
using System.Drawing;
using System.Drawing.Drawing2D;
using ImageViewer3D.Tools;
using System.Windows.Forms;

namespace ImageViewer3D
{

    public class DataEnvironment3D
    {
        public delegate void ContrastChangedEvent(double MinContrast, double MaxContrast);
        public event ContrastChangedEvent ContrastChanged;

        public DataEnvironment3D(ViewerControl3D Viewer)
        {
            mViewerControl = Viewer;
        }

        public ScreenProperties3D[] Screens;
        //setting both to zero causes the displaybox to do autocontrast
        public double MaxPossibleContrast = 0;
        public double MinPossibleContrast = 0;

        private double mMaxContrast = 0;
        private double mMinContrast = 0;

        public double MaxContrast
        {
            get { return mMaxContrast; }
            set
            {
                if (mMaxContrast != value)
                {
                    mMaxContrast = value;
                    try
                    {
                        RedrawBuffers();
                    }
                    catch { }

                }
            }
        }

        public double MinContrast
        {
            get { return mMinContrast; }
            set
            {
                if (mMinContrast != value)
                {
                    mMinContrast = value;
                    try
                    {
                        RedrawBuffers();
                    }
                    catch { }

                }
            }
        }

        public void AutoSetContrast()
        {
            double Sum = 0;
            double Count = 0;
            double Average = 0;
            double SD = 0;

            //OriginalData.Data.MaxMinArray(out MaxPossibleContrast,out  MinPossibleContrast) ;
            //double mid = (MaxPossibleContrast + MinPossibleContrast) / 2d;
            //double HWidth = MaxPossibleContrast - mid;

            MaxPossibleContrast = float.MinValue;// mid + HWidth * 4;
            MinPossibleContrast = float.MaxValue;// mid - HWidth * 4;
            unsafe
            {
                fixed (double* pData = OriginalData.Data)
                {
                    double* pIn = pData;
                    for (int i = 0; i < OriginalData.Data.Length; i += 20)
                    {
                        Sum += *pIn;
                        Count++;
                        if (*pIn > MaxPossibleContrast) MaxPossibleContrast = *pIn;
                        if (*pIn < MinPossibleContrast) MinPossibleContrast = *pIn;
                        pIn += 20;
                    }
                    //get Average
                    Average = Sum / Count;
                    Sum = 0;
                    Count = 0;
                    double d = 0;
                    pIn = pData;
                    for (int i = 0; i < OriginalData.Data.Length; i += 20)
                    {
                        d = *pIn - Average;
                        Sum += d * d;
                        Count++;
                        pIn += 20;
                    }
                    SD = Math.Sqrt(Sum / Count);
                }

            }

            double Brightness = (Average + MaxPossibleContrast) / 2d;
            double contrast = (MaxPossibleContrast - Average) / 2d;
            double reduction = .15;

            MaxContrast = Brightness + contrast * (1 - reduction);
            MinContrast = Brightness - contrast * (1 - reduction);

            if (ContrastChanged != null)
                ContrastChanged(MinContrast, MaxContrast);
        }


        public DataHolder OriginalData;

        public void RedrawBuffers()
        {
            for (int i = 0; i < Screens.Length; i++)
                Screens[i].RedrawBuffers();
        }

        public int MasterThreadId = 0;
        public List<int> ThreadsRunning = null;
        public List<string> WholeFileList = null;

        public Bitmap GetSlice(int Axis, int SliceNumber)
        {
            Rectangle View = ViewBox.AxisBounds(Axis);
            return OriginalData.GetSlice(Axis, SliceNumber, View, mMinContrast, mMaxContrast);
        }

        public double[,] GetSliceDouble(int Axis, int SliceNumber)
        {
            Rectangle View = ViewBox.AxisBounds(Axis);
            return OriginalData.GetSliceDouble(Axis, SliceNumber, View);
        }



        private ViewerControl3D mViewerControl;
        public ViewerControl3D ViewerControl
        {
            get { return mViewerControl; }
        }

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

        #region Zoom Controls
        private Box mFullSizeBox;
        public Box FullSize
        {
            get { return mFullSizeBox; }
        }
        private Box mViewBox;
        public Box ViewBox
        {
            get { return mViewBox; }
            set
            {
                mViewBox = value;
                RedrawBuffers();
            }
        }

        public void SetViewBoxSide(int Axis, Rectangle Bounds)
        {
            mViewBox.SetAxisBounds(Axis, Bounds);
            RedrawBuffers();
        }

        private bool mProportionalZoom = true;
        public bool ProportionalZoom
        {
            get { return mProportionalZoom; }
            set
            {
                mProportionalZoom = value;
                RedrawBuffers();
            }
        }

        private InterpolationMode mInterpolationMode = InterpolationMode.Default;
        public InterpolationMode InterpolationMode
        {
            get { return mInterpolationMode; }
            set
            {
                mInterpolationMode = value;
                RedrawBuffers();
            }
        }

        private bool mZooming = false;
        public bool Zooming
        {
            set
            {
                mZooming = value;
                RedrawBuffers();
            }
            get
            {
                return mZooming;
            }
        }

        public void ResetZoom()
        {
            mViewBox = new Box();
            mViewBox.X = 0;
            mViewBox.Y = 0;
            mViewBox.Z = 0;
            mViewBox.Width = OriginalData.Width;
            mViewBox.Height = OriginalData.Height;
            mViewBox.Depth = OriginalData.Depth;

            mFullSizeBox = new Box();
            mFullSizeBox.X = 0;
            mFullSizeBox.Y = 0;
            mFullSizeBox.Z = 0;
            mFullSizeBox.Width = OriginalData.Width;
            mFullSizeBox.Height = OriginalData.Height;
            mFullSizeBox.Depth = OriginalData.Depth;
        }

        #endregion

        #region Selection handling
        public delegate void SelectionPerfomedEventExtended(DataEnvironment3D SourceImage, ISelection3D Selection);
        public event SelectionPerfomedEventExtended SelectionPerformed;

        /// <summary>
        /// Notifies the user of the viewer control that a selection has been performed.
        /// </summary>
        /// <param name="Selection"></param>
        public void NotifyOfSelection(ISelection3D Selection)
        {
            mActiveSelection = Selection;
            mViewerControl.DoSelectionPerformed(Selection);
            if (SelectionPerformed != null)
            {
                SelectionPerformed(this, Selection);
            }
        }

        private ISelection3D mActiveSelection;

        /// <summary>
        /// This contains a reference to last selection performed.  
        /// </summary>
        public ISelection3D ActiveSelection
        {
            get { return mActiveSelection; }
            set { NotifyOfSelection(value); }
        }

        /// <summary>
        /// Gets/Sets the selected region of the image,  returns/sets the whole image if nothing is selected
        /// </summary>
        public DataHolder ActiveSelectedImage
        {
            get
            {
                if (mViewerControl.SelectedArea == null)
                {
                    return OriginalData;
                }
                else
                {
                    throw new Exception("Not yet implemented");
                    //return OriginalImage.Copy(mViewerControl.SelectedArea.SelectionBounds);
                    //return ImagingTools.ClipImageExactCopy(OriginalImage, mViewerControl.SelectedArea.SelectionBounds);
                }
            }
            set
            {
                if (value != null)
                {
                    if (mViewerControl.SelectedArea == null)
                    {
                        if (OriginalData == null || OriginalData.Data == null || (OriginalData.Width != value.Width || OriginalData.Height != value.Height ||
                            OriginalData.Depth != value.Depth))
                        {
                            OriginalData = value;
                            AutoSetContrast();
                            ResetZoom();
                        }
                        else
                            OriginalData = value;
                    }
                    else
                    {
                        throw new Exception("Not yet Implimented");
                        //perform a perfect copy if needed
                        // if (mViewerControl.SelectedArea.SelectionBounds.Width == v.Width && mViewerControl.SelectedArea.SelectionBounds.Height == v.Height)
                        {
                        }
                    }
                    //reset all the buffers
                    for (int i = 0; i < Screens.Length; i++)
                        Screens[i].RedrawBuffers();
                }
            }

        }
        #endregion
    }


}
