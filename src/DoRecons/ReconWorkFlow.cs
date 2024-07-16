using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathHelpLib;

namespace DoRecons
{
    public partial class ReconWorkFlow : UserControl
    {
        public ReconWorkFlow()
        {
            InitializeComponent();
        }

        public void SetFilter(Filtering.FilterTypes Filter)
        {
            string FBPWindow = Filter.ToString();
            for (int i = 0; i < lFBPWindowing.Items.Count; i++)
            {
                if (FBPWindow == lFBPWindowing.Items[i].ToString())
                    lFBPWindowing.SetSelected(i, true);
                //else
                //lFBPWindowing.SetSelected(i, false);
            }
        }

        public void SetupControl(Dictionary<string, string> PropertyList)
        {
            lFBPWindowing.Items.Clear();
            string[] filters = Enum.GetNames(typeof(Filtering.FilterTypes));
            for (int i = 0; i < filters.Length; i++)
                lFBPWindowing.Items.Add(filters[i]);
            lFBPWindowing.SetSelected(0, true);

            string COGMethod = "Threshold";
            string BackgroundSubMethod = "TopAndBottom";
            string PreprocessingMethod = "Median";
            string ProprocessingRadius = "3";
            string ReconMethod = "FBP";
            string FBPResolution = "512";
            string FBPWindow = "Han";
            string FBPMedian = "False";
            string ARTAngle = "72";
            string SaveCenteredImage = "False";
            string SaveCenteringMovie = "True";
            string Save8Bit = "False";
            string Save16Bit = "False";
            string SaveVolume = "True";
            string SaveAsRawDouble = "False";
            string SaveAsRawFloat = "False";
            string SaveAsRawInt = "True";
            string SaveAsCCT = "True";
            string SaveMIP = "True";
            string CopyStack = "True";
            string DoConvolutionQuality = "True";
            string FlatMethod = "Curve";
            string LoadPreProcessed = "False";
            string GlobalFlatten = "True";
            string FluorCell = "False";
            string SecondCell = "False";
            string SecondCellFolder = @"Y:\Fluor\cct001\201203\15\cct001_20120315_084709";
            string InterpolationMethod = "Siddon";

            if (PropertyList != null)
            {
                #region Get Values

                if (PropertyList.ContainsKey("FluorCell"))
                    FluorCell = PropertyList["FluorCell"];
                if (PropertyList.ContainsKey("SecondCell"))
                    SecondCell = PropertyList["SecondCell"];
                if (PropertyList.ContainsKey("SecondCellFolder"))
                    SecondCellFolder = PropertyList["SecondCellFolder"];
                if (PropertyList.ContainsKey("InterpolationMethod"))
                    InterpolationMethod = PropertyList["InterpolationMethod"];

                if (PropertyList.ContainsKey("LoadPreProcessed"))
                    FlatMethod = PropertyList["LoadPreProcessed"];

                if (PropertyList.ContainsKey("GlobalFlatten"))
                    FlatMethod = PropertyList["GlobalFlatten"];

                if (PropertyList.ContainsKey("FlatMethod"))
                    FlatMethod = PropertyList["FlatMethod"];

                if (PropertyList.ContainsKey("CopyStack"))
                    CopyStack = PropertyList["CopyStack"];

                if (PropertyList.ContainsKey("DoConvolutionQuality"))
                    DoConvolutionQuality = PropertyList["DoConvolutionQuality"];

                if (PropertyList.ContainsKey("COGMethod"))
                    COGMethod = PropertyList["COGMethod"];

                if (PropertyList.ContainsKey("BackgroundSubMethod"))
                    BackgroundSubMethod = PropertyList["BackgroundSubMethod"];

                if (PropertyList.ContainsKey("PreprocessingMethod"))
                    PreprocessingMethod = PropertyList["PreprocessingMethod"];

                if (PropertyList.ContainsKey("ProprocessingRadius"))
                    ProprocessingRadius = PropertyList["ProprocessingRadius"];

                if (PropertyList.ContainsKey("ReconMethod"))
                    ReconMethod = PropertyList["ReconMethod"];

                if (PropertyList.ContainsKey("FBPResolution"))
                    FBPResolution = PropertyList["FBPResolution"];

                if (PropertyList.ContainsKey("FBPWindow"))
                    FBPWindow = PropertyList["FBPWindow"];

                if (PropertyList.ContainsKey("FBPMedian"))
                    FBPMedian = PropertyList["FBPMedian"];

                if (PropertyList.ContainsKey("ARTAngle"))
                    ARTAngle = PropertyList["ARTAngle"];

                if (PropertyList.ContainsKey("SaveCenteredImage"))
                    SaveCenteredImage = PropertyList["SaveCenteredImage"];

                if (PropertyList.ContainsKey("SaveCenteringMovie"))
                    SaveCenteringMovie = PropertyList["SaveCenteringMovie"];

                if (PropertyList.ContainsKey("Save8Bit"))
                    Save8Bit = PropertyList["Save8Bit"];

                if (PropertyList.ContainsKey("Save16Bit"))
                    Save16Bit = PropertyList["Save16Bit"];

                if (PropertyList.ContainsKey("SaveVolume"))
                    SaveVolume = PropertyList["SaveVolume"];

                if (PropertyList.ContainsKey("SaveAsRawDouble"))
                    SaveAsRawDouble = PropertyList["SaveAsRawDouble"];

                if (PropertyList.ContainsKey("SaveAsRawFloat"))
                    SaveAsRawFloat = PropertyList["SaveAsRawFloat"];

                if (PropertyList.ContainsKey("SaveAsRawInt"))
                    SaveAsRawInt = PropertyList["SaveAsRawInt"];

                if (PropertyList.ContainsKey("SaveAsCCT"))
                    SaveAsCCT = PropertyList["SaveAsCCT"];

                if (PropertyList.ContainsKey("SaveMIP"))
                    SaveMIP = PropertyList["SaveMIP"];
                #endregion
            }

            #region Update Display

            if (FluorCell == "True")
                cbFluorCell.Checked = true;
            else
                cbFluorCell.Checked = false;

            if (SecondCell == "True")
                rbSecondCell.Checked = true;
            else
                rbSecondCell.Checked = true;

            tbSecondCellFolder.Text = SecondCellFolder;

            cbInterpolationMethod.Text = InterpolationMethod;

            if (LoadPreProcessed.ToLower() == "true")
                cbLoadPreprocessed.Checked = true;
            else
                cbLoadPreprocessed.Checked = false;

            if (GlobalFlatten.ToLower() == "true")
                cbGlobalFlat.Checked = true;
            else
                cbGlobalFlat.Checked = false;

            if (FlatMethod.ToLower() == "none")
                rFlatNone.Checked = true;
            else if (FlatMethod.ToLower() == "plane")
                rFlatPlane.Checked = true;
            else
                rFlatCurve.Checked = true;

            if (CopyStack.ToLower() == "true") cCopyStack.Checked = true; else cCopyStack.Checked = false;
            if (DoConvolutionQuality.ToLower() == "true") cConvolutionCheck.Checked = true; else cConvolutionCheck.Checked = false;

            if (COGMethod.ToLower() == "threshold")
                rThresholdCOG.Checked = true;
            else
                rMaskCOG.Checked = true;

            if (BackgroundSubMethod.ToLower() == "topandbottom")
                rCenteringTop.Checked = true;
            else if (BackgroundSubMethod.ToLower() == "strip")
                rCenteringStrips.Checked = true;
            else
                rCenteringMask.Checked = true;

            if (PreprocessingMethod.ToLower() == "none")
                rPPNone.Checked = true;
            else if (PreprocessingMethod.ToLower() == "median")
                rPPMedian.Checked = true;
            else if (PreprocessingMethod.ToLower() == "average")
                rPPAverage.Checked = true;
            else if (PreprocessingMethod.ToLower() == "alphatrimmed")
                rPPAlphaTrimmed.Checked = true;
            else if (PreprocessingMethod.ToLower() == "opening")
                rPPOpening.Checked = true;
            else if (PreprocessingMethod.ToLower() == "closing")
                rPPClosing.Checked = true;

            int PPR = 3;
            int.TryParse(ProprocessingRadius, out PPR);
            nPPRadius.Value = PPR;

            if (ReconMethod.ToLower() == "fbp")
                rFBP.Checked = true;
            else
                rART.Checked = true;

            int FBPRes = 512;
            int.TryParse(FBPResolution, out FBPRes);
            nFBPResolution.Value = FBPRes;


            for (int i = 0; i < lFBPWindowing.Items.Count; i++)
            {
                lFBPWindowing.SetSelected(i, false);
            }
            Application.DoEvents();
            for (int i = 0; i < lFBPWindowing.Items.Count; i++)
            {
                if (FBPWindow == lFBPWindowing.Items[i].ToString())
                    lFBPWindowing.SetSelected(i, true);
                //else
                    //lFBPWindowing.SetSelected(i, false);
            }
           // lFBPWindowing.SetSelected(0, true);

            double ARTang = 72;
            double.TryParse(ARTAngle, out ARTang);
            nARTAngle.Value = (decimal)ARTang;

            if (FBPMedian.ToLower() == "true") cFBPMedian.Checked = true; else cFBPMedian.Checked = false;
            if (SaveCenteredImage.ToLower() == "true") cSaveCenteredImages.Checked = true; else cSaveCenteredImages.Checked = false;
            if (SaveCenteringMovie.ToLower() == "true") cSaveCenteringMovie.Checked = true; else cSaveCenteringMovie.Checked = false;
            if (Save8Bit.ToLower() == "true") cSave8bit.Checked = true; else cSave8bit.Checked = false;
            if (Save16Bit.ToLower() == "true") cSave16bit.Checked = true; else cSave16bit.Checked = false;
            if (SaveAsRawDouble.ToLower() == "true") rVolumeRawDouble.Checked = true; else rVolumeRawDouble.Checked = false;
            if (SaveAsRawFloat.ToLower() == "true") rVolumeRawFloat.Checked = true; else rVolumeRawFloat.Checked = false;
            if (SaveAsRawInt.ToLower() == "true") rVolumeRawInt.Checked = true; else rVolumeRawInt.Checked = false;
            if (SaveAsCCT.ToLower() == "true") rVolumeRawCCT.Checked = true; else rVolumeRawCCT.Checked = false;
            if (SaveMIP.ToLower() == "true") cSaveMIP.Checked = true; else cSaveMIP.Checked = false;

            #endregion
        }

        public Dictionary<string, string> SaveGUI()
        {
            Dictionary<string, string> Saves = new Dictionary<string, string>();

            Saves.Add("FluorCell", cbFluorCell.Checked.ToString());
            Saves.Add("SecondCell", rbSecondCell.Checked.ToString());
            Saves.Add ("SecondCellFolder", tbSecondCellFolder.Text);
            Saves.Add("InterpolationMethod", cbInterpolationMethod.Text);

            if (rFlatNone.Checked == true)
                Saves.Add("FlatMethod", "None");
            else if (rFlatPlane.Checked == true)
                Saves.Add("FlatMethod", "Plane");
            else if (rFlatCurve.Checked ==true )
                Saves.Add("FlatMethod", "Curve");

            Saves.Add ( "LoadPreProcessed", (cbLoadPreprocessed.Checked ).ToString());
            Saves.Add ( "GlobalFlatten", (cbGlobalFlat.Checked ).ToString());


            if (rMaskCOG.Checked == true)
                Saves.Add("COGMethod", "Mask");
            else
                Saves.Add("COGMethod", "Threshold");


            if (rCenteringTop.Checked)
                Saves.Add("BackgroundSubMethod", "TopAndBottom");
            else if (rCenteringMask.Checked)
                Saves.Add("BackgroundSubMethod", "Mask");
            else
                Saves.Add("BackgroundSubMethod", "Strip");


            if (rPPNone.Checked)
                Saves.Add("PreprocessingMethod", "none");
            else if (rPPMedian.Checked)
                Saves.Add("PreprocessingMethod", "median");
            else if (rPPAverage.Checked)
                Saves.Add("PreprocessingMethod", "average");
            else if (rPPAlphaTrimmed.Checked)
                Saves.Add("PreprocessingMethod", "alphatrimmed");
            else if (rPPOpening.Checked)
                Saves.Add("PreprocessingMethod", "opening");
            else if (rPPClosing.Checked)
                Saves.Add("PreprocessingMethod", "closing");

            Saves.Add("ProprocessingRadius", nPPRadius.Value.ToString());

            if (rFBP.Checked == true)
                Saves.Add("ReconMethod", "FBP");
            else
                Saves.Add("ReconMethod", "ART");

            for (int i = 0; i < lFBPWindowing.Items.Count; i++)
            {
                if (lFBPWindowing.GetSelected(i))
                    Saves.Add("FBPWindow", lFBPWindowing.Items[i].ToString());
            }
            Saves.Add("FBPResolution", nFBPResolution.Value.ToString());
            Saves.Add("FBPMedian", cFBPMedian.Checked.ToString());
            Saves.Add("ARTAngle", nARTAngle.Value.ToString());


            Saves.Add("SaveCenteredImage", cSaveCenteredImages.Checked.ToString());
            Saves.Add("SaveCenteringMovie", cSaveCenteringMovie.Checked.ToString());
            Saves.Add("Save8Bit", cSave8bit.Checked.ToString());
            Saves.Add("Save16Bit", cSave16bit.Checked.ToString());
            Saves.Add("SaveAsRawDouble", rVolumeRawDouble.Checked.ToString());
            Saves.Add("SaveAsRawFloat", rVolumeRawFloat.Checked.ToString());
            Saves.Add("SaveAsRawInt", rVolumeRawInt.Checked.ToString());
            Saves.Add("SaveAsCCT", rVolumeRawCCT.Checked.ToString());
            Saves.Add("SaveMIP", cSaveMIP.Checked.ToString());

            Saves.Add("CopyStack", cCopyStack.Checked.ToString());
            Saves.Add("DoConvolutionQuality", cConvolutionCheck.Checked.ToString());
            return Saves;
        }

        private void cbLoadPreprocessed_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLoadPreprocessed.Checked)
            {
                groupBox11.Enabled = false;
            }
            else
                groupBox11.Enabled = true;
        }

    }
}
