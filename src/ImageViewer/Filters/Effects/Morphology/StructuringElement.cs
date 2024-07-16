using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageViewer.Filters.Effects.Morphology
{
    public class StructuringElement
    {
        public static  short[,] CreateSECircle(byte radius)
        {
            int  size = (int)((radius - 1) * 2 + 1);
            int pixelCounter = radius - 1;
            short[,] se = new short[size, size];
            // We fill array by ones
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    se[x, y] = 1;
                }
            }

            // We replaced unnecessary ones by zeros 
            for (int row = 0; row < pixelCounter; row++)
            {
                for (int column = 0; column < pixelCounter; column++)
                {
                    try
                    {
                        // upper left corner
                        se[column, row] = 0;
                        // upper right corner
                        se[(size - 1) - column, row] = 0;

                        // bottom right corner
                        se[(size - 1) - column, (size - 1) - row] = 0;

                        // bottom left corner
                        se[column, (size - 1) - row] = 0;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(ex.Message);
                    }
                }
                pixelCounter--;
            }
            return se;
        }
    }
}
