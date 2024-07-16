using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using MathHelpLib.PhysicalArrays;
using System.IO;
using MathHelpLib.ImageProcessing;
namespace MathHelpLib
{
    public enum PhysicalArrayType
    {
        DoubleArray, ComplexArray
    }
    public enum PhysicalArrayRank
    {
        Array1D = 1, Array2D = 2, Array3D = 3
    }
    public class PhysicalArray : IEquatable<PhysicalArray>, IEnumerable<double>, IDisposable
    {

        private PhysicalArrayTokens mArrayInformation = new PhysicalArrayTokens();

        private Array mCurrentArray;
        private double[, ,] mDataDouble = null;
        private complex[, ,] mDataComplex = null;

        private double[] mPhysicalStart = new double[3];
        private double[] mPhysicalEnd = new double[3];
        private double[] mPhysicalStep = new double[3];

        private PhysicalArrayType mArrayType = PhysicalArrayType.DoubleArray;
        private PhysicalArrayRank mArrayRank = PhysicalArrayRank.Array1D;

        GCHandle mhData = new GCHandle();
        IntPtr mipData = new IntPtr();

        public object ActualData1D
        {
            get
            {
                if (mArrayType == PhysicalArrayType.DoubleArray)
                {
                    double[] outArray = new double[mDataDouble.GetLength(2)];
                    Buffer.BlockCopy(mDataDouble, 0, outArray, 0, outArray.Length * 8);
                    return outArray;
                }
                else
                {
                    complex[] outArray = new complex[mDataComplex.GetLength(2)];
                    Buffer.BlockCopy(mDataComplex, 0, outArray, 0, outArray.Length * 16);
                    return outArray;
                }
            }
        }
        public object ActualData2D
        {
            get
            {
                if (mArrayType == PhysicalArrayType.DoubleArray)
                {
                    double[,] outArray = new double[mDataDouble.GetLength(2), mDataDouble.GetLength(1)];
                    Buffer.BlockCopy(mDataDouble, 0, outArray, 0, outArray.Length * 8);
                    return outArray;
                }
                else
                {
                    complex[,] outArray = new complex[mDataComplex.GetLength(1), mDataComplex.GetLength(2)];
                    Buffer.BlockCopy(mDataComplex, 0, outArray, 0, outArray.Length * 16);
                    return outArray;
                }
            }
        }
        public object ActualData3D
        {
            get
            {
                if (mArrayType == PhysicalArrayType.DoubleArray)
                {
                    double[, ,] outArray = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                    Buffer.BlockCopy(mDataDouble, 0, outArray, 0, outArray.Length * 8);
                    return outArray;
                }
                else
                {
                    complex[, ,] outArray = new complex[mDataComplex.GetLength(0), mDataComplex.GetLength(1), mDataComplex.GetLength(2)];
                    Buffer.BlockCopy(mDataComplex, 0, outArray, 0, outArray.Length * 16);
                    return outArray;
                }
            }
        }

        public double[, ,] ReferenceDataDouble
        {
            get { return mDataDouble; }
            set
            {
                mDataDouble = value;
                mCurrentArray = mDataDouble;

                try
                {
                    mhData.Free();
                }
                catch { }

                mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
                mipData = mhData.AddrOfPinnedObject();

                for (int i = 0; i < 3; i++)
                {
                    mPhysicalStep[i] = (mPhysicalEnd[i] - mPhysicalStart[i]) / mCurrentArray.GetLength(i);
                }
                mArrayType = PhysicalArrayType.DoubleArray;
            }
        }
        public complex[, ,] ReferenceDataComplex
        {
            get { return mDataComplex; }
            set
            {
                mDataComplex = value;
                mCurrentArray = mDataComplex;

                try
                {
                    mhData.Free();
                }
                catch { }

                mhData = GCHandle.Alloc(this.mDataComplex, GCHandleType.Pinned);
                mipData = mhData.AddrOfPinnedObject();

                for (int i = 0; i < 3; i++)
                {
                    mPhysicalStep[i] = (mPhysicalEnd[i] - mPhysicalStart[i]) / mCurrentArray.GetLength(i);
                }
                mArrayType = PhysicalArrayType.ComplexArray;
            }
        }

        public PhysicalArrayRank ArrayRank
        {
            get { return mArrayRank; }
            set { mArrayRank = value; }
        }
        public PhysicalArrayType ArrayType
        {
            get { return mArrayType; }
        }
        public PhysicalArrayTokens ArrayInformation
        {
            get { return mArrayInformation; }
        }

        #region Thises
        public object this[int indexX]
        {
            get
            {
                if (mArrayType == PhysicalArrayType.DoubleArray)
                    return mDataDouble[0, 0, indexX];
                else
                    return mDataComplex[0, 0, indexX];
            }
            set
            {

                if (mArrayType == PhysicalArrayType.DoubleArray)
                    mDataDouble[0, 0, indexX] = (double)value;
                else
                    mDataComplex[0, 0, indexX] = (complex)value;

            }
        }
        public object this[int indexX, int indexY]
        {
            get
            {
                if (mArrayType == PhysicalArrayType.DoubleArray)
                    return mDataDouble[0, indexY, indexX];
                else
                    return mDataComplex[0, indexY, indexX];

            }
            set
            {
                if (mArrayType == PhysicalArrayType.DoubleArray)
                    mDataDouble[0, indexY, indexX] = (double)value;
                else
                    mDataComplex[0, indexY, indexX] = (complex)value;

            }
        }
        public object this[int indexX, int indexY, int indexZ]
        {
            get
            {
                if (mArrayType == PhysicalArrayType.DoubleArray)
                    return mDataDouble[indexZ, indexY, indexX];
                else
                    return mDataComplex[indexZ, indexY, indexX];

            }
            set
            {
                if (mArrayType == PhysicalArrayType.DoubleArray)
                    mDataDouble[indexZ, indexY, indexX] = (double)value;
                else
                    mDataComplex[indexZ, indexY, indexX] = (complex)value;

            }
        }

        public object this[PhysicalRange Range]
        {
            get
            {
                int[] StartIndex;
                int[] EndIndex;
                Range.ConvertRangeToIndex(mPhysicalStart, mPhysicalEnd, mPhysicalStep, out StartIndex, out EndIndex);

                if (mArrayType == PhysicalArrayType.DoubleArray)
                {
                    #region Doubles
                    if (Range.RangeRank == PhysicalArrayRank.Array1D)
                    {
                        double[] Data = new double[EndIndex[2] - StartIndex[2]];
                        int cc = 0;
                        for (int i = StartIndex[2]; i < EndIndex[2]; i++)
                        {
                            Data[cc] = mDataDouble[0, 0, i];
                            cc++;
                        }
                        return new PhysicalArray(Data, Range.StartX, Range.EndX);
                    }
                    else if (Range.RangeRank == PhysicalArrayRank.Array2D)
                    {
                        double[,] Data = new double[EndIndex[1] - StartIndex[1], EndIndex[2] - StartIndex[2]];
                        int ccX = 0;
                        for (int i = StartIndex[2]; i < EndIndex[2]; i++)
                        {
                            int cc = 0;
                            for (int j = StartIndex[1]; j < EndIndex[1]; j++)
                            {
                                Data[ccX, cc] = mDataDouble[0, j, i];
                                cc++;
                            }
                            ccX++;
                        }
                        return new PhysicalArray(Data, Range.StartX, Range.EndX, Range.StartY, Range.EndY, false);
                    }
                    else
                    {
                        double[, ,] Data = new double[EndIndex[0] - StartIndex[0], EndIndex[1] - StartIndex[1], EndIndex[2] - StartIndex[2]];
                        int ccZ = 0;
                        for (int z = StartIndex[2]; z < EndIndex[2]; z++)
                        {
                            int ccX = 0;
                            for (int i = StartIndex[2]; i < EndIndex[2]; i++)
                            {
                                int cc = 0;
                                for (int j = StartIndex[1]; j < EndIndex[1]; j++)
                                {
                                    Data[ccZ, ccX, cc] = mDataDouble[z, j, i];
                                    cc++;
                                }
                                ccX++;
                            }
                            ccZ++;
                        }
                        return new PhysicalArray(Data, Range.StartX, Range.EndX, Range.StartY, Range.EndY, Range.StartZ, Range.EndZ, false);
                    }
                    #endregion
                }
                else
                {
                    #region Complex
                    if (Range.RangeRank == PhysicalArrayRank.Array1D)
                    {
                        complex[] Data = new complex[EndIndex[2] - StartIndex[2]];
                        int cc = 0;
                        for (int i = StartIndex[2]; i < EndIndex[2]; i++)
                        {
                            Data[cc] = mDataComplex[0, 0, i];
                            cc++;
                        }
                        return new PhysicalArray(Data, Range.StartX, Range.EndX);
                    }
                    else if (Range.RangeRank == PhysicalArrayRank.Array2D)
                    {
                        complex[,] Data = new complex[EndIndex[1] - StartIndex[1], EndIndex[2] - StartIndex[2]];
                        int ccX = 0;
                        for (int i = StartIndex[2]; i < EndIndex[2]; i++)
                        {
                            int cc = 0;
                            for (int j = StartIndex[1]; j < EndIndex[1]; j++)
                            {
                                Data[ccX, cc] = mDataComplex[0, j, i];
                                cc++;
                            }
                            ccX++;
                        }
                        return new PhysicalArray(Data, Range.StartX, Range.EndX, Range.StartY, Range.EndY);
                    }
                    else
                    {
                        complex[, ,] Data = new complex[EndIndex[0] - StartIndex[0], EndIndex[1] - StartIndex[1], EndIndex[2] - StartIndex[2]];
                        int ccZ = 0;
                        for (int z = StartIndex[2]; z < EndIndex[2]; z++)
                        {
                            int ccX = 0;
                            for (int i = StartIndex[2]; i < EndIndex[2]; i++)
                            {
                                int cc = 0;
                                for (int j = StartIndex[1]; j < EndIndex[1]; j++)
                                {
                                    Data[ccZ, ccX, cc] = mDataComplex[z, j, i];
                                    cc++;
                                }
                                ccX++;
                            }
                            ccZ++;
                        }
                        return new PhysicalArray(Data, Range.StartX, Range.EndX, Range.StartY, Range.EndY, Range.StartZ, Range.EndZ, false);
                    }
                    #endregion
                }
            }
        }
        public object this[IndexedRange Range]
        {
            get
            {
                int[] StartIndex = new int[3];
                int[] EndIndex = new int[3];
                StartIndex[0] = Range.StartXI;
                StartIndex[1] = Range.StartYI;
                StartIndex[2] = Range.StartZI;
                EndIndex[0] = Range.EndXI;
                EndIndex[1] = Range.EndYI;
                EndIndex[2] = Range.EndZI;
                double[] StartRange;
                double[] EndRange;
                Range.ConvertIndexToRange(mPhysicalStart, mPhysicalEnd, mPhysicalStep, out StartRange, out EndRange);
                if (mArrayType == PhysicalArrayType.DoubleArray)
                {
                    #region Doubles
                    if (Range.RangeRank == PhysicalArrayRank.Array1D)
                    {
                        double[] Data = new double[EndIndex[2] - StartIndex[2]];
                        int cc = 0;
                        for (int i = StartIndex[2]; i < EndIndex[2]; i++)
                        {
                            Data[cc] = mDataDouble[0, 0, i];
                            cc++;
                        }
                        return new PhysicalArray(Data, StartRange[2], EndRange[2]);
                    }
                    else if (Range.RangeRank == PhysicalArrayRank.Array2D)
                    {
                        double[,] Data = new double[EndIndex[1] - StartIndex[1], EndIndex[2] - StartIndex[2]];
                        int ccX = 0;
                        for (int i = StartIndex[2]; i < EndIndex[2]; i++)
                        {
                            int cc = 0;
                            for (int j = StartIndex[1]; j < EndIndex[1]; j++)
                            {
                                Data[ccX, cc] = mDataDouble[0, j, i];
                                cc++;
                            }
                            ccX++;
                        }
                        return new PhysicalArray(Data, StartRange[2], EndRange[2], StartRange[1], EndRange[1], false);
                    }
                    else
                    {
                        double[, ,] Data = new double[EndIndex[0] - StartIndex[0], EndIndex[1] - StartIndex[1], EndIndex[2] - StartIndex[2]];
                        int ccZ = 0;
                        for (int z = StartIndex[2]; z < EndIndex[2]; z++)
                        {
                            int ccX = 0;
                            for (int i = StartIndex[2]; i < EndIndex[2]; i++)
                            {
                                int cc = 0;
                                for (int j = StartIndex[1]; j < EndIndex[1]; j++)
                                {
                                    Data[ccZ, ccX, cc] = mDataDouble[z, j, i];
                                    cc++;
                                }
                                ccX++;
                            }
                            ccZ++;
                        }
                        return new PhysicalArray(Data, StartRange[2], EndRange[2], StartRange[1], EndRange[1], StartRange[0], EndRange[0], false);
                    }
                    #endregion
                }
                else
                {
                    #region Complex
                    if (Range.RangeRank == PhysicalArrayRank.Array1D)
                    {
                        complex[] Data = new complex[EndIndex[2] - StartIndex[2]];
                        int cc = 0;
                        for (int i = StartIndex[2]; i < EndIndex[2]; i++)
                        {
                            Data[cc] = mDataComplex[0, 0, i];
                            cc++;
                        }
                        return new PhysicalArray(Data, Range.StartXI, Range.EndXI);
                    }
                    else if (Range.RangeRank == PhysicalArrayRank.Array2D)
                    {
                        complex[,] Data = new complex[EndIndex[1] - StartIndex[1], EndIndex[2] - StartIndex[2]];
                        int ccX = 0;
                        for (int i = StartIndex[2]; i < EndIndex[2]; i++)
                        {
                            int cc = 0;
                            for (int j = StartIndex[1]; j < EndIndex[1]; j++)
                            {
                                Data[ccX, cc] = mDataComplex[0, j, i];
                                cc++;
                            }
                            ccX++;
                        }
                        return new PhysicalArray(Data, Range.StartXI, Range.EndXI, Range.StartYI, Range.EndYI);
                    }
                    else
                    {
                        complex[, ,] Data = new complex[EndIndex[0] - StartIndex[0], EndIndex[1] - StartIndex[1], EndIndex[2] - StartIndex[2]];
                        int ccZ = 0;
                        for (int z = StartIndex[2]; z < EndIndex[2]; z++)
                        {
                            int ccX = 0;
                            for (int i = StartIndex[2]; i < EndIndex[2]; i++)
                            {
                                int cc = 0;
                                for (int j = StartIndex[1]; j < EndIndex[1]; j++)
                                {
                                    Data[ccZ, ccX, cc] = mDataComplex[z, j, i];
                                    cc++;
                                }
                                ccX++;
                            }
                            ccZ++;
                        }
                        return new PhysicalArray(Data, Range.StartXI, Range.EndXI, Range.StartYI, Range.EndYI, Range.StartZI, Range.EndZI, false);
                    }
                    #endregion
                }

            }
        }


        public object this[bool dummy, params object[] Indexs]
        {
            get
            {
                int XStart = 0, XEnd = 0, YStart = 0, YEnd = 0, ZStart = 0, ZEnd = 0;
                #region figure Indexs
                if (Indexs[0] == null)
                {
                    XStart = 0;
                    XEnd = mCurrentArray.GetLength(2);
                    if (Indexs[1] == null)
                    {
                        YStart = 0;
                        YEnd = mCurrentArray.GetLength(1);
                        if (Indexs[2] == null)
                        {
                            ZStart = 0;
                            ZStart = mCurrentArray.GetLength(0);
                        }
                        else
                        {
                            ZStart = (int)Indexs[2];
                            ZEnd = (int)Indexs[3];
                        }
                    }
                    else
                    {
                        YStart = (int)Indexs[1];
                        YEnd = (int)Indexs[2];
                        if (Indexs[3] == null)
                        {
                            ZStart = 0;
                            ZStart = mCurrentArray.GetLength(0);
                        }
                        else
                        {
                            ZStart = (int)Indexs[3];
                            ZEnd = (int)Indexs[4];
                        }
                    }
                }
                else
                {
                    XStart = (int)Indexs[1];
                    YStart = (int)Indexs[2];
                    if (Indexs[3] == null)
                    {
                        YStart = 0;
                        YEnd = mCurrentArray.GetLength(1);
                        if (Indexs[4] == null)
                        {
                            ZStart = 0;
                            ZStart = mCurrentArray.GetLength(0);
                        }
                        else
                        {
                            ZStart = (int)Indexs[4];
                            ZEnd = (int)Indexs[5];
                        }
                    }
                    else
                    {
                        YStart = (int)Indexs[3];
                        YEnd = (int)Indexs[4];
                        if (Indexs[5] == null)
                        {
                            ZStart = 0;
                            ZStart = mCurrentArray.GetLength(0);
                        }
                        else
                        {
                            ZStart = (int)Indexs[5];
                            ZEnd = (int)Indexs[6];
                        }

                    }
                }
                #endregion
                if (mArrayType == PhysicalArrayType.DoubleArray)
                {
                    #region Double Array
                    if (XStart == XEnd)
                    {
                        #region X Still
                        if (YStart == YEnd)
                        {
                            #region ZOnly
                            if (ZStart == ZEnd)
                            {
                                return mDataDouble[ZStart, YStart, XStart];
                            }
                            else
                            {
                                double[] outArray = new double[Math.Abs(ZEnd - ZStart)];
                                if (ZStart < ZEnd)
                                {
                                    for (int i = ZStart; i < ZEnd; i++)
                                        outArray[i - ZStart] = mDataDouble[i, YStart, XStart];
                                }
                                else
                                {
                                    for (int i = ZEnd; i < ZStart; i++)
                                    {
                                        outArray[ZStart - i] = mDataDouble[i, YStart, XStart];
                                    }
                                }
                                return outArray;

                            }
                            #endregion
                        }
                        else if (ZStart == ZEnd)
                        {
                            #region YOnly

                            double[] outArray = new double[Math.Abs(YEnd - YStart)];
                            if (YStart < YEnd)
                            {
                                for (int i = YStart; i < YEnd; i++)
                                    outArray[i - YStart] = mDataDouble[ZStart, i, YStart];
                            }
                            else
                            {
                                for (int i = YEnd; i < YStart; i++)
                                {
                                    outArray[YStart - i] = mDataDouble[ZStart, i, XStart];
                                }
                            }
                            return outArray;

                            #endregion
                        }
                        else
                        {
                            #region Y And Z

                            double[,] outArray = new double[Math.Abs(YEnd - YStart), Math.Abs(ZEnd - ZStart)];
                            if (YStart < YEnd && ZStart < ZEnd)
                            {
                                for (int i = YStart; i < YEnd; i++)
                                    for (int j = ZStart; j < ZEnd; j++)
                                        outArray[i - YStart, j - ZStart] = mDataDouble[j, i, XStart];
                            }
                            else if (YStart > YEnd && ZStart < ZEnd)
                            {
                                for (int i = YEnd; i < YStart; i++)
                                    for (int j = ZStart; j < ZEnd; j++)
                                    {
                                        outArray[YStart - i, j - ZStart] = mDataDouble[j, i, XStart];
                                    }
                            }
                            else if (YStart < YEnd && ZStart > ZEnd)
                            {
                                for (int i = YStart; i < YEnd; i++)
                                    for (int j = ZEnd; j < ZStart; j++)
                                    {
                                        outArray[i - YStart, ZEnd - j] = mDataDouble[j, i, XStart];
                                    }
                            }
                            else if (YStart > YEnd && ZStart > ZEnd)
                            {
                                for (int i = YEnd; i < YStart; i++)
                                    for (int j = ZEnd; j < ZStart; j++)
                                    {
                                        outArray[YStart - i, ZEnd - j] = mDataDouble[j, i, XStart];
                                    }
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else if (YStart == YEnd)
                    {
                        #region YStill
                        if (ZStart == ZEnd)
                        {
                            if (XStart < XEnd)
                            {
                                #region X Row
                                double[] OutArray = new double[XEnd - XStart];
                                for (int i = XStart; i < XEnd; i++)
                                    OutArray[i - XStart] = mDataDouble[ZStart, YStart, i];
                                return OutArray;
                                #endregion
                            }
                            else
                            {
                                #region X and Z

                                double[,] outArray = new double[Math.Abs(XEnd - XStart), Math.Abs(ZEnd - ZStart)];
                                if (XStart < XEnd && ZStart < ZEnd)
                                {
                                    for (int i = XStart; i < XEnd; i++)
                                        for (int j = ZStart; j < ZEnd; j++)
                                            outArray[i - XStart, j - ZStart] = mDataDouble[j, YStart, i];
                                }
                                else if (XStart > XEnd && ZStart < ZEnd)
                                {
                                    for (int i = XEnd; i < XStart; i++)
                                        for (int j = ZStart; j < ZEnd; j++)
                                        {
                                            outArray[XStart - i, j - ZStart] = mDataDouble[j, YStart, i];
                                        }
                                }
                                else if (XStart < XEnd && ZStart > ZEnd)
                                {
                                    for (int i = XStart; i < XEnd; i++)
                                        for (int j = ZEnd; j < ZStart; j++)
                                        {
                                            outArray[i - YStart, ZEnd - j] = mDataDouble[j, YStart, i];
                                        }
                                }
                                else if (XStart > XEnd && ZStart > ZEnd)
                                {
                                    for (int i = XEnd; i < XStart; i++)
                                        for (int j = ZEnd; j < ZStart; j++)
                                        {
                                            outArray[YStart - i, ZEnd - j] = mDataDouble[j, YStart, i];
                                        }
                                }
                                #endregion
                            }
                        }
                        #endregion
                    }
                    else if (ZStart == ZEnd)
                    {
                        #region X and Y
                        double[,] outArray = new double[Math.Abs(XEnd - XStart), Math.Abs(YEnd - YStart)];
                        if (XStart < XEnd && YStart < YEnd)
                        {
                            for (int i = XStart; i < XEnd; i++)
                                for (int j = YStart; j < YEnd; j++)
                                    outArray[i - XStart, j - YStart] = mDataDouble[ZStart, j, i];
                        }
                        else if (XStart > XEnd && YStart < YEnd)
                        {
                            for (int i = XEnd; i < XStart; i++)
                                for (int j = YStart; j < YEnd; j++)
                                {
                                    outArray[XStart - i, j - YStart] = mDataDouble[ZStart, j, i];
                                }
                        }
                        else if (XStart < XEnd && YStart > YEnd)
                        {
                            for (int i = XStart; i < XEnd; i++)
                                for (int j = YEnd; j < YStart; j++)
                                {
                                    outArray[i - YStart, YEnd - j] = mDataDouble[ZStart, j, i];
                                }
                        }
                        else if (XStart > XEnd && YStart > YEnd)
                        {
                            for (int i = XEnd; i < XStart; i++)
                                for (int j = YEnd; j < YStart; j++)
                                {
                                    outArray[YStart - i, YEnd - j] = mDataDouble[ZStart, j, i];
                                }
                        }
                        return outArray;
                        #endregion
                    }
                    else
                    {
                        #region 3D Cuts
                        if (XStart < XEnd && YStart < YEnd && ZStart < ZEnd)
                        {
                            double[, ,] OutArray = new double[XEnd - ZStart, YEnd - YStart, ZEnd - ZStart];
                            for (int z = ZStart; z < ZEnd; z++)
                                for (int y = YStart; y < YEnd; y++)
                                    for (int x = XStart; x < XEnd; x++)
                                    {
                                        OutArray[x - XStart, y - YStart, z - ZStart] =
                                            mDataDouble[z, y, x];
                                    }
                        }

                        #endregion
                    }
                    #endregion
                }
                else
                {

                }
                return null;
            }
        }
        #endregion

        #region Constructors
        public void CopyInDoubleArray(byte[] DataBuffer)
        {
            Buffer.BlockCopy(DataBuffer, 0, mDataDouble, 0, Buffer.ByteLength(DataBuffer));
        }

        #region DataLessConstructors

        #region WithoutUnits
        private PhysicalArray()
        {
        }

        public PhysicalArray(int nPoints, double PhysicalStart, double PhysicalEnd)
            : this(new double[1, 1, nPoints], PhysicalStart, PhysicalEnd, 0, 0, 0, 0, false)
        {
            mArrayRank = PhysicalArrayRank.Array1D;
        }

        public PhysicalArray(int nPointsX, int nPointsY, double PhysicalStartX, double PhysicalEndX, double PhysicalStartY, double PhysicalEndY)
        {
            double[, ,] data = new double[1, nPointsX, nPointsY];
            mDataDouble = data;

            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEndX;
            mPhysicalStart[2] = PhysicalStartX;
            mPhysicalStep[2] = (PhysicalEndX - PhysicalStartX) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEndY;
            mPhysicalStart[1] = PhysicalStartY;
            mPhysicalStep[1] = (PhysicalEndY - PhysicalStartY) / (double)mDataDouble.GetLength(1);

            mPhysicalEnd[0] = 0;
            mPhysicalStart[0] = 0;
            mPhysicalStep[0] = 0;

            mArrayRank = PhysicalArrayRank.Array3D;
            mCurrentArray = mDataDouble;
            mArrayRank = PhysicalArrayRank.Array2D;
        }


        public PhysicalArray(int nPointsX, int nPointsY, int nPointsZ,
            double PhysicalStartX, double PhysicalEndX,
            double PhysicalStartY, double PhysicalEndY,
            double PhysicalStartZ, double PhysicalEndZ)
        // : this(new double[nPointsZ, nPointsY, nPointsX], PhysicalStartX, PhysicalEndX, PhysicalStartY, PhysicalEndY, PhysicalStartZ, PhysicalEndZ, false)
        {
            double[, ,] data = new double[nPointsZ, nPointsY, nPointsX];

            mDataDouble = data;

            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();

            try
            {
                mPhysicalEnd[2] = PhysicalEndX;
                mPhysicalStart[2] = PhysicalStartX;
                mPhysicalStep[2] = (PhysicalEndX - PhysicalStartX) / (double)mDataDouble.GetLength(2);
            }
            catch { }

            try
            {
                mPhysicalEnd[1] = PhysicalEndY;
                mPhysicalStart[1] = PhysicalStartY;
                mPhysicalStep[1] = (PhysicalEndY - PhysicalStartY) / (double)mDataDouble.GetLength(1);
            }
            catch { }

            try
            {
                mPhysicalEnd[0] = PhysicalEndZ;
                mPhysicalStart[0] = PhysicalStartZ;
                mPhysicalStep[0] = (PhysicalEndZ - PhysicalStartZ) / (double)mDataDouble.GetLength(0);
            }
            catch { }


            mCurrentArray = mDataDouble;

            mArrayRank = PhysicalArrayRank.Array3D;
        }

        public PhysicalArray(int nPointsX, int nPointsY, int nPointsZ,
    double[] PhysicalDimX, double[] PhysicalDimY, double[] PhysicalDimZ)
            : this(new double[nPointsZ, nPointsY, nPointsX],
            PhysicalDimX[0], PhysicalDimX[1],
            PhysicalDimY[0], PhysicalDimY[1],
            PhysicalDimZ[0], PhysicalDimZ[1], false)
        {
            mArrayRank = PhysicalArrayRank.Array3D;
        }

        public PhysicalArray(int nPointsX, int nPointsY, int nPointsZ,
double[] PhysicalStart, double[] PhysicalEnd)
            : this(new double[nPointsZ, nPointsY, nPointsX],
            PhysicalStart[0], PhysicalEnd[0],
            PhysicalStart[1], PhysicalEnd[1],
            PhysicalStart[2], PhysicalEnd[2], false)
        {
            mArrayRank = PhysicalArrayRank.Array3D;
        }
        #endregion
        #region WithUnits




        /// <summary>
        /// This will not clone the data, but will produce a blank data array of the desired type that matches the dimenisions of the clonearray
        /// </summary>
        /// <param name="CloneArray"></param>
        /// <param name="DesiredType"></param>
        public PhysicalArray(PhysicalArray CloneArray, PhysicalArrayType DesiredType)
        {
            if (CloneArray == null) throw new ArgumentNullException("must have initialized CloneArray");
            if (DesiredType == PhysicalArrayType.DoubleArray)
            {
                mDataDouble = new double[CloneArray.mCurrentArray.GetLength(0), CloneArray.mCurrentArray.GetLength(1), CloneArray.mCurrentArray.GetLength(2)];
                mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
                mipData = mhData.AddrOfPinnedObject();
                mCurrentArray = mDataDouble;
            }
            else
            {
                mDataComplex = new complex[CloneArray.mCurrentArray.GetLength(0), CloneArray.mCurrentArray.GetLength(1), CloneArray.mCurrentArray.GetLength(2)];
                mhData = GCHandle.Alloc(this.mDataComplex, GCHandleType.Pinned);
                mipData = mhData.AddrOfPinnedObject();
                mCurrentArray = mDataComplex;
            }

            for (int i = 0; i < 3; i++)
            {
                mPhysicalEnd[i] = CloneArray.mPhysicalEnd[i];
                mPhysicalStart[i] = CloneArray.mPhysicalStart[i];
                mPhysicalStep[i] = CloneArray.mPhysicalStep[i];
            }
            mArrayInformation = (PhysicalArrayTokens)CloneArray.mArrayInformation.Clone();
            mArrayRank = CloneArray.mArrayRank;
            mArrayType = DesiredType;
        }

        /// <summary>
        /// This will not clone the data, but will produce a blank data array of the desired type that matches the dimenisions of the clonearray
        /// </summary>
        /// <param name="CloneArray"></param>
        /// <param name="DesiredType"></param>
        /// <param name="MakeArray" >Determines if the blank array is make,  you must use reference array next if you do not create an array here
        public PhysicalArray(PhysicalArray CloneArray, PhysicalArrayType DesiredType, bool MakeArray)
        {
            if (CloneArray == null) throw new ArgumentNullException("must have initialized CloneArray");
            if (MakeArray)
            {
                if (DesiredType == PhysicalArrayType.DoubleArray)
                {
                    mDataDouble = new double[CloneArray.mCurrentArray.GetLength(0), CloneArray.mCurrentArray.GetLength(1), CloneArray.mCurrentArray.GetLength(2)];
                    mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
                    mipData = mhData.AddrOfPinnedObject();
                    mCurrentArray = mDataDouble;
                }
                else
                {
                    mDataComplex = new complex[CloneArray.mCurrentArray.GetLength(0), CloneArray.mCurrentArray.GetLength(1), CloneArray.mCurrentArray.GetLength(2)];
                    mhData = GCHandle.Alloc(this.mDataComplex, GCHandleType.Pinned);
                    mipData = mhData.AddrOfPinnedObject();
                    mCurrentArray = mDataComplex;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                mPhysicalEnd[i] = CloneArray.mPhysicalEnd[i];
                mPhysicalStart[i] = CloneArray.mPhysicalStart[i];
                mPhysicalStep[i] = CloneArray.mPhysicalStep[i];
            }
            mArrayInformation = (PhysicalArrayTokens)CloneArray.mArrayInformation.Clone();
            mArrayRank = CloneArray.mArrayRank;
            mArrayType = DesiredType;
        }

        #endregion
        #endregion

        #region DoubleConstructors
        public PhysicalArray(double[] data, double PhysicalStart, double PhysicalEnd)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            mDataDouble = new double[1, 1, data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                mDataDouble[0, 0, i] = data[i];
            }
            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEnd;
            mPhysicalStart[2] = PhysicalStart;
            mPhysicalStep[2] = (PhysicalEnd - PhysicalStart) / (double)mDataDouble.GetLength(2);

            mArrayRank = PhysicalArrayRank.Array1D;
            mCurrentArray = mDataDouble;
        }

        public PhysicalArray(double[,] data, double PhysicalStartX, double PhysicalEndX, double PhysicalStartY, double PhysicalEndY, bool TransposeData)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            if (TransposeData)
            {
                mDataDouble = new double[1, data.GetLength(1), data.GetLength(0)];
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        mDataDouble[0, j, i] = data[i, j];
                    }
                }
            }
            else
            {
                mDataDouble = new double[1, data.GetLength(0), data.GetLength(1)];
                Buffer.BlockCopy(data, 0, mDataDouble, 0, Buffer.ByteLength(data));
                /*                for (int i = 0; i < data.GetLength(0); i++)
                                {
                                    for (int j = 0; j < data.GetLength(1); j++)
                                    {
                                        mDataDouble[0, i, j] = data[i, j];
                                    }
                                }*/
            }
            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEndX;
            mPhysicalStart[2] = PhysicalStartX;
            mPhysicalStep[2] = (PhysicalEndX - PhysicalStartX) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEndY;
            mPhysicalStart[1] = PhysicalStartY;
            mPhysicalStep[1] = (PhysicalEndY - PhysicalStartY) / (double)mDataDouble.GetLength(1);

            mArrayRank = PhysicalArrayRank.Array2D;
            mCurrentArray = mDataDouble;
        }

        public PhysicalArray(PhysicalArray OriginalArray)
        {
            mDataDouble = new double[OriginalArray.mDataDouble.GetLength(0), OriginalArray.mDataDouble.GetLength(1), OriginalArray.mDataDouble.GetLength(2)];
            Buffer.BlockCopy(OriginalArray.mDataDouble, 0, mDataDouble, 0, Buffer.ByteLength(OriginalArray.mDataDouble));

            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();

            for (int i = 0; i < mPhysicalEnd.Length; i++)
            {
                mPhysicalEnd[i] = OriginalArray.mPhysicalEnd[i];
                mPhysicalStart[i] = OriginalArray.mPhysicalStart[i];
                mPhysicalStep[i] = OriginalArray.mPhysicalStep[i];
            }

            mArrayRank = OriginalArray.mArrayRank;
            mCurrentArray = mDataDouble;
        }


        /*public PhysicalArray(ImageViewer.ImageHolder data, double PhysicalStartX, double PhysicalEndX, double PhysicalStartY, double PhysicalEndY)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");

            mDataDouble = new double[1, data.Width, data.Height];
            float[,] imageData = data.ToDataIntensity();
            for (int i = 0; i < data.Width; i++)
            {
                for (int j = 0; j < data.Height; j++)
                {
                    mDataDouble[0, i, j] = imageData[j, i];
                }
            }

            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEndX;
            mPhysicalStart[2] = PhysicalStartX;
            mPhysicalStep[2] = (PhysicalEndX - PhysicalStartX) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEndY;
            mPhysicalStart[1] = PhysicalStartY;
            mPhysicalStep[1] = (PhysicalEndY - PhysicalStartY) / (double)mDataDouble.GetLength(1);

            mArrayRank = PhysicalArrayRank.Array2D;
            mCurrentArray = mDataDouble;
        }
        */

        public PhysicalArray(double[,] data, double[] PhysicalStart, double[] PhysicalEnd)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            mDataDouble = new double[1, data.GetLength(1), data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    mDataDouble[0, j, i] = data[i, j];
                }
            }
            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();

            if (PhysicalStart.Length == 2)
            {
                mPhysicalEnd[2] = PhysicalEnd[0];
                mPhysicalStart[2] = PhysicalStart[0];
                mPhysicalStep[2] = (PhysicalEnd[0] - PhysicalStart[0]) / (double)mDataDouble.GetLength(2);

                mPhysicalEnd[1] = PhysicalEnd[1];
                mPhysicalStart[1] = PhysicalStart[1];
                mPhysicalStep[1] = (PhysicalEnd[1] - PhysicalStart[1]) / (double)mDataDouble.GetLength(1);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    mPhysicalEnd[2 - i] = PhysicalEnd[i];
                    mPhysicalStart[2 - i] = PhysicalStart[i];
                    mPhysicalStep[2 - i] = (PhysicalEnd[i] - PhysicalStart[i]) / (double)mDataDouble.GetLength(2 - i);
                }
            }
            mArrayRank = PhysicalArrayRank.Array2D;
            mCurrentArray = mDataDouble;
        }


        public PhysicalArray(double[, ,] data,
            double PhysicalStartX, double PhysicalEndX,
            double PhysicalStartY, double PhysicalEndY,
            double PhysicalStartZ, double PhysicalEndZ)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            mDataDouble = new double[data.GetLength(2), data.GetLength(1), data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    for (int m = 0; m < data.GetLength(2); m++)
                    {

                        mDataDouble[m, j, i] = data[i, j, m];
                    }
                }
            }
            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEndX;
            mPhysicalStart[2] = PhysicalStartX;
            mPhysicalStep[2] = (PhysicalEndX - PhysicalStartX) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEndY;
            mPhysicalStart[1] = PhysicalStartY;
            mPhysicalStep[1] = (PhysicalEndY - PhysicalStartY) / (double)mDataDouble.GetLength(1);

            mPhysicalEnd[0] = PhysicalEndZ;
            mPhysicalStart[0] = PhysicalStartZ;
            mPhysicalStep[0] = (PhysicalEndZ - PhysicalStartZ) / (double)mDataDouble.GetLength(0);

            mArrayRank = PhysicalArrayRank.Array3D;
            mCurrentArray = mDataDouble;
        }

        public PhysicalArray(double[, ,] data,
           double[] PhysicalStart, double[] PhysicalEnd)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            mDataDouble = new double[data.GetLength(0), data.GetLength(1), data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    for (int m = 0; m < data.GetLength(1); m++)
                    {

                        mDataDouble[m, j, i] = data[i, j, m];
                    }
                }
            }
            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEnd[2];
            mPhysicalStart[2] = PhysicalStart[2];
            mPhysicalStep[2] = (PhysicalEnd[2] - PhysicalStart[2]) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEnd[1];
            mPhysicalStart[1] = PhysicalStart[1];
            mPhysicalStep[1] = (PhysicalEnd[1] - PhysicalStart[1]) / (double)mDataDouble.GetLength(1);

            mPhysicalEnd[0] = PhysicalEnd[0];
            mPhysicalStart[0] = PhysicalStart[0];
            mPhysicalStep[0] = (PhysicalEnd[0] - PhysicalStart[0]) / (double)mDataDouble.GetLength(0);

            mArrayRank = PhysicalArrayRank.Array3D;
            mCurrentArray = mDataDouble;
        }

        public PhysicalArray(double[, ,] data, double[] PhysicalStart, double[] PhysicalEnd, bool TranscribeData)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            if (TranscribeData)
            {
                mDataDouble = new double[data.GetLength(0), data.GetLength(1), data.GetLength(0)];
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        for (int m = 0; m < data.GetLength(1); m++)
                        {

                            mDataDouble[m, j, i] = data[i, j, m];
                        }
                    }
                }
            }
            else
                mDataDouble = data;

            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEnd[2];
            mPhysicalStart[2] = PhysicalStart[2];
            mPhysicalStep[2] = (PhysicalEnd[2] - PhysicalStart[2]) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEnd[1];
            mPhysicalStart[1] = PhysicalStart[1];
            mPhysicalStep[1] = (PhysicalEnd[1] - PhysicalStart[1]) / (double)mDataDouble.GetLength(1);

            mPhysicalEnd[0] = PhysicalEnd[0];
            mPhysicalStart[0] = PhysicalStart[0];
            mPhysicalStep[0] = (PhysicalEnd[0] - PhysicalStart[0]) / (double)mDataDouble.GetLength(0);

            mArrayRank = PhysicalArrayRank.Array3D;
            mCurrentArray = mDataDouble;
        }

        public PhysicalArray(double[, ,] data, double[] PhysicalStart, double[] PhysicalEnd, bool TranscribeData, PhysicalArrayRank DesiredArrayRank)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            if (TranscribeData)
            {
                mDataDouble = new double[data.GetLength(0), data.GetLength(1), data.GetLength(0)];
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        for (int m = 0; m < data.GetLength(1); m++)
                        {

                            mDataDouble[m, j, i] = data[i, j, m];
                        }
                    }
                }
            }
            else
                mDataDouble = data;

            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEnd[2];
            mPhysicalStart[2] = PhysicalStart[2];
            mPhysicalStep[2] = (PhysicalEnd[2] - PhysicalStart[2]) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEnd[1];
            mPhysicalStart[1] = PhysicalStart[1];
            mPhysicalStep[1] = (PhysicalEnd[1] - PhysicalStart[1]) / (double)mDataDouble.GetLength(1);

            mPhysicalEnd[0] = PhysicalEnd[0];
            mPhysicalStart[0] = PhysicalStart[0];
            mPhysicalStep[0] = (PhysicalEnd[0] - PhysicalStart[0]) / (double)mDataDouble.GetLength(0);

            mArrayRank = DesiredArrayRank;
            mCurrentArray = mDataDouble;
        }

        public PhysicalArray(double[, ,] data,
            double PhysicalStartX, double PhysicalEndX,
            double PhysicalStartY, double PhysicalEndY,
            double PhysicalStartZ, double PhysicalEndZ,
            bool TranscribeData)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            if (TranscribeData)
            {
                mDataDouble = new double[data.GetLength(0), data.GetLength(1), data.GetLength(0)];
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        for (int m = 0; m < data.GetLength(1); m++)
                        {

                            mDataDouble[m, j, i] = data[i, j, m];
                        }
                    }
                }
            }
            else
                mDataDouble = data;

            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEndX;
            mPhysicalStart[2] = PhysicalStartX;
            mPhysicalStep[2] = (PhysicalEndX - PhysicalStartX) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEndY;
            mPhysicalStart[1] = PhysicalStartY;
            mPhysicalStep[1] = (PhysicalEndY - PhysicalStartY) / (double)mDataDouble.GetLength(1);

            mPhysicalEnd[0] = PhysicalEndZ;
            mPhysicalStart[0] = PhysicalStartZ;
            mPhysicalStep[0] = (PhysicalEndZ - PhysicalStartZ) / (double)mDataDouble.GetLength(0);

            mArrayRank = PhysicalArrayRank.Array3D;
            mCurrentArray = mDataDouble;
        }
        #endregion

        #region FloatConstructors

        public PhysicalArray(float[,] data, double PhysicalStartX, double PhysicalEndX, double PhysicalStartY, double PhysicalEndY, bool TransposeData)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            if (TransposeData)
            {
                mDataDouble = new double[1, data.GetLength(1), data.GetLength(0)];
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        mDataDouble[0, j, i] = data[i, j];
                    }
                }
            }
            else
            {
                mDataDouble = new double[1, data.GetLength(0), data.GetLength(1)];
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        mDataDouble[0, i, j] = data[i, j];
                    }
                }
            }
            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEndX;
            mPhysicalStart[2] = PhysicalStartX;
            mPhysicalStep[2] = (PhysicalEndX - PhysicalStartX) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEndY;
            mPhysicalStart[1] = PhysicalStartY;
            mPhysicalStep[1] = (PhysicalEndY - PhysicalStartY) / (double)mDataDouble.GetLength(1);

            mArrayRank = PhysicalArrayRank.Array2D;
            mCurrentArray = mDataDouble;
        }


        public PhysicalArray(float[, ,] data,
            double PhysicalStartX, double PhysicalEndX,
            double PhysicalStartY, double PhysicalEndY,
            double PhysicalStartZ, double PhysicalEndZ)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            mDataDouble = new double[data.GetLength(2), data.GetLength(1), data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    for (int m = 0; m < data.GetLength(2); m++)
                    {

                        mDataDouble[m, j, i] = data[i, j, m];
                    }
                }
            }
            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEndX;
            mPhysicalStart[2] = PhysicalStartX;
            mPhysicalStep[2] = (PhysicalEndX - PhysicalStartX) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEndY;
            mPhysicalStart[1] = PhysicalStartY;
            mPhysicalStep[1] = (PhysicalEndY - PhysicalStartY) / (double)mDataDouble.GetLength(1);

            mPhysicalEnd[0] = PhysicalEndZ;
            mPhysicalStart[0] = PhysicalStartZ;
            mPhysicalStep[0] = (PhysicalEndZ - PhysicalStartZ) / (double)mDataDouble.GetLength(0);

            mArrayRank = PhysicalArrayRank.Array3D;
            mCurrentArray = mDataDouble;
        }
        #endregion

        #region Complex_Constructors
        public PhysicalArray(complex[] data, double PhysicalStart, double PhysicalEnd)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            mDataComplex = new complex[1, 1, data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                mDataComplex[0, 0, i] = data[i];
            }
            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEnd;
            mPhysicalStart[2] = PhysicalStart;
            mPhysicalStep[2] = (PhysicalEnd - PhysicalStart) / (double)mDataDouble.GetLength(2);

            mArrayRank = PhysicalArrayRank.Array1D;
            mArrayType = PhysicalArrayType.ComplexArray;
            mCurrentArray = mDataComplex;
        }

        public PhysicalArray(complex[,] data, double PhysicalStartX, double PhysicalEndX, double PhysicalStartY, double PhysicalEndY)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            mDataComplex = new complex[1, data.GetLength(1), data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    mDataComplex[0, j, i] = data[i, j];
                }
            }
            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEndX;
            mPhysicalStart[2] = PhysicalStartX;
            mPhysicalStep[2] = (PhysicalEndX - PhysicalStartX) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEndY;
            mPhysicalStart[1] = PhysicalStartY;
            mPhysicalStep[1] = (PhysicalEndY - PhysicalStartY) / (double)mDataDouble.GetLength(1);

            mArrayRank = PhysicalArrayRank.Array2D;
            mArrayType = PhysicalArrayType.ComplexArray;
            mCurrentArray = mDataComplex;
        }

        public PhysicalArray(complex[, ,] data,
            double PhysicalStartX, double PhysicalEndX,
            double PhysicalStartY, double PhysicalEndY,
            double PhysicalStartZ, double PhysicalEndZ)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            mDataComplex = new complex[data.GetLength(0), data.GetLength(1), data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    for (int m = 0; m < data.GetLength(1); m++)
                    {

                        mDataComplex[m, j, i] = data[i, j, m];
                    }
                }
            }
            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEndX;
            mPhysicalStart[2] = PhysicalStartX;
            mPhysicalStep[2] = (PhysicalEndX - PhysicalStartX) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEndY;
            mPhysicalStart[1] = PhysicalStartY;
            mPhysicalStep[1] = (PhysicalEndY - PhysicalStartY) / (double)mDataDouble.GetLength(1);

            mPhysicalEnd[0] = PhysicalEndZ;
            mPhysicalStart[0] = PhysicalStartZ;
            mPhysicalStep[0] = (PhysicalEndZ - PhysicalStartZ) / (double)mDataDouble.GetLength(0);

            mArrayRank = PhysicalArrayRank.Array3D;
            mArrayType = PhysicalArrayType.ComplexArray;
            mCurrentArray = mDataComplex;
        }

        public PhysicalArray(complex[, ,] data,
           double[] PhysicalStart, double[] PhysicalEnd)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            mDataComplex = new complex[data.GetLength(0), data.GetLength(1), data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    for (int m = 0; m < data.GetLength(1); m++)
                    {

                        mDataComplex[m, j, i] = data[i, j, m];
                    }
                }
            }
            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEnd[2];
            mPhysicalStart[2] = PhysicalStart[2];
            mPhysicalStep[2] = (PhysicalEnd[2] - PhysicalStart[2]) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEnd[1];
            mPhysicalStart[1] = PhysicalStart[1];
            mPhysicalStep[1] = (PhysicalEnd[1] - PhysicalStart[1]) / (double)mDataDouble.GetLength(1);

            mPhysicalEnd[0] = PhysicalEnd[0];
            mPhysicalStart[0] = PhysicalStart[0];
            mPhysicalStep[0] = (PhysicalEnd[0] - PhysicalStart[0]) / (double)mDataDouble.GetLength(0);

            mArrayRank = PhysicalArrayRank.Array3D;
            mArrayType = PhysicalArrayType.ComplexArray;
            mCurrentArray = mDataComplex;
        }

        public PhysicalArray(complex[, ,] data, double[] PhysicalStart, double[] PhysicalEnd, bool TranscribeData)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            if (TranscribeData)
            {
                mDataComplex = new complex[data.GetLength(0), data.GetLength(1), data.GetLength(0)];
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        for (int m = 0; m < data.GetLength(1); m++)
                        {

                            mDataComplex[m, j, i] = data[i, j, m];
                        }
                    }
                }
            }
            else
                mDataComplex = data;

            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEnd[2];
            mPhysicalStart[2] = PhysicalStart[2];
            mPhysicalStep[2] = (PhysicalEnd[2] - PhysicalStart[2]) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEnd[1];
            mPhysicalStart[1] = PhysicalStart[1];
            mPhysicalStep[1] = (PhysicalEnd[1] - PhysicalStart[1]) / (double)mDataDouble.GetLength(1);

            mPhysicalEnd[0] = PhysicalEnd[0];
            mPhysicalStart[0] = PhysicalStart[0];
            mPhysicalStep[0] = (PhysicalEnd[0] - PhysicalStart[0]) / (double)mDataDouble.GetLength(0);

            mArrayRank = PhysicalArrayRank.Array3D;
            mArrayType = PhysicalArrayType.ComplexArray;
            mCurrentArray = mDataComplex;
        }

        public PhysicalArray(complex[, ,] data, double[] PhysicalStart, double[] PhysicalEnd, bool TranscribeData, PhysicalArrayRank DesiredRank)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            if (TranscribeData)
            {
                mDataComplex = new complex[data.GetLength(0), data.GetLength(1), data.GetLength(0)];
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        for (int m = 0; m < data.GetLength(1); m++)
                        {

                            mDataComplex[m, j, i] = data[i, j, m];
                        }
                    }
                }
            }
            else
                mDataComplex = data;

            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd[2] = PhysicalEnd[2];
            mPhysicalStart[2] = PhysicalStart[2];
            mPhysicalStep[2] = (PhysicalEnd[2] - PhysicalStart[2]) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEnd[1];
            mPhysicalStart[1] = PhysicalStart[1];
            mPhysicalStep[1] = (PhysicalEnd[1] - PhysicalStart[1]) / (double)mDataDouble.GetLength(1);

            mPhysicalEnd[0] = PhysicalEnd[0];
            mPhysicalStart[0] = PhysicalStart[0];
            mPhysicalStep[0] = (PhysicalEnd[0] - PhysicalStart[0]) / (double)mDataDouble.GetLength(0);

            mArrayRank = DesiredRank;
            mArrayType = PhysicalArrayType.ComplexArray;
            mCurrentArray = mDataComplex;
        }

        public PhysicalArray(complex[, ,] data,
            double PhysicalStartX, double PhysicalEndX,
            double PhysicalStartY, double PhysicalEndY,
            double PhysicalStartZ, double PhysicalEndZ,
            bool TranscribeData)
        {
            if (data == null) throw new ArgumentNullException("must have initialized mDataDouble");
            if (TranscribeData)
            {
                mDataComplex = new complex[data.GetLength(0), data.GetLength(1), data.GetLength(0)];
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        for (int m = 0; m < data.GetLength(1); m++)
                        {

                            mDataComplex[m, j, i] = data[i, j, m];
                        }
                    }
                }
            }
            else
                mDataComplex = data;

            mhData = GCHandle.Alloc(this.mDataDouble, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();

            mPhysicalEnd[2] = PhysicalEndX;
            mPhysicalStart[2] = PhysicalStartX;
            mPhysicalStep[2] = (PhysicalEndX - PhysicalStartX) / (double)mDataDouble.GetLength(2);

            mPhysicalEnd[1] = PhysicalEndY;
            mPhysicalStart[1] = PhysicalStartY;
            mPhysicalStep[1] = (PhysicalEndY - PhysicalStartY) / (double)mDataDouble.GetLength(1);

            mPhysicalEnd[0] = PhysicalEndZ;
            mPhysicalStart[0] = PhysicalStartZ;
            mPhysicalStep[0] = (PhysicalEndZ - PhysicalStartZ) / (double)mDataDouble.GetLength(0);

            mArrayRank = PhysicalArrayRank.Array3D;
            mArrayType = PhysicalArrayType.ComplexArray;
            mCurrentArray = mDataComplex;
        }
        #endregion

        #region Deconstructors
        ~PhysicalArray()
        {
            try
            {
                mipData = IntPtr.Zero;
                mhData.Free();
            }
            catch { }
        }

        public void Dispose()
        {
            mipData = IntPtr.Zero;
            mhData.Free();
        }
        #endregion
        #endregion

        #region Physical_World_Interactions

        public object GetValue(double Physical_Xvalue)
        {
            int index = (int)((Physical_Xvalue - mPhysicalStart[2]) / mPhysicalStep[2]);
            if (index >= 0 && index < mCurrentArray.GetLength(2))
                if (mArrayType == PhysicalArrayType.DoubleArray)
                    return mDataDouble[0, 0, index];
                else
                    return mDataComplex[0, 0, index];
            else
                throw new Exception("this is not in the physical range of data");
        }
        public object GetValueUnchecked(double Physical_Xvalue)
        {
            int index = (int)((Physical_Xvalue - mPhysicalStart[2]) / mPhysicalStep[2]);
            if (mArrayType == PhysicalArrayType.DoubleArray)
                return mDataDouble[0, 0, index];
            else
                return mDataComplex[0, 0, index];
        }

        public object GetValue(double Physical_Xvalue, double Physical_Yvalue)
        {
            int indexX = (int)((Physical_Xvalue - mPhysicalStart[2]) / mPhysicalStep[2]);
            int indexY = (int)((Physical_Yvalue - mPhysicalStart[1]) / mPhysicalStep[1]);
            if (indexX >= 0 && indexX < mCurrentArray.GetLength(2) && indexY >= 0 && indexY < mCurrentArray.GetLength(1))
                if (mArrayType == PhysicalArrayType.DoubleArray)
                    return mDataDouble[0, indexY, indexX];
                else
                    return mDataComplex[0, indexY, indexX];
            else
                throw new Exception("this is not in the physical range of data");
        }
        public object GetValueUnchecked(double Physical_Xvalue, double Physical_Yvalue)
        {
            int indexX = (int)((Physical_Xvalue - mPhysicalStart[2]) / mPhysicalStep[2]);
            int indexY = (int)((Physical_Yvalue - mPhysicalStart[1]) / mPhysicalStep[1]);
            if (mArrayType == PhysicalArrayType.DoubleArray)
                return mDataDouble[0, indexY, indexX];
            else
                return mDataComplex[0, indexY, indexX];
        }

        public object GetValue(double Physical_Xvalue, double Physical_Yvalue, double Physical_Zvalue)
        {
            int indexX = (int)((Physical_Xvalue - mPhysicalStart[2]) / mPhysicalStep[2]);
            int indexY = (int)((Physical_Yvalue - mPhysicalStart[1]) / mPhysicalStep[1]);
            int indexZ = (int)((Physical_Zvalue - mPhysicalStart[0]) / mPhysicalStep[0]);
            if (indexX >= 0 && indexX < mCurrentArray.GetLength(2) &&
                indexY >= 0 && indexY < mCurrentArray.GetLength(1) &&
                indexZ >= 0 && indexZ < mCurrentArray.GetLength(0))
                if (mArrayType == PhysicalArrayType.DoubleArray)
                    return mDataDouble[indexZ, indexY, indexX];
                else
                    return mDataComplex[indexZ, indexY, indexX];
            else
                throw new Exception("this is not in the physical range of data");
        }
        public object GetValueUnchecked(double Physical_Xvalue, double Physical_Yvalue, double Physical_Zvalue)
        {
            int indexX = (int)((Physical_Xvalue - mPhysicalStart[2]) / mPhysicalStep[2]);
            int indexY = (int)((Physical_Yvalue - mPhysicalStart[1]) / mPhysicalStep[1]);
            int indexZ = (int)((Physical_Zvalue - mPhysicalStart[0]) / mPhysicalStep[0]);
            if (mArrayType == PhysicalArrayType.DoubleArray)
                return mDataDouble[indexZ, indexY, indexX];
            else
                return mDataComplex[indexZ, indexY, indexX];
        }

        public double[] GetAxisValues(Axis DesiredAxis)
        {
            return GetPhysicalIndicies(DesiredAxis);
        }
        public double[] GetPhysicalIndicies(Axis DesiredAxis)
        {
            switch (DesiredAxis)
            {
                case Axis.XAxis:
                    double LX = mCurrentArray.GetLength(2);
                    double[] XIndicies = new double[(int)LX];
                    double stepX = (mPhysicalEnd[2] - mPhysicalStart[2]) / LX;
                    for (int i = 0; i < LX; i++)
                    {
                        XIndicies[i] = mPhysicalStart[2] + stepX * i;
                    }
                    return XIndicies;
                case Axis.YAxis:
                    double LY = mCurrentArray.GetLength(1);
                    double[] YIndicies = new double[(int)LY];
                    double stepY = (mPhysicalEnd[1] - mPhysicalStart[1]) / LY;
                    for (int j = 0; j < LY; j++)
                    {
                        YIndicies[j] = mPhysicalStart[1] + stepY * j;
                    }
                    return YIndicies;
                case Axis.ZAxis:
                    double LZ = mCurrentArray.GetLength(0);
                    double[] ZIndicies = new double[(int)LZ];
                    double stepZ = (mPhysicalEnd[0] - mPhysicalStart[0]) / LZ;
                    for (int j = 0; j < LZ; j++)
                    {
                        ZIndicies[j] = mPhysicalStart[0] + stepZ * j;
                    }
                    return ZIndicies;
            }
            return null;
        }

        public double PhysicalStart(Axis DesiredAxis)
        {
            switch (DesiredAxis)
            {
                case Axis.XAxis:
                    return mPhysicalStart[2];
                case Axis.YAxis:
                    return mPhysicalStart[1];
                case Axis.ZAxis:
                    return mPhysicalStart[0];
            }
            return 0;
        }
        public double PhysicalEnd(Axis DesiredAxis)
        {
            switch (DesiredAxis)
            {
                case Axis.XAxis:
                    return mPhysicalEnd[2];
                case Axis.YAxis:
                    return mPhysicalEnd[1];
                case Axis.ZAxis:
                    return mPhysicalEnd[0];
            }
            return 0;
        }
        public double PhysicalStep(Axis DesiredAxis)
        {
            switch (DesiredAxis)
            {
                case Axis.XAxis:
                    return mPhysicalStep[2];
                case Axis.YAxis:
                    return mPhysicalStep[1];
                case Axis.ZAxis:
                    return mPhysicalStep[0];
            }
            return 0;
        }
        public int PhysicalMidPointIndex(Axis DesiredAxis)
        {
            switch (DesiredAxis)
            {
                case Axis.XAxis:
                    return (int)(-1 * mPhysicalStart[2] / mPhysicalStep[2]);
                case Axis.YAxis:
                    return (int)(-1 * mPhysicalStart[1] / mPhysicalStep[1]);
                case Axis.ZAxis:
                    return (int)(-1 * mPhysicalStart[0] / mPhysicalStep[0]);
            }
            return 0;
        }
        public double PhysicalLength(Axis DesiredAxis)
        {
            switch (DesiredAxis)
            {
                case Axis.XAxis:
                    return mPhysicalEnd[2] - mPhysicalStart[2];
                case Axis.YAxis:
                    return mPhysicalEnd[1] - mPhysicalStart[1];
                case Axis.ZAxis:
                    return mPhysicalEnd[0] - mPhysicalStart[0];
            }
            return 0;
        }

        public void PhysicalStartSet(Axis DesiredAxis, double AxisValue)
        {
            switch (DesiredAxis)
            {
                case Axis.XAxis:
                    mPhysicalStart[2] = AxisValue;
                    break;
                case Axis.YAxis:
                    mPhysicalStart[1] = AxisValue;
                    break;
                case Axis.ZAxis:
                    mPhysicalStart[0] = AxisValue;
                    break;
            }
            for (int i = 0; i < 3; i++)
            {
                mPhysicalStep[i] = (mPhysicalEnd[i] - mPhysicalStart[i]) / mCurrentArray.GetLength(i);
            }
        }
        public void PhysicalEndSet(Axis DesiredAxis, double AxisValue)
        {
            switch (DesiredAxis)
            {
                case Axis.XAxis:
                    mPhysicalEnd[2] = AxisValue;
                    break;
                case Axis.YAxis:
                    mPhysicalEnd[1] = AxisValue;
                    break;
                case Axis.ZAxis:
                    mPhysicalEnd[0] = AxisValue;
                    break;
            }
            for (int i = 0; i < 3; i++)
            {
                mPhysicalStep[i] = (mPhysicalEnd[i] - mPhysicalStart[i]) / mCurrentArray.GetLength(i);
            }
        }

        private void PhysicalStartSet(double[] AxisValue)
        {
            for (int i = 0; i < AxisValue.Length; i++)
            {
                mPhysicalStart[i] = AxisValue[i];
                mPhysicalStep[i] = (mPhysicalEnd[i] - mPhysicalStart[i]) / mCurrentArray.GetLength(i);
            }
            for (int i = 0; i < 3; i++)
            {
                mPhysicalStep[i] = (mPhysicalEnd[i] - mPhysicalStart[i]) / mCurrentArray.GetLength(i);
            }
        }
        private void PhysicalEndSet(double[] AxisValue)
        {
            for (int i = 0; i < AxisValue.Length; i++)
            {
                mPhysicalEnd[i] = AxisValue[i];
                mPhysicalStep[i] = (mPhysicalEnd[i] - mPhysicalStart[i]) / mCurrentArray.GetLength(i);
            }
            for (int i = 0; i < 3; i++)
            {
                mPhysicalStep[i] = (mPhysicalEnd[i] - mPhysicalStart[i]) / mCurrentArray.GetLength(i);
            }
        }
        #endregion

        #region Default
        private int? hash;
        public override int GetHashCode()
        {
            if (hash == null)
            {
                double result = 13;
                unsafe
                {
                    double* pout = (double*)mipData;
                    for (int i = 0; i < mCurrentArray.Length; i++)
                    {
                        result = (result * 7) + *pout;
                        pout++;
                    }
                }
                hash = (int)result;
            }
            return hash.GetValueOrDefault();
        }

        public int Length { get { return mCurrentArray.Length; } }
        public int GetLength(Axis LengthAxis)
        {
            return mCurrentArray.GetLength(2 - (int)LengthAxis);
        }
        public IEnumerator<double> GetEnumerator()
        {
            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                for (int i = 0; i < mCurrentArray.GetLength(0); i++)
                {
                    for (int j = 0; j < mCurrentArray.GetLength(1); j++)
                    {
                        for (int m = 0; m < mCurrentArray.GetLength(2); m++)
                        {
                            yield return mDataDouble[m, j, i];
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < mDataComplex.GetLength(0); i++)
                {
                    for (int j = 0; j < mDataComplex.GetLength(1); j++)
                    {
                        for (int m = 0; m < mDataComplex.GetLength(2); m++)
                        {
                            yield return mDataComplex[m, j, i].Abs();
                        }
                    }
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return this == (obj as PhysicalArray);
        }
        public bool Equals(PhysicalArray obj)
        {
            return this == obj;
        }

        public static bool operator !=(PhysicalArray x, PhysicalArray y)
        {
            return !(x == y);
        }
        #endregion

        #region OperatorOverloads

        public static bool operator ==(PhysicalArray x, PhysicalArray y)
        {
            if (ReferenceEquals(x, y)) return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

            if (x.hash.HasValue && y.hash.HasValue &&
                x.hash.GetValueOrDefault() != y.hash.GetValueOrDefault()) return false;

            if (x.mDataDouble.Length != y.mDataDouble.Length) return false;

            if (x.mPhysicalStart != y.mPhysicalStart) return false;
            if (x.mPhysicalEnd != y.mPhysicalEnd) return false;

            if (x.mArrayType != y.mArrayType) return false;

            unsafe
            {
                double* pX = (double*)x.mipData;
                double* pY = (double*)y.mipData;
                int Length = x.mCurrentArray.Length;
                for (int i = 0; i < Length; i++)
                {
                    if ((*pX) != (*pY)) return false;
                    pX++;
                    pY++;
                }
            }
            return true;
        }

        private enum MathType
        {
            Addition, Subtraction, Division, Multiplication
        }

        #region DoubleOverloads
        public static PhysicalArray operator *(PhysicalArray x, double y)
        {
            if (x == null) throw new ArgumentNullException();

            if (x.mArrayType == PhysicalArrayType.DoubleArray)
            {
                double[, ,] result = new double[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (double* pResult = result)
                    {
                        double* pX = (double*)x.mipData;
                        double* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) * y);
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = result;
                result = null;
                return nArray;
            }
            else
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {
                        complex* pX = (complex*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) * y);
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
        }
        public static PhysicalArray operator *(double y, PhysicalArray x)
        {
            return x * y;
        }

        public static PhysicalArray operator +(PhysicalArray x, double y)
        {
            if (x == null) throw new ArgumentNullException();

            if (x.mArrayType == PhysicalArrayType.DoubleArray)
            {
                double[, ,] result = new double[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (double* pResult = result)
                    {
                        double* pX = (double*)x.mipData;
                        double* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) + y);
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = result;
                result = null;
                return nArray;
            }
            else
            {

                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {
                        complex* pX = (complex*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) + y);
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
        }
        public static PhysicalArray operator +(double y, PhysicalArray x)
        {
            return x + y;
        }

        public static PhysicalArray operator -(PhysicalArray x, double y)
        {
            return x + (-1 * y);
        }
        public static PhysicalArray operator -(double y, PhysicalArray x)
        {
            if (x == null) throw new ArgumentNullException();

            if (x.mArrayType == PhysicalArrayType.DoubleArray)
            {
                double[, ,] result = new double[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (double* pResult = result)
                    {
                        double* pX = (double*)x.mipData;
                        double* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = (y - (*pX));
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = result;
                result = null;
                return nArray;
            }
            else
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {
                        complex* pX = (complex*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = (y - (*pX));
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;

            }
        }

        public static PhysicalArray operator /(PhysicalArray x, double y)
        {
            return x * (1 / y);
        }
        public static PhysicalArray operator /(double y, PhysicalArray x)
        {
            if (x == null) throw new ArgumentNullException();

            if (x.mArrayType == PhysicalArrayType.DoubleArray)
            {
                double[, ,] result = new double[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (double* pResult = result)
                    {
                        double* pX = (double*)x.mipData;
                        double* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = (y / (*pX));
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = result;
                result = null;
                return nArray;
            }
            else
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {
                        complex* pX = (complex*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = (y / (*pX));
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
        }
        #endregion
        #region ComplexOverloads
        public static PhysicalArray operator *(PhysicalArray x, complex y)
        {
            if (x == null) throw new ArgumentNullException();

            if (x.mArrayType == PhysicalArrayType.DoubleArray)
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {
                        double* pX = (double*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) * y);
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
            else
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {
                        complex* pX = (complex*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) * y);
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
        }
        public static PhysicalArray operator *(complex y, PhysicalArray x)
        {
            return x * y;
        }

        public static PhysicalArray operator +(PhysicalArray x, complex y)
        {
            if (x == null) throw new ArgumentNullException();

            if (x.mArrayType == PhysicalArrayType.DoubleArray)
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {
                        double* pX = (double*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) + y);
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
            else
            {

                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {
                        complex* pX = (complex*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) + y);
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
        }
        public static PhysicalArray operator +(complex y, PhysicalArray x)
        {
            return x + y;
        }

        public static PhysicalArray operator -(PhysicalArray x, complex y)
        {
            return x + (-1 * y);
        }
        public static PhysicalArray operator -(complex y, PhysicalArray x)
        {
            if (x == null) throw new ArgumentNullException();

            if (x.mArrayType == PhysicalArrayType.DoubleArray)
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {
                        double* pX = (double*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = (y - (*pX));
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
            else
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {
                        complex* pX = (complex*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = (y - (*pX));
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;

            }
        }

        public static PhysicalArray operator /(PhysicalArray x, complex y)
        {
            return x * (1 / y);
        }
        public static PhysicalArray operator /(complex y, PhysicalArray x)
        {
            if (x == null) throw new ArgumentNullException();

            if (x.mArrayType == PhysicalArrayType.DoubleArray)
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {
                        double* pX = (double*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = (y / (*pX));
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
            else
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {
                        complex* pX = (complex*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = (y / (*pX));
                            pOut++;
                            pX++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
        }
        #endregion
        #region PhysicalArrayOverloads
        public static PhysicalArray operator *(PhysicalArray x, PhysicalArray y)
        {
            if (x == null || y == null) throw new ArgumentNullException();
            if (x.mCurrentArray.GetLength(0) != y.mCurrentArray.GetLength(0))
                if (x.mCurrentArray.GetLength(1) != y.mCurrentArray.GetLength(1))
                    if (x.mCurrentArray.GetLength(2) != y.mCurrentArray.GetLength(2))
                    {
                        throw new Exception("Arrays are not equal in size, you must use the interpolate functions");
                    }

            if (x.mArrayType == PhysicalArrayType.DoubleArray && y.mArrayType == PhysicalArrayType.DoubleArray)
            {
                double[, ,] result = new double[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (double* pResult = result)
                    {

                        double* pX = (double*)x.mipData;
                        double* pY = (double*)y.mipData;
                        double* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) * (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = result;
                result = null;
                return nArray;
            }
            else if (x.mArrayType == PhysicalArrayType.ComplexArray && y.mArrayType == PhysicalArrayType.ComplexArray)
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {

                        complex* pX = (complex*)x.mipData;
                        complex* pY = (complex*)y.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) * (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
            else
            {
                PhysicalArray cArray;
                PhysicalArray dArray;
                if (x.mArrayType == PhysicalArrayType.ComplexArray)
                {
                    cArray = x;
                    dArray = y;
                }
                else
                {
                    cArray = y;
                    dArray = x;
                }

                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {

                        complex* pX = (complex*)cArray.mipData;
                        double* pY = (double*)dArray.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) * (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;

            }
        }
        public static PhysicalArray operator +(PhysicalArray x, PhysicalArray y)
        {
            if (x == null || y == null) throw new ArgumentNullException();
            if (x.mCurrentArray.GetLength(0) != y.mCurrentArray.GetLength(0))
                if (x.mCurrentArray.GetLength(1) != y.mCurrentArray.GetLength(1))
                    if (x.mCurrentArray.GetLength(2) != y.mCurrentArray.GetLength(2))
                    {
                        throw new Exception("Arrays are not equal in size, you must use the interpolate functions");
                    }

            if (x.mArrayType == PhysicalArrayType.DoubleArray && y.mArrayType == PhysicalArrayType.DoubleArray)
            {
                double[, ,] result = new double[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (double* pResult = result)
                    {

                        double* pX = (double*)x.mipData;
                        double* pY = (double*)y.mipData;
                        double* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) + (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = result;
                result = null;
                return nArray;
            }
            else if (x.mArrayType == PhysicalArrayType.ComplexArray && y.mArrayType == PhysicalArrayType.ComplexArray)
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {

                        complex* pX = (complex*)x.mipData;
                        complex* pY = (complex*)y.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) + (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
            else
            {
                PhysicalArray cArray;
                PhysicalArray dArray;
                if (x.mArrayType == PhysicalArrayType.ComplexArray)
                {
                    cArray = x;
                    dArray = y;
                }
                else
                {
                    cArray = y;
                    dArray = x;
                }

                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {

                        complex* pX = (complex*)cArray.mipData;
                        double* pY = (double*)dArray.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) + (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;

            }
        }
        public static PhysicalArray operator -(PhysicalArray x, PhysicalArray y)
        {
            if (x == null || y == null) throw new ArgumentNullException();
            if (x.mCurrentArray.GetLength(0) != y.mCurrentArray.GetLength(0))
                if (x.mCurrentArray.GetLength(1) != y.mCurrentArray.GetLength(1))
                    if (x.mCurrentArray.GetLength(2) != y.mCurrentArray.GetLength(2))
                    {
                        throw new Exception("Arrays are not equal in size, you must use the interpolate functions");
                    }

            if (x.mArrayType == PhysicalArrayType.DoubleArray && y.mArrayType == PhysicalArrayType.DoubleArray)
            {
                double[, ,] result = new double[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (double* pResult = result)
                    {

                        double* pX = (double*)x.mipData;
                        double* pY = (double*)y.mipData;
                        double* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) - (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = result;
                result = null;
                return nArray;
            }
            else if (x.mArrayType == PhysicalArrayType.ComplexArray && y.mArrayType == PhysicalArrayType.ComplexArray)
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {

                        complex* pX = (complex*)x.mipData;
                        complex* pY = (complex*)y.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) - (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
            else if (x.mArrayType == PhysicalArrayType.ComplexArray && y.mArrayType == PhysicalArrayType.DoubleArray)
            {

                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {

                        complex* pX = (complex*)x.mipData;
                        double* pY = (double*)y.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) - (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;

            }
            else if (x.mArrayType == PhysicalArrayType.DoubleArray && y.mArrayType == PhysicalArrayType.ComplexArray)
            {

                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {

                        complex* pY = (complex*)y.mipData;
                        double* pX = (double*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) - (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;

            }
            return null;
        }
        public static PhysicalArray operator /(PhysicalArray y, PhysicalArray x)
        {
            if (x == null || y == null) throw new ArgumentNullException();
            if (x.mCurrentArray.GetLength(0) != y.mCurrentArray.GetLength(0))
                if (x.mCurrentArray.GetLength(1) != y.mCurrentArray.GetLength(1))
                    if (x.mCurrentArray.GetLength(2) != y.mCurrentArray.GetLength(2))
                    {
                        throw new Exception("Arrays are not equal in size, you must use the interpolate functions");
                    }

            if (x.mArrayType == PhysicalArrayType.DoubleArray && y.mArrayType == PhysicalArrayType.DoubleArray)
            {
                double[, ,] result = new double[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (double* pResult = result)
                    {

                        double* pX = (double*)x.mipData;
                        double* pY = (double*)y.mipData;
                        double* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) / (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = result;
                result = null;
                return nArray;
            }
            else if (x.mArrayType == PhysicalArrayType.ComplexArray && y.mArrayType == PhysicalArrayType.ComplexArray)
            {
                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {

                        complex* pX = (complex*)x.mipData;
                        complex* pY = (complex*)y.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) / (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;
            }
            else if (x.mArrayType == PhysicalArrayType.ComplexArray && y.mArrayType == PhysicalArrayType.DoubleArray)
            {

                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {

                        complex* pX = (complex*)x.mipData;
                        double* pY = (double*)y.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) / (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;

            }
            else if (x.mArrayType == PhysicalArrayType.DoubleArray && y.mArrayType == PhysicalArrayType.ComplexArray)
            {

                complex[, ,] result = new complex[x.mDataDouble.GetLength(0), x.mDataDouble.GetLength(1), x.mDataDouble.GetLength(2)];
                unsafe
                {
                    fixed (complex* pResult = result)
                    {

                        complex* pY = (complex*)y.mipData;
                        double* pX = (double*)x.mipData;
                        complex* pOut = pResult;
                        int Length = x.mDataDouble.Length;
                        for (int i = 0; i < Length; i++)
                        {
                            *pOut = ((*pX) / (*pY));
                            pOut++;
                            pX++;
                            pY++;
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(x, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = result;
                result = null;
                return nArray;

            }
            return null;
        }
        #endregion


        #endregion

        #region Resize_Array

        /// <summary>
        /// Pads out an array to a new length with zeros on the outside and the former data centered
        /// If numpoints is less than the current number of points, then the function just returns.
        /// </summary>
        /// <param name="DesiredAxis"></param>
        /// <param name="NumPoints"></param>
        public void ZeroPad_DataCentered_InPlace(Axis DesiredAxis, int NumPoints)
        {
            ZeroPad_DataCentered(this, this, DesiredAxis, NumPoints);
        }

        /// <summary>
        /// Pads out the data with the Zeros put into the center and the data on the outside.  Ignores the request if the 
        /// number of points are less than the current array
        /// </summary>
        /// <param name="DesiredAxis"></param>
        /// <param name="NumPoints"></param>
        public void ZeroPad_ZerosCenter_InPlace(Axis DesiredAxis, int NumPoints)
        {
            ZeroPad_ZerosCentered(this, this, DesiredAxis, NumPoints);
        }
        public void TruncateDataInPlace(Axis DesiredAxis, double PhysicalStart, double PhysicalEnd)
        {
            TruncateData(this, this, DesiredAxis, PhysicalStart, PhysicalEnd);
        }

        public PhysicalArray ZeroPad_DataCentered(Axis DesiredAxis, int NumPoints)
        {
            PhysicalArray pa = new PhysicalArray(this, this.mArrayType, false);
            ZeroPad_DataCentered(this, pa, DesiredAxis, NumPoints);
            return pa;

        }
        public PhysicalArray ZeroPad_ZerosCenter(Axis DesiredAxis, int NumPoints)
        {
            PhysicalArray pa = new PhysicalArray(this, this.mArrayType, false);
            ZeroPad_ZerosCentered(this, pa, DesiredAxis, NumPoints);
            pa.mArrayRank = mArrayRank;
            pa.mArrayType = mArrayType;
            return pa;
        }
        public PhysicalArray TruncateData(Axis DesiredAxis, double PhysicalStart, double PhysicalEnd)
        {
            PhysicalArray pa = new PhysicalArray(this, this.mArrayType, false);
            TruncateData(this, pa, DesiredAxis, PhysicalStart, PhysicalEnd);
            pa.mArrayRank = mArrayRank;
            pa.mArrayType = mArrayType;
            return pa;
        }

        #region StaticResizeFunctions
        private static void TruncateData(PhysicalArray PhysArrayIn, PhysicalArray PhysArrayOut, Axis DesiredAxis, double PhysicalStart, double PhysicalEnd)
        {
            int DesiredAxisIndex = 2 - (int)DesiredAxis;
            double step = PhysArrayIn.mPhysicalStep[DesiredAxisIndex];
            int sI = (int)((PhysicalStart - PhysArrayIn.mPhysicalStart[DesiredAxisIndex]) / step);
            int eI = (int)((PhysicalEnd - PhysArrayIn.mPhysicalStart[DesiredAxisIndex]) / step);


            if (sI < 0 || eI > PhysArrayIn.mCurrentArray.GetLength(DesiredAxisIndex))
                throw new Exception("Must truncate data within its physical range");

            double OldLength = PhysArrayIn.mCurrentArray.GetLength(DesiredAxisIndex);
            GCHandle hData = new GCHandle();
            IntPtr ipData = new IntPtr();

            double[] nPhysicalStart = new double[3];
            double[] nPhysicalEnd = new double[3];
            double[] nPhysicalStep = new double[3];
            for (int i = 0; i < 3; i++)
            {
                nPhysicalStart[i] = PhysArrayIn.mPhysicalStart[i];
                nPhysicalEnd[i] = PhysArrayIn.mPhysicalEnd[i];
                nPhysicalStep[i] = PhysArrayIn.mPhysicalStep[i];
            }
            nPhysicalStart[DesiredAxisIndex] = PhysicalStart;
            nPhysicalEnd[DesiredAxisIndex] = PhysicalEnd;
            nPhysicalStep[DesiredAxisIndex] = (PhysicalEnd - PhysicalStart) / (double)(eI - sI);

            if (PhysArrayIn.ArrayType == PhysicalArrayType.DoubleArray)
            {
                #region DoubleArray
                double[, ,] nData = null;
                if (DesiredAxis == Axis.XAxis)
                {
                    #region XAxis
                    nData = new double[PhysArrayIn.mDataDouble.GetLength(0), PhysArrayIn.mDataDouble.GetLength(1), eI - sI];
                    hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
                    ipData = hData.AddrOfPinnedObject();

                    unsafe
                    {
                        int offsetZOut = nData.GetLength(1) * nData.GetLength(2);
                        int offsetYout = nData.GetLength(2);
                        int offsetZIn = PhysArrayIn.mCurrentArray.GetLength(1) * PhysArrayIn.mCurrentArray.GetLength(2);
                        int offsetYIn = PhysArrayIn.mCurrentArray.GetLength(2);
                        for (int Z = 0; Z < PhysArrayIn.GetLength(Axis.ZAxis); Z++)
                        {
                            for (int Y = 0; Y < PhysArrayIn.GetLength(Axis.YAxis); Y++)
                            {
                                int Offset = Z * offsetZOut + Y * offsetYout;
                                double* pOut = ((double*)ipData) + Offset;
                                double* pData = ((double*)PhysArrayIn.mipData) + sI + Z * offsetZIn + Y * offsetYIn;
                                for (int X = sI; X < eI; X++)
                                {
                                    *pOut = *pData;
                                    pOut++;
                                    pData++;
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (DesiredAxis == Axis.YAxis)
                {
                    #region YAxis
                    nData = new double[PhysArrayIn.mDataDouble.GetLength(0), eI - sI, PhysArrayIn.mDataDouble.GetLength(2)];
                    hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
                    ipData = hData.AddrOfPinnedObject();

                    unsafe
                    {
                        int offsetZOut = nData.GetLength(1) * nData.GetLength(2);
                        int offsetYout = nData.GetLength(2);
                        int offsetZIn = PhysArrayIn.mCurrentArray.GetLength(1) * PhysArrayIn.mCurrentArray.GetLength(2);
                        int offsetYIn = PhysArrayIn.mCurrentArray.GetLength(2);
                        for (int Z = 0; Z < nData.GetLength(0); Z++)
                        {
                            for (int X = 0; X < nData.GetLength(2); X++)
                            {
                                /* int Offset = Z * offsetZOut + X;
                                 double* pOut = ((double*)ipData) + Offset;
                                 double* pData = ((double*)PhysArrayIn.mipData) + sI + Z * offsetZIn + X;*/
                                int cc = 0;
                                for (int Y = sI; Y < eI; Y++)
                                {
                                    nData[Z, cc, X] = PhysArrayIn.mDataDouble[Z, Y, X];
                                    cc++;
                                    /**pOut = *pData;
                                    pOut += offsetYout;
                                    pData += offsetYIn;*/
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (DesiredAxis == Axis.ZAxis)
                {
                    #region ZAxis
                    nData = new double[PhysArrayIn.mDataDouble.GetLength(0), eI - sI, PhysArrayIn.mDataDouble.GetLength(2)];
                    hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
                    ipData = hData.AddrOfPinnedObject();

                    unsafe
                    {
                        int offsetZOut = nData.GetLength(1) * nData.GetLength(2);
                        int offsetYout = nData.GetLength(2);
                        int offsetZIn = PhysArrayIn.mCurrentArray.GetLength(1) * PhysArrayIn.mCurrentArray.GetLength(2);
                        int offsetYIn = PhysArrayIn.mCurrentArray.GetLength(2);
                        for (int X = 0; X < PhysArrayIn.GetLength(Axis.XAxis); X++)
                        {
                            for (int Y = sI; Y < eI; Y++)
                            {
                                int Offset = Y * offsetYout + X;
                                double* pOut = ((double*)ipData) + Offset;
                                double* pData = ((double*)PhysArrayIn.mipData) + sI + Y * offsetYIn + X;
                                for (int Z = sI; Z < eI; Z++)
                                {
                                    *pOut = *pData;
                                    pOut += offsetZOut;
                                    pData += offsetZIn;
                                }
                            }
                        }
                    }
                    #endregion
                }

                try
                {
                    PhysArrayOut.mhData.Free();
                }
                catch { }

                PhysArrayOut.mDataDouble = nData;
                PhysArrayOut.mCurrentArray = nData;
                PhysArrayOut.mhData = hData;
                PhysArrayOut.mPhysicalStart = nPhysicalStart;
                PhysArrayOut.mPhysicalEnd = nPhysicalEnd;
                PhysArrayOut.mPhysicalStep = nPhysicalStep;
                PhysArrayOut.mipData = ipData;
                nData = null;
                #endregion
            }
            else
            {
                #region ComplexArray
                complex[, ,] nData = null;
                if (DesiredAxis == Axis.XAxis)
                {
                    #region XAxis
                    nData = new complex[PhysArrayIn.mDataDouble.GetLength(0), PhysArrayIn.mDataDouble.GetLength(1), eI - sI];
                    hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
                    ipData = hData.AddrOfPinnedObject();

                    unsafe
                    {
                        int offsetZOut = nData.GetLength(1) * nData.GetLength(2);
                        int offsetYout = nData.GetLength(2);
                        int offsetZIn = PhysArrayIn.mCurrentArray.GetLength(1) * PhysArrayIn.mCurrentArray.GetLength(2);
                        int offsetYIn = PhysArrayIn.mCurrentArray.GetLength(2);
                        for (int Z = 0; Z < PhysArrayIn.GetLength(Axis.ZAxis); Z++)
                        {
                            for (int Y = 0; Y < PhysArrayIn.GetLength(Axis.YAxis); Y++)
                            {
                                int Offset = Z * offsetZOut + Y * offsetYout;
                                complex* pOut = ((complex*)ipData) + Offset;
                                complex* pData = ((complex*)PhysArrayIn.mipData) + sI + Z * offsetZIn + Y * offsetYIn;
                                for (int X = sI; X < eI; X++)
                                {
                                    *pOut = *pData;
                                    pOut++;
                                    pData++;
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (DesiredAxis == Axis.YAxis)
                {
                    #region YAxis
                    nData = new complex[PhysArrayIn.mDataDouble.GetLength(0), eI - sI, PhysArrayIn.mDataDouble.GetLength(2)];
                    hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
                    ipData = hData.AddrOfPinnedObject();

                    unsafe
                    {
                        int offsetZOut = nData.GetLength(1) * nData.GetLength(2);
                        int offsetYout = nData.GetLength(2);
                        int offsetZIn = PhysArrayIn.mCurrentArray.GetLength(1) * PhysArrayIn.mCurrentArray.GetLength(2);
                        int offsetYIn = PhysArrayIn.mCurrentArray.GetLength(2);
                        for (int Z = 0; Z < PhysArrayIn.GetLength(Axis.ZAxis); Z++)
                        {
                            for (int X = 0; X < PhysArrayIn.GetLength(Axis.XAxis); X++)
                            {
                                int Offset = Z * offsetZOut + X;
                                complex* pOut = ((complex*)ipData) + Offset;
                                complex* pData = ((complex*)PhysArrayIn.mipData) + sI + Z * offsetZIn + X;

                                for (int Y = sI; Y < eI; Y++)
                                {

                                    *pOut = *pData;
                                    pOut += offsetYout;
                                    pData += offsetYIn;
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (DesiredAxis == Axis.ZAxis)
                {
                    #region ZAxis
                    nData = new complex[PhysArrayIn.mDataDouble.GetLength(0), eI - sI, PhysArrayIn.mDataDouble.GetLength(2)];
                    hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
                    ipData = hData.AddrOfPinnedObject();

                    unsafe
                    {
                        int offsetZOut = nData.GetLength(1) * nData.GetLength(2);
                        int offsetYout = nData.GetLength(2);
                        int offsetZIn = PhysArrayIn.mCurrentArray.GetLength(1) * PhysArrayIn.mCurrentArray.GetLength(2);
                        int offsetYIn = PhysArrayIn.mCurrentArray.GetLength(2);
                        for (int X = 0; X < PhysArrayIn.GetLength(Axis.XAxis); X++)
                        {
                            for (int Y = sI; Y < eI; Y++)
                            {
                                int Offset = Y * offsetYout + X;
                                complex* pOut = ((complex*)ipData) + Offset;
                                complex* pData = ((complex*)PhysArrayIn.mipData) + sI + Y * offsetYIn + X;
                                for (int Z = sI; Z < eI; Z++)
                                {
                                    *pOut = *pData;
                                    pOut += offsetZOut;
                                    pData += offsetZIn;
                                }
                            }
                        }
                    }
                    #endregion
                }

                try
                {
                    PhysArrayOut.mhData.Free();
                }
                catch { }

                PhysArrayOut.mDataComplex = nData;
                PhysArrayOut.mCurrentArray = nData;
                PhysArrayOut.mhData = hData;
                PhysArrayOut.mPhysicalStart = nPhysicalStart;
                PhysArrayOut.mPhysicalEnd = nPhysicalEnd;
                PhysArrayOut.mPhysicalStep = nPhysicalStep;
                PhysArrayOut.mipData = ipData;
                nData = null;
                #endregion
            }

        }

        private static void ZeroPad_DataCentered(
            PhysicalArray PhysArrayIn,
            PhysicalArray PhysArrayOut,
            Axis DesiredAxis, int NumPoints)
        {
            int DesiredAxisIndex = 2 - (int)DesiredAxis;
            if (NumPoints < PhysArrayIn.mCurrentArray.GetLength(DesiredAxisIndex))
                return;

            double OldLength = PhysArrayIn.mCurrentArray.GetLength(DesiredAxisIndex);
            GCHandle hData = new GCHandle();
            IntPtr ipData = new IntPtr();

            double a = NumPoints / OldLength;
            double L = PhysArrayIn.mPhysicalEnd[DesiredAxisIndex] - PhysArrayIn.mPhysicalStart[DesiredAxisIndex];
            double nPhysicalEnd = (a * L + PhysArrayIn.mPhysicalStart[DesiredAxisIndex] + PhysArrayIn.mPhysicalEnd[DesiredAxisIndex]) / 2;
            double nPhysicalStart = (PhysArrayIn.mPhysicalStart[DesiredAxisIndex] + PhysArrayIn.mPhysicalEnd[DesiredAxisIndex] - nPhysicalEnd);
            double nPhysicalStep = (nPhysicalEnd - nPhysicalStart) / NumPoints;

            double[] PhysicalStart = new double[3];
            double[] PhysicalEnd = new double[3];
            double[] PhysicalStep = new double[3];
            for (int i = 0; i < 3; i++)
            {
                PhysicalStart[i] = PhysArrayIn.mPhysicalStart[i];
                PhysicalEnd[i] = PhysArrayIn.mPhysicalEnd[i];
                PhysicalStep[i] = PhysArrayIn.mPhysicalStep[i];
            }
            PhysicalStart[DesiredAxisIndex] = nPhysicalStart;
            PhysicalEnd[DesiredAxisIndex] = nPhysicalEnd;
            PhysicalStep[DesiredAxisIndex] = nPhysicalStep;


            if (PhysArrayIn.mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region DoubleNumbers
                int inPoints = PhysArrayIn.mDataDouble.GetLength(DesiredAxisIndex);
                int StartD = (int)((double)(NumPoints - inPoints) / 2d);
                double[, ,] nData = null;


                unsafe
                {
                    if (DesiredAxis == Axis.XAxis)
                    {
                        nData = new double[PhysArrayIn.mDataDouble.GetLength(0), PhysArrayIn.mDataDouble.GetLength(1), NumPoints];
                        hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
                        ipData = hData.AddrOfPinnedObject();

                        int offsetZOut = nData.GetLength(1) * nData.GetLength(2);
                        int offsetYout = nData.GetLength(2);
                        int offsetZIn = PhysArrayIn.mCurrentArray.GetLength(1) * PhysArrayIn.mCurrentArray.GetLength(2);
                        int offsetYIn = PhysArrayIn.mCurrentArray.GetLength(2);

                        for (int Z = 0; Z < PhysArrayIn.mDataDouble.GetLength(0); Z++)
                        {
                            for (int Y = 0; Y < PhysArrayIn.mDataDouble.GetLength(1); Y++)
                            {
                                double* pOut = ((double*)ipData) + Y * offsetYout + Z * offsetZOut + StartD;
                                double* pData = (double*)PhysArrayIn.mipData + Y * offsetYIn + Z * offsetYIn;
                                for (int X = 0; X < inPoints; X++)
                                {
                                    *pOut = *pData;
                                    pOut++;
                                    pData++;
                                }
                            }
                        }
                    }
                    else if (DesiredAxis == Axis.YAxis)
                    {
                        nData = new double[PhysArrayIn.mDataDouble.GetLength(0), NumPoints, PhysArrayIn.mDataDouble.GetLength(2)];
                        hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
                        ipData = hData.AddrOfPinnedObject();

                        int offsetZOut = nData.GetLength(1) * nData.GetLength(2);
                        int offsetYout = nData.GetLength(2);
                        int offsetZIn = PhysArrayIn.mCurrentArray.GetLength(1) * PhysArrayIn.mCurrentArray.GetLength(2);
                        int offsetYIn = PhysArrayIn.mCurrentArray.GetLength(2);

                        for (int Z = 0; Z < PhysArrayIn.mDataDouble.GetLength(0); Z++)
                        {
                            for (int X = 0; X < PhysArrayIn.mDataDouble.GetLength(2); X++)
                            {
                                double* pOut = ((double*)ipData) + X + Z * offsetZOut + StartD * offsetYout;
                                double* pData = (double*)PhysArrayIn.mipData + X + Z * offsetYIn;
                                for (int Y = 0; Y < inPoints; Y++)
                                {
                                    *pOut = *pData;
                                    pOut += offsetYout;
                                    pData += offsetYIn;
                                }
                            }
                        }
                    }
                    else if (DesiredAxis == Axis.ZAxis)
                    {
                        throw new Exception("Not yet Implimented");
                    }
                }
                try
                {
                    PhysArrayOut.mhData.Free();
                }
                catch { }

                PhysArrayOut.mDataDouble = nData;
                PhysArrayOut.mCurrentArray = nData;
                PhysArrayOut.mhData = hData;
                PhysArrayOut.mPhysicalStart = PhysicalStart;
                PhysArrayOut.mPhysicalEnd = PhysicalEnd;
                PhysArrayOut.mPhysicalStep = PhysicalStep;
                PhysArrayOut.mipData = ipData;
                nData = null;
                #endregion
            }
            else
            {
                throw new Exception("Not Yet Implimented");
                #region ComplexNumbers

                #endregion
            }


        }

        private static void ZeroPad_ZerosCentered(PhysicalArray PhysArrayIn, PhysicalArray PhysArrayOut, Axis DesiredAxis, int NumPoints)
        {
            int DesiredAxisIndex = (int)DesiredAxis;
            if (NumPoints < PhysArrayIn.mCurrentArray.GetLength(DesiredAxisIndex))
                return;


            double OldLength = 0;
            GCHandle hData = new GCHandle();
            IntPtr ipData = new IntPtr();
            int StartD = (int)((NumPoints - PhysArrayIn.mCurrentArray.GetLength(DesiredAxisIndex)) / 2d);


            double a = NumPoints / OldLength;
            double L = PhysArrayIn.mPhysicalEnd[DesiredAxisIndex] - PhysArrayIn.mPhysicalStart[DesiredAxisIndex];
            double nPhysicalEnd = (a * L + PhysArrayIn.mPhysicalStart[DesiredAxisIndex] + PhysArrayIn.mPhysicalEnd[DesiredAxisIndex]) / 2;
            double nPhysicalStart = (PhysArrayIn.mPhysicalStart[DesiredAxisIndex] + PhysArrayIn.mPhysicalEnd[DesiredAxisIndex] - nPhysicalEnd);


            double[] PhysicalStart = new double[3];
            double[] PhysicalEnd = new double[3];
            double[] PhysicalStep = new double[3];
            for (int i = 0; i < 3; i++)
            {
                PhysicalStart[i] = PhysArrayIn.mPhysicalStart[i];
                PhysicalEnd[i] = PhysArrayIn.mPhysicalEnd[i];
                PhysicalStep[i] = PhysArrayIn.mPhysicalStep[i];
            }
            PhysicalStart[DesiredAxisIndex] = nPhysicalStart;
            PhysicalEnd[DesiredAxisIndex] = nPhysicalEnd;
            PhysicalStep[DesiredAxisIndex] = (PhysicalEnd[DesiredAxisIndex] - PhysicalStart[DesiredAxisIndex]) / NumPoints;


            if (PhysArrayIn.mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region DoubleNumbers
                double[, ,] nData = null;
                unsafe
                {
                    if (DesiredAxis == Axis.XAxis)
                    {
                        nData = new double[PhysArrayIn.mCurrentArray.GetLength(0), PhysArrayIn.mCurrentArray.GetLength(1), NumPoints];
                        hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
                        ipData = hData.AddrOfPinnedObject();

                        int offsetZOut = nData.GetLength(1) * nData.GetLength(2);
                        int offsetYout = nData.GetLength(2);
                        int offsetZIn = PhysArrayIn.mCurrentArray.GetLength(1) * PhysArrayIn.mCurrentArray.GetLength(2);
                        int offsetYIn = PhysArrayIn.mCurrentArray.GetLength(2);

                        int HalfLength = nData.GetLength(2) / 2;
                        for (int Z = 0; Z < nData.GetLength(0); Z++)
                        {
                            for (int Y = 0; Y < nData.GetLength(1); Y++)
                            {
                                int offsetOut = Z * offsetZOut + Y * offsetYout;
                                int offsetIn = Z * offsetZIn + Y * offsetYIn;
                                double* pOut = ((double*)ipData) + offsetOut;
                                double* pOutE = ((double*)ipData) + nData.GetLength(2) + offsetOut;
                                double* pData = (double*)PhysArrayIn.mipData + offsetIn;
                                double* pDataE = ((double*)PhysArrayIn.mipData) + nData.GetLength(2) + offsetIn;
                                for (int X = 0; X < HalfLength; X++)
                                {
                                    *pOut = *pData;
                                    pOut++;
                                    pData++;

                                    *pOutE = *pDataE;
                                    pOutE--;
                                    pDataE--;
                                }
                            }
                        }
                    }
                    else if (DesiredAxis == Axis.YAxis)
                    {
                        throw new Exception("Not yet Implimented");
                    }
                    else if (DesiredAxis == Axis.ZAxis)
                    {
                        throw new Exception("Not yet Implimented");
                    }

                }
                try
                {
                    PhysArrayOut.mhData.Free();
                }
                catch { }

                PhysArrayOut.mDataDouble = nData;
                PhysArrayOut.mCurrentArray = nData;
                PhysArrayOut.mhData = hData;
                PhysArrayOut.mPhysicalStart = PhysicalStart;
                PhysArrayOut.mPhysicalEnd = PhysicalEnd;
                PhysArrayOut.mPhysicalStep = PhysicalStep;
                PhysArrayOut.mipData = ipData;
                nData = null;
                #endregion
            }
            else
            {
                throw new Exception("Not Yet implimented");
            }


        }

        private static void GetNewPhysicalDimensions(double NewNumPoints, PhysicalArray PhysArrayIn, int AxisIndex,
            out double[] PhysicalStart, out double[] PhysicalEnd, out double[] PhysicalStep)
        {
            double a = NewNumPoints / PhysArrayIn.mCurrentArray.GetLength(AxisIndex);
            double L = PhysArrayIn.mPhysicalEnd[AxisIndex] - PhysArrayIn.mPhysicalStart[AxisIndex];
            double nPhysicalEnd = (a * L + PhysArrayIn.mPhysicalStart[AxisIndex] + PhysArrayIn.mPhysicalEnd[AxisIndex]) / 2;
            double nPhysicalStart = (PhysArrayIn.mPhysicalStart[AxisIndex] + PhysArrayIn.mPhysicalEnd[AxisIndex] - nPhysicalEnd);
            double nPhysicalStep = (nPhysicalEnd - nPhysicalStart) / NewNumPoints;

            PhysicalStart = new double[3];
            PhysicalEnd = new double[3];
            PhysicalStep = new double[3];
            for (int i = 0; i < 3; i++)
            {
                PhysicalStart[i] = PhysArrayIn.mPhysicalStart[i];
                PhysicalEnd[i] = PhysArrayIn.mPhysicalEnd[i];
                PhysicalStep[i] = PhysArrayIn.mPhysicalStep[i];
            }
            PhysicalStart[AxisIndex] = nPhysicalStart;
            PhysicalEnd[AxisIndex] = nPhysicalEnd;
            PhysicalStep[AxisIndex] = nPhysicalStep;
        }

        public static double[,] SliceArray(Axis Axis1, Axis Axis2, double[, ,] InArray, int SliceNumber)
        {
            Axis[] OrderedAxis = OrderAxis(Axis1, Axis2);
            Axis1 = OrderedAxis[0];
            Axis2 = OrderedAxis[1];


            if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
            {
                double[,] OutArray = new double[InArray.GetLength(2), InArray.GetLength(1)];
                for (int i = 0; i < InArray.GetLength(2); i++)
                {
                    for (int j = 0; j < InArray.GetLength(1); j++)
                    {
                        OutArray[i, j] = InArray[SliceNumber, j, i];
                    }
                }
                return OutArray;
            }
            else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
            {
                double[,] OutArray = new double[InArray.GetLength(2), InArray.GetLength(0)];
                for (int i = 0; i < InArray.GetLength(2); i++)
                {
                    for (int j = 0; j < InArray.GetLength(0); j++)
                    {
                        OutArray[i, j] = InArray[j, SliceNumber, i];
                    }
                }
                return OutArray;
            }
            else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
            {
                double[,] OutArray = new double[InArray.GetLength(1), InArray.GetLength(0)];

                for (int i = 0; i < InArray.GetLength(1); i++)
                {
                    for (int j = 0; j < InArray.GetLength(0); j++)
                    {
                        OutArray[i, j] = InArray[j, i, SliceNumber];
                    }
                }
                return OutArray;
            }
            return null;
        }
        public static double[,] SliceAndPadZeroArray(Axis Axis1, Axis Axis2, double[, ,] InArray, int SliceNumber, int PaddingX, int PaddingY)
        {
            Axis[] OrderedAxis = OrderAxis(Axis1, Axis2);
            Axis1 = OrderedAxis[0];
            Axis2 = OrderedAxis[1];

            double[,] OutArray = null;
            if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
            {
                if (InArray.GetLength(2) >= PaddingX && InArray.GetLength(1) >= PaddingY)
                    return SliceArray(Axis1, Axis2, InArray, SliceNumber);

                OutArray = new double[PaddingX, PaddingY];
                int OffsetX = (int)((InArray.GetLength(2) - PaddingX) / -2d);
                int OffsetY = (int)((InArray.GetLength(1) - PaddingY) / -2d);

                for (int i = 0; i < InArray.GetLength(2); i++)
                {
                    for (int j = 0; j < InArray.GetLength(1); j++)
                    {
                        OutArray[i + OffsetX, j + OffsetY] = InArray[SliceNumber, j, i];
                    }
                }
            }
            else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
            {
                if (InArray.GetLength(2) >= PaddingX && InArray.GetLength(0) >= PaddingY)
                    return SliceArray(Axis1, Axis2, InArray, SliceNumber);

                OutArray = new double[PaddingX, PaddingY];
                int OffsetX = (int)((InArray.GetLength(2) - PaddingX) / -2d);
                int OffsetY = (int)((InArray.GetLength(0) - PaddingY) / -2d);

                for (int i = 0; i < InArray.GetLength(2); i++)
                {
                    for (int j = 0; j < InArray.GetLength(0); j++)
                    {
                        OutArray[i + OffsetX, j + OffsetY] = InArray[j, SliceNumber, i];
                    }
                }
            }
            else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
            {
                if (InArray.GetLength(1) >= PaddingX && InArray.GetLength(0) >= PaddingY)
                    return SliceArray(Axis1, Axis2, InArray, SliceNumber);

                OutArray = new double[PaddingX, PaddingY];
                int OffsetX = (int)((InArray.GetLength(1) - PaddingX) / -2d);
                int OffsetY = (int)((InArray.GetLength(0) - PaddingY) / -2d);

                for (int i = 0; i < InArray.GetLength(1); i++)
                {
                    for (int j = 0; j < InArray.GetLength(0); j++)
                    {
                        OutArray[i + OffsetX, j + OffsetY] = InArray[j, i, SliceNumber];
                    }
                }
            }
            return OutArray;
        }
        public static float[,] SliceAndPadZeroArrayToFloat(Axis Axis1, Axis Axis2, double[, ,] InArray, int SliceNumber, int PaddingX, int PaddingY)
        {
            Axis[] OrderedAxis = OrderAxis(Axis1, Axis2);
            Axis1 = OrderedAxis[0];
            Axis2 = OrderedAxis[1];

            float[,] OutArray = null;
            if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
            {
                OutArray = new float[PaddingX, PaddingY];
                int OffsetX = (int)((InArray.GetLength(2) - PaddingX) / -2d);
                int OffsetY = (int)((InArray.GetLength(1) - PaddingY) / -2d);

                for (int i = 0; i < InArray.GetLength(2); i++)
                {
                    for (int j = 0; j < InArray.GetLength(1); j++)
                    {
                        OutArray[i + OffsetX, j + OffsetY] = (float)InArray[SliceNumber, j, i];
                    }
                }
            }
            else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
            {
                OutArray = new float[PaddingX, PaddingY];
                int OffsetX = (int)((InArray.GetLength(2) - PaddingX) / -2d);
                int OffsetY = (int)((InArray.GetLength(0) - PaddingY) / -2d);

                for (int i = 0; i < InArray.GetLength(2); i++)
                {
                    for (int j = 0; j < InArray.GetLength(0); j++)
                    {
                        OutArray[i + OffsetX, j + OffsetY] = (float)InArray[j, SliceNumber, i];
                    }
                }
            }
            else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
            {

                OutArray = new float[PaddingX, PaddingY];
                int OffsetX = (int)((InArray.GetLength(1) - PaddingX) / -2d);
                int OffsetY = (int)((InArray.GetLength(0) - PaddingY) / -2d);

                for (int i = 0; i < InArray.GetLength(1); i++)
                {
                    for (int j = 0; j < InArray.GetLength(0); j++)
                    {
                        OutArray[i + OffsetX, j + OffsetY] = (float)InArray[j, i, SliceNumber];
                    }
                }
            }
            return OutArray;
        }
        private static void RewritePaddedData(Axis Axis1, Axis Axis2, ref double[, ,] MasterArray, int SliceNumber, double[,] PaddedArray)
        {
            Axis[] OrderedAxis = OrderAxis(Axis1, Axis2);
            Axis1 = OrderedAxis[0];
            Axis2 = OrderedAxis[1];
            if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
            {
                int OffsetX = (int)((double)(MasterArray.GetLength(2) - PaddedArray.GetLength(0)) / -2d);
                int OffsetY = (int)((double)(MasterArray.GetLength(1) - PaddedArray.GetLength(1)) / -2d);

                for (int i = 0; i < MasterArray.GetLength(2); i++)
                {
                    for (int j = 0; j < MasterArray.GetLength(1); j++)
                    {
                        MasterArray[SliceNumber, j, i] = PaddedArray[i + OffsetX, (j + OffsetY)];
                    }
                }
            }
            else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
            {
                int OffsetX = (int)((MasterArray.GetLength(2) - PaddedArray.GetLength(0)) / -2d);
                int OffsetY = (int)((MasterArray.GetLength(0) - PaddedArray.GetLength(1)) / -2d);

                for (int i = 0; i < MasterArray.GetLength(0); i++)
                {
                    for (int j = 0; j < MasterArray.GetLength(2); j++)
                    {
                        MasterArray[i, SliceNumber, j] = PaddedArray[i + OffsetX, j + OffsetY];
                    }
                }
            }
            else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
            {
                int OffsetX = (int)((MasterArray.GetLength(0) - PaddedArray.GetLength(0)) / -2d);
                int OffsetY = (int)((MasterArray.GetLength(1) - PaddedArray.GetLength(1)) / -2d);

                for (int i = 0; i < MasterArray.GetLength(1); i++)
                {
                    for (int j = 0; j < MasterArray.GetLength(0); j++)
                    {
                        MasterArray[j, i, SliceNumber] = PaddedArray[i + OffsetX, j + OffsetY];
                    }
                }
            }
        }
        private static void RewritePaddedData(Axis Axis1, Axis Axis2, ref double[, ,] MasterArray, int SliceNumber, float[,] PaddedArray)
        {
            Axis[] OrderedAxis = OrderAxis(Axis1, Axis2);
            Axis1 = OrderedAxis[0];
            Axis2 = OrderedAxis[1];
            if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
            {
                int OffsetX = (int)((double)(MasterArray.GetLength(2) - PaddedArray.GetLength(0)) / -2d);
                int OffsetY = (int)((double)(MasterArray.GetLength(1) - PaddedArray.GetLength(1)) / -2d);

                for (int i = 0; i < MasterArray.GetLength(2); i++)
                {
                    for (int j = 0; j < MasterArray.GetLength(1); j++)
                    {
                        MasterArray[SliceNumber, j, i] = PaddedArray[i + OffsetX, (j + OffsetY)];
                    }
                }
            }
            else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
            {
                int OffsetX = (int)((MasterArray.GetLength(2) - PaddedArray.GetLength(0)) / -2d);
                int OffsetY = (int)((MasterArray.GetLength(0) - PaddedArray.GetLength(1)) / -2d);

                for (int i = 0; i < MasterArray.GetLength(0); i++)
                {
                    for (int j = 0; j < MasterArray.GetLength(2); j++)
                    {
                        MasterArray[i, SliceNumber, j] = PaddedArray[i + OffsetX, j + OffsetY];
                    }
                }
            }
            else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
            {
                int OffsetX = (int)((MasterArray.GetLength(0) - PaddedArray.GetLength(0)) / -2d);
                int OffsetY = (int)((MasterArray.GetLength(1) - PaddedArray.GetLength(1)) / -2d);

                for (int i = 0; i < MasterArray.GetLength(1); i++)
                {
                    for (int j = 0; j < MasterArray.GetLength(0); j++)
                    {
                        MasterArray[j, i, SliceNumber] = PaddedArray[i + OffsetX, j + OffsetY];
                    }
                }
            }
        }


        #endregion
        #endregion

        #region To_Conversions

        public double[] ToDoubleArray()
        {
            return (double[])ActualData1D;
        }
        public double[,] ToDoubleArrayIndexed()
        {
            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                double[,] ArrayOut;
                if (mDataDouble.GetLength(1) == 2)
                {
                    ArrayOut = new double[2, mDataDouble.GetLength(2)];
                    for (int i = 0; i < mDataDouble.GetLength(2); i++)
                    {
                        ArrayOut[0, i] = mPhysicalStart[2] + i * mPhysicalStep[2];
                        ArrayOut[1, i] = mDataDouble[0, 1, i];
                    }
                }
                else
                {
                    ArrayOut = new double[2, mDataDouble.GetLength(2)];
                    for (int i = 0; i < mDataDouble.GetLength(2); i++)
                    {
                        ArrayOut[0, i] = mPhysicalStart[2] + i * mPhysicalStep[2];
                        ArrayOut[1, i] = mDataDouble[0, 0, i];
                    }

                }
                return ArrayOut;
            }
            else
            {
                throw new Exception("Not yet implemented");
            }
        }
        public double[,] MakeGraphableArray()
        {
            return ToDoubleArrayIndexed();
        }
        public Bitmap MakeBitmap()
        {
            double[, ,] ImageArray = mDataDouble;
            int iWidth = ImageArray.GetLength(2);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int x = 0; x < iWidth; x++)
                for (int y = 0; y < iHeight; y++)
                {
                    if (iMax < ImageArray[0, y, x]) iMax = ImageArray[0, y, x];
                    if (iMin > ImageArray[0, y, x]) iMin = ImageArray[0, y, x];
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[0, y, x] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }

            b.UnlockBits(bmd);
            ImageArray = null;
            return b;
        }

        public Bitmap MakeBitmap(double MinContrast, double MaxContrast)
        {
            double[, ,] ImageArray = mDataDouble;
            int iWidth = ImageArray.GetLength(2);
            int iHeight = ImageArray.GetLength(1);
            double iMax = MaxContrast;
            double iMin = MinContrast;

            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 0; y < iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[0, y, x] - iMin) / iLength);
                        if (g > 255) g = 255;
                        if (g < 0) g = 0;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            ImageArray = null;
            return b;
        }
        #endregion

        #region Math_Operations

        #region Convolutions

        #region 1D
        public PhysicalArray Convolute1D(Axis DesiredAxis, double[] impulse)
        {
            int DesiredAxisIndex = (int)DesiredAxis;
            double[] PhysicalStart;
            double[] PhysicalEnd;
            double[] PhysicalStep;
            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[, ,] nData = null;

                if (DesiredAxis == Axis.XAxis)
                {
                    nData = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2) + impulse.Length];
                    unsafe
                    {
                        fixed (double* pnData = nData)
                        {
                            fixed (double* pImpulse = impulse)
                            {
                                for (int m = 0; m < mDataDouble.GetLength(0); m++)
                                {
                                    for (int j = 0; j < mDataDouble.GetLength(1); j++)
                                    {
                                        int Offset = m * mDataDouble.GetLength(2) * mDataDouble.GetLength(1) + j * mDataDouble.GetLength(2);
                                        int nOffset = m * nData.GetLength(2) * nData.GetLength(1) + j * nData.GetLength(2);
                                        Filtering. Convolute((double*)mipData + Offset, mDataDouble.GetLength(2), pImpulse, impulse.Length, (pnData + nOffset));
                                    }
                                }
                            }
                        }
                    }
                }
                else if (DesiredAxis == Axis.YAxis)
                {
                    throw new Exception("Not Yet Implimented");
                }
                else if (DesiredAxis == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implimented");
                }

                GetNewPhysicalDimensions(nData.GetLength(DesiredAxisIndex), this, DesiredAxisIndex, out PhysicalStart, out PhysicalEnd, out PhysicalStep);

                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;
                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);
                nData = null;
                return nArray;
                #endregion
            }
            else
            {
                #region Complex
                throw new Exception("Not Yet implimented");
                #endregion
            }

        }

        private unsafe static void ConvolutionChop1D(Axis DesiredAxis, Array currentArray, complex* pArrayIn, int StrideZ, int StrideY, int StrideX, double* pImpulse, int impulseLength, complex* outArray)
        {
            if (DesiredAxis == Axis.XAxis)
            {
                for (int z = 0; z < currentArray.GetLength(0); z++)
                {
                    for (int y = 0; y < currentArray.GetLength(1); y++)
                    {
                        int Offset = z * StrideZ + y * StrideY;
                        MathComplexHelps.ConvoluteChop(pArrayIn + Offset, currentArray.GetLength(2), pImpulse, impulseLength, (outArray + Offset));
                    }
                }
            }
            else if (DesiredAxis == Axis.YAxis)
            {
                for (int z = 0; z < currentArray.GetLength(0); z++)
                {
                    for (int x = 0; x < currentArray.GetLength(2); x++)
                    {
                        int Offset = z * StrideZ + x * StrideX;
                        MathComplexHelps.ConvoluteChop(pArrayIn + Offset, currentArray.GetLength(2), pImpulse, impulseLength, (outArray + Offset), StrideY);
                    }
                }
            }
            else if (DesiredAxis == Axis.ZAxis)
            {
                for (int y = 0; y < currentArray.GetLength(1); y++)
                {
                    for (int x = 0; x < currentArray.GetLength(2); x++)
                    {
                        int Offset = y * StrideY + x * StrideX;
                        MathComplexHelps.ConvoluteChop(pArrayIn + Offset, currentArray.GetLength(2), pImpulse, impulseLength, (outArray + Offset), StrideZ);
                    }
                }
            }
        }
        private unsafe void ConvolutionChop1D(Axis DesiredAxis, Array currentArray, double* pArrayIn, int StrideZ, int StrideY, int StrideX, double* pImpulse, int impulseLength, double* outArray)
        {
            if (DesiredAxis == Axis.XAxis)
            {
                for (int z = 0; z < currentArray.GetLength(0); z++)
                {
                    for (int y = 0; y < currentArray.GetLength(1); y++)
                    {
                        int Offset = z * StrideZ + y * StrideY;
                        ConvoluteChop(pArrayIn + Offset, currentArray.GetLength(2), pImpulse, impulseLength, (outArray + Offset), 1);
                    }
                }
            }
            else if (DesiredAxis == Axis.YAxis)
            {
                for (int z = 0; z < currentArray.GetLength(0); z++)
                {
                    for (int x = 0; x < currentArray.GetLength(2); x++)
                    {
                        int Offset = z * StrideZ + x * StrideX;
                        ConvoluteChop(pArrayIn + Offset, currentArray.GetLength(1), pImpulse, impulseLength, (outArray + Offset), StrideY);
                    }
                }
            }
            else if (DesiredAxis == Axis.ZAxis)
            {
                for (int y = 0; y < currentArray.GetLength(1); y++)
                {
                    for (int x = 0; x < currentArray.GetLength(2); x++)
                    {
                        int Offset = y * StrideY + x * StrideX;
                        ConvoluteChop(pArrayIn + Offset, currentArray.GetLength(0), pImpulse, impulseLength, (outArray + Offset), StrideZ);
                    }
                }
            }
        }


        private unsafe void ConvoluteChop(double* Array1, int Length1, double* pImpulse, int LImpulse, double* pArrayOut, int Stride)
        {

            int LengthWhole = Length1 + LImpulse;

            int StartI = (int)Math.Truncate((double)LengthWhole / 2d - (double)Length1 / 2d);
            int EndI = (int)Math.Truncate((double)LengthWhole / 2d + (double)Length1 / 2d);

            ///check if there is a rounding error
            if (EndI - StartI != Length1)
                EndI--;

            int sI, eI;

            double p1;
            double* p2;
            double* pOut;

            for (int i = 0; i < Length1; i++)
            {
                p1 = Array1[i * Stride];
                sI = StartI - i;
                eI = EndI - i;
                if (eI > LImpulse) eI = LImpulse;
                if (sI < 0) sI = 0;
                if (sI < eI)
                {
                    p2 = pImpulse + sI;
                    pOut = pArrayOut + i + sI - StartI;
                    for (int j = sI; j < eI; j++)
                    {
                        *pOut += p1 * (*p2);
                        pOut += Stride;
                        p2++;
                    }
                }
            }
        }

        public PhysicalArray ConvoluteChop1D(Axis DesiredAxis, double[] impulse)
        {
            double[] PhysicalStart;
            double[] PhysicalEnd;
            double[] PhysicalStep;
            int DesiredAxisIndex = (int)DesiredAxis;
            int StrideZ = mCurrentArray.GetLength(2) * mCurrentArray.GetLength(1);
            int StrideY = mCurrentArray.GetLength(2);
            int StrideX = 1;

            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[, ,] nData = null;
                nData = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];

                unsafe
                {
                    fixed (double* pnData = nData)
                    {
                        fixed (double* pImpulse = impulse)
                        {
                            ConvolutionChop1D(DesiredAxis, mCurrentArray, (double*)mipData, StrideZ, StrideY, StrideX, pImpulse, impulse.Length, pnData);
                        }
                    }
                }

                GetNewPhysicalDimensions(nData.GetLength(DesiredAxisIndex), this, DesiredAxisIndex, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;
                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);
                nData = null;
                return nArray;
                #endregion
            }
            else
            {
                #region Complex
                complex[, ,] nData = null;
                nData = new complex[mDataComplex.GetLength(0), mDataComplex.GetLength(1), mDataComplex.GetLength(2)];

                unsafe
                {
                    fixed (complex* pnData = nData)
                    {
                        fixed (double* pImpulse = impulse)
                        {
                            ConvolutionChop1D(DesiredAxis, mCurrentArray, (complex*)mipData, StrideZ, StrideY, StrideX, pImpulse, impulse.Length, pnData);
                        }
                    }
                }

                GetNewPhysicalDimensions(nData.GetLength(DesiredAxisIndex), this, DesiredAxisIndex, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = nData;
                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);
                return nArray;
                #endregion
            }


        }

        public static void ConvoluteChop1D(Axis DesiredAxis, PhysicalArray InputArray, PhysicalArray OutPutArray, double[] impulse, int StartIndex, int EndIndex)
        {
            double[] PhysicalStart;
            double[] PhysicalEnd;
            double[] PhysicalStep;
            int DesiredAxisIndex = (int)DesiredAxis;
            if (InputArray.mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[, ,] nData = OutPutArray.ReferenceDataDouble;
                int StrideZ = InputArray.mDataDouble.GetLength(2) * InputArray.mDataDouble.GetLength(1);
                int StrideY = InputArray.mDataDouble.GetLength(2);
                int StrideX = 1;
                //nData = new double[InputArray.mDataDouble.GetLength(0), InputArray.mDataDouble.GetLength(1), InputArray.mDataDouble.GetLength(2)];

                unsafe
                {
                    fixed (double* pnData = nData)
                    {
                        fixed (double* pImpulse = impulse)
                        {

                            if (DesiredAxis == Axis.XAxis)
                            {
                                for (int z = 0; z < InputArray.mDataDouble.GetLength(0); z++)
                                {
                                    for (int y = StartIndex; y < EndIndex; y++)
                                    {
                                        int Offset = z * StrideZ + y * StrideY;
                                        Filtering.ConvoluteChop((double*)InputArray.mipData + Offset, InputArray.mDataDouble.GetLength(2), pImpulse, impulse.Length, (pnData + Offset));
                                    }
                                }
                            }
                            else if (DesiredAxis == Axis.YAxis)
                            {
                                for (int z = 0; z < InputArray.mDataDouble.GetLength(0); z++)
                                {
                                    for (int x = StartIndex; x < EndIndex; x++)
                                    {
                                        int Offset = z * StrideZ + x * StrideX;
                                        Filtering.ConvoluteChop((double*)InputArray.mipData + Offset, InputArray.mDataDouble.GetLength(2), pImpulse, impulse.Length, (pnData + Offset), StrideY);
                                    }
                                }
                            }
                            else if (DesiredAxis == Axis.ZAxis)
                            {
                                for (int y = 0; y < InputArray.mDataDouble.GetLength(1); y++)
                                {
                                    for (int x = StartIndex; x < EndIndex; x++)
                                    {
                                        int Offset = y * StrideY + x * StrideX;
                                        Filtering.ConvoluteChop((double*)InputArray.mipData + Offset, InputArray.mDataDouble.GetLength(2), pImpulse, impulse.Length, (pnData + Offset), StrideZ);
                                    }
                                }
                            }
                        }
                    }
                }

                GetNewPhysicalDimensions(nData.GetLength(DesiredAxisIndex), InputArray, DesiredAxisIndex, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                nData = null;
                #endregion
            }
            else
            {
                #region Complex
                complex[, ,] nData = OutPutArray.ReferenceDataComplex;
                int StrideZ = InputArray.mDataComplex.GetLength(2) * InputArray.mDataComplex.GetLength(1);
                int StrideY = InputArray.mDataComplex.GetLength(2);
                int StrideX = 1;

                unsafe
                {
                    fixed (complex* pnData = nData)
                    {
                        fixed (double* pImpulse = impulse)
                        {

                            if (DesiredAxis == Axis.XAxis)
                            {
                                for (int z = 0; z < InputArray.mDataComplex.GetLength(0); z++)
                                {
                                    for (int y = 0; y < InputArray.mDataComplex.GetLength(1); y++)
                                    {
                                        int Offset = z * StrideZ + y * StrideY;
                                        MathComplexHelps.ConvoluteChop((complex*)InputArray.mipData + Offset, InputArray.mDataComplex.GetLength(2), pImpulse, impulse.Length, (pnData + Offset));
                                    }
                                }
                            }
                            else if (DesiredAxis == Axis.YAxis)
                            {
                                for (int z = 0; z < InputArray.mDataComplex.GetLength(0); z++)
                                {
                                    for (int x = 0; x < InputArray.mDataComplex.GetLength(2); x++)
                                    {
                                        int Offset = z * StrideZ + x * StrideX;
                                        MathComplexHelps.ConvoluteChop((complex*)InputArray.mipData + Offset, InputArray.mDataComplex.GetLength(2), pImpulse, impulse.Length, (pnData + Offset), StrideY);
                                    }
                                }
                            }
                            else if (DesiredAxis == Axis.ZAxis)
                            {
                                for (int y = 0; y < InputArray.mDataComplex.GetLength(1); y++)
                                {
                                    for (int x = 0; x < InputArray.mDataComplex.GetLength(2); x++)
                                    {
                                        int Offset = y * StrideY + x * StrideX;
                                        MathComplexHelps.ConvoluteChop((complex*)InputArray.mipData + Offset, InputArray.mDataComplex.GetLength(2), pImpulse, impulse.Length, (pnData + Offset), StrideZ);
                                    }
                                }
                            }
                        }
                    }
                }

                GetNewPhysicalDimensions(nData.GetLength(DesiredAxisIndex), InputArray, DesiredAxisIndex, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                #endregion
            }

        }
        #endregion

        #region 2D
        public PhysicalArray ConvoluteReal2D(Axis Axis1, Axis Axis2, double[,] impulse)
        {
            Axis[] OrderedAxis = OrderAxis(Axis1, Axis2);
            Axis1 = OrderedAxis[0];
            Axis2 = OrderedAxis[1];

            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[, ,] nData = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                double[,] Slice;

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    Slice = SliceAndPadZeroArray(Axis1, Axis2, mDataDouble, 0, impulse.GetLength(0), impulse.GetLength(1));

                    for (int Z = 0; Z < mDataDouble.GetLength(0); Z++)
                    {
                        if (Z > 0)
                            Slice = SliceAndPadZeroArray(Axis1, Axis2, mDataDouble, Z, impulse.GetLength(0), impulse.GetLength(1));
                        Slice = Filtering.ConvoluteChop(Slice, impulse);
                        //return Slice;
                        RewritePaddedData(Axis1, Axis2, ref nData, Z, Slice);
                    }
                }
                else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }

                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;
                nArray.PhysicalEndSet(mPhysicalEnd);
                nArray.PhysicalStartSet(mPhysicalStart);
                return nArray;

                #endregion
            }
            else
            {
                throw new Exception("Not yet Implimented");
            }

        }
        public PhysicalArray ConvoluteFFT2D(Axis Axis1, Axis Axis2, double[,] impulse, bool ImpulseInRealSpace)
        {
            Axis[] OrderedAxis = OrderAxis(Axis1, Axis2);
            Axis1 = OrderedAxis[0];
            Axis2 = OrderedAxis[1];

            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[, ,] nData = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                double[,] Slice;

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    Slice = SliceAndPadZeroArray(Axis1, Axis2, mDataDouble, 0, impulse.GetLength(0), impulse.GetLength(1));

                    for (int Z = 0; Z < mDataDouble.GetLength(0); Z++)
                    {
                        if (Z > 0)
                            Slice = SliceAndPadZeroArray(Axis1, Axis2, mDataDouble, Z, impulse.GetLength(0), impulse.GetLength(1));
                        Slice = Filtering.ConvoluteFFT(Slice, impulse, ImpulseInRealSpace);
                        //return Slice;
                        RewritePaddedData(Axis1, Axis2, ref nData, Z, Slice);
                    }
                }
                else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }

                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;
                nArray.PhysicalEndSet(mPhysicalEnd);
                nArray.PhysicalStartSet(mPhysicalStart);
                return nArray;

                #endregion
            }
            else
            {
                throw new Exception("Not yet Implimented");
            }

        }

        public PhysicalArray ConvoluteChopSeperable(Axis DesiredAxis1, Axis DesiredAxis2, double[] impulse)
        {
            int StrideZ = mCurrentArray.GetLength(2) * mCurrentArray.GetLength(1);
            int StrideY = mCurrentArray.GetLength(2);
            int StrideX = 1;

            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[, ,] nData1 = null;
                if (DesiredAxis1 != DesiredAxis2)
                    nData1 = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                double[, ,] nData = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];

                unsafe
                {
                    fixed (double* pImpulse = impulse)
                    {
                        if (DesiredAxis1 != DesiredAxis2)
                        {
                            fixed (double* pnData1 = nData1)
                            {
                                ConvolutionChop1D(DesiredAxis1, mCurrentArray, (double*)mipData, StrideZ, StrideY, StrideX, pImpulse, impulse.Length, pnData1);

                                fixed (double* pnData = nData)
                                {
                                    ConvolutionChop1D(DesiredAxis2, mCurrentArray, pnData1, StrideZ, StrideY, StrideX, pImpulse, impulse.Length, pnData);
                                }
                            }
                        }
                        else
                        {
                            fixed (double* pnData = nData)
                            {
                                ConvolutionChop1D(DesiredAxis1, mCurrentArray, (double*)mipData, StrideZ, StrideY, StrideX, pImpulse, impulse.Length, pnData);
                            }
                        }
                    }
                }
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;
                nArray.PhysicalEndSet(mPhysicalEnd);
                nArray.PhysicalStartSet(mPhysicalStart);
                return nArray;
                #endregion
            }
            else
            {
                #region Complex
                complex[, ,] nData1 = new complex[mDataComplex.GetLength(0), mDataComplex.GetLength(1), mDataComplex.GetLength(2)];
                complex[, ,] nData = new complex[mDataComplex.GetLength(0), mDataComplex.GetLength(1), mDataComplex.GetLength(2)];

                unsafe
                {
                    fixed (double* pImpulse = impulse)
                    {
                        fixed (complex* pnData1 = nData)
                        {
                            ConvolutionChop1D(DesiredAxis1, mCurrentArray, (complex*)mipData, StrideZ, StrideY, StrideX, pImpulse, impulse.Length, pnData1);

                            fixed (complex* pnData = nData)
                            {
                                ConvolutionChop1D(DesiredAxis2, mCurrentArray, pnData1, StrideZ, StrideY, StrideX, pImpulse, impulse.Length, pnData);
                            }
                        }
                    }
                }

                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = nData;
                nArray.PhysicalEndSet(mPhysicalEnd);
                nArray.PhysicalStartSet(mPhysicalStart);
                return nArray;
                #endregion
            }


        }

        /*
        public PhysicalArray ConvolutionGPU(Axis Axis1, Axis Axis2, float[,] impulse)
        {

            GPUConvolution.GPUConvoluter Convoluter = new GPUConvolution.GPUConvoluter();
            Convoluter.Setup2DConvolution(impulse.GetLength(0));

            Axis[] OrderedAxis = OrderAxis(Axis1, Axis2);
            Axis1 = OrderedAxis[0];
            Axis2 = OrderedAxis[1];

            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[, ,] nData = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                float[,] Slice;

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    Slice = SliceAndPadZeroArrayToFloat(Axis1, Axis2, mDataDouble, 0, impulse.GetLength(0) + nData.GetLength(2), impulse.GetLength(1) + nData.GetLength(1));

                    for (int Z = 0; Z < mDataDouble.GetLength(0); Z++)
                    {
                        if (Z > 0)
                            Slice = SliceAndPadZeroArrayToFloat(Axis1, Axis2, mDataDouble, Z, impulse.GetLength(0) + nData.GetLength(2), impulse.GetLength(1) + nData.GetLength(1));
                        Slice = Convoluter.Convolution2D(Slice, impulse);
                        //return Slice;
                        RewritePaddedData(Axis1, Axis2, ref nData, Z, Slice);
                    }
                }
                else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }

                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;
                nArray.PhysicalEndSet(mPhysicalEnd);
                nArray.PhysicalStartSet(mPhysicalStart);
                Convoluter.Dispose();
                return nArray;

                #endregion
            }
            else
            {
                throw new Exception("Not yet Implimented");
            }

        }

        public PhysicalArray ConvolutionGPU(Axis Axis1, Axis Axis2, float[] impulse)
        {
            GPUConvolution.GPUConvoluter1D Convoluter = new GPUConvolution.GPUConvoluter1D();
            Convoluter.SetupConvolution();

            Axis[] OrderedAxis = OrderAxis(Axis1, Axis2);
            Axis1 = OrderedAxis[0];
            Axis2 = OrderedAxis[1];

            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[, ,] nData = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                float[,] Slice;

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    Slice = SliceAndPadZeroArrayToFloat(Axis1, Axis2, mDataDouble, 0, nData.GetLength(2),nData.GetLength(1) );

                    for (int Z = 0; Z < mDataDouble.GetLength(0); Z++)
                    {
                        if (Z > 0)
                            Slice = SliceAndPadZeroArrayToFloat(Axis1, Axis2, mDataDouble, Z, nData.GetLength(2), nData.GetLength(1) );
                        Slice = Convoluter.ConvolutionGPU(Slice, impulse);
                        //return Slice;
                        RewritePaddedData(Axis1, Axis2, ref nData, Z, Slice);
                    }
                }
                else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }

                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;
                nArray.PhysicalEndSet(mPhysicalEnd);
                nArray.PhysicalStartSet(mPhysicalStart);
                Convoluter.Dispose();
                return nArray;

                #endregion
            }
            else
            {
                throw new Exception("Not yet Implimented");
            }

        }
        */

        #endregion

        #endregion

        #region ArraySlicing
        /* /// <summary>
        /// code Used to get walking though the array at different axis running.  Not very useful otherwise
        /// </summary>
        /// <param name="DesiredAxis"></param>
        /// <param name="impulseFreqSpace"></param>
        /// <returns></returns>
        public double[] TestStride(Axis DesiredAxis, int x, int y, int z)
        {

            int DesiredAxisIndex = (int)DesiredAxis;
            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[] nData = null;
                int StrideZ = mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                int StrideY = mDataDouble.GetLength(2);
                int StrideX = 1;
                nData = new double[this.GetLength(DesiredAxis)];

                unsafe
                {
                    fixed (double* pnData = nData)
                    {


                        if (DesiredAxis == Axis.XAxis)
                        {
                            int Offset = z * StrideZ + y * StrideY;
                            MathHelps.TestCodeGetLine((double*)mipData + Offset, mDataDouble.GetLength(2), (pnData), StrideX);
                        }
                        else if (DesiredAxis == Axis.YAxis)
                        {
                            int Offset = z * StrideZ + x * StrideX;
                            MathHelps.TestCodeGetLine((double*)mipData + Offset, mDataDouble.GetLength(2), (pnData), StrideY);
                        }
                        else if (DesiredAxis == Axis.ZAxis)
                        {
                            int Offset = y * StrideY + x * StrideX;
                            MathHelps.TestCodeGetLine((double*)mipData + Offset, mDataDouble.GetLength(2), (pnData), StrideZ);
                        }
                    }

                }

                return nData;
                #endregion
            }
            else
            {
                throw new Exception("not yet implimented");
            }

        }*/
        #endregion

        #region FFTs

        #region 1D
        public PhysicalArray FFT_1D(Axis DesiredAxis)
        {
            double[] PhysicalStart;
            double[] PhysicalEnd;
            double[] PhysicalStep;
            int DesiredAxisIndex = (int)DesiredAxis;
            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                complex[, ,] nData = null;

                if (DesiredAxis == Axis.XAxis)
                {
                    nData = new complex[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                    unsafe
                    {
                        fixed (complex* pnData = nData)
                        {
                            for (int m = 0; m < mDataDouble.GetLength(0); m++)
                            {
                                for (int j = 0; j < mDataDouble.GetLength(1); j++)
                                {
                                    int Offset = m * mDataDouble.GetLength(2) * mDataDouble.GetLength(1) + j * mDataDouble.GetLength(2);
                                    int nOffset = m * nData.GetLength(2) * nData.GetLength(1) + j * nData.GetLength(2);
                                    MathFFTHelps.FFTreal2complex((double*)mipData + Offset, mDataDouble.GetLength(2), (pnData + nOffset));
                                }
                            }

                        }
                    }
                }
                GetNewPhysicalDimensions(nData.GetLength(DesiredAxisIndex), this, DesiredAxisIndex, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                int i = (int)DesiredAxis;
                {
                    double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                    nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalStep[i] = 1 / Length;
                }

                nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, true);
                nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, true);
                return nArray;
                #endregion
            }
            else
            {
                #region Complex
                complex[, ,] nData = null;
                if (DesiredAxis == Axis.XAxis)
                {
                    nData = new complex[mDataComplex.GetLength(0), mDataComplex.GetLength(1), mDataComplex.GetLength(2)];
                    unsafe
                    {
                        fixed (complex* pnData = nData)
                        {

                            for (int m = 0; m < mDataComplex.GetLength(0); m++)
                            {
                                for (int j = 0; j < mDataComplex.GetLength(1); j++)
                                {
                                    int Offset = m * mDataDouble.GetLength(2) * mDataDouble.GetLength(1) + j * mDataDouble.GetLength(2);
                                    int nOffset = m * nData.GetLength(2) * nData.GetLength(1) + j * nData.GetLength(2);
                                    MathFFTHelps.FFTcomplex2complex((complex*)mipData + Offset, mDataComplex.GetLength(2), (pnData + nOffset));
                                }
                            }

                        }
                    }
                }
                GetNewPhysicalDimensions(nData.GetLength(DesiredAxisIndex), this, DesiredAxisIndex, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                int i = (int)DesiredAxis;
                {
                    double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                    nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalStep[i] = 1 / Length;
                }

                nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, true);
                nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, true);
                return nArray;
                #endregion
            }
        }
        public PhysicalArray iFFT_1D(Axis DesiredAxis)
        {
            double[] PhysicalStart;
            double[] PhysicalEnd;
            double[] PhysicalStep;
            int DesiredAxisIndex = (int)DesiredAxis;
            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                complex[, ,] nData = null;

                if (DesiredAxis == Axis.XAxis)
                {
                    nData = new complex[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                    unsafe
                    {
                        fixed (complex* pnData = nData)
                        {
                            for (int m = 0; m < mDataDouble.GetLength(0); m++)
                            {
                                for (int j = 0; j < mDataDouble.GetLength(1); j++)
                                {
                                    int Offset = m * mDataDouble.GetLength(2) * mDataDouble.GetLength(1) + j * mDataDouble.GetLength(2);
                                    int nOffset = m * nData.GetLength(2) * nData.GetLength(1) + j * nData.GetLength(2);
                                    MathFFTHelps.iFFTcomplex2complex((complex*)mipData + Offset, mDataDouble.GetLength(2), (pnData + nOffset));
                                }
                            }

                        }
                    }
                }
                GetNewPhysicalDimensions(nData.GetLength(DesiredAxisIndex), this, DesiredAxisIndex, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                int i = (int)DesiredAxis;
                {
                    double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                    nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalStep[i] = 1 / Length;
                }

                nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, false);
                nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, false);
                return nArray;
                #endregion
            }
            else
            {
                #region Complex
                complex[, ,] nData = null;
                if (DesiredAxis == Axis.XAxis)
                {
                    nData = new complex[mDataComplex.GetLength(0), mDataComplex.GetLength(1), mDataComplex.GetLength(2)];
                    unsafe
                    {
                        fixed (complex* pnData = nData)
                        {

                            for (int m = 0; m < mDataComplex.GetLength(0); m++)
                            {
                                for (int j = 0; j < mDataComplex.GetLength(1); j++)
                                {
                                    int Offset = m * mDataDouble.GetLength(2) * mDataDouble.GetLength(1) + j * mDataDouble.GetLength(2);
                                    int nOffset = m * nData.GetLength(2) * nData.GetLength(1) + j * nData.GetLength(2);
                                    MathFFTHelps.iFFTcomplex2complex((complex*)mipData + Offset, mDataComplex.GetLength(2), (pnData + nOffset));
                                }
                            }

                        }
                    }
                }
                GetNewPhysicalDimensions(nData.GetLength(DesiredAxisIndex), this, DesiredAxisIndex, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                int i = (int)DesiredAxis;
                {
                    double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                    nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalStep[i] = 1 / Length;
                }

                nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, false);
                nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, false);
                return nArray;
                #endregion
            }
        }

        public PhysicalArray FFT_RealOnly_1D(Axis DesiredAxis)
        {
            double[] PhysicalStart;
            double[] PhysicalEnd;
            double[] PhysicalStep;
            int DesiredAxisIndex = (int)DesiredAxis;
            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[, ,] nData = null;

                if (DesiredAxis == Axis.XAxis)
                {
                    nData = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                    unsafe
                    {
                        fixed (double* pnData = nData)
                        {
                            for (int m = 0; m < mDataDouble.GetLength(0); m++)
                            {
                                for (int j = 0; j < mDataDouble.GetLength(1); j++)
                                {
                                    int Offset = m * mDataDouble.GetLength(2) * mDataDouble.GetLength(1) + j * mDataDouble.GetLength(2);
                                    int nOffset = m * nData.GetLength(2) * nData.GetLength(1) + j * nData.GetLength(2);
                                    MathFFTHelps.FFTreal2real((double*)mipData + Offset, mDataDouble.GetLength(2), (pnData + nOffset));
                                }
                            }

                        }
                    }
                }
                GetNewPhysicalDimensions(nData.GetLength(DesiredAxisIndex), this, DesiredAxisIndex, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                int i = (int)DesiredAxis;
                {
                    double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                    nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalStep[i] = 1 / Length;
                }

                nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, true);
                nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, true);
                return nArray;
                #endregion
            }
            else
            {
                #region Complex
                throw new Exception("Cannot return a real fft from a complex array");
                #endregion
            }
        }
        public PhysicalArray iFFTreal_1D(Axis DesiredAxis)
        {
            double[] PhysicalStart;
            double[] PhysicalEnd;
            double[] PhysicalStep;
            int DesiredAxisIndex = (int)DesiredAxis;
            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[, ,] nData = null;

                if (DesiredAxis == Axis.XAxis)
                {
                    nData = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                    unsafe
                    {
                        fixed (double* pnData = nData)
                        {
                            for (int m = 0; m < mDataDouble.GetLength(0); m++)
                            {
                                for (int j = 0; j < mDataDouble.GetLength(1); j++)
                                {
                                    int Offset = m * mDataDouble.GetLength(2) * mDataDouble.GetLength(1) + j * mDataDouble.GetLength(2);
                                    int nOffset = m * nData.GetLength(2) * nData.GetLength(1) + j * nData.GetLength(2);
                                    MathFFTHelps.iFFTreal2real((double*)mipData + Offset, mDataDouble.GetLength(2), (pnData + nOffset));
                                }
                            }

                        }
                    }
                }
                GetNewPhysicalDimensions(nData.GetLength(DesiredAxisIndex), this, DesiredAxisIndex, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                int i = (int)DesiredAxis;
                {
                    double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                    nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalStep[i] = 1 / Length;
                }

                nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, false);
                nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, false);
                return nArray;
                #endregion
            }
            else
            {
                #region Complex
                double[, ,] nData = null;
                if (DesiredAxis == Axis.XAxis)
                {
                    nData = new double[mDataComplex.GetLength(0), mDataComplex.GetLength(1), mDataComplex.GetLength(2)];
                    unsafe
                    {
                        fixed (double* pnData = nData)
                        {

                            for (int m = 0; m < mDataComplex.GetLength(0); m++)
                            {
                                for (int j = 0; j < mDataComplex.GetLength(1); j++)
                                {
                                    int Offset = m * mDataDouble.GetLength(2) * mDataDouble.GetLength(1) + j * mDataDouble.GetLength(2);
                                    int nOffset = m * nData.GetLength(2) * nData.GetLength(1) + j * nData.GetLength(2);
                                    MathFFTHelps.iFFTcomplex2real((complex*)mipData + Offset, mDataComplex.GetLength(2), (pnData + nOffset));
                                }
                            }

                        }
                    }
                }
                GetNewPhysicalDimensions(nData.GetLength(DesiredAxisIndex), this, DesiredAxisIndex, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                int i = (int)DesiredAxis;
                {
                    double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                    nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                    nArray.mPhysicalStep[i] = 1 / Length;
                }

                nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, false);
                nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, false);
                return nArray;

                #endregion
            }
        }
        #endregion

        #region 2D
        private static Axis[] OrderAxis(Axis Axis1, Axis Axis2)
        {
            if (Axis1 == Axis2)
                throw new Exception("Axis1 must not match axis2");

            if (Axis1 == Axis.XAxis)
                return new Axis[] { Axis1, Axis2 };
            else if (Axis2 == Axis.XAxis)
                return new Axis[] { Axis2, Axis1 };
            else if (Axis1 == Axis.YAxis)
                return new Axis[] { Axis1, Axis2 };
            else if (Axis2 == Axis.YAxis)
                return new Axis[] { Axis2, Axis1 };
            throw new Exception("Axis cannot be ordered");
        }

        public PhysicalArray FFT_2D(Axis Axis1, Axis Axis2)
        {
            Axis[] OrderedAxis = OrderAxis(Axis1, Axis2);
            Axis1 = OrderedAxis[0];
            Axis2 = OrderedAxis[1];

            double[] PhysicalStart;
            double[] PhysicalEnd;
            double[] PhysicalStep;

            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                complex[, ,] nData = null;

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    nData = new complex[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                    unsafe
                    {
                        fixed (complex* pnData = nData)
                        {
                            for (int Z = 0; Z < mDataDouble.GetLength(0); Z++)
                            {

                                int Offset = Z * mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                                int nOffset = Z * nData.GetLength(2) * nData.GetLength(1);
                                MathFFTHelps.FFTreal2complex((double*)mipData + Offset, mDataDouble.GetLength(1), mDataDouble.GetLength(2), (pnData + nOffset));
                            }
                        }
                    }
                }
                else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                GetNewPhysicalDimensions(nData.GetLength(1), this, 1, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                GetNewPhysicalDimensions(nData.GetLength(0), this, 0, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                        nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalStep[i] = 1 / Length;
                        nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, true);
                        nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, true);
                    }
                }
                return nArray;
                #endregion
            }
            else
            {
                #region Complex
                complex[, ,] nData = null;

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    nData = new complex[mDataComplex.GetLength(0), mDataComplex.GetLength(1), mDataComplex.GetLength(2)];
                    unsafe
                    {
                        fixed (complex* pnData = nData)
                        {
                            for (int Z = 0; Z < mDataDouble.GetLength(0); Z++)
                            {

                                int Offset = Z * mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                                int nOffset = Z * nData.GetLength(2) * nData.GetLength(1);
                                MathFFTHelps.FFTcomplex2complex((complex*)mipData + Offset, mDataDouble.GetLength(1), mDataDouble.GetLength(2), (pnData + nOffset));
                            }
                        }
                    }
                }
                else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                GetNewPhysicalDimensions(nData.GetLength(1), this, 1, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                GetNewPhysicalDimensions(nData.GetLength(0), this, 0, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                        nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalStep[i] = 1 / Length;
                        nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, true);
                        nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, true);
                    }
                }
                return nArray;
                #endregion
            }
        }

        public PhysicalArray iFFT_2D(Axis Axis1, Axis Axis2)
        {
            Axis[] OrderedAxis = OrderAxis(Axis1, Axis2);
            Axis1 = OrderedAxis[0];
            Axis2 = OrderedAxis[1];

            double[] PhysicalStart;
            double[] PhysicalEnd;
            double[] PhysicalStep;

            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[, ,] nData = null;

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    nData = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                    unsafe
                    {
                        fixed (double* pnData = nData)
                        {
                            for (int Z = 0; Z < mDataDouble.GetLength(0); Z++)
                            {

                                int Offset = Z * mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                                int nOffset = Z * nData.GetLength(2) * nData.GetLength(1);
                                MathFFTHelps.iFFTreal2real((double*)mipData + Offset, mDataDouble.GetLength(1), mDataDouble.GetLength(2), (pnData + nOffset));
                            }
                        }
                    }
                }
                else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                GetNewPhysicalDimensions(nData.GetLength(1), this, 1, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                GetNewPhysicalDimensions(nData.GetLength(0), this, 0, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                        nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalStep[i] = 1 / Length;
                        nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, true);
                        nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, true);
                    }
                }
                return nArray;
                #endregion
            }
            else
            {
                #region Complex
                complex[, ,] nData = null;

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    nData = new complex[mDataComplex.GetLength(0), mDataComplex.GetLength(1), mDataComplex.GetLength(2)];
                    unsafe
                    {
                        fixed (complex* pnData = nData)
                        {
                            for (int Z = 0; Z < mDataDouble.GetLength(0); Z++)
                            {

                                int Offset = Z * mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                                int nOffset = Z * nData.GetLength(2) * nData.GetLength(1);
                                MathFFTHelps.iFFTcomplex2complex((complex*)mipData + Offset, mDataDouble.GetLength(1), mDataDouble.GetLength(2), (pnData + nOffset));
                            }
                        }
                    }
                }
                else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                GetNewPhysicalDimensions(nData.GetLength(1), this, 1, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                GetNewPhysicalDimensions(nData.GetLength(0), this, 0, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.ComplexArray, false);
                nArray.ReferenceDataComplex = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                        nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalStep[i] = 1 / Length;
                        nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, true);
                        nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, true);
                    }
                }
                return nArray;
                #endregion
            }
        }

        public PhysicalArray FFT_RealOnly_2D(Axis Axis1, Axis Axis2)
        {
            Axis[] OrderedAxis = OrderAxis(Axis1, Axis2);
            Axis1 = OrderedAxis[0];
            Axis2 = OrderedAxis[1];

            double[] PhysicalStart;
            double[] PhysicalEnd;
            double[] PhysicalStep;

            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[, ,] nData = null;

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    nData = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                    unsafe
                    {
                        fixed (double* pnData = nData)
                        {
                            for (int Z = 0; Z < mDataDouble.GetLength(0); Z++)
                            {

                                int Offset = Z * mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                                int nOffset = Z * nData.GetLength(2) * nData.GetLength(1);
                                MathFFTHelps.FFTreal2real((double*)mipData + Offset, mDataDouble.GetLength(1), mDataDouble.GetLength(2), (pnData + nOffset));
                            }
                        }
                    }
                }
                else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                GetNewPhysicalDimensions(nData.GetLength(1), this, 1, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                GetNewPhysicalDimensions(nData.GetLength(0), this, 0, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                        nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalStep[i] = 1 / Length;
                        nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, true);
                        nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, true);
                    }
                }
                return nArray;
                #endregion
            }
            else
            {
                throw new Exception("Cannot return a real fft from a complex array");
            }
        }
        public PhysicalArray iFFTreal_2D(Axis Axis1, Axis Axis2)
        {
            Axis[] OrderedAxis = OrderAxis(Axis1, Axis2);
            Axis1 = OrderedAxis[0];
            Axis2 = OrderedAxis[1];

            double[] PhysicalStart;
            double[] PhysicalEnd;
            double[] PhysicalStep;

            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                #region Double
                double[, ,] nData = null;

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    nData = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
                    unsafe
                    {
                        fixed (double* pnData = nData)
                        {
                            for (int Z = 0; Z < mDataDouble.GetLength(0); Z++)
                            {

                                int Offset = Z * mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                                int nOffset = Z * nData.GetLength(2) * nData.GetLength(1);
                                MathFFTHelps.iFFTreal2real((double*)mipData + Offset, mDataDouble.GetLength(1), mDataDouble.GetLength(2), (pnData + nOffset));
                            }
                        }
                    }
                }
                else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                GetNewPhysicalDimensions(nData.GetLength(1), this, 1, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                GetNewPhysicalDimensions(nData.GetLength(0), this, 0, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                        nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalStep[i] = 1 / Length;
                        nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, true);
                        nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, true);
                    }
                }
                return nArray;
                #endregion
            }
            else
            {
                #region Complex
                double[, ,] nData = null;

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    nData = new double[mDataComplex.GetLength(0), mDataComplex.GetLength(1), mDataComplex.GetLength(2)];
                    unsafe
                    {
                        fixed (double* pnData = nData)
                        {
                            for (int Z = 0; Z < mDataDouble.GetLength(0); Z++)
                            {

                                int Offset = Z * mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                                int nOffset = Z * nData.GetLength(2) * nData.GetLength(1);
                                MathFFTHelps.iFFTcomplex2real((complex*)mipData + Offset, mDataDouble.GetLength(1), mDataDouble.GetLength(2), (pnData + nOffset));
                            }
                        }
                    }
                }
                else if (Axis1 == Axis.XAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                else if (Axis1 == Axis.YAxis && Axis2 == Axis.ZAxis)
                {
                    throw new Exception("Not Yet Implemented");
                }
                GetNewPhysicalDimensions(nData.GetLength(1), this, 1, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                GetNewPhysicalDimensions(nData.GetLength(0), this, 0, out PhysicalStart, out PhysicalEnd, out PhysicalStep);
                PhysicalArray nArray = new PhysicalArray(this, PhysicalArrayType.DoubleArray, false);
                nArray.ReferenceDataDouble = nData;

                nArray.PhysicalEndSet(PhysicalEnd);
                nArray.PhysicalStartSet(PhysicalStart);

                if (Axis1 == Axis.XAxis && Axis2 == Axis.YAxis)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        double Length = this.mPhysicalEnd[i] - this.mPhysicalStart[i];
                        nArray.mPhysicalStart[i] = -.5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalEnd[i] = .5 / Length * this.mDataDouble.GetLength(i);
                        nArray.mPhysicalStep[i] = 1 / Length;
                        nArray.mArrayInformation.FFTSpace_Set((GraphAxis)i, true);
                        nArray.mArrayInformation.MachineReadableFFT_Set((GraphAxis)i, true);
                    }
                }
                return nArray;
                #endregion
            }
        }
        #endregion

        #endregion

        public object Sum()
        {
            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                double result = 0;
                unsafe
                {
                    double* pX = (double*)mipData;
                    int Length = mDataDouble.Length;
                    for (int i = 0; i < Length; i++)
                    {
                        result += ((*pX));
                        pX++;
                    }
                }
                return result;
            }
            else
            {
                complex result = new complex(0, 0);
                unsafe
                {
                    complex* pX = (complex*)mipData;
                    int Length = mDataComplex.Length;
                    for (int i = 0; i < Length; i++)
                    {
                        result += ((*pX));
                        pX++;
                    }
                }
                return result;
            }

        }

        public void Clear()
        {
            if (mArrayType == PhysicalArrayType.DoubleArray)
            {
                unsafe
                {
                    double* pX = (double*)mipData;
                    int Length = mDataDouble.Length;
                    for (int i = 0; i < Length; i++)
                    {
                        *pX = 0;
                        pX++;
                    }
                }

            }
            else
            {
                complex result = new complex(0, 0);
                unsafe
                {
                    complex* pX = (complex*)mipData;
                    int Length = mDataComplex.Length;
                    for (int i = 0; i < Length; i++)
                    {
                        *pX = result;
                        pX++;
                    }
                }
            }

        }


        public void SubtractCenteredInPlace(PhysicalArray Subtractee)
        {
            if (this.mArrayType == PhysicalArrayType.DoubleArray && Subtractee.mArrayType == PhysicalArrayType.DoubleArray)
            {
                // double[, ,] result = new double[this.mDataDouble.GetLength(0), this.mDataDouble.GetLength(1), this.mDataDouble.GetLength(2)];
                unsafe
                {
                    if (this.mDataDouble.GetLength(2) >= Subtractee.mDataDouble.GetLength(2))
                    {
                        //center the offsets
                        int offsetZ = (int)(((double)this.mDataDouble.GetLength(0) - Subtractee.mDataDouble.GetLength(0)) / 2d);
                        int offsetY = (int)(((double)this.mDataDouble.GetLength(1) - Subtractee.mDataDouble.GetLength(1)) / 2d);
                        int offsetX = (int)(((double)this.mDataDouble.GetLength(2) - Subtractee.mDataDouble.GetLength(2)) / 2d);

                        //get the size of each of the slices
                        int tSizeZ = this.mDataDouble.GetLength(1) * this.mDataDouble.GetLength(2);
                        int tSizeY = this.mDataDouble.GetLength(2);

                        int subSizeZ = Subtractee.mDataDouble.GetLength(1) * Subtractee.mDataDouble.GetLength(2);
                        int subSizeY = Subtractee.mDataDouble.GetLength(2);

                        //   fixed (double* pResult = result)
                        {
                            for (int z = 0; z < Subtractee.mDataDouble.GetLength(0); z++)
                            {
                                for (int y = 0; y < Subtractee.mDataDouble.GetLength(1); y++)
                                {
                                    //locate the pointer correctly for the larger image with offsets
                                    double* pX = (double*)this.mipData + (z + offsetZ) * tSizeZ + (y + offsetY) * tSizeY + offsetX;
                                    //  double* pOut = pResult +(z + offsetZ) * tSizeZ + (y + offsetY) * tSizeY + offsetX;

                                    double* pSub = (double*)Subtractee.mipData + z * subSizeZ + y * subSizeY;
                                    for (int x = 0; x < Subtractee.mDataDouble.GetLength(2); x++)
                                    {
                                        *pX = (*pX) - (*pSub);
                                        //pOut++;
                                        pX++;
                                        pSub++;
                                    }
                                }
                            }
                        }
                        //   ReferenceDataDouble = result;
                    }
                    else
                        throw new Exception("not yet implimented");
                }

            }
            else
            {
                throw new Exception("Not yet implimented");
            }
        }

        public void NormalizeSlices(Axis NormalizationAxis)
        {
            if (NormalizationAxis == Axis.ZAxis)
            {
                int LX = mDataDouble.GetLength(2);
                int LY = mDataDouble.GetLength(1);
                int LZ = mDataDouble.GetLength(0);
                double[] Normalization = new double[LZ];
                for (int z = 0; z < LZ; z++)
                    for (int y = 0; y < LY; y++)
                        for (int x = 0; x < LX; x++)
                        {
                            Normalization[z] += mDataDouble[z, y, x];
                        }

                for (int i = 0; i < Normalization.Length; i++)
                {
                    if (Normalization[i] != 0)
                        Normalization[i] /= (LX * LY);
                    else
                        Normalization[i] = 1;
                }

                for (int z = 0; z < LZ; z++)
                    for (int y = 0; y < LY; y++)
                        for (int x = 0; x < LX; x++)
                        {
                            mDataDouble[z, y, x] /= Normalization[z];
                        }
            }
            else
                throw new Exception("Not yet implemented");

        }

        static Queue<int> CylinderSlices = null;
        static object CriticalRemoveLock1 = new object();
        static object CriticalRemoveLock2 = new object();
        public bool CylinderArtifactRemoved = false;
        public void RemoveOutterCylinder()
        {

            lock (CriticalRemoveLock1)
            {
                if (CylinderSlices == null)
                {
                    CylinderSlices = new Queue<int>();
                    for (int i = 0; i < mCurrentArray.GetLength(0); i++)
                    {
                        CylinderSlices.Enqueue(i);
                    }
                }
            }

            double RX = mCurrentArray.GetLength(2) / 2d - 5;
            double RY = mCurrentArray.GetLength(1) / 2d - 5;
            double RZ = mCurrentArray.GetLength(0) / 2d - 5;

            double HalfI = mCurrentArray.GetLength(2) / 2d;
            double HalfJ = mCurrentArray.GetLength(1) / 2d;
            double HalfK = mCurrentArray.GetLength(0) / 2d;

            try
            {
                while (CylinderSlices.Count > 0)
                {
                    int CurrentIndex = 0;
                    lock (CriticalRemoveLock2)
                    {
                        CurrentIndex = CylinderSlices.Dequeue();
                    }

                    double x;
                    double y;

                    for (int j = 0; j < mCurrentArray.GetLength(1); j++)
                        for (int k = 0; k < mCurrentArray.GetLength(2); k++)
                        {
                            y = (j - HalfJ) / RY;
                            x = (k - HalfK) / RX;
                            if ((x * x + y * y) > 1)
                            {
                                mDataDouble[CurrentIndex, j, k] = 0;
                            }
                        }
                }
            }
            catch { }
            CylinderSlices = null;
            CylinderArtifactRemoved = true;
        }
        #endregion

        #region Projections
        /*
        private PhysicalArray ProjectArray2D(Point3D Direction, Point3D FastScanDirection)
        {
            double LengthCorner = 0;
            double StepSize = double.MaxValue;
            for (int i = 1; i < 3; i++)
            {
                LengthCorner += mPhysicalStart[i] * mPhysicalStart[i];
                if (mPhysicalStep[i] < StepSize)
                    StepSize = mPhysicalStep[i];
            }
            LengthCorner = Math.Sqrt(LengthCorner);


            double[] PImage = new double[(int)(2 * LengthCorner / StepSize)];

            FastScanDirection.Normalize();

            Point3D FastScanAxis = FastScanDirection * StepSize;
            Point3D Origin = -1 * Direction * LengthCorner - FastScanDirection * LengthCorner;
            Direction.Normalize();
            Direction *= StepSize;

            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LOut = PImage.GetLength(0);

            double sX = mPhysicalStart[2];
            double sY = mPhysicalStart[1];

            double stepX = mPhysicalStep[2];
            double stepY = mPhysicalStep[1];

            double x1, y1;
            double x3, y3;


            double nXp1, nYp1;
            double sum;

            for (int tnXI = 0; tnXI < LOut; tnXI++)
            {
                x1 = Origin.X + FastScanAxis.X * tnXI;// -mPhysicalStart[2];
                y1 = Origin.Y + FastScanAxis.Y * tnXI;// -mPhysicalStart[1];

                sum = 0;
                for (int tI = 0; tI < LOut; tI++)
                {
                    x3 = (x1 + Direction.X * tI);
                    y3 = (y1 + Direction.Y * tI);

                    nXp1 = (x3 - mPhysicalStart[2]) / stepX;
                    nYp1 = (y3 - mPhysicalStart[1]) / stepY;
                    if ((nXp1 >= 0 && nXp1 < LX))
                        if (nYp1 >= 0 && nYp1 < LY)
                        {
                            sum += mDataDouble[0, (int)nYp1, (int)nXp1];
                        }
                }
                PImage[tnXI] = sum;

            }

            return new PhysicalArray(PImage, -1 * LengthCorner, LengthCorner);
        }

        private PhysicalArray ProjectArray3D(Point3D Direction, Point3D FastScanDirection)
        {
            double LengthCorner = 0;
            double StepSize = double.MaxValue;
            for (int i = 1; i < 3; i++)
            {
                LengthCorner += mPhysicalStart[i] * mPhysicalStart[i];
                if (mPhysicalStep[i] < StepSize)
                    StepSize = mPhysicalStep[i];
            }
            LengthCorner = Math.Sqrt(LengthCorner);


            double[,] PImage = new double[(int)(2 * LengthCorner / StepSize), (int)(2 * LengthCorner / StepSize)];

            FastScanDirection.Normalize();

            Point3D FastScanAxis = FastScanDirection * StepSize;
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanAxis);
            Point3D Origin = -1 * Direction * LengthCorner - FastScanDirection * LengthCorner - SlowScanAxis / StepSize * LengthCorner;
            Direction.Normalize();
            Direction *= StepSize;

            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LZ = mDataDouble.GetLength(0);
            int LOut = PImage.GetLength(0);

            double sX = mPhysicalStart[2];
            double sY = mPhysicalStart[1];
            double sZ = mPhysicalStart[0];

            double stepX = mPhysicalStep[2];
            double stepY = mPhysicalStep[1];
            double stepZ = mPhysicalStep[0];

            double x1, y1, z1;
            double x2, y2, z2;
            double x3, y3, z3;

            double nXp1, nYp1, nZp1;
            double sum;

            for (int tnXI = 0; tnXI < LOut; tnXI++)
            {
                x1 = Origin.X + FastScanAxis.X * tnXI - mPhysicalStart[2];
                y1 = Origin.Y + FastScanAxis.Y * tnXI - mPhysicalStart[1];
                z1 = Origin.Z + FastScanAxis.Z * tnXI - mPhysicalStart[0];

                for (int tnYI = 0; tnYI < LOut; tnYI++)
                {
                    x2 = x1 + SlowScanAxis.X * tnYI;
                    y2 = y1 + SlowScanAxis.Y * tnYI;
                    z2 = z1 + SlowScanAxis.Z * tnYI;
                    sum = 0;
                    for (int tI = 0; tI < LOut; tI++)
                    {
                        x3 = (x2 + Direction.X * tI);
                        y3 = (y2 + Direction.Y * tI);
                        z3 = (z2 + Direction.Z * tI);

                        nXp1 = (x3) / stepX;
                        nYp1 = (y3) / stepY;
                        nZp1 = (z3) / stepZ;

                        if ((nXp1 >= 0 && nXp1 < LX))
                            if (nYp1 >= 0 && nYp1 < LY)
                                if (nZp1 >= 0 && nZp1 < LZ)
                                {
                                    sum += mDataDouble[(int)nZp1, (int)nXp1, (int)nYp1];
                                }
                    }
                    PImage[tnXI, tnYI] = sum;
                }
            }

            return new PhysicalArray(PImage, -1 * LengthCorner, LengthCorner, -1 * LengthCorner, LengthCorner, true);
        }

        public PhysicalArray ProjectArray(Point3D Direction, Point3D FastScanDirection)
        {
            Direction.Normalize();
            if (mArrayRank == PhysicalArrayRank.Array2D)
                return ProjectArray2D(Direction, FastScanDirection);
            if (mArrayRank == PhysicalArrayRank.Array3D)
                return ProjectArray3D(Direction, FastScanDirection);
            return this;
        }


        private PhysicalArray ProjectArray2DInterpolate(Point3D Direction, Point3D FastScanDirection)
        {
            double LengthCorner = 0;
            double StepSize = double.MaxValue;
            for (int i = 1; i < 3; i++)
            {
                LengthCorner += mPhysicalStart[i] * mPhysicalStart[i];
                if (mPhysicalStep[i] < StepSize)
                    StepSize = mPhysicalStep[i];
            }
            LengthCorner = Math.Sqrt(LengthCorner);


            double[] PImage = new double[(int)(2 * LengthCorner / StepSize)];

            FastScanDirection.Normalize();

            Point3D FastScanAxis = FastScanDirection * StepSize;
            Point3D Origin = -1 * Direction * LengthCorner - FastScanDirection * LengthCorner;
            Direction.Normalize();
            Direction *= StepSize;

            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LOut = PImage.GetLength(0);

            double sX = mPhysicalStart[2];
            double sY = mPhysicalStart[1];

            double stepX = mPhysicalStep[2];
            double stepY = mPhysicalStep[1];

            double x1, y1;
            double x3, y3;


            double nXp1, nYp1;
            double sum;

            for (int tnXI = 0; tnXI < LOut; tnXI++)
            {
                x1 = Origin.X + FastScanAxis.X * tnXI;// -mPhysicalStart[2];
                y1 = Origin.Y + FastScanAxis.Y * tnXI;// -mPhysicalStart[1];

                sum = 0;
                for (int tI = 0; tI < LOut; tI++)
                {
                    x3 = (x1 + Direction.X * tI);
                    y3 = (y1 + Direction.Y * tI);

                    nXp1 = (x3 - mPhysicalStart[2]) / stepX;
                    nYp1 = (y3 - mPhysicalStart[1]) / stepY;
                    if ((nXp1 >= 0 && nXp1 < LX))
                        if (nYp1 >= 0 && nYp1 < LY)
                        {
                            sum += mDataDouble[0, (int)nYp1, (int)nXp1];
                        }
                }
                PImage[tnXI] = sum;

            }

            return new PhysicalArray(PImage, -1 * LengthCorner, LengthCorner);
        }

        private PhysicalArray ProjectArray3DInterpolate(Point3D Direction, Point3D FastScanDirection)
        {
            double LengthCorner = 0;
            double StepSize = double.MaxValue;
            for (int i = 1; i < 3; i++)
            {
                LengthCorner += mPhysicalStart[i] * mPhysicalStart[i];
                if (mPhysicalStep[i] < StepSize)
                    StepSize = mPhysicalStep[i];
            }
            LengthCorner = Math.Sqrt(LengthCorner) / 1.4;


            double[,] PImage = new double[(int)(2 * LengthCorner / StepSize), (int)(2 * LengthCorner / StepSize)];

            FastScanDirection.Normalize();

            Point3D FastScanAxis = FastScanDirection * StepSize;
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanAxis);
            Point3D Origin = -1 * Direction * LengthCorner - FastScanDirection * LengthCorner - SlowScanAxis / StepSize * LengthCorner;
            Direction.Normalize();
            Direction *= StepSize;

            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LZ = mDataDouble.GetLength(0);
            int LOut = PImage.GetLength(0);

            double sX = mPhysicalStart[2];
            double sY = mPhysicalStart[1];
            double sZ = mPhysicalStart[0];

            double stepX = mPhysicalStep[2];
            double stepY = mPhysicalStep[1];
            double stepZ = mPhysicalStep[0];

            double x1, y1, z1;
            double x2, y2, z2;
            double x3, y3, z3;

            double nXp1, nYp1, nZp1;
            double sum, Count;
            double uX, uY, Xu, Yu;
            int xL, yL;
            for (int tnXI = 0; tnXI < LOut; tnXI++)
            {
                x1 = Origin.X + FastScanAxis.X * tnXI - mPhysicalStart[2];
                y1 = Origin.Y + FastScanAxis.Y * tnXI - mPhysicalStart[1];
                z1 = Origin.Z + FastScanAxis.Z * tnXI - mPhysicalStart[0];

                for (int tnYI = 0; tnYI < LOut; tnYI++)
                {
                    x2 = x1 + SlowScanAxis.X * tnYI;
                    y2 = y1 + SlowScanAxis.Y * tnYI;
                    z2 = z1 + SlowScanAxis.Z * tnYI;
                    sum = 0;
                    Count = 0;
                    for (int tI = 0; tI < LOut; tI++)
                    {
                        x3 = (x2 + Direction.X * tI);
                        y3 = (y2 + Direction.Y * tI);
                        z3 = (z2 + Direction.Z * tI);

                        nXp1 = (x3) / stepX;
                        nYp1 = (y3) / stepY;
                        nZp1 = (z3) / stepZ;

                        if ((nXp1 >= 0 && nXp1 < LX))
                            if (nYp1 >= 0 && nYp1 < LY)
                                if (nZp1 >= 0 && nZp1 < LZ)
                                {
                                    try
                                    {
                                        if (nXp1 > LX - 1 || nYp1 > LY - 1)
                                        {
                                            sum += mDataDouble[(int)Math.Floor(nZp1), (int)Math.Floor(nXp1), (int)Math.Floor(nYp1)];
                                        }
                                        else
                                        {
                                            //bilinear interpolation
                                            xL = (int)Math.Floor(nXp1);
                                            yL = (int)Math.Floor(nYp1);
                                            uX = nXp1 - xL;
                                            uY = nYp1 - yL;
                                            Xu = (1 - uX);
                                            Yu = (1 - uY);

                                            sum += mDataDouble[(int)Math.Floor(nZp1), yL, xL] * Xu * Yu;
                                            sum += mDataDouble[(int)Math.Floor(nZp1), yL, xL + 1] * uX * Yu;
                                            sum += mDataDouble[(int)Math.Floor(nZp1), yL + 1, xL] * Xu * uY;
                                            sum += mDataDouble[(int)Math.Floor(nZp1), yL + 1, xL + 1] * uX * uY;
                                        }
                                    }
                                    catch { }
                                    // sum += mDataDouble[(int)nZp1, (int)nXp1, (int)nYp1];
                                    Count = 1;

                                }
                    }
                    if (Count > 0)
                        PImage[tnXI, tnYI] = sum / Count;
                    else
                        PImage[tnXI, tnYI] = sum;
                }
            }

            return new PhysicalArray(PImage, -1 * LengthCorner, LengthCorner, -1 * LengthCorner, LengthCorner, true);
        }

        public PhysicalArray ProjectArrayInterpolate(Point3D Direction, Point3D FastScanDirection)
        {
            Direction.Normalize();
            if (mArrayRank == PhysicalArrayRank.Array2D)
                return ProjectArray2DInterpolate(Direction, FastScanDirection);
            if (mArrayRank == PhysicalArrayRank.Array3D)
                return ProjectArray3DInterpolate(Direction, FastScanDirection);
            return this;
        }
        */

        public void UnSmearArray2DInterpolate(ref PhysicalArray UnPaintingArray, Point3D Direction, Point3D FastScanDirection)
        {
            Direction.Normalize();
            FastScanDirection.Normalize();
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LZ = mDataDouble.GetLength(0);

            int LOutX = UnPaintingArray.GetLength(Axis.XAxis) - 1;
            int LOutY = UnPaintingArray.GetLength(Axis.YAxis) - 1;

            double sX = mPhysicalStart[2];
            double sY = mPhysicalStart[1];
            double sZ = mPhysicalStart[0];

            double sOutX = UnPaintingArray.PhysicalStart(Axis.XAxis);
            double sOutY = UnPaintingArray.PhysicalStart(Axis.YAxis);

            double stepOutX = UnPaintingArray.PhysicalStep(Axis.XAxis);
            double stepOutY = UnPaintingArray.PhysicalStep(Axis.YAxis);

            double stepX = mPhysicalStep[2];
            double stepY = mPhysicalStep[1];
            double stepZ = mPhysicalStep[0];

            double z;
            double tXI, tYI, tX1, tY1;

            double aX, aY, bXY, bXX, bYY, bYX;
            double aZX, bZX, aZY, bZY;

            bXX = normalX.Y * stepY / stepOutX;
            bXY = normalX.X * stepX / stepOutX;

            bYX = normalY.Y * stepY / stepOutY;
            bYY = normalY.X * stepX / stepOutY;

            aZX = (normalX.Y * sY - sOutX + normalX.X * sX) / stepOutX;
            bZX = normalX.Z / stepOutX;

            aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY; ;
            bZY = normalY.Z / stepOutY;

            double uX, uY, sum, Xu, Yu;
            double[] Normalization = new double[UnPaintingArray.mDataDouble.GetLength(2)];
            ///this works by defining a plane normalX normalY that is placed under the screen.  Then each voxel in the cube is 
            ///projected by defining the vector R from the origin to the voxel.  the projection coordinates are then determined 
            ///by taking the dot product of the normals and R.
            unsafe
            {
                int zOffest = mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                int yOffset = mDataDouble.GetLength(2);
                double* POut = (double*)mipData;
                double* POutZ = POut;
                double* POutY;
                double* POutX;
                double VoxelVal;
                int xL, yL;
                for (int zI = 0; zI < LZ; zI++)
                {
                    z = (zI * stepZ + sZ);
                    aX = bZX * z + aZX;
                    aY = bZY * z + aZY;
                    tX1 = aX;
                    tY1 = aY;
                    POutY = POutZ;
                    for (int yI = 0; yI < LY; yI++)
                    {
                        tX1 += bXY;
                        tY1 += bYY;

                        tXI = tX1;
                        tYI = tY1;
                        POutX = POutY;
                        for (int xI = 0; xI < LX; xI++)
                        {
                            tXI += bXX;
                            tYI += bYX;

                            if (tXI > 0 && tXI < LOutX)
                                if (tYI > 0 && tYI < LOutY)
                                {

                                    xL = (int)tXI;
                                    yL = (int)tYI;
                                    uX = ((tXI - xL));
                                    uY = ((tYI - yL));
                                    Xu = (1 - uX);
                                    Yu = (1 - uY);

                                    VoxelVal = (*POutX);
                                    UnPaintingArray.mDataDouble[0, yL, xL] += VoxelVal * Xu * Yu;
                                    UnPaintingArray.mDataDouble[0, yL, xL + 1] += VoxelVal * uX * Yu;
                                    UnPaintingArray.mDataDouble[0, yL + 1, xL] += VoxelVal * Xu * uY;
                                    UnPaintingArray.mDataDouble[0, yL + 1, xL + 1] += VoxelVal * uX * uY;

                                    Normalization[xL] += Xu * Yu + Xu * uY;
                                    Normalization[xL + 1] += uX * Yu + uX * uY;
                                    //*POutX += sum;
                                }
                            POutX++;
                        }
                        POutY += yOffset;
                    }
                    POutZ += zOffest;
                }

                sum = 0;
                for (int i = 0; i < Normalization.GetLength(0); i++)
                    sum += Normalization[i];

                sum = sum / (double)Normalization.GetLength(0);
                sum = Math.Sqrt(sum);
                for (int i = 0; i < UnPaintingArray.mDataDouble.GetLength(2); i++)
                    for (int j = 0; j < UnPaintingArray.mDataDouble.GetLength(1); j++)
                    {
                        if (Normalization[i] != 0)
                            UnPaintingArray.mDataDouble[0, j, i] /= (Math.Sqrt(Normalization[i] * sum));
                    }
                /*
                for (int i = 0; i < UnPaintingArray.mDataDouble.GetLength(2); i++)
                    for (int j = 0; j < UnPaintingArray.mDataDouble.GetLength(1); j++)
                    {
                        if (Normalization[i] != 0)
                            UnPaintingArray.mDataDouble[0, j, i] /= Normalization[i];

                    }*/


            }
        }
        /*public void MIPProjection(ref PhysicalArray UnPaintingArray, Point3D Direction, Point3D FastScanDirection)
        {
            Direction.Normalize();
            FastScanDirection.Normalize();
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LZ = mDataDouble.GetLength(0);

            int LOutX = UnPaintingArray.GetLength(Axis.XAxis) - 1;
            int LOutY = UnPaintingArray.GetLength(Axis.YAxis) - 1;

            double sX = mPhysicalStart[2];
            double sY = mPhysicalStart[1];
            double sZ = mPhysicalStart[0];

            double sOutX = UnPaintingArray.PhysicalStart(Axis.XAxis);
            double sOutY = UnPaintingArray.PhysicalStart(Axis.YAxis);

            double stepOutX = UnPaintingArray.PhysicalStep(Axis.XAxis);
            double stepOutY = UnPaintingArray.PhysicalStep(Axis.YAxis);

            double stepX = mPhysicalStep[2];
            double stepY = mPhysicalStep[1];
            double stepZ = mPhysicalStep[0];

            double z;
            double tXI, tYI, tX1, tY1;

            double aX, aY, bXY, bXX, bYY, bYX;
            double aZX, bZX, aZY, bZY;

            bXX = normalX.Y * stepY / stepOutX;
            bXY = normalX.X * stepX / stepOutX;

            bYX = normalY.Y * stepY / stepOutY;
            bYY = normalY.X * stepX / stepOutY;

            aZX = (normalX.Y * sY - sOutX + normalX.X * sX) / stepOutX;
            bZX = normalX.Z / stepOutX;

            aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY; ;
            bZY = normalY.Z / stepOutY;

            double uX, uY, Xu, Yu;
            double[] Normalization = new double[UnPaintingArray.mDataDouble.GetLength(2)];
            double A, B, C, D;
            ///this works by defining a plane normalX normalY that is placed under the screen.  Then each voxel in the cube is 
            ///projected by defining the vector R from the origin to the voxel.  the projection coordinates are then determined 
            ///by taking the dot product of the normals and R.
            unsafe
            {
                int zOffest = mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                int yOffset = mDataDouble.GetLength(2);
                double* POut = (double*)mipData;
                double* POutZ = POut;
                double* POutY;
                double* POutX;
                double VoxelVal;
                int xL, yL;
                for (int zI = 0; zI < LZ; zI++)
                {
                    z = (zI * stepZ + sZ);
                    aX = bZX * z + aZX;
                    aY = bZY * z + aZY;
                    tX1 = aX;
                    tY1 = aY;
                    POutY = POutZ;
                    for (int yI = 0; yI < LY; yI++)
                    {
                        tX1 += bXY;
                        tY1 += bYY;

                        tXI = tX1;
                        tYI = tY1;
                        POutX = POutY;
                        for (int xI = 0; xI < LX; xI++)
                        {
                            tXI += bXX;
                            tYI += bYX;

                            if (tXI > 0 && tXI < LOutX)
                                if (tYI > 0 && tYI < LOutY)
                                {

                                    xL = (int)tXI;
                                    yL = (int)tYI;
                                    uX = (1 - (tXI - xL));
                                    uY = tYI - yL;
                                    Xu = (1 - uX);
                                    Yu = (1 - uY);

                                    VoxelVal = (*POutX);
                                    A = VoxelVal * Xu * Yu;
                                    B = VoxelVal * uX * Yu;
                                    C = VoxelVal * Xu * uY;
                                    D = VoxelVal * uX * uY;
                                    if (A > UnPaintingArray.mDataDouble[0, yL, xL])
                                        UnPaintingArray.mDataDouble[0, yL, xL] = A;

                                    if (B > UnPaintingArray.mDataDouble[0, yL, xL + 1])
                                        UnPaintingArray.mDataDouble[0, yL, xL + 1] = B;

                                    if (C > UnPaintingArray.mDataDouble[0, yL + 1, xL])
                                        UnPaintingArray.mDataDouble[0, yL + 1, xL] = C;

                                    if (D > UnPaintingArray.mDataDouble[0, yL + 1, xL + 1])
                                        UnPaintingArray.mDataDouble[0, yL + 1, xL + 1] = D;


                                }
                            POutX++;
                        }
                        POutY += yOffset;
                    }
                    POutZ += zOffest;
                }
            }
        }*/
        public PhysicalArray ProjectMIP(Point3D Direction, Point3D FastScanDirection)
        {
            double LengthCorner = 0;
            double StepSize = double.MaxValue;
            for (int i = 1; i < 3; i++)
            {
                LengthCorner += mPhysicalStart[i] * mPhysicalStart[i];
                if (mPhysicalStep[i] < StepSize)
                    StepSize = mPhysicalStep[i];
            }
            LengthCorner = Math.Sqrt(LengthCorner) / Math.Sqrt(2);


            double[,] PImage = new double[(int)(2 * LengthCorner / StepSize), (int)(2 * LengthCorner / StepSize)];

            FastScanDirection.Normalize();

            Point3D FastScanAxis = FastScanDirection * StepSize;
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanAxis);
            Point3D Origin = -1 * Direction * LengthCorner - FastScanDirection * LengthCorner - SlowScanAxis / StepSize * LengthCorner;
            Direction.Normalize();
            Direction *= StepSize;

            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LZ = mDataDouble.GetLength(0);
            int LOut = PImage.GetLength(0);

            double sX = mPhysicalStart[2];
            double sY = mPhysicalStart[1];
            double sZ = mPhysicalStart[0];

            double stepX = mPhysicalStep[2];
            double stepY = mPhysicalStep[1];
            double stepZ = mPhysicalStep[0];

            double x1, y1, z1;
            double x2, y2, z2;
            double x3, y3, z3;

            double nXp1, nYp1, nZp1;
            double sum;//, Count;
            double uX, uY, Xu, Yu;
            double val;
            int xL, yL;
            for (int tnXI = 0; tnXI < LOut; tnXI++)
            {
                x1 = Origin.X + FastScanAxis.X * tnXI - mPhysicalStart[2];
                y1 = Origin.Y + FastScanAxis.Y * tnXI - mPhysicalStart[1];
                z1 = Origin.Z + FastScanAxis.Z * tnXI - mPhysicalStart[0];
                double A;//, B, C, D;
                //for (int tnYI = 0; tnYI < LOut; tnYI++)
                for (int tnYI = 0; tnYI < LZ; tnYI++)
                {
                    x2 = x1 + SlowScanAxis.X * tnYI;
                    y2 = y1 + SlowScanAxis.Y * tnYI;
                    z2 = z1 + SlowScanAxis.Z * tnYI;
                    sum = 0;
                    // Count = 0;
                    for (int tI = 0; tI < LOut; tI++)
                    {
                        x3 = (x2 + Direction.X * tI);
                        y3 = (y2 + Direction.Y * tI);
                        z3 = (z2 + Direction.Z * tI);

                        nXp1 = (x3) / stepX;
                        nYp1 = (y3) / stepY;
                        //nZp1 = (z3) / stepZ;
                        nZp1 = tnYI;

                        if ((nXp1 >= 0 && nXp1 < LX))
                            if (nYp1 >= 0 && nYp1 < LY)
                                if (nZp1 >= 0 && nZp1 < LZ)
                                {
                                    if (nXp1 >= LX - 1 || nYp1 >= LY - 1)
                                    {
                                        val = mDataDouble[(int)nZp1, (int)nXp1, (int)nYp1];
                                    }
                                    else
                                    {
                                        //bilinear interpolation
                                        xL = (int)nXp1;
                                        yL = (int)nYp1;
                                        uX = 1 - (nXp1 - xL);
                                        uY = nYp1 - yL;
                                        Xu = (1 - uX);
                                        Yu = (1 - uY);

                                        A = mDataDouble[(int)nZp1, yL, xL];// *Xu * Yu;
                                        //             B = 0;// mDataDouble[(int)nZp1, yL, xL + 1] * uX * Yu;
                                        //           C = 0;// mDataDouble[(int)nZp1, yL + 1, xL] * Xu * uY;
                                        //         D = 0;// mDataDouble[(int)nZp1, yL + 1, xL + 1] * uX * uY;
                                        val = A;
                                        //if (B > val) val=B;
                                        //if (C > val) val = C;
                                        //if (D > val) val = D;

                                    }
                                    if (val > sum) sum = val;
                                }
                    }
                    PImage[tnXI, tnYI] = sum;
                }
            }

            return new PhysicalArray(PImage, -1 * LengthCorner, LengthCorner, -1 * LengthCorner, LengthCorner, true);
        }




        private object[] SafetyArray;
        private object SmearCriticalSection = new object();

        public void SmearArray(PhysicalArray PaintingArray, Point3D Direction, Point3D FastScanDirection)
        {
            if (mArrayRank == PhysicalArrayRank.Array2D)
                SmearArray1D(PaintingArray, Direction, FastScanDirection);
            else
                SmearArray2D(PaintingArray, Direction, FastScanDirection);
        }
        private void SmearArray2D(PhysicalArray PaintingArray, Point3D Direction, Point3D FastScanDirection)
        {
            // Queue<int> SmearArray2DQueue = null;
            lock (SmearCriticalSection)
            {
                if (SafetyArray == null || SafetyArray.Length != mCurrentArray.GetLength(0))
                {
                    SafetyArray = new object[mCurrentArray.GetLength(0)];
                    for (int i = 0; i < SafetyArray.Length; i++)
                        SafetyArray[i] = new object();
                }
            }

            //randomize the start of the z indexes to decrease blocking by the threads.
            //doesnt work because there needs to be one smearqueu per thread
            /*if (SmearArray2DQueue == null)
            {
                SmearArray2DQueue = new Queue<int>();
                int LLZ = mDataDouble.GetLength(0);
                int RandomStart = rnd.Next(LLZ);
                for (int i = 0; i < LLZ ; i++)
                    SmearArray2DQueue.Enqueue( ( i+ RandomStart ) % LLZ );
            }*/


            Direction.Normalize();
            FastScanDirection.Normalize();
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LZ = mDataDouble.GetLength(0);

            int LOutX = PaintingArray.GetLength(Axis.XAxis);
            int LOutY = PaintingArray.GetLength(Axis.YAxis);

            double sX = mPhysicalStart[2];
            double sY = mPhysicalStart[1];
            double sZ = mPhysicalStart[0];

            double sOutX = PaintingArray.PhysicalStart(Axis.XAxis);
            double sOutY = PaintingArray.PhysicalStart(Axis.YAxis);
            double stepOutX = PaintingArray.PhysicalStep(Axis.XAxis);
            double stepOutY = PaintingArray.PhysicalStep(Axis.YAxis);

            double stepX = mPhysicalStep[2];
            double stepY = mPhysicalStep[1];
            double stepZ = mPhysicalStep[0];

            double z;
            double tXI, tYI, tX1, tY1;

            double aX, aY, bXY, bXX, bYY, bYX;
            double aZX, bZX, aZY, bZY;

            bXX = normalX.Y * stepY / stepOutX;
            bXY = normalX.X * stepX / stepOutX;

            bYX = normalY.Y * stepY / stepOutY;
            bYY = normalY.X * stepX / stepOutY;

            aZX = (normalX.Y * sY - sOutX + normalX.X * sX) / stepOutX;
            bZX = normalX.Z / stepOutX;

            aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY; ;
            bZY = normalY.Z / stepOutY;


            double[] Normalization = new double[mDataDouble.GetLength(0)];

            unsafe
            {
                int zOffest = mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                int yOffset = mDataDouble.GetLength(2);
                double* POut = (double*)mipData;
                double* POutZ = POut;
                double* POutY;
                double* POutX;
                for (int zI = 0; zI < LZ; zI++)
                // while ( SmearArray2DQueue.Count>0)
                {
                    //int zI = SmearArray2DQueue.Dequeue();
                    lock (SafetyArray[zI])
                    {
                        z = (zI * stepZ + sZ);
                        aX = bZX * z + aZX;
                        aY = bZY * z + aZY;
                        tX1 = aX;
                        tY1 = aY;
                        POutY = POutZ;
                        for (int yI = 0; yI < LY; yI++)
                        {

                            tX1 += bXY;
                            tY1 += bYY;

                            tXI = tX1;
                            tYI = tY1;
                            POutX = POutY;
                            for (int xI = 0; xI < LX; xI++)
                            {
                                tXI += bXX;
                                tYI += bYX;

                                if (tXI > 0 && tXI < LOutX)
                                    if (tYI > 0 && tYI < LOutY)
                                    {
                                        *POutX += PaintingArray.mDataDouble[0, (int)tYI, (int)tXI];
                                    }
                                POutX++;
                            }
                            POutY += yOffset;

                        }
                        POutZ += zOffest;
                    }
                }
                //SmearArray2DQueue = null;
            }
        }
        private void SmearArray1D(PhysicalArray PaintingArray, Point3D Direction, Point3D FastScanDirection)
        {
            Direction.Normalize();
            FastScanDirection.Normalize();
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LZ = mDataDouble.GetLength(0);

            int LOutX = PaintingArray.GetLength(Axis.XAxis);
            int LOutY = PaintingArray.GetLength(Axis.YAxis);

            double sX = mPhysicalStart[2];
            double sY = mPhysicalStart[1];
            double sZ = mPhysicalStart[0];

            double sOutX = PaintingArray.PhysicalStart(Axis.XAxis);
            double sOutY = PaintingArray.PhysicalStart(Axis.YAxis);
            double stepOutX = PaintingArray.PhysicalStep(Axis.XAxis);
            double stepOutY = PaintingArray.PhysicalStep(Axis.YAxis);

            double stepX = mPhysicalStep[2];
            double stepY = mPhysicalStep[1];
            double stepZ = mPhysicalStep[0];

            double x, y;

            double tXI, tX1;

            for (int xI = 0; xI < LX; xI++)
            {
                x = (xI * stepX + sX);
                for (int yI = 0; yI < LY; yI++)
                {
                    y = (yI * stepY + sY);
                    tX1 = normalX.X * x + normalX.Y * y - sOutX;

                    tXI = (tX1) / stepOutX;

                    if (tXI > 0 && tXI < LOutX)
                    {
                        mDataDouble[0, yI, xI] += PaintingArray.mDataDouble[0, 0, (int)tXI];
                    }
                }
            }

        }


        public enum InterpolationMethod
        {
            Linear,
            Cosine,
            Cubic
        }

        public void SmearArrayInterpolate1D(PhysicalArray PaintingArray, Point3D Direction, Point3D FastScanDirection, InterpolationMethod interpolationMethod)
        {
            if (mArrayRank == PhysicalArrayRank.Array2D)
                SmearArray1DInterpolate(PaintingArray, Direction, FastScanDirection, interpolationMethod);
            else
                SmearArray2DInterpolate1D(PaintingArray, Direction, FastScanDirection, interpolationMethod);
        }

        Random rnd = new Random();
        private void SmearArray2DInterpolate1D(PhysicalArray PaintingArray, Point3D Direction, Point3D FastScanDirection, InterpolationMethod interpolationMethod)
        {
            // Queue<int> SmearIndexes=null;
            lock (SmearCriticalSection)
            {
                if (SafetyArray == null || SafetyArray.Length != mCurrentArray.GetLength(0))
                {
                    SafetyArray = new object[mCurrentArray.GetLength(0)];
                    for (int i = 0; i < SafetyArray.Length; i++)
                        SafetyArray[i] = new object();
                }
            }

            /*  if (SmearIndexes == null)
                 {
                     SmearIndexes = new Queue<int>();
                     int LLZ = mDataDouble.GetLength(0);
                     int RandomStart = rnd.Next(LLZ); 
                     for (int i = 0; i <LLZ ; i++)
                         SmearIndexes.Enqueue( (i+RandomStart) % LLZ  );
                    
                 }
             */

            Direction.Normalize();
            FastScanDirection.Normalize();
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LZ = mDataDouble.GetLength(0);

            int LOutX = PaintingArray.GetLength(Axis.XAxis);
            int LOutY = PaintingArray.GetLength(Axis.YAxis);

            double sX = mPhysicalStart[2];
            double sY = mPhysicalStart[1];
            double sZ = mPhysicalStart[0];

            double sOutX = PaintingArray.PhysicalStart(Axis.XAxis);
            double sOutY = PaintingArray.PhysicalStart(Axis.YAxis);
            double stepOutX = PaintingArray.PhysicalStep(Axis.XAxis);
            double stepOutY = PaintingArray.PhysicalStep(Axis.YAxis);

            double stepX = mPhysicalStep[2];
            double stepY = mPhysicalStep[1];
            double stepZ = mPhysicalStep[0];

            double z;
            double tXI, tYI, tX1, tY1;

            double aX, aY, bXY, bXX, bYY, bYX;
            double aZX, bZX, aZY, bZY;

            bXX = normalX.Y * stepY / stepOutX;
            bXY = normalX.X * stepX / stepOutX;

            bYX = normalY.Y * stepY / stepOutY;
            bYY = normalY.X * stepX / stepOutY;

            aZX = (normalX.Y * sY - sOutX + normalX.X * sX) / stepOutX;
            bZX = normalX.Z / stepOutX;

            aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY; ;
            bZY = normalY.Z / stepOutY;


            double[] Normalization = new double[mDataDouble.GetLength(0)];

            unsafe
            {
                int zOffest = mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                int yOffset = mDataDouble.GetLength(2);
                double* POut = (double*)mipData;
                double* POutZ = POut;
                double* POutY;
                double* POutX;
                double mu, mu2, y0, y1, y2, y3, a0, a1, a2, a3;
                int xLess, yLess;
                for (int zI = 0; zI < LZ; zI++)
                // while (SmearIndexes!=null && SmearIndexes.Count>0)
                {
                    // int zI = SmearIndexes.Dequeue();
                    lock (SafetyArray[zI])
                    {
                        z = (zI * stepZ + sZ);
                        aX = bZX * z + aZX;
                        aY = bZY * z + aZY;
                        tX1 = aX;
                        tY1 = aY;
                        POutY = POutZ;
                        for (int yI = 0; yI < LY; yI++)
                        {

                            tX1 += bXY;
                            tY1 += bYY;

                            tXI = tX1;
                            tYI = tY1;
                            POutX = POutY;
                            for (int xI = 0; xI < LX; xI++)
                            {
                                tXI += bXX;
                                tYI += bYX;

                                if (tXI > 0 && tXI < LOutX)
                                    if (tYI > 0 && tYI < LOutY)
                                    {
                                        xLess = (int)Math.Floor(tXI);
                                        yLess = (int)Math.Floor(tYI);
                                        mu = tXI - xLess;
                                        switch (interpolationMethod)
                                        {
                                            case InterpolationMethod.Linear:
                                                *POutX += PaintingArray.mDataDouble[0, yLess, xLess] * (mu) + PaintingArray.mDataDouble[0, yLess, xLess + 1] * (1 - mu);
                                                break;
                                            case InterpolationMethod.Cosine:
                                                mu2 = (1 - Math.Cos(mu * Math.PI)) / 2;
                                                *POutX += PaintingArray.mDataDouble[0, yLess, xLess] * (mu) + PaintingArray.mDataDouble[0, yLess, xLess + 1] * (1 - mu);
                                                break;
                                            case InterpolationMethod.Cubic:
                                                try
                                                {
                                                    y0 = PaintingArray.mDataDouble[0, yLess, xLess - 1];
                                                    y1 = PaintingArray.mDataDouble[0, yLess, xLess];
                                                    y2 = PaintingArray.mDataDouble[0, yLess, xLess + 1];
                                                    y3 = PaintingArray.mDataDouble[0, yLess, xLess + 2];

                                                    a0 = y3 - y2 - y0 + y1;
                                                    a1 = y0 - y1 - a0;
                                                    a2 = y2 - y0;
                                                    a3 = y1;

                                                    *POutX += a0 * mu * mu * mu + a1 * mu * mu + a2 * mu + a3;
                                                }
                                                catch
                                                {
                                                    try
                                                    {
                                                        if (xLess < 4)
                                                            *POutX += PaintingArray.mDataDouble[0, yLess, 0];
                                                        else
                                                            *POutX += PaintingArray.mDataDouble[0, yLess, mCurrentArray.GetLength(2) - 1];
                                                    }
                                                    catch { }
                                                }
                                                break;

                                        }
                                    }
                                POutX++;
                            }
                            POutY += yOffset;

                        }
                        POutZ += zOffest;
                    }
                }
                // SmearIndexes = null;
            }
        }

        public void SmearArrayInterpolate(PhysicalArray PaintingArray, Point3D Direction, Point3D FastScanDirection)
        {
            if (mArrayRank == PhysicalArrayRank.Array2D)
                SmearArray1DInterpolate(PaintingArray, Direction, FastScanDirection, InterpolationMethod.Linear);
            else
                SmearArray2DInterpolate2D(PaintingArray, Direction, FastScanDirection);
        }
        private void SmearArray2DInterpolate2D(PhysicalArray PaintingArray, Point3D Direction, Point3D FastScanDirection)
        {
            Direction.Normalize();
            FastScanDirection.Normalize();
            Point3D SlowScanAxis = Point3D.CrossProduct(Direction, FastScanDirection);

            Point3D normalX = new Point3D(FastScanDirection.X, FastScanDirection.Y, FastScanDirection.Z);
            Point3D normalY = new Point3D(SlowScanAxis.X, SlowScanAxis.Y, SlowScanAxis.Z);
            normalX.Normalize();
            normalY.Normalize();

            int LX = mDataDouble.GetLength(2);
            int LY = mDataDouble.GetLength(1);
            int LZ = mDataDouble.GetLength(0);

            int LOutX = PaintingArray.GetLength(Axis.XAxis) - 1;
            int LOutY = PaintingArray.GetLength(Axis.YAxis) - 1;

            double sX = mPhysicalStart[2];
            double sY = mPhysicalStart[1];
            double sZ = mPhysicalStart[0];

            double sOutX = PaintingArray.PhysicalStart(Axis.XAxis);
            double sOutY = PaintingArray.PhysicalStart(Axis.YAxis);
            double stepOutX = PaintingArray.PhysicalStep(Axis.XAxis);
            double stepOutY = PaintingArray.PhysicalStep(Axis.YAxis);

            double stepX = mPhysicalStep[2];
            double stepY = mPhysicalStep[1];
            double stepZ = mPhysicalStep[0];

            double z;
            double tXI, tYI, tX1, tY1;

            double aX, aY, bXY, bXX, bYY, bYX;
            double aZX, bZX, aZY, bZY;

            bXX = normalX.Y * stepY / stepOutX;
            bXY = normalX.X * stepX / stepOutX;

            bYX = normalY.Y * stepY / stepOutY;
            bYY = normalY.X * stepX / stepOutY;

            aZX = (normalX.Y * sY - sOutX + normalX.X * sX) / stepOutX;
            bZX = normalX.Z / stepOutX;

            aZY = (normalY.Y * sY - sOutY + normalY.X * sX) / stepOutY; ;
            bZY = normalY.Z / stepOutY;

            double uX, uY, sum, Xu, Yu;
            ///this works by defining a plane normalX normalY that is placed under the screen.  Then each voxel in the cube is 
            ///projected by defining the vector R from the origin to the voxel.  the projection coordinates are then determined 
            ///by taking the dot product of the normals and R.
            unsafe
            {
                int zOffest = mDataDouble.GetLength(2) * mDataDouble.GetLength(1);
                int yOffset = mDataDouble.GetLength(2);
                double* POut = (double*)mipData;
                double* POutZ = POut;
                double* POutY;
                double* POutX;
                int xL, yL;
                for (int zI = 0; zI < LZ; zI++)
                {
                    z = (zI * stepZ + sZ);
                    aX = bZX * z + aZX;
                    aY = bZY * z + aZY;
                    tX1 = aX;
                    tY1 = aY;
                    POutY = POutZ;
                    for (int yI = 0; yI < LY; yI++)
                    {
                        tX1 += bXY;
                        tY1 += bYY;

                        tXI = tX1;
                        tYI = tY1;
                        POutX = POutY;
                        for (int xI = 0; xI < LX; xI++)
                        {
                            tXI += bXX;
                            tYI += bYX;

                            if (tXI > 0 && tXI < LOutX)
                                if (tYI > 0 && tYI < LOutY)
                                {
                                    xL = (int)Math.Floor(tXI);
                                    yL = (int)Math.Floor(tYI);
                                    //todo: this may not be correct
                                    uX = ((tXI - xL));
                                    uY = ((tYI - yL));
                                    Xu = (1 - uX);
                                    Yu = (1 - uY);

                                    sum = PaintingArray.mDataDouble[0, yL, xL] * Xu * Yu;
                                    sum += PaintingArray.mDataDouble[0, yL, xL + 1] * uX * Yu;
                                    sum += PaintingArray.mDataDouble[0, yL + 1, xL] * Xu * uY;
                                    sum += PaintingArray.mDataDouble[0, yL + 1, xL + 1] * uX * uY;
                                    *POutX += sum;
                                }
                            POutX++;
                        }
                        POutY += yOffset;
                    }
                    POutZ += zOffest;

                }
            }


        }

        private void SmearArray1DInterpolate(PhysicalArray PaintingArray, Point3D Direction, Point3D FastScanDirection, InterpolationMethod interpolationMethod)
        {
            throw new Exception("Not yet implimented");


        }

        #endregion

        public void SaveData(string Filename)
        {
            PhysicalArray SliceData = this;
            double[, ,] Slice = (double[, ,])SliceData.ReferenceDataDouble;

             MathHelpLib.MathHelpsFileLoader.Save_Raw(Filename, Slice);

/*
            string extention = Path.GetExtension(Filename).ToLower();
            if (extention == ".cct")
            {
                #region RawFile
                FileStream BinaryFile = new FileStream(Filename, FileMode.Create, FileAccess.Write);
                BinaryWriter Writer = new BinaryWriter(BinaryFile);

                Writer.Write((Int32)SliceData.ArrayRank);

                Writer.Write((Int32)SliceData.GetLength(Axis.XAxis));
                Writer.Write((Int32)SliceData.GetLength(Axis.YAxis));
                Writer.Write((Int32)SliceData.GetLength(Axis.ZAxis));

                Writer.Write((double)SliceData.PhysicalStart(Axis.XAxis));
                Writer.Write((double)SliceData.PhysicalEnd(Axis.XAxis));

                Writer.Write((double)SliceData.PhysicalStart(Axis.YAxis));
                Writer.Write((double)SliceData.PhysicalEnd(Axis.YAxis));

                Writer.Write((double)SliceData.PhysicalStart(Axis.ZAxis));
                Writer.Write((double)SliceData.PhysicalEnd(Axis.ZAxis));

                for (int z = 0; z < SliceData.GetLength(Axis.ZAxis); z++)
                {
                    for (int y = 0; y < SliceData.GetLength(Axis.YAxis); y++)
                    {
                        for (int x = 0; x < SliceData.GetLength(Axis.XAxis); x++)
                        {
                            Writer.Write((double)SliceData[x, y, z]);
                        }
                    }
                }
                Writer.Close();
                BinaryFile.Close();
                #endregion
            }
            else if (extention == ".bmp" || extention == ".gif" ||
                extention == ".jpeg" || extention == ".png" || extention == ".tiff" || extention == ".tif" ||
                extention == ".jpg")
            {
                if (extention == ".png" )
                {
                       
                        double Min = double.MaxValue;
                        double Max = double.MinValue;
                        for (int z = 0; z < Slice.GetLength(0); z++)
                        {
                            for (int y = 0; y < Slice.GetLength(1); y++)
                            {
                                for (int x = 0; x < Slice.GetLength(2); x++)
                                {
                                    if (Slice[z, y, x] > Max) Max = Slice[z, y, x];
                                    if (Slice[z, y, x] < Min) Min = Slice[z, y, x];
                                }
                            }
                        }

                        double Scale = byte.MaxValue / (Max - Min);

                        string ppath = Path.GetDirectoryName(Filename);
                        string pFilename = Path.GetFileNameWithoutExtension(Filename);
                        for (int z = 0; z < Slice.GetLength(0); z++)
                        {
                            //FreeImageAPI.FreeImageBitmap fib = new FreeImageAPI.FreeImageBitmap(Slice.GetLength(0), Slice.GetLength(1), FreeImageAPI.FREE_IMAGE_TYPE.FIT_UINT32);
                            FreeImageAPI.FreeImageBitmap fib = new FreeImageAPI.FreeImageBitmap(new Bitmap(Slice.GetLength(0), Slice.GetLength(1), PixelFormat.Format32bppArgb), Slice.GetLength(0), Slice.GetLength(1));
                            unsafe
                            {
                                byte* pOutB;
                                for (int y = 0; y < Slice.GetLength(1); y++)
                                {
                                    UInt32* pOut = (UInt32*)((byte*)fib.Scan0 + fib.Stride * y);

                                    for (int x = 0; x < Slice.GetLength(0); x++)
                                    {
                                        pOutB = (byte*)pOut;
                                        float Gray = (float)(Scale * (Slice[z,y,x] - Min));
                                        if (Gray > 255) Gray = 255;
                                        if (Gray < 0) Gray = 0;
                                        byte GrayB =(byte) Gray;
                                        *pOutB = GrayB;
                                        *(pOutB+1) = GrayB;
                                        *(pOutB+2) = GrayB;
                                        pOut++;
                                    }
                                }
                            }
                            fib.Rotate(90);
                            fib.Save(ppath + "\\" + pFilename + z.ToString() + ".png");
                        }
                }
                else if (extention == ".tiff" || extention == ".tif")
                {
                    if (SliceData.ArrayRank == PhysicalArrayRank.Array2D)
                    {
                        double[,] Slice = (double[,])SliceData.ActualData2D;

                        FreeImageAPI.FreeImageBitmap fib = new FreeImageAPI.FreeImageBitmap(Slice.GetLength(0), Slice.GetLength(1), FreeImageAPI.FREE_IMAGE_TYPE.FIT_UINT16);
                        unsafe
                        {
                            for (int y = 0; y < Slice.GetLength(1); y++)
                            {
                                UInt16* pOut = (UInt16*)((byte*)fib.Scan0 + fib.Stride * y);

                                for (int x = 0; x < Slice.GetLength(0); x++)
                                {
                                    UInt16 Gray = (UInt16)Math.Truncate(ushort.MaxValue * (Slice[x, y] - 0));
                                    *pOut = Gray;
                                    pOut++;
                                }
                            }
                        }
                        fib.Rotate(90);
                        fib.Save(Filename, FreeImageAPI.FREE_IMAGE_FORMAT.FIF_TIFF);
                    }
                    else
                    {
                        double[, ,] Slice = (double[, ,])SliceData.ActualData3D;

                        double Min = double.MaxValue;
                        double Max = double.MinValue;
                        for (int z = 0; z < Slice.GetLength(0); z++)
                        {
                            for (int y = 0; y < Slice.GetLength(1); y++)
                            {
                                for (int x = 0; x < Slice.GetLength(2); x++)
                                {
                                    if (Slice[z, y, x] > Max) Max = Slice[z, y, x];
                                    if (Slice[z, y, x] < Min) Min = Slice[z, y, x];
                                }
                            }
                        }

                        double Scale = Int16.MaxValue / (Max - Min);

                        string ppath = Path.GetDirectoryName(Filename);
                        string pFilename = Path.GetFileNameWithoutExtension(Filename);
                        for (int z = 0; z < Slice.GetLength(0); z++)
                        {
                            double[,] SSlice = new double[Slice.GetLength(1), Slice.GetLength(2)];
                            for (int y = 0; y < Slice.GetLength(1); y++)
                            {
                                for (int x = 0; x < Slice.GetLength(2); x++)
                                {
                                    SSlice[y, x] = Slice[z, y, x];

                                }
                            }
                            MathHelpsFileLoader.Save_16bit_TIFF(ppath + "\\" + pFilename + z.ToString() + ".tif", SSlice, Scale, Min);


                        }


                    }
                    //MathHelpsFileLoader.Save_16bit_TIFF(Filename, Slice, ushort.MaxValue / 2, 0);
                }
                else
                {
                    Bitmap b = SliceData.MakeBitmap();
                    b.Save(Filename);
                }
            }
*/

        }
        public Bitmap[] ShowCross()
        {

            #region Save as Images

            double[, ,] Data = mDataDouble;

            int ZSlices = Data.GetLength(0);
            int XSlices = Data.GetLength(2);
            int YSlices = Data.GetLength(1);

            Bitmap[] Bitmaps = new Bitmap[3];

            {
                double[,] SliceZ = new double[XSlices, YSlices];
                for (int y = 0; y < YSlices; y++)
                {
                    for (int x = 0; x < XSlices; x++)
                    {
                        SliceZ[x, y] = Data[ZSlices / 2, x, y];
                    }
                }
                Bitmaps[0] = SliceZ.MakeBitmap();
            }

            {
                double[,] SliceY = new double[XSlices, ZSlices];
                for (int z = 0; z < ZSlices; z++)
                {
                    for (int x = 0; x < XSlices; x++)
                    {
                        SliceY[x, z] = Data[z, YSlices / 2, x];
                    }
                }
                Bitmaps[1] = SliceY.MakeBitmap();
            }

            {
                double[,] SliceX = new double[YSlices, ZSlices];
                for (int z = 0; z < ZSlices; z++)
                {
                    for (int y = 0; y < YSlices; y++)
                    {
                        SliceX[y, z] = Data[z, y, XSlices / 2];
                    }
                }
                Bitmaps[2] = SliceX.MakeBitmap();
            }
            #endregion
            return Bitmaps;
        }

        public void SaveCross(string Filename)
        {
            string Extension = Path.GetExtension(Filename).ToLower();
            if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                #region Save as Images

                double[, ,] Data = mDataDouble;

                int ZSlices = Data.GetLength(0);
                int XSlices = Data.GetLength(2);
                int YSlices = Data.GetLength(1);

                string outFile = Path.GetDirectoryName(Filename) + "\\" + Path.GetFileNameWithoutExtension(Filename);


                {
                    double[,] SliceZ = new double[XSlices, YSlices];

                    for (int y = 0; y < YSlices; y++)
                    {
                        for (int x = 0; x < XSlices; x++)
                        {
                            SliceZ[x, y] = Data[ZSlices / 2, x, y];
                        }
                    }

                    Bitmap b = SliceZ.MakeBitmap();
                    b.Save(outFile + "_Z" + Extension);
                }

                {
                    double[,] SliceY = new double[XSlices, ZSlices];
                    for (int z = 0; z < ZSlices; z++)
                    {
                        for (int x = 0; x < XSlices; x++)
                        {
                            SliceY[x, z] = Data[z, YSlices / 2, x];
                        }
                    }
                    Bitmap b = SliceY.MakeBitmap();
                    b.Save(outFile + "_Y" + Extension);
                }

                {
                    double[,] SliceX = new double[YSlices, ZSlices];
                    for (int z = 0; z < ZSlices; z++)
                    {
                        for (int y = 0; y < YSlices; y++)
                        {
                            SliceX[y, z] = Data[z, y, XSlices / 2];
                        }
                    }
                    Bitmap b = SliceX.MakeBitmap();
                    b.Save(outFile + "_X" + Extension);
                }
                #endregion
            }

        }


        public void CombineWithPhysicalArray(PhysicalArray pa, int dx, int dy, int dz)
        {
            double[, ,] OtherArray = pa.mDataDouble;

            int cx, cy, cz = dz;
            for (int z = 0; z < mDataDouble.GetLength(0); z++)
            {
                cy = dy;
                for (int y = 0; y < mDataDouble.GetLength(1); y++)
                {
                    cx = dx;
                    for (int x = 0; x < mDataDouble.GetLength(2); x++)
                    {
                        if (cx > 0 && cy > 0 && cz > 0)
                        {
                            if (cx < OtherArray.GetLength(2) && cy < OtherArray.GetLength(1) && cz < OtherArray.GetLength(0))
                            {

                                mDataDouble[z, y, x] += OtherArray[cz, cy, cx];
                            }
                        }
                        cx++;
                    }
                    cy++;
                }
                cz++;
            }


        }

        public void ShiftPhysicalArray(int dx, int dy, int dz)
        {
            double[, ,] OtherArray = new double[mDataDouble.GetLength(0), mDataDouble.GetLength(1), mDataDouble.GetLength(2)];
            Buffer.BlockCopy(mDataDouble, 0, OtherArray, 0, Buffer.ByteLength(mDataDouble));

            int cx, cy, cz = dz;
            for (int z = 0; z < mDataDouble.GetLength(0); z++)
            {
                cy = dy;
                for (int y = 0; y < mDataDouble.GetLength(1); y++)
                {
                    cx = dx;
                    for (int x = 0; x < mDataDouble.GetLength(2); x++)
                    {
                        if (cx > 0 && cy > 0 && cz > 0)
                        {
                            if (cx < OtherArray.GetLength(2) && cy < OtherArray.GetLength(1) && cz < OtherArray.GetLength(0))
                            {

                                mDataDouble[z, y, x] = OtherArray[cz, cy, cx];
                            }
                            else
                                mDataDouble[z, y, x] = 0;
                        }
                        else
                            mDataDouble[z, y, x] = 0;
                        cx++;
                    }
                    cy++;
                }
                cz++;
            }
        }

        public static PhysicalArray OpenDensityData(string Filename)
        {
            string Extension = Path.GetExtension(Filename).ToLower();
            if (Path.GetExtension(Filename).ToLower() == ".cct")
            {
                #region Open Raw
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);
                int sizeX, sizeY, sizeZ;

                PhysicalArrayRank ArrayRank = (PhysicalArrayRank)Reader.ReadInt32();

                sizeX = Reader.ReadInt32();
                sizeY = Reader.ReadInt32();
                sizeZ = Reader.ReadInt32();

                PhysicalArray mDensityGrid = new PhysicalArray(sizeX, sizeY, sizeZ,
                    Reader.ReadDouble(), Reader.ReadDouble(),
                    Reader.ReadDouble(), Reader.ReadDouble(),
                    Reader.ReadDouble(), Reader.ReadDouble()
                    );

                byte[] buffer = new byte[sizeX * sizeY * sizeZ * sizeof(double)];
                Reader.Read(buffer, 0, buffer.Length);

                mDensityGrid.CopyInDoubleArray(buffer);

                Reader.Close();
                BinaryFile.Close();

                return mDensityGrid;
                #endregion
            }
            else if (Path.GetExtension(Filename).ToLower() == ".dat")
            {
                #region Open Bin
                int DataType = 0;
                int sizeX, sizeY, sizeZ;

                sizeX = 0;
                sizeY = 0;
                sizeZ = 0;
                Dictionary<string, string> Tags = new Dictionary<string, string>();

                using (StreamReader sr = new StreamReader(Filename))
                {

                    String line;
                    string[] parts;
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Trim() != "" && line.Trim().StartsWith("#") == false)
                        {
                            parts = line.Replace("\t", "").Split(':');
                            Tags.Add(parts[0].Trim().ToLower(), parts[1]);
                        }
                    }
                    /*  ObjectFileName:	ProjectionObject.raw
                      Resolution:	220 220 220
                      SliceThickness:	1 1 1
                      Format:	FLOAT
                      ObjectModel:	I*/
                    parts = Tags["resolution"].Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    int.TryParse(parts[0], out sizeX);
                    int.TryParse(parts[1], out sizeY);
                    int.TryParse(parts[2], out sizeZ);

                    if (Tags["format"].ToLower() == "float")
                        DataType = 1;
                    if (Tags["format"].ToLower() == "double")
                        DataType = 2;
                    if (Tags["format"].ToLower() == "ushort")
                        DataType = 3;
                }

                Filename = Path.GetDirectoryName(Filename) + "\\" + Tags["objectfilename"];
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);


                PhysicalArray mDensityGrid = new PhysicalArray(sizeX, sizeY, sizeZ,
                    -1, 1, -1, 1, -1, 1
                    );


                if (DataType == 2)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int x = 0; x < sizeX; x++)
                            {
                                mDensityGrid[x, y, z] = Reader.ReadDouble();
                            }
                        }
                    }
                }
                else if (DataType == 1)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int x = 0; x < sizeX; x++)
                            {
                                mDensityGrid[x, y, z] = (double)Reader.ReadSingle();
                            }
                        }
                    }
                }
                else if (DataType == 3)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {
                            for (int x = 0; x < sizeX; x++)
                            {
                                mDensityGrid[x, y, z] = (double)Reader.ReadUInt16();
                            }
                        }
                    }
                }


                Reader.Close();
                BinaryFile.Close();
                return mDensityGrid;
                #endregion
            }
            else if (Path.GetExtension(Filename).ToLower() == ".raw")
            {
                #region Open Bin
                FileStream BinaryFile = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader Reader = new BinaryReader(BinaryFile);
                BinaryFile.Seek(0, SeekOrigin.Begin);

                long nPoints = (int)(BinaryFile.Length / 8d);
                nPoints = (long)Math.Pow((double)nPoints, (1d / 3d));

                int sizeX, sizeY, sizeZ;

                sizeX = (int)nPoints;
                sizeY = (int)nPoints;
                sizeZ = (int)nPoints;

                PhysicalArray mDensityGrid = new PhysicalArray(sizeX, sizeY, sizeZ,
                    -1, 1, -1, 1, -1, 1
                    );

                for (int z = 0; z < sizeZ; z++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[x, y, z] = Reader.ReadDouble();
                        }
                    }
                }
                Reader.Close();
                BinaryFile.Close();
                return mDensityGrid;
                #endregion
            }
            else if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                #region Open 2D image
                int sizeX, sizeY;

                Bitmap b = new Bitmap(Filename);
                sizeX = b.Width;
                sizeY = b.Height;
                PhysicalArray mDensityGrid = new PhysicalArray(sizeX, sizeY, -1, 1, -1, 1);


                double[,] Data = MathImageHelps.ConvertToDoubleArray(b, false);
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        mDensityGrid[x, y] = Data[x, y];
                    }
                }
                return mDensityGrid;
                #endregion
            }
            return null;
        }

        public static PhysicalArray OpenDensityData(string[] Filenames)
        {
            string Extension = Path.GetExtension(Filenames[0]).ToLower();
            if (Extension == ".bmp" || Extension == ".jpg" || Extension == ".png" || Extension == ".tif")
            {
                Filenames = MathStringHelps.SortNumberedFiles(Filenames);

                int sizeX, sizeY, sizeZ;

                Bitmap b = new Bitmap(Filenames[0]);
                sizeX = b.Width;
                sizeY = b.Height;
                sizeZ = Filenames.Length;
                PhysicalArray mDensityGrid = new PhysicalArray(sizeX, sizeY, sizeZ, -1, 1, -1, 1, -1, 1);

                for (int z = 0; z < sizeZ; z++)
                {
                    b = new Bitmap(Filenames[z]);
                    double[,] Data = MathImageHelps.ConvertToDoubleArray(b, false);
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            mDensityGrid[x, y, z] = Data[x, y];
                        }
                    }
                }
                return mDensityGrid;
            }
            else if (Extension == ".ivg")
            {
                Filenames = MathStringHelps.SortNumberedFiles(Filenames);

                int sizeX, sizeY, sizeZ;

                ImageHolder b = MathHelpLib.MathHelpsFileLoader.LoadIVGFile(Filenames[0]);
                sizeX = b.Width;
                sizeY = b.Height;
                sizeZ = Filenames.Length;
                PhysicalArray mDensityGrid = new PhysicalArray(sizeX, sizeY, sizeZ, -1, 1, -1, 1, -1, 1);

                double[, ,] ReferenceGrid = mDensityGrid.ReferenceDataDouble;
                for (int z = 0; z < sizeZ; z++)
                {
                    b = MathHelpLib.MathHelpsFileLoader.LoadIVGFile(Filenames[z]);
                    // double[,] Data = MathImageHelps. ConvertToDoubleArray(b, false);
                    for (int y = 0; y < sizeY; y++)
                    {
                        for (int x = 0; x < sizeX; x++)
                        {
                            ReferenceGrid[z, y, x] = b.ImageData[y, x, 0];// Data[y, x];
                        }
                    }
                }


                return mDensityGrid;
            }
            return null;
        }
    }
}
