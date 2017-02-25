using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class IEEE754Utility
    {
        public static string FloatToHexString(float f)
        {
            try
            {
                return BitConverter.ToUInt32(BitConverter.GetBytes(f), 0).ToString("X");
            }
            catch
            {
                throw;
            }
        }

        public static float HexStringToFloat(string str)
        {
            try
            {
                return BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToUInt32(str, 16)), 0);
            }
            catch
            {
                throw;
            }
        }

        public static string DoubleToHexString(double d)
        {
            try
            {
                return BitConverter.ToUInt64(BitConverter.GetBytes(d), 0).ToString("X");
            }
            catch
            {
                throw;
            }
        }

        public static double HexStringToDouble(string str)
        {
            try
            {
                return BitConverter.ToDouble(BitConverter.GetBytes(Convert.ToUInt64(str, 16)), 0);
            }
            catch 
            {
                throw;
            }
        }
    }
}
