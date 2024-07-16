using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

using ImageViewer.Filters;
using ImageViewer;
using MathHelpLib;
using MathHelpLib.ProjectionFilters;
using ImageViewer.Filters.Blobs;

namespace MathHelpLib.ProjectionFilters
{
    public class ReconQualityCheckTool : aEffectNoForm
    {
        public override string EffectName { get { return "Recon Quality Check vs Stack"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Statistics"; } }

        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        public override bool PassesPassData
        {
            get
            {
                return false;
            }
        }
        private static object CriticalSectionLock = new object();

        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mDataEnvironment = dataEnvironment;
            mFilterToken = Parameters;
            mPassData = PassData;
            if (mFilterToken == null)
                mFilterToken = DefaultProperties;

            //try
            {
                double[,] GrayImage = null;

                if (Parameters[0].GetType() == typeof(Bitmap))
                {
                    GrayImage = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(((Bitmap)SourceImage), false);
                }
                else if (Parameters[0].GetType() == typeof(ImageHolder))
                {
                    GrayImage = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray((ImageHolder)SourceImage, false);
                }
                else if (Parameters[0].GetType() == typeof(double[, ,]))
                {
                    double[, ,] Data3D = (double[, ,])Parameters[0];
                    GrayImage = new double[Data3D.GetLength(0), Data3D.GetLength(1)];
                    for (int i = 0; i < Data3D.GetLength(0); i++)
                        for (int j = 0; j < Data3D.GetLength(1); j++)
                        {
                            GrayImage[i, j] = Data3D[i, j, Data3D.GetLength(2) / 2];
                        }
                }
                else if (Parameters[0].GetType() == typeof(ProjectionArrayObject))
                {
                    ProjectionArrayObject Data3DPO = (ProjectionArrayObject)Parameters[0];
                    // GrayImage = Data3D.Data[Data3D.Data.GetLength(0) / 2];

                    double[][,] Data3D = Data3DPO.Data;

                    int HX = Data3D.GetLength(0) / 2;
                    int HY = Data3D[0].GetLength(0) / 2;
                    int HZ = Data3D[0].GetLength(1) / 2;


                    GrayImage = new double[Data3D.GetLength(0), Data3D[0].GetLength(0)];// Data3D[Data3D.GetLength(0) / 2];
                    int Width = GrayImage.GetLength(0) - 1;
                    for (int x = 0; x < GrayImage.GetLength(0); x++)
                        for (int y = 0; y < GrayImage.GetLength(1); y++)
                        {
                            GrayImage[Width - x, y] = Data3D[x][y, HX];
                        }

                }
                else if (Parameters[0].GetType() == typeof(double[][,]))
                {
                    double[][,] Data3D = (double[][,])Parameters[0];

                    int HX = Data3D.GetLength(0) / 2;
                    int HY = Data3D[0].GetLength(0) / 2;
                    int HZ = Data3D[0].GetLength(1) / 2;

                    GrayImage = new double[Data3D.GetLength(0), Data3D[0].GetLength(0)];// Data3D[Data3D.GetLength(0) / 2];
                    int Width = GrayImage.GetLength(0) - 1;
                    for (int x = 0; x < GrayImage.GetLength(0); x++)
                        for (int y = 0; y < GrayImage.GetLength(1); y++)
                        {
                            GrayImage[Width - x, y] = Data3D[x][y, HX];
                        }
                }

                bool dipp = false;
                double innerclip = 0;
                double outerclip = 1;

                if (Parameters.Length > 3)
                {
                    dipp = (bool)Parameters[3];
                    if (dipp == true)
                    {
                        innerclip = EffectHelps.ConvertToDouble(Parameters[4]);
                        outerclip = EffectHelps.ConvertToDouble(Parameters[5]);
                    }
                }
                bool ColorImage = true;
                try
                {
                    ColorImage = mPassData["IsColor"].ToString().ToLower() == "true";
                }
                catch { }
                double[] Quality = ImageQuality(GrayImage, ColorImage, (string)Parameters[1], dipp, innerclip, outerclip,mPassData );

                mPassData.AddSafe("QualityValue", Quality[0]);
                try
                {
                    mPassData.AddSafe("QualityValueLowerHalf", Quality[1]);
                    mPassData.AddSafe("QualityValueUpperHalf", Quality[2]);
                    mPassData.AddSafe("QualityValueLowerThird", Quality[3]);
                    mPassData.AddSafe("QualityValueUpperQuarter", Quality[4]);
                    mPassData.AddSafe("QualityValueLowerThirdReo", Quality[5]);
                    mPassData.AddSafe("QualityValueUpperQuarterReo", Quality[6]);

                    mPassData.AddSafe("QualityValueFreq", Quality[7]);
                    mPassData.AddSafe("QualityValueLowerHalfFreq", Quality[8]);
                    mPassData.AddSafe("QualityValueUpperHalfFreq", Quality[9]);
                    mPassData.AddSafe("QualityValueLowerThirdFreq", Quality[10]);
                    mPassData.AddSafe("QualityValueUpperQuarterFreq", Quality[11]);
                    mPassData.AddSafe("QualityValueLowerThirdReoFreq", Quality[12]);
                    mPassData.AddSafe("QualityValueUpperQuarterReoFreq", Quality[13]);
                }
                catch { }
            }
            return SourceImage;
        }


        public static double[] ImageQuality(double[,] slice, bool ColorImage, string StackFolder, bool dipp, double innerclip, double outerclip, ReplaceStringDictionary outPassData )
        {

            double intensityslice = 0;
            double MaxIntensitySlice = 0;
            double d;

            for (int i = 0; i < slice.GetLength(0); i++)
                for (int j = 0; j < slice.GetLength(1); j++)
                {
                    d = slice[i, j];
                    intensityslice += d;

                }

            intensityslice = intensityslice / slice.GetLength(0) / slice.GetLength(1);

            double sdSlice = 0;
            for (int i = 0; i < slice.GetLength(0); i++)
                for (int j = 0; j < slice.GetLength(1); j++)
                {
                    d = (intensityslice - slice[i, j]);
                    sdSlice += d * d;
                }

            sdSlice = Math.Sqrt(sdSlice / slice.GetLength(0) / slice.GetLength(1));
            slice.SubtractInPlace(intensityslice);
            slice.DivideInPlace(sdSlice);
            slice = MathHelpLib.ImageProcessing.MathImageHelps.FlattenImageEdges(slice);

            for (int i = 0; i < slice.GetLength(0); i++)
                for (int j = 0; j < slice.GetLength(1); j++)
                {
                    d = slice[i, j];
                    MaxIntensitySlice += Math.Abs(d);
                }

            MaxIntensitySlice = Math.Sqrt(MaxIntensitySlice);

            StackFolder = StackFolder + "stack\\000\\";

            string[] FileNames = Directory.GetFiles(StackFolder, "*.bmp");

            if (FileNames == null || FileNames.Length < 2)
                FileNames = Directory.GetFiles(StackFolder, "*.png");

            if (FileNames == null || FileNames.Length < 2)
                FileNames = Directory.GetFiles(StackFolder, "*.jpeg");

            if (FileNames == null || FileNames.Length < 2)
                FileNames = Directory.GetFiles(StackFolder, "*.ivg");
            Array.Sort(FileNames);

            string[] FileNames2 = new string[FileNames.Length - 2];
            for (int i = 2; i < FileNames.Length; i++)
                FileNames2[i - 2] = FileNames[i];

            double m = Math.Floor((double)(FileNames2.Length / 2));
            int r = 0;

            string[] Files = new string[FileNames2.Length];

            while (r < FileNames2.Length)
            {
                Files[(int)m] = FileNames2[r];
                r++;
                m += Math.Pow((-1), r + 1) * r;
            }

            ImageHolder BitmapImage = MathHelpsFileLoader.Load_Bitmap(Files[Files.Length / 2]);


            if (ColorImage == true)
            {
                BitmapImage =  MathHelpsFileLoader. FixVisionGateImage(BitmapImage, 3);
            }

            ImageViewer.Filters.ReplaceStringDictionary PassData = new ReplaceStringDictionary();
            IEffect Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdEffect();
            BitmapImage = (ImageHolder)Filter.DoEffect(null, BitmapImage, PassData);

            //WaterShed
            Filter = new ImageViewer.Filters.Blobs.WaterShedTool();
            BitmapImage = (ImageHolder)Filter.DoEffect(null, BitmapImage, PassData);
            //Data out of type :
            PassData = Filter.PassData;

            //Get Biggest Blob
            Filter = new ImageViewer.Filters.Blobs.GetBiggestCenterBlob();
            BitmapImage = (ImageHolder)Filter.DoEffect(null, BitmapImage, PassData, PassData["Blobs"], false);
            //Data out of type :
            PassData = Filter.PassData;
            BlobDescription Rect = (BlobDescription)PassData["MaxBlob"];
            int x = Rect.CenterOfGravity.X;
            int y = Rect.CenterOfGravity.Y;

            int CellSize = (int)(slice.GetLength(0) * 1);
            double CellHalf = CellSize / 2;


            Rectangle CellArea = new Rectangle((int)Math.Truncate(x - CellHalf), (int)Math.Truncate(y - CellHalf), CellSize, CellSize);

            double[][] maxcorre = new double[14][];//[Files.Length];
            for (int i = 0; i < maxcorre.Length; i++)
            {
                maxcorre[i] = new double[Files.Length];
            }
            for (int q = 0; q < Files.Length; q += 2)
            {
                ImageHolder a = MathHelpLib.MathHelpsFileLoader . Load_Bitmap(Files[q]);

                if (ColorImage == true)
                {
                    a =  MathHelpLib.MathHelpsFileLoader. FixVisionGateImage(a, 2);
                }

                try
                {
                    //Clip Image to New Image
                    Filter = new ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect();
                    //Parameters required: Clip Bounds as Rectangle
                    a = (ImageHolder)Filter.DoEffect(null, a, PassData, CellArea);

                    a.Invert();

                    double[,] stackimag = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(a, false);

                    double intensitystack = 0;
                    double MaxIntensityStack = 0;
                    double sdStack = 0;
                    for (int i = 0; i < stackimag.GetLength(0); i++)
                        for (int j = 0; j < stackimag.GetLength(1); j++)
                        {
                            d = stackimag[i, j];
                            intensitystack += d;

                        }

                    intensitystack = intensitystack / stackimag.GetLength(0) / stackimag.GetLength(1);

                    for (int i = 0; i < stackimag.GetLength(0); i++)
                        for (int j = 0; j < stackimag.GetLength(1); j++)
                        {
                            d = intensitystack - stackimag[i, j];
                            sdStack += d * d;
                        }

                    sdStack = Math.Sqrt(sdStack / stackimag.GetLength(0) / stackimag.GetLength(1));

                    stackimag.SubtractInPlace(intensitystack);
                    stackimag.DivideInPlace(sdStack);
                    stackimag = MathHelpLib.ImageProcessing.MathImageHelps. FlattenImageEdges(stackimag);

                    for (int i = 0; i < stackimag.GetLength(0); i++)
                        for (int j = 0; j < stackimag.GetLength(1); j++)
                        {
                            d = stackimag[i, j];
                            MaxIntensityStack += Math.Abs(d);
                        }
                    MaxIntensityStack = Math.Sqrt(MaxIntensityStack);

                    double[,] correlation;
                    double sumFreq = 0;
                    if (dipp == true)
                        correlation = MathFFTHelps.CrossCorrelationFFTFiltered(stackimag, slice, innerclip, outerclip, out sumFreq);
                    else
                        correlation = MathFFTHelps.CrossCorrelationFFT(stackimag, slice);
                    double maxa = MathArrayHelps. MaxArray(correlation);
                    maxcorre[0][q] = Math.Sqrt(maxa) / MaxIntensityStack / MaxIntensitySlice * 1000;
                    maxcorre[7][q] = sumFreq;

                    try
                    {
                        sumFreq = 0;
                        correlation = MathFFTHelps.CrossCorrelationFFTFiltered(stackimag, slice, .25, .75, out sumFreq);
                        maxa = MathArrayHelps.MaxArray(correlation);
                        maxcorre[1][q] = Math.Sqrt(maxa) / MaxIntensityStack / MaxIntensitySlice * 1000;
                        maxcorre[8][q] = sumFreq;
                    }
                    catch { }


                    try
                    {
                        sumFreq = 0;
                        correlation = MathFFTHelps.CrossCorrelationFFTFiltered(stackimag, slice, .5, .95, out sumFreq);
                        maxa = MathArrayHelps.MaxArray(correlation);
                        maxcorre[2][q] = Math.Sqrt(maxa) / MaxIntensityStack / MaxIntensitySlice * 1000;
                        maxcorre[9][q] = sumFreq;
                    }
                    catch { }

                    try
                    {
                        sumFreq = 0;
                        correlation = MathFFTHelps.CrossCorrelationFFTFiltered(stackimag, slice, .3, .6, out sumFreq);
                        maxa = MathArrayHelps.MaxArray(correlation);
                        maxcorre[3][q] = Math.Sqrt(maxa) / MaxIntensityStack / MaxIntensitySlice * 1000;
                        maxcorre[10][q] = sumFreq;
                    }
                    catch { }

                    try
                    {
                        sumFreq = 0;
                        correlation = MathFFTHelps.CrossCorrelationFFTFiltered(stackimag, slice, .75, .95, out sumFreq);
                        maxa = MathArrayHelps.MaxArray(correlation);
                        maxcorre[4][q] = Math.Sqrt(maxa) / MaxIntensityStack / MaxIntensitySlice * 1000;
                        maxcorre[11][q] = sumFreq;
                    }
                    catch { }

                    try
                    {
                        sumFreq = 0;
                        correlation = MathFFTHelps.CrossCorrelationFFTFilteredReordered(stackimag, slice, .3, .6, out sumFreq);
                        maxa = MathArrayHelps.MaxArray(correlation);
                        maxcorre[5][q] = Math.Sqrt(maxa) / MaxIntensityStack / MaxIntensitySlice * 1000;
                        maxcorre[12][q] = sumFreq;
                    }
                    catch { }

                    try
                    {
                        sumFreq = 0;
                        correlation = MathFFTHelps.CrossCorrelationFFTFilteredReordered(stackimag, slice, .75, .95, out sumFreq);
                        maxa = MathArrayHelps.MaxArray(correlation);
                        maxcorre[6][q] = Math.Sqrt(maxa) / MaxIntensityStack / MaxIntensitySlice * 1000;
                        maxcorre[13][q] = sumFreq;
                    }
                    catch { }

                    stackimag = null;
                    correlation = null;
                }
                catch { }
            }

            double[] maxcorr = new double[maxcorre.Length];
            for (int i = 0; i < maxcorr.Length; i++)
            {
                maxcorr[i] = maxcorre[i].Max();
            }

            if (outPassData != null)
            {
                for (int i = 0; i < maxcorre[0].Length; i++)
                {
                    if (maxcorr[0] == maxcorre[0][i])
                    {
                        ImageHolder a = MathHelpsFileLoader.Load_Bitmap(Files[i]);
                        if (ColorImage == true)
                        {
                            a = MathHelpsFileLoader.FixVisionGateImage(a, 2);
                        }
                        try
                        {
                            //Clip Image to New Image
                            Filter = new ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect();
                            //Parameters required: Clip Bounds as Rectangle
                            a = (ImageHolder)Filter.DoEffect(null, a, PassData, CellArea);
                            a.Invert();
                            outPassData.AddSafe("StackExample", a);
                            outPassData.AddSafe("StackBounds", CellArea);
                        }
                        catch { }
                    }
                }
            }

            return maxcorr;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataPath"></param>
        /// <param name="PositionPercent">middle image is .5</param>
        /// <returns></returns>
        private static  double[, ,] LoadVGRecon(string DataPath)
        {
            Console.WriteLine(DataPath);
            string dirName;
            
            if (DataPath.EndsWith ("\\"))
                dirName = Path.GetFileNameWithoutExtension( Path.GetDirectoryName(DataPath ));
            else 
                dirName = Path.GetFileNameWithoutExtension(DataPath );
            Console.WriteLine(dirName);
            string[] parts = dirName.Split(new string[]{"_"},StringSplitOptions.RemoveEmptyEntries );
            string Prefix = parts[0];
            string Year = parts[1].Substring(0, 4);
            string month = parts[1].Substring(4, 2);
            string day = parts[1].Substring(6, 2);

            string basePath = "Y:\\" + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName + "\\500PP\\recon_cropped_16bit\\";

            string[] Slices = Directory.GetFiles(basePath);
            List<string> GoodFiles = new List<string>();
            foreach (string s in Slices)
                if (Path.GetExtension(s).ToLower().Trim() != ".vvi")
                    GoodFiles.Add(s);

            Slices = EffectHelps.SortNumberedFiles(GoodFiles.ToArray());

            return (double[, ,])PhysicalArray.OpenDensityData(Slices).ActualData3D;
        }

        public static void CompareProjection(double[, ,] densityData, string basePath, ReplaceStringDictionary PassData)
        {
            double[,] Slice = new double[densityData.GetLength(2), densityData.GetLength(1)];

            int HX = densityData.GetLength(0) / 2;
            int HY = densityData.GetLength(1) / 2;
            int HZ = densityData.GetLength(2) / 2;

            for (int x = 0; x < Slice.GetLength(0); x++)
                for (int y = 0; y < Slice.GetLength(1); y++)
                {
                    Slice[Slice.GetLength(0) - x - 1, y] = densityData[HX, y, x];
                }

            HX = 5;
            double[,] Background = new double[densityData.GetLength(2), densityData.GetLength(1)];
            for (int x = 0; x < Slice.GetLength(0); x++)
                for (int y = 0; y < Slice.GetLength(1); y++)
                {
                    Background[Slice.GetLength(0) - x - 1, y] = densityData[HX, y, x];
                }


            HX = (int)(densityData.GetLength(0) * .10);
            double[,] Background25 = new double[densityData.GetLength(2), densityData.GetLength(1)];
            for (int x = 0; x < Slice.GetLength(0); x++)
                for (int y = 0; y < Slice.GetLength(1); y++)
                {
                    Background25[Slice.GetLength(0) - x - 1, y] = densityData[HX, y, x];
                }

            double[,] VGExample = null;
            double[,] VGBackGround = null;
            double[,] VGBackGround25 = null;

            try
            {
                double[, ,] VGVolume = LoadVGRecon(basePath);

                int LX = VGVolume.GetLength(0);
                int LY = VGVolume.GetLength(1);
                int LZ = VGVolume.GetLength(2);


                VGExample = new double[LX, LZ];

                HX = VGVolume.GetLength(0) / 2;
                HY = VGVolume.GetLength(1) / 2;
                HZ = VGVolume.GetLength(2) / 2;



                for (int x = 0; x < LX; x++)
                    for (int z = 0; z < LZ; z++)
                    {
                        VGExample[x, z] = VGVolume[x, HY, z];
                    }

                HY = 5;
                VGBackGround = new double[LX, LZ];
                for (int x = 0; x < LX; x++)
                    for (int z = 0; z < LZ; z++)
                    {
                        VGBackGround[x, z] = VGVolume[x, HY, z];
                    }


                HY = (int)(LY * .10);
                VGBackGround25 = new double[LX, LZ];
                for (int x = 0; x < LX; x++)
                    for (int z = 0; z < LZ; z++)
                    {
                        VGBackGround25[x, z] = VGVolume[x, HY, z];
                    }

            }
            catch
            {
                VGExample = new double[Slice.GetLength(0), Slice.GetLength(1)];
                VGBackGround = new double[Slice.GetLength(0), Slice.GetLength(1)];
            }

         

            Bitmap BestMatch = null;
           
            double[] Fits = MathHelpLib.ProjectionFilters.ReconQualityCheckTool.ImageQualityWholeTest(Slice, true, basePath + "\\", null, false, 0, 1, out BestMatch, Background, VGExample, Background25, VGBackGround, VGBackGround25, ref PassData);

        
        }

        public static double MostProbableValue(double[,] imag1)
        {
            double max = imag1.MaxArray();
            double min = imag1.MinArray();

            double Bins = 300;
            double[] hist1 = new double[(int)Bins + 1];
            double step = (max - min) / hist1.Length;

            for (int i = 0; i < imag1.GetLength(0); i++)
                for (int j = 0; j < imag1.GetLength(1); j++)
                {
                    double ii = (Bins * (imag1[i, j] - min) / (max - min));

                    hist1[(int)ii] += 1;
                }

            double MaxVal = double.MinValue;
            double MaxX = 0;

            for (int i = 0; i < hist1.Length; i++)
            {
                if (hist1[i] > MaxVal)
                {
                    MaxVal = hist1[i];
                    MaxX = min + step * i;
                }
            }
            return MaxX;
        }
        private static double[,] Normalize(double[,] slice, out double Average, out double SD)
        {
            double intensityslice = 0;

            double d;

            for (int i = 0; i < slice.GetLength(0); i++)
                for (int j = 0; j < slice.GetLength(1); j++)
                {
                    d = slice[i, j];
                    intensityslice += d;

                }

            intensityslice = intensityslice / slice.GetLength(0) / slice.GetLength(1);
            Average = intensityslice;

            double sdSlice = 0;
            for (int i = 0; i < slice.GetLength(0); i++)
                for (int j = 0; j < slice.GetLength(1); j++)
                {
                    d = (intensityslice - slice[i, j]);
                    sdSlice += d * d;
                }

            sdSlice = Math.Sqrt(sdSlice / (slice.GetLength(0) * slice.GetLength(1) - 1));
            SD = sdSlice;
            slice.SubtractInPlace(intensityslice);
            slice.DivideInPlace(sdSlice);
            slice = MathHelpLib.ImageProcessing.MathImageHelps.FlattenImageEdges(slice);
            return slice;
        }


        public static double[] ImageQualityWholeTest(double[,] slice, bool ColorImage, string experimentfolder, string DataFolder, bool dipp, double innerclip, double outerclip,
           out Bitmap BestMatch, double[,] Background, double[,] VGExample, double[,] Background25, double[,] VGBackGround, double[,] VGBackGround25, ref ReplaceStringDictionary PassData)
        {


            double[,] NonNormalizedSlice = new double[slice.GetLength(0), slice.GetLength(1)];
            Buffer.BlockCopy(slice, 0, NonNormalizedSlice, 0, Buffer.ByteLength(slice));

            double[,] NonNormalizeVG = new double[slice.GetLength(0), slice.GetLength(1)];
            Buffer.BlockCopy(VGExample, 0, NonNormalizeVG, 0, Buffer.ByteLength(VGExample));


            double AveSlice, SDSlice, VGAve, VGSD, junkA, junkS, BGAve, BG_SD;
            slice = Normalize(slice, out AveSlice, out SDSlice);

            Background = Normalize(Background, out junkA, out junkS); //just overwritten on next step
            Background25 = Normalize(Background25, out BGAve, out BG_SD);

            VGBackGround = Normalize(VGBackGround, out junkA, out junkS);
            VGBackGround = VGBackGround.ZeroPadArray2D(slice.GetLength(0));

            VGBackGround25 = Normalize(VGBackGround25, out junkA, out junkS);
            VGBackGround25 = VGBackGround25.ZeroPadArray2D(slice.GetLength(0));


            VGExample = Normalize(VGExample, out VGAve, out VGSD);
            VGExample = VGExample.ZeroPadArray2D(slice.GetLength(0));
            double d = 0;
            /* double MaxIntensitySlice = 0;
             for (int i = 0; i < slice.GetLength(0); i++)
                 for (int j = 0; j < slice.GetLength(1); j++)
                 {
                     d = slice[i, j];
                     MaxIntensitySlice += Math.Abs(d);
                 }

             MaxIntensitySlice = Math.Sqrt(MaxIntensitySlice);*/
            double NormSlice = slice.Length;
            System.Diagnostics.Debug.Print(experimentfolder + "stack\\000\\");
            if (Directory.Exists(experimentfolder + "stack\\000\\"))
            {
                experimentfolder = experimentfolder + "stack\\000\\";
            }
            else 
                experimentfolder = experimentfolder + "stack\\";

            string[] FileNames = Directory.GetFiles(experimentfolder, "*.bmp");

            if (FileNames == null || FileNames.Length < 2)
                FileNames = Directory.GetFiles(experimentfolder, "*.png");

            if (FileNames == null || FileNames.Length < 2)
                FileNames = Directory.GetFiles(experimentfolder, "*.jpeg");

            if (FileNames == null || FileNames.Length < 2)
                FileNames = Directory.GetFiles(experimentfolder, "*.ivg");
            Array.Sort(FileNames);

            /* string[] FileNames2 = new string[FileNames.Length - 2];
             for (int i = 2; i < FileNames.Length; i++)
                 FileNames2[i - 2] = FileNames[i];*/

            double m = Math.Floor((double)(FileNames.Length / 2));
            int r = 0;

            string[] Files = new string[FileNames.Length];

            while (r < FileNames.Length)
            {
                Files[(int)m] = FileNames[r];
                r++;
                m += Math.Pow((-1), r + 1) * r;
            }

            ImageHolder BitmapImage = MathHelpsFileLoader. Load_Bitmap(Files[Files.Length / 2]);


            if (ColorImage == true)
            {
                BitmapImage = MathHelpsFileLoader.FixVisionGateImage(BitmapImage, 3);
            }


            IEffect Filter = new ImageViewer.Filters.Thresholding.IterativeThresholdEffect();
            Bitmap  BitmapImageB = (Bitmap )Filter.DoEffect(null, BitmapImage, PassData);

            //BitmapImageB.Save("c:\\temp\\thres.bmp");
            //WaterShed
            Filter = new ImageViewer.Filters.Blobs.WaterShedTool();
            Filter.DoEffect(null, BitmapImageB, PassData);
            //Data out of type :
            PassData = Filter.PassData;

            //Get Biggest Blob
            Filter = new ImageViewer.Filters.Blobs.GetBiggestCenterBlob();
            Filter.DoEffect(null, BitmapImage, PassData, PassData["Blobs"], false);
            //Data out of type :
            PassData = Filter.PassData;
            BlobDescription Rect = (BlobDescription)PassData["MaxBlob"];
            int x = Rect.CenterOfGravity.X;
            int y = Rect.CenterOfGravity.Y;

            int CellSize = (int)(slice.GetLength(0) * 1);
            double CellHalf = CellSize / 2;


            Rectangle CellArea = new Rectangle((int)Math.Truncate(x - CellHalf), (int)Math.Truncate(y - CellHalf), CellSize, CellSize);

            double[][] maxcorre = new double[14][];//[Files.Length];
            double[][] Averages = new double[14][];
            double[][] MostProb = new double[14][];
            for (int i = 0; i < maxcorre.Length; i++)
            {
                maxcorre[i] = new double[Files.Length];
                Averages[i] = new double[Files.Length];
                MostProb[i] = new double[Files.Length];
            }

            for (int i = 0; i < maxcorre.Length; i++)
            {
                for (int j = 0; j < Files.Length; j++)
                {
                    maxcorre[i][j] = double.MinValue;
                    Averages[i][j] = double.MinValue;
                    MostProb[i][j] = double.MinValue;
                }
            }


            for (int q = Files.Length /2-5; q < Files.Length/2+5; q++)
            {
                ImageHolder a = MathHelpsFileLoader.Load_Bitmap(Files[q]);

                if (ColorImage == true)
                {
                    a = MathHelpsFileLoader.FixVisionGateImage(a, 2);
                }

                try
                {
                    //Clip Image to New Image
                    Filter = new ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect();
                    //Parameters required: Clip Bounds as Rectangle
                    a = (ImageHolder)Filter.DoEffect(null, a, PassData, CellArea);

                    a.Invert();

                    double[,] stackimag = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(a, false);

                    double StackAve, StackSD;
                    stackimag = Normalize(stackimag, out StackAve, out StackSD);

                    /*double MaxIntensityStack = 0;
                    for (int i = 0; i < stackimag.GetLength(0); i++)
                        for (int j = 0; j < stackimag.GetLength(1); j++)
                        {
                            d = stackimag[i, j];
                            MaxIntensityStack += Math.Abs(d);
                        }*/
                    // MaxIntensityStack = Math.Sqrt(MaxIntensityStack);
                    double NormStack = 1 / 100d;// stackimag.Length;

                    double SumStack = stackimag.SumArray();
                    double[,] correlation;
                    double sumFreq = 0;
                    if (dipp == true)
                        correlation = MathFFTHelps.CrossCorrelationFFTFiltered(stackimag, slice, innerclip, outerclip, out sumFreq);
                    else
                        correlation = MathFFTHelps.CrossCorrelationFFT(stackimag, slice);

                    double maxa = MathArrayHelps.MaxArray(correlation);
                    Application.DoEvents();
                    //normalize by the energy contained in each matrix
                    maxcorre[0][q] = Math.Sqrt(maxa) / NormSlice / NormStack;
                    maxcorre[7][q] = sumFreq / NormSlice / NormStack;

                    double AveA = correlation.SumArray() / slice.Length;
                    Averages[0][q] = (AveA);

                    double MP = MostProbableValue(correlation);
                    MostProb[0][q] = MP / NormSlice / NormStack;

                    try
                    {
                        sumFreq = 0;
                        correlation = MathFFTHelps.CrossCorrelationFFTFiltered(stackimag, slice, .25, .75, out sumFreq);

                        maxa = MathArrayHelps.MaxArray(correlation);
                        maxcorre[1][q] = Math.Sqrt(maxa) / NormSlice / NormStack;
                        maxcorre[8][q] = sumFreq / NormSlice / NormStack;

                        AveA = correlation.SumArray() / slice.Length;
                        Averages[1][q] = (AveA);

                        MP = MostProbableValue(correlation);
                        MostProb[1][q] = MP / NormSlice / NormStack;
                    }
                    catch { }
                    Application.DoEvents();

                    try
                    {
                        sumFreq = 0;
                        correlation = MathFFTHelps.CrossCorrelationFFTFiltered(stackimag, slice, .5, .95, out sumFreq);
                        maxa = MathArrayHelps.MaxArray(correlation);
                        maxcorre[2][q] = Math.Sqrt(maxa) / NormSlice / NormStack;
                        maxcorre[9][q] = sumFreq / NormSlice / NormStack;

                        AveA = correlation.SumArray() / slice.Length;
                        Averages[2][q] = (AveA);

                        MP = MostProbableValue(correlation);
                        MostProb[2][q] = MP / NormSlice / NormStack;
                    }
                    catch { }
                    Application.DoEvents();
                    try
                    {
                        sumFreq = 0;
                        correlation = MathFFTHelps.CrossCorrelationFFTFiltered(stackimag, slice, .3, .6, out sumFreq);
                        maxa = MathArrayHelps.MaxArray(correlation);
                        maxcorre[3][q] = Math.Sqrt(maxa) / NormSlice / NormStack;
                        maxcorre[10][q] = sumFreq / NormSlice / NormStack;

                        AveA = correlation.SumArray() / slice.Length;
                        Averages[3][q] = (AveA);

                        MP = MostProbableValue(correlation);
                        MostProb[3][q] = MP / NormSlice / NormStack;
                    }
                    catch { }
                    Application.DoEvents();
                    try
                    {
                        sumFreq = 0;
                        correlation = MathFFTHelps.CrossCorrelationFFTFiltered(stackimag, slice, .75, .95, out sumFreq);
                        maxa = MathArrayHelps.MaxArray(correlation);
                        maxcorre[4][q] = Math.Sqrt(maxa) / NormSlice / NormStack;
                        maxcorre[11][q] = sumFreq / NormSlice / NormStack;

                        AveA = correlation.SumArray() / slice.Length;
                        Averages[4][q] = (AveA);

                        MP = MostProbableValue(correlation);
                        MostProb[4][q] = MP / NormSlice / NormStack;
                    }
                    catch { }
                    Application.DoEvents();
                    try
                    {
                        sumFreq = 0;
                        correlation = MathFFTHelps.CrossCorrelationFFTFilteredReordered(stackimag, slice, .3, .6, out sumFreq);
                        maxa = MathArrayHelps.MaxArray(correlation);
                        maxcorre[5][q] = Math.Sqrt(maxa) / NormSlice / NormStack;
                        maxcorre[12][q] = sumFreq / NormSlice / NormStack;

                        AveA = correlation.SumArray() / slice.Length;
                        Averages[5][q] = (AveA);

                        MP = MostProbableValue(correlation);
                        MostProb[5][q] = MP / NormSlice / NormStack;
                    }
                    catch { }
                    Application.DoEvents();
                    try
                    {
                        sumFreq = 0;
                        correlation = MathFFTHelps.CrossCorrelationFFTFilteredReordered(stackimag, slice, .75, .95, out sumFreq);
                        maxa = MathArrayHelps.MaxArray(correlation);
                        maxcorre[6][q] = Math.Sqrt(maxa) / NormSlice / NormStack;
                        maxcorre[13][q] = sumFreq / NormSlice / NormStack;

                        AveA = correlation.SumArray() / slice.Length;
                        Averages[6][q] = (AveA);

                        MP = MostProbableValue(correlation);
                        MostProb[6][q] = MP / NormSlice / NormStack;
                    }
                    catch { }
                    Application.DoEvents();
                    stackimag = null;
                    correlation = null;
                }
                catch { }
            }

            double[] maxcorr = new double[maxcorre.Length];

            for (int i = 0; i < maxcorr.Length; i++)
            {
                maxcorr[i] = maxcorre[i].Max();
            }

            double[] MaxAve = new double[Averages.Length];
            for (int i = 0; i < Averages.Length; i++)
                MaxAve[i] = Averages[i].Max();

            double[] MaxMP = new double[MostProb.Length];
            for (int i = 0; i < MostProb.Length; i++)
                MaxMP[i] = MostProb[i].Max();

            PassData.Add("RightQualityFull", maxcorr[0]);
            PassData.Add("RightQualityValueLowerHalf", maxcorr[1]);
            PassData.Add("RightQualityValueUpperHalf", maxcorr[2]);
            PassData.Add("RightQualityValueLowerThird", maxcorr[3]);
            PassData.Add("RightQualityValueUpperQuarter", maxcorr[4]);
            PassData.Add("RightQualityValueLowerThirdReo", maxcorr[5]);
            PassData.Add("RightQualityValueUpperQuarterReo", maxcorr[6]);

            PassData.Add("RightQualityValueFreq", maxcorr[7]);
            PassData.Add("RightQualityValueLowerHalfFreq", maxcorr[8]);
            PassData.Add("RightQualityValueUpperHalfFreq", maxcorr[9]);
            PassData.Add("RightQualityValueLowerThirdFreq", maxcorr[10]);
            PassData.Add("RightQualityValueUpperQuarterFreq", maxcorr[11]);
            PassData.Add("RightQualityValueLowerThirdReoFreq", maxcorr[12]);
            PassData.Add("RightQualityValueUpperQuarterReoFreq", maxcorr[13]);


            PassData.Add("RightQualityAverageFull", MaxAve[0]);
            PassData.Add("RightQualityAverageValueLowerHalf", MaxAve[1]);
            PassData.Add("RightQualityAverageValueUpperHalf", MaxAve[2]);
            PassData.Add("RightQualityAverageValueLowerThird", MaxAve[3]);
            PassData.Add("RightQualityAverageValueUpperQuarter", MaxAve[4]);
            PassData.Add("RightQualityAverageValueLowerThirdReo", MaxAve[5]);
            PassData.Add("RightQualityAverageValueUpperQuarterReo", MaxAve[6]);

            PassData.Add("RightQualityMaxMPFull", MaxMP[0]);
            PassData.Add("RightQualityMaxMPValueLowerHalf", MaxMP[1]);
            PassData.Add("RightQualityMaxMPValueUpperHalf", MaxMP[2]);
            PassData.Add("RightQualityMaxMPValueLowerThird", MaxMP[3]);
            PassData.Add("RightQualityMaxMPValueUpperQuarter", MaxMP[4]);
            PassData.Add("RightQualityMaxMPValueLowerThirdReo", MaxMP[5]);
            PassData.Add("RightQualityMaxMPValueUpperQuarterReo", MaxMP[6]);


            int Mid = maxcorre[0].Length / 2;
            PassData.Add("RightQualityFull" + "_M", maxcorre[0][Mid]);
            PassData.Add("RightQualityValueLowerHalf" + "_M", maxcorre[1][Mid]);
            PassData.Add("RightQualityValueUpperHalf" + "_M", maxcorre[2][Mid]);
            PassData.Add("RightQualityValueLowerThird" + "_M", maxcorre[3][Mid]);
            PassData.Add("RightQualityValueUpperQuarter" + "_M", maxcorre[4][Mid]);
            PassData.Add("RightQualityValueLowerThirdReo" + "_M", maxcorre[5][Mid]);
            PassData.Add("RightQualityValueUpperQuarterReo" + "_M", maxcorre[6][Mid]);

            PassData.Add("RightQualityValueFreq" + "_M", maxcorre[7][Mid]);
            PassData.Add("RightQualityValueLowerHalfFreq" + "_M", maxcorre[8][Mid]);
            PassData.Add("RightQualityValueUpperHalfFreq" + "_M", maxcorre[9][Mid]);
            PassData.Add("RightQualityValueLowerThirdFreq" + "_M", maxcorre[10][Mid]);
            PassData.Add("RightQualityValueUpperQuarterFreq" + "_M", maxcorre[11][Mid]);
            PassData.Add("RightQualityValueLowerThirdReoFreq" + "_M", maxcorre[12][Mid]);
            PassData.Add("RightQualityValueUpperQuarterReoFreq" + "_M", maxcorre[13][Mid]);


            PassData.Add("RightQualityAverageFull" + "_M", Averages[0][Mid]);
            PassData.Add("RightQualityAverageValueLowerHalf" + "_M", Averages[1][Mid]);
            PassData.Add("RightQualityAverageValueUpperHalf" + "_M", Averages[2][Mid]);
            PassData.Add("RightQualityAverageValueLowerThird" + "_M", Averages[3][Mid]);
            PassData.Add("RightQualityAverageValueUpperQuarter" + "_M", Averages[4][Mid]);
            PassData.Add("RightQualityAverageValueLowerThirdReo" + "_M", Averages[5][Mid]);
            PassData.Add("RightQualityAverageValueUpperQuarterReo" + "_M", Averages[6][Mid]);

            PassData.Add("RightQualityMaxMPFull" + "_M", MostProb[0][Mid]);
            PassData.Add("RightQualityMaxMPValueLowerHalf" + "_M", MostProb[1][Mid]);
            PassData.Add("RightQualityMaxMPValueUpperHalf" + "_M", MostProb[2][Mid]);
            PassData.Add("RightQualityMaxMPValueLowerThird" + "_M", MostProb[3][Mid]);
            PassData.Add("RightQualityMaxMPValueUpperQuarter" + "_M", MostProb[4][Mid]);
            PassData.Add("RightQualityMaxMPValueLowerThirdReo" + "_M", MostProb[5][Mid]);
            PassData.Add("RightQualityMaxMPValueUpperQuarterReo" + "_M", MostProb[6][Mid]);

            ///output the image
            Bitmap bestImage = null;
            for (int i = 0; i < maxcorre[0].Length; i++)
            {
                Application.DoEvents();
                if (maxcorr[0] == maxcorre[0][i] || i == (maxcorre[0].Length / 2))
                {
                    ImageHolder a = MathHelpsFileLoader.Load_Bitmap(Files[i]);
                    if (ColorImage == true)
                    {
                        a = MathHelpsFileLoader.FixVisionGateImage(a, 2);
                    }
                    try
                    {
                        string PostFix = "";
                        if (i == (maxcorre[0].Length / 2)) PostFix = "_M"; else PostFix = "";
                        //Clip Image to New Image
                        Filter = new ImageViewer.PythonScripting.Programming_Tools.ClipImageToNewEffect();
                        //Parameters required: Clip Bounds as Rectangle
                        a = (ImageHolder)Filter.DoEffect(null, a, PassData, CellArea);

                        a.Invert();

                        bestImage = a.ToBitmap();

                        double[,] BestArray = MathHelpLib.ImageProcessing.MathImageHelps.ConvertToDoubleArray(a, false);

                        double BestAve, BestSD;

                        double[,] NonNormalizedBA = new double[BestArray.GetLength(0), BestArray.GetLength(1)];
                        Buffer.BlockCopy(BestArray, 0, NonNormalizedBA, 0, Buffer.ByteLength(BestArray));

                        BestArray = Normalize(BestArray, out BestAve, out BestSD);

                        try
                        {
                            PassData.Add("MI_slice_Stack" + PostFix, MathFFTHelps.MI(slice, BestArray));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("MI_slice_VG" + PostFix, MathFFTHelps.MI(slice, VGExample));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("MI_VG_Stack" + PostFix, MathFFTHelps.MI(VGExample, BestArray));
                        }
                        catch { }

                        try
                        {
                            PassData.Add("CNR" + PostFix, MathFFTHelps.CNR(AveSlice, BGAve, SDSlice, BG_SD));
                        }
                        catch { }

                        List<double[,]> BROIs = MathFFTHelps.ROISelection(Background);
                        List<double[,]> SROIs = MathFFTHelps.ROISelection(slice);

                        double[,] Spec = null;
                        try
                        {
                            Spec = MathFFTHelps.PowerSpectrum(SROIs);
                        }
                        catch { }
                        try
                        {
                            PassData.Add("PowerSpec_all" + PostFix, MathFFTHelps.Turn1D(Spec, 0));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("PowerSpec_.5" + PostFix, MathFFTHelps.Turn1D(Spec, 0.5));
                        }
                        catch
                        { }
                        try
                        {
                            PassData.Add("PowerSpec_.75" + PostFix, MathFFTHelps.Turn1D(Spec, 0.75));
                        }
                        catch { }

                        try
                        {
                            Spec = MathFFTHelps.PowerSpectrum(BROIs);
                        }
                        catch { }
                        try
                        {
                            PassData.Add("PowerSpec_Back_all" + PostFix, MathFFTHelps.Turn1D(Spec, 0));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("PowerSpec_Back_.5" + PostFix, MathFFTHelps.Turn1D(Spec, 0.5));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("PowerSpec_Back_.75" + PostFix, MathFFTHelps.Turn1D(Spec, 0.75));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("DetectionTask_Back" + PostFix, MathFFTHelps.DetectionTask(slice, Background));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("DetectionTask_Self" + PostFix, MathFFTHelps.DetectionTask(slice, slice));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("DetectionTask_Back25" + PostFix, MathFFTHelps.DetectionTask(slice, Background25));
                        }
                        catch { }

                        try
                        {
                            PassData.Add("UQI_slice_Stack" + PostFix, MathFFTHelps.UQI(NonNormalizedSlice, NonNormalizedBA));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("UQI_slice_VG" + PostFix, MathFFTHelps.UQI(NonNormalizedSlice, NonNormalizeVG));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("UQI_STack_VG" + PostFix, MathFFTHelps.UQI(NonNormalizedBA, NonNormalizeVG));
                        }
                        catch { }


                        ///VisionGate Stuff
                        BROIs = MathFFTHelps.ROISelection(VGBackGround25);
                        SROIs = MathFFTHelps.ROISelection(VGExample);


                        try
                        {
                            Spec = MathFFTHelps.PowerSpectrum(SROIs);
                        }
                        catch { }
                        try
                        {
                            PassData.Add("VGPowerSpec_all" + PostFix, MathFFTHelps.Turn1D(Spec, 0));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("VGPowerSpec_.5" + PostFix, MathFFTHelps.Turn1D(Spec, 0.5));
                        }
                        catch
                        { }
                        try
                        {
                            PassData.Add("VGPowerSpec_.75" + PostFix, MathFFTHelps.Turn1D(Spec, 0.75));
                        }
                        catch { }

                        try
                        {
                            Spec = MathFFTHelps.PowerSpectrum(BROIs);
                        }
                        catch { }
                        try
                        {
                            PassData.Add("VGPowerSpec_Back_all" + PostFix, MathFFTHelps.Turn1D(Spec, 0));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("VGPowerSpec_Back_.5" + PostFix, MathFFTHelps.Turn1D(Spec, 0.5));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("VGPowerSpec_Back_.75" + PostFix, MathFFTHelps.Turn1D(Spec, 0.75));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("VGDetectionTask_Back" + PostFix, MathFFTHelps.DetectionTask(VGExample, VGBackGround));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("VGDetectionTask_Self" + PostFix, MathFFTHelps.DetectionTask(VGExample, VGExample));
                        }
                        catch { }
                        try
                        {
                            PassData.Add("VGDetectionTask_Back25" + PostFix, MathFFTHelps.DetectionTask(VGExample, VGBackGround25));
                        }
                        catch { }

                    }
                    catch { }
                }
            }

            BestMatch = bestImage;
            return maxcorr;
        }

        public override object[] DefaultProperties
        {
            get { return new object[] { new Bitmap(1, 1) }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "Image|image" }; }
        }

    }
}
