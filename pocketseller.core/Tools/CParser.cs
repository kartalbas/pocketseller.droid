using System;
using System.Globalization;

namespace pocketseller.core.Tools
{
    public class CParser
    {
        //public static byte[] StringToByteArray(string str)
        //{
        //    System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        //    return enc.GetBytes(str);
        //}

        //public static string ByteArrayToString(byte[] arr)
        //{
        //    System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        //    return enc.GetString(arr);
        //}
        
        public static int SafeParseInt(string value)
        {
            try
            {
                int ret = 0;

                if (!Int32.TryParse(value, out ret))
                {
                    ret = 0;
                }
                return ret;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static double SafeParseDouble(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return 0;
                }
                return Convert.ToDouble(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static float SafeParseFloat(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return 0;
                }
                return float.Parse(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static decimal SafeParseDecimal(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return 0;
                }
                return decimal.Parse(value, NumberFormatInfo.InvariantInfo);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static Single SafeParseSingle(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return 0;
                }
                return Single.Parse(value, NumberFormatInfo.InvariantInfo);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
