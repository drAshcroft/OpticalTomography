using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MathHelpLib;

namespace ImageViewer
{
    public class HistoryMonitor
    {
        private class HistoryMomento
        {
            private string ImageFilename;
            public Rectangle? ActiveArea;
            public ImageHolder RememberedImage
            {
                ///The bitmap must be closed before another process tries to access its file, so
                ///it is better to just clone it and let it close
                get
                {
                    ImageHolder b = new ImageHolder(ImageFilename);
                    ImageHolder b2 = b.Copy();
                    b.Dispose();
                    b = null;

                    return b2;
                }
            }
            /// <summary>
            /// Indicated which filter or tool generated the history momento
            /// </summary>
            public string momentoGenerator;
            public HistoryMomento(string Generator, Rectangle? activeArea, string ImageFilename)
            {
                ActiveArea = activeArea;
                this.ImageFilename = ImageFilename;
                momentoGenerator = Generator;
            }
        }

        private int mHistoryStepIndex = 0;
        private List<HistoryMomento[]> mHistoryList = new List<HistoryMomento[]>();

        public void PushMomento(string Generator, ScreenProperties Screens)
        {
            try
            {
                HistoryMomento[] MomentoPack = new HistoryMomento[1];
                string filename = Path.GetDirectoryName(Application.ExecutablePath) + "\\history\\Momento0_" + mHistoryStepIndex.ToString() + ".bmp";
                if (File.Exists(filename) == true)
                    File.Delete(filename);
                ((ImageHolder)Screens.ActiveSelectedImage).Save(filename);
                if (Screens.ActiveSelection != null)
                    MomentoPack[0] = new HistoryMomento(Generator, Screens.ActiveSelection.SelectionBounds, filename);
                else
                    MomentoPack[0] = new HistoryMomento(Generator, null, filename);
                mHistoryStepIndex++;
                if (mHistoryStepIndex >= mHistoryList.Count)
                    mHistoryList.Add(MomentoPack);
                else
                {
                    mHistoryList.RemoveRange(mHistoryStepIndex, mHistoryList.Count - mHistoryStepIndex);
                    mHistoryList.Insert(mHistoryStepIndex, MomentoPack);
                }
            }
            catch { }
        }

        public void StepForwardMomento(ScreenProperties Screens)
        {

            mHistoryStepIndex++;
            if (mHistoryStepIndex == mHistoryList.Count)
            {
                mHistoryStepIndex = mHistoryList.Count - 1;
                return;
            }

            HistoryMomento[] MomentoPack = mHistoryList[mHistoryStepIndex];


            if (MomentoPack[0].ActiveArea == null)
                Screens.NotifyOfSelection(null);
            else
                Screens.NotifyOfSelection(new Selections.ROISelection(MomentoPack[0].ActiveArea.Value, 0));
            Screens.ActiveSelectedImage = MomentoPack[0].RememberedImage;
        }

        public void StepBackMomento(ScreenProperties Screens)
        {

            //if this is the most current image, we must insert it into the stack or there is no momento to redo
            if (mHistoryStepIndex == mHistoryList.Count)
            {
                PushMomento("Undo", Screens);
                mHistoryStepIndex--;
            }


            mHistoryStepIndex--;
            if (mHistoryStepIndex < 0)
            {
                mHistoryStepIndex = 0;
                return;
            }
            HistoryMomento[] MomentoPack = mHistoryList[mHistoryStepIndex];

            
                if (MomentoPack[0].ActiveArea == null)
                    Screens.NotifyOfSelection(null);
                else
                    Screens.NotifyOfSelection(new Selections.ROISelection(MomentoPack[0].ActiveArea.Value, 0));
                Screens.ActiveSelectedImage = MomentoPack[0].RememberedImage;
            
        }
    }
}
