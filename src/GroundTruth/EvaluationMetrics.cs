using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundTruth
{
    class EvaluationMetrics
    {
        public double GroundTruthArea(double[, ,] GroundTruth)
        {
            double area = 0;
            for (int i = 0; i < GroundTruth.GetLength(0); i++)
            {
                for (int j = 0; j < GroundTruth.GetLength(1); j++)
                {
                    for (int k = 0; k < GroundTruth.GetLength(2); k++)
                    {
                        if ( GroundTruth[i, j, k] == 255)
                            area++;
                    }
                }
            }
            return area;
        }

        public double segVolumeArea(double[, ,] segVolume)
        {
            double area = 0;
            for (int i = 0; i < segVolume.GetLength(0); i++)
            {
                for (int j = 0; j < segVolume.GetLength(1); j++)
                {
                    for (int k = 0; k < segVolume.GetLength(2); k++)
                    {
                        if (segVolume[i, j, k] == 255)
                            area++;

                    }
                }
            }
            return area;
        }

        public double TruePositive (double [,,] GroundTruth, double [,,] segVolume)
        {

            double TruePositive = 0;
            for (int i = 0; i < segVolume.GetLength(0); i++)
            {
                for (int j = 0; j < segVolume.GetLength(1); j++)
                {
                    for (int k = 0; k < segVolume.GetLength(2); k++)
                    {
                        if (segVolume[i, j, k] == 255 && GroundTruth[i, j, k] == 255)
                            TruePositive++;
                    
                    }
                }
            }
        return TruePositive;
        }

        public double FalsePositive (double [,,] GroundTruth, double [,,] segVolume)
        {

            double FalsePositive = 0;
            for (int i = 0; i < segVolume.GetLength(0); i++)
            {
                for (int j = 0; j < segVolume.GetLength(1); j++)
                {
                    for (int k = 0; k < segVolume.GetLength(2); k++)
                    {
                        if (segVolume[i, j, k] == 255 && GroundTruth[i, j, k] == 0)
                            FalsePositive++;
                    
                    }
                }
            }
        return FalsePositive;
        }
        public double FalseNegative(double[, ,] GroundTruth, double[, ,] segVolume)
        {

            double FalseNegative = 0;
            for (int i = 0; i < segVolume.GetLength(0); i++)
            {
                for (int j = 0; j < segVolume.GetLength(1); j++)
                {
                    for (int k = 0; k < segVolume.GetLength(2); k++)
                    {
                        if (segVolume[i, j, k] == 0 && GroundTruth[i, j, k] == 255)
                            FalseNegative++;

                    }
                }
            }
            return FalseNegative;
        }

        public double TrueNegative(double[, ,] GroundTruth, double[, ,] segVolume)
        {

            double TrueNegative = 0;
            for (int i = 0; i < segVolume.GetLength(0); i++)
            {
                for (int j = 0; j < segVolume.GetLength(1); j++)
                {
                    for (int k = 0; k < segVolume.GetLength(2); k++)
                    {
                        if (segVolume[i, j, k] == 0 && GroundTruth[i, j, k] == 0)
                            TrueNegative++;

                    }
                }
            }
            return TrueNegative;
        }

        public double Sensitivity(double[, ,] GroundTruth, double[, ,] segVolume)
        {
            double TP = TruePositive(GroundTruth, segVolume);
            double groundtruth = GroundTruthArea(GroundTruth);
            return TP / groundtruth;
        
        }

        public double Specificity(double[, ,] GroundTruth, double[, ,] segVolume)
        {
            double TN = TrueNegative(GroundTruth, segVolume);
            double groundtruth = GroundTruthArea(GroundTruth);
            
            return TN / (GroundTruth.GetLength(0)*GroundTruth.GetLength(1)*GroundTruth.GetLength(1) - groundtruth);

        }

        public double JaccardSimilarityMetric(double[, ,] GroundTruth, double[, ,] segVolume)
        {
            double TP = TruePositive(GroundTruth, segVolume);
            double FP = FalsePositive(GroundTruth, segVolume);
            double FN = FalseNegative(GroundTruth, segVolume);
            

            return TP / (TP + FP + FN);

        }

        public double DiceCoefficient(double[, ,] GroundTruth, double[, ,] segVolume)
        {
            double TP = TruePositive(GroundTruth, segVolume);
            double groundtruth = GroundTruthArea(GroundTruth);
            double segvolume = segVolumeArea(GroundTruth);


            return TP / ((groundtruth + segvolume)/2);

        }
    }

}
