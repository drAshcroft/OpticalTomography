using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageViewer
{
    public class IronPythonConversions
    {
        public static double ConvertToDouble(object value)
        {
            return (double)value;
        }
        public static int ConvertToInt(object value)
        {
            return (int)value;
        }
        public static byte  ConvertToByte(object value)
        {
            return (Byte)value;
        }
        public static Type DoubleType()
        {
            return typeof(double);
        }
    }
}
