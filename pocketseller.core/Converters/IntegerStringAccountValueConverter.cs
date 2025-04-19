using System;
using System.Globalization;
using MvvmCross.Converters;
using pocketseller.core.Tools;

namespace pocketseller.core.Converters
{
    public class IntegerStringAccountValueConverter : MvxValueConverter<int, string>
    {
        protected override string Convert(int objValue, Type objTargetType, object objParameter, CultureInfo objCulture)
        {
            return objValue == 0 ? "" : objValue.ToString();
        }

        protected override int ConvertBack(string objValue, Type objTargetType, object objParameter, CultureInfo objCulture)
        {           
            return CParser.SafeParseInt(objValue);
        }
    }
}
