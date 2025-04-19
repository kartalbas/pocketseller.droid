using System;
using System.Globalization;
using MvvmCross.Converters;
using pocketseller.core.Tools;

namespace pocketseller.core.Converters
{
    public class DoubleStringValueConverter : MvxValueConverter<double, string>
    {
        protected override string Convert(double objValue, Type objTargetType, object objParameter, CultureInfo objCulture)
        {
            return objValue.ToString("F").Replace(",", ".");
        }

        protected override double ConvertBack(string objValue, Type objTargetType, object objParameter, CultureInfo objCulture)
        {           
            return CParser.SafeParseDouble(objValue);
        }
    }
}
