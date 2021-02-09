using System;
using System.Globalization;
using MvvmCross.Converters;
using pocketseller.core.Tools;

namespace pocketseller.core.Converters
{
    public class DecimalStringAccountValueConverter : MvxValueConverter<decimal, string>
    {
        protected override string Convert(decimal objValue, Type objTargetType, object objParameter, CultureInfo objCulture)
        {
            return objValue == 0 ? "" : objValue.ToString("F").Replace(",", ".");
        }

        protected override decimal ConvertBack(string objValue, Type objTargetType, object objParameter, CultureInfo objCulture)
        {           
            return CParser.SafeParseDecimal(objValue);
        }
    }
}
