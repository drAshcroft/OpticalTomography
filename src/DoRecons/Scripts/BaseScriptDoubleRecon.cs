using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ImageViewer;
using ImageViewer.PythonScripting;
using ImageViewer.Filters;
using ImageViewer.Filters.Blobs;
using System.Threading;
using MathHelpLib;
using System.IO;
using MathHelpLib.ProjectionFilters;
using ImageViewer.PythonScripting.Threads;
using Tomographic_Imaging_2;

using DoRecons.Scripts;

using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoRecons
{
    public class BaseScriptDouble : BaseScriptSingle
    {
        public virtual IScript CloneScript()
        {
            return new BaseScriptDouble();
        }

        public string GetName()
        {
            return "BaseScriptSingle";
        }

   
        protected override void PreBatchProcessCenter()
        {
            ReplaceStringDictionary PassData = new ReplaceStringDictionary();
            dataEnvironment.ProgressLog.AddSafe("Prep", "Center");

            BatchLoopThroughImages("FindFine");

            double tCellSize = 170;
            if (LopsidedCell)
            {
                tCellSize = CellSizes[0];
                for (int i = 0; i < CellSizes.Length; i++)
                {
                    if (CellSizes[i] > tCellSize) tCellSize = CellSizes[i];
                }
            }
            else
            {
                tCellSize = CellSizes[0];
                for (int i = 1; i < CellSizes.Length; i++)
                {
                    tCellSize += CellSizes[i];
                }
                tCellSize /= CellSizes.Length;
            }

            FineCellSize = (int)(tCellSize * 1.8) + 20;
            FineCellHalf = FineCellSize / 2;

            ReconCellSize = (int)(tCellSize * 1.3) + 20;
            ReconCellHalf = ReconCellSize / 2;


            dataEnvironment.ProgressLog.AddSafe("Position", "Centering Fit");
            //Center Cells
            ImageViewer.Filters.CenterCellsTool2Form Filter = new ImageViewer.Filters.CenterCellsTool2Form();

            //Parameters required: Bitmap_Filenames as string[], X_Positions as int[], Y_Positions as int[], SmoothingTypeX as string, X_Smooth_Param as int, SmootingTypeY as string, Y_Smooth_Param as int, ShowForm as string, CutSize as Size, OptionalOutputDir as string
           // Filter.DoEffect2(dataEnvironment, null, GlobalPassData, 100, X_Positions, Y_Positions, "MovingAverage", 5, "MovingAverage", 5, false, new Size(FineCellSize, FineCellSize), true, TempPath);
            //Data out of type :
          //  PassData = Filter.PassData;
//

            dataEnvironment.ProgressLog.AddSafe("Centering", "Centering Line Created");
            try
            {
                dataEnvironment.ProgressLog.AddSafe("CenteringQualityActual", PassData["CenterAccuracyActual"].ToString() + "%");
            }
            catch { }
            dataEnvironment.ProgressLog.AddSafe("CenteringQuality","100%"/* PassData["CenterAccuracy"].ToString() + "%"*/);


          //  X_Positions = (double[])PassData["CorrectedX"];
           // Y_Positions = (double[])PassData["CorrectedY"];

            ImageViewer.Filters.CenterCellsTool2Form.SaveCenters(dataEnvironment, X_Positions, Y_Positions);
            return;

            dataEnvironment.ProgressLog.AddSafe("Position", "Clipping");

            BatchLoopThroughImagesSave("ClipImages");

            dataEnvironment.ProgressLog.AddSafe("Clipping", "Images Clipped");

            dataEnvironment.ProgressLog.AddSafe("Cell Staining Average", Average(CellStain));
            dataEnvironment.ProgressLog.AddSafe("Cell Staining Variance", max(CellStain) - min(CellStain));

            if (ScriptParams["SaveCenteringMovie"].ToLower() == "true")
            {
                ImagingTools.CreateAVIVideo(DataPath + "Centering.avi", dataEnvironment.AllImages, 10);
            }
        }
     
        protected virtual void DoRun(Dictionary<string, object> Variables)
        {
            //ColorImage = false;

            //format loaded images (i.e. select only one channel) //now done inside find rough
            //  BatchLoopThroughImagesSave(6, dataEnvironment, ImageFileListIn, ScriptParams);

            CellWanderArea = new Rectangle(0,0, dataEnvironment.AllImages[1].Width, dataEnvironment.AllImages[1].Height);


            if ((Variables.ContainsKey("LoadPreProcessed") != true || (string)Variables["LoadPreProcessed"] == "False"))
            {
                FindFirstCell();
                FindCell();
                RemoveBackground();
               
                PreBatchProcessCenter();
            }
            return;
            //do any pre convolution work.  This is where most of the changes should be located
            BatchLoopThroughImagesSave("BeforeConvolution");

            PreBatchProcessRecon();
            PostBatchProcess();
        }
    }
}

